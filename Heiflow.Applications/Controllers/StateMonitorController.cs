// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
