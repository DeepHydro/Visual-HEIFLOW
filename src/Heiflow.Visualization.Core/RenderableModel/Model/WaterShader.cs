using System;
using System.Reflection;
using Heiflow.Core;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Heiflow.Visualization.Renderable.Grid
{
    /// <summary>
    /// Everything the water shader is told about one body of water: where the eye and the sun are,
    /// what the waves are like, and the colours the water is drawn in.
    /// </summary>
    public sealed class WaterParameters
    {
        public Matrix WorldViewProjection;
        public Vector3 EyePosition;

        /// <summary>
        /// Towards the sun, in the same coordinates the vertices are in.
        /// </summary>
        public Vector3 SunDirection;

        /// <summary>
        /// Unit vectors along the east and the north of the water body. The texture coordinates of a
        /// vertex are metres along these two, and the shader needs them to know which way a wave
        /// leans.
        /// </summary>
        public Vector3 East;
        public Vector3 North;

        /// <summary>
        /// Seconds since the water started running, it is what makes it move.
        /// </summary>
        public float Time;

        /// <summary>
        /// Metres from a trough to a crest, metres between two crests, and how fast a crest runs.
        /// </summary>
        public float WaveHeight;
        public float WaveLength;
        public float WaveSpeed;

        public float Opacity;

        public float[] ShallowColor;
        public float[] DeepColor;
        public float[] SkyColor;
        public float[] SunColor;

        public WaterParameters()
        {
            WaveHeight = 0.6f;
            WaveLength = 40f;
            WaveSpeed = 1f;
            Opacity = 0.9f;
            ShallowColor = new[] { 0.38f, 0.66f, 0.76f, 1f };
            DeepColor = new[] { 0.05f, 0.21f, 0.38f, 1f };
            SkyColor = new[] { 0.58f, 0.74f, 0.88f, 1f };
            SunColor = new[] { 1f, 0.96f, 0.88f, 1f };
        }
    }

    /// <summary>
    /// Draws a sheet of triangles as water, see Shaders\Water.fx.
    /// </summary>
    /// <remarks>
    /// Everything that can go wrong here leaves IsAvailable false and the layer then draws the water
    /// the way the fixed pipeline can: a flat sheet of one colour. A card without shaders, or one
    /// that will not take this one, gets a still lake rather than a black screen.
    /// </remarks>
    public sealed class WaterShader : IDisposable
    {
        private const string TechniqueName = "Water";
        private const string ResourceName = "Heiflow.Visualization.Shaders.Water.fx";

        private Effect _effect;
        private bool _broken;

        /// <summary>
        /// True while the layer can be drawn through the shader.
        /// </summary>
        public bool IsAvailable
        {
            get { return _effect != null && !_broken; }
        }

        /// <summary>
        /// Loads the shader the first time it is asked for. A card that cannot take it, and one that
        /// has lost it, are remembered, and a device that is lost and comes back is given another
        /// try on the next frame.
        /// </summary>
        public void Ensure(Device device)
        {
            if (_effect != null || _broken || device == null)
                return;

            try
            {
                if (!SupportsShaders(device))
                {
                    _broken = true;
                    return;
                }

                _effect = Load(device);

                // A card that will not take the shader is remembered as one. Left unremembered, the
                // next frame asks for it again, and the one after that, and every one of those asks
                // is a shader compiled from scratch: the water came out flat and the whole picture
                // ran at a frame every third of a second, which is how it was found.
                if (_effect == null)
                    _broken = true;
            }
            catch (DirectXException)
            {
                _effect = null;
                _broken = true;
            }
        }

        /// <summary>
        /// Draws one body of water. Returns false when the layer has to fall back on the fixed
        /// pipeline.
        /// </summary>
        public bool Draw(Device device, VertexBuffer vertices, IndexBuffer indices,
            int vertexCount, int indexCount, WaterParameters p)
        {
            if (!IsAvailable || vertices == null || indices == null)
                return false;

            try
            {
                _effect.Technique = TechniqueName;
                _effect.SetValue("WorldViewProjection", p.WorldViewProjection);

                // A float3 is handed over as three floats. The overload that takes a vector is the one
                // for a texture, and the compiler picks it for a Vector3 before it complains.
                _effect.SetValue("EyePosition", new[] { p.EyePosition.X, p.EyePosition.Y, p.EyePosition.Z });
                _effect.SetValue("SunDirection", new[] { p.SunDirection.X, p.SunDirection.Y, p.SunDirection.Z });
                _effect.SetValue("LocalEast", new[] { p.East.X, p.East.Y, p.East.Z });
                _effect.SetValue("LocalNorth", new[] { p.North.X, p.North.Y, p.North.Z });

                _effect.SetValue("Time", p.Time);
                _effect.SetValue("WaveHeight", p.WaveHeight);
                _effect.SetValue("WaveLength", p.WaveLength);
                _effect.SetValue("WaveSpeed", p.WaveSpeed);
                _effect.SetValue("Opacity", p.Opacity);

                _effect.SetValue("ShallowColor", p.ShallowColor);
                _effect.SetValue("DeepColor", p.DeepColor);
                _effect.SetValue("SkyColor", p.SkyColor);
                _effect.SetValue("SunColor", p.SunColor);

                device.VertexFormat = CustomVertex.PositionNormalTextured.Format;
                device.SetStreamSource(0, vertices, 0, CustomVertex.PositionNormalTextured.StrideSize);
                device.Indices = indices;

                var passes = _effect.Begin(0);
                for (int pass = 0; pass < passes; pass++)
                {
                    _effect.BeginPass(pass);
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0,
                        indexCount / 3);
                    _effect.EndPass();
                }
                _effect.End();

                device.Indices = null;
                return true;
            }
            catch (DirectXException)
            {
                // The device is gone, the shader goes with it and is loaded again once it is back.
                _effect = null;
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
                {
                    Log.Write(Log.Levels.Error, "WATR", "The water shader is not in the assembly: " + ResourceName);
                    return null;
                }

                string errors;

                try
                {
                    var effect = Effect.FromStream(device, stream, null, null, ShaderFlags.None, null,
                        out errors);

                    // What the compiler says is written down. It used to be thrown away, and a
                    // shader the card would not take then failed without a word, and the water
                    // came up flat with nothing anywhere to say why.
                    if (effect == null)
                    {
                        Log.Write(Log.Levels.Error, "WATR", "The water shader would not compile: " + errors);
                        return null;
                    }

                    if (!string.IsNullOrEmpty(errors))
                        Log.Write(Log.Levels.Warning, "WATR", "The water shader compiled with this to say: " + errors);

                    return effect;
                }
                catch (DirectXException ex)
                {
                    Log.Write(Log.Levels.Error, "WATR", "The water shader would not compile: " + ex.Message);
                    return null;
                }
            }
        }

        public void Dispose()
        {
            if (_effect != null)
            {
                _effect.Dispose();
                _effect = null;
            }
        }
    }
}
