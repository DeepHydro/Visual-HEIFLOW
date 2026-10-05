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
using System.Drawing; 

namespace ILNumerics.Drawing {
    /// <summary>
    /// Single item with rendering instructions, used in ILRenderQueues
    /// </summary>
    public class ILRenderQueueItem { 
        /// <summary>
        /// unique key, identifies the item in the render cache
        /// </summary>
        public string Key; 
        /// <summary>
        /// rendering offset for the item (used for sub-/superscripts etc.)
        /// </summary>
        public System.Drawing.RectangleF Rect; 
        /// <summary>
        /// individual color for the item 
        /// </summary>
        /// <remarks>If this property is set to Color.Empty, the item will 
        /// be drawn with the color assigned to hosting element.</remarks>
        public Color Color; 
        /// <summary>
        /// [optional] area of the item to draw from 
        /// </summary>
        public RectangleF TextureRect; 
        /// <summary>
        /// The transformed text of the item 
        /// </summary>
        public string Text; 

        /// <summary>
        /// construct a new ILRenderQueueItem
        /// </summary>
        /// <param name="key">unique key</param>
        /// <param name="offset">offset</param>
        /// <param name="color">individual color</param>
        public ILRenderQueueItem(string key, System.Drawing.RectangleF rect, RectangleF texRect, Color color) {
            Key = key;
            Rect = rect;
            Color = color; 
            TextureRect = texRect; 
        }
    }
}
