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
using System.Runtime.InteropServices; 
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Native;
using ILNumerics.Exceptions;

namespace ILNumerics {

    public partial class ILMath {

        #region counter
        /// <summary>
        /// Create n-dimensional array with elements counting from 1, double precision
        /// </summary>
        /// <param name="dimensions">Variable number of numeric scalar arrays with dimension specification</param>
        /// <returns>Double array with elements counting from 1 to dimensions.NumberOfElements</returns>
        /// <remarks>This function may be used for the convenient creation of arrays for testing purposes.</remarks>
        public static ILRetArray<double> counter(params ILBaseArray[] dimensions) {
            return counter<double>(1.0, 1.0, dimensions);
        }
        /// <summary>
        /// Create n-dimensional array with elements counting from 1, double precision
        /// </summary>
        /// <param name="dimensions">Variable number of numeric scalar arrays with dimension specification</param>
        /// <returns>Double array with elements counting from 1 ... dimensions.NumberOfElements</returns>
        /// <remarks>This function may be used for the convenient creation of arrays for testing purposes.</remarks>
        public static ILRetArray<double> counter(params int[] dimensions) {
            return counter<double>(1.0, 1.0, new ILSize(dimensions));
        }
        /// <summary>
        /// Create n-dimensional array with elements counting from 1, double precision
        /// </summary>
        /// <param name="dimensions">Variable number of numeric scalar arrays with dimension specification</param>
        /// <param name="start">Starting number</param>
        /// <param name="inc">Incrementing step</param>
        /// <returns>Double array with elements counting from start to start + (dimensions.NumberOfElements * inc)</returns>
        /// <remarks>This function may be used for the convenient creation of arrays for testing purposes.</remarks>
        public static ILRetArray<double> counter(double start, double inc, params ILBaseArray[] dimensions) {
            return counter<double>(start, inc, dimensions);
        }
        /// <summary>
        /// Create n-dimensional array with elements counting from 1, double precision
        /// </summary>
        /// <param name="dimensions">Variable number of numeric scalar arrays with dimension specification</param>
        /// <param name="start">Starting number</param>
        /// <param name="inc">Incrementing step</param>
        /// <returns>Double array with elements counting from start to start + (dimensions.NumberOfElements * inc)</returns>
        /// <remarks>This function may be used for the convenient creation of arrays for testing purposes.</remarks>
        public static ILRetArray<double> counter(double start, double inc, ILSize dimensions) {
            return counter<double>(start, inc, dimensions);
        }
        /// <summary>
        /// Create n-dimensional array with elements counting from 1, double precision
        /// </summary>
        /// <param name="dimensions">Variable number of numeric scalar arrays with dimension specification</param>
        /// <returns>Double array with elements counting from 1 ... dimensions.NumberOfElements</returns>
        /// <remarks>This function may be used for the convenient creation of arrays for testing purposes.</remarks>
        public static ILRetArray<double> counter(ILSize dimensions) {
            return counter<double>(1.0, 1.0, dimensions);
        }
        /// <summary>
        /// Create n-dimensional array with counting elements 
        /// </summary>
        /// <param name="start">Initial value</param>
        /// <param name="increment">Increment for each element</param>
        /// <param name="dimensions">Variable number of numeric, scalar arrays with dimension specification</param>
        /// <returns>Array with elements counting from <paramref name="start"/> along the first dimension with steps of <paramref name="increment"/>.</returns>
        /// <remarks>
        /// <example><code>
        /// // This will create elements counting from 1...24: 
        /// <![CDATA[ILArray<double>]]> A = ILMath.counter(4,3,2); 
        /// // This will create elements counting from 1...48 with intervals of 2.0: 
        /// <![CDATA[ILArray<double>]]> A = ILMath.counter(1.0,2.0,4,3,2); 
        /// // This will create an array with all elements having contant value of -4f:
        /// // (note: start, increment and dimension specifier do not need to be of the same type)
        /// <![CDATA[ILArray<float> A = ILMath.counter<float>(4.0,0,4,3,2);]]> 
        /// </code></example>  </remarks>
        public static ILRetArray<T> counter<T>(ILBaseArray start, ILBaseArray increment, params ILBaseArray[] dimensions) {
            using (ILScope.Enter(dimensions))
                return counter<T>(start, increment, new ILSize(dimensions));
        }
        /// <summary>
        /// Create n-dimensional array with counting elements 
        /// </summary>
        /// <param name="start">Initial value</param>
        /// <param name="increment">Increment for each element</param>
        /// <param name="dimensions">Variable int array with dimension specification</param>
        /// <returns>Array with elements counting from <paramref name="start"/> to dimensions.NumberOfElements - <paramref name="start"/>.</returns>
        /// <remarks>
        /// <example><code>
        /// // This will create elements counting from 1...24: 
        /// <![CDATA[ILArray<double>]]> A = ILMath.counter(4,3,2); 
        /// // This will create elements counting from 1...48 with intervals of 2.0: 
        /// <![CDATA[ILArray<double>]]> A = ILMath.counter(1.0,2.0,4,3,2); 
        /// // This will create an array with all elements having contant value of -4f:
        /// // (note: start, increment and dimension specifier do not need to be of the same type)
        /// <![CDATA[ILArray<float> A = ILMath.counter<float>(4.0,0,4,3,2);]]> 
        /// </code></example>  </remarks>
        public static ILRetArray<T> counter<T>(ILBaseArray start, ILBaseArray increment, ILSize dimensions) {
            using (ILScope.Enter(start, increment)) {
                if (object.Equals(start, null) || !start.IsNumeric || !start.IsScalar) throw new ILArgumentException("start must be a numeric scalar");
                if (object.Equals(increment, null) || !increment.IsNumeric || !increment.IsScalar) throw new ILArgumentException("increment must be a numeric scalar");
                double dStart = todouble(start).GetValue(0);
                double dInc = todouble(increment).GetValue(0);

                T[] retArr = ILMemoryPool.Pool.New<T>(dimensions.NumberOfElements);
                if (false) {

                    
                } else if (retArr is  double[]) {
                    
                    double val = ( double)dStart;
                    unsafe {
                        fixed ( double* pRetArr = (retArr as  double[])) {
                            
                            double* pRetArray = pRetArr;
                            
                            double* pEnd = pRetArr + dimensions.NumberOfElements;
                            while (pRetArray < pEnd) {
                                *pRetArray++ = val;
                                val = ( double)(val + dInc);
                            }
                        }
                    }

#region HYCALPER AUTO GENERATED CODE

                   
                } else if (retArr is  byte[]) {
                   
                    byte val = ( byte)dStart;
                    unsafe {
                        fixed ( byte* pRetArr = (retArr as  byte[])) {
                           
                            byte* pRetArray = pRetArr;
                           
                            byte* pEnd = pRetArr + dimensions.NumberOfElements;
                            while (pRetArray < pEnd) {
                                *pRetArray++ = val;
                                val = ( byte)(val + dInc);
                            }
                        }
                    }
                   
                } else if (retArr is  Int64[]) {
                   
                    Int64 val = ( Int64)dStart;
                    unsafe {
                        fixed ( Int64* pRetArr = (retArr as  Int64[])) {
                           
                            Int64* pRetArray = pRetArr;
                           
                            Int64* pEnd = pRetArr + dimensions.NumberOfElements;
                            while (pRetArray < pEnd) {
                                *pRetArray++ = val;
                                val = ( Int64)(val + dInc);
                            }
                        }
                    }
                   
                } else if (retArr is  complex[]) {
                   
                    complex val = ( complex)dStart;
                    unsafe {
                        fixed ( complex* pRetArr = (retArr as  complex[])) {
                           
                            complex* pRetArray = pRetArr;
                           
                            complex* pEnd = pRetArr + dimensions.NumberOfElements;
                            while (pRetArray < pEnd) {
                                *pRetArray++ = val;
                                val = ( complex)(val + dInc);
                            }
                        }
                    }
                   
                } else if (retArr is  fcomplex[]) {
                   
                    fcomplex val = ( fcomplex)dStart;
                    unsafe {
                        fixed ( fcomplex* pRetArr = (retArr as  fcomplex[])) {
                           
                            fcomplex* pRetArray = pRetArr;
                           
                            fcomplex* pEnd = pRetArr + dimensions.NumberOfElements;
                            while (pRetArray < pEnd) {
                                *pRetArray++ = val;
                                val = ( fcomplex)(val + dInc);
                            }
                        }
                    }
                   
                } else if (retArr is  Int32[]) {
                   
                    Int32 val = ( Int32)dStart;
                    unsafe {
                        fixed ( Int32* pRetArr = (retArr as  Int32[])) {
                           
                            Int32* pRetArray = pRetArr;
                           
                            Int32* pEnd = pRetArr + dimensions.NumberOfElements;
                            while (pRetArray < pEnd) {
                                *pRetArray++ = val;
                                val = ( Int32)(val + dInc);
                            }
                        }
                    }
                   
                } else if (retArr is  float[]) {
                   
                    float val = ( float)dStart;
                    unsafe {
                        fixed ( float* pRetArr = (retArr as  float[])) {
                           
                            float* pRetArray = pRetArr;
                           
                            float* pEnd = pRetArr + dimensions.NumberOfElements;
                            while (pRetArray < pEnd) {
                                *pRetArray++ = val;
                                val = ( float)(val + dInc);
                            }
                        }
                    }

#endregion HYCALPER AUTO GENERATED CODE
               } else {
                    throw new ILArgumentException(String.Format("counter is not supported for arrays of inner type '{0}'", typeof(T).Name));
                }
                return new ILRetArray<T>(retArr, dimensions);
            }
        }
        #endregion

    }
}
