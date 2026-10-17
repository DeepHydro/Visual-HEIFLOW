using Heiflow.Visualization.RenderableModel.Model.Renders;
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
        /// <summary>
        /// Vertex and index buffer of the arrows of _RenderDX.
        /// </summary>
        private readonly ModelMeshBuffer _Mesh = new ModelMeshBuffer();

        /// <summary>
        /// Draws the mesh as a flow that runs along the lines of it, see Shaders/Streamline.fx.
        /// </summary>
        private StreamlineShader _Streamline;

        /// <summary>
        /// Where the flow started, the clock the dashes are moved by. It belongs to the layer rather
        /// than to the frame, so that a line stays where it ran to when the frame before left it.
        /// </summary>
        private readonly DateTime _FlowStarted = DateTime.UtcNow;

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
            if (!this.isInitialized || _RenderDX == null)
                return;

            var vertices = _RenderDX.VertexList;
            var indices = _RenderDX.VertexIndexList;
            if (vertices == null || indices == null || vertices.Length == 0 || indices.Length == 0)
                return;

            Device device = DrawArgs.Device;

            // The mesh holds an arrow on a cell while the arrows are drawn, three lines each, and a
            // ribbon of two triangles for every piece of a line while the lines are. The fallback the
            // shader leaves behind has to ask for the one the mesh is holding.
            var lines = _RenderDX is IStreamlineRender && ((IStreamlineRender)_RenderDX).StreamlinesActive;
            var primitiveType = lines ? PrimitiveType.TriangleList : PrimitiveType.LineList;
            var perPrimitive = lines ? 3 : 2;

            using (ModelRenderScope.Begin(device, drawArgs, FillMode, !World.Settings.EnableHighPerfomance))
            {
                if (RenderObject.VerticalExaggeration != World.Settings.VerticalExaggeration)
                {
                    RenderObject.VerticalExaggeration = World.Settings.VerticalExaggeration;
                }

                if (_Mesh.Update(device, vertices, indices, _RenderDX.MeshVersion, _RenderDX.ColorVersion))
                {
                    if (!DrawStreamlines(device))
                        _Mesh.Draw(device, primitiveType);
                }
                else
                {
                    device.DrawIndexedUserPrimitives(primitiveType, 0, vertices.Length, indices.Length / perPrimitive,
                        indices, false, vertices);
                }
            }
        }

        /// <summary>
        /// Draws the mesh through Streamline.fx while it holds lines traced through the flow, and
        /// says false while it holds arrows or the shader cannot be had, which leaves the caller to
        /// draw it the way it always did.
        /// </summary>
        private bool DrawStreamlines(Device device)
        {
            var flow = _RenderDX as IStreamlineRender;
            if (flow == null || !flow.StreamlinesActive || flow.StreamlineFlow == null)
                return false;

            try
            {
                if (_Streamline == null)
                    _Streamline = new StreamlineShader();

                if (!_Streamline.Ensure(device, flow.StreamlineFlow, _RenderDX.MeshVersion))
                    return false;

                var worldViewProjection = Matrix.Multiply(device.Transform.World,
                    Matrix.Multiply(device.Transform.View, device.Transform.Projection));

                var look = flow.Look;

                // How far the head of a dash has run since the layer started to draw, folded into
                // the distance between two heads. Only what is left of that division is handed over,
                // a head that has run for hours would leave too few digits of a float for the
                // fraction to mean anything.
                var seconds = (DateTime.UtcNow - _FlowStarted).TotalSeconds;
                var head = (float)((seconds * flow.FlowSpeed) % look.Spacing);
                if (head < 0)
                    head += look.Spacing;

                return _Streamline.Draw(device, _Mesh, worldViewProjection, look, head,
                    _RenderDX.Opacity / 255f);
            }
            catch (DirectXException)
            {
                return false;
            }
        }

        public override void Dispose()
        {
            if (_Streamline != null)
            {
                _Streamline.Dispose();
                _Streamline = null;
            }
            _Mesh.Dispose();
        }

        public override bool PerformSelectionAction(HUST.WREIS.Dot3D.DrawArgs drawArgs)
        {
            return false;
        }
    }
}
