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
        /// Compute eigenvalues of general square matrix A
        /// </summary>
        /// <param name="A">Input matrix A. Size [n x n]</param>
        /// <returns>Vector of eigenvalues of A. Size [n x 1]</returns>
        /// <remarks><para>The eigenvalues of A are found by use of the Lapack functions dgeevx, sgeevx, cgeevx and zgeevx. </para>
        /// <para>The vector returned will be of complex inner type, since no further constraints are set on the structure of A (it may be nonsymmetric). Use <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;)"/> or <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;,ILOutArray&lt;double&gt;)"/> functions for computing the real eigenvalues of symmetric matrices explicitly.</para>
        /// <para>A will be balanced first. This includes permutations and scaling of A in order to improve the conditioning of the eigenvalues.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;)"/>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;,ref MatrixProperties,bool)"/>
        public static ILRetArray< complex > eig(ILInArray< double > A) {
            using (ILScope.Enter(A)) {
                ILArray< complex> V = null;
                MatrixProperties props = MatrixProperties.None;
                return eig(A, V, ref props, true);
            }
        }
        /// <summary>
        /// Compute eigenvalues and eigenvectors of general square matrix A
        /// </summary>
        /// <param name="A">Input matrix A. Size [n x n]</param>
        /// <param name="V">Output matrix, eigenvectors EV of size [n x n]. May be null on input. If not null, content of V will be destroyed.</param>
        /// <returns>Diagonal matrix with eigenvalues of A. Size [n x n]</returns>
        /// <remarks><para>The eigenvalues of A are found by use of the Lapack functions dgeevx, sgeevx, cgeevx and zgeevx. </para>
        /// <para>The matrices returned will be of complex inner type, since no further constrains are set on the structure of A (it may be nonsymmetric). Use <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;)"/> or <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;,ILOutArray&lt;double&gt;)"/> functions for computing the real eigenvalues of symmetric matrices explicitly.</para>
        /// <para>A will be balanced first. This includes permutations and scaling of A in order to improve the conditioning of the eigenvalues.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;)"/>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;,ref MatrixProperties,bool)"/>
        public static ILRetArray< complex > eig(ILInArray< double > A, ILOutArray< complex > V) {
            MatrixProperties props = MatrixProperties.None; 
            return eig(A, V, ref props, true); 
        }
        /// <summary>
        /// Find eigenvalues  and eigenvectors 
        /// </summary>
        /// <param name="A">Input: square matrix, size [n x n]</param>
        /// <param name="V">Output (optional): eigenvectors</param>   
        /// <param name="propsA">Matrix properties, on input - if specified, 
        /// will be used to choose the proper method of solution. On exit will be 
        /// filled according to the properties of A.</param>
        /// <param name="balance">true: permute A in order to increase the 
        /// numerical stability, false: do not permute A.</param>
        /// <returns>eigenvalues as vector (if V is null) or as diagonoal 
        /// matrix (if V was requested, i.e. not null).</returns>
        /// <remarks><para>The eigenvalues of A are found by use of the 
        /// Lapack functions dgeevx, sgeevx, cgeevx and zgeevx. </para>
        /// <para>The arrays returned will be of complex inner type, 
        /// since no further constraints are set on the structure of 
        /// A (it may be nonsymmetric). Use 
        /// <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;)"/> 
        /// or <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;,ILOutArray&lt;double&gt;)"/> 
        /// functions for computing the real eigenvalues of symmetric 
        /// matrices explicitly.</para>
        /// <para>Depending on the parameter <paramref name="balance"/>, 
        /// A will be balanced first. This includes permutations and 
        /// scaling of A in order to improve the conditioning of the 
        /// eigenvalues.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;)"/>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;,ref MatrixProperties,bool)"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if a 
        /// is not square</exception>
        public static ILRetArray< complex > eig(ILInArray< double > A, ILOutArray< complex > V, ref MatrixProperties propsA, bool balance) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V,null))
                        V.a = empty<complex>(A.Size); 
                    return  empty<complex>(A.Size); 
                }
                ILArray< complex > ret = empty< complex >(ILSize.Empty00);  
                int n = A.Size[0]; 
                bool createVR = (object.Equals(V,null))? false:true; 
                if (n != A.Size[1]) 
                    throw new ILArgumentException("eig: matrix A must be square");
                propsA |= MatrixProperties.Square; 
                if (((propsA & MatrixProperties.Hermitian) != 0 || ILMath.ishermitian(A.C))) {
                    propsA |= MatrixProperties.Hermitian; 
                    ILArray< double > Vd = null; 
                    if (createVR) 
                        Vd = localMember< double >();
                    ILArray< double > tmpRet = eigSymm(A,Vd);
                    if (createVR)
                        V.a =  ILMath.tocomplex (Vd); 
                    ret =  ILMath.tocomplex (tmpRet);
                } else {
                    // nonsymmetric case
                    char bal = (balance)? 'B':'N', jobvr;  
                    ILArray< double > tmpA = A.C; 
                     double [] vr = null;
                     double [] wr = ILMemoryPool.Pool.New< double >(n); 
                     
                    double[] wi = ILMemoryPool.Pool.New<double>(n); 
                     double [] scale  = ILMemoryPool.Pool.New< double >(n);
                     double [] rconde = ILMemoryPool.Pool.New< double >(n); 
                     double [] rcondv = ILMemoryPool.Pool.New< double >(n); 
                     double abnrm = 0; 
                    int ldvr, ilo = 0, ihi = 0, info = 0;
                    if (createVR) {
                        ldvr = n;
                        vr = ILMemoryPool.Pool.New< double >(n * n);
                        jobvr = 'V'; 
                    } else {
                        ldvr = 1; 
                        vr = new  double [1]; 
                        jobvr = 'N'; 
                    }
                    /*!HC:HC?geevx*/
                    Lapack.dgeevx(bal,'N',jobvr,'N',n,tmpA.GetArrayForWrite(),n,wr,wi,new  double [1],1,vr,ldvr,ref ilo,ref ihi,scale,ref abnrm,rconde,rcondv,ref info);   
                    ILMemoryPool.Pool.Free(rconde);
                    ILMemoryPool.Pool.Free(rcondv);
                    ILMemoryPool.Pool.Free(scale);
                    if (info != 0) 
                        throw new ILArgumentException("eig: error in Lapack '?geevx': (" + info + ")");
                    // create eigenvalues 
                     complex [] retArr = ILMemoryPool.Pool.New< complex >(n); 
                    for (int i = 0; i < n; i++) {
                        
                        retArr[i].real = wr[i]; retArr[i].imag = wi[i]; 
                    }
                    ret = new ILArray< complex > (retArr,n,1);
                    if (createVR) {
                        #region HCSortEVec
                        complex [] VArr = ILMemoryPool.Pool.New< complex >(n * n);
                        for (int c = 0; c < n; c++) {
                            if (wi[c] != 0 && wi[c+1] != 0 && c < n-1) {
                                ilo = n * c; ihi = ilo + n;
                                for (int r = 0; r < n; r++) {
                                    VArr[ilo].real = vr[ilo];
                                    VArr[ilo].imag = vr[ihi];  
                                    VArr[ihi].real = vr[ilo];  
                                    VArr[ihi].imag = -vr[ihi]; 
                                    ilo++; ihi++; 
                                }
                                c++; 
                            } else {
                                ilo = n * c;
                                for (int r = 0; r < n; r++) {
                                    VArr[ilo].real = vr[ilo];
                                    ilo++; 
                                }
                            }
                        }
                        V.a = array<complex> (VArr,n,n); 
                        #endregion HYCALPER
                        if (createVR)
                            ret.a = ILMath.diag< complex>(ret); 
                    }
                    
                    ILMemoryPool.Pool.Free(wi); 
                    ILMemoryPool.Pool.Free(vr); 
                    ILMemoryPool.Pool.Free(wr); 
                }
                return ret; 
            }
        }

        /// <summary>
        /// Find all eigenvalues of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <returns>Array of size [n,1] with eigenvalues of A.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square.</exception>
        public static ILRetArray< double > eigSymm (ILInArray< double > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return empty< double>(A.Size); 
                }
                int n = A.Size[0]; 
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian.");
                int m = 0,info = 0; 
                ILArray< double > w = new ILArray< double > (new  double [n],1,n); 
                 double [] z = new  double [1]; ; 
                int [] isuppz = new int[2 * n];
                ILArray< double> Acopy = A.C;  
                 double [] AcArr = Acopy.GetArrayForWrite(); 
                /*!HC:lapack_???evr*/ Lapack.dsyevr ('N','A','U',n,AcArr,n,0,0,0,0,0,ref m,w.GetArrayForWrite(),z,1,isuppz,ref info);
                ILMemoryPool.Pool.Free(AcArr); 
                return  /*dummy*/ (w); 
            }
        }
        /// <summary>
        /// Find all eigenvalues and -vectors of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <param name="V">Output: n eigenvectors as columns. Size [n x n]. If V is null on input, the eigenvectors 
        /// will not be computed and V is not changed. In order to make the function return the vectors, V should be initialized with any non null value (e.g.'1') before calling eigSymm.</param>
        /// <returns>Diagonal matrix of size [n,n] with eigenvalues of A on the main diagonal.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square.</exception>
        public static ILRetArray< double > eigSymm (ILInArray< double > A, ILOutArray< double > V) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V,null))
                        V.a = empty<double>(A.Size); 
                    return empty<double>(A.Size); 
                }
                int n = A.Size[0]; 
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian.");
                int m = 0,ldz = 0,info = 0; 
                ILArray< double > w = new ILArray< double >(ILMemoryPool.Pool.New< double >(n),n,1); 
                 double [] z; 
                char jobz;
                if (object.Equals(V,null)) {
                    z = new  double [1]; 
                    jobz = 'N';
                    ldz = 1; 
                } else {
                    z = ILMemoryPool.Pool.New<  double >(n * n);
                    jobz = 'V';
                    ldz = n; 
                } 
                int [] isuppz = ILMemoryPool.Pool.New<int>( 2 * n);
                ILArray< double> Acopy = A.C;
                
                double[] AcArr = Acopy.GetArrayForWrite(); 
                /*!HC:lapack_???evr*/ Lapack.dsyevr (jobz,'A','U',n,AcArr,n,1,n,0,0,0,ref m,w.GetArrayForWrite(),z,ldz,isuppz,ref info);
                ILMemoryPool.Pool.Free(AcArr); 
                ILMemoryPool.Pool.Free(isuppz);
                if (info != 0) 
                    throw new ILException("error returned from lapack: " + info); 
                if (jobz == 'V') {
                    System.Diagnostics.Debug.Assert(!object.Equals(V,null));
                    V.a =  array< double > (z,n,n);
                    V.a = V[full,r(0,m-1)]; 
                    return ILMath.diag( /*dummy*/ (w)); 
                } else {
                    ILMemoryPool.Pool.Free(z); 
                    return  /*dummy*/ (w);
                }
            }
        }
        /// <summary>
        /// Find some eigenvalues and -vectors of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <param name="V">Output: n eigenvectors as columns. Size [n x n]. If V is null on input, the eigenvectors will not be computed and V is not changed. </param>
        /// <param name="rangeStart">Specify the lowest limit for the range of eigenvalues to be queried.</param>
        /// <param name="rangeEnd">Specify the upper limit for the range of eigenvalues to be queried.</param>
        /// <returns>Diagonal matrix of size [n,n] with eigenvalues of A on the main diagonal.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square or <paramref name="rangeEnd"/> &lt; <paramref name="rangeStart"/></exception>
        public static ILRetArray< double > eigSymm (ILInArray< double > A, ILOutArray< double > V, int rangeStart, int rangeEnd) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V,null))
                        V.a = empty<double>(A.Size); 
                    return empty<double>(A.Size); 
                }
                int n = A.Size[0]; 
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian.");
                int m = 0,ldz = 0,info = 0; 
                if (rangeEnd < rangeStart || rangeStart < 1) 
                    throw new ILArgumentException("invalid range of eigenvalues requested");
                ILArray< double > w = array< double > (
                                            ILMemoryPool.Pool.New< double>(n),1,n); 
                 double [] z; 
                char jobz;
                if (object.Equals(V,null)) {
                    z = new  double [1]; 
                    jobz = 'N';
                    ldz = 1; 
                } else {
                    z = ILMemoryPool.Pool.New<double>(n * n);
                    jobz = 'V';
                    ldz = n; 
                } 
                int [] isuppz = ILMemoryPool.Pool.New<int>(2 * n);
                ILArray< double> Acopy = A.C;
                
                double[] AcArr = Acopy.GetArrayForWrite(); 
                /*!HC:lapack_???evr*/ Lapack.dsyevr (jobz,'I','U',n,AcArr,n,0,0,rangeStart,rangeEnd,0,ref m,w.GetArrayForWrite(),z,ldz,isuppz,ref info);
                ILMemoryPool.Pool.Free(isuppz); 
                ILMemoryPool.Pool.Free(AcArr); 

                if (jobz == 'V') {
                    V.a = array<  double >(z,n,n);
                    V.a = V[full,r(0,m-1)]; 
                }
                ILMemoryPool.Pool.Free(z); 
                return ILMath.diag( /*dummy*/ (w)); 
            }
        }
        /// <summary>
        /// Find some eigenvalues and -vectors of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <param name="V">Output: n eigenvectors as columns. Size [n x n]. If V is null on input, the eigenvectors will not be computed and V is not changed. </param>
        /// <param name="rangeStart">The eigenvalues will be returned by increasing size. This will determine the number of the first eigenvalue to be returned.</param>
        /// <param name="rangeEnd">Determine the number of the last eigenvalue to be returned.</param>
        /// <returns>Diagonal matrix of size [n,n] with eigenvalues of A on the main diagonal.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square or <paramref name="rangeEnd"/> &lt; <paramref name="rangeStart"/> or if either one is &lt;= 0.</exception>
        public static ILRetArray< double> eigSymm( ILInArray< double> A, ILOutArray< double> V,  double rangeStart,  double rangeEnd ) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V, null))
                        V.a = empty<double>(A.Size);
                    return empty<double>(A.Size);
                }
                int n = A.Size[0];
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian");
                int m = 0, ldz = 0, info = 0;
                if (rangeStart > rangeEnd)
                    throw new ILArgumentException("invalid range of eigenvalues requested");
                ILArray< double> w = zeros<double>(1, n);
                
                double[] z;
                char jobz;
                if (object.Equals(V, null)) {
                    z = new  double[1];
                    jobz = 'N';
                    ldz = 1;
                } else {
                    z = ILMemoryPool.Pool.New<double>(n * n);
                    jobz = 'V';
                    ldz = n;
                }
                int[] isuppz = ILMemoryPool.Pool.New<int>(2 * n);

                ILArray< double> Acopy = A.C;
                
                double[] AcArr = Acopy.GetArrayForWrite();
                /*!HC:lapack_???evr*/
                Lapack.dsyevr(jobz, 'V', 'U', n, AcArr, n, rangeStart, rangeEnd, 0, 0, 0, ref m, w.GetArrayForWrite(), z, ldz, isuppz, ref info);
                ILMemoryPool.Pool.Free(AcArr);
                ILMemoryPool.Pool.Free(isuppz);

                if (jobz == 'V') {
                    V.a = array< double>(z, n, n);
                    V.a = V[full, r(0, m - 1)];
                }
                ILMemoryPool.Pool.Free(z);
                return diag( /*dummy*/ (w));
            }
        }

