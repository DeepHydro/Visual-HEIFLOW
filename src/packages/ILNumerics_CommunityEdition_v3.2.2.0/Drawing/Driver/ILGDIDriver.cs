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
using System.Drawing.Drawing2D; 
using System.Drawing.Imaging; 

namespace ILNumerics.Drawing {
    public sealed partial class ILGDIDriver : ILDriver, IDisposable {

        #region attributes
        ILTextureStorage m_textureStorage; 
        ILBackBuffer m_backBuffer;
        #endregion

        #region properties
        public override bool IsDisposed {
            get { return m_backBuffer.ZBuffer.IsDisposed; }
        }
        public override Size Size {
            get {
                return m_backBuffer.Rectangle.Size; 
            }
            set {
                if (m_backBuffer.Rectangle.Size != value) {
                    m_backBuffer.Rectangle = new Rectangle(0,0,value.Width,value.Height); 
                }
            }
        }
        public override RendererTypes Driver {
            get { return RendererTypes.GDI; }
        }
        public ILBackBuffer BackBuffer {
            get { return m_backBuffer; }
        }
        #endregion

        #region constructors
        public ILGDIDriver(int width = 1200, int height = 800, ILScene scene = null, Color? BackColor = null)
            : this(new ILBackBuffer(), scene) {
            BackBuffer.Rectangle = new Rectangle(0,0,width,height); 
            this.BackColor = BackColor ?? Color.White; 
        }
        public ILGDIDriver(ILBackBuffer backbuffer, ILScene scene = null)
            : base(scene) {
            m_backBuffer = backbuffer; 
            m_textureStorage = new ILDummyTextureStorage(); 
        }
        #endregion

        #region public interface 
        protected override void BeginRender(ILRenderParameter renderParams) {
            base.BeginRender(renderParams);
            m_backBuffer.Clear(BackColor);
        }
        protected override void EndRender(ILRenderParameter renderParams) {
            base.EndRender(renderParams);
            m_backBuffer.Unlock();
        }
        public override bool Supports(Capabilities Capability) {
            switch (Capability) {
                case Capabilities.Buffered:
                    return false;
                default:
                    return false;
            }
        }
        protected override void BeginPickAt() {
            m_backBuffer.Clear(Color.Black); 
        }
        protected override int EndPickAt(System.Drawing.Point screen) {
            if (screen.X >= 0 && screen.X < m_backBuffer.Bitmap.Size.Width &&
                screen.Y >= 0 && screen.Y < m_backBuffer.Bitmap.Size.Height) {
                Color pix = m_backBuffer.Bitmap.GetPixel(screen.X, screen.Y);
                return pix.ToArgb() & 0xFFFFFF;
            } else {
                return 0; 
            }
        }
        protected override void BeginDrawScreen2D(ILRenderParameter renderParams) {
            base.BeginDrawScreen2D(renderParams);
            m_backBuffer.ZBuffer[ILMath.full] = float.MaxValue;
        }
        #endregion

        #region renderer implementation

