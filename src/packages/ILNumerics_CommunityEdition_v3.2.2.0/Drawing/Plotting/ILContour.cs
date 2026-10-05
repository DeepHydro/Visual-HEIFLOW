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
using System.Xml.Serialization;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Exceptions;
using CONTOURLINETYPE = ILNumerics.Drawing.Plotting.ILContourLine; 

namespace ILNumerics.Drawing.Plotting {

    [Serializable]
    public class ILContourPlot : ILGroup, IILColormapProvider {

        /**
         * handle NaNs, 
         * make filled areas
         * adjust labels spacing on a contour level 
         * adjust label relative position on line
         * labels: switch full / exponent format (limit 4) 
         * 
         * set number of contours and/or give contour levels 
         * cyclic style enumerations (every 2..3..4...nth line gets different style / label)
         * - OR -
         * individual level settings: 
         * * level?, linestyle?, label?, fill color? { RGBA or index into CM }, 
         **/

        #region attributes
        /// <summary>
        /// Tag used to identify contour plot objects within the scene graph
        /// </summary>
        public static readonly string DefaultContourTag = "CountourPlot";
        /// <summary>
        /// Tag used to identify contour line objects within the scene graph
        /// </summary>
        public static readonly string DefaultLineTag = "CountourLine";
        private List<ContourLevel> m_levels;
        private ILColormap m_colormap;
        private float m_colorRangeMin;
        private float m_colorRangeMax;
        private bool m_isColormapped;
        #endregion

        #region properties
        /// <summary>
        /// Colormap used for mapping values to colors
        /// </summary>
        public ILColormap Colormap {
            get {
                return m_colormap;
            }
            set {
                if (m_colormap != value) {
                    m_colormap = value;
                    OnPropertyChanged("Colormap");
                }
            }
        }
        /// <summary>
        /// Collection of contour levels 
        /// </summary>
        [XmlArray]
        public List<ContourLevel> Levels {
            get {
                return m_levels;
            }
            set {
                if (m_levels != value) {
                    m_levels = value;
                    OnPropertyChanged("Levels");
                }
            }
        }
        /// <summary>
        /// Gets minimum for the range used to map countour data values to color values within the colormap range
        /// </summary>
        public float ColorRangeMin {
            get {
                return m_colorRangeMin;
            }
        }
        /// <summary>
        /// Gets maximum for the range used to map countour data values to color values within the colormap range
        /// </summary>
        public float ColorRangeMax {
            get {
                return m_colorRangeMax;
            }
        }
        #endregion

