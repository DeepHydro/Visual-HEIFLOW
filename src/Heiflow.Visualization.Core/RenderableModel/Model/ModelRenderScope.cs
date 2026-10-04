using System;
using HUST.WREIS.Dot3D;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Heiflow.Visualization.Renderable.Grid
{
    /// <summary>
    /// Device state of one model layer during a frame.
    /// </summary>
    /// <remarks>
    /// The four model layers carried the same block of device setup, and every one of them allocated a
    /// Material and computed the position of the sun again on every frame. The setup now lives here, the
    /// material is shared and the direction of the sun is refreshed once a minute, which is far below the
    /// speed the sun moves with.
    /// </remarks>
    public struct ModelRenderScope : IDisposable
    {
        private static Material _material;
        private static Vector3 _sunVector;
        private static DateTime _sunTime;
        private static bool _sunVectorKnown;

        /// <summary>
        /// Minutes the cached direction of the sun is kept.
        /// </summary>
        private const double SunVectorMinutes = 1.0;

        private Device _device;
        private DrawArgs _drawArgs;
        private Cull _cullMode;
        private FillMode _fillMode;

        /// <summary>
        /// Sets the states every model layer needs and returns a scope that restores them.
        /// </summary>
        /// <param name="device">Device to draw on.</param>
        /// <param name="drawArgs">Frame arguments, they carry the camera and the frame time.</param>
        /// <param name="fillMode">Fill mode of the layer.</param>
        /// <param name="clearDepthBuffer">Clear the depth buffer before drawing.</param>
        public static ModelRenderScope Begin(Device device, DrawArgs drawArgs, FillMode fillMode, bool clearDepthBuffer)
        {
            var scope = new ModelRenderScope();
            scope._device = device;
            scope._drawArgs = drawArgs;
            scope._cullMode = device.RenderState.CullMode;
            scope._fillMode = device.RenderState.FillMode;

            if (clearDepthBuffer)
                ClearDepthBuffer(device);

            device.RenderState.ZBufferEnable = true;
            device.VertexFormat = CustomVertex.PositionNormalColored.Format;
            device.TextureState[0].ColorOperation = TextureOperation.Disable;

            device.Transform.World = Matrix.Translation(
                (float)(-drawArgs.WorldCamera.ReferenceCenter.X),
                (float)(-drawArgs.WorldCamera.ReferenceCenter.Y),
                (float)(-drawArgs.WorldCamera.ReferenceCenter.Z));

            ApplyLighting(device);

            device.RenderState.CullMode = Cull.None;
            device.RenderState.FillMode = fillMode;

            return scope;
        }

        /// <summary>
        /// Restores the states the layer found.
        /// </summary>
        public void Dispose()
        {
            if (_device == null)
                return;

            _device.RenderState.FillMode = _fillMode;
            _device.RenderState.CullMode = _cullMode;

            if (_drawArgs != null && _drawArgs.WorldCamera != null)
                _device.Transform.World = _drawArgs.WorldCamera.WorldMatrix;
        }

        private static void ApplyLighting(Device device)
        {
            if (World.Settings.EnableSunShading)
            {
                device.RenderState.Lighting = true;
                device.Material = SharedMaterial;
                device.RenderState.AmbientColor = World.Settings.ShadingAmbientColor.ToArgb();
                device.RenderState.NormalizeNormals = true;
                device.RenderState.AlphaBlendEnable = true;

                device.Lights[0].Enabled = true;
                device.Lights[0].Type = LightType.Directional;
                device.Lights[0].Diffuse = System.Drawing.Color.White;
                device.Lights[0].Direction = SunVector;
            }
            else
            {
                device.RenderState.Lighting = false;
                device.RenderState.Ambient = World.Settings.StandardAmbientColor;
            }
        }

        private static Material SharedMaterial
        {
            get
            {
                if (_material == null)
                {
                    _material = new Material();
                    _material.Diffuse = System.Drawing.Color.White;
                    _material.Ambient = System.Drawing.Color.White;
                }
                return _material;
            }
        }

        private static Vector3 SunVector
        {
            get
            {
                DateTime now = TimeKeeper.CurrentTimeUtc;
                if (!_sunVectorKnown || (now - _sunTime).TotalMinutes >= SunVectorMinutes)
                {
                    Point3d position = SunCalculator.GetGeocentricPosition(now);
                    _sunVector = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
                    _sunTime = now;
                    _sunVectorKnown = true;
                }
                return _sunVector;
            }
        }

        /// <summary>
        /// Every layer clears the depth buffer before it draws, as it did before. Merging the clears of
        /// the layers of one frame into a single one would save two full screen operations per frame,
        /// but the model layers sit on the same surface and the ones drawn later would then be hidden
        /// behind the ones drawn before, so the behaviour is kept.
        /// </summary>
        private static void ClearDepthBuffer(Device device)
        {
            device.Clear(ClearFlags.ZBuffer, 0, 1.0f, 0);
        }
    }
}
