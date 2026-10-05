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
        /// Create empty double array of size [0,0].
        /// </summary>
        /// <returns>Empty array</returns>
        public static ILRetArray<double> empty() {
            return ILRetArray<double>.empty(ILSize.Empty00);
        }
        /// <summary>
        /// Create empty double array of specific size
        /// </summary>
        /// <param name="size">(Empty-) size of new empty array</param>
        /// <returns>Empty array</returns>
        public static ILRetArray<T> empty<T>(ILSize size) {
            if (size.NumberOfElements > 0)
                throw new ILArgumentException("the 'size' parameter must define an empty array size"); 
            return new ILRetArray<T>(size);
        }
        /// <summary>
        /// Create empty double array of specific size
        /// </summary>
        /// <param name="size">(Empty-) size of new empty array</param>
        /// <returns>Empty array</returns>
        public static ILRetArray<T> empty<T>(params int[] size) {
            ILSize newDims = new ILSize(size);
            if (newDims.NumberOfElements > 0)
                throw new ILArgumentException("the 'size' parameter must define an empty array size");
            return new ILRetArray<T>(newDims);
        }
        /// <summary>
        /// Create empty array of size [0,0] and arbitrary element type
        /// </summary>
        /// <returns>Empty array</returns>
        public static ILRetArray<T> empty<T>() {
            return new ILRetArray<T>(ILSize.Empty00);
        }
        /// <summary>
        /// Create empty double array of specific size
        /// </summary>
        /// <param name="size">(Empty-) size of new empty array</param>
        /// <returns>Empty array</returns>
        public static ILRetArray<double> empty(ILSize size) {
            if (size.NumberOfElements > 0)
                throw new ILArgumentException("the 'size' parameter must define an empty array size");
            return new ILRetArray<double>(size);
        }
        /// <summary>
        /// Create empty double array of specific size
        /// </summary>
        /// <param name="size">(Empty-) size of new empty array</param>
        /// <returns>Empty array</returns>
        public static ILRetArray<double> empty(params int[] size) {
            ILSize newDims = new ILSize(size);
            if (newDims.NumberOfElements > 0)
                throw new ILArgumentException("the 'size' parameter must define an empty array size");
            return new ILRetArray<double>(newDims);
        }
        /// <summary>
        /// Create new empty array, used for array class members
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <returns>Empty array which can afterwards be used for arbitrary assignements (Assign(), or 'array.A = ..' assignements)</returns>
        /// <remarks>The array returned will be an empty array initially. Its main purpose is to provide 
        /// a persistant array initialization. The array will not be disposed after leaving the current scope
        /// and can therefore be utilized for initializing class attributes. After initialization, use the 'ILArray.A = ...' property (C#) 
        /// or the ILArray.Assign() function to assign new values to the array.</remarks>
        public static ILArray<T> localMember<T>() {
            return new ILArray<T>(new ILDenseStorage<T>(ILSize.Empty00), false);
        }

    }
}
