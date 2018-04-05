// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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

        public My3DMat<float> Direction { get; private set; }

        public My3DMat<float> Metric { get; private set; }

        public void ComputeHorizontal(My3DMat<float> cbc, int dim1, int dim2)
        {
            //cbc.Value
            int steps = cbc.Size[1];
            int nfea = cbc.Size[2];
            Direction = new My3DMat<float>(1, steps, nfea);
            Metric = new My3DMat<float>(1, steps, nfea);

            for (int s = 0; s < steps; s++)
            {
                for (int i = 0; i < nfea; i++)
                {
                    var r = Math.Sqrt(cbc.Value[dim1][steps][i] * cbc.Value[dim1][steps][i]
                        + cbc.Value[dim2][steps][i] * cbc.Value[dim2][steps][i]);
                    if (r == 0)
                        r = 1;
                    if (r < 0)
                        r = 1;
                    Direction.Value[0][s][i] = (float)(Math.Asin(cbc.Value[dim2][steps][i] / r)); //* 57.29578
                    Metric.Value[0][s][i] = (float)r;
                }
            }
        }

        public void Scale(float min, float max, int step, int sdNumber)
        {
            var vv = Metric.GetVector(0, step, MyMath.full);
            var stat = MyStatisticsMath.SimpleStatistics(vv);
            float vmax = (float)(stat.Average + sdNumber * stat.StandardDeviation);
            float vmin = (float)(stat.Average - sdNumber * stat.StandardDeviation);
            float vdelta = vmax - vmin;
            float ndelta = max - min;
            float value = 0;
            for (int i = 0; i < vv.Length; i++)
            {
                value = Metric.Value[0][step][i];
                if (value < vmin)
                    Metric.Value[0][step][i] = min;
                else if (value > vmax)
                    Metric.Value[0][step][i] = max;
                else
                    Metric.Value[0][step][i] = min + (value - vmin) / vdelta * ndelta;
            }
        }

        public void Scale(int step, float min, float max)
        {
            var vv = Metric.GetVector(0, step, MyMath.full);
            var stat = MyStatisticsMath.SimpleStatistics(vv);
   
            float vmax = (float)stat.Max;
            float vmin = (float)stat.Min;
            float vdelta = vmax - vmin;
            float ndelta = max - min;

            for (int i = 0; i < vv.Length; i++)
            {
                Metric.Value[0][step][i] = min + (Metric.Value[0][step][i] - vmin) / vdelta * ndelta;
            }
        }
    }
}