        #region ctors
        /// <summary>
        /// Create contour plot, automatic level definition; allows configuration of all contour lines at once
        /// </summary>
        /// <param name="Z">data matrix</param>
        /// <param name="labelColor">[optional] if set, the color for contour labels. Otherwise: take ContourLevel.Default setting</param>
        /// <param name="lineColor">[optional] if set, the color map value for the contour lines color. Otherwise: determine colors automatically</param>
        /// <param name="lineStyle">[optional] if set, the dash style for contour lines. Otherwise: take ContourLevel.Default setting</param>
        /// <param name="lineWidth">[optional] if set, sets the width for all contour lines. Otherwise: take ContourLevel.Default setting</param>
        /// <param name="showLabels">[optional] determine visibility of contour labels. Default: true</param>
        /// <param name="colormap">[optional] if set, the colormap used for mapping Z values to colors in the colormap</param>
        /// <param name="create3D">[optional] determine, if the contour lines are created with with Z coordinates of the level value. Default: false, create at Z=0.</param>
        public ILContourPlot(ILInArray<float> Z, Color? labelColor = null, float? lineColor = null, DashStyle? lineStyle = null,
                            int? lineWidth = null, bool showLabels = true, 
                            ILColormap colormap = null, bool create3D = false) {
            ContourLevel cl = ContourLevel.Default;
            if (labelColor.HasValue) cl.LabelColor = labelColor;
            if (lineColor.HasValue) cl.LineColor = lineColor;
            if (lineStyle.HasValue) cl.LineStyle = lineStyle;
            if (lineWidth.HasValue) cl.LineWidth = lineWidth;
            cl.ShowLabel = showLabels;
            m_levels = new List<ContourLevel>() { cl };
            m_colormap = colormap ?? new ILColormap();
            var helper = new ContourHelper(Z, this, create3D);
        }
        /// <summary>
        /// Create contour plot, manual level definition; individual contour level configuration
        /// </summary>
        /// <param name="Z">data matrix</param>
        /// <param name="levels">Collection of individual contour levels</param>
        /// <param name="colormap">[optional] if set, the colormap used for mapping Z values to colors in the colormap</param>
        /// <param name="create3D">[optional] determine, if the contour lines are created with with Z coordinates of the level value. Default: false, create levels at Z=0.</param>
        /// <remarks>This constructor creates a new contour plot object with individual contour level specification. <paramref name="levels"/> - if not null - 
        /// is expected to contain <see cref="ILNumerics.Drawing.Plotting.ContourLevel"/> objects with the configuration and data value for all contour lines to be created. No 
        /// levels will be aquired automatically.</remarks>
        public ILContourPlot(ILInArray<float> Z, List<ContourLevel> levels, ILColormap colormap = null, bool create3D = false) {
            m_levels = new List<ContourLevel>(); 
            m_colormap = colormap ?? new ILColormap();
            if (levels != null) 
                m_levels.AddRange(levels); 
            var helper = new ContourHelper(Z, this, create3D); 
        }
        protected ILContourPlot(ILContourPlot source)
            : base(source) {
            m_colormap = source.Colormap.Copy(); 
            m_colorRangeMax = source.m_colorRangeMax; 
            m_colorRangeMin = source.m_colorRangeMin; 
            m_isColormapped = source.m_isColormapped; 
            m_levels = new List<ContourLevel>(source.m_levels); 
        }
        private ILContourPlot() { }
        #endregion

        #region public interface 
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILContourPlot();
        }
        internal override ILNode Copy() {
            return new ILContourPlot(this);
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILContourPlot ret = (ILContourPlot)base.Synchronize(copy, syncParams);
            if (copy == null || ret.SynchedVersion != Version) {
                ret.m_colormap = m_colormap.Copy(); 
                ret.m_colorRangeMax = m_colorRangeMax; 
                ret.m_colorRangeMin = m_colorRangeMin; 
                ret.m_isColormapped = m_isColormapped; 
                ret.m_levels = new List<ContourLevel>(m_levels); 
            }
            return ret; 
        }
        #endregion

        #region private helpers
        [System.Security.SecuritySafeCritical]
        private class ContourHelper : ILMath {

            #region attributes
            ILArray<float> m_curLineBuffer = localMember<float>(); 
            int m_curLineLength = 0;
            int m_m; 
            int m_n;
            float m_zmax, m_zmin;
            Stack<CONTOURLINETYPE> m_linesPool;
            int m_levelID;
            int m_lineStartIDX, m_IDXinCline;
            IDictionary<int, int> m_shapes = new Dictionary<int,int>(10);   
            
            ILContourPlot m_plot; 
            ContourLevel m_level;
            bool m_create3D; 
            #endregion

