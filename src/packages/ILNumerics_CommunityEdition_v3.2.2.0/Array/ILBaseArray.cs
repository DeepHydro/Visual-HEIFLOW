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
using ILNumerics;
using ILNumerics.Misc;
using ILNumerics.Storage;
using ILNumerics.Native; 
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections; 

namespace ILNumerics {
    /// <summary>
    /// Base type for all arrays in ILNumerics
    /// </summary>
    /// <remarks>All numerical arrays derive from ILBaseArray. ILBaseArrays itself 
    /// cannot be instantiated. Currently, only ILArray<![CDATA[<>]]> exist, which describe 
    /// a rectangular array as full (solid) or referencing array. There are plans to 
    /// extend the collection of derived types to encompass triangular, diagonal and sparse arrays. 
    /// </remarks>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public abstract class ILBaseArray : IDisposable {

        #region attributes
        /// <summary>
        /// Name of this array
        /// </summary>
        protected String m_name = "";
        internal ILStorage m_storage; 
        internal int m_scopeCounter = 0;
        internal bool m_isTempArray;
        #endregion

        #region properties
        internal int ScopeID {
            get { return m_scopeCounter; }

        }
        internal bool IsDisposed {
            get { return m_storage == null || m_storage.IsDisposed; }
        }

        internal ILStorage Storage {
            get { return m_storage; }
        }

        /// <summary>
        /// Size descriptor specification
        /// </summary>
        public virtual ILSize Size {
            get {
                return Storage.Size;
            }
        }
        /// <summary>
        /// [deprecated] Use 'Size' as size descriptor!
        /// </summary>
        [Obsolete("Use 'Size' instead!")]
        public ILSize Dimensions {
            get {
                return Size;
            }
        }
        /// <summary>
        /// Size descriptor shortcut
        /// </summary>
        public virtual ILSize S {
            get {
                return Storage.Size;
            }
        }
        /// <summary>
        /// [deprecated] Use 'S' as size descriptor!
        /// </summary>
        [Obsolete("Use S instead!")]
        public virtual ILSize D {
            get {
                return S;
            }
        }
        /// <summary>
        /// Length of the longest dimension of this instance
        /// </summary>
        /// <remarks>This property is readonly.</remarks>
        public virtual int Length {
            get {
                return Size.Longest;
            }
        }
        /// <summary>
        /// Gets the name of this array or sets it
        /// </summary>
        public String Name {
            get {
                return m_name;
            }
            set {
                m_name = value;
            }
        }
        /// <summary>
        /// Test if this instance is a scalar
        /// </summary>
        /// <remarks>This attribute is readonly. It returns: Size.NumberOfElements == 1.</remarks>
        public virtual bool IsScalar {
            get {
                return Size.NumberOfElements == 1;
            }
        }
        /// <summary>
        /// Test if this instance is a matrix
        /// </summary>
        /// <remarks>In order for an array to be a matrix the number of <b>non singleton</b> 
        /// dimensions must equal 2. This attribute is readonly.</remarks>
        public virtual bool IsMatrix {
            get {
                if (Size.NumberOfDimensions == 2) 
                    return true;
                return (Size.Squeeze().NumberOfDimensions == 2); 
            }
        }
        /// <summary>
        /// Test if this array is a vector
        /// </summary>
        /// <remarks>In order for an array to be a vector the number of <b>non singleton</b> 
        /// dimensions must equal 1. Keep in mind that all ILArrays have at least 2 dimensions. Therefore 
        /// it is not sufficient to test for the number of dimensions, but to take the number of 
        /// <b>non singleton</b> dimensions into account. This attribute is readonly.</remarks>
        public virtual bool IsVector {
            get {
                return (Size[0] == 1 || Size[1] == 1) && Size.NumberOfDimensions == 2;
            }
        }
        /// <summary>
        /// Test if this array instance is a row vector
        /// </summary>
        public virtual bool IsRowVector {
            get {
                return Size[0] == 1 && Size.NumberOfDimensions == 2;
            }
        }
        /// <summary>
        /// Test if this array instance is a column vector
        /// </summary>
        public virtual bool IsColumnVector {
            get {
                return Size[1] == 1 && Size.NumberOfDimensions == 2;; 
            }
        }
        /// <summary>
        /// Test if this instance is an empty array (number of elements stored = 0)
        /// </summary>
        public virtual bool IsEmpty {
            get {
                return Size.NumberOfElements == 0;
            }
        }
        #endregion

