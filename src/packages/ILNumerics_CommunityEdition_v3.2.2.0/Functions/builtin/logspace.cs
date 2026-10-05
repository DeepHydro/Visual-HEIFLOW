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
        /// Create logarithmically spaced row vector of 30 elements 
        /// </summary>
        /// <param name="start">First exponent value</param>
        /// <param name="end">Last exponent value</param>
        /// <returns>Row vector with 30 elements logathmically spaced between 10^start and 10^end</returns>
        public static ILRetArray<double> logspace (ILInArray<double> start, ILInArray<double> end) {
            return logspace(start, end,30); 
        }

        /// <summary>
        /// Create logarithmically spaced row vector
        /// </summary>
        /// <param name="start">First exponent value</param>
        /// <param name="end">Last exponent value</param>
        /// <param name="length">Number of elements to create</param>
        /// <returns>Row vector with 'length' elements logarithmically spaced between 10^start and 10^end</returns>
        public static ILRetArray<double> logspace (ILInArray<double> start, ILInArray<double> end, ILInArray<double> length) {
            using (ILScope.Enter(start,end, length)) {
                if (end == ILMath.pi)
                    end = Math.Log10(pi); 
                if (length < 2) 
                    return new ILRetArray<double>(new double[1]{Math.Pow(10,(double)end)},1,1); 
                ILArray<double> fact = ( end - start ) / (length - 1); 
                return pow(10.0, linspace<double>(start,end,length));  
            }
        }    
    }
}
