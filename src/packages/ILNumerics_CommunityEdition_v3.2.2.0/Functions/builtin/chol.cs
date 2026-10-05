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



namespace ILNumerics {

    public partial class ILMath {


        /// <summary>Cholesky factorization</summary>
        /// <param name="A">Input array A. A must be a symmetric/hermitian matrix. 
        /// Therefore the upper triangular part of A must be the transpose (complex conjugate for complex input) of the lower triangular part. No check 
        /// is made for that! <br/> 
        /// The elements of A will not be altered.</param>
        /// <param name="throwException">Throw an ILArgumentException if A 
        /// is found not to be positive definite.</param>
        /// <returns>Cholesky factorization</returns>
        /// <remarks><para>If <paramref name="throwException"/> is true and 
        /// A is found not to be positive definite, an ILArgumentException 
        /// will be thrown and the operation will be canceled.</para>
        /// <para>If <paramref name="throwException"/> is false, check the
        /// return value's dimension to determine the success of the 
        /// operation (unless you are sure, A was positive definite). 
        /// If A was found not to be positive definite the matrix returned 
        /// will be of dimension [k x k] and the result of the cholesky 
        /// factorization of A[0:k-1;0:k-1]. Here k is the first leading 
        /// minor of A at which A was found to be not positive definite.</para>
        /// The factorization is carried out by use of the LAPACK functions 
        /// DPOTRF, ZPOTRF, SPOTRF or CPOTRF respectively. </remarks>
        public static ILRetArray<double> chol(ILInArray<double> A, bool throwException = true) {
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentSizeException("chol is defined for matrices only!");
                int n = A.Size[0], info = 0; 
                if (A.Size[1] != n) 
                    throw new ILArgumentException("chol: input matrix must be square!");
                ILDenseStorage<double> ret = A.Storage.copyUpperTriangle(n);
                /*!HC:lapack_*potrf*/ Lapack.dpotrf ('U', n, ret.GetArrayForRead(), n, ref info); 
                if (info < 0 ) {
                    throw new ILArgumentException("chol: illegal parameter error.");
                } else if (info > 0) {
                    // not pos.definite 
                    if (! throwException) {
                        if (info > 1) {
                            int newDim = info - 1;
                            ret = A.Storage.copyUpperTriangle(newDim); 
                            /*!HC:lapack_*potrf*/  Lapack.dpotrf ('U',newDim,ret.GetArrayForRead(), newDim, ref info);
                            if (info != 0) 
                                throw new ILArgumentException("chol: the original matrix given was not pos. definit. An attempt to decompose the submatrix of order " + (newDim + 1).ToString() + " failed."); 
                            return new ILRetArray<double>(ret); 
                        } else {
                            return empty<double>(ILSize.Empty00); 
                        } 
                    } else {
                        throw new ILArgumentException("chol: the matrix given is not positive definite!"); 
                    }
                }
                return new ILRetArray<double>(ret);
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>Cholesky factorization</summary>
        /// <param name="A">Input array A. A must be a symmetric/hermitian matrix. 
        /// Therefore the upper triangular part of A must be the transpose (complex conjugate for complex input) of the lower triangular part. No check 
        /// is made for that! <br/> 
        /// The elements of A will not be altered.</param>
        /// <param name="throwException">Throw an ILArgumentException if A 
        /// is found not to be positive definite.</param>
        /// <returns>Cholesky factorization</returns>
        /// <remarks><para>If <paramref name="throwException"/> is true and 
        /// A is found not to be positive definite, an ILArgumentException 
        /// will be thrown and the operation will be canceled.</para>
        /// <para>If <paramref name="throwException"/> is false, check the
        /// return value's dimension to determine the success of the 
        /// operation (unless you are sure, A was positive definite). 
        /// If A was found not to be positive definite the matrix returned 
        /// will be of dimension [k x k] and the result of the cholesky 
        /// factorization of A[0:k-1;0:k-1]. Here k is the first leading 
        /// minor of A at which A was found to be not positive definite.</para>
        /// The factorization is carried out by use of the LAPACK functions 
        /// DPOTRF, ZPOTRF, SPOTRF or CPOTRF respectively. </remarks>
        public static ILRetArray<float> chol(ILInArray<float> A, bool throwException = true) {
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentSizeException("chol is defined for matrices only!");
                int n = A.Size[0], info = 0; 
                if (A.Size[1] != n) 
                    throw new ILArgumentException("chol: input matrix must be square!");
                ILDenseStorage<float> ret = A.Storage.copyUpperTriangle(n);
                Lapack.spotrf ('U', n, ret.GetArrayForRead(), n, ref info); 
                if (info < 0 ) {
                    throw new ILArgumentException("chol: illegal parameter error.");
                } else if (info > 0) {
                    // not pos.definite 
                    if (! throwException) {
                        if (info > 1) {
                            int newDim = info - 1;
                            ret = A.Storage.copyUpperTriangle(newDim); 
                             Lapack.spotrf ('U',newDim,ret.GetArrayForRead(), newDim, ref info);
                            if (info != 0) 
                                throw new ILArgumentException("chol: the original matrix given was not pos. definit. An attempt to decompose the submatrix of order " + (newDim + 1).ToString() + " failed."); 
                            return new ILRetArray<float>(ret); 
                        } else {
                            return empty<float>(ILSize.Empty00); 
                        } 
                    } else {
                        throw new ILArgumentException("chol: the matrix given is not positive definite!"); 
                    }
                }
                return new ILRetArray<float>(ret);
            }
        }
        /// <summary>Cholesky factorization</summary>
        /// <param name="A">Input array A. A must be a symmetric/hermitian matrix. 
        /// Therefore the upper triangular part of A must be the transpose (complex conjugate for complex input) of the lower triangular part. No check 
        /// is made for that! <br/> 
        /// The elements of A will not be altered.</param>
        /// <param name="throwException">Throw an ILArgumentException if A 
        /// is found not to be positive definite.</param>
        /// <returns>Cholesky factorization</returns>
        /// <remarks><para>If <paramref name="throwException"/> is true and 
        /// A is found not to be positive definite, an ILArgumentException 
        /// will be thrown and the operation will be canceled.</para>
        /// <para>If <paramref name="throwException"/> is false, check the
        /// return value's dimension to determine the success of the 
        /// operation (unless you are sure, A was positive definite). 
        /// If A was found not to be positive definite the matrix returned 
        /// will be of dimension [k x k] and the result of the cholesky 
        /// factorization of A[0:k-1;0:k-1]. Here k is the first leading 
        /// minor of A at which A was found to be not positive definite.</para>
        /// The factorization is carried out by use of the LAPACK functions 
        /// DPOTRF, ZPOTRF, SPOTRF or CPOTRF respectively. </remarks>
        public static ILRetArray<fcomplex> chol(ILInArray<fcomplex> A, bool throwException = true) {
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentSizeException("chol is defined for matrices only!");
                int n = A.Size[0], info = 0; 
                if (A.Size[1] != n) 
                    throw new ILArgumentException("chol: input matrix must be square!");
                ILDenseStorage<fcomplex> ret = A.Storage.copyUpperTriangle(n);
                Lapack.cpotrf ('U', n, ret.GetArrayForRead(), n, ref info); 
                if (info < 0 ) {
                    throw new ILArgumentException("chol: illegal parameter error.");
                } else if (info > 0) {
                    // not pos.definite 
                    if (! throwException) {
                        if (info > 1) {
                            int newDim = info - 1;
                            ret = A.Storage.copyUpperTriangle(newDim); 
                             Lapack.cpotrf ('U',newDim,ret.GetArrayForRead(), newDim, ref info);
                            if (info != 0) 
                                throw new ILArgumentException("chol: the original matrix given was not pos. definit. An attempt to decompose the submatrix of order " + (newDim + 1).ToString() + " failed."); 
                            return new ILRetArray<fcomplex>(ret); 
                        } else {
                            return empty<fcomplex>(ILSize.Empty00); 
                        } 
                    } else {
                        throw new ILArgumentException("chol: the matrix given is not positive definite!"); 
                    }
                }
                return new ILRetArray<fcomplex>(ret);
            }
        }
        /// <summary>Cholesky factorization</summary>
        /// <param name="A">Input array A. A must be a symmetric/hermitian matrix. 
        /// Therefore the upper triangular part of A must be the transpose (complex conjugate for complex input) of the lower triangular part. No check 
        /// is made for that! <br/> 
        /// The elements of A will not be altered.</param>
        /// <param name="throwException">Throw an ILArgumentException if A 
        /// is found not to be positive definite.</param>
        /// <returns>Cholesky factorization</returns>
        /// <remarks><para>If <paramref name="throwException"/> is true and 
        /// A is found not to be positive definite, an ILArgumentException 
        /// will be thrown and the operation will be canceled.</para>
        /// <para>If <paramref name="throwException"/> is false, check the
        /// return value's dimension to determine the success of the 
        /// operation (unless you are sure, A was positive definite). 
        /// If A was found not to be positive definite the matrix returned 
        /// will be of dimension [k x k] and the result of the cholesky 
        /// factorization of A[0:k-1;0:k-1]. Here k is the first leading 
        /// minor of A at which A was found to be not positive definite.</para>
        /// The factorization is carried out by use of the LAPACK functions 
        /// DPOTRF, ZPOTRF, SPOTRF or CPOTRF respectively. </remarks>
        public static ILRetArray<complex> chol(ILInArray<complex> A, bool throwException = true) {
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentSizeException("chol is defined for matrices only!");
                int n = A.Size[0], info = 0; 
                if (A.Size[1] != n) 
                    throw new ILArgumentException("chol: input matrix must be square!");
                ILDenseStorage<complex> ret = A.Storage.copyUpperTriangle(n);
                Lapack.zpotrf ('U', n, ret.GetArrayForRead(), n, ref info); 
                if (info < 0 ) {
                    throw new ILArgumentException("chol: illegal parameter error.");
                } else if (info > 0) {
                    // not pos.definite 
                    if (! throwException) {
                        if (info > 1) {
                            int newDim = info - 1;
                            ret = A.Storage.copyUpperTriangle(newDim); 
                             Lapack.zpotrf ('U',newDim,ret.GetArrayForRead(), newDim, ref info);
                            if (info != 0) 
                                throw new ILArgumentException("chol: the original matrix given was not pos. definit. An attempt to decompose the submatrix of order " + (newDim + 1).ToString() + " failed."); 
                            return new ILRetArray<complex>(ret); 
                        } else {
                            return empty<complex>(ILSize.Empty00); 
                        } 
                    } else {
                        throw new ILArgumentException("chol: the matrix given is not positive definite!"); 
                    }
                }
                return new ILRetArray<complex>(ret);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}
