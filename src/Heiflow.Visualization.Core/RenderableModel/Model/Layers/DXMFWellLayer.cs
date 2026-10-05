using HUST.WREIS.Dot3D;
using HUST.WREIS.Dot3D.Renderable;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class DXMFWellLayer:RegularGridLayer
    {
        private int _OutlineColor = 0;
        public DXMFWellLayer(string name,
            World parentWorld,
            double minDisplayAltitude,
            double maxDisplayAltitude,
            double distanceAboveSurface)
            : base(name, parentWorld,minDisplayAltitude, maxDisplayAltitude, distanceAboveSurface)
        {
            _OutlineColor = System.Drawing.Color.Gray.ToArgb();
        }

        public override void Initialize(DrawArgs drawArgs)
        {
            if (RenderObject != null)
            {
                this.isInitialized = true;         
            }
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            bool selected = false;
            return selected;
        }

        public override void Update(DrawArgs drawArgs)
        {
            if (!this.isInitialized)
                this.Initialize(drawArgs);
            this.isInitialized = true;
        }
    }
}
