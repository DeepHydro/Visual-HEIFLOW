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


#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;

namespace ILNumerics.Native {
    /// <summary>
    /// Interface to all LAPACK/BLAS functions available
    /// </summary>
    /// <remarks>Each native module must implement this interface explicitly. Calls 
    /// to native functions are made virtual by calling functions of this interface.
    /// Therefore the user can transparently call any function regardless of the 
    /// plattform the assymbly (currently) runs on. The native modules implementing
    /// this interface take care of the details of implementation. 
    /// <para>Usually users of the library will not have to handle with this interface. 
    /// Its functions will be used from inside built in functions and are therefore wrapped 
    /// (mainly from inside <see cref="ILNumerics.ILMath">ILNumerics.ILMath</see>).</para>
    /// <para>Every LAPACK/BLAS function is explicitly implemented for any type supported.
    /// e.g. IILLapack includes four functions doing general matrix multiply: dgemm, zgemm, cgemm and sgemm - 
    /// for all four floating point datatypes supported from the LAPACK package.</para>
    /// <para>LAPACK is an open source linear algebra functions package optimized for 
    /// use together with highly natively optimized BLAS functions. A LAPACK guide is 
    /// available in the internet: <see href="http://www.netlib.org/lapack">www.netlib.org</see>.</para>
    /// </remarks>
    public interface IILLapack {
        
        #region ?GEMM

        /// <summary>
        /// Wrapper implementiation for ATLAS GeneralMatrixMultiply
        /// </summary>
        /// <param name="TransA">Transposition state for matrix A: one of the constants in enum CBlas_Transpose</param>
        /// <param name="TransB">Transposition state for matrix B: one of the constants in enum CBlas_Transpose</param>
        /// <param name="M">Number of rows in A</param>
        /// <param name="N">Number of columns in B</param>
        /// <param name="K">Number of columns in A and number of rows in B</param>
        /// <param name="alpha">multiplicationi factor for A</param>
        /// <param name="A">pointer to array A</param>
        /// <param name="lda">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix A</param>
        /// <param name="B">pointer to array B</param>
        /// <param name="ldb">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix B</param>
        /// <param name="beta">multiplication faktor for matrix B</param>
        /// <param name="C">pointer to predefined array C of neccessary length</param>
        /// <param name="ldc">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix C</param>
        /// <remarks>All parameters except C are readonly. Only elements of matrix C will be altered. C must be a predefined 
        /// continous array of size MxN</remarks>
        void  dgemm (char TransA, char TransB, int M, int N, int K,
             double alpha,  IntPtr A, int lda,
            IntPtr B, int ldb,
             double beta,  double [] C, int ldc);

        /// <summary>
        /// Wrapper implementiation for ATLAS GeneralMatrixMultiply
        /// </summary>
        /// <param name="TransA">Transposition state for matrix A: one of the constants in enum CBlas_Transpose</param>
        /// <param name="TransB">Transposition state for matrix B: one of the constants in enum CBlas_Transpose</param>
        /// <param name="M">Number of rows in A</param>
        /// <param name="N">Number of columns in B</param>
        /// <param name="K">Number of columns in A and number of rows in B</param>
        /// <param name="alpha">multiplicationi factor for A</param>
        /// <param name="A">pointer to array A</param>
        /// <param name="lda">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix A</param>
        /// <param name="B">pointer to array B</param>
        /// <param name="ldb">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix B</param>
        /// <param name="beta">multiplication faktor for matrix B</param>
        /// <param name="C">pointer to predefined array C of neccessary length</param>
        /// <param name="ldc">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix C</param>
        /// <remarks>All parameters except C are readonly. Only elements of matrix C will be altered. C must be a predefined 
        /// continous array of size MxN</remarks>
        void  sgemm (char TransA, char TransB, int M, int N, int K,
            float alpha,  IntPtr A, int lda,
            IntPtr B, int ldb,
            float beta,  float [] C, int ldc);
       

        /// <summary>
        /// Wrapper implementiation for ATLAS GeneralMatrixMultiply
        /// </summary>
        /// <param name="TransA">Transposition state for matrix A: one of the constants in enum CBlas_Transpose</param>
        /// <param name="TransB">Transposition state for matrix B: one of the constants in enum CBlas_Transpose</param>
        /// <param name="M">Number of rows in A</param>
        /// <param name="N">Number of columns in B</param>
        /// <param name="K">Number of columns in A and number of rows in B</param>
        /// <param name="alpha">multiplicationi factor for A</param>
        /// <param name="A">pointer to array A</param>
        /// <param name="lda">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix A</param>
        /// <param name="B">pointer to array B</param>
        /// <param name="ldb">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix B</param>
        /// <param name="beta">multiplication faktor for matrix B</param>
        /// <param name="C">pointer to predefined array C of neccessary length</param>
        /// <param name="ldc">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix C</param>
        /// <remarks>All parameters except C are readonly. Only elements of matrix C will be altered. C must be a predefined 
        /// continous array of size MxN</remarks>
        void  cgemm (char TransA, char TransB, int M, int N, int K,
            fcomplex alpha,  IntPtr A, int lda,
            IntPtr B, int ldb,
            fcomplex beta,  fcomplex [] C, int ldc);
       

        /// <summary>
        /// Wrapper implementiation for ATLAS GeneralMatrixMultiply
        /// </summary>
        /// <param name="TransA">Transposition state for matrix A: one of the constants in enum CBlas_Transpose</param>
        /// <param name="TransB">Transposition state for matrix B: one of the constants in enum CBlas_Transpose</param>
        /// <param name="M">Number of rows in A</param>
        /// <param name="N">Number of columns in B</param>
        /// <param name="K">Number of columns in A and number of rows in B</param>
        /// <param name="alpha">multiplicationi factor for A</param>
        /// <param name="A">pointer to array A</param>
        /// <param name="lda">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix A</param>
        /// <param name="B">pointer to array B</param>
        /// <param name="ldb">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix B</param>
        /// <param name="beta">multiplication faktor for matrix B</param>
        /// <param name="C">pointer to predefined array C of neccessary length</param>
        /// <param name="ldc">distance between first elements of each column for column based orientation or 
        /// distance between first elements of each row for row based orientation for matrix C</param>
        /// <remarks>All parameters except C are readonly. Only elements of matrix C will be altered. C must be a predefined 
        /// continous array of size MxN</remarks>
        void  zgemm (char TransA, char TransB, int M, int N, int K,
            complex alpha,  IntPtr A, int lda,
            IntPtr B, int ldb,
            complex beta,  complex [] C, int ldc);

#endregion 

