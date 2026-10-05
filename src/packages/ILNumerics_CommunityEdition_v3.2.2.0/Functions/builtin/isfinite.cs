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
        /// <summary>
        /// Find out, if value is finite
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>True for finite values</returns>
        internal static bool isfinite(double input) {
            if (!double.IsInfinity(input) && !double.IsNaN(input))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Find out, if value is finite
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>True for finite values</returns>
        internal static bool isfinite(complex input) {
            if (!complex.IsInfinity(input) && !complex.IsNaN(input))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Find out, if value is finite
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>True for finite values</returns>
        internal static bool isfinite(float input) {
            if (!float.IsInfinity(input) && !float.IsNaN(input))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Find out, if value is finite
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>True for finite values</returns>
        internal static bool isfinite(fcomplex input) {
            if (!fcomplex.IsInfinity(input) && !fcomplex.IsNaN(input))
                return true;
            else
                return false;
        }




#region HYCALPER AUTO GENERATED CODE

        /// <summary>Finds finite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is finite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isfinite (ILInArray< double > A) {
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
                            cp[0] =  isfinite(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  isfinite(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  isfinite(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  isfinite(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  isfinite(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  isfinite(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  isfinite(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  isfinite(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  isfinite(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  isfinite(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  isfinite(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  isfinite(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  isfinite(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  isfinite(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  isfinite(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  isfinite(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  isfinite(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  isfinite(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  isfinite(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  isfinite(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  isfinite(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  isfinite(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        double* ap = ((double*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  isfinite(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  isfinite(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  isfinite(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  isfinite(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  isfinite(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  isfinite(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  isfinite(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  isfinite(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  isfinite(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  isfinite(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  isfinite(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  isfinite(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  isfinite(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  isfinite(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  isfinite(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  isfinite(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  isfinite(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  isfinite(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  isfinite(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  isfinite(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  isfinite(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  isfinite(*ap  )  ?(byte)1:(byte)0;;
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
        /// <summary>Finds finite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is finite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isfinite (ILInArray< float > A) {
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
                            cp[0] =  isfinite(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  isfinite(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  isfinite(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  isfinite(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  isfinite(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  isfinite(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  isfinite(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  isfinite(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  isfinite(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  isfinite(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  isfinite(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  isfinite(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  isfinite(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  isfinite(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  isfinite(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  isfinite(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  isfinite(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  isfinite(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  isfinite(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  isfinite(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  isfinite(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  isfinite(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        float* ap = ((float*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  isfinite(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  isfinite(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  isfinite(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  isfinite(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  isfinite(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  isfinite(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  isfinite(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  isfinite(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  isfinite(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  isfinite(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  isfinite(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  isfinite(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  isfinite(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  isfinite(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  isfinite(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  isfinite(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  isfinite(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  isfinite(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  isfinite(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  isfinite(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  isfinite(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  isfinite(*ap  )  ?(byte)1:(byte)0;;
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
        /// <summary>Finds finite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is finite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isfinite (ILInArray< fcomplex > A) {
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
                            cp[0] =  fcomplex.IsFinite(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  fcomplex.IsFinite(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  fcomplex.IsFinite(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  fcomplex.IsFinite(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  fcomplex.IsFinite(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  fcomplex.IsFinite(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  fcomplex.IsFinite(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  fcomplex.IsFinite(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  fcomplex.IsFinite(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  fcomplex.IsFinite(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  fcomplex.IsFinite(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  fcomplex.IsFinite(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  fcomplex.IsFinite(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  fcomplex.IsFinite(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  fcomplex.IsFinite(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  fcomplex.IsFinite(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  fcomplex.IsFinite(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  fcomplex.IsFinite(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  fcomplex.IsFinite(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  fcomplex.IsFinite(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  fcomplex.IsFinite(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.IsFinite(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  fcomplex.IsFinite(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  fcomplex.IsFinite(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  fcomplex.IsFinite(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  fcomplex.IsFinite(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  fcomplex.IsFinite(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  fcomplex.IsFinite(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  fcomplex.IsFinite(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  fcomplex.IsFinite(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  fcomplex.IsFinite(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  fcomplex.IsFinite(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  fcomplex.IsFinite(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  fcomplex.IsFinite(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  fcomplex.IsFinite(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  fcomplex.IsFinite(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  fcomplex.IsFinite(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  fcomplex.IsFinite(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  fcomplex.IsFinite(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  fcomplex.IsFinite(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  fcomplex.IsFinite(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  fcomplex.IsFinite(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  fcomplex.IsFinite(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.IsFinite(*ap  )  ?(byte)1:(byte)0;;
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
        /// <summary>Finds finite value elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logical array with 1 if the corresponding elements of input array is finite, 0 else.</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetLogical  isfinite (ILInArray< complex > A) {
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
                            cp[0] =  complex.IsFinite(cp[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  complex.IsFinite(cp[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  complex.IsFinite(cp[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  complex.IsFinite(cp[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  complex.IsFinite(cp[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  complex.IsFinite(cp[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  complex.IsFinite(cp[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  complex.IsFinite(cp[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  complex.IsFinite(cp[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  complex.IsFinite(cp[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  complex.IsFinite(cp[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  complex.IsFinite(cp[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  complex.IsFinite(cp[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  complex.IsFinite(cp[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  complex.IsFinite(cp[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  complex.IsFinite(cp[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  complex.IsFinite(cp[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  complex.IsFinite(cp[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  complex.IsFinite(cp[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  complex.IsFinite(cp[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  complex.IsFinite(cp[20]  )  ?(byte)1:(byte)0;;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.IsFinite(*cp  )  ?(byte)1:(byte)0;;
                            cp++;
                        }
                    } else {
                        complex* ap = ((complex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  complex.IsFinite(ap[0]  )  ?(byte)1:(byte)0;;
                            cp[1] =  complex.IsFinite(ap[1]  )  ?(byte)1:(byte)0;;
                            cp[2] =  complex.IsFinite(ap[2]  )  ?(byte)1:(byte)0;;
                            cp[3] =  complex.IsFinite(ap[3]  )  ?(byte)1:(byte)0;;
                            cp[4] =  complex.IsFinite(ap[4]  )  ?(byte)1:(byte)0;;
                            cp[5] =  complex.IsFinite(ap[5]  )  ?(byte)1:(byte)0;;
                            cp[6] =  complex.IsFinite(ap[6]  )  ?(byte)1:(byte)0;;
                            cp[7] =  complex.IsFinite(ap[7]  )  ?(byte)1:(byte)0;;
                            cp[8] =  complex.IsFinite(ap[8]  )  ?(byte)1:(byte)0;;
                            cp[9] =  complex.IsFinite(ap[9]  )  ?(byte)1:(byte)0;;
                            cp[10] =  complex.IsFinite(ap[10]  )  ?(byte)1:(byte)0;;
                            cp[11] =  complex.IsFinite(ap[11]  )  ?(byte)1:(byte)0;;
                            cp[12] =  complex.IsFinite(ap[12]  )  ?(byte)1:(byte)0;;
                            cp[13] =  complex.IsFinite(ap[13]  )  ?(byte)1:(byte)0;;
                            cp[14] =  complex.IsFinite(ap[14]  )  ?(byte)1:(byte)0;;
                            cp[15] =  complex.IsFinite(ap[15]  )  ?(byte)1:(byte)0;;
                            cp[16] =  complex.IsFinite(ap[16]  )  ?(byte)1:(byte)0;;
                            cp[17] =  complex.IsFinite(ap[17]  )  ?(byte)1:(byte)0;;
                            cp[18] =  complex.IsFinite(ap[18]  )  ?(byte)1:(byte)0;;
                            cp[19] =  complex.IsFinite(ap[19]  )  ?(byte)1:(byte)0;;
                            cp[20] =  complex.IsFinite(ap[20]  )  ?(byte)1:(byte)0;;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.IsFinite(*ap  )  ?(byte)1:(byte)0;;
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