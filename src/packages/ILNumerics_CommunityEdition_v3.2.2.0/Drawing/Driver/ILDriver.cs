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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Security; 

namespace ILNumerics.Drawing {
    /// <remarks>Abstract driver base class for all output driver implementations</remarks>
    public abstract class ILDriver : IILDriver {

        #region Events
        public event EventHandler FPSChanged;
        protected void OnFPSChanged() {
            if (FPSChanged != null) {
                FPSChanged(this, EventArgs.Empty);
            }
        }
        public event EventHandler<ILRenderEventArgs> BeginRenderFrame;
        protected void OnBeginRenderFrame(ILRenderParameter parameter) {
            if (BeginRenderFrame != null) {
                BeginRenderFrame(this, new ILRenderEventArgs(parameter));
            }
        }
        public event EventHandler<ILRenderEventArgs> EndRenderFrame;
        protected void OnEndRenderFrame(ILRenderParameter parameter) {
            if (EndRenderFrame != null) {
                EndRenderFrame(this, new ILRenderEventArgs(parameter));
            }
        }
        public event EventHandler<ILRenderErrorEventArgs> RenderingFailed;
        protected void OnRenderingFailed(Exception exc) {
            if (RenderingFailed != null) {
                RenderingFailed(this, new ILRenderErrorEventArgs() {
                    Exception = exc
                });
            }
        }
        #endregion

        #region attributes
        public static readonly int MAX_NUMBER_LIGHTS = 20; 
        private Stopwatch m_frameStopwatch = new Stopwatch(); 
        private int m_fps;
        private int m_frameCount;
        private long m_accumFrameMS; 
        private ILGroup m_globalSceneSyncRoot; 
        private int m_errorCount = 0; 
        #endregion

        #region properties
        public abstract bool IsDisposed { get; } 
        public Color BackColor { get; set; }
        public abstract Size Size { get; set; }
        public virtual Matrix4 ViewTransform {
            get {
                return Matrix4.Translation(1,-1,0)
                            .Scale(Rectangle.Width / 2f * (Size.Width - 1), Rectangle.Height / -2f * (Size.Height - 1), 1)
                            .Translate(Rectangle.Left,Rectangle.Top,0);
            }
        }
        public ILCamera Camera { get { return Scene.First<ILCamera>(); } }
        public RectangleF Rectangle { get; set; }
        private ILScene m_scene;
        public ILScene Scene {
            get { return m_scene; }
            set {
                if (!object.ReferenceEquals(m_scene, value)) {
                    ILScene oldScene = m_scene; 
                    m_scene = value; 
                    if (oldScene != null) 
                        oldScene.Dispose(); 
                }
            }
        }
        public ILScene LocalScene { get; set; }
        public ILGroup SceneSyncRoot { 
            get { 
                if (m_globalSceneSyncRoot == null) {
                    m_globalSceneSyncRoot = Scene.Snapshot(0, null); 
                }
                return m_globalSceneSyncRoot; 
            }
            set { m_globalSceneSyncRoot = value; } 
        }
        public ILGroup LocalSceneSyncRoot { get; set; }

        public int FPS {
            get { return m_fps; }
            private set {
                if (m_fps != value) {
                    m_fps = value;
                    OnFPSChanged(); 
                }
            }
        }

        #endregion

        #region constructors
        protected ILDriver(ILScene scene) {
            if (scene == null) 
                scene = new ILScene();  
            this.Scene = scene;
            BackColor = Color.White; 
            //m_camera.Changed += new EventHandler((a,e) => { OnCameraChanged(); });
            this.Rectangle = new RectangleF(0,0,1,1); 
            this.LocalScene = new ILScene(true); 
            m_frameStopwatch = new Stopwatch(); 
            m_frameStopwatch.Start(); 
        }
        #endregion 

