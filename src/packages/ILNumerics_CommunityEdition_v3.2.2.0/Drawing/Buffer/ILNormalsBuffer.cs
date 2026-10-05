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

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILNormalsBuffer : ILBuffer<float> {

        public ILNormalsBuffer() : base(3) {
            BufferType = Drawing.BufferType.NormalBuffer;
        }

        public Vector3 GetNormalAt(int i) {
            lock (Lock) {
                return new Vector3(m_storage.GetValue(0, i),
                                      m_storage.GetValue(1, i),
                                      m_storage.GetValue(2, i));
            }
        }
        public override ILBuffer<float> Copy() {
            ILNormalsBuffer ret = new ILNormalsBuffer();
            lock (Lock) {
                ret.m_storage.a = m_storage.C;
            }
            return ret;
        }
        public static implicit operator ILNormalsBuffer(ILInArray<float> A) {
            using (ILScope.Enter(A)) {
                if (ILMath.isnull(A) || A.S[0] != 3) {
                    throw new Exceptions.ILArgumentException("Only matrices of size (3 x n) can be converted to normals buffers");
                }
                ILNormalsBuffer ret = new ILNormalsBuffer();
                ret.Update(A);
                return ret;
            }
        }
        public static implicit operator ILNormalsBuffer(ILRetArray<float> A) {
            using (ILScope.Enter(A)) {
                if (ILMath.isnull(A) || A.S[0] != 3) {
                    throw new Exceptions.ILArgumentException("Only matrices of size (3 x n) can be converted to normals buffers");
                }
                ILNormalsBuffer ret = new ILNormalsBuffer();
                ret.Update(A);
                return ret;
            }
        }
        public static implicit operator ILNormalsBuffer(ILArray<float> A) {
            using (ILScope.Enter(A)) {
                if (ILMath.isnull(A) || A.S[0] != 3) {
                    throw new Exceptions.ILArgumentException("Only matrices of size (3 x n) can be converted to normals buffers");
                }
                ILNormalsBuffer ret = new ILNormalsBuffer();
                ret.Update(A);
                return ret;
            }
        }
        public static implicit operator ILNormalsBuffer(float[,] A) {
            using (ILScope.Enter()) {
                if (A == null || A.GetLength(1) != 3) {
                    throw new Exceptions.ILArgumentException("Expected matrix of size (n x 3). Since .NET 2dim arrays are row based, A is defined as usual (row based) and gets transposed for conversion!");
                }
                ILNormalsBuffer ret = new ILNormalsBuffer();
                ret.Update(A);
                return ret;
            }
        }

    }
}