            #region ctors
            public ContourHelper(ILInArray<float> Z, ILContourPlot plot, bool create3D = false) {
                using (ILScope.Enter(Z)) {
                    if (isnullorempty(Z) || !Z.IsMatrix || Z.S[0] < 2 || Z.S[1] < 2) {
                        throw new ILArgumentException("Parameter Z must be a matrix with more than 1 rows and more than 1 columns."); 
                    }
                    m_m = Z.S[0]; 
                    m_n = Z.S[1]; 
                    m_create3D = create3D; 
                    Z.GetLimits(out m_zmin, out m_zmax);
                    if (plot != null) {
                        m_plot = plot; 
                        // take matching shapes from plot and reuse them
                        m_linesPool = new Stack<CONTOURLINETYPE>(plot.Find<CONTOURLINETYPE>(ILContourPlot.DefaultLineTag));
                        if (m_plot.Levels == null) {
                            m_plot.Levels = new List<ContourLevel>();
                        }
                        if (m_plot.Levels.Count == 0) {
                            m_plot.Levels.Add(ContourLevel.Default); 
                        }
                        if (m_plot.Levels.Where(lev => !lev.Value.HasValue).Any()) {
                            #region setup auto levels for existing level settings
                            var levels = ILTickCollection.CreateTicksAuto(m_zmin, m_zmax, 10);

                            int i = 0;
                            for (; i < Math.Min(m_plot.Levels.Count, levels.Count); i++) {
                                if (!m_plot.Levels[i].Value.HasValue) {
                                    ContourLevel cl = m_plot.Levels[i];
                                    cl.Value = levels[i];
                                    m_plot.Levels[i] = cl;
                                }
                            }
                            int reuseID = -1, loopCount = m_plot.Levels.Count;
                            for (; i < levels.Count; i++) {
                                reuseID = (++reuseID % loopCount);
                                ContourLevel settings = m_plot.Levels[reuseID];
                                settings.Value = levels[i];
                                m_plot.Levels.Add(settings);
                            }
                            while (m_plot.Levels.Count > levels.Count) {
                                m_plot.Levels.Remove(m_plot.Levels.Last());
                            }
                            #endregion
                        }
                    } else {
                        throw new ILArgumentException("missing required parameter plot"); 
                    }
                    m_levelID = -1;
                    m_curLineBuffer.a = zeros<float>(3, 20);

                    foreach (var level in m_plot.Levels) {
                        System.Diagnostics.Debug.Assert(level.Value.HasValue); 
                        startLevel(++m_levelID); 
                        CreateContourLevel(Z - level.Value.Value, plot);
                        endLevel(); 
                    }
                    m_plot.m_isColormapped = m_plot.Levels.TrueForAll(lev => lev.LineColor == null);
                    m_plot.m_colorRangeMin = m_zmin; 
                    m_plot.m_colorRangeMax = m_zmax; 
                    // clean up
                    foreach (var line in m_linesPool) {
                        m_plot.Remove(line); 
                    }
                    // transfer collected shape ids for later reference (f.e. needed for legends) 
                    // must do it here, because we cannot change the collection while iterating ... :|
                    for (int i = 0; i < m_plot.Levels.Count; i++) {
                        var lev = m_plot.Levels[i];
                        if (m_shapes.ContainsKey(i))
                            lev.ShapeID = m_shapes[i];
                        else {
                            // the levels value was not found in the plot -> no clines created
                            lev.ShapeID = -1; 
                        }
                        m_plot.Levels[i] = lev;
                    }

                    m_linesPool = null;
                    m_curLineBuffer.Dispose();
                    m_plot = null;
                }
            }
            #endregion

