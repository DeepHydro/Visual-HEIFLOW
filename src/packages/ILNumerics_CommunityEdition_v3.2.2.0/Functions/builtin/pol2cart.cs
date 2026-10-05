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

    public partial class ILMath {
        /// <summary>
        /// Transform polar/ cylindrical coordinates into scalar coordinates
        /// </summary>
        /// <param name="theta">Angle to x axis</param>
        /// <param name="radius">Radius from z axis</param>
        /// <param name="Z">Height</param>
        /// <param name="outY">If on entry not null, the Y components are returned in outY</param>
        /// <param name="outZ">If on entry not null, the Z components are returned in outZ</param>
        /// <returns>X component, Y and Z are returned as out parameter if requested</returns>
        /// <remarks>Theta, radius and Z must be of the same size or either one may be scalar. 
        /// Scalar coordinate arrays returned are of the same size then the input arrays.</remarks>
        public static ILRetArray<double> pol2cart(ILInArray<double> theta, ILInArray<double> radius, ILInArray<double> Z
                                                   ,ILOutArray<double> outY, ILOutArray<double> outZ) {
            using (ILScope.Enter(theta, radius, Z)) {
                if (!object.Equals(outY,null))
                    outY.a = radius * sin(theta); 
                if (!object.Equals(outZ,null))
                    outZ.a = Z.C; 
                return radius * cos(theta); 
            }
        }

        /// <summary>
        /// Transform polar/ cylindrical coordinates into scalar coordinates
        /// </summary>
        /// <param name="theta">Angle to x axis</param>
        /// <param name="radius">Radius from z axis</param>
        /// <param name="Z">Height</param>
        /// <param name="outY">If on entry not null, the Y components are returned in outY</param>
        /// <param name="outZ">If on entry not null, the Z components are returned in outZ</param>
        /// <returns>X component, Y and Z are returned as out parameter if requested</returns>
        /// <remarks>Theta, radius and Z must be of the same size or either one may be scalar. 
        /// Scalar coordinate arrays returned are of the same size then the input arrays.</remarks>
        public static ILRetArray<float> pol2cart( ILInArray<float> theta, ILInArray<float> radius, ILInArray<float> Z
                                                   , ILOutArray<float> outY, ILOutArray<float> outZ) {
            using (ILScope.Enter(theta, radius, Z)) {
                if (!object.Equals(outY, null))
                    outY.a = radius * sin(theta);
                if (!object.Equals(outZ, null))
                    outZ.a = Z.C;
                return radius * cos(theta);
            }
        }

    }
}
