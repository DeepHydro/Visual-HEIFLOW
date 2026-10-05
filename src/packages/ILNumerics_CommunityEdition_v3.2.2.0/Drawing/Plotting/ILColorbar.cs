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
using System.Linq;
using System.Text;
using ILNumerics.Drawing; 
using ILNumerics.Drawing.Plotting; 
using ILNumerics;
using System.Xml.Serialization; 

namespace ILNumerics.Drawing {
    /// <summary>
    /// Colorbar objects are used to visualize colormaps 
    /// </summary>
    /// <remarks>Colorbars are added to arbitrary object groups, supporting the IILColormapProvider interface. <see cref="ILSurface"/> is a common example. 
    /// They visualize the colormap and the data range mapped to the colors for the object.</remarks>
    [Serializable]
    public class ILColorbar : ILScreenObject {

        #region attributes

        public static readonly string BorderInnerTag = "BorderInner";
        public static readonly string ColorFillTag = "ColorFill";
        public static readonly string ColorFillGroupTag = "ColorFillGroup";
        public static readonly string AxisTag = "Axis";
        public static readonly string ColorbarTag = "Colorbar";

        ///// <summary>
        ///// Function used to locate the source colormap provider for this colorbar
        ///// </summary>
        ///// <remarks>This function by default takes the first colormap provider among its parents on its path up to the root.</remarks>
        //public Func<ILColorbar, IILColormapProvider> SourceFinder = (cb) => {
        //    ILGroup cur = cb.Parent;
        //    while (cur != null) {
        //        if (cur is IILColormapProvider)
        //            return (IILColormapProvider)cur;
        //        cur = cur.Parent;
        //    }
        //    return null; 
        //};
        private SizeF m_padding;
        private RectangleF? m_colorsRect; 
        private long m_lastColormapVersion; 
        private int m_lastColormapHashCode; 
        private Units m_paddingXUnit;
        private Units m_paddingYUnit;
        private IILColormapProvider m_colormapProvider; 
        private float m_colorFillWidth;
        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the width of the inner colored bar area; fractions / multiples of the font height in pixels
        /// </summary>
        /// <remarks>As measure of the inner colored bar width, the height of the font for the tick labels is used. If the font gets resized, the bar width does so automatically. 
        /// The tick label font size is determined by ILColorbar.Axis.Ticks.DefaultLabel.Font.Height</remarks>
        public float ColorFillWidth {
            get {
                return m_colorFillWidth;
            }
            set {
                if (m_colorFillWidth != value) {
                    m_colorFillWidth = value;
                    OnPropertyChanged("ColorFillWidth");
                }
            }
        }

        public Units PaddingXUnit {
            get {
                return m_paddingXUnit;
            }
            set {
                if (m_paddingXUnit != value) {
                    m_paddingXUnit = value;
                    OnPropertyChanged("PaddingXUnit");
                }
            }
        }
        public Units PaddingYUnit {
            get {
                return m_paddingYUnit;
            }
            set {
                if (m_paddingYUnit != value) {
                    m_paddingYUnit = value;
                    OnPropertyChanged("PaddingYUnit");
                }
            }
        }
        public RectangleF? ColorsRect {
            get { return m_colorsRect; }
            set {
                if (m_colorsRect != value) {
                    m_colorsRect = value; 
                    OnPropertyChanged("ColorsRect"); 
                }
            }
        }
        [XmlIgnore]
        protected ILGroup InnerGroup {
            get { return First<ILGroup>(tag: ColorFillGroupTag); }
        }
        [XmlIgnore]
        public ILLineStrip BorderInner {
            get {
                return First<ILLineStrip>(tag: BorderInnerTag);
            }
        }
        [XmlIgnore]
        public ILTriangles ColorFill {
            get {
                return First<ILTriangles>(tag: ColorFillTag);
            }
        }
        [XmlIgnore]
        public ILAxis Axis {
            get {
                return First<ILAxis>(tag: AxisTag);
            }
        }
        [XmlIgnore]
        public IILColormapProvider ColormapProvider {
            get {
                if (m_colormapProvider == null) {
                    m_colormapProvider = FirstUp<ILGroup>(predicate: g => g is IILColormapProvider) as IILColormapProvider; 
                }
                return m_colormapProvider; 
            }
            set {
                if (value != m_colormapProvider) {
                    m_colormapProvider = value; 
                    OnPropertyChanged("ColormapProvider"); 
                }
            }
        }
        /// <summary>
        /// Padding between inner elements of the colorbar and the outer border in units defined by PaddingXUnit and PaddingYUnit
        /// </summary>
        public SizeF Padding {
            get { return m_padding; }
            set {
                if (m_padding != value) {
                    m_padding = value;
                    OnPropertyChanged("Padding");
                }
            }
        }
        #endregion

