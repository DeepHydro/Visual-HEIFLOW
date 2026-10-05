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

        /// <summary>Locate infinite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is infinite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isinf (ILInArray< double > A) {
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
                            cp[0] =  Double.IsInfinity(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  Double.IsInfinity(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  Double.IsInfinity(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  Double.IsInfinity(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  Double.IsInfinity(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  Double.IsInfinity(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  Double.IsInfinity(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  Double.IsInfinity(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  Double.IsInfinity(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  Double.IsInfinity(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  Double.IsInfinity(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  Double.IsInfinity(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  Double.IsInfinity(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  Double.IsInfinity(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  Double.IsInfinity(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  Double.IsInfinity(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  Double.IsInfinity(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  Double.IsInfinity(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  Double.IsInfinity(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  Double.IsInfinity(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  Double.IsInfinity(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Double.IsInfinity(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        double* ap = ((double*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Double.IsInfinity(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  Double.IsInfinity(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  Double.IsInfinity(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  Double.IsInfinity(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  Double.IsInfinity(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  Double.IsInfinity(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  Double.IsInfinity(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  Double.IsInfinity(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  Double.IsInfinity(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  Double.IsInfinity(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  Double.IsInfinity(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  Double.IsInfinity(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  Double.IsInfinity(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  Double.IsInfinity(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  Double.IsInfinity(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  Double.IsInfinity(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  Double.IsInfinity(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  Double.IsInfinity(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  Double.IsInfinity(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  Double.IsInfinity(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  Double.IsInfinity(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Double.IsInfinity(*ap  )  ?(byte)1:(byte)0;;
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
        /// <summary>Locate infinite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is infinite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isinf (ILInArray< float > A) {
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
                            cp[0] =  Single.IsInfinity(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  Single.IsInfinity(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  Single.IsInfinity(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  Single.IsInfinity(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  Single.IsInfinity(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  Single.IsInfinity(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  Single.IsInfinity(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  Single.IsInfinity(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  Single.IsInfinity(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  Single.IsInfinity(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  Single.IsInfinity(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  Single.IsInfinity(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  Single.IsInfinity(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  Single.IsInfinity(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  Single.IsInfinity(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  Single.IsInfinity(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  Single.IsInfinity(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  Single.IsInfinity(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  Single.IsInfinity(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  Single.IsInfinity(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  Single.IsInfinity(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Single.IsInfinity(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        float* ap = ((float*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Single.IsInfinity(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  Single.IsInfinity(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  Single.IsInfinity(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  Single.IsInfinity(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  Single.IsInfinity(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  Single.IsInfinity(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  Single.IsInfinity(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  Single.IsInfinity(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  Single.IsInfinity(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  Single.IsInfinity(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  Single.IsInfinity(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  Single.IsInfinity(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  Single.IsInfinity(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  Single.IsInfinity(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  Single.IsInfinity(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  Single.IsInfinity(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  Single.IsInfinity(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  Single.IsInfinity(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  Single.IsInfinity(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  Single.IsInfinity(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  Single.IsInfinity(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Single.IsInfinity(*ap  )  ?(byte)1:(byte)0;;
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
        /// <summary>Locate infinite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is infinite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isinf (ILInArray< fcomplex > A) {
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
                            cp[0] =  fcomplex.IsInfinity(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  fcomplex.IsInfinity(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  fcomplex.IsInfinity(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  fcomplex.IsInfinity(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  fcomplex.IsInfinity(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  fcomplex.IsInfinity(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  fcomplex.IsInfinity(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  fcomplex.IsInfinity(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  fcomplex.IsInfinity(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  fcomplex.IsInfinity(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  fcomplex.IsInfinity(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  fcomplex.IsInfinity(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  fcomplex.IsInfinity(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  fcomplex.IsInfinity(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  fcomplex.IsInfinity(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  fcomplex.IsInfinity(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  fcomplex.IsInfinity(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  fcomplex.IsInfinity(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  fcomplex.IsInfinity(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  fcomplex.IsInfinity(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  fcomplex.IsInfinity(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.IsInfinity(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  fcomplex.IsInfinity(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  fcomplex.IsInfinity(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  fcomplex.IsInfinity(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  fcomplex.IsInfinity(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  fcomplex.IsInfinity(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  fcomplex.IsInfinity(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  fcomplex.IsInfinity(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  fcomplex.IsInfinity(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  fcomplex.IsInfinity(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  fcomplex.IsInfinity(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  fcomplex.IsInfinity(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  fcomplex.IsInfinity(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  fcomplex.IsInfinity(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  fcomplex.IsInfinity(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  fcomplex.IsInfinity(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  fcomplex.IsInfinity(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  fcomplex.IsInfinity(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  fcomplex.IsInfinity(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  fcomplex.IsInfinity(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  fcomplex.IsInfinity(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  fcomplex.IsInfinity(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.IsInfinity(*ap  )  ?(byte)1:(byte)0;;
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
        /// <summary>Locate infinite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is infinite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isinf (ILInArray< complex > A) {
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
                            cp[0] =  complex.IsInfinity(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  complex.IsInfinity(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  complex.IsInfinity(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  complex.IsInfinity(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  complex.IsInfinity(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  complex.IsInfinity(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  complex.IsInfinity(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  complex.IsInfinity(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  complex.IsInfinity(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  complex.IsInfinity(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  complex.IsInfinity(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  complex.IsInfinity(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  complex.IsInfinity(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  complex.IsInfinity(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  complex.IsInfinity(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  complex.IsInfinity(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  complex.IsInfinity(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  complex.IsInfinity(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  complex.IsInfinity(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  complex.IsInfinity(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  complex.IsInfinity(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.IsInfinity(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        complex* ap = ((complex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  complex.IsInfinity(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  complex.IsInfinity(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  complex.IsInfinity(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  complex.IsInfinity(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  complex.IsInfinity(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  complex.IsInfinity(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  complex.IsInfinity(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  complex.IsInfinity(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  complex.IsInfinity(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  complex.IsInfinity(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  complex.IsInfinity(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  complex.IsInfinity(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  complex.IsInfinity(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  complex.IsInfinity(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  complex.IsInfinity(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  complex.IsInfinity(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  complex.IsInfinity(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  complex.IsInfinity(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  complex.IsInfinity(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  complex.IsInfinity(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  complex.IsInfinity(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.IsInfinity(*ap  )  ?(byte)1:(byte)0;;
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