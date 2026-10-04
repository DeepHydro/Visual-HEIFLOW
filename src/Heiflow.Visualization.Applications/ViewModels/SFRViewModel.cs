using Heiflow.Models.UI;
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
    public class SFRViewModel : ViewModel<ISFRView>
    {
        private VGSProjectService _VGSProjectService;
        private LayerService _LayerService;

        [ImportingConstructor]
        public SFRViewModel(ISFRView view, VGSProjectService project, LayerService ls)
            : base(view)
        {
            _VGSProjectService = project;
            _LayerService = ls;
        }

        public VGSProjectService ProjectService
        {
            get
            {
                return _VGSProjectService;
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