        #region public properties
        public virtual bool Supports(Capabilities Capability) {
            return false; 
        }
        public virtual void Configure() {
            Scene.Configure(); 
        }
        public abstract RendererTypes Driver { get; }
        public bool IsInvalidated { get; protected set; }
        protected virtual void BeginDrawTransparent(ILRenderParameter renderParams) { }
        protected virtual void EndDrawTransparent(ILRenderParameter renderParams) { }
        protected virtual void BeginRenderPass(ILRenderParameter renderParams) { }
        protected virtual void EndRenderPass(ILRenderParameter renderParams) { 
            //System.Diagnostics.Debug.WriteLine("RENDER PASS {0} FINISHED",renderParams.CurrentPassCount); 
        }
        public ILScene GetCurrentScene(long ms = 0) {
            ILScene ret = new ILScene(false); 
            ret.Add(SceneSyncRoot.Copy()); 
            ret.Add(LocalSceneSyncRoot.Copy()); 
            return ret; 
        }
        protected float GetGlobalAttenuation(ILRenderParameter renderParams) {
            float ret = 0;
            if (!Scene.MaxLightIntensity.HasValue) {
                ret = autoGlobalHDRFactor(renderParams.Lights, Scene.AmbientLight);
            } else {
                ret = (float)(1.0 / Scene.MaxLightIntensity);
            }
            return ret;
        }
        protected float autoGlobalHDRFactor(List<Light> list, Color ambient) {
            double maxIntens = 0;
            list.ForEach((l) => { if (l.Intensity > maxIntens) maxIntens = l.Intensity; });
            return (maxIntens > 0) ? (float)(1 / Math.Sqrt(maxIntens)) : 1;
        }

        [System.Security.SecurityCritical]
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptionsAttribute]
        protected void RenderInternal(ILRenderParameter renderParams, ILGroup root1, ILGroup root2) {
            try {
                #region RENDER PASS 0
                // collect all ligths, transparent nodes and screen nodes

                renderParams.CurrentPassCount = 0;
                BeginRenderPass(renderParams);
                root1.Visit(renderParams);
                root2.Visit(renderParams);
                EndRenderPass(renderParams);
                #endregion

                #region RENDER PASS 1
                // render opaque world (3D) nodes 
                renderParams.CurrentPassCount = 1;
                BeginRenderPass(renderParams);
                root1.Visit(renderParams);
                root2.Visit(renderParams);
                EndRenderPass(renderParams);
                #endregion

                #region RENDER PASS 2
                // render transparent world (3D) nodes

                renderParams.CurrentPassCount = 2;
                BeginRenderPass(renderParams);
                if (renderParams.Transparent3DNodes.Count > 0) {
                    BeginDrawTransparent(renderParams);
                    var sorted = renderParams.Transparent3DNodes.OrderBy((a) => {
                        return a.Position.Z;
                    });
                    foreach (ILPreprocessedShape prepropshape in sorted) {
                        prepropshape.VisitNode(renderParams);
                    }
                    EndDrawTransparent(renderParams);
                }
                EndRenderPass(renderParams);
                #endregion

                #region RENDER PASS 3
                // render opaque screen far nodes 
                renderParams.CurrentPassCount = 3;
                BeginRenderPass(renderParams);
                BeginDrawScreen2D(renderParams);
                if (renderParams.Screen2DFarNodes.Count > 0) {
                    foreach (ILPreprocessedShape prepropshape in renderParams.Screen2DFarNodes) {
                        prepropshape.VisitNode(renderParams);
                    }
                }
                EndRenderPass(renderParams);
                #endregion

                #region RENDER PASS 4
                // render transparent screen 2d far nodes 
                renderParams.CurrentPassCount = 4;
                BeginRenderPass(renderParams);
                if (renderParams.TransparentScreen2DFarNodes.Count > 0) {
                    BeginDrawTransparent(renderParams);
                    var sorted = renderParams.TransparentScreen2DFarNodes.OrderBy((a) => {
                        return a.Position.Z;
                    });
                    foreach (ILPreprocessedShape prepropshape in sorted) {
                        prepropshape.VisitNode(renderParams);
                    }
                    EndDrawTransparent(renderParams);
                }
                EndDrawScreen2D(renderParams);
                EndRenderPass(renderParams);
                #endregion

                #region RENDER PASS 5
                // render opaque screen near nodes 
                renderParams.CurrentPassCount = 5;
                BeginRenderPass(renderParams);
                BeginDrawScreen2D(renderParams);
                if (renderParams.Screen2DNearNodes.Count > 0) {
                    foreach (ILPreprocessedShape prepropshape in renderParams.Screen2DNearNodes) {
                        prepropshape.VisitNode(renderParams);
                    }
                }
                EndRenderPass(renderParams);
                #endregion

                #region RENDER PASS 6
                // render transparent screen 2d near nodes 
                renderParams.CurrentPassCount = 6;
                BeginRenderPass(renderParams);
                if (renderParams.TransparentScreen2DNearNodes.Count > 0) {
                    BeginDrawTransparent(renderParams);
                    var sorted = renderParams.TransparentScreen2DNearNodes.OrderBy((a) => {
                        return a.Position.Z;
                    });
                    foreach (ILPreprocessedShape prepropshape in sorted) {
                        prepropshape.VisitNode(renderParams);
                    }
                    EndDrawTransparent(renderParams);
                }
                EndDrawScreen2D(renderParams);
                EndRenderPass(renderParams);
                #endregion
            } catch (Exception exc) {
                if (IsDisposed) {
                    // an event has been left over after removing this driver... ignore
                    System.Diagnostics.Trace.WriteLine("Ignoring failure on RenderInternal, because the driver has been disposed already.");
                    System.Diagnostics.Trace.WriteLine(exc.ToString());
                } else {
                    if (m_errorCount++ < 20) {
                        System.Diagnostics.Trace.WriteLine("");
                        System.Diagnostics.Trace.WriteLine("RenderInternal() Failure #" + m_errorCount.ToString());
                        System.Diagnostics.Trace.WriteLine("===============================");
                        System.Diagnostics.Trace.WriteLine(exc.ToString());
                        OnRenderingFailed(exc);
                    } else if (m_errorCount == 21) {
                        System.Diagnostics.Trace.WriteLine("More than 20 errors in RenderInternal. Further error reporting has been disabled.");
                    }
                }
            }
        }
        protected virtual void BeginDrawScreen2D(ILRenderParameter renderParams) {
            //renderParams.DepthTestEnabled = false; 
        }

