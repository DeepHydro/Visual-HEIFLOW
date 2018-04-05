// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Heiflow.Core.Data;

namespace Heiflow.Core
{
    public interface ITimeSeriesTransform
    {
        IVectorTimeSeries<double> GetTimeSeries(ITimeSeriesQueryCriteria qc);
        IVectorTimeSeries<double> GetTransformedTimeSeries(ITimeSeriesQueryCriteria qc, double multiplier);
    }
}
