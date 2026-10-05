///
///    This file is part of ILNumerics Community Edition.
///
///    ILNumerics Community Edition - high performance computing for applications.
///    Copyright (C) 2006 - 2013 Haymo Kutschbach, http://ilnumerics.net
///
///    ILNumerics Community Edition is free software: you can redistribute it and/or modify
///    it under the terms of the GNU General Public License version 3 as published by
///    the Free Software Foundation.
///
///    ILNumerics Community Edition is distributed in the hope that it will be useful,
///    but WITHOUT ANY WARRANTY; without even the implied warranty of
///    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
///    GNU General Public License for more details.
///
///    You should have received a copy of the GNU General Public License
///    along with ILNumerics Community Edition. See the file License.txt in the root
///    of your distribution package. If not, see <http://www.gnu.org/licenses/>.
///
///    In addition this software uses the following components and/or licenses: 
///
///    =================================================================================
///    The Open Toolkit Library License
///    
///    Copyright (c) 2006 - 2009 the Open Toolkit library.
///    
///    Permission is hereby granted, free of charge, to any person obtaining a copy
///    of this software and associated documentation files (the "Software"), to deal
///    in the Software without restriction, including without limitation the rights to 
///    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
///    the Software, and to permit persons to whom the Software is furnished to do
///    so, subject to the following conditions:
///
///    The above copyright notice and this permission notice shall be included in all
///    copies or substantial portions of the Software.
///
///    =================================================================================
///    Intel® Math Kernel Library 11.1 for Windows
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Intel® Math Kernel Library 10.3 for Linux
///        
///        http://www.intel.com/software/products/mkl
///
///    =================================================================================
///    Products / Software which is implicitly used by ILNumerics due to the inclusion 
///    of 3rd party components: 
///  
///        BLAS/ LAPACK; see: http://netlib.org
///        FFT Functions; see: http://www.spiral.net, http://fftw.org
///        OpenGL; see: http://opengl.org
///
///    =================================================================================


using System;
using System.Collections; 
using System.Collections.Generic;
using System.Collections.Concurrent; 
using System.Linq;
using System.Text;
using System.Drawing;
using ILNumerics;
using OpenTK; 
using OpenTK.Graphics; 
using OpenTK.Graphics.OpenGL;

namespace ILNumerics.Drawing  {

    struct A1 {
        internal Vector4 AmbientColor;
        internal float Attenuation;
        internal int NumberOfLights;
    }
    struct LightParameterExt {
        internal Vector4 AmbientColor;
        internal float Attenuation;
        internal int NumberOfLights;
    }
    struct ClippingPlanes0_5 {
        internal Vector4 Plane0;
        internal Vector4 Plane1;
        internal Vector4 Plane2;
        internal Vector4 Plane3;
        internal Vector4 Plane4;
        internal Vector4 Plane5;
        internal float State; 
    }
    internal class ILOGLDriver : ILDriver {

        #region attributes
        object m_bufferSync = new object();
        Dictionary<int, ILOGLLabel> m_labelMapping = new Dictionary<int, ILOGLLabel>(); 
        Dictionary<int, ILOGLBuffer> m_bufferMapping = new Dictionary<int, ILOGLBuffer>();
        Dictionary<int, ILOGLShape> m_shapeMapping = new Dictionary<int, ILOGLShape>();
        ILOGLControl m_control; 
        CullFaces m_cull = CullFaces.None; 
        ILOGLUniformBlockBuffer<Matrix4> m_camera2ClipTransform;
        ILOGLUniformBlockBuffer<LightParameterExt> m_ligthParameterExt;
        ILOGLUniformBlockBuffer<ClippingPlanes0_5> m_clippingPlanes; 
        ILOGLUniformBlockBuffer<Light> m_lights;
        ILOGLUniformBlockBuffer<Vector4> m_logState;
        //ILOGLUniformBlockBuffer<Vector4> m_clipPlanes; 
        Rectangle m_currentViewport;
        private bool m_depthTestState = false; 
        private Size m_size;
        private float? m_actualVersion;
        #endregion

        #region properties 
        public override bool IsDisposed {
            get { return m_control.IsDisposed; }
        }

