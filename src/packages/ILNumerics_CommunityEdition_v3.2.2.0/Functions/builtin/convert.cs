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
using System.Runtime.InteropServices; 
using System.Numerics;


namespace ILNumerics  {
    public partial class ILMath {
        /// <summary>
        /// Convert a numeric array to another numeric type
        /// </summary>
        /// <param name="X">Input array</param>
        /// <typeparam name="inT">Type of array to convert</typeparam>
        /// <typeparam name="outT">Type of array to return</typeparam>
        /// <returns>Converted array</returns>
        /// <remarks> The newly created array will be converted to the required type. 
        /// <para>The array returned will always use new memory! Even if the type requested 
        /// matches the incoming type.</para></remarks>
        [System.Security.SecuritySafeCritical]
        public static unsafe ILRetArray<outT> convert<inT, outT>(ILInArray<inT> X) {
            using (ILScope.Enter(X)) {
                outT[] retArrGen = ILMemoryPool.Pool.New<outT>(X.Size.NumberOfElements); 
                inT[] inArrGen = X.GetArrayForRead(); 
                if (inArrGen is double[]) {
    #region input double
                    double[] inArr = (double[])(object)inArrGen;  
                    if (false) {

                    } else if (retArrGen is  double []) {
						 double [] dummyArray = ( double [])(object)retArrGen; 
                        fixed ( double * pretArr = dummyArray)
                        fixed (double * pinArr = inArr) {
                            double * pInWalk = pinArr; 
                            double * pInEnd = pinArr + X.S.NumberOfElements;
                             double * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( double ) (*(pInWalk++)); 
                            }
                        }

#region HYCALPER AUTO GENERATED CODE

                    } else if (retArrGen is  Int64 []) {
						 Int64 [] dummyArray = ( Int64 [])(object)retArrGen; 
                        fixed ( Int64 * pretArr = dummyArray)
                        fixed (double * pinArr = inArr) {
                            double * pInWalk = pinArr; 
                            double * pInEnd = pinArr + X.S.NumberOfElements;
                            Int64 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int64 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int32 []) {
						 Int32 [] dummyArray = ( Int32 [])(object)retArrGen; 
                        fixed ( Int32 * pretArr = dummyArray)
                        fixed (double * pinArr = inArr) {
                            double * pInWalk = pinArr; 
                            double * pInEnd = pinArr + X.S.NumberOfElements;
                            Int32 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int32 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int16 []) {
						 Int16 [] dummyArray = ( Int16 [])(object)retArrGen; 
                        fixed ( Int16 * pretArr = dummyArray)
                        fixed (double * pinArr = inArr) {
                            double * pInWalk = pinArr; 
                            double * pInEnd = pinArr + X.S.NumberOfElements;
                            Int16 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int16 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  byte []) {
						 byte [] dummyArray = ( byte [])(object)retArrGen; 
                        fixed ( byte * pretArr = dummyArray)
                        fixed (double * pinArr = inArr) {
                            double * pInWalk = pinArr; 
                            double * pInEnd = pinArr + X.S.NumberOfElements;
                            byte * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( byte ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  fcomplex []) {
						 fcomplex [] dummyArray = ( fcomplex [])(object)retArrGen; 
                        fixed ( fcomplex * pretArr = dummyArray)
                        fixed (double * pinArr = inArr) {
                            double * pInWalk = pinArr; 
                            double * pInEnd = pinArr + X.S.NumberOfElements;
                            fcomplex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( fcomplex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  complex []) {
						 complex [] dummyArray = ( complex [])(object)retArrGen; 
                        fixed ( complex * pretArr = dummyArray)
                        fixed (double * pinArr = inArr) {
                            double * pInWalk = pinArr; 
                            double * pInEnd = pinArr + X.S.NumberOfElements;
                            complex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( complex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  float []) {
						 float [] dummyArray = ( float [])(object)retArrGen; 
                        fixed ( float * pretArr = dummyArray)
                        fixed (double * pinArr = inArr) {
                            double * pInWalk = pinArr; 
                            double * pInEnd = pinArr + X.S.NumberOfElements;
                            float * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( float ) (*(pInWalk++)); 
                            }
                        }

#endregion HYCALPER AUTO GENERATED CODE
                   } else 
                        throw new ILArgumentException("unsupported target type: " + typeof(outT).Name);
    #endregion
                } else if (inArrGen is float[]) {
    #region input float
                    float[] inArr = (float[])(object)inArrGen;  
                    if (false) {

                    } else if (retArrGen is  double []) {
						 double [] dummyArray = ( double [])(object)retArrGen; 
                        fixed ( double * pretArr = dummyArray)
                        fixed (float * pinArr = inArr) {
                            float * pInWalk = pinArr; 
                            float * pInEnd = pinArr + X.S.NumberOfElements;
                             double * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( double ) (*(pInWalk++)); 
                            }
                        }

#region HYCALPER AUTO GENERATED CODE

                    } else if (retArrGen is  Int64 []) {
						 Int64 [] dummyArray = ( Int64 [])(object)retArrGen; 
                        fixed ( Int64 * pretArr = dummyArray)
                        fixed (float * pinArr = inArr) {
                            float * pInWalk = pinArr; 
                            float * pInEnd = pinArr + X.S.NumberOfElements;
                            Int64 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int64 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int32 []) {
						 Int32 [] dummyArray = ( Int32 [])(object)retArrGen; 
                        fixed ( Int32 * pretArr = dummyArray)
                        fixed (float * pinArr = inArr) {
                            float * pInWalk = pinArr; 
                            float * pInEnd = pinArr + X.S.NumberOfElements;
                            Int32 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int32 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int16 []) {
						 Int16 [] dummyArray = ( Int16 [])(object)retArrGen; 
                        fixed ( Int16 * pretArr = dummyArray)
                        fixed (float * pinArr = inArr) {
                            float * pInWalk = pinArr; 
                            float * pInEnd = pinArr + X.S.NumberOfElements;
                            Int16 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int16 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  byte []) {
						 byte [] dummyArray = ( byte [])(object)retArrGen; 
                        fixed ( byte * pretArr = dummyArray)
                        fixed (float * pinArr = inArr) {
                            float * pInWalk = pinArr; 
                            float * pInEnd = pinArr + X.S.NumberOfElements;
                            byte * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( byte ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  fcomplex []) {
						 fcomplex [] dummyArray = ( fcomplex [])(object)retArrGen; 
                        fixed ( fcomplex * pretArr = dummyArray)
                        fixed (float * pinArr = inArr) {
                            float * pInWalk = pinArr; 
                            float * pInEnd = pinArr + X.S.NumberOfElements;
                            fcomplex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( fcomplex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  complex []) {
						 complex [] dummyArray = ( complex [])(object)retArrGen; 
                        fixed ( complex * pretArr = dummyArray)
                        fixed (float * pinArr = inArr) {
                            float * pInWalk = pinArr; 
                            float * pInEnd = pinArr + X.S.NumberOfElements;
                            complex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( complex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  float []) {
						 float [] dummyArray = ( float [])(object)retArrGen; 
                        fixed ( float * pretArr = dummyArray)
                        fixed (float * pinArr = inArr) {
                            float * pInWalk = pinArr; 
                            float * pInEnd = pinArr + X.S.NumberOfElements;
                            float * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( float ) (*(pInWalk++)); 
                            }
                        }

#endregion HYCALPER AUTO GENERATED CODE
                   } else 
                        throw new ILArgumentException("unsupported target type: " + typeof(outT).Name);
    #endregion
                } else if (inArrGen is complex[]) {
    #region input complex
                    complex[] inArr = (complex[])(object)inArrGen;  
                    if (false) {

                    } else if (retArrGen is  double []) {
						 double [] dummyArray = ( double [])(object)retArrGen;
                        fixed ( double * pretArr = dummyArray)
                        fixed (complex * pinArr = inArr) {
                            complex * pInWalk = pinArr; 
                            complex * pInEnd = pinArr + X.S.NumberOfElements;
                             double * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( double ) (*(pInWalk++)); 
                            }
                        }

#region HYCALPER AUTO GENERATED CODE

                    } else if (retArrGen is  Int64 []) {
						 Int64 [] dummyArray = ( Int64 [])(object)retArrGen;
                        fixed ( Int64 * pretArr = dummyArray)
                        fixed (complex * pinArr = inArr) {
                            complex * pInWalk = pinArr; 
                            complex * pInEnd = pinArr + X.S.NumberOfElements;
                            Int64 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int64 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int32 []) {
						 Int32 [] dummyArray = ( Int32 [])(object)retArrGen;
                        fixed ( Int32 * pretArr = dummyArray)
                        fixed (complex * pinArr = inArr) {
                            complex * pInWalk = pinArr; 
                            complex * pInEnd = pinArr + X.S.NumberOfElements;
                            Int32 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int32 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int16 []) {
						 Int16 [] dummyArray = ( Int16 [])(object)retArrGen;
                        fixed ( Int16 * pretArr = dummyArray)
                        fixed (complex * pinArr = inArr) {
                            complex * pInWalk = pinArr; 
                            complex * pInEnd = pinArr + X.S.NumberOfElements;
                            Int16 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int16 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  byte []) {
						 byte [] dummyArray = ( byte [])(object)retArrGen;
                        fixed ( byte * pretArr = dummyArray)
                        fixed (complex * pinArr = inArr) {
                            complex * pInWalk = pinArr; 
                            complex * pInEnd = pinArr + X.S.NumberOfElements;
                            byte * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( byte ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  fcomplex []) {
						 fcomplex [] dummyArray = ( fcomplex [])(object)retArrGen;
                        fixed ( fcomplex * pretArr = dummyArray)
                        fixed (complex * pinArr = inArr) {
                            complex * pInWalk = pinArr; 
                            complex * pInEnd = pinArr + X.S.NumberOfElements;
                            fcomplex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( fcomplex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  complex []) {
						 complex [] dummyArray = ( complex [])(object)retArrGen;
                        fixed ( complex * pretArr = dummyArray)
                        fixed (complex * pinArr = inArr) {
                            complex * pInWalk = pinArr; 
                            complex * pInEnd = pinArr + X.S.NumberOfElements;
                            complex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( complex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  float []) {
						 float [] dummyArray = ( float [])(object)retArrGen;
                        fixed ( float * pretArr = dummyArray)
                        fixed (complex * pinArr = inArr) {
                            complex * pInWalk = pinArr; 
                            complex * pInEnd = pinArr + X.S.NumberOfElements;
                            float * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( float ) (*(pInWalk++)); 
                            }
                        }

#endregion HYCALPER AUTO GENERATED CODE
                    } else if (retArrGen is Complex[]) {
                        GCHandle retHandle = GCHandle.Alloc(retArrGen,GCHandleType.Pinned); 
                        GCHandle inHandle = GCHandle.Alloc(inArrGen,GCHandleType.Pinned);
                        complex* retP = (complex*)retHandle.AddrOfPinnedObject();
                        complex* inP = (complex*)inHandle.AddrOfPinnedObject();
                        complex2ComplexHelper(inP, retP, X.S.NumberOfElements);
                        retHandle.Free(); 
                        inHandle.Free(); 
                    } else 
                        throw new ILArgumentException("unsupported target type: " + typeof(outT).Name);
    #endregion
                } else if (inArrGen is fcomplex[]) {
    #region input fcomplex
                    fcomplex[] inArr = (fcomplex[])(object)inArrGen;  
                    if (false) {

                    } else if (retArrGen is  double []) {
						 double [] dummyArray = ( double [])(object)retArrGen;
                        fixed ( double * pretArr = dummyArray)
                        fixed (fcomplex * pinArr = inArr) {
                            fcomplex * pInWalk = pinArr; 
                            fcomplex * pInEnd = pinArr + X.S.NumberOfElements;
                             double * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( double ) (*(pInWalk++)); 
                            }
                        }

#region HYCALPER AUTO GENERATED CODE

                    } else if (retArrGen is  Int64 []) {
						 Int64 [] dummyArray = ( Int64 [])(object)retArrGen;
                        fixed ( Int64 * pretArr = dummyArray)
                        fixed (fcomplex * pinArr = inArr) {
                            fcomplex * pInWalk = pinArr; 
                            fcomplex * pInEnd = pinArr + X.S.NumberOfElements;
                            Int64 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int64 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int32 []) {
						 Int32 [] dummyArray = ( Int32 [])(object)retArrGen;
                        fixed ( Int32 * pretArr = dummyArray)
                        fixed (fcomplex * pinArr = inArr) {
                            fcomplex * pInWalk = pinArr; 
                            fcomplex * pInEnd = pinArr + X.S.NumberOfElements;
                            Int32 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int32 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int16 []) {
						 Int16 [] dummyArray = ( Int16 [])(object)retArrGen;
                        fixed ( Int16 * pretArr = dummyArray)
                        fixed (fcomplex * pinArr = inArr) {
                            fcomplex * pInWalk = pinArr; 
                            fcomplex * pInEnd = pinArr + X.S.NumberOfElements;
                            Int16 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int16 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  byte []) {
						 byte [] dummyArray = ( byte [])(object)retArrGen;
                        fixed ( byte * pretArr = dummyArray)
                        fixed (fcomplex * pinArr = inArr) {
                            fcomplex * pInWalk = pinArr; 
                            fcomplex * pInEnd = pinArr + X.S.NumberOfElements;
                            byte * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( byte ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  fcomplex []) {
						 fcomplex [] dummyArray = ( fcomplex [])(object)retArrGen;
                        fixed ( fcomplex * pretArr = dummyArray)
                        fixed (fcomplex * pinArr = inArr) {
                            fcomplex * pInWalk = pinArr; 
                            fcomplex * pInEnd = pinArr + X.S.NumberOfElements;
                            fcomplex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( fcomplex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  complex []) {
						 complex [] dummyArray = ( complex [])(object)retArrGen;
                        fixed ( complex * pretArr = dummyArray)
                        fixed (fcomplex * pinArr = inArr) {
                            fcomplex * pInWalk = pinArr; 
                            fcomplex * pInEnd = pinArr + X.S.NumberOfElements;
                            complex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( complex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  float []) {
						 float [] dummyArray = ( float [])(object)retArrGen;
                        fixed ( float * pretArr = dummyArray)
                        fixed (fcomplex * pinArr = inArr) {
                            fcomplex * pInWalk = pinArr; 
                            fcomplex * pInEnd = pinArr + X.S.NumberOfElements;
                            float * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( float ) (*(pInWalk++)); 
                            }
                        }

#endregion HYCALPER AUTO GENERATED CODE
                   } else 
                        throw new ILArgumentException("unsupported target type: " + typeof(outT).Name);
    #endregion
                } else if (inArrGen is byte[]) {
    #region input byte
                    byte[] inArr = (byte[])(object)inArrGen;  
                    if (false) {

                    } else if (retArrGen is  double []) {
						 double [] dummyArray = ( double [])(object)retArrGen;
                        fixed ( double * pretArr = dummyArray)
                        fixed (byte * pinArr = inArr) {
                            byte * pInWalk = pinArr; 
                            byte * pInEnd = pinArr + X.S.NumberOfElements;
                             double * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( double ) (*(pInWalk++)); 
                            }
                        }

#region HYCALPER AUTO GENERATED CODE

                    } else if (retArrGen is  Int64 []) {
						 Int64 [] dummyArray = ( Int64 [])(object)retArrGen;
                        fixed ( Int64 * pretArr = dummyArray)
                        fixed (byte * pinArr = inArr) {
                            byte * pInWalk = pinArr; 
                            byte * pInEnd = pinArr + X.S.NumberOfElements;
                            Int64 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int64 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int32 []) {
						 Int32 [] dummyArray = ( Int32 [])(object)retArrGen;
                        fixed ( Int32 * pretArr = dummyArray)
                        fixed (byte * pinArr = inArr) {
                            byte * pInWalk = pinArr; 
                            byte * pInEnd = pinArr + X.S.NumberOfElements;
                            Int32 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int32 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int16 []) {
						 Int16 [] dummyArray = ( Int16 [])(object)retArrGen;
                        fixed ( Int16 * pretArr = dummyArray)
                        fixed (byte * pinArr = inArr) {
                            byte * pInWalk = pinArr; 
                            byte * pInEnd = pinArr + X.S.NumberOfElements;
                            Int16 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int16 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  byte []) {
						 byte [] dummyArray = ( byte [])(object)retArrGen;
                        fixed ( byte * pretArr = dummyArray)
                        fixed (byte * pinArr = inArr) {
                            byte * pInWalk = pinArr; 
                            byte * pInEnd = pinArr + X.S.NumberOfElements;
                            byte * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( byte ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  fcomplex []) {
						 fcomplex [] dummyArray = ( fcomplex [])(object)retArrGen;
                        fixed ( fcomplex * pretArr = dummyArray)
                        fixed (byte * pinArr = inArr) {
                            byte * pInWalk = pinArr; 
                            byte * pInEnd = pinArr + X.S.NumberOfElements;
                            fcomplex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( fcomplex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  complex []) {
						 complex [] dummyArray = ( complex [])(object)retArrGen;
                        fixed ( complex * pretArr = dummyArray)
                        fixed (byte * pinArr = inArr) {
                            byte * pInWalk = pinArr; 
                            byte * pInEnd = pinArr + X.S.NumberOfElements;
                            complex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( complex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  float []) {
						 float [] dummyArray = ( float [])(object)retArrGen;
                        fixed ( float * pretArr = dummyArray)
                        fixed (byte * pinArr = inArr) {
                            byte * pInWalk = pinArr; 
                            byte * pInEnd = pinArr + X.S.NumberOfElements;
                            float * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( float ) (*(pInWalk++)); 
                            }
                        }

#endregion HYCALPER AUTO GENERATED CODE
                   } else 
                        throw new ILArgumentException("unsupported target type: " + typeof(outT).Name);
    #endregion
                } else if (inArrGen is Int16[]) {
    #region input Int16
                    Int16[] inArr = (Int16[])(object)inArrGen;  
                    if (false) {

                    } else if (retArrGen is  double []) {
						 double [] dummyArray = ( double [])(object)retArrGen;
                        fixed ( double * pretArr = dummyArray)
                        fixed (Int16 * pinArr = inArr) {
                            Int16 * pInWalk = pinArr;
                            Int16 * pInEnd = pinArr + X.S.NumberOfElements;
                             double * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( double ) (*(pInWalk++)); 
                            }
                        }

#region HYCALPER AUTO GENERATED CODE

                    } else if (retArrGen is  Int64 []) {
						 Int64 [] dummyArray = ( Int64 [])(object)retArrGen;
                        fixed ( Int64 * pretArr = dummyArray)
                        fixed (Int16 * pinArr = inArr) {
                            Int16 * pInWalk = pinArr;
                            Int16 * pInEnd = pinArr + X.S.NumberOfElements;
                            Int64 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int64 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int32 []) {
						 Int32 [] dummyArray = ( Int32 [])(object)retArrGen;
                        fixed ( Int32 * pretArr = dummyArray)
                        fixed (Int16 * pinArr = inArr) {
                            Int16 * pInWalk = pinArr;
                            Int16 * pInEnd = pinArr + X.S.NumberOfElements;
                            Int32 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int32 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int16 []) {
						 Int16 [] dummyArray = ( Int16 [])(object)retArrGen;
                        fixed ( Int16 * pretArr = dummyArray)
                        fixed (Int16 * pinArr = inArr) {
                            Int16 * pInWalk = pinArr;
                            Int16 * pInEnd = pinArr + X.S.NumberOfElements;
                            Int16 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int16 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  byte []) {
						 byte [] dummyArray = ( byte [])(object)retArrGen;
                        fixed ( byte * pretArr = dummyArray)
                        fixed (Int16 * pinArr = inArr) {
                            Int16 * pInWalk = pinArr;
                            Int16 * pInEnd = pinArr + X.S.NumberOfElements;
                            byte * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( byte ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  fcomplex []) {
						 fcomplex [] dummyArray = ( fcomplex [])(object)retArrGen;
                        fixed ( fcomplex * pretArr = dummyArray)
                        fixed (Int16 * pinArr = inArr) {
                            Int16 * pInWalk = pinArr;
                            Int16 * pInEnd = pinArr + X.S.NumberOfElements;
                            fcomplex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( fcomplex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  complex []) {
						 complex [] dummyArray = ( complex [])(object)retArrGen;
                        fixed ( complex * pretArr = dummyArray)
                        fixed (Int16 * pinArr = inArr) {
                            Int16 * pInWalk = pinArr;
                            Int16 * pInEnd = pinArr + X.S.NumberOfElements;
                            complex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( complex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  float []) {
						 float [] dummyArray = ( float [])(object)retArrGen;
                        fixed ( float * pretArr = dummyArray)
                        fixed (Int16 * pinArr = inArr) {
                            Int16 * pInWalk = pinArr;
                            Int16 * pInEnd = pinArr + X.S.NumberOfElements;
                            float * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( float ) (*(pInWalk++)); 
                            }
                        }

#endregion HYCALPER AUTO GENERATED CODE
                   } else 
                        throw new ILArgumentException("unsupported target type: " + typeof(outT).Name);
    #endregion
                } else if (inArrGen is Int32[]) {
    #region input Int32
                    Int32[] inArr = (Int32[])(object)inArrGen;  
                    if (false) {

                    } else if (retArrGen is  double []) {
						 double [] dummyArray = ( double [])(object)retArrGen;
                        fixed ( double * pretArr = dummyArray)
                        fixed (Int32 * pinArr = inArr) {
                            Int32 * pInWalk = pinArr; 
                            Int32 * pInEnd = pinArr + X.S.NumberOfElements;
                             double * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( double ) (*(pInWalk++)); 
                            }
                        }

#region HYCALPER AUTO GENERATED CODE

                    } else if (retArrGen is  Int64 []) {
						 Int64 [] dummyArray = ( Int64 [])(object)retArrGen;
                        fixed ( Int64 * pretArr = dummyArray)
                        fixed (Int32 * pinArr = inArr) {
                            Int32 * pInWalk = pinArr; 
                            Int32 * pInEnd = pinArr + X.S.NumberOfElements;
                            Int64 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int64 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int32 []) {
						 Int32 [] dummyArray = ( Int32 [])(object)retArrGen;
                        fixed ( Int32 * pretArr = dummyArray)
                        fixed (Int32 * pinArr = inArr) {
                            Int32 * pInWalk = pinArr; 
                            Int32 * pInEnd = pinArr + X.S.NumberOfElements;
                            Int32 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int32 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int16 []) {
						 Int16 [] dummyArray = ( Int16 [])(object)retArrGen;
                        fixed ( Int16 * pretArr = dummyArray)
                        fixed (Int32 * pinArr = inArr) {
                            Int32 * pInWalk = pinArr; 
                            Int32 * pInEnd = pinArr + X.S.NumberOfElements;
                            Int16 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int16 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  byte []) {
						 byte [] dummyArray = ( byte [])(object)retArrGen;
                        fixed ( byte * pretArr = dummyArray)
                        fixed (Int32 * pinArr = inArr) {
                            Int32 * pInWalk = pinArr; 
                            Int32 * pInEnd = pinArr + X.S.NumberOfElements;
                            byte * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( byte ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  fcomplex []) {
						 fcomplex [] dummyArray = ( fcomplex [])(object)retArrGen;
                        fixed ( fcomplex * pretArr = dummyArray)
                        fixed (Int32 * pinArr = inArr) {
                            Int32 * pInWalk = pinArr; 
                            Int32 * pInEnd = pinArr + X.S.NumberOfElements;
                            fcomplex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( fcomplex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  complex []) {
						 complex [] dummyArray = ( complex [])(object)retArrGen;
                        fixed ( complex * pretArr = dummyArray)
                        fixed (Int32 * pinArr = inArr) {
                            Int32 * pInWalk = pinArr; 
                            Int32 * pInEnd = pinArr + X.S.NumberOfElements;
                            complex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( complex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  float []) {
						 float [] dummyArray = ( float [])(object)retArrGen;
                        fixed ( float * pretArr = dummyArray)
                        fixed (Int32 * pinArr = inArr) {
                            Int32 * pInWalk = pinArr; 
                            Int32 * pInEnd = pinArr + X.S.NumberOfElements;
                            float * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( float ) (*(pInWalk++)); 
                            }
                        }

#endregion HYCALPER AUTO GENERATED CODE
                   } else 
                        throw new ILArgumentException("unsupported target type: " + typeof(outT).Name);
    #endregion
                } else if (inArrGen is Int64[]) {
    #region input Int64
                    Int64[] inArr = (Int64[])(object)inArrGen;  
                    if (false) {

                    } else if (retArrGen is  double []) {
						 double [] dummyArray = ( double [])(object)retArrGen;
                        fixed ( double * pretArr = dummyArray)
                        fixed (Int64 * pinArr = inArr) {
                            Int64 * pInWalk = pinArr; 
                            Int64 * pInEnd = pinArr + X.S.NumberOfElements;
                             double * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( double ) (*(pInWalk++)); 
                            }
                        }

#region HYCALPER AUTO GENERATED CODE

                    } else if (retArrGen is  Int64 []) {
						 Int64 [] dummyArray = ( Int64 [])(object)retArrGen;
                        fixed ( Int64 * pretArr = dummyArray)
                        fixed (Int64 * pinArr = inArr) {
                            Int64 * pInWalk = pinArr; 
                            Int64 * pInEnd = pinArr + X.S.NumberOfElements;
                            Int64 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int64 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int32 []) {
						 Int32 [] dummyArray = ( Int32 [])(object)retArrGen;
                        fixed ( Int32 * pretArr = dummyArray)
                        fixed (Int64 * pinArr = inArr) {
                            Int64 * pInWalk = pinArr; 
                            Int64 * pInEnd = pinArr + X.S.NumberOfElements;
                            Int32 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int32 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  Int16 []) {
						 Int16 [] dummyArray = ( Int16 [])(object)retArrGen;
                        fixed ( Int16 * pretArr = dummyArray)
                        fixed (Int64 * pinArr = inArr) {
                            Int64 * pInWalk = pinArr; 
                            Int64 * pInEnd = pinArr + X.S.NumberOfElements;
                            Int16 * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( Int16 ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  byte []) {
						 byte [] dummyArray = ( byte [])(object)retArrGen;
                        fixed ( byte * pretArr = dummyArray)
                        fixed (Int64 * pinArr = inArr) {
                            Int64 * pInWalk = pinArr; 
                            Int64 * pInEnd = pinArr + X.S.NumberOfElements;
                            byte * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( byte ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  fcomplex []) {
						 fcomplex [] dummyArray = ( fcomplex [])(object)retArrGen;
                        fixed ( fcomplex * pretArr = dummyArray)
                        fixed (Int64 * pinArr = inArr) {
                            Int64 * pInWalk = pinArr; 
                            Int64 * pInEnd = pinArr + X.S.NumberOfElements;
                            fcomplex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( fcomplex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  complex []) {
						 complex [] dummyArray = ( complex [])(object)retArrGen;
                        fixed ( complex * pretArr = dummyArray)
                        fixed (Int64 * pinArr = inArr) {
                            Int64 * pInWalk = pinArr; 
                            Int64 * pInEnd = pinArr + X.S.NumberOfElements;
                            complex * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( complex ) (*(pInWalk++)); 
                            }
                        }
                    } else if (retArrGen is  float []) {
						 float [] dummyArray = ( float [])(object)retArrGen;
                        fixed ( float * pretArr = dummyArray)
                        fixed (Int64 * pinArr = inArr) {
                            Int64 * pInWalk = pinArr; 
                            Int64 * pInEnd = pinArr + X.S.NumberOfElements;
                            float * pRetWalk = pretArr; 
                            while (pInWalk < pInEnd) {
                                *(pRetWalk++) = ( float ) (*(pInWalk++)); 
                            }
                        }

#endregion HYCALPER AUTO GENERATED CODE
                   } else 
                        throw new ILArgumentException("unsupported target type: " + typeof(outT).Name);
    #endregion
                } else if (inArrGen is Complex[] && retArrGen is complex[]) {
                    GCHandle retHandle = GCHandle.Alloc(retArrGen, GCHandleType.Pinned);
                    GCHandle inHandle = GCHandle.Alloc(inArrGen, GCHandleType.Pinned);
                    complex* retP = (complex*)retHandle.AddrOfPinnedObject();
                    complex* inP = (complex*)inHandle.AddrOfPinnedObject();
                    complex2ComplexHelper(inP, retP, X.S.NumberOfElements);
                    retHandle.Free();
                    inHandle.Free();
                } else
                    throw new ILArgumentException(String.Format("conversion from {0} to {1} is currently not supported.", typeof(inT).Name, typeof(outT).Name)); 
                return new ILRetArray<outT>(retArrGen,X.Size); 
            }
        }

        unsafe internal static void complex2ComplexHelper(complex* inArr, complex* outArr, int len) {
            while (len > 8) {
                outArr[0] = inArr[0];
                outArr[1] = inArr[1];
                outArr[2] = inArr[2];
                outArr[3] = inArr[3];
                outArr[4] = inArr[4];
                outArr[5] = inArr[5];
                outArr[6] = inArr[6];
                outArr[7] = inArr[7];
                inArr += 8; outArr += 8; len -= 8;
            }
            while (len-- > 0) *outArr++ = *inArr++;
        }
       

        /// <summary>
        /// Convert numeric array to double array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>double array</returns>
        /// <remarks><para>The function converts elements of X to double using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<double> todouble(ILInArray< double > X) {
             return convert< double ,double>(X); 
        }
        /// <summary>
        /// Convert numeric array to float array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>float array</returns>
        /// <remarks><para>The new array converts elements of X to float using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<float> tosingle(ILInArray< double > X) {
             return convert< double ,float>(X); 
        }
        /// <summary>
        /// Convert numeric array to complex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>complex array</returns>
        /// <remarks><para>Real input arrays will be converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to complex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<complex> tocomplex(ILInArray< double > X) {
             return convert< double ,complex>(X); 
        }
        /// <summary>
        /// Convert numeric array to fcomplex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>fcomplex array</returns>
        /// <remarks>
        /// <para>Real input arrays are converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to fcomplex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<fcomplex> tofcomplex(ILInArray< double > X) {
             return convert< double ,fcomplex>(X); 
        }
        /// <summary>
        /// Convert numeric array to byte array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>byte array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<byte> tobyte(ILInArray< double > X) {
             return convert< double ,byte>(X); 
        }
        /// <summary>
        /// Convert numeric array to logical array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Logical array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. Non-zero
        /// elements are converted to true, zero-elements are converted to false. 
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetLogical tological(ILInArray< double > X) {
             return new ILRetLogical (convert< double ,byte>(X).Storage as ILDenseStorage<byte>); 
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int16> toint16(ILInArray< double> X) {
            return convert< double, Int16>(X);
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int32> toint32(ILInArray< double> X) {
            return convert< double, Int32>(X);
        }
        /// <summary>
        /// Convert numeric array to Int64 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int64 array</returns>
        /// <remarks><para>The function converts elements of X to Int64 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int64> toint64(ILInArray< double > X) {
             return convert< double ,Int64>(X); 
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Convert numeric array to double array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>double array</returns>
        /// <remarks><para>The function converts elements of X to double using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<double> todouble(ILInArray< Int64 > X) {
             return convert< Int64 ,double>(X); 
        }
        /// <summary>
        /// Convert numeric array to float array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>float array</returns>
        /// <remarks><para>The new array converts elements of X to float using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<float> tosingle(ILInArray< Int64 > X) {
             return convert< Int64 ,float>(X); 
        }
        /// <summary>
        /// Convert numeric array to complex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>complex array</returns>
        /// <remarks><para>Real input arrays will be converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to complex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<complex> tocomplex(ILInArray< Int64 > X) {
             return convert< Int64 ,complex>(X); 
        }
        /// <summary>
        /// Convert numeric array to fcomplex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>fcomplex array</returns>
        /// <remarks>
        /// <para>Real input arrays are converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to fcomplex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<fcomplex> tofcomplex(ILInArray< Int64 > X) {
             return convert< Int64 ,fcomplex>(X); 
        }
        /// <summary>
        /// Convert numeric array to byte array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>byte array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<byte> tobyte(ILInArray< Int64 > X) {
             return convert< Int64 ,byte>(X); 
        }
        /// <summary>
        /// Convert numeric array to logical array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Logical array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. Non-zero
        /// elements are converted to true, zero-elements are converted to false. 
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetLogical tological(ILInArray< Int64 > X) {
             return new ILRetLogical (convert< Int64 ,byte>(X).Storage as ILDenseStorage<byte>); 
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int16> toint16(ILInArray< Int64> X) {
            return convert< Int64, Int16>(X);
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int32> toint32(ILInArray< Int64> X) {
            return convert< Int64, Int32>(X);
        }
        /// <summary>
        /// Convert numeric array to Int64 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int64 array</returns>
        /// <remarks><para>The function converts elements of X to Int64 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int64> toint64(ILInArray< Int64 > X) {
             return convert< Int64 ,Int64>(X); 
        }
        /// <summary>
        /// Convert numeric array to double array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>double array</returns>
        /// <remarks><para>The function converts elements of X to double using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<double> todouble(ILInArray< Int32 > X) {
             return convert< Int32 ,double>(X); 
        }
        /// <summary>
        /// Convert numeric array to float array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>float array</returns>
        /// <remarks><para>The new array converts elements of X to float using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<float> tosingle(ILInArray< Int32 > X) {
             return convert< Int32 ,float>(X); 
        }
        /// <summary>
        /// Convert numeric array to complex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>complex array</returns>
        /// <remarks><para>Real input arrays will be converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to complex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<complex> tocomplex(ILInArray< Int32 > X) {
             return convert< Int32 ,complex>(X); 
        }
        /// <summary>
        /// Convert numeric array to fcomplex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>fcomplex array</returns>
        /// <remarks>
        /// <para>Real input arrays are converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to fcomplex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<fcomplex> tofcomplex(ILInArray< Int32 > X) {
             return convert< Int32 ,fcomplex>(X); 
        }
        /// <summary>
        /// Convert numeric array to byte array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>byte array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<byte> tobyte(ILInArray< Int32 > X) {
             return convert< Int32 ,byte>(X); 
        }
        /// <summary>
        /// Convert numeric array to logical array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Logical array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. Non-zero
        /// elements are converted to true, zero-elements are converted to false. 
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetLogical tological(ILInArray< Int32 > X) {
             return new ILRetLogical (convert< Int32 ,byte>(X).Storage as ILDenseStorage<byte>); 
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int16> toint16(ILInArray< Int32> X) {
            return convert< Int32, Int16>(X);
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int32> toint32(ILInArray< Int32> X) {
            return convert< Int32, Int32>(X);
        }
        /// <summary>
        /// Convert numeric array to Int64 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int64 array</returns>
        /// <remarks><para>The function converts elements of X to Int64 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int64> toint64(ILInArray< Int32 > X) {
             return convert< Int32 ,Int64>(X); 
        }
        /// <summary>
        /// Convert numeric array to double array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>double array</returns>
        /// <remarks><para>The function converts elements of X to double using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<double> todouble(ILInArray< Int16 > X) {
             return convert< Int16 ,double>(X); 
        }
        /// <summary>
        /// Convert numeric array to float array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>float array</returns>
        /// <remarks><para>The new array converts elements of X to float using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<float> tosingle(ILInArray< Int16 > X) {
             return convert< Int16 ,float>(X); 
        }
        /// <summary>
        /// Convert numeric array to complex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>complex array</returns>
        /// <remarks><para>Real input arrays will be converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to complex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<complex> tocomplex(ILInArray< Int16 > X) {
             return convert< Int16 ,complex>(X); 
        }
        /// <summary>
        /// Convert numeric array to fcomplex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>fcomplex array</returns>
        /// <remarks>
        /// <para>Real input arrays are converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to fcomplex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<fcomplex> tofcomplex(ILInArray< Int16 > X) {
             return convert< Int16 ,fcomplex>(X); 
        }
        /// <summary>
        /// Convert numeric array to byte array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>byte array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<byte> tobyte(ILInArray< Int16 > X) {
             return convert< Int16 ,byte>(X); 
        }
        /// <summary>
        /// Convert numeric array to logical array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Logical array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. Non-zero
        /// elements are converted to true, zero-elements are converted to false. 
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetLogical tological(ILInArray< Int16 > X) {
             return new ILRetLogical (convert< Int16 ,byte>(X).Storage as ILDenseStorage<byte>); 
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int16> toint16(ILInArray< Int16> X) {
            return convert< Int16, Int16>(X);
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int32> toint32(ILInArray< Int16> X) {
            return convert< Int16, Int32>(X);
        }
        /// <summary>
        /// Convert numeric array to Int64 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int64 array</returns>
        /// <remarks><para>The function converts elements of X to Int64 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int64> toint64(ILInArray< Int16 > X) {
             return convert< Int16 ,Int64>(X); 
        }
        /// <summary>
        /// Convert numeric array to double array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>double array</returns>
        /// <remarks><para>The function converts elements of X to double using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<double> todouble(ILInArray< byte > X) {
             return convert< byte ,double>(X); 
        }
        /// <summary>
        /// Convert numeric array to float array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>float array</returns>
        /// <remarks><para>The new array converts elements of X to float using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<float> tosingle(ILInArray< byte > X) {
             return convert< byte ,float>(X); 
        }
        /// <summary>
        /// Convert numeric array to complex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>complex array</returns>
        /// <remarks><para>Real input arrays will be converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to complex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<complex> tocomplex(ILInArray< byte > X) {
             return convert< byte ,complex>(X); 
        }
        /// <summary>
        /// Convert numeric array to fcomplex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>fcomplex array</returns>
        /// <remarks>
        /// <para>Real input arrays are converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to fcomplex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<fcomplex> tofcomplex(ILInArray< byte > X) {
             return convert< byte ,fcomplex>(X); 
        }
        /// <summary>
        /// Convert numeric array to byte array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>byte array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<byte> tobyte(ILInArray< byte > X) {
             return convert< byte ,byte>(X); 
        }
        /// <summary>
        /// Convert numeric array to logical array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Logical array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. Non-zero
        /// elements are converted to true, zero-elements are converted to false. 
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetLogical tological(ILInArray< byte > X) {
             return new ILRetLogical (convert< byte ,byte>(X).Storage as ILDenseStorage<byte>); 
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int16> toint16(ILInArray< byte> X) {
            return convert< byte, Int16>(X);
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int32> toint32(ILInArray< byte> X) {
            return convert< byte, Int32>(X);
        }
        /// <summary>
        /// Convert numeric array to Int64 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int64 array</returns>
        /// <remarks><para>The function converts elements of X to Int64 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int64> toint64(ILInArray< byte > X) {
             return convert< byte ,Int64>(X); 
        }
        /// <summary>
        /// Convert numeric array to double array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>double array</returns>
        /// <remarks><para>The function converts elements of X to double using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<double> todouble(ILInArray< fcomplex > X) {
             return convert< fcomplex ,double>(X); 
        }
        /// <summary>
        /// Convert numeric array to float array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>float array</returns>
        /// <remarks><para>The new array converts elements of X to float using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<float> tosingle(ILInArray< fcomplex > X) {
             return convert< fcomplex ,float>(X); 
        }
        /// <summary>
        /// Convert numeric array to complex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>complex array</returns>
        /// <remarks><para>Real input arrays will be converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to complex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<complex> tocomplex(ILInArray< fcomplex > X) {
             return convert< fcomplex ,complex>(X); 
        }
        /// <summary>
        /// Convert numeric array to fcomplex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>fcomplex array</returns>
        /// <remarks>
        /// <para>Real input arrays are converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to fcomplex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<fcomplex> tofcomplex(ILInArray< fcomplex > X) {
             return convert< fcomplex ,fcomplex>(X); 
        }
        /// <summary>
        /// Convert numeric array to byte array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>byte array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<byte> tobyte(ILInArray< fcomplex > X) {
             return convert< fcomplex ,byte>(X); 
        }
        /// <summary>
        /// Convert numeric array to logical array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Logical array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. Non-zero
        /// elements are converted to true, zero-elements are converted to false. 
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetLogical tological(ILInArray< fcomplex > X) {
             return new ILRetLogical (convert< fcomplex ,byte>(X).Storage as ILDenseStorage<byte>); 
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int16> toint16(ILInArray< fcomplex> X) {
            return convert< fcomplex, Int16>(X);
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int32> toint32(ILInArray< fcomplex> X) {
            return convert< fcomplex, Int32>(X);
        }
        /// <summary>
        /// Convert numeric array to Int64 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int64 array</returns>
        /// <remarks><para>The function converts elements of X to Int64 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int64> toint64(ILInArray< fcomplex > X) {
             return convert< fcomplex ,Int64>(X); 
        }
        /// <summary>
        /// Convert numeric array to double array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>double array</returns>
        /// <remarks><para>The function converts elements of X to double using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<double> todouble(ILInArray< complex > X) {
             return convert< complex ,double>(X); 
        }
        /// <summary>
        /// Convert numeric array to float array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>float array</returns>
        /// <remarks><para>The new array converts elements of X to float using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<float> tosingle(ILInArray< complex > X) {
             return convert< complex ,float>(X); 
        }
        /// <summary>
        /// Convert numeric array to complex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>complex array</returns>
        /// <remarks><para>Real input arrays will be converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to complex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<complex> tocomplex(ILInArray< complex > X) {
             return convert< complex ,complex>(X); 
        }
        /// <summary>
        /// Convert numeric array to fcomplex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>fcomplex array</returns>
        /// <remarks>
        /// <para>Real input arrays are converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to fcomplex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<fcomplex> tofcomplex(ILInArray< complex > X) {
             return convert< complex ,fcomplex>(X); 
        }
        /// <summary>
        /// Convert numeric array to byte array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>byte array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<byte> tobyte(ILInArray< complex > X) {
             return convert< complex ,byte>(X); 
        }
        /// <summary>
        /// Convert numeric array to logical array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Logical array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. Non-zero
        /// elements are converted to true, zero-elements are converted to false. 
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetLogical tological(ILInArray< complex > X) {
             return new ILRetLogical (convert< complex ,byte>(X).Storage as ILDenseStorage<byte>); 
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int16> toint16(ILInArray< complex> X) {
            return convert< complex, Int16>(X);
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int32> toint32(ILInArray< complex> X) {
            return convert< complex, Int32>(X);
        }
        /// <summary>
        /// Convert numeric array to Int64 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int64 array</returns>
        /// <remarks><para>The function converts elements of X to Int64 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int64> toint64(ILInArray< complex > X) {
             return convert< complex ,Int64>(X); 
        }
        /// <summary>
        /// Convert numeric array to double array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>double array</returns>
        /// <remarks><para>The function converts elements of X to double using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<double> todouble(ILInArray< float > X) {
             return convert< float ,double>(X); 
        }
        /// <summary>
        /// Convert numeric array to float array
        /// </summary>
        /// <param name="X">Input array</param>
        /// <returns>float array</returns>
        /// <remarks><para>The new array converts elements of X to float using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<float> tosingle(ILInArray< float > X) {
             return convert< float ,float>(X); 
        }
        /// <summary>
        /// Convert numeric array to complex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>complex array</returns>
        /// <remarks><para>Real input arrays will be converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to complex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<complex> tocomplex(ILInArray< float > X) {
             return convert< float ,complex>(X); 
        }
        /// <summary>
        /// Convert numeric array to fcomplex array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>fcomplex array</returns>
        /// <remarks>
        /// <para>Real input arrays are converted to the real part of the complex array returned.</para>
        /// <para>The function converts elements of X to fcomplex using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<fcomplex> tofcomplex(ILInArray< float > X) {
             return convert< float ,fcomplex>(X); 
        }
        /// <summary>
        /// Convert numeric array to byte array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>byte array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. 
        /// The new array uses new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<byte> tobyte(ILInArray< float > X) {
             return convert< float ,byte>(X); 
        }
        /// <summary>
        /// Convert numeric array to logical array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Logical array</returns>
        /// <remarks><para>The function converts elements of X to byte using standard explicit system conversions. Non-zero
        /// elements are converted to true, zero-elements are converted to false. 
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetLogical tological(ILInArray< float > X) {
             return new ILRetLogical (convert< float ,byte>(X).Storage as ILDenseStorage<byte>); 
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int16> toint16(ILInArray< float> X) {
            return convert< float, Int16>(X);
        }
        /// <summary>
        /// Convert numeric array to Int32 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int32 array</returns>
        /// <remarks><para>The function converts elements of X to Int32 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int32> toint32(ILInArray< float> X) {
            return convert< float, Int32>(X);
        }
        /// <summary>
        /// Convert numeric array to Int64 array
        /// </summary>
        /// <param name="X">Input array </param>
        /// <returns>Int64 array</returns>
        /// <remarks><para>The function converts elements of X to Int64 using standard explicit system conversions.  
        /// The new array will always use new memory, even if the incoming type is the same as the output type.</para></remarks>
        public static ILRetArray<Int64> toint64(ILInArray< float > X) {
             return convert< float ,Int64>(X); 
        }

#endregion HYCALPER AUTO GENERATED CODE


        
        /// <summary>
        /// convert arbitrary numeric array to double array
        /// </summary>
        /// <param name="X">numeric array, one of supported numeric type</param>
        /// <returns>double array</returns>
        /// <remarks>This function enables to convert arbitrary numeric (dense) arrays to a known output array type 
        /// - without knowing the concrete numeric type of the source. Supported element types include: double, 
        /// float, complex, fcomplex, byte, logical, Int32, Int64.
        /// <para>This function will always create new memory for the new array, even if both 
        /// arrays have the same element type.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if elements of X are 
        /// not of any supported numeric type</exception>
        public static ILRetArray< double>  todouble(ILBaseArray X) {
            using (ILScope.Enter(X)) {
                if (X is ILDenseArray<double>)
                    return convert<double, double>((X as ILDenseArray<double>).C);
                else if (X is ILDenseArray<float>)
                    return convert<float, double>((X as ILDenseArray<float>).C);
                else if (X is ILDenseArray<complex>)
                    return convert<complex, double>((X as ILDenseArray<complex>).C);
                else if (X is ILDenseArray<fcomplex>)
                    return convert<fcomplex, double>((X as ILDenseArray<fcomplex>).C);
                else if (X is ILDenseArray<byte>)
                    return convert<byte, double>((X as ILDenseArray<byte>).C);
                else if (X is ILDenseArray<Int32>)
                    return convert<Int32, double>((X as ILDenseArray<Int32>).C);
                else if (X is ILDenseArray<Int64>)
                    return convert<Int64, double>((X as ILDenseArray<Int64>).C);
                else
                    throw new ILArgumentException("input type not supported: " + X.GetType().Name);
            }
        }

#region HYCALPER AUTO GENERATED CODE

       
        /// <summary>convert arbitrary numeric array to Int64 array</summary>
        /// <param name="X">numeric array, one of supported numeric type</param>
        /// <returns>Int64 array</returns>
        /// <remarks>This function enables to convert arbitrary numeric (dense) arrays to a known output array type 
        /// - without knowing the concrete numeric type of the source. Supported element types include: double, 
        /// float, complex, fcomplex, byte, logical, Int32, Int64.
        /// <para>This function will always create new memory for the new array, even if both 
        /// arrays have the same element type.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if elements of X are 
        /// not of any supported numeric type</exception>
        public static ILRetArray< Int64>  toint64(ILBaseArray X) {
            using (ILScope.Enter(X)) {
                if (X is ILDenseArray<double>)
                    return convert<double, Int64>((X as ILDenseArray<double>).C);
                else if (X is ILDenseArray<float>)
                    return convert<float, Int64>((X as ILDenseArray<float>).C);
                else if (X is ILDenseArray<complex>)
                    return convert<complex, Int64>((X as ILDenseArray<complex>).C);
                else if (X is ILDenseArray<fcomplex>)
                    return convert<fcomplex, Int64>((X as ILDenseArray<fcomplex>).C);
                else if (X is ILDenseArray<byte>)
                    return convert<byte, Int64>((X as ILDenseArray<byte>).C);
                else if (X is ILDenseArray<Int32>)
                    return convert<Int32, Int64>((X as ILDenseArray<Int32>).C);
                else if (X is ILDenseArray<Int64>)
                    return convert<Int64, Int64>((X as ILDenseArray<Int64>).C);
                else
                    throw new ILArgumentException("input type not supported: " + X.GetType().Name);
            }
        }
       
        /// <summary>convert arbitrary numeric array to Int32 array</summary>
        /// <param name="X">numeric array, one of supported numeric type</param>
        /// <returns>Int32 array</returns>
        /// <remarks>This function enables to convert arbitrary numeric (dense) arrays to a known output array type 
        /// - without knowing the concrete numeric type of the source. Supported element types include: double, 
        /// float, complex, fcomplex, byte, logical, Int32, Int64.
        /// <para>This function will always create new memory for the new array, even if both 
        /// arrays have the same element type.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if elements of X are 
        /// not of any supported numeric type</exception>
        public static ILRetArray< Int32>  toint32(ILBaseArray X) {
            using (ILScope.Enter(X)) {
                if (X is ILDenseArray<double>)
                    return convert<double, Int32>((X as ILDenseArray<double>).C);
                else if (X is ILDenseArray<float>)
                    return convert<float, Int32>((X as ILDenseArray<float>).C);
                else if (X is ILDenseArray<complex>)
                    return convert<complex, Int32>((X as ILDenseArray<complex>).C);
                else if (X is ILDenseArray<fcomplex>)
                    return convert<fcomplex, Int32>((X as ILDenseArray<fcomplex>).C);
                else if (X is ILDenseArray<byte>)
                    return convert<byte, Int32>((X as ILDenseArray<byte>).C);
                else if (X is ILDenseArray<Int32>)
                    return convert<Int32, Int32>((X as ILDenseArray<Int32>).C);
                else if (X is ILDenseArray<Int64>)
                    return convert<Int64, Int32>((X as ILDenseArray<Int64>).C);
                else
                    throw new ILArgumentException("input type not supported: " + X.GetType().Name);
            }
        }
       
        /// <summary>convert arbitrary numeric array to Int16 array</summary>
        /// <param name="X">numeric array, one of supported numeric type</param>
        /// <returns>Int16 array</returns>
        /// <remarks>This function enables to convert arbitrary numeric (dense) arrays to a known output array type 
        /// - without knowing the concrete numeric type of the source. Supported element types include: double, 
        /// float, complex, fcomplex, byte, logical, Int32, Int64.
        /// <para>This function will always create new memory for the new array, even if both 
        /// arrays have the same element type.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if elements of X are 
        /// not of any supported numeric type</exception>
        public static ILRetArray< Int16>  toint16(ILBaseArray X) {
            using (ILScope.Enter(X)) {
                if (X is ILDenseArray<double>)
                    return convert<double, Int16>((X as ILDenseArray<double>).C);
                else if (X is ILDenseArray<float>)
                    return convert<float, Int16>((X as ILDenseArray<float>).C);
                else if (X is ILDenseArray<complex>)
                    return convert<complex, Int16>((X as ILDenseArray<complex>).C);
                else if (X is ILDenseArray<fcomplex>)
                    return convert<fcomplex, Int16>((X as ILDenseArray<fcomplex>).C);
                else if (X is ILDenseArray<byte>)
                    return convert<byte, Int16>((X as ILDenseArray<byte>).C);
                else if (X is ILDenseArray<Int32>)
                    return convert<Int32, Int16>((X as ILDenseArray<Int32>).C);
                else if (X is ILDenseArray<Int64>)
                    return convert<Int64, Int16>((X as ILDenseArray<Int64>).C);
                else
                    throw new ILArgumentException("input type not supported: " + X.GetType().Name);
            }
        }
       
        /// <summary>convert arbitrary numeric array to byte array</summary>
        /// <param name="X">numeric array, one of supported numeric type</param>
        /// <returns>byte array</returns>
        /// <remarks>This function enables to convert arbitrary numeric (dense) arrays to a known output array type 
        /// - without knowing the concrete numeric type of the source. Supported element types include: double, 
        /// float, complex, fcomplex, byte, logical, Int32, Int64.
        /// <para>This function will always create new memory for the new array, even if both 
        /// arrays have the same element type.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if elements of X are 
        /// not of any supported numeric type</exception>
        public static ILRetArray< byte>  tobyte(ILBaseArray X) {
            using (ILScope.Enter(X)) {
                if (X is ILDenseArray<double>)
                    return convert<double, byte>((X as ILDenseArray<double>).C);
                else if (X is ILDenseArray<float>)
                    return convert<float, byte>((X as ILDenseArray<float>).C);
                else if (X is ILDenseArray<complex>)
                    return convert<complex, byte>((X as ILDenseArray<complex>).C);
                else if (X is ILDenseArray<fcomplex>)
                    return convert<fcomplex, byte>((X as ILDenseArray<fcomplex>).C);
                else if (X is ILDenseArray<byte>)
                    return convert<byte, byte>((X as ILDenseArray<byte>).C);
                else if (X is ILDenseArray<Int32>)
                    return convert<Int32, byte>((X as ILDenseArray<Int32>).C);
                else if (X is ILDenseArray<Int64>)
                    return convert<Int64, byte>((X as ILDenseArray<Int64>).C);
                else
                    throw new ILArgumentException("input type not supported: " + X.GetType().Name);
            }
        }
       
        /// <summary>convert arbitrary numeric array to fcomplex array</summary>
        /// <param name="X">numeric array, one of supported numeric type</param>
        /// <returns>fcomplex array</returns>
        /// <remarks>This function enables to convert arbitrary numeric (dense) arrays to a known output array type 
        /// - without knowing the concrete numeric type of the source. Supported element types include: double, 
        /// float, complex, fcomplex, byte, logical, Int32, Int64.
        /// <para>This function will always create new memory for the new array, even if both 
        /// arrays have the same element type.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if elements of X are 
        /// not of any supported numeric type</exception>
        public static ILRetArray< fcomplex>  tofcomplex(ILBaseArray X) {
            using (ILScope.Enter(X)) {
                if (X is ILDenseArray<double>)
                    return convert<double, fcomplex>((X as ILDenseArray<double>).C);
                else if (X is ILDenseArray<float>)
                    return convert<float, fcomplex>((X as ILDenseArray<float>).C);
                else if (X is ILDenseArray<complex>)
                    return convert<complex, fcomplex>((X as ILDenseArray<complex>).C);
                else if (X is ILDenseArray<fcomplex>)
                    return convert<fcomplex, fcomplex>((X as ILDenseArray<fcomplex>).C);
                else if (X is ILDenseArray<byte>)
                    return convert<byte, fcomplex>((X as ILDenseArray<byte>).C);
                else if (X is ILDenseArray<Int32>)
                    return convert<Int32, fcomplex>((X as ILDenseArray<Int32>).C);
                else if (X is ILDenseArray<Int64>)
                    return convert<Int64, fcomplex>((X as ILDenseArray<Int64>).C);
                else
                    throw new ILArgumentException("input type not supported: " + X.GetType().Name);
            }
        }
       
        /// <summary>convert arbitrary numeric array to complex array</summary>
        /// <param name="X">numeric array, one of supported numeric type</param>
        /// <returns>complex array</returns>
        /// <remarks>This function enables to convert arbitrary numeric (dense) arrays to a known output array type 
        /// - without knowing the concrete numeric type of the source. Supported element types include: double, 
        /// float, complex, fcomplex, byte, logical, Int32, Int64.
        /// <para>This function will always create new memory for the new array, even if both 
        /// arrays have the same element type.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if elements of X are 
        /// not of any supported numeric type</exception>
        public static ILRetArray< complex>  tocomplex(ILBaseArray X) {
            using (ILScope.Enter(X)) {
                if (X is ILDenseArray<double>)
                    return convert<double, complex>((X as ILDenseArray<double>).C);
                else if (X is ILDenseArray<float>)
                    return convert<float, complex>((X as ILDenseArray<float>).C);
                else if (X is ILDenseArray<complex>)
                    return convert<complex, complex>((X as ILDenseArray<complex>).C);
                else if (X is ILDenseArray<fcomplex>)
                    return convert<fcomplex, complex>((X as ILDenseArray<fcomplex>).C);
                else if (X is ILDenseArray<byte>)
                    return convert<byte, complex>((X as ILDenseArray<byte>).C);
                else if (X is ILDenseArray<Int32>)
                    return convert<Int32, complex>((X as ILDenseArray<Int32>).C);
                else if (X is ILDenseArray<Int64>)
                    return convert<Int64, complex>((X as ILDenseArray<Int64>).C);
                else
                    throw new ILArgumentException("input type not supported: " + X.GetType().Name);
            }
        }
       
        /// <summary>convert arbitrary numeric array to float array</summary>
        /// <param name="X">numeric array, one of supported numeric type</param>
        /// <returns>float array</returns>
        /// <remarks>This function enables to convert arbitrary numeric (dense) arrays to a known output array type 
        /// - without knowing the concrete numeric type of the source. Supported element types include: double, 
        /// float, complex, fcomplex, byte, logical, Int32, Int64.
        /// <para>This function will always create new memory for the new array, even if both 
        /// arrays have the same element type.</para>
        /// </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">if elements of X are 
        /// not of any supported numeric type</exception>
        public static ILRetArray< float>  tosingle(ILBaseArray X) {
            using (ILScope.Enter(X)) {
                if (X is ILDenseArray<double>)
                    return convert<double, float>((X as ILDenseArray<double>).C);
                else if (X is ILDenseArray<float>)
                    return convert<float, float>((X as ILDenseArray<float>).C);
                else if (X is ILDenseArray<complex>)
                    return convert<complex, float>((X as ILDenseArray<complex>).C);
                else if (X is ILDenseArray<fcomplex>)
                    return convert<fcomplex, float>((X as ILDenseArray<fcomplex>).C);
                else if (X is ILDenseArray<byte>)
                    return convert<byte, float>((X as ILDenseArray<byte>).C);
                else if (X is ILDenseArray<Int32>)
                    return convert<Int32, float>((X as ILDenseArray<Int32>).C);
                else if (X is ILDenseArray<Int64>)
                    return convert<Int64, float>((X as ILDenseArray<Int64>).C);
                else
                    throw new ILArgumentException("input type not supported: " + X.GetType().Name);
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }

}
