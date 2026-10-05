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
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Exceptions;



namespace ILNumerics  {

    public partial class ILMath {
        

        /// <summary>
        /// Vector or matrix norm
        /// </summary>
        /// <param name="A">input matrix or vector</param>
        /// <param name="degree">[optional] degree of norm (default: 2)</param>
        /// <returns>Array of same type as input array A</returns>
        /// <remarks>For vectors, <paramref name="degree"/> must be one of: 
        /// <list type="bullet">
        /// <item>0 : returns sqrt(sum(A * A))</item>
        /// <item>arbitrary double value : returns sum(pow(abs(A),degree))^(1/degree)</item>
        /// <item>System.double.PositiveInfinity: return Max(abs(A))</item>
        /// <item>System.double.NegativeInfinity: return Min(abs(A))</item>
        /// </list>
        /// </para>
        /// For matrices <paramref name="degree"/> must be one out of: 
        /// <list type="bullet">
        /// <item>0: returns Frobenius norm: sqrt(sum(diag(multiply(A, A[1]))))</item>
        /// <item>1: returns 1-norm, max(sum(abs(A)))</item>
        /// <item>2: returns the largest singular value of A, max(svd(A))</item>
        /// <item>PositiveInfinity: returns maxall(sum(abs(A), 1)), the largest value of the sums along the rows</item>
        /// </list>
        /// <para>norm(A,0) with A being a vector extends naturally to the frobenius norm for matrices.</para>
        /// <para>For empty arrays A, scalar 0 is returned.</para>
        /// </remarks>
        public static ILRetArray< double> norm(ILInArray< double> A, double degree = 2) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A, null) || !A.IsMatrix)
                    throw new ILArgumentSizeException("input array must be matrix or vector.");
                if (A.IsEmpty)
                    return new ILRetArray< double>(ILSize.Scalar1_1);
                else if (A.IsVector) {
                    if (degree == Double.PositiveInfinity) {
                        return max(abs(A));
                    } else if (degree == Double.NegativeInfinity) {
                        return min(abs(A));
                    } else {
                        if (degree == 0.0)
                            
                            return sqrt(sum(A * A));
                        return pow(sum(pow(abs(A), (  double)degree)), (  double)(1.0 / degree));
                    }
                } else {
                    if (degree == 1.0) {
                        return max(sum(abs(A)));
                    } else if (degree == 2.0) {
                        return max(svd(A));
                    } else if (degree == Double.PositiveInfinity) {
                        return maxall(sum(abs(A), 1));
                    } else if (degree == 0.0) {
                        
                        return sqrt(sum(diag <double>(multiply(A, A.T))));
                    } else {
                        throw new ILArgumentException("invalid argument 'degree' for input matrix A. Valid options are: 0,1,2,Double.PositiveInfinity");
                    }
                }
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Vector or matrix norm
        /// </summary>
        /// <param name="A">input matrix or vector</param>
        /// <param name="degree">[optional] degree of norm (default: 2)</param>
        /// <returns>Array of same type as input array A</returns>
        /// <remarks>For vectors, <paramref name="degree"/> must be one of: 
        /// <list type="bullet">
        /// <item>0 : returns sqrt(sum(A * A))</item>
        /// <item>arbitrary double value : returns sum(pow(abs(A),degree))^(1/degree)</item>
        /// <item>System.double.PositiveInfinity: return Max(abs(A))</item>
        /// <item>System.double.NegativeInfinity: return Min(abs(A))</item>
        /// </list>
        /// </para>
        /// For matrices <paramref name="degree"/> must be one out of: 
        /// <list type="bullet">
        /// <item>0: returns Frobenius norm: sqrt(sum(diag(multiply(A, A[1]))))</item>
        /// <item>1: returns 1-norm, max(sum(abs(A)))</item>
        /// <item>2: returns the largest singular value of A, max(svd(A))</item>
        /// <item>PositiveInfinity: returns maxall(sum(abs(A), 1)), the largest value of the sums along the rows</item>
        /// </list>
        /// <para>norm(A,0) with A being a vector extends naturally to the frobenius norm for matrices.</para>
        /// <para>For empty arrays A, scalar 0 is returned.</para>
        /// </remarks>
        public static ILRetArray< float> norm(ILInArray< fcomplex> A, double degree = 2) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A, null) || !A.IsMatrix)
                    throw new ILArgumentSizeException("input array must be matrix or vector.");
                if (A.IsEmpty)
                    return new ILRetArray< float>(ILSize.Scalar1_1);
                else if (A.IsVector) {
                    if (degree == Double.PositiveInfinity) {
                        return max(abs(A));
                    } else if (degree == Double.NegativeInfinity) {
                        return min(abs(A));
                    } else {
                        if (degree == 0.0)
                            return sqrt(real(sum(A * conj(A))));
                        return pow(sum(pow(abs(A), (  float)degree)), (  float)(1.0 / degree));
                    }
                } else {
                    if (degree == 1.0) {
                        return max(sum(abs(A)));
                    } else if (degree == 2.0) {
                        return max(svd(A));
                    } else if (degree == Double.PositiveInfinity) {
                        return maxall(sum(abs(A), 1));
                    } else if (degree == 0.0) {
                        return sqrt(sum(real(diag<fcomplex>(multiply(A, conj(A.T))))));
                    } else {
                        throw new ILArgumentException("invalid argument 'degree' for input matrix A. Valid options are: 0,1,2,Double.PositiveInfinity");
                    }
                }
            }
        }
        /// <summary>
        /// Vector or matrix norm
        /// </summary>
        /// <param name="A">input matrix or vector</param>
        /// <param name="degree">[optional] degree of norm (default: 2)</param>
        /// <returns>Array of same type as input array A</returns>
        /// <remarks>For vectors, <paramref name="degree"/> must be one of: 
        /// <list type="bullet">
        /// <item>0 : returns sqrt(sum(A * A))</item>
        /// <item>arbitrary double value : returns sum(pow(abs(A),degree))^(1/degree)</item>
        /// <item>System.double.PositiveInfinity: return Max(abs(A))</item>
        /// <item>System.double.NegativeInfinity: return Min(abs(A))</item>
        /// </list>
        /// </para>
        /// For matrices <paramref name="degree"/> must be one out of: 
        /// <list type="bullet">
        /// <item>0: returns Frobenius norm: sqrt(sum(diag(multiply(A, A[1]))))</item>
        /// <item>1: returns 1-norm, max(sum(abs(A)))</item>
        /// <item>2: returns the largest singular value of A, max(svd(A))</item>
        /// <item>PositiveInfinity: returns maxall(sum(abs(A), 1)), the largest value of the sums along the rows</item>
        /// </list>
        /// <para>norm(A,0) with A being a vector extends naturally to the frobenius norm for matrices.</para>
        /// <para>For empty arrays A, scalar 0 is returned.</para>
        /// </remarks>
        public static ILRetArray< float> norm(ILInArray< float> A, double degree = 2) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A, null) || !A.IsMatrix)
                    throw new ILArgumentSizeException("input array must be matrix or vector.");
                if (A.IsEmpty)
                    return new ILRetArray< float>(ILSize.Scalar1_1);
                else if (A.IsVector) {
                    if (degree == Double.PositiveInfinity) {
                        return max(abs(A));
                    } else if (degree == Double.NegativeInfinity) {
                        return min(abs(A));
                    } else {
                        if (degree == 0.0)
                            return sqrt(sum(A * A));
                        return pow(sum(pow(abs(A), (  float)degree)), (  float)(1.0 / degree));
                    }
                } else {
                    if (degree == 1.0) {
                        return max(sum(abs(A)));
                    } else if (degree == 2.0) {
                        return max(svd(A));
                    } else if (degree == Double.PositiveInfinity) {
                        return maxall(sum(abs(A), 1));
                    } else if (degree == 0.0) {
                        return sqrt(sum(diag<float>(multiply(A, A.T))));
                    } else {
                        throw new ILArgumentException("invalid argument 'degree' for input matrix A. Valid options are: 0,1,2,Double.PositiveInfinity");
                    }
                }
            }
        }
        /// <summary>
        /// Vector or matrix norm
        /// </summary>
        /// <param name="A">input matrix or vector</param>
        /// <param name="degree">[optional] degree of norm (default: 2)</param>
        /// <returns>Array of same type as input array A</returns>
        /// <remarks>For vectors, <paramref name="degree"/> must be one of: 
        /// <list type="bullet">
        /// <item>0 : returns sqrt(sum(A * A))</item>
        /// <item>arbitrary double value : returns sum(pow(abs(A),degree))^(1/degree)</item>
        /// <item>System.double.PositiveInfinity: return Max(abs(A))</item>
        /// <item>System.double.NegativeInfinity: return Min(abs(A))</item>
        /// </list>
        /// </para>
        /// For matrices <paramref name="degree"/> must be one out of: 
        /// <list type="bullet">
        /// <item>0: returns Frobenius norm: sqrt(sum(diag(multiply(A, A[1]))))</item>
        /// <item>1: returns 1-norm, max(sum(abs(A)))</item>
        /// <item>2: returns the largest singular value of A, max(svd(A))</item>
        /// <item>PositiveInfinity: returns maxall(sum(abs(A), 1)), the largest value of the sums along the rows</item>
        /// </list>
        /// <para>norm(A,0) with A being a vector extends naturally to the frobenius norm for matrices.</para>
        /// <para>For empty arrays A, scalar 0 is returned.</para>
        /// </remarks>
        public static ILRetArray< double> norm(ILInArray< complex> A, double degree = 2) {
            using (ILScope.Enter(A)) {
                if (Object.Equals(A, null) || !A.IsMatrix)
                    throw new ILArgumentSizeException("input array must be matrix or vector.");
                if (A.IsEmpty)
                    return new ILRetArray< double>(ILSize.Scalar1_1);
                else if (A.IsVector) {
                    if (degree == Double.PositiveInfinity) {
                        return max(abs(A));
                    } else if (degree == Double.NegativeInfinity) {
                        return min(abs(A));
                    } else {
                        if (degree == 0.0)
                            return sqrt(real(sum(A * conj(A))));
                        return pow(sum(pow(abs(A), (  double)degree)), (  double)(1.0 / degree));
                    }
                } else {
                    if (degree == 1.0) {
                        return max(sum(abs(A)));
                    } else if (degree == 2.0) {
                        return max(svd(A));
                    } else if (degree == Double.PositiveInfinity) {
                        return maxall(sum(abs(A), 1));
                    } else if (degree == 0.0) {
                        return sqrt(sum(real(diag<complex>(multiply(A, conj(A.T))))));
                    } else {
                        throw new ILArgumentException("invalid argument 'degree' for input matrix A. Valid options are: 0,1,2,Double.PositiveInfinity");
                    }
                }
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }

}
