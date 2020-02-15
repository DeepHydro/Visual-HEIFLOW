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

#define DEBUG
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Models.Integration;
using Heiflow.Core.IO;
using Heiflow.Core.Data;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using DotSpatial.Data;

namespace Heiflow.Models.Subsurface
{

    public class VelocityPackage : MFDataPackage
    {
        public static string PackageName = "Velocity Field";

        public VelocityPackage()
        {
            Name = PackageName;
            DimX = 1;
            DimY = 2;
#if DEBUG
            _MaxTimeStep = 10;
#else
              _MaxTimeStep = -1;
#endif
            SkippedSteps = 0;
            _Layer3DToken = "RegularGrid";
        }


        public int DimX
        {
            get;
            set;
        }
        public int DimY
        {
            get;
            set;
        }

        public CBCPackage CBCPackage
        {
            get;
            set;
        }

        public override void Initialize()
        {
            this.Grid = Owner.Grid;
            this.TimeService = Owner.TimeService;
            this.TimeService.Updated += this.OnTimeServiceUpdated;
            State = ModelObjectState.Ready;
            _Initialized = true;
        }
        public override LoadingState Load(ICancelProgressHandler progresshandler)
        {
            var cbcpck = CBCPackage;
            var result = LoadingState.Normal;
            if (cbcpck.DataCube != null)
            {
                var cbc = cbcpck.DataCube;
                int steps = cbc.Size[1];
                int nfea = cbc.Size[2];
                OnLoading(0);
                int progress = 0;
                if (DataCube == null || DataCube.Size[1] != steps)
                {
                    DataCube = new DataCube<float>(2, steps, nfea);
                    DataCube.DateTimes = new DateTime[steps];
                }

                if (cbc.IsAllocated(DimX) && cbc.IsAllocated(DimY))
                {
                    for (int s = 0; s < steps; s++)
                    {
                        for (int i = 0; i < nfea; i++)
                        {
                            var r = Math.Sqrt(cbc[DimX, s, i] * cbc[DimX, s, i]
                                + cbc[DimY, s, i] * cbc[DimY, s, i]);
                            if (r == 0)
                                r = 1;
                            if (r < 0)
                                r = 1;
                            DataCube[0, s, i] = (float)r;
                            DataCube[1, s, i] = (float)(Math.Asin(cbc[DimY, s, i] / r)); //* 57.29578
                        }
                        progress = Convert.ToInt32(s * 100 / steps);
                        OnLoading(progress);
                    }
                }
                for (int s = 0; s < steps; s++)
                {
                    DataCube.DateTimes[s] = TimeService.Timeline[s];
                }
                DataCube.Variables = this.Variables;
                DataCube.Topology = (Grid as RegularGrid).Topology;
              
                result = LoadingState.Normal;
            }
            else
            {
                result = LoadingState.Warning;
            }

            OnLoaded(progresshandler, new LoadingObjectState() { State = result });
            return result;
        }

        public override void Attach(DotSpatial.Controls.IMap map, string directory)
        {
            this.Feature = Owner.Grid.FeatureSet;
            this.FeatureLayer = Owner.Grid.FeatureLayer;
        }

        public override bool Scan()
        {
            var vv = new string[] { "Magnitude", "Direction" };
            Variables = vv;
            var cbcpck = CBCPackage;
            if (cbcpck.DataCube != null)
            {
                var cbc = cbcpck.DataCube;
                int steps = cbc.Size[1];
                _StartLoading = TimeService.Start;
                NumTimeStep = steps;
                MaxTimeStep = steps;
            }
            return true;
        }
        public override void Clear()
        {
            if (_Initialized)
                this.TimeService.Updated -= this.OnTimeServiceUpdated;
            State = ModelObjectState.Standby;
            _Initialized = false;
        }
        public override LoadingState Load(int var_index, ICancelProgressHandler progress)
        {
            return Load(progress);
        }
    }
}