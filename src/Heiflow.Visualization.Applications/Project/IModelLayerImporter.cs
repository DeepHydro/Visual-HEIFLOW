using Heiflow.Models.Generic;
using Heiflow.Visualization.Applications;
using HUST.WREIS.Dot3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Project
{
    public interface IModelLayerImporter
    {
        string Name { get; }
        void Import(IBasicModel model, World world);

        VGSProjectService ProjectService
        {
            get;
            set;
        }

        LayerService LayerService
        {
            get;
            set;
        }

        IVGSShellService ShellService
        {
            get;
            set;
        }
        IGridFileFactory GridFileFactory
        {
            get;
            set;
        }

    }
}
