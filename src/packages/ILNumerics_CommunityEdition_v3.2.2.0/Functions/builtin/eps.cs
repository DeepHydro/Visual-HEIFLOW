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

namespace ILNumerics {

    public partial class ILMath {
        /// <summary>
        /// Double precision epsilon - the smallest difference from 1.0
        /// </summary>
        public static double eps {
            get {
                return MachineParameterDouble.eps;
            }
        }
        /// <summary>
        /// Single precision epsilon - the smalles difference from 1.0f
        /// </summary>
        public static float epsf {
            get {
                return MachineParameterSingle.eps;
            }
        }
    }

    /// <summary>
    /// Extensive numerical machine parameter infos - single precision
    /// </summary>
    public struct MachineParameterSingle {
        /// <summary>
        /// Radix
        /// </summary>
        public int ibeta; 
        /// <summary>
        /// Number of base digits(bits) in the mantissa
        /// </summary>
        public int it;
        /// <summary>
        /// Rounding and underflow information.  
        /// </summary>
        /// <remarks><list type="bullet"><listheader>Rounding properties</listheader>
        /// <item>2,5: IEEE rounding conform </item>
        /// <item>1,4: not IEEE conform rounding </item>
        /// <item>0,3: truncating - no rounding </item></list>
        /// <list type="bullet"><listheader>Under-/ Overflow. numbers below xmin will be interpreted as... </listheader>
        /// <item>0,1,2: zero</item>
        /// <item>3,4,5: xmin (IEEE conform)</item></list></remarks>
        public int irnd; 
        /// <summary>
        /// Number of guard digits in the product of 2 mantissas
        /// </summary>
        public int ngrd; 
        /// <summary>
        /// Exponent of the smalles number ibeta^machep &gt; 1.0
        /// </summary>
        public int machep; 
        /// <summary>
        /// Exponent of smallest number ibeta^negep wich may be substracted from 1.0, giving a result not equal to 1.0
        /// </summary>
        public int negep; 
        /// <summary>
        /// Number of exponent bits 
        /// </summary>
        public int iexp; 
        /// <summary>
        /// Smallest power of ibeta without leading zeros in the mantissa
        /// </summary>
        public int minexp; 
        /// <summary>
        /// Smallest power of ibeta where overflow occours
        /// </summary>
        public int maxexp; 
        /// <summary>
        /// Distance between the smallest number &gt; 1.0, distinguishable from 1.0 and 1.0
        /// </summary>
        /// <remarks>This number is computed by ibeta <sup>machep</sup>.</remarks>
        public float eps; 
        /// <summary>
        /// Alternative eps. ibeta <sup>negep</sup>
        /// </summary>
        public float epsneg; 
        /// <summary>
        /// Smallest floating point number
        /// </summary>
        public float xmin; 
        /// <summary>
        /// Largest floating point number
        /// </summary>
        public float xmax;
    }
    /// <summary>
    /// Extensive numerical machine parameter infos - double precision
    /// </summary>
    public struct MachineParameterDouble {
        /// <summary>
        /// Radix
        /// </summary>
        public int ibeta; 
        /// <summary>
        /// Number of base digits(bits) in the mantissa
        /// </summary>
        public int it;
        /// <summary>
        /// Rounding and underflow information.  
        /// </summary>
        /// <remarks><list type="bullet"><listheader>Rounding properties</listheader>
        /// <item>2,5: IEEE rounding conform </item>
        /// <item>1,4: not IEEE conform rounding </item>
        /// <item>0,3: truncating - no rounding </item></list>
        /// <list type="bullet"><listheader>Under-/ Overflow. numbers below xmin will be interpreted as... </listheader>
        /// <item>0,1,2: zero</item>
        /// <item>3,4,5: xmin (IEEE conform)</item></list></remarks>
        public int irnd; 
        /// <summary>
        /// Number of guard digits in the product of 2 mantissas
        /// </summary>
        public int ngrd; 
        /// <summary>
        /// Exponent of the smalles number ibeta^machep &gt; 1.0
        /// </summary>
        public int machep; 
        /// <summary>
        /// Exponent of smallest number ibeta^negep wich may be substracted from 1.0, giving a result not equal to 1.0
        /// </summary>
        public int negep; 
        /// <summary>
        /// Number of exponent bits 
        /// </summary>
        public int iexp; 
        /// <summary>
        /// Smallest power of ibeta without leading zeros in the mantissa
        /// </summary>
        public int minexp; 
        /// <summary>
        /// Smallest power of ibeta where overflow occours
        /// </summary>
        public int maxexp; 
        /// <summary>
        /// Distance between the smallest number &gt; 1.0, distinguishable from 1.0 and 1.0
        /// </summary>
        /// <remarks>This number is computed by ibeta <sup>machep</sup>.</remarks>
        public double eps; 
        /// <summary>
        /// Alternative eps. ibeta <sup>negep</sup>
        /// </summary>
        public double epsneg; 
        /// <summary>
        /// Smallest floating point number
        /// </summary>
        public double xmin; 
        /// <summary>
        /// Largest floating point number
        /// </summary>
        public double xmax;
    }
}


