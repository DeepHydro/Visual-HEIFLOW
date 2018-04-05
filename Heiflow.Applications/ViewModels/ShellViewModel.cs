// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Applications.Properties;
using Heiflow.Applications.Services;
using Heiflow.Applications.Views;
using Heiflow.Presentation.Services;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Waf.Applications;
using System.Windows.Input;

namespace Heiflow.Applications.ViewModels
{
    [Export]
    public class ShellViewModel : ViewModel<IShellView>
    {
        private readonly IShellService shellService;
        private readonly IMessageService messageService;
        private readonly DelegateCommand aboutCommand;
        public event CancelEventHandler Closing;
        private ICommand saveCommand;
        private ICommand exitCommand;
        private bool isValid = true;
        private string appPath = "";

        [ImportingConstructor]
        public ShellViewModel(IShellView view, IShellService shellSer, IMessageService msgService)
            : base(view)
        {
            shellService = shellSer;
            messageService = msgService;
            this.aboutCommand = new DelegateCommand(ShowAboutMessage);
            view.Closing += ViewClosing;
            view.Closed += ViewClosed;
        }

        public string Title { get { return "Visual HEIFLOW"; } }

        public IShellService ShellService { get { return shellService; } }

        public ICommand AboutCommand { get { return aboutCommand; } }

        public ICommand SaveCommand
        {
            get { return saveCommand; }
            set { SetProperty(ref saveCommand, value); }
        }

        public ICommand ExitCommand
        {
            get { return exitCommand; }
            set { SetProperty(ref exitCommand, value); }
        }

        public bool IsValid
        {
            get { return isValid; }
            set { SetProperty(ref isValid, value); }
        }

        public string AppPath
        {
            get { return appPath; }
            set { SetProperty(ref appPath, value); }
        }


        public void Show()
        {
            ViewCore.Show();
        }

        public void Close()
        {
            ViewCore.Close();
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            if (Closing != null) { Closing(this, e); }
        }

        private void ViewClosing(object sender, CancelEventArgs e)
        {
            OnClosing(e);
        }

        private void ViewClosed(object sender, EventArgs e)
        {
            Settings.Default.Left = ViewCore.Left;
            Settings.Default.Top = ViewCore.Top;
            Settings.Default.Height = ViewCore.Height;
            Settings.Default.Width = ViewCore.Width;
            Settings.Default.IsMaximized = ViewCore.IsMaximized;
        }

        private void ShowAboutMessage()
        {
            messageService.ShowMessage(View, string.Format(CultureInfo.CurrentCulture,Resources.AboutText,
                ApplicationInfo.ProductName, ApplicationInfo.Version));
        }

   
    }
}
