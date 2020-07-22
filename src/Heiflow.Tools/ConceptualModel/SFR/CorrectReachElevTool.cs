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

namespace Heiflow.Tools.ConceptualModel
{
    public enum CorrectionMethod { ModifyBottom, Offset2Bottom }
    public class CorrectReachElevTool : MapLayerRequiredTool
    {
        public CorrectReachElevTool()
        {
            Name = "Correct Reach Bed Elevation";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Correct reach bed elevation to make sure that the bed elevation is greater than the bottom elevatin of top layer";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            Offset = 5;
            CorrectionMethod = CorrectionMethod.ModifyBottom;
        }
        [Category("Parameter")]
        [Description("The offset applied to the original value.")]
        public float Offset
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("The offset applied to the original value. If ModifyBottom is selected, Offset is not used.")]
        public CorrectionMethod CorrectionMethod
        {
            get;
            set;
        }

        public override void Initialize()
        {
            Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
            var dis = prj.Project.Model.GetPackage(DISPackage.PackageName) as DISPackage;
            if (sfr != null)
            {
                var rvnet = sfr.RiverNetwork;
                int count = 0;
                string msg = "";
                int i = 0;
                if (CorrectionMethod == CorrectionMethod.Offset2Bottom)
                {
                    foreach (var river in rvnet.Rivers)
                    {
                        foreach (Reach reach in river.Reaches)
                        {
                            var index = grid.Topology.GetSerialIndex(reach.IRCH - 1, reach.JRCH - 1);
                            if ((reach.TopElevation - reach.BedThick) < dis.Elevation[1, 0, index])
                            {
                                reach.TopElevation = dis.Elevation[1, 0, index] + Offset + reach.BedThick;
                                msg = string.Format("The reach at {0},{1} is modified.", river.ID, reach.ID);
                                cancelProgressHandler.Progress("tool", i / rvnet.RiverCount * 100, msg);
                                count++;
                            }
                        }
                        i++;
                    }
                }
                else if (CorrectionMethod == CorrectionMethod.ModifyBottom)
                {
                    var thickness = new float[grid.ActualLayerCount];
                    foreach (var river in rvnet.Rivers)
                    {
                        foreach (Reach reach in river.Reaches)
                        {
                            var index = grid.Topology.GetSerialIndex(reach.IRCH - 1, reach.JRCH - 1);
                            if ((reach.TopElevation - reach.BedThick) < dis.Elevation[1, 0, index])
                            {
                                for (int j = 0; j < grid.ActualLayerCount;j++ )
                                {
                                    thickness[j] = dis.Elevation[j, 0, index] - dis.Elevation[j + 1, 0, index];
                                }
                                dis.Elevation[1, 0, index] = (float)(reach.TopElevation - reach.BedThick - 1);
                                for (int j = 2; j < grid.LayerCount; j++)
                                {
                                    dis.Elevation[j, 0, index] = dis.Elevation[j - 1, 0, index] - thickness[j - 1];
                                }
                                msg = string.Format("The bottom elevations at {0},{1} are modified.", river.ID, reach.ID);
                                cancelProgressHandler.Progress("tool", i / rvnet.RiverCount * 100, msg);
                                count++;
                            }
                        }
                        i++;
                    }
                }
                msg = string.Format("Total number of modified reaches is: {0}", count);
                cancelProgressHandler.Progress("tool", 100, msg);
                sfr.NetworkToMat();
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 90, "SFR package not loaded.");
                return false;
            }

        }
    }
}
