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
