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
using System.Drawing; 


namespace ILNumerics.Drawing {
    internal class ILOGLLabel {

        internal readonly int LABEL_TEXTURE_UNIT = 0; 
        private ILLabel m_label;
        private ILTextureStorage m_textureStorage;
        private string m_cachedExpression = String.Empty; 
        private IILTextInterpreter m_cachedInterpreter; 
        private string m_cachedFont; 
        private ILFringe m_cachedFringe;
        private Color m_cachedColor; 

        private ILRenderQueue m_renderQueue; 
        private float[] m_vertices; 
        private float[] m_textureCoords;
        private float[] m_colors;
        internal ILOGLProgram Program { get; set; }
        internal ILOGLProgram PickProgram { get; set; }
        internal int? VAO { get; set; }

        private int m_unifBasePositionLoc, m_unifTextureSamplerLoc, m_unifOrthoLoc, m_unifFringeSizeLoc, m_unifFringeColorLoc, m_unifPickColorLoc; 
        private int m_positionAttrLoc, m_textureAttrLoc; 
        private int? m_positionBufferObj, m_textureBufferObj, m_colorBufferObj; 

        public ILOGLLabel(ILLabel label, GraphicsContext context) {
            // TODO: Complete member initialization
            this.m_label = label;
            this.m_textureStorage = ILTextureStorage.GetStorage(context, () => new ILOGLTextureStorage(ILTextureStorage.DefaultSize));
            m_textureStorage.Cleared += new EventHandler((a,e) => { m_cachedExpression = string.Empty; });
            setup();
        }

