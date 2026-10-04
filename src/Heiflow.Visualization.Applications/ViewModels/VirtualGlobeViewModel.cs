using Heiflow.Applications.Views;
using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Display;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Threading;

namespace Heiflow.Visualization.Applications
{
    [Export]
    public class VirtualGlobeViewModel : ViewModel<IVirtualGlobeView>
    {
        private readonly IVirtualGlobeView _VirtualGlobeView;
        private IVGSShellService _ShellService;
        public event EventHandler ViewLoaded;

        [ImportingConstructor]
        public VirtualGlobeViewModel(IVirtualGlobeView view, IVGSShellService shell)
            : base(view)
        {
            _VirtualGlobeView = view;
            _ShellService = shell;
        }

        public SceneWindow VirtualGlobe 
        { 
            get
            {
                return _VirtualGlobeView.VirtualGlobe;
            }
        }

        public IVGSShellService ShellService
        {
            get
            {
                return _ShellService;
            }
        }
        public void OnViewLoaded()
        {
            if(ViewLoaded != null)
            {
                ViewLoaded(this, EventArgs.Empty);
            }
        }
    }
}
