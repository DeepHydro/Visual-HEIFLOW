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
using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Packages;
using Heiflow.Models.Properties;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    public class HOBOutputPackage : MFDataPackage, ISitesProvider
    {
        public static string PackageName = "HOB Output";
        private int _nsite = 0;

        public HOBOutputPackage()
        {
            Name = PackageName;
            _FullName = "Head Observation Output";
            Sites = new List<IObservationsSite>();
            ODMVariableID = 51;
            TimeUnits = TimeUnits.Month;
            IsMandatory = false;
            _Layer3DToken = "Well";
            Category = Resources.ObsCategory; 
        }

        [Browsable(false)]
        public IList<IObservationsSite> Sites
        {
            get;
            set;
        }

        [Browsable(false)]
        [PackageOptionalViewItem("HOB Output")]
        public override IPackageOptionalView OptionalView
        {
            get;
            set;
        }

        [Browsable(false)]
        public DataCube<float> ComparingValues
        {
            get;
            set;
        }

        public int SelectedLayerIndex
        {
            get;
            set;
        }
        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            base.Initialize();
        }
        public override bool Scan()
        {
            if (File.Exists(FileName))
            {
                Variables = new string[] { "Groundwater Table"};
                var buf = from pp in Owner.Packages[MFOutputPackage.PackageName].Children where pp.Name == FHDPackage.PackageName select pp;
                if (buf.Count() == 1)
                {
                    var _fhd = buf.First() as FHDPackage;
                    NumTimeStep = _fhd.NumTimeStep;
                    StartOfLoading = _fhd.StartOfLoading;
                    EndOfLoading = _fhd.EndOfLoading;
                    MaxTimeStep = _fhd.MaxTimeStep;
                    Variables = new string[] { _fhd.Variables[0] };
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override LoadingState Load(ICancelProgressHandler progress)
        {
            var result = LoadingState.Normal;
            _ProgressHandler = progress;
            result = ExtractTo() ? LoadingState.Normal : LoadingState.Warning;
            OnLoaded(progress, new LoadingObjectState() { State = result });
            return result;
        }
        public override LoadingState Load(int var_index, ICancelProgressHandler progress)
        {
            var result = LoadingState.Normal;
            _ProgressHandler = progress;
            result = ExtractTo() ? LoadingState.Normal : LoadingState.Warning;
            OnLoaded(progress, new LoadingObjectState() { State = result });
            return result;
        }
        public override void Clear()
        {
            if (_Initialized)
            {
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            }
            base.Clear();
        }
        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Parent.Feature;
            this.FeatureLayer = Parent.FeatureLayer;
        }
        public bool ExtractTo()
        {
            if (NumTimeStep == 0)
                Scan();
            if (Feature != null)
            {
                var hob = Parent as HOBPackage;
                if (Sites == null || Sites.Count == 0)
                    Sites = hob.Observations;
                if (Sites == null)
                {
                    goto error;
                }
                var buf = Owner.GetPackage(FHDPackage.PackageName);
                if (buf != null)                
                {
                    var fhd = buf as FHDPackage;
                    foreach (HeadObservation site in Sites)
                    {
                        site.Variables = new Core.Data.ODM.Variable[] { new Core.Data.ODM.Variable() { Name = fhd.Variables[SelectedLayerIndex] } };
                    }
                    if (fhd.DataCube != null && fhd.DataCube[SelectedLayerIndex] != null)
                    {
                        int nstep = StepsToLoad;
                        OnLoading(50);
                        DataCube = fhd.ExtractTo(Sites, SelectedLayerIndex);
                        DataCube.Topology = hob.Topology;
                        OnLoaded(_ProgressHandler, new LoadingObjectState());
                        return true;
                    }
                    else
                    {
                        goto error;
                    }
                }
                else
                {
                    goto error;
                }
            }
            else
            {
                goto error;
            }
            error:
            {
                State = ModelObjectState.Error;
                OnLoaded(_ProgressHandler, new LoadingObjectState() { State = LoadingState.Warning });
                return false;
            }
        }
        public void LoadComparingValues()
        {
            OnLoading(0);
            var grid = Owner.Grid as MFGrid;
            int nstep = _nsite;
            int progress = 0;

            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName);
                ComparingValues = new DataCube<float>(2, 1, _nsite);
                ComparingValues.Allocate(0, 1, _nsite);
                ComparingValues.Allocate(1, 1, _nsite);
                string line = sr.ReadLine();
                for (int i = 0; i < _nsite; i++)
                {
                    line = sr.ReadLine();
                    if (!TypeConverterEx.IsNull(line))
                    {
                        var vv = TypeConverterEx.Split<float>(line);
                        ComparingValues[0,0,i] = vv[0];
                        ComparingValues[1,0,i] = vv[1];
                    }
                    progress = Convert.ToInt32(i * 100 / nstep);
                    OnLoading(progress);
                }
                if (progress < 100)
                    OnLoading(100);
                ComparingValues.DateTimes = new DateTime[] { TimeService.Timeline[0] };
                OnLoaded(_ProgressHandler, new LoadingObjectState());
                sr.Close();
            }
        }
        public float[] GetHeads(int var_index, int time_index)
        {
            if (DataCube != null && Sites != null)
            {
                int nsites = Sites.Count;
                float[] heads = new float[nsites];

                for (int i = 0; i < nsites; i++)
                {
                    heads[i] = DataCube[var_index,time_index,i];
                }
                return heads;
            }
            else
            {
                return null;
            }
        }
 
    }
}