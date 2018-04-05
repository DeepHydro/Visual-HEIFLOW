// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications.Views;
using Heiflow.Models.Running;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Input;

namespace Heiflow.Applications.ViewModels
{

    [Export]
    public class RunningMonitorViewModel : ViewModel<IRunningMonitorView>
    {
        private IRunningWorker _RunningWorker;
        private StateMonitorViewModel _StateMonitor;
        private string _RunningInfo;
        public event EventHandler RunningStarted;
        public event EventHandler RunningFinished;
        public event EventHandler<int> ProgressChanged;
        public event EventHandler<string> MessageReported;

        [ImportingConstructor]
        public RunningMonitorViewModel(IRunningMonitorView view, StateMonitorViewModel state)
            : base(view)
        {
            _StateMonitor = state;
        }

        public ICommand Start
        {
            get;
            set;
        }

        public ICommand Stop
        {
            get;
            set;
        }
     
        public string RunningInfo
        {
            get
            {
                return _RunningInfo;
            }
            set
            {
                SetProperty(ref _RunningInfo, value);
            }
        }

        public IRunningWorker Worker
        {
            get
            {
                return _RunningWorker;
            }
            set
            {
                _RunningWorker = value;
            }
        }

        public StateMonitorViewModel StateMonitor
        {
            get
            {
                return _StateMonitor;
            }
        }

        public IProjectService ProjectService
        {
            get;
            set;
        }
 
        public string ErrorMessage
        {
            get;
            set;
        }

        public void OnRunningStarted()
        {
            if (RunningStarted != null)
                RunningStarted(_RunningWorker, EventArgs.Empty);
        }

        public void OnRunningFinished()
        {
            if (RunningFinished != null)
                RunningFinished(_RunningWorker, EventArgs.Empty);
        }

        public void OnMessageReported(string msg)
        {
            if (MessageReported != null)
                MessageReported(_RunningWorker, msg);
        }

        public void OnProgressChanged(int prg)
        {
            if (ProgressChanged != null)
                ProgressChanged(_RunningWorker, prg);
        }
    }
}