        /// <summary>
        /// Get the actual version of the current OpenGL graphics context
        /// </summary>
        /// <remarks><para>The actual version may differs from the requested version if the graphics card / driver does not support the version requested. 
        /// This provides an efficient way of checking the actual version and exit rendering in a failsafe, deterministic way. 
        /// </para>
        /// <para>Querying this property successfully requires an OpenGL context to be created and set current. Therefore, if the property is queried too early, 
        /// this requirement may not be met yet. A version number of 0.0 is returned in this case.</para>
        /// </remarks>
        public float ActualVersion {
            get {
                if (!m_actualVersion.HasValue && m_control.Context != null && !m_control.Context.IsDisposed) {
                    string vers = OpenTK.Graphics.OpenGL.GL.GetString(OpenTK.Graphics.OpenGL.StringName.Version);
                    System.Diagnostics.Trace.WriteLine("OpenGL Version string follows...:");
                    System.Diagnostics.Trace.WriteLine(vers); 
                    var split = vers.Split('.',' ');
                    float major, minor; 
                    if (split.Length > 1 && 
                        float.TryParse(split[0],System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out major) &&
                        float.TryParse(split[1],System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out minor)) {
                        m_actualVersion = major + minor / 10f;
                        System.Diagnostics.Trace.WriteLine("Extracted version number: " + m_actualVersion.ToString());
                    } else {
                        System.Diagnostics.Trace.WriteLine("Error parsing version string. Retrying on next frame ...");
                    }
                }
                return m_actualVersion.GetValueOrDefault();
            }
        }
        protected bool DepthTestEnabled {
            get { 
                return m_depthTestState; 
            }
            set {
                if (value != m_depthTestState) {
                    if (value) {
                        GL.Enable(EnableCap.DepthTest);
                    } else {
                        GL.Disable(EnableCap.DepthTest);
                    }
                    m_depthTestState = value;
                }
            }
        }
        public override System.Drawing.Size Size {
            get { return m_control.ClientSize; }
            set { 
                m_size = value;
            }
        }
        public override RendererTypes Driver {
            get { return RendererTypes.OpenGL; }
        }
        protected ILClipParams Clipping {
            get {
                if (m_clippingPlanes.First.State == 0) 
                    return null; 
                return new ILClipParams() {
                    Plane0 = m_clippingPlanes.First.Plane0, 
                    Plane1 = m_clippingPlanes.First.Plane1, 
                    Plane2 = m_clippingPlanes.First.Plane2, 
                    Plane3 = m_clippingPlanes.First.Plane3, 
                    Plane4 = m_clippingPlanes.First.Plane4, 
                    Plane5 = m_clippingPlanes.First.Plane5, 
                }; 
            }
            set {
                if (value != null) {
                    var val = m_clippingPlanes.First; 
                    // if currently deactivated or different
                    if (val.State == 0 || 
                        val.Plane0 != value.Plane0 ||
                        val.Plane1 != value.Plane1 ||
                        val.Plane2 != value.Plane2 ||
                        val.Plane3 != value.Plane3 ||
                        val.Plane4 != value.Plane4 ||
                        val.Plane5 != value.Plane5) {
                        val = new ClippingPlanes0_5() {
                            State = 1,
                            Plane0 = value.Plane0,
                            Plane1 = value.Plane1,
                            Plane2 = value.Plane2,
                            Plane3 = value.Plane3,
                            Plane4 = value.Plane4,
                            Plane5 = value.Plane5,
                        }; 
                        m_clippingPlanes.First = val;
                        if (Settings.OpenGL31_FIX_GL_CLIPVERTEX) {
                            GL.ClipPlane(ClipPlaneName.ClipPlane0, new double[] { value.Plane0.X, value.Plane0.Y, value.Plane0.Z, value.Plane0.W });
                            GL.ClipPlane(ClipPlaneName.ClipPlane1, new double[] { value.Plane1.X, value.Plane1.Y, value.Plane1.Z, value.Plane1.W });
                            GL.ClipPlane(ClipPlaneName.ClipPlane2, new double[] { value.Plane2.X, value.Plane2.Y, value.Plane2.Z, value.Plane2.W });
                            GL.ClipPlane(ClipPlaneName.ClipPlane3, new double[] { value.Plane3.X, value.Plane3.Y, value.Plane3.Z, value.Plane3.W });
                            GL.ClipPlane(ClipPlaneName.ClipPlane4, new double[] { value.Plane4.X, value.Plane4.Y, value.Plane4.Z, value.Plane4.W });
                            GL.ClipPlane(ClipPlaneName.ClipPlane5, new double[] { value.Plane5.X, value.Plane5.Y, value.Plane5.Z, value.Plane5.W }); 
                        }
                        GL.Enable(EnableCap.ClipPlane0);
                        GL.Enable(EnableCap.ClipPlane1);
                        GL.Enable(EnableCap.ClipPlane2);
                        GL.Enable(EnableCap.ClipPlane3);
                        GL.Enable(EnableCap.ClipPlane4);
                        GL.Enable(EnableCap.ClipPlane5);
                    }
                } else {

                    var val = m_clippingPlanes.First;
                    if (val.State != 0) {
                        val.State = 0;
                        m_clippingPlanes.First = val;
                        GL.Disable(EnableCap.ClipPlane0);
                        GL.Disable(EnableCap.ClipPlane1);
                        GL.Disable(EnableCap.ClipPlane2);
                        GL.Disable(EnableCap.ClipPlane3);
                        GL.Disable(EnableCap.ClipPlane4);
                        GL.Disable(EnableCap.ClipPlane5);
                    }
                }
            }
        }
        #endregion

