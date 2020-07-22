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
            MaxDepth = 5;
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

        [Category("Optional")]
        [Description("maximum depth")]
        public double MaxDepth
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
                    SmoothNodeElevBySlope(tree.Root);
                    Check(tree.Root);
                    SmoothReach(tree.Root, net);
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
        private void SmoothNodeElevBySlope(HydroTreeNode curent)
        {
            if (curent.Parent == null)
            {
                foreach (var node in curent.Children)
                {
                    SmoothNodeElevBySlope(node);
                }
            }
            else if (curent.ChildrenCount == 0)
            {
                return;
            }
           
            foreach (var node in curent.Children)
            {
                var elevs = node.Parent.BedElevation + node.Slope * node.Length;
                var temp = System.Math.Min(elevs, node.BedElevation);
                var minelev= node.SurfaceElevation - MaxDepth;
                if (temp < minelev)
                    temp = minelev;

                if (temp > node.Parent.BedElevation)
                    node.BedElevation = temp;
                else
                {
                    node.BedElevation = node.Parent.BedElevation + 0.1;
                    node.Slope = 0.1 / node.Length;
                }
                SmoothNodeElevBySlope(node);
            }
        }
        private void Check(HydroTreeNode parent)
        {
            if(parent.Parent == null)
            {
                var drnode = (from DataRow dr in _junction_layer.DataTable.Rows where int.Parse(dr[RiverIDField].ToString()) == parent.River.ID && int.Parse(dr[NodeTypeField].ToString()) > 0 select dr).First();
                drnode[ElevErrorField] = 0;
                drnode[NewBedElevField] = parent.BedElevation;
            }
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
                    // Greater than surface elevation
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

        private void SmoothReach(HydroTreeNode curent, RiverNetwork net)
        {
            if (curent.Parent == null)
            {
                SetReachElev(curent);
                //curent.River.OutletNode.Elevation = curent.River.LastReach.TopElevation;
                foreach (var node in curent.Children)
                {
                    SmoothReach(node,net);
                }
            }
            else if (curent.ChildrenCount == 0)
            {
                SetReachElev(curent);
                return;
            }

            foreach (var node in curent.Children)
            {
                SetReachElev(curent);
                SmoothReach(node, net);
            }
        }

        private void SetReachElev(HydroTreeNode node)
        {
            var river = node.River;
            var nrch = river.Reaches.Count;
                            var lengths = (from rch in river.Reaches select rch.Length).ToArray();
                var totallen = lengths.Sum() - river.Reaches[nrch - 1].Length;
            if (node.Parent != null)
            {
                var dh = node.BedElevation - node.Parent.BedElevation;
                if (dh < 0)
                {
                    throw new Exception("invalid bed elevation");
                }


                double aclen = 0;
                river.Reaches[0].TopElevation = node.BedElevation;
                river.Reaches[0].InletNode.Elevation = node.BedElevation;
                for (int i = 0; i < nrch; i++)
                {
                    river.Reaches[i].Slope = node.Slope;
                }
                for (int i = 1; i < nrch; i++)
                {
                    aclen += lengths[i - 1];
                    river.Reaches[i].TopElevation = node.BedElevation - aclen / totallen * dh;
                    river.Reaches[i].InletNode.Elevation = river.Reaches[i].TopElevation;
                }
                river.LastReach.OutletNode.Elevation = node.Parent.BedElevation;
                if (nrch > 2)
                    river.LastReach.InletNode.Elevation = (river.Reaches[nrch - 2].InletNode.Elevation + river.LastReach.OutletNode.Elevation) * 0.5;
                else if (nrch == 2)
                    river.LastReach.InletNode.Elevation = (node.BedElevation + river.LastReach.OutletNode.Elevation) * 0.5;
            }
            else
            {
                river.Reaches[0].TopElevation = node.BedElevation;
                river.Reaches[0].InletNode.Elevation = node.BedElevation;
                for (int i = 0; i < nrch; i++)
                {
                    river.Reaches[i].Slope = node.Slope;
                }
                for (int i = 1; i < nrch; i++)
                {
                    river.Reaches[i].TopElevation = river.Reaches[i - 1].TopElevation - node.Slope * river.Reaches[i - 1].Length;
                    river.Reaches[i].InletNode.Elevation = river.Reaches[i].TopElevation;
                }
                //if (nrch > 2)
                //{
                    river.LastReach.OutletNode.Elevation = river.LastReach.InletNode.Elevation - river.LastReach.Slope * river.LastReach.Length;  
                //}
                //else
                //{
                //    river.LastReach.OutletNode.Elevation = node.BedElevation - lengths.Sum() * node.Slope;
                //}
                river.OutletNode.Elevation = river.LastReach.OutletNode.Elevation;
            }
        }
    }
}
