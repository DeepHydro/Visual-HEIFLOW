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
using System.Security;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL; 

namespace ILNumerics.Drawing {
    public class ILOGLShape {

        protected ILOGLBuffer m_positions;
        protected ILOGLBuffer m_colors;
        protected ILOGLBuffer m_normals;
        protected ILOGLBuffer m_indices;
        protected ILOGLBuffer m_indicesSorted;

        public bool SmoothShading { get { return !Shape.Color.HasValue && Shape.Colors.DataCount > 0; } }
        public int? VAO { get; private set; }
        public ILShape Shape { get; private set; }
        public ILOGLProgram Program { get; internal set; }
        public ILOGLProgram FlatProgram { get; private set; }
        public BeginMode Primitive {
            get {
                return shape2BeginMode(Shape);
            }
        }

        public static ILOGLShape Create(ILShape shape, Dictionary<int, ILOGLBuffer> bufferMapping) {
            if (shape.Type == Primitives.Lines || shape.Type == Primitives.LineStrip) {
                return new ILOGLLines(shape, bufferMapping); 
            } else if (shape.Type == Primitives.Points) {
                return new ILOGLPoints(shape, bufferMapping); 
            }else {
                return new ILOGLShape(shape,bufferMapping); 
            }
        }

        protected ILOGLShape(ILShape shape, Dictionary<int, ILOGLBuffer> bufferMapping) {
            Shape = shape;
            m_positions = manageBuffer(shape.Positions, bufferMapping);
            m_colors = manageBuffer(shape.Colors, bufferMapping);
            m_normals = manageBuffer(shape.Normals, bufferMapping);
            m_indices = manageBuffer(shape.Indices, bufferMapping);

            m_positions.Recreated += new EventHandler(m_buffer_Recreated); // this is currently not used (3.0)
            m_colors.Recreated += new EventHandler(m_buffer_Recreated);
            m_normals.Recreated += new EventHandler(m_buffer_Recreated);
            m_indices.Recreated += new EventHandler(m_buffer_Recreated);
        }

        public void Delete() {
            if (VAO.HasValue) {
                int oldId = VAO.Value; 
                GL.DeleteVertexArrays(1,ref oldId);
                VAO = null;
            }
            if (m_indicesSorted != null && m_indicesSorted.GLID >= 0) {
                m_indicesSorted.Delete(); 
                m_indicesSorted = null; 
            }
        }
        public virtual void Draw(ILRenderParameter renderParams, Matrix4 transform) {
            #region buffer updates & transparency
            m_positions.PopulateChanges();
            m_colors.PopulateChanges();
            m_normals.PopulateChanges();
            int indicesCount = 0;
            if (VAO.HasValue == false || ILOGLProgram.CreateFor(this, renderParams)) // program has changed: need to rebuilt VAO
                Recreate(renderParams);

            GL.BindVertexArray(VAO.Value);
            uint[] indices = null; 
            if (Shape.IsTransparent || renderParams.Alpha.Peek() < 1) {
                if (m_indicesSorted == null || !m_indicesSorted.GLID.HasValue) {
                    m_indicesSorted = new ILOGLBuffer();
                }
                using (ILScope.Enter()) {
                    ILArray<int> sorted = ILHelper.SortIndices(Shape, transform);
                    m_indicesSorted.UpdateOrReplace(sorted);
                    indicesCount = sorted.S.NumberOfElements;
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_indicesSorted.GLID.Value);
                }
            } else if (m_indices.DataCount > 0 || m_indices.ChangeQueue.Count > 0) {
                m_indices.PopulateChanges();
                if (m_indicesSorted != null && m_indicesSorted.GLID.HasValue) {
                    m_indicesSorted.Delete();
                    m_indicesSorted = null;
                }
                indicesCount = Shape.Indices.DataCount;
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_indices.GLID.Value);
            } else {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }
                #endregion

            #region handle picking and special color modes
            ILOGLProgram myProgram;
            System.Drawing.Color myColor;
            if (renderParams.PickingContext != null) {
                myProgram = FlatProgram;
                myColor = renderParams.PickingContext.RegisterNextShape(Shape.PickingID);
            } else if (Shape.Marked) {
                myProgram = FlatProgram;
                myColor = System.Drawing.Color.Magenta;
            } else if (renderParams.ColorOverride.Peek().HasValue) {
                myProgram = FlatProgram;
                myColor = renderParams.ColorOverride.Peek().GetValueOrDefault();
            } else {
                myProgram = Program;
                myColor = this.Shape.Color ?? System.Drawing.Color.Empty;
            }
            #endregion

