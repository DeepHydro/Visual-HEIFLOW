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

        private static ILArrayCache s_mvnrndcache = ILCacheManager.Manager.GetCache();

        /// <summary>
        /// Choose samples from a multivariate random distribution
        /// </summary>
        /// <returns>n random numbers as taken from the multivariate random probability distribution with zero mean and unity covariance</returns>
        /// <remarks><para>This is an alias for <see cref="M:ILNumerics.ILMath.randn(params int[])"/>. If n was not specified, a single random number is generated.</para>
        /// <para>The samples are returned as row vector of size 1 x n.</para></remarks>
        public static ILRetArray<double> mvnrnd(int n = -1) {
            if (n >= 0)
                return randn(1,n); 
            return randn(1,1);  
        }

        /// <summary>
        /// Choose one sample from a multivariate random distribution
        /// </summary>
        /// <returns>Single random number, taken from the multivariate random probability distribution with zero mean and unity covariance</returns>
        /// <remarks><para>This is an alias for <see cref="M:ILNumerics.ILMath.randn(params int[])"/>. A single (scalar) random number is generated.</para></remarks>
        public static ILRetArray<double> mvnrnd() {
            return randn(1, 1); 
        }



        /// <summary>
        /// choose samples from a multivariate random distribution  
        /// </summary>
        /// <param name="inMu">[optional] centers, size d x n; if d x 1 is given, optional parameter <paramref name="n"/> is used to replicate mu accordingly, if null, the values will be genereated with a center of zero</param>
        /// <param name="inSigma">[optional] covariance matrix, must be positive definite, size d x d or vector of lenght d, if null (not set), unitiy matrix is expected</param>
        /// <param name="n">[optional] number of samples to generate, per default (-1) the number of columns of <paramref name="mu"/> defines that number</param>
        /// <param name="sigmaIsSquaredCov">[optional] if false: safe the effort of finding the square root of <paramref name="sigma"/> parameter; default: true</param>
        /// <returns>random numbers as taken from the multivariate random probability distribution given by mu and sigma</returns>
        /// <remarks><para>In order to safe the step of finding the root of sigma, the following options exist: 
        /// <list type="bullet">
        /// <item>Provide only the diagonal of a (virtual) diagonal matrix to <paramref name="sigma"/>.</item>
        /// <item>Compute the root manually, give it to sigma and set <paramref name="sigmaIsSquaredCov"/> to false.</item>
        /// </list></para>
        /// <para>In case <paramref name="sigmaIsSquaredCov"/> set to 'false' and <paramref name="sigma"/> is given, 
        /// the root is computed via cholesky factorization. The result of the last root finding process is cached and reused for 
        /// subsequent requests with the same set of <paramref name="n"/> and <paramref name="sigma"/> parameters.</para></remarks>
        public static ILRetArray<double> mvnrnd(ILInArray<double> inMu = null, ILInArray<double> inSigma = null,
                                                       int n = -1, bool sigmaIsSquaredCov = true) {
            using (ILScope.Enter(inMu, inSigma)) {

                // early exit, trivial case 
                if (isnullorempty(inMu) && isnullorempty(inSigma)) {
                    return  /*dummy*/ (randn(1, Math.Max(n, 0)));
                }
                ILArray<double> mu = check(inMu, Default: empty< double>());
                ILArray<double> sigma = check(inSigma, Default: empty<double>()); 

                // determine output size
                n = Math.Max(Math.Max(mu.S[1], sigma.S[2]), n);
                int d = Math.Max(Math.Max(mu.S[0], sigma.S[0]), sigma.S[1]);
                ILArray<double> ret = empty<double>();

                if (mu.IsEmpty)
                    mu = zeros<double>(d, 1);
                if (mu.IsRowVector)
                    mu = mu.T;
                if (sigma.IsEmpty)
                    sigma = ones<double>(d, 1);
                if (sigma.IsRowVector)
                    sigma = sigma.T;

                if (mu.IsVector && mu.Length != d) {
                    throw new ILArgumentException("mu must be empty or have the same dimensionality as sigma");
                } else if (mu.S[1] > 1 && (mu.S[0] != d || mu.S[1] != n)) {
                    throw new ILArgumentException("mu must be empty, vector d x 1 or matrix d x n, d = dimensionality of sigma");
                }
                ILArray<double> sigmaLoc = sigma;
                // main
                if (sigma.IsVector && sigma.Length == d) {
                    // we dont cach vector sigmas
                    if (any(sigmaLoc < 0)) {
                        throw new ILArgumentException("all diagonal elements of sigma must be >= 0"); 
                    }
                    sigmaLoc.a = sqrt(sigmaLoc);
                    ret.a =  /*dummy*/ (randn(d, n)) * sigmaLoc + mu;

                } else if ((sigma.IsMatrix || sigma.S[2] == n) && sigma.S[0] == d && sigma.S[1] == d) {
                    if (sigmaIsSquaredCov && !s_mvnrndcache.TryGetArray<double>(sigmaLoc, n, sigma)) {
                        for (int i = sigmaLoc.S[2]; i --> 0; ) {
                            sigmaLoc[full, full, i] = chol(sigmaLoc[full, full, i]); 
                        }
                        s_mvnrndcache.Cache<double>(sigmaLoc, n, sigma);
                    }
                    ret.a  = zeros<double>(d,n);
                    bool sigmaIsMatrix = sigmaLoc.S[2] <= 1; 
                    if (mu.IsVector) {
                        for (int i = 0; i < n; i++) {
                            using (ILScope.Enter()) {
                                if (sigmaIsMatrix)
                                    ret[full, i] = multiply( /*dummy*/ (randn(1, d)), sigmaLoc).T + mu;
                                else
                                    ret[full, i] = multiply( /*dummy*/ (randn(1, d)), sigmaLoc[full, full, i]).T + mu;
                            }
                        }
                    } else {
                        for (int i = 0; i < n; i++) {
                            using (ILScope.Enter()) {
                                if (sigmaIsMatrix)
                                    ret[full, i] = multiply( /*dummy*/ (randn(1, d)), sigmaLoc).T + mu[full, i];
                                else
                                    ret[full, i] = multiply( /*dummy*/ (randn(1, d)), sigmaLoc[full, full, i]).T + mu[full, i];

                            }
                        }
                    }
                } else {
                    throw new ILArgumentException("invalid size of sigma, check the documentation for valid options");
                }
                return ret; 
            }
        }

