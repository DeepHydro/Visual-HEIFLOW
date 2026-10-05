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
using ILNumerics;

namespace ILNumerics  {

    public partial class ILMath {




        /// <summary>
        /// Pseudo - inverse of input argument M
        /// </summary>
        /// <param name="M">Input matrix M</param>
        /// <returns>Pseudo inverse of input matrix M</returns>
        /// <remarks>The function returns the pseudo inverse (Moore-Penrose pseudoinverse)
        /// of input matrix M. The return value will be of the same size as 
        /// the transposed of M. it will satisfy the following conditions: 
        /// <list type="bullet">
        /// <item>M * pinv(M) * M  = M </item>
        /// <item>pinv(M) * M * pinv(M) = pinv(M)</item>
        /// <item>pinv(M) * M is hermitian</item>
        /// <item>M * pinv(M) is hermitian</item>
        /// </list>
        /// pinv uses Lapack's function svd internally. Any singular values less than 
        /// the default tolerance will be set to zero. As tolerance the following equation is used: \\
        /// tol = length(M) * norm(M) * Double.epsilon \\
        /// with 
        /// <list type="bullet">
        /// <item>length(M) - the longest dimension of M</item>
        /// <item>norm(M) being the largest singular value of M, </item>
        /// <item>Double.epsilon - the smallest number greater than zero</item>
        /// </list>
        /// You may use a overloaded function to define an alternative tolerance. 
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.pinv(ILInArray{double}, double)"/>
        public static ILRetArray< double > pinv(ILInArray< double > M) {
            return pinv(M, -1); 
        }
        /// <summary>
        /// Pseudo inverse of input matrix M
        /// </summary>
        /// <param name="M">Input matrix M</param>
        /// <param name="tolerance">Tolerance, see remarks (default = -1; use default tolerance)</param>
        /// <returns>Pseudo inverse of M</returns>
        /// <remarks>The function returns the pseudo inverse (Moore-Penrose pseudoinverse)
        /// of input matrix M. The return value will be of the same size as 
        /// the transposed of M. it will satisfy the following conditions: 
        /// <list type="bullet">
        /// <item>M * pinv(M) * M  = M </item>
        /// <item>pinv(M) * M * pinv(M) = pinv(M)</item>
        /// <item>pinv(M) * M is hermitian</item>
        /// <item>M * pinv(M) is hermitian</item>
        /// </list>
        /// pinv uses LAPACK's function svd internally. Any singular values less than 
        /// tolerance will be set to zero. If tolerance is less than zero, the following equation 
        /// is used as default: \\
        /// tol = length(M) * norm(M) * Double.epsilon \\
        /// with 
        /// <list type="bullet">
        /// <item>length(M) - the longest dimension of M</item>
        /// <item>norm(M) being the largest singular value of M, </item>
        /// <item>Double.epsilon - the smallest constructable double precision number greater than zero</item>
        /// </list>
        /// </remarks>
        public static ILRetArray< double > pinv(ILInArray< double > M,  double tolerance) {
            using (ILScope.Enter(M)) {
                // let svd check the dimensions! 
                //if (M.Dimensions.NumberOfDimensions > 2)
                //   throw new ILDimensionMismatchException("pinv: ...");

                // in order to use the cheap packed version of svd, the matrix must be m x n with m > n! 
                if (M.Size[0] < M.Size[1]) {
                    
                    return pinv(M.T, tolerance).T;
                }
                if (M.IsScalar)
                    return  1.0 / M;

                ILArray< double> U = empty< double>(ILSize.Empty00);
                ILArray< double> V = empty< double>(ILSize.Empty00);
                ILArray< double> S = svd(M, U, V, true, false);

                int m = M.Size[0];
                int n = M.Size[1];

                ILArray< double> s;
                switch (m) {
                    case 0:
                        s = zeros< double>(ILSize.Scalar1_1);
                        break;
                    case 1:
                        s = S[0];
                        break;
                    default:
                        s = diag< double>(S);
                        break;
                }
                if (tolerance < 0) {
                    tolerance = ( double)(M.Size.Longest * max(s).GetValue(0) *  MachineParameterDouble.eps);
                }
                // sum vector elements: s is dense vector returned from svd
                int count = (int)sum(s > ( double)tolerance);

                ILArray< double> Ret = empty< double>(ILSize.Empty00);
                if (count == 0)
                    S.a = zeros< double>(new ILSize(n, m));
                else {
                    ILArray< double> OneVec = array< double>( 1.0, count, 1);

                    S.a = diag(divide(OneVec, s[r(0,count - 1)]));
                    
                    U.a = U[full,r(0,count-1)].T;
                    
                    Ret.a = multiply(multiply(V[full,r(0,count - 1)], S), U);
                }
                return Ret;
            }
        }

#region HYCALPER AUTO GENERATED CODE



