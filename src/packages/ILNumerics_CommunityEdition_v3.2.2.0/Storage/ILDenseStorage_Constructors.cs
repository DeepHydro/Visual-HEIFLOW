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
using System.Linq.Expressions;
using System.Runtime.Serialization; 
using ILNumerics;
using ILNumerics.Misc;
using ILNumerics.Data;
using ILNumerics.Exceptions;

namespace ILNumerics.Storage {

    [System.Diagnostics.DebuggerTypeProxy(typeof(ILNumerics.Misc.ILArrayDebuggerProxy<>))]
    [System.Diagnostics.DebuggerDisplay("{ShortInfo(),nq}")]
    internal partial class ILDenseStorage<ElementType> : ILStorage<ElementType> {
        
        #region Constructors
        /// <summary>
        /// create new dense storage
        /// </summary>
        /// <param name="system_array">countable array</param>
        /// <param name="dimensions">dimensions</param>
        internal ILDenseStorage(ElementType[] system_array, ILSize dimensions) 
            : base(dimensions) {
                if (system_array == null || dimensions == null) 
                throw new ILArgumentException("system array and dimensions parameter may not be null");
#if DEBUG
            //DebuggerTraceHelper.Output.Append("\r\nILDenseStorage(ElementType[],dim) : "); 
            //var m = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
            //DebuggerTraceHelper.Output.Append(String.Format("Called from: {1} - {0}",m.ToString(),m.ReflectedType.Name)); 
#endif
            m_data = new ILCountableArray<ElementType>(system_array,dimensions.NumberOfElements); 
            m_data.IncreaseReference(); 
        }
        internal ILDenseStorage(ILCountableArray<ElementType> countableArray, ILSize dimensions) 
            : base(dimensions) {
                if (countableArray == null || dimensions == null) 
                throw new ILArgumentException("countableArray and dimensions parameter may not be null");
            m_data = countableArray; 
            m_data.IncreaseReference(); 
        }
        internal ILDenseStorage(ILSize dimensions) 
            : base(dimensions) {
            m_data = new ILCountableArray<ElementType>(dimensions.NumberOfElements,true); 
            m_data.IncreaseReference(); 
        }
        #endregion

        #region Destructor + Dispose

        /// <summary>
		/// dispose this storage
		/// </summary>
		/// <remarks><para>Calling Dispose should be the last method called for an ILDenseStorage.</para></remarks>
        internal override void Dispose(bool manual) {

#if VERBOSE
            System.Diagnostics.Debug.WriteLine("Disposing " + this.GetHashCode() + " called from: " + getCallee() ); 
#endif 
            if (manual) {
                if (Settings.s_measurePerformanceAtRuntime && m_size.NumberOfElements >= ILMemoryPool.Pool.MinArrayLength<ElementType>()) {
                    ILMemoryPoolInternal<ElementType>.s_performanceCounters.PCDisposedHighIncrement(); 
                }
            }
            if (!IsDisposed) {
                if (m_data != null) {
#if DEBUG
                    //DebuggerTraceHelper.Output.Append(String.Format("\r\nDenseStorage Dispose HashCode: " + GetHashCode())); 
#endif
                    m_data.DecreaseReference(); 
                    m_data = null; 
                }
            } 
        }
        /// <summary>
        /// detach this storage: copy its countable array if necessary
        /// </summary>
        protected virtual void Detach() {
            if (m_data.ReferenceCount <= 1) 
                return; 
            Data = m_data.CreateCopy(); 
        }
        #endregion

