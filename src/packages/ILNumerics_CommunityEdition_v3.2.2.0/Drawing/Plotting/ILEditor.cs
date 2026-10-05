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
using System.Drawing;
using System.Text;
using ILNumerics.Drawing.Animation;

namespace ILNumerics.Drawing {
    public class ILEditor : ILGroup {

        public static readonly string Tagprefix = "ILEditor_";
        ILLines m_border = new ILLines(Tagprefix + "border");
        ILTrianglesFan m_background = new ILTrianglesFan(Tagprefix + "background");
        ILBlendAnimation m_blendAction = new ILBlendAnimation(100, 2000, 400, null, 0.9f, null, Tagprefix + "blend");

        public ILEditor() { 
            m_border.Positions.Update(new float[,] {
                { 0, 0.8f, 0},
                { 1, 0.8f, 0},
                { 1, 1, 0},
                { 0, 1, 0}
            }); 
            m_border.Indices.Update(0,0,1,1,2,2,3,3,0); 
            m_background.Positions = m_border.Positions; 
            m_background.Color = Color.FromArgb(140, Color.Yellow);
            m_background.Markable = false;
            m_background.Selectable = false;
            m_background.AutoNormals = false;
            Add(m_background); 

            m_border.Color = Color.DarkGray;
            m_border.Width = 2;
            m_border.Selectable = false; 
            m_border.Markable = false; 
            Add(m_border); 

            // setup actions 
            Animations.Add(m_blendAction); 
            Configure(); 
        }

        public void BlendIn() {
            m_blendAction.Reset(); 
        }

    }
}
