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
using ILNumerics.Exceptions;



namespace ILNumerics {

    public partial class ILMath {



        /// <summary>
        /// pairwise L1 distance 
        /// </summary>
        /// <param name="A">input points (matrix)</param>
        /// <param name="B">input point (vector)</param>
        /// <returns>pairwise L1 distances between the data point provided in the input vector <paramref name="B"/> and the data points stored in the matrix <paramref name="A"/>.</returns>
        /// <remarks>
        /// <para>If <paramref name="B"/> is a colum vector, the distances between <paramref name="B"/> and the columns of <paramref name="A"/> are calculated. The number of rows of <paramref name="A"/> 
        /// must match the length of vector <paramref name="B"/> than. Therefore, the length of the returned vector of distances matches the number of columns of <paramref name="A"/>: <code>A.S[1]</code>.</para>
        /// <para>If <paramref name="B"/> is a row vector, the distances between <paramref name="B"/> and the rows of <paramref name="A"/> are calculated. The number of columns of <paramref name="A"/> 
        /// must match the length of vector <paramref name="B"/> than. Therefore, the length of the returned vector of distances matches the number of rows of <paramref name="A"/>: <code>A.S[0]</code>.</para>
        /// <para>This function is cummulative, the single data point may be provided in <paramref name="A"/> and the data point matrix may be provided in <paramref name="B"/> as well.</para>
        /// </remarks>
        public static unsafe ILRetArray<double> distL1(ILInArray<double> A, ILInArray<double> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<double>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<double>(B.S);
                } else if (B.IsEmpty) {
                    return empty<double>(A.S);
                }
                // early exit: make the function cummutative
                if (A.IsVector && !B.IsVector) {
                    return distL1(B, A); 
                } 
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
                ILSize outDims; 
                if (dim == 0) {
                    outDims = size(1,A.S[1]);    
                } else if (dim == 1) {
                    outDims = size(A.S[0], 1);
                } else if (A.S.IsSameSize(B.S)) {
                    outDims = A.S;
                    dim = 0; 
                } else 
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");

                
                double[] retArr = ILMemoryPool.Pool.New<double>(outDims.NumberOfElements);

                
                double[] arrA = A.GetArrayForRead();

                
                double[] arrB = B.GetArrayForRead();
                int itLen = A.S[0]; 
                #endregion

