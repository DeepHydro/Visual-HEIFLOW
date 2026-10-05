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
using System.Runtime.InteropServices; 
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Native;
using ILNumerics.Exceptions;



namespace ILNumerics {

	public partial class ILMath {




#region HYCALPER AUTO GENERATED CODE

        /// <summary>Elementwise logical 'lower or equal' operator</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Logical array having '1' for elements in A being lower or equal corresponding elements in B, '0' else</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the other array.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILDimensionMismatchException">If neither A nor B is scalar or empty, the dimensions of both arrays must match.</exception>
        public unsafe static ILRetLogical  le(ILInArray<Int64> A, ILInArray<Int64> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                byte[] retArr;
               
                Int64[] arrA = A.GetArrayForRead();
               
                Int64[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                if (A.IsScalar) {
                    if (B.IsScalar) {
                       
                        return new ILRetLogical(new byte[1] { (A.GetValue(0)  <= B.GetValue(0)) ? (byte)1 : (byte)0 });
                    } else if (B.IsEmpty) {
                        return new ILRetLogical(B.Size);
                    } else {
                        outLen = B.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.SAN;
                    }
                    outDims = B.Size;
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return new ILRetLogical(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.ASN;
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  leEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.AAN;
                    }
                }
                int workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    byte* cLast, cp = (byte*)range.Item5 + range.Item1;
                    Int64 scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAN:
                            Int64* ap = ((Int64*)range.Item3 + range.Item1);
                            Int64* bp = ((Int64*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++; 
                                bp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((Int64*)range.Item3 + range.Item1);
                            scalar = *((Int64*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= scalar) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++;

                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((Int64*)range.Item3);
                            bp = ((Int64*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (scalar  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                bp++; 
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                #region do the work
                fixed (Int64* arrAP = arrA)
                fixed (Int64* arrBP = arrB)
                fixed (byte* retArrP = retArr) {
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                    if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                        if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                            workItemLength = outLen / workItemCount;
                        } else {
                            workItemLength = outLen / 2;
                            workItemCount = 2;
                        }
                    } else {
                        workItemLength = outLen;
                        workItemCount = 1;
                    }

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                }

                #endregion
                return new ILRetLogical(retArr, outDims);
            }
        }

        private static unsafe ILRetLogical  leEx(ILInArray<Int64> A, ILInArray<Int64> B) {
            using (ILScope.Enter(A, B)) {
                #region parameter checking
                if (isnull(A) || isnull(B))
                    return new ILRetLogical(ILSize.Empty00);
                if (A.IsEmpty) {
                    return new ILRetLogical(B.S);
                } else if (B.IsEmpty) {
                    return new ILRetLogical(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation
                byte[] retArr;
               
                Int64[] arrA = A.GetArrayForRead();
               
                Int64[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILLogicalStorage retStorage = new ILLogicalStorage(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
                   
                    Int64* ap;
                   
                    Int64* bp;
                    byte* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (Int64*)range.Item3;
                                bp = (Int64*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (Int64*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (Int64*)range.Item4;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed ( Int64* arrAP = arrA)
                fixed ( Int64* arrBP = arrB)
                fixed (byte* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetLogical(retStorage);
            }
        }

        /// <summary>Elementwise logical 'lower or equal' operator</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Logical array having '1' for elements in A being lower or equal corresponding elements in B, '0' else</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the other array.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILDimensionMismatchException">If neither A nor B is scalar or empty, the dimensions of both arrays must match.</exception>
        public unsafe static ILRetLogical  le(ILInArray<Int32> A, ILInArray<Int32> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                byte[] retArr;
               
                Int32[] arrA = A.GetArrayForRead();
               
                Int32[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                if (A.IsScalar) {
                    if (B.IsScalar) {
                       
                        return new ILRetLogical(new byte[1] { (A.GetValue(0)  <= B.GetValue(0)) ? (byte)1 : (byte)0 });
                    } else if (B.IsEmpty) {
                        return new ILRetLogical(B.Size);
                    } else {
                        outLen = B.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.SAN;
                    }
                    outDims = B.Size;
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return new ILRetLogical(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.ASN;
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  leEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.AAN;
                    }
                }
                int workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    byte* cLast, cp = (byte*)range.Item5 + range.Item1;
                    Int32 scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAN:
                            Int32* ap = ((Int32*)range.Item3 + range.Item1);
                            Int32* bp = ((Int32*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++; 
                                bp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((Int32*)range.Item3 + range.Item1);
                            scalar = *((Int32*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= scalar) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++;

                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((Int32*)range.Item3);
                            bp = ((Int32*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (scalar  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                bp++; 
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                #region do the work
                fixed (Int32* arrAP = arrA)
                fixed (Int32* arrBP = arrB)
                fixed (byte* retArrP = retArr) {
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                    if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                        if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                            workItemLength = outLen / workItemCount;
                        } else {
                            workItemLength = outLen / 2;
                            workItemCount = 2;
                        }
                    } else {
                        workItemLength = outLen;
                        workItemCount = 1;
                    }

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                }

                #endregion
                return new ILRetLogical(retArr, outDims);
            }
        }

        private static unsafe ILRetLogical  leEx(ILInArray<Int32> A, ILInArray<Int32> B) {
            using (ILScope.Enter(A, B)) {
                #region parameter checking
                if (isnull(A) || isnull(B))
                    return new ILRetLogical(ILSize.Empty00);
                if (A.IsEmpty) {
                    return new ILRetLogical(B.S);
                } else if (B.IsEmpty) {
                    return new ILRetLogical(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation
                byte[] retArr;
               
                Int32[] arrA = A.GetArrayForRead();
               
                Int32[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILLogicalStorage retStorage = new ILLogicalStorage(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
                   
                    Int32* ap;
                   
                    Int32* bp;
                    byte* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (Int32*)range.Item3;
                                bp = (Int32*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (Int32*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (Int32*)range.Item4;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed ( Int32* arrAP = arrA)
                fixed ( Int32* arrBP = arrB)
                fixed (byte* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetLogical(retStorage);
            }
        }

        /// <summary>Elementwise logical 'lower or equal' operator</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Logical array having '1' for elements in A being lower or equal corresponding elements in B, '0' else</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the other array.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILDimensionMismatchException">If neither A nor B is scalar or empty, the dimensions of both arrays must match.</exception>
        public unsafe static ILRetLogical  le(ILInArray<float> A, ILInArray<float> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                byte[] retArr;
               
                float[] arrA = A.GetArrayForRead();
               
                float[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                if (A.IsScalar) {
                    if (B.IsScalar) {
                       
                        return new ILRetLogical(new byte[1] { (A.GetValue(0)  <= B.GetValue(0)) ? (byte)1 : (byte)0 });
                    } else if (B.IsEmpty) {
                        return new ILRetLogical(B.Size);
                    } else {
                        outLen = B.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.SAN;
                    }
                    outDims = B.Size;
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return new ILRetLogical(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.ASN;
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  leEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.AAN;
                    }
                }
                int workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    byte* cLast, cp = (byte*)range.Item5 + range.Item1;
                    float scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAN:
                            float* ap = ((float*)range.Item3 + range.Item1);
                            float* bp = ((float*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++; 
                                bp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((float*)range.Item3 + range.Item1);
                            scalar = *((float*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= scalar) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++;

                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((float*)range.Item3);
                            bp = ((float*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (scalar  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                bp++; 
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                #region do the work
                fixed (float* arrAP = arrA)
                fixed (float* arrBP = arrB)
                fixed (byte* retArrP = retArr) {
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                    if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                        if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                            workItemLength = outLen / workItemCount;
                        } else {
                            workItemLength = outLen / 2;
                            workItemCount = 2;
                        }
                    } else {
                        workItemLength = outLen;
                        workItemCount = 1;
                    }

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                }

                #endregion
                return new ILRetLogical(retArr, outDims);
            }
        }

        private static unsafe ILRetLogical  leEx(ILInArray<float> A, ILInArray<float> B) {
            using (ILScope.Enter(A, B)) {
                #region parameter checking
                if (isnull(A) || isnull(B))
                    return new ILRetLogical(ILSize.Empty00);
                if (A.IsEmpty) {
                    return new ILRetLogical(B.S);
                } else if (B.IsEmpty) {
                    return new ILRetLogical(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation
                byte[] retArr;
               
                float[] arrA = A.GetArrayForRead();
               
                float[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILLogicalStorage retStorage = new ILLogicalStorage(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
                   
                    float* ap;
                   
                    float* bp;
                    byte* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (float*)range.Item3;
                                bp = (float*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (float*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (float*)range.Item4;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed ( float* arrAP = arrA)
                fixed ( float* arrBP = arrB)
                fixed (byte* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetLogical(retStorage);
            }
        }

        /// <summary>Elementwise logical 'lower or equal' operator</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Logical array having '1' for elements in A being lower or equal corresponding elements in B, '0' else</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the other array.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILDimensionMismatchException">If neither A nor B is scalar or empty, the dimensions of both arrays must match.</exception>
        public unsafe static ILRetLogical  le(ILInArray<fcomplex> A, ILInArray<fcomplex> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                byte[] retArr;
               
                fcomplex[] arrA = A.GetArrayForRead();
               
                fcomplex[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                if (A.IsScalar) {
                    if (B.IsScalar) {
                       
                        return new ILRetLogical(new byte[1] { (A.GetValue(0)  <= B.GetValue(0)) ? (byte)1 : (byte)0 });
                    } else if (B.IsEmpty) {
                        return new ILRetLogical(B.Size);
                    } else {
                        outLen = B.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.SAN;
                    }
                    outDims = B.Size;
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return new ILRetLogical(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.ASN;
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  leEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.AAN;
                    }
                }
                int workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    byte* cLast, cp = (byte*)range.Item5 + range.Item1;
                    fcomplex scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAN:
                            fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                            fcomplex* bp = ((fcomplex*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++; 
                                bp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((fcomplex*)range.Item3 + range.Item1);
                            scalar = *((fcomplex*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= scalar) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++;

                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((fcomplex*)range.Item3);
                            bp = ((fcomplex*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (scalar  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                bp++; 
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                #region do the work
                fixed (fcomplex* arrAP = arrA)
                fixed (fcomplex* arrBP = arrB)
                fixed (byte* retArrP = retArr) {
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                    if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                        if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                            workItemLength = outLen / workItemCount;
                        } else {
                            workItemLength = outLen / 2;
                            workItemCount = 2;
                        }
                    } else {
                        workItemLength = outLen;
                        workItemCount = 1;
                    }

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                }

                #endregion
                return new ILRetLogical(retArr, outDims);
            }
        }

        private static unsafe ILRetLogical  leEx(ILInArray<fcomplex> A, ILInArray<fcomplex> B) {
            using (ILScope.Enter(A, B)) {
                #region parameter checking
                if (isnull(A) || isnull(B))
                    return new ILRetLogical(ILSize.Empty00);
                if (A.IsEmpty) {
                    return new ILRetLogical(B.S);
                } else if (B.IsEmpty) {
                    return new ILRetLogical(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation
                byte[] retArr;
               
                fcomplex[] arrA = A.GetArrayForRead();
               
                fcomplex[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILLogicalStorage retStorage = new ILLogicalStorage(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
                   
                    fcomplex* ap;
                   
                    fcomplex* bp;
                    byte* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (fcomplex*)range.Item3;
                                bp = (fcomplex*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (fcomplex*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (fcomplex*)range.Item4;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed ( fcomplex* arrAP = arrA)
                fixed ( fcomplex* arrBP = arrB)
                fixed (byte* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetLogical(retStorage);
            }
        }

        /// <summary>Elementwise logical 'lower or equal' operator</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Logical array having '1' for elements in A being lower or equal corresponding elements in B, '0' else</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the other array.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILDimensionMismatchException">If neither A nor B is scalar or empty, the dimensions of both arrays must match.</exception>
        public unsafe static ILRetLogical  le(ILInArray<complex> A, ILInArray<complex> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                byte[] retArr;
               
                complex[] arrA = A.GetArrayForRead();
               
                complex[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                if (A.IsScalar) {
                    if (B.IsScalar) {
                       
                        return new ILRetLogical(new byte[1] { (A.GetValue(0)  <= B.GetValue(0)) ? (byte)1 : (byte)0 });
                    } else if (B.IsEmpty) {
                        return new ILRetLogical(B.Size);
                    } else {
                        outLen = B.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.SAN;
                    }
                    outDims = B.Size;
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return new ILRetLogical(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.ASN;
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  leEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.AAN;
                    }
                }
                int workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    byte* cLast, cp = (byte*)range.Item5 + range.Item1;
                    complex scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAN:
                            complex* ap = ((complex*)range.Item3 + range.Item1);
                            complex* bp = ((complex*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++; 
                                bp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((complex*)range.Item3 + range.Item1);
                            scalar = *((complex*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= scalar) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++;

                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((complex*)range.Item3);
                            bp = ((complex*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (scalar  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                bp++; 
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                #region do the work
                fixed (complex* arrAP = arrA)
                fixed (complex* arrBP = arrB)
                fixed (byte* retArrP = retArr) {
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                    if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                        if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                            workItemLength = outLen / workItemCount;
                        } else {
                            workItemLength = outLen / 2;
                            workItemCount = 2;
                        }
                    } else {
                        workItemLength = outLen;
                        workItemCount = 1;
                    }

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                }

                #endregion
                return new ILRetLogical(retArr, outDims);
            }
        }

        private static unsafe ILRetLogical  leEx(ILInArray<complex> A, ILInArray<complex> B) {
            using (ILScope.Enter(A, B)) {
                #region parameter checking
                if (isnull(A) || isnull(B))
                    return new ILRetLogical(ILSize.Empty00);
                if (A.IsEmpty) {
                    return new ILRetLogical(B.S);
                } else if (B.IsEmpty) {
                    return new ILRetLogical(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation
                byte[] retArr;
               
                complex[] arrA = A.GetArrayForRead();
               
                complex[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILLogicalStorage retStorage = new ILLogicalStorage(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
                   
                    complex* ap;
                   
                    complex* bp;
                    byte* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (complex*)range.Item3;
                                bp = (complex*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (complex*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (complex*)range.Item4;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed ( complex* arrAP = arrA)
                fixed ( complex* arrBP = arrB)
                fixed (byte* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetLogical(retStorage);
            }
        }

        /// <summary>Elementwise logical 'lower or equal' operator</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Logical array having '1' for elements in A being lower or equal corresponding elements in B, '0' else</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the other array.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILDimensionMismatchException">If neither A nor B is scalar or empty, the dimensions of both arrays must match.</exception>
        public unsafe static ILRetLogical  le(ILInArray<byte> A, ILInArray<byte> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                byte[] retArr;
               
                byte[] arrA = A.GetArrayForRead();
               
                byte[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                if (A.IsScalar) {
                    if (B.IsScalar) {
                       
                        return new ILRetLogical(new byte[1] { (A.GetValue(0)  <= B.GetValue(0)) ? (byte)1 : (byte)0 });
                    } else if (B.IsEmpty) {
                        return new ILRetLogical(B.Size);
                    } else {
                        outLen = B.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.SAN;
                    }
                    outDims = B.Size;
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return new ILRetLogical(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.ASN;
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  leEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.AAN;
                    }
                }
                int workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    byte* cLast, cp = (byte*)range.Item5 + range.Item1;
                    byte scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAN:
                            byte* ap = ((byte*)range.Item3 + range.Item1);
                            byte* bp = ((byte*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++; 
                                bp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((byte*)range.Item3 + range.Item1);
                            scalar = *((byte*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= scalar) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++;

                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((byte*)range.Item3);
                            bp = ((byte*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (scalar  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                bp++; 
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                #region do the work
                fixed (byte* arrAP = arrA)
                fixed (byte* arrBP = arrB)
                fixed (byte* retArrP = retArr) {
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                    if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                        if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                            workItemLength = outLen / workItemCount;
                        } else {
                            workItemLength = outLen / 2;
                            workItemCount = 2;
                        }
                    } else {
                        workItemLength = outLen;
                        workItemCount = 1;
                    }

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                }

                #endregion
                return new ILRetLogical(retArr, outDims);
            }
        }

        private static unsafe ILRetLogical  leEx(ILInArray<byte> A, ILInArray<byte> B) {
            using (ILScope.Enter(A, B)) {
                #region parameter checking
                if (isnull(A) || isnull(B))
                    return new ILRetLogical(ILSize.Empty00);
                if (A.IsEmpty) {
                    return new ILRetLogical(B.S);
                } else if (B.IsEmpty) {
                    return new ILRetLogical(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation
                byte[] retArr;
               
                byte[] arrA = A.GetArrayForRead();
               
                byte[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILLogicalStorage retStorage = new ILLogicalStorage(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
                   
                    byte* ap;
                   
                    byte* bp;
                    byte* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (byte*)range.Item3;
                                bp = (byte*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (byte*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (byte*)range.Item4;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed ( byte* arrAP = arrA)
                fixed ( byte* arrBP = arrB)
                fixed (byte* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetLogical(retStorage);
            }
        }

        /// <summary>Elementwise logical 'lower or equal' operator</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Logical array having '1' for elements in A being lower or equal corresponding elements in B, '0' else</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the other array.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILDimensionMismatchException">If neither A nor B is scalar or empty, the dimensions of both arrays must match.</exception>
        public unsafe static ILRetLogical  le(ILInArray<double> A, ILInArray<double> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                byte[] retArr;
               
                double[] arrA = A.GetArrayForRead();
               
                double[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                if (A.IsScalar) {
                    if (B.IsScalar) {
                       
                        return new ILRetLogical(new byte[1] { (A.GetValue(0)  <= B.GetValue(0)) ? (byte)1 : (byte)0 });
                    } else if (B.IsEmpty) {
                        return new ILRetLogical(B.Size);
                    } else {
                        outLen = B.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.SAN;
                    }
                    outDims = B.Size;
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return new ILRetLogical(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.ASN;
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  leEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        retArr = ILMemoryPool.Pool.New<byte>(outLen);
                        mode = BinOpItMode.AAN;
                    }
                }
                int workerCount = 1;
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    byte* cLast, cp = (byte*)range.Item5 + range.Item1;
                    double scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAN:
                            double* ap = ((double*)range.Item3 + range.Item1);
                            double* bp = ((double*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++; 
                                bp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((double*)range.Item3 + range.Item1);
                            scalar = *((double*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp = (*ap  <= scalar) ? (byte)1 : (byte)0; 
                                cp++; 
                                ap++;

                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((double*)range.Item3);
                            bp = ((double*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp = (scalar  <= *bp) ? (byte)1 : (byte)0; 
                                cp++; 
                                bp++; 
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };

                #region do the work
                fixed (double* arrAP = arrA)
                fixed (double* arrBP = arrB)
                fixed (byte* retArrP = retArr) {
                    int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                    if (Settings.s_maxNumberThreads > 1 && outLen / 2 > Settings.s_minParallelElement1Count) {
                        if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                            workItemLength = outLen / workItemCount;
                        } else {
                            workItemLength = outLen / 2;
                            workItemCount = 2;
                        }
                    } else {
                        workItemLength = outLen;
                        workItemCount = 1;
                    }

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));
                    ILThreadPool.Wait4Workers(ref workerCount); 
                }

                #endregion
                return new ILRetLogical(retArr, outDims);
            }
        }

        private static unsafe ILRetLogical  leEx(ILInArray<double> A, ILInArray<double> B) {
            using (ILScope.Enter(A, B)) {
                #region parameter checking
                if (isnull(A) || isnull(B))
                    return new ILRetLogical(ILSize.Empty00);
                if (A.IsEmpty) {
                    return new ILRetLogical(B.S);
                } else if (B.IsEmpty) {
                    return new ILRetLogical(A.S);
                }
                //if (A.IsScalar || B.IsScalar || A.D.IsSameSize(B.D)) 
                //    return add(A,B);
                int dim = -1;
                for (int l = 0; l < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); l++) {
                    if (A.S[l] != B.S[l]) {
                        if (dim >= 0 || (A.S[l] != 1 && B.S[l] != 1)) {
                            throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                        }
                        dim = l;
                    }
                }
                if (dim > 1)
                    throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
                #endregion

                #region parameter preparation
                byte[] retArr;
               
                double[] arrA = A.GetArrayForRead();
               
                double[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int arrInc = 0;
                int arrStepInc = 0;
                int dimLen = 0;
                if (A.IsVector) {
                    outDims = B.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                    dimLen = A.Length;
                } else if (B.IsVector) {
                    outDims = A.S;
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                    dimLen = B.Length;
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                }
                arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                arrStepInc = outDims.SequentialIndexDistance(dim);
                #endregion

                #region worker loops definition
                ILLogicalStorage retStorage = new ILLogicalStorage(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
                   
                    double* ap;
                   
                    double* bp;
                    byte* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (double*)range.Item3;
                                bp = (double*)range.Item4 + range.Item1 + s * arrStepInc; ;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap++;
                                    bp += arrInc;
                                    cp += arrInc;
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            for (int s = 0; s < range.Item2; s++) {
                                ap = (double*)range.Item3 + range.Item1 + s * arrStepInc;
                                bp = (double*)range.Item4;
                                cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                                for (int l = 0; l < dimLen; l++) {
                                   
                                    *cp = (*ap  <= *bp) ? (byte)1 : (byte)0;
                                    ap += arrInc;
                                    bp++;
                                    cp += arrInc;
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                int outLen = outDims.NumberOfElements;
                if (Settings.s_maxNumberThreads > 1 && outLen / 2 >= Settings.s_minParallelElement1Count) {
                    if (outLen / workItemCount > Settings.s_minParallelElement1Count) {
                        workItemLength = outLen / dimLen / workItemCount;
                        //workItemLength = (int)((double)outLen / workItemCount * 1.05);
                    } else {
                        workItemLength = outLen / dimLen / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outLen / dimLen;
                    workItemCount = 1;
                }

                fixed ( double* arrAP = arrA)
                fixed ( double* arrBP = arrB)
                fixed (byte* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, int, IntPtr, IntPtr, IntPtr> range
                            = new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                               (i * workItemLength * arrStepInc, workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP);
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr>
                                (i * workItemLength * arrStepInc, (outLen / dimLen) - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetLogical(retStorage);
            }
        }


#endregion HYCALPER AUTO GENERATED CODE

    }
}
