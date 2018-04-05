// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Heiflow.Core
{
    [DefaultPropertyAttribute("Unit"), Browsable(true)]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class TimeRange
    {
        public TimeRange(DateTime start, DateTime end,TimeUnits tunit)
        {
            Start = start;
            End = end;
            Unit= tunit;
        }
        
        public TimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public TimeRange()
        {
            Start = DateTime.Now.AddYears(-10);
            End = DateTime.Now;
            Unit = TimeUnits.Day;
        }

        public DateTime Start
        {
            get;
            set;
        }
        public DateTime End
        {
            get;
            set;
        }
        public TimeUnits Unit
        {
            get;
            set;
        }
    }
}
