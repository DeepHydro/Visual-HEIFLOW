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
