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
using ILNumerics.Algorithms; 
using ILNumerics.Exceptions;
using ILNumerics;
using ILNumerics.Misc;
using System.Threading;



namespace ILNumerics.Misc {

    /// <summary>
    /// the class provides a number of one dimensional quicksort implementations for several datatypes/ properties
    /// </summary>
    public partial class ILQuickSort {

        private static int s_threadCount; 
        private struct ILQuickSortQueueItemData {
            public int lo;
            public int hi;
        }
        [ThreadStatic]
        private static int[] s_quickSortStackMT;
        [ThreadStatic]
        private volatile static int s_quickSortStackPos;



        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100. The stack array
        /// is cached internally.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscMT( double[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( double* vec = vecP) {
                    
                    if (checkNaNAsc(lo, ref hi, inc, vec)) return; 
                    s_threadCount = 1;
                    QuickSortAsc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>        
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescMT( double[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( double* vec = vecP) {
                    
                    if (checkNaNDesc(lo, ref hi, inc, vec)) return; 
                    s_threadCount = 1;
                    QuickSortDesc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }
        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscIDXMT( double[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( double* vec = vecP) {
                    
                    if (checkNaNIDXAsc(lo, ref hi, inc, vecIdx, vec)) return;
                    s_threadCount = 1;
                    QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescIDXMT( double[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
           
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( double* vec = vecP) {
                    
                    if (checkNaNIDXDesc(lo,ref hi, inc, vecIdx, vec)) return; 
                    Interlocked.Increment(ref s_threadCount);
                    QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);
                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        #region private functions
        unsafe private static void QuickSortAsc_ITMT_noNaN( double* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
            
            double a1, a2, a3;
            
            double temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                    
                    double* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI < pivotItem);
                        do vecJ -= inc; while (*vecJ > pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortDesc_ITMT_noNaN( double* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
            
            double a1, a2, a3;
            
            double temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                    
                    double* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI > pivotItem);
                        do vecJ -= inc; while (*vecJ < pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortAscIDX_ITMT_noNaN( double* vec, int* vecIdx, int lo, int hi, int inc) {
            
            double temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
            
            double a1, a2, a3;
            
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) < pivotItem);
                        do j -= inc; while (*(vec + j) > pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        unsafe private static void QuickSortDescIDX_ITMT_noNaN( double* vec, int* vecIdx, int lo, int hi, int inc) {
            
            double temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
            
            double a1, a2, a3;
            
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) > pivotItem);
                        do j -= inc; while (*(vec + j) < pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        #endregion 
        

#region HYCALPER AUTO GENERATED CODE


        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100. The stack array
        /// is cached internally.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscMT( UInt64[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( UInt64* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAsc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>        
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescMT( UInt64[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( UInt64* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortDesc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }
        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscIDXMT( UInt64[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( UInt64* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescIDXMT( UInt64[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
           
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( UInt64* vec = vecP) {
                    
                    Interlocked.Increment(ref s_threadCount);
                    QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);
                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        #region private functions
        unsafe private static void QuickSortAsc_ITMT_noNaN( UInt64* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            UInt64 a1, a2, a3;
           
            UInt64 temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    UInt64* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI < pivotItem);
                        do vecJ -= inc; while (*vecJ > pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortDesc_ITMT_noNaN( UInt64* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            UInt64 a1, a2, a3;
           
            UInt64 temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    UInt64* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI > pivotItem);
                        do vecJ -= inc; while (*vecJ < pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortAscIDX_ITMT_noNaN( UInt64* vec, int* vecIdx, int lo, int hi, int inc) {
           
            UInt64 temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            UInt64 a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) < pivotItem);
                        do j -= inc; while (*(vec + j) > pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        unsafe private static void QuickSortDescIDX_ITMT_noNaN( UInt64* vec, int* vecIdx, int lo, int hi, int inc) {
           
            UInt64 temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            UInt64 a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) > pivotItem);
                        do j -= inc; while (*(vec + j) < pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        #endregion 
        

        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100. The stack array
        /// is cached internally.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscMT( UInt32[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( UInt32* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAsc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>        
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescMT( UInt32[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( UInt32* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortDesc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }
        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscIDXMT( UInt32[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( UInt32* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescIDXMT( UInt32[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
           
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( UInt32* vec = vecP) {
                    
                    Interlocked.Increment(ref s_threadCount);
                    QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);
                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        #region private functions
        unsafe private static void QuickSortAsc_ITMT_noNaN( UInt32* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            UInt32 a1, a2, a3;
           
            UInt32 temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    UInt32* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI < pivotItem);
                        do vecJ -= inc; while (*vecJ > pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortDesc_ITMT_noNaN( UInt32* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            UInt32 a1, a2, a3;
           
            UInt32 temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    UInt32* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI > pivotItem);
                        do vecJ -= inc; while (*vecJ < pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortAscIDX_ITMT_noNaN( UInt32* vec, int* vecIdx, int lo, int hi, int inc) {
           
            UInt32 temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            UInt32 a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) < pivotItem);
                        do j -= inc; while (*(vec + j) > pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        unsafe private static void QuickSortDescIDX_ITMT_noNaN( UInt32* vec, int* vecIdx, int lo, int hi, int inc) {
           
            UInt32 temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            UInt32 a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) > pivotItem);
                        do j -= inc; while (*(vec + j) < pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        #endregion 
        

        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100. The stack array
        /// is cached internally.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscMT( UInt16[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( UInt16* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAsc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>        
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescMT( UInt16[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( UInt16* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortDesc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }
        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscIDXMT( UInt16[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( UInt16* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescIDXMT( UInt16[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
           
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( UInt16* vec = vecP) {
                    
                    Interlocked.Increment(ref s_threadCount);
                    QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);
                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        #region private functions
        unsafe private static void QuickSortAsc_ITMT_noNaN( UInt16* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            UInt16 a1, a2, a3;
           
            UInt16 temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    UInt16* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI < pivotItem);
                        do vecJ -= inc; while (*vecJ > pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortDesc_ITMT_noNaN( UInt16* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            UInt16 a1, a2, a3;
           
            UInt16 temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    UInt16* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI > pivotItem);
                        do vecJ -= inc; while (*vecJ < pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortAscIDX_ITMT_noNaN( UInt16* vec, int* vecIdx, int lo, int hi, int inc) {
           
            UInt16 temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            UInt16 a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) < pivotItem);
                        do j -= inc; while (*(vec + j) > pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        unsafe private static void QuickSortDescIDX_ITMT_noNaN( UInt16* vec, int* vecIdx, int lo, int hi, int inc) {
           
            UInt16 temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            UInt16 a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) > pivotItem);
                        do j -= inc; while (*(vec + j) < pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        #endregion 
        

        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100. The stack array
        /// is cached internally.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscMT( Int64[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( Int64* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAsc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>        
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescMT( Int64[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( Int64* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortDesc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }
        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscIDXMT( Int64[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( Int64* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescIDXMT( Int64[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
           
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( Int64* vec = vecP) {
                    
                    Interlocked.Increment(ref s_threadCount);
                    QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);
                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        #region private functions
        unsafe private static void QuickSortAsc_ITMT_noNaN( Int64* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            Int64 a1, a2, a3;
           
            Int64 temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    Int64* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI < pivotItem);
                        do vecJ -= inc; while (*vecJ > pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortDesc_ITMT_noNaN( Int64* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            Int64 a1, a2, a3;
           
            Int64 temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    Int64* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI > pivotItem);
                        do vecJ -= inc; while (*vecJ < pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortAscIDX_ITMT_noNaN( Int64* vec, int* vecIdx, int lo, int hi, int inc) {
           
            Int64 temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            Int64 a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) < pivotItem);
                        do j -= inc; while (*(vec + j) > pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        unsafe private static void QuickSortDescIDX_ITMT_noNaN( Int64* vec, int* vecIdx, int lo, int hi, int inc) {
           
            Int64 temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            Int64 a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) > pivotItem);
                        do j -= inc; while (*(vec + j) < pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        #endregion 
        

        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100. The stack array
        /// is cached internally.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscMT( Int32[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( Int32* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAsc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>        
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescMT( Int32[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( Int32* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortDesc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }
        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscIDXMT( Int32[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( Int32* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescIDXMT( Int32[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
           
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( Int32* vec = vecP) {
                    
                    Interlocked.Increment(ref s_threadCount);
                    QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);
                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        #region private functions
        unsafe private static void QuickSortAsc_ITMT_noNaN( Int32* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            Int32 a1, a2, a3;
           
            Int32 temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    Int32* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI < pivotItem);
                        do vecJ -= inc; while (*vecJ > pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortDesc_ITMT_noNaN( Int32* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            Int32 a1, a2, a3;
           
            Int32 temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    Int32* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI > pivotItem);
                        do vecJ -= inc; while (*vecJ < pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortAscIDX_ITMT_noNaN( Int32* vec, int* vecIdx, int lo, int hi, int inc) {
           
            Int32 temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            Int32 a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) < pivotItem);
                        do j -= inc; while (*(vec + j) > pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        unsafe private static void QuickSortDescIDX_ITMT_noNaN( Int32* vec, int* vecIdx, int lo, int hi, int inc) {
           
            Int32 temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            Int32 a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) > pivotItem);
                        do j -= inc; while (*(vec + j) < pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        #endregion 
        

        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100. The stack array
        /// is cached internally.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscMT( Int16[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( Int16* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAsc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>        
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescMT( Int16[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( Int16* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortDesc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }
        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscIDXMT( Int16[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( Int16* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescIDXMT( Int16[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
           
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( Int16* vec = vecP) {
                    
                    Interlocked.Increment(ref s_threadCount);
                    QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);
                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        #region private functions
        unsafe private static void QuickSortAsc_ITMT_noNaN( Int16* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            Int16 a1, a2, a3;
           
            Int16 temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    Int16* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI < pivotItem);
                        do vecJ -= inc; while (*vecJ > pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortDesc_ITMT_noNaN( Int16* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            Int16 a1, a2, a3;
           
            Int16 temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    Int16* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI > pivotItem);
                        do vecJ -= inc; while (*vecJ < pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortAscIDX_ITMT_noNaN( Int16* vec, int* vecIdx, int lo, int hi, int inc) {
           
            Int16 temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            Int16 a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) < pivotItem);
                        do j -= inc; while (*(vec + j) > pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        unsafe private static void QuickSortDescIDX_ITMT_noNaN( Int16* vec, int* vecIdx, int lo, int hi, int inc) {
           
            Int16 temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            Int16 a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) > pivotItem);
                        do j -= inc; while (*(vec + j) < pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        #endregion 
        

        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100. The stack array
        /// is cached internally.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscMT( float[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( float* vec = vecP) {
                    if (checkNaNAsc(lo,ref hi, inc, vec)) return;
                    s_threadCount = 1;
                    QuickSortAsc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>        
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescMT( float[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( float* vec = vecP) {
                    if (checkNaNDesc(lo,ref hi, inc, vec)) return;
                    s_threadCount = 1;
                    QuickSortDesc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }
        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscIDXMT( float[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( float* vec = vecP) {
                    if (checkNaNIDXAsc(lo,ref hi, inc, vecIdx, vec)) return;
                    s_threadCount = 1;
                    QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescIDXMT( float[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
           
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( float* vec = vecP) {
                    if (checkNaNIDXDesc(lo,ref hi, inc, vecIdx, vec)) return;
                    Interlocked.Increment(ref s_threadCount);
                    QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);
                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        #region private functions
        unsafe private static void QuickSortAsc_ITMT_noNaN( float* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            float a1, a2, a3;
           
            float temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    float* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI < pivotItem);
                        do vecJ -= inc; while (*vecJ > pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortDesc_ITMT_noNaN( float* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            float a1, a2, a3;
           
            float temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    float* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI > pivotItem);
                        do vecJ -= inc; while (*vecJ < pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortAscIDX_ITMT_noNaN( float* vec, int* vecIdx, int lo, int hi, int inc) {
           
            float temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            float a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) < pivotItem);
                        do j -= inc; while (*(vec + j) > pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        unsafe private static void QuickSortDescIDX_ITMT_noNaN( float* vec, int* vecIdx, int lo, int hi, int inc) {
           
            float temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            float a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) > pivotItem);
                        do j -= inc; while (*(vec + j) < pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        #endregion 
        

        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100. The stack array
        /// is cached internally.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscMT( fcomplex[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( fcomplex* vec = vecP) {
                    if (checkNaNAsc(lo,ref hi, inc, vec)) return;
                    s_threadCount = 1;
                    QuickSortAsc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>        
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescMT( fcomplex[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( fcomplex* vec = vecP) {
                    if (checkNaNDesc(lo,ref hi, inc, vec)) return;
                    s_threadCount = 1;
                    QuickSortDesc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }
        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscIDXMT( fcomplex[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( fcomplex* vec = vecP) {
                    if (checkNaNIDXAsc(lo,ref hi, inc, vecIdx, vec)) return;
                    s_threadCount = 1;
                    QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescIDXMT( fcomplex[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
           
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( fcomplex* vec = vecP) {
                    if (checkNaNIDXDesc(lo,ref hi, inc, vecIdx, vec)) return;
                    Interlocked.Increment(ref s_threadCount);
                    QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);
                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        #region private functions
        unsafe private static void QuickSortAsc_ITMT_noNaN( fcomplex* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            fcomplex a1, a2, a3;
           
            fcomplex temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    fcomplex* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI < pivotItem);
                        do vecJ -= inc; while (*vecJ > pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortDesc_ITMT_noNaN( fcomplex* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            fcomplex a1, a2, a3;
           
            fcomplex temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    fcomplex* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI > pivotItem);
                        do vecJ -= inc; while (*vecJ < pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortAscIDX_ITMT_noNaN( fcomplex* vec, int* vecIdx, int lo, int hi, int inc) {
           
            fcomplex temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            fcomplex a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) < pivotItem);
                        do j -= inc; while (*(vec + j) > pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        unsafe private static void QuickSortDescIDX_ITMT_noNaN( fcomplex* vec, int* vecIdx, int lo, int hi, int inc) {
           
            fcomplex temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            fcomplex a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) > pivotItem);
                        do j -= inc; while (*(vec + j) < pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        #endregion 
        

        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100. The stack array
        /// is cached internally.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscMT( complex[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( complex* vec = vecP) {
                    if (checkNaNAsc(lo,ref hi, inc, vec)) return;
                    s_threadCount = 1;
                    QuickSortAsc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>        
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescMT( complex[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( complex* vec = vecP) {
                    if (checkNaNDesc(lo,ref hi, inc, vec)) return;
                    s_threadCount = 1;
                    QuickSortDesc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }
        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscIDXMT( complex[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( complex* vec = vecP) {
                    if (checkNaNIDXAsc(lo,ref hi, inc, vecIdx, vec)) return;
                    s_threadCount = 1;
                    QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescIDXMT( complex[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
           
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( complex* vec = vecP) {
                    if (checkNaNIDXDesc(lo,ref hi, inc, vecIdx, vec)) return;
                    Interlocked.Increment(ref s_threadCount);
                    QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);
                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        #region private functions
        unsafe private static void QuickSortAsc_ITMT_noNaN( complex* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            complex a1, a2, a3;
           
            complex temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    complex* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI < pivotItem);
                        do vecJ -= inc; while (*vecJ > pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortDesc_ITMT_noNaN( complex* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            complex a1, a2, a3;
           
            complex temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    complex* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI > pivotItem);
                        do vecJ -= inc; while (*vecJ < pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortAscIDX_ITMT_noNaN( complex* vec, int* vecIdx, int lo, int hi, int inc) {
           
            complex temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            complex a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) < pivotItem);
                        do j -= inc; while (*(vec + j) > pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        unsafe private static void QuickSortDescIDX_ITMT_noNaN( complex* vec, int* vecIdx, int lo, int hi, int inc) {
           
            complex temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            complex a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) > pivotItem);
                        do j -= inc; while (*(vec + j) < pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        #endregion 
        

        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100. The stack array
        /// is cached internally.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscMT( char[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( char* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAsc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>        
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescMT( char[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( char* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortDesc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }
        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscIDXMT( char[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( char* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescIDXMT( char[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
           
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( char* vec = vecP) {
                    
                    Interlocked.Increment(ref s_threadCount);
                    QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);
                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        #region private functions
        unsafe private static void QuickSortAsc_ITMT_noNaN( char* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            char a1, a2, a3;
           
            char temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    char* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI < pivotItem);
                        do vecJ -= inc; while (*vecJ > pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortDesc_ITMT_noNaN( char* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            char a1, a2, a3;
           
            char temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    char* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI > pivotItem);
                        do vecJ -= inc; while (*vecJ < pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortAscIDX_ITMT_noNaN( char* vec, int* vecIdx, int lo, int hi, int inc) {
           
            char temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            char a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) < pivotItem);
                        do j -= inc; while (*(vec + j) > pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        unsafe private static void QuickSortDescIDX_ITMT_noNaN( char* vec, int* vecIdx, int lo, int hi, int inc) {
           
            char temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            char a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) > pivotItem);
                        do j -= inc; while (*(vec + j) < pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        #endregion 
        

        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100. The stack array
        /// is cached internally.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscMT( byte[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( byte* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAsc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, multithreaded
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>        
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory defaults to length 100.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescMT( byte[] vecP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            unsafe {
                fixed ( byte* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortDesc_ITMT_noNaN(vec, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }
        /// <summary>
        /// inline one dimensional quick sort, ascending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortAscIDXMT( byte[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( byte* vec = vecP) {
                    
                    s_threadCount = 1;
                    QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);

                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        /// <summary>
        /// inline one dimensional quick sort, descending, arbitrary element spacing, indices aware
        /// </summary>
        /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
        /// <param name="lo">lowest index of sorting range</param>
        /// <param name="hi">highest index of sorting range</param>
        /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
        /// <param name="inc">spacing between elements (dimension specifier)</param>
        /// <remarks>
        /// <para>This function provides a low level in-place implementation of the quicksort algorithm. It is used by the 
        /// higher level sorting API provided by <see cref="M:ILNumerics.ILMath.sort"/>. It is recommended to use the overloads 
        /// of <see cref="M:ILNumerics.ILMath.sort()"/> instead of this function. That way, the decision, which lower level function
        /// is to be called is made by the library automatically.</para>
        /// <para>A quicksort algorithm is used for ranges of minimum lengths specified by 
        /// <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/> and above. 
        /// Below that length, a simple insertion sort is used instead.</para>
        /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
        /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
        /// The size is determined by the value of 
        /// <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/>.</para>
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>This function internally parallelizes the sorting in a very efficient manner. It usually outperforms 
        /// non-parallelized versions of the quicksort algorithm for larger arrays. For smaller arrays, it may be more 
        /// efficient, to use the non parallelized version instead: <see cref="M:ILNumerics.ILMath.QuickSortAsc(double[], int, int, int)"/></para></remarks>
        public static void QuickSortDescIDXMT( byte[] vecP,  int[] idxP, int lo, int hi, int inc) {
            System.Diagnostics.Debug.Assert(vecP != null && vecP.Length > hi);
            System.Diagnostics.Debug.Assert(idxP != null && idxP.Length > hi);
            System.Diagnostics.Debug.Assert(lo <= hi);
            System.Diagnostics.Debug.Assert(lo >= 0);
           
            if (vecP.Length < 2) return;
            int[] stack = ILMemoryPool.Pool.New<int>(ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth * 2);
            unsafe {
                fixed ( int* vecIdx = idxP)
                fixed ( byte* vec = vecP) {
                    
                    Interlocked.Increment(ref s_threadCount);
                    QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, lo, hi, inc);
                } // fixed
            } // unsafe 
            Interlocked.Decrement(ref s_threadCount);
            SpinWait.SpinUntil(() => {
                return s_threadCount <= 0;
            });
        }

        #region private functions
        unsafe private static void QuickSortAsc_ITMT_noNaN( byte* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            byte a1, a2, a3;
           
            byte temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    byte* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI < pivotItem);
                        do vecJ -= inc; while (*vecJ > pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortAsc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortDesc_ITMT_noNaN( byte* vec, int lo, int hi, int inc) {
            int pivotIndex, i, j;
            s_quickSortStackPos = -1;
           
            byte a1, a2, a3;
           
            byte temp, pivotItem;
            for (; ; ) {
                #region innerloop
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                        }
                        *(vec + i + inc) = pivotItem;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    #region quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                        }
                    }
                    #endregion
                   
                    byte* vecI = vec + lo + inc, vecJ = vec + hi;
                    pivotItem = *vecI;
                    for (; ; ) {
                        do vecI += inc; while (*vecI > pivotItem);
                        do vecJ -= inc; while (*vecJ < pivotItem);
                        if (vecJ < vecI) break;
                        temp = *vecI;
                        *vecI = *vecJ;
                        *vecJ = temp;
                    }
                    *(vec + lo + inc) = *vecJ;
                    *vecJ = pivotItem;

#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    #endregion
                    i = (int)(vecI - vec);
                    j = (int)(vecJ - vec);
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
                                QuickSortDesc_ITMT_noNaN(vec, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion inner loop
            }
        }
        unsafe private static void QuickSortAscIDX_ITMT_noNaN( byte* vec, int* vecIdx, int lo, int hi, int inc) {
           
            byte temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            byte a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp <= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 > a2) {
                        if (a1 > a3) {
                            if (a3 > a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 > a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 > a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) < pivotItem);
                        do j -= inc; while (*(vec + j) > pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortAscIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        unsafe private static void QuickSortDescIDX_ITMT_noNaN( byte* vec, int* vecIdx, int lo, int hi, int inc) {
           
            byte temp, pivotItem;
            int pivotIndex, j, i;
            s_quickSortStackPos = -1;
           
            byte a1, a2, a3;
           
            int ai1, ai2, ai3;
            for (; ; ) {
                #region inner loop
                if (hi - lo < (ILNumerics.Settings.s_minimumQuicksortLength * inc)) {
                    #region small array: insertion sort
                    for (j = lo + inc; j <= hi; j += inc) {
                        pivotItem = (*(vec + j));
                        ai1 = *(vecIdx + j);
                        for (i = j - inc; i >= lo; i -= inc) {
                            temp = *(vec + i);
                            if (temp >= pivotItem)
                                break;
                            *(vec + i + inc) = temp;
                            *(vecIdx + i + inc) = *(vecIdx + i);
                        }
                        *(vec + i + inc) = pivotItem;
                        *(vecIdx + i + inc) = ai1;
                    }
                    #endregion
                    if (s_quickSortStackPos < 0) break;
                    lo = s_quickSortStackMT[s_quickSortStackPos--];
                    hi = s_quickSortStackMT[s_quickSortStackPos--];
                } else {
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo);
                    a2 = *(vec + pivotIndex);
                    a3 = *(vec + hi);
                    ai1 = *(vecIdx + lo);
                    ai2 = *(vecIdx + pivotIndex);
                    ai3 = *(vecIdx + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    *(vecIdx + pivotIndex) = *(vecIdx + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a1 < a3) {
                            if (a3 < a2) {
                                *(vec + hi) = a1;
                                *(vec + lo) = a2;
                                *(vec + lo + inc) = a3;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai2;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + hi) = a1;
                                *(vec + lo) = a3;
                                *(vec + lo + inc) = a2;
                                *(vecIdx + hi) = ai1;
                                *(vecIdx + lo) = ai3;
                                *(vecIdx + lo + inc) = ai2;
                            }
                        } else {
                            *(vec + lo) = a2;
                            *(vec + lo + inc) = a1;
                            *(vecIdx + lo) = ai2;
                            *(vecIdx + lo + inc) = ai1;
                        }
                    } else {
                        if (a2 < a3) {
                            *(vec + hi) = a2;
                            *(vecIdx + hi) = ai2;
                            if (a3 < a1) {
                                *(vec + lo + inc) = a3;
                                *(vecIdx + lo + inc) = ai3;
                            } else {
                                *(vec + lo + inc) = a1;
                                *(vec + lo) = a3;
                                *(vecIdx + lo + inc) = ai1;
                                *(vecIdx + lo) = ai3;
                            }
                        } else {
                            *(vec + lo + inc) = a2;
                            *(vecIdx + lo + inc) = ai2;
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi;
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (; ; ) {
                        do i += inc; while (*(vec + i) > pivotItem);
                        do j -= inc; while (*(vec + j) < pivotItem);
                        if (j < i) break;
                        temp = *(vec + i);
                        ai2 = *(vecIdx + i);
                        *(vec + i) = *(vec + j);
                        *(vec + j) = temp;
                        *(vecIdx + i) = *(vecIdx + j);
                        *(vecIdx + j) = ai2;
                    }
                    *(vec + lo + inc) = *(vec + j);
                    *(vec + j) = pivotItem;
                    *(vecIdx + lo + inc) = *(vecIdx + j);
                    *(vecIdx + j) = ai1;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif
                    if (hi - i + inc < j - lo) {
                        if (s_threadCount < 100
                                && (j - lo) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = i;
                            data.hi = hi;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = hi;
                            s_quickSortStackMT[++s_quickSortStackPos] = i;
                        }
                        hi = j - inc;
                    } else {
                        if (s_threadCount < 100
                            && (hi - i + inc) / inc > Settings.s_minParallelElement2Count) {
                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
                            Interlocked.Increment(ref s_threadCount);

                            ILQuickSortQueueItemData data;
                            data.lo = lo;
                            data.hi = j - inc;
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
                                ILQuickSortQueueItemData d = (ILQuickSortQueueItemData)o;
                                QuickSortDescIDX_ITMT_noNaN(vec, vecIdx, d.lo, d.hi, inc);
                                Interlocked.Decrement(ref s_threadCount);
                            }, data);
                        } else {
                            // TODO: in order to minimize the stack size, do smaller chunk now and put larger chunk into stack! 
                            if (s_quickSortStackMT == null)
                                s_quickSortStackMT = new int[100];
                            s_quickSortStackMT[++s_quickSortStackPos] = j - inc;
                            s_quickSortStackMT[++s_quickSortStackPos] = lo;
                        }
                        lo = i;
                    }
                }
                #endregion
            }
        }
        #endregion 
        

#endregion HYCALPER AUTO GENERATED CODE



        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNIDXAsc(int lo, ref int hi, int inc, int* vecIdx, double* vec) {
            int tempIdx;
            bool isSorted = true;
            for (; hi >= lo && double.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
                
                double lastVal = *(vec + hi), curVal;
                for (int i = hi - inc; i >= lo; i -= inc) {
                    curVal = *(vec + i);
                    if (double.IsNaN(curVal)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = double.NaN;
                        tempIdx = *(vecIdx + hi);
                        *(vecIdx + hi) = *(vecIdx + i);
                        *(vecIdx + i) = tempIdx;
                        hi -= inc;
                    } else if (isSorted) {
                        if (curVal > lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = curVal;
                        }
                    }
                }
            }
            return isSorted;
        }

        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNIDXDesc(int lo, ref int hi, int inc, int* vecIdx, double* vec) {
            bool isSorted = true;
            int tempIDX;
            for (; hi >= lo && double.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
                
                double lastVal = *(vec + hi), curVal;
                for (int i = hi - inc; i >= lo; i -= inc) {
                    curVal = *(vec + i); 
                    if (double.IsNaN(curVal)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = double.NaN;
                        tempIDX = *(vecIdx + hi);
                        *(vecIdx + hi) = *(vecIdx + i);
                        *(vecIdx + i) = tempIDX;
                        hi -= inc;
                    } else if (isSorted) {
                        if (curVal < lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = curVal;
                        }
                    }
                }
            }
            return isSorted;
        }

        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNAsc(int lo, ref int hi, int inc, double* vec) {
            bool isSorted = true; 
            for (; hi >= lo && double.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
                
                double temp, lastVal = *(vec + hi);
                for (int i = hi; i >= lo; i -= inc) { // SMA: Started at hi-inc, which would yield isSorted = true when all elements are in order but the last!
                    temp = *(vec + i);
                    if (double.IsNaN(temp)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = double.NaN;
                        hi -= inc;
                    } else if (isSorted) {
                        if (temp > lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = temp;
                        }
                    }
                }
            }
            return isSorted; 
        }
        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNDesc(int lo, ref int hi, int inc, double* vec) {
            bool isSorted = true; 
            for (; hi >= lo && double.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
                
                double temp, lastVal = *(vec + hi);
                for (int i = hi; i >= lo; i -= inc) { // SMA: Started at hi-inc, which would yield isSorted = true when all elements are in order but the last!
                    temp = *(vec + i);
                    if (double.IsNaN(temp)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = double.NaN;
                        hi -= inc;
                    } else if (isSorted) {
                        if (temp < lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = temp;
                        }
                    }
                }
            }
            return isSorted; 
        }


#region HYCALPER AUTO GENERATED CODE


        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNIDXAsc(int lo, ref int hi, int inc, int* vecIdx, float* vec) {
            int tempIdx;
            bool isSorted = true;
            for (; hi >= lo && float.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
               
                float lastVal = *(vec + hi), curVal;
                for (int i = hi - inc; i >= lo; i -= inc) {
                    curVal = *(vec + i);
                    if (float.IsNaN(curVal)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = float.NaN;
                        tempIdx = *(vecIdx + hi);
                        *(vecIdx + hi) = *(vecIdx + i);
                        *(vecIdx + i) = tempIdx;
                        hi -= inc;
                    } else if (isSorted) {
                        if (curVal > lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = curVal;
                        }
                    }
                }
            }
            return isSorted;
        }

        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNIDXDesc(int lo, ref int hi, int inc, int* vecIdx, float* vec) {
            bool isSorted = true;
            int tempIDX;
            for (; hi >= lo && float.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
               
                float lastVal = *(vec + hi), curVal;
                for (int i = hi - inc; i >= lo; i -= inc) {
                    curVal = *(vec + i); 
                    if (float.IsNaN(curVal)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = float.NaN;
                        tempIDX = *(vecIdx + hi);
                        *(vecIdx + hi) = *(vecIdx + i);
                        *(vecIdx + i) = tempIDX;
                        hi -= inc;
                    } else if (isSorted) {
                        if (curVal < lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = curVal;
                        }
                    }
                }
            }
            return isSorted;
        }

        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNAsc(int lo, ref int hi, int inc, float* vec) {
            bool isSorted = true; 
            for (; hi >= lo && float.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
               
                float temp, lastVal = *(vec + hi);
                for (int i = hi; i >= lo; i -= inc) { // SMA: Started at hi-inc, which would yield isSorted = true when all elements are in order but the last!
                    temp = *(vec + i);
                    if (float.IsNaN(temp)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = float.NaN;
                        hi -= inc;
                    } else if (isSorted) {
                        if (temp > lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = temp;
                        }
                    }
                }
            }
            return isSorted; 
        }
        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNDesc(int lo, ref int hi, int inc, float* vec) {
            bool isSorted = true; 
            for (; hi >= lo && float.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
               
                float temp, lastVal = *(vec + hi);
                for (int i = hi; i >= lo; i -= inc) { // SMA: Started at hi-inc, which would yield isSorted = true when all elements are in order but the last!
                    temp = *(vec + i);
                    if (float.IsNaN(temp)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = float.NaN;
                        hi -= inc;
                    } else if (isSorted) {
                        if (temp < lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = temp;
                        }
                    }
                }
            }
            return isSorted; 
        }


        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNIDXAsc(int lo, ref int hi, int inc, int* vecIdx, fcomplex* vec) {
            int tempIdx;
            bool isSorted = true;
            for (; hi >= lo && fcomplex.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
               
                fcomplex lastVal = *(vec + hi), curVal;
                for (int i = hi - inc; i >= lo; i -= inc) {
                    curVal = *(vec + i);
                    if (fcomplex.IsNaN(curVal)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = fcomplex.NaN;
                        tempIdx = *(vecIdx + hi);
                        *(vecIdx + hi) = *(vecIdx + i);
                        *(vecIdx + i) = tempIdx;
                        hi -= inc;
                    } else if (isSorted) {
                        if (curVal > lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = curVal;
                        }
                    }
                }
            }
            return isSorted;
        }

        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNIDXDesc(int lo, ref int hi, int inc, int* vecIdx, fcomplex* vec) {
            bool isSorted = true;
            int tempIDX;
            for (; hi >= lo && fcomplex.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
               
                fcomplex lastVal = *(vec + hi), curVal;
                for (int i = hi - inc; i >= lo; i -= inc) {
                    curVal = *(vec + i); 
                    if (fcomplex.IsNaN(curVal)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = fcomplex.NaN;
                        tempIDX = *(vecIdx + hi);
                        *(vecIdx + hi) = *(vecIdx + i);
                        *(vecIdx + i) = tempIDX;
                        hi -= inc;
                    } else if (isSorted) {
                        if (curVal < lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = curVal;
                        }
                    }
                }
            }
            return isSorted;
        }

        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNAsc(int lo, ref int hi, int inc, fcomplex* vec) {
            bool isSorted = true; 
            for (; hi >= lo && fcomplex.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
               
                fcomplex temp, lastVal = *(vec + hi);
                for (int i = hi; i >= lo; i -= inc) { // SMA: Started at hi-inc, which would yield isSorted = true when all elements are in order but the last!
                    temp = *(vec + i);
                    if (fcomplex.IsNaN(temp)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = fcomplex.NaN;
                        hi -= inc;
                    } else if (isSorted) {
                        if (temp > lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = temp;
                        }
                    }
                }
            }
            return isSorted; 
        }
        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNDesc(int lo, ref int hi, int inc, fcomplex* vec) {
            bool isSorted = true; 
            for (; hi >= lo && fcomplex.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
               
                fcomplex temp, lastVal = *(vec + hi);
                for (int i = hi; i >= lo; i -= inc) { // SMA: Started at hi-inc, which would yield isSorted = true when all elements are in order but the last!
                    temp = *(vec + i);
                    if (fcomplex.IsNaN(temp)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = fcomplex.NaN;
                        hi -= inc;
                    } else if (isSorted) {
                        if (temp < lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = temp;
                        }
                    }
                }
            }
            return isSorted; 
        }


        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNIDXAsc(int lo, ref int hi, int inc, int* vecIdx, complex* vec) {
            int tempIdx;
            bool isSorted = true;
            for (; hi >= lo && complex.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
               
                complex lastVal = *(vec + hi), curVal;
                for (int i = hi - inc; i >= lo; i -= inc) {
                    curVal = *(vec + i);
                    if (complex.IsNaN(curVal)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = complex.NaN;
                        tempIdx = *(vecIdx + hi);
                        *(vecIdx + hi) = *(vecIdx + i);
                        *(vecIdx + i) = tempIdx;
                        hi -= inc;
                    } else if (isSorted) {
                        if (curVal > lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = curVal;
                        }
                    }
                }
            }
            return isSorted;
        }

        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNIDXDesc(int lo, ref int hi, int inc, int* vecIdx, complex* vec) {
            bool isSorted = true;
            int tempIDX;
            for (; hi >= lo && complex.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
               
                complex lastVal = *(vec + hi), curVal;
                for (int i = hi - inc; i >= lo; i -= inc) {
                    curVal = *(vec + i); 
                    if (complex.IsNaN(curVal)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = complex.NaN;
                        tempIDX = *(vecIdx + hi);
                        *(vecIdx + hi) = *(vecIdx + i);
                        *(vecIdx + i) = tempIDX;
                        hi -= inc;
                    } else if (isSorted) {
                        if (curVal < lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = curVal;
                        }
                    }
                }
            }
            return isSorted;
        }

        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNAsc(int lo, ref int hi, int inc, complex* vec) {
            bool isSorted = true; 
            for (; hi >= lo && complex.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
               
                complex temp, lastVal = *(vec + hi);
                for (int i = hi; i >= lo; i -= inc) { // SMA: Started at hi-inc, which would yield isSorted = true when all elements are in order but the last!
                    temp = *(vec + i);
                    if (complex.IsNaN(temp)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = complex.NaN;
                        hi -= inc;
                    } else if (isSorted) {
                        if (temp > lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = temp;
                        }
                    }
                }
            }
            return isSorted; 
        }
        /// <summary>
        /// sort nan elements to the end, test if the array is already sorted
        /// </summary>
        /// <returns>true, if already sorted</returns>
        unsafe private static bool checkNaNDesc(int lo, ref int hi, int inc, complex* vec) {
            bool isSorted = true; 
            for (; hi >= lo && complex.IsNaN(*(vec + hi)); hi -= inc) ;
            if (hi >= lo) {
               
                complex temp, lastVal = *(vec + hi);
                for (int i = hi; i >= lo; i -= inc) { // SMA: Started at hi-inc, which would yield isSorted = true when all elements are in order but the last!
                    temp = *(vec + i);
                    if (complex.IsNaN(temp)) {
                        *(vec + i) = *(vec + hi);
                        *(vec + hi) = complex.NaN;
                        hi -= inc;
                    } else if (isSorted) {
                        if (temp < lastVal) {
                            isSorted = false;
                        } else {
                            lastVal = temp;
                        }
                    }
                }
            }
            return isSorted; 
        }


#endregion HYCALPER AUTO GENERATED CODE

        // FOLLOWING ATTEMPT TO REPLACE INT POINTER INCREMENTS WITH RUNNING POINTERs WAS YET BUGGY, BUT MAY BRING FURTHER INCREASE OF SPEED
        //        unsafe private static void QuickSortAsc_ITMT_noNaN( double* vecLo,  double* vecHi, int inc) {
        //            
        //            double* veci, vecj, pivotIndex;
        //            s_quickSortStackPos = -1;
        //            
        //            double a1, a2, a3;
        //            
        //            double temp, pivotItem;
        //            //System.Diagnostics.Debug.WriteLine(@"Thread:{0} len:{1} lo:{2} hi:{3}", System.Threading.Thread.CurrentThread.ManagedThreadId, vecP.Length, lo, hi);

        //            for (; ; ) {
        //                #region innerloop
        //                if (vecHi - vecLo < (Settings.s_minimumQuicksortLength * inc)) {
        //                    #region small array: insertion sort
        //                    for (vecj = vecLo + inc; vecj <= vecHi; vecj += inc) {
        //                        pivotItem = *vecj;
        //                        for (veci = vecj - inc; veci >= vecLo; veci -= inc) {
        //                            temp = *veci;
        //                            if (temp <= pivotItem)
        //                                break;
        //                            *(veci + inc) = temp;
        //                        }
        //                        *(veci + inc) = pivotItem;
        //                    }
        //                    #endregion
        //                    if (s_quickSortStackPos < 0) break;
        //                    vecLo = ( double*)QuickSortStack[s_quickSortStackPos--];
        //                    vecHi = ( double*)QuickSortStack[s_quickSortStackPos--];
        //                } else {
        //                    #region quick sort
        //                    pivotIndex = vecLo + (int)((vecHi - vecLo) / inc / 2) * inc;
        //                    a1 = *vecLo;
        //                    a2 = *pivotIndex;
        //                    a3 = *vecHi;
        //                    *pivotIndex = *(vecLo + inc);
        //                    #region pivoting bring lo, lo+inc, hi in order
        //                    // by explicitely stepping through all possible paths,
        //                    // we save some assignments and comparisons
        //                    if (a1 > a2) {
        //                        if (a1 > a3) {
        //                            if (a3 > a2) {
        //                                *vecHi = a1;
        //                                *vecLo = a2;
        //                                *(vecLo + inc) = a3;
        //                            } else {
        //                                *vecHi = a1;
        //                                *vecLo = a3;
        //                                *(vecLo + inc) = a2;
        //                            }
        //                        } else {
        //                            *vecLo = a2;
        //                            *(vecLo + inc) = a1;
        //                        }
        //                    } else {
        //                        if (a2 > a3) {
        //                            *vecHi = a2;
        //                            if (a3 > a1) {
        //                                *(vecLo + inc) = a3;
        //                            } else {
        //                                *(vecLo + inc) = a1;
        //                                *vecLo = a3;
        //                            }
        //                        } else {
        //                            *(vecLo + inc) = a2;
        //                        }
        //                    }
        //                    #endregion
        //                    veci = vecLo + inc; vecj = vecHi;
        //                    pivotItem = *veci;
        //                    for (; ; ) {
        //                        do veci += inc; while (*veci < pivotItem);
        //                        do vecj -= inc; while (*vecj > pivotItem);
        //                        if (vecj < veci) break;
        //                        temp = *veci;
        //                        *veci = *vecj;
        //                        *vecj = temp;
        //                    }
        //                    *(vecLo + inc) = *vecj;
        //                    *vecj = pivotItem;

        //#if DEBUG
        //                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
        //#endif
        //                    #endregion
        //                    if (vecHi - veci + inc < vecj - vecLo) {
        //                        if (s_threadCount < int.MaxValue //ILSettings.s_maxNumberThreads
        //                                && (vecj - vecLo) / inc > ILSettings.s_minParallelElement2Count) {
        //                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
        //                            Interlocked.Increment(ref s_threadCount);
        //                            //s_threadCount++;
        //                            ILQuickSortQueueItemDataIntPtr data;
        //                            data.lo = (IntPtr)veci;
        //                            data.hi = (IntPtr)vecHi;
        //                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
        //                                ILQuickSortQueueItemDataIntPtr d = (ILQuickSortQueueItemDataIntPtr)o;
        //                                QuickSortAsc_ITMT_noNaN(( double*)d.lo, ( double*)d.hi, inc);
        //                                Interlocked.Decrement(ref s_threadCount);
        //                                //s_threadCount--;
        //                            }, data);
        //                            vecHi = vecj - inc;
        //                        } else {
        //                            // smaller chunk now, bigger chunk to stack
        //                            if (QuickSortStack == null)
        //                                QuickSortStack = new IntPtr[50];
        //                            QuickSortStack[++s_quickSortStackPos] = (IntPtr) (vecj - inc);
        //                            QuickSortStack[++s_quickSortStackPos] = (IntPtr)vecLo;
        //                            vecLo = veci; 
        //                        }
        //                    } else {
        //                        if (s_threadCount < int.MaxValue //ILSettings.s_maxNumberThreads
        //                            && (vecHi - veci + inc) / inc > ILSettings.s_minParallelElement2Count) {
        //                            // do parallel: bigger chunk now, deliver smaller chunk to new thread 
        //                            Interlocked.Increment(ref s_threadCount);
        //                            //s_threadCount++;
        //                            ILQuickSortQueueItemDataIntPtr data;
        //                            data.lo = (IntPtr)vecLo;
        //                            data.hi = (IntPtr)(vecj - inc);
        //                            System.Threading.ThreadPool.QueueUserWorkItem(delegate(object o) {
        //                                ILQuickSortQueueItemDataIntPtr d = (ILQuickSortQueueItemDataIntPtr)o;
        //                                //Console.Out.WriteLine("Thread start: {0} ({1} - {2} Count:{3})", Thread.CurrentThread.ManagedThreadId, lo, hi, threadCount.ThreadCount);
        //                                QuickSortAsc_ITMT_noNaN(( double*)d.lo, ( double*)d.hi, inc);
        //                                Interlocked.Decrement(ref s_threadCount);
        //                                //s_threadCount--;
        //                            }, data);
        //                            vecLo = veci;
        //                        } else {
        //                            if (QuickSortStack == null)
        //                                QuickSortStack = new IntPtr[50];
        //                            QuickSortStack[++s_quickSortStackPos] = (IntPtr)vecHi;
        //                            QuickSortStack[++s_quickSortStackPos] = (IntPtr)veci;
        //                            vecHi = vecj - inc;
        //                        }
        //                    }
        //                }
        //                #endregion inner loop
        //            }
        //        }
    }
}
