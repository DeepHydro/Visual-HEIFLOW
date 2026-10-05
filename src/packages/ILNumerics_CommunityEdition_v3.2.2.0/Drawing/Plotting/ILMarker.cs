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
using System.Xml.Serialization;

namespace ILNumerics.Drawing.Plotting {
    [Serializable]
    public class ILMarker : ILGroup {

        #region attributes
        public static string DefaultFillTag = "MarkerFill";
        public static string DefaultBorderTag = "MarkerBorder"; 
        private MarkerStyle m_style; 
        private Vector4 m_logState; 
        int m_size; 
        Vector3? m_position; 
        //private ILArray<float> m_clipPositions = ILMath.localMember<float>(); 
        ILTriangles m_fill; 
        ILLines m_border; 

        #endregion
       
        #region properties
        /// <summary>
        /// Get/set the size of the marker in pixels
        /// </summary>
        [XmlAttribute]
        public int Size {
            get { return m_size; }
            set {
                if (m_size != value) {
                    m_size = value;
                    OnPropertyChanged("Size");
                }
            }
        }
        /// <summary>
        /// Get/set the position of the marker in world coords
        /// </summary>
        [XmlAttribute]
        public Vector3? Position {
            get { return m_position; }
            set {
                if (m_position != value) {
                    m_position = value;
                    OnPropertyChanged("Position");
                }
            }
        }
        /// <summary>
        /// Get a reference to the triangles shape which represents the inner marker area
        /// </summary>
        [XmlIgnore]
        public ILTriangles Fill {
            get { return m_fill; }
            private set {
                m_fill = value; 
            }
        }
        /// <summary>
        /// Gets a reference to the line shape which represents the marker border
        /// </summary>
        [XmlIgnore]
        public ILLines Border {
            get { return m_border; }
            private set { m_border = value; }
        }
        /// <summary>
        /// Gets the style of the marker or sets it.
        /// </summary>
        [XmlAttribute]
        public MarkerStyle Style {
            get { return m_style; }
            set {
                if (m_style != value) {
                    m_style = value;
                    if (Fill != null) {
                        Setup(Fill.Color);
                    } else {
                        Setup(null); 
                    }
                    OnPropertyChanged("Style"); 
                }
            }
        }
        #endregion

        #region constructor
        private ILMarker() { }
        public ILMarker(MarkerStyle style = MarkerStyle.Dot, Color? color = null, object tag = null, Vector3? position = null) : base(tag) {
            Size = 9; 
            m_style = style;
            Position = position; 
            //Fill = Add(new ILTriangles(DefaultFillTag) { AutoNormals = false, Color = Color.Blue });
            //Fill.Colors = new ILColorsBuffer(); 
            //Border = Add(new ILLines(DefaultBorderTag) { Color = Color.DarkGray }); 
            Tag = tag ?? style.ToString() + "Marker"; 
            Target = RenderTarget.Screen2DFar; 
            Setup(color); 
        }
        public ILMarker(ILMarker source) : base(source) {
            Position = source.Position; 
            m_style = source.Style; 
            Size = source.Size;
            Fill = First<ILTriangles>(source.Fill.Tag);
            Border = First<ILLines>(source.Border.Tag); 
        }
        #endregion

        protected override bool BeginVisit(ILRenderParameter parameter) {
            if (Style == MarkerStyle.None) return false; 
            // disable log rendering
            m_logState = parameter.LogState.Peek();
            parameter.LogState.Push(new Vector4(0,0,0,0));
           
            var ret = base.BeginVisit(parameter);
            // if individual clipping was defined on this marker, it has been "transformed" to 
            // camera coords in the base method. Since we use our own 
            // coordinate systems that clipping is useless and needs to
            // get reverted here
            if (Clipping != null) {
                parameter.PopClipping();
                parameter.PushClipping(Clipping, true);
            } else if (parameter.PeekClipping() != null) {
                // if we inherit the clipping from parent nodes, it must be converted to
                // our local marker coordinate system
                ILClipParams clip = parameter.PeekClipping();
                ILClipParams markClip = new ILClipParams();
                Matrix4 invTransp = Matrix4.Transpose(Matrix4.Invert(parameter.ProjectionTransform));
                for (int i = 0; i < 6; i++) {
                    var v = invTransp * clip[i];
                    markClip[i] = v;
                }
                parameter.PushClipping(markClip, true); 
            }

            //parameter.Pop(); 
            //parameter.PushNew(Transform);
            return ret; 

        }

