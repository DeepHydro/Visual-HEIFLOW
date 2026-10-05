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
using ILNumerics;
using ILNumerics.Misc;
using ILNumerics.Exceptions; 
using ILNumerics.Storage;
using ILNumerics.Native;
using System.Text; 

namespace ILNumerics {
	/// <summary>
	/// Boolean array for high performance relational operations on arbitrary arrays 
	/// </summary>
	/// <remarks>
	/// Logical arrays are derived from <![CDATA[ILArray<byte>]]>. It consumes
	/// 1 byte per element and is the output parameter of all relational comparisons
	/// as well as the input parameter for all functions consuming <![CDATA[ILArray<byte>]]>.
    /// The difference between <![CDATA[<byte>]]> and an ILLogical is, the ILLogical 
    /// storing a integer value with the number of nonzero elements as additional information. 
    /// Therefore functions like 'find' are able to determine the lenght of output array to 
    /// be created omitting the need of multiple walks through the array. Therefore ILLogicalArrays 
    /// consume (a little) more time while construction but are much more performand on functions like 
    /// 'find'.
	/// </remarks>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public class ILBaseLogical : ILDenseArray<byte> {

        #region properties 
        /// <summary>
        /// Number of 'true' elements in this array
        /// </summary>
        /// <remarks>This value caches the number of 'true' elements in this logical array. 
        /// It may be used for information purposes but is actually needed internally for performance 
        /// reasons.</remarks>
        public virtual long NumberNonZero {
            get {
                return Storage.NumberNonZero;
            }
            internal set {
                Storage.NumberNonZero = value; 
            }
        }
        /// <summary>
        /// Shift the dimensions of this array by one (transpose for matrix)
        /// </summary>
        public new ILRetLogical T {
            get {
                return new ILRetLogical((ILLogicalStorage)Storage.ShiftDimensions(1), NumberNonZero);
            }
        }
        internal new ILLogicalStorage Storage {
            get {
                return (m_storage as ILLogicalStorage);
            }
        }
        /// <summary>
        /// Create clone of this array 
        /// </summary>
        public new ILRetLogical C {
            get {
                return new ILRetLogical((ILLogicalStorage)base.Clone().Storage, NumberNonZero);
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Constructor creating ILLogical from dense storage
        /// </summary>
        /// <param name="A">Input array, the storage of this ILArray will directly be used for 
        /// storage of the new ILLogical</param>
        /// <param name="isTempArray">Indicate whether the result is supposed to be a temporary array (true) or persistent (false)</param>
		internal ILBaseLogical(ILDenseStorage<byte> A, bool isTempArray)
			: base(A,isTempArray) {
			NumberNonZero = sumElements();
		}
        #endregion

        #region helper functions
        /// <summary>
        /// Sum all elements of this storage. 
        /// </summary>
        /// <returns>Number of non zero elements</returns>
        protected int sumElements() {
            int ret = 0; 
            int nrElements = Storage.Size.NumberOfElements; 
            // physical storage 
            unsafe {
                fixed (byte* pInArray = Storage.GetArrayForRead()) {
                    byte* pCurData = pInArray;
                    byte* pLastElement = pInArray + nrElements;
                    while (pCurData < pLastElement)
                        ret += *pCurData++;
                }
            }
            return ret; 
        }
        #endregion
 
        #region public functions
        /// <summary>
        /// Concatenate this array
        /// </summary>
        /// <param name="A">N-dimensional array. Except for dimensions <paramref name="dim"/>
        /// the dimensions of A must match the dimensions of this storage</param>
        /// <param name="dim">Index of dimension to concatenate arrays along.
        /// If dim is larger than the number of dimensions of any of the arrays,
        /// its value will be used in modulus the number of dimensions.</param>
        /// <returns>New array having the size 
        /// of both input arrays layed behind each other along the dim's-dimension</returns>
        public ILRetLogical Concat(ILInLogical A, int dim) {
            using (ILScope.Enter(A))
            return new ILRetLogical ((ILLogicalStorage)Storage.Concat(A.Storage, dim));
        }
        /// <summary>
        /// Create reshaped copy of this logical array
        /// </summary>
        /// <param name="size">New dimensions of the array</param>
        /// <returns>Reshaped copy of this array</returns>
        /// <remarks><para>The current instance will not be changed! A new array is created, having 
        /// the elements of this array and a shape as determined by <paramref name="size"/>.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the number of elements in 
        /// <paramref name="size"/> do not match the number of elements in this array.</exception>
        public new ILRetLogical Reshape(ILSize size) {
            using (ILScope.Enter()) {
                ILLogical ret = C;
                ret.Storage.Reshape(size);
                return ret;
            }
        }
        /// <summary>
        /// Create reshaped copy of this logical array
        /// </summary>
        /// <param name="size">New dimensions of the array</param>
        /// <returns>Reshaped copy of the array</returns>
        /// <remarks><para>The current instance will not be changed! A new array is created, having 
        /// the elements of this array and a shape as determined by <paramref name="size"/>.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the number of elements in 
        /// <paramref name="size"/> do not match the number of elements in this array.</exception>
        public new ILRetLogical Reshape(params int[] size) {
            return Reshape(new ILSize(size));
        }
        /// <summary>
        /// Create replication of this array
        /// </summary>
        /// <param name="dims">Dimensions specifier. If the number of elements in <paramref name="dims"/> is 
        /// less than the number of dimensions in this array, the trailing dimensions will 
        /// be set to 1 (singleton dimensions). On the other hand, if the number specified 
        /// is larger then the number of dimension stored inside the storge the resulting 
        /// storage will get its number of dimensions extended accordingly. </param>
        /// <returns>array being created out of multiple replications of this array along 
        /// arbitrary dimensions according to <paramref name="dims"/></returns>
        public new ILRetLogical Repmat(params int[] dims) {
            return new ILRetLogical((ILLogicalStorage)Storage.Repmat(dims));
        }
        /// <summary>
        /// Create logical array from this logical and shift dimensions
        /// </summary>
        /// <param name="shift">Number of dimensions to shift</param>
        /// <returns>Shifted version of this array</returns>
        /// <remarks><para>The shift is done 'to the left':</para>
        /// <example><code>ILArray&lt;double> A = zeros(2,4); 
        /// ILArray&lt;double> B = A.Shifted(1); 
        /// // B is now: &lt;double> [4,2] 
        /// 
        /// ILArray&lt;double> C = zeros(2,4,3);
        /// ILArray&lt;double> D = C.Shifted(1); 
        /// // D is now: &lt;double> [4,3,2] 
        /// </code></example>
        /// <para>The dimensions are shifted circulary to the left. This 
        /// can be imagined as removing the first dimensions from the beginning of the list of 
        /// dimensions and "append" them to the end in a ringbuffer style.</para>
        /// <para>For dimension shifts of '1', you may consider using the 
        /// <see cref="ILNumerics.ILDenseArray{ElementType}.T"/> property for readability.</para>
        /// <para><paramref name="shift"/> must be positive. It is taken modulus the number of dimensions.</para>
        /// <seealso cref="ILNumerics.ILDenseArray{ElementType}.T"/></remarks>
        public new ILRetLogical Shifted(int shift) {
            return new ILRetLogical((ILLogicalStorage)Storage.ShiftDimensions(shift));
        }
        /// <summary>
        ///	Subarray from this array
        /// </summary>
        /// <param name="range">Arrays specifying the ranges to create subarray from</param>
        /// <returns>Subarray as specified</returns>
        public new ILRetLogical Subarray(params ILBaseArray[] range) {
            using (ILScope.Enter(range))
            return new ILRetLogical((ILLogicalStorage)Storage.Subarray(range));
        }
        /// <summary>
        /// Short summary of this logical array
        /// </summary>
        /// <returns>Type and size information</returns>
        public override string ShortInfo() {
            string ret = "Logical ";
            if (object.Equals(m_storage,null)) 
                return ret + " (disposed)"; 
            if (Storage.Size.NumberOfElements == 1)
                ret += Storage.GetValue(0);
            else
                ret += Storage.Size.ToString();
            return ret.ToString();
        }
        #endregion

        #region depricated 
        /// <summary>
        /// [deprecated] create empty ILLogical 
        /// </summary>
        /// <returns>empty ILLogical.</returns>
        [Obsolete()]
        public static ILRetLogical empty(ILSize dim) {
            return new ILRetLogical(new byte[0],dim);
        }
        #endregion

    }
}
