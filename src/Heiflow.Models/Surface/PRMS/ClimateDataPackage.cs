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
using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Atmosphere;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.IO;
using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Description = "Climate data";
        }
        [Browsable(false)]
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
            StartOfLoading = TimeService.Start;
            EndOfLoading = TimeService.End;
            NumTimeStep = TimeService.IOTimeline.Count;
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
                list_files.Add(tempMax);
            }
            var tempMin = MasterPackage.TempMinFile;
            if (tempMin != null)
            {
                list_vars.Add("Minimum Temperature");
                list_files.Add(tempMin);
            }
            var ppt = MasterPackage.PrecipitationFile;
            if (ppt != null)
            {
                list_vars.Add("Precipitaiton");
                list_files.Add(ppt);
            }
            var pet = MasterPackage.PETFile;
            if (pet != null)
            {
                list_vars.Add("PET");
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
            return true;
        }
        public override bool New()
        {
            Scan();
            base.New();
            return true;
        }
        public override bool Load(ICancelProgressHandler progress)
        {
            _ProgressHandler = progress;
            OnLoaded(progress);
            _SelectedIndex = -1;
            return true;
        }

        public override bool Load(int var_index, ICancelProgressHandler progress)
        {
            _ProgressHandler = progress;
            var full_file = Path.Combine(ModelService.WorkDirectory, _FileNames[var_index]);
            var grid = Owner.Grid as MFGrid;
            _SelectedIndex = var_index;

            DataCube = new DataCube<float>(Variables.Length, StepsToLoad, grid.ActiveCellCount, true);
            DataCube.Name = "clm_input";
            DataCube.Variables = this.Variables;
            DataCube.Topology = (Owner.Grid as RegularGrid).Topology;
            DataCube.TimeBrowsable = true;
            DataCube.AllowTableEdit = false;
           
            if (MasterPackage.ClimateInputFormat == FileFormat.Text)
            {
                MMSDataFile data = new MMSDataFile(full_file);
                data.NumTimeStep = this.NumTimeStep;
                data.MaxTimeStep = this.MaxTimeStep;
                data.DataCube = this.DataCube;
                data.Loading += stream_LoadingProgressChanged;
                data.DataCubeLoaded += data_DataCubeLoaded;
                data.LoadFailed += data_LoadFailed;
                if (MasterPackage.UseGridClimate)
                    data.Load(_GridHruMapping, var_index);
                else
                    data.LoadDataCube(var_index);
            }
            else
            {
                DataCubeStreamReader stream = new DataCubeStreamReader(full_file);
                stream.NumTimeStep = this.NumTimeStep;
                stream.MaxTimeStep = this.MaxTimeStep;
                stream.DataCube = this.DataCube;
                stream.Scale = (float)this.ScaleFactor;
                stream.Loading += stream_LoadingProgressChanged;
                stream.DataCubeLoaded += data_DataCubeLoaded;
                if (MasterPackage.UseGridClimate)
                    stream.LoadDataCubeSingle(_GridHruMapping, var_index);
                else
                    stream.LoadDataCubeSingle(var_index);
            }

            return true;
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }
        public void Constant(float ppt = 0.15f, float tmax = 15, float tmin = 5, float pet = 0.1f)
        {
            var grid = this.Grid as MFGrid;
            DataCube<float> mat = new DataCube<float>(1, this.TimeService.NumTimeStep, grid.ActiveCellCount, false);
            mat.Variables = new string[] { "hru_ppt" };
            mat.DateTimes = this.TimeService.Timeline.ToArray();
            mat.ILArrays[0][":", ":"] = 0.1f;

            MMSDataFile data = new MMSDataFile(MasterPackage.PrecipitationFile);
            data.Save(mat);

            mat.Variables[0] = "hru_tmax";
            mat.ILArrays[0][":", ":"] = UnitConversion.Celsius2Fahrenheit(15);
            data = new MMSDataFile(MasterPackage.TempMaxFile);
            data.Save(mat);

            mat.Variables[0] = "hru_tmin";
            mat.ILArrays[0][":", ":"] = UnitConversion.Celsius2Fahrenheit(5);
            data = new MMSDataFile(MasterPackage.TempMinFile);
            data.Save(mat);

            mat.Variables[0] = "hru_pet";
            mat.ILArrays[0][":", ":"] = 0.15f;
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

        private void data_DataCubeLoaded(object sender, DataCube<float> e)
        {
            e.DateTimes = this.TimeService.IOTimeline.ToArray();
            OnLoaded(_ProgressHandler);
        }
        private void data_LoadFailed(object sender, string e)
        {
            OnLoadFailed(e, _ProgressHandler);
        }
    }
}
