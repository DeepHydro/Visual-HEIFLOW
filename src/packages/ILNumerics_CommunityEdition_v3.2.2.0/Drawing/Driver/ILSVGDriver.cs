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
using System.IO;
using System.Drawing; 
using System.Globalization; 
using ILNumerics.Data; 

namespace ILNumerics.Drawing {
    public class ILSVGDriver : ILDriver {

        #region attributes
        TextWriter m_writer; 
        Size m_size; 
        CultureInfo m_culture = CultureInfo.GetCultureInfo("en-us"); // decimal POINTS required by SVG  
        bool m_performZBuffering = false; 
        int[] m_tempBuffer = new int[512]; 
        ILTextureStorage m_textureStorage; 
        StringBuilder m_sbuilder;
        public static float COLOR_TOLERANCE = 0.05f; 
        public static float TRIANGLE_EXPANSION_FACTOR = .5f;  // expand triangles by so many pixels in each direction; reasonable default: half pixel
        List<BSPPrimitive<ILDrawable>> m_3dPrimitives = new List<BSPPrimitive<ILDrawable>>(1000);
        List<BSPPrimitive<ILDrawable>> m_2dPrimitivesFar = new List<BSPPrimitive<ILDrawable>>(500);
        List<BSPPrimitive<ILDrawable>> m_2dPrimitivesNear = new List<BSPPrimitive<ILDrawable>>(500); 
        List<BSPPrimitive<ILDrawable>> m_currentPrimitiveCollection; 
        #endregion

        #region constructors
        /// <summary>
        /// Create SVG driver 
        /// </summary>
        /// <param name="outStream">output stream to write SVG data into</param>
        /// <param name="width">the SVG artwork width</param>
        /// <param name="height">the SVG artwork height</param>
        /// <param name="scene">[optional] the scene to render. If null: a new scene will be created</param>
        /// <param name="BackColor">[optional] background color. On null: System.Drawing.Color.White is used.</param>
        public ILSVGDriver(Stream outStream, int width = 1200, int height = 800, ILScene scene = null, Color? BackColor = null) 
            : base(scene ?? new ILScene()) {
                m_writer = new StreamWriter(outStream);
            m_size = new Size(width, height);
            m_textureStorage = new ILDummyTextureStorage(); 
            m_sbuilder = new StringBuilder();
            this.BackColor = BackColor ?? Color.White; 
        }
        //public ILSVGDriver(string filename, ILScene scene = null)
        //    : base(scene ?? new ILScene()) {
        //    m_stream = new FileStream(filename, FileMode.Create);  
        //}
        #endregion

        #region properties
        public override bool IsDisposed {
            get { return false; }
        }
        public override System.Drawing.Size Size {
            get { return m_size; }
            set { m_size = value; }
        }
        public override RendererTypes Driver {
            get { return RendererTypes.SVG; }
        }
        #endregion