        #region ?GESDD
        /// <summary>
        /// singular value decomposition, new version, more memory needed
        /// </summary>
        /// <param name="jobz">Specifies options for computing all or part of the matrix U
        /// <list type="table">
        /// <listheader>
        ///     <term>jobz value</term>
        ///     <description>... will result in:</description>
        /// </listheader>
        ///     <item>
        ///         <term>A</term>
        ///         <description>all M columns of U and all N rows of V**T are
        ///                     returned in the arrays U and VT</description>
        ///     </item>
        ///     <item>  <term>S</term>
        ///             <description>the first min(M,N) columns of U and the first
        ///              min(M,N) rows of V**T are returned in the arrays U
        ///              and VT</description>
        ///     </item>
        ///     <item> <term>O</term>  
        ///            <description>If M >= N, the first N columns of U are overwritten
        ///              on the array A and all rows of V**T are returned in
        ///              the array VT,
        ///              otherwise, all columns of U are returned in the
        ///              array U and the first M rows of V**T are overwritten
        ///              in the array VT</description>
        ///     </item>
        ///     <item> <term>N</term> 
        ///            <description>no columns of U or rows of V**T are computed.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <param name="m">The number of rows of the input matrix A.  M greater or equal to 0.</param>
        /// <param name="n">The number of columns of the input matrix A.  N greater or equal to 0</param>
        /// <param name="a">On entry, the M-by-N matrix A.
        ///          On exit, <list><item>
        ///          if JOBZ = 'O',  A is overwritten with the first N columns
        ///                          of U (the left singular vectors, stored
        ///                          columnwise) if M >= N;
        ///                          A is overwritten with the first M rows
        ///                          of V**T (the right singular vectors, stored
        ///                          rowwise) otherwise.</item>
        ///          <item>if JOBZ .ne. 'O', the contents of A are destroyed.</item></list></param>
        /// <param name="lda">The leading dimension of the array A.  LDA ge max(1,M).</param>
        /// <param name="s">array, dimension (min(M,N)). The singular values of A, sorted so that S(i) ge S(i+1)</param>
        /// <param name="u">array, dimension (LDU,UCOL)
        ///          UCOL = M if JOBZ = 'A' or JOBZ = 'O' and M &lt; N;
        ///          UCOL = min(M,N) if JOBZ = 'S'.
        ///          If JOBZ = 'A' or JOBZ = 'O' and M &lt; N, U contains the M-by-M
        ///          orthogonal matrix U;
        ///          if JOBZ = 'S', U contains the first min(M,N) columns of U
        ///          (the left singular vectors, stored columnwise);
        ///          if JOBZ = 'O' and M &gt;= N, or JOBZ = 'N', U is not referenced.</param>
        /// <param name="ldu">The leading dimension of the array U.  LDU &gt;= 1; if  
        /// JOBZ = 'S' or 'A' or JOBZ = 'O' and M &lt; N, LDU &gt;= M</param>
        /// <param name="vt">array, dimension (LDVT,N). If JOBZ = 'A' or JOBZ = 'O' and 
        /// M >= N, VT contains the N-by-N orthogonal matrix V**T; if JOBZ = 'S', 
        /// VT contains the first min(M,N) rows of V**T 
        /// (the right singular vectors, stored rowwise); if JOBZ = 'O' and M &lt; N, 
        /// or JOBZ = 'N', VT is not referenced</param>
        /// <param name="ldvt">The leading dimension of the array VT.  LDVT &gt; = 1; 
        /// if JOBZ = 'A' or JOBZ = 'O' and M &gt; = N, LDVT &gt;= N; 
        /// if JOBZ = 'S', LDVT &gt; min(M,N).</param>
        /// <param name="info">
        /// <list>
        ///     <item> 0:  successful exit.</item>
        ///     <item> <![CDATA[< 0]]> :  if INFO = -i, the i-th argument had an illegal value.</item>
        ///     <item> <![CDATA[> 0]]> :  ?BGSDD did not converge, updating process failed.</item>
        /// </list>
        /// </param>
        /// <remarks>(From the lapack manual):DGESDD computes the singular value decomposition (SVD) of a real
        ///M-by-N matrix A, optionally computing the left and right singular
        ///vectors.  If singular vectors are desired, it uses a
        ///divide-and-conquer algorithm.
        ///The SVD is written
        ///    <code>A = U * SIGMA * transpose(V)</code> 
        ///where SIGMA is an M-by-N matrix which is zero except for its
        ///min(m,n) diagonal elements, U is an M-by-M orthogonal matrix, and
        ///V is an N-by-N orthogonal matrix.  The diagonal elements of SIGMA
        ///are the singular values of A; they are real and non-negative, and
        ///are returned in descending order.  The first min(m,n) columns of
        ///U and V are the left and right singular vectors of A.
        ///Note that the routine returns VT = V**T, not V.
        ///The divide and conquer algorithm makes very mild assumptions about
        ///floating point arithmetic. It will work on machines with a guard
        ///digit in add/subtract, or on those binary machines without guard
        ///digits which subtract like the Cray X-MP, Cray Y-MP, Cray C-90, or
        ///Cray-2. It could conceivably fail on hexadecimal or decimal machines
        ///without guard digits, but we know of none.</remarks>
        void  dgesdd (char jobz, int m, int n,
             double [] a, int lda,  double [] s,
             double [] u, int ldu,  double [] vt,
                   int ldvt, //double [] work, int lwork, 
                   // int [] iwork,
                   ref int info);

        /// <summary>
        /// singular value decomposition, new version, more memory needed
        /// </summary>
        /// <param name="jobz">Specifies options for computing all or part of the matrix U
        /// <list type="table">
        /// <listheader>
        ///     <term>jobz value</term>
        ///     <description>... will result in:</description>
        /// </listheader>
        ///     <item>
        ///         <term>A</term>
        ///         <description>all M columns of U and all N rows of V**T are
        ///                     returned in the arrays U and VT</description>
        ///     </item>
        ///     <item>  <term>S</term>
        ///             <description>the first min(M,N) columns of U and the first
        ///              min(M,N) rows of V**T are returned in the arrays U
        ///              and VT</description>
        ///     </item>
        ///     <item> <term>O</term>  
        ///            <description>If M >= N, the first N columns of U are overwritten
        ///              on the array A and all rows of V**T are returned in
        ///              the array VT,
        ///              otherwise, all columns of U are returned in the
        ///              array U and the first M rows of V**T are overwritten
        ///              in the array VT</description>
        ///     </item>
        ///     <item> <term>N</term> 
        ///            <description>no columns of U or rows of V**T are computed.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <param name="m">The number of rows of the input matrix A.  M greater or equal to 0.</param>
        /// <param name="n">The number of columns of the input matrix A.  N greater or equal to 0</param>
        /// <param name="a">On entry, the M-by-N matrix A.
        ///          On exit, <list><item>
        ///          if JOBZ = 'O',  A is overwritten with the first N columns
        ///                          of U (the left singular vectors, stored
        ///                          columnwise) if M >= N;
        ///                          A is overwritten with the first M rows
        ///                          of V**T (the right singular vectors, stored
        ///                          rowwise) otherwise.</item>
        ///          <item>if JOBZ .ne. 'O', the contents of A are destroyed.</item></list></param>
        /// <param name="lda">The leading dimension of the array A.  LDA ge max(1,M).</param>
        /// <param name="s">array, dimension (min(M,N)). The singular values of A, sorted so that S(i) ge S(i+1)</param>
        /// <param name="u">array, dimension (LDU,UCOL)
        ///          UCOL = M if JOBZ = 'A' or JOBZ = 'O' and M &lt; N;
        ///          UCOL = min(M,N) if JOBZ = 'S'.
        ///          If JOBZ = 'A' or JOBZ = 'O' and M &lt; N, U contains the M-by-M
        ///          orthogonal matrix U;
        ///          if JOBZ = 'S', U contains the first min(M,N) columns of U
        ///          (the left singular vectors, stored columnwise);
        ///          if JOBZ = 'O' and M &gt;= N, or JOBZ = 'N', U is not referenced.</param>
        /// <param name="ldu">The leading dimension of the array U.  LDU &gt;= 1; if  
        /// JOBZ = 'S' or 'A' or JOBZ = 'O' and M &lt; N, LDU &gt;= M</param>
        /// <param name="vt">array, dimension (LDVT,N). If JOBZ = 'A' or JOBZ = 'O' and 
        /// M >= N, VT contains the N-by-N orthogonal matrix V**T; if JOBZ = 'S', 
        /// VT contains the first min(M,N) rows of V**T 
        /// (the right singular vectors, stored rowwise); if JOBZ = 'O' and M &lt; N, 
        /// or JOBZ = 'N', VT is not referenced</param>
        /// <param name="ldvt">The leading dimension of the array VT.  LDVT &gt; = 1; 
        /// if JOBZ = 'A' or JOBZ = 'O' and M &gt; = N, LDVT &gt;= N; 
        /// if JOBZ = 'S', LDVT &gt; min(M,N).</param>
        /// <param name="info">
        /// <list>
        ///     <item> 0:  successful exit.</item>
        ///     <item> <![CDATA[< 0]]> :  if INFO = -i, the i-th argument had an illegal value.</item>
        ///     <item> <![CDATA[> 0]]> :  ?BGSDD did not converge, updating process failed.</item>
        /// </list>
        /// </param>
        /// <remarks>(From the lapack manual):DGESDD computes the singular value decomposition (SVD) of a real
        ///M-by-N matrix A, optionally computing the left and right singular
        ///vectors.  If singular vectors are desired, it uses a
        ///divide-and-conquer algorithm.
        ///The SVD is written
        ///    <code>A = U * SIGMA * transpose(V)</code> 
        ///where SIGMA is an M-by-N matrix which is zero except for its
        ///min(m,n) diagonal elements, U is an M-by-M orthogonal matrix, and
        ///V is an N-by-N orthogonal matrix.  The diagonal elements of SIGMA
        ///are the singular values of A; they are real and non-negative, and
        ///are returned in descending order.  The first min(m,n) columns of
        ///U and V are the left and right singular vectors of A.
        ///Note that the routine returns VT = V**T, not V.
        ///The divide and conquer algorithm makes very mild assumptions about
        ///floating point arithmetic. It will work on machines with a guard
        ///digit in add/subtract, or on those binary machines without guard
        ///digits which subtract like the Cray X-MP, Cray Y-MP, Cray C-90, or
        ///Cray-2. It could conceivably fail on hexadecimal or decimal machines
        ///without guard digits, but we know of none.</remarks>
        void  sgesdd (char jobz, int m, int n,
            float [] a, int lda,  float [] s,
            float [] u, int ldu,  float [] vt,
                   int ldvt, //double [] work, int lwork, 
                   // int [] iwork,
                   ref int info);
       