#region HYCALPER AUTO GENERATED CODE

        
        /// <summary>
        /// Compute eigenvalues of general square matrix A
        /// </summary>
        /// <param name="A">Input matrix A. Size [n x n]</param>
        /// <returns>Vector of eigenvalues of A. Size [n x 1]</returns>
        /// <remarks><para>The eigenvalues of A are found by use of the Lapack functions dgeevx, sgeevx, cgeevx and zgeevx. </para>
        /// <para>The vector returned will be of complex inner type, since no further constraints are set on the structure of A (it may be nonsymmetric). Use <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;)"/> or <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;,ILOutArray&lt;double&gt;)"/> functions for computing the real eigenvalues of symmetric matrices explicitly.</para>
        /// <para>A will be balanced first. This includes permutations and scaling of A in order to improve the conditioning of the eigenvalues.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;)"/>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;,ref MatrixProperties,bool)"/>
        public static ILRetArray< fcomplex > eig(ILInArray< float > A) {
            using (ILScope.Enter(A)) {
                ILArray< fcomplex> V = null;
                MatrixProperties props = MatrixProperties.None;
                return eig(A, V, ref props, true);
            }
        }
        /// <summary>
        /// Compute eigenvalues and eigenvectors of general square matrix A
        /// </summary>
        /// <param name="A">Input matrix A. Size [n x n]</param>
        /// <param name="V">Output matrix, eigenvectors EV of size [n x n]. May be null on input. If not null, content of V will be destroyed.</param>
        /// <returns>Diagonal matrix with eigenvalues of A. Size [n x n]</returns>
        /// <remarks><para>The eigenvalues of A are found by use of the Lapack functions dgeevx, sgeevx, cgeevx and zgeevx. </para>
        /// <para>The matrices returned will be of complex inner type, since no further constrains are set on the structure of A (it may be nonsymmetric). Use <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;)"/> or <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;,ILOutArray&lt;double&gt;)"/> functions for computing the real eigenvalues of symmetric matrices explicitly.</para>
        /// <para>A will be balanced first. This includes permutations and scaling of A in order to improve the conditioning of the eigenvalues.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;)"/>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;,ref MatrixProperties,bool)"/>
        public static ILRetArray< fcomplex > eig(ILInArray< float > A, ILOutArray< fcomplex > V) {
            MatrixProperties props = MatrixProperties.None; 
            return eig(A, V, ref props, true); 
        }
        /// <summary>
        /// Find eigenvalues  and eigenvectors 
        /// </summary>
        /// <param name="A">Input: square matrix, size [n x n]</param>
        /// <param name="V">Output (optional): eigenvectors</param>   
        /// <param name="propsA">Matrix properties, on input - if specified, 
        /// will be used to choose the proper method of solution. On exit will be 
        /// filled according to the properties of A.</param>
        /// <param name="balance">true: permute A in order to increase the 
        /// numerical stability, false: do not permute A.</param>
        /// <returns>eigenvalues as vector (if V is null) or as diagonoal 
        /// matrix (if V was requested, i.e. not null).</returns>
        /// <remarks><para>The eigenvalues of A are found by use of the 
        /// Lapack functions dgeevx, sgeevx, cgeevx and zgeevx. </para>
        /// <para>The arrays returned will be of complex inner type, 
        /// since no further constraints are set on the structure of 
        /// A (it may be nonsymmetric). Use 
        /// <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;)"/> 
        /// or <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;,ILOutArray&lt;double&gt;)"/> 
        /// functions for computing the real eigenvalues of symmetric 
        /// matrices explicitly.</para>
        /// <para>Depending on the parameter <paramref name="balance"/>, 
        /// A will be balanced first. This includes permutations and 
        /// scaling of A in order to improve the conditioning of the 
        /// eigenvalues.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;)"/>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;,ref MatrixProperties,bool)"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if a 
        /// is not square</exception>
        public static ILRetArray< fcomplex > eig(ILInArray< float > A, ILOutArray< fcomplex > V, ref MatrixProperties propsA, bool balance) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V,null))
                        V.a = empty<fcomplex>(A.Size); 
                    return  empty<fcomplex>(A.Size); 
                }
                ILArray< fcomplex > ret = empty< fcomplex >(ILSize.Empty00);  
                int n = A.Size[0]; 
                bool createVR = (object.Equals(V,null))? false:true; 
                if (n != A.Size[1]) 
                    throw new ILArgumentException("eig: matrix A must be square");
                propsA |= MatrixProperties.Square; 
                if (((propsA & MatrixProperties.Hermitian) != 0 || ILMath.ishermitian(A.C))) {
                    propsA |= MatrixProperties.Hermitian; 
                    ILArray< float > Vd = null; 
                    if (createVR) 
                        Vd = localMember< float >();
                    ILArray< float > tmpRet = eigSymm(A,Vd);
                    if (createVR)
                        V.a =  ILMath.tofcomplex (Vd); 
                    ret =  ILMath.tofcomplex (tmpRet);
                } else {
                    // nonsymmetric case
                    char bal = (balance)? 'B':'N', jobvr;  
                    ILArray< float > tmpA = A.C; 
                    float [] vr = null;
                    float [] wr = ILMemoryPool.Pool.New< float >(n); 
                    float[] wi = ILMemoryPool.Pool.New< float>(n);
                    float [] scale  = ILMemoryPool.Pool.New< float >(n);
                    float [] rconde = ILMemoryPool.Pool.New< float >(n); 
                    float [] rcondv = ILMemoryPool.Pool.New< float >(n); 
                    float abnrm = 0; 
                    int ldvr, ilo = 0, ihi = 0, info = 0;
                    if (createVR) {
                        ldvr = n;
                        vr = ILMemoryPool.Pool.New< float >(n * n);
                        jobvr = 'V'; 
                    } else {
                        ldvr = 1; 
                        vr = new  float [1]; 
                        jobvr = 'N'; 
                    }
                    Lapack.sgeevx(bal,'N',jobvr,'N',n,tmpA.GetArrayForWrite(),n,wr,wi,new float   [1],1,vr,ldvr,ref ilo,ref ihi,scale,ref abnrm,rconde,rcondv,ref info);   
                    ILMemoryPool.Pool.Free(rconde);
                    ILMemoryPool.Pool.Free(rcondv);
                    ILMemoryPool.Pool.Free(scale);
                    if (info != 0) 
                        throw new ILArgumentException("eig: error in Lapack '?geevx': (" + info + ")");
                    // create eigenvalues 
                    fcomplex [] retArr = ILMemoryPool.Pool.New< fcomplex >(n); 
                    for (int i = 0; i < n; i++) {
                        retArr[i].real = wr[i]; retArr[i].imag = wi[i];
                    }
                    ret = new ILArray< fcomplex > (retArr,n,1);
                    if (createVR) {
                        fcomplex [] VArr = ILMemoryPool.Pool.New< fcomplex >(n * n);
                    for (int c = 0; c < n; c++) {
                        if (wi[c] != 0 && wi[c+1] != 0 && c < n-1) {
                            ilo = n * c; ihi = ilo + n;
                            for (int r = 0; r < n; r++) {
                                VArr[ilo].real = vr[ilo];
                                VArr[ilo].imag = vr[ihi];  
                                VArr[ihi].real = vr[ilo];  
                                VArr[ihi].imag = -vr[ihi]; 
                                ilo++; ihi++; 
                            }
                            c++; 
                        } else {
                            ilo = n * c;
                            for (int r = 0; r < n; r++) {
                                VArr[ilo].real = vr[ilo];
                                ilo++; 
                            }
                        }
                    }
                    V.a = array<fcomplex> (VArr,n,n);  
                        if (createVR)
                            ret.a = ILMath.diag< fcomplex>(ret); 
                    }
                    ILMemoryPool.Pool.Free(wi);
                    ILMemoryPool.Pool.Free(vr); 
                    ILMemoryPool.Pool.Free(wr); 
                }
                return ret; 
            }
        }

        /// <summary>
        /// Find all eigenvalues of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <returns>Array of size [n,1] with eigenvalues of A.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square.</exception>
        public static ILRetArray< float > eigSymm (ILInArray< float > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return empty< float>(A.Size); 
                }
                int n = A.Size[0]; 
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian.");
                int m = 0,info = 0; 
                ILArray< float > w = new ILArray< float > (new  float [n],1,n); 
                float [] z = new  float [1]; ; 
                int [] isuppz = new int[2 * n];
                ILArray< float> Acopy = A.C;  
                float [] AcArr = Acopy.GetArrayForWrite(); 
                Lapack.ssyevr ('N','A','U',n,AcArr,n,0,0,0,0,0,ref m,w.GetArrayForWrite(),z,1,isuppz,ref info);
                ILMemoryPool.Pool.Free(AcArr); 
                return  (w); 
            }
        }
        /// <summary>
        /// Find all eigenvalues and -vectors of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <param name="V">Output: n eigenvectors as columns. Size [n x n]. If V is null on input, the eigenvectors 
        /// will not be computed and V is not changed. In order to make the function return the vectors, V should be initialized with any non null value (e.g.'1') before calling eigSymm.</param>
        /// <returns>Diagonal matrix of size [n,n] with eigenvalues of A on the main diagonal.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square.</exception>
        public static ILRetArray< float > eigSymm (ILInArray< float > A, ILOutArray< float > V) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V,null))
                        V.a = empty<float>(A.Size); 
                    return empty<float>(A.Size); 
                }
                int n = A.Size[0]; 
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian.");
                int m = 0,ldz = 0,info = 0; 
                ILArray< float > w = new ILArray< float >(ILMemoryPool.Pool.New< float >(n),n,1); 
                float [] z; 
                char jobz;
                if (object.Equals(V,null)) {
                    z = new  float [1]; 
                    jobz = 'N';
                    ldz = 1; 
                } else {
                    z = ILMemoryPool.Pool.New<  float >(n * n);
                    jobz = 'V';
                    ldz = n; 
                } 
                int [] isuppz = ILMemoryPool.Pool.New<int>( 2 * n);
                ILArray< float> Acopy = A.C;
               
                float[] AcArr = Acopy.GetArrayForWrite(); 
                Lapack.ssyevr (jobz,'A','U',n,AcArr,n,1,n,0,0,0,ref m,w.GetArrayForWrite(),z,ldz,isuppz,ref info);
                ILMemoryPool.Pool.Free(AcArr); 
                ILMemoryPool.Pool.Free(isuppz);
                if (info != 0) 
                    throw new ILException("error returned from lapack: " + info); 
                if (jobz == 'V') {
                    System.Diagnostics.Debug.Assert(!object.Equals(V,null));
                    V.a =  array< float > (z,n,n);
                    V.a = V[full,r(0,m-1)]; 
                    return ILMath.diag( (w)); 
                } else {
                    ILMemoryPool.Pool.Free(z); 
                    return  (w);
                }
            }
        }
        /// <summary>
        /// Find some eigenvalues and -vectors of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <param name="V">Output: n eigenvectors as columns. Size [n x n]. If V is null on input, the eigenvectors will not be computed and V is not changed. </param>
        /// <param name="rangeStart">Specify the lowest limit for the range of eigenvalues to be queried.</param>
        /// <param name="rangeEnd">Specify the upper limit for the range of eigenvalues to be queried.</param>
        /// <returns>Diagonal matrix of size [n,n] with eigenvalues of A on the main diagonal.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square or <paramref name="rangeEnd"/> &lt; <paramref name="rangeStart"/></exception>
        public static ILRetArray< float > eigSymm (ILInArray< float > A, ILOutArray< float > V, int rangeStart, int rangeEnd) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V,null))
                        V.a = empty<float>(A.Size); 
                    return empty<float>(A.Size); 
                }
                int n = A.Size[0]; 
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian.");
                int m = 0,ldz = 0,info = 0; 
                if (rangeEnd < rangeStart || rangeStart < 1) 
                    throw new ILArgumentException("invalid range of eigenvalues requested");
                ILArray< float > w = array< float > (
                                            ILMemoryPool.Pool.New< float>(n),1,n); 
                float [] z; 
                char jobz;
                if (object.Equals(V,null)) {
                    z = new  float [1]; 
                    jobz = 'N';
                    ldz = 1; 
                } else {
                    z = ILMemoryPool.Pool.New<float>(n * n);
                    jobz = 'V';
                    ldz = n; 
                } 
                int [] isuppz = ILMemoryPool.Pool.New<int>(2 * n);
                ILArray< float> Acopy = A.C;
               
                float[] AcArr = Acopy.GetArrayForWrite(); 
                Lapack.ssyevr (jobz,'I','U',n,AcArr,n,0,0,rangeStart,rangeEnd,0,ref m,w.GetArrayForWrite(),z,ldz,isuppz,ref info);
                ILMemoryPool.Pool.Free(isuppz); 
                ILMemoryPool.Pool.Free(AcArr); 

                if (jobz == 'V') {
                    V.a = array<  float >(z,n,n);
                    V.a = V[full,r(0,m-1)]; 
                }
                ILMemoryPool.Pool.Free(z); 
                return ILMath.diag( (w)); 
            }
        }
        /// <summary>
        /// Find some eigenvalues and -vectors of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <param name="V">Output: n eigenvectors as columns. Size [n x n]. If V is null on input, the eigenvectors will not be computed and V is not changed. </param>
        /// <param name="rangeStart">The eigenvalues will be returned by increasing size. This will determine the number of the first eigenvalue to be returned.</param>
        /// <param name="rangeEnd">Determine the number of the last eigenvalue to be returned.</param>
        /// <returns>Diagonal matrix of size [n,n] with eigenvalues of A on the main diagonal.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square or <paramref name="rangeEnd"/> &lt; <paramref name="rangeStart"/> or if either one is &lt;= 0.</exception>
        public static ILRetArray< float> eigSymm( ILInArray< float> A, ILOutArray< float> V,  float rangeStart,  float rangeEnd ) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V, null))
                        V.a = empty<float>(A.Size);
                    return empty<float>(A.Size);
                }
                int n = A.Size[0];
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian");
                int m = 0, ldz = 0, info = 0;
                if (rangeStart > rangeEnd)
                    throw new ILArgumentException("invalid range of eigenvalues requested");
                ILArray< float> w = zeros<float>(1, n);
               
                float[] z;
                char jobz;
                if (object.Equals(V, null)) {
                    z = new  float[1];
                    jobz = 'N';
                    ldz = 1;
                } else {
                    z = ILMemoryPool.Pool.New<float>(n * n);
                    jobz = 'V';
                    ldz = n;
                }
                int[] isuppz = ILMemoryPool.Pool.New<int>(2 * n);

                ILArray< float> Acopy = A.C;
               
                float[] AcArr = Acopy.GetArrayForWrite();
               
                Lapack.ssyevr(jobz, 'V', 'U', n, AcArr, n, rangeStart, rangeEnd, 0, 0, 0, ref m, w.GetArrayForWrite(), z, ldz, isuppz, ref info);
                ILMemoryPool.Pool.Free(AcArr);
                ILMemoryPool.Pool.Free(isuppz);

                if (jobz == 'V') {
                    V.a = array< float>(z, n, n);
                    V.a = V[full, r(0, m - 1)];
                }
                ILMemoryPool.Pool.Free(z);
                return diag( (w));
            }
        }
        
        /// <summary>
        /// Compute eigenvalues of general square matrix A
        /// </summary>
        /// <param name="A">Input matrix A. Size [n x n]</param>
        /// <returns>Vector of eigenvalues of A. Size [n x 1]</returns>
        /// <remarks><para>The eigenvalues of A are found by use of the Lapack functions dgeevx, sgeevx, cgeevx and zgeevx. </para>
        /// <para>The vector returned will be of complex inner type, since no further constraints are set on the structure of A (it may be nonsymmetric). Use <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;)"/> or <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;,ILOutArray&lt;double&gt;)"/> functions for computing the real eigenvalues of symmetric matrices explicitly.</para>
        /// <para>A will be balanced first. This includes permutations and scaling of A in order to improve the conditioning of the eigenvalues.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;)"/>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;,ref MatrixProperties,bool)"/>
        public static ILRetArray< fcomplex > eig(ILInArray< fcomplex > A) {
            using (ILScope.Enter(A)) {
                ILArray< fcomplex> V = null;
                MatrixProperties props = MatrixProperties.None;
                return eig(A, V, ref props, true);
            }
        }
        /// <summary>
        /// Compute eigenvalues and eigenvectors of general square matrix A
        /// </summary>
        /// <param name="A">Input matrix A. Size [n x n]</param>
        /// <param name="V">Output matrix, eigenvectors EV of size [n x n]. May be null on input. If not null, content of V will be destroyed.</param>
        /// <returns>Diagonal matrix with eigenvalues of A. Size [n x n]</returns>
        /// <remarks><para>The eigenvalues of A are found by use of the Lapack functions dgeevx, sgeevx, cgeevx and zgeevx. </para>
        /// <para>The matrices returned will be of complex inner type, since no further constrains are set on the structure of A (it may be nonsymmetric). Use <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;)"/> or <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;,ILOutArray&lt;double&gt;)"/> functions for computing the real eigenvalues of symmetric matrices explicitly.</para>
        /// <para>A will be balanced first. This includes permutations and scaling of A in order to improve the conditioning of the eigenvalues.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;)"/>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;,ref MatrixProperties,bool)"/>
        public static ILRetArray< fcomplex > eig(ILInArray< fcomplex > A, ILOutArray< fcomplex > V) {
            MatrixProperties props = MatrixProperties.None; 
            return eig(A, V, ref props, true); 
        }
        /// <summary>
        /// Find eigenvalues  and eigenvectors 
        /// </summary>
        /// <param name="A">Input: square matrix, size [n x n]</param>
        /// <param name="V">Output (optional): eigenvectors</param>   
        /// <param name="propsA">Matrix properties, on input - if specified, 
        /// will be used to choose the proper method of solution. On exit will be 
        /// filled according to the properties of A.</param>
        /// <param name="balance">true: permute A in order to increase the 
        /// numerical stability, false: do not permute A.</param>
        /// <returns>eigenvalues as vector (if V is null) or as diagonoal 
        /// matrix (if V was requested, i.e. not null).</returns>
        /// <remarks><para>The eigenvalues of A are found by use of the 
        /// Lapack functions dgeevx, sgeevx, cgeevx and zgeevx. </para>
        /// <para>The arrays returned will be of complex inner type, 
        /// since no further constraints are set on the structure of 
        /// A (it may be nonsymmetric). Use 
        /// <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;)"/> 
        /// or <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;,ILOutArray&lt;double&gt;)"/> 
        /// functions for computing the real eigenvalues of symmetric 
        /// matrices explicitly.</para>
        /// <para>Depending on the parameter <paramref name="balance"/>, 
        /// A will be balanced first. This includes permutations and 
        /// scaling of A in order to improve the conditioning of the 
        /// eigenvalues.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;)"/>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;,ref MatrixProperties,bool)"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if a 
        /// is not square</exception>
        public static ILRetArray< fcomplex > eig(ILInArray< fcomplex > A, ILOutArray< fcomplex > V, ref MatrixProperties propsA, bool balance) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V,null))
                        V.a = empty<fcomplex>(A.Size); 
                    return  empty<fcomplex>(A.Size); 
                }
                ILArray< fcomplex > ret = empty< fcomplex >(ILSize.Empty00);  
                int n = A.Size[0]; 
                bool createVR = (object.Equals(V,null))? false:true; 
                if (n != A.Size[1]) 
                    throw new ILArgumentException("eig: matrix A must be square");
                propsA |= MatrixProperties.Square; 
                if (((propsA & MatrixProperties.Hermitian) != 0 || ILMath.ishermitian(A.C))) {
                    propsA |= MatrixProperties.Hermitian; 
                    ILArray< fcomplex > Vd = null; 
                    if (createVR) 
                        Vd = localMember< fcomplex >();
                    ILArray< fcomplex > tmpRet = eigSymm(A,Vd);
                    if (createVR)
                        V.a =   (Vd); 
                    ret =   (tmpRet);
                } else {
                    // nonsymmetric case
                    char bal = (balance)? 'B':'N', jobvr;  
                    ILArray< fcomplex > tmpA = A.C; 
                    fcomplex [] vr = null;
                    fcomplex [] wr = ILMemoryPool.Pool.New< fcomplex >(n); 
                    
                    float [] scale  = ILMemoryPool.Pool.New< float >(n);
                    float [] rconde = ILMemoryPool.Pool.New< float >(n); 
                    float [] rcondv = ILMemoryPool.Pool.New< float >(n); 
                    float abnrm = 0; 
                    int ldvr, ilo = 0, ihi = 0, info = 0;
                    if (createVR) {
                        ldvr = n;
                        vr = ILMemoryPool.Pool.New< fcomplex >(n * n);
                        jobvr = 'V'; 
                    } else {
                        ldvr = 1; 
                        vr = new  fcomplex [1]; 
                        jobvr = 'N'; 
                    }
                    Lapack.cgeevx(bal,'N',jobvr,'N',n,tmpA.GetArrayForWrite(),n,wr,   new fcomplex[1],1,vr,ldvr,ref ilo,ref ihi,scale,ref abnrm,rconde,rcondv,ref info);   
                    ILMemoryPool.Pool.Free(rconde);
                    ILMemoryPool.Pool.Free(rcondv);
                    ILMemoryPool.Pool.Free(scale);
                    if (info != 0) 
                        throw new ILArgumentException("eig: error in Lapack '?geevx': (" + info + ")");
                    // create eigenvalues 
                    fcomplex [] retArr = ILMemoryPool.Pool.New< fcomplex >(n); 
                    for (int i = 0; i < n; i++) {
                        retArr[i] = wr[i]; 
                    }
                    ret = new ILArray< fcomplex > (retArr,n,1);
                    if (createVR) {
                        V.a = array<fcomplex> (vr,n,n);
                        if (createVR)
                            ret.a = ILMath.diag< fcomplex>(ret); 
                    }
                    
                    ILMemoryPool.Pool.Free(vr); 
                    ILMemoryPool.Pool.Free(wr); 
                }
                return ret; 
            }
        }

        /// <summary>
        /// Find all eigenvalues of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <returns>Array of size [n,1] with eigenvalues of A.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square.</exception>
        public static ILRetArray< fcomplex > eigSymm (ILInArray< fcomplex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return empty< fcomplex>(A.Size); 
                }
                int n = A.Size[0]; 
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian.");
                int m = 0,info = 0; 
                ILArray< float > w = new ILArray< float > (new  float [n],1,n); 
                fcomplex [] z = new  fcomplex [1]; ; 
                int [] isuppz = new int[2 * n];
                ILArray< fcomplex> Acopy = A.C;  
                fcomplex [] AcArr = Acopy.GetArrayForWrite(); 
                Lapack.cheevr ('N','A','U',n,AcArr,n,0,0,0,0,0,ref m,w.GetArrayForWrite(),z,1,isuppz,ref info);
                ILMemoryPool.Pool.Free(AcArr); 
                return  ILMath.tofcomplex(w); 
            }
        }
        /// <summary>
        /// Find all eigenvalues and -vectors of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <param name="V">Output: n eigenvectors as columns. Size [n x n]. If V is null on input, the eigenvectors 
        /// will not be computed and V is not changed. In order to make the function return the vectors, V should be initialized with any non null value (e.g.'1') before calling eigSymm.</param>
        /// <returns>Diagonal matrix of size [n,n] with eigenvalues of A on the main diagonal.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square.</exception>
        public static ILRetArray< fcomplex > eigSymm (ILInArray< fcomplex > A, ILOutArray< fcomplex > V) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V,null))
                        V.a = empty<fcomplex>(A.Size); 
                    return empty<fcomplex>(A.Size); 
                }
                int n = A.Size[0]; 
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian.");
                int m = 0,ldz = 0,info = 0; 
                ILArray< float > w = new ILArray< float >(ILMemoryPool.Pool.New< float >(n),n,1); 
                fcomplex [] z; 
                char jobz;
                if (object.Equals(V,null)) {
                    z = new  fcomplex [1]; 
                    jobz = 'N';
                    ldz = 1; 
                } else {
                    z = ILMemoryPool.Pool.New<  fcomplex >(n * n);
                    jobz = 'V';
                    ldz = n; 
                } 
                int [] isuppz = ILMemoryPool.Pool.New<int>( 2 * n);
                ILArray< fcomplex> Acopy = A.C;
               
                fcomplex[] AcArr = Acopy.GetArrayForWrite(); 
                Lapack.cheevr (jobz,'A','U',n,AcArr,n,1,n,0,0,0,ref m,w.GetArrayForWrite(),z,ldz,isuppz,ref info);
                ILMemoryPool.Pool.Free(AcArr); 
                ILMemoryPool.Pool.Free(isuppz);
                if (info != 0) 
                    throw new ILException("error returned from lapack: " + info); 
                if (jobz == 'V') {
                    System.Diagnostics.Debug.Assert(!object.Equals(V,null));
                    V.a =  array< fcomplex > (z,n,n);
                    V.a = V[full,r(0,m-1)]; 
                    return ILMath.diag( ILMath.tofcomplex(w)); 
                } else {
                    ILMemoryPool.Pool.Free(z); 
                    return  ILMath.tofcomplex(w);
                }
            }
        }
        /// <summary>
        /// Find some eigenvalues and -vectors of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <param name="V">Output: n eigenvectors as columns. Size [n x n]. If V is null on input, the eigenvectors will not be computed and V is not changed. </param>
        /// <param name="rangeStart">Specify the lowest limit for the range of eigenvalues to be queried.</param>
        /// <param name="rangeEnd">Specify the upper limit for the range of eigenvalues to be queried.</param>
        /// <returns>Diagonal matrix of size [n,n] with eigenvalues of A on the main diagonal.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square or <paramref name="rangeEnd"/> &lt; <paramref name="rangeStart"/></exception>
        public static ILRetArray< fcomplex > eigSymm (ILInArray< fcomplex > A, ILOutArray< fcomplex > V, int rangeStart, int rangeEnd) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V,null))
                        V.a = empty<fcomplex>(A.Size); 
                    return empty<fcomplex>(A.Size); 
                }
                int n = A.Size[0]; 
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian.");
                int m = 0,ldz = 0,info = 0; 
                if (rangeEnd < rangeStart || rangeStart < 1) 
                    throw new ILArgumentException("invalid range of eigenvalues requested");
                ILArray< float > w = array< float > (
                                            ILMemoryPool.Pool.New< float>(n),1,n); 
                fcomplex [] z; 
                char jobz;
                if (object.Equals(V,null)) {
                    z = new  fcomplex [1]; 
                    jobz = 'N';
                    ldz = 1; 
                } else {
                    z = ILMemoryPool.Pool.New<fcomplex>(n * n);
                    jobz = 'V';
                    ldz = n; 
                } 
                int [] isuppz = ILMemoryPool.Pool.New<int>(2 * n);
                ILArray< fcomplex> Acopy = A.C;
               
                fcomplex[] AcArr = Acopy.GetArrayForWrite(); 
                Lapack.cheevr (jobz,'I','U',n,AcArr,n,0,0,rangeStart,rangeEnd,0,ref m,w.GetArrayForWrite(),z,ldz,isuppz,ref info);
                ILMemoryPool.Pool.Free(isuppz); 
                ILMemoryPool.Pool.Free(AcArr); 

                if (jobz == 'V') {
                    V.a = array<  fcomplex >(z,n,n);
                    V.a = V[full,r(0,m-1)]; 
                }
                ILMemoryPool.Pool.Free(z); 
                return ILMath.diag( ILMath.tofcomplex(w)); 
            }
        }
        /// <summary>
        /// Find some eigenvalues and -vectors of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <param name="V">Output: n eigenvectors as columns. Size [n x n]. If V is null on input, the eigenvectors will not be computed and V is not changed. </param>
        /// <param name="rangeStart">The eigenvalues will be returned by increasing size. This will determine the number of the first eigenvalue to be returned.</param>
        /// <param name="rangeEnd">Determine the number of the last eigenvalue to be returned.</param>
        /// <returns>Diagonal matrix of size [n,n] with eigenvalues of A on the main diagonal.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square or <paramref name="rangeEnd"/> &lt; <paramref name="rangeStart"/> or if either one is &lt;= 0.</exception>
        public static ILRetArray< fcomplex> eigSymm( ILInArray< fcomplex> A, ILOutArray< fcomplex> V,  float rangeStart,  float rangeEnd ) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V, null))
                        V.a = empty<fcomplex>(A.Size);
                    return empty<fcomplex>(A.Size);
                }
                int n = A.Size[0];
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian");
                int m = 0, ldz = 0, info = 0;
                if (rangeStart > rangeEnd)
                    throw new ILArgumentException("invalid range of eigenvalues requested");
                ILArray< float> w = zeros<float>(1, n);
               
                fcomplex[] z;
                char jobz;
                if (object.Equals(V, null)) {
                    z = new  fcomplex[1];
                    jobz = 'N';
                    ldz = 1;
                } else {
                    z = ILMemoryPool.Pool.New<fcomplex>(n * n);
                    jobz = 'V';
                    ldz = n;
                }
                int[] isuppz = ILMemoryPool.Pool.New<int>(2 * n);

                ILArray< fcomplex> Acopy = A.C;
               
                fcomplex[] AcArr = Acopy.GetArrayForWrite();
               
                Lapack.cheevr(jobz, 'V', 'U', n, AcArr, n, rangeStart, rangeEnd, 0, 0, 0, ref m, w.GetArrayForWrite(), z, ldz, isuppz, ref info);
                ILMemoryPool.Pool.Free(AcArr);
                ILMemoryPool.Pool.Free(isuppz);

                if (jobz == 'V') {
                    V.a = array< fcomplex>(z, n, n);
                    V.a = V[full, r(0, m - 1)];
                }
                ILMemoryPool.Pool.Free(z);
                return diag( ILMath.tofcomplex(w));
            }
        }
        
        /// <summary>
        /// Compute eigenvalues of general square matrix A
        /// </summary>
        /// <param name="A">Input matrix A. Size [n x n]</param>
        /// <returns>Vector of eigenvalues of A. Size [n x 1]</returns>
        /// <remarks><para>The eigenvalues of A are found by use of the Lapack functions dgeevx, sgeevx, cgeevx and zgeevx. </para>
        /// <para>The vector returned will be of complex inner type, since no further constraints are set on the structure of A (it may be nonsymmetric). Use <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;)"/> or <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;,ILOutArray&lt;double&gt;)"/> functions for computing the real eigenvalues of symmetric matrices explicitly.</para>
        /// <para>A will be balanced first. This includes permutations and scaling of A in order to improve the conditioning of the eigenvalues.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;)"/>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;,ref MatrixProperties,bool)"/>
        public static ILRetArray< complex > eig(ILInArray< complex > A) {
            using (ILScope.Enter(A)) {
                ILArray< complex> V = null;
                MatrixProperties props = MatrixProperties.None;
                return eig(A, V, ref props, true);
            }
        }
        /// <summary>
        /// Compute eigenvalues and eigenvectors of general square matrix A
        /// </summary>
        /// <param name="A">Input matrix A. Size [n x n]</param>
        /// <param name="V">Output matrix, eigenvectors EV of size [n x n]. May be null on input. If not null, content of V will be destroyed.</param>
        /// <returns>Diagonal matrix with eigenvalues of A. Size [n x n]</returns>
        /// <remarks><para>The eigenvalues of A are found by use of the Lapack functions dgeevx, sgeevx, cgeevx and zgeevx. </para>
        /// <para>The matrices returned will be of complex inner type, since no further constrains are set on the structure of A (it may be nonsymmetric). Use <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;)"/> or <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;,ILOutArray&lt;double&gt;)"/> functions for computing the real eigenvalues of symmetric matrices explicitly.</para>
        /// <para>A will be balanced first. This includes permutations and scaling of A in order to improve the conditioning of the eigenvalues.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;)"/>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;,ref MatrixProperties,bool)"/>
        public static ILRetArray< complex > eig(ILInArray< complex > A, ILOutArray< complex > V) {
            MatrixProperties props = MatrixProperties.None; 
            return eig(A, V, ref props, true); 
        }
        /// <summary>
        /// Find eigenvalues  and eigenvectors 
        /// </summary>
        /// <param name="A">Input: square matrix, size [n x n]</param>
        /// <param name="V">Output (optional): eigenvectors</param>   
        /// <param name="propsA">Matrix properties, on input - if specified, 
        /// will be used to choose the proper method of solution. On exit will be 
        /// filled according to the properties of A.</param>
        /// <param name="balance">true: permute A in order to increase the 
        /// numerical stability, false: do not permute A.</param>
        /// <returns>eigenvalues as vector (if V is null) or as diagonoal 
        /// matrix (if V was requested, i.e. not null).</returns>
        /// <remarks><para>The eigenvalues of A are found by use of the 
        /// Lapack functions dgeevx, sgeevx, cgeevx and zgeevx. </para>
        /// <para>The arrays returned will be of complex inner type, 
        /// since no further constraints are set on the structure of 
        /// A (it may be nonsymmetric). Use 
        /// <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;)"/> 
        /// or <see cref="ILNumerics.ILMath.eigSymm(ILInArray&lt;double&gt;,ILOutArray&lt;double&gt;)"/> 
        /// functions for computing the real eigenvalues of symmetric 
        /// matrices explicitly.</para>
        /// <para>Depending on the parameter <paramref name="balance"/>, 
        /// A will be balanced first. This includes permutations and 
        /// scaling of A in order to improve the conditioning of the 
        /// eigenvalues.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;)"/>
        /// <seealso cref="ILNumerics.ILMath.eig(ILInArray&lt;double&gt;,ILOutArray&lt;complex&gt;,ref MatrixProperties,bool)"/>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if a 
        /// is not square</exception>
        public static ILRetArray< complex > eig(ILInArray< complex > A, ILOutArray< complex > V, ref MatrixProperties propsA, bool balance) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V,null))
                        V.a = empty<complex>(A.Size); 
                    return  empty<complex>(A.Size); 
                }
                ILArray< complex > ret = empty< complex >(ILSize.Empty00);  
                int n = A.Size[0]; 
                bool createVR = (object.Equals(V,null))? false:true; 
                if (n != A.Size[1]) 
                    throw new ILArgumentException("eig: matrix A must be square");
                propsA |= MatrixProperties.Square; 
                if (((propsA & MatrixProperties.Hermitian) != 0 || ILMath.ishermitian(A.C))) {
                    propsA |= MatrixProperties.Hermitian; 
                    ILArray< complex > Vd = null; 
                    if (createVR) 
                        Vd = localMember< complex >();
                    ILArray< complex > tmpRet = eigSymm(A,Vd);
                    if (createVR)
                        V.a =   (Vd); 
                    ret =   (tmpRet);
                } else {
                    // nonsymmetric case
                    char bal = (balance)? 'B':'N', jobvr;  
                    ILArray< complex > tmpA = A.C; 
                    complex [] vr = null;
                    complex [] wr = ILMemoryPool.Pool.New< complex >(n); 
                    
                    double [] scale  = ILMemoryPool.Pool.New< double >(n);
                    double [] rconde = ILMemoryPool.Pool.New< double >(n); 
                    double [] rcondv = ILMemoryPool.Pool.New< double >(n); 
                    double abnrm = 0; 
                    int ldvr, ilo = 0, ihi = 0, info = 0;
                    if (createVR) {
                        ldvr = n;
                        vr = ILMemoryPool.Pool.New< complex >(n * n);
                        jobvr = 'V'; 
                    } else {
                        ldvr = 1; 
                        vr = new  complex [1]; 
                        jobvr = 'N'; 
                    }
                    Lapack.zgeevx(bal,'N',jobvr,'N',n,tmpA.GetArrayForWrite(),n,wr,   new complex [1],1,vr,ldvr,ref ilo,ref ihi,scale,ref abnrm,rconde,rcondv,ref info);   
                    ILMemoryPool.Pool.Free(rconde);
                    ILMemoryPool.Pool.Free(rcondv);
                    ILMemoryPool.Pool.Free(scale);
                    if (info != 0) 
                        throw new ILArgumentException("eig: error in Lapack '?geevx': (" + info + ")");
                    // create eigenvalues 
                    complex [] retArr = ILMemoryPool.Pool.New< complex >(n); 
                    for (int i = 0; i < n; i++) {
                        retArr[i] = wr[i]; 
                    }
                    ret = new ILArray< complex > (retArr,n,1);
                    if (createVR) {
                        V.a = array<complex> (vr,n,n);
                        if (createVR)
                            ret.a = ILMath.diag< complex>(ret); 
                    }
                    
                    ILMemoryPool.Pool.Free(vr); 
                    ILMemoryPool.Pool.Free(wr); 
                }
                return ret; 
            }
        }

        /// <summary>
        /// Find all eigenvalues of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <returns>Array of size [n,1] with eigenvalues of A.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square.</exception>
        public static ILRetArray< complex > eigSymm (ILInArray< complex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return empty< complex>(A.Size); 
                }
                int n = A.Size[0]; 
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian.");
                int m = 0,info = 0; 
                ILArray< double > w = new ILArray< double > (new  double [n],1,n); 
                complex [] z = new  complex [1]; ; 
                int [] isuppz = new int[2 * n];
                ILArray< complex> Acopy = A.C;  
                complex [] AcArr = Acopy.GetArrayForWrite(); 
                Lapack.zheevr ('N','A','U',n,AcArr,n,0,0,0,0,0,ref m,w.GetArrayForWrite(),z,1,isuppz,ref info);
                ILMemoryPool.Pool.Free(AcArr); 
                return  ILMath.tocomplex(w); 
            }
        }
        /// <summary>
        /// Find all eigenvalues and -vectors of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <param name="V">Output: n eigenvectors as columns. Size [n x n]. If V is null on input, the eigenvectors 
        /// will not be computed and V is not changed. In order to make the function return the vectors, V should be initialized with any non null value (e.g.'1') before calling eigSymm.</param>
        /// <returns>Diagonal matrix of size [n,n] with eigenvalues of A on the main diagonal.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square.</exception>
        public static ILRetArray< complex > eigSymm (ILInArray< complex > A, ILOutArray< complex > V) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V,null))
                        V.a = empty<complex>(A.Size); 
                    return empty<complex>(A.Size); 
                }
                int n = A.Size[0]; 
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian.");
                int m = 0,ldz = 0,info = 0; 
                ILArray< double > w = new ILArray< double >(ILMemoryPool.Pool.New< double >(n),n,1); 
                complex [] z; 
                char jobz;
                if (object.Equals(V,null)) {
                    z = new  complex [1]; 
                    jobz = 'N';
                    ldz = 1; 
                } else {
                    z = ILMemoryPool.Pool.New<  complex >(n * n);
                    jobz = 'V';
                    ldz = n; 
                } 
                int [] isuppz = ILMemoryPool.Pool.New<int>( 2 * n);
                ILArray< complex> Acopy = A.C;
               
                complex[] AcArr = Acopy.GetArrayForWrite(); 
                Lapack.zheevr (jobz,'A','U',n,AcArr,n,1,n,0,0,0,ref m,w.GetArrayForWrite(),z,ldz,isuppz,ref info);
                ILMemoryPool.Pool.Free(AcArr); 
                ILMemoryPool.Pool.Free(isuppz);
                if (info != 0) 
                    throw new ILException("error returned from lapack: " + info); 
                if (jobz == 'V') {
                    System.Diagnostics.Debug.Assert(!object.Equals(V,null));
                    V.a =  array< complex > (z,n,n);
                    V.a = V[full,r(0,m-1)]; 
                    return ILMath.diag( ILMath.tocomplex(w)); 
                } else {
                    ILMemoryPool.Pool.Free(z); 
                    return  ILMath.tocomplex(w);
                }
            }
        }
        /// <summary>
        /// Find some eigenvalues and -vectors of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <param name="V">Output: n eigenvectors as columns. Size [n x n]. If V is null on input, the eigenvectors will not be computed and V is not changed. </param>
        /// <param name="rangeStart">Specify the lowest limit for the range of eigenvalues to be queried.</param>
        /// <param name="rangeEnd">Specify the upper limit for the range of eigenvalues to be queried.</param>
        /// <returns>Diagonal matrix of size [n,n] with eigenvalues of A on the main diagonal.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square or <paramref name="rangeEnd"/> &lt; <paramref name="rangeStart"/></exception>
        public static ILRetArray< complex > eigSymm (ILInArray< complex > A, ILOutArray< complex > V, int rangeStart, int rangeEnd) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V,null))
                        V.a = empty<complex>(A.Size); 
                    return empty<complex>(A.Size); 
                }
                int n = A.Size[0]; 
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian.");
                int m = 0,ldz = 0,info = 0; 
                if (rangeEnd < rangeStart || rangeStart < 1) 
                    throw new ILArgumentException("invalid range of eigenvalues requested");
                ILArray< double > w = array< double > (
                                            ILMemoryPool.Pool.New< double>(n),1,n); 
                complex [] z; 
                char jobz;
                if (object.Equals(V,null)) {
                    z = new  complex [1]; 
                    jobz = 'N';
                    ldz = 1; 
                } else {
                    z = ILMemoryPool.Pool.New<complex>(n * n);
                    jobz = 'V';
                    ldz = n; 
                } 
                int [] isuppz = ILMemoryPool.Pool.New<int>(2 * n);
                ILArray< complex> Acopy = A.C;
               
                complex[] AcArr = Acopy.GetArrayForWrite(); 
                Lapack.zheevr (jobz,'I','U',n,AcArr,n,0,0,rangeStart,rangeEnd,0,ref m,w.GetArrayForWrite(),z,ldz,isuppz,ref info);
                ILMemoryPool.Pool.Free(isuppz); 
                ILMemoryPool.Pool.Free(AcArr); 

                if (jobz == 'V') {
                    V.a = array<  complex >(z,n,n);
                    V.a = V[full,r(0,m-1)]; 
                }
                ILMemoryPool.Pool.Free(z); 
                return ILMath.diag( ILMath.tocomplex(w)); 
            }
        }
        /// <summary>
        /// Find some eigenvalues and -vectors of symmetric (hermitian) matrix
        /// </summary>
        /// <param name="A">Input matrix, Size [n x n], symmetric (hermitian for complex A) </param> 
        /// <param name="V">Output: n eigenvectors as columns. Size [n x n]. If V is null on input, the eigenvectors will not be computed and V is not changed. </param>
        /// <param name="rangeStart">The eigenvalues will be returned by increasing size. This will determine the number of the first eigenvalue to be returned.</param>
        /// <param name="rangeEnd">Determine the number of the last eigenvalue to be returned.</param>
        /// <returns>Diagonal matrix of size [n,n] with eigenvalues of A on the main diagonal.</returns>
        /// <remarks><para>For computation the Lapack functions dsyevr, ssyevr, chesvr and zheesv are used. </para>
        /// <para>Since A is symmetric, the eigenvalues will always be real. Therefore the return value will be of the same inner type as A.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A is not square or <paramref name="rangeEnd"/> &lt; <paramref name="rangeStart"/> or if either one is &lt;= 0.</exception>
        public static ILRetArray< complex> eigSymm( ILInArray< complex> A, ILOutArray< complex> V,  double rangeStart,  double rangeEnd ) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    if (!object.Equals(V, null))
                        V.a = empty<complex>(A.Size);
                    return empty<complex>(A.Size);
                }
                int n = A.Size[0];
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrix A must be square and symmetric/hermitian");
                int m = 0, ldz = 0, info = 0;
                if (rangeStart > rangeEnd)
                    throw new ILArgumentException("invalid range of eigenvalues requested");
                ILArray< double> w = zeros<double>(1, n);
               
                complex[] z;
                char jobz;
                if (object.Equals(V, null)) {
                    z = new  complex[1];
                    jobz = 'N';
                    ldz = 1;
                } else {
                    z = ILMemoryPool.Pool.New<complex>(n * n);
                    jobz = 'V';
                    ldz = n;
                }
                int[] isuppz = ILMemoryPool.Pool.New<int>(2 * n);

                ILArray< complex> Acopy = A.C;
               
                complex[] AcArr = Acopy.GetArrayForWrite();
               
                Lapack.zheevr(jobz, 'V', 'U', n, AcArr, n, rangeStart, rangeEnd, 0, 0, 0, ref m, w.GetArrayForWrite(), z, ldz, isuppz, ref info);
                ILMemoryPool.Pool.Free(AcArr);
                ILMemoryPool.Pool.Free(isuppz);

                if (jobz == 'V') {
                    V.a = array< complex>(z, n, n);
                    V.a = V[full, r(0, m - 1)];
                }
                ILMemoryPool.Pool.Free(z);
                return diag( ILMath.tocomplex(w));
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Specifies the type of eigenproblem 
        /// </summary>
        /// <remarks>The enumeration describes possible problem definitions for generelized eigenproblems:
        /// <list type="bullet">
        /// <item>Ax_eq_lambBx: A*V = r*B*V</item>
        /// <item>ABx_eq_lambx: A*B*V = r*V</item>
        /// <item>BAx_eq_lambx: B*A*V = r*V</item>
        /// </list></remarks>
        public enum GenEigenType {
            /// <summary>
            /// A*V = r*B*V
            /// </summary>
            Ax_eq_lambBx,
            /// <summary>
            /// A*B*V = r*V
            /// </summary>
            ABx_eq_lambx,
            /// <summary>
            /// B*A*V = r*V
            /// </summary>
            BAx_eq_lambx
        }




        /// <summary>
        /// Compute eigenvalues <it>lambda</it> of symmetrical/hermitian inputs A and B: A*V=lamda*B*V
        /// </summary>
        /// <param name="A">Square, symmetric/hermitian input matrix, size [n x n]</param>
        /// <param name="B">Square, symmetric/hermitian and positive definite matrix, size [n x n]</param>
        /// <returns>Vector of eigenvalues. size [n x 1]</returns>
        /// <remarks>
        /// <para>Internally, the generalized eigenproblem A*V = r*B*V will be reduced to B<sup>-1</sup>*A*V = r*V using cholesky factorization. The 
        /// computations are handled by LAPACK functions DSYGV,SSYGV,CHEGV and ZHEGV.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if B was not positive definite</exception> 
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A and B was not of the same size</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if either A and/or B was found not to be symmetric/hermitian</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the algorithm did not converge. All exceptions will contain an informational message describing the problem verbosely.</exception>
        public static ILRetArray<double> eigSymm(ILInArray<double> A, ILInArray<double> B) {
            using (ILScope.Enter(A, B)) {
                return eigSymm(A, B, null, GenEigenType.Ax_eq_lambBx, false);
            }
        }

        /// <summary>
        /// Compute eigenvalues and eigenvectors of symmetric/hermitian input
        /// </summary>
        /// <param name="A">Square, symmetric/hermitian input matrix, size [n x n]</param>
        /// <param name="B">Square, symmetric/hermitian and positive definite matrix, size [n x n]</param>
        /// <param name="outV">[Output] Returns eigenvectors in columns (size [n x n]). </param>
        /// <param name="skipSymmCheck">true: skip tests for A and B being hermitian.</param>
        /// <returns>Vector of eigenvalues. The return value will be a diagonal matrix with the eigenvalues on the main diagonal.</returns>
        /// <remarks><para>The eigenvectors in 'V' are not normalized!</para>
        /// <para>Internally, the generalized eigenproblem A*V = r*B*V will be reduced to B<sup>-1</sup>*A*V = r*V using cholesky factorization. The 
        /// computations are handled by LAPACK functions DSYGV,SSYGV,CHEGV and ZHEGV.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if B was not positive definite</exception> 
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A and B was not of the same size</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if <paramref name="skipSymmCheck"/> is false and either A and/or B was found not to be symmetric/hermitian</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the algorithm did not converge. All exceptions will contain an informational message describing the problem verbosely.</exception>
        public static ILRetArray<double> eigSymm(ILInArray<double> A, ILInArray<double> B, ILOutArray<double> outV, bool skipSymmCheck) {
            using (ILScope.Enter(A, B)) {
                return eigSymm(A, B, outV, GenEigenType.Ax_eq_lambBx, skipSymmCheck);
            }
        }

        /// <summary>
        /// Compute eigenvalues and eigenvectors (optional) of symmetric/hermitian input
        /// </summary>
        /// <param name="A">Square, symmetric/hermitian input matrix, size [n x n]</param>
        /// <param name="B">Square, symmetric/hermitian and positive definite matrix, size [n x n]</param>
        /// <param name="outV">[Output] If on input not null-> returns eigenvectors in columns (size [n x n]). If null on input -> eigenvectors will not get computed.</param>
        /// <param name="type">Determine the type of problem. This is one of the following types:
        /// <list type="bullet">
        /// <item>Ax_eq_lambBx: A*V = r*B*V</item>
        /// <item>ABx_eq_lambx: A*B*V = r*V</item>
        /// <item>BAx_eq_lambx: B*A*V = r*V</item>
        /// </list>Here 'r' is the eigenvalue corresponding to the eigenvector 'V'.</param>
        /// <param name="skipSymmCheck">true: skip tests for A and B being hermitian.</param>
        /// <returns>Vector of eigenvalues. If the eigenvectors are requested as well (V not null on input), 
        /// the return value will be a diagonal matrix with the eigenvalues on the main diagonal.</returns>
        /// <remarks><para>The eigenvectors in 'V' are not normalized!</para>
        /// <para>Internally, the generalized eigenproblem A*V = r*B*V will be reduced to B<sup>-1</sup>*A*V = r*V using cholesky factorization. The 
        /// computations are handled by LAPACK functions DSYGV,SSYGV,CHEGV and ZHEGV.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if B was not positive definite</exception> 
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A and B was not of the same size</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if <paramref name="skipSymmCheck"/> is false and either A and/or B was found not to be symmetric/hermitian</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the algorithm did not converge. All exceptions will contain an informational message describing the problem verbosely.</exception>
        public static ILRetArray<double> eigSymm(ILInArray<double> A, ILInArray<double> B, ILOutArray< double> outV, GenEigenType type, bool skipSymmCheck) {
            using (ILScope.Enter(A, B)) {
                // check input arguments
                if (object.Equals(A, null) || object.Equals(B, null))
                    throw new ILArgumentException("A and B must not be null!");
                if (!A.IsMatrix || !B.IsMatrix)
                    throw new ILArgumentException("A & B must be matrices!");
                int n = A.Size[0];
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrices must be square!");
                if (!A.Size.IsSameSize(B.Size))
                    throw new ILArgumentException("A and B must have the same size!");
                if (A.IsEmpty) {
                    if (object.Equals(outV, null))
                        return empty< double>(A.Size);
                    else {
                        outV.a = A.C;
                        return empty< double>(A.Size);
                    }
                }
                if (!skipSymmCheck && !ILMath.ishermitian(A)) {
                    throw new ILArgumentException("A must be hermitian!");
                }
                if (!skipSymmCheck && !ILMath.ishermitian(B)) {
                    throw new ILArgumentException("B must be hermitian!");
                }
                int info = -1;
                int itype = 1;
                switch (type) {
                    case GenEigenType.Ax_eq_lambBx:
                        itype = 1;
                        break;
                    case GenEigenType.ABx_eq_lambx:
                        itype = 2;
                        break;
                    case GenEigenType.BAx_eq_lambx:
                        itype = 3;
                        break;
                }
                char jobz = 'N';
                if (!object.Equals(outV, null)) {
                    jobz = 'V';
                    outV.a = copyUpperTriangle(A, n, n);
                } else {
                    ILArray< double> tmpOutV = copyUpperTriangle(A, n, n);
                    outV = tmpOutV; 
                }
                ILArray< double> BC = B.C;
                
                double[] w = ILMemoryPool.Pool.New< double>(n);
                /*!HC:lapack_func*/
                Lapack.dsygv(itype, jobz, 'U', n, outV.GetArrayForWrite(), n, BC.GetArrayForWrite(), BC.Size[0], w, ref info);
                if (info == 0) {
                    if (jobz == 'N')
                        return array< double>(w, 1, n);
                    else
                        return diag(array< double>(w, 1, n));
                } else if (info < 0) {
                    throw new ILArgumentException("invalid parameter reported from Lapack module: #" + (-info));
                } else {
                    if (info <= n) {
                        throw new ILArgumentException(String.Format("eigSymm did not converge! {0} off-diagonal elements unequal 0", info));
                    } else if (info < 2 * n) {
                        throw new ILArgumentException("eigSymm: B must be positive definite!");
                    } else {
                        throw new ILArgumentException("eigSymm: unknown Lapack module error");
                    }
                }
            }
        }

