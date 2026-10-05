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
using OpenTK; 
using OpenTK.Graphics; 
using OpenTK.Graphics.OpenGL;
using ILNumerics.Exceptions; 

namespace ILNumerics.Drawing {

    /// <summary>
    /// an uniform buffer object, holds and synchronizes all light data 
    /// </summary>
    internal class ILOGLLightsBuffer {

        internal static readonly int MAX_NUMBER_LIGHTS = 20;
        internal static readonly int LIGHTS_BLOCK_STRUCT_SIZE_1STPART;
        internal static readonly int LIGHTS_BLOCK_STRUCT_SIZE_2NDPART_ITEM;

        internal LightParameterExt Parameter; 
        private Light[] m_lights;
        internal List<Light> Lights {
            set {
                if (value.Count > MAX_NUMBER_LIGHTS) 
                    throw new ILArgumentException("Too many lights defined. Max allowed: " + MAX_NUMBER_LIGHTS); 
                value.CopyTo(m_lights);
                Parameter.NumberOfLights = value.Count; 
            }
        }

        int m_uniformBlockGLID = -1;
        int m_glLightsCount; 

        public ILOGLLightsBuffer() {
            Parameter.NumberOfLights = 0;
            m_glLightsCount = 0; 
            m_lights = new Light[MAX_NUMBER_LIGHTS]; 
            Parameter.Attenuation = 1f; 
        }
        static ILOGLLightsBuffer() {
            LIGHTS_BLOCK_STRUCT_SIZE_1STPART = System.Runtime.InteropServices.Marshal.SizeOf(typeof(LightParameterExt));
            LIGHTS_BLOCK_STRUCT_SIZE_2NDPART_ITEM = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Light));
        }

        public void Update() {
            if (m_lights == null) return; 
            // DEBUG - TODO improve! 
            if (Parameter.NumberOfLights < 1) return;
            if (Parameter.NumberOfLights > MAX_NUMBER_LIGHTS)
                throw new ILArgumentException("The number of lights must not be greater than " + MAX_NUMBER_LIGHTS);
            if (m_uniformBlockGLID <= 0 || Parameter.NumberOfLights != m_glLightsCount) {
                setupBuffer(Parameter.NumberOfLights); 
            }
            // send to GL memory 
            GL.BindBuffer(BufferTarget.UniformBuffer, m_uniformBlockGLID);
            GL.BufferData(BufferTarget.UniformBuffer, (IntPtr)LIGHTS_BLOCK_STRUCT_SIZE_1STPART, ref Parameter, BufferUsageHint.DynamicRead);
            int size = ComputeBufferSize(Parameter.NumberOfLights) - LIGHTS_BLOCK_STRUCT_SIZE_1STPART; // size without ambient parameter
            GL.BufferSubData(BufferTarget.UniformBuffer,(IntPtr)LIGHTS_BLOCK_STRUCT_SIZE_1STPART, (IntPtr)size, m_lights);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }
        /*  The corresponding GLSL buffer struct looks like that: 
        * 
        *      struct Light {           |
        *      	vec4 CamSpacePos;       > this is like our
        *      	vec3 Color;             | Drawing.Light struct
        *      	uint Flags;             }
        *      }
        *      uniform LightsBuffer {    
        *      	vec3 AmbientColor;      <- added to buffer on
        *      	float Attenuation;      <- the fly
        *      	Light Lights[NumberOfLights];
        *      } LgtBuffer; 
        * 
        */
        private static int ComputeBufferSize(int lightsCount) {
            return LIGHTS_BLOCK_STRUCT_SIZE_2NDPART_ITEM * lightsCount + LIGHTS_BLOCK_STRUCT_SIZE_1STPART;  // uniform LightsBuffer size
        }
        private void setupBuffer(int nrLights) {
            if (m_uniformBlockGLID != 0) {
                freeBuffer(); 
            }
            m_glLightsCount = Parameter.NumberOfLights; 
            GL.GenBuffers(1, out m_uniformBlockGLID); 
            GL.BindBuffer(BufferTarget.UniformBuffer, m_uniformBlockGLID);
            int size = ComputeBufferSize(nrLights);
            GL.BufferData(BufferTarget.UniformBuffer, (IntPtr)size, (IntPtr)0, BufferUsageHint.DynamicRead);
            GL.BindBufferRange(BufferTarget.UniformBuffer, ILOGLUniformBlockIndices.LightsArray, 
                                m_uniformBlockGLID, (IntPtr)0, (IntPtr)size); 
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }
        private void freeBuffer() {
            System.Diagnostics.Debug.Assert(m_uniformBlockGLID != 0); 
            GL.DeleteBuffers(1, ref m_uniformBlockGLID); 
            m_uniformBlockGLID = 0; 
        }
    }
}