        /// <summary>
        /// singular value decomposition, new version, more memory needed
        /// </summary>
        /// <param name="jobz">Specifies options for computing all or part of the matrix U
        /// <list type="table">
        /// <listheader>
        ///     <term>jobz value</term>
        ///     <description>... will result in:</description>
        /// </listheader>
        ///     <item>
        ///         <term>A</term>
        ///         <description>all M columns of U and all N rows of V**T are
        ///                     returned in the arrays U and VT</description>
        ///     </item>
        ///     <item>  <term>S</term>
        ///             <description>the first min(M,N) columns of U and the first
        ///              min(M,N) rows of V**T are returned in the arrays U
        ///              and VT</description>
        ///     </item>
        ///     <item> <term>O</term>  
        ///            <description>If M >= N, the first N columns of U are overwritten
        ///              on the array A and all rows of V**T are returned in
        ///              the array VT,
        ///              otherwise, all columns of U are returned in the
        ///              array U and the first M rows of V**T are overwritten
        ///              in the array VT</description>
        ///     </item>
        ///     <item> <term>N</term> 
        ///            <description>no columns of U or rows of V**T are computed.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <param name="m">The number of rows of the input matrix A.  M greater or equal to 0.</param>
        /// <param name="n">The number of columns of the input matrix A.  N greater or equal to 0</param>
        /// <param name="a">On entry, the M-by-N matrix A.
        ///          On exit, <list><item>
        ///          if JOBZ = 'O',  A is overwritten with the first N columns
        ///                          of U (the left singular vectors, stored
        ///                          columnwise) if M >= N;
        ///                          A is overwritten with the first M rows
        ///                          of V**T (the right singular vectors, stored
        ///                          rowwise) otherwise.</item>
        ///          <item>if JOBZ .ne. 'O', the contents of A are destroyed.</item></list></param>
        /// <param name="lda">The leading dimension of the array A.  LDA ge max(1,M).</param>
        /// <param name="s">array, dimension (min(M,N)). The singular values of A, sorted so that S(i) ge S(i+1)</param>
        /// <param name="u">array, dimension (LDU,UCOL)
        ///          UCOL = M if JOBZ = 'A' or JOBZ = 'O' and M &lt; N;
        ///          UCOL = min(M,N) if JOBZ = 'S'.
        ///          If JOBZ = 'A' or JOBZ = 'O' and M &lt; N, U contains the M-by-M
        ///          orthogonal matrix U;
        ///          if JOBZ = 'S', U contains the first min(M,N) columns of U
        ///          (the left singular vectors, stored columnwise);
        ///          if JOBZ = 'O' and M &gt;= N, or JOBZ = 'N', U is not referenced.</param>
        /// <param name="ldu">The leading dimension of the array U.  LDU &gt;= 1; if  
        /// JOBZ = 'S' or 'A' or JOBZ = 'O' and M &lt; N, LDU &gt;= M</param>
        /// <param name="vt">array, dimension (LDVT,N). If JOBZ = 'A' or JOBZ = 'O' and 
        /// M >= N, VT contains the N-by-N orthogonal matrix V**T; if JOBZ = 'S', 
        /// VT contains the first min(M,N) rows of V**T 
        /// (the right singular vectors, stored rowwise); if JOBZ = 'O' and M &lt; N, 
        /// or JOBZ = 'N', VT is not referenced</param>
        /// <param name="ldvt">The leading dimension of the array VT.  LDVT &gt; = 1; 
        /// if JOBZ = 'A' or JOBZ = 'O' and M &gt; = N, LDVT &gt;= N; 
        /// if JOBZ = 'S', LDVT &gt; min(M,N).</param>
        /// <param name="info">
        /// <list>
        ///     <item> 0:  successful exit.</item>
        ///     <item> <![CDATA[< 0]]> :  if INFO = -i, the i-th argument had an illegal value.</item>
        ///     <item> <![CDATA[> 0]]> :  ?BGSDD did not converge, updating process failed.</item>
        /// </list>
        /// </param>
        /// <remarks>(From the lapack manual):DGESDD computes the singular value decomposition (SVD) of a real
        ///M-by-N matrix A, optionally computing the left and right singular
        ///vectors.  If singular vectors are desired, it uses a
        ///divide-and-conquer algorithm.
        ///The SVD is written
        ///    <code>A = U * SIGMA * transpose(V)</code> 
        ///where SIGMA is an M-by-N matrix which is zero except for its
        ///min(m,n) diagonal elements, U is an M-by-M orthogonal matrix, and
        ///V is an N-by-N orthogonal matrix.  The diagonal elements of SIGMA
        ///are the singular values of A; they are real and non-negative, and
        ///are returned in descending order.  The first min(m,n) columns of
        ///U and V are the left and right singular vectors of A.
        ///Note that the routine returns VT = V**T, not V.
        ///The divide and conquer algorithm makes very mild assumptions about
        ///floating point arithmetic. It will work on machines with a guard
        ///digit in add/subtract, or on those binary machines without guard
        ///digits which subtract like the Cray X-MP, Cray Y-MP, Cray C-90, or
        ///Cray-2. It could conceivably fail on hexadecimal or decimal machines
        ///without guard digits, but we know of none.</remarks>
        void  cgesdd (char jobz, int m, int n,
            fcomplex [] a, int lda,  float [] s,
            fcomplex [] u, int ldu,  fcomplex [] vt,
                   int ldvt, //double [] work, int lwork, 
                   // int [] iwork,
                   ref int info);
       
