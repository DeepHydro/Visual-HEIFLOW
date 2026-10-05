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
using ILNumerics.Storage; 
using ILNumerics.Exceptions; 

namespace ILNumerics {

    /// <summary>
    /// Rectangular array, used as output parameter only 
    /// </summary>
    /// <typeparam name="ElementType">Inner type. This will mostly be a system numeric type or a 
    /// complex floating point type.</typeparam>
    /// <remarks><para>This class extends the primary <c>ILArray</c> by optimizing its behavior for the case when used as an
    /// output parameter of functions.</para><para>When writing your own function all <i>output</i> parameters should be of type <c>ILOutArray</c>. Do not
    /// use <c>out ILArray</c>! Similary, all return types should be of type <see cref="ILNumerics.ILRetArray{T}"/> and all input parameters of type <see cref="ILNumerics.ILInArray{T}"/>.</para>
    /// <para>Other than being used to retrieve results from functions, <c>ILOutArray</c> should not be used.</para>
    /// </remarks>
    /// <seealso cref="ILNumerics.ILArray{T}"/>
    /// <seealso cref="ILNumerics.ILRetArray{T}"/>
    /// <seealso cref="ILNumerics.ILInArray{T}"/>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public sealed class ILOutArray<ElementType> : ILDenseArray<ElementType> {

        #region attributes
        private ILArray<ElementType> m_originalArray; 
        private static bool s_isTempArray = false; 
        #endregion

        #region constructors
        /// <summary>
        /// Create new output parameter array
        /// </summary>
        /// <param name="storage">Dense storage for the new array</param>
        internal ILOutArray(ILDenseStorage<ElementType> storage)
            : base(storage, s_isTempArray) {
            //ILScope.Context.RegisterArray(this);
        }
        #endregion

        #region implicit cast operators
        #region constructional operators
        ///// <summary>
        ///// Implicitly convert scalar to array of size 1x1 (scalar).
        ///// </summary>
        ///// <param name="val">System type of size scalar</param>
        ///// <returns>New ILOutArray of type ILOutArray <![CDATA[<typeof(val)>]]> of size 1x1 
        ///// holding the only element with value of val.
        ///// </returns>
        //public static implicit operator ILOutArray<ElementType> (ElementType val) {
        //    ILOutArray<ElementType> ret = new ILOutArray<ElementType>(
        //                new ILDenseStorage<ElementType>(
        //                    new ElementType[1] {val}, 
        //                    new ILSize(1,1)));
        //    return ret; 
        //}
        ///// <summary>
        ///// implicitly cast one dimensional System.Array to ILNumerics array (vector)
        ///// </summary>
        ///// <param name="A">1 dimensional system array, arbitrary type</param>
        ///// <returns>ILNumerics array of same element type as elements of A. 
        ///// Row vector. If A is null: empty array.</returns>
        ///// <remarks>The System.Array A will directly be used for the new ILNumerics array! 
        ///// No copy will be done! Make sure, not to reference A after this conversion!</remarks>
        //public static implicit operator ILOutArray<ElementType> (ElementType[] A) {
        //    if (A == null) {
        //        ILDenseStorage<ElementType> dS = new ILDenseStorage<ElementType>(new ElementType[0], ILSize.Empty00); 
        //        return new ILOutArray<ElementType>(dS);                          
        //    }
        //    return new ILOutArray<ElementType>(A,1,A.Length);  // constructor will register the array in scope context! 
        //}
        ///// <summary>
        ///// implicitly convert n-dimensional System.Array to ILNumerics array
        ///// </summary>
        ///// <param name="A">arbitrarily sized System.Array</param>
        ///// <returns>If A is null: empty array. Else: new ILNumerics array of the same size as A</returns>
        ///// <remarks>The inner type of input array <paramref name="A"/> must match the requested type
        ///// <typeparamref name="ElementType"/>. The resulting ILOutArray will reflect all dimensions of 
        ///// A. Elements of A will get copied to elements of output array (shallow copy).</remarks>
        ///// <exception cref="ILNumerics.Exceptions.ILCastException"> if type of input does not match 
        ///// ElementType</exception>
        //public static implicit operator ILOutArray<ElementType> (Array elements) {
        //    if (elements == null || elements.Length == 0) {
        //        return new ILOutArray<ElementType>(ILSize.Empty00);
        //    }
        //    if (elements.GetType().GetElementType() != typeof(ElementType)) 
        //        throw new ILCastException("inner type of System.Array must match"); 
        //    int [] dims = new int[elements.Rank]; 
        //    ElementType [] retArr = ILMemoryPool.Pool.New<ElementType>(elements.Length);
        //    int posArr = 0; 
        //    for (int i = 0; i < dims.Length; i++) {
        //        dims[i] = elements.GetLength(dims.Length-i-1);
        //    }
        //    foreach (ElementType item in elements) 
        //        retArr[posArr++] = item; 
        //    ILOutArray<ElementType> ret = new ILOutArray<ElementType>(retArr,dims);
        //    return ret; 
        //}
        ///// <summary>
        ///// implicitly cast two dimensional System.Array to ILNumerics array
        ///// </summary>
        ///// <param name="A">2D System.Array</param>
        ///// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        //public static implicit operator ILOutArray<ElementType>(ElementType[,] A) {
        //    if (A == null || A.Length == 0) {
        //        return new ILOutArray<ElementType>(ILSize.Empty00);
        //    }
        //    int[] dims = new int[2];
        //    ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(A.Length);
        //    int posArr = 0;
        //    for (int i = 0; i < 2; i++) {
        //        dims[i] = A.GetLength(dims.Length - i - 1);
        //    }
        //    foreach (ElementType item in A)
        //        retArr[posArr++] = item;
        //    ILOutArray<ElementType> ret = new ILOutArray<ElementType>(retArr, dims);
        //    return ret; 
        //}
        ///// <summary>
        ///// implicitly cast three dimensional System.Array to ILNumerics array
        ///// </summary>
        ///// <param name="A">3D System.Array</param>
        ///// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        //public static implicit operator ILOutArray<ElementType>(ElementType[,,] A) {
        //    if (A == null || A.Length == 0) {
        //        return new ILOutArray<ElementType>(ILSize.Empty00);
        //    }
        //    int[] dims = new int[3];
        //    ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(A.Length);
        //    int posArr = 0;
        //    for (int i = 0; i < 3; i++) {
        //        dims[i] = A.GetLength(dims.Length - i - 1);
        //    }
        //    foreach (ElementType item in A)
        //        retArr[posArr++] = item;
        //    ILOutArray<ElementType> ret = new ILOutArray<ElementType>(retArr, dims);
        //    return ret; 
        //}
        #endregion
        #region conversional operators
        /// <summary>
        /// Creates an output parameter type array from regular array
        /// </summary>
        /// <param name="A">Original array</param>
        /// <returns>Output parameter type array, references the original array</returns>
        public static implicit operator ILOutArray<ElementType>(ILArray<ElementType> A) {
            if (object.Equals(A, null))
                return null;
            // we over take the storage (identical reference) to the new OutArray
            // and store the reference to the original as well.
            // It will be needed to synchronize changes to the denseStorage later (on Remove and Expand).
            ILOutArray<ElementType> ret = new ILOutArray<ElementType>(A.Storage);
            ret.m_originalArray = A; 
            return ret; 
        }
        #endregion
        #endregion

