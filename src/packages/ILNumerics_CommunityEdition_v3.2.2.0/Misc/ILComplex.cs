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


#pragma warning disable 162 
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
 


namespace ILNumerics {
    /// <summary>
    /// Floating point complex value data type of double precision
    /// </summary>
    /// <remarks>This class extends the system value types for real numbers to complex double 
    /// values. Besides the publicly available members 'real' and 'imag' it provides all the 
    /// basis functionality the floating point system.double brings (abs, log, sqrt, tan etc.),
    /// as well as it overrides the basic unary and binary operators for all common system value 
    /// types including rarely used types (e.g. UInt16). This includes the basic numerical operations 
    /// like '+','-','/','*' and the relational operators: '==','>','>=' etc. Also there are some 
    /// explicit and some implicit casting operators from / to complex value into system 
    /// value types.</remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct complex : IEquatable<complex> {
        /// <summary>
        /// Real part of this complex number
        /// </summary>
        public double real;
        /// <summary>
        /// Imaginary part of this complex number
        /// </summary>
        public double imag;
        /// <summary>
        /// Imaginary unit 
        /// </summary>
        public static readonly complex i = new complex(0.0f,1.0f); 

        /// <summary>
        /// Constructor creating a new complex value
        /// </summary>
        /// <param name="real">Real part</param>
        /// <param name="imag">Imaginary part</param>
        public complex(double real, double imag) {
            this.real = real; 
            this.imag = imag; 
        }
        /// <summary>
        /// Complex conjugate 
        /// </summary>
        public complex conj {
            get{
                return new complex(real,-imag);
            }
        }

        /// <summary>
        /// Positive infinity for real and imag part of complex value
        /// </summary>
       public static complex INF {
            get {
                return new complex(
                    double.PositiveInfinity,
                    double.PositiveInfinity
                );
            }
        }

        /// <summary>
        /// New complex, real and imaginary parts are zero
        /// </summary>
       public static complex Zero {
            get {
                return new complex(0.0,0.0);
            }
        }

        /// <summary>
        /// Complex quantity, marked as being "not a number"
        /// </summary>
        public static complex NaN {
            get {
                return new complex(double.NaN,double.NaN); 
            }
        }

