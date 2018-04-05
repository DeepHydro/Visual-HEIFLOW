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
    public class MFWell : CellSite
    {
        public MFWell(int id)
        {
            IFace = 0;
        }

        public float PumpingRate { get; set; }

        public float[] PumpingRates { get; set; }

        public float[] SpecifiedHeads { get; set; }
        public int IFace { get; set; }

    }
}
