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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Data;
using Heiflow.Core.MyMath;

namespace Heiflow.Models.Subsurface
{
    public class MFFlowField
    {
        public MFFlowField(MFGrid grid)
        {
            mfgrid = grid;
        }

        private MFGrid mfgrid;

        public DataCube<float> Direction { get; private set; }

        public DataCube<float> Metric { get; private set; }

        public void ComputeHorizontal(DataCube<float> cbc, int dim1, int dim2)
        {
            //cbc.Value
            int steps = cbc.Size[1];
            int nfea = cbc.Size[2];
            Direction = new DataCube<float>(1, steps, nfea,false);
            Metric = new DataCube<float>(1, steps, nfea, false);

            for (int s = 0; s < steps; s++)
            {
                for (int i = 0; i < nfea; i++)
                {
                    var r = Math.Sqrt(cbc[dim1,steps,i] * cbc[dim1,steps,i]
                        + cbc[dim2,steps,i] * cbc[dim2,steps,i]);
                    if (r == 0)
                        r = 1;
                    if (r < 0)
                        r = 1;
                    Direction[0,s,i] = (float)(Math.Asin(cbc[dim2,steps,i] / r)); //* 57.29578
                    Metric[0,s,i] = (float)r;
                }
            }
        }

        public void Scale(float min, float max, int step, int sdNumber)
        {
            var vv = Metric.GetVector(0, step.ToString(),":");
            var stat = MyStatisticsMath.SimpleStatistics(vv);
            float vmax = (float)(stat.Average + sdNumber * stat.StandardDeviation);
            float vmin = (float)(stat.Average - sdNumber * stat.StandardDeviation);
            float vdelta = vmax - vmin;
            float ndelta = max - min;
            float value = 0;
            for (int i = 0; i < vv.Length; i++)
            {
                value = Metric[0,step,i];
                if (value < vmin)
                    Metric[0, step, i] = min;
                else if (value > vmax)
                    Metric[0, step, i] = max;
                else
                    Metric[0, step, i] = min + (value - vmin) / vdelta * ndelta;
            }
        }

        public void Scale(int step, float min, float max)
        {
            var vv = Metric.GetVector(0, step.ToString(), ":");
            var stat = MyStatisticsMath.SimpleStatistics(vv);
   
            float vmax = (float)stat.Max;
            float vmin = (float)stat.Min;
            float vdelta = vmax - vmin;
            float ndelta = max - min;

            for (int i = 0; i < vv.Length; i++)
            {
                Metric[0,step,i] = min + (Metric[0,step,i] - vmin) / vdelta * ndelta;
            }
        }
    }
}