// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Atmosphere;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.IO;
using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Surface.PRMS
{
    public class ClimateDataPackage : DataPackage
    {
        private string[] _FileNames;
        private string _GridHruMappingFile;
        private Dictionary<int, int> _GridHruMapping;
        public const string PackageName = "Climate Input";
        private int _SelectedIndex = -1;
        private List<string> list_vars;
        public ClimateDataPackage()
        {
            Name = PackageName;
            _GridHruMapping = new Dictionary<int, int>();
            IsDirty = false;
            list_vars = new List<string>();
            _Layer3DToken = "RegularGrid";
        }

        public MasterPackage MasterPackage
        {
            get;
            set;
        }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            this.Grid.Updated += this.OnGridUpdated;
            base.Initialize();
        }
        public override bool Scan()
        {
            var tempMax = MasterPackage.TempMaxFile;
            list_vars.Clear();
            var list_files = new List<string>();
            if (tempMax != null)
            {
                list_vars.Add("Maximum Temperature");
                //list_files.Add(Path.Combine(WorkDirectory, tempMax));
                list_files.Add(tempMax);
            }
            var tempMin = MasterPackage.TempMinFile;
            if (tempMin != null)
            {
                list_vars.Add("Minimum Temperature");
               // list_files.Add(Path.Combine(WorkDirectory, tempMin));
                list_files.Add(tempMin);
            }
            var ppt = MasterPackage.PrecipitationFile;
            if (ppt != null)
            {
                list_vars.Add("Precipitaiton");
               // list_files.Add(Path.Combine(WorkDirectory, ppt));
                list_files.Add(ppt);
            }
            var pet = MasterPackage.PETFile;
            if (pet != null)
            {
                list_vars.Add("PET");
                // list_files.Add(Path.Combine(WorkDirectory, ppt));
                list_files.Add(pet);
            }

            Variables = list_vars.ToArray();
            _FileNames = list_files.ToArray();

            if (MasterPackage.UseGridClimate)
            {
                _GridHruMappingFile = Path.Combine(ModelService.WorkDirectory, MasterPackage.GridClimateFile);
                LoadMapping(); 
            }
            NumTimeStep = TimeService.NumTimeStep;
            _StartLoading = TimeService.Start;
            MaxTimeStep = NumTimeStep;
            Start = TimeService.Start;
            End = TimeService.End;
            return true;
        }
        public override bool New()
        {
            Scan();
            base.New();
            return true;
        }
        public override bool Load()
        {
            OnLoaded("");
            _SelectedIndex = -1;
            return true;
        }

        public override bool Load(int var_index)
        {
            var full_file = Path.Combine(ModelService.WorkDirectory, _FileNames[var_index]);
            var grid = Owner.Grid as MFGrid;
            _SelectedIndex = var_index;
            if (Values == null)
            {
                Values = new MyLazy3DMat<float>(Variables.Length, StepsToLoad, grid.ActiveCellCount);
            }
            else
            {
                if (Values.Size[1] != StepsToLoad)
                    Values = new MyLazy3DMat<float>(Variables.Length, StepsToLoad, grid.ActiveCellCount);
            }
            Values.Variables = this.Variables;
            Values.Topology = (Owner.Grid as RegularGrid).Topology;
            Values.TimeBrowsable = true;
            Values.AllowTableEdit = false;
            if (MasterPackage.ClimateInputFormat == FileFormat.Text)
            {
                MMSDataFile data = new MMSDataFile(full_file);
                data.Source = this.Values;
                data.StepsToLoad = this.StepsToLoad;
                data.Loading += stream_LoadingProgressChanged;
                data.Loaded += stream_Loaded;
                if (MasterPackage.UseGridClimate)
                    data.Load(_GridHruMapping, var_index);
                else
                    data.Load(var_index);
            }
            else
            {
                DataCubeStreamReader stream = new DataCubeStreamReader(full_file);
                stream.Source = this.Values;
                stream.Scale = (float)this.ScaleFactor;
                stream.StepsToLoad = StepsToLoad;
                stream.Loading += stream_LoadingProgressChanged;
                stream.Loaded += stream_Loaded;
                if (MasterPackage.UseGridClimate)
                    stream.LoadSingle(_GridHruMapping, var_index);
                else
                    stream.LoadSingle(var_index);
            }

            return true;
        }

        public override void Attach(DotSpatial.Controls.IMap map,  string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }
        public void Constant(float ppt=0.15f, float tmax=15, float tmin=5, float pet=0.1f)
        {
            var grid = this.Grid as MFGrid;
            My3DMat<float> mat = new My3DMat<float>(1, this.TimeService.NumTimeStep, grid.ActiveCellCount);
            mat.Variables = new string[] { "hru_ppt"};
            mat.DateTimes = this.TimeService.Timeline.ToArray();
            mat.Constant(0.1f);

            MMSDataFile data = new MMSDataFile(MasterPackage.PrecipitationFile);
            data.Save(mat);

            mat.Variables[0] = "hru_tmax";
            mat.Constant(UnitConversion.Celsius2Fahrenheit(15));
            data = new MMSDataFile(MasterPackage.TempMaxFile);
            data.Save(mat);

            mat.Variables[0] = "hru_tmin";
            mat.Constant(UnitConversion.Celsius2Fahrenheit(5));
            data = new MMSDataFile(MasterPackage.TempMinFile);
            data.Save(mat);

            mat.Variables[0] = "hru_pet";
            mat.Constant(0.15f);
            data = new MMSDataFile(MasterPackage.PETFile);
            data.Save(mat);
        }

        public override void Clear()
        {
            if (_Initialized)
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            base.Clear();
        }

        public override void OnGridUpdated(IGrid sender)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }

        public override void OnTimeServiceUpdated(ITimeService time)
        {
             
        }

        private void LoadMapping()
        {
            if (File.Exists(_GridHruMappingFile))
            {
                _GridHruMapping.Clear();
                StreamReader sr = new StreamReader(_GridHruMappingFile);
                int nfea = 0;
                string line = "";
                line = sr.ReadLine();
                line = sr.ReadLine();
                var strs = TypeConverterEx.Split<string>(line);
                nfea = int.Parse(strs[1]);
                for (int i = 0; i < nfea; i++)
                {
                    line = sr.ReadLine();
                    var vv = TypeConverterEx.Split<int>(line);
                    _GridHruMapping.Add(vv[1], vv[0]);
                }
                sr.Close();
            }
        }

        private void stream_LoadingProgressChanged(object sender, int e)
        {
            OnLoading(e);
        }

        private void stream_Loaded(object sender, MyLazy3DMat<float> e)
        {
            OnLoaded(e);
        }
    }
}
