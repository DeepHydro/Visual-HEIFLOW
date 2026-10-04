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
    public class AnimationPlayerViewModel : ViewModel<IWPFAnimationView>
    {
        private IWPFAnimationView _AnimationView;
        private IVGSShellService _ShellService;

        [ImportingConstructor]
        public AnimationPlayerViewModel(IWPFAnimationView view, IVGSShellService shell)
            : base(view)
        {
            _AnimationView = view;
            _ShellService = shell;
           
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