        #region constructors
        internal ILBaseArray(ILStorage storage, bool isTempArray) { 
            m_storage = storage;
            m_isTempArray = isTempArray; 
        }
        /// <summary>
        /// Implicit cast from scalar of typeof(a) to <c>ILRetArray&lt;typeof(A)&gt;</c>
        /// </summary>
        /// <param name="a">Input scalar</param>
        /// <returns>A ILRetArray of same type as <paramref name="a"/> and size 1x1</returns>
        public static implicit operator ILBaseArray(double a) {
            return new ILRetArray<double>(new double[] { a }, ILSize.Scalar1_1);  
        }
        /// <summary>
        /// Implicit cast from scalar of typeof(A) to ILRetArray&lt;typeof(A)&gt;
        /// </summary>
        /// <param name="a">Input scalar</param>
        /// <returns>A ILRetArray of same type as <paramref name="a"/> ans size 1x1</returns>
        public static implicit operator ILBaseArray(complex a) {
            return new ILRetArray<complex>(new complex[] { a, 0 }, ILSize.Scalar1_1);  
        }
        /// <summary>
        /// Implicit cast from scalar of typeof(A) to ILRetArray&lt;typeof(A)&gt;
        /// </summary>
        /// <param name="a">Input scalar</param>
        /// <returns>A ILRetArray of same type as <paramref name="a"/> ans size 1x1</returns>
        public static implicit operator ILBaseArray(fcomplex a) {
            return new ILRetArray<fcomplex>(new fcomplex[] { a, 0 }, ILSize.Scalar1_1);  
        }
        /// <summary>
        /// Implicit cast from scalar of typeof(A) to ILRetArray&lt;typeof(A)&gt;
        /// </summary>
        /// <param name="s">Input scalar</param>
        /// <returns>A ILRetArray of same type as <paramref name="s"/> and size 1x1</returns>
        public static implicit operator ILBaseArray(string s) {
            return new ILRetArray<string>(new string[] { s }, ILSize.Scalar1_1); 
        }
        //public static implicit operator ILBaseArray(string[] arr) {
        //    return new ILRetArray<string>(new string[] { String.Join(";", arr) }, ILSize.Scalar1_1); 
        //}
        /// <summary>
        /// Implicit cast from scalar of typeof(A) to ILRetArray&lt;typeof(A)&gt;
        /// </summary>
        /// <param name="a">Input scalar</param>
        /// <returns>A ILRetArray of same type as <paramref name="a"/> ans size 1x1</returns>
        public static implicit operator ILBaseArray(double[] a) {
            if (a == null || a.Length == 0) {
                return new ILRetArray<double>(ILSize.Empty00); 
            }
            ILCell ret = new ILCell(1,a.Length);
            for (int i = 0; i < a.Length; i++) {
                ret.SetValue(new ILRetArray<double>(new double[] { a[i] }, ILSize.Scalar1_1),0,i);  
            }
            //ret.SetValue("int[]",1,0); 
            return ret; 
        }
        //public static implicit operator ILBaseArray(int[] a) {
        //    if (a == null || a.Length == 0) {
        //        return new ILRetArray<int>(ILSize.Empty00); 
        //    }
        //    ILCell ret = new ILCell(1,a.Length);
        //    for (int i = 0; i < a.Length; i++) {
        //        ret.SetValue(new ILRetArray<int>(new int[] { a[i] }, ILSize.Scalar1_1),0,i);  
        //    }
        //    //ret.SetValue("int[]",1,0); 
        //    return ret; 
        //}
        #endregion

