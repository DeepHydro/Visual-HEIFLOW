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
    internal class ILLeftSideRange : ILRange {       
        /// <summary>
        /// true for left side ranges, if at least one dimension must be expanded
        /// </summary>
        internal bool Expanding {
            get {
                return m_expanding;
            }
            set {
                m_expanding = value; 
            }
        }
        /// <summary>
        /// sizes of dimensions to be expanded
        /// </summary>
        private int[] m_expandDimensions; 
       /// <summary>
        /// internal field, stores expanding flag
        /// </summary>
        private bool m_expanding; 
        /// <summary>
        /// array with sizes of dimensions to be expanded
        /// </summary>
        internal int[] ExpandDimensions {
            get {
                return m_expandDimensions; 
            }
        }

        #region constructor
        // indices may not address a single dimension! This case is handled by sequential addressing from callee (ILDenseArray<T>)
        public ILLeftSideRange(ILSize dimension, ILBaseArray[] indices) : base()  {
            create(indices,dimension);
        }
	    #endregion

        protected void create(ILBaseArray[] indices, ILSize dimensions) {
            using (ILScope.Enter(indices)) {
                if (indices.Length == 0) {
                    m_range = new ILIntList[0];
                    m_size = ILSize.Empty00;
                    return;
                }
                if (indices.Length == 1) {
                    if (indices[0] is ILDenseArray<string> && indices[0].IsScalar) {
                        // special case? A[":;0:3;0:end;..."] -> multiple dimensions given as single string
                        //System.Diagnostics.Debug.Fail("single dimension subarray specification should be handled seperately by ILDenseStorage.Subarray()"); 
                        // (some rare cases (remove e.g.) use this path to parse given dimensions... ) 
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
                            // caution: using ILBaseArray as input is EVIL!! 
                            // now we have an array of temparrays! Those are too fragile
                            // to work with - so start again and safe them in a scope
                            create(indices,dimensions); 
                            return; 
                        }
                    }
                }
                m_range = new ILIntList[indices.Length];
                // outdims: immer so wie der range (indices), min 2
                int[] outDims = new int[Math.Max(2, m_range.Length)];
                outDims[1] = 1; // in case it is not set later 
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
                        #region special cases A(ind) = B
                        if (dimensions.NonSingletonDimensions > 1 &&
                            (d == m_range.Length - 1)
                            && (m_range.Length < dimensions.NumberOfDimensions)) {
                            throw new Exceptions.ILArgumentException(String.Format("Cannot expand array along ambiguous dimension #{0}", d));
                        }
                        m_expanding = true;
                        if (m_expandDimensions == null) {
                            m_expandDimensions = new int[Math.Max(indices.Length, 2)];
                        }
                        if (indices.Length == 1 /* -> d == 0! */
                            && dimensions.NonSingletonDimensions == 1 && dimensions[1] > 1) {
                            // special case: row vector, adressing by single dimension A(end+1) = ...
                            m_expandDimensions[1] = max + 1;
                            outDims[1] = outLen;
                            outDims[0] = 1;
                            break;
                        } else {
                            // common case 
                            m_expandDimensions[d] = max + 1;
                        }
                        #endregion
                    }
                }
                m_size = new ILSize(outDims);
            }
        }
    }
}
