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
        /// Multiplicate an arbitrary number of matrices from left to right
        /// </summary>
        /// <param name="matrices">Input matrices </param>
        /// <returns>Result of matrix multiplication for all matrices</returns>
        public static ILRetArray< double> multiply(params ILInArray< double>[] matrices) {
            if (matrices == null || matrices.Length < 2) 
                throw new ILArgumentException("the number of matching parameters for multiply must be at least 2"); 
            using (ILScope.Enter(matrices)) {
                ILArray< double> ret = multiply(matrices[0], matrices[1]); 
                for (int i = 2; i < matrices.Length; i++) {
                    ret.a = multiply(ret, matrices[i]); 
                }
                return ret; 
            }
        }

        /// <summary>
        /// General matrix multiply this array
        /// </summary>
        /// <param name="A">Input matrix A</param>
        /// <param name="B">Input matrix B</param>
        /// <returns>Matrix with result of matrix multiplication</returns>
        /// <remarks>Both arrays must be matrices with matching dimension length. Therefore the number of rows 
        /// of B must equal the number of columns of A. An ILArgumentSizeException will be thrown otherwise. 
        /// The multiplication will be carried out inside optimized BLAS libraries if availiable. If not it 
        /// will be done in managed code. 
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException">If at least one arrays is not a matrix</exception>
        /// <exception cref="ILNumerics.Exceptions.ILDimensionMismatchException">If the size of both matrices do not match</exception>
        public static ILRetArray< double > multiply(ILInArray< double > A, ILInArray< double > B) {
            using (ILScope.Enter(A,B)) {
                if (A.Size[1] != B.Size[0])
                    throw new ILArgumentException("inner matrix dimensions must match");

                ILArray<double> ret = zeros<double>(size(A.Size[0], B.Size[1])); 
                
                double[] retArr = ret.GetArrayForWrite(); 

                if (A.Size.NumberOfElements > Settings.ManagedMultiplyMaxElementSize ||
                    B.Size.NumberOfElements > Settings.ManagedMultiplyMaxElementSize) {
                    // do BLAS GEMM 
                    unsafe {
                        fixed ( double* ptrC = retArr)
                        fixed ( double* pA = A.GetArrayForRead())
                        fixed ( double* pB = B.GetArrayForRead()) {
                            
                            Lapack.dgemm(TRANS_NONE, TRANS_NONE,
                                A.Size[0], B.Size[1],
                                A.Size[1], ( double)1.0,
                                (IntPtr)pA, A.Size[0],
                                (IntPtr)pB, B.Size[0], ( double)1.0,
                                retArr, A.Size[0]);
                        }
                    }
                } else {
                    if (Settings.MaxNumberThreads > 1) {
                        MMultBlockedThreaded(A.GetArrayForRead(), B.GetArrayForRead(), retArr, A.S[0], B.S[1], A.S[1], Settings.ManagedMultiplyBlockSize);
                    } else {
                        MMultBlocked        (A.GetArrayForRead(), B.GetArrayForRead(), retArr, A.S[0], B.S[1], A.S[1], Settings.ManagedMultiplyBlockSize);
                    }
                }
                return ret;
            }
        }


