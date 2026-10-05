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

        /// <summary>Finds negative infinite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is negative infinite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isneginf (ILInArray< double > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size); 
                ILSize inDim = A.Size;
                double[] arrA = A.GetArrayForRead(); 
                byte [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true) {
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
                            cp[0] =  Double.IsNegativeInfinity(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  Double.IsNegativeInfinity(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  Double.IsNegativeInfinity(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  Double.IsNegativeInfinity(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  Double.IsNegativeInfinity(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  Double.IsNegativeInfinity(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  Double.IsNegativeInfinity(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  Double.IsNegativeInfinity(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  Double.IsNegativeInfinity(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  Double.IsNegativeInfinity(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  Double.IsNegativeInfinity(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  Double.IsNegativeInfinity(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  Double.IsNegativeInfinity(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  Double.IsNegativeInfinity(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  Double.IsNegativeInfinity(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  Double.IsNegativeInfinity(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  Double.IsNegativeInfinity(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  Double.IsNegativeInfinity(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  Double.IsNegativeInfinity(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  Double.IsNegativeInfinity(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  Double.IsNegativeInfinity(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Double.IsNegativeInfinity(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        double* ap = ((double*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Double.IsNegativeInfinity(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  Double.IsNegativeInfinity(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  Double.IsNegativeInfinity(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  Double.IsNegativeInfinity(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  Double.IsNegativeInfinity(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  Double.IsNegativeInfinity(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  Double.IsNegativeInfinity(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  Double.IsNegativeInfinity(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  Double.IsNegativeInfinity(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  Double.IsNegativeInfinity(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  Double.IsNegativeInfinity(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  Double.IsNegativeInfinity(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  Double.IsNegativeInfinity(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  Double.IsNegativeInfinity(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  Double.IsNegativeInfinity(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  Double.IsNegativeInfinity(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  Double.IsNegativeInfinity(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  Double.IsNegativeInfinity(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  Double.IsNegativeInfinity(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  Double.IsNegativeInfinity(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  Double.IsNegativeInfinity(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Double.IsNegativeInfinity(*ap  )  ?(byte)1:(byte)0;;
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
        /// <summary>Finds negative infinite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is negative infinite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isneginf (ILInArray< float > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size); 
                ILSize inDim = A.Size;
                float[] arrA = A.GetArrayForRead(); 
                byte [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true) {
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
                            cp[0] =  Single.IsNegativeInfinity(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  Single.IsNegativeInfinity(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  Single.IsNegativeInfinity(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  Single.IsNegativeInfinity(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  Single.IsNegativeInfinity(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  Single.IsNegativeInfinity(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  Single.IsNegativeInfinity(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  Single.IsNegativeInfinity(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  Single.IsNegativeInfinity(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  Single.IsNegativeInfinity(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  Single.IsNegativeInfinity(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  Single.IsNegativeInfinity(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  Single.IsNegativeInfinity(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  Single.IsNegativeInfinity(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  Single.IsNegativeInfinity(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  Single.IsNegativeInfinity(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  Single.IsNegativeInfinity(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  Single.IsNegativeInfinity(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  Single.IsNegativeInfinity(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  Single.IsNegativeInfinity(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  Single.IsNegativeInfinity(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Single.IsNegativeInfinity(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        float* ap = ((float*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Single.IsNegativeInfinity(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  Single.IsNegativeInfinity(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  Single.IsNegativeInfinity(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  Single.IsNegativeInfinity(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  Single.IsNegativeInfinity(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  Single.IsNegativeInfinity(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  Single.IsNegativeInfinity(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  Single.IsNegativeInfinity(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  Single.IsNegativeInfinity(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  Single.IsNegativeInfinity(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  Single.IsNegativeInfinity(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  Single.IsNegativeInfinity(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  Single.IsNegativeInfinity(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  Single.IsNegativeInfinity(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  Single.IsNegativeInfinity(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  Single.IsNegativeInfinity(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  Single.IsNegativeInfinity(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  Single.IsNegativeInfinity(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  Single.IsNegativeInfinity(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  Single.IsNegativeInfinity(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  Single.IsNegativeInfinity(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Single.IsNegativeInfinity(*ap  )  ?(byte)1:(byte)0;;
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
        /// <summary>Finds negative infinite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is negative infinite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isneginf (ILInArray< fcomplex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size); 
                ILSize inDim = A.Size;
                fcomplex[] arrA = A.GetArrayForRead(); 
                byte [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true) {
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
                            cp[0] =  fcomplex.IsNegativeInfinity(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  fcomplex.IsNegativeInfinity(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  fcomplex.IsNegativeInfinity(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  fcomplex.IsNegativeInfinity(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  fcomplex.IsNegativeInfinity(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  fcomplex.IsNegativeInfinity(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  fcomplex.IsNegativeInfinity(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  fcomplex.IsNegativeInfinity(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  fcomplex.IsNegativeInfinity(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  fcomplex.IsNegativeInfinity(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  fcomplex.IsNegativeInfinity(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  fcomplex.IsNegativeInfinity(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  fcomplex.IsNegativeInfinity(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  fcomplex.IsNegativeInfinity(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  fcomplex.IsNegativeInfinity(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  fcomplex.IsNegativeInfinity(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  fcomplex.IsNegativeInfinity(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  fcomplex.IsNegativeInfinity(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  fcomplex.IsNegativeInfinity(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  fcomplex.IsNegativeInfinity(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  fcomplex.IsNegativeInfinity(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.IsNegativeInfinity(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  fcomplex.IsNegativeInfinity(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  fcomplex.IsNegativeInfinity(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  fcomplex.IsNegativeInfinity(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  fcomplex.IsNegativeInfinity(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  fcomplex.IsNegativeInfinity(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  fcomplex.IsNegativeInfinity(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  fcomplex.IsNegativeInfinity(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  fcomplex.IsNegativeInfinity(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  fcomplex.IsNegativeInfinity(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  fcomplex.IsNegativeInfinity(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  fcomplex.IsNegativeInfinity(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  fcomplex.IsNegativeInfinity(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  fcomplex.IsNegativeInfinity(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  fcomplex.IsNegativeInfinity(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  fcomplex.IsNegativeInfinity(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  fcomplex.IsNegativeInfinity(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  fcomplex.IsNegativeInfinity(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  fcomplex.IsNegativeInfinity(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  fcomplex.IsNegativeInfinity(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  fcomplex.IsNegativeInfinity(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  fcomplex.IsNegativeInfinity(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.IsNegativeInfinity(*ap  )  ?(byte)1:(byte)0;;
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
        /// <summary>Finds negative infinite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is negative infinite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isneginf (ILInArray< complex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size); 
                ILSize inDim = A.Size;
                complex[] arrA = A.GetArrayForRead(); 
                byte [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true) {
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
                            cp[0] =  complex.IsNegativeInfinity(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  complex.IsNegativeInfinity(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  complex.IsNegativeInfinity(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  complex.IsNegativeInfinity(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  complex.IsNegativeInfinity(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  complex.IsNegativeInfinity(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  complex.IsNegativeInfinity(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  complex.IsNegativeInfinity(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  complex.IsNegativeInfinity(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  complex.IsNegativeInfinity(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  complex.IsNegativeInfinity(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  complex.IsNegativeInfinity(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  complex.IsNegativeInfinity(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  complex.IsNegativeInfinity(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  complex.IsNegativeInfinity(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  complex.IsNegativeInfinity(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  complex.IsNegativeInfinity(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  complex.IsNegativeInfinity(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  complex.IsNegativeInfinity(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  complex.IsNegativeInfinity(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  complex.IsNegativeInfinity(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.IsNegativeInfinity(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        complex* ap = ((complex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  complex.IsNegativeInfinity(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  complex.IsNegativeInfinity(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  complex.IsNegativeInfinity(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  complex.IsNegativeInfinity(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  complex.IsNegativeInfinity(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  complex.IsNegativeInfinity(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  complex.IsNegativeInfinity(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  complex.IsNegativeInfinity(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  complex.IsNegativeInfinity(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  complex.IsNegativeInfinity(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  complex.IsNegativeInfinity(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  complex.IsNegativeInfinity(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  complex.IsNegativeInfinity(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  complex.IsNegativeInfinity(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  complex.IsNegativeInfinity(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  complex.IsNegativeInfinity(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  complex.IsNegativeInfinity(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  complex.IsNegativeInfinity(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  complex.IsNegativeInfinity(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  complex.IsNegativeInfinity(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  complex.IsNegativeInfinity(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.IsNegativeInfinity(*ap  )  ?(byte)1:(byte)0;;
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