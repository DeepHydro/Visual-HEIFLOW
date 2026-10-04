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

namespace Heiflow.Models.Hydrodynamics.Susbed
{
    public  class SourceInfo
    {
        //NO1	MFH1	MQ1	MS1	NMODE	
        //支流编号、分汇类型、Q形式、S形式、进出口断面编号	
        public SourceInfo()
        {
            MFH = 1;
            MQ = 1;
            MS = 1;
            UpperSectionID = 1;
            LowerSectionID = 2;
            Name = "未命名";
            SectionIDs = new List<int>();
        }

        public int NO { get; set; }
        public int MFH { get; set; }
        public int MQ { get; set; }
        public int MS { get; set; }
        public int UpperSectionID { get; set; }
        public int LowerSectionID { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// ID starts from 1
        /// </summary>
        public List<int> SectionIDs { get; private set; }


    }
}
