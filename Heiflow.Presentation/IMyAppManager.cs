// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Presentation;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Presentation
{
    public interface IMyAppManager
    {
        AppMode AppMode
        {
            get;
            set;
        }

        CompositionContainer CompositionContainer
        {
            get;
            set;
        }

        string ApplicationPath
        {
            get;
            set;
        }

        IConfigManager ConfigManager
        {
            get;
            set;
        }

        bool IsExiting
        {
            get; set; 
        }
    }
}
