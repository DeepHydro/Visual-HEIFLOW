// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.IO
{
    public class ConsoleAutomator : ConsoleAutomatorBase
    {
        public ConsoleAutomator(StreamWriter standardInput, StreamReader standardOutput)
        {
            this.StandardInput = standardInput;
            this.StandardOutput = standardOutput;
        }

        public void StartAutomate()
        {
            this.stopAutomation = false;
            this.BeginReadAsync();
        }

        public void StopAutomation()
        {
            this.OnAutomationStopped();
        }
    }
}
