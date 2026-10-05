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

namespace ILNumerics.Data {

    internal class ILIntList {

        int[] m_data; 
        int m_count;

        public static ILIntList Create() { 
            return new ILIntList(); 
        }
        public static ILIntList Create(int initialCapacity) {
            return new ILIntList(initialCapacity);
        }
        public static ILIntList Create(int initialCapacity, int value) {
            return new ILIntList(initialCapacity, value);
        }
        internal ILIntList(ILDenseArray<int> array) {
            using (ILScope.Enter(array)) {
                m_count = array.S.NumberOfElements;
                m_data = ILMemoryPool.Pool.New<int>(m_count);
                System.Array.Copy(array.Storage.GetArrayForRead(), m_data, m_count);
        }
        }

        private ILIntList() {
            m_count = 0; 
            m_data = ILMemoryPool.Pool.New<int>(4); 
        }
        private ILIntList(int initialCapacity) {
            m_count = 0;
            m_data = ILMemoryPool.Pool.New<int>(initialCapacity); 
        }

        public ILIntList(int initialCapacity, int value) {
            m_count = initialCapacity; 
            m_data = ILMemoryPool.Pool.New<int>(initialCapacity);
            for (int i = 0; i < m_data.Length; i++) {
                if (i == initialCapacity) break; 
                m_data[i] = value; 
            }
        } 

        public int this[int index] {
            get { 
                //if (index >= m_count) 
                //    throw new IndexOutOfRangeException(); 
                return m_data[index]; 
            }
            set { m_data[index] = value; }
        }

        public int Count {
            get { return m_count; }
        }

        public IEnumerator<int> GetEnumerator() {
            for (int i = 0; i < m_count; i++) {
                yield return m_data[i]; 
            }
        }

        internal bool Contains( int val ) {
            for (int i = 0; i < m_count; i++) {
                if (m_data[i] == val) return true; 
            }
            return false; 
        }

        internal void Add( int val ) {
            if (m_count == m_data.Length) {
                doubleCapacity(); 
            }
            m_data[m_count++] = val; 
        }

        private void doubleCapacity() {
            // we always give our data array at least 4 elements
            int[] newArr = ILMemoryPool.Pool.New<int>(m_data.Length * 2); 
            System.Array.Copy(m_data,newArr,m_count);
            ILMemoryPool.Pool.Free(m_data); 
            m_data = newArr; 
        }
        /// <summary>
        /// caution! This may bring the ILIntList in inconsistent state! 
        /// </summary>
        /// <returns></returns>
        internal int[] GetArray() {
            return m_data; 
        }

        internal ILRetArray<int> ToTempArray() {
            ILSize newDims = new ILSize(m_count, 1); 
            ILRetArray<int> ret = new ILRetArray<int>(new Storage.ILDenseStorage<int>(m_data,newDims));
            m_data = null; 
            return ret; 
        }

        internal void Clear() {
            ILMemoryPool.Pool.Free(m_data); 
            m_data = new int[4]; 
            m_count = 0; 
        }

        internal void Dispose() {
            ILMemoryPool.Pool.Free(m_data); 
            m_count = -1; 
            m_data = null; 
        }
    }
}
