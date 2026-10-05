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

            using (ModelRenderScope.Begin(device, drawArgs, FillMode, !World.Settings.EnableHighPerfomance))
            {
                if (RenderObject.VerticalExaggeration != World.Settings.VerticalExaggeration)
                {
                    RenderObject.VerticalExaggeration = World.Settings.VerticalExaggeration;
                }

                if (_Mesh.Update(device, vertices, indices, _RenderDX.MeshVersion, _RenderDX.ColorVersion))
                {
                    _Mesh.Draw(device, PrimitiveType.LineList);
                }
                else
                {
                    device.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, vertices.Length, indices.Length / 2,
                        indices, false, vertices);
                }
            }
        }

        public override void Dispose()
        {
            _Mesh.Dispose();
        }

        public override bool PerformSelectionAction(HUST.WREIS.Dot3D.DrawArgs drawArgs)
        {
            return false;
        }
    }
}
