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
        /// Create new double array, set initial values to 1
        /// </summary>
        /// <returns>array</returns>
        public static ILRetArray<double> ones(params int[] dimensions) {
            return (ILRetArray<double>)ones(NumericType.Double, new ILSize(dimensions));
        }
        /// <summary>
        /// Create array initialized with all elements set to one
        /// </summary>
        /// <param name="type">Numeric type specification. One value out of the types listed in the <see cred="ILNumerics.NumericType"/>
        /// enum.</param>
        /// <param name="size">Size descriptor</param>
        /// <returns>Array of inner type corresponding to <paramref name="type"/> argument.</returns>
        /// <remarks>The array returned must be casted to the appropriate actual type afterwards and assigned to a concrete array! Caution: 
        /// This overload is provided for compatibility reasons only! Use <see cref="ones{T}(ILSize)"/> instead.
        /// <para>
        /// <list type="number"> 
        /// <listheader>The following types are supported: </listheader>
        /// <item>Double</item>
        /// <item>Single</item>
        /// <item>Complex</item>
        /// <item>FComplex</item>
        /// <item>Byte</item>
        /// <item>Int32</item>
        /// <item>Int64</item>
        /// </list>
        /// </para>
        /// </remarks>
        public static ILBaseArray ones(NumericType type, ILSize size) {
            switch (type) {
                case NumericType.Double:
                    return array<double>(1.0, size);
                case NumericType.Single:
                    return array<float>(1f, size);
                case NumericType.Complex:
                    return array<complex>(new complex(1, 0), size);
                case NumericType.FComplex:
                    return array<fcomplex>(new fcomplex(1f, 0f), size);
                case NumericType.Byte:
                    return array<byte>(1, size);
                case NumericType.Int32:
                    return array<int>(1, size).T;
                case NumericType.Int64:
                    return array<long>(1, size);
            }
            return null;
        }
        /// <summary>
        /// Create array initialized with all elements set to one
        /// </summary>
        /// <typeparam name="T">Numeric type specification.</typeparam>
        /// <param name="size">Size descriptor</param>
        /// <returns>Array of inner type corresponding to the given type.</returns>
        ///  <remarks>The array returned may be casted to the appropriate actual type afterwards. 
        /// <para>
        /// <list type="number"> 
        /// <listheader>The following types are supported: </listheader>
        /// <item>Double</item>
        /// <item>Single</item>
        /// <item>complex</item>
        /// <item>fcomplex</item>
        /// <item>Byte</item>
        /// <item>Int32</item>
        /// <item>Int64</item>
        /// </list>
        /// </para>
        /// </remarks>
        public static ILRetArray<T> ones<T>(params int[] size) {
            return ones<T>(new ILSize(size)); 
        }
        /// <summary>
        /// Create array initialized with all elements set to one
        /// </summary>
        /// <typeparam name="T">Numeric type specification.</typeparam>
        /// <param name="size">Size descriptor</param>
        /// <returns>Array of inner type corresponding to the given type.</returns>
        ///  <remarks>The array returned may be casted to the appropriate actual type afterwards. 
        /// <para>
        /// <list type="number"> 
        /// <listheader>The following types are supported: </listheader>
        /// <item>Double</item>
        /// <item>Single</item>
        /// <item>complex</item>
        /// <item>fcomplex</item>
        /// <item>Byte</item>
        /// <item>Int32</item>
        /// <item>Int64</item>
        /// </list>
        /// </para>
        /// </remarks>
        public static ILRetArray<T> ones<T>(ILSize size) {
            using (ILScope.Enter()) {
                if (typeof(T) == typeof(double)) {
                    return (ILRetArray<T>)(object)array<double>(1.0, size);
                } else if (typeof(T) == typeof(float)) {
                    return (ILRetArray<T>)(object)array<float>(1f, size);
                } else if (typeof(T) == typeof(int)) {
                    return (ILRetArray<T>)(object)array<int>(1, size);
                } else if (typeof(T) == typeof(long)) {
                    return (ILRetArray<T>)(object)array<long>(1, size);
                } else if (typeof(T) == typeof(complex)) {
                    return (ILRetArray<T>)(object)array<complex>(new complex(1, 0), size);
                } else if (typeof(T) == typeof(fcomplex)) {
                    return (ILRetArray<T>)(object)array<fcomplex>(new fcomplex(1f, 0f), size);
                } else if (typeof(T) == typeof(byte)) {
                    return (ILRetArray<T>)(object)array<byte>(1, size);
                } else if (typeof(T) == typeof(short)) {
                    return (ILRetArray<T>)(object)array<short>(1, size);
                }
                return null;
            }
        }
        /// <summary>
        /// Create new double array, set initial values to 1
        /// </summary>
        /// <returns>Array</returns>
        public static ILRetArray<double> ones(ILSize dimensions) {
            return (ILRetArray<double>)ones(NumericType.Double, dimensions);
        }

    }
}