        protected virtual void EndDrawScreen2D(ILRenderParameter renderParams) {
            //renderParams.DepthTestEnabled = true; 
        }

        internal virtual void VisitNode(ILDrawable node, ILRenderParameter parameters) {
            switch (parameters.CurrentPassCount) {
                case 0:
                    if (parameters.RenderTargets.Peek() == RenderTarget.Screen2DFar) {
                        if (node.IsTransparent || parameters.Alpha.Peek() < 1) {
                            Vector3 pos = parameters.CurrentModel2CameraTransform * node.GetPosition();
                            parameters.TransparentScreen2DFarNodes.Add(new ILPreprocessedShape(node, parameters, pos));
                        } else {
                            Vector3 pos = parameters.CurrentModel2CameraTransform * node.GetPosition();
                            parameters.Screen2DFarNodes.Add(new ILPreprocessedShape(node, parameters, pos));
                        }
                    } else if (parameters.RenderTargets.Peek() == RenderTarget.Screen2DNear) {
                        if (node.IsTransparent || parameters.Alpha.Peek() < 1) {
                            Vector3 pos = parameters.CurrentModel2CameraTransform * node.GetPosition();
                            parameters.TransparentScreen2DNearNodes.Add(new ILPreprocessedShape(node, parameters, pos));
                        } else {
                            Vector3 pos = parameters.CurrentModel2CameraTransform * node.GetPosition();
                            parameters.Screen2DNearNodes.Add(new ILPreprocessedShape(node, parameters, pos)); 
                        }
                    } else if (node.IsTransparent || parameters.Alpha.Peek() < 1) {
                        Vector3 pos = parameters.CurrentModel2CameraTransform * node.GetPosition();
                        parameters.Transparent3DNodes.Add(new ILPreprocessedShape(node, parameters, pos));
                    }
                    break;
                default:
                    System.Diagnostics.Debug.Assert(parameters.CurrentPassCount == 1);
                    if (parameters.RenderTargets.Peek() == RenderTarget.World3D && !node.IsTransparent && parameters.Alpha.Peek() >= 1) {
                        DrawNode(node, parameters);
                    }
                    break;
                //case 2:
                //    if (parameters.RenderTargets.Peek() == RenderTarget.World3D && (node.IsTransparent || parameters.Alpha.Peek() < 1)) {   // this check should not be needed? only transparent nodes are delivered here? 
                //        DrawNode(node, parameters);
                //    }
                //    break;
                //case 3:
                //    if (parameters.RenderTargets.Peek() == RenderTarget.Screen2DNear && !node.IsTransparent && parameters.Alpha.Peek() >= 1) {
                //        DrawNode(node, parameters);
                //    }
                //    break;
                //case 4:
                //    if (parameters.RenderTargets.Peek() == RenderTarget.Screen2DNear && (node.IsTransparent || parameters.Alpha.Peek() < 1)) {
                //        DrawNode(node, parameters);
                //    }
                //    break;
                //default:
                //    break;
            }
        }