        #region ctors
        public ILColorbar(object tag = null)
            : base(tag ?? ColorbarTag) {
            Padding = new SizeF(10,10);

            Add(new ILGroup(ColorFillGroupTag));

            this.InnerGroup.Add(new ILTriangles(ColorFillTag));
            ColorFill.Normals.Update(null); 
            ColorFill.AutoNormals = false; 
            ColorFill.Markable = false;
            ColorFillWidth = 2; 

            this.InnerGroup.Add(new ILLineStrip(BorderInnerTag));
            BorderInner.Positions.Update(new float[,] {
                {0,0,0.1f},
                {1,0,0.1f},
                {1,1,0.1f},
                {0,1,0.1f}
            });
            BorderInner.Indices.Update(new int[] { 3, 0, 1, 2, 3 });
            BorderInner.Colors.Update(null);
            BorderInner.Normals.Update(null);
            BorderInner.AutoNormals = false;
            BorderInner.Color = Color.DarkGray;
            BorderInner.Width = 1;

            this.InnerGroup.Add(new ILAxis(null, AxisTag) {
                AxisName = AxisNames.CAxis,
                Direction = new Vector3(0, -1, 0),
                Position = new Vector3(0, 1, 0.5f)
            });
            Axis.Label.Visible = false;
            Axis.GridMajor.Visible = false;
            Axis.GridMinor.Visible = false; 
            Axis.Ticks.TickLength = -2;
            
            //m_axis.Ticks.DefaultFont = new Font(m_axis.Ticks.DefaultFont.FontFamily, 7.0f); 

            HeightUnit = Units.Viewport; 
            WidthUnit = Units.Pixels;
            Height = 0.4f; 
            Width = null; // auto
 
            //SizeF tickSize = Axis.Ticks.DefaultTickLabelSize;
            //Size = new SizeF((tickSize.Width + 10) * 1.8f, 0.4f); //(tickSize.Width + 10) * 4);

            Location = new PointF(0.9f,0.1f); 
            LocationXUnit = LocationYUnit = Units.Viewport; 
            Anchor = new PointF(1,0);

            //if (Source != null)
            //    m_axis = Add(new ILAxis(Source, DefaultAxisTag) { AxisName = AxisNames.ZAxis });
            //Setup(); 
        }
        internal ILColorbar(ILColorbar source) : base(source) {
            m_padding = source.m_padding; 
            m_colorFillWidth = source.m_colorFillWidth; 
            m_paddingXUnit = source.m_paddingXUnit; 
            m_paddingYUnit = source.m_paddingYUnit;             
        }
        private ILColorbar() { }
        #endregion

