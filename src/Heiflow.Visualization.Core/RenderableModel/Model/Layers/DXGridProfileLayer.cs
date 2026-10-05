using Heiflow.Models.Generic;
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
    public class GridProfileLayer : RenderableModelObject
    {
   
        public GridProfileLayer(string name,
            World parentWorld,
            double minDisplayAltitude,
            double maxDisplayAltitude,
            double distanceAboveSurface)
            : base(name, parentWorld.Position, Quaternion.RotationYawPitchRoll(0, 0, 0))
        {
        }


        public override void Initialize(HUST.WREIS.Dot3D.DrawArgs drawArgs)
        {
           
        }

        public override void Render(HUST.WREIS.Dot3D.DrawArgs drawArgs)
        {
            if (RenderDX == null || RenderDX.Profiles == null)
                return;

            Device device = DrawArgs.Device;

            using (ModelRenderScope.Begin(device, drawArgs, FillMode, false))
            {
                foreach (var pf in RenderDX.Profiles)
                {
                    if (pf.VertexList == null || pf.VertexIndexList == null || pf.VertexList.Length == 0)
                        continue;

                    device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, pf.VertexList.Length,
                        pf.VertexIndexList.Length / 3, pf.VertexIndexList, false, pf.VertexList);
                }
            }
        }

        public override void Update(DrawArgs drawArgs)
        {
          
        }

        public override void Dispose()
        {
            
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }
    }
}
