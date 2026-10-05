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
using System.IO; 
using System.Text;
using ILNumerics.Storage;
using ILNumerics; 
using ILNumerics.Exceptions; 
using ILNumerics.Misc;

namespace ILNumerics {
    /// <summary>
    /// N-dimensional, generic array class, temporary variant, is disposed after first use
    /// </summary>
    /// <typeparam name="ElementType">Inner element type</typeparam>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public sealed class ILRetArray<ElementType> : ILDenseArray<ElementType> {

        #region properties
        /// <summary>
        /// Clone of this array (fast, cheap and shallow)
        /// </summary>
        /// <remarks><para>Calling this member will dispose this instance afterwards (for temporary arrays).</para>
        /// </remarks>
        public override ILRetArray<ElementType> C {
            get {
                return this;
            }
        }

        /// <summary>
        /// Size descriptor shortcut
        /// </summary>
        public override ILSize S {
            get {
                using (ILScope.Enter(this))
                    return Storage.Size;
            }
        }
        /// <summary>
        /// Size descriptor 
        /// </summary>
        public override ILSize Size {
            get {
                using (ILScope.Enter(this))
                    return Storage.Size;
            }
        }
        /// <summary>
        /// Test if this array instance is a column vector
        /// </summary>
        public override bool IsColumnVector {
            get {
                using (ILScope.Enter(this))
                    return Size[1] == 1 && Size.NumberOfDimensions == 2; ;
            }
        }
        /// <summary>
        /// Test if this array instance is a row vector
        /// </summary>
        public override bool IsRowVector {
            get {
                using (ILScope.Enter(this))
                    return Size[0] == 1 && Size.NumberOfDimensions == 2;
            }
        }
        /// <summary>
        /// Determine if this array has complex elements.
        /// </summary>
        /// <remarks><para>Calling this member will dispose this instance afterwards (for temporary arrays).</para></remarks>
        public override bool IsComplex {
            get {
                using (ILScope.Enter(this))
                    return base.IsComplex;
            }
        }
        /// <summary>
        /// Test if this instance is an empty array (number of elements stored = 0)
        /// </summary>
        public override bool IsEmpty {
            get {
                using (ILScope.Enter(this))
                    return Size.NumberOfElements == 0;
            }
        }
        /// <summary>
        /// Test if this instance is a matrix
        /// </summary>
        /// <remarks>In order for an array to be a matrix the number of <b>non singleton</b> 
        /// dimensions must equal 2. This attribute is readonly.</remarks>
        public override bool IsMatrix {
            get {
                using (ILScope.Enter(this))
                    return Size.NumberOfDimensions == 2;
            }
        }
        /// <summary>
        /// Determine if this array holds numeric values.
        /// </summary>
        /// <remarks>An ILArray is numeric as long as its elements are one of the 
        /// following types: 
        /// <list type="table">
        /// <listheader>
        ///     <term>inner type</term>
        /// </listheader>
        /// <item>
        ///     <term>System.double</term>
        ///     <description>floating point, real, 8 bytes </description>
        /// </item>
        /// <item>
        ///     <term>System.float</term>
        ///     <description>floating point real, 4 bytes</description>
        /// </item>
        /// <item>
        ///     <term>ILNumerics.complex</term>
        ///     <description>floating point complex, 16 bytes</description>
        /// </item>
        /// <item>
        ///     <term>ILNumerics.fcomplex</term>
        ///     <description>floating point complex, 8 bytes</description>
        /// </item>
        /// <item>
        ///     <term>System.char</term>
        ///     <description>integer, real, 1 byte</description>
        /// </item>
        /// <item>
        ///     <term>System.byte</term>
        ///     <description>integer, real, 1 byte</description>
        /// </item>
        /// <item>
        ///     <term>System.Int16</term>
        ///     <description>integer, real, 2 byte</description>
        /// </item>
        /// <item>
        ///     <term>System.Int32</term>
        ///     <description>integer, real, 4 byte</description>
        /// </item>
        /// <item>
        ///     <term>System.Int64</term>
        ///     <description>integer, real, 8 byte</description>
        /// </item>
        /// <item>
        ///     <term>System.UInt16</term>
        ///     <description>unsigned integer, real, 2 byte</description>
        /// </item>
        /// <item>
        ///     <term>System.UInt32</term>
        ///     <description>unsigned integer, real, 4 byte</description>
        /// </item>
        /// <item>
        ///     <term>System.UInt64</term>
        ///     <description>unsigned integer, real, 8 byte</description>
        /// </item>
        /// </list>
        /// <para>Calling this member will dispose this instance afterwards (for temporary arrays).</para>
        /// </remarks>
        public override bool IsNumeric {
            get {
                using (ILScope.Enter(this))
                    return base.IsNumeric;
            }
        }
        /// <summary>
        /// Test if this instance is a scalar
        /// </summary>
        /// <remarks>This attribute is readonly. It returns: Size.NumberOfElements == 1.</remarks>
        public override bool IsScalar {
            get {
                using (ILScope.Enter(this))
                    return Size.NumberOfElements == 1;
            }
        }
        /// <summary>
        /// Test if this array is a vector
        /// </summary>
        /// <remarks>In order for an array to be a vector the number of <b>non singleton</b> 
        /// dimensions must equal 1. Keep in mind that all ILArrays have at least 2 dimensions. Therefore 
        /// it is not sufficient to test for the number of dimensions, but to take the number of 
        /// <b>non singleton</b> dimensions into account. This attribute is readonly.</remarks>
        public override bool IsVector {
            get {
                using (ILScope.Enter(this))
                    return (Size[0] == 1 || Size[1] == 1) && Size.NumberOfDimensions == 2;
            }
        }
        /// <summary>
        /// Length of the longest dimension of this instance
        /// </summary>
        /// <remarks>This property is readonly.
        /// <para>Calling this member will dispose this instance afterwards (for temporary arrays).</para></remarks>
        public override int Length {
            get {
                using (ILScope.Enter(this))
                    return base.Length;
            }
        }
        /// <summary>
        /// Return transposed version of this array
        /// </summary>
        public new ILRetArray<ElementType> T {
            get {
                using (ILScope.Enter(this))
                    return new ILRetArray<ElementType>(Storage.ShiftDimensions(1));
            }
        }
        /// <summary>
        /// Gets the name of this array (readonly)
        /// </summary>
        public new String Name {
            get {
                using (ILScope.Enter(this))
                    return m_name;
            }
        }
        #endregion