namespace ILNumerics  {
    public partial class ILMath {

        /// <summary>
        /// Definition of pi
        /// </summary>
        /// <remarks>This is an convenience alias for Math.PI</remarks>
        public static readonly double pi = Math.PI;

        /// <summary>
        /// Definition of pi, single precision
        /// </summary>
        /// <remarks>This is an convenience alias for Math.PI</remarks>
        public static readonly float pif = (float)Math.PI; 

        private static MachineParameterDouble m_machparDouble; 
        /// <summary>
        /// Give extensive numerical machine parameter informations - double precision
        /// </summary>
        public static MachineParameterDouble MachineParameterDouble {
            get {
                return m_machparDouble;
            }
        }
        private static MachineParameterSingle m_machparSingle; 
        /// <summary>
        /// Give extensive numerical machine parameter informations - single precision
        /// </summary>
        public static MachineParameterSingle MachineParameterSingle {
            get {
                return m_machparSingle; 
            }
        }
        /// <summary>
        /// Prevent JIT "optimizations" - force single precision to be applied
        /// </summary>
        /// <typeparam name="T">mainly float here</typeparam>
        private class precisionHelper<T> { 
            public T V;
        }
        /// <summary>
        /// Determine machine specific parameter
        /// </summary>
        /// <remarks>Source: Numerical Recipes in C, p.892</remarks>
        internal static void macharF(ref int ibeta, ref int it, ref int irnd, ref int ngrd, ref int machep, ref int negep,
                        ref int iexp, ref int minexp, ref int maxexp, ref float eps, ref float epsneg, ref float xmin, ref float xmax) {
            int i, itemp, iz, j, k, mx, nxres; 
            precisionHelper<float> a = new precisionHelper<float>(), 
                b = new precisionHelper<float>(),
                beta = new precisionHelper<float>(),
                betah = new precisionHelper<float>(),
                betain = new precisionHelper<float>(),
                one = new precisionHelper<float>(),
                t = new precisionHelper<float>(),
                temp = new precisionHelper<float>(),
                temp1 = new precisionHelper<float>(),
                tempa = new precisionHelper<float>(),
                two = new precisionHelper<float>(),
                y = new precisionHelper<float>(),
                z = new precisionHelper<float>(),
                zero = new precisionHelper<float>();

            one.V = 1f;
            two.V = one.V + one.V;
            zero.V = one.V - one.V;
            a.V = one.V; 
            do {
                a.V += a.V;
                temp.V = a.V + one.V;
                temp1.V = temp.V - a.V;
            } while (temp1.V - one.V == zero.V);
            b.V = one.V; 
            do {
                b.V += b.V;
                temp.V = a.V + b.V;
                itemp = (int)(temp.V - a.V); 
            } while (itemp == 0); 
            ibeta = itemp;
            beta.V = (float)ibeta; 
            it = 0; 
            b.V = one.V; 
            do {
                ++it;
                b.V *= beta.V;
                temp.V = b.V + one.V;
                temp1.V = temp.V - b.V;
            } while (temp1.V - one.V == zero.V);
            irnd = 0;
            betah.V = beta.V / two.V; 
            temp.V = a.V + betah.V; 
            if (temp.V-a.V != zero.V) irnd = 1; 
            tempa.V = a.V + beta.V; 
            temp.V = tempa.V + betah.V; 
            if (irnd == 0 && temp.V - tempa.V != zero.V) irnd = 2; 
            negep = it + 3; 
            betain.V = one.V / beta.V; 
            a.V = one.V; 
            for (i = 1; i <= negep; i++) a.V *= betain.V; 
            b.V = a.V; 
            for (;;) {
                temp.V = one.V -a.V; 
                if (temp.V-one.V != zero.V)  break; 
                a.V *= beta.V; 
                --negep; 
            } 
            negep = -negep; 
            epsneg = a.V; 
            machep = -it -3; 
            a.V = b.V; 
            for (;;) {
                temp.V = one.V +a.V; 
                if (temp.V - one.V != zero.V) break; 
                a.V *= beta.V; 
                ++machep; 
            }
            eps = a.V; 
            ngrd =0; 
            temp.V = one.V + eps;
            if (irnd == 0 && temp.V * one.V - one.V != zero.V) ngrd = 1;
            i = 0; 
            k = 1; 
            z.V = betain.V; 
            t.V = one.V + eps; 
            nxres = 0; 
            for (;;) {
                y.V = z.V; 
                z.V = y.V *y.V;
                a.V = z.V * one.V; 
                temp.V = z.V * t.V; 
                if (a.V + a.V == zero.V || (float)Math.Abs(z.V) >= y.V) break; 
                temp1.V = temp.V * betain.V; 
                if (temp1.V * beta.V == z.V) break; 
                ++i; 
                k += k; 
            }
            if (ibeta != 10) {
                iexp = i+1; 
                mx = k + k; 
            }else {
                iexp = 2; 
                iz = ibeta; 
                while (k >= iz) {
                    iz *= ibeta; 
                    ++iexp; 
                }
                mx = iz + iz-1; 
            }
            for (;;) {
                xmin = y.V; 
                y.V *= betain.V;
                a.V = y.V * one.V; 
                temp.V = y.V * t.V; 
                if (a.V+a.V != zero.V && (float)Math.Abs(y.V) < xmin) {
                    ++k; 
                    temp1.V = temp.V* betain.V; 
                    if (temp1.V * beta.V == y.V && temp.V != y.V) {
                        nxres = 3; 
                        xmin = y.V; 
                        break; 
                    }
                }
                else break; 
            }
            minexp = -k; 
            if (mx <= k + k - 3 && ibeta != 10) {
                mx += mx; 
                ++iexp; 
            }
            maxexp = mx + minexp; 
            irnd += nxres; 
            if (irnd >= 2) maxexp -= 2; 
            i = maxexp + minexp; 
            if (ibeta == 2 && i != 0) --maxexp; 
            if (i > 20) --maxexp; 
            if (a.V != y.V) maxexp -= 2;
            xmax = one.V -epsneg; 
            if (maxexp * one.V != xmax) xmax = one.V - beta.V * epsneg; 
            xmax /= xmin * beta.V * beta.V * beta.V; 
            i = maxexp + minexp + 3; 
            for (j = 1; j <= i; j++){ 
                if (ibeta == 2) xmax += xmax; 
                else xmax *= beta.V; 
            }
        }