        #region constructor
        public ILOGLDriver(ILOGLControl control)
            : this(control, new ILScene()) { }
        public ILOGLDriver(ILOGLControl control, ILScene scene)
            : base(scene) {
            if (control == null) return; 
            m_control = control;
            m_control.HandleCreated += new EventHandler(m_control_HandleCreated);
            m_camera2ClipTransform = new ILOGLUniformBlockBuffer<Matrix4>(ILOGLUniformBlockIndices.Camera2ClipTransform);

            m_lights = new ILOGLUniformBlockBuffer<Light>(ILOGLUniformBlockIndices.LightsArray, MAX_NUMBER_LIGHTS);
            m_ligthParameterExt = new ILOGLUniformBlockBuffer<LightParameterExt>(ILOGLUniformBlockIndices.LightsParameterExt);
            m_clippingPlanes = new ILOGLUniformBlockBuffer<ClippingPlanes0_5>(ILOGLUniformBlockIndices.ClipParams); // state + 6 planes 
            //m_clippingPlanes.Data = new List<Vector4>(); 
            m_logState = new ILOGLUniformBlockBuffer<Vector4>(ILOGLUniformBlockIndices.LogState);
        }

        #endregion

        #region private helper
        void Resize() {
            if (m_control.Context == null || !m_control.IsHandleCreated)
                return; 
            if (!m_control.Context.IsCurrent) 
                m_control.MakeCurrent();
            GL.Viewport((int)(Rectangle.Left * m_control.ClientSize.Width), (int)(Rectangle.Top * m_control.ClientSize.Height), (int)(Rectangle.Width * m_control.ClientSize.Width), (int)(Rectangle.Height * m_control.ClientSize.Height)); 
        }
        void m_control_HandleCreated(object sender, EventArgs e) {
            try {
                if (!m_control.Context.IsCurrent)
                    m_control.MakeCurrent();
                if (m_control == null || m_control.Context == null || m_control.Context.IsDisposed || !m_control.IsHandleCreated) {
                    return;
                }
                m_cull = CullFaces.None;
                ConfigureCull();
                //GL.Enable(EnableCap.DepthTest);
                //m_control_Resize(this, EventArgs.Empty); 
            } catch (Exception exc) {
                System.Diagnostics.Trace.WriteLine(exc.ToString());
#if !DEBUG
                throw;
#endif
            }
        }
        private void drawOGLLabel(ILRenderParameter parameters, ILLabel label) {
            ILOGLLabel oglLabel;
            if (!m_labelMapping.TryGetValue(label.ID, out oglLabel)) {
                oglLabel = new ILOGLLabel(label, m_control.Context as GraphicsContext);
                label.Disposing += new EventHandler(label_Disposing);
                m_labelMapping.Add(label.ID, oglLabel);
            }
            if (!m_camera2ClipTransform.First.Equals(parameters.ProjectionTransform)) {
                m_camera2ClipTransform.First = parameters.ProjectionTransform;
            }
            RectangleF viewport = parameters.ViewTransform.ToViewRectangle();
            GLViewport = System.Drawing.Rectangle.Round(viewport);
            oglLabel.Draw(parameters);
        }

