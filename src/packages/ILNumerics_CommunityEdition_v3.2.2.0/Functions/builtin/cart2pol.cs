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
using System.Linq;
using System.Text;

namespace ILNumerics {

    // ToDo: DOKU remarks and returns seem outdated...
    public partial class ILMath {

        /// <summary>
        /// Transform scalar coordinates into polar (cylindrical) coordinates
        /// </summary>
        /// <param name="X">X coordinates</param>
        /// <param name="Y">Y coordinates</param>
        /// <param name="Z">Z coordinates (height)</param>
        /// <param name="outRadius">[Output] Radius if not null on entry</param>
        /// <param name="outZ">[Output] Z if not null on entry</param>
        /// <returns>Angles; radius and Z are returned as output parameters, if on entry not null</returns>
        /// <remarks>Theta, radius and Z must be the same size or either one may be scalar. 
        /// Polar coordinate arrays returned are of the same size then the input arrays.</remarks>
        public static ILRetArray<double> cart2pol(ILInArray<double> X, ILInArray<double> Y, ILInArray<double> Z
                                        ,ILOutArray<double> outRadius, ILOutArray<double> outZ) {
            using (ILScope.Enter(X,Y,Z)) {    
                if (!object.Equals(outRadius, null)) 
                    outRadius.a = sqrt(X * X + Y * Y); 
                if (!object.Equals(outZ,null)) 
                    outZ.a = Z.C; 
                return atan2(Y,X); 
            }
        }

        /// <summary>
        /// Transform scalar coordinates into polar (cylindrical) coordinates
        /// </summary>
        /// <param name="X">X coordinates</param>
        /// <param name="Y">Y coordinates</param>
        /// <param name="Z">Z coordinates (height)</param>
        /// <param name="outRadius">[Output] Radius if not null on entry</param>
        /// <param name="outZ">[Output] Z if not null on entry</param>
        /// <returns>Angles; radius and Z are returned as output parameters, if on entry not null</returns>
        /// <remarks>Theta, radius and Z must be the same size or either one may be scalar. 
        /// Polar coordinate arrays returned are of the same size then the input arrays.</remarks>
        public static ILRetArray<float> cart2pol(ILInArray<float> X, ILInArray<float> Y, ILInArray<float> Z
                                        ,ILOutArray<float> outRadius, ILOutArray<float> outZ) {
            using (ILScope.Enter(X,Y,Z)) {    
                if (!object.Equals(outRadius, null))
                    outRadius.a = sqrt(X * X + Y * Y); 
                if (!object.Equals(outZ,null)) 
                    outZ.a = Z.C; 
                return atan2(Y,X); 
            }
        }
    }
}
