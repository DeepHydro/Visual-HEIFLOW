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

namespace ILNumerics.Drawing.Plotting {
    [Serializable]
    public class ILContourLine : ILGroup, IILLegendItemDataProvider {

        #region properties
        /// <summary>
        /// Gets access to the line object of the contour line
        /// </summary>
        [XmlIgnore]
        public ILLineStrip Line {
            get {
                return First<ILLineStrip>();
            }
        }
        /// <summary>
        /// Gets access to the label object of the contour line
        /// </summary>
        [XmlIgnore]
        public ILLabel Label {
            get {
                return First<ILLabel>();
            }
        }
        /// <summary>
        /// Determines if the label for the contour line is rendered as Screen2D or as 3D object 
        /// </summary>
        [XmlIgnore]
        public RenderTarget? LabelTarget {
            get {
                if (Label != null) {
                    return Label.Parent.m_renderTarget; 
                } 
                return null; 
            }
            set {
                if (Label != null) {
                    Label.Parent.m_renderTarget = value; 
                }
            }
        }
        /// <summary>
        /// Determines if the labels for the contour line is to be shown
        /// </summary>
        [XmlIgnore]
        public bool ShowLabel {
            get {
                return Label.Parent.Visible; 
            }
            set {
                Label.Parent.Visible = value; 
            }
        }
        #endregion

        #region ctor
        /// <summary>
        /// Create a new countour line 
        /// </summary>
        /// <param name="tag">[optional] tag identifying the object in the scene graph</param>
        public ILContourLine(object tag = null)
            : base(tag) {
            Add(new ILLineStrip());
            Add(new ILGroup { 
                new ILLabel { Visible = false }
            }); 
        }
        internal ILContourLine(ILContourLine source)
            : base(source) {
        }
        protected ILContourLine() { }
        #endregion

        #region public interface 
        internal override ILNode Copy() {
            return new ILContourLine(this);
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILContourLine();
        }
        protected override bool BeginVisit(ILRenderParameter parameter) {
            // configure label 
            if (Line != null && Label != null && parameter.CurrentPassCount == 0) {
                if (Line.Positions.DataCount > 1) {
                    // collect positions until label width exceeded
                    int ind = Math.Min(Line.Positions.DataCount / 2, Line.Positions.DataCount - 2); 
                    SizeF labSize = Label.MeasureSize();
                    int start = ind; 
                    Vector3 p1 = Line.Positions.GetPositionAt(ind);
                    Vector3 p2 = Line.Positions.GetPositionAt(ind + 1); 
                    parameter.ToScreen(ref p1, ref p2);
                    while ((p2 - p1).LengthFast < labSize.Width && ind < Line.Positions.DataCount - 2) {
                        ind++;
                        p2 = Line.Positions.GetPositionAt(ind);
                        p2 = parameter.ToScreen(p2);
                    }
                    if ((p2 - p1).LengthFast >= labSize.Width) {
                        Label.Position = Line.Positions.GetPositionAt(start);
                        Label.Anchor = new PointF(0, 0.5f);
                        p1 = (p2 - p1);
                        p1.NormalizeFast();
                        Label.Rotation = Math.Atan2(p1.Y,p1.X);
                        if (Math.Abs(Label.Rotation) > ILMath.pif / 2f) {
                            Label.Rotation += ILMath.pif; 
                            Label.Anchor = new PointF(1,0.5f); 
                        }
                        Label.Visible = true;
                    } else {
                        Label.Visible = false;
                    }
                }
            }

            return base.BeginVisit(parameter);
        }
        #endregion

        #region IILLegendItemDataProvider Members
        /// <summary>
        /// Renders a visual representation of this countour line into a legend 
        /// </summary>
        /// <param name="renderArea">Root group for output</param>
        /// <remarks>The <paramref name="renderArea"/> is expected to provide a coordinate system in range [0,0,0] -> [1,1,1]. 
        /// The function may adds and configures new objects to the group. Properties of the group will not get altered.</remarks>
        public void ConfigureLegendVisual(ILGroup renderArea) {
            if (renderArea != null && Line != null) {
                renderArea.Children.Clear();
                ILLineStrip legendLine = renderArea.Add(Line);
                legendLine.Detach();
                legendLine.Positions.Update(new float[,] { 
                    {0.0f,0.5f,0}, 
                    {0.5f,0.5f,0}, 
                    {0.75f,0.5f,0}
                });
                legendLine.Indices.Update(null);
                renderArea.Configure(true,false); 
            }
        }
        /// <summary>
        /// Renders a textual representation / label of this countour line into a legend 
        /// </summary>
        /// <param name="renderArea">Root group for output</param>
        /// <remarks>The <paramref name="renderArea"/> is expected to provide a coordinate system in range [0,0,0] -> [1,1,1]. 
        /// The function may adds and configures new objects to the group. Properties of the group will not get altered.</remarks>
        public void ConfigureLegendLabel(ILGroup renderArea) {
            if (renderArea != null && Label != null) {
                ILLabel label = renderArea.First<ILLabel>();
                if (label == null) {
                    //renderArea.Childs.Clear();
                    label = renderArea.Add(new ILLabel());
                }
                label.Text = Label.Text;
                label.Visible = true; 
            }
        }
        /// <summary>
        /// Gets an ID for the contour line; used to identify the line among objects in the scene graph
        /// </summary>
        /// <returns></returns>
        public int GetID() {
            return ID;
        }
        /// <summary>
        /// Gets the current version for the contour line
        /// </summary>
        /// <returns>Current version</returns>
        public long GetVersion() {
            return Version; 
        }

        #endregion
    }
}
