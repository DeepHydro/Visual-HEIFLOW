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
    public class Add3DModelViewModel : ViewModel<IAdd3DModelView>
    {
        private VGSProjectService _ProjectService;

        [ImportingConstructor]
        public Add3DModelViewModel(IAdd3DModelView view, VGSProjectService project)
            : base(view)
        {
            _ProjectService = project;
        }

        public VGSProjectService ProjectService
        {
            get
            {
                return _ProjectService;
            }
        }
    }
}
