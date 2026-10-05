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
using System.Xml.Serialization;
using ILNumerics.Data; 

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILLines : ILShape {

        #region attributes
        private int m_width;
        private DashStyle m_style;
        private short m_pattern;
        private float m_patternScale;
        private bool m_antialiasing;
        public static float PolygonOffset = 0.001f; 
        #endregion

        #region properties
        /// <summary>
        /// Get/set the width of the lines, default: 1
        /// </summary>
        [XmlAttribute]
        public int Width {
            get {
                return m_width;
            }
            set {
                // for width = 1, antialiasing will not work and 
                // must get disabled temporarily here 
                if (value != m_width) {
                    m_width = value;
                    OnPropertyChanged("Width");
                }
            }
        }
        /// <summary>
        /// Dash style for the lines, default: solid
        /// </summary>
        [XmlAttribute]
        public DashStyle DashStyle {
            get {
                return m_style;
            }
            set {
                if (value != m_style) {
                    m_style = value;
                    OnPropertyChanged("DashStyle");
                }
            }
        }
        /// <summary>
        /// User defined line stipple pattern for dash style 'UserPattern'
        /// </summary>
        /// <remarks>the pattern is defined by corresponding bits 
        /// set in the short value. It may be stretched via the 
        /// LinePatternScale parameter. Default: 15</remarks>
        [XmlAttribute]
        public short Pattern {
            get {
                return m_pattern;
            }
            set {
                if (value != m_pattern) {
                    m_pattern = value;
                    OnPropertyChanged("Pattern");
                }
                if (DashStyle != Drawing.DashStyle.UserPattern) {
                    DashStyle = DashStyle.UserPattern;
                }
            }
        }
        /// <summary>
        /// Scaling for line stipple patterns (default: 2.0f)
        /// </summary>
        [XmlAttribute]
        public float PatternScale {
            get {
                return m_patternScale;
            }
            set {
                if (value != m_patternScale) {
                    m_patternScale = value;
                    OnPropertyChanged("PatternScale");
                }
            }
        }

        /// <summary>
        /// Get/set value, if lines are drawn with smooth antialiasing (if possible and supported)
        /// </summary>
        /// <remarks>Smooth edges will be drawn if the driver supports antialiased lines. 
        /// This sometimes comes with the drawback of the lines apppearing to be 
        /// thicker. Not all objects support antialiasing. Default value is 'false'.</remarks>
        [XmlAttribute]
        public bool Antialiasing {
            get {
                return m_antialiasing;
            }
            set {
                if (m_antialiasing != value) {
                    m_antialiasing = value;
                }
                OnPropertyChanged("Antialiasing");
            }
        }
        /// <summary>
        /// Get the number of vertices used per line: 2
        /// </summary>
        [XmlAttribute]
        public override int VerticesPerPrimitive {
            get {
                return 2; 
            }
        }
        #endregion

        #region constructors
        public ILLines(object tag = null)
            : base(tag) {
            Type = Primitives.Lines;
            m_style = DashStyle.Solid;
            m_width = 1;
            m_pattern = 15;
            m_patternScale = 2.0f;
            m_antialiasing = false; 
            Color = null; 
        }
        internal ILLines(ILLines source)
            : base(source) {
            Type = Primitives.Lines;
            m_style = source.DashStyle;
            m_width = source.Width;
            m_pattern = source.Pattern;
            m_patternScale = source.PatternScale;
            m_antialiasing = source.Antialiasing;
        }
        private ILLines() { }
        #endregion

        #region public interface

        public virtual IEnumerable<Line> GetSortedandClippedScreen(ILRenderParameter parameters, SortingMode sorting = SortingMode.None, bool frustumClipping = false) {
            using (ILScope.Enter()) {
                if (Positions.IsEmpty) yield break;
                ILArray<float> positions_camera = 1;
                ILArray<int> indices = 1;
                ILArray<float> positions_screen = 1;
                ILArray<float> normals_camera = 1;
                ILArray<float> dashOffsets = ILMath.empty<float>();  

                bool userClipping = parameters.PeekClipping() != null;
                int minPlaneId = frustumClipping ? 0 : 6;
                int maxPlaneId = userClipping ? 12 : 6;
                StartPipeline(parameters, sorting, frustumClipping,
                                 positions_camera, indices, positions_screen, normals_camera);
                int maxIndex = GetPrimitiveCount();

                if (DashStyle != Drawing.DashStyle.Solid) {
                    using (ILScope.Enter()) {
                        ILArray<int> ind = GetIndicesForSorting(Indices.Storage);
                        ILArray<float> dScreen = positions_screen["0,1", ind["0;:"]] - positions_screen["0,1", ind["1;:"]];
                        dashOffsets.a = ILMath.cumsum(ILMath.sqrt(ILMath.sum(dScreen * dScreen, dim: 0)));
                    }
                }

                for (int curIndex = 0; curIndex < maxIndex; curIndex++) {
                    #region make new line
                    Line ret;
                    ret.PatternOffset = (!dashOffsets.IsEmpty) ? dashOffsets.GetValue(curIndex) : 0; 
                    int i0 = curIndex * 2, i1 = i0 + 1;
                    if (!indices.IsEmpty) {
                        i0 = indices.GetValue(i0);
                        i1 = indices.GetValue(i1);
                    }

                    ret.PCam1 = positions_camera.GetPosition4At(i0);
                    ret.PCam2 = positions_camera.GetPosition4At(i1);
                    ret.P1 = positions_screen.GetPositionAt(i0);
                    ret.P2 = positions_screen.GetPositionAt(i1);
                    ret.P1.Z -= ILLines.PolygonOffset;
                    ret.P2.Z -= ILLines.PolygonOffset; 

                    if (float.IsNaN(ret.P1.X) || float.IsNaN(ret.P1.Y) ||
                        float.IsNaN(ret.P2.X) || float.IsNaN(ret.P2.Y))
                        continue;

                    if (parameters.ColorOverride.Peek().HasValue) {
                        ret.C1 = parameters.ColorOverride.Peek().GetValueOrDefault().ToVector4();
                        ret.C2 = parameters.ColorOverride.Peek().GetValueOrDefault().ToVector4();
                    } else if (Color.HasValue) {
                        ret.C1 = Color.Value.ToVector4();
                        ret.C2 = Color.Value.ToVector4();
                    } else if (Colors.DataCount > i1) {
                        ret.C1 = Colors.GetVector4At(i0);
                        ret.C2 = Colors.GetVector4At(i1);
                    } else {
                        ret.C1 = new Vector4(0, 0, 0, 1);
                        ret.C2 = new Vector4(0, 0, 0, 1);
                    }
                    if (parameters.Alpha.Peek() < 1) {
                        ret.C1.W = parameters.Alpha.Peek();
                        ret.C2.W = parameters.Alpha.Peek(); 
                    }
                    if (Normals != null && Normals.DataCount > i1) {
                        ret.N1 = Normals.GetNormalAt(i0);
                        ret.N2 = Normals.GetNormalAt(i1);
                    } else {
                        ret.N1 = new Vector3();
                        ret.N2 = new Vector3(); 
                    }
                    #endregion
                    
                    #region frustum + user clipping
                    bool skip = false;
                    for (int c = minPlaneId; c < maxPlaneId; c++) {
                        Vector4 K = parameters.GetClipping(c); 
                        if (Vector4.Dot(K, ret.PCam1) < 0) {
                            if (Vector4.Dot(K, ret.PCam2) < 0) {
                                // skip entirely 
                                skip = true;
                                break;
                            } else {
                                float t;
                                ret.PCam1 = ILHelper.ComputeNewVertex(ret.PCam2, ret.PCam1, K, out t);
                                ret.P1 = parameters.Cam2Screen(ret.PCam1);
                                ret.C1 = ret.C2 * (1 - t) + ret.C1 * t;
                                ret.N1 = ret.N2 * (1 - t) + ret.N1 * t;
                            }
                        } else if (Vector4.Dot(K, ret.PCam2) < 0) {
                            float t;
                            ret.PCam2 = ILHelper.ComputeNewVertex(ret.PCam1, ret.PCam2, K, out t);
                            ret.P2 = parameters.Cam2Screen(ret.PCam2);
                            ret.C2 = ret.C1 * (1 - t) + ret.C2 * t;
                            ret.N2 = ret.N1 * (1 - t) + ret.N2 * t;
                        }
                    }
                    if (skip) continue;
                    #endregion

                    yield return ret;
                }
            }
        }

        internal override ILNode Copy() {
            return new ILLines(this); 
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILLines(); 
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILLines ret = (ILLines)base.Synchronize(copy, syncParams);
            if (ret.SynchedVersion != Version) {
                ret.m_style = DashStyle;
                ret.m_width = Width;
                ret.m_pattern = Pattern;
                ret.m_patternScale = PatternScale;
                ret.m_antialiasing = Antialiasing;
            }
            return ret; 
        }
        #endregion
    }
}
