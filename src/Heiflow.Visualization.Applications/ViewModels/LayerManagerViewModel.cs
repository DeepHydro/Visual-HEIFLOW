using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Input;

namespace Heiflow.Visualization.Applications
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class LayerManagerViewModel : ViewModel<ILayerManagerView>
    {
        private VGSProjectService _ProjectService;
        private IVGSShellService _ShellService;
        private LayerService _LayerService;
        private ICommand _RefreshCommand;
        private ICommand _RemoveCommand;


        [ImportingConstructor]
        public LayerManagerViewModel(ILayerManagerView view, VGSProjectService project,
            IVGSShellService shell, LayerService layer)
            : base(view)
        {
            _ProjectService = project;
            _ShellService = shell;
            _LayerService = layer;
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

        public LayerService LayerService
        {
            get
            {
                return _LayerService;
            }
        }

        public ICommand RefreshCommand
        {
            get { return _RefreshCommand; }
            set { SetProperty(ref _RefreshCommand, value); }
        }

        public ICommand RemoveCommand
        {
            get { return _RemoveCommand; }
            set { SetProperty(ref _RemoveCommand, value); }
        }

    }
}
