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
using System.Drawing; 

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILCylinder : ILGroup {

        #region attributes
        public ILCircle Bottom { get; private set; }
        public ILCircle Top { get; private set; }
        public ILTrianglesStrip Hull { get; private set; }
        public int Resolution { get; private set; }
        public static string BottomTag = "Bottom";
        public static string TopTag = "Top";
        public static string HullTag = "Hull";
        public static string CylinderGroupTag = "Cylinder";  
        #endregion

        #region constructors
        internal ILCylinder(ILCylinder source) : base(source) { }
        public ILCylinder(object tag = null, int resolution = 50)
            : base(tag ?? CylinderGroupTag) {
                Resolution = resolution;
            Recreate();
        }
        #endregion 

        #region public functions
        #endregion

        #region private helper
        private void Recreate() {
            using (ILScope.Enter()) {
                ILCircle circ = new ILCircle(Resolution);
                Bottom = Add(new ILGroup() { Transform = Matrix4.Rotation(new Vector3(1, 0, 0), Math.PI) })
                        .Add(circ, BottomTag);
                Top = Add(new ILGroup() { Transform = Matrix4.Translation(0, 0, 1) }) 
                        .Add(circ, TopTag); 
                Hull = (ILTrianglesStrip)Add(new ILTrianglesStrip(), HullTag);
                Hull.Positions.Update(0, Resolution * 2, Computation.CreatePositions(Resolution));
                Hull.Color = Color.DarkRed;
                Hull.SpecularColor = Color.White; 
                Bottom.Fill.Color = Color.DarkGray;
                Top.Fill.Color = Color.DarkGray; 
            }
        }
        #endregion

        [System.Security.SecuritySafeCritical]
        private class Computation : ILMath {
            internal static ILRetArray<float> CreatePositions(int resolution) {
                using (ILScope.Enter()) {
                    ILArray<float> ret = zeros<float>(3,resolution); 
                    ILArray<float> cir = linspace<float>(0,pi*2,resolution); 
                    cir[end] = cir[0]; // against fp rounding issues
                    ret[0, r(0, end)] = sin(cir);
                    ret[1, r(0, end)] = cos(cir);
                    ret.a = ret[":,:;:"]; // double along columns
                    ret.a = reshape(ret, 3, resolution * 2); 
                    ret["2;0:2:end"] = 1;
                    return ret; 
                }
            }

            internal static ILRetArray<int> CreateIndices(int Resolution) {
                using (ILScope.Enter()) {
                    ILArray<int> ret = counter<int>(1.0,1.0,size(1,Resolution+1)); 
                    return ret; 
                }
            }
        }
    }
}
