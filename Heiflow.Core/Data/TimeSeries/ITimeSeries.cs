// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections;
using System.Collections.Generic;
namespace Heiflow.Core.Data
{
    public interface ITimeSeries<T>
    {
        string Name { get; set; }
        DateTime[] DateTimes { get; set; }
        T[] GetSeriesAt(int var_index, int spatial_index);
    }
}
