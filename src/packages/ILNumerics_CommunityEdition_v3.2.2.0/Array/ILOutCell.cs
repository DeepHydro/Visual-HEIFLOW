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
using System.Text; 
using ILNumerics.Storage; 
using ILNumerics.Exceptions;
using System.Collections.Generic; 

namespace ILNumerics {
    /// <summary>
    /// Mathematical container class holding arbitrary array objects as elements 
    /// </summary>
    /// <remarks>
    /// Cells act as general purpose container. Cells store arbitrary arrays of arbitrary element type. 
    /// Theses elements commonly are cells itself. <c>ILOutCell</c> acts as output parameter 
    /// in function declarations when a function needs to return multiple (cell) arrays. 
    /// Use <see cref="ILNumerics.ILCell"/> for local cell variables in your algorithms. 
    /// <para>More details about cells are found in the <a href="http://ilnumerics.net/$Cells.html">online manual</a>.</para>
    /// </remarks>
    /// <seealso cref="ILNumerics.ILCell"/>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public sealed class ILOutCell : ILBaseCell {

        #region attributes
        ILCell m_originalCell;
        private static readonly bool s_isTempArray = false;
        #endregion

        #region properties
        /// <summary>
        /// Replace the elements of this array with another array's elements, preventing memory leaks
        /// </summary>
        /// <param name="value">New array</param>
        public ILRetCell a {
            set { Assign(value); }
            get { return this.C; }
        }
        #endregion

        #region constructors

        /// <summary>
        /// do not use this constructor! Out arrays are to be created implicitely only!
        /// </summary>
        /// <param name="cellStorage">storage of source cell</param>
        internal ILOutCell(ILCellStorage cellStorage)
            : base(cellStorage, s_isTempArray) { }

        #endregion constructors

        #region implicit casts 
        /// <summary>
        /// Implicitely convert persistent cell to output parameter type cell 
        /// </summary>
        /// <param name="A">Original cell</param>
        /// <returns>Output parameter cell</returns>
        public static implicit operator ILOutCell(ILCell A) {
            if (object.Equals(A, null))
                return null;
            ILOutCell ret = new ILOutCell(A.Storage);
            ret.m_originalCell = A; 
            return ret;
        }
        #endregion

