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
        /// Diagonal matrix or diagonal of matrix
        /// </summary>
        /// <param name="X">Input matrix or vector. If X is matrix, diag(X) returns the 
        /// elements on the diagPosition's diagonal as column vector. If X is vector, a square matrix of size 
        /// [length(X) + abs(diagPosition), length(X) + abs(diagPosition)] will be created, having 
        /// the elements of X on the diagPosition's diagonal.</param>
        /// <param name="diagPosition">[Optional] Index of diagonal to extract/create. Here 0 means the 
        /// main diagonal, diagPosition > 0 is above the main diagonal, diagPosition smaller 0 means 
        /// below the main diagonal.</param>
        /// <returns>Depending on X a matrix or a vector with the elements on a diagonal.</returns>
        /// <remarks>The type of the ILArray returned will be the same as the type of X.</remarks>
        public static ILRetArray<double> diag(ILInArray<double> X, int diagPosition = 0) {
            return diag<double>(X, diagPosition); 
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Diagonal matrix or diagonal of matrix
        /// </summary>
        /// <param name="X">Input matrix or vector. If X is matrix, diag(X) returns the 
        /// elements on the diagPosition's diagonal as column vector. If X is vector, a square matrix of size 
        /// [length(X) + abs(diagPosition), length(X) + abs(diagPosition)] will be created, having 
        /// the elements of X on the diagPosition's diagonal.</param>
        /// <param name="diagPosition">[Optional] Index of diagonal to extract/create. Here 0 means the 
        /// main diagonal, diagPosition > 0 is above the main diagonal, diagPosition smaller 0 means 
        /// below the main diagonal.</param>
        /// <returns>Depending on X a matrix or a vector with the elements on a diagonal.</returns>
        /// <remarks>The type of the ILArray returned will be the same as the type of X.</remarks>
        public static ILRetArray<fcomplex> diag(ILInArray<fcomplex> X, int diagPosition = 0) {
            return diag<fcomplex>(X, diagPosition); 
        }
        /// <summary>
        /// Diagonal matrix or diagonal of matrix
        /// </summary>
        /// <param name="X">Input matrix or vector. If X is matrix, diag(X) returns the 
        /// elements on the diagPosition's diagonal as column vector. If X is vector, a square matrix of size 
        /// [length(X) + abs(diagPosition), length(X) + abs(diagPosition)] will be created, having 
        /// the elements of X on the diagPosition's diagonal.</param>
        /// <param name="diagPosition">[Optional] Index of diagonal to extract/create. Here 0 means the 
        /// main diagonal, diagPosition > 0 is above the main diagonal, diagPosition smaller 0 means 
        /// below the main diagonal.</param>
        /// <returns>Depending on X a matrix or a vector with the elements on a diagonal.</returns>
        /// <remarks>The type of the ILArray returned will be the same as the type of X.</remarks>
        public static ILRetArray<complex> diag(ILInArray<complex> X, int diagPosition = 0) {
            return diag<complex>(X, diagPosition); 
        }
        /// <summary>
        /// Diagonal matrix or diagonal of matrix
        /// </summary>
        /// <param name="X">Input matrix or vector. If X is matrix, diag(X) returns the 
        /// elements on the diagPosition's diagonal as column vector. If X is vector, a square matrix of size 
        /// [length(X) + abs(diagPosition), length(X) + abs(diagPosition)] will be created, having 
        /// the elements of X on the diagPosition's diagonal.</param>
        /// <param name="diagPosition">[Optional] Index of diagonal to extract/create. Here 0 means the 
        /// main diagonal, diagPosition > 0 is above the main diagonal, diagPosition smaller 0 means 
        /// below the main diagonal.</param>
        /// <returns>Depending on X a matrix or a vector with the elements on a diagonal.</returns>
        /// <remarks>The type of the ILArray returned will be the same as the type of X.</remarks>
        public static ILRetArray<byte> diag(ILInArray<byte> X, int diagPosition = 0) {
            return diag<byte>(X, diagPosition); 
        }
        /// <summary>
        /// Diagonal matrix or diagonal of matrix
        /// </summary>
        /// <param name="X">Input matrix or vector. If X is matrix, diag(X) returns the 
        /// elements on the diagPosition's diagonal as column vector. If X is vector, a square matrix of size 
        /// [length(X) + abs(diagPosition), length(X) + abs(diagPosition)] will be created, having 
        /// the elements of X on the diagPosition's diagonal.</param>
        /// <param name="diagPosition">[Optional] Index of diagonal to extract/create. Here 0 means the 
        /// main diagonal, diagPosition > 0 is above the main diagonal, diagPosition smaller 0 means 
        /// below the main diagonal.</param>
        /// <returns>Depending on X a matrix or a vector with the elements on a diagonal.</returns>
        /// <remarks>The type of the ILArray returned will be the same as the type of X.</remarks>
        public static ILRetArray<Int64> diag(ILInArray<Int64> X, int diagPosition = 0) {
            return diag<Int64>(X, diagPosition); 
        }
        /// <summary>
        /// Diagonal matrix or diagonal of matrix
        /// </summary>
        /// <param name="X">Input matrix or vector. If X is matrix, diag(X) returns the 
        /// elements on the diagPosition's diagonal as column vector. If X is vector, a square matrix of size 
        /// [length(X) + abs(diagPosition), length(X) + abs(diagPosition)] will be created, having 
        /// the elements of X on the diagPosition's diagonal.</param>
        /// <param name="diagPosition">[Optional] Index of diagonal to extract/create. Here 0 means the 
        /// main diagonal, diagPosition > 0 is above the main diagonal, diagPosition smaller 0 means 
        /// below the main diagonal.</param>
        /// <returns>Depending on X a matrix or a vector with the elements on a diagonal.</returns>
        /// <remarks>The type of the ILArray returned will be the same as the type of X.</remarks>
        public static ILRetArray<Int32> diag(ILInArray<Int32> X, int diagPosition = 0) {
            return diag<Int32>(X, diagPosition); 
        }
        /// <summary>
        /// Diagonal matrix or diagonal of matrix
        /// </summary>
        /// <param name="X">Input matrix or vector. If X is matrix, diag(X) returns the 
        /// elements on the diagPosition's diagonal as column vector. If X is vector, a square matrix of size 
        /// [length(X) + abs(diagPosition), length(X) + abs(diagPosition)] will be created, having 
        /// the elements of X on the diagPosition's diagonal.</param>
        /// <param name="diagPosition">[Optional] Index of diagonal to extract/create. Here 0 means the 
        /// main diagonal, diagPosition > 0 is above the main diagonal, diagPosition smaller 0 means 
        /// below the main diagonal.</param>
        /// <returns>Depending on X a matrix or a vector with the elements on a diagonal.</returns>
        /// <remarks>The type of the ILArray returned will be the same as the type of X.</remarks>
        public static ILRetArray<float> diag(ILInArray<float> X, int diagPosition = 0) {
            return diag<float>(X, diagPosition); 
        }

#endregion HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Diagonal matrix or diagonal of matrix
        /// </summary>
        /// <param name="X">Input matrix or vector. If X is matrix, diag(X) returns the 
        /// elements on the diagPosition's diagonal as column vector. If X is vector, a square matrix of size 
        /// [length(X) + abs(diagPosition), length(X) + abs(diagPosition)] will be created, having 
        /// the elements of X on the diagPosition's diagonal.</param>
        /// <param name="diagPosition">[Optional] Index of diagonal to extract/create. Here 0 means the 
        /// main diagonal, diagPosition > 0 is above the main diagonal, diagPosition smaller 0 means 
        /// below the main diagonal.</param>
        /// <returns>Depending on 'X' a matrix or a vector with the elements on a diagonal.</returns>
        /// <remarks>The type of the ILArray returned will be the same as the type of X.</remarks>
        public static ILRetArray<T> diag<T>(ILInArray<T> X, int diagPosition = 0) {
            using (ILScope.Enter(X)) {
                if (Object.Equals(X, null) || X.Size.NumberOfDimensions > 2) {
                    throw new ILArgumentException("diag: input array must be matrix, vector or scalar"); 
                }
                if (X.IsEmpty) 
                    return empty<T>(X.Size); 
                ILArray<T> ret = empty<T>(ILSize.Empty00);
                if  (X.IsScalar) {
                    ret.a = X.GetValue(0,0);
                } else if (X.IsVector) {
                    int retLen = X.Length + Math.Abs(diagPosition);
                    ret.a = zeros<T> (new ILSize(retLen, retLen));
                    T [] dIn = X.GetArrayForRead();
                    if (diagPosition > 0) {
                        for (int i = 0; i < X.S.NumberOfElements; i++) {
                            ret.SetValue(dIn[i], i, i + diagPosition);
                        }
                    } else {
                        for (int i = 0; i < X.S.NumberOfElements; i++) {
                            ret.SetValue(dIn[i],i - diagPosition, i);
                        }
                    }
                } else  { // X is matrix
                    int retLen = Math.Min(X.Size[0], X.Size[1]) - Math.Abs(diagPosition);
                    ret = zeros<T> (new ILSize(retLen, 1));
                    T [] dRet = ret.GetArrayForWrite();
                    if (diagPosition > 0) {
                        if (diagPosition >= X.Size[1])
                            return empty<T>(ILSize.Empty00);
                        for (int i = 0; i < retLen; i++) {
                            dRet[i] = X.GetValue(i, diagPosition + i);
                        }
                    } else {
                        if (diagPosition <= -X.Size[0])
                            return empty<T>(ILSize.Empty00);
                        for (int i = 0; i < retLen; i++) {
                            dRet[i] = X.GetValue(diagPosition + i, i);
                        }
                    }
                }
                return ret;
            }
        }
    }
}
