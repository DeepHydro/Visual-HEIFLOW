// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using Heiflow.Applications;
using Heiflow.Applications.Spatial;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Plugins.Default
{
    public class ModelActiveIdentifyPlugin : Extension
    {
        private SimpleActionItem _plot_ts;
        private MapFunctionActiveIdentify _identify;

        public ModelActiveIdentifyPlugin()
        {

        }

        [Import("VHFManager", typeof(VHFAppManager))]
        public VHFAppManager Manager
        {
            get;
            set;
        }

        public override void Activate()
        {
            _plot_ts = new SimpleActionItem("kModel", "Active Datasets", PlotActiveData_Clicked)
            {
                Key = "kActivePlot",
                ToolTipText = "Plot active data set time series",
                GroupCaption = "Analysis",
                LargeImage = Properties.Resources.curve_chart
            };
            App.HeaderControl.Add(_plot_ts);
            if (_identify == null)
                _identify = new MapFunctionActiveIdentify(App, Manager );
            if (!App.Map.MapFunctions.Contains(_identify))
                App.Map.MapFunctions.Add(_identify);
        }

        private void PlotActiveData_Clicked(object sender, EventArgs e)
        {
            App.Map.Cursor = Cursors.Cross;
            _identify.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            base.Deactivate();
        }

    }
}