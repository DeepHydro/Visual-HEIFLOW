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
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Serialization;

namespace ILNumerics.Drawing {
    /// <summary>
    /// This class specifies the camera's positioning and orientation
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    [System.Diagnostics.DebuggerTypeProxy(typeof(ILGroupVisualizer))]
    [Serializable]
    public class ILCamera : ILGroup {

        /// <summary>
        /// Default camera instance 
        /// </summary>
        public static readonly ILCamera Default = new ILCamera(); 

        #region event handling 
        /// <summary>
        /// Fires a Changed event
        /// </summary>
        protected override void OnPropertyChanged(string propertyName) {
            base.OnPropertyChanged(propertyName); 
            if (IsGlobal && m_globalCamera != null) {
                m_globalCamera.CopyFrom(this,true); 
            }
        }
        #endregion

        #region attributes 
        private Vector3 m_position;
        private Vector3 m_lookat;
        private Vector3 m_top;
        private float m_zNear;
        private float m_zFar; 
        private Projection m_projection;
        private Matrix4? m_positionTransform;
        private Matrix4? m_projectionTransform;
        private ILCamera m_globalCamera;
        private RectangleF m_screenRect;
        private AspectRatioMode m_aspectRatioMode;
        private float m_fieldOfView;
        internal float m_currAspectRatioDriverWindow = 1f; 
        protected System.Drawing.PointF m_mouseStartF; 
        #endregion
        
        #region properties 
        /// <summary>
        /// Screen rectangle identifying the area this object is using inside its container (0..1/0..1, get/set)
        /// </summary>
        [XmlAttribute]
        [ILXmlSerializeAs("{X},{Y},{Width},{Height}")]
        public RectangleF ScreenRect {
            get {
                return m_screenRect;
            }
            set {
                if (value != m_screenRect) {
                    m_screenRect = value;
                    OnPropertyChanged("ScreenRect");
                }
            }
        }

        /// <summary>
        /// Determine if this camera is acting on the global scene. Changes made to one driver will affect all instances of the same scene in all drivers. 
        /// </summary>
        [XmlAttribute]
        public bool IsGlobal { get; set; }
        /// <summary>
        /// Get the type of projection (orthographic/ perspective) or sets it 
        /// </summary> 
        [XmlAttribute]
        public Projection Projection {
            get { return m_projection; }
            set {
                if (value != m_projection) {
                    m_projectionTransform = null; 
                    m_projection = value;
                    OnPropertyChanged("Projection");
                }
            }
        }

        /// <summary>
        /// Near clipping limit (Z axis) 
        /// </summary>
        [XmlAttribute]
        public float ZNear {
            get { return m_zNear; }
            set {
                if (m_zNear != value) {
                    m_projectionTransform = null; 
                    m_zNear = value;
                    OnPropertyChanged("ZNear");
                }
            }
        }
        /// <summary>
        /// Far clipping limit (Z axis)
        /// </summary>
        [XmlAttribute]
        public float ZFar {
            get { return m_zFar; }
            set {
                if (m_zFar != value) {
                    m_projectionTransform = null;
                    m_zFar = value;
                    OnPropertyChanged("ZFar");
                }
            }
        }
        
        /// <summary>
        /// point, the camera is aiming at (world coords)
        /// </summary>
        [XmlAttribute]
        public Vector3 LookAt {
            get { return m_lookat; }
            set {
                if (m_lookat != value) {
                    m_lookat = value;
                    m_positionTransform = null;
                    OnPropertyChanged("LookAt");
                }
            }
        }
        /// <summary>
        /// get/set camera position, absolute cartesian coordinates
        /// </summary>
        /// <remarks>Keep in mind, the angle for phi points towards negative Y axis! The cartesian property 
        /// <paramref name="Position"/> handles the camera position in absolute world coordinates, while the 
        /// polar coordinates (Rho, Phi, Distance) supress the camera position by means of coordinates 
        /// relative to the LookAt point (i.e. usually the center of the viewing cube)!</remarks>
        [XmlAttribute]
        public Vector3 Position {
            get {
                return m_position;
            }
            set {
                if (m_position != value) {
                    m_position = value;
                    m_positionTransform = null;
                    OnPropertyChanged("Position");
                }
            }
        }

