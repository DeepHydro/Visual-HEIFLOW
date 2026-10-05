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


namespace ILNumerics.Misc {

    /// <summary>
    /// the class provides a number of one dimensional quicksort implementations for several datatypes/ properties
    /// </summary>
    [System.Security.SecuritySafeCritical]
    public partial class ILQuickSort {



    /// <summary>
    /// inline one dimensional quick sort, ascending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscST( double [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return; 
        int[] s_quickSortStackMT = ILMemoryPool.Pool.New<int>(Settings.s_maxSafeQuicksortRecursionDepth * 2);
        s_quickSortStackPos = -1;
         double a1, a2, a3; 
        unsafe {
        fixed (
             double * vec = vecP) {
             double temp, pivotItem;
            
            if (checkNaNAsc(lo, ref hi, inc, vec)) return;

            for (; ; ) {
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
                    // quicksort
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
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) < pivotItem); 
                        do j -= inc; while (*(vec + j) > pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline one dimensional quick sort, descending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescST( double [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1;
         double a1, a2, a3; 
        unsafe {
        fixed (
             double * vec = vecP) {
             double temp, pivotItem;                
             
            if (checkNaNDesc(lo, ref hi, inc, vec)) return;
            for (; ; ) {
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
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo); 
                    a2 = *(vec + pivotIndex); 
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) > pivotItem); 
                        do j -= inc; while (*(vec + j) < pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4D(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, ascending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MminimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscIDXST( double [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1; 
         double a1, a2, a3; 
         int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
             double * vec = vecP) {
             double temp, pivotItem;                
              
            if (checkNaNIDXAsc(lo, ref hi, inc, vecIdx, vec)) return; 
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, descending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescIDXST( double [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        
        s_quickSortStackPos = -1;
         double a1, a2, a3; 
         int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
             double * vec = vecP) {
             double temp, pivotItem;
            
            if (checkNaNIDXDesc(lo, ref hi, inc, vecIdx, vec)) return;
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                            *(vecIdx + hi) = ai1; 
                            *(vecIdx + lo) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                                *(vecIdx + hi) = ai1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                                //*(vecIdx + hi) = ai3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                                *(vecIdx + lo) = ai3; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                                //*(vecIdx + lo) = ai1; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                            //*(vecIdx + lo) = ai1; 
                            //*(vecIdx + hi) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }

#region HYCALPER AUTO GENERATED CODE


    /// <summary>
    /// inline one dimensional quick sort, ascending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscST( UInt64 [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return; 
        int[] s_quickSortStackMT = ILMemoryPool.Pool.New<int>(Settings.s_maxSafeQuicksortRecursionDepth * 2);
        s_quickSortStackPos = -1;
        UInt64 a1, a2, a3; 
        unsafe {
        fixed (
            UInt64 * vec = vecP) {
            UInt64 temp, pivotItem;
            

            for (; ; ) {
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
                    // quicksort
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
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) < pivotItem); 
                        do j -= inc; while (*(vec + j) > pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline one dimensional quick sort, descending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescST( UInt64 [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1;
        UInt64 a1, a2, a3; 
        unsafe {
        fixed (
            UInt64 * vec = vecP) {
            UInt64 temp, pivotItem;                
            
            for (; ; ) {
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
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo); 
                    a2 = *(vec + pivotIndex); 
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) > pivotItem); 
                        do j -= inc; while (*(vec + j) < pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4D(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, ascending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MminimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscIDXST( UInt64 [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1; 
        UInt64 a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            UInt64 * vec = vecP) {
            UInt64 temp, pivotItem;                
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, descending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescIDXST( UInt64 [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        
        s_quickSortStackPos = -1;
        UInt64 a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            UInt64 * vec = vecP) {
            UInt64 temp, pivotItem;
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                            *(vecIdx + hi) = ai1; 
                            *(vecIdx + lo) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                                *(vecIdx + hi) = ai1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                                //*(vecIdx + hi) = ai3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                                *(vecIdx + lo) = ai3; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                                //*(vecIdx + lo) = ai1; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                            //*(vecIdx + lo) = ai1; 
                            //*(vecIdx + hi) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }

    /// <summary>
    /// inline one dimensional quick sort, ascending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscST( UInt32 [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return; 
        int[] s_quickSortStackMT = ILMemoryPool.Pool.New<int>(Settings.s_maxSafeQuicksortRecursionDepth * 2);
        s_quickSortStackPos = -1;
        UInt32 a1, a2, a3; 
        unsafe {
        fixed (
            UInt32 * vec = vecP) {
            UInt32 temp, pivotItem;
            

            for (; ; ) {
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
                    // quicksort
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
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) < pivotItem); 
                        do j -= inc; while (*(vec + j) > pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline one dimensional quick sort, descending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescST( UInt32 [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1;
        UInt32 a1, a2, a3; 
        unsafe {
        fixed (
            UInt32 * vec = vecP) {
            UInt32 temp, pivotItem;                
            
            for (; ; ) {
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
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo); 
                    a2 = *(vec + pivotIndex); 
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) > pivotItem); 
                        do j -= inc; while (*(vec + j) < pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4D(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, ascending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MminimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscIDXST( UInt32 [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1; 
        UInt32 a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            UInt32 * vec = vecP) {
            UInt32 temp, pivotItem;                
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, descending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescIDXST( UInt32 [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        
        s_quickSortStackPos = -1;
        UInt32 a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            UInt32 * vec = vecP) {
            UInt32 temp, pivotItem;
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                            *(vecIdx + hi) = ai1; 
                            *(vecIdx + lo) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                                *(vecIdx + hi) = ai1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                                //*(vecIdx + hi) = ai3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                                *(vecIdx + lo) = ai3; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                                //*(vecIdx + lo) = ai1; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                            //*(vecIdx + lo) = ai1; 
                            //*(vecIdx + hi) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }

    /// <summary>
    /// inline one dimensional quick sort, ascending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscST( UInt16 [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return; 
        int[] s_quickSortStackMT = ILMemoryPool.Pool.New<int>(Settings.s_maxSafeQuicksortRecursionDepth * 2);
        s_quickSortStackPos = -1;
        UInt16 a1, a2, a3; 
        unsafe {
        fixed (
            UInt16 * vec = vecP) {
            UInt16 temp, pivotItem;
            

            for (; ; ) {
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
                    // quicksort
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
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) < pivotItem); 
                        do j -= inc; while (*(vec + j) > pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline one dimensional quick sort, descending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescST( UInt16 [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1;
        UInt16 a1, a2, a3; 
        unsafe {
        fixed (
            UInt16 * vec = vecP) {
            UInt16 temp, pivotItem;                
            
            for (; ; ) {
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
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo); 
                    a2 = *(vec + pivotIndex); 
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) > pivotItem); 
                        do j -= inc; while (*(vec + j) < pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4D(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, ascending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MminimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscIDXST( UInt16 [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1; 
        UInt16 a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            UInt16 * vec = vecP) {
            UInt16 temp, pivotItem;                
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, descending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescIDXST( UInt16 [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        
        s_quickSortStackPos = -1;
        UInt16 a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            UInt16 * vec = vecP) {
            UInt16 temp, pivotItem;
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                            *(vecIdx + hi) = ai1; 
                            *(vecIdx + lo) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                                *(vecIdx + hi) = ai1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                                //*(vecIdx + hi) = ai3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                                *(vecIdx + lo) = ai3; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                                //*(vecIdx + lo) = ai1; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                            //*(vecIdx + lo) = ai1; 
                            //*(vecIdx + hi) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }

    /// <summary>
    /// inline one dimensional quick sort, ascending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscST( Int64 [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return; 
        int[] s_quickSortStackMT = ILMemoryPool.Pool.New<int>(Settings.s_maxSafeQuicksortRecursionDepth * 2);
        s_quickSortStackPos = -1;
        Int64 a1, a2, a3; 
        unsafe {
        fixed (
            Int64 * vec = vecP) {
            Int64 temp, pivotItem;
            

            for (; ; ) {
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
                    // quicksort
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
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) < pivotItem); 
                        do j -= inc; while (*(vec + j) > pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline one dimensional quick sort, descending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescST( Int64 [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1;
        Int64 a1, a2, a3; 
        unsafe {
        fixed (
            Int64 * vec = vecP) {
            Int64 temp, pivotItem;                
            
            for (; ; ) {
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
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo); 
                    a2 = *(vec + pivotIndex); 
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) > pivotItem); 
                        do j -= inc; while (*(vec + j) < pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4D(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, ascending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MminimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscIDXST( Int64 [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1; 
        Int64 a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            Int64 * vec = vecP) {
            Int64 temp, pivotItem;                
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, descending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescIDXST( Int64 [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        
        s_quickSortStackPos = -1;
        Int64 a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            Int64 * vec = vecP) {
            Int64 temp, pivotItem;
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                            *(vecIdx + hi) = ai1; 
                            *(vecIdx + lo) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                                *(vecIdx + hi) = ai1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                                //*(vecIdx + hi) = ai3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                                *(vecIdx + lo) = ai3; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                                //*(vecIdx + lo) = ai1; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                            //*(vecIdx + lo) = ai1; 
                            //*(vecIdx + hi) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }

    /// <summary>
    /// inline one dimensional quick sort, ascending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscST( Int32 [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return; 
        int[] s_quickSortStackMT = ILMemoryPool.Pool.New<int>(Settings.s_maxSafeQuicksortRecursionDepth * 2);
        s_quickSortStackPos = -1;
        Int32 a1, a2, a3; 
        unsafe {
        fixed (
            Int32 * vec = vecP) {
            Int32 temp, pivotItem;
            

            for (; ; ) {
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
                    // quicksort
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
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) < pivotItem); 
                        do j -= inc; while (*(vec + j) > pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline one dimensional quick sort, descending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescST( Int32 [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1;
        Int32 a1, a2, a3; 
        unsafe {
        fixed (
            Int32 * vec = vecP) {
            Int32 temp, pivotItem;                
            
            for (; ; ) {
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
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo); 
                    a2 = *(vec + pivotIndex); 
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) > pivotItem); 
                        do j -= inc; while (*(vec + j) < pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4D(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, ascending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MminimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscIDXST( Int32 [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1; 
        Int32 a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            Int32 * vec = vecP) {
            Int32 temp, pivotItem;                
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, descending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescIDXST( Int32 [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        
        s_quickSortStackPos = -1;
        Int32 a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            Int32 * vec = vecP) {
            Int32 temp, pivotItem;
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                            *(vecIdx + hi) = ai1; 
                            *(vecIdx + lo) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                                *(vecIdx + hi) = ai1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                                //*(vecIdx + hi) = ai3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                                *(vecIdx + lo) = ai3; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                                //*(vecIdx + lo) = ai1; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                            //*(vecIdx + lo) = ai1; 
                            //*(vecIdx + hi) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }

    /// <summary>
    /// inline one dimensional quick sort, ascending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscST( Int16 [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return; 
        int[] s_quickSortStackMT = ILMemoryPool.Pool.New<int>(Settings.s_maxSafeQuicksortRecursionDepth * 2);
        s_quickSortStackPos = -1;
        Int16 a1, a2, a3; 
        unsafe {
        fixed (
            Int16 * vec = vecP) {
            Int16 temp, pivotItem;
            

            for (; ; ) {
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
                    // quicksort
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
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) < pivotItem); 
                        do j -= inc; while (*(vec + j) > pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline one dimensional quick sort, descending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescST( Int16 [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1;
        Int16 a1, a2, a3; 
        unsafe {
        fixed (
            Int16 * vec = vecP) {
            Int16 temp, pivotItem;                
            
            for (; ; ) {
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
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo); 
                    a2 = *(vec + pivotIndex); 
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) > pivotItem); 
                        do j -= inc; while (*(vec + j) < pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4D(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, ascending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MminimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscIDXST( Int16 [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1; 
        Int16 a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            Int16 * vec = vecP) {
            Int16 temp, pivotItem;                
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, descending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescIDXST( Int16 [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        
        s_quickSortStackPos = -1;
        Int16 a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            Int16 * vec = vecP) {
            Int16 temp, pivotItem;
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                            *(vecIdx + hi) = ai1; 
                            *(vecIdx + lo) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                                *(vecIdx + hi) = ai1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                                //*(vecIdx + hi) = ai3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                                *(vecIdx + lo) = ai3; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                                //*(vecIdx + lo) = ai1; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                            //*(vecIdx + lo) = ai1; 
                            //*(vecIdx + hi) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }

    /// <summary>
    /// inline one dimensional quick sort, ascending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscST( float [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return; 
        int[] s_quickSortStackMT = ILMemoryPool.Pool.New<int>(Settings.s_maxSafeQuicksortRecursionDepth * 2);
        s_quickSortStackPos = -1;
        float a1, a2, a3; 
        unsafe {
        fixed (
            float * vec = vecP) {
            float temp, pivotItem;
            if (checkNaNAsc(lo, ref hi, inc, vec)) return;

            for (; ; ) {
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
                    // quicksort
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
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) < pivotItem); 
                        do j -= inc; while (*(vec + j) > pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline one dimensional quick sort, descending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescST( float [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1;
        float a1, a2, a3; 
        unsafe {
        fixed (
            float * vec = vecP) {
            float temp, pivotItem;                
            if (checkNaNDesc(lo, ref hi, inc, vec)) return;
            for (; ; ) {
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
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo); 
                    a2 = *(vec + pivotIndex); 
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) > pivotItem); 
                        do j -= inc; while (*(vec + j) < pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4D(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, ascending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MminimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscIDXST( float [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1; 
        float a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            float * vec = vecP) {
            float temp, pivotItem;                
            if (checkNaNIDXAsc(lo, ref hi, inc, vecIdx, vec)) return;
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, descending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescIDXST( float [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        
        s_quickSortStackPos = -1;
        float a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            float * vec = vecP) {
            float temp, pivotItem;
            if (checkNaNIDXDesc(lo, ref hi, inc, vecIdx, vec)) return;
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                            *(vecIdx + hi) = ai1; 
                            *(vecIdx + lo) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                                *(vecIdx + hi) = ai1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                                //*(vecIdx + hi) = ai3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                                *(vecIdx + lo) = ai3; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                                //*(vecIdx + lo) = ai1; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                            //*(vecIdx + lo) = ai1; 
                            //*(vecIdx + hi) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }

    /// <summary>
    /// inline one dimensional quick sort, ascending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscST( fcomplex [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return; 
        int[] s_quickSortStackMT = ILMemoryPool.Pool.New<int>(Settings.s_maxSafeQuicksortRecursionDepth * 2);
        s_quickSortStackPos = -1;
        fcomplex a1, a2, a3; 
        unsafe {
        fixed (
            fcomplex * vec = vecP) {
            fcomplex temp, pivotItem;
            if (checkNaNAsc(lo, ref hi, inc, vec)) return;

            for (; ; ) {
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
                    // quicksort
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
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) < pivotItem); 
                        do j -= inc; while (*(vec + j) > pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline one dimensional quick sort, descending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescST( fcomplex [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1;
        fcomplex a1, a2, a3; 
        unsafe {
        fixed (
            fcomplex * vec = vecP) {
            fcomplex temp, pivotItem;                
            if (checkNaNDesc(lo, ref hi, inc, vec)) return;
            for (; ; ) {
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
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo); 
                    a2 = *(vec + pivotIndex); 
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) > pivotItem); 
                        do j -= inc; while (*(vec + j) < pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4D(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, ascending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MminimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscIDXST( fcomplex [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1; 
        fcomplex a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            fcomplex * vec = vecP) {
            fcomplex temp, pivotItem;                
            if (checkNaNIDXAsc(lo, ref hi, inc, vecIdx, vec)) return;
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, descending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescIDXST( fcomplex [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        
        s_quickSortStackPos = -1;
        fcomplex a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            fcomplex * vec = vecP) {
            fcomplex temp, pivotItem;
            if (checkNaNIDXDesc(lo, ref hi, inc, vecIdx, vec)) return;
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                            *(vecIdx + hi) = ai1; 
                            *(vecIdx + lo) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                                *(vecIdx + hi) = ai1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                                //*(vecIdx + hi) = ai3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                                *(vecIdx + lo) = ai3; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                                //*(vecIdx + lo) = ai1; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                            //*(vecIdx + lo) = ai1; 
                            //*(vecIdx + hi) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }

    /// <summary>
    /// inline one dimensional quick sort, ascending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscST( complex [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return; 
        int[] s_quickSortStackMT = ILMemoryPool.Pool.New<int>(Settings.s_maxSafeQuicksortRecursionDepth * 2);
        s_quickSortStackPos = -1;
        complex a1, a2, a3; 
        unsafe {
        fixed (
            complex * vec = vecP) {
            complex temp, pivotItem;
            if (checkNaNAsc(lo, ref hi, inc, vec)) return;

            for (; ; ) {
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
                    // quicksort
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
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) < pivotItem); 
                        do j -= inc; while (*(vec + j) > pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline one dimensional quick sort, descending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescST( complex [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1;
        complex a1, a2, a3; 
        unsafe {
        fixed (
            complex * vec = vecP) {
            complex temp, pivotItem;                
            if (checkNaNDesc(lo, ref hi, inc, vec)) return;
            for (; ; ) {
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
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo); 
                    a2 = *(vec + pivotIndex); 
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) > pivotItem); 
                        do j -= inc; while (*(vec + j) < pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4D(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, ascending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MminimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscIDXST( complex [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1; 
        complex a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            complex * vec = vecP) {
            complex temp, pivotItem;                
            if (checkNaNIDXAsc(lo, ref hi, inc, vecIdx, vec)) return;
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, descending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescIDXST( complex [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        
        s_quickSortStackPos = -1;
        complex a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            complex * vec = vecP) {
            complex temp, pivotItem;
            if (checkNaNIDXDesc(lo, ref hi, inc, vecIdx, vec)) return;
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                            *(vecIdx + hi) = ai1; 
                            *(vecIdx + lo) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                                *(vecIdx + hi) = ai1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                                //*(vecIdx + hi) = ai3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                                *(vecIdx + lo) = ai3; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                                //*(vecIdx + lo) = ai1; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                            //*(vecIdx + lo) = ai1; 
                            //*(vecIdx + hi) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }

    /// <summary>
    /// inline one dimensional quick sort, ascending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscST( char [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return; 
        int[] s_quickSortStackMT = ILMemoryPool.Pool.New<int>(Settings.s_maxSafeQuicksortRecursionDepth * 2);
        s_quickSortStackPos = -1;
        char a1, a2, a3; 
        unsafe {
        fixed (
            char * vec = vecP) {
            char temp, pivotItem;
            

            for (; ; ) {
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
                    // quicksort
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
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) < pivotItem); 
                        do j -= inc; while (*(vec + j) > pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline one dimensional quick sort, descending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescST( char [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1;
        char a1, a2, a3; 
        unsafe {
        fixed (
            char * vec = vecP) {
            char temp, pivotItem;                
            
            for (; ; ) {
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
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo); 
                    a2 = *(vec + pivotIndex); 
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) > pivotItem); 
                        do j -= inc; while (*(vec + j) < pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4D(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, ascending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MminimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscIDXST( char [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1; 
        char a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            char * vec = vecP) {
            char temp, pivotItem;                
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, descending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescIDXST( char [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        
        s_quickSortStackPos = -1;
        char a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            char * vec = vecP) {
            char temp, pivotItem;
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                            *(vecIdx + hi) = ai1; 
                            *(vecIdx + lo) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                                *(vecIdx + hi) = ai1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                                //*(vecIdx + hi) = ai3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                                *(vecIdx + lo) = ai3; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                                //*(vecIdx + lo) = ai1; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                            //*(vecIdx + lo) = ai1; 
                            //*(vecIdx + hi) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }

    /// <summary>
    /// inline one dimensional quick sort, ascending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscST( byte [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return; 
        int[] s_quickSortStackMT = ILMemoryPool.Pool.New<int>(Settings.s_maxSafeQuicksortRecursionDepth * 2);
        s_quickSortStackPos = -1;
        byte a1, a2, a3; 
        unsafe {
        fixed (
            byte * vec = vecP) {
            byte temp, pivotItem;
            

            for (; ; ) {
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
                    // quicksort
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
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) < pivotItem); 
                        do j -= inc; while (*(vec + j) > pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4A(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline one dimensional quick sort, descending, arbitrary element spacing
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescST( byte [] vecP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1;
        byte a1, a2, a3; 
        unsafe {
        fixed (
            byte * vec = vecP) {
            byte temp, pivotItem;                
            
            for (; ; ) {
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
                    // quicksort
                    pivotIndex = lo + (int)((hi - lo) / inc / 2) * inc;
                    a1 = *(vec + lo); 
                    a2 = *(vec + pivotIndex); 
                    a3 = *(vec + hi);
                    *(vec + pivotIndex) = *(vec + lo + inc);
                    #region bring lo, lo+inc, hi in order
                    // by explicitely stepping through all possible paths,
                    // we save some assignments and comparisons
                    if (a1 < a2) {
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    for (;;) {
                        do i += inc; while (*(vec + i) > pivotItem); 
                        do j -= inc; while (*(vec + j) < pivotItem); 
                        if (j < i) break; 
                        temp = *(vec + i); 
                        *(vec + i) = *(vec + j); 
                        *(vec + j) = temp; 
                    }
                    *(vec + lo + inc) = *(vec + j); 
                    *(vec + j) = pivotItem;
#if DEBUG
                    //System.Diagnostics.Debug.Assert(Check4D(vecP,lo,hi,j,inc)); 
#endif 
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, ascending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MminimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortAscIDXST( byte [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        s_quickSortStackPos = -1; 
        byte a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            byte * vec = vecP) {
            byte temp, pivotItem;                
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }
    /// <summary>
    /// inline 1 dimensional quick sort, descending, arbitrary element spacing, indices aware
    /// </summary>
    /// <param name="vecP">data array, 1dim, replaced on output with sorted values</param>
    /// <param name="lo">lowest index of sorting range</param>
    /// <param name="hi">highest index of sorting range</param>
    /// <param name="idxP">indices vector, content will in-place be sorted along with <paramref name="vecP"/></param>
    /// <param name="inc">spacing between elements (dimension specifier)</param>
    /// <remarks><para>Quick sort algorithm is used for ranges of minimum lengths specified by 
    /// <see cref="P:ILNumerics.Settings.MinimumQuicksortLength"/> and above. 
    /// Below that length, a simple insertion sort is used instead.</para>
    /// <para>The algorithm implements a non-recursive quicksort variant. Therefore a small amount of 
    /// memory is needed to perform the sorting. That 'stack' memory is preserved from the ILMemoryPool. 
    /// The size is determined by the value of 
    /// <see cref="P:ILNumerics.Settings.MaxSafeQuicksortRecursionDepth"/>.</para>
    /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
    /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para></remarks>
    public static void QuickSortDescIDXST( byte [] vecP,  int [] idxP, int lo, int hi, int inc) {
        System.Diagnostics.Debug.Assert(vecP != null); 
        System.Diagnostics.Debug.Assert(lo <= hi);
        System.Diagnostics.Debug.Assert(lo >= 0);
        //int loSwap, hiSwap;
        int pivotIndex, i, j;
        if (vecP.Length < 2) return;
        
        s_quickSortStackPos = -1;
        byte a1, a2, a3; 
        int ai1, ai2, ai3; 
        unsafe {
        fixed ( int * vecIdx = idxP)
        fixed (
            byte * vec = vecP) {
            byte temp, pivotItem;
            
            for (; ; ) {
                if (hi - lo < (Settings.s_minimumQuicksortLength * inc)) {
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
                        if (a2 < a3) {
                            *(vec + hi) = a1; 
                            *(vec + lo) = a3; 
                            *(vec + lo + inc) = a2; 
                            *(vecIdx + hi) = ai1; 
                            *(vecIdx + lo) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        } else {
                            if (a1 < a3) {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a3; 
                                *(vec + hi) = a1; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                                *(vecIdx + hi) = ai1; 
                            } else {
                                *(vec + lo) = a2; 
                                *(vec + lo + inc) = a1; 
                                //*(vec + hi) = a3; 
                                *(vecIdx + lo) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                                //*(vecIdx + hi) = ai3; 
                            }
                        }
                    } else {
                        if (a2 < a3) {
                            if (a1 < a3) {
                                *(vec + lo) = a3; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a1; 
                                *(vecIdx + lo) = ai3; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai1; 
                            } else {
                                //*(vec + lo) = a1; 
                                *(vec + hi) = a2; 
                                *(vec + lo + inc) = a3; 
                                //*(vecIdx + lo) = ai1; 
                                *(vecIdx + hi) = ai2; 
                                *(vecIdx + lo + inc) = ai3; 
                            }
                        } else {
                            //*(vec + lo) = a1; 
                            //*(vec + hi) = a3; 
                            *(vec + lo + inc) = a2; 
                            //*(vecIdx + lo) = ai1; 
                            //*(vecIdx + hi) = ai3; 
                            *(vecIdx + lo + inc) = ai2; 
                        }
                    }
                    #endregion
                    i = lo + inc; j = hi; 
                    pivotItem = *(vec + lo + inc);
                    ai1 = *(vecIdx + lo + inc);
                    for (;;) {
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
                    if (hi - i + inc >= j - lo) {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = hi; 
                        s_quickSortStackMT[++s_quickSortStackPos] = i; 
                        hi = j - inc; 
                    } else {
                        if (s_quickSortStackMT == null)
                            s_quickSortStackMT = new int[100];
                        s_quickSortStackMT[++s_quickSortStackPos] = j - inc; 
                        s_quickSortStackMT[++s_quickSortStackPos] = lo; 
                        lo = i; 
                    }
                }
            }
        } // fixed
        } // unsafe 
    }

#endregion HYCALPER AUTO GENERATED CODE

#region Debug assertion checks
        private static bool Check4A<T>(T[] vec, int lo, int hi, int pivIndex, int inc) {
            double piv = Convert.ToDouble(vec[pivIndex].ToString()); 
            for (int i = lo; i <= pivIndex; i += inc) {
                if (Convert.ToDouble(vec[i].ToString()) > piv) {
                    return false; 
                }
            }
            for (int i = hi; i >= pivIndex; i -= inc) {
                if (Convert.ToDouble(vec[i].ToString()) < piv) {
                    return false; 
                }
            }
            return true; 
        }
        private static bool Check4D<T>(T[] vec, int lo, int hi, int pivIndex, int inc) {
            double piv = Convert.ToDouble(vec[pivIndex].ToString()); 
            for (int i = lo; i <= pivIndex; i += inc) {
                if (Convert.ToDouble(vec[i].ToString()) < piv) {
                    return false; 
                }
            }
            for (int i = hi; i >= pivIndex; i -= inc) {
                if (Convert.ToDouble(vec[i].ToString()) > piv) {
                    return false; 
                }
            }
            return true; 
        }
        private static bool Check3(double[] vec, int hiBound, int loBound, int loSwap, int hiSwap, double pivotItem) {
            for (int i = loBound; i < loSwap - 1; i++) {
                if (!(vec[i] <= pivotItem)) {
                    return false; 
                }
            }
            for (int i = hiSwap + 1; i < hiBound; i++) {
                if (!(vec[i] > pivotItem)) {
                    return false; 
                }
            }
            if (loSwap < hiSwap) {
                if (!(vec[loSwap] <= pivotItem && pivotItem < vec[hiSwap])) {
                    return false; 
                }
            } else {
                if (!(vec[hiSwap] <= pivotItem && (loBound <= loSwap) && (loSwap <= hiSwap + 1) && (hiSwap + 1 <= hiBound + 1))) {
                    return false; 
                }
            }
            return true; 
        }
        private static bool Check1(double[] vec, int loBound, int loSwap, int hiSwap, double pivotItem) {
            for (int i = loBound + 1; i < loSwap - 1; i++) {
                if (!(vec[i] <= pivotItem && loSwap <= hiSwap + 1)) {
                    return false; 
                }
            }
            return true; 
        }
        private static bool Check2(double[] vec, int hiBound, int loSwap, int hiSwap, double pivotItem) {
            for (int i = hiSwap + 1; i < hiBound; i++) {
                if (!(vec[i] > pivotItem && loSwap <= hiSwap + 1)) {
                    return false; 
                }
            }
            return true;
        }
#endregion 

    }
}
