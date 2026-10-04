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
    public class SiteViewModel : ViewModel<ISiteView>
    {
        private LayerService _LayerService;
        private IVGSShellService _Shell;
         
        [ImportingConstructor]
        public SiteViewModel(ISiteView view, LayerService layer_service,IVGSShellService shell)
            : base(view)
        {
            _LayerService = layer_service;
            _Shell=shell;
        }

        public LayerService LayerService
        {
            get
            {
                return _LayerService;
            }
        }

        public IVGSShellService VGSShellService
        {
            get
            {
                return _Shell;
            }
        }
    }
}