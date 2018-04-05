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

using DotSpatial.Controls;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;
using Heiflow.Applications;
using Heiflow.Applications.Controllers;
using Heiflow.Controls.WinForm.Display;
using Heiflow.Controls.WinForm.Processing;
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
    public class ModelRunPlugin : Extension
    {
        private SimpleActionItem runmodel;
        private SimpleActionItem stopmodel;

        private RunningMonitor _RunningMonitor;
        public static string PanelKey = "kRunningMonitor";

        [Import("VHFManager", typeof(VHFAppManager))]
        public VHFAppManager Manager
        {
            get;
            set;
        }
 
        public ModelRunPlugin()
        {
            _RunningMonitor = new RunningMonitor();
        }

        public override void Activate()
        {
            runmodel = new SimpleActionItem("kModel", "Start", RunModel_Clicked)
            {
                Key = "kRunModel",
                ToolTipText = "Start running a model",
                GroupCaption = "Running",
                LargeImage = Properties.Resources.GenericBlackRightArrowNoTail32,
                SortOrder = 4
            };
            App.HeaderControl.Add(runmodel);

            stopmodel = new SimpleActionItem("kModel", "Stop", StopModel_Clicked)
            {
                Key = "kStopModel",
                ToolTipText = "Stop running",
                GroupCaption = "Running",
                LargeImage = Properties.Resources.GenericBlackStop32,
                Enabled = false,
                SortOrder = 4
            };
            App.HeaderControl.Add(stopmodel);

            var showRunningMonitor = new SimpleActionItem("kView", "Model Running", Show_Monitor)
            {
                Key = "kShowRunningMonitor",
                ToolTipText = "Show Model Running",
                GroupCaption = "Model",
                LargeImage = Resources.ModelBuilderAutoLayout32
            };
            App.HeaderControl.Add(showRunningMonitor);
            Manager.ProjectController.ShellService.AddChild(_RunningMonitor);
            (Manager.ProjectController as ProjectController).RunningMonitorViewModel.RunningFinished += ViewModel_RunningFinished;
            base.Activate();
        }

        private void ViewModel_RunningFinished(object sender, EventArgs e)
        {
            runmodel.Enabled = true;
            stopmodel.Enabled = false;
        }

        private void RunModel_Clicked(object sender, EventArgs e)
        {
            Show_Monitor(null, null);
            var vm = Manager.RunningMonitor.ViewModel;
            if (vm.Start.CanExecute(null))
            {
                vm.Start.Execute(null);
                runmodel.Enabled = false;
                stopmodel.Enabled = true;
            }
            else
            {
                MessageBox.Show(vm.ErrorMessage, "Running Model", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void StopModel_Clicked(object sender, EventArgs e)
        {
            var vm = Manager.RunningMonitor.ViewModel;
            if (vm.Stop.CanExecute(null))
            {
                vm.Stop.Execute(null);
            }
            runmodel.Enabled = true;
            stopmodel.Enabled = false;
        }
        private void Show_Monitor(object sender, EventArgs e)
        {
            Manager.ProjectController.ShellService.SelectChild(RunningMonitor._ChildName).ShowView(Manager.ProjectController.ShellService.MainForm);

        }
        public override void Deactivate()
        {
            App.HeaderControl.Remove("kRunModel");
            App.HeaderControl.Remove("kStopModel");
            App.DockManager.Remove(PanelKey);
            base.Deactivate();
        }
    }
}
