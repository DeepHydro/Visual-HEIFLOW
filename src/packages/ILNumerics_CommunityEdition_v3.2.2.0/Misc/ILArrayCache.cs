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

namespace ILNumerics.Misc {
    // ToDo: DOKU Missing for complete class
    public class ILArrayCache : IDisposable {

        int m_hashKey; 
        ILCell m_cache;
        bool m_hasValue = false;

        internal ILArrayCache() {
            m_hasValue = false;  
            m_cache = new ILCell(1,1);
        }

        public bool IsCached(params ILBaseArray[] keys) {
            return hash(keys) == m_hashKey;
        }

        private int hash(ILBaseArray[] keys) {
            if (keys == null) return 0; 
            using (ILScope.Enter(keys)) {
                int ret = keys.Length; 
                foreach (ILBaseArray a in keys) {
                    ret = unchecked(ret * 17) + a.GetHashCode(); 
                }
                return ret; 
            }
        }

        public void Cache<T>(ILInArray<T> array, params ILBaseArray[] keys) {
            using (ILScope.Enter(array)) {
                m_hashKey = hash(keys); 
                m_cache[0] = array;
            }
        }
        public bool TryGetArray<T>(ILOutArray<T> cache, params ILBaseArray[] keys) {
            if (object.Equals(cache,null)) 
                return false; 
            int curKey = hash(keys);
            if (curKey == m_hashKey) {
                cache.a = m_cache.GetArray<T>(0);
                return true;
            } 
            return false;            
        }

        #region IDisposable Members

        public void Dispose() {
            if (! object.Equals( m_cache, null) )
                m_cache.Dispose(); 
            m_hasValue = false; 
            m_hashKey = -1; 
        }

        #endregion
    }
}
