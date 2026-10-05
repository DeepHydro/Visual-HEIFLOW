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

        /// <summary>Signum of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Array with elements signaling the sign of corresponding elements of A</returns>
        /// <remarks><para>The signum function produces following values: <list type="bullet"><item>-1 - for elements &lt; zero,</item><item> 0 - for elements equal zero,</item><item> 1 - for elements &gt; zero,</item></list></para><para>If the input array is empty, an empty array will be returned.</para>
        ///                         <para>The array returned will be a solid array.</para></remarks>
        public unsafe static  ILRetArray<double>  sign (ILInArray< double > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<double>(A.Size); 
                ILSize inDim = A.Size;
                double[] arrA = A.GetArrayForRead(); 
                double [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
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
                            cp[0] =  (double)Math.Sign(cp[0]  )  /*dummy*/;
                            cp[1] =  (double)Math.Sign(cp[1]  )  /*dummy*/;
                            cp[2] =  (double)Math.Sign(cp[2]  )  /*dummy*/;
                            cp[3] =  (double)Math.Sign(cp[3]  )  /*dummy*/;
                            cp[4] =  (double)Math.Sign(cp[4]  )  /*dummy*/;
                            cp[5] =  (double)Math.Sign(cp[5]  )  /*dummy*/;
                            cp[6] =  (double)Math.Sign(cp[6]  )  /*dummy*/;
                            cp[7] =  (double)Math.Sign(cp[7]  )  /*dummy*/;
                            cp[8] =  (double)Math.Sign(cp[8]  )  /*dummy*/;
                            cp[9] =  (double)Math.Sign(cp[9]  )  /*dummy*/;
                            cp[10] =  (double)Math.Sign(cp[10]  )  /*dummy*/;
                            cp[11] =  (double)Math.Sign(cp[11]  )  /*dummy*/;
                            cp[12] =  (double)Math.Sign(cp[12]  )  /*dummy*/;
                            cp[13] =  (double)Math.Sign(cp[13]  )  /*dummy*/;
                            cp[14] =  (double)Math.Sign(cp[14]  )  /*dummy*/;
                            cp[15] =  (double)Math.Sign(cp[15]  )  /*dummy*/;
                            cp[16] =  (double)Math.Sign(cp[16]  )  /*dummy*/;
                            cp[17] =  (double)Math.Sign(cp[17]  )  /*dummy*/;
                            cp[18] =  (double)Math.Sign(cp[18]  )  /*dummy*/;
                            cp[19] =  (double)Math.Sign(cp[19]  )  /*dummy*/;
                            cp[20] =  (double)Math.Sign(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (double)Math.Sign(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        double* ap = ((double*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  (double)Math.Sign(ap[0]  )  /*dummy*/;
                            cp[1] =  (double)Math.Sign(ap[1]  )  /*dummy*/;
                            cp[2] =  (double)Math.Sign(ap[2]  )  /*dummy*/;
                            cp[3] =  (double)Math.Sign(ap[3]  )  /*dummy*/;
                            cp[4] =  (double)Math.Sign(ap[4]  )  /*dummy*/;
                            cp[5] =  (double)Math.Sign(ap[5]  )  /*dummy*/;
                            cp[6] =  (double)Math.Sign(ap[6]  )  /*dummy*/;
                            cp[7] =  (double)Math.Sign(ap[7]  )  /*dummy*/;
                            cp[8] =  (double)Math.Sign(ap[8]  )  /*dummy*/;
                            cp[9] =  (double)Math.Sign(ap[9]  )  /*dummy*/;
                            cp[10] =  (double)Math.Sign(ap[10]  )  /*dummy*/;
                            cp[11] =  (double)Math.Sign(ap[11]  )  /*dummy*/;
                            cp[12] =  (double)Math.Sign(ap[12]  )  /*dummy*/;
                            cp[13] =  (double)Math.Sign(ap[13]  )  /*dummy*/;
                            cp[14] =  (double)Math.Sign(ap[14]  )  /*dummy*/;
                            cp[15] =  (double)Math.Sign(ap[15]  )  /*dummy*/;
                            cp[16] =  (double)Math.Sign(ap[16]  )  /*dummy*/;
                            cp[17] =  (double)Math.Sign(ap[17]  )  /*dummy*/;
                            cp[18] =  (double)Math.Sign(ap[18]  )  /*dummy*/;
                            cp[19] =  (double)Math.Sign(ap[19]  )  /*dummy*/;
                            cp[20] =  (double)Math.Sign(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (double)Math.Sign(*ap  )  /*dummy*/;
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
        /// <summary>Signum of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Array with elements signaling the sign of corresponding elements of A</returns>
        /// <remarks><para>The signum function produces following values: <list type="bullet"><item>-1 - for elements &lt; zero,</item><item> 0 - for elements equal zero,</item><item> 1 - for elements &gt; zero,</item></list></para><para>If the input array is empty, an empty array will be returned.</para>
        ///                         <para>The array returned will be a solid array.</para></remarks>
        public unsafe static  ILRetArray<float>  sign (ILInArray< float > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<float>(A.Size); 
                ILSize inDim = A.Size;
                float[] arrA = A.GetArrayForRead(); 
                float [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
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
                            cp[0] =  (float)Math.Sign(cp[0]  )  /*dummy*/;
                            cp[1] =  (float)Math.Sign(cp[1]  )  /*dummy*/;
                            cp[2] =  (float)Math.Sign(cp[2]  )  /*dummy*/;
                            cp[3] =  (float)Math.Sign(cp[3]  )  /*dummy*/;
                            cp[4] =  (float)Math.Sign(cp[4]  )  /*dummy*/;
                            cp[5] =  (float)Math.Sign(cp[5]  )  /*dummy*/;
                            cp[6] =  (float)Math.Sign(cp[6]  )  /*dummy*/;
                            cp[7] =  (float)Math.Sign(cp[7]  )  /*dummy*/;
                            cp[8] =  (float)Math.Sign(cp[8]  )  /*dummy*/;
                            cp[9] =  (float)Math.Sign(cp[9]  )  /*dummy*/;
                            cp[10] =  (float)Math.Sign(cp[10]  )  /*dummy*/;
                            cp[11] =  (float)Math.Sign(cp[11]  )  /*dummy*/;
                            cp[12] =  (float)Math.Sign(cp[12]  )  /*dummy*/;
                            cp[13] =  (float)Math.Sign(cp[13]  )  /*dummy*/;
                            cp[14] =  (float)Math.Sign(cp[14]  )  /*dummy*/;
                            cp[15] =  (float)Math.Sign(cp[15]  )  /*dummy*/;
                            cp[16] =  (float)Math.Sign(cp[16]  )  /*dummy*/;
                            cp[17] =  (float)Math.Sign(cp[17]  )  /*dummy*/;
                            cp[18] =  (float)Math.Sign(cp[18]  )  /*dummy*/;
                            cp[19] =  (float)Math.Sign(cp[19]  )  /*dummy*/;
                            cp[20] =  (float)Math.Sign(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (float)Math.Sign(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        float* ap = ((float*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  (float)Math.Sign(ap[0]  )  /*dummy*/;
                            cp[1] =  (float)Math.Sign(ap[1]  )  /*dummy*/;
                            cp[2] =  (float)Math.Sign(ap[2]  )  /*dummy*/;
                            cp[3] =  (float)Math.Sign(ap[3]  )  /*dummy*/;
                            cp[4] =  (float)Math.Sign(ap[4]  )  /*dummy*/;
                            cp[5] =  (float)Math.Sign(ap[5]  )  /*dummy*/;
                            cp[6] =  (float)Math.Sign(ap[6]  )  /*dummy*/;
                            cp[7] =  (float)Math.Sign(ap[7]  )  /*dummy*/;
                            cp[8] =  (float)Math.Sign(ap[8]  )  /*dummy*/;
                            cp[9] =  (float)Math.Sign(ap[9]  )  /*dummy*/;
                            cp[10] =  (float)Math.Sign(ap[10]  )  /*dummy*/;
                            cp[11] =  (float)Math.Sign(ap[11]  )  /*dummy*/;
                            cp[12] =  (float)Math.Sign(ap[12]  )  /*dummy*/;
                            cp[13] =  (float)Math.Sign(ap[13]  )  /*dummy*/;
                            cp[14] =  (float)Math.Sign(ap[14]  )  /*dummy*/;
                            cp[15] =  (float)Math.Sign(ap[15]  )  /*dummy*/;
                            cp[16] =  (float)Math.Sign(ap[16]  )  /*dummy*/;
                            cp[17] =  (float)Math.Sign(ap[17]  )  /*dummy*/;
                            cp[18] =  (float)Math.Sign(ap[18]  )  /*dummy*/;
                            cp[19] =  (float)Math.Sign(ap[19]  )  /*dummy*/;
                            cp[20] =  (float)Math.Sign(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (float)Math.Sign(*ap  )  /*dummy*/;
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
        /// <summary>Signum of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Array with elements signaling the sign of corresponding elements of A</returns>
        /// <remarks><para>The signum function for each element a is computed by: a / abs(a). Therefore, values returned will all lay on the unit circle.</para><para>If the input array is empty, an empty array will be returned.</para>
        ///                         <para>The array returned will be a solid array.</para></remarks>
        public unsafe static  ILRetArray<fcomplex>  sign (ILInArray< fcomplex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<fcomplex>(A.Size); 
                ILSize inDim = A.Size;
                fcomplex[] arrA = A.GetArrayForRead(); 
                fcomplex [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
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
                            cp[0] =  fcomplex.Sign(cp[0]  )  /*dummy*/;
                            cp[1] =  fcomplex.Sign(cp[1]  )  /*dummy*/;
                            cp[2] =  fcomplex.Sign(cp[2]  )  /*dummy*/;
                            cp[3] =  fcomplex.Sign(cp[3]  )  /*dummy*/;
                            cp[4] =  fcomplex.Sign(cp[4]  )  /*dummy*/;
                            cp[5] =  fcomplex.Sign(cp[5]  )  /*dummy*/;
                            cp[6] =  fcomplex.Sign(cp[6]  )  /*dummy*/;
                            cp[7] =  fcomplex.Sign(cp[7]  )  /*dummy*/;
                            cp[8] =  fcomplex.Sign(cp[8]  )  /*dummy*/;
                            cp[9] =  fcomplex.Sign(cp[9]  )  /*dummy*/;
                            cp[10] =  fcomplex.Sign(cp[10]  )  /*dummy*/;
                            cp[11] =  fcomplex.Sign(cp[11]  )  /*dummy*/;
                            cp[12] =  fcomplex.Sign(cp[12]  )  /*dummy*/;
                            cp[13] =  fcomplex.Sign(cp[13]  )  /*dummy*/;
                            cp[14] =  fcomplex.Sign(cp[14]  )  /*dummy*/;
                            cp[15] =  fcomplex.Sign(cp[15]  )  /*dummy*/;
                            cp[16] =  fcomplex.Sign(cp[16]  )  /*dummy*/;
                            cp[17] =  fcomplex.Sign(cp[17]  )  /*dummy*/;
                            cp[18] =  fcomplex.Sign(cp[18]  )  /*dummy*/;
                            cp[19] =  fcomplex.Sign(cp[19]  )  /*dummy*/;
                            cp[20] =  fcomplex.Sign(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.Sign(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  fcomplex.Sign(ap[0]  )  /*dummy*/;
                            cp[1] =  fcomplex.Sign(ap[1]  )  /*dummy*/;
                            cp[2] =  fcomplex.Sign(ap[2]  )  /*dummy*/;
                            cp[3] =  fcomplex.Sign(ap[3]  )  /*dummy*/;
                            cp[4] =  fcomplex.Sign(ap[4]  )  /*dummy*/;
                            cp[5] =  fcomplex.Sign(ap[5]  )  /*dummy*/;
                            cp[6] =  fcomplex.Sign(ap[6]  )  /*dummy*/;
                            cp[7] =  fcomplex.Sign(ap[7]  )  /*dummy*/;
                            cp[8] =  fcomplex.Sign(ap[8]  )  /*dummy*/;
                            cp[9] =  fcomplex.Sign(ap[9]  )  /*dummy*/;
                            cp[10] =  fcomplex.Sign(ap[10]  )  /*dummy*/;
                            cp[11] =  fcomplex.Sign(ap[11]  )  /*dummy*/;
                            cp[12] =  fcomplex.Sign(ap[12]  )  /*dummy*/;
                            cp[13] =  fcomplex.Sign(ap[13]  )  /*dummy*/;
                            cp[14] =  fcomplex.Sign(ap[14]  )  /*dummy*/;
                            cp[15] =  fcomplex.Sign(ap[15]  )  /*dummy*/;
                            cp[16] =  fcomplex.Sign(ap[16]  )  /*dummy*/;
                            cp[17] =  fcomplex.Sign(ap[17]  )  /*dummy*/;
                            cp[18] =  fcomplex.Sign(ap[18]  )  /*dummy*/;
                            cp[19] =  fcomplex.Sign(ap[19]  )  /*dummy*/;
                            cp[20] =  fcomplex.Sign(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.Sign(*ap  )  /*dummy*/;
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
        /// <summary>Signum of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Array with elements signaling the sign of corresponding elements of A</returns>
        /// <remarks><para>The signum function for each element a is computed by: a / abs(a). Therefore, values returned will all lay on the unit circle.</para><para>If the input array is empty, an empty array will be returned.</para>
        ///                         <para>The array returned will be a solid array.</para></remarks>
        public unsafe static  ILRetArray<complex>  sign (ILInArray< complex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<complex>(A.Size); 
                ILSize inDim = A.Size;
                complex[] arrA = A.GetArrayForRead(); 
                complex [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
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
                            cp[0] =  complex.Sign(cp[0]  )  /*dummy*/;
                            cp[1] =  complex.Sign(cp[1]  )  /*dummy*/;
                            cp[2] =  complex.Sign(cp[2]  )  /*dummy*/;
                            cp[3] =  complex.Sign(cp[3]  )  /*dummy*/;
                            cp[4] =  complex.Sign(cp[4]  )  /*dummy*/;
                            cp[5] =  complex.Sign(cp[5]  )  /*dummy*/;
                            cp[6] =  complex.Sign(cp[6]  )  /*dummy*/;
                            cp[7] =  complex.Sign(cp[7]  )  /*dummy*/;
                            cp[8] =  complex.Sign(cp[8]  )  /*dummy*/;
                            cp[9] =  complex.Sign(cp[9]  )  /*dummy*/;
                            cp[10] =  complex.Sign(cp[10]  )  /*dummy*/;
                            cp[11] =  complex.Sign(cp[11]  )  /*dummy*/;
                            cp[12] =  complex.Sign(cp[12]  )  /*dummy*/;
                            cp[13] =  complex.Sign(cp[13]  )  /*dummy*/;
                            cp[14] =  complex.Sign(cp[14]  )  /*dummy*/;
                            cp[15] =  complex.Sign(cp[15]  )  /*dummy*/;
                            cp[16] =  complex.Sign(cp[16]  )  /*dummy*/;
                            cp[17] =  complex.Sign(cp[17]  )  /*dummy*/;
                            cp[18] =  complex.Sign(cp[18]  )  /*dummy*/;
                            cp[19] =  complex.Sign(cp[19]  )  /*dummy*/;
                            cp[20] =  complex.Sign(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.Sign(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        complex* ap = ((complex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  complex.Sign(ap[0]  )  /*dummy*/;
                            cp[1] =  complex.Sign(ap[1]  )  /*dummy*/;
                            cp[2] =  complex.Sign(ap[2]  )  /*dummy*/;
                            cp[3] =  complex.Sign(ap[3]  )  /*dummy*/;
                            cp[4] =  complex.Sign(ap[4]  )  /*dummy*/;
                            cp[5] =  complex.Sign(ap[5]  )  /*dummy*/;
                            cp[6] =  complex.Sign(ap[6]  )  /*dummy*/;
                            cp[7] =  complex.Sign(ap[7]  )  /*dummy*/;
                            cp[8] =  complex.Sign(ap[8]  )  /*dummy*/;
                            cp[9] =  complex.Sign(ap[9]  )  /*dummy*/;
                            cp[10] =  complex.Sign(ap[10]  )  /*dummy*/;
                            cp[11] =  complex.Sign(ap[11]  )  /*dummy*/;
                            cp[12] =  complex.Sign(ap[12]  )  /*dummy*/;
                            cp[13] =  complex.Sign(ap[13]  )  /*dummy*/;
                            cp[14] =  complex.Sign(ap[14]  )  /*dummy*/;
                            cp[15] =  complex.Sign(ap[15]  )  /*dummy*/;
                            cp[16] =  complex.Sign(ap[16]  )  /*dummy*/;
                            cp[17] =  complex.Sign(ap[17]  )  /*dummy*/;
                            cp[18] =  complex.Sign(ap[18]  )  /*dummy*/;
                            cp[19] =  complex.Sign(ap[19]  )  /*dummy*/;
                            cp[20] =  complex.Sign(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.Sign(*ap  )  /*dummy*/;
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
        /// <summary>Signum of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Array with elements signaling the sign of corresponding elements of A</returns>
        /// <remarks><para>The signum function produces following values: <list type="bullet"><item>-1 - for elements &lt; zero,</item><item> 0 - for elements equal zero,</item><item> 1 - for elements &gt; zero,</item></list></para><para>If the input array is empty, an empty array will be returned.</para>
        ///                         <para>The array returned will be a solid array.</para></remarks>
        public unsafe static  ILRetArray<Int64>  sign (ILInArray< Int64 > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<Int64>(A.Size); 
                ILSize inDim = A.Size;
                Int64[] arrA = A.GetArrayForRead(); 
                Int64 [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
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
                            cp[0] =  (Int64)Math.Sign(cp[0]  )  /*dummy*/;
                            cp[1] =  (Int64)Math.Sign(cp[1]  )  /*dummy*/;
                            cp[2] =  (Int64)Math.Sign(cp[2]  )  /*dummy*/;
                            cp[3] =  (Int64)Math.Sign(cp[3]  )  /*dummy*/;
                            cp[4] =  (Int64)Math.Sign(cp[4]  )  /*dummy*/;
                            cp[5] =  (Int64)Math.Sign(cp[5]  )  /*dummy*/;
                            cp[6] =  (Int64)Math.Sign(cp[6]  )  /*dummy*/;
                            cp[7] =  (Int64)Math.Sign(cp[7]  )  /*dummy*/;
                            cp[8] =  (Int64)Math.Sign(cp[8]  )  /*dummy*/;
                            cp[9] =  (Int64)Math.Sign(cp[9]  )  /*dummy*/;
                            cp[10] =  (Int64)Math.Sign(cp[10]  )  /*dummy*/;
                            cp[11] =  (Int64)Math.Sign(cp[11]  )  /*dummy*/;
                            cp[12] =  (Int64)Math.Sign(cp[12]  )  /*dummy*/;
                            cp[13] =  (Int64)Math.Sign(cp[13]  )  /*dummy*/;
                            cp[14] =  (Int64)Math.Sign(cp[14]  )  /*dummy*/;
                            cp[15] =  (Int64)Math.Sign(cp[15]  )  /*dummy*/;
                            cp[16] =  (Int64)Math.Sign(cp[16]  )  /*dummy*/;
                            cp[17] =  (Int64)Math.Sign(cp[17]  )  /*dummy*/;
                            cp[18] =  (Int64)Math.Sign(cp[18]  )  /*dummy*/;
                            cp[19] =  (Int64)Math.Sign(cp[19]  )  /*dummy*/;
                            cp[20] =  (Int64)Math.Sign(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (Int64)Math.Sign(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        Int64* ap = ((Int64*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  (Int64)Math.Sign(ap[0]  )  /*dummy*/;
                            cp[1] =  (Int64)Math.Sign(ap[1]  )  /*dummy*/;
                            cp[2] =  (Int64)Math.Sign(ap[2]  )  /*dummy*/;
                            cp[3] =  (Int64)Math.Sign(ap[3]  )  /*dummy*/;
                            cp[4] =  (Int64)Math.Sign(ap[4]  )  /*dummy*/;
                            cp[5] =  (Int64)Math.Sign(ap[5]  )  /*dummy*/;
                            cp[6] =  (Int64)Math.Sign(ap[6]  )  /*dummy*/;
                            cp[7] =  (Int64)Math.Sign(ap[7]  )  /*dummy*/;
                            cp[8] =  (Int64)Math.Sign(ap[8]  )  /*dummy*/;
                            cp[9] =  (Int64)Math.Sign(ap[9]  )  /*dummy*/;
                            cp[10] =  (Int64)Math.Sign(ap[10]  )  /*dummy*/;
                            cp[11] =  (Int64)Math.Sign(ap[11]  )  /*dummy*/;
                            cp[12] =  (Int64)Math.Sign(ap[12]  )  /*dummy*/;
                            cp[13] =  (Int64)Math.Sign(ap[13]  )  /*dummy*/;
                            cp[14] =  (Int64)Math.Sign(ap[14]  )  /*dummy*/;
                            cp[15] =  (Int64)Math.Sign(ap[15]  )  /*dummy*/;
                            cp[16] =  (Int64)Math.Sign(ap[16]  )  /*dummy*/;
                            cp[17] =  (Int64)Math.Sign(ap[17]  )  /*dummy*/;
                            cp[18] =  (Int64)Math.Sign(ap[18]  )  /*dummy*/;
                            cp[19] =  (Int64)Math.Sign(ap[19]  )  /*dummy*/;
                            cp[20] =  (Int64)Math.Sign(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (Int64)Math.Sign(*ap  )  /*dummy*/;
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
        /// <summary>Signum of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Array with elements signaling the sign of corresponding elements of A</returns>
        /// <remarks><para>The signum function produces following values: <list type="bullet"><item>-1 - for elements &lt; zero,</item><item> 0 - for elements equal zero,</item><item> 1 - for elements &gt; zero,</item></list></para><para>If the input array is empty, an empty array will be returned.</para>
        ///                         <para>The array returned will be a solid array.</para></remarks>
        public unsafe static  ILRetArray<Int32>  sign (ILInArray< Int32 > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<Int32>(A.Size); 
                ILSize inDim = A.Size;
                Int32[] arrA = A.GetArrayForRead(); 
                Int32 [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
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
                            cp[0] =  (Int32)Math.Sign(cp[0]  )  /*dummy*/;
                            cp[1] =  (Int32)Math.Sign(cp[1]  )  /*dummy*/;
                            cp[2] =  (Int32)Math.Sign(cp[2]  )  /*dummy*/;
                            cp[3] =  (Int32)Math.Sign(cp[3]  )  /*dummy*/;
                            cp[4] =  (Int32)Math.Sign(cp[4]  )  /*dummy*/;
                            cp[5] =  (Int32)Math.Sign(cp[5]  )  /*dummy*/;
                            cp[6] =  (Int32)Math.Sign(cp[6]  )  /*dummy*/;
                            cp[7] =  (Int32)Math.Sign(cp[7]  )  /*dummy*/;
                            cp[8] =  (Int32)Math.Sign(cp[8]  )  /*dummy*/;
                            cp[9] =  (Int32)Math.Sign(cp[9]  )  /*dummy*/;
                            cp[10] =  (Int32)Math.Sign(cp[10]  )  /*dummy*/;
                            cp[11] =  (Int32)Math.Sign(cp[11]  )  /*dummy*/;
                            cp[12] =  (Int32)Math.Sign(cp[12]  )  /*dummy*/;
                            cp[13] =  (Int32)Math.Sign(cp[13]  )  /*dummy*/;
                            cp[14] =  (Int32)Math.Sign(cp[14]  )  /*dummy*/;
                            cp[15] =  (Int32)Math.Sign(cp[15]  )  /*dummy*/;
                            cp[16] =  (Int32)Math.Sign(cp[16]  )  /*dummy*/;
                            cp[17] =  (Int32)Math.Sign(cp[17]  )  /*dummy*/;
                            cp[18] =  (Int32)Math.Sign(cp[18]  )  /*dummy*/;
                            cp[19] =  (Int32)Math.Sign(cp[19]  )  /*dummy*/;
                            cp[20] =  (Int32)Math.Sign(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (Int32)Math.Sign(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        Int32* ap = ((Int32*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  (Int32)Math.Sign(ap[0]  )  /*dummy*/;
                            cp[1] =  (Int32)Math.Sign(ap[1]  )  /*dummy*/;
                            cp[2] =  (Int32)Math.Sign(ap[2]  )  /*dummy*/;
                            cp[3] =  (Int32)Math.Sign(ap[3]  )  /*dummy*/;
                            cp[4] =  (Int32)Math.Sign(ap[4]  )  /*dummy*/;
                            cp[5] =  (Int32)Math.Sign(ap[5]  )  /*dummy*/;
                            cp[6] =  (Int32)Math.Sign(ap[6]  )  /*dummy*/;
                            cp[7] =  (Int32)Math.Sign(ap[7]  )  /*dummy*/;
                            cp[8] =  (Int32)Math.Sign(ap[8]  )  /*dummy*/;
                            cp[9] =  (Int32)Math.Sign(ap[9]  )  /*dummy*/;
                            cp[10] =  (Int32)Math.Sign(ap[10]  )  /*dummy*/;
                            cp[11] =  (Int32)Math.Sign(ap[11]  )  /*dummy*/;
                            cp[12] =  (Int32)Math.Sign(ap[12]  )  /*dummy*/;
                            cp[13] =  (Int32)Math.Sign(ap[13]  )  /*dummy*/;
                            cp[14] =  (Int32)Math.Sign(ap[14]  )  /*dummy*/;
                            cp[15] =  (Int32)Math.Sign(ap[15]  )  /*dummy*/;
                            cp[16] =  (Int32)Math.Sign(ap[16]  )  /*dummy*/;
                            cp[17] =  (Int32)Math.Sign(ap[17]  )  /*dummy*/;
                            cp[18] =  (Int32)Math.Sign(ap[18]  )  /*dummy*/;
                            cp[19] =  (Int32)Math.Sign(ap[19]  )  /*dummy*/;
                            cp[20] =  (Int32)Math.Sign(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (Int32)Math.Sign(*ap  )  /*dummy*/;
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
        /// <summary>Signum of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Array with elements signaling the sign of corresponding elements of A</returns>
        /// <remarks><para>The signum function produces following values: <list type="bullet"><item>-1 - for elements &lt; zero,</item><item> 0 - for elements equal zero,</item><item> 1 - for elements &gt; zero,</item></list></para><para>If the input array is empty, an empty array will be returned.</para>
        ///                         <para>The array returned will be a solid array.</para></remarks>
        public unsafe static  ILRetArray<Int16>  sign (ILInArray< Int16 > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<Int16>(A.Size); 
                ILSize inDim = A.Size;
                Int16[] arrA = A.GetArrayForRead(); 
                Int16 [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<Int16>(outLen);
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
                ILDenseStorage<Int16> retStorage = new ILDenseStorage<Int16>(retArr, inDim);
                
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, bool> range = (Tuple<int, int, IntPtr, IntPtr, bool>)data;
                   
                    Int16* cp = ((Int16*)range.Item4 + range.Item1);
                    int len = range.Item2; 
                    if (range.Item5) { 
                        // inplace
                        while (len > 20) {
                            cp[0] =  (Int16)Math.Sign(cp[0]  )  /*dummy*/;
                            cp[1] =  (Int16)Math.Sign(cp[1]  )  /*dummy*/;
                            cp[2] =  (Int16)Math.Sign(cp[2]  )  /*dummy*/;
                            cp[3] =  (Int16)Math.Sign(cp[3]  )  /*dummy*/;
                            cp[4] =  (Int16)Math.Sign(cp[4]  )  /*dummy*/;
                            cp[5] =  (Int16)Math.Sign(cp[5]  )  /*dummy*/;
                            cp[6] =  (Int16)Math.Sign(cp[6]  )  /*dummy*/;
                            cp[7] =  (Int16)Math.Sign(cp[7]  )  /*dummy*/;
                            cp[8] =  (Int16)Math.Sign(cp[8]  )  /*dummy*/;
                            cp[9] =  (Int16)Math.Sign(cp[9]  )  /*dummy*/;
                            cp[10] =  (Int16)Math.Sign(cp[10]  )  /*dummy*/;
                            cp[11] =  (Int16)Math.Sign(cp[11]  )  /*dummy*/;
                            cp[12] =  (Int16)Math.Sign(cp[12]  )  /*dummy*/;
                            cp[13] =  (Int16)Math.Sign(cp[13]  )  /*dummy*/;
                            cp[14] =  (Int16)Math.Sign(cp[14]  )  /*dummy*/;
                            cp[15] =  (Int16)Math.Sign(cp[15]  )  /*dummy*/;
                            cp[16] =  (Int16)Math.Sign(cp[16]  )  /*dummy*/;
                            cp[17] =  (Int16)Math.Sign(cp[17]  )  /*dummy*/;
                            cp[18] =  (Int16)Math.Sign(cp[18]  )  /*dummy*/;
                            cp[19] =  (Int16)Math.Sign(cp[19]  )  /*dummy*/;
                            cp[20] =  (Int16)Math.Sign(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (Int16)Math.Sign(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        Int16* ap = ((Int16*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  (Int16)Math.Sign(ap[0]  )  /*dummy*/;
                            cp[1] =  (Int16)Math.Sign(ap[1]  )  /*dummy*/;
                            cp[2] =  (Int16)Math.Sign(ap[2]  )  /*dummy*/;
                            cp[3] =  (Int16)Math.Sign(ap[3]  )  /*dummy*/;
                            cp[4] =  (Int16)Math.Sign(ap[4]  )  /*dummy*/;
                            cp[5] =  (Int16)Math.Sign(ap[5]  )  /*dummy*/;
                            cp[6] =  (Int16)Math.Sign(ap[6]  )  /*dummy*/;
                            cp[7] =  (Int16)Math.Sign(ap[7]  )  /*dummy*/;
                            cp[8] =  (Int16)Math.Sign(ap[8]  )  /*dummy*/;
                            cp[9] =  (Int16)Math.Sign(ap[9]  )  /*dummy*/;
                            cp[10] =  (Int16)Math.Sign(ap[10]  )  /*dummy*/;
                            cp[11] =  (Int16)Math.Sign(ap[11]  )  /*dummy*/;
                            cp[12] =  (Int16)Math.Sign(ap[12]  )  /*dummy*/;
                            cp[13] =  (Int16)Math.Sign(ap[13]  )  /*dummy*/;
                            cp[14] =  (Int16)Math.Sign(ap[14]  )  /*dummy*/;
                            cp[15] =  (Int16)Math.Sign(ap[15]  )  /*dummy*/;
                            cp[16] =  (Int16)Math.Sign(ap[16]  )  /*dummy*/;
                            cp[17] =  (Int16)Math.Sign(ap[17]  )  /*dummy*/;
                            cp[18] =  (Int16)Math.Sign(ap[18]  )  /*dummy*/;
                            cp[19] =  (Int16)Math.Sign(ap[19]  )  /*dummy*/;
                            cp[20] =  (Int16)Math.Sign(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (Int16)Math.Sign(*ap  )  /*dummy*/;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( Int16* arrAP = arrA)
                fixed ( Int16* retArrP = retArr) {
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
                return new  ILRetArray<Int16>(retStorage);
            }
        }
        /// <summary>Signum of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Array with elements signaling the sign of corresponding elements of A</returns>
        /// <remarks><para>The signum function produces following values: <list type="bullet"><item>-1 - for elements &lt; zero,</item><item> 0 - for elements equal zero,</item><item> 1 - for elements &gt; zero,</item></list></para><para>If the input array is empty, an empty array will be returned.</para>
        ///                         <para>The array returned will be a solid array.</para></remarks>
        public unsafe static  ILRetArray<byte>  sign (ILInArray< byte > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty)
                    return new  ILRetArray<byte>(A.Size); 
                ILSize inDim = A.Size;
                byte[] arrA = A.GetArrayForRead(); 
                byte [] retArr;
                int outLen = inDim.NumberOfElements;
                bool inplace = true;
               
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
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
                            cp[0] =  (byte)Math.Sign(cp[0]  )  /*dummy*/;
                            cp[1] =  (byte)Math.Sign(cp[1]  )  /*dummy*/;
                            cp[2] =  (byte)Math.Sign(cp[2]  )  /*dummy*/;
                            cp[3] =  (byte)Math.Sign(cp[3]  )  /*dummy*/;
                            cp[4] =  (byte)Math.Sign(cp[4]  )  /*dummy*/;
                            cp[5] =  (byte)Math.Sign(cp[5]  )  /*dummy*/;
                            cp[6] =  (byte)Math.Sign(cp[6]  )  /*dummy*/;
                            cp[7] =  (byte)Math.Sign(cp[7]  )  /*dummy*/;
                            cp[8] =  (byte)Math.Sign(cp[8]  )  /*dummy*/;
                            cp[9] =  (byte)Math.Sign(cp[9]  )  /*dummy*/;
                            cp[10] =  (byte)Math.Sign(cp[10]  )  /*dummy*/;
                            cp[11] =  (byte)Math.Sign(cp[11]  )  /*dummy*/;
                            cp[12] =  (byte)Math.Sign(cp[12]  )  /*dummy*/;
                            cp[13] =  (byte)Math.Sign(cp[13]  )  /*dummy*/;
                            cp[14] =  (byte)Math.Sign(cp[14]  )  /*dummy*/;
                            cp[15] =  (byte)Math.Sign(cp[15]  )  /*dummy*/;
                            cp[16] =  (byte)Math.Sign(cp[16]  )  /*dummy*/;
                            cp[17] =  (byte)Math.Sign(cp[17]  )  /*dummy*/;
                            cp[18] =  (byte)Math.Sign(cp[18]  )  /*dummy*/;
                            cp[19] =  (byte)Math.Sign(cp[19]  )  /*dummy*/;
                            cp[20] =  (byte)Math.Sign(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (byte)Math.Sign(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        byte* ap = ((byte*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  (byte)Math.Sign(ap[0]  )  /*dummy*/;
                            cp[1] =  (byte)Math.Sign(ap[1]  )  /*dummy*/;
                            cp[2] =  (byte)Math.Sign(ap[2]  )  /*dummy*/;
                            cp[3] =  (byte)Math.Sign(ap[3]  )  /*dummy*/;
                            cp[4] =  (byte)Math.Sign(ap[4]  )  /*dummy*/;
                            cp[5] =  (byte)Math.Sign(ap[5]  )  /*dummy*/;
                            cp[6] =  (byte)Math.Sign(ap[6]  )  /*dummy*/;
                            cp[7] =  (byte)Math.Sign(ap[7]  )  /*dummy*/;
                            cp[8] =  (byte)Math.Sign(ap[8]  )  /*dummy*/;
                            cp[9] =  (byte)Math.Sign(ap[9]  )  /*dummy*/;
                            cp[10] =  (byte)Math.Sign(ap[10]  )  /*dummy*/;
                            cp[11] =  (byte)Math.Sign(ap[11]  )  /*dummy*/;
                            cp[12] =  (byte)Math.Sign(ap[12]  )  /*dummy*/;
                            cp[13] =  (byte)Math.Sign(ap[13]  )  /*dummy*/;
                            cp[14] =  (byte)Math.Sign(ap[14]  )  /*dummy*/;
                            cp[15] =  (byte)Math.Sign(ap[15]  )  /*dummy*/;
                            cp[16] =  (byte)Math.Sign(ap[16]  )  /*dummy*/;
                            cp[17] =  (byte)Math.Sign(ap[17]  )  /*dummy*/;
                            cp[18] =  (byte)Math.Sign(ap[18]  )  /*dummy*/;
                            cp[19] =  (byte)Math.Sign(ap[19]  )  /*dummy*/;
                            cp[20] =  (byte)Math.Sign(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (byte)Math.Sign(*ap  )  /*dummy*/;
                            ap++;
                            cp++;
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                fixed ( byte* arrAP = arrA)
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
                return new  ILRetArray<byte>(retStorage);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}
