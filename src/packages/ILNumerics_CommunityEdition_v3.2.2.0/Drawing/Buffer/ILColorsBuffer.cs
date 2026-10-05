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
using System.Drawing; 

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILColorsBuffer : ILBuffer<float> {

        public ILColorsBuffer() : base(4) {
            BufferType = Drawing.BufferType.ColorBuffer; 
        }

        public Color GetColorAt(int i) {
            lock (Lock) {
                return Color.FromArgb((int)(m_storage.GetValue(3, i) * 0xFF),
                                        (int)(m_storage.GetValue(0, i) * 0xFF),
                                        (int)(m_storage.GetValue(1, i) * 0xFF),
                                        (int)(m_storage.GetValue(2, i) * 0xFF));
            }
        }
        public Vector3 GetVector3At(int i) {
            lock (Lock) {
                return new Vector3(m_storage.GetValue(0, i),
                                   m_storage.GetValue(1, i),
                                   m_storage.GetValue(2, i));
            }
        }

        public Vector4 GetVector4At(int i) {
            lock (Lock) {
                return new Vector4(m_storage.GetValue(0, i),
                                   m_storage.GetValue(1, i),
                                   m_storage.GetValue(2, i),
                                   m_storage.GetValue(3, i));
            }
        }

        public override ILBuffer<float> Copy() {
            ILColorsBuffer ret = new ILColorsBuffer();
            lock (Lock) {
                ret.m_storage.a = m_storage.C;
            }
            return ret;
        }
        public void Update(int index, double R, double G, double B, double A) {
            base.Update(index, 1, new float[] { (float)R, (float)G, (float)B, (float)A });
        }
        public override void Update(ILInArray<float> data) {
            using (ILScope.Enter(data)) {
                if (ILMath.isnull(data) || data.S[0] == 4) {
                    base.Update(data); 
                } else if (data.S[0] == 3) {
                    int oldCount = DataCount;
                    if (ILMath.isnull(data) || ILMath.isempty(data)) {
                        m_storage.a = ILMath.empty<float>(m_dataLength, 0);
                        Version++;
                        OnChanged(0, oldCount, ChangeQueueActions.Delete);  // todo: make sure, buffered driver react on Delete changes properly
                    } else {
                        ILArray<float> data_ = data;
                        m_storage["0:2", ILMath.r(0, data_.S[1] - 1)] = data_;
                        m_storage["3", ILMath.r(0, data_.S[1] - 1)] = 1;
                        if (DataCount > data.S[1]) {
                            m_storage[ILMath.full, ILMath.r(data_.S[1], ILMath.end)] = null;
                        }
                        Version++;
                        OnChanged(0, DataCount, ChangeQueueActions.Update);
                    }
                } else {
                    throw new Exceptions.ILArgumentException("Only matrices of size ([3|4] x n) can be converted to colors buffers. Found: " + data.S.ToString());
                }
            }
        }
        public void Update(int index, Color color) {
            base.Update(index, 1, new float[] { (float)(color.R / 255f), (float)(color.G / 255f), (float)(color.B / 255f), (float)(color.A / 255f) });
        }
        public static implicit operator ILColorsBuffer(ILRetArray<float> A) {
            using (ILScope.Enter(A)) {
                if (ILMath.isnull(A) || (A.S[0] != 3 && A.S[0] != 4)) {
                    throw new Exceptions.ILArgumentException("Only matrices of size ([3|4] x n) can be converted to colors buffers");
                }
                ILColorsBuffer ret = new ILColorsBuffer();
                ret.Update(A);
                return ret;
            }
        }
        public static implicit operator ILColorsBuffer(ILInArray<float> A) {
            using (ILScope.Enter(A)) {
                if (ILMath.isnull(A) || (A.S[0] != 3 && A.S[0] != 4)) {
                    throw new Exceptions.ILArgumentException("Only matrices of size ([3|4] x n) can be converted to colors buffers");
                }
                ILColorsBuffer ret = new ILColorsBuffer();
                ret.Update(A);
                return ret;
            }
        }
        public static implicit operator ILColorsBuffer(ILArray<float> A) {
            using (ILScope.Enter(A)) {
                if (ILMath.isnull(A) || (A.S[0] != 3 && A.S[0] != 4)) {
                    throw new Exceptions.ILArgumentException("Only matrices of size ([3|4] x n) can be converted to colors buffers");
                }
                ILColorsBuffer ret = new ILColorsBuffer();
                ret.Update(A);
                return ret;
            }
        }
        public static implicit operator ILColorsBuffer(float[,] A) {
            using (ILScope.Enter()) {
                if (A == null || (A.GetLength(1) != 3 && A.GetLength(1) != 4)) {
                    throw new Exceptions.ILArgumentException("Expected matrix of size ([3|4] x n). Since .NET 2dim arrays are row based, A is defined as usual (row based) and gets transposed for conversion!");
                }
                ILColorsBuffer ret = new ILColorsBuffer();
                ret.Update(A);
                return ret;
            }
        }


    }
}
