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

        /// <summary>Logarithm of array elements - complex output</summary>
        /// <param name="A">Input array</param>
        /// <returns>Complex logarithm of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<fcomplex>  logc (ILInArray< float > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<fcomplex>(A.Size); 
                ILSize inDim = A.Size;
                float[] arrA = A.GetArrayForRead(); 
                fcomplex [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true) {
                    retArr = ILMemoryPool.Pool.New<fcomplex>(outLen);
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
                ILDenseStorage<fcomplex> retStorage = new ILDenseStorage<fcomplex>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    fcomplex* cp = ((fcomplex*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  fcomplex.Log(cp[0]  )  /*dummy*/;
                            cp[1] =  fcomplex.Log(cp[1]  )  /*dummy*/;
                            cp[2] =  fcomplex.Log(cp[2]  )  /*dummy*/;
                            cp[3] =  fcomplex.Log(cp[3]  )  /*dummy*/;
                            cp[4] =  fcomplex.Log(cp[4]  )  /*dummy*/;
                            cp[5] =  fcomplex.Log(cp[5]  )  /*dummy*/;
                            cp[6] =  fcomplex.Log(cp[6]  )  /*dummy*/;
                            cp[7] =  fcomplex.Log(cp[7]  )  /*dummy*/;
                            cp[8] =  fcomplex.Log(cp[8]  )  /*dummy*/;
                            cp[9] =  fcomplex.Log(cp[9]  )  /*dummy*/;
                            cp[10] =  fcomplex.Log(cp[10]  )  /*dummy*/;
                            cp[11] =  fcomplex.Log(cp[11]  )  /*dummy*/;
                            cp[12] =  fcomplex.Log(cp[12]  )  /*dummy*/;
                            cp[13] =  fcomplex.Log(cp[13]  )  /*dummy*/;
                            cp[14] =  fcomplex.Log(cp[14]  )  /*dummy*/;
                            cp[15] =  fcomplex.Log(cp[15]  )  /*dummy*/;
                            cp[16] =  fcomplex.Log(cp[16]  )  /*dummy*/;
                            cp[17] =  fcomplex.Log(cp[17]  )  /*dummy*/;
                            cp[18] =  fcomplex.Log(cp[18]  )  /*dummy*/;
                            cp[19] =  fcomplex.Log(cp[19]  )  /*dummy*/;
                            cp[20] =  fcomplex.Log(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.Log(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        float* ap = ((float*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  fcomplex.Log(ap[0]  )  /*dummy*/;
                            cp[1] =  fcomplex.Log(ap[1]  )  /*dummy*/;
                            cp[2] =  fcomplex.Log(ap[2]  )  /*dummy*/;
                            cp[3] =  fcomplex.Log(ap[3]  )  /*dummy*/;
                            cp[4] =  fcomplex.Log(ap[4]  )  /*dummy*/;
                            cp[5] =  fcomplex.Log(ap[5]  )  /*dummy*/;
                            cp[6] =  fcomplex.Log(ap[6]  )  /*dummy*/;
                            cp[7] =  fcomplex.Log(ap[7]  )  /*dummy*/;
                            cp[8] =  fcomplex.Log(ap[8]  )  /*dummy*/;
                            cp[9] =  fcomplex.Log(ap[9]  )  /*dummy*/;
                            cp[10] =  fcomplex.Log(ap[10]  )  /*dummy*/;
                            cp[11] =  fcomplex.Log(ap[11]  )  /*dummy*/;
                            cp[12] =  fcomplex.Log(ap[12]  )  /*dummy*/;
                            cp[13] =  fcomplex.Log(ap[13]  )  /*dummy*/;
                            cp[14] =  fcomplex.Log(ap[14]  )  /*dummy*/;
                            cp[15] =  fcomplex.Log(ap[15]  )  /*dummy*/;
                            cp[16] =  fcomplex.Log(ap[16]  )  /*dummy*/;
                            cp[17] =  fcomplex.Log(ap[17]  )  /*dummy*/;
                            cp[18] =  fcomplex.Log(ap[18]  )  /*dummy*/;
                            cp[19] =  fcomplex.Log(ap[19]  )  /*dummy*/;
                            cp[20] =  fcomplex.Log(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.Log(*ap  )  /*dummy*/;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( float* arrAP = arrA)
                fixed ( fcomplex* retArrP = retArr) {
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
                return new  ILRetArray<fcomplex>(retStorage);
            }
        }
        /// <summary>Logarithm of array elements - complex output</summary>
        /// <param name="A">Input array</param>
        /// <returns>Complex logarithm of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<complex>  logc (ILInArray< double > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<complex>(A.Size); 
                ILSize inDim = A.Size;
                double[] arrA = A.GetArrayForRead(); 
                complex [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true) {
                    retArr = ILMemoryPool.Pool.New<complex>(outLen);
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
                ILDenseStorage<complex> retStorage = new ILDenseStorage<complex>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    complex* cp = ((complex*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  complex.Log(cp[0]  )  /*dummy*/;
                            cp[1] =  complex.Log(cp[1]  )  /*dummy*/;
                            cp[2] =  complex.Log(cp[2]  )  /*dummy*/;
                            cp[3] =  complex.Log(cp[3]  )  /*dummy*/;
                            cp[4] =  complex.Log(cp[4]  )  /*dummy*/;
                            cp[5] =  complex.Log(cp[5]  )  /*dummy*/;
                            cp[6] =  complex.Log(cp[6]  )  /*dummy*/;
                            cp[7] =  complex.Log(cp[7]  )  /*dummy*/;
                            cp[8] =  complex.Log(cp[8]  )  /*dummy*/;
                            cp[9] =  complex.Log(cp[9]  )  /*dummy*/;
                            cp[10] =  complex.Log(cp[10]  )  /*dummy*/;
                            cp[11] =  complex.Log(cp[11]  )  /*dummy*/;
                            cp[12] =  complex.Log(cp[12]  )  /*dummy*/;
                            cp[13] =  complex.Log(cp[13]  )  /*dummy*/;
                            cp[14] =  complex.Log(cp[14]  )  /*dummy*/;
                            cp[15] =  complex.Log(cp[15]  )  /*dummy*/;
                            cp[16] =  complex.Log(cp[16]  )  /*dummy*/;
                            cp[17] =  complex.Log(cp[17]  )  /*dummy*/;
                            cp[18] =  complex.Log(cp[18]  )  /*dummy*/;
                            cp[19] =  complex.Log(cp[19]  )  /*dummy*/;
                            cp[20] =  complex.Log(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.Log(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        double* ap = ((double*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  complex.Log(ap[0]  )  /*dummy*/;
                            cp[1] =  complex.Log(ap[1]  )  /*dummy*/;
                            cp[2] =  complex.Log(ap[2]  )  /*dummy*/;
                            cp[3] =  complex.Log(ap[3]  )  /*dummy*/;
                            cp[4] =  complex.Log(ap[4]  )  /*dummy*/;
                            cp[5] =  complex.Log(ap[5]  )  /*dummy*/;
                            cp[6] =  complex.Log(ap[6]  )  /*dummy*/;
                            cp[7] =  complex.Log(ap[7]  )  /*dummy*/;
                            cp[8] =  complex.Log(ap[8]  )  /*dummy*/;
                            cp[9] =  complex.Log(ap[9]  )  /*dummy*/;
                            cp[10] =  complex.Log(ap[10]  )  /*dummy*/;
                            cp[11] =  complex.Log(ap[11]  )  /*dummy*/;
                            cp[12] =  complex.Log(ap[12]  )  /*dummy*/;
                            cp[13] =  complex.Log(ap[13]  )  /*dummy*/;
                            cp[14] =  complex.Log(ap[14]  )  /*dummy*/;
                            cp[15] =  complex.Log(ap[15]  )  /*dummy*/;
                            cp[16] =  complex.Log(ap[16]  )  /*dummy*/;
                            cp[17] =  complex.Log(ap[17]  )  /*dummy*/;
                            cp[18] =  complex.Log(ap[18]  )  /*dummy*/;
                            cp[19] =  complex.Log(ap[19]  )  /*dummy*/;
                            cp[20] =  complex.Log(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.Log(*ap  )  /*dummy*/;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( double* arrAP = arrA)
                fixed ( complex* retArrP = retArr) {
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
                return new  ILRetArray<complex>(retStorage);
            }
        }
        /// <summary>Logarithm of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Complex logarithm of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<complex>  log (ILInArray< complex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<complex>(A.Size); 
                ILSize inDim = A.Size;
                complex[] arrA = A.GetArrayForRead(); 
                complex [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true) {
                    retArr = ILMemoryPool.Pool.New<complex>(outLen);
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
                ILDenseStorage<complex> retStorage = new ILDenseStorage<complex>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    complex* cp = ((complex*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  complex.Log(cp[0]  )  /*dummy*/;
                            cp[1] =  complex.Log(cp[1]  )  /*dummy*/;
                            cp[2] =  complex.Log(cp[2]  )  /*dummy*/;
                            cp[3] =  complex.Log(cp[3]  )  /*dummy*/;
                            cp[4] =  complex.Log(cp[4]  )  /*dummy*/;
                            cp[5] =  complex.Log(cp[5]  )  /*dummy*/;
                            cp[6] =  complex.Log(cp[6]  )  /*dummy*/;
                            cp[7] =  complex.Log(cp[7]  )  /*dummy*/;
                            cp[8] =  complex.Log(cp[8]  )  /*dummy*/;
                            cp[9] =  complex.Log(cp[9]  )  /*dummy*/;
                            cp[10] =  complex.Log(cp[10]  )  /*dummy*/;
                            cp[11] =  complex.Log(cp[11]  )  /*dummy*/;
                            cp[12] =  complex.Log(cp[12]  )  /*dummy*/;
                            cp[13] =  complex.Log(cp[13]  )  /*dummy*/;
                            cp[14] =  complex.Log(cp[14]  )  /*dummy*/;
                            cp[15] =  complex.Log(cp[15]  )  /*dummy*/;
                            cp[16] =  complex.Log(cp[16]  )  /*dummy*/;
                            cp[17] =  complex.Log(cp[17]  )  /*dummy*/;
                            cp[18] =  complex.Log(cp[18]  )  /*dummy*/;
                            cp[19] =  complex.Log(cp[19]  )  /*dummy*/;
                            cp[20] =  complex.Log(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.Log(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        complex* ap = ((complex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  complex.Log(ap[0]  )  /*dummy*/;
                            cp[1] =  complex.Log(ap[1]  )  /*dummy*/;
                            cp[2] =  complex.Log(ap[2]  )  /*dummy*/;
                            cp[3] =  complex.Log(ap[3]  )  /*dummy*/;
                            cp[4] =  complex.Log(ap[4]  )  /*dummy*/;
                            cp[5] =  complex.Log(ap[5]  )  /*dummy*/;
                            cp[6] =  complex.Log(ap[6]  )  /*dummy*/;
                            cp[7] =  complex.Log(ap[7]  )  /*dummy*/;
                            cp[8] =  complex.Log(ap[8]  )  /*dummy*/;
                            cp[9] =  complex.Log(ap[9]  )  /*dummy*/;
                            cp[10] =  complex.Log(ap[10]  )  /*dummy*/;
                            cp[11] =  complex.Log(ap[11]  )  /*dummy*/;
                            cp[12] =  complex.Log(ap[12]  )  /*dummy*/;
                            cp[13] =  complex.Log(ap[13]  )  /*dummy*/;
                            cp[14] =  complex.Log(ap[14]  )  /*dummy*/;
                            cp[15] =  complex.Log(ap[15]  )  /*dummy*/;
                            cp[16] =  complex.Log(ap[16]  )  /*dummy*/;
                            cp[17] =  complex.Log(ap[17]  )  /*dummy*/;
                            cp[18] =  complex.Log(ap[18]  )  /*dummy*/;
                            cp[19] =  complex.Log(ap[19]  )  /*dummy*/;
                            cp[20] =  complex.Log(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.Log(*ap  )  /*dummy*/;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( complex* arrAP = arrA)
                fixed ( complex* retArrP = retArr) {
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
                return new  ILRetArray<complex>(retStorage);
            }
        }
        /// <summary>Logarithm of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Complex logarithm of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<fcomplex>  log (ILInArray< fcomplex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<fcomplex>(A.Size); 
                ILSize inDim = A.Size;
                fcomplex[] arrA = A.GetArrayForRead(); 
                fcomplex [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true) {
                    retArr = ILMemoryPool.Pool.New<fcomplex>(outLen);
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
                ILDenseStorage<fcomplex> retStorage = new ILDenseStorage<fcomplex>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    fcomplex* cp = ((fcomplex*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  fcomplex.Log(cp[0]  )  /*dummy*/;
                            cp[1] =  fcomplex.Log(cp[1]  )  /*dummy*/;
                            cp[2] =  fcomplex.Log(cp[2]  )  /*dummy*/;
                            cp[3] =  fcomplex.Log(cp[3]  )  /*dummy*/;
                            cp[4] =  fcomplex.Log(cp[4]  )  /*dummy*/;
                            cp[5] =  fcomplex.Log(cp[5]  )  /*dummy*/;
                            cp[6] =  fcomplex.Log(cp[6]  )  /*dummy*/;
                            cp[7] =  fcomplex.Log(cp[7]  )  /*dummy*/;
                            cp[8] =  fcomplex.Log(cp[8]  )  /*dummy*/;
                            cp[9] =  fcomplex.Log(cp[9]  )  /*dummy*/;
                            cp[10] =  fcomplex.Log(cp[10]  )  /*dummy*/;
                            cp[11] =  fcomplex.Log(cp[11]  )  /*dummy*/;
                            cp[12] =  fcomplex.Log(cp[12]  )  /*dummy*/;
                            cp[13] =  fcomplex.Log(cp[13]  )  /*dummy*/;
                            cp[14] =  fcomplex.Log(cp[14]  )  /*dummy*/;
                            cp[15] =  fcomplex.Log(cp[15]  )  /*dummy*/;
                            cp[16] =  fcomplex.Log(cp[16]  )  /*dummy*/;
                            cp[17] =  fcomplex.Log(cp[17]  )  /*dummy*/;
                            cp[18] =  fcomplex.Log(cp[18]  )  /*dummy*/;
                            cp[19] =  fcomplex.Log(cp[19]  )  /*dummy*/;
                            cp[20] =  fcomplex.Log(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.Log(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  fcomplex.Log(ap[0]  )  /*dummy*/;
                            cp[1] =  fcomplex.Log(ap[1]  )  /*dummy*/;
                            cp[2] =  fcomplex.Log(ap[2]  )  /*dummy*/;
                            cp[3] =  fcomplex.Log(ap[3]  )  /*dummy*/;
                            cp[4] =  fcomplex.Log(ap[4]  )  /*dummy*/;
                            cp[5] =  fcomplex.Log(ap[5]  )  /*dummy*/;
                            cp[6] =  fcomplex.Log(ap[6]  )  /*dummy*/;
                            cp[7] =  fcomplex.Log(ap[7]  )  /*dummy*/;
                            cp[8] =  fcomplex.Log(ap[8]  )  /*dummy*/;
                            cp[9] =  fcomplex.Log(ap[9]  )  /*dummy*/;
                            cp[10] =  fcomplex.Log(ap[10]  )  /*dummy*/;
                            cp[11] =  fcomplex.Log(ap[11]  )  /*dummy*/;
                            cp[12] =  fcomplex.Log(ap[12]  )  /*dummy*/;
                            cp[13] =  fcomplex.Log(ap[13]  )  /*dummy*/;
                            cp[14] =  fcomplex.Log(ap[14]  )  /*dummy*/;
                            cp[15] =  fcomplex.Log(ap[15]  )  /*dummy*/;
                            cp[16] =  fcomplex.Log(ap[16]  )  /*dummy*/;
                            cp[17] =  fcomplex.Log(ap[17]  )  /*dummy*/;
                            cp[18] =  fcomplex.Log(ap[18]  )  /*dummy*/;
                            cp[19] =  fcomplex.Log(ap[19]  )  /*dummy*/;
                            cp[20] =  fcomplex.Log(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.Log(*ap  )  /*dummy*/;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( fcomplex* arrAP = arrA)
                fixed ( fcomplex* retArrP = retArr) {
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
                return new  ILRetArray<fcomplex>(retStorage);
            }
        }
        /// <summary>Logarithm of array elements - real output</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logarithm of array elements - real output</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<float>  log (ILInArray< float > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<float>(A.Size); 
                ILSize inDim = A.Size;
                float[] arrA = A.GetArrayForRead(); 
                float [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true) {
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
                            cp[0] =  (float)Math.Log(cp[0]  )  /*dummy*/;
                            cp[1] =  (float)Math.Log(cp[1]  )  /*dummy*/;
                            cp[2] =  (float)Math.Log(cp[2]  )  /*dummy*/;
                            cp[3] =  (float)Math.Log(cp[3]  )  /*dummy*/;
                            cp[4] =  (float)Math.Log(cp[4]  )  /*dummy*/;
                            cp[5] =  (float)Math.Log(cp[5]  )  /*dummy*/;
                            cp[6] =  (float)Math.Log(cp[6]  )  /*dummy*/;
                            cp[7] =  (float)Math.Log(cp[7]  )  /*dummy*/;
                            cp[8] =  (float)Math.Log(cp[8]  )  /*dummy*/;
                            cp[9] =  (float)Math.Log(cp[9]  )  /*dummy*/;
                            cp[10] =  (float)Math.Log(cp[10]  )  /*dummy*/;
                            cp[11] =  (float)Math.Log(cp[11]  )  /*dummy*/;
                            cp[12] =  (float)Math.Log(cp[12]  )  /*dummy*/;
                            cp[13] =  (float)Math.Log(cp[13]  )  /*dummy*/;
                            cp[14] =  (float)Math.Log(cp[14]  )  /*dummy*/;
                            cp[15] =  (float)Math.Log(cp[15]  )  /*dummy*/;
                            cp[16] =  (float)Math.Log(cp[16]  )  /*dummy*/;
                            cp[17] =  (float)Math.Log(cp[17]  )  /*dummy*/;
                            cp[18] =  (float)Math.Log(cp[18]  )  /*dummy*/;
                            cp[19] =  (float)Math.Log(cp[19]  )  /*dummy*/;
                            cp[20] =  (float)Math.Log(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (float)Math.Log(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        float* ap = ((float*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  (float)Math.Log(ap[0]  )  /*dummy*/;
                            cp[1] =  (float)Math.Log(ap[1]  )  /*dummy*/;
                            cp[2] =  (float)Math.Log(ap[2]  )  /*dummy*/;
                            cp[3] =  (float)Math.Log(ap[3]  )  /*dummy*/;
                            cp[4] =  (float)Math.Log(ap[4]  )  /*dummy*/;
                            cp[5] =  (float)Math.Log(ap[5]  )  /*dummy*/;
                            cp[6] =  (float)Math.Log(ap[6]  )  /*dummy*/;
                            cp[7] =  (float)Math.Log(ap[7]  )  /*dummy*/;
                            cp[8] =  (float)Math.Log(ap[8]  )  /*dummy*/;
                            cp[9] =  (float)Math.Log(ap[9]  )  /*dummy*/;
                            cp[10] =  (float)Math.Log(ap[10]  )  /*dummy*/;
                            cp[11] =  (float)Math.Log(ap[11]  )  /*dummy*/;
                            cp[12] =  (float)Math.Log(ap[12]  )  /*dummy*/;
                            cp[13] =  (float)Math.Log(ap[13]  )  /*dummy*/;
                            cp[14] =  (float)Math.Log(ap[14]  )  /*dummy*/;
                            cp[15] =  (float)Math.Log(ap[15]  )  /*dummy*/;
                            cp[16] =  (float)Math.Log(ap[16]  )  /*dummy*/;
                            cp[17] =  (float)Math.Log(ap[17]  )  /*dummy*/;
                            cp[18] =  (float)Math.Log(ap[18]  )  /*dummy*/;
                            cp[19] =  (float)Math.Log(ap[19]  )  /*dummy*/;
                            cp[20] =  (float)Math.Log(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (float)Math.Log(*ap  )  /*dummy*/;
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
        /// <summary>Logarithm of array elements - real output</summary>
        /// <param name="A">Input array</param>
        /// <returns>Logarithm of array elements - real output</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<double>  log (ILInArray< double > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<double>(A.Size); 
                ILSize inDim = A.Size;
                double[] arrA = A.GetArrayForRead(); 
                double [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (true) {
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
                            cp[0] =  Math.Log(cp[0]  )  /*dummy*/;
                            cp[1] =  Math.Log(cp[1]  )  /*dummy*/;
                            cp[2] =  Math.Log(cp[2]  )  /*dummy*/;
                            cp[3] =  Math.Log(cp[3]  )  /*dummy*/;
                            cp[4] =  Math.Log(cp[4]  )  /*dummy*/;
                            cp[5] =  Math.Log(cp[5]  )  /*dummy*/;
                            cp[6] =  Math.Log(cp[6]  )  /*dummy*/;
                            cp[7] =  Math.Log(cp[7]  )  /*dummy*/;
                            cp[8] =  Math.Log(cp[8]  )  /*dummy*/;
                            cp[9] =  Math.Log(cp[9]  )  /*dummy*/;
                            cp[10] =  Math.Log(cp[10]  )  /*dummy*/;
                            cp[11] =  Math.Log(cp[11]  )  /*dummy*/;
                            cp[12] =  Math.Log(cp[12]  )  /*dummy*/;
                            cp[13] =  Math.Log(cp[13]  )  /*dummy*/;
                            cp[14] =  Math.Log(cp[14]  )  /*dummy*/;
                            cp[15] =  Math.Log(cp[15]  )  /*dummy*/;
                            cp[16] =  Math.Log(cp[16]  )  /*dummy*/;
                            cp[17] =  Math.Log(cp[17]  )  /*dummy*/;
                            cp[18] =  Math.Log(cp[18]  )  /*dummy*/;
                            cp[19] =  Math.Log(cp[19]  )  /*dummy*/;
                            cp[20] =  Math.Log(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Math.Log(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        double* ap = ((double*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Math.Log(ap[0]  )  /*dummy*/;
                            cp[1] =  Math.Log(ap[1]  )  /*dummy*/;
                            cp[2] =  Math.Log(ap[2]  )  /*dummy*/;
                            cp[3] =  Math.Log(ap[3]  )  /*dummy*/;
                            cp[4] =  Math.Log(ap[4]  )  /*dummy*/;
                            cp[5] =  Math.Log(ap[5]  )  /*dummy*/;
                            cp[6] =  Math.Log(ap[6]  )  /*dummy*/;
                            cp[7] =  Math.Log(ap[7]  )  /*dummy*/;
                            cp[8] =  Math.Log(ap[8]  )  /*dummy*/;
                            cp[9] =  Math.Log(ap[9]  )  /*dummy*/;
                            cp[10] =  Math.Log(ap[10]  )  /*dummy*/;
                            cp[11] =  Math.Log(ap[11]  )  /*dummy*/;
                            cp[12] =  Math.Log(ap[12]  )  /*dummy*/;
                            cp[13] =  Math.Log(ap[13]  )  /*dummy*/;
                            cp[14] =  Math.Log(ap[14]  )  /*dummy*/;
                            cp[15] =  Math.Log(ap[15]  )  /*dummy*/;
                            cp[16] =  Math.Log(ap[16]  )  /*dummy*/;
                            cp[17] =  Math.Log(ap[17]  )  /*dummy*/;
                            cp[18] =  Math.Log(ap[18]  )  /*dummy*/;
                            cp[19] =  Math.Log(ap[19]  )  /*dummy*/;
                            cp[20] =  Math.Log(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Math.Log(*ap  )  /*dummy*/;
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

#endregion HYCALPER AUTO GENERATED CODE

    }
}
