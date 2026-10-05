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
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix A</param>
        /// <returns>Vector with min(M,N) singular values of A as column vector</returns>
        public static ILRetArray< double > svd(ILInArray< double > A) {
            return svd(A,null, null, false, false);
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix A</param>
        /// <returns>Vector with min(M,N) singular values of A as column vector</returns>
        public static ILRetArray< float > svd(ILInArray< float > A) {
            return svd(A,null, null, false, false);
        }
        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix A</param>
        /// <returns>Vector with min(M,N) singular values of A as column vector</returns>
        public static ILRetArray< float > svd(ILInArray< fcomplex > A) {
            return svd(A,null, null, false, false);
        }
        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix A</param>
        /// <returns>Vector with min(M,N) singular values of A as column vector</returns>
        public static ILRetArray< double > svd(ILInArray< complex > A) {
            return svd(A,null, null, false, false);
        }

#endregion HYCALPER AUTO GENERATED CODE


        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="U">[Output] Left singular vectors of A as columns of matrix U. 
        /// Setting this parameter to a non-null value signals the need of returning those values.</param>
        /// <returns>Singluar values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< double > svd(ILInArray< double > A, ILOutArray< double > U) {
            return svd(A, U, null , false,false);
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="U">[Output] Left singular vectors of A as columns of matrix U. 
        /// Setting this parameter to a non-null value signals the need of returning those values.</param>
        /// <returns>Singluar values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< float > svd(ILInArray< float > A, ILOutArray< float > U) {
            return svd(A, U, null , false,false);
        }
        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="U">[Output] Left singular vectors of A as columns of matrix U. 
        /// Setting this parameter to a non-null value signals the need of returning those values.</param>
        /// <returns>Singluar values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< float > svd(ILInArray< fcomplex > A, ILOutArray< fcomplex > U) {
            return svd(A, U, null , false,false);
        }
        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="U">[Output] Left singular vectors of A as columns of matrix U. 
        /// Setting this parameter to a non-null value signals the need of returning those values.</param>
        /// <returns>Singluar values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< double > svd(ILInArray< complex > A, ILOutArray< complex > U) {
            return svd(A, U, null , false,false);
        }

#endregion HYCALPER AUTO GENERATED CODE


        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="outU">[Output] Left singular vectors of A as columns of matrix outU. 
        /// Setting this parameter to a non-null value (e.g. 'empty') signals the need of returning those values.</param>
        /// <param name="small">If true: return only first min(M,N) columns of outU will be 
        /// of size [min(M,N),min(M,N)]</param>
        /// <returns>Singluar values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< double > svd(ILInArray< double > A, ILOutArray< double > outU, bool small) {
            return svd(A, outU, null, small,false);
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="outU">[Output] Left singular vectors of A as columns of matrix outU. 
        /// Setting this parameter to a non-null value (e.g. 'empty') signals the need of returning those values.</param>
        /// <param name="small">If true: return only first min(M,N) columns of outU will be 
        /// of size [min(M,N),min(M,N)]</param>
        /// <returns>Singluar values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< float > svd(ILInArray< float > A, ILOutArray< float > outU, bool small) {
            return svd(A, outU, null, small,false);
        }
        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="outU">[Output] Left singular vectors of A as columns of matrix outU. 
        /// Setting this parameter to a non-null value (e.g. 'empty') signals the need of returning those values.</param>
        /// <param name="small">If true: return only first min(M,N) columns of outU will be 
        /// of size [min(M,N),min(M,N)]</param>
        /// <returns>Singluar values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< float > svd(ILInArray< fcomplex > A, ILOutArray< fcomplex > outU, bool small) {
            return svd(A, outU, null, small,false);
        }
        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="outU">[Output] Left singular vectors of A as columns of matrix outU. 
        /// Setting this parameter to a non-null value (e.g. 'empty') signals the need of returning those values.</param>
        /// <param name="small">If true: return only first min(M,N) columns of outU will be 
        /// of size [min(M,N),min(M,N)]</param>
        /// <returns>Singluar values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< double > svd(ILInArray< complex > A, ILOutArray< complex > outU, bool small) {
            return svd(A, outU, null, small,false);
        }

