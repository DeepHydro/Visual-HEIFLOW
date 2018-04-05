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
