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



namespace ILNumerics {
    public partial class ILMath {


        /// <summary>
        /// Array replication 
        /// </summary>
        /// <param name="X">Input array to be replicated</param>
        /// <param name="size">Dimensions specifier, number of rows, columns .. to replicate this array</param>
        /// <returns>Reference ILArray as replication of X</returns>
        public static ILRetArray<double> repmat(ILInArray<double> X, params int[] size) {
            using (ILScope.Enter(X)) {
                return new ILRetArray<double>(X.Storage.Repmat(size));
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Array replication 
        /// </summary>
        /// <param name="X">Input array to be replicated</param>
        /// <param name="size">Dimensions specifier, number of rows, columns .. to replicate this array</param>
        /// <returns>Reference ILArray as replication of X</returns>
        public static ILRetArray<fcomplex> repmat(ILInArray<fcomplex> X, params int[] size) {
            using (ILScope.Enter(X)) {
                return new ILRetArray<fcomplex>(X.Storage.Repmat(size));
            }
        }
        /// <summary>
        /// Array replication 
        /// </summary>
        /// <param name="X">Input array to be replicated</param>
        /// <param name="size">Dimensions specifier, number of rows, columns .. to replicate this array</param>
        /// <returns>Reference ILArray as replication of X</returns>
        public static ILRetArray<complex> repmat(ILInArray<complex> X, params int[] size) {
            using (ILScope.Enter(X)) {
                return new ILRetArray<complex>(X.Storage.Repmat(size));
            }
        }
        /// <summary>
        /// Array replication 
        /// </summary>
        /// <param name="X">Input array to be replicated</param>
        /// <param name="size">Dimensions specifier, number of rows, columns .. to replicate this array</param>
        /// <returns>Reference ILArray as replication of X</returns>
        public static ILRetArray<byte> repmat(ILInArray<byte> X, params int[] size) {
            using (ILScope.Enter(X)) {
                return new ILRetArray<byte>(X.Storage.Repmat(size));
            }
        }
        /// <summary>
        /// Array replication 
        /// </summary>
        /// <param name="X">Input array to be replicated</param>
        /// <param name="size">Dimensions specifier, number of rows, columns .. to replicate this array</param>
        /// <returns>Reference ILArray as replication of X</returns>
        public static ILRetArray<Int64> repmat(ILInArray<Int64> X, params int[] size) {
            using (ILScope.Enter(X)) {
                return new ILRetArray<Int64>(X.Storage.Repmat(size));
            }
        }
        /// <summary>
        /// Array replication 
        /// </summary>
        /// <param name="X">Input array to be replicated</param>
        /// <param name="size">Dimensions specifier, number of rows, columns .. to replicate this array</param>
        /// <returns>Reference ILArray as replication of X</returns>
        public static ILRetArray<Int32> repmat(ILInArray<Int32> X, params int[] size) {
            using (ILScope.Enter(X)) {
                return new ILRetArray<Int32>(X.Storage.Repmat(size));
            }
        }
        /// <summary>
        /// Array replication 
        /// </summary>
        /// <param name="X">Input array to be replicated</param>
        /// <param name="size">Dimensions specifier, number of rows, columns .. to replicate this array</param>
        /// <returns>Reference ILArray as replication of X</returns>
        public static ILRetArray<float> repmat(ILInArray<float> X, params int[] size) {
            using (ILScope.Enter(X)) {
                return new ILRetArray<float>(X.Storage.Repmat(size));
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Array replication 
        /// </summary>
        /// <param name="X">Input array to be replicated</param>
        /// <param name="size">Dimensions specifier, number of rows, columns .. to replicate this array</param>
        /// <returns>Reference ILArray as replication of X</returns>
        public static ILRetArray<T> repmat<T>(ILInArray<T> X, params int[] size) {
            using (ILScope.Enter(X)) {
                return new ILRetArray<T>(X.Storage.Repmat(size));
            }
        }
    }
}