        protected override void VisitInternal(ILRenderParameter parameter) {
            if (!Position.HasValue && m_style != MarkerStyle.None) {
                // find responsible position provider, typically the direct parent (e.g. line plots) 
                ILGroup positionProv = Parent;
                while (positionProv != null && !(positionProv is IILPositionProvider)) {
                    positionProv = positionProv.Parent;
                }
                if (positionProv != null) {
                    ILPositionsBuffer positions = (positionProv as IILPositionProvider).Positions;
                    using (ILScope.Enter()) {
                        var vt = parameter.ViewTransform.ToViewRectangle(); 
                        ILArray<float> clip_pos = parameter.ToClipWithDivide(positions.Storage); 
                        ILIndicesBuffer indices = (positionProv as IILPositionProvider).Indices;
                        
                        // markers group is rendered in orthographic transform 
                        parameter.PushProjectionTransform(Matrix4.Identity); 

                        Matrix4 mat = Matrix4.ScaleTransform(1f / vt.Width * this.Size, 1f / vt.Height * this.Size, 1); 
                        if (indices != null && !indices.IsEmpty) {
                            // indexed
                            for (int i = 0; i < indices.DataCount; i++) {
                                int ind = indices.GetIndexAt(i);
                                Vector3 transl = clip_pos.GetPositionAt(ind); 
                                Matrix4 modelMat = mat.Translate(transl.X,transl.Y,transl.Z);

                                parameter.PushNew(modelMat);
                                base.VisitInternal(parameter);
                                parameter.Pop();
                            }
                        } else {
                            // non indexed
                            System.Diagnostics.Debug.Assert(positions != null);
                            for (int i = 0; i < positions.DataCount; i++) {
                                Vector3 transl = clip_pos.GetPositionAt(i);
                                Matrix4 modelMat = mat.Translate(transl.X, transl.Y, transl.Z);

                                parameter.PushNew(modelMat);
                                base.VisitInternal(parameter);
                                parameter.Pop();
                            }
                        }

                    }
                }
            } else {
                // markers group is rendered in orthographic transform 
                parameter.PushProjectionTransform(Matrix4.Identity); 
                base.VisitInternal(parameter);
            }
            parameter.PopProjectionTransform(); 
        }

        protected override void EndVisit(ILRenderParameter parameter) {

            if (parameter.PeekClipping() != null && Clipping == null) 
                parameter.PopClipping();
            base.EndVisit(parameter);
            parameter.LogState.Pop(); 
        }
        protected override void getLimitsInternal(Stack<Matrix4> transforms, ILLimits ret, bool ignoreRootTransform = true, Vector3? lowerBound = null) {
            // markers should not contribute to data limits 
            return; 
        }
        //protected override void getLogLimitsInternal(Stack<Matrix4> transforms, ref Vector3 min, ref Vector3 max) {
        //    // markers should not contribute to data limits 
        //    return;
        //}
        private void CalculateTransform(ILRenderParameter parameter, Vector3 position) {
            Matrix4 rot = parameter.CurrentModel2CameraTransform * (m_logState.W != 0 ? 
                        Matrix4.Translation(
                            m_logState.X != 0 ? Math.Log10(position.X) : position.X,
                            m_logState.Y != 0 ? Math.Log10(position.Y) : position.Y,
                           (m_logState.Z != 0 ? Math.Log10(position.Z) : position.Z)) :
                        Matrix4.Translation(position.X,
                                            position.Y,
                                            position.Z)); 
            rot.M11 = 1; rot.M12 = 0; rot.M13 = 0; 
            rot.M21 = 0; rot.M22 = 1; rot.M23 = 0; 
            rot.M31 = 0; rot.M32 = 0; rot.M33 = 1;
            //rot.M34 = 0; // place Markers at screen's Z = 0

            Transform = rot 
                        * Matrix4.ScaleTransform(
                            Size / (float)(parameter.Driver.Size.Width - 1) / parameter.ViewportScaleFactor.Width,
                            Size / (float)(parameter.Driver.Size.Height - 1) / parameter.ViewportScaleFactor.Height, 
                            1);     
        } 

