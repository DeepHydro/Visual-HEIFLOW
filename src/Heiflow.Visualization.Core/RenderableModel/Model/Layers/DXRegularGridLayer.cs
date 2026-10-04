using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.ComponentModel;
using System.Collections;
using HUST.WREIS.Dot3D.Renderable;
using Heiflow.Visualization.Renderable;
using HUST.WREIS.Dot3D;
using Heiflow.Models.Generic;
using Heiflow.Models.Visualization;
namespace Heiflow.Visualization.Renderable.Grid
{
    public enum MeshType { Line, Polygon }
    public class RegularGridLayer :RenderableModelObject
    {
            public RegularGridLayer(
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
            MeshType = Renderable.Grid.MeshType.Polygon;
             
        }

        public RegularGridLayer()
            {

            }

        public override void Initialize(DrawArgs drawArgs)
        {
            if (_RenderDX != null)
            {
                this.isInitialized = true;
                HighLightSelectedCell = true;
            }
        }

        public override void Dispose()
        {
            // _Grid = null;
            // mVertexes = null;
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            bool selected = false;
            if (this.isInitialized)
            {
                System.Drawing.Point pt = DrawArgs.LastMousePosition;
                double lat;
                double lon;
                drawArgs.WorldCamera.PickingRayIntersection(pt.X, pt.Y, out lat, out lon);
                if (_RenderDX.SelectCell(lat, lon) != null)
                    selected = true;
            }
            return selected;
        }

        public override void Update(DrawArgs drawArgs)
        {
            if (!this.isInitialized)
                this.Initialize(drawArgs);
            this.isInitialized = true;
        }

        public override void Render(DrawArgs drawArgs)
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

                if (MeshType == Renderable.Grid.MeshType.Line)
                {
                    drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, _RenderDX.VertexList.Length, _RenderDX.VertexIndexList.Length / 2,
    _RenderDX.VertexIndexList, false, _RenderDX.VertexList);
                }
                else
                {
                    drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, _RenderDX.VertexList.Length, _RenderDX.VertexIndexList.Length / 3,
                        _RenderDX.VertexIndexList, false, _RenderDX.VertexList);
                }

                if (HighLightSelectedCell)
                {
                    if (_RenderDX.SelectedVertexes != null)
                    {
                        currentCull = drawArgs.device.RenderState.CullMode;
                        drawArgs.device.RenderState.CullMode = Cull.None;

                        drawArgs.device.VertexFormat = CustomVertex.PositionColored.Format;
                        drawArgs.device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, _RenderDX.SelectedVertexes.Length,
                             _RenderDX.SelectedVertexIndexes.Length / 3, _RenderDX.SelectedVertexIndexes, false, _RenderDX.SelectedVertexes);

                        drawArgs.device.RenderState.CullMode = currentCull;
                    }
                }
                device.RenderState.FillMode = fillmode;
                drawArgs.device.RenderState.CullMode = currentCull;
                drawArgs.device.Transform.World = drawArgs.WorldCamera.WorldMatrix;
            }
        }

    }
}
