using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable
{
    public class ODMProvider:RenderableProvider
    {
        public ODMProvider()
        {

        }


        public override HUST.WREIS.Dot3D.Renderable.RenderableObject Load(string filename, HUST.WREIS.Dot3D.World world)
        {
            return base.Load(filename, world);
        }
    }
}