        #region Subarray interface
        /// <summary>
        ///	Subarray from this array
        /// </summary>
        /// <param name="range"> arrays specifying the ranges to create subarray from</param>
        /// <returns>subarray as specified</returns>
        internal virtual ILDenseStorage<ElementType> Subarray (params ILBaseArray[] range) {
            if (range.Length == 0) {
                return CreateSelf(ILSize.Empty00);
            } else if (range.Length == 1) {
                #region sequential index access
                if (object.Equals(range[0],null)) {
                    return CreateSelf(ILSize.Empty00);
                } else if (range[0] is ILBaseArray<ILFullRange>) {
                    return CreateSubarrayStorageFull(); 
                } else if (range[0] is ILBaseArray<ILRegularRange>) {
                    return CreateRangedSubarray(range[0] as ILBaseArray<ILRegularRange>); 
                } else if (range[0] is ILBaseArray<double>) {
                    return CreateSubarrayStorageSequential((ILBaseArray<double>)range[0]); 
                } else if (range[0] is ILDenseArray<string>) {
                    // special case? A[":;0:3;0:end;..."] -> multiple dimensions given as single string
                    string indStr = (string)(range[0] as ILDenseArray<string>).GetValue(0); 
                    string[] dimParts = indStr.Split(';'); 
                    if (dimParts.Length == 0) {
                        // empty range given 
                        return CreateSelf(ILSize.Empty00); 
                    } else if (dimParts.Length > 1) {
                        range = new ILBaseArray [dimParts.Length];
                        for (int i = 0; i < dimParts.Length; i++) {
                            range[i] = dimParts[i]; 
                        } // continue below: multi-dim range specification
                    } else { 
                        return Subarray(ILRange.ParseDimension(indStr, Size.NumberOfElements));  
                    }
                } else if (range[0].Storage is ILCellStorage) {
                    ILCellStorage indices = range[0].Storage as ILCellStorage;  
                    if (indices.Size.NumberOfElements == 1) {
                        return Subarray(indices.GetScalar(0));       
                    } else if (indices.Size.NumberOfElements == 0) {
                        return CreateSelf(ILSize.Empty00); 
                    } else {
                        ILRightSideRange rngCell = new ILRightSideRange(Size, range); 
                        return CreateSubarrayStorage(rngCell); 
                    }
                } else if (range[0] is ILRetLogical) {
                    return CreateSubarrayStorageSequential(ILNumerics.ILMath.find((ILRetLogical)range[0])); 
                } else if (range[0] is ILLogical) {
                    return CreateSubarrayStorageSequential(ILNumerics.ILMath.find((ILLogical)range[0])); 
                } else if (range[0] is ILBaseArray<float>) { 
                    return CreateSubarrayStorageSequential((ILBaseArray<float>)range[0]); 
                } else if (range[0] is ILBaseArray<Int32>) {
                    return CreateSubarrayStorageSequential((ILBaseArray<Int32>)range[0]); 
                } else if (range[0] is ILBaseArray<Int64>) {
                    return CreateSubarrayStorageSequential((ILBaseArray<Int64>)range[0]); 
                } else if (range[0] is ILExpression) {
                    return CreateSubarrayStorageSequential((ILExpression)range[0]); 
                } else 
                    throw new ILArgumentException ("specified type of indices array is not supported for sequential addressing!");
                #endregion sequential index access
            } 
            ILRightSideRange rng = new ILRightSideRange(m_size, range); 
            if (rng.Size.NumberOfElements == 0)
                return CreateSelf(rng.Size); 
            return CreateSubarrayStorage(rng); 
        }

        /// <summary>
        /// subarray from single dim, single range
        /// </summary>
        /// <returns>subarray (column vector)</returns>
        private ILDenseStorage<ElementType> CreateRangedSubarray(ILBaseArray<ILRegularRange> range) {
            System.Diagnostics.Debug.Assert(range.IsScalar); 
            ILRegularRange regRange = range.GetValue(0); 
            regRange.Evaluate(Size.NumberOfElements - 1); 
            bool dummy; 
            ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(regRange.Length,false, out dummy);
            regRange.Extract(GetArrayForRead(),retArr, Size.NumberOfElements - 1);
            return CreateSelf(retArr, new ILSize(regRange.Length, 1)); 
        }

        /// <summary>
        /// create new subarray storage, sequentially addressed elements
        /// </summary>
        /// <param name="indices">sequential indices, arbitrary size</param>
        /// <returns>new storage, type of this storage, size and shape of indices</returns>
        private ILDenseStorage<ElementType> CreateSubarrayStorageSequential(ILBaseArray<Expression> indices) {
            if (indices.Storage is  ILDenseStorage<Expression> ) {
                Expression expr = (indices.Storage as ILDenseStorage<Expression>).GetArrayForRead()[0]; 
                int idx = ILExpression.Evaluate(expr,Size.NumberOfElements-1); 
                if (idx < 0 || idx >= Size.NumberOfElements)
                    throw new ILArgumentException("sequential index out of range"); 
                return CreateSelf(new ElementType[]{GetArrayForRead()[idx]}, indices.Size);
            } else {
                throw new ILArgumentException("Unsupported storage method for indices!"); 
            }
        }


        

