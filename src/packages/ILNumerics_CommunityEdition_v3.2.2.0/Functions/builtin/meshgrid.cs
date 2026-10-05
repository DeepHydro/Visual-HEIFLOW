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
using ILNumerics.Misc;
using ILNumerics.Exceptions; 


namespace ILNumerics {

    public partial class ILMath {

        /// <summary>
        /// Create two matrices for evaluation and visualization of 2 dimensional functions
        /// </summary>
        /// <param name="X">Vector of x values</param>
        /// <param name="Y">Vector of y values</param>
        /// <param name="outY">[Output, Optional]: if on entry outY is not null, on return it will hold the values for the Y dimension</param>
        /// <returns>X values matrix along the values of the X input array. Corresponding values for Y are returned in outY</returns>
        /// <remarks>The matrices returned can be utilize to evaluate and visualize functions of 2 variables X and Y.</remarks>
        public static ILRetArray< double > meshgrid(ILInArray< double > X, ILInArray< double > Y
                                                                  , ILOutArray< double > outY = null)
        {
            using (ILScope.Enter(X,Y)) {
                if (!X.IsVector || !Y.IsVector)
                {
                    throw new ILArgumentException("inputs must be vectors");
                }
                if (!X.IsRowVector) X = X.T; 
                if (!Y.IsColumnVector) Y = Y.T; 
                ILArray< double > ret = repmat(X,Y.Length,1);
                if (!isnull(outY)) 
                    outY.a = repmat(Y,1,X.Length);
                return ret;
            }
        }

        /// <summary>
        /// Create 3d arrays for evaluation and visualization of 3 dimensional functions
        /// </summary>
        /// <param name="X">Vector of x values</param>
        /// <param name="Y">Vector of y values</param>
        /// <param name="Z">Vector of z values</param>
        /// <param name="outY">[Output, Optional] If on entry outY is not null, on return it will hold the values for the y dimension</param>
        /// <param name="outZ">[Output, Optional] If on entry outZ is not null, on return it will hold the values for the z dimension</param>
        /// <returns>X value array along the values of the X input vector, arrays for y and z dimensions are returned in outY and outZ respectively</returns>
        /// <remarks>The arrays returned can be used to evaluate and visualize functions of 3 variables X, Y and Z.</remarks>
        public static ILRetArray< double > meshgrid(ILInArray< double > X, ILInArray< double > Y, ILInArray< double > Z
                                                                ,ILOutArray< double > outY = null, ILOutArray< double > outZ = null)
        {
            using (ILScope.Enter()) {
                if (!X.IsVector || !Y.IsVector || !Z.IsVector)
                {
                    throw new ILArgumentException("inputs must be vectors");
                }
                if (!X.IsRowVector) X = X.T; 
                if (!Y.IsColumnVector) Y = Y.T;  
                Z = Z[full].Reshape(new ILSize(1,1,Z.Size.NumberOfElements)); 

                ILArray< double > ret = repmat(X,Y.Length,1,Z.Length);
                if (!isnull(outY)) outY.a = repmat(Y,1,X.Length,Z.Length);
                if (!isnull(outZ)) outZ.a = repmat(Z, Y.Length, X.Length, 1);
                return ret;
            }
        }

#region HYCALPER AUTO GENERATED CODE

        /// <summary>
        /// Create two matrices for evaluation and visualization of 2 dimensional functions
        /// </summary>
        /// <param name="X">Vector of x values</param>
        /// <param name="Y">Vector of y values</param>
        /// <param name="outY">[Output, Optional]: if on entry outY is not null, on return it will hold the values for the Y dimension</param>
        /// <returns>X values matrix along the values of the X input array. Corresponding values for Y are returned in outY</returns>
        /// <remarks>The matrices returned can be utilize to evaluate and visualize functions of 2 variables X and Y.</remarks>
        public static ILRetArray< Int64 > meshgrid(ILInArray< Int64 > X, ILInArray< Int64 > Y
                                                                  , ILOutArray< Int64 > outY = null)
        {
            using (ILScope.Enter(X,Y)) {
                if (!X.IsVector || !Y.IsVector)
                {
                    throw new ILArgumentException("inputs must be vectors");
                }
                if (!X.IsRowVector) X = X.T; 
                if (!Y.IsColumnVector) Y = Y.T; 
                ILArray< Int64 > ret = repmat(X,Y.Length,1);
                if (!isnull(outY)) 
                    outY.a = repmat(Y,1,X.Length);
                return ret;
            }
        }

