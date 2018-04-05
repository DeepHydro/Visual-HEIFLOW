//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

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
