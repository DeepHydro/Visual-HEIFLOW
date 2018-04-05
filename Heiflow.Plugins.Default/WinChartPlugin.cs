// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Plugins.Default.Properties;
using Heiflow.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Plugins.Default
{
    public class WinChartPlugin : Extension
    {
        private WinChartView _WinChart;

        public static string WinChartPluginPanel = "kWinChartPanel";

        public WinChartPlugin()
        {
            DeactivationAllowed = false;
            _WinChart = new  WinChartView();  
        }

        [Import("ProjectController", typeof(IProjectController))]
        public IProjectController ProjectManager
        {
            get;
            set;
        }


        public override void Activate()
        {
            ProjectManager.ShellService.WinChart = _WinChart;
            ProjectManager.ShellService.AddChild(_WinChart);
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }
    }
}
