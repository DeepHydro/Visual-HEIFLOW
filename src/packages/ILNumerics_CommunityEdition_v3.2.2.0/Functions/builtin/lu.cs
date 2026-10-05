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


        /// <summary>
        /// LU matrix decomposition. Decompose general matrix A into strictly upper part and lower part. 
        /// </summary>
        /// <param name="A">Input matrix. Size [m x n]</param>
        /// <returns>Triangular matrices L and U composed into a single matrix as returned from LAPACK function ?getrf. Size [m x n]</returns>
        /// <remarks><para>The matrix returned is composed out of the lower triangular matrix L with unit diagonal and the strict upper triangular matrix U.</para>
        /// <code>
        /// :'''''''|
        /// |1 \    |
        /// | 1 \ R |
        /// |  1 \  |
        /// | L 1 \ |
        /// |    1 \|
        /// '''''''''
        /// </code>
        /// <para>This overload is mainly needed for further operations via Lapack libraries. If you need the 
        /// L and U matrices directly, you'd better use one of the overloaded versions 
        /// <see cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double})"/>
        ///  or <see cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/> instead.</para>
        /// <para>The matrix L will be a solid ILArray.</para>
        /// <para>lu uses the Lapack function ?getrf.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double})"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException"> if input A is not a matrix.</exception>
        public static ILRetArray< double > lu(ILInArray<double> A) {
            ILArray< double > U = null;
            ILArray< double > P = null; 
            return lu(A, U, P); 
        }
        /// <summary>
        /// LU matrix decomposition. Decompose general matrix A into strictly upper part and lower part.
        /// </summary>
        /// <param name="A">Input matrix to be decomposed. Size [m x n]</param>
        /// <param name="U">[Output] Reference to U. On return this will be the strict upper triangular matrix of size [min(m,n) x n]. Must not be null on input.</param>
        /// <returns>Lower triangular matrix L of size [m x min(m,n)]</returns>
        /// <remarks>A is decomposed into L and U, so that ILMath.multiply (L,U) will result in A.
        /// <para>L will only be a permuted version of a true triangular matrix. I.e. the rows of L will be permuted in order 
        /// to fullfill <c>ILMath.multiply(L,U) == A</c></para>
        /// <example> <code>
        /// //we construct a matrix X: 
        /// ILArray&lt;double&gt; X = new ILArray&lt;double&gt;(new double[]{1, 2, 3, 4, 4, 4, 5, 6, 7},3,3).T;
        /// // now X.ToString() will give something like:
        /// // {&lt;Double&gt; 63238509 [3x3] Ref(2) 
        /// //(:,:) 
        /// // 1,00000  2,00000  3,00000 
        /// // 4,00000  4,00000  4,00000 
        /// // 5,00000  6,00000  7,00000 
        /// //}
        /// // construct reference on U and call the decomposition
        /// ILArray&lt;double&gt; U = new ILArray&lt;double&gt;.empty();
        /// ILArray&lt;double&gt; L = ILMath.lu(X, ref U); 
        /// 
        /// // L.ToString() is now: 
        /// // {&lt;Double&gt; 19634871 [3x3] Phys. 
        /// //(:,:) 
        /// // 0,20000  -1,00000  1,00000 
        /// // 0,80000  1,00000  0,00000 
        /// // 1,00000  0,00000  0,00000 
        /// //}
        /// // and U is now: 
        /// //{&lt;Double&gt; 22584602 [3x3] Phys. 
        /// //(:,:) 
        /// // 5,00000  6,00000  7,00000 
        /// // 0,00000  -0,80000  -1,60000 
        /// // 0,00000  0,00000  0,00000 
        /// //}
        /// </code>
        /// Pay attention to the structure of L. In the example above the first and third row are exchanged. This permutation reflects the pivoting done during the decomposition inside the Lapack function ?getrf. </example>
        /// <para>In order to access the permutation of L, one can use the overloaded version <see cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/> which returns the permuation matrix P also.</para>
        /// <para>All of the matrices U and L returned will be solid ILArrays.</para>
        /// <para>lu uses the Lapack function ?getrf.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double})"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException"> if input A is not a matrix.</exception>
        public static ILRetArray< double> lu(ILInArray<double> A, ILOutArray<double> U) {
            return lu(A,U,null); 
        }
        /// <summary>
        /// Decompose matrix A into uper and lower triangular part. Returns permutation matrix also. 
        /// </summary>
        /// <param name="A">Input matrix. Size [m x n]</param>
        /// <param name="U">[Output] Reference to upper triangular matrix. Size [min(m,n) x n]. Must not be null.</param>
        /// <param name="P">[Output] Reference to permutation matrix. Size [min(m,n) x min(m,n)]. Must not be null.</param>
        /// <returns>Lower triangular matrix L of size [m x min(m,n)]</returns>
        /// <remarks>A is decomposed into L and U, so that the equation 
        /// <c>ILMath.multiply(L,U) == ILMath.multiply(P,A)</c> 
        /// will hold except for round off error.
        /// <para>L and U will be true lower triangular matrices.</para>
        /// <example> <code>
        /// //Let's construct a matrix X: 
        /// ILArray&lt;double&gt; X = new ILArray&lt;double&gt;(new double[]{1, 2, 3, 4, 4, 4, 5, 6, 7},3,3).T;
        /// // now X.ToString() will give something like:
        /// // {&lt;Double&gt; 63238509 [3x3] Ref(2) 
        /// //(:,:) 
        /// // 1,00000  2,00000  3,00000 
        /// // 4,00000  4,00000  4,00000 
        /// // 5,00000  6,00000  7,00000 
        /// //}
        /// // construct references on U and P and call the decomposition
        /// ILArray&lt;double&gt; U = new ILArray&lt;double&gt;.empty();
        /// ILArray&lt;double&gt; P = new ILArray&lt;double&gt;.empty();
        /// ILArray&lt;double&gt; L = ILMath.lu(X, ref U, ref P); 
        /// 
        /// // L.ToString() is now: 
        /// // {&lt;Double&gt; 19634871 [3x3] Phys. 
        /// //(:,:) 
        /// // 1,00000   0,00000   0,00000 
        /// // 0,80000   1,00000   0,00000 
        /// // 0,20000  -1,00000   1,00000 
        /// //}
        /// // U is now: 
        /// //{&lt;Double&gt; 22584602 [3x3] Phys. 
        /// //(:,:) 
        /// // 5,00000  6,00000  7,00000 
        /// // 0,00000  -0,80000  -1,60000 
        /// // 0,00000  0,00000  0,00000 
        /// //}
        /// // and P is: 
        /// //{&lt;Double&gt; 2192437 [3x3] Phys. 
        /// //(:,:) 
        /// // 0,00000  0,00000  1,00000 
        /// // 0,00000  1,00000  0,00000 
        /// // 1,00000  0,00000  0,00000 
        /// //}
        /// </code>
        /// In order to reflect the pivoting done during the decomposition inside ?getrf, the matrix P may be used on A:
        /// <code>
        /// (ILMath.multiply(P,A) - ILMath.multiply(L,U)).ToString();
        /// // will give:
        /// //{&lt;Double&gt; 59192235 [3x3] Phys. 
        /// //(:,:) 
        /// // 0,00000  0,00000  0,00000 
        /// // 0,00000  0,00000  0,00000 
        /// // 0,00000  0,00000  0,00000 
        /// //}
        /// </code>
        /// </example>
        /// <para>lu uses the Lapack function ?getrf.</para>
        /// <para>All of the matrices U,L,P returned will be solid ILArrays.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double})"/>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double})"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException"> if input A is not a matrix.</exception>
        public static ILRetArray< double> lu(ILInArray<double> A, ILOutArray<double> U
                                                            , ILOutArray< double> P) {
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentSizeException("lu is defined for matrices only!");
                int m = A.Size[0], n = A.Size[1], info = 0, minMN = (m < n) ? m : n;
                ILArray< double> L = A.C;
                int[] pivInd = ILMemoryPool.Pool.New<int>(minMN);
                /*!HC:lapack_*getrf*/
                Lapack.dgetrf(m, n, L.GetArrayForWrite(), m, pivInd, ref info);
                if (info < 0) {
                    ILMemoryPool.Pool.Free(pivInd);
                    throw new ILArgumentException("invalid parameter");
                    //} else if (info > 0) {
                    //    // singular diagonal entry found 

                } else {
                    // completed successfuly 
                    if (!Object.Equals(U, null)) {
                        if (!Object.Equals(P, null)) {
                            pivInd = perm2indicesForward(pivInd);
                            U.a = copyUpperTriangle<double>(L, minMN, n);
                            L.a = copyLowerTriangle<double>(L, m, minMN, 1);
                            P.a = zeros< double>(new ILSize(minMN, minMN));
                            // construct permutation matrix P 
                            for (int r = 0; r < m; r++) {
                                P[r, pivInd[r]] =  1.0;
                            }
                        } else {
                            pivInd = perm2indicesBackward(pivInd);
                            U.a = copyUpperTriangle<double>(L, minMN, n);
                            L.a = copyLowerTrianglePerm< double>(L, m, minMN, 1, pivInd);
                        }
                    }
                }
                ILMemoryPool.Pool.Free(pivInd);
                return L;
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// LU matrix decomposition. Decompose general matrix A into strictly upper part and lower part. 
        /// </summary>
        /// <param name="A">Input matrix. Size [m x n]</param>
        /// <returns>Triangular matrices L and U composed into a single matrix as returned from LAPACK function ?getrf. Size [m x n]</returns>
        /// <remarks><para>The matrix returned is composed out of the lower triangular matrix L with unit diagonal and the strict upper triangular matrix U.</para>
        /// <code>
        /// :'''''''|
        /// |1 \    |
        /// | 1 \ R |
        /// |  1 \  |
        /// | L 1 \ |
        /// |    1 \|
        /// '''''''''
        /// </code>
        /// <para>This overload is mainly needed for further operations via Lapack libraries. If you need the 
        /// L and U matrices directly, you'd better use one of the overloaded versions 
        /// <see cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double})"/>
        ///  or <see cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/> instead.</para>
        /// <para>The matrix L will be a solid ILArray.</para>
        /// <para>lu uses the Lapack function ?getrf.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double})"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException"> if input A is not a matrix.</exception>
        public static ILRetArray< float > lu(ILInArray<float> A) {
            ILArray< float > U = null;
            ILArray< float > P = null; 
            return lu(A, U, P); 
        }
        /// <summary>
        /// LU matrix decomposition. Decompose general matrix A into strictly upper part and lower part.
        /// </summary>
        /// <param name="A">Input matrix to be decomposed. Size [m x n]</param>
        /// <param name="U">[Output] Reference to U. On return this will be the strict upper triangular matrix of size [min(m,n) x n]. Must not be null on input.</param>
        /// <returns>Lower triangular matrix L of size [m x min(m,n)]</returns>
        /// <remarks>A is decomposed into L and U, so that ILMath.multiply (L,U) will result in A.
        /// <para>L will only be a permuted version of a true triangular matrix. I.e. the rows of L will be permuted in order 
        /// to fullfill <c>ILMath.multiply(L,U) == A</c></para>
        /// <example> <code>
        /// //we construct a matrix X: 
        /// ILArray&lt;double&gt; X = new ILArray&lt;double&gt;(new double[]{1, 2, 3, 4, 4, 4, 5, 6, 7},3,3).T;
        /// // now X.ToString() will give something like:
        /// // {&lt;Double&gt; 63238509 [3x3] Ref(2) 
        /// //(:,:) 
        /// // 1,00000  2,00000  3,00000 
        /// // 4,00000  4,00000  4,00000 
        /// // 5,00000  6,00000  7,00000 
        /// //}
        /// // construct reference on U and call the decomposition
        /// ILArray&lt;double&gt; U = new ILArray&lt;double&gt;.empty();
        /// ILArray&lt;double&gt; L = ILMath.lu(X, ref U); 
        /// 
        /// // L.ToString() is now: 
        /// // {&lt;Double&gt; 19634871 [3x3] Phys. 
        /// //(:,:) 
        /// // 0,20000  -1,00000  1,00000 
        /// // 0,80000  1,00000  0,00000 
        /// // 1,00000  0,00000  0,00000 
        /// //}
        /// // and U is now: 
        /// //{&lt;Double&gt; 22584602 [3x3] Phys. 
        /// //(:,:) 
        /// // 5,00000  6,00000  7,00000 
        /// // 0,00000  -0,80000  -1,60000 
        /// // 0,00000  0,00000  0,00000 
        /// //}
        /// </code>
        /// Pay attention to the structure of L. In the example above the first and third row are exchanged. This permutation reflects the pivoting done during the decomposition inside the Lapack function ?getrf. </example>
        /// <para>In order to access the permutation of L, one can use the overloaded version <see cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/> which returns the permuation matrix P also.</para>
        /// <para>All of the matrices U and L returned will be solid ILArrays.</para>
        /// <para>lu uses the Lapack function ?getrf.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double})"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException"> if input A is not a matrix.</exception>
        public static ILRetArray< float> lu(ILInArray<float> A, ILOutArray<float> U) {
            return lu(A,U,null); 
        }
        /// <summary>
        /// Decompose matrix A into uper and lower triangular part. Returns permutation matrix also. 
        /// </summary>
        /// <param name="A">Input matrix. Size [m x n]</param>
        /// <param name="U">[Output] Reference to upper triangular matrix. Size [min(m,n) x n]. Must not be null.</param>
        /// <param name="P">[Output] Reference to permutation matrix. Size [min(m,n) x min(m,n)]. Must not be null.</param>
        /// <returns>Lower triangular matrix L of size [m x min(m,n)]</returns>
        /// <remarks>A is decomposed into L and U, so that the equation 
        /// <c>ILMath.multiply(L,U) == ILMath.multiply(P,A)</c> 
        /// will hold except for round off error.
        /// <para>L and U will be true lower triangular matrices.</para>
        /// <example> <code>
        /// //Let's construct a matrix X: 
        /// ILArray&lt;double&gt; X = new ILArray&lt;double&gt;(new double[]{1, 2, 3, 4, 4, 4, 5, 6, 7},3,3).T;
        /// // now X.ToString() will give something like:
        /// // {&lt;Double&gt; 63238509 [3x3] Ref(2) 
        /// //(:,:) 
        /// // 1,00000  2,00000  3,00000 
        /// // 4,00000  4,00000  4,00000 
        /// // 5,00000  6,00000  7,00000 
        /// //}
        /// // construct references on U and P and call the decomposition
        /// ILArray&lt;double&gt; U = new ILArray&lt;double&gt;.empty();
        /// ILArray&lt;double&gt; P = new ILArray&lt;double&gt;.empty();
        /// ILArray&lt;double&gt; L = ILMath.lu(X, ref U, ref P); 
        /// 
        /// // L.ToString() is now: 
        /// // {&lt;Double&gt; 19634871 [3x3] Phys. 
        /// //(:,:) 
        /// // 1,00000   0,00000   0,00000 
        /// // 0,80000   1,00000   0,00000 
        /// // 0,20000  -1,00000   1,00000 
        /// //}
        /// // U is now: 
        /// //{&lt;Double&gt; 22584602 [3x3] Phys. 
        /// //(:,:) 
        /// // 5,00000  6,00000  7,00000 
        /// // 0,00000  -0,80000  -1,60000 
        /// // 0,00000  0,00000  0,00000 
        /// //}
        /// // and P is: 
        /// //{&lt;Double&gt; 2192437 [3x3] Phys. 
        /// //(:,:) 
        /// // 0,00000  0,00000  1,00000 
        /// // 0,00000  1,00000  0,00000 
        /// // 1,00000  0,00000  0,00000 
        /// //}
        /// </code>
        /// In order to reflect the pivoting done during the decomposition inside ?getrf, the matrix P may be used on A:
        /// <code>
        /// (ILMath.multiply(P,A) - ILMath.multiply(L,U)).ToString();
        /// // will give:
        /// //{&lt;Double&gt; 59192235 [3x3] Phys. 
        /// //(:,:) 
        /// // 0,00000  0,00000  0,00000 
        /// // 0,00000  0,00000  0,00000 
        /// // 0,00000  0,00000  0,00000 
        /// //}
        /// </code>
        /// </example>
        /// <para>lu uses the Lapack function ?getrf.</para>
        /// <para>All of the matrices U,L,P returned will be solid ILArrays.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double})"/>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double})"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException"> if input A is not a matrix.</exception>
        public static ILRetArray< float> lu(ILInArray<float> A, ILOutArray<float> U
                                                            , ILOutArray< float> P) {
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentSizeException("lu is defined for matrices only!");
                int m = A.Size[0], n = A.Size[1], info = 0, minMN = (m < n) ? m : n;
                ILArray< float> L = A.C;
                int[] pivInd = ILMemoryPool.Pool.New<int>(minMN);
               
                Lapack.sgetrf(m, n, L.GetArrayForWrite(), m, pivInd, ref info);
                if (info < 0) {
                    ILMemoryPool.Pool.Free(pivInd);
                    throw new ILArgumentException("invalid parameter");
                    //} else if (info > 0) {
                    //    // singular diagonal entry found 

                } else {
                    // completed successfuly 
                    if (!Object.Equals(U, null)) {
                        if (!Object.Equals(P, null)) {
                            pivInd = perm2indicesForward(pivInd);
                            U.a = copyUpperTriangle<float>(L, minMN, n);
                            L.a = copyLowerTriangle<float>(L, m, minMN, 1.0f);
                            P.a = zeros< float>(new ILSize(minMN, minMN));
                            // construct permutation matrix P 
                            for (int r = 0; r < m; r++) {
                                P[r, pivInd[r]] =  1.0f;
                            }
                        } else {
                            pivInd = perm2indicesBackward(pivInd);
                            U.a = copyUpperTriangle<float>(L, minMN, n);
                            L.a = copyLowerTrianglePerm< float>(L, m, minMN, 1.0f, pivInd);
                        }
                    }
                }
                ILMemoryPool.Pool.Free(pivInd);
                return L;
            }
        }
        /// <summary>
        /// LU matrix decomposition. Decompose general matrix A into strictly upper part and lower part. 
        /// </summary>
        /// <param name="A">Input matrix. Size [m x n]</param>
        /// <returns>Triangular matrices L and U composed into a single matrix as returned from LAPACK function ?getrf. Size [m x n]</returns>
        /// <remarks><para>The matrix returned is composed out of the lower triangular matrix L with unit diagonal and the strict upper triangular matrix U.</para>
        /// <code>
        /// :'''''''|
        /// |1 \    |
        /// | 1 \ R |
        /// |  1 \  |
        /// | L 1 \ |
        /// |    1 \|
        /// '''''''''
        /// </code>
        /// <para>This overload is mainly needed for further operations via Lapack libraries. If you need the 
        /// L and U matrices directly, you'd better use one of the overloaded versions 
        /// <see cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double})"/>
        ///  or <see cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/> instead.</para>
        /// <para>The matrix L will be a solid ILArray.</para>
        /// <para>lu uses the Lapack function ?getrf.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double})"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException"> if input A is not a matrix.</exception>
        public static ILRetArray< fcomplex > lu(ILInArray<fcomplex> A) {
            ILArray< fcomplex > U = null;
            ILArray< fcomplex > P = null; 
            return lu(A, U, P); 
        }
        /// <summary>
        /// LU matrix decomposition. Decompose general matrix A into strictly upper part and lower part.
        /// </summary>
        /// <param name="A">Input matrix to be decomposed. Size [m x n]</param>
        /// <param name="U">[Output] Reference to U. On return this will be the strict upper triangular matrix of size [min(m,n) x n]. Must not be null on input.</param>
        /// <returns>Lower triangular matrix L of size [m x min(m,n)]</returns>
        /// <remarks>A is decomposed into L and U, so that ILMath.multiply (L,U) will result in A.
        /// <para>L will only be a permuted version of a true triangular matrix. I.e. the rows of L will be permuted in order 
        /// to fullfill <c>ILMath.multiply(L,U) == A</c></para>
        /// <example> <code>
        /// //we construct a matrix X: 
        /// ILArray&lt;double&gt; X = new ILArray&lt;double&gt;(new double[]{1, 2, 3, 4, 4, 4, 5, 6, 7},3,3).T;
        /// // now X.ToString() will give something like:
        /// // {&lt;Double&gt; 63238509 [3x3] Ref(2) 
        /// //(:,:) 
        /// // 1,00000  2,00000  3,00000 
        /// // 4,00000  4,00000  4,00000 
        /// // 5,00000  6,00000  7,00000 
        /// //}
        /// // construct reference on U and call the decomposition
        /// ILArray&lt;double&gt; U = new ILArray&lt;double&gt;.empty();
        /// ILArray&lt;double&gt; L = ILMath.lu(X, ref U); 
        /// 
        /// // L.ToString() is now: 
        /// // {&lt;Double&gt; 19634871 [3x3] Phys. 
        /// //(:,:) 
        /// // 0,20000  -1,00000  1,00000 
        /// // 0,80000  1,00000  0,00000 
        /// // 1,00000  0,00000  0,00000 
        /// //}
        /// // and U is now: 
        /// //{&lt;Double&gt; 22584602 [3x3] Phys. 
        /// //(:,:) 
        /// // 5,00000  6,00000  7,00000 
        /// // 0,00000  -0,80000  -1,60000 
        /// // 0,00000  0,00000  0,00000 
        /// //}
        /// </code>
        /// Pay attention to the structure of L. In the example above the first and third row are exchanged. This permutation reflects the pivoting done during the decomposition inside the Lapack function ?getrf. </example>
        /// <para>In order to access the permutation of L, one can use the overloaded version <see cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/> which returns the permuation matrix P also.</para>
        /// <para>All of the matrices U and L returned will be solid ILArrays.</para>
        /// <para>lu uses the Lapack function ?getrf.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double})"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException"> if input A is not a matrix.</exception>
        public static ILRetArray< fcomplex> lu(ILInArray<fcomplex> A, ILOutArray<fcomplex> U) {
            return lu(A,U,null); 
        }
        /// <summary>
        /// Decompose matrix A into uper and lower triangular part. Returns permutation matrix also. 
        /// </summary>
        /// <param name="A">Input matrix. Size [m x n]</param>
        /// <param name="U">[Output] Reference to upper triangular matrix. Size [min(m,n) x n]. Must not be null.</param>
        /// <param name="P">[Output] Reference to permutation matrix. Size [min(m,n) x min(m,n)]. Must not be null.</param>
        /// <returns>Lower triangular matrix L of size [m x min(m,n)]</returns>
        /// <remarks>A is decomposed into L and U, so that the equation 
        /// <c>ILMath.multiply(L,U) == ILMath.multiply(P,A)</c> 
        /// will hold except for round off error.
        /// <para>L and U will be true lower triangular matrices.</para>
        /// <example> <code>
        /// //Let's construct a matrix X: 
        /// ILArray&lt;double&gt; X = new ILArray&lt;double&gt;(new double[]{1, 2, 3, 4, 4, 4, 5, 6, 7},3,3).T;
        /// // now X.ToString() will give something like:
        /// // {&lt;Double&gt; 63238509 [3x3] Ref(2) 
        /// //(:,:) 
        /// // 1,00000  2,00000  3,00000 
        /// // 4,00000  4,00000  4,00000 
        /// // 5,00000  6,00000  7,00000 
        /// //}
        /// // construct references on U and P and call the decomposition
        /// ILArray&lt;double&gt; U = new ILArray&lt;double&gt;.empty();
        /// ILArray&lt;double&gt; P = new ILArray&lt;double&gt;.empty();
        /// ILArray&lt;double&gt; L = ILMath.lu(X, ref U, ref P); 
        /// 
        /// // L.ToString() is now: 
        /// // {&lt;Double&gt; 19634871 [3x3] Phys. 
        /// //(:,:) 
        /// // 1,00000   0,00000   0,00000 
        /// // 0,80000   1,00000   0,00000 
        /// // 0,20000  -1,00000   1,00000 
        /// //}
        /// // U is now: 
        /// //{&lt;Double&gt; 22584602 [3x3] Phys. 
        /// //(:,:) 
        /// // 5,00000  6,00000  7,00000 
        /// // 0,00000  -0,80000  -1,60000 
        /// // 0,00000  0,00000  0,00000 
        /// //}
        /// // and P is: 
        /// //{&lt;Double&gt; 2192437 [3x3] Phys. 
        /// //(:,:) 
        /// // 0,00000  0,00000  1,00000 
        /// // 0,00000  1,00000  0,00000 
        /// // 1,00000  0,00000  0,00000 
        /// //}
        /// </code>
        /// In order to reflect the pivoting done during the decomposition inside ?getrf, the matrix P may be used on A:
        /// <code>
        /// (ILMath.multiply(P,A) - ILMath.multiply(L,U)).ToString();
        /// // will give:
        /// //{&lt;Double&gt; 59192235 [3x3] Phys. 
        /// //(:,:) 
        /// // 0,00000  0,00000  0,00000 
        /// // 0,00000  0,00000  0,00000 
        /// // 0,00000  0,00000  0,00000 
        /// //}
        /// </code>
        /// </example>
        /// <para>lu uses the Lapack function ?getrf.</para>
        /// <para>All of the matrices U,L,P returned will be solid ILArrays.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double})"/>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double})"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException"> if input A is not a matrix.</exception>
        public static ILRetArray< fcomplex> lu(ILInArray<fcomplex> A, ILOutArray<fcomplex> U
                                                            , ILOutArray< fcomplex> P) {
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentSizeException("lu is defined for matrices only!");
                int m = A.Size[0], n = A.Size[1], info = 0, minMN = (m < n) ? m : n;
                ILArray< fcomplex> L = A.C;
                int[] pivInd = ILMemoryPool.Pool.New<int>(minMN);
               
                Lapack.cgetrf(m, n, L.GetArrayForWrite(), m, pivInd, ref info);
                if (info < 0) {
                    ILMemoryPool.Pool.Free(pivInd);
                    throw new ILArgumentException("invalid parameter");
                    //} else if (info > 0) {
                    //    // singular diagonal entry found 

                } else {
                    // completed successfuly 
                    if (!Object.Equals(U, null)) {
                        if (!Object.Equals(P, null)) {
                            pivInd = perm2indicesForward(pivInd);
                            U.a = copyUpperTriangle<fcomplex>(L, minMN, n);
                            L.a = copyLowerTriangle<fcomplex>(L, m, minMN, new fcomplex(1.0f,0.0f));
                            P.a = zeros< fcomplex>(new ILSize(minMN, minMN));
                            // construct permutation matrix P 
                            for (int r = 0; r < m; r++) {
                                P[r, pivInd[r]] =  new fcomplex(1.0f,0.0f);
                            }
                        } else {
                            pivInd = perm2indicesBackward(pivInd);
                            U.a = copyUpperTriangle<fcomplex>(L, minMN, n);
                            L.a = copyLowerTrianglePerm< fcomplex>(L, m, minMN, new fcomplex(1.0f,0.0f), pivInd);
                        }
                    }
                }
                ILMemoryPool.Pool.Free(pivInd);
                return L;
            }
        }
        /// <summary>
        /// LU matrix decomposition. Decompose general matrix A into strictly upper part and lower part. 
        /// </summary>
        /// <param name="A">Input matrix. Size [m x n]</param>
        /// <returns>Triangular matrices L and U composed into a single matrix as returned from LAPACK function ?getrf. Size [m x n]</returns>
        /// <remarks><para>The matrix returned is composed out of the lower triangular matrix L with unit diagonal and the strict upper triangular matrix U.</para>
        /// <code>
        /// :'''''''|
        /// |1 \    |
        /// | 1 \ R |
        /// |  1 \  |
        /// | L 1 \ |
        /// |    1 \|
        /// '''''''''
        /// </code>
        /// <para>This overload is mainly needed for further operations via Lapack libraries. If you need the 
        /// L and U matrices directly, you'd better use one of the overloaded versions 
        /// <see cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double})"/>
        ///  or <see cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/> instead.</para>
        /// <para>The matrix L will be a solid ILArray.</para>
        /// <para>lu uses the Lapack function ?getrf.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double})"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException"> if input A is not a matrix.</exception>
        public static ILRetArray< complex > lu(ILInArray<complex> A) {
            ILArray< complex > U = null;
            ILArray< complex > P = null; 
            return lu(A, U, P); 
        }
        /// <summary>
        /// LU matrix decomposition. Decompose general matrix A into strictly upper part and lower part.
        /// </summary>
        /// <param name="A">Input matrix to be decomposed. Size [m x n]</param>
        /// <param name="U">[Output] Reference to U. On return this will be the strict upper triangular matrix of size [min(m,n) x n]. Must not be null on input.</param>
        /// <returns>Lower triangular matrix L of size [m x min(m,n)]</returns>
        /// <remarks>A is decomposed into L and U, so that ILMath.multiply (L,U) will result in A.
        /// <para>L will only be a permuted version of a true triangular matrix. I.e. the rows of L will be permuted in order 
        /// to fullfill <c>ILMath.multiply(L,U) == A</c></para>
        /// <example> <code>
        /// //we construct a matrix X: 
        /// ILArray&lt;double&gt; X = new ILArray&lt;double&gt;(new double[]{1, 2, 3, 4, 4, 4, 5, 6, 7},3,3).T;
        /// // now X.ToString() will give something like:
        /// // {&lt;Double&gt; 63238509 [3x3] Ref(2) 
        /// //(:,:) 
        /// // 1,00000  2,00000  3,00000 
        /// // 4,00000  4,00000  4,00000 
        /// // 5,00000  6,00000  7,00000 
        /// //}
        /// // construct reference on U and call the decomposition
        /// ILArray&lt;double&gt; U = new ILArray&lt;double&gt;.empty();
        /// ILArray&lt;double&gt; L = ILMath.lu(X, ref U); 
        /// 
        /// // L.ToString() is now: 
        /// // {&lt;Double&gt; 19634871 [3x3] Phys. 
        /// //(:,:) 
        /// // 0,20000  -1,00000  1,00000 
        /// // 0,80000  1,00000  0,00000 
        /// // 1,00000  0,00000  0,00000 
        /// //}
        /// // and U is now: 
        /// //{&lt;Double&gt; 22584602 [3x3] Phys. 
        /// //(:,:) 
        /// // 5,00000  6,00000  7,00000 
        /// // 0,00000  -0,80000  -1,60000 
        /// // 0,00000  0,00000  0,00000 
        /// //}
        /// </code>
        /// Pay attention to the structure of L. In the example above the first and third row are exchanged. This permutation reflects the pivoting done during the decomposition inside the Lapack function ?getrf. </example>
        /// <para>In order to access the permutation of L, one can use the overloaded version <see cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/> which returns the permuation matrix P also.</para>
        /// <para>All of the matrices U and L returned will be solid ILArrays.</para>
        /// <para>lu uses the Lapack function ?getrf.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double})"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException"> if input A is not a matrix.</exception>
        public static ILRetArray< complex> lu(ILInArray<complex> A, ILOutArray<complex> U) {
            return lu(A,U,null); 
        }
        /// <summary>
        /// Decompose matrix A into uper and lower triangular part. Returns permutation matrix also. 
        /// </summary>
        /// <param name="A">Input matrix. Size [m x n]</param>
        /// <param name="U">[Output] Reference to upper triangular matrix. Size [min(m,n) x n]. Must not be null.</param>
        /// <param name="P">[Output] Reference to permutation matrix. Size [min(m,n) x min(m,n)]. Must not be null.</param>
        /// <returns>Lower triangular matrix L of size [m x min(m,n)]</returns>
        /// <remarks>A is decomposed into L and U, so that the equation 
        /// <c>ILMath.multiply(L,U) == ILMath.multiply(P,A)</c> 
        /// will hold except for round off error.
        /// <para>L and U will be true lower triangular matrices.</para>
        /// <example> <code>
        /// //Let's construct a matrix X: 
        /// ILArray&lt;double&gt; X = new ILArray&lt;double&gt;(new double[]{1, 2, 3, 4, 4, 4, 5, 6, 7},3,3).T;
        /// // now X.ToString() will give something like:
        /// // {&lt;Double&gt; 63238509 [3x3] Ref(2) 
        /// //(:,:) 
        /// // 1,00000  2,00000  3,00000 
        /// // 4,00000  4,00000  4,00000 
        /// // 5,00000  6,00000  7,00000 
        /// //}
        /// // construct references on U and P and call the decomposition
        /// ILArray&lt;double&gt; U = new ILArray&lt;double&gt;.empty();
        /// ILArray&lt;double&gt; P = new ILArray&lt;double&gt;.empty();
        /// ILArray&lt;double&gt; L = ILMath.lu(X, ref U, ref P); 
        /// 
        /// // L.ToString() is now: 
        /// // {&lt;Double&gt; 19634871 [3x3] Phys. 
        /// //(:,:) 
        /// // 1,00000   0,00000   0,00000 
        /// // 0,80000   1,00000   0,00000 
        /// // 0,20000  -1,00000   1,00000 
        /// //}
        /// // U is now: 
        /// //{&lt;Double&gt; 22584602 [3x3] Phys. 
        /// //(:,:) 
        /// // 5,00000  6,00000  7,00000 
        /// // 0,00000  -0,80000  -1,60000 
        /// // 0,00000  0,00000  0,00000 
        /// //}
        /// // and P is: 
        /// //{&lt;Double&gt; 2192437 [3x3] Phys. 
        /// //(:,:) 
        /// // 0,00000  0,00000  1,00000 
        /// // 0,00000  1,00000  0,00000 
        /// // 1,00000  0,00000  0,00000 
        /// //}
        /// </code>
        /// In order to reflect the pivoting done during the decomposition inside ?getrf, the matrix P may be used on A:
        /// <code>
        /// (ILMath.multiply(P,A) - ILMath.multiply(L,U)).ToString();
        /// // will give:
        /// //{&lt;Double&gt; 59192235 [3x3] Phys. 
        /// //(:,:) 
        /// // 0,00000  0,00000  0,00000 
        /// // 0,00000  0,00000  0,00000 
        /// // 0,00000  0,00000  0,00000 
        /// //}
        /// </code>
        /// </example>
        /// <para>lu uses the Lapack function ?getrf.</para>
        /// <para>All of the matrices U,L,P returned will be solid ILArrays.</para>
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double})"/>
        /// <seealso cref="ILNumerics.ILMath.lu(ILInArray{double}, ILOutArray{double})"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException"> if input A is not a matrix.</exception>
        public static ILRetArray< complex> lu(ILInArray<complex> A, ILOutArray<complex> U
                                                            , ILOutArray< complex> P) {
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentSizeException("lu is defined for matrices only!");
                int m = A.Size[0], n = A.Size[1], info = 0, minMN = (m < n) ? m : n;
                ILArray< complex> L = A.C;
                int[] pivInd = ILMemoryPool.Pool.New<int>(minMN);
               
                Lapack.zgetrf(m, n, L.GetArrayForWrite(), m, pivInd, ref info);
                if (info < 0) {
                    ILMemoryPool.Pool.Free(pivInd);
                    throw new ILArgumentException("invalid parameter");
                    //} else if (info > 0) {
                    //    // singular diagonal entry found 

                } else {
                    // completed successfuly 
                    if (!Object.Equals(U, null)) {
                        if (!Object.Equals(P, null)) {
                            pivInd = perm2indicesForward(pivInd);
                            U.a = copyUpperTriangle<complex>(L, minMN, n);
                            L.a = copyLowerTriangle<complex>(L, m, minMN, new complex(1.0,0.0));
                            P.a = zeros< complex>(new ILSize(minMN, minMN));
                            // construct permutation matrix P 
                            for (int r = 0; r < m; r++) {
                                P[r, pivInd[r]] =  new complex(1.0,0.0);
                            }
                        } else {
                            pivInd = perm2indicesBackward(pivInd);
                            U.a = copyUpperTriangle<complex>(L, minMN, n);
                            L.a = copyLowerTrianglePerm< complex>(L, m, minMN, new complex(1.0,0.0), pivInd);
                        }
                    }
                }
                ILMemoryPool.Pool.Free(pivInd);
                return L;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Copy upper triangle from PHYSICAL array A
        /// </summary>
        /// <typeparam name="T">Arbitrary inner type </typeparam>
        /// <param name="A">PHYSICAL ILArray</param>
        /// <param name="m">Number of rows</param>
        /// <param name="n">Number of columns</param>
        /// <returns>Newly created physical array with the upper triangle of A</returns>
        /// <remarks>No checks are made for m,n fit inside A!</remarks>
        internal static ILRetArray<T> copyUpperTriangle<T>(ILInArray<T> A, int m, int n) {
            using (ILScope.Enter(A)) {
                T[] arr = ILMemoryPool.Pool.New<T>(m * n);
                for (int r = 0; r < m; r++) {
                    for (int c = r; c < n; c++) {
                        arr[r + c * m] = A.GetValue(r, c);
                    }
                }
                return new ILRetArray<T>(arr, m, n);
            }
        }
        /// <summary>
        /// Copy upper triangle from system array A
        /// </summary>
        /// <typeparam name="T">Arbitrary inner type </typeparam>
        /// <param name="arrIn">System array, size (m x n), column wise ordered</param>
        /// <param name="arrInM">Number of rows</param>
        /// <param name="arrInN">Number of columns</param>
        /// <param name="outM">Number of rows in output matrix</param>
        /// <returns>Newly created physical array with the upper triangle of A</returns>
        /// <remarks>No checks are made for m,n fit inside A! copies the main diagonal also.
        /// the array returned will be of size (min(m,n) x n)</remarks>
        private static ILRetArray<T> copyUpperTriangle<T>(T[] arrIn, int arrInM, int arrInN, int outM) {
            T[] arrOu = ILMemoryPool.Pool.New<T>(outM * arrInN);
            for (int c = 0; c < arrInN; c++) {
                for (int r = 0; r <= c && r < outM; r++) {
                    arrOu[c * outM + r] = arrIn[c * arrInM + r];
                }
            }
            return new ILRetArray<T> (arrOu,outM,arrInN);
        }
        /// <summary>
        /// Copy lower triangle from PHYSICAL array A, set diagonal to val
        /// </summary>
        /// <typeparam name="T">Arbitrary inner type </typeparam>
        /// <param name="A">PHYSICAL ILArray</param>
        /// <param name="m">Number of rows</param>
        /// <param name="n">Number of columns</param>
        /// <param name="val">Value for diagonal entries</param>
        /// <returns>Newly created physical array with the lower triangle of A</returns>
        /// <remarks>No checks are made for m,n fit inside A!</remarks>
        private static ILRetArray<T> copyLowerTriangle<T>(ILInArray<T> A, int m, int n, T val) {
            using (ILScope.Enter(A)) {
                T[] arr = ILMemoryPool.Pool.New<T>(m * n);
                for (int r = 0; r < m; r++) {
                    for (int c = r + 1; c-- > 0; ) {
                        arr[r + m * c] = A.GetValue(r, c);
                    }
                    arr[r + m * r] = val;
                }
                return new ILRetArray<T>(arr, m, n);
            }
        }
        /// <summary>
        /// Copy lower triangle from PHYSICAL array A, set diagonal to val, permuted version
        /// </summary>
        /// <typeparam name="T">Arbitrary inner type </typeparam>
        /// <param name="A">PHYSICAL ILArray</param>
        /// <param name="m">Number of rows</param>
        /// <param name="n">Number of columns</param>
        /// <param name="perm">Mapping for rows, must be converted fom LAPACK version to single indices </param>
        /// <param name="val">Value for diagonal entries</param>
        /// <returns>Newly created physical array with the lower triangle of A</returns>
        /// <remarks>No checks are made for m,n fit inside A!</remarks>
        private static ILRetArray<T> copyLowerTrianglePerm<T>(ILInArray<T> A, int m, int n, T val, int[] perm) {
            using (ILScope.Enter(A)) {
                T[] arr = ILMemoryPool.Pool.New<T>(m * n);
                int trueRow;
                for (int r = 0; r < perm.Length; r++) {
                    trueRow = perm[r];
                    for (int c = 0; c < trueRow; c++) {
                        arr[r + c * m] = A.GetValue(trueRow, c);
                    }
                    arr[r + m * trueRow] = val;
                }
                return new ILRetArray<T>(arr, m, n);
            }
        }

        /// <summary>
        /// Relabel permutation indices from LAPACK ?getrf
        /// </summary>
        /// <param name="perm">Lapack pivoting permutation array</param>
        /// <returns>Index mapping for direct addressing the rows </returns>
        /// <remarks>Exchange the row labels in the same manner as LAPACK did for pivoting</remarks>
        private static int[] perm2indicesForward(int[] perm) {
            int [] ret = ILMemoryPool.Pool.New<int>(perm.Length);
            for (int i = 0; i < ret.Length; i++) {
                ret[i] = i; 
            }
            int tmp; 
            for (int i = 0; i < ret.Length; i++) {
                if (perm[i] != i+1) {
                    tmp = ret[perm[i]-1]; 
                    ret[perm[i]-1] = i; 
                    ret[i] = tmp; 
                }
            }
            return ret; 
        }
        /// <summary>
        /// Relabel permutation indices from LAPACK ?getrf - backward version
        /// </summary>
        /// <param name="perm">Lapack pivoting permutation array</param>
        /// <returns>Index mapping for direct addressing the rows </returns>
        /// <remarks>Exchange the row labels in the same manner as LAPACK did for pivoting, but backwards</remarks>
        private static int[] perm2indicesBackward(int[] perm) {
            int [] ret = ILMemoryPool.Pool.New<int>(perm.Length);
            for (int i = 0; i < ret.Length; i++) {
                ret[i] = i; 
            }
            int tmp; 
            for (int i = ret.Length - 1; i-->0;) {
                if (perm[i] != i+1) {
                    tmp = ret[perm[i]-1]; 
                    ret[perm[i]-1] = i; 
                    ret[i] = tmp; 
                }
            }
            return ret; 
        }

    }
}
