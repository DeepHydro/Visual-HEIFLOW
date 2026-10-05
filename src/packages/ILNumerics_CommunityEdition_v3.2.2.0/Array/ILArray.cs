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
using System.Security; 
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
    /// The main rectangular array to be used in algorithms 
    /// </summary>
    /// <typeparam name="ElementType">Inner type. This will mostly be a system numeric type or a 
    /// complex floating point type.</typeparam>
    /// <remarks>This class serves as the main rectangular array, holding arbitrary elements (usually numeric types) 
    /// in arbitrary dimensions.
    /// <para>Arrays of this type may use any type as generic element. However, common mathematical functions and operators 
    /// are defined for a limited number of inner types only. All binary operations (+,-,*,/,<![CDATA[<,>,<=]]>,etc.) are 
    /// defined for two arrays with the same <i>numeric type</i>, would it be from the <c>System</c> namespace (<c>double</c>, 
    /// <c>int</c>,...) or <c>ILNumerics.complex</c>/ <c>ILNumerics.fcomplex</c>. Most algebraic functions require floating point 
    /// types. See the <see cref="ILNumerics.ILMath"/> class for a list of all computational functions.</para>
    /// <para>Arrays are capable of creating flexible <a href="http://ilnumerics.net/$Subarray0.html" target="ILMain">subarrays</a> 
    /// and to get altered at runtime. Read about all details of ILNumerics arrays in the 
    /// <a href="http://ilnumerics.net/$Arrays.html" target="ILMain">ILNumerics Array documentation</a>.</para>
    /// <para>Arrays of this type are dense arrays. Cloning arrays is done as lazy 
    /// copy on write, i.e. clones do only use new memory, if attempting to write on them. Arrays integrate into the memory 
    /// management of ILNumerics. Read about the most <a href="http://ilnumerics.net/$GeneralRules.html" target="ILMain">important 
    /// simple rules</a>, for using arrays in custom computational functions.</para>
    /// <para>Arrays come with overloaded mathematical operators, allowing for a convenient syntax. A 
    /// sophisticated memory management in the back will make sure, that as little memory as needed is used, even in 
    /// expressions like: a + c * 2 / abs(sin(c) * -b / log(a)). Here all arrays are of the same size. Evaluating 
    /// this expression does only need the memory of twice the size of one array. Memory gets collected and reused 
    /// for every subexpression evaluation. Further optimization options exist, as described in 
    /// <a href="http://ilnumerics.net/$PerfMemoryOpt.html" target="ILMain">Optimizing Algorithm Performance</a>.</para>
    /// </remarks>
    /// <example><para>A simple example demonstrating some uses of arrays in a very simple application:</para>
    /// <code>using System;
    ///using System.Collections.Generic;
    ///using System.Linq;
    ///using System.Text;
    ///using ILNumerics; 
    ///
    ///
    ///namespace ConsoleApplication1 {
    ///    class Program : ILMath {
    ///        static void Main(string[] args) {
    ///            ILArray&lt;double> A = rand(10,20);
    ///            ILArray&lt;double> B = A * 30 + 100; 
    ///            ILLogical C = any(multiply(B,B.T)); 
    ///            Console.Out.Write(-B); 
    ///            Console.ReadKey(); 
    ///        }
    ///    }
    ///}
    ///</code>
    /// </example>
    /// <seealso cref="ILNumerics.ILLogical"/>
    /// <seealso cref="ILNumerics.ILCell"/>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public sealed class ILArray<ElementType> : ILDenseArray<ElementType> {

        private static bool s_isTempArray = false; 

        #region constructors
        /// <summary>
        /// Create new ILArray, specify (dense) storage
        /// </summary>
        /// <param name="storage"></param>
        internal ILArray(ILDenseStorage<ElementType> storage)
            : base(storage, s_isTempArray) {
        }
        /// <summary>
        /// create new ILArray, specify dimensions
        /// </summary>
        /// <param name="dimensions"></param>
        private ILArray(ILSize dimensions)
            : base(new ILDenseStorage<ElementType>(dimensions), s_isTempArray) {
        }
        
        /// <summary>
        /// create new ILArray, specify storage and if the new array should be disposed automatically
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="registerForDisposal"></param>
        internal ILArray(ILDenseStorage<ElementType> storage, bool registerForDisposal)
            : base(storage, s_isTempArray) {
            if (registerForDisposal) 
                ILScope.Context.RegisterArray(this);
        }
        /// <summary>
        /// create new ILArray, specify dimensions
        /// </summary>
        /// <param name="dimensions"></param>
        /// <param name="registerForDisposal"></param>
        private ILArray(ILSize dimensions, bool registerForDisposal)
            : base(new ILDenseStorage<ElementType>(dimensions), s_isTempArray) {
            if (registerForDisposal) 
                ILScope.Context.RegisterArray(this);
        }
        /// <summary>
        /// create new ILArray from System.Array
        /// </summary>
        /// <param name="elements">System.Array</param>
        /// <param name="size">dimension specifier</param>
        internal ILArray(ElementType[] elements, ILSize size) :
            base(new ILDenseStorage<ElementType>(elements, size), s_isTempArray) {
        }
        /// <summary>
        /// create new ILArray from System.Array, optionally register the array for disposal 
        /// </summary>
        /// <param name="elements">System.Array</param>
        /// <param name="registerForDisposal">if true, the array will be disposed once the current scope is closed</param>
        /// <param name="size">dimension specifier</param>
        internal ILArray(ElementType[] elements, ILSize size, bool registerForDisposal) :
            base(new ILDenseStorage<ElementType>(elements, size), s_isTempArray) {
                if (registerForDisposal)
                    ILScope.Context.RegisterArray(this);
        }
        /// <summary>
        /// create new ILArray from System.Array
        /// </summary>
        /// <param name="elements">System.Array</param>
        /// <param name="size">dimension specifier</param>
        internal ILArray(ElementType[] elements, params int[] size) :
            base(new ILDenseStorage<ElementType>(elements, new ILSize(size)), s_isTempArray) {
        }
        /// <summary>
        /// create new ILArray from System.Array
        /// </summary>
        /// <param name="elements">variable length System.Array</param>
        internal ILArray(params ElementType[] elements) :
            base(new ILDenseStorage<ElementType>(elements, new ILSize(1, elements.Length)), s_isTempArray) {
        }


        /// <summary>
        /// [deprecated] Create empty array of arbitrary size
        /// </summary>
        /// <param name="size">Dimension sizes</param>
        /// <returns>New empty array</returns>
        /// <remarks>This function is markes as deprecated and only included for compatibility reasons.
        /// It will be removed in a future version. Use <see cref="ILNumerics.ILMath.empty{T}()"/> instead.</remarks>
        [Obsolete("Use ILNumerics.ILMath.empty{T}() instead")]
        public static ILRetArray<ElementType> empty(params int[] size) {
            if (size == null || size.Length == 0) 
                return new ILArray<ElementType>(ILSize.Empty00); 
            ILArray<ElementType> ret = new ILArray<ElementType>(new ILSize(size)); 
            if (!ret.IsEmpty) 
                throw new ILArgumentException("'size' must specify the size of an empty array"); 
            return ret; 
        }
        /// <summary>
        /// [deprecated] Create empty array of arbitrary size
        /// </summary>
        /// <param name="size">Dimension sizes</param>
        /// <returns>New empty array</returns>
        /// <remarks>This function is markes as deprecated and only included for compatibility reasons.
        /// It will be removed in a future version. Use <see cref="ILNumerics.ILMath.empty{T}()"/> instead.</remarks>
        [Obsolete("Use ILNumerics.ILMath.empty{T}() instead")]
        public static ILRetArray<ElementType> empty(ILSize size) {
            ILArray<ElementType> ret = new ILArray<ElementType>(size); 
            if (!ret.IsEmpty)
                throw new ILArgumentException("'size' must specify the size of an empty array"); 
            return ret; 
        }
        #endregion

        #region implicit cast operators
        
        #region constructional operators
        /// <summary>
        /// Implicitly convert scalar to array of size 1x1 (scalar).
        /// </summary>
        /// <param name="val">System type of size scalar</param>
        /// <returns>New ILArray of type ILArray <![CDATA[<typeof(val)>]]> of size 1x1 
        /// holding the only element with value of val.
        /// </returns>
        public static implicit operator ILArray<ElementType> (ElementType val) {
            ILArray<ElementType> ret = new ILArray<ElementType>(
                        new ILDenseStorage<ElementType>(
                            new ElementType[1] {val}, 
                            new ILSize(1,1)));
            return ret; 
        }
        /// <summary>
        /// Implicitly cast one dimensional System.Array to ILNumerics array (vector)
        /// </summary>
        /// <param name="A">1-dimensional system array, arbitrary type</param>
        /// <returns>ILNumerics array of same element type as elements of A. If A is null: empty array.</returns>
        /// <remarks><para>The System.Array A will directly be used for the new ILNumerics array! 
        /// No copy will be done! Make sure, not to reference A after this conversion</para>
        /// <para>The size of the result depends on the global option <c>Settings.CreateRowVectorByDefault</c></para></remarks>
        /// <seealso cref="Settings.CreateRowVectorsByDefault"/>
        public static implicit operator ILArray<ElementType> (ElementType[] A) {
            if (A == null) 
                return null;
            ILArray<ElementType> ret; 
            if (Settings.CreateRowVectorsByDefault) {
                ret = new ILArray<ElementType>(A, 1, A.Length);
            } else {
                ret = new ILArray<ElementType>(A, A.Length, 1);
            }
            ILScope.Context.RegisterArray(ret); 
            return ret; 
        }
        /// <summary>
        /// Implicitly convert n-dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">Arbitrarily sized System.Array</param>
        /// <returns>If A is null: empty array. Else: new ILNumerics array of the same size as A</returns>
        /// <remarks>The inner type of input array <paramref name="A"/> must match the requested type
        /// <typeparamref name="ElementType"/>. The resulting ILArray will reflect all dimensions of 
        /// A. Elements of A will get copied to elements of the output array (shallow copy).</remarks>
        /// <seealso cref="Settings.CreateRowVectorsByDefault"/>
        /// <exception cref="ILNumerics.Exceptions.ILCastException">If type of input does not match 
        /// ElementType</exception>
        public static implicit operator ILArray<ElementType> (Array A) {
            if (A == null) 
                return null; 
            if (A.Length == 0) {
                return new ILArray<ElementType>(ILSize.Empty00);
            }
            int [] dims = new int[A.Rank]; 
            ElementType [] retArr = ILMemoryPool.Pool.New<ElementType>(A.Length);
            int posArr = 0; 
            for (int i = 0; i < dims.Length; i++) {
                dims[i] = A.GetLength(dims.Length-i-1);
            }
            if (dims.Length == 1 && Settings.CreateRowVectorsByDefault) {
                dims = new int[2] {1, dims[0]}; 
            }
            if (A.GetType().GetElementType() == typeof(ElementType)) {
                foreach (ElementType item in A)
                    retArr[posArr++] = item;
            } else {
                
            }
            ILArray<ElementType> ret = new ILArray<ElementType>(retArr,dims);
            ILScope.Context.RegisterArray(ret);
            return ret; 
        }
        /// <summary>
        /// Implicitly cast two dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">2-dimensional System.Array</param>
        /// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        public static implicit operator ILArray<ElementType>(ElementType[,] A) {
            if (A == null) 
                return null; 
            if (A.Length == 0) {
                return new ILArray<ElementType>(ILSize.Empty00);
            }
            int[] dims = new int[2];
            ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(A.Length);
            int posArr = 0;
            for (int i = 0; i < 2; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (ElementType item in A)
                retArr[posArr++] = item;
            ILArray<ElementType> ret = new ILArray<ElementType>(retArr, dims);
            ILScope.Context.RegisterArray(ret);
            return ret; 
        }
        /// <summary>
        /// Implicitly cast three dimensional System.Array to ILNumerics array
        /// </summary>
        /// <param name="A">3-dimensional System.Array</param>
        /// <returns>If A is null: empty array. ILNumerics array of same size and type as A otherwise.</returns>
        public static implicit operator ILArray<ElementType>(ElementType[,,] A) {
            if (A == null)
                return null;
            if (A.Length == 0) {
                return new ILArray<ElementType>(ILSize.Empty00);
            }
            int[] dims = new int[3];
            ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(A.Length);
            int posArr = 0;
            for (int i = 0; i < 3; i++) {
                dims[i] = A.GetLength(dims.Length - i - 1);
            }
            foreach (ElementType item in A)
                retArr[posArr++] = item;
            ILArray<ElementType> ret = new ILArray<ElementType>(retArr, dims);
            ILScope.Context.RegisterArray(ret);
            return ret; 
        }
        #endregion 

        #region conversional operators
        /// <summary>
        /// "Persistence cast" make a steady array out of a temp array
        /// </summary>
        /// <param name="A">Temporary array</param>
        /// <returns>Steady ILArray, will survive multiple usages</returns>
        /// <remarks>This is one of the most important casts in the ILNumerics framework. It changes temporary 
        /// arrays from temporary state to ('steady') ILArrays and persistent state. The inner storage is kept and used for 
        /// the new array. The new arrays can than get used multiple times in any function. In contrast to
        /// that, ILRetArrays are disposed off after first use.</remarks>
        public static implicit operator ILArray<ElementType>(ILRetArray<ElementType> A) {
            if (object.Equals(A, null))
                return null;
            ILArray<ElementType> ret = new ILArray<ElementType>(A.GiveStorageAwayOrClone());
            ILScope.Context.RegisterArray(ret); 
            return ret; 
        }
        /// <summary>
        /// "Persistence cast" make a steady array out of an input array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Steady ILArray, will survive multiple usages</returns>
        /// <remarks>This is one of the most important casts in the ILNumerics framework. It changes temporary 
        /// arrays from temporary state to ('steady') ILArrays and persistent state. The inner storage is kept and used for 
        /// the new array. The new arrays can than get used multiple times in any function. In contrast to
        /// that, ILRetArrays are disposed off after first use.</remarks>
        public static implicit operator ILArray<ElementType>(ILInArray<ElementType> A) {
            if (object.Equals(A, null))
                return null;
            ILArray < ElementType > ret = new ILArray<ElementType>(new ILDenseStorage<ElementType>(
                A.Storage.GetDataArray(), A.Size));
            ILScope.Context.RegisterArray(ret);
            return ret;
        }
        /// <summary>
        /// "Persistence cast" make a steady array out of an input array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Steady ILArray, will survive multiple usages</returns>
        /// <remarks>This is one of the most important casts in the ILNumerics framework. It changes temporary 
        /// arrays from temporary state to ('steady') ILArrays and persistent state. The inner storage is kept and used for 
        /// the new array. The new arrays can than get used multiple times in any function. In contrast to
        /// that, ILRetArrays are disposed off after first use.</remarks>
        public static implicit operator ILArray<ElementType>(ILOutArray<ElementType> A) {
            if (object.Equals(A, null))
                return null;
            ILArray<ElementType> ret = new ILArray<ElementType>(new ILDenseStorage<ElementType>(
                A.Storage.GetDataArray(), A.Size));
            ILScope.Context.RegisterArray(ret);
            return ret;
        }
        /// <summary>
        /// Convert dense array to scalar temporary cell 
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>Scalar cell having the only element with a clone of array</returns>
        public static implicit operator ILRetCell(ILArray<ElementType> A) {
            using (ILScope.Enter(A)) {
                ILRetCell ret = new ILRetCell(ILSize.Scalar1_1, A.Storage);
                ret.Storage.FromImplicitCast = true; 
                return ret; 
            }
        }
        #endregion
        #endregion

        #region memory management
        /// <summary>
        /// Assign another array to this array variable. This is an optional, yet more efficient alternative to using '='
        /// </summary>
        /// <param name="value">New array</param>
        /// <remarks>By assigning to this property, this array is immediately released to the memory pool and replaced by the new array. In difference to that, 
        /// by using the common '=' assignment operator, the array is released only at the time, the current 
        /// <see cref="ILNumerics.ILScope.Enter"/> block  is left. Therefeore, prefere this method, if a 
        /// smaller memory pool is crucial. However, for variables of type <c>ILArray</c>, regular assignments are allowed as well.</remarks>
        /// <seealso cref="ILNumerics.ILOutArray{T}.Assign"/>
        public ILRetArray<ElementType> a {
            set { Assign(value); }
            get { return this.C; }
        }
        /// <summary>
        /// Assign another array to this array variable. This is an optional, yet more efficient alternative to '='
        /// </summary>
        /// <param name="value">New array</param>
        /// <remarks>By using this method, this array is immediately released to the memory pool and replaced by the new array. In difference to that, 
        /// by using the common '=' assignment operator, the array is released only at the time, the current 
        /// <see cref="ILNumerics.ILScope.Enter"/> block  is left. Therefeore, prefere this method, if a 
        /// smaller memory pool is crucial.</remarks>
        /// <seealso cref="ILNumerics.ILOutArray{T}.Assign"/>
        public void Assign(ILRetArray<ElementType> value) {
            if (!IsDisposed)
                Storage.Dispose();
            m_storage = value.GiveStorageAwayOrClone();
            //ILScope.Context.RegisterArray(this);  
        }
        internal override bool EnterScope() {
            return false; 
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
        /// <para>For elements of reference types (e.g. ILCell), retrieving and storing elements from/into the System.Array directly 
        /// obviously does not simulate a value semantic as all other common API methods do! This means, references are copied. Attention
        /// must be paid to dereference / clone elements accordingly or (recommended) prevent from using this 
        /// function with reference element types.</para></remarks>
        public ElementType[] GetArrayForWrite() {
            return Storage.GetArrayForWrite();
        }

        #endregion

        #region mutability + indexer 
        /// <summary>
        /// Set single value to element at index specified
        /// </summary>
        /// <param name="value">New value</param>
        /// <param name="idx">Index of element to be altered</param>
        public void SetValue(ElementType value, params int[] idx) {
            Storage.SetValueTyped(value, idx);
        }

        /// <summary>
        /// Alter range of this array
        /// </summary>
        /// <param name="value">Array with new values</param>
        /// <param name="range">Range specification</param>
        public void SetRange(ILInArray<ElementType> value, params ILBaseArray[] range) {
            using (ILScope.Enter(value)) 
            using (ILScope.Enter(range)) {
                if (object.Equals(value, null)) {
                    Storage.IndexSubrange(null, range);
                } else {
                    Storage.IndexSubrange(value.Storage, range);
                }
            }
        }
        /// <summary>
        /// Subarray creation/ manipulation/ deletion
        /// </summary>
        /// <param name="range">Range specification, defining the size of the subarray</param>
        /// <returns>Subarray as copy of this array</returns>
        public ILRetArray<ElementType> this[params ILBaseArray[] range] {
            get {
                using (ILScope.Enter(range))
                    return new ILRetArray<ElementType>(Storage.Subarray(range));
            }
            set {
                SetRange(value, range); 
            }
        }

        #endregion

    }
}
