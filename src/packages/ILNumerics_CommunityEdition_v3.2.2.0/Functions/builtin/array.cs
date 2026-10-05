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
        /// Create new array, fill elements with constant value
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="value">Constant value for all elements</param>
        /// <param name="size">Size of new array</param>
        /// <returns>New array according to size with all elements set to 'value'</returns>
        public static ILRetArray<T> array<T>(T value, ILSize size) {
            T[] newData = ILMemoryPool.Pool.New<T>(size.NumberOfElements);
            int i = 0, itemLength, itemCount, workerCount = 1;
            if (Settings.s_maxNumberThreads > 2 &&
                size.NumberOfElements >= ILNumerics.Settings.s_minParallelElement1Count / 2) {
                if (size.NumberOfElements >= ILNumerics.Settings.s_minParallelElement1Count / Settings.s_maxNumberThreads) {
                    itemCount = Settings.s_maxNumberThreads;
                    itemLength = size.NumberOfElements / Settings.s_maxNumberThreads;
                } else {
                    itemCount = 2;
                    itemLength = size.NumberOfElements / 2;
                }
            } else {
                itemCount = 1;
                itemLength = size.NumberOfElements;
            }
            Action<object> worker = (data) => {
                Tuple<int, int> range = (Tuple<int, int>)data;
                int start = range.Item1, endEx = range.Item2;
                for (int c = start; c < endEx; c++) {
                    newData[c] = value;
                }
                System.Threading.Interlocked.Decrement(ref workerCount);
            };
            for (i = 0; i < workerCount - 1; i++) {
                System.Threading.Interlocked.Increment(ref workerCount);
                Tuple<int, int> range = Tuple.Create(i * itemLength, (i + 1) * itemLength);
                ILThreadPool.QueueUserWorkItem(i, worker, range);
            }
            worker(Tuple.Create(i * itemLength, size.NumberOfElements));
            System.Threading.SpinWait.SpinUntil(() => { return workerCount <= 0; });
            ILRetArray<T> ret = new ILRetArray<T>(newData, size);
            return ret;
        }
        /// <summary>
        /// Create new array, fill element with constant value
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="value">Constant value for all elements</param>
        /// <param name="size">Size of new array</param>
        /// <returns>New array according to size with all elements set to 'value'</returns>
        public static ILRetArray<T> array<T>(T value, params int[] size) {
            return array<T>(value, new ILSize(size));
        }
        /// <summary>
        /// Create array, given elements and size
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="elements">System.Array of predefined elements</param>
        /// <param name="size">Size of every dimension for the new array, must correspond to the number of elements in <paramref name="elements"/>.</param>
        /// <returns>Newly created array</returns>
        /// <remarks><para>The System.Array given as <paramref name="elements"/>is taken 
        /// as storage for the new array without copy. Make sure not to reference that 
        /// System.Array directly afterwards.</para>
        /// <para>In order to prevent for memory leaks on long runnning algorithms, <c>System.Array</c>s should 
        /// not get created via the 'new' keyword - but fetched from the ILMemoryPool. </para></remarks>
        public static ILRetArray<T> array<T>(T[] elements, params int[] size) {
            ILSize newDimensions = new ILSize(size);
            ILRetArray<T> ret = new ILRetArray<T>(elements, newDimensions);
            return ret;
        }
        /// <summary>
        /// Create array, given elements and size
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="elements">System.Array of predefined elements</param>
        /// <param name="size">Size of the new array, must correspond to the number of elements in <paramref name="elements"/>.</param>
        /// <returns>Newly created array</returns>
        /// <remarks><para>The System.Array given as <paramref name="elements"/>is taken 
        /// as storage for the new array without copy. Make sure not to reference that 
        /// System.Array directly afterwards.</para>
        /// <para>In order to prevent for memory leaks on long runnning algorithms, <c>System.Array</c>s should 
        /// not get created via the 'new' keyword - but fetched from the ILMemoryPool. 
        /// </para></remarks>
        public static ILRetArray<T> array<T>(T[] elements, ILSize size) {
            ILRetArray<T> ret = new ILRetArray<T>(elements, size);
            return ret;
        }
        /// <summary>
        /// Create array, given elements and size
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="elements">Variable argument list with elements</param>
        /// <param name="size">Size of the new array, must correspond to the number of elements in <paramref name="elements"/>.</param>
        /// <returns>Newly created array</returns>
        /// <remarks><para>The elements given as <paramref name="elements"/> are used 
        /// for the new array without copy. For <typeparamref name="T"/> being a reference type, make sure not to reference any  
        /// elements directly afterwards.</para>
        /// </remarks>
        public static ILRetArray<T> array<T>(ILSize size, params T[] elements) {
            if (elements == null || (size.NumberOfElements != 0 && elements.Length == 0)) {
                return new ILRetArray<T>(size);
            } else {
                return new ILRetArray<T>(elements, size);
        }
        }
        /// <summary>
        /// Create column vector from given elements 
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="elements">List of elements</param>
        /// <returns>Newly created vector with elements given</returns>
        /// <remarks><para>If an System.Array was given as params argument, the array is directly taken 
        /// as storage for the new array without copy. Make sure not to reference the 
        /// System.Array directly afterwards!</para>
        /// <para>In order to prevent for memory leaks on long runnning algorithms, <c>System.Array</c>s should 
        /// not get created via the 'new' keyword - but fetched from the ILMemoryPool.</para>
        /// <para>The shape of the vector created is controlled by the setting switch <see cref="ILNumerics.Settings.CreateRowVectorsByDefault"/>.
        /// This switch defaults to <c>false</c> which will cause the creation of a column vector. </para></remarks>
        /// <see cref="ILNumerics.ILMath.row{T}(T[])"/>
        /// <see cref="ILNumerics.ILMath.column{T}(T[])"/>
        public static ILRetArray<T> array<T>(params T[] elements) {
            ILSize newDimensions;
            if (Settings.CreateRowVectorsByDefault) {
                newDimensions = new ILSize(1, elements.Length);
            } else {
                newDimensions = new ILSize(elements.Length, 1);
            }
            ILRetArray<T> ret = new ILRetArray<T>(elements, newDimensions);
            return ret;
        }

        /// <summary>
        /// Create row vector
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="elements">Elements of the row vector</param>
        /// <returns>New row vector</returns>
        public static ILRetArray<T> row<T>(params T[] elements) {
            if (elements == null || elements.Length == 0) {
                return empty<T>(ILSize.Empty00);
            }
            return new ILRetArray<T>(elements, new ILSize(1, elements.Length));
        }
        /// <summary>
        /// Create column vector
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="elements">Elements of the column vector</param>
        /// <returns>New column vector</returns>
        public static ILRetArray<T> column<T>(params T[] elements) {
            if (elements == null || elements.Length == 0) {
                return empty<T>(ILSize.Empty00);
            }
            return new ILRetArray<T>(elements, new ILSize(elements.Length, 1));
        }

    }
}
