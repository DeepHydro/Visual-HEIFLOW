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
        /// Trace of matrix
        /// </summary>
        /// <param name="A">Input matrix, size [m x n]</param>
        /// <returns>Scalar of same type as A with the sum of diagonal elements of A.</returns>
        public static ILRetArray< double> trace( ILInArray< double> A ) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return empty< double>(A.Size);
                if (A.IsVector || A.IsScalar)
                    return A[0];
                return sum(diag(A));
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Trace of matrix
        /// </summary>
        /// <param name="A">Input matrix, size [m x n]</param>
        /// <returns>Scalar of same type as A with the sum of diagonal elements of A.</returns>
        public static ILRetArray< Int64> trace( ILInArray< Int64> A ) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return empty< Int64>(A.Size);
                if (A.IsVector || A.IsScalar)
                    return A[0];
                return sum(diag(A));
            }
        }
        /// <summary>
        /// Trace of matrix
        /// </summary>
        /// <param name="A">Input matrix, size [m x n]</param>
        /// <returns>Scalar of same type as A with the sum of diagonal elements of A.</returns>
        public static ILRetArray< Int32> trace( ILInArray< Int32> A ) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return empty< Int32>(A.Size);
                if (A.IsVector || A.IsScalar)
                    return A[0];
                return sum(diag(A));
            }
        }
        /// <summary>
        /// Trace of matrix
        /// </summary>
        /// <param name="A">Input matrix, size [m x n]</param>
        /// <returns>Scalar of same type as A with the sum of diagonal elements of A.</returns>
        public static ILRetArray< float> trace( ILInArray< float> A ) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return empty< float>(A.Size);
                if (A.IsVector || A.IsScalar)
                    return A[0];
                return sum(diag(A));
            }
        }
        /// <summary>
        /// Trace of matrix
        /// </summary>
        /// <param name="A">Input matrix, size [m x n]</param>
        /// <returns>Scalar of same type as A with the sum of diagonal elements of A.</returns>
        public static ILRetArray< fcomplex> trace( ILInArray< fcomplex> A ) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return empty< fcomplex>(A.Size);
                if (A.IsVector || A.IsScalar)
                    return A[0];
                return sum(diag(A));
            }
        }
        /// <summary>
        /// Trace of matrix
        /// </summary>
        /// <param name="A">Input matrix, size [m x n]</param>
        /// <returns>Scalar of same type as A with the sum of diagonal elements of A.</returns>
        public static ILRetArray< complex> trace( ILInArray< complex> A ) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return empty< complex>(A.Size);
                if (A.IsVector || A.IsScalar)
                    return A[0];
                return sum(diag(A));
            }
        }
        /// <summary>
        /// Trace of matrix
        /// </summary>
        /// <param name="A">Input matrix, size [m x n]</param>
        /// <returns>Scalar of same type as A with the sum of diagonal elements of A.</returns>
        public static ILRetArray< byte> trace( ILInArray< byte> A ) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return empty< byte>(A.Size);
                if (A.IsVector || A.IsScalar)
                    return A[0];
                return sum(diag(A));
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}