            #region setup program
            GL.UseProgram(myProgram.GLID);
            unsafe {
                if (myProgram.Model2CameraTransformMat4Loc >= 0) {
                    GL.UniformMatrix4(myProgram.Model2CameraTransformMat4Loc, 1, false, (float*)&transform);
                }
                if (myProgram.Model2Camera4NormalTransformMat3Loc >= 0) {
                    // the normals are transformed into world space as well
                    Matrix3 normalTransform = Matrix3.TransposeInvert(transform);
                    GL.UniformMatrix3(myProgram.Model2Camera4NormalTransformMat3Loc, 1, false, (float*)&normalTransform);
                }
                if (myProgram.ColorUniformLoc >= 0) {
                    GL.Uniform4(myProgram.ColorUniformLoc, myColor);
                }
                if (myProgram.AlphaMultiplyUniformLoc >= 0) {
                    GL.Uniform1(myProgram.AlphaMultiplyUniformLoc, renderParams.Alpha.Peek());
                }
                if (myProgram.SpecularUniformLocation >= 0) {
                    GL.Uniform4(myProgram.SpecularUniformLocation, Shape.SpecularColor);
                }
                if (myProgram.EmissionUniformLocation >= 0) {
                    GL.Uniform4(myProgram.EmissionUniformLocation, Shape.EmissionColor);
                }
                if (myProgram.ShininessUniformLocation >= 0) {
                    GL.Uniform1(myProgram.ShininessUniformLocation, Shape.Shininess);
                }

            }
            #endregion

            PreRender();

#if DEBUG
            unsafe {
                bool a;
                GL.GetBoolean(GetPName.PolygonOffsetFill, &a);
                System.Diagnostics.Debug.Assert(a);
            }

#endif
            #region drawing
            try {
                if (Shape.IsTransparent || (m_indices != null && m_indices.DataCount > 0) || renderParams.Alpha.Peek() < 1) {
                    GL.DrawElements(shape2BeginMode(Shape), indicesCount, DrawElementsType.UnsignedInt, (IntPtr)0);
                } else {
                    GL.DrawArrays(Primitive, 0, m_positions.DataCount);
                }
            } catch (Exception exc) {
                throw new Exception("Rendering failed at ILOGLSShape.Draw()", exc); 
            }
            #endregion

