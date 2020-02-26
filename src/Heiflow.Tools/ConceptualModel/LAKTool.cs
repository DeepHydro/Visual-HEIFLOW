using DotSpatial.Data;
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Parameters;
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
    public  class LAKTool : MapLayerRequiredTool
    {
        private IMapLayerDescriptor _LakeFeatureLayerDescriptor;
            public LAKTool()
        {
            Name = "Create LAK Package";
            Category = Cat_CMG;
            SubCategory = "LAK";
            Description = "Create LAK package from polygon shapefile";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
       
        }

        [Category("Input")]
        [Description("Model grid  layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor GridFeatureLayer
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("Stream layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor LakeFeatureLayer
        {
            get
            {
                return _LakeFeatureLayerDescriptor;
            }
            set
            {
                _LakeFeatureLayerDescriptor = value;
                var sourcefs = _LakeFeatureLayerDescriptor.DataSet as IFeatureSet;
                if (sourcefs != null)
                {
                    var buf = from DataColumn dc in sourcefs.DataTable.Columns select dc.ColumnName;
                    Fields = buf.ToArray();
                }
            }
        }
        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }

        [Category("Field")]
        [Description("Field name of lake ID")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string LakeIDField
        {
            get;
            set;
        }

        [Category("Field")]
        [Description("Field name of segment that flows into the lake.")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string InletSegIDField
        {
            get;
            set;
        }

        [Category("Field")]
        [Description("Field name of lakebed leakance")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string BDLKNCField
        {
            get;
            set;
        }
        public override void Initialize()
        {
            Initialized = true;
            if (GridFeatureLayer == null || LakeFeatureLayer == null)
            {
                this.Initialized = false;
                return;
            }
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            var shell = MyAppManager.Instance.CompositionContainer.GetExportedValue<IShellService>();
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            int progress = 0;
            int count = 1;
            var _sourcefs = LakeFeatureLayer.DataSet as IFeatureSet;
            var _grid_layer = this.GridFeatureLayer.DataSet as IFeatureSet;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;
            if (mf != null)
            {
                var grid = mf.Grid as MFGrid;
                var nlayer = grid.ActualLayerCount;
                var nsp = mf.TimeService.StressPeriods.Count;
                if (!mf.Packages.ContainsKey(LakePackage.PackageName))
                {
                    var fhb = mf.Select(LakePackage.PackageName);
                    mf.Add(fhb);
                }
                var pck = mf.GetPackage(LakePackage.PackageName) as LakePackage;
                int nlake = _sourcefs.Features.Count;
                pck.LakeSerialIndex.Clear();
                Coordinate[][] lake_bund = new Coordinate[nlake][];
                var lak_leakance = new float[nlake];
                for (int i = 0; i < nlake; i++)
                {
                    lake_bund[i] = _sourcefs.Features[i].Geometry.Coordinates;
                    var lake_id = int.Parse(_sourcefs.DataTable.Rows[i][LakeIDField].ToString());
                    pck.LakeSerialIndex.Add(lake_id, new List<int>());
                    lak_leakance[i] = float.Parse(_sourcefs.DataTable.Rows[i][BDLKNCField].ToString());
                }
                for (int i = 0; i < _grid_layer.Features.Count; i++)
                {
                    var cell = _grid_layer.Features[i].Geometry.Centroid;
                    var centroid = new Coordinate(cell.X, cell.Y);

                    for (int j = 0; j < nlake; j++)
                    {
                        if (SpatialRelationship.PointInPolygon(lake_bund[j], centroid))
                        {
                            var hru_id = int.Parse(_grid_layer.DataTable.Rows[i]["HRU_ID"].ToString());
                            var lake_id = int.Parse(_sourcefs.DataTable.Rows[j][LakeIDField].ToString());
                            pck.LakeSerialIndex[lake_id].Add(hru_id - 1);
                            break;
                        }
                    }
                    progress = i * 100 / _grid_layer.Features.Count;
                    if (progress > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing cell: " + i);
                        count++;
                    }
                }
                pck.NLAKES = nlake;
                pck.STAGES = new DataCube2DLayout<float>(1, nlake, 3)
                {
                    ColumnNames = new string[] { "STAGES", "SSMN", "SSMX", "IUNITTAB", "CLAKE" }
                };
                for (int i = 0; i < nlake; i++)
                {
                    pck.STAGES[0][i, ":"] = new float[] { 100, 90, 110, 0, 0 };
                }

                pck.ITMP = new DataCube2DLayout<int>(1, nsp, 3)
                {
                    ColumnNames = new string[] { "ITMP", "ITMP1", "LWRT" }
                };
                pck.LKARR = new DataCube<int>(nlayer, 1, grid.ActiveCellCount)
                {
                    Name = "Lake ID",
                    Variables = new string[nlayer],
                    ZeroDimension = DimensionFlag.Spatial
                };
                for (int l = 0; l < nlayer; l++)
                {
                    pck.LKARR.Variables[l] = "Lake ID of " + " Layer " + (l + 1);
                }
                pck.BDLKNC = new DataCube<float>(nlayer, 1, grid.ActiveCellCount)
                {
                    Name = "Leakance",
                    Variables = new string[nlayer],
                    ZeroDimension = DimensionFlag.Spatial
                };
                for (int l = 0; l < nlayer; l++)
                {
                    pck.BDLKNC.Variables[l] = " Layer " + (l + 1);
                }
                pck.NSLMS = new DataCube2DLayout<int>(1, nsp, 1)
                {
                    Name = "Num of Sublakes",
                    Variables = new string[nsp],
                    ColumnNames = new string[] { "Num of Sublakes" }
                };
                for (int l = 0; l < nsp; l++)
                {
                    pck.NSLMS.Variables[l] = "Stress Period " + (l + 1);
                }
                pck.WSOUR = new DataCube2DLayout<float>(nsp, nlake, 6)
                {
                    Name = "Recharge Discharge",
                    Variables = new string[nsp],
                    ZeroDimension = DimensionFlag.Time,
                    ColumnNames = new string[] { "PRCPLK", "EVAPLK", "RNF", "WTHDRW", "SSMN", "SSMX" }
                };
                for (int l = 0; l < nsp; l++)
                {
                    pck.WSOUR.Variables[l] = "Stress Period " + (l + 1);
                }

                pck.ITMP[0][0, ":"] = new int[] { 1, 1, 0 };
                for (int i = 1; i < nsp; i++)
                {
                    pck.ITMP[0][i, ":"] = new int[] { -1, 1, 0 };
                }

                for (int i = 0; i < pck.LakeSerialIndex.Keys.Count; i++)
                {
                    var id = pck.LakeSerialIndex.Keys.ElementAt(i);
                    foreach (var hru_index in pck.LakeSerialIndex[id])
                    {
                        pck.LKARR[0, 0, hru_index] = id;
                        pck.BDLKNC[0, 0, hru_index] = lak_leakance[i];
                    }
                }
                cancelProgressHandler.Progress("Package_Tool", progress, "Saving feature file");
                pck.CreateFeature(shell.MapAppManager.Map.Projection, prj.Project.GeoSpatialDirectory);
                pck.BuildTopology();
                pck.IsDirty = true;
                pck.Save(null);
                pck.ChangeState(Models.Generic.ModelObjectState.Ready);
                return true;
            }
            else
            {             
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow is used by this tool.");
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
                var pck = model.GetPackage(LakePackage.PackageName) as LakePackage;
                var hru_type = model.PRMSModel.MMSPackage.Parameters["hru_type"];
                var lake_hru_id = model.PRMSModel.MMSPackage.Parameters["lake_hru_id"];
                for (int i = 0; i < pck.LakeSerialIndex.Keys.Count; i++)
                {
                    var id = pck.LakeSerialIndex.Keys.ElementAt(i);
                    foreach (var hru_index in pck.LakeSerialIndex[id])
                    {
                        lake_hru_id.SetValue(0, hru_index, 0, id);
                        hru_type.SetValue(0, hru_index, 0, 2);
                    }
                }
                var nlake = model.PRMSModel.MMSPackage.Select("nlake");
                if(nlake == null)
                {
                    var para = (from p in model.PRMSModel.MMSPackage.DefaultParameters where p.Name == "nlake" select p).First();
                    DataCubeParameter<int> gv = new DataCubeParameter<int>(1, 1, 1, false)
                    {
                        ValueType = para.ValueType,
                        VariableType = ParameterType.Dimension,
                        Dimension = 1,
                        DimensionNames = para.DimensionNames,
                        Owner = model.PRMSModel.MMSPackage,
                        Name = "nlake",
                        ModuleName = para.ModuleName,
                        DefaultValue = para.DefaultValue,
                        Description = para.Description,
                        Maximum = para.Maximum,
                        Minimum = para.Minimum,
                        Units = para.Units
                    };
                    gv[0, 0, 0] = pck.NLAKES;
                    model.PRMSModel.MMSPackage.Parameters.Add(gv.Name, gv);
                }
                model.PRMSModel.MMSPackage.AlterLength("nlake", pck.NLAKES);
                var lake_evap_adj = model.PRMSModel.MMSPackage.Select("lake_evap_adj");
                if (lake_evap_adj != null)
                {
                    var para = lake_evap_adj as DataCubeParameter<float>;
                    for (int i = 0; i < pck.NLAKES; i++)
                    {
                        para[0][":", i] = new float[] { 1, 1, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.5f, 1.4f, 1.3f, 1.2f, 1.1f };
                    }
                }
                else
                {
                    var buf = from p in model.PRMSModel.MMSPackage.DefaultParameters where p.Name == "lake_evap_adj" select p;
                    if (buf.Any())
                    {
                        var para = buf.First();
                        DataCubeParameter<float> gv = new DataCubeParameter<float>(1, 12, pck.NLAKES, false)
                        {
                            ValueType = para.ValueType,
                            VariableType = ParameterType.Parameter,
                            Dimension = 2,
                            DimensionNames = para.DimensionNames,
                            Owner = model.PRMSModel.MMSPackage,
                            Name = "lake_evap_adj",
                            ModuleName = para.ModuleName,
                            DefaultValue = para.DefaultValue,
                            Description = para.Description,
                            Maximum = para.Maximum,
                            Minimum = para.Minimum,
                            Units = para.Units
                        };
                        for (int i = 0; i < pck.NLAKES; i++)
                        {
                            gv[0][":", i] = new float[] { 1, 1, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.5f, 1.4f, 1.3f, 1.2f, 1.1f };
                        }
                        model.PRMSModel.MMSPackage.Parameters.Add(gv.Name, gv);
                    }
                }

                model.PRMSModel.MMSPackage.IsDirty = true;
                model.PRMSModel.MMSPackage.Save(null);

                var lak = prj.Project.Model.GetPackage(LakePackage.PackageName) as LakePackage;
                lak.Attach(shell.MapAppManager.Map, prj.Project.GeoSpatialDirectory);
                shell.ProjectExplorer.ClearContent();
                shell.ProjectExplorer.AddProject(prj.Project);
            }
        }
    }
}
