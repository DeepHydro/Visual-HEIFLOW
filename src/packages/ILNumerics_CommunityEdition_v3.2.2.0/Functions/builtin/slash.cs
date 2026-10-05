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
        /// Solve linear equation system
        /// </summary>
        /// <param name="A">Matrix A. Size [n x q]</param>
        /// <param name="B">Right hand side B. Size [n x m]</param>
        /// <returns>Solution x solving the equation system: multiply(A, x) = B. Size [n x m]</returns>
        /// <remarks><para>Depending on the structure and properties of A, the equation system will be solved in different ways:
        /// <list type="bullet">
        /// <item>If A is square (q == n) and an <b>upper or lower triangular</b> matrix, the system will directly be solved via backward- or forward substitution. Therefore the Lapack function ?trtrs will be used, whenever the memory layout of A is suitable. This may be the case even for reference ILArray's! 
        /// <example><code><![CDATA[ILArray<double> A = ILMath.randn(4); // construct 4 x 4 matrix 
        /// A = A.T; // A is a reference array now! The transpose is fast and does not consume much memory
        /// // now construct a right side and solve the equations: 
        /// ILArray<double> B = new ILArray<double> (1.0,2.0,3.0).T; 
        /// ILMath.linsolve(A,B); // ... will be carried out via Lapack, even for all arrays involved being reference arrays! ]]></code></example></item>
        /// <item><para>if A is square and symmetric or hermitian, A will be decomposed into a triangular equation system using cholesky factorization and Lapack. The system is than solved using performant Lapack routines.</para>
        /// <para>If during the cholesky factorization A was found to be <b>not positive definite</b> - the cholesky factorization is canceled. </para></item>
        /// <item>otherwise, if A is square only, it will be decomposed into upper and lower triangular martices using LU decomposition and Lapack. The triangular system is than solved using performant Lapack routines.</item>
        /// <item>otherwise, if A is of size [q x n] and q != n, the system is solved using QR decomposition. A may be rank deficient. The solution is computed by use of the Lapack routine '?gelsy' and may be a reference array.</item>
        /// </list></para>
        /// <para>Compatibility with Matlab<sup>(R)</sup>: If A is square, the algorithm used follows the same logic as Matlab up to Rel 14, with the exception of Hessenberg matrices wich are solved via LU factorization here. The un-squared case is handled differently. A direct Lapack driver function (?gelsy) is used here. Therefore the solutions might differ! However, the solution will of course fullfill the equation A * x = B without round off errrors. </para>
        /// <para>For specifiying the rank of A in the unsquare case (q != n), <see cref="ILNumerics.ILMath.eps"/> is used.</para></remarks>
        public static ILRetArray< double > linsolve(ILInArray< double > A, ILInArray< double > B) {
            if (object.Equals(A,null) || object.Equals(B,null))
                throw new ILArgumentException("parameter must not be null!");
            using (ILScope.Enter(A, B)) {
                MatrixProperties props = MatrixProperties.None;
                if (A.Size[0] == A.Size[1]) {
                    props |= MatrixProperties.Square;
                    if (ILMath.istriup(A)) {
                        props |= MatrixProperties.UpperTriangular;
                        return linsolve(A, B, ref props);
                    }
                    if (ILMath.istrilow(A)) {
                        props |= MatrixProperties.LowerTriangular;
                        return linsolve(A, B, ref props);
                    }
                    if (ILMath.ishermitian(A)) {
                        // give cholesky a try
                        props |= MatrixProperties.Hermitian;
                        props |= MatrixProperties.PositivDefinite;
                        ILArray< double> ret = linsolve(A, B, ref props);
                        if (!object.Equals(ret, null)) {
                            return ret;
                        } else {
                            props ^= MatrixProperties.PositivDefinite;
                        }
                    }
                }
                return linsolve(A, B, ref props);
            }
        }

        /// <summary>
        /// Solve linear equation system
        /// </summary>
        /// <param name="A">Matrix A. Size [n x q]</param>
        /// <param name="B">Right hand side B. Size [n x m]</param>
        /// <param name="props">Matrix properties. If defined, no checks are made for the structure of A. If the 
        /// matrix A was found to be (close to or) singular, the 'MatrixProperties.Singular' flag in props will be set. 
        /// This flag should be tested on return, in order to verify the reliability of the solution.</param>
        /// <returns>The solution x solving multiply(A,x) = B. Size [n x m]</returns>
        /// <remarks><para>Depending on the <paramref name="props"/> parameter the equation system will be solved 
        /// differently for special structures of A:
        /// <list type="bullet">
        /// <item>If A is square (q == n) and an <b>upper or lower triangular</b> matrix, the system will directly 
        /// be solved via backward- or forward substitution. Therefore the Lapack function ?trtrs will be used, 
        /// whenever the memory layout of A is suitable. This may be the case even for reference ILArray's! 
        /// <example><code><![CDATA[ILArray<double> A = ILMath.randn(4); // construct 4 x 4 matrix 
        /// A = A.T; // A is a reference array now! The transpose is fast and does not consume much memory
        /// // now construct a right side and solve the equations: 
        /// ILArray<double> B = new ILArray<double> (1.0,2.0,3.0).T; 
        /// ILMath.linsolve(A,B); // ... will be carried out via Lapack, even for all arrays involved being reference arrays! ]]></code></example></item>
        /// <item><para>If A is square and symmetric or hermitian, A will be decomposed into a triangular equation 
        /// system using cholesky factorization and Lapack. The system is than solved using performant Lapack routines.</para>
        /// <para>If during the cholesky factorization A was found to be <b>not positive definite</b> - the 
        /// corresponding flag in props will be cleaned and <c>null</c> will be returned.</para></item>
        /// <item>Otherwise if A is square only, it will be decomposed into upper and lower triangular matrices 
        /// using LU decomposition and Lapack. The triangular system is than solved using performant Lapack routines.</item>
        /// <item>Otherwise, if A is of size [q x n] and q != n, the system is solved using QR decomposition. 
        /// A may be rank deficient. The solution is computed by use of the Lapack routine '?gelsy' and may be a 
        /// reference array.</item>
        /// </list></para>
        /// <para>Compatibility with Matlab<sup>(R)</sup>: If A is square, the algorithm used follows the same 
        /// logic as Matlab up to Rel 14, with the exception of Hessenberg matrices wich are solved via LU 
        /// factorization here. The un-squared case is handled differently. A direct Lapack driver function 
        /// (?gelsy) is used here. Therefore the solutions might differ! However, the solution will of course 
        /// fullfill the equation A * x = B without round off errrors. </para>
        /// <para>For specifiying the rank of A in the unsquare case (q != n), 
        /// <see cref="ILNumerics.ILMath.eps"/> is used.</para></remarks>
        public static ILRetArray< double > linsolve(ILInArray< double > A, ILInArray< double > B, ref MatrixProperties props) {
            if (object.Equals(A,null)) 
                throw new ILArgumentException("input argument A must not be null!"); 
            if (object.Equals(B,null))
                throw new ILArgumentException("input argument B must not be null!");
            using (ILScope.Enter(A, B)) {
                if (A.IsEmpty || B.IsEmpty)
                    return empty< double>(A.Size);
                if (A.Size[0] != B.Size[0])
                    throw new ILArgumentException("number of rows for matrix A must match number of rows for RHS!");
                int info = 0, m = A.Size[0];
                ILArray< double> ret = empty<double>(ILSize.Empty00);
                if (m == A.Size[1]) {
                    props |= MatrixProperties.Square;
                    if ((props & MatrixProperties.LowerTriangular) != 0) {
                        ret.a = linsolveTriLow(A, B, ref info);
                        if (info > 0)
                            props |= MatrixProperties.Singular;
                        return ret;
                    }
                    if ((props & MatrixProperties.UpperTriangular) != 0) {
                        ret.a = linsolveTriUp(A, B, ref info);
                        if (info > 0)
                            props |= MatrixProperties.Singular;
                        return ret;
                    }
                    if ((props & MatrixProperties.Hermitian) != 0) {
                        ILDenseStorage< double> cholFact = A.Storage.copyUpperTriangle(m);
                        /*!HC:lapack_*potrf*/
                        Lapack.dpotrf('U', m, cholFact.GetArrayForWrite(), m, ref info);
                        if (info > 0) {
                            props ^= MatrixProperties.Hermitian;
                            cholFact.Dispose(); 
                            return null;
                        } else {
                            // solve 
                            ret.a = B.C;
                            /*!HC:lapack_*potrs*/
                            Lapack.dpotrs('U', m, B.Size[1], cholFact.GetArrayForWrite(), m, ret.GetArrayForWrite(), m, ref info);
                            cholFact.Dispose(); 
                            return ret;
                        }
                    } else {
                        // attempt complete (expensive) LU factorization 
                        ILArray< double> L = A.C;
                        int[] pivInd = ILMemoryPool.Pool.New<int>(m);
                        /*!HC:lapack_*getrf*/
                        Lapack.dgetrf(m, m, L.GetArrayForWrite(), m, pivInd, ref info);
                        if (info > 0)
                            props |= MatrixProperties.Singular;
                        ret.a = B.C;
                        /*!HC:lapack_*getrs*/
                        Lapack.dgetrs('N', m, B.Size[1], L.GetArrayForWrite(), m, pivInd, ret.GetArrayForWrite(), m, ref info);
                        if (info < 0)
                            throw new ILArgumentException("failed to solve via lapack dgetrs");
                        return ret;
                    }
                } else {
                    // under- / overdetermined system
                    int n = A.Size[1], rank = 0, minMN = (m < n) ? m : n, maxMN = (m > n) ? m : n;
                    int nrhs = B.Size[1];
                    if (B.Size[0] != m)
                        throw new ILArgumentException("right hand side matrix B must match input A!");
                    ILArray</*!HCinArr1*/ double> tmpA = A.C;
                    if (m < n) {
                        ret.a = zeros< double>(n, nrhs);
                        ret[r(0, m - 1), full] = B.C;
                    } else {
                        ret.a = B.C;
                    }
                    int[] JPVT = new int[n];
                    /*!HC:Lapack.?gelsy*/
                    Lapack.dgelsy(m, n, B.Size[1], tmpA.GetArrayForWrite(), m, ret.GetArrayForWrite(),
                        maxMN, JPVT,  ILMath.MachineParameterDouble.eps,
                        ref rank, ref info);
                    if (n < m) {
                        ret.a = ret[r(0, n - 1), full];
                    }
                    if (rank < minMN)
                        props |= MatrixProperties.RankDeficient;
                    return ret;
                }
            }
        }
        
        /// <summary>
        /// Solve system of linear equations A*x = b, with A being a upper triangular matrix
        /// </summary>
        /// <param name="A">Input matrix of size [n x n], must be upper triangular. No check is made for that!</param>
        /// <param name="B">Solution vector or matrix. Size [n x m]</param>
        /// <param name="singularityDetect">[Output] This value gives the row of A, where a singularity has been detected (if any). If A is not singular, this will be a negative value.</param>
        /// <returns>Solution x solving A * x = b.</returns>
        /// <remarks><para>The solution will be determined via backward substitution</para>
        /// <para>Make sure, A and b are of correct size, since no checks are made for that!</para>
        /// <para>This function is used by ILMath.linsolve. There should be rare need for you to call this function directly.</para>
        /// <para>Elements of A below the main diagonal will not be accessed.</para>
        /// <para>If A has been found to be singular, the array returned will contain NaN values for corresponding elements!</para></remarks>
        public static ILRetArray< double > linsolveTriUp (ILInArray< double > A, ILInArray< double > B, ref int singularityDetect) {
            System.Diagnostics.Debug.Assert(B.Size[1] >= 0); 
            System.Diagnostics.Debug.Assert(B.Size[0] == A.Size[1]); 
            System.Diagnostics.Debug.Assert(A.Size[0] == A.Size[1]);
            using (ILScope.Enter(A, B)) {
                singularityDetect = -1;
                int n = A.Size[0];
                int m = B.Size[1];
                int info = 0;
                ILArray< double> ret = B.C;
                
                double[] retArr = ret.GetArrayForWrite();
                // solve using Lapack
                unsafe {
                    fixed ( double* ptrA = A.GetArrayForRead())
                    fixed ( double* ptrB = ret.GetArrayForWrite()) {
                        /*!HC:lapack.?trtrs*/
                        Lapack.dtrtrs('U', 'N', 'N', A.Size[0], B.Size[1], (IntPtr)ptrA, A.Size[0], (IntPtr)ptrB, B.Size[0], ref info);
                    }
                }
                if (info < 0)
                    throw new ILArgumentException("error inside Lapack function ?trtrs for argument: " + (-info));
                if (info > 0) {
                    singularityDetect = info - 1;
                    for (m = 0; m < ret.Size[1]; m++) {
                        info = m * n + singularityDetect;
                        for (int i = singularityDetect; i < n; i++) {
                            retArr[info++] =  double.NaN;
                        }
                    }
                } else {
                    singularityDetect = -1;
                }
                return ret;
            }
        }

        /// <summary>
        /// Solve system of linear equations A*x = b, with A being a lower triangular matrix
        /// </summary>
        /// <param name="A">Input matrix of size [n x n], must be lower triangular. No check is made for that!</param>
        /// <param name="B">Solution vector. Size [n x 1]</param>
        /// <param name="singularityDetect">[Output] This value gives the row of A, where a singularity has been detected (if any). If A is not singular, this will be a negative value.</param>
        /// <returns>Solution x solving A * x = b.</returns>
        /// <remarks><para>The solution will be determined via forward substitution</para>
        /// <para>Make sure, A and b are of correct size, since no checks are made for that!</para>
        /// <para>This function is used by ILMath.linsolve. There should be rare need for you to call this function directly.</para>
        /// <para>Elements of A above the main diagonal will not be accessed.</para>
        /// <para>If A has been found to be singular, the array returned will contain NaN values for corresponding elements!</para></remarks>
        public static ILRetArray< double> linsolveTriLow(ILInArray< double> A, ILInArray< double> B, ref int singularityDetect) {
            System.Diagnostics.Debug.Assert(B.Size[1] >= 0);
            System.Diagnostics.Debug.Assert(B.Size[0] == A.Size[1]);
            System.Diagnostics.Debug.Assert(A.Size[0] == A.Size[1]);
            using (ILScope.Enter(A, B)) {
                singularityDetect = -1;
                int n = A.Size[0];
                int m = B.Size[1];
                int info = 0;
                ILArray< double> ret = B.C;
                
                double[] retArr = ret.GetArrayForWrite();
                // solve using Lapack
                unsafe {
                    fixed ( double* ptrA = A.GetArrayForRead())
                    fixed ( double* ptrB = ret.GetArrayForWrite()) {
                        /*!HC:lapack.?trtrs*/
                        Lapack.dtrtrs('L', 'N', 'N', A.Size[0], B.Size[1], (IntPtr)ptrA, A.Size[0], (IntPtr)ptrB, B.Size[0], ref info);
                    }
                }
                if (info < 0)
                    throw new ILArgumentException("error inside Lapack function ?trtrs for argument: " + (-info));
                if (info > 0) {
                    singularityDetect = info - 1;
                    for (m = 0; m < ret.Size[1]; m++) {
                        info = m * n + singularityDetect;
                        for (int i = singularityDetect; i < n; i++) {
                            retArr[info++] =  double.NaN;
                        }
                    }
                } else {
                    singularityDetect = -1;
                }
                return ret;
            }
        }


