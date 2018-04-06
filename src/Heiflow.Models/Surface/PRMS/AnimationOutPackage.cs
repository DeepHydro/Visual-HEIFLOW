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

using Heiflow.Core.Data;
using Heiflow.Core.IO;
using Heiflow.Models.Generic;
using Heiflow.Models.Integration;
using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Surface.PRMS
{
    public class AnimationOutPackage : DataPackage
    {
        private MasterPackage _master;

        public AnimationOutPackage()
        {
            Name = "Animation Output";
            _Layer3DToken = "RegularGrid";
        }
        [Browsable(false)]
        public MasterPackage MasterPackage
        {
            get
            {
                return _master;
            }
            set
            {
                _master = value;
          
            }
        }
        [Category("Metadata")]
        public int FeatureCount
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            State = ModelObjectState.Ready;
            _Initialized = true;
        }

        public override bool Scan()
        {
            FileName = _master.AniOutFileName;
            if (UseSpecifiedFile)
                FileName = SpecifiedFileName;
            DataCubeStreamReader stream = new DataCubeStreamReader(FileName);
            stream.StepsToLoad = StepsToLoad;
            stream.Scan();
            Variables = stream.Variables;
            NumTimeStep = stream.NumTimeStep;
            FeatureCount = stream.FeatureCount;
            _StartLoading = TimeService.Start;
            MaxTimeStep = NumTimeStep;
            Start = TimeService.Start;
            End = EndOfLoading;
            return true;
        }

        public override bool Load()
        {
            if (UseSpecifiedFile)
                FileName = SpecifiedFileName;
            DataCubeStreamReader stream = new DataCubeStreamReader(FileName);
            stream.Scale = (float)this.ScaleFactor;
            stream.StepsToLoad = StepsToLoad;
            stream.Loading += stream_LoadingProgressChanged;
            stream.Loaded += stream_Loaded;
            stream.Load();
            return true;
        }

        public override bool Load(int var_index)
        {
            int nstep = StepsToLoad;

            if (Values == null)
            {
                Values = new MyLazy3DMat<float>(Variables.Length, nstep, FeatureCount);
                
            }
            else
            {
                if (Values.Size[1] != nstep)
                    Values = new MyLazy3DMat<float>(Variables.Length, nstep, FeatureCount);
            }
            Values.Variables = this.Variables;
            Values.Topology = (Owner.Grid as RegularGrid).Topology;

            if (UseSpecifiedFile)
                FileName = SpecifiedFileName;

            DataCubeStreamReader stream = new DataCubeStreamReader(FileName);
            stream.Scale = (float)this.ScaleFactor;
            stream.Source = Values as MyLazy3DMat<float>;
            stream.StepsToLoad = StepsToLoad;
            stream.Loading += stream_LoadingProgressChanged;
            stream.Loaded += stream_Loaded;
            stream.Load(var_index);
            return true;
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }

        public override void Clear()
        {
            if (_Initialized)
            {
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            }
            State = ModelObjectState.Standby;
            _Initialized = false;
        }

        private void stream_LoadingProgressChanged(object sender, int e)
        {
            OnLoading(e);
        }

        private void stream_Loaded(object sender, MyLazy3DMat<float> e)
        {
            Values = e;
            if (Values.DateTimes == null || Values.DateTimes.Length != Values.Size[1])
            {
                Values.DateTimes = new DateTime[Values.Size[1]];
                for (int i = 0; i < Values.Size[1]; i++)
                {
                    Values.DateTimes[i] = TimeService.Timeline[i];
                }
            }
            Values.TimeBrowsable = true;
            OnLoaded(e);
        }
    }
}
