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
    public class TimeSeriesQueryCriteria:ITimeSeriesQueryCriteria
    {
        public TimeSeriesQueryCriteria()
        {
            Start = DateTime.MinValue;
            End = DateTime.MinValue;
        }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int SiteID{ get; set; }
        public int VariableID { get; set; }
        public HydroFeatureType SiteType { get; set; }
    }
}
