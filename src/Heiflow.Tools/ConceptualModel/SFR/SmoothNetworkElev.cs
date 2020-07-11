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
    public class SmoothNetworkElev : MapLayerRequiredTool
    {
        private IMapLayerDescriptor _JunctionFeatureLayerDescriptor;
        private IFeatureSet _junction_layer;
        public SmoothNetworkElev()
        {
            Name = "Smooth Stream Network Elevations";
            Category = Cat_CMG;
            SubCategory = "SFR";
            Description = "Smooth stream networkd elevations based on river feature";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            NodeTypeField = "NodeType";
            RiverIDField = "RiverID";
            ElevErrorField = "ElevError";
            BedElevField = "BedElev";
            SlopeField = "Slope";
            LengthField = "Length";
            SurfaceElevField = "Elev";
            NewBedElevField = "NewBedElev";
            MultiThreadRequired = true;
        }
        [Category("GIS Layer")]
        [Description("Junction layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor JuncitonFeatureLayer
        {
            get
            {
                return _JunctionFeatureLayerDescriptor;
            }
            set
            {
                _JunctionFeatureLayerDescriptor = value;
                if (_JunctionFeatureLayerDescriptor != null)
                {
                    _junction_layer = _JunctionFeatureLayerDescriptor.DataSet as IFeatureSet;
                    if (_junction_layer != null)
                    {
                        var buf = from DataColumn dc in _junction_layer.DataTable.Columns select dc.ColumnName;
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
        [Description("The node type field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string NodeTypeField
        {
            get;
            set;
        }
        [Category("Field Binding")]
        [Description("The river bed elevation field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string BedElevField
        {
            get;
            set;
        }
        [Category("Field Binding")]
        [Description("The smoothed river bed elevation field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string NewBedElevField
        {
            get;
            set;
        }
        [Category("Field Binding")]
        [Description("The surface elevation field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string SurfaceElevField
        {
            get;
            set;
        }
        [Category("Field Binding")]
        [Description("The river length field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string LengthField
        {
            get;
            set;
        }
        [Category("Field Binding")]
        [Description("The river slope field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string SlopeField
        {
            get;
            set;
        }
        [Category("Field Binding")]
        [Description("The elevation error field")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string ElevErrorField
        {
            get;
            set;
        }
        #endregion
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
                if(!_junction_layer.DataTable.Columns.Contains("ElevError"))
                {
                    _junction_layer.DataTable.Columns.Add(new DataColumn("ElevError", Type.GetType("System.Int32")));
                }
                var net = sfr.RiverNetwork;
                net.BuildHydroTrees();
                string msg = string.Format("Total number of outfalls is {0}", net.Outfalls.Count);
                int progress = 0;
                int i = 1;

                foreach(var tree in net.HydroTrees)
                {
                    msg = string.Format("Processing the tree {0}", tree.ID);
                    progress = i / net.HydroTrees.Count * 100;
                    cancelProgressHandler.Progress("Package_Tool", progress, msg);
                    var drnode = (from DataRow dr in _junction_layer.DataTable.Rows where int.Parse(dr[RiverIDField].ToString()) == tree.Root.River.ID && int.Parse(dr[NodeTypeField].ToString()) > 0 select dr).First();
                    tree.Root.BedElevation = double.Parse(drnode[BedElevField].ToString());
                    tree.Root.Slope = double.Parse(drnode[SlopeField].ToString());
                    tree.Root.Length = double.Parse(drnode[LengthField].ToString());
                    tree.Root.SurfaceElevation = double.Parse(drnode[SurfaceElevField].ToString());
                    GetElevFromShp(tree.Root);
                    SmoothElev(tree.Root);
                    Check(tree.Root);
                    i++;
                }
 
                sfr.NetworkToMat();
                _junction_layer.Save();
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 90, "SFR package not loaded.");
                return false;
            }
        }

        private void SmoothElev(HydroTreeNode curent)
        {
            if (curent.Parent == null)
            {
                foreach (var node in curent.Children)
                {
                    SmoothElev(node);
                }
                return;
            }
            else if (curent.ChildrenCount == 0)
            {
                if (curent.BedElevation <= curent.Parent.BedElevation)
                {
                    curent.BedElevation = curent.Parent.BedElevation + curent.Length * curent.Slope;
                    if (curent.BedElevation > curent.SurfaceElevation)
                        curent.BedElevation = curent.Parent.BedElevation + 0.1;
                    return;
                }
                else
                {
                    return;
                }
            }
            var child_elevs = (from ch in curent.Children select ch.BedElevation).ToArray();
            var child_min = child_elevs.Min();
            if (curent.Parent.BedElevation >= child_min)
            {
                if (curent.Parent.Parent != null)
                {
                    curent.Parent.BedElevation = curent.Parent.Parent.BedElevation + curent.Parent.Length * curent.Parent.Slope;
                    if (curent.Parent.BedElevation > curent.Parent.SurfaceElevation)
                        curent.Parent.BedElevation = curent.Parent.Parent.BedElevation + 0.1;
                }
                if (curent.Parent.BedElevation >= child_min)
                {
                    curent.Parent.BedElevation = child_min - 0.1;
                }
            }
            if (curent.BedElevation < curent.Parent.BedElevation || curent.BedElevation > child_min)
            {
                curent.BedElevation = (curent.Parent.BedElevation + child_min) * 0.5;
            }
            foreach (var node in curent.Children)
            {
                SmoothElev(node);
            }
        }

        private void Check(HydroTreeNode parent)
        {
            if (parent.ChildrenCount == 0)
                return;
            else
            {
                foreach (var node in parent.Children)
                {
                    node.ElevationFlag = 0;
                    if (node.BedElevation <= parent.BedElevation)
                    {
                        node.ElevationFlag = 1;
                    }
                    if (node.BedElevation > node.SurfaceElevation)
                    {
                        node.ElevationFlag = 2;
                    }
                    var drnode = (from DataRow dr in _junction_layer.DataTable.Rows where int.Parse(dr[RiverIDField].ToString()) == node.River.ID && int.Parse(dr[NodeTypeField].ToString()) > 0 select dr).First();
                    drnode[ElevErrorField] = node.ElevationFlag;
                    drnode[NewBedElevField] = node.BedElevation;
                    Check(node);
                }
            }
        }

        private void GetElevFromShp(HydroTreeNode parent)
        {
            if (parent.ChildrenCount == 0)
                return;
            else
            {
                foreach(var node in parent.Children)
                {
                    var drnode = (from DataRow dr in _junction_layer.DataTable.Rows where int.Parse(dr[RiverIDField].ToString()) == node.River.ID && int.Parse(dr[NodeTypeField].ToString()) > 0 select dr).First();
                    node.BedElevation = double.Parse(drnode[BedElevField].ToString());
                    node.Slope = double.Parse(drnode[BedElevField].ToString());
                    node.Slope = double.Parse(drnode[SlopeField].ToString());
                    node.Length = double.Parse(drnode[LengthField].ToString());
                    node.SurfaceElevation = double.Parse(drnode[SurfaceElevField].ToString());
                    GetElevFromShp(node);
                }
            }
        }

    }
}