#endregion HYCALPER AUTO GENERATED CODE
       

        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="outU">[Output] Left singular vectors of A as columns of matrix U. 
        /// Setting this parameter to a non-null value signals the need of returning those values.</param>
        /// <param name="outV">[Output] Right singular vectors of X as rows of matrix V.
        /// This parameter must not be null. It might be an empty array on input.</param>
        /// <returns>Singluar values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< double > svd(ILInArray< double > A, ILOutArray< double > outU, ILOutArray< double > outV) {
            return svd(A, outU, outV, false,false);
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="outU">[Output] Left singular vectors of A as columns of matrix U. 
        /// Setting this parameter to a non-null value signals the need of returning those values.</param>
        /// <param name="outV">[Output] Right singular vectors of X as rows of matrix V.
        /// This parameter must not be null. It might be an empty array on input.</param>
        /// <returns>Singluar values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< float > svd(ILInArray< float > A, ILOutArray< float > outU, ILOutArray< float > outV) {
            return svd(A, outU, outV, false,false);
        }
        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="outU">[Output] Left singular vectors of A as columns of matrix U. 
        /// Setting this parameter to a non-null value signals the need of returning those values.</param>
        /// <param name="outV">[Output] Right singular vectors of X as rows of matrix V.
        /// This parameter must not be null. It might be an empty array on input.</param>
        /// <returns>Singluar values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< float > svd(ILInArray< fcomplex > A, ILOutArray< fcomplex > outU, ILOutArray< fcomplex > outV) {
            return svd(A, outU, outV, false,false);
        }
        /// <summary>
        /// Singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="outU">[Output] Left singular vectors of A as columns of matrix U. 
        /// Setting this parameter to a non-null value signals the need of returning those values.</param>
        /// <param name="outV">[Output] Right singular vectors of X as rows of matrix V.
        /// This parameter must not be null. It might be an empty array on input.</param>
        /// <returns>Singluar values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< double > svd(ILInArray< complex > A, ILOutArray< complex > outU, ILOutArray< complex > outV) {
            return svd(A, outU, outV, false,false);
        }

