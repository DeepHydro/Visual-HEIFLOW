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
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="dim">New dimension</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by dim. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements for the 
        /// new dimensions specified by <paramref name="dim"/> 
        /// do not match.</exception>
        public static ILRetArray<double> reshape(ILInArray<double> A, ILSize dim) {
            using (ILScope.Enter(A))
                return A.Reshape(dim);
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="newDimensions">New dimensions. This may be 
        /// a comma seperated list or an int array</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by <paramref name="newDimensions"/>. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements specified 
        /// by <paramref name="newDimensions"/> do not match.</exception>
        public static ILRetArray<double> reshape(ILInArray<double> A, params int[] newDimensions) {
            using (ILScope.Enter(A))
                return A.Reshape(new ILSize(newDimensions)); 
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="dim">New dimension</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by dim. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements for the 
        /// new dimensions specified by <paramref name="dim"/> 
        /// do not match.</exception>
        public static ILRetArray<fcomplex> reshape(ILInArray<fcomplex> A, ILSize dim) {
            using (ILScope.Enter(A))
                return A.Reshape(dim);
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="newDimensions">New dimensions. This may be 
        /// a comma seperated list or an int array</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by <paramref name="newDimensions"/>. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements specified 
        /// by <paramref name="newDimensions"/> do not match.</exception>
        public static ILRetArray<fcomplex> reshape(ILInArray<fcomplex> A, params int[] newDimensions) {
            using (ILScope.Enter(A))
                return A.Reshape(new ILSize(newDimensions)); 
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="dim">New dimension</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by dim. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements for the 
        /// new dimensions specified by <paramref name="dim"/> 
        /// do not match.</exception>
        public static ILRetArray<complex> reshape(ILInArray<complex> A, ILSize dim) {
            using (ILScope.Enter(A))
                return A.Reshape(dim);
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="newDimensions">New dimensions. This may be 
        /// a comma seperated list or an int array</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by <paramref name="newDimensions"/>. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements specified 
        /// by <paramref name="newDimensions"/> do not match.</exception>
        public static ILRetArray<complex> reshape(ILInArray<complex> A, params int[] newDimensions) {
            using (ILScope.Enter(A))
                return A.Reshape(new ILSize(newDimensions)); 
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="dim">New dimension</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by dim. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements for the 
        /// new dimensions specified by <paramref name="dim"/> 
        /// do not match.</exception>
        public static ILRetArray<byte> reshape(ILInArray<byte> A, ILSize dim) {
            using (ILScope.Enter(A))
                return A.Reshape(dim);
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="newDimensions">New dimensions. This may be 
        /// a comma seperated list or an int array</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by <paramref name="newDimensions"/>. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements specified 
        /// by <paramref name="newDimensions"/> do not match.</exception>
        public static ILRetArray<byte> reshape(ILInArray<byte> A, params int[] newDimensions) {
            using (ILScope.Enter(A))
                return A.Reshape(new ILSize(newDimensions)); 
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="dim">New dimension</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by dim. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements for the 
        /// new dimensions specified by <paramref name="dim"/> 
        /// do not match.</exception>
        public static ILRetArray<Int64> reshape(ILInArray<Int64> A, ILSize dim) {
            using (ILScope.Enter(A))
                return A.Reshape(dim);
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="newDimensions">New dimensions. This may be 
        /// a comma seperated list or an int array</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by <paramref name="newDimensions"/>. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements specified 
        /// by <paramref name="newDimensions"/> do not match.</exception>
        public static ILRetArray<Int64> reshape(ILInArray<Int64> A, params int[] newDimensions) {
            using (ILScope.Enter(A))
                return A.Reshape(new ILSize(newDimensions)); 
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="dim">New dimension</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by dim. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements for the 
        /// new dimensions specified by <paramref name="dim"/> 
        /// do not match.</exception>
        public static ILRetArray<Int32> reshape(ILInArray<Int32> A, ILSize dim) {
            using (ILScope.Enter(A))
                return A.Reshape(dim);
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="newDimensions">New dimensions. This may be 
        /// a comma seperated list or an int array</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by <paramref name="newDimensions"/>. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements specified 
        /// by <paramref name="newDimensions"/> do not match.</exception>
        public static ILRetArray<Int32> reshape(ILInArray<Int32> A, params int[] newDimensions) {
            using (ILScope.Enter(A))
                return A.Reshape(new ILSize(newDimensions)); 
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="dim">New dimension</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by dim. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements for the 
        /// new dimensions specified by <paramref name="dim"/> 
        /// do not match.</exception>
        public static ILRetArray<float> reshape(ILInArray<float> A, ILSize dim) {
            using (ILScope.Enter(A))
                return A.Reshape(dim);
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="newDimensions">New dimensions. This may be 
        /// a comma seperated list or an int array</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by <paramref name="newDimensions"/>. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements specified 
        /// by <paramref name="newDimensions"/> do not match.</exception>
        public static ILRetArray<float> reshape(ILInArray<float> A, params int[] newDimensions) {
            using (ILScope.Enter(A))
                return A.Reshape(new ILSize(newDimensions)); 
        }

#endregion HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="newDimensions">New dimensions array. This may be 
        /// a comma seperated list or an int array</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new array is created, having 
        /// the size and number of dimensions specified by <paramref name="newDimensions"/>. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements specified 
        /// by <paramref name="newDimensions"/> 
        /// do not match.</exception>
        public static ILRetArray<T> reshape<T>(ILInArray<T> A, params int[] newDimensions) {
            return A.Reshape(new ILSize(newDimensions));
        }
        /// <summary>
        /// Array reshaping
        /// </summary>
        /// <param name="A">Input array A</param>
        /// <param name="dim">New dimension</param>
        /// <returns>Reshaped array</returns>
        /// <remarks>A will not be changed. A new reference array is created, having 
        /// the size and number of dimensions specified by dim. </remarks>
        /// <exception cref="ILNumerics.Exceptions.ILArgumentSizeException"> 
        /// If the number of elements in A and the number of elements for the 
        /// new dimensions specified by <paramref name="dim"/> 
        /// do not match.</exception>
        public static ILRetArray<T> reshape<T>(ILInArray<T> A, ILSize dim) {
            using (ILScope.Enter(A))
                return A.Reshape(dim); 
        }
    }
}
