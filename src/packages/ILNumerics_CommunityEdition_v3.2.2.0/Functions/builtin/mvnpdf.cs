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
        /// Probability density function for a multivariate normal random distribution
        /// </summary>
        /// <param name="A">Matrix of points in columns, where the probability density function is to be evaluated</param>
        /// <param name="mu">[Optional] Centers, size d x 1, if 'null': zeros are attempted, default: null</param>
        /// <param name="sigma">Covariance matrix, must be positive definite, size d x d or vector of lenght d</param>
        /// <returns>Random numbers as taken from the multivariate random probability distribution given by mu and sigma</returns>
        public static ILRetArray<double> mvnpdf(ILInArray<double> A, ILInArray<double> mu = null, ILInArray<double> sigma = null) {
            using (ILScope.Enter(A, mu, sigma)) {
                if (isnull(A)) {
                    throw new ILArgumentException("input parameter 'samples' may not be null");
                }
                int d = A.S[0], n = A.S[1];
                if (A.IsEmpty) {
                    if (d > 0)
                        return empty<double>(A.S); 
                    else {
                        return empty<double>(ILSize.Empty00); 
                    }
                }
                // early exit, trivial case 
                if (isnullorempty(mu) && isnullorempty(sigma)) {
                    return 1 / (pow(sqrt(2 * pi), d)) * exp(-0.5f * (diag(multiply(A.T, A))));
                }
                ILArray<double> muLoc = mu; 
                if (isnullorempty(mu)) {
                    muLoc.a = zeros<double>(d,1);      
                }
                ILArray<double> sigmaLoc = sigma; 
                if (isnullorempty(sigma)) {
                    sigmaLoc.a = eye<double>(d,d);      
                }
                ILArray<double> sampMinMu = A - muLoc;
                return 1 / (pow(sqrt(2 * pi), d) * det(sigmaLoc)) * exp(-0.5 * (diag(multiply(sampMinMu.T, eye(d, d) / sigmaLoc, sampMinMu))));
            }
        }

        /// <summary>
        /// Probability density function for a multivariate normal random distribution
        /// </summary>
        /// <param name="A">Matrix of points in columns, where the probability density function is to be evaluated</param>
        /// <param name="mu">[Optional] Centers, size d x 1, if 'null': zeros are attempted, default: null</param>
        /// <param name="sigma">Covariance matrix, must be positive definite, size d x d or vector of lenght d</param>
        /// <returns>Random numbers as taken from the multivariate random probability distribution given by mu and sigma</returns>
        public static ILRetArray<float> mvnpdf(ILInArray<float> A, ILInArray<float> mu = null, ILInArray<float> sigma = null) {
            using (ILScope.Enter(A, mu, sigma)) {
                if (isnull(A)) {
                    throw new ILArgumentException("input parameter 'samples' may not be null");
                }
                int d = A.S[0], n = A.S[1];
                if (A.IsEmpty) {
                    if (d > 0)
                        return empty<float>(A.S); 
                    else {
                        return empty<float>(ILSize.Empty00); 
                    }
                }
                // early exit, trivial case 
                if (isnullorempty(mu) && isnullorempty(sigma)) {
                    return 1 / tosingle(pow(sqrt(2 * pi), d)) * exp(-0.5f * (diag(multiply(A.T, A))));
                }
                ILArray<float> muLoc = mu; 
                if (isnullorempty(mu)) {
                    muLoc.a = zeros<float>(d,1);      
                }
                ILArray<float> sigmaLoc = sigma; 
                if (isnullorempty(sigma)) {
                    sigmaLoc.a = eye<float>(d,d);      
                }
                ILArray<float> sampMinMu = A - muLoc;
                return 1f / tosingle(pow(sqrt(2 * pi), d)) * det(sigmaLoc) * exp(-0.5f * (diag(multiply(sampMinMu.T, eye<float>(d, d) / sigmaLoc, sampMinMu))));
            }
        }

    }
}