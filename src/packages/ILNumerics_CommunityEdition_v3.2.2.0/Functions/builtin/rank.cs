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
        /// Rank of matrix A
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="tolerance">[Optional]Tolerance used to decide, if a singular value is 
        /// treated as zero. If a value &lt; 0 is specified the tolerance will be determined automatically (see below - default = -1.0)</param>
        /// <returns>Rank of matrix A</returns>
        /// <remarks>The rank is the number of singular values greater than 
        /// tolerance. If tolerance is smaller than zero, the following equation is used as 
        /// default: \\
        /// tol = length(A) * norm(A) * MachineParameterDouble.epsilon \\
        /// with 
        /// <list type="bullet">
        /// <item>length(A) - the longest dimension of A</item>
        /// <item>norm(A) being the largest singular value of A, </item>
        /// <item>MachineParameterDouble.eps - the distance between 1 and the smallest next greater value</item>
        /// </list>
        /// </remarks>
        public static ILRetArray< double> rank( ILInArray< double> A, double tolerance = -1.0 ) {
            using (ILScope.Enter(A)) {
                if (A.Size.NumberOfDimensions > 2)
                    throw new ILArgumentSizeException("The input array must be matrix or vector");
                ILArray< double> ret = svd(A);
                if (tolerance < 0) {
                    tolerance = A.Size.Longest * max(ret).GetValue(0) *  MachineParameterDouble.eps;
                }
                // count vector elements: ret is vector returned from svd
                return find(ret > ( double)tolerance).Length;
            }
        }

#region HYCALPER AUTO GENERATED CODE



        /// <summary>
        /// Rank of matrix A
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="tolerance">[Optional]Tolerance used to decide, if a singular value is 
        /// treated as zero. If a value &lt; 0 is specified the tolerance will be determined automatically (see below - default = -1.0)</param>
        /// <returns>Rank of matrix A</returns>
        /// <remarks>The rank is the number of singular values greater than 
        /// tolerance. If tolerance is smaller than zero, the following equation is used as 
        /// default: \\
        /// tol = length(A) * norm(A) * MachineParameterDouble.epsilon \\
        /// with 
        /// <list type="bullet">
        /// <item>length(A) - the longest dimension of A</item>
        /// <item>norm(A) being the largest singular value of A, </item>
        /// <item>MachineParameterDouble.eps - the distance between 1 and the smallest next greater value</item>
        /// </list>
        /// </remarks>
        public static ILRetArray< float> rank( ILInArray< fcomplex> A, double tolerance = -1.0 ) {
            using (ILScope.Enter(A)) {
                if (A.Size.NumberOfDimensions > 2)
                    throw new ILArgumentSizeException("The input array must be matrix or vector");
                ILArray< float> ret = svd(A);
                if (tolerance < 0) {
                    tolerance = A.Size.Longest * max(ret).GetValue(0) *  MachineParameterSingle.eps;
                }
                // count vector elements: ret is vector returned from svd
                return find(ret > ( float)tolerance).Length;
            }
        }


        /// <summary>
        /// Rank of matrix A
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="tolerance">[Optional]Tolerance used to decide, if a singular value is 
        /// treated as zero. If a value &lt; 0 is specified the tolerance will be determined automatically (see below - default = -1.0)</param>
        /// <returns>Rank of matrix A</returns>
        /// <remarks>The rank is the number of singular values greater than 
        /// tolerance. If tolerance is smaller than zero, the following equation is used as 
        /// default: \\
        /// tol = length(A) * norm(A) * MachineParameterDouble.epsilon \\
        /// with 
        /// <list type="bullet">
        /// <item>length(A) - the longest dimension of A</item>
        /// <item>norm(A) being the largest singular value of A, </item>
        /// <item>MachineParameterDouble.eps - the distance between 1 and the smallest next greater value</item>
        /// </list>
        /// </remarks>
        public static ILRetArray< float> rank( ILInArray< float> A, double tolerance = -1.0 ) {
            using (ILScope.Enter(A)) {
                if (A.Size.NumberOfDimensions > 2)
                    throw new ILArgumentSizeException("The input array must be matrix or vector");
                ILArray< float> ret = svd(A);
                if (tolerance < 0) {
                    tolerance = A.Size.Longest * max(ret).GetValue(0) *  MachineParameterSingle.eps;
                }
                // count vector elements: ret is vector returned from svd
                return find(ret > ( float)tolerance).Length;
            }
        }


        /// <summary>
        /// Rank of matrix A
        /// </summary>
        /// <param name="A">Input matrix</param>
        /// <param name="tolerance">[Optional]Tolerance used to decide, if a singular value is 
        /// treated as zero. If a value &lt; 0 is specified the tolerance will be determined automatically (see below - default = -1.0)</param>
        /// <returns>Rank of matrix A</returns>
        /// <remarks>The rank is the number of singular values greater than 
        /// tolerance. If tolerance is smaller than zero, the following equation is used as 
        /// default: \\
        /// tol = length(A) * norm(A) * MachineParameterDouble.epsilon \\
        /// with 
        /// <list type="bullet">
        /// <item>length(A) - the longest dimension of A</item>
        /// <item>norm(A) being the largest singular value of A, </item>
        /// <item>MachineParameterDouble.eps - the distance between 1 and the smallest next greater value</item>
        /// </list>
        /// </remarks>
        public static ILRetArray< double> rank( ILInArray< complex> A, double tolerance = -1.0 ) {
            using (ILScope.Enter(A)) {
                if (A.Size.NumberOfDimensions > 2)
                    throw new ILArgumentSizeException("The input array must be matrix or vector");
                ILArray< double> ret = svd(A);
                if (tolerance < 0) {
                    tolerance = A.Size.Longest * max(ret).GetValue(0) *  MachineParameterDouble.eps;
                }
                // count vector elements: ret is vector returned from svd
                return find(ret > ( double)tolerance).Length;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}