        unsafe private void drawOGLShape(ILRenderParameter parameters, ILShape shape) {
            Matrix4 mat = parameters.CurrentModel2CameraTransform;
            if (!m_shapeMapping.ContainsKey(shape.ID)) {
                m_shapeMapping.Add(shape.ID, ILOGLShape.Create(shape, m_bufferMapping));
                shape.Disposing += new EventHandler(node_Disposing);
            }
            // fetch shape from mapping and render
            ILOGLShape oglShape = m_shapeMapping[shape.ID];
            // do we need to update the uniform blocks? 
            if (!m_camera2ClipTransform.First.Equals(parameters.ProjectionTransform)) {
                m_camera2ClipTransform.First = parameters.ProjectionTransform;
            }
            if (m_logState.First != parameters.LogState.Peek()) {
                m_logState.First = parameters.LogState.Peek();
            }
            ILClipParams clip = parameters.PeekClipping();
            Clipping = clip; 
            RectangleF viewport = parameters.ViewTransform.ToViewRectangle();
            GLViewport = System.Drawing.Rectangle.Round(viewport);
            // depth test /mask
            //DepthTestEnabled = parameters.DepthTestEnabled; 
            oglShape.Draw(parameters, mat);
        }
        private bool CheckValidShapes(Dictionary<ILBufferSet, ILOGLShape> map) {
            foreach (var o in map.Values) {
                if (o.Shape.Buffers == null) return false;
            }
            return true;
        }
        void label_Disposing(object sender, EventArgs e) {
            int key = ((ILDrawable)sender).ID;
            m_labelMapping.Remove(key);
        }
        void node_Disposing(object sender, EventArgs e) {
            ILShape shape = sender as ILShape;
            if (shape != null)
                m_shapeMapping.Remove(shape.ID);
        }
        internal void ConfigureCull() {
            // misc configurations
            switch (m_cull) {
                case CullFaces.CW:
                    GL.Enable(EnableCap.CullFace);
                    GL.CullFace(CullFaceMode.Back);
                    GL.FrontFace(FrontFaceDirection.Cw);
                    break;
                case CullFaces.CCW:
                    GL.Enable(EnableCap.CullFace);
                    GL.CullFace(CullFaceMode.Back);
                    GL.FrontFace(FrontFaceDirection.Ccw);
                    break;
                default: // none
                    GL.Disable(EnableCap.CullFace);
                    break;
            }
        }
        #endregion

