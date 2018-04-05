// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;

namespace Heiflow.Presentation
{
    public  interface IConfigManager
    {
        string AppPath { get; }
        string ConfigPath { get; }
        string VHFPath { get; }

        IEnumerable<IOptionControl> OptionControls
        {
            get;
            set;
        }

        IOptionForm OptionForm
        {
            get;
            set;
        }
        void SetPath(string appPath);
    }
}
