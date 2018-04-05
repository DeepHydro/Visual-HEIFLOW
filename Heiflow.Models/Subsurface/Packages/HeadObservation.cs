// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data.ODM;
using Heiflow.Core.Hydrology;
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Subsurface
{
    public class HeadObservation : CellSite
    {
        public HeadObservation(int id)
        {
            ID = id;
            IREFSPFlag = -1;
            ITT = 1;
            ROFF = 0;
            COFF = 0;
        }
        public int ITT { get; set; }
        public string[] OBSNAM { get; set; }
        public int[] IREFSP { get; set; }
        public int IREFSPFlag { get; set; }
        public float[] TOFFSET { get; set; }
        public float[] HOBS { get; set; }
        public float[] HSIM { get; set; }
        public float ROFF { get; set; }
        public float COFF { get; set; }
    }
}
