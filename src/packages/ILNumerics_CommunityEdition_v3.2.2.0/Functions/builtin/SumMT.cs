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
        /// Sum elements of A along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the 'dim's dimension 
        /// reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<double>  sum (ILInArray<double> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return  ILRetArray<double>.empty(A.Size);
                if (A.IsScalar) {
                    
                    return new  ILRetArray<double>(new double[] { A.GetValue(0) }, 1, 1);
                }
                
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1; 
                ILSize retDimension = new ILSize(newDims); 
                 double[] retArr = ILMemoryPool.Pool.New< double>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
                
                double[] aArray = A.GetArrayForRead();
                if (maxRuns == 1) {
                    
                    double tmp = 0;
                    for (int j = 0; j < dimLen; j++) {
                        tmp += aArray[j];
                    }
                    retArr[0] = tmp;
                } else {
                    #region may run parallel 
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
                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                            
                            double tmp = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc) {
                                tmp += aArray[j];
                            }
                            retArr[posOut] = tmp;
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
                return new  ILRetArray<double>(retArr, newDims);
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Sum elements of A along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the 'dim's dimension 
        /// reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<Int64>  sum (ILInArray<Int64> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return  ILRetArray<Int64>.empty(A.Size);
                if (A.IsScalar) {
                   
                    return new  ILRetArray<Int64>(new Int64[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1; 
                ILSize retDimension = new ILSize(newDims); 
                Int64[] retArr = ILMemoryPool.Pool.New< Int64>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
               
                Int64[] aArray = A.GetArrayForRead();
                if (maxRuns == 1) {
                   
                    Int64 tmp = 0;
                    for (int j = 0; j < dimLen; j++) {
                        tmp += aArray[j];
                    }
                    retArr[0] = tmp;
                } else {
                    #region may run parallel 
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
                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                           
                            Int64 tmp = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc) {
                                tmp += aArray[j];
                            }
                            retArr[posOut] = tmp;
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
                return new  ILRetArray<Int64>(retArr, newDims);
            }
        }
        /// <summary>
        /// Sum elements of A along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the 'dim's dimension 
        /// reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<Int32>  sum (ILInArray<Int32> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return  ILRetArray<Int32>.empty(A.Size);
                if (A.IsScalar) {
                   
                    return new  ILRetArray<Int32>(new Int32[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1; 
                ILSize retDimension = new ILSize(newDims); 
                Int32[] retArr = ILMemoryPool.Pool.New< Int32>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
               
                Int32[] aArray = A.GetArrayForRead();
                if (maxRuns == 1) {
                   
                    Int32 tmp = 0;
                    for (int j = 0; j < dimLen; j++) {
                        tmp += aArray[j];
                    }
                    retArr[0] = tmp;
                } else {
                    #region may run parallel 
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
                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                           
                            Int32 tmp = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc) {
                                tmp += aArray[j];
                            }
                            retArr[posOut] = tmp;
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
                return new  ILRetArray<Int32>(retArr, newDims);
            }
        }
        /// <summary>
        /// Sum elements of A along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the 'dim's dimension 
        /// reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<byte>  sum (ILInArray<byte> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return  ILRetArray<byte>.empty(A.Size);
                if (A.IsScalar) {
                   
                    return new  ILRetArray<byte>(new byte[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1; 
                ILSize retDimension = new ILSize(newDims); 
                byte[] retArr = ILMemoryPool.Pool.New< byte>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
               
                byte[] aArray = A.GetArrayForRead();
                if (maxRuns == 1) {
                   
                    byte tmp = 0;
                    for (int j = 0; j < dimLen; j++) {
                        tmp += aArray[j];
                    }
                    retArr[0] = tmp;
                } else {
                    #region may run parallel 
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
                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                           
                            byte tmp = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc) {
                                tmp += aArray[j];
                            }
                            retArr[posOut] = tmp;
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
                return new  ILRetArray<byte>(retArr, newDims);
            }
        }
        /// <summary>
        /// Sum elements of A along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the 'dim's dimension 
        /// reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<fcomplex>  sum (ILInArray<fcomplex> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return  ILRetArray<fcomplex>.empty(A.Size);
                if (A.IsScalar) {
                   
                    return new  ILRetArray<fcomplex>(new fcomplex[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1; 
                ILSize retDimension = new ILSize(newDims); 
                fcomplex[] retArr = ILMemoryPool.Pool.New< fcomplex>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
               
                fcomplex[] aArray = A.GetArrayForRead();
                if (maxRuns == 1) {
                   
                    fcomplex tmp = 0;
                    for (int j = 0; j < dimLen; j++) {
                        tmp += aArray[j];
                    }
                    retArr[0] = tmp;
                } else {
                    #region may run parallel 
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
                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                           
                            fcomplex tmp = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc) {
                                tmp += aArray[j];
                            }
                            retArr[posOut] = tmp;
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
                return new  ILRetArray<fcomplex>(retArr, newDims);
            }
        }
        /// <summary>
        /// Sum elements of A along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the 'dim's dimension 
        /// reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<float>  sum (ILInArray<float> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return  ILRetArray<float>.empty(A.Size);
                if (A.IsScalar) {
                   
                    return new  ILRetArray<float>(new float[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1; 
                ILSize retDimension = new ILSize(newDims); 
                float[] retArr = ILMemoryPool.Pool.New< float>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
               
                float[] aArray = A.GetArrayForRead();
                if (maxRuns == 1) {
                   
                    float tmp = 0;
                    for (int j = 0; j < dimLen; j++) {
                        tmp += aArray[j];
                    }
                    retArr[0] = tmp;
                } else {
                    #region may run parallel 
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
                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                           
                            float tmp = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc) {
                                tmp += aArray[j];
                            }
                            retArr[posOut] = tmp;
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
                return new  ILRetArray<float>(retArr, newDims);
            }
        }
        /// <summary>
        /// Sum elements of A along specified dimension
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the 'dim's dimension 
        /// reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<complex>  sum (ILInArray<complex> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return  ILRetArray<complex>.empty(A.Size);
                if (A.IsScalar) {
                   
                    return new  ILRetArray<complex>(new complex[] { A.GetValue(0) }, 1, 1);
                }
               
                if (A.S[dim] == 1) return A.C;

                int[] newDims = A.S.ToIntArray();
                newDims[dim] = 1; 
                ILSize retDimension = new ILSize(newDims); 
                complex[] retArr = ILMemoryPool.Pool.New< complex>(retDimension.NumberOfElements);

                int inc = A.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = retDimension.NumberOfElements;
                int modHelp = A.Size.NumberOfElements - 1;
                int modOut = retDimension.NumberOfElements - 1;
                int incOut = retDimension.SequentialIndexDistance(dim); 
                int numelA = A.S.NumberOfElements;
               
                complex[] aArray = A.GetArrayForRead();
                if (maxRuns == 1) {
                   
                    complex tmp = 0;
                    for (int j = 0; j < dimLen; j++) {
                        tmp += aArray[j];
                    }
                    retArr[0] = tmp;
                } else {
                    #region may run parallel 
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
                            long posOut = ((long)c * incOut);
                            if (posOut > modOut)
                                posOut = ((posOut - 1) % modOut) + 1;
                           
                            complex tmp = 0;
                            int end = pos + dimLen * inc;
                            for (int j = pos; j < end; j += inc) {
                                tmp += aArray[j];
                            }
                            retArr[posOut] = tmp;
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
                return new  ILRetArray<complex>(retArr, newDims);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}