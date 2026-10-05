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
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Exceptions;



namespace ILNumerics  {
    public partial class ILMath {

        /// <summary>
        /// Convert real array to complex array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Complex array with A as real part, imaginary part is zero</returns>
        public static ILRetArray<complex> real2complex( ILInArray< double> A ) {
            using (ILScope.Enter(A)) {
                int nrX = A.Size.NumberOfElements;
                complex[] retArr = ILMemoryPool.Pool.New<complex>(nrX);
                ILRetArray<complex> ret = new ILRetArray<complex>(retArr, A.Size);
                
                double[] inArr = A.GetArrayForRead();
                for (int i = 0; i < nrX; i++) {
                    retArr[i] = new complex((double)inArr[i], 0.0);
                }
                return ret;
            }
        }
        /// <summary>
        /// Create complex array out of real and imaginary parts
        /// </summary>
        /// <param name="real">Real array for real part</param>
        /// <param name="imag">Real array for imaginary part</param>
        /// <returns>Complex array having the real- and part imaginary parts constructed out of 
        /// real and imag.</returns>
        /// <remarks>Real and imag must have the same number of elements.
        /// The array returned will have the same size as the input arrays.</remarks>
        public static ILRetArray<complex> real2complex( ILInArray< double> real, ILInArray< double> imag ) {
            using (ILScope.Enter(real, imag)) {
                int nrX = real.Size.NumberOfElements;
                if (nrX != imag.Size.NumberOfElements)
                    throw new ILArgumentSizeException("size of real and imag input arrays must match!");
                complex[] retArr = ILMemoryPool.Pool.New<complex>(nrX);
                ILRetArray<complex> ret = new ILRetArray<complex>(retArr, real.Size);
                
                double[] inArrReal = real.GetArrayForRead();
                
                double[] inArrImag = imag.GetArrayForRead();
                for (int i = 0; i < nrX; i++) {
                    retArr[i].real = (double)inArrReal[i];
                    retArr[i].imag = (double)inArrImag[i];
                }
                return ret;
            }
        }


#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Convert real array to complex array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Complex array with A as real part, imaginary part is zero</returns>
        public static ILRetArray<complex> real2complex( ILInArray< byte> A ) {
            using (ILScope.Enter(A)) {
                int nrX = A.Size.NumberOfElements;
                complex[] retArr = ILMemoryPool.Pool.New<complex>(nrX);
                ILRetArray<complex> ret = new ILRetArray<complex>(retArr, A.Size);
               
                byte[] inArr = A.GetArrayForRead();
                for (int i = 0; i < nrX; i++) {
                    retArr[i] = new complex((double)inArr[i], 0.0);
                }
                return ret;
            }
        }
        /// <summary>
        /// Create complex array out of real and imaginary parts
        /// </summary>
        /// <param name="real">Real array for real part</param>
        /// <param name="imag">Real array for imaginary part</param>
        /// <returns>Complex array having the real- and part imaginary parts constructed out of 
        /// real and imag.</returns>
        /// <remarks>Real and imag must have the same number of elements.
        /// The array returned will have the same size as the input arrays.</remarks>
        public static ILRetArray<complex> real2complex( ILInArray< byte> real, ILInArray< byte> imag ) {
            using (ILScope.Enter(real, imag)) {
                int nrX = real.Size.NumberOfElements;
                if (nrX != imag.Size.NumberOfElements)
                    throw new ILArgumentSizeException("size of real and imag input arrays must match!");
                complex[] retArr = ILMemoryPool.Pool.New<complex>(nrX);
                ILRetArray<complex> ret = new ILRetArray<complex>(retArr, real.Size);
               
                byte[] inArrReal = real.GetArrayForRead();
               
                byte[] inArrImag = imag.GetArrayForRead();
                for (int i = 0; i < nrX; i++) {
                    retArr[i].real = (double)inArrReal[i];
                    retArr[i].imag = (double)inArrImag[i];
                }
                return ret;
            }
        }

