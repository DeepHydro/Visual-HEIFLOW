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
        /// Mean of array along specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Mean of elements along specified or first non singleton dimension</returns>
        /// <remarks>The return array has the same shape as A, but the specified or first non singleton dimension is reduced 
        /// to length 1. If A is scalar, an scalar array will be returned. On empty input, the output will be empty.</remarks>
        public static ILRetArray< double> mean( ILInArray< double> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim < 0)
                    dim = 0;
                return sum(A, dim) / ( double)A.Size[dim];
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Mean of array along specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Mean of elements along specified or first non singleton dimension</returns>
        /// <remarks>The return array has the same shape as A, but the specified or first non singleton dimension is reduced 
        /// to length 1. If A is scalar, an scalar array will be returned. On empty input, the output will be empty.</remarks>
        public static ILRetArray< Int64> mean( ILInArray< Int64> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim < 0)
                    dim = 0;
                return sum(A, dim) / ( Int64)A.Size[dim];
            }
        }
        /// <summary>
        /// Mean of array along specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Mean of elements along specified or first non singleton dimension</returns>
        /// <remarks>The return array has the same shape as A, but the specified or first non singleton dimension is reduced 
        /// to length 1. If A is scalar, an scalar array will be returned. On empty input, the output will be empty.</remarks>
        public static ILRetArray< Int32> mean( ILInArray< Int32> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim < 0)
                    dim = 0;
                return sum(A, dim) / ( Int32)A.Size[dim];
            }
        }
        /// <summary>
        /// Mean of array along specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Mean of elements along specified or first non singleton dimension</returns>
        /// <remarks>The return array has the same shape as A, but the specified or first non singleton dimension is reduced 
        /// to length 1. If A is scalar, an scalar array will be returned. On empty input, the output will be empty.</remarks>
        public static ILRetArray< byte> mean( ILInArray< byte> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim < 0)
                    dim = 0;
                return sum(A, dim) / ( byte)A.Size[dim];
            }
        }
        /// <summary>
        /// Mean of array along specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Mean of elements along specified or first non singleton dimension</returns>
        /// <remarks>The return array has the same shape as A, but the specified or first non singleton dimension is reduced 
        /// to length 1. If A is scalar, an scalar array will be returned. On empty input, the output will be empty.</remarks>
        public static ILRetArray< fcomplex> mean( ILInArray< fcomplex> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim < 0)
                    dim = 0;
                return sum(A, dim) / ( fcomplex)A.Size[dim];
            }
        }
        /// <summary>
        /// Mean of array along specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Mean of elements along specified or first non singleton dimension</returns>
        /// <remarks>The return array has the same shape as A, but the specified or first non singleton dimension is reduced 
        /// to length 1. If A is scalar, an scalar array will be returned. On empty input, the output will be empty.</remarks>
        public static ILRetArray< float> mean( ILInArray< float> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim < 0)
                    dim = 0;
                return sum(A, dim) / ( float)A.Size[dim];
            }
        }
        /// <summary>
        /// Mean of array along specified dimension
        /// </summary>
        /// <param name="A">Input Array</param>
        /// <param name="dim">[Optional] Index of the dimension to operate along. If omitted operates along the first non singleton dimension (i.e. != 1).</param>
        /// <returns>Mean of elements along specified or first non singleton dimension</returns>
        /// <remarks>The return array has the same shape as A, but the specified or first non singleton dimension is reduced 
        /// to length 1. If A is scalar, an scalar array will be returned. On empty input, the output will be empty.</remarks>
        public static ILRetArray< complex> mean( ILInArray< complex> A, int dim = -1) {
            using (ILScope.Enter(A)) {
                if (dim < 0)
                    dim = A.Size.WorkingDimension();
                if (dim < 0)
                    dim = 0;
                return sum(A, dim) / ( complex)A.Size[dim];
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}
