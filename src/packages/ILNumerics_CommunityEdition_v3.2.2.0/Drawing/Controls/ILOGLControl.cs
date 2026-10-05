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
using System.Windows.Forms; 
using System.Linq;
using System.Drawing; 
using System.Text;
using OpenTK;
using System.Reflection; 

namespace ILNumerics.Drawing {
    [System.Security.SecuritySafeCritical]
    public class ILOGLControl : GLControl, IILDriver {

        #region events
        public event EventHandler FPSChanged;
        protected void OnFPSChanged() {
            if (FPSChanged != null)
                FPSChanged(this, EventArgs.Empty);
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
        ILDriver m_driver;
        private System.ComponentModel.IContainer components;
        private ILInputController m_inputController;
        private ILClock m_timer;
        #endregion

        #region constructors
        public ILOGLControl()
            : base(OpenTK.Graphics.GraphicsMode.Default, 3, 1, OpenTK.Graphics.GraphicsContextFlags.Default) {
                try {
                    InitializeComponent();
                    m_timer = new ILClock() { Running = false };
                    /**
                     * we cannot check for Design Mode here (because it will not work). But in Design Mode 
                     * there will be no valid OpenGL context either so we create a GDI driver as dummy.
                     **/
                    ILBackBuffer bbuffer = new ILBackBuffer();
                    bbuffer.Rectangle = ClientRectangle;
                    m_driver = new ILGDIDriver(bbuffer);
                    // only up from this point 'Driver' and rel. properties will work!  
                    m_inputController = new ILInputController(m_driver);
                    BackColor = Color.White;
                } catch (Exception exc) {
                    System.Diagnostics.Trace.WriteLine("Exception in ILOGLControl(): " + exc.ToString()); 
                    throw; 
                }
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets the clock which provides the time base for animations
        /// </summary>
        public ILClock Clock {
            get {
                return m_timer;
            }
        }
        public override Color BackColor {
            get {
                return m_driver.BackColor;
            }
            set {
                if (m_driver != null)
                    m_driver.BackColor = value;
            }
        }
        #endregion

        #region public interface 
        // cannot catch SEH exceptions here, must modify OpenTK.GLControl..
        //[System.Security.SecurityCritical]
        //[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        protected override void OnHandleCreated(EventArgs e) {
            try {
                base.OnHandleCreated(e);
                if (!isDesignMode())
                    System.Windows.Forms.Application.Idle += Application_Idle;
            } catch (Exception exc) {
                System.Diagnostics.Trace.WriteLine("Error creating OpenGL graphics context: ");
                System.Diagnostics.Trace.WriteLine(exc.ToString());
                OnRenderingFailed(exc);
            }
        }
        protected override void OnHandleDestroyed(EventArgs e) {
            base.OnHandleDestroyed(e);
            System.Windows.Forms.Application.Idle -= Application_Idle;
        }
        protected override void OnParentChanged(EventArgs e) {
            base.OnParentChanged(e);
            if (!isDesignMode() && Parent != null) {
                ILDriver oldDriver = m_driver; 
                ILOGLDriver newDriver = new ILOGLDriver(this);
                m_driver = newDriver; 
                m_driver.Size = ClientSize;
                newDriver.RenderingFailed += (s, arg) => { OnRenderingFailed(arg.Exception); }; 
                if (oldDriver != null) {
                    oldDriver.FPSChanged -= (s, a) => { OnFPSChanged(); }; 
                    m_driver.BackColor = oldDriver.BackColor;
                    //m_driver.LocalScene.Camera.TakeChildsFrom(oldDriver.LocalScene.Camera);
                    m_driver.LocalScene.Screen.TakeChildsFrom(oldDriver.LocalScene.Screen);
                    // todo: transfer other properties may already stored in oldDriver
                }
                m_inputController.Driver = m_driver;
                m_driver.FPSChanged += (s, a) => { OnFPSChanged(); };
                m_driver.BeginRenderFrame += (s, a) => { OnBeginRenderFrame(a.Parameter); };
                m_driver.EndRenderFrame += (s, a) => { OnEndRenderFrame(a.Parameter); }; 
                m_driver.BackColor = base.BackColor; 
            }
        }
        protected override void OnMouseClick(MouseEventArgs e) {
            InputController.OnMouseClick(e.ToILMouseEventArgs(Size, m_timer.TimeMilliseconds));
            base.OnMouseClick(e);
        }
        protected override void OnMouseDoubleClick(MouseEventArgs e) {
            InputController.OnMouseDoubleClick(e.ToILMouseEventArgs(Size, m_timer.TimeMilliseconds));
            base.OnMouseDoubleClick(e);
        }
        protected override void OnMouseDown(MouseEventArgs e) {
            InputController.OnMouseDown(e.ToILMouseEventArgs(Size, m_timer.TimeMilliseconds));
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            InputController.OnMouseUp(e.ToILMouseEventArgs(Size, m_timer.TimeMilliseconds));
            base.OnMouseUp(e);
        }
        protected override void OnMouseEnter(EventArgs e) {
            InputController.OnMouseEnter(ILMouseEventArgs.Empty);
            base.OnMouseEnter(e);
        }
        protected override void OnMouseLeave(EventArgs e) {
            InputController.OnMouseLeave(ILMouseEventArgs.Empty);
            base.OnMouseLeave(e);
        }
        protected override void OnMouseWheel(MouseEventArgs e) {
            InputController.OnMouseWheel(e.ToILMouseEventArgs(Size, m_timer.TimeMilliseconds));
            base.OnMouseWheel(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            InputController.OnMouseMove(e.ToILMouseEventArgs(Size, m_timer.TimeMilliseconds));
            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (m_driver != null) {
                m_driver.Size = ClientSize;
                Invalidate();
            }
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
            if (isDesignMode()) {
                ILGDIControl.DrawBranding(e, BackColor, ClientRectangle, "ILNumerics ILPanel (OpenGL)");
            } else {
                m_driver.Render(m_timer.TimeMilliseconds);
            }
        }
        #endregion

        #region private helpers

        private bool isDesignMode() {
            return ILHelper.IsDesignMode(); 
        }
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            // 
            // ILOGLControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.Name = "ILOGLControl";
            this.ResumeLayout(false);

        }
        void Application_Idle(object sender, EventArgs e) {
            if (m_timer.Running)
                Invalidate();
        }
        #endregion

        #region IILDriver Members
        /// <summary>
        /// Return a scene which reflects the rendering result, including all local compositions and modifications
        /// </summary>
        /// <param name="ms">[optional] time of render frame</param>
        /// <returns>scene composition as copies of local and global scene with user interaction</returns>
        public ILScene GetCurrentScene(long ms = 0) {
            return m_driver.GetCurrentScene(ms);
        }
        public ILCamera Camera {
            get {
                return m_driver.Camera;
            }
        }
        public void Configure() {
            m_driver.Configure();
        }
        public void Render(long timeMs) {
            m_driver.Render(timeMs);
        }
        public ILScene Scene {
            get { 
                return m_driver.Scene;  }
            set { 
                m_driver.Scene = value; 
            }
        }
        public ILScene LocalScene {
            get {
                return m_driver.LocalScene;
            }
        }
        public ILGroup SceneSyncRoot {
            get {
                return m_driver.SceneSyncRoot;
            }
        }
        public ILGroup LocalSceneSyncRoot {
            get {
                return m_driver.LocalSceneSyncRoot;
            }
        }
        public System.Drawing.RectangleF Rectangle {
            get { 
                return m_driver.Rectangle; 
            }
            set {
                m_driver.Rectangle = value; 
            }
        }
        public bool Supports(Capabilities Capability) {
            return m_driver.Supports(Capability);
        }
        public Matrix4 ViewTransform {
            get {
                return m_driver.ViewTransform;  
            }
        }
        public RendererTypes Driver {
            get { return RendererTypes.OpenGL; }
        }
        #endregion

        #region IILControl Members
        public int? PickAt(System.Drawing.Point screenCoords, long timeMs) {
            return m_driver.PickAt(screenCoords, timeMs); 
        }
        #endregion

        #region IILInteractiveControl Members
        public ILInputController InputController {
            get { return m_inputController; }
        }
        public int FPS {
            get { return m_driver.FPS; }
        }

        #endregion


    }
}