        protected override void RenderText(ILLabel label, ILRenderParameter parameters) {
            if (!String.IsNullOrWhiteSpace(label.Text) && !label.Position.IsEmtpy()) {
                Matrix4 world2ClipTransform = parameters.PeekClipTransform();
                ILRenderQueue queue = label.Interpreter.Transform(label, m_textureStorage);
                Vector3 pos = parameters.ViewTransform * (world2ClipTransform * label.Position);
                PointF anchor = new PointF(pos.X - label.Anchor.X * queue.Size.Width, pos.Y - label.Anchor.Y * queue.Size.Height);
                Matrix old = m_backBuffer.Graphics.Transform;  
                if (Math.Abs(label.Rotation) > ILMath.epsf) {
                    Matrix rot = m_backBuffer.Graphics.Transform;
                    old = rot.Clone();
                    rot.RotateAt((float)(label.Rotation / Math.PI * 180), new PointF(pos.X, pos.Y));
                    m_backBuffer.Graphics.Transform = rot;
                }
                if (parameters.PickingContext != null) {
                    Color pickCol = parameters.PickingContext.RegisterNextShape(label.PickingID);
                    foreach (ILRenderQueueItem item in queue) {
                        using (Brush brush = new SolidBrush(pickCol)) {
                            //m_backBuffer.Graphics.DrawString(item.Text, font, brush, item.Rect.Left + anchor.X, item.Rect.Top + anchor.Y);
                            m_backBuffer.Graphics.FillRectangle(brush, item.Rect.Left + anchor.X, item.Rect.Top + anchor.Y, item.Rect.Width, item.Rect.Height);
                        }
                    }
                } else {
                    // render fringe
                    if (label.Fringe.Width > 0) {
                        // render regular content
                        foreach (ILRenderQueueItem item in queue) {
                            using (Brush brush = new SolidBrush(label.Fringe.Color)) {
                                Font font;
                                ILHashCreator.Parse(item.Key, out font);
                                for (int c = -label.Fringe.Width; c < label.Fringe.Width + 1; c++) {
                                    for (int r = -label.Fringe.Width; r < label.Fringe.Width + 1; r++) {
                                        m_backBuffer.Graphics.DrawString(item.Text, font, brush, item.Rect.Left + anchor.X + r, item.Rect.Top + anchor.Y + c);
                                    }

                                }
                                font.Dispose();
                            }
                        }
                    }
                    // render regular content
                    foreach (ILRenderQueueItem item in queue) {
                        using (Brush brush = new SolidBrush(item.Color)) {
                            Font font;
                            ILHashCreator.Parse(item.Key, out font);
                            m_backBuffer.Graphics.DrawString(item.Text, font, brush, item.Rect.Left + anchor.X, item.Rect.Top + anchor.Y);
                            font.Dispose();
                        }
                    }
                }
                if (Math.Abs(label.Rotation) > ILMath.epsf) {
                    m_backBuffer.Graphics.Transform = old;
                }
            }
        }
        [System.Security.SecuritySafeCritical]
        protected unsafe override void RenderLines(ILLines lines, ILRenderParameter parameters) {
            if (lines.Width > 1) {
                //lines.Width = 1; 
                RenderLinesWidthGreaterThan1(lines, parameters);
                return;
            }
            // lines of width 1 are handled here 
            // TODO: some optimization potential: 
            // 1) skip clipped parts completely, i.e. start drawing the visible line end until first pixel is clipped, than break
            // 2) ... (?)
            float width = lines.Width;
            var stipple = StippleFromLineStyle(lines);
            short pattern = stipple.Pattern; 
            float stippleFact = stipple.Factor;
            bool solid = (lines.DashStyle == DashStyle.Solid); 
            bool depthTest = parameters.DepthTestEnabled; 
            // handle picking
            bool picking; Vector4 pickColor;
            if (parameters.PickingContext != null) {
                picking = true;
                pickColor = parameters.PickingContext.RegisterNextShape(lines.PickingID).ToVector4();
            } else {
                pickColor = new Vector4();
                picking = false; 
            }

            foreach (Line line in lines.GetSortedandClippedScreen(parameters, lines.IsTransparent ? SortingMode.BackToFront : SortingMode.None, true)) {
                if (float.IsNaN(line.P1.X) || float.IsNaN(line.P1.Y) || float.IsNaN(line.P2.X) || float.IsNaN(line.P2.Y)) 
                    continue;

                int x1, x2, y1, y2;
                if (line.P1.X < line.P2.X) {
                    x1 = (int)Math.Ceiling(line.P1.X - 0.5f); x2 = (int)Math.Floor(line.P2.X + 0.5f);
                } else if (line.P1.X > line.P2.X) {
                    x1 = (int)Math.Floor(line.P1.X + 0.5f); x2 = (int)Math.Ceiling(line.P2.X - 0.5f);
                } else {
                    x1 = (int)Math.Ceiling(line.P1.X - 0.5f); x2 = (int)Math.Ceiling(line.P2.X - 0.5f);
                    //x1 = (int)Math.Round(Math.Round(line.P1.X * 10, MidpointRounding.AwayFromZero) / 10, MidpointRounding.AwayFromZero);
                    //x2 = (int)Math.Round(Math.Round(line.P2.X * 10, MidpointRounding.AwayFromZero) / 10, MidpointRounding.AwayFromZero);
                }
                if (line.P1.Y < line.P2.Y) {
                    y1 = (int)Math.Ceiling(line.P1.Y - 0.5f); y2 = (int)Math.Floor(line.P2.Y + 0.5f);
                } else if (line.P1.Y > line.P2.Y) {
                    y1 = (int)Math.Floor(line.P1.Y + 0.5f); y2 = (int)Math.Ceiling(line.P2.Y - 0.5f);
                } else {
                    y1 = (int)Math.Floor(line.P1.Y + 0.5f); y2 = (int)Math.Floor(line.P2.Y + 0.5f);
                    //y1 = (int)Math.Round(Math.Round(line.P1.Y * 10, MidpointRounding.AwayFromZero) / 10, MidpointRounding.AwayFromZero);
                    //y2 = (int)Math.Round(Math.Round(line.P2.Y * 10, MidpointRounding.AwayFromZero) / 10, MidpointRounding.AwayFromZero);
                }

                                  // TAKE THE OTHER ONE! It is SOO MUCH SIMPLER!!!
                x1 = (int)Math.Floor(line.P1.X); x2 = (int)Math.Floor(line.P2.X);
                y1 = (int)Math.Floor(line.P1.Y); y2 = (int)Math.Floor(line.P2.Y);

                //plotLine(x1,y1,x2,y2); continue; 

                float dX = x2 - x1; //line.P2.X - line.P1.X; // 
                float dY = y2 - y1; //line.P2.Y - line.P1.Y; // 
                #region view frustum clipping: find one valid line end, take this as start. move the other end to the border (obsolete now, since frustum is clipped in shape enumerator)
                //bool valid1, valid2;
                //if (x1 < 0 || y1 < 0 || x1 >= Size.Width || y1 >= Size.Height) {
                //    valid1 = false;
                //} else {
                //    valid1 = true;
                //}
                //if (x2 < 0 || y2 < 0 || x2 >= Size.Width || y2 >= Size.Height) {
                //    if (!valid1) 
                //        continue; // early exit
                //    valid2 = false;
                //} else {
                //    valid2 = true;
                //}
                //if (valid1 && !valid2) {
                //    moveInvalidLineEndOntoBorder(x1, y1, ref x2, ref y2, ref dX,ref dY);
                //} else if (!valid1 && valid2) {
                //    moveInvalidLineEndOntoBorder(x2, y2, ref x1, ref y1, ref dX, ref dY);
                //}
                #endregion
                int x = x1;
                int y = y1;
                int offsetY;
                int offsetX;
                int ind = 0;

                offsetX = Math.Sign(dX); 
                dX = Math.Abs(dX); 

                offsetY = Math.Sign(dY);
                dY = Math.Abs(dY); 

                Vector4 C1 = (picking) ? pickColor : line.C1;
                Vector4 C2 = (picking) ? pickColor : line.C2;

                float depth = line.P1.Z; 
                ind = x + y * m_backBuffer.Stride;
                int* pPix = m_backBuffer.Pointer;   // <- this pointer is invalidated once m_backBuffer.Bitmap is requested!!
                byte* pComp;
				if (ind >= 0 && ind < m_backBuffer.ZBuffer.S.NumberOfElements
                    && (solid || ((pattern >> (int)((- line.PatternOffset) * stippleFact) % 16) & 0x01) == 1)) {
 					if (!depthTest || m_backBuffer.ZBuffer.GetValue(ind) >= depth) {
                        pComp = (byte*)(pPix + ind);
						pComp[0] = (byte)(pComp[0] * (1 - C1.W) + C1.Z * C1.W * 255); 
						pComp[1] = (byte)(pComp[1] * (1 - C1.W) + C1.Y * C1.W * 255);
						pComp[2] = (byte)(pComp[2] * (1 - C1.W) + C1.X * C1.W * 255);
						pComp[3] = (byte)(255);
						m_backBuffer.ZBuffer.SetValue((float)depth, ind);
                    }
                }
                if (dX > dY) {
                    #region case 1
                    float error = dX / 2;
                    float dSX = (line.P2.Z - line.P1.Z) / dX;
                    float dCR = (line.C2.X - line.C1.X) / dX;
                    float dCG = (line.C2.Y - line.C1.Y) / dX;
                    float dCB = (line.C2.Z - line.C1.Z) / dX;
                    float dCA = (line.C2.W - line.C1.W) / dX;

                    int ddX = 0; 
                    while (x != x2) {
                        error = error - dY;

                        if (error < 0) {
                            y = y + offsetY;
                            error = error + dX;
                        }
                        x = x + offsetX;
                        ddX += 1;
                        ind = x + y * m_backBuffer.Stride;
                        if (ind >= 0 && ind < m_backBuffer.ZBuffer.S.NumberOfElements
                            && (solid || ((pattern >> (int)((ddX + line.PatternOffset) * stippleFact) % 16) & 0x01) == 1)) {
                            depth = line.P1.Z + dSX * ddX;
                            if (!depthTest || m_backBuffer.ZBuffer.GetValue(ind) >= depth) {
                                Vector4 col = new Vector4(
                                    C1.X + dCR * ddX,
                                    C1.Y + dCG * ddX,
                                    C1.Z + dCB * ddX,
                                    C1.W + dCA * ddX);
                                pComp = (byte*)(pPix + ind);
                                pComp[0] = (byte)(pComp[0] * (1 - col.W) + col.Z * col.W * 255);
                                pComp[1] = (byte)(pComp[1] * (1 - col.W) + col.Y * col.W * 255);
                                pComp[2] = (byte)(pComp[2] * (1 - col.W) + col.X * col.W * 255);
                                pComp[3] = (byte)(255);
                                m_backBuffer.ZBuffer.SetValue((float)depth, ind);
                            }
                        }
                    #endregion
                    }
                } else if (dY > dX) {
                    #region case 2
                    float error = dY / 2;
                    float dSY = (line.P2.Z - line.P1.Z) / dY;
                    float dCR = (C2.X - C1.X) / dY;
                    float dCG = (C2.Y - C1.Y) / dY;
                    float dCB = (C2.Z - C1.Z) / dY;
                    float dCA = (C2.W - C1.W) / dY;
                    int ddY = 0; 
                    while (y != y2) {
                        error = error - dX;
                        if (error < 0) {
                            x = x + offsetX;
                            error = error + dY;
                        }
                            y = y + offsetY;
                        ddY += 1;
                        ind = x + y * m_backBuffer.Stride;
                        if (ind >= 0 && ind < m_backBuffer.ZBuffer.S.NumberOfElements
                            && (solid || ((pattern >> (int)((ddY + line.PatternOffset) * stippleFact) % 16) & 0x01) == 1)) {
                            depth = line.P1.Z + dSY * ddY;
                            if (!depthTest || m_backBuffer.ZBuffer.GetValue(ind) >= depth) {
                                Vector4 col = new Vector4(
                                    C1.X + dCR * ddY,
                                    C1.Y + dCG * ddY,
                                    C1.Z + dCB * ddY,
                                    C1.W + dCA * ddY);
                                pComp = (byte*)(pPix + ind);
                                pComp[0] = (byte)(pComp[0] * (1 - col.W) + col.Z * col.W * 255);
                                pComp[1] = (byte)(pComp[1] * (1 - col.W) + col.Y * col.W * 255);
                                pComp[2] = (byte)(pComp[2] * (1 - col.W) + col.X * col.W * 255);
                                pComp[3] = (byte)(255);
                                m_backBuffer.ZBuffer.SetValue((float)depth, ind);
                            }
                        }
                    #endregion
                    }
                } else if (dX == dY) {
					#region 45°
                    float dSX = (line.P2.Z - line.P1.Z) / dX;
                    float dCR = (C2.X - C1.X) / dX;
                    float dCG = (C2.Y - C1.Y) / dX;
                    float dCB = (C2.Z - C1.Z) / dX;
                    float dCA = (C2.W - C1.W) / dX;
                    int ddX = 0; 
                    while (x != x2) {
                        x += offsetX;
                        y += offsetY;
                        ddX += 1;
                        ind = x + y * m_backBuffer.Stride;
                        if (ind >= 0 && ind < m_backBuffer.ZBuffer.S.NumberOfElements
                            && (solid || ((pattern >> (int)((ddX + line.PatternOffset) * stippleFact) % 16) & 0x01) == 1)) {
                            depth = line.P1.Z + dSX * ddX;
                            if (!depthTest || m_backBuffer.ZBuffer.GetValue(ind) >= depth) {
                                Vector4 col = new Vector4(
                                    C1.X + dCR * ddX,
                                    C1.Y + dCG * ddX,
                                    C1.Z + dCB * ddX,
                                    C1.W + dCA * ddX);
                                pComp = (byte*)(pPix + ind);
                                pComp[0] = (byte)(pComp[0] * (1 - col.W) + col.Z * col.W * 255);
                                pComp[1] = (byte)(pComp[1] * (1 - col.W) + col.Y * col.W * 255);
                                pComp[2] = (byte)(pComp[2] * (1 - col.W) + col.X * col.W * 255);
                                pComp[3] = (byte)(255);
                                m_backBuffer.ZBuffer.SetValue((float)depth, ind);
                            }
                        }
                    }
                    #endregion
                }
            }
        }
        private unsafe void plotLine(int x0, int y0, int x1, int y1) {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy, e2; /* error value e_xy */
            int* ind = m_backBuffer.Pointer; // + x + y * m_backBuffer.Stride;

            for (; ; ) {  /* loop */
                ind[x0 + m_backBuffer.Stride * y0] = unchecked((int)0xFFFFFFFF);
                if (x0 == x1 && y0 == y1) break;
                e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; } /* e_xy+e_x > 0 */
                if (e2 <= dx) { err += dx; y0 += sy; } /* e_xy+e_y < 0 */
            }
        }
        //private unsafe void digline(int x1, int y1, int x2, int y2) {
        //    int d, x, y, ax, ay, sx, sy, dx, dy;

