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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Security; 

namespace ILNumerics.Drawing {
    public static class ExtensionMethods {

        public static RectangleF ToViewRectangle(this Matrix4 viewTransform) {
            Vector4 min = viewTransform * new Vector4(-1, -1, 0, 1);
            Vector4 max = viewTransform * new Vector4(1, 1, 0, 1);
            return new RectangleF(min.X, max.Y, max.X - min.X, min.Y - max.Y);
        }

        internal static ILMouseEventArgs ToILMouseEventArgs(this MouseEventArgs args, Size ClientRect, long timeMS) {
            PointF point = new PointF(
                args.Location.X / (float)ClientRect.Width,
                args.Location.Y / (float)ClientRect.Height);
            bool shiftPressed = (Control.ModifierKeys & Keys.Shift) != 0;
            bool altPressed = (Control.ModifierKeys & Keys.Alt) != 0;
            bool ctrlPressed = (Control.ModifierKeys & Keys.Control) != 0;
            return new ILMouseEventArgs(point, args,shiftPressed, altPressed, ctrlPressed) { TimeMS = timeMS };
        }

        internal static Vector3 GetPositionAt(this ILDenseArray<float> array, int i) {
            using (ILScope.Enter(array))
                return new Vector3(array.GetValue(0, i),
                                array.GetValue(1, i),
                                array.GetValue(2, i));
        }
        internal static Vector4 GetPosition4At(this ILDenseArray<float> array, int i) {
            using (ILScope.Enter(array))
                return new Vector4(array.GetValue(0, i),
                                array.GetValue(1, i),
                                array.GetValue(2, i),
                                array.GetValue(3, i));
        }
        [SecuritySafeCritical]
        public static Vector3 ToVector3(this System.Drawing.Color color) {
            return new Vector3(color.R, color.G, color.B) / 255f;
        }
        [SecuritySafeCritical]
        public static Vector4 ToVector4(this System.Drawing.Color color) {
            return new Vector4(color.R, color.G, color.B, color.A) / 255f;
        }
        [SecuritySafeCritical]
        public static System.Drawing.Color ToColor(this Vector4 color) {
            if (float.IsNaN(color.X) || float.IsNaN(color.Y) || float.IsNaN(color.Z) || float.IsNaN(color.W)) return Color.Empty;
            if (color.X > 1) color.X = 1; if (color.X < 0) color.X = 0;
            if (color.Y > 1) color.Y = 1; if (color.Y < 0) color.Y = 0;
            if (color.Z > 1) color.Z = 1; if (color.Z < 0) color.Z = 0; 
            return System.Drawing.Color.FromArgb(
                (int)(color.W * 255),
                (int)(color.X * 255),
                (int)(color.Y * 255),
                (int)(color.Z * 255));
        }
        
        [SecuritySafeCritical]
        public static ILControlBridge Make3D(this Control control) {
            ILControlBridge bridge = new ILControlBridge(); 
            bridge.Control = control; 
            return bridge; 
        }
        [SecuritySafeCritical]
        public static PointF ToPointF(this Vector3 vector) {
            return new PointF(vector.X, vector.Y); 
        }
     }
}