        /// <summary>
        /// singular value decomposition, new version, more memory needed
        /// </summary>
        /// <param name="jobz">Specifies options for computing all or part of the matrix U
        /// <list type="table">
        /// <listheader>
        ///     <term>jobz value</term>
        ///     <description>... will result in:</description>
        /// </listheader>
        ///     <item>
        ///         <term>A</term>
        ///         <description>all M columns of U and all N rows of V**T are
        ///                     returned in the arrays U and VT</description>
        ///     </item>
        ///     <item>  <term>S</term>
        ///             <description>the first min(M,N) columns of U and the first
        ///              min(M,N) rows of V**T are returned in the arrays U
        ///              and VT</description>
        ///     </item>
        ///     <item> <term>O</term>  
        ///            <description>If M >= N, the first N columns of U are overwritten
        ///              on the array A and all rows of V**T are returned in
        ///              the array VT,
        ///              otherwise, all columns of U are returned in the
        ///              array U and the first M rows of V**T are overwritten
        ///              in the array VT</description>
        ///     </item>
        ///     <item> <term>N</term> 
        ///            <description>no columns of U or rows of V**T are computed.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <param name="m">The number of rows of the input matrix A.  M greater or equal to 0.</param>
        /// <param name="n">The number of columns of the input matrix A.  N greater or equal to 0</param>
        /// <param name="a">On entry, the M-by-N matrix A.
        ///          On exit, <list><item>
        ///          if JOBZ = 'O',  A is overwritten with the first N columns
        ///                          of U (the left singular vectors, stored
        ///                          columnwise) if M >= N;
        ///                          A is overwritten with the first M rows
        ///                          of V**T (the right singular vectors, stored
        ///                          rowwise) otherwise.</item>
        ///          <item>if JOBZ .ne. 'O', the contents of A are destroyed.</item></list></param>
        /// <param name="lda">The leading dimension of the array A.  LDA ge max(1,M).</param>
        /// <param name="s">array, dimension (min(M,N)). The singular values of A, sorted so that S(i) ge S(i+1)</param>
        /// <param name="u">array, dimension (LDU,UCOL)
        ///          UCOL = M if JOBZ = 'A' or JOBZ = 'O' and M &lt; N;
        ///          UCOL = min(M,N) if JOBZ = 'S'.
        ///          If JOBZ = 'A' or JOBZ = 'O' and M &lt; N, U contains the M-by-M
        ///          orthogonal matrix U;
        ///          if JOBZ = 'S', U contains the first min(M,N) columns of U
        ///          (the left singular vectors, stored columnwise);
        ///          if JOBZ = 'O' and M &gt;= N, or JOBZ = 'N', U is not referenced.</param>
        /// <param name="ldu">The leading dimension of the array U.  LDU &gt;= 1; if  
        /// JOBZ = 'S' or 'A' or JOBZ = 'O' and M &lt; N, LDU &gt;= M</param>
        /// <param name="vt">array, dimension (LDVT,N). If JOBZ = 'A' or JOBZ = 'O' and 
        /// M >= N, VT contains the N-by-N orthogonal matrix V**T; if JOBZ = 'S', 
        /// VT contains the first min(M,N) rows of V**T 
        /// (the right singular vectors, stored rowwise); if JOBZ = 'O' and M &lt; N, 
        /// or JOBZ = 'N', VT is not referenced</param>
        /// <param name="ldvt">The leading dimension of the array VT.  LDVT &gt; = 1; 
        /// if JOBZ = 'A' or JOBZ = 'O' and M &gt; = N, LDVT &gt;= N; 
        /// if JOBZ = 'S', LDVT &gt; min(M,N).</param>
        /// <param name="info">
        /// <list>
        ///     <item> 0:  successful exit.</item>
        ///     <item> <![CDATA[< 0]]> :  if INFO = -i, the i-th argument had an illegal value.</item>
        ///     <item> <![CDATA[> 0]]> :  ?BGSDD did not converge, updating process failed.</item>
        /// </list>
        /// </param>
        /// <remarks>(From the lapack manual):DGESDD computes the singular value decomposition (SVD) of a real
        ///M-by-N matrix A, optionally computing the left and right singular
        ///vectors.  If singular vectors are desired, it uses a
        ///divide-and-conquer algorithm.
        ///The SVD is written
        ///    <code>A = U * SIGMA * transpose(V)</code> 
        ///where SIGMA is an M-by-N matrix which is zero except for its
        ///min(m,n) diagonal elements, U is an M-by-M orthogonal matrix, and
        ///V is an N-by-N orthogonal matrix.  The diagonal elements of SIGMA
        ///are the singular values of A; they are real and non-negative, and
        ///are returned in descending order.  The first min(m,n) columns of
        ///U and V are the left and right singular vectors of A.
        ///Note that the routine returns VT = V**T, not V.
        ///The divide and conquer algorithm makes very mild assumptions about
        ///floating point arithmetic. It will work on machines with a guard
        ///digit in add/subtract, or on those binary machines without guard
        ///digits which subtract like the Cray X-MP, Cray Y-MP, Cray C-90, or
        ///Cray-2. It could conceivably fail on hexadecimal or decimal machines
        ///without guard digits, but we know of none.</remarks>
        void  zgesdd (char jobz, int m, int n,
            complex [] a, int lda,  double [] s,
            complex [] u, int ldu,  complex [] vt,
                   int ldvt, //double [] work, int lwork, 
                   // int [] iwork,
                   ref int info);

#endregion 

        #region ?GESVD
        /// <summary>
        /// singular value decomposition, older version, less memory needed 
        /// </summary>
        /// <param name="jobz">Specifies options for computing all or part of the matrix U
        /// <list type="bullet"><item>= 'A':  all M columns of U and all N rows of V**T are
        ///returned in the arrays U and VT</item>
        ///     <item> = 'S':  the first min(M,N) columns of U and the first
        ///              min(M,N) rows of V**T are returned in the arrays U
        ///              and VT</item>
        ///     <item> = 'O':  If M >= N, the first N columns of U are overwritten
        ///              on the array A and all rows of V**T are returned in
        ///              the array VT. Otherwise, all columns of U are returned in the
        ///              array U and the first M rows of V**T are overwritten
        ///              in the array VT</item>
        ///     <item> = 'N':  no columns of U or rows of V**T are computed.</item>
        ///        </list>
        /// </param>
        /// <param name="m">The number of rows of the input matrix A.  M greater or equal to 0.</param>
        /// <param name="n">The number of columns of the input matrix A.  N greater or equal to 0</param>
        /// <param name="a">On entry, the M-by-N matrix A.
        ///          On exit, <list><item>
        ///          if JOBZ = 'O',  A is overwritten with the first N columns
        ///                          of U (the left singular vectors, stored
        ///                          columnwise) if M >= N;
        ///                          A is overwritten with the first M rows
        ///                          of V**T (the right singular vectors, stored
        ///                          rowwise) otherwise.</item>
        ///          <item>if JOBZ .ne. 'O', the contents of A are destroyed.</item></list></param>
        /// <param name="lda">The leading dimension of the array A.  LDA ge max(1,M).</param>
        /// <param name="s">array, dimension (min(M,N)). The singular values of A, sorted so that S(i) ge S(i+1)</param>
        /// <param name="u">array, dimension (LDU,UCOL)
        ///          UCOL = M if JOBZ = 'A' or JOBZ = 'O' and M &lt; N;
        ///          UCOL = min(M,N) if JOBZ = 'S'.
        ///          If JOBZ = 'A' or JOBZ = 'O' and M &lt; N, U contains the M-by-M
        ///          orthogonal matrix U;
        ///          if JOBZ = 'S', U contains the first min(M,N) columns of U
        ///          (the left singular vectors, stored columnwise);
        ///          if JOBZ = 'O' and M &gt;= N, or JOBZ = 'N', U is not referenced.</param>
        /// <param name="ldu">The leading dimension of the array U.  LDU &gt;= 1; if  
        /// JOBZ = 'S' or 'A' or JOBZ = 'O' and M &lt; N, LDU &gt;= M</param>
        /// <param name="vt">array, dimension (LDVT,N). If JOBZ = 'A' or JOBZ = 'O' and 
        /// M >= N, VT contains the N-by-N orthogonal matrix V**T; if JOBZ = 'S', 
        /// VT contains the first min(M,N) rows of V**T 
        /// (the right singular vectors, stored rowwise); if JOBZ = 'O' and M &lt; N, 
        /// or JOBZ = 'N', VT is not referenced</param>
        /// <param name="ldvt">The leading dimension of the array VT.  LDVT &gt; = 1; 
        /// if JOBZ = 'A' or JOBZ = 'O' and M &gt; = N, LDVT &gt;= N; 
        /// if JOBZ = 'S', LDVT &gt; min(M,N).</param>
        /// <param name="info">
        /// <list>
        ///     <item> 0:  successful exit.</item>
        ///     <item> lower 0:  if INFO = -i, the i-th argument had an illegal value.</item>
        ///     <item> greater 0:  DBDSDC did not converge, updating process failed.</item>
        /// </list>
        /// </param>
        /// <remarks>(From the lapack manual):DGESDD computes the singular value decomposition (SVD) of a real
        ///M-by-N matrix A, optionally computing the left and right singular
        ///vectors.  If singular vectors are desired, it uses a
        ///divide-and-conquer algorithm.
        ///The SVD is written
        ///    <br>A = U * SIGMA * transpose(V)</br> 
        ///where SIGMA is an M-by-N matrix which is zero except for its
        ///min(m,n) diagonal elements, U is an M-by-M orthogonal matrix, and
        ///V is an N-by-N orthogonal matrix.  The diagonal elements of SIGMA
        ///are the singular values of A; they are real and non-negative, and
        ///are returned in descending order.  The first min(m,n) columns of
        ///U and V are the left and right singular vectors of A.
        ///Note that the routine returns VT = V**T, not V.
        ///The divide and conquer algorithm makes very mild assumptions about
        ///floating point arithmetic. It will work on machines with a guard
        ///digit in add/subtract, or on those binary machines without guard
        ///digits which subtract like the Cray X-MP, Cray Y-MP, Cray C-90, or
        ///Cray-2. It could conceivably fail on hexadecimal or decimal machines
        ///without guard digits, but we know of none.</remarks>
        void  dgesvd (char jobz, int m, int n,  double [] a, int lda,
             double [] s,  double [] u, int ldu, 
             double [] vt, int ldvt, ref int info);

