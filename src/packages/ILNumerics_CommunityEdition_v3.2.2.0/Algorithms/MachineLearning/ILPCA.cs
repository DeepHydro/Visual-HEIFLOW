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

namespace ILNumerics {
    // ToDo: DOKU Example PCA

    public partial class ILMath {

        /// <summary>
        /// Principal Component Analysis (PCA)
        /// </summary>
        /// <param name="A">Data matrix, size (m,n); each of n observations is expected in a column of m variables</param>
        /// <param name="outWeights">[Output] Weights for scaling the components according to the original data</param>
        /// <param name="outCenter">[Output] Vector pointing to the center of the input data A</param>
        /// <param name="outScores">[Output] Scaling factors for the component to recreate original data</param>
        /// <returns>PCA components; weights, center and scores are returned at optional output parameters</returns>
        /// <remarks>Principal Component Analysis (PCA) is commonly used as method for dimension reduction. It computes 
        /// a number of 'principal components' which span a space of orthogonal directions. The nice property is, these 
        /// directions are choosen such, as to maximize the variance of original data, once they are projected onto them. 
        /// We can simply pick only a subset of components, having associated a high variance and leave out other components, which do
        /// not contribute much to the distribution of the original data. The resulting subspace is constructed of fewer
        /// dimensions as the original space - with a smaller reconstrution error. Therefore, PCA is commonly used 
        /// for visualizing higher dimensional data in only two or three dimensional plots. It helps analyzing datasets which 
        /// otherwise could only be visualized by picking individual dimensions. By help of PCA, 'interesting' directions 
        /// in the data are identified.
        /// <para>Any output parameter are optional and may be ommited ore provided as null parameter: 
        /// <list type="bullet">
        /// <item><b>components</b> (return value) prinicipal components. Matrix of size m x m, m components are provided in columns. The first 
        /// component marks the direction in the data A, which corresponds to the largest variance (i.e. by projecting the data onto 
        /// that direction, the largest variance would be created). Adjacent components are all orthogonal to each other. 
        /// The components are ordered in columns of decreasing variance.</item>
        /// <item><b>weights</b> vectors. While the components returned are normalized to length 1, the 'weights' vector 
        /// contains the factors needed, to scale the components in order to reflect the real spacial distances in the 
        /// original data.</item>
        /// <item><b>center</b> of the original data. The vector points to the weight middle of A.</item>
        /// <item><b>scores</b> is a matrix of size m by n. For each datapoint given in A, it contains factors for each component 
        /// needed to reproduce the original data point in terms of the components.</item>
        /// </list></para>
        /// </remarks>
        public static ILRetArray<double> pca(ILInArray<double> A,ILOutArray<double> outWeights = null, 
                                              ILOutArray<double> outCenter = null, ILOutArray<double> outScores = null) {
            using (ILScope.Enter(A)) {
                if (isnull(A))
                    throw new Exceptions.ILArgumentException("data input argument A must not be null"); 
                ILArray<double> ret = empty(); 
                if (!A.IsEmpty) {
                    ILArray<double> Center = sum(A, 1) / A.S[1];
                    ILArray<double> centeredA = A - Center;
                    if (!isnull(outWeights)) {
                        outWeights.a = eigSymm(multiply(centeredA, centeredA.T), ret);
                        if (!outWeights.IsVector) {
                            outWeights.a = diag(outWeights);
                        }
                        outWeights.a = flipud(outWeights);
                    } else {
                        eigSymm(multiply(centeredA, centeredA.T), ret).Dispose();
                    }
                    ret = fliplr(ret); 
                    if (!isnull(outScores))
                        outScores.a = multiply(ret.T, centeredA);
                    if (!isnull(outCenter)) 
                        outCenter.a = Center; 
                    return ret;
                } else {
                    if (!isnull(outWeights))
                        outWeights.a = empty<double>(); 
                    if (!isnull(outCenter)) 
                        outCenter.a = empty<double>(); 
                    if (!isnull(outScores))
                        outScores.a = empty<double>(); 
                    return empty<double>(); 
                }
            }
        }

