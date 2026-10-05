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
using System.Xml.Serialization; 

namespace ILNumerics.Drawing.Plotting {
    [Serializable]
    public class ILPlotCube : ILCamera {

        #region attributes
        /// <summary>
        /// Default tag for the plot box group node itself
        /// </summary>
        public static string DefaultTag = "PlotCube";
        /// <summary>
        /// Default tag for the camera node
        /// </summary>
        public static string CameraDefaultTag = "PlotCubeCamera";
        private System.Drawing.Point m_mouseStartScreen;
        /// <summary>
        /// Get the mouse position distance threshold in pixels before a drag operation is triggered
        /// </summary>
        public static int MinZoomDragLimit = 2;
        private bool m_allowPan;
        private bool m_allowRotation;
        private bool m_allowZoom;
        private bool m_2dMode;
        #endregion

        #region properties
        /// <summary>
        /// Determines, if this plot cube should be optimized for 2D content (true, default) or for 3D content (false)
        /// </summary>
        [XmlAttribute]
        public bool TwoDMode {
            get {
                return m_2dMode;
            }
            set {
                if (m_2dMode != value) {
                    m_2dMode = value;
                    OnPropertyChanged("TwoDMode");
                }
            }
        }
        /// <summary>
        /// Determines if users are allowed to move the scene interactively
        /// </summary>
        [XmlAttribute]
        public bool AllowPan {
            get {
                return m_allowPan;
            }
            set {
                if (m_allowPan != value) {
                    m_allowPan = value;
                    OnPropertyChanged("AllowPan");
                }
            }
        }
        /// <summary>
        /// Determines if users are allowed to rotate the scene interactively
        /// </summary>
        [XmlAttribute]
        public bool AllowRotation {
            get {
                return m_allowRotation;
            }
            set {
                if (m_allowRotation != value) {
                    m_allowRotation = value;
                    OnPropertyChanged("AllowRotation");
                }
            }
        }
        /// <summary>
        /// Determines if users are allowed to zoom the scene interactively
        /// </summary>
        [XmlAttribute]
        public bool AllowZoom {
            get {
                return m_allowZoom;
            }
            set {
                if (m_allowZoom != value) {
                    m_allowZoom = value;
                    OnPropertyChanged("AllowZoom");
                }
            }
        }

        /// <summary>
        /// Rotation matrix used to rotate the plot cube
        /// </summary>
        [XmlIgnore]
        public Matrix4 Rotation {
            get { return ScaleGroup.Rotation; }
            set { ScaleGroup.Rotation = value; }
        }
        /// <summary>
        /// Access to the lines for the zoom rectangle in interactive 2D mode
        /// </summary>
        [XmlIgnore]
        public ILSelectionRectangle ZoomRectangle {
            get { return First<ILSelectionRectangle>(); }
        }

        /// <summary>
        /// helper node holding box lines, axes and plots data group, scales the plot cube screen rect area
        /// </summary>
        [XmlIgnore]
        internal ILPlotCubeScaleGroup ScaleGroup {
            get { return First<ILPlotCubeScaleGroup>(); }
        }
        #endregion

