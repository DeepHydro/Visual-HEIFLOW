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

namespace ILNumerics  {
    public partial class ILMath {

        /// <summary>Determine if array has nonzero elements in all dimensions</summary>
        /// <param name="A">Input Array</param>
        /// <returns>Scalar logical array</returns>
        /// <remarks><para>The function evaluates all elements of A in all dimensions. It returnes 'true' (1) if
        /// all elements are found to be non-zero, false otherwise.</para>
        /// <para>If A is empty, a scalar logical 'false' (0) is returned. </para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null.</exception>
        /// <seealso cref="ILNumerics.ILMath.all(ILInArray{double},int)"/>
        public static ILRetLogical allall (ILInArray<double> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return new ILRetLogical(ILSize.Empty00); 
                } 
                byte retArr = 1; 
                unsafe {
                    fixed ( double * inArrStart = A.GetArrayForRead()) {
                         double * inArrWalk = inArrStart; 
                         double * inArrEnd = inArrStart + A.Size.NumberOfElements; 
                        while (inArrWalk < inArrEnd) {
                            if ( *inArrWalk++ == 0) {
                                retArr = 0; 
                                break; 
                            }
                        }
                    }
                }
                return new ILRetLogical (new byte [1] {retArr},1,1);
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>Determine if array has nonzero elements in all dimensions</summary>
        /// <param name="A">Input Array</param>
        /// <returns>Scalar logical array</returns>
        /// <remarks><para>The function evaluates all elements of A in all dimensions. It returnes 'true' (1) if
        /// all elements are found to be non-zero, false otherwise.</para>
        /// <para>If A is empty, a scalar logical 'false' (0) is returned. </para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null.</exception>
        /// <seealso cref="ILNumerics.ILMath.all(ILInArray{double},int)"/>
        public static ILRetLogical allall (ILInArray<Int64> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return new ILRetLogical(ILSize.Empty00); 
                } 
                byte retArr = 1; 
                unsafe {
                    fixed ( Int64 * inArrStart = A.GetArrayForRead()) {
                        Int64 * inArrWalk = inArrStart; 
                        Int64 * inArrEnd = inArrStart + A.Size.NumberOfElements; 
                        while (inArrWalk < inArrEnd) {
                            if ( *inArrWalk++ == 0) {
                                retArr = 0; 
                                break; 
                            }
                        }
                    }
                }
                return new ILRetLogical (new byte [1] {retArr},1,1);
            }
        }
        /// <summary>Determine if array has nonzero elements in all dimensions</summary>
        /// <param name="A">Input Array</param>
        /// <returns>Scalar logical array</returns>
        /// <remarks><para>The function evaluates all elements of A in all dimensions. It returnes 'true' (1) if
        /// all elements are found to be non-zero, false otherwise.</para>
        /// <para>If A is empty, a scalar logical 'false' (0) is returned. </para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null.</exception>
        /// <seealso cref="ILNumerics.ILMath.all(ILInArray{double},int)"/>
        public static ILRetLogical allall (ILInArray<Int32> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return new ILRetLogical(ILSize.Empty00); 
                } 
                byte retArr = 1; 
                unsafe {
                    fixed ( Int32 * inArrStart = A.GetArrayForRead()) {
                        Int32 * inArrWalk = inArrStart; 
                        Int32 * inArrEnd = inArrStart + A.Size.NumberOfElements; 
                        while (inArrWalk < inArrEnd) {
                            if ( *inArrWalk++ == 0) {
                                retArr = 0; 
                                break; 
                            }
                        }
                    }
                }
                return new ILRetLogical (new byte [1] {retArr},1,1);
            }
        }
        /// <summary>Determine if array has nonzero elements in all dimensions</summary>
        /// <param name="A">Input Array</param>
        /// <returns>Scalar logical array</returns>
        /// <remarks><para>The function evaluates all elements of A in all dimensions. It returnes 'true' (1) if
        /// all elements are found to be non-zero, false otherwise.</para>
        /// <para>If A is empty, a scalar logical 'false' (0) is returned. </para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null.</exception>
        /// <seealso cref="ILNumerics.ILMath.all(ILInArray{double},int)"/>
        public static ILRetLogical allall (ILInArray<byte> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return new ILRetLogical(ILSize.Empty00); 
                } 
                byte retArr = 1; 
                unsafe {
                    fixed ( byte * inArrStart = A.GetArrayForRead()) {
                        byte * inArrWalk = inArrStart; 
                        byte * inArrEnd = inArrStart + A.Size.NumberOfElements; 
                        while (inArrWalk < inArrEnd) {
                            if ( *inArrWalk++ == 0) {
                                retArr = 0; 
                                break; 
                            }
                        }
                    }
                }
                return new ILRetLogical (new byte [1] {retArr},1,1);
            }
        }
        /// <summary>Determine if array has nonzero elements in all dimensions</summary>
        /// <param name="A">Input Array</param>
        /// <returns>Scalar logical array</returns>
        /// <remarks><para>The function evaluates all elements of A in all dimensions. It returnes 'true' (1) if
        /// all elements are found to be non-zero, false otherwise.</para>
        /// <para>If A is empty, a scalar logical 'false' (0) is returned. </para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null.</exception>
        /// <seealso cref="ILNumerics.ILMath.all(ILInArray{double},int)"/>
        public static ILRetLogical allall (ILInArray<fcomplex> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return new ILRetLogical(ILSize.Empty00); 
                } 
                byte retArr = 1; 
                unsafe {
                    fixed ( fcomplex * inArrStart = A.GetArrayForRead()) {
                        fcomplex * inArrWalk = inArrStart; 
                        fcomplex * inArrEnd = inArrStart + A.Size.NumberOfElements; 
                        while (inArrWalk < inArrEnd) {
                            if ( *inArrWalk++ == 0) {
                                retArr = 0; 
                                break; 
                            }
                        }
                    }
                }
                return new ILRetLogical (new byte [1] {retArr},1,1);
            }
        }
        /// <summary>Determine if array has nonzero elements in all dimensions</summary>
        /// <param name="A">Input Array</param>
        /// <returns>Scalar logical array</returns>
        /// <remarks><para>The function evaluates all elements of A in all dimensions. It returnes 'true' (1) if
        /// all elements are found to be non-zero, false otherwise.</para>
        /// <para>If A is empty, a scalar logical 'false' (0) is returned. </para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null.</exception>
        /// <seealso cref="ILNumerics.ILMath.all(ILInArray{double},int)"/>
        public static ILRetLogical allall (ILInArray<float> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return new ILRetLogical(ILSize.Empty00); 
                } 
                byte retArr = 1; 
                unsafe {
                    fixed ( float * inArrStart = A.GetArrayForRead()) {
                        float * inArrWalk = inArrStart; 
                        float * inArrEnd = inArrStart + A.Size.NumberOfElements; 
                        while (inArrWalk < inArrEnd) {
                            if ( *inArrWalk++ == 0) {
                                retArr = 0; 
                                break; 
                            }
                        }
                    }
                }
                return new ILRetLogical (new byte [1] {retArr},1,1);
            }
        }
        /// <summary>Determine if array has nonzero elements in all dimensions</summary>
        /// <param name="A">Input Array</param>
        /// <returns>Scalar logical array</returns>
        /// <remarks><para>The function evaluates all elements of A in all dimensions. It returnes 'true' (1) if
        /// all elements are found to be non-zero, false otherwise.</para>
        /// <para>If A is empty, a scalar logical 'false' (0) is returned. </para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null.</exception>
        /// <seealso cref="ILNumerics.ILMath.all(ILInArray{double},int)"/>
        public static ILRetLogical allall (ILInArray<complex> A) {
            if (object.Equals(A,null))
                throw new ILArgumentException("A must not be null!");
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return new ILRetLogical(ILSize.Empty00); 
                } 
                byte retArr = 1; 
                unsafe {
                    fixed ( complex * inArrStart = A.GetArrayForRead()) {
                        complex * inArrWalk = inArrStart; 
                        complex * inArrEnd = inArrStart + A.Size.NumberOfElements; 
                        while (inArrWalk < inArrEnd) {
                            if ( *inArrWalk++ == 0) {
                                retArr = 0; 
                                break; 
                            }
                        }
                    }
                }
                return new ILRetLogical (new byte [1] {retArr},1,1);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}