        //    dx = x2 - x1; ax = Math.Abs(dx) << 1; sx = Math.Sign(dx);
        //    dy = y2 - y1; ay = Math.Abs(dy) << 1; sy = Math.Sign(dy);

        //    x = x1;
        //    y = y1;
        //    int* ind = m_backBuffer.Pointer; // + x + y * m_backBuffer.Stride;

        //    if (ax > ay) {		/* x dominant */
        //        d = ay - (ax >> 1);
        //        for (; ; ) {
        //            ind[x + m_backBuffer.Stride * y] = unchecked((int)0xFFFFFFFF);
        //            if (x == x2) return;
        //            if (d >= 0) {
        //                y += sy;
        //                d -= ax;
        //            }
        //            x += sx;
        //            d += ay;
        //        }
        //    } else {			/* y dominant */
        //        d = ax - (ay >> 1);
        //        for (; ; ) {
        //            ind[x + m_backBuffer.Stride * y] = unchecked((int)0xFF00FF00);
        //            if (y == y2) return;
        //            if (d >= 0) {
        //                x += sx;
        //                d -= ay;
        //            }
        //            y += sy;
        //            d += ax;
        //        }
        //    }
        //}
        private void RenderLinesWidthGreaterThan1(ILLines lines, ILRenderParameter parameters) {
                Matrix4 world2CameraTransform = parameters.CurrentModel2CameraTransform;
                Matrix4 camera2ClipTransform = parameters.ProjectionTransform;
                float width = lines.Width;
                // handle picking
                bool picking; Vector4 pickColor;
                if (parameters.PickingContext != null) {
                    picking = true;
                    pickColor = parameters.PickingContext.RegisterNextShape(lines.PickingID).ToVector4();
                } else {
                    pickColor = new Vector4();
                    picking = false;
                }

                var patt = StippleFromLineStyle(lines);

                #region z-buffer rendering
                foreach (Line line in lines.GetSortedandClippedScreen(parameters, frustumClipping: true)) {
                    Triangle tri1 = new Triangle();
                    Triangle tri2 = new Triangle();
                    // make the width
                    Vector3 direct = line.P2 - line.P1;
                    Vector3 widthDir = Vector3.CrossN(new Vector3(0, 0, 1), direct) * lines.Width / 2f;
                    tri1.P1 = line.P1 - widthDir;
                    tri1.P2 = line.P1 + widthDir;
                    tri1.P3 = line.P2 + widthDir;
                    tri2.P1 = line.P1 - widthDir;
                    tri2.P2 = line.P2 + widthDir;
                    tri2.P3 = line.P2 - widthDir;

                    tri1.C1 = picking ? pickColor : line.C1;
                    tri1.C2 = picking ? pickColor : line.C1;
                    tri1.C3 = picking ? pickColor : line.C2;
                    tri2.C1 = picking ? pickColor : line.C1;
                    tri2.C2 = picking ? pickColor : line.C2;
                    tri2.C3 = picking ? pickColor : line.C2;

                    if (lines.DashStyle != DashStyle.Solid) {
                        patt.Offset = line.PatternOffset; 
                        direct.Z = 0; 
                        direct.NormalizeFast(); 
                        patt.Direction = direct; 
                        tri1.Pattern = patt;
                        tri2.Pattern = patt;
                    }

                    RenderTriangleZBufferScanL(ref tri1, parameters, true);
                    RenderTriangleZBufferScanL(ref tri2, parameters, true);
                }

                #endregion

        }
        [System.Security.SecuritySafeCritical]
        protected unsafe override void RenderPoints(ILPoints points, ILRenderParameter parameters) {
            float w = points.Size; 
            float w2 = (w + 1) / 2f;
            // handle picking
            bool picking; Vector4 pickColor;
            if (parameters.PickingContext != null) {
                picking = true;
                pickColor = parameters.PickingContext.RegisterNextShape(points.PickingID).ToVector4();
            } else {
                pickColor = new Vector4();
                picking = false;
            }
            using (ILScope.Enter()) {
                ILArray<float> lin = ILMath.linspace<float>(-1,1,w); 
                ILArray<float> y1 = 1; 
                ILArray<float> x1 = ILMath.meshgrid(lin,lin,y1); 
                ILLogical circ = (x1 * x1 + y1 * y1) < 1.3f;
                foreach (Point point in points.GetSortedAndClippedScreen(parameters, points.IsTransparent ? SortingMode.BackToFront : SortingMode.None, false)) {
                    if (float.IsNaN(point.P1.X) || float.IsNaN(point.P1.Y)) continue; 
                    Vector3 p1 = point.P1;
                    if (p1.X <= -w2 || p1.Y <= -w2 || p1.X >= m_backBuffer.ZBuffer.S[0] + w2 - 1 || p1.Y >= m_backBuffer.ZBuffer.S[1] + w2 - 1) continue;
                    #region render using Z-Buffer
                    // bounding box 
                    int bbxMin = (int)Math.Round(p1.X - w2);
                    int bbyMin = (int)Math.Round(p1.Y - w2);
                    int bbxMax = (int)(bbxMin + w - 1);
                    int bbyMax = (int)(bbyMin + w - 1);
                    int bCutX = 0, bCutY = 0;
                    if (bbxMin < 0) { 
                        bCutX = -bbxMin; 
                        bbxMin = 0;
                    }
                    if (bbyMin < 0) {
                        bCutY = -bbyMin; 
                        bbyMin = 0;
                    }
                    if (bbxMax >= m_backBuffer.ZBuffer.S[0]) bbxMax = m_backBuffer.ZBuffer.S[0] - 1;
                    if (bbyMax >= m_backBuffer.ZBuffer.S[1]) bbyMax = m_backBuffer.ZBuffer.S[1] - 1;
                    if (bbxMax-bbxMin == 0 || bbyMax - bbyMin == 0) continue; 
                    ILLogical draw = (parameters.DepthTestEnabled ? p1.Z : float.MinValue) 
                                    <= m_backBuffer.ZBuffer[ILMath.r(bbxMin, bbxMax), ILMath.r(bbyMin, bbyMax)].T;
                    // make it a circle 
                    draw.a = ILMath.and(draw, circ[ILMath.r(bCutY, bCutY + draw.S[0] - 1), ILMath.r(bCutX, bCutX + draw.S[1] - 1)]); 
                    int* pPix = m_backBuffer.Pointer;
                    byte* pComp; 
                    int stride = m_backBuffer.Stride; 
                    int maxRowID = bbyMin+draw.S[0]; 
                    int c = bbxMin, r = bbyMin; 

                    // handle picking
                    Vector4 C1 = picking ? pickColor : point.C1;
                    foreach (byte drawFlag in draw) {
                        int id = r * stride + c; 
                        if (drawFlag == 1) {
                            if (point.C1.W < 1f) {
                                pComp = (byte*)(pPix + id);
                                pComp[0] = (byte)(pComp[0] * (1 - C1.W) + C1.Z * C1.W * 255);
                                pComp[1] = (byte)(pComp[1] * (1 - C1.W) + C1.Y * C1.W * 255);
                                pComp[2] = (byte)(pComp[2] * (1 - C1.W) + C1.X * C1.W * 255);
                                pComp[3] = (byte)(255);
                            } else {
                                pPix[id] = (byte)(C1.Z * 255) | ((byte)(C1.Y * 255) << 8) | ((byte)(C1.X * 255) << 16) | ((byte)(C1.W * 255) << 24);
                            }
                            m_backBuffer.ZBuffer.SetValue(p1.Z, id);  // <-- transposed storage! 
                        }
                        r++;
                        if (r >= maxRowID) {
                            r = bbyMin; 
                            c++; 
                        }
                    }
                    #endregion
                }
            }
        }
        protected unsafe override void RenderTriangles(ILTriangles triangles, ILRenderParameter parameters) {
            // potential improvements: since we use true z-buffering, one might utilize front to back sorting of ALL 
            // triangles/objects. Or even 'correct' occlusion culling with a BSP tree.

            // handle picking
            bool picking; Vector4 pickColor;
            if (parameters.PickingContext != null) {
                picking = true;
                pickColor = parameters.PickingContext.RegisterNextShape(triangles.PickingID).ToVector4();
            } else {
                pickColor = new Vector4();
                picking = false;
            }
            bool transparent = triangles.IsTransparent || parameters.Alpha.Peek() < 1; 
            if (triangles.Normals.DataCount > 2) {
                // vertex based lighting 
                foreach (Triangle tri in triangles.GetSortedAndClippedScreen(parameters, transparent ? SortingMode.BackToFront : SortingMode.None, true)) {
                    if (tri.P1 == tri.P2 || tri.P1 == tri.P3 || tri.P2 == tri.P3) continue;
                    Triangle t = tri;
                    if (!picking) {
                        #region compute Light
                        t.C1 = ComputeLight(tri.C1, tri.PCam1.Xyz, tri.N1, triangles.Shininess
                                    , triangles.SpecularColor, triangles.EmissionColor, parameters);
                        t.C2 = ComputeLight(tri.C2, tri.PCam2.Xyz, tri.N2, triangles.Shininess
                                    , triangles.SpecularColor, triangles.EmissionColor, parameters);
                        t.C3 = ComputeLight(tri.C3, tri.PCam3.Xyz, tri.N3, triangles.Shininess
                                    , triangles.SpecularColor, triangles.EmissionColor, parameters);
                        #endregion
                    } else {
                        t.C1 = pickColor;
                        t.C2 = pickColor;
                        t.C3 = pickColor; 
                    }
                    RenderTriangleZBufferScanL(ref t, parameters);
                }
            } else {
                // without lighting, but always smooth shading 
                foreach (Triangle tri in triangles.GetSortedAndClippedScreen(parameters, transparent ? SortingMode.BackToFront : SortingMode.None, true)) {
                    if (tri.P1 == tri.P2 || tri.P1 == tri.P3 || tri.P2 == tri.P3) continue;
                    Triangle t = tri;
                    if (picking) {
                        t.C1 = pickColor;
                        t.C2 = pickColor;
                        t.C3 = pickColor;
                    }
                    RenderTriangleZBufferScanL(ref t, parameters);
                }
            }
        }

