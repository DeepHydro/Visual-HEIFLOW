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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ILNumerics.Drawing {
    /// <summary>
    /// The main Windows.Forms rendering panel in ILNumerics, supports various drivers
    /// </summary>
    public partial class ILPanel : UserControl, IILDriver {

        #region Events
        /// <summary>
        /// Fires when the number of rendered frames per second changes
        /// </summary>
        public event EventHandler FPSChanged;
        protected void OnFPSChanged() {
            if (FPSChanged != null) {
                FPSChanged(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Fires when the rendering of a frame was started
        /// </summary>
        public event EventHandler<ILRenderEventArgs> BeginRenderFrame;
        protected void OnBeginRenderFrame(ILRenderParameter parameter) {
            if (BeginRenderFrame != null) {
                BeginRenderFrame(this, new ILRenderEventArgs(parameter));
            }
        }
        /// <summary>
        /// Fires when the rendering of a frame was finished
        /// </summary>
        public event EventHandler<ILRenderEventArgs> EndRenderFrame;
        protected void OnEndRenderFrame(ILRenderParameter parameter) {
            if (EndRenderFrame != null) {
                EndRenderFrame(this, new ILRenderEventArgs(parameter));
            }
        }
        /// <summary>
        /// Fires when the rendering of a frame failed
        /// </summary>
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
        bool m_initialized = false; 
        RendererTypes m_rendererType = RendererTypes.OpenGL;
        Control m_control;
        private ILEditor m_editor;
        #endregion

        #region constructors
        public ILPanel() {
            //m_editor = new ILEditor();
            //this.Dock = DockStyle.Fill; 
            
            replaceControl(m_rendererType); 
            ShowUIControls = false; 
            InitializeComponent();
            this.Rectangle = new RectangleF(0,0,1,1);
            //if (ShowUIControls) {
            //    Editor = new ILEditor(); 
            //}
            RenderingFailed += RenderingFailedInformUser;
        }
        #endregion 

        #region protected overloads

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Configure(); // in case the user forgot to call Configure in panel_load
        }
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (m_control != null) // this absolutely might happen !!
                m_control.Size = Size; 
        }
        protected override void OnParentChanged(EventArgs e) {
            if (Parent != null) {
                if (!ILHelper.IsVSTO(this)) {
                    Dock = DockStyle.Fill;
                } else {
                    ILHelper.s_isDesignMode = false;
                }
                replaceControl(m_rendererType);
                this.Rectangle = new RectangleF(0, 0, 1, 1);
            }
            base.OnParentChanged(e);
        }
        private void timer1_Tick(object sender, EventArgs e) {
            Refresh(); 
        }
        #endregion

        #region IILDriver Members
        [Browsable(false)]
        public ILClock Clock {
            get {
                if (ILControl as ILGDIControl != null) {
                    return (ILControl as ILGDIControl).Clock;
                } else if (ILControl as ILOGLControl != null) {
                    return (ILControl as ILOGLControl).Clock;
                } else {
                    return null;
                }
            }
        }
        /// <summary>
        /// [Reserved for future use] Determines if extended interactive UI controls are shown at runtime
        /// </summary>
        [Browsable(false)]
        public bool ShowUIControls { get; set; }
        /// <summary>
        /// [Reserved for future use]
        /// </summary>
        [Browsable(false)]
        public ILEditor Editor {
            get {
                return m_editor; 
            }
            set {
                if (m_editor != null) {
                    MouseMove -= (s, a) => { m_editor.BlendIn(); };
                    LocalScene.Screen.Remove(m_editor);
                }
                if (value != null) {
                    m_editor = value; 
                    MouseMove += (s, a) => { m_editor.BlendIn(); };
                    LocalScene.Screen.Add(m_editor);
                }
            }
        }
        /// <summary>
        /// Gets the current frame rate (frames per second) when the clock is running
        /// </summary>
        [Browsable(false)]
        public int FPS { 
            get { return ILControl.FPS; }
        }

        internal IILDriver ILControl {
            get {
                return (m_control as IILDriver); 
            }
        }
        /// <summary>
        /// Get the back color of the control or sets it
        /// </summary>
        public override Color BackColor {
            get {
                return ILControl.BackColor;
            }
            set {
                ILControl.BackColor = value;
            }
        }
        /// <summary>
        /// Get a reference to the default camera of the scene
        /// </summary>
        [Browsable(false)]
        public ILCamera Camera {
            get { 
                return ILControl.Camera;  
            }
        }
        /// <summary>
        /// Trigger the rendering of a complete frame
        /// </summary>
        /// <param name="timeMs"></param>
        public void Render(long timeMs) {
            ILControl.Render(timeMs);
        }
        /// <summary>
        /// Configure panel and scene after modifications to any buffers
        /// </summary>
        public void Configure() {
            ILControl.Configure(); 
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ILScene Scene {
            get {
                return ILControl.Scene;
            }
            set {
                ILControl.Scene = value;
            }
        }
        /// <summary>
        /// Gets the rectangular area which this panel occupies inside its container control or sets that rectangle
        /// </summary>
        public RectangleF Rectangle {
            get {
                return ILControl.Rectangle; 
            }
            set {
                ILControl.Rectangle = value;
            }
        }
        /// <summary>
        /// [Reserved for future use]
        /// </summary>
        /// <param name="Capability">Capability to query</param>
        /// <returns>True if this driver supports the capability requested, false otherwise</returns>
        public bool Supports(Capabilities Capability) {
            return ILControl.Supports(Capability); 
        }
        /// <summary>
        /// Get the rectangular area as fraction of the area defined by Rectangle, which is used to render the scene to. ViewTransform Matrix4.
        /// </summary>
        public Matrix4 ViewTransform {
            get {
                return ILControl.ViewTransform;
            }
        }
        /// <summary>
        /// Gets the driver used to render the scene to the rendering surface or sets it. Default: OpenGL, fallback: GDI
        /// </summary>
        public RendererTypes Driver {
            get {
                return ILControl.Driver; 
            }
            set {
                if (m_rendererType != value) {
                    m_rendererType = value;
                    replaceControl(m_rendererType);
                }
            }
        }
        #endregion

        #region private helpers
        private Control removeControl() {
            if (m_control == null) return null;
            m_control.MouseUp -= (s, e) => { OnMouseUp(e); };
            m_control.MouseDown -= (s, e) => { OnMouseDown(e); };
            m_control.MouseMove -= (s, e) => { OnMouseMove(e); };
            m_control.MouseWheel -= (s, e) => { OnMouseWheel(e); };
            m_control.MouseEnter -= (s, e) => { OnMouseEnter(e); };
            m_control.MouseLeave -= (s, e) => { OnMouseLeave(e); };
            m_control.Click -= (s, e) => { OnClick(e); };
            m_control.DoubleClick -= (s, e) => { OnDoubleClick(e); };


            Controls.Remove(m_control);
            if (ILControl is IDisposable && ILControl != null) {
                ILControl.FPSChanged -= (s, a) => { OnFPSChanged(); };
                ILControl.BeginRenderFrame -= (s, a) => { OnBeginRenderFrame(a.Parameter); };
                ILControl.EndRenderFrame -= (s, a) => { OnEndRenderFrame(a.Parameter); };
                ILControl.RenderingFailed -= (s, a) => { OnRenderingFailed(a.Exception); };
                (ILControl as IDisposable).Dispose();
            }
            return m_control;
        }
        private void replaceControl(RendererTypes type) {
            //if (ILControl != null && type == Driver) return; 
            bool loading = true; 
            while (true) {
                try {
                    ILScene scene = (ILControl != null) ? ILControl.Scene : null; 
                    Control oldControl = removeControl(); 
                    switch (type) {
                        case RendererTypes.GDI:
                            m_control = new ILGDIControl();
                            break;
                        case RendererTypes.OpenGL:
                            var oglcontrol = new ILOGLControl();
                            m_control = oglcontrol;  
                            // the creation may fails later... check now
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    Controls.Add(m_control);
                    ILControl.Size = Size;
                    if (oldControl != null) {
                        m_control.BackColor = oldControl.BackColor;
                        while (ILControl.Scene.Children.Count() > 0) 
                            ILControl.Scene.Remove(ILControl.Scene.Children.First<ILNode>()); 
                        ILControl.Scene.TakeChildsFrom(scene);
                    }
                    //input events
                    m_control.MouseUp += (s, e) => { OnMouseUp(e); };
                    m_control.MouseDown += (s, e) => { OnMouseDown(e); };
                    m_control.MouseMove += (s, e) => { OnMouseMove(e); };
                    m_control.MouseEnter += (s, e) => { OnMouseEnter(e); };
                    m_control.MouseLeave += (s, e) => { OnMouseLeave(e); };
                    m_control.MouseWheel += (s, e) => { OnMouseWheel(e); };
                    m_control.Click += (s, e) => { OnClick(e); };
                    m_control.DoubleClick += (s, e) => { OnDoubleClick(e); };
                    ILControl.FPSChanged += (s, a) => { OnFPSChanged(); };
                    ILControl.BeginRenderFrame += (s, a) => { OnBeginRenderFrame(a.Parameter); };
                    ILControl.EndRenderFrame += (s, a) => { OnEndRenderFrame(a.Parameter); };
                    ILControl.RenderingFailed += (s, a) => { OnRenderingFailed(a.Exception); };
                    break; 
                } catch (Exception exc) {
                    Trace.WriteLine("Failed to load / add driver: " + m_rendererType);
                    Trace.WriteLine(exc.ToString());
 
                    if (loading && m_rendererType != RendererTypes.GDI) {
                        Trace.WriteLine("Attempting to fallback to GDI ... "); 
                        m_rendererType = RendererTypes.GDI;
                        loading = false; 
                    } else {
                        Trace.WriteLine("2nd attempt failed. Unable to recover ... sorry.");
                        throw new Exceptions.ILArgumentException("could not initialize renderer", exc); 
                    }
                }
            }
        }

        private void RenderingFailedInformUser(object sender, ILRenderErrorEventArgs arg) {
            replaceControl(RendererTypes.GDI);
            if (Settings.ShowMessageBoxOnGDIFallback) {
                string causeInfo = "";
                if (arg.Exception != null) {
                    causeInfo = Environment.NewLine; 
                    if (!String.IsNullOrEmpty(arg.Exception.Message)) {
                        causeInfo += "Error reported: '" + arg.Exception.Message + "'" + Environment.NewLine;
                    } else {
                        causeInfo += "An exception of type '" + arg.Exception.GetType().Name + "' has been generated." + Environment.NewLine;
                    }
                    if (arg.Exception.InnerException != null && !String.IsNullOrEmpty(arg.Exception.InnerException.Message)) {
                        causeInfo += "Additional information: " + arg.Exception.InnerException.Message + Environment.NewLine;
                    }
                }
                string text = String.Format(@"No compatible hardware accelerated driver could be found or activated. Hardware acceleration has been disabled for the current session. Rendering will continue with software rendering (GDI+) only. This may cause lower framerates for large scenes.
{0}
Potential problems with accelerated drivers are often caused by... 

* Outdated drivers: for OpenGL GPU rendering, currently OpenGL version 3.1 or higher is required. Try updating your drivers to the most uptodate version available! Please consult your hardware vendor for latest driver downloads! 
* Driver bugs: especially 'first day support' drivers often show bugs with newer OpenGL functionality. Try updating your drivers! 
* Deactivated drivers: mobile devices often deactivate GPU rendering for energy saving purposes. Make sure, no energy saving plan is preventing the application from accelerated GPU processing! One common place to check for NVIDIA cards is: Control Panel -> NVIDIA Control Panel -> 3D Settings Management | Preferred Graphics Processor: NVIDIA High Performance GPU. 
* Remote desktop sessions often do not support recent OpenGL versions.

The application trace log may contain further details about the cause of the problem. 
", causeInfo);
                System.Diagnostics.Trace.WriteLine("Rendering failed .... falling back to GDI renderer.");
                System.Diagnostics.Trace.WriteLine("Reason(s): " + causeInfo);
                try {
                    MessageBox.Show(text, "ILNumerics - determining rendering device....", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                } catch (Exception exc) {
                    System.Diagnostics.Trace.WriteLine("The message box informing the user about the rendering failure could not be shown.");
                    System.Diagnostics.Trace.WriteLine("Reason:");
                    System.Diagnostics.Trace.WriteLine(exc.ToString());
                }
            }
        }

        #endregion

        #region IILDriver Members
        /// <summary>
        /// Returns a scene which reflects the rendering result, including all local compositions and modifications
        /// </summary>
        /// <param name="ms">[optional] time of render frame</param>
        /// <returns>scene composition as copies of local and global scene, including user interactions and auto generated shapes (ticks, camera etc.)</returns>
        public ILScene GetCurrentScene(long ms = 0) {
            return ILControl.GetCurrentScene(ms);
        }
        /// <summary>
        /// Determines the ID of the shape according to a specific pixel position inside the Rectangle output area at a specific point in time
        /// </summary>
        /// <param name="screenCoords">pixel coords, (0,0) is at upper left corner</param>
        /// <param name="timeMs">the point in time for rendering</param>
        /// <returns>the id of the shape which exists at the given screen coordinates, null if no such shape exists</returns>
        public int? PickAt(System.Drawing.Point screenCoords, long timeMs) {
            return ILControl.PickAt(screenCoords, timeMs); 
        }
        /// <summary>
        /// Gets the scene which is only maintained by this panel and not shared between multiple drivers.
        /// </summary>
        [Browsable(false)]
        public ILScene LocalScene {
            get {
                return ILControl.LocalScene; 
            }
        }
        /// <summary>
        /// Gets the rendering scene which is constantly synchronized/derived with/from the (global) Scene 
        /// </summary>
        [Browsable(false)]
        public ILGroup SceneSyncRoot {
            get {
                return ILControl.SceneSyncRoot;
            }
        }
        /// <summary>
        /// Get the rendering scene which is constantly synchronized/derived with/from the LocalScene  
        /// </summary>
        [Browsable(false)]
        public ILGroup LocalSceneSyncRoot {
            get {
                return ILControl.LocalSceneSyncRoot;
            }
        }
        #endregion


    }
}
