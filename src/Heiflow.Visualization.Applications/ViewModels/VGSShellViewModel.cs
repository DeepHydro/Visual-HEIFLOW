using Heiflow.Presentation.Services;
using HUST.WREIS.Dot3D;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;

namespace Heiflow.Visualization.Applications
{
    [Export]
    public class VGSShellViewModel : ViewModel<IVGSShellView>
    {

        private IVGSShellView _ShellView;
        private IVGSShellService _ShellService;
        private VGSProjectService _VGSProjectService;
        private ICommand _OpenProject;
        private ICommand _ClearProject;
        private ICommand _Exit;
        private ICommand _LoadShpFile;
        private ICommand _LoadRasterFile;
        private ICommand _Load3DModel;
        private ICommand _LoadODM;
        private ICommand _ViewCommand;
        private ICommand _MeasureCommand;
        private ICommand _ScreenShotCommand;

        [ImportingConstructor]
        public VGSShellViewModel(IVGSShellView view, IVGSShellService shell,VGSProjectService vgs)
            : base(view)
        {
            _ShellView = view;
            _ShellService = shell;
            _VGSProjectService = vgs;
            view.Closed += ViewClosed;
        }

        public IVGSShellService ShellService
        {
            get
            {
                return _ShellService;
            }
        }

        public VGSProjectService ProjectService
        {
            get
            {
                return _VGSProjectService;
            }
        }
        public IPackageUIService PackageUIService
        {
            get;
            set;
        }
        public ICommand OpenProject
        {
            get
            {
                return _OpenProject;
            }
            set
            {
                SetProperty(ref _OpenProject, value);
            }
        }
        public ICommand ClearProject
        {
            get
            {
                return _ClearProject;
            }
            set
            {
                SetProperty(ref _ClearProject, value);
            }
        }
        public ICommand Exit
        {
            get
            {
                return _Exit;
            }
            set
            {
                SetProperty(ref _Exit, value);
            }
        }
        public ICommand ViewCommand
        {
            get
            {
                return _ViewCommand;
            }
            set
            {
                SetProperty(ref _ViewCommand, value);
            }
        }
        public ICommand MeasureCommand
        {
            get
            {
                return _MeasureCommand;
            }
            set
            {
                SetProperty(ref _MeasureCommand, value);
            }
        }
        public ICommand ScreenShotCommand
        {
            get
            {
                return _ScreenShotCommand;
            }
            set
            {
                SetProperty(ref _ScreenShotCommand, value);
            }
        }

        public ICommand LoadShpFile
        {
            get
            {
                return _LoadShpFile;
            }
            set
            {
                SetProperty(ref _LoadShpFile, value);
            }
        }

        public ICommand LoadRasterFile
        {
            get
            {
                return _LoadRasterFile;
            }
            set
            {
                SetProperty(ref _LoadRasterFile, value);
            }
        }

        public ICommand Load3DModel
        {
            get
            {
                return _Load3DModel;
            }
            set
            {
                SetProperty(ref _Load3DModel, value);
            }
        }
        public ICommand LoadODM
        {
            get
            {
                return _LoadODM;
            }
            set
            {
                SetProperty(ref _LoadODM, value);
            }
        }
        public WorldSettings WorldSettings
        {
            get;
            set;
        }

        public void Show()
        {
            ViewCore.Show();
        }

        public void Close()       
        {
            _ShellService.AboutView.CloseAllowed = true;
            ViewCore.Close();
        }

        private void ViewClosed(object sender, EventArgs e)
        {
            //Settings.Default.Left = ViewCore.Left;
            //Settings.Default.Top = ViewCore.Top;
            //Settings.Default.Height = ViewCore.Height;
            //Settings.Default.Width = ViewCore.Width;
            //Settings.Default.IsMaximized = ViewCore.IsMaximized;
        }
    }
}
