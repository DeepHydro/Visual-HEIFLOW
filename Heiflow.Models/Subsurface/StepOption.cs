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

namespace Heiflow.Models.Subsurface
{
    public class StepOption
    {
        public StepOption(int spid, int step)
        {
            SPID = spid;
            Step = step;

            SaveBudget = true;
            SaveDarwdown = false;
            SaveHead = true;
            SaveIbound = false;
            PrintBudget = true;
            PrintDarwdown = false;
            PrintHead = false;
        }

        public int SPID { get; private set; }

        public int Step { get; private set; }
        public bool SaveHead { get; set; }
        public bool SaveDarwdown { get; set; }
        public bool SaveIbound { get; set; }
        public bool SaveBudget { get; set; }
        public bool PrintHead { get; set; }
        public bool PrintDarwdown { get; set; }
        public bool PrintBudget { get; set; }

        public override string ToString()
        {
            var str = string.Format("PERIOD {0} STEP {1}", SPID, Step);
            if (SaveHead)
                str += "\n SAVE HEAD";
            if (SaveDarwdown)
                str += "\n SAVE DRAWDOWN";
            if (SaveIbound)
                str += "\n SAVE IBOUND";
            if (SaveBudget)
                str += "\n SAVE BUDGET";
            if (PrintHead)
                str += "\n PRINT HEAD";
            if (PrintDarwdown)
                str += "\n PRINT DRAWDOWN";
            if (PrintBudget)
                str += "\n PRINT BUDGET";
            return str;
        }
    }
}
