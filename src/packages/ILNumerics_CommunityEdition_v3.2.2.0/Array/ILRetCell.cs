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
using ILNumerics.Exceptions;
using ILNumerics.Storage;

namespace ILNumerics {
    /// <summary>
    /// Mathematical container class holding arbitrary array objects as elements 
    /// </summary>
    /// <remarks>
    /// Cells act as general purpose container. Cells store arbitrary arrays of arbitrary element type. 
    /// Theses elements commonly are cells itself. <c>ILRetCell</c> acts as return type 
    /// in function declarations when a cell needs to be returned from a function. 
    /// Use <see cref="ILNumerics.ILCell"/> for local cell variables in your algorithms. 
    /// <para>More details about cells are found in the <a href="http://ilnumerics.net/$Cells.html">online manual</a>.</para>
    /// </remarks>
    /// <seealso cref="ILNumerics.ILCell"/>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public sealed class ILRetCell : ILBaseCell {

        private static readonly bool s_isTempArray = true;

        #region constructors
        /// <summary>
        /// Create cell object with pre-created data in specified dimensions 
        /// </summary>
        /// <param name="data">Predefined element data array, will be used for new cell (no copy will be made)</param>
        /// <param name="size">Size of the new cell</param>
        /// <remarks>Object array data will directly be used for storage. No 
        /// copy will be made. However, any arrays referenced by data are dereferenced for storage inside the cell. The size must match prod(size)</remarks>
        [Obsolete("use <see cref='ILNumerics.ILMath.cell'/> instead!")]
        internal ILRetCell(ILStorage[] data, params int[] size)
            : base(new ILCellStorage(data, new ILSize(size)), s_isTempArray) {
        }
        /// <summary>
        /// Create cell object with pre-created data in specified dimensions 
        /// </summary>
        /// <param name="data">Predefined element data array, will be used for new cell (no copy will be made)</param>
        /// <param name="size">Size of the new cell</param>
        /// <remarks>Object array data will directly be used for storage. No 
        /// copy will be made. However, any arrays referenced by data are dereferenced for storage inside the cell. The size must match prod(size)</remarks>
        [Obsolete("use <see cref='ILNumerics.ILMath.cell'/> instead!")]
        internal ILRetCell(ILStorage[] data, ILSize size)
            : base(new ILCellStorage(data, size), s_isTempArray) {
        }
        /// <summary>
        /// Create new cell object, elements will be 'null' 
        /// </summary>
        /// <param name="size">Size descriptor of the new cell</param>
        [Obsolete("use <see cref='ILNumerics.ILMath.cell'/> instead!")]
        internal ILRetCell(params int[] size)
            : base(new ILCellStorage(new ILStorage[ILCell.prod(size)], new ILSize(size)), s_isTempArray) { }

        internal ILRetCell(ILCellStorage cellStorage)
            : base(cellStorage, s_isTempArray) { }

        [Obsolete("use <see cref='ILNumerics.ILMath.cell'/> instead!")]
        internal ILRetCell(ILSize size, params ILStorage[] values)
            : base(new ILCellStorage(values, size), s_isTempArray) { }

        #endregion constructors

