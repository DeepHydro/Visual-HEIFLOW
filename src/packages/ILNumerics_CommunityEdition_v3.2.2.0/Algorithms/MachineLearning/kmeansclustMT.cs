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
using System.Threading; 
using System.Text;
using ILNumerics.Exceptions; 

namespace ILNumerics {
    
    
    public partial class ILMath { 

        /// <summary>
        /// find clusters for data matrix X
        /// </summary>
        /// <param name="X">data matrix, data points are given as columns</param>
        /// <param name="k">initial number of clusters expected</param>
        /// <param name="centerInitRandom">false: pick the first k data points as initial centers, true: pick random datapoints</param>
        /// <param name="maxIterations">maximum number of iterations, the computation will exit after that many iterations.</param>
        /// <returns>vector of length n with with indices of clusters assigned to each datapoint</returns>
        public static ILRetArray<double> kMeansClustMT(ILInArray<double> X, ILBaseArray k, int maxIterations, bool centerInitRandom) {
            return kMeansClustMT(X, k, maxIterations, centerInitRandom, null);
        }
        /// <summary>
        /// find clusters for data matrix X
        /// </summary>
        /// <param name="X">data matrix, data points are given as columns</param>
        /// <param name="k">initial number of clusters expected</param>
        /// <param name="centerInitRandom">false: pick the first k data points as initial centers, true: pick random datapoints</param>
        /// <param name="maxIterations">maximum number of iterations, the computation will exit after that many iterations.</param>
        /// <param name="outCenters">return type. if assigned on entry, outCenters will contain the centers of the clusters found.</param>
        /// <returns>vector of length n with with indices of clusters assigned to each datapoint</returns>
        public static ILRetArray<double> kMeansClustMT (ILInArray<double> X, ILBaseArray k, int maxIterations, bool centerInitRandom, ILOutArray<double> outCenters) {
            using (ILScope.Enter(X, k)) {
                if (object.Equals(X,null)) {
                    throw new ILArgumentException("X must be data matrix (not null)"); 
                }
                if (X.IsEmpty) {
                    if (!object.Equals(outCenters, null)) {
                        if (X.D[0] > 0) {
                            outCenters.a = empty<double>(new ILSize(X.D[0], 0));
                        } else {
                            outCenters.a = empty<double>(new ILSize(0, X.D[1]));
                        }
                        return empty<double>(X.D);
                    }
                }
                if (object.Equals(k,null) || !k.IsScalar || !k.IsNumeric) {
                    throw new ILArgumentException("number of clusters k must be numeric scalar"); 
                }
                int iK = toint32(k).GetValue(0);
                if (X.D[1] < iK) {
                    throw new ILArgumentException("too few datapoints provided for " + iK.ToString() + " clusters");
                }
                if (iK < 0) {
                    throw new ILArgumentException("number of clusters must be positive");
                }
                int d = X.D[0], n = X.D[1]; 
                if (iK == 0) {
                    if (!object.Equals(outCenters, null)) {
                        outCenters.a = empty<double>(new ILSize(d,iK)); 
                    }
                    return empty<double>(new ILSize(0,n)); 
                }

                // initialize centers by using random datapoints
                ILArray<double> centers = empty();
                if (centerInitRandom) {
                    ILArray<double> pickIndices = empty(); 
                    sort(rand(1,n),pickIndices,1,false).Dispose();
                    centers.a = X[full,pickIndices[r(0,iK-1)]]; 
                } else {
                    centers.a = X[full,r(0,iK-1)]; 
                }

                ILArray<double> classes = zeros(1,n);
                ILArray<double> oldCenters = centers.C; 
#if KMEANSVERBOSE
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch(); 
#endif
                int maxNTSetting = Settings.MaxNumberThreads, workerCount = 1, workItemLen = 0;
                int workItemCount = maxNTSetting; 
                int wi = 0; 
                Settings.MaxNumberThreads = 1;
                Action<object> loopOverN = data => {
                        Tuple<ILInArray<double>, ILInArray<double>, ILOutArray<double>> rng
                            = (Tuple<ILInArray<double>, ILInArray<double>, ILOutArray<double>>)data;
                        try {
                            using (ILScope.Enter(rng.Item1, rng.Item2)) {
                                ILArray<double> Xl = rng.Item1;
                                ILArray<double> centersl = rng.Item2;
                                for (int i = Xl.D[1]; i-- > 0; ) {
                                    ILArray<double> minDistIdx = empty();
                                    min(sum(abs(centersl - repmat(Xl[full, i], 1, iK))), minDistIdx, 1).Dispose();
                                    rng.Item3[i] = minDistIdx[0];
                                }
                            }
                        } finally {
                            Interlocked.Decrement(ref workerCount);
                        }
                };
                List<ILArray<double>> classesSplit = new List<ILArray<double>>(workItemCount);
                List<ILArray<double>> XSplit = new List<ILArray<double>>(workItemCount); 
                
                while (maxIterations --> 0) {
#if KMEANSVERBOSE
                    sw.Restart(); 
#endif
                    workItemLen = n / workItemCount;
                    workerCount = 1; 
                    for (wi = 0; wi < workItemCount - 1; wi++) {
                        if (classesSplit.Count <= wi) {
                            classesSplit.Add(zeros<double>(1,workItemLen)); 
                        }
                        if (XSplit.Count <= wi) {
                            XSplit.Add(X[full,r(wi * workItemLen, (wi + 1) * workItemLen - 1)]); 
                        }
                        Tuple<ILInArray<double>, ILInArray<double>, ILOutArray<double>> rng
                            = new Tuple<ILInArray<double>, ILInArray<double>, ILOutArray<double>>(
                                XSplit[wi], centers.C, classesSplit[wi]);
                        Interlocked.Increment(ref workerCount);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(loopOverN), rng);
                    }
                    // loop for main thread
                    ILArray<double> tmpOutClasses = zeros<double>(1,n - wi * workItemLen); 
                    Tuple<ILInArray<double>, ILInArray<double>, ILOutArray<double>> rngL
                        = new Tuple<ILInArray<double>, ILInArray<double>, ILOutArray<double>>(
                            X[full, r(wi * workItemLen, n - 1)], centers.C, tmpOutClasses);
                    loopOverN(rngL);
                    classes[r(wi * workItemLen, n - 1)] = tmpOutClasses; 

                    SpinWait.SpinUntil(() => { return workerCount <= 0; });
                    Settings.MaxNumberThreads = maxNTSetting;
                    // resamble
                    for (wi = 0; wi < workItemCount - 1; wi++)
                    {
                        classes[r(wi * workItemLen, (wi + 1)* workItemLen - 1)] = classesSplit[wi]; 
			        }
                    System.Diagnostics.Debug.Print("kmeans: 1 of {0} MemoryPool.Info: {1}", maxIterations, ILMemoryPool.Pool.Info(true));
                    for (int i = 0; i < iK; i++) {
                        using (EnterScope()) {
                            ILArray<double> inClass = X[full, find(classes == i)];
                            if (inClass.IsEmpty) {
                                centers[full, i] = double.NaN;
                            } else {
                                centers[full, i] = mean(inClass, 1);
                                ILArray<double> inClassDiff = inClass - repmat(centers[full, i], 1, size(inClass, 1));
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

    }
}