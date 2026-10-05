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
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Exceptions;

namespace ILNumerics {
    /// <summary>
    /// Rectangular array, used as input parameter only 
    /// </summary>
    /// <typeparam name="ElementType">Inner type. This will mostly be a system numeric type or a 
    /// complex floating point type.</typeparam>
    /// <remarks><para>This class extends the primary <c>ILArray</c> by optimizing its behavior for the case when used as an
    /// input parameter to functions.</para><para>When writing your own function all <i>input</i> parameters should be of type <c>ILInArray</c>. 
    /// Similary, all return types should be of type <see cref="ILNumerics.ILRetArray{T}"/> and all "out" parameters of type <see cref="ILNumerics.ILOutArray{T}"/>.</para>
    /// <para>Other than being used to transfer arguments into functions, <c>ILInArray</c> should not be used.</para>
    /// </remarks>
    /// <seealso cref="ILNumerics.ILArray{T}"/>
    /// <seealso cref="ILNumerics.ILRetArray{T}"/>
    /// <seealso cref="ILNumerics.ILOutArray{T}"/>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public sealed partial class ILInArray<ElementType> : ILDenseArray<ElementType> {

        private static readonly bool s_isTempArray = true; 

        #region constructors
        /// <summary>
        /// create new ILInArray, specify (dense) storage
        /// </summary>
        /// <param name="storage"></param>
        internal ILInArray(ILDenseStorage<ElementType> storage)
            : base(storage, s_isTempArray) {
        }
        /// <summary>
        /// create new ILInArray, specify dimensions
        /// </summary>
        /// <param name="dimensions"></param>
        internal ILInArray(ILSize dimensions) 
            : base(new ILDenseStorage<ElementType>(dimensions), s_isTempArray) {
        }
        /// <summary>
        /// create new ILInArray from System.Array
        /// </summary>
        /// <param name="elements">System.Array</param>
        /// <param name="dimensions">dimension specifier</param>
        internal ILInArray(ElementType[] elements, ILSize dimensions) :
            base(new ILDenseStorage<ElementType>(elements, dimensions), s_isTempArray) {
        }
        /// <summary>
        /// create new ILInArray from System.Array
        /// </summary>
        /// <param name="elements">System.Array</param>
        /// <param name="dimensions">dimension specifier</param>
        internal ILInArray(ElementType[] elements, params int[] dimensions) :
            base(new ILDenseStorage<ElementType>(elements, new ILSize(dimensions)), s_isTempArray) {
        }
        /// <summary>
        /// create new ILInArray from System.Array
        /// </summary>
        /// <param name="elements">variable length System.Array</param>
        internal ILInArray(params ElementType[] elements) :
            base(new ILDenseStorage<ElementType>(elements, new ILSize(1, elements.Length)), s_isTempArray) {
        }

        #endregion

