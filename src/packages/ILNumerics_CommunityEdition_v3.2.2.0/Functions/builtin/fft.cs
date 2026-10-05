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
using ILNumerics.Exceptions; 
using ILNumerics.Misc; 

namespace ILNumerics  {
    public partial class ILMath {




        #region fft(A)
        /// <summary>
        /// Fast fourier transform (1D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Transformed output array</returns>
        /// <remarks><para>The transformation is computed along the first 
        /// non singleton dimension.</para>
        /// <para>The output array returned will be complex hermitian. I.e. the real 
        /// part being even and the imaginary part being odd symmetrical.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< complex> fft(ILInArray< double> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex>(A.Size);
                if (A.IsScalar) return new  complex(A.GetValue(0), 0);
                int fnsd = A.Size.WorkingDimension();
                return ILMath.FFTImplementation.FFTForward1D(A, fnsd);
            }
        }
        /// <summary>
        /// Fast fourier transform (1D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Transformed output array</returns>
        /// <remarks>
        /// <para>The transformation is computed along the first non 
        /// singleton dimension.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< complex > fft(ILInArray< complex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex>(A.Size);
                if (A.IsScalar) return A.C;
                int fnsd = A.Size.WorkingDimension();
                return ILMath.FFTImplementation.FFTForward1D(A, fnsd);
            }
        }
        /// <summary>
        /// Fast inverse fourier transform (1D)
        /// </summary>
        /// <param name="A">Input (frequency domain)</param>
        /// <returns>Inverse transformed output array</returns>
        /// <remarks>
        /// <para>The transformation is computed along the first non 
        /// singleton dimension.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray<complex> ifft(ILInArray<complex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex>(A.Size);
                if (A.IsScalar) return A.C;
                int fnsd = A.Size.WorkingDimension();
                return ILMath.FFTImplementation.FFTBackward1D(A, fnsd);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform, complex hermitian input
        /// </summary>
        /// <param name="A">Complex hermitian input array</param>
        /// <returns>Real output array, same size as A</returns>
        /// <remarks>
        /// <para>Since a transform of complex hermitian input data results in 
        /// the output having all imaginary part equal zero, only the real part is 
        /// returned for convenience reasons.</para>
        /// <para>No check is made for A being hermitian! If A is not hermitian (by means 
        /// of round-off errors), the result will be wrong!</para>
        /// <para>The transformation is computed along the first non 
        /// singleton dimension.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< double > ifftsym(ILInArray< complex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< double>(A.Size);
                if (A.IsScalar) return real(A);
                int fnsd = A.Size.WorkingDimension();
                return ILMath.FFTImplementation.FFTBackwSym1D(A, fnsd);
            }
        }
        #endregion

        #region fft(A, dim)
        /// <summary>
        /// Fast fourier transform along specific dimension
        /// </summary>
        /// <param name="A">Real input array</param>
        /// <param name="dim">Dimension to compute FFT along. This parameter must be non-negative. </param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The output array returned will be complex hermitian. I.e. the real 
        /// part being even and the imaginary part being odd symmetrical.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if 
        /// the dim parameter is negative</exception>
        public static ILRetArray< complex > fft(ILInArray< double > A, int dim) {
            using (ILScope.Enter(A)) {
                if (dim < 0) throw new ILArgumentException("the 'dim' parameter must point to an existing dimension index of A");
                if (A.IsEmpty) return empty< complex>(A.Size);
                if (A.IsScalar) return new  complex(A.GetValue(0), 0);
                return ILMath.FFTImplementation.FFTForward1D(A, dim);
            }
        }
        /// <summary>
        /// Fast fourier transform along specific dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">Dimension to compute FFT along. This parameter
        /// must be non-negative. </param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if 
        /// the dim parameter is negative</exception>
        public static ILRetArray< complex > fft(ILInArray< complex > A, int dim) {
            using (ILScope.Enter(A)) {
                if (dim < 0) throw new ILArgumentException("the 'dim' parameter must point to an existing dimension index of A");
                if (A.IsEmpty) return empty< complex>(A.Size);
                if (A.IsScalar) return A.C;
                return ILMath.FFTImplementation.FFTForward1D(A, dim); 
            }
        }
        /// <summary>
        /// Inverse fast fourier transform along specific dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">Dimension to compute FFT along. This parameter
        /// must be non-negative. </param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if the dim parameter is negative</exception>
        public static ILRetArray< complex> ifft(ILInArray< complex> A, int dim) {
            using (ILScope.Enter(A)) {
                if (dim < 0) throw new ILArgumentException("the 'dim' parameter must point to an existing dimension index of A");
                if (A.IsEmpty) return empty< complex>(A.Size);
                if (A.IsScalar) return A.C;
                return ILMath.FFTImplementation.FFTBackward1D(A, dim);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform, complex hermitian input
        /// </summary>
        /// <param name="A">Complex hermitian input array (frequency domain)</param>
        /// <param name="dim">Dimension to compute FFT along. This parameter
        /// must be non-negative. </param>
        /// <returns>Real output array, same size as A</returns>
        /// <remarks>
        /// <para>Since a transform of complex hermitian input data results in the 
        /// output having the imaginary part equals zero, only the real part is 
        /// returned for convenience reasons.</para>
        /// <para>No check is made for A being hermitian! If A is not hermitian (by means 
        /// of round-off errors), the result will be wrong!</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if the dim parameter is negative</exception>
        public static ILRetArray< double > ifftsym(ILInArray< complex > A, int dim) {
            using (ILScope.Enter(A)) {
                if (dim < 0) throw new ILArgumentException("the 'dim' parameter must point to an existing dimension index of A");
                if (A.IsEmpty) return empty< double>(A.Size);
                if (A.IsScalar) return real(A);
                return ILMath.FFTImplementation.FFTBackwSym1D(A, dim);
            }
        }
        #endregion

        #region fft2(A)
        /// <summary>
        /// Fast fourier transform (2D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The 2D transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The output array returned will be complex hermitian.</para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< complex > fft2(ILInArray< double > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex>(A.Size);
                if (A.IsScalar) return new  complex(A.GetValue(0), 0);
                return FFTImplementation.FFTForward(A, 2);
            } 
        }
        /// <summary>
        /// Fast fourier transform (2D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< complex> fft2(ILInArray< complex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex>(A.Size);
                if (A.IsScalar) return A.C;
                return FFTImplementation.FFTForward(A, 2);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (2D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< complex> ifft2(ILInArray< complex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex>(A.Size);
                if (A.IsScalar) return A.C;
                return FFTImplementation.FFTBackward(A, 2);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (2D, hermitian input)
        /// </summary>
        /// <param name="A">Complex hermitian input array (frequency domain)</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>Since a transform of complex hermitian input data results in the 
        /// output having the imaginary part equals zero, only the real part is 
        /// returned for convenience reasons.</para>
        /// <para>No check is made for A being hermitian! If A is not hermitian (by means 
        /// of round-off errors), the result will be wrong!</para>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< double> ifft2sym(ILInArray< complex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< double>(A.Size);
                if (A.IsScalar) return real(A);
                return FFTImplementation.FFTBackwSym(A, 2);
            }
        }
        #endregion

        #region fft2(A,m,n)
        /// <summary>
        /// Fast fourier transform (2D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="m">Transformation column length</param>
        /// <param name="n">Transformation row length</param>
        /// <returns>Transformation result, complex hermitian</returns>
        /// <remarks>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The data to be transformed (based on the A array) are resized according to 
        /// the length parameter m and n. If m or n is larger then the length of the corresponding 
        /// dimension of A, zeros will be padded, otherwise the dimensions are truncated respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if one of n or m is negative</exception>
        public static ILRetArray< complex> fft2(ILInArray< double> A, int m, int n) {
            using (ILScope.Enter(A)) {
                if (m < 0 || n < 0) throw new ILArgumentException("dimension length specifier 'm' and 'n' must be non-negative!");
                if (A.IsEmpty) return empty< complex>(A.Size);
                int[] Asize = A.Size.ToIntArray();
                Asize[0] = m; Asize[1] = n;
                ILArray< double> resizedA = resize4Transform(A, Asize);
                return FFTImplementation.FFTForward(resizedA, 2);
            }
        }
        /// <summary>
        /// Fast fourier transform (2D)
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="m">Transformation column length</param>
        /// <param name="n">Transformation row length</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The data to be transformed (based on the A array) are resized according to 
        /// the length parameter m and n. If m or n is larger then the length of the corresponding 
        /// dimension of A, zeros will be padded, otherwise the dimensions are truncated respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if one of n or m is negative</exception>
        public static ILRetArray< complex> fft2(ILInArray< complex> A, int m, int n) {
            using (ILScope.Enter(A)) {
                if (m < 0 || n < 0) throw new ILArgumentException("dimension length specifier 'm' and 'n' must be non-negative!");
                if (A.IsEmpty) return empty< complex>(A.Size);
                int[] Asize = A.Size.ToIntArray();
                Asize[0] = m; Asize[1] = n;
                ILRetArray< complex> resizedA = resize4Transform(A, Asize);
                return FFTImplementation.FFTForward(resizedA, 2);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (2D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="m">Transformation column length</param>
        /// <param name="n">Transformation row length</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The data to be transformed (based on the array A) are resized according to 
        /// the length parameter m and n. If m or n is larger then the length of the corresponding 
        /// dimension of A, zeros will be padded, otherwise the dimensions are truncated respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if one of n or m is negative</exception>
        public static ILRetArray< complex> ifft2(ILInArray< complex> A, int m, int n) {
            using (ILScope.Enter(A)) {
                if (m < 0 || n < 0) throw new ILArgumentException("dimension length specifier 'm' and 'n' must be non-negative!");
                if (A.IsEmpty) return empty< complex>(A.Size);
                int[] Asize = A.Size.ToIntArray();
                Asize[0] = m; Asize[1] = n;
                ILRetArray< complex> resizedA = resize4Transform(A, Asize);
                return FFTImplementation.FFTBackward(resizedA, 2);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (2D)
        /// </summary>
        /// <param name="A">Complex hermitian input array, symmetric in first 2 dimensions</param>
        /// <param name="m">Transformation column length</param>
        /// <param name="n">Transformation row length</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. The 
        /// lengths of those trailing dimensions are not altered.</para>
        /// <para>Since a transform of complex hermitian input data results in the 
        /// output having the imaginary part equals zero, only the real part is 
        /// returned for convenience reasons.</para>
        /// <para>No check is made for A being hermitian! If A is not hermitian (by means 
        /// of round-off errors), the result will be wrong!</para>
        /// <para>The data to be transformed (based on the array A) are resized according to 
        /// the length parameter m and n. If m or n is larger then the length of the corresponding 
        /// dimension of A, zeros will be padded, otherwise the dimensions are truncated respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if one of n or m is negative</exception>
        public static ILRetArray< double> ifft2sym(ILInArray< complex> A, int m, int n) {
            using (ILScope.Enter(A)) {
                if (m < 0 || n < 0) throw new ILArgumentException("dimension length specifier 'm' and 'n' must be non-negative!");
                if (A.IsEmpty) return empty< double>(A.Size);
                int[] Asize = A.Size.ToIntArray();
                Asize[0] = m; Asize[1] = n;
                ILRetArray< complex> resizedA = resize4Transform(A, Asize);
                return FFTImplementation.FFTBackwSym(resizedA, 2);
            }
        }
        #endregion

        #region fftn(A)
        /// <summary>
        /// Fast fourier transform (n-D)
        /// </summary>
        /// <param name="A">Input array, n-D</param>
        /// <returns>Transformation result, complex hermitian</returns>
        /// <remarks>
        /// <para>The n-dimensional transformation is computed for the n-dimensional array A. 
        /// This is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< complex> fftn(ILInArray< double> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex>(A.Size);
                return FFTImplementation.FFTForward(A, A.Size.NumberOfDimensions);
            }
        }
        /// <summary>
        /// Fast fourier transform (n-D)
        /// </summary>
        /// <param name="A">Input array, n-D</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The n-dimensional transformation is computed for the n-dimensional array A. 
        /// This is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< complex> fftn(ILInArray< complex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex>(A.Size);
                return FFTImplementation.FFTForward(A, A.Size.NumberOfDimensions);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (n-D)
        /// </summary>
        /// <param name="A">Input array, n-D (frequency domain)</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The n-dimensional inverse transformation is computed for the n-dimensional array A. 
        /// This is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< complex> ifftn(ILInArray< complex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex>(A.Size);
                return FFTImplementation.FFTBackward(A, A.Size.NumberOfDimensions);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (n-D)
        /// </summary>
        /// <param name="A">Input array, n-D, complex hermitian (frequency domain)</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The n-dimensional inverse transformation is computed for the n-dimensional array A. 
        /// This is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>Since a transform of complex hermitian input data results in the 
        /// output having the imaginary part equals zero, only the real part is 
        /// returned for convenience reasons.</para>
        /// <para>No check is made for A being hermitian! If A is not hermitian (by means 
        /// of round-off errors), the result will be wrong!</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< double> ifftnsym(ILInArray< complex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< double>(A.Size);
                return FFTImplementation.FFTBackwSym(A, A.Size.NumberOfDimensions);
            }
        }
        #endregion

        #region fftn(A, params dims)
        /// <summary>
        /// Fast fourier transform (n-D, specific size)
        /// </summary>
        /// <param name="A">Input array, n-D</param>
        /// <param name="dims">Transformation lengths, specifies the length of the dimensions 
        /// for the transformation array. The length of dims must be &gt; or equal to the number of 
        /// dimensions of A. For elements in dim being smaller than corresponding dimension 
        /// length in A, the dimensions will be truncated, otherwise zeros will be padded.</param>
        /// <returns>Transformation result of size specified by 'dims' parameter, complex hermitian</returns>
        /// <remarks>
        /// <para>The n-dimensional transformation is computed for the n-dimensional array A. 
        /// Before the transform, the input is resized according to the 'dims' parameter. 
        /// Dimensions larger than corresponding entries in 'dim' are truncated, dimensions
        /// smaller than corresponding entries in 'dim' are zero padded.</para>
        /// <para>The n-dimensional transformation is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if the 
        /// dim parameter is null, its length is less then the number of dimensions of A 
        /// or any element of dims is non-negative</exception>
        public static ILRetArray< complex> fftn(ILInArray< double> A, params int[] dims) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex>(A.Size);
                ILRetArray< double> resizedA = resize4Transform(A, dims);
                return FFTImplementation.FFTForward(resizedA, dims.Length);
            }
        }
        /// <summary>
        /// Fast fourier transform (n-D, specific size)
        /// </summary>
        /// <param name="A">Tnput array, n-D</param>
        /// <param name="dims">Transformation lengths, specifies the length of the dimensions 
        /// for the transformation array. The length of dims must be &gt; or equal to  the number of 
        /// dimensions of A. For elements in dim being smaller than corresponding dimension 
        /// length in A, the dimensions will be truncated, otherwise zeros will be padded.</param>
        /// <returns>Transformation result of size specified by 'dims' parameter</returns>
        /// <remarks>
        /// <para>The n-dimensional transformation is computed for the n-dimensional array A. 
        /// Before the transform, the input is resized according to the 'dims' parameter. 
        /// Dimensions larger than corresponding entries in 'dim' are truncated, dimensions
        /// smaller than corresponding entries in 'dim' are zero padded.</para>
        /// <para>The n-dimensional transformation is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if the 
        /// dim parameter is null, its length is less then the number of dimensions of A 
        /// or any element of dims is non-negative</exception>
        public static ILRetArray< complex> fftn(ILInArray< complex> A, params int[] dims) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex>(A.Size);
                ILRetArray< complex> resizedA = resize4Transform(A, dims);
                return FFTImplementation.FFTForward(resizedA, dims.Length);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (n-D, specific size)
        /// </summary>
        /// <param name="A">Input array, n-D</param>
        /// <param name="dims">Transformation lengths, specifies the length of the dimensions 
        /// for the transformation array. The length of dims must be &gt; or equal to  the number of 
        /// dimensions of A. For elements in dim being smaller than corresponding dimension 
        /// length in A, the dimensions will be truncated, otherwise zeros will be padded.</param>
        /// <returns>Transformation result of size specified by 'dims' parameter</returns>
        /// <remarks>
        /// <para>The n-dimensional transformation is computed for the n-dimensional array A. 
        /// Before the transform, the input is resized according to the 'dims' parameter. 
        /// Dimensions larger than corresponding entries in 'dim' are truncated, dimensions
        /// smaller than corresponding entries in 'dim' are zero padded.</para>
        /// <para>The n-dimensional transformation is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if the 
        /// dim parameter is null, its length is less then the number of dimensions of A 
        /// or any element of dims is non-negative</exception>
        public static ILRetArray< complex> ifftn(ILInArray< complex> A, params int[] dims) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex>(A.Size);
                ILRetArray< complex> resizedA = resize4Transform(A, dims);
                return FFTImplementation.FFTBackward(resizedA, dims.Length);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (n-D, complex hermitian, specific size)
        /// </summary>
        /// <param name="A">Complex hermitian input array, n-D</param>
        /// <param name="dims">Transformation lengths, specifies the length of the dimensions 
        /// for the transformation array. The length of dims must be &gt; or equal to  the number of 
        /// dimensions of A. For elements in dim being smaller than corresponding dimension 
        /// length in A, the dimensions will be truncated, otherwise zeros will be padded.</param>
        /// <returns>Transformation result, real array of the size specified by the 'dims' parameter</returns>
        /// <remarks>
        /// <para>The n-dimensional transformation is computed for the n-dimensional array A. 
        /// Before the transform, the input is resized according to the 'dims' parameter. 
        /// Dimensions larger than corresponding entries in 'dim' are truncated, dimensions
        /// smaller than corresponding entries in 'dim' are zero padded.</para>
        /// <para>The n-dimensional transformation is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>Since a transform of complex hermitian input data results in the 
        /// output having the imaginary part equals zero, only the real part is 
        /// returned for convenience reasons.</para>
        /// <para>No check is made for A being hermitian! If A is not hermitian (by means 
        /// of round-off errors), the result will be wrong!</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if the 
        /// dim parameter is null, its length is less then the number of dimensions of A 
        /// or any element of dims is non-negative</exception>
        public static ILRetArray< double> ifftnsym(ILInArray< complex> A, params int[] dims) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< double>(A.Size);
                ILRetArray< complex> resizedA = resize4Transform(A, dims);
                return FFTImplementation.FFTBackwSym(resizedA, dims.Length);
            }
        }
        #endregion


