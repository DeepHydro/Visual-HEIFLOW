//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using DotSpatial.Data;
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Core.Data;
using Heiflow.Core.Hydrology;
using Heiflow.Core.IO;
using Heiflow.Core.MyMath;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.ConceptualModel
{
    public class SmoothElev : MapLayerRequiredTool
    {
        public SmoothElev()
        {
            Name = "Smooth Reach Bed Elevation";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Smooth stream bed elevation based on Slope value";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            DefaultOutfallElevation = 0;
            //MaxOffsetToGroundElev = 10;
            MinOffsetToGroundElev = 5;
            //GroundElevOffsetToStream = 5;
            StreamBedOffsetToBtmElev = 10;
        }
        [Category("Parameter")]
        [Description("The default elevation of outfalls")]
        public double DefaultOutfallElevation
        {
            get;
            set;
        }
        //[Category("Parameter")]
        //[Description("The maximum offset to ground elevation. It is used to determin the minimum elevation of stream bed")]
        //public double MaxOffsetToGroundElev
        //{
        //    get;
        //    set;
        //}
        [Category("Parameter")]
        [Description("The minimum offset to ground elevation. It is used to determin the maximum elevation of stream bed")]
        public double MinOffsetToGroundElev
        {
            get;
            set;
        }
        //[Category("Parameter")]
        //[Description("The ground elevation offset to stream bed")]
        //public double GroundElevOffsetToStream
        //{
        //    get;
        //    set;
        //}
        [Category("Parameter")]
        [Description("The stream bed elevation offset to the bottom elevation of top layer")]
        public double StreamBedOffsetToBtmElev
        {
            get;
            set;
        }
        public override void Initialize()
        {
            this.Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
            var dis = prj.Project.Model.GetPackage(DISPackage.PackageName) as DISPackage;
            if (sfr != null)
            {         
                var net = sfr.RiverNetwork;
                string msg = string.Format("Total number of outfalls is {0}", net.Outfalls.Count);
                int progress = 0;
                int i = 1;
                foreach (var outfall in net.Outfalls)
                {
                    msg = string.Format("Processing the Outfall {0}", outfall.ID);
                    ModifyRiverElev(DefaultOutfallElevation, outfall.RiverObject, grid,dis);
                    progress = i / net.Outfalls.Count;
                    cancelProgressHandler.Progress("Package_Tool", progress, msg);
                    i++;
                }
                sfr.NetworkToMat();
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 90, "SFR package not loaded.");
                return false;
            }
        }

        private void ModifyRiverElev(double outlet_elev, River river, MFGrid grid, DISPackage dis)
        {
            river.OutletNode.Elevation = outlet_elev;
            river.LastReach.OutletNode.Elevation = outlet_elev;
            river.LastReach.TopElevation = outlet_elev;
            int nrch = river.Reaches.Count;
            for (int i = nrch - 2; i >= 0; i--)
            {
                var reach = river.Reaches[i];
                reach.TopElevation = river.Reaches[i + 1].TopElevation + reach.Length * reach.Slope;
                var index = grid.Topology.GetSerialIndex(reach.IRCH - 1, reach.JRCH - 1);
                if (reach.TopElevation < dis.Elevation[1, 0, index])
                {
                    reach.TopElevation = dis.Elevation[1, 0, index] + StreamBedOffsetToBtmElev;
                }
                else if (reach.TopElevation > dis.Elevation[0, 0, index])
                {
                    //var delta = (float)reach.TopElevation - dis.Elevation[0, 0, index];
                    //dis.Elevation[0, 0, index] += delta;
                    if (i > 1 && i < nrch - 1)
                    {
                        var rch1 = river.Reaches[i + 1];
                        var rch2 = river.Reaches[i - 1];
                        var avelev = System.Math.Min(System.Math.Min(reach.TopElevation, rch1.TopElevation), rch2.TopElevation);
                        reach.TopElevation = avelev - MinOffsetToGroundElev;
                    }
                    else
                    {
                        reach.TopElevation = dis.Elevation[0, 0, index] - MinOffsetToGroundElev;
                    }
                }
                //var minelev = dis.Elevation[0, 0, index] - MaxOffsetToGroundElev;
                //var maxelev = dis.Elevation[0, 0, index] - MinOffsetToGroundElev;
                //if (reach.TopElevation < minelev)
                //    reach.TopElevation = minelev;
                //if (reach.TopElevation > maxelev)
                //    reach.TopElevation = maxelev;
            }
            river.InletNode.Elevation = river.FirstReach.TopElevation;
            if(river.Upstreams.Count > 0)
            {
                foreach(var upriver in river.Upstreams)
                {
                    ModifyRiverElev(river.InletNode.Elevation, upriver, grid, dis);
                }
            }
        }
    }
}
