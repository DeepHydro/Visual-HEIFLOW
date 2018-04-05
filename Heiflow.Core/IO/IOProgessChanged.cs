// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.IO
{
    public class IOProgessArgs : EventArgs
    {
        public IOProgessArgs()
        {
        }
        public int Maximum { get; set; }
        public int Minimum { get; set; }
        public int Current { get; set; }
    }

    public delegate void IOProgessChanged(object sender, IOProgessArgs args);
    public enum AnimationState { Opened, Paused, Stopped, Playing, Closed, Completed }
}
