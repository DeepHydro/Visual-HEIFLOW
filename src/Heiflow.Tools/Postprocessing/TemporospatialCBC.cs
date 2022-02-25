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
using Heiflow.Models.IO;
using Heiflow.Core.Data;

namespace Heiflow.Tools.Postprocessing
{
     public class TemporospatialCBC : MapLayerRequiredTool
    {
         public TemporospatialCBC()
        {
            Name = "Temporospatial Mean of CBC";
            Category = "Postprocessing";
            Description = "Get Temporospatial CBC";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            MaxTimeStep = 0;
            Layer = 0;
            NoDataValue = -9999;
            VariableIndex = 0;
            TemporalMeanDataCube = "temproal_mean";
            SpatialMeanDataCube = "spatial_mean";
            StatAllLayer = false;
            Filter = Models.Subsurface.Filter.None;
            LoadingBehavior = MFLoadingLayersBehavior.None;
        }
         private ICancelProgressHandler _ICancelProgressHandler;
         private CBCFile _CBCFile;

         [Category("Parameter")]
         [Description("Maximum time step to load. If set to zero, all time steps will be loaded")]
         public int MaxTimeStep
         {
             get;
             set;
         }
         [Category("Parameter")]
         [Description("The layer from which head will be loaded. Layer starts from 0")]
         public int Layer
         {
             get;
             set;
         }
        [Category("Parameter")]
        [Description("The layer from which head will be loaded. Variable index starts from 0")]
        public int VariableIndex
        {
            get;
            set;
        }
        [Category("Parameter")]
         [Description("If ture, all layers will be loaded")]
         public bool StatAllLayer
         {
             get;
             set;
         }
        [Category("Parameter")]
        [Description("")]
        public Filter Filter
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("")]
        public MFLoadingLayersBehavior LoadingBehavior
        {
            get;
            set;
        }
         [Category("Parameter")]
         [Description("No Data Value")]
         public float NoDataValue
         {
             get;
             set;
         }
         [Category("Output")]
         [Description("The temporal mean datacube")]
         public string TemporalMeanDataCube 
         {
             get;
             set;
         }
         [Category("Output")]
         [Description("The spatial mean datacube")]
         public string SpatialMeanDataCube
         {
             get;
             set;
         }

        public override void Initialize()
        { 
                this.Initialized = true;
                return;
        }
   
        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            var prj = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var model = prj.Project.Model;
            _ICancelProgressHandler = cancelProgressHandler;
            Modflow mf = null;
            if (model is HeiflowModel)
                mf = (model as HeiflowModel).ModflowModel;
            else if (model is Modflow)
                mf = model as Modflow;
            if (mf != null)
            {
                var buf = from pp in mf.Packages[MFOutputPackage.PackageName].Children where pp.Name == CBCPackage.PackageName select pp;
                if (buf.Any())
                {
                    var grid = (mf.Grid as MFGrid);
                    var cbcpck = buf.First() as CBCPackage;
                    _CBCFile = new CBCFile(cbcpck.FileName, grid);
                    _CBCFile.MaxTimeStep = this.MaxTimeStep;
                    _CBCFile.NoDataValue = this.NoDataValue;
                    _CBCFile.StatAllLayers = this.StatAllLayer;
                    _CBCFile.Loading += fhd_Loading;
                    _CBCFile.DataCubeLoaded += fhd_DataCubeLoaded;
                    _CBCFile.Layer = this.Layer;
                    _CBCFile.Filter = this.Filter;
                    _CBCFile.LoadingBehavior = this.LoadingBehavior;
                    _CBCFile.StatDataCube(VariableIndex);
                }
                else
                {
                    cancelProgressHandler.Progress("Package_Tool", 100, "CBC package not found.");
                }
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: Modflow is used by this tool.");
                return false;
            }
        }

        private void fhd_Loading(object sender, int e)
        {
            _ICancelProgressHandler.Progress("Package_Tool", e, "");
        }

        private void fhd_DataCubeLoaded(object sender, DataCube<float> e)
        {
            int ntime=_CBCFile.TemporalMean.Length;
            int ncell = _CBCFile.SpatialMean.Length;
            DataCube<float> tempmean = new DataCube<float>(1, ntime, 1)
            {
                Name = TemporalMeanDataCube
            };
            DataCube<float> spatialmean = new DataCube<float>(1, 1, ncell)
            {
                Name = SpatialMeanDataCube
            };
            float [] templist= new float[ntime];
            _CBCFile.TemporalMean.CopyTo(templist, 0);
            tempmean.ILArrays[0][":", 0] = templist;

            float[] splist = new float[ncell];
            _CBCFile.SpatialMean.CopyTo(splist, 0);
            spatialmean.ILArrays[0][0, ":"] = splist;

            Workspace.Add(tempmean);
            Workspace.Add(spatialmean);
            _ICancelProgressHandler.Progress("Package_Tool", 100, "Finished");
        }
    }
}