        public void Setup(Color? color) {
            using (ILScope.Enter()) {
                if (Fill == null) {
                    Fill = Add(new ILTrianglesFan(DefaultFillTag)); 
                }
                if (Border == null) {
                    Border = Add(new ILLineStrip(DefaultBorderTag)); 
                }
                switch (Style) {
                    case MarkerStyle.Circle:
                    case MarkerStyle.Dot:
                        ILCircle circle = new ILCircle(15); 
                        circle.Fill.AutoNormals = false;
                        Fill.Buffers = circle.Fill.Buffers;
                        Fill.Colors = new ILColorsBuffer();
                        Border.Buffers = circle.Border.Buffers;
                        break;
                    case MarkerStyle.Diamond:
                        Fill.Positions.Update(new float[,] { { 0, 0, 0 }, { 0, 1, 0 }, { 0.7f, 0, 0 }, { 0, -1, 0 }, { -0.7f, 0, 0 }, { 0, 1, 0 } });
                        Fill.Colors = new ILColorsBuffer();
                        Border.Positions = Fill.Positions;
                        Border.Indices.Update(new int[] { 1, 2, 3, 4, 1 });
                        break;
                    case MarkerStyle.Square:
                        Fill.Positions.Update(new float[,] { { 1, 1, 0 }, { 1, -1, 0 }, { -1, -1, 0 }, { -1, 1, 0 }, { 1, 1, 0 } });
                        Fill.Colors = new ILColorsBuffer();
                        Border.Positions = Fill.Positions;
                        Border.Indices.Update(new int[] { 0, 1, 2, 3, 0 });
                        break;
                    case MarkerStyle.Rectangle:
                        Fill.Positions.Update(new float[,] { { 1, 0.6f, 0 }, { 1, -0.6f, 0 }, { -1, -0.6f, 0 }, { -1, 0.6f, 0 }, { 1, 0.6f, 0 } });
                        Fill.Colors = new ILColorsBuffer();
                        Border.Positions = Fill.Positions;
                        Border.Indices.Update(new int[] { 0, 1, 2, 3, 0 });
                        break;
                    case MarkerStyle.TriangleUp:
                        Fill.Positions.Update(new float[,] { { 0, 1, 0 }, { 0.7f, -0.7f, 0 }, { -0.7f, -0.7f, 0 }, { 0, 1, 0 } });
                        Fill.Colors = new ILColorsBuffer();
                        Border.Positions = Fill.Positions;
                        Border.Indices.Update(new int[] { 0, 1, 2, 0 });
                        break;
                    case MarkerStyle.TriangleDown:
                        Fill.Positions.Update(new float[,] { { 0, -1, 0 }, { -0.7f, 0.7f, 0 }, { 0.7f, 0.7f, 0 }, { 0, -1, 0 } });
                        Fill.Colors = new ILColorsBuffer();
                        Border.Positions = Fill.Positions;
                        Border.Indices.Update(new int[] { 0, 1, 2, 0 });
                        break;
                    case MarkerStyle.TriangleLeft:
                        Fill.Positions.Update(new float[,] { { 0.7f, 0.7f, 0 }, { 0.7f, -0.7f, 0 }, { -1, 0, 0 }, { 0.7f, 0.7f, 0 } });
                        Fill.Colors = new ILColorsBuffer();
                        Border.Positions = Fill.Positions;
                        Border.Indices.Update(new int[] { 0, 1, 2, 0 });
                        break;
                    case MarkerStyle.TriangleRight:
                        Fill.Positions.Update(new float[,] { { -0.7f, 0.7f, 0 }, { 1, 0, 0 }, { -0.7f, -0.7f, 0 }, { -0.7f, 0.7f, 0 } });
                        Fill.Colors = new ILColorsBuffer();
                        Border.Positions = Fill.Positions;
                        Border.Indices.Update(new int[] { 0, 1, 2, 0 });
                        break;
                    case MarkerStyle.Plus:
                        Fill.Positions.Update(new float[,] { { 0, 0, 0 }, { 0.3f, 0.3f, 0 }, { 1, 0.3f, 0 }, { 1, -0.3f, 0 }, { 0.3f, -0.3f, 0 },
                                                             { 0.3f,-1, 0 }, { -0.3f, -1, 0 }, { -0.3f, -0.3f, 0 }, { -1, -0.3f, 0 }, 
                                                             { -1, 0.3f, 0 },{ -.3f, 0.3f, 0 },{ -.3f, 1, 0 },{ .3f, 1, 0 },{ .3f, 0.3f, 0 }});
                        Fill.Colors = new ILColorsBuffer();
                        Border.Positions = Fill.Positions;
                        Border.Indices.Update(new int[] { 1, 2, 3,4,5,6,7,8,9,10,11,12,13,1 });
                        break;
                    case MarkerStyle.Cross:
                        float z = 2/3f, e = 1/3f; 
                        Fill.Positions.Update(new float[,] { { 0, 0, 0 }, 
                                                             { e, 0, 0 },
                                                             { 1, -z, 0 },
                                                             { z, -1, 0 },
                                                             { 0, -e, 0 },
                                                             { -z, -1, 0 },
                                                             { -1, -z, 0 },
                                                             { -e, 0, 0 },
                                                             { -1, z, 0 },
                                                             { -z, 1, 0 },
                                                             { 0, e, 0 },
                                                             { z, 1, 0 },
                                                             { 1, z, 0 },
                                                             { e, 0, 0 }
                        });
                        Fill.Colors = new ILColorsBuffer();
                        Border.Positions = Fill.Positions;
                        Border.Indices.Update(new int[] { 1, 2, 3,4,5,6,7,8,9,10,11,12,1 });
                        break;
                    default: // custom
                        break;
                }
                Fill.AutoNormals = false; 
                Fill.Normals.Update(null); 
                if (Style == MarkerStyle.Circle)
                    Fill.Color = Color.Empty; 
                else
                    Fill.Color = color ?? Color.DarkGray; 
                if (Tag == null) {
                    Tag = Style.ToString() + "Marker"; 
                }
            }
            base.Configure(); // configure line and markers
        }

        internal override ILNode Copy() {
            return new ILMarker(this); 
        }

        protected internal override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILMarker ret = (ILMarker)base.Synchronize(copy, syncParams);
            if (copy == null) {
                ret.Fill = ret.FindById < ILTriangles>(Fill.ID);
                ret.Border = ret.FindById < ILLines>(Border.ID);
            }
            if (ret.SynchedVersion != Version) {
                ret.Size = Size;
                ret.Position = Position;
                ret.m_style = m_style;
            }
            return ret; 
        }

        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILMarker(); 
        }

    }
}
