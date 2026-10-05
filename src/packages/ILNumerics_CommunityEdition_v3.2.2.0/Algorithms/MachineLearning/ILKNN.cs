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

    public partial class ILMath {

        public enum DistanceMetrics {
            Euclidian_L2,
            Mahalanobis,
            Manhattan_L1,
            Minkowski,
            Chebychev,
            Cosine,
            Pearsons,
            Hamming,
            Jaccard,
            Spearman
        }


        /// <summary>
        /// Search for k nearest neighbors for every sample in <paramref name="Samples"/> samples
        /// </summary>
        /// <param name="Samples">Samples matrix, samples in columns, the number of rows (dimensionality) must match the number of rows in <paramref name="Neighbors"/> </param>
        /// <param name="Neighbors">Matrix of training samples/ neighbors, this will be searched for matching points, rows: dimensionality, columns: number of points</param>
        /// <param name="k">[Optional] Number of neighbors to return, k must lay in range: 0 &le; k &lt neighbors.D[1]; default: 1</param>
        /// <param name="metric">[Optional] Distance metric, one out of the <see cref="ILNumerics.ILMath.DistanceMetrics"/> enumeration. Supported are: Euclidian_L2,Manhattan_L1, 
        /// Minkowski, Cosine, Pearsons and Hamming distances; default: 'Euclidian_L2'</param>
        /// <param name="minkowski_parameter">[Optional] Exponent for minkowski distance; default: 2</param>
        /// <param name="unstable_error">[Optional] For cosine and pearson distances: if some samples lead to numerical instabilities, an exception is generated; default: true</param>
        /// <returns>Matrix of nearest neighbors, size: k x samples.D[1]; indices of points in <paramref name="Neighbors"/> matrix</returns>
        public static ILRetArray<int> knn(ILInArray<double> Samples, ILInArray<double> Neighbors, int k = 10, 
                                                DistanceMetrics metric = DistanceMetrics.Euclidian_L2, double minkowski_parameter = 2.0,
                                                bool unstable_error = true) {
            using (ILScope.Enter(Samples, Neighbors)) {

                ILArray<double> samples = Samples; 
                ILArray<double> neighbors = Neighbors; 

                if (k < 0) {
                    throw new ILArgumentException("k must be greater or equal 0");  
                }
                if (isnullorempty(neighbors)) {
                    throw new ILArgumentException("input argument 'neighbors' must not be null or empty");
                }
                if (isnull(samples)) {
                    throw new ILArgumentException("input argument 'samples' must not be null"); 
                }
                if (samples.S[0] != neighbors.S[0])
                    throw new ILArgumentException("number of rows for 'neighbors' and 'samples' must match"); 
                if (k > neighbors.S[1]) 
                    throw new ILArgumentException("k must be smaller or equal to the number of datapoints (number of columns) in A"); 
                int nn = neighbors.S[1], am = neighbors.S[0], sn = samples.S[1];
                ILArray<int> ret = zeros<int>(k, sn);
                switch (metric) {
                    case DistanceMetrics.Euclidian_L2:
                        for (int i = 0; i < sn; i++) {
                            using (ILScope.Enter()) {
                                ILArray<double> dist = neighbors - samples[full, i];
                                dist.a = sum(dist * dist, 0);
                                ILArray<int> indices = 1;
                                if (k == 1) {
                                    min(dist, indices, 1).Dispose();
                                    ret[full, i] = indices[0];
                                } else {
                                    sort(dist, indices, 1, false).Dispose();
                                    ret[full, i] = indices[r(0, k - 1)];
                                }
                            }
                        }
                        break;
                    case DistanceMetrics.Manhattan_L1:
                        for (int i = 0; i < sn; i++) {
                            using (ILScope.Enter()) {
                                ILArray<double> dist = neighbors - samples[full, i];
                                dist.a = sum(abs(dist), 0);
                                ILArray<int> indices = 1;
                                if (k == 1) {
                                    min(dist, indices, 1).Dispose();
                                    ret[full, i] = indices[0];
                                } else {
                                    sort(dist, indices, 1, false).Dispose();
                                    ret[full, i] = indices[r(0, k - 1)];
                                }
                            }
                        }
                        break;
                    case DistanceMetrics.Minkowski:
                        for (int i = 0; i < sn; i++) {
                            using (ILScope.Enter()) {
                                ILArray<double> dist = neighbors - samples[full, i];
                                dist.a = sum(pow(dist,(double)minkowski_parameter), 0);
                                ILArray<int> indices = 1;
                                if (k == 1) {
                                    min(dist, indices, 0).Dispose();
                                    ret[full, i] = indices[0];
                                } else {
                                    sort(dist, indices, 0, false).Dispose();
                                    ret[full, i] = indices[r(0, k - 1)];
                                }
                            }
                        }
                        break;
                    case DistanceMetrics.Cosine:
                        ILArray<double> samples_normalized = sqrt(sum(samples * samples, 0));
                        ILArray<double> neighbs_normalized = sqrt(sum(neighbors * neighbors, 0));
                        if (unstable_error && !testStable(samples_normalized)) {
                            throw new ILArgumentException("possibly numerical instability: some samples are too close to 0. Try using a different metric instead!");
                        }
                        if (unstable_error && !testStable(neighbs_normalized)) {
                            throw new ILArgumentException("possibly numerical instability: some neighbors are too close to 0. Try using a different metric instead!");
                        }
                        neighbs_normalized.a = neighbors / neighbs_normalized;
                        for (int i = 0; i < sn; i++) {
                            using (ILScope.Enter()) {
                                ILArray<double> dist = 1 - multiply(neighbs_normalized.T, samples[full, i]) / samples_normalized[i];
                                ILArray<int> indices = 1;
                                if (k == 1) {
                                    min(dist, indices, 0).Dispose();
                                    ret[full, i] = indices[0];
                                } else {
                                    sort(dist, indices, 0, false).Dispose();
                                    ret[full, i] = indices[r(0, k - 1)];
                                }
                            }
                        }
                        break;
                    case DistanceMetrics.Pearsons:
                        ILArray<double> samples_centered = samples - mean(samples, 0);
                        ILArray<double> neighbs_centered = neighbors - mean(neighbors, 0);
                        samples_normalized = sqrt(sum(samples_centered * samples_centered, 0));
                        neighbs_normalized = sqrt(sum(neighbs_centered * neighbs_centered, 0));
                        if (unstable_error && !testStable(samples_normalized)) {
                            throw new ILArgumentException("possibly numerical instability: standard deviation for some neighbor points is close to zero. Try using a different metric instead!");
                        }
                        if (unstable_error && !testStable(neighbs_normalized)) {
                            throw new ILArgumentException("possibly numerical instability: standard deviation for some neighbor points is close to zero. Try using a different metric instead!");
                        }
                        neighbs_normalized.a = neighbs_centered / neighbs_normalized;
                        for (int i = 0; i < sn; i++) {
                            using (ILScope.Enter()) {
                                ILArray<double> dist = 1 - multiply(neighbs_normalized.T, samples_centered[full, i]) / samples_normalized[i];
                                ILArray<int> indices = 1;
                                if (k == 1) {
                                    min(dist,indices,0).Dispose(); 
                                    ret[full, i] = indices[0]; 
                                } else {
                                    sort(dist, indices, 0, false).Dispose();
                                    ret[full, i] = indices[r(0, k - 1)];
                                }
                            }
                        }
                        break;
                    case DistanceMetrics.Hamming:
                        if (samples.Any((a) => { return a != 0 && a != 1; })) {
                            throw new ILArgumentException("hamming distance requires 0 and 1 as value for all elements of 'samples'");
                        }
                        if (neighbors.Any((a) => { return a != 0 && a != 1; })) {
                            throw new ILArgumentException("hamming distance requires 0 and 1 as value for all elements of 'neighbors'");
                        }
                        for (int i = 0; i < sn; i++) {
                            using (ILScope.Enter()) {
                                ILArray<double> dist = sum(abs(neighbors - samples[full, i]), 0) / am;
                                ILArray<int> indices = 1;
                                if (k == 1) {
                                    min(dist, indices, 1).Dispose();
                                    ret[full, i] = indices[0];
                                } else {
                                    sort(dist, indices, 1, false).Dispose();
                                    ret[full, i] = indices[r(0, k - 1)];
                                }
                            }
                        }
                        break;
                    default:
                        throw new ILArgumentException("the selected distance is not supported"); 
                }
                return ret; 
            }
             
        }
        /// <summary>
        /// Test for numerical instability, expects positive data only!
        /// </summary>
        /// <param name="samples_normalized">Input data</param>
        /// <returns>true: no instability detected, false, possible instablility</returns>
        private static bool testStable(ILInArray<double> samples_normalized) {
            using (ILScope.Enter(samples_normalized)) {
                
                double max, min;
                samples_normalized.GetLimits(out min, out max);
                return min > MachineParameterDouble.eps * max;
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Search for k nearest neighbors for every sample in <paramref name="Samples"/> samples
        /// </summary>
        /// <param name="Samples">Samples matrix, samples in columns, the number of rows (dimensionality) must match the number of rows in <paramref name="Neighbors"/> </param>
        /// <param name="Neighbors">Matrix of training samples/ neighbors, this will be searched for matching points, rows: dimensionality, columns: number of points</param>
        /// <param name="k">[Optional] Number of neighbors to return, k must lay in range: 0 &le; k &lt neighbors.D[1]; default: 1</param>
        /// <param name="metric">[Optional] Distance metric, one out of the <see cref="ILNumerics.ILMath.DistanceMetrics"/> enumeration. Supported are: Euclidian_L2,Manhattan_L1, 
        /// Minkowski, Cosine, Pearsons and Hamming distances; default: 'Euclidian_L2'</param>
        /// <param name="minkowski_parameter">[Optional] Exponent for minkowski distance; default: 2</param>
        /// <param name="unstable_error">[Optional] For cosine and pearson distances: if some samples lead to numerical instabilities, an exception is generated; default: true</param>
        /// <returns>Matrix of nearest neighbors, size: k x samples.D[1]; indices of points in <paramref name="Neighbors"/> matrix</returns>
        public static ILRetArray<int> knn(ILInArray<float> Samples, ILInArray<float> Neighbors, int k = 10, 
                                                DistanceMetrics metric = DistanceMetrics.Euclidian_L2, double minkowski_parameter = 2.0,
                                                bool unstable_error = true) {
            using (ILScope.Enter(Samples, Neighbors)) {

                ILArray<float> samples = Samples; 
                ILArray<float> neighbors = Neighbors; 

                if (k < 0) {
                    throw new ILArgumentException("k must be greater or equal 0");  
                }
                if (isnullorempty(neighbors)) {
                    throw new ILArgumentException("input argument 'neighbors' must not be null or empty");
                }
                if (isnull(samples)) {
                    throw new ILArgumentException("input argument 'samples' must not be null"); 
                }
                if (samples.S[0] != neighbors.S[0])
                    throw new ILArgumentException("number of rows for 'neighbors' and 'samples' must match"); 
                if (k > neighbors.S[1]) 
                    throw new ILArgumentException("k must be smaller or equal to the number of datapoints (number of columns) in A"); 
                int nn = neighbors.S[1], am = neighbors.S[0], sn = samples.S[1];
                ILArray<int> ret = zeros<int>(k, sn);
                switch (metric) {
                    case DistanceMetrics.Euclidian_L2:
                        for (int i = 0; i < sn; i++) {
                            using (ILScope.Enter()) {
                                ILArray<float> dist = neighbors - samples[full, i];
                                dist.a = sum(dist * dist, 0);
                                ILArray<int> indices = 1;
                                if (k == 1) {
                                    min(dist, indices, 1).Dispose();
                                    ret[full, i] = indices[0];
                                } else {
                                    sort(dist, indices, 1, false).Dispose();
                                    ret[full, i] = indices[r(0, k - 1)];
                                }
                            }
                        }
                        break;
                    case DistanceMetrics.Manhattan_L1:
                        for (int i = 0; i < sn; i++) {
                            using (ILScope.Enter()) {
                                ILArray<float> dist = neighbors - samples[full, i];
                                dist.a = sum(abs(dist), 0);
                                ILArray<int> indices = 1;
                                if (k == 1) {
                                    min(dist, indices, 1).Dispose();
                                    ret[full, i] = indices[0];
                                } else {
                                    sort(dist, indices, 1, false).Dispose();
                                    ret[full, i] = indices[r(0, k - 1)];
                                }
                            }
                        }
                        break;
                    case DistanceMetrics.Minkowski:
                        for (int i = 0; i < sn; i++) {
                            using (ILScope.Enter()) {
                                ILArray<float> dist = neighbors - samples[full, i];
                                dist.a = sum(pow(dist,(float)minkowski_parameter), 0);
                                ILArray<int> indices = 1;
                                if (k == 1) {
                                    min(dist, indices, 0).Dispose();
                                    ret[full, i] = indices[0];
                                } else {
                                    sort(dist, indices, 0, false).Dispose();
                                    ret[full, i] = indices[r(0, k - 1)];
                                }
                            }
                        }
                        break;
                    case DistanceMetrics.Cosine:
                        ILArray<float> samples_normalized = sqrt(sum(samples * samples, 0));
                        ILArray<float> neighbs_normalized = sqrt(sum(neighbors * neighbors, 0));
                        if (unstable_error && !testStable(samples_normalized)) {
                            throw new ILArgumentException("possibly numerical instability: some samples are too close to 0. Try using a different metric instead!");
                        }
                        if (unstable_error && !testStable(neighbs_normalized)) {
                            throw new ILArgumentException("possibly numerical instability: some neighbors are too close to 0. Try using a different metric instead!");
                        }
                        neighbs_normalized.a = neighbors / neighbs_normalized;
                        for (int i = 0; i < sn; i++) {
                            using (ILScope.Enter()) {
                                ILArray<float> dist = 1 - multiply(neighbs_normalized.T, samples[full, i]) / samples_normalized[i];
                                ILArray<int> indices = 1;
                                if (k == 1) {
                                    min(dist, indices, 0).Dispose();
                                    ret[full, i] = indices[0];
                                } else {
                                    sort(dist, indices, 0, false).Dispose();
                                    ret[full, i] = indices[r(0, k - 1)];
                                }
                            }
                        }
                        break;
                    case DistanceMetrics.Pearsons:
                        ILArray<float> samples_centered = samples - mean(samples, 0);
                        ILArray<float> neighbs_centered = neighbors - mean(neighbors, 0);
                        samples_normalized = sqrt(sum(samples_centered * samples_centered, 0));
                        neighbs_normalized = sqrt(sum(neighbs_centered * neighbs_centered, 0));
                        if (unstable_error && !testStable(samples_normalized)) {
                            throw new ILArgumentException("possibly numerical instability: standard deviation for some neighbor points is close to zero. Try using a different metric instead!");
                        }
                        if (unstable_error && !testStable(neighbs_normalized)) {
                            throw new ILArgumentException("possibly numerical instability: standard deviation for some neighbor points is close to zero. Try using a different metric instead!");
                        }
                        neighbs_normalized.a = neighbs_centered / neighbs_normalized;
                        for (int i = 0; i < sn; i++) {
                            using (ILScope.Enter()) {
                                ILArray<float> dist = 1 - multiply(neighbs_normalized.T, samples_centered[full, i]) / samples_normalized[i];
                                ILArray<int> indices = 1;
                                if (k == 1) {
                                    min(dist,indices,0).Dispose(); 
                                    ret[full, i] = indices[0]; 
                                } else {
                                    sort(dist, indices, 0, false).Dispose();
                                    ret[full, i] = indices[r(0, k - 1)];
                                }
                            }
                        }
                        break;
                    case DistanceMetrics.Hamming:
                        if (samples.Any((a) => { return a != 0 && a != 1; })) {
                            throw new ILArgumentException("hamming distance requires 0 and 1 as value for all elements of 'samples'");
                        }
                        if (neighbors.Any((a) => { return a != 0 && a != 1; })) {
                            throw new ILArgumentException("hamming distance requires 0 and 1 as value for all elements of 'neighbors'");
                        }
                        for (int i = 0; i < sn; i++) {
                            using (ILScope.Enter()) {
                                ILArray<float> dist = sum(abs(neighbors - samples[full, i]), 0) / am;
                                ILArray<int> indices = 1;
                                if (k == 1) {
                                    min(dist, indices, 1).Dispose();
                                    ret[full, i] = indices[0];
                                } else {
                                    sort(dist, indices, 1, false).Dispose();
                                    ret[full, i] = indices[r(0, k - 1)];
                                }
                            }
                        }
                        break;
                    default:
                        throw new ILArgumentException("the selected distance is not supported"); 
                }
                return ret; 
            }
             
        }
        /// <summary>
        /// Test for numerical instability, expects positive data only!
        /// </summary>
        /// <param name="samples_normalized">Input data</param>
        /// <returns>true: no instability detected, false, possible instablility</returns>
        private static bool testStable(ILInArray<float> samples_normalized) {
            using (ILScope.Enter(samples_normalized)) {
               
                float max, min;
                samples_normalized.GetLimits(out min, out max);
                return min > MachineParameterSingle.eps * max;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}
