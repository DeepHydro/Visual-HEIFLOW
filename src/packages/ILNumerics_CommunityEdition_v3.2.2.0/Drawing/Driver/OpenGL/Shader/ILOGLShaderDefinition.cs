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
    public class ILOGLShaderDefinition {
        // attribute and uniform names must match those used within the shaders! 
        public static readonly string PositionAttribute = "PositionAttribute";
        public static readonly string NormalAttribute = "NormalAttribute";
        public static readonly string ColorAttribute = "ColorAttribute";
        public static readonly string TexCoordAttribute = "TexCoordAttribute";
        public static readonly string ColorUniform = "ColorUniform";
        public static readonly string ColorOverrideUniform = "ColorOverrideUniform";
        public static readonly string AlphaMultiplyUniform = "AlphaMultiplyUniform";

        public static readonly string SpecularUniform = "SpecularUniform";
        public static readonly string EmissionUniform = "EmissionUniform";
        public static readonly string ShininessUniform = "ShininessUniform";
        public static readonly string BasePositionUniform = "BasePositionUniform";
        public static readonly string FringeSizeUniform = "FringeSizeUniform";
        public static readonly string FringeColorUniform = "FringeColorUniform";
        public static readonly string ColorTextureSampler = "ColorTextureSampler";
        public static readonly string OrthographicTransform = "OrthographicTransform";
        public static readonly string Model2CameraTransform = "Model2CameraTransform";
        public static readonly string Model2Camera4NormalTransform = "Model2Camera4NormalTransform";

        public static readonly string Camera2ClipTransformUniformBlock = "Camera2ClipTransform";
        public static readonly string LogStateUniformBlock = "LogState";
        public static readonly string ClippingUniformBlock = "A4Clip"; // name is a fix for a nasty bug in some graphics driver!
        public static readonly string LightsParameterExtUniformBlock = "A1LightParameterExt";  // name is a fix for a nasty bug in some graphics driver!
        public static readonly string LightsArrayUniformBlock = "A2LightsArray";

        public static readonly string VS_POS_COL_FLAT_TEXBLOOM = "POS_COL_FLAT_TEXBLOOM.vp";
        public static readonly string VS_POS_COL_FLAT = "POS_COL_FLAT.vp";
        public static readonly string VS_POS_COL_FLAT_PICK = "POS_COL_FLAT_PICK.vp";  // not used ? 
        public static readonly string VS_POS_COLS_SMOOTH = "POS_COLS_SMOOTH.vp";
        public static readonly string VS_POS_COL_FLAT_LIGHT = "POS_COL_FLAT_LIGHT.vp";
        public static readonly string VS_POS_COLS_SMOOTH_LIGHT = "POS_COLS_SMOOTH_LIGHT.vp";
        public static readonly string VS_POS_BASEPOS_PICK_LABEL = "POS_COL_FLAT_4PICK.vp";

        public static readonly string FS_COL_FLAT_TEXBLOOM = "COL_FLAT_TEXBLOOM.fp";
        public static readonly string FS_COL_FLAT = "COL_FLAT.fp";
        public static readonly string FS_COL_SMOOTH = "COL_SMOOTH.fp";
        public static readonly string FS_COL_SMOOTH_LIGHT = "COL_SMOOTH_LIGHT.fp";
        public static readonly string FS_COL_FLAT_LIGHT = "COL_FLAT_LIGHT.fp";
       
        public string Name; 
        public ShaderType Type; 

    }
}
