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
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.ConceptualModel
{
    public class HOBTool : MapLayerRequiredTool
    {
        public HOBTool()
        {
            Name = "Create HOB Package";
            Category = Cat_CMG;
            SubCategory = "HOB";
            Description = "Create Head Observation (HOB) package from point shapefile";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
        }

        private IMapLayerDescriptor _SourceLayer;
        private IFeatureSet _sourcefs;
        private IFeatureSet _grid_layer;
        private string _ObservedHeadField;
        private string _OBSNAMField;
        private bool _issuccess;
        private string _LayerField;

        [Category("Input")]
        [Description("HOB source layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor SourceLayer
        {
            get
            {
                return _SourceLayer;
            }
            set
            {
                _SourceLayer = value;
                _sourcefs = _SourceLayer.DataSet as IFeatureSet;
                if (_sourcefs != null)
                {
                    var buf = from DataColumn dc in _sourcefs.DataTable.Columns select dc.ColumnName;
                    Fields = buf.ToArray();
                }
            }
        }

        [Category("Input")]
        [Description("Model grid layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GridFeatureLayer
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("Field name of the Observed Head")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string ObservedHeadField
        {
            get
            {
                return _ObservedHeadField;
            }
            set
            {
                _ObservedHeadField = value;
            }
        }
        [Category("Parameter")]
        [Description("Field name of Observation Name")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string OBSNAMField
        {
            get
            {
                return _OBSNAMField;
            }
            set
            {
                _OBSNAMField = value;
            }
        }
        [Category("Parameter")]
        [Description("Field name of the layer")]
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
            if (_sourcefs == null || _grid_layer == null )
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
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;
            int nsp = mf.TimeService.StressPeriods.Count;
            if (mf != null)
            {
                if (!mf.Packages.ContainsKey(HOBPackage.PackageName))
                {
                    var hob = mf.Select(HOBPackage.PackageName);
                    mf.Add(hob);
                }
                var grid = (mf.Grid as MFGrid);
                var pck = mf.GetPackage(HOBPackage.PackageName) as HOBPackage;
                int nwel = _sourcefs.Features.Count;
                pck.MOBS = 0;
                pck.MAXM = 2;
                pck.NoDataValue = -999;
                pck.Option = "NOPRINT";

                float head = 0;
                Coordinate pt = null;
                int layer = 1;
                string name = "";
                pck.Observations.Clear();
                for (int i = 0; i < nwel; i++)
                {
                    float.TryParse(_sourcefs.DataTable.Rows[i][ObservedHeadField].ToString(), out head);
                    pt = _sourcefs.Features[i].Geometry.Coordinate;
                    for (int j = 0; j < _grid_layer.Features.Count; j++)
                    {
                        var cell = _grid_layer.Features[j].Geometry.Coordinates;
                        if (SpatialRelationship.PointInPolygon(cell, pt))
                        {
                            HeadObservation obs = new HeadObservation(i)
                            {
                                Row = int.Parse(_grid_layer.DataTable.Rows[j]["ROW"].ToString()),
                                Column = int.Parse(_grid_layer.DataTable.Rows[j]["COLUMN"].ToString()),
                                IREFSPFlag = -1
                            };
                            name =  _sourcefs.DataTable.Rows[i][OBSNAMField].ToString();
                            if (name.Length > 12)
                                name = name.Substring(0, 12);
                            int.TryParse(_sourcefs.DataTable.Rows[i][LayerField].ToString(), out layer);
                            obs.ID = i + 1;
                            obs.Name = name;
                            obs.CellID = grid.Topology.GetID(obs.Row - 1, obs.Column - 1);
                            obs.ITT = 1;
                            obs.IREFSP = new int[nsp];
                            obs.TOFFSET = new float[nsp];
                            obs.HOBS = new float[nsp];
                            obs.ROFF = 0;
                            obs.COFF = 0;
                            obs.Layer = layer;
                            for (int t = 0; t < nsp; t++)
                            {
                                obs.IREFSP[t] = 1;
                                obs.TOFFSET[t] = 0;
                                obs.HOBS[t] = head;
                            }
                            pck.Observations.Add(obs);
                            break;
                        }
                    }
                    progress = i * 100 / nwel;
                    cancelProgressHandler.Progress("Package_Tool", progress, "Process observation: " + (i + 1));
                }
                pck.NH = pck.Observations.Count;
                var mfout = mf.GetPackage(MFOutputPackage.PackageName) as MFOutputPackage;
                pck.CompositeOutput(mfout);
                pck.CreateFeature(shell.MapAppManager.Map.Projection, prj.Project.GeoSpatialDirectory);
                pck.BuildTopology();
                pck.IsDirty = true;
                pck.Save(null);
                pck.ChangeState(Models.Generic.ModelObjectState.Ready);
                _issuccess = true;
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow is used by this tool.");
                _issuccess = false;
                return false;
            }
        }

        public override void AfterExecution(object args)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();

            if (prj.Project.Model is Heiflow.Models.Integration.HeiflowModel)
            {
                var hob = prj.Project.Model.GetPackage(HOBPackage.PackageName) as HOBPackage;
                if (hob != null && _issuccess)
                {
                    hob.Attach(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);
                    shell.ProjectExplorer.ClearContent();
                    shell.ProjectExplorer.AddProject(prj.Project);
                }
            }
        }
    }
}
