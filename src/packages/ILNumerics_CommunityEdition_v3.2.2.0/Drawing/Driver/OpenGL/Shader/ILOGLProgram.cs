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

    [System.Diagnostics.DebuggerDisplay("{GLID} - {Shaders}")]
    public class ILOGLProgram {

        // we dont need locks on this cache, since it is accessed only by the rendering thread
        public readonly static Dictionary<string,int> Programs = new Dictionary<string,int>(); 

        //public static ILOGLProgram Positions = Create(
        //                            new ILOGLShaderDefinition() { Name = "VS_POS.vp", Type = ShaderType.VertexShader },
        //                            new ILOGLShaderDefinition() { Name = "FS_BLACK.fp", Type = ShaderType.FragmentShader });
        //public static ILOGLProgram PositionsColors = Create(
        //                            new ILOGLShaderDefinition() { Name = "VS_POS_COL.vp", Type = ShaderType.VertexShader },
        //                            new ILOGLShaderDefinition() { Name = "FS_COL.fp", Type = ShaderType.FragmentShader });
        
        public int GLID { get; private set; }
        public int PositionsAttribLoc { get; private set; }
        public int ColorsAttribLoc { get; private set; }
        public int NormalsAttribLoc { get; private set; }
        public int ColorUniformLoc { get; private set; }
        public int AlphaMultiplyUniformLoc { get; private set; }
        public int Model2CameraTransformMat4Loc { get; private set; }
        public int Model2Camera4NormalTransformMat3Loc { get; private set; }
        public int Camera2ClipTransformUniformLoc { get; private set; }
        public int LightsParameterExtUniformLoc { get; private set; }
        public int LogStateUniformLoc { get; private set; }
        public int ClippingUniformLoc { get; private set; }
        public int LightsArrayUniformLoc { get; private set; }
        public int A1UniformLocation { get; private set; }
        public int A2UniformLocation { get; private set; }
        public int EmissionUniformLocation { get; private set; }
        public int SpecularUniformLocation { get; private set; }
        public int ShininessUniformLocation { get; private set; }

        public int BasePositionUniformLocation { get; private set; }
        
        public string Shaders { get; private set; }

        private static ILOGLProgram Create(params ILOGLShaderDefinition[] shaders) {
            ILOGLProgram ret = new ILOGLProgram();
            GetOGLProgram(shaders, ret);
            // uniform locations
            GL.UseProgram(ret.GLID); 
            ret.Model2CameraTransformMat4Loc = GL.GetUniformLocation(ret.GLID, ILOGLShaderDefinition.Model2CameraTransform);
            ret.Model2Camera4NormalTransformMat3Loc = GL.GetUniformLocation(ret.GLID, ILOGLShaderDefinition.Model2Camera4NormalTransform);
            ret.ColorUniformLoc = GL.GetUniformLocation(ret.GLID, ILOGLShaderDefinition.ColorUniform);
            ret.AlphaMultiplyUniformLoc = GL.GetUniformLocation(ret.GLID, ILOGLShaderDefinition.AlphaMultiplyUniform);
            ret.SpecularUniformLocation = GL.GetUniformLocation(ret.GLID, ILOGLShaderDefinition.SpecularUniform);
            ret.EmissionUniformLocation = GL.GetUniformLocation(ret.GLID, ILOGLShaderDefinition.EmissionUniform);
            ret.ShininessUniformLocation = GL.GetUniformLocation(ret.GLID, ILOGLShaderDefinition.ShininessUniform);
            ret.BasePositionUniformLocation = GL.GetUniformLocation(ret.GLID, ILOGLShaderDefinition.BasePositionUniform);

            // attribute locations 
            ret.PositionsAttribLoc = GL.GetAttribLocation(ret.GLID, ILOGLShaderDefinition.PositionAttribute);
            ret.ColorsAttribLoc = GL.GetAttribLocation(ret.GLID, ILOGLShaderDefinition.ColorAttribute);
            ret.NormalsAttribLoc = GL.GetAttribLocation(ret.GLID, ILOGLShaderDefinition.NormalAttribute);
            
            // uniform block locations
            ret.Camera2ClipTransformUniformLoc = GL.GetUniformBlockIndex(ret.GLID, ILOGLShaderDefinition.Camera2ClipTransformUniformBlock);
            if (ret.Camera2ClipTransformUniformLoc >= 0) {
                GL.UniformBlockBinding(ret.GLID, ret.Camera2ClipTransformUniformLoc, ILOGLUniformBlockIndices.Camera2ClipTransform);
            }
            ret.LogStateUniformLoc = GL.GetUniformBlockIndex(ret.GLID, ILOGLShaderDefinition.LogStateUniformBlock);
            if (ret.LogStateUniformLoc >= 0) {
                GL.UniformBlockBinding(ret.GLID, ret.LogStateUniformLoc, ILOGLUniformBlockIndices.LogState);
            }
            ret.ClippingUniformLoc = GL.GetUniformBlockIndex(ret.GLID, ILOGLShaderDefinition.ClippingUniformBlock);
            if (ret.ClippingUniformLoc >= 0) {
                GL.UniformBlockBinding(ret.GLID, ret.ClippingUniformLoc, ILOGLUniformBlockIndices.ClipParams);
            }
            ret.LightsArrayUniformLoc = GL.GetUniformBlockIndex(ret.GLID, ILOGLShaderDefinition.LightsArrayUniformBlock);
            if (ret.LightsArrayUniformLoc >= 0) {
                GL.UniformBlockBinding(ret.GLID, ret.LightsArrayUniformLoc, ILOGLUniformBlockIndices.LightsArray);       
            }
            ret.LightsParameterExtUniformLoc = GL.GetUniformBlockIndex(ret.GLID, ILOGLShaderDefinition.LightsParameterExtUniformBlock);
            if (ret.LightsParameterExtUniformLoc >= 0) {
                GL.UniformBlockBinding(ret.GLID, ret.LightsParameterExtUniformLoc, ILOGLUniformBlockIndices.LightsParameterExt);
            }
            ret.A2UniformLocation = GL.GetUniformBlockIndex(ret.GLID, "A2");
            if (ret.A2UniformLocation >= 0) {
                GL.UniformBlockBinding(ret.GLID, ret.A2UniformLocation, ILOGLUniformBlockIndices.LightsArray);
            }

            GL.UseProgram(0); 
            return ret; 
        }

        private static void GetOGLProgram(ILOGLShaderDefinition[] shaders, ILOGLProgram ret) {
            try {

                ret.Shaders = string.Empty;
                foreach (ILOGLShaderDefinition shader in shaders) {
                    ret.Shaders += shader.Name + " ";
                }
                ret.Shaders = ret.Shaders.Trim();
                if (Programs.ContainsKey(ret.Shaders)) {
                    ret.GLID = Programs[ret.Shaders];
                } else {
                    int exit = 2;
                    while (exit-- > 0) {
                        List<int> shaderIDs = new List<int>();
                        foreach (ILOGLShaderDefinition shader in shaders) {
                            shaderIDs.Add(ILOGLShaderManager.UploadShaderFromResource(shader.Name, shader.Type));
                        }
                        while (GL.GetError() != ErrorCode.NoError); 
                        string error;
                        ret.GLID = ILOGLShaderManager.CreateProgram(shaderIDs, out error);
                        if (!String.IsNullOrWhiteSpace(error.Trim())) {
                            System.Diagnostics.Trace.WriteLine("");
                            System.Diagnostics.Trace.WriteLine("Shader Program Linking Error: ");
                            System.Diagnostics.Trace.WriteLine("==============================");

                            System.Diagnostics.Trace.WriteLine("Shaders: " + ret.Shaders);
                            System.Diagnostics.Trace.WriteLine(error);
                        }
                        if (GL.GetError() != ErrorCode.NoError || error.ToLower().Contains("error")) {
                            if (!Settings.OpenGL31_FIX_GL_CLIPVERTEX) {
                                Settings.OpenGL31_FIX_GL_CLIPVERTEX = true;
                            }
                            shaderIDs.ForEach((a) => {
                                GL.DeleteShader(a);
                            });
                            continue; 
                        } else {
                            Programs.Add(ret.Shaders, ret.GLID);
                            shaderIDs.ForEach((a) => {
                                GL.DeleteShader(a);
                            });
                            break; 
                        }
                    }
                }
            } catch (Exception exc) {
                System.Diagnostics.Trace.WriteLine("ILOGLProgram.GetOGLProgram Error");
                System.Diagnostics.Trace.WriteLine("================================");
                System.Diagnostics.Trace.WriteLine(exc.ToString()); 

            }
        }
        /// <summary>
        /// Create or reuse program for a specific shape. 
        /// </summary>
        /// <param name="shape">the shape</param>
        /// <returns>true: program was newly created, false: program was reused (no need to recreate VAO)</returns>
        public static bool CreateFor(ILOGLShape shape, ILRenderParameter parameter) {
            string vertShaderName, fragShaderName;
            shape.GetShaderNames(parameter, out vertShaderName, out fragShaderName);
            if (shape.Program == null || shape.Program.Shaders != vertShaderName + " " + fragShaderName) {
                ILOGLProgram ret = Create(
                    new ILOGLShaderDefinition() { Name = vertShaderName, Type = ShaderType.VertexShader },
                    new ILOGLShaderDefinition() { Name = fragShaderName, Type = ShaderType.FragmentShader });
                shape.Program = ret;
                return true; 
            }
            return false; 
        }
        public static ILOGLProgram Create(params string[] shaderNames) {
            System.Diagnostics.Debug.Assert(shaderNames.Length == 2); 
            ILOGLShaderDefinition[] shaderDefs = new ILOGLShaderDefinition[] {
                new ILOGLShaderDefinition() { Name = shaderNames[0], Type = ShaderType.VertexShader },
                new ILOGLShaderDefinition() { Name = shaderNames[1], Type = ShaderType.FragmentShader }
            }; 
            ILOGLProgram ret = Create(shaderDefs); 
            return ret;
        }

    }
}
