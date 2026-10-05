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
using System.Drawing;
using System.Linq;
using System.Text;

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILCircle : ILGroup {

        #region attributes
        public ILTrianglesFan Fill { get; private set; }
        public ILLineStrip Border { get; private set; }
        public int Resolution { get; private set; }
        public static string FillTagDefault = "Fill";
        public static string BorderTagDefault = "Border"; 
        public static string CircleGroupTag = "Circle"; 
        #endregion

        #region constructors
        internal ILCircle(ILCircle source) : base(source) {
            Fill = First<ILTrianglesFan>(source.Fill.Tag);
            Border = First<ILLineStrip>(source.Border.Tag); 
            Resolution = source.Resolution; 
        }

        public ILCircle(int resolution = 100, object tag = null)
            : base(tag ?? CircleGroupTag) {
                Resolution = resolution;
            Recreate();
        }
        #endregion 

        #region public functions
        internal override ILNode Copy() {
            return new ILCircle(this);
        }
        #endregion

        #region private helper
        private void Recreate() {
            using (ILScope.Enter()) {
                Fill = Add(new ILTrianglesFan() { Color = Color.DarkGray }, FillTagDefault);
                Border = Add(new ILLineStrip() { Color = Color.Black }, BorderTagDefault);

                Fill.Positions.Update(0, Resolution + 1, Computation.CreatePositions(Resolution));
                Border.Positions = Fill.Positions; 

                ILArray<int> ind = Computation.CreateIndices(Resolution); 
                Border.Indices.Update(0,ind.S[1],ind); 
            }
        }
        #endregion

        [System.Security.SecuritySafeCritical]
        private class Computation : ILMath {
            internal static ILRetArray<float> CreatePositions(int resolution) {
                using (ILScope.Enter()) {
                    ILArray<float> ret = zeros<float>(3,resolution+1); 
                    ILArray<float> cir = linspace<float>(0,pi*2,resolution);
                    ret[0, r(1, end)] = sin(cir);
                    ret[1, r(1, end)] = cos(cir);
                    //ret[2, r(0, end-1)] = 0; 
                    return ret; 
                }
            }

            internal static ILRetArray<int> CreateIndices(int Resolution) {
                using (ILScope.Enter()) {
                    ILArray<int> ret = counter<int>(1.0,1.0,size(1,Resolution+1)); 
                    ret[end] = 1; 
                    return ret; 
                }
            }
        }


    }
}