        private struct Vertex {
            public Vector3 Position; 
            public Vector4 Color; 
            public static readonly int Length = 7;
        }
        private struct Poly {
            public Vertex V1;
            public Vertex V2;
            public Vertex V3;
            public static readonly int Length = 3;
        }

        [System.Security.SecuritySafeCritical]
        //private unsafe void RenderTriangleZBufferScanL(ref Triangle tri, ILRenderParameter parameters) {
        //    Poly poly = new Poly() {
        //        V1 = new Vertex() { Position = tri.P1, Color = tri.C1 },
        //        V2 = new Vertex() { Position = tri.P2, Color = tri.C2 },
        //        V3 = new Vertex() { Position = tri.P3, Color = tri.C3 },
        //    };
        //    float* pa = &poly.V1.Position.m_x;
        //    Vertex* vertices = &poly.V1; 

        //    int y, li, ri, winding, i = 0;
        //    Vertex dl, dr, l = new Vertex(), r = new Vertex();
        //    float ly, ry;
        //    int rem = Poly.Length;

        //    // determine top vertex
        //    float minY = float.MaxValue;
        //    for (int j = 0; j < Poly.Length; j++) {
        //        if (vertices[j].Position.Y < minY) {
        //            i = j;
        //            minY = vertices[j].Position.Y;
        //        }
        //    }
        //    li = ri = i;
        //    //y = (int)Math.Ceiling(minY - 0.5f);
        //    y = (int)Math.Floor(minY);
        //    if (y < 0) y = 0; 
        //    if (y >= parameters.Driver.Size.Height - 1) y = parameters.Driver.Size.Height - 1; 
        //    ly = ry = y - 1;

