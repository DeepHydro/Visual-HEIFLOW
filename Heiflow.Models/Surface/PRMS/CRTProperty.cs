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

namespace Heiflow.Models.Surface.PRMS
{
    public class CRTProperty
    {
        public CRTProperty()
        {
            HRUFLG = 1;
            STRMFLG = 1;
            FLOWFLG = 1;
            VISFLG = 1;
            IPRN = 1;
            IFILL = 1;
            DPIT = 0.1f;
            OUTITMAX = 10000;
        }

        public int HRUFLG { get; set; }
        public int STRMFLG { get; set; }
        public int FLOWFLG { get; set; }
        public int VISFLG { get; set; }
        public int IPRN { get; set; }
        public int IFILL { get; set; }
        public float DPIT { get; set; }
        public int OUTITMAX { get; set; }

    }
}
