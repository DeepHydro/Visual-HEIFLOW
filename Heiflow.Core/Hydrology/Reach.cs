// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.Hydrology
{
    public class Reach : HydroLine
    {
        public Reach(int id)
            : base(id)
        {
            THTS = 0.3;
            THTI = 0.2;
            EPS = 4;
            IFACE = 0;
        }

        public River Parent { get; set; }

        public double THTS { get; set; }
        public double THTI { get; set; }
        public double EPS { get; set; }
        public double STRHC1 { get; set; }

        /// <summary>
        /// Row
        /// </summary>
        public int IRCH { get; set; }
        /// <summary>
        /// Column
        /// </summary>
        public int JRCH { get; set; }
        public int KRCH { get; set; }
        public int IFACE { get; set; }
        public int ISEG { get; set; }
        public int IREACH { get; set; }
    }

    //public  CONDUITS
}
