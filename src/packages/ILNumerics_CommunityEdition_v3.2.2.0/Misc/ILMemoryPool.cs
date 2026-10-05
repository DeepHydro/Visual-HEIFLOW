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
using System.Collections; 
using System.Collections.Generic;
using System.Text;
using ILNumerics.Exceptions; 
using ILNumerics.Misc; 

namespace ILNumerics {
    /// <summary>
    /// The ILNumerics Memory Pool is the heart of the ILNumerics memory management.
    /// </summary>
    /// <remarks>The pool reduces the pressure on the systems memory caused by larger objects.
    /// <para>Arrays created in ILNumerics will try to allocate their memory from the pool. If the attempt fails, the memory is gathered from the regular managed heap instead.</para>
    /// <para>Disposed array objects deterministically register their underlying storage in the pool for later reusing. The process is triggered by a deterministic disposal 
    /// pattern in conjunction with <a href="http://ilnumerics.net/$Arrays.html">ILNumerics array types</a> and <a href="http://ilnumerics.net/$FunctionRules.html">ILNumerics Function Rules</a>.</para></remarks>
    public sealed class ILMemoryPool {
        /// <summary>
        /// The only global ILMemoryPool instance 
        /// </summary>
        public static readonly ILMemoryPool Pool = new ILMemoryPool();
        internal static readonly Dictionary<Type,IILMemoryPool> Pools = new Dictionary<Type,IILMemoryPool>(); 
        /// <summary>
        /// Gets the minimum length required for arrays of a certain element type to be stored in the pool
        /// </summary>
        /// <typeparam name="T">Element type of the pool</typeparam>
        /// <returns>minumun element length</returns>
        public long MinArrayLength<T>()
        {
            return ILMemoryPoolInternal<T>.Pool.MinArrayLength;
        }
        //public void SetMinArrayLength<T>(long value) {
        //    ILMemoryPoolInternal<T>.Pool.MinArrayLength = value;
        //}
        /// <summary>
        /// Get new array of type T from memory pool.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="length">Size of T[]</param>
        /// <param name="clear">If true, set the elements in T[] to default(T)</param>
        /// <param name="cleared">Always true if <paramref name="clear"/> was set. Otherwise this will be true if the array was newly created instead of being recycled from the pool. False otherwise.</param>
        /// <remarks><para>If the pool contains an unused matching element of sufficient size this element will be returned. If <paramref name="clear"/> was false,
        /// the result might still contain the old data.</para>
        /// <para>If the pool does not contain a matching element a new one is created.</para>
        /// <para><b>Note:</b> The returned array may be larger than requested if it was recycled from the pool.</para></remarks>
        /// <returns>An array of type T of at least length <paramref name="length"/></returns>
        public T[] New<T>(long length, bool clear, out bool cleared) {
            return ILMemoryPoolInternal<T>.Pool.New(length, clear, out cleared);
        }

        /// <summary>
        /// Get an array from the memory pool without clearing the elements
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="length">Required minimum length</param>
        /// <remarks>The returned array is only guaranteed to have at least length <paramref name="length"/>. Its length may exceed <paramref name="length"/>.
        /// There is no guarantee on the values contained in the returned array! To force all elements to their default value 
        /// use <see cref="ILNumerics.ILMemoryPool.New&lt;T&gt;(long length, bool clear, out bool cleared)"/></remarks>
        /// <returns>An array of type T of at least length <paramref name="length"/>.</returns>
        public T[] New<T>(long length)
        {
            return ILMemoryPoolInternal<T>.Pool.New(length);
        }

        /// <summary>
        /// Return a array of type T that is not needed anymore to the pool.
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="array">The array to add to the pool</param>
        /// <remarks>You may also "free" objects not retrieved from the pool, in which case they are registered in the pool.</remarks>
        public void Free<T>(T[] array) {
            ILMemoryPoolInternal<T>.Pool.Free(array); 
        }

       
        /// <summary>
        /// Get maximal potential size of the memory pool for objects of type T
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <returns>Maximal number of bytes in pool</returns>
        public long MaxBytes<T>() {
            return ILMemoryPoolInternal<T>.Pool.MaxBytes; 
        }

        /// <summary>
        /// Reset &amp; reconfigure the pool 
        /// </summary>
        /// <param name="MinPoolArrayLen">Minimum length for array object to be stored inside the pool</param>
        /// <param name="maxSizeMB">Overall size the memory pool consumes at most</param>
        /// <remarks>Reset will dispose all objects currently hold in the pool!</remarks>
        public void Reset<T>(long MinPoolArrayLen, int maxSizeMB) {
            ILMemoryPoolInternal<T>.Pool.Reset(MinPoolArrayLen, maxSizeMB); 
        }

        /// <summary>
        /// Give information about pool state
        /// </summary>
        /// <param name="shortVersion">true (default): abbreviate infos to: current MB in Pool, reclaimed MB for livetime, reclaimed objects for livetime. False: give full info</param>
        /// <returns>Infos about current pool state</returns>
        public string Info(bool shortVersion = true) {
            if (Pools.ContainsKey(typeof(double))) {
                return Pools[typeof(double)].Info(shortVersion);
            } else if (Pools.Count > 0) {
                foreach (IILMemoryPool pool in Pools.Values) {
                    // todo: what to return? (work around: just give the first one)
                    return pool.Info(shortVersion); 
                }
            }
            return "no pool found"; 
        }

    }
}