                #region worker loops definition
                ILDenseStorage<double> retStorage = new ILDenseStorage<double>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;

                    
                    double* ap;

                    
                    double* bp;

                    
                    double* cp;
                    if (dim == 0) {
                        ap = (double*)range.Item2;
                        cp = (double*)range.Item4;
                        for (int s = 0; s < range.Item1; s++) {
                            bp = (double*)range.Item3;
                            
                            double sum = 0; 
                            int l = itLen;
                            while (l > 20) {
                                sum +=  Math.Abs(ap[0] - bp[0])
                                    +   Math.Abs(ap[1] - bp[1])
                                    +   Math.Abs(ap[2] - bp[2])
                                    +   Math.Abs(ap[3] - bp[3])
                                    +   Math.Abs(ap[4] - bp[4])
                                    +   Math.Abs(ap[5] - bp[5])
                                    +   Math.Abs(ap[6] - bp[6])
                                    +   Math.Abs(ap[7] - bp[7])
                                    +   Math.Abs(ap[8] - bp[8])
                                    +   Math.Abs(ap[9] - bp[9])
                                    +   Math.Abs(ap[10] - bp[10])
                                    +   Math.Abs(ap[11] - bp[11])
                                    +   Math.Abs(ap[12] - bp[12])
                                    +   Math.Abs(ap[13] - bp[13])
                                    +   Math.Abs(ap[14] - bp[14])
                                    +   Math.Abs(ap[15] - bp[15])
                                    +   Math.Abs(ap[16] - bp[16])
                                    +   Math.Abs(ap[17] - bp[17])
                                    +   Math.Abs(ap[18] - bp[18])
                                    +   Math.Abs(ap[19] - bp[19])
                                    +   Math.Abs(ap[20] - bp[20]);
                                ap += 21;
                                bp += 21;
                                l -= 21;
                            }
                            while (l-- > 0) {
                                sum +=  Math.Abs(*ap - *bp);
                                ap++;
                                bp++;
                            }
                            *cp++ = sum; 
                        }
                    } else {
                        // dim = 1
                        ap = (double*)range.Item2;
                        bp = (double*)range.Item3;
                        cp = (double*)range.Item4;
                        for (int s = 0; s < range.Item1; s++) { // TODO: further potential speedup: unroll in s direction!
                            
                            double val = *bp++;
                            int l = itLen;
                            while (l > 10) {
                                cp[0] +=   Math.Abs(ap[0] - val);
                                cp[1] +=   Math.Abs(ap[1] - val);
                                cp[2] +=   Math.Abs(ap[2] - val);
                                cp[3] +=   Math.Abs(ap[3] - val);
                                cp[4] +=   Math.Abs(ap[4] - val);
                                cp[5] +=   Math.Abs(ap[5] - val);
                                cp[6] +=   Math.Abs(ap[6] - val);
                                cp[7] +=   Math.Abs(ap[7] - val);
                                cp[8] +=   Math.Abs(ap[8] - val);
                                cp[9] +=   Math.Abs(ap[9] - val);
                                cp[10] +=  Math.Abs(ap[10] - val);
                                cp[11] +=  Math.Abs(ap[11] - val);
                                cp[12] +=  Math.Abs(ap[12] - val);
                                cp[13] +=  Math.Abs(ap[13] - val);
                                cp[14] +=  Math.Abs(ap[14] - val);
                                cp[15] +=  Math.Abs(ap[15] - val);
                                cp[16] +=  Math.Abs(ap[16] - val);
                                cp[17] +=  Math.Abs(ap[17] - val);
                                cp[18] +=  Math.Abs(ap[18] - val);
                                cp[19] +=  Math.Abs(ap[19] - val);
                                cp[20] +=  Math.Abs(ap[20] - val);
                                ap += 21;
                                cp += 21;
                                l -= 21;
                            }
                            while (l-- > 0) {
                                *cp +=  Math.Abs(*ap - val);
                                ap++;
                                cp++;
                            }
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                //int[] partResults = new int[workItemCount]; 
                fixed ( double* arrAP = arrA)
                fixed ( double* arrBP = arrB)
                fixed ( double* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * outDims[0] * workItemLength)
                               , (IntPtr)(arrBP + i * dim * workItemLength)
                               , (IntPtr)(retArrP + i * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * outDims[0] * workItemLength)
                                , (IntPtr)(arrBP + i * dim * workItemLength)
                                , (IntPtr)(retArrP + i * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<double>(retStorage);
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// pairwise L1 distance 
        /// </summary>
        /// <param name="A">input points (matrix)</param>
        /// <param name="B">input point (vector)</param>
        /// <returns>pairwise L1 distances between the data point provided in the input vector <paramref name="B"/> and the data points stored in the matrix <paramref name="A"/>.</returns>
        /// <remarks>
        /// <para>If <paramref name="B"/> is a colum vector, the distances between <paramref name="B"/> and the columns of <paramref name="A"/> are calculated. The number of rows of <paramref name="A"/> 
        /// must match the length of vector <paramref name="B"/> than. Therefore, the length of the returned vector of distances matches the number of columns of <paramref name="A"/>: <code>A.S[1]</code>.</para>
        /// <para>If <paramref name="B"/> is a row vector, the distances between <paramref name="B"/> and the rows of <paramref name="A"/> are calculated. The number of columns of <paramref name="A"/> 
        /// must match the length of vector <paramref name="B"/> than. Therefore, the length of the returned vector of distances matches the number of rows of <paramref name="A"/>: <code>A.S[0]</code>.</para>
        /// <para>This function is cummulative, the single data point may be provided in <paramref name="A"/> and the data point matrix may be provided in <paramref name="B"/> as well.</para>
        /// </remarks>
        public static unsafe ILRetArray<Int64> distL1(ILInArray<Int64> A, ILInArray<Int64> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<Int64>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<Int64>(B.S);
                } else if (B.IsEmpty) {
                    return empty<Int64>(A.S);
                }
                // early exit: make the function cummutative
                if (A.IsVector && !B.IsVector) {
                    return distL1(B, A); 
                } 
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
                ILSize outDims; 
                if (dim == 0) {
                    outDims = size(1,A.S[1]);    
                } else if (dim == 1) {
                    outDims = size(A.S[0], 1);
                } else if (A.S.IsSameSize(B.S)) {
                    outDims = A.S;
                    dim = 0; 
                } else 
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");

               
                Int64[] retArr = ILMemoryPool.Pool.New<Int64>(outDims.NumberOfElements);

               
                Int64[] arrA = A.GetArrayForRead();

               
                Int64[] arrB = B.GetArrayForRead();
                int itLen = A.S[0]; 
                #endregion

                #region worker loops definition
                ILDenseStorage<Int64> retStorage = new ILDenseStorage<Int64>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;

                   
                    Int64* ap;

                   
                    Int64* bp;

                   
                    Int64* cp;
                    if (dim == 0) {
                        ap = (Int64*)range.Item2;
                        cp = (Int64*)range.Item4;
                        for (int s = 0; s < range.Item1; s++) {
                            bp = (Int64*)range.Item3;
                           
                            Int64 sum = 0; 
                            int l = itLen;
                            while (l > 20) {
                                sum +=  Math.Abs(ap[0] - bp[0])
                                    +  Math.Abs(ap[1] - bp[1])
                                    +  Math.Abs(ap[2] - bp[2])
                                    +  Math.Abs(ap[3] - bp[3])
                                    +  Math.Abs(ap[4] - bp[4])
                                    +  Math.Abs(ap[5] - bp[5])
                                    +  Math.Abs(ap[6] - bp[6])
                                    +  Math.Abs(ap[7] - bp[7])
                                    +  Math.Abs(ap[8] - bp[8])
                                    +  Math.Abs(ap[9] - bp[9])
                                    +  Math.Abs(ap[10] - bp[10])
                                    +  Math.Abs(ap[11] - bp[11])
                                    +  Math.Abs(ap[12] - bp[12])
                                    +  Math.Abs(ap[13] - bp[13])
                                    +  Math.Abs(ap[14] - bp[14])
                                    +  Math.Abs(ap[15] - bp[15])
                                    +  Math.Abs(ap[16] - bp[16])
                                    +  Math.Abs(ap[17] - bp[17])
                                    +  Math.Abs(ap[18] - bp[18])
                                    +  Math.Abs(ap[19] - bp[19])
                                    +  Math.Abs(ap[20] - bp[20]);
                                ap += 21;
                                bp += 21;
                                l -= 21;
                            }
                            while (l-- > 0) {
                                sum +=  Math.Abs(*ap - *bp);
                                ap++;
                                bp++;
                            }
                            *cp++ = sum; 
                        }
                    } else {
                        // dim = 1
                        ap = (Int64*)range.Item2;
                        bp = (Int64*)range.Item3;
                        cp = (Int64*)range.Item4;
                        for (int s = 0; s < range.Item1; s++) { // TODO: further potential speedup: unroll in s direction!
                           
                            Int64 val = *bp++;
                            int l = itLen;
                            while (l > 10) {
                                cp[0] +=  Math.Abs(ap[0] - val);
                                cp[1] +=  Math.Abs(ap[1] - val);
                                cp[2] +=  Math.Abs(ap[2] - val);
                                cp[3] +=  Math.Abs(ap[3] - val);
                                cp[4] +=  Math.Abs(ap[4] - val);
                                cp[5] +=  Math.Abs(ap[5] - val);
                                cp[6] +=  Math.Abs(ap[6] - val);
                                cp[7] +=  Math.Abs(ap[7] - val);
                                cp[8] +=  Math.Abs(ap[8] - val);
                                cp[9] +=  Math.Abs(ap[9] - val);
                                cp[10] +=  Math.Abs(ap[10] - val);
                                cp[11] +=  Math.Abs(ap[11] - val);
                                cp[12] +=  Math.Abs(ap[12] - val);
                                cp[13] +=  Math.Abs(ap[13] - val);
                                cp[14] +=  Math.Abs(ap[14] - val);
                                cp[15] +=  Math.Abs(ap[15] - val);
                                cp[16] +=  Math.Abs(ap[16] - val);
                                cp[17] +=  Math.Abs(ap[17] - val);
                                cp[18] +=  Math.Abs(ap[18] - val);
                                cp[19] +=  Math.Abs(ap[19] - val);
                                cp[20] +=  Math.Abs(ap[20] - val);
                                ap += 21;
                                cp += 21;
                                l -= 21;
                            }
                            while (l-- > 0) {
                                *cp +=  Math.Abs(*ap - val);
                                ap++;
                                cp++;
                            }
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                //int[] partResults = new int[workItemCount]; 
                fixed ( Int64* arrAP = arrA)
                fixed ( Int64* arrBP = arrB)
                fixed ( Int64* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * outDims[0] * workItemLength)
                               , (IntPtr)(arrBP + i * dim * workItemLength)
                               , (IntPtr)(retArrP + i * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * outDims[0] * workItemLength)
                                , (IntPtr)(arrBP + i * dim * workItemLength)
                                , (IntPtr)(retArrP + i * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<Int64>(retStorage);
            }
        }
        /// <summary>
        /// pairwise L1 distance 
        /// </summary>
        /// <param name="A">input points (matrix)</param>
        /// <param name="B">input point (vector)</param>
        /// <returns>pairwise L1 distances between the data point provided in the input vector <paramref name="B"/> and the data points stored in the matrix <paramref name="A"/>.</returns>
        /// <remarks>
        /// <para>If <paramref name="B"/> is a colum vector, the distances between <paramref name="B"/> and the columns of <paramref name="A"/> are calculated. The number of rows of <paramref name="A"/> 
        /// must match the length of vector <paramref name="B"/> than. Therefore, the length of the returned vector of distances matches the number of columns of <paramref name="A"/>: <code>A.S[1]</code>.</para>
        /// <para>If <paramref name="B"/> is a row vector, the distances between <paramref name="B"/> and the rows of <paramref name="A"/> are calculated. The number of columns of <paramref name="A"/> 
        /// must match the length of vector <paramref name="B"/> than. Therefore, the length of the returned vector of distances matches the number of rows of <paramref name="A"/>: <code>A.S[0]</code>.</para>
        /// <para>This function is cummulative, the single data point may be provided in <paramref name="A"/> and the data point matrix may be provided in <paramref name="B"/> as well.</para>
        /// </remarks>
        public static unsafe ILRetArray<Int32> distL1(ILInArray<Int32> A, ILInArray<Int32> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<Int32>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<Int32>(B.S);
                } else if (B.IsEmpty) {
                    return empty<Int32>(A.S);
                }
                // early exit: make the function cummutative
                if (A.IsVector && !B.IsVector) {
                    return distL1(B, A); 
                } 
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
                ILSize outDims; 
                if (dim == 0) {
                    outDims = size(1,A.S[1]);    
                } else if (dim == 1) {
                    outDims = size(A.S[0], 1);
                } else if (A.S.IsSameSize(B.S)) {
                    outDims = A.S;
                    dim = 0; 
                } else 
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");

               
                Int32[] retArr = ILMemoryPool.Pool.New<Int32>(outDims.NumberOfElements);

               
                Int32[] arrA = A.GetArrayForRead();

               
                Int32[] arrB = B.GetArrayForRead();
                int itLen = A.S[0]; 
                #endregion

                #region worker loops definition
                ILDenseStorage<Int32> retStorage = new ILDenseStorage<Int32>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;

                   
                    Int32* ap;

                   
                    Int32* bp;

                   
                    Int32* cp;
                    if (dim == 0) {
                        ap = (Int32*)range.Item2;
                        cp = (Int32*)range.Item4;
                        for (int s = 0; s < range.Item1; s++) {
                            bp = (Int32*)range.Item3;
                           
                            Int32 sum = 0; 
                            int l = itLen;
                            while (l > 20) {
                                sum +=  Math.Abs(ap[0] - bp[0])
                                    +  Math.Abs(ap[1] - bp[1])
                                    +  Math.Abs(ap[2] - bp[2])
                                    +  Math.Abs(ap[3] - bp[3])
                                    +  Math.Abs(ap[4] - bp[4])
                                    +  Math.Abs(ap[5] - bp[5])
                                    +  Math.Abs(ap[6] - bp[6])
                                    +  Math.Abs(ap[7] - bp[7])
                                    +  Math.Abs(ap[8] - bp[8])
                                    +  Math.Abs(ap[9] - bp[9])
                                    +  Math.Abs(ap[10] - bp[10])
                                    +  Math.Abs(ap[11] - bp[11])
                                    +  Math.Abs(ap[12] - bp[12])
                                    +  Math.Abs(ap[13] - bp[13])
                                    +  Math.Abs(ap[14] - bp[14])
                                    +  Math.Abs(ap[15] - bp[15])
                                    +  Math.Abs(ap[16] - bp[16])
                                    +  Math.Abs(ap[17] - bp[17])
                                    +  Math.Abs(ap[18] - bp[18])
                                    +  Math.Abs(ap[19] - bp[19])
                                    +  Math.Abs(ap[20] - bp[20]);
                                ap += 21;
                                bp += 21;
                                l -= 21;
                            }
                            while (l-- > 0) {
                                sum +=  Math.Abs(*ap - *bp);
                                ap++;
                                bp++;
                            }
                            *cp++ = sum; 
                        }
                    } else {
                        // dim = 1
                        ap = (Int32*)range.Item2;
                        bp = (Int32*)range.Item3;
                        cp = (Int32*)range.Item4;
                        for (int s = 0; s < range.Item1; s++) { // TODO: further potential speedup: unroll in s direction!
                           
                            Int32 val = *bp++;
                            int l = itLen;
                            while (l > 10) {
                                cp[0] +=  Math.Abs(ap[0] - val);
                                cp[1] +=  Math.Abs(ap[1] - val);
                                cp[2] +=  Math.Abs(ap[2] - val);
                                cp[3] +=  Math.Abs(ap[3] - val);
                                cp[4] +=  Math.Abs(ap[4] - val);
                                cp[5] +=  Math.Abs(ap[5] - val);
                                cp[6] +=  Math.Abs(ap[6] - val);
                                cp[7] +=  Math.Abs(ap[7] - val);
                                cp[8] +=  Math.Abs(ap[8] - val);
                                cp[9] +=  Math.Abs(ap[9] - val);
                                cp[10] +=  Math.Abs(ap[10] - val);
                                cp[11] +=  Math.Abs(ap[11] - val);
                                cp[12] +=  Math.Abs(ap[12] - val);
                                cp[13] +=  Math.Abs(ap[13] - val);
                                cp[14] +=  Math.Abs(ap[14] - val);
                                cp[15] +=  Math.Abs(ap[15] - val);
                                cp[16] +=  Math.Abs(ap[16] - val);
                                cp[17] +=  Math.Abs(ap[17] - val);
                                cp[18] +=  Math.Abs(ap[18] - val);
                                cp[19] +=  Math.Abs(ap[19] - val);
                                cp[20] +=  Math.Abs(ap[20] - val);
                                ap += 21;
                                cp += 21;
                                l -= 21;
                            }
                            while (l-- > 0) {
                                *cp +=  Math.Abs(*ap - val);
                                ap++;
                                cp++;
                            }
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                //int[] partResults = new int[workItemCount]; 
                fixed ( Int32* arrAP = arrA)
                fixed ( Int32* arrBP = arrB)
                fixed ( Int32* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * outDims[0] * workItemLength)
                               , (IntPtr)(arrBP + i * dim * workItemLength)
                               , (IntPtr)(retArrP + i * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * outDims[0] * workItemLength)
                                , (IntPtr)(arrBP + i * dim * workItemLength)
                                , (IntPtr)(retArrP + i * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<Int32>(retStorage);
            }
        }
        /// <summary>
        /// pairwise L1 distance 
        /// </summary>
        /// <param name="A">input points (matrix)</param>
        /// <param name="B">input point (vector)</param>
        /// <returns>pairwise L1 distances between the data point provided in the input vector <paramref name="B"/> and the data points stored in the matrix <paramref name="A"/>.</returns>
        /// <remarks>
        /// <para>If <paramref name="B"/> is a colum vector, the distances between <paramref name="B"/> and the columns of <paramref name="A"/> are calculated. The number of rows of <paramref name="A"/> 
        /// must match the length of vector <paramref name="B"/> than. Therefore, the length of the returned vector of distances matches the number of columns of <paramref name="A"/>: <code>A.S[1]</code>.</para>
        /// <para>If <paramref name="B"/> is a row vector, the distances between <paramref name="B"/> and the rows of <paramref name="A"/> are calculated. The number of columns of <paramref name="A"/> 
        /// must match the length of vector <paramref name="B"/> than. Therefore, the length of the returned vector of distances matches the number of rows of <paramref name="A"/>: <code>A.S[0]</code>.</para>
        /// <para>This function is cummulative, the single data point may be provided in <paramref name="A"/> and the data point matrix may be provided in <paramref name="B"/> as well.</para>
        /// </remarks>
        public static unsafe ILRetArray<float> distL1(ILInArray<float> A, ILInArray<float> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<float>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<float>(B.S);
                } else if (B.IsEmpty) {
                    return empty<float>(A.S);
                }
                // early exit: make the function cummutative
                if (A.IsVector && !B.IsVector) {
                    return distL1(B, A); 
                } 
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
                ILSize outDims; 
                if (dim == 0) {
                    outDims = size(1,A.S[1]);    
                } else if (dim == 1) {
                    outDims = size(A.S[0], 1);
                } else if (A.S.IsSameSize(B.S)) {
                    outDims = A.S;
                    dim = 0; 
                } else 
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");

               
                float[] retArr = ILMemoryPool.Pool.New<float>(outDims.NumberOfElements);

               
                float[] arrA = A.GetArrayForRead();

               
                float[] arrB = B.GetArrayForRead();
                int itLen = A.S[0]; 
                #endregion

                #region worker loops definition
                ILDenseStorage<float> retStorage = new ILDenseStorage<float>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;

                   
                    float* ap;

                   
                    float* bp;

                   
                    float* cp;
                    if (dim == 0) {
                        ap = (float*)range.Item2;
                        cp = (float*)range.Item4;
                        for (int s = 0; s < range.Item1; s++) {
                            bp = (float*)range.Item3;
                           
                            float sum = 0; 
                            int l = itLen;
                            while (l > 20) {
                                sum +=  Math.Abs(ap[0] - bp[0])
                                    +  Math.Abs(ap[1] - bp[1])
                                    +  Math.Abs(ap[2] - bp[2])
                                    +  Math.Abs(ap[3] - bp[3])
                                    +  Math.Abs(ap[4] - bp[4])
                                    +  Math.Abs(ap[5] - bp[5])
                                    +  Math.Abs(ap[6] - bp[6])
                                    +  Math.Abs(ap[7] - bp[7])
                                    +  Math.Abs(ap[8] - bp[8])
                                    +  Math.Abs(ap[9] - bp[9])
                                    +  Math.Abs(ap[10] - bp[10])
                                    +  Math.Abs(ap[11] - bp[11])
                                    +  Math.Abs(ap[12] - bp[12])
                                    +  Math.Abs(ap[13] - bp[13])
                                    +  Math.Abs(ap[14] - bp[14])
                                    +  Math.Abs(ap[15] - bp[15])
                                    +  Math.Abs(ap[16] - bp[16])
                                    +  Math.Abs(ap[17] - bp[17])
                                    +  Math.Abs(ap[18] - bp[18])
                                    +  Math.Abs(ap[19] - bp[19])
                                    +  Math.Abs(ap[20] - bp[20]);
                                ap += 21;
                                bp += 21;
                                l -= 21;
                            }
                            while (l-- > 0) {
                                sum +=  Math.Abs(*ap - *bp);
                                ap++;
                                bp++;
                            }
                            *cp++ = sum; 
                        }
                    } else {
                        // dim = 1
                        ap = (float*)range.Item2;
                        bp = (float*)range.Item3;
                        cp = (float*)range.Item4;
                        for (int s = 0; s < range.Item1; s++) { // TODO: further potential speedup: unroll in s direction!
                           
                            float val = *bp++;
                            int l = itLen;
                            while (l > 10) {
                                cp[0] +=  Math.Abs(ap[0] - val);
                                cp[1] +=  Math.Abs(ap[1] - val);
                                cp[2] +=  Math.Abs(ap[2] - val);
                                cp[3] +=  Math.Abs(ap[3] - val);
                                cp[4] +=  Math.Abs(ap[4] - val);
                                cp[5] +=  Math.Abs(ap[5] - val);
                                cp[6] +=  Math.Abs(ap[6] - val);
                                cp[7] +=  Math.Abs(ap[7] - val);
                                cp[8] +=  Math.Abs(ap[8] - val);
                                cp[9] +=  Math.Abs(ap[9] - val);
                                cp[10] +=  Math.Abs(ap[10] - val);
                                cp[11] +=  Math.Abs(ap[11] - val);
                                cp[12] +=  Math.Abs(ap[12] - val);
                                cp[13] +=  Math.Abs(ap[13] - val);
                                cp[14] +=  Math.Abs(ap[14] - val);
                                cp[15] +=  Math.Abs(ap[15] - val);
                                cp[16] +=  Math.Abs(ap[16] - val);
                                cp[17] +=  Math.Abs(ap[17] - val);
                                cp[18] +=  Math.Abs(ap[18] - val);
                                cp[19] +=  Math.Abs(ap[19] - val);
                                cp[20] +=  Math.Abs(ap[20] - val);
                                ap += 21;
                                cp += 21;
                                l -= 21;
                            }
                            while (l-- > 0) {
                                *cp +=  Math.Abs(*ap - val);
                                ap++;
                                cp++;
                            }
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                //int[] partResults = new int[workItemCount]; 
                fixed ( float* arrAP = arrA)
                fixed ( float* arrBP = arrB)
                fixed ( float* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * outDims[0] * workItemLength)
                               , (IntPtr)(arrBP + i * dim * workItemLength)
                               , (IntPtr)(retArrP + i * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * outDims[0] * workItemLength)
                                , (IntPtr)(arrBP + i * dim * workItemLength)
                                , (IntPtr)(retArrP + i * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<float>(retStorage);
            }
        }
        /// <summary>
        /// pairwise L1 distance 
        /// </summary>
        /// <param name="A">input points (matrix)</param>
        /// <param name="B">input point (vector)</param>
        /// <returns>pairwise L1 distances between the data point provided in the input vector <paramref name="B"/> and the data points stored in the matrix <paramref name="A"/>.</returns>
        /// <remarks>
        /// <para>If <paramref name="B"/> is a colum vector, the distances between <paramref name="B"/> and the columns of <paramref name="A"/> are calculated. The number of rows of <paramref name="A"/> 
        /// must match the length of vector <paramref name="B"/> than. Therefore, the length of the returned vector of distances matches the number of columns of <paramref name="A"/>: <code>A.S[1]</code>.</para>
        /// <para>If <paramref name="B"/> is a row vector, the distances between <paramref name="B"/> and the rows of <paramref name="A"/> are calculated. The number of columns of <paramref name="A"/> 
        /// must match the length of vector <paramref name="B"/> than. Therefore, the length of the returned vector of distances matches the number of rows of <paramref name="A"/>: <code>A.S[0]</code>.</para>
        /// <para>This function is cummulative, the single data point may be provided in <paramref name="A"/> and the data point matrix may be provided in <paramref name="B"/> as well.</para>
        /// </remarks>
        public static unsafe ILRetArray<fcomplex> distL1(ILInArray<fcomplex> A, ILInArray<fcomplex> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<fcomplex>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<fcomplex>(B.S);
                } else if (B.IsEmpty) {
                    return empty<fcomplex>(A.S);
                }
                // early exit: make the function cummutative
                if (A.IsVector && !B.IsVector) {
                    return distL1(B, A); 
                } 
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
                ILSize outDims; 
                if (dim == 0) {
                    outDims = size(1,A.S[1]);    
                } else if (dim == 1) {
                    outDims = size(A.S[0], 1);
                } else if (A.S.IsSameSize(B.S)) {
                    outDims = A.S;
                    dim = 0; 
                } else 
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");

               
                fcomplex[] retArr = ILMemoryPool.Pool.New<fcomplex>(outDims.NumberOfElements);

               
                fcomplex[] arrA = A.GetArrayForRead();

               
                fcomplex[] arrB = B.GetArrayForRead();
                int itLen = A.S[0]; 
                #endregion

                #region worker loops definition
                ILDenseStorage<fcomplex> retStorage = new ILDenseStorage<fcomplex>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;

                   
                    fcomplex* ap;

                   
                    fcomplex* bp;

                   
                    fcomplex* cp;
                    if (dim == 0) {
                        ap = (fcomplex*)range.Item2;
                        cp = (fcomplex*)range.Item4;
                        for (int s = 0; s < range.Item1; s++) {
                            bp = (fcomplex*)range.Item3;
                           
                            fcomplex sum = 0; 
                            int l = itLen;
                            while (l > 20) {
                                sum +=  fcomplex.Abs(ap[0] - bp[0])
                                    +  fcomplex.Abs(ap[1] - bp[1])
                                    +  fcomplex.Abs(ap[2] - bp[2])
                                    +  fcomplex.Abs(ap[3] - bp[3])
                                    +  fcomplex.Abs(ap[4] - bp[4])
                                    +  fcomplex.Abs(ap[5] - bp[5])
                                    +  fcomplex.Abs(ap[6] - bp[6])
                                    +  fcomplex.Abs(ap[7] - bp[7])
                                    +  fcomplex.Abs(ap[8] - bp[8])
                                    +  fcomplex.Abs(ap[9] - bp[9])
                                    +  fcomplex.Abs(ap[10] - bp[10])
                                    +  fcomplex.Abs(ap[11] - bp[11])
                                    +  fcomplex.Abs(ap[12] - bp[12])
                                    +  fcomplex.Abs(ap[13] - bp[13])
                                    +  fcomplex.Abs(ap[14] - bp[14])
                                    +  fcomplex.Abs(ap[15] - bp[15])
                                    +  fcomplex.Abs(ap[16] - bp[16])
                                    +  fcomplex.Abs(ap[17] - bp[17])
                                    +  fcomplex.Abs(ap[18] - bp[18])
                                    +  fcomplex.Abs(ap[19] - bp[19])
                                    +  fcomplex.Abs(ap[20] - bp[20]);
                                ap += 21;
                                bp += 21;
                                l -= 21;
                            }
                            while (l-- > 0) {
                                sum +=  fcomplex.Abs(*ap - *bp);
                                ap++;
                                bp++;
                            }
                            *cp++ = sum; 
                        }
                    } else {
                        // dim = 1
                        ap = (fcomplex*)range.Item2;
                        bp = (fcomplex*)range.Item3;
                        cp = (fcomplex*)range.Item4;
                        for (int s = 0; s < range.Item1; s++) { // TODO: further potential speedup: unroll in s direction!
                           
                            fcomplex val = *bp++;
                            int l = itLen;
                            while (l > 10) {
                                cp[0] +=  fcomplex.Abs(ap[0] - val);
                                cp[1] +=  fcomplex.Abs(ap[1] - val);
                                cp[2] +=  fcomplex.Abs(ap[2] - val);
                                cp[3] +=  fcomplex.Abs(ap[3] - val);
                                cp[4] +=  fcomplex.Abs(ap[4] - val);
                                cp[5] +=  fcomplex.Abs(ap[5] - val);
                                cp[6] +=  fcomplex.Abs(ap[6] - val);
                                cp[7] +=  fcomplex.Abs(ap[7] - val);
                                cp[8] +=  fcomplex.Abs(ap[8] - val);
                                cp[9] +=  fcomplex.Abs(ap[9] - val);
                                cp[10] +=  fcomplex.Abs(ap[10] - val);
                                cp[11] +=  fcomplex.Abs(ap[11] - val);
                                cp[12] +=  fcomplex.Abs(ap[12] - val);
                                cp[13] +=  fcomplex.Abs(ap[13] - val);
                                cp[14] +=  fcomplex.Abs(ap[14] - val);
                                cp[15] +=  fcomplex.Abs(ap[15] - val);
                                cp[16] +=  fcomplex.Abs(ap[16] - val);
                                cp[17] +=  fcomplex.Abs(ap[17] - val);
                                cp[18] +=  fcomplex.Abs(ap[18] - val);
                                cp[19] +=  fcomplex.Abs(ap[19] - val);
                                cp[20] +=  fcomplex.Abs(ap[20] - val);
                                ap += 21;
                                cp += 21;
                                l -= 21;
                            }
                            while (l-- > 0) {
                                *cp +=  fcomplex.Abs(*ap - val);
                                ap++;
                                cp++;
                            }
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                //int[] partResults = new int[workItemCount]; 
                fixed ( fcomplex* arrAP = arrA)
                fixed ( fcomplex* arrBP = arrB)
                fixed ( fcomplex* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * outDims[0] * workItemLength)
                               , (IntPtr)(arrBP + i * dim * workItemLength)
                               , (IntPtr)(retArrP + i * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * outDims[0] * workItemLength)
                                , (IntPtr)(arrBP + i * dim * workItemLength)
                                , (IntPtr)(retArrP + i * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<fcomplex>(retStorage);
            }
        }
        /// <summary>
        /// pairwise L1 distance 
        /// </summary>
        /// <param name="A">input points (matrix)</param>
        /// <param name="B">input point (vector)</param>
        /// <returns>pairwise L1 distances between the data point provided in the input vector <paramref name="B"/> and the data points stored in the matrix <paramref name="A"/>.</returns>
        /// <remarks>
        /// <para>If <paramref name="B"/> is a colum vector, the distances between <paramref name="B"/> and the columns of <paramref name="A"/> are calculated. The number of rows of <paramref name="A"/> 
        /// must match the length of vector <paramref name="B"/> than. Therefore, the length of the returned vector of distances matches the number of columns of <paramref name="A"/>: <code>A.S[1]</code>.</para>
        /// <para>If <paramref name="B"/> is a row vector, the distances between <paramref name="B"/> and the rows of <paramref name="A"/> are calculated. The number of columns of <paramref name="A"/> 
        /// must match the length of vector <paramref name="B"/> than. Therefore, the length of the returned vector of distances matches the number of rows of <paramref name="A"/>: <code>A.S[0]</code>.</para>
        /// <para>This function is cummulative, the single data point may be provided in <paramref name="A"/> and the data point matrix may be provided in <paramref name="B"/> as well.</para>
        /// </remarks>
        public static unsafe ILRetArray<complex> distL1(ILInArray<complex> A, ILInArray<complex> B) {
            using (ILScope.Enter(A, B)) {

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<complex>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<complex>(B.S);
                } else if (B.IsEmpty) {
                    return empty<complex>(A.S);
                }
                // early exit: make the function cummutative
                if (A.IsVector && !B.IsVector) {
                    return distL1(B, A); 
                } 
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
                ILSize outDims; 
                if (dim == 0) {
                    outDims = size(1,A.S[1]);    
                } else if (dim == 1) {
                    outDims = size(A.S[0], 1);
                } else if (A.S.IsSameSize(B.S)) {
                    outDims = A.S;
                    dim = 0; 
                } else 
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");

               
                complex[] retArr = ILMemoryPool.Pool.New<complex>(outDims.NumberOfElements);

               
                complex[] arrA = A.GetArrayForRead();

               
                complex[] arrB = B.GetArrayForRead();
                int itLen = A.S[0]; 
                #endregion

                #region worker loops definition
                ILDenseStorage<complex> retStorage = new ILDenseStorage<complex>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;

                   
                    complex* ap;

                   
                    complex* bp;

                   
                    complex* cp;
                    if (dim == 0) {
                        ap = (complex*)range.Item2;
                        cp = (complex*)range.Item4;
                        for (int s = 0; s < range.Item1; s++) {
                            bp = (complex*)range.Item3;
                           
                            complex sum = 0; 
                            int l = itLen;
                            while (l > 20) {
                                sum +=  complex.Abs(ap[0] - bp[0])
                                    +  complex.Abs(ap[1] - bp[1])
                                    +  complex.Abs(ap[2] - bp[2])
                                    +  complex.Abs(ap[3] - bp[3])
                                    +  complex.Abs(ap[4] - bp[4])
                                    +  complex.Abs(ap[5] - bp[5])
                                    +  complex.Abs(ap[6] - bp[6])
                                    +  complex.Abs(ap[7] - bp[7])
                                    +  complex.Abs(ap[8] - bp[8])
                                    +  complex.Abs(ap[9] - bp[9])
                                    +  complex.Abs(ap[10] - bp[10])
                                    +  complex.Abs(ap[11] - bp[11])
                                    +  complex.Abs(ap[12] - bp[12])
                                    +  complex.Abs(ap[13] - bp[13])
                                    +  complex.Abs(ap[14] - bp[14])
                                    +  complex.Abs(ap[15] - bp[15])
                                    +  complex.Abs(ap[16] - bp[16])
                                    +  complex.Abs(ap[17] - bp[17])
                                    +  complex.Abs(ap[18] - bp[18])
                                    +  complex.Abs(ap[19] - bp[19])
                                    +  complex.Abs(ap[20] - bp[20]);
                                ap += 21;
                                bp += 21;
                                l -= 21;
                            }
                            while (l-- > 0) {
                                sum +=  complex.Abs(*ap - *bp);
                                ap++;
                                bp++;
                            }
                            *cp++ = sum; 
                        }
                    } else {
                        // dim = 1
                        ap = (complex*)range.Item2;
                        bp = (complex*)range.Item3;
                        cp = (complex*)range.Item4;
                        for (int s = 0; s < range.Item1; s++) { // TODO: further potential speedup: unroll in s direction!
                           
                            complex val = *bp++;
                            int l = itLen;
                            while (l > 10) {
                                cp[0] +=  complex.Abs(ap[0] - val);
                                cp[1] +=  complex.Abs(ap[1] - val);
                                cp[2] +=  complex.Abs(ap[2] - val);
                                cp[3] +=  complex.Abs(ap[3] - val);
                                cp[4] +=  complex.Abs(ap[4] - val);
                                cp[5] +=  complex.Abs(ap[5] - val);
                                cp[6] +=  complex.Abs(ap[6] - val);
                                cp[7] +=  complex.Abs(ap[7] - val);
                                cp[8] +=  complex.Abs(ap[8] - val);
                                cp[9] +=  complex.Abs(ap[9] - val);
                                cp[10] +=  complex.Abs(ap[10] - val);
                                cp[11] +=  complex.Abs(ap[11] - val);
                                cp[12] +=  complex.Abs(ap[12] - val);
                                cp[13] +=  complex.Abs(ap[13] - val);
                                cp[14] +=  complex.Abs(ap[14] - val);
                                cp[15] +=  complex.Abs(ap[15] - val);
                                cp[16] +=  complex.Abs(ap[16] - val);
                                cp[17] +=  complex.Abs(ap[17] - val);
                                cp[18] +=  complex.Abs(ap[18] - val);
                                cp[19] +=  complex.Abs(ap[19] - val);
                                cp[20] +=  complex.Abs(ap[20] - val);
                                ap += 21;
                                cp += 21;
                                l -= 21;
                            }
                            while (l-- > 0) {
                                *cp +=  complex.Abs(*ap - val);
                                ap++;
                                cp++;
                            }
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                //int[] partResults = new int[workItemCount]; 
                fixed ( complex* arrAP = arrA)
                fixed ( complex* arrBP = arrB)
                fixed ( complex* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * outDims[0] * workItemLength)
                               , (IntPtr)(arrBP + i * dim * workItemLength)
                               , (IntPtr)(retArrP + i * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * outDims[0] * workItemLength)
                                , (IntPtr)(arrBP + i * dim * workItemLength)
                                , (IntPtr)(retArrP + i * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<complex>(retStorage);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}