        //    // determine winding order, ccw / cw
        //    if (Vector3.Cross(tri.P3 - tri.P1, tri.P2 - tri.P1).Z < 0) {
        //        winding = 1;  // cw
        //    } else {
        //        winding = -1; // ccw
        //    }

        //    int* pPix = m_backBuffer.Pointer; 

        //    while (rem > 0) {

        //        while (ly <= y && rem > 0) {
        //            rem--;
        //            i = li - winding;  // go ccw down
        //            if (i < 0) i = Poly.Length - 1;
        //            if (i >= Poly.Length) i = 0;

        //            float dy, frac;
        //            dy = (int)vertices[i].Position.Y - (int)vertices[li].Position.Y;
        //            if (dy == 0) dy = 1;
        //            //frac = y + .5f - vertices[li].Position.Y;
        //            frac = y - (int)vertices[li].Position.Y;
        //            if (frac < 0) frac = 0;
        //            interpolate(&vertices[li].Position.m_x, &vertices[i].Position.m_x, &l.Position.m_x, &dl.Position.m_x, dy, frac, Vertex.Length);

        //            //ly = (int)Math.Floor(vertices[i].Position.Y + 0.5f);
        //            ly = (int)Math.Floor(vertices[i].Position.Y);
        //            li = i;
        //        }
        //        while (ry <= y && rem > 0) {
        //            rem--;
        //            i = ri + winding; // go cw down
        //            if (i < 0) i = Poly.Length - 1;
        //            if (i >= Poly.Length) i = 0;

        //            float dy, frac;
        //            dy = (int)vertices[i].Position.Y - (int)vertices[ri].Position.Y;
        //            if (dy == 0) dy = 1;
        //            //frac = y + .5f - vertices[ri].Position.Y;
        //            frac = y - (int)vertices[ri].Position.Y;
        //            if (frac < 0) frac = 0;
        //            interpolate(&vertices[ri].Position.m_x, &vertices[i].Position.m_x, &r.Position.m_x, &dr.Position.m_x, dy, frac, Vertex.Length);
        //            //ry = (int)Math.Floor(vertices[i].Position.Y + 0.5f);
        //            ry = (int)Math.Floor(vertices[i].Position.Y);
        //            ri = i;
        //        }
        //        while (y < ry && y < ly) {
        //            if (y >= 0 && y < parameters.Driver.Size.Height) {
        //                //System.Diagnostics.Debug.Assert(l.Position.X < r.Position.X);

        //                #region do single scanline
        //                int x, lx, rx, bufferIdx;
        //                Vertex p = new Vertex(), dp = new Vertex();

        //                //lx = (int)Math.Ceiling(l.Position.X - .5f);
        //                lx = (int)Math.Floor(l.Position.X);
        //                if (lx < 0) lx = 0;
        //                //rx = (int)Math.Floor(r.Position.X + .5f);
        //                rx = (int)Math.Floor(r.Position.X);
        //                if (rx >= parameters.Driver.Size.Width) rx = parameters.Driver.Size.Width - 1;
        //                //System.Diagnostics.Debug.Assert(lx <= rx); 

        //                bufferIdx = lx + m_backBuffer.Stride * y;
                        