        /// <summary>
        /// Create 3d arrays for evaluation and visualization of 3 dimensional functions
        /// </summary>
        /// <param name="X">Vector of x values</param>
        /// <param name="Y">Vector of y values</param>
        /// <param name="Z">Vector of z values</param>
        /// <param name="outY">[Output, Optional] If on entry outY is not null, on return it will hold the values for the y dimension</param>
        /// <param name="outZ">[Output, Optional] If on entry outZ is not null, on return it will hold the values for the z dimension</param>
        /// <returns>X value array along the values of the X input vector, arrays for y and z dimensions are returned in outY and outZ respectively</returns>
        /// <remarks>The arrays returned can be used to evaluate and visualize functions of 3 variables X, Y and Z.</remarks>
        public static ILRetArray< Int64 > meshgrid(ILInArray< Int64 > X, ILInArray< Int64 > Y, ILInArray< Int64 > Z
                                                                ,ILOutArray< Int64 > outY = null, ILOutArray< Int64 > outZ = null)
        {
            using (ILScope.Enter()) {
                if (!X.IsVector || !Y.IsVector || !Z.IsVector)
                {
                    throw new ILArgumentException("inputs must be vectors");
                }
                if (!X.IsRowVector) X = X.T; 
                if (!Y.IsColumnVector) Y = Y.T;  
                Z = Z[full].Reshape(new ILSize(1,1,Z.Size.NumberOfElements)); 

                ILArray< Int64 > ret = repmat(X,Y.Length,1,Z.Length);
                if (!isnull(outY)) outY.a = repmat(Y,1,X.Length,Z.Length);
                if (!isnull(outZ)) outZ.a = repmat(Z, Y.Length, X.Length, 1);
                return ret;
            }
        }
        /// <summary>
        /// Create two matrices for evaluation and visualization of 2 dimensional functions
        /// </summary>
        /// <param name="X">Vector of x values</param>
        /// <param name="Y">Vector of y values</param>
        /// <param name="outY">[Output, Optional]: if on entry outY is not null, on return it will hold the values for the Y dimension</param>
        /// <returns>X values matrix along the values of the X input array. Corresponding values for Y are returned in outY</returns>
        /// <remarks>The matrices returned can be utilize to evaluate and visualize functions of 2 variables X and Y.</remarks>
        public static ILRetArray< Int32 > meshgrid(ILInArray< Int32 > X, ILInArray< Int32 > Y
                                                                  , ILOutArray< Int32 > outY = null)
        {
            using (ILScope.Enter(X,Y)) {
                if (!X.IsVector || !Y.IsVector)
                {
                    throw new ILArgumentException("inputs must be vectors");
                }
                if (!X.IsRowVector) X = X.T; 
                if (!Y.IsColumnVector) Y = Y.T; 
                ILArray< Int32 > ret = repmat(X,Y.Length,1);
                if (!isnull(outY)) 
                    outY.a = repmat(Y,1,X.Length);
                return ret;
            }
        }