        #region public interface
        /// <summary>
        /// get/set the _OpenGL_ viewport, expects rectangles origin at UPPER left corner 
        /// </summary>
        public Rectangle GLViewport {
            get { return m_currentViewport; }
            set {
                if (value != m_currentViewport) {
                    m_currentViewport = value;
                    GL.Viewport(value.Left, m_control.ClientSize.Height - value.Height - value.Top, value.Width, value.Height);
                }
            }
        }
        /// <summary>
        /// get the _OpenGL_ viewport according to ClientSize, Rectangle and innerRect
        /// </summary>
        /// <param name="innerRect"></param>
        /// <returns></returns>
        protected Rectangle GetGLViewport(RectangleF innerRect) {
            Rectangle renderArea = new System.Drawing.Rectangle((int)(Rectangle.Left * m_control.ClientSize.Width * innerRect.Left),
                                            m_control.ClientSize.Height - (int)(Rectangle.Top * m_control.ClientSize.Height * innerRect.Top),
                                            (int)(Rectangle.Width * m_control.ClientSize.Width * innerRect.Width),
                                            (int)(Rectangle.Height * m_control.ClientSize.Height * innerRect.Height));
            return renderArea; 
        }
        protected override void BeginDrawTransparent(ILRenderParameter renderParams) {
            if (!renderParams.BlendingEnabled && renderParams.PickingContext == null) {
                GL.Enable(EnableCap.Blend);
                GL.BlendFuncSeparate(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.Zero);
                //GL.BlendFuncSeparate(BlendingFactorSrc.DstColor, BlendingFactorDest.Zero, BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                renderParams.BlendingEnabled = true;
            }
            GL.DepthMask(false); 
            //if (renderParams.DepthTestEnabled) {
            //    GL.Disable(EnableCap.DepthTest);
            //    renderParams.DepthTestEnabled = false;
            //}
        }
        protected override void EndDrawTransparent(ILRenderParameter renderParams) {
            if (renderParams.BlendingEnabled && renderParams.PickingContext == null) {
                GL.Disable(EnableCap.Blend);
                renderParams.BlendingEnabled = false;
            }
            GL.DepthMask(true);
            //if (!renderParams.DepthTestEnabled) {
            //    GL.Enable(EnableCap.DepthTest);
            //    renderParams.DepthTestEnabled = true;
            //}
        }
        protected override void BeginDrawScreen2D(ILRenderParameter renderParams) {
            base.BeginDrawScreen2D(renderParams);
            GL.Clear(ClearBufferMask.DepthBufferBit); 
            //GL.Disable(EnableCap.DepthTest); 
        }
        protected override void EndDrawScreen2D(ILRenderParameter renderParams) {
            base.EndDrawScreen2D(renderParams);
            GL.Enable(EnableCap.DepthTest); 
        }
        protected override void BeginRenderPass(ILRenderParameter renderParams) {
            base.BeginRenderPass(renderParams);
            if (renderParams.CurrentPassCount == 0) {
                // general setup 
                int failcount = 5;
                while (failcount > 0) {
                    try {
                        // this might fail while returning from suspend
                        m_control.MakeCurrent();
                        break;
                    } catch (InvalidOperationException) {
                        failcount--;
                        System.Threading.Thread.Sleep(500);
                    }
                }
                if (failcount == 0) {
                    throw new InvalidOperationException("Unable to prepare the OpenGL driver for rendering. Try restarting!");
                }
                if (ActualVersion < 3.1f) {
                    System.Diagnostics.Trace.WriteLine(String.Format("The OpenGL version of the context is '{0}'. Minimal required version is '3.1'. This context is not sufficient to continue rendering. Please update your driver and/or update to a graphics hardware which supports at least OpenGL 3.1.! Failing now ...", ActualVersion)); 
                    throw new InvalidOperationException("Insufficient OpenGL version: " + ActualVersion); 
                }
                if (renderParams.PickingContext != null) {
                    GL.ClearColor(Color.FromArgb(0)); // we start with color '1' on the shapes
                } else {
                    GL.ClearColor(m_control.BackColor);
                }
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                DepthTestEnabled = true;
                GL.PolygonOffset(1, 2);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Enable(EnableCap.PolygonOffsetFill);
            }
        }
        protected override void EndRenderPass(ILRenderParameter renderParams) {
            base.EndRenderPass(renderParams);
            if (renderParams.CurrentPassCount == 0) {
                // just finished collecting lights
                LightParameterExt lpe = m_ligthParameterExt.First;
                lpe.AmbientColor = new Vector4(Scene.AmbientLight.R / 255f, Scene.AmbientLight.G / 255f, Scene.AmbientLight.B / 255f, 1f);
                lpe.NumberOfLights = renderParams.Lights.Count;
                lpe.Attenuation = GetGlobalAttenuation(renderParams);
                m_ligthParameterExt.First = lpe;
                m_lights.Data = renderParams.Lights;
            }
        }
        internal protected override void DrawNode(ILDrawable node, ILRenderParameter parameters) {
            ILShape shape = node as ILShape; 
            if (shape != null) {
                drawOGLShape(parameters, shape); 
            } else {
                drawOGLLabel(parameters, node as ILLabel); 
            }
        }
        protected override void EndRender(ILRenderParameter renderParams) {
            if (!m_control.IsDisposed) {
                m_control.SwapBuffers();
                base.EndRender(renderParams);
            }
        }
        protected override int EndPickAt(System.Drawing.Point screen) {
            // m_control.SwapBuffers();
            int val = 0;
            unsafe {
                GL.ReadPixels(screen.X, (int)(Rectangle.Height * m_control.ClientSize.Height) - screen.Y, 1, 1, PixelFormat.Bgra, PixelType.UnsignedByte, ref val);
            }
            //m_control.SwapBuffers(); 
            // make it little endian
            return val & 0x00ffffff; // (val >> 16) & 0xFF + (val & (0xff << 8)) + ((val & 0xff) << 16); 
        } 
        #endregion

    }
}
