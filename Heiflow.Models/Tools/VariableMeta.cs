// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Tools
{
    public class VariableMeta
    {
        public string Index { get; set; }
        public string Variable { get; set; }
        public string Size { get; set; }
        public string Max { get; set; }
        public string Min { get; set; }
        public My3DMat<float> Mat { get; set; }
    }
}
