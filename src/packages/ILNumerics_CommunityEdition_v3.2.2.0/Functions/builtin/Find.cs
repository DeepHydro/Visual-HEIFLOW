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
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Exceptions;



namespace ILNumerics {
    public partial class ILMath {
       

        /// <summary>
        /// Find nonzero elements in A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="limit">[Optional] Number of elements to search for. If this value is <![CDATA[< 0]]> the function 
        /// will return at most 'limit' nonzero elements from the end of the array ordered by ascending index.
        /// Set to 0 to search full array (default).</param>
        /// <param name="C">[Optional] If not null, the function will return the row indices of nonzero elements 
        /// as main return value. C will therefore hold the column indices of those elements. If A
        /// has more than 2 dimensions, the column indices will go along the 2nd dimension.</param>
        /// <param name="V">[Optional] If not null on entrance, V will hold a copy of the values of nonzero elements returned.</param>
        /// <returns>Vector containing (sequential) indices of nonzero elements in A. If C was 
        /// not null, return value will contain row indices of nonzero elements. </returns>
        /// <remarks>The return type of the index vectors is always 'double'. The return type 
        /// of the element vector 'V' depends on the type of input array A. V and C may be null on 
        /// entrance, indicating their information is not needed. If V is not null (e.g. 'empty()') C must be 
        /// not null also. Any initial data of V or C will be lost.</remarks>
        public static ILRetArray<int> find(ILInArray< double> A, int limit = 0,
                      ILOutArray<int> C = null, ILOutArray< double> V = null) {
            using (ILScope.Enter(A)) {
                bool create_row_columns = !Object.Equals(C, null);
                bool return_values = !Object.Equals(V, null);
                ILArray<int> ret = 1;
                ILSize inDim = A.Size;
                if (inDim.NumberOfElements == 1) {
                    #region SCALAR
                    // scalar -> return copy
                    
                    if (A.GetValue(0, 0) != 0.0) {
                        if (create_row_columns) {
                            C.a = zeros<int>(ILSize.Scalar1_1);
                        }
                        if (return_values) {
                            V.a = A.C;
                        }
                        return zeros<int>(1, 1);
                    } else {
                        if (create_row_columns) {
                            C.a = empty<int>(ILSize.Empty00);
                        }
                        if (return_values) {
                            V.a = empty<double>(ILSize.Empty00);
                        }
                        return empty<int>(ILSize.Empty00);
                    }
                    #endregion SCALAR
                }
                long nrElements = inDim.NumberOfElements;
                

                if (limit != 0) {
                    int lim = Math.Abs(limit);
                    if (lim < nrElements)
                        nrElements = lim;
                }
                int[] indices = ILMemoryPool.Pool.New<int>(nrElements); // init return array with most elements for non logical inarray -> shorten afterwards 
                int foundIdx = 0;
                // physical -> pointer arithmetic
                if (limit >= 0) {
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( double* pX = A.GetArrayForRead()) {
                            
                            double* lastElement = pX + inDim.NumberOfElements;
                            
                            double* tmpIn = pX;
                            int* pI = pIndices;
                            int* pFoundLast = pI + indices.Length;
                            while (tmpIn < lastElement && pI < pFoundLast) {
                                
                                if (*tmpIn != 0.0)
                                    *pI++ = (int)(tmpIn - pX);
                                tmpIn++;
                            }
                            foundIdx = (int)(pI - pIndices);
                        }
                    }
                } else {
                    // search backwards 
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( double* pX = A.GetArrayForRead()) {
                            
                            double* lastElementX = pX;
                            
                            double* tmpIn = pX + inDim.NumberOfElements;
                            int* pI = pIndices + indices.Length;
                            while (tmpIn > lastElementX && pI > pIndices) {
                                tmpIn--;
                                
                                if (*tmpIn != 0.0)
                                    *(--pI) = (int)(tmpIn - pX);
                            }
                            foundIdx = (int)(pIndices + indices.Length - pI);
                        }
                    }
                }
                if (foundIdx == 0) {
                    return empty<int>();
                }
                // transform to row / columns; extract values if needed
                int leadDimLen = inDim[0];
                if (create_row_columns) {
                    #region RETURN ROWS / COLUMNS /VALUES
                    C.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    ret.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    if (return_values) {
                        V.a = array< double>(ILMemoryPool.Pool.New<  double>(foundIdx), foundIdx, 1);
                        // copy values, transform to row/columns
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( double* pValues = V.GetArrayForWrite(), pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                                
                                double* pV = pValues;
                                
                                double* pX = pInput;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)*(pI) / leadDimLen;
                                    *pV++ = *(pInput + (int)*pI++);
                                }
                            }
                        }
                    } else {
                        // just return row / columns 
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( double* pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)(*(pI++) / leadDimLen);
                                }
                            }
                        }
                    }
                    #endregion RETURN ROWS / COLUMNS
                } else {
                    #region RETURN INDICES ONLY
                    if (foundIdx != indices.Length) {
                        ret.a = new ILArray<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                        unsafe {
                            fixed (int* pIndices = indices, pRows = ret.GetArrayForWrite()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                while (pI < pLastIndex) {
                                    *pR++ = *pI++;
                                }
                            }
                        }
                    } else {
                        ret.a = array<int>(indices, foundIdx, 1);
                    }
                    #endregion RETURN INDICES ONLY
                }
                return ret;
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Find nonzero elements in A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="limit">[Optional] Number of elements to search for. If this value is <![CDATA[< 0]]> the function 
        /// will return at most 'limit' nonzero elements from the end of the array ordered by ascending index.
        /// Set to 0 to search full array (default).</param>
        /// <param name="C">[Optional] If not null, the function will return the row indices of nonzero elements 
        /// as main return value. C will therefore hold the column indices of those elements. If A
        /// has more than 2 dimensions, the column indices will go along the 2nd dimension.</param>
        /// <param name="V">[Optional] If not null on entrance, V will hold a copy of the values of nonzero elements returned.</param>
        /// <returns>Vector containing (sequential) indices of nonzero elements in A. If C was 
        /// not null, return value will contain row indices of nonzero elements. </returns>
        /// <remarks>The return type of the index vectors is always 'double'. The return type 
        /// of the element vector 'V' depends on the type of input array A. V and C may be null on 
        /// entrance, indicating their information is not needed. If V is not null (e.g. 'empty()') C must be 
        /// not null also. Any initial data of V or C will be lost.</remarks>
        public static ILRetArray<int> find(ILInArray< Int64> A, int limit = 0,
                      ILOutArray<int> C = null, ILOutArray< Int64> V = null) {
            using (ILScope.Enter(A)) {
                bool create_row_columns = !Object.Equals(C, null);
                bool return_values = !Object.Equals(V, null);
                ILArray<int> ret = 1;
                ILSize inDim = A.Size;
                if (inDim.NumberOfElements == 1) {
                    #region SCALAR
                    // scalar -> return copy
                    if (A.GetValue(0,0) != 0) {
                        if (create_row_columns) {
                            C.a = zeros<int>(ILSize.Scalar1_1);
                        }
                        if (return_values) {
                            V.a = A.C;
                        }
                        return zeros<int>(1, 1);
                    } else {
                        if (create_row_columns) {
                            C.a = empty<int>(ILSize.Empty00);
                        }
                        if (return_values) {
                            V.a = empty<Int64>(ILSize.Empty00);
                        }
                        return empty<int>(ILSize.Empty00);
                    }
                    #endregion SCALAR
                }
                long nrElements = inDim.NumberOfElements;
                
                if (limit != 0) {
                    int lim = Math.Abs(limit);
                    if (lim < nrElements)
                        nrElements = lim;
                }
                int[] indices = ILMemoryPool.Pool.New<int>(nrElements); // init return array with most elements for non logical inarray -> shorten afterwards 
                int foundIdx = 0;
                // physical -> pointer arithmetic
                if (limit >= 0) {
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( Int64* pX = A.GetArrayForRead()) {
                           
                            Int64* lastElement = pX + inDim.NumberOfElements;
                           
                            Int64* tmpIn = pX;
                            int* pI = pIndices;
                            int* pFoundLast = pI + indices.Length;
                            while (tmpIn < lastElement && pI < pFoundLast) {
                                if (*tmpIn != 0)
                                    *pI++ = (int)(tmpIn - pX);
                                tmpIn++;
                            }
                            foundIdx = (int)(pI - pIndices);
                        }
                    }
                } else {
                    // search backwards 
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( Int64* pX = A.GetArrayForRead()) {
                           
                            Int64* lastElementX = pX;
                           
                            Int64* tmpIn = pX + inDim.NumberOfElements;
                            int* pI = pIndices + indices.Length;
                            while (tmpIn > lastElementX && pI > pIndices) {
                                tmpIn--;
                                if (*tmpIn != 0)
                                    *(--pI) = (int)(tmpIn - pX);
                            }
                            foundIdx = (int)(pIndices + indices.Length - pI);
                        }
                    }
                }
                if (foundIdx == 0) {
                    return empty<int>();
                }
                // transform to row / columns; extract values if needed
                int leadDimLen = inDim[0];
                if (create_row_columns) {
                    #region RETURN ROWS / COLUMNS /VALUES
                    C.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    ret.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    if (return_values) {
                        V.a = array< Int64>(ILMemoryPool.Pool.New<  Int64>(foundIdx), foundIdx, 1);
                        // copy values, transform to row/columns
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( Int64* pValues = V.GetArrayForWrite(), pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                               
                                Int64* pV = pValues;
                               
                                Int64* pX = pInput;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)*(pI) / leadDimLen;
                                    *pV++ = *(pInput + (int)*pI++);
                                }
                            }
                        }
                    } else {
                        // just return row / columns 
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( Int64* pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)(*(pI++) / leadDimLen);
                                }
                            }
                        }
                    }
                    #endregion RETURN ROWS / COLUMNS
                } else {
                    #region RETURN INDICES ONLY
                    if (foundIdx != indices.Length) {
                        ret.a = new ILArray<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                        unsafe {
                            fixed (int* pIndices = indices, pRows = ret.GetArrayForWrite()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                while (pI < pLastIndex) {
                                    *pR++ = *pI++;
                                }
                            }
                        }
                    } else {
                        ret.a = array<int>(indices, foundIdx, 1);
                    }
                    #endregion RETURN INDICES ONLY
                }
                return ret;
            }
        }
        /// <summary>
        /// Find nonzero elements in A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="limit">[Optional] Number of elements to search for. If this value is <![CDATA[< 0]]> the function 
        /// will return at most 'limit' nonzero elements from the end of the array ordered by ascending index.
        /// Set to 0 to search full array (default).</param>
        /// <param name="C">[Optional] If not null, the function will return the row indices of nonzero elements 
        /// as main return value. C will therefore hold the column indices of those elements. If A
        /// has more than 2 dimensions, the column indices will go along the 2nd dimension.</param>
        /// <param name="V">[Optional] If not null on entrance, V will hold a copy of the values of nonzero elements returned.</param>
        /// <returns>Vector containing (sequential) indices of nonzero elements in A. If C was 
        /// not null, return value will contain row indices of nonzero elements. </returns>
        /// <remarks>The return type of the index vectors is always 'double'. The return type 
        /// of the element vector 'V' depends on the type of input array A. V and C may be null on 
        /// entrance, indicating their information is not needed. If V is not null (e.g. 'empty()') C must be 
        /// not null also. Any initial data of V or C will be lost.</remarks>
        public static ILRetArray<int> find(ILInArray< Int32> A, int limit = 0,
                      ILOutArray<int> C = null, ILOutArray< Int32> V = null) {
            using (ILScope.Enter(A)) {
                bool create_row_columns = !Object.Equals(C, null);
                bool return_values = !Object.Equals(V, null);
                ILArray<int> ret = 1;
                ILSize inDim = A.Size;
                if (inDim.NumberOfElements == 1) {
                    #region SCALAR
                    // scalar -> return copy
                    if (A.GetValue(0,0) != 0) {
                        if (create_row_columns) {
                            C.a = zeros<int>(ILSize.Scalar1_1);
                        }
                        if (return_values) {
                            V.a = A.C;
                        }
                        return zeros<int>(1, 1);
                    } else {
                        if (create_row_columns) {
                            C.a = empty<int>(ILSize.Empty00);
                        }
                        if (return_values) {
                            V.a = empty<Int32>(ILSize.Empty00);
                        }
                        return empty<int>(ILSize.Empty00);
                    }
                    #endregion SCALAR
                }
                long nrElements = inDim.NumberOfElements;
                
                if (limit != 0) {
                    int lim = Math.Abs(limit);
                    if (lim < nrElements)
                        nrElements = lim;
                }
                int[] indices = ILMemoryPool.Pool.New<int>(nrElements); // init return array with most elements for non logical inarray -> shorten afterwards 
                int foundIdx = 0;
                // physical -> pointer arithmetic
                if (limit >= 0) {
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( Int32* pX = A.GetArrayForRead()) {
                           
                            Int32* lastElement = pX + inDim.NumberOfElements;
                           
                            Int32* tmpIn = pX;
                            int* pI = pIndices;
                            int* pFoundLast = pI + indices.Length;
                            while (tmpIn < lastElement && pI < pFoundLast) {
                                if (*tmpIn != 0)
                                    *pI++ = (int)(tmpIn - pX);
                                tmpIn++;
                            }
                            foundIdx = (int)(pI - pIndices);
                        }
                    }
                } else {
                    // search backwards 
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( Int32* pX = A.GetArrayForRead()) {
                           
                            Int32* lastElementX = pX;
                           
                            Int32* tmpIn = pX + inDim.NumberOfElements;
                            int* pI = pIndices + indices.Length;
                            while (tmpIn > lastElementX && pI > pIndices) {
                                tmpIn--;
                                if (*tmpIn != 0)
                                    *(--pI) = (int)(tmpIn - pX);
                            }
                            foundIdx = (int)(pIndices + indices.Length - pI);
                        }
                    }
                }
                if (foundIdx == 0) {
                    return empty<int>();
                }
                // transform to row / columns; extract values if needed
                int leadDimLen = inDim[0];
                if (create_row_columns) {
                    #region RETURN ROWS / COLUMNS /VALUES
                    C.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    ret.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    if (return_values) {
                        V.a = array< Int32>(ILMemoryPool.Pool.New<  Int32>(foundIdx), foundIdx, 1);
                        // copy values, transform to row/columns
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( Int32* pValues = V.GetArrayForWrite(), pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                               
                                Int32* pV = pValues;
                               
                                Int32* pX = pInput;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)*(pI) / leadDimLen;
                                    *pV++ = *(pInput + (int)*pI++);
                                }
                            }
                        }
                    } else {
                        // just return row / columns 
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( Int32* pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)(*(pI++) / leadDimLen);
                                }
                            }
                        }
                    }
                    #endregion RETURN ROWS / COLUMNS
                } else {
                    #region RETURN INDICES ONLY
                    if (foundIdx != indices.Length) {
                        ret.a = new ILArray<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                        unsafe {
                            fixed (int* pIndices = indices, pRows = ret.GetArrayForWrite()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                while (pI < pLastIndex) {
                                    *pR++ = *pI++;
                                }
                            }
                        }
                    } else {
                        ret.a = array<int>(indices, foundIdx, 1);
                    }
                    #endregion RETURN INDICES ONLY
                }
                return ret;
            }
        }
        /// <summary>
        /// Find nonzero elements in A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="limit">[Optional] Number of elements to search for. If this value is <![CDATA[< 0]]> the function 
        /// will return at most 'limit' nonzero elements from the end of the array ordered by ascending index.
        /// Set to 0 to search full array (default).</param>
        /// <param name="C">[Optional] If not null, the function will return the row indices of nonzero elements 
        /// as main return value. C will therefore hold the column indices of those elements. If A
        /// has more than 2 dimensions, the column indices will go along the 2nd dimension.</param>
        /// <param name="V">[Optional] If not null on entrance, V will hold a copy of the values of nonzero elements returned.</param>
        /// <returns>Vector containing (sequential) indices of nonzero elements in A. If C was 
        /// not null, return value will contain row indices of nonzero elements. </returns>
        /// <remarks>The return type of the index vectors is always 'double'. The return type 
        /// of the element vector 'V' depends on the type of input array A. V and C may be null on 
        /// entrance, indicating their information is not needed. If V is not null (e.g. 'empty()') C must be 
        /// not null also. Any initial data of V or C will be lost.</remarks>
        public static ILRetArray<int> find(ILInArray< float> A, int limit = 0,
                      ILOutArray<int> C = null, ILOutArray< float> V = null) {
            using (ILScope.Enter(A)) {
                bool create_row_columns = !Object.Equals(C, null);
                bool return_values = !Object.Equals(V, null);
                ILArray<int> ret = 1;
                ILSize inDim = A.Size;
                if (inDim.NumberOfElements == 1) {
                    #region SCALAR
                    // scalar -> return copy
                    if (A.GetValue(0,0) != 0.0) {
                        if (create_row_columns) {
                            C.a = zeros<int>(ILSize.Scalar1_1);
                        }
                        if (return_values) {
                            V.a = A.C;
                        }
                        return zeros<int>(1, 1);
                    } else {
                        if (create_row_columns) {
                            C.a = empty<int>(ILSize.Empty00);
                        }
                        if (return_values) {
                            V.a = empty<float>(ILSize.Empty00);
                        }
                        return empty<int>(ILSize.Empty00);
                    }
                    #endregion SCALAR
                }
                long nrElements = inDim.NumberOfElements;
                
                if (limit != 0) {
                    int lim = Math.Abs(limit);
                    if (lim < nrElements)
                        nrElements = lim;
                }
                int[] indices = ILMemoryPool.Pool.New<int>(nrElements); // init return array with most elements for non logical inarray -> shorten afterwards 
                int foundIdx = 0;
                // physical -> pointer arithmetic
                if (limit >= 0) {
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( float* pX = A.GetArrayForRead()) {
                           
                            float* lastElement = pX + inDim.NumberOfElements;
                           
                            float* tmpIn = pX;
                            int* pI = pIndices;
                            int* pFoundLast = pI + indices.Length;
                            while (tmpIn < lastElement && pI < pFoundLast) {
                                if (*tmpIn != 0.0f)
                                    *pI++ = (int)(tmpIn - pX);
                                tmpIn++;
                            }
                            foundIdx = (int)(pI - pIndices);
                        }
                    }
                } else {
                    // search backwards 
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( float* pX = A.GetArrayForRead()) {
                           
                            float* lastElementX = pX;
                           
                            float* tmpIn = pX + inDim.NumberOfElements;
                            int* pI = pIndices + indices.Length;
                            while (tmpIn > lastElementX && pI > pIndices) {
                                tmpIn--;
                                if (*tmpIn != 0.0f)
                                    *(--pI) = (int)(tmpIn - pX);
                            }
                            foundIdx = (int)(pIndices + indices.Length - pI);
                        }
                    }
                }
                if (foundIdx == 0) {
                    return empty<int>();
                }
                // transform to row / columns; extract values if needed
                int leadDimLen = inDim[0];
                if (create_row_columns) {
                    #region RETURN ROWS / COLUMNS /VALUES
                    C.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    ret.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    if (return_values) {
                        V.a = array< float>(ILMemoryPool.Pool.New<  float>(foundIdx), foundIdx, 1);
                        // copy values, transform to row/columns
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( float* pValues = V.GetArrayForWrite(), pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                               
                                float* pV = pValues;
                               
                                float* pX = pInput;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)*(pI) / leadDimLen;
                                    *pV++ = *(pInput + (int)*pI++);
                                }
                            }
                        }
                    } else {
                        // just return row / columns 
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( float* pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)(*(pI++) / leadDimLen);
                                }
                            }
                        }
                    }
                    #endregion RETURN ROWS / COLUMNS
                } else {
                    #region RETURN INDICES ONLY
                    if (foundIdx != indices.Length) {
                        ret.a = new ILArray<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                        unsafe {
                            fixed (int* pIndices = indices, pRows = ret.GetArrayForWrite()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                while (pI < pLastIndex) {
                                    *pR++ = *pI++;
                                }
                            }
                        }
                    } else {
                        ret.a = array<int>(indices, foundIdx, 1);
                    }
                    #endregion RETURN INDICES ONLY
                }
                return ret;
            }
        }
        /// <summary>
        /// Find nonzero elements in A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="limit">[Optional] Number of elements to search for. If this value is <![CDATA[< 0]]> the function 
        /// will return at most 'limit' nonzero elements from the end of the array ordered by ascending index.
        /// Set to 0 to search full array (default).</param>
        /// <param name="C">[Optional] If not null, the function will return the row indices of nonzero elements 
        /// as main return value. C will therefore hold the column indices of those elements. If A
        /// has more than 2 dimensions, the column indices will go along the 2nd dimension.</param>
        /// <param name="V">[Optional] If not null on entrance, V will hold a copy of the values of nonzero elements returned.</param>
        /// <returns>Vector containing (sequential) indices of nonzero elements in A. If C was 
        /// not null, return value will contain row indices of nonzero elements. </returns>
        /// <remarks>The return type of the index vectors is always 'double'. The return type 
        /// of the element vector 'V' depends on the type of input array A. V and C may be null on 
        /// entrance, indicating their information is not needed. If V is not null (e.g. 'empty()') C must be 
        /// not null also. Any initial data of V or C will be lost.</remarks>
        public static ILRetArray<int> find(ILInArray< fcomplex> A, int limit = 0,
                      ILOutArray<int> C = null, ILOutArray< fcomplex> V = null) {
            using (ILScope.Enter(A)) {
                bool create_row_columns = !Object.Equals(C, null);
                bool return_values = !Object.Equals(V, null);
                ILArray<int> ret = 1;
                ILSize inDim = A.Size;
                if (inDim.NumberOfElements == 1) {
                    #region SCALAR
                    // scalar -> return copy
                    if (A.GetValue(0,0).real != 0.0 || A.GetValue(0,0).imag != 0.0) {
                        if (create_row_columns) {
                            C.a = zeros<int>(ILSize.Scalar1_1);
                        }
                        if (return_values) {
                            V.a = A.C;
                        }
                        return zeros<int>(1, 1);
                    } else {
                        if (create_row_columns) {
                            C.a = empty<int>(ILSize.Empty00);
                        }
                        if (return_values) {
                            V.a = empty<fcomplex>(ILSize.Empty00);
                        }
                        return empty<int>(ILSize.Empty00);
                    }
                    #endregion SCALAR
                }
                long nrElements = inDim.NumberOfElements;
                
                if (limit != 0) {
                    int lim = Math.Abs(limit);
                    if (lim < nrElements)
                        nrElements = lim;
                }
                int[] indices = ILMemoryPool.Pool.New<int>(nrElements); // init return array with most elements for non logical inarray -> shorten afterwards 
                int foundIdx = 0;
                // physical -> pointer arithmetic
                if (limit >= 0) {
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( fcomplex* pX = A.GetArrayForRead()) {
                           
                            fcomplex* lastElement = pX + inDim.NumberOfElements;
                           
                            fcomplex* tmpIn = pX;
                            int* pI = pIndices;
                            int* pFoundLast = pI + indices.Length;
                            while (tmpIn < lastElement && pI < pFoundLast) {
                                if ((*tmpIn).real != 0.0 || (*tmpIn).imag != 0.0)
                                    *pI++ = (int)(tmpIn - pX);
                                tmpIn++;
                            }
                            foundIdx = (int)(pI - pIndices);
                        }
                    }
                } else {
                    // search backwards 
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( fcomplex* pX = A.GetArrayForRead()) {
                           
                            fcomplex* lastElementX = pX;
                           
                            fcomplex* tmpIn = pX + inDim.NumberOfElements;
                            int* pI = pIndices + indices.Length;
                            while (tmpIn > lastElementX && pI > pIndices) {
                                tmpIn--;
                                if ((*tmpIn).real != 0.0 || (*tmpIn).imag != 0.0)
                                    *(--pI) = (int)(tmpIn - pX);
                            }
                            foundIdx = (int)(pIndices + indices.Length - pI);
                        }
                    }
                }
                if (foundIdx == 0) {
                    return empty<int>();
                }
                // transform to row / columns; extract values if needed
                int leadDimLen = inDim[0];
                if (create_row_columns) {
                    #region RETURN ROWS / COLUMNS /VALUES
                    C.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    ret.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    if (return_values) {
                        V.a = array< fcomplex>(ILMemoryPool.Pool.New<  fcomplex>(foundIdx), foundIdx, 1);
                        // copy values, transform to row/columns
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( fcomplex* pValues = V.GetArrayForWrite(), pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                               
                                fcomplex* pV = pValues;
                               
                                fcomplex* pX = pInput;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)*(pI) / leadDimLen;
                                    *pV++ = *(pInput + (int)*pI++);
                                }
                            }
                        }
                    } else {
                        // just return row / columns 
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( fcomplex* pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)(*(pI++) / leadDimLen);
                                }
                            }
                        }
                    }
                    #endregion RETURN ROWS / COLUMNS
                } else {
                    #region RETURN INDICES ONLY
                    if (foundIdx != indices.Length) {
                        ret.a = new ILArray<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                        unsafe {
                            fixed (int* pIndices = indices, pRows = ret.GetArrayForWrite()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                while (pI < pLastIndex) {
                                    *pR++ = *pI++;
                                }
                            }
                        }
                    } else {
                        ret.a = array<int>(indices, foundIdx, 1);
                    }
                    #endregion RETURN INDICES ONLY
                }
                return ret;
            }
        }
        /// <summary>
        /// Find nonzero elements in A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="limit">[Optional] Number of elements to search for. If this value is <![CDATA[< 0]]> the function 
        /// will return at most 'limit' nonzero elements from the end of the array ordered by ascending index.
        /// Set to 0 to search full array (default).</param>
        /// <param name="C">[Optional] If not null, the function will return the row indices of nonzero elements 
        /// as main return value. C will therefore hold the column indices of those elements. If A
        /// has more than 2 dimensions, the column indices will go along the 2nd dimension.</param>
        /// <param name="V">[Optional] If not null on entrance, V will hold a copy of the values of nonzero elements returned.</param>
        /// <returns>Vector containing (sequential) indices of nonzero elements in A. If C was 
        /// not null, return value will contain row indices of nonzero elements. </returns>
        /// <remarks>The return type of the index vectors is always 'double'. The return type 
        /// of the element vector 'V' depends on the type of input array A. V and C may be null on 
        /// entrance, indicating their information is not needed. If V is not null (e.g. 'empty()') C must be 
        /// not null also. Any initial data of V or C will be lost.</remarks>
        public static ILRetArray<int> find(ILInArray< complex> A, int limit = 0,
                      ILOutArray<int> C = null, ILOutArray< complex> V = null) {
            using (ILScope.Enter(A)) {
                bool create_row_columns = !Object.Equals(C, null);
                bool return_values = !Object.Equals(V, null);
                ILArray<int> ret = 1;
                ILSize inDim = A.Size;
                if (inDim.NumberOfElements == 1) {
                    #region SCALAR
                    // scalar -> return copy
                    if (A.GetValue(0,0).real != 0.0 || A.GetValue(0,0).imag != 0.0) {
                        if (create_row_columns) {
                            C.a = zeros<int>(ILSize.Scalar1_1);
                        }
                        if (return_values) {
                            V.a = A.C;
                        }
                        return zeros<int>(1, 1);
                    } else {
                        if (create_row_columns) {
                            C.a = empty<int>(ILSize.Empty00);
                        }
                        if (return_values) {
                            V.a = empty<complex>(ILSize.Empty00);
                        }
                        return empty<int>(ILSize.Empty00);
                    }
                    #endregion SCALAR
                }
                long nrElements = inDim.NumberOfElements;
                
                if (limit != 0) {
                    int lim = Math.Abs(limit);
                    if (lim < nrElements)
                        nrElements = lim;
                }
                int[] indices = ILMemoryPool.Pool.New<int>(nrElements); // init return array with most elements for non logical inarray -> shorten afterwards 
                int foundIdx = 0;
                // physical -> pointer arithmetic
                if (limit >= 0) {
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( complex* pX = A.GetArrayForRead()) {
                           
                            complex* lastElement = pX + inDim.NumberOfElements;
                           
                            complex* tmpIn = pX;
                            int* pI = pIndices;
                            int* pFoundLast = pI + indices.Length;
                            while (tmpIn < lastElement && pI < pFoundLast) {
                                if ((*tmpIn).real != 0.0 || (*tmpIn).imag != 0.0)
                                    *pI++ = (int)(tmpIn - pX);
                                tmpIn++;
                            }
                            foundIdx = (int)(pI - pIndices);
                        }
                    }
                } else {
                    // search backwards 
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( complex* pX = A.GetArrayForRead()) {
                           
                            complex* lastElementX = pX;
                           
                            complex* tmpIn = pX + inDim.NumberOfElements;
                            int* pI = pIndices + indices.Length;
                            while (tmpIn > lastElementX && pI > pIndices) {
                                tmpIn--;
                                if ((*tmpIn).real != 0.0 || (*tmpIn).imag != 0.0)
                                    *(--pI) = (int)(tmpIn - pX);
                            }
                            foundIdx = (int)(pIndices + indices.Length - pI);
                        }
                    }
                }
                if (foundIdx == 0) {
                    return empty<int>();
                }
                // transform to row / columns; extract values if needed
                int leadDimLen = inDim[0];
                if (create_row_columns) {
                    #region RETURN ROWS / COLUMNS /VALUES
                    C.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    ret.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    if (return_values) {
                        V.a = array< complex>(ILMemoryPool.Pool.New<  complex>(foundIdx), foundIdx, 1);
                        // copy values, transform to row/columns
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( complex* pValues = V.GetArrayForWrite(), pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                               
                                complex* pV = pValues;
                               
                                complex* pX = pInput;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)*(pI) / leadDimLen;
                                    *pV++ = *(pInput + (int)*pI++);
                                }
                            }
                        }
                    } else {
                        // just return row / columns 
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( complex* pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)(*(pI++) / leadDimLen);
                                }
                            }
                        }
                    }
                    #endregion RETURN ROWS / COLUMNS
                } else {
                    #region RETURN INDICES ONLY
                    if (foundIdx != indices.Length) {
                        ret.a = new ILArray<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                        unsafe {
                            fixed (int* pIndices = indices, pRows = ret.GetArrayForWrite()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                while (pI < pLastIndex) {
                                    *pR++ = *pI++;
                                }
                            }
                        }
                    } else {
                        ret.a = array<int>(indices, foundIdx, 1);
                    }
                    #endregion RETURN INDICES ONLY
                }
                return ret;
            }
        }
        /// <summary>
        /// Find nonzero elements in A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="limit">[Optional] Number of elements to search for. If this value is <![CDATA[< 0]]> the function 
        /// will return at most 'limit' nonzero elements from the end of the array ordered by ascending index.
        /// Set to 0 to search full array (default).</param>
        /// <param name="C">[Optional] If not null, the function will return the row indices of nonzero elements 
        /// as main return value. C will therefore hold the column indices of those elements. If A
        /// has more than 2 dimensions, the column indices will go along the 2nd dimension.</param>
        /// <param name="V">[Optional] If not null on entrance, V will hold a copy of the values of nonzero elements returned.</param>
        /// <returns>Vector containing (sequential) indices of nonzero elements in A. If C was 
        /// not null, return value will contain row indices of nonzero elements. </returns>
        /// <remarks>The return type of the index vectors is always 'double'. The return type 
        /// of the element vector 'V' depends on the type of input array A. V and C may be null on 
        /// entrance, indicating their information is not needed. If V is not null (e.g. 'empty()') C must be 
        /// not null also. Any initial data of V or C will be lost.</remarks>
        public static ILRetArray<int> find(ILInArray< byte> A, int limit = 0,
                      ILOutArray<int> C = null, ILOutArray< byte> V = null) {
            using (ILScope.Enter(A)) {
                bool create_row_columns = !Object.Equals(C, null);
                bool return_values = !Object.Equals(V, null);
                ILArray<int> ret = 1;
                ILSize inDim = A.Size;
                if (inDim.NumberOfElements == 1) {
                    #region SCALAR
                    // scalar -> return copy
                    if (A.GetValue(0,0) != 0) {
                        if (create_row_columns) {
                            C.a = zeros<int>(ILSize.Scalar1_1);
                        }
                        if (return_values) {
                            V.a = A.C;
                        }
                        return zeros<int>(1, 1);
                    } else {
                        if (create_row_columns) {
                            C.a = empty<int>(ILSize.Empty00);
                        }
                        if (return_values) {
                            V.a = empty<byte>(ILSize.Empty00);
                        }
                        return empty<int>(ILSize.Empty00);
                    }
                    #endregion SCALAR
                }
                long nrElements = inDim.NumberOfElements;
                
                if (limit != 0) {
                    int lim = Math.Abs(limit);
                    if (lim < nrElements)
                        nrElements = lim;
                }
                int[] indices = ILMemoryPool.Pool.New<int>(nrElements); // init return array with most elements for non logical inarray -> shorten afterwards 
                int foundIdx = 0;
                // physical -> pointer arithmetic
                if (limit >= 0) {
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( byte* pX = A.GetArrayForRead()) {
                           
                            byte* lastElement = pX + inDim.NumberOfElements;
                           
                            byte* tmpIn = pX;
                            int* pI = pIndices;
                            int* pFoundLast = pI + indices.Length;
                            while (tmpIn < lastElement && pI < pFoundLast) {
                                if (*tmpIn != 0)
                                    *pI++ = (int)(tmpIn - pX);
                                tmpIn++;
                            }
                            foundIdx = (int)(pI - pIndices);
                        }
                    }
                } else {
                    // search backwards 
                    unsafe {
                        fixed (int* pIndices = indices)
                        fixed ( byte* pX = A.GetArrayForRead()) {
                           
                            byte* lastElementX = pX;
                           
                            byte* tmpIn = pX + inDim.NumberOfElements;
                            int* pI = pIndices + indices.Length;
                            while (tmpIn > lastElementX && pI > pIndices) {
                                tmpIn--;
                                if (*tmpIn != 0)
                                    *(--pI) = (int)(tmpIn - pX);
                            }
                            foundIdx = (int)(pIndices + indices.Length - pI);
                        }
                    }
                }
                if (foundIdx == 0) {
                    return empty<int>();
                }
                // transform to row / columns; extract values if needed
                int leadDimLen = inDim[0];
                if (create_row_columns) {
                    #region RETURN ROWS / COLUMNS /VALUES
                    C.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    ret.a = array<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                    if (return_values) {
                        V.a = array< byte>(ILMemoryPool.Pool.New<  byte>(foundIdx), foundIdx, 1);
                        // copy values, transform to row/columns
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( byte* pValues = V.GetArrayForWrite(), pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                               
                                byte* pV = pValues;
                               
                                byte* pX = pInput;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)*(pI) / leadDimLen;
                                    *pV++ = *(pInput + (int)*pI++);
                                }
                            }
                        }
                    } else {
                        // just return row / columns 
                        unsafe {
                            fixed (int* pIndices = indices,
                                   pRows = ret.GetArrayForWrite(), pCols = C.GetArrayForWrite())
                            fixed ( byte* pInput = A.GetArrayForRead()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                int* pC = pCols;
                                while (pI < pLastIndex) {
                                    *pR++ = *(pI) % leadDimLen;
                                    *pC++ = (int)(*(pI++) / leadDimLen);
                                }
                            }
                        }
                    }
                    #endregion RETURN ROWS / COLUMNS
                } else {
                    #region RETURN INDICES ONLY
                    if (foundIdx != indices.Length) {
                        ret.a = new ILArray<int>(ILMemoryPool.Pool.New<int>(foundIdx), foundIdx, 1);
                        unsafe {
                            fixed (int* pIndices = indices, pRows = ret.GetArrayForWrite()) {
                                int* pI = (limit >= 0) ?
                                        pIndices : (pIndices + indices.Length - foundIdx);
                                int* pLastIndex = pI + foundIdx;
                                int* pR = pRows;
                                while (pI < pLastIndex) {
                                    *pR++ = *pI++;
                                }
                            }
                        }
                    } else {
                        ret.a = array<int>(indices, foundIdx, 1);
                    }
                    #endregion RETURN INDICES ONLY
                }
                return ret;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        #region logicals 
        /// <summary>
        /// Find nonzero elements in A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="limit">Number of elements to search for. If this value is <![CDATA[< 0]]> the function 
        /// will return at most 'limit' nonzero elements from the end of the array ordered by ascending index.
        /// Set to 0 to search full array (default).</param>
        /// <param name="C">If not null, the function will return the row indices of nonzero elements 
        /// as main return value. C will therefore hold the column indices of those elements. If A
        /// has more than 2 dimensions, the column indices will go along the 2nd dimension.</param>
        /// <param name="V">If not null on entrance, V will hold a copy of the values of nonzero elements returned.</param>
        /// <returns>Vector containing (sequential) indices of nonzero elements in A. If C was 
        /// not null, return value will contain row indices of nonzero elements. </returns>
        /// <remarks>The return type of the index vectors is always 'double'. The return type 
        /// of the element vector 'V' depends on the type of input array A. V and C may be null on 
        /// entrance, indicating their information is not needed. If V is not null (e.g. 'empty()') C must be 
        /// not null also. Any initial data of V or C will be lost.</remarks>
        public static ILRetArray<int> find(ILInLogical A, int limit,
                        ILOutArray<int> C, ILOutLogical V) {
            using (ILScope.Enter(A)) {
                if (isnullorempty(A)) {
                    if (!isnull(V)) {
                        V.a = new ILLogical(ILSize.Empty00);
                    }
                    return empty<int>();
                }
                ILArray<byte> tmpA = new ILArray<byte>(A.Storage);
                ILArray<byte> tmpV = null;
                if (!isnull(V)) {
                    tmpV = new ILArray<byte>(V.Storage);
                }
                ILArray<int> ret = find(tmpA, limit, C, tmpV);
                if (!isnull(tmpV)) {
                    V.a = new ILLogical(new ILLogicalStorage( tmpV.Storage.GetDataArray(), tmpV.S));
                }
                return ret; 
            }
        }

        #endregion

    }
}