        /// <summary>
        /// create new subarray storage, sequentially addressed elements
        /// </summary>
        /// <param name="indices">sequential indices, arbitrary size</param>
        /// <returns>new storage, type of this storage, size and shape of indices</returns>
        private ILDenseStorage<ElementType> CreateSubarrayStorageSequential(ILBaseArray<double> indices) {
            if (indices.Storage is  ILDenseStorage< double > ) {
                  double [] indArr = (indices.Storage as ILDenseStorage< double >).GetArrayForRead(); 
                int outLen = indices.Size.NumberOfElements; 
                int myLen = Size.NumberOfElements; 
                ElementType[] outdata = ILMemoryPool.Pool.New<ElementType> (outLen);
                ElementType[] myData = GetArrayForRead(); 
                for (int i = 0; i < outLen; i++) {
                    int targetIndex = (int)indArr[i]; 
                    if (targetIndex < 0 || targetIndex >= myLen)
                        throw new ILArgumentException(String.Format("sequential index at position {0} out of range: {1}", i, targetIndex)); 
                    outdata[i] = myData[targetIndex]; 
                }
                return CreateSelf(outdata, indices.Size);
            } else {
                throw new ILArgumentException("unsupported storage type for indices"); 
            }
        }

#region HYCALPER AUTO GENERATED CODE

       

        /// <summary>
        /// create new subarray storage, sequentially addressed elements
        /// </summary>
        /// <param name="indices">sequential indices, arbitrary size</param>
        /// <returns>new storage, type of this storage, size and shape of indices</returns>
        private ILDenseStorage<ElementType> CreateSubarrayStorageSequential(ILBaseArray<long> indices) {
            if (indices.Storage is  ILDenseStorage< long > ) {
                 long [] indArr = (indices.Storage as ILDenseStorage< long >).GetArrayForRead(); 
                int outLen = indices.Size.NumberOfElements; 
                int myLen = Size.NumberOfElements; 
                ElementType[] outdata = ILMemoryPool.Pool.New<ElementType> (outLen);
                ElementType[] myData = GetArrayForRead(); 
                for (int i = 0; i < outLen; i++) {
                    int targetIndex = (int)indArr[i]; 
                    if (targetIndex < 0 || targetIndex >= myLen)
                        throw new ILArgumentException(String.Format("sequential index at position {0} out of range: {1}", i, targetIndex)); 
                    outdata[i] = myData[targetIndex]; 
                }
                return CreateSelf(outdata, indices.Size);
            } else {
                throw new ILArgumentException("unsupported storage type for indices"); 
            }
        }
       

        /// <summary>
        /// create new subarray storage, sequentially addressed elements
        /// </summary>
        /// <param name="indices">sequential indices, arbitrary size</param>
        /// <returns>new storage, type of this storage, size and shape of indices</returns>
        private ILDenseStorage<ElementType> CreateSubarrayStorageSequential(ILBaseArray<int> indices) {
            if (indices.Storage is  ILDenseStorage< int > ) {
                 int [] indArr = (indices.Storage as ILDenseStorage< int >).GetArrayForRead(); 
                int outLen = indices.Size.NumberOfElements; 
                int myLen = Size.NumberOfElements; 
                ElementType[] outdata = ILMemoryPool.Pool.New<ElementType> (outLen);
                ElementType[] myData = GetArrayForRead(); 
                for (int i = 0; i < outLen; i++) {
                    int targetIndex = (int)indArr[i]; 
                    if (targetIndex < 0 || targetIndex >= myLen)
                        throw new ILArgumentException(String.Format("sequential index at position {0} out of range: {1}", i, targetIndex)); 
                    outdata[i] = myData[targetIndex]; 
                }
                return CreateSelf(outdata, indices.Size);
            } else {
                throw new ILArgumentException("unsupported storage type for indices"); 
            }
        }
       

