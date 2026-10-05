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

namespace ILNumerics {
    /// <summary>
    /// A helper class that can be used to generate various simple yet non-trivial test data sets
    /// </summary>
    [System.Security.SecuritySafeCritical]
    public class ILSpecialData : ILMath {
        /// <summary>
        /// Get example terrain data, 401 x 401 short matrix with heights in meters
        /// </summary>
        public static ILRetArray<short> terrain {
            get {
                using (var s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ILNumerics.terrain.bin")) {
                    ILArray<short> ret = loadBinary<short>(s,401,401,401); 
                    return ret; 
                }
            }
        }

        /// <summary>
        /// Generate sinc function in 2D, useful for plotting examples
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="cols">Number of columns</param>
        /// <param name="periods">Influences the number of periods to be drawn in both directions. 1 will result in 4 zero crossings, higher values result in more, lower values in less zero crossings.</param>
        /// <returns>Matrix with sinc data in 2 dimensions</returns>
        public static ILRetArray<double> sinc (int rows, int cols, float periods) {
            using (ILScope.Enter()) {
                ILArray<double> X = repmat<double>(vec<double>(-cols,2.0,cols-1).T,rows,1) / cols*pi*2*periods;  
                ILArray<double> Y = repmat<double>(vec<double>(-rows,2.0,rows-1),1,cols) / rows*pi*2*periods; 
                ILArray<double> ret = sqrt(X * X + Y * Y);
                ret[ret == 0.0] = MachineParameterDouble.eps; 
                ret.a = sin(ret)/ret; 
                return ret; 
            }
        }
        /// <summary>
        /// Generate sinc function in 2D, useful for plotting examples
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="cols">Number of columns</param>
        /// <returns>Matrix with sinc data in 2 dimensions</returns>
        /// <remarks>The function generates 4 zero crossings in each direction</remarks>
        public static ILRetArray<double> sinc(int rows, int cols) {
            return sinc(rows, cols, 1.0f);
        }
        /// <summary>
        /// Generate sinc function in 2D, single precision, useful for plotting examples
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="cols">Number of columns</param>
        /// <returns>Matrix with sinc data in 2 dimensions</returns>
        /// <remarks>The function generates 4 zero crossings in each direction</remarks>
        public static ILRetArray<float> sincf(int rows, int cols) {
            return tosingle(sinc(rows, cols, 1.0f));
        }
        /// <summary>
        /// Generate sinc function in 2D, useful for plotting examples
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="cols">Number of columns</param>
        /// <param name="periods">Influences the number of periods to be drawn in both directions. 1 will result in 4 zero crossings, higher values result in more, lower values in less zero crossings.</param>
        /// <returns>Matrix with sinc data in 2 dimensions</returns>
        public static ILRetArray<float> sincf(int rows, int cols, float periods) {
            return tosingle(sinc(rows,cols,periods)); 
        }

        /// <summary>
        /// Create specified periods of sine and cosine data
        /// </summary>
        /// <param name="numSamples">Number of samples</param>
        /// <param name="periods">Number of (full) periods to be generated, must be &gt; 0</param>
        /// <returns>Matrix with sine data in first column, cosine data in second column</returns>
        public static ILRetArray<double> sincos1D(int numSamples, double periods) {
            using (ILScope.Enter()) {
                ILArray<double> t = linspace(0.0, 2 * pi * periods, numSamples);
                return horzcat(sin(t).T, cos(t).T);
            }
        }
        /// <summary>
        /// Create specified periods of sine and cosine data, single precision
        /// </summary>
        /// <param name="numSamples">Number of samples</param>
        /// <param name="periods">Number of (full) periods to be generated, must be &gt; 0</param>
        /// <returns>Matrix with sine data in first column, cosine data in second column</returns>
        public static ILRetArray<float> sincos1Df(int numSamples, double periods) {
            using (ILScope.Enter()) {
                ILArray<float> t = linspace<float>(0, 2 * pi * periods, numSamples);
                return horzcat(sin(t).T, cos(t).T);
            }
        }
        /// <summary>
        /// Create demo data for surface plots looking like a waterfall
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="cols">Number of columns</param>
        /// <returns>Matrix with data showing a waterfall terrain. </returns>
        public static ILRetArray<double> waterfall(int rows, int cols) {
            using (ILScope.Enter()) {
                ILArray<double> a = rand(rows, cols);
                ILArray<double> bord = rand(1, cols) * 3 + (rows / 2);
                for (int c = 0; c < cols; c++) {
                    int b = (int)(bord[c] - sin(c / cols * pi) * cols / 5);
                    a[vec(0.0, b), c] = a[vec(0.0, b), c] + 2.0;
                }
                return a;
            }
        }

