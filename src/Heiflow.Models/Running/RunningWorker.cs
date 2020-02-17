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
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using Heiflow.Models.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Running
{
    public enum RunningState { Busy, Stopped }
    public class RunningWorker : Heiflow.Models.Running.IRunningWorker
    {
        private Process workProcess;
        private BackgroundWorker worker;
        private string _Exepath;
        private string _WorkingDirectory;
        public event EventHandler<string> Echoed;
        public event EventHandler Completed;

        public RunningWorker()
        {
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(RunOnBGThread);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGThreadWorkDone);
 
        }

        public bool IsBusy
        {
            get 
            {
                return worker.IsBusy;
            }
        }

        public string ExePath
        {
            get
            {
                return _Exepath;
            }
            set
            {
                _Exepath = value;
            }
        }

        public string WorkingDirectory
        {
            get
            {
                return _WorkingDirectory;
            }
            set
            {
                _WorkingDirectory = value;
            }
        }

        public string OutputInfo
        {
            get;
           private set;
        }

        public void Start(object arg)
        {
            worker.RunWorkerAsync(arg);
        }

        public void Stop()
        {
            if (worker.IsBusy)
            {
                workProcess.Kill();
                worker.CancelAsync();
            }
            else
            {
                workProcess.Kill();
            }
        }

        private void RunOnBGThread(object sender, DoWorkEventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                FileName = _Exepath,
                UseShellExecute = false, 
                ErrorDialog = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                WorkingDirectory = _WorkingDirectory,
                WindowStyle = ProcessWindowStyle.Hidden,         
                Arguments = e.Argument.ToString(),
            };
            workProcess = Process.Start(info);
 
            //var automator = new ConsoleAutomator(workProcess.StandardInput, workProcess.StandardOutput);
            //// AutomatorStandardInputRead is the event handler
            //automator.StandardInputRead += new EventHandler<ConsoleInputReadEventArgs>(automator_StandardInputRead);
            //automator.StartAutomate();
            //// do whatever you want while that process is running
            //workProcess.WaitForExit();
            //automator.StandardInputRead -= automator_StandardInputRead;

            //double elapsed = 0;
            //while (!workProcess.StandardOutput.EndOfStream)
            //{
            //    OutputInfo = workProcess.StandardOutput.ReadLine();
            //    if (OutputInfo.Contains("Elapsed run time"))
            //    {
            //      //  DataCubeService.TryParseDouble(OutputInfo, out elapsed);
            //        ModelStarted(this, elapsed);
            //        break;
            //    }
            //    OnEchoed(OutputInfo);
            //}
            while (!workProcess.StandardOutput.EndOfStream)
            {
                OutputInfo = workProcess.StandardOutput.ReadLine();
                OnEchoed(OutputInfo);
            }

            workProcess.Close();
        }

        private void automator_StandardInputRead(object sender, ConsoleInputReadEventArgs e)
        {
            OutputInfo = e.Input;
        }

        private void BGThreadWorkDone(object sender, RunWorkerCompletedEventArgs e)
        {
            OnCompleted();
        }
        public void OnEchoed(string msg)
        {
            if (Echoed != null)
            {
                Echoed(this, msg);
            }
        }
   
        public void OnCompleted()
        {
            if (Completed != null)
            {
                Completed(this,  null);
            }
        }

    }
}
