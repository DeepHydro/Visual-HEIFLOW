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
using System.Runtime.InteropServices; 
using ILNumerics.Storage;
using ILNumerics.Misc;
using ILNumerics.Native;
using ILNumerics.Exceptions;


namespace ILNumerics {

    public partial class ILMath {


        /// <summary>
        /// Cross products of columns of two arrays 
        /// </summary>
        /// <param name="A">First array with vectors in columns</param>
        /// <param name="B">Second array with vectors in columns</param>
        /// <returns>Array with cross products of vectors from A and B</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arguments is not the same.</exception>
        public static ILRetArray<double> cross(ILInArray<double> A, 
                                                            ILInArray<double> B,
                                                            bool normalize = false) {
            using (ILScope.Enter(A, B)) {
                if (!A.Size.IsSameSize(B.Size))
                    throw new ILArgumentException("input arrays must have the same size!");
                ILArray<double> ret = array<double>(A.Size);
                using (ILScope.Enter()) {
                    ILArray<double> Ax = A[0, full];
                    ILArray<double> Ay = A[1, full];
                    ILArray<double> Az = A[2, full];
                    ILArray<double> Bx = B[0, full];
                    ILArray<double> By = B[1, full];
                    ILArray<double> Bz = B[2, full];
                    ret[0, full] = Ay * Bz - Az * By;
                    ret[1, full] = Az * Bx - Ax * Bz;
                    ret[2, full] = Ax * By - Ay * Bx;
                }
                if (!normalize)
                    return ret; 
                else 
                    return ret / sqrt(sum(ret * ret,0)); 
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Cross products of columns of two arrays 
        /// </summary>
        /// <param name="A">First array with vectors in columns</param>
        /// <param name="B">Second array with vectors in columns</param>
        /// <returns>Array with cross products of vectors from A and B</returns>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentException">If the size of both arguments is not the same.</exception>
        public static ILRetArray<float> cross(ILInArray<float> A, 
                                                            ILInArray<float> B,
                                                            bool normalize = false) {
            using (ILScope.Enter(A, B)) {
                if (!A.Size.IsSameSize(B.Size))
                    throw new ILArgumentException("input arrays must have the same size!");
                ILArray<float> ret = array<float>(A.Size);
                using (ILScope.Enter()) {
                    ILArray<float> Ax = A[0, full];
                    ILArray<float> Ay = A[1, full];
                    ILArray<float> Az = A[2, full];
                    ILArray<float> Bx = B[0, full];
                    ILArray<float> By = B[1, full];
                    ILArray<float> Bz = B[2, full];
                    ret[0, full] = Ay * Bz - Az * By;
                    ret[1, full] = Az * Bx - Ax * Bz;
                    ret[2, full] = Ax * By - Ay * Bx;
                }
                if (!normalize)
                    return ret; 
                else 
                    return ret / sqrt(sum(ret * ret,0)); 
            }
        }

#endregion HYCALPER AUTO GENERATED CODE

    }
}