        /// <summary>
        /// Convert real array to complex array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Complex array with A as real part, imaginary part is zero</returns>
        public static ILRetArray<complex> real2complex( ILInArray< Int64> A ) {
            using (ILScope.Enter(A)) {
                int nrX = A.Size.NumberOfElements;
                complex[] retArr = ILMemoryPool.Pool.New<complex>(nrX);
                ILRetArray<complex> ret = new ILRetArray<complex>(retArr, A.Size);
               
                Int64[] inArr = A.GetArrayForRead();
                for (int i = 0; i < nrX; i++) {
                    retArr[i] = new complex((double)inArr[i], 0.0);
                }
                return ret;
            }
        }
        /// <summary>
        /// Create complex array out of real and imaginary parts
        /// </summary>
        /// <param name="real">Real array for real part</param>
        /// <param name="imag">Real array for imaginary part</param>
        /// <returns>Complex array having the real- and part imaginary parts constructed out of 
        /// real and imag.</returns>
        /// <remarks>Real and imag must have the same number of elements.
        /// The array returned will have the same size as the input arrays.</remarks>
        public static ILRetArray<complex> real2complex( ILInArray< Int64> real, ILInArray< Int64> imag ) {
            using (ILScope.Enter(real, imag)) {
                int nrX = real.Size.NumberOfElements;
                if (nrX != imag.Size.NumberOfElements)
                    throw new ILArgumentSizeException("size of real and imag input arrays must match!");
                complex[] retArr = ILMemoryPool.Pool.New<complex>(nrX);
                ILRetArray<complex> ret = new ILRetArray<complex>(retArr, real.Size);
               
                Int64[] inArrReal = real.GetArrayForRead();
               
                Int64[] inArrImag = imag.GetArrayForRead();
                for (int i = 0; i < nrX; i++) {
                    retArr[i].real = (double)inArrReal[i];
                    retArr[i].imag = (double)inArrImag[i];
                }
                return ret;
            }
        }

        /// <summary>
        /// Convert real array to complex array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Complex array with A as real part, imaginary part is zero</returns>
        public static ILRetArray<complex> real2complex( ILInArray< Int32> A ) {
            using (ILScope.Enter(A)) {
                int nrX = A.Size.NumberOfElements;
                complex[] retArr = ILMemoryPool.Pool.New<complex>(nrX);
                ILRetArray<complex> ret = new ILRetArray<complex>(retArr, A.Size);
               
                Int32[] inArr = A.GetArrayForRead();
                for (int i = 0; i < nrX; i++) {
                    retArr[i] = new complex((double)inArr[i], 0.0);
                }
                return ret;
            }
        }
        /// <summary>
        /// Create complex array out of real and imaginary parts
        /// </summary>
        /// <param name="real">Real array for real part</param>
        /// <param name="imag">Real array for imaginary part</param>
        /// <returns>Complex array having the real- and part imaginary parts constructed out of 
        /// real and imag.</returns>
        /// <remarks>Real and imag must have the same number of elements.
        /// The array returned will have the same size as the input arrays.</remarks>
        public static ILRetArray<complex> real2complex( ILInArray< Int32> real, ILInArray< Int32> imag ) {
            using (ILScope.Enter(real, imag)) {
                int nrX = real.Size.NumberOfElements;
                if (nrX != imag.Size.NumberOfElements)
                    throw new ILArgumentSizeException("size of real and imag input arrays must match!");
                complex[] retArr = ILMemoryPool.Pool.New<complex>(nrX);
                ILRetArray<complex> ret = new ILRetArray<complex>(retArr, real.Size);
               
                Int32[] inArrReal = real.GetArrayForRead();
               
                Int32[] inArrImag = imag.GetArrayForRead();
                for (int i = 0; i < nrX; i++) {
                    retArr[i].real = (double)inArrReal[i];
                    retArr[i].imag = (double)inArrImag[i];
                }
                return ret;
            }
        }

        /// <summary>
        /// Convert real array to complex array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Complex array with A as real part, imaginary part is zero</returns>
        public static ILRetArray<complex> real2complex( ILInArray< float> A ) {
            using (ILScope.Enter(A)) {
                int nrX = A.Size.NumberOfElements;
                complex[] retArr = ILMemoryPool.Pool.New<complex>(nrX);
                ILRetArray<complex> ret = new ILRetArray<complex>(retArr, A.Size);
               
                float[] inArr = A.GetArrayForRead();
                for (int i = 0; i < nrX; i++) {
                    retArr[i] = new complex((double)inArr[i], 0.0);
                }
                return ret;
            }
        }
        /// <summary>
        /// Create complex array out of real and imaginary parts
        /// </summary>
        /// <param name="real">Real array for real part</param>
        /// <param name="imag">Real array for imaginary part</param>
        /// <returns>Complex array having the real- and part imaginary parts constructed out of 
        /// real and imag.</returns>
        /// <remarks>Real and imag must have the same number of elements.
        /// The array returned will have the same size as the input arrays.</remarks>
        public static ILRetArray<complex> real2complex( ILInArray< float> real, ILInArray< float> imag ) {
            using (ILScope.Enter(real, imag)) {
                int nrX = real.Size.NumberOfElements;
                if (nrX != imag.Size.NumberOfElements)
                    throw new ILArgumentSizeException("size of real and imag input arrays must match!");
                complex[] retArr = ILMemoryPool.Pool.New<complex>(nrX);
                ILRetArray<complex> ret = new ILRetArray<complex>(retArr, real.Size);
               
                float[] inArrReal = real.GetArrayForRead();
               
                float[] inArrImag = imag.GetArrayForRead();
                for (int i = 0; i < nrX; i++) {
                    retArr[i].real = (double)inArrReal[i];
                    retArr[i].imag = (double)inArrImag[i];
                }
                return ret;
            }
        }


#endregion HYCALPER AUTO GENERATED CODE
   }

}
