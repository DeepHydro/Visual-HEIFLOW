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
using System.Text;
using System.Drawing; 
using System.Collections.Generic;
using ILNumerics.Drawing;
using ILNumerics.Exceptions; 
using System.Drawing.Text; 

namespace ILNumerics.Drawing {
    /// <summary>
    /// Transforms characters into bitmaps (1:1)
    /// </summary>
    /// <remarks>this is the base class for most IILTextInterpreter implementations</remarks>
    [Serializable]
    public class ILSimpleInterpreter : IILTextInterpreter {

        #region attributes
        static Graphics m_measGraphics; 
        static Bitmap m_measBitmap; 
        protected Font m_normalFont; 
        protected Size m_size;
        protected float m_oversamplingFact = 1; 
        #endregion

        #region properties
	    #endregion

        #region constructor

        /// <summary>
        /// create a new instance of this text interpreter
        /// </summary>
        public ILSimpleInterpreter () : base () {
            resizeMeasureBMP(20,20);
        }

#endregion

        #region helper functions 

        protected void resizeMeasureBMP(int width, int height) {
            if (m_measBitmap != null) 
                m_measBitmap.Dispose(); 
            if (m_measGraphics != null)
                m_measGraphics.Dispose();
            m_measBitmap = new Bitmap((int)((width + 1) * m_oversamplingFact), (int)((height + 1) * m_oversamplingFact)); 
            m_measGraphics = Graphics.FromImage(m_measBitmap); 
            m_measGraphics.ScaleTransform(m_oversamplingFact,m_oversamplingFact); 
        }

        protected virtual float parseString (string expression, Font font, System.Drawing.PointF offset, Color color, 
                                    ref SizeF size, IList<ILRenderQueueItem> queue, ILTextureStorage texStorage) {
            int pos = 0;
            string itemText; 
            int curHeigth = 0, curWidth = 0; 
            float lineHeight = 0, lineWidth = 0; 
            Size itemSize = Size.Empty; 
            while (pos < expression.Length) {
                itemText = expression.Substring(pos++,1);

                ILRenderQueueItem item = texStorage.Store(itemText, font, offset, color, transformItem);
                if (item == null) {
                    throw new ILArgumentException("The text cannot be drawn. The length or size is too large."); 
                }
                if (item.Rect.Height > lineHeight)
                    lineHeight = item.Rect.Height;
                lineWidth += item.Rect.Width;
                offset.X += item.Rect.Width; 
                queue.Add(item);
            }
            size.Width += ((curWidth > lineWidth) ? curWidth : lineWidth);
            size.Height = curHeigth + lineHeight; 
            return size.Width; 
        }

        /// <summary>
        /// Render a string onto a bitmap and measure exact size
        /// </summary>
        /// <param name="item">item to be rendered</param>
        /// <param name="font">font used for rendering</param>
        /// <param name="textureStorageSize">[output] size of the rendered item</param>
        /// <returns>bitmap containing the item</returns>
        public Bitmap TransformItem (string item, Font font, out RectangleF textureStorageSize, out RectangleF textureGlyphSize, out SizeF glyphScreenSize, bool oversample = true) {
            Bitmap ret = transformItem(item, font, out textureStorageSize, out textureGlyphSize, out glyphScreenSize, oversample); 
            return ret.Clone(Rectangle.Round(textureStorageSize),ret.PixelFormat); 
        }

