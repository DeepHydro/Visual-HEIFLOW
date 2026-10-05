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
    /// Logical arrays are derived from <c><![CDATA[ILArray<byte>]]></c>. It consumes
    /// 1 byte per element and is the output parameter of all relational comparisons
    /// as well as the input parameter for all functions consuming <c><![CDATA[ILArray<byte>]]></c>.
    /// The difference between arrays and logical arrays is: the logical array 
    /// stores a integer value with the number of nonzero elements as additional information. 
    /// Therefore functions like 'find' are able to determine the lenght of output array to 
    /// be created omitting the need of multiple walks through the array. Therefore ILLogicalArrays 
    /// consume (a little) more time while construction but are much more performand on functions like 
    /// 'find'.
    /// </remarks>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public sealed class ILLogical : ILBaseLogical {
        
        private static readonly bool s_isTempArray = false;

        #region constructors
        /// <summary>
        ///	constructor - create logical array of type <c>Byte</c> of specified size
        /// </summary>
        /// <param name="size">
        /// Variable length int array specifying the number and size of dimensions to 
        /// be created.
        /// </param>
        /// <remarks>
        /// The size parameter may not be null or an empty array! An Exception will be 
        /// thrown in this case. The dimensions will be trimmed before processing 
        /// (removing trailing non singleton dimensions). 
        /// Depending on the requested size an ILArray &lt; byte &gt; of the specified dimensions 
        /// will be created. The type of storage will be <c>bool</c>.
        /// </remarks>
        internal ILLogical(params int[] size)
            : base(new ILLogicalStorage(new ILSize(size)), s_isTempArray) {
            NumberNonZero = sumElements();
        }
        /// <summary>
        ///	constructor - create logical array of type <c>Byte</c> of specified size
        /// </summary>
        /// <param name="size">
        /// dimension object
        /// </param>
        /// <remarks>
        /// The size parameter may not be null. An Exception will be 
        /// thrown in this case. The dimensions will be trimmed before processing 
        /// (removing trailing singleton dimensions). 
        /// Depending on the requested size an logical array of the specified dimensions 
        /// will be created. The element type is be <c>bool</c>.
        /// </remarks>
        internal ILLogical(ILSize size)
            : base(new ILLogicalStorage(size), s_isTempArray) {
            NumberNonZero = sumElements();
        }
        /// <summary>
        /// Constructor creating logical array from dense storage
        /// </summary>
        /// <param name="A">input array, the storage of this ILArray will directly be used for 
        /// storage of the new logical array</param>
        internal ILLogical(ILLogicalStorage A)
            : base(A, s_isTempArray) {
            NumberNonZero = sumElements();
        }
        /// <summary>
        /// Constructor creating logical array from (dense) storage
        /// </summary>
        /// <param name="A">input array, the storage of this ILArray will directly be used for 
        /// storage of the new logical array</param>
        /// <param name="numberNonZero">number of nonzero elements in A. Must be positive or 0.</param>
        /// <remarks> Providing this parameter prevents the constructor from having to count the 
        /// 'true' elements in A.</remarks>
        internal ILLogical(ILLogicalStorage A, long numberNonZero)
            : base(A, s_isTempArray) {
            if (numberNonZero < 0)
                throw new ILNumerics.Exceptions.ILArgumentException("invalid number of non-zero-elements given!");
            NumberNonZero = numberNonZero;
        }
        /// <summary>
        ///	constructor - create logical array of specified size 
        /// from data array
        /// </summary>
        /// <param name="size">
        /// Variable length int array specifying the number and size of dimensions to 
        /// be created.
        /// </param>
        /// <param name="data"> byte array matching the size of the dimensions 
        /// specified. The data will directly be used as storage! No copy will be made!</param>
        /// <remarks>
        /// The size parameter may not be null or an empty array! An Exception will be 
        /// thrown in this case. The dimensions will be trimmed before processing 
        /// (removing trailing non singleton dimensions). 
        /// Depending on the requested size an logical array of the specified size 
        /// will be created. The type of storage will be <c>byte</c>.
        /// </remarks>
        internal ILLogical(byte[] data, params int[] size)
            : base(new ILLogicalStorage(data, new ILSize(size)), s_isTempArray) {
            NumberNonZero = sumElements();
        }
        /// <summary>
        /// Constructor creating logical array, provide predefined storage
        /// </summary>
        /// <param name="data">predefined storage elements. The array will directly be used 
        /// as underlying storage. No copy will be made! </param>
        /// <param name="dimension">Dimensions specification.</param>
        internal ILLogical(byte[] data, ILSize dimension)
            : base(new ILLogicalStorage(data, dimension), s_isTempArray) {
           NumberNonZero = sumElements();
        }
        /// <summary>
        /// Constructor creating logical array, predefined storage (fast version)
        /// </summary>
        /// <param name="data">predefined storage elements. The array will directly be used 
        /// as underlying storage. No copy will be made! </param>
        /// <param name="dimension">Dimensions specification.</param>
        /// <param name="nonZeroCount">number of nonzero elements in <paramref name="data"/>. 
        /// Providing this parameter prevents from counting the 'true' elements (again). </param>
        internal ILLogical(byte[] data, ILSize dimension, long nonZeroCount)
            : base(new ILLogicalStorage(data, dimension), s_isTempArray) {
            if (nonZeroCount < 0)
                throw new ILNumerics.Exceptions.ILArgumentException("invalid number of non-zero-elements given!");
            NumberNonZero = nonZeroCount;
        }
        #endregion

        #region operator overloading
        #region constructional operators
        /// <summary>
        /// Implicitly convert boolean scalar to logical array of size 1x1 (scalar).
        /// </summary>
        /// <param name="val">Boolean scalar</param>
        /// <returns>New logical array of size 1x1 holding the only element of type Byte 
        /// with value of val.</returns>
        public static implicit operator ILLogical(bool val) {
            ILLogical ret = new ILLogical(new byte[1] { val ? (byte)1 : (byte)0 }, 1, 1);
            return ret;
        }
        /// <summary>
        /// Implicitly convert logical array to bool
        /// </summary>
        /// <param name="A">logical array</param>
        /// <returns>true if <b>all</b> elements of A are non-zero, false otherwise 
        /// </returns>
        /// <remarks> If A is null or empty, the function returns false. Otherwise allall returns true, 
        /// if all elements of A are non-zero and returns false, if A contains any zero elements.</remarks>
        public static implicit operator bool(ILLogical A) {
            // this operator is implicit for convenience reasons: 
            // if(tmp[0]!=-10.0) { ... is only possible this way
            if (object.Equals(A, null) || A.IsEmpty)
                return false;
            if (A.IsScalar)
                return A.GetValue(0, 0) == 1;
            return ILMath.allall(A).GetValue(0) == 1;
        }
        /// <summary>
        /// Implicitly convert integer scalar to logical array of size 1x1 (scalar).
        /// </summary>
        /// <param name="val">Scalar value</param>
        /// <returns>New logical array of size 1x1 holding the only element of type Byte 
        /// with value of val.</returns>
        public static implicit operator ILLogical(int val) {
            ILLogical ret = new ILLogical(new Byte[1] { 
                                 val != 0 ? (byte)1:(byte)0 }, 1, 1);
            return ret;
        }
        /// <summary>
        /// Implicitly cast one dimensional System.Array to ILNumerics array (vector)
        /// </summary>
        /// <param name="A">1-dimensional system array, arbitrary type</param>
        /// <returns>ILNumerics array of same element type as elements of A. 
        /// Row vector. If A is null: empty array.</returns>
        /// <remarks>The System.Array A will directly be used for the new ILNumerics array! 
        /// No copy will be done! Make sure, not to reference A after this conversion!</remarks>
        public static implicit operator ILLogical(byte[] A) {
            if (A == null) {
                ILLogicalStorage dS = new ILLogicalStorage(new byte[0], ILSize.Empty00);
                return new ILLogical(dS);
            }
            return new ILLogical(A, 1, A.Length);
        }
        /// <summary>
        /// Implicitly convert n-dim. System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">Arbitrarily sized System.Array</param>
        /// <returns>If A is null: empty array. Else: new ILNumerics array of the same size as A</returns>
        /// <remarks>The inner type of input array <paramref name="A"/> must match the requested type
        /// <typeparamref name="ElementType"/>. The resulting ILArray will reflect all dimensions of 
        /// A. Elements of A will get copied to elements of output array (shallow copy).</remarks>
        /// <exception cref="ILNumerics.Exceptions.ILCastException"> if type of input does not match 
        /// ElementType</exception>
        public static implicit operator ILLogical(Array elements) {
            if (elements == null || elements.Length == 0) {
                return new ILLogical(ILSize.Empty00);
            }
            if (elements.GetType().GetElementType() != typeof(byte))
                throw new ILCastException("inner type of System.Array must match");
            int[] dims = new int[elements.Rank];
            byte[] retArr = ILMemoryPool.Pool.New<byte>(elements.Length);
            int posArr = 0;
            for (int i = 0; i < dims.Length; i++) {
                dims[i] = elements.GetLength(dims.Length - i - 1);
            }
            foreach (byte item in elements)
                retArr[posArr++] = item;
            return new ILLogical(retArr, dims);
        }
        /// <summary>
        /// Implicitly cast two dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">2D System.Array</param>
        /// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        public static implicit operator ILLogical(byte[,] A) {
            if (A == null || A.Length == 0) {
                return new ILLogical(ILSize.Empty00);
            }
            int[] dims = new int[2];
            byte[] retArr = ILMemoryPool.Pool.New<byte>(A.Length);
            int posArr = 0;
            for (int i = 0; i < 2; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (byte item in A)
                retArr[posArr++] = item;
            return new ILLogical(retArr, dims);
        }
        /// <summary>
        /// Implicitly cast three dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">3-dimensional System.Array</param>
        /// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        public static implicit operator ILLogical(byte[, ,] A) {
            if (A == null || A.Length == 0) {
                return new ILLogical(ILSize.Empty00);
            }
            int[] dims = new int[3];
            byte[] retArr = ILMemoryPool.Pool.New<byte>(A.Length);
            int posArr = 0;
            for (int i = 0; i < 3; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (byte item in A)
                retArr[posArr++] = item;
            return new ILLogical(retArr, dims);
        }
        #endregion

        #region operational operators
        /// <summary>
        /// Invert values of array elements 
        /// </summary>
        /// <param name="in1">Input array</param>
        /// <returns>New solid logical array, inverted element values</returns>
        public static ILRetLogical operator !(ILLogical in1) {
            if (object.Equals(in1, null))
                throw new ILArgumentException("operator -(): parameter must not be null!");
            return (in1 != (byte)1);
        }
        #endregion

        #region conversional operators
        public static implicit operator ILLogical(ILRetLogical A) {
            if (object.Equals(A, null))
                return null;
            ILLogicalStorage aStorage = (ILLogicalStorage)A.GiveStorageAwayOrClone(); 
            ILLogical ret = new ILLogical(aStorage, aStorage.NumberNonZero);
            ILScope.Context.RegisterArray(ret);
            return ret;
        }
        public static implicit operator ILLogical(ILInLogical A) {
            if (object.Equals(A, null))
                return null;
            ILLogicalStorage storage = A.Storage; 
            ILLogical ret = new ILLogical(
                    new ILLogicalStorage(storage.GetDataArray(), storage.Size));
            ILScope.Context.RegisterArray(ret);
            return ret;
        }
        public static implicit operator ILLogical(ILArray<byte> A) {
            if (object.Equals(A, null))
                return null;
            ILLogical ret = new ILLogical(
                new ILLogicalStorage(A.Storage.GetDataArray(), A.Size));
            ILScope.Context.RegisterArray(ret);
            return ret;
        }
        public static implicit operator ILLogical(ILInArray<byte> A) {
            if (object.Equals(A, null))
                return null;
            ILLogical ret = new ILLogical(
                new ILLogicalStorage(A.Storage.GetDataArray(), A.Size));
            ILScope.Context.RegisterArray(ret);
            return ret;
        }

        public static implicit operator ILArray<byte>(ILLogical A) {
            if (object.Equals(A, null))
                return null;
            ILArray<byte> ret = new ILArray<byte>(
                new ILDenseStorage<byte>(A.Storage.GetDataArray(), A.Size)); 
            ILScope.Context.RegisterArray(ret);
            return ret; 
        }
        public static implicit operator ILInArray<byte>(ILLogical A) {
            if (object.Equals(A,null)) 
                return null; 
            ILInArray<byte> ret = new ILInArray<byte>(
                new ILDenseStorage<byte>(A.Storage.GetDataArray(), A.Size)); 
            return ret;
        }
        #endregion

        #endregion operator overloads 

        #region index access + mutability
        /// <summary>
        /// Subarray access
        /// </summary>
        /// <param name="range">Range specification</param>
        /// <returns>Reference pointing to the elements of this array specified by range. If used for removal: 
        /// the array will be changed to a referencing array having the parts requested removed and reshaped accordingly.</returns>
        /// <remarks>Query access: for N-dimensional arrays trailing dimensions will be choosen to be 0. Therefore you 
        /// may ommit those trailing dimensions in range.
        /// <para>The indexer may be used for querying or altering single/any elements 
        /// in this array. <c>range</c> may contains index specifications for one ... any 
        /// dimension. The array returned will have the size specified by range.</para>
        /// <para>The indexer may also be used for removing parts of the array. Therefore an empty array 
        /// (of the same type) or 'null' must be assigned to the range specified by <c>range</c> using the set-access. <c>range</c> 
        /// must contain exactly one dimension specification other than null. This may be any vector-sized numeric ILArray of any 
        /// type. If <c>range</c> applies 
        /// to less dimensions than dimensions existing in the array, the upper dimensions will be 
        /// merged and the array will be reshaped before applying the removal to it.</para>
        /// <para>In case of removal the ILArray returned will be a reference array.</para></remarks>
        public ILRetLogical this[params ILBaseArray[] dims] {
            get {
                return new ILRetLogical((ILLogicalStorage)Storage.Subarray(dims));
            }
            set {
                SetRange(value,dims); 
                NumberNonZero = sumElements();
            }
        }
        /// <summary>
        /// Set single value to element at index specified
        /// </summary>
        /// <param name="value">New value</param>
        /// <param name="idx">Index of element to be altered</param>
        public void SetValue(byte value, params int[] idx) {
            Storage.SetValueTyped(value, idx);
        }
        /// <summary>
        /// Alter a range of this array
        /// </summary>
        /// <param name="value">Array with new values</param>
        /// <param name="range">Range specification</param>
        public void SetRange(ILInLogical value, params ILBaseArray[] range) {
            using (ILScope.Enter(value))
            using (ILScope.Enter(range)) {
                if (object.Equals(value, null)) {
                    Storage.IndexSubrange(null, range); 
                } else {
                    Storage.IndexSubrange(value.Storage, range);
                }
            }
        }
        /// <summary>
        /// Replace the elements of this array with another array's elements, preventing memory leaks
        /// </summary>
        /// <param name="value">New array</param>
        public ILRetLogical a {
            set { Assign(value); }
            get { return this.C; }
        }
        /// <summary>
        /// Replaces storage of this array with new array elements, registers this array for out-of-scope disposal
        /// </summary>
        /// <param name="value">New array</param>
        public void Assign(ILRetLogical value) { 
            if (!IsDisposed)
                Storage.Dispose();
            m_storage = value.GiveStorageAwayOrClone();
            //ILScope.Context.RegisterArray(this);  
        }
        #endregion

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
        /// <para>Unlike (almost) all other member function of an array, this function
        /// does not keep track of internal memory management. It means, the storage which this array is based 
        /// upon, will not be set free after the function returns. You (as the user of the array) will have to pay 
        /// attention yourself, when to call dispose on the array - if necessary. Also, for elements of reference
        /// types (e.g. ILCell), retrieving and storing elements from/into the System.Array directly does 
        /// not simulate a value semantic as all other functions do! This means, references are copied. Attention
        /// must be paid to dereference / clone elements accordingly. </para></remarks>
        public byte[] GetArrayForWrite() {
            return Storage.GetArrayForWrite();
        }

        #region memory management
        internal override bool EnterScope() {
            return false;
        }
        #endregion

    }
}
