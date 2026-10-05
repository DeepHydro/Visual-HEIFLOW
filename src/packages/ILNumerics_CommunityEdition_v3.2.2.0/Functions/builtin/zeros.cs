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
        /// Create double array with all elements initialized to 0
        /// </summary>
        /// <param name="size">Size description</param>
        /// <returns>Zeros-filled array.</returns>
        public static ILRetArray<double> zeros(params int[] size) {
            return (ILRetArray<double>)zeros<double>(new ILSize(size));
        }
        /// <summary>
        /// Create double array with all elements initialized to 0
        /// </summary>
        /// <param name="size">Size descriptor</param>
        /// <returns>Zeros-filled array.</returns>
        public static ILRetArray<double> zeros(ILSize size) {
            return (ILRetArray<double>)zeros<double>(size);
        }
        /// <summary>
        /// Create array with all elements initialized to default(T)
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="size">Size descriptor</param>
        /// <returns>New array, initialized to default(T)</returns>
        public static ILRetArray<T> zeros<T>(ILSize size) {
            return array<T>(default(T), size);
        }
        /// <summary>
        /// Create new array of arbitrary element type, initialized to '0'
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="size">Size description</param>
        /// <returns>New array having the size determined by 'dims', initialized to '0'</returns>
        /// <remarks>For T deriving from Sytem.ValueType elements will be '0'. All other element types
        /// will be initialized to default(T).</remarks>
        public static ILRetArray<T> zeros<T>(params int[] size) {
            return array<T>(default(T), new ILSize(size));
        }
        /// <summary>
        /// Create array initialized with all elements set to zero
        /// </summary>
        /// <param name="type">Numeric type specification. One value out of the types listed in the <see cred="ILNumerics.NumericType"/>
        /// enum.</param>
        /// <param name="size">Size descriptor</param>
        /// <returns>Array of inner type corresponding to <paramref name="type"/> argument.</returns>
        /// <remarks>The array returned may be casted to the appropriate actual type afterwards. 
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
        /// <para>This function is provided for downward compatibility reasons only and will be removed in a future update. It is recommended to use the <see cref="zeros{T}(ILSize)"/> or <see cref="zeros{T}(int[])"/> overloads instead.</para>
        /// <para>The interface of this function does not confirm to the rules of functions in ILNumerics. Therefore, in order to prevent for potential memory issues, the return value should be converted to 
        /// a concrete array type explicitely: </para>
        /// <example>
        /// <code>
        /// ILArray&lt;double> A = todouble(zeros(NumericType.double, size(10,20))); 
        /// 
        /// // better and easier would be: 
        /// ILArray&lt;double> B = zeros&lt;double>(10,20); 
        /// </code>
        /// </example>
        /// </remarks>
        [Obsolete("Use overload ILMath.zeros<T> instead!")]
        public static ILBaseArray zeros(NumericType type, ILSize size) {
            switch (type) {
                case NumericType.Double:
                    return zeros<double>(size);
                case NumericType.Single:
                    return zeros<float>(size);
                case NumericType.Complex:
                    return zeros<complex>(size);
                case NumericType.FComplex:
                    return zeros<fcomplex>(size);
                case NumericType.Byte:
                    return zeros<byte>(size);
                case NumericType.Int32:
                    return zeros<int>(size);
                case NumericType.Int64:
                    return zeros<Int64>(size);
                default:
                    return zeros<double>(size);
            }
        }

    }
}