        /// <summary>
        /// Pseudo - inverse of input argument M
        /// </summary>
        /// <param name="M">Input matrix M</param>
        /// <returns>Pseudo inverse of input matrix M</returns>
        /// <remarks>The function returns the pseudo inverse (Moore-Penrose pseudoinverse)
        /// of input matrix M. The return value will be of the same size as 
        /// the transposed of M. it will satisfy the following conditions: 
        /// <list type="bullet">
        /// <item>M * pinv(M) * M  = M </item>
        /// <item>pinv(M) * M * pinv(M) = pinv(M)</item>
        /// <item>pinv(M) * M is hermitian</item>
        /// <item>M * pinv(M) is hermitian</item>
        /// </list>
        /// pinv uses Lapack's function svd internally. Any singular values less than 
        /// the default tolerance will be set to zero. As tolerance the following equation is used: \\
        /// tol = length(M) * norm(M) * Double.epsilon \\
        /// with 
        /// <list type="bullet">
        /// <item>length(M) - the longest dimension of M</item>
        /// <item>norm(M) being the largest singular value of M, </item>
        /// <item>Double.epsilon - the smallest number greater than zero</item>
        /// </list>
        /// You may use a overloaded function to define an alternative tolerance. 
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.pinv(ILInArray{double}, double)"/>
        public static ILRetArray< float > pinv(ILInArray< float > M) {
            return pinv(M, -1); 
        }
        /// <summary>
        /// Pseudo inverse of input matrix M
        /// </summary>
        /// <param name="M">Input matrix M</param>
        /// <param name="tolerance">Tolerance, see remarks (default = -1; use default tolerance)</param>
        /// <returns>Pseudo inverse of M</returns>
        /// <remarks>The function returns the pseudo inverse (Moore-Penrose pseudoinverse)
        /// of input matrix M. The return value will be of the same size as 
        /// the transposed of M. it will satisfy the following conditions: 
        /// <list type="bullet">
        /// <item>M * pinv(M) * M  = M </item>
        /// <item>pinv(M) * M * pinv(M) = pinv(M)</item>
        /// <item>pinv(M) * M is hermitian</item>
        /// <item>M * pinv(M) is hermitian</item>
        /// </list>
        /// pinv uses LAPACK's function svd internally. Any singular values less than 
        /// tolerance will be set to zero. If tolerance is less than zero, the following equation 
        /// is used as default: \\
        /// tol = length(M) * norm(M) * Double.epsilon \\
        /// with 
        /// <list type="bullet">
        /// <item>length(M) - the longest dimension of M</item>
        /// <item>norm(M) being the largest singular value of M, </item>
        /// <item>Double.epsilon - the smallest constructable double precision number greater than zero</item>
        /// </list>
        /// </remarks>
        public static ILRetArray< float > pinv(ILInArray< float > M,  float tolerance) {
            using (ILScope.Enter(M)) {
                // let svd check the dimensions! 
                //if (M.Dimensions.NumberOfDimensions > 2)
                //   throw new ILDimensionMismatchException("pinv: ...");

                // in order to use the cheap packed version of svd, the matrix must be m x n with m > n! 
                if (M.Size[0] < M.Size[1]) {
                    return pinv(M.T, tolerance).T;
                }
                if (M.IsScalar)
                    return  1 / M;

                ILArray< float> U = empty< float>(ILSize.Empty00);
                ILArray< float> V = empty< float>(ILSize.Empty00);
                ILArray< float> S = svd(M, U, V, true, false);

                int m = M.Size[0];
                int n = M.Size[1];

                ILArray< float> s;
                switch (m) {
                    case 0:
                        s = zeros< float>(ILSize.Scalar1_1);
                        break;
                    case 1:
                        s = S[0];
                        break;
                    default:
                        s = diag< float>(S);
                        break;
                }
                if (tolerance < 0) {
                    tolerance = ( float)(M.Size.Longest * max(s).GetValue(0) *  ILMath.MachineParameterSingle.eps);
                }
                // sum vector elements: s is dense vector returned from svd
                int count = (int)sum(s > ( float)tolerance);

                ILArray< float> Ret = empty< float>(ILSize.Empty00);
                if (count == 0)
                    S.a = zeros< float>(new ILSize(n, m));
                else {
                    ILArray< float> OneVec = array< float>( 1.0f, count, 1);

                    S.a = diag(divide(OneVec, s[r(0,count - 1)]));
                    U = U[":;0:" + (count - 1)].T;
                    Ret = multiply(multiply(V[":;0:" + (count - 1)], S), U);
                }
                return Ret;
            }
        }


