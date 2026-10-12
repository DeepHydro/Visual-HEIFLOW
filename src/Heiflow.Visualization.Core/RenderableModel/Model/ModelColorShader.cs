using System;
using System.IO;
using System.Reflection;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Heiflow.Visualization.Renderable.Grid
{
    /// <summary>
    /// Draws a model mesh with the colours taken from textures rather than from the vertices.
    /// </summary>
    /// <remarks>
    /// A layer that plays an animation used to write the colour of every vertex again on every frame
    /// and push the whole vertex array at the card, several megabytes for a grid of a few hundred
    /// thousand cells, only to change four bytes per vertex. Here the geometry is uploaded once, the
    /// class or the shade of a vertex travels in a texture of its own, and a frame costs the one
    /// float that says how far the animation has come between the step before and the step it is at.
    ///
    /// Everything that can fail here leaves IsAvailable false, and the layer then draws the way it
    /// always did. A card without shaders, one that will not take a one byte texture, or a shader
    /// that did not compile all end up on the fixed pipeline instead of on a black screen.
    /// </remarks>
    public sealed class ModelColorShader : IDisposable
    {
        private const string TechniqueName = "RenderModelColor";
        private const string TechniqueNameSmooth = "RenderModelColorSmooth";
        private const string ResourceName = "Heiflow.Visualization.Shaders.ModelColor.fx";

        private Effect _effect;
        private VertexDeclaration _declaration;
        private VertexBuffer _uvBuffer;
        private Texture _stepBefore;
        private Texture _stepNow;
        private Texture _ramp;

        private int[] _texel;
        private int _width;
        private int _height;

        /// <summary>
        /// One byte for every texel of a step, filled from the classes or shades of the step before it
        /// is handed to the card. Kept between uploads so a step costs no allocation.
        /// </summary>
        private byte[] _texels;

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
        /// Makes the buffers and textures of one grid. The layout says which texel of the step texture
        /// carries which vertex, see ModelRenderDX.VertexTexel.
        /// </summary>
        public bool Ensure(Device device, int[] texel, int width, int height)
        {
            if (_broken || device == null || texel == null || width <= 0 || height <= 0)
                return false;

            try
            {
                if (!_watchingDevice)
                {
                    // A reset throws the buffers and the effect away, and the versions of the mesh
                    // would still say nothing changed, so everything is let go of and built again.
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

                if (!ReferenceEquals(_texel, texel) || _width != width || _height != height)
                {
                    ReleaseTargets();
                    _texel = texel;
                    _width = width;
                    _height = height;
                    _texels = new byte[width * height];
                    _stepBefore = CreateStepTexture(device, width, height);
                    _stepNow = CreateStepTexture(device, width, height);
                    _uvBuffer = CreateUv(device, texel, width, height);
                    _declaration = CreateDeclaration(device);
                }

                if (_ramp == null)
                    _ramp = new Texture(device, 256, 1, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);

                return _stepBefore != null && _stepNow != null && _uvBuffer != null
                    && _declaration != null && _ramp != null;
            }
            catch (DirectXException)
            {
                _broken = true;
                return false;
            }
        }

        /// <summary>
        /// Fills one of the two step textures with the classes or shades of one step. The layout
        /// decides where in the texture a vertex ends up, which is not the order the vertices come in
        /// for a grid that is laid out in rows and columns.
        /// </summary>
        /// <param name="levels">Classes or shades, one byte for every vertex of every step.</param>
        /// <param name="step">Which step of them to take.</param>
        /// <param name="stride">How many vertices one step takes. It is the count of active vertices
        /// and not the length of the vertex array, which is twice as long while the walls are drawn,
        /// and taking that for the stride walked off the end of the levels and left the texture
        /// with the classes of the step that was uploaded before.</param>
        /// <param name="count">How many vertices of the step are spread over the texture.</param>
        public void UploadStep(Device device, byte[] levels, int step, int stride, int count, bool before)
        {
            var target = before ? _stepBefore : _stepNow;
            if (_broken || target == null || levels == null || _texels == null)
                return;

            if (stride <= 0 || count <= 0)
                return;

            var offset = step * stride;
            if (offset < 0 || offset + count > levels.Length)
                return;

            try
            {
                // The texels of vertices that are not active are left at the bottom of the ramp
                // rather than keeping the class of whatever step was uploaded before.
                Array.Clear(_texels, 0, _texels.Length);
                for (int i = 0; i < count; i++)
                {
                    var at = _texel[i];
                    if (at >= 0 && at < _texels.Length)
                        _texels[at] = levels[offset + i];
                }
                Write(target, _texels, _width, _height);
            }
            catch (DirectXException)
            {
                _broken = true;
            }
        }

        /// <summary>
        /// Fills the ramp texture with the colours of the classes or of the shades, in the order the
        /// byte of a vertex names them.
        /// </summary>
        public void UploadRamp(int[] palette)
        {
            if (_broken || _ramp == null || palette == null)
                return;

            var bytes = new byte[palette.Length * 4];
            Buffer.BlockCopy(palette, 0, bytes, 0, bytes.Length);

            try
            {
                int pitch;
                var stream = _ramp.LockRectangle(0, LockFlags.None, out pitch);
                try
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                finally
                {
                    _ramp.UnlockRectangle(0);
                }
            }
            catch (DirectXException)
            {
                _broken = true;
            }
        }

        /// <summary>
        /// Draws the mesh. Returns false when the layer has to fall back on the fixed pipeline.
        /// </summary>
        public bool Draw(Device device, VertexBuffer vertices, IndexBuffer indices, int vertexCount,
            int indexCount, PrimitiveType primitiveType, Matrix worldViewProjection, Vector3 sunDirection,
            float sunLight, float[] ambient, float opacity, float blend, bool smooth)
        {
            if (!IsAvailable || vertices == null || indices == null || _declaration == null || _uvBuffer == null)
                return false;

            try
            {
                _effect.Technique = smooth ? TechniqueNameSmooth : TechniqueName;
                _effect.SetValue("WorldViewProjection", worldViewProjection);
                // A float3 is handed over as three floats. The overload that takes a vector is the one
                // for a texture, and the compiler picks it for a Vector3 before it complains.
                _effect.SetValue("SunDirection", new[] { sunDirection.X, sunDirection.Y, sunDirection.Z });
                _effect.SetValue("SunLight", sunLight);
                _effect.SetValue("Ambient", ambient);
                _effect.SetValue("Opacity", opacity);
                _effect.SetValue("Blend", blend);
                _effect.SetValue("StepBefore", _stepBefore);
                _effect.SetValue("StepNow", _stepNow);
                _effect.SetValue("Ramp", _ramp);

                var verticesPerPrimitive = primitiveType == PrimitiveType.LineList ? 2 : 3;

                device.VertexDeclaration = _declaration;
                device.SetStreamSource(0, vertices, 0, CustomVertex.PositionNormalColored.StrideSize);
                device.SetStreamSource(1, _uvBuffer, 0, 8);
                device.Indices = indices;

                var passes = _effect.Begin(0);
                for (int pass = 0; pass < passes; pass++)
                {
                    _effect.BeginPass(pass);
                    device.DrawIndexedPrimitives(primitiveType, 0, 0, vertexCount, 0,
                        indexCount / verticesPerPrimitive);
                    _effect.EndPass();
                }
                _effect.End();

                device.SetStreamSource(1, null, 0, 0);
                device.VertexDeclaration = null;
                device.Indices = null;
                return true;
            }
            catch (DirectXException)
            {
                _broken = true;
                return false;
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

        private static Texture CreateStepTexture(Device device, int width, int height)
        {
            // One byte a texel is enough for a class and for a shade alike, and it is a quarter of
            // what an integer texture would take.
            return new Texture(device, width, height, 1, Usage.None, Format.L8, Pool.Managed);
        }

        private static VertexBuffer CreateUv(Device device, int[] texel, int width, int height)
        {
            var uv = new float[texel.Length * 2];
            for (int i = 0; i < texel.Length; i++)
            {
                var at = texel[i];
                // The middle of the texel, so that a sample lands on it and not on the seam between
                // it and the next one.
                uv[i * 2] = ((at % width) + 0.5f) / width;
                uv[i * 2 + 1] = ((at / width) + 0.5f) / height;
            }

            var buffer = new VertexBuffer(typeof(float), uv.Length, device, Usage.None,
                VertexFormats.None, Pool.Managed);
            var stream = buffer.Lock(0, 0, LockFlags.None);
            try
            {
                stream.Write(uv);
            }
            finally
            {
                buffer.Unlock();
            }
            return buffer;
        }

        /// <summary>
        /// The geometry keeps the format it has, position, normal and colour in one stream, and the
        /// coordinate into the step texture comes in a second one. The colour stays where it is so
        /// that the fixed pipeline can still draw the same vertices when the shader cannot.
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
                new VertexElement(1, 0, DeclarationType.Float2, DeclarationMethod.Default,
                    DeclarationUsage.TextureCoordinate, 0),
                VertexElement.VertexDeclarationEnd
            };
            return new VertexDeclaration(device, elements);
        }

        private static void Write(Texture texture, byte[] texels, int width, int height)
        {
            int pitch;
            var stream = texture.LockRectangle(0, LockFlags.None, out pitch);
            try
            {
                for (int y = 0; y < height; y++)
                {
                    stream.Write(texels, y * width, width);
                    // A row of a texture is not always as wide as the texels in it, the card is
                    // allowed to leave a gap at the end of it.
                    if (pitch > width)
                        stream.Seek(pitch - width, SeekOrigin.Current);
                }
            }
            finally
            {
                texture.UnlockRectangle(0);
            }
        }

        private void OnDeviceReset(object sender, EventArgs e)
        {
            ReleaseTargets();
            if (_effect != null)
            {
                _effect.Dispose();
                _effect = null;
            }
            _texel = null;
        }

        private void ReleaseTargets()
        {
            if (_stepBefore != null)
            {
                _stepBefore.Dispose();
                _stepBefore = null;
            }
            if (_stepNow != null)
            {
                _stepNow.Dispose();
                _stepNow = null;
            }
            if (_ramp != null)
            {
                _ramp.Dispose();
                _ramp = null;
            }
            if (_uvBuffer != null)
            {
                _uvBuffer.Dispose();
                _uvBuffer = null;
            }
            if (_declaration != null)
            {
                _declaration.Dispose();
                _declaration = null;
            }
            _texels = null;
        }

        public void Dispose()
        {
            ReleaseTargets();
            if (_effect != null)
            {
                _effect.Dispose();
                _effect = null;
            }
        }
    }
}
