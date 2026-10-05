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
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace ILNumerics.Drawing {
    /// <summary>
    /// Screen oriented rectangle with viewport relative position and size ([0..1])
    /// </summary>
    [Serializable]
    public class ILScreenObject : ILGroup {
         
        #region attributes
        public static readonly string BorderTag = "Border"; 
        public static readonly string BackgroundTag = "Background"; 
        PointF m_location; 
        PointF m_anchor; 
        //SizeF? m_size;
        float? m_width; 
        float? m_height; 
        private Units m_heightUnit;
        private Units m_widthUnit;
        private Units m_locationYUnit;
        private Units m_locationXUnit;
        private bool m_oldDepthTestFlag;
        private PointF m_mouseDownLocation;
        private PointF m_mouseDownLocationF;
        private bool m_movable = true;
        private float m_ZCoord;
        private SizeF m_minimumSize;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Z coordinate for this screen object. Childs will take this as base depth value.
        /// </summary>
        [XmlAttribute]
        public float ZCoord {
            get {
                return m_ZCoord;
            }
            set {
                if (m_ZCoord != value) {
                    m_ZCoord = value;
                    OnPropertyChanged("ZCoord");
                }
            }
        }

        /// <summary>
        /// Dis-/allows the user to move the rectangle on screen interactively (if supported by the driver). Default: true 
        /// </summary>
        [XmlAttribute]
        public bool Movable {
            get {
                return m_movable;
            }
            set {
                if (m_movable != value) {
                    m_movable = value;
                    OnPropertyChanged("Movable");
                }
            }
        }

        /// <summary>
        /// Border of the screen rectangle
        /// </summary>
        [XmlIgnore]
        public ILLines Border {
            get {
                return First<ILLines>(BorderTag);
            }
        }
        /// <summary>
        /// Background fill area 
        /// </summary>
        [XmlIgnore]
        public ILTriangles Background {
            get {
                return First<ILTriangles>(BackgroundTag);
            }
        }
        
        /// <summary>
        /// Gets the reference point for the location of the screen rectangle relative to the rectangles size. Default: center of the rectangle (.5,.5)
        /// </summary>
        [ILXmlSerializeAs("{X},{Y}")]
        public PointF Anchor {
            get {
                return m_anchor; 
            }
            set {
                if (m_anchor != value) {
                    m_anchor = value;
                    OnPropertyChanged("Anchor"); 
                }
            }
        }
        /// <summary>
        /// Location of the anchor point of the screen rectangle in units defined by LocationXUnit and LocationYUnit. Default: viewport center 
        /// </summary>
        [ILXmlSerializeAs("{X},{Y}")]
        public PointF Location {
            get {
                return m_location;
            }
            set {
                if (m_location != value) {
                    m_location = value;
                    OnPropertyChanged("Location");
                }
            }
        }
        /// <summary>
        /// Width of the screen rectangle in units defined by WidthUnit. Default: Auto (null); determined by content
        /// </summary>
        [XmlAttribute]
        public float? Width {
            get {
                return m_width;
            }
            set {
                if (m_width != value) {
                    m_width = value;
                    OnPropertyChanged("Width");
                }
            }
        }
        /// <summary>
        /// Height of the screen rectangle in units defined by HeightUnit. Default: Auto (null); determined by content
        /// </summary>
        [XmlAttribute]
        public float? Height {
            get {
                return m_height;
            }
            set {
                if (m_height != value) {
                    m_height = value;
                    OnPropertyChanged("Height");
                }
            }
        }

        /// <summary>
        /// Minimum size of the screen rect when Size mode is null (auto), in units defined by WidthUnit and HeightUnit
        /// </summary>
        [ILXmlSerializeAs("{Width},{Height}")]
        public SizeF MinimumSize {
            get {
                return m_minimumSize;
            }
            set {
                if (m_minimumSize != value) {
                    m_minimumSize = value;
                    OnPropertyChanged("MinimumSize");
                }
            }
        }
        
        /// <summary>
        /// Units for the horizontal location of the anchor point
        /// </summary>
        [XmlAttribute]
        public Units LocationXUnit {
            get {
                return m_locationXUnit;
            }
            set {
                if (m_locationXUnit != value) {
                    m_locationXUnit = value;
                    OnPropertyChanged("LocationXUnit");
                }
            }
        }
        /// <summary>
        /// Units for the vertical location of the anchor point
        /// </summary>
        [XmlAttribute]
        public Units LocationYUnit {
            get {
                return m_locationYUnit;
            }
            set {
                if (m_locationYUnit != value) {
                    m_locationYUnit = value;
                    OnPropertyChanged("LocationYUnit");
                }
            }
        }
        /// <summary>
        /// Units for the width of the screen rectangle
        /// </summary>
        [XmlAttribute]
        public Units WidthUnit {
            get {
                return m_widthUnit;
            }
            set {
                if (m_widthUnit != value) {
                    m_widthUnit = value;
                    OnPropertyChanged("WidthUnit");
                }
            }
        }
        /// <summary>
        /// Units for the height of the screen rectangle
        /// </summary>
        [XmlAttribute]
        public Units HeightUnit {
            get {
                return m_heightUnit;
            }
            set {
                if (m_heightUnit != value) {
                    m_heightUnit = value;
                    OnPropertyChanged("HeightUnit");
                }
            }
        }
        #endregion

        #region ctors
        private ILScreenObject() { }
        protected ILScreenObject(ILScreenObject source)
            : base(source) {
            m_anchor = source.m_anchor; 
            m_location = source.m_location; 
            m_locationXUnit = source.m_locationXUnit; 
            m_locationYUnit = source.m_locationYUnit;

            m_height = source.m_height; 
            m_width = source.m_width; 
            m_widthUnit = source.m_widthUnit; 
            m_heightUnit = source.m_heightUnit; 
            m_movable = source.m_movable;
            
            RegisterEvents();

        }
        public ILScreenObject(object tag = null)
            : base(tag) {
            m_anchor = new PointF(0.5f,0.5f);
            m_heightUnit = Units.Pixels;
            m_location = new PointF(0.5f, 0.5f);
            m_locationXUnit = Units.Viewport;
            m_locationYUnit = Units.Viewport;
            //m_size = new SizeF(100,100); 
            m_widthUnit = Units.Pixels;
            Target = RenderTarget.Screen2DNear; 
            Add(new ILTrianglesFan(BackgroundTag)); 
            Background.Positions.Update(new float[,] {
                {0,0,-.05f},
                {1,0,-.05f},
                {1,1,-.05f},
                {0,1,-.05f}
            });
            Background.Colors.Update(null);
            Background.Color = Color.White; 
            Background.AutoNormals = false; 
            Background.Normals.Update(null);

            Background.Markable = false;

            RegisterEvents();
            
            Add(new ILLineStrip(BorderTag)); 
            Border.Positions = Background.Positions;
            Border.Indices.Update(new int[] { 3, 0, 1, 2, 3 }); 
            Border.Colors.Update(null); 
            Border.Normals.Update(null); 
            Border.AutoNormals = false; 
            Border.Color = Color.DarkGray; 
            Border.Width = 1; 
        }

        private void RegisterEvents() {
            MouseDown += (s, arg) => {
                if (m_movable) {
                    arg.Cancel = true;
                    m_mouseDownLocation = arg.Location;
                    m_mouseDownLocationF = arg.LocationF;
                }
            };
            MouseUp += (s, arg) => {
                if (m_movable)
                    arg.Cancel = true;
            };
            MouseMove += (s, arg) => {
                if (m_movable && arg.Button == System.Windows.Forms.MouseButtons.Left) {
                    float newX = m_locationXUnit == Units.Pixels ?
                                    Location.X + (arg.Location.X - m_mouseDownLocation.X) :
                                    Location.X + (arg.LocationF.X - m_mouseDownLocationF.X);
                    float newY = m_locationYUnit == Units.Pixels ?
                                    Location.Y + (arg.Location.Y - m_mouseDownLocation.Y) :
                                    Location.Y + (arg.LocationF.Y - m_mouseDownLocationF.Y);
                    Location = new PointF(newX, newY);
                    m_mouseDownLocation = arg.Location;
                    m_mouseDownLocationF = arg.LocationF;
                    arg.Cancel = true;
                    arg.Refresh = true;
                }

            };
            MouseEnter += (s, args) => {
                var form = System.Windows.Forms.Form.ActiveForm;
                if (form != null) {
                    form.Cursor = System.Windows.Forms.Cursors.SizeAll;
                };
                args.Cancel = true;
            };
            MouseLeave += (s, args) => {
                var form = System.Windows.Forms.Form.ActiveForm;
                if (form != null) {
                    form.Cursor = System.Windows.Forms.Cursors.Default;
                };
                args.Cancel = true;
            };
        }
        #endregion

        #region public functions
        public void MoveByPixels(float xPx, float yPx, float xVp, float yVp) {
            float newX = 0, newY = 0;
            if (m_locationXUnit == Units.Pixels) {
                newX = Location.X + xPx; 
            } else if (m_locationXUnit == Units.Viewport) {
                newX = Location.X + xVp; 
            }
            if (m_locationYUnit == Units.Pixels) {
                newY = Location.Y + yPx;
            } else if (m_locationYUnit == Units.Viewport) {
                newY = Location.Y + yVp;
            }
            Location = new PointF(newX, newY); 
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILScreenObject ret = (ILScreenObject)base.Synchronize(copy, syncParams);
            if (copy == null || ret.SynchedVersion != Version) {
                ret.m_anchor = m_anchor;
                ret.m_heightUnit = m_heightUnit;
                ret.m_location = m_location;
                ret.m_locationXUnit = m_locationXUnit;
                ret.m_locationYUnit = m_locationYUnit;
                if (m_width.HasValue)
                    ret.m_width = m_width;
                if (m_height.HasValue)
                    ret.m_height = m_height;
                ret.m_widthUnit = m_widthUnit;
                ret.m_minimumSize = m_minimumSize; 
                ret.m_movable = m_movable; 
            }
            return ret; 
        }
        internal override ILNode Copy() {
            return new ILScreenObject(this);
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILScreenObject();
        }

        protected override bool BeginVisit(ILRenderParameter parameter) {
            if (parameter.CurrentPassCount == 0) {
                CalculateTransform(parameter); 
            }
            base.BeginVisit(parameter);
            parameter.Pop(); 
            parameter.PushNew(Transform);

            var proj = Matrix4.Identity; 
            // invert Z axis, important for depth sorting in screen coords
            proj.M33 = -1; 
            parameter.PushProjectionTransform(proj); 

            parameter.LogState.Push(new Vector4()); 
            parameter.PushClipping(null); 
            //m_oldDepthTestFlag = parameter.DepthTestEnabled; 
            //parameter.DepthTestEnabled = false; 
            return true; 
        }

        protected override void EndVisit(ILRenderParameter parameter) {
            //parameter.DepthTestEnabled = m_oldDepthTestFlag;

            parameter.PopClipping(); 
            parameter.LogState.Pop();
            parameter.PopProjectionTransform();
            base.EndVisit(parameter);
        }

        protected override void getLimitsInternal(Stack<Matrix4> transforms, ILLimits ret, bool ignoreRootTransform = true, Vector3? lowerBound = null) {
            //base.getLimitsInternal(transforms, ret, ignoreRootTransform, lowerBound);
        }
        #endregion

        #region private helpers
        private void CalculateTransform(ILRenderParameter parameter) {
            SizeF size = new SizeF(Width.HasValue ? Width.GetValueOrDefault() : MinimumSize.Width, Height.HasValue ? Height.GetValueOrDefault() : MinimumSize.Height); 
            float w = m_widthUnit == Units.Viewport ? size.Width : size.Width / (parameter.Driver.Size.Width - 1) / parameter.ViewportScaleFactor.Width;
            float h = m_heightUnit == Units.Viewport ? size.Height : size.Height / (parameter.Driver.Size.Height - 1) / parameter.ViewportScaleFactor.Height;
            float x = m_locationXUnit == Units.Viewport ? m_location.X : m_location.X / (parameter.Driver.Size.Width - 1) / parameter.ViewportScaleFactor.Width;
            float y = m_locationYUnit == Units.Viewport ? m_location.Y : m_location.Y / (parameter.Driver.Size.Height - 1) / parameter.ViewportScaleFactor.Height;
            Matrix4 trans = Matrix4.OrthographicTransform(0, 1, 0, 1, 1, -1) *
                Matrix4.Translation(x - m_anchor.X * w, y - m_anchor.Y * h, m_ZCoord) * 
                Matrix4.ScaleTransform(w, h, 1);
            Transform = trans;                 
        } 
        #endregion


    }
}
