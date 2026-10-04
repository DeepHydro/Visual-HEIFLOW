using Heiflow.Models.Generic;
using Heiflow.Models.Surface.PRMS;
using Heiflow.Visualization.Renderable.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Project
{
    [Export(typeof(IModelLayerImporter))]
    public class PRMSLayerImporter : ModelLayerImporter
    {

        public PRMSLayerImporter()
        {
            Name = "PRMS";
        }

        public IDX3DLayer GridLayer { get; set; }

        public override void Import(Models.Generic.IBasicModel model, HUST.WREIS.Dot3D.World world)
        {
            foreach (var pck in model.Packages.Values)
            {
                if (GridLayer.Token == pck.Layer3DToken)
                {
                    pck.Layer3D = GridLayer;
                }
                foreach (var ch in pck.Children)
                {
                    if (GridLayer.Token == ch.Layer3DToken)
                    {
                        ch.Layer3D = GridLayer;
                    }
                }
            }
        }
    }
}