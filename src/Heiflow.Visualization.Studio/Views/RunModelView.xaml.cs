using Heiflow.Models.Running;
using Heiflow.Presentation.Controls;
using Heiflow.Visualization.Applications;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Heiflow.Visualization.Studio.Views
{
    /// <summary>
    /// RunModelView.xaml 的交互逻辑
    /// </summary>
    /// 
        [Export(typeof(IRunModelView))]
    public partial class RunModelView : MetroWindow, IRunModelView
    {
            private readonly Lazy<RunModelViewModel> viewModel;
            private BackgroundWorker worker;
        public RunModelView()
        {
            InitializeComponent();
            viewModel = new Lazy<RunModelViewModel>(() => ViewHelper.GetViewModel<RunModelViewModel>(this));
            CloseAllowed = false;
            this.Name = DockPanelNames.RunModelPanel;
            this.Closing += Window_Closing;
            this.Loaded += RunModelView_Loaded;
        }
        private int iteration = 0;
        private Process workProcess;
        public bool CloseAllowed
        {
            get;
            set;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            iteration = 0;
            string filename = viewModel.Value.VGSProjectService.Project.Name;
            SetButtonState(false);
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(RunOnBGThread);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGThreadWorkDone);
            worker.RunWorkerAsync(filename);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
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
            SetButtonState(true);
        }

        private void RunModelView_Loaded(object sender, RoutedEventArgs e)
        {

        }
        public void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CloseAllowed)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        public void ClearContents()
        {
            outputText.Clear();
        }

        private void SetButtonState(bool runEnable)
        {
            PlayButton.IsEnabled = runEnable;
            StopButton.IsEnabled = !runEnable;
        }

        private void RunOnBGThread(object sender, DoWorkEventArgs e)
        {
            string exepath = System.IO.Path.Combine(viewModel.Value.VGSProjectService.Project.FullModelWorkDirectory, "DotFVM.exe");
            ProcessStartInfo info = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                FileName = exepath,
                UseShellExecute = false,
                ErrorDialog = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                WorkingDirectory = viewModel.Value.VGSProjectService.Project.FullModelWorkDirectory,
                Arguments = e.Argument.ToString()
            };
            workProcess = Process.Start(info);

            var automator = new ConsoleAutomator(workProcess.StandardInput, workProcess.StandardOutput);

            // AutomatorStandardInputRead is the event handler
            automator.StandardInputRead += new EventHandler<ConsoleInputReadEventArgs>(automator_StandardInputRead);
            automator.StartAutomate();

            // do whatever you want while that process is running
            workProcess.WaitForExit();
            automator.StandardInputRead -= automator_StandardInputRead;
            workProcess.Close();
        }

        private void automator_StandardInputRead(object sender, ConsoleInputReadEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, (Action)(() => SetOutputText(e.Input)));
        }

        private void SetOutputText(string str)
        {
            iteration++;
            if (iteration % 200 == 0)
            {
                //outputText.Text = "";
                outputText.Clear();
                scrollViewer.ScrollToBottom();
            }
            else
            {
                outputText.Text = outputText.Text + str;
            }
            scrollViewer.ScrollToBottom();
        }
        private void BGThreadWorkDone(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { SetButtonState(true); }));
        }
    }
}
