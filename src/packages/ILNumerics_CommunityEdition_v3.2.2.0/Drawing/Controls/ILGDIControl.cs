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

namespace ILNumerics.Drawing {
    
    [System.Security.SecuritySafeCritical]
    public partial class ILGDIControl : Control, IILDriver {

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
        protected ILClock m_timer; 
        private ILGDIDriver m_driver;
        private ILInputController m_inputController;
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
                return Renderer.BackColor;
            }
            set {
                Renderer.BackColor = value;
            }
        }
        #endregion

        #region constructors
        public ILGDIControl() {
            InitializeComponent();
            m_timer = new ILClock() { Running = false }; 
            ILBackBuffer bbuffer = new ILBackBuffer(); 
            bbuffer.Rectangle = ClientRectangle; 
            m_driver = new ILGDIDriver(bbuffer); 
            m_driver.FPSChanged += (s,a) => { OnFPSChanged(); };
            m_driver.BeginRenderFrame += (s, a) => { OnBeginRenderFrame(a.Parameter); };
            m_driver.EndRenderFrame += (s, a) => { OnEndRenderFrame(a.Parameter); };
            m_inputController = new ILInputController(this); 
            DoubleBuffered = true;
            BackColor = Color.SteelBlue; 
        }
        #endregion

        #region private helpers
        void Application_Idle(object sender, EventArgs e) {
            if (m_timer.Running) 
                Invalidate();
        }
        internal static void DrawBranding(PaintEventArgs e, Color backColor, Rectangle clientRectangle, string text) {
            using (Brush brush = new SolidBrush(backColor)) {
                e.Graphics.FillRectangle(brush, clientRectangle);
            }
            using (var pen = new Pen(System.Drawing.Color.LightSkyBlue, 3)) {
                e.Graphics.DrawEllipse(pen, clientRectangle);
            }
            using (Brush brush = new SolidBrush(Color.LightSkyBlue)) {
                //Font font = new System.Drawing.Font(DefaultFont.FontFamily.Name,1.0f, DefaultFont.Style);
                SizeF size = e.Graphics.MeasureString(text, (Font)DefaultFont.Clone());
                PointF point = new PointF((clientRectangle.Width - size.Width) / 2f, (clientRectangle.Height - size.Height) / 2f);
                e.Graphics.DrawString(text, (Font)DefaultFont.Clone(), brush, point);
            }
        }
        private bool isDesignMode() {
            return ILHelper.IsDesignMode();
        }
        #endregion

        #region IILInteractiveControl Members
        public int FPS {
            get { return m_driver.FPS; }
        }

        public ILCamera Camera {
            get {
                return m_driver.Camera; 
            }
        }
        public void Render(long timeMs) {
            Invalidate(); 
            //m_driver.Render(); 
        }
        public void Configure() {
            m_driver.Configure(); 
        }
        public ILScene Scene {
            get { return m_driver.Scene; }
            set { m_driver.Scene = value; }
        }
        public ILScene LocalScene {
            get { return m_driver.LocalScene; }
            set { m_driver.LocalScene = value; }
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

        public RectangleF Rectangle {
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
        public RendererTypes Driver {
            get { return RendererTypes.GDI; }
        }
        #endregion

        #region IILControl Members
        /// <summary>
        /// Return a scene which reflects the rendering result, including all local compositions and modifications
        /// </summary>
        /// <param name="ms">[optional] time of render frame</param>
        /// <returns>scene composition as copies of local and global scene with user interaction</returns>
        public ILScene GetCurrentScene(long ms = 0) {
            return m_driver.GetCurrentScene();
        }

        public int? PickAt(System.Drawing.Point screenCoords, long timeMs) {
            return m_driver.PickAt(screenCoords, timeMs); 
        }
        public ILDriver Renderer {
            get { return m_driver; }
        }
        #endregion

        #region IILInteractiveControl Members

        public ILInputController InputController {
            get { return m_inputController; }
        }

        #endregion

        #region eventing overrides
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            if (!isDesignMode())
                Application.Idle += Application_Idle;
        }
        protected override void OnSizeChanged(EventArgs e) {
            if (Size.Width > 0 && Size.Height > 0) {
                m_driver.Size = Size;
                Invalidate();
            }
            base.OnSizeChanged(e);
        }
        protected override void OnHandleDestroyed(EventArgs e) {
            base.OnHandleDestroyed(e);
            m_timer.Running = false;
            Application.Idle -= Application_Idle;
        }
        protected override void OnPaint(PaintEventArgs e) {
            if (isDesignMode()) {
                DrawBranding(e, BackColor, ClientRectangle, "ILNumerics ILPanel (GDI)");
            } else {
                m_driver.Render(m_timer.TimeMilliseconds); 
                // blit the backbuffer to my graphics 
                e.Graphics.DrawImage(m_driver.BackBuffer.Bitmap, m_driver.BackBuffer.Rectangle); 
            }
            base.OnPaint(e); 
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
        #endregion

    }
}