        #region constructors
        internal ILRetArray(ILDenseStorage<ElementType> denseStorage) :
            base(denseStorage, true) { }

        internal ILRetArray(ElementType[] elements, params int[] dimensions) :
            this(elements, new ILSize(dimensions)) { }

        internal ILRetArray(ElementType[] elements, ILSize dimensions) :
            base(new ILDenseStorage<ElementType>(elements, dimensions), true) { }

        internal ILRetArray(ILSize dimensions) :
            base(new ILDenseStorage<ElementType>(dimensions), true) { }

        internal static ILRetArray<ElementType> empty(ILSize dimension) {
            return new ILRetArray<ElementType>(new ElementType[0], dimension);
        }

        #endregion

        #region operator overloads
        #region constructive operators
        /// <summary>
        /// Implicitly convert scalar to array of size 1x1 (scalar).
        /// </summary>
        /// <param name="val">Single element of ElementType type</param>
        /// <returns>New array of of size 1x1 holding the only element with value of val.
        /// </returns>
        public static implicit operator ILRetArray<ElementType>(ElementType val) {
            ILRetArray<ElementType> ret = new ILRetArray<ElementType>(
                        new ILDenseStorage<ElementType>(new ElementType[] { val }, new ILSize(1, 1)));
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
        public static implicit operator ILRetArray<ElementType>(ElementType[] A) {
            if (A == null) { 
                // TODO: check: return null better? 
                ILDenseStorage<ElementType> dS = new ILDenseStorage<ElementType>(new ElementType[0], ILSize.Empty00);
                return new ILRetArray<ElementType>(dS);
            }
            if (Settings.CreateRowVectorsByDefault) {
                return new ILRetArray<ElementType>(A, 1, A.Length);
            } else {
                return new ILRetArray<ElementType>(A, A.Length, 1);
            }
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
        public static implicit operator ILRetArray<ElementType>(Array A) {
            if (A == null || A.Length == 0) {
                // TODO: check: return null better? 
                return new ILRetArray<ElementType>(ILSize.Empty00);
            }
            if (A.GetType().GetElementType() != typeof(ElementType))
                throw new ILCastException("inner type of System.Array must match");
            int[] dims = new int[A.Rank];
            ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(A.Length);
            int posArr = 0;
            for (int i = 0; i < dims.Length; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (ElementType item in A)
                retArr[posArr++] = item;
            return new ILRetArray<ElementType>(retArr, dims);
        }
        /// <summary>
        /// Implicitly cast two dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">2-dimensional System.Array</param>
        /// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        public static implicit operator ILRetArray<ElementType>(ElementType[,] A) {
            if (A == null || A.Length == 0) {
                // TODO: check: return null better? 
                return new ILRetArray<ElementType>(ILSize.Empty00);
            }
            int[] dims = new int[2];
            ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(A.Length);
            int posArr = 0;
            for (int i = 0; i < 2; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (ElementType item in A)
                retArr[posArr++] = item;
            return new ILRetArray<ElementType>(retArr, dims);
        }
        /// <summary>
        /// Implicitly cast three dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">3-dimensional System.Array</param>
        /// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        public static implicit operator ILRetArray<ElementType>(ElementType[, ,] A) {
            if (A == null || A.Length == 0) {
                // TODO: check: return null better? 
                return new ILRetArray<ElementType>(ILSize.Empty00);
            }
            int[] dims = new int[3];
            ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(A.Length);
            int posArr = 0;
            for (int i = 0; i < 3; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (ElementType item in A)
                retArr[posArr++] = item;
            return new ILRetArray<ElementType>(retArr, dims);
        }
        /// <summary>
        /// Convert array to temporary cell 
        /// </summary>
        /// <param name="A">Source array</param>
        /// <returns>Temporary scalar cell, having the only element with a clone of A</returns>
        public static implicit operator ILRetCell(ILRetArray<ElementType> A) {
            using (ILScope.Enter(A)) {
                ILRetCell ret = new ILRetCell(ILSize.Scalar1_1, A.Storage);
                ret.Storage.FromImplicitCast = true;
                return ret;
            }
        }
        #endregion

        #region implicit conversions
        /// <summary>
        /// 'Clone' conversion
        /// </summary>
        /// <param name="A">Source array</param>
        /// <returns>Temporary ILArray, will be disposed after next operation</returns>
        /// <remarks>This conversion is for convenient reasons only. It enables the direct 
        /// use of (persistent) ILArray objects in situations where (temporary) ILRetArray 
        /// is required. A (fast) clone will be made. This is an alias for A.C.</remarks>
        public static implicit operator ILRetArray<ElementType>(ILArray<ElementType> A) {
            if (object.Equals(A,null)) return null; 
            return A.C;
        }
        /// <summary>
        /// 'Clone' conversion
        /// </summary>
        /// <param name="A">Source array</param>
        /// <returns>Temporary ILArray, will be disposed after next operation</returns>
        /// <remarks>This conversion is for convenient reasons only. It enables the direct 
        /// use of (persistent) ILArray objects in situations where (temporary) ILRetArray 
        /// is required. One example is the conversion of ILArray as return type. 
        /// A (fast) clone will be made. This is an alias for A.C.</remarks>
        public static implicit operator ILRetArray<ElementType>(ILInArray<ElementType> A) {
            if (object.Equals(A, null)) return null;
            return A.C;
        }
        /// <summary>
        /// 'Clone' conversion
        /// </summary>
        /// <param name="A">Source array</param>
        /// <returns>temporary ILArray, will be disposed after next operation</returns>
        /// <remarks>This conversion is for convenient reasons only. It enables the direct 
        /// use of (persistent) ILArray objects in situations where (temporary) ILRetArray 
        /// is required. One example is the conversion of ILArray as return type. 
        /// A (fast) clone will be made. This is an alias for A.C.</remarks>
        public static implicit operator ILRetArray<ElementType>(ILOutArray<ElementType> A) {
            if (object.Equals(A, null)) return null;
            return A.C;
        }
        #endregion
        #endregion operator overloads

        #region public function
        /// <summary>
        /// Clone of this array
        /// </summary>
        /// <remarks><para>
        /// Clones of all arrays in ILNumerics are done in a very fast, lazy way. This means, 
        /// at the time the clone is made, no relevant memory is copied. Elements of both arrays rather point to the same 
        /// underlying System.Array. A reference counting mechanism ensures the detaching of thoses arrays 
        /// on write access.</para>
        /// <para>The clone returned will be of the same type as this instance.</para></remarks>
        internal override ILBaseArray Clone() {
            return this;
        }
        /// <summary>
        /// Concatenate this array 
        /// </summary>
        /// <param name="A">n-dimensional storage</param>
        /// <param name="dim">Dimension index along which to concatenate the arrays.</param>
        /// <returns>New array with copy elements of this array and A</returns>
        /// <remarks>The array returned will be a copy of both arrays involved. None 
        /// of the input arrays will be altered. If <paramref name="dim"/> is larger than 
        /// the number of dimensions of one of the arrays its value will be used in modulus. 
        /// <para>The resulting array has the size of both input arrays, laid beside one 
        /// another along the <paramref name="dim"/> dimension.</para></remarks>
        public new ILRetArray<ElementType> Concat(ILInArray<ElementType> A, int dim) {
            using (ILScope.Enter(this,A))
                return new ILRetArray<ElementType>(Storage.Concat(A.Storage, dim));
        }
        /// <summary>
        /// Compare elements and shape of this array with another array
        /// </summary>
        /// <param name="A">Other array</param>
        /// <returns>true if shape and element values of both arrays match, false otherwise</returns>
        /// <remarks><para>Calling this member will dispose this instance afterwards (for temporary arrays.</para></remarks>
        public override bool Equals(object A) {
            using (ILScope.Enter(this))
                return base.Equals(A);
        }
        /// <summary>
        /// Copy values of all elements into System.Array.
        /// </summary>
        /// <param name="outArray">[Output] System.Array, holding all element values of this ILDenseStorage.</param>
        /// <remarks>The System.Array may be predefined. If its length is sufficient, it will be used and 
        /// its leading elements will be overwritten when function returns. If 'outArray' is null or has too few elements, 
        /// it will be recreated from the ILNumerics memory pool.</remarks>
        public new void ExportValues(ref ElementType[] outArray) {
            using (ILScope.Enter(this))
                Storage.ExportValues(ref outArray); 
        }
        /// <summary>
        /// Enumerator returning elements as ElementType
        /// </summary>
        /// <returns>Enumerator</returns>
        /// <remarks>This method enables the use of ILNumerics arrays in foreach loops.
        /// <para>The iterator is returned, if arrays are directly used in foreach statements. The iterator 
        /// is compatible with ILNumerics memory management.</para></remarks>
        /// <example><code>ILArray&lt;double&gt; A = rand(5,4,6);
        /// foreach (double element in A) {
        ///     // all elements are scalar double values
        ///     String.Format("Element: {0} ", element);
        ///     // Note: 'element' cannot be used to alter the collection! 
        /// } </code>
        /// </example> 
        public override IEnumerator<ElementType> GetEnumerator() {
            using (ILScope.Enter(this)) {
                return Storage.GetEnumerator(); 
            }
        }
        /// <summary>
        /// Generate a hash code based on the current arrays values
        /// </summary>
        /// <returns>Hash code</returns>
        /// <remarks>The hashcode is generated by taking the values currently stored in the array into account.
        /// Therefore, the function must iterate over all elements in the array - which makes it somehow a costly 
        /// operation. Take this into account, if you consider using large arrays in collections like dictionaries 
        /// or hashtables, which make great use of hash codes.
        /// <para>Calling this member will dispose this instance afterwards (for temporary arrays).</para></remarks>
        public override int GetHashCode() {
            using (ILScope.Enter(this))
                return base.GetHashCode();
        }
        /// <summary>
        /// Get minimum and maximum value of all elements - if any
        /// </summary>
        /// <param name="min">[Output] Minimum value</param>
        /// <param name="max">[Output] Maximum value</param>
        /// <returns>true if the limits exists and could be computed, false otherwise</returns>
        /// <remarks>Empty arrays will return false. In this case the output parameter will be: default(ElementType).
        /// <para>Calling this member will dispose this instance afterwards (for temporary arrays).</para></remarks>
        public override bool GetLimits(out ElementType min, out ElementType max) {
            using (ILScope.Enter(this))
                return base.GetLimits(out min, out max);
        }
        /// <summary>
        /// Get single element from this array
        /// </summary>
        /// <param name="idx">Indices, location of element</param>
        /// <returns>The requested element</returns>
        public override ElementType GetValue(params int[] idx) {
            using (ILScope.Enter(this))
                return base.GetValue(idx);
        }
        /// <summary>
        /// Create replication of this array
        /// </summary>
        /// <param name="dims">Dimensions specifier. If the number of elements in <paramref name="dims"/> is 
        /// less than the number of dimensions in this array, the trailing dimensions will 
        /// be set to 1 (singleton dimensions). On the other hand, if the number specified 
        /// is larger then the number of dimension stored inside the storge the resulting 
        /// storage will get its number of dimensions extended accordingly. </param>
        /// <returns>Array being created by multiple replications of this array along 
        /// arbitrary dimensions according to <paramref name="dims"/></returns>
        public new ILRetArray<ElementType> Repmat(params int[] dims) {
            using (ILScope.Enter(this))
                return new ILRetArray<ElementType>(Storage.Repmat(dims));
        }
        /// <summary>
        /// Reshaped copy of this array
        /// </summary>
        /// <param name="dimensions">New dimensions of the array</param>
        /// <returns>Reshaped copy of the array</returns>
        /// <remarks><para>The current instance will not be changed. A new storage is created, having 
        /// the elements of this array and a shape as determined by <paramref name="dimensions"/>.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the number of elements in <paramref name="dimensions"/>
        /// do not match the number of elements in this array.</exception>
        public  new ILRetArray<ElementType> Reshape(ILSize dimensions) {
            using (ILScope.Enter(this)) {
                ILArray<ElementType> ret = C;
                ret.Storage.Reshape(dimensions);
                return ret;
            }
        }
        /// <summary>
        ///  Serialize this ILArray into a binary stream.
        /// </summary>
        /// <param name="outStream">System.IO.Stream to receive the byte stream 
        /// for this ILBaseArray</param>
        /// <returns>True on success, false on error.</returns>
        /// <remarks><para>Calling this member will dispose this instance afterwards (for temporary arrays).</para>
        /// </remarks>
        public override bool Serialize(Stream outStream) {
            using (ILScope.Enter(this))
                return base.Serialize(outStream);
        }
        /// <summary>
        /// Create array from this array and shift dimensions
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
        public new ILRetArray<ElementType> Shifted(int shift) {
            using (ILScope.Enter(this))
                return new ILRetArray<ElementType>(Storage.ShiftDimensions(shift));
        }
        /// <summary>
        /// Subarray creation
        /// </summary>
        /// <param name="size">Range specification, defining the size of the subarray</param>
        /// <returns>Subarray as copy of a part of this array</returns>
        /// <remarks>Consult the ILNumerics subarray documentation for all subarray indexing rules.</remarks>
        public new ILRetArray<ElementType> Subarray(params ILBaseArray[] size) {
            using (ILScope.Enter(this))
            using (ILScope.Enter(size))
                return new ILRetArray<ElementType>(Storage.Subarray(size));
        }
        
        ///// <summary>
        ///// Short textual summary of this instance; used for debug output
        ///// </summary>
        ///// <returns>String representation of type and size</returns>
        ///// <remarks>The type of elements and the size of the array are displayed. If the array
        ///// is scalar, its value is displayed next to the type.
        ///// <para>Calling this member will dispose this instance afterwards (for temporary arrays).</para></remarks>
        //public override string ShortInfo() {
        //    using (ILScope.Enter(this))
        //        return base.ShortInfo();
        //}
       
        /// <summary>
        /// Send values of this instance to stream. 
        /// </summary>
        /// <param name="stream">Stream to write the values into.</param>
        /// <param name="format">Format string to be used for output. See <see cref="System.String.Format(string,object)"/> for a specification
        /// of valid formating expressions. This flag is only used, when 'method' is set to 'Serial'.</param>
        /// <param name="method">A constant out of <see cref="ILArrayStreamSerializationFlags"/>. Specifies the way in which
        /// the values will be serialized.</param>
        /// <remarks><para>If the 'Formatted' method is used, any occurences of the NewLine character(s) 
        /// will be replaced from the format string before applying to the elements. This is done to 
        /// prevent the format from breaking the 'page' style of the output.</para>
        /// <para>If 'method' is set to 'Matlab', the array will be written as Matfile version 5.0. No compression will be used. The internal 'Name' property will be used as the
        /// array name for writing. This array instance will be the only array in the .mat file. If you want to write several arrays bundled into one mat file, use the MatFile class to
        /// create a collection of arrays and write the MatFile to stream.</para></remarks>
        public override void ToStream(Stream stream, string format, ILArrayStreamSerializationFlags method) {
            using (ILScope.Enter(this))
                try {
                    int len;
                    switch (method) {
                        case ILArrayStreamSerializationFlags.Serial:
                            len = Size.NumberOfElements;
                            using (TextWriter tw = new StreamWriter(stream)) {
                                for (int i = 0; i < len; i++) {
                                    tw.Write(format, GetValue(i));
                                }
                            }
                            break;
                        case ILArrayStreamSerializationFlags.Formatted:
                            format = format.Replace(Environment.NewLine, "");
                            ILDenseStorage<ElementType> temp = this.Storage.ShiftDimensions(1);
                            //len = Dimensions.NumberOfElements / Dimensions[1]; 
                            using (TextWriter tw = new StreamWriter(stream)) {
                                tw.Write(temp.ValuesToString(0));
                            }
                            break;
                        case ILArrayStreamSerializationFlags.Matlab:
                            ILMatFile mf = new ILMatFile(new ILBaseArray[1] { this });
                            mf.Write(stream);
                            break;
                    }
                } catch (Exception e) {
                    throw new ILException("ToStream: Could not serialize to stream.", e);
                }
        }
        #endregion

        #region indexer
        /// <summary>
        /// Subarray creation
        /// </summary>
        /// <param name="range">Range specification, defining the size of the subarray</param>
        /// <returns>Subarray as copy of a part of this array</returns>
        /// <remarks>Consult the ILNumerics subarray documentation for all subarray indexing rules.</remarks>
        public ILRetArray<ElementType> this[params ILBaseArray[] range] {
            get {
                using (ILScope.Enter(this))
                using (ILScope.Enter(range))
                    return new ILRetArray<ElementType>(Storage.Subarray(range));
            }
        }
        #endregion

        #region memory management   
        internal override bool EnterScope() {
            return false; 
        }
        #endregion
    }
}
