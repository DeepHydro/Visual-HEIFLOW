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
            if (RenderDX.Profiles != null)
            {
                Device device = DrawArgs.Device;
               // device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);
                device.RenderState.ZBufferEnable = true;
                device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                device.TextureState[0].ColorOperation = TextureOperation.Disable;

                device.Transform.World = Matrix.Translation(
                (float)(-drawArgs.WorldCamera.ReferenceCenter.X),
                (float)(-drawArgs.WorldCamera.ReferenceCenter.Y),
                (float)(-drawArgs.WorldCamera.ReferenceCenter.Z)
                );

                if (World.Settings.EnableSunShading)
                {
                    Point3d sunPosition = SunCalculator.GetGeocentricPosition(TimeKeeper.CurrentTimeUtc);
                    Vector3 sunVector = new Vector3(
                        (float)sunPosition.X,
                        (float)sunPosition.Y,
                        (float)sunPosition.Z);

                    device.RenderState.Lighting = true;
                    Material material = new Material();
                    material.Diffuse = System.Drawing.Color.White;
                    material.Ambient = System.Drawing.Color.White;

                    device.Material = material;
                    device.RenderState.AmbientColor = World.Settings.ShadingAmbientColor.ToArgb();
                    device.RenderState.NormalizeNormals = true;
                    device.RenderState.AlphaBlendEnable = true;

                    device.Lights[0].Enabled = true;
                    device.Lights[0].Type = LightType.Directional;
                    device.Lights[0].Diffuse = System.Drawing.Color.White;
                    device.Lights[0].Direction = sunVector;

                }
                else
                {
                    device.RenderState.Lighting = false;
                    device.RenderState.Ambient = World.Settings.StandardAmbientColor;
                }

                Cull currentCull = drawArgs.device.RenderState.CullMode;
                drawArgs.device.RenderState.CullMode = Cull.None;
                FillMode fillmode = device.RenderState.FillMode;
                device.RenderState.FillMode = FillMode;
                foreach (var pf in RenderDX.Profiles)
                {
                    drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, pf.VertexList.Length,
                     pf.VertexIndexList.Length / 3, pf.VertexIndexList, false, pf.VertexList);
                }
                device.RenderState.FillMode = fillmode;
                drawArgs.device.RenderState.CullMode = currentCull;
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