        /// <summary>
        /// Determine machine specific parameter (double precision)
        /// </summary>
        /// <remarks>Source: Numerical Recipes in C, p.892</remarks>
        internal static void macharD(ref int ibeta, ref int it, ref int irnd, ref int ngrd, ref int machep, ref int negep,
                        ref int iexp, ref int minexp, ref int maxexp, ref double eps, ref double epsneg, ref double xmin, ref double xmax) {
            int i, itemp, iz, j, k, mx, nxres; 
            double a,b,beta,betah,betain,one,t,temp,temp1,tempa,two,y,z,zero;

            one= (double)1; 
            two = one + one; 
            zero = one - one; 
            a = one; 
            do {
                a += a; 
                temp = a+one; 
                temp1= temp-a; 
            } while (temp1-one == zero); 
            b = one; 
            do {
                b += b; 
                temp=a+b; 
                itemp=(int)(temp-a); 
            } while (itemp == 0); 
            ibeta = itemp; 
            beta = (double)ibeta; 
            it = 0; 
            b = one; 
            do {
                ++it; 
                b *= beta;
                temp = b +one; 
                temp1 = temp -b; 
            }while (temp1-one == zero);
            irnd = 0; 
            betah = beta / two; 
            temp = a + betah; 
            if (temp-a != zero) irnd = 1; 
            tempa = a + beta; 
            temp = tempa + betah; 
            if (irnd == 0 && temp - tempa != zero) irnd = 2; 
            negep = it + 3; 
            betain = one / beta; 
            a = one; 
            for (i = 1; i <= negep; i++) a *= betain; 
            b =a; 
            for (;;) {
                temp = one -a; 
                if (temp-one != zero)  break; 
                a *= beta; 
                --negep; 
            } 
            negep = -negep; 
            epsneg = a; 
            machep = -it -3; 
            a = b; 
            for (;;) {
                temp = one +a; 
                if (temp - one != zero) break; 
                a *= beta; 
                ++machep; 
            }
            eps = a; 
            ngrd =0; 
            temp = one + eps;
            if (irnd == 0 && temp * one - one != zero) ngrd = 1;
            i = 0; 
            k = 1; 
            z = betain; 
            t = one + eps; 
            nxres = 0; 
            for (;;) {
                y = z; 
                z = y *y;
                a = z * one; 
                temp = z * t; 
                if (a + a == zero || (double)Math.Abs(z) >= y) break; 
                temp1 = temp * betain; 
                if (temp1 * beta == z) break; 
                ++i; 
                k += k; 
            }
            if (ibeta != 10) {
                iexp = i+1; 
                mx = k + k; 
            }else {
                iexp = 2; 
                iz = ibeta; 
                while (k >= iz) {
                    iz *= ibeta; 
                    ++iexp; 
                }
                mx = iz + iz-1; 
            }
            for (;;) {
                xmin = y; 
                y *= betain;
                a = y * one; 
                temp = y * t; 
                if (a+a != zero && (double)Math.Abs(y) < xmin) {
                    ++k; 
                    temp1 = temp* betain; 
                    if (temp1 * beta == y && temp != y) {
                        nxres = 3; 
                        xmin = y; 
                        break; 
                    }
                }
                else break; 
            }
            minexp = -k; 
            if (mx <= k + k - 3 && ibeta != 10) {
                mx += mx; 
                ++iexp; 
            }
            maxexp = mx + minexp; 
            irnd += nxres; 
            if (irnd >= 2) maxexp -= 2; 
            i = maxexp + minexp; 
            if (ibeta == 2 && i != 0) --maxexp; 
            if (i > 20) --maxexp; 
            if (a != y) maxexp -= 2;
            xmax = one -epsneg; 
            if (maxexp * one != xmax) xmax = one - beta * epsneg; 
            xmax /= xmin * beta * beta * beta; 
            i = maxexp + minexp + 3; 
            for (j = 1; j <= i; j++){ 
                if (ibeta == 2) xmax += xmax; 
                else xmax *= beta; 
            }
        }

    }
}
