// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using Heiflow.Applications;
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
    public class ModelBudgetAnalysisPlugin : Extension
    {
        public static  string PanelKey = "kStateMonitor";
        private UserControl _StateMonitor;

        [Import("VHFManager", typeof(VHFAppManager))]
        public VHFAppManager Manager
        {
            get;
            set;
        }


        public override void Activate()
        {
            this._StateMonitor = Manager.StateMonitor.ViewModel.View as UserControl;
            this._StateMonitor.Name = "stateMonitor1";
            App.DockManager.Add(new DockablePanel(PanelKey, "Budget Analysis",
                _StateMonitor, DockStyle.None) { SmallImage = Properties.Resources.SpatialAnalystTrainingSampleHistograms16 });
            App.DockManager.HidePanel(PanelKey);

            var showStateMonitor = new SimpleActionItem("kModel", "Budget Analysis", delegate(object sender, EventArgs e)
            { App.DockManager.ShowPanel(PanelKey); })
            {
                Key = "kShowRStateMonitor",
                ToolTipText = "Show Budget Analysis",
                GroupCaption = "Analysis",
                LargeImage = Resources.Abacus
            };
            App.HeaderControl.Add(showStateMonitor);

            base.Activate();
        }

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            this.App.DockManager.Remove("kStateMonitor");
            base.Deactivate();
        }
    }
}