#endregion HYCALPER AUTO GENERATED CODE


        /// <summary>
        /// singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="outU">[Output] Left singular vectors of A as columns of matrix outU. 
        /// Setting this parameter to a non-null value signals the need of returning those values.</param>
        /// <param name="outV">[Output] Right singular vectors of X as rows of matrix outV.
        /// This parameter must not be null. It might be an empty array on input.</param>
        /// <param name="small">If true: return only first min(M,N) columns of outU and S (returned) will be 
        /// of size [min(M,N),min(M,N)]</param>
        /// <param name="discardFiniteTest">If true: the matrix given will not be checked for infinte or NaN values. If such elements 
        /// exist nevertheless, this may result in failing convergence or error. In worst case 
        /// the function may hang inside the Lapack lib! Use with care! </param>
        /// <returns>Singular values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< double> svd( ILInArray< double> A, ILOutArray< double> outU, 
                                                                ILOutArray< double> outV, bool small, bool discardFiniteTest ) {
            if (object.Equals(A,null))
                throw new ILArgumentException("input matrix X must not be null"); 
            using (ILScope.Enter(A)) {
                
                if (!A.IsMatrix)
                    throw new ILArgumentSizeException("svd is defined for matrices only");
                // early exit for small matrices
                if (A.Size[1] < 4 && A.Size[0] == A.Size[1]) {
                    switch (A.Size[0]) {
                        case 1:
                            if (!Object.Equals(outU, null))
                                outU.a = ( double)1.0;
                            if (!Object.Equals(outV, null))
                                outV.a = ( double)1.0;
                            return abs(A);
                        //case 2:
                        //    return -1; 
                        //case 3: 
                        //    return -1; 
                    }
                }
                if (!discardFiniteTest && !allall(isfinite(A)))
                    throw new ILArgumentException("svd: input must have only finite elements");
                if (Lapack == null)
                    throw new ILMathException("no Lapack package available");
                // parameter evaluation
                int M = A.Size[0]; int N = A.Size[1];
                int minMN = (M < N) ? M : N;
                int LDU = M; int LDVT = N;
                int LDA = M, lenDU = 0, lenVT = 0;
                
                double[] dS = ILMemoryPool.Pool.New<  double>(minMN);
                char jobz = (small) ? 'S' : 'A';
                
                double[] dU = null;
                
                double[] dVT = null;
                int info = 0;
                if (!Object.Equals(outU, null) || !Object.Equals(outV, null)) {
                    // need to return U and VT 
                    if (small) {
                        lenDU = M * minMN; 
                        dU = ILMemoryPool.Pool.New<  double>(lenDU);
                        lenVT = N * minMN; 
                        dVT = ILMemoryPool.Pool.New<  double>(lenVT);
                    } else {
                        lenDU = M * M; 
                        dU = ILMemoryPool.Pool.New<  double>(lenDU);
                        lenVT = N * N; 
                        dVT = ILMemoryPool.Pool.New<  double>(lenVT);
                    }
                } else {
                    jobz = 'N';
                }

                // must create copy of input ! 
                ILArray<double> Acopy = A.C; 
                
                double[] dInput = Acopy.GetArrayForWrite();
                /*!HC:lapack_dgesdd*/
                Lapack.dgesdd(jobz, M, N, dInput, LDA, dS, dU, LDU, dVT, LDVT, ref info);
                if (info < 0)
                    throw new ILArgumentException("the " + (-info).ToString() + "th argument to SVD was invalid");
                if (info > 0)
                    throw new ILArgumentException("svd was not converging");
                ILArray< double> ret = empty<double>(ILSize.Empty00);
                if (info == 0) {
                    // success
                    if (!Object.Equals(outU, null) || !Object.Equals(outV, null)) {
                        if (small) {
                            ret.a = zeros< double>(new ILSize(minMN, minMN));
                        } else {
                            ret.a = zeros< double>(new ILSize(M, N));
                        }
                        for (int i = 0; i < minMN; i++) {
                            ret.SetValue(dS[i], i, i);
                        }
                        ILMemoryPool.Pool.Free(dS); 
                        if (!Object.Equals(outU, null)) {
                            outU.a = array< double>(dU, M, lenDU / M);
                        } else {
                            ILMemoryPool.Pool.Free(dU); 
                        }
                        if (!Object.Equals(outV, null)) {
                            
                            outV.a = array< double>(dVT, N, lenVT / N).T;
                        } else {
                            ILMemoryPool.Pool.Free(dVT); 
                        }
                    } else {
                        ret.a = array< double>(dS,minMN, 1);
                    }
                }
                return ret;
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="outU">[Output] Left singular vectors of A as columns of matrix outU. 
        /// Setting this parameter to a non-null value signals the need of returning those values.</param>
        /// <param name="outV">[Output] Right singular vectors of X as rows of matrix outV.
        /// This parameter must not be null. It might be an empty array on input.</param>
        /// <param name="small">If true: return only first min(M,N) columns of outU and S (returned) will be 
        /// of size [min(M,N),min(M,N)]</param>
        /// <param name="discardFiniteTest">If true: the matrix given will not be checked for infinte or NaN values. If such elements 
        /// exist nevertheless, this may result in failing convergence or error. In worst case 
        /// the function may hang inside the Lapack lib! Use with care! </param>
        /// <returns>Singular values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< float> svd( ILInArray< float> A, ILOutArray< float> outU, 
                                                                ILOutArray< float> outV, bool small, bool discardFiniteTest ) {
            if (object.Equals(A,null))
                throw new ILArgumentException("input matrix X must not be null"); 
            using (ILScope.Enter(A)) {
                
                if (!A.IsMatrix)
                    throw new ILArgumentSizeException("svd is defined for matrices only");
                // early exit for small matrices
                if (A.Size[1] < 4 && A.Size[0] == A.Size[1]) {
                    switch (A.Size[0]) {
                        case 1:
                            if (!Object.Equals(outU, null))
                                outU.a = ( float)1.0;
                            if (!Object.Equals(outV, null))
                                outV.a = ( float)1.0;
                            return abs(A);
                        //case 2:
                        //    return -1; 
                        //case 3: 
                        //    return -1; 
                    }
                }
                if (!discardFiniteTest && !allall(isfinite(A)))
                    throw new ILArgumentException("svd: input must have only finite elements");
                if (Lapack == null)
                    throw new ILMathException("no Lapack package available");
                // parameter evaluation
                int M = A.Size[0]; int N = A.Size[1];
                int minMN = (M < N) ? M : N;
                int LDU = M; int LDVT = N;
                int LDA = M, lenDU = 0, lenVT = 0;
               
                float[] dS = ILMemoryPool.Pool.New<  float>(minMN);
                char jobz = (small) ? 'S' : 'A';
               
                float[] dU = null;
               
                float[] dVT = null;
                int info = 0;
                if (!Object.Equals(outU, null) || !Object.Equals(outV, null)) {
                    // need to return U and VT 
                    if (small) {
                        lenDU = M * minMN; 
                        dU = ILMemoryPool.Pool.New<  float>(lenDU);
                        lenVT = N * minMN; 
                        dVT = ILMemoryPool.Pool.New<  float>(lenVT);
                    } else {
                        lenDU = M * M; 
                        dU = ILMemoryPool.Pool.New<  float>(lenDU);
                        lenVT = N * N; 
                        dVT = ILMemoryPool.Pool.New<  float>(lenVT);
                    }
                } else {
                    jobz = 'N';
                }

                // must create copy of input ! 
                ILArray<float> Acopy = A.C; 
               
                float[] dInput = Acopy.GetArrayForWrite();
                Lapack.sgesdd(jobz, M, N, dInput, LDA, dS, dU, LDU, dVT, LDVT, ref info);
                if (info < 0)
                    throw new ILArgumentException("the " + (-info).ToString() + "th argument to SVD was invalid");
                if (info > 0)
                    throw new ILArgumentException("svd was not converging");
                ILArray< float> ret = empty<float>(ILSize.Empty00);
                if (info == 0) {
                    // success
                    if (!Object.Equals(outU, null) || !Object.Equals(outV, null)) {
                        if (small) {
                            ret.a = zeros< float>(new ILSize(minMN, minMN));
                        } else {
                            ret.a = zeros< float>(new ILSize(M, N));
                        }
                        for (int i = 0; i < minMN; i++) {
                            ret.SetValue(dS[i], i, i);
                        }
                        ILMemoryPool.Pool.Free(dS); 
                        if (!Object.Equals(outU, null)) {
                            outU.a = array< float>(dU, M, lenDU / M);
                        } else {
                            ILMemoryPool.Pool.Free(dU); 
                        }
                        if (!Object.Equals(outV, null)) {
                            outV.a = new  ILRetArray<float> (dVT,N,lenVT / N).T;
                        } else {
                            ILMemoryPool.Pool.Free(dVT); 
                        }
                    } else {
                        ret.a = array< float>(dS,minMN, 1);
                    }
                }
                return ret;
            }
        }
        /// <summary>
        /// singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="outU">[Output] Left singular vectors of A as columns of matrix outU. 
        /// Setting this parameter to a non-null value signals the need of returning those values.</param>
        /// <param name="outV">[Output] Right singular vectors of X as rows of matrix outV.
        /// This parameter must not be null. It might be an empty array on input.</param>
        /// <param name="small">If true: return only first min(M,N) columns of outU and S (returned) will be 
        /// of size [min(M,N),min(M,N)]</param>
        /// <param name="discardFiniteTest">If true: the matrix given will not be checked for infinte or NaN values. If such elements 
        /// exist nevertheless, this may result in failing convergence or error. In worst case 
        /// the function may hang inside the Lapack lib! Use with care! </param>
        /// <returns>Singular values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< float> svd( ILInArray< fcomplex> A, ILOutArray< fcomplex> outU, 
                                                                ILOutArray< fcomplex> outV, bool small, bool discardFiniteTest ) {
            if (object.Equals(A,null))
                throw new ILArgumentException("input matrix X must not be null"); 
            using (ILScope.Enter(A)) {
                
                if (!A.IsMatrix)
                    throw new ILArgumentSizeException("svd is defined for matrices only");
                // early exit for small matrices
                if (A.Size[1] < 4 && A.Size[0] == A.Size[1]) {
                    switch (A.Size[0]) {
                        case 1:
                            if (!Object.Equals(outU, null))
                                outU.a = ( fcomplex)1.0;
                            if (!Object.Equals(outV, null))
                                outV.a = ( fcomplex)1.0;
                            return abs(A);
                        //case 2:
                        //    return -1; 
                        //case 3: 
                        //    return -1; 
                    }
                }
                if (!discardFiniteTest && !allall(isfinite(A)))
                    throw new ILArgumentException("svd: input must have only finite elements");
                if (Lapack == null)
                    throw new ILMathException("no Lapack package available");
                // parameter evaluation
                int M = A.Size[0]; int N = A.Size[1];
                int minMN = (M < N) ? M : N;
                int LDU = M; int LDVT = N;
                int LDA = M, lenDU = 0, lenVT = 0;
               
                float[] dS = ILMemoryPool.Pool.New<  float>(minMN);
                char jobz = (small) ? 'S' : 'A';
               
                fcomplex[] dU = null;
               
                fcomplex[] dVT = null;
                int info = 0;
                if (!Object.Equals(outU, null) || !Object.Equals(outV, null)) {
                    // need to return U and VT 
                    if (small) {
                        lenDU = M * minMN; 
                        dU = ILMemoryPool.Pool.New<  fcomplex>(lenDU);
                        lenVT = N * minMN; 
                        dVT = ILMemoryPool.Pool.New<  fcomplex>(lenVT);
                    } else {
                        lenDU = M * M; 
                        dU = ILMemoryPool.Pool.New<  fcomplex>(lenDU);
                        lenVT = N * N; 
                        dVT = ILMemoryPool.Pool.New<  fcomplex>(lenVT);
                    }
                } else {
                    jobz = 'N';
                }

                // must create copy of input ! 
                ILArray<fcomplex> Acopy = A.C; 
               
                fcomplex[] dInput = Acopy.GetArrayForWrite();
                Lapack.cgesdd(jobz, M, N, dInput, LDA, dS, dU, LDU, dVT, LDVT, ref info);
                if (info < 0)
                    throw new ILArgumentException("the " + (-info).ToString() + "th argument to SVD was invalid");
                if (info > 0)
                    throw new ILArgumentException("svd was not converging");
                ILArray< float> ret = empty<float>(ILSize.Empty00);
                if (info == 0) {
                    // success
                    if (!Object.Equals(outU, null) || !Object.Equals(outV, null)) {
                        if (small) {
                            ret.a = zeros< float>(new ILSize(minMN, minMN));
                        } else {
                            ret.a = zeros< float>(new ILSize(M, N));
                        }
                        for (int i = 0; i < minMN; i++) {
                            ret.SetValue(dS[i], i, i);
                        }
                        ILMemoryPool.Pool.Free(dS); 
                        if (!Object.Equals(outU, null)) {
                            outU.a = array< fcomplex>(dU, M, lenDU / M);
                        } else {
                            ILMemoryPool.Pool.Free(dU); 
                        }
                        if (!Object.Equals(outV, null)) {
                            outV.a = conj(new  ILRetArray<fcomplex> (dVT,N,lenVT / N).T);
                        } else {
                            ILMemoryPool.Pool.Free(dVT); 
                        }
                    } else {
                        ret.a = array< float>(dS,minMN, 1);
                    }
                }
                return ret;
            }
        }
        /// <summary>
        /// singular value decomposition 
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="outU">[Output] Left singular vectors of A as columns of matrix outU. 
        /// Setting this parameter to a non-null value signals the need of returning those values.</param>
        /// <param name="outV">[Output] Right singular vectors of X as rows of matrix outV.
        /// This parameter must not be null. It might be an empty array on input.</param>
        /// <param name="small">If true: return only first min(M,N) columns of outU and S (returned) will be 
        /// of size [min(M,N),min(M,N)]</param>
        /// <param name="discardFiniteTest">If true: the matrix given will not be checked for infinte or NaN values. If such elements 
        /// exist nevertheless, this may result in failing convergence or error. In worst case 
        /// the function may hang inside the Lapack lib! Use with care! </param>
        /// <returns>Singular values as diagonal matrix of same size and type as A</returns>
        public static ILRetArray< double> svd( ILInArray< complex> A, ILOutArray< complex> outU, 
                                                                ILOutArray< complex> outV, bool small, bool discardFiniteTest ) {
            if (object.Equals(A,null))
                throw new ILArgumentException("input matrix X must not be null"); 
            using (ILScope.Enter(A)) {
                
                if (!A.IsMatrix)
                    throw new ILArgumentSizeException("svd is defined for matrices only");
                // early exit for small matrices
                if (A.Size[1] < 4 && A.Size[0] == A.Size[1]) {
                    switch (A.Size[0]) {
                        case 1:
                            if (!Object.Equals(outU, null))
                                outU.a = ( complex)1.0;
                            if (!Object.Equals(outV, null))
                                outV.a = ( complex)1.0;
                            return abs(A);
                        //case 2:
                        //    return -1; 
                        //case 3: 
                        //    return -1; 
                    }
                }
                if (!discardFiniteTest && !allall(isfinite(A)))
                    throw new ILArgumentException("svd: input must have only finite elements");
                if (Lapack == null)
                    throw new ILMathException("no Lapack package available");
                // parameter evaluation
                int M = A.Size[0]; int N = A.Size[1];
                int minMN = (M < N) ? M : N;
                int LDU = M; int LDVT = N;
                int LDA = M, lenDU = 0, lenVT = 0;
               
                double[] dS = ILMemoryPool.Pool.New<  double>(minMN);
                char jobz = (small) ? 'S' : 'A';
               
                complex[] dU = null;
               
                complex[] dVT = null;
                int info = 0;
                if (!Object.Equals(outU, null) || !Object.Equals(outV, null)) {
                    // need to return U and VT 
                    if (small) {
                        lenDU = M * minMN; 
                        dU = ILMemoryPool.Pool.New<  complex>(lenDU);
                        lenVT = N * minMN; 
                        dVT = ILMemoryPool.Pool.New<  complex>(lenVT);
                    } else {
                        lenDU = M * M; 
                        dU = ILMemoryPool.Pool.New<  complex>(lenDU);
                        lenVT = N * N; 
                        dVT = ILMemoryPool.Pool.New<  complex>(lenVT);
                    }
                } else {
                    jobz = 'N';
                }

                // must create copy of input ! 
                ILArray<complex> Acopy = A.C; 
               
                complex[] dInput = Acopy.GetArrayForWrite();
                Lapack.zgesdd(jobz, M, N, dInput, LDA, dS, dU, LDU, dVT, LDVT, ref info);
                if (info < 0)
                    throw new ILArgumentException("the " + (-info).ToString() + "th argument to SVD was invalid");
                if (info > 0)
                    throw new ILArgumentException("svd was not converging");
                ILArray< double> ret = empty<double>(ILSize.Empty00);
                if (info == 0) {
                    // success
                    if (!Object.Equals(outU, null) || !Object.Equals(outV, null)) {
                        if (small) {
                            ret.a = zeros< double>(new ILSize(minMN, minMN));
                        } else {
                            ret.a = zeros< double>(new ILSize(M, N));
                        }
                        for (int i = 0; i < minMN; i++) {
                            ret.SetValue(dS[i], i, i);
                        }
                        ILMemoryPool.Pool.Free(dS); 
                        if (!Object.Equals(outU, null)) {
                            outU.a = array< complex>(dU, M, lenDU / M);
                        } else {
                            ILMemoryPool.Pool.Free(dU); 
                        }
                        if (!Object.Equals(outV, null)) {
                            outV.a = conj(new  ILRetArray<complex> (dVT,N,lenVT / N).T);
                        } else {
                            ILMemoryPool.Pool.Free(dVT); 
                        }
                    } else {
                        ret.a = array< double>(dS,minMN, 1);
                    }
                }
                return ret;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}