        /// <summary>
        /// Default implementation, simply print out a copy of all shapes data
        /// </summary>
        /// <param name="node">the shape to draw</param>
        internal protected virtual void DrawNode(ILDrawable node, ILRenderParameter parameters) {
            if (node is ILLabel) {
                RenderText((ILLabel)node, parameters);
            } else if (node is ILShape) {
                ILShape shape = (ILShape)node;
                switch (shape.Type) {
                    case Primitives.Points:
                        RenderPoints((ILPoints)shape, parameters);
                        break;
                    case Primitives.Lines:
                    case Primitives.LineStrip:
                        RenderLines((ILLines)shape, parameters);
                        break;
                    case Primitives.Triangles:
                    case Primitives.TriangleStrip:
                    case Primitives.TriangleFan:
                        RenderTriangles((ILTriangles)shape, parameters);
                        break;
                    default:
                        break;
                }
            }
        }

        protected virtual void RenderText(ILLabel label, ILRenderParameter parameters) {
            //System.Diagnostics.Debug.WriteLine("Label: " + label.Text); 
        }

        protected virtual void RenderTriangles(ILTriangles triangles, ILRenderParameter parameters) {
            //System.Diagnostics.Debug.WriteLine("Triangles: " + triangles.ID);
        }

        protected virtual void RenderLines(ILLines lines, ILRenderParameter parameters) {
            //System.Diagnostics.Debug.WriteLine("Lines: " + lines.ID);
        }

        protected virtual void RenderPoints(ILPoints points, ILRenderParameter parameters) {
            //System.Diagnostics.Debug.WriteLine("Points: " + points.ID);
        }

        [SecuritySafeCritical]
        public void Render(long ms = 0) {
            //System.Diagnostics.Debug.WriteLine("******* BEGIN RENDER ****************************************************"); 
            #region measure FPS
            m_frameCount ++;
            m_frameStopwatch.Stop();
            long frameElapsed = m_frameStopwatch.ElapsedMilliseconds;
            m_frameStopwatch.Restart();
            m_accumFrameMS += frameElapsed;
            if (m_accumFrameMS >= 1000) {
                FPS = m_frameCount;
                m_frameCount = 0;
                m_accumFrameMS = 0;
            }
            #endregion
            ILRenderParameter renderParams = new ILRenderParameter(this);
            renderParams.Time_ms = ms; 

            BeginRender(renderParams);
            SceneSyncRoot = Scene.Snapshot(ms, m_globalSceneSyncRoot);
            LocalSceneSyncRoot = LocalScene.Snapshot(ms, LocalSceneSyncRoot);


            RenderInternal(renderParams, SceneSyncRoot, LocalSceneSyncRoot);

            EndRender(renderParams);
            //System.Diagnostics.Debug.WriteLine("******* END RENDER ****************************************************");
        }

