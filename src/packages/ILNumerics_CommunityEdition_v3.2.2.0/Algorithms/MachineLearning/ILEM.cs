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
using System.Linq;
using System.Text;
using ILNumerics.Exceptions;


namespace ILNumerics {
    [System.Security.SecuritySafeCritical]
    public partial class ILMath {

        /// <summary>
        /// Determine method of center initialization for EM algorithm
        /// </summary>
        public enum EMInitializationMethod {
            /// <summary>
            /// Use the kmeans algorithm, choose random samples as centers for start 
            /// </summary>
            KMeans_random,
            /// <summary>
            /// Use the kmeans algorithm, choose first k samples as centers for start 
            /// </summary>
            KMeans_firstK,
            /// <summary>
            /// Provide custom centers in the 'InitCenter' argument
            /// </summary>
            User
        }



        /// <summary>
        /// Expectation maximization algorithm
        /// </summary>
        /// <param name="Samples">Input data, data points in columns</param>
        /// <param name="k">Number of clusters</param>
        /// <param name="method">[Optional] Method used for initializing the cluster centers, default: kmeans_random</param>
        /// <param name="UserCenters">[Optional] For method 'user': initial cluster centers, size samples.D[0] x k, for other methods ignored</param>
        /// <param name="maxiterexit">[Optional] Break after that number of iterations, if no convergence was reached</param>
        /// <param name="Sigma">[Output] Covariance estimation for all clusters, size d x d x k, d = samples.D[0]</param>
        /// <param name="centerconverg_exit">[Optional] Exit iteration if norm(L) falls below that value, default: 0.001</param>
        /// <returns>Estimated centers for all clusters, size samples.D[0] x k</returns>
        /// <remarks><para>The EM algorithm expects the data samples to be drawn from <paramref name="k"/> multivariate normal distributions. 
        /// It estimates the parameters 'center' and 'sigma (covariance)' of every distribution. Therefore, the position and 'shape' 
        /// of each distribution is calculated in such a way, that the likelyhood of generating the given sample points is maximized.</para>
        /// <para>The parameter k must be determined by the user. This reflects the a priori knowledge of the number of distributions 
        /// or clusters in the data.</para>
        /// <para>The algorithm exits, if one of the exit criteria is reached: 
        /// <list type="bullet"><item>norm(L) &lt; 'centerconverg_exit' - where L is the difference between 
        /// the centers from the last step and the centers just computed in the current step</item>
        /// <item>the number of iteration steps exceeds the limit of 'maxiterexit' iterations.</item></list></para>
        /// </remarks>
        public static ILRetArray<double> em(ILInArray<double> Samples,
                                    int k, ILOutArray<double> Sigma = null, EMInitializationMethod method = EMInitializationMethod.KMeans_random,
                                    ILInArray<double> UserCenters = null, int maxiterexit = 10000, double centerconverg_exit = 0.001) {
            using (ILScope.Enter(Samples, UserCenters)) {

                ILArray<double> samples = check(Samples);
                ILArray<double> userCenters = check(UserCenters, allowNullInput: true); 
                if (k < 0)
                    throw new ILArgumentException("k must be greater or equal 0");
                if (isnull(samples))
                    throw new ILArgumentException("input argument 'samples' must not be null");
                if (method == EMInitializationMethod.User) {
                    if (isnull(userCenters))
                        throw new ILArgumentException("if initialization method 'user' was choosen, the parameter 'userCenters' must be used to provide custom initialization centers");
                    if (userCenters.S[0] != samples.S[0])
                        throw new ILArgumentException("the dimensionality (number of rows) of 'samples' and 'userCenters' must match");
                }
                // initialization
                int d = samples.S[0], n = samples.S[1], count = 0;
                ILArray<double> mu = empty<double>();
                ILArray<double> sigm = repmat(eye<double>(d, d), 1, 1, k);
                ILArray<double> gamma = zeros<double>(k, n);
                ILArray<double> priors = ones<double>(1, k) / k;
                ILArray<double> oldmu = zeros<double>(d, k);
                switch (method) {
                    case EMInitializationMethod.KMeans_random:
                        kMeansClust(samples, k, outCenters: mu, centerInitRandom: true).Dispose();
                        break;
                    case EMInitializationMethod.KMeans_firstK:
                        kMeansClust(samples, k, outCenters: mu, centerInitRandom: false).Dispose();
                        break;
                    case EMInitializationMethod.User:
                        mu.a = userCenters;
                        break;
                    default:
                        throw new ILArgumentException("invalid 'method' argument given");
                }
                // main loop
                while (true) {
                    using (ILScope.Enter()) {
                        // E - Step
                        for (int i = 0; i < k; i++) {
                            gamma[i, full] = gauss(samples, mu[full, i], sigm[full, full, i]) / priors[i];
                        }
                        gamma.a = gamma / max(sum(gamma, 0), MachineParameterDouble.eps);

                        // M - Step
                        ILArray<double> Nk = sum(gamma, 1);
                        priors.a = Nk / n;
                        for (int i = 0; i < k; i++) {
                            using (ILScope.Enter()) {
                                mu[full, i] = sum(gamma[i, full] * samples, 1) / Nk[i];
                                ILArray<double> tmpXnMinMuk = samples - mu[full, i];
                                sigm[full, full, i] = multiply((gamma[i, full] * tmpXnMinMuk), tmpXnMinMuk.T) / Nk[i];
                            }
                        }

                        // check exit condition
                        if (norm(oldmu - mu) < (double)centerconverg_exit || count > maxiterexit)
                            break;
                        else {
                            oldmu = mu;
                            count = count + 1;
                        }
                    }
                }
                if (!isnull(Sigma)) {
                    Sigma.a = sigm; 
                }
                return mu; 
            }
        }

