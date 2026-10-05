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
using ILNumerics.Exceptions;



namespace ILNumerics {


    public partial class ILMath {


        /// <summary>
        /// k-means clustering: find clusters in data matrix X
        /// </summary>
        /// <param name="X">Data matrix, data points are given as columns</param>
        /// <param name="k">Initial number of clusters expected</param>
        /// <param name="centerInitRandom">[Optional] false: pick the first k data points as initial centers, true: pick random datapoints (default)</param>
        /// <param name="maxIterations">[Optional] Maximum number of iterations, the computation will exit after that many iterations, default: 10.000</param>
        /// <param name="outCenters">[Input/Output/Optional] If not null on entry, outCenters will contain the centers of the clusters found, default: null</param>
        /// <returns>Vector of length n with with indices of the clustersm which were assigned to each datapoint</returns>
        /// <remarks><para>If <paramref name="outCenters"/> is given not null on input, the algorithm returns the computed centers in that parameter. A 
        /// matrix may be given on input, in order to give a hint of the initial center positions. This may help to find correct cluster centers - even if 
        /// the initial hint is not exact. In order to do so, the matrix given must be of the correct size (X.D[0] by k) and <paramref name="centerInitRandom"/>
        /// must be set to <c>false</c>.</para></remarks>
        public static ILRetArray<double> kMeansClust(ILInArray<double> X, ILBaseArray k, int maxIterations = 10000, 
                                                                    bool centerInitRandom = true, ILOutArray<double> outCenters = null) {
            using (ILScope.Enter(X, k)) {
                if (object.Equals(X, null)) {
                    throw new ILArgumentException("X must be data matrix (not null)");
                }
                if (X.IsEmpty) {
                    if (!object.Equals(outCenters, null)) {
                        if (X.S[0] > 0) {
                            outCenters.a = empty<double>(new ILSize(X.S[0], 0));
                        } else {
                            outCenters.a = empty<double>(new ILSize(0, X.S[1]));
                        }
                        return empty<double>(X.S);
                    }
                }
                if (object.Equals(k, null) || !k.IsScalar || !k.IsNumeric) {
                    throw new ILArgumentException("number of clusters k must be numeric scalar");
                }
                int iK = toint32(k).GetValue(0);
                if (X.S[1] < iK) {
                    throw new ILArgumentException("too few datapoints provided for " + iK.ToString() + " clusters");
                }
                if (iK < 0) {
                    throw new ILArgumentException("number of clusters must be positive");
                }
                int d = X.S[0], n = X.S[1];
                if (iK == 0) {
                    if (!object.Equals(outCenters, null)) {
                        outCenters.a = empty<double>(new ILSize(d, iK));
                    }
                    return empty<double>(new ILSize(0, n));
                }

                // initialize centers by using random datapoints
                ILArray<double> centers = empty<double>();
                if (centerInitRandom) {
                    ILArray<int> pickIndices = 1;
                    sort(rand(1, n), pickIndices, 1, false).Dispose();
                    centers.a = X[full, pickIndices[r(0, iK - 1)]];
                } else {
                    if (!isnull(outCenters) && outCenters.S[0] == d && outCenters.S[1] == iK) {
                        centers.a = outCenters; 
                    } else {
                        centers.a = X[full, r(0, iK - 1)];
                    }
                }

                ILArray<double> classes = zeros<double>(1, n);
                ILArray<double> oldCenters = centers.C;
#if KMEANSVERBOSE
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch(); 
#endif
                while (maxIterations-- > 0) {
#if KMEANSVERBOSE
                    sw.Restart(); 
#endif
                    for (int i = 0; i < n; i++) {
                        // find cluster affiliates
                        using (ILScope.Enter()) {
                            ILArray<int> minDistIdx = 1;
                            //min(sum(abs(centers - X[full, i])), minDistIdx, 1).Dispose();
                            min(distL1(centers, X[full, i]), minDistIdx, 1).Dispose();
                            classes[i] = (double)minDistIdx[0];
                        }
                    }
                    System.Diagnostics.Debug.Print("kmeans: 1 of {0} MemoryPool.Info: {1}", maxIterations, ILMemoryPool.Pool.Info(true));
                    for (int i = 0; i < iK; i++) {
                        using (ILScope.Enter())
                        {
                            ILArray<double> inClass = X[full, find(classes == i)];
                            if (inClass.IsEmpty) {
                                centers[full, i] = double.NaN;
                            } else {
                                centers[full, i] = mean(inClass, 1);
                            }
                        }
                    }
#if KMEANSVERBOSE
                    sw.Stop(); 
                    Console.Out.WriteLine("Changed centers: {0} elapsed: {1}ms",(double)sum(any(oldCenters != centers)), sw.ElapsedMilliseconds); 
#endif
                    if (allall(oldCenters == centers)) break;
                    oldCenters.a = centers.C;
                }
                if (!object.Equals(outCenters, null))
                    outCenters.a = centers;
                return classes;
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// k-means clustering: find clusters in data matrix X
        /// </summary>
        /// <param name="X">Data matrix, data points are given as columns</param>
        /// <param name="k">Initial number of clusters expected</param>
        /// <param name="centerInitRandom">[Optional] false: pick the first k data points as initial centers, true: pick random datapoints (default)</param>
        /// <param name="maxIterations">[Optional] Maximum number of iterations, the computation will exit after that many iterations, default: 10.000</param>
        /// <param name="outCenters">[Input/Output/Optional] If not null on entry, outCenters will contain the centers of the clusters found, default: null</param>
        /// <returns>Vector of length n with with indices of the clustersm which were assigned to each datapoint</returns>
        /// <remarks><para>If <paramref name="outCenters"/> is given not null on input, the algorithm returns the computed centers in that parameter. A 
        /// matrix may be given on input, in order to give a hint of the initial center positions. This may help to find correct cluster centers - even if 
        /// the initial hint is not exact. In order to do so, the matrix given must be of the correct size (X.D[0] by k) and <paramref name="centerInitRandom"/>
        /// must be set to <c>false</c>.</para></remarks>
        public static ILRetArray<float> kMeansClust(ILInArray<float> X, ILBaseArray k, int maxIterations = 10000, 
                                                                    bool centerInitRandom = true, ILOutArray<float> outCenters = null) {
            using (ILScope.Enter(X, k)) {
                if (object.Equals(X, null)) {
                    throw new ILArgumentException("X must be data matrix (not null)");
                }
                if (X.IsEmpty) {
                    if (!object.Equals(outCenters, null)) {
                        if (X.S[0] > 0) {
                            outCenters.a = empty<float>(new ILSize(X.S[0], 0));
                        } else {
                            outCenters.a = empty<float>(new ILSize(0, X.S[1]));
                        }
                        return empty<float>(X.S);
                    }
                }
                if (object.Equals(k, null) || !k.IsScalar || !k.IsNumeric) {
                    throw new ILArgumentException("number of clusters k must be numeric scalar");
                }
                int iK = toint32(k).GetValue(0);
                if (X.S[1] < iK) {
                    throw new ILArgumentException("too few datapoints provided for " + iK.ToString() + " clusters");
                }
                if (iK < 0) {
                    throw new ILArgumentException("number of clusters must be positive");
                }
                int d = X.S[0], n = X.S[1];
                if (iK == 0) {
                    if (!object.Equals(outCenters, null)) {
                        outCenters.a = empty<float>(new ILSize(d, iK));
                    }
                    return empty<float>(new ILSize(0, n));
                }

                // initialize centers by using random datapoints
                ILArray<float> centers = empty<float>();
                if (centerInitRandom) {
                    ILArray<int> pickIndices = 1;
                    sort(rand(1, n), pickIndices, 1, false).Dispose();
                    centers.a = X[full, pickIndices[r(0, iK - 1)]];
                } else {
                    if (!isnull(outCenters) && outCenters.S[0] == d && outCenters.S[1] == iK) {
                        centers.a = outCenters; 
                    } else {
                        centers.a = X[full, r(0, iK - 1)];
                    }
                }

                ILArray<float> classes = zeros<float>(1, n);
                ILArray<float> oldCenters = centers.C;
#if KMEANSVERBOSE
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch(); 
#endif
                while (maxIterations-- > 0) {
#if KMEANSVERBOSE
                    sw.Restart(); 
#endif
                    for (int i = 0; i < n; i++) {
                        // find cluster affiliates
                        using (ILScope.Enter()) {
                            ILArray<int> minDistIdx = 1;
                            //min(sum(abs(centers - X[full, i])), minDistIdx, 1).Dispose();
                            min(distL1(centers, X[full, i]), minDistIdx, 1).Dispose();
                            classes[i] = (float)minDistIdx[0];
                        }
                    }
                    System.Diagnostics.Debug.Print("kmeans: 1 of {0} MemoryPool.Info: {1}", maxIterations, ILMemoryPool.Pool.Info(true));
                    for (int i = 0; i < iK; i++) {
                        using (ILScope.Enter())
                        {
                            ILArray<float> inClass = X[full, find(classes == i)];
                            if (inClass.IsEmpty) {
                                centers[full, i] = float.NaN;
                            } else {
                                centers[full, i] = mean(inClass, 1);
                            }
                        }
                    }
#if KMEANSVERBOSE
                    sw.Stop(); 
                    Console.Out.WriteLine("Changed centers: {0} elapsed: {1}ms",(double)sum(any(oldCenters != centers)), sw.ElapsedMilliseconds); 
#endif
                    if (allall(oldCenters == centers)) break;
                    oldCenters.a = centers.C;
                }
                if (!object.Equals(outCenters, null))
                    outCenters.a = centers;
                return classes;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}
