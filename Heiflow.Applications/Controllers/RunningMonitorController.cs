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
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Applications.Controllers
{
    [Export]
    public class RunningMonitorController
    {
        private readonly RunningMonitorViewModel _RunningMonitor;
        private readonly StateMonitorViewModel _StateMonitor;
        private readonly IProjectService _ProjectService;

        [ImportingConstructor]
        public RunningMonitorController(RunningMonitorViewModel vm, StateMonitorViewModel state, IProjectService project )
        {
            _RunningMonitor = vm;
            _StateMonitor = state;
            _RunningMonitor.Start = new DelegateCommand(Start, CanStart);
            _RunningMonitor.Stop = new DelegateCommand(Stop, CanStop);
            _RunningMonitor.ProjectService = project;
            _ProjectService = project;
            _ProjectService.ProjectOpenedOrCreated += ProjectService_ProjectOpenedOrCreated;
        }

        public RunningMonitorViewModel ViewModel
        {
            get
            {
                return _RunningMonitor;
            }
        }

        public void Initialize()
        {
            _RunningMonitor.Worker = new RunningWorker();
            _RunningMonitor.Worker.Echoed += Worker_Echoed;
            _RunningMonitor.Worker.Completed += Worker_Completed;
        }

        private void Start()
        {
            _RunningMonitor.OnRunningStarted();
            var model = _ProjectService.Project.Model;
            _StateMonitor.Monitor.CurrentStep = 0;
            _StateMonitor.Monitor.Clear();
            
            string[] strs = new string[2];
            strs[0] = _ProjectService.Project.RelativeControlFileName;
            strs[1] = strs[0].Replace(".control", ".xml");
            var args = strs[0] + " " + strs[1];
            _RunningMonitor.Worker.ExePath = _ProjectService.Project.ModelExeFileName;
            _RunningMonitor.Worker.WorkingDirectory = _ProjectService.Project.FullModelWorkDirectory;
            _RunningMonitor.Worker.Start(args);
        }
        private bool CanStart()
        {
            if (_ProjectService.Project == null )
            {
                ViewModel.ErrorMessage = "No project opened ";
                return false;
            }
            if (!File.Exists(_ProjectService.Project.ModelExeFileName))
            {
                ViewModel.ErrorMessage = "The model execution file dose not exist: " + _ProjectService.Project.ModelExeFileName;
                return false;
            }

            if ( _RunningMonitor.Worker.IsBusy)
            {
                ViewModel.ErrorMessage = "Another model is running.";
                return false;
            }
 
            return true;
        }

        private void Stop()
        {
            _StateMonitor.Monitor.Stop();
            _RunningMonitor.Worker.Stop();
        }

        private bool CanStop()
        {
            return _RunningMonitor.Worker.IsBusy;
        }


        private void Worker_Echoed(object sender, string e)
        {
            _RunningMonitor.RunningInfo = e;
            if (e.Contains("Date:"))
            {
                if(!_StateMonitor.Monitor.IsStarted)
                    _StateMonitor.Monitor.Start();
                _StateMonitor.Monitor.Watcher.Update();
                _StateMonitor.Monitor.CurrentStep = _StateMonitor.Monitor.Watcher.DataSource.Dates.Count;
                _RunningMonitor.OnProgressChanged(_StateMonitor.Monitor.CurrentStep);
            }
            _RunningMonitor.OnMessageReported(e);
        }

        private void Worker_Completed(object sender, EventArgs e)
        {
            _StateMonitor.Monitor.Stop();
            _RunningMonitor.OnRunningFinished();
        }

        public void Shutdown()
        {
            if (_RunningMonitor.Worker.IsBusy)
                _RunningMonitor.Worker.Stop();
        }

        private void ProjectService_ProjectOpenedOrCreated(object sender, Models.Generic.Project.IProject e)
        {
        }
    }
}
