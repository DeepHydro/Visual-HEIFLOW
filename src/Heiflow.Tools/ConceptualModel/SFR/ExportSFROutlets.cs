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
    public class ExportSFROutlets : MapLayerRequiredTool
    {
        public ExportSFROutlets()
        {
            Name = "Export SFR As SWMM";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Export SFR As SWMM Input File";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }

        [Category("Input")]
        [Description("The ID of outlets")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public int OutletID
        {
            get;
            set;
        }
     

        [Category("Output")]
        [Description("The SWMM inp filename")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OutputFileName
        {
            get;
            set;
        }

        public override void Initialize()
        {
            this.Initialized = true;
            this.Initialized = TypeConverterEx.IsNotNull(OutputFileName);
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
                StreamWriter sw = new StreamWriter(OutputFileName);
                string line = "Cell_ID,Row,Column";
                sw.WriteLine(line);
                foreach(var riv in net.Rivers)
                {
                    if(riv.OutRiverID == OutletID)
                    {
                        var cellid = grid.Topology.GetID(riv.LastReach.IRCH - 1, riv.LastReach.JRCH - 1);
                        line = string.Format("{0},{1},{2}", cellid, riv.LastReach.IRCH, riv.LastReach.JRCH);
                        sw.WriteLine(line);
                    }
                }
                sw.Close();
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