        /// <summary>
        /// singular value decomposition, older version, less memory needed 
        /// </summary>
        /// <param name="jobz">Specifies options for computing all or part of the matrix U
        /// <list type="bullet"><item>= 'A':  all M columns of U and all N rows of V**T are
        ///returned in the arrays U and VT</item>
        ///     <item> = 'S':  the first min(M,N) columns of U and the first
        ///              min(M,N) rows of V**T are returned in the arrays U
        ///              and VT</item>
        ///     <item> = 'O':  If M >= N, the first N columns of U are overwritten
        ///              on the array A and all rows of V**T are returned in
        ///              the array VT. Otherwise, all columns of U are returned in the
        ///              array U and the first M rows of V**T are overwritten
        ///              in the array VT</item>
        ///     <item> = 'N':  no columns of U or rows of V**T are computed.</item>
        ///        </list>
        /// </param>
        /// <param name="m">The number of rows of the input matrix A.  M greater or equal to 0.</param>
        /// <param name="n">The number of columns of the input matrix A.  N greater or equal to 0</param>
        /// <param name="a">On entry, the M-by-N matrix A.
        ///          On exit, <list><item>
        ///          if JOBZ = 'O',  A is overwritten with the first N columns
        ///                          of U (the left singular vectors, stored
        ///                          columnwise) if M >= N;
        ///                          A is overwritten with the first M rows
        ///                          of V**T (the right singular vectors, stored
        ///                          rowwise) otherwise.</item>
        ///          <item>if JOBZ .ne. 'O', the contents of A are destroyed.</item></list></param>
        /// <param name="lda">The leading dimension of the array A.  LDA ge max(1,M).</param>
        /// <param name="s">array, dimension (min(M,N)). The singular values of A, sorted so that S(i) ge S(i+1)</param>
        /// <param name="u">array, dimension (LDU,UCOL)
        ///          UCOL = M if JOBZ = 'A' or JOBZ = 'O' and M &lt; N;
        ///          UCOL = min(M,N) if JOBZ = 'S'.
        ///          If JOBZ = 'A' or JOBZ = 'O' and M &lt; N, U contains the M-by-M
        ///          orthogonal matrix U;
        ///          if JOBZ = 'S', U contains the first min(M,N) columns of U
        ///          (the left singular vectors, stored columnwise);
        ///          if JOBZ = 'O' and M &gt;= N, or JOBZ = 'N', U is not referenced.</param>
        /// <param name="ldu">The leading dimension of the array U.  LDU &gt;= 1; if  
        /// JOBZ = 'S' or 'A' or JOBZ = 'O' and M &lt; N, LDU &gt;= M</param>
        /// <param name="vt">array, dimension (LDVT,N). If JOBZ = 'A' or JOBZ = 'O' and 
        /// M >= N, VT contains the N-by-N orthogonal matrix V**T; if JOBZ = 'S', 
        /// VT contains the first min(M,N) rows of V**T 
        /// (the right singular vectors, stored rowwise); if JOBZ = 'O' and M &lt; N, 
        /// or JOBZ = 'N', VT is not referenced</param>
        /// <param name="ldvt">The leading dimension of the array VT.  LDVT &gt; = 1; 
        /// if JOBZ = 'A' or JOBZ = 'O' and M &gt; = N, LDVT &gt;= N; 
        /// if JOBZ = 'S', LDVT &gt; min(M,N).</param>
        /// <param name="info">
        /// <list>
        ///     <item> 0:  successful exit.</item>
        ///     <item> lower 0:  if INFO = -i, the i-th argument had an illegal value.</item>
        ///     <item> greater 0:  DBDSDC did not converge, updating process failed.</item>
        /// </list>
        /// </param>
        /// <remarks>(From the lapack manual):DGESDD computes the singular value decomposition (SVD) of a real
        ///M-by-N matrix A, optionally computing the left and right singular
        ///vectors.  If singular vectors are desired, it uses a
        ///divide-and-conquer algorithm.
        ///The SVD is written
        ///    <br>A = U * SIGMA * transpose(V)</br> 
        ///where SIGMA is an M-by-N matrix which is zero except for its
        ///min(m,n) diagonal elements, U is an M-by-M orthogonal matrix, and
        ///V is an N-by-N orthogonal matrix.  The diagonal elements of SIGMA
        ///are the singular values of A; they are real and non-negative, and
        ///are returned in descending order.  The first min(m,n) columns of
        ///U and V are the left and right singular vectors of A.
        ///Note that the routine returns VT = V**T, not V.
        ///The divide and conquer algorithm makes very mild assumptions about
        ///floating point arithmetic. It will work on machines with a guard
        ///digit in add/subtract, or on those binary machines without guard
        ///digits which subtract like the Cray X-MP, Cray Y-MP, Cray C-90, or
        ///Cray-2. It could conceivably fail on hexadecimal or decimal machines
        ///without guard digits, but we know of none.</remarks>
        void  sgesvd (char jobz, int m, int n,  float [] a, int lda,
            float [] s,  float [] u, int ldu, 
            float [] vt, int ldvt, ref int info);

       
        /// <summary>
        /// singular value decomposition, older version, less memory needed 
        /// </summary>
        /// <param name="jobz">Specifies options for computing all or part of the matrix U
        /// <list type="bullet"><item>= 'A':  all M columns of U and all N rows of V**T are
        ///returned in the arrays U and VT</item>
        ///     <item> = 'S':  the first min(M,N) columns of U and the first
        ///              min(M,N) rows of V**T are returned in the arrays U
        ///              and VT</item>
        ///     <item> = 'O':  If M >= N, the first N columns of U are overwritten
        ///              on the array A and all rows of V**T are returned in
        ///              the array VT. Otherwise, all columns of U are returned in the
        ///              array U and the first M rows of V**T are overwritten
        ///              in the array VT</item>
        ///     <item> = 'N':  no columns of U or rows of V**T are computed.</item>
        ///        </list>
        /// </param>
        /// <param name="m">The number of rows of the input matrix A.  M greater or equal to 0.</param>
        /// <param name="n">The number of columns of the input matrix A.  N greater or equal to 0</param>
        /// <param name="a">On entry, the M-by-N matrix A.
        ///          On exit, <list><item>
        ///          if JOBZ = 'O',  A is overwritten with the first N columns
        ///                          of U (the left singular vectors, stored
        ///                          columnwise) if M >= N;
        ///                          A is overwritten with the first M rows
        ///                          of V**T (the right singular vectors, stored
        ///                          rowwise) otherwise.</item>
        ///          <item>if JOBZ .ne. 'O', the contents of A are destroyed.</item></list></param>
        /// <param name="lda">The leading dimension of the array A.  LDA ge max(1,M).</param>
        /// <param name="s">array, dimension (min(M,N)). The singular values of A, sorted so that S(i) ge S(i+1)</param>
        /// <param name="u">array, dimension (LDU,UCOL)
        ///          UCOL = M if JOBZ = 'A' or JOBZ = 'O' and M &lt; N;
        ///          UCOL = min(M,N) if JOBZ = 'S'.
        ///          If JOBZ = 'A' or JOBZ = 'O' and M &lt; N, U contains the M-by-M
        ///          orthogonal matrix U;
        ///          if JOBZ = 'S', U contains the first min(M,N) columns of U
        ///          (the left singular vectors, stored columnwise);
        ///          if JOBZ = 'O' and M &gt;= N, or JOBZ = 'N', U is not referenced.</param>
        /// <param name="ldu">The leading dimension of the array U.  LDU &gt;= 1; if  
        /// JOBZ = 'S' or 'A' or JOBZ = 'O' and M &lt; N, LDU &gt;= M</param>
        /// <param name="vt">array, dimension (LDVT,N). If JOBZ = 'A' or JOBZ = 'O' and 
        /// M >= N, VT contains the N-by-N orthogonal matrix V**T; if JOBZ = 'S', 
        /// VT contains the first min(M,N) rows of V**T 
        /// (the right singular vectors, stored rowwise); if JOBZ = 'O' and M &lt; N, 
        /// or JOBZ = 'N', VT is not referenced</param>
        /// <param name="ldvt">The leading dimension of the array VT.  LDVT &gt; = 1; 
        /// if JOBZ = 'A' or JOBZ = 'O' and M &gt; = N, LDVT &gt;= N; 
        /// if JOBZ = 'S', LDVT &gt; min(M,N).</param>
        /// <param name="info">
        /// <list>
        ///     <item> 0:  successful exit.</item>
        ///     <item> lower 0:  if INFO = -i, the i-th argument had an illegal value.</item>
        ///     <item> greater 0:  DBDSDC did not converge, updating process failed.</item>
        /// </list>
        /// </param>
        /// <remarks>(From the lapack manual):DGESDD computes the singular value decomposition (SVD) of a real
        ///M-by-N matrix A, optionally computing the left and right singular
        ///vectors.  If singular vectors are desired, it uses a
        ///divide-and-conquer algorithm.
        ///The SVD is written
        ///    <br>A = U * SIGMA * transpose(V)</br> 
        ///where SIGMA is an M-by-N matrix which is zero except for its
        ///min(m,n) diagonal elements, U is an M-by-M orthogonal matrix, and
        ///V is an N-by-N orthogonal matrix.  The diagonal elements of SIGMA
        ///are the singular values of A; they are real and non-negative, and
        ///are returned in descending order.  The first min(m,n) columns of
        ///U and V are the left and right singular vectors of A.
        ///Note that the routine returns VT = V**T, not V.
        ///The divide and conquer algorithm makes very mild assumptions about
        ///floating point arithmetic. It will work on machines with a guard
        ///digit in add/subtract, or on those binary machines without guard
        ///digits which subtract like the Cray X-MP, Cray Y-MP, Cray C-90, or
        ///Cray-2. It could conceivably fail on hexadecimal or decimal machines
        ///without guard digits, but we know of none.</remarks>
        void  cgesvd (char jobz, int m, int n,  fcomplex [] a, int lda,
            float [] s,  fcomplex [] u, int ldu, 
            fcomplex [] vt, int ldvt, ref int info);

       
        /// <summary>
        /// singular value decomposition, older version, less memory needed 
        /// </summary>
        /// <param name="jobz">Specifies options for computing all or part of the matrix U
        /// <list type="bullet"><item>= 'A':  all M columns of U and all N rows of V**T are
        ///returned in the arrays U and VT</item>
        ///     <item> = 'S':  the first min(M,N) columns of U and the first
        ///              min(M,N) rows of V**T are returned in the arrays U
        ///              and VT</item>
        ///     <item> = 'O':  If M >= N, the first N columns of U are overwritten
        ///              on the array A and all rows of V**T are returned in
        ///              the array VT. Otherwise, all columns of U are returned in the
        ///              array U and the first M rows of V**T are overwritten
        ///              in the array VT</item>
        ///     <item> = 'N':  no columns of U or rows of V**T are computed.</item>
        ///        </list>
        /// </param>
        /// <param name="m">The number of rows of the input matrix A.  M greater or equal to 0.</param>
        /// <param name="n">The number of columns of the input matrix A.  N greater or equal to 0</param>
        /// <param name="a">On entry, the M-by-N matrix A.
        ///          On exit, <list><item>
        ///          if JOBZ = 'O',  A is overwritten with the first N columns
        ///                          of U (the left singular vectors, stored
        ///                          columnwise) if M >= N;
        ///                          A is overwritten with the first M rows
        ///                          of V**T (the right singular vectors, stored
        ///                          rowwise) otherwise.</item>
        ///          <item>if JOBZ .ne. 'O', the contents of A are destroyed.</item></list></param>
        /// <param name="lda">The leading dimension of the array A.  LDA ge max(1,M).</param>
        /// <param name="s">array, dimension (min(M,N)). The singular values of A, sorted so that S(i) ge S(i+1)</param>
        /// <param name="u">array, dimension (LDU,UCOL)
        ///          UCOL = M if JOBZ = 'A' or JOBZ = 'O' and M &lt; N;
        ///          UCOL = min(M,N) if JOBZ = 'S'.
        ///          If JOBZ = 'A' or JOBZ = 'O' and M &lt; N, U contains the M-by-M
        ///          orthogonal matrix U;
        ///          if JOBZ = 'S', U contains the first min(M,N) columns of U
        ///          (the left singular vectors, stored columnwise);
        ///          if JOBZ = 'O' and M &gt;= N, or JOBZ = 'N', U is not referenced.</param>
        /// <param name="ldu">The leading dimension of the array U.  LDU &gt;= 1; if  
        /// JOBZ = 'S' or 'A' or JOBZ = 'O' and M &lt; N, LDU &gt;= M</param>
        /// <param name="vt">array, dimension (LDVT,N). If JOBZ = 'A' or JOBZ = 'O' and 
        /// M >= N, VT contains the N-by-N orthogonal matrix V**T; if JOBZ = 'S', 
        /// VT contains the first min(M,N) rows of V**T 
        /// (the right singular vectors, stored rowwise); if JOBZ = 'O' and M &lt; N, 
        /// or JOBZ = 'N', VT is not referenced</param>
        /// <param name="ldvt">The leading dimension of the array VT.  LDVT &gt; = 1; 
        /// if JOBZ = 'A' or JOBZ = 'O' and M &gt; = N, LDVT &gt;= N; 
        /// if JOBZ = 'S', LDVT &gt; min(M,N).</param>
        /// <param name="info">
        /// <list>
        ///     <item> 0:  successful exit.</item>
        ///     <item> lower 0:  if INFO = -i, the i-th argument had an illegal value.</item>
        ///     <item> greater 0:  DBDSDC did not converge, updating process failed.</item>
        /// </list>
        /// </param>
        /// <remarks>(From the lapack manual):DGESDD computes the singular value decomposition (SVD) of a real
        ///M-by-N matrix A, optionally computing the left and right singular
        ///vectors.  If singular vectors are desired, it uses a
        ///divide-and-conquer algorithm.
        ///The SVD is written
        ///    <br>A = U * SIGMA * transpose(V)</br> 
        ///where SIGMA is an M-by-N matrix which is zero except for its
        ///min(m,n) diagonal elements, U is an M-by-M orthogonal matrix, and
        ///V is an N-by-N orthogonal matrix.  The diagonal elements of SIGMA
        ///are the singular values of A; they are real and non-negative, and
        ///are returned in descending order.  The first min(m,n) columns of
        ///U and V are the left and right singular vectors of A.
        ///Note that the routine returns VT = V**T, not V.
        ///The divide and conquer algorithm makes very mild assumptions about
        ///floating point arithmetic. It will work on machines with a guard
        ///digit in add/subtract, or on those binary machines without guard
        ///digits which subtract like the Cray X-MP, Cray Y-MP, Cray C-90, or
        ///Cray-2. It could conceivably fail on hexadecimal or decimal machines
        ///without guard digits, but we know of none.</remarks>
        void  zgesvd (char jobz, int m, int n,  complex [] a, int lda,
            double [] s,  complex [] u, int ldu, 
            complex [] vt, int ldvt, ref int info);
        #endregion 
        