        /// <summary>
        /// Are obj's real and imaginary part identical to the real and imaginary parts of this fcomplex
        /// </summary>
        /// <param name="obj">fcomplex object to determine the equality for</param>
        /// <returns>true if obj is of fcomplex type and its real and imag part has the same 
        /// values as the real and imaginary part of this array.</returns>
        public override bool Equals(object obj) {
            if (obj is complex && ((complex)obj) == this)
                return true; 
            return false; 
        }
        /// <summary>
        /// Check if a complex number equals this complex number
        /// </summary>
        /// <param name="other">other complex number</param>
        /// <returns>true if both, real and imaginary parts of both complex number are (binary) equal, false otherwise</returns>
        public bool Equals(complex other) {
            return real.Equals(other.real) && imag.Equals(other.imag); 
        }
        /// <summary>
        /// Hash code of this comples
        /// </summary>
        /// <returns>Hash code of this complex</returns>
        public override int GetHashCode() {
            return 77101 * real.GetHashCode() + imag.GetHashCode();
        }


        
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>result</returns>
        public static  complex operator +( complex A,  complex B) {
             complex ret; 
             ret.real =   (double) (A.real + B.real );
             ret.imag =   (double) (A.imag + B.imag );
            return ret;
        }
        /// <summary>
        /// Subtract two complex values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  complex operator -( complex A,  complex B) {
             complex ret; 
            ret.real =  (double) (A.real  - B.real );
            ret.imag =  (double) (A.imag - B.imag );
            return ret;
        }
        /// <summary>
        /// Multiply two complex values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  complex operator *( complex A,  complex B) {
             complex ret;
            ret.real =  (double) ((A.real * B.real ) - (A.imag * B.imag ));
            ret.imag =  (double) ((A.real * B.imag ) + (A.imag * B.real ));
            return ret; 
        }
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>Result</returns>
        /// <remarks><para>Unless the operator must handle special inputs (Inf or 0 values), 
        /// the algorithm described in [1] is used for division. This is considered to be 
        /// more robust against floating point overflow than the naive approach of simple 
        /// cartesian division.</para>
        /// <para>References: [1]: Smith, R.L., Algorithm 116: Complex division. Commun.ACM 5,8 (1962),435 <br />
        /// [2]: Stewart, G.W., A note on complex division, ACM trans.on math software, Vol.11, N.3 (1985)</para></remarks>
        public static  complex operator /( complex A,  complex B) {
            if (B.imag == 0) return A / B.real; 
            return A * (1 / B); 
            if (IsNaN(A) ||  complex .IsNaN(B)) return NaN;
            //if ( complex .IsInfinity(B)) return NaN;            
            //if (A.real == 0 && A.imag == 0) return ( complex )0;
             complex ret;
            if (B.real == 0) {
                ret.imag =  (double) -(A.real / B.imag); 
                ret.real =  (double) (A.imag / B.imag); 
                return ret; 
            }
            // this would be the naive approach. But it come with to little robustness against overflow
            //double norm2 = B.real * B.real + B.imag * B.imag;
            //if (norm2 == 0) return INF;    // this may be removed, since division by 0 results in inf anyway ? 
            //ret.real =  (double) (((A.real  * B.real ) + (A.imag  * B.imag )) / norm2);
            //ret.imag =  (double) (((A.imag  * B.real ) - (A.real  * B.imag )) / norm2); 
            
            // this algorithm is taken from [1]. The one described in [2] was not taken. Tests 
            // did not show any advantage when using double precision floating point arithmetic.
             double tmp1, tmp2; 
            if (Math.Abs(B.real) >= Math.Abs(B.imag)) {
                tmp1 =  (double) (B.imag * (1/B.real)); 
                tmp2 =  (double) (B.real + B.imag*tmp1); 
                ret.real =  (double) (A.real + A.imag*tmp1)/tmp2; 
                ret.imag =  (double) (A.imag - A.real*tmp1)/tmp2; 
            } else {
                tmp1 =  (double) (B.real * (1/B.imag));
                tmp2 =  (double) (B.imag + B.real*tmp1); 
                ret.real =  (double) (A.imag + A.real*tmp1)/tmp2; 
                ret.imag = -  (double) (A.real - A.imag*tmp1)/tmp2; 
            }
            return ret;                                            
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true, if real and imaginary part are identical</returns>
        public static bool operator ==( complex A,  complex B) {
            return (A.imag  == B.imag ) && (A.real  == B.real );
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        public static bool operator !=( complex A,  complex B) {
            return (A.imag  != B.imag ) || (A.real  != B.real );
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( complex A,  complex B) {
            return (A.real > B.real );
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( complex A,  complex B) {
            return (A.real < B.real );
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( complex A,  complex B) {
            return (A.real >= B.real );
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( complex A,  complex B) { 
            return (A.real <= B.real );
        }

#region HYCALPER AUTO GENERATED CODE

       
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>result</returns>
        public static  complex operator +( complex A,  fcomplex B) {
            complex ret; 
             ret.real =  (double) (A.real + B.real );
             ret.imag =  (double) (A.imag + B.imag );
            return ret;
        }
        /// <summary>
        /// Subtract two complex values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  complex operator -( complex A,  fcomplex B) {
            complex ret; 
            ret.real =  (double) (A.real  - B.real );
            ret.imag =  (double) (A.imag - B.imag );
            return ret;
        }
        /// <summary>
        /// Multiply two complex values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  complex operator *( complex A,  fcomplex B) {
            complex ret;
            ret.real =  (double) ((A.real * B.real ) - (A.imag * B.imag ));
            ret.imag =  (double) ((A.real * B.imag ) + (A.imag * B.real ));
            return ret; 
        }
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>Result</returns>
        /// <remarks><para>Unless the operator must handle special inputs (Inf or 0 values), 
        /// the algorithm described in [1] is used for division. This is considered to be 
        /// more robust against floating point overflow than the naive approach of simple 
        /// cartesian division.</para>
        /// <para>References: [1]: Smith, R.L., Algorithm 116: Complex division. Commun.ACM 5,8 (1962),435 <br />
        /// [2]: Stewart, G.W., A note on complex division, ACM trans.on math software, Vol.11, N.3 (1985)</para></remarks>
        public static  complex operator /( complex A,  fcomplex B) {
            if (B.imag == 0) return A / B.real; 
            return A * (1 / B); 
            if (IsNaN(A) ||  fcomplex .IsNaN(B)) return NaN;
            //if ( fcomplex .IsInfinity(B)) return NaN;            
            //if (A.real == 0 && A.imag == 0) return ( complex )0;
            complex ret;
            if (B.real == 0) {
                ret.imag =  (double) -(A.real / B.imag); 
                ret.real =  (double) (A.imag / B.imag); 
                return ret; 
            }
            // this would be the naive approach. But it come with to little robustness against overflow
            //double norm2 = B.real * B.real + B.imag * B.imag;
            //if (norm2 == 0) return INF;    // this may be removed, since division by 0 results in inf anyway ? 
            //ret.real =  (double) (((A.real  * B.real ) + (A.imag  * B.imag )) / norm2);
            //ret.imag =  (double) (((A.imag  * B.real ) - (A.real  * B.imag )) / norm2); 
            
            // this algorithm is taken from [1]. The one described in [2] was not taken. Tests 
            // did not show any advantage when using double precision floating point arithmetic.
            double tmp1, tmp2; 
            if (Math.Abs(B.real) >= Math.Abs(B.imag)) {
                tmp1 =  (double) (B.imag * (1/B.real)); 
                tmp2 =  (double) (B.real + B.imag*tmp1); 
                ret.real =  (double) (A.real + A.imag*tmp1)/tmp2; 
                ret.imag =  (double) (A.imag - A.real*tmp1)/tmp2; 
            } else {
                tmp1 =  (double) (B.real * (1/B.imag));
                tmp2 =  (double) (B.imag + B.real*tmp1); 
                ret.real =  (double) (A.imag + A.real*tmp1)/tmp2; 
                ret.imag = -  (double) (A.real - A.imag*tmp1)/tmp2; 
            }
            return ret;                                            
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true, if real and imaginary part are identical</returns>
        public static bool operator ==( complex A,  fcomplex B) {
            return (A.imag  == B.imag ) && (A.real  == B.real );
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        public static bool operator !=( complex A,  fcomplex B) {
            return (A.imag  != B.imag ) || (A.real  != B.real );
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( complex A,  fcomplex B) {
            return (A.real > B.real );
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( complex A,  fcomplex B) {
            return (A.real < B.real );
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( complex A,  fcomplex B) {
            return (A.real >= B.real );
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( complex A,  fcomplex B) { 
            return (A.real <= B.real );
        }

#endregion HYCALPER AUTO GENERATED CODE


        
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  complex operator +( complex A,  double B) {
             complex ret;
            ret.real =  (double) (A.real + B);
            ret.imag =  (double) A.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  complex operator -( complex A,  double B) {
             complex ret;
            ret.real =  (double) (A.real - B);
            ret.imag =  (double) A.imag;
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  complex operator *( complex A,  double B) {
             complex ret;
            ret.real =  (double) (A.real * B);
            ret.imag =  (double) (A.imag * B);
            return ret;
        }
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>result</returns>
        public static  complex operator /( complex A,  double B) {
            if (IsNaN(A)) return NaN;
            
            if (double.IsNaN(B)) return NaN; 
            if (A.real == 0 && A.imag == 0) {
                if (B == 0) return NaN; 
                return ( complex )0;
            } else {
                
                if (double .IsInfinity(B)) 
                {
                    if (IsInfinity(A)) {
                        return NaN; 
                    } else {
                        return ( complex )0;
                    }
                }
            }
             complex ret;
            if (B == 0) return INF ;
            ret.real =  (double) (A.real / B);
            ret.imag =  (double) (A.imag / B);
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( complex A,  double B) {
            return (A.real == B && A.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( complex A,  double B) {
            return (A.imag != 0.0) || (A.real != B);
        }
        /// <summary>
        /// Freater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( complex A,  double B) {
            return (A.real > B);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <(  complex A,  double B) {
            return (A.real < B);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( complex A,  double B) {
            return (A.real >= B);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( complex A,  double B) {
            return (A.real <= B);
        }

#region HYCALPER AUTO GENERATED CODE

       
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  complex operator +( complex A,  Int64 B) {
            complex ret;
            ret.real =  (double) (A.real + B);
            ret.imag =  (double) A.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  complex operator -( complex A,  Int64 B) {
            complex ret;
            ret.real =  (double) (A.real - B);
            ret.imag =  (double) A.imag;
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  complex operator *( complex A,  Int64 B) {
            complex ret;
            ret.real =  (double) (A.real * B);
            ret.imag =  (double) (A.imag * B);
            return ret;
        }
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>result</returns>
        public static  complex operator /( complex A,  Int64 B) {
            if (IsNaN(A)) return NaN;
            
            if (A.real == 0 && A.imag == 0) {
                if (B == 0) return NaN; 
                return ( complex )0;
            } else {
                if (false)
                {
                    if (IsInfinity(A)) {
                        return NaN; 
                    } else {
                        return ( complex )0;
                    }
                }
            }
            complex ret;
            if (B == 0) return INF ;
            ret.real =  (double) (A.real / B);
            ret.imag =  (double) (A.imag / B);
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( complex A,  Int64 B) {
            return (A.real == B && A.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( complex A,  Int64 B) {
            return (A.imag != 0.0) || (A.real != B);
        }
        /// <summary>
        /// Freater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( complex A,  Int64 B) {
            return (A.real > B);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <(  complex A,  Int64 B) {
            return (A.real < B);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( complex A,  Int64 B) {
            return (A.real >= B);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( complex A,  Int64 B) {
            return (A.real <= B);
        }
       
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  complex operator +( complex A,  Int32 B) {
            complex ret;
            ret.real =  (double) (A.real + B);
            ret.imag =  (double) A.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  complex operator -( complex A,  Int32 B) {
            complex ret;
            ret.real =  (double) (A.real - B);
            ret.imag =  (double) A.imag;
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  complex operator *( complex A,  Int32 B) {
            complex ret;
            ret.real =  (double) (A.real * B);
            ret.imag =  (double) (A.imag * B);
            return ret;
        }
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>result</returns>
        public static  complex operator /( complex A,  Int32 B) {
            if (IsNaN(A)) return NaN;
            
            if (A.real == 0 && A.imag == 0) {
                if (B == 0) return NaN; 
                return ( complex )0;
            } else {
                if (false)
                {
                    if (IsInfinity(A)) {
                        return NaN; 
                    } else {
                        return ( complex )0;
                    }
                }
            }
            complex ret;
            if (B == 0) return INF ;
            ret.real =  (double) (A.real / B);
            ret.imag =  (double) (A.imag / B);
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( complex A,  Int32 B) {
            return (A.real == B && A.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( complex A,  Int32 B) {
            return (A.imag != 0.0) || (A.real != B);
        }
        /// <summary>
        /// Freater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( complex A,  Int32 B) {
            return (A.real > B);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <(  complex A,  Int32 B) {
            return (A.real < B);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( complex A,  Int32 B) {
            return (A.real >= B);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( complex A,  Int32 B) {
            return (A.real <= B);
        }
       
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  complex operator +( complex A,  float B) {
            complex ret;
            ret.real =  (double) (A.real + B);
            ret.imag =  (double) A.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  complex operator -( complex A,  float B) {
            complex ret;
            ret.real =  (double) (A.real - B);
            ret.imag =  (double) A.imag;
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  complex operator *( complex A,  float B) {
            complex ret;
            ret.real =  (double) (A.real * B);
            ret.imag =  (double) (A.imag * B);
            return ret;
        }
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>result</returns>
        public static  complex operator /( complex A,  float B) {
            if (IsNaN(A)) return NaN;
            if (float.IsNaN(B)) return NaN;
            if (A.real == 0 && A.imag == 0) {
                if (B == 0) return NaN; 
                return ( complex )0;
            } else {
                if (float.IsInfinity(B))
                {
                    if (IsInfinity(A)) {
                        return NaN; 
                    } else {
                        return ( complex )0;
                    }
                }
            }
            complex ret;
            if (B == 0) return INF ;
            ret.real =  (double) (A.real / B);
            ret.imag =  (double) (A.imag / B);
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( complex A,  float B) {
            return (A.real == B && A.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( complex A,  float B) {
            return (A.imag != 0.0) || (A.real != B);
        }
        /// <summary>
        /// Freater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( complex A,  float B) {
            return (A.real > B);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <(  complex A,  float B) {
            return (A.real < B);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( complex A,  float B) {
            return (A.real >= B);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( complex A,  float B) {
            return (A.real <= B);
        }
       
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  complex operator +( complex A,  byte B) {
            complex ret;
            ret.real =  (double) (A.real + B);
            ret.imag =  (double) A.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  complex operator -( complex A,  byte B) {
            complex ret;
            ret.real =  (double) (A.real - B);
            ret.imag =  (double) A.imag;
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  complex operator *( complex A,  byte B) {
            complex ret;
            ret.real =  (double) (A.real * B);
            ret.imag =  (double) (A.imag * B);
            return ret;
        }
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>result</returns>
        public static  complex operator /( complex A,  byte B) {
            if (IsNaN(A)) return NaN;
            
            if (A.real == 0 && A.imag == 0) {
                if (B == 0) return NaN; 
                return ( complex )0;
            } else {
                if (false)
                {
                    if (IsInfinity(A)) {
                        return NaN; 
                    } else {
                        return ( complex )0;
                    }
                }
            }
            complex ret;
            if (B == 0) return INF ;
            ret.real =  (double) (A.real / B);
            ret.imag =  (double) (A.imag / B);
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( complex A,  byte B) {
            return (A.real == B && A.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( complex A,  byte B) {
            return (A.imag != 0.0) || (A.real != B);
        }
        /// <summary>
        /// Freater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( complex A,  byte B) {
            return (A.real > B);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <(  complex A,  byte B) {
            return (A.real < B);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( complex A,  byte B) {
            return (A.real >= B);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( complex A,  byte B) {
            return (A.real <= B);
        }

#endregion HYCALPER AUTO GENERATED CODE


        
        /// <summary>
        /// Add two complex values
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  complex operator +( double A,  complex B) {
             complex ret; 
            ret.real =  (double) (A + B.real);
            ret.imag =  (double) B.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>Result</returns>
        public static  complex operator -( double A,  complex B) {
             complex ret;
            ret.real =  (double) (A - B.real);
            ret.imag = - (double) B.imag; 
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>Result</returns>
        public static  complex operator *( double A,  complex B) {
             complex ret;
            ret.real =  (double) (A * B.real);
            ret.imag =  (double) (A * B.imag);
            return ret;
        }
        /// <summary>
        /// Divide two values
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>Result</returns>
        public static  complex operator /( double A,  complex B) {
             complex ret; 
            if (A == 0) {
                if (IsInfinity(B)) return NaN; 
            } else {
                if (IsInfinity(B)) return ( complex )0; 
            }
            if (B.real == 0 && B.imag == 0) {
                return INF;
            }
            // this algorithm is taken from [1]. The one described in [2] was not taken. Tests 
            // did not show any advantage when using double precision floating point arithmetic.
             double tmp; 
            if (Math.Abs(B.real) >= Math.Abs(B.imag)) {
                tmp =  (double) (B.imag * (1/B.real)); 
                ret.imag =  (double) (B.real + B.imag*tmp); 
                ret.real =  (double) A/ret.imag; 
                ret.imag = -  (double) (A*tmp)/ret.imag; 
            } else {
                tmp =  (double) (B.real * (1/B.imag));
                ret.imag =  (double) (B.imag + B.real*tmp); 
                ret.real =  (double) (A*tmp)/ret.imag; 
                ret.imag = -  (double) A/ret.imag; 
            }
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( double A,  complex B) {
            return (B.real == A && B.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( double A,  complex B) {
            return (B.imag != 0.0) || (B.real != A);
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( double A,  complex B) {
            return (A > B.real);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( double A,  complex B) {
            return (A < B.real);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( double A,  complex B) {
            return (A >= B.real);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( double A,  complex B) {
            return (A <= B.real);
        }

#region HYCALPER AUTO GENERATED CODE

       
        /// <summary>
        /// Add two complex values
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  complex operator +( Int64 A,  complex B) {
            complex ret; 
            ret.real =  (double) (A + B.real);
            ret.imag =  (double) B.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>Result</returns>
        public static  complex operator -( Int64 A,  complex B) {
            complex ret;
            ret.real =  (double) (A - B.real);
            ret.imag = - (double) B.imag; 
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>Result</returns>
        public static  complex operator *( Int64 A,  complex B) {
            complex ret;
            ret.real =  (double) (A * B.real);
            ret.imag =  (double) (A * B.imag);
            return ret;
        }
        /// <summary>
        /// Divide two values
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>Result</returns>
        public static  complex operator /( Int64 A,  complex B) {
            complex ret; 
            if (A == 0) {
                if (IsInfinity(B)) return NaN; 
            } else {
                if (IsInfinity(B)) return ( complex )0; 
            }
            if (B.real == 0 && B.imag == 0) {
                return INF;
            }
            // this algorithm is taken from [1]. The one described in [2] was not taken. Tests 
            // did not show any advantage when using double precision floating point arithmetic.
            double tmp; 
            if (Math.Abs(B.real) >= Math.Abs(B.imag)) {
                tmp =  (double) (B.imag * (1/B.real)); 
                ret.imag =  (double) (B.real + B.imag*tmp); 
                ret.real =  (double) A/ret.imag; 
                ret.imag = -  (double) (A*tmp)/ret.imag; 
            } else {
                tmp =  (double) (B.real * (1/B.imag));
                ret.imag =  (double) (B.imag + B.real*tmp); 
                ret.real =  (double) (A*tmp)/ret.imag; 
                ret.imag = -  (double) A/ret.imag; 
            }
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( Int64 A,  complex B) {
            return (B.real == A && B.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( Int64 A,  complex B) {
            return (B.imag != 0.0) || (B.real != A);
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( Int64 A,  complex B) {
            return (A > B.real);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( Int64 A,  complex B) {
            return (A < B.real);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( Int64 A,  complex B) {
            return (A >= B.real);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( Int64 A,  complex B) {
            return (A <= B.real);
        }
       
        /// <summary>
        /// Add two complex values
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  complex operator +( Int32 A,  complex B) {
            complex ret; 
            ret.real =  (double) (A + B.real);
            ret.imag =  (double) B.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>Result</returns>
        public static  complex operator -( Int32 A,  complex B) {
            complex ret;
            ret.real =  (double) (A - B.real);
            ret.imag = - (double) B.imag; 
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>Result</returns>
        public static  complex operator *( Int32 A,  complex B) {
            complex ret;
            ret.real =  (double) (A * B.real);
            ret.imag =  (double) (A * B.imag);
            return ret;
        }
        /// <summary>
        /// Divide two values
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>Result</returns>
        public static  complex operator /( Int32 A,  complex B) {
            complex ret; 
            if (A == 0) {
                if (IsInfinity(B)) return NaN; 
            } else {
                if (IsInfinity(B)) return ( complex )0; 
            }
            if (B.real == 0 && B.imag == 0) {
                return INF;
            }
            // this algorithm is taken from [1]. The one described in [2] was not taken. Tests 
            // did not show any advantage when using double precision floating point arithmetic.
            double tmp; 
            if (Math.Abs(B.real) >= Math.Abs(B.imag)) {
                tmp =  (double) (B.imag * (1/B.real)); 
                ret.imag =  (double) (B.real + B.imag*tmp); 
                ret.real =  (double) A/ret.imag; 
                ret.imag = -  (double) (A*tmp)/ret.imag; 
            } else {
                tmp =  (double) (B.real * (1/B.imag));
                ret.imag =  (double) (B.imag + B.real*tmp); 
                ret.real =  (double) (A*tmp)/ret.imag; 
                ret.imag = -  (double) A/ret.imag; 
            }
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( Int32 A,  complex B) {
            return (B.real == A && B.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( Int32 A,  complex B) {
            return (B.imag != 0.0) || (B.real != A);
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( Int32 A,  complex B) {
            return (A > B.real);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( Int32 A,  complex B) {
            return (A < B.real);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( Int32 A,  complex B) {
            return (A >= B.real);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( Int32 A,  complex B) {
            return (A <= B.real);
        }
       
        /// <summary>
        /// Add two complex values
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  complex operator +( float A,  complex B) {
            complex ret; 
            ret.real =  (double) (A + B.real);
            ret.imag =  (double) B.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>Result</returns>
        public static  complex operator -( float A,  complex B) {
            complex ret;
            ret.real =  (double) (A - B.real);
            ret.imag = - (double) B.imag; 
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>Result</returns>
        public static  complex operator *( float A,  complex B) {
            complex ret;
            ret.real =  (double) (A * B.real);
            ret.imag =  (double) (A * B.imag);
            return ret;
        }
        /// <summary>
        /// Divide two values
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>Result</returns>
        public static  complex operator /( float A,  complex B) {
            complex ret; 
            if (A == 0) {
                if (IsInfinity(B)) return NaN; 
            } else {
                if (IsInfinity(B)) return ( complex )0; 
            }
            if (B.real == 0 && B.imag == 0) {
                return INF;
            }
            // this algorithm is taken from [1]. The one described in [2] was not taken. Tests 
            // did not show any advantage when using double precision floating point arithmetic.
            double tmp; 
            if (Math.Abs(B.real) >= Math.Abs(B.imag)) {
                tmp =  (double) (B.imag * (1/B.real)); 
                ret.imag =  (double) (B.real + B.imag*tmp); 
                ret.real =  (double) A/ret.imag; 
                ret.imag = -  (double) (A*tmp)/ret.imag; 
            } else {
                tmp =  (double) (B.real * (1/B.imag));
                ret.imag =  (double) (B.imag + B.real*tmp); 
                ret.real =  (double) (A*tmp)/ret.imag; 
                ret.imag = -  (double) A/ret.imag; 
            }
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( float A,  complex B) {
            return (B.real == A && B.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( float A,  complex B) {
            return (B.imag != 0.0) || (B.real != A);
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( float A,  complex B) {
            return (A > B.real);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( float A,  complex B) {
            return (A < B.real);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( float A,  complex B) {
            return (A >= B.real);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( float A,  complex B) {
            return (A <= B.real);
        }
       
        /// <summary>
        /// Add two complex values
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  complex operator +( byte A,  complex B) {
            complex ret; 
            ret.real =  (double) (A + B.real);
            ret.imag =  (double) B.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>Result</returns>
        public static  complex operator -( byte A,  complex B) {
            complex ret;
            ret.real =  (double) (A - B.real);
            ret.imag = - (double) B.imag; 
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>Result</returns>
        public static  complex operator *( byte A,  complex B) {
            complex ret;
            ret.real =  (double) (A * B.real);
            ret.imag =  (double) (A * B.imag);
            return ret;
        }
        /// <summary>
        /// Divide two values
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>Result</returns>
        public static  complex operator /( byte A,  complex B) {
            complex ret; 
            if (A == 0) {
                if (IsInfinity(B)) return NaN; 
            } else {
                if (IsInfinity(B)) return ( complex )0; 
            }
            if (B.real == 0 && B.imag == 0) {
                return INF;
            }
            // this algorithm is taken from [1]. The one described in [2] was not taken. Tests 
            // did not show any advantage when using double precision floating point arithmetic.
            double tmp; 
            if (Math.Abs(B.real) >= Math.Abs(B.imag)) {
                tmp =  (double) (B.imag * (1/B.real)); 
                ret.imag =  (double) (B.real + B.imag*tmp); 
                ret.real =  (double) A/ret.imag; 
                ret.imag = -  (double) (A*tmp)/ret.imag; 
            } else {
                tmp =  (double) (B.real * (1/B.imag));
                ret.imag =  (double) (B.imag + B.real*tmp); 
                ret.real =  (double) (A*tmp)/ret.imag; 
                ret.imag = -  (double) A/ret.imag; 
            }
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>Result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( byte A,  complex B) {
            return (B.real == A && B.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( byte A,  complex B) {
            return (B.imag != 0.0) || (B.real != A);
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( byte A,  complex B) {
            return (A > B.real);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( byte A,  complex B) {
            return (A < B.real);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( byte A,  complex B) {
            return (A >= B.real);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( byte A,  complex B) {
            return (A <= B.real);
        }

#endregion HYCALPER AUTO GENERATED CODE

        #region unary minus
        /// <summary>
        /// Unary minus operator
        /// </summary>
        /// <param name="A">Complex input</param>
        /// <returns>Complex number similar to A, having real and imag part negated</returns>
        public static complex operator -( complex A) {
            complex ret = new complex(); 
            ret.imag = -A.imag; 
            ret.real = -A.real; 
            return ret;
        }
        #endregion

        #region CAST_OPERATORS
        /// <summary>
        /// Cast value to complex number
        /// </summary>
        /// <param name="a">Value to cast</param>
        /// <returns>Complex number with the real part having the same value as a and the imaginary part is 0.</returns>
        public static implicit operator complex(double a) {
            return new complex(a, 0.0);
        }
        /// <summary>
        /// Cast value to complex number
        /// </summary>
        /// <param name="a">Value to cast</param>
        /// <returns>Complex number with the real part having the same value as the a and the imaginary part is 0.</returns>
        public static implicit operator complex(float a) {
            return new complex(a, 0.0);
        }
        /// <summary>
        /// Cast value to complex number
        /// </summary>
        /// <param name="a">Value to cast</param>
        /// <returns>Complex number being a copy of the real and imaginary parts of a.</returns>
        public static implicit operator complex(fcomplex a) {
            return new complex(a.real, a.imag);
        }
        /// <summary>
        /// Cast value to complex number
        /// </summary>
        /// <param name="a">Value to cast</param>
        /// <returns>Complex number with the real part having the same value as a and the imaginary part is 0.</returns>
        public static implicit operator complex(byte a) {
            return new complex(a, 0.0);
        }
        /// <summary>
        /// Cast value to complex number
        /// </summary>
        /// <param name="a">Value to cast</param>
        /// <returns>Complex number with the real part having the same value as a and the imaginary part is 0.</returns>
        public static implicit operator complex(Int32 a) {
            return new complex(a, 0.0);
        }
        /// <summary>
        /// Cast value to complex number
        /// </summary>
        /// <param name="a">Value to cast</param>
        /// <returns>Complex number with the real part having the same value as a and the imaginary part is 0.</returns>
        public static implicit operator complex(Int64 a) {
            return new complex(a, 0.0);
        }

        /// <summary>
        /// Cast value from complex number
        /// </summary>
        /// <param name="a">Complex value to cast</param>
        /// <returns>Double number with the real part of a </returns>
        public static explicit operator double(complex a) {
            return a.real; 
        }
        /// <summary>
        /// Cast value from complex number
        /// </summary>
        /// <param name="a">Complex value to cast</param>
        /// <returns>number with the real part of a </returns>
        /// <remarks>the return value is the result of a cast from double to float.</remarks>
        public static explicit operator float(complex a) {
            return (float)a.real;
        }
        /// <summary>
        /// Cast value from complex number
        /// </summary>
        /// <param name="a">Complex value to cast</param>
        /// <returns>float complex number with the real and imaginary parts being a copy of a </returns>
        /// <remarks>The real and imaginary parts are the result of a cast to float.</remarks>
        public static explicit operator fcomplex(complex a) {
            return new fcomplex((float)a.real, (float)a.imag);
        }
        /// <summary>
        /// Cast value from complex number
        /// </summary>
        /// <param name="a">Complex value to cast</param>
        /// <returns>Number with the real part of a </returns>
        /// <remarks>The return value is the result of a cast to byte.</remarks>
        public static explicit operator byte(complex a) {
            return (byte) a.real; 
        }
        /// <summary>
        /// Cast value from complex number
        /// </summary>
        /// <param name="a">Complex value to cast</param>
        /// <returns>Number with the real part of a </returns>
        /// <remarks>The return value is the result of a cast to Int32.</remarks>
        public static explicit operator Int32(complex a) {
            return (Int32) a.real; 
        }
        /// <summary>
        /// Cast value from complex number
        /// </summary>
        /// <param name="a">Complex value to cast</param>
        /// <returns>number with the real part of a </returns>
        /// <remarks>the return value is the result of a cast to Int64.</remarks>
        public static explicit operator Int64(complex a) {
            return (Int64) a.real; 
        }

        public static implicit operator complex(System.Numerics.Complex a) {
            return new complex(a.Real, a.Imaginary); 
        }
#endregion CAST_OPERATORS

        #region Functions Basic Math
        /// <summary>
        /// Absolute value of input
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>The absolute value of the input</returns>
        public static double Abs(complex input) {
            return Math.Sqrt(input.real * input.real + input.imag * input.imag);
        }
        /// <summary>
        /// Phase angle of complex number
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>The phase angle of the input</returns>
        /// <remarks>For the result the Atan2 function of the <see cref="Math"/> class is used.</remarks>
        public static double Angle(complex input) {
            return Math.Atan2(input.imag, input.real);
        }
        /// <summary>
        /// Arcus tangens of complex input
        /// </summary>
        /// <param name="input">Complex input</param>
        /// <returns>Arcus tangens of complex input</returns>
        /// <remarks></remarks>
        public static complex Atan(complex input) {
            complex ret = new complex(0, (float)0.5);
            return (ret * Log((complex.i + input) / (complex.i - input)));
        }
        /// <summary>
        /// Arcus cosinus of complex input
        /// </summary>
        /// <param name="input">Complex input</param>
        /// <returns>Arcus cosinus of input</returns>
        public static complex Acos(complex input) {
            complex ni = complex.i * -1.0;
            return complex.Log(complex.Sqrt(input * input - 1)
                + input) * ni;
        }
        /// <summary>
        /// Arcus cosinus of input
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Arcus cosinus of input</returns>
        public static complex Acos(double input) {
            if (Math.Abs(input) <= 1.0)
                return new complex(Math.Acos(input), 0.0);
            else {
                return Acos((complex)input);
            }
        }
        /// <summary>
        /// Arcus sinus of complex input
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Arcus sinus of input</returns>
        public static complex Asin(double input) {
            if (Math.Abs(input) <= 1.0)
                return new complex(Math.Asin(input), 0.0);
            else {
                return Asin((complex)input);
            }
        }
        /// <summary>
        /// Arcus sinus of input
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Arcus sinus of  input</returns>
        public static complex Asin(complex input) {
            complex ret = Acos(input);
            ret.real = Math.PI / 2 - ret.real;
            return ret;
        }
        /// <summary>
        /// Round towards positive infinity
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Result is the next integer value greater then input</returns>
        /// <remarks>ILMath.Ceiling operates in both: real and imaginary parts seperately</remarks>
        public static complex Ceiling (complex input){
            return new complex(
                    Math.Ceiling(input.real),
                    Math.Ceiling(input.imag)
            );
        }
        /// <summary>
        /// Round towards negative infinity
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Result is the next integer value lower then input</returns>
        /// <remarks>ILMath.Floor operates in both: real and imaginary parts seperately</remarks>
        public static complex Floor (complex input){
            return new complex(
                    Math.Floor(input.real),
                    Math.Floor(input.imag)
            );
        }
        /// <summary>
        /// Rounds towards nearest integer
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Result is the nearest integer value for input</returns>
        /// <remarks>ILMath.Round operates in both: real and imaginary parts deperately</remarks>
        public static complex Round (complex input){
            return new complex(
                    Math.Round(input.real),
                    Math.Round(input.imag)
            );
        }
        /// <summary>
        /// Signum function
        /// </summary>
        /// <param name="input">Complex input </param>
        /// <returns>Sesult as input / Abs(input)</returns>
        /// <remarks>Sign(input) with input being complex returns the projection onto
        /// the unit circle. If input is 0+0i the result will be 0+0i.</remarks>
        public static complex Sign (complex input){
            if (input.real == 0.0 && input.imag == 0.0)
                return new complex(); 
            else {
                double mag = Math.Sqrt(input.real * input.real + input.imag * input.imag); 
                return new complex(
                    input.real / mag,
                    input.imag / mag);
            }
        }
        /// <summary>
        /// Truncate a floating point complex value
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Integer part of input</returns>
        /// <remarks>Operates on real and imaginary parts seperately.</remarks>
        public static complex Truncate (complex input){
            return new complex(
                    Math.Truncate(input.real),
                    Math.Truncate(input.imag)
            );
        }
        /// <summary>
        /// Cosinus
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Cosine of input</returns>
        /// <remarks><para>The cosine is computed by the trigonometric euler equation: </para>
        /// <para>0.5 * [exp(i input) + exp(-i input)]</para></remarks>
        public static complex Cos(complex input) {
            complex i = new complex(0, 1.0);
            complex mi = new complex(0, -1.0);
            return (Exp(i * input) + Exp(mi * input)) / 2.0;
        }
        /// <summary>
        /// Cosinus hyperbolicus
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>Cosine hyperbolicus of input</returns>
        /// <remarks><para>The cosine is computed by the trigonometric euler equation: </para>
        /// <para>(Exp(input) + Exp(-1.0 * input)) / 2.0</para></remarks>
        public static complex Cosh(complex input) {
            return (Exp(input) + Exp(-1.0 * input)) / 2.0;
        }
        /// <summary>
        /// Sinus
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Sinus of input</returns>
        /// <remarks><para>The sinus is computed by the trigonometric euler equation: </para>
        /// <para>(Exp(i * input) - Exp(-1.0 * i * input)) / (2.0 * i)</para></remarks>
        public static complex Sin(complex input) {
            complex i = new complex(0, 1.0);
            complex mi = new complex(0, -1.0);
            return (Exp(i * input) - Exp(mi * input)) / (2.0 * i);
        }
        /// <summary>
        /// Sinus hyperbolicus
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>Sinus hyperbolicus of input</returns>
        /// <remarks><para>The sinus hyperbolicus is computed by the trigonometric euler equation: </para>
        /// <para>(Exp(input) - Exp(-1.0 * input)) / 2.0</para></remarks>
        public static complex Sinh(complex input) {
            return (Exp(input) - Exp(-1.0 * input)) / 2.0;
        }
        /// <summary>
        /// Complex exponent
        /// </summary>
        /// <param name="exponent">Exponent</param>
        /// <returns>Result of exp(exponent)</returns>
        /// <remarks>For complex exponents, exp(exponent) is computed by
        /// <para>complex.FromPol(Math.Exp(exponent.real), exponent.imag)</para></remarks>
        public static complex Exp(complex exponent) {
            return complex.FromPol(Math.Exp(exponent.real), exponent.imag);
        }
        /// <summary>
        /// Complex power for real exponent
        /// </summary>
        /// <param name="input">Basis</param>
        /// <param name="exponent">Exponent</param>
        /// <returns>Result of input power exponent</returns>
        /// <remarks>The computation will be carried out by 
        /// <para>exp(log(input) * exponent)</para></remarks>
        public static complex Pow(complex input, double exponent) {
            complex ret = input.Log();
            ret.imag *= exponent;
            ret.real *= exponent;
            return ret.Exp();
        }
        /// <summary>
        /// Complex power - real basis, real exponent
        /// </summary>
        /// <param name="basis">Basis</param>
        /// <param name="exponent">Exponent</param>
        /// <returns>Complex number.</returns>
        /// <remarks>The result will be a complex number. For negative basis 
        /// the basis will be converted to a complex number and the power 
        /// will be computed in the complex plane.</remarks>
        public static complex Pow(double basis, double exponent) {
            if (basis >= 0.0)
                return Math.Pow(basis, exponent);
            else
                return Pow((complex)basis, exponent);
        }
        /// <summary>
        /// Complex power - complex exponent
        /// </summary>
        /// <param name="basis">Basis</param>
        /// <param name="exponent">Exponent</param>
        /// <returns>Complex number exp(log(basis) * exponent).</returns>
        /// <remarks>The result will be the complex number exp(log(basis) * exponent). </remarks>
        public static complex Pow(complex basis, complex exponent) {
            complex ret = (basis.Log() * exponent);
            return ret.Exp();
        }
        /// <summary>
        /// Square root
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>The square root of input</returns>
        /// <remarks>If input is smaller than 0.0, the computation will be done in the complex plane. </remarks>
        public static complex Sqrt(double input) {
            if (input > 0.0)
                return new complex(Math.Sqrt(input), 0.0);
            else
                return Sqrt(input);
        }
        /// <summary>
        /// Square root
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>The square root of input</returns>
        /// <remarks>Numerical recipes in C: Appendix C </remarks>
        public static complex Sqrt(complex input) {
            // Reference : numerical recipes in C: Appendix C
            complex ret = new complex();
            double x, y, w, r;
            if (input.real == 0.0 && input.imag == 0.0)
                return ret;
            else {
                x = (double)Math.Abs(input.real);
                y = (double)Math.Abs(input.imag);
                if (x >= y) {
                    r = y / x;
                    w = Math.Sqrt(x) * Math.Sqrt(0.5 * (1.0 + Math.Sqrt(1.0 + r * r)));
                } else {
                    r = x / y;
                    w = Math.Sqrt(y) * Math.Sqrt(0.5 * (r + Math.Sqrt(1.0 + r * r)));
                }
                if (input.real >= 0.0) {
                    ret.real = w;
                    ret.imag = input.imag / (2.0 * w);
                } else {
                    ret.imag = (input.imag >= 0) ? w : -w;
                    ret.real = input.imag / (2.0 * ret.imag);
                }
                return ret;
            }
        }
        /// <summary>
        /// Tangens
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Tangens of input</returns>
        /// <remarks>The tangens is 
        /// <para>sin(input) / cos(input)</para>
        /// if cos(input) == 0.0+0.0i, INF will be returned.</remarks>
        public static complex Tan(complex input) {
            complex ci = Cos(input);
            if (ci.real == 0.0 && ci.imag == 0.0)
                return INF;
            return (Sin(input) / ci);
        }
        /// <summary>
        /// Tangens hyperbolicus
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Tangens hyperbolicus</returns>
        /// <remarks>The tangens hyperbolicus is 
        /// <para>sinh(input) / cosh(input)</para>
        /// if cosh(input) == 0.0+0.0i, INF will be returned.</remarks>
        public static complex Tanh(complex input) {
            complex si = Cosh(input);
            if (si.real == 0.0 && si.imag == 0.0)
                return INF;
            return (Sinh(input) / si);
        }
        /// <summary>
        /// Complex logarithm 
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Complex logarithm of input</returns>
        /// <remarks>The real part of the logarithm is computed by 
        /// <para>log (abs (input))</para>
        /// <para>The imaginary part holds the phase of input.</para>
        /// </remarks>
        public static complex Log(complex input) {
            complex ret = new complex();
            ret.real = Math.Log(Math.Sqrt(input.real * input.real + input.imag * input.imag));
            ret.imag = Math.Atan2(input.imag, input.real);
            return ret;
        }
        /// <summary>
        /// Logarithm of real input 
        /// </summary>
        /// <param name="input">Input value - may be negative</param>
        /// <returns>Complex logarithm</returns>
        public static complex Log(double input) {
            return Log (new complex(input,0.0)); 
        }
        /// <summary>
        /// Logarithm of base 10 of real input 
        /// </summary>
        /// <param name="input">Input value - may be negative</param>
        /// <returns>Complex logarithm of base 10</returns>
        public static complex Log10(double input) {
            return Log(new complex(input,0.0)) * 0.43429448190325176;
        }
        /// <summary>
        /// Logarithm of base 2 of real input 
        /// </summary>
        /// <param name="input">Input value - may be negative</param>
        /// <returns>Complex logarithm of base 2</returns>
        public static complex Log2(double input) {
            return Log(new complex(input,0.0)) * 1.4426950408889641;
        }
        /// <summary>
        /// Logarithm of base 10
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Logarithm of base 10</returns>
        /// <seealso cref="ILNumerics.complex.Log(complex)"/>
        public static complex Log10(complex input) {
            return Log(input) * 0.43429448190325176;
        }
        /// <summary>
        /// Logarithm of base 2
        /// </summary>
        /// <param name="input">Input value</param>
        /// <returns>Logarithm of base 2.</returns>
        /// <seealso cref="ILNumerics.complex.Log(complex)"/>
        public static complex Log2(complex input) {
            return Log(input) * 1.4426950408889634;
        }
        /// <summary>
        /// Convert polar notation into cartesian notation
        /// </summary>
        /// <param name="magnitude">Magnitude</param>
        /// <param name="angle">Phase</param>
        /// <returns>Complex value having magnitude and phase</returns>
        public static complex FromPol(double magnitude, double angle) {
            return new complex(
                magnitude * Math.Cos(angle),
                magnitude * Math.Sin(angle)
            );
        }
        /// <summary>
        /// Convert to string
        /// </summary>
        /// <returns>String displaying the comlex number (full precision)</returns>
        public override String ToString() {
            if (imag>=0)
                return String.Format("{0} + {1}i",real,imag);
            else 
                return String.Format("{0} - {1}i",real,-imag);
        }
        private static string m_precSpecI = ""; 
        private static string m_precSpecR = ""; 
        private static int m_lastDigits = 0; 
        /// <summary>
        /// Print formated output of this number, determine number of digits
        /// </summary>
        /// <param name="digits">Number of digits</param>
        /// <returns>Formated output</returns>
        public string ToString(int digits) {
            if (digits < 1) return ""; 
            if (digits != m_lastDigits) {
                m_lastDigits = digits; 
                m_precSpecR = String.Format("{{0:f{0}}}",digits);
                m_precSpecI = String.Format("{{1:f{0}}}i",digits);
            }
            if (imag >= 0) {
                return String.Format(m_precSpecR+"+"+m_precSpecI,real,imag); 
            } else {
                return String.Format(m_precSpecR+"-"+m_precSpecI,real,-imag); 
            }
        }
        /// <summary>
        /// Magnitude of this complex instance
        /// </summary>
        /// <returns>Magnitude</returns>
        public double Abs() {
            return Math.Sqrt(real * real + imag * imag);
        }
        /// <summary>
        /// Phase of this complex instance
        /// </summary>
        /// <returns>Phase</returns>
        public double Angle() {
            return Math.Atan2(imag, real);
        }
        /// <summary>
        /// Arcus cosinus of this complex instance
        /// </summary>
        /// <returns>Arcus cosinus</returns>
        public complex Acos() {
            complex ret = new complex(0, -1);
            return complex.Log(complex.Sqrt(this * this - 1)
                + this) * ret;
        }
        /// <summary>
        /// Arcus sinus of this complex instance
        /// </summary>
        /// <returns>arcus sinus</returns>
        public complex Asin() {
            complex ret = Acos(this);
            ret.real = Math.PI / 2 - ret.real;
            return ret;
        }
        /// <summary>
        /// Exponential / power of base e
        /// </summary>
        /// <returns>Power of base e</returns>
        public complex Exp() {
            return complex.FromPol(Math.Exp(real), imag);
        }
        /// <summary>
        /// Complex power real exponent
        /// </summary>
        /// <param name="exponent">Exponent</param>
        /// <returns>New complex number with result</returns>
        /// <remarks>If this instance is a and the exponent is e than 
        /// the result will be the complex number exp(log(a) * e). </remarks>
        public complex Pow(double exponent) {
            complex ret = Log();
            ret.imag *= exponent;
            ret.real *= exponent;
            return ret.Exp();
        }
        /// <summary>
        /// Complex power - complex exponent
        /// </summary>
        /// <param name="exponent">Exponent</param>
        /// <returns>Complex number exp(log(this) * exponent).</returns>
        /// <remarks>If this instance is a than 
        /// the result will be the complex number exp(log(a) * exponent). </remarks>
        public complex Pow(complex exponent) {
            complex ret = (Log() * exponent);
            return ret.Exp();
        }
        /// <summary>
        /// Square root of this complex value
        /// </summary>
        /// <returns>Square root of this complex value</returns>
        public complex Sqrt() {
            // Reference : numerical recipes in C: Appendix C
            complex ret = new complex();
            double x, y, w, r;
            if (real == 0.0 && imag == 0.0)
                return ret;
            else {
                x = (double)Math.Abs(real);
                y = (double)Math.Abs(imag);
                if (x >= y) {
                    r = y / x;
                    w = Math.Sqrt(x) * Math.Sqrt(0.5 * (1.0 + Math.Sqrt(1.0 + r * r)));
                } else {
                    r = x / y;
                    w = Math.Sqrt(y) * Math.Sqrt(0.5 * (r + Math.Sqrt(1.0 + r * r)));
                }
                if (real >= 0.0) {
                    ret.real = w;
                    ret.imag = imag / (2.0 * w);
                } else {
                    ret.imag = (imag >= 0) ? w : -w;
                    ret.real = imag / ( 2.0 * ret.imag );
                }
                return ret;
            }
        }
        /// <summary>
        /// Logarithm of base e
        /// </summary>
        /// <returns>Logarithm of base e</returns>
        /// <remarks>The logarithm of a complex number A is defined as follows: <br />
        /// <list type="none"><item>real part: log(abs(A))</item>
        /// <item>imag part: Atan2(imag(A),real(A))</item></list>
        /// </remarks>
        public complex Log() {
            complex ret = new complex();
            ret.real = Math.Log(Math.Sqrt(real * real + imag * imag));
            ret.imag = Math.Atan2(imag, real);
            return ret;
        }
        /// <summary>
        /// Test if any of real or imaginary parts are NAN's
        /// </summary>
        /// <param name="input">Complex number to test</param>
        /// <returns>true if any of real or imag part is not a number</returns>
        public static bool IsNaN(complex input) {
            if (double.IsNaN(input.real) || double.IsNaN(input.imag)) 
                return true; 
            else 
                return false; 
        }
        /// <summary>
        /// Test if any of real or imaginary parts are infinite
        /// </summary>
        /// <param name="input">Complex number to test</param>
        /// <returns>true if any of real or imag part is infinite</returns>
        public static bool IsInfinity(complex input) {  
            if (double.IsInfinity(input.real) || double.IsInfinity(input.imag)) 
                return true; 
            else 
                return false; 
        }
        /// <summary>
        /// Test if any of real or imaginary parts are pos.nfinite
        /// </summary>
        /// <param name="input">Complex number to test</param>
        /// <returns>true if any of real or imag part is positive infinite</returns>
        public static bool IsPositiveInfinity(complex input) {  
            if (double.IsPositiveInfinity(input.real) || double.IsPositiveInfinity(input.imag)) 
                return true; 
            else 
                return false; 
        }
        /// <summary>
        /// Test if any of real or imaginary parts are neg. infinite
        /// </summary>
        /// <param name="input">Complex number to test</param>
        /// <returns>true if any of real or imag part is negative infinite</returns>
        public static bool IsNegativeInfinity(complex input) {  
            if (double.IsNegativeInfinity(input.real) || double.IsNegativeInfinity(input.imag)) 
                return true; 
            else 
                return false; 
        }
        /// <summary>
        /// Test if any of real or imaginary parts are finite
        /// </summary>
        /// <param name="input">Complex number to test</param>
        /// <returns>true if any of real and imag part is finite</returns>
        public static bool IsFinite (complex input) {
            if (ILMath.isfinite(input.real) && ILMath.isfinite(input.imag)) 
                return true; 
            else 
                return false; 
        }
        /// <summary>
        /// Test if both of real or imaginary parts are 0
        /// </summary>
        /// <returns>true if real and imag part is 0</returns>
        public bool iszero() {
            if (real == 0.0 && imag == 0.0) 
                return true; 
            else 
                return false; 
        }
        #endregion Functions Basic Math
    }
    
}
