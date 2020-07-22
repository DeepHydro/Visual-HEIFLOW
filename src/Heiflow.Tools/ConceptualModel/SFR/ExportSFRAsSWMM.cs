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
    public class ExportSFRAsSWMM : MapLayerRequiredTool
    {
        private IMapLayerDescriptor _IntersectionLayerDescriptor;
        private IFeatureSet _intersection_layer;
        public ExportSFRAsSWMM()
        {
            Name = "Export SFR As SWMM";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Export SFR As SWMM Input File";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            RiverIDField = "RiverID";
            RowField = "ROW";
            ColumnField = "COLUMN";
            MultiThreadRequired = true;
        }
        [Category("GIS Layer")]
        [Description("Junction layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GirdReachIntersectionLayer
        {
            get
            {
                return _IntersectionLayerDescriptor;
            }
            set
            {
                _IntersectionLayerDescriptor = value;
                if (_IntersectionLayerDescriptor != null)
                {
                    _intersection_layer = _IntersectionLayerDescriptor.DataSet as IFeatureSet;
                    if (_intersection_layer != null)
                    {
                        var buf = from DataColumn dc in _intersection_layer.DataTable.Columns select dc.ColumnName;
                        Fields = buf.ToArray();
                    }
                }
            }
        }

        #region Field Binding
        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }
        [Category("Field Binding")]
        [Description("The river ID field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string RiverIDField
        {
            get;
            set;
        }
        [Category("Field Binding")]
        [Description("The row field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string RowField
        {
            get;
            set;
        }
        [Category("Field Binding")]
        [Description("The column field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string ColumnField
        {
            get;
            set;
        }
        #endregion

        [Category("Output")]
        [Description("The SWMM inp filename")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DataFileName
        {
            get;
            set;
        }

        public override void Initialize()
        {
            this.Initialized = true;
            this.Initialized = TypeConverterEx.IsNotNull(DataFileName);
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
                ReachesToSWMM(DataFileName, net.Rivers, cancelProgressHandler);
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 90, "SFR package not loaded.");
                return false;
            }
        }
        private void WriteDefaultSWMMSections(StreamWriter sw)
        {
            sw.WriteLine(Resource.SWMMSections);
        }
        public void ReachesToSWMM(string filename, List<River> rivers, DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            StreamWriter sw = new StreamWriter(filename);
            WriteDefaultSWMMSections(sw);
            List<HydroPoint> outfalls = new List<HydroPoint>();
            List<HydroPoint> junctions = new List<HydroPoint>();
            double progress = 0;
            int i = 1;
            foreach (var river in rivers)
            {
                var msg = string.Format("Processing the river {0}", river.ID);
                progress = i / rivers.Count * 100.0;
                cancelProgressHandler.Progress("Package_Tool", (int)progress, msg);

                foreach (var rch in river.Reaches)
                {
                    if (junctions.Where(t => t.ID == rch.InletNode.ID).Count() == 0)
                    {
                        var rchrow = (from IFeature dr in _intersection_layer.Features
                                      where int.Parse(dr.DataRow[RiverIDField].ToString()) == river.ID && int.Parse(dr.DataRow[RowField].ToString()) == rch.IRCH &&
                                          int.Parse(dr.DataRow[ColumnField].ToString()) == rch.JRCH
                                      select dr).First();
                        //rch.InletNode.Elevation = rch.TopElevation;
                        rch.InletNode.Coordinate = rchrow.Geometry.Coordinate;
                        junctions.Add(rch.InletNode);
                    }
                    if (junctions.Where(t => t.ID == rch.OutletNode.ID).Count() == 0)
                    {
                        var rchrow = (from IFeature dr in _intersection_layer.Features
                                      where int.Parse(dr.DataRow[RiverIDField].ToString()) == river.ID && int.Parse(dr.DataRow[RowField].ToString()) == rch.IRCH &&
                                          int.Parse(dr.DataRow[ColumnField].ToString()) == rch.JRCH
                                      select dr).First();
                        //rch.OutletNode.Elevation = rch.TopElevation;
                        rch.OutletNode.Coordinate = rchrow.Geometry.Coordinate;
                        junctions.Add(rch.OutletNode);
                    }
                }
                i++;
            }

            foreach (var river in rivers)
            {
                if (river.Downstream == null)
                {
                    var rchrow = (from IFeature dr in _intersection_layer.Features
                                  where int.Parse(dr.DataRow[RiverIDField].ToString()) == river.ID
                                  select dr).Last();
                    river.OutletNode.Coordinate = rchrow.Geometry.Coordinate;
                    outfalls.Add(river.OutletNode);
                    if (junctions.Where(t => t.ID == river.OutletNode.ID).Count() == 1)
                    {
                        junctions.Remove(river.OutletNode);
                    }
                }
            }

            string line = "[JUNCTIONS]\n;;               Invert     Max.       Init.      Surcharge  Ponded    \n;;Name           Elev.      Depth      Depth      Depth      Area";
            sw.WriteLine(line);
            line = ";;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);

            foreach (var junc in junctions)
            {
                if (junc != null)
                {
                    line = string.Format("{0}\t{1}\t{2}", junc.ID, junc.Elevation.ToString("0.0000"), " 15.0\t0 .00\t0\t0");
                    sw.WriteLine(line);
                }
            }

            line = "[OUTFALLS]\n;;               Invert     Outfall    Stage/Table      Tide   \n;;Name           Elev.      Type       Time Series      Gate";
            sw.WriteLine(line);
            line = ";;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);
            foreach (var junc in outfalls)
            {
                if (junc != null)
                {
                    line = junc.ID + " " + junc.Elevation.ToString("0.0000") + "     FREE                        NO    ";
                    sw.WriteLine(line);
                }
            }

            line = "[CONDUITS]\n;;               Inlet            Outlet                      Manning    Inlet      Outlet     Init.      Max. ";
            sw.WriteLine(line);
            line = ";;Name           Node             Node             Length     N          Offset     Offset     Flow       Flow      ";
            sw.WriteLine(line);
            line = ";;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);
            foreach (var r in rivers)
            {
                foreach (var rch in r.Reaches)
                {
                    line = rch.ID + " " + rch.InletNode.ID + " " + rch.OutletNode.ID + " " + rch.Length.ToString("0.000") + " " + rch.ROUGHCH + "  0      0        0          0    ";
                    sw.WriteLine(line);
                }
            }

            line = "[XSECTIONS]\n;;Link           Shape        Geom1            Geom2      Geom3      Geom4      Barrels";
            sw.WriteLine(line);
            line = ";;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);
            foreach (var r in rivers)
            {
                foreach (var rch in r.Reaches)
                {
                    line = rch.ID + " RECT_OPEN  10.0  " + rch.Width + "     1        1          1    ";
                    sw.WriteLine(line);
                }
            }

            line = "[COORDINATES]";
            sw.WriteLine(line);
            line = ";;-------------- ---------- ---------- ---------- ---------- ----------";
            sw.WriteLine(line);

            foreach (var junc in junctions)
            {
                line = String.Format("{0}\t{1}\t{2}", junc.ID, junc.Coordinate.X, junc.Coordinate.Y);
                sw.WriteLine(line);
            }
            foreach (var junc in outfalls)
            {
                line = String.Format("{0}\t{1}\t{2}", junc.ID, junc.Coordinate.X, junc.Coordinate.Y);
                sw.WriteLine(line);
            }
            sw.Close();
        }
    }
}