        #region public functions 
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILColorbar(); 
        }
        internal override ILNode Copy() {
            return new ILColorbar(this);
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILColorbar ret = (ILColorbar)base.Synchronize(copy, syncParams);
            if (copy == null || ret.SynchedVersion != Version) {
                ret.m_padding = m_padding;
                ret.m_paddingXUnit = m_paddingXUnit; 
                ret.m_paddingYUnit = m_paddingYUnit; 
                ret.m_colorFillWidth = m_colorFillWidth; 
            }
            return ret; 
        }
        protected override bool BeginVisit(ILRenderParameter parameter) {
            if (parameter.CurrentPassCount == 0) {
                IILColormapProvider colormapProvider = ColormapProvider;
                if (colormapProvider != null && colormapProvider.IsColormapped) {

                    #region establish size of ILScreenObj 
                    int tickCount = (int)(getHeightInPixels(parameter) / Axis.Ticks.DefaultLabel.Font.Height); 
                    float min = colormapProvider.GetRangeMinValue(Axis.AxisName), max = colormapProvider.GetRangeMaxValue(Axis.AxisName);
                    float scale = (float)Math.Truncate(Math.Max(Math.Log10(Math.Abs(min)), Math.Log10(Math.Abs(max))));
                    bool needsScaleLabel = scale < 0 ? Math.Abs(scale) >= Axis.Ticks.MaxNumberDigitsShowFull - 1 
                                                     : Math.Abs(scale) >= Axis.Ticks.MaxNumberDigitsShowFull; 

                    var ticksEstimate = Axis.Ticks.TickCreationFunc(min, max, tickCount);
                    // get true label size 
                    float maxLabSz = 0;
                    ILLabel dummy = new ILLabel();
                    if (needsScaleLabel) {
                        // measure scale label only
                        dummy.Text = "\u00D710^{" + scale.ToString() + "}"; 
                        maxLabSz = dummy.MeasureSize().Width; 
                    } else {
                        // measure individual ticks
                        int i = 0;
                        foreach (var lab in ticksEstimate) {
                            dummy.Text = Axis.Ticks.LabelTransformFunc(i++, lab);
                            SizeF s = dummy.MeasureSize();
                            if (s.Width > maxLabSz)
                                maxLabSz = s.Width;
                        }
                    }
                    base.Width = maxLabSz + Padding.Width * 2 + ColorFillWidth * Axis.Ticks.DefaultLabel.Font.Height; 
                    base.WidthUnit = Units.Pixels; 
                    base.BeginVisit(parameter); 
                    #endregion


                    #region configure inner group
                    // measure pixel size 
                    RectangleF viewRect = parameter.ViewTransform.ToViewRectangle();
                    float w = base.Width.HasValue ? Width.GetValueOrDefault() : MinimumSize.Width; 
                    float h = base.Height.HasValue ? Height.GetValueOrDefault() : MinimumSize.Height; 
                    float pixelWidth =  1 / w;
                    float pixelHeight = 1 / getHeightInPixels(parameter); // reuse!
                    float padX = PaddingXUnit == Units.Viewport ? Padding.Width : (Padding.Width * pixelWidth);
                    float padY = PaddingYUnit == Units.Viewport ? Padding.Height : (Padding.Height * pixelHeight);
                    float colorFillWidth = ColorFillWidth * pixelWidth * Axis.Ticks.DefaultLabel.Font.Height;
                    InnerGroup.Transform = Matrix4.Translation(1 - colorFillWidth, padY, 0) *
                                             Matrix4.ScaleTransform(colorFillWidth - padX, 1 - padY * 2, 1);    
                    #endregion

                    ILViewAxesParameters viewparams = new ILViewAxesParameters();
                    Vector3 u1 = Axis.Position.HasValue ? Axis.Position.Value : new Vector3(0, 1, 0.5);
                    Vector3 u2 = u1 + new Vector3(0, -1, 0);
                    viewparams.UnitCube.Add((int)Axis.AxisName, Tuple.Create(u1, u2));
                    parameter.ToScreen(ref u1, ref u2);
                    viewparams.Screen.Add((int)Axis.AxisName, Tuple.Create(
                        new PointF(u1.X, u1.Y), new PointF(u2.X, u2.Y)));
                    Axis.ScaleLabelPosition = new Vector3(1,1f,0.5f); 
                    Axis.ScaleLabelAnchor = new PointF(1,0f); 
                    Axis.ConfigureAxis(parameter, viewparams);

                    ILColormap cm = colormapProvider.Colormap;
                    if (cm.Version != m_lastColormapVersion || cm.SynchedHashCode != m_lastColormapHashCode) {
                        m_lastColormapHashCode = cm.SynchedHashCode;
                        m_lastColormapVersion = cm.Version;
                        Update(cm, viewparams);
                        Configure(); 
                    }
                } else {
                    Visible = false;
                    base.BeginVisit(parameter);
                }
            } else {
                return base.BeginVisit(parameter);
            }
            return true; 
        }

