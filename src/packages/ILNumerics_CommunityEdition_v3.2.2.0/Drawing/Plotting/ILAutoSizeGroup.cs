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

namespace ILNumerics.Drawing.Plotting {
    /// <summary>
    /// Container class for arbitrary 3D nodes, provides a local coordinate system for drawing inside a specific screen rectangle
    /// </summary>
    /// <remarks>The class ensures, that the projection of a virtual cube (of size [-1 ... 1] in each direction) is 
    /// limited to the screen rectangle given in 'Rect'. The current camera position and projection is taken into account. 
    /// Therefore, any transformation does only affect the local driver and not the global scene.</remarks>
    public class ILAutoSizeGroup : ILGroup {

        /// <summary>
        /// Rectangular area of the controls surface the rendering of all childs controls is limited to, range 0..1
        /// </summary>
        public RectangleF ScreenRect { get; set; }

        public ILAutoSizeGroup() { 
            ScreenRect = new RectangleF(0,0,1,1); 
        }
        public ILAutoSizeGroup(object tag = null) : base(tag) {
            ScreenRect = new RectangleF(0,0,1,1); 
        }
        public ILAutoSizeGroup(ILAutoSizeGroup source)
            : base(source) {
            ScreenRect = source.ScreenRect; 
        }
        internal override ILNode Copy() {
            return new ILAutoSizeGroup(this);
        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILAutoSizeGroup();
        }
        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILAutoSizeGroup ret = (ILAutoSizeGroup)base.Synchronize(copy, syncParams);
            ret.ScreenRect = ScreenRect;
            return ret; 
        }
        protected override bool BeginVisit(ILRenderParameter parameter) {
            if (parameter.CurrentPassCount == 0) {
                CalculateTransform(parameter);
            }
            return base.BeginVisit(parameter);
        }
        protected virtual void CalculateTransform(ILRenderParameter parameter) {
            // todo: take projection transform into account: take current limits, transform them into clip 
            // coordinates and use the extend to scale up/down the transform
            Matrix4 CT =
                Matrix4.Translation(-1, 1, 0) *
                Matrix4.ScaleTransform(2, -2, 1) *
                Matrix4.Translation(ScreenRect.Left, ScreenRect.Top, 0) *
                Matrix4.ScaleTransform(ScreenRect.Width, ScreenRect.Height, 1) *
                Matrix4.ScaleTransform(0.5f, -0.5f, 1) *               
                Matrix4.Translation(1,-1,0); 
            Matrix4 cam = parameter.PeekClipTransform();
            Matrix4 camInv = Matrix4.Invert(cam);
            Transform = camInv * CT * cam * Transform; 
        }

    }
}