#region HYCALPER AUTO GENERATED CODE


        /// <summary>
        /// choose samples from a multivariate random distribution  
        /// </summary>
        /// <param name="inMu">[optional] centers, size d x n; if d x 1 is given, optional parameter <paramref name="n"/> is used to replicate mu accordingly, if null, the values will be genereated with a center of zero</param>
        /// <param name="inSigma">[optional] covariance matrix, must be positive definite, size d x d or vector of lenght d, if null (not set), unitiy matrix is expected</param>
        /// <param name="n">[optional] number of samples to generate, per default (-1) the number of columns of <paramref name="mu"/> defines that number</param>
        /// <param name="sigmaIsSquaredCov">[optional] if false: safe the effort of finding the square root of <paramref name="sigma"/> parameter; default: true</param>
        /// <returns>random numbers as taken from the multivariate random probability distribution given by mu and sigma</returns>
        /// <remarks><para>In order to safe the step of finding the root of sigma, the following options exist: 
        /// <list type="bullet">
        /// <item>Provide only the diagonal of a (virtual) diagonal matrix to <paramref name="sigma"/>.</item>
        /// <item>Compute the root manually, give it to sigma and set <paramref name="sigmaIsSquaredCov"/> to false.</item>
        /// </list></para>
        /// <para>In case <paramref name="sigmaIsSquaredCov"/> set to 'false' and <paramref name="sigma"/> is given, 
        /// the root is computed via cholesky factorization. The result of the last root finding process is cached and reused for 
        /// subsequent requests with the same set of <paramref name="n"/> and <paramref name="sigma"/> parameters.</para></remarks>
        public static ILRetArray<float> mvnrnd(ILInArray<float> inMu = null, ILInArray<float> inSigma = null,
                                                       int n = -1, bool sigmaIsSquaredCov = true) {
            using (ILScope.Enter(inMu, inSigma)) {

                // early exit, trivial case 
                if (isnullorempty(inMu) && isnullorempty(inSigma)) {
                    return  tosingle (randn(1, Math.Max(n, 0)));
                }
                ILArray<float> mu = check(inMu, Default: empty< float>());
                ILArray<float> sigma = check(inSigma, Default: empty<float>()); 

                // determine output size
                n = Math.Max(Math.Max(mu.S[1], sigma.S[2]), n);
                int d = Math.Max(Math.Max(mu.S[0], sigma.S[0]), sigma.S[1]);
                ILArray<float> ret = empty<float>();

                if (mu.IsEmpty)
                    mu = zeros<float>(d, 1);
                if (mu.IsRowVector)
                    mu = mu.T;
                if (sigma.IsEmpty)
                    sigma = ones<float>(d, 1);
                if (sigma.IsRowVector)
                    sigma = sigma.T;

                if (mu.IsVector && mu.Length != d) {
                    throw new ILArgumentException("mu must be empty or have the same dimensionality as sigma");
                } else if (mu.S[1] > 1 && (mu.S[0] != d || mu.S[1] != n)) {
                    throw new ILArgumentException("mu must be empty, vector d x 1 or matrix d x n, d = dimensionality of sigma");
                }
                ILArray<float> sigmaLoc = sigma;
                // main
                if (sigma.IsVector && sigma.Length == d) {
                    // we dont cach vector sigmas
                    if (any(sigmaLoc < 0)) {
                        throw new ILArgumentException("all diagonal elements of sigma must be >= 0"); 
                    }
                    sigmaLoc.a = sqrt(sigmaLoc);
                    ret.a =  tosingle (randn(d, n)) * sigmaLoc + mu;

                } else if ((sigma.IsMatrix || sigma.S[2] == n) && sigma.S[0] == d && sigma.S[1] == d) {
                    if (sigmaIsSquaredCov && !s_mvnrndcache.TryGetArray<float>(sigmaLoc, n, sigma)) {
                        for (int i = sigmaLoc.S[2]; i --> 0; ) {
                            sigmaLoc[full, full, i] = chol(sigmaLoc[full, full, i]); 
                        }
                        s_mvnrndcache.Cache<float>(sigmaLoc, n, sigma);
                    }
                    ret.a  = zeros<float>(d,n);
                    bool sigmaIsMatrix = sigmaLoc.S[2] <= 1; 
                    if (mu.IsVector) {
                        for (int i = 0; i < n; i++) {
                            using (ILScope.Enter()) {
                                if (sigmaIsMatrix)
                                    ret[full, i] = multiply( tosingle (randn(1, d)), sigmaLoc).T + mu;
                                else
                                    ret[full, i] = multiply( tosingle (randn(1, d)), sigmaLoc[full, full, i]).T + mu;
                            }
                        }
                    } else {
                        for (int i = 0; i < n; i++) {
                            using (ILScope.Enter()) {
                                if (sigmaIsMatrix)
                                    ret[full, i] = multiply( tosingle (randn(1, d)), sigmaLoc).T + mu[full, i];
                                else
                                    ret[full, i] = multiply( tosingle (randn(1, d)), sigmaLoc[full, full, i]).T + mu[full, i];

                            }
                        }
                    }
                } else {
                    throw new ILArgumentException("invalid size of sigma, check the documentation for valid options");
                }
                return ret; 
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}