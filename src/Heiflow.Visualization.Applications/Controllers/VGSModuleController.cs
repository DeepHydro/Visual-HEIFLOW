using Heiflow.Applications;
using Heiflow.Controls;
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

        [ImportingConstructor]
        public VGSModuleController(VirtualGlobeController vgc, LayerController lc, VGSProjectController pc,
            VGSShellViewModel shellVM, IVGSShellService shell)
        {
            _VirtualGlobeController = vgc;
            _VGSShellViewModel = shellVM;
            _ShellService = shell;
            _LayerController = lc;
            _ProjectController = pc;
        }

        public void Initialize()
        {
            _ShellService.Initialize();
            _VGSShellViewModel.Exit = new DelegateCommand(Close);
            _ShellService.ShellView = _VGSShellViewModel.View;
            _VirtualGlobeController.Initialize();
            _LayerController.Initialize();
            _ProjectController.Initialize();

            var _ProgressForm = new ProgressForm(); 
            _ShellService.ProgressWindow = _ProgressForm;
            _ShellService.AddChild(_ProgressForm);
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