        [SecuritySafeCritical]
        public virtual int? PickAt(System.Drawing.Point screenCoords, long time) {
            BeginPickAt();
            SceneSyncRoot = Scene.Snapshot(time, SceneSyncRoot);
            LocalSceneSyncRoot = LocalScene.Snapshot(time, LocalSceneSyncRoot);
            ILRenderParameter renderParams = new ILRenderParameter(this) {
                PickingContext = new ILPickingContext() {
                    Location = screenCoords
                },
            };
            RenderInternal(renderParams, SceneSyncRoot, LocalSceneSyncRoot);
            int col = EndPickAt(screenCoords);
            //System.Diagnostics.Debug.WriteLine("Picked Color: {0:x}", col); 
            if (renderParams.PickingContext.ColorShapeMapping.ContainsKey(col)) {
                return renderParams.PickingContext.ColorShapeMapping[col];
            } else if (renderParams.PickingContext.CurrentScreenRectTarget != null) {
                return renderParams.PickingContext.CurrentScreenRectTarget.ID;
            }
            return null; 
        }

        /// <summary>
        /// Fires the OnBeginRenderFrame event
        /// </summary>
        /// <param name="renderParams"></param>
        /// <remarks>This is not guaranteed to be called in certain scenarios. Do not rely on this function to be called for important driver setups! Use BeginRenderPass instead. </remarks>
        protected virtual void BeginRender(ILRenderParameter renderParams) { OnBeginRenderFrame(renderParams);  }
        protected virtual void EndRender(ILRenderParameter renderParams) { OnEndRenderFrame(renderParams); }
        protected virtual void BeginPickAt() { }
        protected virtual int EndPickAt(System.Drawing.Point screen) {
            return 0; 
        }
        
        /// <summary>
        /// vertex based lighting equations (Lambert' + Gauss) 
        /// </summary>
        /// <param name="col">Diffuse color</param>
        /// <param name="pos">position (camera coords)</param>
        /// <param name="normal">normal (camera coords, properly scaled)</param>
        /// <param name="Emission">emissive color</param>
        /// <param name="Shininess">shininess exponent</param>
        /// <param name="Specular">specular color</param>
        /// <param name="parameters">render parameter</param>
        internal Vector4 ComputeLight(Vector4 col, Vector3 pos, Vector3 normal,
                                  float Shininess, Color Specular, Color Emission,
                                  ILRenderParameter parameters) {
            // ambient ligth
            Vector3 accumLight = new Vector3(col.X * Scene.AmbientLight.R, col.Y * Scene.AmbientLight.G, col.Z * Scene.AmbientLight.B) / 255;
            Vector3 normNormal = Vector3.Normalize(normal);
            Vector3 normCam2Fragment = Vector3.Normalize(pos);
            for (int i = 0; i < parameters.Lights.Count; i++) {
                // attenuation: diffuse 
                Light l = parameters.Lights[i];
                Vector3 direction2Light = (l.Position.Xyz - pos);
                float lightDistSqr = Vector3.Dot(direction2Light, direction2Light);
                direction2Light = direction2Light / (float)Math.Sqrt(lightDistSqr);
                float AOI = Vector3.Dot(direction2Light, normNormal);
                if (AOI < 0.0001) AOI = 0;
                if (AOI > 1) AOI = 1;

                // attenuation: distance
                float att = 1f / (1f + lightDistSqr);

                // attenuation: specular
                float matchFact;
                if (direction2Light != normCam2Fragment) {
                    Vector3 halfAngle = Vector3.Normalize(direction2Light - normCam2Fragment);
                    matchFact = Vector3.Dot(halfAngle, normNormal);
                } else {
                    matchFact = 0;
                }
                double expnt = Math.Acos(matchFact) / Shininess;
                double specAtt = Math.Exp(-(expnt * expnt));
                specAtt = (AOI > 0) ? specAtt : 0;

                // put it all together
                Vector3 thisLight = (l.Color * new Vector3(col.X, col.Y, col.Z) * AOI * att
                                    + new Vector3(Specular.R, Specular.G, Specular.B) / 255f * (float)specAtt) * l.Intensity
                                    + new Vector3(Emission.R, Emission.G, Emission.B) / 255f;
                accumLight += thisLight;
            }
            // global attenuation and gamma correction
            accumLight = Vector3.Pow(accumLight * GetGlobalAttenuation(parameters), 1 / 2.2f);
            // clipping (System.Color does not allow values outside [0..255])
            if (accumLight.X > 1) accumLight.X = 1;
            if (accumLight.Y > 1) accumLight.Y = 1;
            if (accumLight.Z > 1) accumLight.Z = 1;
            return new Vector4(accumLight, col.W);
        }

