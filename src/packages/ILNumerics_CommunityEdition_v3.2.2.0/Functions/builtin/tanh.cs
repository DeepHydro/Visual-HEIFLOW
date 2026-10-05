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

        /// <summary>Hyperbolic tangent of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Hyperbolic tangent of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<complex>  tanh (ILInArray< complex > A) {
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
                            cp[0] =  complex.Tanh(cp[0]  )  /*dummy*/;
                            cp[1] =  complex.Tanh(cp[1]  )  /*dummy*/;
                            cp[2] =  complex.Tanh(cp[2]  )  /*dummy*/;
                            cp[3] =  complex.Tanh(cp[3]  )  /*dummy*/;
                            cp[4] =  complex.Tanh(cp[4]  )  /*dummy*/;
                            cp[5] =  complex.Tanh(cp[5]  )  /*dummy*/;
                            cp[6] =  complex.Tanh(cp[6]  )  /*dummy*/;
                            cp[7] =  complex.Tanh(cp[7]  )  /*dummy*/;
                            cp[8] =  complex.Tanh(cp[8]  )  /*dummy*/;
                            cp[9] =  complex.Tanh(cp[9]  )  /*dummy*/;
                            cp[10] =  complex.Tanh(cp[10]  )  /*dummy*/;
                            cp[11] =  complex.Tanh(cp[11]  )  /*dummy*/;
                            cp[12] =  complex.Tanh(cp[12]  )  /*dummy*/;
                            cp[13] =  complex.Tanh(cp[13]  )  /*dummy*/;
                            cp[14] =  complex.Tanh(cp[14]  )  /*dummy*/;
                            cp[15] =  complex.Tanh(cp[15]  )  /*dummy*/;
                            cp[16] =  complex.Tanh(cp[16]  )  /*dummy*/;
                            cp[17] =  complex.Tanh(cp[17]  )  /*dummy*/;
                            cp[18] =  complex.Tanh(cp[18]  )  /*dummy*/;
                            cp[19] =  complex.Tanh(cp[19]  )  /*dummy*/;
                            cp[20] =  complex.Tanh(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.Tanh(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        complex* ap = ((complex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  complex.Tanh(ap[0]  )  /*dummy*/;
                            cp[1] =  complex.Tanh(ap[1]  )  /*dummy*/;
                            cp[2] =  complex.Tanh(ap[2]  )  /*dummy*/;
                            cp[3] =  complex.Tanh(ap[3]  )  /*dummy*/;
                            cp[4] =  complex.Tanh(ap[4]  )  /*dummy*/;
                            cp[5] =  complex.Tanh(ap[5]  )  /*dummy*/;
                            cp[6] =  complex.Tanh(ap[6]  )  /*dummy*/;
                            cp[7] =  complex.Tanh(ap[7]  )  /*dummy*/;
                            cp[8] =  complex.Tanh(ap[8]  )  /*dummy*/;
                            cp[9] =  complex.Tanh(ap[9]  )  /*dummy*/;
                            cp[10] =  complex.Tanh(ap[10]  )  /*dummy*/;
                            cp[11] =  complex.Tanh(ap[11]  )  /*dummy*/;
                            cp[12] =  complex.Tanh(ap[12]  )  /*dummy*/;
                            cp[13] =  complex.Tanh(ap[13]  )  /*dummy*/;
                            cp[14] =  complex.Tanh(ap[14]  )  /*dummy*/;
                            cp[15] =  complex.Tanh(ap[15]  )  /*dummy*/;
                            cp[16] =  complex.Tanh(ap[16]  )  /*dummy*/;
                            cp[17] =  complex.Tanh(ap[17]  )  /*dummy*/;
                            cp[18] =  complex.Tanh(ap[18]  )  /*dummy*/;
                            cp[19] =  complex.Tanh(ap[19]  )  /*dummy*/;
                            cp[20] =  complex.Tanh(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  complex.Tanh(*ap  )  /*dummy*/;
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
        /// <summary>Hyperbolic tangent of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Hyperbolic tangent of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<fcomplex>  tanh (ILInArray< fcomplex > A) {
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
                            cp[0] =  fcomplex.Tanh(cp[0]  )  /*dummy*/;
                            cp[1] =  fcomplex.Tanh(cp[1]  )  /*dummy*/;
                            cp[2] =  fcomplex.Tanh(cp[2]  )  /*dummy*/;
                            cp[3] =  fcomplex.Tanh(cp[3]  )  /*dummy*/;
                            cp[4] =  fcomplex.Tanh(cp[4]  )  /*dummy*/;
                            cp[5] =  fcomplex.Tanh(cp[5]  )  /*dummy*/;
                            cp[6] =  fcomplex.Tanh(cp[6]  )  /*dummy*/;
                            cp[7] =  fcomplex.Tanh(cp[7]  )  /*dummy*/;
                            cp[8] =  fcomplex.Tanh(cp[8]  )  /*dummy*/;
                            cp[9] =  fcomplex.Tanh(cp[9]  )  /*dummy*/;
                            cp[10] =  fcomplex.Tanh(cp[10]  )  /*dummy*/;
                            cp[11] =  fcomplex.Tanh(cp[11]  )  /*dummy*/;
                            cp[12] =  fcomplex.Tanh(cp[12]  )  /*dummy*/;
                            cp[13] =  fcomplex.Tanh(cp[13]  )  /*dummy*/;
                            cp[14] =  fcomplex.Tanh(cp[14]  )  /*dummy*/;
                            cp[15] =  fcomplex.Tanh(cp[15]  )  /*dummy*/;
                            cp[16] =  fcomplex.Tanh(cp[16]  )  /*dummy*/;
                            cp[17] =  fcomplex.Tanh(cp[17]  )  /*dummy*/;
                            cp[18] =  fcomplex.Tanh(cp[18]  )  /*dummy*/;
                            cp[19] =  fcomplex.Tanh(cp[19]  )  /*dummy*/;
                            cp[20] =  fcomplex.Tanh(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.Tanh(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  fcomplex.Tanh(ap[0]  )  /*dummy*/;
                            cp[1] =  fcomplex.Tanh(ap[1]  )  /*dummy*/;
                            cp[2] =  fcomplex.Tanh(ap[2]  )  /*dummy*/;
                            cp[3] =  fcomplex.Tanh(ap[3]  )  /*dummy*/;
                            cp[4] =  fcomplex.Tanh(ap[4]  )  /*dummy*/;
                            cp[5] =  fcomplex.Tanh(ap[5]  )  /*dummy*/;
                            cp[6] =  fcomplex.Tanh(ap[6]  )  /*dummy*/;
                            cp[7] =  fcomplex.Tanh(ap[7]  )  /*dummy*/;
                            cp[8] =  fcomplex.Tanh(ap[8]  )  /*dummy*/;
                            cp[9] =  fcomplex.Tanh(ap[9]  )  /*dummy*/;
                            cp[10] =  fcomplex.Tanh(ap[10]  )  /*dummy*/;
                            cp[11] =  fcomplex.Tanh(ap[11]  )  /*dummy*/;
                            cp[12] =  fcomplex.Tanh(ap[12]  )  /*dummy*/;
                            cp[13] =  fcomplex.Tanh(ap[13]  )  /*dummy*/;
                            cp[14] =  fcomplex.Tanh(ap[14]  )  /*dummy*/;
                            cp[15] =  fcomplex.Tanh(ap[15]  )  /*dummy*/;
                            cp[16] =  fcomplex.Tanh(ap[16]  )  /*dummy*/;
                            cp[17] =  fcomplex.Tanh(ap[17]  )  /*dummy*/;
                            cp[18] =  fcomplex.Tanh(ap[18]  )  /*dummy*/;
                            cp[19] =  fcomplex.Tanh(ap[19]  )  /*dummy*/;
                            cp[20] =  fcomplex.Tanh(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  fcomplex.Tanh(*ap  )  /*dummy*/;
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
        /// <summary>Hyperbolic tangent of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Hyperbolic tangent of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<float>  tanh (ILInArray< float > A) {
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
                            cp[0] =  (float)Math.Tanh(cp[0]  )  /*dummy*/;
                            cp[1] =  (float)Math.Tanh(cp[1]  )  /*dummy*/;
                            cp[2] =  (float)Math.Tanh(cp[2]  )  /*dummy*/;
                            cp[3] =  (float)Math.Tanh(cp[3]  )  /*dummy*/;
                            cp[4] =  (float)Math.Tanh(cp[4]  )  /*dummy*/;
                            cp[5] =  (float)Math.Tanh(cp[5]  )  /*dummy*/;
                            cp[6] =  (float)Math.Tanh(cp[6]  )  /*dummy*/;
                            cp[7] =  (float)Math.Tanh(cp[7]  )  /*dummy*/;
                            cp[8] =  (float)Math.Tanh(cp[8]  )  /*dummy*/;
                            cp[9] =  (float)Math.Tanh(cp[9]  )  /*dummy*/;
                            cp[10] =  (float)Math.Tanh(cp[10]  )  /*dummy*/;
                            cp[11] =  (float)Math.Tanh(cp[11]  )  /*dummy*/;
                            cp[12] =  (float)Math.Tanh(cp[12]  )  /*dummy*/;
                            cp[13] =  (float)Math.Tanh(cp[13]  )  /*dummy*/;
                            cp[14] =  (float)Math.Tanh(cp[14]  )  /*dummy*/;
                            cp[15] =  (float)Math.Tanh(cp[15]  )  /*dummy*/;
                            cp[16] =  (float)Math.Tanh(cp[16]  )  /*dummy*/;
                            cp[17] =  (float)Math.Tanh(cp[17]  )  /*dummy*/;
                            cp[18] =  (float)Math.Tanh(cp[18]  )  /*dummy*/;
                            cp[19] =  (float)Math.Tanh(cp[19]  )  /*dummy*/;
                            cp[20] =  (float)Math.Tanh(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (float)Math.Tanh(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        float* ap = ((float*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  (float)Math.Tanh(ap[0]  )  /*dummy*/;
                            cp[1] =  (float)Math.Tanh(ap[1]  )  /*dummy*/;
                            cp[2] =  (float)Math.Tanh(ap[2]  )  /*dummy*/;
                            cp[3] =  (float)Math.Tanh(ap[3]  )  /*dummy*/;
                            cp[4] =  (float)Math.Tanh(ap[4]  )  /*dummy*/;
                            cp[5] =  (float)Math.Tanh(ap[5]  )  /*dummy*/;
                            cp[6] =  (float)Math.Tanh(ap[6]  )  /*dummy*/;
                            cp[7] =  (float)Math.Tanh(ap[7]  )  /*dummy*/;
                            cp[8] =  (float)Math.Tanh(ap[8]  )  /*dummy*/;
                            cp[9] =  (float)Math.Tanh(ap[9]  )  /*dummy*/;
                            cp[10] =  (float)Math.Tanh(ap[10]  )  /*dummy*/;
                            cp[11] =  (float)Math.Tanh(ap[11]  )  /*dummy*/;
                            cp[12] =  (float)Math.Tanh(ap[12]  )  /*dummy*/;
                            cp[13] =  (float)Math.Tanh(ap[13]  )  /*dummy*/;
                            cp[14] =  (float)Math.Tanh(ap[14]  )  /*dummy*/;
                            cp[15] =  (float)Math.Tanh(ap[15]  )  /*dummy*/;
                            cp[16] =  (float)Math.Tanh(ap[16]  )  /*dummy*/;
                            cp[17] =  (float)Math.Tanh(ap[17]  )  /*dummy*/;
                            cp[18] =  (float)Math.Tanh(ap[18]  )  /*dummy*/;
                            cp[19] =  (float)Math.Tanh(ap[19]  )  /*dummy*/;
                            cp[20] =  (float)Math.Tanh(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  (float)Math.Tanh(*ap  )  /*dummy*/;
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
        /// <summary>Hyperbolic tangent of array elements</summary>
        /// <param name="A">Input array</param>
        /// <returns>Hyperbolic tangent of array elements</returns>
        /// <remarks><para>If the input array is empty, an empty array will be returned.</para>
        /// <para>The array returned will be a dense array.</para></remarks>
        public unsafe static  ILRetArray<double>  tanh (ILInArray< double > A) {
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
                            cp[0] =  Math.Tanh(cp[0]  )  /*dummy*/;
                            cp[1] =  Math.Tanh(cp[1]  )  /*dummy*/;
                            cp[2] =  Math.Tanh(cp[2]  )  /*dummy*/;
                            cp[3] =  Math.Tanh(cp[3]  )  /*dummy*/;
                            cp[4] =  Math.Tanh(cp[4]  )  /*dummy*/;
                            cp[5] =  Math.Tanh(cp[5]  )  /*dummy*/;
                            cp[6] =  Math.Tanh(cp[6]  )  /*dummy*/;
                            cp[7] =  Math.Tanh(cp[7]  )  /*dummy*/;
                            cp[8] =  Math.Tanh(cp[8]  )  /*dummy*/;
                            cp[9] =  Math.Tanh(cp[9]  )  /*dummy*/;
                            cp[10] =  Math.Tanh(cp[10]  )  /*dummy*/;
                            cp[11] =  Math.Tanh(cp[11]  )  /*dummy*/;
                            cp[12] =  Math.Tanh(cp[12]  )  /*dummy*/;
                            cp[13] =  Math.Tanh(cp[13]  )  /*dummy*/;
                            cp[14] =  Math.Tanh(cp[14]  )  /*dummy*/;
                            cp[15] =  Math.Tanh(cp[15]  )  /*dummy*/;
                            cp[16] =  Math.Tanh(cp[16]  )  /*dummy*/;
                            cp[17] =  Math.Tanh(cp[17]  )  /*dummy*/;
                            cp[18] =  Math.Tanh(cp[18]  )  /*dummy*/;
                            cp[19] =  Math.Tanh(cp[19]  )  /*dummy*/;
                            cp[20] =  Math.Tanh(cp[20]  )  /*dummy*/;
                            cp+=21; len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Math.Tanh(*cp  )  /*dummy*/;
                            cp++;
                        }
                    } else {
                        double* ap = ((double*)range.Item3 + range.Item1);
                        while (len > 20) {
                            cp[0] =  Math.Tanh(ap[0]  )  /*dummy*/;
                            cp[1] =  Math.Tanh(ap[1]  )  /*dummy*/;
                            cp[2] =  Math.Tanh(ap[2]  )  /*dummy*/;
                            cp[3] =  Math.Tanh(ap[3]  )  /*dummy*/;
                            cp[4] =  Math.Tanh(ap[4]  )  /*dummy*/;
                            cp[5] =  Math.Tanh(ap[5]  )  /*dummy*/;
                            cp[6] =  Math.Tanh(ap[6]  )  /*dummy*/;
                            cp[7] =  Math.Tanh(ap[7]  )  /*dummy*/;
                            cp[8] =  Math.Tanh(ap[8]  )  /*dummy*/;
                            cp[9] =  Math.Tanh(ap[9]  )  /*dummy*/;
                            cp[10] =  Math.Tanh(ap[10]  )  /*dummy*/;
                            cp[11] =  Math.Tanh(ap[11]  )  /*dummy*/;
                            cp[12] =  Math.Tanh(ap[12]  )  /*dummy*/;
                            cp[13] =  Math.Tanh(ap[13]  )  /*dummy*/;
                            cp[14] =  Math.Tanh(ap[14]  )  /*dummy*/;
                            cp[15] =  Math.Tanh(ap[15]  )  /*dummy*/;
                            cp[16] =  Math.Tanh(ap[16]  )  /*dummy*/;
                            cp[17] =  Math.Tanh(ap[17]  )  /*dummy*/;
                            cp[18] =  Math.Tanh(ap[18]  )  /*dummy*/;
                            cp[19] =  Math.Tanh(ap[19]  )  /*dummy*/;
                            cp[20] =  Math.Tanh(ap[20]  )  /*dummy*/;
                            ap += 21;
                            cp += 21;
                            len -= 21; 
                        }
                        while (len-- > 0) {
                            *cp =  Math.Tanh(*ap  )  /*dummy*/;
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
