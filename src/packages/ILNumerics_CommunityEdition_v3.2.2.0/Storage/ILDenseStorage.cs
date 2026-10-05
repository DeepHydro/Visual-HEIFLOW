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
using System.Numerics; 
using ILNumerics.Misc;
using ILNumerics.Data;
using ILNumerics.Exceptions;
using System.Linq.Expressions; 

namespace ILNumerics.Storage {

    /// <summary>
    /// The class realizes an internal storage wrapper. It stores the internal data array
    /// and the dimension specifications for both: ILRetArray and ILDenseStorage (reference and solid). 
    /// </summary>
    [System.Diagnostics.DebuggerTypeProxy(typeof(ILNumerics.Misc.ILArrayDebuggerProxy<>))]
    [System.Diagnostics.DebuggerDisplay("{ShortInfo(),nq}")]
    [Serializable]
    internal partial class ILDenseStorage<ElementType> : ILStorage<ElementType> {

        #region attributes
        /// <summary> 
        /// Internal storage object. Contains the final System.Array storage and a reference counter.
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)] 
        protected ILCountableArray<ElementType> m_data;
        #endregion

        #region properties 

        /// <summary>
        /// number of storages referencing the current data array
        /// </summary>
        internal int ReferenceCount {
            get {
                return m_data.ReferenceCount; 
            }
        }

        /// <summary>
        /// Determine, if this storage has already been disposed.
        /// </summary>
        internal override bool IsDisposed {
            get {
                return m_data == null; 
            } 
        }
        protected ILCountableArray<ElementType> Data {
            get { 
                return m_data; 
            }
            set {
                ILCountableArray<ElementType> old = m_data;
                m_data = value; 
                m_data.IncreaseReference(); 
                if (old != null) 
                    old.DecreaseReference(); 
            }
        }
        /// <summary>
        /// Get minimum and maximum value of all elements - if these exist.
        /// </summary>
        /// <param name="minValue">Output: minimum value.</param>
        /// <param name="maxValue">Output: maximum value.</param>
        /// <returns>True if the limits exists and could be computed, false otherwise.</returns>
        /// <remarks>Empty arrays will return false. The output parameter will be default(type).</remarks>
        internal override bool GetLimits(out ElementType minValue, out ElementType maxValue) { 
            return GetLimits(out minValue, out maxValue, true); 
        }
        /// <summary>
        /// Get minimum and maximum value of all elements - if existing
        /// </summary>
        /// <param name="minValue">Output: minimum value.</param>
        /// <param name="maxValue">Output: maximum value.</param>
        /// <param name="includeInfNaNs">true: recognize Inf, NaN values; false: ignore those values</param>
        /// <returns>True if the limits exists and could be computed, false otherwise.</returns>
        /// <remarks>Empty arrays will return false. The output parameter will be default(ElementType) then.</remarks>
        internal override bool GetLimits(out ElementType minValue, out ElementType maxValue, bool includeInfNaNs) {
            minValue = default(ElementType);   
            maxValue = default(ElementType);
            if (m_size.NumberOfElements == 0)
                return false; 
            if (false) {

    
                } else if (this is ILDenseStorage<double> ) {    
                     double [] data = ( double [])(object) GetArrayForRead(); 
                     double curVal; 
                     double curMin =  Double.PositiveInfinity ;
                     double curMax =  Double.NegativeInfinity ;
                    int curMinInd = 0; 
                    int curMaxInd = 0; 
                    int len = m_size.NumberOfElements; 

                    for (int i = 0; i < len; i++) { 
                        curVal = data[i]; 
                        
                        if (double.IsInfinity(curVal)) continue; 
                        if (curVal < curMin) { 
                            curMin = curVal; 
                            curMinInd = i; 
                        }
                        if (curVal > curMax) { 
                            curMax = curVal; 
                            curMaxInd = i; 
                        }
                    }
                    maxValue = m_data.Data[curMaxInd];
                    minValue = m_data.Data[curMinInd]; 
                    return (curMax >  Double.NegativeInfinity || curMin <  Double.PositiveInfinity ); 

#region HYCALPER AUTO GENERATED CODE

    
                } else if (this is ILDenseStorage<fcomplex> ) {    
                    fcomplex [] data = ( fcomplex [])(object) GetArrayForRead(); 
                    fcomplex curVal; 
                    fcomplex curMin =  new fcomplex(float.PositiveInfinity,float.PositiveInfinity) ;
                    fcomplex curMax =  new fcomplex(float.NegativeInfinity,float.NegativeInfinity) ;
                    int curMinInd = 0; 
                    int curMaxInd = 0; 
                    int len = m_size.NumberOfElements; 

                    for (int i = 0; i < len; i++) { 
                        curVal = data[i]; 
                        if (fcomplex.IsInfinity(curVal)) continue;
                        if (curVal < curMin) { 
                            curMin = curVal; 
                            curMinInd = i; 
                        }
                        if (curVal > curMax) { 
                            curMax = curVal; 
                            curMaxInd = i; 
                        }
                    }
                    maxValue = m_data.Data[curMaxInd];
                    minValue = m_data.Data[curMinInd]; 
                    return (curMax >  new fcomplex(float.NegativeInfinity,float.NegativeInfinity) || curMin <  new fcomplex(float.NegativeInfinity,float.NegativeInfinity) ); 
    
                } else if (this is ILDenseStorage<complex> ) {    
                    complex [] data = ( complex [])(object) GetArrayForRead(); 
                    complex curVal; 
                    complex curMin =  new complex(Double.PositiveInfinity,Double.PositiveInfinity) ;
                    complex curMax =  new complex(Double.NegativeInfinity,Double.NegativeInfinity) ;
                    int curMinInd = 0; 
                    int curMaxInd = 0; 
                    int len = m_size.NumberOfElements; 

                    for (int i = 0; i < len; i++) { 
                        curVal = data[i]; 
                        if (complex.IsInfinity(curVal)) continue;
                        if (curVal < curMin) { 
                            curMin = curVal; 
                            curMinInd = i; 
                        }
                        if (curVal > curMax) { 
                            curMax = curVal; 
                            curMaxInd = i; 
                        }
                    }
                    maxValue = m_data.Data[curMaxInd];
                    minValue = m_data.Data[curMinInd]; 
                    return (curMax >  new complex(Double.NegativeInfinity,Double.NegativeInfinity) || curMin <  new complex(Double.NegativeInfinity,Double.NegativeInfinity) ); 
    
                } else if (this is ILDenseStorage<byte> ) {    
                    byte [] data = ( byte [])(object) GetArrayForRead(); 
                    byte curVal; 
                    byte curMin =  Byte.MaxValue ;
                    byte curMax =  Byte.MinValue ;
                    int curMinInd = 0; 
                    int curMaxInd = 0; 
                    int len = m_size.NumberOfElements; 

                    for (int i = 0; i < len; i++) { 
                        curVal = data[i]; 
                        
                        if (curVal < curMin) { 
                            curMin = curVal; 
                            curMinInd = i; 
                        }
                        if (curVal > curMax) { 
                            curMax = curVal; 
                            curMaxInd = i; 
                        }
                    }
                    maxValue = m_data.Data[curMaxInd];
                    minValue = m_data.Data[curMinInd]; 
                    return (curMax >  Byte.MinValue || curMin <  Byte.MinValue ); 
    
                } else if (this is ILDenseStorage<Int64> ) {    
                    Int64 [] data = ( Int64 [])(object) GetArrayForRead(); 
                    Int64 curVal; 
                    Int64 curMin =  Int64.MaxValue ;
                    Int64 curMax =  Int64.MinValue ;
                    int curMinInd = 0; 
                    int curMaxInd = 0; 
                    int len = m_size.NumberOfElements; 

                    for (int i = 0; i < len; i++) { 
                        curVal = data[i]; 
                        
                        if (curVal < curMin) { 
                            curMin = curVal; 
                            curMinInd = i; 
                        }
                        if (curVal > curMax) { 
                            curMax = curVal; 
                            curMaxInd = i; 
                        }
                    }
                    maxValue = m_data.Data[curMaxInd];
                    minValue = m_data.Data[curMinInd]; 
                    return (curMax >  Int64.MinValue || curMin <  Int64.MinValue ); 
    
                } else if (this is ILDenseStorage<Int32> ) {    
                    Int32 [] data = ( Int32 [])(object) GetArrayForRead(); 
                    Int32 curVal; 
                    Int32 curMin =  Int32.MaxValue ;
                    Int32 curMax =  Int32.MinValue ;
                    int curMinInd = 0; 
                    int curMaxInd = 0; 
                    int len = m_size.NumberOfElements; 

                    for (int i = 0; i < len; i++) { 
                        curVal = data[i]; 
                        
                        if (curVal < curMin) { 
                            curMin = curVal; 
                            curMinInd = i; 
                        }
                        if (curVal > curMax) { 
                            curMax = curVal; 
                            curMaxInd = i; 
                        }
                    }
                    maxValue = m_data.Data[curMaxInd];
                    minValue = m_data.Data[curMinInd]; 
                    return (curMax >  Int32.MinValue || curMin <  Int32.MinValue ); 
    
                } else if (this is ILDenseStorage<float> ) {    
                    float [] data = ( float [])(object) GetArrayForRead(); 
                    float curVal; 
                    float curMin =  float.PositiveInfinity ;
                    float curMax =  float.NegativeInfinity ;
                    int curMinInd = 0; 
                    int curMaxInd = 0; 
                    int len = m_size.NumberOfElements; 

                    for (int i = 0; i < len; i++) { 
                        curVal = data[i]; 
                        if(float.IsInfinity(curVal)) continue;
                        if (curVal < curMin) { 
                            curMin = curVal; 
                            curMinInd = i; 
                        }
                        if (curVal > curMax) { 
                            curMax = curVal; 
                            curMaxInd = i; 
                        }
                    }
                    maxValue = m_data.Data[curMaxInd];
                    minValue = m_data.Data[curMinInd]; 
                    return (curMax >  float.NegativeInfinity || curMin <  float.NegativeInfinity ); 

#endregion HYCALPER AUTO GENERATED CODE
           } else if ((typeof(ElementType) is IComparable<ElementType>)) {
                ElementType[] tmpArr = GetArrayForRead();
                for (int i = 0; i < m_size.NumberOfElements; i++) {
                    ElementType val = tmpArr[i];
                    if (((IComparable<ElementType>)val).CompareTo(minValue) < 0)
                        minValue = val;
                    if (((IComparable<ElementType>)val).CompareTo(minValue) > 0)
                        maxValue = val;
                }
                return true;
            }
            return false; 
        }
