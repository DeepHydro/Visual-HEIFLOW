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
    public class SetSegmentByCSV : MapLayerRequiredTool
    {
        public SetSegmentByCSV()
        {
            Name = "Set Segment Parameters By CSV File";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Set Segment Parameters by CSV File";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }

        [Category("Input")]
        [Description("The  input csv data filename. The csv file is generated either by exporting from the DataView or by exporting from the Profile in the SFR Window.")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string CSVFileName
        {
            get;
            set;
        }

        public override void Initialize()
        {
            this.Initialized = TypeConverterEx.IsNotNull(CSVFileName);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
            if (sfr != null)
            {
                CSVFileStream csv = new CSVFileStream(CSVFileName);
                var dt = csv.Load();
                var rvnet = sfr.RiverNetwork;
                int count = 0;
                var msg = string.Format("CSV file is loaded. Total rows: {0}", dt.Rows.Count);
                cancelProgressHandler.Progress("Package_Tool", 10, msg);
                WorkspaceView.OutputTo(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    var iseg = 0;
                    int.TryParse(dr["NSEG"].ToString(), out iseg);
                    var river = rvnet.GetRiver(iseg);
                    if(river != null)
                    {
                        river.UpRiverID = int.Parse(dr["IUPSEG"].ToString());
                        river.Flow = double.Parse(dr["FLOW"].ToString());
                        river.Runoff = double.Parse(dr["RUNOFF"].ToString());
                        river.ETSW = double.Parse(dr["ETSW"].ToString());
                        river.PPTSW = double.Parse(dr["PPTSW"].ToString());
                        river.ROUGHCH = double.Parse(dr["ROUGHCH"].ToString());
                        river.Width = double.Parse(dr["WIDTH1"].ToString());
                        river.Width1 = double.Parse(dr["WIDTH1"].ToString());
                        river.Width2 = double.Parse(dr["WIDTH2"].ToString());
                        river.IPrior = int.Parse(dr["IPRIOR"].ToString());
                        foreach(var rch in river.Reaches)
                        {
                            rch.Width = river.Width;
                            rch.Width1 = river.Width;
                            rch.Width2 = river.Width;
                        }
                        count++;
                    }
                }
                msg = string.Format("{0} segments are modified", count);
                sfr.NetworkToMat();
                cancelProgressHandler.Progress("Package_Tool", 90, msg);
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
