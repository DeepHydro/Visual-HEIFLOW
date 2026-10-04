using Heiflow.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Visualization.Applications
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class RunModelViewModel : ViewModel<IRunModelView>
    {
        private IRunModelView _AnimationView;
        private IVGSShellService _ShellService;
        private VGSProjectService _VGSProjectService;

        [ImportingConstructor]
        public RunModelViewModel(IRunModelView view, IVGSShellService shell, VGSProjectService vgs)
            : base(view)
        {
            _AnimationView = view;
            _ShellService = shell;
            _VGSProjectService = vgs;
        }

        public IVGSShellService ShellService
        {
            get
            {
                return _ShellService;
            }
        }

        public VGSProjectService VGSProjectService
        {
            get
            {
              return  _VGSProjectService;
            }
        }
    }
}
