using System;
using System.IO;
using System.Reflection;
using Heiflow.Visualization.RenderableModel.Model.Renders;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Heiflow.Visualization.Renderable.Grid
{
    /// <summary>
    /// Draws the lines of a vector layer as a flow that runs along them.
    /// </summary>
    /// <remarks>
    /// The geometry of a streamline is built once, and what makes it move is one float a frame. The
    /// distance a vertex lies along its line, the speed of the flow there and the side of the line it
    /// lies on travel beside the vertex in a second stream, the way the coordinate into the step
    /// texture does in <see cref="ModelColorShader"/>, so the vertex itself keeps the layout every
    /// other layer draws it with and the fixed pipeline can still take it when this shader cannot.
    ///
    /// Everything that can fail leaves IsAvailable false and the layer then draws the mesh the way it
    /// always did: a card without shaders, a shader that did not compile, or a buffer the card would
    /// not take.
    /// </remarks>
    public sealed class StreamlineShader : IDisposable
    {
        private const string TechniqueName = "RenderStreamline";
        private const string ResourceName = "Heiflow.Visualization.Shaders.Streamline.fx";

        private Effect _effect;
        private VertexDeclaration _declaration;
        private VertexBuffer _flowBuffer;

        /// <summary>
        /// The array the buffer was last filled from. The lines are rebuilt for every step, and a new
        /// array is what says the buffer may have to be filled again.
        /// </summary>
        private float[] _source;

        /// <summary>
        /// Version of the geometry the flow buffer holds, see ModelRenderDX.MeshVersion.
        /// </summary>
        private int _version = -1;

        /// <summary>
        /// How many floats the flow buffer holds, which is not always how many were written into it,
        /// a line that leaves the grid is shorter than the room kept for it.
        /// </summary>
        private int _flowCapacity;

        private bool _broken;
        private bool _watchingDevice;

        /// <summary>
        /// True while the layer can be drawn through the shader.
        /// </summary>
        public bool IsAvailable
        {
            get { return !_broken && _effect != null; }
        }

        /// <summary>
        /// Loads the effect and fills the buffer that carries the distance, the speed and the side of
        /// every vertex.
        /// </summary>
        /// <param name="device">Device to draw on.</param>
        /// <param name="flow">Three floats for every vertex of the mesh, how far along its line the
        /// vertex lies in the units the scene is drawn in, how fast the flow is there between 0 and
        /// 1, and which side of the line it lies on.</param>
        /// <param name="version">Version of the geometry, see ModelRenderDX.MeshVersion. The lines
        /// are written into the same array again for every step, so the array alone does not say
        /// whether what it holds is new.</param>
        public bool Ensure(Device device, float[] flow, int version)
        {
            if (_broken || device == null || flow == null || flow.Length < 3)
                return false;

            try
            {
                if (!_watchingDevice)
                {
                    // A reset throws the buffer and the effect away, and the geometry would still
                    // say nothing changed, so everything is let go of and built again.
                    device.DeviceReset += OnDeviceReset;
                    _watchingDevice = true;
                }

                if (_effect == null)
                {
                    if (!SupportsShaders(device))
                    {
                        _broken = true;
                        return false;
                    }
                    _effect = Load(device);
                    if (_effect == null)
                    {
                        _broken = true;
                        return false;
                    }
                }

                if (_declaration == null)
                    _declaration = CreateDeclaration(device);

                if (!ReferenceEquals(_source, flow) || _version != version)
                {
                    // A line is rebuilt for every step of the animation, and the count of vertices
                    // only changes when the density, the length or the smoothness of a line does, so
                    // the buffer is let go of only when it has grown too small to hold the new array.
                    if (_flowBuffer == null || _flowCapacity < flow.Length)
                    {
                        if (_flowBuffer != null)
                            _flowBuffer.Dispose();
                        _flowBuffer = CreateFlow(device, flow.Length);
                        _flowCapacity = flow.Length;
                    }
                    Upload(_flowBuffer, flow);
                    _source = flow;
                    _version = version;
                }

                return _declaration != null && _flowBuffer != null;
            }
            catch (DirectXException)
            {
                _broken = true;
                return false;
            }
        }

        /// <summary>
        /// Draws the mesh. Returns false when the layer has to fall back on the fixed pipeline.
        /// </summary>
        /// <param name="device">Device to draw on.</param>
        /// <param name="mesh">Buffers of the lines.</param>
        /// <param name="worldViewProjection">World, view and projection of the frame.</param>
        /// <param name="look">How the dashes are drawn, see StreamlineLook.</param>
        /// <param name="head">Where the head of a dash has run to, below the distance between two
        /// heads.</param>
        /// <param name="opacity">Opacity of the layer, between 0 and 1.</param>
        public bool Draw(Device device, ModelMeshBuffer mesh, Matrix worldViewProjection,
            StreamlineLook look, float head, float opacity)
        {
            if (!IsAvailable || mesh == null || _declaration == null || _flowBuffer == null)
                return false;
            if (mesh.VertexBuffer == null || mesh.IndexBuffer == null || mesh.IndexCount < 3)
                return false;

            var viewport = device.Viewport;
            if (viewport.Width <= 0 || viewport.Height <= 0 || look.Spacing <= 0)
                return false;

            // Where the light of a dash has fallen to by the time its trail runs out, and how much
            // of it the slowest part of the field keeps. Both are looks rather than settings, so
            // they are fixed here and not asked of the layer.
            const float tailSharpness = 2.0f;
            const float floor = 0.16f;
            const float slowest = 0.45f;

            var viewportSize = new float[2] { viewport.Width, viewport.Height };
            var blend = device.RenderState.AlphaBlendEnable;
            var source = device.RenderState.SourceBlend;
            var destination = device.RenderState.DestinationBlend;

            try
            {
                _effect.Technique = TechniqueName;
                _effect.SetValue("WorldViewProjection", worldViewProjection);
                _effect.SetValue("Viewport", viewportSize);
                _effect.SetValue("Width", look.Width);
                _effect.SetValue("TangentOffset", look.TangentOffset);
                _effect.SetValue("Head", head);
                _effect.SetValue("Spacing", look.Spacing);
                _effect.SetValue("Trail", look.Trail);
                _effect.SetValue("Opacity", opacity);
                _effect.SetValue("TailSharpness", tailSharpness);
                _effect.SetValue("Floor", floor);
                _effect.SetValue("Slowest", slowest);

                // The dashes fade out along their trail, which the fixed pipeline only asks for while
                // the sun is switched on, so the blend is put on here and taken off again.
                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.SourceBlend = Blend.SourceAlpha;
                device.RenderState.DestinationBlend = Blend.InvSourceAlpha;

                device.VertexDeclaration = _declaration;
                device.SetStreamSource(0, mesh.VertexBuffer, 0, CustomVertex.PositionNormalColored.StrideSize);
                device.SetStreamSource(1, _flowBuffer, 0, 12);
                device.Indices = mesh.IndexBuffer;

                var passes = _effect.Begin(0);
                for (int pass = 0; pass < passes; pass++)
                {
                    _effect.BeginPass(pass);
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.VertexCount, 0,
                        mesh.IndexCount / 3);
                    _effect.EndPass();
                }
                _effect.End();

                return true;
            }
            catch (DirectXException)
            {
                _broken = true;
                return false;
            }
            finally
            {
                device.SetStreamSource(1, null, 0, 0);
                device.VertexDeclaration = null;
                device.Indices = null;
                device.RenderState.AlphaBlendEnable = blend;
                device.RenderState.SourceBlend = source;
                device.RenderState.DestinationBlend = destination;
            }
        }

        private static bool SupportsShaders(Device device)
        {
            var caps = device.DeviceCaps;
            return caps.VertexShaderVersion >= new Version(2, 0)
                && caps.PixelShaderVersion >= new Version(2, 0);
        }

        private static Effect Load(Device device)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(ResourceName))
            {
                if (stream == null)
                    return null;

                string errors;
                return Effect.FromStream(device, stream, null, null, ShaderFlags.None, null, out errors);
            }
        }

        private static VertexBuffer CreateFlow(Device device, int count)
        {
            return new VertexBuffer(typeof(float), count, device, Usage.None, VertexFormats.None, Pool.Managed);
        }

        private static void Upload(VertexBuffer buffer, float[] flow)
        {
            var stream = buffer.Lock(0, 0, LockFlags.None);
            try
            {
                stream.Write(flow);
            }
            finally
            {
                buffer.Unlock();
            }
        }

        /// <summary>
        /// The geometry keeps the format it has, position, normal and colour in one stream, and the
        /// distance, the speed and the side come in a second one. The colour stays where it is so
        /// that the fixed pipeline can still draw the same vertices when the shader cannot, and the
        /// normal carries the way the line runs, which is what the width is put on across.
        /// </summary>
        private static VertexDeclaration CreateDeclaration(Device device)
        {
            var elements = new[]
            {
                new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default,
                    DeclarationUsage.Position, 0),
                new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default,
                    DeclarationUsage.Normal, 0),
                new VertexElement(0, 24, DeclarationType.Color, DeclarationMethod.Default,
                    DeclarationUsage.Color, 0),
                new VertexElement(1, 0, DeclarationType.Float3, DeclarationMethod.Default,
                    DeclarationUsage.TextureCoordinate, 0),
                VertexElement.VertexDeclarationEnd
            };
            return new VertexDeclaration(device, elements);
        }

        private void OnDeviceReset(object sender, EventArgs e)
        {
            if (_flowBuffer != null)
            {
                _flowBuffer.Dispose();
                _flowBuffer = null;
            }
            if (_declaration != null)
            {
                _declaration.Dispose();
                _declaration = null;
            }
            if (_effect != null)
            {
                _effect.Dispose();
                _effect = null;
            }
            _source = null;
            _flowCapacity = 0;
            _version = -1;
        }

        public void Dispose()
        {
            OnDeviceReset(this, EventArgs.Empty);
        }
    }
}
