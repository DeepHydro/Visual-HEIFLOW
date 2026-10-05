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

        /// <summary>Arcus tangens of elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with elementwise arcus tangens of both inputs</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<float>  atan2(ILInArray<float> A, ILInArray<float> B) {
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
                       
                        return new  ILRetArray<float> (new  float [1]{  (float)Math.Atan2 (A.GetValue(0)  , B.GetValue(0))}, A.Size);
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
                            return  atan2Ex(A,B); 
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
                                cp[0] =   (float)Math.Atan2 (cp[0]  , bp[0]);
                                cp[1] =   (float)Math.Atan2 (cp[1]  , bp[1]);
                                cp[2] =   (float)Math.Atan2 (cp[2]  , bp[2]);
                                cp[3] =   (float)Math.Atan2 (cp[3]  , bp[3]);
                                cp[4] =   (float)Math.Atan2 (cp[4]  , bp[4]);
                                cp[5] =   (float)Math.Atan2 (cp[5]  , bp[5]);
                                cp[6] =   (float)Math.Atan2 (cp[6]  , bp[6]);
                                cp[7] =   (float)Math.Atan2 (cp[7]  , bp[7]);
                                cp[8] =   (float)Math.Atan2 (cp[8]  , bp[8]);
                                cp[9] =   (float)Math.Atan2 (cp[9]  , bp[9]);
                                cp[10] =   (float)Math.Atan2 (cp[10]  , bp[10]);
                                cp[11] =   (float)Math.Atan2 (cp[11]  , bp[11]);
                                cp[12] =   (float)Math.Atan2 (cp[12]  , bp[12]);
                                cp[13] =   (float)Math.Atan2 (cp[13]  , bp[13]);
                                cp[14] =   (float)Math.Atan2 (cp[14]  , bp[14]);
                                cp[15] =   (float)Math.Atan2 (cp[15]  , bp[15]);
                                cp[16] =   (float)Math.Atan2 (cp[16]  , bp[16]);
                                cp[17] =   (float)Math.Atan2 (cp[17]  , bp[17]);
                                cp[18] =   (float)Math.Atan2 (cp[18]  , bp[18]);
                                cp[19] =   (float)Math.Atan2 (cp[19]  , bp[19]);
                                cp[20] =   (float)Math.Atan2 (cp[20]  , bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (float)Math.Atan2 (*cp  , *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           float* ap = ((float*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =   (float)Math.Atan2 (ap[0]  , cp[0]);
                                cp[1] =   (float)Math.Atan2 (ap[1]  , cp[1]);
                                cp[2] =   (float)Math.Atan2 (ap[2]  , cp[2]);
                                cp[3] =   (float)Math.Atan2 (ap[3]  , cp[3]);
                                cp[4] =   (float)Math.Atan2 (ap[4]  , cp[4]);
                                cp[5] =   (float)Math.Atan2 (ap[5]  , cp[5]);
                                cp[6] =   (float)Math.Atan2 (ap[6]  , cp[6]);
                                cp[7] =   (float)Math.Atan2 (ap[7]  , cp[7]);
                                cp[8] =   (float)Math.Atan2 (ap[8]  , cp[8]);
                                cp[9] =   (float)Math.Atan2 (ap[9]  , cp[9]);
                                cp[10] =   (float)Math.Atan2 (ap[10]  , cp[10]);
                                cp[11] =   (float)Math.Atan2 (ap[11]  , cp[11]);
                                cp[12] =   (float)Math.Atan2 (ap[12]  , cp[12]);
                                cp[13] =   (float)Math.Atan2 (ap[13]  , cp[13]);
                                cp[14] =   (float)Math.Atan2 (ap[14]  , cp[14]);
                                cp[15] =   (float)Math.Atan2 (ap[15]  , cp[15]);
                                cp[16] =   (float)Math.Atan2 (ap[16]  , cp[16]);
                                cp[17] =   (float)Math.Atan2 (ap[17]  , cp[17]);
                                cp[18] =   (float)Math.Atan2 (ap[18]  , cp[18]);
                                cp[19] =   (float)Math.Atan2 (ap[19]  , cp[19]);
                                cp[20] =   (float)Math.Atan2 (ap[20]  , cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (float)Math.Atan2 (*ap  , *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((float*)range.Item3 + range.Item1);
                            bp = ((float*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =   (float)Math.Atan2 (ap[0]  , bp[0]);
                                cp[1] =   (float)Math.Atan2 (ap[1]  , bp[1]);
                                cp[2] =   (float)Math.Atan2 (ap[2]  , bp[2]);
                                cp[3] =   (float)Math.Atan2 (ap[3]  , bp[3]);
                                cp[4] =   (float)Math.Atan2 (ap[4]  , bp[4]);
                                cp[5] =   (float)Math.Atan2 (ap[5]  , bp[5]);
                                cp[6] =   (float)Math.Atan2 (ap[6]  , bp[6]);
                                cp[7] =   (float)Math.Atan2 (ap[7]  , bp[7]);
                                cp[8] =   (float)Math.Atan2 (ap[8]  , bp[8]);
                                cp[9] =   (float)Math.Atan2 (ap[9]  , bp[9]);
                                cp[10] =   (float)Math.Atan2 (ap[10]  , bp[10]);
                                cp[11] =   (float)Math.Atan2 (ap[11]  , bp[11]);
                                cp[12] =   (float)Math.Atan2 (ap[12]  , bp[12]);
                                cp[13] =   (float)Math.Atan2 (ap[13]  , bp[13]);
                                cp[14] =   (float)Math.Atan2 (ap[14]  , bp[14]);
                                cp[15] =   (float)Math.Atan2 (ap[15]  , bp[15]);
                                cp[16] =   (float)Math.Atan2 (ap[16]  , bp[16]);
                                cp[17] =   (float)Math.Atan2 (ap[17]  , bp[17]);
                                cp[18] =   (float)Math.Atan2 (ap[18]  , bp[18]);
                                cp[19] =   (float)Math.Atan2 (ap[19]  , bp[19]);
                                cp[20] =   (float)Math.Atan2 (ap[20]  , bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =   (float)Math.Atan2 (*ap  , *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((float*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =   (float)Math.Atan2 (cp[0]  , scalar);
                                cp[1] =   (float)Math.Atan2 (cp[1]  , scalar);
                                cp[2] =   (float)Math.Atan2 (cp[2]  , scalar);
                                cp[3] =   (float)Math.Atan2 (cp[3]  , scalar);
                                cp[4] =   (float)Math.Atan2 (cp[4]  , scalar);
                                cp[5] =   (float)Math.Atan2 (cp[5]  , scalar);
                                cp[6] =   (float)Math.Atan2 (cp[6]  , scalar);
                                cp[7] =   (float)Math.Atan2 (cp[7]  , scalar);
                                cp[8] =   (float)Math.Atan2 (cp[8]  , scalar);
                                cp[9] =   (float)Math.Atan2 (cp[9]  , scalar);
                                cp[10] =   (float)Math.Atan2 (cp[10]  , scalar);
                                cp[11] =   (float)Math.Atan2 (cp[11]  , scalar);
                                cp[12] =   (float)Math.Atan2 (cp[12]  , scalar);
                                cp[13] =   (float)Math.Atan2 (cp[13]  , scalar);
                                cp[14] =   (float)Math.Atan2 (cp[14]  , scalar);
                                cp[15] =   (float)Math.Atan2 (cp[15]  , scalar);
                                cp[16] =   (float)Math.Atan2 (cp[16]  , scalar);
                                cp[17] =   (float)Math.Atan2 (cp[17]  , scalar);
                                cp[18] =   (float)Math.Atan2 (cp[18]  , scalar);
                                cp[19] =   (float)Math.Atan2 (cp[19]  , scalar);
                                cp[20] =   (float)Math.Atan2 (cp[20]  , scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (float)Math.Atan2 (*cp  , scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((float*)range.Item3 + range.Item1);
                            scalar = *((float*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =   (float)Math.Atan2 (ap[0]  , scalar);
                                cp[1] =   (float)Math.Atan2 (ap[1]  , scalar);
                                cp[2] =   (float)Math.Atan2 (ap[2]  , scalar);
                                cp[3] =   (float)Math.Atan2 (ap[3]  , scalar);
                                cp[4] =   (float)Math.Atan2 (ap[4]  , scalar);
                                cp[5] =   (float)Math.Atan2 (ap[5]  , scalar);
                                cp[6] =   (float)Math.Atan2 (ap[6]  , scalar);
                                cp[7] =   (float)Math.Atan2 (ap[7]  , scalar);
                                cp[8] =   (float)Math.Atan2 (ap[8]  , scalar);
                                cp[9] =   (float)Math.Atan2 (ap[9]  , scalar);
                                cp[10] =   (float)Math.Atan2 (ap[10]  , scalar);
                                cp[11] =   (float)Math.Atan2 (ap[11]  , scalar);
                                cp[12] =   (float)Math.Atan2 (ap[12]  , scalar);
                                cp[13] =   (float)Math.Atan2 (ap[13]  , scalar);
                                cp[14] =   (float)Math.Atan2 (ap[14]  , scalar);
                                cp[15] =   (float)Math.Atan2 (ap[15]  , scalar);
                                cp[16] =   (float)Math.Atan2 (ap[16]  , scalar);
                                cp[17] =   (float)Math.Atan2 (ap[17]  , scalar);
                                cp[18] =   (float)Math.Atan2 (ap[18]  , scalar);
                                cp[19] =   (float)Math.Atan2 (ap[19]  , scalar);
                                cp[20] =   (float)Math.Atan2 (ap[20]  , scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (float)Math.Atan2 (*ap  , scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((float*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =   (float)Math.Atan2 (scalar  , cp[0]);
                                cp[1] =   (float)Math.Atan2 (scalar  , cp[1]);
                                cp[2] =   (float)Math.Atan2 (scalar  , cp[2]);
                                cp[3] =   (float)Math.Atan2 (scalar  , cp[3]);
                                cp[4] =   (float)Math.Atan2 (scalar  , cp[4]);
                                cp[5] =   (float)Math.Atan2 (scalar  , cp[5]);
                                cp[6] =   (float)Math.Atan2 (scalar  , cp[6]);
                                cp[7] =   (float)Math.Atan2 (scalar  , cp[7]);
                                cp[8] =   (float)Math.Atan2 (scalar  , cp[8]);
                                cp[9] =   (float)Math.Atan2 (scalar  , cp[9]);
                                cp[10] =   (float)Math.Atan2 (scalar  , cp[10]);
                                cp[11] =   (float)Math.Atan2 (scalar  , cp[11]);
                                cp[12] =   (float)Math.Atan2 (scalar  , cp[12]);
                                cp[13] =   (float)Math.Atan2 (scalar  , cp[13]);
                                cp[14] =   (float)Math.Atan2 (scalar  , cp[14]);
                                cp[15] =   (float)Math.Atan2 (scalar  , cp[15]);
                                cp[16] =   (float)Math.Atan2 (scalar  , cp[16]);
                                cp[17] =   (float)Math.Atan2 (scalar  , cp[17]);
                                cp[18] =   (float)Math.Atan2 (scalar  , cp[18]);
                                cp[19] =   (float)Math.Atan2 (scalar  , cp[19]);
                                cp[20] =   (float)Math.Atan2 (scalar  , cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (float)Math.Atan2 (scalar  , *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((float*)range.Item3);
                            bp = ((float*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =   (float)Math.Atan2 (scalar  , bp[0]);
                                cp[1] =   (float)Math.Atan2 (scalar  , bp[1]);
                                cp[2] =   (float)Math.Atan2 (scalar  , bp[2]);
                                cp[3] =   (float)Math.Atan2 (scalar  , bp[3]);
                                cp[4] =   (float)Math.Atan2 (scalar  , bp[4]);
                                cp[5] =   (float)Math.Atan2 (scalar  , bp[5]);
                                cp[6] =   (float)Math.Atan2 (scalar  , bp[6]);
                                cp[7] =   (float)Math.Atan2 (scalar  , bp[7]);
                                cp[8] =   (float)Math.Atan2 (scalar  , bp[8]);
                                cp[9] =   (float)Math.Atan2 (scalar  , bp[9]);
                                cp[10] =   (float)Math.Atan2 (scalar  , bp[10]);
                                cp[11] =   (float)Math.Atan2 (scalar  , bp[11]);
                                cp[12] =   (float)Math.Atan2 (scalar  , bp[12]);
                                cp[13] =   (float)Math.Atan2 (scalar  , bp[13]);
                                cp[14] =   (float)Math.Atan2 (scalar  , bp[14]);
                                cp[15] =   (float)Math.Atan2 (scalar  , bp[15]);
                                cp[16] =   (float)Math.Atan2 (scalar  , bp[16]);
                                cp[17] =   (float)Math.Atan2 (scalar  , bp[17]);
                                cp[18] =   (float)Math.Atan2 (scalar  , bp[18]);
                                cp[19] =   (float)Math.Atan2 (scalar  , bp[19]);
                                cp[20] =   (float)Math.Atan2 (scalar  , bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   (float)Math.Atan2 (scalar  , *bp);
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

        private static unsafe ILRetArray<float>  atan2Ex(ILInArray<float> A, ILInArray<float> B) {
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
                                        cp[0] =   (float)Math.Atan2 (ap[0]  , bp[0]);
                                        cp[1] =   (float)Math.Atan2 (ap[1]  , bp[1]);
                                        cp[2] =   (float)Math.Atan2 (ap[2]  , bp[2]);
                                        cp[3] =   (float)Math.Atan2 (ap[3]  , bp[3]);
                                        cp[4] =   (float)Math.Atan2 (ap[4]  , bp[4]);
                                        cp[5] =   (float)Math.Atan2 (ap[5]  , bp[5]);
                                        cp[6] =   (float)Math.Atan2 (ap[6]  , bp[6]);
                                        cp[7] =   (float)Math.Atan2 (ap[7]  , bp[7]);
                                        cp[8] =   (float)Math.Atan2 (ap[8]  , bp[8]);
                                        cp[9] =   (float)Math.Atan2 (ap[9]  , bp[9]);
                                        cp[10] =   (float)Math.Atan2 (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =   (float)Math.Atan2 (*ap++  , *bp++);
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
                                        cp[0] =   (float)Math.Atan2 (val  , bp[0]);
                                        cp[1] =   (float)Math.Atan2 (val  , bp[1]);
                                        cp[2] =   (float)Math.Atan2 (val  , bp[2]);
                                        cp[3] =   (float)Math.Atan2 (val  , bp[3]);
                                        cp[4] =   (float)Math.Atan2 (val  , bp[4]);
                                        cp[5] =   (float)Math.Atan2 (val  , bp[5]);
                                        cp[6] =   (float)Math.Atan2 (val  , bp[6]);
                                        cp[7] =   (float)Math.Atan2 (val  , bp[7]);
                                        cp[8] =   (float)Math.Atan2 (val  , bp[8]);
                                        cp[9] =   (float)Math.Atan2 (val  , bp[9]);
                                        cp[10] =   (float)Math.Atan2 (val  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =   (float)Math.Atan2 (val  , *bp++);
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
                                        cp[0] =   (float)Math.Atan2 (ap[0]  , cp[0]);
                                        cp[1] =   (float)Math.Atan2 (ap[1]  , cp[1]);
                                        cp[2] =   (float)Math.Atan2 (ap[2]  , cp[2]);
                                        cp[3] =   (float)Math.Atan2 (ap[3]  , cp[3]);
                                        cp[4] =   (float)Math.Atan2 (ap[4]  , cp[4]);
                                        cp[5] =   (float)Math.Atan2 (ap[5]  , cp[5]);
                                        cp[6] =   (float)Math.Atan2 (ap[6]  , cp[6]);
                                        cp[7] =   (float)Math.Atan2 (ap[7]  , cp[7]);
                                        cp[8] =   (float)Math.Atan2 (ap[8]  , cp[8]);
                                        cp[9] =   (float)Math.Atan2 (ap[9]  , cp[9]);
                                        cp[10] =   (float)Math.Atan2 (ap[10]  , cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (float)Math.Atan2 (*ap++  , *cp);
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
                                        cp[0] =   (float)Math.Atan2 (val  , cp[0]);
                                        cp[1] =   (float)Math.Atan2 (val  , cp[1]);
                                        cp[2] =   (float)Math.Atan2 (val  , cp[2]);
                                        cp[3] =   (float)Math.Atan2 (val  , cp[3]);
                                        cp[4] =   (float)Math.Atan2 (val  , cp[4]);
                                        cp[5] =   (float)Math.Atan2 (val  , cp[5]);
                                        cp[6] =   (float)Math.Atan2 (val  , cp[6]);
                                        cp[7] =   (float)Math.Atan2 (val  , cp[7]);
                                        cp[8] =   (float)Math.Atan2 (val  , cp[8]);
                                        cp[9] =   (float)Math.Atan2 (val  , cp[9]);
                                        cp[10] =   (float)Math.Atan2 (val  , cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (float)Math.Atan2 (val  , *cp);
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
                                        cp[0] =   (float)Math.Atan2 (ap[0]  , bp[0]);
                                        cp[1] =   (float)Math.Atan2 (ap[1]  , bp[1]);
                                        cp[2] =   (float)Math.Atan2 (ap[2]  , bp[2]);
                                        cp[3] =   (float)Math.Atan2 (ap[3]  , bp[3]);
                                        cp[4] =   (float)Math.Atan2 (ap[4]  , bp[4]);
                                        cp[5] =   (float)Math.Atan2 (ap[5]  , bp[5]);
                                        cp[6] =   (float)Math.Atan2 (ap[6]  , bp[6]);
                                        cp[7] =   (float)Math.Atan2 (ap[7]  , bp[7]);
                                        cp[8] =   (float)Math.Atan2 (ap[8]  , bp[8]);
                                        cp[9] =   (float)Math.Atan2 (ap[9]  , bp[9]);
                                        cp[10] =   (float)Math.Atan2 (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (float)Math.Atan2 (*ap  , *bp);
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
                                        cp[0] =   (float)Math.Atan2 (ap[0]  , val);
                                        cp[1] =   (float)Math.Atan2 (ap[1]  , val);
                                        cp[2] =   (float)Math.Atan2 (ap[2]  , val);
                                        cp[3] =   (float)Math.Atan2 (ap[3]  , val);
                                        cp[4] =   (float)Math.Atan2 (ap[4]  , val);
                                        cp[5] =   (float)Math.Atan2 (ap[5]  , val);
                                        cp[6] =   (float)Math.Atan2 (ap[6]  , val);
                                        cp[7] =   (float)Math.Atan2 (ap[7]  , val);
                                        cp[8] =   (float)Math.Atan2 (ap[8]  , val);
                                        cp[9] =   (float)Math.Atan2 (ap[9]  , val);
                                        cp[10] =   (float)Math.Atan2 (ap[10]  , val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (float)Math.Atan2 (*ap  , val);
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
                                        cp[0] =   (float)Math.Atan2 (cp[0]  , bp[0]);
                                        cp[1] =   (float)Math.Atan2 (cp[1]  , bp[1]);
                                        cp[2] =   (float)Math.Atan2 (cp[2]  , bp[2]);
                                        cp[3] =   (float)Math.Atan2 (cp[3]  , bp[3]);
                                        cp[4] =   (float)Math.Atan2 (cp[4]  , bp[4]);
                                        cp[5] =   (float)Math.Atan2 (cp[5]  , bp[5]);
                                        cp[6] =   (float)Math.Atan2 (cp[6]  , bp[6]);
                                        cp[7] =   (float)Math.Atan2 (cp[7]  , bp[7]);
                                        cp[8] =   (float)Math.Atan2 (cp[8]  , bp[8]);
                                        cp[9] =   (float)Math.Atan2 (cp[9]  , bp[9]);
                                        cp[10] =   (float)Math.Atan2 (cp[10]  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   (float)Math.Atan2 (*cp  , *bp);
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
                                        cp[0] =   (float)Math.Atan2 (cp[0]  , val);
                                        cp[1] =   (float)Math.Atan2 (cp[1]  , val);
                                        cp[2] =   (float)Math.Atan2 (cp[2]  , val);
                                        cp[3] =   (float)Math.Atan2 (cp[3]  , val);
                                        cp[4] =   (float)Math.Atan2 (cp[4]  , val);
                                        cp[5] =   (float)Math.Atan2 (cp[5]  , val);
                                        cp[6] =   (float)Math.Atan2 (cp[6]  , val);
                                        cp[7] =   (float)Math.Atan2 (cp[7]  , val);
                                        cp[8] =   (float)Math.Atan2 (cp[8]  , val);
                                        cp[9] =   (float)Math.Atan2 (cp[9]  , val);
                                        cp[10] =   (float)Math.Atan2 (cp[10]  , val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =   (float)Math.Atan2 (*cp  , val);
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

        /// <summary>Arcus tangens of elements</summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>Array with elementwise arcus tangens of both inputs</returns>
        /// <remarks><para>On empty input an empty array will be returned.</para>
        /// <para>A and/or B may be scalar. The scalar value will be applied on all elements of the 
        /// other array.</para>
        /// <para>If A or B is a colum vector and the other parameter is an array with a matching colum length, the vector is used to operate on all columns of the array. 
        /// Similar, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length. This feature 
        /// can be used to replace the (costly) repmat function for most binary operators.</para>
        /// <para>For all other cases the dimensions of A and B must match.</para></remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arrays does not match any parameter rule.</exception>
        public unsafe static ILRetArray<double>  atan2(ILInArray<double> A, ILInArray<double> B) {
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
                       
                        return new  ILRetArray<double> (new  double [1]{  Math.Atan2 (A.GetValue(0)  , B.GetValue(0))}, A.Size);
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
                            return  atan2Ex(A,B); 
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
                                cp[0] =   Math.Atan2 (cp[0]  , bp[0]);
                                cp[1] =   Math.Atan2 (cp[1]  , bp[1]);
                                cp[2] =   Math.Atan2 (cp[2]  , bp[2]);
                                cp[3] =   Math.Atan2 (cp[3]  , bp[3]);
                                cp[4] =   Math.Atan2 (cp[4]  , bp[4]);
                                cp[5] =   Math.Atan2 (cp[5]  , bp[5]);
                                cp[6] =   Math.Atan2 (cp[6]  , bp[6]);
                                cp[7] =   Math.Atan2 (cp[7]  , bp[7]);
                                cp[8] =   Math.Atan2 (cp[8]  , bp[8]);
                                cp[9] =   Math.Atan2 (cp[9]  , bp[9]);
                                cp[10] =   Math.Atan2 (cp[10]  , bp[10]);
                                cp[11] =   Math.Atan2 (cp[11]  , bp[11]);
                                cp[12] =   Math.Atan2 (cp[12]  , bp[12]);
                                cp[13] =   Math.Atan2 (cp[13]  , bp[13]);
                                cp[14] =   Math.Atan2 (cp[14]  , bp[14]);
                                cp[15] =   Math.Atan2 (cp[15]  , bp[15]);
                                cp[16] =   Math.Atan2 (cp[16]  , bp[16]);
                                cp[17] =   Math.Atan2 (cp[17]  , bp[17]);
                                cp[18] =   Math.Atan2 (cp[18]  , bp[18]);
                                cp[19] =   Math.Atan2 (cp[19]  , bp[19]);
                                cp[20] =   Math.Atan2 (cp[20]  , bp[20]);
                                cp += 21; bp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   Math.Atan2 (*cp  , *bp);
                                cp++; bp++;
                            }
                            break;
                        case BinOpItMode.AAIB:
                           double* ap = ((double*)range.Item3 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =   Math.Atan2 (ap[0]  , cp[0]);
                                cp[1] =   Math.Atan2 (ap[1]  , cp[1]);
                                cp[2] =   Math.Atan2 (ap[2]  , cp[2]);
                                cp[3] =   Math.Atan2 (ap[3]  , cp[3]);
                                cp[4] =   Math.Atan2 (ap[4]  , cp[4]);
                                cp[5] =   Math.Atan2 (ap[5]  , cp[5]);
                                cp[6] =   Math.Atan2 (ap[6]  , cp[6]);
                                cp[7] =   Math.Atan2 (ap[7]  , cp[7]);
                                cp[8] =   Math.Atan2 (ap[8]  , cp[8]);
                                cp[9] =   Math.Atan2 (ap[9]  , cp[9]);
                                cp[10] =   Math.Atan2 (ap[10]  , cp[10]);
                                cp[11] =   Math.Atan2 (ap[11]  , cp[11]);
                                cp[12] =   Math.Atan2 (ap[12]  , cp[12]);
                                cp[13] =   Math.Atan2 (ap[13]  , cp[13]);
                                cp[14] =   Math.Atan2 (ap[14]  , cp[14]);
                                cp[15] =   Math.Atan2 (ap[15]  , cp[15]);
                                cp[16] =   Math.Atan2 (ap[16]  , cp[16]);
                                cp[17] =   Math.Atan2 (ap[17]  , cp[17]);
                                cp[18] =   Math.Atan2 (ap[18]  , cp[18]);
                                cp[19] =   Math.Atan2 (ap[19]  , cp[19]);
                                cp[20] =   Math.Atan2 (ap[20]  , cp[20]);
                                ap += 21; cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   Math.Atan2 (*ap  , *cp);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.AAN:
                            ap = ((double*)range.Item3 + range.Item1);
                            bp = ((double*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =   Math.Atan2 (ap[0]  , bp[0]);
                                cp[1] =   Math.Atan2 (ap[1]  , bp[1]);
                                cp[2] =   Math.Atan2 (ap[2]  , bp[2]);
                                cp[3] =   Math.Atan2 (ap[3]  , bp[3]);
                                cp[4] =   Math.Atan2 (ap[4]  , bp[4]);
                                cp[5] =   Math.Atan2 (ap[5]  , bp[5]);
                                cp[6] =   Math.Atan2 (ap[6]  , bp[6]);
                                cp[7] =   Math.Atan2 (ap[7]  , bp[7]);
                                cp[8] =   Math.Atan2 (ap[8]  , bp[8]);
                                cp[9] =   Math.Atan2 (ap[9]  , bp[9]);
                                cp[10] =   Math.Atan2 (ap[10]  , bp[10]);
                                cp[11] =   Math.Atan2 (ap[11]  , bp[11]);
                                cp[12] =   Math.Atan2 (ap[12]  , bp[12]);
                                cp[13] =   Math.Atan2 (ap[13]  , bp[13]);
                                cp[14] =   Math.Atan2 (ap[14]  , bp[14]);
                                cp[15] =   Math.Atan2 (ap[15]  , bp[15]);
                                cp[16] =   Math.Atan2 (ap[16]  , bp[16]);
                                cp[17] =   Math.Atan2 (ap[17]  , bp[17]);
                                cp[18] =   Math.Atan2 (ap[18]  , bp[18]);
                                cp[19] =   Math.Atan2 (ap[19]  , bp[19]);
                                cp[20] =   Math.Atan2 (ap[20]  , bp[20]);
                                ap+=21; bp+=21; cp+=21; j-=21;
                            }
                            while (j --> 0) {
                               
                                *cp =   Math.Atan2 (*ap  , *bp);
                                ap++; bp++; cp++;
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((double*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =   Math.Atan2 (cp[0]  , scalar);
                                cp[1] =   Math.Atan2 (cp[1]  , scalar);
                                cp[2] =   Math.Atan2 (cp[2]  , scalar);
                                cp[3] =   Math.Atan2 (cp[3]  , scalar);
                                cp[4] =   Math.Atan2 (cp[4]  , scalar);
                                cp[5] =   Math.Atan2 (cp[5]  , scalar);
                                cp[6] =   Math.Atan2 (cp[6]  , scalar);
                                cp[7] =   Math.Atan2 (cp[7]  , scalar);
                                cp[8] =   Math.Atan2 (cp[8]  , scalar);
                                cp[9] =   Math.Atan2 (cp[9]  , scalar);
                                cp[10] =   Math.Atan2 (cp[10]  , scalar);
                                cp[11] =   Math.Atan2 (cp[11]  , scalar);
                                cp[12] =   Math.Atan2 (cp[12]  , scalar);
                                cp[13] =   Math.Atan2 (cp[13]  , scalar);
                                cp[14] =   Math.Atan2 (cp[14]  , scalar);
                                cp[15] =   Math.Atan2 (cp[15]  , scalar);
                                cp[16] =   Math.Atan2 (cp[16]  , scalar);
                                cp[17] =   Math.Atan2 (cp[17]  , scalar);
                                cp[18] =   Math.Atan2 (cp[18]  , scalar);
                                cp[19] =   Math.Atan2 (cp[19]  , scalar);
                                cp[20] =   Math.Atan2 (cp[20]  , scalar);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   Math.Atan2 (*cp  , scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((double*)range.Item3 + range.Item1);
                            scalar = *((double*)range.Item4);
                            while (j > 20) {
                               
                                cp[0] =   Math.Atan2 (ap[0]  , scalar);
                                cp[1] =   Math.Atan2 (ap[1]  , scalar);
                                cp[2] =   Math.Atan2 (ap[2]  , scalar);
                                cp[3] =   Math.Atan2 (ap[3]  , scalar);
                                cp[4] =   Math.Atan2 (ap[4]  , scalar);
                                cp[5] =   Math.Atan2 (ap[5]  , scalar);
                                cp[6] =   Math.Atan2 (ap[6]  , scalar);
                                cp[7] =   Math.Atan2 (ap[7]  , scalar);
                                cp[8] =   Math.Atan2 (ap[8]  , scalar);
                                cp[9] =   Math.Atan2 (ap[9]  , scalar);
                                cp[10] =   Math.Atan2 (ap[10]  , scalar);
                                cp[11] =   Math.Atan2 (ap[11]  , scalar);
                                cp[12] =   Math.Atan2 (ap[12]  , scalar);
                                cp[13] =   Math.Atan2 (ap[13]  , scalar);
                                cp[14] =   Math.Atan2 (ap[14]  , scalar);
                                cp[15] =   Math.Atan2 (ap[15]  , scalar);
                                cp[16] =   Math.Atan2 (ap[16]  , scalar);
                                cp[17] =   Math.Atan2 (ap[17]  , scalar);
                                cp[18] =   Math.Atan2 (ap[18]  , scalar);
                                cp[19] =   Math.Atan2 (ap[19]  , scalar);
                                cp[20] =   Math.Atan2 (ap[20]  , scalar);
                                ap+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   Math.Atan2 (*ap  , scalar);
                                ap++; cp++; 
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((double*)range.Item3);
                            while (j > 20) {
                               
                                cp[0] =   Math.Atan2 (scalar  , cp[0]);
                                cp[1] =   Math.Atan2 (scalar  , cp[1]);
                                cp[2] =   Math.Atan2 (scalar  , cp[2]);
                                cp[3] =   Math.Atan2 (scalar  , cp[3]);
                                cp[4] =   Math.Atan2 (scalar  , cp[4]);
                                cp[5] =   Math.Atan2 (scalar  , cp[5]);
                                cp[6] =   Math.Atan2 (scalar  , cp[6]);
                                cp[7] =   Math.Atan2 (scalar  , cp[7]);
                                cp[8] =   Math.Atan2 (scalar  , cp[8]);
                                cp[9] =   Math.Atan2 (scalar  , cp[9]);
                                cp[10] =   Math.Atan2 (scalar  , cp[10]);
                                cp[11] =   Math.Atan2 (scalar  , cp[11]);
                                cp[12] =   Math.Atan2 (scalar  , cp[12]);
                                cp[13] =   Math.Atan2 (scalar  , cp[13]);
                                cp[14] =   Math.Atan2 (scalar  , cp[14]);
                                cp[15] =   Math.Atan2 (scalar  , cp[15]);
                                cp[16] =   Math.Atan2 (scalar  , cp[16]);
                                cp[17] =   Math.Atan2 (scalar  , cp[17]);
                                cp[18] =   Math.Atan2 (scalar  , cp[18]);
                                cp[19] =   Math.Atan2 (scalar  , cp[19]);
                                cp[20] =   Math.Atan2 (scalar  , cp[20]);
                                cp += 21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   Math.Atan2 (scalar  , *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((double*)range.Item3);
                            bp = ((double*)range.Item4 + range.Item1);
                            while (j > 20) {
                               
                                cp[0] =   Math.Atan2 (scalar  , bp[0]);
                                cp[1] =   Math.Atan2 (scalar  , bp[1]);
                                cp[2] =   Math.Atan2 (scalar  , bp[2]);
                                cp[3] =   Math.Atan2 (scalar  , bp[3]);
                                cp[4] =   Math.Atan2 (scalar  , bp[4]);
                                cp[5] =   Math.Atan2 (scalar  , bp[5]);
                                cp[6] =   Math.Atan2 (scalar  , bp[6]);
                                cp[7] =   Math.Atan2 (scalar  , bp[7]);
                                cp[8] =   Math.Atan2 (scalar  , bp[8]);
                                cp[9] =   Math.Atan2 (scalar  , bp[9]);
                                cp[10] =   Math.Atan2 (scalar  , bp[10]);
                                cp[11] =   Math.Atan2 (scalar  , bp[11]);
                                cp[12] =   Math.Atan2 (scalar  , bp[12]);
                                cp[13] =   Math.Atan2 (scalar  , bp[13]);
                                cp[14] =   Math.Atan2 (scalar  , bp[14]);
                                cp[15] =   Math.Atan2 (scalar  , bp[15]);
                                cp[16] =   Math.Atan2 (scalar  , bp[16]);
                                cp[17] =   Math.Atan2 (scalar  , bp[17]);
                                cp[18] =   Math.Atan2 (scalar  , bp[18]);
                                cp[19] =   Math.Atan2 (scalar  , bp[19]);
                                cp[20] =   Math.Atan2 (scalar  , bp[20]);
                                bp+=21; cp+=21; j -= 21; 
                            }
                            while (j --> 0) {
                               
                                *cp =   Math.Atan2 (scalar  , *bp);
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

        private static unsafe ILRetArray<double>  atan2Ex(ILInArray<double> A, ILInArray<double> B) {
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
                                        cp[0] =   Math.Atan2 (ap[0]  , bp[0]);
                                        cp[1] =   Math.Atan2 (ap[1]  , bp[1]);
                                        cp[2] =   Math.Atan2 (ap[2]  , bp[2]);
                                        cp[3] =   Math.Atan2 (ap[3]  , bp[3]);
                                        cp[4] =   Math.Atan2 (ap[4]  , bp[4]);
                                        cp[5] =   Math.Atan2 (ap[5]  , bp[5]);
                                        cp[6] =   Math.Atan2 (ap[6]  , bp[6]);
                                        cp[7] =   Math.Atan2 (ap[7]  , bp[7]);
                                        cp[8] =   Math.Atan2 (ap[8]  , bp[8]);
                                        cp[9] =   Math.Atan2 (ap[9]  , bp[9]);
                                        cp[10] =   Math.Atan2 (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp++ =   Math.Atan2 (*ap++  , *bp++);
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
                                        cp[0] =   Math.Atan2 (val  , bp[0]);
                                        cp[1] =   Math.Atan2 (val  , bp[1]);
                                        cp[2] =   Math.Atan2 (val  , bp[2]);
                                        cp[3] =   Math.Atan2 (val  , bp[3]);
                                        cp[4] =   Math.Atan2 (val  , bp[4]);
                                        cp[5] =   Math.Atan2 (val  , bp[5]);
                                        cp[6] =   Math.Atan2 (val  , bp[6]);
                                        cp[7] =   Math.Atan2 (val  , bp[7]);
                                        cp[8] =   Math.Atan2 (val  , bp[8]);
                                        cp[9] =   Math.Atan2 (val  , bp[9]);
                                        cp[10] =   Math.Atan2 (val  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11; 
                                    }
                                    while (l-- > 0) {
                                        *cp++ =   Math.Atan2 (val  , *bp++);
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
                                        cp[0] =   Math.Atan2 (ap[0]  , cp[0]);
                                        cp[1] =   Math.Atan2 (ap[1]  , cp[1]);
                                        cp[2] =   Math.Atan2 (ap[2]  , cp[2]);
                                        cp[3] =   Math.Atan2 (ap[3]  , cp[3]);
                                        cp[4] =   Math.Atan2 (ap[4]  , cp[4]);
                                        cp[5] =   Math.Atan2 (ap[5]  , cp[5]);
                                        cp[6] =   Math.Atan2 (ap[6]  , cp[6]);
                                        cp[7] =   Math.Atan2 (ap[7]  , cp[7]);
                                        cp[8] =   Math.Atan2 (ap[8]  , cp[8]);
                                        cp[9] =   Math.Atan2 (ap[9]  , cp[9]);
                                        cp[10] =   Math.Atan2 (ap[10]  , cp[10]);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   Math.Atan2 (*ap++  , *cp);
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
                                        cp[0] =   Math.Atan2 (val  , cp[0]);
                                        cp[1] =   Math.Atan2 (val  , cp[1]);
                                        cp[2] =   Math.Atan2 (val  , cp[2]);
                                        cp[3] =   Math.Atan2 (val  , cp[3]);
                                        cp[4] =   Math.Atan2 (val  , cp[4]);
                                        cp[5] =   Math.Atan2 (val  , cp[5]);
                                        cp[6] =   Math.Atan2 (val  , cp[6]);
                                        cp[7] =   Math.Atan2 (val  , cp[7]);
                                        cp[8] =   Math.Atan2 (val  , cp[8]);
                                        cp[9] =   Math.Atan2 (val  , cp[9]);
                                        cp[10] =   Math.Atan2 (val  , cp[10]);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   Math.Atan2 (val  , *cp);
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
                                        cp[0] =   Math.Atan2 (ap[0]  , bp[0]);
                                        cp[1] =   Math.Atan2 (ap[1]  , bp[1]);
                                        cp[2] =   Math.Atan2 (ap[2]  , bp[2]);
                                        cp[3] =   Math.Atan2 (ap[3]  , bp[3]);
                                        cp[4] =   Math.Atan2 (ap[4]  , bp[4]);
                                        cp[5] =   Math.Atan2 (ap[5]  , bp[5]);
                                        cp[6] =   Math.Atan2 (ap[6]  , bp[6]);
                                        cp[7] =   Math.Atan2 (ap[7]  , bp[7]);
                                        cp[8] =   Math.Atan2 (ap[8]  , bp[8]);
                                        cp[9] =   Math.Atan2 (ap[9]  , bp[9]);
                                        cp[10] =   Math.Atan2 (ap[10]  , bp[10]);
                                        ap += 11;
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   Math.Atan2 (*ap  , *bp);
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
                                        cp[0] =   Math.Atan2 (ap[0]  , val);
                                        cp[1] =   Math.Atan2 (ap[1]  , val);
                                        cp[2] =   Math.Atan2 (ap[2]  , val);
                                        cp[3] =   Math.Atan2 (ap[3]  , val);
                                        cp[4] =   Math.Atan2 (ap[4]  , val);
                                        cp[5] =   Math.Atan2 (ap[5]  , val);
                                        cp[6] =   Math.Atan2 (ap[6]  , val);
                                        cp[7] =   Math.Atan2 (ap[7]  , val);
                                        cp[8] =   Math.Atan2 (ap[8]  , val);
                                        cp[9] =   Math.Atan2 (ap[9]  , val);
                                        cp[10] =   Math.Atan2 (ap[10]  , val);
                                        ap += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   Math.Atan2 (*ap  , val);
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
                                        cp[0] =   Math.Atan2 (cp[0]  , bp[0]);
                                        cp[1] =   Math.Atan2 (cp[1]  , bp[1]);
                                        cp[2] =   Math.Atan2 (cp[2]  , bp[2]);
                                        cp[3] =   Math.Atan2 (cp[3]  , bp[3]);
                                        cp[4] =   Math.Atan2 (cp[4]  , bp[4]);
                                        cp[5] =   Math.Atan2 (cp[5]  , bp[5]);
                                        cp[6] =   Math.Atan2 (cp[6]  , bp[6]);
                                        cp[7] =   Math.Atan2 (cp[7]  , bp[7]);
                                        cp[8] =   Math.Atan2 (cp[8]  , bp[8]);
                                        cp[9] =   Math.Atan2 (cp[9]  , bp[9]);
                                        cp[10] =   Math.Atan2 (cp[10]  , bp[10]);
                                        bp += 11;
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l-- > 0) {
                                        *cp =   Math.Atan2 (*cp  , *bp);
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
                                        cp[0] =   Math.Atan2 (cp[0]  , val);
                                        cp[1] =   Math.Atan2 (cp[1]  , val);
                                        cp[2] =   Math.Atan2 (cp[2]  , val);
                                        cp[3] =   Math.Atan2 (cp[3]  , val);
                                        cp[4] =   Math.Atan2 (cp[4]  , val);
                                        cp[5] =   Math.Atan2 (cp[5]  , val);
                                        cp[6] =   Math.Atan2 (cp[6]  , val);
                                        cp[7] =   Math.Atan2 (cp[7]  , val);
                                        cp[8] =   Math.Atan2 (cp[8]  , val);
                                        cp[9] =   Math.Atan2 (cp[9]  , val);
                                        cp[10] =   Math.Atan2 (cp[10]  , val);
                                        cp += 11;
                                        l -= 11;
                                    }
                                    while (l --> 0) {
                                        *cp =   Math.Atan2 (*cp  , val);
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