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
using System.Threading; 
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Exceptions;
using System.Linq;


namespace ILNumerics  {
    public partial class ILMath {

        /// <summary>
        /// Calculate median along the specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the median of 
        /// all elements along that dimension.</para>
        /// <param name="dim">[Optional] Index of dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <para>The result will have the same number of dimensions as A, but the specified dimension will have the 
        /// size 1.</para><para>If the specified dimension of A is empty, the median along that dimension will be NaN and that dimension will be 1.</para></returns>
        public static ILRetArray<double>  median(ILInArray<double> A, int dim = -1)
        {
           
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return   A.C;
               
                if (A.IsScalar) {
                    
                    return array<double>(new double[] { A.GetValue(0) }, 1, 1);
                }

                if (A.S[dim] == 1)
                    return  A.C;

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1;
                ILSize retDimension = new ILSize(newDims);

                if (A.IsEmpty)
                    return array<double>(double.NaN,retDimension);

                 double[] retArr = ILMemoryPool.Pool.New< double>(retDimension.NumberOfElements);

                int maxRuns = retDimension.NumberOfElements;
                
                double[] aArray = null;
                if (maxRuns == 1) {
                    // ho: easier would be to use a new local array: 
                    // ILArray<double> aClone = A.C; 
                    // aArray = aClone.GetArrayForWrite();  here... (but the implemented way works just as well) 
                    aArray = ILMemoryPool.Pool.New< double>(A.S[dim]);
                    A.ExportValues(ref aArray);
                    retArr[0] = median_worker(aArray, A.S[dim]);
                    ILMemoryPool.Pool.Free(aArray);
                } else
                {
                    #region may run parallel
                    aArray = A.GetArrayForRead();
                    int inc = A.Size.SequentialIndexDistance(dim);
                    int dimLen = A.Size[dim];
                    int modHelp = A.Size.NumberOfElements - 1;
                    int modOut = retDimension.NumberOfElements - 1;
                    int incOut = retDimension.SequentialIndexDistance(dim);
                    int numelA = A.S.NumberOfElements;
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                    if (Settings.s_maxNumberThreads > 1 && maxRuns > 1 
                        && numelA / 2 >= Settings.s_minParallelElement1Count) {

                        if (maxRuns >= Settings.s_maxNumberThreads
                            && numelA / Settings.s_maxNumberThreads > Settings.s_minParallelElement1Count) {
                            workItemLength = maxRuns / workItemCount;
                        } else {
                            workItemLength = maxRuns / 2;
                            workItemCount = 2;
                        }
                        
                    } else {
                        workItemLength = maxRuns;
                        workItemCount = 1;
                    }
                    Action<object> action = (data) => {
                        Tuple<int, int> range = (Tuple<int, int>)data;
                        int from = range.Item1, to = range.Item2;
                        
                        double[] tmpArr = ILMemoryPool.Pool.New< double>(dimLen);
                        for (int c = from; c < to; c++)
                        {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);

                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                            int end = pos + dimLen * inc;
                            int k = 0;
                            // ho: TODO: probably faster if operating on a full clone of A. median_worker would handle LDA, start, end than ... 
                            for (int j = pos; j < end; j += inc)
                                tmpArr[k++] = aArray[j];
                            retArr[posOut] = median_worker(tmpArr, dimLen);
                        }
                        ILMemoryPool.Pool.Free(tmpArr); 
                        System.Threading.Interlocked.Decrement(ref workerCount);
                    };
                    for (; i < workItemCount - 1; i++) {
                        Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,action, Tuple.Create(i * workItemLength, (i + 1) * workItemLength));
                    }
                    action(Tuple.Create(i * workItemLength, maxRuns));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                    #endregion
                }
                return array(retArr, newDims);
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Calculate median along the specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the median of 
        /// all elements along that dimension.</para>
        /// <param name="dim">[Optional] Index of dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <para>The result will have the same number of dimensions as A, but the specified dimension will have the 
        /// size 1.</para><para>If the specified dimension of A is empty, the median along that dimension will be NaN and that dimension will be 1.</para></returns>
        public static ILRetArray<double>  median(ILInArray<Int64> A, int dim = -1)
        {
           
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return  todouble(A.C);
               
                if (A.IsScalar) {
                    
                    return array<double>(new double[] { A.GetValue(0) }, 1, 1);
                }

                if (A.S[dim] == 1)
                    return  todouble(A.C);

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1;
                ILSize retDimension = new ILSize(newDims);

                if (A.IsEmpty)
                    return array<double>(double.NaN,retDimension);

                double[] retArr = ILMemoryPool.Pool.New< double>(retDimension.NumberOfElements);

                int maxRuns = retDimension.NumberOfElements;
               
                Int64[] aArray = null;
                if (maxRuns == 1) {
                    // ho: easier would be to use a new local array: 
                    // ILArray<double> aClone = A.C; 
                    // aArray = aClone.GetArrayForWrite();  here... (but the implemented way works just as well) 
                    aArray = ILMemoryPool.Pool.New< Int64>(A.S[dim]);
                    A.ExportValues(ref aArray);
                    retArr[0] = median_worker(aArray, A.S[dim]);
                    ILMemoryPool.Pool.Free(aArray);
                } else
                {
                    #region may run parallel
                    aArray = A.GetArrayForRead();
                    int inc = A.Size.SequentialIndexDistance(dim);
                    int dimLen = A.Size[dim];
                    int modHelp = A.Size.NumberOfElements - 1;
                    int modOut = retDimension.NumberOfElements - 1;
                    int incOut = retDimension.SequentialIndexDistance(dim);
                    int numelA = A.S.NumberOfElements;
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                    if (Settings.s_maxNumberThreads > 1 && maxRuns > 1 
                        && numelA / 2 >= Settings.s_minParallelElement1Count) {

                        if (maxRuns >= Settings.s_maxNumberThreads
                            && numelA / Settings.s_maxNumberThreads > Settings.s_minParallelElement1Count) {
                            workItemLength = maxRuns / workItemCount;
                        } else {
                            workItemLength = maxRuns / 2;
                            workItemCount = 2;
                        }
                        
                    } else {
                        workItemLength = maxRuns;
                        workItemCount = 1;
                    }
                    Action<object> action = (data) => {
                        Tuple<int, int> range = (Tuple<int, int>)data;
                        int from = range.Item1, to = range.Item2;
                       
                        Int64[] tmpArr = ILMemoryPool.Pool.New< Int64>(dimLen);
                        for (int c = from; c < to; c++)
                        {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);

                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                            int end = pos + dimLen * inc;
                            int k = 0;
                            // ho: TODO: probably faster if operating on a full clone of A. median_worker would handle LDA, start, end than ... 
                            for (int j = pos; j < end; j += inc)
                                tmpArr[k++] = aArray[j];
                            retArr[posOut] = median_worker(tmpArr, dimLen);
                        }
                        ILMemoryPool.Pool.Free(tmpArr); 
                        System.Threading.Interlocked.Decrement(ref workerCount);
                    };
                    for (; i < workItemCount - 1; i++) {
                        Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,action, Tuple.Create(i * workItemLength, (i + 1) * workItemLength));
                    }
                    action(Tuple.Create(i * workItemLength, maxRuns));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                    #endregion
                }
                return array(retArr, newDims);
            }
        }
        /// <summary>
        /// Calculate median along the specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the median of 
        /// all elements along that dimension.</para>
        /// <param name="dim">[Optional] Index of dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <para>The result will have the same number of dimensions as A, but the specified dimension will have the 
        /// size 1.</para><para>If the specified dimension of A is empty, the median along that dimension will be NaN and that dimension will be 1.</para></returns>
        public static ILRetArray<double>  median(ILInArray<Int32> A, int dim = -1)
        {
           
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return  todouble(A.C);
               
                if (A.IsScalar) {
                    
                    return array<double>(new double[] { A.GetValue(0) }, 1, 1);
                }

                if (A.S[dim] == 1)
                    return  todouble(A.C);

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1;
                ILSize retDimension = new ILSize(newDims);

                if (A.IsEmpty)
                    return array<double>(double.NaN,retDimension);

                double[] retArr = ILMemoryPool.Pool.New< double>(retDimension.NumberOfElements);

                int maxRuns = retDimension.NumberOfElements;
               
                Int32[] aArray = null;
                if (maxRuns == 1) {
                    // ho: easier would be to use a new local array: 
                    // ILArray<double> aClone = A.C; 
                    // aArray = aClone.GetArrayForWrite();  here... (but the implemented way works just as well) 
                    aArray = ILMemoryPool.Pool.New< Int32>(A.S[dim]);
                    A.ExportValues(ref aArray);
                    retArr[0] = median_worker(aArray, A.S[dim]);
                    ILMemoryPool.Pool.Free(aArray);
                } else
                {
                    #region may run parallel
                    aArray = A.GetArrayForRead();
                    int inc = A.Size.SequentialIndexDistance(dim);
                    int dimLen = A.Size[dim];
                    int modHelp = A.Size.NumberOfElements - 1;
                    int modOut = retDimension.NumberOfElements - 1;
                    int incOut = retDimension.SequentialIndexDistance(dim);
                    int numelA = A.S.NumberOfElements;
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                    if (Settings.s_maxNumberThreads > 1 && maxRuns > 1 
                        && numelA / 2 >= Settings.s_minParallelElement1Count) {

                        if (maxRuns >= Settings.s_maxNumberThreads
                            && numelA / Settings.s_maxNumberThreads > Settings.s_minParallelElement1Count) {
                            workItemLength = maxRuns / workItemCount;
                        } else {
                            workItemLength = maxRuns / 2;
                            workItemCount = 2;
                        }
                        
                    } else {
                        workItemLength = maxRuns;
                        workItemCount = 1;
                    }
                    Action<object> action = (data) => {
                        Tuple<int, int> range = (Tuple<int, int>)data;
                        int from = range.Item1, to = range.Item2;
                       
                        Int32[] tmpArr = ILMemoryPool.Pool.New< Int32>(dimLen);
                        for (int c = from; c < to; c++)
                        {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);

                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                            int end = pos + dimLen * inc;
                            int k = 0;
                            // ho: TODO: probably faster if operating on a full clone of A. median_worker would handle LDA, start, end than ... 
                            for (int j = pos; j < end; j += inc)
                                tmpArr[k++] = aArray[j];
                            retArr[posOut] = median_worker(tmpArr, dimLen);
                        }
                        ILMemoryPool.Pool.Free(tmpArr); 
                        System.Threading.Interlocked.Decrement(ref workerCount);
                    };
                    for (; i < workItemCount - 1; i++) {
                        Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,action, Tuple.Create(i * workItemLength, (i + 1) * workItemLength));
                    }
                    action(Tuple.Create(i * workItemLength, maxRuns));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                    #endregion
                }
                return array(retArr, newDims);
            }
        }
        /// <summary>
        /// Calculate median along the specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the median of 
        /// all elements along that dimension.</para>
        /// <param name="dim">[Optional] Index of dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <para>The result will have the same number of dimensions as A, but the specified dimension will have the 
        /// size 1.</para><para>If the specified dimension of A is empty, the median along that dimension will be NaN and that dimension will be 1.</para></returns>
        public static ILRetArray<double>  median(ILInArray<byte> A, int dim = -1)
        {
           
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return  todouble(A.C);
               
                if (A.IsScalar) {
                    
                    return array<double>(new double[] { A.GetValue(0) }, 1, 1);
                }

                if (A.S[dim] == 1)
                    return  todouble(A.C);

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1;
                ILSize retDimension = new ILSize(newDims);

                if (A.IsEmpty)
                    return array<double>(double.NaN,retDimension);

                double[] retArr = ILMemoryPool.Pool.New< double>(retDimension.NumberOfElements);

                int maxRuns = retDimension.NumberOfElements;
               
                byte[] aArray = null;
                if (maxRuns == 1) {
                    // ho: easier would be to use a new local array: 
                    // ILArray<double> aClone = A.C; 
                    // aArray = aClone.GetArrayForWrite();  here... (but the implemented way works just as well) 
                    aArray = ILMemoryPool.Pool.New< byte>(A.S[dim]);
                    A.ExportValues(ref aArray);
                    retArr[0] = median_worker(aArray, A.S[dim]);
                    ILMemoryPool.Pool.Free(aArray);
                } else
                {
                    #region may run parallel
                    aArray = A.GetArrayForRead();
                    int inc = A.Size.SequentialIndexDistance(dim);
                    int dimLen = A.Size[dim];
                    int modHelp = A.Size.NumberOfElements - 1;
                    int modOut = retDimension.NumberOfElements - 1;
                    int incOut = retDimension.SequentialIndexDistance(dim);
                    int numelA = A.S.NumberOfElements;
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                    if (Settings.s_maxNumberThreads > 1 && maxRuns > 1 
                        && numelA / 2 >= Settings.s_minParallelElement1Count) {

                        if (maxRuns >= Settings.s_maxNumberThreads
                            && numelA / Settings.s_maxNumberThreads > Settings.s_minParallelElement1Count) {
                            workItemLength = maxRuns / workItemCount;
                        } else {
                            workItemLength = maxRuns / 2;
                            workItemCount = 2;
                        }
                        
                    } else {
                        workItemLength = maxRuns;
                        workItemCount = 1;
                    }
                    Action<object> action = (data) => {
                        Tuple<int, int> range = (Tuple<int, int>)data;
                        int from = range.Item1, to = range.Item2;
                       
                        byte[] tmpArr = ILMemoryPool.Pool.New< byte>(dimLen);
                        for (int c = from; c < to; c++)
                        {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);

                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                            int end = pos + dimLen * inc;
                            int k = 0;
                            // ho: TODO: probably faster if operating on a full clone of A. median_worker would handle LDA, start, end than ... 
                            for (int j = pos; j < end; j += inc)
                                tmpArr[k++] = aArray[j];
                            retArr[posOut] = median_worker(tmpArr, dimLen);
                        }
                        ILMemoryPool.Pool.Free(tmpArr); 
                        System.Threading.Interlocked.Decrement(ref workerCount);
                    };
                    for (; i < workItemCount - 1; i++) {
                        Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,action, Tuple.Create(i * workItemLength, (i + 1) * workItemLength));
                    }
                    action(Tuple.Create(i * workItemLength, maxRuns));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                    #endregion
                }
                return array(retArr, newDims);
            }
        }
        /// <summary>
        /// Calculate median along the specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the median of 
        /// all elements along that dimension.</para>
        /// <param name="dim">[Optional] Index of dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <para>The result will have the same number of dimensions as A, but the specified dimension will have the 
        /// size 1.</para><para>If the specified dimension of A is empty, the median along that dimension will be NaN and that dimension will be 1.</para></returns>
        public static ILRetArray<fcomplex>  median(ILInArray<fcomplex> A, int dim = -1)
        {
           
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return  A.C;
               
                if (A.IsScalar) {
                    
                    return array<fcomplex>(new fcomplex[] { A.GetValue(0) }, 1, 1);
                }

                if (A.S[dim] == 1)
                    return  A.C;

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1;
                ILSize retDimension = new ILSize(newDims);

                if (A.IsEmpty)
                    return array<fcomplex>(fcomplex.NaN,retDimension);

                fcomplex[] retArr = ILMemoryPool.Pool.New< fcomplex>(retDimension.NumberOfElements);

                int maxRuns = retDimension.NumberOfElements;
               
                fcomplex[] aArray = null;
                if (maxRuns == 1) {
                    // ho: easier would be to use a new local array: 
                    // ILArray<double> aClone = A.C; 
                    // aArray = aClone.GetArrayForWrite();  here... (but the implemented way works just as well) 
                    aArray = ILMemoryPool.Pool.New< fcomplex>(A.S[dim]);
                    A.ExportValues(ref aArray);
                    retArr[0] = median_worker(aArray, A.S[dim]);
                    ILMemoryPool.Pool.Free(aArray);
                } else
                {
                    #region may run parallel
                    aArray = A.GetArrayForRead();
                    int inc = A.Size.SequentialIndexDistance(dim);
                    int dimLen = A.Size[dim];
                    int modHelp = A.Size.NumberOfElements - 1;
                    int modOut = retDimension.NumberOfElements - 1;
                    int incOut = retDimension.SequentialIndexDistance(dim);
                    int numelA = A.S.NumberOfElements;
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                    if (Settings.s_maxNumberThreads > 1 && maxRuns > 1 
                        && numelA / 2 >= Settings.s_minParallelElement1Count) {

                        if (maxRuns >= Settings.s_maxNumberThreads
                            && numelA / Settings.s_maxNumberThreads > Settings.s_minParallelElement1Count) {
                            workItemLength = maxRuns / workItemCount;
                        } else {
                            workItemLength = maxRuns / 2;
                            workItemCount = 2;
                        }
                        
                    } else {
                        workItemLength = maxRuns;
                        workItemCount = 1;
                    }
                    Action<object> action = (data) => {
                        Tuple<int, int> range = (Tuple<int, int>)data;
                        int from = range.Item1, to = range.Item2;
                       
                        fcomplex[] tmpArr = ILMemoryPool.Pool.New< fcomplex>(dimLen);
                        for (int c = from; c < to; c++)
                        {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);

                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                            int end = pos + dimLen * inc;
                            int k = 0;
                            // ho: TODO: probably faster if operating on a full clone of A. median_worker would handle LDA, start, end than ... 
                            for (int j = pos; j < end; j += inc)
                                tmpArr[k++] = aArray[j];
                            retArr[posOut] = median_worker(tmpArr, dimLen);
                        }
                        ILMemoryPool.Pool.Free(tmpArr); 
                        System.Threading.Interlocked.Decrement(ref workerCount);
                    };
                    for (; i < workItemCount - 1; i++) {
                        Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,action, Tuple.Create(i * workItemLength, (i + 1) * workItemLength));
                    }
                    action(Tuple.Create(i * workItemLength, maxRuns));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                    #endregion
                }
                return array(retArr, newDims);
            }
        }
        /// <summary>
        /// Calculate median along the specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the median of 
        /// all elements along that dimension.</para>
        /// <param name="dim">[Optional] Index of dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <para>The result will have the same number of dimensions as A, but the specified dimension will have the 
        /// size 1.</para><para>If the specified dimension of A is empty, the median along that dimension will be NaN and that dimension will be 1.</para></returns>
        public static ILRetArray<float>  median(ILInArray<float> A, int dim = -1)
        {
           
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return  A.C;
               
                if (A.IsScalar) {
                    
                    return array<float>(new float[] { A.GetValue(0) }, 1, 1);
                }

                if (A.S[dim] == 1)
                    return  A.C;

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1;
                ILSize retDimension = new ILSize(newDims);

                if (A.IsEmpty)
                    return array<float>(float.NaN,retDimension);

                float[] retArr = ILMemoryPool.Pool.New< float>(retDimension.NumberOfElements);

                int maxRuns = retDimension.NumberOfElements;
               
                float[] aArray = null;
                if (maxRuns == 1) {
                    // ho: easier would be to use a new local array: 
                    // ILArray<double> aClone = A.C; 
                    // aArray = aClone.GetArrayForWrite();  here... (but the implemented way works just as well) 
                    aArray = ILMemoryPool.Pool.New< float>(A.S[dim]);
                    A.ExportValues(ref aArray);
                    retArr[0] = median_worker(aArray, A.S[dim]);
                    ILMemoryPool.Pool.Free(aArray);
                } else
                {
                    #region may run parallel
                    aArray = A.GetArrayForRead();
                    int inc = A.Size.SequentialIndexDistance(dim);
                    int dimLen = A.Size[dim];
                    int modHelp = A.Size.NumberOfElements - 1;
                    int modOut = retDimension.NumberOfElements - 1;
                    int incOut = retDimension.SequentialIndexDistance(dim);
                    int numelA = A.S.NumberOfElements;
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                    if (Settings.s_maxNumberThreads > 1 && maxRuns > 1 
                        && numelA / 2 >= Settings.s_minParallelElement1Count) {

                        if (maxRuns >= Settings.s_maxNumberThreads
                            && numelA / Settings.s_maxNumberThreads > Settings.s_minParallelElement1Count) {
                            workItemLength = maxRuns / workItemCount;
                        } else {
                            workItemLength = maxRuns / 2;
                            workItemCount = 2;
                        }
                        
                    } else {
                        workItemLength = maxRuns;
                        workItemCount = 1;
                    }
                    Action<object> action = (data) => {
                        Tuple<int, int> range = (Tuple<int, int>)data;
                        int from = range.Item1, to = range.Item2;
                       
                        float[] tmpArr = ILMemoryPool.Pool.New< float>(dimLen);
                        for (int c = from; c < to; c++)
                        {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);

                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                            int end = pos + dimLen * inc;
                            int k = 0;
                            // ho: TODO: probably faster if operating on a full clone of A. median_worker would handle LDA, start, end than ... 
                            for (int j = pos; j < end; j += inc)
                                tmpArr[k++] = aArray[j];
                            retArr[posOut] = median_worker(tmpArr, dimLen);
                        }
                        ILMemoryPool.Pool.Free(tmpArr); 
                        System.Threading.Interlocked.Decrement(ref workerCount);
                    };
                    for (; i < workItemCount - 1; i++) {
                        Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,action, Tuple.Create(i * workItemLength, (i + 1) * workItemLength));
                    }
                    action(Tuple.Create(i * workItemLength, maxRuns));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                    #endregion
                }
                return array(retArr, newDims);
            }
        }
        /// <summary>
        /// Calculate median along the specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the median of 
        /// all elements along that dimension.</para>
        /// <param name="dim">[Optional] Index of dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <para>The result will have the same number of dimensions as A, but the specified dimension will have the 
        /// size 1.</para><para>If the specified dimension of A is empty, the median along that dimension will be NaN and that dimension will be 1.</para></returns>
        public static ILRetArray<complex>  median(ILInArray<complex> A, int dim = -1)
        {
           
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return  A.C;
               
                if (A.IsScalar) {
                    
                    return array<complex>(new complex[] { A.GetValue(0) }, 1, 1);
                }

                if (A.S[dim] == 1)
                    return  A.C;

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1;
                ILSize retDimension = new ILSize(newDims);

                if (A.IsEmpty)
                    return array<complex>(complex.NaN,retDimension);

                complex[] retArr = ILMemoryPool.Pool.New< complex>(retDimension.NumberOfElements);

                int maxRuns = retDimension.NumberOfElements;
               
                complex[] aArray = null;
                if (maxRuns == 1) {
                    // ho: easier would be to use a new local array: 
                    // ILArray<double> aClone = A.C; 
                    // aArray = aClone.GetArrayForWrite();  here... (but the implemented way works just as well) 
                    aArray = ILMemoryPool.Pool.New< complex>(A.S[dim]);
                    A.ExportValues(ref aArray);
                    retArr[0] = median_worker(aArray, A.S[dim]);
                    ILMemoryPool.Pool.Free(aArray);
                } else
                {
                    #region may run parallel
                    aArray = A.GetArrayForRead();
                    int inc = A.Size.SequentialIndexDistance(dim);
                    int dimLen = A.Size[dim];
                    int modHelp = A.Size.NumberOfElements - 1;
                    int modOut = retDimension.NumberOfElements - 1;
                    int incOut = retDimension.SequentialIndexDistance(dim);
                    int numelA = A.S.NumberOfElements;
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                    if (Settings.s_maxNumberThreads > 1 && maxRuns > 1 
                        && numelA / 2 >= Settings.s_minParallelElement1Count) {

                        if (maxRuns >= Settings.s_maxNumberThreads
                            && numelA / Settings.s_maxNumberThreads > Settings.s_minParallelElement1Count) {
                            workItemLength = maxRuns / workItemCount;
                        } else {
                            workItemLength = maxRuns / 2;
                            workItemCount = 2;
                        }
                        
                    } else {
                        workItemLength = maxRuns;
                        workItemCount = 1;
                    }
                    Action<object> action = (data) => {
                        Tuple<int, int> range = (Tuple<int, int>)data;
                        int from = range.Item1, to = range.Item2;
                       
                        complex[] tmpArr = ILMemoryPool.Pool.New< complex>(dimLen);
                        for (int c = from; c < to; c++)
                        {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);

                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                            int end = pos + dimLen * inc;
                            int k = 0;
                            // ho: TODO: probably faster if operating on a full clone of A. median_worker would handle LDA, start, end than ... 
                            for (int j = pos; j < end; j += inc)
                                tmpArr[k++] = aArray[j];
                            retArr[posOut] = median_worker(tmpArr, dimLen);
                        }
                        ILMemoryPool.Pool.Free(tmpArr); 
                        System.Threading.Interlocked.Decrement(ref workerCount);
                    };
                    for (; i < workItemCount - 1; i++) {
                        Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,action, Tuple.Create(i * workItemLength, (i + 1) * workItemLength));
                    }
                    action(Tuple.Create(i * workItemLength, maxRuns));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                    #endregion
                }
                return array(retArr, newDims);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        #region private helpers

        static double median_worker(double[] list, int length)
        {
            // ATTENTION: list will be changed in here!!!
            if (length % 2 == 0)
            {
                // even number of elements
                int newRight;
                
                double a = (double)quickselect_worker(list, 0, length - 1, length / 2, out newRight);
                
                
                double b = (double)quickselect_worker(list, newRight + 1, length - 1, 1, out newRight);
                return b - (b - a) / (double) 2.0; // SMA: Reduces rounding errors...apparently.
            }
            else
            {
                // odd number of elements
                int dummy;
                return (double) quickselect_worker(list, 0, length - 1, (length - 1) / 2 + 1, out dummy);
            }
        }