        private void setup() {
            m_renderQueue = m_label.Interpreter.Transform(m_label, m_textureStorage); 
            m_cachedInterpreter = m_label.Interpreter; 
            m_cachedExpression = m_label.Text; 
            m_cachedFont = m_label.Font.ToString(); 
            m_cachedFringe = m_label.Fringe.Clone();
            m_cachedColor = m_label.Color ?? Color.Empty; 

            // setup vertex buffer object
            if (!VAO.HasValue) {
                int newID;
                GL.GenVertexArrays(1, out newID);
                VAO = newID;
            }
            GL.BindVertexArray(VAO.Value);

            // setup shader and program - regular shaders
            if (Program == null) {
                Program = ILOGLProgram.Create(ILOGLShaderDefinition.VS_POS_COL_FLAT_TEXBLOOM, ILOGLShaderDefinition.FS_COL_FLAT_TEXBLOOM);
            }

            m_textureAttrLoc = GL.GetAttribLocation(Program.GLID, ILOGLShaderDefinition.TexCoordAttribute);
            m_positionAttrLoc = GL.GetAttribLocation(Program.GLID, ILOGLShaderDefinition.PositionAttribute);
            m_unifBasePositionLoc = GL.GetUniformLocation(Program.GLID, ILOGLShaderDefinition.BasePositionUniform);
            m_unifFringeColorLoc = GL.GetUniformLocation(Program.GLID, ILOGLShaderDefinition.FringeColorUniform);
            m_unifFringeSizeLoc = GL.GetUniformLocation(Program.GLID, ILOGLShaderDefinition.FringeSizeUniform);
            m_unifOrthoLoc = GL.GetUniformLocation(Program.GLID, ILOGLShaderDefinition.OrthographicTransform);
            m_unifTextureSamplerLoc = GL.GetUniformLocation(Program.GLID, ILOGLShaderDefinition.ColorTextureSampler);
            
            // setup shader and program - picking shaders
            if (PickProgram == null) {
                PickProgram = ILOGLProgram.Create(ILOGLShaderDefinition.VS_POS_BASEPOS_PICK_LABEL, ILOGLShaderDefinition.FS_COL_FLAT);
            }

            // update buffer
            if (m_vertices == null || m_vertices.Length < m_renderQueue.Count * 8) {
                m_vertices = new float[m_renderQueue.Count * 12]; // ... give some headroom
            }
            if (m_textureCoords == null || m_textureCoords.Length < m_renderQueue.Count * 8) {
                m_textureCoords = new float[m_renderQueue.Count * 12]; // ... give some headroom
            }
            if (m_colors == null || m_colors.Length < m_renderQueue.Count * 16) {
                m_colors = new float[m_renderQueue.Count * 20]; // ... give some headroom
            }
            int pos = 0, cpos = 0;

            #region setup vertex attributes
            foreach (ILRenderQueueItem item in m_renderQueue) {
                // bottom left
                m_textureCoords[pos++] = item.TextureRect.Left;
                m_textureCoords[pos++] = item.TextureRect.Bottom;
                m_textureCoords[pos++] = item.TextureRect.Left;
                m_textureCoords[pos++] = item.TextureRect.Top;
                m_textureCoords[pos++] = item.TextureRect.Right;
                m_textureCoords[pos++] = item.TextureRect.Top;
                m_textureCoords[pos++] = item.TextureRect.Right;
                m_textureCoords[pos++] = item.TextureRect.Bottom;
                pos -= 8; 
                // vertices
                m_vertices[pos++] = (int)item.Rect.Left;
                m_vertices[pos++] = (int)item.Rect.Bottom;
                m_vertices[pos++] = (int)item.Rect.Left;
                m_vertices[pos++] = (int)item.Rect.Top;
                m_vertices[pos++] = (int)item.Rect.Right;
                m_vertices[pos++] = (int)item.Rect.Top;
                m_vertices[pos++] = (int)item.Rect.Right;
                m_vertices[pos++] = (int)item.Rect.Bottom;
                //m_vertices[pos++] = (int)item.Rect.Left + 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Bottom - 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Left + 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Top + 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Right - 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Top + 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Right - 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Bottom - 0.5f;

                //m_vertices[pos++] = (int)item.Rect.Left - 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Bottom + 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Left - 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Top - 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Right + 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Top - 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Right + 0.5f;
                //m_vertices[pos++] = (int)item.Rect.Bottom + 0.5f;
                float r = item.Color.R / (float)0xff; 
                float g = item.Color.G / (float)0xff;
                float b = item.Color.B / (float)0xff;
                float a = item.Color.A / (float)0xff;

                m_colors[cpos++] = r;
                m_colors[cpos++] = g;
                m_colors[cpos++] = b;
                m_colors[cpos++] = a;
                m_colors[cpos++] = r;
                m_colors[cpos++] = g;
                m_colors[cpos++] = b;
                m_colors[cpos++] = a;
                m_colors[cpos++] = r;
                m_colors[cpos++] = g;
                m_colors[cpos++] = b;
                m_colors[cpos++] = a;
                m_colors[cpos++] = r;
                m_colors[cpos++] = g;
                m_colors[cpos++] = b;
                m_colors[cpos++] = a;
            }
            #endregion

            if (!m_positionBufferObj.HasValue) {
                int id;
                GL.GenBuffers(1, out id);
                m_positionBufferObj = id;
            }

            //end DEBUG
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_positionBufferObj.Value);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * m_renderQueue.Count * 8), m_vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(m_positionAttrLoc); 
            GL.VertexAttribPointer(m_positionAttrLoc, 2, VertexAttribPointerType.Float, false, 0, 0);