        #region ?POTRF - cholesky factorization
        /// <summary>
        /// cholesky factorization 
        /// </summary>
        void  dpotrf (char uplo, int n,  double [] A, int lda, ref int info);
        /// <summary>
        /// cholesky factorization 
        /// </summary>
        void  spotrf (char uplo, int n,  float [] A, int lda, ref int info);
       
        /// <summary>
        /// cholesky factorization 
        /// </summary>
        void  cpotrf (char uplo, int n,  fcomplex [] A, int lda, ref int info);
       
        /// <summary>
        /// cholesky factorization 
        /// </summary>
        void  zpotrf (char uplo, int n,  complex [] A, int lda, ref int info);
#endregion 

        #region ?POTRI - inverse via cholesky factorization
        /// <summary>
        /// matrix inverse via cholesky factorization (?potrf)
        /// </summary>
        void  dpotri (char uplo, int n,  double [] A, int lda,ref int info);
        /// <summary>
        /// matrix inverse via cholesky factorization (?potrf)
        /// </summary>
        void  spotri (char uplo, int n,  float [] A, int lda,ref int info);
       
        /// <summary>
        /// matrix inverse via cholesky factorization (?potrf)
        /// </summary>
        void  cpotri (char uplo, int n,  fcomplex [] A, int lda,ref int info);
       
        /// <summary>
        /// matrix inverse via cholesky factorization (?potrf)
        /// </summary>
        void  zpotri (char uplo, int n,  complex [] A, int lda,ref int info);

#endregion 

