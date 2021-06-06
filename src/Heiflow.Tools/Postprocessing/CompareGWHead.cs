using DotSpatial.Data;
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
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

namespace Heiflow.Tools.Postprocessing
{
    public enum CompareMode { Head,Depth};
    public class CompareGWHead : MapLayerRequiredTool
    {
        public CompareGWHead()
        {
            Name = "Compare Groundwater Head";
            Category = "Postprocessing";
            Description = "Compare groundwater head for steady state";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            TimeStep = 1;
            CompareMode = CompareMode.Head;
        }

        private IMapLayerDescriptor _SourceLayer;
        private IFeatureSet _sourcefs;
        private IFeatureSet _grid_layer;
        private string _ObservedHeadField;
        private string _ObservedDepthField;
        private string _LayerItem;

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

        [Category("Field Parameter")]
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
        [Category("Field Parameter")]
        [Description("Field name of the Observed Head")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string ObservedDepthField
        {
            get
            {
                return _ObservedDepthField;
            }
            set
            {
                _ObservedDepthField = value;
            }
        }
        [Category("Parameter")]
        [Description("Field name of the Observed Head")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("LayerItems")]
        public string GroundwaterLayer
        {
            get
            {
                return _LayerItem;
            }
            set
            {
                _LayerItem = value;
            }
        }
        [Category("Parameter")]
        [Description("The time step when the comparison is done")]
        public int TimeStep
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("Comparision mode")]
        public CompareMode CompareMode
        {
            get;
            set;
        }
        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }
        [Browsable(false)]
        public string[] LayerItems
        {
            get;
            protected set;
        }
        public override void Setup()
        {
            base.Setup();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            if (prj != null)
            {
                var model = prj.Project.Model;
                var grid = model.Grid as MFGrid;
                LayerItems = new string[grid.LayerCount];
                LayerItems[0] = "Groundwater table";
                for (int i = 0; i < grid.LayerCount - 1; i++)
                {
                    LayerItems[i + 1] = "Groundwater head for Layer " + (i + 1);
                }
                GroundwaterLayer = LayerItems[0];
            }
            else
            {
                this.Initialized = false;
            }
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
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            int progress = 0;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;
            if (mf != null)
            {
                var buf = from pp in mf.Packages[MFOutputPackage.PackageName].Children where pp.Name == FHDPackage.PackageName select pp;
                if (buf.Any())
                {
                    var grid = (mf.Grid as MFGrid);
                    var fhd = buf.First() as FHDPackage;
                    int nwel = _sourcefs.DataTable.Rows.Count;
                    if (!_sourcefs.DataTable.Columns.Contains("SimHead"))
                    {
                        DataColumn dc = new DataColumn("SimHead", Type.GetType("System.Double"));
                        _sourcefs.DataTable.Columns.Add(dc);
                    }
                    if (!_sourcefs.DataTable.Columns.Contains("SimDepth"))
                    {
                        DataColumn dc = new DataColumn("SimDepth", Type.GetType("System.Double"));
                        _sourcefs.DataTable.Columns.Add(dc);
                    }
                    if(!_sourcefs.DataTable.Columns.Contains("HeadDiff"))
                    {
                        DataColumn dc = new DataColumn("HeadDiff", Type.GetType("System.Double"));
                        _sourcefs.DataTable.Columns.Add(dc);
                    }
                    if (!_sourcefs.DataTable.Columns.Contains("DepthDiff"))
                    {
                        DataColumn dc = new DataColumn("DepthDiff", Type.GetType("System.Double"));
                        _sourcefs.DataTable.Columns.Add(dc);
                    }

                    int layer = GetLayerIndex();
                    if (fhd.DataCube.IsAllocated(layer))
                    {
                        float head = 0;
                        float elev = 0;
                        float obsdep = 0;
                        float[] sim = new float[nwel];
                        float[] obs = new float[nwel];
                        float[] sim_dep = new float[nwel];
                        float[] obs_dep = new float[nwel];

                        if (TimeStep <= 0 || TimeStep > fhd.DataCube.Size[1])
                            TimeStep = fhd.DataCube.Size[1];
                        for (int i = 0; i < nwel; i++)
                        {
                            float.TryParse(_sourcefs.DataTable.Rows[i][ObservedHeadField].ToString(), out head);
                            float.TryParse(_sourcefs.DataTable.Rows[i][ObservedDepthField].ToString(), out obsdep);
                            //var pt = _sourcefs.Features[i].Geometry.Coordinate;
                            for (int j = 0; j < _grid_layer.Features.Count; j++)
                            {
                                //var cell = _grid_layer.Features[j].Geometry.Coordinates;
                                //if (SpatialRelationship.PointInPolygon(cell, pt))
                                if (_sourcefs.Features[i].Geometry.Within(_grid_layer.Features[j].Geometry))
                                {
                                    var Row = int.Parse(_grid_layer.DataTable.Rows[j]["ROW"].ToString());
                                    var Column = int.Parse(_grid_layer.DataTable.Rows[j]["COLUMN"].ToString());
                                    var index = grid.Topology.GetSerialIndex(Row - 1, Column - 1);
                                    sim[i] = fhd.DataCube[layer, TimeStep - 1, index];
                                    elev = grid.Elevations[0, 0, index];
                                    break;
                                } 
                            }
                            obs[i] = head;
                            obs_dep[i] = obsdep;
                            if (CompareMode == CompareMode.Head)
                            {
                                sim_dep[i] = elev - sim[i];
                                _sourcefs.DataTable.Rows[i]["SimHead"] = sim[i];
                                _sourcefs.DataTable.Rows[i]["SimDepth"] = sim_dep[i];
                                _sourcefs.DataTable.Rows[i]["HeadDiff"] = System.Math.Round((sim[i] - head), 2);
                                _sourcefs.DataTable.Rows[i]["DepthDiff"] = System.Math.Round((sim_dep[i] - obsdep), 2);
                            }
                            else
                            {
                                sim_dep[i] =  sim[i];
                                _sourcefs.DataTable.Rows[i]["SimHead"] = elev - sim[i];
                                _sourcefs.DataTable.Rows[i]["SimDepth"] = sim_dep[i];
                                _sourcefs.DataTable.Rows[i]["HeadDiff"] = System.Math.Round((elev - sim_dep[i] - head), 2);
                                _sourcefs.DataTable.Rows[i]["DepthDiff"] = System.Math.Round((sim_dep[i] - obsdep), 2);
                            }

                            progress = i * 100 / nwel;
                            cancelProgressHandler.Progress("Package_Tool", progress, "Process observation well: " + (i + 1));
                        }
                        _sourcefs.Save();
                        if (obs.Length > 0)
                        {
                            if (CompareMode == CompareMode.Head)
                            {
                                var min = System.Math.Min(obs.Min(), sim.Min());
                                var max = System.Math.Max(obs.Max(), sim.Max());
                                var xx = new float[] { min, max };
                                var yy = new float[] { min, max };
                                WorkspaceView.Plot<float>(xx, yy, "45 Degree Line", Models.UI.MySeriesChartType.FastLine);
                                WorkspaceView.Plot<float>(obs, sim, -999, "Observed VS. Simulated Head", Models.UI.MySeriesChartType.FastPoint);
                            }
                            else
                            {
                                var min = System.Math.Min(obs_dep.Min(), sim_dep.Min());
                                var max = System.Math.Max(obs_dep.Max(), sim_dep.Max());
                                var xx = new float[] { min, max };
                                var yy = new float[] { min, max };
                                WorkspaceView.Plot<float>(xx, yy, "45 Degree Line", Models.UI.MySeriesChartType.FastLine);
                                WorkspaceView.Plot<float>(obs_dep, sim_dep, -999, "Observed VS. Simulated Depth", Models.UI.MySeriesChartType.FastPoint);
                            }
                        }
                    }
                    else
                    {
                        cancelProgressHandler.Progress("Package_Tool", 100, "FHD must be loaded at first.");
                    }
                }
                else
                {
                    cancelProgressHandler.Progress("Package_Tool", 100, "FHD package not found.");
                }
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow is used by this tool.");
                return false;
            }
        }

        private int GetLayerIndex()
        {
            int index = 0;
            for (int i = 0; i < LayerItems.Length; i++)
            {
                if (LayerItems[i] == GroundwaterLayer)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }
}