            if (!m_textureBufferObj.HasValue) {
                int id;
                GL.GenBuffers(1, out id);
                m_textureBufferObj = id;
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_textureBufferObj.Value);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * m_renderQueue.Count * 8), m_textureCoords, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(m_textureAttrLoc, 2, VertexAttribPointerType.Float, false, 0, 0);
            
            if (!m_colorBufferObj.HasValue) {
                int id;
                GL.GenBuffers(1, out id);
                m_colorBufferObj = id;
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_colorBufferObj.Value);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * m_renderQueue.Count * 16), m_colors, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(Program.ColorsAttribLoc);
            GL.VertexAttribPointer(Program.ColorsAttribLoc, 4, VertexAttribPointerType.Float, false, 0, 0); 

            GL.BindVertexArray(0);
        }

        internal void Draw(ILRenderParameter parameters) {
            if (!String.IsNullOrEmpty(m_label.Text) && m_label.Visible) {
                if (m_cachedInterpreter != m_label.Interpreter || m_cachedExpression != m_label.Text
                    || m_cachedFont != m_label.Font.ToString() || !m_label.Fringe.Equals(m_cachedFringe)
                    || m_label.Color != m_cachedColor) {
                    setup(); 
                }
                Vector3 basePosition = parameters.ToScreen(m_label.Position);  // screen coords
                Vector3 anch = Vector3.Round(new Vector3(m_label.Anchor.X * m_renderQueue.Size.Width,  m_label.Anchor.Y * m_renderQueue.Size.Height, 0));
                // in order to rotate the label we first move it to the origin, rotate and move it back:
                RectangleF viewport = parameters.ViewTransform.ToViewRectangle();
                //viewport.Inflate(1,1);
                Vector3 position = Vector3.Round(basePosition); 
                Vector4 clip = parameters.ToClip(m_label.Position); 
                position.Z = clip.Z / clip.W; 
                Matrix4 orthoMat = Matrix4.OrthographicTransform(viewport.Left, viewport.Right, viewport.Top, viewport.Bottom, 1, -1) // screen -> ndc coords
                                 * Matrix4.Translation(position.X, position.Y, 0)
                                 * Matrix4.Rotation(new Vector3(0, 0, 1), -m_label.Rotation)
                                 * Matrix4.Translation(-position.X - anch.X, -position.Y - anch.Y, 0);

                // general setup
                GL.Disable(EnableCap.CullFace);
                //if (m_label.Target == RenderTarget.Screen2D)
                //GL.Disable(EnableCap.DepthTest);

                // prepare for rendering
                GL.BindVertexArray(VAO.Value);
                
                if (parameters.PickingContext == null) {
                    //GL.ActiveTexture(TextureUnit.Texture0); 
                    // .. binding of texture is done from TextureStorage.MakeCurrent()
                    GL.UseProgram(Program.GLID);
                    GL.Uniform1(m_unifTextureSamplerLoc, LABEL_TEXTURE_UNIT);
                    GL.Uniform3(Program.BasePositionUniformLocation, position.X, position.Y, position.Z);
                    unsafe {
                        GL.UniformMatrix4(m_unifOrthoLoc, 1, false, (float*)&orthoMat);
                    }

                    m_textureStorage.MakeCurrent();

                    //GL.Enable(EnableCap.Blend);
                    //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                    // fringe setup
                    if (m_cachedFringe.Width > 0) {
                        unsafe {
                            int dummy = m_cachedFringe.Width;
                            if (dummy == 0 && parameters.PickingContext != null) {
                                dummy = 1;
                            }
                            GL.Uniform1(m_unifFringeSizeLoc, dummy);
                            Color frColor = m_cachedFringe.Color;
                            GL.Uniform3(m_unifFringeColorLoc, frColor.R / (float)0xff, frColor.G / (float)0xff, frColor.B / (float)0xff);
                        }
                        GL.DrawArrays(BeginMode.Quads, 0, m_renderQueue.Count * 4);
                    }
                    // regular setup 
                    GL.Uniform1(m_unifFringeSizeLoc, 0);
                    GL.DrawArrays(BeginMode.Quads, 0, m_renderQueue.Count * 4);

                } else {
                    // picking setup
                    GL.UseProgram(PickProgram.GLID);
                    GL.Uniform3(PickProgram.BasePositionUniformLocation, position.X, position.Y, position.Z);
                    unsafe {
                        GL.UniformMatrix4(PickProgram.Model2CameraTransformMat4Loc, 1, false, (float*)&orthoMat);
                    }
                    Color frColor = parameters.PickingContext.RegisterNextShape(m_label.PickingID);
                    GL.Uniform4(PickProgram.ColorUniformLoc, frColor); 
                    GL.Uniform1(PickProgram.AlphaMultiplyUniformLoc, 1f);
                    GL.DrawArrays(BeginMode.Quads, 0, m_renderQueue.Count * 4);
                }
                ((ILOGLDriver)parameters.Driver).ConfigureCull();
                //if (parameters.DepthTestEnabled)
                //    GL.Enable(EnableCap.DepthTest);
                //GL.Disable(EnableCap.Blend);

                GL.BindVertexArray(0); 
                GL.UseProgram(0); 
            }
        }
        public override string ToString() {
            return String.Format("VAO:{0} '{1}' Queue Len:{2}" , this.VAO, m_cachedExpression, m_renderQueue.Count);
        }


    }
}
