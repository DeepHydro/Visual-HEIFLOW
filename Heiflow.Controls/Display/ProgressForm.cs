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

using DevExpress.XtraSplashScreen;
using Heiflow.Applications;
using Heiflow.Models.UI;
using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Heiflow.Controls
{
    [Export(typeof(IProgressView))]
    public partial class ProgressForm : Form, IProgressView, IChildView
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
   
        private  BackgroundWorker worker;
        private int lastPercent;
        private string lastStatus;
        /// <summary>
        /// Delegate for the DoWork event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Contains the event data.</param>
        //   public delegate void DoWorkEventHandler(ProgressForm sender, DoWorkEventArgs e);
        /// <summary>
        /// Occurs when the background worker starts.
        /// </summary>
        public event DoWorkEventHandler DoWork;
        public event EventHandler WorkCompleted;

        public ProgressForm()
        {
            InitializeComponent();
            EnableCancel = true;
            DefaultStatusText = "Please wait...";
            CancellingText = "Cancelling operation...";
            buttonCancel.Visible = false;
            this.ShowInTaskbar = false;
            this.FormClosing += ProgressForm_FormClosing;
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            DialogMod = DialogMode.ShowDialog;

            checkBox1.Checked = true;
        }

        /// <summary>
        /// Gets the progress bar so it is possible to customize it
        /// before displaying the form.
        /// Do not use it directly from the background worker function!
        /// </summary>
        public ProgressBar ProgressBar { get { return progressBar1; } }

        /// <summary>
        /// Background worker's result.
        /// You may also check ShowDialog return value
        /// to know how the background worker finished.
        /// </summary>
        public RunWorkerCompletedEventArgs Result { get; private set; }
        /// <summary>
        /// True if the user clicked the Cancel button
        /// and the background worker is still running.
        /// </summary>
        public bool CancellationPending
        {
            get { return worker.CancellationPending; }
        }
        /// <summary>
        /// Text displayed once the Cancel button is clicked.
        /// </summary>
        public string CancellingText { get; set; }
        /// <summary>
        /// Default status text.
        /// </summary>
        public string DefaultStatusText
        {
            get
            {
                return _lbPrg.Text;
            }
            set
            {
                _lbPrg.Text = value;
            }
        }

        public bool EnableCancel
        {
            get
            {
                return buttonCancel.Enabled;
            }
            set
            {
                buttonCancel.Enabled = value; 
            }
        }

        public ProgressBarStyle ProgressBarStyle
        {        
            get
            {
                return progressBar1.Style;
            }
            set
            {
                this.Invoke((MethodInvoker)delegate { progressBar1.Style = value; });     
            }
        }

        public IWin32Window MainForm
        {
            get;
            set;
        }

        public DialogMode DialogMod
        {
            get;
            set;
        }
        public string ChildName
        {
            get { return "ProgressForm"; }
        }

        public void ShowView(IWin32Window pararent)
        {
            this.Show(MainForm);
            SetForegroundWindow(this.Handle);
        }
        private  void ProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (worker.IsBusy || e.CloseReason == CloseReason.UserClosing)
            {
                this.CloseView();
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        public void Run(object arg)
        {
            if (!this.Visible)
            {
                this.ShowView(this.MainForm);
                //Reset();
                worker.RunWorkerAsync(arg);          
            }
        }

        public void Reset()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
             {
                 Result = null;
                 buttonCancel.Enabled = true;
                 progressBar1.Value = progressBar1.Minimum;
                 lastStatus = DefaultStatusText;
                 lastPercent = progressBar1.Minimum;
                 _txtBoxStatus.Text = "";
             });
            }
            else
            {
                Result = null;
                buttonCancel.Enabled = true;
                progressBar1.Value = progressBar1.Minimum;
                lastStatus = DefaultStatusText;
                lastPercent = progressBar1.Minimum;
                _txtBoxStatus.Text = "";
            }
        }
 

        public void CloseView()
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Hide();
            });
        }

        public void Progress(int percent, string message)
        {
            this.Invoke((MethodInvoker)delegate { this.SetProgress(percent, message); });
        }

        public void Progress(string message)
        {
            this.Invoke((MethodInvoker)delegate { this.SetProgress(message); });
        }

        /// <summary>
        /// Changes the status text only.
        /// </summary>
        /// <param name="status">New status text.</param>
        public void SetProgress(string status)
        {
            //do not update the text if it didn't change
            //or if a cancellation request is pending
            if ( !worker.CancellationPending)
            {
                lastStatus = status;
                worker.ReportProgress(progressBar1.Minimum - 1, status);
            }
        }
        /// <summary>
        /// Changes the progress bar value only.
        /// </summary>
        /// <param name="percent">New value for the progress bar.</param>
        public void SetProgress(int percent)
        {
            //do not update the progress bar if the value didn't change
            if (percent != lastPercent)
            {
                lastPercent = percent;
                worker.ReportProgress(percent);
            }
        }
        /// <summary>
        /// Changes both progress bar value and status text.
        /// </summary>
        /// <param name="percent">New value for the progress bar.</param>
        /// <param name="status">New status text.</param>
        public void SetProgress(int percent, string status)
        {
            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;
            //update the form is at least one of the values need to be updated
            if (percent != lastPercent || (status != lastStatus && !worker.CancellationPending))
            {
                lastPercent = percent;
                lastStatus = status;
                worker.ReportProgress(percent, status);
            }
        }

        public void ClearContent()
        {
            Reset();
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            //notify the background worker we want to cancel
            worker.CancelAsync();
            //disable the cancel button and change the status text
            buttonCancel.Enabled = false;
            _txtBoxStatus.AppendText("\r\n" + DateTime.Now + ": " + CancellingText);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //the background worker started
            //let's call the user's event handler
            if (DoWork != null)
            {
                DoWork(this, e);
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //make sure the new value is valid for the progress bar and update it
            if (e.ProgressPercentage >= progressBar1.Minimum && e.ProgressPercentage <= progressBar1.Maximum)
                progressBar1.Value = e.ProgressPercentage;
            //do not update the text if a cancellation request is pending
            if (e.UserState != null && !worker.CancellationPending)
            {
                string info = e.UserState.ToString();
                if (!String.IsNullOrEmpty(info))
                {
                    _txtBoxStatus.AppendText("\r\n" + DateTime.Now + ": " + info);
                }
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //the background worker completed
            //keep the resul and close the form
            Result = e;
            if (e.Error != null)
            {
                _txtBoxStatus.AppendText("\r\n" + DateTime.Now +  ": Failed. Error message: " + e.Error.Message );
                DialogResult = DialogResult.Abort;
            }
            else if (e.Cancelled)
            {
                _txtBoxStatus.AppendText("\r\n" + DateTime.Now + ": Cancelled ");
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                Reset();
                DialogResult = DialogResult.OK;
            }

            if (WorkCompleted != null)
                WorkCompleted(this, EventArgs.Empty);
            if (checkBox1.Checked)
                CloseView();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseView();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            btnClose.Visible = checkBox1.Checked;
        }
        public void InitService()
        {

        }

    }
}