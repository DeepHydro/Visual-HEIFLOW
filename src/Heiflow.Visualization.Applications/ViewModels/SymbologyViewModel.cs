using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Visualization.Applications
{
    [Export]
    public class SymbologyViewModel : ViewModel<ISymbologyView>
    {
        private VGSProjectService _ProjectService;
        private IVGSShellService _ShellService;

        [ImportingConstructor]
        public SymbologyViewModel(ISymbologyView view, VGSProjectService project, IVGSShellService shell)
            : base(view)
        {
            _ProjectService = project;
            _ShellService = shell;
        }

        public VGSProjectService ProjectService
        {
            get
            {
                return _ProjectService;
            }
        }
        public IVGSShellService ShellService
        {
            get
            {
                return _ShellService;
            }
        }

    }
}
