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
            /// <summary>
            /// Vertex and index buffer of the mesh of _RenderDX. The vertices are uploaded once and again
            /// only when the renderer reports new geometry or new colours.
            /// </summary>
            protected readonly ModelMeshBuffer _Mesh = new ModelMeshBuffer();

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
            _Mesh.Dispose();
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
            if (!this.isInitialized || _RenderDX == null)
                return;

            var vertices = _RenderDX.VertexList;
            var indices = _RenderDX.VertexIndexList;
            if (vertices == null || indices == null || vertices.Length == 0 || indices.Length == 0)
                return;

            Device device = DrawArgs.Device;

            using (ModelRenderScope.Begin(device, drawArgs, FillMode, !World.Settings.EnableHighPerfomance))
            {
                if (RenderObject.VerticalExaggeration != World.Settings.VerticalExaggeration)
                {
                    RenderObject.VerticalExaggeration = World.Settings.VerticalExaggeration;
                }

                var primitiveType = MeshType == Renderable.Grid.MeshType.Line
                    ? PrimitiveType.LineList
                    : PrimitiveType.TriangleList;

                if (_Mesh.Update(device, vertices, indices, _RenderDX.MeshVersion, _RenderDX.ColorVersion))
                {
                    _Mesh.Draw(device, primitiveType);
                }
                else
                {
                    // no buffers, the mesh is tiny or the device could not create them
                    int step = primitiveType == PrimitiveType.LineList ? 2 : 3;
                    device.DrawIndexedUserPrimitives(primitiveType, 0, vertices.Length, indices.Length / step,
                        indices, false, vertices);
                }

                if (HighLightSelectedCell && _RenderDX.SelectedVertexes != null && _RenderDX.SelectedVertexIndexes != null)
                {
                    device.RenderState.CullMode = Cull.None;
                    device.VertexFormat = CustomVertex.PositionColored.Format;
                    device.DrawIndexedUserPrimitives(PrimitiveType.TriangleList, 0, _RenderDX.SelectedVertexes.Length,
                         _RenderDX.SelectedVertexIndexes.Length / 3, _RenderDX.SelectedVertexIndexes, false, _RenderDX.SelectedVertexes);
                }
            }
        }

    }
}
