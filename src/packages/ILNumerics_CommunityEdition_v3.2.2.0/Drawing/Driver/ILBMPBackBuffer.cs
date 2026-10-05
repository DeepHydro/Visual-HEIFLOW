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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text;

namespace ILNumerics.Drawing {
    
    [System.Security.SecuritySafeCritical]
    public unsafe class ILBackBuffer : IDisposable {

        #region attributes
        Rectangle m_rect; 
        Bitmap m_bmp; 
        Graphics m_graphics;
        BitmapData m_bmData; 
        int* m_pointer;
        object m_syncLock = new object();
        ILArray<float> m_zBuffer = ILMath.localMember<float>(); 
        #endregion

        #region public interface
        internal bool IsLocked {
            get { return m_pointer != (int*)0; }
        }
        internal int* Pointer {
            get {
                lock (m_syncLock) {
                    if (!IsLocked) {
                        Lock();
                    }
                    return m_pointer;
                }
            }
        }
        /// <summary>
        /// storage between adjacent pixels of two rows in number of pixels; valid only when the buffer is locked!
        /// </summary>
        internal int Stride { get; set; }

        public Rectangle Rectangle {
            get {
                return m_rect; 
            }
            set {
                lock (m_syncLock) {
                    if (m_rect != value) {
                        Bitmap = new Bitmap(value.Width, value.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    }
                }
            }
        }
        public Bitmap Bitmap {
            get {
                lock (m_syncLock) {
                    if (IsLocked)
                        Unlock();
                    if (m_bmp == null)
                        Bitmap = new Bitmap(0, 0);
                    return m_bmp;
                }
            }
            set {
                lock (m_syncLock) {
                    if (object.ReferenceEquals(m_bmp, value)) return; 
                    if (m_graphics != null) {
                        m_graphics.Dispose();
                    }
                    if (m_bmp != null) {
                        if (IsLocked)
                            Unlock();
                        m_bmp.Dispose();
                    }
                    m_bmp = value;
                    m_graphics = Graphics.FromImage(m_bmp);
                    m_rect = new Rectangle(0, 0, m_bmp.Size.Width, m_bmp.Size.Height);
                    // the z buffer is stored transposed for simplified indexing with bitmap
                    m_zBuffer.a = ILMath.array<float>(0,ILMath.size(m_rect.Width, m_rect.Height)); 
                }
            }
        }
        public Graphics Graphics {
            get {
                lock (m_syncLock) {
                    if (IsLocked)
                        Unlock();
                    return m_graphics;
                }
            }
        }
        public ILBackBuffer() { }
        internal void Lock() {
            lock (m_syncLock) {
                m_bmData = m_bmp.LockBits(m_rect, ImageLockMode.ReadWrite, m_bmp.PixelFormat);
                m_pointer = (int*)m_bmData.Scan0;
                Stride = m_bmData.Stride / 4;
            }
        }
        internal void Unlock() {
            lock (m_syncLock) {
                if (IsLocked) {
                    m_bmp.UnlockBits(m_bmData);
                    m_bmData = null;
                    m_pointer = (int*)0;
                }
            }
        }
        internal ILArray<float> ZBuffer {
            get { return m_zBuffer; }
        }
        internal void Clear(Color backcolor) {
            m_zBuffer[ILMath.full] = float.MaxValue;
            Graphics.Clear(backcolor);
        }

        public void Dispose() {
            Dispose(true);
        }
        #endregion

        protected void Dispose(bool cleanManaged) {
            if (cleanManaged) {
                if (IsLocked) {
                    Unlock();
                }
                if (m_bmp != null) {
                    m_bmp.Dispose();
                }
                if (Graphics != null) {
                    Graphics.Dispose();
                }
                if (m_zBuffer != null) {
                    m_zBuffer.Dispose();
                }
            }
        }
    }
}
