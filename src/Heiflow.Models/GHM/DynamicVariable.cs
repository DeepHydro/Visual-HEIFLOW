using Heiflow.Core.IO;
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.GHM
{
    [Serializable]
    public class DynamicVariable : GHMVariable
    {
        public DynamicVariable()
        {
        }
        private DotSpatial.Data.ICancelProgressHandler _progess;
        public override bool Scan()
        {
            DataCubeStreamReader dr = new DataCubeStreamReader(this.FullPath);
            var info = dr.GetFileInfo();
            Variables = info.VariableNames;
            NumTimeStep = info.TotalTimeSteps;
            return true;
        }

        public override LoadingState Load(int var_index, DotSpatial.Data.ICancelProgressHandler progess)
        {
            _progess = progess;
            DataCubeStreamReader dr = new DataCubeStreamReader(this.FullPath);
            dr.Loading += dr_Loading;
            dr.LoadFailed += dr_LoadFailed;
            dr.DataCubeLoaded += dr_DataCubeLoaded;
            if(LoadAllVars)
            {
                dr.LoadDataCube();
            }
            else
                dr.LoadDataCube(var_index);
            return LoadingState.Normal;
        }

        void dr_DataCubeLoaded(object sender, Core.Data.DataCube<float> e)
        {
            var sate = new LoadingObjectState() { DataCube = e, State = LoadingState.Normal };
            this.DataCube = e;
            this.DataCube.DataOwner = this;
            this.DataCube.OwnerName = this.Name;
            int ntime = this.DataCube.Size[1];
            this.DataCube.DateTimes = new DateTime[ntime];
            DateTime current = this.Start;
            for (int i = 0; i < ntime; i++)
            {
                this.DataCube.DateTimes[i] = current;
                current = current.AddSeconds(TimeInteval);
            }
            if (ValueAsDepth)
            {
                var elev = (this.Owner.Grid as TriangularGrid).Elevations.GetVector(0, "0", ":");
                for (int i = 0; i < this.DataCube.Size[0]; i++)
                {
                    if (this.DataCube[i] != null)
                    {
                        for (int j = 0; j < this.DataCube.Size[1]; j++)
                        {
                            var buf = this.DataCube.GetVector(0, j.ToString(), ":");
                            for (int k = 0; k < buf.Length; k++)
                            {
                                buf[k] = -elev[k] + buf[k];
                            }
                            this.DataCube[i][j, ":"] = buf;
                        }
                    }
                }
            }
            OnLoaded(_progess, sate);
        }

        private void dr_LoadFailed(object sender, string e)
        {
            _progess.Progress("HydroEarth", 100, "Failed to load. Error message: " + e);
            LoadingObjectState state = new LoadingObjectState()
            {
                State = LoadingState.FatalError,
                Message = e
            };
            OnLoaded(_progess, state);
        }

        private void dr_Loading(object sender, int e)
        {
            OnLoading(e);
        }
    }
}