        #region driver implementation 
        protected override void BeginRender(ILRenderParameter renderParams) {
            Scene.Configure(); 
            base.BeginRender(renderParams);
            m_currentPrimitiveCollection = m_3dPrimitives; 
            m_writer.WriteLine(@"<?xml version='1.0' encoding='UTF-8'?>
<!DOCTYPE svg PUBLIC '-//W3C//DTD SVG 1.1//EN' 'http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd'>
 
<svg xmlns='http://www.w3.org/2000/svg'
     xmlns:xlink='http://www.w3.org/1999/xlink' xmlns:ev='http://www.w3.org/2001/xml-events'
     version='1.1' baseProfile='full'
     width='" + Size.Width + @"' height='" + Size.Height + @"'>
<rect x='0' y='0' width='" + Size.Width + @"' height='" + Size.Height + @"' fill='" + ColorToHtml(BackColor) + @"'/> 
"); 
        }
        protected override void BeginDrawScreen2D(ILRenderParameter renderParams) {
 	        base.BeginDrawScreen2D(renderParams);
            if (renderParams.CurrentPassCount < 5) {
                m_currentPrimitiveCollection = m_2dPrimitivesFar;
            } else {
                m_currentPrimitiveCollection = m_2dPrimitivesNear;
            }
        }
        
        protected override void RenderPoints(ILPoints points, ILRenderParameter parameters) {
            foreach (var point in points.GetSortedAndClippedScreen(parameters, SortingMode.None, false)) {
                BSPPrimitive<ILDrawable> primitive = new BSPPrimitive<ILDrawable>(point, points);
                m_currentPrimitiveCollection.Add(primitive);
            }
        }
        protected override void RenderLines(ILLines lines, ILRenderParameter parameters) {
            float dashOffset = 0; 
            foreach (var line in lines.GetSortedandClippedScreen(parameters, SortingMode.None, true)) {
                BSPPrimitive<ILDrawable> primitive = new BSPPrimitive<ILDrawable>(line, lines);
                //primitive.Positions[0].Z -= ILLines.PolygonOffset;
                if (lines.DashStyle != DashStyle.Solid) {
                    float dx = line.P1.X - line.P2.X, dy = line.P1.Y - line.P2.Y;
                    primitive.LineOffset = dashOffset; 
                    dashOffset += (float)Math.Sqrt(dx * dx + dy * dy);
                }
                m_currentPrimitiveCollection.Add(primitive);
            }
        }
        protected override void RenderTriangles(ILTriangles triangles, ILRenderParameter parameters) {
            foreach (var tri in triangles.GetSortedAndClippedScreen(parameters, SortingMode.None, true)) {
                BSPPrimitive<ILDrawable> primitive = new BSPPrimitive<ILDrawable>(tri, triangles); 
                m_currentPrimitiveCollection.Add(primitive); 
            }
        }
        protected override void RenderText(ILLabel label, ILRenderParameter parameters) {
            Vector3 position = parameters.ToScreen(label.Position); 
            BSPPrimitive<ILDrawable> primitive = new BSPPrimitive<ILDrawable>(label, label, position); 
            m_currentPrimitiveCollection.Add(primitive); 
        }

        protected override void EndRender(ILRenderParameter renderParams) {
            base.EndRender(renderParams);

            RenderPrimitivesList(m_3dPrimitives, renderParams);
            RenderPrimitivesList(m_2dPrimitivesFar, renderParams);
            RenderPrimitivesList(m_2dPrimitivesNear, renderParams);

            m_writer.Write(m_sbuilder.ToString()); 
            m_writer.Write("</svg>"); 
            m_writer.Flush(); 
        }
        #endregion

        #region private helpers
        private void RenderPrimitivesList(List<BSPPrimitive<ILDrawable>> list, ILRenderParameter renderParams) {
            ILBSPTree<ILDrawable> tree = new ILBSPTree<ILDrawable>(list, new ILBSPTreeSettings {
                BalanceSplitRatio = 1  // prefer fewer splits over a balanced tree
            });
            // camera position in screen coords for sorting the BSP tree
            Vector3 camScreen = ViewTransform * new Vector3(0,0,-1000);  // todo: FIX: somehow ILPlotCube translates its content to 0..-1 in screen coords!
            foreach (var p in tree.GetSorted(camScreen, true)) {
                switch (p.VertexCount) {
                    case 1:
                        if (p.Data is ILLabel) {
                            RenderTextSVGFringe(p, renderParams);
                        } else {
                            RenderPointSVG(p, renderParams);
                        }

                        // this must be a point (labels are handled seperately)
                        break;
                    case 2:
                        // render line
                        RenderLineSVG(p, renderParams); 
                        break;
                    case 3:
                        RenderTriangleSVG(renderParams, m_sbuilder, p.GetTriangle(), p.Data as ILTriangles);
                        break;
                }
            }
        }
        /// <summary>
        /// compute ligthing and do recursive subdivisioning for a single triangle for rendering
        /// </summary>
        /// <param name="parameters">current render parameters</param>
        /// <param name="sb">string builder for writing output to</param>
        /// <param name="tri">triangle</param>
        /// <param name="triangles">original triangles shape (for color computations)</param>
        private void RenderTriangleSVG(ILRenderParameter parameters, StringBuilder sb, Triangle tri, ILTriangles triangles) {
            Vector4 outCol1 = tri.C1, outCol2 = tri.C2, outCol3 = tri.C3;

            #region compute Light
            if (triangles.Normals.DataCount > 2) {
                outCol1 = ComputeLight(tri.C1, tri.PCam1.Xyz, tri.N1, triangles.Shininess
                            , triangles.SpecularColor, triangles.EmissionColor, parameters);
                outCol2 = ComputeLight(tri.C2, tri.PCam1.Xyz, tri.N2, triangles.Shininess
                            , triangles.SpecularColor, triangles.EmissionColor, parameters);
                outCol3 = ComputeLight(tri.C3, tri.PCam1.Xyz, tri.N3, triangles.Shininess
                            , triangles.SpecularColor, triangles.EmissionColor, parameters);
            }
            #endregion

            if (aboveColorThreshold(outCol1, outCol2, outCol3) && triangleIsDivisible(ref tri.P1, ref tri.P2, ref tri.P3)) {
                // subdivisioning
                Triangle tr1, tr2, tr3, tr4;
                tri.Subdivide(out tr1, out tr2, out tr3, out tr4);
                RenderTriangleSVG(parameters, sb, tr1, triangles);
                RenderTriangleSVG(parameters, sb, tr2, triangles);
                RenderTriangleSVG(parameters, sb, tr3, triangles);
                RenderTriangleSVG(parameters, sb, tr4, triangles);
            } else {
                // expand the triangle slightly to work around the svg aliasing problem (thin "back bleeding" - lines between connected polygons)
                Vector3 cent = (tri.P1 + tri.P2 + tri.P3) / 3;

                //Vector3 sc = sc1 - cent; sc1 = cent + sc / sc.LengthFast * TRIANGLE_EXPANSION_FACTOR * sc.LengthFast;  // <- empirical value
                //sc = sc2 - cent; sc2 = cent + sc / sc.LengthFast * TRIANGLE_EXPANSION_FACTOR * sc.LengthFast;  // <- empirical value
                //sc = sc3 - cent; sc3 = cent + sc / sc.LengthFast * TRIANGLE_EXPANSION_FACTOR * sc.LengthFast;  // <- empirical value
                float len;
                Vector3 sc = tri.P1 - cent; len = sc.Length; tri.P1 = cent + sc / len * (TRIANGLE_EXPANSION_FACTOR + len);  // <- empirical value
                sc = tri.P2 - cent; len = sc.Length; tri.P2 = cent + sc / len * (TRIANGLE_EXPANSION_FACTOR + len);  // <- empirical value
                sc = tri.P3 - cent; len = sc.Length; tri.P3 = cent + sc / len * (TRIANGLE_EXPANSION_FACTOR + len);  // <- empirical value
                // SVG does not handle barycentric interpolation.... 
                Vector4 col = (outCol1 + outCol2 + outCol3) / 3;
                sb.Append(String.Format("<polygon fill='{0}'{7} points='{1} {2} {3} {4} {5} {6}' shape-rendering='crispEdges' />"
                    , ColorToHtml(col.ToColor())
                    , print(tri.P1.X), print(tri.P1.Y)
                    , print(tri.P2.X), print(tri.P2.Y)
                    , print(tri.P3.X), print(tri.P3.Y)
                    , col.W < 1 ? " fill-opacity='" + print(col.W) + "'" : ""));
            }
        }
        private void RenderLineSVG(BSPPrimitive<ILDrawable> primitive, ILRenderParameter parameters) {
            ILLines lines = primitive.Data as ILLines; 
            string styleString = string.Empty;
            //prepareStyle(lines, parameters, out styleString, out singleColor, out useAlpha);
            // opacity
            float w = (primitive.Colors[0].W + primitive.Colors[1].W) / 2f; 
            if (w < 1) {
                styleString += @" opacity='" + print(w) + "'"; 
            }
            // dash style
            if (lines.DashStyle != DashStyle.Solid) {
                var stipple = StippleFromLineStyle(lines);
                short pattern = stipple.Pattern;
                float stippleFact = stipple.Factor * 4;
                var array = Stipple2StrokeDash(pattern, stippleFact);
                styleString += String.Format(" stroke-dasharray='{0}'", String.Join(",", array.Select(a => a.ToString(m_culture))));
                if (primitive.LineOffset != 0) {
                    styleString += @" stroke-dashoffset='" + print(primitive.LineOffset, 4) + "'"; 
                }
            }
            styleString += " stroke='" + ColorToHtml(((primitive.Colors[0] + primitive.Colors[1]) / 2f).ToColor()) + "'";
            if (lines.Width != 1) {
                styleString += " stroke-width='" + lines.Width.ToString() + "'"; 
            }
            write("<line x1='{0}' y1='{1}' x2='{2}' y2='{3}' {4}/>"
                , print(primitive.Positions[0].X), print(primitive.Positions[0].Y)
                , print(primitive.Positions[1].X), print(primitive.Positions[1].Y)
                , styleString);
        }
        private void RenderPointSVG(BSPPrimitive<ILDrawable> primitive, ILRenderParameter parameters) {
            ILPoints pointShape = primitive.Data as ILPoints;
            float w = pointShape.Size;
            float w2 = (w + 1) / 2f;
            bool colAll = false;
            string styleString = string.Empty;
            if (pointShape.Color.HasValue) {
                styleString += " fill='" + ColorToHtml(pointShape.Color.GetValueOrDefault()) + "'";
                if (pointShape.Color.GetValueOrDefault().A < 255) {
                    styleString += " opacity='" + (pointShape.Color.GetValueOrDefault().A / 255f).ToString(m_culture) + "'";
                }
                colAll = true;
            }
            string r = String.Format(" r='{0}'", print(pointShape.Size / 2f)); // Radius! not Size is diameter!
            Vector3 p1 = primitive.Positions[0];
            // frustum clipping
            if (p1.X <= -w2 || p1.Y <= -w2 || p1.X >= Size.Width + w2 - 1 || p1.Y >= Size.Height + w2 - 1) return;
            if (colAll) {
                write("<circle cx='{0}' cy='{1}'" + r + styleString + "/>",
                        print(p1.X), print(p1.Y));
            } else {
                write("<circle cx='{0}' cy='{1}' fill='{2}' {3}" + r + styleString + "/>"
                    , print(p1.X), print(p1.Y)
                    , ColorToHtml(primitive.Colors[0].ToColor())
                    , (primitive.Colors[0].W < 1) ? "opacity='" + print(primitive.Colors[0].W) + "' " : "");
            }
        }
        private void RenderTextSVGFringe(BSPPrimitive<ILDrawable> primitive, ILRenderParameter parameters) {
            ILLabel label = primitive.Data as ILLabel;
            if (!String.IsNullOrWhiteSpace(label.Text) && !label.Position.IsEmtpy()) {
                ILRenderQueue queue = label.Interpreter.Transform(label, m_textureStorage);
                PointF rotationPos = primitive.Positions[0].ToPointF();
                PointF anchor = new PointF(rotationPos.X - label.Anchor.X * queue.Size.Width,
                                           rotationPos.Y - label.Anchor.Y * queue.Size.Height + label.Font.Height);
                if (label.Fringe.Width > 0) {
                    for (int c = -label.Fringe.Width; c < label.Fringe.Width + 1; c++) {
                        for (int r = -label.Fringe.Width; r < label.Fringe.Width + 1; r++) {
                            if (c == 0 && r == 0) continue;
                            PointF p = new PointF(anchor.X + c, anchor.Y + r);
                            renderTextSVGsingle(label, label.Fringe.Color, queue, p, rotationPos, true);
                        }
                    }
                }
                Color labelColor = label.Color ?? Color.Black;
                if (parameters.ColorOverride.Peek().HasValue) {
                    labelColor = parameters.ColorOverride.Peek().GetValueOrDefault();
                }
                renderTextSVGsingle(label, labelColor, queue, anchor, rotationPos);
            }
        }
        private void renderTextSVGsingle(ILLabel label, Color labelColor, ILRenderQueue queue,
                                         PointF baseRenderPos, PointF rotationPos, bool forceColor = false) {
            write("<text x='{0}' y='{1}'",
                            print(baseRenderPos.X),
                            print(baseRenderPos.Y));

            writeSVGFontSpec(null, label.Font);
            writeSVGColorSpec(Color.Black, labelColor);
            // IE9 does not recognize xml:space=preserve .. :( 
            //if (label.Text.Contains(' ')) 
            //    sb.Append(" xml:space='preserve'"); 

            #region setup rotation
            if (Math.Abs(label.Rotation) > ILMath.epsf) {
                write(" transform='rotate({0} {1} {2})'"
                                , print((float)label.Rotation / ILMath.pif * 180f)
                                , print(rotationPos.X), print(rotationPos.Y));
            }
            #endregion
            write(">");
            Font baseFont = label.Font;
            string baseStyle = ILHashCreator.Hash(" ", label.Font);
            baseStyle = baseStyle.Substring(0, baseStyle.LastIndexOf('&'));
            Color baseColor = labelColor;
            bool inSpan = false;
            float baseLine = 0;
            float sVGxPos = 0;
            //float myxPos = 0; 
            foreach (ILRenderQueueItem item in queue) {
                #region
                // collect whitespace
                if (item.Text.All(c => char.IsWhiteSpace(c))) { // no whitespace in svg! 
                    continue;
                }
                Font font;
                ILHashCreator.Parse(item.Key, out font);
                if (item.Key.Substring(0, item.Key.LastIndexOf('&')) != baseStyle
                    || (!forceColor && baseColor != item.Color)
                    || baseLine != item.Rect.Y
                    || Math.Abs(item.Rect.X - sVGxPos) > 0.00001) {
                    if (inSpan) {
                        write("</tspan>");
                    }
                    // start new tspan
                    write("<tspan");
                    writeSVGFontSpec(label.Font, font);
                    if (!forceColor)
                        writeSVGColorSpec(labelColor, item.Color);

                    if (baseLine != item.Rect.Y) {
                        write(String.Format(" dy='{0}px'", print(item.Rect.Y - baseLine)));
                        baseLine = item.Rect.Y;
                    }
                    if (Math.Abs(item.Rect.X - sVGxPos) > 0.001) {
                        write(String.Format(" x='{0}px'", print(baseRenderPos.X + item.Rect.X)));
                        sVGxPos = item.Rect.X;
                    }
                    write(">");
                    baseFont = (Font)font.Clone();
                    baseColor = item.Color;
                    baseStyle = item.Key.Substring(0, item.Key.LastIndexOf('&'));
                    inSpan = true;
                }
                write(item.Text);
                sVGxPos += item.Rect.Width;
                font.Dispose();
                #endregion
            }
            if (inSpan) {
                write("</tspan>");
            }
            write("</text>");
        }
        private void writeSVGColorSpec(Color baseColor, Color curColor) {
            if (baseColor != curColor) {
                write(" fill='{0}'", ColorToHtml(curColor));
            }
        }
        private void writeSVGFontSpec(Font baseFont, Font curFont) {
            if ((baseFont == null && curFont.Bold) || (baseFont != null && curFont.Bold != baseFont.Bold)) {
                write(" font-weight='{0}'", curFont.Bold ? "bold" : "normal");
            }
            if (baseFont == null || curFont.Name != baseFont.Name) {
                string f = curFont.Name;
                if (curFont.OriginalFontName != null && curFont.OriginalFontName != curFont.Name) {
                    f = curFont.OriginalFontName + "," + f;
                }
                write(" font-family='" + f + "'");
            }
            if (baseFont == null || curFont.Height != baseFont.Height) {
                write(" font-size='{0}px'", print(curFont.Height * 0.94f));
            }
            if ((baseFont == null && curFont.Italic) || (baseFont != null && curFont.Italic != baseFont.Italic)) {
                write(" font-style='{0}'", curFont.Italic ? "italic" : "normal");
            }
        }
        private string print(float val, int prec = 2) {
            return val.ToString("f" + prec.ToString(), m_culture);
        }
        private bool triangleIsDivisible(ref Vector3 sc1, ref Vector3 sc2, ref Vector3 sc3) {
            return (sc1 - sc2).LengthSquared > 4
                && (sc1 - sc3).LengthSquared > 4
                && (sc2 - sc3).LengthSquared > 4;
        }
        private bool aboveColorThreshold(Vector4 c1, Vector4 c2, Vector4 c3) {
            Vector4 mid = (c1 + c2 + c3) / 3;
            Vector4 d = c1 - mid;
            if (Math.Abs(d.X) > COLOR_TOLERANCE || Math.Abs(d.Y) > COLOR_TOLERANCE || Math.Abs(d.Z) > COLOR_TOLERANCE || Math.Abs(d.W) > COLOR_TOLERANCE) {
                return true;
            }
            d = c2 - mid;
            if (Math.Abs(d.X) > COLOR_TOLERANCE || Math.Abs(d.Y) > COLOR_TOLERANCE || Math.Abs(d.Z) > COLOR_TOLERANCE || Math.Abs(d.W) > COLOR_TOLERANCE) {
                return true;
            }
            d = c3 - mid;
            if (Math.Abs(d.X) > COLOR_TOLERANCE || Math.Abs(d.Y) > COLOR_TOLERANCE || Math.Abs(d.Z) > COLOR_TOLERANCE || Math.Abs(d.W) > COLOR_TOLERANCE) {
                return true;
            }
            return false;
        }
        private void prepareStyle(ILLines lines, ILRenderParameter parameters, out string styleString, out bool singleColor, out bool useAlpha) {
            styleString = "";
            singleColor = false;
            useAlpha = lines.IsTransparent;

            if (lines.Width != 1) {
                styleString += String.Format(@" stroke-width='{0}'", lines.Width);
            }

            if (lines.DashStyle != DashStyle.Solid) {
                var stipple = StippleFromLineStyle(lines);
                short pattern = stipple.Pattern;
                float stippleFact = stipple.Factor * 4;
                var array = Stipple2StrokeDash(pattern, stippleFact);
                styleString += String.Format(" stroke-dasharray='{0}'", String.Join(",", array.Select(a => a.ToString(m_culture))));
            }
        }
        private List<float> Stipple2StrokeDash(short pattern, float stippleFact) {
            int i = 16;
            int on = 0, off = 0;
            List<float> ret = new List<float>();
            while (i > 0) {
                while (i > 0 && (pattern & 01) == 1) { on++; pattern = (short)(pattern >> 1); i--; }
                while (i > 0 && (pattern & 01) == 0) { off++; pattern = (short)(pattern >> 1); i--; }
                if (on > 0) {
                    ret.Add(on * stippleFact);
                    on = 0;
                }
                if (off > 0) {
                    ret.Add(off * stippleFact);
                    off = 0;
                }
            }
            return ret;
        }
        private string ColorToHtml(Color color) {
            return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }
        private void write(string format, params object[] parameters) {
            m_sbuilder.Append(String.Format(format, parameters)); 
        }
        #endregion

    }
}
