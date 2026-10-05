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


        /// <summary>Apply an arbitrary function to two arrays</summary>
        /// <param name="func">A function c = f(a,b), which will be applied to elements in A and B</param>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>The combination of A and B. The result and size depends on the inputs:<list type="table">
        /// <item>
        ///     <term>size(A) == size(B)</term>
        ///     <description>Same size as A/B, elementwise combination of A and B.</description>
        /// </item>
        /// <item>
        ///     <term>isscalar(A) || isscalar(B)</term>
        ///     <description>Same size as A or B, whichever is not a scalar, the scalar value being applied to each element 
        ///     (i.e. if the non-scalar input is empty, the result is empty).</description>
        /// </item>
        /// <item>
        ///     <term>All other cases</term>
        ///     <description>If A or B is a colum vector and the other parameter is an array with a matching column length, the vector is used to operate on all columns of the array. 
        /// Similarly, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length.</description>
        /// </item>
        /// </list></returns>
        /// <remarks><para>The <c>apply</c> function is also implemented for input if e.g. sizes (mxn) and (mx1). 
        /// In this case the vector argument will be combined to each column, resulting in an (mxn) array. 
        /// This feature is, however, officiallny not supported.</para></remarks>
        public unsafe static ILRetArray<double> apply(Func<double, double, double> func, ILInArray<double> A, ILInArray<double> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
                
                double[] retArr;
                
                double[] arrA = A.GetArrayForRead();
                
                double[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {

                        return new ILRetArray<double>(new double[1] { func(A.GetValue(0), B.GetValue(0)) }, A.Size);
                    } else if (B.IsEmpty) {
                        return ILRetArray<double>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<double>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<double>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<double>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return applyEx(func,A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr)) {
                            mode = BinOpItMode.AAIB;
                        } else {
                            retArr = ILMemoryPool.Pool.New<double>(outLen);
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
                    
                    double* cLast, cp = (double*)range.Item5 + range.Item1;
                    
                    double scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                            
                            double* bp = ((double*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, *bp++);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.AAIB:
                            
                            double* ap = ((double*)range.Item3 + range.Item1);
                            while (cp < cLast) {

                                *cp = func(*ap++, *cp);
                                cp++;

                            }
                            //ap = ((double*)range.Item3 + range.Item1);
                            //for (int i2 = range.Item2; i2-- > 0; ) {
                            //    *(cp + i2) = *(ap + i2) - *(cp + i2);
                            //}
                            //int ie = range.Item1 + range.Item2-1;
                            //double[] locRetArr = retArr;
                            //for (int i2 = range.Item1; i2 < locRetArr.Length; i2++) {
                            //    locRetArr[i2] = arrA[i2] - locRetArr[i2];
                            //    if (i2 >= ie) break; 
                            //}

                            break;
                        case BinOpItMode.AAN:
                            ap = ((double*)range.Item3 + range.Item1);
                            bp = ((double*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, *bp++);
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((double*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((double*)range.Item3 + range.Item1);
                            scalar = *((double*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, scalar);
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((double*)range.Item3);
                            while (cp < cLast) {
                               
                                *cp =   func(scalar, *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((double*)range.Item3);
                            bp = ((double*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(scalar, *bp++);
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
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
                
                // retStorage.PendingEvents = new System.Threading.CountdownEvent(workItemCount); 

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
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    System.Threading.SpinWait.SpinUntil(() => {
                        return workerCount <= 0;
                    });
                    //while (workerCount > 0) ; 
                }

                #endregion
                return new ILRetArray<double>(retStorage);
            }
        }

        private static unsafe ILRetArray<double> applyEx(Func<double, double, double> applyFunc, ILInArray<double> A, ILInArray<double> B) {
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
            for (int _L = 0; _L < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); _L++) {
                if (A.S[_L] != B.S[_L]) {
                    if (dim >= 0 || (A.S[_L] != 1 && B.S[_L] != 1)) {
                        throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                    }
                    dim = _L;
                }
            }
            if (dim > 1)
                throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
            #endregion

            #region parameter preparation

            
            double[] retArr;

            
            double[] arrA = A.GetArrayForRead();

            
            double[] arrB = B.GetArrayForRead();
            ILSize outDims;
            BinOptItExMode mode;
            int arrInc = 0;
            int arrStepInc = 0;
            int dimLen = 0;
            if (A.IsVector) {
                outDims = B.S;
                if (!B.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<double>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                } else {
                    mode = BinOptItExMode.VAI;
                }
                dimLen = A.Length;
            } else if (B.IsVector) {
                outDims = A.S;
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<double>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                } else {
                    mode = BinOptItExMode.AVI;
                }
                dimLen = B.Length;
            } else {
                throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
            }
            arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
            arrStepInc = outDims.SequentialIndexDistance(dim);
            #endregion

            #region worker loops definition
            ILDenseStorage<double> retStorage = new ILDenseStorage<double>(retArr, outDims);
            int workerCount = 1;
            Action<object> worker = data => {
                // expects: iStart, iLen, ap, bp, cp
                Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                    (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
                
                double* ap;
                
                double* bp;
                
                double* cp;
                switch (mode) {
                    case BinOptItExMode.VAN:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (double*)range.Item3;
                            bp = (double*)range.Item4 + range.Item1 + s * arrStepInc; ;
                            cp = (double*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *bp);
                                ap++;
                                bp += arrInc;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.VAI:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (double*)range.Item3;
                            cp = (double*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *cp);
                                ap++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVN:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (double*)range.Item3 + range.Item1 + s * arrStepInc;
                            bp = (double*)range.Item4;
                            cp = (double*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *bp);
                                ap += arrInc;
                                bp++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVI:
                        for (int s = 0; s < range.Item2; s++) {
                            bp = (double*)range.Item4;
                            cp = (double*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*cp, *bp);
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

            fixed (double* arrAP = arrA)
            fixed (double* arrBP = arrB)
            fixed (double* retArrP = retArr) {

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

            return new ILRetArray<double>(retStorage);
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>Apply an arbitrary function to two arrays</summary>
        /// <param name="func">A function c = f(a,b), which will be applied to elements in A and B</param>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>The combination of A and B. The result and size depends on the inputs:<list type="table">
        /// <item>
        ///     <term>size(A) == size(B)</term>
        ///     <description>Same size as A/B, elementwise combination of A and B.</description>
        /// </item>
        /// <item>
        ///     <term>isscalar(A) || isscalar(B)</term>
        ///     <description>Same size as A or B, whichever is not a scalar, the scalar value being applied to each element 
        ///     (i.e. if the non-scalar input is empty, the result is empty).</description>
        /// </item>
        /// <item>
        ///     <term>All other cases</term>
        ///     <description>If A or B is a colum vector and the other parameter is an array with a matching column length, the vector is used to operate on all columns of the array. 
        /// Similarly, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length.</description>
        /// </item>
        /// </list></returns>
        /// <remarks><para>The <c>apply</c> function is also implemented for input if e.g. sizes (mxn) and (mx1). 
        /// In this case the vector argument will be combined to each column, resulting in an (mxn) array. 
        /// This feature is, however, officiallny not supported.</para></remarks>
        public unsafe static ILRetArray<float> apply(Func<float, float, float> func, ILInArray<float> A, ILInArray<float> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
               
                float[] retArr;
               
                float[] arrA = A.GetArrayForRead();
               
                float[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {

                        return new ILRetArray<float>(new float[1] { func(A.GetValue(0), B.GetValue(0)) }, A.Size);
                    } else if (B.IsEmpty) {
                        return ILRetArray<float>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<float>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<float>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<float>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return applyEx(func,A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr)) {
                            mode = BinOpItMode.AAIB;
                        } else {
                            retArr = ILMemoryPool.Pool.New<float>(outLen);
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
                   
                    float* cLast, cp = (float*)range.Item5 + range.Item1;
                   
                    float scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           
                            float* bp = ((float*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, *bp++);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.AAIB:
                           
                            float* ap = ((float*)range.Item3 + range.Item1);
                            while (cp < cLast) {

                                *cp = func(*ap++, *cp);
                                cp++;

                            }
                            //ap = ((double*)range.Item3 + range.Item1);
                            //for (int i2 = range.Item2; i2-- > 0; ) {
                            //    *(cp + i2) = *(ap + i2) - *(cp + i2);
                            //}
                            //int ie = range.Item1 + range.Item2-1;
                            //double[] locRetArr = retArr;
                            //for (int i2 = range.Item1; i2 < locRetArr.Length; i2++) {
                            //    locRetArr[i2] = arrA[i2] - locRetArr[i2];
                            //    if (i2 >= ie) break; 
                            //}

                            break;
                        case BinOpItMode.AAN:
                            ap = ((float*)range.Item3 + range.Item1);
                            bp = ((float*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, *bp++);
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((float*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((float*)range.Item3 + range.Item1);
                            scalar = *((float*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, scalar);
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((float*)range.Item3);
                            while (cp < cLast) {
                               
                                *cp =   func(scalar, *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((float*)range.Item3);
                            bp = ((float*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(scalar, *bp++);
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
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
                
                // retStorage.PendingEvents = new System.Threading.CountdownEvent(workItemCount); 

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
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    System.Threading.SpinWait.SpinUntil(() => {
                        return workerCount <= 0;
                    });
                    //while (workerCount > 0) ; 
                }

                #endregion
                return new ILRetArray<float>(retStorage);
            }
        }

        private static unsafe ILRetArray<float> applyEx(Func<float, float, float> applyFunc, ILInArray<float> A, ILInArray<float> B) {
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
            for (int _L = 0; _L < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); _L++) {
                if (A.S[_L] != B.S[_L]) {
                    if (dim >= 0 || (A.S[_L] != 1 && B.S[_L] != 1)) {
                        throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                    }
                    dim = _L;
                }
            }
            if (dim > 1)
                throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
            #endregion

            #region parameter preparation

           
            float[] retArr;

           
            float[] arrA = A.GetArrayForRead();

           
            float[] arrB = B.GetArrayForRead();
            ILSize outDims;
            BinOptItExMode mode;
            int arrInc = 0;
            int arrStepInc = 0;
            int dimLen = 0;
            if (A.IsVector) {
                outDims = B.S;
                if (!B.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<float>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                } else {
                    mode = BinOptItExMode.VAI;
                }
                dimLen = A.Length;
            } else if (B.IsVector) {
                outDims = A.S;
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<float>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                } else {
                    mode = BinOptItExMode.AVI;
                }
                dimLen = B.Length;
            } else {
                throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
            }
            arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
            arrStepInc = outDims.SequentialIndexDistance(dim);
            #endregion

            #region worker loops definition
            ILDenseStorage<float> retStorage = new ILDenseStorage<float>(retArr, outDims);
            int workerCount = 1;
            Action<object> worker = data => {
                // expects: iStart, iLen, ap, bp, cp
                Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                    (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
               
                float* ap;
               
                float* bp;
               
                float* cp;
                switch (mode) {
                    case BinOptItExMode.VAN:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (float*)range.Item3;
                            bp = (float*)range.Item4 + range.Item1 + s * arrStepInc; ;
                            cp = (float*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *bp);
                                ap++;
                                bp += arrInc;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.VAI:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (float*)range.Item3;
                            cp = (float*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *cp);
                                ap++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVN:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (float*)range.Item3 + range.Item1 + s * arrStepInc;
                            bp = (float*)range.Item4;
                            cp = (float*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *bp);
                                ap += arrInc;
                                bp++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVI:
                        for (int s = 0; s < range.Item2; s++) {
                            bp = (float*)range.Item4;
                            cp = (float*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*cp, *bp);
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

            fixed (float* arrAP = arrA)
            fixed (float* arrBP = arrB)
            fixed (float* retArrP = retArr) {

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

            return new ILRetArray<float>(retStorage);
        }
        /// <summary>Apply an arbitrary function to two arrays</summary>
        /// <param name="func">A function c = f(a,b), which will be applied to elements in A and B</param>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>The combination of A and B. The result and size depends on the inputs:<list type="table">
        /// <item>
        ///     <term>size(A) == size(B)</term>
        ///     <description>Same size as A/B, elementwise combination of A and B.</description>
        /// </item>
        /// <item>
        ///     <term>isscalar(A) || isscalar(B)</term>
        ///     <description>Same size as A or B, whichever is not a scalar, the scalar value being applied to each element 
        ///     (i.e. if the non-scalar input is empty, the result is empty).</description>
        /// </item>
        /// <item>
        ///     <term>All other cases</term>
        ///     <description>If A or B is a colum vector and the other parameter is an array with a matching column length, the vector is used to operate on all columns of the array. 
        /// Similarly, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length.</description>
        /// </item>
        /// </list></returns>
        /// <remarks><para>The <c>apply</c> function is also implemented for input if e.g. sizes (mxn) and (mx1). 
        /// In this case the vector argument will be combined to each column, resulting in an (mxn) array. 
        /// This feature is, however, officiallny not supported.</para></remarks>
        public unsafe static ILRetArray<fcomplex> apply(Func<fcomplex, fcomplex, fcomplex> func, ILInArray<fcomplex> A, ILInArray<fcomplex> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
               
                fcomplex[] retArr;
               
                fcomplex[] arrA = A.GetArrayForRead();
               
                fcomplex[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {

                        return new ILRetArray<fcomplex>(new fcomplex[1] { func(A.GetValue(0), B.GetValue(0)) }, A.Size);
                    } else if (B.IsEmpty) {
                        return ILRetArray<fcomplex>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<fcomplex>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<fcomplex>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<fcomplex>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return applyEx(func,A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr)) {
                            mode = BinOpItMode.AAIB;
                        } else {
                            retArr = ILMemoryPool.Pool.New<fcomplex>(outLen);
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
                   
                    fcomplex* cLast, cp = (fcomplex*)range.Item5 + range.Item1;
                   
                    fcomplex scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           
                            fcomplex* bp = ((fcomplex*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, *bp++);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.AAIB:
                           
                            fcomplex* ap = ((fcomplex*)range.Item3 + range.Item1);
                            while (cp < cLast) {

                                *cp = func(*ap++, *cp);
                                cp++;

                            }
                            //ap = ((double*)range.Item3 + range.Item1);
                            //for (int i2 = range.Item2; i2-- > 0; ) {
                            //    *(cp + i2) = *(ap + i2) - *(cp + i2);
                            //}
                            //int ie = range.Item1 + range.Item2-1;
                            //double[] locRetArr = retArr;
                            //for (int i2 = range.Item1; i2 < locRetArr.Length; i2++) {
                            //    locRetArr[i2] = arrA[i2] - locRetArr[i2];
                            //    if (i2 >= ie) break; 
                            //}

                            break;
                        case BinOpItMode.AAN:
                            ap = ((fcomplex*)range.Item3 + range.Item1);
                            bp = ((fcomplex*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, *bp++);
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((fcomplex*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((fcomplex*)range.Item3 + range.Item1);
                            scalar = *((fcomplex*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, scalar);
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((fcomplex*)range.Item3);
                            while (cp < cLast) {
                               
                                *cp =   func(scalar, *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((fcomplex*)range.Item3);
                            bp = ((fcomplex*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(scalar, *bp++);
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
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
                
                // retStorage.PendingEvents = new System.Threading.CountdownEvent(workItemCount); 

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
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    System.Threading.SpinWait.SpinUntil(() => {
                        return workerCount <= 0;
                    });
                    //while (workerCount > 0) ; 
                }

                #endregion
                return new ILRetArray<fcomplex>(retStorage);
            }
        }

        private static unsafe ILRetArray<fcomplex> applyEx(Func<fcomplex, fcomplex, fcomplex> applyFunc, ILInArray<fcomplex> A, ILInArray<fcomplex> B) {
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
            for (int _L = 0; _L < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); _L++) {
                if (A.S[_L] != B.S[_L]) {
                    if (dim >= 0 || (A.S[_L] != 1 && B.S[_L] != 1)) {
                        throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                    }
                    dim = _L;
                }
            }
            if (dim > 1)
                throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
            #endregion

            #region parameter preparation

           
            fcomplex[] retArr;

           
            fcomplex[] arrA = A.GetArrayForRead();

           
            fcomplex[] arrB = B.GetArrayForRead();
            ILSize outDims;
            BinOptItExMode mode;
            int arrInc = 0;
            int arrStepInc = 0;
            int dimLen = 0;
            if (A.IsVector) {
                outDims = B.S;
                if (!B.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<fcomplex>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                } else {
                    mode = BinOptItExMode.VAI;
                }
                dimLen = A.Length;
            } else if (B.IsVector) {
                outDims = A.S;
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<fcomplex>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                } else {
                    mode = BinOptItExMode.AVI;
                }
                dimLen = B.Length;
            } else {
                throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
            }
            arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
            arrStepInc = outDims.SequentialIndexDistance(dim);
            #endregion

            #region worker loops definition
            ILDenseStorage<fcomplex> retStorage = new ILDenseStorage<fcomplex>(retArr, outDims);
            int workerCount = 1;
            Action<object> worker = data => {
                // expects: iStart, iLen, ap, bp, cp
                Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                    (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
               
                fcomplex* ap;
               
                fcomplex* bp;
               
                fcomplex* cp;
                switch (mode) {
                    case BinOptItExMode.VAN:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (fcomplex*)range.Item3;
                            bp = (fcomplex*)range.Item4 + range.Item1 + s * arrStepInc; ;
                            cp = (fcomplex*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *bp);
                                ap++;
                                bp += arrInc;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.VAI:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (fcomplex*)range.Item3;
                            cp = (fcomplex*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *cp);
                                ap++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVN:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (fcomplex*)range.Item3 + range.Item1 + s * arrStepInc;
                            bp = (fcomplex*)range.Item4;
                            cp = (fcomplex*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *bp);
                                ap += arrInc;
                                bp++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVI:
                        for (int s = 0; s < range.Item2; s++) {
                            bp = (fcomplex*)range.Item4;
                            cp = (fcomplex*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*cp, *bp);
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

            fixed (fcomplex* arrAP = arrA)
            fixed (fcomplex* arrBP = arrB)
            fixed (fcomplex* retArrP = retArr) {

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

            return new ILRetArray<fcomplex>(retStorage);
        }
        /// <summary>Apply an arbitrary function to two arrays</summary>
        /// <param name="func">A function c = f(a,b), which will be applied to elements in A and B</param>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>The combination of A and B. The result and size depends on the inputs:<list type="table">
        /// <item>
        ///     <term>size(A) == size(B)</term>
        ///     <description>Same size as A/B, elementwise combination of A and B.</description>
        /// </item>
        /// <item>
        ///     <term>isscalar(A) || isscalar(B)</term>
        ///     <description>Same size as A or B, whichever is not a scalar, the scalar value being applied to each element 
        ///     (i.e. if the non-scalar input is empty, the result is empty).</description>
        /// </item>
        /// <item>
        ///     <term>All other cases</term>
        ///     <description>If A or B is a colum vector and the other parameter is an array with a matching column length, the vector is used to operate on all columns of the array. 
        /// Similarly, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length.</description>
        /// </item>
        /// </list></returns>
        /// <remarks><para>The <c>apply</c> function is also implemented for input if e.g. sizes (mxn) and (mx1). 
        /// In this case the vector argument will be combined to each column, resulting in an (mxn) array. 
        /// This feature is, however, officiallny not supported.</para></remarks>
        public unsafe static ILRetArray<complex> apply(Func<complex, complex, complex> func, ILInArray<complex> A, ILInArray<complex> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
               
                complex[] retArr;
               
                complex[] arrA = A.GetArrayForRead();
               
                complex[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {

                        return new ILRetArray<complex>(new complex[1] { func(A.GetValue(0), B.GetValue(0)) }, A.Size);
                    } else if (B.IsEmpty) {
                        return ILRetArray<complex>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<complex>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<complex>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<complex>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return applyEx(func,A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr)) {
                            mode = BinOpItMode.AAIB;
                        } else {
                            retArr = ILMemoryPool.Pool.New<complex>(outLen);
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
                   
                    complex* cLast, cp = (complex*)range.Item5 + range.Item1;
                   
                    complex scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           
                            complex* bp = ((complex*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, *bp++);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.AAIB:
                           
                            complex* ap = ((complex*)range.Item3 + range.Item1);
                            while (cp < cLast) {

                                *cp = func(*ap++, *cp);
                                cp++;

                            }
                            //ap = ((double*)range.Item3 + range.Item1);
                            //for (int i2 = range.Item2; i2-- > 0; ) {
                            //    *(cp + i2) = *(ap + i2) - *(cp + i2);
                            //}
                            //int ie = range.Item1 + range.Item2-1;
                            //double[] locRetArr = retArr;
                            //for (int i2 = range.Item1; i2 < locRetArr.Length; i2++) {
                            //    locRetArr[i2] = arrA[i2] - locRetArr[i2];
                            //    if (i2 >= ie) break; 
                            //}

                            break;
                        case BinOpItMode.AAN:
                            ap = ((complex*)range.Item3 + range.Item1);
                            bp = ((complex*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, *bp++);
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((complex*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((complex*)range.Item3 + range.Item1);
                            scalar = *((complex*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, scalar);
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((complex*)range.Item3);
                            while (cp < cLast) {
                               
                                *cp =   func(scalar, *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((complex*)range.Item3);
                            bp = ((complex*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(scalar, *bp++);
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
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
                
                // retStorage.PendingEvents = new System.Threading.CountdownEvent(workItemCount); 

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
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    System.Threading.SpinWait.SpinUntil(() => {
                        return workerCount <= 0;
                    });
                    //while (workerCount > 0) ; 
                }

                #endregion
                return new ILRetArray<complex>(retStorage);
            }
        }

        private static unsafe ILRetArray<complex> applyEx(Func<complex, complex, complex> applyFunc, ILInArray<complex> A, ILInArray<complex> B) {
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
            for (int _L = 0; _L < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); _L++) {
                if (A.S[_L] != B.S[_L]) {
                    if (dim >= 0 || (A.S[_L] != 1 && B.S[_L] != 1)) {
                        throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                    }
                    dim = _L;
                }
            }
            if (dim > 1)
                throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
            #endregion

            #region parameter preparation

           
            complex[] retArr;

           
            complex[] arrA = A.GetArrayForRead();

           
            complex[] arrB = B.GetArrayForRead();
            ILSize outDims;
            BinOptItExMode mode;
            int arrInc = 0;
            int arrStepInc = 0;
            int dimLen = 0;
            if (A.IsVector) {
                outDims = B.S;
                if (!B.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<complex>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                } else {
                    mode = BinOptItExMode.VAI;
                }
                dimLen = A.Length;
            } else if (B.IsVector) {
                outDims = A.S;
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<complex>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                } else {
                    mode = BinOptItExMode.AVI;
                }
                dimLen = B.Length;
            } else {
                throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
            }
            arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
            arrStepInc = outDims.SequentialIndexDistance(dim);
            #endregion

            #region worker loops definition
            ILDenseStorage<complex> retStorage = new ILDenseStorage<complex>(retArr, outDims);
            int workerCount = 1;
            Action<object> worker = data => {
                // expects: iStart, iLen, ap, bp, cp
                Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                    (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
               
                complex* ap;
               
                complex* bp;
               
                complex* cp;
                switch (mode) {
                    case BinOptItExMode.VAN:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (complex*)range.Item3;
                            bp = (complex*)range.Item4 + range.Item1 + s * arrStepInc; ;
                            cp = (complex*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *bp);
                                ap++;
                                bp += arrInc;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.VAI:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (complex*)range.Item3;
                            cp = (complex*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *cp);
                                ap++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVN:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (complex*)range.Item3 + range.Item1 + s * arrStepInc;
                            bp = (complex*)range.Item4;
                            cp = (complex*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *bp);
                                ap += arrInc;
                                bp++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVI:
                        for (int s = 0; s < range.Item2; s++) {
                            bp = (complex*)range.Item4;
                            cp = (complex*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*cp, *bp);
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

            fixed (complex* arrAP = arrA)
            fixed (complex* arrBP = arrB)
            fixed (complex* retArrP = retArr) {

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

            return new ILRetArray<complex>(retStorage);
        }
        /// <summary>Apply an arbitrary function to two arrays</summary>
        /// <param name="func">A function c = f(a,b), which will be applied to elements in A and B</param>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>The combination of A and B. The result and size depends on the inputs:<list type="table">
        /// <item>
        ///     <term>size(A) == size(B)</term>
        ///     <description>Same size as A/B, elementwise combination of A and B.</description>
        /// </item>
        /// <item>
        ///     <term>isscalar(A) || isscalar(B)</term>
        ///     <description>Same size as A or B, whichever is not a scalar, the scalar value being applied to each element 
        ///     (i.e. if the non-scalar input is empty, the result is empty).</description>
        /// </item>
        /// <item>
        ///     <term>All other cases</term>
        ///     <description>If A or B is a colum vector and the other parameter is an array with a matching column length, the vector is used to operate on all columns of the array. 
        /// Similarly, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length.</description>
        /// </item>
        /// </list></returns>
        /// <remarks><para>The <c>apply</c> function is also implemented for input if e.g. sizes (mxn) and (mx1). 
        /// In this case the vector argument will be combined to each column, resulting in an (mxn) array. 
        /// This feature is, however, officiallny not supported.</para></remarks>
        public unsafe static ILRetArray<Int64> apply(Func<Int64, Int64, Int64> func, ILInArray<Int64> A, ILInArray<Int64> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
               
                Int64[] retArr;
               
                Int64[] arrA = A.GetArrayForRead();
               
                Int64[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {

                        return new ILRetArray<Int64>(new Int64[1] { func(A.GetValue(0), B.GetValue(0)) }, A.Size);
                    } else if (B.IsEmpty) {
                        return ILRetArray<Int64>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<Int64>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<Int64>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<Int64>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return applyEx(func,A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr)) {
                            mode = BinOpItMode.AAIB;
                        } else {
                            retArr = ILMemoryPool.Pool.New<Int64>(outLen);
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
                   
                    Int64* cLast, cp = (Int64*)range.Item5 + range.Item1;
                   
                    Int64 scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           
                            Int64* bp = ((Int64*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, *bp++);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.AAIB:
                           
                            Int64* ap = ((Int64*)range.Item3 + range.Item1);
                            while (cp < cLast) {

                                *cp = func(*ap++, *cp);
                                cp++;

                            }
                            //ap = ((double*)range.Item3 + range.Item1);
                            //for (int i2 = range.Item2; i2-- > 0; ) {
                            //    *(cp + i2) = *(ap + i2) - *(cp + i2);
                            //}
                            //int ie = range.Item1 + range.Item2-1;
                            //double[] locRetArr = retArr;
                            //for (int i2 = range.Item1; i2 < locRetArr.Length; i2++) {
                            //    locRetArr[i2] = arrA[i2] - locRetArr[i2];
                            //    if (i2 >= ie) break; 
                            //}

                            break;
                        case BinOpItMode.AAN:
                            ap = ((Int64*)range.Item3 + range.Item1);
                            bp = ((Int64*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, *bp++);
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((Int64*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((Int64*)range.Item3 + range.Item1);
                            scalar = *((Int64*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, scalar);
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((Int64*)range.Item3);
                            while (cp < cLast) {
                               
                                *cp =   func(scalar, *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((Int64*)range.Item3);
                            bp = ((Int64*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(scalar, *bp++);
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
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
                
                // retStorage.PendingEvents = new System.Threading.CountdownEvent(workItemCount); 

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
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    System.Threading.SpinWait.SpinUntil(() => {
                        return workerCount <= 0;
                    });
                    //while (workerCount > 0) ; 
                }

                #endregion
                return new ILRetArray<Int64>(retStorage);
            }
        }

        private static unsafe ILRetArray<Int64> applyEx(Func<Int64, Int64, Int64> applyFunc, ILInArray<Int64> A, ILInArray<Int64> B) {
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
            for (int _L = 0; _L < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); _L++) {
                if (A.S[_L] != B.S[_L]) {
                    if (dim >= 0 || (A.S[_L] != 1 && B.S[_L] != 1)) {
                        throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                    }
                    dim = _L;
                }
            }
            if (dim > 1)
                throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
            #endregion

            #region parameter preparation

           
            Int64[] retArr;

           
            Int64[] arrA = A.GetArrayForRead();

           
            Int64[] arrB = B.GetArrayForRead();
            ILSize outDims;
            BinOptItExMode mode;
            int arrInc = 0;
            int arrStepInc = 0;
            int dimLen = 0;
            if (A.IsVector) {
                outDims = B.S;
                if (!B.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<Int64>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                } else {
                    mode = BinOptItExMode.VAI;
                }
                dimLen = A.Length;
            } else if (B.IsVector) {
                outDims = A.S;
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<Int64>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                } else {
                    mode = BinOptItExMode.AVI;
                }
                dimLen = B.Length;
            } else {
                throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
            }
            arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
            arrStepInc = outDims.SequentialIndexDistance(dim);
            #endregion

            #region worker loops definition
            ILDenseStorage<Int64> retStorage = new ILDenseStorage<Int64>(retArr, outDims);
            int workerCount = 1;
            Action<object> worker = data => {
                // expects: iStart, iLen, ap, bp, cp
                Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                    (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
               
                Int64* ap;
               
                Int64* bp;
               
                Int64* cp;
                switch (mode) {
                    case BinOptItExMode.VAN:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (Int64*)range.Item3;
                            bp = (Int64*)range.Item4 + range.Item1 + s * arrStepInc; ;
                            cp = (Int64*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *bp);
                                ap++;
                                bp += arrInc;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.VAI:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (Int64*)range.Item3;
                            cp = (Int64*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *cp);
                                ap++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVN:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (Int64*)range.Item3 + range.Item1 + s * arrStepInc;
                            bp = (Int64*)range.Item4;
                            cp = (Int64*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *bp);
                                ap += arrInc;
                                bp++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVI:
                        for (int s = 0; s < range.Item2; s++) {
                            bp = (Int64*)range.Item4;
                            cp = (Int64*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*cp, *bp);
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

            fixed (Int64* arrAP = arrA)
            fixed (Int64* arrBP = arrB)
            fixed (Int64* retArrP = retArr) {

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

            return new ILRetArray<Int64>(retStorage);
        }
        /// <summary>Apply an arbitrary function to two arrays</summary>
        /// <param name="func">A function c = f(a,b), which will be applied to elements in A and B</param>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>The combination of A and B. The result and size depends on the inputs:<list type="table">
        /// <item>
        ///     <term>size(A) == size(B)</term>
        ///     <description>Same size as A/B, elementwise combination of A and B.</description>
        /// </item>
        /// <item>
        ///     <term>isscalar(A) || isscalar(B)</term>
        ///     <description>Same size as A or B, whichever is not a scalar, the scalar value being applied to each element 
        ///     (i.e. if the non-scalar input is empty, the result is empty).</description>
        /// </item>
        /// <item>
        ///     <term>All other cases</term>
        ///     <description>If A or B is a colum vector and the other parameter is an array with a matching column length, the vector is used to operate on all columns of the array. 
        /// Similarly, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length.</description>
        /// </item>
        /// </list></returns>
        /// <remarks><para>The <c>apply</c> function is also implemented for input if e.g. sizes (mxn) and (mx1). 
        /// In this case the vector argument will be combined to each column, resulting in an (mxn) array. 
        /// This feature is, however, officiallny not supported.</para></remarks>
        public unsafe static ILRetArray<Int32> apply(Func<Int32, Int32, Int32> func, ILInArray<Int32> A, ILInArray<Int32> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
               
                Int32[] retArr;
               
                Int32[] arrA = A.GetArrayForRead();
               
                Int32[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {

                        return new ILRetArray<Int32>(new Int32[1] { func(A.GetValue(0), B.GetValue(0)) }, A.Size);
                    } else if (B.IsEmpty) {
                        return ILRetArray<Int32>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<Int32>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<Int32>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<Int32>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return applyEx(func,A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr)) {
                            mode = BinOpItMode.AAIB;
                        } else {
                            retArr = ILMemoryPool.Pool.New<Int32>(outLen);
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
                   
                    Int32* cLast, cp = (Int32*)range.Item5 + range.Item1;
                   
                    Int32 scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           
                            Int32* bp = ((Int32*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, *bp++);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.AAIB:
                           
                            Int32* ap = ((Int32*)range.Item3 + range.Item1);
                            while (cp < cLast) {

                                *cp = func(*ap++, *cp);
                                cp++;

                            }
                            //ap = ((double*)range.Item3 + range.Item1);
                            //for (int i2 = range.Item2; i2-- > 0; ) {
                            //    *(cp + i2) = *(ap + i2) - *(cp + i2);
                            //}
                            //int ie = range.Item1 + range.Item2-1;
                            //double[] locRetArr = retArr;
                            //for (int i2 = range.Item1; i2 < locRetArr.Length; i2++) {
                            //    locRetArr[i2] = arrA[i2] - locRetArr[i2];
                            //    if (i2 >= ie) break; 
                            //}

                            break;
                        case BinOpItMode.AAN:
                            ap = ((Int32*)range.Item3 + range.Item1);
                            bp = ((Int32*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, *bp++);
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((Int32*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((Int32*)range.Item3 + range.Item1);
                            scalar = *((Int32*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, scalar);
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((Int32*)range.Item3);
                            while (cp < cLast) {
                               
                                *cp =   func(scalar, *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((Int32*)range.Item3);
                            bp = ((Int32*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(scalar, *bp++);
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
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
                
                // retStorage.PendingEvents = new System.Threading.CountdownEvent(workItemCount); 

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
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    System.Threading.SpinWait.SpinUntil(() => {
                        return workerCount <= 0;
                    });
                    //while (workerCount > 0) ; 
                }

                #endregion
                return new ILRetArray<Int32>(retStorage);
            }
        }

        private static unsafe ILRetArray<Int32> applyEx(Func<Int32, Int32, Int32> applyFunc, ILInArray<Int32> A, ILInArray<Int32> B) {
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
            for (int _L = 0; _L < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); _L++) {
                if (A.S[_L] != B.S[_L]) {
                    if (dim >= 0 || (A.S[_L] != 1 && B.S[_L] != 1)) {
                        throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                    }
                    dim = _L;
                }
            }
            if (dim > 1)
                throw new ILArgumentException("singleton dimension expansion currently is only supported for colum- and row vectors");
            #endregion

            #region parameter preparation

           
            Int32[] retArr;

           
            Int32[] arrA = A.GetArrayForRead();

           
            Int32[] arrB = B.GetArrayForRead();
            ILSize outDims;
            BinOptItExMode mode;
            int arrInc = 0;
            int arrStepInc = 0;
            int dimLen = 0;
            if (A.IsVector) {
                outDims = B.S;
                if (!B.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<Int32>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                } else {
                    mode = BinOptItExMode.VAI;
                }
                dimLen = A.Length;
            } else if (B.IsVector) {
                outDims = A.S;
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<Int32>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                } else {
                    mode = BinOptItExMode.AVI;
                }
                dimLen = B.Length;
            } else {
                throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
            }
            arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
            arrStepInc = outDims.SequentialIndexDistance(dim);
            #endregion

            #region worker loops definition
            ILDenseStorage<Int32> retStorage = new ILDenseStorage<Int32>(retArr, outDims);
            int workerCount = 1;
            Action<object> worker = data => {
                // expects: iStart, iLen, ap, bp, cp
                Tuple<int, int, IntPtr, IntPtr, IntPtr> range =
                    (Tuple<int, int, IntPtr, IntPtr, IntPtr>)data;
               
                Int32* ap;
               
                Int32* bp;
               
                Int32* cp;
                switch (mode) {
                    case BinOptItExMode.VAN:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (Int32*)range.Item3;
                            bp = (Int32*)range.Item4 + range.Item1 + s * arrStepInc; ;
                            cp = (Int32*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *bp);
                                ap++;
                                bp += arrInc;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.VAI:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (Int32*)range.Item3;
                            cp = (Int32*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *cp);
                                ap++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVN:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (Int32*)range.Item3 + range.Item1 + s * arrStepInc;
                            bp = (Int32*)range.Item4;
                            cp = (Int32*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *bp);
                                ap += arrInc;
                                bp++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVI:
                        for (int s = 0; s < range.Item2; s++) {
                            bp = (Int32*)range.Item4;
                            cp = (Int32*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*cp, *bp);
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

            fixed (Int32* arrAP = arrA)
            fixed (Int32* arrBP = arrB)
            fixed (Int32* retArrP = retArr) {

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

            return new ILRetArray<Int32>(retStorage);
        }
        /// <summary>Apply an arbitrary function to two arrays</summary>
        /// <param name="func">A function c = f(a,b), which will be applied to elements in A and B</param>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>The combination of A and B. The result and size depends on the inputs:<list type="table">
        /// <item>
        ///     <term>size(A) == size(B)</term>
        ///     <description>Same size as A/B, elementwise combination of A and B.</description>
        /// </item>
        /// <item>
        ///     <term>isscalar(A) || isscalar(B)</term>
        ///     <description>Same size as A or B, whichever is not a scalar, the scalar value being applied to each element 
        ///     (i.e. if the non-scalar input is empty, the result is empty).</description>
        /// </item>
        /// <item>
        ///     <term>All other cases</term>
        ///     <description>If A or B is a colum vector and the other parameter is an array with a matching column length, the vector is used to operate on all columns of the array. 
        /// Similarly, if one parameter is a row vector, it is used to operate along the rows of the other array if its number of columns matches the vector length.</description>
        /// </item>
        /// </list></returns>
        /// <remarks><para>The <c>apply</c> function is also implemented for input if e.g. sizes (mxn) and (mx1). 
        /// In this case the vector argument will be combined to each column, resulting in an (mxn) array. 
        /// This feature is, however, officiallny not supported.</para></remarks>
        public unsafe static ILRetArray<byte> apply(Func<byte, byte, byte> func, ILInArray<byte> A, ILInArray<byte> B) {
            using (ILScope.Enter(A, B)) {
                int outLen;
                BinOpItMode mode;
               
                byte[] retArr;
               
                byte[] arrA = A.GetArrayForRead();
               
                byte[] arrB = B.GetArrayForRead();
                ILSize outDims;
                #region determine operation mode
                if (A.IsScalar) {
                    outDims = B.Size;
                    if (B.IsScalar) {

                        return new ILRetArray<byte>(new byte[1] { func(A.GetValue(0), B.GetValue(0)) }, A.Size);
                    } else if (B.IsEmpty) {
                        return ILRetArray<byte>.empty(outDims);
                    } else {
                        outLen = outDims.NumberOfElements;
                        if (!B.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<byte>(outLen);
                            mode = BinOpItMode.SAN;
                        } else {
                            mode = BinOpItMode.SAI;
                        }
                    }
                } else {
                    outDims = A.Size;
                    if (B.IsScalar) {
                        if (A.IsEmpty) {
                            return ILRetArray<byte>.empty(A.Size);
                        }
                        outLen = A.S.NumberOfElements;
                        if (!A.TryGetStorage4InplaceOp(out retArr)) {
                            retArr = ILMemoryPool.Pool.New<byte>(outLen);
                            mode = BinOpItMode.ASN;
                        } else {
                            mode = BinOpItMode.ASI;
                        }
                    } else {
                        // array + array 
                        if (!A.Size.IsSameSize(B.Size)) {
                            return applyEx(func,A,B); 
                        }
                        outLen = A.S.NumberOfElements;
                        if (A.TryGetStorage4InplaceOp(out retArr))
                            mode = BinOpItMode.AAIA;
                        else if (B.TryGetStorage4InplaceOp(out retArr)) {
                            mode = BinOpItMode.AAIB;
                        } else {
                            retArr = ILMemoryPool.Pool.New<byte>(outLen);
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
                   
                    byte* cLast, cp = (byte*)range.Item5 + range.Item1;
                   
                    byte scalar;
                    cLast = cp + range.Item2;
                    #region loops
                    switch (mode) {
                        case BinOpItMode.AAIA:
                           
                            byte* bp = ((byte*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, *bp++);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.AAIB:
                           
                            byte* ap = ((byte*)range.Item3 + range.Item1);
                            while (cp < cLast) {

                                *cp = func(*ap++, *cp);
                                cp++;

                            }
                            //ap = ((double*)range.Item3 + range.Item1);
                            //for (int i2 = range.Item2; i2-- > 0; ) {
                            //    *(cp + i2) = *(ap + i2) - *(cp + i2);
                            //}
                            //int ie = range.Item1 + range.Item2-1;
                            //double[] locRetArr = retArr;
                            //for (int i2 = range.Item1; i2 < locRetArr.Length; i2++) {
                            //    locRetArr[i2] = arrA[i2] - locRetArr[i2];
                            //    if (i2 >= ie) break; 
                            //}

                            break;
                        case BinOpItMode.AAN:
                            ap = ((byte*)range.Item3 + range.Item1);
                            bp = ((byte*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, *bp++);
                            }
                            break;
                        case BinOpItMode.ASI:
                            scalar = *((byte*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp =   func(*cp, scalar);
                                cp++; 
                            }
                            break;
                        case BinOpItMode.ASN:
                            ap = ((byte*)range.Item3 + range.Item1);
                            scalar = *((byte*)range.Item4);
                            while (cp < cLast) {
                               
                                *cp++ =   func(*ap++, scalar);
                            }
                            break;
                        case BinOpItMode.SAI:
                            scalar = *((byte*)range.Item3);
                            while (cp < cLast) {
                               
                                *cp =   func(scalar, *cp);
                                cp++;
                            }
                            break;
                        case BinOpItMode.SAN:
                            scalar = *((byte*)range.Item3);
                            bp = ((byte*)range.Item4 + range.Item1);
                            while (cp < cLast) {
                               
                                *cp++ =   func(scalar, *bp++);
                            }
                            break;
                        default:
                            break;
                    }
                    #endregion
                    System.Threading.Interlocked.Decrement(ref workerCount);
                    //retStorage.PendingEvents.Signal(); 
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
                
                // retStorage.PendingEvents = new System.Threading.CountdownEvent(workItemCount); 

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
                    //System.Threading.Interlocked.Increment(ref retStorage.PendingTasks);
                    worker(new Tuple<int, int, IntPtr, IntPtr, IntPtr, BinOpItMode>
                                (i * workItemLength, outLen - i * workItemLength, (IntPtr)arrAP, (IntPtr)arrBP, (IntPtr)retArrP, mode));

                    System.Threading.SpinWait.SpinUntil(() => {
                        return workerCount <= 0;
                    });
                    //while (workerCount > 0) ; 
                }

                #endregion
                return new ILRetArray<byte>(retStorage);
            }
        }

        private static unsafe ILRetArray<byte> applyEx(Func<byte, byte, byte> applyFunc, ILInArray<byte> A, ILInArray<byte> B) {
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
            for (int _L = 0; _L < Math.Max(A.S.NumberOfDimensions, B.S.NumberOfDimensions); _L++) {
                if (A.S[_L] != B.S[_L]) {
                    if (dim >= 0 || (A.S[_L] != 1 && B.S[_L] != 1)) {
                        throw new ILArgumentException("A and B must have the same size except for one singleton dimension in A or B");
                    }
                    dim = _L;
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
                if (!B.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.VAN;
                } else {
                    mode = BinOptItExMode.VAI;
                }
                dimLen = A.Length;
            } else if (B.IsVector) {
                outDims = A.S;
                if (!A.TryGetStorage4InplaceOp(out retArr)) {
                    retArr = ILMemoryPool.Pool.New<byte>(outDims.NumberOfElements);
                    mode = BinOptItExMode.AVN;
                } else {
                    mode = BinOptItExMode.AVI;
                }
                dimLen = B.Length;
            } else {
                throw new ILArgumentException("A and B must have the same size except for one simgleton dimension in A or B");
            }
            arrInc = (dim == 0) ? outDims.SequentialIndexDistance(1) : outDims.SequentialIndexDistance(0);
            arrStepInc = outDims.SequentialIndexDistance(dim);
            #endregion

            #region worker loops definition
            ILDenseStorage<byte> retStorage = new ILDenseStorage<byte>(retArr, outDims);
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

                                *cp = applyFunc(*ap, *bp);
                                ap++;
                                bp += arrInc;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.VAI:
                        for (int s = 0; s < range.Item2; s++) {
                            ap = (byte*)range.Item3;
                            cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*ap, *cp);
                                ap++;
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

                                *cp = applyFunc(*ap, *bp);
                                ap += arrInc;
                                bp++;
                                cp += arrInc;
                            }
                        }
                        break;
                    case BinOptItExMode.AVI:
                        for (int s = 0; s < range.Item2; s++) {
                            bp = (byte*)range.Item4;
                            cp = (byte*)range.Item5 + range.Item1 + s * arrStepInc;
                            for (int l = 0; l < dimLen; l++) {

                                *cp = applyFunc(*cp, *bp);
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

            fixed (byte* arrAP = arrA)
            fixed (byte* arrBP = arrB)
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

            return new ILRetArray<byte>(retStorage);
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}