        /// <summary>
        /// create new subarray storage, sequentially addressed elements
        /// </summary>
        /// <param name="indices">sequential indices, arbitrary size</param>
        /// <returns>new storage, type of this storage, size and shape of indices</returns>
        private ILDenseStorage<ElementType> CreateSubarrayStorageSequential(ILBaseArray<float> indices) {
            if (indices.Storage is  ILDenseStorage< float > ) {
                 float [] indArr = (indices.Storage as ILDenseStorage< float >).GetArrayForRead(); 
                int outLen = indices.Size.NumberOfElements; 
                int myLen = Size.NumberOfElements; 
                ElementType[] outdata = ILMemoryPool.Pool.New<ElementType> (outLen);
                ElementType[] myData = GetArrayForRead(); 
                for (int i = 0; i < outLen; i++) {
                    int targetIndex = (int)indArr[i]; 
                    if (targetIndex < 0 || targetIndex >= myLen)
                        throw new ILArgumentException(String.Format("sequential index at position {0} out of range: {1}", i, targetIndex)); 
                    outdata[i] = myData[targetIndex]; 
                }
                return CreateSelf(outdata, indices.Size);
            } else {
                throw new ILArgumentException("unsupported storage type for indices"); 
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

       
        /// <summary>
        /// create column vector of all this array elements
        /// </summary>
        /// <returns>new storage, type of this storage, size and shape of indices</returns>
        private ILDenseStorage<ElementType> CreateSubarrayStorageFull( ) {
            return CreateSelf(Data,new ILSize(Size.NumberOfElements,1)); 
        }

      
        /// <summary>
        /// create subarray from ILDenseStorage 
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        private ILDenseStorage<ElementType> CreateSubarrayStorage( ILRange range ) {
            using (range) {
                if (range == null || range.Size.NumberOfElements == 0)
                    return CreateSelf(ILSize.Empty00);
                int rangeDimLen = range.RangeArray.Length, higherDimSum = 0;
                int leadDimLenRange = range[0].Count, leadDimLen = m_size[0];
                int curPosOut = 0, d, outElemCount = range.Size.NumberOfElements;
                ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(outElemCount);
                ElementType[] myArr = GetArrayForRead();
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
                
                while (true) {
                    // copy along leading dimension
                    int rng0Count = rng[0].Count; int[] rng0 = rng[0].GetArray();  
                    for (int i = 0; i < rng0Count; i++) {
                        int rng0i = rng0[i]; 
                        if (rng0i < 0) {
                            if (-rng0i > Settings.s_minElementLength4SystemArrayCopy) {
                                //int endOut = curPosOut -(rng0i - 1), startIn = higherDimSum;
                                //for (; curPosOut < retArr.Length; curPosOut++) {
                                //    if (curPosOut >= endOut) break; 
                                //    retArr[curPosOut] = myArr[startIn++];
                                //}
                                System.Array.Copy(myArr, higherDimSum, retArr, curPosOut, -(rng0i - 1));
                                curPosOut -= (rng0i - 1);
                            } else {
                                //for (int c = -rng0[i]; c-- >= 0; ) {
                                //    retArr[curPosOut++] = myArr[higherDimSum++];
                                //}
                                //higherDimSum += (rng0[i] - 1); // negative value in rng!  
                                int endOut = curPosOut - (rng0i - 1), startIn = higherDimSum;
                                for (; curPosOut < outElemCount; curPosOut++) {
                                    if (curPosOut >= endOut) break;
                                    retArr[curPosOut] = myArr[startIn++];
                                }
                            }
                        } else {
                            retArr[curPosOut++] = myArr[higherDimSum + rng0i];
                        }
                    }

                    // increase higher dims 
                    d = 1;
                    while (d < rangeDimLen) {
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
                            if (rng[d][0] < 0) {
                                inFullDim[d] = rng[d][0];
                            } else {
                                higherDimSum += seqDistances[d] * rng[d][0];
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
                return CreateSelf(retArr, range.Size);
            }
        }
        /// <summary>
        /// create new storage, shift dimensions 
        /// </summary>
        /// <param name="shift">number of dimensions to shift</param>
        /// <returns>shifted storage </returns>
        private ILDenseStorage<ElementType> CreateShiftedStorage(int shift) {
            ILSize retDimensions = m_size.GetShifted(shift); 
            int numElem = m_size.NumberOfElements - 1;
            ElementType[] retArr = ILMemoryPool.Pool.New<ElementType>(retDimensions.NumberOfElements);
            ElementType[] myArr = GetArrayForRead();
            int inc = m_size.SequentialIndexDistance(Math.Min(shift, m_size.NumberOfDimensions));
            int pos = 0;
            int i = 0;
            while (i < numElem) {
                retArr[i++] = myArr[pos];
                pos = (pos + inc) % numElem;
            }
            if (numElem >= 0) // except empty 
                retArr[i] = myArr[numElem];
            return CreateSelf(retArr,retDimensions);
        }

        protected virtual ILDenseStorage<ElementType> CreateSelf(ElementType[] data, ILSize size) {
            return new ILDenseStorage<ElementType>(data, size);
        }
        protected virtual ILDenseStorage<ElementType> CreateSelf(ILSize size) {
            return new ILDenseStorage<ElementType>(size);
        }
        protected virtual ILDenseStorage<ElementType> CreateSelf(ILCountableArray<ElementType> Data, ILSize iLDimension) {
            return new ILDenseStorage<ElementType>(Data, iLDimension); 
        }

        #endregion

    }
}
