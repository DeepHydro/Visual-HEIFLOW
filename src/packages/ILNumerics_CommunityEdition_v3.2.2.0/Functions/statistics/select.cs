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
 


namespace ILNumerics  {
    public partial class ILMath {

        /// <summary>
        /// Select the k-th smallest element from an array along a specific dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="k">The element to find. If k is smaller 1 or larger than the number of elements in list, the smallest/largest value will be returned.</param>
        /// <param name="dim">[Optional] Dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the value of the k-the smallest element along that dimension.</para>
        /// <para>Exception: If the selected dimension is of size 0 it will remain 0 (an empty set).</para></returns>
        public static ILRetArray<double> select(ILInArray<double> A, int k, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
               
                if (A.IsScalar) {
                    return new ILRetArray<double>(new double[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                if (A.IsEmpty)
                {
                    // If the array is empty, check whether it is empty along the chosen dimension
                    
                    if (A.S[dim] > 0)
                        // no there are potential elements in that dimension, hence we reduce it to 1
                        newDims[dim] = 1;
                    else
                        // yes, empty along chosen dimension so the result is empty, i.e. 0 elements in that dimension
                        newDims[dim] = 0;
                    return ILRetArray<double>.empty(new ILSize(newDims));
                }

                newDims[dim] = 1;

                // Check selected element and replace by useful value
                if (k < 1)
                    k = 1;
                if (k > A.S[dim])
                    k = A.S[dim];

                ILSize retDimension = new ILSize(newDims);

                 double[] retArr = ILMemoryPool.Pool.New< double>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
                if (maxRuns == 1) {
                    int dummy;
                    ILArray<double> Acopy = A.C; 
                    retArr[0] = quickselect_worker(Acopy.GetArrayForWrite(), 0, A.S[dim] - 1, k, out dummy);
                } else {
                    #region may run parallel 
                    
                    double[] aArray = A.GetArrayForRead();
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
                        for (int c = from; c < to; c++) {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);
                            int posOut = (c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                            
                            double[] tmp = ILMemoryPool.Pool.New< double>(dimLen);
                            int locPos = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc)
                                tmp[locPos++] = aArray[j];
                            int dummy;
                            retArr[posOut] = quickselect_worker(tmp, 0, dimLen - 1, k, out dummy);
                            ILMemoryPool.Pool.Free(tmp);
                        }
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
                return new ILRetArray<double>(retArr, newDims);
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Select the k-th smallest element from an array along a specific dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="k">The element to find. If k is smaller 1 or larger than the number of elements in list, the smallest/largest value will be returned.</param>
        /// <param name="dim">[Optional] Dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the value of the k-the smallest element along that dimension.</para>
        /// <para>Exception: If the selected dimension is of size 0 it will remain 0 (an empty set).</para></returns>
        public static ILRetArray<Int64> select(ILInArray<Int64> A, int k, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
               
                if (A.IsScalar) {
                    return new ILRetArray<Int64>(new Int64[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                if (A.IsEmpty)
                {
                    // If the array is empty, check whether it is empty along the chosen dimension
                    
                    if (A.S[dim] > 0)
                        // no there are potential elements in that dimension, hence we reduce it to 1
                        newDims[dim] = 1;
                    else
                        // yes, empty along chosen dimension so the result is empty, i.e. 0 elements in that dimension
                        newDims[dim] = 0;
                    return ILRetArray<Int64>.empty(new ILSize(newDims));
                }

                newDims[dim] = 1;

                // Check selected element and replace by useful value
                if (k < 1)
                    k = 1;
                if (k > A.S[dim])
                    k = A.S[dim];

                ILSize retDimension = new ILSize(newDims);

                Int64[] retArr = ILMemoryPool.Pool.New< Int64>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
                if (maxRuns == 1) {
                    int dummy;
                    ILArray<Int64> Acopy = A.C; 
                    retArr[0] = quickselect_worker(Acopy.GetArrayForWrite(), 0, A.S[dim] - 1, k, out dummy);
                } else {
                    #region may run parallel 
                   
                    Int64[] aArray = A.GetArrayForRead();
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
                        for (int c = from; c < to; c++) {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);
                            int posOut = (c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                           
                            Int64[] tmp = ILMemoryPool.Pool.New< Int64>(dimLen);
                            int locPos = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc)
                                tmp[locPos++] = aArray[j];
                            int dummy;
                            retArr[posOut] = quickselect_worker(tmp, 0, dimLen - 1, k, out dummy);
                            ILMemoryPool.Pool.Free(tmp);
                        }
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
                return new ILRetArray<Int64>(retArr, newDims);
            }
        }
        /// <summary>
        /// Select the k-th smallest element from an array along a specific dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="k">The element to find. If k is smaller 1 or larger than the number of elements in list, the smallest/largest value will be returned.</param>
        /// <param name="dim">[Optional] Dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the value of the k-the smallest element along that dimension.</para>
        /// <para>Exception: If the selected dimension is of size 0 it will remain 0 (an empty set).</para></returns>
        public static ILRetArray<Int32> select(ILInArray<Int32> A, int k, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
               
                if (A.IsScalar) {
                    return new ILRetArray<Int32>(new Int32[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                if (A.IsEmpty)
                {
                    // If the array is empty, check whether it is empty along the chosen dimension
                    
                    if (A.S[dim] > 0)
                        // no there are potential elements in that dimension, hence we reduce it to 1
                        newDims[dim] = 1;
                    else
                        // yes, empty along chosen dimension so the result is empty, i.e. 0 elements in that dimension
                        newDims[dim] = 0;
                    return ILRetArray<Int32>.empty(new ILSize(newDims));
                }

                newDims[dim] = 1;

                // Check selected element and replace by useful value
                if (k < 1)
                    k = 1;
                if (k > A.S[dim])
                    k = A.S[dim];

                ILSize retDimension = new ILSize(newDims);

                Int32[] retArr = ILMemoryPool.Pool.New< Int32>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
                if (maxRuns == 1) {
                    int dummy;
                    ILArray<Int32> Acopy = A.C; 
                    retArr[0] = quickselect_worker(Acopy.GetArrayForWrite(), 0, A.S[dim] - 1, k, out dummy);
                } else {
                    #region may run parallel 
                   
                    Int32[] aArray = A.GetArrayForRead();
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
                        for (int c = from; c < to; c++) {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);
                            int posOut = (c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                           
                            Int32[] tmp = ILMemoryPool.Pool.New< Int32>(dimLen);
                            int locPos = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc)
                                tmp[locPos++] = aArray[j];
                            int dummy;
                            retArr[posOut] = quickselect_worker(tmp, 0, dimLen - 1, k, out dummy);
                            ILMemoryPool.Pool.Free(tmp);
                        }
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
                return new ILRetArray<Int32>(retArr, newDims);
            }
        }
        /// <summary>
        /// Select the k-th smallest element from an array along a specific dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="k">The element to find. If k is smaller 1 or larger than the number of elements in list, the smallest/largest value will be returned.</param>
        /// <param name="dim">[Optional] Dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the value of the k-the smallest element along that dimension.</para>
        /// <para>Exception: If the selected dimension is of size 0 it will remain 0 (an empty set).</para></returns>
        public static ILRetArray<byte> select(ILInArray<byte> A, int k, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
               
                if (A.IsScalar) {
                    return new ILRetArray<byte>(new byte[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                if (A.IsEmpty)
                {
                    // If the array is empty, check whether it is empty along the chosen dimension
                    
                    if (A.S[dim] > 0)
                        // no there are potential elements in that dimension, hence we reduce it to 1
                        newDims[dim] = 1;
                    else
                        // yes, empty along chosen dimension so the result is empty, i.e. 0 elements in that dimension
                        newDims[dim] = 0;
                    return ILRetArray<byte>.empty(new ILSize(newDims));
                }

                newDims[dim] = 1;

                // Check selected element and replace by useful value
                if (k < 1)
                    k = 1;
                if (k > A.S[dim])
                    k = A.S[dim];

                ILSize retDimension = new ILSize(newDims);

                byte[] retArr = ILMemoryPool.Pool.New< byte>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
                if (maxRuns == 1) {
                    int dummy;
                    ILArray<byte> Acopy = A.C; 
                    retArr[0] = quickselect_worker(Acopy.GetArrayForWrite(), 0, A.S[dim] - 1, k, out dummy);
                } else {
                    #region may run parallel 
                   
                    byte[] aArray = A.GetArrayForRead();
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
                        for (int c = from; c < to; c++) {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);
                            int posOut = (c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                           
                            byte[] tmp = ILMemoryPool.Pool.New< byte>(dimLen);
                            int locPos = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc)
                                tmp[locPos++] = aArray[j];
                            int dummy;
                            retArr[posOut] = quickselect_worker(tmp, 0, dimLen - 1, k, out dummy);
                            ILMemoryPool.Pool.Free(tmp);
                        }
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
                return new ILRetArray<byte>(retArr, newDims);
            }
        }
        /// <summary>
        /// Select the k-th smallest element from an array along a specific dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="k">The element to find. If k is smaller 1 or larger than the number of elements in list, the smallest/largest value will be returned.</param>
        /// <param name="dim">[Optional] Dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the value of the k-the smallest element along that dimension.</para>
        /// <para>Exception: If the selected dimension is of size 0 it will remain 0 (an empty set).</para></returns>
        public static ILRetArray<fcomplex> select(ILInArray<fcomplex> A, int k, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
               
                if (A.IsScalar) {
                    return new ILRetArray<fcomplex>(new fcomplex[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                if (A.IsEmpty)
                {
                    // If the array is empty, check whether it is empty along the chosen dimension
                    
                    if (A.S[dim] > 0)
                        // no there are potential elements in that dimension, hence we reduce it to 1
                        newDims[dim] = 1;
                    else
                        // yes, empty along chosen dimension so the result is empty, i.e. 0 elements in that dimension
                        newDims[dim] = 0;
                    return ILRetArray<fcomplex>.empty(new ILSize(newDims));
                }

                newDims[dim] = 1;

                // Check selected element and replace by useful value
                if (k < 1)
                    k = 1;
                if (k > A.S[dim])
                    k = A.S[dim];

                ILSize retDimension = new ILSize(newDims);

                fcomplex[] retArr = ILMemoryPool.Pool.New< fcomplex>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
                if (maxRuns == 1) {
                    int dummy;
                    ILArray<fcomplex> Acopy = A.C; 
                    retArr[0] = quickselect_worker(Acopy.GetArrayForWrite(), 0, A.S[dim] - 1, k, out dummy);
                } else {
                    #region may run parallel 
                   
                    fcomplex[] aArray = A.GetArrayForRead();
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
                        for (int c = from; c < to; c++) {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);
                            int posOut = (c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                           
                            fcomplex[] tmp = ILMemoryPool.Pool.New< fcomplex>(dimLen);
                            int locPos = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc)
                                tmp[locPos++] = aArray[j];
                            int dummy;
                            retArr[posOut] = quickselect_worker(tmp, 0, dimLen - 1, k, out dummy);
                            ILMemoryPool.Pool.Free(tmp);
                        }
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
                return new ILRetArray<fcomplex>(retArr, newDims);
            }
        }
        /// <summary>
        /// Select the k-th smallest element from an array along a specific dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="k">The element to find. If k is smaller 1 or larger than the number of elements in list, the smallest/largest value will be returned.</param>
        /// <param name="dim">[Optional] Dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the value of the k-the smallest element along that dimension.</para>
        /// <para>Exception: If the selected dimension is of size 0 it will remain 0 (an empty set).</para></returns>
        public static ILRetArray<float> select(ILInArray<float> A, int k, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
               
                if (A.IsScalar) {
                    return new ILRetArray<float>(new float[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                if (A.IsEmpty)
                {
                    // If the array is empty, check whether it is empty along the chosen dimension
                    
                    if (A.S[dim] > 0)
                        // no there are potential elements in that dimension, hence we reduce it to 1
                        newDims[dim] = 1;
                    else
                        // yes, empty along chosen dimension so the result is empty, i.e. 0 elements in that dimension
                        newDims[dim] = 0;
                    return ILRetArray<float>.empty(new ILSize(newDims));
                }

                newDims[dim] = 1;

                // Check selected element and replace by useful value
                if (k < 1)
                    k = 1;
                if (k > A.S[dim])
                    k = A.S[dim];

                ILSize retDimension = new ILSize(newDims);

                float[] retArr = ILMemoryPool.Pool.New< float>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
                if (maxRuns == 1) {
                    int dummy;
                    ILArray<float> Acopy = A.C; 
                    retArr[0] = quickselect_worker(Acopy.GetArrayForWrite(), 0, A.S[dim] - 1, k, out dummy);
                } else {
                    #region may run parallel 
                   
                    float[] aArray = A.GetArrayForRead();
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
                        for (int c = from; c < to; c++) {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);
                            int posOut = (c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                           
                            float[] tmp = ILMemoryPool.Pool.New< float>(dimLen);
                            int locPos = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc)
                                tmp[locPos++] = aArray[j];
                            int dummy;
                            retArr[posOut] = quickselect_worker(tmp, 0, dimLen - 1, k, out dummy);
                            ILMemoryPool.Pool.Free(tmp);
                        }
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
                return new ILRetArray<float>(retArr, newDims);
            }
        }
        /// <summary>
        /// Select the k-th smallest element from an array along a specific dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="k">The element to find. If k is smaller 1 or larger than the number of elements in list, the smallest/largest value will be returned.</param>
        /// <param name="dim">[Optional] Dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array having the specified dimension reduced to the length 1 with the value of the k-the smallest element along that dimension.</para>
        /// <para>Exception: If the selected dimension is of size 0 it will remain 0 (an empty set).</para></returns>
        public static ILRetArray<complex> select(ILInArray<complex> A, int k, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();

                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
               
                if (A.IsScalar) {
                    return new ILRetArray<complex>(new complex[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                if (A.IsEmpty)
                {
                    // If the array is empty, check whether it is empty along the chosen dimension
                    
                    if (A.S[dim] > 0)
                        // no there are potential elements in that dimension, hence we reduce it to 1
                        newDims[dim] = 1;
                    else
                        // yes, empty along chosen dimension so the result is empty, i.e. 0 elements in that dimension
                        newDims[dim] = 0;
                    return ILRetArray<complex>.empty(new ILSize(newDims));
                }

                newDims[dim] = 1;

                // Check selected element and replace by useful value
                if (k < 1)
                    k = 1;
                if (k > A.S[dim])
                    k = A.S[dim];

                ILSize retDimension = new ILSize(newDims);

                complex[] retArr = ILMemoryPool.Pool.New< complex>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
                if (maxRuns == 1) {
                    int dummy;
                    ILArray<complex> Acopy = A.C; 
                    retArr[0] = quickselect_worker(Acopy.GetArrayForWrite(), 0, A.S[dim] - 1, k, out dummy);
                } else {
                    #region may run parallel 
                   
                    complex[] aArray = A.GetArrayForRead();
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
                        for (int c = from; c < to; c++) {
                            int pos = (int)(((long)dimLen * c * inc) % modHelp);
                            int posOut = (c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                           
                            complex[] tmp = ILMemoryPool.Pool.New< complex>(dimLen);
                            int locPos = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc)
                                tmp[locPos++] = aArray[j];
                            int dummy;
                            retArr[posOut] = quickselect_worker(tmp, 0, dimLen - 1, k, out dummy);
                            ILMemoryPool.Pool.Free(tmp);
                        }
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
                return new ILRetArray<complex>(retArr, newDims);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        #region private helpers

        private static int partition(double[] list, int left, int right, int pivotIndex)
        {
            
            double pivotValue = list[pivotIndex];
            
            double tmp = list[right];
            list[right] = list[pivotIndex];
            list[pivotIndex] = tmp; // Move pivot to end
            int storeIndex = left;
            for (int i = left; i <= right; i++)
            {
                if (list[i] < pivotValue)
                {
                    tmp = list[storeIndex];
                    list[storeIndex] = list[i];
                    list[i] = tmp;
                    storeIndex++;
                }
            }
            // Move pivot to its final place
            tmp = list[right];
            list[right] = list[storeIndex];
            list[storeIndex] = tmp;

            return storeIndex;
        }

#region HYCALPER AUTO GENERATED CODE

        private static int partition(Int64[] list, int left, int right, int pivotIndex)
        {
           
            Int64 pivotValue = list[pivotIndex];
           
            Int64 tmp = list[right];
            list[right] = list[pivotIndex];
            list[pivotIndex] = tmp; // Move pivot to end
            int storeIndex = left;
            for (int i = left; i <= right; i++)
            {
                if (list[i] < pivotValue)
                {
                    tmp = list[storeIndex];
                    list[storeIndex] = list[i];
                    list[i] = tmp;
                    storeIndex++;
                }
            }
            // Move pivot to its final place
            tmp = list[right];
            list[right] = list[storeIndex];
            list[storeIndex] = tmp;

            return storeIndex;
        }
        private static int partition(Int32[] list, int left, int right, int pivotIndex)
        {
           
            Int32 pivotValue = list[pivotIndex];
           
            Int32 tmp = list[right];
            list[right] = list[pivotIndex];
            list[pivotIndex] = tmp; // Move pivot to end
            int storeIndex = left;
            for (int i = left; i <= right; i++)
            {
                if (list[i] < pivotValue)
                {
                    tmp = list[storeIndex];
                    list[storeIndex] = list[i];
                    list[i] = tmp;
                    storeIndex++;
                }
            }
            // Move pivot to its final place
            tmp = list[right];
            list[right] = list[storeIndex];
            list[storeIndex] = tmp;

            return storeIndex;
        }
        private static int partition(byte[] list, int left, int right, int pivotIndex)
        {
           
            byte pivotValue = list[pivotIndex];
           
            byte tmp = list[right];
            list[right] = list[pivotIndex];
            list[pivotIndex] = tmp; // Move pivot to end
            int storeIndex = left;
            for (int i = left; i <= right; i++)
            {
                if (list[i] < pivotValue)
                {
                    tmp = list[storeIndex];
                    list[storeIndex] = list[i];
                    list[i] = tmp;
                    storeIndex++;
                }
            }
            // Move pivot to its final place
            tmp = list[right];
            list[right] = list[storeIndex];
            list[storeIndex] = tmp;

            return storeIndex;
        }
        private static int partition(fcomplex[] list, int left, int right, int pivotIndex)
        {
           
            fcomplex pivotValue = list[pivotIndex];
           
            fcomplex tmp = list[right];
            list[right] = list[pivotIndex];
            list[pivotIndex] = tmp; // Move pivot to end
            int storeIndex = left;
            for (int i = left; i <= right; i++)
            {
                if (list[i] < pivotValue)
                {
                    tmp = list[storeIndex];
                    list[storeIndex] = list[i];
                    list[i] = tmp;
                    storeIndex++;
                }
            }
            // Move pivot to its final place
            tmp = list[right];
            list[right] = list[storeIndex];
            list[storeIndex] = tmp;

            return storeIndex;
        }
        private static int partition(float[] list, int left, int right, int pivotIndex)
        {
           
            float pivotValue = list[pivotIndex];
           
            float tmp = list[right];
            list[right] = list[pivotIndex];
            list[pivotIndex] = tmp; // Move pivot to end
            int storeIndex = left;
            for (int i = left; i <= right; i++)
            {
                if (list[i] < pivotValue)
                {
                    tmp = list[storeIndex];
                    list[storeIndex] = list[i];
                    list[i] = tmp;
                    storeIndex++;
                }
            }
            // Move pivot to its final place
            tmp = list[right];
            list[right] = list[storeIndex];
            list[storeIndex] = tmp;

            return storeIndex;
        }
        private static int partition(complex[] list, int left, int right, int pivotIndex)
        {
           
            complex pivotValue = list[pivotIndex];
           
            complex tmp = list[right];
            list[right] = list[pivotIndex];
            list[pivotIndex] = tmp; // Move pivot to end
            int storeIndex = left;
            for (int i = left; i <= right; i++)
            {
                if (list[i] < pivotValue)
                {
                    tmp = list[storeIndex];
                    list[storeIndex] = list[i];
                    list[i] = tmp;
                    storeIndex++;
                }
            }
            // Move pivot to its final place
            tmp = list[right];
            list[right] = list[storeIndex];
            list[storeIndex] = tmp;

            return storeIndex;
        }

#endregion HYCALPER AUTO GENERATED CODE


        /// <summary>
        /// Quick select algorithm: Find the k-th smallest element in list.
        /// Will change the list parameter!
        /// </summary>
        /// <remarks><para>Elements in the array list will be reordered. Make sure to pass a copy if you intend to use that data later</para></remarks>
        /// <param name="list">The list to search in</param>
        /// <param name="left">The first index in the list to start the search</param>
        /// <param name="right">The last index in the list to end the search</param>
        /// <param name="k">The k-th smallest element to find in list[left:right]. If k is smaller than 1 or larger than the number of elements the smallest/largest value will be returned.</param>
        /// <param name="position">Returns the index in list where the smallest element was found</param>
        /// <returns>The k-th smallest element</returns>
        private static double quickselect_worker(double[] list, int left, int right, int k, out int position)
        {
            if ((left < 0) || (right > list.Length - 1))
                throw new Exception("Arguments out of range. Left and right must be within the array limits");
            position = -1;
            while (true)
            {
                if (left == right) // If the list contains only one element
                {
                    position = left;
                    return list[left];  // Return that element
                }
                // select pivotIndex between left and right
                int pivotIndex = (left + right) / 2;
                position = partition(list, left, right, pivotIndex); // = new pivot index
                int pivotDist = position - left + 1;
                // The pivot is in its final sorted position, 
                // so pivotDist reflects its 1-based position if list were sorted
                if (pivotDist == k)
                    return list[position];
                else if (k < pivotDist)
                {
                    //return quickselect(list, left, pivotNewIndex - 1, k);
                    right = position - 1;
                }
                else
                {
                    //  return quickselect(list, pivotNewIndex + 1, right, k - pivotDist);
                    left = position + 1;
                    k = k - pivotDist;
                }
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Quick select algorithm: Find the k-th smallest element in list.
        /// Will change the list parameter!
        /// </summary>
        /// <remarks><para>Elements in the array list will be reordered. Make sure to pass a copy if you intend to use that data later</para></remarks>
        /// <param name="list">The list to search in</param>
        /// <param name="left">The first index in the list to start the search</param>
        /// <param name="right">The last index in the list to end the search</param>
        /// <param name="k">The k-th smallest element to find in list[left:right]. If k is smaller than 1 or larger than the number of elements the smallest/largest value will be returned.</param>
        /// <param name="position">Returns the index in list where the smallest element was found</param>
        /// <returns>The k-th smallest element</returns>
        private static Int64 quickselect_worker(Int64[] list, int left, int right, int k, out int position)
        {
            if ((left < 0) || (right > list.Length - 1))
                throw new Exception("Arguments out of range. Left and right must be within the array limits");
            position = -1;
            while (true)
            {
                if (left == right) // If the list contains only one element
                {
                    position = left;
                    return list[left];  // Return that element
                }
                // select pivotIndex between left and right
                int pivotIndex = (left + right) / 2;
                position = partition(list, left, right, pivotIndex); // = new pivot index
                int pivotDist = position - left + 1;
                // The pivot is in its final sorted position, 
                // so pivotDist reflects its 1-based position if list were sorted
                if (pivotDist == k)
                    return list[position];
                else if (k < pivotDist)
                {
                    //return quickselect(list, left, pivotNewIndex - 1, k);
                    right = position - 1;
                }
                else
                {
                    //  return quickselect(list, pivotNewIndex + 1, right, k - pivotDist);
                    left = position + 1;
                    k = k - pivotDist;
                }
            }
        }
        /// <summary>
        /// Quick select algorithm: Find the k-th smallest element in list.
        /// Will change the list parameter!
        /// </summary>
        /// <remarks><para>Elements in the array list will be reordered. Make sure to pass a copy if you intend to use that data later</para></remarks>
        /// <param name="list">The list to search in</param>
        /// <param name="left">The first index in the list to start the search</param>
        /// <param name="right">The last index in the list to end the search</param>
        /// <param name="k">The k-th smallest element to find in list[left:right]. If k is smaller than 1 or larger than the number of elements the smallest/largest value will be returned.</param>
        /// <param name="position">Returns the index in list where the smallest element was found</param>
        /// <returns>The k-th smallest element</returns>
        private static Int32 quickselect_worker(Int32[] list, int left, int right, int k, out int position)
        {
            if ((left < 0) || (right > list.Length - 1))
                throw new Exception("Arguments out of range. Left and right must be within the array limits");
            position = -1;
            while (true)
            {
                if (left == right) // If the list contains only one element
                {
                    position = left;
                    return list[left];  // Return that element
                }
                // select pivotIndex between left and right
                int pivotIndex = (left + right) / 2;
                position = partition(list, left, right, pivotIndex); // = new pivot index
                int pivotDist = position - left + 1;
                // The pivot is in its final sorted position, 
                // so pivotDist reflects its 1-based position if list were sorted
                if (pivotDist == k)
                    return list[position];
                else if (k < pivotDist)
                {
                    //return quickselect(list, left, pivotNewIndex - 1, k);
                    right = position - 1;
                }
                else
                {
                    //  return quickselect(list, pivotNewIndex + 1, right, k - pivotDist);
                    left = position + 1;
                    k = k - pivotDist;
                }
            }
        }
        /// <summary>
        /// Quick select algorithm: Find the k-th smallest element in list.
        /// Will change the list parameter!
        /// </summary>
        /// <remarks><para>Elements in the array list will be reordered. Make sure to pass a copy if you intend to use that data later</para></remarks>
        /// <param name="list">The list to search in</param>
        /// <param name="left">The first index in the list to start the search</param>
        /// <param name="right">The last index in the list to end the search</param>
        /// <param name="k">The k-th smallest element to find in list[left:right]. If k is smaller than 1 or larger than the number of elements the smallest/largest value will be returned.</param>
        /// <param name="position">Returns the index in list where the smallest element was found</param>
        /// <returns>The k-th smallest element</returns>
        private static byte quickselect_worker(byte[] list, int left, int right, int k, out int position)
        {
            if ((left < 0) || (right > list.Length - 1))
                throw new Exception("Arguments out of range. Left and right must be within the array limits");
            position = -1;
            while (true)
            {
                if (left == right) // If the list contains only one element
                {
                    position = left;
                    return list[left];  // Return that element
                }
                // select pivotIndex between left and right
                int pivotIndex = (left + right) / 2;
                position = partition(list, left, right, pivotIndex); // = new pivot index
                int pivotDist = position - left + 1;
                // The pivot is in its final sorted position, 
                // so pivotDist reflects its 1-based position if list were sorted
                if (pivotDist == k)
                    return list[position];
                else if (k < pivotDist)
                {
                    //return quickselect(list, left, pivotNewIndex - 1, k);
                    right = position - 1;
                }
                else
                {
                    //  return quickselect(list, pivotNewIndex + 1, right, k - pivotDist);
                    left = position + 1;
                    k = k - pivotDist;
                }
            }
        }
        /// <summary>
        /// Quick select algorithm: Find the k-th smallest element in list.
        /// Will change the list parameter!
        /// </summary>
        /// <remarks><para>Elements in the array list will be reordered. Make sure to pass a copy if you intend to use that data later</para></remarks>
        /// <param name="list">The list to search in</param>
        /// <param name="left">The first index in the list to start the search</param>
        /// <param name="right">The last index in the list to end the search</param>
        /// <param name="k">The k-th smallest element to find in list[left:right]. If k is smaller than 1 or larger than the number of elements the smallest/largest value will be returned.</param>
        /// <param name="position">Returns the index in list where the smallest element was found</param>
        /// <returns>The k-th smallest element</returns>
        private static fcomplex quickselect_worker(fcomplex[] list, int left, int right, int k, out int position)
        {
            if ((left < 0) || (right > list.Length - 1))
                throw new Exception("Arguments out of range. Left and right must be within the array limits");
            position = -1;
            while (true)
            {
                if (left == right) // If the list contains only one element
                {
                    position = left;
                    return list[left];  // Return that element
                }
                // select pivotIndex between left and right
                int pivotIndex = (left + right) / 2;
                position = partition(list, left, right, pivotIndex); // = new pivot index
                int pivotDist = position - left + 1;
                // The pivot is in its final sorted position, 
                // so pivotDist reflects its 1-based position if list were sorted
                if (pivotDist == k)
                    return list[position];
                else if (k < pivotDist)
                {
                    //return quickselect(list, left, pivotNewIndex - 1, k);
                    right = position - 1;
                }
                else
                {
                    //  return quickselect(list, pivotNewIndex + 1, right, k - pivotDist);
                    left = position + 1;
                    k = k - pivotDist;
                }
            }
        }
        /// <summary>
        /// Quick select algorithm: Find the k-th smallest element in list.
        /// Will change the list parameter!
        /// </summary>
        /// <remarks><para>Elements in the array list will be reordered. Make sure to pass a copy if you intend to use that data later</para></remarks>
        /// <param name="list">The list to search in</param>
        /// <param name="left">The first index in the list to start the search</param>
        /// <param name="right">The last index in the list to end the search</param>
        /// <param name="k">The k-th smallest element to find in list[left:right]. If k is smaller than 1 or larger than the number of elements the smallest/largest value will be returned.</param>
        /// <param name="position">Returns the index in list where the smallest element was found</param>
        /// <returns>The k-th smallest element</returns>
        private static float quickselect_worker(float[] list, int left, int right, int k, out int position)
        {
            if ((left < 0) || (right > list.Length - 1))
                throw new Exception("Arguments out of range. Left and right must be within the array limits");
            position = -1;
            while (true)
            {
                if (left == right) // If the list contains only one element
                {
                    position = left;
                    return list[left];  // Return that element
                }
                // select pivotIndex between left and right
                int pivotIndex = (left + right) / 2;
                position = partition(list, left, right, pivotIndex); // = new pivot index
                int pivotDist = position - left + 1;
                // The pivot is in its final sorted position, 
                // so pivotDist reflects its 1-based position if list were sorted
                if (pivotDist == k)
                    return list[position];
                else if (k < pivotDist)
                {
                    //return quickselect(list, left, pivotNewIndex - 1, k);
                    right = position - 1;
                }
                else
                {
                    //  return quickselect(list, pivotNewIndex + 1, right, k - pivotDist);
                    left = position + 1;
                    k = k - pivotDist;
                }
            }
        }
        /// <summary>
        /// Quick select algorithm: Find the k-th smallest element in list.
        /// Will change the list parameter!
        /// </summary>
        /// <remarks><para>Elements in the array list will be reordered. Make sure to pass a copy if you intend to use that data later</para></remarks>
        /// <param name="list">The list to search in</param>
        /// <param name="left">The first index in the list to start the search</param>
        /// <param name="right">The last index in the list to end the search</param>
        /// <param name="k">The k-th smallest element to find in list[left:right]. If k is smaller than 1 or larger than the number of elements the smallest/largest value will be returned.</param>
        /// <param name="position">Returns the index in list where the smallest element was found</param>
        /// <returns>The k-th smallest element</returns>
        private static complex quickselect_worker(complex[] list, int left, int right, int k, out int position)
        {
            if ((left < 0) || (right > list.Length - 1))
                throw new Exception("Arguments out of range. Left and right must be within the array limits");
            position = -1;
            while (true)
            {
                if (left == right) // If the list contains only one element
                {
                    position = left;
                    return list[left];  // Return that element
                }
                // select pivotIndex between left and right
                int pivotIndex = (left + right) / 2;
                position = partition(list, left, right, pivotIndex); // = new pivot index
                int pivotDist = position - left + 1;
                // The pivot is in its final sorted position, 
                // so pivotDist reflects its 1-based position if list were sorted
                if (pivotDist == k)
                    return list[position];
                else if (k < pivotDist)
                {
                    //return quickselect(list, left, pivotNewIndex - 1, k);
                    right = position - 1;
                }
                else
                {
                    //  return quickselect(list, pivotNewIndex + 1, right, k - pivotDist);
                    left = position + 1;
                    k = k - pivotDist;
                }
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
       #endregion
    }
}