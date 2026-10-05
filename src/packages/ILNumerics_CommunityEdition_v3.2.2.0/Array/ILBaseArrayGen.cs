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
using ILNumerics.Exceptions; 
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Xml; 

namespace ILNumerics {

    /// <summary>
    /// Typed base class for all ILNumerics data storage classes for any storage type. 
    /// </summary>
    /// <remarks><para>You should not use this type directly. It serves as a base class 
    /// for all typed storages only and will be used by derived classes like ILArray<![CDATA[<>]]>.</para>
    /// <para>If you are looking for an (untyped) base class to be used as generic class for any ILArray types, you should use ILBaseArray instead!</para>
    /// </remarks>
    [Serializable]
    [System.Security.SecuritySafeCritical]
    public abstract partial class ILBaseArray<ElementType> 
        : ILBaseArray, IEnumerable<ElementType> {
        
        internal ILBaseArray(ILStorage storage, bool isTempArray) : base(storage, isTempArray) { }

        #region attributes
        #endregion

        #region properties 
        /// <summary>
        /// Determine if this array has complex elements.
        /// </summary>
        public override bool IsComplex {
            get {
                return (this is ILBaseArray<complex> 
                 || this is ILBaseArray<fcomplex>);
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
        /// </remarks>
        public override bool IsNumeric {
            get {
                if (this is ILBaseArray<double> ||
                    this is ILBaseArray<float> ||
                    this is ILBaseArray<complex> ||
                    this is ILBaseArray<fcomplex> ||
                    this is ILBaseArray<byte> ||
                    this is ILBaseArray<char> ||
                    this is ILBaseArray<Int16> ||
                    this is ILBaseArray<Int32> ||
                    this is ILBaseArray<Int64> ||
                    this is ILBaseArray<UInt16> ||
                    this is ILBaseArray<UInt32> ||
                    this is ILBaseArray<UInt64>)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Access to internal typed storage 
        /// </summary>
        internal new ILStorage<ElementType> Storage {
            get { return (m_storage as ILStorage<ElementType>); }
        }

        #endregion

        #region public interface
        /// <summary>
        ///  Serialize this array into a binary stream.
        /// </summary>
        /// <param name="outStream">System.IO.Stream to receive the byte stream 
        /// for this ILBaseArray</param>
        /// <returns>True on success, false on error.</returns>
        public virtual bool Serialize(Stream outStream) {
            try {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(outStream, this);
                return true;
            } catch (Exception) {
                return false;
            }
        }
        /// <summary>
        /// Deserialize / restore array from binary stream 'inStream'
        /// </summary>
        /// <param name="inStream">System.IO.Stream to reconstruct the 
        /// array from</param>
        /// <returns>Array reconstructed from stream.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">If the array could not get restored</exception>
        public static ILBaseArray<ElementType> Deserialize(Stream inStream) {
            try {
                BinaryFormatter bf = new BinaryFormatter();
                ILBaseArray<ElementType> ret = (ILBaseArray<ElementType>)bf.Deserialize(inStream);
                return ret;
            } catch (Exception) {
                throw;
            }
        }
        public void ToXML(XmlWriter writer) {
            writer.WriteAttributeString("element_type", typeof(ElementType).Name);
            writer.WriteElementString("Size",String.Join(",",S.ToIntArrayEx(S.NumberOfDimensions))); 
            writer.WriteStartElement("ColumnMajor");
            foreach (var val in this)
                writer.WriteString(val.ToString() + ","); 
            writer.WriteEndElement(); 
        }
        /// <summary>
        /// Get single element from this array
        /// </summary>
        /// <param name="idx">Indices, location of element</param>
        /// <returns>The selected value</returns>
        public virtual ElementType GetValue(params int[] idx) {
            return Storage.GetValueTyped(idx); 
        }
        /// <summary>
        /// Get minimum and maximum value of all elements - if any
        /// </summary>
        /// <param name="min">[Output] Minimum value</param>
        /// <param name="max">[Output] Maximum value</param>
        /// <returns>true if the limits exists and could be computed, false otherwise</returns>
        /// <remarks>Empty arrays will return false. In this case the output parameter will be: default(ElementType).
        /// </remarks>
        public virtual bool GetLimits(out ElementType min, out ElementType max) {
            return Storage.GetLimits(out min, out max); 
        }
        /// <summary>
        /// Get minimum and maximum value of all elements - if any
        /// </summary>
        /// <param name="min">[Output] Minimum value</param>
        /// <param name="max">[Output] Maximum value</param>
        /// <param name="includeInfNaNs">true: recognize Inf, NaN values; false: ignore those values</param>
        /// <returns>true if the limits exists and could be computed, false otherwise</returns>
        /// <remarks>Empty arrays will return false. In this case the output parameter will be: default(ElementType).</remarks>
        internal bool GetLimits(out ElementType min, out ElementType max, bool includeInfNaNs) {
            return Storage.GetLimits(out min, out max, includeInfNaNs); 
        }
        #endregion

        #region IEnumerable<ILBaseArray<ElementType>> Member

        /// <summary>
        /// Enumerator returning elements as ElementType
        /// </summary>
        /// <returns>Enumerator</returns>
        /// <remarks>This method enables the us of ILNumerics arrays in foreach loops.
        /// <para>This iterator implements IEnumerable&lt;ElementType> explicitely and is used in situations, 
        /// where instances of ILNumerics arrays are casted to instances of the IEnumerable interface. This iterator 
        /// is not integrated into the ILNumerics memory management.</para></remarks>
        /// <example><code>ILDenseStorage&lt;T&gt; A = ILMath.rand(5,4,6);
        /// foreach (double element in A) {
        /// // all elements are scalar double values
        /// String.Format("Element: {0} ",element);
        /// // Note: 'element' cannot be used to alter the collection! 
        /// } 
        /// </code></example> 
        IEnumerator<ElementType> System.Collections.Generic.IEnumerable<ElementType>.GetEnumerator() {
            return Storage.GetEnumerator();
        }
        /// <summary>
        /// Enumerator returning elements as ElementType
        /// </summary>
        /// <returns>Enumerator</returns>
        /// <remarks>This method enables the use of ILNumerics arrays in foreach loops directly.</remarks>
        IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator(); 
        }
        /// <summary>
        /// Enumerator returning elements as ElementType
        /// </summary>
        /// <returns>Enumerator</returns>
        /// <remarks>This method enables the use of ILNumerics arrays in foreach loops directly.</remarks>
        public abstract IEnumerator<ElementType> GetEnumerator(); 
        #endregion

    }
}