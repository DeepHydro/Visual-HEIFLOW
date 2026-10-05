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
    /// Logical arrays store true/false conditions as elements. Each element consumes
    /// one byte. Logical arrays are the output parameter of all relational comparisons.</remarks>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public sealed class ILRetLogical : ILBaseLogical {

        private static readonly bool s_isTempArray = true;

        #region properties
        /// <summary>
        /// Number of 'true' elements in this array
        /// </summary>
        /// <remarks>This value caches the number of 'true' elements in this logical array. 
        /// It may be used for information purposes but is actually needed internally for performance 
        /// reasons.</remarks>
        public override long NumberNonZero {
            get {
                using (ILScope.Enter(this))
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
                using (ILScope.Enter(this))
                    return new ILRetLogical((ILLogicalStorage)Storage.ShiftDimensions(1), NumberNonZero);
            }
        }
        /// <summary>
        /// Create clone of this array 
        /// </summary>
        public new ILRetLogical C {
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
        /// Test if this array instance is a column vector (e.g. n x 1)
        /// </summary>
        public override bool IsColumnVector {
            get {
                using (ILScope.Enter(this))
                    return Size[1] == 1 && Size.NumberOfDimensions == 2; ;
            }
        }
        /// <summary>
        /// Test if this array instance is a row vector (e.g. 1 x n)
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
                    return false;
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
                    return true;
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
        internal ILRetLogical(ILDenseStorage<byte> A)
            : base(new ILLogicalStorage(A.GetDataArray(), A.Size), s_isTempArray) {
        }
        /// <summary>
        /// Constructor creating ILRetLogical from dense storage
        /// </summary>
        /// <param name="A">input array, the storage given will directly be used for 
        /// storage of the new logical array</param>
        internal ILRetLogical(ILLogicalStorage A)
            : base(A, s_isTempArray) {
        }
        /// <summary>
        /// create temporary logical from predefined storage
        /// </summary>
        /// <param name="A">the storage will directly be used as storage of the new logical array</param>
        /// <param name="numberNonZero">number of nonzero elements in A. Must be positive or 0.</param>
        /// <remarks> Providing this parameter prevents the constructor from having to count the 
        /// 'true' elements in A.</remarks>
        internal ILRetLogical(ILLogicalStorage A, long numberNonZero)
            : base(A, s_isTempArray) {
            if (numberNonZero < 0) 
                throw new ILNumerics.Exceptions.ILArgumentException("invalid number of non-zero-elements given!");
            NumberNonZero = numberNonZero;
        }
        /// <summary>
        ///	create temporary logical array of specified size
        /// </summary>
        /// <param name="size">
        /// variable length int array specifying the number and size of dimensions to 
        /// be created.
        /// </param>
        /// <remarks>
        /// The size parameter may not be null or an empty array. An exception will be 
        /// thrown in this case. The dimensions will be trimmed before processing 
        /// (removing trailing non singleton dimensions). 
        /// Depending on the requested size a logical temporary array of the specified size 
        /// will be created. </remarks>
        internal ILRetLogical(ILSize size)
            : base(new ILLogicalStorage(size), s_isTempArray) {
        }
        /// <summary>
        ///	Constructor - create ILRetLogical of specified size 
        /// from data array
        /// </summary>
        /// <param name="size">
        /// Variable length int array specifying the number and size of dimensions to 
        /// be created.
        /// </param>
        /// <param name="data">byte array matching the size of the dimensions 
        /// specified. The data will directly be used as storage! No copy will be made!</param>
        /// <remarks>
        /// The size parameter may not be null or an empty array! An Exception will be 
        /// thrown in this case. The dimensions will be trimmed before processing 
        /// (removing trailing non singleton dimensions). 
        /// Depending on the requested size an ILRetLogical of the specified size 
        /// will be created. The type of storage will be <code>byte</code>.
        /// </remarks>
        public ILRetLogical(byte[] data, params int[] size)
            : base(new ILLogicalStorage(data, new ILSize(size)), s_isTempArray) {
        }
        /// <summary>
        /// Constructor creating ILRetLogical, provide predefined storage
        /// </summary>
        /// <param name="data">Predefined storage elements. The array will directly be used 
        /// as underlying storage. No copy will be made! </param>
        /// <param name="size">Size descriptor</param>
        public ILRetLogical(byte[] data, ILSize size)
            : base(new ILLogicalStorage(data, size), s_isTempArray) {
        }
        /// <summary>
        /// Constructor creating ILRetLogical, predefined storage (fast version)
        /// </summary>
        /// <param name="data">Predefined storage elements. The array will directly be used 
        /// as underlying storage. No copy will be made! </param>
        /// <param name="size">Size descriptor</param>
        /// <param name="nonZeroCount">Number of nonzero elements in <paramref name="data"/>. 
        /// Providing this parameter prevents from counting the 'true' elements (again). </param>
        public ILRetLogical(byte[] data, ILSize size, long nonZeroCount)
            : base(new ILLogicalStorage(data, size), s_isTempArray) {
            if (nonZeroCount < 0)
                throw new ILNumerics.Exceptions.ILArgumentException("invalid number of non-zero-elements given!");
            NumberNonZero = nonZeroCount;
        }
        #endregion

        #region public functions
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
        /// <param name="A">N-dimensional array. Except for dimensions <paramref name="dim"/>
        /// the dimensions of A must match the dimensions of this storage</param>
        /// <param name="dim">Index of dimension to concatenate arrays along.
        /// If dim is larger than the number of dimensions of any of the arrays,
        /// its value will be used in modulus the number of dimensions.</param>
        /// <returns>New array having the size 
        /// of both input arrays layed behind each other along the dim's-dimension</returns>
        public new ILRetLogical Concat(ILInLogical A, int dim) {
            using (ILScope.Enter(this,A))
                return new ILRetLogical((ILLogicalStorage)Storage.Concat(A.Storage, dim));
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
        public new void ExportValues(ref byte[] outArray) {
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
        /// <example><code>ILDenseStorage&lt;T&gt; A = ILMath.rand(5,4,6);
        /// foreach (double element in A) {
        /// // all elements are scalar double values
        /// String.Format("Element: {0} ",element);
        /// // Note: 'element' cannot be used to alter the collection! 
        /// } 
        /// </code></example> 
        public override IEnumerator<byte> GetEnumerator() {
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
        public override bool GetLimits(out byte min, out byte max) {
            using (ILScope.Enter(this))
                return base.GetLimits(out min, out max);
        }
        /// <summary>
        /// Get single element from this array
        /// </summary>
        /// <param name="idx">Indices, location of element</param>
        /// <returns>The requested element</returns>
        public override byte GetValue(params int[] idx) {
            using (ILScope.Enter(this))
                return base.GetValue(idx);
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
            using (ILScope.Enter(this)) {
                ILLogical ret = C;
                ret.Storage.Reshape(size);
                return ret;
            }
        }
        /// <summary>
        /// Create replication of this array
        /// </summary>
        /// <param name="dims">Dimensions specifier. If the number of elements in <paramref name="dims"/> is 
        /// less than the number of dimensions in this array, the trailing dimensions will 
        /// be set to 1 (singleton dimensions). On the other hand, if the number specified 
        /// is larger then the number of dimension stored inside the storge the resulting 
        /// storage will get its number of dimensions extended accordingly. </param>
        /// <returns>Array being created out of multiple replications of this array along 
        /// arbitrary dimensions according to <paramref name="dims"/></returns>
        public new ILRetLogical Repmat(params int[] dims) {
            using (ILScope.Enter(this)) 
            return new ILRetLogical((ILLogicalStorage)Storage.Repmat(dims));
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
            using (ILScope.Enter(this))
                return new ILRetLogical((ILLogicalStorage)Storage.ShiftDimensions(shift));
        }
        /// <summary>
        ///	Subarray from this array
        /// </summary>
        /// <param name="range">Arrays specifying the ranges to create subarray from</param>
        /// <returns>Subarray as specified</returns>
        public new ILRetLogical Subarray(params ILBaseArray[] range) {
            using (ILScope.Enter(this))
            using (ILScope.Enter(range))
                return new ILRetLogical((ILLogicalStorage)Storage.Subarray(range));
        }
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
                            ILDenseStorage<byte> temp = this.Storage.ShiftDimensions(1);
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
        public ILRetLogical this[params ILBaseArray[] range] {
            get {
                using (ILScope.Enter(this))
                using (ILScope.Enter(range))
                    return new ILRetLogical((ILLogicalStorage)Storage.Subarray(range));
            }
        } 
        #endregion

        #region operator overloading
        #region constructional operators 
        /// <summary>
        /// Implicitly cast one dimensional System.Array to ILNumerics array (vector)
        /// </summary>
        /// <param name="A">1-dimensional system array, arbitrary type</param>
        /// <returns>ILNumerics array of same element type as elements of A. 
        /// Row vector. If A is null: empty array.</returns>
        /// <remarks>The System.Array A will directly be used for the new ILNumerics array! 
        /// No copy will be done! Make sure, not to reference A after this conversion!</remarks>
        public static implicit operator ILRetLogical(byte[] A) {
            if (A == null) {
                return new ILRetLogical(ILSize.Empty00);
            }
            return new ILRetLogical(A, 1, A.Length);
        }
        /// <summary>
        /// Implicitly convert n-dim. System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">Arbitrarily sized System.Array</param>
        /// <returns>If A is null: empty array. Else: new ILNumerics array of the same size as A</returns>
        /// <remarks>The resulting ILArray will reflect all dimensions of 
        /// A. Elements of A will get copied to elements of output array (shallow copy).</remarks>
        /// <exception cref="ILNumerics.Exceptions.ILCastException">If type of input does not match 
        /// ElementType</exception>
        public static implicit operator ILRetLogical(Array A) {
            if (A == null || A.Length == 0) {
                return new ILLogical(ILSize.Empty00);
            }
            if (A.GetType().GetElementType() != typeof(byte))
                throw new ILCastException("inner type of System.Array must match");
            int[] dims = new int[A.Rank];
            byte[] retArr = ILMemoryPool.Pool.New<byte>(A.Length);
            int posArr = 0;
            for (int i = 0; i < dims.Length; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (byte item in A)
                retArr[posArr++] = item;
            return new ILRetLogical(retArr, dims);
        }
        /// <summary>
        /// Implicitly cast two dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">2D System.Array</param>
        /// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        public static implicit operator ILRetLogical(byte[,] A) {
            if (A == null || A.Length == 0) {
                return new ILRetLogical(ILSize.Empty00);
            }
            int[] dims = new int[2];
            byte[] retArr = ILMemoryPool.Pool.New<byte>(A.Length);
            int posArr = 0;
            for (int i = 0; i < 2; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (byte item in A)
                retArr[posArr++] = item;
            return new ILRetLogical(retArr, dims);
        }
        /// <summary>
        /// Implicitly cast three dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">3D System.Array</param>
        /// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        public static implicit operator ILRetLogical(byte[, ,] A) {
            if (A == null || A.Length == 0) {
                return new ILRetLogical(ILSize.Empty00);
            }
            int[] dims = new int[3];
            byte[] retArr = ILMemoryPool.Pool.New<byte>(A.Length);
            int posArr = 0;
            for (int i = 0; i < 3; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (byte item in A)
                retArr[posArr++] = item;
            return new ILRetLogical(retArr, dims);
        }

        /// <summary>
        /// Implicitly convert boolean System.Byte to scalar logical array
        /// </summary>
        /// <param name="val">System.Byte</param>
        /// <returns>Scalar logical array with value of val.</returns>
        public static implicit operator ILRetLogical(bool val) {
            ILRetLogical ret = new ILRetLogical(new byte[1] { val ? (byte)1 : (byte)0 }, 1, 1);
            return ret;
        }
        /// <summary>
        /// Implicitly convert logical array to System.Boolean
        /// </summary>
        /// <param name="A">Logical array</param>
        /// <returns>true if elements of A are non-zero, false otherwise 
        /// </returns>
        /// <remarks>If A is null or empty, the function returns false. Otherwise returns true, 
        /// if all elements of A are non-zero and returns false, if A contains zero elements.
        /// <para>The behavior depends on the setting of the ILSettings.LogicalArrayToBoolConversion switch.
        /// Per default, only scalar arrays are allowed to be converted implicitely. This can be changed 
        /// to implicitely convert non-scalar arrays by using ILMath.any on the array.</para>
        /// <seealso cref="ILNumerics.Settings.LogicalArrayToBoolConversion"/>
        /// <seealso cref="ILNumerics.ILMath.any(ILInArray{double}, int)"/>
        /// </remarks>
        public static implicit operator bool(ILRetLogical A) {
            using (ILScope.Enter(A)) {
                // this operator is implicit for convenience reasons: 
                // if(tmp[0]!=-10.0) { ... is only possible this way
                if (object.Equals(A, null) || A.IsEmpty)
                    return false;
                if (A.IsScalar)
                    return A.GetValue(0, 0) == 1;
                if (Settings.LogicalArrayToBoolConversion == LogicalConversionMode.ImplicitAllAll)
                    return ILMath.allall(A).GetValue(0) == 1;
                //else if (Settings.ILSettings.LogicalArrayToBoolConversion == LogicalConversionMode.NonScalarThrowsException) 
                throw new ILArgumentException("Nonscalar logical to bool conversion. See ILSettings.LogicalArrayToBoolConversion");
            }
        }
        /// <summary>
        /// Implicitly convert integer scalar to logical array of size 1x1 (scalar).
        /// </summary>
        /// <param name="val">Scalar value</param>
        /// <returns>New logical array of size 1x1 holding the only element of type Byte 
        /// with value of val.</returns>
        public static implicit operator ILRetLogical(int val) {
            ILRetLogical ret = new ILRetLogical(new Byte[1] { 
                                 val != 0 ? (byte)1:(byte)0 }, 1, 1);
            return ret;
        }
        #endregion

        #region conversional operators 
        /// <summary>
        /// Convert logical to temporary logical array
        /// </summary>
        /// <param name="a">Original logical array</param>
        /// <returns>Temporary logical array</returns>
        public static implicit operator ILRetLogical(ILLogical a) {
            if (object.Equals(a,null))
                return null; 
            return a.C;
        }
        /// <summary>
        /// Convert logical input parameter type array to temporary logical array
        /// </summary>
        /// <param name="a">Logical input parameter type</param>
        /// <returns>Temporary logical array</returns>
        public static implicit operator ILRetLogical(ILInLogical a) {
            if (object.Equals(a, null))
                return null;
            return a.C;
        }
        /// <summary>
        /// Convert logical output parameter type array to temporary logical array
        /// </summary>
        /// <param name="a">Logical output parameter type</param>
        /// <returns>Temporary logical array</returns>
        public static implicit operator ILRetLogical(ILOutLogical a) {
            if (object.Equals(a, null))
                return null;
            return a.C;
        }
        /// <summary>
        /// Implicitly cast to ILArray&lt;byte&gt;
        /// </summary>
        /// <param name="a">A ILRetLogical</param>
        /// <returns>ILArray&lt;byte&gt;</returns>
        public static implicit operator ILArray<byte>(ILRetLogical a) {
            if (object.Equals(a, null))
                return null;
            ILArray<byte> ret = new ILArray<byte>(a.GiveStorageAwayOrClone()); 
            ILScope.Context.RegisterArray(ret); 
            return ret; 
        }
        /// <summary>
        /// Implicitly cast from ILArray&lt;byte&gt;
        /// </summary>
        /// <param name="a">An ILArray&lt;byte&gt;</param>
        /// <returns>logical return array</returns>
        public static implicit operator ILRetLogical(ILArray<byte> a) {
            if (object.Equals(a,null))
                return null; 
            return new ILRetLogical(new ILLogicalStorage(
                a.Storage.GetDataArray(),a.Size)); 
        }
        /// <summary>
        /// Implicitly cast from ILInArray&lt;byte&gt;
        /// </summary>
        /// <param name="a">An ILInArray&lt;byte&gt;</param>
        /// <returns>logical return array</returns>
        public static implicit operator ILInArray<byte>(ILRetLogical a) {
            if (object.Equals(a,null))
                return null; 
            ILArray<byte> ret = new ILArray<byte>(a.GiveStorageAwayOrClone());
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret;
        }
        /// <summary>
        /// Implicitly cast from ILInArray&lt;byte&gt;
        /// </summary>
        /// <param name="a">An ILInArray&lt;byte&gt;</param>
        /// <returns>logical return array</returns>
        public static implicit operator ILRetLogical(ILInArray<byte> a) {
            if (object.Equals(a, null))
                return null;
            return new ILRetLogical(new ILLogicalStorage(
                a.Storage.GetDataArray(), a.Size));
        }
        #endregion

        #region operational operators
        /// <summary>
        /// Invert values of array elements 
        /// </summary>
        /// <param name="in1">Input array</param>
        /// <returns>New logical array, inverted element values</returns>
        public static ILRetLogical operator !(ILRetLogical in1) {
            if (object.Equals(in1,null))
                throw new ILArgumentException("operator -(): parameter must not be null!");
            return (in1 != (byte)1);
        }
        #endregion
        #endregion

    }
}
