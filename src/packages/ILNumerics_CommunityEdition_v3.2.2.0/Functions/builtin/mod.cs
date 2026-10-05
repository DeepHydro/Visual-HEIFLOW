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




#region HYCALPER AUTO GENERATED CODE

        /// <summary>Modulus of array elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>New array with result of elementwise modulus operation</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<Int64>  mod(ILInArray<Int64> A, ILInArray<Int64> B) {
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
                       
                        return new  ILRetArray<Int64> (new  Int64 [1]{ saturateInt64 (A.GetValue(0)  % (double) B.GetValue(0))}, A.Size);
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
                            return  modEx(A,B); 
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
                                cp[0] =  saturateInt64 (cp[0]  % (double) bp[0]);
                                cp[1] =  saturateInt64 (cp[1]  % (double) bp[1]);
                                cp[2] =  saturateInt64 (cp[2]  % (double) bp[2]);
                                cp[3] =  saturateInt64 (cp[3]  % (double) bp[3]);
                                cp[4] =  saturateInt64 (cp[4]  % (double) bp[4]);
                                cp[5] =  saturateInt64 (cp[5]  % (double) bp[5]);
                                cp[6] =  saturateInt64 (cp[6]  % (double) bp[6]);
                                cp[7] =  saturateInt64 (cp[7]  % (double) bp[7]);
                                cp[8] =  saturateInt64 (cp[8]  % (double) bp[8]);
                                cp[9] =  saturateInt64 (cp[9]  % (double) bp[9]);
                                cp[10] =  saturateInt64 (cp[10]  % (double) bp[10]);
                                cp[11] =  saturateInt64 (cp[11]  % (double) bp[11]);
                                cp[12] =  saturateInt64 (cp[12]  % (double) bp[12]);
                                cp[13] =  saturateInt64 (cp[13]  % (double) bp[13]);
                                cp[14] =  saturateInt64 (cp[14]  % (double) bp[14]);
                                cp[15] =  saturateInt64 (cp[15]  % (double) bp[15]);
                                cp[16] =  saturateInt64 (cp[16]  % (double) bp[16]);
                                cp[17] =  saturateInt64 (cp[17]  % (double) bp[17]);
                                cp[18] =  saturateInt64 (cp[18]  % (double) bp[18]);
                                cp[19] =  saturateInt64 (cp[19]  % (double) bp[19]);
                                cp[20] =  saturateInt64 (cp[20]  % (double) bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64 (*cp  % (double) *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           Int64* ap = ((Int64*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt64 (ap[0]  % (double) cp[0]);
                                cp[1] =  saturateInt64 (ap[1]  % (double) cp[1]);
                                cp[2] =  saturateInt64 (ap[2]  % (double) cp[2]);
                                cp[3] =  saturateInt64 (ap[3]  % (double) cp[3]);
                                cp[4] =  saturateInt64 (ap[4]  % (double) cp[4]);
                                cp[5] =  saturateInt64 (ap[5]  % (double) cp[5]);
                                cp[6] =  saturateInt64 (ap[6]  % (double) cp[6]);
                                cp[7] =  saturateInt64 (ap[7]  % (double) cp[7]);
                                cp[8] =  saturateInt64 (ap[8]  % (double) cp[8]);
                                cp[9] =  saturateInt64 (ap[9]  % (double) cp[9]);
                                cp[10] =  saturateInt64 (ap[10]  % (double) cp[10]);
                                cp[11] =  saturateInt64 (ap[11]  % (double) cp[11]);
                                cp[12] =  saturateInt64 (ap[12]  % (double) cp[12]);
                                cp[13] =  saturateInt64 (ap[13]  % (double) cp[13]);
                                cp[14] =  saturateInt64 (ap[14]  % (double) cp[14]);
                                cp[15] =  saturateInt64 (ap[15]  % (double) cp[15]);
                                cp[16] =  saturateInt64 (ap[16]  % (double) cp[16]);
                                cp[17] =  saturateInt64 (ap[17]  % (double) cp[17]);
                                cp[18] =  saturateInt64 (ap[18]  % (double) cp[18]);
                                cp[19] =  saturateInt64 (ap[19]  % (double) cp[19]);
                                cp[20] =  saturateInt64 (ap[20]  % (double) cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64 (*ap  % (double) *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((Int64*)range.Item3 + range.Item1);
                            bp = ((Int64*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt64 (ap[0]  % (double) bp[0]);
                                cp[1] =  saturateInt64 (ap[1]  % (double) bp[1]);
                                cp[2] =  saturateInt64 (ap[2]  % (double) bp[2]);
                                cp[3] =  saturateInt64 (ap[3]  % (double) bp[3]);
                                cp[4] =  saturateInt64 (ap[4]  % (double) bp[4]);
                                cp[5] =  saturateInt64 (ap[5]  % (double) bp[5]);
                                cp[6] =  saturateInt64 (ap[6]  % (double) bp[6]);
                                cp[7] =  saturateInt64 (ap[7]  % (double) bp[7]);
                                cp[8] =  saturateInt64 (ap[8]  % (double) bp[8]);
                                cp[9] =  saturateInt64 (ap[9]  % (double) bp[9]);
                                cp[10] =  saturateInt64 (ap[10]  % (double) bp[10]);
                                cp[11] =  saturateInt64 (ap[11]  % (double) bp[11]);
                                cp[12] =  saturateInt64 (ap[12]  % (double) bp[12]);
                                cp[13] =  saturateInt64 (ap[13]  % (double) bp[13]);
                                cp[14] =  saturateInt64 (ap[14]  % (double) bp[14]);
                                cp[15] =  saturateInt64 (ap[15]  % (double) bp[15]);
                                cp[16] =  saturateInt64 (ap[16]  % (double) bp[16]);
                                cp[17] =  saturateInt64 (ap[17]  % (double) bp[17]);
                                cp[18] =  saturateInt64 (ap[18]  % (double) bp[18]);
                                cp[19] =  saturateInt64 (ap[19]  % (double) bp[19]);
                                cp[20] =  saturateInt64 (ap[20]  % (double) bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64 (*ap  % (double) *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((Int64*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt64 (cp[0]  % (double) scalar);
                                cp[1] =  saturateInt64 (cp[1]  % (double) scalar);
                                cp[2] =  saturateInt64 (cp[2]  % (double) scalar);
                                cp[3] =  saturateInt64 (cp[3]  % (double) scalar);
                                cp[4] =  saturateInt64 (cp[4]  % (double) scalar);
                                cp[5] =  saturateInt64 (cp[5]  % (double) scalar);
                                cp[6] =  saturateInt64 (cp[6]  % (double) scalar);
                                cp[7] =  saturateInt64 (cp[7]  % (double) scalar);
                                cp[8] =  saturateInt64 (cp[8]  % (double) scalar);
                                cp[9] =  saturateInt64 (cp[9]  % (double) scalar);
                                cp[10] =  saturateInt64 (cp[10]  % (double) scalar);
                                cp[11] =  saturateInt64 (cp[11]  % (double) scalar);
                                cp[12] =  saturateInt64 (cp[12]  % (double) scalar);
                                cp[13] =  saturateInt64 (cp[13]  % (double) scalar);
                                cp[14] =  saturateInt64 (cp[14]  % (double) scalar);
                                cp[15] =  saturateInt64 (cp[15]  % (double) scalar);
                                cp[16] =  saturateInt64 (cp[16]  % (double) scalar);
                                cp[17] =  saturateInt64 (cp[17]  % (double) scalar);
                                cp[18] =  saturateInt64 (cp[18]  % (double) scalar);
                                cp[19] =  saturateInt64 (cp[19]  % (double) scalar);
                                cp[20] =  saturateInt64 (cp[20]  % (double) scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64 (*cp  % (double) scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((Int64*)range.Item3 + range.Item1);
                            scalar = *((Int64*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt64 (ap[0]  % (double) scalar);
                                cp[1] =  saturateInt64 (ap[1]  % (double) scalar);
                                cp[2] =  saturateInt64 (ap[2]  % (double) scalar);
                                cp[3] =  saturateInt64 (ap[3]  % (double) scalar);
                                cp[4] =  saturateInt64 (ap[4]  % (double) scalar);
                                cp[5] =  saturateInt64 (ap[5]  % (double) scalar);
                                cp[6] =  saturateInt64 (ap[6]  % (double) scalar);
                                cp[7] =  saturateInt64 (ap[7]  % (double) scalar);
                                cp[8] =  saturateInt64 (ap[8]  % (double) scalar);
                                cp[9] =  saturateInt64 (ap[9]  % (double) scalar);
                                cp[10] =  saturateInt64 (ap[10]  % (double) scalar);
                                cp[11] =  saturateInt64 (ap[11]  % (double) scalar);
                                cp[12] =  saturateInt64 (ap[12]  % (double) scalar);
                                cp[13] =  saturateInt64 (ap[13]  % (double) scalar);
                                cp[14] =  saturateInt64 (ap[14]  % (double) scalar);
                                cp[15] =  saturateInt64 (ap[15]  % (double) scalar);
                                cp[16] =  saturateInt64 (ap[16]  % (double) scalar);
                                cp[17] =  saturateInt64 (ap[17]  % (double) scalar);
                                cp[18] =  saturateInt64 (ap[18]  % (double) scalar);
                                cp[19] =  saturateInt64 (ap[19]  % (double) scalar);
                                cp[20] =  saturateInt64 (ap[20]  % (double) scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64 (*ap  % (double) scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((Int64*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt64 (scalar  % (double) cp[0]);
                                cp[1] =  saturateInt64 (scalar  % (double) cp[1]);
                                cp[2] =  saturateInt64 (scalar  % (double) cp[2]);
                                cp[3] =  saturateInt64 (scalar  % (double) cp[3]);
                                cp[4] =  saturateInt64 (scalar  % (double) cp[4]);
                                cp[5] =  saturateInt64 (scalar  % (double) cp[5]);
                                cp[6] =  saturateInt64 (scalar  % (double) cp[6]);
                                cp[7] =  saturateInt64 (scalar  % (double) cp[7]);
                                cp[8] =  saturateInt64 (scalar  % (double) cp[8]);
                                cp[9] =  saturateInt64 (scalar  % (double) cp[9]);
                                cp[10] =  saturateInt64 (scalar  % (double) cp[10]);
                                cp[11] =  saturateInt64 (scalar  % (double) cp[11]);
                                cp[12] =  saturateInt64 (scalar  % (double) cp[12]);
                                cp[13] =  saturateInt64 (scalar  % (double) cp[13]);
                                cp[14] =  saturateInt64 (scalar  % (double) cp[14]);
                                cp[15] =  saturateInt64 (scalar  % (double) cp[15]);
                                cp[16] =  saturateInt64 (scalar  % (double) cp[16]);
                                cp[17] =  saturateInt64 (scalar  % (double) cp[17]);
                                cp[18] =  saturateInt64 (scalar  % (double) cp[18]);
                                cp[19] =  saturateInt64 (scalar  % (double) cp[19]);
                                cp[20] =  saturateInt64 (scalar  % (double) cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64 (scalar  % (double) *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((Int64*)range.Item3);
                            bp = ((Int64*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt64 (scalar  % (double) bp[0]);
                                cp[1] =  saturateInt64 (scalar  % (double) bp[1]);
                                cp[2] =  saturateInt64 (scalar  % (double) bp[2]);
                                cp[3] =  saturateInt64 (scalar  % (double) bp[3]);
                                cp[4] =  saturateInt64 (scalar  % (double) bp[4]);
                                cp[5] =  saturateInt64 (scalar  % (double) bp[5]);
                                cp[6] =  saturateInt64 (scalar  % (double) bp[6]);
                                cp[7] =  saturateInt64 (scalar  % (double) bp[7]);
                                cp[8] =  saturateInt64 (scalar  % (double) bp[8]);
                                cp[9] =  saturateInt64 (scalar  % (double) bp[9]);
                                cp[10] =  saturateInt64 (scalar  % (double) bp[10]);
                                cp[11] =  saturateInt64 (scalar  % (double) bp[11]);
                                cp[12] =  saturateInt64 (scalar  % (double) bp[12]);
                                cp[13] =  saturateInt64 (scalar  % (double) bp[13]);
                                cp[14] =  saturateInt64 (scalar  % (double) bp[14]);
                                cp[15] =  saturateInt64 (scalar  % (double) bp[15]);
                                cp[16] =  saturateInt64 (scalar  % (double) bp[16]);
                                cp[17] =  saturateInt64 (scalar  % (double) bp[17]);
                                cp[18] =  saturateInt64 (scalar  % (double) bp[18]);
                                cp[19] =  saturateInt64 (scalar  % (double) bp[19]);
                                cp[20] =  saturateInt64 (scalar  % (double) bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt64 (scalar  % (double) *bp);
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

        private static unsafe ILRetArray<Int64>  modEx(ILInArray<Int64> A, ILInArray<Int64> B) {
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
                                        cp[0] =  saturateInt64 (ap[0]  % (double) bp[0]);
                                        cp[1] =  saturateInt64 (ap[1]  % (double) bp[1]);
                                        cp[2] =  saturateInt64 (ap[2]  % (double) bp[2]);
                                        cp[3] =  saturateInt64 (ap[3]  % (double) bp[3]);
                                        cp[4] =  saturateInt64 (ap[4]  % (double) bp[4]);
                                        cp[5] =  saturateInt64 (ap[5]  % (double) bp[5]);
                                        cp[6] =  saturateInt64 (ap[6]  % (double) bp[6]);
                                        cp[7] =  saturateInt64 (ap[7]  % (double) bp[7]);
                                        cp[8] =  saturateInt64 (ap[8]  % (double) bp[8]);
                                        cp[9] =  saturateInt64 (ap[9]  % (double) bp[9]);
                                        cp[10] =  saturateInt64 (ap[10]  % (double) bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  saturateInt64 (*ap++  % (double) *bp++);
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
                                        cp[0] =  saturateInt64 (val  % (double) bp[0]);
                                        cp[1] =  saturateInt64 (val  % (double) bp[1]);
                                        cp[2] =  saturateInt64 (val  % (double) bp[2]);
                                        cp[3] =  saturateInt64 (val  % (double) bp[3]);
                                        cp[4] =  saturateInt64 (val  % (double) bp[4]);
                                        cp[5] =  saturateInt64 (val  % (double) bp[5]);
                                        cp[6] =  saturateInt64 (val  % (double) bp[6]);
                                        cp[7] =  saturateInt64 (val  % (double) bp[7]);
                                        cp[8] =  saturateInt64 (val  % (double) bp[8]);
                                        cp[9] =  saturateInt64 (val  % (double) bp[9]);
                                        cp[10] =  saturateInt64 (val  % (double) bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  saturateInt64 (val  % (double) *bp++);
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
                                        cp[0] =  saturateInt64 (ap[0]  % (double) cp[0]);
                                        cp[1] =  saturateInt64 (ap[1]  % (double) cp[1]);
                                        cp[2] =  saturateInt64 (ap[2]  % (double) cp[2]);
                                        cp[3] =  saturateInt64 (ap[3]  % (double) cp[3]);
                                        cp[4] =  saturateInt64 (ap[4]  % (double) cp[4]);
                                        cp[5] =  saturateInt64 (ap[5]  % (double) cp[5]);
                                        cp[6] =  saturateInt64 (ap[6]  % (double) cp[6]);
                                        cp[7] =  saturateInt64 (ap[7]  % (double) cp[7]);
                                        cp[8] =  saturateInt64 (ap[8]  % (double) cp[8]);
                                        cp[9] =  saturateInt64 (ap[9]  % (double) cp[9]);
                                        cp[10] =  saturateInt64 (ap[10]  % (double) cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt64 (*ap++  % (double) *cp);
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
                                        cp[0] =  saturateInt64 (val  % (double) cp[0]);
                                        cp[1] =  saturateInt64 (val  % (double) cp[1]);
                                        cp[2] =  saturateInt64 (val  % (double) cp[2]);
                                        cp[3] =  saturateInt64 (val  % (double) cp[3]);
                                        cp[4] =  saturateInt64 (val  % (double) cp[4]);
                                        cp[5] =  saturateInt64 (val  % (double) cp[5]);
                                        cp[6] =  saturateInt64 (val  % (double) cp[6]);
                                        cp[7] =  saturateInt64 (val  % (double) cp[7]);
                                        cp[8] =  saturateInt64 (val  % (double) cp[8]);
                                        cp[9] =  saturateInt64 (val  % (double) cp[9]);
                                        cp[10] =  saturateInt64 (val  % (double) cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt64 (val  % (double) *cp);
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
                                        cp[0] =  saturateInt64 (ap[0]  % (double) bp[0]);
                                        cp[1] =  saturateInt64 (ap[1]  % (double) bp[1]);
                                        cp[2] =  saturateInt64 (ap[2]  % (double) bp[2]);
                                        cp[3] =  saturateInt64 (ap[3]  % (double) bp[3]);
                                        cp[4] =  saturateInt64 (ap[4]  % (double) bp[4]);
                                        cp[5] =  saturateInt64 (ap[5]  % (double) bp[5]);
                                        cp[6] =  saturateInt64 (ap[6]  % (double) bp[6]);
                                        cp[7] =  saturateInt64 (ap[7]  % (double) bp[7]);
                                        cp[8] =  saturateInt64 (ap[8]  % (double) bp[8]);
                                        cp[9] =  saturateInt64 (ap[9]  % (double) bp[9]);
                                        cp[10] =  saturateInt64 (ap[10]  % (double) bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt64 (*ap  % (double) *bp);
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
                                        cp[0] =  saturateInt64 (ap[0]  % (double) val);
                                        cp[1] =  saturateInt64 (ap[1]  % (double) val);
                                        cp[2] =  saturateInt64 (ap[2]  % (double) val);
                                        cp[3] =  saturateInt64 (ap[3]  % (double) val);
                                        cp[4] =  saturateInt64 (ap[4]  % (double) val);
                                        cp[5] =  saturateInt64 (ap[5]  % (double) val);
                                        cp[6] =  saturateInt64 (ap[6]  % (double) val);
                                        cp[7] =  saturateInt64 (ap[7]  % (double) val);
                                        cp[8] =  saturateInt64 (ap[8]  % (double) val);
                                        cp[9] =  saturateInt64 (ap[9]  % (double) val);
                                        cp[10] =  saturateInt64 (ap[10]  % (double) val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt64 (*ap  % (double) val);
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
                                        cp[0] =  saturateInt64 (cp[0]  % (double) bp[0]);
                                        cp[1] =  saturateInt64 (cp[1]  % (double) bp[1]);
                                        cp[2] =  saturateInt64 (cp[2]  % (double) bp[2]);
                                        cp[3] =  saturateInt64 (cp[3]  % (double) bp[3]);
                                        cp[4] =  saturateInt64 (cp[4]  % (double) bp[4]);
                                        cp[5] =  saturateInt64 (cp[5]  % (double) bp[5]);
                                        cp[6] =  saturateInt64 (cp[6]  % (double) bp[6]);
                                        cp[7] =  saturateInt64 (cp[7]  % (double) bp[7]);
                                        cp[8] =  saturateInt64 (cp[8]  % (double) bp[8]);
                                        cp[9] =  saturateInt64 (cp[9]  % (double) bp[9]);
                                        cp[10] =  saturateInt64 (cp[10]  % (double) bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt64 (*cp  % (double) *bp);
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
                                        cp[0] =  saturateInt64 (cp[0]  % (double) val);
                                        cp[1] =  saturateInt64 (cp[1]  % (double) val);
                                        cp[2] =  saturateInt64 (cp[2]  % (double) val);
                                        cp[3] =  saturateInt64 (cp[3]  % (double) val);
                                        cp[4] =  saturateInt64 (cp[4]  % (double) val);
                                        cp[5] =  saturateInt64 (cp[5]  % (double) val);
                                        cp[6] =  saturateInt64 (cp[6]  % (double) val);
                                        cp[7] =  saturateInt64 (cp[7]  % (double) val);
                                        cp[8] =  saturateInt64 (cp[8]  % (double) val);
                                        cp[9] =  saturateInt64 (cp[9]  % (double) val);
                                        cp[10] =  saturateInt64 (cp[10]  % (double) val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =  saturateInt64 (*cp  % (double) val);
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

        /// <summary>Modulus of array elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>New array with result of elementwise modulus operation</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<Int32>  mod(ILInArray<Int32> A, ILInArray<Int32> B) {
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
                       
                        return new  ILRetArray<Int32> (new  Int32 [1]{ saturateInt32 (A.GetValue(0)  % (double) B.GetValue(0))}, A.Size);
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
                            return  modEx(A,B); 
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
                                cp[0] =  saturateInt32 (cp[0]  % (double) bp[0]);
                                cp[1] =  saturateInt32 (cp[1]  % (double) bp[1]);
                                cp[2] =  saturateInt32 (cp[2]  % (double) bp[2]);
                                cp[3] =  saturateInt32 (cp[3]  % (double) bp[3]);
                                cp[4] =  saturateInt32 (cp[4]  % (double) bp[4]);
                                cp[5] =  saturateInt32 (cp[5]  % (double) bp[5]);
                                cp[6] =  saturateInt32 (cp[6]  % (double) bp[6]);
                                cp[7] =  saturateInt32 (cp[7]  % (double) bp[7]);
                                cp[8] =  saturateInt32 (cp[8]  % (double) bp[8]);
                                cp[9] =  saturateInt32 (cp[9]  % (double) bp[9]);
                                cp[10] =  saturateInt32 (cp[10]  % (double) bp[10]);
                                cp[11] =  saturateInt32 (cp[11]  % (double) bp[11]);
                                cp[12] =  saturateInt32 (cp[12]  % (double) bp[12]);
                                cp[13] =  saturateInt32 (cp[13]  % (double) bp[13]);
                                cp[14] =  saturateInt32 (cp[14]  % (double) bp[14]);
                                cp[15] =  saturateInt32 (cp[15]  % (double) bp[15]);
                                cp[16] =  saturateInt32 (cp[16]  % (double) bp[16]);
                                cp[17] =  saturateInt32 (cp[17]  % (double) bp[17]);
                                cp[18] =  saturateInt32 (cp[18]  % (double) bp[18]);
                                cp[19] =  saturateInt32 (cp[19]  % (double) bp[19]);
                                cp[20] =  saturateInt32 (cp[20]  % (double) bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32 (*cp  % (double) *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           Int32* ap = ((Int32*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt32 (ap[0]  % (double) cp[0]);
                                cp[1] =  saturateInt32 (ap[1]  % (double) cp[1]);
                                cp[2] =  saturateInt32 (ap[2]  % (double) cp[2]);
                                cp[3] =  saturateInt32 (ap[3]  % (double) cp[3]);
                                cp[4] =  saturateInt32 (ap[4]  % (double) cp[4]);
                                cp[5] =  saturateInt32 (ap[5]  % (double) cp[5]);
                                cp[6] =  saturateInt32 (ap[6]  % (double) cp[6]);
                                cp[7] =  saturateInt32 (ap[7]  % (double) cp[7]);
                                cp[8] =  saturateInt32 (ap[8]  % (double) cp[8]);
                                cp[9] =  saturateInt32 (ap[9]  % (double) cp[9]);
                                cp[10] =  saturateInt32 (ap[10]  % (double) cp[10]);
                                cp[11] =  saturateInt32 (ap[11]  % (double) cp[11]);
                                cp[12] =  saturateInt32 (ap[12]  % (double) cp[12]);
                                cp[13] =  saturateInt32 (ap[13]  % (double) cp[13]);
                                cp[14] =  saturateInt32 (ap[14]  % (double) cp[14]);
                                cp[15] =  saturateInt32 (ap[15]  % (double) cp[15]);
                                cp[16] =  saturateInt32 (ap[16]  % (double) cp[16]);
                                cp[17] =  saturateInt32 (ap[17]  % (double) cp[17]);
                                cp[18] =  saturateInt32 (ap[18]  % (double) cp[18]);
                                cp[19] =  saturateInt32 (ap[19]  % (double) cp[19]);
                                cp[20] =  saturateInt32 (ap[20]  % (double) cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32 (*ap  % (double) *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((Int32*)range.Item3 + range.Item1);
                            bp = ((Int32*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt32 (ap[0]  % (double) bp[0]);
                                cp[1] =  saturateInt32 (ap[1]  % (double) bp[1]);
                                cp[2] =  saturateInt32 (ap[2]  % (double) bp[2]);
                                cp[3] =  saturateInt32 (ap[3]  % (double) bp[3]);
                                cp[4] =  saturateInt32 (ap[4]  % (double) bp[4]);
                                cp[5] =  saturateInt32 (ap[5]  % (double) bp[5]);
                                cp[6] =  saturateInt32 (ap[6]  % (double) bp[6]);
                                cp[7] =  saturateInt32 (ap[7]  % (double) bp[7]);
                                cp[8] =  saturateInt32 (ap[8]  % (double) bp[8]);
                                cp[9] =  saturateInt32 (ap[9]  % (double) bp[9]);
                                cp[10] =  saturateInt32 (ap[10]  % (double) bp[10]);
                                cp[11] =  saturateInt32 (ap[11]  % (double) bp[11]);
                                cp[12] =  saturateInt32 (ap[12]  % (double) bp[12]);
                                cp[13] =  saturateInt32 (ap[13]  % (double) bp[13]);
                                cp[14] =  saturateInt32 (ap[14]  % (double) bp[14]);
                                cp[15] =  saturateInt32 (ap[15]  % (double) bp[15]);
                                cp[16] =  saturateInt32 (ap[16]  % (double) bp[16]);
                                cp[17] =  saturateInt32 (ap[17]  % (double) bp[17]);
                                cp[18] =  saturateInt32 (ap[18]  % (double) bp[18]);
                                cp[19] =  saturateInt32 (ap[19]  % (double) bp[19]);
                                cp[20] =  saturateInt32 (ap[20]  % (double) bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32 (*ap  % (double) *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((Int32*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt32 (cp[0]  % (double) scalar);
                                cp[1] =  saturateInt32 (cp[1]  % (double) scalar);
                                cp[2] =  saturateInt32 (cp[2]  % (double) scalar);
                                cp[3] =  saturateInt32 (cp[3]  % (double) scalar);
                                cp[4] =  saturateInt32 (cp[4]  % (double) scalar);
                                cp[5] =  saturateInt32 (cp[5]  % (double) scalar);
                                cp[6] =  saturateInt32 (cp[6]  % (double) scalar);
                                cp[7] =  saturateInt32 (cp[7]  % (double) scalar);
                                cp[8] =  saturateInt32 (cp[8]  % (double) scalar);
                                cp[9] =  saturateInt32 (cp[9]  % (double) scalar);
                                cp[10] =  saturateInt32 (cp[10]  % (double) scalar);
                                cp[11] =  saturateInt32 (cp[11]  % (double) scalar);
                                cp[12] =  saturateInt32 (cp[12]  % (double) scalar);
                                cp[13] =  saturateInt32 (cp[13]  % (double) scalar);
                                cp[14] =  saturateInt32 (cp[14]  % (double) scalar);
                                cp[15] =  saturateInt32 (cp[15]  % (double) scalar);
                                cp[16] =  saturateInt32 (cp[16]  % (double) scalar);
                                cp[17] =  saturateInt32 (cp[17]  % (double) scalar);
                                cp[18] =  saturateInt32 (cp[18]  % (double) scalar);
                                cp[19] =  saturateInt32 (cp[19]  % (double) scalar);
                                cp[20] =  saturateInt32 (cp[20]  % (double) scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32 (*cp  % (double) scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((Int32*)range.Item3 + range.Item1);
                            scalar = *((Int32*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt32 (ap[0]  % (double) scalar);
                                cp[1] =  saturateInt32 (ap[1]  % (double) scalar);
                                cp[2] =  saturateInt32 (ap[2]  % (double) scalar);
                                cp[3] =  saturateInt32 (ap[3]  % (double) scalar);
                                cp[4] =  saturateInt32 (ap[4]  % (double) scalar);
                                cp[5] =  saturateInt32 (ap[5]  % (double) scalar);
                                cp[6] =  saturateInt32 (ap[6]  % (double) scalar);
                                cp[7] =  saturateInt32 (ap[7]  % (double) scalar);
                                cp[8] =  saturateInt32 (ap[8]  % (double) scalar);
                                cp[9] =  saturateInt32 (ap[9]  % (double) scalar);
                                cp[10] =  saturateInt32 (ap[10]  % (double) scalar);
                                cp[11] =  saturateInt32 (ap[11]  % (double) scalar);
                                cp[12] =  saturateInt32 (ap[12]  % (double) scalar);
                                cp[13] =  saturateInt32 (ap[13]  % (double) scalar);
                                cp[14] =  saturateInt32 (ap[14]  % (double) scalar);
                                cp[15] =  saturateInt32 (ap[15]  % (double) scalar);
                                cp[16] =  saturateInt32 (ap[16]  % (double) scalar);
                                cp[17] =  saturateInt32 (ap[17]  % (double) scalar);
                                cp[18] =  saturateInt32 (ap[18]  % (double) scalar);
                                cp[19] =  saturateInt32 (ap[19]  % (double) scalar);
                                cp[20] =  saturateInt32 (ap[20]  % (double) scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32 (*ap  % (double) scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((Int32*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt32 (scalar  % (double) cp[0]);
                                cp[1] =  saturateInt32 (scalar  % (double) cp[1]);
                                cp[2] =  saturateInt32 (scalar  % (double) cp[2]);
                                cp[3] =  saturateInt32 (scalar  % (double) cp[3]);
                                cp[4] =  saturateInt32 (scalar  % (double) cp[4]);
                                cp[5] =  saturateInt32 (scalar  % (double) cp[5]);
                                cp[6] =  saturateInt32 (scalar  % (double) cp[6]);
                                cp[7] =  saturateInt32 (scalar  % (double) cp[7]);
                                cp[8] =  saturateInt32 (scalar  % (double) cp[8]);
                                cp[9] =  saturateInt32 (scalar  % (double) cp[9]);
                                cp[10] =  saturateInt32 (scalar  % (double) cp[10]);
                                cp[11] =  saturateInt32 (scalar  % (double) cp[11]);
                                cp[12] =  saturateInt32 (scalar  % (double) cp[12]);
                                cp[13] =  saturateInt32 (scalar  % (double) cp[13]);
                                cp[14] =  saturateInt32 (scalar  % (double) cp[14]);
                                cp[15] =  saturateInt32 (scalar  % (double) cp[15]);
                                cp[16] =  saturateInt32 (scalar  % (double) cp[16]);
                                cp[17] =  saturateInt32 (scalar  % (double) cp[17]);
                                cp[18] =  saturateInt32 (scalar  % (double) cp[18]);
                                cp[19] =  saturateInt32 (scalar  % (double) cp[19]);
                                cp[20] =  saturateInt32 (scalar  % (double) cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32 (scalar  % (double) *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((Int32*)range.Item3);
                            bp = ((Int32*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateInt32 (scalar  % (double) bp[0]);
                                cp[1] =  saturateInt32 (scalar  % (double) bp[1]);
                                cp[2] =  saturateInt32 (scalar  % (double) bp[2]);
                                cp[3] =  saturateInt32 (scalar  % (double) bp[3]);
                                cp[4] =  saturateInt32 (scalar  % (double) bp[4]);
                                cp[5] =  saturateInt32 (scalar  % (double) bp[5]);
                                cp[6] =  saturateInt32 (scalar  % (double) bp[6]);
                                cp[7] =  saturateInt32 (scalar  % (double) bp[7]);
                                cp[8] =  saturateInt32 (scalar  % (double) bp[8]);
                                cp[9] =  saturateInt32 (scalar  % (double) bp[9]);
                                cp[10] =  saturateInt32 (scalar  % (double) bp[10]);
                                cp[11] =  saturateInt32 (scalar  % (double) bp[11]);
                                cp[12] =  saturateInt32 (scalar  % (double) bp[12]);
                                cp[13] =  saturateInt32 (scalar  % (double) bp[13]);
                                cp[14] =  saturateInt32 (scalar  % (double) bp[14]);
                                cp[15] =  saturateInt32 (scalar  % (double) bp[15]);
                                cp[16] =  saturateInt32 (scalar  % (double) bp[16]);
                                cp[17] =  saturateInt32 (scalar  % (double) bp[17]);
                                cp[18] =  saturateInt32 (scalar  % (double) bp[18]);
                                cp[19] =  saturateInt32 (scalar  % (double) bp[19]);
                                cp[20] =  saturateInt32 (scalar  % (double) bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateInt32 (scalar  % (double) *bp);
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

        private static unsafe ILRetArray<Int32>  modEx(ILInArray<Int32> A, ILInArray<Int32> B) {
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
                                        cp[0] =  saturateInt32 (ap[0]  % (double) bp[0]);
                                        cp[1] =  saturateInt32 (ap[1]  % (double) bp[1]);
                                        cp[2] =  saturateInt32 (ap[2]  % (double) bp[2]);
                                        cp[3] =  saturateInt32 (ap[3]  % (double) bp[3]);
                                        cp[4] =  saturateInt32 (ap[4]  % (double) bp[4]);
                                        cp[5] =  saturateInt32 (ap[5]  % (double) bp[5]);
                                        cp[6] =  saturateInt32 (ap[6]  % (double) bp[6]);
                                        cp[7] =  saturateInt32 (ap[7]  % (double) bp[7]);
                                        cp[8] =  saturateInt32 (ap[8]  % (double) bp[8]);
                                        cp[9] =  saturateInt32 (ap[9]  % (double) bp[9]);
                                        cp[10] =  saturateInt32 (ap[10]  % (double) bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  saturateInt32 (*ap++  % (double) *bp++);
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
                                        cp[0] =  saturateInt32 (val  % (double) bp[0]);
                                        cp[1] =  saturateInt32 (val  % (double) bp[1]);
                                        cp[2] =  saturateInt32 (val  % (double) bp[2]);
                                        cp[3] =  saturateInt32 (val  % (double) bp[3]);
                                        cp[4] =  saturateInt32 (val  % (double) bp[4]);
                                        cp[5] =  saturateInt32 (val  % (double) bp[5]);
                                        cp[6] =  saturateInt32 (val  % (double) bp[6]);
                                        cp[7] =  saturateInt32 (val  % (double) bp[7]);
                                        cp[8] =  saturateInt32 (val  % (double) bp[8]);
                                        cp[9] =  saturateInt32 (val  % (double) bp[9]);
                                        cp[10] =  saturateInt32 (val  % (double) bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  saturateInt32 (val  % (double) *bp++);
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
                                        cp[0] =  saturateInt32 (ap[0]  % (double) cp[0]);
                                        cp[1] =  saturateInt32 (ap[1]  % (double) cp[1]);
                                        cp[2] =  saturateInt32 (ap[2]  % (double) cp[2]);
                                        cp[3] =  saturateInt32 (ap[3]  % (double) cp[3]);
                                        cp[4] =  saturateInt32 (ap[4]  % (double) cp[4]);
                                        cp[5] =  saturateInt32 (ap[5]  % (double) cp[5]);
                                        cp[6] =  saturateInt32 (ap[6]  % (double) cp[6]);
                                        cp[7] =  saturateInt32 (ap[7]  % (double) cp[7]);
                                        cp[8] =  saturateInt32 (ap[8]  % (double) cp[8]);
                                        cp[9] =  saturateInt32 (ap[9]  % (double) cp[9]);
                                        cp[10] =  saturateInt32 (ap[10]  % (double) cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt32 (*ap++  % (double) *cp);
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
                                        cp[0] =  saturateInt32 (val  % (double) cp[0]);
                                        cp[1] =  saturateInt32 (val  % (double) cp[1]);
                                        cp[2] =  saturateInt32 (val  % (double) cp[2]);
                                        cp[3] =  saturateInt32 (val  % (double) cp[3]);
                                        cp[4] =  saturateInt32 (val  % (double) cp[4]);
                                        cp[5] =  saturateInt32 (val  % (double) cp[5]);
                                        cp[6] =  saturateInt32 (val  % (double) cp[6]);
                                        cp[7] =  saturateInt32 (val  % (double) cp[7]);
                                        cp[8] =  saturateInt32 (val  % (double) cp[8]);
                                        cp[9] =  saturateInt32 (val  % (double) cp[9]);
                                        cp[10] =  saturateInt32 (val  % (double) cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt32 (val  % (double) *cp);
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
                                        cp[0] =  saturateInt32 (ap[0]  % (double) bp[0]);
                                        cp[1] =  saturateInt32 (ap[1]  % (double) bp[1]);
                                        cp[2] =  saturateInt32 (ap[2]  % (double) bp[2]);
                                        cp[3] =  saturateInt32 (ap[3]  % (double) bp[3]);
                                        cp[4] =  saturateInt32 (ap[4]  % (double) bp[4]);
                                        cp[5] =  saturateInt32 (ap[5]  % (double) bp[5]);
                                        cp[6] =  saturateInt32 (ap[6]  % (double) bp[6]);
                                        cp[7] =  saturateInt32 (ap[7]  % (double) bp[7]);
                                        cp[8] =  saturateInt32 (ap[8]  % (double) bp[8]);
                                        cp[9] =  saturateInt32 (ap[9]  % (double) bp[9]);
                                        cp[10] =  saturateInt32 (ap[10]  % (double) bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt32 (*ap  % (double) *bp);
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
                                        cp[0] =  saturateInt32 (ap[0]  % (double) val);
                                        cp[1] =  saturateInt32 (ap[1]  % (double) val);
                                        cp[2] =  saturateInt32 (ap[2]  % (double) val);
                                        cp[3] =  saturateInt32 (ap[3]  % (double) val);
                                        cp[4] =  saturateInt32 (ap[4]  % (double) val);
                                        cp[5] =  saturateInt32 (ap[5]  % (double) val);
                                        cp[6] =  saturateInt32 (ap[6]  % (double) val);
                                        cp[7] =  saturateInt32 (ap[7]  % (double) val);
                                        cp[8] =  saturateInt32 (ap[8]  % (double) val);
                                        cp[9] =  saturateInt32 (ap[9]  % (double) val);
                                        cp[10] =  saturateInt32 (ap[10]  % (double) val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt32 (*ap  % (double) val);
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
                                        cp[0] =  saturateInt32 (cp[0]  % (double) bp[0]);
                                        cp[1] =  saturateInt32 (cp[1]  % (double) bp[1]);
                                        cp[2] =  saturateInt32 (cp[2]  % (double) bp[2]);
                                        cp[3] =  saturateInt32 (cp[3]  % (double) bp[3]);
                                        cp[4] =  saturateInt32 (cp[4]  % (double) bp[4]);
                                        cp[5] =  saturateInt32 (cp[5]  % (double) bp[5]);
                                        cp[6] =  saturateInt32 (cp[6]  % (double) bp[6]);
                                        cp[7] =  saturateInt32 (cp[7]  % (double) bp[7]);
                                        cp[8] =  saturateInt32 (cp[8]  % (double) bp[8]);
                                        cp[9] =  saturateInt32 (cp[9]  % (double) bp[9]);
                                        cp[10] =  saturateInt32 (cp[10]  % (double) bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateInt32 (*cp  % (double) *bp);
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
                                        cp[0] =  saturateInt32 (cp[0]  % (double) val);
                                        cp[1] =  saturateInt32 (cp[1]  % (double) val);
                                        cp[2] =  saturateInt32 (cp[2]  % (double) val);
                                        cp[3] =  saturateInt32 (cp[3]  % (double) val);
                                        cp[4] =  saturateInt32 (cp[4]  % (double) val);
                                        cp[5] =  saturateInt32 (cp[5]  % (double) val);
                                        cp[6] =  saturateInt32 (cp[6]  % (double) val);
                                        cp[7] =  saturateInt32 (cp[7]  % (double) val);
                                        cp[8] =  saturateInt32 (cp[8]  % (double) val);
                                        cp[9] =  saturateInt32 (cp[9]  % (double) val);
                                        cp[10] =  saturateInt32 (cp[10]  % (double) val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =  saturateInt32 (*cp  % (double) val);
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

        /// <summary>Modulus of array elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>New array with result of elementwise modulus operation</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<float>  mod(ILInArray<float> A, ILInArray<float> B) {
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
                       
                        return new  ILRetArray<float> (new  float [1]{  (A.GetValue(0)  % B.GetValue(0))}, A.Size);
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
                            return  modEx(A,B); 
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
                                cp[0] =   (cp[0]  % bp[0]);
                                cp[1] =   (cp[1]  % bp[1]);
                                cp[2] =   (cp[2]  % bp[2]);
                                cp[3] =   (cp[3]  % bp[3]);
                                cp[4] =   (cp[4]  % bp[4]);
                                cp[5] =   (cp[5]  % bp[5]);
                                cp[6] =   (cp[6]  % bp[6]);
                                cp[7] =   (cp[7]  % bp[7]);
                                cp[8] =   (cp[8]  % bp[8]);
                                cp[9] =   (cp[9]  % bp[9]);
                                cp[10] =   (cp[10]  % bp[10]);
                                cp[11] =   (cp[11]  % bp[11]);
                                cp[12] =   (cp[12]  % bp[12]);
                                cp[13] =   (cp[13]  % bp[13]);
                                cp[14] =   (cp[14]  % bp[14]);
                                cp[15] =   (cp[15]  % bp[15]);
                                cp[16] =   (cp[16]  % bp[16]);
                                cp[17] =   (cp[17]  % bp[17]);
                                cp[18] =   (cp[18]  % bp[18]);
                                cp[19] =   (cp[19]  % bp[19]);
                                cp[20] =   (cp[20]  % bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (*cp  % *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           float* ap = ((float*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =   (ap[0]  % cp[0]);
                                cp[1] =   (ap[1]  % cp[1]);
                                cp[2] =   (ap[2]  % cp[2]);
                                cp[3] =   (ap[3]  % cp[3]);
                                cp[4] =   (ap[4]  % cp[4]);
                                cp[5] =   (ap[5]  % cp[5]);
                                cp[6] =   (ap[6]  % cp[6]);
                                cp[7] =   (ap[7]  % cp[7]);
                                cp[8] =   (ap[8]  % cp[8]);
                                cp[9] =   (ap[9]  % cp[9]);
                                cp[10] =   (ap[10]  % cp[10]);
                                cp[11] =   (ap[11]  % cp[11]);
                                cp[12] =   (ap[12]  % cp[12]);
                                cp[13] =   (ap[13]  % cp[13]);
                                cp[14] =   (ap[14]  % cp[14]);
                                cp[15] =   (ap[15]  % cp[15]);
                                cp[16] =   (ap[16]  % cp[16]);
                                cp[17] =   (ap[17]  % cp[17]);
                                cp[18] =   (ap[18]  % cp[18]);
                                cp[19] =   (ap[19]  % cp[19]);
                                cp[20] =   (ap[20]  % cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (*ap  % *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((float*)range.Item3 + range.Item1);
                            bp = ((float*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =   (ap[0]  % bp[0]);
                                cp[1] =   (ap[1]  % bp[1]);
                                cp[2] =   (ap[2]  % bp[2]);
                                cp[3] =   (ap[3]  % bp[3]);
                                cp[4] =   (ap[4]  % bp[4]);
                                cp[5] =   (ap[5]  % bp[5]);
                                cp[6] =   (ap[6]  % bp[6]);
                                cp[7] =   (ap[7]  % bp[7]);
                                cp[8] =   (ap[8]  % bp[8]);
                                cp[9] =   (ap[9]  % bp[9]);
                                cp[10] =   (ap[10]  % bp[10]);
                                cp[11] =   (ap[11]  % bp[11]);
                                cp[12] =   (ap[12]  % bp[12]);
                                cp[13] =   (ap[13]  % bp[13]);
                                cp[14] =   (ap[14]  % bp[14]);
                                cp[15] =   (ap[15]  % bp[15]);
                                cp[16] =   (ap[16]  % bp[16]);
                                cp[17] =   (ap[17]  % bp[17]);
                                cp[18] =   (ap[18]  % bp[18]);
                                cp[19] =   (ap[19]  % bp[19]);
                                cp[20] =   (ap[20]  % bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =   (*ap  % *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((float*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =   (cp[0]  % scalar);
                                cp[1] =   (cp[1]  % scalar);
                                cp[2] =   (cp[2]  % scalar);
                                cp[3] =   (cp[3]  % scalar);
                                cp[4] =   (cp[4]  % scalar);
                                cp[5] =   (cp[5]  % scalar);
                                cp[6] =   (cp[6]  % scalar);
                                cp[7] =   (cp[7]  % scalar);
                                cp[8] =   (cp[8]  % scalar);
                                cp[9] =   (cp[9]  % scalar);
                                cp[10] =   (cp[10]  % scalar);
                                cp[11] =   (cp[11]  % scalar);
                                cp[12] =   (cp[12]  % scalar);
                                cp[13] =   (cp[13]  % scalar);
                                cp[14] =   (cp[14]  % scalar);
                                cp[15] =   (cp[15]  % scalar);
                                cp[16] =   (cp[16]  % scalar);
                                cp[17] =   (cp[17]  % scalar);
                                cp[18] =   (cp[18]  % scalar);
                                cp[19] =   (cp[19]  % scalar);
                                cp[20] =   (cp[20]  % scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (*cp  % scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((float*)range.Item3 + range.Item1);
                            scalar = *((float*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =   (ap[0]  % scalar);
                                cp[1] =   (ap[1]  % scalar);
                                cp[2] =   (ap[2]  % scalar);
                                cp[3] =   (ap[3]  % scalar);
                                cp[4] =   (ap[4]  % scalar);
                                cp[5] =   (ap[5]  % scalar);
                                cp[6] =   (ap[6]  % scalar);
                                cp[7] =   (ap[7]  % scalar);
                                cp[8] =   (ap[8]  % scalar);
                                cp[9] =   (ap[9]  % scalar);
                                cp[10] =   (ap[10]  % scalar);
                                cp[11] =   (ap[11]  % scalar);
                                cp[12] =   (ap[12]  % scalar);
                                cp[13] =   (ap[13]  % scalar);
                                cp[14] =   (ap[14]  % scalar);
                                cp[15] =   (ap[15]  % scalar);
                                cp[16] =   (ap[16]  % scalar);
                                cp[17] =   (ap[17]  % scalar);
                                cp[18] =   (ap[18]  % scalar);
                                cp[19] =   (ap[19]  % scalar);
                                cp[20] =   (ap[20]  % scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (*ap  % scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((float*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =   (scalar  % cp[0]);
                                cp[1] =   (scalar  % cp[1]);
                                cp[2] =   (scalar  % cp[2]);
                                cp[3] =   (scalar  % cp[3]);
                                cp[4] =   (scalar  % cp[4]);
                                cp[5] =   (scalar  % cp[5]);
                                cp[6] =   (scalar  % cp[6]);
                                cp[7] =   (scalar  % cp[7]);
                                cp[8] =   (scalar  % cp[8]);
                                cp[9] =   (scalar  % cp[9]);
                                cp[10] =   (scalar  % cp[10]);
                                cp[11] =   (scalar  % cp[11]);
                                cp[12] =   (scalar  % cp[12]);
                                cp[13] =   (scalar  % cp[13]);
                                cp[14] =   (scalar  % cp[14]);
                                cp[15] =   (scalar  % cp[15]);
                                cp[16] =   (scalar  % cp[16]);
                                cp[17] =   (scalar  % cp[17]);
                                cp[18] =   (scalar  % cp[18]);
                                cp[19] =   (scalar  % cp[19]);
                                cp[20] =   (scalar  % cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (scalar  % *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((float*)range.Item3);
                            bp = ((float*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =   (scalar  % bp[0]);
                                cp[1] =   (scalar  % bp[1]);
                                cp[2] =   (scalar  % bp[2]);
                                cp[3] =   (scalar  % bp[3]);
                                cp[4] =   (scalar  % bp[4]);
                                cp[5] =   (scalar  % bp[5]);
                                cp[6] =   (scalar  % bp[6]);
                                cp[7] =   (scalar  % bp[7]);
                                cp[8] =   (scalar  % bp[8]);
                                cp[9] =   (scalar  % bp[9]);
                                cp[10] =   (scalar  % bp[10]);
                                cp[11] =   (scalar  % bp[11]);
                                cp[12] =   (scalar  % bp[12]);
                                cp[13] =   (scalar  % bp[13]);
                                cp[14] =   (scalar  % bp[14]);
                                cp[15] =   (scalar  % bp[15]);
                                cp[16] =   (scalar  % bp[16]);
                                cp[17] =   (scalar  % bp[17]);
                                cp[18] =   (scalar  % bp[18]);
                                cp[19] =   (scalar  % bp[19]);
                                cp[20] =   (scalar  % bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (scalar  % *bp);
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

        private static unsafe ILRetArray<float>  modEx(ILInArray<float> A, ILInArray<float> B) {
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
                                        cp[0] =   (ap[0]  % bp[0]);
                                        cp[1] =   (ap[1]  % bp[1]);
                                        cp[2] =   (ap[2]  % bp[2]);
                                        cp[3] =   (ap[3]  % bp[3]);
                                        cp[4] =   (ap[4]  % bp[4]);
                                        cp[5] =   (ap[5]  % bp[5]);
                                        cp[6] =   (ap[6]  % bp[6]);
                                        cp[7] =   (ap[7]  % bp[7]);
                                        cp[8] =   (ap[8]  % bp[8]);
                                        cp[9] =   (ap[9]  % bp[9]);
                                        cp[10] =   (ap[10]  % bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =   (*ap++  % *bp++);
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
                                        cp[0] =   (val  % bp[0]);
                                        cp[1] =   (val  % bp[1]);
                                        cp[2] =   (val  % bp[2]);
                                        cp[3] =   (val  % bp[3]);
                                        cp[4] =   (val  % bp[4]);
                                        cp[5] =   (val  % bp[5]);
                                        cp[6] =   (val  % bp[6]);
                                        cp[7] =   (val  % bp[7]);
                                        cp[8] =   (val  % bp[8]);
                                        cp[9] =   (val  % bp[9]);
                                        cp[10] =   (val  % bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =   (val  % *bp++);
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
                                        cp[0] =   (ap[0]  % cp[0]);
                                        cp[1] =   (ap[1]  % cp[1]);
                                        cp[2] =   (ap[2]  % cp[2]);
                                        cp[3] =   (ap[3]  % cp[3]);
                                        cp[4] =   (ap[4]  % cp[4]);
                                        cp[5] =   (ap[5]  % cp[5]);
                                        cp[6] =   (ap[6]  % cp[6]);
                                        cp[7] =   (ap[7]  % cp[7]);
                                        cp[8] =   (ap[8]  % cp[8]);
                                        cp[9] =   (ap[9]  % cp[9]);
                                        cp[10] =   (ap[10]  % cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (*ap++  % *cp);
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
                                        cp[0] =   (val  % cp[0]);
                                        cp[1] =   (val  % cp[1]);
                                        cp[2] =   (val  % cp[2]);
                                        cp[3] =   (val  % cp[3]);
                                        cp[4] =   (val  % cp[4]);
                                        cp[5] =   (val  % cp[5]);
                                        cp[6] =   (val  % cp[6]);
                                        cp[7] =   (val  % cp[7]);
                                        cp[8] =   (val  % cp[8]);
                                        cp[9] =   (val  % cp[9]);
                                        cp[10] =   (val  % cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (val  % *cp);
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
                                        cp[0] =   (ap[0]  % bp[0]);
                                        cp[1] =   (ap[1]  % bp[1]);
                                        cp[2] =   (ap[2]  % bp[2]);
                                        cp[3] =   (ap[3]  % bp[3]);
                                        cp[4] =   (ap[4]  % bp[4]);
                                        cp[5] =   (ap[5]  % bp[5]);
                                        cp[6] =   (ap[6]  % bp[6]);
                                        cp[7] =   (ap[7]  % bp[7]);
                                        cp[8] =   (ap[8]  % bp[8]);
                                        cp[9] =   (ap[9]  % bp[9]);
                                        cp[10] =   (ap[10]  % bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (*ap  % *bp);
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
                                        cp[0] =   (ap[0]  % val);
                                        cp[1] =   (ap[1]  % val);
                                        cp[2] =   (ap[2]  % val);
                                        cp[3] =   (ap[3]  % val);
                                        cp[4] =   (ap[4]  % val);
                                        cp[5] =   (ap[5]  % val);
                                        cp[6] =   (ap[6]  % val);
                                        cp[7] =   (ap[7]  % val);
                                        cp[8] =   (ap[8]  % val);
                                        cp[9] =   (ap[9]  % val);
                                        cp[10] =   (ap[10]  % val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (*ap  % val);
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
                                        cp[0] =   (cp[0]  % bp[0]);
                                        cp[1] =   (cp[1]  % bp[1]);
                                        cp[2] =   (cp[2]  % bp[2]);
                                        cp[3] =   (cp[3]  % bp[3]);
                                        cp[4] =   (cp[4]  % bp[4]);
                                        cp[5] =   (cp[5]  % bp[5]);
                                        cp[6] =   (cp[6]  % bp[6]);
                                        cp[7] =   (cp[7]  % bp[7]);
                                        cp[8] =   (cp[8]  % bp[8]);
                                        cp[9] =   (cp[9]  % bp[9]);
                                        cp[10] =   (cp[10]  % bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (*cp  % *bp);
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
                                        cp[0] =   (cp[0]  % val);
                                        cp[1] =   (cp[1]  % val);
                                        cp[2] =   (cp[2]  % val);
                                        cp[3] =   (cp[3]  % val);
                                        cp[4] =   (cp[4]  % val);
                                        cp[5] =   (cp[5]  % val);
                                        cp[6] =   (cp[6]  % val);
                                        cp[7] =   (cp[7]  % val);
                                        cp[8] =   (cp[8]  % val);
                                        cp[9] =   (cp[9]  % val);
                                        cp[10] =   (cp[10]  % val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =   (*cp  % val);
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

        /// <summary>Modulus of array elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>New array with result of elementwise modulus operation</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<byte>  mod(ILInArray<byte> A, ILInArray<byte> B) {
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
                       
                        return new  ILRetArray<byte> (new  byte [1]{ saturateByte (A.GetValue(0)  % (double) B.GetValue(0))}, A.Size);
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
                            return  modEx(A,B); 
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
                                cp[0] =  saturateByte (cp[0]  % (double) bp[0]);
                                cp[1] =  saturateByte (cp[1]  % (double) bp[1]);
                                cp[2] =  saturateByte (cp[2]  % (double) bp[2]);
                                cp[3] =  saturateByte (cp[3]  % (double) bp[3]);
                                cp[4] =  saturateByte (cp[4]  % (double) bp[4]);
                                cp[5] =  saturateByte (cp[5]  % (double) bp[5]);
                                cp[6] =  saturateByte (cp[6]  % (double) bp[6]);
                                cp[7] =  saturateByte (cp[7]  % (double) bp[7]);
                                cp[8] =  saturateByte (cp[8]  % (double) bp[8]);
                                cp[9] =  saturateByte (cp[9]  % (double) bp[9]);
                                cp[10] =  saturateByte (cp[10]  % (double) bp[10]);
                                cp[11] =  saturateByte (cp[11]  % (double) bp[11]);
                                cp[12] =  saturateByte (cp[12]  % (double) bp[12]);
                                cp[13] =  saturateByte (cp[13]  % (double) bp[13]);
                                cp[14] =  saturateByte (cp[14]  % (double) bp[14]);
                                cp[15] =  saturateByte (cp[15]  % (double) bp[15]);
                                cp[16] =  saturateByte (cp[16]  % (double) bp[16]);
                                cp[17] =  saturateByte (cp[17]  % (double) bp[17]);
                                cp[18] =  saturateByte (cp[18]  % (double) bp[18]);
                                cp[19] =  saturateByte (cp[19]  % (double) bp[19]);
                                cp[20] =  saturateByte (cp[20]  % (double) bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateByte (*cp  % (double) *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           byte* ap = ((byte*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateByte (ap[0]  % (double) cp[0]);
                                cp[1] =  saturateByte (ap[1]  % (double) cp[1]);
                                cp[2] =  saturateByte (ap[2]  % (double) cp[2]);
                                cp[3] =  saturateByte (ap[3]  % (double) cp[3]);
                                cp[4] =  saturateByte (ap[4]  % (double) cp[4]);
                                cp[5] =  saturateByte (ap[5]  % (double) cp[5]);
                                cp[6] =  saturateByte (ap[6]  % (double) cp[6]);
                                cp[7] =  saturateByte (ap[7]  % (double) cp[7]);
                                cp[8] =  saturateByte (ap[8]  % (double) cp[8]);
                                cp[9] =  saturateByte (ap[9]  % (double) cp[9]);
                                cp[10] =  saturateByte (ap[10]  % (double) cp[10]);
                                cp[11] =  saturateByte (ap[11]  % (double) cp[11]);
                                cp[12] =  saturateByte (ap[12]  % (double) cp[12]);
                                cp[13] =  saturateByte (ap[13]  % (double) cp[13]);
                                cp[14] =  saturateByte (ap[14]  % (double) cp[14]);
                                cp[15] =  saturateByte (ap[15]  % (double) cp[15]);
                                cp[16] =  saturateByte (ap[16]  % (double) cp[16]);
                                cp[17] =  saturateByte (ap[17]  % (double) cp[17]);
                                cp[18] =  saturateByte (ap[18]  % (double) cp[18]);
                                cp[19] =  saturateByte (ap[19]  % (double) cp[19]);
                                cp[20] =  saturateByte (ap[20]  % (double) cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateByte (*ap  % (double) *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((byte*)range.Item3 + range.Item1);
                            bp = ((byte*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateByte (ap[0]  % (double) bp[0]);
                                cp[1] =  saturateByte (ap[1]  % (double) bp[1]);
                                cp[2] =  saturateByte (ap[2]  % (double) bp[2]);
                                cp[3] =  saturateByte (ap[3]  % (double) bp[3]);
                                cp[4] =  saturateByte (ap[4]  % (double) bp[4]);
                                cp[5] =  saturateByte (ap[5]  % (double) bp[5]);
                                cp[6] =  saturateByte (ap[6]  % (double) bp[6]);
                                cp[7] =  saturateByte (ap[7]  % (double) bp[7]);
                                cp[8] =  saturateByte (ap[8]  % (double) bp[8]);
                                cp[9] =  saturateByte (ap[9]  % (double) bp[9]);
                                cp[10] =  saturateByte (ap[10]  % (double) bp[10]);
                                cp[11] =  saturateByte (ap[11]  % (double) bp[11]);
                                cp[12] =  saturateByte (ap[12]  % (double) bp[12]);
                                cp[13] =  saturateByte (ap[13]  % (double) bp[13]);
                                cp[14] =  saturateByte (ap[14]  % (double) bp[14]);
                                cp[15] =  saturateByte (ap[15]  % (double) bp[15]);
                                cp[16] =  saturateByte (ap[16]  % (double) bp[16]);
                                cp[17] =  saturateByte (ap[17]  % (double) bp[17]);
                                cp[18] =  saturateByte (ap[18]  % (double) bp[18]);
                                cp[19] =  saturateByte (ap[19]  % (double) bp[19]);
                                cp[20] =  saturateByte (ap[20]  % (double) bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateByte (*ap  % (double) *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((byte*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  saturateByte (cp[0]  % (double) scalar);
                                cp[1] =  saturateByte (cp[1]  % (double) scalar);
                                cp[2] =  saturateByte (cp[2]  % (double) scalar);
                                cp[3] =  saturateByte (cp[3]  % (double) scalar);
                                cp[4] =  saturateByte (cp[4]  % (double) scalar);
                                cp[5] =  saturateByte (cp[5]  % (double) scalar);
                                cp[6] =  saturateByte (cp[6]  % (double) scalar);
                                cp[7] =  saturateByte (cp[7]  % (double) scalar);
                                cp[8] =  saturateByte (cp[8]  % (double) scalar);
                                cp[9] =  saturateByte (cp[9]  % (double) scalar);
                                cp[10] =  saturateByte (cp[10]  % (double) scalar);
                                cp[11] =  saturateByte (cp[11]  % (double) scalar);
                                cp[12] =  saturateByte (cp[12]  % (double) scalar);
                                cp[13] =  saturateByte (cp[13]  % (double) scalar);
                                cp[14] =  saturateByte (cp[14]  % (double) scalar);
                                cp[15] =  saturateByte (cp[15]  % (double) scalar);
                                cp[16] =  saturateByte (cp[16]  % (double) scalar);
                                cp[17] =  saturateByte (cp[17]  % (double) scalar);
                                cp[18] =  saturateByte (cp[18]  % (double) scalar);
                                cp[19] =  saturateByte (cp[19]  % (double) scalar);
                                cp[20] =  saturateByte (cp[20]  % (double) scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateByte (*cp  % (double) scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((byte*)range.Item3 + range.Item1);
                            scalar = *((byte*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =  saturateByte (ap[0]  % (double) scalar);
                                cp[1] =  saturateByte (ap[1]  % (double) scalar);
                                cp[2] =  saturateByte (ap[2]  % (double) scalar);
                                cp[3] =  saturateByte (ap[3]  % (double) scalar);
                                cp[4] =  saturateByte (ap[4]  % (double) scalar);
                                cp[5] =  saturateByte (ap[5]  % (double) scalar);
                                cp[6] =  saturateByte (ap[6]  % (double) scalar);
                                cp[7] =  saturateByte (ap[7]  % (double) scalar);
                                cp[8] =  saturateByte (ap[8]  % (double) scalar);
                                cp[9] =  saturateByte (ap[9]  % (double) scalar);
                                cp[10] =  saturateByte (ap[10]  % (double) scalar);
                                cp[11] =  saturateByte (ap[11]  % (double) scalar);
                                cp[12] =  saturateByte (ap[12]  % (double) scalar);
                                cp[13] =  saturateByte (ap[13]  % (double) scalar);
                                cp[14] =  saturateByte (ap[14]  % (double) scalar);
                                cp[15] =  saturateByte (ap[15]  % (double) scalar);
                                cp[16] =  saturateByte (ap[16]  % (double) scalar);
                                cp[17] =  saturateByte (ap[17]  % (double) scalar);
                                cp[18] =  saturateByte (ap[18]  % (double) scalar);
                                cp[19] =  saturateByte (ap[19]  % (double) scalar);
                                cp[20] =  saturateByte (ap[20]  % (double) scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateByte (*ap  % (double) scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((byte*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =  saturateByte (scalar  % (double) cp[0]);
                                cp[1] =  saturateByte (scalar  % (double) cp[1]);
                                cp[2] =  saturateByte (scalar  % (double) cp[2]);
                                cp[3] =  saturateByte (scalar  % (double) cp[3]);
                                cp[4] =  saturateByte (scalar  % (double) cp[4]);
                                cp[5] =  saturateByte (scalar  % (double) cp[5]);
                                cp[6] =  saturateByte (scalar  % (double) cp[6]);
                                cp[7] =  saturateByte (scalar  % (double) cp[7]);
                                cp[8] =  saturateByte (scalar  % (double) cp[8]);
                                cp[9] =  saturateByte (scalar  % (double) cp[9]);
                                cp[10] =  saturateByte (scalar  % (double) cp[10]);
                                cp[11] =  saturateByte (scalar  % (double) cp[11]);
                                cp[12] =  saturateByte (scalar  % (double) cp[12]);
                                cp[13] =  saturateByte (scalar  % (double) cp[13]);
                                cp[14] =  saturateByte (scalar  % (double) cp[14]);
                                cp[15] =  saturateByte (scalar  % (double) cp[15]);
                                cp[16] =  saturateByte (scalar  % (double) cp[16]);
                                cp[17] =  saturateByte (scalar  % (double) cp[17]);
                                cp[18] =  saturateByte (scalar  % (double) cp[18]);
                                cp[19] =  saturateByte (scalar  % (double) cp[19]);
                                cp[20] =  saturateByte (scalar  % (double) cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateByte (scalar  % (double) *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((byte*)range.Item3);
                            bp = ((byte*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =  saturateByte (scalar  % (double) bp[0]);
                                cp[1] =  saturateByte (scalar  % (double) bp[1]);
                                cp[2] =  saturateByte (scalar  % (double) bp[2]);
                                cp[3] =  saturateByte (scalar  % (double) bp[3]);
                                cp[4] =  saturateByte (scalar  % (double) bp[4]);
                                cp[5] =  saturateByte (scalar  % (double) bp[5]);
                                cp[6] =  saturateByte (scalar  % (double) bp[6]);
                                cp[7] =  saturateByte (scalar  % (double) bp[7]);
                                cp[8] =  saturateByte (scalar  % (double) bp[8]);
                                cp[9] =  saturateByte (scalar  % (double) bp[9]);
                                cp[10] =  saturateByte (scalar  % (double) bp[10]);
                                cp[11] =  saturateByte (scalar  % (double) bp[11]);
                                cp[12] =  saturateByte (scalar  % (double) bp[12]);
                                cp[13] =  saturateByte (scalar  % (double) bp[13]);
                                cp[14] =  saturateByte (scalar  % (double) bp[14]);
                                cp[15] =  saturateByte (scalar  % (double) bp[15]);
                                cp[16] =  saturateByte (scalar  % (double) bp[16]);
                                cp[17] =  saturateByte (scalar  % (double) bp[17]);
                                cp[18] =  saturateByte (scalar  % (double) bp[18]);
                                cp[19] =  saturateByte (scalar  % (double) bp[19]);
                                cp[20] =  saturateByte (scalar  % (double) bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =  saturateByte (scalar  % (double) *bp);
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

        private static unsafe ILRetArray<byte>  modEx(ILInArray<byte> A, ILInArray<byte> B) {
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
                                        cp[0] =  saturateByte (ap[0]  % (double) bp[0]);
                                        cp[1] =  saturateByte (ap[1]  % (double) bp[1]);
                                        cp[2] =  saturateByte (ap[2]  % (double) bp[2]);
                                        cp[3] =  saturateByte (ap[3]  % (double) bp[3]);
                                        cp[4] =  saturateByte (ap[4]  % (double) bp[4]);
                                        cp[5] =  saturateByte (ap[5]  % (double) bp[5]);
                                        cp[6] =  saturateByte (ap[6]  % (double) bp[6]);
                                        cp[7] =  saturateByte (ap[7]  % (double) bp[7]);
                                        cp[8] =  saturateByte (ap[8]  % (double) bp[8]);
                                        cp[9] =  saturateByte (ap[9]  % (double) bp[9]);
                                        cp[10] =  saturateByte (ap[10]  % (double) bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  saturateByte (*ap++  % (double) *bp++);
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
                                        cp[0] =  saturateByte (val  % (double) bp[0]);
                                        cp[1] =  saturateByte (val  % (double) bp[1]);
                                        cp[2] =  saturateByte (val  % (double) bp[2]);
                                        cp[3] =  saturateByte (val  % (double) bp[3]);
                                        cp[4] =  saturateByte (val  % (double) bp[4]);
                                        cp[5] =  saturateByte (val  % (double) bp[5]);
                                        cp[6] =  saturateByte (val  % (double) bp[6]);
                                        cp[7] =  saturateByte (val  % (double) bp[7]);
                                        cp[8] =  saturateByte (val  % (double) bp[8]);
                                        cp[9] =  saturateByte (val  % (double) bp[9]);
                                        cp[10] =  saturateByte (val  % (double) bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =  saturateByte (val  % (double) *bp++);
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
                                        cp[0] =  saturateByte (ap[0]  % (double) cp[0]);
                                        cp[1] =  saturateByte (ap[1]  % (double) cp[1]);
                                        cp[2] =  saturateByte (ap[2]  % (double) cp[2]);
                                        cp[3] =  saturateByte (ap[3]  % (double) cp[3]);
                                        cp[4] =  saturateByte (ap[4]  % (double) cp[4]);
                                        cp[5] =  saturateByte (ap[5]  % (double) cp[5]);
                                        cp[6] =  saturateByte (ap[6]  % (double) cp[6]);
                                        cp[7] =  saturateByte (ap[7]  % (double) cp[7]);
                                        cp[8] =  saturateByte (ap[8]  % (double) cp[8]);
                                        cp[9] =  saturateByte (ap[9]  % (double) cp[9]);
                                        cp[10] =  saturateByte (ap[10]  % (double) cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateByte (*ap++  % (double) *cp);
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
                                        cp[0] =  saturateByte (val  % (double) cp[0]);
                                        cp[1] =  saturateByte (val  % (double) cp[1]);
                                        cp[2] =  saturateByte (val  % (double) cp[2]);
                                        cp[3] =  saturateByte (val  % (double) cp[3]);
                                        cp[4] =  saturateByte (val  % (double) cp[4]);
                                        cp[5] =  saturateByte (val  % (double) cp[5]);
                                        cp[6] =  saturateByte (val  % (double) cp[6]);
                                        cp[7] =  saturateByte (val  % (double) cp[7]);
                                        cp[8] =  saturateByte (val  % (double) cp[8]);
                                        cp[9] =  saturateByte (val  % (double) cp[9]);
                                        cp[10] =  saturateByte (val  % (double) cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateByte (val  % (double) *cp);
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
                                        cp[0] =  saturateByte (ap[0]  % (double) bp[0]);
                                        cp[1] =  saturateByte (ap[1]  % (double) bp[1]);
                                        cp[2] =  saturateByte (ap[2]  % (double) bp[2]);
                                        cp[3] =  saturateByte (ap[3]  % (double) bp[3]);
                                        cp[4] =  saturateByte (ap[4]  % (double) bp[4]);
                                        cp[5] =  saturateByte (ap[5]  % (double) bp[5]);
                                        cp[6] =  saturateByte (ap[6]  % (double) bp[6]);
                                        cp[7] =  saturateByte (ap[7]  % (double) bp[7]);
                                        cp[8] =  saturateByte (ap[8]  % (double) bp[8]);
                                        cp[9] =  saturateByte (ap[9]  % (double) bp[9]);
                                        cp[10] =  saturateByte (ap[10]  % (double) bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateByte (*ap  % (double) *bp);
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
                                        cp[0] =  saturateByte (ap[0]  % (double) val);
                                        cp[1] =  saturateByte (ap[1]  % (double) val);
                                        cp[2] =  saturateByte (ap[2]  % (double) val);
                                        cp[3] =  saturateByte (ap[3]  % (double) val);
                                        cp[4] =  saturateByte (ap[4]  % (double) val);
                                        cp[5] =  saturateByte (ap[5]  % (double) val);
                                        cp[6] =  saturateByte (ap[6]  % (double) val);
                                        cp[7] =  saturateByte (ap[7]  % (double) val);
                                        cp[8] =  saturateByte (ap[8]  % (double) val);
                                        cp[9] =  saturateByte (ap[9]  % (double) val);
                                        cp[10] =  saturateByte (ap[10]  % (double) val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateByte (*ap  % (double) val);
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
                                        cp[0] =  saturateByte (cp[0]  % (double) bp[0]);
                                        cp[1] =  saturateByte (cp[1]  % (double) bp[1]);
                                        cp[2] =  saturateByte (cp[2]  % (double) bp[2]);
                                        cp[3] =  saturateByte (cp[3]  % (double) bp[3]);
                                        cp[4] =  saturateByte (cp[4]  % (double) bp[4]);
                                        cp[5] =  saturateByte (cp[5]  % (double) bp[5]);
                                        cp[6] =  saturateByte (cp[6]  % (double) bp[6]);
                                        cp[7] =  saturateByte (cp[7]  % (double) bp[7]);
                                        cp[8] =  saturateByte (cp[8]  % (double) bp[8]);
                                        cp[9] =  saturateByte (cp[9]  % (double) bp[9]);
                                        cp[10] =  saturateByte (cp[10]  % (double) bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =  saturateByte (*cp  % (double) *bp);
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
                                        cp[0] =  saturateByte (cp[0]  % (double) val);
                                        cp[1] =  saturateByte (cp[1]  % (double) val);
                                        cp[2] =  saturateByte (cp[2]  % (double) val);
                                        cp[3] =  saturateByte (cp[3]  % (double) val);
                                        cp[4] =  saturateByte (cp[4]  % (double) val);
                                        cp[5] =  saturateByte (cp[5]  % (double) val);
                                        cp[6] =  saturateByte (cp[6]  % (double) val);
                                        cp[7] =  saturateByte (cp[7]  % (double) val);
                                        cp[8] =  saturateByte (cp[8]  % (double) val);
                                        cp[9] =  saturateByte (cp[9]  % (double) val);
                                        cp[10] =  saturateByte (cp[10]  % (double) val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =  saturateByte (*cp  % (double) val);
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

        /// <summary>Modulus of array elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>New array with result of elementwise modulus operation</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<double>  mod(ILInArray<double> A, ILInArray<double> B) {
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
                       
                        return new  ILRetArray<double> (new  double [1]{  (A.GetValue(0)  % B.GetValue(0))}, A.Size);
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
                            return  modEx(A,B); 
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
                                cp[0] =   (cp[0]  % bp[0]);
                                cp[1] =   (cp[1]  % bp[1]);
                                cp[2] =   (cp[2]  % bp[2]);
                                cp[3] =   (cp[3]  % bp[3]);
                                cp[4] =   (cp[4]  % bp[4]);
                                cp[5] =   (cp[5]  % bp[5]);
                                cp[6] =   (cp[6]  % bp[6]);
                                cp[7] =   (cp[7]  % bp[7]);
                                cp[8] =   (cp[8]  % bp[8]);
                                cp[9] =   (cp[9]  % bp[9]);
                                cp[10] =   (cp[10]  % bp[10]);
                                cp[11] =   (cp[11]  % bp[11]);
                                cp[12] =   (cp[12]  % bp[12]);
                                cp[13] =   (cp[13]  % bp[13]);
                                cp[14] =   (cp[14]  % bp[14]);
                                cp[15] =   (cp[15]  % bp[15]);
                                cp[16] =   (cp[16]  % bp[16]);
                                cp[17] =   (cp[17]  % bp[17]);
                                cp[18] =   (cp[18]  % bp[18]);
                                cp[19] =   (cp[19]  % bp[19]);
                                cp[20] =   (cp[20]  % bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (*cp  % *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           double* ap = ((double*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =   (ap[0]  % cp[0]);
                                cp[1] =   (ap[1]  % cp[1]);
                                cp[2] =   (ap[2]  % cp[2]);
                                cp[3] =   (ap[3]  % cp[3]);
                                cp[4] =   (ap[4]  % cp[4]);
                                cp[5] =   (ap[5]  % cp[5]);
                                cp[6] =   (ap[6]  % cp[6]);
                                cp[7] =   (ap[7]  % cp[7]);
                                cp[8] =   (ap[8]  % cp[8]);
                                cp[9] =   (ap[9]  % cp[9]);
                                cp[10] =   (ap[10]  % cp[10]);
                                cp[11] =   (ap[11]  % cp[11]);
                                cp[12] =   (ap[12]  % cp[12]);
                                cp[13] =   (ap[13]  % cp[13]);
                                cp[14] =   (ap[14]  % cp[14]);
                                cp[15] =   (ap[15]  % cp[15]);
                                cp[16] =   (ap[16]  % cp[16]);
                                cp[17] =   (ap[17]  % cp[17]);
                                cp[18] =   (ap[18]  % cp[18]);
                                cp[19] =   (ap[19]  % cp[19]);
                                cp[20] =   (ap[20]  % cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (*ap  % *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((double*)range.Item3 + range.Item1);
                            bp = ((double*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =   (ap[0]  % bp[0]);
                                cp[1] =   (ap[1]  % bp[1]);
                                cp[2] =   (ap[2]  % bp[2]);
                                cp[3] =   (ap[3]  % bp[3]);
                                cp[4] =   (ap[4]  % bp[4]);
                                cp[5] =   (ap[5]  % bp[5]);
                                cp[6] =   (ap[6]  % bp[6]);
                                cp[7] =   (ap[7]  % bp[7]);
                                cp[8] =   (ap[8]  % bp[8]);
                                cp[9] =   (ap[9]  % bp[9]);
                                cp[10] =   (ap[10]  % bp[10]);
                                cp[11] =   (ap[11]  % bp[11]);
                                cp[12] =   (ap[12]  % bp[12]);
                                cp[13] =   (ap[13]  % bp[13]);
                                cp[14] =   (ap[14]  % bp[14]);
                                cp[15] =   (ap[15]  % bp[15]);
                                cp[16] =   (ap[16]  % bp[16]);
                                cp[17] =   (ap[17]  % bp[17]);
                                cp[18] =   (ap[18]  % bp[18]);
                                cp[19] =   (ap[19]  % bp[19]);
                                cp[20] =   (ap[20]  % bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =   (*ap  % *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((double*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =   (cp[0]  % scalar);
                                cp[1] =   (cp[1]  % scalar);
                                cp[2] =   (cp[2]  % scalar);
                                cp[3] =   (cp[3]  % scalar);
                                cp[4] =   (cp[4]  % scalar);
                                cp[5] =   (cp[5]  % scalar);
                                cp[6] =   (cp[6]  % scalar);
                                cp[7] =   (cp[7]  % scalar);
                                cp[8] =   (cp[8]  % scalar);
                                cp[9] =   (cp[9]  % scalar);
                                cp[10] =   (cp[10]  % scalar);
                                cp[11] =   (cp[11]  % scalar);
                                cp[12] =   (cp[12]  % scalar);
                                cp[13] =   (cp[13]  % scalar);
                                cp[14] =   (cp[14]  % scalar);
                                cp[15] =   (cp[15]  % scalar);
                                cp[16] =   (cp[16]  % scalar);
                                cp[17] =   (cp[17]  % scalar);
                                cp[18] =   (cp[18]  % scalar);
                                cp[19] =   (cp[19]  % scalar);
                                cp[20] =   (cp[20]  % scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (*cp  % scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((double*)range.Item3 + range.Item1);
                            scalar = *((double*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =   (ap[0]  % scalar);
                                cp[1] =   (ap[1]  % scalar);
                                cp[2] =   (ap[2]  % scalar);
                                cp[3] =   (ap[3]  % scalar);
                                cp[4] =   (ap[4]  % scalar);
                                cp[5] =   (ap[5]  % scalar);
                                cp[6] =   (ap[6]  % scalar);
                                cp[7] =   (ap[7]  % scalar);
                                cp[8] =   (ap[8]  % scalar);
                                cp[9] =   (ap[9]  % scalar);
                                cp[10] =   (ap[10]  % scalar);
                                cp[11] =   (ap[11]  % scalar);
                                cp[12] =   (ap[12]  % scalar);
                                cp[13] =   (ap[13]  % scalar);
                                cp[14] =   (ap[14]  % scalar);
                                cp[15] =   (ap[15]  % scalar);
                                cp[16] =   (ap[16]  % scalar);
                                cp[17] =   (ap[17]  % scalar);
                                cp[18] =   (ap[18]  % scalar);
                                cp[19] =   (ap[19]  % scalar);
                                cp[20] =   (ap[20]  % scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (*ap  % scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((double*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =   (scalar  % cp[0]);
                                cp[1] =   (scalar  % cp[1]);
                                cp[2] =   (scalar  % cp[2]);
                                cp[3] =   (scalar  % cp[3]);
                                cp[4] =   (scalar  % cp[4]);
                                cp[5] =   (scalar  % cp[5]);
                                cp[6] =   (scalar  % cp[6]);
                                cp[7] =   (scalar  % cp[7]);
                                cp[8] =   (scalar  % cp[8]);
                                cp[9] =   (scalar  % cp[9]);
                                cp[10] =   (scalar  % cp[10]);
                                cp[11] =   (scalar  % cp[11]);
                                cp[12] =   (scalar  % cp[12]);
                                cp[13] =   (scalar  % cp[13]);
                                cp[14] =   (scalar  % cp[14]);
                                cp[15] =   (scalar  % cp[15]);
                                cp[16] =   (scalar  % cp[16]);
                                cp[17] =   (scalar  % cp[17]);
                                cp[18] =   (scalar  % cp[18]);
                                cp[19] =   (scalar  % cp[19]);
                                cp[20] =   (scalar  % cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (scalar  % *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((double*)range.Item3);
                            bp = ((double*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =   (scalar  % bp[0]);
                                cp[1] =   (scalar  % bp[1]);
                                cp[2] =   (scalar  % bp[2]);
                                cp[3] =   (scalar  % bp[3]);
                                cp[4] =   (scalar  % bp[4]);
                                cp[5] =   (scalar  % bp[5]);
                                cp[6] =   (scalar  % bp[6]);
                                cp[7] =   (scalar  % bp[7]);
                                cp[8] =   (scalar  % bp[8]);
                                cp[9] =   (scalar  % bp[9]);
                                cp[10] =   (scalar  % bp[10]);
                                cp[11] =   (scalar  % bp[11]);
                                cp[12] =   (scalar  % bp[12]);
                                cp[13] =   (scalar  % bp[13]);
                                cp[14] =   (scalar  % bp[14]);
                                cp[15] =   (scalar  % bp[15]);
                                cp[16] =   (scalar  % bp[16]);
                                cp[17] =   (scalar  % bp[17]);
                                cp[18] =   (scalar  % bp[18]);
                                cp[19] =   (scalar  % bp[19]);
                                cp[20] =   (scalar  % bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (scalar  % *bp);
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

        private static unsafe ILRetArray<double>  modEx(ILInArray<double> A, ILInArray<double> B) {
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
                                        cp[0] =   (ap[0]  % bp[0]);
                                        cp[1] =   (ap[1]  % bp[1]);
                                        cp[2] =   (ap[2]  % bp[2]);
                                        cp[3] =   (ap[3]  % bp[3]);
                                        cp[4] =   (ap[4]  % bp[4]);
                                        cp[5] =   (ap[5]  % bp[5]);
                                        cp[6] =   (ap[6]  % bp[6]);
                                        cp[7] =   (ap[7]  % bp[7]);
                                        cp[8] =   (ap[8]  % bp[8]);
                                        cp[9] =   (ap[9]  % bp[9]);
                                        cp[10] =   (ap[10]  % bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =   (*ap++  % *bp++);
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
                                        cp[0] =   (val  % bp[0]);
                                        cp[1] =   (val  % bp[1]);
                                        cp[2] =   (val  % bp[2]);
                                        cp[3] =   (val  % bp[3]);
                                        cp[4] =   (val  % bp[4]);
                                        cp[5] =   (val  % bp[5]);
                                        cp[6] =   (val  % bp[6]);
                                        cp[7] =   (val  % bp[7]);
                                        cp[8] =   (val  % bp[8]);
                                        cp[9] =   (val  % bp[9]);
                                        cp[10] =   (val  % bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =   (val  % *bp++);
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
                                        cp[0] =   (ap[0]  % cp[0]);
                                        cp[1] =   (ap[1]  % cp[1]);
                                        cp[2] =   (ap[2]  % cp[2]);
                                        cp[3] =   (ap[3]  % cp[3]);
                                        cp[4] =   (ap[4]  % cp[4]);
                                        cp[5] =   (ap[5]  % cp[5]);
                                        cp[6] =   (ap[6]  % cp[6]);
                                        cp[7] =   (ap[7]  % cp[7]);
                                        cp[8] =   (ap[8]  % cp[8]);
                                        cp[9] =   (ap[9]  % cp[9]);
                                        cp[10] =   (ap[10]  % cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (*ap++  % *cp);
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
                                        cp[0] =   (val  % cp[0]);
                                        cp[1] =   (val  % cp[1]);
                                        cp[2] =   (val  % cp[2]);
                                        cp[3] =   (val  % cp[3]);
                                        cp[4] =   (val  % cp[4]);
                                        cp[5] =   (val  % cp[5]);
                                        cp[6] =   (val  % cp[6]);
                                        cp[7] =   (val  % cp[7]);
                                        cp[8] =   (val  % cp[8]);
                                        cp[9] =   (val  % cp[9]);
                                        cp[10] =   (val  % cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (val  % *cp);
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
                                        cp[0] =   (ap[0]  % bp[0]);
                                        cp[1] =   (ap[1]  % bp[1]);
                                        cp[2] =   (ap[2]  % bp[2]);
                                        cp[3] =   (ap[3]  % bp[3]);
                                        cp[4] =   (ap[4]  % bp[4]);
                                        cp[5] =   (ap[5]  % bp[5]);
                                        cp[6] =   (ap[6]  % bp[6]);
                                        cp[7] =   (ap[7]  % bp[7]);
                                        cp[8] =   (ap[8]  % bp[8]);
                                        cp[9] =   (ap[9]  % bp[9]);
                                        cp[10] =   (ap[10]  % bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (*ap  % *bp);
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
                                        cp[0] =   (ap[0]  % val);
                                        cp[1] =   (ap[1]  % val);
                                        cp[2] =   (ap[2]  % val);
                                        cp[3] =   (ap[3]  % val);
                                        cp[4] =   (ap[4]  % val);
                                        cp[5] =   (ap[5]  % val);
                                        cp[6] =   (ap[6]  % val);
                                        cp[7] =   (ap[7]  % val);
                                        cp[8] =   (ap[8]  % val);
                                        cp[9] =   (ap[9]  % val);
                                        cp[10] =   (ap[10]  % val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (*ap  % val);
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
                                        cp[0] =   (cp[0]  % bp[0]);
                                        cp[1] =   (cp[1]  % bp[1]);
                                        cp[2] =   (cp[2]  % bp[2]);
                                        cp[3] =   (cp[3]  % bp[3]);
                                        cp[4] =   (cp[4]  % bp[4]);
                                        cp[5] =   (cp[5]  % bp[5]);
                                        cp[6] =   (cp[6]  % bp[6]);
                                        cp[7] =   (cp[7]  % bp[7]);
                                        cp[8] =   (cp[8]  % bp[8]);
                                        cp[9] =   (cp[9]  % bp[9]);
                                        cp[10] =   (cp[10]  % bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (*cp  % *bp);
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
                                        cp[0] =   (cp[0]  % val);
                                        cp[1] =   (cp[1]  % val);
                                        cp[2] =   (cp[2]  % val);
                                        cp[3] =   (cp[3]  % val);
                                        cp[4] =   (cp[4]  % val);
                                        cp[5] =   (cp[5]  % val);
                                        cp[6] =   (cp[6]  % val);
                                        cp[7] =   (cp[7]  % val);
                                        cp[8] =   (cp[8]  % val);
                                        cp[9] =   (cp[9]  % val);
                                        cp[10] =   (cp[10]  % val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =   (*cp  % val);
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