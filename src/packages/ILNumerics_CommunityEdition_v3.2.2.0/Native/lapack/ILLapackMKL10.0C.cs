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


#pragma warning disable 1570, 1591
using System;
using System.Collections.Generic;
using System.Text;
using ILNumerics.Native;
using System.Runtime.InteropServices;
using ILNumerics.Exceptions;
using System.Security; 
using ILNumerics.Misc; 

namespace ILNumerics.Native  {
    /// <summary>
    /// Generic LAPACK implementation, unsupported processor types
    /// </summary>
    public class ILLapackMKL10_0 : IILLapack {

        #region DLL INCLUDES 
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        internal static extern int mkl_domain_set_num_threads(ref int num, ref int mask);
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        internal static extern int mkl_domain_get_max_threads(ref int mask);
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical] 
        internal static extern void mkl_set_num_threads(int num);
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        internal static extern void omp_set_num_threads(int num);
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        internal static extern int mkl_get_max_threads();
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        internal static extern void mkl_set_dynamic(ref int boolean_value);
        [DllImport("mkl_custom", CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        internal static extern void mkl_free_buffers();

        [DllImport("mkl_custom", EntryPoint = "ilaenv",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern int mkl_ilaenv(ref int ispec, ref string name, ref string opts, ref int n1, ref int n2, ref int n3, ref int n4);

        ///////////////////////////   DOUBLE LAPACK /////////////////////////////////
        [DllImport("mkl_custom", EntryPoint = "DGEMM",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dgemm(ref char TransA, ref char TransB, ref int M,ref int N, ref int K, ref  double alpha, IntPtr A, ref int lda, IntPtr B, ref int ldb, ref double beta, double[] C, ref int ldc);
        [DllImport("mkl_custom", EntryPoint = "SGEMM",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_sgemm(ref char TransA, ref char TransB, ref int M,ref int N, ref int K, ref float alpha, IntPtr A,ref int lda, IntPtr B, ref int ldb, ref float beta,float[] C, ref int ldc);
        [DllImport("mkl_custom", EntryPoint = "CGEMM",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cgemm(ref char TransA, ref char TransB, ref int M,ref int N, ref int K, ref fcomplex alpha, IntPtr A, ref int lda, IntPtr B, ref int ldb, ref fcomplex beta,[In,Out] fcomplex [] C, ref int ldc);
        [DllImport("mkl_custom", EntryPoint = "ZGEMM",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zgemm(ref char TransA, ref char TransB, ref int M, ref int N, ref int K, ref complex alpha, IntPtr A, ref int lda, IntPtr B, ref int ldb, ref complex beta,[In,Out] complex [] C, ref int ldc);

        [DllImport("mkl_custom", EntryPoint = "DGESDD",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dgesdd(ref char jobz, ref int m, ref int n,double[] a, ref int lda, double[] s,double[] u, ref int ldu, double[] vt,ref int ldvt, double[] work, ref int lwork,int[] iwork, ref int info);
        [DllImport("mkl_custom", EntryPoint = "SGESDD",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_sgesdd(ref char jobz, ref int m, ref int n,float[] a, ref int lda, float[] s,float[] u, ref int ldu, float[] vt, ref int ldvt, float[] work, ref int lwork,int[] iwork, ref int info);
        [DllImport("mkl_custom", EntryPoint = "CGESDD",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cgesdd(ref char jobz, ref int m, ref int n,[In, Out] fcomplex[] a, ref int lda, float[] s,[In, Out] fcomplex[] u, ref int ldu, [In, Out]  fcomplex[] vt, ref int ldvt, [In, Out] fcomplex[] work, ref int lwork,[In,Out] float[] rwork, int[] iwork, ref int info);
        [DllImport("mkl_custom", EntryPoint = "ZGESDD",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zgesdd(ref char jobz, ref int m, ref int n,[In, Out] complex[] a, ref int lda, double[] s,[In, Out] complex[] u, ref int ldu, [In, Out] complex[] vt,ref int ldvt, [In, Out] complex[] work, ref int lwork,[In,Out] double[] rwork, int[] iwork, ref int info);

        [DllImport("mkl_custom", EntryPoint = "DGESVD",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dgesvd(ref char jobu, ref  char jobvt, ref int m, ref int n,double[] a, ref int lda, double[] s,double[] u, ref int ldu, double[] vt,ref int ldvt, double[] work, ref int lwork,int[] iwork,ref int info);
        [DllImport("mkl_custom", EntryPoint = "SGESVD",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_sgesvd(ref char jobu, ref  char jobvt, ref int m, ref int n, float[] a, ref int lda, float[] s,float[] u, ref int ldu, float[] vt,ref int ldvt, float[] work, ref int lwork,int[] iwork, ref int info);
        [DllImport("mkl_custom", EntryPoint = "CGESVD",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cgesvd(ref char jobu, ref  char jobvt, ref int m, ref int n,[In, Out] fcomplex[] a, ref int lda, float[] s,[In, Out] fcomplex[] u, ref int ldu,[In,Out] fcomplex[] vt,ref int ldvt, [In, Out]  fcomplex[] work, ref int lwork,int[] iwork,ref int info);
        [DllImport("mkl_custom", EntryPoint = "ZGESVD",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zgesvd(ref char jobu, ref  char jobvt, ref int m, ref int n,[In, Out] complex[] a, ref int lda, double[] s,[In, Out] complex[] u, ref int ldu, [In, Out] complex[] vt,ref int ldvt, [In, Out]  complex[] work, ref int lwork,int[] iwork,ref int info);

        [DllImport("mkl_custom", EntryPoint = "DPOTRF",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dpotrf (ref char uplo, ref int n, double [] A, ref int lda, ref int info);
        [DllImport("mkl_custom", EntryPoint = "SPOTRF",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_spotrf (ref char uplo, ref int n, float [] A, ref int lda, ref int info);
        [DllImport("mkl_custom", EntryPoint = "CPOTRF",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cpotrf (ref char uplo, ref int n,[In,Out] fcomplex [] A, ref int lda, ref int info);
        [DllImport("mkl_custom", EntryPoint = "ZPOTRF",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zpotrf (ref char uplo, ref int n, [In,Out] complex [] A, ref int lda, ref int info);
        
        [DllImport("mkl_custom", EntryPoint = "DPOTRI",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dpotri (ref char uplo,ref  int n, double [] A, ref int lda, ref int info);
        [DllImport("mkl_custom", EntryPoint = "SPOTRI",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_spotri (ref char uplo, ref int n, float [] A, ref int lda, ref int info);
        [DllImport("mkl_custom", EntryPoint = "CPOTRI",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cpotri (ref char uplo, ref int n,[In,Out] fcomplex [] A, ref int lda,ref int info);
        [DllImport("mkl_custom", EntryPoint = "ZPOTRI",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zpotri (ref char uplo, ref int n, [In,Out] complex [] A, ref int lda,ref int info);

        [DllImport("mkl_custom", EntryPoint = "DPOTRS",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dpotrs (ref char uplo,ref  int n, ref int NRHS, double [] A, ref int lda, double[] B, ref int ldb, ref int info);
        [DllImport("mkl_custom", EntryPoint = "SPOTRS",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_spotrs (ref char uplo,ref  int n, ref int NRHS, float [] A, ref int lda, float[] B, ref int ldb, ref int info);
        [DllImport("mkl_custom", EntryPoint = "CPOTRS",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cpotrs (ref char uplo,ref  int n, ref int NRHS, [In,Out] fcomplex [] A, ref int lda, [In,Out] fcomplex[] B, ref int ldb, ref int info);
        [DllImport("mkl_custom", EntryPoint = "ZPOTRS",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zpotrs (ref char uplo,ref  int n, ref int NRHS, [In,Out] complex [] A, ref int lda, [In,Out] complex[] B, ref int ldb, ref int info);

        [DllImport("mkl_custom", EntryPoint = "DGETRF",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dgetrf (ref int M, ref int N, double [] A, ref int LDA, int[] IPIV, ref int info);
        [DllImport("mkl_custom", EntryPoint = "SGETRF",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_sgetrf (ref int M, ref int N, float[] A, ref int LDA, int [] IPIV, ref int info);
        [DllImport("mkl_custom", EntryPoint = "CGETRF",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cgetrf (ref int M, ref int N, [In,Out] fcomplex [] A, ref int LDA, int [] IPIV, ref int info);
        [DllImport("mkl_custom", EntryPoint = "ZGETRF",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zgetrf (ref int M, ref int N, [In,Out] complex [] A, ref int LDA, int [] IPIV, ref int info);

        [DllImport("mkl_custom", EntryPoint = "DGETRI",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dgetri (ref int N, double[] A,ref  int LDA, int[] IPIV, double[] work, ref int lwork, ref int info);
        [DllImport("mkl_custom", EntryPoint = "SGETRI",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_sgetri (ref int N, float [] A, ref int LDA, int [] IPIV, float[] work, ref int lwork, ref int info);
        [DllImport("mkl_custom", EntryPoint = "CGETRI",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cgetri (ref int N, [In,Out] fcomplex [] A, ref int LDA,[In,Out] int [] IPIV, [In,Out] fcomplex[] work, ref int lwork, ref int info);
        [DllImport("mkl_custom", EntryPoint = "ZGETRI",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zgetri (ref int N, [In,Out] complex [] A, ref int LDA, int [] IPIV, [In,Out] complex[] work, ref int lwork, ref int info);

        [DllImport("mkl_custom", EntryPoint = "DGEQRF",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dgeqrf (ref int M, ref int N, double [] A, ref int lda, double [] tau, double[] work, ref int lwork, ref int info);
        [DllImport("mkl_custom", EntryPoint = "SGEQRF",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_sgeqrf (ref int M, ref int N, float [] A, ref int lda, float [] tau, float[] work, ref int lwork,  ref int info);
        [DllImport("mkl_custom", EntryPoint = "CGEQRF",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cgeqrf (ref int M, ref int N, [In,Out] fcomplex [] A, ref int lda, [In,Out] fcomplex [] tau, [In,Out] fcomplex[] work, ref int lwork, ref int info);
        [DllImport("mkl_custom", EntryPoint = "ZGEQRF",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zgeqrf (ref int M, ref int N, [In,Out] complex [] A, ref int lda, [In,Out] complex [] tau, [In,Out] complex[] work, ref int lwork,  ref int info);

        [DllImport("mkl_custom", EntryPoint = "DGEQP3",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dgeqp3 (ref int M, ref int N, double [] A, ref int LDA, int [] JPVT, double [] tau, double [] work, ref int lwork, ref int info );
        [DllImport("mkl_custom", EntryPoint = "SGEQP3",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_sgeqp3 (ref int M, ref int N, float [] A, ref int LDA, int [] JPVT, float [] tau, float [] work, ref int lwork, ref int info );
        [DllImport("mkl_custom", EntryPoint = "CGEQP3",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cgeqp3 (ref int M, ref int N,[In,Out] fcomplex [] A, ref int LDA,[In,Out] int [] JPVT, [In,Out] fcomplex [] tau, [In,Out] fcomplex [] work, ref int lwork,[In,Out] float [] rwork, ref int info );
        [DllImport("mkl_custom", EntryPoint = "ZGEQP3",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zgeqp3 (ref int M, ref int N, [In,Out] complex [] A, ref int LDA, [In,Out] int [] JPVT, [In,Out] complex [] tau, [In,Out] complex [] work, ref int lwork, [In,Out] double [] rwork, ref int info );

        [DllImport("mkl_custom", EntryPoint = "DORMQR",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dormqr (ref char side, ref char trans, ref int m, ref int n, ref int k, double[] A, int lda, double[] tau, double[] C, ref int ldc, ref int info);
        [DllImport("mkl_custom", EntryPoint = "SORMQR",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_sormqr (ref char side, ref char trans, ref int m, ref int n, ref int k, float [] A, ref int lda, float [] tau , float [] C, ref int ldc, ref int info);
        
        [DllImport("mkl_custom", EntryPoint = "DORGQR",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dorgqr (ref int m, ref int n, ref int k, double[] A,ref int lda, double[] tau, double[] work, ref int lwork, ref int info);
        [DllImport("mkl_custom", EntryPoint = "SORGQR",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_sorgqr (ref int m, ref int n, ref int k, float [] A, ref int lda, float [] tau , float[] work, ref int lwork,  ref int info);
        [DllImport("mkl_custom", EntryPoint = "CUNGQR",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cungqr (ref int m, ref int n, ref int k, [In,Out] fcomplex[] A,ref int lda, [In,Out] fcomplex[] tau, [In,Out] fcomplex[] work, ref int lwork, ref int info);
        [DllImport("mkl_custom", EntryPoint = "ZUNGQR",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zungqr (ref int m, ref int n, ref int k, [In,Out] complex[] A,ref int lda, [In,Out] complex[] tau, [In,Out] complex[] work, ref int lwork, ref int info);
        
        [DllImport("mkl_custom", EntryPoint = "DTRTRS", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dtrtrs (ref char uplo, ref char transA, ref char diag, ref int N, ref int nrhs, IntPtr A, ref int LDA, IntPtr B, ref int LDB, ref int info);
        [DllImport("mkl_custom", EntryPoint = "STRTRS",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_strtrs (ref char uplo, ref char transA, ref char diag, ref int N, ref int nrhs, IntPtr A, ref int LDA, IntPtr B, ref int LDB, ref int info);
        [DllImport("mkl_custom", EntryPoint = "CTRTRS",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_ctrtrs (ref char uplo, ref char transA, ref char diag, ref int N, ref int nrhs, IntPtr A, ref int LDA, IntPtr B, ref int LDB, ref int info);
        [DllImport("mkl_custom", EntryPoint = "ZTRTRS",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_ztrtrs (ref char uplo, ref char transA, ref char diag, ref int N, ref int nrhs, IntPtr A, ref int LDA, IntPtr B, ref int LDB, ref int info);
        
        [DllImport("mkl_custom", EntryPoint = "DGETRS",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dgetrs(ref char trans, ref int N, ref int NRHS, double[] A, ref int LDA, int[] IPIV, double[] B, ref int LDB, ref int info);
        [DllImport("mkl_custom", EntryPoint = "SGETRS",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_sgetrs(ref char trans, ref int N, ref int NRHS, float[] A, ref int LDA, int[] IPIV, float[] B, ref int LDB, ref int info);
        [DllImport("mkl_custom", EntryPoint = "CGETRS",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cgetrs(ref char trans, ref int N, ref int NRHS, [In,Out] fcomplex[] A, ref int LDA, int[] IPIV, [In,Out] fcomplex[] B, ref int LDB, ref int info);
        [DllImport("mkl_custom", EntryPoint = "ZGETRS",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zgetrs(ref char trans, ref int N, ref int NRHS, [In,Out] complex[] A, ref int LDA, int[] IPIV, [In,Out] complex[] B, ref int LDB, ref int info);

        [DllImport("mkl_custom", EntryPoint = "DGELSD",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dgelsd (ref int m,ref int n,ref int nrhs, double[] A,ref int lda, double[] B,ref int ldb, double[] S, ref double RCond, ref int rank, double[] work,ref int lwork, int[] iwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "SGELSD",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_sgelsd (ref int m,ref int n,ref int nrhs, float[] A,ref int lda, float[] B,ref int ldb, float[] S, ref float RCond, ref int rank, float[] work,ref int lwork, int[] iwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "CGELSD",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cgelsd (ref int m,ref int n,ref int nrhs, [In,Out] fcomplex[] A,ref int lda, [In,Out] fcomplex[] B,ref int ldb, float[] S , ref float RCond, ref int rank, [In,Out] fcomplex[] work, ref int lwork, float[] rwork, int[] iwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "ZGELSD",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zgelsd (ref int m,ref int n,ref int nrhs, [In,Out] complex[] A,ref int lda, [In,Out] complex[] B,ref int ldb, double[] S, ref double RCond, ref int rank, [In,Out]  complex[] work,ref int lwork, double[] rwork, int[] iwork, ref int info); 
        
        [DllImport("mkl_custom", EntryPoint = "DGELSY",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dgelsy (ref int m,ref int n,ref int nrhs, double[] A,ref int lda, double[] B,ref int ldb, int[] JPVT0, ref double RCond, ref int rank, double[] work, ref int lwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "SGELSY",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_sgelsy (ref int m,ref int n,ref int nrhs, float[] A,ref int lda, float[] B,ref int ldb, int[] JPVT0, ref float RCond, ref int rank, float[] work,ref int lwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "CGELSY",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cgelsy (ref int m,ref int n,ref int nrhs, [In,Out] fcomplex[] A,ref int lda, [In,Out] fcomplex[] B,ref int ldb, int[] JPVT0, ref float RCond, ref int rank, [In,Out] fcomplex[] work, ref int lwork, float[] rwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "ZGELSY",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zgelsy (ref int m,ref int n,ref int nrhs, [In,Out] complex[] A,ref int lda, [In,Out] complex[] B,ref int ldb, int[] JPVT0, ref double RCond, ref int rank, [In,Out]  complex[] work, ref int lwork, double[] rwork, ref int info); 
        
        [DllImport("mkl_custom", EntryPoint = "DGEEVX",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dgeevx (ref char balance, ref char jobvl, ref char jobvr, ref char sense, ref int n,            double[] A, ref int lda,          double[] wr, double[] wi, double[] vl, ref int ldvl,            double[] vr, ref int ldvr, ref int ilo, ref int ihi, double[] scale, ref double abnrm, double[] rconde, double[] rcondv, double[] work, ref int lwork, int [] iwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "SGEEVX",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]       
        private static extern void mkl_sgeevx (ref char balance, ref char jobvl, ref char jobvr, ref char sense, ref int n,             float[] A, ref int lda,             float[] wr, float[] wi, float[] vl, ref int ldvl,             float[] vr, ref int ldvr, ref int ilo, ref int ihi, float[]  scale, ref float  abnrm, float[]  rconde, float[]  rcondv, float [] work, ref int lwork, int [] iwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "CGEEVX",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]       
        private static extern void mkl_cgeevx (ref char balance, ref char jobvl, ref char jobvr, ref char sense, ref int n, [In,Out] fcomplex[] A, ref int lda, [In,Out] fcomplex[] w,  [In,Out] fcomplex[] vl, ref int ldvl, [In,Out] fcomplex[] vr, ref int ldvr, ref int ilo, ref int ihi, float[]  scale, ref float  abnrm, float[]  rconde, float[]  rcondv, [In,Out] fcomplex[] work, ref int lwork, float[] rwork, ref int info);
        [DllImport("mkl_custom", EntryPoint = "ZGEEVX",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]       
        private static extern void mkl_zgeevx (ref char balance, ref char jobvl, ref char jobvr, ref char sense, ref int n, [In,Out]  complex[] A, ref int lda, [In,Out] complex[]  w,   [In,Out] complex[] vl, ref int ldvl, [In,Out]  complex[] vr, ref int ldvr, ref int ilo, ref int ihi, double[] scale, ref double abnrm, double[] rconde, double[] rcondv, [In,Out] complex[] work, ref int lwork, double[] rwork, ref int info);

        [DllImport("mkl_custom", EntryPoint = "DSYEVR",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dsyevr (ref char jobz, ref char range, ref char uplo, ref int n,          double  [] A, ref int lda, ref double vl, ref double vu, ref int il, ref int iu, ref double abstol, ref int m, double[] w,          double  [] z, ref int ldz, int[] isuppz, double[] work, ref int lwork, int[] iwork, ref int liwork, ref int info); 
        //[DllImport("mkl_custom", EntryPoint = "DSYEVR",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        //private static extern void mkl_dsyevr (ref char jobz, ref char range, ref char uplo, ref int n,          double  [] A, ref int lda, ref double vl, ref double vu, ref int il, ref int iu, ref double abstol, ref int m, double[] w,          double  [] z, ref int ldz, int[] isuppz, double[] work, ref int lwork, int[] iwork, ref int liwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "SSYEVR",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_ssyevr (ref char jobz, ref char range, ref char uplo, ref int n,          float   [] A, ref int lda, ref float  vl, ref float  vu, ref int il, ref int iu, ref float  abstol, ref int m, float [] w,          float   [] z, ref int ldz, int[] isuppz, float [] work, ref int lwork, int[] iwork, ref int liwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "CHEEVR",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_cheevr (ref char jobz, ref char range, ref char uplo, ref int n, [In,Out] fcomplex[] A, ref int lda, ref float  vl, ref float  vu, ref int il, ref int iu, ref float  abstol, ref int m, float [] w, [In,Out] fcomplex[] z, ref int ldz, int[] isuppz, [In,Out] fcomplex[] work, ref int lwork, float[] rwork, ref int lrwork, int[] iwork, ref int liwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "ZHEEVR",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zheevr (ref char jobz, ref char range, ref char uplo, ref int n, [In,Out] complex [] A, ref int lda, ref double vl, ref double vu, ref int il, ref int iu, ref double abstol, ref int m, double[] w, [In,Out] complex [] z, ref int ldz, int[] isuppz, [In,Out] complex[] work, ref int lwork, double[] rwork, ref int lrwork, int[] iwork, ref int liwork, ref int info); 

        [DllImport("mkl_custom", EntryPoint = "DSYGV",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_dsygv (ref int itype, ref char jobz, ref char uplo, ref int n, double  [] A, ref int lda, double  [] B, ref int ldb, double [] w, double [] work, ref int lwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "SSYGV",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_ssygv (ref int itype, ref char jobz, ref char uplo, ref int n, float   [] A, ref int lda, float   [] B, ref int ldb, float [] w,  float  [] work, ref int lwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "CHEGV",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_chegv (ref int itype, ref char jobz, ref char uplo, ref int n, [In,Out] fcomplex[] A, ref int lda, [In,Out] fcomplex [] B, ref int ldb, float  [] w, [In,Out] fcomplex[] work , ref int lwork, float[] rwork, ref int info); 
        [DllImport("mkl_custom", EntryPoint = "ZHEGV",CallingConvention =CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity, SecurityCritical]
        private static extern void mkl_zhegv (ref int itype, ref char jobz, ref char uplo, ref int n, [In,Out] complex [] A, ref int lda, [In,Out] complex  [] B, ref int ldb, double [] w, [In,Out]  complex[] work , ref int lwork, double[] rwork, ref int info); 
        #endregion DLL INCLUDES

        public ILLapackMKL10_0() {
            Init();
        }

        public static void Init() {
            try {
                // unless the user explicitely configured the number of threads, let omp configure it! 
                if (Settings.MaxNumberThreadsConfigured) {
                    SetDomainNumThreads(Settings.MaxNumberThreads);
                }
            } catch (System.IO.FileNotFoundException) { }
        }

        #region ILLAPACK INTERFACE 

        [SecuritySafeCritical]
        private int ILAENV(int ispec, string name, string opts, int n1, int n2, int n3, int n4) {
            return mkl_ilaenv(ref ispec, ref name, ref opts, ref n1, ref n2, ref n3, ref n4); 
        }
        [SecuritySafeCritical]
        private static void SetNumThreads(int numThreads) {
            mkl_set_num_threads(numThreads);
            //omp_set_num_threads(numThreads); 
        }
        [SecuritySafeCritical]
        private static void SetDomainNumThreads(int numThreads) {
            int mklblas = MKLValues.MKL_BLAS;
            mkl_domain_set_num_threads(ref numThreads,ref mklblas);
            //omp_set_num_threads(numThreads); 
        }
        [SecuritySafeCritical]
        private static void SetDynamicThreads(bool dynamic) {
            int val = dynamic ? 1 : 0; 
            mkl_set_dynamic(ref val);
        }
        [SecuritySafeCritical]
        public static int GetMaxThreads() {
            int dummy = MKLValues.MKL_BLAS; 
            int ret = mkl_domain_get_max_threads(ref dummy);
            return ret; 
        }
        /// <summary>
        /// Free all buffers from the MKL Fast Memory Management. Use sparingly and carefully! 
        /// </summary>
        [SecuritySafeCritical]
        public void FreeBuffers() {
            mkl_free_buffers();
        }

        /// <summary>
        /// Implement wrapper for ATLAS GeneralMatrixMultiply
        /// </summary>
        /// <param name="TransA">Transposition state for matrix A: one of the constants in enum CBlas_Transpose</param>
        /// <param name="TransB">Transposition state for matrix B: one of the constants in enum CBlas_Transpose</param>
        /// <param name="M">Number of rows in A</param>
        /// <param name="N">Number of columns in B</param>
        /// <param name="K">Number of columns in A and number of rows in B</param>
        /// <param name="alpha">multiplicationi factor for A</param>
        /// <param name="A">pointer to double array A</param>
        /// <param name="lda">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix B</param>
        /// <param name="B">pointer to double array B</param>
        /// <param name="ldb">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix A</param>
        /// <param name="beta">multiplication faktor for matrix B</param>
        /// <param name="C">pointer to predefined double array C of neccessary length</param>
        /// <param name="ldc">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix C</param>
        /// <remarks>All parameters except C are readonly. Only elements of matrix C will be altered. C must be a predefined 
        /// continous double array of size MxN</remarks>
        [SecuritySafeCritical]
        public void dgemm(char TransA, char TransB, int M, int N, int K, double alpha, IntPtr A, int lda, IntPtr B, int ldb, double beta, double[] C, int ldc) {
            mkl_dgemm(ref TransA, ref TransB, ref M, ref  N, ref  K, ref  alpha, A, ref lda, B, ref ldb, ref  beta, C, ref  ldc);
        }
        /// <summary>
        /// Implement wrapper for ATLAS GeneralMatrixMultiply
        /// </summary>
        /// <param name="TransA">Transposition state for matrix A: one of the constants in enum CBlas_Transpose</param>
        /// <param name="TransB">Transposition state for matrix B: one of the constants in enum CBlas_Transpose</param>
        /// <param name="M">Number of rows in A</param>
        /// <param name="N">Number of columns in B</param>
        /// <param name="K">Number of columns in A and number of rows in B</param>
        /// <param name="alpha">multiplicationi factor for A</param>
        /// <param name="A">pointer to double array A</param>
        /// <param name="lda">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix B</param>
        /// <param name="B">pointer to double array B</param>
        /// <param name="ldb">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix A</param>
        /// <param name="beta">multiplication faktor for matrix B</param>
        /// <param name="C">pointer to predefined double array C of neccessary length</param>
        /// <param name="ldc">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix C</param>
        /// <remarks>All parameters except C are readonly. Only elements of matrix C will be altered. C must be a predefined 
        /// continous double array of size MxN</remarks>
        [SecuritySafeCritical]
        public void sgemm(char TransA, char TransB, int M, int N, int K, float alpha, IntPtr A, int lda, IntPtr B, int ldb, float beta, float[] C, int ldc) {
            mkl_sgemm(ref TransA, ref TransB, ref M, ref  N, ref  K, ref  alpha, A, ref lda, B, ref ldb, ref  beta, C, ref  ldc);
        }
        /// <summary>
        /// Implement wrapper for ATLAS GeneralMatrixMultiply
        /// </summary>
        /// <param name="TransA">Transposition state for matrix A: one of the constants in enum CBlas_Transpose</param>
        /// <param name="TransB">Transposition state for matrix B: one of the constants in enum CBlas_Transpose</param>
        /// <param name="M">Number of rows in A</param>
        /// <param name="N">Number of columns in B</param>
        /// <param name="K">Number of columns in A and number of rows in B</param>
        /// <param name="alpha">multiplicationi factor for A</param>
        /// <param name="A">pointer to double array A</param>
        /// <param name="lda">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix B</param>
        /// <param name="B">pointer to double array B</param>
        /// <param name="ldb">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix A</param>
        /// <param name="beta">multiplication faktor for matrix B</param>
        /// <param name="C">pointer to predefined double array C of neccessary length</param>
        /// <param name="ldc">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix C</param>
        /// <remarks>All parameters except C are readonly. Only elements of matrix C will be altered. C must be a predefined 
        /// continous double array of size MxN</remarks>
        [SecuritySafeCritical]
        public void cgemm(char TransA, char TransB, int M, int N, int K, fcomplex alpha, IntPtr A, int lda, IntPtr B, int ldb, fcomplex beta, fcomplex[] C, int ldc) {
            mkl_cgemm(ref TransA, ref TransB, ref M, ref  N, ref  K, ref  alpha, A, ref lda, B, ref ldb, ref  beta, C, ref  ldc);
        }
        /// <summary>
        /// Implement wrapper for ATLAS GeneralMatrixMultiply
        /// </summary>
        /// <param name="TransA">Transposition state for matrix A: one of the constants in enum CBlas_Transpose</param>
        /// <param name="TransB">Transposition state for matrix B: one of the constants in enum CBlas_Transpose</param>
        /// <param name="M">Number of rows in A</param>
        /// <param name="N">Number of columns in B</param>
        /// <param name="K">Number of columns in A and number of rows in B</param>
        /// <param name="alpha">multiplicationi factor for A</param>
        /// <param name="A">pointer to double array A</param>
        /// <param name="lda">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix B</param>
        /// <param name="B">pointer to double array B</param>
        /// <param name="ldb">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix A</param>
        /// <param name="beta">multiplication faktor for matrix B</param>
        /// <param name="C">pointer to predefined double array C of neccessary length</param>
        /// <param name="ldc">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix C</param>
        /// <remarks>All parameters except C are readonly. Only elements of matrix C will be altered. C must be a predefined 
        /// continous double array of size MxN</remarks>
        [SecuritySafeCritical]
        public void zgemm(char TransA, char TransB, int M, int N, int K, complex alpha, IntPtr A, int lda, IntPtr B, int ldb, complex beta, complex[] C, int ldc) {
            mkl_zgemm(ref TransA, ref TransB, ref M, ref  N, ref  K, ref  alpha, A, ref lda, B, ref ldb, ref  beta, C, ref  ldc);
        }


        
        [SecuritySafeCritical]
        public void  dgesdd(char jobz, int m, int n,  double[] a, int lda,  double[] s,  double[] u, int ldu,  double[] vt, int ldvt, ref int info) {
            try {
                 double [] work = new  double [1] { ( double )0.0 };
                int lwork = -1;
                int[] iwork = new int[((m < n) ? m : n) * 8];
                 mkl_dgesdd (ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, iwork, ref info);
                if (work[0] != 0) {
                    work = new  double [(int)work[0]];
                    lwork = work.Length;
                     mkl_dgesdd (ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, iwork, ref info);
                }
            } catch (Exception e) {
                if (e is OutOfMemoryException) {
                     dgesvd (jobz, m, n, a, lda, s, u, ldu, vt, ldvt, ref info); 
                }
                throw new ILException("Unable to do " +  "dgesdd"  + ".", e); 
            }
        }

#region HYCALPER AUTO GENERATED CODE

       
        [SecuritySafeCritical]
        public void  sgesdd(char jobz, int m, int n,  float[] a, int lda,  float[] s,  float[] u, int ldu,  float[] vt, int ldvt, ref int info) {
            try {
                float [] work = new  float [1] { ( float )0.0 };
                int lwork = -1;
                int[] iwork = new int[((m < n) ? m : n) * 8];
                mkl_sgesdd (ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, iwork, ref info);
                if (work[0] != 0) {
                    work = new  float [(int)work[0]];
                    lwork = work.Length;
                    mkl_sgesdd (ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, iwork, ref info);
                }
            } catch (Exception e) {
                if (e is OutOfMemoryException) {
                    sgesvd (jobz, m, n, a, lda, s, u, ldu, vt, ldvt, ref info); 
                }
                throw new ILException("Unable to do " +  "sgesdd"  + ".", e); 
            }
        }

#endregion HYCALPER AUTO GENERATED CODE



        [SecuritySafeCritical]
        public void  zgesdd(char jobz, int m, int n,  complex[] a, int lda,  double[] s,  complex[] u, int ldu, complex[] vt, int ldvt, ref int info)
        {
            try {
                 complex [] work = new  complex [1] { (  complex )0.0 };
                 double [] rwork; 
                int minMN = (m < n) ? m : n;
                if (jobz == 'N') {
                    rwork = new  double [minMN * 7];  
                } else {
                    rwork = new  double [5 * minMN * minMN + 5 * minMN];  
                }
                int lwork = -1;
                int[] iwork = new int[minMN * 8];
                 mkl_zgesdd (ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, rwork, iwork, ref info);
                if (work[0] != 0) {
                    work = new  complex [(int)work[0].real];
                    lwork = work.Length;
                     mkl_zgesdd (ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork,rwork, iwork, ref info);
                }
            } catch (Exception e) {
                if (e is OutOfMemoryException) {
                     zgesvd (jobz, m, n, a, lda, s, u, ldu, vt, ldvt, ref info); 
                }
                throw new ILException("Unable to do " +  "zgesdd"  + ".", e); 
            }
        }

#region HYCALPER AUTO GENERATED CODE


        [SecuritySafeCritical]
        public void  cgesdd(char jobz, int m, int n,  fcomplex[] a, int lda,  float[] s,  fcomplex[] u, int ldu, fcomplex[] vt, int ldvt, ref int info)
        {
            try {
                fcomplex [] work = new  fcomplex [1] { (  fcomplex )0.0 };
                float [] rwork; 
                int minMN = (m < n) ? m : n;
                if (jobz == 'N') {
                    rwork = new  float [minMN * 7];  
                } else {
                    rwork = new  float [5 * minMN * minMN + 5 * minMN];  
                }
                int lwork = -1;
                int[] iwork = new int[minMN * 8];
                mkl_cgesdd (ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, rwork, iwork, ref info);
                if (work[0] != 0) {
                    work = new  fcomplex [(int)work[0].real];
                    lwork = work.Length;
                    mkl_cgesdd (ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork,rwork, iwork, ref info);
                }
            } catch (Exception e) {
                if (e is OutOfMemoryException) {
                    cgesvd (jobz, m, n, a, lda, s, u, ldu, vt, ldvt, ref info); 
                }
                throw new ILException("Unable to do " +  "zgesdd"  + ".", e); 
            }
        }

#endregion HYCALPER AUTO GENERATED CODE


        

        /// <summary>
        /// singular value decomposition
        /// </summary>
        /// <param name="jobz"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="lda"></param>
        /// <param name="s"></param>
        /// <param name="u"></param>
        /// <param name="ldu"></param>
        /// <param name="vt"></param>
        /// <param name="ldvt"></param>
        /// <param name="info"></param>
        [SecuritySafeCritical]
        public void  dgesvd(char jobz, int m, int n,  double[] a, int lda,
                            double [] s,  double [] u, int ldu,  double [] vt, int ldvt, ref int info) {
            if (jobz != 'A' && jobz != 'S' && jobz != 'N')
                throw new ILArgumentException("Argument jobz must be one of 'A','S' or 'N'"); 
            try {
                 double [] work = new  double [1] { ( double )0.0 };
                int lwork = -1;
                int[] iwork = new int[((m < n) ? m : n) * 8];
                 mkl_dgesvd (ref jobz, ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, iwork, ref info);
                if (work[0] != 0) {
                    work = new  double [(int)work[0]];
                    lwork = work.Length; 
                     mkl_dgesvd (ref jobz, ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, iwork, ref info);
                }
            } catch (Exception e) {
                if (e is OutOfMemoryException) {
                    throw new ILMemoryException("Not enough memory for given arguments."); 
                }
                throw new ILException("Unable to do gesvd.", e);
            }
        }

#region HYCALPER AUTO GENERATED CODE

       

        /// <summary>
        /// singular value decomposition
        /// </summary>
        /// <param name="jobz"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="lda"></param>
        /// <param name="s"></param>
        /// <param name="u"></param>
        /// <param name="ldu"></param>
        /// <param name="vt"></param>
        /// <param name="ldvt"></param>
        /// <param name="info"></param>
        [SecuritySafeCritical]
        public void  sgesvd(char jobz, int m, int n,  float[] a, int lda,
                           float [] s,  float [] u, int ldu,  float [] vt, int ldvt, ref int info) {
            if (jobz != 'A' && jobz != 'S' && jobz != 'N')
                throw new ILArgumentException("Argument jobz must be one of 'A','S' or 'N'"); 
            try {
                float [] work = new  float [1] { ( float )0.0 };
                int lwork = -1;
                int[] iwork = new int[((m < n) ? m : n) * 8];
                mkl_sgesvd (ref jobz, ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, iwork, ref info);
                if (work[0] != 0) {
                    work = new  float [(int)work[0]];
                    lwork = work.Length; 
                    mkl_sgesvd (ref jobz, ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, iwork, ref info);
                }
            } catch (Exception e) {
                if (e is OutOfMemoryException) {
                    throw new ILMemoryException("Not enough memory for given arguments."); 
                }
                throw new ILException("Unable to do gesvd.", e);
            }
        }
       

        /// <summary>
        /// singular value decomposition
        /// </summary>
        /// <param name="jobz"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="lda"></param>
        /// <param name="s"></param>
        /// <param name="u"></param>
        /// <param name="ldu"></param>
        /// <param name="vt"></param>
        /// <param name="ldvt"></param>
        /// <param name="info"></param>
        [SecuritySafeCritical]
        public void  zgesvd(char jobz, int m, int n,  complex[] a, int lda,
                           double [] s,  complex [] u, int ldu,  complex [] vt, int ldvt, ref int info) {
            if (jobz != 'A' && jobz != 'S' && jobz != 'N')
                throw new ILArgumentException("Argument jobz must be one of 'A','S' or 'N'"); 
            try {
                complex [] work = new  complex [1] { ( complex )0.0 };
                int lwork = -1;
                int[] iwork = new int[((m < n) ? m : n) * 8];
                mkl_zgesvd (ref jobz, ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, iwork, ref info);
                if (work[0] != 0) {
                    work = new  complex [(int)work[0]];
                    lwork = work.Length; 
                    mkl_zgesvd (ref jobz, ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, iwork, ref info);
                }
            } catch (Exception e) {
                if (e is OutOfMemoryException) {
                    throw new ILMemoryException("Not enough memory for given arguments."); 
                }
                throw new ILException("Unable to do gesvd.", e);
            }
        }
       

        /// <summary>
        /// singular value decomposition
        /// </summary>
        /// <param name="jobz"></param>
        /// <param name="m"></param>
        /// <param name="n"></param>
        /// <param name="a"></param>
        /// <param name="lda"></param>
        /// <param name="s"></param>
        /// <param name="u"></param>
        /// <param name="ldu"></param>
        /// <param name="vt"></param>
        /// <param name="ldvt"></param>
        /// <param name="info"></param>
        [SecuritySafeCritical]
        public void  cgesvd(char jobz, int m, int n,  fcomplex[] a, int lda,
                           float [] s,  fcomplex [] u, int ldu,  fcomplex [] vt, int ldvt, ref int info) {
            if (jobz != 'A' && jobz != 'S' && jobz != 'N')
                throw new ILArgumentException("Argument jobz must be one of 'A','S' or 'N'"); 
            try {
                fcomplex [] work = new  fcomplex [1] { ( fcomplex )0.0 };
                int lwork = -1;
                int[] iwork = new int[((m < n) ? m : n) * 8];
                mkl_cgesvd (ref jobz, ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, iwork, ref info);
                if (work[0] != 0) {
                    work = new  fcomplex [(int)work[0]];
                    lwork = work.Length; 
                    mkl_cgesvd (ref jobz, ref jobz, ref m, ref n, a, ref lda, s, u, ref ldu, vt, ref ldvt, work, ref lwork, iwork, ref info);
                }
            } catch (Exception e) {
                if (e is OutOfMemoryException) {
                    throw new ILMemoryException("Not enough memory for given arguments."); 
                }
                throw new ILException("Unable to do gesvd.", e);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        [SecuritySafeCritical]
        public void dpotrf(char uplo, int n, double[] A, int lda, ref int info) {
            mkl_dpotrf(ref uplo, ref n, A, ref lda, ref info);
        }
        [SecuritySafeCritical]
        public void spotrf(char uplo, int n, float[] A, int lda, ref int info) {
            mkl_spotrf(ref uplo, ref n, A, ref lda, ref info);
        }
        [SecuritySafeCritical]
        public void cpotrf(char uplo, int n, fcomplex[] A, int lda, ref int info) {
            mkl_cpotrf(ref uplo, ref n, A, ref lda, ref info);
        }
        [SecuritySafeCritical]
        public void zpotrf(char uplo, int n, complex[] A, int lda, ref int info) {
            mkl_zpotrf(ref uplo, ref n, A, ref lda, ref info);
        }


        [SecuritySafeCritical]
        public void dpotri(char uplo, int n, double[] A, int lda, ref int info) {
            mkl_dpotri(ref uplo, ref n, A, ref lda, ref info);
        }
        [SecuritySafeCritical]
        public void spotri(char uplo, int n, float[] A, int lda, ref int info) {
            mkl_spotri(ref uplo, ref n, A, ref lda, ref info);
        }
        [SecuritySafeCritical]
        public void cpotri(char uplo, int n, fcomplex[] A, int lda, ref int info) {
            mkl_cpotri(ref uplo, ref n, A, ref lda, ref info);
        }
        [SecuritySafeCritical]
        public void zpotri(char uplo, int n, complex[] A, int lda, ref int info) {
            mkl_zpotri(ref uplo, ref n, A, ref lda, ref info);
        }


        [SecuritySafeCritical]
        public void dgetrf(int M, int N, double[] A, int LDA, int[] IPIV, ref int info) {
            mkl_dgetrf(ref M, ref N, A, ref LDA, IPIV, ref info);
        }
        [SecuritySafeCritical]
        public void sgetrf(int M, int N, float[] A, int LDA, int[] IPIV, ref int info) {
            mkl_sgetrf(ref M, ref N, A, ref LDA, IPIV, ref info);
        }
        [SecuritySafeCritical]
        public void cgetrf(int M, int N, fcomplex[] A, int LDA, int[] IPIV, ref int info) {
            mkl_cgetrf(ref M, ref N, A, ref LDA, IPIV, ref info);
        }
        [SecuritySafeCritical]
        public void zgetrf(int M, int N, complex[] A, int LDA, int[] IPIV, ref int info) {
            mkl_zgetrf(ref M, ref N, A, ref LDA, IPIV, ref info);
        }


        
        [SecuritySafeCritical]
        public void  dgetri(int N,  double[] A, int LDA, int[] IPIV, ref int info) {
             double [] work = new  double [1];  
            int lwork = -1; 
            try {
                 mkl_dgetri (ref N, A, ref LDA, IPIV, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = new  double [lwork]; 
                     mkl_dgetri (ref N, A, ref LDA, IPIV, work, ref lwork, ref info);
                } else {
                    throw new ILException("error in mkl_dgetri"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on dgetri. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }

#region HYCALPER AUTO GENERATED CODE

       
        [SecuritySafeCritical]
        public void  sgetri(int N,  float[] A, int LDA, int[] IPIV, ref int info) {
            float [] work = new  float [1];  
            int lwork = -1; 
            try {
                mkl_sgetri (ref N, A, ref LDA, IPIV, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = new  float [lwork]; 
                    mkl_sgetri (ref N, A, ref LDA, IPIV, work, ref lwork, ref info);
                } else {
                    throw new ILException("error in mkl_dgetri"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on dgetri. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }
       
        [SecuritySafeCritical]
        public void  zgetri(int N,  complex[] A, int LDA, int[] IPIV, ref int info) {
            complex [] work = new  complex [1];  
            int lwork = -1; 
            try {
                mkl_zgetri (ref N, A, ref LDA, IPIV, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = new  complex [lwork]; 
                    mkl_zgetri (ref N, A, ref LDA, IPIV, work, ref lwork, ref info);
                } else {
                    throw new ILException("error in mkl_dgetri"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on dgetri. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }
       
        [SecuritySafeCritical]
        public void  cgetri(int N,  fcomplex[] A, int LDA, int[] IPIV, ref int info) {
            fcomplex [] work = new  fcomplex [1];  
            int lwork = -1; 
            try {
                mkl_cgetri (ref N, A, ref LDA, IPIV, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = new  fcomplex [lwork]; 
                    mkl_cgetri (ref N, A, ref LDA, IPIV, work, ref lwork, ref info);
                } else {
                    throw new ILException("error in mkl_dgetri"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on dgetri. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }

#endregion HYCALPER AUTO GENERATED CODE


        
        [SecuritySafeCritical]
        public void  dgeqrf(int M, int N,  double[] A, int lda,  double[] tau, ref int info) {
             double [] work = new  double [1];  
            int lwork = -1; 
            try {
                 mkl_dgeqrf (ref M, ref N, A, ref lda, tau, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = new  double [lwork]; 
                     mkl_dgeqrf (ref M, ref N, A, ref lda, tau, work, ref lwork, ref info);
                } else {
                    throw new ILException("error in mkl_?geqrf"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on ?geqrf. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }

#region HYCALPER AUTO GENERATED CODE

       
        [SecuritySafeCritical]
        public void  sgeqrf(int M, int N,  float[] A, int lda,  float[] tau, ref int info) {
            float [] work = new  float [1];  
            int lwork = -1; 
            try {
                mkl_sgeqrf (ref M, ref N, A, ref lda, tau, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = new  float [lwork]; 
                    mkl_sgeqrf (ref M, ref N, A, ref lda, tau, work, ref lwork, ref info);
                } else {
                    throw new ILException("error in mkl_?geqrf"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on ?geqrf. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }
       
        [SecuritySafeCritical]
        public void  zgeqrf(int M, int N,  complex[] A, int lda,  complex[] tau, ref int info) {
            complex [] work = new  complex [1];  
            int lwork = -1; 
            try {
                mkl_zgeqrf (ref M, ref N, A, ref lda, tau, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = new  complex [lwork]; 
                    mkl_zgeqrf (ref M, ref N, A, ref lda, tau, work, ref lwork, ref info);
                } else {
                    throw new ILException("error in mkl_?geqrf"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on ?geqrf. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }
       
        [SecuritySafeCritical]
        public void  cgeqrf(int M, int N,  fcomplex[] A, int lda,  fcomplex[] tau, ref int info) {
            fcomplex [] work = new  fcomplex [1];  
            int lwork = -1; 
            try {
                mkl_cgeqrf (ref M, ref N, A, ref lda, tau, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = new  fcomplex [lwork]; 
                    mkl_cgeqrf (ref M, ref N, A, ref lda, tau, work, ref lwork, ref info);
                } else {
                    throw new ILException("error in mkl_?geqrf"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on ?geqrf. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        [SecuritySafeCritical]
        public void dormqr(char side, char trans, int m, int n, int k, double[] A, int lda, double[] tau, double[] C, int LDC, ref int info) {
            throw new Exception("The method or operation is not implemented.");
        }
        [SecuritySafeCritical]
        public void sormqr(char side, char trans, int m, int n, int k, float[] A, int lda, float[] tau, float[] C, int LDC, ref int info) {
            throw new Exception("The method or operation is not implemented.");
        }


        
        [SecuritySafeCritical]
        public void  dorgqr(int M, int N, int K,  double[] A, int lda,  double[] tau, ref int info) {
             double [] work = new  double [1];  
            int lwork = -1; 
            try {
                /*!HC:mkl_***gqr*/ mkl_dorgqr (ref M, ref N, ref K, A, ref lda, tau, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = new  double [lwork]; 
                    /*!HC:mkl_***gqr*/ mkl_dorgqr (ref M, ref N, ref K, A, ref lda, tau, work, ref lwork, ref info);
                } else {
                    throw new ILException("error in mkl_?[un/or]gqr"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on ?[un/or]gqr. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }

#region HYCALPER AUTO GENERATED CODE

       
        [SecuritySafeCritical]
        public void  sorgqr(int M, int N, int K,  float[] A, int lda,  float[] tau, ref int info) {
            float [] work = new  float [1];  
            int lwork = -1; 
            try {
                mkl_sorgqr (ref M, ref N, ref K, A, ref lda, tau, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = new  float [lwork]; 
                    mkl_sorgqr (ref M, ref N, ref K, A, ref lda, tau, work, ref lwork, ref info);
                } else {
                    throw new ILException("error in mkl_?[un/or]gqr"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on ?[un/or]gqr. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }
       
        [SecuritySafeCritical]
        public void  zungqr(int M, int N, int K,  complex[] A, int lda,  complex[] tau, ref int info) {
            complex [] work = new  complex [1];  
            int lwork = -1; 
            try {
                mkl_zungqr (ref M, ref N, ref K, A, ref lda, tau, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = new  complex [lwork]; 
                    mkl_zungqr (ref M, ref N, ref K, A, ref lda, tau, work, ref lwork, ref info);
                } else {
                    throw new ILException("error in mkl_?[un/or]gqr"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on ?[un/or]gqr. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }
       
        [SecuritySafeCritical]
        public void  cungqr(int M, int N, int K,  fcomplex[] A, int lda,  fcomplex[] tau, ref int info) {
            fcomplex [] work = new  fcomplex [1];  
            int lwork = -1; 
            try {
                mkl_cungqr (ref M, ref N, ref K, A, ref lda, tau, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = new  fcomplex [lwork]; 
                    mkl_cungqr (ref M, ref N, ref K, A, ref lda, tau, work, ref lwork, ref info);
                } else {
                    throw new ILException("error in mkl_?[un/or]gqr"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on ?[un/or]gqr. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }

#endregion HYCALPER AUTO GENERATED CODE


        
        [SecuritySafeCritical]
        public void  dgeqp3(int M, int N, double[] A, int LDA, int[] JPVT, double[] tau, ref int info) {
             double [] work = new  double [1];
            int lwork = -1; 
            try {
                 
                /*dummy*/
                 
                mkl_dgeqp3 (ref M, ref N, A, ref LDA, JPVT, tau, work, ref lwork, ref info); 
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = ILMemoryPool.Pool.New<  double >(lwork); 
                     
                    mkl_dgeqp3 (ref M, ref N, A, ref LDA, JPVT, tau, work, ref lwork, ref info);
                    ILMemoryPool.Pool.Free(work); 
                } else {
                    throw new ILException("error in mkl_?geqp3"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on ?geqp3. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }

#region HYCALPER AUTO GENERATED CODE

       
        [SecuritySafeCritical]
        public void  sgeqp3(int M, int N, float[] A, int LDA, int[] JPVT, float[] tau, ref int info) {
            float [] work = new  float [1];
            int lwork = -1; 
            try {
                
                mkl_sgeqp3 (ref M, ref N, A, ref LDA, JPVT, tau, work, ref lwork, ref info);
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = ILMemoryPool.Pool.New<  float >(lwork); 
                    mkl_sgeqp3 (ref M, ref N, A, ref LDA, JPVT, tau, work, ref lwork, ref info);
                    ILMemoryPool.Pool.Free(work); 
                } else {
                    throw new ILException("error in mkl_?geqp3"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on ?geqp3. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }
       
        [SecuritySafeCritical]
        public void  zgeqp3(int M, int N, complex[] A, int LDA, int[] JPVT, complex[] tau, ref int info) {
            complex [] work = new  complex [1];
            int lwork = -1; 
            try {
                double[] rwork = new double[2 * N];
                mkl_zgeqp3 (ref M, ref N, A, ref LDA, JPVT, tau, work, ref lwork, rwork, ref info);
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = ILMemoryPool.Pool.New<  complex >(lwork); 
                    mkl_zgeqp3 (ref M, ref N, A, ref LDA, JPVT, tau, work, ref lwork, rwork, ref info);
                    ILMemoryPool.Pool.Free(work); 
                } else {
                    throw new ILException("error in mkl_?geqp3"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on ?geqp3. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }
       
        [SecuritySafeCritical]
        public void  cgeqp3(int M, int N, fcomplex[] A, int LDA, int[] JPVT, fcomplex[] tau, ref int info) {
            fcomplex [] work = new  fcomplex [1];
            int lwork = -1; 
            try {
                float[] rwork = new float[2 * N];
                mkl_cgeqp3 (ref M, ref N, A, ref LDA, JPVT, tau, work, ref lwork, rwork, ref info);
                lwork = (int)work[0]; 
                if (lwork > 0 && info == 0) {
                    work = ILMemoryPool.Pool.New<  fcomplex >(lwork); 
                    mkl_cgeqp3 (ref M, ref N, A, ref LDA, JPVT, tau, work, ref lwork, rwork, ref info);
                    ILMemoryPool.Pool.Free(work); 
                } else {
                    throw new ILException("error in mkl_?geqp3"); 
                }
            } catch (OutOfMemoryException e) {
                throw new ILException("error on ?geqp3. Not enough memory! " + (lwork * Marshal.SizeOf( work[0] )).ToString() + " bytes has been requested.",e); 
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        [SecuritySafeCritical]
        public void dtrtrs(char uplo, char transA, char diag, int N, int nrhs, IntPtr A, int LDA, IntPtr B, int LDB, ref int info) {
            mkl_dtrtrs(ref uplo, ref transA, ref diag, ref N, ref nrhs, A, ref LDA, B, ref LDB, ref info); 
        }
        [SecuritySafeCritical]
        public void strtrs(char uplo, char transA, char diag, int N, int nrhs, IntPtr A, int LDA, IntPtr B, int LDB, ref int info) {
            mkl_strtrs(ref uplo, ref transA, ref diag, ref N, ref nrhs, A, ref LDA, B, ref LDB, ref info); 
        }
        [SecuritySafeCritical]
        public void ctrtrs(char uplo, char transA, char diag, int N, int nrhs, IntPtr A, int LDA, IntPtr B, int LDB, ref int info) {
            mkl_ctrtrs(ref uplo, ref transA, ref diag, ref N, ref nrhs, A, ref LDA, B, ref LDB, ref info); 
        }
        [SecuritySafeCritical]
        public void ztrtrs(char uplo, char transA, char diag, int N, int nrhs, IntPtr A, int LDA, IntPtr B, int LDB, ref int info) {
            mkl_ztrtrs(ref uplo, ref transA, ref diag, ref N, ref nrhs, A, ref LDA, B, ref LDB, ref info); 
        }

        [SecuritySafeCritical]
        public void dgetrs(char trans, int N, int NRHS, double[] A, int LDA, int[] IPIV, double[] B, int LDB, ref int info) {
            mkl_dgetrs(ref trans,ref N,ref NRHS, A, ref LDA, IPIV, B, ref LDB, ref info);     
        }

        [SecuritySafeCritical]
        public void sgetrs(char trans, int N, int NRHS, float[] A, int LDA, int[] IPIV, float[] B, int LDB, ref int info) {
            mkl_sgetrs(ref trans,ref N,ref NRHS, A, ref LDA, IPIV, B, ref LDB, ref info);     
        }

        [SecuritySafeCritical]
        public void cgetrs(char trans, int N, int NRHS, fcomplex[] A, int LDA, int[] IPIV, fcomplex[] B, int LDB, ref int info) {
            mkl_cgetrs(ref trans,ref N,ref NRHS, A, ref LDA, IPIV, B, ref LDB, ref info);     
        }

        [SecuritySafeCritical]
        public void zgetrs(char trans, int N, int NRHS, complex[] A, int LDA, int[] IPIV, complex[] B, int LDB, ref int info) {
            mkl_zgetrs(ref trans,ref N,ref NRHS, A, ref LDA, IPIV, B, ref LDB, ref info);
        }

        [SecuritySafeCritical]
        public void dpotrs(char uplo, int n, int nrhs, double[] A, int lda, double[] B, int ldb, ref int info) {
            mkl_dpotrs(ref uplo, ref n, ref nrhs, A, ref lda, B, ref ldb, ref info); 
        }

        [SecuritySafeCritical]
        public void spotrs(char uplo, int n, int nrhs, float[] A, int lda, float[] B, int ldb, ref int info) {
            mkl_spotrs(ref uplo, ref n, ref nrhs, A, ref lda, B, ref ldb, ref info); 
        }

        [SecuritySafeCritical]
        public void cpotrs(char uplo, int n, int nrhs, fcomplex[] A, int lda, fcomplex[] B, int ldb, ref int info) {
            mkl_cpotrs(ref uplo, ref n, ref nrhs, A, ref lda, B, ref ldb, ref info); 
        }

        [SecuritySafeCritical]
        public void zpotrs(char uplo, int n, int nrhs, complex[] A, int lda, complex[] B, int ldb, ref int info) {
            mkl_zpotrs(ref uplo, ref n, ref nrhs, A, ref lda, B, ref ldb, ref info); 
        }


        
        [SecuritySafeCritical]
        public void  dgelsd(int m, int n, int nrhs,  double[] A, int lda,  double[] B, int ldb,  double[] S,  double RCond, ref int rank, ref int info) {
             double [] work = new  double [1]; 
            int [] iwork = new int[1]; 
            int lwork = -1; 
            /*HC:HycalpTag1*/
            mkl_dgelsd (ref m, ref n, ref nrhs, A, ref lda, B, ref ldb, S,ref RCond, ref rank, work, ref lwork, iwork, ref info);
            if (info != 0) 
                throw new ILArgumentException("dgelsd: invalid parameter: #" + (-info).ToString());
            lwork = (int)work[0]; //ILAENV(9, "dgelsd", " ",0,0,0,0); 
            if (lwork <= 0)
                throw new ILArgumentException("dgelsd: unknown error determining working size lwork");
            iwork = new int[lwork * 1000];
            
            work = ILMemoryPool.Pool.New<  double >(lwork); 
            /*HC:HycalpTag2*/
            mkl_dgelsd (ref m, ref n, ref nrhs, A, ref lda, B, ref ldb, S,ref RCond, ref rank, work, ref lwork, iwork, ref info);
            ILMemoryPool.Pool.Free(work);
        }

#region HYCALPER AUTO GENERATED CODE

       
        [SecuritySafeCritical]
        public void  sgelsd (int m, int n, int nrhs,  float[] A, int lda,  float[] B, int ldb,  float[] S,  float RCond, ref int rank, ref int info) {
            float [] work = new  float [1]; 
            int [] iwork = new int[1]; 
            int lwork = -1; 
            mkl_sgelsd (ref m, ref n, ref nrhs, A, ref lda, B, ref ldb, S,ref RCond, ref rank, work, ref lwork, iwork, ref info);
            if (info != 0) 
                throw new ILArgumentException("dgelsd: invalid parameter: #" + (-info).ToString());
            lwork = (int)work[0]; //ILAENV(9, "dgelsd", " ",0,0,0,0); 
            if (lwork <= 0)
                throw new ILArgumentException("dgelsd: unknown error determining working size lwork");
            iwork = new int[lwork * 1000];
            
            work = ILMemoryPool.Pool.New<  float >(lwork); 
            mkl_sgelsd (ref m, ref n, ref nrhs, A, ref lda, B, ref ldb, S,ref RCond, ref rank, work, ref lwork, iwork, ref info);
            ILMemoryPool.Pool.Free(work);
        }
       
        [SecuritySafeCritical]
        public void  zgelsd (int m, int n, int nrhs,  complex[] A, int lda,  complex[] B, int ldb,  double[] S,  double RCond, ref int rank, ref int info) {
            complex [] work = new  complex [1]; 
            int [] iwork = new int[1]; 
            int lwork = -1; 
            double [] rwork = new double [1];  mkl_zgelsd (ref m, ref n, ref nrhs, A, ref lda, B, ref ldb, S,ref RCond, ref rank, work, ref lwork, rwork, iwork, ref info);
            if (info != 0) 
                throw new ILArgumentException("dgelsd: invalid parameter: #" + (-info).ToString());
            lwork = (int)work[0]; //ILAENV(9, "dgelsd", " ",0,0,0,0); 
            if (lwork <= 0)
                throw new ILArgumentException("dgelsd: unknown error determining working size lwork");
            iwork = new int[lwork * 1000];
            
            work = ILMemoryPool.Pool.New<  complex >(lwork); 
            rwork = new double [(int)rwork[0]];  mkl_zgelsd (ref m, ref n, ref nrhs, A, ref lda, B, ref ldb, S,ref RCond, ref rank, work, ref lwork, rwork, iwork, ref info);
            ILMemoryPool.Pool.Free(work);
        }
       
        [SecuritySafeCritical]
        public void  cgelsd (int m, int n, int nrhs,  fcomplex[] A, int lda,  fcomplex[] B, int ldb,  float[] S,  float RCond, ref int rank, ref int info) {
            fcomplex [] work = new  fcomplex [1]; 
            int [] iwork = new int[1]; 
            int lwork = -1; 
            float [] rwork = new float [1];  mkl_cgelsd (ref m, ref n, ref nrhs, A, ref lda, B, ref ldb, S,ref RCond, ref rank, work, ref lwork, rwork, iwork, ref info);
            if (info != 0) 
                throw new ILArgumentException("dgelsd: invalid parameter: #" + (-info).ToString());
            lwork = (int)work[0]; //ILAENV(9, "dgelsd", " ",0,0,0,0); 
            if (lwork <= 0)
                throw new ILArgumentException("dgelsd: unknown error determining working size lwork");
            iwork = new int[lwork * 1000];
            
            work = ILMemoryPool.Pool.New<  fcomplex >(lwork); 
            rwork = new float [(int)rwork[0]];  mkl_cgelsd (ref m, ref n, ref nrhs, A, ref lda, B, ref ldb, S,ref RCond, ref rank, work, ref lwork, rwork, iwork, ref info);
            ILMemoryPool.Pool.Free(work);
        }

#endregion HYCALPER AUTO GENERATED CODE

        #region ?gelsy  
        [SecuritySafeCritical]
        public void dgelsy(int m, int n, int nrhs, double[] A, int lda, double[] B, int ldb, int[] JPVT0, double RCond, ref int rank, ref int info) {
            int lwork = -1;
            double [] work = new double [1]; 
            mkl_dgelsy (ref m,ref n,ref nrhs,A,ref lda,B,ref ldb,JPVT0,ref RCond,ref rank,work,ref lwork,ref info);
            if (info != 0)
                throw new ILArgumentException("?gelsy: unable to determine optimal block size. cancelling...");
            lwork = (int) work[0];
            work = ILMemoryPool.Pool.New<double>(lwork);
            mkl_dgelsy (ref m,ref n,ref nrhs,A,ref lda,B,ref ldb,JPVT0,ref RCond,ref rank,work,ref lwork,ref info);
            ILMemoryPool.Pool.Free(work); 
        }
        [SecuritySafeCritical]
        public void sgelsy(int m, int n, int nrhs, float[] A, int lda, float[] B, int ldb, int[] JPVT0, float RCond, ref int rank, ref int info) {
            int lwork = -1;
            float [] work = new  float [1]; 
            mkl_sgelsy (ref m,ref n,ref nrhs,A,ref lda,B,ref ldb,JPVT0,ref RCond,ref rank,work,ref lwork,ref info);
            if (info != 0)
                throw new ILArgumentException("?gelsy: unable to determine optimal block size. cancelling...");
            lwork = (int) work[0];
            work = ILMemoryPool.Pool.New<float>(lwork);
            mkl_sgelsy (ref m,ref n,ref nrhs,A,ref lda,B,ref ldb,JPVT0,ref RCond,ref rank,work,ref lwork,ref info);
            ILMemoryPool.Pool.Free(work); 
        }

        [SecuritySafeCritical]
        public void zgelsy(int m, int n, int nrhs, complex[] A, int lda, complex[] B, int ldb, int[] JPVT0, double RCond, ref int rank, ref int info) {
            int lwork = -1;
            complex [] work = new  complex [1]; 
            double[] rwork = new double[1];
            mkl_zgelsy (ref m,ref n,ref nrhs,A,ref lda,B,ref ldb,JPVT0,ref RCond,ref rank,work,ref lwork, rwork, ref info);
            if (info != 0)
                throw new ILArgumentException("?gelsy: unable to determine optimal block size. cancelling...");
            lwork = (int) work[0];
            work = ILMemoryPool.Pool.New<complex>(lwork);
            rwork = ILMemoryPool.Pool.New<double>(lwork);
            mkl_zgelsy (ref m,ref n,ref nrhs,A,ref lda,B,ref ldb,JPVT0,ref RCond,ref rank,work,ref lwork, rwork, ref info);
            ILMemoryPool.Pool.Free(work); 
            ILMemoryPool.Pool.Free(rwork); 
        }

        [SecuritySafeCritical]
        public void cgelsy(int m, int n, int nrhs, fcomplex[] A, int lda, fcomplex[] B, int ldb, int[] JPVT0, float RCond, ref int rank, ref int info) {
            int lwork = -1;
            fcomplex [] work = new  fcomplex [1]; 
            float[] rwork = new float[1];
            mkl_cgelsy (ref m,ref n,ref nrhs,A,ref lda,B,ref ldb,JPVT0,ref RCond,ref rank,work,ref lwork, rwork, ref info);
            if (info != 0)
                throw new ILArgumentException("?gelsy: unable to determine optimal block size. cancelling...");
            lwork = (int) work[0];
            work = ILMemoryPool.Pool.New<fcomplex>(lwork);
            rwork = ILMemoryPool.Pool.New<float>(lwork);
            mkl_cgelsy (ref m,ref n,ref nrhs,A,ref lda,B,ref ldb,JPVT0,ref RCond,ref rank,work,ref lwork, rwork, ref info);
            ILMemoryPool.Pool.Free(work); 
            ILMemoryPool.Pool.Free(rwork); 
        }
#endregion

        #region ?GEEVX
        [SecuritySafeCritical]
        public void dgeevx(char balance, char jobvl, char jobvr, char sense, int n, double[] A, int lda, double[] wr, double[] wi, double[] vl, int ldvl, double[] vr, int ldvr, ref int ilo, ref int ihi, double[] scale, ref double abnrm, double[] rconde, double[] rcondv, ref int info) {
            double [] work = new double[1]; 
            int lwork = -1;
            int [] iwork = ILMemoryPool.Pool.New<int>(2 * n - 2);
            mkl_dgeevx(ref balance, ref jobvl, ref jobvr, ref sense, ref n, A, ref lda, wr, wi, vl, ref ldvl, vr, ref ldvr, ref ilo, ref ihi, scale, ref abnrm, rconde, rcondv, work, ref lwork, iwork, ref info);
            if (info != 0) 
                throw new ILArgumentException("error in lapack call: ?geevx. (" + info + ")");
            lwork = (int)work[0]; 
            work = ILMemoryPool.Pool.New<double>(lwork); 
            mkl_dgeevx(ref balance, ref jobvl, ref jobvr, ref sense, ref n, A, ref lda, wr, wi, vl, ref ldvl, vr, ref ldvr, ref ilo, ref ihi, scale, ref abnrm, rconde, rcondv, work, ref lwork, iwork, ref info);
            ILMemoryPool.Pool.Free(work);
            ILMemoryPool.Pool.Free(iwork);
        }
        [SecuritySafeCritical]
        public void sgeevx(char balance, char jobvl, char jobvr, char sense, int n, float[] A, int lda, float[] wr, float[] wi, float[] vl, int ldvl, float[] vr, int ldvr, ref int ilo, ref int ihi, float[] scale, ref float abnrm, float[] rconde, float[] rcondv, ref int info) {
            float [] work = new float[1]; 
            int lwork = -1;
            int [] iwork = ILMemoryPool.Pool.New<int>(2 * n - 2);
            mkl_sgeevx(ref balance, ref jobvl, ref jobvr, ref sense, ref n, A, ref lda, wr, wi, vl, ref ldvl, vr, ref ldvr, ref ilo, ref ihi, scale, ref abnrm, rconde, rcondv, work, ref lwork, iwork, ref info);
            if (info != 0) 
                throw new ILArgumentException("error in lapack call: ?geevx. (" + info + ")");
            lwork = (int)work[0]; 
            work = ILMemoryPool.Pool.New<float>(lwork); 
            mkl_sgeevx(ref balance, ref jobvl, ref jobvr, ref sense, ref n, A, ref lda, wr, wi, vl, ref ldvl, vr, ref ldvr, ref ilo, ref ihi, scale, ref abnrm, rconde, rcondv, work, ref lwork, iwork, ref info);
            ILMemoryPool.Pool.Free(work);
            ILMemoryPool.Pool.Free(iwork);
        }
        [SecuritySafeCritical]
        public void cgeevx(char balance, char jobvl, char jobvr, char sense, int n, fcomplex[] A, int lda, fcomplex[] w, fcomplex[] vl, int ldvl, fcomplex[] vr, int ldvr, ref int ilo, ref int ihi, float[] scale, ref float abnrm, float[] rconde, float[] rcondv, ref int info) {
            fcomplex [] work = new fcomplex[1]; 
            int lwork = -1;
            float[] rwork = ILMemoryPool.Pool.New<float>(2 * n); 
            mkl_cgeevx(ref balance, ref jobvl, ref jobvr, ref sense, ref n, A, ref lda, w, vl, ref ldvl, vr, ref ldvr, ref ilo, ref ihi, scale, ref abnrm, rconde, rcondv, work, ref lwork, rwork, ref info);
            if (info != 0) 
                throw new ILArgumentException("error in lapack call: ?geevx. (" + info + ")");
            lwork = (int)work[0]; 
            work = ILMemoryPool.Pool.New<fcomplex>(lwork); 
            mkl_cgeevx(ref balance, ref jobvl, ref jobvr, ref sense, ref n, A, ref lda, w, vl, ref ldvl, vr, ref ldvr, ref ilo, ref ihi, scale, ref abnrm, rconde, rcondv, work, ref lwork, rwork, ref info);
            ILMemoryPool.Pool.Free(work);
            ILMemoryPool.Pool.Free(rwork);
        }
        [SecuritySafeCritical]
        public void zgeevx(char balance, char jobvl, char jobvr, char sense, int n, complex[] A, int lda, complex[] w, complex[] vl, int ldvl, complex[] vr, int ldvr, ref int ilo, ref int ihi, double[] scale, ref double abnrm, double[] rconde, double[] rcondv, ref int info) {
            complex [] work = new complex[1]; 
            int lwork = -1;
            double[] rwork = ILMemoryPool.Pool.New<double>(2 * n); 
            mkl_zgeevx(ref balance, ref jobvl, ref jobvr, ref sense, ref n, A, ref lda, w, vl, ref ldvl, vr, ref ldvr, ref ilo, ref ihi, scale, ref abnrm, rconde, rcondv, work, ref lwork, rwork, ref info);
            if (info != 0) 
                throw new ILArgumentException("error in lapack call: ?geevx. (" + info + ")");
            lwork = (int)work[0]; 
            work = ILMemoryPool.Pool.New<complex>(lwork); 
            mkl_zgeevx(ref balance, ref jobvl, ref jobvr, ref sense, ref n, A, ref lda, w, vl, ref ldvl, vr, ref ldvr, ref ilo, ref ihi, scale, ref abnrm, rconde, rcondv, work, ref lwork, rwork, ref info);
            ILMemoryPool.Pool.Free(work);
            ILMemoryPool.Pool.Free(rwork);
        }
        #endregion

        #region ?syevr
        [SecuritySafeCritical]
        public void dsyevr(char jobz, char range, char uplo, int n, double[] A, int lda, double vl, double vu, int il, int iu, double abstol, ref int m, double[] w, double[] z, int ldz, int[] isuppz, ref int info) {
            double [] work = new double[1]; 
            int lwork = -1, liwork = -1; 
            int [] iwork = new int[1]; 
            //byte jz = (byte)jobz,rn = (byte) range,ul = (byte)uplo; 
            mkl_dsyevr(ref jobz,ref range,ref uplo,ref n, A,ref lda, ref vl, ref vu, ref il, ref iu, ref abstol,ref m, w,z,ref ldz,isuppz, work,ref lwork,iwork,ref liwork,ref info);
            if (info != 0) {
                throw new ILArgumentException("?syevr: error returned from lapack: " + info);
            }
            lwork = (int)work[0]; 
            bool dummy; 
            work = ILMemoryPool.Pool.New<double>(lwork,true, out dummy);
            liwork = (int) iwork[0]; 
            iwork = ILMemoryPool.Pool.New<int>(liwork,true, out dummy);
            mkl_dsyevr(ref jobz,ref range,ref uplo,ref n, A,ref lda, ref vl, ref vu, ref il, ref iu, ref abstol,ref m, w,z,ref ldz,isuppz, work,ref lwork,iwork,ref liwork,ref info);
            ILMemoryPool.Pool.Free(iwork); 
            ILMemoryPool.Pool.Free(work); 
        }
        [SecuritySafeCritical]
        public void ssyevr(char jobz, char range, char uplo, int n, float[] A, int lda, float vl, float vu, int il, int iu, float abstol, ref int m, float[] w, float[] z, int ldz, int[] isuppz, ref int info) {
            float [] work = new float[1]; 
            int lwork = -1, liwork = -1; 
            int [] iwork = new int[1]; 
            mkl_ssyevr(ref jobz,ref range,ref uplo,ref n, A,ref lda, ref vl, ref vu, ref il, ref iu, ref abstol,ref m, w,z,ref ldz,isuppz, work,ref lwork,iwork,ref liwork,ref info);
            if (info != 0) {
                throw new ILArgumentException("?syevr: error returned from lapack: " + info);
            }
            lwork = (int)work[0]; 
            work = ILMemoryPool.Pool.New<float>(lwork);
            liwork = (int) iwork[0]; 
            iwork = ILMemoryPool.Pool.New<int>(liwork); 
            mkl_ssyevr(ref jobz,ref range,ref uplo,ref n, A,ref lda, ref vl, ref vu, ref il, ref iu, ref abstol,ref m, w,z,ref ldz,isuppz, work,ref lwork,iwork,ref liwork,ref info);
            ILMemoryPool.Pool.Free(iwork); 
            ILMemoryPool.Pool.Free(work); 
        }
        [SecuritySafeCritical]
        public void cheevr(char jobz, char range, char uplo, int n, fcomplex[] A, int lda, float vl, float vu, int il, int iu, float abstol, ref int m, float[] w, fcomplex[] z, int ldz, int[] isuppz, ref int info) {
            fcomplex[] work = new fcomplex[1]; 
            float [] rwork = new float[1]; 
            int [] iwork = new int[1]; 
            int lrwork = -1, liwork = -1, lwork = -1; 
            mkl_cheevr(ref jobz,ref range,ref uplo,ref n, A,ref lda, ref vl, ref vu, ref il, ref iu, ref abstol,ref m, w,z,ref ldz,isuppz, work,ref lwork,rwork,ref lrwork, iwork,ref liwork,ref info);
            if (info != 0) {
                throw new ILArgumentException("?syevr: error returned from lapack: " + info);
            }
            lrwork = (int)rwork[0]; 
            rwork = ILMemoryPool.Pool.New<float>(lrwork);
            lwork = (int) work[0]; 
            work = ILMemoryPool.Pool.New<fcomplex>(lwork); 
            liwork = (int) iwork[0]; 
            iwork = ILMemoryPool.Pool.New<int>(liwork); 
            mkl_cheevr(ref jobz,ref range,ref uplo,ref n, A,ref lda, ref vl, ref vu, ref il, ref iu, ref abstol,ref m, w,z,ref ldz,isuppz, work,ref lwork,rwork,ref lrwork,iwork,ref liwork,ref info);
            ILMemoryPool.Pool.Free(iwork); 
            ILMemoryPool.Pool.Free(work); 
            ILMemoryPool.Pool.Free(rwork); 
        }
        [SecuritySafeCritical]
        public void zheevr(char jobz, char range, char uplo, int n, complex[] A, int lda, double vl, double vu, int il, int iu, double abstol, ref int m, double[] w, complex[] z, int ldz, int[] isuppz, ref int info) {
            complex[] work = new complex[1]; 
            double [] rwork = new double[1]; 
            int [] iwork = new int[1]; 
            int lrwork = -1, liwork = -1, lwork = -1; 
            mkl_zheevr(ref jobz,ref range,ref uplo,ref n, A,ref lda, ref vl, ref vu, ref il, ref iu, ref abstol,ref m, w,z,ref ldz,isuppz, work,ref lwork,rwork,ref lrwork, iwork,ref liwork,ref info);
            if (info != 0) {
                throw new ILArgumentException("?syevr: error returned from lapack: " + info);
            }
            lrwork = (int)rwork[0]; 
            rwork = ILMemoryPool.Pool.New<double>(lrwork);
            lwork = (int) work[0]; 
            work = ILMemoryPool.Pool.New<complex>(lwork); 
            liwork = (int) iwork[0]; 
            iwork = ILMemoryPool.Pool.New<int>(liwork); 
            mkl_zheevr(ref jobz,ref range,ref uplo,ref n, A,ref lda, ref vl, ref vu, ref il, ref iu, ref abstol,ref m, w,z,ref ldz,isuppz, work,ref lwork,rwork,ref lrwork,iwork,ref liwork,ref info);
            ILMemoryPool.Pool.Free(iwork); 
            ILMemoryPool.Pool.Free(work); 
            ILMemoryPool.Pool.Free(rwork); 
        }
        #endregion 

        #region ?[he/sy]gv - generalized eigenproblem
        [SecuritySafeCritical]
        public void dsygv(int itype, char jobz, char uplo, int n, double[] A, int lda, double[] B, int ldb, double[] w, ref int info) {
            // query workspace 
            int lwork = -1; 
            double [] work = new double[1] {0.0}; 
            mkl_dsygv (ref itype,ref jobz,ref uplo,ref n, A,ref lda, B,ref ldb, w, work, ref lwork, ref info); 
            if (info != 0 || work[0] <= 0.0) return; 
            // create temporary array(s)
            lwork = (int) work[0]; 
            work = ILMemoryPool.Pool.New<double>(lwork); 
            mkl_dsygv (ref itype,ref jobz,ref uplo,ref n, A,ref lda, B,ref ldb, w, work, ref lwork, ref info); 
            ILMemoryPool.Pool.Free(work); 
        }
        [SecuritySafeCritical]
        public void ssygv(int itype, char jobz, char uplo, int n, float[] A, int lda, float[] B, int ldb, float[] w, ref int info) {
            // query workspace 
            int lwork = -1; 
            float [] work = new float[1] {0.0f}; 
            mkl_ssygv (ref itype,ref jobz,ref uplo,ref n, A,ref lda, B,ref ldb, w, work, ref lwork, ref info); 
            if (info != 0 || work[0] <= 0.0) return; 
            // create temporary array(s)
            lwork = (int) work[0]; 
            work = ILMemoryPool.Pool.New<float>(lwork); 
            mkl_ssygv (ref itype,ref jobz,ref uplo,ref n, A,ref lda, B,ref ldb, w, work, ref lwork, ref info); 
            ILMemoryPool.Pool.Free(work); 
        }
        [SecuritySafeCritical]
        public void chegv(int itype, char jobz, char uplo, int n, fcomplex[] A, int lda, fcomplex[] B, int ldb, float[] w, ref int info) {
            // query workspace 
            int lwork = -1; 
            fcomplex [] work = new fcomplex[1] {0.0f}; 
            float [] rwork = ILMemoryPool.Pool.New<float>(Math.Max(1,3*n-2)); 
            mkl_chegv(ref itype,ref  jobz,ref  uplo,ref  n, A,ref  lda, B,ref  ldb, w, work, ref lwork, rwork, ref info); 
            if (info != 0 || work[0] <= 0.0) return; 
            // create temporary array(s)
            lwork = (int) work[0]; 
            work = ILMemoryPool.Pool.New<fcomplex>(lwork); 
            mkl_chegv (ref itype,ref  jobz,ref  uplo,ref  n, A,ref  lda, B,ref  ldb, w, work, ref lwork, rwork, ref info); 
            ILMemoryPool.Pool.Free(rwork); 
            ILMemoryPool.Pool.Free(work); 
        }
        [SecuritySafeCritical]
        public void zhegv(int itype, char jobz, char uplo, int n, complex[] A, int lda, complex[] B, int ldb, double[] w, ref int info) { 
            // query workspace 
            int lwork = -1; 
            complex [] work = new complex[1] {0.0f}; 
            double [] rwork = ILMemoryPool.Pool.New<double>(Math.Max(1,3*n-2)); 
            mkl_zhegv(ref itype,ref  jobz,ref  uplo,ref  n, A,ref  lda, B,ref  ldb, w, work, ref lwork, rwork, ref info); 
            if (info != 0 || work[0] <= 0.0) return; 
            // create temporary array(s)
            lwork = (int) work[0]; 
            work = ILMemoryPool.Pool.New<complex>(lwork); 
            mkl_zhegv (ref itype,ref  jobz,ref  uplo,ref  n, A,ref  lda, B,ref  ldb, w, work, ref lwork, rwork, ref info); 
            ILMemoryPool.Pool.Free(rwork); 
            ILMemoryPool.Pool.Free(work); 
        }
        #endregion 

        #endregion
    }
}