            #region private helpers
            private void CreateContourLevel(ILInArray<float> inZ, ILContourPlot plot) {
                using (ILScope.Enter(inZ)) {
                    ILArray<float> Z = inZ; 
                    Z[Z==0] = epsf;  // prevent from special handling for sattlepoints (which was only partially implemented below)
                    int m = Z.S[0], n = Z.S[1];
                    ILArray<sbyte> faces = zeros<sbyte>(Z.S); 
                    sbyte[] faceArr = faces.GetArrayForWrite(); 
                    float[] ZArr = Z.GetArrayForRead(); 

                    /**
                     * Adress schema - faces[..,..] 
                     *       |- [0,0] & 1
                     *      --- --- ---  <- [0,3]
                     *     |   |   |   | <- [0,3] & 1
                     *      --- --- ---  <- [1,3]
                     *     |   |   |   | <- [1,3] & 1
                     *      --- --- --- <- [2,3]
                     *     ^[2,0]    ^[2,2] & 2          ?>
                     *  
                     *  * address: left, top corner
                     *  * horizontal edges: & 1
                     *  * vertical edges: & 2
                     **/
                    // visit border edges
                    for (int c = 0; c < n-1; c++) {
                        VisitFace(faceArr, ZArr, 0, c, m, 0, false); 
                    }
                    for (int r = 1; r < m-1; r++) {
                        VisitFace(faceArr, ZArr, r, n - 2, m, 1, false); 
                    }
                    for (int c = n-2; c--> 0;) {
                        VisitFace(faceArr, ZArr, m - 2, c, m, 2, false); 
                    }
                    for (int r = m - 2; r--> 0;) {
                        VisitFace(faceArr, ZArr, r, 0, m, 3, false);
                    }
                    for (int c = 0; c < n-1; c++) {
                        for (int r = 0; r < m-1; r++) {
                            VisitFace(faceArr, ZArr, r, c, m, 2, true); 
                        }
                        
                    }
                }
            }
            /// <summary>
            /// find crossing within a face
            /// </summary>
            /// <param name="faces">visited marks for all faces, column major order</param>
            /// <param name="Z">level values (Z - level), column major order</param>
            /// <param name="c">index of column for upper left face corner into Z and faces</param>
            /// <param name="r">index of row for upper left face corner into Z and faces</param>
            /// <param name="ldz">leading dimension of Z and faces matrices</param>
            /// <param name="edge">id of edge to start with: 0 (top)... 3 (left)</param>
            void VisitFace(sbyte[] faces, float[] Z, int r, int c, int ldz, int edge, bool inspectAllEdges = true) {
                Tuple<int, int, int> startEdge = null;
                Tuple<int, int, int> next = Tuple.Create(r, c, edge); ;
                
                // startLine
                startLine(); 
                bool first = true; 
                while (next != null) { 
                    float ret = 0;
                    r = next.Item1; c = next.Item2; edge = next.Item3;
                    int ind = r + c * ldz;

                    next = null; 
                    for (int i = 0; i < 4; edge = ++edge % 4, i++) {
                        if (startEdge != null && r == startEdge.Item1 && c == startEdge.Item2 && edge == startEdge.Item3) {
                            // finishLine 
                            finishLine(true);
                            return;
                        }
                        switch (edge) {
                            case 0: 
                                #region upper edge
                                sbyte face = faces[ind];
                                if ((face & 1) != 0) {
                                    continue;
                                }
                                // mark it visited
                                faces[ind] = (sbyte)(face | 1);
                                float f0 = Z[ind];
                                float f1 = Z[ind + ldz];
                                if (visitEdge(f0, f1, ref ret)) {
                                    AddPoint(new Vector3(ret + c, r, 0));
                                    if (first) { 
                                        first = false;
                                        startEdge = Tuple.Create(r - 1, c, (edge + 2) % 4); 
                                    } else if (r > 0) {
                                        next = Tuple.Create(r - 1, c, (edge + 3) % 4); // continue with left edge 
                                        i = 4; 
                                    } else {
                                        // reached a plot edge 
                                        System.Diagnostics.Debug.Assert(next == null); 
                                        // finish line
                                        next = null; 
                                        i = 4; 
                                    }
                                } else if (first && !inspectAllEdges) {
                                    return; 
                                }
                                break;
                                #endregion
                            case 1:
                                #region right edge
                                face = faces[ind + ldz];
                                if ((face & 2) != 0) {
                                    continue;
                                }
                                // mark it visited
                                faces[ind + ldz] = (sbyte)(face | 2);
                                f0 = Z[ind + ldz];
                                f1 = Z[ind + ldz + 1];
                                if (visitEdge(f0, f1, ref ret)) {
                                    AddPoint(new Vector3(c + 1, r + ret, 0));
                                    if (first) {
                                        first = false;
                                        startEdge = Tuple.Create(r, c + 1, (edge + 2) % 4);
                                    } else if (c < m_n - 2) {
                                        next = Tuple.Create(r, c + 1, (edge + 3) % 4); // continue with top edge
                                        i = 4;
                                    } else {
                                        // reached a plot edge 
                                        System.Diagnostics.Debug.Assert(next == null);
                                        // finish line
                                        next = null;
                                        i = 4;
                                    }
                                } else if (first && !inspectAllEdges) {
                                    return;
                                }
                                break;
                                #endregion
                            case 2:
                                #region bottom edge
                                face = faces[ind + 1];
                                if ((face & 1) != 0) {
                                    continue;
                                }
                                // mark it visited
                                faces[ind + 1] = (sbyte)(face | 1);
                                f0 = Z[ind + 1];
                                f1 = Z[ind + 1 + ldz];
                                if (visitEdge(f0, f1, ref ret)) {
                                    AddPoint(new Vector3(c + ret, r + 1, 0));
                                    if (first) {
                                        first = false;
                                        startEdge = Tuple.Create(r + 1, c, (edge + 2) % 4);
                                    } else if (r < m_m - 2) {
                                        next = Tuple.Create(r + 1, c, (edge + 3) % 4); // cont. with right edge
                                        i = 4;
                                    } else {
                                        // reached a plot edge 
                                        System.Diagnostics.Debug.Assert(next == null);
                                        // finish line
                                        next = null;
                                        i = 4;
                                    }
                                } else if (first && !inspectAllEdges) {
                                    return;
                                }
                                break;
                                #endregion
                            case 3:                                 
                                #region left edge
                                face = faces[ind];
                                if ((face & 2) != 0) {
                                    continue;
                                }
                                // mark it visited
                                faces[ind] = (sbyte)(face | 2);
                                f0 = Z[ind];
                                f1 = Z[ind + 1];
                                if (visitEdge(f0, f1, ref ret)) {
                                    AddPoint(new Vector3(c, r + ret, 0));
                                    if (first) {
                                        first = false;
                                        startEdge = Tuple.Create(r, c - 1, (edge + 2) % 4);
                                    } else if (c > 0) {
                                        next = Tuple.Create(r, c - 1, (edge + 3) % 4); // cont. bottom
                                        i = 4;
                                    } else {
                                        // reached a plot edge 
                                        System.Diagnostics.Debug.Assert(next == null);
                                        // finish line
                                        next = null;
                                        i = 4;
                                    }
                                } else if (first && !inspectAllEdges) {
                                    return;
                                }
                                break;
                                #endregion
                            default:
                                break;
                        }
                    }
                }
                // finish line
                finishLine(false); 
            }
            private void AddPoint(Vector3 xyz) {
                if (m_curLineBuffer.S[1] <= m_IDXinCline) {
                    m_curLineBuffer[0, m_IDXinCline * 2] = 0;
                }
                m_curLineBuffer.SetValue(xyz.X, 0, m_IDXinCline);
                m_curLineBuffer.SetValue(xyz.Y, 1, m_IDXinCline);
                m_curLineBuffer.SetValue(xyz.Z, 2, m_IDXinCline);
                m_IDXinCline++; 
            }

