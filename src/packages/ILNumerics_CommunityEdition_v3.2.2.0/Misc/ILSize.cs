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
using ILNumerics.Storage;     
using ILNumerics.Exceptions;
using System.Collections.Generic; 

namespace ILNumerics
{
    /// <summary>
    /// ILSize - dimensions for array objects (immutable)
    /// </summary>
    /// <remarks>The class internally manages the dimensions of ILNumerics arrays. 
    /// The class is immutable. Therefore, once created, it informs the user 
    /// about all dimension related properties, but cannot get altered.</remarks>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{ToString(),nq}")]
    public class ILSize {
        
        #region attributes
        internal int [] m_dims; 
        int m_nrDims = 0;
        int m_numberOfElements = 0;
        int m_length = 0;
        int [] m_seqDistancesCache; 
        static ILSize s_emptyDimension = new ILSize(0,0); 
        static ILSize s_scalarDimension = new ILSize(1,1); 
        static ILSize s_twoElementDim = new ILSize(2,1); 
        static ILSize s_threeElementDim = new ILSize(3,1);
        
        [ThreadStatic]
        static List<int> s_listCache;
        static List<int> ListCache {
            get {
                List<int> ret = s_listCache;
                if (ret == null) {
                    s_listCache = new List<int>();
                    ret = s_listCache;
                }
                return ret; 
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Create new ILSize 
        /// </summary>
        /// <param name="dims">variable length dimensions specifier</param>
        /// <remarks>Trailing singleton dimensions of dims will be kept.</remarks>
        public ILSize(params int[] dims) {
            if (dims == null)
                throw new ILArgumentException("invalid dimension specification. the number of dimensions must not be 0.");
            int outLen = Math.Max(2, dims.Length);
            m_dims = new int[outLen];
            for (int i = 0; i < dims.Length; i++) {
                m_dims[i] = dims[i];
            }
            for (int i = dims.Length; i < 2; i++) {
                m_dims[i] = 1;
            }
            m_numberOfElements = 1;
            m_nrDims = m_dims.Length;
            int t = 0,
                d;
            for (; t < m_dims.Length; t++) {
                d = m_dims[t];
                if (d > m_length) m_length = d;
                m_numberOfElements *= d;
            }
        }

        /// <summary>
        /// Create new size descriptor from given data
        /// </summary>
        /// <param name="size">Size description</param>
        public ILSize(params ILBaseArray[] size)
            : this (convert2int(size)) {

        }

        /// <summary>
        /// Create new size descriptor
        /// </summary>
        /// <param name="trimSingletons">true: trailing singleton 
        /// dimensions will be trimmed, false: keep trailing singleton dimensions</param>
        /// <param name="size">Size description</param>
        public ILSize (bool trimSingletons, params int[] size)  {
            if (!trimSingletons) {
                if (size == null)
                    throw new ILArgumentException("invalid size specification. the number of dimensions must not be 0.");
                int outLen = Math.Max(2, size.Length);
                m_dims = new int[outLen];
                for (int i = 0; i < size.Length; i++) {
                    m_dims[i] = size[i];
                }
                for (int i = size.Length; i < 2; i++) {
                    m_dims[i] = 1;
                }
                m_numberOfElements = 1;
                m_nrDims = m_dims.Length;
                int t = 0,
                    d;
                for (; t < m_dims.Length; t++) {
                    d = m_dims[t];
                    if (d > m_length) m_length = d;
                    m_numberOfElements *= d;
                }
            } else {
                ListCache.Clear();
                s_listCache.AddRange(size);
                while (s_listCache.Count < 2)
                    s_listCache.Add(1);
                if (trimSingletons)
                    for (int i = s_listCache.Count; i-- > 2; ) {
                        if (s_listCache[i] == 1) s_listCache.RemoveAt(i);
                        else break;
                    }
                m_dims = s_listCache.ToArray();
                m_numberOfElements = 1;
                m_nrDims = m_dims.Length;
                int t = 0,
                    d;
                for (; t < m_dims.Length; t++) {
                    d = m_dims[t];
                    if (d > m_length) m_length = d;
                    m_numberOfElements *= d;
                }
            }
        }

        /// <summary>
        /// An size descriptor of size 0x0
        /// </summary>
        public static ILSize Empty00 {
            get { 
                return s_emptyDimension; }
        }
        /// <summary>
        /// An size descriptor of size 1x1
        /// </summary>
        public static ILSize Scalar1_1
        {
            get { 
                return s_scalarDimension; }
        }
        /// <summary>
        /// An size descriptor of size 2x1
        /// </summary>

        public static ILSize Column2_1 {
            get { 
                return s_twoElementDim; }
        }
        /// <summary>
        /// An size descriptor of size 3x1
        /// </summary>
        public static ILSize Column3_1 {
            get { 
                return s_threeElementDim; }
        }
        #endregion

        #region properties
        /// <summary>Get number of dimensions.</summary>
        public int NumberOfDimensions {
            get {
                return m_nrDims; 
            }
        }

        /// <summary>
        /// Number of non singleton dimensions of the array
        /// </summary>
        /// <remarks>Non singleton dimensions are dimensions which length is larger than 1. 
        /// Empty dimensions (length = 0) will not be taken into account.</remarks>
        public int NonSingletonDimensions {
            // ToDo: Definition of non-singelton (= not 1) should be reconsidered. Here 0 is not counted as non-singelton
            get {
                int ret = 0;
                for (int i = 0; i < m_nrDims; i++) {
                    if (m_dims[i] > 1) ret++;
                }
                return ret;
            }
        }
        /// <summary>
        /// Number of elements in the array
        /// </summary>
        public int NumberOfElements {
            get {
                return m_numberOfElements;
            }
        }
        /// <summary>
        /// Length of the longest dimension
        /// </summary>
        public int Longest {
            get {
                return m_length; 
            }
        }
        /// <summary>
        /// Find default dimension to work on
        /// </summary>
        /// <returns>Index of first non singleton dimension (i.e. dimension that is not 1); 0, if this array is a scalar.</returns>
        public int WorkingDimension() {
            for (int i = 0; i < m_nrDims; i++) {
                if (m_dims[i] != 1) return i; 
            }
            return 0;
        }
        #endregion 

        #region public member
        /// <summary>
        /// Storage distance of elements in dimension dim
        /// </summary>
        /// <param name="dim">0-based index of dimension to query the element distance for</param>
        /// <returns>Storage distance of elements between adjacent elements of dimension dim
        /// </returns>
        /// <remarks>If dimension index dim is larger than the number of 
        /// dimensions of this array, the number of elements will 
        /// be returned (trailing singleton dimensions).</remarks>
        public int SequentialIndexDistance(int dim) {
            if (dim >= m_nrDims)
                return m_numberOfElements; 
            dim %= m_nrDims; 
            int ret = 1; 
            for (int i = 0; i < dim && i < m_nrDims; i++) {
                ret *= m_dims[i];
            }
            return ret; 
        }
        /// <summary>
        /// Distances between adjacent elements for all dimensions
        /// </summary>
        /// <param name="minLength">minimum length of array to be 
        /// returned. If this is larger than the number of dimensions 
        /// in this size descriptor, the array will have minLength elements, 
        /// with elements outside this dimensions repeating the value 
        /// of the last dimension. The length of the array returned will 
        /// be equal or greater than max(minLength,NumberOfDimensions).</param>
        /// <remarks>This is provided for performance reasons and should be 
        /// used internally only. It enables developer of index access routines 
        /// to cache the elements distances directly inside their functions 
        /// without having to query the info on every index access.
        /// <para>Keep in mind, only the distances for the number of my 
        /// dimensions are returned. Higher dimensions must be set to 
        /// NumberOfElements if needed. This is different than querying 
        /// the distances by SequentialIndexDistance(int), which will assume 
        /// and return trailing dimensions to be 1.</para>
        /// <para>IMPORTANT: ALTERING THE ARRAY RETURNED IS NOT ALLOWED AND 
        /// MAY LEAD TO SERIOUS INSTABILITY AND UNWANTED SIDE EFFECTS!</para></remarks>
        internal int[] GetSequentialIndexDistances(int minLength) {
            minLength = Math.Max(m_nrDims,minLength);
            if (m_seqDistancesCache == null || m_seqDistancesCache.Length < minLength){
                m_seqDistancesCache = new int[minLength]; 
                int tmp = 1, i = 0; 
                for (; i < m_nrDims; i++) {
                    m_seqDistancesCache[i] = tmp; 
                    tmp *= m_dims[i];
                }
                for (; i < m_seqDistancesCache.Length; i++) {
                    m_seqDistancesCache[i] = tmp; 
                }
            }
            return m_seqDistancesCache; 
        }
        /// <summary>
        /// Transfer my dimensions to integer array 
        /// </summary>
        /// <returns>Integer array containing a copy of all dimensions length</returns>
        public int[] ToIntArray(){
            return (int[])m_dims.Clone(); 
        }
        /// <summary>
        /// Transfer my dimensions to integer array 
        /// </summary>
        /// <param name="length">Minimum length of output array. If length 
        /// is larger than my dimensions, trailing ones will be added.</param>
        /// <returns>Integer array containing a copy of dimensions length. 
        /// Trailing elements outside my dims will be one.</returns>
        public int[] ToIntArray (int length) {
            int[] ret; 
            ret = new int[length > m_nrDims ? length : m_nrDims];
            Array.Copy(m_dims,0,ret,0,m_nrDims);
            for (int i = m_nrDims; i < length; i++) {
                ret[i] = 1; 
            }
            return ret; 
        }
        /// <summary>
        /// return dimension vector, fixed length, for subarray operations
        /// </summary>
        /// <param name="length"></param>
        /// <returns>dimension vector, corresponds to reshaped or unlimited dimensions</returns>
        internal int[] ToIntArrayEx (int length) {
            int[] ret; 
            if (length == m_nrDims) {
                ret = new int[length];
                Array.Copy(m_dims,0,ret,0,m_nrDims);
            } else if (length > m_nrDims) {
                ret = new int[length];
                int i = 0; 
                for (; i < m_dims.Length; i++) {
                    ret[i] = m_dims[i]; 
                }
                for (; i < length; i++) {
                    ret[i] = 1; 
                }
            } else if (length > 0) {
                ret = new int[length];
                int i = 0; 
                for (; i < length; i++) {
                    ret[i] = m_dims[i]; 
                }
                for (int a = i--; a < m_dims.Length; a++) {
                    ret[i] *= m_dims[a]; 
                }
            } else 
                throw new ILArgumentException("the length parameter must be positive"); 
            return ret; 
        }
        /// <summary>
        /// Transform indices from int[] System.Array into sequential index of underlying 1dim array 
        /// </summary>
        /// <param name="idx">int array of nrDims length, min length: 1, all indices must fit into my dimensions</param>
        /// <returns>Index pointing to element defined by 'idx'</returns>
        public int IndexFromArray(int[] idx) {      
            int ret = idx[0];
            if (ret < 0 || ret >= m_dims[0]) 
                throw new ILArgumentException("index out of bounds, dimension # 0");
            int i = 1, tmp, len = (idx.Length < m_nrDims) ? idx.Length : m_nrDims; 
            for (; i < len-1; i++) {
                tmp = idx[i]; 
                if (tmp >= m_dims[i] || tmp < 0)
                    throw new ILArgumentException("index out of bounds, dimension # " + i);
                ret += tmp * SequentialIndexDistance(i); 
            }
            if (i < len) {
                tmp = idx[i];
                if (tmp < 0 || tmp >= m_dims[i]) throw new ILArgumentException("index out of bounds, dimension # " + i);
                ret += tmp * SequentialIndexDistance(i);
            }
            return ret; 
        }
        /// <summary>
        /// Transform dimension position into sequential index, gather expand 
        /// information
        /// </summary>
        /// <param name="idx">int array of arbitrary length</param>
        /// <param name="MustExpand">[output] true, if the indices 
        /// given address an element outside of 
        /// this dimensions size. In this case, the output parameter 
        /// 'Dimensions' carry the sizes 
        /// of new dimensions needed. False otherwise</param>
        /// <param name="dimensions">sizes of dimension if expansion is needed. 
        /// Must be predefined to length of max(idx.Length,m_nrDims) at least</param>
        /// <returns>Index number pointing to the value's position in 
        /// sequential storage.</returns>
        /// <remarks>no checks are made for idx to fit inside dimensions! 
        /// This functions is used for left side assignments. Therefore it 
        /// computes the destination index also if it lays outside 
        /// the array bounds.</remarks>
        internal int IndexFromArray(ref bool MustExpand, ref int[] dimensions, int[] idx) {
            int tmp; 
            if (idx.Length < m_nrDims) {
                #region idx < nrDims
                // expanding is allowed for all but the last specified dimension
                // reason: if less than m_nrDims idx entries exist, the array is 
                // reshaped to that number of dimensions and the index access 
                // computed according to the reshaped version. Attempts to expand
                // that last (virtual) dimension is not allowed than since that 
                // dimension would be ambigous. 
                #region special case: sequential addressing
                if (idx.Length < 2) {
                    if (idx.Length == 1) {
                        tmp = idx[0]; 
                        if (tmp >= m_numberOfElements) {
                            // allowed only for scalars and vectors
                            if (m_dims[0] == m_numberOfElements) {
                                dimensions[0] = tmp+1; 
                                MustExpand = true; 
                                return tmp; 
                            } else if (m_dims[1] == m_numberOfElements) {
                                dimensions[1] = tmp+1; 
                                MustExpand = true; 
                                return tmp; 
                            } else 
                                throw new ILArgumentException("invalid attempt to expand non vector sized array"); 
                        } else {
                            return tmp; 
                        }
                    }
                    throw new ILArgumentException("invalid index specification: must not be empty");
                }
                #endregion
                int d = 0, faktor = 1; 
                int ret = 0; 
                tmp = idx[0]; 
                while (d < idx.Length-1) {
                    if (tmp < 0)
                        throw new ILArgumentException("check index at dimension #" + d.ToString() + "!");
                    if (tmp >= m_dims[d]) {
                        dimensions[d] = tmp+1; 
                        MustExpand = true; 
                        ret += (faktor * tmp); 
                        faktor *= tmp+1; 
                    } else {
                        ret += (faktor * tmp); 
                        faktor *= m_dims[d]; 
                    }
                    tmp = idx[++d];
                }
                while (d < m_nrDims) {
                    ret += faktor * ((tmp % m_dims[d])); 
                    tmp /= m_dims[d]; 
                    faktor *= m_dims[d++]; 
                }
                if (tmp > 0) 
                    throw new ILArgumentException("expanding is allowed for explicitly bounded dimensions only! You must specify indices into all existing dimensions."); 
                return ret;
                #endregion
            } else if (idx.Length == m_nrDims) {
                // expanding is allowed for all dimensions 
                int d = 0,faktor = 1; 
                int ret = 0; 
                while (d < idx.Length) {
                    tmp = idx[d];
                    if (tmp < 0)
                        throw new ILArgumentException("check index at dimension #" + d.ToString() + " !");
                    if (tmp >= m_dims[d]) {
                        dimensions[d] = tmp+1; 
                        MustExpand = true; 
                        ret += (faktor * tmp); 
                        faktor *= tmp+1; 
                    } else {
                        ret += (faktor * tmp); 
                        faktor *= m_dims[d]; 
                    }
                    d++; 
                }
                return ret;
            } else {
                // idx dimensions are larger than my dimensions
                int d = 0, faktor = 1; 
                int ret = 0;
                tmp = idx[0]; 
                while (d < m_nrDims) {
                    tmp = idx[d];
                    if (tmp < 0)
                        throw new ILArgumentException("check index at dimension " + d.ToString() + "!");
                    if (tmp >= m_dims[d]) {
                        dimensions[d] = tmp+1; 
                        MustExpand = true; 
                        ret += (faktor * tmp); 
                        faktor *= tmp+1; 
                   } else { 
                        ret += (faktor * tmp); 
                        faktor *= m_dims[d]; 
                    }
                    d++; 
                }
                while (d < idx.Length) {
                    tmp = idx[d]; 
                    if (tmp > 0) {
                        dimensions[d] = tmp +1; 
                        MustExpand = true; 
                    }
                    d++; 
                    faktor *= tmp; 
                    ret += faktor; 
                }
                return ret; 
            }
        }
        /// <summary>
        /// Unshift dimensions of indices from int[] Array 
        /// and translate to index for sequential storage access 
        /// in my dimensions </summary>
        /// <param name="idx">int array of the same length as 
        /// the number of dimensions of this storage. Indices must 
        /// lay within my dimensions.</param>
        /// <param name="unshift">Number of dimensions to unshift 
        /// idx before computing index</param>
        /// <returns>Index number pointing to the value's position 
        /// in sequential storage.</returns>
        /// <remarks>If idx contains elements (indices) larger than 
        /// my dimension bounds, an exception will be thrown. If unshift 
        /// is 0, the length of idx may be smaller than the length of 
        /// my dimensions. However, with unshift &gt; 0 the result 
        /// is undefined.</remarks>
        public int IndexFromArray(int[] idx, int unshift) {
            unshift %= m_nrDims;
            int faktor = m_dims[0];
            int ret = idx[(-unshift) % m_nrDims]; 
            int d = 1;
            for (; d<idx.Length; d++) {
                ret += idx[(-unshift + d) % m_nrDims] * faktor; 
                faktor *= m_dims[d]; 
            }
            return ret; 			
        }
        /// <summary>
        /// Return shifted version
        /// </summary>
        /// <param name="shift">Number of dimensions to shift. The value
        /// will be considered modules the number of dimensions of 
        /// this size descriptor.</param>
        /// <returns>Shifted version of this size descriptor object.</returns>
        public ILSize GetShifted(int shift){ 
            shift %= m_nrDims; 
            if (shift < 0) shift += m_nrDims; 
            int [] tmp = new int [m_nrDims]; 
            int id;
            for (int d = 0; d < m_nrDims; d++) {
                id = (d + shift) % m_nrDims;
                tmp[d] = m_dims[id];
            }
            return new ILSize(tmp);
        }
        /// <summary>
        /// Create dimension sizes for reshaping index adressing
        /// </summary>
        /// <param name="dimCount">Needed number of destination array dimensions</param>
        /// <returns>Dimension sizes, cutted trailing dimensions are multiplied to last dimension returned.</returns>
        public int[] GetReshapedSize(int dimCount) {
            dimCount = Math.Max(1,dimCount); 
            if (dimCount >= m_nrDims) {
                return ToIntArray(dimCount); 
            }
            int[] ret = new int[Math.Max(dimCount,2)];
            for (int i = 0; i < dimCount; i++) {
                ret[i] = m_dims[i]; 
            }
            for (int i = dimCount; i < m_nrDims; i++) {
                ret[dimCount - 1] *= m_dims[i]; 
            }
            if (dimCount < 2) 
                ret[1] = 1; 
            return ret; 
        }
        /// <summary>
        /// Get length for dimension specified (Readonly)
        /// </summary>
        /// <param name="idx">Index of dimension</param>
        /// <returns>Length of dimension specified by idx</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If idx is negative</exception>
        /// <remarks><para>For idx corresponds to an existing dimension, 
        /// the length of that dimension is returned. If idx is larger than 
        /// the number of dimensions 1 is returned. </para>
        /// </remarks>
        public int this [int idx] {
            get {
                if (idx < 0) 
                    throw new ArgumentOutOfRangeException("index","Index out of Range!");
                else if (idx >= m_nrDims) {
                    return 1; 
                }
                return m_dims[idx];
            }
        }
        /// <summary>
        /// Compares the size of this dimension to another dimension object. 
        /// </summary>
        /// <param name="dim2">size descriptor to compare this to.</param>
        /// <returns>Returns true if the sizes are the same, else returns false. 
        /// The comparison is made by recognizing singleton dimensions. Therefore 
        /// only non singleton dimensions are compared in the order of their 
        /// appearance. </returns>
        /// <remarks>The function returns true, if the squeezed dimensions of 
        /// both size descriptors match.</remarks>
        public bool IsSameSize(ILSize dim2) { 
            if (dim2.NumberOfElements != m_numberOfElements) return false; 
            for (int d2 = dim2.NumberOfDimensions,d1 = m_nrDims;d1 >= 0;) {
                d1--; d2--;
                while (d1 >= 0 && m_dims[d1]== 1) d1--; 
                while (d2 >= 0 && dim2[d2]  == 1) d2--; 
                if (d1 >= 0 && d2 >= 0) {
                    if (m_dims[d1] != dim2[d2]) 
                        return false;

                }
            }
            return true;
        }
        /// <summary>
        /// Compares the shape of this dimension to another dimension object 
        /// </summary>
        /// <param name="dim2">size descriptor to compare this to.</param>
        /// <returns>Returns true if the shapes are the same, else returns false. </returns>
        /// <remarks>This function is more strict than IsSameSize. In order 
        /// for two dimensions to have the same shape, ALL dimensions must match - 
        /// even singleton dimensions.</remarks>
        public bool IsSameShape(ILSize dim2) { 
            if (dim2.NumberOfElements != m_numberOfElements) return false; 
            if (dim2.NumberOfDimensions != m_nrDims) return false; 
            for (int d1 = m_nrDims;d1-->0;) { 
                if (m_dims[d1] != dim2.m_dims[d1]) 
                    return false;
            }
            return true;
        }
        /// <summary>
        /// [deprecated] Create copy of this size descriptor having all singleton 
        /// dimensions removed.
        /// </summary>
        /// <returns>a squeezed copy</returns>
        /// <remarks>This function is deprecated. Use the ILSize.Squeeze()
        /// memeber instead. </remarks>
        [Obsolete("Use the ILSize.Squeeze() instead")]
        public ILSize GetSqueezed() {
            return Squeeze();
        }
        /// <summary>
        /// Create and return copy without singleton dimensions
        /// </summary>
        /// <returns>Copy of this size descriptor having all singleton dimensions removed.</returns>
        /// <remarks> This function does not alter this object (since ILSize is 
        /// immutable).
        /// <para>All arrays in ILNumerics have at least 2 dimensions. 
        /// Therefore all but the first two singleton dimensions can be removed.</para>
        /// </remarks>
        public ILSize Squeeze() {
            List<int> tmp = new List<int>(); 
            foreach (int d in m_dims) {
                if (d != 1) tmp.Add(d); 
            }
            while (tmp.Count < 2) { 
                tmp.Add(1); 
            }
            return new ILSize(tmp.ToArray());
        }
        /// <summary>
        /// Return size descriptor, having trailing singleton dimensions removed
        /// </summary>
        /// <returns>Copy without trailing singleton dimensions</returns>
        /// <remarks>This object will NOT be altered. As usual for all ILArrays, 
        /// the result wil have at least 2 dimensions.</remarks>
        public ILSize Trim() {
            if (m_nrDims == 2 || m_dims[m_nrDims-1] != 1) {
                return this; 
            }
            int i = m_nrDims; 
            for (; i-->2; ) if (m_dims[i] != 1) break; 
            int[] newdims = new int[++i]; 
            System.Array.Copy(m_dims,0,newdims,0,i);
            return new ILSize(newdims);
        }

        /// <summary>
        /// Pretty print dimensions in the format "[a,b,c]"
        /// </summary>
        /// <returns>Dimensions as String</returns>
        public override String ToString (){
            String s = "[";
            for (int t = 0; t < m_nrDims; t++) {
                s = s + m_dims[t];
                if (t < m_nrDims -1 ) 
                    s = s + ",";
            }
            s = s + "]"; 
            return s;
        }
        /// <summary>
        /// Generate hash code based on the dimension information 
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode() {
            int ret = m_dims.Length;
            foreach (int i in m_dims) {
                ret = ret * 17 + i; 
            }
            return ret; 
        }
        /// <summary>
        /// Checks for equaltiy of this dimensions to another dimensions object
        /// </summary>
        /// <param name="obj">Dimensions object to compare this instance with</param>
        /// <returns>true, if both dimensions have the same <b>shape</b></returns>
        /// <remarks>This is equivalent to <c>IsSameShape((ILSize)obj)</c>.</remarks>
        public override bool Equals(object obj) {
            if (object.ReferenceEquals(obj, null)) return false;
            ILSize objDim = obj as ILSize;
            if (objDim != null) {
                return IsSameShape(objDim); 
            } else {
                return false; 
            }

        }
        #endregion 

        #region private helpers
        private static int[] convert2int(ILBaseArray[] dimensions) {
            using (ILScope.Enter(dimensions)) {
                if (dimensions == null || dimensions.Length == 0) {
                    return new int[2];
                }
                int[] ret = new int[dimensions.Length];
                if (dimensions.Length > 1) {
                    for (int i = 0; i < ret.Length; i++) {
                        ILBaseArray A = dimensions[i];
                        if (object.Equals(A, null) || !A.IsScalar || !A.IsNumeric) {
                            throw new ILArgumentException("dimension specifier must be numeric scalars");
                        }
                        ret[i] = (int)ILMath.todouble(A).GetValue(0);
                    }
                } else {
                    // if exactly one array was given, it must be a numeric int vector with the dim specification 
                    ILBaseArray A = dimensions[0];
                    if (object.Equals(A, null) || !A.IsVector || !A.IsNumeric) {
                        throw new ILArgumentException("invalid dimension specification"); 
                    }
                    ILArray<int> Aint = ILMath.toint32(A); 
                    Aint.ExportValues(ref ret);  
                }
                return ret;
            }
        }
        #endregion

    }
}
