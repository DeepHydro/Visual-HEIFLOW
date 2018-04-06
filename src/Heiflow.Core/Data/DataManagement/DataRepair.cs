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
    public class DataRepairer
    {
        public DataRepairer(double noDataValue)
        {
            NoDataValue = noDataValue;
        }

        private double NoDataValue;

        public void Repair(NumericalSeriesPair[] pairs)
        {
            if (pairs != null)
            {
                var rawseries = (from p in pairs  select p.Value).ToArray();
                var newseries = from p in pairs where p.Value != NoDataValue select p.Value;
                double mean = NoDataValue;
                if (newseries.Any())
                {
                    mean = newseries.Average();
                }
                
                for (int i = 0; i < pairs.Length; i++)
                {
                    if (pairs[i].Value == NoDataValue)
                    {
                        pairs[i].Value = FindNearestValue(rawseries, i, mean, NoDataValue);
                    }
                }
            }
        }

        public void Repair(IVectorTimeSeries<double> ts)
        {
            if (ts != null)
            {
                var newdata = from d in ts.Value where d != NoDataValue select d;
                double mean = newdata.Average();

                for (int i = 0; i < ts.Value.Length; i++)
                {
                    if (ts.Value[i] == NoDataValue)
                    {
                        ts.Value[i] = FindNearestValue(ts.Value, i, mean, NoDataValue);
                    }
                }
            }
        }

        public void Repair(IVectorTimeSeries<double> ts, double multiplier)
        {
            if (ts != null)
            {
                var newdata = from d in ts.Value where d != NoDataValue select d;
                double mean = newdata.Average();

                for (int i = 0; i < ts.Value.Length; i++)
                {
                    if (ts.Value[i] == NoDataValue)
                    {
                        ts.Value[i] = FindNearestValue(ts.Value, i, mean, NoDataValue);
                    }
                    ts.Value[i] *= multiplier;
                }
            }
        }

        private double FindNearestValue(double[] data, int index, double defaultValue, double noDataValue)
        {
            if (index == 0)
            {
                return defaultValue;
            }
            else
            {
                if (data[index - 1] != noDataValue)
                {
                    return data[index - 1];
                }
                else
                {
                    return FindNearestValue(data, index - 1, defaultValue, noDataValue);
                }
            }
        }
    }
}