        /// <summary>
        /// Principal Component Analysis (PCA)
        /// </summary>
        /// <param name="A">Data matrix, size (m,n); each of n observations is expected in a column of m variables</param>
        /// <param name="outWeights">[Output] Weights for scaling the components according to the original data</param>
        /// <param name="outCenter">[Output] Vector pointing to the center of the input data A</param>
        /// <param name="outScores">[Output] Scaling factors for the component to recreate original data</param>
        /// <returns>PCA components; weights, center and scores are returned at optional output parameters</returns>
        /// <remarks>Principal Component Analysis (PCA) is commonly used as method for dimension reduction. It computes 
        /// a number of 'principal components' which span a space of orthogonal directions. The nice property is, these 
        /// directions are choosen such, as to maximize the variance of original data, once they are projected onto them. 
        /// We can simply pick only a subset of components, having associated a high variance and leave out other components, which do
        /// not contribute much to the distribution of the original data. The resulting subspace is constructed of fewer
        /// dimensions as the original space - with a smaller reconstrution error. Therefore, PCA is commonly used 
        /// for visualizing higher dimensional data in only two or three dimensional plots. It helps analyzing datasets which 
        /// otherwise could only be visualized by picking individual dimensions. By help of PCA, 'interesting' directions 
        /// in the data are identified.
        /// <para>Any output parameter are optional and may be ommited ore provided as null parameter: 
        /// <list type="bullet">
        /// <item><b>components</b> (return value) prinicipal components. Matrix of size m x m, m components ar provided in columns. The first 
        /// component marks the direction in the data A, which corresponds to the largest variance (i.e. by projecting the data onto 
        /// that direction, the largest variance would be created). Adjacent components are all orthogonal to each other. 
        /// The components are ordered in columns of decreasing variance.</item>
        /// <item><b>weights</b> vectors. While the components returned are normalized to length 1, the 'weights' vector 
        /// contains the factors needed, to scale the components in order to reflect the real spacial distances in the 
        /// original data.</item>
        /// <item><b>center</b> of the original data. The vector points to the weight middle of A.</item>
        /// <item><b>scores</b> is a matrix of size m by n. For each datapoint given in A, it contains factors for each component 
        /// needed to reproduce the original data point in terms of the components.</item>
        /// </list></para>
        /// </remarks>
        public static ILRetArray<float> pca(ILInArray<float> A, ILOutArray<float> outWeights = null,
                                              ILOutArray<float> outCenter = null, ILOutArray<float> outScores = null) {
            using (ILScope.Enter(A)) {
                if (isnull(A))
                    throw new Exceptions.ILArgumentException("data input argument A must not be null");
                ILArray<float> ret = empty<float>();
                if (!A.IsEmpty) {
                    ILArray<float> Center = sum(A, 1) / A.S[1];
                    ILArray<float> centeredA = A - Center;
                    if (!isnull(outWeights)) {
                        outWeights.a = eigSymm(multiply(centeredA, centeredA.T), ret);
                        if (!outWeights.IsVector) {
                            outWeights.a = diag(outWeights);
                        }
                        outWeights.a = flipud<float>(outWeights);
                    } else {
                        eigSymm(multiply(centeredA, centeredA.T), ret).Dispose();
                    }
                    if (!isnull(outScores))
                        outScores.a = multiply(ret.T, centeredA);
                    if (!isnull(outCenter))
                        outCenter.a = Center;
                    return fliplr<float>(ret);
                } else {
                    if (!isnull(outWeights))
                        outWeights.a = empty<float>();
                    if (!isnull(outCenter))
                        outCenter.a = empty<float>();
                    if (!isnull(outScores))
                        outScores.a = empty<float>();
                    return empty<float>();
                }
            }
        }
    }
}
