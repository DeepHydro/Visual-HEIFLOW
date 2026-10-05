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
        /// QR decomposition - raw Lapack output
        /// </summary>
        /// <param name="A">Input matrix A</param>
        /// <returns>Orthonormal / unitary matrix Q and upper triangular 
        /// matrix R packed into single matrix. This is the output of the 
        /// lapack function ?geqrf.</returns>
        /// <remarks><para>Input matrix A will not be altered. </para>
        /// <para>The matrix returned is the direct output of the lapack 
        /// function [d,s,c,z]geqrf respectively. This means that it contains 
        /// the decomposition factors Q and R, but they are combined into a 
        /// single matrix for performance reasons. If you need one of the factors, 
        /// you would use the overloaded function 
        /// <see cref="ILNumerics.ILMath.qr(ILInArray{double},ILOutArray{double})"/> 
        /// instead, which returns those factors separately.</para></remarks>
        internal static ILRetArray<double> qr( ILInArray<double> A ) {
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentException("input A must be matrix");
                int m = A.Size[0], n = A.Size[1];
                ILArray<double> ret = A.C;
                 double[] tau = ILMemoryPool.Pool.New<  double>((m < n) ? m : n);
                int info = 0;
                /*!HC:lapack_*geqrf*/
                Lapack.dgeqrf(m, n, ret.GetArrayForWrite(), m, tau, ref info);
                if (info < 0)
                    throw new ILArgumentException("an unknown error occoured during decomposition");
                return ret;
            }
        }
        /// <summary>
        /// QR decomposition, returning Q and R
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix R as 
        /// result of decomposition, size [m x n]</param>
        /// <returns>Orthonormal / unitary matrix Q as result of decomposition, size [m x m]</returns>
        /// <remarks>The function returns Q and R such that the equation 
        /// <para>A = Q * R</para> holds within roundoff errors. ('*' denotes 
        /// matrix multiplication)</remarks>
        public static ILRetArray<double> qr(
                                      ILInArray<double> A,
                                      ILOutArray<double> outR ) {
            return qr(A, outR, false);
        }
        /// <summary>
        /// QR decomposition, returning Q and R, optionally economical sized
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix R as 
        /// result of decomposition, size [m x n]</param>
        /// <param name="economySize">If true, the size of Q and R will 
        /// be [m x m] and [m x n] respectively. However, if m &lt; n, 
        /// the economySize parameter has no effect. </param>
        /// <returns>Orthonormal real / unitary complex matrix Q as result 
        /// of decomposition. Size [m x m] or [m x min(m,n)], depending 
        /// on <paramref name="economySize"/> (see remarks below)</returns>
        /// <remarks>The function returns Q and R such that the equation 
        /// <para>A = Q * R</para> holds with roundoff errors. ('*' 
        /// denotes matrix multiplication.)</remarks>
        public static ILRetArray<double> qr( ILInArray<double> A
            , ILOutArray<double> outR, bool economySize ) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(outR, null)) {
                    return qr(A);
                }
                int m = A.Size[0];
                int n = A.Size[1];
                if (m < n && economySize) return qr(A, outR, false);
                ILArray<double> ret = empty<double>(ILSize.Empty00);
                if (m == 0 || n == 0) {
                    if (!object.Equals(outR,null))
                        outR.a = empty<double>(new ILSize(m, n));
                    return empty<double>(A.Size);
                }
                int minMN = (m < n) ? m : n;
                int info = 0;
                 double[] tau = ILMemoryPool.Pool.New<  double>(minMN);
                if (m >= n) {
                    ret.a = zeros<double>(m, (economySize) ? minMN : m);
                } else {
                    // economySize is always false ... !
                    // a temporary array is needed for extraction of the compact lapack Q (?geqrf)
                    ret.a = zeros<double>(m, n);
                }
                ret[full, r(0, n - 1)] = A[full, full]; 
                 double[] QArr = ret.GetArrayForWrite();
                /*!HC:lapack_*geqrf*/
                Lapack.dgeqrf(m, n, QArr, m, tau, ref info);
                if (info != 0) {
                    throw new ILArgumentException("error inside lapack library (?geqrf). info=" + info.ToString());
                }
                // extract R, Q
                if (economySize) {
                    outR.a = copyUpperTriangle(QArr, m, n, minMN);
                    /*!HC:lapack_*orgqr*/
                    Lapack.dorgqr(m, minMN, minMN, QArr, m, tau, ref info);
                } else {
                    outR.a = copyUpperTriangle(QArr, m, n, m);
                    /*!HC:lapack_*orgqr*/
                    Lapack.dorgqr(m, m, minMN, QArr, m, tau, ref info);
                    if (m < n)
                        ret.a = ret[full,r(0,m - 1)];
                }
                if (info != 0)
                    throw new ILArgumentException("error in lapack library (???gqr). info=" + info.ToString());
                return ret;
            }
        }
        /// <summary>
        /// QR decomposition with pivoting
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix 
        /// R as result of decomposition. Size [m x n] or [min(m,n) x n] 
        /// (see remarks). </param>
        /// <param name="outE">[Output] Permutation matrix from pivoting. Size [m x m]</param>
        /// <returns>Orthonormal / unitary matrix Q as result of decomposition. 
        /// Size [m x min(m,n)]</returns>
        /// <remarks>The function returns Q, R and E such that the equation 
        /// <para>A * E = Q * R</para> holds with roundoff errors, where '*' 
        /// denotes matrix multiplication. E reflects the pivoting done 
        /// inside LAPACK in order to give R increasingly diagonal elements.</remarks>
        /// <seealso cref="ILNumerics.ILMath.qr(ILInArray{double}, ILOutArray{double}, ILOutArray{double},bool)"/>
        public static  ILRetArray<double> qr(
                                    ILInArray<double> A,
                                    ILOutArray< double> outR,
                                    ILOutArray< double> outE ) {
            return qr(A, outR, outE, false);
        }
        /// <summary>
        /// QR decomposition with pivoting, possibly economical sized
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix R as 
        /// result of decomposition. Size [m x n] or [min(m,n) x n] depending 
        /// on <paramref name="economySize"/> (see remarks).</param>
        /// <param name="economySize"><para>If true, <list type="bullet">
        /// <item>the size of Q and R will be [m x m] and [m x n] respectively. 
        /// However, if m &lt; n, the economySize parameter has no effect on 
        /// those sizes.</item>
        /// <item>the output parameter E will be returned as row permutation 
        /// vector rather than as permutation matrix</item></list></para>
        /// <para>If false, this function acts exactly as its overload 
        /// <see cref="ILNumerics.ILMath.qr(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/></para>
        /// </param>
        /// <param name="outE">[Output] Permutation matrix from pivoting. Size [m x m]. 
        /// If this is not null, the permutation matrix/ vector E will be returned.
        /// <para>E is of size [n x n], if <paramref name="economySize"/> is 
        /// true, a row vector of length n otherwise</para></param>
        /// <returns>Orthonormal / unitary matrix Q as result of decomposition. 
        /// Size [m x m] or [m x min(m,n)], depending on <paramref name="economySize"/> 
        /// (see remarks below)</returns>
        /// <remarks><para> If <paramref name="economySize"/> is false, the function 
        /// returns Q, R and E such that the equation A * E = Q * R holds within 
        /// roundoff errors. </para>
        /// <para>If <paramref name="economySize"/> is true, E will be a permutation 
        /// vector and the equation A[":",E] == Q * R holds (except roundoff).</para>
        /// <para>E reflects the pivoting of A done inside LAPACK in order to give R 
        /// increasingly diagonal elements.</para></remarks>
        public static ILRetArray<double> qr(
                        ILInArray<  double> A,
                        ILOutArray<  double> outR,
                        ILOutArray<  double> outE,
                        bool economySize ) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(outR, null)) {
                    return qr(A);
                }
                int m = A.Size[0];
                int n = A.Size[1];
                if (m < n && economySize) return qr(A, outR, false);
                if (m == 0 || n == 0) {
                    if (!object.Equals(outR,null)) 
                        outR.a = zeros< double>(m, n);
                    if (!object.Equals(outE,null))
                        outE.a = zeros< double>(1, 0);
                    return empty<double>(A.Size);
                }
                // prepare IPVT
                if (object.Equals(outE, null))
                    return qr(A, outR, economySize);
                if (!economySize) {
                    outE.a = zeros< double>( n, n);
                } else {
                    outE.a = zeros< double>( 1, n);
                }
                int[] ipvt = ILMemoryPool.Pool.New<int>(n);
                int minMN = (m < n) ? m : n;
                int info = 0;
                 double[] tau = ILMemoryPool.Pool.New<  double>(minMN);
                 double[] QArr;
                ILArray<double> ret;
                if (m >= n) {
                    ret = zeros<double>(m, (economySize) ? minMN : m);
                } else {
                    // economySize is always false ... !
                    // a temporary array is needed for extraction of the compact lapack Q (?geqrf)
                    ret = zeros<double>( m, n);
                }
                ret[full,r(0,n-1)] = A[full,full]; 
                QArr = ret.GetArrayForWrite();
                /*!HC:lapack_*geqp3*/
                Lapack.dgeqp3(m, n, QArr, m, ipvt, tau, ref info);
                if (info != 0) {
                    throw new ILArgumentException("error inside lapack library (?geqrf). info=" + info.ToString());
                }
                // extract R, Q
                 double[] eArr = outE.GetArrayForWrite();
                if (economySize) {
                    outR.a = copyUpperTriangle(QArr, m, n, minMN);
                    /*!HC:lapack_*orgqr*/
                    Lapack.dorgqr(m, minMN, minMN, QArr, m, tau, ref info);
                    // transform E into out typed vector
                    for (int i = 0; i < n; i++) {
                        eArr[i] = ipvt[i] - 1;
                    }
                } else {
                    outR.a = copyUpperTriangle(QArr, m, n, m);
                    /*!HC:lapack_*orgqr*/
                    Lapack.dorgqr(m, m, minMN, QArr, m, tau, ref info);
                    if (m < n)
                        ret.a = ret[full,r(0,m - 1)];
                    // transform E into matrix
                    for (int i = 0; i < n; i++) {
                        eArr[(ipvt[i] - 1) + n * i] =  1.0;
                    }
                }
                if (info != 0)
                    throw new ILArgumentException("error in lapack library (???gqr). info=" + info.ToString());
                return ret;
            }
        }


