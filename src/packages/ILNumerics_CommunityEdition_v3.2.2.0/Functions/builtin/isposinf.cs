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

        /// <summary>Finds positive infinite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is positive infinite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isposinf (ILInArray< double > A) {
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
                            cp[0] =  Double.IsPositiveInfinity(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  Double.IsPositiveInfinity(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  Double.IsPositiveInfinity(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  Double.IsPositiveInfinity(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  Double.IsPositiveInfinity(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  Double.IsPositiveInfinity(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  Double.IsPositiveInfinity(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  Double.IsPositiveInfinity(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  Double.IsPositiveInfinity(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  Double.IsPositiveInfinity(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  Double.IsPositiveInfinity(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  Double.IsPositiveInfinity(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  Double.IsPositiveInfinity(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  Double.IsPositiveInfinity(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  Double.IsPositiveInfinity(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  Double.IsPositiveInfinity(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  Double.IsPositiveInfinity(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  Double.IsPositiveInfinity(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  Double.IsPositiveInfinity(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  Double.IsPositiveInfinity(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  Double.IsPositiveInfinity(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Double.IsPositiveInfinity(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        double* ap = ((double*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Double.IsPositiveInfinity(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  Double.IsPositiveInfinity(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  Double.IsPositiveInfinity(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  Double.IsPositiveInfinity(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  Double.IsPositiveInfinity(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  Double.IsPositiveInfinity(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  Double.IsPositiveInfinity(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  Double.IsPositiveInfinity(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  Double.IsPositiveInfinity(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  Double.IsPositiveInfinity(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  Double.IsPositiveInfinity(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  Double.IsPositiveInfinity(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  Double.IsPositiveInfinity(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  Double.IsPositiveInfinity(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  Double.IsPositiveInfinity(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  Double.IsPositiveInfinity(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  Double.IsPositiveInfinity(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  Double.IsPositiveInfinity(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  Double.IsPositiveInfinity(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  Double.IsPositiveInfinity(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  Double.IsPositiveInfinity(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Double.IsPositiveInfinity(*ap  )  ?(byte)1:(byte)0;;
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
        /// <summary>Finds positive infinite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is positive infinite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isposinf (ILInArray< float > A) {
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
                            cp[0] =  Single.IsPositiveInfinity(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  Single.IsPositiveInfinity(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  Single.IsPositiveInfinity(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  Single.IsPositiveInfinity(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  Single.IsPositiveInfinity(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  Single.IsPositiveInfinity(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  Single.IsPositiveInfinity(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  Single.IsPositiveInfinity(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  Single.IsPositiveInfinity(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  Single.IsPositiveInfinity(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  Single.IsPositiveInfinity(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  Single.IsPositiveInfinity(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  Single.IsPositiveInfinity(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  Single.IsPositiveInfinity(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  Single.IsPositiveInfinity(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  Single.IsPositiveInfinity(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  Single.IsPositiveInfinity(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  Single.IsPositiveInfinity(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  Single.IsPositiveInfinity(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  Single.IsPositiveInfinity(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  Single.IsPositiveInfinity(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Single.IsPositiveInfinity(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        float* ap = ((float*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Single.IsPositiveInfinity(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  Single.IsPositiveInfinity(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  Single.IsPositiveInfinity(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  Single.IsPositiveInfinity(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  Single.IsPositiveInfinity(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  Single.IsPositiveInfinity(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  Single.IsPositiveInfinity(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  Single.IsPositiveInfinity(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  Single.IsPositiveInfinity(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  Single.IsPositiveInfinity(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  Single.IsPositiveInfinity(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  Single.IsPositiveInfinity(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  Single.IsPositiveInfinity(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  Single.IsPositiveInfinity(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  Single.IsPositiveInfinity(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  Single.IsPositiveInfinity(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  Single.IsPositiveInfinity(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  Single.IsPositiveInfinity(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  Single.IsPositiveInfinity(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  Single.IsPositiveInfinity(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  Single.IsPositiveInfinity(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Single.IsPositiveInfinity(*ap  )  ?(byte)1:(byte)0;;
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
        /// <summary>Finds positive infinite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is positive infinite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isposinf (ILInArray< fcomplex > A) {
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
                            cp[0] =  fcomplex.IsPositiveInfinity(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  fcomplex.IsPositiveInfinity(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  fcomplex.IsPositiveInfinity(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  fcomplex.IsPositiveInfinity(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  fcomplex.IsPositiveInfinity(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  fcomplex.IsPositiveInfinity(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  fcomplex.IsPositiveInfinity(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  fcomplex.IsPositiveInfinity(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  fcomplex.IsPositiveInfinity(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  fcomplex.IsPositiveInfinity(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  fcomplex.IsPositiveInfinity(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  fcomplex.IsPositiveInfinity(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  fcomplex.IsPositiveInfinity(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  fcomplex.IsPositiveInfinity(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  fcomplex.IsPositiveInfinity(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  fcomplex.IsPositiveInfinity(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  fcomplex.IsPositiveInfinity(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  fcomplex.IsPositiveInfinity(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  fcomplex.IsPositiveInfinity(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  fcomplex.IsPositiveInfinity(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  fcomplex.IsPositiveInfinity(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.IsPositiveInfinity(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  fcomplex.IsPositiveInfinity(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  fcomplex.IsPositiveInfinity(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  fcomplex.IsPositiveInfinity(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  fcomplex.IsPositiveInfinity(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  fcomplex.IsPositiveInfinity(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  fcomplex.IsPositiveInfinity(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  fcomplex.IsPositiveInfinity(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  fcomplex.IsPositiveInfinity(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  fcomplex.IsPositiveInfinity(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  fcomplex.IsPositiveInfinity(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  fcomplex.IsPositiveInfinity(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  fcomplex.IsPositiveInfinity(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  fcomplex.IsPositiveInfinity(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  fcomplex.IsPositiveInfinity(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  fcomplex.IsPositiveInfinity(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  fcomplex.IsPositiveInfinity(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  fcomplex.IsPositiveInfinity(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  fcomplex.IsPositiveInfinity(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  fcomplex.IsPositiveInfinity(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  fcomplex.IsPositiveInfinity(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  fcomplex.IsPositiveInfinity(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.IsPositiveInfinity(*ap  )  ?(byte)1:(byte)0;;
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
        /// <summary>Finds positive infinite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is positive infinite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isposinf (ILInArray< complex > A) {
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
                            cp[0] =  complex.IsPositiveInfinity(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  complex.IsPositiveInfinity(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  complex.IsPositiveInfinity(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  complex.IsPositiveInfinity(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  complex.IsPositiveInfinity(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  complex.IsPositiveInfinity(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  complex.IsPositiveInfinity(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  complex.IsPositiveInfinity(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  complex.IsPositiveInfinity(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  complex.IsPositiveInfinity(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  complex.IsPositiveInfinity(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  complex.IsPositiveInfinity(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  complex.IsPositiveInfinity(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  complex.IsPositiveInfinity(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  complex.IsPositiveInfinity(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  complex.IsPositiveInfinity(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  complex.IsPositiveInfinity(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  complex.IsPositiveInfinity(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  complex.IsPositiveInfinity(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  complex.IsPositiveInfinity(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  complex.IsPositiveInfinity(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.IsPositiveInfinity(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        complex* ap = ((complex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  complex.IsPositiveInfinity(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  complex.IsPositiveInfinity(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  complex.IsPositiveInfinity(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  complex.IsPositiveInfinity(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  complex.IsPositiveInfinity(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  complex.IsPositiveInfinity(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  complex.IsPositiveInfinity(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  complex.IsPositiveInfinity(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  complex.IsPositiveInfinity(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  complex.IsPositiveInfinity(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  complex.IsPositiveInfinity(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  complex.IsPositiveInfinity(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  complex.IsPositiveInfinity(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  complex.IsPositiveInfinity(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  complex.IsPositiveInfinity(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  complex.IsPositiveInfinity(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  complex.IsPositiveInfinity(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  complex.IsPositiveInfinity(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  complex.IsPositiveInfinity(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  complex.IsPositiveInfinity(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  complex.IsPositiveInfinity(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.IsPositiveInfinity(*ap  )  ?(byte)1:(byte)0;;
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