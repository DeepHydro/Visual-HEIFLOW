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
            var triangles = _RenderDX.VertexIndexList;
            if (vertices == null || triangles == null || vertices.Length == 0 || triangles.Length == 0)
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

                // A line list takes its indices two by two, so the three corners of a triangle cannot
                // be handed to it: the last corner of one would be joined to the first of the next and
                // the line between them would run across the grid. A render that knows the edges of its
                // mesh keeps them for exactly this, see ModelRenderDX.LineIndexList.
                var indices = primitiveType == PrimitiveType.LineList
                    ? (_RenderDX.LineIndexList ?? triangles)
                    : triangles;

                if (indices.Length == 0)
                    return;

                var uploaded = _Mesh.Update(device, vertices, indices, _RenderDX.MeshVersion,
                    _RenderDX.ColorVersion);

                // The colours can be read out of textures instead of out of the vertices, and then a
                // frame costs the one float that says how far the animation has come between two steps
                // rather than an array as long as the grid. Whatever cannot be done that way is drawn
                // from the vertices as it always was.
                var shaded = uploaded && _RenderDX.DrawShaded(device, _Mesh, primitiveType,
                    Matrix.Multiply(device.Transform.World,
                        Matrix.Multiply(device.Transform.View, device.Transform.Projection)),
                    Vector3.Normalize(ModelRenderScope.SunVector),
                    World.Settings.EnableSunShading ? 1f : 0f,
                    // The fixed pipeline puts the ambient on a vertex only while it lights it, so a
                    // layer drawn without the sun is given white, or everything it draws would come
                    // out darker than it did before the shader.
                    AmbientOf(World.Settings.EnableSunShading
                        ? World.Settings.ShadingAmbientColor
                        : System.Drawing.Color.White));

                if (!shaded)
                {
                    if (uploaded)
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

        /// <summary>
        /// The ambient colour of the scene as the three numbers the shader multiplies the colour of a
        /// vertex by. It is a colour and not one number because the fixed pipeline put a colour on a
        /// vertex, and an ambient that is not grey would come out tinted if it were averaged.
        /// </summary>
        private static float[] AmbientOf(System.Drawing.Color color)
        {
            return new[] { color.R / 255f, color.G / 255f, color.B / 255f };
        }
    }
}
