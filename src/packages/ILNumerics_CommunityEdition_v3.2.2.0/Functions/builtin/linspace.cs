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
using ILNumerics;
using ILNumerics.Exceptions;
using ILNumerics.Storage;
using ILNumerics.Misc;

namespace ILNumerics {
    public partial class ILMath {

        /// <summary>
        /// Create linearly spaced row vector of 100 elements 
        /// </summary>
        /// <param name="start">First value</param>
        /// <param name="end">Last value</param>
        /// <returns>Row vector with 100 elements linearly spaced between start and end</returns>
        public static ILRetArray<double> linspace (ILBaseArray start, ILBaseArray end) {
            return linspace<double>(start, end, 100); 
        }

        /// <summary>
        /// Create linearly spaced row vector, generic output type
        /// </summary>
        /// <param name="start">First value, scalar, numeric</param>
        /// <param name="end">Last value, scalar, numeric</param>
        /// <param name="length">Number of elements to create, scalar, numeric</param>
        /// <returns>Row vector with 'length' elements linearly spaced between start and end</returns>
        public static ILRetArray<T> linspace<T> (ILBaseArray start, ILBaseArray end, ILBaseArray length) {
            using (ILScope.Enter(start,end,length)) {
                if (object.Equals(start, null) || !start.IsScalar || !start.IsNumeric) throw new ILArgumentException("'start' must be numeric scalar");              
                if (object.Equals(end, null) || !end.IsScalar || !end.IsNumeric) throw new ILArgumentException("'end' must be numeric scalar");              
                if (object.Equals(length, null) || !length.IsScalar || !length.IsNumeric) throw new ILArgumentException("'length' must be numeric scalar");              
                ILArray<double> dStart = todouble(start); 
                ILArray<double> dEnd = todouble(end); 
                ILArray<double> dLength = todouble(length);
                       
                if (dLength < 2) 
                    return convert<double,T>(dEnd); 
                ILArray<double> fact = ( dEnd - dStart ) / (dLength - 1); 
                ILArray<double> i;
                //if (Settings.FavorRowForColumnVectorCreation) {
                i = counter(0.0, 1.0, 1, (int)dLength.GetValue(0));
                //} else {
                //    i = counter(0.0, 1.0, (int)dLength.GetValue(0), 1);
                //}
                return convert<double, T>(dStart + i * fact);
            }
        }    
        /// <summary>
        /// Create linearly spaced row vector, double precision
        /// </summary>
        /// <param name="start">First value</param>
        /// <param name="end">Last value</param>
        /// <param name="length">Number of elements to create</param>
        /// <returns>Row vector with 'length' elements linearly spaced between start and end</returns>
        public static ILRetArray<double> linspace (ILBaseArray start, ILBaseArray end, ILBaseArray length) {
            return linspace<double>(start,end,length); 
        }    
    }
}
