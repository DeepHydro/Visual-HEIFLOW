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
    // ToDo: DOKU two overloads undocumented
    public partial class ILMath {

        /// <summary>
        /// Create a cell row vector from given arrays 
        /// </summary>
        /// <param name="arrays">Arrays to be copied inside the cell</param>
        /// <returns>Cell vector</returns>
        /// <remarks><para>The new cell will be created as vector, having the array 'values' given as parameter as cell elements. Those elements will 
        /// be protected by changes from outside the cell.</para>
        /// <para>Visit the <a href="http://ilnumerics.net/$Cells.html" target="ILMain">online documentation</a> for cell.</para></remarks>
        /// <example>
        /// <para>A common use of the <c>cell</c> function is the concatenation of arrays and constants for subarray definitions, as shown in the following example:</para>
        /// <code>
        /// ILArray&lt;double> A = counter(4,3,2); 
        /// A
        /// //&lt;Double> [4,3]
        /// // 1          5          9 
        /// // 2          6         10 
        /// // 3          7         11 
        /// // 4          8         12 
        /// 
        /// // extract 1st, 2nd and last row:
        /// ILArray&lt;double> B = A[cell(0,1,end),full]; 
        /// B
        /// //&lt;Double> [3,3]
        /// // 1          5          9 
        /// // 2          6         10 
        /// // 4          8         12 
        /// </code>
        /// <para>Here, <c>cell</c> is used to concatenate individual indices determining the rows to select for the subarray. Using a cell here is convenient, because arbitrary types 
        /// can be stored in cells - integer, floating point types or special placeholders, like expressions (<see cref="ILNumerics.ILMath.end"/> and <see cref="ILNumerics.ILMath.full"/>).</para>
        /// </example>
        public static ILRetCell cell(params ILBaseArray[] arrays) {
            using (ILScope.Enter(arrays)) {
                if (arrays == null)
                    return cell(ILSize.Empty00);
                if (Settings.CreateRowVectorsByDefault) {
                    return cell(new ILSize(1, arrays.Length), arrays);
                } else {
                    return cell(new ILSize(arrays.Length, 1), arrays);
                }
            }
        }
        /// <summary>
        /// Create cell, initialize with arrays and size
        /// </summary>
        /// <param name="size">Size of the new cell</param>
        /// <param name="arrays">List of arrays for the cell elements, column major order</param>
        /// <returns>Cell of specified size, initialized with arrays</returns>
        /// <remarks>If number of arrays given is smaller than the number of elements given by <paramref name="size"/>, trailing 
        /// elements in the cell returned will be set to null.
        /// <para>The <see cref="M:ILNumerics_ILMath_size(params int[])"/> function is convenient for the specification of size descriptors.</para>
        /// <para>Visit the <a href="http://ilnumerics.net/$Cells.html" target="ILMain">online documentation</a> for cell.</para></remarks>
        /// <example>
        /// <code>
        /// ILArray&lt;double> A = rand(10,20,30); 
        /// ILCell C = cell(size(3,2),A, A+1, zeros(2,3)); 
        /// C
        /// //Cell [3,2]
        /// // &lt;Double> [10,20,30]          &lt;String>        4th element  
        /// // &lt;Double> [10,20,30]                                 (null)
        /// // &lt;Double> [2,3]                                      (null)
        /// </code>
        /// </example>
        public static ILRetCell cell(ILSize size, params ILBaseArray[] arrays) {
            using (ILScope.Enter(arrays)) {
                if (object.Equals(arrays, null) || arrays.Length == 0) {
                    return new ILRetCell(new ILCellStorage(size));
                }
                return new ILRetCell(new ILCellStorage(arrays, size));
            }
        }
        /// <summary>
        /// Create cell, initialize with arrays and size
        /// </summary>
        /// <param name="arrays">Predefined array with arrays, directly be used as new cell element storage</param>
        /// <param name="size">Size of newly created cell</param>
        /// <returns>Cell with size of <paramref name="size"/> and elements from <paramref name="arrays"/></returns>
        /// <remarks>The array given in <paramref name="arrays"/> is directly be used for the newly created cell. Make sure, not 
        /// to reference the system array afterwards. However, arrays  referenced from within the parameter <paramref name="arrays"/> 
        /// are stored as clone into the new cell. Therefore, those arrays are properly protected from changes by altering any array outside the cell.
        /// <para>Visit the <a href="http://ilnumerics.net/$Cells.html" target="ILMain">online documentation</a> for cell.</para></remarks>
        /// <seealso cref="ILNumerics.ILMath.cell(ILNumerics.ILBaseArray[])"/>
        public static ILRetCell cell(ILBaseArray[] arrays, params int[] size) {
            using (ILScope.Enter(arrays)) {
                return cell(new ILSize(size), arrays);
            }
        }
    }
}