#region HYCALPER AUTO GENERATED CODE

        static double median_worker(Int64[] list, int length)
        {
            // ATTENTION: list will be changed in here!!!
            if (length % 2 == 0)
            {
                // even number of elements
                int newRight;
               
                double a = (double)quickselect_worker(list, 0, length - 1, length / 2, out newRight);
                
               
                double b = (double)quickselect_worker(list, newRight + 1, length - 1, 1, out newRight);
                return b - (b - a) / (double) 2.0; // SMA: Reduces rounding errors...apparently.
            }
            else
            {
                // odd number of elements
                int dummy;
                return (double) quickselect_worker(list, 0, length - 1, (length - 1) / 2 + 1, out dummy);
            }
        }
        static double median_worker(Int32[] list, int length)
        {
            // ATTENTION: list will be changed in here!!!
            if (length % 2 == 0)
            {
                // even number of elements
                int newRight;
               
                double a = (double)quickselect_worker(list, 0, length - 1, length / 2, out newRight);
                
               
                double b = (double)quickselect_worker(list, newRight + 1, length - 1, 1, out newRight);
                return b - (b - a) / (double) 2.0; // SMA: Reduces rounding errors...apparently.
            }
            else
            {
                // odd number of elements
                int dummy;
                return (double) quickselect_worker(list, 0, length - 1, (length - 1) / 2 + 1, out dummy);
            }
        }
        static double median_worker(byte[] list, int length)
        {
            // ATTENTION: list will be changed in here!!!
            if (length % 2 == 0)
            {
                // even number of elements
                int newRight;
               
                double a = (double)quickselect_worker(list, 0, length - 1, length / 2, out newRight);
                
               
                double b = (double)quickselect_worker(list, newRight + 1, length - 1, 1, out newRight);
                return b - (b - a) / (double) 2.0; // SMA: Reduces rounding errors...apparently.
            }
            else
            {
                // odd number of elements
                int dummy;
                return (double) quickselect_worker(list, 0, length - 1, (length - 1) / 2 + 1, out dummy);
            }
        }
        static fcomplex median_worker(fcomplex[] list, int length)
        {
            // ATTENTION: list will be changed in here!!!
            if (length % 2 == 0)
            {
                // even number of elements
                int newRight;
               
                fcomplex a = (fcomplex)quickselect_worker(list, 0, length - 1, length / 2, out newRight);
                
               
                fcomplex b = (fcomplex)quickselect_worker(list, newRight + 1, length - 1, 1, out newRight);
                return b - (b - a) / (fcomplex) 2.0; // SMA: Reduces rounding errors...apparently.
            }
            else
            {
                // odd number of elements
                int dummy;
                return (fcomplex) quickselect_worker(list, 0, length - 1, (length - 1) / 2 + 1, out dummy);
            }
        }
        static float median_worker(float[] list, int length)
        {
            // ATTENTION: list will be changed in here!!!
            if (length % 2 == 0)
            {
                // even number of elements
                int newRight;
               
                float a = (float)quickselect_worker(list, 0, length - 1, length / 2, out newRight);
                
               
                float b = (float)quickselect_worker(list, newRight + 1, length - 1, 1, out newRight);
                return b - (b - a) / (float) 2.0; // SMA: Reduces rounding errors...apparently.
            }
            else
            {
                // odd number of elements
                int dummy;
                return (float) quickselect_worker(list, 0, length - 1, (length - 1) / 2 + 1, out dummy);
            }
        }
        static complex median_worker(complex[] list, int length)
        {
            // ATTENTION: list will be changed in here!!!
            if (length % 2 == 0)
            {
                // even number of elements
                int newRight;
               
                complex a = (complex)quickselect_worker(list, 0, length - 1, length / 2, out newRight);
                
               
                complex b = (complex)quickselect_worker(list, newRight + 1, length - 1, 1, out newRight);
                return b - (b - a) / (complex) 2.0; // SMA: Reduces rounding errors...apparently.
            }
            else
            {
                // odd number of elements
                int dummy;
                return (complex) quickselect_worker(list, 0, length - 1, (length - 1) / 2 + 1, out dummy);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
       #endregion
    }
}