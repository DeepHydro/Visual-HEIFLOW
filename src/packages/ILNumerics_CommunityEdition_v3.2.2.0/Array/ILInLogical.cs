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
    /// The difference between <![CDATA[ILArray<byte>]]> and logical arrays is, the logical array 
    /// storing a integer value with the number of nonzero elements as additional information. 
    /// Therefore functions like 'find' are able to determine the lenght of output array to 
    /// be created omitting the need of multiple walks through the array. Therefore ILLogicalArrays 
    /// consume (a little) more time while construction but are much more performand on functions like 
    /// 'find'.
    /// </remarks>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public sealed class ILInLogical : ILBaseLogical {

        private static readonly bool s_isTempArray = true;

        #region constructors
        /// <summary>
        ///	constructor - create logical array of specified size
        /// </summary>
        /// <param name="size">
        /// Variable length int array specifying the number and size of dimensions to 
        /// be created.
        /// </param>
        /// <remarks>
        /// The size parameter may not be null or an empty array! An exception will be 
        /// thrown in this case. The dimensions will be trimmed before processing 
        /// (removing trailing non singleton dimensions). 
        /// </remarks>
        internal ILInLogical(params int[] size)
            : base(new ILLogicalStorage(new ILSize(size)), s_isTempArray) {
            NumberNonZero = sumElements();
        }
        /// <summary>
        ///	constructor - create logical array of specified size
        /// </summary>
        /// <param name="size">
        /// dimension object
        /// </param>
        /// <remarks>
        /// The size parameter may not be null. An exception will be 
        /// thrown in this case. The dimensions will be trimmed before processing 
        /// (removing trailing singleton dimensions). 
        /// </remarks>
        internal ILInLogical(ILSize size)
            : base(new ILLogicalStorage(size), s_isTempArray) {
            NumberNonZero = sumElements();
        }
        /// <summary>
        /// Constructor creating logical array from dense storage
        /// </summary>
        /// <param name="A">input array, the storage of this ILArray will directly be used for 
        /// storage of the new logical array</param>
        internal ILInLogical(ILLogicalStorage A)
            : base(A, s_isTempArray) {
            NumberNonZero = sumElements();
        }
        /// <summary>
        /// Constructor creating logical array from (dense) storage
        /// </summary>
        /// <param name="A">input array storage, the storage will directly be used for 
        /// storage of the new logical array</param>
        /// <param name="numberNonZero">number of nonzero elements in A. Must be positive or 0.</param>
        /// <remarks> Providing this parameter prevents the constructor from having to count the 
        /// 'true' elements in A.</remarks>
        internal ILInLogical(ILLogicalStorage A, long numberNonZero)
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
        /// </remarks>
        internal ILInLogical(byte[] data, params int[] size)
            : base(new ILLogicalStorage(data, new ILSize(size)), s_isTempArray) {
            NumberNonZero = sumElements();
        }
        /// <summary>
        /// Constructor creating logical array, provide predefined storage
        /// </summary>
        /// <param name="data">predefined storage elements. The array will directly be used 
        /// as underlying storage. No copy will be made! </param>
        /// <param name="dimension">Dimensions specification.</param>
        internal ILInLogical(byte[] data, ILSize dimension)
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
        internal ILInLogical(byte[] data, ILSize dimension, long nonZeroCount)
            : base(new ILLogicalStorage(data, dimension), s_isTempArray) {
            if (nonZeroCount < 0)
                throw new ILNumerics.Exceptions.ILArgumentException("invalid number of non-zero-elements given!");
            NumberNonZero = nonZeroCount;
        }
        #endregion

        #region operator overloading

        #region operational operators
        /// <summary>
        /// Invert values of array elements 
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>New logical array, having the element values of A inverted</returns>
        public static ILRetLogical operator !(ILInLogical A) {
            if (object.Equals(A, null))
                throw new ILArgumentException("operator !: array must not be null");
            return (A != (byte)1);
        }
        #endregion

        #region conversional operators 
        public static implicit operator ILInLogical(ILArray<byte> a) {
            if (object.Equals(a, null))
                return null;
            ILInLogical ret = new ILInLogical(new ILLogicalStorage(
                a.Storage.GetDataArray(), a.Size));
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret; 
        }
        public static implicit operator ILInLogical(ILInArray<byte> a) {
            if (object.Equals(a, null))
                return null;
            ILInLogical ret = new ILInLogical(new ILLogicalStorage(
                a.Storage.GetDataArray(), a.Size));
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret; 
        }
        public static implicit operator ILArray<byte>(ILInLogical a) {
            if (object.Equals(a, null))
                return null;
            ILArray<byte> ret = new ILArray<byte>(
                new ILDenseStorage<byte>(a.Storage.GetDataArray(), a.Size));
            ILScope.Context.RegisterArray(ret);
            return ret;
        }
        public static implicit operator ILInArray<byte>(ILInLogical a) {
            if (object.Equals(a, null))
                return null;
            ILInArray<byte> ret = new ILInArray<byte>(
                new ILDenseStorage<byte>(a.Storage.GetDataArray(), a.Size));
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret;
        }
        public static implicit operator ILInLogical(ILRetLogical a) {
            if (object.Equals(a,null))
                return null;
            ILLogicalStorage storage = (ILLogicalStorage)a.GiveStorageAwayOrClone();
            ILInLogical ret = new ILInLogical(storage, storage.NumberNonZero);
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret;
        }
        public static implicit operator ILInLogical(ILLogical a) {
            if (object.Equals(a, null))
                return null;
            ILInLogical ret = new ILInLogical(new ILLogicalStorage(a.Storage.GetDataArray(), a.Size));
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret;
        }
        #endregion

        #region constructional operators 
        /// <summary>
        /// Implicitly convert logical array to bool
        /// </summary>
        /// <param name="A">logical array</param>
        /// <returns>true if <b>all</b> elements of A are non-zero, false otherwise 
        /// </returns>
        /// <remarks>If A is null or empty, the function returns false. Otherwise allall returns true, 
        /// if all elements of A are non-zero and returns false, if A contains any zero elements.</remarks>
        public static implicit operator bool(ILInLogical A) {
            // this operator is implicit for convenience reasons: 
            // if(tmp[0]!=-10.0) { ... is only possible this way
            if (object.Equals(A, null) || A.IsEmpty)
                return false;
            if (A.IsScalar)
                return A.GetValue(0, 0) == 1;
            if (Settings.LogicalArrayToBoolConversion == LogicalConversionMode.ImplicitAllAll) {
                if (A.Size.NumberOfElements > 0) {
                    return ILMath.allall(A).GetValue(0) == 1;
                } else {
                    return false; 
                }
            } else { // if (Settings.LogicalArrayToBoolConversion == LogicalConversionMode.NonScalarThrowsException) {
                if (!A.IsScalar) {
                    throw new ILArgumentException("error while attempting to convert logical of size " + A.Size.ToString() + " to boolean");
                } else {
                    return A.GetValue(0) == 1; 
                }
            }
        }

        /// <summary>
        /// Implicitly convert scalar byte to logical array of size 1x1 (scalar).
        /// </summary>
        /// <param name="val">Byte scalar</param>
        /// <returns>New logical array of size 1x1 holding the only element of type <c>byte</c>
        /// with value of val.</returns>
        public static implicit operator ILInLogical(bool val) {
            ILInLogical ret = new ILInLogical(new byte[1] { val ? (byte)1 : (byte)0 }, 1, 1);
            return ret;
        }

        /// <summary>
        /// Implicitly convert integer scalar to logical array of size 1x1 (scalar).
        /// </summary>
        /// <param name="val">Scalar value</param>
        /// <returns>New logical array of size 1x1 holding the only element of type Byte 
        /// with value of val.</returns>
        public static implicit operator ILInLogical(int val) {
            ILInLogical ret = new ILInLogical(new Byte[1] { 
                                 val != 0 ? (byte)1:(byte)0 }, 1, 1);
            return ret;
        }
        /// <summary>
        /// implicitly cast one dimensional System.Array to ILNumerics array (vector)
        /// </summary>
        /// <param name="A">1 dimensional system array, arbitrary type</param>
        /// <returns>ILNumerics array of same element type as elements of A. 
        /// Row vector. If A is null: empty array.</returns>
        /// <remarks>The System.Array A will directly be used for the new ILNumerics array! 
        /// No copy will be done! Make sure, not to reference A after this conversion!</remarks>
        public static implicit operator ILInLogical(byte[] A) {
            if (A == null) {
                ILLogicalStorage dS = new ILLogicalStorage(new byte[0], ILSize.Empty00);
                return new ILInLogical(dS);
            }
            ILInLogical ret = new ILInLogical(A, 1, A.Length);
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret;
        }
        /// <summary>
        /// Implicitly convert n-dim. System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">Arbitrarily sized System.Array</param>
        /// <returns>If A is null: empty array. Else: new ILNumerics array of the same size as A</returns>
        /// <remarks>The inner type of input array <paramref name="A"/> must match the requested type
        /// <typeparamref name="ElementType"/>. The resulting ILArray will reflect all dimensions of 
        /// A. Elements of A will get copied to elements of output array (shallow copy).</remarks>
        /// <exception cref="ILNumerics.Exceptions.ILCastException">If type of input does not match 
        /// ElementType</exception>
        public static implicit operator ILInLogical(Array elements) {
            if (elements == null || elements.Length == 0) {
                return new ILInLogical(ILSize.Empty00);
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
            ILInLogical ret = new ILInLogical(retArr, dims);
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret; 
        }
        /// <summary>
        /// Implicitly cast two dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">2-dimensional System.Array</param>
        /// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        public static implicit operator ILInLogical(byte[,] A) {
            if (A == null || A.Length == 0) {
                return new ILInLogical(ILSize.Empty00);
            }
            int[] dims = new int[2];
            byte[] retArr = ILMemoryPool.Pool.New<byte>(A.Length);
            int posArr = 0;
            for (int i = 0; i < 2; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (byte item in A)
                retArr[posArr++] = item;
            ILInLogical ret = new ILInLogical(retArr, dims);
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret;
        }
        /// <summary>
        /// Implicitly cast three dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">3-dimensional System.Array</param>
        /// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        public static implicit operator ILInLogical(byte[, ,] A) {
            if (A == null || A.Length == 0) {
                return new ILInLogical(ILSize.Empty00);
            }
            int[] dims = new int[3];
            byte[] retArr = ILMemoryPool.Pool.New<byte>(A.Length);
            int posArr = 0;
            for (int i = 0; i < 3; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (byte item in A)
                retArr[posArr++] = item;
            ILInLogical ret = new ILInLogical(retArr, dims);
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret;
        }
        #endregion
        #endregion

        #region index access + mutability
        /// <summary>
        /// Subarray access (readonly)
        /// </summary>
        /// <param name="range">Range specification</param>
        /// <returns>Logical array with the elements specified by range</returns>
        /// <remarks>Query access: for N-dimensional arrays trailing dimensions will be choosen to be 0. Therefore you 
        /// may ommit those trailing dimensions in range.
        /// <para>The indexer may be used for querying any elements 
        /// in this array. <c>range</c> may contains index specifications for one ... to any 
        /// dimension. The array returned will have the size specified by range.</para>
        /// </remarks>
        public ILRetLogical this[params ILBaseArray[] dims] {
            get {
                return new ILRetLogical((ILLogicalStorage)Storage.Subarray(dims));
            }
        }
        #endregion

    }
}
