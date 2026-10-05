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
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL; 

namespace ILNumerics.Drawing {
    public class ILOGLLines : ILOGLShape {

        internal ILOGLLines(ILShape shape, Dictionary<int, ILOGLBuffer> bufferMapping) 
            : base(shape, bufferMapping) { }

        protected override void PreRender() {
            // line setup
            SetupLineStyle(Shape as ILLines);
        }
        protected override void PostRender() {
            // nothing to do
            // ( overwrites polygon offset ) 
        }
        internal static void SetupLineStyle(ILLines wireprops) {
            if (wireprops.DashStyle == DashStyle.Solid) {
                GL.Disable(EnableCap.LineStipple);
            } else {
                int stipFactr = 10;
                short stipple;
                if (wireprops.DashStyle != DashStyle.UserPattern)
                    stipple = StippleFromLineStyle(
                                    wireprops.DashStyle, ref stipFactr);
                else {
                    stipple = wireprops.Pattern;
                    stipFactr = (int)wireprops.PatternScale * 10;
                }
                GL.Enable(EnableCap.LineStipple);
                GL.LineStipple(stipFactr, stipple);
            }
            if (wireprops.Antialiasing && wireprops.Width > 1)
                GL.Enable(EnableCap.LineSmooth);
            else
                GL.Disable(EnableCap.LineSmooth);
            GL.LineWidth(wireprops.Width);
            //GL.Color3(wireprops.Color);
        }
        internal static short StippleFromLineStyle(DashStyle style, ref int stipFactr) {
            short ret = 1;
            switch (style) {
                case DashStyle.Dashed:
                    ret = (short)255;
                    stipFactr = 2;
                    break;
                case DashStyle.PointDash:
                    ret = (short)255 + 2048;
                    stipFactr = 3;
                    break;
                case DashStyle.Dotted:
                    ret = (short)13107; // 3 + 48 + 768 + 8192 + 4096;
                    stipFactr = 2;
                    break;
                case DashStyle.UserPattern:
                    break;
                default:      // solid
                    ret = (short)-1;
                    break;
            }
            return ret;
        }

    }
}
