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

namespace Heiflow.Core.Schema
{
    [Serializable]
    public class WorkSpaceSettings
    {
        public WorkSpaceSettings()
        {
            TrainingPeriod = new TimeRange();
            CrossValidationPeriod = new TimeRange();
            ForecastingPeriod = new TimeRange();
        }

        [CategoryAttribute("Time Horizon"), DescriptionAttribute(""), Browsable(true)]
        
        public TimeRange TrainingPeriod
        {
            get;
            set;
        }
         [CategoryAttribute("Time Horizon"), DescriptionAttribute(""), Browsable(true)]
        public TimeRange CrossValidationPeriod
        {
            get;
            set;
        }
         [CategoryAttribute("Time Horizon"), DescriptionAttribute(""), Browsable(true)]
        public TimeRange ForecastingPeriod
        {
            get;
            set;
        }
    }
}
