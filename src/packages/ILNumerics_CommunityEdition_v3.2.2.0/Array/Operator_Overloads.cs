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

    public abstract partial class ILDenseArray<ElementType> {

        /// <summary>
        /// Elementwise subtraction operator
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Result of subtraction</returns>
        /// <remarks>The operator is defined for both arrays of the same numeric 'ElementType' (double, float, complex,
        /// fcomplex, int, long, byte).</remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the array elements are of an unsupported type</exception>
        public static ILRetArray<ElementType> operator -(ILDenseArray<ElementType> A, ILDenseArray<ElementType> B) {
            using (ILScope.Enter(A,B)) 
            // TODO: check if (A.GetType() == typeof(ILDenseArray<double>)) is faster due to JIT optimizations
            if (A is ILDenseArray< double>) {
                return (ILRetArray<ElementType>)(object)ILMath.subtract((A as ILDenseArray<double>).C, (B as ILDenseArray<double>).C);
            } else if (A is  ILDenseArray<float>) {
                return (ILRetArray<ElementType>)(object)ILMath.subtract((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C);
            } else if (A is  ILDenseArray<complex>) {
                return (ILRetArray<ElementType>)(object)ILMath.subtract((A as ILDenseArray<complex>).C, (B as ILDenseArray<complex>).C);
            } else if (A is  ILDenseArray<fcomplex>) {
                return (ILRetArray<ElementType>)(object)ILMath.subtract((A as ILDenseArray<fcomplex>).C, (B as ILDenseArray<fcomplex>).C);
            } else if (A is  ILDenseArray<int>) {
                return (ILRetArray<ElementType>)(object)ILMath.subtract((A as ILDenseArray<int>).C, (B as ILDenseArray<int>).C);
            } else if (A is  ILDenseArray<long>) {
                return (ILRetArray<ElementType>)(object)ILMath.subtract((A as ILDenseArray<long>).C, (B as ILDenseArray<long>).C);
            } else if (A is  ILDenseArray<byte>) {
                return (ILRetArray<ElementType>)(object)ILMath.subtract((A as ILDenseArray<byte>).C, (B as ILDenseArray<byte>).C);
            } else {
                throw new ILArgumentException("Operator - not defined for arrays of element type '" + typeof(ElementType).Name + "'");
            }
        }

        /// <summary>
        /// Elementwise addition operator
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Result of addition</returns>
        /// <remarks>The operator is defined for both arrays of the same numeric 'ElementType' (double, float, complex,
        /// fcomplex, int, long, byte).</remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the array elements are of an unsupported type</exception>
        public static ILRetArray<ElementType> operator +(ILDenseArray<ElementType> A, ILDenseArray<ElementType> B) {
            using (ILScope.Enter(A, B))
                if (A is ILDenseArray< double>) {
                return (ILRetArray<ElementType>)(object)ILMath.add((A as ILDenseArray<double>).C, (B as ILDenseArray<double>).C);
            } else if (A is  ILDenseArray<float>) {
                return (ILRetArray<ElementType>)(object)ILMath.add((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C);
            } else if (A is  ILDenseArray<complex>) {
                return (ILRetArray<ElementType>)(object)ILMath.add((A as ILDenseArray<complex>).C, (B as ILDenseArray<complex>).C);
            } else if (A is  ILDenseArray<fcomplex>) {
                return (ILRetArray<ElementType>)(object)ILMath.add((A as ILDenseArray<fcomplex>).C, (B as ILDenseArray<fcomplex>).C);
            } else if (A is  ILDenseArray<int>) {
                return (ILRetArray<ElementType>)(object)ILMath.add((A as ILDenseArray<int>).C, (B as ILDenseArray<int>).C);
            } else if (A is  ILDenseArray<long>) {
                return (ILRetArray<ElementType>)(object)ILMath.add((A as ILDenseArray<long>).C, (B as ILDenseArray<long>).C);
            } else if (A is  ILDenseArray<byte>) {
                return (ILRetArray<ElementType>)(object)ILMath.add((A as ILDenseArray<byte>).C, (B as ILDenseArray<byte>).C);
            } else {
                throw new ILArgumentException("Operator + not defined for arrays of element type '" + typeof(ElementType).Name + "'");
            }
        }

        /// <summary>
        /// Elementwise multiplication operator
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Result of multiplication</returns>
        /// <remarks>The operator is defined for both arrays of the same numeric 'ElementType' (double, float, complex,
        /// fcomplex, int, long, byte).</remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the array elements are of an unsupported type</exception>
        public static ILRetArray<ElementType> operator *(ILDenseArray<ElementType> A, ILDenseArray<ElementType> B) {
            using (ILScope.Enter(A, B))
                if (A is ILDenseArray< double>) {
                return (ILRetArray<ElementType>)(object)ILMath.multiplyElem((A as ILDenseArray<double>).C, (B as ILDenseArray<double>).C);
            } else if (A is  ILDenseArray<float>) {
                return (ILRetArray<ElementType>)(object)ILMath.multiplyElem((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C);
            } else if (A is  ILDenseArray<complex>) {
                return (ILRetArray<ElementType>)(object)ILMath.multiplyElem((A as ILDenseArray<complex>).C, (B as ILDenseArray<complex>).C);
            } else if (A is  ILDenseArray<fcomplex>) {
                return (ILRetArray<ElementType>)(object)ILMath.multiplyElem((A as ILDenseArray<fcomplex>).C, (B as ILDenseArray<fcomplex>).C);
            } else if (A is  ILDenseArray<int>) {
                return (ILRetArray<ElementType>)(object)ILMath.multiplyElem((A as ILDenseArray<int>).C, (B as ILDenseArray<int>).C);
            } else if (A is  ILDenseArray<long>) {
                return (ILRetArray<ElementType>)(object)ILMath.multiplyElem((A as ILDenseArray<long>).C, (B as ILDenseArray<long>).C);
            } else if (A is  ILDenseArray<byte>) {
                return (ILRetArray<ElementType>)(object)ILMath.multiplyElem((A as ILDenseArray<byte>).C, (B as ILDenseArray<byte>).C);
            } else {
                throw new ILArgumentException("Operator * not defined for arrays of element type '" + typeof(ElementType).Name + "'");
            }
        }

        ///// <summary>
        ///// Elementwise exponentiation operator   ----------- CAUTION! THE ^ OPERATOR HAS LOWER PRECEEDENCE AS THE *,+,-... OPERATORS! 
        ///// </summary>
        ///// <param name="A">Bases</param>
        ///// <param name="B">Exponents</param>
        ///// <returns>Result of exponentiation of base elements A and exponents in B</returns>
        ///// <remarks>The operator is defined for both arrays of the same numeric 'ElementType' (double, float, complex,
        ///// fcomplex, int, long, byte).</remarks>
        ///// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the array elements are of an unsupported type</exception>
        //public static ILRetArray<ElementType> operator ^(ILDenseArray<ElementType> A, ILDenseArray<ElementType> B) {
        //    if (A is ILDenseArray< double >) {
        //        return (ILRetArray<ElementType>)(object)ILMath.pow((A as ILDenseArray<double>).C, (B as ILDenseArray<double>).C); 
        //    } else if (A is  ILDenseArray<float>) {
        //        return (ILRetArray<ElementType>)(object)ILMath.pow((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C); 
        //    } else if (A is  ILDenseArray<complex>) {
        //        return (ILRetArray<ElementType>)(object)ILMath.pow((A as ILDenseArray<complex>).C, (B as ILDenseArray<complex>).C); 
        //    } else if (A is  ILDenseArray<fcomplex>) {
        //        return (ILRetArray<ElementType>)(object)ILMath.pow((A as ILDenseArray<fcomplex>).C, (B as ILDenseArray<fcomplex>).C); 
        //    } else if (A is  ILDenseArray<int>) {
        //        return (ILRetArray<ElementType>)(object)ILMath.pow((A as ILDenseArray<int>).C, (B as ILDenseArray<int>).C); 
        //    } else if (A is  ILDenseArray<long>) {
        //        return (ILRetArray<ElementType>)(object)ILMath.pow((A as ILDenseArray<long>).C, (B as ILDenseArray<long>).C); 
        //    } else if (A is  ILDenseArray<byte>) {
        //        return (ILRetArray<ElementType>)(object)ILMath.pow((A as ILDenseArray<byte>).C, (B as ILDenseArray<byte>).C); 
        //    } else  {
        //        throw new ILArgumentException("Operator * not defined for "+ A.GetType().Name + " and " + B.GetType().Name);
        //    }
        //}

        /// <summary>
        /// Elementwise division operator
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Result of division</returns>
        /// <remarks>The operator is defined for both arrays of the same numeric 'ElementType' (double, float, complex,
        /// fcomplex, int, long, byte).</remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the array elements are of an unsupported type</exception>
        public static ILRetArray<ElementType> operator /(ILDenseArray<ElementType> A, ILDenseArray<ElementType> B) {
            using (ILScope.Enter(A, B))
                if (A is ILDenseArray< double>) {
                    return (ILRetArray<ElementType>)(object)ILMath.divide((A as ILDenseArray<double>).C, (B as ILDenseArray<double>).C);
                } else if (A is  ILDenseArray<float>) {
                    return (ILRetArray<ElementType>)(object)ILMath.divide((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C);
                } else if (A is  ILDenseArray<complex>) {
                    return (ILRetArray<ElementType>)(object)ILMath.divide((A as ILDenseArray<complex>).C, (B as ILDenseArray<complex>).C);
                } else if (A is  ILDenseArray<fcomplex>) {
                    return (ILRetArray<ElementType>)(object)ILMath.divide((A as ILDenseArray<fcomplex>).C, (B as ILDenseArray<fcomplex>).C);
                } else if (A is  ILDenseArray<int>) {
                    return (ILRetArray<ElementType>)(object)ILMath.divide((A as ILDenseArray<int>).C, (B as ILDenseArray<int>).C);
                } else if (A is  ILDenseArray<long>) {
                    return (ILRetArray<ElementType>)(object)ILMath.divide((A as ILDenseArray<long>).C, (B as ILDenseArray<long>).C);
                } else if (A is  ILDenseArray<byte>) {
                    return (ILRetArray<ElementType>)(object)ILMath.divide((A as ILDenseArray<byte>).C, (B as ILDenseArray<byte>).C);
                } else {
                    throw new ILArgumentException("Operator / not defined for arrays of element type '" + typeof(ElementType).Name + "'");
                }
        }

        /// <summary>
        /// Elementwise modulus operator
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Result of modulus operation</returns>
        /// <remarks>The operator is defined for 2 arrays of the same numeric real element types: double, float, int, long, byte.</remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the array elements are of an unsupported type</exception>
        public static ILRetArray<ElementType> operator %(ILDenseArray<ElementType> A, ILDenseArray<ElementType> B) {
            using (ILScope.Enter(A, B))
                if (A is ILDenseArray< double>) {
                    return (ILRetArray<ElementType>)(object)ILMath.mod((A as ILDenseArray<double>).C, (B as ILDenseArray<double>).C);
                } else if (A is  ILDenseArray<float>) {
                    return (ILRetArray<ElementType>)(object)ILMath.mod((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C);
                } else if (A is  ILDenseArray<int>) {
                    return (ILRetArray<ElementType>)(object)ILMath.mod((A as ILDenseArray<int>).C, (B as ILDenseArray<int>).C);
                } else if (A is  ILDenseArray<long>) {
                    return (ILRetArray<ElementType>)(object)ILMath.mod((A as ILDenseArray<long>).C, (B as ILDenseArray<long>).C);
                } else if (A is  ILDenseArray<byte>) {
                    return (ILRetArray<ElementType>)(object)ILMath.mod((A as ILDenseArray<byte>).C, (B as ILDenseArray<byte>).C);
                } else {
                    throw new ILArgumentException("Operator % not defined for arrays of element type '" + typeof(ElementType).Name + "'");
                }
        }

        /// <summary>
        /// 'Equalty' operator of two arrays
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Logical array of same size than A and B, result of operation along all elements</returns>
        /// <remarks>Sizes and types of A and B must match.</remarks>
        public static ILRetLogical operator ==(ILDenseArray<ElementType> A, ILDenseArray<ElementType> B) {
            using (ILScope.Enter(A, B)) {
                if (object.Equals(A, null)) {
                    if (object.Equals(B, null)) {
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    if (object.Equals(B, null)) {
                        return false;
                    }
                }
                if (false) {

                } else if (A is ILDenseArray< double> && B is ILDenseArray< double>) {
                    return ILMath.eq((A as ILDenseArray< double>).C, (B as ILDenseArray< double>).C);

                    #region HYCALPER AUTO GENERATED CODE

                } else if (A is ILDenseArray<Int64> && B is ILDenseArray<Int64>) {
                    return ILMath.eq((A as ILDenseArray<Int64>).C, (B as ILDenseArray<Int64>).C);
                } else if (A is ILDenseArray<Int32> && B is ILDenseArray<Int32>) {
                    return ILMath.eq((A as ILDenseArray<Int32>).C, (B as ILDenseArray<Int32>).C);
                } else if (A is ILDenseArray<float> && B is ILDenseArray<float>) {
                    return ILMath.eq((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C);
                } else if (A is ILDenseArray<fcomplex> && B is ILDenseArray<fcomplex>) {
                    return ILMath.eq((A as ILDenseArray<fcomplex>).C, (B as ILDenseArray<fcomplex>).C);
                } else if (A is ILDenseArray<complex> && B is ILDenseArray<complex>) {
                    return ILMath.eq((A as ILDenseArray<complex>).C, (B as ILDenseArray<complex>).C);
                } else if (A is ILDenseArray<byte> && B is ILDenseArray<byte>) {
                    return ILMath.eq((A as ILDenseArray<byte>).C, (B as ILDenseArray<byte>).C);

                    #endregion HYCALPER AUTO GENERATED CODE
                } else if (A is ILDenseArray<String> && B is ILDenseArray<String>) {
                    return ILMath.eq((A as ILDenseArray<String>).C, (B as ILDenseArray<String>).C);
                } else {
                    throw new ILArgumentTypeException("Operator '==' not defined for arrays of element type '" + typeof(ElementType).Name + "'");
                }
            }
        }

        /// <summary>
        /// 'unequalty' operator of two arrays
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Logical array of same size than A and B, result of operation along all elements</returns>
        /// <remarks>Sizes and types of A and B must match.</remarks>
        public static ILRetLogical operator !=(ILDenseArray<ElementType> A, ILDenseArray<ElementType> B) {
            using (ILScope.Enter(A, B)) {
                if (object.Equals(A, null)) {
                    if (object.Equals(B, null)) {
                        return false;
                    } else {
                        return true;
                    }
                } else {
                    if (object.Equals(B, null)) {
                        return true;
                    }
                }
                if (false) {

                } else if (A is ILDenseArray< double> && B is ILDenseArray< double>) {
                    return ILMath.neq((A as ILDenseArray< double>).C, (B as ILDenseArray< double>).C);

                    #region HYCALPER AUTO GENERATED CODE

                } else if (A is ILDenseArray<Int64> && B is ILDenseArray<Int64>) {
                    return ILMath.neq((A as ILDenseArray<Int64>).C, (B as ILDenseArray<Int64>).C);
                } else if (A is ILDenseArray<Int32> && B is ILDenseArray<Int32>) {
                    return ILMath.neq((A as ILDenseArray<Int32>).C, (B as ILDenseArray<Int32>).C);
                } else if (A is ILDenseArray<float> && B is ILDenseArray<float>) {
                    return ILMath.neq((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C);
                } else if (A is ILDenseArray<fcomplex> && B is ILDenseArray<fcomplex>) {
                    return ILMath.neq((A as ILDenseArray<fcomplex>).C, (B as ILDenseArray<fcomplex>).C);
                } else if (A is ILDenseArray<complex> && B is ILDenseArray<complex>) {
                    return ILMath.neq((A as ILDenseArray<complex>).C, (B as ILDenseArray<complex>).C);
                } else if (A is ILDenseArray<byte> && B is ILDenseArray<byte>) {
                    return ILMath.neq((A as ILDenseArray<byte>).C, (B as ILDenseArray<byte>).C);

                    #endregion HYCALPER AUTO GENERATED CODE
                } else if (A is ILDenseArray<String> && B is ILDenseArray<String>) {
                    return ILMath.neq((A as ILDenseArray<String>).C, (B as ILDenseArray<String>).C);
                } else {
                    throw new ILArgumentTypeException("Operator '!=' not defined for arrays of element type '" + typeof(ElementType).Name + "'");
                }
            }
        }

        /// <summary>
        /// 'greater or equal' operator of two arrays
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Logical array of same size than A and B, result of operation along all elements</returns>
        /// <remarks>Sizes and types of A and B must match.</remarks>
        public static ILRetLogical operator >=(ILDenseArray<ElementType> A, ILDenseArray<ElementType> B) {
            using (ILScope.Enter(A, B)) {
                if (false) {

                } else if (A is ILDenseArray< double> && B is ILDenseArray< double>) {
                    return ILMath.ge((A as ILDenseArray< double>).C, (B as ILDenseArray< double>).C);

                    #region HYCALPER AUTO GENERATED CODE

                } else if (A is ILDenseArray<Int64> && B is ILDenseArray<Int64>) {
                    return ILMath.ge((A as ILDenseArray<Int64>).C, (B as ILDenseArray<Int64>).C);
                } else if (A is ILDenseArray<Int32> && B is ILDenseArray<Int32>) {
                    return ILMath.ge((A as ILDenseArray<Int32>).C, (B as ILDenseArray<Int32>).C);
                } else if (A is ILDenseArray<float> && B is ILDenseArray<float>) {
                    return ILMath.ge((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C);
                } else if (A is ILDenseArray<fcomplex> && B is ILDenseArray<fcomplex>) {
                    return ILMath.ge((A as ILDenseArray<fcomplex>).C, (B as ILDenseArray<fcomplex>).C);
                } else if (A is ILDenseArray<complex> && B is ILDenseArray<complex>) {
                    return ILMath.ge((A as ILDenseArray<complex>).C, (B as ILDenseArray<complex>).C);
                } else if (A is ILDenseArray<byte> && B is ILDenseArray<byte>) {
                    return ILMath.ge((A as ILDenseArray<byte>).C, (B as ILDenseArray<byte>).C);

                    #endregion HYCALPER AUTO GENERATED CODE
                } else {
                    throw new ILArgumentTypeException("Operator '>=' not defined for arrays of element type '" + typeof(ElementType).Name + "'");
                }
            }
        }

        /// <summary>
        /// 'smaller or equal' operator of two arrays
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Logical array of same size than A and B, result of operation along all elements</returns>
        /// <remarks>Sizes and types of A and B must match.</remarks>
        public static ILRetLogical operator <=(ILDenseArray<ElementType> A, ILDenseArray<ElementType> B) {
            using (ILScope.Enter(A, B)) {
                if (false) {

                } else if (A is ILDenseArray< double> && B is ILDenseArray< double>) {
                    return ILMath.le((A as ILDenseArray< double>).C, (B as ILDenseArray< double>).C);

                    #region HYCALPER AUTO GENERATED CODE

                } else if (A is ILDenseArray<Int64> && B is ILDenseArray<Int64>) {
                    return ILMath.le((A as ILDenseArray<Int64>).C, (B as ILDenseArray<Int64>).C);
                } else if (A is ILDenseArray<Int32> && B is ILDenseArray<Int32>) {
                    return ILMath.le((A as ILDenseArray<Int32>).C, (B as ILDenseArray<Int32>).C);
                } else if (A is ILDenseArray<float> && B is ILDenseArray<float>) {
                    return ILMath.le((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C);
                } else if (A is ILDenseArray<fcomplex> && B is ILDenseArray<fcomplex>) {
                    return ILMath.le((A as ILDenseArray<fcomplex>).C, (B as ILDenseArray<fcomplex>).C);
                } else if (A is ILDenseArray<complex> && B is ILDenseArray<complex>) {
                    return ILMath.le((A as ILDenseArray<complex>).C, (B as ILDenseArray<complex>).C);
                } else if (A is ILDenseArray<byte> && B is ILDenseArray<byte>) {
                    return ILMath.le((A as ILDenseArray<byte>).C, (B as ILDenseArray<byte>).C);

                    #endregion HYCALPER AUTO GENERATED CODE
                } else {
                    throw new ILArgumentTypeException("Operator '<=' not defined for arrays of element type '" + typeof(ElementType).Name + "'");
                }
            }
        }

        /// <summary>
        /// 'greater' operator of two arrays
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Logical array of same size than A and B, result of operation along all elements</returns>
        /// <remarks>Dimension and types of A and B must match.</remarks>
        public static ILRetLogical operator >(ILDenseArray<ElementType> A, ILDenseArray<ElementType> B) {
            using (ILScope.Enter(A, B)) {
                if (false) {

                } else if (A is ILDenseArray< double> && B is ILDenseArray< double>) {
                    return ILMath.gt((A as ILDenseArray< double>).C, (B as ILDenseArray< double>).C);

                    #region HYCALPER AUTO GENERATED CODE

                } else if (A is ILDenseArray<Int64> && B is ILDenseArray<Int64>) {
                    return ILMath.gt((A as ILDenseArray<Int64>).C, (B as ILDenseArray<Int64>).C);
                } else if (A is ILDenseArray<Int32> && B is ILDenseArray<Int32>) {
                    return ILMath.gt((A as ILDenseArray<Int32>).C, (B as ILDenseArray<Int32>).C);
                } else if (A is ILDenseArray<float> && B is ILDenseArray<float>) {
                    return ILMath.gt((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C);
                } else if (A is ILDenseArray<fcomplex> && B is ILDenseArray<fcomplex>) {
                    return ILMath.gt((A as ILDenseArray<fcomplex>).C, (B as ILDenseArray<fcomplex>).C);
                } else if (A is ILDenseArray<complex> && B is ILDenseArray<complex>) {
                    return ILMath.gt((A as ILDenseArray<complex>).C, (B as ILDenseArray<complex>).C);
                } else if (A is ILDenseArray<byte> && B is ILDenseArray<byte>) {
                    return ILMath.gt((A as ILDenseArray<byte>).C, (B as ILDenseArray<byte>).C);

                    #endregion HYCALPER AUTO GENERATED CODE
                } else {
                    throw new ILArgumentTypeException("Operator '>' not defined for arrays of element type '" + typeof(ElementType).Name + "'");
                }
            }
        }
        /// <summary>
        /// 'smaller' operator of two arrays
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Logical array of same size than A and B, result of operation along all elements</returns>
        /// <remarks>Dimension and types of A and B must match.</remarks>
        public static ILRetLogical operator <(ILDenseArray<ElementType> A, ILDenseArray<ElementType> B) {
            using (ILScope.Enter(A, B)) {
                if (false) {

                } else if (A is ILDenseArray< double> && B is ILDenseArray< double>) {
                    return ILMath.lt((A as ILDenseArray< double>).C, (B as ILDenseArray< double>).C);

                    #region HYCALPER AUTO GENERATED CODE

                } else if (A is ILDenseArray<Int64> && B is ILDenseArray<Int64>) {
                    return ILMath.lt((A as ILDenseArray<Int64>).C, (B as ILDenseArray<Int64>).C);
                } else if (A is ILDenseArray<Int32> && B is ILDenseArray<Int32>) {
                    return ILMath.lt((A as ILDenseArray<Int32>).C, (B as ILDenseArray<Int32>).C);
                } else if (A is ILDenseArray<float> && B is ILDenseArray<float>) {
                    return ILMath.lt((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C);
                } else if (A is ILDenseArray<fcomplex> && B is ILDenseArray<fcomplex>) {
                    return ILMath.lt((A as ILDenseArray<fcomplex>).C, (B as ILDenseArray<fcomplex>).C);
                } else if (A is ILDenseArray<complex> && B is ILDenseArray<complex>) {
                    return ILMath.lt((A as ILDenseArray<complex>).C, (B as ILDenseArray<complex>).C);
                } else if (A is ILDenseArray<byte> && B is ILDenseArray<byte>) {
                    return ILMath.lt((A as ILDenseArray<byte>).C, (B as ILDenseArray<byte>).C);

                    #endregion HYCALPER AUTO GENERATED CODE
                } else {
                    throw new ILArgumentTypeException("Operator '<' not defined for arrays of element type '" + typeof(ElementType).Name + "'");
                }
            }
        }

        /// <summary>
        /// Explicitly convert scalar array to System.Value type (ElementType)
        /// </summary>
        /// <param name="val">Array of arbitrary type and size of 1x1</param>
        /// <returns>Single value of type ElementType and of scalar size (1x1)</returns>
        /// <exception cref="ILNumerics.Exceptions.ILCastException">If input array is not scalar</exception>
        /// <exception cref="System.NullReferenceException">If val is null</exception>
        public static explicit operator ElementType(ILDenseArray<ElementType> val) {
            using (ILScope.Enter(val)) {
                if (!val.IsScalar)
                    throw new ILCastException("unable to convert array to scalar");
                return val.GetValue(0);
            }
        }

        /// <summary>
        /// Implicitly convert scalar to array of size 1x1 (scalar).
        /// </summary>
        /// <param name="val">Single element of ElementType type</param>
        /// <returns>New array of type ILRetArray<![CDATA[<ElementType>]]> of size 1x1 
        /// holding the only element with value of val.
        /// </returns>
        public static implicit operator ILDenseArray<ElementType>(ElementType val) {
            ILArray<ElementType> ret = new ILArray<ElementType>(new ILDenseStorage<ElementType>(ILSize.Scalar1_1));
            ret.SetValue(val, 0, 0);
            return ret.C;
        }

        /// <summary>
        /// Negate elements of array - if applicable
        /// </summary>
        /// <param name="A">Input array</param>
        /// <returns>New solid arrray having the elements of A negated</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If elements of the array are not of 
        /// any supported numeric type</exception>
        public static ILRetArray<ElementType> operator -(ILDenseArray<ElementType> A) {
            using (ILScope.Enter(A)) {
                ILArray<ElementType> ret = ILMath.empty<ElementType>();
                if (A is ILDenseArray<double>) {
                    ILMath.invert((A as ILDenseArray<double>).C, (ret as ILArray<double>));
                } else if (A is ILDenseArray<float>) {
                    ILMath.invert((A as ILDenseArray<float>).C, (ret as ILArray<float>));
                } else if (A is ILDenseArray<complex>) {
                    ILMath.invert((A as ILDenseArray<complex>).C, (ret as ILArray<complex>));
                } else if (A is ILDenseArray<fcomplex>) {
                    ILMath.invert((A as ILDenseArray<fcomplex>).C, (ret as ILArray<fcomplex>));
                } else if (A is ILDenseArray<Int32>) {
                    ILMath.invert((A as ILDenseArray<Int32>).C, (ret as ILArray<Int32>));
                } else if (A is ILDenseArray<Int64>) {
                    ILMath.invert((A as ILDenseArray<Int64>).C, (ret as ILArray<Int64>));
                } else if (A is ILDenseArray<byte>) {
                    ILMath.invert((A as ILDenseArray<byte>).C, (ret as ILArray<byte>));
                } else {
                    throw new ILArgumentException("unary - operator not defined for arrays of element type '" + typeof(ElementType).Name + "'");
                }
                return ret.C;
            }
        }

    }
}
