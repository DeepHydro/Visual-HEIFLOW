using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

namespace Heiflow.Visualization.Renderable
{
    public interface IRenderableProvider
    {
        string Name { get; set; }
        byte Opacity
        {
            get;
            set;
        }

        float DistanceAboveSurface
        {
            get;
            set;
        }

        float MinimumDisplayAltitude
        {
            get;
            set;
        }
        float MaximumDisplayAltitude
        {
            get;
            set;
        }
        RenderPriority RenderPriority
        {
            get;
            set;
        }

        FillMode FillMode
        {
            get;
            set;
        }

        bool IsOn
        {
            get;
            set;
        }
        bool IsSelectable
        {
            get;
            set;
        }
        string FullFileName
        {
            get;
        }
        string RelativeFileName
        {
            get;
            set;
        }
        bool ShowAtStartup
        {
            get;
            set;
        }

        RenderableObject Load(string filename, World world);
    }
}
