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
        



#region HYCALPER AUTO GENERATED CODE

        /// <summary>Finds invalid value elements (not-a-number)</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is invalid ("Not-a-Number"), 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isnan (ILInArray< double > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size); 
                ILSize inDim = A.Size;
                double[] arrA = A.GetArrayForRead(); 
                byte [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true){
                    retArr = ILMemoryPool.Pool.New<byte>(outLen);
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
                ILDenseStorage<byte> retStorage = new ILDenseStorage<byte>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    byte* cp = ((byte*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  Double.IsNaN(cp[0]  )  ?(byte)1:(byte)0;
                            cp[1] =  Double.IsNaN(cp[1]  )  ?(byte)1:(byte)0;
                            cp[2] =  Double.IsNaN(cp[2]  )  ?(byte)1:(byte)0;
                            cp[3] =  Double.IsNaN(cp[3]  )  ?(byte)1:(byte)0;
                            cp[4] =  Double.IsNaN(cp[4]  )  ?(byte)1:(byte)0;
                            cp[5] =  Double.IsNaN(cp[5]  )  ?(byte)1:(byte)0;
                            cp[6] =  Double.IsNaN(cp[6]  )  ?(byte)1:(byte)0;
                            cp[7] =  Double.IsNaN(cp[7]  )  ?(byte)1:(byte)0;
                            cp[8] =  Double.IsNaN(cp[8]  )  ?(byte)1:(byte)0;
                            cp[9] =  Double.IsNaN(cp[9]  )  ?(byte)1:(byte)0;
                            cp[10] =  Double.IsNaN(cp[10]  )  ?(byte)1:(byte)0;
                            cp[11] =  Double.IsNaN(cp[11]  )  ?(byte)1:(byte)0;
                            cp[12] =  Double.IsNaN(cp[12]  )  ?(byte)1:(byte)0;
                            cp[13] =  Double.IsNaN(cp[13]  )  ?(byte)1:(byte)0;
                            cp[14] =  Double.IsNaN(cp[14]  )  ?(byte)1:(byte)0;
                            cp[15] =  Double.IsNaN(cp[15]  )  ?(byte)1:(byte)0;
                            cp[16] =  Double.IsNaN(cp[16]  )  ?(byte)1:(byte)0;
                            cp[17] =  Double.IsNaN(cp[17]  )  ?(byte)1:(byte)0;
                            cp[18] =  Double.IsNaN(cp[18]  )  ?(byte)1:(byte)0;
                            cp[19] =  Double.IsNaN(cp[19]  )  ?(byte)1:(byte)0;
                            cp[20] =  Double.IsNaN(cp[20]  )  ?(byte)1:(byte)0;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Double.IsNaN(*cp  )  ?(byte)1:(byte)0;
                            cp++;
                        }
                    } else {
                        double* ap = ((double*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Double.IsNaN(ap[0]  )  ?(byte)1:(byte)0;
                            cp[1] =  Double.IsNaN(ap[1]  )  ?(byte)1:(byte)0;
                            cp[2] =  Double.IsNaN(ap[2]  )  ?(byte)1:(byte)0;
                            cp[3] =  Double.IsNaN(ap[3]  )  ?(byte)1:(byte)0;
                            cp[4] =  Double.IsNaN(ap[4]  )  ?(byte)1:(byte)0;
                            cp[5] =  Double.IsNaN(ap[5]  )  ?(byte)1:(byte)0;
                            cp[6] =  Double.IsNaN(ap[6]  )  ?(byte)1:(byte)0;
                            cp[7] =  Double.IsNaN(ap[7]  )  ?(byte)1:(byte)0;
                            cp[8] =  Double.IsNaN(ap[8]  )  ?(byte)1:(byte)0;
                            cp[9] =  Double.IsNaN(ap[9]  )  ?(byte)1:(byte)0;
                            cp[10] =  Double.IsNaN(ap[10]  )  ?(byte)1:(byte)0;
                            cp[11] =  Double.IsNaN(ap[11]  )  ?(byte)1:(byte)0;
                            cp[12] =  Double.IsNaN(ap[12]  )  ?(byte)1:(byte)0;
                            cp[13] =  Double.IsNaN(ap[13]  )  ?(byte)1:(byte)0;
                            cp[14] =  Double.IsNaN(ap[14]  )  ?(byte)1:(byte)0;
                            cp[15] =  Double.IsNaN(ap[15]  )  ?(byte)1:(byte)0;
                            cp[16] =  Double.IsNaN(ap[16]  )  ?(byte)1:(byte)0;
                            cp[17] =  Double.IsNaN(ap[17]  )  ?(byte)1:(byte)0;
                            cp[18] =  Double.IsNaN(ap[18]  )  ?(byte)1:(byte)0;
                            cp[19] =  Double.IsNaN(ap[19]  )  ?(byte)1:(byte)0;
                            cp[20] =  Double.IsNaN(ap[20]  )  ?(byte)1:(byte)0;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Double.IsNaN(*ap  )  ?(byte)1:(byte)0;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( double* arrAP = arrA)
                fixed ( byte* retArrP = retArr) {
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
                return new  ILRetLogical(retStorage);
            }
        }
        /// <summary>Finds invalid value elements (not-a-number)</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is invalid ("Not-a-Number"), 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isnan (ILInArray< float > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size); 
                ILSize inDim = A.Size;
                float[] arrA = A.GetArrayForRead(); 
                byte [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true){
                    retArr = ILMemoryPool.Pool.New<byte>(outLen);
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
                ILDenseStorage<byte> retStorage = new ILDenseStorage<byte>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    byte* cp = ((byte*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  Single.IsNaN(cp[0]  )  ?(byte)1:(byte)0;
                            cp[1] =  Single.IsNaN(cp[1]  )  ?(byte)1:(byte)0;
                            cp[2] =  Single.IsNaN(cp[2]  )  ?(byte)1:(byte)0;
                            cp[3] =  Single.IsNaN(cp[3]  )  ?(byte)1:(byte)0;
                            cp[4] =  Single.IsNaN(cp[4]  )  ?(byte)1:(byte)0;
                            cp[5] =  Single.IsNaN(cp[5]  )  ?(byte)1:(byte)0;
                            cp[6] =  Single.IsNaN(cp[6]  )  ?(byte)1:(byte)0;
                            cp[7] =  Single.IsNaN(cp[7]  )  ?(byte)1:(byte)0;
                            cp[8] =  Single.IsNaN(cp[8]  )  ?(byte)1:(byte)0;
                            cp[9] =  Single.IsNaN(cp[9]  )  ?(byte)1:(byte)0;
                            cp[10] =  Single.IsNaN(cp[10]  )  ?(byte)1:(byte)0;
                            cp[11] =  Single.IsNaN(cp[11]  )  ?(byte)1:(byte)0;
                            cp[12] =  Single.IsNaN(cp[12]  )  ?(byte)1:(byte)0;
                            cp[13] =  Single.IsNaN(cp[13]  )  ?(byte)1:(byte)0;
                            cp[14] =  Single.IsNaN(cp[14]  )  ?(byte)1:(byte)0;
                            cp[15] =  Single.IsNaN(cp[15]  )  ?(byte)1:(byte)0;
                            cp[16] =  Single.IsNaN(cp[16]  )  ?(byte)1:(byte)0;
                            cp[17] =  Single.IsNaN(cp[17]  )  ?(byte)1:(byte)0;
                            cp[18] =  Single.IsNaN(cp[18]  )  ?(byte)1:(byte)0;
                            cp[19] =  Single.IsNaN(cp[19]  )  ?(byte)1:(byte)0;
                            cp[20] =  Single.IsNaN(cp[20]  )  ?(byte)1:(byte)0;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Single.IsNaN(*cp  )  ?(byte)1:(byte)0;
                            cp++;
                        }
                    } else {
                        float* ap = ((float*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Single.IsNaN(ap[0]  )  ?(byte)1:(byte)0;
                            cp[1] =  Single.IsNaN(ap[1]  )  ?(byte)1:(byte)0;
                            cp[2] =  Single.IsNaN(ap[2]  )  ?(byte)1:(byte)0;
                            cp[3] =  Single.IsNaN(ap[3]  )  ?(byte)1:(byte)0;
                            cp[4] =  Single.IsNaN(ap[4]  )  ?(byte)1:(byte)0;
                            cp[5] =  Single.IsNaN(ap[5]  )  ?(byte)1:(byte)0;
                            cp[6] =  Single.IsNaN(ap[6]  )  ?(byte)1:(byte)0;
                            cp[7] =  Single.IsNaN(ap[7]  )  ?(byte)1:(byte)0;
                            cp[8] =  Single.IsNaN(ap[8]  )  ?(byte)1:(byte)0;
                            cp[9] =  Single.IsNaN(ap[9]  )  ?(byte)1:(byte)0;
                            cp[10] =  Single.IsNaN(ap[10]  )  ?(byte)1:(byte)0;
                            cp[11] =  Single.IsNaN(ap[11]  )  ?(byte)1:(byte)0;
                            cp[12] =  Single.IsNaN(ap[12]  )  ?(byte)1:(byte)0;
                            cp[13] =  Single.IsNaN(ap[13]  )  ?(byte)1:(byte)0;
                            cp[14] =  Single.IsNaN(ap[14]  )  ?(byte)1:(byte)0;
                            cp[15] =  Single.IsNaN(ap[15]  )  ?(byte)1:(byte)0;
                            cp[16] =  Single.IsNaN(ap[16]  )  ?(byte)1:(byte)0;
                            cp[17] =  Single.IsNaN(ap[17]  )  ?(byte)1:(byte)0;
                            cp[18] =  Single.IsNaN(ap[18]  )  ?(byte)1:(byte)0;
                            cp[19] =  Single.IsNaN(ap[19]  )  ?(byte)1:(byte)0;
                            cp[20] =  Single.IsNaN(ap[20]  )  ?(byte)1:(byte)0;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Single.IsNaN(*ap  )  ?(byte)1:(byte)0;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( float* arrAP = arrA)
                fixed ( byte* retArrP = retArr) {
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
                return new  ILRetLogical(retStorage);
            }
        }
        /// <summary>Finds invalid value elements (not-a-number)</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is invalid ("Not-a-Number"), 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isnan (ILInArray< fcomplex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size); 
                ILSize inDim = A.Size;
                fcomplex[] arrA = A.GetArrayForRead(); 
                byte [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true){
                    retArr = ILMemoryPool.Pool.New<byte>(outLen);
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
                ILDenseStorage<byte> retStorage = new ILDenseStorage<byte>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    byte* cp = ((byte*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  fcomplex.IsNaN(cp[0]  )  ?(byte)1:(byte)0;
                            cp[1] =  fcomplex.IsNaN(cp[1]  )  ?(byte)1:(byte)0;
                            cp[2] =  fcomplex.IsNaN(cp[2]  )  ?(byte)1:(byte)0;
                            cp[3] =  fcomplex.IsNaN(cp[3]  )  ?(byte)1:(byte)0;
                            cp[4] =  fcomplex.IsNaN(cp[4]  )  ?(byte)1:(byte)0;
                            cp[5] =  fcomplex.IsNaN(cp[5]  )  ?(byte)1:(byte)0;
                            cp[6] =  fcomplex.IsNaN(cp[6]  )  ?(byte)1:(byte)0;
                            cp[7] =  fcomplex.IsNaN(cp[7]  )  ?(byte)1:(byte)0;
                            cp[8] =  fcomplex.IsNaN(cp[8]  )  ?(byte)1:(byte)0;
                            cp[9] =  fcomplex.IsNaN(cp[9]  )  ?(byte)1:(byte)0;
                            cp[10] =  fcomplex.IsNaN(cp[10]  )  ?(byte)1:(byte)0;
                            cp[11] =  fcomplex.IsNaN(cp[11]  )  ?(byte)1:(byte)0;
                            cp[12] =  fcomplex.IsNaN(cp[12]  )  ?(byte)1:(byte)0;
                            cp[13] =  fcomplex.IsNaN(cp[13]  )  ?(byte)1:(byte)0;
                            cp[14] =  fcomplex.IsNaN(cp[14]  )  ?(byte)1:(byte)0;
                            cp[15] =  fcomplex.IsNaN(cp[15]  )  ?(byte)1:(byte)0;
                            cp[16] =  fcomplex.IsNaN(cp[16]  )  ?(byte)1:(byte)0;
                            cp[17] =  fcomplex.IsNaN(cp[17]  )  ?(byte)1:(byte)0;
                            cp[18] =  fcomplex.IsNaN(cp[18]  )  ?(byte)1:(byte)0;
                            cp[19] =  fcomplex.IsNaN(cp[19]  )  ?(byte)1:(byte)0;
                            cp[20] =  fcomplex.IsNaN(cp[20]  )  ?(byte)1:(byte)0;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.IsNaN(*cp  )  ?(byte)1:(byte)0;
                            cp++;
                        }
                    } else {
                        fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  fcomplex.IsNaN(ap[0]  )  ?(byte)1:(byte)0;
                            cp[1] =  fcomplex.IsNaN(ap[1]  )  ?(byte)1:(byte)0;
                            cp[2] =  fcomplex.IsNaN(ap[2]  )  ?(byte)1:(byte)0;
                            cp[3] =  fcomplex.IsNaN(ap[3]  )  ?(byte)1:(byte)0;
                            cp[4] =  fcomplex.IsNaN(ap[4]  )  ?(byte)1:(byte)0;
                            cp[5] =  fcomplex.IsNaN(ap[5]  )  ?(byte)1:(byte)0;
                            cp[6] =  fcomplex.IsNaN(ap[6]  )  ?(byte)1:(byte)0;
                            cp[7] =  fcomplex.IsNaN(ap[7]  )  ?(byte)1:(byte)0;
                            cp[8] =  fcomplex.IsNaN(ap[8]  )  ?(byte)1:(byte)0;
                            cp[9] =  fcomplex.IsNaN(ap[9]  )  ?(byte)1:(byte)0;
                            cp[10] =  fcomplex.IsNaN(ap[10]  )  ?(byte)1:(byte)0;
                            cp[11] =  fcomplex.IsNaN(ap[11]  )  ?(byte)1:(byte)0;
                            cp[12] =  fcomplex.IsNaN(ap[12]  )  ?(byte)1:(byte)0;
                            cp[13] =  fcomplex.IsNaN(ap[13]  )  ?(byte)1:(byte)0;
                            cp[14] =  fcomplex.IsNaN(ap[14]  )  ?(byte)1:(byte)0;
                            cp[15] =  fcomplex.IsNaN(ap[15]  )  ?(byte)1:(byte)0;
                            cp[16] =  fcomplex.IsNaN(ap[16]  )  ?(byte)1:(byte)0;
                            cp[17] =  fcomplex.IsNaN(ap[17]  )  ?(byte)1:(byte)0;
                            cp[18] =  fcomplex.IsNaN(ap[18]  )  ?(byte)1:(byte)0;
                            cp[19] =  fcomplex.IsNaN(ap[19]  )  ?(byte)1:(byte)0;
                            cp[20] =  fcomplex.IsNaN(ap[20]  )  ?(byte)1:(byte)0;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.IsNaN(*ap  )  ?(byte)1:(byte)0;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( fcomplex* arrAP = arrA)
                fixed ( byte* retArrP = retArr) {
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
                return new  ILRetLogical(retStorage);
            }
        }
        /// <summary>Finds invalid value elements (not-a-number)</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is invalid ("Not-a-Number"), 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isnan (ILInArray< complex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size); 
                ILSize inDim = A.Size;
                complex[] arrA = A.GetArrayForRead(); 
                byte [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true){
                    retArr = ILMemoryPool.Pool.New<byte>(outLen);
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
                ILDenseStorage<byte> retStorage = new ILDenseStorage<byte>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    byte* cp = ((byte*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  complex.IsNaN(cp[0]  )  ?(byte)1:(byte)0;
                            cp[1] =  complex.IsNaN(cp[1]  )  ?(byte)1:(byte)0;
                            cp[2] =  complex.IsNaN(cp[2]  )  ?(byte)1:(byte)0;
                            cp[3] =  complex.IsNaN(cp[3]  )  ?(byte)1:(byte)0;
                            cp[4] =  complex.IsNaN(cp[4]  )  ?(byte)1:(byte)0;
                            cp[5] =  complex.IsNaN(cp[5]  )  ?(byte)1:(byte)0;
                            cp[6] =  complex.IsNaN(cp[6]  )  ?(byte)1:(byte)0;
                            cp[7] =  complex.IsNaN(cp[7]  )  ?(byte)1:(byte)0;
                            cp[8] =  complex.IsNaN(cp[8]  )  ?(byte)1:(byte)0;
                            cp[9] =  complex.IsNaN(cp[9]  )  ?(byte)1:(byte)0;
                            cp[10] =  complex.IsNaN(cp[10]  )  ?(byte)1:(byte)0;
                            cp[11] =  complex.IsNaN(cp[11]  )  ?(byte)1:(byte)0;
                            cp[12] =  complex.IsNaN(cp[12]  )  ?(byte)1:(byte)0;
                            cp[13] =  complex.IsNaN(cp[13]  )  ?(byte)1:(byte)0;
                            cp[14] =  complex.IsNaN(cp[14]  )  ?(byte)1:(byte)0;
                            cp[15] =  complex.IsNaN(cp[15]  )  ?(byte)1:(byte)0;
                            cp[16] =  complex.IsNaN(cp[16]  )  ?(byte)1:(byte)0;
                            cp[17] =  complex.IsNaN(cp[17]  )  ?(byte)1:(byte)0;
                            cp[18] =  complex.IsNaN(cp[18]  )  ?(byte)1:(byte)0;
                            cp[19] =  complex.IsNaN(cp[19]  )  ?(byte)1:(byte)0;
                            cp[20] =  complex.IsNaN(cp[20]  )  ?(byte)1:(byte)0;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.IsNaN(*cp  )  ?(byte)1:(byte)0;
                            cp++;
                        }
                    } else {
                        complex* ap = ((complex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  complex.IsNaN(ap[0]  )  ?(byte)1:(byte)0;
                            cp[1] =  complex.IsNaN(ap[1]  )  ?(byte)1:(byte)0;
                            cp[2] =  complex.IsNaN(ap[2]  )  ?(byte)1:(byte)0;
                            cp[3] =  complex.IsNaN(ap[3]  )  ?(byte)1:(byte)0;
                            cp[4] =  complex.IsNaN(ap[4]  )  ?(byte)1:(byte)0;
                            cp[5] =  complex.IsNaN(ap[5]  )  ?(byte)1:(byte)0;
                            cp[6] =  complex.IsNaN(ap[6]  )  ?(byte)1:(byte)0;
                            cp[7] =  complex.IsNaN(ap[7]  )  ?(byte)1:(byte)0;
                            cp[8] =  complex.IsNaN(ap[8]  )  ?(byte)1:(byte)0;
                            cp[9] =  complex.IsNaN(ap[9]  )  ?(byte)1:(byte)0;
                            cp[10] =  complex.IsNaN(ap[10]  )  ?(byte)1:(byte)0;
                            cp[11] =  complex.IsNaN(ap[11]  )  ?(byte)1:(byte)0;
                            cp[12] =  complex.IsNaN(ap[12]  )  ?(byte)1:(byte)0;
                            cp[13] =  complex.IsNaN(ap[13]  )  ?(byte)1:(byte)0;
                            cp[14] =  complex.IsNaN(ap[14]  )  ?(byte)1:(byte)0;
                            cp[15] =  complex.IsNaN(ap[15]  )  ?(byte)1:(byte)0;
                            cp[16] =  complex.IsNaN(ap[16]  )  ?(byte)1:(byte)0;
                            cp[17] =  complex.IsNaN(ap[17]  )  ?(byte)1:(byte)0;
                            cp[18] =  complex.IsNaN(ap[18]  )  ?(byte)1:(byte)0;
                            cp[19] =  complex.IsNaN(ap[19]  )  ?(byte)1:(byte)0;
                            cp[20] =  complex.IsNaN(ap[20]  )  ?(byte)1:(byte)0;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.IsNaN(*ap  )  ?(byte)1:(byte)0;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( complex* arrAP = arrA)
                fixed ( byte* retArrP = retArr) {
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
                return new  ILRetLogical(retStorage);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}