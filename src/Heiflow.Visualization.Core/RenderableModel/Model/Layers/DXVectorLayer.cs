using HUST.WREIS.Dot3D;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Visualization.Renderable.Grid
{
    public class DXVectorLayer :RenderableModelObject, IDX3DLayer
    {

        public DXVectorLayer()
        {
            MeshType = Grid.MeshType.Line;
            Token = "Velocity Field";
        }
        public DXVectorLayer(
            string name,
            World parentWorld,
            double minDisplayAltitude,
            double maxDisplayAltitude,
            double distanceAboveSurface)
            : base(name, parentWorld.Position, Quaternion.RotationYawPitchRoll(0, 0, 0))
        {
            this.World = parentWorld;
            this.DistanceAboveSurface = distanceAboveSurface;
            this.MinDisplayAltitude = minDisplayAltitude;
            this.MaxDisplayAltitude = maxDisplayAltitude;
            MeshType = Grid.MeshType.Line;
            Token = "Velocity Field";
             
        }
        public override void Initialize(HUST.WREIS.Dot3D.DrawArgs drawArgs)
        {
            if (_RenderDX != null)
            {
                this.isInitialized = true;
                HighLightSelectedCell = true;
            }
        }

        public override void Update(HUST.WREIS.Dot3D.DrawArgs drawArgs)
        {
            if (!this.isInitialized)
                this.Initialize(drawArgs);
            this.isInitialized = true;
        }

        public override void Render(HUST.WREIS.Dot3D.DrawArgs drawArgs)
        {
            if (this.isInitialized)
            {
                Device device = DrawArgs.Device;
                if (!World.Settings.EnableHighPerfomance)
                    device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);
                device.RenderState.ZBufferEnable = true; 
                device.VertexFormat = CustomVertex.PositionNormalColored.Format;
                device.TextureState[0].ColorOperation = TextureOperation.Disable;

                device.Transform.World = Matrix.Translation(
                (float)(-drawArgs.WorldCamera.ReferenceCenter.X),
                (float)(-drawArgs.WorldCamera.ReferenceCenter.Y),
                (float)(-drawArgs.WorldCamera.ReferenceCenter.Z)
                );

                if (RenderObject.VerticalExaggeration != World.Settings.VerticalExaggeration)
                {
                    RenderObject.VerticalExaggeration = World.Settings.VerticalExaggeration;
                }

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

                    drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, _RenderDX.VertexList.Length, _RenderDX.VertexIndexList.Length / 2,
    _RenderDX.VertexIndexList, false, _RenderDX.VertexList);

                device.RenderState.FillMode = fillmode;
                drawArgs.device.RenderState.CullMode = currentCull;
                drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
            }
        }

        public override void Dispose()
        {
        
        }

        public override bool PerformSelectionAction(HUST.WREIS.Dot3D.DrawArgs drawArgs)
        {
            return false;
        }
    }
}
