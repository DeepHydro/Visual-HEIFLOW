using HUST.WREIS.Dot3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace Heiflow.Visualization.Applications
{
    public interface IVirtualGlobeView:IView
    {
        SceneWindow VirtualGlobe { get; }
    }
}