#endregion

        #region overriding object.ToString(), Equals()
        /// <summary>
        /// Write information about the ILDenseStorage to string.
        /// </summary>
        /// <returns>String containing general information about the current instance of 
        /// ILDenseStorage and the formatted elements' values.</returns>
        /// <remarks>If the number of elements exceeds a certain amount, the display will be abreviated.</remarks>
        public override string ToString() {
            return ValuesToString(0).ToString();
        }
        /// <summary>
        /// print formated values to string
        /// </summary>
        /// <param name="maxLength">Maximum number of characters per line. 0: no limit</param>
        /// <returns>StringBuilder object filled with formated values.</returns>
        internal override StringBuilder ValuesToString(int maxLength) {
            StringBuilder s = new StringBuilder();
            if (maxLength <= 0) maxLength = int.MaxValue;
            //if (m_dimensions.NumberOfElements > 100000) {
            //    s.Append(String.Format("({0})", m_dimensions.ToString()));
            //    return s;
            //}
            if (m_size.NumberOfElements < 1)
                return new StringBuilder("(Empty)"); 
            int[] acc = new int[m_size.NumberOfDimensions];
            int d;
            String sElement;
            int elemLength = 10; 
            //string format = "{0,20: E13;-E13; 0.0         } "; 
            int curLineLength = 0, curRowsCount = 0;
            int maxRows = (maxLength == int.MaxValue) ? int.MaxValue : 500; // limit data rows in debugger view 
            if (false) {

                
            } else if (this.Data.Data.GetType() == typeof(  double [] )) {
                #region 
                 double element;
                 double [] elements = (this.Data.Data as  double []);
                elemLength =  10 ; 
                 
                double scaling = getScalingForPrint(); 
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                
                                sElement = String.Format ((scaling == 1 && (int)element == element) ? "{0} " : "{0:f5} ", element / scaling).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion

#region HYCALPER AUTO GENERATED CODE

               
            } else if (this.Data.Data.GetType() == typeof(  float [] )) {
                #region 
                float element;
                float [] elements = (this.Data.Data as  float []);
                elemLength =  5 ; 
                float scaling = (float)getScalingForPrint();
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0:f5} ", element / scaling).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  fcomplex [] )) {
                #region 
                fcomplex element;
                fcomplex [] elements = (this.Data.Data as  fcomplex []);
                elemLength =  24 ; 
                float scaling = (float)getScalingForPrint();
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0} ", (element / scaling).ToString(5)).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  complex [] )) {
                #region 
                complex element;
                complex [] elements = (this.Data.Data as  complex []);
                elemLength =  36 ; 
                double scaling = getScalingForPrint();
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0} ", (element / scaling).ToString(5)).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  Complex [] )) {
                #region 
                Complex element;
                Complex [] elements = (this.Data.Data as  Complex []);
                elemLength =  36 ; 
                double scaling = getScalingForPrint();
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0} ", ComplexHelper(element / scaling,5)).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  byte [] )) {
                #region 
                byte element;
                byte [] elements = (this.Data.Data as  byte []);
                elemLength =  3 ; 
                double scaling = 1.0; 
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0,3:G} ", element).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  char [] )) {
                #region 
                char element;
                char [] elements = (this.Data.Data as  char []);
                elemLength =  3 ; 
                double scaling = 1.0; 
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0,3:G} ", element).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  UInt64 [] )) {
                #region 
                UInt64 element;
                UInt64 [] elements = (this.Data.Data as  UInt64 []);
                elemLength =  18 ; 
                double scaling = 1.0; 
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0,18:G} ", element).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  UInt32 [] )) {
                #region 
                UInt32 element;
                UInt32 [] elements = (this.Data.Data as  UInt32 []);
                elemLength =  10 ; 
                double scaling = 1.0; 
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0,10:G} ", element).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  UInt16 [] )) {
                #region 
                UInt16 element;
                UInt16 [] elements = (this.Data.Data as  UInt16 []);
                elemLength =  5 ; 
                double scaling = 1.0; 
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0,5:G} ", element).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  Int64 [] )) {
                #region 
                Int64 element;
                Int64 [] elements = (this.Data.Data as  Int64 []);
                elemLength =  18 ; 
                double scaling = 1.0; 
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0,18:G} ", element).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  Int32 [] )) {
                #region 
                Int32 element;
                Int32 [] elements = (this.Data.Data as  Int32 []);
                elemLength =  10 ; 
                double scaling = 1.0; 
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0,10:G} ", element).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  Int16 [] )) {
                #region 
                Int16 element;
                Int16 [] elements = (this.Data.Data as  Int16 []);
                elemLength =  5 ; 
                double scaling = 1.0; 
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0,5:G} ", element).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  ILStorage [] )) {
                #region 
                ILStorage element;
                ILStorage [] elements = (this.Data.Data as  ILStorage []);
                elemLength =  0 ; 
                double scaling = 1.0; 
                for (int i = 0; i < m_size.NumberOfElements; i++) { 
                    element = elements[i]; 
                    int l = (element == null)? 6 : element.ShortInfo().Length; 
                    if (l > elemLength) elemLength = l; 
                }

                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = element.ShortInfo().PadRight(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
               
            } else if (this.Data.Data.GetType() == typeof(  object [] )) {
                #region 
                object element;
                object [] elements = (this.Data.Data as  object []);
                elemLength =  18 ; 
                double scaling = 1.0; 
                elemLength++; 
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2 || scaling != 1) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") " + ((scaling!=1.0)? (scaling.ToString("e0") + " * "):""));
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");    
                            return s; 
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0; 
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements [m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element,null)) {
                                sElement = String.Format ("{0} ", element).PadLeft(elemLength);
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;  
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion

#endregion HYCALPER AUTO GENERATED CODE
           } else if (this is ILDenseStorage<string> ) {
                #region
                string element;
                string[] elements = (this.Data.Data as string[]);
                elemLength = 18;
                int maxElemLength = 50;  // todo: may better be a setting? abreviate after as many chars
                elemLength++;
                while (acc[m_size.NumberOfDimensions - 1] <
                        m_size[m_size.NumberOfDimensions - 1]) {
                    // show only two first dimensions at the same time ... 
                    // print header
                    if (m_size.NumberOfDimensions > 2) {
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        s.Append("(:,:");
                        for (d = 2; d < m_size.NumberOfDimensions; d++)
                            s.Append(String.Format(",{0}", acc[d]));
                        s.Append(") ");
                    }
                    // show this for 2 leading dimensions
                    for (int d0 = 0; d0 < m_size[0]; d0++) {
                        if (++curRowsCount > maxRows) {
                            s.Append(Environment.NewLine + "( ... more rows exist)");
                            return s;
                        }
                        if (s.Length > 0) s.Append(Environment.NewLine);
                        acc[0] = d0; curLineLength = 0;
                        for (int d1 = 0; d1 < m_size[1]; d1++) {
                            acc[1] = d1;
                            element = elements[m_size.IndexFromArray(acc)];
                            if (!Object.ReferenceEquals(element, null)) {
                                if (element.Length > maxElemLength) {
                                    sElement = element.Substring(0,maxElemLength - 3) + "..."; 
                                } else {
                                    sElement = String.Format("{0} ", element).PadLeft(elemLength);
                                }
                            } else {
                                sElement = "(null)".PadLeft(elemLength);
                            }
                            curLineLength += sElement.Length;
                            if (curLineLength > maxLength - " ...".Length) {
                                s.Append(" ...");
                                break;
                            } else {
                                s.Append(sElement);
                            }
                        }
                    }
                    // increase higher dimension
                    d = 2;
                    while (d <= m_size.NumberOfDimensions - 1) {
                        acc[d]++;
                        if (acc[d] < m_size[d]) break;
                        acc[d] = 0;
                        d++;
                    }
                    if (d >= m_size.NumberOfDimensions) break;
                }
                #endregion
            } else { 
                return new StringBuilder ("(Unknown data type.)"); 
           } 
            return s;
        }
        private double getScalingForPrint() {
            ElementType min, max; 
            if (GetLimits(out min, out max, false)) {
                try {
                    double scaling = Math.Max(
                                        Math.Abs(double.Parse(min.ToString())),
                                        Math.Abs( double.Parse(max.ToString())) 
                                     ); 
                    if (scaling == 0  || (scaling > 1e-1 && scaling < 1e1)) 
                        scaling = 1; 
                    scaling = Math.Pow(10,Math.Floor(Math.Log10(scaling)));
                    if (scaling < 10000 && scaling >= 1) 
                        scaling = 1; 
                    return scaling; 
                } catch (Exception) {
                    return 1.0;
                }
            } else {
                return 1.0; 
            }
        }
        private int GetTypedElementStringProperties(double scaling, out string format) {
            format = "E+00000000000000;0.0;-E+00000000000000";
            if (this is ILDenseStorage<double>) {
                return double.MaxValue.ToString().Length;
            } else if (this is ILDenseStorage<float>) {
                return float.MaxValue.ToString().Length;
            } else if (this is ILDenseStorage<Complex>) {
                return double.MaxValue.ToString().Length * 2 + 2;
            } else if (this is ILDenseStorage<complex>) {
                return double.MaxValue.ToString().Length * 2 + 2;
            } else if (this is ILDenseStorage<fcomplex>) {
                return float.MaxValue.ToString().Length * 2 + 2;
            } else if (this is ILDenseStorage<char>) {
                return char.MaxValue.ToString().Length;
            } else if (this is ILDenseStorage<byte>) {
                return byte.MaxValue.ToString().Length;
            } else if (this is ILDenseStorage<Int16>) {
                return Int16.MaxValue.ToString().Length;
            } else if (this is ILDenseStorage<Int32>) {
                return Int32.MaxValue.ToString().Length;
            } else if (this is ILDenseStorage<Int64>) {
                return Int64.MaxValue.ToString().Length;
            } else if (this is ILDenseStorage<UInt16>) {
                return UInt16.MaxValue.ToString().Length;
            } else if (this is ILDenseStorage<UInt32>) {
                return UInt32.MaxValue.ToString().Length;
            } else if (this is ILDenseStorage<UInt64>) {
                return UInt64.MaxValue.ToString().Length;
            } else
                return 10; 
        }
        private string ComplexHelper(Complex a, int digits) {
            if (digits < 1) return ""; 
            if (a.Imaginary >= 0) {
                return String.Format("{0:f10}+{1:f10}", a.Real, a.Imaginary); 
            } else {
                return String.Format("{0:f10}-{1:f10}", a.Real, -a.Imaginary); 
            }
        }
        /// <summary>
        /// Check if the content of this array equals the content of obj.
        /// </summary>
        /// <param name="obj">storage containing the values with which to compare this array.</param>
        /// <returns>True if all elements contained in obj are equal to the 
        /// elements of this array, false otherwise.</returns>
        /// <remarks>This method compares the object references of corresponding elements. 
        /// The size and type of both arrays must match. Otherwise false will be returned.</remarks>
        internal override bool Equals(object obj) {
            if (object.Equals(obj,null)) return false; 
            if (obj is ElementType)
                if (Size.NumberOfElements == 1
                    && obj.ToString() == GetValueTyped(0).ToString())
                    return true;
                else 
                    return false; 
            ILDenseStorage<ElementType> denseStorage = obj as ILDenseStorage<ElementType>;
            if (object.Equals(denseStorage,null))
                return false;
            return Equals(denseStorage);
        }
        /// <summary>
        /// Test if this dense storage equals another dense storage.
        /// </summary>
        /// <param name="A">storage to compare this storage with</param>
        /// <returns>True if all elements and dimension sizes match, false otherwise.</returns>
        internal bool Equals(ILDenseStorage<ElementType> A) {
            if (A == null) return false;
            if (!A.Size.IsSameSize(m_size))
                return false;
            int len = m_size.NumberOfElements; 
            if (typeof(ElementType).IsValueType) {
                for (int i = 0; i < len;i++) {
                    if (!GetValueTyped(i).Equals(A.GetValueTyped(i)))
                        return false; 
                }
            } else {
                // references may contain null element values! 
                for (int i = 0; i < len;i++) {
                    ElementType t1 = GetValueTyped(i); 
                    if (t1 == null) {
                        if (!A.GetValueTyped(i).Equals(null))
                            return false; 
                    } else {
                        if (!t1.Equals(A.GetValueTyped(i))) 
                            return false; 
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// generate a hash code based on the current arrays values
        /// </summary>
        /// <returns>hash code</returns>
        /// <remarks>The hashcode is generated by taking the values currently stored in the array into account.
        /// Therefore, the function must iterate over all elements in the array - which makes it somehow a expensive 
        /// operation. Take this into account, if you consider using large arrays in collections like dictionaries 
        /// or hashtables, which make great use of hash codes.</remarks>
        public override int GetHashCode() {
            if (IsDisposed) return base.GetHashCode();
            int ret = Size.GetHashCode();
            ElementType[] data = GetArrayForRead();
            ret = (ret * 17) + data.Length;
            foreach (ElementType t in data) {
                ret = unchecked( ret * 17 );
                if (t != null) 
                    ret = unchecked(ret + t.GetHashCode());
            }
            return (int)ret;
        }
        #endregion

        #region subarray + range get / set 
        /// <summary>
        ///	Alter values specified by range.
        /// </summary>
        /// <param name="range">ILRange specifying the dimensions/indices to be altered.</param>
        /// <param name="values">new values</param>
        /// <remarks>
        /// The values pointed to by range will be replaced with the values 
        /// found in 'values'. Important: the range cannot specify indices outside of my dimensions! 
        /// Therefore, the storage must have been expanded in advance, if needed!</remarks>
        public virtual void SetRange(ILRange range, ILDenseStorage<ElementType> values) {
            if (range.Size.NumberOfElements == 0) return; 
            int rangeDimLen = range.RangeArray.Length,higherDimSum = 0;
            int leadDimLenRange = range[0].Count, leadDimLen = m_size[0]; 
            int d, outElemCount = range.Size.NumberOfElements; 
            ElementType[] myArr = GetArrayForWrite(); 
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
                ElementType scalar = values.GetValueTyped(0); 
                while (true) {
                    // copy along leading dimension
                    for (int i = 0; i < rng[0].Count; i++) {
                        if (rng[0][i] < 0) {
                            for (int c = -rng[0][i]; c-- >= 0; ) {
                                // we need a upward counter variable anyway, initialized with 
                                // higherDimSum. So we simply take that and reset it after the loop: 
                                myArr[ higherDimSum++ ] = scalar;  
                            }
                            higherDimSum += (rng[0][i]-1); // negative value in rng!  
                        } else {
                             myArr[ higherDimSum + rng[0][i] ] = scalar;
                        }

                    }
                
                    // increase higher dims 
                    d = 1; 
                    while  (d < idxArr.Length) {
                        if (inFullDim[d] < 0)  {
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
                            if (rng[d][0] < 0) {
                                inFullDim[d] = rng[d][idxArr[d]];
                            } else {
                                higherDimSum += (rng[d][0] * seqDistances[d]);
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
            } else {
#region non scalar case 
                ElementType [] valuesArr = values.GetArrayForRead(); 
                int valuesPos = 0; 
                while (true) {
                    // copy along leading dimension
                    for (int i = 0; i < rng[0].Count; i++) {
                        if (rng[0][i] < 0) {
                            for (int c = -rng[0][i]; c-- >= 0; ) {
                                myArr[ higherDimSum++ ] = valuesArr[ valuesPos++ ];  
                            }
                            higherDimSum += (rng[0][i]-1); // negative value in rng!  
                        } else {
                            myArr[ higherDimSum + rng[0][i] ] = valuesArr[ valuesPos++ ];
                        }
                    }
                
                    // increase higher dims 
                    d = 1; 
                    while  (d < idxArr.Length) {
                        if (inFullDim[d] < 0)  {
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
                            } else {
                                higherDimSum += (rng[d][0] * seqDistances[d]);
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

        internal virtual void SetRangeFull(ILDenseStorage<ElementType> values) {
            if (values.Size.NumberOfElements != 1 && values.Size.NumberOfElements != Size.NumberOfElements) 
                throw new ILArgumentException("source and destination array must have the same number of arguments"); 
            ElementType[] myArr = GetArrayForWrite(); 
            if (values.Size.NumberOfElements == 1) {
                ElementType scal = values.GetValueTyped(0); 
                for (int i = 0; i < myArr.Length; i++) {
                    myArr[i] = scal; 
                }
            } else {
                ElementType[] valArr = values.GetArrayForRead(); 
                System.Array.Copy(valArr,myArr,Size.NumberOfElements); 
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
        public virtual void SetRange(ILBaseArray<double> indices, ILDenseStorage<ElementType> values) {
            //if (object.Equals(indices,null))
            //    throw new ILArgumentException("indices given must not be null");
            if (object.Equals(values, null))
                throw new ILArgumentException("values given must not be null. Use A[ind] = null; if removal was intended.");
            using (ILScope.Enter(indices)) {
                #region set full shortcut
                if (indices is ILFullRange || object.Equals(indices, null)) {
                    int pos;
                    ElementType[] myArray = GetArrayForWrite();
                    if (values.Size.NumberOfElements == 1) {
                        pos = Size.NumberOfElements;
                        ElementType val = values.GetValueTyped(0);
                        while (pos-- > 0)
                            myArray[pos] = val;
                    } else {
                        if (values.Size.NumberOfElements != Size.NumberOfElements)
                            throw new ILArgumentException("number of elements in source must match number of elements in destination array");
                        pos = 0;
                        foreach (ElementType val in values) {
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
                    ElementType[] myArray = GetArrayForWrite();
                    
                    double[] indArray = indStorage.GetArrayForRead();

                    int len = indices.Size.NumberOfElements;
                    if (values.Size.NumberOfElements == 1) {
                        ElementType scalarElement = values.GetValueTyped(0);
                        for (int i = 0; i < len; ) {
                            myArray[(int)indArray[i++]] = scalarElement;
                        }
                    } else {
                        ElementType[] valArray = values.GetArrayForRead();
                        for (int i = 0; i < len; ) {
                            myArray[(int)indArray[i]] = valArray[i++];
                        }
                    }
                } else {
                    throw new ILArgumentException("storage type of given indices is not supported");
                }
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
        public virtual void SetRange(ILBaseArray<Int64> indices, ILDenseStorage<ElementType> values) {
            //if (object.Equals(indices,null))
            //    throw new ILArgumentException("indices given must not be null");
            if (object.Equals(values, null))
                throw new ILArgumentException("values given must not be null. Use A[ind] = null; if removal was intended.");
            using (ILScope.Enter(indices)) {
                #region set full shortcut
                if (indices is ILFullRange || object.Equals(indices, null)) {
                    int pos;
                    ElementType[] myArray = GetArrayForWrite();
                    if (values.Size.NumberOfElements == 1) {
                        pos = Size.NumberOfElements;
                        ElementType val = values.GetValueTyped(0);
                        while (pos-- > 0)
                            myArray[pos] = val;
                    } else {
                        if (values.Size.NumberOfElements != Size.NumberOfElements)
                            throw new ILArgumentException("number of elements in source must match number of elements in destination array");
                        pos = 0;
                        foreach (ElementType val in values) {
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
                    ElementType[] myArray = GetArrayForWrite();
                   
                    Int64[] indArray = indStorage.GetArrayForRead();

                    int len = indices.Size.NumberOfElements;
                    if (values.Size.NumberOfElements == 1) {
                        ElementType scalarElement = values.GetValueTyped(0);
                        for (int i = 0; i < len; ) {
                            myArray[(int)indArray[i++]] = scalarElement;
                        }
                    } else {
                        ElementType[] valArray = values.GetArrayForRead();
                        for (int i = 0; i < len; ) {
                            myArray[(int)indArray[i]] = valArray[i++];
                        }
                    }
                } else {
                    throw new ILArgumentException("storage type of given indices is not supported");
                }
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
        public virtual void SetRange(ILBaseArray<Int32> indices, ILDenseStorage<ElementType> values) {
            //if (object.Equals(indices,null))
            //    throw new ILArgumentException("indices given must not be null");
            if (object.Equals(values, null))
                throw new ILArgumentException("values given must not be null. Use A[ind] = null; if removal was intended.");
            using (ILScope.Enter(indices)) {
                #region set full shortcut
                if (indices is ILFullRange || object.Equals(indices, null)) {
                    int pos;
                    ElementType[] myArray = GetArrayForWrite();
                    if (values.Size.NumberOfElements == 1) {
                        pos = Size.NumberOfElements;
                        ElementType val = values.GetValueTyped(0);
                        while (pos-- > 0)
                            myArray[pos] = val;
                    } else {
                        if (values.Size.NumberOfElements != Size.NumberOfElements)
                            throw new ILArgumentException("number of elements in source must match number of elements in destination array");
                        pos = 0;
                        foreach (ElementType val in values) {
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
                    ElementType[] myArray = GetArrayForWrite();
                   
                    Int32[] indArray = indStorage.GetArrayForRead();

                    int len = indices.Size.NumberOfElements;
                    if (values.Size.NumberOfElements == 1) {
                        ElementType scalarElement = values.GetValueTyped(0);
                        for (int i = 0; i < len; ) {
                            myArray[(int)indArray[i++]] = scalarElement;
                        }
                    } else {
                        ElementType[] valArray = values.GetArrayForRead();
                        for (int i = 0; i < len; ) {
                            myArray[(int)indArray[i]] = valArray[i++];
                        }
                    }
                } else {
                    throw new ILArgumentException("storage type of given indices is not supported");
                }
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
        public virtual void SetRange(ILBaseArray<Int16> indices, ILDenseStorage<ElementType> values) {
            //if (object.Equals(indices,null))
            //    throw new ILArgumentException("indices given must not be null");
            if (object.Equals(values, null))
                throw new ILArgumentException("values given must not be null. Use A[ind] = null; if removal was intended.");
            using (ILScope.Enter(indices)) {
                #region set full shortcut
                if (indices is ILFullRange || object.Equals(indices, null)) {
                    int pos;
                    ElementType[] myArray = GetArrayForWrite();
                    if (values.Size.NumberOfElements == 1) {
                        pos = Size.NumberOfElements;
                        ElementType val = values.GetValueTyped(0);
                        while (pos-- > 0)
                            myArray[pos] = val;
                    } else {
                        if (values.Size.NumberOfElements != Size.NumberOfElements)
                            throw new ILArgumentException("number of elements in source must match number of elements in destination array");
                        pos = 0;
                        foreach (ElementType val in values) {
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
                    ElementType[] myArray = GetArrayForWrite();
                   
                    Int16[] indArray = indStorage.GetArrayForRead();

                    int len = indices.Size.NumberOfElements;
                    if (values.Size.NumberOfElements == 1) {
                        ElementType scalarElement = values.GetValueTyped(0);
                        for (int i = 0; i < len; ) {
                            myArray[(int)indArray[i++]] = scalarElement;
                        }
                    } else {
                        ElementType[] valArray = values.GetArrayForRead();
                        for (int i = 0; i < len; ) {
                            myArray[(int)indArray[i]] = valArray[i++];
                        }
                    }
                } else {
                    throw new ILArgumentException("storage type of given indices is not supported");
                }
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
        public virtual void SetRange(ILBaseArray<float> indices, ILDenseStorage<ElementType> values) {
            //if (object.Equals(indices,null))
            //    throw new ILArgumentException("indices given must not be null");
            if (object.Equals(values, null))
                throw new ILArgumentException("values given must not be null. Use A[ind] = null; if removal was intended.");
            using (ILScope.Enter(indices)) {
                #region set full shortcut
                if (indices is ILFullRange || object.Equals(indices, null)) {
                    int pos;
                    ElementType[] myArray = GetArrayForWrite();
                    if (values.Size.NumberOfElements == 1) {
                        pos = Size.NumberOfElements;
                        ElementType val = values.GetValueTyped(0);
                        while (pos-- > 0)
                            myArray[pos] = val;
                    } else {
                        if (values.Size.NumberOfElements != Size.NumberOfElements)
                            throw new ILArgumentException("number of elements in source must match number of elements in destination array");
                        pos = 0;
                        foreach (ElementType val in values) {
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
                    ElementType[] myArray = GetArrayForWrite();
                   
                    float[] indArray = indStorage.GetArrayForRead();

                    int len = indices.Size.NumberOfElements;
                    if (values.Size.NumberOfElements == 1) {
                        ElementType scalarElement = values.GetValueTyped(0);
                        for (int i = 0; i < len; ) {
                            myArray[(int)indArray[i++]] = scalarElement;
                        }
                    } else {
                        ElementType[] valArray = values.GetArrayForRead();
                        for (int i = 0; i < len; ) {
                            myArray[(int)indArray[i]] = valArray[i++];
                        }
                    }
                } else {
                    throw new ILArgumentException("storage type of given indices is not supported");
                }
            }
        }  

#endregion HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Create referencing or solid array from this array, with shifted dimensions.
        /// </summary>
        /// <param name="shift">Number of dimensions to shift the array.</param>
        /// <returns>Shifted ILDenseStorage of the same type.</returns>
        /// <remarks><para>Shift is done 'to the left'.</para></remarks>
		public virtual ILDenseStorage<ElementType> ShiftDimensions (int shift) {
            return CreateShiftedStorage(shift);
        }

        #endregion

        #region Reshape, Concat, Repmat  - overriding ILBaseArray<ElementType>

        /// <summary>
        /// reshape <b>this</b> storage
        /// </summary>
        /// <param name="newDimensions">new dimensions of the storage.</param>
        /// <remarks><para>This storage will be changed! The operation is cheap, since the 
        /// number of elements (and their values) do not change. The same countable array
        /// is used in conjunction with a new dimension specifier. </para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the number of elements in 'newDimension'
        /// do not match the number of elements in this storage.</exception>
        internal void Reshape(ILSize newDimensions) {
            if (newDimensions.NumberOfElements != m_size.NumberOfElements)
                throw new ILArgumentSizeException ("the number of elements must not change");
            m_size = newDimensions; 
        }
        /// <summary>
        /// reshape <b>this</b> storage
        /// </summary>
        /// <param name="dims">new dimension length of the storage.</param>
        /// <remarks><para>This storage will be changed! The operation is cheap, since the 
        /// number of elements (and their values) do not change. The same underlying storage 
        /// is used in conjunction with the new dimension specifier.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the number of elements in 'newDimension'
        /// do not match the number of elements in this storage.</exception>
        public void Reshape(params int[] dims) {
            ILSize newDimension = new ILSize(dims); 
            Reshape(newDimension); 
        }
        /// <summary>
        /// concatenate this storage 
        /// </summary>
        /// <param name="A">n dimensional storage</param>
        /// <param name="dim">Index of dimension along which to concatenate the arrays.
        /// If dim is larger than the number of dimensions of one of the arrays
        /// its value will be used in modulus.</param>
        /// <returns>Array having the size of both input arrays laid beside one 
        /// another along the <paramref name="dim"/>'s-dimension</returns>
        /// <remarks>The array returned will be a copy of both arrays involved. None 
        /// of the input arrays will be altered.</remarks>
        public virtual ILDenseStorage<ElementType> Concat(ILDenseStorage<ElementType> A, int dim) {
            ILSize inDim = A.m_size;
            for (int i = 0; i < Math.Max(inDim.NumberOfDimensions, m_size.NumberOfDimensions); i++) {
                if (i != dim)
                    if (m_size[i] != inDim[i])
                        throw new ILArgumentSizeException("the length of dimensions of both arrays (except the dimension "
                                            + "to be concatenated) must match");
            }
            // concatenate 
            if (inDim.NumberOfElements == 1 && m_size.NumberOfElements == 1) {
                // both scalars
                int[] dims = new int[2] { 1, 1 };
                dims[dim] = 2;
                ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(2);
                retArr[0] = m_data.Data[0];
                retArr[1] = A.m_data.Data[0];
                return CreateSelf(retArr, new ILSize(dims)); 
            }
            int lenOutArr = m_size.NumberOfElements + inDim.NumberOfElements;
            int[] retDims = m_size.ToIntArray(dim+1);
            retDims[dim] += inDim[dim];
            ILSize retDimension = new ILSize(retDims);
            ElementType[] retData = ILMemoryPool.Pool.New<ElementType>(retDimension.NumberOfElements);
            ElementType[] myData = GetArrayForRead();
            ElementType[] inData = A.GetArrayForRead(); 

            int len1 = m_size.SequentialIndexDistance(dim + 1);
            int len2 = inDim.SequentialIndexDistance(dim + 1);
            int posOutArr = 0, inPos = 0, myPos = 0;
            while (posOutArr < lenOutArr) {
                for (int i = 0; i < len1; i++)
                    retData[posOutArr++] = myData[myPos++]; 
                for (int i = 0; i < len2; i++)
                    retData[posOutArr++] = inData[inPos++]; 
            }
            return CreateSelf(retData,retDimension);
        }

        /// <summary>
        /// Replicate this storage to create a larger array.
        /// </summary>
        /// <param name="sizes">Sizes description. This may be a 
        /// list or an array of integer values. If the number of elements in <paramref name="sizes"/> is 
        /// less the number of dimensions in this array, the trailing dimensions will 
        /// be set to 1 (singleton dimensions). On the other hand, if the number specified 
        /// is larger then the number of dimensions of this array, the result 
        /// will have its number of dimensions extended accordingly. </param>
        /// <returns>array which is made out of multiple copies of this array along 
        /// specified dimensions, according to <paramref name="sizes"/>.</returns>
        internal virtual ILDenseStorage<ElementType> Repmat(params int[] sizes) {
            if (m_size.NumberOfElements == 0)
                return CreateSelf(new ILSize(sizes));
            ILRightSideRange rsRange = new ILRightSideRange(m_size, sizes);
            return CreateSubarrayStorage(rsRange); 

            
            // experimental: via subindexing full dimensions 
            //ILBaseArray[] dimArrays = new ILBaseArray[dims.Length];
            //for (int i = 0; i < dimArrays.Length; i++) {
            //    dimArrays[i] = ILMath.cell(ILMath.full)[ILMath.zeros(1,dims[i])]; 
            //}
            //return Subarray(dimArrays); 
            
            //build return dimensions
            //int[] newDim = new int[Math.Max(dims.Length, m_dimensions.NumberOfDimensions)];
            //for (int d = 0; d < newDim.Length; d++) {
            //    if (d < dims.Length) {
            //        newDim[d] = m_dimensions[d] * dims[d];
            //    } else {
            //        newDim[d] = m_dimensions[d];
            //    }
            //}
            //ILSize outDim = new ILSize(true, newDim);
            //if (m_dimensions.NumberOfElements == 0 || outDim.NumberOfElements == 0)
            //    return new ILDenseStorage<ElementType>(new ILCountableArray<ElementType>(0), outDim);
            //ILDenseStorage<ElementType> ret = new ILDenseStorage<ElementType>(
            //                    new ILCountableArray<ElementType>(outDim.NumberOfElements), outDim);
            //ElementType[] retArray = ret.GetArrayForWrite();
            //ElementType[] myArray = GetArrayForRead();
            //int[] outStrides = outDim.GetSequentialIndexDistances(m_dimensions.NumberOfDimensions); 
            //int[] myStrides = m_dimensions.GetSequentialIndexDistances(m_dimensions.NumberOfDimensions); 
            //retArray[0] = myArray[0];
            //if (outDim.NumberOfElements < ILNumerics.Settings.s_minParallelElement1Count) {
            //    for (int outPos = 1; outPos < outDim.NumberOfElements; ) {
            //        int inPos = 0;
            //        for (int d = m_dimensions.NumberOfDimensions; d-- > 0; ) {
            //            inPos += ((outPos / outStrides[d]) % m_dimensions[d])
            //                     * myStrides[d];
            //        }
            //        retArray[outPos++] = myArray[inPos];
            //    }
            //} else {
            //    System.Threading.Tasks.Parallel.For(1,outDim.NumberOfElements, i => {
            //        int inPos = 0;
            //        for (int d = m_dimensions.NumberOfDimensions; d-- > 0; ) {
            //            inPos += ((i / outStrides[d]) % m_dimensions[d])
            //                     * myStrides[d];
            //        }
            //        retArray[i] = myArray[inPos];
            //    }); 
            //}
            //return ret;
        }

        /// <summary>
        /// Remove individual parts of a dimension from <b>this</b> storage
        /// </summary>
        /// <param name="dimension">index of the dimension, where <c>indices</c> are to be removed</param>
        /// <param name="indices">indices to be removed from <paramref name="dimension"/>, -1 for "wipe" (make this an empty storage)</param>
        /// <remarks>The function directly operates on <b>this</b> storage! After the function returns, 
        /// this storage may have its dimensions changed!</remarks>
        internal virtual void Remove(int dimension, ILIntList indices) {
            using (ILScope.Enter()) {
                if (dimension >= Size.NumberOfDimensions)
                    throw new ILArgumentException("index out of range");
                if (dimension == -1) {
                    Data = new ILCountableArray<ElementType>(0);
                    m_size = ILSize.Empty00;
                    return;
                }
                // TODO: may should be replaced with a faster (&fancier? ) version... ??
                ILBaseArray[] dims = new ILBaseArray[Size.NumberOfDimensions];
                ILIntList keepInd = ILIntList.Create();
                ILIntList remvInd = indices;
                int max = Size[dimension];
                foreach (int i in indices)
                    if (i >= max || i < 0) throw new ILArgumentException("removal index out of range");
                for (int i = 0; i < max; i++) {
                    if (!remvInd.Contains(i)) {
                        keepInd.Add(i);
                    }
                }
                // we give the indices to _keep_ to the subarray function
                int keepIndCount = keepInd.Count;
                ILArray<int> remIndices = ILMath.array<int>(keepInd.GetArray(), new ILSize(1, keepIndCount));
                if (remIndices.Size.NumberOfElements == Size[dimension]) {
                    // nothing to do
                    return;
                }
                for (int i = 0; i < dims.Length; i++) {
                    dims[i] = ILMath.full;
                }
                dims[dimension] = remIndices;
                ILDenseStorage<ElementType> ret = Subarray(dims);
                Dispose();  // this will for reference element types (e.g. ILCell) also dispose the removed elements 
                
                Data = ret.Data;
                m_size = ret.Size;
                
            }
        }

        #endregion

        #region serialize 
        /// <summary>
        /// Prepare for serialization.
        /// </summary>
        /// <param name="context">Streaming Context - provided by the formatter.</param>
        /// <remarks>nothing to do here</remarks>
        [OnSerializing]
        private void OnSerialize(StreamingContext context) {
        }

        /// <summary>
        /// Post operations aftre deserializing is finished.
        /// </summary>
        /// <param name="context">Streaming context provided by the formatter.</param>
        /// <remarks>nothing to do here</remarks>
        [OnDeserialized]
        void OnDeserialized(StreamingContext context) {
        }

        #endregion 

        #region single element access 

        /// <summary>
        /// Get single value from this storage.
        /// </summary>
        /// <param name="idx">Integer array holding the dimension specifier</param>
        /// <returns>Element at the position pointed to by idx.</returns>
        internal override object GetValue(params int[] idx) {
            return GetValueTyped(idx); 
        }
        /// <summary>
        /// Get single value from this storage.
        /// </summary>
        /// <param name="idx">Integer array holding the dimension specifier</param>
        /// <returns>Element at the position pointed to by idx.</returns>
        internal override ElementType GetValueTyped(params int[] idx) {
            if (idx.Length == 1)
                return m_data.Data[idx[0]]; 
            int destIdx = idx[0], d, highDims; 
            if (destIdx >= m_size[0] || destIdx < 0)
                    throw new ILArgumentException("GetValue: index out of bound for dimensions: 0");
            int [] seqDist = m_size.GetSequentialIndexDistances(0); 
            if (idx.Length <= m_size.NumberOfDimensions) {
                for (d = 1; d < idx.Length - 1; d++) {
                    if (idx[d] >= m_size[d] || idx[d] < 0)
                        throw new ILArgumentException("GetValue: index out of bound for dimensions: " + d.ToString());
                    destIdx += idx[d] * seqDist[d]; 
                }
                for (highDims = idx[d]; d<m_size.NumberOfDimensions && highDims > 0; d++) {
                    destIdx += (highDims % m_size[d]) * seqDist[d]; 
                    highDims /= m_size[d]; 
                }
                if (highDims > 0) throw new ILArgumentException ("GetValue: index out of bound!"); 
                return m_data.Data[destIdx];
            } else {
                highDims = m_size.NumberOfDimensions; 
                for (d = 1; d < highDims; d++) {
                    if (idx[d] >= m_size[d] || idx[d] < 0)
                        throw new ILArgumentException("GetValue: index out of bound for dimensions: " + d.ToString());
                    destIdx += idx[d] * seqDist[d]; 
                }
                for (; d<idx.Length; d++) {
                    if (idx[d] != 0) 
                        throw new ILArgumentException("GetValue: index out of bound for dimension: " + d.ToString() + ". Trailing indices must be 0!");
                }
                return m_data.Data[destIdx];
            }
        }
        internal override ILStorage GetValueAsStorage(params int[] innerIndices) {
            return new ILDenseStorage<ElementType>(new ElementType[1] { GetValueTyped(innerIndices) }, ILSize.Scalar1_1);  
        }
        internal override ILBaseArray GetAsBaseArray() {
            return new ILRetArray<ElementType>((ILDenseStorage<ElementType>)Clone());
        }

        /// <summary>
        /// Get single value from this storage by a single sequential access.
        /// </summary>
        /// <param name="idx">Integer array holding the dimension specifier 
        /// pointing to the value.</param>
        /// <param name="dims">Out value: return position mapped to dimensions.</param>
        /// <returns>Object in the position pointed to by idx.</returns>
        /// <remarks>dims is the final position into the array for the sequential index specification <c>idx</c>.</remarks>
        internal override object GetValueSeq(int idx, ref int[] dims) {
            int IdxCpy = idx; 
            for (int d = 0; d<m_size.NumberOfDimensions; d++) {
                dims[d] = (IdxCpy % m_size[d]); 
                IdxCpy /= m_size[d];  // must go to end, to fully clear dims
            }
            return m_data.Data[idx];
        }
        /// <summary>
        /// Set single value to element at the specified index.
        /// </summary>
        /// <param name="value">New value.</param>
        /// <param name="idx">Index of the element to be altered.</param>
        internal override void SetValue(object value, params int[] idx) {
            try {
                SetValueTyped((ElementType)value, idx);
            } catch (InvalidCastException) {
                SetValueTyped((ElementType)Convert.ChangeType(value, typeof(ElementType)), idx);
            }
        }
        /// <summary>
        /// Set value of element at the specified position.
        /// </summary>
        /// <param name="value">new value</param>
        /// <param name="idx">position of the element to be altered</param>
        /// <remarks><para>This function does support automatic expansion of the array
        /// if indices lay outside the dimension limits of the array. However, because 
        /// of ambiguity reasons this is not reliable supported for vector sized arrays.</para></remarks>
        internal override void SetValueTyped(ElementType value, params int[] idx) {
            try {
                if (idx.Length < 2) {
                    if (idx.Length == 1) {
                        if (idx[0] >= m_size.NumberOfElements)
                            throw new IndexOutOfRangeException(); 
                        GetArrayForWrite()[idx[0]] = value; 
                    }
                    return; 
                }
                int i = m_size.IndexFromArray(idx);
                if (i >= m_size.NumberOfElements)
                    throw new IndexOutOfRangeException(); 
                GetArrayForWrite()[i] = (ElementType)value; 
            } catch (Exception exc) {
                if (exc is ILArgumentException 
                    || exc is IndexOutOfRangeException ) {
                    // expanding ?
                    int [] dimensions = m_size.ToIntArray(Math.Max(idx.Length,m_size.NumberOfDimensions));  
                    bool mustExpand = false; 
                    int i = m_size.IndexFromArray(ref mustExpand, ref dimensions, idx);  
                    if (mustExpand) {
                        ExpandArray(dimensions);
                        m_data.Data[i] = value;
                    } else {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// [depricated] Convert index array into sequential index for storage access.
        /// </summary>
        /// <param name="idx">int array with dimension specification.</param>
        /// <returns>Index of requested value inside the solid storage. This 
        /// value can directly be used to query the corresponding value via GetArrayForRead()[return_value].
        /// </returns>
        /// <remarks><b>This function is deprecated! Use <see cref="ILNumerics.ILSize.IndexFromArray(int[])"/> instead!</b><br/>
        /// If the length of idx is smaler than the number of dimensions 
        /// of this storage, the trailing dimensions will be replaced with "0". I.e 
        /// the first index of each non specified dimension will be used. 
        /// If length of idx is larger than the dimensions of this storage, the behavior
        /// is undefined. Therefore this function should be enclosed in try, catch blocks 
        /// to handle this case!</remarks>
        internal int getBaseIndex(params int[] idx) {
            // arghs! -> ugly! piuh!!
            if (idx == null) throw new ILArgumentException("indices specified must not be null"); 
            if (idx.Length < m_size.NumberOfDimensions) {
                bool dummy; 
                int [] tmp = ILMemoryPool.Pool.New<int>(m_size.NumberOfDimensions,true, out dummy); 
                for (int i = 0; i < idx.Length; i++) {
                    tmp[i] = idx[i]; 
                }
                return m_size.IndexFromArray(tmp);
            }
            return m_size.IndexFromArray(idx); 
        }
        /// <summary>
        /// [depricated] Convert index array into sequential index for storage access. Ommit any bound checking. 
        /// </summary>
        /// <param name="idx">int array with dimensions specification.</param>
        /// <param name="MustExpand">Output parameter. On return determine, if the index 
        /// specification points outside of the dimensions of this ILDenseStorage and the array 
        /// must be expanded before accessing elements on that position.</param>
        /// <param name="dimensions">if the array was found to be expanded, this are the 
        /// needed dimension sizes for the new array. The sizes are computed from the range 
        /// specification given.</param>
        /// <returns>Index of requested value inside the solid storage. This 
        /// value may directly be used to query the value via m_data[return_value].
        /// The value returned is valid for solid storages as well as for reference 
        /// storages.
        /// </returns>
        /// <remarks>
        /// <para>idx must be not null and must contain at least one element.</para>
        /// <para>If the length of idx is smaler than the number of dimensions 
        /// of this storage, the trailing dimensions will be replaced with "0". I.e 
        /// the first index of each non specified dimensions will be used. 
        /// If length of idx is larger than the dimensions of this storage, the index of 
        /// the expanded array will be returned.</para></remarks>
        internal int getBaseIndex(ref bool MustExpand, ref int[] dimensions, params int[] idx) {
            int destIdx = 0;
            if (idx.Length == 1) {
                destIdx = idx[0]; 
                if (destIdx < 0)
                    throw new ILArgumentException("check index for dimension 0!");
                if (destIdx >= m_size.NumberOfElements) {
                    MustExpand = true; 
                    dimensions[0] = destIdx + 1; 
                }
                return destIdx;
            }
            return m_size.IndexFromArray(ref MustExpand, ref dimensions, idx);
        }
        /// <summary>
        /// Copy values of all elements into System.Array.
        /// </summary>
        /// <param name="result">System.Array, holding all element values of this ILDenseStorage.</param>
        /// <remarks>The System.Array may be predefined. If its length is sufficient, it will be used and 
        /// its leading elements will be overwritten when function returns. If 'result' is null or has too few elements, 
        /// it will be recreated from the ILNumerics memory pool.</remarks>
        public void ExportValues(ref ElementType[] result) {
            if (result == null || result.Length < m_size.NumberOfElements)
                result = ILMemoryPool.Pool.New<ElementType>(m_size.NumberOfElements);
            int pos = 0; 
            foreach (ElementType v in this) {
                result[pos++] = v; 
            }
        }
        /// <summary>
        /// Get direct reference to inner System.Array storage for <b>write access</b>
        /// </summary>
        /// <returns>reference to inner System.Array</returns>
        /// <remarks>Altering this array can be done directly. If necessary, the array is detached before 
        /// returned. Watch the column order format of storages in ILNumerics! Keep in minds, the length 
        /// of the array may exceeds the number of elements.
        /// <para>Accessing the inner system array directly should be left to ILNumerics experts only! 
        /// Unless you really know, what you are doing, you should rather use the higher order access 
        /// methods provided by ILArray&lt;T>! (You have been warned!)</para></remarks>
        internal ElementType[] GetArrayForWrite() {
            if (m_data.ReferenceCount > 1) {
                Detach(); 
            }
            return m_data.Data; 
        }
        /// <summary>
        /// Get direct reference to inner System.Array storage for <b>read access</b>
        /// </summary>
        /// <returns>reference to inner System.Array for reading</returns>
        /// <remarks>This method is provided for experts only! Altering elements of this 
        /// array may cause the data to be invalidated or corrupted! Use this array only for reading! Note 
        /// the ILNumerics array storage format (column major). Keep in mind, the length 
        /// of the array may exceeds the number of elements! 
        /// <para>Accessing the inner system array directly should be left to ILNumerics experts only! 
        /// Unless you really know, what you are doing, you should rather use the higher order access 
        /// methods provided by ILArray&lt;T>! (You have been warned!)</para></remarks>
        internal ElementType[] GetArrayForRead() {
            //if (PendingTasks > 0) 
            //    System.Threading.SpinWait.SpinUntil(() => { return PendingTasks == 0; });
            return m_data.Data; 
        }

        #endregion 

        /// <summary>
        /// Create lazy,shallow copy of this array 
        /// </summary>
        /// <returns>ILDenseStorage as copy of this storage.</returns>
        /// <remarks>The ILDenseStorage object returned will be of the same size and type than this object.
        /// <para>The copy is done lazy. This means, the new storage will at first share the memory 
        /// with that storage. This will take almost no memory / processor time. As soon as attempts 
        /// are made to <b>alter</b> the new storage, it will be detached from this storage and use own memeory.</para></remarks>
        internal override ILStorage Clone() { 
            return CreateSelf(m_data,m_size); 
        }
        internal ILCountableArray<ElementType> GetDataArray() {
            return m_data;
        }
        internal virtual void IndexSubrange(ILDenseStorage<ElementType> value, ILBaseArray[] range) {
            using (ILScope.Enter(range)) {
                if (Object.ReferenceEquals(value, null) || value.Size.NumberOfElements == 0) {
                    #region remove
                    if (range == null) return;

                    ILLeftSideRange rng = new ILLeftSideRange(Size, range);
                    if (rng.Expanding) {
                        throw new ILArgumentException("invalid range for removal specified");
                    }
                    if (rng.Size.NumberOfElements == 0) {
                        return;
                    }
                    int nonFullDims = 0;
                    int dimIdx = -1;
                    // check validity
                    for (int i = 0; i < rng.RangeArray.Length; i++) {
                        if (rng[i].Count != 1 || rng[i][0] > 0 || (rng[i][0] == 0 && Size[i] != 1)) {
                            if (++nonFullDims > 1)
                                throw new ILArgumentException("for removal only one dimension can be 'non-full'");
                            dimIdx = i;
                            foreach (int ind in rng[i]) {
                                if (ind < 0) {
                                    throw new ILArgumentException("invalid removal indices: all but at most one dimensions must be specified as 'full'. check dimension #" + i.ToString());
                                }
                            }
                        }
                    }
                    // remove
                    ILSize newDim = new ILSize(Size.ToIntArrayEx(rng.RangeArray.Length));
                    ILSize oldDimensions = Size;
                    try {
                        Reshape(newDim); // <- cheap!
                        if (dimIdx >= 0) {
                            Remove(dimIdx, rng[dimIdx]); // <- expensive! 
                        } else {
                            // all dims full specified
                            Remove(dimIdx, null); // <- expensive! 
                        }
                    } catch (Exception) {
                        Reshape(oldDimensions);
                        throw;
                    }
                    #endregion
                } else {
                    #region setrange
                    if (range == null || range.Length == 0) {
                        return;
                    } else if (range.Length == 1) {
                        if (range[0] is ILDenseArray<double>) {
                            SetRange(range[0] as ILDenseArray<double>, value);
                            return;
                        } else if (range[0] is ILDenseArray<string>) {
                            // special case? A[":;0:3;0:end;..."] -> multiple dimensions given as single string
                            string indStr = (string)(range[0] as ILDenseArray<string>).GetValue(0);
                            string[] dimParts = indStr.Split(';');
                            if (dimParts.Length == 0) {
                                // empty range given 
                                return;
                            } else if (dimParts.Length > 1) {
                                range = new ILBaseArray[dimParts.Length];
                                for (int i = 0; i < dimParts.Length; i++) {
                                    range[i] = dimParts[i];
                                } 
                                // re-enter function to push the new arrays into scope
                                IndexSubrange(value,range); 
                                return; 
                            } else {
                                ILBaseArray indices = ILRange.ParseDimension(indStr, Size.NumberOfElements);
                                if (indices is ILBaseArray<Misc.ILFullRange>) {
                                    SetRangeFull(value);
                                } else {
                                    SetRange(indices as ILDenseArray<int>, value);
                                }
                                return;
                            }
                        } else if (range[0] is ILDenseArray<float>) {
                            SetRange(range[0] as ILDenseArray<float>, value);
                            return;
                        } else if (range[0] is ILDenseArray<Int32>) {
                            SetRange(range[0] as ILDenseArray<Int32>, value);
                            return;
                        } else if (range[0] is ILDenseArray<Int64>) {
                            SetRange(range[0] as ILDenseArray<Int64>, value);
                            return;
                        } else if (range[0] is ILLogical) {
                            SetRange(ILNumerics.ILMath.find(range[0] as ILLogical), value);
                            return;
                        } else if (range[0] is ILRetLogical) {
                            SetRange(ILNumerics.ILMath.find(range[0] as ILRetLogical), value);
                            return;
                        }
                    }
                    ILLeftSideRange rng = new ILLeftSideRange(Size, range);
                    if (rng.Expanding) {
                        ExpandArray(rng);
                    }
                    SetRange(rng, value);
                    #endregion
                }
            }
        }

        #region IEnumerable<ILBaseArray<ElementType>> Member

        /// <summary>
        /// enumerator returning elements as ElementType
        /// </summary>
        public override IEnumerator<ElementType> GetEnumerator ( ) {
            int len = m_size.NumberOfElements; 
            ElementType[] myData = GetArrayForRead(); 
            for (int i = 0; i < len; i++) 
                yield return myData[i];
        }
        #endregion

        #region private helper 
        /// <summary>
        /// helper function to gather some parameters for partial dimension removal 
        /// </summary>
        /// <param name="rng">object with index specifications. May be of 
        /// type ILBaseArray[] with numeric arrays or a string array according 
        /// to the format of <see cref="ILNumerics.Storage.ILRange"/>. 
        /// </param>
        /// <param name="dimensionIdx">Out parameter: index of dimension the indices to be removed lie in.</param>
        /// <param name="indices">Indices to be removed.</param>
        /// <param name="dimensions">Dimension structure, can be used to reshape the storage <b>before</b> the removal</param>
        /// <remarks>If range comprises a range dimension specification which is smaller than 
        /// the actual number of dimension of this storage, the storage must be reshaped in advance of the removal. 
        /// This reshaping proccess will <b>not</b> be done inside this function! However 
        /// the <c>dimension</c> value returned reflects the size of the storage before removing and therefore
        /// can be utilized for reshaping the storage.</remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If:<list type="bullet">
        /// <item>The length of range exceeds the dimensions of this storage.</item>
        /// <item>More than one or less than one dimension of <c>range</c> was not null.</item>
        /// <item>The type of range was invalid, or</item>
        /// <item>Range is of type <see cref="ILNumerics.ILBaseArray"/>, but the element type is not numeric</item>
        /// </list></exception>
        internal void ExtractRemovalParameter(ILBaseArray[] rng, out int dimensionIdx, ref ILIntList indices, out ILSize dimensions) {
            dimensionIdx = 0;
            dimensions = null;
            if (rng.Length > m_size.NumberOfDimensions)
                throw new ILArgumentException("Error removing: dimension specification exceeds matrix dimensions.");
            int specCount = 0;
            int tmp = 0;
            if (rng.Length == 1 && rng[0] is ILBaseArray<string>) {
                string allRangeString = (string)(rng[0] as ILBaseArray<string>).GetValue(0);
                string[] ranges = allRangeString.Split(';'); 
                if (ranges.Length > 1) {
                    rng = new ILBaseArray[ranges.Length];
                    for (int i = 0; i < ranges.Length; i++) {
                        rng[i] = ranges[i]; 
                    }
                }
            }
            int[] outDim = Size.GetReshapedSize(rng.Length); 
            for (int i = 0; i < rng.Length; i++) {
                if (object.Equals(rng[i], null)) {
                    // nothing to remove
                    if (indices != null) {
                        indices.Clear(); 
                    }
                    return; 
                } else if (!rng[i].IsEmpty) {
                    if (rng[i] is ILDenseArray<string>) {
                        string stringVal = (rng[i] as ILDenseArray<string>).GetValue(0);  
                        ILBaseArray indFromString = ILRange.ParseDimension(stringVal,outDim[i]);
                        if (!(indFromString is ILBaseArray<ILFullRange>)) {
                            if (specCount++ > 0) {
                                throw new ILArgumentException("only one dimension can be non-fully specified for removal");
                            }
                            dimensionIdx = i;
                            indices = new ILIntList(indFromString as ILDenseArray<int>);
                        }
                        continue; 
                    } 
                    dimensionIdx = i;
                    if (specCount++ > 0) {
                        throw new ILArgumentException("only one dimension can be non-fully specified for removal");
                    }
                    if (rng[i] is ILBaseArray<Expression>) {
                        // A[end + ...] given
                        Expression expr = (rng[i] as ILBaseArray<Expression>).GetValue(0); 
                        int exprVal = ILExpression.Evaluate(expr, Size[i] - 1); 
                        indices = ILIntList.Create();
                        for (int p = 0; p < tmp; p++) {
                            indices.Add(exprVal);
                        }
                    } else if (rng[i] is ILLogical) {
                        ILArray<int> ind = ILNumerics.ILMath.find((ILLogical)rng[i]);
                        tmp = ind.Size.NumberOfElements;
                        indices = ILIntList.Create();
                        for (int p = 0; p < tmp; p++) {
                            indices.Add((int)ind.GetValue(p));
                        }
                    } else if (rng[i] is ILRetLogical) {
                        ILArray<int> ind = ILNumerics.ILMath.find((ILRetLogical)rng[i]);
                        tmp = ind.Size.NumberOfElements;
                        indices = ILIntList.Create();
                        for (int p = 0; p < tmp; p++) {
                            indices.Add((int)ind.GetValue(p));
                        }

/* !HC:TYPELIST:
<hycalper>
<type>
<source locate="after">
    inArr1
</source>
<destination>float</destination>
<destination>Int16</destination>
<destination>Int32</destination>
<destination>Int64</destination>
<destination>byte</destination>
</type>
</hycalper>
*/

                    } else if (rng[i] is ILDenseArray<double> ) {
                        ILDenseArray<double> ind = rng[i] as ILDenseArray<double> ;
                        tmp = ind.Size.NumberOfElements;
                        indices = ILIntList.Create();
                        for (int p = 0; p < tmp; p++) {
                            indices.Add((int)ind.GetValue(p));
                        }

#region HYCALPER AUTO GENERATED CODE

/* !HC:TYPELIST:

*/

                    } else if (rng[i] is ILDenseArray<byte> ) {
                        ILDenseArray<byte> ind = rng[i] as ILDenseArray<byte> ;
                        tmp = ind.Size.NumberOfElements;
                        indices = ILIntList.Create();
                        for (int p = 0; p < tmp; p++) {
                            indices.Add((int)ind.GetValue(p));
                        }
/* !HC:TYPELIST:

*/

                    } else if (rng[i] is ILDenseArray<Int64> ) {
                        ILDenseArray<Int64> ind = rng[i] as ILDenseArray<Int64> ;
                        tmp = ind.Size.NumberOfElements;
                        indices = ILIntList.Create();
                        for (int p = 0; p < tmp; p++) {
                            indices.Add((int)ind.GetValue(p));
                        }
/* !HC:TYPELIST:

*/

                    } else if (rng[i] is ILDenseArray<Int32> ) {
                        ILDenseArray<Int32> ind = rng[i] as ILDenseArray<Int32> ;
                        tmp = ind.Size.NumberOfElements;
                        indices = ILIntList.Create();
                        for (int p = 0; p < tmp; p++) {
                            indices.Add((int)ind.GetValue(p));
                        }
/* !HC:TYPELIST:

*/

                    } else if (rng[i] is ILDenseArray<Int16> ) {
                        ILDenseArray<Int16> ind = rng[i] as ILDenseArray<Int16> ;
                        tmp = ind.Size.NumberOfElements;
                        indices = ILIntList.Create();
                        for (int p = 0; p < tmp; p++) {
                            indices.Add((int)ind.GetValue(p));
                        }
/* !HC:TYPELIST:

*/

                    } else if (rng[i] is ILDenseArray<float> ) {
                        ILDenseArray<float> ind = rng[i] as ILDenseArray<float> ;
                        tmp = ind.Size.NumberOfElements;
                        indices = ILIntList.Create();
                        for (int p = 0; p < tmp; p++) {
                            indices.Add((int)ind.GetValue(p));
                        }

#endregion HYCALPER AUTO GENERATED CODE
                   } else
                        throw new ILArgumentTypeException("error removing: invalid dimensions specifier, dimension #" + i.ToString());
                }
            }
            if (indices == null) {
                dimensionIdx = 0;
                dimensions = m_size;
                //indices = new int[0] { };
                return;
            }
            // if only one dimension specified -> make row vector
            //if (rng.Length < 2) {
            //    outDim[1] = outDim[0];
            //    outDim[0] = 1;
            //    dimensionIdx = 1;
            //}
            dimensions = new ILSize(outDim);
        }

        /// <summary>
        /// Expanded <b>this</b> storage for index addressing outside of my dimensions
        /// </summary>
        /// <param name="range">range specification with size for destination array</param>
        internal void ExpandArray (ILLeftSideRange range) {
            int[] outDims; 
            int i= 0;
            ILSize outDimensions; 
            if (range.Size.NumberOfDimensions > m_size.NumberOfDimensions) {
                outDims = new int[range.Size.NumberOfDimensions]; 
                for (; i < m_size.NumberOfDimensions; i ++) {
                    outDims[i] = (range.ExpandDimensions[i] > m_size[i]) ?
                        range.ExpandDimensions[i] : m_size[i]; 
                }
                for(; i < range.Size.NumberOfDimensions; i++) 
                    outDims[i] = range.ExpandDimensions[i];
            } else {
                outDims = new int[m_size.NumberOfDimensions]; 
                for (; i < range.Size.NumberOfDimensions; i ++) {
                    outDims[i] = (range.ExpandDimensions[i] > m_size[i]) ?
                        range.ExpandDimensions[i] : m_size[i]; 
                }
                for(; i < m_size.NumberOfDimensions; i++) 
                    outDims[i] = m_size[i];
            }
            outDimensions = new ILSize(outDims); 
            ElementType [] outData = ILMemoryPool.Pool.New<ElementType>(outDimensions.NumberOfElements); 
            // transfer old data to new array
            int [] tmpIdx = new int[outDims.Length]; 
            for (i = 0; i < m_size.NumberOfElements; i++) {
                ElementType tmp = (ElementType)GetValueSeq(i,ref tmpIdx); 
                outData[outDimensions.IndexFromArray(tmpIdx)] = tmp; 
            }
            // exchange my data 
            Data = new ILCountableArray<ElementType>(outData, outDimensions.NumberOfElements); 
            m_size = outDimensions; 
        }
        /// <summary>
        /// Expand <b>this</b> storage for index addressing outside of my dimensions
        /// </summary>
        /// <param name="indices">sizes of dimensions for the new storage</param>
        protected void ExpandArray(int[] indices) {
            if (indices.Length == 2 && (indices[0] * indices[1] == 0)) {
               if (indices[0] > 0) 
                   indices[1] = 1; 
               else if (indices[1] > 0) 
                   indices[0] = 1; 
            }
            ILSize outDimensions = new ILSize(indices); 
            bool cleared; 
            ElementType [] outData = ILMemoryPool.Pool.New<ElementType>(outDimensions.NumberOfElements,true,out cleared); 
            // transfer old data to new array
            int [] tmpIdx = new int[indices.Length]; 
            for (int i = 0; i < m_size.NumberOfElements; i++) {
                ElementType tmpData = (ElementType)GetValueSeq(i,ref tmpIdx); 
                outData[outDimensions.IndexFromArray(tmpIdx)] = tmpData; 
            }
            // replace with my data
            Data = new ILCountableArray<ElementType>(outData,outDimensions.NumberOfElements); 
            m_size = outDimensions; 
        }
        /// <summary>
        /// Copy upper triangular part of this array into new solid array.
        /// </summary>
        /// <param name="n">Length of first dimension of destination array.</param>
        /// <returns>Solid array of size [n x {ThisColumnCount})].</returns>
        internal ILDenseStorage<ElementType> copyUpperTriangle(int n) {
            ILDenseStorage<ElementType> ret = new ILDenseStorage<ElementType>(new ILSize(n,n)); 
            ElementType[] arr = ret.GetArrayForWrite();
            ElementType[] myData = GetArrayForRead();
            if (m_size[0] == n) {
                for (int rcount = 0 , pos = 0; rcount < n; rcount++) {
                    for (int i = 0; i <= rcount; i++) {
                        arr[pos] = myData[pos++];
                    }
                    pos += (n - rcount - 1);
                }
            } else {
                for (int rcount = 0 , posIn = 0, posOut = 0, lenA = m_size[0]; rcount < n; rcount++) {
                    for (int i = 0; i <= rcount; i++) {
                        arr[posOut++] = myData[posIn++];
                    }
                    posOut += (n - rcount - 1);
                    posIn += (lenA - rcount - 1);
                }
            }
            return ret;
        }
        /// <summary>
        /// Copy lower triangular part of this array into new solid array.
        /// </summary>
        /// <returns>Solid array of same size than this array.</returns>
        /// <remarks>If this is not a 2D array, only the first dimension is referenced.</remarks>
        internal ILDenseStorage<ElementType> copyLowerTriangle() {
            int n = m_size[0],pos = 0; 
            ILDenseStorage<ElementType> ret = new ILDenseStorage<ElementType>(new ILSize(n,n)); 
            ElementType[] arr = ret.GetArrayForRead();
            ElementType[] myData = GetArrayForRead();
            for (int c = 0; c < m_size[1]; c++) {
                pos += c;
                for (int r = c; r < n; r++,pos++) {
                    arr[pos] = myData[pos];
                    pos++; 
                } 
            }
            return ret; 
        }

        #endregion

    }
}