        #region delegated properties
        /// <summary>
        /// Automatically expand the plotting view limits when a larger new plot is added 
        /// </summary>
        [XmlIgnore]
        public bool AutoScaleOnAdd {
            get { return Plots.AutoScaleOnAdd; }
            set { Plots.AutoScaleOnAdd = value; }
        }
        /// <summary>
        /// Gets access to the axes collection
        /// </summary>
        [XmlIgnore]
        public ILAxisCollection Axes { get { return ScaleGroup.Axes; } }
        /// <summary>
        /// Gets the scale modes collection, used to determine linear / logarithmic scales for eache axis
        /// </summary>
        [XmlIgnore]
        public ILScaleModes ScaleModes { get { return Plots.ScaleModes; } }
        /// <summary>
        /// gets / sets the limits for the main plot cube axes 
        /// </summary>
        [XmlIgnore]
        public ILLimits Limits { get { return Plots.Limits; } }
        /// <summary>
        /// Allows access to the cube main lines for arbitrary configuration
        /// </summary>
        [XmlIgnore]
        public ILLines Lines { get { return ScaleGroup.Lines; } }
        /// <summary>
        /// Gets the collection of the first (default) plot cube data group, hosting the plot objects 
        /// </summary>
        [XmlIgnore]
        public ILPlotCubeDataGroup Plots { get { return ScaleGroup.Plots; } }
        /// <summary>
        /// Gets the screen recangle of the data cube containing the plots or sets it, default: automatic. range [0..1] of the container.
        /// </summary>
        /// <remarks>The data screen rectangle marks the rectangular area on the rendering surface, where the plots are contained. 
        /// This area depends on the rotation of the plots and does only take the extend of the (rotated and projected) plots within the
        /// plot cube into account. The space used by labels and ticks are not considered here.
        /// <para>Per default, the ILNumerics plot cube does automatically determine a good setting in order to make all plot cube 
        /// elements (ticks &amp; labels) visible. However, for the purpose of aligning a plot cube with another plot cube, it might be helpful to choose the 
        /// extend manually. </para>
        /// <para>Note, the rectangle is given in relative coordinates of X and Y within the plot cube screen rect area, as determined by 
        /// <see cref="ILCamera.ScreenRect"/>. (0,0) is the upper left corner of the rectangle within the one defined by ScreenRect. </para>
        /// </remarks>
        [ILXmlSerializeAs("{X},{Y},{Width},{Height}")]
        public RectangleF DataScreenRect { 
            get { return ScaleGroup.DataScreenRect; }
            set { ScaleGroup.DataScreenRect = value; }
        }
        /// <summary>
        /// The collection of plots contained in this plot cubes first data group
        /// </summary>
        [XmlIgnore]
        public override ILNodeCollection Children {
            get {
                return Plots.Children;
            }
        }
        /// <summary>
        /// The collection of plots contained in this plot cubes first data group
        /// </summary>
        [Obsolete("Use 'Children' instead!")]
        public override ILNodeCollection Childs {
            get {
                return Plots.Children;
            }
        }
        #endregion

        #region constructors
        protected ILPlotCube(ILPlotCube source) : base(source) {
            this.m_2dMode = source.TwoDMode;
            this.m_allowPan = source.m_allowPan;
            this.m_allowRotation = source.m_allowRotation; 
            this.m_allowZoom = source.m_allowZoom; 
            this.ScaleGroup.PropertyChanged += (s, arg) => {
                OnPropertyChanged(arg.PropertyName);
            };
        }
        public ILPlotCube(object tag = null, bool twoDMode = true) : base(tag ?? DefaultTag) {
            Projection = Projection.Orthographic;
            m_children.Add(new ILPlotCubeScaleGroup(ILPlotCubeScaleGroup.DefaultTag));
            m_children.Add(new ILSelectionRectangle(ILSelectionRectangle.GroupTag)); // zoom rectangle
            TwoDMode = twoDMode;  
            AllowPan = true; 
            AllowRotation = true; 
            AllowZoom = true;
            ScaleGroup.PropertyChanged += (s, arg) => {
                OnPropertyChanged(arg.PropertyName);
            };
        }
        private ILPlotCube() { }
        #endregion

        #region public interface
        /// <summary>
        /// Add an object to the first plots data container of this cube
        /// </summary>
        /// <typeparam name="T">plot object type</typeparam>
        /// <param name="node">new plot object</param>
        /// <param name="tag">[optional] tag identifying the new plot object in the scene</param>
        /// <returns>The new object added to the scene</returns>
        /// <remarks>This override redirects Add requests to the first plot data container object within the plot cube.</remarks>
        public override T Add<T>(T node, object tag = null, bool shareBuffers = true) {
            return Plots.Add<T>(node, tag);
        }
        /// <summary>
        /// Add a new data group to the data groups of the plot cube
        /// </summary>
        /// <param name="tag">[optional] tag used to identify the data group within the scene graph</param>
        /// <returns>the newly created plot cube data group</returns>
        public ILPlotCubeDataGroup AddDataGroup(object tag = null) {
            return ScaleGroup.Add(new ILPlotCubeDataGroup(tag)); 
        }

        #endregion

        #region synced node implementation
        internal override ILNode Copy() {
            return new ILPlotCube(this);
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILPlotCube();
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILPlotCube ret = (ILPlotCube)base.Synchronize(copy, syncParams);
            if (copy == null) {
                ret.ScaleGroup.PropertyChanged += (s, arg) => {
                    ret.OnPropertyChanged(arg.PropertyName);
                };
            }
            if (copy == null || SynchedVersion != copy.Version) {
                ret.TwoDMode = TwoDMode; 
                ret.AllowZoom = AllowZoom; 
                ret.AllowRotation = AllowRotation; 
                ret.AllowPan = AllowPan; 
                ret.Projection = Projection; 
            }
            return ret; 
        }
        #endregion

