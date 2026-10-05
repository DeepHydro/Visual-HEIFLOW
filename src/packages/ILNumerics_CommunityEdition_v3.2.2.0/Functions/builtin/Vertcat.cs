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



namespace ILNumerics {
    public partial class ILMath {


        /// <summary>
        /// Vertical array concatenation 
        /// </summary>
        /// <param name="arrays">Arrays to be concatenated with each other. All
        /// arrays must be of the same inner type. The dimensions of all arrays 
        /// must match - except for the first dimension (index 0).</param>
        /// <returns>Larger array having all arrays in 'arrays' placed beneath each other (along dimension 0).
        /// </returns>
        public static ILRetArray<double> vertcat(params ILInArray<double>[] arrays) {
            return vertcat<double>(arrays); 
        }


#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Vertical array concatenation 
        /// </summary>
        /// <param name="arrays">Arrays to be concatenated with each other. All
        /// arrays must be of the same inner type. The dimensions of all arrays 
        /// must match - except for the first dimension (index 0).</param>
        /// <returns>Larger array having all arrays in 'arrays' placed beneath each other (along dimension 0).
        /// </returns>
        public static ILRetArray<fcomplex> vertcat(params ILInArray<fcomplex>[] arrays) {
            return vertcat<fcomplex>(arrays); 
        }

        /// <summary>
        /// Vertical array concatenation 
        /// </summary>
        /// <param name="arrays">Arrays to be concatenated with each other. All
        /// arrays must be of the same inner type. The dimensions of all arrays 
        /// must match - except for the first dimension (index 0).</param>
        /// <returns>Larger array having all arrays in 'arrays' placed beneath each other (along dimension 0).
        /// </returns>
        public static ILRetArray<complex> vertcat(params ILInArray<complex>[] arrays) {
            return vertcat<complex>(arrays); 
        }

        /// <summary>
        /// Vertical array concatenation 
        /// </summary>
        /// <param name="arrays">Arrays to be concatenated with each other. All
        /// arrays must be of the same inner type. The dimensions of all arrays 
        /// must match - except for the first dimension (index 0).</param>
        /// <returns>Larger array having all arrays in 'arrays' placed beneath each other (along dimension 0).
        /// </returns>
        public static ILRetArray<byte> vertcat(params ILInArray<byte>[] arrays) {
            return vertcat<byte>(arrays); 
        }

        /// <summary>
        /// Vertical array concatenation 
        /// </summary>
        /// <param name="arrays">Arrays to be concatenated with each other. All
        /// arrays must be of the same inner type. The dimensions of all arrays 
        /// must match - except for the first dimension (index 0).</param>
        /// <returns>Larger array having all arrays in 'arrays' placed beneath each other (along dimension 0).
        /// </returns>
        public static ILRetArray<Int64> vertcat(params ILInArray<Int64>[] arrays) {
            return vertcat<Int64>(arrays); 
        }

        /// <summary>
        /// Vertical array concatenation 
        /// </summary>
        /// <param name="arrays">Arrays to be concatenated with each other. All
        /// arrays must be of the same inner type. The dimensions of all arrays 
        /// must match - except for the first dimension (index 0).</param>
        /// <returns>Larger array having all arrays in 'arrays' placed beneath each other (along dimension 0).
        /// </returns>
        public static ILRetArray<Int32> vertcat(params ILInArray<Int32>[] arrays) {
            return vertcat<Int32>(arrays); 
        }

        /// <summary>
        /// Vertical array concatenation 
        /// </summary>
        /// <param name="arrays">Arrays to be concatenated with each other. All
        /// arrays must be of the same inner type. The dimensions of all arrays 
        /// must match - except for the first dimension (index 0).</param>
        /// <returns>Larger array having all arrays in 'arrays' placed beneath each other (along dimension 0).
        /// </returns>
        public static ILRetArray<float> vertcat(params ILInArray<float>[] arrays) {
            return vertcat<float>(arrays); 
        }


#endregion HYCALPER AUTO GENERATED CODE
       /// <summary>
        /// Vertical array concatenation 
        /// </summary>
        /// <param name="arrays">Arrays to be concatenated with each other. All
        /// arrays must be of the same inner type. The dimensions of all arrays 
        /// must match - except for the first dimension (index 0).</param>
        /// <returns>Larger array having all arrays in 'arrays' placed beneath each other.
        /// </returns>
        public static ILRetArray<T> vertcat<T>(params ILInArray<T>[] arrays) {
            if (arrays == null) 
                throw new ILArgumentException("input argument must not be null!"); 
            if (arrays.Length == 0 || object.Equals(arrays[0],null)) 
                return empty<T>(ILSize.Empty00);
            using (ILScope.Enter(arrays)) {
                if (arrays.Length == 1)
                    return arrays[0].C;
                ILDenseStorage<T> ret = arrays[0].Storage.Concat(arrays[1].Storage, 0);
                for (int i = 2; i < arrays.Length; i++) {
                    ILDenseStorage<T> tmp = ret.Concat(arrays[i].Storage, 0);
                    ret.Dispose();
                    ret = tmp;
                }
                return new ILRetArray<T>(ret);
            }
        }
    }
}