#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// QR decomposition - raw Lapack output
        /// </summary>
        /// <param name="A">Input matrix A</param>
        /// <returns>Orthonormal / unitary matrix Q and upper triangular 
        /// matrix R packed into single matrix. This is the output of the 
        /// lapack function ?geqrf.</returns>
        /// <remarks><para>Input matrix A will not be altered. </para>
        /// <para>The matrix returned is the direct output of the lapack 
        /// function [d,s,c,z]geqrf respectively. This means that it contains 
        /// the decomposition factors Q and R, but they are combined into a 
        /// single matrix for performance reasons. If you need one of the factors, 
        /// you would use the overloaded function 
        /// <see cref="ILNumerics.ILMath.qr(ILInArray{double},ILOutArray{double})"/> 
        /// instead, which returns those factors separately.</para></remarks>
        internal static ILRetArray<float> qr( ILInArray<float> A ) {
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentException("input A must be matrix");
                int m = A.Size[0], n = A.Size[1];
                ILArray<float> ret = A.C;
                float[] tau = ILMemoryPool.Pool.New<  float>((m < n) ? m : n);
                int info = 0;
               
                Lapack.sgeqrf(m, n, ret.GetArrayForWrite(), m, tau, ref info);
                if (info < 0)
                    throw new ILArgumentException("an unknown error occoured during decomposition");
                return ret;
            }
        }
        /// <summary>
        /// QR decomposition, returning Q and R
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix R as 
        /// result of decomposition, size [m x n]</param>
        /// <returns>Orthonormal / unitary matrix Q as result of decomposition, size [m x m]</returns>
        /// <remarks>The function returns Q and R such that the equation 
        /// <para>A = Q * R</para> holds within roundoff errors. ('*' denotes 
        /// matrix multiplication)</remarks>
        public static ILRetArray<float> qr(
                                      ILInArray<float> A,
                                      ILOutArray<float> outR ) {
            return qr(A, outR, false);
        }
        /// <summary>
        /// QR decomposition, returning Q and R, optionally economical sized
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix R as 
        /// result of decomposition, size [m x n]</param>
        /// <param name="economySize">If true, the size of Q and R will 
        /// be [m x m] and [m x n] respectively. However, if m &lt; n, 
        /// the economySize parameter has no effect. </param>
        /// <returns>Orthonormal real / unitary complex matrix Q as result 
        /// of decomposition. Size [m x m] or [m x min(m,n)], depending 
        /// on <paramref name="economySize"/> (see remarks below)</returns>
        /// <remarks>The function returns Q and R such that the equation 
        /// <para>A = Q * R</para> holds with roundoff errors. ('*' 
        /// denotes matrix multiplication.)</remarks>
        public static ILRetArray<float> qr( ILInArray<float> A
            , ILOutArray<float> outR, bool economySize ) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(outR, null)) {
                    return qr(A);
                }
                int m = A.Size[0];
                int n = A.Size[1];
                if (m < n && economySize) return qr(A, outR, false);
                ILArray<float> ret = empty<float>(ILSize.Empty00);
                if (m == 0 || n == 0) {
                    if (!object.Equals(outR,null))
                        outR.a = empty<float>(new ILSize(m, n));
                    return empty<float>(A.Size);
                }
                int minMN = (m < n) ? m : n;
                int info = 0;
                float[] tau = ILMemoryPool.Pool.New<  float>(minMN);
                if (m >= n) {
                    ret.a = zeros<float>(m, (economySize) ? minMN : m);
                } else {
                    // economySize is always false ... !
                    // a temporary array is needed for extraction of the compact lapack Q (?geqrf)
                    ret.a = zeros<float>(m, n);
                }
                ret[full, r(0, n - 1)] = A[full, full]; 
                float[] QArr = ret.GetArrayForWrite();
               
                Lapack.sgeqrf(m, n, QArr, m, tau, ref info);
                if (info != 0) {
                    throw new ILArgumentException("error inside lapack library (?geqrf). info=" + info.ToString());
                }
                // extract R, Q
                if (economySize) {
                    outR.a = copyUpperTriangle(QArr, m, n, minMN);
                   
                    Lapack.sorgqr(m, minMN, minMN, QArr, m, tau, ref info);
                } else {
                    outR.a = copyUpperTriangle(QArr, m, n, m);
                   
                    Lapack.sorgqr(m, m, minMN, QArr, m, tau, ref info);
                    if (m < n)
                        ret.a = ret[full,r(0,m - 1)];
                }
                if (info != 0)
                    throw new ILArgumentException("error in lapack library (???gqr). info=" + info.ToString());
                return ret;
            }
        }
        /// <summary>
        /// QR decomposition with pivoting
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix 
        /// R as result of decomposition. Size [m x n] or [min(m,n) x n] 
        /// (see remarks). </param>
        /// <param name="outE">[Output] Permutation matrix from pivoting. Size [m x m]</param>
        /// <returns>Orthonormal / unitary matrix Q as result of decomposition. 
        /// Size [m x min(m,n)]</returns>
        /// <remarks>The function returns Q, R and E such that the equation 
        /// <para>A * E = Q * R</para> holds with roundoff errors, where '*' 
        /// denotes matrix multiplication. E reflects the pivoting done 
        /// inside LAPACK in order to give R increasingly diagonal elements.</remarks>
        /// <seealso cref="ILNumerics.ILMath.qr(ILInArray{double}, ILOutArray{double}, ILOutArray{double},bool)"/>
        public static  ILRetArray<float> qr(
                                    ILInArray<float> A,
                                    ILOutArray< float> outR,
                                    ILOutArray< float> outE ) {
            return qr(A, outR, outE, false);
        }
        /// <summary>
        /// QR decomposition with pivoting, possibly economical sized
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix R as 
        /// result of decomposition. Size [m x n] or [min(m,n) x n] depending 
        /// on <paramref name="economySize"/> (see remarks).</param>
        /// <param name="economySize"><para>If true, <list type="bullet">
        /// <item>the size of Q and R will be [m x m] and [m x n] respectively. 
        /// However, if m &lt; n, the economySize parameter has no effect on 
        /// those sizes.</item>
        /// <item>the output parameter E will be returned as row permutation 
        /// vector rather than as permutation matrix</item></list></para>
        /// <para>If false, this function acts exactly as its overload 
        /// <see cref="ILNumerics.ILMath.qr(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/></para>
        /// </param>
        /// <param name="outE">[Output] Permutation matrix from pivoting. Size [m x m]. 
        /// If this is not null, the permutation matrix/ vector E will be returned.
        /// <para>E is of size [n x n], if <paramref name="economySize"/> is 
        /// true, a row vector of length n otherwise</para></param>
        /// <returns>Orthonormal / unitary matrix Q as result of decomposition. 
        /// Size [m x m] or [m x min(m,n)], depending on <paramref name="economySize"/> 
        /// (see remarks below)</returns>
        /// <remarks><para> If <paramref name="economySize"/> is false, the function 
        /// returns Q, R and E such that the equation A * E = Q * R holds within 
        /// roundoff errors. </para>
        /// <para>If <paramref name="economySize"/> is true, E will be a permutation 
        /// vector and the equation A[":",E] == Q * R holds (except roundoff).</para>
        /// <para>E reflects the pivoting of A done inside LAPACK in order to give R 
        /// increasingly diagonal elements.</para></remarks>
        public static ILRetArray<float> qr(
                        ILInArray<  float> A,
                        ILOutArray<  float> outR,
                        ILOutArray<  float> outE,
                        bool economySize ) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(outR, null)) {
                    return qr(A);
                }
                int m = A.Size[0];
                int n = A.Size[1];
                if (m < n && economySize) return qr(A, outR, false);
                if (m == 0 || n == 0) {
                    if (!object.Equals(outR,null)) 
                        outR.a = zeros< float>(m, n);
                    if (!object.Equals(outE,null))
                        outE.a = zeros< float>(1, 0);
                    return empty<float>(A.Size);
                }
                // prepare IPVT
                if (object.Equals(outE, null))
                    return qr(A, outR, economySize);
                if (!economySize) {
                    outE.a = zeros< float>( n, n);
                } else {
                    outE.a = zeros< float>( 1, n);
                }
                int[] ipvt = ILMemoryPool.Pool.New<int>(n);
                int minMN = (m < n) ? m : n;
                int info = 0;
                float[] tau = ILMemoryPool.Pool.New<  float>(minMN);
                float[] QArr;
                ILArray<float> ret;
                if (m >= n) {
                    ret = zeros<float>(m, (economySize) ? minMN : m);
                } else {
                    // economySize is always false ... !
                    // a temporary array is needed for extraction of the compact lapack Q (?geqrf)
                    ret = zeros<float>( m, n);
                }
                ret[full,r(0,n-1)] = A[full,full]; 
                QArr = ret.GetArrayForWrite();
               
                Lapack.sgeqp3(m, n, QArr, m, ipvt, tau, ref info);
                if (info != 0) {
                    throw new ILArgumentException("error inside lapack library (?geqrf). info=" + info.ToString());
                }
                // extract R, Q
                float[] eArr = outE.GetArrayForWrite();
                if (economySize) {
                    outR.a = copyUpperTriangle(QArr, m, n, minMN);
                   
                    Lapack.sorgqr(m, minMN, minMN, QArr, m, tau, ref info);
                    // transform E into out typed vector
                    for (int i = 0; i < n; i++) {
                        eArr[i] = ipvt[i] - 1;
                    }
                } else {
                    outR.a = copyUpperTriangle(QArr, m, n, m);
                   
                    Lapack.sorgqr(m, m, minMN, QArr, m, tau, ref info);
                    if (m < n)
                        ret.a = ret[full,r(0,m - 1)];
                    // transform E into matrix
                    for (int i = 0; i < n; i++) {
                        eArr[(ipvt[i] - 1) + n * i] =  1.0f;
                    }
                }
                if (info != 0)
                    throw new ILArgumentException("error in lapack library (???gqr). info=" + info.ToString());
                return ret;
            }
        }

        /// <summary>
        /// QR decomposition - raw Lapack output
        /// </summary>
        /// <param name="A">Input matrix A</param>
        /// <returns>Orthonormal / unitary matrix Q and upper triangular 
        /// matrix R packed into single matrix. This is the output of the 
        /// lapack function ?geqrf.</returns>
        /// <remarks><para>Input matrix A will not be altered. </para>
        /// <para>The matrix returned is the direct output of the lapack 
        /// function [d,s,c,z]geqrf respectively. This means that it contains 
        /// the decomposition factors Q and R, but they are combined into a 
        /// single matrix for performance reasons. If you need one of the factors, 
        /// you would use the overloaded function 
        /// <see cref="ILNumerics.ILMath.qr(ILInArray{double},ILOutArray{double})"/> 
        /// instead, which returns those factors separately.</para></remarks>
        internal static ILRetArray<fcomplex> qr( ILInArray<fcomplex> A ) {
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentException("input A must be matrix");
                int m = A.Size[0], n = A.Size[1];
                ILArray<fcomplex> ret = A.C;
                fcomplex[] tau = ILMemoryPool.Pool.New<  fcomplex>((m < n) ? m : n);
                int info = 0;
               
                Lapack.cgeqrf(m, n, ret.GetArrayForWrite(), m, tau, ref info);
                if (info < 0)
                    throw new ILArgumentException("an unknown error occoured during decomposition");
                return ret;
            }
        }
        /// <summary>
        /// QR decomposition, returning Q and R
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix R as 
        /// result of decomposition, size [m x n]</param>
        /// <returns>Orthonormal / unitary matrix Q as result of decomposition, size [m x m]</returns>
        /// <remarks>The function returns Q and R such that the equation 
        /// <para>A = Q * R</para> holds within roundoff errors. ('*' denotes 
        /// matrix multiplication)</remarks>
        public static ILRetArray<fcomplex> qr(
                                      ILInArray<fcomplex> A,
                                      ILOutArray<fcomplex> outR ) {
            return qr(A, outR, false);
        }
        /// <summary>
        /// QR decomposition, returning Q and R, optionally economical sized
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix R as 
        /// result of decomposition, size [m x n]</param>
        /// <param name="economySize">If true, the size of Q and R will 
        /// be [m x m] and [m x n] respectively. However, if m &lt; n, 
        /// the economySize parameter has no effect. </param>
        /// <returns>Orthonormal real / unitary complex matrix Q as result 
        /// of decomposition. Size [m x m] or [m x min(m,n)], depending 
        /// on <paramref name="economySize"/> (see remarks below)</returns>
        /// <remarks>The function returns Q and R such that the equation 
        /// <para>A = Q * R</para> holds with roundoff errors. ('*' 
        /// denotes matrix multiplication.)</remarks>
        public static ILRetArray<fcomplex> qr( ILInArray<fcomplex> A
            , ILOutArray<fcomplex> outR, bool economySize ) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(outR, null)) {
                    return qr(A);
                }
                int m = A.Size[0];
                int n = A.Size[1];
                if (m < n && economySize) return qr(A, outR, false);
                ILArray<fcomplex> ret = empty<fcomplex>(ILSize.Empty00);
                if (m == 0 || n == 0) {
                    if (!object.Equals(outR,null))
                        outR.a = empty<fcomplex>(new ILSize(m, n));
                    return empty<fcomplex>(A.Size);
                }
                int minMN = (m < n) ? m : n;
                int info = 0;
                fcomplex[] tau = ILMemoryPool.Pool.New<  fcomplex>(minMN);
                if (m >= n) {
                    ret.a = zeros<fcomplex>(m, (economySize) ? minMN : m);
                } else {
                    // economySize is always false ... !
                    // a temporary array is needed for extraction of the compact lapack Q (?geqrf)
                    ret.a = zeros<fcomplex>(m, n);
                }
                ret[full, r(0, n - 1)] = A[full, full]; 
                fcomplex[] QArr = ret.GetArrayForWrite();
               
                Lapack.cgeqrf(m, n, QArr, m, tau, ref info);
                if (info != 0) {
                    throw new ILArgumentException("error inside lapack library (?geqrf). info=" + info.ToString());
                }
                // extract R, Q
                if (economySize) {
                    outR.a = copyUpperTriangle(QArr, m, n, minMN);
                   
                    Lapack.cungqr(m, minMN, minMN, QArr, m, tau, ref info);
                } else {
                    outR.a = copyUpperTriangle(QArr, m, n, m);
                   
                    Lapack.cungqr(m, m, minMN, QArr, m, tau, ref info);
                    if (m < n)
                        ret.a = ret[full,r(0,m - 1)];
                }
                if (info != 0)
                    throw new ILArgumentException("error in lapack library (???gqr). info=" + info.ToString());
                return ret;
            }
        }
        /// <summary>
        /// QR decomposition with pivoting
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix 
        /// R as result of decomposition. Size [m x n] or [min(m,n) x n] 
        /// (see remarks). </param>
        /// <param name="outE">[Output] Permutation matrix from pivoting. Size [m x m]</param>
        /// <returns>Orthonormal / unitary matrix Q as result of decomposition. 
        /// Size [m x min(m,n)]</returns>
        /// <remarks>The function returns Q, R and E such that the equation 
        /// <para>A * E = Q * R</para> holds with roundoff errors, where '*' 
        /// denotes matrix multiplication. E reflects the pivoting done 
        /// inside LAPACK in order to give R increasingly diagonal elements.</remarks>
        /// <seealso cref="ILNumerics.ILMath.qr(ILInArray{double}, ILOutArray{double}, ILOutArray{double},bool)"/>
        public static  ILRetArray<fcomplex> qr(
                                    ILInArray<fcomplex> A,
                                    ILOutArray< fcomplex> outR,
                                    ILOutArray< fcomplex> outE ) {
            return qr(A, outR, outE, false);
        }
        /// <summary>
        /// QR decomposition with pivoting, possibly economical sized
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix R as 
        /// result of decomposition. Size [m x n] or [min(m,n) x n] depending 
        /// on <paramref name="economySize"/> (see remarks).</param>
        /// <param name="economySize"><para>If true, <list type="bullet">
        /// <item>the size of Q and R will be [m x m] and [m x n] respectively. 
        /// However, if m &lt; n, the economySize parameter has no effect on 
        /// those sizes.</item>
        /// <item>the output parameter E will be returned as row permutation 
        /// vector rather than as permutation matrix</item></list></para>
        /// <para>If false, this function acts exactly as its overload 
        /// <see cref="ILNumerics.ILMath.qr(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/></para>
        /// </param>
        /// <param name="outE">[Output] Permutation matrix from pivoting. Size [m x m]. 
        /// If this is not null, the permutation matrix/ vector E will be returned.
        /// <para>E is of size [n x n], if <paramref name="economySize"/> is 
        /// true, a row vector of length n otherwise</para></param>
        /// <returns>Orthonormal / unitary matrix Q as result of decomposition. 
        /// Size [m x m] or [m x min(m,n)], depending on <paramref name="economySize"/> 
        /// (see remarks below)</returns>
        /// <remarks><para> If <paramref name="economySize"/> is false, the function 
        /// returns Q, R and E such that the equation A * E = Q * R holds within 
        /// roundoff errors. </para>
        /// <para>If <paramref name="economySize"/> is true, E will be a permutation 
        /// vector and the equation A[":",E] == Q * R holds (except roundoff).</para>
        /// <para>E reflects the pivoting of A done inside LAPACK in order to give R 
        /// increasingly diagonal elements.</para></remarks>
        public static ILRetArray<fcomplex> qr(
                        ILInArray<  fcomplex> A,
                        ILOutArray<  fcomplex> outR,
                        ILOutArray<  fcomplex> outE,
                        bool economySize ) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(outR, null)) {
                    return qr(A);
                }
                int m = A.Size[0];
                int n = A.Size[1];
                if (m < n && economySize) return qr(A, outR, false);
                if (m == 0 || n == 0) {
                    if (!object.Equals(outR,null)) 
                        outR.a = zeros< fcomplex>(m, n);
                    if (!object.Equals(outE,null))
                        outE.a = zeros< fcomplex>(1, 0);
                    return empty<fcomplex>(A.Size);
                }
                // prepare IPVT
                if (object.Equals(outE, null))
                    return qr(A, outR, economySize);
                if (!economySize) {
                    outE.a = zeros< fcomplex>( n, n);
                } else {
                    outE.a = zeros< fcomplex>( 1, n);
                }
                int[] ipvt = ILMemoryPool.Pool.New<int>(n);
                int minMN = (m < n) ? m : n;
                int info = 0;
                fcomplex[] tau = ILMemoryPool.Pool.New<  fcomplex>(minMN);
                fcomplex[] QArr;
                ILArray<fcomplex> ret;
                if (m >= n) {
                    ret = zeros<fcomplex>(m, (economySize) ? minMN : m);
                } else {
                    // economySize is always false ... !
                    // a temporary array is needed for extraction of the compact lapack Q (?geqrf)
                    ret = zeros<fcomplex>( m, n);
                }
                ret[full,r(0,n-1)] = A[full,full]; 
                QArr = ret.GetArrayForWrite();
               
                Lapack.cgeqp3(m, n, QArr, m, ipvt, tau, ref info);
                if (info != 0) {
                    throw new ILArgumentException("error inside lapack library (?geqrf). info=" + info.ToString());
                }
                // extract R, Q
                fcomplex[] eArr = outE.GetArrayForWrite();
                if (economySize) {
                    outR.a = copyUpperTriangle(QArr, m, n, minMN);
                   
                    Lapack.cungqr(m, minMN, minMN, QArr, m, tau, ref info);
                    // transform E into out typed vector
                    for (int i = 0; i < n; i++) {
                        eArr[i] = ipvt[i] - 1;
                    }
                } else {
                    outR.a = copyUpperTriangle(QArr, m, n, m);
                   
                    Lapack.cungqr(m, m, minMN, QArr, m, tau, ref info);
                    if (m < n)
                        ret.a = ret[full,r(0,m - 1)];
                    // transform E into matrix
                    for (int i = 0; i < n; i++) {
                        eArr[(ipvt[i] - 1) + n * i] =  1.0f;
                    }
                }
                if (info != 0)
                    throw new ILArgumentException("error in lapack library (???gqr). info=" + info.ToString());
                return ret;
            }
        }

        /// <summary>
        /// QR decomposition - raw Lapack output
        /// </summary>
        /// <param name="A">Input matrix A</param>
        /// <returns>Orthonormal / unitary matrix Q and upper triangular 
        /// matrix R packed into single matrix. This is the output of the 
        /// lapack function ?geqrf.</returns>
        /// <remarks><para>Input matrix A will not be altered. </para>
        /// <para>The matrix returned is the direct output of the lapack 
        /// function [d,s,c,z]geqrf respectively. This means that it contains 
        /// the decomposition factors Q and R, but they are combined into a 
        /// single matrix for performance reasons. If you need one of the factors, 
        /// you would use the overloaded function 
        /// <see cref="ILNumerics.ILMath.qr(ILInArray{double},ILOutArray{double})"/> 
        /// instead, which returns those factors separately.</para></remarks>
        internal static ILRetArray<complex> qr( ILInArray<complex> A ) {
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentException("input A must be matrix");
                int m = A.Size[0], n = A.Size[1];
                ILArray<complex> ret = A.C;
                complex[] tau = ILMemoryPool.Pool.New<  complex>((m < n) ? m : n);
                int info = 0;
               
                Lapack.zgeqrf(m, n, ret.GetArrayForWrite(), m, tau, ref info);
                if (info < 0)
                    throw new ILArgumentException("an unknown error occoured during decomposition");
                return ret;
            }
        }
        /// <summary>
        /// QR decomposition, returning Q and R
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix R as 
        /// result of decomposition, size [m x n]</param>
        /// <returns>Orthonormal / unitary matrix Q as result of decomposition, size [m x m]</returns>
        /// <remarks>The function returns Q and R such that the equation 
        /// <para>A = Q * R</para> holds within roundoff errors. ('*' denotes 
        /// matrix multiplication)</remarks>
        public static ILRetArray<complex> qr(
                                      ILInArray<complex> A,
                                      ILOutArray<complex> outR ) {
            return qr(A, outR, false);
        }
        /// <summary>
        /// QR decomposition, returning Q and R, optionally economical sized
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix R as 
        /// result of decomposition, size [m x n]</param>
        /// <param name="economySize">If true, the size of Q and R will 
        /// be [m x m] and [m x n] respectively. However, if m &lt; n, 
        /// the economySize parameter has no effect. </param>
        /// <returns>Orthonormal real / unitary complex matrix Q as result 
        /// of decomposition. Size [m x m] or [m x min(m,n)], depending 
        /// on <paramref name="economySize"/> (see remarks below)</returns>
        /// <remarks>The function returns Q and R such that the equation 
        /// <para>A = Q * R</para> holds with roundoff errors. ('*' 
        /// denotes matrix multiplication.)</remarks>
        public static ILRetArray<complex> qr( ILInArray<complex> A
            , ILOutArray<complex> outR, bool economySize ) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(outR, null)) {
                    return qr(A);
                }
                int m = A.Size[0];
                int n = A.Size[1];
                if (m < n && economySize) return qr(A, outR, false);
                ILArray<complex> ret = empty<complex>(ILSize.Empty00);
                if (m == 0 || n == 0) {
                    if (!object.Equals(outR,null))
                        outR.a = empty<complex>(new ILSize(m, n));
                    return empty<complex>(A.Size);
                }
                int minMN = (m < n) ? m : n;
                int info = 0;
                complex[] tau = ILMemoryPool.Pool.New<  complex>(minMN);
                if (m >= n) {
                    ret.a = zeros<complex>(m, (economySize) ? minMN : m);
                } else {
                    // economySize is always false ... !
                    // a temporary array is needed for extraction of the compact lapack Q (?geqrf)
                    ret.a = zeros<complex>(m, n);
                }
                ret[full, r(0, n - 1)] = A[full, full]; 
                complex[] QArr = ret.GetArrayForWrite();
               
                Lapack.zgeqrf(m, n, QArr, m, tau, ref info);
                if (info != 0) {
                    throw new ILArgumentException("error inside lapack library (?geqrf). info=" + info.ToString());
                }
                // extract R, Q
                if (economySize) {
                    outR.a = copyUpperTriangle(QArr, m, n, minMN);
                   
                    Lapack.zungqr(m, minMN, minMN, QArr, m, tau, ref info);
                } else {
                    outR.a = copyUpperTriangle(QArr, m, n, m);
                   
                    Lapack.zungqr(m, m, minMN, QArr, m, tau, ref info);
                    if (m < n)
                        ret.a = ret[full,r(0,m - 1)];
                }
                if (info != 0)
                    throw new ILArgumentException("error in lapack library (???gqr). info=" + info.ToString());
                return ret;
            }
        }
        /// <summary>
        /// QR decomposition with pivoting
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix 
        /// R as result of decomposition. Size [m x n] or [min(m,n) x n] 
        /// (see remarks). </param>
        /// <param name="outE">[Output] Permutation matrix from pivoting. Size [m x m]</param>
        /// <returns>Orthonormal / unitary matrix Q as result of decomposition. 
        /// Size [m x min(m,n)]</returns>
        /// <remarks>The function returns Q, R and E such that the equation 
        /// <para>A * E = Q * R</para> holds with roundoff errors, where '*' 
        /// denotes matrix multiplication. E reflects the pivoting done 
        /// inside LAPACK in order to give R increasingly diagonal elements.</remarks>
        /// <seealso cref="ILNumerics.ILMath.qr(ILInArray{double}, ILOutArray{double}, ILOutArray{double},bool)"/>
        public static  ILRetArray<complex> qr(
                                    ILInArray<complex> A,
                                    ILOutArray< complex> outR,
                                    ILOutArray< complex> outE ) {
            return qr(A, outR, outE, false);
        }
        /// <summary>
        /// QR decomposition with pivoting, possibly economical sized
        /// </summary>
        /// <param name="A">Input matrix A of size [m x n]</param>
        /// <param name="outR">[Output] Upper triangular matrix R as 
        /// result of decomposition. Size [m x n] or [min(m,n) x n] depending 
        /// on <paramref name="economySize"/> (see remarks).</param>
        /// <param name="economySize"><para>If true, <list type="bullet">
        /// <item>the size of Q and R will be [m x m] and [m x n] respectively. 
        /// However, if m &lt; n, the economySize parameter has no effect on 
        /// those sizes.</item>
        /// <item>the output parameter E will be returned as row permutation 
        /// vector rather than as permutation matrix</item></list></para>
        /// <para>If false, this function acts exactly as its overload 
        /// <see cref="ILNumerics.ILMath.qr(ILInArray{double}, ILOutArray{double}, ILOutArray{double})"/></para>
        /// </param>
        /// <param name="outE">[Output] Permutation matrix from pivoting. Size [m x m]. 
        /// If this is not null, the permutation matrix/ vector E will be returned.
        /// <para>E is of size [n x n], if <paramref name="economySize"/> is 
        /// true, a row vector of length n otherwise</para></param>
        /// <returns>Orthonormal / unitary matrix Q as result of decomposition. 
        /// Size [m x m] or [m x min(m,n)], depending on <paramref name="economySize"/> 
        /// (see remarks below)</returns>
        /// <remarks><para> If <paramref name="economySize"/> is false, the function 
        /// returns Q, R and E such that the equation A * E = Q * R holds within 
        /// roundoff errors. </para>
        /// <para>If <paramref name="economySize"/> is true, E will be a permutation 
        /// vector and the equation A[":",E] == Q * R holds (except roundoff).</para>
        /// <para>E reflects the pivoting of A done inside LAPACK in order to give R 
        /// increasingly diagonal elements.</para></remarks>
        public static ILRetArray<complex> qr(
                        ILInArray<  complex> A,
                        ILOutArray<  complex> outR,
                        ILOutArray<  complex> outE,
                        bool economySize ) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(outR, null)) {
                    return qr(A);
                }
                int m = A.Size[0];
                int n = A.Size[1];
                if (m < n && economySize) return qr(A, outR, false);
                if (m == 0 || n == 0) {
                    if (!object.Equals(outR,null)) 
                        outR.a = zeros< complex>(m, n);
                    if (!object.Equals(outE,null))
                        outE.a = zeros< complex>(1, 0);
                    return empty<complex>(A.Size);
                }
                // prepare IPVT
                if (object.Equals(outE, null))
                    return qr(A, outR, economySize);
                if (!economySize) {
                    outE.a = zeros< complex>( n, n);
                } else {
                    outE.a = zeros< complex>( 1, n);
                }
                int[] ipvt = ILMemoryPool.Pool.New<int>(n);
                int minMN = (m < n) ? m : n;
                int info = 0;
                complex[] tau = ILMemoryPool.Pool.New<  complex>(minMN);
                complex[] QArr;
                ILArray<complex> ret;
                if (m >= n) {
                    ret = zeros<complex>(m, (economySize) ? minMN : m);
                } else {
                    // economySize is always false ... !
                    // a temporary array is needed for extraction of the compact lapack Q (?geqrf)
                    ret = zeros<complex>( m, n);
                }
                ret[full,r(0,n-1)] = A[full,full]; 
                QArr = ret.GetArrayForWrite();
               
                Lapack.zgeqp3(m, n, QArr, m, ipvt, tau, ref info);
                if (info != 0) {
                    throw new ILArgumentException("error inside lapack library (?geqrf). info=" + info.ToString());
                }
                // extract R, Q
                complex[] eArr = outE.GetArrayForWrite();
                if (economySize) {
                    outR.a = copyUpperTriangle(QArr, m, n, minMN);
                   
                    Lapack.zungqr(m, minMN, minMN, QArr, m, tau, ref info);
                    // transform E into out typed vector
                    for (int i = 0; i < n; i++) {
                        eArr[i] = ipvt[i] - 1;
                    }
                } else {
                    outR.a = copyUpperTriangle(QArr, m, n, m);
                   
                    Lapack.zungqr(m, m, minMN, QArr, m, tau, ref info);
                    if (m < n)
                        ret.a = ret[full,r(0,m - 1)];
                    // transform E into matrix
                    for (int i = 0; i < n; i++) {
                        eArr[(ipvt[i] - 1) + n * i] =  1.0;
                    }
                }
                if (info != 0)
                    throw new ILArgumentException("error in lapack library (???gqr). info=" + info.ToString());
                return ret;
            }
        }


#endregion HYCALPER AUTO GENERATED CODE

    }
}