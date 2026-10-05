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
using System.Text;
using System.IO; 
using System.Runtime.Serialization; 
using System.Runtime.CompilerServices;
using ILNumerics.Misc;
using ILNumerics.Data;
using ILNumerics.Exceptions;
using System.Linq.Expressions; 

namespace ILNumerics.Storage {

    /// <summary>
    /// The class realizes an internal storage wrapper for cell arrays. It takes care of value semantics for reference types. 
    /// </summary>
    [Serializable]
    internal class ILCellStorage : ILDenseStorage<ILStorage> {

        #region attributes
        bool m_fromImplicitCast = false;
        #endregion

        #region properties
        internal bool FromImplicitCast {
            get { return m_fromImplicitCast; }
            set { m_fromImplicitCast = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// create new cell storage
        /// </summary>
        /// <param name="system_array">countable array</param>
        /// <param name="dimensions">dimensions</param>
        internal ILCellStorage(ILStorage[] system_array, ILSize dimensions) 
            : base(system_array, dimensions) {
                DeepReferenceElements(); 
        }
        internal ILCellStorage(ILBaseArray[] system_array, ILSize dimensions)
            : base(baseArray2Storages(system_array, dimensions.NumberOfElements), dimensions) {     
                DeepReferenceElements();
        }
        internal ILCellStorage(ILCountableArray<ILStorage> countableArray, ILSize dimensions)
            : base(countableArray, dimensions) {
                DeepReferenceElements(); 
        }
        internal ILCellStorage(ILSize dimensions) 
            : base(dimensions) { }
        #endregion

        #region public interface
        /// <summary>
        /// replace array at specified location
        /// </summary>
        /// <param name="value">new scalar value for element</param>
        /// <param name="indices">indices of element to be altered, supports 'deep indexing'</param>
        internal override void SetValueTyped(ILStorage value, params int[] indices) {
            //if (Object.ReferenceEquals(value, null) || value.Size.NumberOfElements == 0)
            //    throw new ILArgumentException("cell removal is not supported for single integer indexing. value specified must not be null");
            int mydimlen = Size.NumberOfDimensions;
            if (indices.Length <= mydimlen) {
                try {
                    // address this ILCell
                    ILStorage curVal = base.GetValueTyped(indices);
                    if (curVal != null)
                        curVal.Dispose();
                    
                } catch (IndexOutOfRangeException) {
                } catch (ILArgumentException) {
                    // address outside range -> no need to dispose 
                } finally {
                    base.SetValueTyped(value == null ? null: value.Clone(), indices);
                }
            } else {
                if (value != null && value.Size.NumberOfElements > 1)
                    throw new ILArgumentException("value given must be scalar or null");
                // address element of an element of this ILCell
                int[] innerIndices = new int[indices.Length - mydimlen];
                int i = 1, elIndex = indices[0];
                for (; i < mydimlen; i++)
                    elIndex += (Size.SequentialIndexDistance(i) * indices[i]);
                if (elIndex > Size.NumberOfElements)
                    throw new ILArgumentException("index out of range");
                for (int n = 0; i < indices.Length; i++)
                    innerIndices[n++] = indices[i];
                ILStorage innerElement = GetArrayForRead()[elIndex];
                if (object.Equals(innerElement, null))
                    throw new ILArgumentException("No array found in cell at indices specified. Invalid deep indices given.");
                if (innerElement is ILCellStorage) {
                    innerElement.SetValue(value, innerIndices);
                } else {
                    if (value != null && value.Size.NumberOfElements != 1)
                        throw new ILArgumentException("deep cell indexing requires the new value to be a scalar. Given: " 
                            + value.Size.ToString()); 
                    innerElement.SetValue(value != null ? value.GetValue(0) : null, innerIndices);
                }
                //if (innerElement is ILDenseStorage<ILStorage>) {  // send the _reference_ of value to inner storage (clone will be done at the end)
                //    (innerElement as ILDenseStorage<ILStorage>).SetValueTyped(value, innerIndices);
                //} else if (value is ILStorage<double> && innerElement is ILStorage<double>) {
                //    (innerElement as ILStorage<double>).SetValueTyped((value as ILStorage<double>).GetValueTyped(0), innerIndices);
                //} else if (value is ILStorage<float> && innerElement is ILStorage<float>) {
                //    (innerElement as ILStorage<float>).SetValueTyped((value as ILStorage<float>).GetValueTyped(0), innerIndices);
                //} else if (value is ILStorage<complex> && innerElement is ILStorage<complex>) {
                //    (innerElement as ILStorage<complex>).SetValueTyped((value as ILStorage<complex>).GetValueTyped(0), innerIndices);
                //} else if (value is ILStorage<fcomplex> && innerElement is ILStorage<fcomplex>) {
                //    (innerElement as ILStorage<fcomplex>).SetValueTyped((value as ILStorage<fcomplex>).GetValueTyped(0), innerIndices);
                //} else if (value is ILStorage<Int32> && innerElement is ILStorage<Int32>) {
                //    (innerElement as ILStorage<Int32>).SetValueTyped((value as ILStorage<Int32>).GetValueTyped(0), innerIndices);
                //} else if (value is ILStorage<Int64> && innerElement is ILStorage<Int64>) {
                //    (innerElement as ILStorage<Int64>).SetValueTyped((value as ILStorage<Int64>).GetValueTyped(0), innerIndices);
                //} else if (value is ILStorage<byte> && innerElement is ILStorage<byte>) {
                //    (innerElement as ILStorage<byte>).SetValueTyped((value as ILStorage<byte>).GetValueTyped(0), innerIndices);
                //} else if (value is ILStorage<string> && innerElement is ILStorage<string>) {
                //    (innerElement as ILStorage<string>).SetValueTyped((value as ILStorage<string>).GetValueTyped(0), innerIndices);
                //} else {
                //    throw new ILArgumentTypeException("right side argument type is not valid in this context. the value must have the same type as the inner array addressed");
                //}
            }
        }
        /// <summary>
        /// get element from this cell storage
        /// </summary>
        /// <param name="indices">indices of element</param>
        /// <returns>element </returns>
        internal override ILStorage GetValueTyped(params int[] indices) {
            int mydimlen = Size.NumberOfDimensions;
            if (indices.Length <= mydimlen) {
                // address this ILCell 
                ILStorage element = base.GetValueTyped(indices);
                return (element != null) ? element.Clone() : null; 
            } else {
                // address element of an element of this ILCell (deep addressing)
                int[] innerIndices = new int[indices.Length - mydimlen];
                int i = 1, elIndex = indices[0];
                for (; i < mydimlen; i++)
                    elIndex += (Size.SequentialIndexDistance(i) * indices[i]);
                for (int n = 0; i < indices.Length; i++)
                    innerIndices[n++] = indices[i];
                ILStorage innerElement = base.GetValueTyped(elIndex);
                if (innerElement == null)
                    throw new ILArgumentException("Deep indices out of range. The specified element does not exist."); 
                return innerElement.GetValueAsStorage(innerIndices); 
            }
        }
        internal override ILStorage GetValueAsStorage(params int[] innerIndices) {
            return GetValueTyped(innerIndices); 
        }
        /// <summary>
        /// test if an element of the cell is an array of the given element type
        /// </summary>
        /// <typeparam name="T">the array element type to check the cell element against</typeparam>
        /// <param name="position">position of the cell element to be tested</param>
        /// <returns>true if the element found at the given position is an array of the element type <typeparamref name="T"/>, false otherwise</returns>
        /// <remarks>The method is helpful in order to investigate the contents of a cell array. If you are not sure about the 
        /// types of elements in the cell, this function can be used to make sure, elements are of the expected type before retrieving them as such.
        /// <para>In most situations, elements of a cell are stored arrays of a distinct element type. That element type is given to IsTypeOf as 
        /// typeparameter <typeparamref name="T"/>. That means, in order to find out, if the first cell element stores an array of int (<c>ILArray&lt;int></c>), 
        /// one may use <c>cell.IsTypeOf&lt;int>(0)</c></para>
        /// <para>In order to test, if a cell element is of cell type, one can provide the type <c>ILCell</c> as type parameter: 
        /// <c>cell.IsTypeOf&lt;ILCell>(0)</c>. Note the different semantic when checking for cell elements of type cell. Here we do not test for the 
        /// element type but for the array type itself, ie. <c>ILCell</c>. The reason of this is: the type of elements of <c>ILCell</c> is 
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
            ILCellStorage cellStorage = (ILCellStorage)Subarray(position);
            try {
                if (cellStorage.Size.NumberOfElements == 1) {
                    ILStorage innerStorage = cellStorage.GetValueTyped(0);
                    try {
                        if (typeof(T) == typeof(ILCell)) {
                            return innerStorage is ILCellStorage;
                        }
                        return (innerStorage is ILStorage<T>);
                    } finally {
                        if (innerStorage != null) {
                            innerStorage.Dispose();
                        }
                    }
                } else {
                    throw new ILArgumentException("indices must define a scalar subarray range");
                }
            } finally {
                cellStorage.Dispose();
            }
        }
        /// <summary>
        /// gives clone of value addressed, supports deep index addressing
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        internal override object GetValue(params int[] indices) {
            int mydimlen = Size.NumberOfDimensions;
            if (indices.Length <= mydimlen) {
                // address this ILCell 
                ILStorage element = base.GetValueTyped(indices);
                return (element != null) ? element.Clone() : null;
            } else {
                // address element of an element of this ILCell (deep addressing)
                int[] innerIndices = new int[indices.Length - mydimlen];
                int i = 1, elIndex = indices[0];
                for (; i < mydimlen; i++)
                    elIndex += (Size.SequentialIndexDistance(i) * indices[i]);
                for (int n = 0; i < indices.Length; i++)
                    innerIndices[n++] = indices[i];
                ILStorage innerElement = base.GetValueTyped(elIndex);
                if (innerElement == null)
                    throw new ILArgumentException("Deep indices out of range. The specified element does not exist.");
                return innerElement.GetValue(innerIndices);
            }
        }
        /// <summary>
        /// get single element from cell storage, predefined element type 
        /// </summary>
        /// <typeparam name="T">predefined type</typeparam>
        /// <param name="indices">location of element to return</param>
        /// <returns>element</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if the element found is not of the specified type</exception>
        public T GetValue<T>(params int[] indices) {
            object obj = GetValue(indices);
            if (obj is T)
                return (T)obj;
            else
                throw new ILArgumentException("the element at specified position is not of requested type or does not exist"); 
        }

        
        /// <summary>
        /// Alter elements of this storage adressed by sequential indices 
        /// </summary>
        /// <param name="indices">array specifying the elements to be altered, sequential indexing</param>
        /// <param name="values">ILBaseArray of the same type than this array, holding the new values. 
        /// The number of elements of storage must match the 
        /// number of elements of indices. The only exception to this rule is if 'values' is a scalar array. The 
        /// single value of 'values' is than used to set all elements addressed by 'indices'.</param>
        /// <remarks><para>For empty arrays, scalar or vectors, indices outside the current bounds for 
        /// this array will expand this array to the size neccessary. 
        /// For other arrays the sequential indices given must fit inside this arrays dimensions. </para></remarks>
        public override void SetRange(ILBaseArray<double> indices, ILDenseStorage<ILStorage> values) {
            //if (object.Equals(indices,null))
            //    throw new ILArgumentException("indices given must not be null");
            if (object.Equals(values, null))
                throw new ILArgumentException("values given must not be null. Use A[ind] = null; if removal was intended.");
            #region set full shortcut
            // if (indices is ILFullRange || object.Equals(indices, null)) {
            if (object.Equals(indices, null))
            {
                int pos;
                ILStorage[] myArray = GetArrayForWrite();
                if (values.Size.NumberOfElements == 1) {
                    pos = Size.NumberOfElements;
                    ILStorage val = values.GetValueTyped(0);
                    while (pos-- > 0) {
                        if (myArray[pos] != null)
                            myArray[pos].Dispose();
                        myArray[pos] = val;
                    }
                } else {
                    if (values.Size.NumberOfElements != Size.NumberOfElements)
                        throw new ILArgumentException("number of elements in source must match number of elements in destination array");
                    pos = 0;
                    foreach (ILStorage val in values) {
                        if (myArray[pos] != null)
                            myArray[pos].Dispose();
                        myArray[pos++] = val;
                    }
                }
            } else if (indices.Size.NumberOfElements != values.Size.NumberOfElements
                && values.Size.NumberOfElements != 1)
                throw new ILArgumentException("number of elements in source must match number of elements in destination array");
            #endregion
            if (indices.IsEmpty)
                return;
            if (indices.Storage is ILDenseStorage< double>) {
                
                double maxIndex, minIndex;
                ILDenseStorage< double> indStorage = indices.Storage as ILDenseStorage< double>;
                indStorage.GetLimits(out minIndex, out maxIndex);
                if (minIndex < 0)
                    throw new ILArgumentException("sequential indices can not be negative");
                if ((int)maxIndex >= m_size.NumberOfElements) {
                    // handle resize
                    if (m_size.NumberOfDimensions == 2) {
                        if (m_size.NumberOfElements <= 1) {
                            // scalar or empty -> expand along 1 
                            ExpandArray(new int[] { (int)maxIndex + 1, 1 });
                        } else if (m_size[0] > 1 && m_size[1] == 1) {
                            ExpandArray(new int[] { (int)maxIndex + 1, 1 });
                        } else if (m_size[0] == 1 && m_size[1] > 1) {
                            ExpandArray(new int[] { 1, (int)maxIndex + 1 });
                        } else
                            throw new ILArgumentException("resizing array via sequential index access is supported for empty, scalar or vector only");
                    } else
                        throw new ILArgumentException("resizing array via sequential index access is supported for empty, scalar or vector only");
                }
                ILStorage[] myArray = GetArrayForWrite();
                
                double[] indArray = indStorage.GetArrayForRead();

                int len = indices.Size.NumberOfElements;
                int ind; 
                if (values.Size.NumberOfElements == 1) {
                    ILStorage scalarElement = values.GetValueTyped(0);
                    for (int i = 0; i < len; i++) {
                        ind = (int)indArray[i];
                        if (myArray[ind] != null)
                            myArray[ind].Dispose(); 
                        myArray[ind] = scalarElement;
                    }
                } else {
                    ILStorage[] valArray = values.GetArrayForRead();
                    for (int i = 0; i < len; ) {
                        ind = (int)indArray[i];
                        if (myArray[ind] != null)
                            myArray[ind].Dispose();
                        myArray[(int)indArray[i]] = valArray[i++];
                    }
                }
            } else {
                throw new ILArgumentException("storage type of given indices is not supported");
            }
        }

#region HYCALPER AUTO GENERATED CODE

       
        /// <summary>
        /// Alter elements of this storage adressed by sequential indices 
        /// </summary>
        /// <param name="indices">array specifying the elements to be altered, sequential indexing</param>
        /// <param name="values">ILBaseArray of the same type than this array, holding the new values. 
        /// The number of elements of storage must match the 
        /// number of elements of indices. The only exception to this rule is if 'values' is a scalar array. The 
        /// single value of 'values' is than used to set all elements addressed by 'indices'.</param>
        /// <remarks><para>For empty arrays, scalar or vectors, indices outside the current bounds for 
        /// this array will expand this array to the size neccessary. 
        /// For other arrays the sequential indices given must fit inside this arrays dimensions. </para></remarks>
        public override void SetRange(ILBaseArray<Int64> indices, ILDenseStorage<ILStorage> values) {
            //if (object.Equals(indices,null))
            //    throw new ILArgumentException("indices given must not be null");
            if (object.Equals(values, null))
                throw new ILArgumentException("values given must not be null. Use A[ind] = null; if removal was intended.");
            #region set full shortcut
            // if (indices is ILFullRange || object.Equals(indices, null)) {
            if (object.Equals(indices, null))
            {
                int pos;
                ILStorage[] myArray = GetArrayForWrite();
                if (values.Size.NumberOfElements == 1) {
                    pos = Size.NumberOfElements;
                    ILStorage val = values.GetValueTyped(0);
                    while (pos-- > 0) {
                        if (myArray[pos] != null)
                            myArray[pos].Dispose();
                        myArray[pos] = val;
                    }
                } else {
                    if (values.Size.NumberOfElements != Size.NumberOfElements)
                        throw new ILArgumentException("number of elements in source must match number of elements in destination array");
                    pos = 0;
                    foreach (ILStorage val in values) {
                        if (myArray[pos] != null)
                            myArray[pos].Dispose();
                        myArray[pos++] = val;
                    }
                }
            } else if (indices.Size.NumberOfElements != values.Size.NumberOfElements
                && values.Size.NumberOfElements != 1)
                throw new ILArgumentException("number of elements in source must match number of elements in destination array");
            #endregion
            if (indices.IsEmpty)
                return;
            if (indices.Storage is ILDenseStorage< Int64>) {
               
                Int64 maxIndex, minIndex;
                ILDenseStorage< Int64> indStorage = indices.Storage as ILDenseStorage< Int64>;
                indStorage.GetLimits(out minIndex, out maxIndex);
                if (minIndex < 0)
                    throw new ILArgumentException("sequential indices can not be negative");
                if ((int)maxIndex >= m_size.NumberOfElements) {
                    // handle resize
                    if (m_size.NumberOfDimensions == 2) {
                        if (m_size.NumberOfElements <= 1) {
                            // scalar or empty -> expand along 1 
                            ExpandArray(new int[] { (int)maxIndex + 1, 1 });
                        } else if (m_size[0] > 1 && m_size[1] == 1) {
                            ExpandArray(new int[] { (int)maxIndex + 1, 1 });
                        } else if (m_size[0] == 1 && m_size[1] > 1) {
                            ExpandArray(new int[] { 1, (int)maxIndex + 1 });
                        } else
                            throw new ILArgumentException("resizing array via sequential index access is supported for empty, scalar or vector only");
                    } else
                        throw new ILArgumentException("resizing array via sequential index access is supported for empty, scalar or vector only");
                }
                ILStorage[] myArray = GetArrayForWrite();
               
                Int64[] indArray = indStorage.GetArrayForRead();

                int len = indices.Size.NumberOfElements;
                int ind; 
                if (values.Size.NumberOfElements == 1) {
                    ILStorage scalarElement = values.GetValueTyped(0);
                    for (int i = 0; i < len; i++) {
                        ind = (int)indArray[i];
                        if (myArray[ind] != null)
                            myArray[ind].Dispose(); 
                        myArray[ind] = scalarElement;
                    }
                } else {
                    ILStorage[] valArray = values.GetArrayForRead();
                    for (int i = 0; i < len; ) {
                        ind = (int)indArray[i];
                        if (myArray[ind] != null)
                            myArray[ind].Dispose();
                        myArray[(int)indArray[i]] = valArray[i++];
                    }
                }
            } else {
                throw new ILArgumentException("storage type of given indices is not supported");
            }
        }
       
        /// <summary>
        /// Alter elements of this storage adressed by sequential indices 
        /// </summary>
        /// <param name="indices">array specifying the elements to be altered, sequential indexing</param>
        /// <param name="values">ILBaseArray of the same type than this array, holding the new values. 
        /// The number of elements of storage must match the 
        /// number of elements of indices. The only exception to this rule is if 'values' is a scalar array. The 
        /// single value of 'values' is than used to set all elements addressed by 'indices'.</param>
        /// <remarks><para>For empty arrays, scalar or vectors, indices outside the current bounds for 
        /// this array will expand this array to the size neccessary. 
        /// For other arrays the sequential indices given must fit inside this arrays dimensions. </para></remarks>
        public override void SetRange(ILBaseArray<Int32> indices, ILDenseStorage<ILStorage> values) {
            //if (object.Equals(indices,null))
            //    throw new ILArgumentException("indices given must not be null");
            if (object.Equals(values, null))
                throw new ILArgumentException("values given must not be null. Use A[ind] = null; if removal was intended.");
            #region set full shortcut
            // if (indices is ILFullRange || object.Equals(indices, null)) {
            if (object.Equals(indices, null))
            {
                int pos;
                ILStorage[] myArray = GetArrayForWrite();
                if (values.Size.NumberOfElements == 1) {
                    pos = Size.NumberOfElements;
                    ILStorage val = values.GetValueTyped(0);
                    while (pos-- > 0) {
                        if (myArray[pos] != null)
                            myArray[pos].Dispose();
                        myArray[pos] = val;
                    }
                } else {
                    if (values.Size.NumberOfElements != Size.NumberOfElements)
                        throw new ILArgumentException("number of elements in source must match number of elements in destination array");
                    pos = 0;
                    foreach (ILStorage val in values) {
                        if (myArray[pos] != null)
                            myArray[pos].Dispose();
                        myArray[pos++] = val;
                    }
                }
            } else if (indices.Size.NumberOfElements != values.Size.NumberOfElements
                && values.Size.NumberOfElements != 1)
                throw new ILArgumentException("number of elements in source must match number of elements in destination array");
            #endregion
            if (indices.IsEmpty)
                return;
            if (indices.Storage is ILDenseStorage< Int32>) {
               
                Int32 maxIndex, minIndex;
                ILDenseStorage< Int32> indStorage = indices.Storage as ILDenseStorage< Int32>;
                indStorage.GetLimits(out minIndex, out maxIndex);
                if (minIndex < 0)
                    throw new ILArgumentException("sequential indices can not be negative");
                if ((int)maxIndex >= m_size.NumberOfElements) {
                    // handle resize
                    if (m_size.NumberOfDimensions == 2) {
                        if (m_size.NumberOfElements <= 1) {
                            // scalar or empty -> expand along 1 
                            ExpandArray(new int[] { (int)maxIndex + 1, 1 });
                        } else if (m_size[0] > 1 && m_size[1] == 1) {
                            ExpandArray(new int[] { (int)maxIndex + 1, 1 });
                        } else if (m_size[0] == 1 && m_size[1] > 1) {
                            ExpandArray(new int[] { 1, (int)maxIndex + 1 });
                        } else
                            throw new ILArgumentException("resizing array via sequential index access is supported for empty, scalar or vector only");
                    } else
                        throw new ILArgumentException("resizing array via sequential index access is supported for empty, scalar or vector only");
                }
                ILStorage[] myArray = GetArrayForWrite();
               
                Int32[] indArray = indStorage.GetArrayForRead();

                int len = indices.Size.NumberOfElements;
                int ind; 
                if (values.Size.NumberOfElements == 1) {
                    ILStorage scalarElement = values.GetValueTyped(0);
                    for (int i = 0; i < len; i++) {
                        ind = (int)indArray[i];
                        if (myArray[ind] != null)
                            myArray[ind].Dispose(); 
                        myArray[ind] = scalarElement;
                    }
                } else {
                    ILStorage[] valArray = values.GetArrayForRead();
                    for (int i = 0; i < len; ) {
                        ind = (int)indArray[i];
                        if (myArray[ind] != null)
                            myArray[ind].Dispose();
                        myArray[(int)indArray[i]] = valArray[i++];
                    }
                }
            } else {
                throw new ILArgumentException("storage type of given indices is not supported");
            }
        }
       
        /// <summary>
        /// Alter elements of this storage adressed by sequential indices 
        /// </summary>
        /// <param name="indices">array specifying the elements to be altered, sequential indexing</param>
        /// <param name="values">ILBaseArray of the same type than this array, holding the new values. 
        /// The number of elements of storage must match the 
        /// number of elements of indices. The only exception to this rule is if 'values' is a scalar array. The 
        /// single value of 'values' is than used to set all elements addressed by 'indices'.</param>
        /// <remarks><para>For empty arrays, scalar or vectors, indices outside the current bounds for 
        /// this array will expand this array to the size neccessary. 
        /// For other arrays the sequential indices given must fit inside this arrays dimensions. </para></remarks>
        public override void SetRange(ILBaseArray<Int16> indices, ILDenseStorage<ILStorage> values) {
            //if (object.Equals(indices,null))
            //    throw new ILArgumentException("indices given must not be null");
            if (object.Equals(values, null))
                throw new ILArgumentException("values given must not be null. Use A[ind] = null; if removal was intended.");
            #region set full shortcut
            // if (indices is ILFullRange || object.Equals(indices, null)) {
            if (object.Equals(indices, null))
            {
                int pos;
                ILStorage[] myArray = GetArrayForWrite();
                if (values.Size.NumberOfElements == 1) {
                    pos = Size.NumberOfElements;
                    ILStorage val = values.GetValueTyped(0);
                    while (pos-- > 0) {
                        if (myArray[pos] != null)
                            myArray[pos].Dispose();
                        myArray[pos] = val;
                    }
                } else {
                    if (values.Size.NumberOfElements != Size.NumberOfElements)
                        throw new ILArgumentException("number of elements in source must match number of elements in destination array");
                    pos = 0;
                    foreach (ILStorage val in values) {
                        if (myArray[pos] != null)
                            myArray[pos].Dispose();
                        myArray[pos++] = val;
                    }
                }
            } else if (indices.Size.NumberOfElements != values.Size.NumberOfElements
                && values.Size.NumberOfElements != 1)
                throw new ILArgumentException("number of elements in source must match number of elements in destination array");
            #endregion
            if (indices.IsEmpty)
                return;
            if (indices.Storage is ILDenseStorage< Int16>) {
               
                Int16 maxIndex, minIndex;
                ILDenseStorage< Int16> indStorage = indices.Storage as ILDenseStorage< Int16>;
                indStorage.GetLimits(out minIndex, out maxIndex);
                if (minIndex < 0)
                    throw new ILArgumentException("sequential indices can not be negative");
                if ((int)maxIndex >= m_size.NumberOfElements) {
                    // handle resize
                    if (m_size.NumberOfDimensions == 2) {
                        if (m_size.NumberOfElements <= 1) {
                            // scalar or empty -> expand along 1 
                            ExpandArray(new int[] { (int)maxIndex + 1, 1 });
                        } else if (m_size[0] > 1 && m_size[1] == 1) {
                            ExpandArray(new int[] { (int)maxIndex + 1, 1 });
                        } else if (m_size[0] == 1 && m_size[1] > 1) {
                            ExpandArray(new int[] { 1, (int)maxIndex + 1 });
                        } else
                            throw new ILArgumentException("resizing array via sequential index access is supported for empty, scalar or vector only");
                    } else
                        throw new ILArgumentException("resizing array via sequential index access is supported for empty, scalar or vector only");
                }
                ILStorage[] myArray = GetArrayForWrite();
               
                Int16[] indArray = indStorage.GetArrayForRead();

                int len = indices.Size.NumberOfElements;
                int ind; 
                if (values.Size.NumberOfElements == 1) {
                    ILStorage scalarElement = values.GetValueTyped(0);
                    for (int i = 0; i < len; i++) {
                        ind = (int)indArray[i];
                        if (myArray[ind] != null)
                            myArray[ind].Dispose(); 
                        myArray[ind] = scalarElement;
                    }
                } else {
                    ILStorage[] valArray = values.GetArrayForRead();
                    for (int i = 0; i < len; ) {
                        ind = (int)indArray[i];
                        if (myArray[ind] != null)
                            myArray[ind].Dispose();
                        myArray[(int)indArray[i]] = valArray[i++];
                    }
                }
            } else {
                throw new ILArgumentException("storage type of given indices is not supported");
            }
        }
       
        /// <summary>
        /// Alter elements of this storage adressed by sequential indices 
        /// </summary>
        /// <param name="indices">array specifying the elements to be altered, sequential indexing</param>
        /// <param name="values">ILBaseArray of the same type than this array, holding the new values. 
        /// The number of elements of storage must match the 
        /// number of elements of indices. The only exception to this rule is if 'values' is a scalar array. The 
        /// single value of 'values' is than used to set all elements addressed by 'indices'.</param>
        /// <remarks><para>For empty arrays, scalar or vectors, indices outside the current bounds for 
        /// this array will expand this array to the size neccessary. 
        /// For other arrays the sequential indices given must fit inside this arrays dimensions. </para></remarks>
        public override void SetRange(ILBaseArray<float> indices, ILDenseStorage<ILStorage> values) {
            //if (object.Equals(indices,null))
            //    throw new ILArgumentException("indices given must not be null");
            if (object.Equals(values, null))
                throw new ILArgumentException("values given must not be null. Use A[ind] = null; if removal was intended.");
            #region set full shortcut
            // if (indices is ILFullRange || object.Equals(indices, null)) {
            if (object.Equals(indices, null))
            {
                int pos;
                ILStorage[] myArray = GetArrayForWrite();
                if (values.Size.NumberOfElements == 1) {
                    pos = Size.NumberOfElements;
                    ILStorage val = values.GetValueTyped(0);
                    while (pos-- > 0) {
                        if (myArray[pos] != null)
                            myArray[pos].Dispose();
                        myArray[pos] = val;
                    }
                } else {
                    if (values.Size.NumberOfElements != Size.NumberOfElements)
                        throw new ILArgumentException("number of elements in source must match number of elements in destination array");
                    pos = 0;
                    foreach (ILStorage val in values) {
                        if (myArray[pos] != null)
                            myArray[pos].Dispose();
                        myArray[pos++] = val;
                    }
                }
            } else if (indices.Size.NumberOfElements != values.Size.NumberOfElements
                && values.Size.NumberOfElements != 1)
                throw new ILArgumentException("number of elements in source must match number of elements in destination array");
            #endregion
            if (indices.IsEmpty)
                return;
            if (indices.Storage is ILDenseStorage< float>) {
               
                float maxIndex, minIndex;
                ILDenseStorage< float> indStorage = indices.Storage as ILDenseStorage< float>;
                indStorage.GetLimits(out minIndex, out maxIndex);
                if (minIndex < 0)
                    throw new ILArgumentException("sequential indices can not be negative");
                if ((int)maxIndex >= m_size.NumberOfElements) {
                    // handle resize
                    if (m_size.NumberOfDimensions == 2) {
                        if (m_size.NumberOfElements <= 1) {
                            // scalar or empty -> expand along 1 
                            ExpandArray(new int[] { (int)maxIndex + 1, 1 });
                        } else if (m_size[0] > 1 && m_size[1] == 1) {
                            ExpandArray(new int[] { (int)maxIndex + 1, 1 });
                        } else if (m_size[0] == 1 && m_size[1] > 1) {
                            ExpandArray(new int[] { 1, (int)maxIndex + 1 });
                        } else
                            throw new ILArgumentException("resizing array via sequential index access is supported for empty, scalar or vector only");
                    } else
                        throw new ILArgumentException("resizing array via sequential index access is supported for empty, scalar or vector only");
                }
                ILStorage[] myArray = GetArrayForWrite();
               
                float[] indArray = indStorage.GetArrayForRead();

                int len = indices.Size.NumberOfElements;
                int ind; 
                if (values.Size.NumberOfElements == 1) {
                    ILStorage scalarElement = values.GetValueTyped(0);
                    for (int i = 0; i < len; i++) {
                        ind = (int)indArray[i];
                        if (myArray[ind] != null)
                            myArray[ind].Dispose(); 
                        myArray[ind] = scalarElement;
                    }
                } else {
                    ILStorage[] valArray = values.GetArrayForRead();
                    for (int i = 0; i < len; ) {
                        ind = (int)indArray[i];
                        if (myArray[ind] != null)
                            myArray[ind].Dispose();
                        myArray[(int)indArray[i]] = valArray[i++];
                    }
                }
            } else {
                throw new ILArgumentException("storage type of given indices is not supported");
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        internal override void SetRangeFull(ILDenseStorage<ILStorage> values) {
            if (values.Size.NumberOfElements != 1 && values.Size.NumberOfElements != Size.NumberOfElements)
                throw new ILArgumentException("source and destination array must have the same number of arguments");
            ILStorage[] myArr = GetArrayForWrite();
            if (values.Size.NumberOfElements == 1) {
                ILStorage scal = values.GetValueTyped(0);
                for (int i = Size.NumberOfElements; i-- > 0; ) {
                    if (myArr[i] != null)
                        myArr[i].Dispose();
                    myArr[i] = scal;
                }
            } else {
                ILStorage[] valArr = values.GetArrayForRead();
                for (int i = Size.NumberOfElements; i-- > 0; ) {
                    if (myArr[i] != null)
                        myArr[i].Dispose();
                    myArr[i] = valArr[i];
                }
            }
        }
        public override void SetRange(ILRange range, ILDenseStorage<ILStorage> values) {
            if (range.Size.NumberOfElements == 0) return;
            int rangeDimLen = range.RangeArray.Length, higherDimSum = 0;
            int leadDimLenRange = range[0].Count, leadDimLen = m_size[0];
            int d, outElemCount = range.Size.NumberOfElements;
            ILStorage[] myArr = GetArrayForWrite();
            int[] idxArr = new int[rangeDimLen];    // used to store current position inside higher dims 
            ILIntList[] rng = range.RangeArray;
            int[] seqDistances = m_size.GetSequentialIndexDistances(range.Size.NumberOfDimensions);
            int[] inFullDim = new int[rangeDimLen];
            // initialize higher dimension summand and inFullDim[] flag array
            for (int i = 1; i < idxArr.Length; i++) {
                if (rng[i][0] < 0) {
                    inFullDim[i] = rng[i][0];
                } else {
                    inFullDim[i] = 0;
                    higherDimSum += seqDistances[i] * rng[i][0];
                }
            }
            //for (int lIdx = 0; lIdx < leadDimLenRange; lIdx ++) { 
            //    retArr[curPosOut++] = myArr[higherDimSum + rleadDim[lIdx]]; 
            //}
            if (values.Size.NumberOfElements == 1) {
                #region scalar case
                ILStorage scalar = values.GetValueTyped(0);
                while (true) {
                    #region copy along leading dimension
                    for (int i = 0; i < rng[0].Count; i++) {
                        if (rng[0][i] < 0) {
                            for (int c = -rng[0][i]; c-- >= 0; ) {
                                ILStorage cur = myArr[higherDimSum];
                                if (cur != null)
                                    cur.Dispose(); 
                                myArr[higherDimSum] = scalar.Clone();
                                higherDimSum++;
                            }
                            higherDimSum += (rng[0][i] - 1); // negative value in rng!  
                        } else {
                            int ind = higherDimSum + rng[0][i]; 
                            ILStorage cur = myArr[ind];
                            if (cur != null)
                                cur.Dispose();
                            myArr[ind] = scalar.Clone();      
                        }
                    }
                    #endregion
                    #region increase higher dims
                    d = 1; 
                    while (d < idxArr.Length) {
                        if (inFullDim[d] < 0) {
                            higherDimSum += seqDistances[d];
                            inFullDim[d]++;
                            break;
                        }

                        if (rng[d][idxArr[d]] >= 0) {
                            higherDimSum -= (rng[d][idxArr[d]] * seqDistances[d]);
                        } else {
                            higherDimSum += (rng[d][idxArr[d]] * seqDistances[d]);
                        }
                        idxArr[d]++;
                        if (idxArr[d] == rng[d].Count) {
                            idxArr[d] = 0;
                            if (rng[d][idxArr[d]] < 0) {
                                inFullDim[d] = rng[d][idxArr[d]];
                            }
                            d++;
                        } else if (rng[d][idxArr[d]] < 0) {
                            inFullDim[d] = rng[d][idxArr[d]];
                            break;
                        } else {
                            higherDimSum += seqDistances[d] * rng[d][idxArr[d]];
                            break;
                        }

                    }
                    #endregion
                    if (d >= idxArr.Length)
                        break;
                }
                scalar.Dispose();  // we have only made copies of scalar input
                #endregion
            } else {
                #region non scalar case
                ILStorage[] valuesArr = values.GetArrayForRead();
                int valuesPos = 0;
                while (true) {
                    // copy along leading dimension
                    for (int i = 0; i < rng[0].Count; i++) {
                        if (rng[0][i] < 0) {
                            for (int c = -rng[0][i]; c-- >= 0; ) {
                                ILStorage cur = myArr[higherDimSum];
                                if (cur != null)
                                    cur.Dispose();
                                myArr[higherDimSum] = valuesArr[valuesPos];
                                valuesPos++;
                                higherDimSum++;
                            }
                            higherDimSum += (rng[0][i] - 1); // negative value in rng!  
                        } else {
                            int ind = higherDimSum + rng[0][i];
                            ILStorage cur = myArr[ind];
                            if (cur != null)
                                cur.Dispose();
                            myArr[ind] = valuesArr[valuesPos];
                            valuesPos++;
                        }
                    }

                    // increase higher dims 
                    d = 1;
                    while (d < idxArr.Length) {
                        if (inFullDim[d] < 0) {
                            higherDimSum += seqDistances[d];
                            inFullDim[d]++;
                            break;
                        }

                        if (rng[d][idxArr[d]] >= 0) {
                            higherDimSum -= (rng[d][idxArr[d]] * seqDistances[d]);
                        } else {
                            higherDimSum += (rng[d][idxArr[d]] * seqDistances[d]);
                        }
                        idxArr[d]++;
                        if (idxArr[d] == rng[d].Count) {
                            idxArr[d] = 0;
                            if (rng[d][idxArr[d]] < 0) {
                                inFullDim[d] = rng[d][idxArr[d]];
                            }
                            d++;
                        } else if (rng[d][idxArr[d]] < 0) {
                            inFullDim[d] = rng[d][idxArr[d]];
                            break;
                        } else {
                            higherDimSum += seqDistances[d] * rng[d][idxArr[d]];
                            break;
                        }

                    }
                    if (d >= idxArr.Length)
                        break;
                }
                #endregion
            }
        }
        /// <summary>
        /// detach this storage: copy its countable array if needed
        /// </summary>
        protected override void Detach() {
            if (m_data.ReferenceCount <= 1)
                return;
            Data = m_data.CreateCopy();
        }
        /// <summary>
        /// walk all elements of this cell and recursively replace them with clones of themself 
        /// </summary>
        /// <remarks>Due to the cloning mechanism of ILNumerics, the full clone 
        /// will be cheap by using lazy copies of the data. Cloned elements will automatically be detached on write access.</remarks>
        internal void DeepReferenceElements() {
            int len = Size.NumberOfElements;
            ILStorage[] myArr = GetArrayForWrite();
            for (int i = 0; i < len; i++) {
                ILStorage a = myArr[i];
                if (!Object.ReferenceEquals(a, null) && !a.IsDisposed) {
                    ILStorage clone = a.Clone(); 
                    myArr[i] =clone;
                } else
                    myArr[i] = null;
            }
        }
        internal ILDenseStorage<T> GetDenseStorage<T>(params ILBaseArray[] indices) {
            ILCellStorage retStorage = (ILCellStorage)Subarray(indices);
            if (retStorage.Size.NumberOfElements == 1) {
                ILStorage innerStorage = (retStorage as ILCellStorage).GetValueTyped(0);
                if (innerStorage is ILDenseStorage<T>) {
                    return innerStorage as ILDenseStorage<T>; 
                } else {
                    if (innerStorage == null) {
                        throw new ILArgumentException("the type of the element at the specified position could not be determined"); 
                    }
                    Type innerStorageType = innerStorage.GetType();
                    if (innerStorageType == typeof(ILCellStorage)) 
                        throw new ILArgumentException("The cell element specified contains a cell storage. Use GetCell() in order to retrieve cells from cells!");
                    if (!innerStorageType.Name.Contains("ILDenseStorage")) // unlikely to happen in ILNumerics R2
                        throw new ILArgumentException("the type of the element at the specified position is not valid in this context"); 
                    Type[] arguments =  innerStorageType.GetGenericArguments(); 
                    if (arguments.Length > 0) 
                        throw new ILArgumentException(String.Format("the array found at the specified location is of type <{0}> and not of the requested type <{1}>",arguments[0].Name, typeof(T).Name));  
                    throw new ILArgumentException("an unknown error occoured while trying to retrieve the cell element"); 
                }
            } else {
                throw new ILArgumentException("indices must define a scalar subarray range to an existing cell element");
            }
        }
        internal ILBaseArray GetScalar(params ILBaseArray[] indices) {
            ILCellStorage cellStorage = (ILCellStorage)Subarray(indices);
            if (cellStorage.Size.NumberOfElements == 1) {
                ILStorage innerStorage = cellStorage.GetValueTyped(0);
                return (innerStorage == null) ? null : innerStorage.GetAsBaseArray(); 
            } else {
                throw new ILArgumentException("indices must define a scalar subarray range");
            }

        }
        internal override ILBaseArray GetAsBaseArray() {
            return new ILRetCell((ILCellStorage)Clone());
        }
        internal override void Dispose(bool manual) {
            if (IsDisposed) return; 
            
            for (int i = m_size.NumberOfElements; i--> 0;) {
                ILStorage storage = m_data.Data[i]; 
                if(storage != null)
                    storage.Dispose(manual); 
            }
            base.Dispose(manual);
        }
        internal override bool Equals(object obj) {
            if (object.Equals(obj, null)) return false;
            ILCellStorage obStor = obj as ILCellStorage;
            if (obStor != null) {
                if (!obStor.Size.IsSameSize(Size)) return false;
                ILStorage[] myArr = GetArrayForRead();
                ILStorage[] objArr = obStor.GetArrayForRead();

                for (int i = Size.NumberOfElements; i --> 0; ) {
                    ILStorage stor1 = myArr[i];
                    ILStorage stor2 = objArr[i];
                    if (stor1 == null ^ stor2 == null) return false;
                    if (stor1 == null) continue;
                    if (!myArr[i].Equals(objArr[i])) return false;
                }
                return true;
            } else {
                return false; 
            }
        }
        #endregion

        #region helper 
        private static ILStorage[] baseArray2Storages(ILBaseArray[] system_array, int minLength) {
            using (ILScope.Enter(system_array)) {
                ILStorage[] ret = ILMemoryPool.Pool.New<ILStorage>(minLength);
                int i = 0;
                for (; i < system_array.Length; i++) {
                    ILBaseArray curArr = system_array[i];
                    if (curArr == null) {
                        ret[i] = null;
                    } else {
                        ILStorage cur = system_array[i].Storage;
                        ret[i] = cur; 
                    }
                }
                // clear the rest of the array
                for (; i < minLength; i++)
                    ret[i] = null; 
                return ret; 
            }
        }
        protected override ILDenseStorage<ILStorage> CreateSelf(ILStorage[] data, ILSize size) {
            ILCellStorage ret = new ILCellStorage(data, size);
            //ret.DeepReferenceElements();
            return ret; 
        }
        protected override ILDenseStorage<ILStorage> CreateSelf(ILSize size) {
            ILCellStorage ret = new ILCellStorage(size);
            //ret.DeepReferenceElements();
            return ret; 
        }
        protected override ILDenseStorage<ILStorage> CreateSelf(ILCountableArray<ILStorage> Data, ILSize ILSize) {
            ILCellStorage ret = new ILCellStorage(Data, ILSize);
            //ret.DeepReferenceElements();
            return ret; 
        }
        internal override string ShortInfo() {
            if (IsDisposed)
                return "Cell (disposed)";
            return "Cell " + Size.ToString();
        }
        //internal override void Remove(int dimension, ILIntList indices) {
        //    using (ILScope.Enter()) {
        //        if (dimension >= Dimensions.NumberOfDimensions)
        //            throw new ILArgumentException("index out of range");
        //        if (dimension == -1) {
        //            Data = new ILCountableArray<ILStorage>(0);
        //            m_dimensions = ILSize.Empty00;
        //            return;
        //        }
        //        // TODO: may should be replaced with a faster (&fancier? ) version... ??
        //        ILBaseArray[] dims = new ILBaseArray[Dimensions.NumberOfDimensions];
        //        ILIntList keepInd = ILIntList.Create();
        //        ILIntList remvInd = indices;
        //        int max = Dimensions[dimension];
        //        foreach (int i in indices)
        //            if (i >= max || i < 0) throw new ILArgumentException("removal index out of range");
        //        for (int i = 0; i < max; i++) {
        //            if (!remvInd.Contains(i)) {
        //                keepInd.Add(i);
        //            }
        //        }
        //        // we give the indices to _keep_ to the subarray function
        //        ILArray<int> remIndices = ILMath.create<int>(keepInd.GetArray(), new ILSize(1, keepInd.Count));
        //        if (remIndices.Dimensions.NumberOfElements == Dimensions[dimension]) {
        //            // nothing to do
        //            return;
        //        }
        //        for (int i = 0; i < dims.Length; i++) {
        //            dims[i] = ILMath.full;
        //        }
        //        dims[dimension] = remIndices;
        //        ILCellStorage ret = (ILCellStorage)Subarray(dims);
        //        //ret.DeepReferenceElements(); 
                
        //        Dispose();
        //        Data = ret.Data;
        //        m_dimensions = ret.Dimensions;
        //    }
        //}
        #endregion

    }
}