        #region ?POTRS - Solve via cholesky factors
        /// <summary>
        /// solve equation system via cholesky factorization (?potrs)
        /// </summary>
        void dpotrs (char uplo, int n, int nrhs, double [] A, int lda, double [] B, int ldb, ref int info);
        /// <summary>
        /// solve equation system via cholesky factorization (?potrs)
        /// </summary>
        void  spotrs (char uplo, int n, int nrhs, float [] A, int lda, float [] B, int ldb, ref int info);
        /// <summary>
        /// solve equation system via cholesky factorization (?potrs)
        /// </summary>
        void  cpotrs (char uplo, int n, int nrhs, fcomplex [] A, int lda, fcomplex [] B, int ldb, ref int info);
        /// <summary>
        /// solve equation system via cholesky factorization (?potrs)
        /// </summary>
        void  zpotrs (char uplo, int n, int nrhs, complex [] A, int lda, complex [] B, int ldb, ref int info);
#endregion

        #region ?getrf - LU factorization
        /// <summary>
        /// LU factorization of general matrix
        /// </summary>
        void  dgetrf (int M, int N,  double [] A, int LDA, int [] IPIV, ref int info);
        /// <summary>
        /// LU factorization of general matrix
        /// </summary>
        void  sgetrf (int M, int N,  float [] A, int LDA, int [] IPIV, ref int info);
       
        /// <summary>
        /// LU factorization of general matrix
        /// </summary>
        void  cgetrf (int M, int N,  fcomplex [] A, int LDA, int [] IPIV, ref int info);
       
        /// <summary>
        /// LU factorization of general matrix
        /// </summary>
        void  zgetrf (int M, int N,  complex [] A, int LDA, int [] IPIV, ref int info);
        #endregion 
        
        #region ?getri - inverse via LU factorization
        /// <summary>
        /// inverse of a matrix via LU factorization
        /// </summary>
        void  dgetri (int N,  double [] A, int LDA, int [] IPIV, ref int info);
        /// <summary>
        /// inverse of a matrix via LU factorization
        /// </summary>
        void  sgetri (int N,  float [] A, int LDA, int [] IPIV, ref int info);
       
        /// <summary>
        /// inverse of a matrix via LU factorization
        /// </summary>
        void  cgetri (int N,  fcomplex [] A, int LDA, int [] IPIV, ref int info);
       
        /// <summary>
        /// inverse of a matrix via LU factorization
        /// </summary>
        void  zgetri (int N,  complex [] A, int LDA, int [] IPIV, ref int info);
        #endregion 

        #region ORGQR
        /// <summary>
        /// QR factor extraction
        /// </summary>
        void  dorgqr (int M, int N, int K,  double [] A, int lda,  double [] tau, ref int info);
        /// <summary>
        /// QR factor extraction
        /// </summary>
        void  sorgqr (int M, int N, int K,  float [] A, int lda,  float [] tau, ref int info);
        /// <summary>
        /// QR factor extraction
        /// </summary>
        void  cungqr (int M, int N, int K,  fcomplex [] A, int lda,  fcomplex [] tau, ref int info);
        /// <summary>
        /// QR factor extraction
        /// </summary>
        void  zungqr (int M, int N, int K,  complex [] A, int lda,  complex [] tau, ref int info);
        #endregion

        #region ?geqrf - QR factorization
        /// <summary>
        /// QR factorization
        /// </summary>
        void  dgeqrf (int M, int N,  double [] A, int lda,  double [] tau, ref int info);
        /// <summary>
        /// QR factorization
        /// </summary>
        void  sgeqrf (int M, int N,  float [] A, int lda,  float [] tau, ref int info);
        /// <summary>
        /// QR factorization
        /// </summary>
        void  cgeqrf (int M, int N,  fcomplex [] A, int lda,  fcomplex [] tau, ref int info);
        /// <summary>
        /// QR factorization
        /// </summary>
        void  zgeqrf (int M, int N,  complex [] A, int lda,  complex [] tau, ref int info);
        #endregion 

        #region GEQP3
        /// <summary>
        /// QR factorisation with column pivoting
        /// </summary>
        void dgeqp3 (int M, int N, double [] A, int LDA, int [] JPVT, double [] tau, ref int info);
        /// <summary>
        /// QR factorisation with column pivoting
        /// </summary>
        void sgeqp3 (int M, int N, float [] A, int LDA, int [] JPVT, float [] tau, ref int info);
        /// <summary>
        /// QR factorisation with column pivoting
        /// </summary>
        void cgeqp3 (int M, int N, fcomplex [] A, int LDA, int [] JPVT, fcomplex [] tau, ref int info);
        /// <summary>
        /// QR factorisation with column pivoting
        /// </summary>
        void zgeqp3 (int M, int N, complex [] A, int LDA, int [] JPVT, complex [] tau, ref int info);
        #endregion

        #region ?ormqr - mmult of QR factorization result
        /// <summary>
        /// multipliation for general matrix with QR decomposition factor
        /// </summary>
        void  dormqr (char side, char trans, int m, int n, int k,  double [] A, int lda,  double [] tau,  double [] C, int LDC, ref int info);
        /// <summary>
        /// multipliation for general matrix with QR decomposition factor
        /// </summary>
        void  sormqr (char side, char trans, int m, int n, int k,  float [] A, int lda,  float [] tau,  float [] C, int LDC, ref int info);

        #endregion 
        
        #region DTRTRS 
        /// <summary>
        /// Solve triangular system of linear equations (forward-/ backward substitution)
        /// </summary>
        /// <param name="uplo">'U': A is upper triangular, 'L': A is lower triangular</param>
        /// <param name="transA">'N':  A * X = B  (No transpose); 'T':  A**T * X = B  (Transpose), 'T':  A**T * X = B  (Transpose)</param>
        /// <param name="diag">'N' arbitrary diagonal elements, 'U' unit diagonal</param>
        /// <param name="N">order of A</param>
        /// <param name="nrhs">number of right hand sides - columns of matrix B</param>
        /// <param name="A">square matrix A</param>
        /// <param name="LDA">spacing between columns for A</param>
        /// <param name="B">(input/output) on input: right hand side, on output: solution x </param>
        /// <param name="LDB">spacing between columns for B</param>
        /// <param name="info">(output) 0: success; &lt; 0: illigal argument, &gt; 0: A is sinular having a zero on the i-th diagonal element. No solution will be computed than. </param>
        void dtrtrs (char uplo, char transA, char diag, int N, int nrhs, IntPtr A, int LDA, IntPtr B, int LDB, ref int info); 
        /// <summary>
        /// Solve triangular system of linear equations (forward-/ backward substitution)
        /// </summary>
        /// <param name="uplo">'U': A is upper triangular, 'L': A is lower triangular</param>
        /// <param name="transA">'N':  A * X = B  (No transpose); 'T':  A**T * X = B  (Transpose), 'T':  A**T * X = B  (Transpose)</param>
        /// <param name="diag">'N' arbitrary diagonal elements, 'U' unit diagonal</param>
        /// <param name="N">order of A</param>
        /// <param name="nrhs">number of right hand sides - columns of matrix B</param>
        /// <param name="A">square matrix A</param>
        /// <param name="LDA">spacing between columns for A</param>
        /// <param name="B">(input/output) on input: right hand side, on output: solution x </param>
        /// <param name="LDB">spacing between columns for B</param>
        /// <param name="info">(output) 0: success; &lt; 0: illigal argument, &gt; 0: A is sinular having a zero on the i-th diagonal element. No solution will be computed than. </param>
        void strtrs (char uplo, char transA, char diag, int N, int nrhs, IntPtr A, int LDA, IntPtr B, int LDB, ref int info); 
        /// <summary>
        /// Solve triangular system of linear equations (forward-/ backward substitution)
        /// </summary>
        /// <param name="uplo">'U': A is upper triangular, 'L': A is lower triangular</param>
        /// <param name="transA">'N':  A * X = B  (No transpose); 'T':  A**T * X = B  (Transpose), 'T':  A**T * X = B  (Transpose)</param>
        /// <param name="diag">'N' arbitrary diagonal elements, 'U' unit diagonal</param>
        /// <param name="N">order of A</param>
        /// <param name="nrhs">number of right hand sides - columns of matrix B</param>
        /// <param name="A">square matrix A</param>
        /// <param name="LDA">spacing between columns for A</param>
        /// <param name="B">(input/output) on input: right hand side, on output: solution x </param>
        /// <param name="LDB">spacing between columns for B</param>
        /// <param name="info">(output) 0: success; &lt; 0: illigal argument, &gt; 0: A is sinular having a zero on the i-th diagonal element. No solution will be computed than. </param>
        void ctrtrs (char uplo, char transA, char diag, int N, int nrhs, IntPtr A, int LDA, IntPtr B, int LDB, ref int info); 
        /// <summary>
        /// Solve triangular system of linear equations (forward-/ backward substitution)
        /// </summary>
        /// <param name="uplo">'U': A is upper triangular, 'L': A is lower triangular</param>
        /// <param name="transA">'N':  A * X = B  (No transpose); 'T':  A**T * X = B  (Transpose), 'T':  A**T * X = B  (Transpose)</param>
        /// <param name="diag">'N' arbitrary diagonal elements, 'U' unit diagonal</param>
        /// <param name="N">order of A</param>
        /// <param name="nrhs">number of right hand sides - columns of matrix B</param>
        /// <param name="A">square matrix A</param>
        /// <param name="LDA">spacing between columns for A</param>
        /// <param name="B">(input/output) on input: right hand side, on output: solution x </param>
        /// <param name="LDB">spacing between columns for B</param>
        /// <param name="info">(output) 0: success; &lt; 0: illigal argument, &gt; 0: A is sinular having a zero on the i-th diagonal element. No solution will be computed than. </param>
        void ztrtrs (char uplo, char transA, char diag, int N, int nrhs, IntPtr A, int LDA, IntPtr B, int LDB, ref int info); 
        #endregion

