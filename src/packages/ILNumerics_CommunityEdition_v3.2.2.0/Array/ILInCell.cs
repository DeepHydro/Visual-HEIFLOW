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
    /// Theses elements commonly are cells itself. <c>ILInCell</c> acts as input parameter 
    /// in function declarations only. 
    /// Use <see cref="ILNumerics.ILCell"/> for local cell variables in your algorithms. 
    /// <para>More details about cells are found in the <a href="http://ilnumerics.net/$Cells.html">online manual</a>.</para>
    /// </remarks>
    /// <seealso cref="ILNumerics.ILCell"/>
    [System.Security.SecuritySafeCritical]
    public sealed class ILInCell : ILBaseCell {

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
        internal ILInCell(ILStorage[] data, params int[] size)
            : base(new ILCellStorage(data, new ILSize(size)), s_isTempArray) {
        }
        /// <summary>
        /// Create cell object with pre-created data in specified dimensions 
        /// </summary>
        /// <param name="data">predefined element data array, will be used for new cell (no copy will be made)</param>
        /// <param name="size">size of the new cell</param>
        /// <remarks>object array data will directly be used for storage. No 
        /// copy will be made. However, any arrays referenced by data are dereferenced for storage inside the cell. The size must match prod(size)</remarks>
        [Obsolete("use <see cref='ILNumerics.ILMath.cell'/> instead!")]
        internal ILInCell(ILStorage[] data, ILSize size)
            : base(new ILCellStorage(data, size), s_isTempArray) {
        }
        /// <summary>
        /// create new cell object, elements will be 'null' 
        /// </summary>
        /// <param name="size">dimension sizes of the new cell</param>
        [Obsolete("use <see cref='ILNumerics.ILMath.cell'/> instead!")]
        internal ILInCell(params int[] size)
            : base(new ILCellStorage(new ILStorage[prod(size)], new ILSize(size)), s_isTempArray) { }

        internal ILInCell(ILCellStorage cellStorage)
            : base(cellStorage, s_isTempArray) { }

        [Obsolete("use <see cref='ILNumerics.ILMath.cell'/> instead!")]
        internal ILInCell(ILSize size, params ILStorage[] values)
            : base(new ILCellStorage(values, size), s_isTempArray) { }

        #endregion constructors

        #region implicit casts (for subarray definitions)
        #region constructional operators
        //public static implicit operator ILCell(double value) {
        //    return createScalar(value); 
        //}
        //public static implicit operator ILCell(float value) {
        //    return createScalar(value); 
        //}
        //public static implicit operator ILCell(byte value) {
        //    return createScalar(value); 
        //}
        //public static implicit operator ILCell(int[] value) {
        //    return createArray(value); 
        //}
        #endregion

        #region conversional operators
        /// <summary>
        /// Convert temporary cell to input parameter type cell
        /// </summary>
        /// <param name="cell">Temporary cell</param>
        /// <returns>Input parameter type cell</returns>
        public static implicit operator ILInCell(ILRetCell cell) {
            if (object.Equals(cell, null))
                return null;
            ILInCell ret = new ILInCell((ILCellStorage)cell.GiveStorageAwayOrClone());
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret;
        }
        /// <summary>
        /// Convert temporary cell to input parameter type cell
        /// </summary>
        /// <param name="cell">ILCell</param>
        /// <returns>Input parameter type cell</returns>
        public static implicit operator ILInCell(ILCell cell) {
            if (object.Equals(cell, null))
                return null;
            ILInCell ret = new ILInCell(new ILCellStorage(
                cell.Storage.GetDataArray(), cell.Size));
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret;
        }
        /// <summary>
        /// Convert output parameter type cell to input parameter type cell
        /// </summary>
        /// <param name="cell">Output parameter type cell</param>
        /// <returns>Input parameter type cell</returns>
        public static implicit operator ILInCell(ILOutCell cell) {
            if (object.Equals(cell, null))
                return null;
            ILInCell ret = new ILInCell(new ILCellStorage(
                cell.Storage.GetDataArray(), cell.Size));
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret;
        }
        #endregion
        #endregion

        #region helper functions
        internal static long prod(int[] sizes) {
            int ret = 1;
            for (int i = 0; i < sizes.Length; i++)
                ret *= sizes[i];
            return ret;
        }
        #endregion helper functions

        #region Index access
        /// <summary>
        /// Return single element in a scalar cell 
        /// </summary> 
        /// <paramref name="indices" value="Index to element"/>
        /// <remarks>
        /// <para>The indexer retrieves a single element from within the cell. It returns the single element 
        /// in a new (scalar) cell. Use dereferencing functions like GetArray&lt;T>() in order to retrieve 
        /// the element value (ie. the array addressed) without a cell container.</para>
        /// <para>Input parameter type cells are immutable! They are not intended to be altered! Therefore, 
        /// write access on such cells is disabled. In order to alter content of a cell, assign the cell to a regular
        /// persistent cell of type <see cref="T:ILNumerics.ILCell"/>.</para></remarks>
        public ILRetCell this[params int[] indices] {
            get {
                ILStorage val = Storage.GetValueTyped(indices);
                if (val is ILCellStorage)
                    return new ILRetCell((ILCellStorage)val);
                else
                    return new ILRetCell(new ILStorage[] { val }, ILSize.Scalar1_1);
            }
        }

        /// <summary>
        /// Subarray access. Get regular subarray. 
        /// </summary>
        /// <param name="indices">Address range</param>
        /// <returns>Cell array with subcell range from this instance, addressed by <c>indices</c> </returns>
        /// <remarks>
        /// <para>The indexer is used to query subcell ranges. All general indexing/ subarray addressing rules apply.
        /// The elements addressed are returned within a new temporary cell instance.</para>
        /// <para>Input parameter type cells are immutable! They are not intended to be altered! Therefore, 
        /// write access on such cells is disabled. In order to alter content of a cell, assign the cell to a regular
        /// persistent cell of type <see cref="T:ILNumerics.ILCell"/>.</para></remarks>
        public ILRetCell this[params ILBaseArray[] indices] {
            get {
                using (ILScope.Enter(indices)) {
                    ILCellStorage elements = (ILCellStorage)Storage.Subarray(indices);
                    return new ILRetCell(elements);
                }
            }
        }

        #endregion index access

        #region memory management
        internal override void LeaveScope() {
            m_scopeCounter--;
            if (m_scopeCounter <= 0) {
                Dispose();
            }
        }
        #endregion

    }
}
