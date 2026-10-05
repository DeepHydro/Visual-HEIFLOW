using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Hydrodynamics.Susbed.SWMM
{
    public class RainGage
    {
        public RainGage(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public string RelativeFileName { get; set; }
        public string FullFileName { get; set; }
        public string ID   { get; set; }
        public DoubleTimeSeries TimeSeries { get; set; }

        public void Load()
        {
            if (File.Exists(FullFileName))
            {
                TimeSeries = new DoubleTimeSeries();
                StreamReader sr = new StreamReader(FullFileName, Encoding.Default);
                var times = new List<DateTime>();
                var vv = new List<double>();
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (TypeConverterEx.IsNotNull(line))
                    {
                        var buf = TypeConverterEx.Split<double>(line);
                        var date = new DateTime((int)buf[1], (int)buf[2], (int)buf[3], (int)buf[4], (int)buf[5], 0);
                        times.Add(date);
                        vv.Add(buf[6]);
                    }
                }
                TimeSeries.DateTimes = times.ToArray();
                TimeSeries.Values = vv.ToArray();
            }
        }
    }
}
