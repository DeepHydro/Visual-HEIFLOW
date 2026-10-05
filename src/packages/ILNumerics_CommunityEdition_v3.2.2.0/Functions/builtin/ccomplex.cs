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
using System.Runtime.InteropServices; 
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Native;
using ILNumerics.Exceptions;

namespace ILNumerics {

    public partial class ILMath {

        /// <summary>
        /// Create complex array from real and imaginary parts 
        /// </summary>
        /// <param name="real">Array with real part elements</param>
        /// <param name="imag">Array with imaginary part elements</param>
        /// <returns>Complex array constructed out of real and imaginary parts</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arguments is not the same and neither of 
        /// the input is scalar.</exception>
        public static ILRetArray<complex> ccomplex(ILInArray<double> real, ILInArray<double> imag) {
            using (ILScope.Enter(real, imag)) {
                if (!real.Size.IsSameSize(imag.Size) && !real.IsScalar && !imag.IsScalar) {

                    throw new ILArgumentException("fcomplex: input arrays must have the same size!");
                }
                ILSize outSize = real.Size; 
                if (real.IsScalar) 
                    outSize = imag.Size;

                ILArray<complex> ret = array<complex>(outSize);
                int nelem = outSize.NumberOfElements;
                complex[] retArr = ret.GetArrayForWrite();
                double[] realArr = real.GetArrayForRead();
                double[] imagArr = imag.GetArrayForRead();
                if (!real.IsScalar && !imag.IsScalar) {
                    for (int i = 0; i < nelem; i++) {
                        retArr[i].imag = imagArr[i];
                        retArr[i].real = realArr[i];
                    }
                } else if (imag.IsScalar) {
                    // real and/or imag is scalar
                    double imagValue = imagArr[0]; 
                    for (int i = 0; i < nelem; i++) {
                        retArr[i].imag = imagValue;
                        retArr[i].real = realArr[i];
                    }
                } else {
                    // real is scalar
                    double realValue = realArr[0];
                    for (int i = 0; i < nelem; i++) {
                        retArr[i].imag = imagArr[i];
                        retArr[i].real = realValue;
                    }
                }
                return ret;
            }
        }
        /// <summary>
        /// Create complex array from real and imaginary parts
        /// </summary>
        /// <param name="real">Array with real part elements</param>
        /// <param name="imag">Array with imaginary part elements</param>
        /// <returns>Complex array constructed out of real and imaginary parts</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arguments is not the same and neither of 
        /// the input is scalar.</exception>
        public static ILRetArray<fcomplex> ccomplex(ILInArray<float> real, ILInArray<float> imag) {
            using (ILScope.Enter(real, imag)) {
                if (!real.Size.IsSameSize(imag.Size) && !real.IsScalar && !imag.IsScalar) {

                    throw new ILArgumentException("fcomplex: input arrays must have the same size!");
                }
                ILSize outSize = real.Size;
                if (real.IsScalar)
                    outSize = imag.Size;

                ILArray<fcomplex> ret = array<fcomplex>(outSize);
                int nelem = outSize.NumberOfElements;
                fcomplex[] retArr = ret.GetArrayForWrite();
                float[] realArr = real.GetArrayForRead();
                float[] imagArr = imag.GetArrayForRead();
                if (!real.IsScalar && !imag.IsScalar) {
                    for (int i = 0; i < nelem; i++) {
                        retArr[i].imag = imagArr[i];
                        retArr[i].real = realArr[i];
                    }
                } else if (imag.IsScalar) {
                    // real and/or imag is scalar
                    float imagValue = imagArr[0];
                    for (int i = 0; i < nelem; i++) {
                        retArr[i].imag = imagValue;
                        retArr[i].real = realArr[i];
                    }
                } else {
                    // real is scalar
                    float realValue = realArr[0];
                    for (int i = 0; i < nelem; i++) {
                        retArr[i].imag = imagArr[i];
                        retArr[i].real = realValue;
                    }
                }
                return ret;
            }
        }

    }
}