        /// <summary>
        /// Pseudo - inverse of input argument M
        /// </summary>
        /// <param name="M">Input matrix M</param>
        /// <returns>Pseudo inverse of input matrix M</returns>
        /// <remarks>The function returns the pseudo inverse (Moore-Penrose pseudoinverse)
        /// of input matrix M. The return value will be of the same size as 
        /// the transposed of M. it will satisfy the following conditions: 
        /// <list type="bullet">
        /// <item>M * pinv(M) * M  = M </item>
        /// <item>pinv(M) * M * pinv(M) = pinv(M)</item>
        /// <item>pinv(M) * M is hermitian</item>
        /// <item>M * pinv(M) is hermitian</item>
        /// </list>
        /// pinv uses Lapack's function svd internally. Any singular values less than 
        /// the default tolerance will be set to zero. As tolerance the following equation is used: \\
        /// tol = length(M) * norm(M) * Double.epsilon \\
        /// with 
        /// <list type="bullet">
        /// <item>length(M) - the longest dimension of M</item>
        /// <item>norm(M) being the largest singular value of M, </item>
        /// <item>Double.epsilon - the smallest number greater than zero</item>
        /// </list>
        /// You may use a overloaded function to define an alternative tolerance. 
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.pinv(ILInArray{double}, double)"/>
        public static ILRetArray< fcomplex > pinv(ILInArray< fcomplex > M) {
            return pinv(M, -1); 
        }
        /// <summary>
        /// Pseudo inverse of input matrix M
        /// </summary>
        /// <param name="M">Input matrix M</param>
        /// <param name="tolerance">Tolerance, see remarks (default = -1; use default tolerance)</param>
        /// <returns>Pseudo inverse of M</returns>
        /// <remarks>The function returns the pseudo inverse (Moore-Penrose pseudoinverse)
        /// of input matrix M. The return value will be of the same size as 
        /// the transposed of M. it will satisfy the following conditions: 
        /// <list type="bullet">
        /// <item>M * pinv(M) * M  = M </item>
        /// <item>pinv(M) * M * pinv(M) = pinv(M)</item>
        /// <item>pinv(M) * M is hermitian</item>
        /// <item>M * pinv(M) is hermitian</item>
        /// </list>
        /// pinv uses LAPACK's function svd internally. Any singular values less than 
        /// tolerance will be set to zero. If tolerance is less than zero, the following equation 
        /// is used as default: \\
        /// tol = length(M) * norm(M) * Double.epsilon \\
        /// with 
        /// <list type="bullet">
        /// <item>length(M) - the longest dimension of M</item>
        /// <item>norm(M) being the largest singular value of M, </item>
        /// <item>Double.epsilon - the smallest constructable double precision number greater than zero</item>
        /// </list>
        /// </remarks>
        public static ILRetArray< fcomplex > pinv(ILInArray< fcomplex > M,  fcomplex tolerance) {
            using (ILScope.Enter(M)) {
                // let svd check the dimensions! 
                //if (M.Dimensions.NumberOfDimensions > 2)
                //   throw new ILDimensionMismatchException("pinv: ...");

                // in order to use the cheap packed version of svd, the matrix must be m x n with m > n! 
                if (M.Size[0] < M.Size[1]) {
                    return conj(pinv(conj(M.T), tolerance)).T;
                }
                if (M.IsScalar)
                    return  new fcomplex(1f,0f) / M;

                ILArray< fcomplex> U = empty< fcomplex>(ILSize.Empty00);
                ILArray< fcomplex> V = empty< fcomplex>(ILSize.Empty00);
                ILArray< float> S = svd(M, U, V, true, false);

                int m = M.Size[0];
                int n = M.Size[1];

                ILArray< float> s;
                switch (m) {
                    case 0:
                        s = zeros< float>(ILSize.Scalar1_1);
                        break;
                    case 1:
                        s = S[0];
                        break;
                    default:
                        s = diag< float>(S);
                        break;
                }
                if (tolerance < 0) {
                    tolerance = ( fcomplex)(M.Size.Longest * max(s).GetValue(0) *  ILMath.MachineParameterSingle.eps);
                }
                // sum vector elements: s is dense vector returned from svd
                int count = (int)sum(s > ( float)tolerance);

                ILArray< fcomplex> Ret = empty< fcomplex>(ILSize.Empty00);
                if (count == 0)
                    S.a = zeros< float>(new ILSize(n, m));
                else {
                    ILArray< float> OneVec = array< float>( 1.0f, count, 1);

                    S.a = diag(divide(OneVec, s[r(0,count - 1)]));
                    U = conj(U[":;0:" + (count - 1)].T);
                    Ret = multiply(multiply(V[":;0:" + (count - 1)], real2fcomplex(S)), U);
                }
                return Ret;
            }
        }