        /// <summary>
        /// Render a string onto a bitmap and measure exact size
        /// </summary>
        /// <param name="item">item to be rendered</param>
        /// <param name="font">font used for rendering</param>
        /// <param name="textureStorageSize">[output] size of the rendered item</param>
        /// <returns>bitmap containing the item</returns>
        protected Bitmap transformItem(string item, Font font, out RectangleF textureStorageSize, out RectangleF textureGlyphSize, out SizeF glyphScreenSize, 
                                    bool oversample = true) {
            if (String.IsNullOrEmpty(item)) {
                textureStorageSize = new RectangleF(0,0,1,1);  
                textureGlyphSize = textureStorageSize; 
                glyphScreenSize = textureGlyphSize.Size; 
                return m_measBitmap; 
            }
            if (item == " ") {
                glyphScreenSize = m_measGraphics.MeasureString(item, font);
                textureGlyphSize = new RectangleF(0, 0, glyphScreenSize.Width * (oversample ? m_oversamplingFact : 1f), glyphScreenSize.Height * (oversample ? m_oversamplingFact : 1f));
                if (textureGlyphSize.Right >= m_measBitmap.Width || textureGlyphSize.Bottom >= m_measBitmap.Height) {
                    resizeMeasureBMP((int)textureGlyphSize.Right + (2 * ILFringe.MAX_FRINGE_WIDTH), (int)textureGlyphSize.Bottom + (2 * ILFringe.MAX_FRINGE_WIDTH));     
                    return transformItem(item,font,out textureStorageSize, out textureGlyphSize, out glyphScreenSize, oversample);
                }
                m_measGraphics.Clear(Color.Transparent); 
                textureStorageSize = textureGlyphSize; 
                return m_measBitmap;
            }
            StringFormat sformat = StringFormat.GenericDefault; 
            sformat.FormatFlags = StringFormatFlags.NoWrap 
                                | StringFormatFlags.NoClip 
                                | StringFormatFlags.MeasureTrailingSpaces; 
            sformat.SetMeasurableCharacterRanges(new CharacterRange[]{new CharacterRange(0,item.Length)});

            // draw the text
            if (font.SizeInPoints < 16) {
                m_measGraphics.TextRenderingHint = ILLabel.SmallFontTextRenderingHint;
            } else {
                m_measGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }
            RectangleF layout = new RectangleF(ILFringe.MAX_FRINGE_WIDTH, ILFringe.MAX_FRINGE_WIDTH, 
                                            (oversample ? m_oversamplingFact : 1f) * m_measBitmap.Width - 2 * ILFringe.MAX_FRINGE_WIDTH,
                                            (oversample ? m_oversamplingFact : 1f) * m_measBitmap.Height - 2 * ILFringe.MAX_FRINGE_WIDTH);
            m_measGraphics.Clear(Color.Transparent); 
            m_measGraphics.DrawString(item,font,Brushes.White,layout,sformat);
            // measure bounds 
            Region[] reg = m_measGraphics.MeasureCharacterRanges(item,font,layout,sformat);
            textureGlyphSize = reg[0].GetBounds(m_measGraphics);
            #region testbmp
#if TESTBMP
            //Bitmap testbmp = (Bitmap)Bitmap.FromFile("testpx.bmp");
            //if (testbmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb) {
            //    Image target = new Bitmap(testbmp.Width, testbmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //    Graphics g = Graphics.FromImage(target);
            //    g.Clear(Color.Transparent);
            //    g.DrawImage(testbmp, 0, 0);
            //    target.Save("testpx1.bmp");
            //    testbmp = (Bitmap)Bitmap.FromFile("testpx1.bmp");
            //}
            textureGlyphSize = new RectangleF(3,3,6,6); 
            m_measGraphics.Clear(Color.Transparent);
            m_measGraphics.DrawRectangle(Pens.White, 3, 3, 5, 5); 
            m_measGraphics.DrawLine(Pens.White, 0,0,11,11); 
            
            //m_measGraphics.DrawImageUnscaled(testbmp,0,0); 
            m_measBitmap.Save("measbitmap.bmp"); 
#endif
            #endregion
            glyphScreenSize = new SizeF(textureGlyphSize.Size.Width + 1, textureGlyphSize.Size.Height + 1);
            textureGlyphSize = new RectangleF(textureGlyphSize.X * (oversample ? m_oversamplingFact : 1f),
                                            textureGlyphSize.Y * (oversample ? m_oversamplingFact : 1f),
                                            glyphScreenSize.Width * (oversample ? m_oversamplingFact : 1f),
                                            glyphScreenSize.Height * (oversample ? m_oversamplingFact : 1f));
            //size = new RectangleF((glyphSize.X / m_oversamplingFact) - ILFringe.MAX_FRINGE_WIDTH, (glyphSize.Y / m_oversamplingFact) - ILFringe.MAX_FRINGE_WIDTH, (glyphSize.Width / m_oversamplingFact) + 2 * ILFringe.MAX_FRINGE_WIDTH, (glyphSize.Height / m_oversamplingFact) + 2 * ILFringe.MAX_FRINGE_WIDTH);
            textureStorageSize = RectangleF.Inflate(textureGlyphSize, ILFringe.MAX_FRINGE_WIDTH, ILFringe.MAX_FRINGE_WIDTH); 
            //size.Width += 1; 
            //size.Height += 1; 
            // compensate 
            //size = new RectangleF((size.Left>0)?size.Left-1:size.Left,size.Top,size.Width,size.Height);
#if EXPORTBMP   // debug support only 
            if (false) {
                //Bitmap debBitmap = (Bitmap)m_measBitmap.Clone(); 
                //Graphics debGrap = Graphics.FromImage(debBitmap); 
                
                //debGrap.Clear(Color.White); 
                //debGrap.DrawString(item,font,Brushes.Black,layout,sformat);
                m_measGraphics.ScaleTransform(1f / m_oversamplingFact, 1f / m_oversamplingFact); 
                m_measGraphics.DrawRectangle(new Pen(Brushes.Red), Rectangle.Round(textureGlyphSize));
                m_measGraphics.DrawRectangle(new Pen(Brushes.Blue,1), Rectangle.Round(textureStorageSize));
                m_measBitmap.Save("EXPORTBMP_transformItemResult.bmp",System.Drawing.Imaging.ImageFormat.Bmp);
                m_measGraphics.ScaleTransform(m_oversamplingFact, m_oversamplingFact); 

            }
#endif
            if (textureStorageSize.Right >= m_measBitmap.Width 
                || textureStorageSize.Bottom >= m_measBitmap.Height) {
                resizeMeasureBMP((int)textureStorageSize.Right + (2 * ILFringe.MAX_FRINGE_WIDTH), (int)textureStorageSize.Bottom + (2 * ILFringe.MAX_FRINGE_WIDTH));     
                return transformItem(item,font,out textureStorageSize, out textureGlyphSize, out glyphScreenSize);
            }
#if testbmp
            //return testbmp;
#endif
            return m_measBitmap; 
        }

        #endregion

        #region IILTextInterpreter Member

        public ILRenderQueue Transform(ILLabel label, ILTextureStorage textureStorage) {
            SizeF labelSize = new SizeF();
            System.Drawing.PointF offset = new System.Drawing.PointF();
            List<ILRenderQueueItem> queue = new List<ILRenderQueueItem>();
            parseString(label.Text, label.Font, offset, label.Color ?? Color.Empty, ref labelSize, queue, textureStorage);
            return new ILRenderQueue(label.Text, queue, labelSize);
        }

        #endregion

    }
}
