using Heiflow.Models.Integration;
using Heiflow.Visualization.Applications;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Project
{
    [Export(typeof(IModelLayerImporter))]
    public class HeiflowModelLayerImporter : ModelLayerImporter
    {
        public HeiflowModelLayerImporter()
        {
            Name = "HEIFLOW";
        }

        public override void Import(Models.Generic.IBasicModel model, HUST.WREIS.Dot3D.World world)
        {
            MFLayerImporter mf = new MFLayerImporter()
            {
                LayerService = this.LayerService,
                ProjectService = this.ProjectService,
                ShellService = this.ShellService
            };
            mf.Initialize(world);
            mf.Import((model as HeiflowModel).ModflowModel, world);
            PRMSLayerImporter prms = new PRMSLayerImporter()
            {
                LayerService = this.LayerService,
                ProjectService = this.ProjectService,
                ShellService = this.ShellService
            };
            prms.GridLayer = mf["Grid"];
            prms.Import((model as HeiflowModel).PRMSModel, world);
        }
    }
}