        /// <summary>
        /// orientation of the camera, normalized, readonly
        /// </summary>
        /// <remarks>This vector is readonly always points 'upwards'.</remarks>
        [XmlAttribute]
        public Vector3 Top {
            get {
                return m_top;
            }
            set {
                if (m_top != value) {
                    m_top = value;
                    m_positionTransform = null;
                    OnPropertyChanged("Top");
                }
            }
        }

        /// <summary>
        /// debugger helper: display phi in degrees (readonly)
        /// </summary>
        [XmlIgnore]
        private int m_phiDebugDisp {
            get {
                Vector3 tmp = Polar;
                return (int)Math.Round(tmp.X * 180 / Math.PI); 
            }
        }
        /// <summary>
        /// debugger helper: display rho in degrees
        /// </summary>
        [XmlIgnore]
        private int m_rhoDebugDisp {
            get {
                Vector3 tmp = Polar;
                return (int)Math.Round(tmp.Y * 180 / Math.PI); 
            }
        }

        /// <summary>
        /// spherical coordinates relative to the look at point
        /// </summary>
        [XmlIgnore]
        public Vector3 Polar {
            get {
                return (Position-LookAt).ToPolar(); 
            }
        }
        /// <summary>
        /// true, when looking from top on the un-rotated scene (common for 2D plots)
        /// </summary>
        [XmlIgnore]
        public bool Is2DView {
            get {
                Vector3 p = Polar; 
                return p.X < 1e-5 && p.Y < 1e-5; 
            }
        }

        /// <summary>
        /// Determines, if objects keep their shape, regardless from the windows aspect ratio. Default: keep shape
        /// </summary>
        [XmlAttribute]
        public AspectRatioMode AspectRatioMode {
            get {
                return m_aspectRatioMode;
            }
            set {
                if (m_aspectRatioMode != value) {
                    m_aspectRatioMode = value; 
                    OnPropertyChanged("AspectRatioMode");
                }
            }
        }

        /// <summary>
        /// Determines the field of view for perspective projection. Small: more fish eye effect, large: more overview. Default: 10
        /// </summary>
        [XmlAttribute]
        public float FieldOfView {
            get {
                return m_fieldOfView;
            }
            set {
                if (m_fieldOfView != value) {
                    m_fieldOfView = value;
                    OnPropertyChanged("FieldOfView");
                }
            }
        }

        /// <summary>
        /// Get position transform matrix (readonly)
        /// </summary>
        [XmlIgnore]
        public Matrix4 PositionTransform {
            get {
                if (!m_positionTransform.HasValue) {
                    m_positionTransform = Matrix4.LookAtTransformation(Position, LookAt, Top);
                }
                return m_positionTransform.Value; 
            }
        }
        /// <summary>
        /// Get projection transform matrix (readonly)
        /// </summary>
        internal Matrix4 ProjectionTransform {
            get {
                if (!m_projectionTransform.HasValue) {
                    if (Projection == Drawing.Projection.Orthographic) {
                        m_projectionTransform = Matrix4.OrthographicTransform(-1, 1, 1, -1, ZNear, ZFar);
                    } else {
                        if (m_aspectRatioMode == Drawing.AspectRatioMode.MaintainRatios) {
                            m_projectionTransform = Matrix4.PerspectiveTransform(m_fieldOfView, ZNear, ZFar, m_currAspectRatioDriverWindow);
                        } else {
                            m_projectionTransform = Matrix4.PerspectiveTransform(m_fieldOfView, ZNear, ZFar);
                        }
                    }
                }
                return m_projectionTransform.Value; 
            }
        }
        internal Matrix4 ViewTransform {
            get {
                //return Matrix4.FromViewRectangleF(ScreenRect.Left, ScreenRect.Right, ScreenRect.Top, ScreenRect.Bottom);
                // (3) and go back to clip coords
                Matrix4 to =  Matrix4.Translation(-1, 1, 0) * Matrix4.ScaleTransform(2, -2, 1) *
                // (2) apply rectangle 
                Matrix4.Translation(ScreenRect.Left, ScreenRect.Top, 0) *
                Matrix4.ScaleTransform(ScreenRect.Width, ScreenRect.Height, 1) *
                // (1) transform to 0..1/0..1 coords
                Matrix4.ScaleTransform(0.5f, -0.5f, 1) *
                Matrix4.Translation(1, -1, 0);
                return to; 
            }
        }
        #endregion 

