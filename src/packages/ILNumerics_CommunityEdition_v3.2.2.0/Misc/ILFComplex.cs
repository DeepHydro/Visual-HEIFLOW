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
    /// Floating point complex value data type of float (single) precision
    /// </summary>
    /// <remarks>This class extends the system value types for real numbers to complex float 
    /// values. Besides the publicly available members 'real' and 'imag' it provides all the 
    /// basis functionality the floating point System.double brings (abs, log, sqrt, tan etc.) for 
    /// float precision complex,
    /// as well as it overrides the basic unary and binary operators for all common system value 
    /// types including rarely used types (e.g. UInt16). This includes the basic numerical operations 
    /// like '+','-','/','*' and the relational operators: '==','>','>=' etc. Also there are some 
    /// explicit and some implicit casting operators from / to fcomplex values into system 
    /// value types. </remarks>
    [Serializable]    
    [StructLayout(LayoutKind.Sequential)]
    public struct fcomplex : IEquatable<fcomplex> {
        /// <summary>
        /// Real part of this complex number
        /// </summary>
        public float real;
        /// <summary>
        /// Imaginary part of this complex number
        /// </summary>
        public float imag;
        /// <summary>
        /// Imaginary unit 
        /// </summary>
        public static readonly fcomplex i = new fcomplex(0.0f,1.0f); 

        /// <summary>
        /// Construct new float complex number
        /// </summary>
        /// <param name="real">Real part</param>
        /// <param name="imag">Imaginary part</param>
        public fcomplex(float real, float imag) {
            this.real = real;
            this.imag = imag;
        }
        
        /// <summary>
        /// Complex conjugate 
        /// </summary>
        public fcomplex conj {
            get{
                return new fcomplex(real,imag * (-1.0f));
            }
        }

        /// <summary>
        /// Positive infinity for real and imag part of complex value
        /// </summary>
        public static fcomplex INF {
            get {
                return new fcomplex(
                    float.PositiveInfinity,
                    float.PositiveInfinity
                );
            }
        }

        /// <summary>
        /// New fcomplex, real and imaginary parts are zero
        /// </summary>
       public static fcomplex Zero {
            get {
                return new fcomplex(0f,0f);
            }
        }

        /// <summary>
        /// fcomplex quantity, marked as being "not a number"
        /// </summary>
        public static fcomplex NaN {
            get {
                return new fcomplex(float.NaN,float.NaN); 
            }
        }

        /// <summary>
        /// Are obj's real and imaginary part identical to the real and imaginary parts of this fcomplex
        /// </summary>
        /// <param name="obj">fcomplex object to determine the equality for</param>
        /// <returns>true if obj is of fcomplex type and its real and imag part has the same 
        /// values as the real and imaginary part of this array.</returns>
        public override bool Equals(object obj) {
            if (obj is fcomplex && ((fcomplex)obj) == this)
                return true; 
            return false; 
        }

        /// <summary>
        /// Check if a fcomplex number equals this fcomplex number
        /// </summary>
        /// <param name="other">other complex number</param>
        /// <returns>true if both, real and imaginary parts of both complex number are (binary) equal, false otherwise</returns>
        public bool Equals(fcomplex other) {
            return real.Equals(other.real) && imag.Equals(other.imag);
        }

        /// <summary>
        /// Give HashCode of this fcomplex number
        /// </summary>
        /// <returns>HashCode of this fcomplex number</returns>
        public override int GetHashCode() {
            return 31 * real.GetHashCode() + imag.GetHashCode();
        }


        

#region HYCALPER AUTO GENERATED CODE

       
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>result</returns>
        public static  complex operator +( fcomplex A,  complex B) {
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
        public static  complex operator -( fcomplex A,  complex B) {
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
        public static  complex operator *( fcomplex A,  complex B) {
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
        public static  complex operator /( fcomplex A,  complex B) {
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
        public static bool operator ==( fcomplex A,  complex B) {
            return (A.imag  == B.imag ) && (A.real  == B.real );
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        public static bool operator !=( fcomplex A,  complex B) {
            return (A.imag  != B.imag ) || (A.real  != B.real );
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( fcomplex A,  complex B) {
            return (A.real > B.real );
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( fcomplex A,  complex B) {
            return (A.real < B.real );
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( fcomplex A,  complex B) {
            return (A.real >= B.real );
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( fcomplex A,  complex B) { 
            return (A.real <= B.real );
        }
       
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>result</returns>
        public static  fcomplex operator +( fcomplex A,  fcomplex B) {
            fcomplex ret; 
             ret.real =  (float) (A.real + B.real );
             ret.imag =  (float) (A.imag + B.imag );
            return ret;
        }
        /// <summary>
        /// Subtract two complex values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  fcomplex operator -( fcomplex A,  fcomplex B) {
            fcomplex ret; 
            ret.real =  (float) (A.real  - B.real );
            ret.imag =  (float) (A.imag - B.imag );
            return ret;
        }
        /// <summary>
        /// Multiply two complex values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  fcomplex operator *( fcomplex A,  fcomplex B) {
            fcomplex ret;
            ret.real =  (float) ((A.real * B.real ) - (A.imag * B.imag ));
            ret.imag =  (float) ((A.real * B.imag ) + (A.imag * B.real ));
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
        public static  fcomplex operator /( fcomplex A,  fcomplex B) {
            if (B.imag == 0) return A / B.real; 
            return A * (1 / B); 
            if (IsNaN(A) ||  fcomplex .IsNaN(B)) return NaN;
            //if ( fcomplex .IsInfinity(B)) return NaN;            
            //if (A.real == 0 && A.imag == 0) return ( fcomplex )0;
            fcomplex ret;
            if (B.real == 0) {
                ret.imag =  (float) -(A.real / B.imag); 
                ret.real =  (float) (A.imag / B.imag); 
                return ret; 
            }
            // this would be the naive approach. But it come with to little robustness against overflow
            //double norm2 = B.real * B.real + B.imag * B.imag;
            //if (norm2 == 0) return INF;    // this may be removed, since division by 0 results in inf anyway ? 
            //ret.real =  (float) (((A.real  * B.real ) + (A.imag  * B.imag )) / norm2);
            //ret.imag =  (float) (((A.imag  * B.real ) - (A.real  * B.imag )) / norm2); 
            
            // this algorithm is taken from [1]. The one described in [2] was not taken. Tests 
            // did not show any advantage when using double precision floating point arithmetic.
            float tmp1, tmp2; 
            if (Math.Abs(B.real) >= Math.Abs(B.imag)) {
                tmp1 =  (float) (B.imag * (1/B.real)); 
                tmp2 =  (float) (B.real + B.imag*tmp1); 
                ret.real =  (float) (A.real + A.imag*tmp1)/tmp2; 
                ret.imag =  (float) (A.imag - A.real*tmp1)/tmp2; 
            } else {
                tmp1 =  (float) (B.real * (1/B.imag));
                tmp2 =  (float) (B.imag + B.real*tmp1); 
                ret.real =  (float) (A.imag + A.real*tmp1)/tmp2; 
                ret.imag = -  (float) (A.real - A.imag*tmp1)/tmp2; 
            }
            return ret;                                            
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true, if real and imaginary part are identical</returns>
        public static bool operator ==( fcomplex A,  fcomplex B) {
            return (A.imag  == B.imag ) && (A.real  == B.real );
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        public static bool operator !=( fcomplex A,  fcomplex B) {
            return (A.imag  != B.imag ) || (A.real  != B.real );
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( fcomplex A,  fcomplex B) {
            return (A.real > B.real );
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( fcomplex A,  fcomplex B) {
            return (A.real < B.real );
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( fcomplex A,  fcomplex B) {
            return (A.real >= B.real );
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( fcomplex A,  fcomplex B) { 
            return (A.real <= B.real );
        }

#endregion HYCALPER AUTO GENERATED CODE


        

#region HYCALPER AUTO GENERATED CODE

       
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  fcomplex operator +( fcomplex A,  Int64 B) {
            fcomplex ret;
            ret.real =  (float) (A.real + B);
            ret.imag =  (float) A.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  fcomplex operator -( fcomplex A,  Int64 B) {
            fcomplex ret;
            ret.real =  (float) (A.real - B);
            ret.imag =  (float) A.imag;
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  fcomplex operator *( fcomplex A,  Int64 B) {
            fcomplex ret;
            ret.real =  (float) (A.real * B);
            ret.imag =  (float) (A.imag * B);
            return ret;
        }
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>result</returns>
        public static  fcomplex operator /( fcomplex A,  Int64 B) {
            if (IsNaN(A)) return NaN;
            
            if (A.real == 0 && A.imag == 0) {
                if (B == 0) return NaN; 
                return ( fcomplex )0;
            } else {
                if (false)
                {
                    if (IsInfinity(A)) {
                        return NaN; 
                    } else {
                        return ( fcomplex )0;
                    }
                }
            }
            fcomplex ret;
            if (B == 0) return INF ;
            ret.real =  (float) (A.real / B);
            ret.imag =  (float) (A.imag / B);
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( fcomplex A,  Int64 B) {
            return (A.real == B && A.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( fcomplex A,  Int64 B) {
            return (A.imag != 0.0) || (A.real != B);
        }
        /// <summary>
        /// Freater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( fcomplex A,  Int64 B) {
            return (A.real > B);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <(  fcomplex A,  Int64 B) {
            return (A.real < B);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( fcomplex A,  Int64 B) {
            return (A.real >= B);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( fcomplex A,  Int64 B) {
            return (A.real <= B);
        }
       
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  fcomplex operator +( fcomplex A,  Int32 B) {
            fcomplex ret;
            ret.real =  (float) (A.real + B);
            ret.imag =  (float) A.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  fcomplex operator -( fcomplex A,  Int32 B) {
            fcomplex ret;
            ret.real =  (float) (A.real - B);
            ret.imag =  (float) A.imag;
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  fcomplex operator *( fcomplex A,  Int32 B) {
            fcomplex ret;
            ret.real =  (float) (A.real * B);
            ret.imag =  (float) (A.imag * B);
            return ret;
        }
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>result</returns>
        public static  fcomplex operator /( fcomplex A,  Int32 B) {
            if (IsNaN(A)) return NaN;
            
            if (A.real == 0 && A.imag == 0) {
                if (B == 0) return NaN; 
                return ( fcomplex )0;
            } else {
                if (false)
                {
                    if (IsInfinity(A)) {
                        return NaN; 
                    } else {
                        return ( fcomplex )0;
                    }
                }
            }
            fcomplex ret;
            if (B == 0) return INF ;
            ret.real =  (float) (A.real / B);
            ret.imag =  (float) (A.imag / B);
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( fcomplex A,  Int32 B) {
            return (A.real == B && A.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( fcomplex A,  Int32 B) {
            return (A.imag != 0.0) || (A.real != B);
        }
        /// <summary>
        /// Freater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( fcomplex A,  Int32 B) {
            return (A.real > B);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <(  fcomplex A,  Int32 B) {
            return (A.real < B);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( fcomplex A,  Int32 B) {
            return (A.real >= B);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( fcomplex A,  Int32 B) {
            return (A.real <= B);
        }
       
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  fcomplex operator +( fcomplex A,  float B) {
            fcomplex ret;
            ret.real =  (float) (A.real + B);
            ret.imag =  (float) A.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  fcomplex operator -( fcomplex A,  float B) {
            fcomplex ret;
            ret.real =  (float) (A.real - B);
            ret.imag =  (float) A.imag;
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  fcomplex operator *( fcomplex A,  float B) {
            fcomplex ret;
            ret.real =  (float) (A.real * B);
            ret.imag =  (float) (A.imag * B);
            return ret;
        }
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>result</returns>
        public static  fcomplex operator /( fcomplex A,  float B) {
            if (IsNaN(A)) return NaN;
            if (float.IsNaN(B)) return NaN;
            if (A.real == 0 && A.imag == 0) {
                if (B == 0) return NaN; 
                return ( fcomplex )0;
            } else {
                if (float.IsInfinity(B))
                {
                    if (IsInfinity(A)) {
                        return NaN; 
                    } else {
                        return ( fcomplex )0;
                    }
                }
            }
            fcomplex ret;
            if (B == 0) return INF ;
            ret.real =  (float) (A.real / B);
            ret.imag =  (float) (A.imag / B);
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( fcomplex A,  float B) {
            return (A.real == B && A.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( fcomplex A,  float B) {
            return (A.imag != 0.0) || (A.real != B);
        }
        /// <summary>
        /// Freater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( fcomplex A,  float B) {
            return (A.real > B);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <(  fcomplex A,  float B) {
            return (A.real < B);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( fcomplex A,  float B) {
            return (A.real >= B);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( fcomplex A,  float B) {
            return (A.real <= B);
        }
       
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  fcomplex operator +( fcomplex A,  byte B) {
            fcomplex ret;
            ret.real =  (float) (A.real + B);
            ret.imag =  (float) A.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  fcomplex operator -( fcomplex A,  byte B) {
            fcomplex ret;
            ret.real =  (float) (A.real - B);
            ret.imag =  (float) A.imag;
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  fcomplex operator *( fcomplex A,  byte B) {
            fcomplex ret;
            ret.real =  (float) (A.real * B);
            ret.imag =  (float) (A.imag * B);
            return ret;
        }
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>result</returns>
        public static  fcomplex operator /( fcomplex A,  byte B) {
            if (IsNaN(A)) return NaN;
            
            if (A.real == 0 && A.imag == 0) {
                if (B == 0) return NaN; 
                return ( fcomplex )0;
            } else {
                if (false)
                {
                    if (IsInfinity(A)) {
                        return NaN; 
                    } else {
                        return ( fcomplex )0;
                    }
                }
            }
            fcomplex ret;
            if (B == 0) return INF ;
            ret.real =  (float) (A.real / B);
            ret.imag =  (float) (A.imag / B);
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( fcomplex A,  byte B) {
            return (A.real == B && A.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( fcomplex A,  byte B) {
            return (A.imag != 0.0) || (A.real != B);
        }
        /// <summary>
        /// Freater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( fcomplex A,  byte B) {
            return (A.real > B);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <(  fcomplex A,  byte B) {
            return (A.real < B);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( fcomplex A,  byte B) {
            return (A.real >= B);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( fcomplex A,  byte B) {
            return (A.real <= B);
        }
       
        /// <summary>
        /// Add two complex numbers
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  fcomplex operator +( fcomplex A,  double B) {
            fcomplex ret;
            ret.real =  (float) (A.real + B);
            ret.imag =  (float) A.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>result</returns>
        public static  fcomplex operator -( fcomplex A,  double B) {
            fcomplex ret;
            ret.real =  (float) (A.real - B);
            ret.imag =  (float) A.imag;
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>result</returns>
        public static  fcomplex operator *( fcomplex A,  double B) {
            fcomplex ret;
            ret.real =  (float) (A.real * B);
            ret.imag =  (float) (A.imag * B);
            return ret;
        }
        /// <summary>
        /// Divide two numbers
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>result</returns>
        public static  fcomplex operator /( fcomplex A,  double B) {
            if (IsNaN(A)) return NaN;
            if (double.IsNaN(B)) return NaN;
            if (A.real == 0 && A.imag == 0) {
                if (B == 0) return NaN; 
                return ( fcomplex )0;
            } else {
                if (double.IsInfinity(B))
                {
                    if (IsInfinity(A)) {
                        return NaN; 
                    } else {
                        return ( fcomplex )0;
                    }
                }
            }
            fcomplex ret;
            if (B == 0) return INF ;
            ret.real =  (float) (A.real / B);
            ret.imag =  (float) (A.imag / B);
            return ret;
        }
        /// <summary>
        /// Equality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>result</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator ==( fcomplex A,  double B) {
            return (A.real == B && A.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( fcomplex A,  double B) {
            return (A.imag != 0.0) || (A.real != B);
        }
        /// <summary>
        /// Freater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( fcomplex A,  double B) {
            return (A.real > B);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <(  fcomplex A,  double B) {
            return (A.real < B);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( fcomplex A,  double B) {
            return (A.real >= B);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( fcomplex A,  double B) {
            return (A.real <= B);
        }

#endregion HYCALPER AUTO GENERATED CODE


        

#region HYCALPER AUTO GENERATED CODE

       
        /// <summary>
        /// Add two complex values
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  fcomplex operator +( Int64 A,  fcomplex B) {
            fcomplex ret; 
            ret.real =  (float) (A + B.real);
            ret.imag =  (float) B.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>Result</returns>
        public static  fcomplex operator -( Int64 A,  fcomplex B) {
            fcomplex ret;
            ret.real =  (float) (A - B.real);
            ret.imag = - (float) B.imag; 
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>Result</returns>
        public static  fcomplex operator *( Int64 A,  fcomplex B) {
            fcomplex ret;
            ret.real =  (float) (A * B.real);
            ret.imag =  (float) (A * B.imag);
            return ret;
        }
        /// <summary>
        /// Divide two values
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>Result</returns>
        public static  fcomplex operator /( Int64 A,  fcomplex B) {
            fcomplex ret; 
            if (A == 0) {
                if (IsInfinity(B)) return NaN; 
            } else {
                if (IsInfinity(B)) return ( fcomplex )0; 
            }
            if (B.real == 0 && B.imag == 0) {
                return INF;
            }
            // this algorithm is taken from [1]. The one described in [2] was not taken. Tests 
            // did not show any advantage when using double precision floating point arithmetic.
            double tmp; 
            if (Math.Abs(B.real) >= Math.Abs(B.imag)) {
                tmp =  (float) (B.imag * (1/B.real)); 
                ret.imag =  (float) (B.real + B.imag*tmp); 
                ret.real =  (float) A/ret.imag; 
                ret.imag = -  (float) (A*tmp)/ret.imag; 
            } else {
                tmp =  (float) (B.real * (1/B.imag));
                ret.imag =  (float) (B.imag + B.real*tmp); 
                ret.real =  (float) (A*tmp)/ret.imag; 
                ret.imag = -  (float) A/ret.imag; 
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
        public static bool operator ==( Int64 A,  fcomplex B) {
            return (B.real == A && B.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( Int64 A,  fcomplex B) {
            return (B.imag != 0.0) || (B.real != A);
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( Int64 A,  fcomplex B) {
            return (A > B.real);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( Int64 A,  fcomplex B) {
            return (A < B.real);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( Int64 A,  fcomplex B) {
            return (A >= B.real);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( Int64 A,  fcomplex B) {
            return (A <= B.real);
        }
       
        /// <summary>
        /// Add two complex values
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  fcomplex operator +( Int32 A,  fcomplex B) {
            fcomplex ret; 
            ret.real =  (float) (A + B.real);
            ret.imag =  (float) B.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>Result</returns>
        public static  fcomplex operator -( Int32 A,  fcomplex B) {
            fcomplex ret;
            ret.real =  (float) (A - B.real);
            ret.imag = - (float) B.imag; 
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>Result</returns>
        public static  fcomplex operator *( Int32 A,  fcomplex B) {
            fcomplex ret;
            ret.real =  (float) (A * B.real);
            ret.imag =  (float) (A * B.imag);
            return ret;
        }
        /// <summary>
        /// Divide two values
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>Result</returns>
        public static  fcomplex operator /( Int32 A,  fcomplex B) {
            fcomplex ret; 
            if (A == 0) {
                if (IsInfinity(B)) return NaN; 
            } else {
                if (IsInfinity(B)) return ( fcomplex )0; 
            }
            if (B.real == 0 && B.imag == 0) {
                return INF;
            }
            // this algorithm is taken from [1]. The one described in [2] was not taken. Tests 
            // did not show any advantage when using double precision floating point arithmetic.
            double tmp; 
            if (Math.Abs(B.real) >= Math.Abs(B.imag)) {
                tmp =  (float) (B.imag * (1/B.real)); 
                ret.imag =  (float) (B.real + B.imag*tmp); 
                ret.real =  (float) A/ret.imag; 
                ret.imag = -  (float) (A*tmp)/ret.imag; 
            } else {
                tmp =  (float) (B.real * (1/B.imag));
                ret.imag =  (float) (B.imag + B.real*tmp); 
                ret.real =  (float) (A*tmp)/ret.imag; 
                ret.imag = -  (float) A/ret.imag; 
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
        public static bool operator ==( Int32 A,  fcomplex B) {
            return (B.real == A && B.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( Int32 A,  fcomplex B) {
            return (B.imag != 0.0) || (B.real != A);
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( Int32 A,  fcomplex B) {
            return (A > B.real);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( Int32 A,  fcomplex B) {
            return (A < B.real);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( Int32 A,  fcomplex B) {
            return (A >= B.real);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( Int32 A,  fcomplex B) {
            return (A <= B.real);
        }
       
        /// <summary>
        /// Add two complex values
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  fcomplex operator +( float A,  fcomplex B) {
            fcomplex ret; 
            ret.real =  (float) (A + B.real);
            ret.imag =  (float) B.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>Result</returns>
        public static  fcomplex operator -( float A,  fcomplex B) {
            fcomplex ret;
            ret.real =  (float) (A - B.real);
            ret.imag = - (float) B.imag; 
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>Result</returns>
        public static  fcomplex operator *( float A,  fcomplex B) {
            fcomplex ret;
            ret.real =  (float) (A * B.real);
            ret.imag =  (float) (A * B.imag);
            return ret;
        }
        /// <summary>
        /// Divide two values
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>Result</returns>
        public static  fcomplex operator /( float A,  fcomplex B) {
            fcomplex ret; 
            if (A == 0) {
                if (IsInfinity(B)) return NaN; 
            } else {
                if (IsInfinity(B)) return ( fcomplex )0; 
            }
            if (B.real == 0 && B.imag == 0) {
                return INF;
            }
            // this algorithm is taken from [1]. The one described in [2] was not taken. Tests 
            // did not show any advantage when using double precision floating point arithmetic.
            double tmp; 
            if (Math.Abs(B.real) >= Math.Abs(B.imag)) {
                tmp =  (float) (B.imag * (1/B.real)); 
                ret.imag =  (float) (B.real + B.imag*tmp); 
                ret.real =  (float) A/ret.imag; 
                ret.imag = -  (float) (A*tmp)/ret.imag; 
            } else {
                tmp =  (float) (B.real * (1/B.imag));
                ret.imag =  (float) (B.imag + B.real*tmp); 
                ret.real =  (float) (A*tmp)/ret.imag; 
                ret.imag = -  (float) A/ret.imag; 
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
        public static bool operator ==( float A,  fcomplex B) {
            return (B.real == A && B.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( float A,  fcomplex B) {
            return (B.imag != 0.0) || (B.real != A);
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( float A,  fcomplex B) {
            return (A > B.real);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( float A,  fcomplex B) {
            return (A < B.real);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( float A,  fcomplex B) {
            return (A >= B.real);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( float A,  fcomplex B) {
            return (A <= B.real);
        }
       
        /// <summary>
        /// Add two complex values
        /// </summary>
        /// <param name="A">First summand</param>
        /// <param name="B">Second summand</param>
        /// <returns>Result</returns>
        public static  fcomplex operator +( byte A,  fcomplex B) {
            fcomplex ret; 
            ret.real =  (float) (A + B.real);
            ret.imag =  (float) B.imag;
            return ret;
        }
        /// <summary>
        /// Subtract two values
        /// </summary>
        /// <param name="A">Minuend</param>
        /// <param name="B">Subtrahend</param>
        /// <returns>Result</returns>
        public static  fcomplex operator -( byte A,  fcomplex B) {
            fcomplex ret;
            ret.real =  (float) (A - B.real);
            ret.imag = - (float) B.imag; 
            return ret;
        }
        /// <summary>
        /// Multiply two values
        /// </summary>
        /// <param name="A">First factor</param>
        /// <param name="B">Second factor</param>
        /// <returns>Result</returns>
        public static  fcomplex operator *( byte A,  fcomplex B) {
            fcomplex ret;
            ret.real =  (float) (A * B.real);
            ret.imag =  (float) (A * B.imag);
            return ret;
        }
        /// <summary>
        /// Divide two values
        /// </summary>
        /// <param name="A">Divident</param>
        /// <param name="B">Divisor</param>
        /// <returns>Result</returns>
        public static  fcomplex operator /( byte A,  fcomplex B) {
            fcomplex ret; 
            if (A == 0) {
                if (IsInfinity(B)) return NaN; 
            } else {
                if (IsInfinity(B)) return ( fcomplex )0; 
            }
            if (B.real == 0 && B.imag == 0) {
                return INF;
            }
            // this algorithm is taken from [1]. The one described in [2] was not taken. Tests 
            // did not show any advantage when using double precision floating point arithmetic.
            double tmp; 
            if (Math.Abs(B.real) >= Math.Abs(B.imag)) {
                tmp =  (float) (B.imag * (1/B.real)); 
                ret.imag =  (float) (B.real + B.imag*tmp); 
                ret.real =  (float) A/ret.imag; 
                ret.imag = -  (float) (A*tmp)/ret.imag; 
            } else {
                tmp =  (float) (B.real * (1/B.imag));
                ret.imag =  (float) (B.imag + B.real*tmp); 
                ret.real =  (float) (A*tmp)/ret.imag; 
                ret.imag = -  (float) A/ret.imag; 
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
        public static bool operator ==( byte A,  fcomplex B) {
            return (B.real == A && B.imag == 0.0);
        }
        /// <summary>
        /// Unequality comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real and imaginary parts of A and B are not equal, false otherwise</returns>
        /// <remarks>Real inputs are converted to a complex number and the result is compared to the complex input.</remarks>
        public static bool operator !=( byte A,  fcomplex B) {
            return (B.imag != 0.0) || (B.real != A);
        }
        /// <summary>
        /// Greater than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator > ( byte A,  fcomplex B) {
            return (A > B.real);
        }
        /// <summary>
        /// Lower than comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator < ( byte A,  fcomplex B) {
            return (A < B.real);
        }
        /// <summary>
        /// Greater than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is greater than real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator >=( byte A,  fcomplex B) {
            return (A >= B.real);
        }
        /// <summary>
        /// Lower than or equal to comparison for complex numbers
        /// </summary>
        /// <param name="A">Left side</param>
        /// <param name="B">Right side</param>
        /// <returns>true if real part of A is lower then real part of B, false otherwise</returns>
        /// <remarks>Only the real parts are compared!</remarks>
        public static bool operator <=( byte A,  fcomplex B) {
            return (A <= B.real);
        }

#endregion HYCALPER AUTO GENERATED CODE

        #region unary minus
        /// <summary>
        /// Unary minus operator
        /// </summary>
        /// <param name="in1">fcomplex input</param>
        /// <returns>fcomplex number similar to in1, having real and imag part negated</returns>
        public static fcomplex operator -( fcomplex in1) {
            fcomplex ret = new fcomplex(); 
            ret.imag = -in1.imag; 
            ret.real = -in1.real; 
            return ret;
        }
        #endregion

        /// <summary>
        /// Magnitude value of float complex number
        /// </summary>
        /// <param name="input">fcomplex number</param>
        /// <returns>Magnitude of input</returns>
        public static float Abs(fcomplex input) {
            return (float) Math.Sqrt ( input.real * input.real + input.imag * input.imag );
        }
        /// <summary>
        /// Angle of complex number
        /// </summary>
        /// <param name="input">fcomplex number to compute angle of</param>
        /// <returns>Angle of input</returns>
        public static double Angle(fcomplex input) {
            return (float) Math.Atan2 ( input.imag, input.real );
        }
        /// <summary>
        /// Arcus cosinus for float complex number
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Arcus cosinus of input</returns>
        /// <remarks>The arcus cosinus of a complex number is computed by
        /// <para>Log(Sqrt(input^2 - 1) + input) * i </para></remarks>
        public static fcomplex Acos(fcomplex input) {
            fcomplex ret = new fcomplex ( 0, -1 );
            return fcomplex.Log ( fcomplex.Sqrt ( input * input - 1 )
                + input ) * ret;
        }
        /// <summary>
        /// Arcus cosinus of real number
        /// </summary>
        /// <param name="input">float input</param>
        /// <returns>Arcus cosinus of input</returns>
        /// <remarks>For input > 1.0, <see cref="ILNumerics.fcomplex.Acos(fcomplex)"/> will be used. </remarks>
        public static fcomplex Acos(float input) {
            if (Math.Abs(input) <= 1.0)
                return new fcomplex((float)Math.Acos(input), 0.0f);
            else {
                return Acos((fcomplex)input);
            }
        }
        /// <summary>
        /// Arcus sinus of real number
        /// </summary>
        /// <param name="input">float input</param>
        /// <returns>Arcus sinus of input</returns>
        /// <remarks>For input > 1.0, <see cref="ILNumerics.fcomplex.Asin(fcomplex)"/> will be used. </remarks>
        public static fcomplex Asin(float input) {
            if (Math.Abs(input) <= 1.0)
                return new fcomplex((float)Math.Asin(input), 0.0f);
            else {
                return Asin((fcomplex)input);
            }
        }
        /// <summary>
        /// Arcus sinus for complex number
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Arcus sinus of input</returns>
        public static fcomplex Asin(fcomplex input) {
            fcomplex ret = Acos ( input );
            ret.real = (float) (Math.PI / 2 - ret.real);
            return ret; 
        }
        /// <summary>
        /// Power of base e for float complex number
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Result of Exp(input)</returns>
        public static fcomplex Exp(fcomplex input) {
            return fcomplex.FromPol ( (float) Math.Exp ( input.real ), input.imag );
        }
        /// <summary>
        /// fcomplex power real exponent
        /// </summary>
        /// <param name="input">Basis </param>
        /// <param name="exponent">Exponent</param>
        /// <returns>New fcomplex number with result</returns>
        public static fcomplex Pow(fcomplex input, double exponent) {
            fcomplex ret = input.Log ();
            ret.imag *= (float) exponent;
            ret.real *= (float) exponent;
            return ret.Exp ();
        }
        /// <summary>
        /// Complex power - real basis, real exponent
        /// </summary>
        /// <param name="basis">Basis</param>
        /// <param name="exponent">Exponent</param>
        /// <returns>fcomplex number.</returns>
        /// <remarks>The result will be a fcomplex number. For negative basis 
        /// the basis will be converted to a fcomplex number and the power 
        /// will be computed in the fcomplex plane.</remarks>
        public static fcomplex Pow(double basis, double exponent) {
            if (basis < 0) {
                return Pow((fcomplex)basis, exponent);
            } else {
                return (fcomplex)Math.Pow(basis, exponent);
            }
        }
        /// <summary>
        /// Power: complex base, complex exponent
        /// </summary>
        /// <param name="basis">Basis</param>
        /// <param name="exponent">Exponent</param>
        /// <returns>result of basis^exponent</returns>
        public static fcomplex Pow(fcomplex basis, fcomplex exponent) {
            fcomplex ret = ( basis.Log () * exponent );
            return ret.Exp ();
        }
        /// <summary>
        /// Square root of real input
        /// </summary>
        /// <param name="input">float input</param>
        /// <returns>Square root of input</returns>
        public static fcomplex Sqrt(float input) {
            if (input > 0)
                return new fcomplex((float)Math.Sqrt(input), 0.0f);
            else
                return Sqrt((fcomplex)input); 
        }
        /// <summary>
        /// Square root of complex number
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Square root of input</returns>
        public static fcomplex Sqrt(fcomplex input) {
            // Reference : numerical recipes in C: Appendix C
            fcomplex ret = new fcomplex ();
            double x, y, w, r;
            if (input.real == 0.0 && input.imag == 0.0)
                return ret;
            else {
                x = (float) Math.Abs ( input.real );
                y = (float) Math.Abs ( input.imag );
                if (x >= y) {
                    r = y / x;
                    w = Math.Sqrt ( x ) * Math.Sqrt ( 0.5 * ( 1.0 + Math.Sqrt ( 1.0 + r * r ) ) );
                } else {
                    r = x / y;
                    w = Math.Sqrt ( y ) * Math.Sqrt ( 0.5 * ( r + Math.Sqrt ( 1.0 + r * r ) ) );
                }
                if (input.real >= 0.0) {
                    ret.real = (float) w;
                    ret.imag = (float) (input.imag / ( 2.0 * w ));
                } else {
                    ret.imag = (float) (( input.imag >= 0 ) ? w : -w);
                    ret.real = (float) (input.imag / ( 2.0 * ret.imag ));
                }
                return ret;
            }
        }
        /// <summary>
        /// Tangens of float complex number
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Tangens of input</returns>
        public static fcomplex Tan(fcomplex input) {
            fcomplex ci = Cos(input);
            if (ci.real == (float)0.0 && ci.imag == (float)0.0)
                return INF;
            return (Sin(input) / ci);
        }
        /// <summary>
        /// Tangens hyperbolicus of float complex input
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Tangens hyperbolicus</returns>
        public static fcomplex Tanh(fcomplex input) {
            fcomplex si = Sin(input);
            if (si.real == (float)0.0 && si.imag == (float)0.0)
                return INF;
            return (Cos(input) / si);
        }
        /// <summary>
        /// Logarithm of complex input
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Logarithm of input</returns>
        public static fcomplex Log(fcomplex input) {
            fcomplex ret = new fcomplex ();
            ret.real = (float) Math.Log ( Math.Sqrt ( input.real * input.real + input.imag * input.imag ) );
            ret.imag = (float) Math.Atan2 ( input.imag, input.real );
            return ret;
        }
        /// <summary>
        /// Logarithm to base 10
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Logarithm of input</returns>
        public static fcomplex Log10(fcomplex input) {
            return Log(input) / 2.30258509299405f;
        }
        /// <summary>
        /// Logarithm of base 2
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Logarithm of input</returns>
        public static fcomplex Log2(fcomplex input) {
            return Log(input) / 0.693147180559945f;
        }
        /// <summary>
        /// Logarithm of real input 
        /// </summary>
        /// <param name="input">float input - may be negative</param>
        /// <returns>Complex logarithm</returns>
        public static fcomplex Log(float input) {
            return Log (new fcomplex(input,0.0f)); 
        }
        /// <summary>
        /// Logarithm of base 10 of real input 
        /// </summary>
        /// <param name="input">float input - may be negative</param>
        /// <returns>Complex logarithm of base 10</returns>
        public static fcomplex Log10(float input) {
            return Log(new fcomplex(input,0.0f)) / 2.30258509299405f;
        }
        /// <summary>
        /// Logarithm of base 2
        /// </summary>
        /// <param name="input">float input - may be negative</param>
        /// <returns>Complex logarithm of base 2</returns>
        public static fcomplex Log2(float input) {
            return Log(new fcomplex(input,0.0f)) / 0.693147180559945f;
        }
        /// <summary>
        /// Convert from polar to cartesian form
        /// </summary>
        /// <param name="magnitude">Magnitude</param>
        /// <param name="angle">Angle</param>
        /// <returns>fcomplex number with magnitude <c>magnitude</c> 
        /// and phase <c>angle</c></returns>
        public static fcomplex FromPol(float magnitude, float angle) {
            return new fcomplex (
                (magnitude * (float)Math.Cos ( angle )),
                (magnitude * (float)Math.Sin ( angle ))
            );
        }
        /// <summary>
        /// Convert this float complex number to string 
        /// </summary>
        /// <returns>String representation of this float complex number</returns>
        public override String ToString() {
            if (imag>=0)
                return String.Format("{0} + {1}i",real,imag);
            else 
                return String.Format("{0} {1}i",real,imag);
        }
        private static string m_precSpecI = ""; 
        private static string m_precSpecR = ""; 
        private static int m_lastDigits = 0; 
        /// <summary>
        /// Print formated output of this number, determine number of digits
        /// </summary>
        /// <param name="digits">Number of digits</param>
        /// <returns>Formatted output</returns>
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
                return String.Format(m_precSpecR+m_precSpecI,real,imag); 
            }
        }
        /// <summary>
        /// Magnitude of this float complex number
        /// </summary>
        /// <returns>Magnitude</returns>
        public float Abs() {
            return (float)Math.Sqrt(real * real + imag * imag);
        }
        /// <summary>
        /// Phase angle of this float complex number
        /// </summary>
        /// <returns>Phase angle </returns>
        public double Angle() {
            return (float)Math.Atan2(imag, real);
        }
        /// <summary>
        /// Arcus cosinus of this float complex number
        /// </summary>
        /// <returns>Arcus cosinus</returns>
        public fcomplex Acos() {
            fcomplex ret = new fcomplex(0, -1);
            return fcomplex.Log(fcomplex.Sqrt(this * this - 1)
                + this) * ret;
        }
        /// <summary>
        /// Arcus sinus of this float complex number
        /// </summary>
        /// <returns>Arcus sinus</returns>
        public fcomplex Asin() {
            fcomplex ret = Acos(this);
            ret.real = (float)(Math.PI / 2 - ret.real);
            return ret;
        }
        /// <summary>
        /// Arcus tangens of float complex number
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Arcus tangens of input</returns>
        public static fcomplex Atan(fcomplex input) {
            fcomplex ret = new fcomplex(0, (float)0.5);
            return (ret * Log((fcomplex.i + input) / (fcomplex.i - input)));
        }
        /// <summary>
        /// Round towards next greater integer
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Rounded float complex number</returns>
        /// <remarks>Real and imaginary parts are independently rounded 
        /// towards the next integer value towards positive infinity.</remarks>
        public static fcomplex Ceiling (fcomplex input){
            return new fcomplex(
                    (float)Math.Ceiling(input.real),
                    (float)Math.Ceiling(input.imag)
            );
        }
        /// <summary>
        /// Round towards next lower integer
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Rounded float complex number</returns>
        /// <remarks>Real and imaginary parts are independently rounded 
        /// towards the next integer value towards negative infinity.</remarks>
        public static fcomplex Floor (fcomplex input){
            return new fcomplex(
                    (float)Math.Floor(input.real),
                    (float)Math.Floor(input.imag)
            );
        }
        /// <summary>
        /// Round mercantilistic
        /// </summary>
        /// <param name="input">fcomplex number</param>
        /// <returns>Rounded number</returns>
        /// <remarks>Real and imaginaty parts are rounded independently. </remarks>
        public static fcomplex Round (fcomplex input){
            return new fcomplex(
                    (float)Math.Round(input.real),
                    (float)Math.Round(input.imag)
            );
        }
        /// <summary>
        /// Signum function
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns> Signum of input</returns>
        /// <remarks>
        /// For numbers a = 0.0 + 0.0i, sign(a)'s real and imag parts are 0.0. 
        /// For all other numbers sign(a) is the projection onto the unit circle.</remarks>
        public static fcomplex Sign(fcomplex input){
            if (input.real == 0.0 && input.imag == 0.0)
                return new fcomplex(); 
            else {
                float mag = (float)Math.Sqrt(input.real * input.real + input.imag * input.imag); 
                return new fcomplex(
                    input.real / mag,
                    input.imag / mag);
            }
        }
        /// <summary>
        /// Truncate a floating point complex value
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Integer part of input</returns>
        /// <remarks>Operates on real and imaginary parts seperately.</remarks>
        public static fcomplex Truncate (fcomplex input){
            return new fcomplex(
                    (float)Math.Truncate(input.real),
                    (float)Math.Truncate(input.imag)
            );
        }
        /// <summary>
        /// Cosinus
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Cosinus of input</returns>
        /// <remarks><para>The cosinus is computed by the trigonometric euler equation: </para>
        /// <para>0.5 * [exp(i input) + exp(-i input)]</para></remarks>
        public static fcomplex Cos(fcomplex input) {
            fcomplex i = new fcomplex(0, 1.0f);
            fcomplex ni = new fcomplex(0, -1.0f);
            return (Exp(i * input) + Exp(ni * input)) / 2.0f;
        }
        /// <summary>
        /// Cosinus hyperbolicus
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Cosinus hyperbolicus of input</returns>
        /// <remarks><para>The cosinus is computed by the trigonometric euler equation: </para>
        /// <para>(Exp(input) + Exp(-1.0 * input)) / 2.0</para></remarks>
        public static fcomplex Cosh(fcomplex input) {
            return (Exp(input) + Exp(-1.0f * input)) / 2.0f;
        }
        /// <summary>
        /// Sinus
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Sinus of input</returns>
        /// <remarks><para>The sinus is computed by the trigonometric euler equation: </para>
        /// <para>(Exp(i * input) - Exp(-1.0 * i * input)) / (2.0 * i)</para></remarks>
        public static fcomplex Sin(fcomplex input) {
            fcomplex i = new fcomplex(0, (float)1.0);
            fcomplex mi = new fcomplex(0, (float)-1.0);
            return (Exp(i * input) - Exp(mi * input)) / (2.0 * i);
        }
        /// <summary>
        /// Sinus hyperbolicus
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>Sinus hyperbolicus of input</returns>
        /// <remarks><para>The sinus hyperbolicus is computed by the trigonometric euler equation: </para>
        /// <para>(Exp(input) - Exp(-1.0 * input)) / 2.0</para></remarks>
        public static fcomplex Sinh(fcomplex input) {
            fcomplex ret = new fcomplex(0, 2);
            fcomplex i = new fcomplex(0, (float)1.0);
            fcomplex mi = new fcomplex(0, (float)-1.0);
            return (Exp(input) - Exp(-1.0 * input)) / 2.0;
        }
        /// <summary>
        /// Exponential / power of base e
        /// </summary>
        /// <returns>Power of base e</returns>
        public fcomplex Exp() {
            return fcomplex.FromPol((float)Math.Exp(real), imag);
        }
        /// <summary>
        /// Power of fcomplex number, real exponent
        /// </summary>
        /// <param name="exponent">Exponent</param>
        /// <returns>New fcomplex number with result</returns>
        public fcomplex Pow(double exponent) {
            fcomplex ret = Log();
            ret.imag *= (float)exponent;
            ret.real *= (float)exponent;
            return ret.Exp();
        }
        /// <summary>
        /// Power of fcomplex number, complex exponent
        /// </summary>
        /// <param name="exponent">Exponent</param>
        /// <returns>New fcomplex number with result</returns>
        public fcomplex Pow(fcomplex exponent) {
            fcomplex ret = (Log() * exponent);
            return ret.Exp();
        }
        /// <summary>
        /// Square root of fcomplex number
        /// </summary>
        /// <returns>Square root</returns>
        public fcomplex Sqrt() {
            // Reference : numerical recipes in C: Appendix C
            fcomplex ret = new fcomplex();
            double x, y, w, r;
            if ( real == 0.0 && imag == 0.0)
                return ret;
            else {
                x = (float)Math.Abs(real);
                y = (float)Math.Abs( imag);
                if (x >= y) {
                    r = y / x;
                    w = Math.Sqrt(x) * Math.Sqrt(0.5 * (1.0 + Math.Sqrt(1.0 + r * r)));
                } else {
                    r = x / y;
                    w = Math.Sqrt(y) * Math.Sqrt(0.5 * (r + Math.Sqrt(1.0 + r * r)));
                }
                if ( real >= 0.0) {
                    ret.real = (float)w;
                    ret.imag = (float)( imag / (2.0 * w));
                } else {
                    ret.imag = (float)(( imag >= 0) ? w : -w);
                    ret.real = (float)( imag / (2.0 * ret.imag));
                }
                return ret;
            }
        }
        /// <summary>
        /// Logarithm of fcomplex number
        /// </summary>
        /// <returns>Natural logarithm</returns>
        /// <remarks>The logarithm of a complex number A is defined as follows: <br />
        /// <list type="none"><item>real part: log(abs(A))</item>
        /// <item>imag part: Atan2(imag(A),real(A))</item></list>
        /// </remarks>
        public fcomplex Log() {
            fcomplex ret = new fcomplex();
            ret.real = (float)Math.Log(Math.Sqrt( real *  real +  imag *  imag));
            ret.imag = (float)Math.Atan2( imag,  real);
            return ret;
        }
        /// <summary>
        /// Test if any of real or imaginary parts are NAN's
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>true if any of real or imag part is not a number</returns>
        public static bool IsNaN(fcomplex input) {
            if (Single.IsNaN(input.real) || Single.IsNaN(input.imag)) 
                return true; 
            else 
                return false; 
        }
        /// <summary>
        /// Test if any of real or imaginary parts are infinite
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>true if any of real or imag part is infinite</returns>
        public static bool IsInfinity(fcomplex input) {  
            if (Single.IsInfinity(input.real) || Single.IsInfinity(input.imag)) 
                return true; 
            else 
                return false; 
        }
        /// <summary>
        /// Test if any of real or imaginary parts are pos. infinite
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>true if any of real or imag part is positive infinite</returns>
        public static bool IsPositiveInfinity(fcomplex input) {  
            if (Single.IsPositiveInfinity(input.real) || Single.IsPositiveInfinity(input.imag)) 
                return true; 
            else 
                return false; 
        }
        /// <summary>
        /// Test if any of real or imaginary parts are neg. infinite
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>true if any of real or imag part is negative infinite</returns>
        public static bool IsNegativeInfinity(fcomplex input) {  
            if (Single.IsNegativeInfinity(input.real) || Single.IsNegativeInfinity(input.imag)) 
                return true; 
            else 
                return false; 
        }
        /// <summary>
        /// Test if any of real or imaginary parts are finite
        /// </summary>
        /// <param name="input">fcomplex input</param>
        /// <returns>true if any of real and imag part is finite</returns>
        public static bool IsFinite (fcomplex input) {
            if (ILMath.isfinite(input.real) && ILMath.isfinite(input.imag)) 
                return true; 
            else 
                return false; 
        }

        #region CAST_OPERATORS
        /// <summary>
        /// Implicit cast real number into complex number
        /// </summary>
        /// <param name="a">double</param>
        /// <returns>fcomplex number with real part equals a</returns>
        public static implicit operator fcomplex(double a) {
            return new fcomplex((float)a, 0.0F);
        }
        /// <summary>
        /// Implicit cast real number into complex number
        /// </summary>
        /// <param name="a">float</param>
        /// <returns>fcomplex number with real part equals a</returns>
        public static implicit operator fcomplex(float a) {
            return new fcomplex(a, 0.0F);
        }
        /// <summary>
        /// Implicit cast real number into complex number
        /// </summary>
        /// <param name="a">byte</param>
        /// <returns>fcomplex number with real part equals a</returns>
        public static implicit operator fcomplex(byte a) {
            return new fcomplex(a, 0.0F);
        }
        /// <summary>
        /// Implicit cast real number into complex number
        /// </summary>
        /// <param name="a">char</param>
        /// <returns>fcomplex number with real part equals a</returns>
        public static implicit operator fcomplex(char a) {
            return new fcomplex(a, 0.0F);
        }
        /// <summary>
        /// Implicit cast real number into complex number
        /// </summary>
        /// <param name="a">Int16</param>
        /// <returns>fcomplex number with real part equals a</returns>
        public static implicit operator fcomplex(Int16 a) {
            return new fcomplex(a, 0.0F);
        }
        /// <summary>
        /// Implicit cast real number into complex number
        /// </summary>
        /// <param name="a">Int32</param>
        /// <returns>fcomplex number with real part equals a</returns>
        public static implicit operator fcomplex(Int32 a) {
            return new fcomplex((float)a, 0.0F);    
        }
        /// <summary>
        /// Implicit cast real number into complex number
        /// </summary>
        /// <param name="a">Int64</param>
        /// <returns>fcomplex number with real part equals a</returns>
        public static implicit operator fcomplex(Int64 a) {
            return new fcomplex((float)a, 0.0F);
        }
        /// <summary>
        /// Implicit cast real number into complex number
        /// </summary>
        /// <param name="a">UInt16</param>
        /// <returns>fcomplex number with real part equals a</returns>
        public static implicit operator fcomplex(UInt16 a) {
            return new fcomplex((float)a, 0.0F);
        }
        /// <summary>
        /// Implicit cast real number into complex number
        /// </summary>
        /// <param name="a">UInt32</param>
        /// <returns>fcomplex number with real part equals a</returns>
        public static implicit operator fcomplex(UInt32 a) {
            return new fcomplex((float)a, 0.0F);
        }
        /// <summary>
        /// Implicit cast real number into complex number
        /// </summary>
        /// <param name="a">UInt64</param>
        /// <returns>fcomplex number with real part equals a</returns>
        public static implicit operator fcomplex(UInt64 a) {
            return new fcomplex((float)a, 0.0F);
        }

        /// <summary>
        /// Explicit cast complex number into real number
        /// </summary>
        /// <param name="a">fcomplex number</param>
        /// <returns>Real number with real part of a</returns>
        public static explicit operator double(fcomplex a) {
            return a.real; 
        }
        /// <summary>
        /// Explicit cast complex number into real number
        /// </summary>
        /// <param name="a">fcomplex number</param>
        /// <returns>Real number with real part of a</returns>
        public static explicit operator float(fcomplex a) {
            return (float)a.real;
        }
        /// <summary>
        /// Explicit cast complex number into real number
        /// </summary>
        /// <param name="a">fcomplex number</param>
        /// <returns>Real number with real part of a</returns>
        public static explicit operator byte(fcomplex a) {
            return (byte) a.real; 
        }
        /// <summary>
        /// Explicit cast complex number into real number
        /// </summary>
        /// <param name="a">fcomplex number</param>
        /// <returns>Real number with real part of a</returns>
        public static explicit operator char(fcomplex a) {
            return (char) a.real; 
        }
        /// <summary>
        /// Explicit cast complex number into real number
        /// </summary>
        /// <param name="a">fcomplex number</param>
        /// <returns>Real number with real part of a</returns>
        public static explicit operator Int16(fcomplex a) {
            return (Int16) a.real; 
        }
        /// <summary>
        /// Explicit cast complex number into real number
        /// </summary>
        /// <param name="a">complex number</param>
        /// <returns>Real number with real part of a</returns>
        public static explicit operator Int32(fcomplex a) {
            return (Int32) a.real; 
        }
        /// <summary>
        /// Explicit cast complex number into real number
        /// </summary>
        /// <param name="a">fcomplex number</param>
        /// <returns>Real number with real part of a</returns>
        public static explicit operator Int64(fcomplex a) {
            return (Int64) a.real; 
        }
        /// <summary>
        /// Explicit cast complex number into real number
        /// </summary>
        /// <param name="a">fcomplex number</param>
        /// <returns>Real number with real part of a</returns>
        public static explicit operator UInt16(fcomplex a) {
            return (UInt16) a.real; 
        }
        /// <summary>
        /// Explicit cast complex number into real number
        /// </summary>
        /// <param name="a">fcomplex number</param>
        /// <returns>Real number with real part of a</returns>
        public static explicit operator UInt32(fcomplex a) {
            return (UInt32) a.real; 
        }
        /// <summary>
        /// Explicit cast complex number into real number
        /// </summary>
        /// <param name="a">fcomplex number</param>
        /// <returns>Real number with real part of a</returns>
        public static explicit operator UInt64(fcomplex a) {
            return (UInt64) a.real; 
        }
        /// <summary>
        /// Test if real and imag part are zero
        /// </summary>
        /// <returns>true if real and imag parts are zero, false else</returns>
        public bool iszero() {
            if (real == 0.0f && imag == 0.0f) 
                return true; 
            else 
                return false; 
        }
        #endregion CAST_OPERATORS

    }
    
}