#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Multiplicate an arbitrary number of matrices from left to right
        /// </summary>
        /// <param name="matrices">Input matrices </param>
        /// <returns>Result of matrix multiplication for all matrices</returns>
        public static ILRetArray< float> multiply(params ILInArray< float>[] matrices) {
            if (matrices == null || matrices.Length < 2) 
                throw new ILArgumentException("the number of matching parameters for multiply must be at least 2"); 
            using (ILScope.Enter(matrices)) {
                ILArray< float> ret = multiply(matrices[0], matrices[1]); 
                for (int i = 2; i < matrices.Length; i++) {
                    ret.a = multiply(ret, matrices[i]); 
                }
                return ret; 
            }
        }

        /// <summary>
        /// General matrix multiply this array
        /// </summary>
        /// <param name="A">Input matrix A</param>
        /// <param name="B">Input matrix B</param>
        /// <returns>Matrix with result of matrix multiplication</returns>
        /// <remarks>Both arrays must be matrices with matching dimension length. Therefore the number of rows 
        /// of B must equal the number of columns of A. An ILArgumentSizeException will be thrown otherwise. 
        /// The multiplication will be carried out inside optimized BLAS libraries if availiable. If not it 
        /// will be done in managed code. 
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException">If at least one arrays is not a matrix</exception>
        /// <exception cref="ILNumerics.Exceptions.ILDimensionMismatchException">If the size of both matrices do not match</exception>
        public static ILRetArray< float > multiply(ILInArray< float > A, ILInArray< float > B) {
            using (ILScope.Enter(A,B)) {
                if (A.Size[1] != B.Size[0])
                    throw new ILArgumentException("inner matrix dimensions must match");

                ILArray<float> ret = zeros<float>(size(A.Size[0], B.Size[1])); 
               
                float[] retArr = ret.GetArrayForWrite(); 

                if (A.Size.NumberOfElements > Settings.ManagedMultiplyMaxElementSize ||
                    B.Size.NumberOfElements > Settings.ManagedMultiplyMaxElementSize) {
                    // do BLAS GEMM 
                    unsafe {
                        fixed ( float* ptrC = retArr)
                        fixed ( float* pA = A.GetArrayForRead())
                        fixed ( float* pB = B.GetArrayForRead()) {
                           
                            Lapack.sgemm(TRANS_NONE, TRANS_NONE,
                                A.Size[0], B.Size[1],
                                A.Size[1], ( float)1.0,
                                (IntPtr)pA, A.Size[0],
                                (IntPtr)pB, B.Size[0], ( float)1.0,
                                retArr, A.Size[0]);
                        }
                    }
                } else {
                    if (Settings.MaxNumberThreads > 1) {
                        MMultBlockedThreaded(A.GetArrayForRead(), B.GetArrayForRead(), retArr, A.S[0], B.S[1], A.S[1], Settings.ManagedMultiplyBlockSize);
                    } else {
                        MMultBlocked        (A.GetArrayForRead(), B.GetArrayForRead(), retArr, A.S[0], B.S[1], A.S[1], Settings.ManagedMultiplyBlockSize);
                    }
                }
                return ret;
            }
        }

        /// <summary>
        /// Multiplicate an arbitrary number of matrices from left to right
        /// </summary>
        /// <param name="matrices">Input matrices </param>
        /// <returns>Result of matrix multiplication for all matrices</returns>
        public static ILRetArray< fcomplex> multiply(params ILInArray< fcomplex>[] matrices) {
            if (matrices == null || matrices.Length < 2) 
                throw new ILArgumentException("the number of matching parameters for multiply must be at least 2"); 
            using (ILScope.Enter(matrices)) {
                ILArray< fcomplex> ret = multiply(matrices[0], matrices[1]); 
                for (int i = 2; i < matrices.Length; i++) {
                    ret.a = multiply(ret, matrices[i]); 
                }
                return ret; 
            }
        }

        /// <summary>
        /// General matrix multiply this array
        /// </summary>
        /// <param name="A">Input matrix A</param>
        /// <param name="B">Input matrix B</param>
        /// <returns>Matrix with result of matrix multiplication</returns>
        /// <remarks>Both arrays must be matrices with matching dimension length. Therefore the number of rows 
        /// of B must equal the number of columns of A. An ILArgumentSizeException will be thrown otherwise. 
        /// The multiplication will be carried out inside optimized BLAS libraries if availiable. If not it 
        /// will be done in managed code. 
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException">If at least one arrays is not a matrix</exception>
        /// <exception cref="ILNumerics.Exceptions.ILDimensionMismatchException">If the size of both matrices do not match</exception>
        public static ILRetArray< fcomplex > multiply(ILInArray< fcomplex > A, ILInArray< fcomplex > B) {
            using (ILScope.Enter(A,B)) {
                if (A.Size[1] != B.Size[0])
                    throw new ILArgumentException("inner matrix dimensions must match");

                ILArray<fcomplex> ret = zeros<fcomplex>(size(A.Size[0], B.Size[1])); 
               
                fcomplex[] retArr = ret.GetArrayForWrite(); 

                if (A.Size.NumberOfElements > Settings.ManagedMultiplyMaxElementSize ||
                    B.Size.NumberOfElements > Settings.ManagedMultiplyMaxElementSize) {
                    // do BLAS GEMM 
                    unsafe {
                        fixed ( fcomplex* ptrC = retArr)
                        fixed ( fcomplex* pA = A.GetArrayForRead())
                        fixed ( fcomplex* pB = B.GetArrayForRead()) {
                           
                            Lapack.cgemm(TRANS_NONE, TRANS_NONE,
                                A.Size[0], B.Size[1],
                                A.Size[1], ( fcomplex)1.0,
                                (IntPtr)pA, A.Size[0],
                                (IntPtr)pB, B.Size[0], ( fcomplex)1.0,
                                retArr, A.Size[0]);
                        }
                    }
                } else {
                    if (Settings.MaxNumberThreads > 1) {
                        MMultBlockedThreaded(A.GetArrayForRead(), B.GetArrayForRead(), retArr, A.S[0], B.S[1], A.S[1], Settings.ManagedMultiplyBlockSize);
                    } else {
                        MMultBlocked        (A.GetArrayForRead(), B.GetArrayForRead(), retArr, A.S[0], B.S[1], A.S[1], Settings.ManagedMultiplyBlockSize);
                    }
                }
                return ret;
            }
        }

        /// <summary>
        /// Multiplicate an arbitrary number of matrices from left to right
        /// </summary>
        /// <param name="matrices">Input matrices </param>
        /// <returns>Result of matrix multiplication for all matrices</returns>
        public static ILRetArray< complex> multiply(params ILInArray< complex>[] matrices) {
            if (matrices == null || matrices.Length < 2) 
                throw new ILArgumentException("the number of matching parameters for multiply must be at least 2"); 
            using (ILScope.Enter(matrices)) {
                ILArray< complex> ret = multiply(matrices[0], matrices[1]); 
                for (int i = 2; i < matrices.Length; i++) {
                    ret.a = multiply(ret, matrices[i]); 
                }
                return ret; 
            }
        }

        /// <summary>
        /// General matrix multiply this array
        /// </summary>
        /// <param name="A">Input matrix A</param>
        /// <param name="B">Input matrix B</param>
        /// <returns>Matrix with result of matrix multiplication</returns>
        /// <remarks>Both arrays must be matrices with matching dimension length. Therefore the number of rows 
        /// of B must equal the number of columns of A. An ILArgumentSizeException will be thrown otherwise. 
        /// The multiplication will be carried out inside optimized BLAS libraries if availiable. If not it 
        /// will be done in managed code. 
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException">If at least one arrays is not a matrix</exception>
        /// <exception cref="ILNumerics.Exceptions.ILDimensionMismatchException">If the size of both matrices do not match</exception>
        public static ILRetArray< complex > multiply(ILInArray< complex > A, ILInArray< complex > B) {
            using (ILScope.Enter(A,B)) {
                if (A.Size[1] != B.Size[0])
                    throw new ILArgumentException("inner matrix dimensions must match");

                ILArray<complex> ret = zeros<complex>(size(A.Size[0], B.Size[1])); 
               
                complex[] retArr = ret.GetArrayForWrite(); 

                if (A.Size.NumberOfElements > Settings.ManagedMultiplyMaxElementSize ||
                    B.Size.NumberOfElements > Settings.ManagedMultiplyMaxElementSize) {
                    // do BLAS GEMM 
                    unsafe {
                        fixed ( complex* ptrC = retArr)
                        fixed ( complex* pA = A.GetArrayForRead())
                        fixed ( complex* pB = B.GetArrayForRead()) {
                           
                            Lapack.zgemm(TRANS_NONE, TRANS_NONE,
                                A.Size[0], B.Size[1],
                                A.Size[1], ( complex)1.0,
                                (IntPtr)pA, A.Size[0],
                                (IntPtr)pB, B.Size[0], ( complex)1.0,
                                retArr, A.Size[0]);
                        }
                    }
                } else {
                    if (Settings.MaxNumberThreads > 1) {
                        MMultBlockedThreaded(A.GetArrayForRead(), B.GetArrayForRead(), retArr, A.S[0], B.S[1], A.S[1], Settings.ManagedMultiplyBlockSize);
                    } else {
                        MMultBlocked        (A.GetArrayForRead(), B.GetArrayForRead(), retArr, A.S[0], B.S[1], A.S[1], Settings.ManagedMultiplyBlockSize);
                    }
                }
                return ret;
            }
        }


#endregion HYCALPER AUTO GENERATED CODE

    }
}
