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
using System.Drawing;
using System.Xml.Serialization;

namespace ILNumerics.Drawing.Plotting {
    /// <summary>
    /// A class used to visualize 1-,2- and 3 dimensional data as line plots with markers
    /// </summary>
    [Serializable]
    public class ILLinePlot : ILGroup, IILPositionProvider, IILLegendItemDataProvider {

        #region attributes
        /// <summary>
        /// Tag identifying line plots within the scene graph
        /// </summary>
        public static readonly string LinePlotTag = "LinePlot";
        /// <summary>
        /// Tag identifying markers for line plots within the scene graph
        /// </summary>
        public static readonly string MarkerTag = "Marker";
        /// <summary>
        /// Tag identifying individual lines within the line plot
        /// </summary>
        public static readonly string LineTag = "Line";
        /// <summary>
        /// Color enumerator used to color subsequent lines 
        /// </summary>
        public static ILColorEnumerator NextColors = new ILColorEnumerator(); 
        #endregion

        #region properties
        /// <summary>
        /// Gets access to the line of the line plot 
        /// </summary>
        [XmlIgnore]
        public ILLineStrip Line { get { return First<ILLineStrip>(LineTag); } }
        /// <summary>
        /// Gets access to the marker of the line plot 
        /// </summary>
        [XmlIgnore]
        public ILMarker Marker { get { return First<ILMarker>(MarkerTag); } }
        #endregion

        #region ctors
        internal ILLinePlot(ILLinePlot source)
            : base(source) { }
        internal ILLinePlot() { }
        /// <summary>
        /// Creates a new line plot
        /// </summary>
        /// <param name="positions">1, 2 or 3D positions as column vectors</param>
        /// <param name="tag">[optionial] tag identifying the plot in the scene</param>
        /// <param name="lineColor">[optional] color of the line, default: auto</param>
        /// <param name="lineStyle">[optional] line style, default: solid</param>
        /// <param name="lineWidth">[optional] line width, default: 1px</param>
        /// <param name="markerColor">[optional], color for markers, default: auto</param>
        /// <param name="markerStyle">[optional], marker style, default: none</param>
        public ILLinePlot(ILArray<float> positions, object tag = null, 
                          Color? lineColor = null, DashStyle lineStyle = DashStyle.Solid, int lineWidth = 1, 
                          Color? markerColor = null, MarkerStyle markerStyle = MarkerStyle.None)
        : base(tag ?? LinePlotTag) {
            using (ILScope.Enter(positions)) {
                Add(new ILLineStrip(LineTag));
                if (positions.S[0] == 3) {
                    Line.Positions.Update(positions);
                } else {
                    ILArray<float> data = ILMath.zeros<float>(3,positions.S[1]);
                    if (positions.S[0] == 1) {
                        // auto X
                        data[0, ILMath.full] = ILMath.counter<float>(0,1,1,positions.S[1]);
                        data[1, ILMath.full] = positions;
                    } else {
                        data[ILMath.r(0, positions.S[0] - 1), ILMath.full] = positions;
                    }
                    Line.Positions.Update(data); 
                }
                Line.Color = lineColor ?? NextColors.NextColor();
                Line.Width = lineWidth; 
                Line.AutoNormals = false;
                Line.Normals.Update(null); 
                Line.DashStyle = lineStyle; 
                Add(new ILMarker(style: markerStyle, color: markerColor, tag: MarkerTag)); 
            }
        }
        #endregion 

        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILLinePlot();
        }