            private void startLevel(int levelID) {
                System.Diagnostics.Debug.Assert(levelID < m_plot.Levels.Count);
                m_level = ContourLevel.GetSettingOrDefault(m_plot.Levels[levelID]);
                System.Diagnostics.Debug.Assert(m_level.Value.HasValue);
                m_IDXinCline = 0; 
                m_lineStartIDX = 0;
            }
            private void endLevel() {
                if (m_IDXinCline > 0) {
                    CONTOURLINETYPE curCLine = getLine();
                    if (m_create3D) {
                        m_curLineBuffer["2;:"] = m_level.Value.Value;
                    }
                    curCLine.Line.Positions.Update(m_curLineBuffer[full, r(0, m_IDXinCline)]);
                    curCLine.Line.Indices.Update(null);
                    curCLine.Line.Colors.Update(null);
                    curCLine.Line.Width = m_level.LineWidth.HasValue ? m_level.LineWidth.GetValueOrDefault() : 1;
                    if (m_level.LineColor.HasValue) {
                        float colInd = m_level.LineColor.GetValueOrDefault();
                        curCLine.Line.Color = m_plot.Colormap.Map(colInd).ToColor();
                    } else {
                        float colInd = m_level.Value.Value;
                        curCLine.Line.Color = m_plot.Colormap.Map(colInd, Tuple.Create(m_zmin, m_zmax)).ToColor();
                    }
                    curCLine.Line.DashStyle = m_level.LineStyle.HasValue ? m_level.LineStyle.GetValueOrDefault() : DashStyle.Solid;

                    createLabels(curCLine);
                    m_shapes[m_levelID] = curCLine.ID; 
                }
            }
            private void startLine() {
                m_lineStartIDX = m_IDXinCline; 
            }

            private void finishLine(bool closeLoop) {
                if (m_IDXinCline - m_lineStartIDX > 1) {
                    if (closeLoop) {
                        m_curLineBuffer[full, m_IDXinCline] = m_curLineBuffer[full,m_lineStartIDX];
                        m_IDXinCline++; 
                    }
                    // mark EOL
                    m_curLineBuffer[full,m_IDXinCline++] = float.NaN; 
                }
            }

