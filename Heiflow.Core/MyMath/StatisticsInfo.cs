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

namespace Heiflow.Core.MyMath
{
    public class StatisticsInfo
    {
        public int Count { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public double Average { get; set; }
        public double NoDataValue { get; set; }
        public double StandardDeviation { get; set; }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}", Max, Min, Average, NoDataValue, StandardDeviation,Count);
        }
    }
}
