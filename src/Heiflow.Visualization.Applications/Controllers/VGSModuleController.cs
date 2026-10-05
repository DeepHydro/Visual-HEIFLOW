using Heiflow.Applications;
using Heiflow.Models.UI;
using Heiflow.Presentation.Controls;
using HUST.WREIS.Dot3D;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Visualization.Applications
{
    /// <summary>
    /// Responsible for the module lifecycle.
    /// </summary>
    [Export(typeof(IModuleController)), Export]
    public class VGSModuleController : IModuleController
    {
        private readonly VirtualGlobeController _VirtualGlobeController;
        private readonly VGSShellViewModel _VGSShellViewModel;
        private readonly IVGSShellService _ShellService;
        private readonly LayerController _LayerController;
        private readonly VGSProjectController _ProjectController;
        private readonly IProgressView _ProgressWindow;

        [ImportingConstructor]
        public VGSModuleController(VirtualGlobeController vgc, LayerController lc, VGSProjectController pc,
            VGSShellViewModel shellVM, IVGSShellService shell, [Import(typeof(IProgressView))] IProgressView progressWindow)
        {
            _VirtualGlobeController = vgc;
            _VGSShellViewModel = shellVM;
            _ShellService = shell;
            _LayerController = lc;
            _ProjectController = pc;
            _ProgressWindow = progressWindow;
        }

        public void Initialize()
        {
            _ShellService.Initialize();
            _VGSShellViewModel.Exit = new DelegateCommand(Close);
            _ShellService.ShellView = _VGSShellViewModel.View;
            _VirtualGlobeController.Initialize();
            _LayerController.Initialize();
            _ProjectController.Initialize();

            // The progress window is injected instead of being created here, so the module gets the WPF
            // window that the shell exports and not the Windows Forms one.
            _ShellService.ProgressWindow = _ProgressWindow;
            _ProgressWindow.MainForm = _ShellService.MainForm;
            var child = _ProgressWindow as IChildView;
            if (child != null)
                _ShellService.AddChild(child);
        }

        private void Close(object obj)
        {
            _VGSShellViewModel.Close();
        }

        public void Run()
        {
            _VGSShellViewModel.Show();
            _LayerController.Run();
        }

        public void Shutdown()
        {
            _VirtualGlobeController.ShutDown();  
        }

    }
}
