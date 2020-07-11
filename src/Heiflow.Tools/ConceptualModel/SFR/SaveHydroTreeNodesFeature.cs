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
using Heiflow.Core.Data;
using Heiflow.Core.Hydrology;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using NetTopologySuite.Geometries;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Heiflow.Tools.ConceptualModel
{

    public class SaveHydroTreeNodesFeature : MapLayerRequiredTool
    {
        private IMapLayerDescriptor _StreamFeatureLayerDescriptor;
        private IFeatureSet _stream_layer;
        public SaveHydroTreeNodesFeature()
        {
            Name = "Save Network Nodes As Feature Layer";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Save Network Nodes As Feature Layer";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            SegmentIDField = "GRID_CODE";
            ReversePointOrder = false;
        }

        #region GIS Layers

        [Category("GIS Layer")]
        [Description("Stream layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor StreamFeatureLayer
        {
            get
            {
                return _StreamFeatureLayerDescriptor;
            }
            set
            {
                _StreamFeatureLayerDescriptor = value;
                if (_StreamFeatureLayerDescriptor != null)
                {
                    _stream_layer = _StreamFeatureLayerDescriptor.DataSet as IFeatureSet;
                    if (_stream_layer != null)
                    {
                        var buf = from DataColumn dc in _stream_layer.DataTable.Columns select dc.ColumnName;
                        Fields = buf.ToArray();
                    }
                }
            }
        }

        [Category("Output")]
        [Description("The shapefile name")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DataFileName
        {
            get;
            set;
        }
        #endregion

        #region Field Binding
        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }
        [Category("GIS Layer")]
        [Description("The segment ID field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string SegmentIDField
        {
            get;
            set;
        }

        public bool ReversePointOrder
        {
            get;
            set;
        }
        #endregion

        public override void Initialize()
        {
            this.Initialized = _stream_layer != null;
        }

        private void CreateNodeFeature(HydroTreeNode parent)
        {
            if(parent.IsLeaf)
            {
                return;
            }
            var riv = from fea in _stream_layer.Features where int.Parse(fea.DataRow[SegmentIDField].ToString()) == parent.River.ID select fea;
            if(riv.Any())
            {
                
            }
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var dt = _stream_layer.DataTable;
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var grid = prj.Project.Model.Grid as MFGrid;
            var sfr = prj.Project.Model.GetPackage(SFRPackage.PackageName) as SFRPackage;
            if (sfr != null)
            {
                if (sfr.RiverNetwork.RiverCount != dt.Rows.Count)
                {
                    cancelProgressHandler.Progress("Package_Tool", 1, "Warning: Segment count in SFR package is not equal to the row count of the GIS layer");
                    return false;
                }
                sfr.RiverNetwork.BuildHydroTrees();
                DotSpatial.Data.FeatureSet fs = new DotSpatial.Data.FeatureSet(FeatureType.Point);
                fs.DataTable.Columns.Add(new DataColumn("ID", Type.GetType("System.Int32")));
                fs.DataTable.Columns.Add(new DataColumn("TreeID", Type.GetType("System.Int32")));
                fs.DataTable.Columns.Add(new DataColumn("NodeID", Type.GetType("System.Int32")));
                fs.DataTable.Columns.Add(new DataColumn("RiverID", Type.GetType("System.Int32")));
                fs.DataTable.Columns.Add(new DataColumn("NodeType", Type.GetType("System.Int32")));
                fs.DataTable.Columns.Add(new DataColumn("Elev", typeof(double)));
                fs.DataTable.Columns.Add(new DataColumn("Depth", typeof(double)));
                fs.DataTable.Columns.Add(new DataColumn("Slope", typeof(double)));
                fs.DataTable.Columns.Add(new DataColumn("Width", typeof(double)));
                var nodeid = 0;
                foreach(var tree in sfr.RiverNetwork.HydroTrees)
                {
                    nodeid++;
                    var riv = (from fea in _stream_layer.Features where int.Parse(fea.DataRow[SegmentIDField].ToString()) == tree.Outlet.River.ID select fea).First();
                    Coordinate pt = null;
                    if(ReversePointOrder)
                    {
                      pt = riv.Geometry.Coordinates.First();
                    }
                    else
                    {
                        pt = riv.Geometry.Coordinates.Last();
                    }
                    var point = new Point(pt.X, pt.Y);
                    IFeature ft = fs.AddFeature(point);
                    ft.DataRow["ID"] = nodeid;
                    ft.DataRow["TreeID"] = tree.ID;
                    ft.DataRow["NodeID"] = tree.Outlet.ID;
                    ft.DataRow["RiverID"] = tree.Outlet.River.ID;
                    ft.DataRow["NodeType"] = 0;
                    ft.DataRow["Elev"] = 0;
                    ft.DataRow["Depth"] = 0;
                    ft.DataRow["Slope"] = 0;
                    ft.DataRow["Width"] = 0;
                    foreach(var node in tree.Nodes)
                    {
                        nodeid++;
                        riv = (from fea in _stream_layer.Features where int.Parse(fea.DataRow[SegmentIDField].ToString()) == node.River.ID select fea).First();
                        if (ReversePointOrder)
                        {
                            pt = riv.Geometry.Coordinates.Last();
                        }
                        else
                        {
                            pt = riv.Geometry.Coordinates.First();
                        }
                        point = new Point(pt.X, pt.Y);
                        ft = fs.AddFeature(point);
                        ft.DataRow["ID"] = nodeid;
                        ft.DataRow["TreeID"] = tree.ID;
                        ft.DataRow["NodeID"] = node.ID;
                        ft.DataRow["RiverID"] = node.River.ID;
                        ft.DataRow["NodeType"] = node.IsLeaf ? 2 : 1;
                        ft.DataRow["Elev"] = 0;
                        ft.DataRow["Depth"] = 0;
                        ft.DataRow["Slope"] = 0;
                        ft.DataRow["Width"] = 0;
                    }
                }

                fs.Projection = _stream_layer.Projection;
                fs.SaveAs(DataFileName, true);

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
