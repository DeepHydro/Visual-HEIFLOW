// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using Heiflow.Presentation;
using Heiflow.Presentation.Controls;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace Heiflow.Applications.Controllers
{
    [Export(typeof(IConfigManager))]
    public class ConfigManager : IConfigManager
    {
        public ConfigManager()
        {

        }

        [Import(typeof(IOptionForm))]
        public IOptionForm OptionForm
        {
            get;
            set;
        }

        [ImportMany]
        public IEnumerable<IOptionControl> OptionControls
        {
            get;
            set;
        }

        public string AppPath
        {
            get;
           private  set;
        }

        public string ConfigPath
        {
            get;
            private set;
        }

        public string VHFPath
        {
            get;
            private set;
        }

        public void SetPath(string appPath)
        {
            AppPath = appPath;
            VHFPath = System.IO.Path.Combine(AppPath, "Application Extensions\\VHF");
            ConfigPath = Path.Combine(VHFPath, "Config");
        }
    }
}
