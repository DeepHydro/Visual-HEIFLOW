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
using System.Diagnostics; 

namespace ILNumerics.Drawing {

    [DebuggerDisplay("[3 x 3] Matrix3")]
    public struct Matrix3 {
        // column major storage!!
        public float M11, M21, M31;
        public float M12, M22, M32;
        public float M13, M23, M33;

        public Matrix3(float m11, float m12, float m13,
                       float m21, float m22, float m23,
                       float m31, float m32, float m33) {
            M11 = m11; M12 = m12; M13 = m13;
            M21 = m21; M22 = m22; M23 = m23;
            M31 = m31; M32 = m32; M33 = m33;
        }

        public static Matrix3 TransposeInvert(Matrix4 A) {
            return Matrix3.Transpose(Matrix4.Invert(A)); 
        }

        public static Matrix3 Transpose(Matrix4 A) {
            return new Matrix3(
                A.M11, A.M21, A.M31, 
                A.M12, A.M22, A.M32, 
                A.M13, A.M23, A.M33); 
        }
        public static ILRetArray<float> operator *(Matrix3 left, ILInArray<float> right) {
            using (ILScope.Enter(right)) {
                if (right.IsEmpty) {
                    return ILMath.empty<float>(3,right.S[1]); 
                } else {
                    return ILMath.multiply(left.ToArray(), right);
                }
            }
        }
        internal ILRetArray<float> ToArray() {
            using (ILScope.Enter()) {
                float[] retArr = ILMath.New<float>(16);
                ILArray<float> ret = ILMath.array<float>(retArr, ILMath.size(3, 3));
                ret[0] = M11; ret[3] = M12; ret[6] = M13; 
                ret[1] = M21; ret[4] = M22; ret[7] = M23; 
                ret[2] = M31; ret[5] = M32; ret[8] = M33; 
                return ret;
            }
        }
        public static readonly Matrix3 Identity = new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1);
    }
}
