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
        /// Determine if matrix A is a lower triangular matrix
        /// </summary>
        /// <param name="A">Matrix of numeric inner type</param>
        /// <returns>true if A is a lower triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istrilow(ILInArray<double> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istrilow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istrilow: A must be a matrix!");
                int n = A.Size[1];
                for (int c = 1; c < n; c++) {
                    for (int r = 0; r < c; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        
        /// <summary>
        /// Determine if matrix A is upper triangular matrix
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istriup(ILInArray<double> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istriup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istriup: A must be matrix or scalar!");
                int m = A.Size[0];
                int n = A.Size[1];
                for (int c = 0; c < n; c++) {
                    for (int r = c + 1; r < m; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is lower Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a lower Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishesslow(ILInArray<double> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishesslow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int m = A.Size[0];
                int n = A.Size[1];
                if (m != n) return false;
                for (int c = 2; c < n; c++) {
                    for (int r = 0; r < c - 1; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is upper Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishessup(ILInArray<double> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishessup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n - 2; c++) {
                    for (int r = c + 2; r < n; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is Hermitian matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a Hermitian matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishermitian(ILInArray<double> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("ishessup: A must not be null!"); 
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n; c++) {
                    
                    //
                    for (int r = c + 1; r < n; r++) {
                        
                        if (A.GetValue(r, c) != A.GetValue(c, r)) return false;
                    }
                }
                return true;
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Determine if matrix A is a lower triangular matrix
        /// </summary>
        /// <param name="A">Matrix of numeric inner type</param>
        /// <returns>true if A is a lower triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istrilow(ILInArray<fcomplex> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istrilow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istrilow: A must be a matrix!");
                int n = A.Size[1];
                for (int c = 1; c < n; c++) {
                    for (int r = 0; r < c; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        
        /// <summary>
        /// Determine if matrix A is upper triangular matrix
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istriup(ILInArray<fcomplex> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istriup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istriup: A must be matrix or scalar!");
                int m = A.Size[0];
                int n = A.Size[1];
                for (int c = 0; c < n; c++) {
                    for (int r = c + 1; r < m; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is lower Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a lower Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishesslow(ILInArray<fcomplex> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishesslow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int m = A.Size[0];
                int n = A.Size[1];
                if (m != n) return false;
                for (int c = 2; c < n; c++) {
                    for (int r = 0; r < c - 1; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is upper Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishessup(ILInArray<fcomplex> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishessup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n - 2; c++) {
                    for (int r = c + 2; r < n; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is Hermitian matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a Hermitian matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishermitian(ILInArray<fcomplex> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("ishessup: A must not be null!"); 
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n; c++) {
                   
                    if (A.GetValue(c,c).imag != 0.0f) return false; 
                    for (int r = c + 1; r < n; r++) {
                        fcomplex val1 = A.GetValue(r,c); fcomplex val2 = A.GetValue(c,r); if (val1.real != val2.real || val1.imag + val2.imag != 0.0f) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is a lower triangular matrix
        /// </summary>
        /// <param name="A">Matrix of numeric inner type</param>
        /// <returns>true if A is a lower triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istrilow(ILInArray<complex> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istrilow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istrilow: A must be a matrix!");
                int n = A.Size[1];
                for (int c = 1; c < n; c++) {
                    for (int r = 0; r < c; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        
        /// <summary>
        /// Determine if matrix A is upper triangular matrix
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istriup(ILInArray<complex> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istriup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istriup: A must be matrix or scalar!");
                int m = A.Size[0];
                int n = A.Size[1];
                for (int c = 0; c < n; c++) {
                    for (int r = c + 1; r < m; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is lower Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a lower Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishesslow(ILInArray<complex> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishesslow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int m = A.Size[0];
                int n = A.Size[1];
                if (m != n) return false;
                for (int c = 2; c < n; c++) {
                    for (int r = 0; r < c - 1; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is upper Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishessup(ILInArray<complex> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishessup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n - 2; c++) {
                    for (int r = c + 2; r < n; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is Hermitian matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a Hermitian matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishermitian(ILInArray<complex> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("ishessup: A must not be null!"); 
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n; c++) {
                   
                    if (A.GetValue(c,c).imag != 0.0) return false;
                    for (int r = c + 1; r < n; r++) {
                        complex val1 = A.GetValue(r,c); complex val2 = A.GetValue(c,r); if (val1.real != val2.real || val1.imag + val2.imag != 0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is a lower triangular matrix
        /// </summary>
        /// <param name="A">Matrix of numeric inner type</param>
        /// <returns>true if A is a lower triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istrilow(ILInArray<byte> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istrilow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istrilow: A must be a matrix!");
                int n = A.Size[1];
                for (int c = 1; c < n; c++) {
                    for (int r = 0; r < c; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        
        /// <summary>
        /// Determine if matrix A is upper triangular matrix
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istriup(ILInArray<byte> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istriup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istriup: A must be matrix or scalar!");
                int m = A.Size[0];
                int n = A.Size[1];
                for (int c = 0; c < n; c++) {
                    for (int r = c + 1; r < m; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is lower Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a lower Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishesslow(ILInArray<byte> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishesslow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int m = A.Size[0];
                int n = A.Size[1];
                if (m != n) return false;
                for (int c = 2; c < n; c++) {
                    for (int r = 0; r < c - 1; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is upper Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishessup(ILInArray<byte> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishessup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n - 2; c++) {
                    for (int r = c + 2; r < n; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is Hermitian matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a Hermitian matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishermitian(ILInArray<byte> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("ishessup: A must not be null!"); 
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n; c++) {
                   
                    
                    for (int r = c + 1; r < n; r++) {
                        if (A.GetValue(r,c) != A.GetValue(c,r)) return false; 
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is a lower triangular matrix
        /// </summary>
        /// <param name="A">Matrix of numeric inner type</param>
        /// <returns>true if A is a lower triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istrilow(ILInArray<Int64> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istrilow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istrilow: A must be a matrix!");
                int n = A.Size[1];
                for (int c = 1; c < n; c++) {
                    for (int r = 0; r < c; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        
        /// <summary>
        /// Determine if matrix A is upper triangular matrix
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istriup(ILInArray<Int64> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istriup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istriup: A must be matrix or scalar!");
                int m = A.Size[0];
                int n = A.Size[1];
                for (int c = 0; c < n; c++) {
                    for (int r = c + 1; r < m; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is lower Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a lower Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishesslow(ILInArray<Int64> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishesslow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int m = A.Size[0];
                int n = A.Size[1];
                if (m != n) return false;
                for (int c = 2; c < n; c++) {
                    for (int r = 0; r < c - 1; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is upper Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishessup(ILInArray<Int64> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishessup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n - 2; c++) {
                    for (int r = c + 2; r < n; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is Hermitian matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a Hermitian matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishermitian(ILInArray<Int64> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("ishessup: A must not be null!"); 
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n; c++) {
                   
                    
                    for (int r = c + 1; r < n; r++) {
                        if (A.GetValue(r,c) != A.GetValue(c,r)) return false; 
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is a lower triangular matrix
        /// </summary>
        /// <param name="A">Matrix of numeric inner type</param>
        /// <returns>true if A is a lower triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istrilow(ILInArray<Int32> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istrilow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istrilow: A must be a matrix!");
                int n = A.Size[1];
                for (int c = 1; c < n; c++) {
                    for (int r = 0; r < c; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        
        /// <summary>
        /// Determine if matrix A is upper triangular matrix
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istriup(ILInArray<Int32> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istriup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istriup: A must be matrix or scalar!");
                int m = A.Size[0];
                int n = A.Size[1];
                for (int c = 0; c < n; c++) {
                    for (int r = c + 1; r < m; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is lower Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a lower Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishesslow(ILInArray<Int32> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishesslow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int m = A.Size[0];
                int n = A.Size[1];
                if (m != n) return false;
                for (int c = 2; c < n; c++) {
                    for (int r = 0; r < c - 1; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is upper Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishessup(ILInArray<Int32> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishessup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n - 2; c++) {
                    for (int r = c + 2; r < n; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is Hermitian matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a Hermitian matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishermitian(ILInArray<Int32> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("ishessup: A must not be null!"); 
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n; c++) {
                   
                    
                    for (int r = c + 1; r < n; r++) {
                        if (A.GetValue(r,c) != A.GetValue(c,r)) return false; 
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is a lower triangular matrix
        /// </summary>
        /// <param name="A">Matrix of numeric inner type</param>
        /// <returns>true if A is a lower triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istrilow(ILInArray<float> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istrilow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istrilow: A must be a matrix!");
                int n = A.Size[1];
                for (int c = 1; c < n; c++) {
                    for (int r = 0; r < c; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        
        /// <summary>
        /// Determine if matrix A is upper triangular matrix
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper triangular matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was not a matrix or if A was null</exception>
        public static bool istriup(ILInArray<float> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("istriup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix)
                    throw new ILArgumentException("istriup: A must be matrix or scalar!");
                int m = A.Size[0];
                int n = A.Size[1];
                for (int c = 0; c < n; c++) {
                    for (int r = c + 1; r < m; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is lower Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a lower Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishesslow(ILInArray<float> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishesslow: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int m = A.Size[0];
                int n = A.Size[1];
                if (m != n) return false;
                for (int c = 2; c < n; c++) {
                    for (int r = 0; r < c - 1; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is upper Hessenberg matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a upper Hessenberg matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishessup(ILInArray<float> A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("ishessup: A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n - 2; c++) {
                    for (int r = c + 2; r < n; r++) {
                        if (A.GetValue(r, c) !=  0.0) return false;
                    }
                }
                return true;
            }
        }
        /// <summary>
        /// Determine if matrix A is Hermitian matrix 
        /// </summary>
        /// <param name="A">Matrix or scalar A of numeric inner type</param>
        /// <returns>true if A is a Hermitian matrix, false otherwise</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if A was null</exception>
        public static bool ishermitian(ILInArray<float> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("ishessup: A must not be null!"); 
            using (ILScope.Enter(A)) {
                if (A.IsScalar) { return true; }
                if (A.IsEmpty) { return false; }
                if (!A.IsMatrix) return false;
                int n = A.Size[1];
                if (n != A.Size[0]) return false;
                for (int c = 0; c < n; c++) {
                   
                    
                    for (int r = c + 1; r < n; r++) {
                        if (A.GetValue(r,c) != A.GetValue(c,r)) return false; 
                    }
                }
                return true;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}
