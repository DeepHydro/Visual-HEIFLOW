// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