        #region properties
        /// <summary>
        /// Clone of this cell
        /// </summary>
        /// <remarks><para>
        /// Clones of all arrays in ILNumerics are done in a very fast, lazy way. This means, 
        /// at the time the clone is made, no relevant memory is copied. Elements of both arrays rather point to the same 
        /// underlying System.Array. A reference counting mechanism ensures the detaching of thoses arrays on write access.</para>
        /// <para>Cells profit from the same efficient clone creation process. However, since a cell may store an arbitrarily deep 
        /// hierarchy of other cells and arrays, in order to clone a cell, the cell elements have to be cloned as well - in an 
        /// recursive manner. Clones play an important role for ILNumerics cells. They are used to implement value semantics for cell
        /// elements. I.e.: the cloned cell returned cannot not be used to alter elements of the original cell in any way.</para></remarks>
        public new ILRetCell C {
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
        /// <remarks>This attribute is readonly. It returns: Dimension.NumberOfElements == 1.</remarks>
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
        /// Transposed version of this ILCell
        /// </summary>
        public new ILRetCell T {
            get {
                using (ILScope.Enter(this))
                    return new ILRetCell((ILCellStorage)Storage.ShiftDimensions(1));
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
        /// <summary>
        /// Access to the more specialized version (ILCellStorage) of this storage
        /// </summary>
        internal new ILCellStorage Storage {
            get { return (m_storage as ILCellStorage); }
            set {
                // e.g. Storage.Detach() may return itself (if no detaching is 
                // required). So we must check for that equalty here! 
                if (!object.Equals(value, m_storage)) {
                    m_storage.Dispose();
                    m_storage = value;
                }
            }
        }
        #endregion

        #region implicit casts
        #region constructional operators
        /// <summary>
        /// Convert primitive integer to a scalar temporary cell
        /// </summary>
        /// <param name="value">Primitive scalar int value</param>
        /// <returns>New scalar temporary cell</returns>
        public static implicit operator ILRetCell(int value) {
            ILRetCell ret = createScalar(value);
            ret.Storage.FromImplicitCast = true;
            return ret;
        }
        /// <summary>
        /// Convert primitive double to a scalar temporary cell
        /// </summary>
        /// <param name="value">Primitive scalar double value</param>
        /// <returns>New scalar temporary cell</returns>
        public static implicit operator ILRetCell(double value) {
            ILRetCell ret = createScalar(value);
            ret.Storage.FromImplicitCast = true;
            return ret;
        }
        /// <summary>
        /// Convert primitive float to a scalar temporary cell
        /// </summary>
        /// <param name="value">Primitive scalar float value</param>
        /// <returns>New scalar temporary cell</returns>
        public static implicit operator ILRetCell(float value) {
            ILRetCell ret = createScalar(value);
            ret.Storage.FromImplicitCast = true;
            return ret;
        }
        /// <summary>
        /// Convert primitive byte to a scalar temporary cell
        /// </summary>
        /// <param name="value">Primitive byte int value</param>
        /// <returns>New scalar temporary cell</returns>
        public static implicit operator ILRetCell(byte value) {
            ILRetCell ret = createScalar(value);
            ret.Storage.FromImplicitCast = true;
            return ret;
        }
        /// <summary>
        /// Encapsulate integer System.Array into a scalar temporary cell
        /// </summary>
        /// <param name="value">Primitive System.Int32 value - no copy will be made!</param>
        /// <returns>New scalar temporary cell</returns>
        public static implicit operator ILRetCell(int[] value) {
            ILRetCell ret = createArray(value);
            ret.Storage.FromImplicitCast = true;
            return ret;
        }
        /// <summary>
        /// Convert string to a scalar temporary cell
        /// </summary>
        /// <param name="value">String value</param>
        /// <returns>New scalar temporary cell</returns>
        public static implicit operator ILRetCell(string value) {
            ILRetCell ret = createScalar(value);
            ret.Storage.FromImplicitCast = true;
            return ret;
        }
        /// <summary>
        /// Encapsulate array of strings into a scalar temporary cell
        /// </summary>
        /// <param name="value">String array - no copy will be made!</param>
        /// <returns>New scalar temporary cell</returns>
        public static implicit operator ILRetCell(string[] value) {
            ILRetCell ret = createArray(value);
            ret.Storage.FromImplicitCast = true;
            return ret;
        }
        #endregion

        #region conversional operators
        /// <summary>
        /// Convert cell to temporary cell
        /// </summary>
        /// <param name="A">Cell</param>
        /// <returns>Temporary cell</returns>
        public static implicit operator ILRetCell(ILCell A) {
            if (object.Equals(A, null)) return null;
            return A.C;
        }
        /// <summary>
        /// Convert input parameter cell to temporary cell
        /// </summary>
        /// <param name="A">Input parameter cell</param>
        /// <returns>Temporary cell</returns>
        public static implicit operator ILRetCell(ILInCell A) {
            if (object.Equals(A, null)) return null;
            return A.C;
        }
        /// <summary>
        /// Convert output parameter cell to temporary cell
        /// </summary>
        /// <param name="A">Output parameter cell</param>
        /// <returns>Temporary cell</returns>
        public static implicit operator ILRetCell(ILOutCell A) {
            if (object.Equals(A, null)) return null;
            return A.C;
        }
        /// <summary>
        /// Wrap single logical array into a new scalar temporary cell
        /// </summary>
        /// <param name="A">Logical array</param>
        /// <returns>Scalar cell</returns>
        public static implicit operator ILRetCell(ILBaseLogical A) {
            if (object.Equals(A, null)) return null;
            ILRetCell ret = ILMath.cell(A);
            ret.Storage.FromImplicitCast = true;
            return ret;
        }
        #endregion
        #endregion

        #region helper functions
        private static ILRetCell createScalar<T>(T value) {
            using (ILScope.Enter()) {
                ILCell ret = new ILCell(1, 1);
                ret.SetValue(new ILDenseStorage<T>(new T[] { value }, ILSize.Scalar1_1), 0, 0);
                return ret;
            }
        }
        private static ILRetCell createArray<T>(T[] value) {
            using (ILScope.Enter()) {
                ILCell ret = new ILCell(1, 1);
                ret.SetValue(new ILDenseStorage<T>(value, new ILSize(1, value.Length)), 0, 0);
                return ret;
            }
        }
        #endregion helper functions

        #region public interface
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
        /// Concatenate this cell 
        /// </summary>
        /// <param name="A">Cell to concatenate this cell with</param>
        /// <param name="dim">Dimension index along which to concatenate the cells.</param>
        /// <returns>new cell with concatenation of all elements of both cells</returns>
        /// <remarks>The array returned will be a copy of both cells involved. 
        /// If <paramref name="dim"/> is larger than 
        /// the number of dimensions of one of the arrays its value will be used in modulus. 
        /// <para>The resulting cell has the size of both input cells, laid beside one 
        /// another along the <paramref name="dim"/> dimension.</para></remarks>
        public new ILRetCell Concat(ILInCell A, int dim) {
            using (ILScope.Enter(this))
            using (ILScope.Enter(A))
                return base.Concat(A, dim);
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
        internal new void ExportValues(ref ILStorage[] outArray) {
            using (ILScope.Enter(this))
                Storage.ExportValues(ref outArray);
        }
        /// <summary>
        /// Retrieve a single array of a known type from a cell position
        /// </summary>
        /// <typeparam name="T">Element type of the array</typeparam>
        /// <param name="indices">Position of the array within this cell</param>
        /// <returns>Lazy, shallow clone of the array found at the given position</returns>
        public new ILRetArray<T> GetArray<T>(params ILBaseArray[] indices) {
            using (ILScope.Enter(this))
            using (ILScope.Enter(indices))
                return base.GetArray<T>(indices);
        }
        /// <summary>
        /// Retrieve a single element from the given position
        /// </summary>
        /// <param name="indices">Position of the element to retrieve, must evaluate to a scalar position</param>
        /// <returns>A clone of the scalar element found</returns>
        /// <remarks>
        /// <para>The method returns a lazy, shallow clone of the content of the cell element specified by <paramref name="indices"/>. 
        /// However, the return type (<code>ILBaseArray</code>) is not strongly typed and may contain <b>any</b> element. According to the 
        /// true element stored in the cell, this might be an array of arbitrary type, null or even another cell. Moreover, handling 
        /// <code>ILBaseArray</code> directly is not recommended for ILNumerics, since this would hinder the memory management from proper 
        /// functioning. Therefore: <b>The use of this method is not recommended and left to ILNumerics experts - for very 
        /// specific and rare situations.</b></para>
        /// </remarks>
        public new ILBaseArray GetBaseArray(params ILBaseArray[] indices) {
            using (ILScope.Enter(this))
            using (ILScope.Enter(indices))
                return base.GetBaseArray(indices);
        }
        /// <summary>
        /// Retrieve a subcell of this cell
        /// </summary>
        /// <param name="indices">Subcell definition, arbitrary size</param>
        /// <returns>A cell with a lazy, shallow clone of the elements of this cell, addressed by <paramref name="indices"/></returns>
        /// <remarks>The cell returned will have the size and shape specified by <paramref name="indices"/>.</remarks>
        public new ILRetCell GetCell(params ILBaseArray[] indices) {
            using (ILScope.Enter(this))
            using (ILScope.Enter(indices)) {
                return new ILRetCell((ILCellStorage)Storage.Subarray(indices));
            }
        }
        /// <summary>
        /// Enumerator returning elements as scalar cells
        /// </summary>
        /// <returns>Enumerator</returns>
        /// <remarks>This method enables the use of cells in foreach loops.
        /// <para>The iterator is returned, if arrays are directly used in foreach statements. The iterator 
        /// is compatible with ILNumerics memory management.</para></remarks>
        /// <example><code>ILDenseStorage&lt;T&gt; A = ILMath.rand(5,4,6);
        /// foreach (double element in A) {
        /// // all elements are scalar double values
        /// String.Format("Element: {0} ",element);
        /// // Note: 'element' cannot be used to alter the collection! 
        /// } 
        /// </code></example> 
        public new IEnumerator<ILRetCell> GetEnumerator() {
            using (ILScope.Enter(this)) {
                return base.GetEnumerator();
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
        internal new bool GetLimits(out ILStorage min, out ILStorage max) {
            using (ILScope.Enter(this))
                return base.GetLimits(out min, out max);
        }
        /// <summary>
        /// Retrieve single element from this cell
        /// </summary>
        /// <param name="idx">Position of the element</param>
        /// <returns>Lazy, shallow clone of the element to retrieve or null, if there is no element at this place</returns>
        public new object GetValue(params int[] idx) {
            using (ILScope.Enter(this)) {
                return base.GetValue(idx);
            }
        }
        /// <summary>
        /// Retrieve a typed single element from within the cell, supports deep indexing
        /// </summary>
        /// <typeparam name="T">Expected type of the value to be returned</typeparam>
        /// <param name="indices">Location of the single element addressed</param>
        /// <returns>A clone of the single element addressed by <paramref name="indices"/></returns>
        /// <remarks>The element returned will have the type given by <typeparamref name="T"/>. It is an error to specify 
        /// a different type as the true type of the element specified. An exception is thrown if both types differ.</remarks>
        public new T GetValue<T>(params int[] indices) {
            using (ILScope.Enter(this)) {
                return Storage.GetValue<T>(indices);
            }
        }
        /// <summary>
        /// Test if an element of the cell is an array of the given element type
        /// </summary>
        /// <typeparam name="T">The array element type to check the cell element against</typeparam>
        /// <param name="position">Position of the cell element to be tested</param>
        /// <returns>true if the element found at the given position is an array of the element type <typeparamref name="T"/>, false otherwise</returns>
        /// <remarks>The method is helpful in order to investigate the contents of a cell array. If you are not sure about the 
        /// types of elements in the cell, this function can be used to make sure, elements are of the expected type before retrieving them as such.
        /// <para>In most situations, elements of a cell are stored arrays of a distinct element type. That element type is given to IsTypeOf as 
        /// typeparameter <typeparamref name="T"/>. That means, in order to find out, if the first cell element stores an array of int (<code>ILArray&lt;int></code>), 
        /// one may use <code>cell.IsTypeOf&lt;int>(0)</code></para>
        /// <para>In order to test, if a cell element is of type <code>ILCell</code>, one can provide the type <code>ILCell</code> as type parameter: 
        /// <code>cell.IsTypeOf&lt;ILCell>(0)</code>. Note the different semantic when checking for cell elements of type cell. Here we do not test for the 
        /// element type but for the array type itself, ie. <code>ILCell</code>. The reason of this is: the type of elements of <code>ILCell</code> is 
        /// an implementation detail and therefore hidden to the user.</para>
        /// </remarks>
        /// <example>
        /// <para>In the following example a ILCell of size 3x2 is created. It stores several array types, among which other cells are stored as elements of the outer cell.</para>
        /// <code>ILCell cell = ILMath.cell(new ILSize(3, 2) 
        ///                      , "first element"
        ///                      , 2.0
        ///                      , ILMath.cell(Math.PI, 100f)
        ///                      , ILMath.create&lt;short>(1, 2, 3, 4, 5, 6)
        ///                      , new double[] {-1.4, -1.5, -1.6});
        /// </code>
        /// The cell is now: 
        /// <code>ILCell [3,2]
        ///          &lt;String>      first element  &lt;Int16> [2,3,4,5,6] 
        ///          &lt;Double>          2          ILCell [1,3]           
        ///          ILCell [2,1]                                    (null)	
        /// </code>
        /// We test the element type of every element in the cell: 
        /// <code>
        /// Console.Out.WriteLine("cell[0,0] is of type 'string': {0}", cell.IsTypeOf&lt;string>(0));
        /// Console.Out.WriteLine("cell[0,0] is of type 'double': {0}", cell.IsTypeOf&lt;double>(0));
        ///                                      
        /// Console.Out.WriteLine("cell[1,0] is of type 'double': {0}", cell.IsTypeOf&lt;double>(1));
        /// Console.Out.WriteLine("cell[2,0] is of type 'ILCell': {0}", cell.IsTypeOf&lt;ILCell>(2));
        ///                                                                         
        /// Console.Out.WriteLine("cell[0,1] is of type 'short': {0}", cell.IsTypeOf&lt;short>(0, 1));
        /// Console.Out.WriteLine("cell[1,1] is of type 'ILCell': {0}", cell.IsTypeOf&lt;ILCell>(1, 1));
        /// Console.Out.WriteLine("cell[2,1] is of type 'double': {0}", cell.IsTypeOf&lt;double>(2, 1));
        /// </code>
        /// This gives the following output: 
        /// <code>
        /// cell[0,0] is element type 'string': True
        /// cell[0,0] is element type 'double': False
        /// cell[1,0] is element type 'double': True
        /// cell[2,0] is element type 'ILCell': True
        /// cell[0,1] is element type 'short': True
        /// cell[1,1] is element type 'ILCell': True
        /// cell[2,1] is element type 'double': False  // element is null, IsTypeOf&lt;> never gives true
        /// </code></example>
        public new bool IsTypeOf<T>(params ILBaseArray[] position) {
            using (ILScope.Enter(this))
            using (ILScope.Enter(position)) {
                return Storage.IsTypeOf<T>(position);
            }
        }
        /// <summary>
        /// Create replication of this cell
        /// </summary>
        /// <param name="dims">Dimensions specifier. If the number of elements in <paramref name="dims"/> is 
        /// less than the number of dimensions in this cell, the trailing dimensions will 
        /// be set to 1 (singleton dimensions). On the other hand, if the number specified 
        /// is larger then the number of dimension stored inside the storge the resulting 
        /// storage will get its number of dimensions extended accordingly. </param>
        /// <returns>Array being created by multiple replications of this array along 
        /// arbitrary dimensions according to <paramref name="dims"/></returns>
        public new ILRetCell Repmat(params int[] dims) {
            using (ILScope.Enter(this))
                return new ILRetCell((ILCellStorage)Storage.Repmat(dims));
        }
        /// <summary>
        /// Create reshaped copy of this cell
        /// </summary>
        /// <param name="dimensions">New dimensions of the cell</param>
        /// <returns>Reshaped copy of the cell</returns>
        /// <remarks><para>The current instance will not be changed! A new cell is created, having 
        /// the elements of this cell and a shape as determined by <paramref name="dimensions"/>.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the number of elements in 
        /// <paramref name="dimensions"/> do not match the number of elements in this cell.</exception>
        public new ILRetCell Reshape(ILSize dimensions) {
            using (ILScope.Enter(this)) {
                return base.Reshape(dimensions);
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
        /// Dimension shifted cell from this cell
        /// </summary>
        /// <param name="shift">Number of dimensions to shift</param>
        /// <returns>Shifted version of this cell</returns>
        /// <remarks><para>The shift is done 'to the left':</para>
        /// <example><code>ILCell A = cell(2,4); 
        /// ILCell B = A.Shifted(1);
        /// // B is now: ILCell [4,2] 
        /// 
        /// ILCell C = cell(2,4,3);
        /// ILCell D = C.Shifted(1);
        /// // D is now: ILCell [4,3,2]
        /// </code></example>
        /// <para>The dimensions are shifted circulary to the left. This 
        /// can be imagined as removing the first dimensions from the beginning of the list of 
        /// dimensions and "append" them to the end in a ringbuffer style. </para>
        /// <para>For dimension shifts of '1', you may consider using the 
        /// <see cref="ILNumerics.ILDenseArray{ElementType}.T"/> property for readability.</para>
        /// <para><paramref name="shift"/> must be positive. It is taken modulus the number of dimensions.</para>
        /// <seealso cref="ILNumerics.ILDenseArray{ElementType}.T"/></remarks>
        public new ILRetCell Shifted(int shift) {
            using (ILScope.Enter(this))
                return new ILRetCell((ILCellStorage)Storage.ShiftDimensions(shift));
        }
        /// <summary>
        /// Subarray access (readonly) 
        /// </summary>
        /// <param name="indices">Address range</param>
        /// <returns>Reference cell array with subarray addressed by <c>indices</c>. </returns>
        /// <remarks>Query access: for N-dimensional cell arrays missing trailing dimensions indices will be choosen to be 0. Therefore you 
        /// may ommit those trailing dimensions in <c>indices</c>.
        /// <para>The indexer may be used for querying or altering single/any elements 
        /// in this cell. <c>indices</c> may contains index specifications for one to any 
        /// dimension. The cell array returned will have the size specified by <c>indices</c>.</para>
        /// <para>Values returned will be reference cells. All elements contained will be 'deep references' created by 
        /// recursively walking downwards the elements and replacing them by references to itself. Therefore altering the 
        /// values returned will not alter the original elements stored in the cell.</para>
        /// </remarks>
        public new ILRetCell Subarray(params ILBaseArray[] indices) {
            using (ILScope.Enter(this))
            using (ILScope.Enter(indices)) {
                ILCellStorage elements = (ILCellStorage)Storage.Subarray(indices);
                return new ILRetCell(elements);
            }
        }
        /// <summary>
        /// Send values of this instance to stream
        /// </summary>
        /// <param name="stream">Stream to write the values into.</param>
        /// <param name="format">Format string to be used for output. See <see cref="System.String.Format(string,object)"/> for a specification
        /// of valid formating expressions. This flag is only used, when 'method' is set to 'Serial'.</param>
        /// <param name="method">A constant out of <see cref="ILArrayStreamSerializationFlags"/>. Specifies the way in which
        /// the values will be serialized.</param>
        /// <remarks><para>If the 'Formatted' method is used, any occurences of the NewLine character(s) 
        /// will be replaced from the format string before applying to the elements. This is done to 
        /// prevent the format from breaking the 'page' style of the output.</para>
        /// <para>Writing cells to *.mat files is NOT SUPPORTED yet! </para></remarks>
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
                            ILDenseStorage<ILStorage> temp = this.Storage.ShiftDimensions(1);
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

        #region Index access
        /// <summary>
        /// Get/set/remove single element 
        /// </summary> 
        /// <paramref name="indices" value="Index to element"/>
        /// <value>Inner element, new inner element or null</value>
        /// <remarks>The type of access depends on the length of indices. If indices contains only one element, 
        /// the array will be accessed via sequential index access. This is sometimes called referred to as 'linear' 
        /// index addressing. 
        /// Sequential index access reflects the index of internal storage the way the data are actually organized 
        /// in memory. This access method is mainly convinient for vectors where you are not interested of orientation.
        /// The following example demonstrates sequential index access for ILArray's (which also holds for ILCells): 
        /// <example><code>
        /// ILArray&lt;double&gt; A = ILMath.counter(1,12);
        /// A[2] gives: 3.0
        /// </code>But the transpose 
        /// <code>
        /// A.T[2] gives also: 3.0
        /// </code> 
        /// For matrices and N-dimensional arrays this holds as well: 
        /// <code>
        /// ILArray&lt;double&gt; A = ILMath.counter(1.0,1.0,3,2,2);
        /// A = 
        /// [1.0 4.0
        ///  2.0 5.0 
        ///  3.0 6.0 
        /// 
        ///  7.0 10.0
        ///  8.0 11.0
        ///  9.0 12.0]
        /// 
        /// A = ILMath.Reshape(A,3,2,2); 
        /// A[10] gives 11.0
        /// A[10,1] gives ILArgumentException -> out of range
        /// A[2,1,1] gives 12.0
        /// A[2,1] gives 6.0 (set trailing dimension to '0')</code></example>
        /// <para>If the element addressed is a ILCell itself, a deep reference to this element will be returned instead. 
        /// I.e. all elements of the ILCell will be recursively replaced with references to itself. Therefore, altering the 
        /// elements returned will not alter the elements contained in the cell.</para>
        /// <para>
        /// <list type="bullet">
        /// <listheader>The type of the element returned depends on the type of the element addressed:</listheader>
        /// <item>For ILArray&lt;ElementType&gt; the array returned will be a clone of the original array.</item> 
        /// <item>For ILCell the ILBaseArray returned is a deep reference of the original elements stored.</item>
        /// <item>For other types the behavior is undefined. (since other types are not implemented yet ;)</item>
        /// </list> </para>
        /// <para>This indexer may also be used for direct access to inner elements of (elements of elements of ...) this cell:
        /// <example>
        /// <code>
        /// ILCell innerCell = new ILCell(2,1); 
        /// innerCell[0] = ILMath.vec(10,200); 
        /// innerCell[1] = new int[] {-10,-20,-30};
        /// ILCell cell = new ILCell(2,1); 
        /// cell[0] = innerCell; 
        /// cell[1] = new string[] {"foobla"}; 
        /// // cell is now: 
        /// // [ILCell,(1x2)] 
        /// //      [innerCell[0], ILArray&lt;double&gt;(1x181)]
        /// //      [innerCell[0], ILArray&lt;double&gt;(1x3)]
        /// // [ILArray&lt;string&gt;,(1x1)]
        /// 
        /// cell[0,0] -&gt; will give innerCell eq. ILCell (1x2)
        /// cell[0,1] -&gt; will give ILArray&lt;string&gt;
        /// cell[0,0,0,1] -&gt; will give innerCell[1] eq. ILArray&lt;int&gt;(1x3)
        /// </code>
        /// </example> 
        /// In the last example above the trailing indices specified make the indexer walk down into the ILCell element and retrieve 
        /// the content of this element. This kind of index access may be done as deep as you want. Just 
        /// append the inner indices into inner elements to the right side of index specification. Addressing inner elements 
        /// this way is the only way to alter elements <b>directly</b> inside the ILCell. </para></remarks>
        public ILRetCell this[params int[] indices] {
            get {
                using (ILScope.Enter(this)) {
                    ILStorage val = Storage.GetValueTyped(indices);
                    if (val is ILCellStorage)
                        return new ILRetCell((ILCellStorage)val);
                    else
                        return new ILRetCell(new ILStorage[] { val }, ILSize.Scalar1_1);
                }
            }
        }

        /// <summary>
        /// Subarray access. Get/set regular subarray. 
        /// </summary>
        /// <param name="indices">Address range</param>
        /// <returns>Reference cell array with subarray addressed by <c>indices</c>. </returns>
        /// <remarks>Query access: for N-dimensional cell arrays missing trailing dimensions indices will be choosen to be 0. Therefore you 
        /// may ommit those trailing dimensions in <c>indices</c>.
        /// <para>The indexer may be used for querying or altering single/any elements 
        /// in this cell. <c>indices</c> may contains index specifications for one to any 
        /// dimension. The cell array returned will have the size specified by <c>indices</c>.</para>
        /// <para>Values returned will be reference cells. All elements contained will be 'deep references' created by 
        /// recursively walking downwards the elements and replacing them by references to itself. Therefore altering the 
        /// values returned will not alter the original elements.</para>
        /// <para>The indexer may also be used for removing parts of the cell. Therefore null must be assigned to the range specified by <c>indices</c> (using the set-access). <c>indices</c> 
        /// must contain exactly one dimension specification other than 'full' in this case. This may be any vector-sized numeric ILArray of any 
        /// numeric type. If <c>indices</c> apply to fewer dimensions than the number of dimensions existing, the upper dimensions will be 
        /// merged and the array will be reshaped before applying the removal to it.
        /// <example>
        /// <code>
        /// ILCell C = new ILCell(4,10); 
        /// C[":",2] = null;  // &gt;- will remove the third column (index: 2) from the cell.
        /// C[full,vec(2,5)] = null;  &gt;- will remove columns 3...6
        /// C[1,1] = null; &gt;- will produce an error. Only one dimension can be specified not full! 
        /// </code></example></para>
        /// <para>The general behavior of this access methods is full compatible with the corresponding Matlab/Octave/Scilab access: a(:) = []. </para>
        /// </remarks>
        public ILRetCell this[params ILBaseArray[] indices] {
            get {
                using (ILScope.Enter(this))
                using (ILScope.Enter(indices)) {
                    return Subarray(indices);
                }
            }
        }

        #endregion index access

    }
}