            PostRender();
            ErrorCode err = GL.GetError();
            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }
        protected virtual void PreRender() {
        }
        protected virtual void PostRender() {
            //GL.Disable(EnableCap.PolygonOffsetFill);
        }

        public unsafe void Recreate(ILRenderParameter parameter) {
            if (!VAO.HasValue) {
                int newId; 
                GL.GenVertexArrays(1,out newId); 
                VAO = newId; 
            }
            if (Shape != null) { // && m_positions.DataCount > 0) {
                
                System.Diagnostics.Trace.WriteLine(string.Format("Recreate Shape: {0} VAO:{1}, P({10}):{2} [{3}] I:{4} [{5}] C:{6} [{7}] N:{8} [{9}]", this, VAO, m_positions.SourceBuffer.BufferType, m_positions.DataCount, m_indices.SourceBuffer.BufferType,
                                                                                                                                                    m_indices.DataCount, m_colors.SourceBuffer.BufferType, m_colors.DataCount, m_normals.SourceBuffer.BufferType,
                                                                                                                                                    m_normals.DataCount, m_positions.GLID)); 
                System.Diagnostics.Debug.Assert(m_positions != null);
                System.Diagnostics.Debug.Assert(m_colors != null);
                System.Diagnostics.Debug.Assert(m_normals != null);
                System.Diagnostics.Debug.Assert(m_indices != null);
 
                //if (m_positions == null || m_positions.DataCount <= 0) {
                //    throw new InvalidOperationException("Invalid shape state detected. Empty position buffers are not allowed!"); 
                //}
                //System.Diagnostics.Debug.Assert(m_positions != null && m_positions.DataCount > 0 && m_positions.GLID.HasValue);
                // primitive type
                ILOGLProgram.CreateFor(this, parameter); 
                FlatProgram  = ILOGLProgram.Create(ILOGLShaderDefinition.VS_POS_COL_FLAT, ILOGLShaderDefinition.FS_COL_FLAT);
                
                // set up local VAO
                GL.BindVertexArray(VAO.Value);
                System.Diagnostics.Debug.Assert(Program.PositionsAttribLoc >= 0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, m_positions.GLID.Value);
                GL.EnableVertexAttribArray(Program.PositionsAttribLoc);
                GL.VertexAttribPointer(Program.PositionsAttribLoc, m_positions.SourceBuffer.DataLength, VertexAttribPointerType.Float, false, 0, 0);

                if (SmoothShading) {
                    System.Diagnostics.Debug.Assert(Program.ColorsAttribLoc >= 0);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, m_colors.GLID.Value);
                    GL.EnableVertexAttribArray(Program.ColorsAttribLoc);
                    GL.VertexAttribPointer(Program.ColorsAttribLoc, m_colors.SourceBuffer.DataLength, VertexAttribPointerType.Float, false, 0, 0);
                } else {
                    System.Diagnostics.Debug.Assert(Program.ColorUniformLoc >= 0);
                }
                if (m_normals.DataCount > 0) {
                    //System.Diagnostics.Debug.Assert(Program.NormalsAttribLoc >= 0); 
                    GL.BindBuffer(BufferTarget.ArrayBuffer, m_normals.GLID.Value);
                    GL.EnableVertexAttribArray(Program.NormalsAttribLoc);
                    GL.VertexAttribPointer(Program.NormalsAttribLoc, m_normals.SourceBuffer.DataLength, VertexAttribPointerType.Float, false, 0, 0);
                } 
                if (m_indices.DataCount > 0) {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, m_indices.GLID.Value);
                } else {
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                }
                GL.BindVertexArray(0);
            }
        }
        private ILOGLBuffer manageBuffer(ILBufferBase buffer, Dictionary<int, ILOGLBuffer> bufferMapping) {
            ILOGLBuffer ret;
            if (!bufferMapping.ContainsKey(buffer.ID)) {
                ret = new ILOGLBuffer(buffer);
                bufferMapping[buffer.ID] = ret;
                buffer.Disposing += (s,arg) => { 
                    ret.Delete(); 
                    bufferMapping.Remove((s as ILBufferBase).ID); 
                }; 
            } else {
                ret = bufferMapping[buffer.ID];
            }
            return ret;
        }
        void m_buffer_Recreated(object sender, EventArgs e) {
            // todo: remove old buffer from mapping, insert new buffer (currently not used in 3.0)
            //Recreate(null);
        }
        private BeginMode shape2BeginMode(ILShape shape) {
            Primitives shapeType = shape.Type; 
            switch (shapeType) {
                case Primitives.Points:
                    return BeginMode.Points; 
                case Primitives.Lines:
                    return BeginMode.Lines; 
                case Primitives.Triangles:
                    return BeginMode.Triangles; 
                case Primitives.TriangleFan:
                    if (shape.Buffers.TransparencyFlag)
                        return BeginMode.Triangles; 
                    return BeginMode.TriangleFan; 
                case Primitives.TriangleStrip:
                    if (shape.Buffers.TransparencyFlag)
                        return BeginMode.Triangles; 
                    return BeginMode.TriangleStrip; 
                case Primitives.LineStrip:
                    if (shape.Buffers.TransparencyFlag)
                        return BeginMode.Lines; 
                    return BeginMode.LineStrip; 
                default:
                    throw new InvalidProgramException(); 
            }
        }
        public void GetShaderNames(ILRenderParameter parameter, out string vertexShaderName, out string fragmentShaderName) {
            // render parameter currently not used (3.0)

            if (m_normals == null || m_normals.DataCount < 1) {
                if (!Shape.Color.HasValue && m_colors != null && m_colors.DataCount > 0) {
                    vertexShaderName = ILOGLShaderDefinition.VS_POS_COLS_SMOOTH;
                    fragmentShaderName = ILOGLShaderDefinition.FS_COL_SMOOTH;
                } else {
                    vertexShaderName = ILOGLShaderDefinition.VS_POS_COL_FLAT;
                    fragmentShaderName = ILOGLShaderDefinition.FS_COL_FLAT;
                }
            } else {
                if (SmoothShading) {
                    vertexShaderName = ILOGLShaderDefinition.VS_POS_COLS_SMOOTH_LIGHT;
                    fragmentShaderName = ILOGLShaderDefinition.FS_COL_SMOOTH_LIGHT;
                } else {
                    vertexShaderName = ILOGLShaderDefinition.VS_POS_COL_FLAT_LIGHT;
                    fragmentShaderName = ILOGLShaderDefinition.FS_COL_FLAT_LIGHT;
                }
            }
        }

    }
}
