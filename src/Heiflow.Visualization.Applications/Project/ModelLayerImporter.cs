using Heiflow.Models.Generic;
using Heiflow.Visualization.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Project
{
    public abstract class ModelLayerImporter : IModelLayerImporter
    {
        protected VGSProjectService _VGSProjectService;
        protected LayerService _LayerService;
        protected IVGSShellService _VGSShellService;


        public string Name
        {
            get;
            protected set;
        }

        public VGSProjectService ProjectService
        {
            get
            {
                return _VGSProjectService;
            }
            set
            {
                _VGSProjectService = value;
            }
        }

        public LayerService LayerService
        {
            get
            {
                return _LayerService;
            }
            set
            {
                _LayerService = value;
            }
        }

        public IVGSShellService ShellService
        {
            get
            {
                return _VGSShellService;
            }
            set
            {
                _VGSShellService = value;
            }
        }
        public IGridFileFactory GridFileFactory
        {
            get;
            set;
        }

        public abstract void Import(Models.Generic.IBasicModel model, HUST.WREIS.Dot3D.World world);



    }
}
