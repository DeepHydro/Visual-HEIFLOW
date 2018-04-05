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