#region HYCALPER AUTO GENERATED CODE

        
        /// <summary>
        /// Solve linear equation system
        /// </summary>
        /// <param name="A">Matrix A. Size [n x q]</param>
        /// <param name="B">Right hand side B. Size [n x m]</param>
        /// <returns>Solution x solving the equation system: multiply(A, x) = B. Size [n x m]</returns>
        /// <remarks><para>Depending on the structure and properties of A, the equation system will be solved in different ways:
        /// <list type="bullet">
        /// <item>If A is square (q == n) and an <b>upper or lower triangular</b> matrix, the system will directly be solved via backward- or forward substitution. Therefore the Lapack function ?trtrs will be used, whenever the memory layout of A is suitable. This may be the case even for reference ILArray's! 
        /// <example><code><![CDATA[ILArray<double> A = ILMath.randn(4); // construct 4 x 4 matrix 
        /// A = A.T; // A is a reference array now! The transpose is fast and does not consume much memory
        /// // now construct a right side and solve the equations: 
        /// ILArray<double> B = new ILArray<double> (1.0,2.0,3.0).T; 
        /// ILMath.linsolve(A,B); // ... will be carried out via Lapack, even for all arrays involved being reference arrays! ]]></code></example></item>
        /// <item><para>if A is square and symmetric or hermitian, A will be decomposed into a triangular equation system using cholesky factorization and Lapack. The system is than solved using performant Lapack routines.</para>
        /// <para>If during the cholesky factorization A was found to be <b>not positive definite</b> - the cholesky factorization is canceled. </para></item>
        /// <item>otherwise, if A is square only, it will be decomposed into upper and lower triangular martices using LU decomposition and Lapack. The triangular system is than solved using performant Lapack routines.</item>
        /// <item>otherwise, if A is of size [q x n] and q != n, the system is solved using QR decomposition. A may be rank deficient. The solution is computed by use of the Lapack routine '?gelsy' and may be a reference array.</item>
        /// </list></para>
        /// <para>Compatibility with Matlab<sup>(R)</sup>: If A is square, the algorithm used follows the same logic as Matlab up to Rel 14, with the exception of Hessenberg matrices wich are solved via LU factorization here. The un-squared case is handled differently. A direct Lapack driver function (?gelsy) is used here. Therefore the solutions might differ! However, the solution will of course fullfill the equation A * x = B without round off errrors. </para>
        /// <para>For specifiying the rank of A in the unsquare case (q != n), <see cref="ILNumerics.ILMath.eps"/> is used.</para></remarks>
        public static ILRetArray< float > linsolve(ILInArray< float > A, ILInArray< float > B) {
            if (object.Equals(A,null) || object.Equals(B,null))
                throw new ILArgumentException("parameter must not be null!");
            using (ILScope.Enter(A, B)) {
                MatrixProperties props = MatrixProperties.None;
                if (A.Size[0] == A.Size[1]) {
                    props |= MatrixProperties.Square;
                    if (ILMath.istriup(A)) {
                        props |= MatrixProperties.UpperTriangular;
                        return linsolve(A, B, ref props);
                    }
                    if (ILMath.istrilow(A)) {
                        props |= MatrixProperties.LowerTriangular;
                        return linsolve(A, B, ref props);
                    }
                    if (ILMath.ishermitian(A)) {
                        // give cholesky a try
                        props |= MatrixProperties.Hermitian;
                        props |= MatrixProperties.PositivDefinite;
                        ILArray< float> ret = linsolve(A, B, ref props);
                        if (!object.Equals(ret, null)) {
                            return ret;
                        } else {
                            props ^= MatrixProperties.PositivDefinite;
                        }
                    }
                }
                return linsolve(A, B, ref props);
            }
        }

        /// <summary>
        /// Solve linear equation system
        /// </summary>
        /// <param name="A">Matrix A. Size [n x q]</param>
        /// <param name="B">Right hand side B. Size [n x m]</param>
        /// <param name="props">Matrix properties. If defined, no checks are made for the structure of A. If the 
        /// matrix A was found to be (close to or) singular, the 'MatrixProperties.Singular' flag in props will be set. 
        /// This flag should be tested on return, in order to verify the reliability of the solution.</param>
        /// <returns>The solution x solving multiply(A,x) = B. Size [n x m]</returns>
        /// <remarks><para>Depending on the <paramref name="props"/> parameter the equation system will be solved 
        /// differently for special structures of A:
        /// <list type="bullet">
        /// <item>If A is square (q == n) and an <b>upper or lower triangular</b> matrix, the system will directly 
        /// be solved via backward- or forward substitution. Therefore the Lapack function ?trtrs will be used, 
        /// whenever the memory layout of A is suitable. This may be the case even for reference ILArray's! 
        /// <example><code><![CDATA[ILArray<double> A = ILMath.randn(4); // construct 4 x 4 matrix 
        /// A = A.T; // A is a reference array now! The transpose is fast and does not consume much memory
        /// // now construct a right side and solve the equations: 
        /// ILArray<double> B = new ILArray<double> (1.0,2.0,3.0).T; 
        /// ILMath.linsolve(A,B); // ... will be carried out via Lapack, even for all arrays involved being reference arrays! ]]></code></example></item>
        /// <item><para>If A is square and symmetric or hermitian, A will be decomposed into a triangular equation 
        /// system using cholesky factorization and Lapack. The system is than solved using performant Lapack routines.</para>
        /// <para>If during the cholesky factorization A was found to be <b>not positive definite</b> - the 
        /// corresponding flag in props will be cleaned and <c>null</c> will be returned.</para></item>
        /// <item>Otherwise if A is square only, it will be decomposed into upper and lower triangular matrices 
        /// using LU decomposition and Lapack. The triangular system is than solved using performant Lapack routines.</item>
        /// <item>Otherwise, if A is of size [q x n] and q != n, the system is solved using QR decomposition. 
        /// A may be rank deficient. The solution is computed by use of the Lapack routine '?gelsy' and may be a 
        /// reference array.</item>
        /// </list></para>
        /// <para>Compatibility with Matlab<sup>(R)</sup>: If A is square, the algorithm used follows the same 
        /// logic as Matlab up to Rel 14, with the exception of Hessenberg matrices wich are solved via LU 
        /// factorization here. The un-squared case is handled differently. A direct Lapack driver function 
        /// (?gelsy) is used here. Therefore the solutions might differ! However, the solution will of course 
        /// fullfill the equation A * x = B without round off errrors. </para>
        /// <para>For specifiying the rank of A in the unsquare case (q != n), 
        /// <see cref="ILNumerics.ILMath.eps"/> is used.</para></remarks>
        public static ILRetArray< float > linsolve(ILInArray< float > A, ILInArray< float > B, ref MatrixProperties props) {
            if (object.Equals(A,null)) 
                throw new ILArgumentException("input argument A must not be null!"); 
            if (object.Equals(B,null))
                throw new ILArgumentException("input argument B must not be null!");
            using (ILScope.Enter(A, B)) {
                if (A.IsEmpty || B.IsEmpty)
                    return empty< float>(A.Size);
                if (A.Size[0] != B.Size[0])
                    throw new ILArgumentException("number of rows for matrix A must match number of rows for RHS!");
                int info = 0, m = A.Size[0];
                ILArray< float> ret = empty<float>(ILSize.Empty00);
                if (m == A.Size[1]) {
                    props |= MatrixProperties.Square;
                    if ((props & MatrixProperties.LowerTriangular) != 0) {
                        ret.a = linsolveTriLow(A, B, ref info);
                        if (info > 0)
                            props |= MatrixProperties.Singular;
                        return ret;
                    }
                    if ((props & MatrixProperties.UpperTriangular) != 0) {
                        ret.a = linsolveTriUp(A, B, ref info);
                        if (info > 0)
                            props |= MatrixProperties.Singular;
                        return ret;
                    }
                    if ((props & MatrixProperties.Hermitian) != 0) {
                        ILDenseStorage< float> cholFact = A.Storage.copyUpperTriangle(m);
                       
                        Lapack.spotrf('U', m, cholFact.GetArrayForWrite(), m, ref info);
                        if (info > 0) {
                            props ^= MatrixProperties.Hermitian;
                            cholFact.Dispose(); 
                            return null;
                        } else {
                            // solve 
                            ret.a = B.C;
                           
                            Lapack.spotrs('U', m, B.Size[1], cholFact.GetArrayForWrite(), m, ret.GetArrayForWrite(), m, ref info);
                            cholFact.Dispose(); 
                            return ret;
                        }
                    } else {
                        // attempt complete (expensive) LU factorization 
                        ILArray< float> L = A.C;
                        int[] pivInd = ILMemoryPool.Pool.New<int>(m);
                       
                        Lapack.sgetrf(m, m, L.GetArrayForWrite(), m, pivInd, ref info);
                        if (info > 0)
                            props |= MatrixProperties.Singular;
                        ret.a = B.C;
                       
                        Lapack.sgetrs('N', m, B.Size[1], L.GetArrayForWrite(), m, pivInd, ret.GetArrayForWrite(), m, ref info);
                        if (info < 0)
                            throw new ILArgumentException("failed to solve via lapack dgetrs");
                        return ret;
                    }
                } else {
                    // under- / overdetermined system
                    int n = A.Size[1], rank = 0, minMN = (m < n) ? m : n, maxMN = (m > n) ? m : n;
                    int nrhs = B.Size[1];
                    if (B.Size[0] != m)
                        throw new ILArgumentException("right hand side matrix B must match input A!");
                    ILArray< float> tmpA = A.C;
                    if (m < n) {
                        ret.a = zeros< float>(n, nrhs);
                        ret[r(0, m - 1), full] = B.C;
                    } else {
                        ret.a = B.C;
                    }
                    int[] JPVT = new int[n];
                   
                    Lapack.sgelsy(m, n, B.Size[1], tmpA.GetArrayForWrite(), m, ret.GetArrayForWrite(),
                        maxMN, JPVT,  ILMath.MachineParameterSingle.eps,
                        ref rank, ref info);
                    if (n < m) {
                        ret.a = ret[r(0, n - 1), full];
                    }
                    if (rank < minMN)
                        props |= MatrixProperties.RankDeficient;
                    return ret;
                }
            }
        }
        
        /// <summary>
        /// Solve system of linear equations A*x = b, with A being a upper triangular matrix
        /// </summary>
        /// <param name="A">Input matrix of size [n x n], must be upper triangular. No check is made for that!</param>
        /// <param name="B">Solution vector or matrix. Size [n x m]</param>
        /// <param name="singularityDetect">[Output] This value gives the row of A, where a singularity has been detected (if any). If A is not singular, this will be a negative value.</param>
        /// <returns>Solution x solving A * x = b.</returns>
        /// <remarks><para>The solution will be determined via backward substitution</para>
        /// <para>Make sure, A and b are of correct size, since no checks are made for that!</para>
        /// <para>This function is used by ILMath.linsolve. There should be rare need for you to call this function directly.</para>
        /// <para>Elements of A below the main diagonal will not be accessed.</para>
        /// <para>If A has been found to be singular, the array returned will contain NaN values for corresponding elements!</para></remarks>
        public static ILRetArray< float > linsolveTriUp (ILInArray< float > A, ILInArray< float > B, ref int singularityDetect) {
            System.Diagnostics.Debug.Assert(B.Size[1] >= 0); 
            System.Diagnostics.Debug.Assert(B.Size[0] == A.Size[1]); 
            System.Diagnostics.Debug.Assert(A.Size[0] == A.Size[1]);
            using (ILScope.Enter(A, B)) {
                singularityDetect = -1;
                int n = A.Size[0];
                int m = B.Size[1];
                int info = 0;
                ILArray< float> ret = B.C;
               
                float[] retArr = ret.GetArrayForWrite();
                // solve using Lapack
                unsafe {
                    fixed ( float* ptrA = A.GetArrayForRead())
                    fixed ( float* ptrB = ret.GetArrayForWrite()) {
                       
                        Lapack.strtrs('U', 'N', 'N', A.Size[0], B.Size[1], (IntPtr)ptrA, A.Size[0], (IntPtr)ptrB, B.Size[0], ref info);
                    }
                }
                if (info < 0)
                    throw new ILArgumentException("error inside Lapack function ?trtrs for argument: " + (-info));
                if (info > 0) {
                    singularityDetect = info - 1;
                    for (m = 0; m < ret.Size[1]; m++) {
                        info = m * n + singularityDetect;
                        for (int i = singularityDetect; i < n; i++) {
                            retArr[info++] =  float.NaN;
                        }
                    }
                } else {
                    singularityDetect = -1;
                }
                return ret;
            }
        }

        /// <summary>
        /// Solve system of linear equations A*x = b, with A being a lower triangular matrix
        /// </summary>
        /// <param name="A">Input matrix of size [n x n], must be lower triangular. No check is made for that!</param>
        /// <param name="B">Solution vector. Size [n x 1]</param>
        /// <param name="singularityDetect">[Output] This value gives the row of A, where a singularity has been detected (if any). If A is not singular, this will be a negative value.</param>
        /// <returns>Solution x solving A * x = b.</returns>
        /// <remarks><para>The solution will be determined via forward substitution</para>
        /// <para>Make sure, A and b are of correct size, since no checks are made for that!</para>
        /// <para>This function is used by ILMath.linsolve. There should be rare need for you to call this function directly.</para>
        /// <para>Elements of A above the main diagonal will not be accessed.</para>
        /// <para>If A has been found to be singular, the array returned will contain NaN values for corresponding elements!</para></remarks>
        public static ILRetArray< float> linsolveTriLow(ILInArray< float> A, ILInArray< float> B, ref int singularityDetect) {
            System.Diagnostics.Debug.Assert(B.Size[1] >= 0);
            System.Diagnostics.Debug.Assert(B.Size[0] == A.Size[1]);
            System.Diagnostics.Debug.Assert(A.Size[0] == A.Size[1]);
            using (ILScope.Enter(A, B)) {
                singularityDetect = -1;
                int n = A.Size[0];
                int m = B.Size[1];
                int info = 0;
                ILArray< float> ret = B.C;
               
                float[] retArr = ret.GetArrayForWrite();
                // solve using Lapack
                unsafe {
                    fixed ( float* ptrA = A.GetArrayForRead())
                    fixed ( float* ptrB = ret.GetArrayForWrite()) {
                       
                        Lapack.strtrs('L', 'N', 'N', A.Size[0], B.Size[1], (IntPtr)ptrA, A.Size[0], (IntPtr)ptrB, B.Size[0], ref info);
                    }
                }
                if (info < 0)
                    throw new ILArgumentException("error inside Lapack function ?trtrs for argument: " + (-info));
                if (info > 0) {
                    singularityDetect = info - 1;
                    for (m = 0; m < ret.Size[1]; m++) {
                        info = m * n + singularityDetect;
                        for (int i = singularityDetect; i < n; i++) {
                            retArr[info++] =  float.NaN;
                        }
                    }
                } else {
                    singularityDetect = -1;
                }
                return ret;
            }
        }

        
        /// <summary>
        /// Solve linear equation system
        /// </summary>
        /// <param name="A">Matrix A. Size [n x q]</param>
        /// <param name="B">Right hand side B. Size [n x m]</param>
        /// <returns>Solution x solving the equation system: multiply(A, x) = B. Size [n x m]</returns>
        /// <remarks><para>Depending on the structure and properties of A, the equation system will be solved in different ways:
        /// <list type="bullet">
        /// <item>If A is square (q == n) and an <b>upper or lower triangular</b> matrix, the system will directly be solved via backward- or forward substitution. Therefore the Lapack function ?trtrs will be used, whenever the memory layout of A is suitable. This may be the case even for reference ILArray's! 
        /// <example><code><![CDATA[ILArray<double> A = ILMath.randn(4); // construct 4 x 4 matrix 
        /// A = A.T; // A is a reference array now! The transpose is fast and does not consume much memory
        /// // now construct a right side and solve the equations: 
        /// ILArray<double> B = new ILArray<double> (1.0,2.0,3.0).T; 
        /// ILMath.linsolve(A,B); // ... will be carried out via Lapack, even for all arrays involved being reference arrays! ]]></code></example></item>
        /// <item><para>if A is square and symmetric or hermitian, A will be decomposed into a triangular equation system using cholesky factorization and Lapack. The system is than solved using performant Lapack routines.</para>
        /// <para>If during the cholesky factorization A was found to be <b>not positive definite</b> - the cholesky factorization is canceled. </para></item>
        /// <item>otherwise, if A is square only, it will be decomposed into upper and lower triangular martices using LU decomposition and Lapack. The triangular system is than solved using performant Lapack routines.</item>
        /// <item>otherwise, if A is of size [q x n] and q != n, the system is solved using QR decomposition. A may be rank deficient. The solution is computed by use of the Lapack routine '?gelsy' and may be a reference array.</item>
        /// </list></para>
        /// <para>Compatibility with Matlab<sup>(R)</sup>: If A is square, the algorithm used follows the same logic as Matlab up to Rel 14, with the exception of Hessenberg matrices wich are solved via LU factorization here. The un-squared case is handled differently. A direct Lapack driver function (?gelsy) is used here. Therefore the solutions might differ! However, the solution will of course fullfill the equation A * x = B without round off errrors. </para>
        /// <para>For specifiying the rank of A in the unsquare case (q != n), <see cref="ILNumerics.ILMath.eps"/> is used.</para></remarks>
        public static ILRetArray< fcomplex > linsolve(ILInArray< fcomplex > A, ILInArray< fcomplex > B) {
            if (object.Equals(A,null) || object.Equals(B,null))
                throw new ILArgumentException("parameter must not be null!");
            using (ILScope.Enter(A, B)) {
                MatrixProperties props = MatrixProperties.None;
                if (A.Size[0] == A.Size[1]) {
                    props |= MatrixProperties.Square;
                    if (ILMath.istriup(A)) {
                        props |= MatrixProperties.UpperTriangular;
                        return linsolve(A, B, ref props);
                    }
                    if (ILMath.istrilow(A)) {
                        props |= MatrixProperties.LowerTriangular;
                        return linsolve(A, B, ref props);
                    }
                    if (ILMath.ishermitian(A)) {
                        // give cholesky a try
                        props |= MatrixProperties.Hermitian;
                        props |= MatrixProperties.PositivDefinite;
                        ILArray< fcomplex> ret = linsolve(A, B, ref props);
                        if (!object.Equals(ret, null)) {
                            return ret;
                        } else {
                            props ^= MatrixProperties.PositivDefinite;
                        }
                    }
                }
                return linsolve(A, B, ref props);
            }
        }

        /// <summary>
        /// Solve linear equation system
        /// </summary>
        /// <param name="A">Matrix A. Size [n x q]</param>
        /// <param name="B">Right hand side B. Size [n x m]</param>
        /// <param name="props">Matrix properties. If defined, no checks are made for the structure of A. If the 
        /// matrix A was found to be (close to or) singular, the 'MatrixProperties.Singular' flag in props will be set. 
        /// This flag should be tested on return, in order to verify the reliability of the solution.</param>
        /// <returns>The solution x solving multiply(A,x) = B. Size [n x m]</returns>
        /// <remarks><para>Depending on the <paramref name="props"/> parameter the equation system will be solved 
        /// differently for special structures of A:
        /// <list type="bullet">
        /// <item>If A is square (q == n) and an <b>upper or lower triangular</b> matrix, the system will directly 
        /// be solved via backward- or forward substitution. Therefore the Lapack function ?trtrs will be used, 
        /// whenever the memory layout of A is suitable. This may be the case even for reference ILArray's! 
        /// <example><code><![CDATA[ILArray<double> A = ILMath.randn(4); // construct 4 x 4 matrix 
        /// A = A.T; // A is a reference array now! The transpose is fast and does not consume much memory
        /// // now construct a right side and solve the equations: 
        /// ILArray<double> B = new ILArray<double> (1.0,2.0,3.0).T; 
        /// ILMath.linsolve(A,B); // ... will be carried out via Lapack, even for all arrays involved being reference arrays! ]]></code></example></item>
        /// <item><para>If A is square and symmetric or hermitian, A will be decomposed into a triangular equation 
        /// system using cholesky factorization and Lapack. The system is than solved using performant Lapack routines.</para>
        /// <para>If during the cholesky factorization A was found to be <b>not positive definite</b> - the 
        /// corresponding flag in props will be cleaned and <c>null</c> will be returned.</para></item>
        /// <item>Otherwise if A is square only, it will be decomposed into upper and lower triangular matrices 
        /// using LU decomposition and Lapack. The triangular system is than solved using performant Lapack routines.</item>
        /// <item>Otherwise, if A is of size [q x n] and q != n, the system is solved using QR decomposition. 
        /// A may be rank deficient. The solution is computed by use of the Lapack routine '?gelsy' and may be a 
        /// reference array.</item>
        /// </list></para>
        /// <para>Compatibility with Matlab<sup>(R)</sup>: If A is square, the algorithm used follows the same 
        /// logic as Matlab up to Rel 14, with the exception of Hessenberg matrices wich are solved via LU 
        /// factorization here. The un-squared case is handled differently. A direct Lapack driver function 
        /// (?gelsy) is used here. Therefore the solutions might differ! However, the solution will of course 
        /// fullfill the equation A * x = B without round off errrors. </para>
        /// <para>For specifiying the rank of A in the unsquare case (q != n), 
        /// <see cref="ILNumerics.ILMath.eps"/> is used.</para></remarks>
        public static ILRetArray< fcomplex > linsolve(ILInArray< fcomplex > A, ILInArray< fcomplex > B, ref MatrixProperties props) {
            if (object.Equals(A,null)) 
                throw new ILArgumentException("input argument A must not be null!"); 
            if (object.Equals(B,null))
                throw new ILArgumentException("input argument B must not be null!");
            using (ILScope.Enter(A, B)) {
                if (A.IsEmpty || B.IsEmpty)
                    return empty< fcomplex>(A.Size);
                if (A.Size[0] != B.Size[0])
                    throw new ILArgumentException("number of rows for matrix A must match number of rows for RHS!");
                int info = 0, m = A.Size[0];
                ILArray< fcomplex> ret = empty<fcomplex>(ILSize.Empty00);
                if (m == A.Size[1]) {
                    props |= MatrixProperties.Square;
                    if ((props & MatrixProperties.LowerTriangular) != 0) {
                        ret.a = linsolveTriLow(A, B, ref info);
                        if (info > 0)
                            props |= MatrixProperties.Singular;
                        return ret;
                    }
                    if ((props & MatrixProperties.UpperTriangular) != 0) {
                        ret.a = linsolveTriUp(A, B, ref info);
                        if (info > 0)
                            props |= MatrixProperties.Singular;
                        return ret;
                    }
                    if ((props & MatrixProperties.Hermitian) != 0) {
                        ILDenseStorage< fcomplex> cholFact = A.Storage.copyUpperTriangle(m);
                       
                        Lapack.cpotrf('U', m, cholFact.GetArrayForWrite(), m, ref info);
                        if (info > 0) {
                            props ^= MatrixProperties.Hermitian;
                            cholFact.Dispose(); 
                            return null;
                        } else {
                            // solve 
                            ret.a = B.C;
                           
                            Lapack.cpotrs('U', m, B.Size[1], cholFact.GetArrayForWrite(), m, ret.GetArrayForWrite(), m, ref info);
                            cholFact.Dispose(); 
                            return ret;
                        }
                    } else {
                        // attempt complete (expensive) LU factorization 
                        ILArray< fcomplex> L = A.C;
                        int[] pivInd = ILMemoryPool.Pool.New<int>(m);
                       
                        Lapack.cgetrf(m, m, L.GetArrayForWrite(), m, pivInd, ref info);
                        if (info > 0)
                            props |= MatrixProperties.Singular;
                        ret.a = B.C;
                       
                        Lapack.cgetrs('N', m, B.Size[1], L.GetArrayForWrite(), m, pivInd, ret.GetArrayForWrite(), m, ref info);
                        if (info < 0)
                            throw new ILArgumentException("failed to solve via lapack dgetrs");
                        return ret;
                    }
                } else {
                    // under- / overdetermined system
                    int n = A.Size[1], rank = 0, minMN = (m < n) ? m : n, maxMN = (m > n) ? m : n;
                    int nrhs = B.Size[1];
                    if (B.Size[0] != m)
                        throw new ILArgumentException("right hand side matrix B must match input A!");
                    ILArray< fcomplex> tmpA = A.C;
                    if (m < n) {
                        ret.a = zeros< fcomplex>(n, nrhs);
                        ret[r(0, m - 1), full] = B.C;
                    } else {
                        ret.a = B.C;
                    }
                    int[] JPVT = new int[n];
                   
                    Lapack.cgelsy(m, n, B.Size[1], tmpA.GetArrayForWrite(), m, ret.GetArrayForWrite(),
                        maxMN, JPVT,  ILMath.MachineParameterSingle.eps,
                        ref rank, ref info);
                    if (n < m) {
                        ret.a = ret[r(0, n - 1), full];
                    }
                    if (rank < minMN)
                        props |= MatrixProperties.RankDeficient;
                    return ret;
                }
            }
        }
        
        /// <summary>
        /// Solve system of linear equations A*x = b, with A being a upper triangular matrix
        /// </summary>
        /// <param name="A">Input matrix of size [n x n], must be upper triangular. No check is made for that!</param>
        /// <param name="B">Solution vector or matrix. Size [n x m]</param>
        /// <param name="singularityDetect">[Output] This value gives the row of A, where a singularity has been detected (if any). If A is not singular, this will be a negative value.</param>
        /// <returns>Solution x solving A * x = b.</returns>
        /// <remarks><para>The solution will be determined via backward substitution</para>
        /// <para>Make sure, A and b are of correct size, since no checks are made for that!</para>
        /// <para>This function is used by ILMath.linsolve. There should be rare need for you to call this function directly.</para>
        /// <para>Elements of A below the main diagonal will not be accessed.</para>
        /// <para>If A has been found to be singular, the array returned will contain NaN values for corresponding elements!</para></remarks>
        public static ILRetArray< fcomplex > linsolveTriUp (ILInArray< fcomplex > A, ILInArray< fcomplex > B, ref int singularityDetect) {
            System.Diagnostics.Debug.Assert(B.Size[1] >= 0); 
            System.Diagnostics.Debug.Assert(B.Size[0] == A.Size[1]); 
            System.Diagnostics.Debug.Assert(A.Size[0] == A.Size[1]);
            using (ILScope.Enter(A, B)) {
                singularityDetect = -1;
                int n = A.Size[0];
                int m = B.Size[1];
                int info = 0;
                ILArray< fcomplex> ret = B.C;
               
                fcomplex[] retArr = ret.GetArrayForWrite();
                // solve using Lapack
                unsafe {
                    fixed ( fcomplex* ptrA = A.GetArrayForRead())
                    fixed ( fcomplex* ptrB = ret.GetArrayForWrite()) {
                       
                        Lapack.ctrtrs('U', 'N', 'N', A.Size[0], B.Size[1], (IntPtr)ptrA, A.Size[0], (IntPtr)ptrB, B.Size[0], ref info);
                    }
                }
                if (info < 0)
                    throw new ILArgumentException("error inside Lapack function ?trtrs for argument: " + (-info));
                if (info > 0) {
                    singularityDetect = info - 1;
                    for (m = 0; m < ret.Size[1]; m++) {
                        info = m * n + singularityDetect;
                        for (int i = singularityDetect; i < n; i++) {
                            retArr[info++] =  new fcomplex(float.NaN,float.NaN);
                        }
                    }
                } else {
                    singularityDetect = -1;
                }
                return ret;
            }
        }

        /// <summary>
        /// Solve system of linear equations A*x = b, with A being a lower triangular matrix
        /// </summary>
        /// <param name="A">Input matrix of size [n x n], must be lower triangular. No check is made for that!</param>
        /// <param name="B">Solution vector. Size [n x 1]</param>
        /// <param name="singularityDetect">[Output] This value gives the row of A, where a singularity has been detected (if any). If A is not singular, this will be a negative value.</param>
        /// <returns>Solution x solving A * x = b.</returns>
        /// <remarks><para>The solution will be determined via forward substitution</para>
        /// <para>Make sure, A and b are of correct size, since no checks are made for that!</para>
        /// <para>This function is used by ILMath.linsolve. There should be rare need for you to call this function directly.</para>
        /// <para>Elements of A above the main diagonal will not be accessed.</para>
        /// <para>If A has been found to be singular, the array returned will contain NaN values for corresponding elements!</para></remarks>
        public static ILRetArray< fcomplex> linsolveTriLow(ILInArray< fcomplex> A, ILInArray< fcomplex> B, ref int singularityDetect) {
            System.Diagnostics.Debug.Assert(B.Size[1] >= 0);
            System.Diagnostics.Debug.Assert(B.Size[0] == A.Size[1]);
            System.Diagnostics.Debug.Assert(A.Size[0] == A.Size[1]);
            using (ILScope.Enter(A, B)) {
                singularityDetect = -1;
                int n = A.Size[0];
                int m = B.Size[1];
                int info = 0;
                ILArray< fcomplex> ret = B.C;
               
                fcomplex[] retArr = ret.GetArrayForWrite();
                // solve using Lapack
                unsafe {
                    fixed ( fcomplex* ptrA = A.GetArrayForRead())
                    fixed ( fcomplex* ptrB = ret.GetArrayForWrite()) {
                       
                        Lapack.ctrtrs('L', 'N', 'N', A.Size[0], B.Size[1], (IntPtr)ptrA, A.Size[0], (IntPtr)ptrB, B.Size[0], ref info);
                    }
                }
                if (info < 0)
                    throw new ILArgumentException("error inside Lapack function ?trtrs for argument: " + (-info));
                if (info > 0) {
                    singularityDetect = info - 1;
                    for (m = 0; m < ret.Size[1]; m++) {
                        info = m * n + singularityDetect;
                        for (int i = singularityDetect; i < n; i++) {
                            retArr[info++] =  new fcomplex(float.NaN,float.NaN);
                        }
                    }
                } else {
                    singularityDetect = -1;
                }
                return ret;
            }
        }

        
        /// <summary>
        /// Solve linear equation system
        /// </summary>
        /// <param name="A">Matrix A. Size [n x q]</param>
        /// <param name="B">Right hand side B. Size [n x m]</param>
        /// <returns>Solution x solving the equation system: multiply(A, x) = B. Size [n x m]</returns>
        /// <remarks><para>Depending on the structure and properties of A, the equation system will be solved in different ways:
        /// <list type="bullet">
        /// <item>If A is square (q == n) and an <b>upper or lower triangular</b> matrix, the system will directly be solved via backward- or forward substitution. Therefore the Lapack function ?trtrs will be used, whenever the memory layout of A is suitable. This may be the case even for reference ILArray's! 
        /// <example><code><![CDATA[ILArray<double> A = ILMath.randn(4); // construct 4 x 4 matrix 
        /// A = A.T; // A is a reference array now! The transpose is fast and does not consume much memory
        /// // now construct a right side and solve the equations: 
        /// ILArray<double> B = new ILArray<double> (1.0,2.0,3.0).T; 
        /// ILMath.linsolve(A,B); // ... will be carried out via Lapack, even for all arrays involved being reference arrays! ]]></code></example></item>
        /// <item><para>if A is square and symmetric or hermitian, A will be decomposed into a triangular equation system using cholesky factorization and Lapack. The system is than solved using performant Lapack routines.</para>
        /// <para>If during the cholesky factorization A was found to be <b>not positive definite</b> - the cholesky factorization is canceled. </para></item>
        /// <item>otherwise, if A is square only, it will be decomposed into upper and lower triangular martices using LU decomposition and Lapack. The triangular system is than solved using performant Lapack routines.</item>
        /// <item>otherwise, if A is of size [q x n] and q != n, the system is solved using QR decomposition. A may be rank deficient. The solution is computed by use of the Lapack routine '?gelsy' and may be a reference array.</item>
        /// </list></para>
        /// <para>Compatibility with Matlab<sup>(R)</sup>: If A is square, the algorithm used follows the same logic as Matlab up to Rel 14, with the exception of Hessenberg matrices wich are solved via LU factorization here. The un-squared case is handled differently. A direct Lapack driver function (?gelsy) is used here. Therefore the solutions might differ! However, the solution will of course fullfill the equation A * x = B without round off errrors. </para>
        /// <para>For specifiying the rank of A in the unsquare case (q != n), <see cref="ILNumerics.ILMath.eps"/> is used.</para></remarks>
        public static ILRetArray< complex > linsolve(ILInArray< complex > A, ILInArray< complex > B) {
            if (object.Equals(A,null) || object.Equals(B,null))
                throw new ILArgumentException("parameter must not be null!");
            using (ILScope.Enter(A, B)) {
                MatrixProperties props = MatrixProperties.None;
                if (A.Size[0] == A.Size[1]) {
                    props |= MatrixProperties.Square;
                    if (ILMath.istriup(A)) {
                        props |= MatrixProperties.UpperTriangular;
                        return linsolve(A, B, ref props);
                    }
                    if (ILMath.istrilow(A)) {
                        props |= MatrixProperties.LowerTriangular;
                        return linsolve(A, B, ref props);
                    }
                    if (ILMath.ishermitian(A)) {
                        // give cholesky a try
                        props |= MatrixProperties.Hermitian;
                        props |= MatrixProperties.PositivDefinite;
                        ILArray< complex> ret = linsolve(A, B, ref props);
                        if (!object.Equals(ret, null)) {
                            return ret;
                        } else {
                            props ^= MatrixProperties.PositivDefinite;
                        }
                    }
                }
                return linsolve(A, B, ref props);
            }
        }

        /// <summary>
        /// Solve linear equation system
        /// </summary>
        /// <param name="A">Matrix A. Size [n x q]</param>
        /// <param name="B">Right hand side B. Size [n x m]</param>
        /// <param name="props">Matrix properties. If defined, no checks are made for the structure of A. If the 
        /// matrix A was found to be (close to or) singular, the 'MatrixProperties.Singular' flag in props will be set. 
        /// This flag should be tested on return, in order to verify the reliability of the solution.</param>
        /// <returns>The solution x solving multiply(A,x) = B. Size [n x m]</returns>
        /// <remarks><para>Depending on the <paramref name="props"/> parameter the equation system will be solved 
        /// differently for special structures of A:
        /// <list type="bullet">
        /// <item>If A is square (q == n) and an <b>upper or lower triangular</b> matrix, the system will directly 
        /// be solved via backward- or forward substitution. Therefore the Lapack function ?trtrs will be used, 
        /// whenever the memory layout of A is suitable. This may be the case even for reference ILArray's! 
        /// <example><code><![CDATA[ILArray<double> A = ILMath.randn(4); // construct 4 x 4 matrix 
        /// A = A.T; // A is a reference array now! The transpose is fast and does not consume much memory
        /// // now construct a right side and solve the equations: 
        /// ILArray<double> B = new ILArray<double> (1.0,2.0,3.0).T; 
        /// ILMath.linsolve(A,B); // ... will be carried out via Lapack, even for all arrays involved being reference arrays! ]]></code></example></item>
        /// <item><para>If A is square and symmetric or hermitian, A will be decomposed into a triangular equation 
        /// system using cholesky factorization and Lapack. The system is than solved using performant Lapack routines.</para>
        /// <para>If during the cholesky factorization A was found to be <b>not positive definite</b> - the 
        /// corresponding flag in props will be cleaned and <c>null</c> will be returned.</para></item>
        /// <item>Otherwise if A is square only, it will be decomposed into upper and lower triangular matrices 
        /// using LU decomposition and Lapack. The triangular system is than solved using performant Lapack routines.</item>
        /// <item>Otherwise, if A is of size [q x n] and q != n, the system is solved using QR decomposition. 
        /// A may be rank deficient. The solution is computed by use of the Lapack routine '?gelsy' and may be a 
        /// reference array.</item>
        /// </list></para>
        /// <para>Compatibility with Matlab<sup>(R)</sup>: If A is square, the algorithm used follows the same 
        /// logic as Matlab up to Rel 14, with the exception of Hessenberg matrices wich are solved via LU 
        /// factorization here. The un-squared case is handled differently. A direct Lapack driver function 
        /// (?gelsy) is used here. Therefore the solutions might differ! However, the solution will of course 
        /// fullfill the equation A * x = B without round off errrors. </para>
        /// <para>For specifiying the rank of A in the unsquare case (q != n), 
        /// <see cref="ILNumerics.ILMath.eps"/> is used.</para></remarks>
        public static ILRetArray< complex > linsolve(ILInArray< complex > A, ILInArray< complex > B, ref MatrixProperties props) {
            if (object.Equals(A,null)) 
                throw new ILArgumentException("input argument A must not be null!"); 
            if (object.Equals(B,null))
                throw new ILArgumentException("input argument B must not be null!");
            using (ILScope.Enter(A, B)) {
                if (A.IsEmpty || B.IsEmpty)
                    return empty< complex>(A.Size);
                if (A.Size[0] != B.Size[0])
                    throw new ILArgumentException("number of rows for matrix A must match number of rows for RHS!");
                int info = 0, m = A.Size[0];
                ILArray< complex> ret = empty<complex>(ILSize.Empty00);
                if (m == A.Size[1]) {
                    props |= MatrixProperties.Square;
                    if ((props & MatrixProperties.LowerTriangular) != 0) {
                        ret.a = linsolveTriLow(A, B, ref info);
                        if (info > 0)
                            props |= MatrixProperties.Singular;
                        return ret;
                    }
                    if ((props & MatrixProperties.UpperTriangular) != 0) {
                        ret.a = linsolveTriUp(A, B, ref info);
                        if (info > 0)
                            props |= MatrixProperties.Singular;
                        return ret;
                    }
                    if ((props & MatrixProperties.Hermitian) != 0) {
                        ILDenseStorage< complex> cholFact = A.Storage.copyUpperTriangle(m);
                       
                        Lapack.zpotrf('U', m, cholFact.GetArrayForWrite(), m, ref info);
                        if (info > 0) {
                            props ^= MatrixProperties.Hermitian;
                            cholFact.Dispose(); 
                            return null;
                        } else {
                            // solve 
                            ret.a = B.C;
                           
                            Lapack.zpotrs('U', m, B.Size[1], cholFact.GetArrayForWrite(), m, ret.GetArrayForWrite(), m, ref info);
                            cholFact.Dispose(); 
                            return ret;
                        }
                    } else {
                        // attempt complete (expensive) LU factorization 
                        ILArray< complex> L = A.C;
                        int[] pivInd = ILMemoryPool.Pool.New<int>(m);
                       
                        Lapack.zgetrf(m, m, L.GetArrayForWrite(), m, pivInd, ref info);
                        if (info > 0)
                            props |= MatrixProperties.Singular;
                        ret.a = B.C;
                       
                        Lapack.zgetrs('N', m, B.Size[1], L.GetArrayForWrite(), m, pivInd, ret.GetArrayForWrite(), m, ref info);
                        if (info < 0)
                            throw new ILArgumentException("failed to solve via lapack dgetrs");
                        return ret;
                    }
                } else {
                    // under- / overdetermined system
                    int n = A.Size[1], rank = 0, minMN = (m < n) ? m : n, maxMN = (m > n) ? m : n;
                    int nrhs = B.Size[1];
                    if (B.Size[0] != m)
                        throw new ILArgumentException("right hand side matrix B must match input A!");
                    ILArray< complex> tmpA = A.C;
                    if (m < n) {
                        ret.a = zeros< complex>(n, nrhs);
                        ret[r(0, m - 1), full] = B.C;
                    } else {
                        ret.a = B.C;
                    }
                    int[] JPVT = new int[n];
                   
                    Lapack.zgelsy(m, n, B.Size[1], tmpA.GetArrayForWrite(), m, ret.GetArrayForWrite(),
                        maxMN, JPVT,  ILMath.MachineParameterDouble.eps,
                        ref rank, ref info);
                    if (n < m) {
                        ret.a = ret[r(0, n - 1), full];
                    }
                    if (rank < minMN)
                        props |= MatrixProperties.RankDeficient;
                    return ret;
                }
            }
        }
        
        /// <summary>
        /// Solve system of linear equations A*x = b, with A being a upper triangular matrix
        /// </summary>
        /// <param name="A">Input matrix of size [n x n], must be upper triangular. No check is made for that!</param>
        /// <param name="B">Solution vector or matrix. Size [n x m]</param>
        /// <param name="singularityDetect">[Output] This value gives the row of A, where a singularity has been detected (if any). If A is not singular, this will be a negative value.</param>
        /// <returns>Solution x solving A * x = b.</returns>
        /// <remarks><para>The solution will be determined via backward substitution</para>
        /// <para>Make sure, A and b are of correct size, since no checks are made for that!</para>
        /// <para>This function is used by ILMath.linsolve. There should be rare need for you to call this function directly.</para>
        /// <para>Elements of A below the main diagonal will not be accessed.</para>
        /// <para>If A has been found to be singular, the array returned will contain NaN values for corresponding elements!</para></remarks>
        public static ILRetArray< complex > linsolveTriUp (ILInArray< complex > A, ILInArray< complex > B, ref int singularityDetect) {
            System.Diagnostics.Debug.Assert(B.Size[1] >= 0); 
            System.Diagnostics.Debug.Assert(B.Size[0] == A.Size[1]); 
            System.Diagnostics.Debug.Assert(A.Size[0] == A.Size[1]);
            using (ILScope.Enter(A, B)) {
                singularityDetect = -1;
                int n = A.Size[0];
                int m = B.Size[1];
                int info = 0;
                ILArray< complex> ret = B.C;
               
                complex[] retArr = ret.GetArrayForWrite();
                // solve using Lapack
                unsafe {
                    fixed ( complex* ptrA = A.GetArrayForRead())
                    fixed ( complex* ptrB = ret.GetArrayForWrite()) {
                       
                        Lapack.ztrtrs('U', 'N', 'N', A.Size[0], B.Size[1], (IntPtr)ptrA, A.Size[0], (IntPtr)ptrB, B.Size[0], ref info);
                    }
                }
                if (info < 0)
                    throw new ILArgumentException("error inside Lapack function ?trtrs for argument: " + (-info));
                if (info > 0) {
                    singularityDetect = info - 1;
                    for (m = 0; m < ret.Size[1]; m++) {
                        info = m * n + singularityDetect;
                        for (int i = singularityDetect; i < n; i++) {
                            retArr[info++] =  new complex(double.NaN,double.NaN);
                        }
                    }
                } else {
                    singularityDetect = -1;
                }
                return ret;
            }
        }

        /// <summary>
        /// Solve system of linear equations A*x = b, with A being a lower triangular matrix
        /// </summary>
        /// <param name="A">Input matrix of size [n x n], must be lower triangular. No check is made for that!</param>
        /// <param name="B">Solution vector. Size [n x 1]</param>
        /// <param name="singularityDetect">[Output] This value gives the row of A, where a singularity has been detected (if any). If A is not singular, this will be a negative value.</param>
        /// <returns>Solution x solving A * x = b.</returns>
        /// <remarks><para>The solution will be determined via forward substitution</para>
        /// <para>Make sure, A and b are of correct size, since no checks are made for that!</para>
        /// <para>This function is used by ILMath.linsolve. There should be rare need for you to call this function directly.</para>
        /// <para>Elements of A above the main diagonal will not be accessed.</para>
        /// <para>If A has been found to be singular, the array returned will contain NaN values for corresponding elements!</para></remarks>
        public static ILRetArray< complex> linsolveTriLow(ILInArray< complex> A, ILInArray< complex> B, ref int singularityDetect) {
            System.Diagnostics.Debug.Assert(B.Size[1] >= 0);
            System.Diagnostics.Debug.Assert(B.Size[0] == A.Size[1]);
            System.Diagnostics.Debug.Assert(A.Size[0] == A.Size[1]);
            using (ILScope.Enter(A, B)) {
                singularityDetect = -1;
                int n = A.Size[0];
                int m = B.Size[1];
                int info = 0;
                ILArray< complex> ret = B.C;
               
                complex[] retArr = ret.GetArrayForWrite();
                // solve using Lapack
                unsafe {
                    fixed ( complex* ptrA = A.GetArrayForRead())
                    fixed ( complex* ptrB = ret.GetArrayForWrite()) {
                       
                        Lapack.ztrtrs('L', 'N', 'N', A.Size[0], B.Size[1], (IntPtr)ptrA, A.Size[0], (IntPtr)ptrB, B.Size[0], ref info);
                    }
                }
                if (info < 0)
                    throw new ILArgumentException("error inside Lapack function ?trtrs for argument: " + (-info));
                if (info > 0) {
                    singularityDetect = info - 1;
                    for (m = 0; m < ret.Size[1]; m++) {
                        info = m * n + singularityDetect;
                        for (int i = singularityDetect; i < n; i++) {
                            retArr[info++] =  new complex(double.NaN,double.NaN);
                        }
                    }
                } else {
                    singularityDetect = -1;
                }
                return ret;
            }
        }


#endregion HYCALPER AUTO GENERATED CODE

    }
}
