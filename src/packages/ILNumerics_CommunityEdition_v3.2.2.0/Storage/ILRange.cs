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
using ILNumerics.Exceptions;
using ILNumerics.Data;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ILNumerics.Storage {

	/// <summary>
	/// base class for ranges, used to define subarray ranges
    /// </summary>
	/// <description>ILRange is used to define those parts of indices of an array,
    /// which are to be extracted into a new subarray. 
    /// The class (and derived classes) parse indices given for each dimension and expand them into a 2-dimensional 
    /// integer array, needed for fast element traversal.
    /// <para>The class is internally used only and not intended to be used from outside ILNumerics.</para></description>
	internal abstract class ILRange : IDisposable {

		/// <summary>
		/// hold ranges as 2 dimensional System.Array
		/// </summary>
		protected ILIntList[] m_range; 
        /// <summary>
        /// for performance reasons: give reference to internal array
        /// </summary>
        internal ILIntList[] RangeArray {
            get{
                return m_range;
            }
        }
		/// <summary>
		/// Index access for ILRange objects. Set/returns Index array for specified dimension.
        /// </summary>
        /// <remarks>A reference to the internal object will be returned directly! (performance)</remarks>
		internal ILIntList this [int idx] {
			get {
				return m_range[idx];
            }
		}
		/// <summary>
		/// Index access for ILRange objects. returns the destIndex destination dimension for dimension specifyied by dimNr.
		/// </summary>
		/// <remarks>The access is readonly! No checks are made, if the requested indices exist.</remarks>
		internal int this [int dimNr,int destIndx ] {
			get {
				return m_range[dimNr][destIndx];
			}
		}

        internal void ILRanges() {}

		/// <summary>
		/// Evaluates (maps) index array on my range. 
		/// </summary>
		/// <param name="idx">int array indexing location inside this range</param>
        /// <param name="retIdx">(output) also return the result into array given</param>
		/// <returns>Mapped int[] array. It can be used for direct addressing the physical storage object.</returns>
        /// <remarks>retIdx must be at least of length m_nrDims. No check is made for this assumption!</remarks>
		internal int[] Map(int[] idx, ref int [] retIdx) {
            int tmpVal; 
			for (int d = 0; d < m_size.NumberOfDimensions; d++) {
				tmpVal = m_range[d][0]; 
                if (tmpVal < 0)
                    retIdx[d] = idx[d];
                else 
                    retIdx[d] = m_range[d][idx[d]];
			} 
            return retIdx; 
		}
        /// <summary>
        /// parse single dimension specifier from string
        /// </summary>
        /// <param name="indices">valid index specification</param>
        /// <param name="dimlen">number of elements in dimension to be parsed</param>
        /// <returns>array with indices defined in 'indices'</returns>
        /// <remarks>the indices are parsed the way needed for sequential addressing. This means: 
        /// full dimensions address the whole array. Full dimensions are transformed into negative 
        /// placeholder indices nevertheless! SetRange must handle that accordingly.</remarks>
        internal static ILBaseArray ParseDimension(string indices, int dimlen) {
            if (indices.Trim() == ":") {
                return ILMath.full; 
            }
            int pos = 0, min = 0, max = 0; 
            ILIntList list = ILIntList.Create(); 
            expand(list, ref pos, indices, ref min, ref max, dimlen, 0, false); 
            
            return list.ToTempArray(); 
        }

        protected ILSize m_size; 
        /// <summary>
        /// Create trimmed size descriptor from indices in ILRange object
		/// </summary>
        /// <returns>new size descriptor with the neccessary size to define 
        /// an array as defined by all indices in this range</returns>
		internal ILSize Size {
            get {
                return m_size; 
            }
		}

        #region private helper  
        protected static void extractDimension(ILIntList target, ref int outLen, ILBaseArray A, int dim, ref int min, ref int max, int[] dimensions) {
            if (object.Equals(A, null)) {
                return;
            }
            using (ILScope.Enter(A)) {
                int dimLen = dimensions[dim];
                if (A is ILDenseArray<ILFullRange>) {
                    // taken as ':' (i.e.: full)
                    if (dimLen <= 0) return;
                    min = 0;
                    target.Add(-(dimLen - 1));
                    max = dimLen - 1;
                    outLen += dimLen;
                } else if (A is ILBaseArray<ILRegularRange>) {
                    (A as ILBaseArray<ILRegularRange>).GetValue(0).Expand(target, ref outLen, ref min, ref max, dimLen - 1);
                } else if (A is ILDenseArray<double>) {
                    expand(target, ref outLen, A as ILDenseArray<double>, ref min, ref max, dim);
                } else if (A is ILBaseArray<Expression>) {
                    if (dimensions[dim] > 0) {
                        Expression exp = (A as ILBaseArray<Expression>).GetValue(0);
                        int end = ILExpression.Evaluate(exp, dimLen - 1);
                        target.Add(end);
                        outLen++;
                        if (end < min) min = end;
                        if (end > max) max = end;
                    }
                } else if (A is ILDenseArray<float>) {
                    expand(target, ref outLen, A as ILDenseArray<float>, ref min, ref max, dim);
                } else if (A is ILDenseArray<string>) {
                    if (!A.IsScalar)
                        throw new ILArgumentException("string index specification: only one string is allowed per dimension");
                    expand(target, ref outLen, (A as ILDenseArray<string>).GetValue(0), ref min, ref max, (dim < dimensions.Length) ? dimLen : 0, dim, true);
                } else if (A is ILDenseArray<int>) {
                    expand(target, ref outLen, A as ILDenseArray<int>, ref min, ref max, dim);
                } else if (A is ILDenseArray<byte>) {
                    expand(target, ref outLen, ILMath.find((A as ILDenseArray<byte>).C), ref min, ref max, dim);
                } else if (A is ILDenseArray<Int64>) {
                    expand(target, ref outLen, A as ILDenseArray<Int64>, ref min, ref max, dim);
                } else if (A.Storage is ILCellStorage) {
                    ILCellStorage cellStorage = A.Storage as ILCellStorage;
                    for (int ind = 0; ind < cellStorage.Size.NumberOfElements; ind++ ) {
                        extractDimension(target, ref outLen, cellStorage.GetScalar(ind), dim, ref min, ref max, dimensions);
                    }
                } else
                    throw new ILArgumentException(String.Format("invalid type of index specification: '{0}'", A.GetType().Name));
            }
        }



        protected static void expand(ILIntList target, ref int outLen, ILDenseArray</*HC:inArr1*/ double> A, ref int min, ref int max, int dimIdx) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return;
                }
                if (!A.IsVector)
                    throw new ILArgumentException("index specification must be vector sized. found: " + A.Size.ToString());
                foreach (/*HC:inArr1*/ double val in A) {
                    /*HC:NaNCheck*/
                    if (Double.IsInfinity(val) || Double.IsNaN(val)) throw new ILArgumentException("invalid index specification, dimension idx " + dimIdx);
                    Int32 v = (int)val;
                    target.Add(v);
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
                outLen += A.Length;
            }
        }