#region HYCALPER AUTO GENERATED CODE



        /// <summary>
        /// Compute eigenvalues <it>lambda</it> of symmetrical/hermitian inputs A and B: A*V=lamda*B*V
        /// </summary>
        /// <param name="A">Square, symmetric/hermitian input matrix, size [n x n]</param>
        /// <param name="B">Square, symmetric/hermitian and positive definite matrix, size [n x n]</param>
        /// <returns>Vector of eigenvalues. size [n x 1]</returns>
        /// <remarks>
        /// <para>Internally, the generalized eigenproblem A*V = r*B*V will be reduced to B<sup>-1</sup>*A*V = r*V using cholesky factorization. The 
        /// computations are handled by LAPACK functions DSYGV,SSYGV,CHEGV and ZHEGV.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if B was not positive definite</exception> 
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A and B was not of the same size</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if either A and/or B was found not to be symmetric/hermitian</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the algorithm did not converge. All exceptions will contain an informational message describing the problem verbosely.</exception>
        public static ILRetArray<float> eigSymm(ILInArray<float> A, ILInArray<float> B) {
            using (ILScope.Enter(A, B)) {
                return eigSymm(A, B, null, GenEigenType.Ax_eq_lambBx, false);
            }
        }

        /// <summary>
        /// Compute eigenvalues and eigenvectors of symmetric/hermitian input
        /// </summary>
        /// <param name="A">Square, symmetric/hermitian input matrix, size [n x n]</param>
        /// <param name="B">Square, symmetric/hermitian and positive definite matrix, size [n x n]</param>
        /// <param name="outV">[Output] Returns eigenvectors in columns (size [n x n]). </param>
        /// <param name="skipSymmCheck">true: skip tests for A and B being hermitian.</param>
        /// <returns>Vector of eigenvalues. The return value will be a diagonal matrix with the eigenvalues on the main diagonal.</returns>
        /// <remarks><para>The eigenvectors in 'V' are not normalized!</para>
        /// <para>Internally, the generalized eigenproblem A*V = r*B*V will be reduced to B<sup>-1</sup>*A*V = r*V using cholesky factorization. The 
        /// computations are handled by LAPACK functions DSYGV,SSYGV,CHEGV and ZHEGV.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if B was not positive definite</exception> 
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A and B was not of the same size</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if <paramref name="skipSymmCheck"/> is false and either A and/or B was found not to be symmetric/hermitian</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the algorithm did not converge. All exceptions will contain an informational message describing the problem verbosely.</exception>
        public static ILRetArray<float> eigSymm(ILInArray<float> A, ILInArray<float> B, ILOutArray<float> outV, bool skipSymmCheck) {
            using (ILScope.Enter(A, B)) {
                return eigSymm(A, B, outV, GenEigenType.Ax_eq_lambBx, skipSymmCheck);
            }
        }

        /// <summary>
        /// Compute eigenvalues and eigenvectors (optional) of symmetric/hermitian input
        /// </summary>
        /// <param name="A">Square, symmetric/hermitian input matrix, size [n x n]</param>
        /// <param name="B">Square, symmetric/hermitian and positive definite matrix, size [n x n]</param>
        /// <param name="outV">[Output] If on input not null-> returns eigenvectors in columns (size [n x n]). If null on input -> eigenvectors will not get computed.</param>
        /// <param name="type">Determine the type of problem. This is one of the following types:
        /// <list type="bullet">
        /// <item>Ax_eq_lambBx: A*V = r*B*V</item>
        /// <item>ABx_eq_lambx: A*B*V = r*V</item>
        /// <item>BAx_eq_lambx: B*A*V = r*V</item>
        /// </list>Here 'r' is the eigenvalue corresponding to the eigenvector 'V'.</param>
        /// <param name="skipSymmCheck">true: skip tests for A and B being hermitian.</param>
        /// <returns>Vector of eigenvalues. If the eigenvectors are requested as well (V not null on input), 
        /// the return value will be a diagonal matrix with the eigenvalues on the main diagonal.</returns>
        /// <remarks><para>The eigenvectors in 'V' are not normalized!</para>
        /// <para>Internally, the generalized eigenproblem A*V = r*B*V will be reduced to B<sup>-1</sup>*A*V = r*V using cholesky factorization. The 
        /// computations are handled by LAPACK functions DSYGV,SSYGV,CHEGV and ZHEGV.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if B was not positive definite</exception> 
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A and B was not of the same size</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if <paramref name="skipSymmCheck"/> is false and either A and/or B was found not to be symmetric/hermitian</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the algorithm did not converge. All exceptions will contain an informational message describing the problem verbosely.</exception>
        public static ILRetArray<float> eigSymm(ILInArray<float> A, ILInArray<float> B, ILOutArray< float> outV, GenEigenType type, bool skipSymmCheck) {
            using (ILScope.Enter(A, B)) {
                // check input arguments
                if (object.Equals(A, null) || object.Equals(B, null))
                    throw new ILArgumentException("A and B must not be null!");
                if (!A.IsMatrix || !B.IsMatrix)
                    throw new ILArgumentException("A & B must be matrices!");
                int n = A.Size[0];
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrices must be square!");
                if (!A.Size.IsSameSize(B.Size))
                    throw new ILArgumentException("A and B must have the same size!");
                if (A.IsEmpty) {
                    if (object.Equals(outV, null))
                        return empty< float>(A.Size);
                    else {
                        outV.a = A.C;
                        return empty< float>(A.Size);
                    }
                }
                if (!skipSymmCheck && !ILMath.ishermitian(A)) {
                    throw new ILArgumentException("A must be hermitian!");
                }
                if (!skipSymmCheck && !ILMath.ishermitian(B)) {
                    throw new ILArgumentException("B must be hermitian!");
                }
                int info = -1;
                int itype = 1;
                switch (type) {
                    case GenEigenType.Ax_eq_lambBx:
                        itype = 1;
                        break;
                    case GenEigenType.ABx_eq_lambx:
                        itype = 2;
                        break;
                    case GenEigenType.BAx_eq_lambx:
                        itype = 3;
                        break;
                }
                char jobz = 'N';
                if (!object.Equals(outV, null)) {
                    jobz = 'V';
                    outV.a = copyUpperTriangle(A, n, n);
                } else {
                    ILArray< float> tmpOutV = copyUpperTriangle(A, n, n);
                    outV = tmpOutV; 
                }
                ILArray< float> BC = B.C;
               
                float[] w = ILMemoryPool.Pool.New< float>(n);
               
                Lapack.ssygv(itype, jobz, 'U', n, outV.GetArrayForWrite(), n, BC.GetArrayForWrite(), BC.Size[0], w, ref info);
                if (info == 0) {
                    if (jobz == 'N')
                        return array< float>(w, 1, n);
                    else
                        return diag(array< float>(w, 1, n));
                } else if (info < 0) {
                    throw new ILArgumentException("invalid parameter reported from Lapack module: #" + (-info));
                } else {
                    if (info <= n) {
                        throw new ILArgumentException(String.Format("eigSymm did not converge! {0} off-diagonal elements unequal 0", info));
                    } else if (info < 2 * n) {
                        throw new ILArgumentException("eigSymm: B must be positive definite!");
                    } else {
                        throw new ILArgumentException("eigSymm: unknown Lapack module error");
                    }
                }
            }
        }


        /// <summary>
        /// Compute eigenvalues <it>lambda</it> of symmetrical/hermitian inputs A and B: A*V=lamda*B*V
        /// </summary>
        /// <param name="A">Square, symmetric/hermitian input matrix, size [n x n]</param>
        /// <param name="B">Square, symmetric/hermitian and positive definite matrix, size [n x n]</param>
        /// <returns>Vector of eigenvalues. size [n x 1]</returns>
        /// <remarks>
        /// <para>Internally, the generalized eigenproblem A*V = r*B*V will be reduced to B<sup>-1</sup>*A*V = r*V using cholesky factorization. The 
        /// computations are handled by LAPACK functions DSYGV,SSYGV,CHEGV and ZHEGV.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if B was not positive definite</exception> 
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A and B was not of the same size</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if either A and/or B was found not to be symmetric/hermitian</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the algorithm did not converge. All exceptions will contain an informational message describing the problem verbosely.</exception>
        public static ILRetArray<float> eigSymm(ILInArray<fcomplex> A, ILInArray<fcomplex> B) {
            using (ILScope.Enter(A, B)) {
                return eigSymm(A, B, null, GenEigenType.Ax_eq_lambBx, false);
            }
        }

        /// <summary>
        /// Compute eigenvalues and eigenvectors of symmetric/hermitian input
        /// </summary>
        /// <param name="A">Square, symmetric/hermitian input matrix, size [n x n]</param>
        /// <param name="B">Square, symmetric/hermitian and positive definite matrix, size [n x n]</param>
        /// <param name="outV">[Output] Returns eigenvectors in columns (size [n x n]). </param>
        /// <param name="skipSymmCheck">true: skip tests for A and B being hermitian.</param>
        /// <returns>Vector of eigenvalues. The return value will be a diagonal matrix with the eigenvalues on the main diagonal.</returns>
        /// <remarks><para>The eigenvectors in 'V' are not normalized!</para>
        /// <para>Internally, the generalized eigenproblem A*V = r*B*V will be reduced to B<sup>-1</sup>*A*V = r*V using cholesky factorization. The 
        /// computations are handled by LAPACK functions DSYGV,SSYGV,CHEGV and ZHEGV.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if B was not positive definite</exception> 
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A and B was not of the same size</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if <paramref name="skipSymmCheck"/> is false and either A and/or B was found not to be symmetric/hermitian</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the algorithm did not converge. All exceptions will contain an informational message describing the problem verbosely.</exception>
        public static ILRetArray<float> eigSymm(ILInArray<fcomplex> A, ILInArray<fcomplex> B, ILOutArray<fcomplex> outV, bool skipSymmCheck) {
            using (ILScope.Enter(A, B)) {
                return eigSymm(A, B, outV, GenEigenType.Ax_eq_lambBx, skipSymmCheck);
            }
        }

        /// <summary>
        /// Compute eigenvalues and eigenvectors (optional) of symmetric/hermitian input
        /// </summary>
        /// <param name="A">Square, symmetric/hermitian input matrix, size [n x n]</param>
        /// <param name="B">Square, symmetric/hermitian and positive definite matrix, size [n x n]</param>
        /// <param name="outV">[Output] If on input not null-> returns eigenvectors in columns (size [n x n]). If null on input -> eigenvectors will not get computed.</param>
        /// <param name="type">Determine the type of problem. This is one of the following types:
        /// <list type="bullet">
        /// <item>Ax_eq_lambBx: A*V = r*B*V</item>
        /// <item>ABx_eq_lambx: A*B*V = r*V</item>
        /// <item>BAx_eq_lambx: B*A*V = r*V</item>
        /// </list>Here 'r' is the eigenvalue corresponding to the eigenvector 'V'.</param>
        /// <param name="skipSymmCheck">true: skip tests for A and B being hermitian.</param>
        /// <returns>Vector of eigenvalues. If the eigenvectors are requested as well (V not null on input), 
        /// the return value will be a diagonal matrix with the eigenvalues on the main diagonal.</returns>
        /// <remarks><para>The eigenvectors in 'V' are not normalized!</para>
        /// <para>Internally, the generalized eigenproblem A*V = r*B*V will be reduced to B<sup>-1</sup>*A*V = r*V using cholesky factorization. The 
        /// computations are handled by LAPACK functions DSYGV,SSYGV,CHEGV and ZHEGV.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if B was not positive definite</exception> 
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A and B was not of the same size</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if <paramref name="skipSymmCheck"/> is false and either A and/or B was found not to be symmetric/hermitian</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the algorithm did not converge. All exceptions will contain an informational message describing the problem verbosely.</exception>
        public static ILRetArray<float> eigSymm(ILInArray<fcomplex> A, ILInArray<fcomplex> B, ILOutArray< fcomplex> outV, GenEigenType type, bool skipSymmCheck) {
            using (ILScope.Enter(A, B)) {
                // check input arguments
                if (object.Equals(A, null) || object.Equals(B, null))
                    throw new ILArgumentException("A and B must not be null!");
                if (!A.IsMatrix || !B.IsMatrix)
                    throw new ILArgumentException("A & B must be matrices!");
                int n = A.Size[0];
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrices must be square!");
                if (!A.Size.IsSameSize(B.Size))
                    throw new ILArgumentException("A and B must have the same size!");
                if (A.IsEmpty) {
                    if (object.Equals(outV, null))
                        return empty< float>(A.Size);
                    else {
                        outV.a = A.C;
                        return empty< float>(A.Size);
                    }
                }
                if (!skipSymmCheck && !ILMath.ishermitian(A)) {
                    throw new ILArgumentException("A must be hermitian!");
                }
                if (!skipSymmCheck && !ILMath.ishermitian(B)) {
                    throw new ILArgumentException("B must be hermitian!");
                }
                int info = -1;
                int itype = 1;
                switch (type) {
                    case GenEigenType.Ax_eq_lambBx:
                        itype = 1;
                        break;
                    case GenEigenType.ABx_eq_lambx:
                        itype = 2;
                        break;
                    case GenEigenType.BAx_eq_lambx:
                        itype = 3;
                        break;
                }
                char jobz = 'N';
                if (!object.Equals(outV, null)) {
                    jobz = 'V';
                    outV.a = copyUpperTriangle(A, n, n);
                } else {
                    ILArray< fcomplex> tmpOutV = copyUpperTriangle(A, n, n);
                    outV = tmpOutV; 
                }
                ILArray< fcomplex> BC = B.C;
               
                float[] w = ILMemoryPool.Pool.New< float>(n);
               
                Lapack.chegv(itype, jobz, 'U', n, outV.GetArrayForWrite(), n, BC.GetArrayForWrite(), BC.Size[0], w, ref info);
                if (info == 0) {
                    if (jobz == 'N')
                        return array< float>(w, 1, n);
                    else
                        return diag(array< float>(w, 1, n));
                } else if (info < 0) {
                    throw new ILArgumentException("invalid parameter reported from Lapack module: #" + (-info));
                } else {
                    if (info <= n) {
                        throw new ILArgumentException(String.Format("eigSymm did not converge! {0} off-diagonal elements unequal 0", info));
                    } else if (info < 2 * n) {
                        throw new ILArgumentException("eigSymm: B must be positive definite!");
                    } else {
                        throw new ILArgumentException("eigSymm: unknown Lapack module error");
                    }
                }
            }
        }


        /// <summary>
        /// Compute eigenvalues <it>lambda</it> of symmetrical/hermitian inputs A and B: A*V=lamda*B*V
        /// </summary>
        /// <param name="A">Square, symmetric/hermitian input matrix, size [n x n]</param>
        /// <param name="B">Square, symmetric/hermitian and positive definite matrix, size [n x n]</param>
        /// <returns>Vector of eigenvalues. size [n x 1]</returns>
        /// <remarks>
        /// <para>Internally, the generalized eigenproblem A*V = r*B*V will be reduced to B<sup>-1</sup>*A*V = r*V using cholesky factorization. The 
        /// computations are handled by LAPACK functions DSYGV,SSYGV,CHEGV and ZHEGV.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if B was not positive definite</exception> 
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A and B was not of the same size</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if either A and/or B was found not to be symmetric/hermitian</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the algorithm did not converge. All exceptions will contain an informational message describing the problem verbosely.</exception>
        public static ILRetArray<double> eigSymm(ILInArray<complex> A, ILInArray<complex> B) {
            using (ILScope.Enter(A, B)) {
                return eigSymm(A, B, null, GenEigenType.Ax_eq_lambBx, false);
            }
        }

        /// <summary>
        /// Compute eigenvalues and eigenvectors of symmetric/hermitian input
        /// </summary>
        /// <param name="A">Square, symmetric/hermitian input matrix, size [n x n]</param>
        /// <param name="B">Square, symmetric/hermitian and positive definite matrix, size [n x n]</param>
        /// <param name="outV">[Output] Returns eigenvectors in columns (size [n x n]). </param>
        /// <param name="skipSymmCheck">true: skip tests for A and B being hermitian.</param>
        /// <returns>Vector of eigenvalues. The return value will be a diagonal matrix with the eigenvalues on the main diagonal.</returns>
        /// <remarks><para>The eigenvectors in 'V' are not normalized!</para>
        /// <para>Internally, the generalized eigenproblem A*V = r*B*V will be reduced to B<sup>-1</sup>*A*V = r*V using cholesky factorization. The 
        /// computations are handled by LAPACK functions DSYGV,SSYGV,CHEGV and ZHEGV.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if B was not positive definite</exception> 
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A and B was not of the same size</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if <paramref name="skipSymmCheck"/> is false and either A and/or B was found not to be symmetric/hermitian</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the algorithm did not converge. All exceptions will contain an informational message describing the problem verbosely.</exception>
        public static ILRetArray<double> eigSymm(ILInArray<complex> A, ILInArray<complex> B, ILOutArray<complex> outV, bool skipSymmCheck) {
            using (ILScope.Enter(A, B)) {
                return eigSymm(A, B, outV, GenEigenType.Ax_eq_lambBx, skipSymmCheck);
            }
        }

        /// <summary>
        /// Compute eigenvalues and eigenvectors (optional) of symmetric/hermitian input
        /// </summary>
        /// <param name="A">Square, symmetric/hermitian input matrix, size [n x n]</param>
        /// <param name="B">Square, symmetric/hermitian and positive definite matrix, size [n x n]</param>
        /// <param name="outV">[Output] If on input not null-> returns eigenvectors in columns (size [n x n]). If null on input -> eigenvectors will not get computed.</param>
        /// <param name="type">Determine the type of problem. This is one of the following types:
        /// <list type="bullet">
        /// <item>Ax_eq_lambBx: A*V = r*B*V</item>
        /// <item>ABx_eq_lambx: A*B*V = r*V</item>
        /// <item>BAx_eq_lambx: B*A*V = r*V</item>
        /// </list>Here 'r' is the eigenvalue corresponding to the eigenvector 'V'.</param>
        /// <param name="skipSymmCheck">true: skip tests for A and B being hermitian.</param>
        /// <returns>Vector of eigenvalues. If the eigenvectors are requested as well (V not null on input), 
        /// the return value will be a diagonal matrix with the eigenvalues on the main diagonal.</returns>
        /// <remarks><para>The eigenvectors in 'V' are not normalized!</para>
        /// <para>Internally, the generalized eigenproblem A*V = r*B*V will be reduced to B<sup>-1</sup>*A*V = r*V using cholesky factorization. The 
        /// computations are handled by LAPACK functions DSYGV,SSYGV,CHEGV and ZHEGV.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if B was not positive definite</exception> 
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A and B was not of the same size</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if <paramref name="skipSymmCheck"/> is false and either A and/or B was found not to be symmetric/hermitian</exception>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the algorithm did not converge. All exceptions will contain an informational message describing the problem verbosely.</exception>
        public static ILRetArray<double> eigSymm(ILInArray<complex> A, ILInArray<complex> B, ILOutArray< complex> outV, GenEigenType type, bool skipSymmCheck) {
            using (ILScope.Enter(A, B)) {
                // check input arguments
                if (object.Equals(A, null) || object.Equals(B, null))
                    throw new ILArgumentException("A and B must not be null!");
                if (!A.IsMatrix || !B.IsMatrix)
                    throw new ILArgumentException("A & B must be matrices!");
                int n = A.Size[0];
                if (n != A.Size[1])
                    throw new ILArgumentException("input matrices must be square!");
                if (!A.Size.IsSameSize(B.Size))
                    throw new ILArgumentException("A and B must have the same size!");
                if (A.IsEmpty) {
                    if (object.Equals(outV, null))
                        return empty< double>(A.Size);
                    else {
                        outV.a = A.C;
                        return empty< double>(A.Size);
                    }
                }
                if (!skipSymmCheck && !ILMath.ishermitian(A)) {
                    throw new ILArgumentException("A must be hermitian!");
                }
                if (!skipSymmCheck && !ILMath.ishermitian(B)) {
                    throw new ILArgumentException("B must be hermitian!");
                }
                int info = -1;
                int itype = 1;
                switch (type) {
                    case GenEigenType.Ax_eq_lambBx:
                        itype = 1;
                        break;
                    case GenEigenType.ABx_eq_lambx:
                        itype = 2;
                        break;
                    case GenEigenType.BAx_eq_lambx:
                        itype = 3;
                        break;
                }
                char jobz = 'N';
                if (!object.Equals(outV, null)) {
                    jobz = 'V';
                    outV.a = copyUpperTriangle(A, n, n);
                } else {
                    ILArray< complex> tmpOutV = copyUpperTriangle(A, n, n);
                    outV = tmpOutV; 
                }
                ILArray< complex> BC = B.C;
               
                double[] w = ILMemoryPool.Pool.New< double>(n);
               
                Lapack.zhegv(itype, jobz, 'U', n, outV.GetArrayForWrite(), n, BC.GetArrayForWrite(), BC.Size[0], w, ref info);
                if (info == 0) {
                    if (jobz == 'N')
                        return array< double>(w, 1, n);
                    else
                        return diag(array< double>(w, 1, n));
                } else if (info < 0) {
                    throw new ILArgumentException("invalid parameter reported from Lapack module: #" + (-info));
                } else {
                    if (info <= n) {
                        throw new ILArgumentException(String.Format("eigSymm did not converge! {0} off-diagonal elements unequal 0", info));
                    } else if (info < 2 * n) {
                        throw new ILArgumentException("eigSymm: B must be positive definite!");
                    } else {
                        throw new ILArgumentException("eigSymm: unknown Lapack module error");
                    }
                }
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}

