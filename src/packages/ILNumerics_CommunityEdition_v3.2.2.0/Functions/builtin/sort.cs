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
using ILNumerics.Data; 
using ILNumerics.Misc; 


namespace ILNumerics {
    public partial class ILMath {


        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted in ascending order using a non-recursive quick sort algorithm. 
        /// Elements along the columns of A will get sorted independently from each other. A is not altered.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< double > sort (ILInArray< double > A) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, false);
            }
        }
        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Determine the direction of sorting (ascending/ descending)</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted using the quick sort algorithm. Data 
        /// along the first non singleton dimension will get sorted independently from data 
        /// in the next row/column.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< double > sort (ILInArray< double > A, bool descending) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, descending);
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< double > sort (ILInArray< double > A, int dim, bool descending) {
            if (object.Equals (A,null)) 
                throw new Exception("parameter A must not be null");
            using (ILScope.Enter(A)) {
                if (dim < 0) // || dim >= A.Dimensions.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
                // early exit: scalar/ empty
                if (A.IsEmpty || A.IsScalar)
                    return A.C;
                ILArray< double> ret = A.C; // new ILArray<double>(new Storage.ILDenseStorage<double>(ILMemoryPool.Pool.New<double>(A.Dimensions.NumberOfElements), A.Dimensions));
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                        
                        double[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (int)(((long)dimLen * c * inc) % (A.Size.NumberOfElements - 1));
                                ILQuickSort.QuickSortDescST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                        
					    double[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                         
                        double[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                        
                        double[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                //if (descending) {
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortDescSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //} else {
                //    // ascending
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortAscSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //}
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <param name="Indices">[Output] Returns permutation matrix</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>This overload also returns an array 'Indices' which will hold the indices into the original 
        /// elements <b>after sorting</b>. Elements of 'Indices' are of type double.</para>
        /// <example><code>ILArray&lt;double&gt; A = ILMath.rand(1,5); 
        /// //A: 
        /// // 0.4324  0.9231  0.1231  0.1423  0.2991
        /// ILArray&lt;double&gt; I; 
        /// ILArray&lt;double&gt; R = ILMath.sort(A, out I, 1, false);
        /// //R: 
        /// // 0.1231  0.1423  0.2991  0.4324  0.9231  
        /// //I: 
        /// // 2  3  4  0  1
        /// </code>
        /// </example>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< double> sort( ILInArray< double> A, ILOutArray<int> Indices = null, int dim = -1, bool descending = false ) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null))
                    throw new Exception("parameter A must not be null");
                if (object.Equals(Indices, null)) {
                    return sort(A,dim,descending); 
                }
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                // early exit: scalar/ empty
                if (A.IsEmpty) {
                    Indices.a = empty<int>(ILSize.Empty00);
                    return empty< double>(ILSize.Empty00);
                }
                if (A.IsScalar) {
                    Indices.a = 0;
                    return A.C;
                }
                if (dim < 0) {
                    dim = A.S.WorkingDimension(); 
                }
                int[] tmpDims = A.Size.ToIntArray();
                for (int i = 0; i < A.Size.NumberOfDimensions; i++) {
                    if (i == dim)
                        tmpDims[i] = A.Size[dim];
                    else
                        tmpDims[i] = 1;
                }
                //if (Indices.IsEmpty || !Indices.D.IsSameSize(A.D)) {
                Indices.a = reshape(counter<int>(0, 1, size(1, A.Size[dim])), tmpDims);
                tmpDims = A.Size.ToIntArray();
                tmpDims[dim] = 1;
                Indices.a = repmat(Indices, tmpDims);
                //}
                ILArray<double> ret = A.C;
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                        double [] retArr = ret.GetArrayForWrite(); 
                        int[] indArr = Indices.GetArrayForRead(); 
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements-1);
                                ILQuickSort.QuickSortDescIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                        }); 
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                        
                        double[] retArr = ret.GetArrayForWrite();
                        int[] indArr = Indices.GetArrayForRead();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                return ret;
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted in ascending order using a non-recursive quick sort algorithm. 
        /// Elements along the columns of A will get sorted independently from each other. A is not altered.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< Int64 > sort (ILInArray< Int64 > A) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, false);
            }
        }
        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Determine the direction of sorting (ascending/ descending)</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted using the quick sort algorithm. Data 
        /// along the first non singleton dimension will get sorted independently from data 
        /// in the next row/column.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< Int64 > sort (ILInArray< Int64 > A, bool descending) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, descending);
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< Int64 > sort (ILInArray< Int64 > A, int dim, bool descending) {
            if (object.Equals (A,null)) 
                throw new Exception("parameter A must not be null");
            using (ILScope.Enter(A)) {
                if (dim < 0) // || dim >= A.Dimensions.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
                // early exit: scalar/ empty
                if (A.IsEmpty || A.IsScalar)
                    return A.C;
                ILArray< Int64> ret = A.C; // new ILArray<double>(new Storage.ILDenseStorage<double>(ILMemoryPool.Pool.New<double>(A.Dimensions.NumberOfElements), A.Dimensions));
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       
                        Int64[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (int)(((long)dimLen * c * inc) % (A.Size.NumberOfElements - 1));
                                ILQuickSort.QuickSortDescST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                       
					    Int64[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                        
                        Int64[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                       
                        Int64[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                //if (descending) {
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortDescSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //} else {
                //    // ascending
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortAscSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //}
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <param name="Indices">[Output] Returns permutation matrix</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>This overload also returns an array 'Indices' which will hold the indices into the original 
        /// elements <b>after sorting</b>. Elements of 'Indices' are of type double.</para>
        /// <example><code>ILArray&lt;double&gt; A = ILMath.rand(1,5); 
        /// //A: 
        /// // 0.4324  0.9231  0.1231  0.1423  0.2991
        /// ILArray&lt;double&gt; I; 
        /// ILArray&lt;double&gt; R = ILMath.sort(A, out I, 1, false);
        /// //R: 
        /// // 0.1231  0.1423  0.2991  0.4324  0.9231  
        /// //I: 
        /// // 2  3  4  0  1
        /// </code>
        /// </example>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< Int64> sort( ILInArray< Int64> A, ILOutArray<int> Indices = null, int dim = -1, bool descending = false ) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null))
                    throw new Exception("parameter A must not be null");
                if (object.Equals(Indices, null)) {
                    return sort(A,dim,descending); 
                }
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                // early exit: scalar/ empty
                if (A.IsEmpty) {
                    Indices.a = empty<int>(ILSize.Empty00);
                    return empty< Int64>(ILSize.Empty00);
                }
                if (A.IsScalar) {
                    Indices.a = 0;
                    return A.C;
                }
                if (dim < 0) {
                    dim = A.S.WorkingDimension(); 
                }
                int[] tmpDims = A.Size.ToIntArray();
                for (int i = 0; i < A.Size.NumberOfDimensions; i++) {
                    if (i == dim)
                        tmpDims[i] = A.Size[dim];
                    else
                        tmpDims[i] = 1;
                }
                //if (Indices.IsEmpty || !Indices.D.IsSameSize(A.D)) {
                Indices.a = reshape(counter<int>(0, 1, size(1, A.Size[dim])), tmpDims);
                tmpDims = A.Size.ToIntArray();
                tmpDims[dim] = 1;
                Indices.a = repmat(Indices, tmpDims);
                //}
                ILArray<Int64> ret = A.C;
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       Int64 [] retArr = ret.GetArrayForWrite(); 
                        int[] indArr = Indices.GetArrayForRead(); 
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements-1);
                                ILQuickSort.QuickSortDescIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                        }); 
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       
                        Int64[] retArr = ret.GetArrayForWrite();
                        int[] indArr = Indices.GetArrayForRead();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted in ascending order using a non-recursive quick sort algorithm. 
        /// Elements along the columns of A will get sorted independently from each other. A is not altered.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< Int32 > sort (ILInArray< Int32 > A) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, false);
            }
        }
        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Determine the direction of sorting (ascending/ descending)</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted using the quick sort algorithm. Data 
        /// along the first non singleton dimension will get sorted independently from data 
        /// in the next row/column.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< Int32 > sort (ILInArray< Int32 > A, bool descending) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, descending);
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< Int32 > sort (ILInArray< Int32 > A, int dim, bool descending) {
            if (object.Equals (A,null)) 
                throw new Exception("parameter A must not be null");
            using (ILScope.Enter(A)) {
                if (dim < 0) // || dim >= A.Dimensions.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
                // early exit: scalar/ empty
                if (A.IsEmpty || A.IsScalar)
                    return A.C;
                ILArray< Int32> ret = A.C; // new ILArray<double>(new Storage.ILDenseStorage<double>(ILMemoryPool.Pool.New<double>(A.Dimensions.NumberOfElements), A.Dimensions));
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       
                        Int32[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (int)(((long)dimLen * c * inc) % (A.Size.NumberOfElements - 1));
                                ILQuickSort.QuickSortDescST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                       
					    Int32[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                        
                        Int32[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                       
                        Int32[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                //if (descending) {
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortDescSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //} else {
                //    // ascending
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortAscSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //}
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <param name="Indices">[Output] Returns permutation matrix</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>This overload also returns an array 'Indices' which will hold the indices into the original 
        /// elements <b>after sorting</b>. Elements of 'Indices' are of type double.</para>
        /// <example><code>ILArray&lt;double&gt; A = ILMath.rand(1,5); 
        /// //A: 
        /// // 0.4324  0.9231  0.1231  0.1423  0.2991
        /// ILArray&lt;double&gt; I; 
        /// ILArray&lt;double&gt; R = ILMath.sort(A, out I, 1, false);
        /// //R: 
        /// // 0.1231  0.1423  0.2991  0.4324  0.9231  
        /// //I: 
        /// // 2  3  4  0  1
        /// </code>
        /// </example>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< Int32> sort( ILInArray< Int32> A, ILOutArray<int> Indices = null, int dim = -1, bool descending = false ) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null))
                    throw new Exception("parameter A must not be null");
                if (object.Equals(Indices, null)) {
                    return sort(A,dim,descending); 
                }
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                // early exit: scalar/ empty
                if (A.IsEmpty) {
                    Indices.a = empty<int>(ILSize.Empty00);
                    return empty< Int32>(ILSize.Empty00);
                }
                if (A.IsScalar) {
                    Indices.a = 0;
                    return A.C;
                }
                if (dim < 0) {
                    dim = A.S.WorkingDimension(); 
                }
                int[] tmpDims = A.Size.ToIntArray();
                for (int i = 0; i < A.Size.NumberOfDimensions; i++) {
                    if (i == dim)
                        tmpDims[i] = A.Size[dim];
                    else
                        tmpDims[i] = 1;
                }
                //if (Indices.IsEmpty || !Indices.D.IsSameSize(A.D)) {
                Indices.a = reshape(counter<int>(0, 1, size(1, A.Size[dim])), tmpDims);
                tmpDims = A.Size.ToIntArray();
                tmpDims[dim] = 1;
                Indices.a = repmat(Indices, tmpDims);
                //}
                ILArray<Int32> ret = A.C;
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       Int32 [] retArr = ret.GetArrayForWrite(); 
                        int[] indArr = Indices.GetArrayForRead(); 
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements-1);
                                ILQuickSort.QuickSortDescIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                        }); 
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       
                        Int32[] retArr = ret.GetArrayForWrite();
                        int[] indArr = Indices.GetArrayForRead();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted in ascending order using a non-recursive quick sort algorithm. 
        /// Elements along the columns of A will get sorted independently from each other. A is not altered.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< float > sort (ILInArray< float > A) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, false);
            }
        }
        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Determine the direction of sorting (ascending/ descending)</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted using the quick sort algorithm. Data 
        /// along the first non singleton dimension will get sorted independently from data 
        /// in the next row/column.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< float > sort (ILInArray< float > A, bool descending) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, descending);
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< float > sort (ILInArray< float > A, int dim, bool descending) {
            if (object.Equals (A,null)) 
                throw new Exception("parameter A must not be null");
            using (ILScope.Enter(A)) {
                if (dim < 0) // || dim >= A.Dimensions.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
                // early exit: scalar/ empty
                if (A.IsEmpty || A.IsScalar)
                    return A.C;
                ILArray< float> ret = A.C; // new ILArray<double>(new Storage.ILDenseStorage<double>(ILMemoryPool.Pool.New<double>(A.Dimensions.NumberOfElements), A.Dimensions));
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       
                        float[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (int)(((long)dimLen * c * inc) % (A.Size.NumberOfElements - 1));
                                ILQuickSort.QuickSortDescST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                       
					    float[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                        
                        float[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                       
                        float[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                //if (descending) {
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortDescSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //} else {
                //    // ascending
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortAscSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //}
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <param name="Indices">[Output] Returns permutation matrix</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>This overload also returns an array 'Indices' which will hold the indices into the original 
        /// elements <b>after sorting</b>. Elements of 'Indices' are of type double.</para>
        /// <example><code>ILArray&lt;double&gt; A = ILMath.rand(1,5); 
        /// //A: 
        /// // 0.4324  0.9231  0.1231  0.1423  0.2991
        /// ILArray&lt;double&gt; I; 
        /// ILArray&lt;double&gt; R = ILMath.sort(A, out I, 1, false);
        /// //R: 
        /// // 0.1231  0.1423  0.2991  0.4324  0.9231  
        /// //I: 
        /// // 2  3  4  0  1
        /// </code>
        /// </example>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< float> sort( ILInArray< float> A, ILOutArray<int> Indices = null, int dim = -1, bool descending = false ) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null))
                    throw new Exception("parameter A must not be null");
                if (object.Equals(Indices, null)) {
                    return sort(A,dim,descending); 
                }
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                // early exit: scalar/ empty
                if (A.IsEmpty) {
                    Indices.a = empty<int>(ILSize.Empty00);
                    return empty< float>(ILSize.Empty00);
                }
                if (A.IsScalar) {
                    Indices.a = 0;
                    return A.C;
                }
                if (dim < 0) {
                    dim = A.S.WorkingDimension(); 
                }
                int[] tmpDims = A.Size.ToIntArray();
                for (int i = 0; i < A.Size.NumberOfDimensions; i++) {
                    if (i == dim)
                        tmpDims[i] = A.Size[dim];
                    else
                        tmpDims[i] = 1;
                }
                //if (Indices.IsEmpty || !Indices.D.IsSameSize(A.D)) {
                Indices.a = reshape(counter<int>(0, 1, size(1, A.Size[dim])), tmpDims);
                tmpDims = A.Size.ToIntArray();
                tmpDims[dim] = 1;
                Indices.a = repmat(Indices, tmpDims);
                //}
                ILArray<float> ret = A.C;
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       float [] retArr = ret.GetArrayForWrite(); 
                        int[] indArr = Indices.GetArrayForRead(); 
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements-1);
                                ILQuickSort.QuickSortDescIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                        }); 
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       
                        float[] retArr = ret.GetArrayForWrite();
                        int[] indArr = Indices.GetArrayForRead();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted in ascending order using a non-recursive quick sort algorithm. 
        /// Elements along the columns of A will get sorted independently from each other. A is not altered.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< fcomplex > sort (ILInArray< fcomplex > A) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, false);
            }
        }
        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Determine the direction of sorting (ascending/ descending)</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted using the quick sort algorithm. Data 
        /// along the first non singleton dimension will get sorted independently from data 
        /// in the next row/column.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< fcomplex > sort (ILInArray< fcomplex > A, bool descending) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, descending);
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< fcomplex > sort (ILInArray< fcomplex > A, int dim, bool descending) {
            if (object.Equals (A,null)) 
                throw new Exception("parameter A must not be null");
            using (ILScope.Enter(A)) {
                if (dim < 0) // || dim >= A.Dimensions.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
                // early exit: scalar/ empty
                if (A.IsEmpty || A.IsScalar)
                    return A.C;
                ILArray< fcomplex> ret = A.C; // new ILArray<double>(new Storage.ILDenseStorage<double>(ILMemoryPool.Pool.New<double>(A.Dimensions.NumberOfElements), A.Dimensions));
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       
                        fcomplex[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (int)(((long)dimLen * c * inc) % (A.Size.NumberOfElements - 1));
                                ILQuickSort.QuickSortDescST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                       
					    fcomplex[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                        
                        fcomplex[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                       
                        fcomplex[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                //if (descending) {
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortDescSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //} else {
                //    // ascending
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortAscSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //}
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <param name="Indices">[Output] Returns permutation matrix</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>This overload also returns an array 'Indices' which will hold the indices into the original 
        /// elements <b>after sorting</b>. Elements of 'Indices' are of type double.</para>
        /// <example><code>ILArray&lt;double&gt; A = ILMath.rand(1,5); 
        /// //A: 
        /// // 0.4324  0.9231  0.1231  0.1423  0.2991
        /// ILArray&lt;double&gt; I; 
        /// ILArray&lt;double&gt; R = ILMath.sort(A, out I, 1, false);
        /// //R: 
        /// // 0.1231  0.1423  0.2991  0.4324  0.9231  
        /// //I: 
        /// // 2  3  4  0  1
        /// </code>
        /// </example>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< fcomplex> sort( ILInArray< fcomplex> A, ILOutArray<int> Indices = null, int dim = -1, bool descending = false ) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null))
                    throw new Exception("parameter A must not be null");
                if (object.Equals(Indices, null)) {
                    return sort(A,dim,descending); 
                }
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                // early exit: scalar/ empty
                if (A.IsEmpty) {
                    Indices.a = empty<int>(ILSize.Empty00);
                    return empty< fcomplex>(ILSize.Empty00);
                }
                if (A.IsScalar) {
                    Indices.a = 0;
                    return A.C;
                }
                if (dim < 0) {
                    dim = A.S.WorkingDimension(); 
                }
                int[] tmpDims = A.Size.ToIntArray();
                for (int i = 0; i < A.Size.NumberOfDimensions; i++) {
                    if (i == dim)
                        tmpDims[i] = A.Size[dim];
                    else
                        tmpDims[i] = 1;
                }
                //if (Indices.IsEmpty || !Indices.D.IsSameSize(A.D)) {
                Indices.a = reshape(counter<int>(0, 1, size(1, A.Size[dim])), tmpDims);
                tmpDims = A.Size.ToIntArray();
                tmpDims[dim] = 1;
                Indices.a = repmat(Indices, tmpDims);
                //}
                ILArray<fcomplex> ret = A.C;
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       fcomplex [] retArr = ret.GetArrayForWrite(); 
                        int[] indArr = Indices.GetArrayForRead(); 
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements-1);
                                ILQuickSort.QuickSortDescIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                        }); 
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       
                        fcomplex[] retArr = ret.GetArrayForWrite();
                        int[] indArr = Indices.GetArrayForRead();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted in ascending order using a non-recursive quick sort algorithm. 
        /// Elements along the columns of A will get sorted independently from each other. A is not altered.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< complex > sort (ILInArray< complex > A) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, false);
            }
        }
        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Determine the direction of sorting (ascending/ descending)</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted using the quick sort algorithm. Data 
        /// along the first non singleton dimension will get sorted independently from data 
        /// in the next row/column.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< complex > sort (ILInArray< complex > A, bool descending) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, descending);
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< complex > sort (ILInArray< complex > A, int dim, bool descending) {
            if (object.Equals (A,null)) 
                throw new Exception("parameter A must not be null");
            using (ILScope.Enter(A)) {
                if (dim < 0) // || dim >= A.Dimensions.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
                // early exit: scalar/ empty
                if (A.IsEmpty || A.IsScalar)
                    return A.C;
                ILArray< complex> ret = A.C; // new ILArray<double>(new Storage.ILDenseStorage<double>(ILMemoryPool.Pool.New<double>(A.Dimensions.NumberOfElements), A.Dimensions));
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       
                        complex[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (int)(((long)dimLen * c * inc) % (A.Size.NumberOfElements - 1));
                                ILQuickSort.QuickSortDescST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                       
					    complex[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                        
                        complex[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                       
                        complex[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                //if (descending) {
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortDescSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //} else {
                //    // ascending
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortAscSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //}
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <param name="Indices">[Output] Returns permutation matrix</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>This overload also returns an array 'Indices' which will hold the indices into the original 
        /// elements <b>after sorting</b>. Elements of 'Indices' are of type double.</para>
        /// <example><code>ILArray&lt;double&gt; A = ILMath.rand(1,5); 
        /// //A: 
        /// // 0.4324  0.9231  0.1231  0.1423  0.2991
        /// ILArray&lt;double&gt; I; 
        /// ILArray&lt;double&gt; R = ILMath.sort(A, out I, 1, false);
        /// //R: 
        /// // 0.1231  0.1423  0.2991  0.4324  0.9231  
        /// //I: 
        /// // 2  3  4  0  1
        /// </code>
        /// </example>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< complex> sort( ILInArray< complex> A, ILOutArray<int> Indices = null, int dim = -1, bool descending = false ) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null))
                    throw new Exception("parameter A must not be null");
                if (object.Equals(Indices, null)) {
                    return sort(A,dim,descending); 
                }
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                // early exit: scalar/ empty
                if (A.IsEmpty) {
                    Indices.a = empty<int>(ILSize.Empty00);
                    return empty< complex>(ILSize.Empty00);
                }
                if (A.IsScalar) {
                    Indices.a = 0;
                    return A.C;
                }
                if (dim < 0) {
                    dim = A.S.WorkingDimension(); 
                }
                int[] tmpDims = A.Size.ToIntArray();
                for (int i = 0; i < A.Size.NumberOfDimensions; i++) {
                    if (i == dim)
                        tmpDims[i] = A.Size[dim];
                    else
                        tmpDims[i] = 1;
                }
                //if (Indices.IsEmpty || !Indices.D.IsSameSize(A.D)) {
                Indices.a = reshape(counter<int>(0, 1, size(1, A.Size[dim])), tmpDims);
                tmpDims = A.Size.ToIntArray();
                tmpDims[dim] = 1;
                Indices.a = repmat(Indices, tmpDims);
                //}
                ILArray<complex> ret = A.C;
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       complex [] retArr = ret.GetArrayForWrite(); 
                        int[] indArr = Indices.GetArrayForRead(); 
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements-1);
                                ILQuickSort.QuickSortDescIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                        }); 
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       
                        complex[] retArr = ret.GetArrayForWrite();
                        int[] indArr = Indices.GetArrayForRead();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted in ascending order using a non-recursive quick sort algorithm. 
        /// Elements along the columns of A will get sorted independently from each other. A is not altered.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< byte > sort (ILInArray< byte > A) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, false);
            }
        }
        /// <summary>
        /// Sort data in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Determine the direction of sorting (ascending/ descending)</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted using the quick sort algorithm. Data 
        /// along the first non singleton dimension will get sorted independently from data 
        /// in the next row/column.
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< byte > sort (ILInArray< byte > A, bool descending) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, descending);
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< byte > sort (ILInArray< byte > A, int dim, bool descending) {
            if (object.Equals (A,null)) 
                throw new Exception("parameter A must not be null");
            using (ILScope.Enter(A)) {
                if (dim < 0) // || dim >= A.Dimensions.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                if (dim >= A.Size.NumberOfDimensions)
                    return A.C;
                // early exit: scalar/ empty
                if (A.IsEmpty || A.IsScalar)
                    return A.C;
                ILArray< byte> ret = A.C; // new ILArray<double>(new Storage.ILDenseStorage<double>(ILMemoryPool.Pool.New<double>(A.Dimensions.NumberOfElements), A.Dimensions));
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       
                        byte[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (int)(((long)dimLen * c * inc) % (A.Size.NumberOfElements - 1));
                                ILQuickSort.QuickSortDescST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                       
					    byte[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscMT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                        
                        byte[] retArr = ret.GetArrayForWrite();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscST(retArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                       
                        byte[] retArr = ret.GetArrayForWrite();
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscST(retArr, posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                //if (descending) {
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortDescSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //} else {
                //    // ascending
                //    for (int c = 0; c < maxRuns; c++) {
                //        if (posInArr >= A.Dimensions.NumberOfElements)
                //            posInArr -= (A.Dimensions.NumberOfElements - 1);
                //        ILQuickSort.QuickSortAscSolid_IT(ret.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc, false);
                //        posInArr += dimLen * inc;
                //    }
                //}
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array, n-dimensional</param>
        /// <param name="descending">Direction of sorting</param>
        /// <param name="dim">Dimension index to sort along</param>
        /// <param name="Indices">[Output] Returns permutation matrix</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks>The data in A will be sorted along the dimension <paramref name="dim"/> using a non-recursive quicksort 
        /// algorithm.  
        /// <para><b>Handling of NaN values (for double,float,complex or fcomplex arrays element datatypes only):</b> 
        /// if the input array contains any NaN values, those elements will be sorted to the end of the array on output.</para>
        /// <para>Properties of sorting can be tuned by the settings of <see cref="ILNumerics.Settings.s_maxSafeQuicksortRecursionDepth"/> 
        /// and <see cref="ILNumerics.Settings.s_minimumQuicksortLength"/>.</para>
        /// <para>This overload also returns an array 'Indices' which will hold the indices into the original 
        /// elements <b>after sorting</b>. Elements of 'Indices' are of type double.</para>
        /// <example><code>ILArray&lt;double&gt; A = ILMath.rand(1,5); 
        /// //A: 
        /// // 0.4324  0.9231  0.1231  0.1423  0.2991
        /// ILArray&lt;double&gt; I; 
        /// ILArray&lt;double&gt; R = ILMath.sort(A, out I, 1, false);
        /// //R: 
        /// // 0.1231  0.1423  0.2991  0.4324  0.9231  
        /// //I: 
        /// // 2  3  4  0  1
        /// </code>
        /// </example>
        /// <para>The quicksort algorithm is an unstable algorithm. Therefore, if in the input array some elements have 
        /// identical values, their relative order in the output vector is not garanteed to be unchanged.</para>
        /// </remarks>
        public static ILRetArray< byte> sort( ILInArray< byte> A, ILOutArray<int> Indices = null, int dim = -1, bool descending = false ) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null))
                    throw new Exception("parameter A must not be null");
                if (object.Equals(Indices, null)) {
                    return sort(A,dim,descending); 
                }
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                // early exit: scalar/ empty
                if (A.IsEmpty) {
                    Indices.a = empty<int>(ILSize.Empty00);
                    return empty< byte>(ILSize.Empty00);
                }
                if (A.IsScalar) {
                    Indices.a = 0;
                    return A.C;
                }
                if (dim < 0) {
                    dim = A.S.WorkingDimension(); 
                }
                int[] tmpDims = A.Size.ToIntArray();
                for (int i = 0; i < A.Size.NumberOfDimensions; i++) {
                    if (i == dim)
                        tmpDims[i] = A.Size[dim];
                    else
                        tmpDims[i] = 1;
                }
                //if (Indices.IsEmpty || !Indices.D.IsSameSize(A.D)) {
                Indices.a = reshape(counter<int>(0, 1, size(1, A.Size[dim])), tmpDims);
                tmpDims = A.Size.ToIntArray();
                tmpDims[dim] = 1;
                Indices.a = repmat(Indices, tmpDims);
                //}
                ILArray<byte> ret = A.C;
                int inc = ret.Size.SequentialIndexDistance(dim);
                int dimLen = A.Size[dim];
                int maxRuns = A.Size.NumberOfElements / dimLen;
                int posInArr = 0;
                if (descending) {
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortDescIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       byte [] retArr = ret.GetArrayForWrite(); 
                        int[] indArr = Indices.GetArrayForRead(); 
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements-1);
                                ILQuickSort.QuickSortDescIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                        }); 
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortDescIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                } else {
                    // ascending
                    if (maxRuns == 1) {
                        ILQuickSort.QuickSortAscIDXMT(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                    } else if (maxRuns >= 2 && dimLen >= ILNumerics.Settings.s_minParallelElement2Count) {
                       
                        byte[] retArr = ret.GetArrayForWrite();
                        int[] indArr = Indices.GetArrayForRead();
                        System.Threading.Tasks.Parallel.For(0, maxRuns,
                            (c) => {
                                int pos = (dimLen * c * inc) % (A.Size.NumberOfElements - 1);
                                ILQuickSort.QuickSortAscIDXST(retArr, indArr, pos, pos + (dimLen - 1) * inc, inc);
                            });
                    } else {
                        for (int c = 0; c < maxRuns; c++) {
                            if (posInArr >= A.Size.NumberOfElements)
                                posInArr -= (A.Size.NumberOfElements - 1);
                            ILQuickSort.QuickSortAscIDXST(ret.GetArrayForWrite(), Indices.GetArrayForWrite(), posInArr, posInArr + (dimLen - 1) * inc, inc);
                            posInArr += dimLen * inc;
                        }
                    }
                }
                return ret;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

#region bucket sort - string
        /// <summary>
        /// Sort strings in A along first non singleton dimension ascending
        /// </summary>
        /// <param name="A">Input array. A may be an empty, scalar, vector or matrix.</param>
        /// <returns>Sorted array of the same size/shape as A</returns>
        /// <remarks><para>The strings in A will be sorted lexicographically in ascending order using the bucket sort algorithm. Data 
        /// along the first non singleton dimension will get sorted independently from data 
        /// in the other rows/columns.</para>
        /// <para>The sorting order of strings is determined char-wise by comparing the ASCII codes of the characters.</para></remarks>
        public static ILRetArray<string> sort (ILInArray<string> A) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, false);
            }
        }
        /// <summary>
        /// Sort strings in A along first non singleton dimension
        /// </summary>
        /// <param name="A">Input array. A may be an empty, scalar, vector or matrix.</param>
        /// <param name="descending">Specifies the direction of sorting: true: descending sort direction; false: ascending</param>
        /// <returns>Sorted array of the same size/shape as A</returns>
        /// <remarks><para>The strings in A will be sorted lexicographically in ascending order using the bucket sort algorithm. Data 
        /// along the first non singleton dimension will get sorted independently from data 
        /// in the other rows/columns.</para>
        /// <para>The sorting order of strings is determined char-wise by comparing the ASCII codes of the characters.</para></remarks>
        public static ILRetArray<string> sort (ILInArray<string> A, bool descending) {
            using (ILScope.Enter(A)) {
                int fnsd = A.Size.WorkingDimension();
                if (fnsd < 0) return A.C;
                return sort(A, fnsd, descending);
            }
        }
        /// <summary>
        /// Sort strings in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array. A may be an empty, scalar, vector or matrix.</param>
        /// <param name="dim">Dimension to sort along</param>
        /// <param name="descending">Specifies the direction of sorting: true: descending sort direction; false: ascending</param>
        /// <returns>Sorted array of the same size/shape as A</returns>
        /// <remarks><para>The strings in A will be sorted lexicographically in ascending order using the bucket sort algorithm. Data 
        /// along the first non singleton dimension will get sorted independently from data 
        /// in the other rows/columns.</para>
        /// <para>The sorting order of strings is determined char-wise by comparing the ASCII codes of the characters.</para></remarks>
        public static ILRetArray<string> sort (ILInArray<string> A, int dim, bool descending) {
            if (object.Equals (A,null)) 
                throw new Exception("parameter A must not be null");
            using (ILScope.Enter(A)) {
                if (A.Size.NumberOfDimensions > 2)
                    throw new ILArgumentException("for element type string only matrices are supported!");
                if (dim < 0 || dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                // early exit: scalar/ empty
                if (A.IsEmpty || A.IsScalar)
                    return A.C;
                ILArray<string> ret =zeros<string>(A.Size);
                int dim1 = dim % A.Size.NumberOfDimensions;
                int dim2 = (dim1 + 1) % A.Size.NumberOfDimensions;
                int maxRuns = A.Size[dim2];
                ILQueueList<string, byte> ql;
                ILArray<int>[] ind = new ILArray<int>[2];
                int[] indI = new int[2];
                ILASCIIKeyMapper km = new ILASCIIKeyMapper();
                for (int c = 0; c < maxRuns; c++) {
                    ind[dim2] = c;
                    ind[dim1] = null;
                    ql = ILBucketSort.BucketSort<string, char, byte>(A[ind], null, km, ILBucketSort.SortMethod.ConstantLength);
                    indI[dim2] = c;
                    if (descending) {
                        for (int i = ql.Count; i-- > 0; ) {
                            indI[dim1] = i;
                            ret.SetValue(ql.Dequeue().Data, indI);
                        }
                    } else {
                        for (int i = 0; ql.Count > 0; i++) {
                            indI[dim1] = i;
                            ret.SetValue(ql.Dequeue().Data, indI);
                        }
                    }
                }
                return ret;
            }
        }
        /// <summary>
        /// Sort data in A along dimension 'dim'
        /// </summary>
        /// <param name="A">Input array: empty, scalar, vector or matrix</param>
        /// <param name="descending">Specifies the direction of sorting</param>
        /// <param name="dim">Dimension to sort along</param>
        /// <param name="Indices">[Output] Returns permutation matrix also</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks><para>The data in A will be sorted using the quick sort algorithm. Data 
        /// along the dimension <paramref name="dim"/> will get sorted independently from data 
        /// in the next row/column.</para>
        /// <para>This overload also returns an array 'Indices' which will hold the indices into the original 
        /// elements <b>after sorting</b>. Elements of 'Indices' are of type double.</para>
        ///</remarks>
        public static ILRetArray<string> sort (ILInArray<string> A, ILOutArray<double> Indices, int dim, bool descending) {
            if (object.Equals (A,null)) 
                throw new Exception("parameter A must not be null");
            using (ILScope.Enter(A)) {
                if (A.Size.NumberOfDimensions > 2)
                    throw new ILArgumentException("for element type string only matrices are supported!");
                if (dim < 0 || dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                // early exit: scalar/ empty
                if (A.IsEmpty || A.IsScalar) {
                    Indices.a = 0.0;
                    return A.C;
                }
                ILArray<string> ret = zeros<string>(A.Size);
                int dim1 = dim % A.Size.NumberOfDimensions;
                int dim2 = (dim1 + 1) % A.Size.NumberOfDimensions;
                int maxRuns = A.Size[dim2];
                ILQueueList<string, double> ql;
                ILBaseArray[] ind = new ILBaseArray[2];
                int[] indI = new int[2];
                ILASCIIKeyMapper km = new ILASCIIKeyMapper();
                Indices.a = ILMath.counter<double>(0.0, 1.0, A.Size);
                for (int c = 0; c < maxRuns; c++) {
                    ind[dim2] = (ILArray<int>)c;
                    ind[dim1] = ILMath.full;
                    ql = ILBucketSort.BucketSort<string, char, double>(A[ind], Indices[ind], km, ILBucketSort.SortMethod.ConstantLength);
                    indI[dim2] = c;
                    if (descending) {
                        for (int i = ql.Count; i-- > 0; ) {
                            indI[dim1] = i;
                            ILListItem<string, double> item = ql.Dequeue();
                            ret.SetValue(item.Data, indI);
                            Indices.SetValue(item.m_index, indI);
                        }
                    } else {
                        for (int i = 0; ql.Count > 0; i++) {
                            indI[dim1] = i;
                            ILListItem<string, double> item = ql.Dequeue();
                            ret.SetValue(item.Data, indI);
                            Indices.SetValue(item.m_index, indI);
                        }
                    }
                }
                return ret;
            }
        }
        /// <summary>
        /// Generic bucket sort algorithm in A along arbitrary dimension 
        /// </summary>
        /// <param name="A">Input array: empty, scalar, vector or matrix</param>
        /// <param name="descending">Specifies the direction of sorting</param>
        /// <param name="dim">Dimension to sort along</param>
        /// <param name="Indices">[Input/Output] The values in Indices will be returned in the same sorted order as the elements 
        /// in A. This can be used to derive a permutation matrix of the sorted indices.</param>
        /// <typeparam name="T">Element type of values of A</typeparam>
        /// <typeparam name="S">Subelement type. For element type of string this would be 'char'</typeparam>
        /// <typeparam name="I">Element type of indices</typeparam>
        /// <param name="keymapper">Instancce of an object of type ILKeyMapper. This object must 
        /// be derived from ILKeyMapper{T,SubelementType} and match the generic argument <typeparamref name="T"/>. It will be 
        /// used to split single elements into its subelements and map their content into bucket numbers. For all 
        /// reference types except those of type string you will have to write your own ILKeyMapper class for that purpose.</param>
        /// <returns>Sorted array of the same size as A</returns>
        /// <remarks><para>The data in A will be sorted using the bucket sort algorithm. Data 
        /// along the dimension <paramref name="dim"/> will get sorted independently. I.e., for dim = 0, columns are sorted independently.</para>
        /// <para>This overload also returns an array 'Indices' which will hold the indices into the original 
        /// elements <b>after sorting</b>. Therefore, the unsorted indices must be provided by the user on entry. Indices must not be null.</para>
        /// <para>This generic version is able to sort arbitrary element types. Even user defined reference types can be sorted 
        /// by specifying a user defined ILKeyMapper class instance. Also the type of Indices may be arbitrarily choosen. In difference 
        /// to the regular sort function overload, Indices must manually be given to the function on entry. Elements in 'Indices'
        /// are sorted in the same order as the elements of A.</para>
        /// <para>By using this overload you may use the same permutation matrix several times to reflect the 
        /// manipulations done to A due multiple sort processes. The Indices given will directly be used for the sorting 
        /// disregarding initial order.</para>
        /// </remarks>
        public static ILRetArray<T> sort<T, S, I>( ILInArray<T> A, ILOutArray<I> Indices, int dim, bool descending, ILKeyMapper<T, S> keymapper ) {
            if (object.Equals(A, null))
                throw new Exception("parameter A must not be null");
            if (object.Equals(Indices, null))
                throw new Exception("parameter 'Indices' must not be null");
            using (ILScope.Enter(A)) {
                if (!A.IsMatrix)
                    throw new ILArgumentException("for generic element type - only matrices are supported");
                if (dim < 0 || dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("invalid dimension argument");
                if (keymapper == null)
                    throw new ILArgumentException("the keymapper argument must not be null");
                if (object.Equals(Indices, null) || !Indices.Size.IsSameSize(A.Size))
                    throw new ILArgumentException("indices argument must have same size/shape as input A");
                // early exit: scalar/ empty
                if (A.IsEmpty || A.IsScalar) {
                    Indices.a = default(I);
                    return A.C;
                }
                ILArray<T> ret = zeros<T>(A.Size);
                int dim1 = dim % A.Size.NumberOfDimensions;
                int dim2 = (dim1 + 1) % A.Size.NumberOfDimensions;
                int maxRuns = A.Size[dim2];
                ILQueueList<T, I> ql;
                ILBaseArray[] ind = new ILBaseArray[2];
                int[] indI = new int[2];
                for (int c = 0; c < maxRuns; c++) {
                    ind[dim2] = (ILArray<int>)c;
                    ind[dim1] = full;
                    ql = ILBucketSort.BucketSort<T, S, I>(A[ind], Indices[ind], keymapper, ILBucketSort.SortMethod.ConstantLength);
                    indI[dim2] = c;
                    if (descending) {
                        for (int i = ql.Count; i-- > 0; ) {
                            indI[dim1] = i;
                            ILListItem<T, I> item = ql.Dequeue();
                            ret.SetValue(item.Data, indI);
                            Indices.SetValue(item.m_index, indI);
                        }
                    } else {
                        for (int i = 0; ql.Count > 0; i++) {
                            indI[dim1] = i;
                            ILListItem<T, I> item = ql.Dequeue();
                            ret.SetValue(item.Data, indI);
                            Indices.SetValue(item.m_index, indI);
                        }
                    }
                }
                return ret;
            }
        }
#endregion 


        internal static bool IsSorted (double[] vec,int length, bool descending) {
            if (descending) {
                for (int i = 1; i < length; i++) {
                    if (vec[i] > vec[i-1])
                        return false; 
                }
            } else {
                for (int i = 1; i < length; i++) {
                    if (vec[i] < vec[i-1])
                        return false; 
                }
            }
            return true;
        }

    }
}