        #region virtual
        /// <summary>
        /// Compare elements and shape of this array with another array
        /// </summary>
        /// <param name="A">Other array</param>
        /// <returns>true if shape and element values of both arrays match, false otherwise</returns>
        /// <remarks><para>'Equals' accepts two vectors even if the orientations do not match. Therefore, a row vector 
        /// with the same element values than another column vector would be considered equal to each other.</para></remarks>
        public override bool Equals(object A) {
            ILBaseArray baseArray = A as ILBaseArray; 
            if (baseArray != null) {
                return Storage.Equals(baseArray.Storage);
            }
            if (IsScalar && A is System.ValueType) {
                return String.Equals(A.ToString(),Storage.GetValue(0).ToString()); 
            }
            return false; 
        }
        /// <summary>
        /// Generate a hash code based on the current arrays values
        /// </summary>
        /// <returns>Hash code</returns>
        /// <remarks>The hashcode is created by taking the values currently stored in the array into account.
        /// Therefore, the function iterates over all elements in the array - which makes it somehow an expensive 
        /// operation. Take this into account, if you consider using large arrays in collections like dictionaries 
        /// or hashtables, which make great use of hash codes.</remarks>
        public override int GetHashCode() {
            return Storage.GetHashCode();
        }

        internal virtual bool EnterScope() {
            if (m_scopeCounter < 2) {
                m_scopeCounter++;
                return true;
            }
            return false; 
        }
        internal virtual void LeaveScope() {
            if (--m_scopeCounter <= 0) {
                Dispose();
            }
        }

        #endregion

        #region abstract interface
        /// <summary>
        /// Clone this array (shallow) 
        /// </summary>
        /// <returns>ILBaseArray as new representation of this storages data.</returns>
        /// <remarks>The object returned will be of the same size than this array.
        /// This this is a 'shallow' copy only! I.e., if elements are copied only. If they are 
        /// references to any objects, those objects are not replicated.</remarks>
        internal abstract ILBaseArray Clone(); 

        /// <summary>
        /// Determine if this array is of complex inner type.
        /// </summary>
        public abstract bool IsComplex {
            get; 
        }
        /// <summary>
        /// Determine if this array is of numeric inner type.
        /// </summary>
        public abstract bool IsNumeric {
            get; 
        }
        /// <summary>
        /// Print values of this instance to a stream. 
        /// </summary>
        /// <param name="outStream">Stream to write the values into.</param>
        /// <param name="format">Format string to be used for output. See <see cref="System.String.Format(string,object)"/> for a specification
        /// of valid formating expressions. This flag is only used, when 'method' is set to 'Serial'.</param>
        /// <param name="method">A constant out of <see cref="ILArrayStreamSerializationFlags"/>. Specifies the way 
        /// the values will be serialized.</param>
        /// <remarks><para>If method 'Formatted' is used, any occurences of NewLine character(s) 
        /// will be replaced from the format string before applying to the elements. This is done to 
        /// prevent the format from breaking the 'page' style for the output.</para>
        /// <para>If 'method' is set to 'Matlab', the array will be written as Matfile version 5.0. No compression will be used. The internal 'Name' property will be used as 
        /// the array name for writing. This array instance will be the only array in the mat file. If you want to write several arrays bundled into one mat file, use the MatFile class to
        /// create a collection of arrays and write the MatFile to stream.</para></remarks>
        public abstract void ToStream(Stream outStream, string format, ILArrayStreamSerializationFlags method);

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <returns>String representation of content</returns>
        public override string ToString() {
            return ToString(0);
        }
        /// <summary>
        /// Convert to string with limited length
        /// </summary>
        /// <param name="maxLength">Maximal length of returned string; set to 0 to not limit result</param>
        /// <returns>String representation of content</returns>
        public virtual string ToString(int maxLength) {
            string ret = ShortInfo(); 
            if (m_storage.Size.NumberOfElements > 1) 
                ret += Environment.NewLine + Storage.ValuesToString(maxLength); 
            return ret; 
        }
        /// <summary>
        /// Short textual summary of this instance, used for debug output
        /// </summary>
        /// <returns>String representation of type and size</returns>
        /// <remarks>The type of elements and the size of the array are displayed. If the array
        /// is scalar, its value is displayed next to the type.</remarks>
        public virtual String ShortInfo() {
            return Storage.ShortInfo(); 
        }
        /// <summary>
        /// Dispose this array and all its content
        /// </summary>
        public void Dispose() {
            if (IsDisposed) return;     
            if (Storage != null) {
                Storage.Dispose(); 
            }
        }
        #endregion

    }

}