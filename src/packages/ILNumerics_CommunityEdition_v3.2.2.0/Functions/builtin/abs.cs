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
using ILNumerics;
using ILNumerics.Exceptions;
using ILNumerics.Storage;
using ILNumerics.Misc;


namespace ILNumerics {
    public partial class ILMath {
        /// <summary>Absolute values of array elements</summary>
        /// <param name="A">Input Array</param>
        /// <returns>Absolute values of array elements</returns>
        /// <remarks><para>If the Input Array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static ILRetArray<byte> abs(ILInArray<byte> A) {
            using (ILScope.Enter(A)) {
                return A.C;
            }
        }


                        

#region HYCALPER AUTO GENERATED CODE

        /// <summary>Absolute values of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Absolute values of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<double>  abs (ILInArray< double > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<double>(A.Size); 
                ILSize inDim = A.Size;
                double[] arrA = A.GetArrayForRead(); 
                double [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (!A.TryGetStorage4InplaceOp(out retArr)){
                    retArr = ILMemoryPool.Pool.New<double>(outLen);
                    inplace = false; 
                }
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }
                ILDenseStorage<double> retStorage = new ILDenseStorage<double>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    double* cp = ((double*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  Math.Abs(cp[0]  )  /*dummy*/;
                            cp[1] =  Math.Abs(cp[1]  )  /*dummy*/;
                            cp[2] =  Math.Abs(cp[2]  )  /*dummy*/;
                            cp[3] =  Math.Abs(cp[3]  )  /*dummy*/;
                            cp[4] =  Math.Abs(cp[4]  )  /*dummy*/;
                            cp[5] =  Math.Abs(cp[5]  )  /*dummy*/;
                            cp[6] =  Math.Abs(cp[6]  )  /*dummy*/;
                            cp[7] =  Math.Abs(cp[7]  )  /*dummy*/;
                            cp[8] =  Math.Abs(cp[8]  )  /*dummy*/;
                            cp[9] =  Math.Abs(cp[9]  )  /*dummy*/;
                            cp[10] =  Math.Abs(cp[10]  )  /*dummy*/;
                            cp[11] =  Math.Abs(cp[11]  )  /*dummy*/;
                            cp[12] =  Math.Abs(cp[12]  )  /*dummy*/;
                            cp[13] =  Math.Abs(cp[13]  )  /*dummy*/;
                            cp[14] =  Math.Abs(cp[14]  )  /*dummy*/;
                            cp[15] =  Math.Abs(cp[15]  )  /*dummy*/;
                            cp[16] =  Math.Abs(cp[16]  )  /*dummy*/;
                            cp[17] =  Math.Abs(cp[17]  )  /*dummy*/;
                            cp[18] =  Math.Abs(cp[18]  )  /*dummy*/;
                            cp[19] =  Math.Abs(cp[19]  )  /*dummy*/;
                            cp[20] =  Math.Abs(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Math.Abs(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        double* ap = ((double*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Math.Abs(ap[0]  )  /*dummy*/;
                            cp[1] =  Math.Abs(ap[1]  )  /*dummy*/;
                            cp[2] =  Math.Abs(ap[2]  )  /*dummy*/;
                            cp[3] =  Math.Abs(ap[3]  )  /*dummy*/;
                            cp[4] =  Math.Abs(ap[4]  )  /*dummy*/;
                            cp[5] =  Math.Abs(ap[5]  )  /*dummy*/;
                            cp[6] =  Math.Abs(ap[6]  )  /*dummy*/;
                            cp[7] =  Math.Abs(ap[7]  )  /*dummy*/;
                            cp[8] =  Math.Abs(ap[8]  )  /*dummy*/;
                            cp[9] =  Math.Abs(ap[9]  )  /*dummy*/;
                            cp[10] =  Math.Abs(ap[10]  )  /*dummy*/;
                            cp[11] =  Math.Abs(ap[11]  )  /*dummy*/;
                            cp[12] =  Math.Abs(ap[12]  )  /*dummy*/;
                            cp[13] =  Math.Abs(ap[13]  )  /*dummy*/;
                            cp[14] =  Math.Abs(ap[14]  )  /*dummy*/;
                            cp[15] =  Math.Abs(ap[15]  )  /*dummy*/;
                            cp[16] =  Math.Abs(ap[16]  )  /*dummy*/;
                            cp[17] =  Math.Abs(ap[17]  )  /*dummy*/;
                            cp[18] =  Math.Abs(ap[18]  )  /*dummy*/;
                            cp[19] =  Math.Abs(ap[19]  )  /*dummy*/;
                            cp[20] =  Math.Abs(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Math.Abs(*ap  )  /*dummy*/;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( double* arrAP = arrA)
                fixed ( double* retArrP = retArr) {
                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, bool> range
                            = new Tuple<int, int, IntPtr, IntPtr, bool>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)retArrP, inplace);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, bool>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)retArrP, inplace));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                return new  ILRetArray<double>(retStorage);
            }
        }
        /// <summary>Absolute values of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Absolute values of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<float>  abs (ILInArray< float > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<float>(A.Size); 
                ILSize inDim = A.Size;
                float[] arrA = A.GetArrayForRead(); 
                float [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (!A.TryGetStorage4InplaceOp(out retArr)){
                    retArr = ILMemoryPool.Pool.New<float>(outLen);
                    inplace = false; 
                }
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }
                ILDenseStorage<float> retStorage = new ILDenseStorage<float>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    float* cp = ((float*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  Math.Abs(cp[0]  )  /*dummy*/;
                            cp[1] =  Math.Abs(cp[1]  )  /*dummy*/;
                            cp[2] =  Math.Abs(cp[2]  )  /*dummy*/;
                            cp[3] =  Math.Abs(cp[3]  )  /*dummy*/;
                            cp[4] =  Math.Abs(cp[4]  )  /*dummy*/;
                            cp[5] =  Math.Abs(cp[5]  )  /*dummy*/;
                            cp[6] =  Math.Abs(cp[6]  )  /*dummy*/;
                            cp[7] =  Math.Abs(cp[7]  )  /*dummy*/;
                            cp[8] =  Math.Abs(cp[8]  )  /*dummy*/;
                            cp[9] =  Math.Abs(cp[9]  )  /*dummy*/;
                            cp[10] =  Math.Abs(cp[10]  )  /*dummy*/;
                            cp[11] =  Math.Abs(cp[11]  )  /*dummy*/;
                            cp[12] =  Math.Abs(cp[12]  )  /*dummy*/;
                            cp[13] =  Math.Abs(cp[13]  )  /*dummy*/;
                            cp[14] =  Math.Abs(cp[14]  )  /*dummy*/;
                            cp[15] =  Math.Abs(cp[15]  )  /*dummy*/;
                            cp[16] =  Math.Abs(cp[16]  )  /*dummy*/;
                            cp[17] =  Math.Abs(cp[17]  )  /*dummy*/;
                            cp[18] =  Math.Abs(cp[18]  )  /*dummy*/;
                            cp[19] =  Math.Abs(cp[19]  )  /*dummy*/;
                            cp[20] =  Math.Abs(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Math.Abs(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        float* ap = ((float*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Math.Abs(ap[0]  )  /*dummy*/;
                            cp[1] =  Math.Abs(ap[1]  )  /*dummy*/;
                            cp[2] =  Math.Abs(ap[2]  )  /*dummy*/;
                            cp[3] =  Math.Abs(ap[3]  )  /*dummy*/;
                            cp[4] =  Math.Abs(ap[4]  )  /*dummy*/;
                            cp[5] =  Math.Abs(ap[5]  )  /*dummy*/;
                            cp[6] =  Math.Abs(ap[6]  )  /*dummy*/;
                            cp[7] =  Math.Abs(ap[7]  )  /*dummy*/;
                            cp[8] =  Math.Abs(ap[8]  )  /*dummy*/;
                            cp[9] =  Math.Abs(ap[9]  )  /*dummy*/;
                            cp[10] =  Math.Abs(ap[10]  )  /*dummy*/;
                            cp[11] =  Math.Abs(ap[11]  )  /*dummy*/;
                            cp[12] =  Math.Abs(ap[12]  )  /*dummy*/;
                            cp[13] =  Math.Abs(ap[13]  )  /*dummy*/;
                            cp[14] =  Math.Abs(ap[14]  )  /*dummy*/;
                            cp[15] =  Math.Abs(ap[15]  )  /*dummy*/;
                            cp[16] =  Math.Abs(ap[16]  )  /*dummy*/;
                            cp[17] =  Math.Abs(ap[17]  )  /*dummy*/;
                            cp[18] =  Math.Abs(ap[18]  )  /*dummy*/;
                            cp[19] =  Math.Abs(ap[19]  )  /*dummy*/;
                            cp[20] =  Math.Abs(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Math.Abs(*ap  )  /*dummy*/;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( float* arrAP = arrA)
                fixed ( float* retArrP = retArr) {
                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, bool> range
                            = new Tuple<int, int, IntPtr, IntPtr, bool>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)retArrP, inplace);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, bool>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)retArrP, inplace));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                return new  ILRetArray<float>(retStorage);
            }
        }
        /// <summary>Magnitude of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Magnitude of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<float>  abs (ILInArray< fcomplex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<float>(A.Size); 
                ILSize inDim = A.Size;
                fcomplex[] arrA = A.GetArrayForRead(); 
                float [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true){
                    retArr = ILMemoryPool.Pool.New<float>(outLen);
                    inplace = false; 
                }
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }
                ILDenseStorage<float> retStorage = new ILDenseStorage<float>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    float* cp = ((float*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  fcomplex.Abs(cp[0]  )  /*dummy*/;
                            cp[1] =  fcomplex.Abs(cp[1]  )  /*dummy*/;
                            cp[2] =  fcomplex.Abs(cp[2]  )  /*dummy*/;
                            cp[3] =  fcomplex.Abs(cp[3]  )  /*dummy*/;
                            cp[4] =  fcomplex.Abs(cp[4]  )  /*dummy*/;
                            cp[5] =  fcomplex.Abs(cp[5]  )  /*dummy*/;
                            cp[6] =  fcomplex.Abs(cp[6]  )  /*dummy*/;
                            cp[7] =  fcomplex.Abs(cp[7]  )  /*dummy*/;
                            cp[8] =  fcomplex.Abs(cp[8]  )  /*dummy*/;
                            cp[9] =  fcomplex.Abs(cp[9]  )  /*dummy*/;
                            cp[10] =  fcomplex.Abs(cp[10]  )  /*dummy*/;
                            cp[11] =  fcomplex.Abs(cp[11]  )  /*dummy*/;
                            cp[12] =  fcomplex.Abs(cp[12]  )  /*dummy*/;
                            cp[13] =  fcomplex.Abs(cp[13]  )  /*dummy*/;
                            cp[14] =  fcomplex.Abs(cp[14]  )  /*dummy*/;
                            cp[15] =  fcomplex.Abs(cp[15]  )  /*dummy*/;
                            cp[16] =  fcomplex.Abs(cp[16]  )  /*dummy*/;
                            cp[17] =  fcomplex.Abs(cp[17]  )  /*dummy*/;
                            cp[18] =  fcomplex.Abs(cp[18]  )  /*dummy*/;
                            cp[19] =  fcomplex.Abs(cp[19]  )  /*dummy*/;
                            cp[20] =  fcomplex.Abs(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.Abs(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  fcomplex.Abs(ap[0]  )  /*dummy*/;
                            cp[1] =  fcomplex.Abs(ap[1]  )  /*dummy*/;
                            cp[2] =  fcomplex.Abs(ap[2]  )  /*dummy*/;
                            cp[3] =  fcomplex.Abs(ap[3]  )  /*dummy*/;
                            cp[4] =  fcomplex.Abs(ap[4]  )  /*dummy*/;
                            cp[5] =  fcomplex.Abs(ap[5]  )  /*dummy*/;
                            cp[6] =  fcomplex.Abs(ap[6]  )  /*dummy*/;
                            cp[7] =  fcomplex.Abs(ap[7]  )  /*dummy*/;
                            cp[8] =  fcomplex.Abs(ap[8]  )  /*dummy*/;
                            cp[9] =  fcomplex.Abs(ap[9]  )  /*dummy*/;
                            cp[10] =  fcomplex.Abs(ap[10]  )  /*dummy*/;
                            cp[11] =  fcomplex.Abs(ap[11]  )  /*dummy*/;
                            cp[12] =  fcomplex.Abs(ap[12]  )  /*dummy*/;
                            cp[13] =  fcomplex.Abs(ap[13]  )  /*dummy*/;
                            cp[14] =  fcomplex.Abs(ap[14]  )  /*dummy*/;
                            cp[15] =  fcomplex.Abs(ap[15]  )  /*dummy*/;
                            cp[16] =  fcomplex.Abs(ap[16]  )  /*dummy*/;
                            cp[17] =  fcomplex.Abs(ap[17]  )  /*dummy*/;
                            cp[18] =  fcomplex.Abs(ap[18]  )  /*dummy*/;
                            cp[19] =  fcomplex.Abs(ap[19]  )  /*dummy*/;
                            cp[20] =  fcomplex.Abs(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.Abs(*ap  )  /*dummy*/;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( fcomplex* arrAP = arrA)
                fixed ( float* retArrP = retArr) {
                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, bool> range
                            = new Tuple<int, int, IntPtr, IntPtr, bool>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)retArrP, inplace);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, bool>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)retArrP, inplace));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                return new  ILRetArray<float>(retStorage);
            }
        }
        /// <summary>Magnitude of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Magnitude of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<double>  abs (ILInArray< complex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<double>(A.Size); 
                ILSize inDim = A.Size;
                complex[] arrA = A.GetArrayForRead(); 
                double [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true){
                    retArr = ILMemoryPool.Pool.New<double>(outLen);
                    inplace = false; 
                }
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }
                ILDenseStorage<double> retStorage = new ILDenseStorage<double>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    double* cp = ((double*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  complex.Abs(cp[0]  )  /*dummy*/;
                            cp[1] =  complex.Abs(cp[1]  )  /*dummy*/;
                            cp[2] =  complex.Abs(cp[2]  )  /*dummy*/;
                            cp[3] =  complex.Abs(cp[3]  )  /*dummy*/;
                            cp[4] =  complex.Abs(cp[4]  )  /*dummy*/;
                            cp[5] =  complex.Abs(cp[5]  )  /*dummy*/;
                            cp[6] =  complex.Abs(cp[6]  )  /*dummy*/;
                            cp[7] =  complex.Abs(cp[7]  )  /*dummy*/;
                            cp[8] =  complex.Abs(cp[8]  )  /*dummy*/;
                            cp[9] =  complex.Abs(cp[9]  )  /*dummy*/;
                            cp[10] =  complex.Abs(cp[10]  )  /*dummy*/;
                            cp[11] =  complex.Abs(cp[11]  )  /*dummy*/;
                            cp[12] =  complex.Abs(cp[12]  )  /*dummy*/;
                            cp[13] =  complex.Abs(cp[13]  )  /*dummy*/;
                            cp[14] =  complex.Abs(cp[14]  )  /*dummy*/;
                            cp[15] =  complex.Abs(cp[15]  )  /*dummy*/;
                            cp[16] =  complex.Abs(cp[16]  )  /*dummy*/;
                            cp[17] =  complex.Abs(cp[17]  )  /*dummy*/;
                            cp[18] =  complex.Abs(cp[18]  )  /*dummy*/;
                            cp[19] =  complex.Abs(cp[19]  )  /*dummy*/;
                            cp[20] =  complex.Abs(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.Abs(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        complex* ap = ((complex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  complex.Abs(ap[0]  )  /*dummy*/;
                            cp[1] =  complex.Abs(ap[1]  )  /*dummy*/;
                            cp[2] =  complex.Abs(ap[2]  )  /*dummy*/;
                            cp[3] =  complex.Abs(ap[3]  )  /*dummy*/;
                            cp[4] =  complex.Abs(ap[4]  )  /*dummy*/;
                            cp[5] =  complex.Abs(ap[5]  )  /*dummy*/;
                            cp[6] =  complex.Abs(ap[6]  )  /*dummy*/;
                            cp[7] =  complex.Abs(ap[7]  )  /*dummy*/;
                            cp[8] =  complex.Abs(ap[8]  )  /*dummy*/;
                            cp[9] =  complex.Abs(ap[9]  )  /*dummy*/;
                            cp[10] =  complex.Abs(ap[10]  )  /*dummy*/;
                            cp[11] =  complex.Abs(ap[11]  )  /*dummy*/;
                            cp[12] =  complex.Abs(ap[12]  )  /*dummy*/;
                            cp[13] =  complex.Abs(ap[13]  )  /*dummy*/;
                            cp[14] =  complex.Abs(ap[14]  )  /*dummy*/;
                            cp[15] =  complex.Abs(ap[15]  )  /*dummy*/;
                            cp[16] =  complex.Abs(ap[16]  )  /*dummy*/;
                            cp[17] =  complex.Abs(ap[17]  )  /*dummy*/;
                            cp[18] =  complex.Abs(ap[18]  )  /*dummy*/;
                            cp[19] =  complex.Abs(ap[19]  )  /*dummy*/;
                            cp[20] =  complex.Abs(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.Abs(*ap  )  /*dummy*/;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( complex* arrAP = arrA)
                fixed ( double* retArrP = retArr) {
                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, bool> range
                            = new Tuple<int, int, IntPtr, IntPtr, bool>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)retArrP, inplace);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, bool>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)retArrP, inplace));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                return new  ILRetArray<double>(retStorage);
            }
        }
        /// <summary>Absolute values of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Absolute values of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<Int64>  abs (ILInArray< Int64 > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<Int64>(A.Size); 
                ILSize inDim = A.Size;
                Int64[] arrA = A.GetArrayForRead(); 
                Int64 [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (!A.TryGetStorage4InplaceOp(out retArr)){
                    retArr = ILMemoryPool.Pool.New<Int64>(outLen);
                    inplace = false; 
                }
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }
                ILDenseStorage<Int64> retStorage = new ILDenseStorage<Int64>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    Int64* cp = ((Int64*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  Math.Abs(cp[0]  )  /*dummy*/;
                            cp[1] =  Math.Abs(cp[1]  )  /*dummy*/;
                            cp[2] =  Math.Abs(cp[2]  )  /*dummy*/;
                            cp[3] =  Math.Abs(cp[3]  )  /*dummy*/;
                            cp[4] =  Math.Abs(cp[4]  )  /*dummy*/;
                            cp[5] =  Math.Abs(cp[5]  )  /*dummy*/;
                            cp[6] =  Math.Abs(cp[6]  )  /*dummy*/;
                            cp[7] =  Math.Abs(cp[7]  )  /*dummy*/;
                            cp[8] =  Math.Abs(cp[8]  )  /*dummy*/;
                            cp[9] =  Math.Abs(cp[9]  )  /*dummy*/;
                            cp[10] =  Math.Abs(cp[10]  )  /*dummy*/;
                            cp[11] =  Math.Abs(cp[11]  )  /*dummy*/;
                            cp[12] =  Math.Abs(cp[12]  )  /*dummy*/;
                            cp[13] =  Math.Abs(cp[13]  )  /*dummy*/;
                            cp[14] =  Math.Abs(cp[14]  )  /*dummy*/;
                            cp[15] =  Math.Abs(cp[15]  )  /*dummy*/;
                            cp[16] =  Math.Abs(cp[16]  )  /*dummy*/;
                            cp[17] =  Math.Abs(cp[17]  )  /*dummy*/;
                            cp[18] =  Math.Abs(cp[18]  )  /*dummy*/;
                            cp[19] =  Math.Abs(cp[19]  )  /*dummy*/;
                            cp[20] =  Math.Abs(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Math.Abs(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        Int64* ap = ((Int64*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Math.Abs(ap[0]  )  /*dummy*/;
                            cp[1] =  Math.Abs(ap[1]  )  /*dummy*/;
                            cp[2] =  Math.Abs(ap[2]  )  /*dummy*/;
                            cp[3] =  Math.Abs(ap[3]  )  /*dummy*/;
                            cp[4] =  Math.Abs(ap[4]  )  /*dummy*/;
                            cp[5] =  Math.Abs(ap[5]  )  /*dummy*/;
                            cp[6] =  Math.Abs(ap[6]  )  /*dummy*/;
                            cp[7] =  Math.Abs(ap[7]  )  /*dummy*/;
                            cp[8] =  Math.Abs(ap[8]  )  /*dummy*/;
                            cp[9] =  Math.Abs(ap[9]  )  /*dummy*/;
                            cp[10] =  Math.Abs(ap[10]  )  /*dummy*/;
                            cp[11] =  Math.Abs(ap[11]  )  /*dummy*/;
                            cp[12] =  Math.Abs(ap[12]  )  /*dummy*/;
                            cp[13] =  Math.Abs(ap[13]  )  /*dummy*/;
                            cp[14] =  Math.Abs(ap[14]  )  /*dummy*/;
                            cp[15] =  Math.Abs(ap[15]  )  /*dummy*/;
                            cp[16] =  Math.Abs(ap[16]  )  /*dummy*/;
                            cp[17] =  Math.Abs(ap[17]  )  /*dummy*/;
                            cp[18] =  Math.Abs(ap[18]  )  /*dummy*/;
                            cp[19] =  Math.Abs(ap[19]  )  /*dummy*/;
                            cp[20] =  Math.Abs(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Math.Abs(*ap  )  /*dummy*/;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( Int64* arrAP = arrA)
                fixed ( Int64* retArrP = retArr) {
                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, bool> range
                            = new Tuple<int, int, IntPtr, IntPtr, bool>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)retArrP, inplace);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, bool>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)retArrP, inplace));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                return new  ILRetArray<Int64>(retStorage);
            }
        }
        /// <summary>Absolute values of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Absolute values of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<Int32>  abs (ILInArray< Int32 > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<Int32>(A.Size); 
                ILSize inDim = A.Size;
                Int32[] arrA = A.GetArrayForRead(); 
                Int32 [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (!A.TryGetStorage4InplaceOp(out retArr)){
                    retArr = ILMemoryPool.Pool.New<Int32>(outLen);
                    inplace = false; 
                }
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength, workerCount = 1;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen;
                    workItemCount = 1;
                }
                ILDenseStorage<Int32> retStorage = new ILDenseStorage<Int32>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    Int32* cp = ((Int32*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  Math.Abs(cp[0]  )  /*dummy*/;
                            cp[1] =  Math.Abs(cp[1]  )  /*dummy*/;
                            cp[2] =  Math.Abs(cp[2]  )  /*dummy*/;
                            cp[3] =  Math.Abs(cp[3]  )  /*dummy*/;
                            cp[4] =  Math.Abs(cp[4]  )  /*dummy*/;
                            cp[5] =  Math.Abs(cp[5]  )  /*dummy*/;
                            cp[6] =  Math.Abs(cp[6]  )  /*dummy*/;
                            cp[7] =  Math.Abs(cp[7]  )  /*dummy*/;
                            cp[8] =  Math.Abs(cp[8]  )  /*dummy*/;
                            cp[9] =  Math.Abs(cp[9]  )  /*dummy*/;
                            cp[10] =  Math.Abs(cp[10]  )  /*dummy*/;
                            cp[11] =  Math.Abs(cp[11]  )  /*dummy*/;
                            cp[12] =  Math.Abs(cp[12]  )  /*dummy*/;
                            cp[13] =  Math.Abs(cp[13]  )  /*dummy*/;
                            cp[14] =  Math.Abs(cp[14]  )  /*dummy*/;
                            cp[15] =  Math.Abs(cp[15]  )  /*dummy*/;
                            cp[16] =  Math.Abs(cp[16]  )  /*dummy*/;
                            cp[17] =  Math.Abs(cp[17]  )  /*dummy*/;
                            cp[18] =  Math.Abs(cp[18]  )  /*dummy*/;
                            cp[19] =  Math.Abs(cp[19]  )  /*dummy*/;
                            cp[20] =  Math.Abs(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Math.Abs(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        Int32* ap = ((Int32*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Math.Abs(ap[0]  )  /*dummy*/;
                            cp[1] =  Math.Abs(ap[1]  )  /*dummy*/;
                            cp[2] =  Math.Abs(ap[2]  )  /*dummy*/;
                            cp[3] =  Math.Abs(ap[3]  )  /*dummy*/;
                            cp[4] =  Math.Abs(ap[4]  )  /*dummy*/;
                            cp[5] =  Math.Abs(ap[5]  )  /*dummy*/;
                            cp[6] =  Math.Abs(ap[6]  )  /*dummy*/;
                            cp[7] =  Math.Abs(ap[7]  )  /*dummy*/;
                            cp[8] =  Math.Abs(ap[8]  )  /*dummy*/;
                            cp[9] =  Math.Abs(ap[9]  )  /*dummy*/;
                            cp[10] =  Math.Abs(ap[10]  )  /*dummy*/;
                            cp[11] =  Math.Abs(ap[11]  )  /*dummy*/;
                            cp[12] =  Math.Abs(ap[12]  )  /*dummy*/;
                            cp[13] =  Math.Abs(ap[13]  )  /*dummy*/;
                            cp[14] =  Math.Abs(ap[14]  )  /*dummy*/;
                            cp[15] =  Math.Abs(ap[15]  )  /*dummy*/;
                            cp[16] =  Math.Abs(ap[16]  )  /*dummy*/;
                            cp[17] =  Math.Abs(ap[17]  )  /*dummy*/;
                            cp[18] =  Math.Abs(ap[18]  )  /*dummy*/;
                            cp[19] =  Math.Abs(ap[19]  )  /*dummy*/;
                            cp[20] =  Math.Abs(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Math.Abs(*ap  )  /*dummy*/;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( Int32* arrAP = arrA)
                fixed ( Int32* retArrP = retArr) {
                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, bool> range
                            = new Tuple<int, int, IntPtr, IntPtr, bool>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)retArrP, inplace);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i,worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, bool>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)retArrP, inplace));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                return new  ILRetArray<Int32>(retStorage);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}