        /// <summary>
        /// Pseudo - inverse of input argument M
        /// </summary>
        /// <param name="M">Input matrix M</param>
        /// <returns>Pseudo inverse of input matrix M</returns>
        /// <remarks>The function returns the pseudo inverse (Moore-Penrose pseudoinverse)
        /// of input matrix M. The return value will be of the same size as 
        /// the transposed of M. it will satisfy the following conditions: 
        /// <list type="bullet">
        /// <item>M * pinv(M) * M  = M </item>
        /// <item>pinv(M) * M * pinv(M) = pinv(M)</item>
        /// <item>pinv(M) * M is hermitian</item>
        /// <item>M * pinv(M) is hermitian</item>
        /// </list>
        /// pinv uses Lapack's function svd internally. Any singular values less than 
        /// the default tolerance will be set to zero. As tolerance the following equation is used: \\
        /// tol = length(M) * norm(M) * Double.epsilon \\
        /// with 
        /// <list type="bullet">
        /// <item>length(M) - the longest dimension of M</item>
        /// <item>norm(M) being the largest singular value of M, </item>
        /// <item>Double.epsilon - the smallest number greater than zero</item>
        /// </list>
        /// You may use a overloaded function to define an alternative tolerance. 
        /// </remarks>
        /// <seealso cref="ILNumerics.ILMath.pinv(ILInArray{double}, double)"/>
        public static ILRetArray< complex > pinv(ILInArray< complex > M) {
            return pinv(M, -1); 
        }
        /// <summary>
        /// Pseudo inverse of input matrix M
        /// </summary>
        /// <param name="M">Input matrix M</param>
        /// <param name="tolerance">Tolerance, see remarks (default = -1; use default tolerance)</param>
        /// <returns>Pseudo inverse of M</returns>
        /// <remarks>The function returns the pseudo inverse (Moore-Penrose pseudoinverse)
        /// of input matrix M. The return value will be of the same size as 
        /// the transposed of M. it will satisfy the following conditions: 
        /// <list type="bullet">
        /// <item>M * pinv(M) * M  = M </item>
        /// <item>pinv(M) * M * pinv(M) = pinv(M)</item>
        /// <item>pinv(M) * M is hermitian</item>
        /// <item>M * pinv(M) is hermitian</item>
        /// </list>
        /// pinv uses LAPACK's function svd internally. Any singular values less than 
        /// tolerance will be set to zero. If tolerance is less than zero, the following equation 
        /// is used as default: \\
        /// tol = length(M) * norm(M) * Double.epsilon \\
        /// with 
        /// <list type="bullet">
        /// <item>length(M) - the longest dimension of M</item>
        /// <item>norm(M) being the largest singular value of M, </item>
        /// <item>Double.epsilon - the smallest constructable double precision number greater than zero</item>
        /// </list>
        /// </remarks>
        public static ILRetArray< complex > pinv(ILInArray< complex > M,  complex tolerance) {
            using (ILScope.Enter(M)) {
                // let svd check the dimensions! 
                //if (M.Dimensions.NumberOfDimensions > 2)
                //   throw new ILDimensionMismatchException("pinv: ...");

                // in order to use the cheap packed version of svd, the matrix must be m x n with m > n! 
                if (M.Size[0] < M.Size[1]) {
                    return conj(pinv(conj(M.T), tolerance)).T;
                }
                if (M.IsScalar)
                    return  new complex(1,0) / M;

                ILArray< complex> U = empty< complex>(ILSize.Empty00);
                ILArray< complex> V = empty< complex>(ILSize.Empty00);
                ILArray< double> S = svd(M, U, V, true, false);

                int m = M.Size[0];
                int n = M.Size[1];

                ILArray< double> s;
                switch (m) {
                    case 0:
                        s = zeros< double>(ILSize.Scalar1_1);
                        break;
                    case 1:
                        s = S[0];
                        break;
                    default:
                        s = diag< double>(S);
                        break;
                }
                if (tolerance < 0) {
                    tolerance = ( complex)(M.Size.Longest * max(s).GetValue(0) *  ILMath.MachineParameterDouble.eps);
                }
                // sum vector elements: s is dense vector returned from svd
                int count = (int)sum(s > ( double)tolerance);

                ILArray< complex> Ret = empty< complex>(ILSize.Empty00);
                if (count == 0)
                    S.a = zeros< double>(new ILSize(n, m));
                else {
                    ILArray< double> OneVec = array< double>( 1.0, count, 1);

                    S.a = diag(divide(OneVec, s[r(0,count - 1)]));
                    U = conj(U[":;0:" + (count - 1)].T);
                    Ret = multiply(multiply(V[":;0:" + (count - 1)], real2complex(S)), U);
                }
                return Ret;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}
