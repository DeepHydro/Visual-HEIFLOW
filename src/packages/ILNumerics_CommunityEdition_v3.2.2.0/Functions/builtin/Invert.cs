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

/////   this is a template for unary - inplace functions 



namespace ILNumerics {

    public partial class ILMath {         
         

        /// <summary>
        /// Invert elements of A, return result into predefined output array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="outArray">Predefined array for results</param>
        public static void  invert(ILInArray<double> A, ILOutArray<double> outArray) {
            using (ILScope.Enter(A)) {
                ILSize inDim = A.Size;
                if (object.Equals(outArray,null))
                    throw new ILArgumentException("output parameter 'outArray' must not be null but reference existing array");

                bool inplace = true; 
                int outLen = A.S.NumberOfElements; 
                ILSize outDims = A.S; 
                 double[] retArr = outArray.GetArrayForWrite(); 
                 double[] aArr = A.GetArrayForRead();
                if (outArray.S.NumberOfElements != outLen 
                    && !A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILNumerics.ILMemoryPool.Pool.New<double>(outLen); 
                    inplace = false; 
                }
                unsafe {
                    fixed ( double* inA2 =retArr)
                    fixed ( double* inA1 = aArr) {
                        
                        double* pInA1 = inA1;
                        
                        double* pInA2 = inA2;
                        
                        double* outEnd = inA2 + outLen;
                        if (!inplace) {
                            while (pInA2 < outEnd) {  //HC11
                                
                                *pInA2++ =  /*dummy*/ (*pInA1++ /*HC:HCoperation*/ * (-1.0)/*dummy*/);
                            }
                        } else {
                            while (pInA2 < outEnd) {  //HC11
                                *pInA2 =  /*dummy*/ (*pInA2 /*HC:HCoperation*/ * (-1.0)/*dummy*/);
                                pInA2++; 
                            }
                        }
                    }
                }
                outArray.a = new ILRetArray< double>(new ILDenseStorage< double>(retArr, outDims)); 
                return;
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Invert elements of A, return result into predefined output array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="outArray">Predefined array for results</param>
        public static void  invert(ILInArray<byte> A, ILOutArray<byte> outArray) {
            using (ILScope.Enter(A)) {
                ILSize inDim = A.Size;
                if (object.Equals(outArray,null))
                    throw new ILArgumentException("output parameter 'outArray' must not be null but reference existing array");

                bool inplace = true; 
                int outLen = A.S.NumberOfElements; 
                ILSize outDims = A.S; 
                byte[] retArr = outArray.GetArrayForWrite(); 
                byte[] aArr = A.GetArrayForRead();
                if (outArray.S.NumberOfElements != outLen 
                    && !A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILNumerics.ILMemoryPool.Pool.New<byte>(outLen); 
                    inplace = false; 
                }
                unsafe {
                    fixed ( byte* inA2 =retArr)
                    fixed ( byte* inA1 = aArr) {
                       
                        byte* pInA1 = inA1;
                       
                        byte* pInA2 = inA2;
                       
                        byte* outEnd = inA2 + outLen;
                        if (!inplace) {
                            while (pInA2 < outEnd) {  //HC11
                               
                                *pInA2++ =  (byte) (*pInA1++ /*HC:*/ *(-1)/*dummy*/);
                            }
                        } else {
                            while (pInA2 < outEnd) {  //HC11
                                *pInA2 =  (byte) (*pInA2 /*HC:*/ *(-1)/*dummy*/);
                                pInA2++; 
                            }
                        }
                    }
                }
                outArray.a = new ILRetArray< byte>(new ILDenseStorage< byte>(retArr, outDims)); 
                return;
            }
        }
        /// <summary>
        /// Invert elements of A, return result into predefined output array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="outArray">Predefined array for results</param>
        public static void  invert(ILInArray<Int64> A, ILOutArray<Int64> outArray) {
            using (ILScope.Enter(A)) {
                ILSize inDim = A.Size;
                if (object.Equals(outArray,null))
                    throw new ILArgumentException("output parameter 'outArray' must not be null but reference existing array");

                bool inplace = true; 
                int outLen = A.S.NumberOfElements; 
                ILSize outDims = A.S; 
                Int64[] retArr = outArray.GetArrayForWrite(); 
                Int64[] aArr = A.GetArrayForRead();
                if (outArray.S.NumberOfElements != outLen 
                    && !A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILNumerics.ILMemoryPool.Pool.New<Int64>(outLen); 
                    inplace = false; 
                }
                unsafe {
                    fixed ( Int64* inA2 =retArr)
                    fixed ( Int64* inA1 = aArr) {
                       
                        Int64* pInA1 = inA1;
                       
                        Int64* pInA2 = inA2;
                       
                        Int64* outEnd = inA2 + outLen;
                        if (!inplace) {
                            while (pInA2 < outEnd) {  //HC11
                               
                                *pInA2++ =   (*pInA1++ /*HC:*/ *(-1)/*dummy*/);
                            }
                        } else {
                            while (pInA2 < outEnd) {  //HC11
                                *pInA2 =   (*pInA2 /*HC:*/ *(-1)/*dummy*/);
                                pInA2++; 
                            }
                        }
                    }
                }
                outArray.a = new ILRetArray< Int64>(new ILDenseStorage< Int64>(retArr, outDims)); 
                return;
            }
        }
        /// <summary>
        /// Invert elements of A, return result into predefined output array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="outArray">Predefined array for results</param>
        public static void  invert(ILInArray<Int32> A, ILOutArray<Int32> outArray) {
            using (ILScope.Enter(A)) {
                ILSize inDim = A.Size;
                if (object.Equals(outArray,null))
                    throw new ILArgumentException("output parameter 'outArray' must not be null but reference existing array");

                bool inplace = true; 
                int outLen = A.S.NumberOfElements; 
                ILSize outDims = A.S; 
                Int32[] retArr = outArray.GetArrayForWrite(); 
                Int32[] aArr = A.GetArrayForRead();
                if (outArray.S.NumberOfElements != outLen 
                    && !A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILNumerics.ILMemoryPool.Pool.New<Int32>(outLen); 
                    inplace = false; 
                }
                unsafe {
                    fixed ( Int32* inA2 =retArr)
                    fixed ( Int32* inA1 = aArr) {
                       
                        Int32* pInA1 = inA1;
                       
                        Int32* pInA2 = inA2;
                       
                        Int32* outEnd = inA2 + outLen;
                        if (!inplace) {
                            while (pInA2 < outEnd) {  //HC11
                               
                                *pInA2++ =   (*pInA1++ /*HC:*/ *(-1)/*dummy*/);
                            }
                        } else {
                            while (pInA2 < outEnd) {  //HC11
                                *pInA2 =   (*pInA2 /*HC:*/ *(-1)/*dummy*/);
                                pInA2++; 
                            }
                        }
                    }
                }
                outArray.a = new ILRetArray< Int32>(new ILDenseStorage< Int32>(retArr, outDims)); 
                return;
            }
        }
        /// <summary>
        /// Invert elements of A, return result into predefined output array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="outArray">Predefined array for results</param>
        public static void  invert(ILInArray<float> A, ILOutArray<float> outArray) {
            using (ILScope.Enter(A)) {
                ILSize inDim = A.Size;
                if (object.Equals(outArray,null))
                    throw new ILArgumentException("output parameter 'outArray' must not be null but reference existing array");

                bool inplace = true; 
                int outLen = A.S.NumberOfElements; 
                ILSize outDims = A.S; 
                float[] retArr = outArray.GetArrayForWrite(); 
                float[] aArr = A.GetArrayForRead();
                if (outArray.S.NumberOfElements != outLen 
                    && !A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILNumerics.ILMemoryPool.Pool.New<float>(outLen); 
                    inplace = false; 
                }
                unsafe {
                    fixed ( float* inA2 =retArr)
                    fixed ( float* inA1 = aArr) {
                       
                        float* pInA1 = inA1;
                       
                        float* pInA2 = inA2;
                       
                        float* outEnd = inA2 + outLen;
                        if (!inplace) {
                            while (pInA2 < outEnd) {  //HC11
                               
                                *pInA2++ =   (*pInA1++ /*HC:*/ *(-1.0f)/*dummy*/);
                            }
                        } else {
                            while (pInA2 < outEnd) {  //HC11
                                *pInA2 =   (*pInA2 /*HC:*/ *(-1.0f)/*dummy*/);
                                pInA2++; 
                            }
                        }
                    }
                }
                outArray.a = new ILRetArray< float>(new ILDenseStorage< float>(retArr, outDims)); 
                return;
            }
        }
        /// <summary>
        /// Invert elements of A, return result into predefined output array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="outArray">Predefined array for results</param>
        public static void  invert(ILInArray<fcomplex> A, ILOutArray<fcomplex> outArray) {
            using (ILScope.Enter(A)) {
                ILSize inDim = A.Size;
                if (object.Equals(outArray,null))
                    throw new ILArgumentException("output parameter 'outArray' must not be null but reference existing array");

                bool inplace = true; 
                int outLen = A.S.NumberOfElements; 
                ILSize outDims = A.S; 
                fcomplex[] retArr = outArray.GetArrayForWrite(); 
                fcomplex[] aArr = A.GetArrayForRead();
                if (outArray.S.NumberOfElements != outLen 
                    && !A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILNumerics.ILMemoryPool.Pool.New<fcomplex>(outLen); 
                    inplace = false; 
                }
                unsafe {
                    fixed ( fcomplex* inA2 =retArr)
                    fixed ( fcomplex* inA1 = aArr) {
                       
                        fcomplex* pInA1 = inA1;
                       
                        fcomplex* pInA2 = inA2;
                       
                        fcomplex* outEnd = inA2 + outLen;
                        if (!inplace) {
                            while (pInA2 < outEnd) {  //HC11
                               
                                *pInA2++ =   (*pInA1++ /*HC:*/ *(-1.0f)/*dummy*/);
                            }
                        } else {
                            while (pInA2 < outEnd) {  //HC11
                                *pInA2 =   (*pInA2 /*HC:*/ *(-1.0f)/*dummy*/);
                                pInA2++; 
                            }
                        }
                    }
                }
                outArray.a = new ILRetArray< fcomplex>(new ILDenseStorage< fcomplex>(retArr, outDims)); 
                return;
            }
        }
        /// <summary>
        /// Invert elements of A, return result into predefined output array
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="outArray">Predefined array for results</param>
        public static void  invert(ILInArray<complex> A, ILOutArray<complex> outArray) {
            using (ILScope.Enter(A)) {
                ILSize inDim = A.Size;
                if (object.Equals(outArray,null))
                    throw new ILArgumentException("output parameter 'outArray' must not be null but reference existing array");

                bool inplace = true; 
                int outLen = A.S.NumberOfElements; 
                ILSize outDims = A.S; 
                complex[] retArr = outArray.GetArrayForWrite(); 
                complex[] aArr = A.GetArrayForRead();
                if (outArray.S.NumberOfElements != outLen 
                    && !A.TryGetStorage4InplaceOp(out retArr)) {
                        retArr = ILNumerics.ILMemoryPool.Pool.New<complex>(outLen); 
                    inplace = false; 
                }
                unsafe {
                    fixed ( complex* inA2 =retArr)
                    fixed ( complex* inA1 = aArr) {
                       
                        complex* pInA1 = inA1;
                       
                        complex* pInA2 = inA2;
                       
                        complex* outEnd = inA2 + outLen;
                        if (!inplace) {
                            while (pInA2 < outEnd) {  //HC11
                               
                                *pInA2++ =   (*pInA1++ /*HC:*/ *(-1.0)/*dummy*/);
                            }
                        } else {
                            while (pInA2 < outEnd) {  //HC11
                                *pInA2 =   (*pInA2 /*HC:*/ *(-1.0)/*dummy*/);
                                pInA2++; 
                            }
                        }
                    }
                }
                outArray.a = new ILRetArray< complex>(new ILDenseStorage< complex>(retArr, outDims)); 
                return;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}