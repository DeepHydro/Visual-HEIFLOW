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

namespace Heiflow.Core.Data.ODM
{
    public  class ObservationSeries
    {

        public ObservationSeries()
        {

        }

        public int SiteID { get; set; }

        public int VariableID { get; set; }

        public Site Site { get; set; }

        public Variable Variable { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
