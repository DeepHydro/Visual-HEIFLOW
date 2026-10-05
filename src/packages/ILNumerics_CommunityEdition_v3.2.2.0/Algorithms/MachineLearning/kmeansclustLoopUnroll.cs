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
        /// find clusters for data matrix X
        /// </summary>
        /// <param name="X">data matrix, data points are given as columns</param>
        /// <param name="k">initial number of clusters expected</param>
        /// <param name="centerInitRandom">false: pick the first k data points as initial centers, true: pick random datapoints</param>
        /// <param name="maxIterations">maximum number of iterations, the computation will exit after that many iterations.</param>
        /// <returns>vector of length n with with indices of clusters assigned to each datapoint</returns>
        public static ILRetArray<double> kMeansClust(ILInArray<double> X, ILBaseArray k, int maxIterations, bool centerInitRandom) {
            return kMeansClust(X, k, maxIterations, centerInitRandom, null);
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
        public static ILRetArray<double> kMeansClust (ILInArray<double> X, ILBaseArray k, int maxIterations, bool centerInitRandom, ILOutArray<double> outCenters) {
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
                        outCenters.a = empty<double>(new ILSize(d, iK)); 
                    }
                    return empty<double>(new ILSize(0, n)); 
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
                while (maxIterations --> 0) {
#if KMEANSVERBOSE
                    sw.Restart(); 
#endif
                    //ILArray<double> distances = zeros(1, iK);

                    double[] Xarr = X.GetArrayForRead();
                    double[] Carr = classes.GetArrayForWrite(); 
                    double[] CentArr = centers.GetArrayForRead(); 
                    double[] Xcur = ILMemoryPool.Pool.New<double>(X.D[0]); 

                    for (int i = 0; i < n; i++) {
                        // copy current X[i]
                        int startInd = i * X.D[0]; 
                        for (int a = X.D[0]; a --> 0; ) {
                            Xcur[a] = Xarr[startInd + a]; 
                        }
                        // distances to all centers
                        double dist = double.MaxValue; 
                        for (int c = 0; c < iK; c++) {
                            double tmp = 0, tmp1 = 0; 
                            startInd = c * X.D[0]; 
                            for (int c1 = X.D[0]; c1-->0; ) {
                                tmp = CentArr[c1 + startInd] -  Xcur[c1]; 
                                tmp1 += tmp * tmp; 
                            }
                            if (tmp1 < dist) {
                                dist = tmp1;
                                Carr[i] = c;
                                if (dist == 0) 
                                    break; 
                            }
                        }
                    }
                    ILMemoryPool.Pool.RegisterObject(Xcur); 

                        // find cluster affiliates
                        //using (ILScope.Enter()) {

                            ////  - for testing a more "similar 2 Fortran" implementation: 
                            //ILArray<double> tmpX = X[full, i];
                            //for (int j = 0; j < iK; j++) {
                            //    using (ILScope.Enter()) {

                            //        //! ... find its nearest cluster 
                            //        //do j = 1, K
                            //        //    distances(j) = sum(                                &
                            //        //                        abs(                           &
                            //        //                            X(1:M,i) - centers(1:M,j)))
                            //        //end do

                            //        //tmpArr = minloc ( distances(1:K) )
                            //        //classes(i) = tmpArr(1);

                            //        distances[j] = sum(abs(tmpX - centers[full, j]));
                            //    }
                            //}
                            //ILArray<double> minDistIdx = empty();
                            //min(distances, minDistIdx, 1).Dispose();
                            //int found = (int)minDistIdx[0];
                            //classes[i] = found;

                            //ILArray<double> minDistIdx = empty();
                            //min(sum(apply((a, b) => { return Math.Abs(a - b); }, centers, repmat(X[full, i], 1, iK))), minDistIdx, 1).Dispose();
                            //int found = (int)minDistIdx[0];
                            //classes[i] = found;



                            //ILArray<double> minDistIdx = empty();
                            //min(sum(abs(centers - repmat(X[full, i], 1, iK))), minDistIdx, 1).Dispose();
                            //int found = (int)minDistIdx[0];
                            //classes[i] = found;
                            
                            //numInClass[found] = numInClass[found] + 1; 
                        //}
                    //}
                    System.Diagnostics.Debug.Print("kmeans: 1 of {0} MemoryPool.Info: {1}",maxIterations, ILMemoryPool.Pool.Info(true));
                    // update centroids
                    //centers[full] = 0; 
                    //for (int i = 0; i < n; i++) {
                    //    centers[full,classes[i]] = centers[full,classes[i]] + X[full,i]; 
                    //}
                    //numInClass[numInClass == 0] = double.NaN; 
                    //centers = centers / repmat(numInClass,d,1); 

                    for (int i = 0; i < iK; i++) {
                        using (EnterScope()) {
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

    }
}