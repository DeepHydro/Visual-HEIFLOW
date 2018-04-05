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
    public class DataCubeMeta
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public string Owner { get; set; }
        public IDataCubeObject Mat { get; set; }
    }

    public class DCVarientMeta
    {
        public DCVarientMeta()
        {
            //Multiplier = 1;
            //Constant = 0;
        }
        public TimeVarientFlag Behavior { get; set; }
        public int TimeIndex { get; set; }
        public double Multiplier { get; set; }
        public double Constant { get; set; }
        public IDataCubeObject Owner { get; set; }
    }
}
