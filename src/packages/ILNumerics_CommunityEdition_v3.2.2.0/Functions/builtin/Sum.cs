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
        /// Sum elements of A along dimension specified.
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the specified or first non singleton dimension reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<double>  sum (ILInArray<double> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetArray<double>(A.Size);
                if (A.IsScalar) {
                    
                    return new  ILRetArray<double>(new double[] { A.GetValue(0) }, 1, 1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
                
                if (inDim[dim] == 1) return A.C;

                int newLength;
                
                double[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< double>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( double* pOutArr = retArr)
                        fixed ( double* pInArr = A.GetArrayForRead()) {
                            
                            double* lastElement;
                            
                            double* tmpOut = pOutArr;
                            
                            double* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                
                                *tmpOut = 0.0;
                                while (tmpIn < lastElement) {
                                    
                                    double inVal = *(tmpIn++);
                                    
                                    /*dummy*/
                                    
                                    *tmpOut  +=   (double) /*dummy*/ (inVal)  ; 
                                }
                                
                                /*dummy*/
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( double* pOutArr = retArr)
                        fixed ( double* pInArr = A.GetArrayForRead()) {
                            
                            double* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                            
                            double* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                            
                            double* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                            
                            double* leadEnd;
                            
                            double* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                
                                *tmpOut = 0.0;
                                while (tmpIn < leadEnd) {
                                    
                                    double inVal = *(tmpIn);
                                    tmpIn += inc;
                                    
                                    /*dummy*/
                                    
                                    *tmpOut  +=  (double) /*dummy*/ (inVal)  ; //
                                }
                                
                                /*dummy*/
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
                return new  ILRetArray<double>(retArr, newDims);
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Sum elements of A along dimension specified.
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the specified or first non singleton dimension reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<Int64>  sum (ILInArray<Int64> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetArray<Int64>(A.Size);
                if (A.IsScalar) {
                   
                    return new  ILRetArray<Int64>(new Int64[] { A.GetValue(0) }, 1, 1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
               
                if (inDim[dim] == 1) return A.C;

                int newLength;
               
                Int64[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< Int64>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( Int64* pOutArr = retArr)
                        fixed ( Int64* pInArr = A.GetArrayForRead()) {
                           
                            Int64* lastElement;
                           
                            Int64* tmpOut = pOutArr;
                           
                            Int64* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                *tmpOut = 0;
                                while (tmpIn < lastElement) {
                                   
                                    Int64 inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    *tmpOut  +=   (Int64) /*dummy*/ (inVal)  ; 
                                }
                               
                                /*dummy*/
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( Int64* pOutArr = retArr)
                        fixed ( Int64* pInArr = A.GetArrayForRead()) {
                           
                            Int64* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                           
                            Int64* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                           
                            Int64* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            Int64* leadEnd;
                           
                            Int64* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                *tmpOut = 0;
                                while (tmpIn < leadEnd) {
                                   
                                    Int64 inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    *tmpOut  +=  (Int64) /*dummy*/ (inVal)  ; //
                                }
                               
                                /*dummy*/
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
                return new  ILRetArray<Int64>(retArr, newDims);
            }
        }
        /// <summary>
        /// Sum elements of A along dimension specified.
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the specified or first non singleton dimension reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<Int32>  sum (ILInArray<Int32> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetArray<Int32>(A.Size);
                if (A.IsScalar) {
                   
                    return new  ILRetArray<Int32>(new Int32[] { A.GetValue(0) }, 1, 1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
               
                if (inDim[dim] == 1) return A.C;

                int newLength;
               
                Int32[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< Int32>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( Int32* pOutArr = retArr)
                        fixed ( Int32* pInArr = A.GetArrayForRead()) {
                           
                            Int32* lastElement;
                           
                            Int32* tmpOut = pOutArr;
                           
                            Int32* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                *tmpOut = 0;
                                while (tmpIn < lastElement) {
                                   
                                    Int32 inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    *tmpOut  +=   (Int32) /*dummy*/ (inVal)  ; 
                                }
                               
                                /*dummy*/
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( Int32* pOutArr = retArr)
                        fixed ( Int32* pInArr = A.GetArrayForRead()) {
                           
                            Int32* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                           
                            Int32* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                           
                            Int32* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            Int32* leadEnd;
                           
                            Int32* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                *tmpOut = 0;
                                while (tmpIn < leadEnd) {
                                   
                                    Int32 inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    *tmpOut  +=  (Int32) /*dummy*/ (inVal)  ; //
                                }
                               
                                /*dummy*/
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
                return new  ILRetArray<Int32>(retArr, newDims);
            }
        }
        /// <summary>
        /// Sum elements of A along dimension specified.
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the specified or first non singleton dimension reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<byte>  sum (ILInArray<byte> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetArray<byte>(A.Size);
                if (A.IsScalar) {
                   
                    return new  ILRetArray<byte>(new byte[] { A.GetValue(0) }, 1, 1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
               
                if (inDim[dim] == 1) return A.C;

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
                                *tmpOut = 0;
                                while (tmpIn < lastElement) {
                                   
                                    byte inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    *tmpOut  +=   (byte) /*dummy*/ (inVal)  ; 
                                }
                               
                                /*dummy*/
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
                                *tmpOut = 0;
                                while (tmpIn < leadEnd) {
                                   
                                    byte inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    *tmpOut  +=  (byte) /*dummy*/ (inVal)  ; //
                                }
                               
                                /*dummy*/
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
                return new  ILRetArray<byte>(retArr, newDims);
            }
        }
        /// <summary>
        /// Sum elements of A along dimension specified.
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the specified or first non singleton dimension reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<fcomplex>  sum (ILInArray<fcomplex> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetArray<fcomplex>(A.Size);
                if (A.IsScalar) {
                   
                    return new  ILRetArray<fcomplex>(new fcomplex[] { A.GetValue(0) }, 1, 1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
               
                if (inDim[dim] == 1) return A.C;

                int newLength;
               
                fcomplex[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< fcomplex>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( fcomplex* pOutArr = retArr)
                        fixed ( fcomplex* pInArr = A.GetArrayForRead()) {
                           
                            fcomplex* lastElement;
                           
                            fcomplex* tmpOut = pOutArr;
                           
                            fcomplex* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                *tmpOut = 0.0f;
                                while (tmpIn < lastElement) {
                                   
                                    fcomplex inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    *tmpOut  +=   (fcomplex) /*dummy*/ (inVal)  ; 
                                }
                               
                                /*dummy*/
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( fcomplex* pOutArr = retArr)
                        fixed ( fcomplex* pInArr = A.GetArrayForRead()) {
                           
                            fcomplex* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                           
                            fcomplex* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                           
                            fcomplex* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            fcomplex* leadEnd;
                           
                            fcomplex* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                *tmpOut = 0.0f;
                                while (tmpIn < leadEnd) {
                                   
                                    fcomplex inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    *tmpOut  +=  (fcomplex) /*dummy*/ (inVal)  ; //
                                }
                               
                                /*dummy*/
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
                return new  ILRetArray<fcomplex>(retArr, newDims);
            }
        }
        /// <summary>
        /// Sum elements of A along dimension specified.
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the specified or first non singleton dimension reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<float>  sum (ILInArray<float> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetArray<float>(A.Size);
                if (A.IsScalar) {
                   
                    return new  ILRetArray<float>(new float[] { A.GetValue(0) }, 1, 1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
               
                if (inDim[dim] == 1) return A.C;

                int newLength;
               
                float[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< float>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( float* pOutArr = retArr)
                        fixed ( float* pInArr = A.GetArrayForRead()) {
                           
                            float* lastElement;
                           
                            float* tmpOut = pOutArr;
                           
                            float* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                *tmpOut = 0.0f;
                                while (tmpIn < lastElement) {
                                   
                                    float inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    *tmpOut  +=   (float) /*dummy*/ (inVal)  ; 
                                }
                               
                                /*dummy*/
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( float* pOutArr = retArr)
                        fixed ( float* pInArr = A.GetArrayForRead()) {
                           
                            float* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                           
                            float* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                           
                            float* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            float* leadEnd;
                           
                            float* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                *tmpOut = 0.0f;
                                while (tmpIn < leadEnd) {
                                   
                                    float inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    *tmpOut  +=  (float) /*dummy*/ (inVal)  ; //
                                }
                               
                                /*dummy*/
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
                return new  ILRetArray<float>(retArr, newDims);
            }
        }
        /// <summary>
        /// Sum elements of A along dimension specified.
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Array, same size as A, but having the specified or first non singleton dimension reduced to the length 1 with the sum of all
        /// elements along that dimension.</returns>
        public static  ILRetArray<complex>  sum (ILInArray<complex> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim >= A.Size.NumberOfDimensions)
                    throw new ILArgumentException("dimension parameter out of range!");
                if (A.IsEmpty)
                    return new  ILRetArray<complex>(A.Size);
                if (A.IsScalar) {
                   
                    return new  ILRetArray<complex>(new complex[] { A.GetValue(0) }, 1, 1);
                }
                ILSize inDim = A.Size;
                int[] newDims = inDim.ToIntArray();
               
                if (inDim[dim] == 1) return A.C;

                int newLength;
               
                complex[] retArr;
                // build ILSize
                newLength = inDim.NumberOfElements / newDims[dim];
                newDims[dim] = 1;
                retArr = ILMemoryPool.Pool.New< complex>(newLength);
                ILSize newDimension = new ILSize(newDims);
                int incOut = newDimension.SequentialIndexDistance(dim);
                int dimLen = inDim[dim];
                int nrHigherDims = inDim.NumberOfElements / dimLen;
                if (dim == 0) {
                    #region physical along 1st leading dimension
                    unsafe {
                        fixed ( complex* pOutArr = retArr)
                        fixed ( complex* pInArr = A.GetArrayForRead()) {
                           
                            complex* lastElement;
                           
                            complex* tmpOut = pOutArr;
                           
                            complex* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                lastElement = tmpIn + dimLen;
                                *tmpOut = 0.0;
                                while (tmpIn < lastElement) {
                                   
                                    complex inVal = *(tmpIn++);
                                   
                                    /*dummy*/
                                   
                                    *tmpOut  +=   (complex) /*dummy*/ (inVal)  ; 
                                }
                               
                                /*dummy*/
                                tmpOut++;
                            }
                        }
                    }
                    #endregion
                } else {
                    #region physical along abitrary dimension
                    // sum along abitrary dimension 
                    unsafe {
                        fixed ( complex* pOutArr = retArr)
                        fixed ( complex* pInArr = A.GetArrayForRead()) {
                           
                            complex* lastElementOut = newLength + pOutArr - 1;
                            int inLength = inDim.NumberOfElements - 1;
                           
                            complex* lastElementIn = pInArr + inLength;
                            int inc = inDim.SequentialIndexDistance(dim);
                           
                            complex* tmpOut = pOutArr;
                            int outLength = newLength - 1;
                           
                            complex* leadEnd;
                           
                            complex* tmpIn = pInArr;
                            for (int h = nrHigherDims; h-- > 0; ) {
                                leadEnd = tmpIn + dimLen * inc;
                                *tmpOut = 0.0;
                                while (tmpIn < leadEnd) {
                                   
                                    complex inVal = *(tmpIn);
                                    tmpIn += inc;
                                   
                                    /*dummy*/
                                   
                                    *tmpOut  +=  (complex) /*dummy*/ (inVal)  ; //
                                }
                               
                                /*dummy*/
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
                return new  ILRetArray<complex>(retArr, newDims);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}