        //                float dx, frac;
        //                dx = r.Position.X - l.Position.X;
        //                if (dx == 0) dx = 1;
        //                frac = lx - l.Position.X;
        //                if (frac < 0) frac = 0;
        //                interpolate(&l.Position.m_z, &r.Position.m_z, &p.Position.m_z, &dp.Position.m_z, dx, frac, Vertex.Length - 2);
        //                for (x = lx; x <= rx; x++) {		/* scan in x, generating pixels */
        //                    float depth = m_backBuffer.ZBuffer.GetValue(bufferIdx);
        //                    if (!parameters.DepthTestEnabled || p.Position.Z <= depth) {
        //                        if (tri.Pattern != null) {
        //                            float distx = x - tri.P1.X;
        //                            float disty = y - tri.P1.Y;
        //                            byte pattPos = (byte)((Math.Sqrt(distx * distx + disty * disty) * tri.Pattern.Item2) % 16);
        //                            if (((tri.Pattern.Item1 >> pattPos) & 0x1) == 0) {
        //                                goto increment;
        //                            }
        //                        }
        //                        if (p.Color.W < 0.99) {
        //                            byte* pComp = (byte*)(pPix + bufferIdx);
        //                            pComp[0] = (byte)(pComp[0] * (1 - p.Color.W) + p.Color.Z * p.Color.W * 255);
        //                            pComp[1] = (byte)(pComp[1] * (1 - p.Color.W) + p.Color.Y * p.Color.W * 255);
        //                            pComp[2] = (byte)(pComp[2] * (1 - p.Color.W) + p.Color.X * p.Color.W * 255);
        //                            pComp[3] = (byte)(255);
        //                        } else {
        //                            pPix[bufferIdx] = (byte)(p.Color.Z * 255) | ((byte)(p.Color.Y * 255) << 8) | ((byte)(p.Color.X * 255) << 16) | ((byte)(p.Color.W * 255) << 24);
        //                        }
        //                    }
        //                    // update depth buffer
        //                    if (p.Position.Z < depth) {
        //                        m_backBuffer.ZBuffer.SetValue(p.Position.Z, bufferIdx);
        //                    }

        //                increment:
        //                    bufferIdx++;
        //                increment(&p.Position.m_z, &dp.Position.m_z, Vertex.Length - 2);
        //                }
        //                #endregion
        //            }
        //            y++;
        //            increment(&l.Position.m_x, &dl.Position.m_x, Vertex.Length);
        //            increment(&r.Position.m_x, &dr.Position.m_x, Vertex.Length); 
        //        }

        //    }
        //}
        private unsafe static void  increment(float* p, float* dp, int len){
            for (int i = len; i-- > 0; p++, dp++) {
                *p += *dp;
            }
        }

        private unsafe static void interpolate(float* p1, float* p2, float* p, float* dp, float dy, float frac, int len) {

            for (int i = len; i --> 0; p1++, p2++, p++, dp++) {
                *dp = (*p2-*p1)/dy;
                *p = *p1+*dp*frac;
            }
        }
        //private unsafe static void incrementalize_y(float* p1, float* p2, float* p, float* dp, int y) {
        //    float dy, frac;

        //    dy = ((Vertex*)p2)[0].Position.Y - ((Vertex*)p1)[0].Position.Y;
        //    if (dy==0) dy = 1;
        //    frac = y + .5f - ((Vertex*)p1)[0].Position.Y;

        //    for (int i = Vertex.Length; i --> 0; p1++, p2++, p++, dp++) {
        //        *dp = (*p2-*p1)/dy;
        //        *p = *p1+*dp*frac;
        //    }
        //}

        #region obsolete triangle rasterizer implementations 
        //[System.Security.SecuritySafeCritical]
        //unsafe private void RenderTriangleZBufferDeprc(ref Triangle tri, ILRenderParameter parameters, bool polygonOffset = false) {
        //    // bounding box for the triangle and client rectangle clipping 
        //    Vector3 p1 = tri.P1;
        //    Vector3 p2 = tri.P2;
        //    Vector3 p3 = tri.P3;
        //    int bbxMin = (int)((p1.X < p2.X) ? ((p1.X < p3.X) ? p1.X : p3.X) : ((p2.X < p3.X) ? p2.X : p3.X));
        //    int bbyMin = (int)((p1.Y < p2.Y) ? ((p1.Y < p3.Y) ? p1.Y : p3.Y) : ((p2.Y < p3.Y) ? p2.Y : p3.Y));
        //    int bbxMax = (int)((p1.X > p2.X) ? ((p1.X > p3.X) ? p1.X : p3.X) : ((p2.X > p3.X) ? p2.X : p3.X));
        //    int bbyMax = (int)((p1.Y > p2.Y) ? ((p1.Y > p3.Y) ? p1.Y : p3.Y) : ((p2.Y > p3.Y) ? p2.Y : p3.Y));
        //    //// frustum clipping -> done in triangles enumerator
        //    //RectangleF viewRect = parameters.ViewTransform.ToViewRectangle();
        //    //PointF clipLim = new PointF((float)Math.Ceiling(Math.Max(viewRect.X, 0f)), (float)Math.Ceiling(Math.Max(viewRect.Y,0f))); 
        //    //SizeF clipExtends = new SizeF((float)Math.Ceiling(Math.Min(viewRect.Width,m_backBuffer.ZBuffer.S[0])), (float)Math.Ceiling(Math.Min(viewRect.Height, m_backBuffer.ZBuffer.S[1]))); 
        //    //viewRect = new RectangleF(clipLim, clipExtends);
        //    //if (bbxMin < viewRect.X) bbxMin = (int)viewRect.X;
        //    //if (bbyMin < viewRect.Y) bbyMin = (int)viewRect.Y;
        //    //if (bbxMax < viewRect.X) bbxMax = (int)viewRect.X;
        //    //if (bbyMax < viewRect.Y) bbyMax = (int)viewRect.Y;
        //    //if (bbxMin >= viewRect.Right) bbxMin = (int)viewRect.Right;
        //    //if (bbyMin >= viewRect.Bottom) bbyMin = (int)viewRect.Bottom;
        //    //if (bbxMax >= viewRect.Right) bbxMax = (int)viewRect.Right;
        //    //if (bbyMax >= viewRect.Bottom) bbyMax = (int)viewRect.Bottom;
        //    // barycentric coordinates
        //    //fab(x,y) = (ya - yb)*x + (xb - xa)*y + xa*yb - xb*ya
        //    //fbc(x,y) = (yb - yc)*x + (xc - xb)*y + xb*yc - xc*yb
        //    //fca(x,y) = (yc - ya)*x + (xa - xc)*y + xc*ya - xa*yc
        //    using (ILScope.Enter()) {
        //        ILArray<float> Y = 1;
        //        ILArray<float> X = ILMath.meshgrid(ILMath.vec<float>(bbxMin, bbxMax), ILMath.vec<float>(bbyMin, bbyMax), Y);
        //        ILArray<float> gamma = ((p1.Y - p2.Y) * X + (p2.X - p1.X) * Y + p1.X * p2.Y - p2.X * p1.Y)
        //                           / ((p1.Y - p2.Y) * p3.X + (p2.X - p1.X) * p3.Y + p1.X * p2.Y - p2.X * p1.Y);
        //        ILArray<float> alpha = ((p2.Y - p3.Y) * X + (p3.X - p2.X) * Y + p2.X * p3.Y - p3.X * p2.Y)
        //                           / ((p2.Y - p3.Y) * p1.X + (p3.X - p2.X) * p1.Y + p2.X * p3.Y - p3.X * p2.Y);
        //        ILArray<float> beta = 1 - alpha - gamma;
        //        ILArray<float> depth = alpha * p1.Z + beta * p2.Z + gamma * p3.Z;
        //        if (polygonOffset) {
        //            //float bbzMin = (p1.Z < p2.Z) ? ((p1.Z < p3.Z) ? p1.Z : p3.Z) : ((p2.Z < p3.Z) ? p2.Z : p3.Z);
        //            //float bbzMax = ((p1.Z > p2.Z) ? ((p1.Z > p3.Z) ? p1.Z : p3.Z) : ((p2.Z > p3.Z) ? p2.Z : p3.Z)) + 1;
        //            //bbzMin = Math.Max (bbzMax - bbzMin, ILMath.epsf); 
        //            depth.a = depth + ILLines.PolygonOffset; //bbzMin; 
        //        }