#region HYCALPER AUTO GENERATED CODE



        #region fft(A)
        /// <summary>
        /// Fast fourier transform (1D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Transformed output array</returns>
        /// <remarks><para>The transformation is computed along the first 
        /// non singleton dimension.</para>
        /// <para>The output array returned will be complex hermitian. I.e. the real 
        /// part being even and the imaginary part being odd symmetrical.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< fcomplex> fft(ILInArray< float> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                if (A.IsScalar) return new  fcomplex(A.GetValue(0), 0);
                int fnsd = A.Size.WorkingDimension();
                return ILMath.FFTImplementation.FFTForward1D(A, fnsd);
            }
        }
        /// <summary>
        /// Fast fourier transform (1D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Transformed output array</returns>
        /// <remarks>
        /// <para>The transformation is computed along the first non 
        /// singleton dimension.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< fcomplex > fft(ILInArray< fcomplex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                if (A.IsScalar) return A.C;
                int fnsd = A.Size.WorkingDimension();
                return ILMath.FFTImplementation.FFTForward1D(A, fnsd);
            }
        }
        /// <summary>
        /// Fast inverse fourier transform (1D)
        /// </summary>
        /// <param name="A">Input (frequency domain)</param>
        /// <returns>Inverse transformed output array</returns>
        /// <remarks>
        /// <para>The transformation is computed along the first non 
        /// singleton dimension.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray<fcomplex> ifft(ILInArray<fcomplex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                if (A.IsScalar) return A.C;
                int fnsd = A.Size.WorkingDimension();
                return ILMath.FFTImplementation.FFTBackward1D(A, fnsd);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform, complex hermitian input
        /// </summary>
        /// <param name="A">Complex hermitian input array</param>
        /// <returns>Real output array, same size as A</returns>
        /// <remarks>
        /// <para>Since a transform of complex hermitian input data results in 
        /// the output having all imaginary part equal zero, only the real part is 
        /// returned for convenience reasons.</para>
        /// <para>No check is made for A being hermitian! If A is not hermitian (by means 
        /// of round-off errors), the result will be wrong!</para>
        /// <para>The transformation is computed along the first non 
        /// singleton dimension.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< float > ifftsym(ILInArray< fcomplex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< float>(A.Size);
                if (A.IsScalar) return real(A);
                int fnsd = A.Size.WorkingDimension();
                return ILMath.FFTImplementation.FFTBackwSym1D(A, fnsd);
            }
        }
        #endregion

        #region fft(A, dim)
        /// <summary>
        /// Fast fourier transform along specific dimension
        /// </summary>
        /// <param name="A">Real input array</param>
        /// <param name="dim">Dimension to compute FFT along. This parameter must be non-negative. </param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The output array returned will be complex hermitian. I.e. the real 
        /// part being even and the imaginary part being odd symmetrical.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if 
        /// the dim parameter is negative</exception>
        public static ILRetArray< fcomplex > fft(ILInArray< float > A, int dim) {
            using (ILScope.Enter(A)) {
                if (dim < 0) throw new ILArgumentException("the 'dim' parameter must point to an existing dimension index of A");
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                if (A.IsScalar) return new  fcomplex(A.GetValue(0), 0);
                return ILMath.FFTImplementation.FFTForward1D(A, dim);
            }
        }
        /// <summary>
        /// Fast fourier transform along specific dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">Dimension to compute FFT along. This parameter
        /// must be non-negative. </param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if 
        /// the dim parameter is negative</exception>
        public static ILRetArray< fcomplex > fft(ILInArray< fcomplex > A, int dim) {
            using (ILScope.Enter(A)) {
                if (dim < 0) throw new ILArgumentException("the 'dim' parameter must point to an existing dimension index of A");
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                if (A.IsScalar) return A.C;
                return ILMath.FFTImplementation.FFTForward1D(A, dim); 
            }
        }
        /// <summary>
        /// Inverse fast fourier transform along specific dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">Dimension to compute FFT along. This parameter
        /// must be non-negative. </param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL 
        /// (included), AMD ACML and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if the dim parameter is negative</exception>
        public static ILRetArray< fcomplex> ifft(ILInArray< fcomplex> A, int dim) {
            using (ILScope.Enter(A)) {
                if (dim < 0) throw new ILArgumentException("the 'dim' parameter must point to an existing dimension index of A");
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                if (A.IsScalar) return A.C;
                return ILMath.FFTImplementation.FFTBackward1D(A, dim);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform, complex hermitian input
        /// </summary>
        /// <param name="A">Complex hermitian input array (frequency domain)</param>
        /// <param name="dim">Dimension to compute FFT along. This parameter
        /// must be non-negative. </param>
        /// <returns>Real output array, same size as A</returns>
        /// <remarks>
        /// <para>Since a transform of complex hermitian input data results in the 
        /// output having the imaginary part equals zero, only the real part is 
        /// returned for convenience reasons.</para>
        /// <para>No check is made for A being hermitian! If A is not hermitian (by means 
        /// of round-off errors), the result will be wrong!</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if the dim parameter is negative</exception>
        public static ILRetArray< float > ifftsym(ILInArray< fcomplex > A, int dim) {
            using (ILScope.Enter(A)) {
                if (dim < 0) throw new ILArgumentException("the 'dim' parameter must point to an existing dimension index of A");
                if (A.IsEmpty) return empty< float>(A.Size);
                if (A.IsScalar) return real(A);
                return ILMath.FFTImplementation.FFTBackwSym1D(A, dim);
            }
        }
        #endregion

        #region fft2(A)
        /// <summary>
        /// Fast fourier transform (2D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The 2D transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The output array returned will be complex hermitian.</para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< fcomplex > fft2(ILInArray< float > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                if (A.IsScalar) return new  fcomplex(A.GetValue(0), 0);
                return FFTImplementation.FFTForward(A, 2);
            } 
        }
        /// <summary>
        /// Fast fourier transform (2D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< fcomplex> fft2(ILInArray< fcomplex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                if (A.IsScalar) return A.C;
                return FFTImplementation.FFTForward(A, 2);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (2D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< fcomplex> ifft2(ILInArray< fcomplex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                if (A.IsScalar) return A.C;
                return FFTImplementation.FFTBackward(A, 2);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (2D, hermitian input)
        /// </summary>
        /// <param name="A">Complex hermitian input array (frequency domain)</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>Since a transform of complex hermitian input data results in the 
        /// output having the imaginary part equals zero, only the real part is 
        /// returned for convenience reasons.</para>
        /// <para>No check is made for A being hermitian! If A is not hermitian (by means 
        /// of round-off errors), the result will be wrong!</para>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< float> ifft2sym(ILInArray< fcomplex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< float>(A.Size);
                if (A.IsScalar) return real(A);
                return FFTImplementation.FFTBackwSym(A, 2);
            }
        }
        #endregion

        #region fft2(A,m,n)
        /// <summary>
        /// Fast fourier transform (2D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="m">Transformation column length</param>
        /// <param name="n">Transformation row length</param>
        /// <returns>Transformation result, complex hermitian</returns>
        /// <remarks>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The data to be transformed (based on the A array) are resized according to 
        /// the length parameter m and n. If m or n is larger then the length of the corresponding 
        /// dimension of A, zeros will be padded, otherwise the dimensions are truncated respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if one of n or m is negative</exception>
        public static ILRetArray< fcomplex> fft2(ILInArray< float> A, int m, int n) {
            using (ILScope.Enter(A)) {
                if (m < 0 || n < 0) throw new ILArgumentException("dimension length specifier 'm' and 'n' must be non-negative!");
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                int[] Asize = A.Size.ToIntArray();
                Asize[0] = m; Asize[1] = n;
                ILArray< float> resizedA = resize4Transform(A, Asize);
                return FFTImplementation.FFTForward(resizedA, 2);
            }
        }
        /// <summary>
        /// Fast fourier transform (2D)
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="m">Transformation column length</param>
        /// <param name="n">Transformation row length</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The data to be transformed (based on the A array) are resized according to 
        /// the length parameter m and n. If m or n is larger then the length of the corresponding 
        /// dimension of A, zeros will be padded, otherwise the dimensions are truncated respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if one of n or m is negative</exception>
        public static ILRetArray< fcomplex> fft2(ILInArray< fcomplex> A, int m, int n) {
            using (ILScope.Enter(A)) {
                if (m < 0 || n < 0) throw new ILArgumentException("dimension length specifier 'm' and 'n' must be non-negative!");
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                int[] Asize = A.Size.ToIntArray();
                Asize[0] = m; Asize[1] = n;
                ILRetArray< fcomplex> resizedA = resize4Transform(A, Asize);
                return FFTImplementation.FFTForward(resizedA, 2);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (2D)
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="m">Transformation column length</param>
        /// <param name="n">Transformation row length</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. </para>
        /// <para>The data to be transformed (based on the array A) are resized according to 
        /// the length parameter m and n. If m or n is larger then the length of the corresponding 
        /// dimension of A, zeros will be padded, otherwise the dimensions are truncated respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if one of n or m is negative</exception>
        public static ILRetArray< fcomplex> ifft2(ILInArray< fcomplex> A, int m, int n) {
            using (ILScope.Enter(A)) {
                if (m < 0 || n < 0) throw new ILArgumentException("dimension length specifier 'm' and 'n' must be non-negative!");
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                int[] Asize = A.Size.ToIntArray();
                Asize[0] = m; Asize[1] = n;
                ILRetArray< fcomplex> resizedA = resize4Transform(A, Asize);
                return FFTImplementation.FFTBackward(resizedA, 2);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (2D)
        /// </summary>
        /// <param name="A">Complex hermitian input array, symmetric in first 2 dimensions</param>
        /// <param name="m">Transformation column length</param>
        /// <param name="n">Transformation row length</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The transformation is computed for the first 2 dimensions, regardless 
        /// of those dimensions being singleton or non-singleton. If A is an n-d array, 
        /// the transformation is repeated for trailing dimensions of A respectively. The 
        /// lengths of those trailing dimensions are not altered.</para>
        /// <para>Since a transform of complex hermitian input data results in the 
        /// output having the imaginary part equals zero, only the real part is 
        /// returned for convenience reasons.</para>
        /// <para>No check is made for A being hermitian! If A is not hermitian (by means 
        /// of round-off errors), the result will be wrong!</para>
        /// <para>The data to be transformed (based on the array A) are resized according to 
        /// the length parameter m and n. If m or n is larger then the length of the corresponding 
        /// dimension of A, zeros will be padded, otherwise the dimensions are truncated respectively. </para>
        /// <para>The two dimensional transformation is equivalent to repeatedly transforming 
        /// the columns and after that transforming the rows of A. However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if one of n or m is negative</exception>
        public static ILRetArray< float> ifft2sym(ILInArray< fcomplex> A, int m, int n) {
            using (ILScope.Enter(A)) {
                if (m < 0 || n < 0) throw new ILArgumentException("dimension length specifier 'm' and 'n' must be non-negative!");
                if (A.IsEmpty) return empty< float>(A.Size);
                int[] Asize = A.Size.ToIntArray();
                Asize[0] = m; Asize[1] = n;
                ILRetArray< fcomplex> resizedA = resize4Transform(A, Asize);
                return FFTImplementation.FFTBackwSym(resizedA, 2);
            }
        }
        #endregion

        #region fftn(A)
        /// <summary>
        /// Fast fourier transform (n-D)
        /// </summary>
        /// <param name="A">Input array, n-D</param>
        /// <returns>Transformation result, complex hermitian</returns>
        /// <remarks>
        /// <para>The n-dimensional transformation is computed for the n-dimensional array A. 
        /// This is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< fcomplex> fftn(ILInArray< float> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                return FFTImplementation.FFTForward(A, A.Size.NumberOfDimensions);
            }
        }
        /// <summary>
        /// Fast fourier transform (n-D)
        /// </summary>
        /// <param name="A">Input array, n-D</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The n-dimensional transformation is computed for the n-dimensional array A. 
        /// This is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< fcomplex> fftn(ILInArray< fcomplex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                return FFTImplementation.FFTForward(A, A.Size.NumberOfDimensions);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (n-D)
        /// </summary>
        /// <param name="A">Input array, n-D (frequency domain)</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The n-dimensional inverse transformation is computed for the n-dimensional array A. 
        /// This is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< fcomplex> ifftn(ILInArray< fcomplex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                return FFTImplementation.FFTBackward(A, A.Size.NumberOfDimensions);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (n-D)
        /// </summary>
        /// <param name="A">Input array, n-D, complex hermitian (frequency domain)</param>
        /// <returns>Transformation result</returns>
        /// <remarks>
        /// <para>The n-dimensional inverse transformation is computed for the n-dimensional array A. 
        /// This is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>Since a transform of complex hermitian input data results in the 
        /// output having the imaginary part equals zero, only the real part is 
        /// returned for convenience reasons.</para>
        /// <para>No check is made for A being hermitian! If A is not hermitian (by means 
        /// of round-off errors), the result will be wrong!</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        public static ILRetArray< float> ifftnsym(ILInArray< fcomplex> A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< float>(A.Size);
                return FFTImplementation.FFTBackwSym(A, A.Size.NumberOfDimensions);
            }
        }
        #endregion

        #region fftn(A, params dims)
        /// <summary>
        /// Fast fourier transform (n-D, specific size)
        /// </summary>
        /// <param name="A">Input array, n-D</param>
        /// <param name="dims">Transformation lengths, specifies the length of the dimensions 
        /// for the transformation array. The length of dims must be &gt; or equal to the number of 
        /// dimensions of A. For elements in dim being smaller than corresponding dimension 
        /// length in A, the dimensions will be truncated, otherwise zeros will be padded.</param>
        /// <returns>Transformation result of size specified by 'dims' parameter, complex hermitian</returns>
        /// <remarks>
        /// <para>The n-dimensional transformation is computed for the n-dimensional array A. 
        /// Before the transform, the input is resized according to the 'dims' parameter. 
        /// Dimensions larger than corresponding entries in 'dim' are truncated, dimensions
        /// smaller than corresponding entries in 'dim' are zero padded.</para>
        /// <para>The n-dimensional transformation is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if the 
        /// dim parameter is null, its length is less then the number of dimensions of A 
        /// or any element of dims is non-negative</exception>
        public static ILRetArray< fcomplex> fftn(ILInArray< float> A, params int[] dims) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                ILRetArray< float> resizedA = resize4Transform(A, dims);
                return FFTImplementation.FFTForward(resizedA, dims.Length);
            }
        }
        /// <summary>
        /// Fast fourier transform (n-D, specific size)
        /// </summary>
        /// <param name="A">Tnput array, n-D</param>
        /// <param name="dims">Transformation lengths, specifies the length of the dimensions 
        /// for the transformation array. The length of dims must be &gt; or equal to  the number of 
        /// dimensions of A. For elements in dim being smaller than corresponding dimension 
        /// length in A, the dimensions will be truncated, otherwise zeros will be padded.</param>
        /// <returns>Transformation result of size specified by 'dims' parameter</returns>
        /// <remarks>
        /// <para>The n-dimensional transformation is computed for the n-dimensional array A. 
        /// Before the transform, the input is resized according to the 'dims' parameter. 
        /// Dimensions larger than corresponding entries in 'dim' are truncated, dimensions
        /// smaller than corresponding entries in 'dim' are zero padded.</para>
        /// <para>The n-dimensional transformation is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if the 
        /// dim parameter is null, its length is less then the number of dimensions of A 
        /// or any element of dims is non-negative</exception>
        public static ILRetArray< fcomplex> fftn(ILInArray< fcomplex> A, params int[] dims) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                ILRetArray< fcomplex> resizedA = resize4Transform(A, dims);
                return FFTImplementation.FFTForward(resizedA, dims.Length);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (n-D, specific size)
        /// </summary>
        /// <param name="A">Input array, n-D</param>
        /// <param name="dims">Transformation lengths, specifies the length of the dimensions 
        /// for the transformation array. The length of dims must be &gt; or equal to  the number of 
        /// dimensions of A. For elements in dim being smaller than corresponding dimension 
        /// length in A, the dimensions will be truncated, otherwise zeros will be padded.</param>
        /// <returns>Transformation result of size specified by 'dims' parameter</returns>
        /// <remarks>
        /// <para>The n-dimensional transformation is computed for the n-dimensional array A. 
        /// Before the transform, the input is resized according to the 'dims' parameter. 
        /// Dimensions larger than corresponding entries in 'dim' are truncated, dimensions
        /// smaller than corresponding entries in 'dim' are zero padded.</para>
        /// <para>The n-dimensional transformation is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if the 
        /// dim parameter is null, its length is less then the number of dimensions of A 
        /// or any element of dims is non-negative</exception>
        public static ILRetArray< fcomplex> ifftn(ILInArray< fcomplex> A, params int[] dims) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex>(A.Size);
                ILRetArray< fcomplex> resizedA = resize4Transform(A, dims);
                return FFTImplementation.FFTBackward(resizedA, dims.Length);
            }
        }
        /// <summary>
        /// Inverse fast fourier transform (n-D, complex hermitian, specific size)
        /// </summary>
        /// <param name="A">Complex hermitian input array, n-D</param>
        /// <param name="dims">Transformation lengths, specifies the length of the dimensions 
        /// for the transformation array. The length of dims must be &gt; or equal to  the number of 
        /// dimensions of A. For elements in dim being smaller than corresponding dimension 
        /// length in A, the dimensions will be truncated, otherwise zeros will be padded.</param>
        /// <returns>Transformation result, real array of the size specified by the 'dims' parameter</returns>
        /// <remarks>
        /// <para>The n-dimensional transformation is computed for the n-dimensional array A. 
        /// Before the transform, the input is resized according to the 'dims' parameter. 
        /// Dimensions larger than corresponding entries in 'dim' are truncated, dimensions
        /// smaller than corresponding entries in 'dim' are zero padded.</para>
        /// <para>The n-dimensional transformation is equivalent to repeatedly (inplace) 
        /// computing one dimensional transformations along all dimensions of A.
        /// However, using this 
        /// function may be of magnitudes faster than using 1D transformations. This 
        /// depends on the algorithm and API provided by the underlying native library.</para>
        /// <para>Since a transform of complex hermitian input data results in the 
        /// output having the imaginary part equals zero, only the real part is 
        /// returned for convenience reasons.</para>
        /// <para>No check is made for A being hermitian! If A is not hermitian (by means 
        /// of round-off errors), the result will be wrong!</para>
        /// <para>The forward fourier transform and the inverse fourier transform of 
        /// a given data array A are mathematically equivalent. It's only a 
        /// scaling factor which is needed to make sure, A equals ifft(fft(A)). That 
        /// scaling is introduced in the inverse transform.</para>
        /// <para>The transformation is computed by use of the native library 
        /// which currently is set up for your processor and OS version. The underlying 
        /// library is automatically choosen at ILNumerics startup and accessed via the 
        /// static member ILMath.FFT. See the online documentation for more 
        /// details in how to tune/configure and select dedicated native libraries. 
        /// Currently supported libraries are: Intel MKL (included), AMD ACML 
        /// and FFTW3 (prepared, optional modules, not included due to licensing conflicts).</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">is thrown if the 
        /// dim parameter is null, its length is less then the number of dimensions of A 
        /// or any element of dims is non-negative</exception>
        public static ILRetArray< float> ifftnsym(ILInArray< fcomplex> A, params int[] dims) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< float>(A.Size);
                ILRetArray< fcomplex> resizedA = resize4Transform(A, dims);
                return FFTImplementation.FFTBackwSym(resizedA, dims.Length);
            }
        }
        #endregion


