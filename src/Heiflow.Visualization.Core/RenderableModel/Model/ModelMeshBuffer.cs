using System;

using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

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
        private int[] _indexSource;
        private int _meshVersion = -1;
        private int _colorVersion = -1;

        /// <summary>
        /// The buffers themselves, for a caller that draws this mesh with a shader of its own rather
        /// than through Draw below.
        /// </summary>
        public VertexBuffer VertexBuffer
        {
            get { return _vertexBuffer; }
        }

        public IndexBuffer IndexBuffer
        {
            get { return _indexBuffer; }
        }

        public int VertexCount
        {
            get { return _vertexCount; }
        }

        public int IndexCount
        {
            get { return _indexCount; }
        }

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
            // written into the same arrays, for example a new time step. The index array is watched as
            // well because a layer that is shown as a wire frame draws other indices over the very
            // same vertices, and that is a change no version announces.
            bool geometryChanged = !ReferenceEquals(_source, vertices)
                || !ReferenceEquals(_indexSource, indices)
                || _meshVersion != meshVersion;
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

                // The colours are rewritten on every frame of an animation, so the vertex buffer is a
                // dynamic one that lives in the Default pool. The Managed pool it used before keeps a
                // copy in system memory that the driver has to push to the card again after every
                // update, and that second copy is what the playback spent its time on. Neither buffer
                // is ever read back, so both are write only.
                _vertexBuffer = new VertexBuffer(device,
                    _vertexCount * CustomVertex.PositionNormalColored.StrideSize,
                    Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Default);
                _indexBuffer = new IndexBuffer(device, (int)(_indexCount) * 4, Usage.WriteOnly, Pool.Default, true);

                geometryChanged = true;
            }

            // A buffer in the Default pool is gone once the device has been reset, and the versions
            // above would still say nothing changed. The buffers are let go of instead, so the next
            // frame builds them again from the renderer's arrays.
            try
            {
                // Every vertex is written again, so what the buffer held is of no use and saying so
                // keeps the driver from waiting for the card to finish reading it. The indices only
                // move when the geometry does, so that buffer is not dynamic and is written plainly.
                _vertexBuffer.SetData(vertices, 0, LockFlags.Discard);
                if (geometryChanged)
                    _indexBuffer.SetData(indices, 0, LockFlags.None);
            }
            catch (SharpDXException)
            {
                Release();
                return false;
            }

            _source = vertices;
            _indexSource = indices;
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

            device.SetStreamSource(0, _vertexBuffer, 0, CustomVertex.PositionNormalColored.StrideSize);
            device.Indices = _indexBuffer;
            device.VertexFormat = CustomVertex.PositionNormalColored.Format;
            device.DrawIndexedPrimitive(primitiveType, 0, 0, _vertexCount, 0, _indexCount / verticesPerPrimitive);
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
