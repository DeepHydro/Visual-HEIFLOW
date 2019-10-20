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
            Description = "Stores outputs of surface water model";
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
        [Category("General")]
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
            StartOfLoading = TimeService.Start;
            EndOfLoading = TimeService.End;
            NumTimeStep = TimeService.IOTimeline.Count;
            _Initialized = true;
        }

        public override bool Scan()
        {
            if (!FileName.Contains(".nhru"))
                FileName += ".nhru";
            DataCubeStreamReader stream = new DataCubeStreamReader(FileName);
            Variables = stream.GetVariables();
            FeatureCount = stream.FeatureCount;
            NumTimeStep = TimeService.GetIOTimeLength(this.Owner.WorkDirectory);
            _StartLoading = TimeService.Start;
            MaxTimeStep = NumTimeStep; 
            return true;
        }

        public override bool Load(ICancelProgressHandler progress)
        {
            _ProgressHandler = progress;
            string filename = this.FileName;
            if (UseSpecifiedFile)
                filename = SpecifiedFileName;
            NumTimeStep = TimeService.GetIOTimeLength(this.Owner.WorkDirectory);
            DataCubeStreamReader stream = new DataCubeStreamReader(filename);
            stream.Scale = (float)this.ScaleFactor;
            stream.MaxTimeStep = MaxTimeStep;
            stream.Loading += stream_LoadingProgressChanged;
            stream.DataCubeLoaded += stream_DataCubeLoaded;
            stream.LoadFailed += stream_LoadFailed;
            stream.LoadDataCube();
            return true;
        }

        public override bool Load(int var_index, ICancelProgressHandler progress)
        {
            if (!FileName.Contains(".nhru"))
                FileName += ".nhru";
            _ProgressHandler = progress;
            NumTimeStep = TimeService.GetIOTimeLength(this.Owner.WorkDirectory);
            string filename = this.FileName;
            if (UseSpecifiedFile)
                filename = SpecifiedFileName;
            int nstep = StepsToLoad;

            if (DataCube == null)
            {
                DataCube = new DataCube<float>(Variables.Length, nstep, FeatureCount,true);
                DataCube.Name = "sw_out";
            }
            else
            {
                if (DataCube.Size[1] != nstep)
                    DataCube = new DataCube<float>(Variables.Length, nstep, FeatureCount, true);
            }
            DataCube.Variables = this.Variables;
            DataCube.Topology = (Owner.Grid as RegularGrid).Topology;
            DataCube.DateTimes = this.TimeService.IOTimeline.Take(StepsToLoad).ToArray();
            DataCubeStreamReader stream = new DataCubeStreamReader(filename);
            stream.Scale = (float)this.ScaleFactor;
            stream.DataCube = this.DataCube;
            stream.MaxTimeStep = this.StepsToLoad;
            stream.NumTimeStep = this.NumTimeStep;
            stream.Loading += stream_LoadingProgressChanged;
            stream.DataCubeLoaded += stream_DataCubeLoaded;
            stream.LoadFailed += this.stream_LoadFailed;
            stream.LoadDataCube(var_index);
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
        private void stream_DataCubeLoaded(object sender, DataCube<float> e)
        {
            
            OnLoaded( _ProgressHandler);
        }
        private void stream_LoadFailed(object sender, string e)
        {
            OnLoadFailed(e,_ProgressHandler);
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            
        }
    }
}