        //        // test, which pixels lay inside the triangle AND match the depth buffer.
        //        ILArray<byte> find = (alpha > 0) + (alpha < 1) + (beta > 0) + (beta < 1) + (gamma > 0) + (gamma < 1);
        //        ILArray<int> draw;
        //        if (parameters.DepthTestEnabled) {
        //            // Note: the positive Z-axis points INTO the screen for ND coordinates! 
        //            find.a = find + (depth <= m_backBuffer.ZBuffer[ILMath.r(bbxMin, bbxMax), ILMath.r(bbyMin, bbyMax)].T); 
        //            draw = ILMath.find(find == 7);
        //        } else {
        //            draw = ILMath.find(find == 6);
        //        }
        //        int h = alpha.S[0]; 
        //        // render all pixels from 'draw' indices
        //        int* pPix = m_backBuffer.Pointer;
        //        byte* pComp;
        //        foreach (int ind in draw) {
        //            float a = alpha.GetValue(ind), b = beta.GetValue(ind), c = gamma.GetValue(ind);
        //            float d = depth.GetValue(ind); 
        //            int col = ind / h + bbxMin, row = ind % h + bbyMin;
        //            if (tri.Pattern != null) {
        //                float distx = col - tri.P1.X;
        //                float disty = row - tri.P1.Y;
        //                byte pattPos = (byte)((Math.Sqrt(distx * distx + disty * disty) * tri.Pattern.Item2) % 16);
        //                if (((tri.Pattern.Item1 >> pattPos) & 0x1) == 0) {
        //                    continue; 
        //                }
        //            }
        //            int index = col + row * m_backBuffer.Stride; 
        //            Vector4 color = (tri.C1 * a + tri.C2 * b + tri.C3 * c);
        //            if (color.W < 0.99) {
        //                pComp = (byte*)(pPix + index);
        //                pComp[0] = (byte)(pComp[0] * (1 - color.W) + color.Z * color.W * 255);
        //                pComp[1] = (byte)(pComp[1] * (1 - color.W) + color.Y * color.W * 255);
        //                pComp[2] = (byte)(pComp[2] * (1 - color.W) + color.X * color.W * 255);
        //                pComp[3] = (byte)(255); 
        //            } else {
        //                pPix[index] = (byte)(color.Z * 255) | ((byte)(color.Y * 255) << 8) | ((byte)(color.X * 255) << 16) | ((byte)(color.W * 255) << 24);
        //            }
        //            // update depth buffer
        //            m_backBuffer.ZBuffer.SetValue(d, index);
        //        }
        //    }
        //}

