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
using System.Threading; 
using System.Linq;
using System.Text;

namespace ILNumerics.Storage {
    [Serializable]
    internal sealed class ILCountableArray<ElementType> {
    
        private ElementType[] m_data; 
        private int m_length; 
        private int m_referenceCount;

        #region properties
        /// <summary>
        /// Access to the internal system array
        /// </summary>
        internal ElementType[] Data  { get { return m_data; } } 
        /// <summary>
        /// minimal length of the system array to be used for computations
        /// </summary>
        internal int Length  { get { return m_length; } } 
	    #endregion

        /// <summary>
        /// create new countable array by size
        /// </summary>
        /// <param name="length"></param>
        /// <remarks>The memory for the newly created array is requested from memory pool. All elements of the array 
        /// are initialized with default(ElementType).</remarks>
        internal ILCountableArray(int length) {
            bool dummy; 
            m_data = ILMemoryPool.Pool.New<ElementType>(length, false, out dummy);
            m_length = length; 
            // reference counting is matter of the hosting object!
            m_referenceCount = 0;
        }
        /// <summary>
        /// create new countable array by size
        /// </summary>
        /// <param name="length"></param>
        /// <remarks>The memory for the newly created array is requested from memory pool. Depending on the value of 
        /// 'clear', the elements of the array are NOT initialized and therefore may contain garbage data!.</remarks>
        internal ILCountableArray(int length, bool clear) {
            bool dummy;
            m_data = ILMemoryPool.Pool.New<ElementType>(length, clear, out dummy);
            m_length = length; 
            // reference counting is matter of the hosting object!
            m_referenceCount = 0;
        }

        /// <summary>
        /// create new countable array, provide system array
        /// </summary>
        /// <param name="data">system array to be used as storage array directly</param>
        /// <param name="length">minimum lenght of array needed</param>
        internal ILCountableArray(ElementType[] data, int length) {
#if VERBOSE
            //DebuggerTraceHelper.Output.Append(
            //    String.Format("CountableArray (T[]{0}) " + GetHashCode(),data.GetHashCode())
            //); 
#endif
            m_data = data; 
            m_length = length; 
            // reference counting is matter of the hosting object!
            m_referenceCount = 0; 
        }
        /// <summary>
        /// increase reference counter 
        /// </summary>
        internal void IncreaseReference() {
            Interlocked.Increment(ref m_referenceCount); 
        }
        /// <summary>
        /// decrease reference counter
        /// </summary>
        internal void DecreaseReference() {
            Interlocked.Decrement(ref m_referenceCount); 
            if (m_referenceCount == 0) {
                Dispose();
            }
        }
        /// <summary>
        /// Dispose off this array: register it in pool
        /// </summary>
        internal void Dispose() {
#if VERBOSE
            //System.Diagnostics.Debug.Assert(m_referenceCount == 0); 
            //DebuggerTraceHelper.Output.Append(String.Format("\r\nCountableArray HashCode: " + GetHashCode())); 
#endif
            if (m_data != null) {
                ILMemoryPool.Pool.Free(m_data);
                m_data = null;
                m_length = 0;
            }
        }
        /// <summary>
        /// number of storages referencing to this array
        /// </summary>
        /// <remarks>In order to in-/decrease the reference counter, use 
        /// the <see cref="ILNumerics.Storage.ILCountableArray&lt;ElementType>.IncreaseReference()"/>
        /// and <see cref="ILNumerics.Storage.ILCountableArray&lt;ElementType>.DecreaseReference()"/> functions.</remarks>
        internal int ReferenceCount { 
            get { return m_referenceCount; }
        
        }
        /// <summary>
        /// return (solid) copy of this countable array
        /// </summary>
        /// <returns>newly created array</returns>
        internal ILCountableArray<ElementType> CreateCopy() {
            ILCountableArray<ElementType> ret = new ILCountableArray<ElementType>(Length, false);
            System.Array.Copy(m_data,ret.Data,Length); 
            return ret; 
        }
    }
}
