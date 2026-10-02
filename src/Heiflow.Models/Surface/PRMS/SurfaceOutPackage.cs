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

    public class SurfaceOutPackage : DataPackage
    {
        protected MasterPackage _master;

        public SurfaceOutPackage()
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
            protected set;
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
            DataCubeStreamReader stream = new DataCubeStreamReader(FileName);
            var info = stream.GetFileInfo();
            Variables = info.VariableNames;
            FeatureCount = info.CellNum;
            NumTimeStep = info.TotalTimeSteps;
            _StartLoading = TimeService.Start;
            MaxTimeStep = NumTimeStep;
            return true;
        }

        public override LoadingState Load(ICancelProgressHandler progress)
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
            return LoadingState.Normal;
        }

        public override LoadingState Load(int var_index, ICancelProgressHandler progress)
        {
            _ProgressHandler = progress;
            NumTimeStep = TimeService.GetIOTimeLength(this.Owner.WorkDirectory);
            string filename = this.FileName;
            if (UseSpecifiedFile)
                filename = SpecifiedFileName;
            int nstep = StepsToLoad;

            if (DataCube == null)
            {
                DataCube = new DataCube<float>(Variables.Length, nstep, FeatureCount, true);
                DataCube.Name = "surface_out";
            }
            else
            {
                if (DataCube.Size[1] != nstep)
                    DataCube = new DataCube<float>(Variables.Length, nstep, FeatureCount, true);
            }
            var dates = new List<DateTime>();
            for (int i = 0; i < StepsToLoad; i++)
            {
                dates.Add(this.TimeService.Start.AddMonths(i));
            }
            DataCube.Variables = this.Variables;
            DataCube.Topology = (Owner.Grid as RegularGrid).Topology;
            DataCube.DateTimes = dates.ToArray();
            DataCubeStreamReader stream = new DataCubeStreamReader(filename);
            stream.Scale = (float)this.ScaleFactor;
            stream.DataCube = this.DataCube;
            stream.MaxTimeStep = this.StepsToLoad;
            stream.NumTimeStep = this.NumTimeStep;
            stream.Loading += stream_LoadingProgressChanged;
            stream.DataCubeLoaded += stream_DataCubeLoaded;
            stream.LoadFailed += this.stream_LoadFailed;
            stream.LoadDataCube(var_index);
            return LoadingState.Normal;
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

        protected void stream_LoadingProgressChanged(object sender, int e)
        {
            OnLoading(e);
        }
        protected void stream_DataCubeLoaded(object sender, DataCube<float> e)
        {
            OnLoaded(_ProgressHandler, new LoadingObjectState());
        }
        protected void stream_LoadFailed(object sender, string e)
        {
            ShowWarning(e, _ProgressHandler);
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {

        }
    }
}
