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



        /// <summary>
        /// Compares elements of two arrays, assume NaN and +/- infinity values are comparable and equal
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="B">Input array</param>
        /// <returns>True, if all elements of both arrays are equal; false if any elements or the sizes are not equal</returns>
        public static bool isequalwithequalnans(ILInArray</*HC:inArr1*/double> A, ILInArray</*HC:inArr1*/double> B) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || object.Equals(B, null)) {
                    return !(object.Equals(A, null) ^ object.Equals(B, null));
                }
                if (A.IsEmpty && B.IsEmpty) return true;
                if (!A.Size.IsSameSize(B.Size)) return false;
                int pos = 0;
                foreach ( double a in A) {
                    
                    double b = B.GetValue(pos++);
                    if ( double.IsNaN(a) &&  double.IsNaN(b)) continue;
                    if ( double.IsInfinity(a) &&  double.IsInfinity(b)) continue;
                    if (b != a) return false;
                }
                return true;
            }
        }

#region HYCALPER AUTO GENERATED CODE


        /// <summary>
        /// Compares elements of two arrays, assume NaN and +/- infinity values are comparable and equal
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="B">Input array</param>
        /// <returns>True, if all elements of both arrays are equal; false if any elements or the sizes are not equal</returns>
        public static bool isequalwithequalnans(ILInArray</*HC:*/fcomplex> A, ILInArray</*HC:*/fcomplex> B) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || object.Equals(B, null)) {
                    return !(object.Equals(A, null) ^ object.Equals(B, null));
                }
                if (A.IsEmpty && B.IsEmpty) return true;
                if (!A.Size.IsSameSize(B.Size)) return false;
                int pos = 0;
                foreach ( fcomplex a in A) {
                   
                    fcomplex b = B.GetValue(pos++);
                    if ( fcomplex.IsNaN(a) &&  fcomplex.IsNaN(b)) continue;
                    if ( fcomplex.IsInfinity(a) &&  fcomplex.IsInfinity(b)) continue;
                    if (b != a) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Compares elements of two arrays, assume NaN and +/- infinity values are comparable and equal
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="B">Input array</param>
        /// <returns>True, if all elements of both arrays are equal; false if any elements or the sizes are not equal</returns>
        public static bool isequalwithequalnans(ILInArray</*HC:*/float> A, ILInArray</*HC:*/float> B) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || object.Equals(B, null)) {
                    return !(object.Equals(A, null) ^ object.Equals(B, null));
                }
                if (A.IsEmpty && B.IsEmpty) return true;
                if (!A.Size.IsSameSize(B.Size)) return false;
                int pos = 0;
                foreach ( float a in A) {
                   
                    float b = B.GetValue(pos++);
                    if ( float.IsNaN(a) &&  float.IsNaN(b)) continue;
                    if ( float.IsInfinity(a) &&  float.IsInfinity(b)) continue;
                    if (b != a) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Compares elements of two arrays, assume NaN and +/- infinity values are comparable and equal
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="B">Input array</param>
        /// <returns>True, if all elements of both arrays are equal; false if any elements or the sizes are not equal</returns>
        public static bool isequalwithequalnans(ILInArray</*HC:*/complex> A, ILInArray</*HC:*/complex> B) {
            using (ILScope.Enter(A)) {
                if (object.Equals(A, null) || object.Equals(B, null)) {
                    return !(object.Equals(A, null) ^ object.Equals(B, null));
                }
                if (A.IsEmpty && B.IsEmpty) return true;
                if (!A.Size.IsSameSize(B.Size)) return false;
                int pos = 0;
                foreach ( complex a in A) {
                   
                    complex b = B.GetValue(pos++);
                    if ( complex.IsNaN(a) &&  complex.IsNaN(b)) continue;
                    if ( complex.IsInfinity(a) &&  complex.IsInfinity(b)) continue;
                    if (b != a) return false;
                }
                return true;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Compare two arrays, assume NaN and +/- infinity values are comparable and equal
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="B">Input array B</param>
        /// <returns>True if all non-NaN elements in A and B equal, false otherwise</returns>
        public static bool isequalwithequalnans(ILBaseArray A, ILBaseArray B) {
            using (ILScope.Enter(A, B)) {
                if (A is ILDenseArray<double>) {
                    if (B is ILDenseArray<double>) {
                        return isequalwithequalnans((A as ILDenseArray<double>).C, (B as ILDenseArray<double>).C);
                    } else if (B is ILDenseArray<complex>) {
                        ILArray<complex> Bc = (B as ILDenseArray<complex>).C;
                        if (Bc == null) return false;
                        if (any(imag(Bc)).NumberNonZero > 0) return false;
                        return isequalwithequalnans((A as ILDenseArray<double>).C, real(Bc));
                    } return false;
                } else if (A is ILDenseArray<float>) {
                    if (B is ILDenseArray<float>) {
                        return isequalwithequalnans((A as ILDenseArray<float>).C, (B as ILDenseArray<float>).C);
                    } else if (B is ILDenseArray<fcomplex>) {
                        ILArray<fcomplex> Bf = (B as ILDenseArray<fcomplex>).C;
                        if (Bf == null) return false;
                        if (any(imag(Bf)).NumberNonZero > 0) return false;
                        return isequalwithequalnans((A as ILDenseArray<fcomplex>).C, real(Bf));
                    } return false;
                } else if (A is ILDenseArray<complex>) {
                    if (B is ILDenseArray<complex>) {
                        return isequalwithequalnans((A as ILDenseArray<complex>).C, (B as ILDenseArray<complex>).C);
                    } else if (B is ILDenseArray<double>) {
                        ILArray<complex> Ac = (A as ILDenseArray<complex>).C;
                        if (A == null) return false;
                        if (any(imag(Ac)).NumberNonZero > 0) return false;
                        return isequalwithequalnans(real(Ac), (B as ILDenseArray<double>).C);
                    } else return false;
                } else if (A is ILDenseArray<fcomplex>) {
                    if (B is ILDenseArray<fcomplex>) {
                        return isequalwithequalnans((A as ILDenseArray<fcomplex>).C, (B as ILDenseArray<fcomplex>).C);
                    } else if (B is ILDenseArray<float>) {
                        ILArray<fcomplex> Af = (A as ILDenseArray<fcomplex>).C;
                        if (Af == null) return false;
                        if (any(imag(Af)).NumberNonZero > 0) return false;
                        return isequalwithequalnans(real(Af), (B as ILDenseArray<float>).C);
                    } else return false;
                } else {
                    return A.Equals(B);
                }
            }
        }
    }
}
