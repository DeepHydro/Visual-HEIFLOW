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

using Heiflow.Applications.Services;
using Heiflow.Applications.ViewModels;
using Heiflow.Applications.Views;
using Heiflow.Models.Generic;
using Heiflow.Models.Running;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;

namespace Heiflow.Applications.Controllers
{
    [Export]
    public  class StateMonitorController
    {
        private StateMonitorViewModel _StateMonitor;
        private RunningMonitorViewModel _RunningMonitor;
        private IProjectService _ProjectService;
    

        [ImportingConstructor]
        public StateMonitorController(StateMonitorViewModel vm, RunningMonitorViewModel running, IProjectService project)
        {
            _StateMonitor = vm;
            _RunningMonitor = running;
            _ProjectService = project;
            _StateMonitor.LoadCommand = new DelegateCommand(Load, CanLoad);
        }

        public StateMonitorViewModel ViewModel
        {
            get
            {
                return _StateMonitor;
            }
        }

        public void Initialize()
        {
            foreach (var monitor in _StateMonitor.Monitors)
            {
                monitor.Watcher.Updated += Watcher_Updated;
                PropertyChangedEventManager.AddHandler(monitor as Model, MonitorPropertyChanged, "");
            }
            var buf= from mm in _StateMonitor.Monitors where mm.MonitorName=="BasinBudgetMonitor" select mm;
            if (buf.Count() > 0)
            {
                var BasinBudget = buf.First();

                var buf1 = from mm in _StateMonitor.Monitors where mm.MonitorName == "BudgetComponentMonitor" select mm;
                if(buf1.Count() >0)
                {
                    var BudgetComponent = buf1.First();
                    BasinBudget.Partners.Add(BudgetComponent);
                    BudgetComponent.Partners.Add(BasinBudget);
                }
                //var buf2 = from mm in _StateMonitor.Monitors where mm.MonitorName == "IrrigationMonitor" select mm;
                //if (buf2.Count() > 0)
                //{
                //    var irrg = buf2.First();
                //    BasinBudget.Partners.Add(irrg);
                //}

                var buf3 = from mm in _StateMonitor.Monitors where mm.MonitorName == "MFMonitor" select mm;
                if (buf3.Count() > 0)
                {
                    var mf = buf3.First();
                    BasinBudget.Partners.Add(mf);
                }
                _StateMonitor.Monitor = BasinBudget;
            }
        }

        private void Load()
        {
            foreach (var monitor in _StateMonitor.Monitors)
            {
                monitor.Clear();
                monitor.Watcher.Load(monitor.FileName);
            }
        }

        public void Shutdown()
        {
        }

        private bool CanLoad()
        {
            bool can = true;
            foreach (var monitor in _StateMonitor.Monitors)
            {
                if (monitor.Watcher.State == RunningState.Busy)
                {
                    can = false;
                    break;
                }
            }
            return can;
        }


        private void Watcher_Updated(object sender, Models.IO.ArrayWatchObject<double> e)
        {
            (_StateMonitor.View as IStateMonitorView).UpdateView();
     //       (_RunningMonitor.View as IRunningMonitorView).UpdateView();
        }

        private void MonitorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

    }
}
