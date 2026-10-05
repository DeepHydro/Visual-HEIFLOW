//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
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

using Heiflow.Models.UI;
using Heiflow.Presentation.Controls;
using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Heiflow.Visualization.Studio.Controls
{
    /// <summary>
    /// Progress of a long operation, see the note at the top of ProgressWindow.xaml. The work itself is
    /// still done by a BackgroundWorker and handed out through the DoWork event, exactly like the form
    /// did, so a caller neither knows nor cares which of the two windows it got.
    /// </summary>
    [Export(typeof(IProgressView))]
    public partial class ProgressWindow : MetroWindow, IProgressView, IChildView
    {
        /// <summary>The bar is a plain percentage, the bounds are the ones of the control in the XAML.</summary>
        private const int MinimumPercent = 0;
        private const int MaximumPercent = 100;
        /// <summary>Minimum time between two pumps of the dispatcher, see Pump.</summary>
        private static readonly TimeSpan PumpInterval = TimeSpan.FromMilliseconds(60);

        private readonly BackgroundWorker worker;
        private int lastPercent;
        private string lastStatus;
        private bool marquee;
        private DateTime lastPump = DateTime.MinValue;

        public event DoWorkEventHandler DoWork;
        public event EventHandler WorkCompleted;

        public ProgressWindow()
        {
            InitializeComponent();
            Closing += Window_Closing;
            EnableCancel = false;
            CloseAllowed = false;
            DefaultStatusText = "Please wait...";
            CancellingText = "Cancelling operation...";
            DialogMod = DialogMode.ShowDialog;

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        /// <summary>
        /// Background worker's result.
        /// </summary>
        public RunWorkerCompletedEventArgs Result
        {
            get;
            private set;
        }

        /// <summary>
        /// True if the user clicked the Cancel button and the background worker is still running.
        /// </summary>
        public bool CancellationPending
        {
            get { return worker.CancellationPending; }
        }

        public bool Cancel
        {
            get;
            private set;
        }

        /// <summary>
        /// Text displayed once the Cancel button is clicked.
        /// </summary>
        public string CancellingText
        {
            get;
            set;
        }

        public bool CloseAllowed
        {
            get;
            set;
        }

        public string DefaultStatusText
        {
            get { return Read(delegate { return tbStatus.Text; }); }
            set { Invoke(delegate { tbStatus.Text = value; }); }
        }

        public bool EnableCancel
        {
            get { return Read(delegate { return btnCancel.IsEnabled; }); }
            set { Invoke(delegate { btnCancel.IsEnabled = value; }); }
        }

        /// <summary>
        /// Marquee is the indeterminate bar of WPF, it is asked for while the length of the work is not
        /// known yet. The other styles are the plain bar.
        /// </summary>
        public ProgressBarStyle ProgressBarStyle
        {
            get { return marquee ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous; }
            set
            {
                marquee = value == ProgressBarStyle.Marquee;
                Invoke(delegate { prgBar.IsIndeterminate = marquee; });
            }
        }

        public bool PrograssbarVisible
        {
            get { return Read(delegate { return prgBar.Visibility == Visibility.Visible; }); }
            set
            {
                var state = value ? Visibility.Visible : Visibility.Collapsed;
                Invoke(delegate { prgBar.Visibility = state; });
            }
        }

        public bool AutoCloseWindow
        {
            get { return Read(delegate { return chbAutoClose.IsChecked == true; }); }
            set { Invoke(delegate { chbAutoClose.IsChecked = value; }); }
        }

        /// <summary>
        /// Owner of the form the old progress window showed itself on. A WPF window is not an IWin32Window,
        /// the value is kept because the callers set it, but showing the window does not need it.
        /// </summary>
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
            get { return "ProgressWindow"; }
        }

        public void ShowView(IWin32Window pararent)
        {
            Invoke(delegate
            {
                if (!IsVisible)
                {
                    Show();
                    Activate();
                }
                // Forced, or the window would come up white until the next throttled pump.
                Pump(true);
            });
        }

        public void CloseView()
        {
            Invoke(delegate { Hide(); });
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (worker.IsBusy || !CloseAllowed)
            {
                // A running operation must not be torn down by closing the window, the old form hid itself
                // in that case too.
                e.Cancel = true;
                Hide();
            }
        }

        public void Run(object arg)
        {
            Reset();
            EnableCancel = true;
            ShowView(MainForm);
            worker.RunWorkerAsync(arg);
        }

        public void Reset()
        {
            Invoke(delegate
            {
                Result = null;
                btnCancel.IsEnabled = true;
                prgBar.Value = MinimumPercent;
                lastStatus = tbStatus.Text;
                lastPercent = MinimumPercent;
                txtStatus.Clear();
            });
        }

        public void Progress(string msg)
        {
            SetProgress(msg);
        }

        public void Progress(int percent, string message)
        {
            SetProgress(percent, message);
        }

        public void Progress(string key, int percent, string message)
        {
            SetProgress(percent, message);
        }

        /// <summary>
        /// Changes the status text only.
        /// </summary>
        public void SetProgress(string status)
        {
            if (worker.CancellationPending)
                return;
            lastStatus = status;
            // The bounds are constants and not read from the bar: this runs on the thread of the caller,
            // which is the worker thread as often as not, and a dependency property of a control may not
            // be read from anywhere but the thread of the dispatcher.
            Report(MinimumPercent - 1, status);
        }

        /// <summary>
        /// Changes the progress bar value only.
        /// </summary>
        public void SetProgress(int percent)
        {
            if (percent == lastPercent)
                return;
            lastPercent = percent;
            Report(percent, null);
        }

        /// <summary>
        /// Changes both progress bar value and status text.
        /// </summary>
        public void SetProgress(int percent, string status)
        {
            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;
            if (percent == lastPercent && (status == lastStatus || worker.CancellationPending))
                return;
            lastPercent = percent;
            lastStatus = status;
            Report(percent, status);
        }

        /// <summary>
        /// Hands the value over to the background worker while it is running, which brings it back on the
        /// thread of the dispatcher. Outside of a run, which the callers do when something failed before
        /// the work even started, the controls are updated straight away.
        /// </summary>
        private void Report(int percent, string status)
        {
            if (worker.IsBusy)
            {
                try
                {
                    worker.ReportProgress(percent, status);
                }
                catch (InvalidOperationException)
                {
                    // The work ended between the test and the call, report it on the thread directly.
                    ReportHere(percent, status);
                }
            }
            else
            {
                ReportHere(percent, status);
            }
        }

        private void ReportHere(int percent, string status)
        {
            Invoke(delegate
            {
                // The message is shown at the top as well as in the log, it is the line the user looks at
                // to see what the load is doing, and it was left at the default text before.
                if (!string.IsNullOrEmpty(status))
                    tbStatus.Text = status;
                if (percent >= MinimumPercent && percent <= MaximumPercent)
                    prgBar.Value = percent;
                AppendLog(status);
            });
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (DoWork != null)
                DoWork(this, e);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= MinimumPercent && e.ProgressPercentage <= MaximumPercent)
                prgBar.Value = e.ProgressPercentage;
            if (e.UserState != null && !worker.CancellationPending)
                AppendLog(e.UserState.ToString());
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnCancel.IsEnabled = false;
            Result = e;
            if (e.Error != null)
                AppendLog("Failed. Error message: " + e.Error.Message);
            else if (e.Cancelled)
                AppendLog("Cancelled");

            if (WorkCompleted != null)
                WorkCompleted(this, EventArgs.Empty);

            if (AutoCloseWindow)
                CloseView();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel = true;
            worker.CancelAsync();
            EnableCancel = false;
            AppendLog(CancellingText);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseView();
        }

        private void AppendLog(string info)
        {
            if (string.IsNullOrEmpty(info))
                return;
            txtStatus.AppendText(Environment.NewLine + DateTime.Now + ": " + info);
            txtStatus.ScrollToEnd();
        }

        /// <summary>Runs a read of a control, which may be asked for from the worker thread.</summary>
        private T Read<T>(Func<T> read)
        {
            try
            {
                if (Dispatcher.CheckAccess())
                    return read();
                return (T)Dispatcher.Invoke(read);
            }
            catch (Exception ex)
            {
                Fail(ex);
                return default(T);
            }
        }

        /// <summary>Runs an update of a control, which may come from the worker thread.</summary>
        private void Invoke(Action action)
        {
            // The window only reports, it must never throw at the caller: opening a project catches every
            // exception of the loader and gives up on the whole project when one is caught, so a window
            // that failed to paint a message would cost the user the layers of the project.
            Action update = delegate
            {
                try
                {
                    action();
                    Pump(false);
                }
                catch (Exception ex)
                {
                    Fail(ex);
                }
            };

            if (Dispatcher.CheckAccess())
            {
                update();
            }
            else
            {
                // Posted and not waited for: the thread that does the work must never be held up by the
                // window, for a caller that keeps a lock the window needs would freeze them both.
                Dispatcher.BeginInvoke(update);
            }
        }

        private static void Fail(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("ProgressWindow: " + ex.Message);
        }

        /// <summary>
        /// Lets the window paint while the caller is still working. Opening a project loads on the thread
        /// of the window itself, and neither the bar nor a line of the log would be seen before the load
        /// is over unless the dispatcher is pushed here, which is what DoEvents did for the old form.
        /// The pumps are throttled to one every PumpInterval so that a caller reporting a thousand times
        /// does not spend its time painting.
        /// </summary>
        private void Pump(bool force)
        {
            // Nothing to paint before the window is up, and a frame pushed while the window is being
            // composed, which is what happened when the shell created it, would run operations of the
            // shell in the middle of its own start up.
            if (!Dispatcher.CheckAccess() || !IsVisible)
                return;
            var now = DateTime.Now;
            if (!force && now - lastPump < PumpInterval)
                return;
            lastPump = now;

            try
            {
                // The frame ends on a Loaded operation, so only the layout, the render and the binding of
                // the new value run while it is pushed. Input is below that priority and is left alone, or
                // a click of the user would start a second command in the middle of the load.
                var frame = new DispatcherFrame();
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new DispatcherOperationCallback(ExitFrame), frame);
                Dispatcher.PushFrame(frame);
            }
            catch (Exception ex)
            {
                Fail(ex);
            }
        }

        private static object ExitFrame(object state)
        {
            ((DispatcherFrame)state).Continue = false;
            return null;
        }

        public void ClearContent()
        {
            Reset();
        }

        public void InitService()
        {
        }
    }
}
