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
using ILNumerics;
using ILNumerics.Misc;
using ILNumerics.Storage;
using ILNumerics.Native;
using ILNumerics.Exceptions;


namespace ILNumerics {

    public partial class ILMath {

        private static byte saturateBytePow( byte a, byte b ) {
            return saturateByte(Math.Pow(a, b));
        }
        private static int saturateInt32Pow( int a, int b ) {
            return saturateInt32(Math.Pow(a, b));
        }
        private static long saturateInt64Pow( long a, long b ) {
            return saturateInt64(Math.Pow(a, b));
        }
       

        

#region HYCALPER AUTO GENERATED CODE

        /// <summary>Power elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with elementwise result of A <sup>B</sup></returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<Int64>  pow(ILInArray<Int64> A, ILInArray<Int64> B) {
            using (ILScope.Enter(A,B)) {
                int outLen; 
                BinOpItMode mode; 
                Int64 [] retArr;
                Int64 [] arrA = A.GetArrayForRead();
                Int64[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size; 
                    if (B.IsScalar) {
                       
                        return new  ILRetArray<Int64> (new  Int64 [1]{ saturateInt64Pow (A.GetValue(0)  , B.GetValue(0))}, A.Size);
                    } else if (B.IsEmpty) {
                        return  ILRetArray<Int64>.empty(outDims); 
                    } else {
                        outLen = outDims.NumberOfElements; 
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< Int64 > (outLen);
                            mode = BinOpItMode.SAN; 
                        } else {
                            mode = BinOpItMode.SAI; 
                        }
                    }
                } else {
                    outDims = A.Size; 
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return  ILRetArray<Int64>.empty(A.Size);  
                        }
                        outLen = A.S.NumberOfElements; 
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< Int64 > (outLen);
                            mode = BinOpItMode.ASN; 
                        } else {
                            mode = BinOpItMode.ASI; 
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  powEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements; 
                        if (A.TryGetStorage4InplaceOp(out retArr)) 
                            mode  = BinOpItMode.AAIA; 
                        else if (B.TryGetStorage4InplaceOp(out retArr)) 
                            mode = BinOpItMode.AAIB; 
                        else {
                            retArr = ILMemoryPool.Pool.New< Int64 > (outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<Int64> retStorage = new ILDenseStorage<Int64>(retArr, outDims);
                int i = 0, workerCount = 1; 
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    Int64* cp = (Int64*)range.Item5 + range.Item1;
                    Int64 scalar;
                    int j = range.Item2; 
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           Int64* bp = ((Int64*)range.Item4 + range.Item1);
                            while (j > 20) {
                                cp[0] =  saturateInt64Pow (cp[0]  , bp[0]);
                                cp[1] =  saturateInt64Pow (cp[1]  , bp[1]);
                                cp[2] =  saturateInt64Pow (cp[2]  , bp[2]);
                                cp[3] =  saturateInt64Pow (cp[3]  , bp[3]);
                                cp[4] =  saturateInt64Pow (cp[4]  , bp[4]);
                                cp[5] =  saturateInt64Pow (cp[5]  , bp[5]);
                                cp[6] =  saturateInt64Pow (cp[6]  , bp[6]);
                                cp[7] =  saturateInt64Pow (cp[7]  , bp[7]);
                                cp[8] =  saturateInt64Pow (cp[8]  , bp[8]);
                                cp[9] =  saturateInt64Pow (cp[9]  , bp[9]);
                                cp[10] =  saturateInt64Pow (cp[10]  , bp[10]);
                                cp[11] =  saturateInt64Pow (cp[11]  , bp[11]);
                                cp[12] =  saturateInt64Pow (cp[12]  , bp[12]);
                                cp[13] =  saturateInt64Pow (cp[13]  , bp[13]);
                                cp[14] =  saturateInt64Pow (cp[14]  , bp[14]);
                                cp[15] =  saturateInt64Pow (cp[15]  , bp[15]);
                                cp[16] =  saturateInt64Pow (cp[16]  , bp[16]);
                                cp[17] =  saturateInt64Pow (cp[17]  , bp[17]);
                                cp[18] =  saturateInt64Pow (cp[18]  , bp[18]);
                                cp[19] =  saturateInt64Pow (cp[19]  , bp[19]);
                                cp[20] =  saturateInt64Pow (cp[20]  , bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64Pow (*cp  , *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           Int64* ap = ((Int64*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt64Pow (ap[0]  , cp[0]);
                                cp[1] =  saturateInt64Pow (ap[1]  , cp[1]);
                                cp[2] =  saturateInt64Pow (ap[2]  , cp[2]);
                                cp[3] =  saturateInt64Pow (ap[3]  , cp[3]);
                                cp[4] =  saturateInt64Pow (ap[4]  , cp[4]);
                                cp[5] =  saturateInt64Pow (ap[5]  , cp[5]);
                                cp[6] =  saturateInt64Pow (ap[6]  , cp[6]);
                                cp[7] =  saturateInt64Pow (ap[7]  , cp[7]);
                                cp[8] =  saturateInt64Pow (ap[8]  , cp[8]);
                                cp[9] =  saturateInt64Pow (ap[9]  , cp[9]);
                                cp[10] =  saturateInt64Pow (ap[10]  , cp[10]);
                                cp[11] =  saturateInt64Pow (ap[11]  , cp[11]);
                                cp[12] =  saturateInt64Pow (ap[12]  , cp[12]);
                                cp[13] =  saturateInt64Pow (ap[13]  , cp[13]);
                                cp[14] =  saturateInt64Pow (ap[14]  , cp[14]);
                                cp[15] =  saturateInt64Pow (ap[15]  , cp[15]);
                                cp[16] =  saturateInt64Pow (ap[16]  , cp[16]);
                                cp[17] =  saturateInt64Pow (ap[17]  , cp[17]);
                                cp[18] =  saturateInt64Pow (ap[18]  , cp[18]);
                                cp[19] =  saturateInt64Pow (ap[19]  , cp[19]);
                                cp[20] =  saturateInt64Pow (ap[20]  , cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64Pow (*ap  , *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((Int64*)range.Item3 + range.Item1);
                            bp = ((Int64*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt64Pow (ap[0]  , bp[0]);
                                cp[1] =  saturateInt64Pow (ap[1]  , bp[1]);
                                cp[2] =  saturateInt64Pow (ap[2]  , bp[2]);
                                cp[3] =  saturateInt64Pow (ap[3]  , bp[3]);
                                cp[4] =  saturateInt64Pow (ap[4]  , bp[4]);
                                cp[5] =  saturateInt64Pow (ap[5]  , bp[5]);
                                cp[6] =  saturateInt64Pow (ap[6]  , bp[6]);
                                cp[7] =  saturateInt64Pow (ap[7]  , bp[7]);
                                cp[8] =  saturateInt64Pow (ap[8]  , bp[8]);
                                cp[9] =  saturateInt64Pow (ap[9]  , bp[9]);
                                cp[10] =  saturateInt64Pow (ap[10]  , bp[10]);
                                cp[11] =  saturateInt64Pow (ap[11]  , bp[11]);
                                cp[12] =  saturateInt64Pow (ap[12]  , bp[12]);
                                cp[13] =  saturateInt64Pow (ap[13]  , bp[13]);
                                cp[14] =  saturateInt64Pow (ap[14]  , bp[14]);
                                cp[15] =  saturateInt64Pow (ap[15]  , bp[15]);
                                cp[16] =  saturateInt64Pow (ap[16]  , bp[16]);
                                cp[17] =  saturateInt64Pow (ap[17]  , bp[17]);
                                cp[18] =  saturateInt64Pow (ap[18]  , bp[18]);
                                cp[19] =  saturateInt64Pow (ap[19]  , bp[19]);
                                cp[20] =  saturateInt64Pow (ap[20]  , bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64Pow (*ap  , *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((Int64*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt64Pow (cp[0]  , scalar);
                                cp[1] =  saturateInt64Pow (cp[1]  , scalar);
                                cp[2] =  saturateInt64Pow (cp[2]  , scalar);
                                cp[3] =  saturateInt64Pow (cp[3]  , scalar);
                                cp[4] =  saturateInt64Pow (cp[4]  , scalar);
                                cp[5] =  saturateInt64Pow (cp[5]  , scalar);
                                cp[6] =  saturateInt64Pow (cp[6]  , scalar);
                                cp[7] =  saturateInt64Pow (cp[7]  , scalar);
                                cp[8] =  saturateInt64Pow (cp[8]  , scalar);
                                cp[9] =  saturateInt64Pow (cp[9]  , scalar);
                                cp[10] =  saturateInt64Pow (cp[10]  , scalar);
                                cp[11] =  saturateInt64Pow (cp[11]  , scalar);
                                cp[12] =  saturateInt64Pow (cp[12]  , scalar);
                                cp[13] =  saturateInt64Pow (cp[13]  , scalar);
                                cp[14] =  saturateInt64Pow (cp[14]  , scalar);
                                cp[15] =  saturateInt64Pow (cp[15]  , scalar);
                                cp[16] =  saturateInt64Pow (cp[16]  , scalar);
                                cp[17] =  saturateInt64Pow (cp[17]  , scalar);
                                cp[18] =  saturateInt64Pow (cp[18]  , scalar);
                                cp[19] =  saturateInt64Pow (cp[19]  , scalar);
                                cp[20] =  saturateInt64Pow (cp[20]  , scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64Pow (*cp  , scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((Int64*)range.Item3 + range.Item1);
                            scalar = *((Int64*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt64Pow (ap[0]  , scalar);
                                cp[1] =  saturateInt64Pow (ap[1]  , scalar);
                                cp[2] =  saturateInt64Pow (ap[2]  , scalar);
                                cp[3] =  saturateInt64Pow (ap[3]  , scalar);
                                cp[4] =  saturateInt64Pow (ap[4]  , scalar);
                                cp[5] =  saturateInt64Pow (ap[5]  , scalar);
                                cp[6] =  saturateInt64Pow (ap[6]  , scalar);
                                cp[7] =  saturateInt64Pow (ap[7]  , scalar);
                                cp[8] =  saturateInt64Pow (ap[8]  , scalar);
                                cp[9] =  saturateInt64Pow (ap[9]  , scalar);
                                cp[10] =  saturateInt64Pow (ap[10]  , scalar);
                                cp[11] =  saturateInt64Pow (ap[11]  , scalar);
                                cp[12] =  saturateInt64Pow (ap[12]  , scalar);
                                cp[13] =  saturateInt64Pow (ap[13]  , scalar);
                                cp[14] =  saturateInt64Pow (ap[14]  , scalar);
                                cp[15] =  saturateInt64Pow (ap[15]  , scalar);
                                cp[16] =  saturateInt64Pow (ap[16]  , scalar);
                                cp[17] =  saturateInt64Pow (ap[17]  , scalar);
                                cp[18] =  saturateInt64Pow (ap[18]  , scalar);
                                cp[19] =  saturateInt64Pow (ap[19]  , scalar);
                                cp[20] =  saturateInt64Pow (ap[20]  , scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64Pow (*ap  , scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((Int64*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt64Pow (scalar  , cp[0]);
                                cp[1] =  saturateInt64Pow (scalar  , cp[1]);
                                cp[2] =  saturateInt64Pow (scalar  , cp[2]);
                                cp[3] =  saturateInt64Pow (scalar  , cp[3]);
                                cp[4] =  saturateInt64Pow (scalar  , cp[4]);
                                cp[5] =  saturateInt64Pow (scalar  , cp[5]);
                                cp[6] =  saturateInt64Pow (scalar  , cp[6]);
                                cp[7] =  saturateInt64Pow (scalar  , cp[7]);
                                cp[8] =  saturateInt64Pow (scalar  , cp[8]);
                                cp[9] =  saturateInt64Pow (scalar  , cp[9]);
                                cp[10] =  saturateInt64Pow (scalar  , cp[10]);
                                cp[11] =  saturateInt64Pow (scalar  , cp[11]);
                                cp[12] =  saturateInt64Pow (scalar  , cp[12]);
                                cp[13] =  saturateInt64Pow (scalar  , cp[13]);
                                cp[14] =  saturateInt64Pow (scalar  , cp[14]);
                                cp[15] =  saturateInt64Pow (scalar  , cp[15]);
                                cp[16] =  saturateInt64Pow (scalar  , cp[16]);
                                cp[17] =  saturateInt64Pow (scalar  , cp[17]);
                                cp[18] =  saturateInt64Pow (scalar  , cp[18]);
                                cp[19] =  saturateInt64Pow (scalar  , cp[19]);
                                cp[20] =  saturateInt64Pow (scalar  , cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64Pow (scalar  , *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((Int64*)range.Item3);
                            bp = ((Int64*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt64Pow (scalar  , bp[0]);
                                cp[1] =  saturateInt64Pow (scalar  , bp[1]);
                                cp[2] =  saturateInt64Pow (scalar  , bp[2]);
                                cp[3] =  saturateInt64Pow (scalar  , bp[3]);
                                cp[4] =  saturateInt64Pow (scalar  , bp[4]);
                                cp[5] =  saturateInt64Pow (scalar  , bp[5]);
                                cp[6] =  saturateInt64Pow (scalar  , bp[6]);
                                cp[7] =  saturateInt64Pow (scalar  , bp[7]);
                                cp[8] =  saturateInt64Pow (scalar  , bp[8]);
                                cp[9] =  saturateInt64Pow (scalar  , bp[9]);
                                cp[10] =  saturateInt64Pow (scalar  , bp[10]);
                                cp[11] =  saturateInt64Pow (scalar  , bp[11]);
                                cp[12] =  saturateInt64Pow (scalar  , bp[12]);
                                cp[13] =  saturateInt64Pow (scalar  , bp[13]);
                                cp[14] =  saturateInt64Pow (scalar  , bp[14]);
                                cp[15] =  saturateInt64Pow (scalar  , bp[15]);
                                cp[16] =  saturateInt64Pow (scalar  , bp[16]);
                                cp[17] =  saturateInt64Pow (scalar  , bp[17]);
                                cp[18] =  saturateInt64Pow (scalar  , bp[18]);
                                cp[19] =  saturateInt64Pow (scalar  , bp[19]);
                                cp[20] =  saturateInt64Pow (scalar  , bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64Pow (scalar  , *bp);
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);

                };

                #region do the work 
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
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
                
                fixed ( Int64* arrAP = arrA)
                fixed ( Int64* arrBP = arrB)
                fixed ( Int64* retArrP = retArr) {

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
                return new  ILRetArray< Int64>(retStorage);
            }
        }

        private static unsafe ILRetArray<Int64>  powEx(ILInArray<Int64> A, ILInArray<Int64> B) {
            //using (ILScope.Enter(A, B)) { we cannot start a new scope here, since this would prevent A and B to be used implace if applicable

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<Int64>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<Int64>(B.S);
                } else if (B.IsEmpty) {
                    return empty<Int64>(A.S);
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
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
               
                Int64[] retArr;
               
                Int64[] arrA = A.GetArrayForRead();
               
                Int64[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int workItemMultiplierLenA;
                int workItemMultiplierLenB;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<Int64>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    workItemMultiplierLenB = outDims[0]; 
                    workItemMultiplierLenA = dim;  // 0 for column, 1 for row vector
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<Int64>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    workItemMultiplierLenB = dim;  // 0 for column, 1 for row vector
                    workItemMultiplierLenA = outDims[0]; 
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in either A or B");
                }
                int itLen = outDims[0]; // (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                #endregion

                #region worker loops definition
                ILDenseStorage<Int64> retStorage = new ILDenseStorage<Int64>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;
                   
                    Int64* ap;
                   
                    Int64* bp;
                   
                    Int64* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            if (dim == 0) {
                                bp = (Int64*)range.Item3;
                                cp = (Int64*)range.Item4; 
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (Int64*)range.Item2;
                                    int l = itLen; 
                                    while (l > 10) {
                                        cp[0] =  saturateInt64Pow (ap[0]  , bp[0]);
                                        cp[1] =  saturateInt64Pow (ap[1]  , bp[1]);
                                        cp[2] =  saturateInt64Pow (ap[2]  , bp[2]);
                                        cp[3] =  saturateInt64Pow (ap[3]  , bp[3]);
                                        cp[4] =  saturateInt64Pow (ap[4]  , bp[4]);
                                        cp[5] =  saturateInt64Pow (ap[5]  , bp[5]);
                                        cp[6] =  saturateInt64Pow (ap[6]  , bp[6]);
                                        cp[7] =  saturateInt64Pow (ap[7]  , bp[7]);
                                        cp[8] =  saturateInt64Pow (ap[8]  , bp[8]);
                                        cp[9] =  saturateInt64Pow (ap[9]  , bp[9]);
                                        cp[10] =  saturateInt64Pow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  saturateInt64Pow (*ap++  , *bp++);
                                    }
                                }
                            } else {
                                // dim == 1
                                ap = (Int64*)range.Item2;
                                bp = (Int64*)range.Item3;
                                cp = (Int64*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   Int64 val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt64Pow (val  , bp[0]);
                                        cp[1] =  saturateInt64Pow (val  , bp[1]);
                                        cp[2] =  saturateInt64Pow (val  , bp[2]);
                                        cp[3] =  saturateInt64Pow (val  , bp[3]);
                                        cp[4] =  saturateInt64Pow (val  , bp[4]);
                                        cp[5] =  saturateInt64Pow (val  , bp[5]);
                                        cp[6] =  saturateInt64Pow (val  , bp[6]);
                                        cp[7] =  saturateInt64Pow (val  , bp[7]);
                                        cp[8] =  saturateInt64Pow (val  , bp[8]);
                                        cp[9] =  saturateInt64Pow (val  , bp[9]);
                                        cp[10] =  saturateInt64Pow (val  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  saturateInt64Pow (val  , *bp++);
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            if (dim == 0) {
                                cp = (Int64*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (Int64*)range.Item2;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt64Pow (ap[0]  , cp[0]);
                                        cp[1] =  saturateInt64Pow (ap[1]  , cp[1]);
                                        cp[2] =  saturateInt64Pow (ap[2]  , cp[2]);
                                        cp[3] =  saturateInt64Pow (ap[3]  , cp[3]);
                                        cp[4] =  saturateInt64Pow (ap[4]  , cp[4]);
                                        cp[5] =  saturateInt64Pow (ap[5]  , cp[5]);
                                        cp[6] =  saturateInt64Pow (ap[6]  , cp[6]);
                                        cp[7] =  saturateInt64Pow (ap[7]  , cp[7]);
                                        cp[8] =  saturateInt64Pow (ap[8]  , cp[8]);
                                        cp[9] =  saturateInt64Pow (ap[9]  , cp[9]);
                                        cp[10] =  saturateInt64Pow (ap[10]  , cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt64Pow (*ap++  , *cp);
                                        cp++; 
                                    }
                                }
                            } else {
                                // dim == 1
                                cp = (Int64*)range.Item4;
                                ap = (Int64*)range.Item2;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    Int64 val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt64Pow (val  , cp[0]);
                                        cp[1] =  saturateInt64Pow (val  , cp[1]);
                                        cp[2] =  saturateInt64Pow (val  , cp[2]);
                                        cp[3] =  saturateInt64Pow (val  , cp[3]);
                                        cp[4] =  saturateInt64Pow (val  , cp[4]);
                                        cp[5] =  saturateInt64Pow (val  , cp[5]);
                                        cp[6] =  saturateInt64Pow (val  , cp[6]);
                                        cp[7] =  saturateInt64Pow (val  , cp[7]);
                                        cp[8] =  saturateInt64Pow (val  , cp[8]);
                                        cp[9] =  saturateInt64Pow (val  , cp[9]);
                                        cp[10] =  saturateInt64Pow (val  , cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt64Pow (val  , *cp);
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            if (dim == 0) {
                                ap = (Int64*)range.Item2;
                                cp = (Int64*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (Int64*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt64Pow (ap[0]  , bp[0]);
                                        cp[1] =  saturateInt64Pow (ap[1]  , bp[1]);
                                        cp[2] =  saturateInt64Pow (ap[2]  , bp[2]);
                                        cp[3] =  saturateInt64Pow (ap[3]  , bp[3]);
                                        cp[4] =  saturateInt64Pow (ap[4]  , bp[4]);
                                        cp[5] =  saturateInt64Pow (ap[5]  , bp[5]);
                                        cp[6] =  saturateInt64Pow (ap[6]  , bp[6]);
                                        cp[7] =  saturateInt64Pow (ap[7]  , bp[7]);
                                        cp[8] =  saturateInt64Pow (ap[8]  , bp[8]);
                                        cp[9] =  saturateInt64Pow (ap[9]  , bp[9]);
                                        cp[10] =  saturateInt64Pow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt64Pow (*ap  , *bp);
                                        ap++;
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                ap = (Int64*)range.Item2;
                                bp = (Int64*)range.Item3;
                                cp = (Int64*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   Int64 val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt64Pow (ap[0]  , val);
                                        cp[1] =  saturateInt64Pow (ap[1]  , val);
                                        cp[2] =  saturateInt64Pow (ap[2]  , val);
                                        cp[3] =  saturateInt64Pow (ap[3]  , val);
                                        cp[4] =  saturateInt64Pow (ap[4]  , val);
                                        cp[5] =  saturateInt64Pow (ap[5]  , val);
                                        cp[6] =  saturateInt64Pow (ap[6]  , val);
                                        cp[7] =  saturateInt64Pow (ap[7]  , val);
                                        cp[8] =  saturateInt64Pow (ap[8]  , val);
                                        cp[9] =  saturateInt64Pow (ap[9]  , val);
                                        cp[10] =  saturateInt64Pow (ap[10]  , val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt64Pow (*ap  , val);
                                        ap++;
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            if (dim == 0) {
                                cp = (Int64*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (Int64*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt64Pow (cp[0]  , bp[0]);
                                        cp[1] =  saturateInt64Pow (cp[1]  , bp[1]);
                                        cp[2] =  saturateInt64Pow (cp[2]  , bp[2]);
                                        cp[3] =  saturateInt64Pow (cp[3]  , bp[3]);
                                        cp[4] =  saturateInt64Pow (cp[4]  , bp[4]);
                                        cp[5] =  saturateInt64Pow (cp[5]  , bp[5]);
                                        cp[6] =  saturateInt64Pow (cp[6]  , bp[6]);
                                        cp[7] =  saturateInt64Pow (cp[7]  , bp[7]);
                                        cp[8] =  saturateInt64Pow (cp[8]  , bp[8]);
                                        cp[9] =  saturateInt64Pow (cp[9]  , bp[9]);
                                        cp[10] =  saturateInt64Pow (cp[10]  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt64Pow (*cp  , *bp);
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                bp = (Int64*)range.Item3;
                                cp = (Int64*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    Int64 val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt64Pow (cp[0]  , val);
                                        cp[1] =  saturateInt64Pow (cp[1]  , val);
                                        cp[2] =  saturateInt64Pow (cp[2]  , val);
                                        cp[3] =  saturateInt64Pow (cp[3]  , val);
                                        cp[4] =  saturateInt64Pow (cp[4]  , val);
                                        cp[5] =  saturateInt64Pow (cp[5]  , val);
                                        cp[6] =  saturateInt64Pow (cp[6]  , val);
                                        cp[7] =  saturateInt64Pow (cp[7]  , val);
                                        cp[8] =  saturateInt64Pow (cp[8]  , val);
                                        cp[9] =  saturateInt64Pow (cp[9]  , val);
                                        cp[10] =  saturateInt64Pow (cp[10]  , val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =  saturateInt64Pow (*cp  , val);
                                        cp++;
                                    }
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                fixed ( Int64* arrAP = arrA)
                fixed ( Int64* arrBP = arrB)
                fixed ( Int64* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                               , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                               , (IntPtr)(retArrP + i * outDims[0] * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                                , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                                , (IntPtr)(retArrP + i * outDims[0] * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<Int64>(retStorage);
            //}  // no scopes here! it disables implace operations
        }

        /// <summary>Power elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with elementwise result of A <sup>B</sup></returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<Int32>  pow(ILInArray<Int32> A, ILInArray<Int32> B) {
            using (ILScope.Enter(A,B)) {
                int outLen; 
                BinOpItMode mode; 
                Int32 [] retArr;
                Int32 [] arrA = A.GetArrayForRead();
                Int32[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size; 
                    if (B.IsScalar) {
                       
                        return new  ILRetArray<Int32> (new  Int32 [1]{ saturateInt32Pow (A.GetValue(0)  , B.GetValue(0))}, A.Size);
                    } else if (B.IsEmpty) {
                        return  ILRetArray<Int32>.empty(outDims); 
                    } else {
                        outLen = outDims.NumberOfElements; 
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< Int32 > (outLen);
                            mode = BinOpItMode.SAN; 
                        } else {
                            mode = BinOpItMode.SAI; 
                        }
                    }
                } else {
                    outDims = A.Size; 
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return  ILRetArray<Int32>.empty(A.Size);  
                        }
                        outLen = A.S.NumberOfElements; 
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< Int32 > (outLen);
                            mode = BinOpItMode.ASN; 
                        } else {
                            mode = BinOpItMode.ASI; 
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  powEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements; 
                        if (A.TryGetStorage4InplaceOp(out retArr)) 
                            mode  = BinOpItMode.AAIA; 
                        else if (B.TryGetStorage4InplaceOp(out retArr)) 
                            mode = BinOpItMode.AAIB; 
                        else {
                            retArr = ILMemoryPool.Pool.New< Int32 > (outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<Int32> retStorage = new ILDenseStorage<Int32>(retArr, outDims);
                int i = 0, workerCount = 1; 
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    Int32* cp = (Int32*)range.Item5 + range.Item1;
                    Int32 scalar;
                    int j = range.Item2; 
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           Int32* bp = ((Int32*)range.Item4 + range.Item1);
                            while (j > 20) {
                                cp[0] =  saturateInt32Pow (cp[0]  , bp[0]);
                                cp[1] =  saturateInt32Pow (cp[1]  , bp[1]);
                                cp[2] =  saturateInt32Pow (cp[2]  , bp[2]);
                                cp[3] =  saturateInt32Pow (cp[3]  , bp[3]);
                                cp[4] =  saturateInt32Pow (cp[4]  , bp[4]);
                                cp[5] =  saturateInt32Pow (cp[5]  , bp[5]);
                                cp[6] =  saturateInt32Pow (cp[6]  , bp[6]);
                                cp[7] =  saturateInt32Pow (cp[7]  , bp[7]);
                                cp[8] =  saturateInt32Pow (cp[8]  , bp[8]);
                                cp[9] =  saturateInt32Pow (cp[9]  , bp[9]);
                                cp[10] =  saturateInt32Pow (cp[10]  , bp[10]);
                                cp[11] =  saturateInt32Pow (cp[11]  , bp[11]);
                                cp[12] =  saturateInt32Pow (cp[12]  , bp[12]);
                                cp[13] =  saturateInt32Pow (cp[13]  , bp[13]);
                                cp[14] =  saturateInt32Pow (cp[14]  , bp[14]);
                                cp[15] =  saturateInt32Pow (cp[15]  , bp[15]);
                                cp[16] =  saturateInt32Pow (cp[16]  , bp[16]);
                                cp[17] =  saturateInt32Pow (cp[17]  , bp[17]);
                                cp[18] =  saturateInt32Pow (cp[18]  , bp[18]);
                                cp[19] =  saturateInt32Pow (cp[19]  , bp[19]);
                                cp[20] =  saturateInt32Pow (cp[20]  , bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32Pow (*cp  , *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           Int32* ap = ((Int32*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt32Pow (ap[0]  , cp[0]);
                                cp[1] =  saturateInt32Pow (ap[1]  , cp[1]);
                                cp[2] =  saturateInt32Pow (ap[2]  , cp[2]);
                                cp[3] =  saturateInt32Pow (ap[3]  , cp[3]);
                                cp[4] =  saturateInt32Pow (ap[4]  , cp[4]);
                                cp[5] =  saturateInt32Pow (ap[5]  , cp[5]);
                                cp[6] =  saturateInt32Pow (ap[6]  , cp[6]);
                                cp[7] =  saturateInt32Pow (ap[7]  , cp[7]);
                                cp[8] =  saturateInt32Pow (ap[8]  , cp[8]);
                                cp[9] =  saturateInt32Pow (ap[9]  , cp[9]);
                                cp[10] =  saturateInt32Pow (ap[10]  , cp[10]);
                                cp[11] =  saturateInt32Pow (ap[11]  , cp[11]);
                                cp[12] =  saturateInt32Pow (ap[12]  , cp[12]);
                                cp[13] =  saturateInt32Pow (ap[13]  , cp[13]);
                                cp[14] =  saturateInt32Pow (ap[14]  , cp[14]);
                                cp[15] =  saturateInt32Pow (ap[15]  , cp[15]);
                                cp[16] =  saturateInt32Pow (ap[16]  , cp[16]);
                                cp[17] =  saturateInt32Pow (ap[17]  , cp[17]);
                                cp[18] =  saturateInt32Pow (ap[18]  , cp[18]);
                                cp[19] =  saturateInt32Pow (ap[19]  , cp[19]);
                                cp[20] =  saturateInt32Pow (ap[20]  , cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32Pow (*ap  , *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((Int32*)range.Item3 + range.Item1);
                            bp = ((Int32*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt32Pow (ap[0]  , bp[0]);
                                cp[1] =  saturateInt32Pow (ap[1]  , bp[1]);
                                cp[2] =  saturateInt32Pow (ap[2]  , bp[2]);
                                cp[3] =  saturateInt32Pow (ap[3]  , bp[3]);
                                cp[4] =  saturateInt32Pow (ap[4]  , bp[4]);
                                cp[5] =  saturateInt32Pow (ap[5]  , bp[5]);
                                cp[6] =  saturateInt32Pow (ap[6]  , bp[6]);
                                cp[7] =  saturateInt32Pow (ap[7]  , bp[7]);
                                cp[8] =  saturateInt32Pow (ap[8]  , bp[8]);
                                cp[9] =  saturateInt32Pow (ap[9]  , bp[9]);
                                cp[10] =  saturateInt32Pow (ap[10]  , bp[10]);
                                cp[11] =  saturateInt32Pow (ap[11]  , bp[11]);
                                cp[12] =  saturateInt32Pow (ap[12]  , bp[12]);
                                cp[13] =  saturateInt32Pow (ap[13]  , bp[13]);
                                cp[14] =  saturateInt32Pow (ap[14]  , bp[14]);
                                cp[15] =  saturateInt32Pow (ap[15]  , bp[15]);
                                cp[16] =  saturateInt32Pow (ap[16]  , bp[16]);
                                cp[17] =  saturateInt32Pow (ap[17]  , bp[17]);
                                cp[18] =  saturateInt32Pow (ap[18]  , bp[18]);
                                cp[19] =  saturateInt32Pow (ap[19]  , bp[19]);
                                cp[20] =  saturateInt32Pow (ap[20]  , bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32Pow (*ap  , *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((Int32*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt32Pow (cp[0]  , scalar);
                                cp[1] =  saturateInt32Pow (cp[1]  , scalar);
                                cp[2] =  saturateInt32Pow (cp[2]  , scalar);
                                cp[3] =  saturateInt32Pow (cp[3]  , scalar);
                                cp[4] =  saturateInt32Pow (cp[4]  , scalar);
                                cp[5] =  saturateInt32Pow (cp[5]  , scalar);
                                cp[6] =  saturateInt32Pow (cp[6]  , scalar);
                                cp[7] =  saturateInt32Pow (cp[7]  , scalar);
                                cp[8] =  saturateInt32Pow (cp[8]  , scalar);
                                cp[9] =  saturateInt32Pow (cp[9]  , scalar);
                                cp[10] =  saturateInt32Pow (cp[10]  , scalar);
                                cp[11] =  saturateInt32Pow (cp[11]  , scalar);
                                cp[12] =  saturateInt32Pow (cp[12]  , scalar);
                                cp[13] =  saturateInt32Pow (cp[13]  , scalar);
                                cp[14] =  saturateInt32Pow (cp[14]  , scalar);
                                cp[15] =  saturateInt32Pow (cp[15]  , scalar);
                                cp[16] =  saturateInt32Pow (cp[16]  , scalar);
                                cp[17] =  saturateInt32Pow (cp[17]  , scalar);
                                cp[18] =  saturateInt32Pow (cp[18]  , scalar);
                                cp[19] =  saturateInt32Pow (cp[19]  , scalar);
                                cp[20] =  saturateInt32Pow (cp[20]  , scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32Pow (*cp  , scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((Int32*)range.Item3 + range.Item1);
                            scalar = *((Int32*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt32Pow (ap[0]  , scalar);
                                cp[1] =  saturateInt32Pow (ap[1]  , scalar);
                                cp[2] =  saturateInt32Pow (ap[2]  , scalar);
                                cp[3] =  saturateInt32Pow (ap[3]  , scalar);
                                cp[4] =  saturateInt32Pow (ap[4]  , scalar);
                                cp[5] =  saturateInt32Pow (ap[5]  , scalar);
                                cp[6] =  saturateInt32Pow (ap[6]  , scalar);
                                cp[7] =  saturateInt32Pow (ap[7]  , scalar);
                                cp[8] =  saturateInt32Pow (ap[8]  , scalar);
                                cp[9] =  saturateInt32Pow (ap[9]  , scalar);
                                cp[10] =  saturateInt32Pow (ap[10]  , scalar);
                                cp[11] =  saturateInt32Pow (ap[11]  , scalar);
                                cp[12] =  saturateInt32Pow (ap[12]  , scalar);
                                cp[13] =  saturateInt32Pow (ap[13]  , scalar);
                                cp[14] =  saturateInt32Pow (ap[14]  , scalar);
                                cp[15] =  saturateInt32Pow (ap[15]  , scalar);
                                cp[16] =  saturateInt32Pow (ap[16]  , scalar);
                                cp[17] =  saturateInt32Pow (ap[17]  , scalar);
                                cp[18] =  saturateInt32Pow (ap[18]  , scalar);
                                cp[19] =  saturateInt32Pow (ap[19]  , scalar);
                                cp[20] =  saturateInt32Pow (ap[20]  , scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32Pow (*ap  , scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((Int32*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt32Pow (scalar  , cp[0]);
                                cp[1] =  saturateInt32Pow (scalar  , cp[1]);
                                cp[2] =  saturateInt32Pow (scalar  , cp[2]);
                                cp[3] =  saturateInt32Pow (scalar  , cp[3]);
                                cp[4] =  saturateInt32Pow (scalar  , cp[4]);
                                cp[5] =  saturateInt32Pow (scalar  , cp[5]);
                                cp[6] =  saturateInt32Pow (scalar  , cp[6]);
                                cp[7] =  saturateInt32Pow (scalar  , cp[7]);
                                cp[8] =  saturateInt32Pow (scalar  , cp[8]);
                                cp[9] =  saturateInt32Pow (scalar  , cp[9]);
                                cp[10] =  saturateInt32Pow (scalar  , cp[10]);
                                cp[11] =  saturateInt32Pow (scalar  , cp[11]);
                                cp[12] =  saturateInt32Pow (scalar  , cp[12]);
                                cp[13] =  saturateInt32Pow (scalar  , cp[13]);
                                cp[14] =  saturateInt32Pow (scalar  , cp[14]);
                                cp[15] =  saturateInt32Pow (scalar  , cp[15]);
                                cp[16] =  saturateInt32Pow (scalar  , cp[16]);
                                cp[17] =  saturateInt32Pow (scalar  , cp[17]);
                                cp[18] =  saturateInt32Pow (scalar  , cp[18]);
                                cp[19] =  saturateInt32Pow (scalar  , cp[19]);
                                cp[20] =  saturateInt32Pow (scalar  , cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32Pow (scalar  , *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((Int32*)range.Item3);
                            bp = ((Int32*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt32Pow (scalar  , bp[0]);
                                cp[1] =  saturateInt32Pow (scalar  , bp[1]);
                                cp[2] =  saturateInt32Pow (scalar  , bp[2]);
                                cp[3] =  saturateInt32Pow (scalar  , bp[3]);
                                cp[4] =  saturateInt32Pow (scalar  , bp[4]);
                                cp[5] =  saturateInt32Pow (scalar  , bp[5]);
                                cp[6] =  saturateInt32Pow (scalar  , bp[6]);
                                cp[7] =  saturateInt32Pow (scalar  , bp[7]);
                                cp[8] =  saturateInt32Pow (scalar  , bp[8]);
                                cp[9] =  saturateInt32Pow (scalar  , bp[9]);
                                cp[10] =  saturateInt32Pow (scalar  , bp[10]);
                                cp[11] =  saturateInt32Pow (scalar  , bp[11]);
                                cp[12] =  saturateInt32Pow (scalar  , bp[12]);
                                cp[13] =  saturateInt32Pow (scalar  , bp[13]);
                                cp[14] =  saturateInt32Pow (scalar  , bp[14]);
                                cp[15] =  saturateInt32Pow (scalar  , bp[15]);
                                cp[16] =  saturateInt32Pow (scalar  , bp[16]);
                                cp[17] =  saturateInt32Pow (scalar  , bp[17]);
                                cp[18] =  saturateInt32Pow (scalar  , bp[18]);
                                cp[19] =  saturateInt32Pow (scalar  , bp[19]);
                                cp[20] =  saturateInt32Pow (scalar  , bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32Pow (scalar  , *bp);
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);

                };

                #region do the work 
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
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
                
                fixed ( Int32* arrAP = arrA)
                fixed ( Int32* arrBP = arrB)
                fixed ( Int32* retArrP = retArr) {

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
                return new  ILRetArray< Int32>(retStorage);
            }
        }

        private static unsafe ILRetArray<Int32>  powEx(ILInArray<Int32> A, ILInArray<Int32> B) {
            //using (ILScope.Enter(A, B)) { we cannot start a new scope here, since this would prevent A and B to be used implace if applicable

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<Int32>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<Int32>(B.S);
                } else if (B.IsEmpty) {
                    return empty<Int32>(A.S);
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
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
               
                Int32[] retArr;
               
                Int32[] arrA = A.GetArrayForRead();
               
                Int32[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int workItemMultiplierLenA;
                int workItemMultiplierLenB;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<Int32>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    workItemMultiplierLenB = outDims[0]; 
                    workItemMultiplierLenA = dim;  // 0 for column, 1 for row vector
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<Int32>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    workItemMultiplierLenB = dim;  // 0 for column, 1 for row vector
                    workItemMultiplierLenA = outDims[0]; 
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in either A or B");
                }
                int itLen = outDims[0]; // (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                #endregion

                #region worker loops definition
                ILDenseStorage<Int32> retStorage = new ILDenseStorage<Int32>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;
                   
                    Int32* ap;
                   
                    Int32* bp;
                   
                    Int32* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            if (dim == 0) {
                                bp = (Int32*)range.Item3;
                                cp = (Int32*)range.Item4; 
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (Int32*)range.Item2;
                                    int l = itLen; 
                                    while (l > 10) {
                                        cp[0] =  saturateInt32Pow (ap[0]  , bp[0]);
                                        cp[1] =  saturateInt32Pow (ap[1]  , bp[1]);
                                        cp[2] =  saturateInt32Pow (ap[2]  , bp[2]);
                                        cp[3] =  saturateInt32Pow (ap[3]  , bp[3]);
                                        cp[4] =  saturateInt32Pow (ap[4]  , bp[4]);
                                        cp[5] =  saturateInt32Pow (ap[5]  , bp[5]);
                                        cp[6] =  saturateInt32Pow (ap[6]  , bp[6]);
                                        cp[7] =  saturateInt32Pow (ap[7]  , bp[7]);
                                        cp[8] =  saturateInt32Pow (ap[8]  , bp[8]);
                                        cp[9] =  saturateInt32Pow (ap[9]  , bp[9]);
                                        cp[10] =  saturateInt32Pow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  saturateInt32Pow (*ap++  , *bp++);
                                    }
                                }
                            } else {
                                // dim == 1
                                ap = (Int32*)range.Item2;
                                bp = (Int32*)range.Item3;
                                cp = (Int32*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   Int32 val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt32Pow (val  , bp[0]);
                                        cp[1] =  saturateInt32Pow (val  , bp[1]);
                                        cp[2] =  saturateInt32Pow (val  , bp[2]);
                                        cp[3] =  saturateInt32Pow (val  , bp[3]);
                                        cp[4] =  saturateInt32Pow (val  , bp[4]);
                                        cp[5] =  saturateInt32Pow (val  , bp[5]);
                                        cp[6] =  saturateInt32Pow (val  , bp[6]);
                                        cp[7] =  saturateInt32Pow (val  , bp[7]);
                                        cp[8] =  saturateInt32Pow (val  , bp[8]);
                                        cp[9] =  saturateInt32Pow (val  , bp[9]);
                                        cp[10] =  saturateInt32Pow (val  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  saturateInt32Pow (val  , *bp++);
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            if (dim == 0) {
                                cp = (Int32*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (Int32*)range.Item2;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt32Pow (ap[0]  , cp[0]);
                                        cp[1] =  saturateInt32Pow (ap[1]  , cp[1]);
                                        cp[2] =  saturateInt32Pow (ap[2]  , cp[2]);
                                        cp[3] =  saturateInt32Pow (ap[3]  , cp[3]);
                                        cp[4] =  saturateInt32Pow (ap[4]  , cp[4]);
                                        cp[5] =  saturateInt32Pow (ap[5]  , cp[5]);
                                        cp[6] =  saturateInt32Pow (ap[6]  , cp[6]);
                                        cp[7] =  saturateInt32Pow (ap[7]  , cp[7]);
                                        cp[8] =  saturateInt32Pow (ap[8]  , cp[8]);
                                        cp[9] =  saturateInt32Pow (ap[9]  , cp[9]);
                                        cp[10] =  saturateInt32Pow (ap[10]  , cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt32Pow (*ap++  , *cp);
                                        cp++; 
                                    }
                                }
                            } else {
                                // dim == 1
                                cp = (Int32*)range.Item4;
                                ap = (Int32*)range.Item2;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    Int32 val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt32Pow (val  , cp[0]);
                                        cp[1] =  saturateInt32Pow (val  , cp[1]);
                                        cp[2] =  saturateInt32Pow (val  , cp[2]);
                                        cp[3] =  saturateInt32Pow (val  , cp[3]);
                                        cp[4] =  saturateInt32Pow (val  , cp[4]);
                                        cp[5] =  saturateInt32Pow (val  , cp[5]);
                                        cp[6] =  saturateInt32Pow (val  , cp[6]);
                                        cp[7] =  saturateInt32Pow (val  , cp[7]);
                                        cp[8] =  saturateInt32Pow (val  , cp[8]);
                                        cp[9] =  saturateInt32Pow (val  , cp[9]);
                                        cp[10] =  saturateInt32Pow (val  , cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt32Pow (val  , *cp);
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            if (dim == 0) {
                                ap = (Int32*)range.Item2;
                                cp = (Int32*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (Int32*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt32Pow (ap[0]  , bp[0]);
                                        cp[1] =  saturateInt32Pow (ap[1]  , bp[1]);
                                        cp[2] =  saturateInt32Pow (ap[2]  , bp[2]);
                                        cp[3] =  saturateInt32Pow (ap[3]  , bp[3]);
                                        cp[4] =  saturateInt32Pow (ap[4]  , bp[4]);
                                        cp[5] =  saturateInt32Pow (ap[5]  , bp[5]);
                                        cp[6] =  saturateInt32Pow (ap[6]  , bp[6]);
                                        cp[7] =  saturateInt32Pow (ap[7]  , bp[7]);
                                        cp[8] =  saturateInt32Pow (ap[8]  , bp[8]);
                                        cp[9] =  saturateInt32Pow (ap[9]  , bp[9]);
                                        cp[10] =  saturateInt32Pow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt32Pow (*ap  , *bp);
                                        ap++;
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                ap = (Int32*)range.Item2;
                                bp = (Int32*)range.Item3;
                                cp = (Int32*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   Int32 val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt32Pow (ap[0]  , val);
                                        cp[1] =  saturateInt32Pow (ap[1]  , val);
                                        cp[2] =  saturateInt32Pow (ap[2]  , val);
                                        cp[3] =  saturateInt32Pow (ap[3]  , val);
                                        cp[4] =  saturateInt32Pow (ap[4]  , val);
                                        cp[5] =  saturateInt32Pow (ap[5]  , val);
                                        cp[6] =  saturateInt32Pow (ap[6]  , val);
                                        cp[7] =  saturateInt32Pow (ap[7]  , val);
                                        cp[8] =  saturateInt32Pow (ap[8]  , val);
                                        cp[9] =  saturateInt32Pow (ap[9]  , val);
                                        cp[10] =  saturateInt32Pow (ap[10]  , val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt32Pow (*ap  , val);
                                        ap++;
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            if (dim == 0) {
                                cp = (Int32*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (Int32*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt32Pow (cp[0]  , bp[0]);
                                        cp[1] =  saturateInt32Pow (cp[1]  , bp[1]);
                                        cp[2] =  saturateInt32Pow (cp[2]  , bp[2]);
                                        cp[3] =  saturateInt32Pow (cp[3]  , bp[3]);
                                        cp[4] =  saturateInt32Pow (cp[4]  , bp[4]);
                                        cp[5] =  saturateInt32Pow (cp[5]  , bp[5]);
                                        cp[6] =  saturateInt32Pow (cp[6]  , bp[6]);
                                        cp[7] =  saturateInt32Pow (cp[7]  , bp[7]);
                                        cp[8] =  saturateInt32Pow (cp[8]  , bp[8]);
                                        cp[9] =  saturateInt32Pow (cp[9]  , bp[9]);
                                        cp[10] =  saturateInt32Pow (cp[10]  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt32Pow (*cp  , *bp);
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                bp = (Int32*)range.Item3;
                                cp = (Int32*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    Int32 val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateInt32Pow (cp[0]  , val);
                                        cp[1] =  saturateInt32Pow (cp[1]  , val);
                                        cp[2] =  saturateInt32Pow (cp[2]  , val);
                                        cp[3] =  saturateInt32Pow (cp[3]  , val);
                                        cp[4] =  saturateInt32Pow (cp[4]  , val);
                                        cp[5] =  saturateInt32Pow (cp[5]  , val);
                                        cp[6] =  saturateInt32Pow (cp[6]  , val);
                                        cp[7] =  saturateInt32Pow (cp[7]  , val);
                                        cp[8] =  saturateInt32Pow (cp[8]  , val);
                                        cp[9] =  saturateInt32Pow (cp[9]  , val);
                                        cp[10] =  saturateInt32Pow (cp[10]  , val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =  saturateInt32Pow (*cp  , val);
                                        cp++;
                                    }
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                fixed ( Int32* arrAP = arrA)
                fixed ( Int32* arrBP = arrB)
                fixed ( Int32* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                               , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                               , (IntPtr)(retArrP + i * outDims[0] * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                                , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                                , (IntPtr)(retArrP + i * outDims[0] * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<Int32>(retStorage);
            //}  // no scopes here! it disables implace operations
        }

        /// <summary>Power elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with elementwise result of A <sup>B</sup></returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<float>  pow(ILInArray<float> A, ILInArray<float> B) {
            using (ILScope.Enter(A,B)) {
                int outLen; 
                BinOpItMode mode; 
                float [] retArr;
                float [] arrA = A.GetArrayForRead();
                float[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size; 
                    if (B.IsScalar) {
                       
                        return new  ILRetArray<float> (new  float [1]{ (float)Math.Pow (A.GetValue(0)  , B.GetValue(0))}, A.Size);
                    } else if (B.IsEmpty) {
                        return  ILRetArray<float>.empty(outDims); 
                    } else {
                        outLen = outDims.NumberOfElements; 
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< float > (outLen);
                            mode = BinOpItMode.SAN; 
                        } else {
                            mode = BinOpItMode.SAI; 
                        }
                    }
                } else {
                    outDims = A.Size; 
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return  ILRetArray<float>.empty(A.Size);  
                        }
                        outLen = A.S.NumberOfElements; 
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< float > (outLen);
                            mode = BinOpItMode.ASN; 
                        } else {
                            mode = BinOpItMode.ASI; 
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  powEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements; 
                        if (A.TryGetStorage4InplaceOp(out retArr)) 
                            mode  = BinOpItMode.AAIA; 
                        else if (B.TryGetStorage4InplaceOp(out retArr)) 
                            mode = BinOpItMode.AAIB; 
                        else {
                            retArr = ILMemoryPool.Pool.New< float > (outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<float> retStorage = new ILDenseStorage<float>(retArr, outDims);
                int i = 0, workerCount = 1; 
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    float* cp = (float*)range.Item5 + range.Item1;
                    float scalar;
                    int j = range.Item2; 
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           float* bp = ((float*)range.Item4 + range.Item1);
                            while (j > 20) {
                                cp[0] =  (float)Math.Pow (cp[0]  , bp[0]);
                                cp[1] =  (float)Math.Pow (cp[1]  , bp[1]);
                                cp[2] =  (float)Math.Pow (cp[2]  , bp[2]);
                                cp[3] =  (float)Math.Pow (cp[3]  , bp[3]);
                                cp[4] =  (float)Math.Pow (cp[4]  , bp[4]);
                                cp[5] =  (float)Math.Pow (cp[5]  , bp[5]);
                                cp[6] =  (float)Math.Pow (cp[6]  , bp[6]);
                                cp[7] =  (float)Math.Pow (cp[7]  , bp[7]);
                                cp[8] =  (float)Math.Pow (cp[8]  , bp[8]);
                                cp[9] =  (float)Math.Pow (cp[9]  , bp[9]);
                                cp[10] =  (float)Math.Pow (cp[10]  , bp[10]);
                                cp[11] =  (float)Math.Pow (cp[11]  , bp[11]);
                                cp[12] =  (float)Math.Pow (cp[12]  , bp[12]);
                                cp[13] =  (float)Math.Pow (cp[13]  , bp[13]);
                                cp[14] =  (float)Math.Pow (cp[14]  , bp[14]);
                                cp[15] =  (float)Math.Pow (cp[15]  , bp[15]);
                                cp[16] =  (float)Math.Pow (cp[16]  , bp[16]);
                                cp[17] =  (float)Math.Pow (cp[17]  , bp[17]);
                                cp[18] =  (float)Math.Pow (cp[18]  , bp[18]);
                                cp[19] =  (float)Math.Pow (cp[19]  , bp[19]);
                                cp[20] =  (float)Math.Pow (cp[20]  , bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  (float)Math.Pow (*cp  , *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           float* ap = ((float*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  (float)Math.Pow (ap[0]  , cp[0]);
                                cp[1] =  (float)Math.Pow (ap[1]  , cp[1]);
                                cp[2] =  (float)Math.Pow (ap[2]  , cp[2]);
                                cp[3] =  (float)Math.Pow (ap[3]  , cp[3]);
                                cp[4] =  (float)Math.Pow (ap[4]  , cp[4]);
                                cp[5] =  (float)Math.Pow (ap[5]  , cp[5]);
                                cp[6] =  (float)Math.Pow (ap[6]  , cp[6]);
                                cp[7] =  (float)Math.Pow (ap[7]  , cp[7]);
                                cp[8] =  (float)Math.Pow (ap[8]  , cp[8]);
                                cp[9] =  (float)Math.Pow (ap[9]  , cp[9]);
                                cp[10] =  (float)Math.Pow (ap[10]  , cp[10]);
                                cp[11] =  (float)Math.Pow (ap[11]  , cp[11]);
                                cp[12] =  (float)Math.Pow (ap[12]  , cp[12]);
                                cp[13] =  (float)Math.Pow (ap[13]  , cp[13]);
                                cp[14] =  (float)Math.Pow (ap[14]  , cp[14]);
                                cp[15] =  (float)Math.Pow (ap[15]  , cp[15]);
                                cp[16] =  (float)Math.Pow (ap[16]  , cp[16]);
                                cp[17] =  (float)Math.Pow (ap[17]  , cp[17]);
                                cp[18] =  (float)Math.Pow (ap[18]  , cp[18]);
                                cp[19] =  (float)Math.Pow (ap[19]  , cp[19]);
                                cp[20] =  (float)Math.Pow (ap[20]  , cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  (float)Math.Pow (*ap  , *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((float*)range.Item3 + range.Item1);
                            bp = ((float*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  (float)Math.Pow (ap[0]  , bp[0]);
                                cp[1] =  (float)Math.Pow (ap[1]  , bp[1]);
                                cp[2] =  (float)Math.Pow (ap[2]  , bp[2]);
                                cp[3] =  (float)Math.Pow (ap[3]  , bp[3]);
                                cp[4] =  (float)Math.Pow (ap[4]  , bp[4]);
                                cp[5] =  (float)Math.Pow (ap[5]  , bp[5]);
                                cp[6] =  (float)Math.Pow (ap[6]  , bp[6]);
                                cp[7] =  (float)Math.Pow (ap[7]  , bp[7]);
                                cp[8] =  (float)Math.Pow (ap[8]  , bp[8]);
                                cp[9] =  (float)Math.Pow (ap[9]  , bp[9]);
                                cp[10] =  (float)Math.Pow (ap[10]  , bp[10]);
                                cp[11] =  (float)Math.Pow (ap[11]  , bp[11]);
                                cp[12] =  (float)Math.Pow (ap[12]  , bp[12]);
                                cp[13] =  (float)Math.Pow (ap[13]  , bp[13]);
                                cp[14] =  (float)Math.Pow (ap[14]  , bp[14]);
                                cp[15] =  (float)Math.Pow (ap[15]  , bp[15]);
                                cp[16] =  (float)Math.Pow (ap[16]  , bp[16]);
                                cp[17] =  (float)Math.Pow (ap[17]  , bp[17]);
                                cp[18] =  (float)Math.Pow (ap[18]  , bp[18]);
                                cp[19] =  (float)Math.Pow (ap[19]  , bp[19]);
                                cp[20] =  (float)Math.Pow (ap[20]  , bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =  (float)Math.Pow (*ap  , *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((float*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  (float)Math.Pow (cp[0]  , scalar);
                                cp[1] =  (float)Math.Pow (cp[1]  , scalar);
                                cp[2] =  (float)Math.Pow (cp[2]  , scalar);
                                cp[3] =  (float)Math.Pow (cp[3]  , scalar);
                                cp[4] =  (float)Math.Pow (cp[4]  , scalar);
                                cp[5] =  (float)Math.Pow (cp[5]  , scalar);
                                cp[6] =  (float)Math.Pow (cp[6]  , scalar);
                                cp[7] =  (float)Math.Pow (cp[7]  , scalar);
                                cp[8] =  (float)Math.Pow (cp[8]  , scalar);
                                cp[9] =  (float)Math.Pow (cp[9]  , scalar);
                                cp[10] =  (float)Math.Pow (cp[10]  , scalar);
                                cp[11] =  (float)Math.Pow (cp[11]  , scalar);
                                cp[12] =  (float)Math.Pow (cp[12]  , scalar);
                                cp[13] =  (float)Math.Pow (cp[13]  , scalar);
                                cp[14] =  (float)Math.Pow (cp[14]  , scalar);
                                cp[15] =  (float)Math.Pow (cp[15]  , scalar);
                                cp[16] =  (float)Math.Pow (cp[16]  , scalar);
                                cp[17] =  (float)Math.Pow (cp[17]  , scalar);
                                cp[18] =  (float)Math.Pow (cp[18]  , scalar);
                                cp[19] =  (float)Math.Pow (cp[19]  , scalar);
                                cp[20] =  (float)Math.Pow (cp[20]  , scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  (float)Math.Pow (*cp  , scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((float*)range.Item3 + range.Item1);
                            scalar = *((float*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  (float)Math.Pow (ap[0]  , scalar);
                                cp[1] =  (float)Math.Pow (ap[1]  , scalar);
                                cp[2] =  (float)Math.Pow (ap[2]  , scalar);
                                cp[3] =  (float)Math.Pow (ap[3]  , scalar);
                                cp[4] =  (float)Math.Pow (ap[4]  , scalar);
                                cp[5] =  (float)Math.Pow (ap[5]  , scalar);
                                cp[6] =  (float)Math.Pow (ap[6]  , scalar);
                                cp[7] =  (float)Math.Pow (ap[7]  , scalar);
                                cp[8] =  (float)Math.Pow (ap[8]  , scalar);
                                cp[9] =  (float)Math.Pow (ap[9]  , scalar);
                                cp[10] =  (float)Math.Pow (ap[10]  , scalar);
                                cp[11] =  (float)Math.Pow (ap[11]  , scalar);
                                cp[12] =  (float)Math.Pow (ap[12]  , scalar);
                                cp[13] =  (float)Math.Pow (ap[13]  , scalar);
                                cp[14] =  (float)Math.Pow (ap[14]  , scalar);
                                cp[15] =  (float)Math.Pow (ap[15]  , scalar);
                                cp[16] =  (float)Math.Pow (ap[16]  , scalar);
                                cp[17] =  (float)Math.Pow (ap[17]  , scalar);
                                cp[18] =  (float)Math.Pow (ap[18]  , scalar);
                                cp[19] =  (float)Math.Pow (ap[19]  , scalar);
                                cp[20] =  (float)Math.Pow (ap[20]  , scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  (float)Math.Pow (*ap  , scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((float*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =  (float)Math.Pow (scalar  , cp[0]);
                                cp[1] =  (float)Math.Pow (scalar  , cp[1]);
                                cp[2] =  (float)Math.Pow (scalar  , cp[2]);
                                cp[3] =  (float)Math.Pow (scalar  , cp[3]);
                                cp[4] =  (float)Math.Pow (scalar  , cp[4]);
                                cp[5] =  (float)Math.Pow (scalar  , cp[5]);
                                cp[6] =  (float)Math.Pow (scalar  , cp[6]);
                                cp[7] =  (float)Math.Pow (scalar  , cp[7]);
                                cp[8] =  (float)Math.Pow (scalar  , cp[8]);
                                cp[9] =  (float)Math.Pow (scalar  , cp[9]);
                                cp[10] =  (float)Math.Pow (scalar  , cp[10]);
                                cp[11] =  (float)Math.Pow (scalar  , cp[11]);
                                cp[12] =  (float)Math.Pow (scalar  , cp[12]);
                                cp[13] =  (float)Math.Pow (scalar  , cp[13]);
                                cp[14] =  (float)Math.Pow (scalar  , cp[14]);
                                cp[15] =  (float)Math.Pow (scalar  , cp[15]);
                                cp[16] =  (float)Math.Pow (scalar  , cp[16]);
                                cp[17] =  (float)Math.Pow (scalar  , cp[17]);
                                cp[18] =  (float)Math.Pow (scalar  , cp[18]);
                                cp[19] =  (float)Math.Pow (scalar  , cp[19]);
                                cp[20] =  (float)Math.Pow (scalar  , cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  (float)Math.Pow (scalar  , *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((float*)range.Item3);
                            bp = ((float*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  (float)Math.Pow (scalar  , bp[0]);
                                cp[1] =  (float)Math.Pow (scalar  , bp[1]);
                                cp[2] =  (float)Math.Pow (scalar  , bp[2]);
                                cp[3] =  (float)Math.Pow (scalar  , bp[3]);
                                cp[4] =  (float)Math.Pow (scalar  , bp[4]);
                                cp[5] =  (float)Math.Pow (scalar  , bp[5]);
                                cp[6] =  (float)Math.Pow (scalar  , bp[6]);
                                cp[7] =  (float)Math.Pow (scalar  , bp[7]);
                                cp[8] =  (float)Math.Pow (scalar  , bp[8]);
                                cp[9] =  (float)Math.Pow (scalar  , bp[9]);
                                cp[10] =  (float)Math.Pow (scalar  , bp[10]);
                                cp[11] =  (float)Math.Pow (scalar  , bp[11]);
                                cp[12] =  (float)Math.Pow (scalar  , bp[12]);
                                cp[13] =  (float)Math.Pow (scalar  , bp[13]);
                                cp[14] =  (float)Math.Pow (scalar  , bp[14]);
                                cp[15] =  (float)Math.Pow (scalar  , bp[15]);
                                cp[16] =  (float)Math.Pow (scalar  , bp[16]);
                                cp[17] =  (float)Math.Pow (scalar  , bp[17]);
                                cp[18] =  (float)Math.Pow (scalar  , bp[18]);
                                cp[19] =  (float)Math.Pow (scalar  , bp[19]);
                                cp[20] =  (float)Math.Pow (scalar  , bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  (float)Math.Pow (scalar  , *bp);
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);

                };

                #region do the work 
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
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
                
                fixed ( float* arrAP = arrA)
                fixed ( float* arrBP = arrB)
                fixed ( float* retArrP = retArr) {

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
                return new  ILRetArray< float>(retStorage);
            }
        }

        private static unsafe ILRetArray<float>  powEx(ILInArray<float> A, ILInArray<float> B) {
            //using (ILScope.Enter(A, B)) { we cannot start a new scope here, since this would prevent A and B to be used implace if applicable

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<float>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<float>(B.S);
                } else if (B.IsEmpty) {
                    return empty<float>(A.S);
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
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
               
                float[] retArr;
               
                float[] arrA = A.GetArrayForRead();
               
                float[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int workItemMultiplierLenA;
                int workItemMultiplierLenB;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<float>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    workItemMultiplierLenB = outDims[0]; 
                    workItemMultiplierLenA = dim;  // 0 for column, 1 for row vector
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<float>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    workItemMultiplierLenB = dim;  // 0 for column, 1 for row vector
                    workItemMultiplierLenA = outDims[0]; 
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in either A or B");
                }
                int itLen = outDims[0]; // (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                #endregion

                #region worker loops definition
                ILDenseStorage<float> retStorage = new ILDenseStorage<float>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;
                   
                    float* ap;
                   
                    float* bp;
                   
                    float* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            if (dim == 0) {
                                bp = (float*)range.Item3;
                                cp = (float*)range.Item4; 
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (float*)range.Item2;
                                    int l = itLen; 
                                    while (l > 10) {
                                        cp[0] =  (float)Math.Pow (ap[0]  , bp[0]);
                                        cp[1] =  (float)Math.Pow (ap[1]  , bp[1]);
                                        cp[2] =  (float)Math.Pow (ap[2]  , bp[2]);
                                        cp[3] =  (float)Math.Pow (ap[3]  , bp[3]);
                                        cp[4] =  (float)Math.Pow (ap[4]  , bp[4]);
                                        cp[5] =  (float)Math.Pow (ap[5]  , bp[5]);
                                        cp[6] =  (float)Math.Pow (ap[6]  , bp[6]);
                                        cp[7] =  (float)Math.Pow (ap[7]  , bp[7]);
                                        cp[8] =  (float)Math.Pow (ap[8]  , bp[8]);
                                        cp[9] =  (float)Math.Pow (ap[9]  , bp[9]);
                                        cp[10] =  (float)Math.Pow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  (float)Math.Pow (*ap++  , *bp++);
                                    }
                                }
                            } else {
                                // dim == 1
                                ap = (float*)range.Item2;
                                bp = (float*)range.Item3;
                                cp = (float*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   float val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  (float)Math.Pow (val  , bp[0]);
                                        cp[1] =  (float)Math.Pow (val  , bp[1]);
                                        cp[2] =  (float)Math.Pow (val  , bp[2]);
                                        cp[3] =  (float)Math.Pow (val  , bp[3]);
                                        cp[4] =  (float)Math.Pow (val  , bp[4]);
                                        cp[5] =  (float)Math.Pow (val  , bp[5]);
                                        cp[6] =  (float)Math.Pow (val  , bp[6]);
                                        cp[7] =  (float)Math.Pow (val  , bp[7]);
                                        cp[8] =  (float)Math.Pow (val  , bp[8]);
                                        cp[9] =  (float)Math.Pow (val  , bp[9]);
                                        cp[10] =  (float)Math.Pow (val  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  (float)Math.Pow (val  , *bp++);
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            if (dim == 0) {
                                cp = (float*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (float*)range.Item2;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  (float)Math.Pow (ap[0]  , cp[0]);
                                        cp[1] =  (float)Math.Pow (ap[1]  , cp[1]);
                                        cp[2] =  (float)Math.Pow (ap[2]  , cp[2]);
                                        cp[3] =  (float)Math.Pow (ap[3]  , cp[3]);
                                        cp[4] =  (float)Math.Pow (ap[4]  , cp[4]);
                                        cp[5] =  (float)Math.Pow (ap[5]  , cp[5]);
                                        cp[6] =  (float)Math.Pow (ap[6]  , cp[6]);
                                        cp[7] =  (float)Math.Pow (ap[7]  , cp[7]);
                                        cp[8] =  (float)Math.Pow (ap[8]  , cp[8]);
                                        cp[9] =  (float)Math.Pow (ap[9]  , cp[9]);
                                        cp[10] =  (float)Math.Pow (ap[10]  , cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  (float)Math.Pow (*ap++  , *cp);
                                        cp++; 
                                    }
                                }
                            } else {
                                // dim == 1
                                cp = (float*)range.Item4;
                                ap = (float*)range.Item2;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    float val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  (float)Math.Pow (val  , cp[0]);
                                        cp[1] =  (float)Math.Pow (val  , cp[1]);
                                        cp[2] =  (float)Math.Pow (val  , cp[2]);
                                        cp[3] =  (float)Math.Pow (val  , cp[3]);
                                        cp[4] =  (float)Math.Pow (val  , cp[4]);
                                        cp[5] =  (float)Math.Pow (val  , cp[5]);
                                        cp[6] =  (float)Math.Pow (val  , cp[6]);
                                        cp[7] =  (float)Math.Pow (val  , cp[7]);
                                        cp[8] =  (float)Math.Pow (val  , cp[8]);
                                        cp[9] =  (float)Math.Pow (val  , cp[9]);
                                        cp[10] =  (float)Math.Pow (val  , cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  (float)Math.Pow (val  , *cp);
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            if (dim == 0) {
                                ap = (float*)range.Item2;
                                cp = (float*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (float*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  (float)Math.Pow (ap[0]  , bp[0]);
                                        cp[1] =  (float)Math.Pow (ap[1]  , bp[1]);
                                        cp[2] =  (float)Math.Pow (ap[2]  , bp[2]);
                                        cp[3] =  (float)Math.Pow (ap[3]  , bp[3]);
                                        cp[4] =  (float)Math.Pow (ap[4]  , bp[4]);
                                        cp[5] =  (float)Math.Pow (ap[5]  , bp[5]);
                                        cp[6] =  (float)Math.Pow (ap[6]  , bp[6]);
                                        cp[7] =  (float)Math.Pow (ap[7]  , bp[7]);
                                        cp[8] =  (float)Math.Pow (ap[8]  , bp[8]);
                                        cp[9] =  (float)Math.Pow (ap[9]  , bp[9]);
                                        cp[10] =  (float)Math.Pow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  (float)Math.Pow (*ap  , *bp);
                                        ap++;
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                ap = (float*)range.Item2;
                                bp = (float*)range.Item3;
                                cp = (float*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   float val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  (float)Math.Pow (ap[0]  , val);
                                        cp[1] =  (float)Math.Pow (ap[1]  , val);
                                        cp[2] =  (float)Math.Pow (ap[2]  , val);
                                        cp[3] =  (float)Math.Pow (ap[3]  , val);
                                        cp[4] =  (float)Math.Pow (ap[4]  , val);
                                        cp[5] =  (float)Math.Pow (ap[5]  , val);
                                        cp[6] =  (float)Math.Pow (ap[6]  , val);
                                        cp[7] =  (float)Math.Pow (ap[7]  , val);
                                        cp[8] =  (float)Math.Pow (ap[8]  , val);
                                        cp[9] =  (float)Math.Pow (ap[9]  , val);
                                        cp[10] =  (float)Math.Pow (ap[10]  , val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  (float)Math.Pow (*ap  , val);
                                        ap++;
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            if (dim == 0) {
                                cp = (float*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (float*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  (float)Math.Pow (cp[0]  , bp[0]);
                                        cp[1] =  (float)Math.Pow (cp[1]  , bp[1]);
                                        cp[2] =  (float)Math.Pow (cp[2]  , bp[2]);
                                        cp[3] =  (float)Math.Pow (cp[3]  , bp[3]);
                                        cp[4] =  (float)Math.Pow (cp[4]  , bp[4]);
                                        cp[5] =  (float)Math.Pow (cp[5]  , bp[5]);
                                        cp[6] =  (float)Math.Pow (cp[6]  , bp[6]);
                                        cp[7] =  (float)Math.Pow (cp[7]  , bp[7]);
                                        cp[8] =  (float)Math.Pow (cp[8]  , bp[8]);
                                        cp[9] =  (float)Math.Pow (cp[9]  , bp[9]);
                                        cp[10] =  (float)Math.Pow (cp[10]  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  (float)Math.Pow (*cp  , *bp);
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                bp = (float*)range.Item3;
                                cp = (float*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    float val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  (float)Math.Pow (cp[0]  , val);
                                        cp[1] =  (float)Math.Pow (cp[1]  , val);
                                        cp[2] =  (float)Math.Pow (cp[2]  , val);
                                        cp[3] =  (float)Math.Pow (cp[3]  , val);
                                        cp[4] =  (float)Math.Pow (cp[4]  , val);
                                        cp[5] =  (float)Math.Pow (cp[5]  , val);
                                        cp[6] =  (float)Math.Pow (cp[6]  , val);
                                        cp[7] =  (float)Math.Pow (cp[7]  , val);
                                        cp[8] =  (float)Math.Pow (cp[8]  , val);
                                        cp[9] =  (float)Math.Pow (cp[9]  , val);
                                        cp[10] =  (float)Math.Pow (cp[10]  , val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =  (float)Math.Pow (*cp  , val);
                                        cp++;
                                    }
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                fixed ( float* arrAP = arrA)
                fixed ( float* arrBP = arrB)
                fixed ( float* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                               , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                               , (IntPtr)(retArrP + i * outDims[0] * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                                , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                                , (IntPtr)(retArrP + i * outDims[0] * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<float>(retStorage);
            //}  // no scopes here! it disables implace operations
        }

        /// <summary>Power elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with elementwise result of A <sup>B</sup></returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<fcomplex>  pow(ILInArray<fcomplex> A, ILInArray<fcomplex> B) {
            using (ILScope.Enter(A,B)) {
                int outLen; 
                BinOpItMode mode; 
                fcomplex [] retArr;
                fcomplex [] arrA = A.GetArrayForRead();
                fcomplex[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size; 
                    if (B.IsScalar) {
                       
                        return new  ILRetArray<fcomplex> (new  fcomplex [1]{ fcomplex.Pow (A.GetValue(0)  , B.GetValue(0))}, A.Size);
                    } else if (B.IsEmpty) {
                        return  ILRetArray<fcomplex>.empty(outDims); 
                    } else {
                        outLen = outDims.NumberOfElements; 
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< fcomplex > (outLen);
                            mode = BinOpItMode.SAN; 
                        } else {
                            mode = BinOpItMode.SAI; 
                        }
                    }
                } else {
                    outDims = A.Size; 
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return  ILRetArray<fcomplex>.empty(A.Size);  
                        }
                        outLen = A.S.NumberOfElements; 
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< fcomplex > (outLen);
                            mode = BinOpItMode.ASN; 
                        } else {
                            mode = BinOpItMode.ASI; 
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  powEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements; 
                        if (A.TryGetStorage4InplaceOp(out retArr)) 
                            mode  = BinOpItMode.AAIA; 
                        else if (B.TryGetStorage4InplaceOp(out retArr)) 
                            mode = BinOpItMode.AAIB; 
                        else {
                            retArr = ILMemoryPool.Pool.New< fcomplex > (outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<fcomplex> retStorage = new ILDenseStorage<fcomplex>(retArr, outDims);
                int i = 0, workerCount = 1; 
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    fcomplex* cp = (fcomplex*)range.Item5 + range.Item1;
                    fcomplex scalar;
                    int j = range.Item2; 
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           fcomplex* bp = ((fcomplex*)range.Item4 + range.Item1);
                            while (j > 20) {
                                cp[0] =  fcomplex.Pow (cp[0]  , bp[0]);
                                cp[1] =  fcomplex.Pow (cp[1]  , bp[1]);
                                cp[2] =  fcomplex.Pow (cp[2]  , bp[2]);
                                cp[3] =  fcomplex.Pow (cp[3]  , bp[3]);
                                cp[4] =  fcomplex.Pow (cp[4]  , bp[4]);
                                cp[5] =  fcomplex.Pow (cp[5]  , bp[5]);
                                cp[6] =  fcomplex.Pow (cp[6]  , bp[6]);
                                cp[7] =  fcomplex.Pow (cp[7]  , bp[7]);
                                cp[8] =  fcomplex.Pow (cp[8]  , bp[8]);
                                cp[9] =  fcomplex.Pow (cp[9]  , bp[9]);
                                cp[10] =  fcomplex.Pow (cp[10]  , bp[10]);
                                cp[11] =  fcomplex.Pow (cp[11]  , bp[11]);
                                cp[12] =  fcomplex.Pow (cp[12]  , bp[12]);
                                cp[13] =  fcomplex.Pow (cp[13]  , bp[13]);
                                cp[14] =  fcomplex.Pow (cp[14]  , bp[14]);
                                cp[15] =  fcomplex.Pow (cp[15]  , bp[15]);
                                cp[16] =  fcomplex.Pow (cp[16]  , bp[16]);
                                cp[17] =  fcomplex.Pow (cp[17]  , bp[17]);
                                cp[18] =  fcomplex.Pow (cp[18]  , bp[18]);
                                cp[19] =  fcomplex.Pow (cp[19]  , bp[19]);
                                cp[20] =  fcomplex.Pow (cp[20]  , bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  fcomplex.Pow (*cp  , *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  fcomplex.Pow (ap[0]  , cp[0]);
                                cp[1] =  fcomplex.Pow (ap[1]  , cp[1]);
                                cp[2] =  fcomplex.Pow (ap[2]  , cp[2]);
                                cp[3] =  fcomplex.Pow (ap[3]  , cp[3]);
                                cp[4] =  fcomplex.Pow (ap[4]  , cp[4]);
                                cp[5] =  fcomplex.Pow (ap[5]  , cp[5]);
                                cp[6] =  fcomplex.Pow (ap[6]  , cp[6]);
                                cp[7] =  fcomplex.Pow (ap[7]  , cp[7]);
                                cp[8] =  fcomplex.Pow (ap[8]  , cp[8]);
                                cp[9] =  fcomplex.Pow (ap[9]  , cp[9]);
                                cp[10] =  fcomplex.Pow (ap[10]  , cp[10]);
                                cp[11] =  fcomplex.Pow (ap[11]  , cp[11]);
                                cp[12] =  fcomplex.Pow (ap[12]  , cp[12]);
                                cp[13] =  fcomplex.Pow (ap[13]  , cp[13]);
                                cp[14] =  fcomplex.Pow (ap[14]  , cp[14]);
                                cp[15] =  fcomplex.Pow (ap[15]  , cp[15]);
                                cp[16] =  fcomplex.Pow (ap[16]  , cp[16]);
                                cp[17] =  fcomplex.Pow (ap[17]  , cp[17]);
                                cp[18] =  fcomplex.Pow (ap[18]  , cp[18]);
                                cp[19] =  fcomplex.Pow (ap[19]  , cp[19]);
                                cp[20] =  fcomplex.Pow (ap[20]  , cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  fcomplex.Pow (*ap  , *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((fcomplex*)range.Item3 + range.Item1);
                            bp = ((fcomplex*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  fcomplex.Pow (ap[0]  , bp[0]);
                                cp[1] =  fcomplex.Pow (ap[1]  , bp[1]);
                                cp[2] =  fcomplex.Pow (ap[2]  , bp[2]);
                                cp[3] =  fcomplex.Pow (ap[3]  , bp[3]);
                                cp[4] =  fcomplex.Pow (ap[4]  , bp[4]);
                                cp[5] =  fcomplex.Pow (ap[5]  , bp[5]);
                                cp[6] =  fcomplex.Pow (ap[6]  , bp[6]);
                                cp[7] =  fcomplex.Pow (ap[7]  , bp[7]);
                                cp[8] =  fcomplex.Pow (ap[8]  , bp[8]);
                                cp[9] =  fcomplex.Pow (ap[9]  , bp[9]);
                                cp[10] =  fcomplex.Pow (ap[10]  , bp[10]);
                                cp[11] =  fcomplex.Pow (ap[11]  , bp[11]);
                                cp[12] =  fcomplex.Pow (ap[12]  , bp[12]);
                                cp[13] =  fcomplex.Pow (ap[13]  , bp[13]);
                                cp[14] =  fcomplex.Pow (ap[14]  , bp[14]);
                                cp[15] =  fcomplex.Pow (ap[15]  , bp[15]);
                                cp[16] =  fcomplex.Pow (ap[16]  , bp[16]);
                                cp[17] =  fcomplex.Pow (ap[17]  , bp[17]);
                                cp[18] =  fcomplex.Pow (ap[18]  , bp[18]);
                                cp[19] =  fcomplex.Pow (ap[19]  , bp[19]);
                                cp[20] =  fcomplex.Pow (ap[20]  , bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =  fcomplex.Pow (*ap  , *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((fcomplex*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  fcomplex.Pow (cp[0]  , scalar);
                                cp[1] =  fcomplex.Pow (cp[1]  , scalar);
                                cp[2] =  fcomplex.Pow (cp[2]  , scalar);
                                cp[3] =  fcomplex.Pow (cp[3]  , scalar);
                                cp[4] =  fcomplex.Pow (cp[4]  , scalar);
                                cp[5] =  fcomplex.Pow (cp[5]  , scalar);
                                cp[6] =  fcomplex.Pow (cp[6]  , scalar);
                                cp[7] =  fcomplex.Pow (cp[7]  , scalar);
                                cp[8] =  fcomplex.Pow (cp[8]  , scalar);
                                cp[9] =  fcomplex.Pow (cp[9]  , scalar);
                                cp[10] =  fcomplex.Pow (cp[10]  , scalar);
                                cp[11] =  fcomplex.Pow (cp[11]  , scalar);
                                cp[12] =  fcomplex.Pow (cp[12]  , scalar);
                                cp[13] =  fcomplex.Pow (cp[13]  , scalar);
                                cp[14] =  fcomplex.Pow (cp[14]  , scalar);
                                cp[15] =  fcomplex.Pow (cp[15]  , scalar);
                                cp[16] =  fcomplex.Pow (cp[16]  , scalar);
                                cp[17] =  fcomplex.Pow (cp[17]  , scalar);
                                cp[18] =  fcomplex.Pow (cp[18]  , scalar);
                                cp[19] =  fcomplex.Pow (cp[19]  , scalar);
                                cp[20] =  fcomplex.Pow (cp[20]  , scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  fcomplex.Pow (*cp  , scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((fcomplex*)range.Item3 + range.Item1);
                            scalar = *((fcomplex*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  fcomplex.Pow (ap[0]  , scalar);
                                cp[1] =  fcomplex.Pow (ap[1]  , scalar);
                                cp[2] =  fcomplex.Pow (ap[2]  , scalar);
                                cp[3] =  fcomplex.Pow (ap[3]  , scalar);
                                cp[4] =  fcomplex.Pow (ap[4]  , scalar);
                                cp[5] =  fcomplex.Pow (ap[5]  , scalar);
                                cp[6] =  fcomplex.Pow (ap[6]  , scalar);
                                cp[7] =  fcomplex.Pow (ap[7]  , scalar);
                                cp[8] =  fcomplex.Pow (ap[8]  , scalar);
                                cp[9] =  fcomplex.Pow (ap[9]  , scalar);
                                cp[10] =  fcomplex.Pow (ap[10]  , scalar);
                                cp[11] =  fcomplex.Pow (ap[11]  , scalar);
                                cp[12] =  fcomplex.Pow (ap[12]  , scalar);
                                cp[13] =  fcomplex.Pow (ap[13]  , scalar);
                                cp[14] =  fcomplex.Pow (ap[14]  , scalar);
                                cp[15] =  fcomplex.Pow (ap[15]  , scalar);
                                cp[16] =  fcomplex.Pow (ap[16]  , scalar);
                                cp[17] =  fcomplex.Pow (ap[17]  , scalar);
                                cp[18] =  fcomplex.Pow (ap[18]  , scalar);
                                cp[19] =  fcomplex.Pow (ap[19]  , scalar);
                                cp[20] =  fcomplex.Pow (ap[20]  , scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  fcomplex.Pow (*ap  , scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((fcomplex*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =  fcomplex.Pow (scalar  , cp[0]);
                                cp[1] =  fcomplex.Pow (scalar  , cp[1]);
                                cp[2] =  fcomplex.Pow (scalar  , cp[2]);
                                cp[3] =  fcomplex.Pow (scalar  , cp[3]);
                                cp[4] =  fcomplex.Pow (scalar  , cp[4]);
                                cp[5] =  fcomplex.Pow (scalar  , cp[5]);
                                cp[6] =  fcomplex.Pow (scalar  , cp[6]);
                                cp[7] =  fcomplex.Pow (scalar  , cp[7]);
                                cp[8] =  fcomplex.Pow (scalar  , cp[8]);
                                cp[9] =  fcomplex.Pow (scalar  , cp[9]);
                                cp[10] =  fcomplex.Pow (scalar  , cp[10]);
                                cp[11] =  fcomplex.Pow (scalar  , cp[11]);
                                cp[12] =  fcomplex.Pow (scalar  , cp[12]);
                                cp[13] =  fcomplex.Pow (scalar  , cp[13]);
                                cp[14] =  fcomplex.Pow (scalar  , cp[14]);
                                cp[15] =  fcomplex.Pow (scalar  , cp[15]);
                                cp[16] =  fcomplex.Pow (scalar  , cp[16]);
                                cp[17] =  fcomplex.Pow (scalar  , cp[17]);
                                cp[18] =  fcomplex.Pow (scalar  , cp[18]);
                                cp[19] =  fcomplex.Pow (scalar  , cp[19]);
                                cp[20] =  fcomplex.Pow (scalar  , cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  fcomplex.Pow (scalar  , *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((fcomplex*)range.Item3);
                            bp = ((fcomplex*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  fcomplex.Pow (scalar  , bp[0]);
                                cp[1] =  fcomplex.Pow (scalar  , bp[1]);
                                cp[2] =  fcomplex.Pow (scalar  , bp[2]);
                                cp[3] =  fcomplex.Pow (scalar  , bp[3]);
                                cp[4] =  fcomplex.Pow (scalar  , bp[4]);
                                cp[5] =  fcomplex.Pow (scalar  , bp[5]);
                                cp[6] =  fcomplex.Pow (scalar  , bp[6]);
                                cp[7] =  fcomplex.Pow (scalar  , bp[7]);
                                cp[8] =  fcomplex.Pow (scalar  , bp[8]);
                                cp[9] =  fcomplex.Pow (scalar  , bp[9]);
                                cp[10] =  fcomplex.Pow (scalar  , bp[10]);
                                cp[11] =  fcomplex.Pow (scalar  , bp[11]);
                                cp[12] =  fcomplex.Pow (scalar  , bp[12]);
                                cp[13] =  fcomplex.Pow (scalar  , bp[13]);
                                cp[14] =  fcomplex.Pow (scalar  , bp[14]);
                                cp[15] =  fcomplex.Pow (scalar  , bp[15]);
                                cp[16] =  fcomplex.Pow (scalar  , bp[16]);
                                cp[17] =  fcomplex.Pow (scalar  , bp[17]);
                                cp[18] =  fcomplex.Pow (scalar  , bp[18]);
                                cp[19] =  fcomplex.Pow (scalar  , bp[19]);
                                cp[20] =  fcomplex.Pow (scalar  , bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  fcomplex.Pow (scalar  , *bp);
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);

                };

                #region do the work 
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
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
                
                fixed ( fcomplex* arrAP = arrA)
                fixed ( fcomplex* arrBP = arrB)
                fixed ( fcomplex* retArrP = retArr) {

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
                return new  ILRetArray< fcomplex>(retStorage);
            }
        }

        private static unsafe ILRetArray<fcomplex>  powEx(ILInArray<fcomplex> A, ILInArray<fcomplex> B) {
            //using (ILScope.Enter(A, B)) { we cannot start a new scope here, since this would prevent A and B to be used implace if applicable

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<fcomplex>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<fcomplex>(B.S);
                } else if (B.IsEmpty) {
                    return empty<fcomplex>(A.S);
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
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
               
                fcomplex[] retArr;
               
                fcomplex[] arrA = A.GetArrayForRead();
               
                fcomplex[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int workItemMultiplierLenA;
                int workItemMultiplierLenB;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<fcomplex>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    workItemMultiplierLenB = outDims[0]; 
                    workItemMultiplierLenA = dim;  // 0 for column, 1 for row vector
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<fcomplex>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    workItemMultiplierLenB = dim;  // 0 for column, 1 for row vector
                    workItemMultiplierLenA = outDims[0]; 
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in either A or B");
                }
                int itLen = outDims[0]; // (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                #endregion

                #region worker loops definition
                ILDenseStorage<fcomplex> retStorage = new ILDenseStorage<fcomplex>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;
                   
                    fcomplex* ap;
                   
                    fcomplex* bp;
                   
                    fcomplex* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            if (dim == 0) {
                                bp = (fcomplex*)range.Item3;
                                cp = (fcomplex*)range.Item4; 
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (fcomplex*)range.Item2;
                                    int l = itLen; 
                                    while (l > 10) {
                                        cp[0] =  fcomplex.Pow (ap[0]  , bp[0]);
                                        cp[1] =  fcomplex.Pow (ap[1]  , bp[1]);
                                        cp[2] =  fcomplex.Pow (ap[2]  , bp[2]);
                                        cp[3] =  fcomplex.Pow (ap[3]  , bp[3]);
                                        cp[4] =  fcomplex.Pow (ap[4]  , bp[4]);
                                        cp[5] =  fcomplex.Pow (ap[5]  , bp[5]);
                                        cp[6] =  fcomplex.Pow (ap[6]  , bp[6]);
                                        cp[7] =  fcomplex.Pow (ap[7]  , bp[7]);
                                        cp[8] =  fcomplex.Pow (ap[8]  , bp[8]);
                                        cp[9] =  fcomplex.Pow (ap[9]  , bp[9]);
                                        cp[10] =  fcomplex.Pow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  fcomplex.Pow (*ap++  , *bp++);
                                    }
                                }
                            } else {
                                // dim == 1
                                ap = (fcomplex*)range.Item2;
                                bp = (fcomplex*)range.Item3;
                                cp = (fcomplex*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   fcomplex val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  fcomplex.Pow (val  , bp[0]);
                                        cp[1] =  fcomplex.Pow (val  , bp[1]);
                                        cp[2] =  fcomplex.Pow (val  , bp[2]);
                                        cp[3] =  fcomplex.Pow (val  , bp[3]);
                                        cp[4] =  fcomplex.Pow (val  , bp[4]);
                                        cp[5] =  fcomplex.Pow (val  , bp[5]);
                                        cp[6] =  fcomplex.Pow (val  , bp[6]);
                                        cp[7] =  fcomplex.Pow (val  , bp[7]);
                                        cp[8] =  fcomplex.Pow (val  , bp[8]);
                                        cp[9] =  fcomplex.Pow (val  , bp[9]);
                                        cp[10] =  fcomplex.Pow (val  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  fcomplex.Pow (val  , *bp++);
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            if (dim == 0) {
                                cp = (fcomplex*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (fcomplex*)range.Item2;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  fcomplex.Pow (ap[0]  , cp[0]);
                                        cp[1] =  fcomplex.Pow (ap[1]  , cp[1]);
                                        cp[2] =  fcomplex.Pow (ap[2]  , cp[2]);
                                        cp[3] =  fcomplex.Pow (ap[3]  , cp[3]);
                                        cp[4] =  fcomplex.Pow (ap[4]  , cp[4]);
                                        cp[5] =  fcomplex.Pow (ap[5]  , cp[5]);
                                        cp[6] =  fcomplex.Pow (ap[6]  , cp[6]);
                                        cp[7] =  fcomplex.Pow (ap[7]  , cp[7]);
                                        cp[8] =  fcomplex.Pow (ap[8]  , cp[8]);
                                        cp[9] =  fcomplex.Pow (ap[9]  , cp[9]);
                                        cp[10] =  fcomplex.Pow (ap[10]  , cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  fcomplex.Pow (*ap++  , *cp);
                                        cp++; 
                                    }
                                }
                            } else {
                                // dim == 1
                                cp = (fcomplex*)range.Item4;
                                ap = (fcomplex*)range.Item2;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    fcomplex val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  fcomplex.Pow (val  , cp[0]);
                                        cp[1] =  fcomplex.Pow (val  , cp[1]);
                                        cp[2] =  fcomplex.Pow (val  , cp[2]);
                                        cp[3] =  fcomplex.Pow (val  , cp[3]);
                                        cp[4] =  fcomplex.Pow (val  , cp[4]);
                                        cp[5] =  fcomplex.Pow (val  , cp[5]);
                                        cp[6] =  fcomplex.Pow (val  , cp[6]);
                                        cp[7] =  fcomplex.Pow (val  , cp[7]);
                                        cp[8] =  fcomplex.Pow (val  , cp[8]);
                                        cp[9] =  fcomplex.Pow (val  , cp[9]);
                                        cp[10] =  fcomplex.Pow (val  , cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  fcomplex.Pow (val  , *cp);
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            if (dim == 0) {
                                ap = (fcomplex*)range.Item2;
                                cp = (fcomplex*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (fcomplex*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  fcomplex.Pow (ap[0]  , bp[0]);
                                        cp[1] =  fcomplex.Pow (ap[1]  , bp[1]);
                                        cp[2] =  fcomplex.Pow (ap[2]  , bp[2]);
                                        cp[3] =  fcomplex.Pow (ap[3]  , bp[3]);
                                        cp[4] =  fcomplex.Pow (ap[4]  , bp[4]);
                                        cp[5] =  fcomplex.Pow (ap[5]  , bp[5]);
                                        cp[6] =  fcomplex.Pow (ap[6]  , bp[6]);
                                        cp[7] =  fcomplex.Pow (ap[7]  , bp[7]);
                                        cp[8] =  fcomplex.Pow (ap[8]  , bp[8]);
                                        cp[9] =  fcomplex.Pow (ap[9]  , bp[9]);
                                        cp[10] =  fcomplex.Pow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  fcomplex.Pow (*ap  , *bp);
                                        ap++;
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                ap = (fcomplex*)range.Item2;
                                bp = (fcomplex*)range.Item3;
                                cp = (fcomplex*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   fcomplex val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  fcomplex.Pow (ap[0]  , val);
                                        cp[1] =  fcomplex.Pow (ap[1]  , val);
                                        cp[2] =  fcomplex.Pow (ap[2]  , val);
                                        cp[3] =  fcomplex.Pow (ap[3]  , val);
                                        cp[4] =  fcomplex.Pow (ap[4]  , val);
                                        cp[5] =  fcomplex.Pow (ap[5]  , val);
                                        cp[6] =  fcomplex.Pow (ap[6]  , val);
                                        cp[7] =  fcomplex.Pow (ap[7]  , val);
                                        cp[8] =  fcomplex.Pow (ap[8]  , val);
                                        cp[9] =  fcomplex.Pow (ap[9]  , val);
                                        cp[10] =  fcomplex.Pow (ap[10]  , val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  fcomplex.Pow (*ap  , val);
                                        ap++;
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            if (dim == 0) {
                                cp = (fcomplex*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (fcomplex*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  fcomplex.Pow (cp[0]  , bp[0]);
                                        cp[1] =  fcomplex.Pow (cp[1]  , bp[1]);
                                        cp[2] =  fcomplex.Pow (cp[2]  , bp[2]);
                                        cp[3] =  fcomplex.Pow (cp[3]  , bp[3]);
                                        cp[4] =  fcomplex.Pow (cp[4]  , bp[4]);
                                        cp[5] =  fcomplex.Pow (cp[5]  , bp[5]);
                                        cp[6] =  fcomplex.Pow (cp[6]  , bp[6]);
                                        cp[7] =  fcomplex.Pow (cp[7]  , bp[7]);
                                        cp[8] =  fcomplex.Pow (cp[8]  , bp[8]);
                                        cp[9] =  fcomplex.Pow (cp[9]  , bp[9]);
                                        cp[10] =  fcomplex.Pow (cp[10]  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  fcomplex.Pow (*cp  , *bp);
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                bp = (fcomplex*)range.Item3;
                                cp = (fcomplex*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    fcomplex val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  fcomplex.Pow (cp[0]  , val);
                                        cp[1] =  fcomplex.Pow (cp[1]  , val);
                                        cp[2] =  fcomplex.Pow (cp[2]  , val);
                                        cp[3] =  fcomplex.Pow (cp[3]  , val);
                                        cp[4] =  fcomplex.Pow (cp[4]  , val);
                                        cp[5] =  fcomplex.Pow (cp[5]  , val);
                                        cp[6] =  fcomplex.Pow (cp[6]  , val);
                                        cp[7] =  fcomplex.Pow (cp[7]  , val);
                                        cp[8] =  fcomplex.Pow (cp[8]  , val);
                                        cp[9] =  fcomplex.Pow (cp[9]  , val);
                                        cp[10] =  fcomplex.Pow (cp[10]  , val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =  fcomplex.Pow (*cp  , val);
                                        cp++;
                                    }
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                fixed ( fcomplex* arrAP = arrA)
                fixed ( fcomplex* arrBP = arrB)
                fixed ( fcomplex* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                               , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                               , (IntPtr)(retArrP + i * outDims[0] * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                                , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                                , (IntPtr)(retArrP + i * outDims[0] * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<fcomplex>(retStorage);
            //}  // no scopes here! it disables implace operations
        }

        /// <summary>Power elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with elementwise result of A <sup>B</sup></returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<complex>  pow(ILInArray<complex> A, ILInArray<complex> B) {
            using (ILScope.Enter(A,B)) {
                int outLen; 
                BinOpItMode mode; 
                complex [] retArr;
                complex [] arrA = A.GetArrayForRead();
                complex[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size; 
                    if (B.IsScalar) {
                       
                        return new  ILRetArray<complex> (new  complex [1]{ complex.Pow (A.GetValue(0)  , B.GetValue(0))}, A.Size);
                    } else if (B.IsEmpty) {
                        return  ILRetArray<complex>.empty(outDims); 
                    } else {
                        outLen = outDims.NumberOfElements; 
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< complex > (outLen);
                            mode = BinOpItMode.SAN; 
                        } else {
                            mode = BinOpItMode.SAI; 
                        }
                    }
                } else {
                    outDims = A.Size; 
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return  ILRetArray<complex>.empty(A.Size);  
                        }
                        outLen = A.S.NumberOfElements; 
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< complex > (outLen);
                            mode = BinOpItMode.ASN; 
                        } else {
                            mode = BinOpItMode.ASI; 
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  powEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements; 
                        if (A.TryGetStorage4InplaceOp(out retArr)) 
                            mode  = BinOpItMode.AAIA; 
                        else if (B.TryGetStorage4InplaceOp(out retArr)) 
                            mode = BinOpItMode.AAIB; 
                        else {
                            retArr = ILMemoryPool.Pool.New< complex > (outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<complex> retStorage = new ILDenseStorage<complex>(retArr, outDims);
                int i = 0, workerCount = 1; 
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    complex* cp = (complex*)range.Item5 + range.Item1;
                    complex scalar;
                    int j = range.Item2; 
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           complex* bp = ((complex*)range.Item4 + range.Item1);
                            while (j > 20) {
                                cp[0] =  complex.Pow (cp[0]  , bp[0]);
                                cp[1] =  complex.Pow (cp[1]  , bp[1]);
                                cp[2] =  complex.Pow (cp[2]  , bp[2]);
                                cp[3] =  complex.Pow (cp[3]  , bp[3]);
                                cp[4] =  complex.Pow (cp[4]  , bp[4]);
                                cp[5] =  complex.Pow (cp[5]  , bp[5]);
                                cp[6] =  complex.Pow (cp[6]  , bp[6]);
                                cp[7] =  complex.Pow (cp[7]  , bp[7]);
                                cp[8] =  complex.Pow (cp[8]  , bp[8]);
                                cp[9] =  complex.Pow (cp[9]  , bp[9]);
                                cp[10] =  complex.Pow (cp[10]  , bp[10]);
                                cp[11] =  complex.Pow (cp[11]  , bp[11]);
                                cp[12] =  complex.Pow (cp[12]  , bp[12]);
                                cp[13] =  complex.Pow (cp[13]  , bp[13]);
                                cp[14] =  complex.Pow (cp[14]  , bp[14]);
                                cp[15] =  complex.Pow (cp[15]  , bp[15]);
                                cp[16] =  complex.Pow (cp[16]  , bp[16]);
                                cp[17] =  complex.Pow (cp[17]  , bp[17]);
                                cp[18] =  complex.Pow (cp[18]  , bp[18]);
                                cp[19] =  complex.Pow (cp[19]  , bp[19]);
                                cp[20] =  complex.Pow (cp[20]  , bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  complex.Pow (*cp  , *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           complex* ap = ((complex*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  complex.Pow (ap[0]  , cp[0]);
                                cp[1] =  complex.Pow (ap[1]  , cp[1]);
                                cp[2] =  complex.Pow (ap[2]  , cp[2]);
                                cp[3] =  complex.Pow (ap[3]  , cp[3]);
                                cp[4] =  complex.Pow (ap[4]  , cp[4]);
                                cp[5] =  complex.Pow (ap[5]  , cp[5]);
                                cp[6] =  complex.Pow (ap[6]  , cp[6]);
                                cp[7] =  complex.Pow (ap[7]  , cp[7]);
                                cp[8] =  complex.Pow (ap[8]  , cp[8]);
                                cp[9] =  complex.Pow (ap[9]  , cp[9]);
                                cp[10] =  complex.Pow (ap[10]  , cp[10]);
                                cp[11] =  complex.Pow (ap[11]  , cp[11]);
                                cp[12] =  complex.Pow (ap[12]  , cp[12]);
                                cp[13] =  complex.Pow (ap[13]  , cp[13]);
                                cp[14] =  complex.Pow (ap[14]  , cp[14]);
                                cp[15] =  complex.Pow (ap[15]  , cp[15]);
                                cp[16] =  complex.Pow (ap[16]  , cp[16]);
                                cp[17] =  complex.Pow (ap[17]  , cp[17]);
                                cp[18] =  complex.Pow (ap[18]  , cp[18]);
                                cp[19] =  complex.Pow (ap[19]  , cp[19]);
                                cp[20] =  complex.Pow (ap[20]  , cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  complex.Pow (*ap  , *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((complex*)range.Item3 + range.Item1);
                            bp = ((complex*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  complex.Pow (ap[0]  , bp[0]);
                                cp[1] =  complex.Pow (ap[1]  , bp[1]);
                                cp[2] =  complex.Pow (ap[2]  , bp[2]);
                                cp[3] =  complex.Pow (ap[3]  , bp[3]);
                                cp[4] =  complex.Pow (ap[4]  , bp[4]);
                                cp[5] =  complex.Pow (ap[5]  , bp[5]);
                                cp[6] =  complex.Pow (ap[6]  , bp[6]);
                                cp[7] =  complex.Pow (ap[7]  , bp[7]);
                                cp[8] =  complex.Pow (ap[8]  , bp[8]);
                                cp[9] =  complex.Pow (ap[9]  , bp[9]);
                                cp[10] =  complex.Pow (ap[10]  , bp[10]);
                                cp[11] =  complex.Pow (ap[11]  , bp[11]);
                                cp[12] =  complex.Pow (ap[12]  , bp[12]);
                                cp[13] =  complex.Pow (ap[13]  , bp[13]);
                                cp[14] =  complex.Pow (ap[14]  , bp[14]);
                                cp[15] =  complex.Pow (ap[15]  , bp[15]);
                                cp[16] =  complex.Pow (ap[16]  , bp[16]);
                                cp[17] =  complex.Pow (ap[17]  , bp[17]);
                                cp[18] =  complex.Pow (ap[18]  , bp[18]);
                                cp[19] =  complex.Pow (ap[19]  , bp[19]);
                                cp[20] =  complex.Pow (ap[20]  , bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =  complex.Pow (*ap  , *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((complex*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  complex.Pow (cp[0]  , scalar);
                                cp[1] =  complex.Pow (cp[1]  , scalar);
                                cp[2] =  complex.Pow (cp[2]  , scalar);
                                cp[3] =  complex.Pow (cp[3]  , scalar);
                                cp[4] =  complex.Pow (cp[4]  , scalar);
                                cp[5] =  complex.Pow (cp[5]  , scalar);
                                cp[6] =  complex.Pow (cp[6]  , scalar);
                                cp[7] =  complex.Pow (cp[7]  , scalar);
                                cp[8] =  complex.Pow (cp[8]  , scalar);
                                cp[9] =  complex.Pow (cp[9]  , scalar);
                                cp[10] =  complex.Pow (cp[10]  , scalar);
                                cp[11] =  complex.Pow (cp[11]  , scalar);
                                cp[12] =  complex.Pow (cp[12]  , scalar);
                                cp[13] =  complex.Pow (cp[13]  , scalar);
                                cp[14] =  complex.Pow (cp[14]  , scalar);
                                cp[15] =  complex.Pow (cp[15]  , scalar);
                                cp[16] =  complex.Pow (cp[16]  , scalar);
                                cp[17] =  complex.Pow (cp[17]  , scalar);
                                cp[18] =  complex.Pow (cp[18]  , scalar);
                                cp[19] =  complex.Pow (cp[19]  , scalar);
                                cp[20] =  complex.Pow (cp[20]  , scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  complex.Pow (*cp  , scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((complex*)range.Item3 + range.Item1);
                            scalar = *((complex*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  complex.Pow (ap[0]  , scalar);
                                cp[1] =  complex.Pow (ap[1]  , scalar);
                                cp[2] =  complex.Pow (ap[2]  , scalar);
                                cp[3] =  complex.Pow (ap[3]  , scalar);
                                cp[4] =  complex.Pow (ap[4]  , scalar);
                                cp[5] =  complex.Pow (ap[5]  , scalar);
                                cp[6] =  complex.Pow (ap[6]  , scalar);
                                cp[7] =  complex.Pow (ap[7]  , scalar);
                                cp[8] =  complex.Pow (ap[8]  , scalar);
                                cp[9] =  complex.Pow (ap[9]  , scalar);
                                cp[10] =  complex.Pow (ap[10]  , scalar);
                                cp[11] =  complex.Pow (ap[11]  , scalar);
                                cp[12] =  complex.Pow (ap[12]  , scalar);
                                cp[13] =  complex.Pow (ap[13]  , scalar);
                                cp[14] =  complex.Pow (ap[14]  , scalar);
                                cp[15] =  complex.Pow (ap[15]  , scalar);
                                cp[16] =  complex.Pow (ap[16]  , scalar);
                                cp[17] =  complex.Pow (ap[17]  , scalar);
                                cp[18] =  complex.Pow (ap[18]  , scalar);
                                cp[19] =  complex.Pow (ap[19]  , scalar);
                                cp[20] =  complex.Pow (ap[20]  , scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  complex.Pow (*ap  , scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((complex*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =  complex.Pow (scalar  , cp[0]);
                                cp[1] =  complex.Pow (scalar  , cp[1]);
                                cp[2] =  complex.Pow (scalar  , cp[2]);
                                cp[3] =  complex.Pow (scalar  , cp[3]);
                                cp[4] =  complex.Pow (scalar  , cp[4]);
                                cp[5] =  complex.Pow (scalar  , cp[5]);
                                cp[6] =  complex.Pow (scalar  , cp[6]);
                                cp[7] =  complex.Pow (scalar  , cp[7]);
                                cp[8] =  complex.Pow (scalar  , cp[8]);
                                cp[9] =  complex.Pow (scalar  , cp[9]);
                                cp[10] =  complex.Pow (scalar  , cp[10]);
                                cp[11] =  complex.Pow (scalar  , cp[11]);
                                cp[12] =  complex.Pow (scalar  , cp[12]);
                                cp[13] =  complex.Pow (scalar  , cp[13]);
                                cp[14] =  complex.Pow (scalar  , cp[14]);
                                cp[15] =  complex.Pow (scalar  , cp[15]);
                                cp[16] =  complex.Pow (scalar  , cp[16]);
                                cp[17] =  complex.Pow (scalar  , cp[17]);
                                cp[18] =  complex.Pow (scalar  , cp[18]);
                                cp[19] =  complex.Pow (scalar  , cp[19]);
                                cp[20] =  complex.Pow (scalar  , cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  complex.Pow (scalar  , *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((complex*)range.Item3);
                            bp = ((complex*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  complex.Pow (scalar  , bp[0]);
                                cp[1] =  complex.Pow (scalar  , bp[1]);
                                cp[2] =  complex.Pow (scalar  , bp[2]);
                                cp[3] =  complex.Pow (scalar  , bp[3]);
                                cp[4] =  complex.Pow (scalar  , bp[4]);
                                cp[5] =  complex.Pow (scalar  , bp[5]);
                                cp[6] =  complex.Pow (scalar  , bp[6]);
                                cp[7] =  complex.Pow (scalar  , bp[7]);
                                cp[8] =  complex.Pow (scalar  , bp[8]);
                                cp[9] =  complex.Pow (scalar  , bp[9]);
                                cp[10] =  complex.Pow (scalar  , bp[10]);
                                cp[11] =  complex.Pow (scalar  , bp[11]);
                                cp[12] =  complex.Pow (scalar  , bp[12]);
                                cp[13] =  complex.Pow (scalar  , bp[13]);
                                cp[14] =  complex.Pow (scalar  , bp[14]);
                                cp[15] =  complex.Pow (scalar  , bp[15]);
                                cp[16] =  complex.Pow (scalar  , bp[16]);
                                cp[17] =  complex.Pow (scalar  , bp[17]);
                                cp[18] =  complex.Pow (scalar  , bp[18]);
                                cp[19] =  complex.Pow (scalar  , bp[19]);
                                cp[20] =  complex.Pow (scalar  , bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  complex.Pow (scalar  , *bp);
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);

                };

                #region do the work 
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
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
                
                fixed ( complex* arrAP = arrA)
                fixed ( complex* arrBP = arrB)
                fixed ( complex* retArrP = retArr) {

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
                return new  ILRetArray< complex>(retStorage);
            }
        }

        private static unsafe ILRetArray<complex>  powEx(ILInArray<complex> A, ILInArray<complex> B) {
            //using (ILScope.Enter(A, B)) { we cannot start a new scope here, since this would prevent A and B to be used implace if applicable

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<complex>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<complex>(B.S);
                } else if (B.IsEmpty) {
                    return empty<complex>(A.S);
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
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
               
                complex[] retArr;
               
                complex[] arrA = A.GetArrayForRead();
               
                complex[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int workItemMultiplierLenA;
                int workItemMultiplierLenB;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<complex>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    workItemMultiplierLenB = outDims[0]; 
                    workItemMultiplierLenA = dim;  // 0 for column, 1 for row vector
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<complex>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    workItemMultiplierLenB = dim;  // 0 for column, 1 for row vector
                    workItemMultiplierLenA = outDims[0]; 
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in either A or B");
                }
                int itLen = outDims[0]; // (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                #endregion

                #region worker loops definition
                ILDenseStorage<complex> retStorage = new ILDenseStorage<complex>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;
                   
                    complex* ap;
                   
                    complex* bp;
                   
                    complex* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            if (dim == 0) {
                                bp = (complex*)range.Item3;
                                cp = (complex*)range.Item4; 
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (complex*)range.Item2;
                                    int l = itLen; 
                                    while (l > 10) {
                                        cp[0] =  complex.Pow (ap[0]  , bp[0]);
                                        cp[1] =  complex.Pow (ap[1]  , bp[1]);
                                        cp[2] =  complex.Pow (ap[2]  , bp[2]);
                                        cp[3] =  complex.Pow (ap[3]  , bp[3]);
                                        cp[4] =  complex.Pow (ap[4]  , bp[4]);
                                        cp[5] =  complex.Pow (ap[5]  , bp[5]);
                                        cp[6] =  complex.Pow (ap[6]  , bp[6]);
                                        cp[7] =  complex.Pow (ap[7]  , bp[7]);
                                        cp[8] =  complex.Pow (ap[8]  , bp[8]);
                                        cp[9] =  complex.Pow (ap[9]  , bp[9]);
                                        cp[10] =  complex.Pow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  complex.Pow (*ap++  , *bp++);
                                    }
                                }
                            } else {
                                // dim == 1
                                ap = (complex*)range.Item2;
                                bp = (complex*)range.Item3;
                                cp = (complex*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   complex val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  complex.Pow (val  , bp[0]);
                                        cp[1] =  complex.Pow (val  , bp[1]);
                                        cp[2] =  complex.Pow (val  , bp[2]);
                                        cp[3] =  complex.Pow (val  , bp[3]);
                                        cp[4] =  complex.Pow (val  , bp[4]);
                                        cp[5] =  complex.Pow (val  , bp[5]);
                                        cp[6] =  complex.Pow (val  , bp[6]);
                                        cp[7] =  complex.Pow (val  , bp[7]);
                                        cp[8] =  complex.Pow (val  , bp[8]);
                                        cp[9] =  complex.Pow (val  , bp[9]);
                                        cp[10] =  complex.Pow (val  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  complex.Pow (val  , *bp++);
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            if (dim == 0) {
                                cp = (complex*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (complex*)range.Item2;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  complex.Pow (ap[0]  , cp[0]);
                                        cp[1] =  complex.Pow (ap[1]  , cp[1]);
                                        cp[2] =  complex.Pow (ap[2]  , cp[2]);
                                        cp[3] =  complex.Pow (ap[3]  , cp[3]);
                                        cp[4] =  complex.Pow (ap[4]  , cp[4]);
                                        cp[5] =  complex.Pow (ap[5]  , cp[5]);
                                        cp[6] =  complex.Pow (ap[6]  , cp[6]);
                                        cp[7] =  complex.Pow (ap[7]  , cp[7]);
                                        cp[8] =  complex.Pow (ap[8]  , cp[8]);
                                        cp[9] =  complex.Pow (ap[9]  , cp[9]);
                                        cp[10] =  complex.Pow (ap[10]  , cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  complex.Pow (*ap++  , *cp);
                                        cp++; 
                                    }
                                }
                            } else {
                                // dim == 1
                                cp = (complex*)range.Item4;
                                ap = (complex*)range.Item2;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    complex val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  complex.Pow (val  , cp[0]);
                                        cp[1] =  complex.Pow (val  , cp[1]);
                                        cp[2] =  complex.Pow (val  , cp[2]);
                                        cp[3] =  complex.Pow (val  , cp[3]);
                                        cp[4] =  complex.Pow (val  , cp[4]);
                                        cp[5] =  complex.Pow (val  , cp[5]);
                                        cp[6] =  complex.Pow (val  , cp[6]);
                                        cp[7] =  complex.Pow (val  , cp[7]);
                                        cp[8] =  complex.Pow (val  , cp[8]);
                                        cp[9] =  complex.Pow (val  , cp[9]);
                                        cp[10] =  complex.Pow (val  , cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  complex.Pow (val  , *cp);
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            if (dim == 0) {
                                ap = (complex*)range.Item2;
                                cp = (complex*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (complex*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  complex.Pow (ap[0]  , bp[0]);
                                        cp[1] =  complex.Pow (ap[1]  , bp[1]);
                                        cp[2] =  complex.Pow (ap[2]  , bp[2]);
                                        cp[3] =  complex.Pow (ap[3]  , bp[3]);
                                        cp[4] =  complex.Pow (ap[4]  , bp[4]);
                                        cp[5] =  complex.Pow (ap[5]  , bp[5]);
                                        cp[6] =  complex.Pow (ap[6]  , bp[6]);
                                        cp[7] =  complex.Pow (ap[7]  , bp[7]);
                                        cp[8] =  complex.Pow (ap[8]  , bp[8]);
                                        cp[9] =  complex.Pow (ap[9]  , bp[9]);
                                        cp[10] =  complex.Pow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  complex.Pow (*ap  , *bp);
                                        ap++;
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                ap = (complex*)range.Item2;
                                bp = (complex*)range.Item3;
                                cp = (complex*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   complex val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  complex.Pow (ap[0]  , val);
                                        cp[1] =  complex.Pow (ap[1]  , val);
                                        cp[2] =  complex.Pow (ap[2]  , val);
                                        cp[3] =  complex.Pow (ap[3]  , val);
                                        cp[4] =  complex.Pow (ap[4]  , val);
                                        cp[5] =  complex.Pow (ap[5]  , val);
                                        cp[6] =  complex.Pow (ap[6]  , val);
                                        cp[7] =  complex.Pow (ap[7]  , val);
                                        cp[8] =  complex.Pow (ap[8]  , val);
                                        cp[9] =  complex.Pow (ap[9]  , val);
                                        cp[10] =  complex.Pow (ap[10]  , val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  complex.Pow (*ap  , val);
                                        ap++;
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            if (dim == 0) {
                                cp = (complex*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (complex*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  complex.Pow (cp[0]  , bp[0]);
                                        cp[1] =  complex.Pow (cp[1]  , bp[1]);
                                        cp[2] =  complex.Pow (cp[2]  , bp[2]);
                                        cp[3] =  complex.Pow (cp[3]  , bp[3]);
                                        cp[4] =  complex.Pow (cp[4]  , bp[4]);
                                        cp[5] =  complex.Pow (cp[5]  , bp[5]);
                                        cp[6] =  complex.Pow (cp[6]  , bp[6]);
                                        cp[7] =  complex.Pow (cp[7]  , bp[7]);
                                        cp[8] =  complex.Pow (cp[8]  , bp[8]);
                                        cp[9] =  complex.Pow (cp[9]  , bp[9]);
                                        cp[10] =  complex.Pow (cp[10]  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  complex.Pow (*cp  , *bp);
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                bp = (complex*)range.Item3;
                                cp = (complex*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    complex val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  complex.Pow (cp[0]  , val);
                                        cp[1] =  complex.Pow (cp[1]  , val);
                                        cp[2] =  complex.Pow (cp[2]  , val);
                                        cp[3] =  complex.Pow (cp[3]  , val);
                                        cp[4] =  complex.Pow (cp[4]  , val);
                                        cp[5] =  complex.Pow (cp[5]  , val);
                                        cp[6] =  complex.Pow (cp[6]  , val);
                                        cp[7] =  complex.Pow (cp[7]  , val);
                                        cp[8] =  complex.Pow (cp[8]  , val);
                                        cp[9] =  complex.Pow (cp[9]  , val);
                                        cp[10] =  complex.Pow (cp[10]  , val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =  complex.Pow (*cp  , val);
                                        cp++;
                                    }
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                fixed ( complex* arrAP = arrA)
                fixed ( complex* arrBP = arrB)
                fixed ( complex* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                               , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                               , (IntPtr)(retArrP + i * outDims[0] * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                                , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                                , (IntPtr)(retArrP + i * outDims[0] * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<complex>(retStorage);
            //}  // no scopes here! it disables implace operations
        }

        /// <summary>Power elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with elementwise result of A <sup>B</sup></returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<byte>  pow(ILInArray<byte> A, ILInArray<byte> B) {
            using (ILScope.Enter(A,B)) {
                int outLen; 
                BinOpItMode mode; 
                byte [] retArr;
                byte [] arrA = A.GetArrayForRead();
                byte[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size; 
                    if (B.IsScalar) {
                       
                        return new  ILRetArray<byte> (new  byte [1]{ saturateBytePow (A.GetValue(0)  , B.GetValue(0))}, A.Size);
                    } else if (B.IsEmpty) {
                        return  ILRetArray<byte>.empty(outDims); 
                    } else {
                        outLen = outDims.NumberOfElements; 
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< byte > (outLen);
                            mode = BinOpItMode.SAN; 
                        } else {
                            mode = BinOpItMode.SAI; 
                        }
                    }
                } else {
                    outDims = A.Size; 
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return  ILRetArray<byte>.empty(A.Size);  
                        }
                        outLen = A.S.NumberOfElements; 
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< byte > (outLen);
                            mode = BinOpItMode.ASN; 
                        } else {
                            mode = BinOpItMode.ASI; 
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  powEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements; 
                        if (A.TryGetStorage4InplaceOp(out retArr)) 
                            mode  = BinOpItMode.AAIA; 
                        else if (B.TryGetStorage4InplaceOp(out retArr)) 
                            mode = BinOpItMode.AAIB; 
                        else {
                            retArr = ILMemoryPool.Pool.New< byte > (outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<byte> retStorage = new ILDenseStorage<byte>(retArr, outDims);
                int i = 0, workerCount = 1; 
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    byte* cp = (byte*)range.Item5 + range.Item1;
                    byte scalar;
                    int j = range.Item2; 
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           byte* bp = ((byte*)range.Item4 + range.Item1);
                            while (j > 20) {
                                cp[0] =  saturateBytePow (cp[0]  , bp[0]);
                                cp[1] =  saturateBytePow (cp[1]  , bp[1]);
                                cp[2] =  saturateBytePow (cp[2]  , bp[2]);
                                cp[3] =  saturateBytePow (cp[3]  , bp[3]);
                                cp[4] =  saturateBytePow (cp[4]  , bp[4]);
                                cp[5] =  saturateBytePow (cp[5]  , bp[5]);
                                cp[6] =  saturateBytePow (cp[6]  , bp[6]);
                                cp[7] =  saturateBytePow (cp[7]  , bp[7]);
                                cp[8] =  saturateBytePow (cp[8]  , bp[8]);
                                cp[9] =  saturateBytePow (cp[9]  , bp[9]);
                                cp[10] =  saturateBytePow (cp[10]  , bp[10]);
                                cp[11] =  saturateBytePow (cp[11]  , bp[11]);
                                cp[12] =  saturateBytePow (cp[12]  , bp[12]);
                                cp[13] =  saturateBytePow (cp[13]  , bp[13]);
                                cp[14] =  saturateBytePow (cp[14]  , bp[14]);
                                cp[15] =  saturateBytePow (cp[15]  , bp[15]);
                                cp[16] =  saturateBytePow (cp[16]  , bp[16]);
                                cp[17] =  saturateBytePow (cp[17]  , bp[17]);
                                cp[18] =  saturateBytePow (cp[18]  , bp[18]);
                                cp[19] =  saturateBytePow (cp[19]  , bp[19]);
                                cp[20] =  saturateBytePow (cp[20]  , bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateBytePow (*cp  , *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           byte* ap = ((byte*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateBytePow (ap[0]  , cp[0]);
                                cp[1] =  saturateBytePow (ap[1]  , cp[1]);
                                cp[2] =  saturateBytePow (ap[2]  , cp[2]);
                                cp[3] =  saturateBytePow (ap[3]  , cp[3]);
                                cp[4] =  saturateBytePow (ap[4]  , cp[4]);
                                cp[5] =  saturateBytePow (ap[5]  , cp[5]);
                                cp[6] =  saturateBytePow (ap[6]  , cp[6]);
                                cp[7] =  saturateBytePow (ap[7]  , cp[7]);
                                cp[8] =  saturateBytePow (ap[8]  , cp[8]);
                                cp[9] =  saturateBytePow (ap[9]  , cp[9]);
                                cp[10] =  saturateBytePow (ap[10]  , cp[10]);
                                cp[11] =  saturateBytePow (ap[11]  , cp[11]);
                                cp[12] =  saturateBytePow (ap[12]  , cp[12]);
                                cp[13] =  saturateBytePow (ap[13]  , cp[13]);
                                cp[14] =  saturateBytePow (ap[14]  , cp[14]);
                                cp[15] =  saturateBytePow (ap[15]  , cp[15]);
                                cp[16] =  saturateBytePow (ap[16]  , cp[16]);
                                cp[17] =  saturateBytePow (ap[17]  , cp[17]);
                                cp[18] =  saturateBytePow (ap[18]  , cp[18]);
                                cp[19] =  saturateBytePow (ap[19]  , cp[19]);
                                cp[20] =  saturateBytePow (ap[20]  , cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateBytePow (*ap  , *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((byte*)range.Item3 + range.Item1);
                            bp = ((byte*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateBytePow (ap[0]  , bp[0]);
                                cp[1] =  saturateBytePow (ap[1]  , bp[1]);
                                cp[2] =  saturateBytePow (ap[2]  , bp[2]);
                                cp[3] =  saturateBytePow (ap[3]  , bp[3]);
                                cp[4] =  saturateBytePow (ap[4]  , bp[4]);
                                cp[5] =  saturateBytePow (ap[5]  , bp[5]);
                                cp[6] =  saturateBytePow (ap[6]  , bp[6]);
                                cp[7] =  saturateBytePow (ap[7]  , bp[7]);
                                cp[8] =  saturateBytePow (ap[8]  , bp[8]);
                                cp[9] =  saturateBytePow (ap[9]  , bp[9]);
                                cp[10] =  saturateBytePow (ap[10]  , bp[10]);
                                cp[11] =  saturateBytePow (ap[11]  , bp[11]);
                                cp[12] =  saturateBytePow (ap[12]  , bp[12]);
                                cp[13] =  saturateBytePow (ap[13]  , bp[13]);
                                cp[14] =  saturateBytePow (ap[14]  , bp[14]);
                                cp[15] =  saturateBytePow (ap[15]  , bp[15]);
                                cp[16] =  saturateBytePow (ap[16]  , bp[16]);
                                cp[17] =  saturateBytePow (ap[17]  , bp[17]);
                                cp[18] =  saturateBytePow (ap[18]  , bp[18]);
                                cp[19] =  saturateBytePow (ap[19]  , bp[19]);
                                cp[20] =  saturateBytePow (ap[20]  , bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateBytePow (*ap  , *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((byte*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  saturateBytePow (cp[0]  , scalar);
                                cp[1] =  saturateBytePow (cp[1]  , scalar);
                                cp[2] =  saturateBytePow (cp[2]  , scalar);
                                cp[3] =  saturateBytePow (cp[3]  , scalar);
                                cp[4] =  saturateBytePow (cp[4]  , scalar);
                                cp[5] =  saturateBytePow (cp[5]  , scalar);
                                cp[6] =  saturateBytePow (cp[6]  , scalar);
                                cp[7] =  saturateBytePow (cp[7]  , scalar);
                                cp[8] =  saturateBytePow (cp[8]  , scalar);
                                cp[9] =  saturateBytePow (cp[9]  , scalar);
                                cp[10] =  saturateBytePow (cp[10]  , scalar);
                                cp[11] =  saturateBytePow (cp[11]  , scalar);
                                cp[12] =  saturateBytePow (cp[12]  , scalar);
                                cp[13] =  saturateBytePow (cp[13]  , scalar);
                                cp[14] =  saturateBytePow (cp[14]  , scalar);
                                cp[15] =  saturateBytePow (cp[15]  , scalar);
                                cp[16] =  saturateBytePow (cp[16]  , scalar);
                                cp[17] =  saturateBytePow (cp[17]  , scalar);
                                cp[18] =  saturateBytePow (cp[18]  , scalar);
                                cp[19] =  saturateBytePow (cp[19]  , scalar);
                                cp[20] =  saturateBytePow (cp[20]  , scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateBytePow (*cp  , scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((byte*)range.Item3 + range.Item1);
                            scalar = *((byte*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  saturateBytePow (ap[0]  , scalar);
                                cp[1] =  saturateBytePow (ap[1]  , scalar);
                                cp[2] =  saturateBytePow (ap[2]  , scalar);
                                cp[3] =  saturateBytePow (ap[3]  , scalar);
                                cp[4] =  saturateBytePow (ap[4]  , scalar);
                                cp[5] =  saturateBytePow (ap[5]  , scalar);
                                cp[6] =  saturateBytePow (ap[6]  , scalar);
                                cp[7] =  saturateBytePow (ap[7]  , scalar);
                                cp[8] =  saturateBytePow (ap[8]  , scalar);
                                cp[9] =  saturateBytePow (ap[9]  , scalar);
                                cp[10] =  saturateBytePow (ap[10]  , scalar);
                                cp[11] =  saturateBytePow (ap[11]  , scalar);
                                cp[12] =  saturateBytePow (ap[12]  , scalar);
                                cp[13] =  saturateBytePow (ap[13]  , scalar);
                                cp[14] =  saturateBytePow (ap[14]  , scalar);
                                cp[15] =  saturateBytePow (ap[15]  , scalar);
                                cp[16] =  saturateBytePow (ap[16]  , scalar);
                                cp[17] =  saturateBytePow (ap[17]  , scalar);
                                cp[18] =  saturateBytePow (ap[18]  , scalar);
                                cp[19] =  saturateBytePow (ap[19]  , scalar);
                                cp[20] =  saturateBytePow (ap[20]  , scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateBytePow (*ap  , scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((byte*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =  saturateBytePow (scalar  , cp[0]);
                                cp[1] =  saturateBytePow (scalar  , cp[1]);
                                cp[2] =  saturateBytePow (scalar  , cp[2]);
                                cp[3] =  saturateBytePow (scalar  , cp[3]);
                                cp[4] =  saturateBytePow (scalar  , cp[4]);
                                cp[5] =  saturateBytePow (scalar  , cp[5]);
                                cp[6] =  saturateBytePow (scalar  , cp[6]);
                                cp[7] =  saturateBytePow (scalar  , cp[7]);
                                cp[8] =  saturateBytePow (scalar  , cp[8]);
                                cp[9] =  saturateBytePow (scalar  , cp[9]);
                                cp[10] =  saturateBytePow (scalar  , cp[10]);
                                cp[11] =  saturateBytePow (scalar  , cp[11]);
                                cp[12] =  saturateBytePow (scalar  , cp[12]);
                                cp[13] =  saturateBytePow (scalar  , cp[13]);
                                cp[14] =  saturateBytePow (scalar  , cp[14]);
                                cp[15] =  saturateBytePow (scalar  , cp[15]);
                                cp[16] =  saturateBytePow (scalar  , cp[16]);
                                cp[17] =  saturateBytePow (scalar  , cp[17]);
                                cp[18] =  saturateBytePow (scalar  , cp[18]);
                                cp[19] =  saturateBytePow (scalar  , cp[19]);
                                cp[20] =  saturateBytePow (scalar  , cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateBytePow (scalar  , *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((byte*)range.Item3);
                            bp = ((byte*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateBytePow (scalar  , bp[0]);
                                cp[1] =  saturateBytePow (scalar  , bp[1]);
                                cp[2] =  saturateBytePow (scalar  , bp[2]);
                                cp[3] =  saturateBytePow (scalar  , bp[3]);
                                cp[4] =  saturateBytePow (scalar  , bp[4]);
                                cp[5] =  saturateBytePow (scalar  , bp[5]);
                                cp[6] =  saturateBytePow (scalar  , bp[6]);
                                cp[7] =  saturateBytePow (scalar  , bp[7]);
                                cp[8] =  saturateBytePow (scalar  , bp[8]);
                                cp[9] =  saturateBytePow (scalar  , bp[9]);
                                cp[10] =  saturateBytePow (scalar  , bp[10]);
                                cp[11] =  saturateBytePow (scalar  , bp[11]);
                                cp[12] =  saturateBytePow (scalar  , bp[12]);
                                cp[13] =  saturateBytePow (scalar  , bp[13]);
                                cp[14] =  saturateBytePow (scalar  , bp[14]);
                                cp[15] =  saturateBytePow (scalar  , bp[15]);
                                cp[16] =  saturateBytePow (scalar  , bp[16]);
                                cp[17] =  saturateBytePow (scalar  , bp[17]);
                                cp[18] =  saturateBytePow (scalar  , bp[18]);
                                cp[19] =  saturateBytePow (scalar  , bp[19]);
                                cp[20] =  saturateBytePow (scalar  , bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateBytePow (scalar  , *bp);
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);

                };

                #region do the work 
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
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
                
                fixed ( byte* arrAP = arrA)
                fixed ( byte* arrBP = arrB)
                fixed ( byte* retArrP = retArr) {

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
                return new  ILRetArray< byte>(retStorage);
            }
        }

        private static unsafe ILRetArray<byte>  powEx(ILInArray<byte> A, ILInArray<byte> B) {
            //using (ILScope.Enter(A, B)) { we cannot start a new scope here, since this would prevent A and B to be used implace if applicable

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<byte>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<byte>(B.S);
                } else if (B.IsEmpty) {
                    return empty<byte>(A.S);
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
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
               
                byte[] retArr;
               
                byte[] arrA = A.GetArrayForRead();
               
                byte[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int workItemMultiplierLenA;
                int workItemMultiplierLenB;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    workItemMultiplierLenB = outDims[0]; 
                    workItemMultiplierLenA = dim;  // 0 for column, 1 for row vector
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    workItemMultiplierLenB = dim;  // 0 for column, 1 for row vector
                    workItemMultiplierLenA = outDims[0]; 
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in either A or B");
                }
                int itLen = outDims[0]; // (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                #endregion

                #region worker loops definition
                ILDenseStorage<byte> retStorage = new ILDenseStorage<byte>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;
                   
                    byte* ap;
                   
                    byte* bp;
                   
                    byte* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            if (dim == 0) {
                                bp = (byte*)range.Item3;
                                cp = (byte*)range.Item4; 
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (byte*)range.Item2;
                                    int l = itLen; 
                                    while (l > 10) {
                                        cp[0] =  saturateBytePow (ap[0]  , bp[0]);
                                        cp[1] =  saturateBytePow (ap[1]  , bp[1]);
                                        cp[2] =  saturateBytePow (ap[2]  , bp[2]);
                                        cp[3] =  saturateBytePow (ap[3]  , bp[3]);
                                        cp[4] =  saturateBytePow (ap[4]  , bp[4]);
                                        cp[5] =  saturateBytePow (ap[5]  , bp[5]);
                                        cp[6] =  saturateBytePow (ap[6]  , bp[6]);
                                        cp[7] =  saturateBytePow (ap[7]  , bp[7]);
                                        cp[8] =  saturateBytePow (ap[8]  , bp[8]);
                                        cp[9] =  saturateBytePow (ap[9]  , bp[9]);
                                        cp[10] =  saturateBytePow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  saturateBytePow (*ap++  , *bp++);
                                    }
                                }
                            } else {
                                // dim == 1
                                ap = (byte*)range.Item2;
                                bp = (byte*)range.Item3;
                                cp = (byte*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   byte val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateBytePow (val  , bp[0]);
                                        cp[1] =  saturateBytePow (val  , bp[1]);
                                        cp[2] =  saturateBytePow (val  , bp[2]);
                                        cp[3] =  saturateBytePow (val  , bp[3]);
                                        cp[4] =  saturateBytePow (val  , bp[4]);
                                        cp[5] =  saturateBytePow (val  , bp[5]);
                                        cp[6] =  saturateBytePow (val  , bp[6]);
                                        cp[7] =  saturateBytePow (val  , bp[7]);
                                        cp[8] =  saturateBytePow (val  , bp[8]);
                                        cp[9] =  saturateBytePow (val  , bp[9]);
                                        cp[10] =  saturateBytePow (val  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  saturateBytePow (val  , *bp++);
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            if (dim == 0) {
                                cp = (byte*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (byte*)range.Item2;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateBytePow (ap[0]  , cp[0]);
                                        cp[1] =  saturateBytePow (ap[1]  , cp[1]);
                                        cp[2] =  saturateBytePow (ap[2]  , cp[2]);
                                        cp[3] =  saturateBytePow (ap[3]  , cp[3]);
                                        cp[4] =  saturateBytePow (ap[4]  , cp[4]);
                                        cp[5] =  saturateBytePow (ap[5]  , cp[5]);
                                        cp[6] =  saturateBytePow (ap[6]  , cp[6]);
                                        cp[7] =  saturateBytePow (ap[7]  , cp[7]);
                                        cp[8] =  saturateBytePow (ap[8]  , cp[8]);
                                        cp[9] =  saturateBytePow (ap[9]  , cp[9]);
                                        cp[10] =  saturateBytePow (ap[10]  , cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateBytePow (*ap++  , *cp);
                                        cp++; 
                                    }
                                }
                            } else {
                                // dim == 1
                                cp = (byte*)range.Item4;
                                ap = (byte*)range.Item2;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    byte val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateBytePow (val  , cp[0]);
                                        cp[1] =  saturateBytePow (val  , cp[1]);
                                        cp[2] =  saturateBytePow (val  , cp[2]);
                                        cp[3] =  saturateBytePow (val  , cp[3]);
                                        cp[4] =  saturateBytePow (val  , cp[4]);
                                        cp[5] =  saturateBytePow (val  , cp[5]);
                                        cp[6] =  saturateBytePow (val  , cp[6]);
                                        cp[7] =  saturateBytePow (val  , cp[7]);
                                        cp[8] =  saturateBytePow (val  , cp[8]);
                                        cp[9] =  saturateBytePow (val  , cp[9]);
                                        cp[10] =  saturateBytePow (val  , cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateBytePow (val  , *cp);
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            if (dim == 0) {
                                ap = (byte*)range.Item2;
                                cp = (byte*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (byte*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateBytePow (ap[0]  , bp[0]);
                                        cp[1] =  saturateBytePow (ap[1]  , bp[1]);
                                        cp[2] =  saturateBytePow (ap[2]  , bp[2]);
                                        cp[3] =  saturateBytePow (ap[3]  , bp[3]);
                                        cp[4] =  saturateBytePow (ap[4]  , bp[4]);
                                        cp[5] =  saturateBytePow (ap[5]  , bp[5]);
                                        cp[6] =  saturateBytePow (ap[6]  , bp[6]);
                                        cp[7] =  saturateBytePow (ap[7]  , bp[7]);
                                        cp[8] =  saturateBytePow (ap[8]  , bp[8]);
                                        cp[9] =  saturateBytePow (ap[9]  , bp[9]);
                                        cp[10] =  saturateBytePow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateBytePow (*ap  , *bp);
                                        ap++;
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                ap = (byte*)range.Item2;
                                bp = (byte*)range.Item3;
                                cp = (byte*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   byte val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateBytePow (ap[0]  , val);
                                        cp[1] =  saturateBytePow (ap[1]  , val);
                                        cp[2] =  saturateBytePow (ap[2]  , val);
                                        cp[3] =  saturateBytePow (ap[3]  , val);
                                        cp[4] =  saturateBytePow (ap[4]  , val);
                                        cp[5] =  saturateBytePow (ap[5]  , val);
                                        cp[6] =  saturateBytePow (ap[6]  , val);
                                        cp[7] =  saturateBytePow (ap[7]  , val);
                                        cp[8] =  saturateBytePow (ap[8]  , val);
                                        cp[9] =  saturateBytePow (ap[9]  , val);
                                        cp[10] =  saturateBytePow (ap[10]  , val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateBytePow (*ap  , val);
                                        ap++;
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            if (dim == 0) {
                                cp = (byte*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (byte*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateBytePow (cp[0]  , bp[0]);
                                        cp[1] =  saturateBytePow (cp[1]  , bp[1]);
                                        cp[2] =  saturateBytePow (cp[2]  , bp[2]);
                                        cp[3] =  saturateBytePow (cp[3]  , bp[3]);
                                        cp[4] =  saturateBytePow (cp[4]  , bp[4]);
                                        cp[5] =  saturateBytePow (cp[5]  , bp[5]);
                                        cp[6] =  saturateBytePow (cp[6]  , bp[6]);
                                        cp[7] =  saturateBytePow (cp[7]  , bp[7]);
                                        cp[8] =  saturateBytePow (cp[8]  , bp[8]);
                                        cp[9] =  saturateBytePow (cp[9]  , bp[9]);
                                        cp[10] =  saturateBytePow (cp[10]  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateBytePow (*cp  , *bp);
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                bp = (byte*)range.Item3;
                                cp = (byte*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    byte val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  saturateBytePow (cp[0]  , val);
                                        cp[1] =  saturateBytePow (cp[1]  , val);
                                        cp[2] =  saturateBytePow (cp[2]  , val);
                                        cp[3] =  saturateBytePow (cp[3]  , val);
                                        cp[4] =  saturateBytePow (cp[4]  , val);
                                        cp[5] =  saturateBytePow (cp[5]  , val);
                                        cp[6] =  saturateBytePow (cp[6]  , val);
                                        cp[7] =  saturateBytePow (cp[7]  , val);
                                        cp[8] =  saturateBytePow (cp[8]  , val);
                                        cp[9] =  saturateBytePow (cp[9]  , val);
                                        cp[10] =  saturateBytePow (cp[10]  , val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =  saturateBytePow (*cp  , val);
                                        cp++;
                                    }
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                fixed ( byte* arrAP = arrA)
                fixed ( byte* arrBP = arrB)
                fixed ( byte* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                               , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                               , (IntPtr)(retArrP + i * outDims[0] * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                                , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                                , (IntPtr)(retArrP + i * outDims[0] * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<byte>(retStorage);
            //}  // no scopes here! it disables implace operations
        }

        /// <summary>Power elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with elementwise result of A <sup>B</sup></returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<double>  pow(ILInArray<double> A, ILInArray<double> B) {
            using (ILScope.Enter(A,B)) {
                int outLen; 
                BinOpItMode mode; 
                double [] retArr;
                double [] arrA = A.GetArrayForRead();
                double[] arrB = B.GetArrayForRead(); 
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size; 
                    if (B.IsScalar) {
                       
                        return new  ILRetArray<double> (new  double [1]{ Math.Pow (A.GetValue(0)  , B.GetValue(0))}, A.Size);
                    } else if (B.IsEmpty) {
                        return  ILRetArray<double>.empty(outDims); 
                    } else {
                        outLen = outDims.NumberOfElements; 
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< double > (outLen);
                            mode = BinOpItMode.SAN; 
                        } else {
                            mode = BinOpItMode.SAI; 
                        }
                    }
                } else {
                    outDims = A.Size; 
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return  ILRetArray<double>.empty(A.Size);  
                        }
                        outLen = A.S.NumberOfElements; 
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New< double > (outLen);
                            mode = BinOpItMode.ASN; 
                        } else {
                            mode = BinOpItMode.ASI; 
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return  powEx(A,B); 
                        }
                        outLen = A.S.NumberOfElements; 
                        if (A.TryGetStorage4InplaceOp(out retArr)) 
                            mode  = BinOpItMode.AAIA; 
                        else if (B.TryGetStorage4InplaceOp(out retArr)) 
                            mode = BinOpItMode.AAIB; 
                        else {
                            retArr = ILMemoryPool.Pool.New< double > (outLen);
                            mode = BinOpItMode.AAN;
                        }
                    }
                }
                #endregion
                ILDenseStorage<double> retStorage = new ILDenseStorage<double>(retArr, outDims);
                int i = 0, workerCount = 1; 
                Action<object> worker = data => {
                    Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode> range
                            = (Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>)data;
                    double* cp = (double*)range.Item5 + range.Item1;
                    double scalar;
                    int j = range.Item2; 
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           double* bp = ((double*)range.Item4 + range.Item1);
                            while (j > 20) {
                                cp[0] =  Math.Pow (cp[0]  , bp[0]);
                                cp[1] =  Math.Pow (cp[1]  , bp[1]);
                                cp[2] =  Math.Pow (cp[2]  , bp[2]);
                                cp[3] =  Math.Pow (cp[3]  , bp[3]);
                                cp[4] =  Math.Pow (cp[4]  , bp[4]);
                                cp[5] =  Math.Pow (cp[5]  , bp[5]);
                                cp[6] =  Math.Pow (cp[6]  , bp[6]);
                                cp[7] =  Math.Pow (cp[7]  , bp[7]);
                                cp[8] =  Math.Pow (cp[8]  , bp[8]);
                                cp[9] =  Math.Pow (cp[9]  , bp[9]);
                                cp[10] =  Math.Pow (cp[10]  , bp[10]);
                                cp[11] =  Math.Pow (cp[11]  , bp[11]);
                                cp[12] =  Math.Pow (cp[12]  , bp[12]);
                                cp[13] =  Math.Pow (cp[13]  , bp[13]);
                                cp[14] =  Math.Pow (cp[14]  , bp[14]);
                                cp[15] =  Math.Pow (cp[15]  , bp[15]);
                                cp[16] =  Math.Pow (cp[16]  , bp[16]);
                                cp[17] =  Math.Pow (cp[17]  , bp[17]);
                                cp[18] =  Math.Pow (cp[18]  , bp[18]);
                                cp[19] =  Math.Pow (cp[19]  , bp[19]);
                                cp[20] =  Math.Pow (cp[20]  , bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  Math.Pow (*cp  , *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           double* ap = ((double*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  Math.Pow (ap[0]  , cp[0]);
                                cp[1] =  Math.Pow (ap[1]  , cp[1]);
                                cp[2] =  Math.Pow (ap[2]  , cp[2]);
                                cp[3] =  Math.Pow (ap[3]  , cp[3]);
                                cp[4] =  Math.Pow (ap[4]  , cp[4]);
                                cp[5] =  Math.Pow (ap[5]  , cp[5]);
                                cp[6] =  Math.Pow (ap[6]  , cp[6]);
                                cp[7] =  Math.Pow (ap[7]  , cp[7]);
                                cp[8] =  Math.Pow (ap[8]  , cp[8]);
                                cp[9] =  Math.Pow (ap[9]  , cp[9]);
                                cp[10] =  Math.Pow (ap[10]  , cp[10]);
                                cp[11] =  Math.Pow (ap[11]  , cp[11]);
                                cp[12] =  Math.Pow (ap[12]  , cp[12]);
                                cp[13] =  Math.Pow (ap[13]  , cp[13]);
                                cp[14] =  Math.Pow (ap[14]  , cp[14]);
                                cp[15] =  Math.Pow (ap[15]  , cp[15]);
                                cp[16] =  Math.Pow (ap[16]  , cp[16]);
                                cp[17] =  Math.Pow (ap[17]  , cp[17]);
                                cp[18] =  Math.Pow (ap[18]  , cp[18]);
                                cp[19] =  Math.Pow (ap[19]  , cp[19]);
                                cp[20] =  Math.Pow (ap[20]  , cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  Math.Pow (*ap  , *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((double*)range.Item3 + range.Item1);
                            bp = ((double*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  Math.Pow (ap[0]  , bp[0]);
                                cp[1] =  Math.Pow (ap[1]  , bp[1]);
                                cp[2] =  Math.Pow (ap[2]  , bp[2]);
                                cp[3] =  Math.Pow (ap[3]  , bp[3]);
                                cp[4] =  Math.Pow (ap[4]  , bp[4]);
                                cp[5] =  Math.Pow (ap[5]  , bp[5]);
                                cp[6] =  Math.Pow (ap[6]  , bp[6]);
                                cp[7] =  Math.Pow (ap[7]  , bp[7]);
                                cp[8] =  Math.Pow (ap[8]  , bp[8]);
                                cp[9] =  Math.Pow (ap[9]  , bp[9]);
                                cp[10] =  Math.Pow (ap[10]  , bp[10]);
                                cp[11] =  Math.Pow (ap[11]  , bp[11]);
                                cp[12] =  Math.Pow (ap[12]  , bp[12]);
                                cp[13] =  Math.Pow (ap[13]  , bp[13]);
                                cp[14] =  Math.Pow (ap[14]  , bp[14]);
                                cp[15] =  Math.Pow (ap[15]  , bp[15]);
                                cp[16] =  Math.Pow (ap[16]  , bp[16]);
                                cp[17] =  Math.Pow (ap[17]  , bp[17]);
                                cp[18] =  Math.Pow (ap[18]  , bp[18]);
                                cp[19] =  Math.Pow (ap[19]  , bp[19]);
                                cp[20] =  Math.Pow (ap[20]  , bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =  Math.Pow (*ap  , *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((double*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  Math.Pow (cp[0]  , scalar);
                                cp[1] =  Math.Pow (cp[1]  , scalar);
                                cp[2] =  Math.Pow (cp[2]  , scalar);
                                cp[3] =  Math.Pow (cp[3]  , scalar);
                                cp[4] =  Math.Pow (cp[4]  , scalar);
                                cp[5] =  Math.Pow (cp[5]  , scalar);
                                cp[6] =  Math.Pow (cp[6]  , scalar);
                                cp[7] =  Math.Pow (cp[7]  , scalar);
                                cp[8] =  Math.Pow (cp[8]  , scalar);
                                cp[9] =  Math.Pow (cp[9]  , scalar);
                                cp[10] =  Math.Pow (cp[10]  , scalar);
                                cp[11] =  Math.Pow (cp[11]  , scalar);
                                cp[12] =  Math.Pow (cp[12]  , scalar);
                                cp[13] =  Math.Pow (cp[13]  , scalar);
                                cp[14] =  Math.Pow (cp[14]  , scalar);
                                cp[15] =  Math.Pow (cp[15]  , scalar);
                                cp[16] =  Math.Pow (cp[16]  , scalar);
                                cp[17] =  Math.Pow (cp[17]  , scalar);
                                cp[18] =  Math.Pow (cp[18]  , scalar);
                                cp[19] =  Math.Pow (cp[19]  , scalar);
                                cp[20] =  Math.Pow (cp[20]  , scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  Math.Pow (*cp  , scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((double*)range.Item3 + range.Item1);
                            scalar = *((double*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  Math.Pow (ap[0]  , scalar);
                                cp[1] =  Math.Pow (ap[1]  , scalar);
                                cp[2] =  Math.Pow (ap[2]  , scalar);
                                cp[3] =  Math.Pow (ap[3]  , scalar);
                                cp[4] =  Math.Pow (ap[4]  , scalar);
                                cp[5] =  Math.Pow (ap[5]  , scalar);
                                cp[6] =  Math.Pow (ap[6]  , scalar);
                                cp[7] =  Math.Pow (ap[7]  , scalar);
                                cp[8] =  Math.Pow (ap[8]  , scalar);
                                cp[9] =  Math.Pow (ap[9]  , scalar);
                                cp[10] =  Math.Pow (ap[10]  , scalar);
                                cp[11] =  Math.Pow (ap[11]  , scalar);
                                cp[12] =  Math.Pow (ap[12]  , scalar);
                                cp[13] =  Math.Pow (ap[13]  , scalar);
                                cp[14] =  Math.Pow (ap[14]  , scalar);
                                cp[15] =  Math.Pow (ap[15]  , scalar);
                                cp[16] =  Math.Pow (ap[16]  , scalar);
                                cp[17] =  Math.Pow (ap[17]  , scalar);
                                cp[18] =  Math.Pow (ap[18]  , scalar);
                                cp[19] =  Math.Pow (ap[19]  , scalar);
                                cp[20] =  Math.Pow (ap[20]  , scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  Math.Pow (*ap  , scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((double*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =  Math.Pow (scalar  , cp[0]);
                                cp[1] =  Math.Pow (scalar  , cp[1]);
                                cp[2] =  Math.Pow (scalar  , cp[2]);
                                cp[3] =  Math.Pow (scalar  , cp[3]);
                                cp[4] =  Math.Pow (scalar  , cp[4]);
                                cp[5] =  Math.Pow (scalar  , cp[5]);
                                cp[6] =  Math.Pow (scalar  , cp[6]);
                                cp[7] =  Math.Pow (scalar  , cp[7]);
                                cp[8] =  Math.Pow (scalar  , cp[8]);
                                cp[9] =  Math.Pow (scalar  , cp[9]);
                                cp[10] =  Math.Pow (scalar  , cp[10]);
                                cp[11] =  Math.Pow (scalar  , cp[11]);
                                cp[12] =  Math.Pow (scalar  , cp[12]);
                                cp[13] =  Math.Pow (scalar  , cp[13]);
                                cp[14] =  Math.Pow (scalar  , cp[14]);
                                cp[15] =  Math.Pow (scalar  , cp[15]);
                                cp[16] =  Math.Pow (scalar  , cp[16]);
                                cp[17] =  Math.Pow (scalar  , cp[17]);
                                cp[18] =  Math.Pow (scalar  , cp[18]);
                                cp[19] =  Math.Pow (scalar  , cp[19]);
                                cp[20] =  Math.Pow (scalar  , cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  Math.Pow (scalar  , *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((double*)range.Item3);
                            bp = ((double*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  Math.Pow (scalar  , bp[0]);
                                cp[1] =  Math.Pow (scalar  , bp[1]);
                                cp[2] =  Math.Pow (scalar  , bp[2]);
                                cp[3] =  Math.Pow (scalar  , bp[3]);
                                cp[4] =  Math.Pow (scalar  , bp[4]);
                                cp[5] =  Math.Pow (scalar  , bp[5]);
                                cp[6] =  Math.Pow (scalar  , bp[6]);
                                cp[7] =  Math.Pow (scalar  , bp[7]);
                                cp[8] =  Math.Pow (scalar  , bp[8]);
                                cp[9] =  Math.Pow (scalar  , bp[9]);
                                cp[10] =  Math.Pow (scalar  , bp[10]);
                                cp[11] =  Math.Pow (scalar  , bp[11]);
                                cp[12] =  Math.Pow (scalar  , bp[12]);
                                cp[13] =  Math.Pow (scalar  , bp[13]);
                                cp[14] =  Math.Pow (scalar  , bp[14]);
                                cp[15] =  Math.Pow (scalar  , bp[15]);
                                cp[16] =  Math.Pow (scalar  , bp[16]);
                                cp[17] =  Math.Pow (scalar  , bp[17]);
                                cp[18] =  Math.Pow (scalar  , bp[18]);
                                cp[19] =  Math.Pow (scalar  , bp[19]);
                                cp[20] =  Math.Pow (scalar  , bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  Math.Pow (scalar  , *bp);
                                bp++; cp++;
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);

                };

                #region do the work 
                int workItemCount = Settings.s_maxNumberThreads, workItemLength;
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
                
                fixed ( double* arrAP = arrA)
                fixed ( double* arrBP = arrB)
                fixed ( double* retArrP = retArr) {

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
                return new  ILRetArray< double>(retStorage);
            }
        }

        private static unsafe ILRetArray<double>  powEx(ILInArray<double> A, ILInArray<double> B) {
            //using (ILScope.Enter(A, B)) { we cannot start a new scope here, since this would prevent A and B to be used implace if applicable

                #region parameter checking
                if (isnull(A) || isnull(B))
                    return empty<double>(ILSize.Empty00);
                if (A.IsEmpty) {
                    return empty<double>(B.S);
                } else if (B.IsEmpty) {
                    return empty<double>(A.S);
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
                dim = -(dim - 1);  // 0 -> 1, 1 -> 0
                #endregion

                #region parameter preparation
               
                double[] retArr;
               
                double[] arrA = A.GetArrayForRead();
               
                double[] arrB = B.GetArrayForRead();
                ILSize outDims;
                BinOptItExMode mode;
                int workItemMultiplierLenA;
                int workItemMultiplierLenB;
                if (A.IsVector) {
                    outDims = B.S;
                    if (!B.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<double>(outDims.NumberOfElements);
                        mode = BinOptItExMode.VAN;
                    } else {
                        mode = BinOptItExMode.VAI;
                    }
                    workItemMultiplierLenB = outDims[0]; 
                    workItemMultiplierLenA = dim;  // 0 for column, 1 for row vector
                } else if (B.IsVector) {
                    outDims = A.S;
                    if (!A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILMemoryPool.Pool.New<double>(outDims.NumberOfElements);
                        mode = BinOptItExMode.AVN;
                    } else {
                        mode = BinOptItExMode.AVI;
                    }
                    workItemMultiplierLenB = dim;  // 0 for column, 1 for row vector
                    workItemMultiplierLenA = outDims[0]; 
                } else {
                    throw new ILArgumentException("A and B must have the same size except for one singleton dimension in either A or B");
                }
                int itLen = outDims[0]; // (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
                #endregion

                #region worker loops definition
                ILDenseStorage<double> retStorage = new ILDenseStorage<double>(retArr, outDims);
                int workerCount = 1;
                Action<object> worker = data => {
                    // expects: iStart, iLen, ap, bp, cp
                    Tuple<int, IntPtr, IntPtr, IntPtr> range =
                        (Tuple<int, IntPtr, IntPtr, IntPtr>)data;
                   
                    double* ap;
                   
                    double* bp;
                   
                    double* cp;
                    switch (mode) {
                        case BinOptItExMode.VAN:
                            if (dim == 0) {
                                bp = (double*)range.Item3;
                                cp = (double*)range.Item4; 
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (double*)range.Item2;
                                    int l = itLen; 
                                    while (l > 10) {
                                        cp[0] =  Math.Pow (ap[0]  , bp[0]);
                                        cp[1] =  Math.Pow (ap[1]  , bp[1]);
                                        cp[2] =  Math.Pow (ap[2]  , bp[2]);
                                        cp[3] =  Math.Pow (ap[3]  , bp[3]);
                                        cp[4] =  Math.Pow (ap[4]  , bp[4]);
                                        cp[5] =  Math.Pow (ap[5]  , bp[5]);
                                        cp[6] =  Math.Pow (ap[6]  , bp[6]);
                                        cp[7] =  Math.Pow (ap[7]  , bp[7]);
                                        cp[8] =  Math.Pow (ap[8]  , bp[8]);
                                        cp[9] =  Math.Pow (ap[9]  , bp[9]);
                                        cp[10] =  Math.Pow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  Math.Pow (*ap++  , *bp++);
                                    }
                                }
                            } else {
                                // dim == 1
                                ap = (double*)range.Item2;
                                bp = (double*)range.Item3;
                                cp = (double*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   double val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  Math.Pow (val  , bp[0]);
                                        cp[1] =  Math.Pow (val  , bp[1]);
                                        cp[2] =  Math.Pow (val  , bp[2]);
                                        cp[3] =  Math.Pow (val  , bp[3]);
                                        cp[4] =  Math.Pow (val  , bp[4]);
                                        cp[5] =  Math.Pow (val  , bp[5]);
                                        cp[6] =  Math.Pow (val  , bp[6]);
                                        cp[7] =  Math.Pow (val  , bp[7]);
                                        cp[8] =  Math.Pow (val  , bp[8]);
                                        cp[9] =  Math.Pow (val  , bp[9]);
                                        cp[10] =  Math.Pow (val  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  Math.Pow (val  , *bp++);
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.VAI:
                            if (dim == 0) {
                                cp = (double*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    ap = (double*)range.Item2;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  Math.Pow (ap[0]  , cp[0]);
                                        cp[1] =  Math.Pow (ap[1]  , cp[1]);
                                        cp[2] =  Math.Pow (ap[2]  , cp[2]);
                                        cp[3] =  Math.Pow (ap[3]  , cp[3]);
                                        cp[4] =  Math.Pow (ap[4]  , cp[4]);
                                        cp[5] =  Math.Pow (ap[5]  , cp[5]);
                                        cp[6] =  Math.Pow (ap[6]  , cp[6]);
                                        cp[7] =  Math.Pow (ap[7]  , cp[7]);
                                        cp[8] =  Math.Pow (ap[8]  , cp[8]);
                                        cp[9] =  Math.Pow (ap[9]  , cp[9]);
                                        cp[10] =  Math.Pow (ap[10]  , cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  Math.Pow (*ap++  , *cp);
                                        cp++; 
                                    }
                                }
                            } else {
                                // dim == 1
                                cp = (double*)range.Item4;
                                ap = (double*)range.Item2;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    double val = *ap++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  Math.Pow (val  , cp[0]);
                                        cp[1] =  Math.Pow (val  , cp[1]);
                                        cp[2] =  Math.Pow (val  , cp[2]);
                                        cp[3] =  Math.Pow (val  , cp[3]);
                                        cp[4] =  Math.Pow (val  , cp[4]);
                                        cp[5] =  Math.Pow (val  , cp[5]);
                                        cp[6] =  Math.Pow (val  , cp[6]);
                                        cp[7] =  Math.Pow (val  , cp[7]);
                                        cp[8] =  Math.Pow (val  , cp[8]);
                                        cp[9] =  Math.Pow (val  , cp[9]);
                                        cp[10] =  Math.Pow (val  , cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  Math.Pow (val  , *cp);
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVN:
                            if (dim == 0) {
                                ap = (double*)range.Item2;
                                cp = (double*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (double*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  Math.Pow (ap[0]  , bp[0]);
                                        cp[1] =  Math.Pow (ap[1]  , bp[1]);
                                        cp[2] =  Math.Pow (ap[2]  , bp[2]);
                                        cp[3] =  Math.Pow (ap[3]  , bp[3]);
                                        cp[4] =  Math.Pow (ap[4]  , bp[4]);
                                        cp[5] =  Math.Pow (ap[5]  , bp[5]);
                                        cp[6] =  Math.Pow (ap[6]  , bp[6]);
                                        cp[7] =  Math.Pow (ap[7]  , bp[7]);
                                        cp[8] =  Math.Pow (ap[8]  , bp[8]);
                                        cp[9] =  Math.Pow (ap[9]  , bp[9]);
                                        cp[10] =  Math.Pow (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  Math.Pow (*ap  , *bp);
                                        ap++;
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                ap = (double*)range.Item2;
                                bp = (double*)range.Item3;
                                cp = (double*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   double val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  Math.Pow (ap[0]  , val);
                                        cp[1] =  Math.Pow (ap[1]  , val);
                                        cp[2] =  Math.Pow (ap[2]  , val);
                                        cp[3] =  Math.Pow (ap[3]  , val);
                                        cp[4] =  Math.Pow (ap[4]  , val);
                                        cp[5] =  Math.Pow (ap[5]  , val);
                                        cp[6] =  Math.Pow (ap[6]  , val);
                                        cp[7] =  Math.Pow (ap[7]  , val);
                                        cp[8] =  Math.Pow (ap[8]  , val);
                                        cp[9] =  Math.Pow (ap[9]  , val);
                                        cp[10] =  Math.Pow (ap[10]  , val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  Math.Pow (*ap  , val);
                                        ap++;
                                        cp++;
                                    }
                                }
                            }
                            break;
                        case BinOptItExMode.AVI:
                            if (dim == 0) {
                                cp = (double*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                    bp = (double*)range.Item3;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  Math.Pow (cp[0]  , bp[0]);
                                        cp[1] =  Math.Pow (cp[1]  , bp[1]);
                                        cp[2] =  Math.Pow (cp[2]  , bp[2]);
                                        cp[3] =  Math.Pow (cp[3]  , bp[3]);
                                        cp[4] =  Math.Pow (cp[4]  , bp[4]);
                                        cp[5] =  Math.Pow (cp[5]  , bp[5]);
                                        cp[6] =  Math.Pow (cp[6]  , bp[6]);
                                        cp[7] =  Math.Pow (cp[7]  , bp[7]);
                                        cp[8] =  Math.Pow (cp[8]  , bp[8]);
                                        cp[9] =  Math.Pow (cp[9]  , bp[9]);
                                        cp[10] =  Math.Pow (cp[10]  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  Math.Pow (*cp  , *bp);
                                        bp++;
                                        cp++;
                                    }
                                }
                            } else {
                                // dim = 1
                                bp = (double*)range.Item3;
                                cp = (double*)range.Item4;
                                for (int s = 0; s < range.Item1; s++) {
                                   
                                    double val = *bp++;
                                    int l = itLen;
                                    while (l > 10) {
                                        cp[0] =  Math.Pow (cp[0]  , val);
                                        cp[1] =  Math.Pow (cp[1]  , val);
                                        cp[2] =  Math.Pow (cp[2]  , val);
                                        cp[3] =  Math.Pow (cp[3]  , val);
                                        cp[4] =  Math.Pow (cp[4]  , val);
                                        cp[5] =  Math.Pow (cp[5]  , val);
                                        cp[6] =  Math.Pow (cp[6]  , val);
                                        cp[7] =  Math.Pow (cp[7]  , val);
                                        cp[8] =  Math.Pow (cp[8]  , val);
                                        cp[9] =  Math.Pow (cp[9]  , val);
                                        cp[10] =  Math.Pow (cp[10]  , val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =  Math.Pow (*cp  , val);
                                        cp++;
                                    }
                                }
                            }
                            break;
                    }
                    System.Threading.Interlocked.Decrement(ref workerCount);
                };
                #endregion

                #region work distribution
                int i = 0, workItemCount = Settings.s_maxNumberThreads, workItemLength;
                if (Settings.s_maxNumberThreads > 1 && outDims.NumberOfElements >= Settings.s_minParallelElement1Count
                    && outDims[1] > 1) {
                        if (outDims[1] > workItemCount) {
                        workItemLength = outDims[1] / workItemCount;
                    } else {
                        workItemLength = outDims[1] / 2;
                        workItemCount = 2;
                    }
                } else {
                    workItemLength = outDims[1];
                    workItemCount = 1;
                }

                fixed ( double* arrAP = arrA)
                fixed ( double* arrBP = arrB)
                fixed ( double* retArrP = retArr) {

                    for (; i < workItemCount - 1; i++) {
                        Tuple<int, IntPtr, IntPtr, IntPtr> range = new Tuple<int, IntPtr, IntPtr, IntPtr>
                               (workItemLength
                               , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                               , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                               , (IntPtr)(retArrP + i * outDims[0] * workItemLength)); 
                        System.Threading.Interlocked.Increment(ref workerCount);
                        ILThreadPool.QueueUserWorkItem(i, worker, range);
                    }
                    // the last (or may the only) chunk is done right here
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, IntPtr, IntPtr, IntPtr>
                                (outDims[1] - i * workItemLength
                                , (IntPtr)(arrAP + i * workItemMultiplierLenA * workItemLength)
                                , (IntPtr)(arrBP + i * workItemMultiplierLenB * workItemLength)
                                , (IntPtr)(retArrP + i * outDims[0] * workItemLength)));

                    ILThreadPool.Wait4Workers(ref workerCount);
                }
                #endregion

                return new ILRetArray<double>(retStorage);
            //}  // no scopes here! it disables implace operations
        }


#endregion HYCALPER AUTO GENERATED CODE

    }
}