            private void createLabels(CONTOURLINETYPE cline) {
                if (cline == null || cline.Line.Positions.DataCount < 3) 
                    return; 
                ILLabel label = cline.Label; 

                if (m_level.LabelColor.HasValue) 
                    label.Color = m_level.LabelColor.Value; 
                if (m_level.Text != null) 
                    label.Text = m_level.Text; 
                else 
                    label.Text = m_level.Value.Value.ToString("g"); 
                // position label (rotation is done in beginvisit)
                label.Position = cline.Line.Positions.GetPositionAt(1); 
                cline.LabelTarget = m_create3D ? RenderTarget.World3D : RenderTarget.Screen2DNear; 
                cline.ShowLabel = m_level.ShowLabel ?? true; 
            }
            private CONTOURLINETYPE getLine() {
                if (m_linesPool != null && m_linesPool.Count > 0) {
                    return m_linesPool.Pop();
                } else {
                    return m_plot.Add(new CONTOURLINETYPE(DefaultLineTag)); 
                }
            }

            private static bool visitEdge(float f0, float f1, ref float ret) {
                #region value checking
                if (float.IsNaN(f0) || float.IsNaN(f1) || f0 * f1 > 0) {
                    return false; 
                }
                if (f1 == 0) {
                    if (f0 < 0) {
                        f1 = epsf;
                    } else if (f0 > 0) {
                        f1 = -epsf;
                    } else {
                        System.Diagnostics.Trace.WriteLine("Level or sattle area detected. Only the edge will be marked.");
                        return false; 
                    }
                } else if (f0 == 0) {
                    if (f1 < 0) {
                        f0 = epsf;
                    } else if (f1 > 0) {
                        f0 = -epsf;
                    } else {
                        System.Diagnostics.Trace.WriteLine("Level or sattle area detected. Only the edge will be marked.");
                        return false;
                    }
                }
                #endregion
                // we have a match! compute crossing point
                ret = -f0 / (f1 - f0);
                return true;
            }
            #endregion
        }

        #endregion

        #region IILColormapProvider Members
        /// <summary>
        /// Determines if this contour plot uses a colormap for coloring contour lines
        /// </summary>
        [XmlIgnore]
        public bool IsColormapped {
            get { return m_isColormapped; }
        }

        #endregion

        #region IILAxisDataProvider Members
        /// <summary>
        /// Provides the minimum axis scale range for rendering axis scales
        /// </summary>
        /// <param name="AxisName">Name of the axis to query</param>
        /// <returns>Minimum color range</returns>
        public float GetRangeMinValue(AxisNames AxisName) {
            return m_colorRangeMin; 
        }

        /// <summary>
        /// Provides the maximum axis scale range for rendering axis scales
        /// </summary>
        /// <param name="AxisName">Name of the axis to query</param>
        /// <returns>Maximum color range</returns>
        public float GetRangeMaxValue(AxisNames AxisName) {
            return m_colorRangeMax; 
        }
        /// <summary>
        /// Provides the scale mode (linear/logarithmic) for rendering axis scales
        /// </summary>
        /// <param name="AxisName">Name of the axis to query</param>
        /// <returns>Linear scale mode</returns>
        public AxisScale ScaleMode(AxisNames AxisName) {
            return AxisScale.Linear; 
        }

        #endregion

        #region IILLegendDataProvider Members

        //public IDictionary<int, string> GetAllEntries(IEnumerable<int> predefined) {
        //    if (predefined != null) {
        //        var levels = Levels
        //                .Where(lev => predefined.Contains(lev.ShapeID))
        //                .ToDictionary(
        //                    lev => lev.ShapeID,
        //                    lev => lev.Value.Value.ToString("g")); 
        //        return levels; 
        //    } else {
        //        var levels = Levels
        //                .ToDictionary(
        //                    lev => lev.ShapeID,
        //                    lev => lev.Value.Value.ToString("g"));
        //        return levels; 
        //    }
        //}

        //public IILLegendItemDataProvider GetEntry(int id) {
        //    return First<ILContourLine>(predicate: g => g is IILLegendItemDataProvider && g.ID == id) as IILLegendItemDataProvider;
        //}
        #endregion
    }
}
