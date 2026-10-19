using System;
using HUST.WREIS.Dot3D;

using SharpDX;
using SharpDX.Direct3D9;
using Heiflow.Visualization.DirectX;

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
        private static Material? _material;
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
            scope._cullMode = device.GetRenderStateEnum<Cull>(RenderState.CullMode);
            scope._fillMode = device.GetRenderStateEnum<FillMode>(RenderState.FillMode);

            if (clearDepthBuffer)
                ClearDepthBuffer(device);

            device.SetRenderState(RenderState.ZEnable, true);
            device.VertexFormat = CustomVertex.PositionNormalColored.Format;
            device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Disable);

            device.SetTransform(TransformState.World, Matrix.Translation(
                (float)(-drawArgs.WorldCamera.ReferenceCenter.X),
                (float)(-drawArgs.WorldCamera.ReferenceCenter.Y),
                (float)(-drawArgs.WorldCamera.ReferenceCenter.Z)));

            ApplyLighting(device);

            device.SetRenderState(RenderState.CullMode, (int)(Cull.None));
            device.SetRenderState(RenderState.FillMode, fillMode);

            return scope;
        }

        /// <summary>
        /// Restores the states the layer found.
        /// </summary>
        public void Dispose()
        {
            if (_device == null)
                return;

            _device.SetRenderState(RenderState.FillMode, _fillMode);
            _device.SetRenderState(RenderState.CullMode, _cullMode);

            if (_drawArgs != null && _drawArgs.WorldCamera != null)
                _device.SetTransform(TransformState.World, _drawArgs.WorldCamera.WorldMatrix);
        }

        private static void ApplyLighting(Device device)
        {
            if (World.Settings.EnableSunShading)
            {
                device.SetRenderState(RenderState.Lighting, true);
                device.Material = SharedMaterial;
                device.SetRenderState(RenderState.Ambient, World.Settings.ShadingAmbientColor.ToArgb());
                device.SetRenderState(RenderState.NormalizeNormals, true);
                device.SetRenderState(RenderState.AlphaBlendEnable, true);

                LightHub.Get(device, 0).Enabled = true;
                LightHub.Get(device, 0).Type = LightType.Directional;
                LightHub.Get(device, 0).Diffuse = ColorUtil.ToColor4(System.Drawing.Color.White);
                LightHub.Get(device, 0).Direction = SunVector;
            }
            else
            {
                device.SetRenderState(RenderState.Lighting, false);
                device.SetRenderState(RenderState.Ambient, World.Settings.StandardAmbientColor.ToArgb());
            }
        }

        private static Material SharedMaterial
        {
            get
            {
                if (!_material.HasValue)
                {
                    var created = new Material();
                    created.Diffuse = ColorUtil.ToColor4(System.Drawing.Color.White);
                    created.Ambient = ColorUtil.ToColor4(System.Drawing.Color.White);
                    _material = created;
                }
                return _material.Value;
            }
        }

        /// <summary>
        /// Where the sun is, for a caller that does the lighting itself, see ModelColor.fx.
        /// </summary>
        public static Vector3 SunVector
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
