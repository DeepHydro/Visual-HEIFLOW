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
using System.Data;

namespace Heiflow.Core.Data
{
    public interface IVectorTimeSeries<T> : ITimeSeries<T>
    {
        T[] Value { get; set; }
        TimeSeriesPair<T>[] ToPairs();
        void From(IEnumerable<TimeSeriesPair<T>> source);
        DataTable ToDataTable(string var_name);  
    }
}