        [System.Security.SecuritySafeCritical]
        private unsafe void RenderTriangleZBufferScanL(ref Triangle tri, ILRenderParameter parameters, bool polygonOffset = false) {
            try {
                // order vertices: v1 is always the upper (left), v2: 'middle' left/right, v3: lowest (right)
                int v1, v2, v3;
                #region minimum
                if (tri.P2.Y < tri.P1.Y) {
                    if (tri.P2.Y < tri.P3.Y) {
                        v1 = 1;
                    } else {
                        v1 = 2;
                    }
                } else {
                    if (tri.P3.Y < tri.P1.Y) {
                        v1 = 2;
                    } else {
                        v1 = 0;
                    }
                }
                #endregion
                #region maximum
                if (tri.P2.Y > tri.P1.Y) {
                    if (tri.P2.Y > tri.P3.Y) {
                        v3 = 1;
                    } else {
                        v3 = 2;
                    }
                } else {
                    if (tri.P3.Y > tri.P1.Y) {
                        v3 = 2;
                    } else {
                        v3 = 0;
                    }
                }
                #endregion
                v2 = 3 - v1 - v3;

                fixed (Vector3* pa = &tri.P1)
                fixed (Vector4* pc = &tri.C1) {

                    float dy3_1 = pa[v3].Y - pa[v1].Y;
                    float xm = (pa[v3].X - pa[v1].X) / dy3_1;
                    float zm = (pa[v3].Z - pa[v1].Z) / dy3_1;
                    float dy2_1 = pa[v2].Y - pa[v1].Y;

                    Vector3 newPoint3 = new Vector3(pa[v1].X + dy2_1 * xm, pa[v2].Y, pa[v1].Z + dy2_1 * zm);
                    Vector4 newColor = pc[v3] * dy2_1 / dy3_1 + pc[v1] * (1 - dy2_1 / dy3_1);

                    ScanlineData triPart = new ScanlineData();
                    triPart.Pattern = tri.Pattern;

                    int yStart = (int)Math.Ceiling(pa[v1].Y - 0.5f);
                    if (yStart < 0) yStart = 0;
                    if (yStart >= m_backBuffer.ZBuffer.S[1] - 1)
                        yStart = m_backBuffer.ZBuffer.S[1] - 1;

                    triPart.PicIndex = yStart * m_backBuffer.ZBuffer.S[0];
                    triPart.YSteps = (int)Math.Floor(pa[v2].Y + 0.5f) - yStart;

                    if (triPart.YSteps > 0) {
                        #region render upper triangle
                        // v2 in on same scanline than new point3 now

                        triPart.PositionStart1 = pa[v1];
                        triPart.PositionStart2 = pa[v1];
                        triPart.ColorStart1 = pc[v1];
                        triPart.ColorStart2 = pc[v1];

                        if (polygonOffset) {
                            triPart.PositionStart1.Z -= ILLines.PolygonOffset;
                            triPart.PositionStart2.Z -= ILLines.PolygonOffset;
                        }
                        // make sure, scanlines start on left side 
                        if (newPoint3.X < pa[v2].X) {
                            triPart.PositionInc1 = (newPoint3 - pa[v1]) / triPart.YSteps;
                            triPart.PositionInc2 = (pa[v2] - pa[v1]) / triPart.YSteps;

                            triPart.ColorInc1 = (newColor - pc[v1]) / triPart.YSteps;
                            triPart.ColorInc2 = (pc[v2] - pc[v1]) / triPart.YSteps;

                        } else {

                            triPart.PositionInc2 = (newPoint3 - pa[v1]) / triPart.YSteps;
                            triPart.PositionInc1 = (pa[v2] - pa[v1]) / triPart.YSteps;

                            triPart.ColorInc2 = (newColor - pc[v1]) / triPart.YSteps;
                            triPart.ColorInc1 = (pc[v2] - pc[v1]) / triPart.YSteps;

                        }
                        //// compensate for pattern offset; subsequent triangles must 
                        //// reference to the same pattern origin to match their dash styles exactly. 
                        //// because triPart moves the start line index, we must correct the reference point ...
                        //if (triPart.Pattern != null) {
                        //    triPart.Pattern.Offset 
                        //}
                        doScanLines(ref triPart, parameters.DepthTestEnabled);
                        #endregion
                    }

                    #region render lower triangle scanlines
                    yStart += triPart.YSteps;
                    if (yStart < 0) yStart = 0;
                    if (yStart >= m_backBuffer.ZBuffer.S.NumberOfElements - m_backBuffer.Stride)
                        yStart = m_backBuffer.ZBuffer.S.NumberOfElements - m_backBuffer.Stride;

                    triPart.PicIndex = yStart * m_backBuffer.ZBuffer.S[0];
                    triPart.YSteps = (int)Math.Floor(pa[v3].Y + 0.5f) - yStart;

                    if (triPart.YSteps > 0) {

                        if (newPoint3.X < pa[v2].X) {
                            triPart.PositionStart1 = newPoint3;
                            triPart.PositionStart2 = pa[v2];
                            triPart.ColorStart1 = newColor;
                            triPart.ColorStart2 = pc[v2];

                            triPart.PositionInc1 = (pa[v3] - newPoint3) / triPart.YSteps;
                            triPart.PositionInc2 = (pa[v3] - pa[v2]) / triPart.YSteps;
                            triPart.ColorInc1 = (pc[v3] - newColor) / triPart.YSteps;
                            triPart.ColorInc2 = (pc[v3] - pc[v2]) / triPart.YSteps;

                        } else {
                            triPart.PositionStart1 = pa[v2];
                            triPart.PositionStart2 = newPoint3;
                            triPart.ColorStart1 = pc[v2];
                            triPart.ColorStart2 = newColor;

                            triPart.PositionInc1 = (pa[v3] - pa[v2]) / triPart.YSteps;
                            triPart.PositionInc2 = (pa[v3] - newPoint3) / triPart.YSteps;
                            triPart.ColorInc1 = (pc[v3] - pc[v2]) / triPart.YSteps;
                            triPart.ColorInc2 = (pc[v3] - newColor) / triPart.YSteps;
                        }
                        if (polygonOffset) {
                            triPart.PositionStart1.Z += ILLines.PolygonOffset;
                            triPart.PositionStart2.Z += ILLines.PolygonOffset;
                        }

                        doScanLines(ref triPart, parameters.DepthTestEnabled);
                    }
                    #endregion
                }
            } catch (ILNumerics.Exceptions.ILArgumentException exc) {
                System.Diagnostics.Trace.WriteLine("Error in ILGDIDriver.RenderTriangleZBufferScanL:");
                System.Diagnostics.Trace.WriteLine(exc.ToString());
                System.Diagnostics.Trace.WriteLine("Input data:");
                System.Diagnostics.Trace.WriteLine(tri.ToString());
            }

        }
        private struct ScanlineData {
            public Vector3 PositionStart1, PositionStart2;
            public Vector4 ColorStart1, ColorStart2;
            public Vector3 PositionInc1, PositionInc2;
            public Vector4 ColorInc1, ColorInc2; 
            public int YSteps; 
            public int PicIndex;
            public ILDashInfo Pattern;
        }
        private unsafe void doScanLines(ref ScanlineData data, bool depthTest = true) {

            for (int y = 0; y < data.YSteps; y++) {
                Vector3 linePosStart = data.PositionStart1 + y * data.PositionInc1;
                Vector3 linePosEnd = data.PositionStart2 + y * data.PositionInc2;

                int* pPix = m_backBuffer.Pointer;
                byte* pComp;
                int xStart = (int)Math.Ceiling(linePosStart.X - 0.5f);
                int xEnd = (int)Math.Floor(linePosEnd.X + 0.5f);
                // some poor man's clamping 
                if (xStart < 0) xStart = 0;
                if (xEnd >= Size.Width) {
                    xEnd = Size.Width - 1; 
                }

                Vector4 color = data.ColorStart1 + y * data.ColorInc1;
                Vector4 lineColInc = new Vector4();
                float ZInc = 0; 
                if (xEnd - xStart > 0) {
                    lineColInc = (data.ColorStart2 + y * data.ColorInc2 - color) / (xEnd - xStart);
                    ZInc = (linePosEnd.Z - linePosStart.Z) / (xEnd - xStart); 
                }
                float depth = linePosStart.Z; 
                int index = data.PicIndex + y * m_backBuffer.Stride; 
                for (int x = xStart; x <= xEnd; x++) {
                    if (depthTest && m_backBuffer.ZBuffer.GetValue(index + x) < depth) 
                        continue; 
                    if (data.Pattern != null) {
                        float pattdist = Vector3.Dot(new Vector3(x, data.PositionStart1.Y + y, 0), data.Pattern.Direction);
                        byte pattPos = (byte)((byte)(pattdist * data.Pattern.Factor + data.Pattern.Offset) % 16);
                        if (((data.Pattern.Pattern >> pattPos) & 0x1) == 0) {
                            continue;
                        }
                    }
                    if (color.W < 0.99) {
                        pComp = (byte*)(pPix + index + x);
                        pComp[0] = (byte)(pComp[0] * (1 - color.W) + color.Z * color.W * 255);
                        pComp[1] = (byte)(pComp[1] * (1 - color.W) + color.Y * color.W * 255);
                        pComp[2] = (byte)(pComp[2] * (1 - color.W) + color.X * color.W * 255);
                        pComp[3] = (byte)(255);
                    } else {
                        pPix[index + x] = (byte)(color.Z * 255) | ((byte)(color.Y * 255) << 8) | ((byte)(color.X * 255) << 16) | ((byte)(color.W * 255) << 24);
                    }
                    // update depth buffer
                    m_backBuffer.ZBuffer.SetValue(depth, index + x);
                    depth += ZInc; 
                    color += lineColInc; 
                }
            }
        }
        #endregion
        #endregion

        #region IDisposable Members

        public void Dispose() {
            if (m_backBuffer != null) {
                m_backBuffer.Dispose(); 
            }
        }

        #endregion

    }
}
