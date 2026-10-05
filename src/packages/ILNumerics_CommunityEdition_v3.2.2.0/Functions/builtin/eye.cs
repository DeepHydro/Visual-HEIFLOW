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
        /// Create double matrix having unity diagonal values  
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="columns">Number of columns</param>
        /// <returns>Unity matrix (diagonal matrix) of type double</returns>
        public static ILRetArray<double> eye(int rows, int columns) {
            using (ILScope.Enter()) {
                ILArray<double> ret = zeros(rows, columns);
                int diagLen = Math.Min(rows, columns);
                for (int i = 0; i < diagLen; i++)
                    ret.SetValue(1.0, i, i);
                return ret;
            }
        }
        /// <summary>
        /// Create unity matrix, arbitrary numeric type
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="columns">Number of columns</param>
        /// <returns>Unity matrix (diagonal matrix) of element type T</returns>
        /// <typeparam name="T">Element type</typeparam>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the type specified is not supported. Supported types are: double, float, complex, fcomplex, int, long, short, byte</exception>
        public static ILRetArray<T> eye<T>(int rows, int columns) {
            using (ILScope.Enter()) {
                int diagLen = Math.Min(rows, columns);
                ILArray<T> o = ones<T>(new ILSize(1,diagLen)); 
                if (isnull(o))
                    throw new ILArgumentException("eye does not support elements of type '" + typeof(T).Name + "'"); 
                ILArray<T> ret = zeros<T>(rows, columns);
                for (int i = 0; i < diagLen; i++)
                    ret.SetValue(o.GetValue(i), i, i);
                return ret;
            }
        }

    }
}
