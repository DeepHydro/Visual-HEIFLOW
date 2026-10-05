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

namespace ILNumerics {
    /// <summary>
    /// The class implements additional functionality needed for cells
    /// </summary>
    [System.Security.SecuritySafeCritical]
    public class ILBaseCell : ILDenseArray<ILStorage> {

        #region constructors
        internal ILBaseCell(ILCellStorage cellStorage, bool tempArr) 
            : base(cellStorage, tempArr) { }
        #endregion constructors

        #region properties
        /// <summary>
        /// Transposed version of this ILCell
        /// </summary>
        /// <remarks>This property is an alias for 'Shifted(1). </remarks>
        /// <seealso cref="ILNumerics.ILBaseCell.Shifted(int)"/>
        public new ILRetCell T {
            get {
                return new ILRetCell((ILCellStorage)Storage.ShiftDimensions(1));
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
        /// <summary>
        /// Clone of this cell
        /// </summary>
        /// <remarks><para>
        /// Clones of all arrays in ILNumerics.Server are done in a very fast, lazy way. This means, 
        /// at the time the clone is made, no relevant memory is copied. Elements of both arrays rather point to the same 
        /// underlying System.Array. A reference counting mechanism ensures the detaching of thoses arrays on write access.</para>
        /// <para>Cells profit from the same efficient clone creation process. However, since a cell may store an arbitrarily deep 
        /// hirarchy of other cells and arrays, in order to clone a cell, the cells elements have to be cloned as well - in an 
        /// recursive manner. Clones play an important role for ILNumerics cells. They are used to implement value semantics for cell
        /// elements. I.e.: the cloned cell returned cannot not be used to alter elements of the original cell in any way.</para></remarks>
        public new ILRetCell C {
            get {
                return new ILRetCell((ILCellStorage)Storage.Clone());
            }
        }
        #endregion

        #region public interface
        /// <summary>
        /// Concatenate this cell 
        /// </summary>
        /// <param name="A">Cell to concatenate this cell with</param>
        /// <param name="dim">Dimension index along which to concatenate the cells.</param>
        /// <returns>New cell with concatenation of all elements of both cells</returns>
        /// <remarks>The array returned will be a copy of both cells involved. 
        /// If <paramref name="dim"/> is larger than 
        /// the number of dimensions of one of the arrays its value will be used in modulus. 
        /// <para>The resulting cell has the size of both input cells, laid beside one 
        /// another along the <paramref name="dim"/> dimension.</para></remarks>
        public ILRetCell Concat(ILInCell A, int dim) {
            using (ILScope.Enter(A))
                return new ILRetCell((ILCellStorage)Storage.Concat(A.Storage, dim));
        }
        /// <summary>
        /// Retrieve a single array of a known type from a cell position
        /// </summary>
        /// <typeparam name="T">Element type of the array</typeparam>
        /// <param name="indices">Position of the array within this cell</param>
        /// <returns>Lazy, shallow clone of the array found at the given position</returns>
        public ILRetArray<T> GetArray<T>(params ILBaseArray[] indices) {
            using (ILScope.Enter(indices))
                return new ILRetArray<T>(Storage.GetDenseStorage<T>(indices));
        }
        /// <summary>
        /// For EXPERTS only! Retrieve a single element from the given position 
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
        internal ILBaseArray GetBaseArray(params ILBaseArray[] indices) {
            using (ILScope.Enter(indices))
                return Storage.GetScalar(indices);
        }
        /// <summary>
        /// Retrieves a subcell of this cell
        /// </summary>
        /// <param name="indices">Subcell definition, arbitrary size</param>
        /// <returns>A cell with a lazy, shallow clone of the elements of this cell, addressed by <paramref name="indices"/></returns>
        /// <remarks>The cell returned will have the size and shape specified by <paramref name="indices"/>.</remarks>
        public ILRetCell GetCell(params ILBaseArray[] indices) {
            using (ILScope.Enter(indices)) {
                ILCellStorage retStorage = (ILCellStorage)Storage.Subarray(indices);
                if (retStorage.Size.NumberOfElements == 1) {
                    ILCellStorage retInner = retStorage.GetValueTyped(0) as ILCellStorage;
                    if (retInner == null) 
                        throw new Exceptions.ILArgumentException("no cell found at the specified position");
                    return new ILRetCell(retInner); 
                } else {
                    throw new Exceptions.ILArgumentException("index specification must resolve to a scalar element location");
                }
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
            int len = Size.NumberOfElements;
            for (int i = 0; i < len; i++) {
                //yield return GetBaseArray(i);
                yield return new ILRetCell((ILCellStorage)Storage.Subarray(i));
            }
        }
        /// <summary>
        /// Retrieve single element from this cell
        /// </summary>
        /// <param name="idx">Position of the element</param>
        /// <returns>Lazy, shallow clone of the element to retrieve or null, if there is no element at this place</returns>
        public new object GetValue(params int[] idx) {
            return Storage.GetValue(idx); 
        }
        /// <summary>
        /// Retrieve a typed single element from within the cell, supports deep indexing
        /// </summary>
        /// <typeparam name="T">Expected type of the value to be returned</typeparam>
        /// <param name="indices">Location of the single element addressed</param>
        /// <returns>A clone of the single element addressed by <paramref name="indices"/></returns>
        /// <remarks>The element returned will have the type given by <typeparamref name="T"/>. It is an error to specify 
        /// a different type as the true type of the element specified. An exception is thrown if both types differ.</remarks>
        public T GetValue<T>(params int[] indices) {
            return Storage.GetValue<T>(indices);
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
        public bool IsTypeOf<T>(params ILBaseArray[] position) {
            using (ILScope.Enter(position))
                return Storage.IsTypeOf<T>(position);
        }
        /// <summary>
        /// Create reshaped copy of this cell
        /// </summary>
        /// <param name="size">New size of the cell</param>
        /// <returns>Reshaped copy of the cell</returns>
        /// <remarks><para>The current instance will not be changed! A new cell is created, having 
        /// the elements of this cell and a shape as determined by <paramref name="size"/>.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the number of elements in 
        /// <paramref name="size"/> do not match the number of elements in this cell.</exception>
        public new ILRetCell Reshape(ILSize size) {
            ILCell ret = C;
            ret.Storage.Reshape(size);
            return ret;
        }
        /// <summary>
        /// Create reshaped copy of this cell
        /// </summary>
        /// <param name="size">New size of the cell</param>
        /// <returns>Reshaped copy of the cell</returns>
        /// <remarks><para>The current instance will not be changed! A new cell is created, having 
        /// the elements of this cell and a shape as determined by <paramref name="size"/>.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the number of elements in 
        /// <paramref name="size"/> do not match the number of elements in this cell.</exception>
        public new ILRetCell Reshape(params int[] size) {
            return Reshape(new ILSize(size));
        }
        /// <summary>
        /// Create replication of this cell
        /// </summary>
        /// <param name="dims">Size descriptor
        /// <remarks>If the number of elements in <paramref name="dims"/> is 
        /// less than the number of dimensions in this cell, the trailing dimensions will 
        /// be set to 1 (singleton dimensions). On the other hand, if the number specified 
        /// is larger then the number of dimension stored inside the storge the resulting 
        /// storage will get its number of dimensions extended accordingly. </param></remarks>
        /// <returns>Array created by multiple replications of this array along 
        /// arbitrary dimensions according to <paramref name="dims"/></returns>
        public new ILRetCell Repmat(params int[] dims) {
            return new ILRetCell((ILCellStorage)Storage.Repmat(dims));
        }        
        /// <summary>
        /// Dimension shifted cell from this cell
        /// </summary>
        /// <param name="shift">Number of dimensions to shift</param>
        /// <returns>Shifted version of this cell</returns>
        /// <remarks><para>The shift is done 'to the left':</para>
        /// <example><code>ILCell A = cell(2,4); <br/>
        /// ILCell B = A.Shifted(1); <br/>
        /// // B is now: ILCell [4,2] <br/>
        /// // <br/>
        /// ILCell C = cell(2,4,3);<br/>
        /// ILCell D = C.Shifted(1); <br/>
        /// // D is now: ILCell [4,3,2] <br/>
        /// </code></example>
        /// <para>The dimensions are shifted circulary to the left. This 
        /// can be imagined as removing the first dimensions from the beginning of the list of 
        /// dimensions and "append" them to the end in a ringbuffer style. </para>
        /// <para>For dimension shifts of '1', you may consider using the 
        /// <see cref="ILNumerics.ILDenseArray{ElementType}.T"/> property for readability.</para>
        /// <para><paramref name="shift"/> must be positive. It is taken modulus the number of dimensions.</para>
        /// <seealso cref="ILNumerics.ILDenseArray{ElementType}.T"/></remarks>
        public new ILRetCell Shifted(int shift) {
            return new ILRetCell((ILCellStorage)Storage.ShiftDimensions(shift));
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
        public new ILRetCell Subarray(params ILBaseArray[] indices) {
            using (ILScope.Enter(indices)) {
                ILCellStorage elements = (ILCellStorage)Storage.Subarray(indices);
                return new ILRetCell(elements);
            }
        }
        #endregion

    }
}