        private static ILRetArray<double> gauss(ILInArray<double> samples, ILInArray<double> mu, ILRetArray<double> sigma) {
            using (ILScope.Enter(samples, mu, sigma)) { 
                int d = samples.S[0], n = samples.S[1];
                ILArray<double> sampMinMu = samples - mu;
                ILArray<double> sigInv = linsolve(eye<double>(d, d), sigma[full,full]);
                return 1 / ((double)pow(sqrt(2 * pi), d) * det(sigma)) * exp((double)-0.5 * sum(multiply(sigInv.T, sampMinMu) * sampMinMu, 0));  
            }
        }
             

#region HYCALPER AUTO GENERATED CODE


        /// <summary>
        /// Expectation maximization algorithm
        /// </summary>
        /// <param name="Samples">Input data, data points in columns</param>
        /// <param name="k">Number of clusters</param>
        /// <param name="method">[Optional] Method used for initializing the cluster centers, default: kmeans_random</param>
        /// <param name="UserCenters">[Optional] For method 'user': initial cluster centers, size samples.D[0] x k, for other methods ignored</param>
        /// <param name="maxiterexit">[Optional] Break after that number of iterations, if no convergence was reached</param>
        /// <param name="Sigma">[Output] Covariance estimation for all clusters, size d x d x k, d = samples.D[0]</param>
        /// <param name="centerconverg_exit">[Optional] Exit iteration if norm(L) falls below that value, default: 0.001</param>
        /// <returns>Estimated centers for all clusters, size samples.D[0] x k</returns>
        /// <remarks><para>The EM algorithm expects the data samples to be drawn from <paramref name="k"/> multivariate normal distributions. 
        /// It estimates the parameters 'center' and 'sigma (covariance)' of every distribution. Therefore, the position and 'shape' 
        /// of each distribution is calculated in such a way, that the likelyhood of generating the given sample points is maximized.</para>
        /// <para>The parameter k must be determined by the user. This reflects the a priori knowledge of the number of distributions 
        /// or clusters in the data.</para>
        /// <para>The algorithm exits, if one of the exit criteria is reached: 
        /// <list type="bullet"><item>norm(L) &lt; 'centerconverg_exit' - where L is the difference between 
        /// the centers from the last step and the centers just computed in the current step</item>
        /// <item>the number of iteration steps exceeds the limit of 'maxiterexit' iterations.</item></list></para>
        /// </remarks>
        public static ILRetArray<float> em(ILInArray<float> Samples,
                                    int k, ILOutArray<float> Sigma = null, EMInitializationMethod method = EMInitializationMethod.KMeans_random,
                                    ILInArray<float> UserCenters = null, int maxiterexit = 10000, double centerconverg_exit = 0.001) {
            using (ILScope.Enter(Samples, UserCenters)) {

                ILArray<float> samples = check(Samples);
                ILArray<float> userCenters = check(UserCenters, allowNullInput: true); 
                if (k < 0)
                    throw new ILArgumentException("k must be greater or equal 0");
                if (isnull(samples))
                    throw new ILArgumentException("input argument 'samples' must not be null");
                if (method == EMInitializationMethod.User) {
                    if (isnull(userCenters))
                        throw new ILArgumentException("if initialization method 'user' was choosen, the parameter 'userCenters' must be used to provide custom initialization centers");
                    if (userCenters.S[0] != samples.S[0])
                        throw new ILArgumentException("the dimensionality (number of rows) of 'samples' and 'userCenters' must match");
                }
                // initialization
                int d = samples.S[0], n = samples.S[1], count = 0;
                ILArray<float> mu = empty<float>();
                ILArray<float> sigm = repmat(eye<float>(d, d), 1, 1, k);
                ILArray<float> gamma = zeros<float>(k, n);
                ILArray<float> priors = ones<float>(1, k) / k;
                ILArray<float> oldmu = zeros<float>(d, k);
                switch (method) {
                    case EMInitializationMethod.KMeans_random:
                        kMeansClust(samples, k, outCenters: mu, centerInitRandom: true).Dispose();
                        break;
                    case EMInitializationMethod.KMeans_firstK:
                        kMeansClust(samples, k, outCenters: mu, centerInitRandom: false).Dispose();
                        break;
                    case EMInitializationMethod.User:
                        mu.a = userCenters;
                        break;
                    default:
                        throw new ILArgumentException("invalid 'method' argument given");
                }
                // main loop
                while (true) {
                    using (ILScope.Enter()) {
                        // E - Step
                        for (int i = 0; i < k; i++) {
                            gamma[i, full] = gauss(samples, mu[full, i], sigm[full, full, i]) / priors[i];
                        }
                        gamma.a = gamma / max(sum(gamma, 0), MachineParameterSingle.eps);

                        // M - Step
                        ILArray<float> Nk = sum(gamma, 1);
                        priors.a = Nk / n;
                        for (int i = 0; i < k; i++) {
                            using (ILScope.Enter()) {
                                mu[full, i] = sum(gamma[i, full] * samples, 1) / Nk[i];
                                ILArray<float> tmpXnMinMuk = samples - mu[full, i];
                                sigm[full, full, i] = multiply((gamma[i, full] * tmpXnMinMuk), tmpXnMinMuk.T) / Nk[i];
                            }
                        }

                        // check exit condition
                        if (norm(oldmu - mu) < (float)centerconverg_exit || count > maxiterexit)
                            break;
                        else {
                            oldmu = mu;
                            count = count + 1;
                        }
                    }
                }
                if (!isnull(Sigma)) {
                    Sigma.a = sigm; 
                }
                return mu; 
            }
        }

        private static ILRetArray<float> gauss(ILInArray<float> samples, ILInArray<float> mu, ILRetArray<float> sigma) {
            using (ILScope.Enter(samples, mu, sigma)) { 
                int d = samples.S[0], n = samples.S[1];
                ILArray<float> sampMinMu = samples - mu;
                ILArray<float> sigInv = linsolve(eye<float>(d, d), sigma[full,full]);
                return 1 / ((float)pow(sqrt(2 * pi), d) * det(sigma)) * exp((float)-0.5 * sum(multiply(sigInv.T, sampMinMu) * sampMinMu, 0));  
            }
        }
             

#endregion HYCALPER AUTO GENERATED CODE
   }
}
