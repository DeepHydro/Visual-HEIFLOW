using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Heiflow.Visualization.Renderable.Grid
{
    /// <summary>
    /// Vertex and index buffer of one model mesh.
    /// </summary>
    /// <remarks>
    /// The model layers used to call DrawIndexedUserPrimitives, which copies the whole vertex array from
    /// managed memory to the driver on every single frame even when nothing changed. A grid of a few
    /// hundred thousand cells therefore moved several megabytes per frame for nothing. The buffers below
    /// are filled once and uploaded again only when the renderer reports new geometry or new colours.
    /// </remarks>
    public sealed class ModelMeshBuffer : IDisposable
    {
        private Device _device;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        private int _vertexCount;
        private int _indexCount;

        private CustomVertex.PositionNormalColored[] _source;
        private int _meshVersion = -1;
        private int _colorVersion = -1;

        /// <summary>
        /// Uploads the mesh when it changed and returns true when it can be drawn from the buffers.
        /// </summary>
        /// <param name="device">Device the buffers belong to.</param>
        /// <param name="vertices">Vertices of the renderer.</param>
        /// <param name="indices">Indices of the renderer.</param>
        /// <param name="meshVersion">Version of the geometry, see ModelRenderDX.MeshVersion.</param>
        /// <param name="colorVersion">Version of the colours, see ModelRenderDX.ColorVersion.</param>
        public bool Update(Device device, CustomVertex.PositionNormalColored[] vertices, int[] indices,
            int meshVersion, int colorVersion)
        {
            if (device == null || vertices == null || indices == null || vertices.Length == 0 || indices.Length == 0)
            {
                Release();
                return false;
            }

            // The arrays are replaced whenever the mesh is rebuilt, the versions catch changes that are
            // written into the same arrays, for example a new time step.
            bool geometryChanged = !ReferenceEquals(_source, vertices) || _meshVersion != meshVersion;
            bool colorChanged = _colorVersion != colorVersion;

            if (!geometryChanged && !colorChanged)
                return _vertexBuffer != null && _indexBuffer != null;

            if (_vertexBuffer == null || _indexBuffer == null || !ReferenceEquals(_device, device) ||
                _vertexCount != vertices.Length || _indexCount != indices.Length)
            {
                Release();

                _device = device;
                _vertexCount = vertices.Length;
                _indexCount = indices.Length;

                // Pool.Managed keeps a copy in system memory, so the buffers do not have to be rebuilt
                // when the device is reset.
                _vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), _vertexCount, device,
                    Usage.None, CustomVertex.PositionNormalColored.Format, Pool.Managed);
                _indexBuffer = new IndexBuffer(typeof(int), _indexCount, device, Usage.None, Pool.Managed);

                geometryChanged = true;
            }

            _vertexBuffer.SetData(vertices, 0, LockFlags.None);
            if (geometryChanged)
                _indexBuffer.SetData(indices, 0, LockFlags.None);

            _source = vertices;
            _meshVersion = meshVersion;
            _colorVersion = colorVersion;

            return true;
        }

        /// <summary>
        /// Draws the mesh from the buffers. Does nothing before the first successful Update.
        /// </summary>
        public void Draw(Device device, PrimitiveType primitiveType)
        {
            if (device == null || _vertexBuffer == null || _indexBuffer == null)
                return;

            int verticesPerPrimitive = primitiveType == PrimitiveType.LineList ? 2 : 3;

            device.SetStreamSource(0, _vertexBuffer, 0);
            device.Indices = _indexBuffer;
            device.VertexFormat = CustomVertex.PositionNormalColored.Format;
            device.DrawIndexedPrimitives(primitiveType, 0, 0, _vertexCount, 0, _indexCount / verticesPerPrimitive);
            device.Indices = null;
        }

        private void Release()
        {
            if (_vertexBuffer != null)
            {
                _vertexBuffer.Dispose();
                _vertexBuffer = null;
            }
            if (_indexBuffer != null)
            {
                _indexBuffer.Dispose();
                _indexBuffer = null;
            }
            _device = null;
            _source = null;
            _meshVersion = -1;
            _colorVersion = -1;
        }

        public void Dispose()
        {
            Release();
        }
    }
}
