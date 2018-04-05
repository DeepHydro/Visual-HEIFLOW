// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Applications
{
    public  class MyAppManager: IMyAppManager
    {
        protected IConfigManager _ConfigManager;

        public MyAppManager()
        {
            IsExiting = false;
        }

        public static IMyAppManager Instance
        {
            get;
            set;
        }

        public Presentation.Controls.AppMode AppMode
        {
            get;
            set;
        }

        public System.ComponentModel.Composition.Hosting.CompositionContainer CompositionContainer
        {
            get;
            set;
        }

        public string ApplicationPath
        {
            get;
            set;
        }

        [Import(typeof(IConfigManager))]
        public IConfigManager ConfigManager
        {
            get
            {
                return _ConfigManager;
            }
            set
            {
                _ConfigManager = value;
            }
        }


        public bool IsExiting
        {
            get;
            set;
        }
    }
}