        internal static ILDashInfo StippleFromLineStyle(ILLines lines) {
            var ret = new ILDashInfo(); 
            ret.Pattern = 1;
            ret.Factor = 1f;
            switch (lines.DashStyle) {
                case DashStyle.Dashed:
                    ret.Pattern = (short)3855; // 0000111100001111
                    ret.Factor = 1 / 2f;
                    break;
                case DashStyle.PointDash:
                    ret.Pattern = (short)255 + 2048;
                    ret.Factor = 1 / 2f;
                    break;
                case DashStyle.Dotted:
                    ret.Pattern = (short)3171; // 000110001100011;
                    ret.Factor = 1 / 1f;
                    break;
                case DashStyle.UserPattern:
                    ret.Pattern = lines.Pattern;
                    ret.Factor = 1 / lines.PatternScale;
                    break;
                default:      // solid
                    ret.Pattern = (short)-1;
                    ret.Factor = 1 / 1f;
                    break;
            }
            return ret;
        }
        protected void moveInvalidLineEndOntoBorder(int x1, int y1, ref int x2, ref int y2, ref int dX, ref int dY) {
            // find crossing with every border  // TODO: handle dx = 0 !
            if (dX != 0) {
                if (x2 > x1) {
                    int dynew = (int)(y1 + ((Size.Width - 1) - x1) / (float)dX * (y2 - y1));
                    if (dynew >= 0 && dynew < Size.Height) {
                        x2 = Size.Width - 1;
                        y2 = dynew;
                        dX = Math.Abs(x2 - x1);
                        dY = Math.Abs(y2 - y1);
                        return;
                    }
                } else {
                    int dynew = (int)(y1 + x1 / (float)dX * (y2 - y1)); // TODO: handle dx = 0 !
                    if (dynew >= 0 && dynew < Size.Height) {
                        x2 = 0;
                        y2 = dynew;
                        dX = Math.Abs(x2 - x1);
                        dY = Math.Abs(y2 - y1);
                        return;
                    }
                }
            }
            if (dY != 0) {
                if (y1 > y2) {
                    int dxnew = (int)(x1 + y1 / (float)dY * (x2 - x1)); // TODO: handle dy = 0 ! ..ff
                    if (dxnew >= 0 && dxnew < Size.Width) {
                        x2 = dxnew;
                        y2 = 0;
                        dX = Math.Abs(x2 - x1);
                        dY = Math.Abs(y2 - y1);
                        return;
                    }
                } else {
                    // y1 < y2
                    int dxnew = (int)(x1 + ((Size.Height - 1) - y1) / (float)dY * (x2 - x1));
                    System.Diagnostics.Debug.Assert(dxnew >= 0 && dxnew < Size.Width, "expected at least one crossing with border");
                    x2 = dxnew;
                    y2 = Size.Height - 1;
                    dX = Math.Abs(x2 - x1);
                    dY = Math.Abs(y2 - y1);
                    return;
                }
            }
        }

        #endregion 

        #region IILGroup Members

        public ILGroup Add(ILGroup group, object tag = null) {
            return Scene.Camera.Add(group, tag); 
        }

        public T Add<T>(T node, object tag = null) where T : ILNode {
            return Scene.Camera.Add(node, tag); 
        }

        public bool Remove(ILNode node) {
            return Scene.Remove(node); 
        }
        public IEnumerable<T> Find<T>(object tag = null, Predicate<T> predicate = null)
            where T : ILNode {
                return Scene.Find<T>(tag, predicate); 
        }
        public IEnumerable<ILNode> Find(object tag, Primitives? Kind = null) {
            return Scene.Find(tag, Kind);
        }
        public ILNode FindById(int id) {
            return Scene.FindById<ILNode>(id);
        }

        #endregion
    }
}