#endregion HYCALPER AUTO GENERATED CODE

        #region private helper
        internal static ILRetArray<T> resize4Transform<T>(ILInArray<T> A, params int[] size) {
            using (ILScope.Enter(A)) {
                if (size == null || size.Length < A.Size.NumberOfDimensions)
                    throw new ILArgumentException("length of output dimensions must be &gt; or equal to number of dimensions of input array!");
                ILSize newDimensions = new ILSize(size);
                if (A.Size.IsSameShape(newDimensions)) {
                    return A;
                } else {
                    if (newDimensions.NumberOfElements == 0) return empty<T>(newDimensions);
                    ILArray<T> tmp = array<T>(default(T), newDimensions);
                    int minDimsLen = Math.Min(size.Length, A.Size.NumberOfDimensions);
                    ILArray<ILRegularRange>[] indices = new ILArray<ILRegularRange>[minDimsLen];
                    for (int i = 0; i < minDimsLen; i++) {
                        if (size[i] < 0)
                            throw new ILArgumentException("all dimension lengths of 'size' must be non-negative!");
                        if (size[i] == 0) return empty<T>(newDimensions);
                        indices[i] = (ILRetArray<ILRegularRange>)r(0, Math.Min(A.Size[i] - 1, size[i] - 1));
                    }
                    tmp[indices] = A[indices];
                    return tmp;
                }
            }
        }

        #endregion

    }
}
