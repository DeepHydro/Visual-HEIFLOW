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

namespace ILNumerics {
    public partial class ILMath {
        /// <summary>
        /// Request System.Array <typeparamref name="T"/>[] from ILNumerics Memory Pool
        /// </summary>
        /// <typeparam name="T">the element type</typeparam>
        /// <param name="minLength"><b>minimal</b> length of the array returned</param>
        /// <returns>A system array of element type <typeparamref name="T"/> and minimal length <paramref name="minLength"/>.</returns>
        /// <remarks>
        /// <para>This function fetches a System.Array from the collection of available arrays which are currently held in the ILNumerics Memory Pool. 
        /// If no matching array is available, a new array will be allocated from the runtime.</para>
        /// <para>Important note: the array returned is not guaranteed to be cleared! Furthermore, it may contain more elements than requested by <paramref name="minLength"/>!</para>
        /// <para>The array returned is fetched from the collection of available arrays in the pool. If no matching array was found, a new array is requested from the managed heap.</para>
        /// <para>The user is reponsible to return the array to the pool once finished using it. The <see cref="ILNumerics.ILMath.free{T}"/> method should be used for returning an array.</para>
        /// <para>Keep in mind, the array returned may be <b>longer</b> as requested! Therefore, in order to reference its elements use the <paramref name="length"/> parameter rather than the System.Array.Length property.</para>
        /// <example>A system array is needed inside a computational loop. The loop calls a native function repeatedly which requests a working array. The System.Array is requested from the pool and returned to the pool after usage.
        /// <code><![CDATA[
        ///[System.Runtime.InteropServices.DllImport("myDLL")]
        ///private static extern void MyDllFunc(double[] a, int lenA, double[] b, double[] work, int lenWork);
        ///
        ///public static ILRetArray<double> MyFunc(ILInArray<double> A) {
        ///    using (ILScope.Enter(A)) {
        ///        // myDllFunc needs storage for the result
        ///        ILArray<double> ret = zeros(A.Size);
        ///        ILArray<double> tmpRow = zeros(1, A.S[1]);
        ///
        ///        // myDllFunc needs a working storage:
        ///        double[] work = New<double>(A.S[1]);
        ///        // for all rows of A use the same working array
        ///        for (int i = 0; i < A.S[0]; i++) {
        ///            using (ILScope.Enter()) {
        ///                ILArray<double> ARow = A[i, full];
        ///                MyDllFunc(ARow.GetArrayForRead(), A.Length, tmpRow.GetArrayForWrite(), work, A.Length);
        ///                ret[i, full] = tmpRow;
        ///            }
        ///        }
        ///        // return the System.Array to the pool 
        ///        free(work);
        ///        return ret;
        ///    }
        ///}
        /// ]]></code>
        /// However, in most situations, the handling of system.arrays directly can be circumvented. The same goal as in the 
        /// example could be archieved as follows: 
        /// <code><![CDATA[       
        /// public static ILRetArray<double> MyFunc(ILInArray<double> A) {
        ///using (ILScope.Enter(A)) {
        ///    // myDllFunc needs storage for the result
        ///    ILArray<double> ret = zeros(A.Size);
        ///    ILArray<double> tmpRow = zeros(1, A.S[1]);
        ///    // myDllFunc needs a working storage. We us a regular ILNumerics array 
        ///    // which we dont have to free afterwards. 
        ///    ILArray<double> work = zeros(tmpRow.S); 
        ///
        ///    // for all rows of A use the same working array
        ///    for (int i = 0; i < A.S[0]; i++) {
        ///        using (ILScope.Enter()) {
        ///            ILArray<double> ARow = A[i, full];
        ///            MyDllFunc(ARow.GetArrayForRead(), A.Length, tmpRow.GetArrayForWrite(), work.GetArrayForWrite(), A.Length);
        ///            ret[i, full] = tmpRow;
        ///        }
        ///    }
        ///    // returning the working storage is not needed here...
        ///
        ///    return ret;
        ///}
        ///}]]></code>
        /// Both examples archieve a 100% memory efficiency by completely reusing the memory needed within the function. 
        /// </example>
        /// </remarks>
        public static T[] New<T>(int minLength) {
            return ILMemoryPool.Pool.New<T>(minLength); 
        }
    }
}