        internal override ILNode Copy() {
            return new ILLinePlot(this);
        }

        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILLinePlot ret = (ILLinePlot)base.Synchronize(copy, syncParams);
            //if (copy == null) {
            //    ret.Line = ret.FindById < ILLineStrip>(Line.ID);
            //}
            //ret.Marker = (ILMarker)Marker.Synchronize(ret.Marker,syncParams); 
            return ret; 
        }

        #region IILPositionProvider Members
        /// <summary>
        /// Provides positions of line vertices
        /// </summary>
        /// <remarks>This function is used by markers for rendering</remarks>
        [XmlIgnore]
        public ILRetArray<float> Positions {
            get {
                var pcdg = Parent;
                while (pcdg.Parent != null && !(pcdg is ILPlotCubeDataGroup)) {
                    pcdg = pcdg.Parent;
                }
                if (pcdg != null && pcdg is ILPlotCubeDataGroup) {
                    using (ILScope.Enter()) {
                        // acquire log state 
                        var ls = (pcdg as ILPlotCubeDataGroup).ScaleModes;
                        ILArray<float> ret = Line.Positions.Storage;
                        if (ls.XAxisScale == AxisScale.Logarithmic) ret["0;:"] = ILMath.log10(ret["0;:"]);
                        if (ls.YAxisScale == AxisScale.Logarithmic) ret["1;:"] = ILMath.log10(ret["1;:"]);
                        if (ls.ZAxisScale == AxisScale.Logarithmic) ret["2;:"] = ILMath.log10(ret["2;:"]);
                        return ret;
                    }
                } else {
                    return Line.Positions.Storage;
                }
            }
        }
        /// <summary>
        /// Provides indices of line vertices
        /// </summary>
        /// <remarks>This function is used by markers for rendering</remarks>
        [XmlIgnore]
        public ILRetArray<int> Indices {
            get { return Line.Indices.Storage; }
        }

        #endregion

        #region IILLegendItemDataProvider Members
        /// <summary>
        /// Creates visual representation for the line
        /// </summary>
        /// <param name="renderArea">Root group node for visual output; must provide coord system [0,0,0] -> [1,1,1]</param>
        /// <remarks>This function is used by legends for rendering</remarks>
        public void ConfigureLegendVisual(ILGroup renderArea) {
            if (renderArea != null) {
                // has this been drawn already? Remove existing nodes. Limit to MY nodes only.
                var toClear = renderArea.Find<ILNode>("LegendItem"); 
                foreach (var clearNode in toClear) renderArea.Children.Remove(clearNode); 

                ILLineStrip legendLine = renderArea.Add(Line, "LegendItem"); 
                legendLine.Detach(); 
                legendLine.PickingID = Line.PickingID;
                legendLine.Positions.Update(new float[,] { 
                    {0.0f,0.5f,0}, 
                    {0.5f,0.5f,0}, 
                    {0.75f,0.5f,0}
                }); 
                legendLine.Indices.Update(null);
                legendLine.Configure(false,false); 
                // the marker is pushed up slightly to make it appear _on top_ of the line 
                var legendMarker = renderArea.Add(new ILGroup(translate: new Vector3(0, 0, 0.1f)))
                                             .Add(Marker, "LegendItem"); 
                legendMarker.Fill.PickingID = Marker.Fill.PickingID; 
                legendMarker.Border.PickingID = Marker.Border.PickingID; 
                legendMarker.Configure(false,false); 
                legendMarker.m_renderTarget = RenderTarget.Screen2DNear; 
            }
        }
        /// <summary>
        /// Creates textual / label representation for the line
        /// </summary>
        /// <param name="renderArea">Root group node for visual output; must provide coord system [0,0,0] -> [1,1,1]</param>
        /// <remarks>This function is used by legends for rendering</remarks>
        public void ConfigureLegendLabel(ILGroup renderArea) {
            if (renderArea != null) {
                ILLabel label = renderArea.First<ILLabel>();
                if (label != null) {
                    label.Text = (Tag != null) ? Tag.ToString() : "Line " + ID.ToString(); // ToString() 
                } else {
                    // has this been drawn already? Remove existing nodes. Limit to MY nodes only.
                    var toClear = renderArea.Find<ILNode>("LegendItem");
                    foreach (var clearNode in toClear) renderArea.Children.Remove(clearNode);
                    renderArea.Add(new ILLabel(text: (Tag != null) ? Tag.ToString() : "Line " + ID.ToString()), "LegendItem");
                }
            }
        }
        /// <summary>
        /// Gets the Id of the line plot
        /// </summary>
        /// <returns></returns>
        public int GetID() {
            return ID; 
        }
        /// <summary>
        /// Gets the modification version of the line plot 
        /// </summary>
        /// <returns></returns>
        public long GetVersion() {
            return Version; 
        }

        #endregion
        /// <summary>
        /// Creates a new X-line plot for every row in A
        /// </summary>
        /// <param name="A">data matrix with individual line plot data in rows</param>
        /// <param name="lineColors">[optional] defines colors for lines, plots without corresponding elements get default colors</param>
        /// <param name="lineStyles">[optional] defines dash styles, plots without corresponding elements are drawn as solid line</param>
        /// <param name="lineWidth">[optional] defines line width, plots without corresponding elements are drawn as 1px lines</param>
        /// <param name="markers">[optional] defines marker styles, plots without corresponding elements are drawn without markers</param>
        /// <returns>Group node containing all line plots created</returns>
        /// <remarks>The group node returned contains one line plot for every row in A. 
        /// Every line plot gets a tag according to the following naming scheme: 'LinePlotXXXX', 
        /// where XXXX corresponds to the row index in A.</remarks>
        public static ILGroup CreateXPlots(ILInArray<float> A, IEnumerable<Color> lineColors = null, 
                                            IEnumerable<DashStyle> lineStyles = null, 
                                            IEnumerable<int> lineWidth = null, 
                                            IEnumerable<MarkerStyle> markers = null) {
            ILGroup ret = new ILGroup(); 
            using (ILScope.Enter(A)) {
                if (ILMath.isnull(A)) return ret; 
                for (int i = 0; i < A.S[0]; i++) {
                    var plot = ret.Add(new ILLinePlot(
                                A[i,":"],
                                tag: "LinePlot" + i.ToString("d4")));

                    if (lineColors != null && lineColors.Count() > i) {
                        plot.Line.Color = lineColors.ElementAt(i);
                    }
                    if (lineStyles != null && lineStyles.Count() > i) {
                        plot.Line.DashStyle = lineStyles.ElementAt(i);
                    }
                    if (lineWidth != null && lineWidth.Count() > i) {
                        plot.Line.Width = lineWidth.ElementAt(i);
                    }
                    if (markers != null && markers.Count() > i) {
                        plot.Marker.Style = markers.ElementAt(i);
                    }
                }
                return ret;
            }
        }
    }
}
