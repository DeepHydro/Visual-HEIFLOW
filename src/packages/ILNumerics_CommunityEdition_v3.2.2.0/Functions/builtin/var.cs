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
        /// Variance along dimension of A
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="Weights">[Optional] Vector of scaling factors, same length as working dimension of A, default: no scaling</param>
        /// <param name="biased">[Optional] true: apply biased normalization to result, default: false (non-biased)</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Variances</returns>
        /// <remarks><para>On scalar A a scalar 0 of the same shape as A is returned.</para>
        /// <para>On empty A an empty array is returned, having the dimension to operate along reduced to length 1.</para>
        /// <para>The parameters <paramref name="Weights"/>, <paramref name="biased"/> and <paramref name="dim"/> are optional. 
        /// Ommiting either one will choose its respective default value.</para>
        /// <para> The result for <paramref name="biased"/> = true is computed by the following formula: 
        /// <code>r = (A - mean(A)); 
        /// var = sum(r * r) / A.D[dim];</code>
        /// If <paramref name="biased"/> is false (default) the normalization is done with the length of the working dimension of A as follows: 
        /// <code>r = (A - mean(A)); 
        /// var = sum(r * r) / (A.D[dim] - 1); </code>
        /// If <paramref name="Weights"/> is given, the parameter <paramref name="biased"/> is ignored.</para>
        /// <para>If <paramref name="Weights"/> is given, the normalization is applied to r as follows: 
        /// <code>w = w / sum(w); 
        /// r = A - sum(w * A);
        /// var = sum(w * (r * r)); 
        /// </code></para></remarks>
        public static ILRetArray<double> var(ILInArray<double> A, 
                                                            ILInArray<double> Weights = null, 
                                                            bool biased = false, int dim = -1) {
            using (ILScope.Enter(A, Weights)) {
                if (isnull(A)) 
                    throw new ILArgumentException("input parameter A must not be null");
                if (A.IsScalar) {
                    return zeros<double>(A.S); 
                }
                if (dim == -1) {
                    dim = A.S.WorkingDimension(); 
                    if (dim < 0) 
                        dim = 0; 
                }
                if (A.IsEmpty) {
                    int[] dims = A.S.ToIntArray(); 
                    dims[dim] = 1;
                    return zeros<double>(dims); 
                }
                int n = A.S[dim]; 
                if (isnull(Weights)) {
                    // unweighted
                    ILArray<double> tmp = n; 
                    if (!biased && n > 1) {
                        tmp.a = tmp - 1; 
                    }
                    ILArray<double> tmpM = sum(A, dim) / n;
                    ILArray<double> AminTmpM = A - tmpM;
                    return sum(AminTmpM * AminTmpM, dim) / tmp; 
                } else {
                    // weighted 
                    if (!Weights.IsVector || Weights.S.NumberOfElements != n) {
                        throw new ILArgumentException("Weights parameter must be a vector of the length of the working dimension of A"); 
                    }
                    if (any(Weights < 0)) {
                        throw new ILArgumentException("values of Weights parameter must all be positive");
                    }
                    ILArray<double> locWeights; 
                    if (!A.IsMatrix) {
                        // vector expansion currently only works for vectors on matrices
                        ILArray<int> wdims = ones<int>(Math.Max(dim, ndims(A))); 
                        wdims[dim] = Weights.Length;
                        locWeights = reshape(Weights, new ILSize(wdims)) / sumall(Weights);
                        int[] repDims = A.S.ToIntArray(); 
                        repDims[dim] = 1;
                        locWeights = repmat(locWeights, repDims); 
                    }
                    ILArray<double> r = A - sum(Weights * A, dim);
                    return sum(Weights * (r * r), dim);
                }
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Variance along dimension of A
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="Weights">[Optional] Vector of scaling factors, same length as working dimension of A, default: no scaling</param>
        /// <param name="biased">[Optional] true: apply biased normalization to result, default: false (non-biased)</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Variances</returns>
        /// <remarks><para>On scalar A a scalar 0 of the same shape as A is returned.</para>
        /// <para>On empty A an empty array is returned, having the dimension to operate along reduced to length 1.</para>
        /// <para>The parameters <paramref name="Weights"/>, <paramref name="biased"/> and <paramref name="dim"/> are optional. 
        /// Ommiting either one will choose its respective default value.</para>
        /// <para> The result for <paramref name="biased"/> = true is computed by the following formula: 
        /// <code>r = (A - mean(A)); 
        /// var = sum(r * r) / A.D[dim];</code>
        /// If <paramref name="biased"/> is false (default) the normalization is done with the length of the working dimension of A as follows: 
        /// <code>r = (A - mean(A)); 
        /// var = sum(r * r) / (A.D[dim] - 1); </code>
        /// If <paramref name="Weights"/> is given, the parameter <paramref name="biased"/> is ignored.</para>
        /// <para>If <paramref name="Weights"/> is given, the normalization is applied to r as follows: 
        /// <code>w = w / sum(w); 
        /// r = A - sum(w * A);
        /// var = sum(w * (r * r)); 
        /// </code></para></remarks>
        public static ILRetArray<float> var(ILInArray<float> A, 
                                                            ILInArray<float> Weights = null, 
                                                            bool biased = false, int dim = -1) {
            using (ILScope.Enter(A, Weights)) {
                if (isnull(A)) 
                    throw new ILArgumentException("input parameter A must not be null");
                if (A.IsScalar) {
                    return zeros<float>(A.S); 
                }
                if (dim == -1) {
                    dim = A.S.WorkingDimension(); 
                    if (dim < 0) 
                        dim = 0; 
                }
                if (A.IsEmpty) {
                    int[] dims = A.S.ToIntArray(); 
                    dims[dim] = 1;
                    return zeros<float>(dims); 
                }
                int n = A.S[dim]; 
                if (isnull(Weights)) {
                    // unweighted
                    ILArray<float> tmp = n; 
                    if (!biased && n > 1) {
                        tmp.a = tmp - 1; 
                    }
                    ILArray<float> tmpM = sum(A, dim) / n;
                    ILArray<float> AminTmpM = A - tmpM;
                    return sum(AminTmpM * AminTmpM, dim) / tmp; 
                } else {
                    // weighted 
                    if (!Weights.IsVector || Weights.S.NumberOfElements != n) {
                        throw new ILArgumentException("Weights parameter must be a vector of the length of the working dimension of A"); 
                    }
                    if (any(Weights < 0)) {
                        throw new ILArgumentException("values of Weights parameter must all be positive");
                    }
                    ILArray<float> locWeights; 
                    if (!A.IsMatrix) {
                        // vector expansion currently only works for vectors on matrices
                        ILArray<int> wdims = ones<int>(Math.Max(dim, ndims(A))); 
                        wdims[dim] = Weights.Length;
                        locWeights = reshape(Weights, new ILSize(wdims)) / sumall(Weights);
                        int[] repDims = A.S.ToIntArray(); 
                        repDims[dim] = 1;
                        locWeights = repmat(locWeights, repDims); 
                    }
                    ILArray<float> r = A - sum(Weights * A, dim);
                    return sum(Weights * (r * r), dim);
                }
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}