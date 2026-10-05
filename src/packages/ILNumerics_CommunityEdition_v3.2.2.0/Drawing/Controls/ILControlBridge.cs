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
using System.Windows.Forms; 
using System.Drawing; 
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ILNumerics.Drawing {
    public partial class ILControlBridge : Component, IILDriver {
        
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
        private Control m_control;  
        private ILDriver m_driver; 
        private ILBackBuffer m_backBuffer;
        private ILInputController m_inputController; 
        private ILClock m_timer;
        #endregion 

        #region constructors 
        public ILControlBridge() {
            InitializeComponent();
            m_timer = new ILClock(); 
        }

        public ILControlBridge(IContainer container) {
            container.Add(this);
            InitializeComponent();
        }
        #endregion

        #region properties
        public Control Control {
            get { return m_control; }
            set {
                if (object.ReferenceEquals(m_control, value)) 
                    return; 
                setupControl(value); 
            }
        }
        #endregion

        #region private helpers
        private void setupControl(Control value) {
            if (value == null) return;
            if (m_control != null) {
                m_control.Paint -= m_controlPaint;
                m_control.SizeChanged -= (s, e) => { m_driver.Size = m_control.Size; };
            }
            if (m_driver is IDisposable && m_driver != null) {
                m_driver.FPSChanged -= (s, a) => { OnFPSChanged(); };
                m_driver.BeginRenderFrame -= (s, a) => { OnBeginRenderFrame(a.Parameter); };
                m_driver.EndRenderFrame -= (s, a) => { OnEndRenderFrame(a.Parameter); };
               (m_driver as IDisposable).Dispose(); 
            }
            if (m_backBuffer != null) {
                m_backBuffer.Dispose(); 
            }
            m_backBuffer = new ILBackBuffer(); 
            m_driver = new ILGDIDriver(m_backBuffer); 
            m_control = value; 
            m_control.SizeChanged += (s, e) => { m_driver.Size = m_control.Size; };
            m_control.Paint += m_controlPaint;
            m_driver.Size = m_control.Size;
            m_driver.FPSChanged += (s, a) => { OnFPSChanged(); };
            m_driver.BeginRenderFrame += (s, a) => { OnBeginRenderFrame(a.Parameter); };
            m_driver.EndRenderFrame += (s, a) => { OnEndRenderFrame(a.Parameter); };
            m_inputController = new ILInputController(m_driver); 
        }

        protected void m_controlPaint(object sender, PaintEventArgs args) {
            m_driver.Render(m_timer.TimeMilliseconds); 
            args.Graphics.DrawImage(m_backBuffer.Bitmap, 0, 0 );
        }
        #endregion 

        #region IILDriver Members
        public int FPS {
            get { return m_driver.FPS; }
        }

        public ILCamera Camera {
            get {
                return m_driver.Camera;
            }
        }

        public System.Drawing.Color BackColor {
            get {
                return m_driver.BackColor;
            }
            set {
                m_driver.BackColor = value;
            }
        }

        public void Configure() {
            m_driver.Configure();
        }

        public void Render(long timeMs) {
            m_driver.Render(timeMs);
        }

        public RendererTypes Driver {
            get { return m_driver.Driver; }
        }

        public ILScene Scene {
            get {
                return m_driver.Scene;
            }
            set {
                m_driver.Scene = value;
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

        public System.Drawing.Size Size {
            get {
                return m_driver.Size;
            }
            set {
                m_driver.Size = value;
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
            get { return m_driver.ViewTransform; }
        }

        #endregion

        #region IILDriver Members
        public ILScene GetCurrentScene(long ms = 0) {
            return m_driver.GetCurrentScene(ms);
        }

        public int? PickAt(System.Drawing.Point screenCoords, long timeMs) {
            return m_driver.PickAt(screenCoords, m_timer.TimeMilliseconds); 
        }

        public ILScene LocalScene {
            get { return m_driver.LocalScene;  }
        }

        #endregion

        #region IILInteractiveControl Members

        public ILInputController InputController {
            get { return m_inputController; }
        }
        public ILClock Clock { get { return m_timer; } }
        #endregion

    }
}