        #region memory management
        /// <summary>
        /// Replace the elements of this array with another array's elements, preventing memory leaks
        /// </summary>
        /// <param name="value">New array</param>
        public ILRetArray<ElementType> a {
            set { Assign(value); }
            get { return this.C; }
        }
        /// <summary>
        /// Replaces storage of this array with new array elements, registers this array for out-of-scope disposal
        /// </summary>
        /// <param name="value">New array</param>
        public void Assign(ILRetArray<ElementType> value) {
            if (!IsDisposed)
                Storage.Dispose();
            ILDenseStorage<ElementType> storage = value.GiveStorageAwayOrClone();
            m_storage = storage; 
            if (!object.ReferenceEquals(m_originalArray, null)) {
                // update original array as well
                (m_originalArray as ILDenseArray<ElementType>).Storage = storage; 
            }
            //ILScope.Context.RegisterArray(this);  
        }
        internal override bool EnterScope() {
            return false;
        }
        /// <summary>
        /// Direct reference to inner System.Array storage for write access - use with care!
        /// </summary>
        /// <returns>Reference to inner System.Array</returns>
        /// <remarks>Altering this array can be done directly. If necessary, the array is detached before 
        /// returned. Watch the column order format of storages in ILNumerics. Keep in mind, the length 
        /// of the System.Array may exceed the number of elements of the ILNumerics array.
        /// <para>Accessing the inner system array directly should be left to ILNumerics experts only. 
        /// Unless you really know, what you are doing, you should rather use the higher order access 
        /// methods provided by ILArray&lt;T>!</para>
        /// <para>For elements of reference types (e.g. ILCell), retrieving and storing elements from/into the System.Array directly 
        /// obviously does not simulate a value semantic as all other common API methods do! This means, references are copied. Attention
        /// must be paid to dereference / clone elements accordingly or (recommended) prevent from using this 
        /// function with reference element types.</para></remarks>
        public ElementType[] GetArrayForWrite() {
            return Storage.GetArrayForWrite();
        }

        #endregion

        #region mutability + indexer
        /// <summary>
        /// Set single value to element at index specified
        /// </summary>
        /// <param name="value">New value</param>
        /// <param name="idx">Index of element to be altered</param>
        public void SetValue(ElementType value, params int[] idx) {
            Storage.SetValueTyped(value, idx);
        }

        /// <summary>
        /// Alter range of this array
        /// </summary>
        /// <param name="value">Array with new values</param>
        /// <param name="range">Range specification</param>
        public void SetRange(ILInArray<ElementType> value, params ILBaseArray[] range) {
            using (ILScope.Enter(value))
            using (ILScope.Enter(range)) {
                if (object.Equals(value, null)) {
                    Storage.IndexSubrange(null, range);
                } else {
                    using (ILScope.Enter(value))
                        Storage.IndexSubrange(value.Storage, range);
                }
            }
        }
        /// <summary>
        /// Subarray creation/manipulation/deletion
        /// </summary>
        /// <param name="range">Range specification, defining the size of the subarray</param>
        /// <returns>Subarray as copy of this array</returns>
        public ILRetArray<ElementType> this[params ILBaseArray[] range] {
            get {
                using (ILScope.Enter(range))
                    return new ILRetArray<ElementType>(Storage.Subarray(range));
            }
            set {
                SetRange(value, range);
            }
        }

        #endregion
    }
}
