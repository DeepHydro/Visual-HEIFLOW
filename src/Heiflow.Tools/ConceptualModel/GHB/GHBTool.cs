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
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.ConceptualModel
{
    public class GHBTool : MapLayerRequiredTool
    {
        private IFeatureSet _sourcefs;
        private IFeatureSet _grid_layer;
        private string _BheadField;
        private string _EheadField;
        private string _LayerField;
        private int _SelectedLayerIndex = 0;
        private IMapLayerDescriptor _ghbSourceLayer;
        public GHBTool()
        {
            Name = "Create GHB Package ";
            Category = Cat_CMG;
            SubCategory = "GHB";
            Description = "Create General-Head Boundary Package from point shapefile";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }


        [Category("Input")]
        [Description("GHB point source layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GHBSourceLayer
        {
            get
            {
                return _ghbSourceLayer;
            }
            set
            {
                _ghbSourceLayer = value;
                _sourcefs = _ghbSourceLayer.DataSet as IFeatureSet;
                if (_sourcefs != null)
                {
                    var buf = from DataColumn dc in _sourcefs.DataTable.Columns select dc.ColumnName;
                    Fields = buf.ToArray();
                }
            }
        }

        [Category("Input")]
        [Description("Model grid  layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GridFeatureLayer
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("Field of the boundary head")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string BHeadField
        {
            get
            {
                return _BheadField;
            }
            set
            {
                _BheadField = value;
            }
        }
        [Category("Parameter")]
        [Description("Field of the conductance")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string CondField
        {
            get
            {
                return _EheadField;
            }
            set
            {
                _EheadField = value;
            }
        }
        [Category("Parameter")]
        [Description("Field of the layer")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string LayerField
        {
            get
            {
                return _LayerField;
            }
            set
            {
                _LayerField = value;
                if (Fields != null)
                {
                    for (int i = 0; i < Fields.Length; i++)
                    {
                        if (_LayerField == Fields[i])
                        {
                            _SelectedLayerIndex = i;
                            break;
                        }
                    }
                }
            }
        }
        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }

        public override void Initialize()
        {
            _grid_layer = GridFeatureLayer.DataSet as IFeatureSet;
            if (_sourcefs == null || _grid_layer == null)
            {
                this.Initialized = false;
                return;
            }

            this.Initialized = !(_grid_layer == null || _grid_layer.FeatureType != FeatureType.Polygon);
            this.Initialized = !(_sourcefs == null || _sourcefs.FeatureType != FeatureType.Point);
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            int progress = 0;
            int count = 1;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;
            if (mf != null)
            {
                if (!mf.Packages.ContainsKey(GHBPackage.PackageName))
                {
                    var fhb = mf.Select(GHBPackage.PackageName);
                    mf.Add(fhb);
                }
                var nsp = mf.TimeService.StressPeriods.Count;
                var pck = mf.GetPackage(GHBPackage.PackageName) as GHBPackage;
                List<CellHead> list = new List<CellHead>();
                int npt = _sourcefs.Features.Count;
                for (int i = 0; i < npt; i++)
                {
                    var pt = _sourcefs.Features[i].Geometry.Coordinate;
                    int layer = 1;
                    float bhead = 0;
                    float cond = 0;
                    if (!string.IsNullOrEmpty(LayerField))
                    {
                        int.TryParse(_sourcefs.DataTable.Rows[i][LayerField].ToString(), out layer);
                    }
                    if (!string.IsNullOrEmpty(BHeadField))
                    {
                        float.TryParse(_sourcefs.DataTable.Rows[i][BHeadField].ToString(), out bhead);
                    }
                    if (!string.IsNullOrEmpty(CondField))
                    {
                        float.TryParse(_sourcefs.DataTable.Rows[i][CondField].ToString(), out cond);
                    }
                    for (int j = 0; j < _grid_layer.Features.Count; j++)
                    {
                        var cell = _grid_layer.Features[j].Geometry.Coordinates;
                        if (SpatialRelationship.PointInPolygon(cell, pt))
                        {
                            CellHead bound = new CellHead()
                            {
                                Layer = layer,
                                SHead = bhead,
                                EHead=cond,
                                Row = int.Parse(_grid_layer.DataTable.Rows[j]["ROW"].ToString()),
                                Col = int.Parse(_grid_layer.DataTable.Rows[j]["COLUMN"].ToString()),
                            };
                            list.Add(bound);
                            break;
                        }
                    }
                    progress = i * 100 / npt;
                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing point: " + i);
                        count++;
                    }
                }
                if (list.Count > 0)
                {
                    pck.MXACTB = list.Count;
                    var FlowRate = new DataCube<float>(mf.TimeService.StressPeriods.Count, pck.MXACTB, 5, true)
                    {
                        Name = "GHB_Head"
                    };
                    FlowRate.ColumnNames = new string[] { "LAYER", "ROW", "COL", "SHEAD", "EHEAD" };
                    FlowRate.Variables = new string[nsp];
                    for (int i = 0; i < nsp; i++)
                    {
                        FlowRate.Variables[i] = "Stress Period " + (i + 1);
                    }
                    FlowRate.Allocate(0, pck.MXACTB, 5);
                    FlowRate.Flags[0] = TimeVarientFlag.Individual;
                    for (int i = 0; i < pck.MXACTB; i++)
                    {
                        var bound = list[i];
                        FlowRate[0, i, 0] = bound.Layer;
                        FlowRate[0, i, 1] = bound.Row;
                        FlowRate[0, i, 2] = bound.Col;
                        FlowRate[0, i, 3] = bound.SHead;
                        FlowRate[0, i, 4] = bound.EHead;
                    }
                    for (int i = 1; i < nsp; i++)
                    {
                        FlowRate.Flags[i] = TimeVarientFlag.Repeat;
                    }
                
                    pck.ITMP = FlowRate;
                    pck.CreateFeature(shell.MapAppManager.Map.Projection, prj.Project.GeoSpatialDirectory);
                    pck.BuildTopology();
                    pck.IsDirty = true;
                    pck.Save(null);
                    pck.ChangeState(Models.Generic.ModelObjectState.Ready);
                }
                else
                {
                    pck.ChangeState(Models.Generic.ModelObjectState.Standby);
                    cancelProgressHandler.Progress("Package_Tool", 100, "Warning: no points located in the modeling domain.");
                }
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow must be used by this tool.");
                return false;
            }
        }

        public override void AfterExecution(object args)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model as Heiflow.Models.Integration.HeiflowModel;

            if (model != null)
            {
                var uzf = prj.Project.Model.GetPackage(UZFPackage.PackageName) as UZFPackage;
                var chd = prj.Project.Model.GetPackage(GHBPackage.PackageName) as GHBPackage;
                for (int i = 0; i < chd.MFGridInstance.ActiveCellCount; i++)
                {
                    uzf.IUZFBND[0, 0, i] = 1;
                }
                for (int i = 0; i < chd.MXACTB; i++)
                {
                    var index = chd.MFGridInstance.Topology.GetSerialIndex((int)(chd.ITMP[0, i, 1] - 1), (int)(chd.ITMP[0, i, 2] - 1));
                    uzf.IUZFBND[0, 0, index] = 0;
                }
                 
                chd.Attach(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);
                shell.ProjectExplorer.ClearContent();
                shell.ProjectExplorer.AddProject(prj.Project);
            }
        }
    }
}