        #region public interface
        /// <summary>
        /// Replaces storage of this array with new array elements, registers this array for out-of-scope disposal
        /// </summary>
        /// <param name="value">New array</param>
        public void Assign(ILRetCell value) {
            if (!IsDisposed)
                Storage.Dispose();
            ILCellStorage storage = (ILCellStorage)value.GiveStorageAwayOrClone();
            m_storage = storage;
            if (!ILMath.isnull(m_originalCell)) {
                (m_originalCell as ILDenseArray<ILStorage>).Storage = storage;
            }
            //ILScope.Context.RegisterArray(this);  
        }
        /// <summary>
        /// Set single element of the cell
        /// </summary>
        /// <param name="value">The new value</param>
        /// <param name="idx">Indices specifying the location to set the element to</param>
        /// <remarks>The function supports the following features: 
        /// <list type="bullet">
        /// <item>Automatic expansion of the cell, when addressing an element outside of the cells size limits.</item>
        /// <item>Before storing the new element into the cell, an old element may existing on the same location gets disposed.</item>
        /// <item>A clone of the new value is stored, therefore, none of the source and the stored element are altered, whenever the other cell is altered (value semantics).</item>
        /// <item>The function supports deep index addressing. This is the only way of altering array elements inside the cell - without recreation.</item>
        /// </list>
        /// <para>Removal of parts of the cell is <b>not</b> supported. If null or an empty array is provided as <paramref name="value"/>, the corresponding 
        /// element is overwritten or removed.</para>
        /// </remarks>
        public void SetValue(ILBaseArray value, params int[] idx) {
            using (ILScope.Enter(value)) {
                Storage.SetValueTyped(value.Storage, idx);
            }
        }
        /// <summary>
        /// Set single element of the cell
        /// </summary>
        /// <param name="value">The new value</param>
        /// <param name="idx">Indices specifying the location to set the element to</param>
        /// <remarks>The function supports the following features: 
        /// <list type="bullet">
        /// <item>Automatic expansion of the cell, when addressing an element outside of the cells size limits.</item>
        /// <item>Before storing the new element into the cell, an old element may existing on the same location gets disposed.</item>
        /// <item>A clone of the new value is stored, therefore, none of the source and the stored element are altered, whenever the other cell is altered (value semantics).</item>
        /// <item>The function supports deep index addressing. This is the only way of altering array elements inside the cell - without recreation.</item>
        /// </list>
        /// <para>Removal of parts of the cell is <b>not</b> supported. If null or an empty array is provided as <paramref name="value"/>, the corresponding 
        /// element is overwritten or removed.</para>
        /// </remarks>
        internal void SetValue(ILStorage value, params int[] idx) {
            Storage.SetValueTyped(value, idx);
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
        /// <para>Unlike (almost) all other member function of an array, this function
        /// does not keep track of internal memory management. It means, the storage which this array is based 
        /// upon, will not be set free after the function returns. You (as the user of the array) will have to pay 
        /// attention yourself, when to call dispose on the array - if necessary. Also, for elements of reference
        /// types (e.g. ILCell), retrieving and storing elements from/into the System.Array directly does 
        /// not simulate a value semantic as all other functions do! This means, references are copied. Attention
        /// must be paid to dereference / clone elements accordingly. </para></remarks>
        public ILStorage[] GetArrayForWrite() {
            return Storage.GetArrayForWrite();
        }
        #endregion

        #region Index access
        /// <summary>
        /// Get/set/remove single element 
        /// </summary> 
        /// <paramref name="indices" value="index to element"/>
        /// <value>Inner element, new inner element or null</value>
        /// <remarks>The type of access depends on the length of indices. If indices contains only one element, 
        /// the array will be accessed via sequential index access. This is sometimes called referred to as 'linear' 
        /// index addressing. 
        /// Sequential index access reflects the index of internal storage the way the data are actually organized 
        /// in memory. This access method is mainly convinient for vectors where you are not interested of orientation.
        /// The following example demonstrates sequential index access for ILArray's (which also holds for ILCells): 
        /// <example>For <c>ILArray&lt;double&gt; A = ILMath.counter(1,12);</c>, <c>A[2]</c> gives: 3.0.
        /// But the transpose 
        /// <c>A.T[2]</c> gives also: 3.0.
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
        /// this way is the only way to alter elements <b>directly</b> inside the ILCell. </para>
        /// <para>Output parameter type cell carry a reference to the original array they were created from. 
        /// Modifications of outpur parameter type cells are immediately applied to the original array also.</para></remarks>
        public ILRetCell this[params int[] indices] {
            get {
                ILStorage val = Storage.GetValueTyped(indices);
                if (val is ILCellStorage)
                    return new ILRetCell((ILCellStorage)val);
                else
                    return new ILRetCell(new ILStorage[] { val }, ILSize.Scalar1_1);
            }
            set {
                using (ILScope.Enter(value)) {
                    if (!object.Equals(value, null) && value.Storage.FromImplicitCast && value.IsScalar) {
                        SetValue((value as ILDenseArray<ILStorage>).GetValue(0), indices);
                    } else {
                        SetValue((object.Equals(value, null)) ? null : value.Storage, indices);
                    }
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
        /// <para>Output parameter type cell carry a reference to the original array they were created from. 
        /// Modifications of outpur parameter type cells are immediately applied to the original array also.</para></remarks>
        public new ILRetCell this[params ILBaseArray[] indices] {
            get {
                using (ILScope.Enter(indices)) {
                    ILCellStorage elements = (ILCellStorage)Storage.Subarray(indices);
                    return new ILRetCell(elements);
                }
            }
            set {
                using (ILScope.Enter(indices))
                using (ILScope.Enter(value)) {
                    if (Object.ReferenceEquals(value, null)) {
                        Storage.IndexSubrange(null, indices);
                    } else {
                        //if (value.Storage.FromImplicitCast && value.IsScalar) {
                        //    Storage.IndexSubrange((ILDenseStorage<ILStorage>)value.GetValue(0), indices);
                        //} else {
                        Storage.IndexSubrange((ILCellStorage)value.Storage.Clone(), indices);
                        //}
                    }
                }
            }
        }

        #endregion index access

        #region memory management
        internal override bool EnterScope() {
            return false;
        }
        #endregion
    }
}
