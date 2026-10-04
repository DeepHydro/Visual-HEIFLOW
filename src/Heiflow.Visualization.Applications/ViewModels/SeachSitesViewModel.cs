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
    public class SeachSitesViewModel : ViewModel<ISeachSitesView>
    {
        private VGSProjectService _ProjectService;
        private LayerService _LayerService;

        [ImportingConstructor]
        public SeachSitesViewModel(ISeachSitesView view, VGSProjectService project, LayerService layer)
            : base(view)
        {
            _ProjectService = project;
            _LayerService = layer;
        }

        public VGSProjectService ProjectService
        {
            get
            {
                return _ProjectService;
            }
        }

        public LayerService LayerService
        {
            get
            {
                return _LayerService;
            }
        }
    }
}