        #region constructors
        /// <summary>
        /// Create new camera with settings from existing camera
        /// </summary>
        /// <param name="source">source camera</param>
        public ILCamera (ILCamera source) : base(source) {
            m_position = source.m_position; 
            m_lookat = source.m_lookat; 
            m_top = source.m_top;
            m_zNear = source.m_zNear;
            m_zFar = source.m_zFar; 
            m_projection = source.m_projection;
            m_screenRect = source.ScreenRect;
            m_aspectRatioMode = source.m_aspectRatioMode; 
            m_fieldOfView = source.m_fieldOfView; 
        }
        /// <summary>
        /// Creates a new camera
        /// </summary>
        public ILCamera (object tag = null) : base (tag) {
            m_lookat = new Vector3(0,0,0); 
            m_position = new Vector3(0,0,10);
            m_top = new Vector3(0,1,0);
            m_zNear = 0.1f;
            m_zFar = 100;
            m_projection = Projection.Perspective;
            m_screenRect = new RectangleF(0, 0, 1, 1);
            m_aspectRatioMode = Drawing.AspectRatioMode.MaintainRatios; 
            m_fieldOfView = 10; 
            //m_viewTransform = Matrix4.Identity;
        }
        #endregion

        #region public interface 
        /// <summary>
        /// Convert all camera parameter to string
        /// </summary>
        /// <returns>string display: polar coordinates, position, lookat points and top vector</returns>
        public override string ToString() {
            if (this.GetType().Name.StartsWith("ILCamera")) {
                return String.Format("Camera: #{6} - Polar r:{0} φ:{1}° ρ:{2}° - Pos {3} - Lookat {4} - Top {5}",
                    (LookAt - Position).Length, m_phiDebugDisp, m_rhoDebugDisp, Position, LookAt, Top, ID);
            } else {
                return base.ToString(); 
            }
        }
        internal ILCamera Clone() {
            return (ILCamera)MemberwiseClone(); 
        }
        #endregion

        #region transformations
        public new void Rotate(Quaternion offset) {
            Matrix4 rot = Matrix4.Invert(Matrix4.Rotation(offset));
            Position = LookAt + rot * (Position - LookAt); 
            Top = Vector3.Normalize(rot * Top);
            OnPropertyChanged("Position");
        }
        public void RotateX(double offset) {
            Vector3 xAxis = Vector3.CrossN(m_position - m_lookat, m_top);
            Matrix4 rot = Matrix4.Rotation(xAxis, offset);
            m_position = (m_position - m_lookat) * rot + m_lookat;
            Top = m_top * rot;
            OnPropertyChanged("Position");
        }
        public void RotateY(double offset) {
            Matrix4 rot = Matrix4.Rotation(Top, offset);
            Position = (Position - LookAt) * rot + LookAt;
            OnPropertyChanged("Position");
        }
        public void RotateZ(double offset) {
            Matrix4 rot = Matrix4.Rotation(Position - LookAt, offset);
            Top = Top * rot;
            OnPropertyChanged("Position");
        }
        public void Move(Vector3 startClip, Vector3 endClip) {
            Matrix4 projInv = Matrix4.Invert(ProjectionTransform * PositionTransform);
            Vector3 startWorld = projInv * startClip;
            Vector3 endWorld = projInv * endClip; 
            Vector3 distOffset =  startWorld - endWorld; 
            Vector3 xDir = Vector3.CrossN(Position - LookAt, Top); 
            Vector3 yDir = Top;
            float xLen = Vector3.Dot(xDir, distOffset);
            float yLen = Vector3.Dot(Top, distOffset);
            Vector3 move = xDir * xLen + yDir * yLen;
            Position += move;
            LookAt += move;
            OnPropertyChanged("Position");
        }
        #endregion