        #endregion

        #region private helper
        private float getHeightInPixels(ILRenderParameter parameters) {
            if (HeightUnit == Units.Pixels) {
                if (Height.HasValue) return Height.GetValueOrDefault(); 
                else return MinimumSize.Height;
            } else if (HeightUnit == Units.Viewport) {
                if (Height.HasValue) return Height.GetValueOrDefault() * parameters.ViewTransform.ToViewRectangle().Height;
                else return MinimumSize.Height * parameters.ViewTransform.ToViewRectangle().Height;
            } else {
                System.Diagnostics.Trace.TraceWarning("The height of the colorbar could not be computed, due to the use of an unknown unit: " + HeightUnit.ToString()); 
                return 1; 
            } 
        }
        private void Update(ILColormap cm, ILViewAxesParameters viewParams) {
            using (ILScope.Enter()) {

                //ColorFill.Positions.Update(new float[,] {
                //    {0,1,1},
                //    {1,1,-1},
                //    {0,0.5f,-0.5f}
                //});
                //ColorFill.Color = Color.Blue;
                //ColorFill.Configure();
                //return; 

                ILArray<float> keypoints = cm.Data;
                ILArray<float> positions = ILMath.zeros<float>(3, keypoints.S[0] * 2);
                Vector3 start = viewParams.UnitCube[(int)Axis.AxisName].Item1;
                Vector3 end = viewParams.UnitCube[(int)Axis.AxisName].Item2;
                positions["0;0:2:end"] = 0;
                positions["0;1:2:end"] = 1;
                ILArray<float> ys = 1 - (keypoints[":;0"] - keypoints[0] / (keypoints["end;0"] - keypoints[0]));
                positions["1;:"] = ILMath.reshape(ys.T["0,0;:"], 1, keypoints.S[0] * 2);
                positions["2;:"] = 0.1f;
                ColorFill.Positions.Update(positions);
                ILArray<float> colors = keypoints[":;1:4"].T;
                colors.a = ILMath.reshape(colors.Concat(colors, 0), 4, keypoints.S[0] * 2);
                // make it full opaque 
                colors["end;:"] = 1;
                ColorFill.Colors.Update(colors);
                ILArray<int> indices = new int[] { 0, 1, 2, 2, 1, 3 };
                indices.a = 2 * ILMath.vec<int>(0, keypoints.S[0] - 2).T + ILMath.repmat(indices, 1, keypoints.S[0] - 1);
                indices.a = ILMath.reshape(indices, 1, (keypoints.S[0] - 1) * 6);
                ColorFill.Indices.Update(indices);
            }
        }


        private void Setup() {
            BorderInner.Positions.Update(new float[,] {
                {0,0,0},
                {1,0,0},
                {1,1,0},
                {0,1,0}
            });
            BorderInner.Color = Color.DarkGray;
            BorderInner.Width = 2;

            Background.Positions = BorderInner.Positions; 
            Background.Indices.Update(new int[] {0,1,2,0,2,3}); 
            Background.Colors = new ILColorsBuffer(); 
            Background.Color = Color.White; 
            Background.AutoNormals = false; 
            Background.Normals = new ILNormalsBuffer(); 
            // m_innerGroup
            //RectangleF rect = ColorsRect ?? new RectangleF(0.5f,Padding,0.5f - Padding, 1 - 2 * Padding); 
            //m_innerGroup.Transform = Matrix4.Translation(rect.Left,rect.Top,0) * 
            //                        Matrix4.ScaleTransform(rect.Width, rect.Height,1);
            ColorFill.Positions.Update(new float[,] {
                {0,0,-1f},
                {1,0,-1f},
                {1,1,-1f},
                {0,1,-1f}
            });
            ColorFill.Indices.Update(new int[] { 0, 1, 2, 0, 2, 3 }); 
            ColorFill.Color = Color.Red;
            ColorFill.Colors = new ILColorsBuffer(); 
            ColorFill.AutoNormals = false; 
            ColorFill.Normals = new ILNormalsBuffer(); 
            Configure();
        }
        #endregion

    }
}
