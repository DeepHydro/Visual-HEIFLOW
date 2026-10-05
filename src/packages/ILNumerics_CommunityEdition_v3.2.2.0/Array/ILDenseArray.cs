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
using ILNumerics.Storage;
using System.IO; 
using ILNumerics.Exceptions;

namespace ILNumerics {
    [System.Diagnostics.DebuggerTypeProxy(typeof(ILNumerics.Misc.ILArrayDebuggerProxy<>))]
    [System.Diagnostics.DebuggerDisplay("{ShortInfo(),nq}")]
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public partial class ILDenseArray<ElementType> : ILBaseArray<ElementType> {

        #region constructors
        internal ILDenseArray(ILStorage storage, bool isTempArray)
            : base(storage, isTempArray) {

        }
        #endregion

        #region properties
        /// <summary>
        /// internal access to the underlying storage
        /// </summary>
        internal new ILDenseStorage<ElementType> Storage {
            get { return (m_storage as ILDenseStorage<ElementType>); }
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
        /// Clone of this array (fast, lazy and shallow)
        /// </summary>
        public virtual ILRetArray<ElementType> C {
            get {
                return new ILRetArray<ElementType>((ILDenseStorage<ElementType>)Storage.Clone());
            }
        }
        /// <summary>
        /// Return transposed version of this array
        /// </summary>
        /// <remarks>For matrices, this swaps columns with rows. For arrays, the dimensions are shifted by one.
        /// <para>Note, for complex elements, <b>no</b> conjugate is created! Use conj(A.T) if this is intended.</para></remarks>
        public ILRetArray<ElementType> T {
            get {
                // if this is updated to create a conjugate for complex data types 
                // -> do not forget to update quick reference ILNumerics4Matlab also!
                return new ILRetArray<ElementType>(Storage.ShiftDimensions(1));
            }
        }
        /// <summary>
        ///  [deprecated] Get maximum value of array - if any
        /// </summary>
        /// <remarks>This property is marked as deprecated and will be removed in a future version. Use one of the 
        /// following alternatives instead: 
        /// </remarks>
        /// <seealso cref="ILNumerics.ILBaseArray&lt;ElementType>.GetLimits(out ElementType, out ElementType)"/>
        [Obsolete()]
        public ElementType MaxValue {
            get {
                ElementType ret, dummy;
                GetLimits(out dummy, out ret);
                return ret;
            }
        }
        /// <summary>
        ///  [deprecated] Get minimum value of array - if any
        /// </summary>
        /// <remarks>This property is marked as deprecated and will be removed in a future version. Use one of the 
        /// following alternatives instead: 
        /// <list type="bullets">
        /// <item><seealso cref="ILNumerics.ILBaseArray{T}.GetLimits"/></item>
        /// <item><seealso cref="ILNumerics.ILMath.minall"/></item>
        /// </list></remarks>
        [Obsolete()]
        public ElementType MinValue {
            get {
                ElementType ret, dummy;
                GetLimits(out ret, out dummy);
                return ret;
            }
        }
        #endregion

        #region memory management
        /// <summary>
        /// Get number of arrays, referencing the same underlying data storage 
        /// </summary>
        /// <return>This number is always greater than or equal to 1.</return>
        /// <remarks>For temporary arrays, calling this property does not - as usual - 
        /// disposes the array</remarks>
        public int ReferenceCount {
            get {
                return Storage.ReferenceCount;
            }
        }
        /// <summary>
        /// Get direct reference to inner System.Array storage for <b>read access</b> - use with care! 
        /// </summary>
        /// <returns>Reference to inner System.Array for reading</returns>
        /// <remarks>This method is provided for experts only! Altering elements of this 
        /// array may cause the data to be invalidated or corrupted! Use this array only for reading! Note 
        /// the ILNumerics array storage format (column major). Keep in mind, the length 
        /// of the array may exceeds the number of elements! 
        /// <para>Accessing the inner system array directly should be left to ILNumerics experts only! 
        /// Unless you really know, what you are doing, you should rather use the higher order access 
        /// methods provided by ILArray&lt;T>!</para>
        /// <para>Unlike (almost) all other member function of an array, this function
        /// does not keep track of internal memory management. It means, the storage which this array is based 
        /// upon, will not be set free after the function returns. You (as the user of the array) will have to pay 
        /// attention yourself, when to call dispose on the array - if necessary. Also, for elements of reference
        /// types (e.g. ILCell), retrieving and storing elements from/into the System.Array directly does 
        /// not simulate a value semantic as all other functions do! This means, references are copied. Attention
        /// must be paid to dereference / clone elements accordingly. </para></remarks>
        public ElementType[] GetArrayForRead() {
            return Storage.GetArrayForRead();
        }

        #endregion

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
            ILRetArray<ElementType> ret = new ILRetArray<ElementType>((ILDenseStorage<ElementType>)Storage.Clone());
            return ret;
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
        public ILRetArray<ElementType> Concat(ILInArray<ElementType> A, int dim) {
            using (ILScope.Enter(A))
                return new ILRetArray<ElementType>(Storage.Concat(A.Storage, dim));
        }
        /// <summary>
        /// Copy values of all elements into System.Array.
        /// </summary>
        /// <param name="outArray">[Output] System.Array, holding all element values of this ILDenseStorage.</param>
        /// <remarks>The System.Array may be predefined. If its length is sufficient, it will be used and 
        /// its leading elements will be overwritten when function returns. If 'outArray' is null or has too few elements, 
        /// it will be recreated from the ILNumerics memory pool.</remarks>
        public void ExportValues(ref ElementType[] outArray) {
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
            return Storage.GetEnumerator(); 
        }
        /// <summary>
        /// Gives away internal storage for further use (e.g. in ILArray), disposes this array
        /// </summary>
        /// <returns>Internal storage</returns>
        internal ILDenseStorage<ElementType> GiveStorageAwayOrClone() {
            if (m_scopeCounter <= 0) {
                ILDenseStorage<ElementType> ret = Storage;
                m_storage = null;
                return ret;
            } else {
                return (ILDenseStorage<ElementType>)Storage.Clone(); 
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
        /// <returns>Array being created by multiple replications of this array along 
        /// arbitrary dimensions according to <paramref name="dims"/></returns>
        internal ILRetArray<ElementType> Repmat(params int[] dims) {
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
        public ILRetArray<ElementType> Reshape(ILSize dimensions) {
            using (ILScope.Enter()) {
                ILArray<ElementType> ret = C;
                ret.Storage.Reshape(dimensions);
                return ret;
            }
        }
        /// <summary>
        /// Reshaped copy of this array
        /// </summary>
        /// <param name="dimensions">New dimensions of the array</param>
        /// <returns>Reshaped copy of the array</returns>
        /// <remarks><para>The current instance will not be changed. A new array is created, having 
        /// the elements of this array and a shape as determined by <paramref name="dimensions"/>.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the number of elements in 'newDimension'
        /// do not match the number of elements in this array.</exception>
        public ILRetArray<ElementType> Reshape(params int[] dimensions) {
            return Reshape(new ILSize(dimensions));
        }
        /// <summary>
        /// Subarray creation
        /// </summary>
        /// <param name="size">Range specification, defining the size of the subarray</param>
        /// <returns>Subarray as copy of a part of this array</returns>
        /// <remarks>Consult the ILNumerics subarray documentation for all subarray indexing rules.</remarks>
        public ILRetArray<ElementType> Subarray(params ILBaseArray[] size) {
            using (ILScope.Enter(size))
                return new ILRetArray<ElementType>(Storage.Subarray(size));
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
        public ILRetArray<ElementType> Shifted(int shift) {
            return new ILRetArray<ElementType>(Storage.ShiftDimensions(shift));
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
        /// <summary>
        /// Give that storage away for in-place operations, if possible (depends on scope and temp type of array)
        /// </summary>
        /// <returns>true if the storage of the array is about to get disposed anyway</returns>
        /// <remarks>The function investigates the state of the array. If this is a temporary array in 
        /// the outer most scope, it would get disposed after the </remarks>
        internal bool TryGetStorage4InplaceOp(out ElementType[] array) {
            // check if this array is yet to be available 
            if (!Settings.AllowInArrayAssignments && m_isTempArray && m_scopeCounter <= 1) {
                // it's efficient only if we dont have to make a copy of the underlying array
                if (Storage.ReferenceCount == 1) {
                    ILDenseStorage<ElementType> storage = Storage;
                    m_storage = null;
                    array = storage.GetArrayForWrite();
                    return true;
                }
            }
            array = null;
            return false;
        }
        #endregion
    }
}