        #region Node overrides 
        protected override bool BeginVisit(ILRenderParameter parameter) {
            if (parameter.PickingContext != null) {
                // compute absolute screen rect spanned by this camera
                
                Size driverSize = parameter.Driver.Size; 
                RectangleF rect = new RectangleF(ScreenRect.X * driverSize.Width, ScreenRect.Y * driverSize.Height,
                                               ScreenRect.Width * driverSize.Width, ScreenRect.Height * driverSize.Height); 
                if (!rect.Contains(parameter.PickingContext.Location.X, parameter.PickingContext.Location.Y)) {
                    return false;
                } else {
                    if (parameter.PickingContext.CurrentScreenRectTarget != null) {
                        // we got a conflict! -> choose the one which is 
                        // * smaller (reg. ScreenRect area) or
                        // * older (higher ID)
                        var oldarea = parameter.PickingContext.CurrentScreenRectTarget.ScreenRect.Width * 
                                      parameter.PickingContext.CurrentScreenRectTarget.ScreenRect.Height; 
                        var myarea = ScreenRect.Width * ScreenRect.Height; 
                        if (myarea < oldarea) {
                            parameter.PickingContext.CurrentScreenRectTarget = this; 
                        } else if (ID > parameter.PickingContext.CurrentScreenRectTarget.ID) {
                            parameter.PickingContext.CurrentScreenRectTarget = this; 
                        }
                    } else {
                        parameter.PickingContext.CurrentScreenRectTarget = this; 
                    }
                }
            }
            //System.Diagnostics.Debug.WriteLine("BeginVisit Camera Node "); 
            parameter.PushNew(PositionTransform);
            parameter.CameraPositionTransforms.Push(PositionTransform); 
            parameter.ViewTransforms.Push(parameter.ViewTransform * ViewTransform); 
            if (parameter.CurrentPassCount == 0) {
                RectangleF viewport = parameter.ViewTransform.ToViewRectangle(); 
                m_currAspectRatioDriverWindow = viewport.Width / viewport.Height; 
                m_projectionTransform = null; 
            }
            parameter.PushProjectionTransform(ProjectionTransform);
            return base.BeginVisit(parameter); 
        }
        protected override void EndVisit(ILRenderParameter parameter) {
            base.EndVisit(parameter);
            parameter.Pop(); 
            parameter.PopProjectionTransform();
            parameter.ViewTransforms.Pop(); 
            parameter.CameraPositionTransforms.Pop(); 
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILCamera();
        }
        internal override ILNode Copy() {
            ILCamera ret = new ILCamera(this);
            return ret;
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILCamera ret = (ILCamera)base.Synchronize(copy, syncParams);
            if (copy == null || ret.Version < Version) {
                // first time setup or if user changed camera explicitely
                ret.m_globalCamera = this; 
                ret.IsGlobal = IsGlobal; 
                ret.m_lookat = m_lookat; 
                ret.m_position = m_position; 
                ret.m_projection = m_projection; 
                ret.m_top = m_top; 
                ret.m_screenRect = m_screenRect; 
                ret.m_zFar = m_zFar; 
                ret.m_zNear = m_zNear; 
                ret.m_positionTransform = m_positionTransform; 
                ret.m_projectionTransform = m_projectionTransform; 
                ret.m_aspectRatioMode = m_aspectRatioMode; 
                ret.m_fieldOfView = m_fieldOfView; 
            }
            return ret; 
        }
        internal override void TranslateEventLocation(bool capture, ILMouseEventArgs e) {
            if (capture) {
                e.LocationF = new PointF(
                    (e.LocationF.X - ScreenRect.X) / ScreenRect.Width, 
                    (e.LocationF.Y - ScreenRect.Y) / ScreenRect.Height); 
            } else {
                e.LocationF = new PointF(
                    e.LocationF.X * ScreenRect.Width + ScreenRect.X,
                    e.LocationF.Y * ScreenRect.Height + ScreenRect.Y);
            }
        }
        #region IILInputHandler Members
        protected internal override void OnMouseDoubleClick(ILMouseEventArgs e) {
            RaiseMouseDoubleClick(e);
            if (e.Cancel) return;
            Reset();
            e.Refresh = true; 
        }
        protected internal override void OnMouseDown(ILMouseEventArgs e) {
            RaiseMouseDown(e);
            if (e.Cancel) return;
            m_mouseStartF = e.LocationF;
        }
        protected internal override void OnMouseMove(ILMouseEventArgs e) {
            RaiseMouseMove(e); 
            if (e.Cancel) return; 
            if (!e.DirectionUp) return; 

            if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                //System.Diagnostics.Debug.WriteLine("In Camera MouseMove" + Environment.TickCount);
                float diffX = e.LocationF.X - m_mouseStartF.X;
                float diffY = e.LocationF.Y - m_mouseStartF.Y;
                // rotate
                bool useX, useY;
                ILInputController.checkFilterDirection(e, out useX, out useY, m_mouseStartF);
                m_mouseStartF = e.LocationF;
                float scale = e.ShiftPressed ? 0.1f : 1f;
                if (e.ControlPressed) {
                    RotateZ(diffX * (float)Math.PI * 2f * scale);
                } else {
                    if (useY)
                        RotateX(diffY * (float)Math.PI * 2f * scale);
                    if (useX)
                        RotateY(diffX * (float)Math.PI * -2f * scale);
                }
                e.Refresh = true; 
            } else if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                // move LA
                bool useX;
                bool useY;
                ILInputController.checkFilterDirection(e, out useX, out useY, m_mouseStartF);
                float scale = e.ShiftPressed ? 0.3f : 1f;
                Vector3 startClip = new Vector3(
                    m_mouseStartF.X * 2f - 1,
                    (1 - m_mouseStartF.Y) * 2f - 1,
                    0);
                Vector3 endClip = new Vector3(
                    useX ? e.LocationF.X * 2f - 1 : startClip.X,
                    useY ? (1 - e.LocationF.Y) * 2f - 1 : startClip.Y,
                    0);
                endClip = startClip + (endClip - startClip) * scale;
                Move(startClip, endClip);
                m_mouseStartF = e.LocationF;
                e.Refresh = true; 
            }
        }

        protected internal override void OnMouseWheel(ILMouseEventArgs e) {
            RaiseMouseWheel(e);
            if (e.Cancel) return; 
            float scale = (e.ShiftPressed) ? 0.01f : 0.1f;
            Position = Position + (Position - LookAt) * Math.Sign(e.Delta) * scale;
            e.Refresh = true; 
        }

        #endregion

        
        #endregion

        public virtual void Reset() {
            CopyFrom(Default, false); 
        }

        public void CopyFrom(ILCamera source, bool setProjection) {
            m_position = source.m_position;
            m_lookat = source.m_lookat;
            m_top = source.m_top;
            //ScreenRect = source.ScreenRect; 
            m_positionTransform = null;
            if (setProjection) {
                ZNear = source.ZNear;
                ZFar = source.ZFar;
                Projection = source.Projection;
                m_projectionTransform = null; 
                m_aspectRatioMode = source.m_aspectRatioMode; 
                m_fieldOfView = source.m_fieldOfView; 
            }
            OnPropertyChanged("Position");
        }
    }
}
