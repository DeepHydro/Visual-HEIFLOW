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
using System.Linq;
using System.Text;
using ILNumerics.Misc; 
using ILNumerics.Storage; 

namespace ILNumerics {
    public partial class ILMath {

        /// <summary>
        /// Region creator for subarray specifications
        /// </summary>
        /// <param name="start">Begin of region, index of first element</param>
        /// <param name="end">End of region, last element</param>
        /// <returns>An array which specifies all indices of the region</returns>
        /// <remarks>The r function provides a shorter way to specify regions in subarray
        /// expressions. Other than alternatives like <see cref="ILNumerics.ILMath.vec"/>, 
        /// it also enables the use of the <see cref="ILNumerics.ILMath.end"/> keyword - even in 
        /// conjunction with simple mathematical expressions.</remarks>
        /// <example><code>ILArray&lt;double&gt; A = counter(5,4,3), B;
        /// 
        /// // the following subarrays will all create the same B: 
        /// B = A[full,r(1,end-1),full]; 
        /// B = A[r(0,end),"1,2",":"];
        /// B = A[":;1:2;0:end"];
        /// 
        /// // however, these expression will <b>not</b> work:
        /// B = A[":;0:end-1,:"]; // 'end' expression evaluation is not supported in strings. 
        /// </code></example>
        public static ILBaseArray r(ILBaseArray start, ILBaseArray end) {
            return r(start,1,end); 
        }
        /// <summary>
        /// Stepped region creator for subarray specifications
        /// </summary>
        /// <param name="start">Begin of region, index of first element</param>
        /// <param name="end">End of region, last element</param>
        /// <param name="step">Increment, distance between created indices</param>
        /// <returns>An array which specifies the indices of the region</returns>
        /// <remarks>The r function provides a shorter way to specify regions in subarray
        /// expressions. Other than alternatives like <see cref="ILNumerics.ILMath.vec"/>, 
        /// it also enables the use of the <see cref="ILNumerics.ILMath.end"/> keyword - even in 
        /// conjunction with simple mathematical expressions.</remarks>
        /// <example><code>ILArray&lt;double&gt; A = counter(5,4,3), B;
        /// 
        /// // the following subarrays will all create the same B: 
        /// B = A[full,r(1,end-1),full]; 
        /// B = A[r(0,end),"1,2",":"];
        /// B = A[":;1:2;0:end"];
        /// 
        /// // however, these expression will <b>not</b> work:
        /// B = A[":;0:end-1,:"]; // 'end' expression evaluation is not supported in strings. 
        /// </code></example>
        public static ILBaseArray r(ILBaseArray start, ILBaseArray step, ILBaseArray end) {
            ILRegularRange retRange = new ILRegularRange (start, step, end); 
            return new ILRetArray<ILRegularRange>(new ILDenseStorage<ILRegularRange>(
                                new ILRegularRange[]{ retRange},ILSize.Scalar1_1)); 
        }

        private static ILArray<ILFullRange> s_fullRange = null; 
        /// <summary>
        /// Address the whole dimension for subarray access
        /// </summary>
        public static ILBaseArray full {
            get { 
                if (object.Equals(s_fullRange, null)) 
                    s_fullRange = localMember<ILFullRange>(); 
                return s_fullRange; 
            }
        }
    }
}