#region HYCALPER AUTO GENERATED CODE


        protected static void expand(ILIntList target, ref int outLen, ILDenseArray</*HC:*/ Int64> A, ref int min, ref int max, int dimIdx) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return;
                }
                if (!A.IsVector)
                    throw new ILArgumentException("index specification must be vector sized. found: " + A.Size.ToString());
                foreach (/*HC:*/ Int64 val in A) {
                    /*HC:NaNCheck*/
                    if (Double.IsInfinity(val) || Double.IsNaN(val)) throw new ILArgumentException("invalid index specification, dimension idx " + dimIdx);
                    Int32 v = (int)val;
                    target.Add(v);
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
                outLen += A.Length;
            }
        }

        protected static void expand(ILIntList target, ref int outLen, ILDenseArray</*HC:*/ Int32> A, ref int min, ref int max, int dimIdx) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return;
                }
                if (!A.IsVector)
                    throw new ILArgumentException("index specification must be vector sized. found: " + A.Size.ToString());
                foreach (/*HC:*/ Int32 val in A) {
                    /*HC:NaNCheck*/
                    if (Double.IsInfinity(val) || Double.IsNaN(val)) throw new ILArgumentException("invalid index specification, dimension idx " + dimIdx);
                    Int32 v = (int)val;
                    target.Add(v);
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
                outLen += A.Length;
            }
        }

        protected static void expand(ILIntList target, ref int outLen, ILDenseArray</*HC:*/ float> A, ref int min, ref int max, int dimIdx) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return;
                }
                if (!A.IsVector)
                    throw new ILArgumentException("index specification must be vector sized. found: " + A.Size.ToString());
                foreach (/*HC:*/ float val in A) {
                    /*HC:NaNCheck*/
                    if (Double.IsInfinity(val) || Double.IsNaN(val)) throw new ILArgumentException("invalid index specification, dimension idx " + dimIdx);
                    Int32 v = (int)val;
                    target.Add(v);
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
                outLen += A.Length;
            }
        }

        protected static void expand(ILIntList target, ref int outLen, ILDenseArray</*HC:*/ byte> A, ref int min, ref int max, int dimIdx) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) {
                    return;
                }
                if (!A.IsVector)
                    throw new ILArgumentException("index specification must be vector sized. found: " + A.Size.ToString());
                foreach (/*HC:*/ byte val in A) {
                    /*HC:NaNCheck*/
                    if (Double.IsInfinity(val) || Double.IsNaN(val)) throw new ILArgumentException("invalid index specification, dimension idx " + dimIdx);
                    Int32 v = (int)val;
                    target.Add(v);
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
                outLen += A.Length;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        // parse SINGLE (!) dimension given by string
        protected static void expand(ILIntList target, ref int outLen, string p, ref int min, ref int max, int dimLen, int dimIdx, bool useFullDimTag) {
            // possible input: ":", "0:19", "20:-2:5", "1,2,3,4:2:16,end:-2:0,:"
            if (p.Contains(";")) 
                throw new ILArgumentException("invalid string index specification: ambiguous dimension seperator ';'"); 
            string[] parts = p.Split(','); 
            if (parts.Length == 0) {
                // empty dimension? 
                return; 
            } else if (parts.Length > 1) {
                foreach (string part in parts) {
                    expand(target, ref outLen, part, ref min, ref max, dimLen, dimIdx, useFullDimTag); 
                }
                return; 
            }
            // only one range specifier 
            if (p.Trim() == ":") {
                // full dimension
                if (dimLen <= 0) return; 
                if (useFullDimTag) {
                    target.Add(-(dimLen-1)); 
                } else {
                    for (int i = 0; i < dimLen; i++) {
                        target.Add(i); 
                    }
                }
                outLen += dimLen; 
            } else {
                int tmpValue, start, end, step; 
                string[] rngitems = p.Split(':');
                switch (rngitems.Length) {
                    case 1:
                        // single number specified 
                        if (rngitems[0].Contains("end")) {
                            if (rngitems[0].Trim() != "end") throw new ILArgumentException("index expression evaluation is not supported for string definitions. Use non-string definitions instead!");
                            if (dimLen <= 0) break; 
                            tmpValue = (dimLen-1);
                        } else if (String.IsNullOrWhiteSpace(rngitems[0])) {
                            break;   
                        } else if (! int.TryParse(rngitems[0].Trim(),out tmpValue)) {
                            throw new ILArgumentException(String.Format("invalid index for dimension {0}: ({1})", dimIdx,rngitems[0])); 
                        }
                        if (tmpValue < min) min = tmpValue; 
                        if (tmpValue > max) max = tmpValue; 
                        target.Add (tmpValue);
                        outLen++; 
                        break;
                    case 2: 
                        // unstepped range specified 
                        if (rngitems[0].Contains("end")) {
                            if (rngitems[0].Trim() != "end") throw new ILArgumentException("index expression evaluation is not supported for string definitions. Use non-string definitions instead!");
                            if (dimLen <= 0) break; 
                            start = dimLen - 1;
                        } else if (!int.TryParse(rngitems[0],out start)) {
                            throw new ILArgumentException("invalid start index for range in dimension " + dimIdx.ToString());                                        
                        } 
                        if (rngitems[1].Contains("end")) {
                            if (rngitems[1].Trim() != "end") throw new ILArgumentException("index expression evaluation is not supported for string definitions. Use non-string definitions instead!");
                            if (dimLen <= 0) break; 
                            end = dimLen - 1;
                        } else if (!int.TryParse(rngitems[1],out end)) {
                            throw new ILArgumentException("invalid end index for range in dimension " + dimIdx.ToString());                                        
                        } 
                        step = (start <= end) ? 1 : 1; // -1 - make consistent with non-string spec: unspecified step is always 1! 

                        if (start<=end) {
                            for (int t = start; t <= end;) {
                                target.Add (t++);
                            }
                            if (start < min) min = start; 
                            if (end > max) max = end; 
                            outLen += (end-start)+1; 
                        } else {
                            //for (int t = start; t >= end;) {
                            //    target.Add (t--);
                            //}
                            //if (end < min) min = end; 
                            //if (start > max) max = start; 
                            //outLen += (start-end)+1

                            // ^ this is not allowed: unspecified step will always avaluate to 1! 
                        }
                        break;
                    case 3:
                        // stepped range specified
                        if (rngitems[0].Contains("end")) {
                            if (rngitems[0].Trim() != "end") throw new ILArgumentException("index expression evaluation is not supported for string definitions. Use non-string definitions instead!");
                            if (dimLen <= 0) break; 
                            start = dimLen - 1;
                        } else if (!int.TryParse(rngitems[0],out start)) {
                            throw new ILArgumentException("invalid start index for range in dimension " + dimIdx.ToString());                                        
                        } 
                        if (rngitems[1].Contains("end")) {
                            if (rngitems[1].Trim() != "end") throw new ILArgumentException("index expression evaluation is not supported for string definitions. Use non-string definitions instead!");
                            if (dimLen <= 0) break; 
                            step = dimLen - 1;
                        } else if (!int.TryParse(rngitems[1],out step)) {
                            throw new ILArgumentException("invalid step index for range in dimension " + dimIdx.ToString());                                        
                        } 
                        if (rngitems[2].Contains("end")) {
                            if (rngitems[2].Trim() != "end") throw new ILArgumentException("index expression evaluation is not supported for string definitions. Use non-string definitions instead!");
                            if (dimLen <= 0) break; 
                            end = dimLen - 1;
                        } else if (!int.TryParse(rngitems[2],out end)) {
                            throw new ILArgumentException("invalid end index for range in dimension " + dimIdx.ToString());                                        
                        } 
                        if (step == 0) {
                            // empty
                            return; 
                        }
                        int i = start; 
                        if (start == end) {
                            target.Add(start); 
                            outLen++; 
                            if (start < min) min = start; 
                            if (start > max) max = start;   
                        } else if (start < end) {
                            if (step < 0) return; 
                            for (; i <= end; i += step) {
                                target.Add(i); 
                                outLen++; 
			                }
                            if (start < min) min = start; 
                            if (i - step > max) max = i - step;   // cannot compare to end, since it may was not reached!
                        } else {
                            if (step > 0) return; 
                            for (; i >= end; i += step) {
                                target.Add(i); 
                                outLen++; 
			                }
                            if (start > max) max = start; 
                            if (i - step < min) min = i - step; 
                        }
                        break;
                    default: 
                        throw new ILArgumentException("invalid index for dimension " + dimIdx.ToString());
                }
            }
        }
        #endregion

        #region IDisposable Members

        public void Dispose() {
            if (m_range != null) {
                foreach (ILIntList list in m_range) {
                    if (list != null) {
                        list.Dispose();
                    }
                }
            }
            m_range = null; 
        }

        #endregion
    }
}
