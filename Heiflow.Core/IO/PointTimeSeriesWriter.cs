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
using System.IO;
using Heiflow.Core.Data;

namespace Heiflow.Core.IO
{
    public class PointTimeSeriesWriter
    {
        public static void Save2Csv(string filename, IObservationsSite [] sites)
        {
            StreamWriter sw = new StreamWriter(filename);
            if (sites != null)
            {
                var snm = (from s in sites select s.Name).ToArray();
                string line = string.Join(",", snm);
                sw.WriteLine("Date," + line);
                var ts = sites[0].TimeSeries as IVectorTimeSeries<double>;
                int len = ts.Value.Length;

                for (int i = 0; i < len; i++)
                {
                    line = sites[0].TimeSeries.DateTimes[i] + ",";
                    for (int t = 0; t < sites.Length; t++)
                    {
                        ts = sites[t].TimeSeries as IVectorTimeSeries<double>;
                        line += ts.Value[i] + ",";
                    }
                    line = line.Trim(new char[] { ',' });
                    sw.WriteLine(line);
                }              
            }
            sw.Close();
        }
    }
}
