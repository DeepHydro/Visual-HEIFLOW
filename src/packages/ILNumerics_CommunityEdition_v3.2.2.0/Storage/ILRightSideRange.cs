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
using ILNumerics.Data;

namespace ILNumerics.Storage {
    internal class ILRightSideRange : ILRange {
        /// <summary>
        /// rempat consctructor - for performance reasons 
        /// </summary>
        /// <param name="dimension">source array dimensions</param>
        /// <param name="range">destination array repmattings </param>
        internal ILRightSideRange(ILSize dimension, params int[] range) {
            if (range == null || range.Length == 0) {
                m_range = new ILIntList[0]; 
                m_size = ILSize.Empty00; 
            } else {
                m_range = new ILIntList[Math.Max(range.Length, dimension.NumberOfDimensions)];
                int[] outSizes = dimension.ToIntArray(m_range.Length); 
                int i = 0;
                for (; i < range.Length; i++) {
                    int limit = range[i];
                    if (limit < 0) 
                        throw new Exceptions.ILArgumentException("dimension specifier must not be negative"); 
                    // 'full' dimension addressed
                    int fullVal = -(dimension[i] - 1);
                    ILIntList intList = ILIntList.Create(limit, fullVal);
                    //for (int c = limit; c --> 0; ) {
                    //    intList.Add(val);
                    //}
                    m_range[i] = intList; 
                    outSizes[i] *= limit; 
                }
                for (; i < m_range.Length; i++) {
                    if (i < dimension.NumberOfDimensions) {
                        int fullVal = -(dimension[i] - 1);
                        m_range[i] = ILIntList.Create(1, fullVal);
                    } else {
                        m_range[i] = ILIntList.Create(1, 0); 
                    }
                }
                m_size = new ILSize(outSizes); 
            }
        }
        internal ILRightSideRange(ILSize dimension, ILBaseArray[] indices) 
            : base()  { 
            create(indices,dimension); 
        }
        protected void create(ILBaseArray[] indices, ILSize dimensions) {
            using (ILScope.Enter(indices)) {
                //if (object.Equals(indices, null)) {
                //    // A(null)
                //    indices = new ILBaseArray[] { null }; 
                //}
                if (indices.Length == 0) {
                    m_range = new ILIntList[0];
                    m_size = ILSize.Empty00;
                    return;
                }
                if (indices.Length == 1) {
                    if (indices[0] is ILDenseArray<string> && indices[0].IsScalar) {
                        // special case? A[":;0:3;0:end;..."] -> multiple dimensions given as single string
                        System.Diagnostics.Debug.Fail("single dimension subarray specification should be handled seperately by ILDenseStorage.Subarray()");
                        string indStr = (string)(indices[0] as ILDenseArray<string>).GetValue(0);
                        if (String.IsNullOrWhiteSpace(indStr)) {
                            m_range = new ILIntList[0];
                            m_size = ILSize.Empty00;
                            return;
                        }
                        string[] dimParts = indStr.Split(';');
                        if (dimParts.Length > 1) {
                            indices = new ILBaseArray[dimParts.Length];
                            for (int i = 0; i < dimParts.Length; i++) {
                                indices[i] = dimParts[i];
                            }
                            create(indices,dimensions); 
                            return; 
                        }
                    }
                }
                m_range = new ILIntList[indices.Length];  // due to outer constraines: >= 2 dimensions
                // outdims: immer so wie der range (indices), min 2
                int[] outDims = new int[m_range.Length];
                // if indices addresses less dimensions than ex, in dimensions -> expand last dimension 
                int[] inDims = dimensions.ToIntArrayEx(m_range.Length);

                for (int d = 0; d < m_range.Length; d++) {
                    int min = int.MaxValue, max = int.MinValue, outLen = 0;
                    m_range[d] = ILIntList.Create();
                    extractDimension(m_range[d], ref outLen, indices[d], d, ref min, ref max, inDims);
                    outDims[d] = outLen;
                    // check limits
                    if (min < 0) {
                        throw new Exceptions.ILArgumentException(String.Format("index < 0: {1}, dimension  #{0}", d, min));
                    }
                    if (max >= ((d < inDims.Length) ? inDims[d] : 1)) {
                        throw new Exceptions.ILArgumentException(String.Format("index out of range: {1}, dimension: {0}", d, max));
                    }
                }
                m_size = new ILSize(outDims);
            }
        }

    }
}