        /// <summary>
        /// Create demo data for surface plots looking like a waterfall
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="cols">Number of columns</param>
        /// <returns>Matrix with data showing a waterfall terrain. </returns>
        public static ILRetArray<float> waterfallf(int rows, int cols) {
            using (ILScope.Enter()) {
                ILArray<double> a = rand(rows, cols);
                ILArray<double> bord = rand(1, cols) * 3 + (rows / 2);
                for (int c = 0; c < cols; c++) {
                    int b = (int)(bord[c] - sin(c / cols * pi) * cols / 5);
                    a[vec(0.0, b), c] = a[vec(0.0, b), c] + 2.0;
                }
                return tosingle(a);
            }
        }

        /// <summary>
        /// Create surface data of a sphere
        /// </summary>
        /// <param name="n">Number of facettes per angle</param>
        /// <param name="X">[Output] X coords</param>
        /// <param name="Y">[Output] Y coords</param>
        /// <param name="Z">[Output] Z coords</param>
        public static void sphere(int n, ILOutArray<double> X, ILOutArray<double> Y, ILOutArray<double> Z) {
            using (ILScope.Enter()) {
                ILArray<double> phi = repmat(linspace(-pi, pi, n).T, 1, n);
                ILArray<double> rho = repmat(linspace(0, pi, n), n, 1);
                Y.a = sin(phi) * sin(rho);
                X.a = cos(phi) * sin(rho);
                Z.a = cos(rho);
            }
        }
        /// <summary>
        /// Create surface data for a M�bius strip 
        /// </summary>
        /// <param name="n">Granularity (number of facettes)</param>
        /// <param name="w">Width</param>
        /// <param name="R">Radius</param>
        /// <param name="X">[Output] X coords</param>
        /// <param name="Y">[Output] Y coords</param>
        /// <param name="Z">[Output] Z coords</param>
        /// <remarks>M�bius strip is a surfcae, crated by cutting a regular strip, twisting one end by 180 deg and glueing 
        /// both ends together again.</remarks>
        public static void moebius(int n, double w, double R, ILOutArray<double> X, ILOutArray<double> Y, ILOutArray<double> Z) {
            using (ILScope.Enter()) {
                ILArray<double> s = repmat(linspace(-w, w, n), n, 1);
                ILArray<double> t = repmat(linspace(0, 2 * pi, n).T, 1, n);
                X.a = (R + s * cos(0.5 * t)) * cos(t);
                Y.a = (R + s * cos(0.5 * t)) * sin(t);
                Z.a = s * sin(0.5 * t);
            }
        }
        /// <summary>
        /// Create torus cartesian coordinates, to be used for surface plotting
        /// </summary>
        /// <param name="outerRadius">[optional] the outer radius of the torus ring, default: 0.75</param>
        /// <param name="innerRadius">[optional] the inner radius of the torus ring, default: 0.25</param>
        /// <param name="stepsPoloidal">[optional] number of grid points in poloidal direction, default: 100</param>
        /// <param name="stepsToroidal">[optional] number of grid points in toroidal direction, default: 100</param>
        /// <returns>Data array with cartesian coordinates of the torus gris points. </returns>
        /// <remarks></remarks>
        public static ILRetArray<float> torus(float outerRadius = 0.75f, float innerRadius = 0.25f, int stepsPoloidal = 100, int stepsToroidal = 100) {
            using (ILScope.Enter()) {
                ILArray<float> ret = zeros<float>(stepsPoloidal, stepsToroidal, 2); 
                ILArray<float> theta = linspace<float>(-pi, pi, stepsPoloidal);
                ILArray<float> phi = linspace<float>(0, 2f * pif, stepsToroidal);
                ILArray<float> p = 1;
                ILArray<float> t = meshgrid(phi, theta, p); 
                ret[full,full,1] = (outerRadius + innerRadius * cos(p)) * cos(t);
                ret[full,full,2] = (outerRadius + innerRadius * cos(p)) * sin(t);
                ret[full,full,0] = innerRadius * sin(p);
                return ret; 
            }
        }
    }
}
