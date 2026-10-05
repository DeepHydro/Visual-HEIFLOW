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
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Exceptions;



namespace ILNumerics  {
    
    public partial class ILMath {
    

        /// <summary>
        /// Take n-th derivative
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <param name="N">[Optional] Degree of derivates. If not specified N=1 is assumed.</param>
        /// <returns>Array with first derivative of A along dimension <c>dim</c> or of first non singleton dimension respectively</returns>
        /// <remarks>N must be a number in range 1..L, where L is the length of A.Dimensions[dim]. 
        /// Otherwise an empty array will be returned.
        /// <para>If A is empty or scalar, or if N exceeds the length the specified dimension of A, 
        /// an empty array will be returned.</para></remarks>
        public static ILRetArray< double > diff(ILInArray< double > A, int N = 1, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null)) 
                    throw new ILArgumentException ("diff: input array A must not be null"); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< double >(new ILSize(outDims)); 
                }
                if (A.IsScalar) return empty< double >(ILSize.Empty00);
                if (A.IsEmpty) {
                    int [] retDim = A.Size.ToIntArray();
                    if (retDim[dim] > 0) 
                        retDim[dim]--; 
                    return empty<  double >(new ILSize(retDim)); 
                }
                if (N == 0) 
                    return A.C; 
                if (N < 1 || N > A.Size[dim]) {
                    return empty<  double >(ILSize.Empty00); 
                }
                ILArray< double > ret = A.C; 
                for (int n = 0; n < N; n++) {
                    ret.a = diff(dim,ret);      
                }
                return ret;
            }
        }
        /// <summary>
        /// First derivative along specific dimension
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimensions to create derivative along</param>
        /// <returns>array with first derivative of A along dimension <c>dim</c></returns>
        private static ILRetArray< double > diff(int dim, ILInArray< double > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< double >(A.Size);
                if (A.IsScalar) return empty< double >(ILSize.Empty00);
                if (dim < 0)
                    throw new ILArgumentException("diff: leading dimension out of range!");
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< double >(new ILSize(outDims)); 
                }
                ILSize inDim = A.Size; 
                int[] newDims = inDim.ToIntArray();
                
                if (inDim[dim] == 1) return empty< double >(ILSize.Empty00);
                int newLength;
                 double [] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] --;
                newLength = newLength * newDims[dim];
                retArr = ILMemoryPool.Pool.New< double >(newLength);
                ILSize newDimension = new ILSize(newDims); 
                int leadDimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / leadDimLen;
                int incOut = newDimension.SequentialIndexDistance(dim); 
                 double firstVal, secVal; 
                if (A.IsVector) 
                    return A["1:end"] - A[vec(0,A.Length-2)];     
                if (dim == 0) {
    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( double * pOutArr = retArr)
                        fixed ( double * pInArr = A.GetArrayForRead()) {
                             double * lastElement;
                             double * tmpOut = pOutArr;
                             double * tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                 
                                firstVal = *tmpIn++;
                                while (tmpIn < lastElement) {
                                    secVal = *tmpIn++;
                                    *(tmpOut++) = ( double )(secVal-firstVal);
                                    firstVal = secVal; 
                                }
                            }
                        }
                    }
    #endregion
                } else {
    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( double * pOutArr = retArr)
                        fixed ( double * pInArr = A.GetArrayForRead()) {
                             double * lastElementOut = newLength + pOutArr -1;
                            int inLength = inDim.NumberOfElements -1; 
                             double * lastElementIn = pInArr + inLength; 
                            int inc = inDim.SequentialIndexDistance(dim); 
                             double * tmpOut = pOutArr;
                            int outLength = newLength - 1;
                             double * leadEnd; 
                             double * tmpIn = pInArr;
                            for (int h = nrHigherDims; h--> 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                firstVal = *tmpIn; 
                                tmpIn += inc; 
                                while (tmpIn < leadEnd) {
                                    secVal = *tmpIn; 
                                    *tmpOut = ( double )(secVal - firstVal);
                                    tmpIn += inc;
                                    tmpOut += incOut; 
                                    firstVal = secVal; 
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= inLength; 
                            }
                        }
                    }
    #endregion
                }
                return new ILRetArray< double >(retArr, newDimension);
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Take n-th derivative
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <param name="N">[Optional] Degree of derivates. If not specified N=1 is assumed.</param>
        /// <returns>Array with first derivative of A along dimension <c>dim</c> or of first non singleton dimension respectively</returns>
        /// <remarks>N must be a number in range 1..L, where L is the length of A.Dimensions[dim]. 
        /// Otherwise an empty array will be returned.
        /// <para>If A is empty or scalar, or if N exceeds the length the specified dimension of A, 
        /// an empty array will be returned.</para></remarks>
        public static ILRetArray< Int64 > diff(ILInArray< Int64 > A, int N = 1, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null)) 
                    throw new ILArgumentException ("diff: input array A must not be null"); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< Int64 >(new ILSize(outDims)); 
                }
                if (A.IsScalar) return empty< Int64 >(ILSize.Empty00);
                if (A.IsEmpty) {
                    int [] retDim = A.Size.ToIntArray();
                    if (retDim[dim] > 0) 
                        retDim[dim]--; 
                    return empty<  Int64 >(new ILSize(retDim)); 
                }
                if (N == 0) 
                    return A.C; 
                if (N < 1 || N > A.Size[dim]) {
                    return empty<  Int64 >(ILSize.Empty00); 
                }
                ILArray< Int64 > ret = A.C; 
                for (int n = 0; n < N; n++) {
                    ret.a = diff(dim,ret);      
                }
                return ret;
            }
        }
        /// <summary>
        /// First derivative along specific dimension
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimensions to create derivative along</param>
        /// <returns>array with first derivative of A along dimension <c>dim</c></returns>
        private static ILRetArray< Int64 > diff(int dim, ILInArray< Int64 > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< Int64 >(A.Size);
                if (A.IsScalar) return empty< Int64 >(ILSize.Empty00);
                if (dim < 0)
                    throw new ILArgumentException("diff: leading dimension out of range!");
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< Int64 >(new ILSize(outDims)); 
                }
                ILSize inDim = A.Size; 
                int[] newDims = inDim.ToIntArray();
               
                if (inDim[dim] == 1) return empty< Int64 >(ILSize.Empty00);
                int newLength;
                Int64 [] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] --;
                newLength = newLength * newDims[dim];
                retArr = ILMemoryPool.Pool.New< Int64 >(newLength);
                ILSize newDimension = new ILSize(newDims); 
                int leadDimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / leadDimLen;
                int incOut = newDimension.SequentialIndexDistance(dim); 
                Int64 firstVal, secVal; 
                if (A.IsVector) 
                    return A["1:end"] - A[vec(0,A.Length-2)];     
                if (dim == 0) {
    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( Int64 * pOutArr = retArr)
                        fixed ( Int64 * pInArr = A.GetArrayForRead()) {
                            Int64 * lastElement;
                            Int64 * tmpOut = pOutArr;
                            Int64 * tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                
                                firstVal = *tmpIn++;
                                while (tmpIn < lastElement) {
                                    secVal = *tmpIn++;
                                    *(tmpOut++) = ( Int64 )(secVal-firstVal);
                                    firstVal = secVal; 
                                }
                            }
                        }
                    }
    #endregion
                } else {
    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( Int64 * pOutArr = retArr)
                        fixed ( Int64 * pInArr = A.GetArrayForRead()) {
                            Int64 * lastElementOut = newLength + pOutArr -1;
                            int inLength = inDim.NumberOfElements -1; 
                            Int64 * lastElementIn = pInArr + inLength; 
                            int inc = inDim.SequentialIndexDistance(dim); 
                            Int64 * tmpOut = pOutArr;
                            int outLength = newLength - 1;
                            Int64 * leadEnd; 
                            Int64 * tmpIn = pInArr;
                            for (int h = nrHigherDims; h--> 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                firstVal = *tmpIn; 
                                tmpIn += inc; 
                                while (tmpIn < leadEnd) {
                                    secVal = *tmpIn; 
                                    *tmpOut = ( Int64 )(secVal - firstVal);
                                    tmpIn += inc;
                                    tmpOut += incOut; 
                                    firstVal = secVal; 
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= inLength; 
                            }
                        }
                    }
    #endregion
                }
                return new ILRetArray< Int64 >(retArr, newDimension);
            }
        }
        /// <summary>
        /// Take n-th derivative
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <param name="N">[Optional] Degree of derivates. If not specified N=1 is assumed.</param>
        /// <returns>Array with first derivative of A along dimension <c>dim</c> or of first non singleton dimension respectively</returns>
        /// <remarks>N must be a number in range 1..L, where L is the length of A.Dimensions[dim]. 
        /// Otherwise an empty array will be returned.
        /// <para>If A is empty or scalar, or if N exceeds the length the specified dimension of A, 
        /// an empty array will be returned.</para></remarks>
        public static ILRetArray< Int32 > diff(ILInArray< Int32 > A, int N = 1, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null)) 
                    throw new ILArgumentException ("diff: input array A must not be null"); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< Int32 >(new ILSize(outDims)); 
                }
                if (A.IsScalar) return empty< Int32 >(ILSize.Empty00);
                if (A.IsEmpty) {
                    int [] retDim = A.Size.ToIntArray();
                    if (retDim[dim] > 0) 
                        retDim[dim]--; 
                    return empty<  Int32 >(new ILSize(retDim)); 
                }
                if (N == 0) 
                    return A.C; 
                if (N < 1 || N > A.Size[dim]) {
                    return empty<  Int32 >(ILSize.Empty00); 
                }
                ILArray< Int32 > ret = A.C; 
                for (int n = 0; n < N; n++) {
                    ret.a = diff(dim,ret);      
                }
                return ret;
            }
        }
        /// <summary>
        /// First derivative along specific dimension
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimensions to create derivative along</param>
        /// <returns>array with first derivative of A along dimension <c>dim</c></returns>
        private static ILRetArray< Int32 > diff(int dim, ILInArray< Int32 > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< Int32 >(A.Size);
                if (A.IsScalar) return empty< Int32 >(ILSize.Empty00);
                if (dim < 0)
                    throw new ILArgumentException("diff: leading dimension out of range!");
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< Int32 >(new ILSize(outDims)); 
                }
                ILSize inDim = A.Size; 
                int[] newDims = inDim.ToIntArray();
               
                if (inDim[dim] == 1) return empty< Int32 >(ILSize.Empty00);
                int newLength;
                Int32 [] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] --;
                newLength = newLength * newDims[dim];
                retArr = ILMemoryPool.Pool.New< Int32 >(newLength);
                ILSize newDimension = new ILSize(newDims); 
                int leadDimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / leadDimLen;
                int incOut = newDimension.SequentialIndexDistance(dim); 
                Int32 firstVal, secVal; 
                if (A.IsVector) 
                    return A["1:end"] - A[vec(0,A.Length-2)];     
                if (dim == 0) {
    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( Int32 * pOutArr = retArr)
                        fixed ( Int32 * pInArr = A.GetArrayForRead()) {
                            Int32 * lastElement;
                            Int32 * tmpOut = pOutArr;
                            Int32 * tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                
                                firstVal = *tmpIn++;
                                while (tmpIn < lastElement) {
                                    secVal = *tmpIn++;
                                    *(tmpOut++) = ( Int32 )(secVal-firstVal);
                                    firstVal = secVal; 
                                }
                            }
                        }
                    }
    #endregion
                } else {
    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( Int32 * pOutArr = retArr)
                        fixed ( Int32 * pInArr = A.GetArrayForRead()) {
                            Int32 * lastElementOut = newLength + pOutArr -1;
                            int inLength = inDim.NumberOfElements -1; 
                            Int32 * lastElementIn = pInArr + inLength; 
                            int inc = inDim.SequentialIndexDistance(dim); 
                            Int32 * tmpOut = pOutArr;
                            int outLength = newLength - 1;
                            Int32 * leadEnd; 
                            Int32 * tmpIn = pInArr;
                            for (int h = nrHigherDims; h--> 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                firstVal = *tmpIn; 
                                tmpIn += inc; 
                                while (tmpIn < leadEnd) {
                                    secVal = *tmpIn; 
                                    *tmpOut = ( Int32 )(secVal - firstVal);
                                    tmpIn += inc;
                                    tmpOut += incOut; 
                                    firstVal = secVal; 
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= inLength; 
                            }
                        }
                    }
    #endregion
                }
                return new ILRetArray< Int32 >(retArr, newDimension);
            }
        }
        /// <summary>
        /// Take n-th derivative
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <param name="N">[Optional] Degree of derivates. If not specified N=1 is assumed.</param>
        /// <returns>Array with first derivative of A along dimension <c>dim</c> or of first non singleton dimension respectively</returns>
        /// <remarks>N must be a number in range 1..L, where L is the length of A.Dimensions[dim]. 
        /// Otherwise an empty array will be returned.
        /// <para>If A is empty or scalar, or if N exceeds the length the specified dimension of A, 
        /// an empty array will be returned.</para></remarks>
        public static ILRetArray< byte > diff(ILInArray< byte > A, int N = 1, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null)) 
                    throw new ILArgumentException ("diff: input array A must not be null"); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< byte >(new ILSize(outDims)); 
                }
                if (A.IsScalar) return empty< byte >(ILSize.Empty00);
                if (A.IsEmpty) {
                    int [] retDim = A.Size.ToIntArray();
                    if (retDim[dim] > 0) 
                        retDim[dim]--; 
                    return empty<  byte >(new ILSize(retDim)); 
                }
                if (N == 0) 
                    return A.C; 
                if (N < 1 || N > A.Size[dim]) {
                    return empty<  byte >(ILSize.Empty00); 
                }
                ILArray< byte > ret = A.C; 
                for (int n = 0; n < N; n++) {
                    ret.a = diff(dim,ret);      
                }
                return ret;
            }
        }
        /// <summary>
        /// First derivative along specific dimension
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimensions to create derivative along</param>
        /// <returns>array with first derivative of A along dimension <c>dim</c></returns>
        private static ILRetArray< byte > diff(int dim, ILInArray< byte > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< byte >(A.Size);
                if (A.IsScalar) return empty< byte >(ILSize.Empty00);
                if (dim < 0)
                    throw new ILArgumentException("diff: leading dimension out of range!");
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< byte >(new ILSize(outDims)); 
                }
                ILSize inDim = A.Size; 
                int[] newDims = inDim.ToIntArray();
               
                if (inDim[dim] == 1) return empty< byte >(ILSize.Empty00);
                int newLength;
                byte [] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] --;
                newLength = newLength * newDims[dim];
                retArr = ILMemoryPool.Pool.New< byte >(newLength);
                ILSize newDimension = new ILSize(newDims); 
                int leadDimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / leadDimLen;
                int incOut = newDimension.SequentialIndexDistance(dim); 
                byte firstVal, secVal; 
                if (A.IsVector) 
                    return A["1:end"] - A[vec(0,A.Length-2)];     
                if (dim == 0) {
    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( byte * pOutArr = retArr)
                        fixed ( byte * pInArr = A.GetArrayForRead()) {
                            byte * lastElement;
                            byte * tmpOut = pOutArr;
                            byte * tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                
                                firstVal = *tmpIn++;
                                while (tmpIn < lastElement) {
                                    secVal = *tmpIn++;
                                    *(tmpOut++) = ( byte )(secVal-firstVal);
                                    firstVal = secVal; 
                                }
                            }
                        }
                    }
    #endregion
                } else {
    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( byte * pOutArr = retArr)
                        fixed ( byte * pInArr = A.GetArrayForRead()) {
                            byte * lastElementOut = newLength + pOutArr -1;
                            int inLength = inDim.NumberOfElements -1; 
                            byte * lastElementIn = pInArr + inLength; 
                            int inc = inDim.SequentialIndexDistance(dim); 
                            byte * tmpOut = pOutArr;
                            int outLength = newLength - 1;
                            byte * leadEnd; 
                            byte * tmpIn = pInArr;
                            for (int h = nrHigherDims; h--> 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                firstVal = *tmpIn; 
                                tmpIn += inc; 
                                while (tmpIn < leadEnd) {
                                    secVal = *tmpIn; 
                                    *tmpOut = ( byte )(secVal - firstVal);
                                    tmpIn += inc;
                                    tmpOut += incOut; 
                                    firstVal = secVal; 
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= inLength; 
                            }
                        }
                    }
    #endregion
                }
                return new ILRetArray< byte >(retArr, newDimension);
            }
        }
        /// <summary>
        /// Take n-th derivative
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <param name="N">[Optional] Degree of derivates. If not specified N=1 is assumed.</param>
        /// <returns>Array with first derivative of A along dimension <c>dim</c> or of first non singleton dimension respectively</returns>
        /// <remarks>N must be a number in range 1..L, where L is the length of A.Dimensions[dim]. 
        /// Otherwise an empty array will be returned.
        /// <para>If A is empty or scalar, or if N exceeds the length the specified dimension of A, 
        /// an empty array will be returned.</para></remarks>
        public static ILRetArray< fcomplex > diff(ILInArray< fcomplex > A, int N = 1, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null)) 
                    throw new ILArgumentException ("diff: input array A must not be null"); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< fcomplex >(new ILSize(outDims)); 
                }
                if (A.IsScalar) return empty< fcomplex >(ILSize.Empty00);
                if (A.IsEmpty) {
                    int [] retDim = A.Size.ToIntArray();
                    if (retDim[dim] > 0) 
                        retDim[dim]--; 
                    return empty<  fcomplex >(new ILSize(retDim)); 
                }
                if (N == 0) 
                    return A.C; 
                if (N < 1 || N > A.Size[dim]) {
                    return empty<  fcomplex >(ILSize.Empty00); 
                }
                ILArray< fcomplex > ret = A.C; 
                for (int n = 0; n < N; n++) {
                    ret.a = diff(dim,ret);      
                }
                return ret;
            }
        }
        /// <summary>
        /// First derivative along specific dimension
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimensions to create derivative along</param>
        /// <returns>array with first derivative of A along dimension <c>dim</c></returns>
        private static ILRetArray< fcomplex > diff(int dim, ILInArray< fcomplex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< fcomplex >(A.Size);
                if (A.IsScalar) return empty< fcomplex >(ILSize.Empty00);
                if (dim < 0)
                    throw new ILArgumentException("diff: leading dimension out of range!");
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< fcomplex >(new ILSize(outDims)); 
                }
                ILSize inDim = A.Size; 
                int[] newDims = inDim.ToIntArray();
               
                if (inDim[dim] == 1) return empty< fcomplex >(ILSize.Empty00);
                int newLength;
                fcomplex [] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] --;
                newLength = newLength * newDims[dim];
                retArr = ILMemoryPool.Pool.New< fcomplex >(newLength);
                ILSize newDimension = new ILSize(newDims); 
                int leadDimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / leadDimLen;
                int incOut = newDimension.SequentialIndexDistance(dim); 
                fcomplex firstVal, secVal; 
                if (A.IsVector) 
                    return A["1:end"] - A[vec(0,A.Length-2)];     
                if (dim == 0) {
    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( fcomplex * pOutArr = retArr)
                        fixed ( fcomplex * pInArr = A.GetArrayForRead()) {
                            fcomplex * lastElement;
                            fcomplex * tmpOut = pOutArr;
                            fcomplex * tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                
                                firstVal = *tmpIn++;
                                while (tmpIn < lastElement) {
                                    secVal = *tmpIn++;
                                    *(tmpOut++) = ( fcomplex )(secVal-firstVal);
                                    firstVal = secVal; 
                                }
                            }
                        }
                    }
    #endregion
                } else {
    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( fcomplex * pOutArr = retArr)
                        fixed ( fcomplex * pInArr = A.GetArrayForRead()) {
                            fcomplex * lastElementOut = newLength + pOutArr -1;
                            int inLength = inDim.NumberOfElements -1; 
                            fcomplex * lastElementIn = pInArr + inLength; 
                            int inc = inDim.SequentialIndexDistance(dim); 
                            fcomplex * tmpOut = pOutArr;
                            int outLength = newLength - 1;
                            fcomplex * leadEnd; 
                            fcomplex * tmpIn = pInArr;
                            for (int h = nrHigherDims; h--> 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                firstVal = *tmpIn; 
                                tmpIn += inc; 
                                while (tmpIn < leadEnd) {
                                    secVal = *tmpIn; 
                                    *tmpOut = ( fcomplex )(secVal - firstVal);
                                    tmpIn += inc;
                                    tmpOut += incOut; 
                                    firstVal = secVal; 
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= inLength; 
                            }
                        }
                    }
    #endregion
                }
                return new ILRetArray< fcomplex >(retArr, newDimension);
            }
        }
        /// <summary>
        /// Take n-th derivative
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <param name="N">[Optional] Degree of derivates. If not specified N=1 is assumed.</param>
        /// <returns>Array with first derivative of A along dimension <c>dim</c> or of first non singleton dimension respectively</returns>
        /// <remarks>N must be a number in range 1..L, where L is the length of A.Dimensions[dim]. 
        /// Otherwise an empty array will be returned.
        /// <para>If A is empty or scalar, or if N exceeds the length the specified dimension of A, 
        /// an empty array will be returned.</para></remarks>
        public static ILRetArray< float > diff(ILInArray< float > A, int N = 1, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null)) 
                    throw new ILArgumentException ("diff: input array A must not be null"); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< float >(new ILSize(outDims)); 
                }
                if (A.IsScalar) return empty< float >(ILSize.Empty00);
                if (A.IsEmpty) {
                    int [] retDim = A.Size.ToIntArray();
                    if (retDim[dim] > 0) 
                        retDim[dim]--; 
                    return empty<  float >(new ILSize(retDim)); 
                }
                if (N == 0) 
                    return A.C; 
                if (N < 1 || N > A.Size[dim]) {
                    return empty<  float >(ILSize.Empty00); 
                }
                ILArray< float > ret = A.C; 
                for (int n = 0; n < N; n++) {
                    ret.a = diff(dim,ret);      
                }
                return ret;
            }
        }
        /// <summary>
        /// First derivative along specific dimension
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimensions to create derivative along</param>
        /// <returns>array with first derivative of A along dimension <c>dim</c></returns>
        private static ILRetArray< float > diff(int dim, ILInArray< float > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< float >(A.Size);
                if (A.IsScalar) return empty< float >(ILSize.Empty00);
                if (dim < 0)
                    throw new ILArgumentException("diff: leading dimension out of range!");
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< float >(new ILSize(outDims)); 
                }
                ILSize inDim = A.Size; 
                int[] newDims = inDim.ToIntArray();
               
                if (inDim[dim] == 1) return empty< float >(ILSize.Empty00);
                int newLength;
                float [] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] --;
                newLength = newLength * newDims[dim];
                retArr = ILMemoryPool.Pool.New< float >(newLength);
                ILSize newDimension = new ILSize(newDims); 
                int leadDimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / leadDimLen;
                int incOut = newDimension.SequentialIndexDistance(dim); 
                float firstVal, secVal; 
                if (A.IsVector) 
                    return A["1:end"] - A[vec(0,A.Length-2)];     
                if (dim == 0) {
    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( float * pOutArr = retArr)
                        fixed ( float * pInArr = A.GetArrayForRead()) {
                            float * lastElement;
                            float * tmpOut = pOutArr;
                            float * tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                
                                firstVal = *tmpIn++;
                                while (tmpIn < lastElement) {
                                    secVal = *tmpIn++;
                                    *(tmpOut++) = ( float )(secVal-firstVal);
                                    firstVal = secVal; 
                                }
                            }
                        }
                    }
    #endregion
                } else {
    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( float * pOutArr = retArr)
                        fixed ( float * pInArr = A.GetArrayForRead()) {
                            float * lastElementOut = newLength + pOutArr -1;
                            int inLength = inDim.NumberOfElements -1; 
                            float * lastElementIn = pInArr + inLength; 
                            int inc = inDim.SequentialIndexDistance(dim); 
                            float * tmpOut = pOutArr;
                            int outLength = newLength - 1;
                            float * leadEnd; 
                            float * tmpIn = pInArr;
                            for (int h = nrHigherDims; h--> 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                firstVal = *tmpIn; 
                                tmpIn += inc; 
                                while (tmpIn < leadEnd) {
                                    secVal = *tmpIn; 
                                    *tmpOut = ( float )(secVal - firstVal);
                                    tmpIn += inc;
                                    tmpOut += incOut; 
                                    firstVal = secVal; 
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= inLength; 
                            }
                        }
                    }
    #endregion
                }
                return new ILRetArray< float >(retArr, newDimension);
            }
        }
        /// <summary>
        /// Take n-th derivative
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <param name="N">[Optional] Degree of derivates. If not specified N=1 is assumed.</param>
        /// <returns>Array with first derivative of A along dimension <c>dim</c> or of first non singleton dimension respectively</returns>
        /// <remarks>N must be a number in range 1..L, where L is the length of A.Dimensions[dim]. 
        /// Otherwise an empty array will be returned.
        /// <para>If A is empty or scalar, or if N exceeds the length the specified dimension of A, 
        /// an empty array will be returned.</para></remarks>
        public static ILRetArray< complex > diff(ILInArray< complex > A, int N = 1, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null)) 
                    throw new ILArgumentException ("diff: input array A must not be null"); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< complex >(new ILSize(outDims)); 
                }
                if (A.IsScalar) return empty< complex >(ILSize.Empty00);
                if (A.IsEmpty) {
                    int [] retDim = A.Size.ToIntArray();
                    if (retDim[dim] > 0) 
                        retDim[dim]--; 
                    return empty<  complex >(new ILSize(retDim)); 
                }
                if (N == 0) 
                    return A.C; 
                if (N < 1 || N > A.Size[dim]) {
                    return empty<  complex >(ILSize.Empty00); 
                }
                ILArray< complex > ret = A.C; 
                for (int n = 0; n < N; n++) {
                    ret.a = diff(dim,ret);      
                }
                return ret;
            }
        }
        /// <summary>
        /// First derivative along specific dimension
        /// </summary>
        /// <param name="A">input array</param>
        /// <param name="dim">dimensions to create derivative along</param>
        /// <returns>array with first derivative of A along dimension <c>dim</c></returns>
        private static ILRetArray< complex > diff(int dim, ILInArray< complex > A) {
            using (ILScope.Enter(A)) {
                if (A.IsEmpty) return empty< complex >(A.Size);
                if (A.IsScalar) return empty< complex >(ILSize.Empty00);
                if (dim < 0)
                    throw new ILArgumentException("diff: leading dimension out of range!");
                if (dim >= A.Size.NumberOfDimensions) {
                    int[] outDims = A.Size.ToIntArray(dim+1); 
                    outDims[dim] = 0; 
                    return empty< complex >(new ILSize(outDims)); 
                }
                ILSize inDim = A.Size; 
                int[] newDims = inDim.ToIntArray();
               
                if (inDim[dim] == 1) return empty< complex >(ILSize.Empty00);
                int newLength;
                complex [] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] --;
                newLength = newLength * newDims[dim];
                retArr = ILMemoryPool.Pool.New< complex >(newLength);
                ILSize newDimension = new ILSize(newDims); 
                int leadDimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / leadDimLen;
                int incOut = newDimension.SequentialIndexDistance(dim); 
                complex firstVal, secVal; 
                if (A.IsVector) 
                    return A["1:end"] - A[vec(0,A.Length-2)];     
                if (dim == 0) {
    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( complex * pOutArr = retArr)
                        fixed ( complex * pInArr = A.GetArrayForRead()) {
                            complex * lastElement;
                            complex * tmpOut = pOutArr;
                            complex * tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                
                                firstVal = *tmpIn++;
                                while (tmpIn < lastElement) {
                                    secVal = *tmpIn++;
                                    *(tmpOut++) = ( complex )(secVal-firstVal);
                                    firstVal = secVal; 
                                }
                            }
                        }
                    }
    #endregion
                } else {
    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( complex * pOutArr = retArr)
                        fixed ( complex * pInArr = A.GetArrayForRead()) {
                            complex * lastElementOut = newLength + pOutArr -1;
                            int inLength = inDim.NumberOfElements -1; 
                            complex * lastElementIn = pInArr + inLength; 
                            int inc = inDim.SequentialIndexDistance(dim); 
                            complex * tmpOut = pOutArr;
                            int outLength = newLength - 1;
                            complex * leadEnd; 
                            complex * tmpIn = pInArr;
                            for (int h = nrHigherDims; h--> 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                firstVal = *tmpIn; 
                                tmpIn += inc; 
                                while (tmpIn < leadEnd) {
                                    secVal = *tmpIn; 
                                    *tmpOut = ( complex )(secVal - firstVal);
                                    tmpIn += inc;
                                    tmpOut += incOut; 
                                    firstVal = secVal; 
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= inLength; 
                            }
                        }
                    }
    #endregion
                }
                return new ILRetArray< complex >(retArr, newDimension);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
  
    }
}
