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
        /// Standard deviation for values in A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="Weights">Vector of scaling factors, same length as working dimension of A, default: no scaling</param>
        /// <param name="biased">[Optional] true: apply biased normalization to result, default: false (non-biased)</param>
        /// <param name="dim">[Optional] Dimension index of A to operate along, default: first non singleton dimension</param>
        /// <returns>Variances</returns>
        /// <remarks><para>On scalar A a scalar 0 of the same shape as A is returned.</para>
        /// <para>On empty A an empty array is returned, having the dimension to operate along reduced to length 1.</para>
        /// <para>The parameters <paramref name="Weights"/>, <paramref name="biased"/> and <paramref name="dim"/> are optional. 
        /// Ommiting either one will choose its respective default value.</para>
        /// <para>The standard deviation is computed by the formula 
        /// <c>std = sqrt(var(A,...))</c>. For further details on given parameters, see 
        /// <see cref="M:ILNumerics.ILMath.var(ILNumerics.ILInArray{T},ILNumerics.ILInArray{T},bool,false);"/>.</para>
        /// </remarks>
        public static ILRetArray<double> std(ILInArray<double> A, 
                                                            ILInArray<double> Weights = null, 
                                                            bool biased = false, int dim = -1) {
            return sqrt(var(A,Weights,biased,dim)); 
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Standard deviation for values in A
        /// </summary>
        /// <param name="A">Input array</param>
        /// <param name="Weights">Vector of scaling factors, same length as working dimension of A, default: no scaling</param>
        /// <param name="biased">[Optional] true: apply biased normalization to result, default: false (non-biased)</param>
        /// <param name="dim">[Optional] Dimension index of A to operate along, default: first non singleton dimension</param>
        /// <returns>Variances</returns>
        /// <remarks><para>On scalar A a scalar 0 of the same shape as A is returned.</para>
        /// <para>On empty A an empty array is returned, having the dimension to operate along reduced to length 1.</para>
        /// <para>The parameters <paramref name="Weights"/>, <paramref name="biased"/> and <paramref name="dim"/> are optional. 
        /// Ommiting either one will choose its respective default value.</para>
        /// <para>The standard deviation is computed by the formula 
        /// <c>std = sqrt(var(A,...))</c>. For further details on given parameters, see 
        /// <see cref="M:ILNumerics.ILMath.var(ILNumerics.ILInArray{T},ILNumerics.ILInArray{T},bool,false);"/>.</para>
        /// </remarks>
        public static ILRetArray<float> std(ILInArray<float> A, 
                                                            ILInArray<float> Weights = null, 
                                                            bool biased = false, int dim = -1) {
            return sqrt(var(A,Weights,biased,dim)); 
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}