        #region IILInputHandler Members

        protected internal override void OnMouseDoubleClick(ILMouseEventArgs args) {
            RaiseMouseDoubleClick(args);
            if (args.Cancel) return;
            if (!args.DirectionUp) return; 
            Reset();
            args.Refresh = true;
        }
        public override void Reset() {
            base.Reset();
            if (ScaleGroup != null) {
                foreach (var dataGroup in ScaleGroup.Find<ILPlotCubeDataGroup>()) {
                    dataGroup.Reset();
                }
                ScaleGroup.Rotation = Matrix4.Identity;
            }
        }
        protected internal override void OnMouseDown(ILMouseEventArgs args) {
            RaiseMouseDown(args); 
            if (args.Cancel) return; 
            if (!args.DirectionUp) return; 
            m_mouseStartF = args.LocationF;
            m_mouseStartScreen = args.Location;
        }

        protected internal override void OnMouseUp(ILMouseEventArgs args) {
            RaiseMouseUp(args);
            if (args.Cancel) return;
            if (!args.DirectionUp) return; 
            if (m_2dMode && args.Button == System.Windows.Forms.MouseButtons.Left && AllowZoom) {
                #region execute zoom 2d mode
                ZoomRectangle.Visible = false;
                // check for minimum drag distance of 2 px
                if (Math.Max(Math.Abs(args.X - m_mouseStartScreen.X), Math.Abs(args.Y - m_mouseStartScreen.Y)) > MinZoomDragLimit) {
                    Vector4 startClip = new Vector4(
                            m_mouseStartF.X * 2 - 1,
                            m_mouseStartF.Y * -2 + 1, 0, 1);
                    Vector4 nowClip = new Vector4(
                            args.LocationF.X * 2 - 1,
                            args.LocationF.Y * -2 + 1, 0, 1);
                    // transform to data coord and execute zoom
                    var plots = Find<ILPlotCubeDataGroup>();
                    foreach (ILPlotCubeDataGroup plotGroup in plots) {
                        Matrix4 transf = plotGroup.Transform;
                        ILGroup cur = plotGroup;
                        while (cur.Parent != null && cur.Parent != this) {
                            cur = cur.Parent;
                            transf = cur.Transform * transf;
                        }
                        transf = ProjectionTransform * transf;
                        transf = Matrix4.Invert(transf);
                        Vector4 startData = transf * startClip;
                        Vector4 nowData = transf * nowClip;
                        if (startData.W != 0) {
                            startData /= startData.W;
                        }
                        startData.Z = plotGroup.Limits.ZMin;
                        if (nowData.W != 0) {
                            nowData /= nowData.W;
                        }
                        nowData.Z = plotGroup.Limits.ZMax;
                        plotGroup.Limits.Set(startData.Xyz, nowData.Xyz);
                    }
                    args.Refresh = true; 
                }
                #endregion
            }
        }

