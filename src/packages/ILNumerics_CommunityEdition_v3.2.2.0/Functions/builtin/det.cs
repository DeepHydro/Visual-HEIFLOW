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
        /// Determinant of square matrix
        /// </summary>
        /// <param name="A">Input matrix (square)</param>
        /// <returns>Determinant of A</returns>
        /// <remarks><para>The determinant is computed by decomposing A into upper and lower triangular part (using the LAPACK function ?getrf).<br />
        /// Due to the properties of determinants, det(a) is the same as det(L) * det(U),where det(L) can easily be extracted from the permutation indices returned from LU decomposition. det(U) - with U being an upper triangular matrix - equals the product of the diagonal elements.</para>
        /// <para>For scalar A, a plain copy of A is returned.</para></remarks>
        /// <example>Creating a nonsingular 4x4 (double) matrix and it's determinant
        /// <code>ILArray&lt;double&gt; A = ILMath.counter(1.0,1.0,4,4);
        ///A[1] = 0.0;  // make A nonsingular
        ///A[14] = 0.0; //(same as: A[2,3] = 0.0;) 
        /// // A is now:
        /// //&lt;Double&gt; [4,4]
        /// //(:,:) 1e+001 * 
        /// // 0,10000   0,50000   0,90000   1,30000 
        /// // 0,00000   0,60000   1,00000   1,40000 
        /// // 0,30000   0,70000   1,10000   0,00000 
        /// // 0,40000   0,80000   1,20000   1,60000 
        /// 
        ///ILMath.det(A) gives:
        /// //&lt;Double&gt; -360
        ///</code></example>
        ///<exception cref="ILNumerics.Exceptions.ILArgumentException">if A is empty or not a square matrix</exception>
        public static ILRetArray< double > det(ILInArray< double > A) {
            using (ILScope.Enter(A)) {
                if (A.IsScalar) 
                    return A.C; 
                if (A.IsEmpty) 
                    throw new ILArgumentException("det: A must be a matrix"); 
                int m = A.Size[0]; 
                if (m != A.Size[1]) {
                    throw new ILArgumentException("det: matrix A must be square"); 
                }

                ILArray< double > L = A.C; 
                 double [] lArr = L.GetArrayForWrite(); 
                int [] pivInd = new int[m]; 
                int info = 0; 
                /*!HC:lapack_*getrf*/ Lapack.dgetrf (m, m, lArr, m, pivInd ,ref info); 
                if (info < 0 ) {
                    throw new ILArgumentException("det: illegal parameter error");
                }
                // determine pivoting: number of exchanges 
                 double retA =  1.0 ; 
                for (int i = 0; i < m;) {
                    retA *= lArr[i * m + i]; 
                    if (pivInd[i] != ++i) retA *=  -1.0 ; 
                }
                return retA;
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Determinant of square matrix
        /// </summary>
        /// <param name="A">Input matrix (square)</param>
        /// <returns>Determinant of A</returns>
        /// <remarks><para>The determinant is computed by decomposing A into upper and lower triangular part (using the LAPACK function ?getrf).<br />
        /// Due to the properties of determinants, det(a) is the same as det(L) * det(U),where det(L) can easily be extracted from the permutation indices returned from LU decomposition. det(U) - with U being an upper triangular matrix - equals the product of the diagonal elements.</para>
        /// <para>For scalar A, a plain copy of A is returned.</para></remarks>
        /// <example>Creating a nonsingular 4x4 (double) matrix and it's determinant
        /// <code>ILArray&lt;double&gt; A = ILMath.counter(1.0,1.0,4,4);
        ///A[1] = 0.0;  // make A nonsingular
        ///A[14] = 0.0; //(same as: A[2,3] = 0.0;) 
        /// // A is now:
        /// //&lt;Double&gt; [4,4]
        /// //(:,:) 1e+001 * 
        /// // 0,10000   0,50000   0,90000   1,30000 
        /// // 0,00000   0,60000   1,00000   1,40000 
        /// // 0,30000   0,70000   1,10000   0,00000 
        /// // 0,40000   0,80000   1,20000   1,60000 
        /// 
        ///ILMath.det(A) gives:
        /// //&lt;Double&gt; -360
        ///</code></example>
        ///<exception cref="ILNumerics.Exceptions.ILArgumentException">if A is empty or not a square matrix</exception>
        public static ILRetArray< float > det(ILInArray< float > A) {
            using (ILScope.Enter(A)) {
                if (A.IsScalar) 
                    return A.C; 
                if (A.IsEmpty) 
                    throw new ILArgumentException("det: A must be a matrix"); 
                int m = A.Size[0]; 
                if (m != A.Size[1]) {
                    throw new ILArgumentException("det: matrix A must be square"); 
                }

                ILArray< float > L = A.C; 
                float [] lArr = L.GetArrayForWrite(); 
                int [] pivInd = new int[m]; 
                int info = 0; 
                Lapack.sgetrf (m, m, lArr, m, pivInd ,ref info); 
                if (info < 0 ) {
                    throw new ILArgumentException("det: illegal parameter error");
                }
                // determine pivoting: number of exchanges 
                float retA =  1.0f ; 
                for (int i = 0; i < m;) {
                    retA *= lArr[i * m + i]; 
                    if (pivInd[i] != ++i) retA *=  -1.0f ; 
                }
                return retA;
            }
        }
        /// <summary>
        /// Determinant of square matrix
        /// </summary>
        /// <param name="A">Input matrix (square)</param>
        /// <returns>Determinant of A</returns>
        /// <remarks><para>The determinant is computed by decomposing A into upper and lower triangular part (using the LAPACK function ?getrf).<br />
        /// Due to the properties of determinants, det(a) is the same as det(L) * det(U),where det(L) can easily be extracted from the permutation indices returned from LU decomposition. det(U) - with U being an upper triangular matrix - equals the product of the diagonal elements.</para>
        /// <para>For scalar A, a plain copy of A is returned.</para></remarks>
        /// <example>Creating a nonsingular 4x4 (double) matrix and it's determinant
        /// <code>ILArray&lt;double&gt; A = ILMath.counter(1.0,1.0,4,4);
        ///A[1] = 0.0;  // make A nonsingular
        ///A[14] = 0.0; //(same as: A[2,3] = 0.0;) 
        /// // A is now:
        /// //&lt;Double&gt; [4,4]
        /// //(:,:) 1e+001 * 
        /// // 0,10000   0,50000   0,90000   1,30000 
        /// // 0,00000   0,60000   1,00000   1,40000 
        /// // 0,30000   0,70000   1,10000   0,00000 
        /// // 0,40000   0,80000   1,20000   1,60000 
        /// 
        ///ILMath.det(A) gives:
        /// //&lt;Double&gt; -360
        ///</code></example>
        ///<exception cref="ILNumerics.Exceptions.ILArgumentException">if A is empty or not a square matrix</exception>
        public static ILRetArray< fcomplex > det(ILInArray< fcomplex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsScalar) 
                    return A.C; 
                if (A.IsEmpty) 
                    throw new ILArgumentException("det: A must be a matrix"); 
                int m = A.Size[0]; 
                if (m != A.Size[1]) {
                    throw new ILArgumentException("det: matrix A must be square"); 
                }

                ILArray< fcomplex > L = A.C; 
                fcomplex [] lArr = L.GetArrayForWrite(); 
                int [] pivInd = new int[m]; 
                int info = 0; 
                Lapack.cgetrf (m, m, lArr, m, pivInd ,ref info); 
                if (info < 0 ) {
                    throw new ILArgumentException("det: illegal parameter error");
                }
                // determine pivoting: number of exchanges 
                fcomplex retA =  new fcomplex(1.0f,0.0f) ; 
                for (int i = 0; i < m;) {
                    retA *= lArr[i * m + i]; 
                    if (pivInd[i] != ++i) retA *=  -1.0f ; 
                }
                return retA;
            }
        }
        /// <summary>
        /// Determinant of square matrix
        /// </summary>
        /// <param name="A">Input matrix (square)</param>
        /// <returns>Determinant of A</returns>
        /// <remarks><para>The determinant is computed by decomposing A into upper and lower triangular part (using the LAPACK function ?getrf).<br />
        /// Due to the properties of determinants, det(a) is the same as det(L) * det(U),where det(L) can easily be extracted from the permutation indices returned from LU decomposition. det(U) - with U being an upper triangular matrix - equals the product of the diagonal elements.</para>
        /// <para>For scalar A, a plain copy of A is returned.</para></remarks>
        /// <example>Creating a nonsingular 4x4 (double) matrix and it's determinant
        /// <code>ILArray&lt;double&gt; A = ILMath.counter(1.0,1.0,4,4);
        ///A[1] = 0.0;  // make A nonsingular
        ///A[14] = 0.0; //(same as: A[2,3] = 0.0;) 
        /// // A is now:
        /// //&lt;Double&gt; [4,4]
        /// //(:,:) 1e+001 * 
        /// // 0,10000   0,50000   0,90000   1,30000 
        /// // 0,00000   0,60000   1,00000   1,40000 
        /// // 0,30000   0,70000   1,10000   0,00000 
        /// // 0,40000   0,80000   1,20000   1,60000 
        /// 
        ///ILMath.det(A) gives:
        /// //&lt;Double&gt; -360
        ///</code></example>
        ///<exception cref="ILNumerics.Exceptions.ILArgumentException">if A is empty or not a square matrix</exception>
        public static ILRetArray< complex > det(ILInArray< complex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsScalar) 
                    return A.C; 
                if (A.IsEmpty) 
                    throw new ILArgumentException("det: A must be a matrix"); 
                int m = A.Size[0]; 
                if (m != A.Size[1]) {
                    throw new ILArgumentException("det: matrix A must be square"); 
                }

                ILArray< complex > L = A.C; 
                complex [] lArr = L.GetArrayForWrite(); 
                int [] pivInd = new int[m]; 
                int info = 0; 
                Lapack.zgetrf (m, m, lArr, m, pivInd ,ref info); 
                if (info < 0 ) {
                    throw new ILArgumentException("det: illegal parameter error");
                }
                // determine pivoting: number of exchanges 
                complex retA =  new complex(1.0,0.0) ; 
                for (int i = 0; i < m;) {
                    retA *= lArr[i * m + i]; 
                    if (pivInd[i] != ++i) retA *=  -1.0 ; 
                }
                return retA;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}