        #region ?GETRS
        /// <summary>
        /// solve system of linear equations by triangular matrices
        /// </summary>
        /// <param name="trans">transpose before work?</param>
        /// <param name="N">number rows</param>
        /// <param name="NRHS">number right hand sides</param>
        /// <param name="A">matrix A</param>
        /// <param name="LDA">spacing between columns: A</param>
        /// <param name="IPIV">pivoting indices</param>
        /// <param name="B">matrix B</param>
        /// <param name="LDB">spacing between columns: B</param>
        /// <param name="info">success info</param>
        void dgetrs (char trans, int N, int NRHS, double [] A, int LDA, int [] IPIV, double [] B, int LDB, ref int info); 
        /// <summary>
        /// solve system of linear equations by triangular matrices
        /// </summary>
        /// <param name="trans">transpose before work?</param>
        /// <param name="N">number rows</param>
        /// <param name="NRHS">number right hand sides</param>
        /// <param name="A">matrix A</param>
        /// <param name="LDA">spacing between columns: A</param>
        /// <param name="IPIV">pivoting indices</param>
        /// <param name="B">matrix B</param>
        /// <param name="LDB">spacing between columns: B</param>
        /// <param name="info">success info</param>
        void sgetrs (char trans, int N, int NRHS, float [] A, int LDA, int [] IPIV, float [] B, int LDB, ref int info); 
        /// <summary>
        /// solve system of linear equations by triangular matrices
        /// </summary>
        /// <param name="trans">transpose before work?</param>
        /// <param name="N">number rows</param>
        /// <param name="NRHS">number right hand sides</param>
        /// <param name="A">matrix A</param>
        /// <param name="LDA">spacing between columns: A</param>
        /// <param name="IPIV">pivoting indices</param>
        /// <param name="B">matrix B</param>
        /// <param name="LDB">spacing between columns: B</param>
        /// <param name="info">success info</param>
        void cgetrs (char trans, int N, int NRHS, fcomplex [] A, int LDA, int [] IPIV, fcomplex [] B, int LDB, ref int info); 
        /// <summary>
        /// solve system of linear equations by triangular matrices
        /// </summary>
        /// <param name="trans">transpose before work?</param>
        /// <param name="N">number rows</param>
        /// <param name="NRHS">number right hand sides</param>
        /// <param name="A">matrix A</param>
        /// <param name="LDA">spacing between columns: A</param>
        /// <param name="IPIV">pivoting indices</param>
        /// <param name="B">matrix B</param>
        /// <param name="LDB">spacing between columns: B</param>
        /// <param name="info">success info</param>
        void zgetrs (char trans, int N, int NRHS, complex [] A, int LDA, int [] IPIV, complex [] B, int LDB, ref int info); 
        #endregion 

        #region ?GELSD - least square solution, SVD - divide & conquer
        void dgelsd (int m, int n, int nrhs, double[] A, int lda, double[] B, int ldb, double[] S, double RCond, ref int rank, ref int info); 
        void sgelsd (int m, int n, int nrhs, float[] A, int lda, float[] B, int ldb, float[] S, float RCond, ref int rank, ref int info); 
        void cgelsd (int m, int n, int nrhs, fcomplex[] A, int lda, fcomplex[] B, int ldb, float[] S, float RCond, ref int rank, ref int info); 
        void zgelsd (int m, int n, int nrhs, complex[] A, int lda, complex[] B, int ldb, double[] S, double RCond, ref int rank, ref int info); 
        #endregion 

        #region ?GELSY - least square solution, QRP
        void dgelsy (int m,int n,int nrhs, double[] A,int lda, double[] B,int ldb, int[] JPVT0, double RCond, ref int rank, ref int info); 
        void sgelsy (int m,int n,int nrhs, float[] A,int lda, float[] B,int ldb, int[] JPVT0, float RCond, ref int rank, ref int info); 
        void cgelsy (int m,int n,int nrhs, fcomplex[] A,int lda, fcomplex[] B,int ldb, int[] JPVT0, float RCond, ref int rank, ref int info); 
        void zgelsy (int m,int n,int nrhs, complex[] A,int lda, complex[] B,int ldb, int[] JPVT0, double RCond, ref int rank, ref int info); 
        #endregion 

        #region ?GEEVX - eigenvectors & -values, nonsymmetric A 
        void dgeevx (char balance, char jobvl, char jobvr, char sense, int n, double[] A, int lda, double[] wr, double[] wi, double[] vl, int ldvl, double[] vr, int ldvr, ref int ilo, ref int ihi, double[] scale, ref double abnrm, double[] rconde, double[] rcondv, ref int info); 
        void sgeevx (char balance, char jobvl, char jobvr, char sense, int n, float[] A, int lda, float[] wr, float[] wi, float[] vl, int ldvl, float[] vr, int ldvr, ref int ilo, ref int ihi, float[] scale, ref float abnrm, float[] rconde, float[] rcondv, ref int info); 
        void cgeevx (char balance, char jobvl, char jobvr, char sense, int n, fcomplex[] A, int lda, fcomplex[] w, fcomplex[] vl, int ldvl, fcomplex[] vr, int ldvr, ref int ilo, ref int ihi, float[] scale, ref float abnrm, float[] rconde, float[] rcondv, ref int info);
        void zgeevx (char balance, char jobvl, char jobvr, char sense, int n, complex[] A, int lda, complex[] w, complex[] vl, int ldvl, complex[] vr, int ldvr, ref int ilo, ref int ihi, double[] scale, ref double abnrm, double[] rconde, double[] rcondv, ref int info);
        #endregion 

        #region ?GEEVR - eigenvectors & -values, symmetric/hermitian A 
        void dsyevr (char jobz, char range, char uplo, int n, double  [] A, int lda, double vl, double vu, int il, int iu, double abstol, ref int m, double[] w, double  [] z, int ldz, int[] isuppz, ref int info); 
        void ssyevr (char jobz, char range, char uplo, int n, float   [] A, int lda, float  vl, float  vu, int il, int iu, float  abstol, ref int m, float [] w, float   [] z, int ldz, int[] isuppz, ref int info); 
        void cheevr (char jobz, char range, char uplo, int n, fcomplex[] A, int lda, float  vl, float  vu, int il, int iu, float  abstol, ref int m, float [] w, fcomplex[] z, int ldz, int[] isuppz, ref int info); 
        void zheevr (char jobz, char range, char uplo, int n, complex [] A, int lda, double vl, double vu, int il, int iu, double abstol, ref int m, double[] w, complex [] z, int ldz, int[] isuppz, ref int info); 
        #endregion 
        
        #region ?SYGV - eigenvectors & -values, symmetric/hermitian A and B, pos.def.B 
        void dsygv (int itype, char jobz, char uplo, int n, double  [] A, int lda, double  [] B, int ldb, double [] w, ref int info); 
        void ssygv (int itype, char jobz, char uplo, int n, float   [] A, int lda, float   [] B, int ldb, float  [] w, ref int info); 
        void chegv (int itype, char jobz, char uplo, int n, fcomplex[] A, int lda, fcomplex[] B, int ldb, float  [] w, ref int info); 
        void zhegv (int itype, char jobz, char uplo, int n, complex [] A, int lda, complex [] B, int ldb, double [] w, ref int info); 
        #endregion 

    }
}