        /// <summary>
        /// Create 3d arrays for evaluation and visualization of 3 dimensional functions
        /// </summary>
        /// <param name="X">Vector of x values</param>
        /// <param name="Y">Vector of y values</param>
        /// <param name="Z">Vector of z values</param>
        /// <param name="outY">[Output, Optional] If on entry outY is not null, on return it will hold the values for the y dimension</param>
        /// <param name="outZ">[Output, Optional] If on entry outZ is not null, on return it will hold the values for the z dimension</param>
        /// <returns>X value array along the values of the X input vector, arrays for y and z dimensions are returned in outY and outZ respectively</returns>
        /// <remarks>The arrays returned can be used to evaluate and visualize functions of 3 variables X, Y and Z.</remarks>
        public static ILRetArray< Int32 > meshgrid(ILInArray< Int32 > X, ILInArray< Int32 > Y, ILInArray< Int32 > Z
                                                                ,ILOutArray< Int32 > outY = null, ILOutArray< Int32 > outZ = null)
        {
            using (ILScope.Enter()) {
                if (!X.IsVector || !Y.IsVector || !Z.IsVector)
                {
                    throw new ILArgumentException("inputs must be vectors");
                }
                if (!X.IsRowVector) X = X.T; 
                if (!Y.IsColumnVector) Y = Y.T;  
                Z = Z[full].Reshape(new ILSize(1,1,Z.Size.NumberOfElements)); 

                ILArray< Int32 > ret = repmat(X,Y.Length,1,Z.Length);
                if (!isnull(outY)) outY.a = repmat(Y,1,X.Length,Z.Length);
                if (!isnull(outZ)) outZ.a = repmat(Z, Y.Length, X.Length, 1);
                return ret;
            }
        }
        /// <summary>
        /// Create two matrices for evaluation and visualization of 2 dimensional functions
        /// </summary>
        /// <param name="X">Vector of x values</param>
        /// <param name="Y">Vector of y values</param>
        /// <param name="outY">[Output, Optional]: if on entry outY is not null, on return it will hold the values for the Y dimension</param>
        /// <returns>X values matrix along the values of the X input array. Corresponding values for Y are returned in outY</returns>
        /// <remarks>The matrices returned can be utilize to evaluate and visualize functions of 2 variables X and Y.</remarks>
        public static ILRetArray< float > meshgrid(ILInArray< float > X, ILInArray< float > Y
                                                                  , ILOutArray< float > outY = null)
        {
            using (ILScope.Enter(X,Y)) {
                if (!X.IsVector || !Y.IsVector)
                {
                    throw new ILArgumentException("inputs must be vectors");
                }
                if (!X.IsRowVector) X = X.T; 
                if (!Y.IsColumnVector) Y = Y.T; 
                ILArray< float > ret = repmat(X,Y.Length,1);
                if (!isnull(outY)) 
                    outY.a = repmat(Y,1,X.Length);
                return ret;
            }
        }

        /// <summary>
        /// Create 3d arrays for evaluation and visualization of 3 dimensional functions
        /// </summary>
        /// <param name="X">Vector of x values</param>
        /// <param name="Y">Vector of y values</param>
        /// <param name="Z">Vector of z values</param>
        /// <param name="outY">[Output, Optional] If on entry outY is not null, on return it will hold the values for the y dimension</param>
        /// <param name="outZ">[Output, Optional] If on entry outZ is not null, on return it will hold the values for the z dimension</param>
        /// <returns>X value array along the values of the X input vector, arrays for y and z dimensions are returned in outY and outZ respectively</returns>
        /// <remarks>The arrays returned can be used to evaluate and visualize functions of 3 variables X, Y and Z.</remarks>
        public static ILRetArray< float > meshgrid(ILInArray< float > X, ILInArray< float > Y, ILInArray< float > Z
                                                                ,ILOutArray< float > outY = null, ILOutArray< float > outZ = null)
        {
            using (ILScope.Enter()) {
                if (!X.IsVector || !Y.IsVector || !Z.IsVector)
                {
                    throw new ILArgumentException("inputs must be vectors");
                }
                if (!X.IsRowVector) X = X.T; 
                if (!Y.IsColumnVector) Y = Y.T;  
                Z = Z[full].Reshape(new ILSize(1,1,Z.Size.NumberOfElements)); 

                ILArray< float > ret = repmat(X,Y.Length,1,Z.Length);
                if (!isnull(outY)) outY.a = repmat(Y,1,X.Length,Z.Length);
                if (!isnull(outZ)) outZ.a = repmat(Z, Y.Length, X.Length, 1);
                return ret;
            }
        }

#endregion HYCALPER AUTO GENERATED CODE
   }
}
