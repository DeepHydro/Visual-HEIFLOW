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
using System.Text;
using ILNumerics.Misc;
using OpenTK.Graphics.OpenGL; 
using OpenTK.Graphics;
using System.Drawing;
using System.Drawing.Imaging;
using ILNumerics.Exceptions; 

namespace ILNumerics.Drawing {
    /// <summary>
    /// OpenGL implementation of ILTextureStorage
    /// </summary>
    internal class ILOGLTextureStorage : ILTextureStorage {

        #region attributes
        new int [] m_tmpData = new int[20*20];
        #endregion

        #region constructor
        public ILOGLTextureStorage(Size size) : base(size) { }
        #endregion

        #region abstract overrides 
        /// <summary>
        /// Select this storage to be current in GL
        /// </summary>
        public override void MakeCurrent() {
            GL.BindTexture(TextureTarget.Texture2D,m_textureId.Value);  
            //System.Diagnostics.Debug.WriteLine(String.Format("ILOGLTextureStorage: switched to ID#{0}",m_textureId)); 
        }
        public override void Resize(Size size) {
            if (m_textureId.HasValue) {
                GL.DeleteTexture(m_textureId.Value);
            }
            Create(new Size(m_width * 2, m_height));
            OnCleared(); 
        }
        /// <summary>
        /// initialize texture sheet 
        /// </summary>
        protected override void InitTexture() {
            int dummy; 
            GL.GenTextures(1, out dummy);
            m_textureId = dummy;

            //System.Diagnostics.Debug.WriteLine(String.Format("ILTextureStorage{2}: TextureID={0}, Thread={1}"
            //        ,m_textureId, System.Threading.Thread.CurrentThread.ManagedThreadId,this.GetHashCode())); 
            GL.BindTexture(TextureTarget.Texture2D, m_textureId.Value);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Alpha, m_width, m_height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
        }
        /// <summary>
        /// store item in texture sheet in GL
        /// </summary>
        /// <param name="bmp">new item bitmap data, ARGB format</param>
        /// <param name="bmpRectF">rectangle used in bitmap</param>
        /// <param name="rect">rectangle specifying area to store the data into the texture sheet, 
        /// integer pixels coords: range from 0...m_width|m_height (e.g. '512')</param>
        [System.Security.SecuritySafeCritical]
        protected override void Store(Bitmap bmp, RectangleF bmpRectF, RectangleF rect) {
            if (rect.Size.Width == 0 || rect.Size.Height == 0) return; 
            if (m_disposed) 
                throw new InvalidOperationException("The texture storage has been disposed."); 
            if (bmp == null) 
                throw new ILArgumentException("Bitmap argument must not be null!"); 
            Rectangle bmpRect = Rectangle.Ceiling(bmpRectF); 
            if (bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb) {
                bmp = bmp.Clone(bmpRect,System.Drawing.Imaging.PixelFormat.Format32bppArgb); 
                bmpRect.X = 0; bmpRect.Y = 0; 
            }
            GL.BindTexture(TextureTarget.Texture2D, m_textureId.Value);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0,0,bmp.Width,bmp.Height), ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            // setup OpenGL pixel store
            GL.PixelStore(PixelStoreParameter.UnpackAlignment,1.0f); 
            GL.PixelStore(PixelStoreParameter.UnpackRowLength,bmp.Width); 
            GL.PixelStore(PixelStoreParameter.UnpackSkipRows,bmpRect.Y); 
            GL.PixelStore(PixelStoreParameter.UnpackSkipPixels,bmpRect.X); 
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, (int)rect.Left, (int)rect.Top, bmpRect.Width, bmpRect.Height,
                     OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, bmp_data.Scan0);
#region DEBUGGING only
#if EXPORTBMP
            // must copy to new data array
            if (m_tmpData.Length < bmpRect.Height * bmpRect.Width) {
                if (m_tmpData != null) 
                    ILMemoryPool.Pool.Free(m_tmpData); 
                m_tmpData = ILMemoryPool.Pool.New<int>(bmpRect.Height * bmpRect.Width);    
            }
            unsafe {
            fixed (int* tmpDataP = m_tmpData) {
                int* pOut = tmpDataP; 
                int* pIn; 
                int* pInEnd;
                for (int r = 0; r < bmpRect.Height; r++) {
                    //pIn = (int*)bitmap_data.Scan0 + (r+(int)location.Y) * data.Width + (int)location.X; 
                    pIn = ((int*)bmp_data.Scan0) + r * (bmp_data.Stride / 4) + bmpRect.X; 
                    pInEnd = pIn + bmpRect.Width; 
                    while (pIn < pInEnd) {
                        *(pOut++) = *(pIn++);     
                    }
                }
                //GL.TexSubImage2D(TextureTarget.Texture2D, 0, (int)rect.Left, (int)rect.Top, bmpRect.Width, bmpRect.Height,
                //     OpenTK.Graphics.PixelFormat.Rgba, PixelType.UnsignedByte, (IntPtr)tmpDataP);
                Bitmap outB = new Bitmap(bmpRect.Width, bmpRect.Height);
                for (int i = 0; i < bmpRect.Width * bmpRect.Height; i++)
                    outB.SetPixel(i%bmpRect.Width,(int)(i/bmpRect.Width),Color.FromArgb(m_tmpData[i]));
                outB.Save("EXPORTBMP_textureCached.bmp",ImageFormat.Bmp); 
                outB.Dispose(); 
                string dbTrOut = String.Format("usedArea:{0}:{1}:{2}:{3} data:{4} bmp_data: {5}:{6} stride:{7} Loc:{8}"
                                ,bmpRect.Left, bmpRect.Top, bmpRect.Width, bmpRect.Height
                                ,bmp.Size.ToString()
                                ,bmp_data.Width,bmp_data.Height
                                ,bmp_data.Stride
                                ,bmpRectF.Location); 

                Console.Out.WriteLine(dbTrOut); 
                System.Diagnostics.Debug.WriteLine(dbTrOut); 
            }}
#endif
#endregion
            bmp.UnlockBits(bmp_data);
        }
        /// <summary>
        /// dispose off the texture storage's ressources
        /// </summary>
        /// <param name="manual"></param>
        public override void Dispose(bool manual) {
            if (!m_disposed) {
                if (manual) {
                    // free texture from OGL 
                    GL.DeleteTexture(m_textureId.Value);
                }
                m_disposed = true; 
            }
        }
        #endregion

    }
}
