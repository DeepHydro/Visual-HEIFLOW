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


#pragma warning disable 
using System;
using System.Collections.Generic;
using System.Text;
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Exceptions; 


namespace ILNumerics {

    public partial class ILMath {


        /// <summary>
        /// Maximum for all elements of A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns><para>Scalar maximum of all elements for A</para>
        /// <para>If A is empty, an empty array will be returned.</para></returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null.</exception>
        /// <seealso cref="ILNumerics.ILMath.max"/>
        public static ILRetArray<double> maxall(ILInArray<double> A) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null))
                    throw new ILArgumentException("input argument must not be null");
                if (A.IsEmpty) {
                    return empty<double>(ILSize.Empty00);
                }
                
                double retVal =  double.MinValue;
                unsafe {
                    fixed ( double* inArrStart = A.GetArrayForRead()) {
                        
                        double* inArrWalk = inArrStart;
                        
                        double* inArrEnd = inArrStart + A.Size.NumberOfElements;
                        while (inArrWalk < inArrEnd) {
                            if (retVal < *inArrWalk)
                                retVal = *inArrWalk;
                            inArrWalk++;
                        }
                    }
                }
                return array<double>(retVal, 1, 1);
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Maximum for all elements of A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns><para>Scalar maximum of all elements for A</para>
        /// <para>If A is empty, an empty array will be returned.</para></returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null.</exception>
        /// <seealso cref="ILNumerics.ILMath.max"/>
        public static ILRetArray<Int64> maxall(ILInArray<Int64> A) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null))
                    throw new ILArgumentException("input argument must not be null");
                if (A.IsEmpty) {
                    return empty<Int64>(ILSize.Empty00);
                }
               
                Int64 retVal =  Int64.MinValue;
                unsafe {
                    fixed ( Int64* inArrStart = A.GetArrayForRead()) {
                       
                        Int64* inArrWalk = inArrStart;
                       
                        Int64* inArrEnd = inArrStart + A.Size.NumberOfElements;
                        while (inArrWalk < inArrEnd) {
                            if (retVal < *inArrWalk)
                                retVal = *inArrWalk;
                            inArrWalk++;
                        }
                    }
                }
                return array<Int64>(retVal, 1, 1);
            }
        }
        /// <summary>
        /// Maximum for all elements of A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns><para>Scalar maximum of all elements for A</para>
        /// <para>If A is empty, an empty array will be returned.</para></returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null.</exception>
        /// <seealso cref="ILNumerics.ILMath.max"/>
        public static ILRetArray<Int32> maxall(ILInArray<Int32> A) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null))
                    throw new ILArgumentException("input argument must not be null");
                if (A.IsEmpty) {
                    return empty<Int32>(ILSize.Empty00);
                }
               
                Int32 retVal =  Int32.MinValue;
                unsafe {
                    fixed ( Int32* inArrStart = A.GetArrayForRead()) {
                       
                        Int32* inArrWalk = inArrStart;
                       
                        Int32* inArrEnd = inArrStart + A.Size.NumberOfElements;
                        while (inArrWalk < inArrEnd) {
                            if (retVal < *inArrWalk)
                                retVal = *inArrWalk;
                            inArrWalk++;
                        }
                    }
                }
                return array<Int32>(retVal, 1, 1);
            }
        }
        /// <summary>
        /// Maximum for all elements of A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns><para>Scalar maximum of all elements for A</para>
        /// <para>If A is empty, an empty array will be returned.</para></returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null.</exception>
        /// <seealso cref="ILNumerics.ILMath.max"/>
        public static ILRetArray<byte> maxall(ILInArray<byte> A) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null))
                    throw new ILArgumentException("input argument must not be null");
                if (A.IsEmpty) {
                    return empty<byte>(ILSize.Empty00);
                }
               
                byte retVal =  byte.MinValue;
                unsafe {
                    fixed ( byte* inArrStart = A.GetArrayForRead()) {
                       
                        byte* inArrWalk = inArrStart;
                       
                        byte* inArrEnd = inArrStart + A.Size.NumberOfElements;
                        while (inArrWalk < inArrEnd) {
                            if (retVal < *inArrWalk)
                                retVal = *inArrWalk;
                            inArrWalk++;
                        }
                    }
                }
                return array<byte>(retVal, 1, 1);
            }
        }
        /// <summary>
        /// Maximum for all elements of A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns><para>Scalar maximum of all elements for A</para>
        /// <para>If A is empty, an empty array will be returned.</para></returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If A was null.</exception>
        /// <seealso cref="ILNumerics.ILMath.max"/>
        public static ILRetArray<float> maxall(ILInArray<float> A) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null))
                    throw new ILArgumentException("input argument must not be null");
                if (A.IsEmpty) {
                    return empty<float>(ILSize.Empty00);
                }
               
                float retVal =  float.MinValue;
                unsafe {
                    fixed ( float* inArrStart = A.GetArrayForRead()) {
                       
                        float* inArrWalk = inArrStart;
                       
                        float* inArrEnd = inArrStart + A.Size.NumberOfElements;
                        while (inArrWalk < inArrEnd) {
                            if (retVal < *inArrWalk)
                                retVal = *inArrWalk;
                            inArrWalk++;
                        }
                    }
                }
                return array<float>(retVal, 1, 1);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}