        protected internal override void OnMouseMove(ILMouseEventArgs args) {
            RaiseMouseMove(args); 
            if (args.Cancel) return; 
            if (!args.DirectionUp) return; 

            if (args.Button == System.Windows.Forms.MouseButtons.Left && ((AllowZoom && TwoDMode) || (AllowRotation && !TwoDMode))) {
                #region rotating / zoom 2d
                if (m_2dMode) {
                    // coords of the selection rectangle are stored in rel. viewport coords and later 
                    // transformed to clip coords (in ILSelectionRectangle.BeginVisit)
                    if (args.LocationF.X < 0) args.LocationF = new PointF(0, args.LocationF.Y); 
                    if (args.LocationF.Y < 0) args.LocationF = new PointF(args.LocationF.X, 0);
                    if (args.LocationF.X > 1) args.LocationF = new PointF(1, args.LocationF.Y);
                    if (args.LocationF.Y > 1) args.LocationF = new PointF(args.LocationF.X, 1);
                    ZoomRectangle.SetSize(m_mouseStartF, args.LocationF);
                    ZoomRectangle.Visible = true;
                } else {
                    // rotate
                    bool useX, useY;
                    ILInputController.checkFilterDirection(args, out useX, out useY, m_mouseStartF);
                    float scale = args.ShiftPressed ? 0.1f : 1f;
                    if (args.ControlPressed) {
                        ScaleGroup.Rotation = Matrix4.Rotation(Vector3.UnitZ, (args.LocationF.X - m_mouseStartF.X) * (float)Math.PI * 2f * scale) * ScaleGroup.Rotation;
                    } else {
                        if (useY)
                            ScaleGroup.Rotation = Matrix4.Rotation(Vector3.UnitX, (m_mouseStartF.Y - args.LocationF.Y) * 1f * (float)Math.PI * 2f * scale) * ScaleGroup.Rotation;
                        if (useX)
                            ScaleGroup.Rotation = Matrix4.Rotation(Vector3.UnitY, (m_mouseStartF.X - args.LocationF.X) * 1f * (float)Math.PI * 2f * scale) * ScaleGroup.Rotation;
                    }
                    m_mouseStartF = args.LocationF;
                }
                args.Cancel = true;
                args.Refresh = true;
                #endregion
            } else if (args.Button == System.Windows.Forms.MouseButtons.Right && AllowPan) {
                #region move LA
                bool useX;
                bool useY;
                checkFilterDirection(args, out useX, out useY, m_mouseStartF);
                float scale;
                if (Projection == Drawing.Projection.Perspective) {
                    scale = args.ShiftPressed ? 0.2f : 3f;
                } else {
                    scale = args.ShiftPressed ? 0.3f : 1f;
                }
                Vector4 startClip = new Vector4(
                    m_mouseStartF.X * 2 - 1,
                    m_mouseStartF.Y * -2 + 1,
                    ZNear, 1); //(m_plotCube.Camera.ZFar + m_plotCube.Camera.ZNear) / 2, 1);
                Vector4 endClip = new Vector4(
                    useX ? args.LocationF.X * 2 - 1 : startClip.X,
                    useY ? args.LocationF.Y * -2 + 1 : startClip.Y,
                    ZNear, 1); //(m_plotCube.Camera.ZFar + m_plotCube.Camera.ZNear) / 2, 1);
                // we must transform the local (relative, ie. in range 0..1) pixel coords in clip coords. 
                // This is usually done in the FIRST viewtransform on the render parameter stack. 
                // but here, we handle a local camera coord system, so we do not know / do not need 
                // the true extent of the render surface. hence, we change the screen coords to clip 
                // manually. 
                Matrix4 t = ProjectionTransform * Transform;
                t = t * ScaleGroup.Transform;
                foreach (var dataGroup in Find<ILPlotCubeDataGroup>()) {

                    Matrix4 invT = Matrix4.Invert(t * dataGroup.Transform);
                    Vector4 worldLast = invT * startClip;
                    if (worldLast.W != 0) {
                        worldLast = worldLast / worldLast.W;
                    }
                    Vector4 worldEnd = invT * endClip;
                    if (worldEnd.W != 0) {
                        worldEnd = worldEnd / worldEnd.W;
                    }
                    Vector3 offset = worldLast.Xyz - worldEnd.Xyz;
                    dataGroup.Limits.Set(dataGroup.Limits.Min + offset, dataGroup.Limits.Max + offset * scale);
                }
                m_mouseStartF = args.LocationF;
                args.Cancel = true;
                args.Refresh = true;
                #endregion
            }
        }

        protected internal override void OnMouseWheel(ILMouseEventArgs args) {
            RaiseMouseWheel(args); 
            if (args.Cancel) return; 
            if (!args.DirectionUp) return; 

            if (AllowZoom) {
                float val = 1f + Math.Sign(args.Delta) * 0.1f;
                foreach (var dataGroup in Find<ILPlotCubeDataGroup>()) {
                    if (TwoDMode) {
                        Vector3 center = dataGroup.Limits.CenterF;
                        center.Z = 0;
                        dataGroup.Limits.Update(center, val);
                    } else {
                        dataGroup.Limits.Update(dataGroup.Limits.CenterF, val);
                    }
                }
                args.Refresh = true; 
            }
        }
        internal static void checkFilterDirection(ILMouseEventArgs e, out bool useX, out bool useY, PointF mouseDown) {
            if (!e.AltPressed) {
                useX = true;
                useY = true;
            } else {
                float distX = Math.Abs(mouseDown.X - e.Location.X);
                float distY = Math.Abs(mouseDown.Y - e.Location.Y);
                if (distX > distY) {
                    useX = true;
                    useY = false;
                } else {
                    useX = false;
                    useY = true;
                }
            }
        }

        #endregion


    }
}
