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
        /// Covariance matrix of A
        /// </summary>
        /// <param name="A">Input vector or data matrix, samples in columns, variables in rows</param>
        /// <param name="unbiased">[Optional] If true, calculate the best unbiased variance estimate if the observations are from a normal distribution. This normalizes by n-1 if n>1 (n = number of samples). If n == 1 normalization is always 1. If false always normalize by n.</param>
        /// <returns>Variance of vector A/Covariance matrix of A</returns>
        /// <remarks><para>If A is a vector <c>cov(A)</c> returns the variance of A</para>
        /// <para>If A is a m x n matrix, where each of the n columns is an m-dimensional observation, <c>cov(A)</c> is the n x n covariance matrix.</para>
        /// <para>The mean is removed from each column before calculating the result.</para>
        /// </remarks>
        public static ILRetArray<double> cov(ILInArray<double> A, bool unbiased = true) {
            using (ILScope.Enter(A)) {
                if (isnull(A)) {
                    throw new ILArgumentException("Parameter A must not be null");
                }
                if (!A.IsMatrix)
                    throw new ILArgumentException("Input array A must be a matrix (2d)");
                if (A.IsEmpty) {
                    if (A.S[0] == 0 && A.S[1] == 0)
                        return array<double>(double.NaN, 1, 1);
                    return array<double>(double.NaN, A.S[0], A.S[0]);  
                }

                if (A.IsVector)
                {
                    // A vector, return variance
                    int normFactor = unbiased ? (A.Size.NumberOfElements > 1 ? A.Size.NumberOfElements - 1 : 1) : A.Size.NumberOfElements;
                    ILArray<double> AnoMean = A - mean(A);
                    return sum(multiplyElem(AnoMean, AnoMean)) / (double )normFactor;
                    // return zeros<double>(A.D[0], A.D[0]);
                }
                else
                {
                    int normFactor = unbiased ? (A.S[1] > 1 ? A.S[1] - 1 : 1) : A.S[1];
                    ILArray<double> AnoMean = A - mean(A, 1);
                    return multiply(AnoMean, AnoMean.T) / (double )normFactor;
                }
            }
        }

#region HYCALPER AUTO GENERATED CODE

        
        /// <summary>
        /// Covariance matrix of A
        /// </summary>
        /// <param name="A">Input vector or data matrix, samples in columns, variables in rows</param>
        /// <param name="unbiased">[Optional] If true, calculate the best unbiased variance estimate if the observations are from a normal distribution. This normalizes by n-1 if n>1 (n = number of samples). If n == 1 normalization is always 1. If false always normalize by n.</param>
        /// <returns>Variance of vector A/Covariance matrix of A</returns>
        /// <remarks><para>If A is a vector <c>cov(A)</c> returns the variance of A</para>
        /// <para>If A is a m x n matrix, where each of the n columns is an m-dimensional observation, <c>cov(A)</c> is the n x n covariance matrix.</para>
        /// <para>The mean is removed from each column before calculating the result.</para>
        /// </remarks>
        public static ILRetArray<float> cov(ILInArray<float> A, bool unbiased = true) {
            using (ILScope.Enter(A)) {
                if (isnull(A)) {
                    throw new ILArgumentException("Parameter A must not be null");
                }
                if (!A.IsMatrix)
                    throw new ILArgumentException("Input array A must be a matrix (2d)");
                if (A.IsEmpty) {
                    if (A.S[0] == 0 && A.S[1] == 0)
                        return array<float>(float.NaN, 1, 1);
                    return array<float>(float.NaN, A.S[0], A.S[0]);  
                }

                if (A.IsVector)
                {
                    // A vector, return variance
                    int normFactor = unbiased ? (A.Size.NumberOfElements > 1 ? A.Size.NumberOfElements - 1 : 1) : A.Size.NumberOfElements;
                    ILArray<float> AnoMean = A - mean(A);
                    return sum(multiplyElem(AnoMean, AnoMean)) / (float )normFactor;
                    // return zeros<float>(A.D[0], A.D[0]);
                }
                else
                {
                    int normFactor = unbiased ? (A.S[1] > 1 ? A.S[1] - 1 : 1) : A.S[1];
                    ILArray<float> AnoMean = A - mean(A, 1);
                    return multiply(AnoMean, AnoMean.T) / (float )normFactor;
                }
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}