        #region implicit cast operators
        #region constructional operators
        /// <summary>
        /// Implicitly convert scalar to array of size 1x1 (scalar).
        /// </summary>
        /// <param name="val">System type of size scalar</param>
        /// <returns>New ILInArray of type ILInArray <![CDATA[<typeof(val)>]]> of size 1x1 
        /// holding the only element with value of val.
        /// </returns>
        public static implicit operator ILInArray<ElementType> (ElementType val) {
            ILInArray<ElementType> ret = new ILInArray<ElementType>(
                        new ILDenseStorage<ElementType>(
                            new ElementType[1] {val}, 
                            new ILSize(1,1)));
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
        public static implicit operator ILInArray<ElementType> (ElementType[] A) {
            if (A == null) {
                return null; 
                //ILDenseStorage<ElementType> dS = new ILDenseStorage<ElementType>(new ElementType[0], ILSize.Empty00); 
                //return new ILInArray<ElementType>(dS);                          
            }
            ILArray<ElementType> ret; 
            if (Settings.CreateRowVectorsByDefault)
                ret = new ILInArray<ElementType>(A, 1, A.Length);
            else 
                ret = new ILInArray<ElementType>(A, A.Length, 1); 
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret; 
        }
        /// <summary>
        /// Implicitly convert n-dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">Arbitrarily sized System.Array</param>
        /// <returns>If A is null: empty array. Else: new ILNumerics array of the same size as A</returns>
        /// <remarks>The inner type of input array <paramref name="A"/> must match the requested type
        /// <typeparamref name="ElementType"/>. The resulting ILInArray will reflect all dimensions of 
        /// A. Elements of A will get copied to elements of output array (shallow copy).</remarks>
        /// <exception cref="ILNumerics.Exceptions.ILCastException"> if type of input does not match 
        /// ElementType</exception>
        public static implicit operator ILInArray<ElementType> (Array A) {
            if (A == null) {
                return null;
            }
            if (A.Length == 0) {
                return new ILInArray<ElementType>(ILSize.Empty00);
            }
            if (A.GetType().GetElementType() != typeof(ElementType)) 
                throw new ILCastException("inner type of System.Array must match");
            int[] dims = new int[A.Rank];
            ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(A.Length);
            int posArr = 0; 
            for (int i = 0; i < dims.Length; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            if (dims.Length == 1 && Settings.CreateRowVectorsByDefault) {
                dims = new int[2] { 1, dims[0] };
            }
            foreach (ElementType item in A) 
                retArr[posArr++] = item; 
            ILInArray<ElementType> ret = new ILInArray<ElementType>(retArr,dims);
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret; 
        }
        /// <summary>
        /// Implicitly cast two dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">2-dimensional System.Array</param>
        /// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        public static implicit operator ILInArray<ElementType>(ElementType[,] A) {
            if (A == null) {
                return null;
            }
            if (A.Length == 0) {
                return new ILInArray<ElementType>(ILSize.Empty00);
            }
            int[] dims = new int[2];
            ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(A.Length);
            int posArr = 0;
            for (int i = 0; i < 2; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (ElementType item in A)
                retArr[posArr++] = item;
            ILInArray<ElementType> ret = new ILInArray<ElementType>(retArr, dims);
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret; 
        }
        /// <summary>
        /// Implicitly casts three dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">3-dimensional System.Array</param>
        /// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        public static implicit operator ILInArray<ElementType>(ElementType[,,] A) {
            if (A == null) {
                return null;
            }
            if (A.Length == 0) {
                return new ILInArray<ElementType>(ILSize.Empty00);
            }
            int[] dims = new int[3];
            ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(A.Length);
            int posArr = 0;
            for (int i = 0; i < 3; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (ElementType item in A)
                retArr[posArr++] = item;
            ILInArray<ElementType> ret = new ILInArray<ElementType>(retArr, dims);
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret; 
        }
        /// <summary>
        /// Implicitly cast ILInArray to ILRetCell
        /// </summary>
        /// <param name="array">Input array</param>
        /// <returns>1x1 cell holding the array</returns>
        public static implicit operator ILRetCell(ILInArray<ElementType> array) {
            using (ILScope.Enter(array)) {
                ILRetCell ret = new ILRetCell(ILSize.Scalar1_1, array.Storage);
                ret.Storage.FromImplicitCast = true;
                return ret;
            }
        }
        #endregion

        #region conversional operators 
        /// <summary>
        /// Convert temporary to input parameter array
        /// </summary>
        /// <param name="array">Temp array</param>
        /// <returns>Input parameter array, will survive the current scope only</returns>
        public static implicit operator ILInArray<ElementType>(ILRetArray<ElementType> array) {
            if (object.Equals(array, null)) 
                return null;
            ILInArray<ElementType> ret = new ILInArray<ElementType>(array.GiveStorageAwayOrClone());
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret; 
        }
        /// <summary>
        /// Convert persistent to input parameter array
        /// </summary>
        /// <param name="array">Persistent array</param>
        /// <returns>Input parameter array, will survive the current scope only</returns>
        public static implicit operator ILInArray<ElementType>(ILArray<ElementType> array) {
            if (object.Equals(array, null))
                return null;
            ILInArray<ElementType> ret = new ILInArray<ElementType>(new ILDenseStorage<ElementType>(
                array.Storage.GetDataArray(), array.Size));
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret; 

        }
        /// <summary>
        /// Convert output paramter array to input parameter array
        /// </summary>
        /// <param name="array">Output parameter array</param>
        /// <returns>Input parameter array, will survive the current scope only</returns>
        public static implicit operator ILInArray<ElementType>(ILOutArray<ElementType> array) {
            if (object.Equals(array, null))
                return null;
            ILInArray<ElementType> ret = new ILInArray<ElementType>(new ILDenseStorage<ElementType>(
                array.Storage.GetDataArray(), array.Size));
            if (Settings.AllowInArrayAssignments)
                ILScope.Context.RegisterArray(ret);
            return ret;
        }
        #endregion
        #endregion

        #region indexer + memory management
        /// <summary>
        /// Subarray creation/ manipulation/ deletion
        /// </summary>
        /// <param name="range">Range specification, defining the size of the subarray</param>
        /// <returns>Subarray as copy of this array</returns>
        public ILRetArray<ElementType> this[params ILBaseArray[] range] {
            get {
                using (ILScope.Enter(this))
                using (ILScope.Enter(range))
                    return new ILRetArray<ElementType>(Storage.Subarray(range));
            }
        }

        internal override void LeaveScope() {
            m_scopeCounter--;
            if (m_scopeCounter <= 0) {
                Dispose();
            }
        }

        #endregion
    }
}
