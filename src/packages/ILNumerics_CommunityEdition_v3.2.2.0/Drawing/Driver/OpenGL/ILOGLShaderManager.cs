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
using System.Reflection; 
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL; 
using System.IO; 

namespace ILNumerics.Drawing {

    public class ILOGLShaderManager {
        
        static Assembly m_assembly;

        private static Assembly MyAssembly {
            get { 
                if (m_assembly == null) 
                    m_assembly = Assembly.GetExecutingAssembly();
                if (m_assembly == null) {
                    // todo: check the case, when exe was called from CMD!
                }
                return m_assembly; 
            }
        }
        public static int UploadShaderFromString(string shader, ShaderType type, out string info) {

            int shaderId = GL.CreateShader(type);
            GL.ShaderSource(shaderId, shader);
            GL.CompileShader(shaderId);
            info = GL.GetShaderInfoLog(shaderId);
            return shaderId; 
        }

        public static int UploadShaderFromResource(string resourceID, ShaderType type) {
            try {
                string code, dummy;
                if (!TryLoadFromRessource(resourceID, out code, out dummy)) {
                    throw new Exception(String.Format("No resource containing '{0}' was found in the assembly '{1}'",resourceID, MyAssembly.FullName)); 
                }
                // handle #define conditionals
                if (Settings.OpenGL31_FIX_GL_CLIPVERTEX) 
                    code = code.Replace("//#define FIX_GL_CLIPVERTEX", "#define FIX_GL_CLIPVERTEX"); 
                string error; 
                var ret = UploadShaderFromString(code, type, out error);
                if (!String.IsNullOrWhiteSpace(error)) {
                    System.Diagnostics.Trace.WriteLine("Shader Compilation Info/Error. Ressource ID: " + resourceID);
                    System.Diagnostics.Trace.WriteLine(error); 
                }
                return ret; 
            } catch (Exception exc) {
                throw new Exception("Error loading shader. See inner exception for details!",exc); 
            }
        }

        private static bool TryLoadFromRessource(string resourceName, out string  content, out string usedResourceName) {
            bool found = false; usedResourceName = string.Empty;
            if (MyAssembly != null) {
                IEnumerable<string> resNames = MyAssembly.GetManifestResourceNames();
                foreach (string s in resNames) {
                    if (s.Contains(resourceName)) {
                        found = true;
                        usedResourceName = s;
                        break;
                    }
                }
            }
            if (!found) {
                content = string.Empty;
                return false;
            }
            using (Stream resStream = MyAssembly.GetManifestResourceStream(usedResourceName)) {
                using (StreamReader sr = new StreamReader(resStream)) {
                    content = sr.ReadToEnd();
                    return true; 
                }
            }
        }
        public static int CreateProgram(IEnumerable<int> shaders, out string error) {

            int progId = GL.CreateProgram();

            foreach (int shaderID in shaders) {
                GL.AttachShader(progId, shaderID);
            }

            GL.LinkProgram(progId);

            error = GL.GetProgramInfoLog(progId);

            foreach (int shaderId in shaders)
                GL.DetachShader(progId, shaderId);

            return progId;
        }

    }
}
