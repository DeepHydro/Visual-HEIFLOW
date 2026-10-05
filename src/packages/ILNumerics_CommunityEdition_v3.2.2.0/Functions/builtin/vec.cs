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

        /// <summary>
        /// Regulary spaced vector
        /// </summary>
        /// <param name="start">Start value</param>
        /// <param name="end">End value</param>
        /// <returns>Row vector of size 1xN, where N is the number of elements 
        /// between start and end, all equally spaced with interval 1. The last element 
        /// of the returned vector will be less than or equal to end, if start <![CDATA[<]]> end.</returns>
        /// <remarks>This is the same as vector (start,1,end).</remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If start <![CDATA[>]]> end</exception>
        public static ILRetArray<double> vec(ILBaseArray start, ILBaseArray end) {
            return vec<double>(start, 1, end);
        }

        /// <summary>
        /// Regularly spaced vector, spacing 1
        /// </summary>
        /// <param name="start">Start value</param>
        /// <param name="end">End value</param>
        /// <returns>Row vector of size 1xN, where N is the number of elements 
        /// between start and end, all equally spaced with stepsize of 1. The last element 
        /// of the returned vector will be less than or equal to end, if start &lt; end. </returns>
        public static ILRetArray<T> vec<T>(ILBaseArray start, ILBaseArray end) {
            return vec<T>(start, 1, end);
        }

        /// <summary>
        /// Regulary spaced column vector
        /// </summary>
        /// <param name="start">Start value</param>
        /// <param name="step">Step size</param>
        /// <param name="end">End value</param>
        /// <returns>Column vector of length N, where N is the number of elements 
        /// between start and end, all equally spaced with stepsize of 'step'. The last element 
        /// of the returned vector will be less than or equal to end, if start &lt; end. If start 
        /// &gt; end, the elements in the vector will linearly decrease from 
        /// start to end. In this case, step must be negative.</returns>
        /// <remarks><para>The shape of the vector created is controlled by the setting switch <see cref="ILNumerics.Settings.CreateRowVectorsByDefault"/>.
        /// This setting defaults to 'false' which will cause the creation of a column vector. </para></remarks>
        public static ILRetArray<T> vec<T>(ILBaseArray start, ILBaseArray step, ILBaseArray end) {
            using (ILScope.Enter(start, step, end)) {
                if (object.Equals(start, null) || !start.IsNumeric || !start.IsScalar) throw new ILArgumentException("start must be numeric scalar");
                if (object.Equals(step, null) || !step.IsNumeric || !step.IsScalar) throw new ILArgumentException("step must be numeric scalar");
                if (object.Equals(end, null) || !end.IsNumeric || !end.IsScalar) throw new ILArgumentException("end must be numeric scalar");
                double dStart = todouble(start).GetValue(0);
                double dStep = todouble(step).GetValue(0);
                double dEnd = todouble(end).GetValue(0);

                if (dStep == 0)
                    throw new ILArgumentException("step must not be 0");
                if (dStart > dEnd && dStep > 0)
                    throw new ILArgumentException("if start > end, step must be negativ");
                if (dStart < dEnd && dStep < 0)
                    throw new ILArgumentException("if start < end, step must be positiv");
                int nrElements = (int)Math.Floor((dEnd - dStart) / dStep) + 1;
                T[] data = ILMemoryPool.Pool.New<T>(nrElements);
                if (false) {

                    
                } else if (data is  double[]) {
                    unsafe {
                        
                        double val = ( double)dStart;
                        fixed ( double* pDataConst = (data as  double[])) {
                            
                            double* pData = pDataConst;
                            
                            double* lastElement = pDataConst + nrElements;
                            while (pData < lastElement) {
                                *pData++ = val;
                                val = ( double)(val + dStep);
                            }
                        }
                    }

#region HYCALPER AUTO GENERATED CODE

                   
                } else if (data is  byte[]) {
                    unsafe {
                       
                        byte val = ( byte)dStart;
                        fixed ( byte* pDataConst = (data as  byte[])) {
                           
                            byte* pData = pDataConst;
                           
                            byte* lastElement = pDataConst + nrElements;
                            while (pData < lastElement) {
                                *pData++ = val;
                                val = ( byte)(val + dStep);
                            }
                        }
                    }
                   
                } else if (data is  Int64[]) {
                    unsafe {
                       
                        Int64 val = ( Int64)dStart;
                        fixed ( Int64* pDataConst = (data as  Int64[])) {
                           
                            Int64* pData = pDataConst;
                           
                            Int64* lastElement = pDataConst + nrElements;
                            while (pData < lastElement) {
                                *pData++ = val;
                                val = ( Int64)(val + dStep);
                            }
                        }
                    }
                   
                } else if (data is  complex[]) {
                    unsafe {
                       
                        complex val = ( complex)dStart;
                        fixed ( complex* pDataConst = (data as  complex[])) {
                           
                            complex* pData = pDataConst;
                           
                            complex* lastElement = pDataConst + nrElements;
                            while (pData < lastElement) {
                                *pData++ = val;
                                val = ( complex)(val + dStep);
                            }
                        }
                    }
                   
                } else if (data is  fcomplex[]) {
                    unsafe {
                       
                        fcomplex val = ( fcomplex)dStart;
                        fixed ( fcomplex* pDataConst = (data as  fcomplex[])) {
                           
                            fcomplex* pData = pDataConst;
                           
                            fcomplex* lastElement = pDataConst + nrElements;
                            while (pData < lastElement) {
                                *pData++ = val;
                                val = ( fcomplex)(val + dStep);
                            }
                        }
                    }
                   
                } else if (data is  Int32[]) {
                    unsafe {
                       
                        Int32 val = ( Int32)dStart;
                        fixed ( Int32* pDataConst = (data as  Int32[])) {
                           
                            Int32* pData = pDataConst;
                           
                            Int32* lastElement = pDataConst + nrElements;
                            while (pData < lastElement) {
                                *pData++ = val;
                                val = ( Int32)(val + dStep);
                            }
                        }
                    }
                   
                } else if (data is  float[]) {
                    unsafe {
                       
                        float val = ( float)dStart;
                        fixed ( float* pDataConst = (data as  float[])) {
                           
                            float* pData = pDataConst;
                           
                            float* lastElement = pDataConst + nrElements;
                            while (pData < lastElement) {
                                *pData++ = val;
                                val = ( float)(val + dStep);
                            }
                        }
                    }

#endregion HYCALPER AUTO GENERATED CODE
               } else {
                    throw new ILArgumentException(String.Format("vec is not defined for arrays of inner type '{0}'", typeof(T).Name));
                }
                if (Settings.CreateRowVectorsByDefault) {
                    return new ILRetArray<T>(data, 1, nrElements);
                } else {
                    return new ILRetArray<T>(data, nrElements, 1);
                }
            }
        }

    }
}
