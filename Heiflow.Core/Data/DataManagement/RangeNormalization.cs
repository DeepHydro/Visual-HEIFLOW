// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.Data
{
    public class RangeNormalization:INormalizationModel
    {
        public RangeNormalization()
        {
            //RefMin = 0.1;
            //RefMax = 0.8;
            RefMin = 0.0;
            RefMax = 1.0;
        }

        public double RefMin { get; set; }
        public double RefMax { get; set; }

        double mActrualMin;
        double mActrualMax;
        #region INormalizationModel 成员

        public void Normalize(double[] vector)
        {
            if (vector != null)
            {
                mActrualMin = vector.Min();
                mActrualMax = vector.Max();
                if (mActrualMax > mActrualMin)
                {
                    for (int i = 0; i < vector.Length; i++)
                    {
                        vector[i] = Math.Round(RefMin + RefMax * (vector[i] - mActrualMin) / (mActrualMax - mActrualMin),4);
                    }
                }
            }
        }

        public void UnNormalize(double[] vector)
        {
            if (vector != null)
            {
                for (int i = 0; i < vector.Length; i++)
                {
                    double value = mActrualMin + ((vector[i] - RefMin) * (mActrualMax - mActrualMin)) / RefMax;
                    vector[i] = Math.Round(value, 4);
                }
                //mActrualMin = vector.Min();
                //mActrualMax = vector.Max();
            }
        }

        public double Max
        {
            get { return mActrualMax; }
        }

        public double Min
        {
            get { return mActrualMin; }
        }

        #endregion
    }

    public class SqrtNormalization : INormalizationModel
    {
        public SqrtNormalization()
        {
            Magnitute = 1;
        }

        double mActrualMin;
        double mActrualMax;
        public double Magnitute { get; set; }

        #region INormalizationModel 成员

        public void Normalize(double[] vector)
        {
            mActrualMin = vector.Min();
            mActrualMax = vector.Max();
            // Calculate the root of sum of squares
            double factor = 0d;
            for (int i = 0; i < vector.Length; i++)
            {
                factor += vector[i] * vector[i];
            }
            // Divide each value with the root of sum of squares
            if (factor != 0)
            {
                factor = System.Math.Sqrt(Magnitute / factor);
                for (int i = 0; i < vector.Length; i++)
                {
                    vector[i] = Math.Round(vector[i] * factor,4);
                }
            }
        }

        public void UnNormalize(double[] vector)
        {        
            double factor = 0d;
            for (int i = 0; i < vector.Length; i++)
            {
                factor += vector[i] * vector[i];
            }

            if (factor != 0)
            {
                factor = System.Math.Sqrt(Magnitute / factor);
                for (int i = 0; i < vector.Length; i++)
                {
                    vector[i] = Math.Round(vector[i] / factor,4);
                }
            }
            //mActrualMin = vector.Min();
            //mActrualMax = vector.Max();
        }

        public double Max
        {
            get { return mActrualMax; }
        }

        public double Min
        {
            get { return mActrualMin; }
        }
        #endregion
    }
}
