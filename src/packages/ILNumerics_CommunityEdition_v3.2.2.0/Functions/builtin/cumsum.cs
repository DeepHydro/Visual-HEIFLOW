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
        /// Cumulative sum along elements
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If ommited cumsum operates along the first non singleton dimension (i.e. length > 1).</param>
        /// <returns>Array of the same size as A with cumulative sums of elements in A</returns>
        public static ILRetArray< double > cumsum(ILInArray< double > A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null))
                    return empty<double>(); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions || A.S[dim] == 1 || A.IsEmpty) {
                    return A.C; 
                }
                int outLength = A.S.NumberOfElements;
                
                double[] retArr = ILMemoryPool.Pool.New< double>(outLength);
                int leadDimLen = A.S[dim];
                int nrHigherDims = A.S.NumberOfElements / leadDimLen;
                int inc = A.S.SequentialIndexDistance(dim);
                
                double cumsum;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( double* pOutArr = retArr)
                        fixed ( double* pInArr = A.GetArrayForRead()) {
                            
                            double* lastElement;
                            
                            double* tmpOut = pOutArr;
                            
                            double* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                cumsum = 0;
                                while (tmpIn < lastElement) {
                                    cumsum += *tmpIn++;
                                    *tmpOut++ = ( double)(cumsum);
                                }
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    outLength--; 
                    unsafe {
                        fixed ( double* pOutArr = retArr)
                        fixed ( double* pInArr = A.GetArrayForRead()) {
                            
                            double* lastElementOut = pOutArr + outLength;
                            
                            double* lastElementIn = pInArr + outLength;
                            
                            double* tmpOut = pOutArr;
                            
                            double* leadEnd;
                            
                            double* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                cumsum = 0;
                                while (tmpIn < leadEnd) {
                                    cumsum += *tmpIn;
                                    *tmpOut = ( double)(cumsum);
                                    tmpIn += inc;
                                    tmpOut += inc;
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= outLength;
                            }
                        }
                    }
                    #endregion
                }
                return new ILRetArray< double>(retArr, A.S);
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Cumulative sum along elements
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If ommited cumsum operates along the first non singleton dimension (i.e. length > 1).</param>
        /// <returns>Array of the same size as A with cumulative sums of elements in A</returns>
        public static ILRetArray< Int64 > cumsum(ILInArray< Int64 > A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null))
                    return empty<Int64>(); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions || A.S[dim] == 1 || A.IsEmpty) {
                    return A.C; 
                }
                int outLength = A.S.NumberOfElements;
               
                Int64[] retArr = ILMemoryPool.Pool.New< Int64>(outLength);
                int leadDimLen = A.S[dim];
                int nrHigherDims = A.S.NumberOfElements / leadDimLen;
                int inc = A.S.SequentialIndexDistance(dim);
               
                Int64 cumsum;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( Int64* pOutArr = retArr)
                        fixed ( Int64* pInArr = A.GetArrayForRead()) {
                           
                            Int64* lastElement;
                           
                            Int64* tmpOut = pOutArr;
                           
                            Int64* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                cumsum = 0;
                                while (tmpIn < lastElement) {
                                    cumsum += *tmpIn++;
                                    *tmpOut++ = ( Int64)(cumsum);
                                }
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    outLength--; 
                    unsafe {
                        fixed ( Int64* pOutArr = retArr)
                        fixed ( Int64* pInArr = A.GetArrayForRead()) {
                           
                            Int64* lastElementOut = pOutArr + outLength;
                           
                            Int64* lastElementIn = pInArr + outLength;
                           
                            Int64* tmpOut = pOutArr;
                           
                            Int64* leadEnd;
                           
                            Int64* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                cumsum = 0;
                                while (tmpIn < leadEnd) {
                                    cumsum += *tmpIn;
                                    *tmpOut = ( Int64)(cumsum);
                                    tmpIn += inc;
                                    tmpOut += inc;
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= outLength;
                            }
                        }
                    }
                    #endregion
                }
                return new ILRetArray< Int64>(retArr, A.S);
            }
        }
        /// <summary>
        /// Cumulative sum along elements
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If ommited cumsum operates along the first non singleton dimension (i.e. length > 1).</param>
        /// <returns>Array of the same size as A with cumulative sums of elements in A</returns>
        public static ILRetArray< Int32 > cumsum(ILInArray< Int32 > A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null))
                    return empty<Int32>(); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions || A.S[dim] == 1 || A.IsEmpty) {
                    return A.C; 
                }
                int outLength = A.S.NumberOfElements;
               
                Int32[] retArr = ILMemoryPool.Pool.New< Int32>(outLength);
                int leadDimLen = A.S[dim];
                int nrHigherDims = A.S.NumberOfElements / leadDimLen;
                int inc = A.S.SequentialIndexDistance(dim);
               
                Int32 cumsum;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( Int32* pOutArr = retArr)
                        fixed ( Int32* pInArr = A.GetArrayForRead()) {
                           
                            Int32* lastElement;
                           
                            Int32* tmpOut = pOutArr;
                           
                            Int32* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                cumsum = 0;
                                while (tmpIn < lastElement) {
                                    cumsum += *tmpIn++;
                                    *tmpOut++ = ( Int32)(cumsum);
                                }
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    outLength--; 
                    unsafe {
                        fixed ( Int32* pOutArr = retArr)
                        fixed ( Int32* pInArr = A.GetArrayForRead()) {
                           
                            Int32* lastElementOut = pOutArr + outLength;
                           
                            Int32* lastElementIn = pInArr + outLength;
                           
                            Int32* tmpOut = pOutArr;
                           
                            Int32* leadEnd;
                           
                            Int32* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                cumsum = 0;
                                while (tmpIn < leadEnd) {
                                    cumsum += *tmpIn;
                                    *tmpOut = ( Int32)(cumsum);
                                    tmpIn += inc;
                                    tmpOut += inc;
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= outLength;
                            }
                        }
                    }
                    #endregion
                }
                return new ILRetArray< Int32>(retArr, A.S);
            }
        }
        /// <summary>
        /// Cumulative sum along elements
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If ommited cumsum operates along the first non singleton dimension (i.e. length > 1).</param>
        /// <returns>Array of the same size as A with cumulative sums of elements in A</returns>
        public static ILRetArray< byte > cumsum(ILInArray< byte > A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null))
                    return empty<byte>(); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions || A.S[dim] == 1 || A.IsEmpty) {
                    return A.C; 
                }
                int outLength = A.S.NumberOfElements;
               
                byte[] retArr = ILMemoryPool.Pool.New< byte>(outLength);
                int leadDimLen = A.S[dim];
                int nrHigherDims = A.S.NumberOfElements / leadDimLen;
                int inc = A.S.SequentialIndexDistance(dim);
               
                byte cumsum;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( byte* pInArr = A.GetArrayForRead()) {
                           
                            byte* lastElement;
                           
                            byte* tmpOut = pOutArr;
                           
                            byte* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                cumsum = 0;
                                while (tmpIn < lastElement) {
                                    cumsum += *tmpIn++;
                                    *tmpOut++ = ( byte)(cumsum);
                                }
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    outLength--; 
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( byte* pInArr = A.GetArrayForRead()) {
                           
                            byte* lastElementOut = pOutArr + outLength;
                           
                            byte* lastElementIn = pInArr + outLength;
                           
                            byte* tmpOut = pOutArr;
                           
                            byte* leadEnd;
                           
                            byte* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                cumsum = 0;
                                while (tmpIn < leadEnd) {
                                    cumsum += *tmpIn;
                                    *tmpOut = ( byte)(cumsum);
                                    tmpIn += inc;
                                    tmpOut += inc;
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= outLength;
                            }
                        }
                    }
                    #endregion
                }
                return new ILRetArray< byte>(retArr, A.S);
            }
        }
        /// <summary>
        /// Cumulative sum along elements
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If ommited cumsum operates along the first non singleton dimension (i.e. length > 1).</param>
        /// <returns>Array of the same size as A with cumulative sums of elements in A</returns>
        public static ILRetArray< fcomplex > cumsum(ILInArray< fcomplex > A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null))
                    return empty<fcomplex>(); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions || A.S[dim] == 1 || A.IsEmpty) {
                    return A.C; 
                }
                int outLength = A.S.NumberOfElements;
               
                fcomplex[] retArr = ILMemoryPool.Pool.New< fcomplex>(outLength);
                int leadDimLen = A.S[dim];
                int nrHigherDims = A.S.NumberOfElements / leadDimLen;
                int inc = A.S.SequentialIndexDistance(dim);
               
                fcomplex cumsum;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( fcomplex* pOutArr = retArr)
                        fixed ( fcomplex* pInArr = A.GetArrayForRead()) {
                           
                            fcomplex* lastElement;
                           
                            fcomplex* tmpOut = pOutArr;
                           
                            fcomplex* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                cumsum = 0;
                                while (tmpIn < lastElement) {
                                    cumsum += *tmpIn++;
                                    *tmpOut++ = ( fcomplex)(cumsum);
                                }
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    outLength--; 
                    unsafe {
                        fixed ( fcomplex* pOutArr = retArr)
                        fixed ( fcomplex* pInArr = A.GetArrayForRead()) {
                           
                            fcomplex* lastElementOut = pOutArr + outLength;
                           
                            fcomplex* lastElementIn = pInArr + outLength;
                           
                            fcomplex* tmpOut = pOutArr;
                           
                            fcomplex* leadEnd;
                           
                            fcomplex* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                cumsum = 0;
                                while (tmpIn < leadEnd) {
                                    cumsum += *tmpIn;
                                    *tmpOut = ( fcomplex)(cumsum);
                                    tmpIn += inc;
                                    tmpOut += inc;
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= outLength;
                            }
                        }
                    }
                    #endregion
                }
                return new ILRetArray< fcomplex>(retArr, A.S);
            }
        }
        /// <summary>
        /// Cumulative sum along elements
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If ommited cumsum operates along the first non singleton dimension (i.e. length > 1).</param>
        /// <returns>Array of the same size as A with cumulative sums of elements in A</returns>
        public static ILRetArray< float > cumsum(ILInArray< float > A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null))
                    return empty<float>(); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions || A.S[dim] == 1 || A.IsEmpty) {
                    return A.C; 
                }
                int outLength = A.S.NumberOfElements;
               
                float[] retArr = ILMemoryPool.Pool.New< float>(outLength);
                int leadDimLen = A.S[dim];
                int nrHigherDims = A.S.NumberOfElements / leadDimLen;
                int inc = A.S.SequentialIndexDistance(dim);
               
                float cumsum;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( float* pOutArr = retArr)
                        fixed ( float* pInArr = A.GetArrayForRead()) {
                           
                            float* lastElement;
                           
                            float* tmpOut = pOutArr;
                           
                            float* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                cumsum = 0;
                                while (tmpIn < lastElement) {
                                    cumsum += *tmpIn++;
                                    *tmpOut++ = ( float)(cumsum);
                                }
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    outLength--; 
                    unsafe {
                        fixed ( float* pOutArr = retArr)
                        fixed ( float* pInArr = A.GetArrayForRead()) {
                           
                            float* lastElementOut = pOutArr + outLength;
                           
                            float* lastElementIn = pInArr + outLength;
                           
                            float* tmpOut = pOutArr;
                           
                            float* leadEnd;
                           
                            float* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                cumsum = 0;
                                while (tmpIn < leadEnd) {
                                    cumsum += *tmpIn;
                                    *tmpOut = ( float)(cumsum);
                                    tmpIn += inc;
                                    tmpOut += inc;
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= outLength;
                            }
                        }
                    }
                    #endregion
                }
                return new ILRetArray< float>(retArr, A.S);
            }
        }
        /// <summary>
        /// Cumulative sum along elements
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If ommited cumsum operates along the first non singleton dimension (i.e. length > 1).</param>
        /// <returns>Array of the same size as A with cumulative sums of elements in A</returns>
        public static ILRetArray< complex > cumsum(ILInArray< complex > A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A,null))
                    return empty<complex>(); 
                if (dim < 0) {
                    dim = A.Size.WorkingDimension();
                }
                if (dim >= A.Size.NumberOfDimensions || A.S[dim] == 1 || A.IsEmpty) {
                    return A.C; 
                }
                int outLength = A.S.NumberOfElements;
               
                complex[] retArr = ILMemoryPool.Pool.New< complex>(outLength);
                int leadDimLen = A.S[dim];
                int nrHigherDims = A.S.NumberOfElements / leadDimLen;
                int inc = A.S.SequentialIndexDistance(dim);
               
                complex cumsum;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( complex* pOutArr = retArr)
                        fixed ( complex* pInArr = A.GetArrayForRead()) {
                           
                            complex* lastElement;
                           
                            complex* tmpOut = pOutArr;
                           
                            complex* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + leadDimLen;
                                cumsum = 0;
                                while (tmpIn < lastElement) {
                                    cumsum += *tmpIn++;
                                    *tmpOut++ = ( complex)(cumsum);
                                }
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    outLength--; 
                    unsafe {
                        fixed ( complex* pOutArr = retArr)
                        fixed ( complex* pInArr = A.GetArrayForRead()) {
                           
                            complex* lastElementOut = pOutArr + outLength;
                           
                            complex* lastElementIn = pInArr + outLength;
                           
                            complex* tmpOut = pOutArr;
                           
                            complex* leadEnd;
                           
                            complex* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + leadDimLen * inc;
                                cumsum = 0;
                                while (tmpIn < leadEnd) {
                                    cumsum += *tmpIn;
                                    *tmpOut = ( complex)(cumsum);
                                    tmpIn += inc;
                                    tmpOut += inc;
                                }
                                if (tmpOut > lastElementOut)
                                    tmpOut -= outLength;
                                if (tmpIn > lastElementIn)
                                    tmpIn -= outLength;
                            }
                        }
                    }
                    #endregion
                }
                return new ILRetArray< complex>(retArr, A.S);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
  
    }
}
