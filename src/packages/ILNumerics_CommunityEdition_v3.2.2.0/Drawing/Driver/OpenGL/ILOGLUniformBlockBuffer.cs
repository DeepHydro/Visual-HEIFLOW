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
using ILNumerics; 
using ILNumerics.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;  

namespace ILNumerics.Drawing {
    public class ILOGLUniformBlockBuffer<T> where T : struct {
        private T[] m_data; 
        public int UniformBlockIndex { get; private set; }
        public int MaxLength { get; private set; }
        public List<T> Data {
            get {
                return m_data.ToList<T>(); }
            set {
                if (value.Count > MaxLength)
                    throw new Exceptions.ILArgumentException("The buffer is too small to take all values.");
                if (Length != value.Count || !value.SequenceEqual(m_data)) {
                    value.CopyTo(m_data, 0);
                    Length = value.Count;
                    Update();
                } 
            }
        }
        public T First {
            get {
                if (MaxLength < 1) {
                    throw new Exceptions.ILArgumentException("the collection is empty - cannot retrieve first item"); 
                }
                return Data[0]; 
            }
            set {
                if (MaxLength < 1) {
                    throw new Exceptions.ILArgumentException("The maximum buffer size is 0 - cannot access first item");
                }
                if (Length < 1) Length = 1; 
                m_data[0] = value; 
                Update(); 
            }
        }
        public int Length { get; private set; }
        public int GLID { get; private set; }

        public ILOGLUniformBlockBuffer(int bindingBlockIndex, int maxlength = 1) {
            GLID = -1;
            MaxLength = maxlength; 
            UniformBlockIndex = bindingBlockIndex;
            m_data = new T[MaxLength];
            Length = 0; 
            //CreateBuffer(bindingBlockIndex); 
        }

        private void CreateBuffer() {
            int bufID; 
            GL.GenBuffers(1, out bufID);
            GLID = bufID;
            GL.BindBuffer(BufferTarget.UniformBuffer, GLID);
            T item = default(T); 
            int size = System.Runtime.InteropServices.Marshal.SizeOf(item) * MaxLength;  
            GL.BufferData(BufferTarget.UniformBuffer, (IntPtr)size, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.BindBufferBase(BufferTarget.UniformBuffer, UniformBlockIndex, GLID);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0); 
            //Length = MaxLength; 
        }
        public void Update() {
            if (GLID < 0) {
                CreateBuffer(); 
            }
            if (Length > 0) {
                GL.BindBuffer(BufferTarget.UniformBuffer, GLID);
                T item = m_data[0];
                int size = System.Runtime.InteropServices.Marshal.SizeOf(item) * Length;
                GL.BufferSubData(BufferTarget.UniformBuffer, (IntPtr)0, (IntPtr)size, m_data);
                GL.BindBuffer(BufferTarget.UniformBuffer, 0);
                //GL.BindBufferRange(BufferTarget.UniformBuffer, UniformBlockIndex, GLID, (IntPtr)0, (IntPtr)size);
            }
        }
        public void Dispose() {
            if (GLID >= 0) {
                int glID = GLID; 
                GL.DeleteBuffers(1, ref glID); 
                GLID = -1; 
                // todo: check if binding index slot should be reset as well? 
            }
        }
    }
}
