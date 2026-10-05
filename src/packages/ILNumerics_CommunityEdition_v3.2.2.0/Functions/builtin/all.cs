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

        


#region HYCALPER AUTO GENERATED CODE

        /// <summary>Determine, if all elements are nonzero</summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array of same size as A, having specified or first non-singleton dimension reduced to 1, if all elements along that dimension are nonzero, '0' else. </para></returns>
        public static  ILRetLogical  all (ILInArray<double> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size);
                if (A.IsScalar) {
                    return new ILRetLogical (new byte [1]{(A.GetValue(0) == 0.0)?(byte)0:(byte)1},1,1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
                int tmpCount = 0; 

                int newLength;
               
                byte[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< byte>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( double* pInArr = A.GetArrayForRead()) {
                           
                            double* lastElement;
                           
                            byte* tmpOut = pOutArr;
                           
                            double* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                
                                while (tmpIn < lastElement) {
                                   
                                    double inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=   ( /*dummy*/ (inVal)   == 0.0)?0:1; 
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( double* pInArr = A.GetArrayForRead()) {
                           
                            byte* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                           
                            double* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                           
                            byte* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            double* leadEnd;
                           
                            double* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                
                                while (tmpIn < leadEnd) {
                                   
                                    double inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=  ( /*dummy*/ (inVal)   == 0.0)?0:1; //
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut += inc;
                                if (tmpOut > lastElementOut)
                                    tmpOut = pOutArr + ((tmpOut - pOutArr) - outLength);
                                if (tmpIn > lastElementIn)
                                    tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                            }
                        }
                    }
                    #endregion
                }
                return new  ILRetLogical(retArr, newDims);
            }
        }
        /// <summary>Determine, if all elements are nonzero</summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array of same size as A, having specified or first non-singleton dimension reduced to 1, if all elements along that dimension are nonzero, '0' else. </para></returns>
        public static  ILRetLogical  all (ILInArray<Int64> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size);
                if (A.IsScalar) {
                    return new ILRetLogical (new byte [1]{(A.GetValue(0) == 0)?(byte)0:(byte)1},1,1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
                int tmpCount = 0; 

                int newLength;
               
                byte[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< byte>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( Int64* pInArr = A.GetArrayForRead()) {
                           
                            Int64* lastElement;
                           
                            byte* tmpOut = pOutArr;
                           
                            Int64* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                
                                while (tmpIn < lastElement) {
                                   
                                    Int64 inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=   ( /*dummy*/ (inVal)   == 0)?0:1; 
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( Int64* pInArr = A.GetArrayForRead()) {
                           
                            byte* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                           
                            Int64* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                           
                            byte* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            Int64* leadEnd;
                           
                            Int64* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                
                                while (tmpIn < leadEnd) {
                                   
                                    Int64 inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=  ( /*dummy*/ (inVal)   == 0)?0:1; //
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut += inc;
                                if (tmpOut > lastElementOut)
                                    tmpOut = pOutArr + ((tmpOut - pOutArr) - outLength);
                                if (tmpIn > lastElementIn)
                                    tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                            }
                        }
                    }
                    #endregion
                }
                return new  ILRetLogical(retArr, newDims);
            }
        }
        /// <summary>Determine, if all elements are nonzero</summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array of same size as A, having specified or first non-singleton dimension reduced to 1, if all elements along that dimension are nonzero, '0' else. </para></returns>
        public static  ILRetLogical  all (ILInArray<Int32> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size);
                if (A.IsScalar) {
                    return new ILRetLogical (new byte [1]{(A.GetValue(0) == 0)?(byte)0:(byte)1},1,1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
                int tmpCount = 0; 

                int newLength;
               
                byte[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< byte>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( Int32* pInArr = A.GetArrayForRead()) {
                           
                            Int32* lastElement;
                           
                            byte* tmpOut = pOutArr;
                           
                            Int32* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                
                                while (tmpIn < lastElement) {
                                   
                                    Int32 inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=   ( /*dummy*/ (inVal)   == 0)?0:1; 
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( Int32* pInArr = A.GetArrayForRead()) {
                           
                            byte* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                           
                            Int32* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                           
                            byte* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            Int32* leadEnd;
                           
                            Int32* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                
                                while (tmpIn < leadEnd) {
                                   
                                    Int32 inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=  ( /*dummy*/ (inVal)   == 0)?0:1; //
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut += inc;
                                if (tmpOut > lastElementOut)
                                    tmpOut = pOutArr + ((tmpOut - pOutArr) - outLength);
                                if (tmpIn > lastElementIn)
                                    tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                            }
                        }
                    }
                    #endregion
                }
                return new  ILRetLogical(retArr, newDims);
            }
        }
        /// <summary>Determine, if all elements are nonzero</summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array of same size as A, having specified or first non-singleton dimension reduced to 1, if all elements along that dimension are nonzero, '0' else. </para></returns>
        public static  ILRetLogical  all (ILInArray<byte> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size);
                if (A.IsScalar) {
                    return new ILRetLogical (new byte [1]{(A.GetValue(0) == 0)?(byte)0:(byte)1},1,1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
                int tmpCount = 0; 

                int newLength;
               
                byte[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< byte>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( byte* pInArr = A.GetArrayForRead()) {
                           
                            byte* lastElement;
                           
                            byte* tmpOut = pOutArr;
                           
                            byte* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                
                                while (tmpIn < lastElement) {
                                   
                                    byte inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=   ( /*dummy*/ (inVal)   == 0)?0:1; 
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( byte* pInArr = A.GetArrayForRead()) {
                           
                            byte* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                           
                            byte* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                           
                            byte* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            byte* leadEnd;
                           
                            byte* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                
                                while (tmpIn < leadEnd) {
                                   
                                    byte inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=  ( /*dummy*/ (inVal)   == 0)?0:1; //
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut += inc;
                                if (tmpOut > lastElementOut)
                                    tmpOut = pOutArr + ((tmpOut - pOutArr) - outLength);
                                if (tmpIn > lastElementIn)
                                    tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                            }
                        }
                    }
                    #endregion
                }
                return new  ILRetLogical(retArr, newDims);
            }
        }
        /// <summary>Determine, if all elements are nonzero</summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array of same size as A, having specified or first non-singleton dimension reduced to 1, if all elements along that dimension are nonzero, '0' else. </para></returns>
        public static  ILRetLogical  all (ILInArray<fcomplex> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size);
                if (A.IsScalar) {
                    return new ILRetLogical (new byte [1]{(A.GetValue(0).iszero())?(byte)1:(byte)0},1,1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
                int tmpCount = 0; 

                int newLength;
               
                byte[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< byte>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( fcomplex* pInArr = A.GetArrayForRead()) {
                           
                            fcomplex* lastElement;
                           
                            byte* tmpOut = pOutArr;
                           
                            fcomplex* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                
                                while (tmpIn < lastElement) {
                                   
                                    fcomplex inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=   ( /*dummy*/ (inVal)  .iszero())?0:1;  
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( fcomplex* pInArr = A.GetArrayForRead()) {
                           
                            byte* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                           
                            fcomplex* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                           
                            byte* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            fcomplex* leadEnd;
                           
                            fcomplex* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                
                                while (tmpIn < leadEnd) {
                                   
                                    fcomplex inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=  ( /*dummy*/ (inVal)  .iszero())?0:1;  //
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut += inc;
                                if (tmpOut > lastElementOut)
                                    tmpOut = pOutArr + ((tmpOut - pOutArr) - outLength);
                                if (tmpIn > lastElementIn)
                                    tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                            }
                        }
                    }
                    #endregion
                }
                return new  ILRetLogical(retArr, newDims);
            }
        }
        /// <summary>Determine, if all elements are nonzero</summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array of same size as A, having specified or first non-singleton dimension reduced to 1, if any elements along that dimension are nonzero, '0' else. </para></returns>
        public static  ILRetLogical  all (ILInArray<float> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size);
                if (A.IsScalar) {
                    return new ILRetLogical (new byte [1]{(A.GetValue(0) == 0.0f)?(byte)0:(byte)1},1,1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
                int tmpCount = 0; 

                int newLength;
               
                byte[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< byte>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( float* pInArr = A.GetArrayForRead()) {
                           
                            float* lastElement;
                           
                            byte* tmpOut = pOutArr;
                           
                            float* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                
                                while (tmpIn < lastElement) {
                                   
                                    float inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=   ( /*dummy*/ (inVal)   == 0.0f)?0:1; 
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( float* pInArr = A.GetArrayForRead()) {
                           
                            byte* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                           
                            float* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                           
                            byte* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            float* leadEnd;
                           
                            float* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                
                                while (tmpIn < leadEnd) {
                                   
                                    float inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=  ( /*dummy*/ (inVal)   == 0.0f)?0:1; //
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut += inc;
                                if (tmpOut > lastElementOut)
                                    tmpOut = pOutArr + ((tmpOut - pOutArr) - outLength);
                                if (tmpIn > lastElementIn)
                                    tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                            }
                        }
                    }
                    #endregion
                }
                return new  ILRetLogical(retArr, newDims);
            }
        }
        /// <summary>Determine, if all elements are nonzero</summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns><para>Array of same size as A, having specified or first non-singleton dimension reduced to 1, if any elements along that dimension are nonzero, '0' else. </para></returns>
        public static  ILRetLogical  all (ILInArray<complex> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetLogical(A.Size);
                if (A.IsScalar) {
                    return new ILRetLogical (new byte [1]{(A.GetValue(0).iszero())?(byte)0:(byte)1},1,1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
                int tmpCount = 0; 

                int newLength;
               
                byte[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< byte>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( complex* pInArr = A.GetArrayForRead()) {
                           
                            complex* lastElement;
                           
                            byte* tmpOut = pOutArr;
                           
                            complex* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                
                                while (tmpIn < lastElement) {
                                   
                                    complex inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=   ( /*dummy*/ (inVal)  .iszero())?0:1;  
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( byte* pOutArr = retArr)
                        fixed ( complex* pInArr = A.GetArrayForRead()) {
                           
                            byte* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                           
                            complex* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                           
                            byte* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            complex* leadEnd;
                           
                            complex* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                
                                while (tmpIn < leadEnd) {
                                   
                                    complex inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    tmpCount  +=  ( /*dummy*/ (inVal)  .iszero())?0:1;  //
                                }
                                *tmpOut = (tmpCount == dimLen)? (byte)1:(byte)0; tmpCount = 0; 
                                tmpOut += inc;
                                if (tmpOut > lastElementOut)
                                    tmpOut = pOutArr + ((tmpOut - pOutArr) - outLength);
                                if (tmpIn > lastElementIn)
                                    tmpIn = pInArr + ((tmpIn - pInArr) - inLength);
                            }
                        }
                    }
                    #endregion
                }
                return new  ILRetLogical(retArr, newDims);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}