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
using System.Drawing;
using ILNumerics.Exceptions; 
using ILNumerics.Data;
using System.Xml.Serialization; 

namespace ILNumerics.Drawing {
    [Serializable]
    public abstract class ILShape : ILDrawable, IDisposable {

        #region attributes 
        protected ILBufferSet m_buffers;
        private bool m_autoNormals; 
        Primitives m_type; 
        private bool m_isSelected;
        private bool m_selectable = true; 
        private ILSelectionBox m_selectionBox;
        private bool m_isDisposed; 
        #endregion

        #region IILShape Properties
        [XmlAttribute]
        public bool Selectable {
            get { return m_selectable; }
            set {
                if (m_selectable != value) {
                    m_selectable = value;
                    OnPropertyChanged("Selectable"); 
                }
            }
        }
        [XmlIgnore]
        public ILSelectionBox SelectionBox {
            get {
                if (m_selectionBox == null) {
                    m_selectionBox = new ILSelectionBox(this);
                }
                return m_selectionBox;
            }
            set {
                if (m_selectionBox != value) {
                    m_selectionBox = value;
                    OnPropertyChanged("SelectionBox");
                }
            }
        }
        [XmlIgnore]
        public bool IsDisposed {
            get { return m_isDisposed; }
            protected set {
                if (m_isDisposed != value) {
                    m_isDisposed = value; 
                    OnPropertyChanged("IsDisposed"); 
                }
            }
        }
        /// <summary>
        /// Get the complete set of buffers for the shape or sets it
        /// </summary>
        /// <remarks>Buffer sets contain any buffers for the shape: vertex and index buffers. 
        /// A buffer set can be shared among multiple shapes in the same way as individual buffers can. Reference 
        /// counting keeps track of the instances and memory management transparently.</remarks>
        public ILBufferSet Buffers {
            get { return m_buffers; }
            set {
                if (!ReferenceEquals(m_buffers, value)) {
                    lock (StructureLock) {
                        ILBufferSet old = m_buffers;
                        m_buffers = value;
                        if (m_buffers != null) {
                            m_buffers.IncreaseReference();
                        }
                        if (old != null) {
                            old.DecreaseReference();
                        }
                    }
                    OnPropertyChanged("Buffers");
                }
            }
        }
        /// <summary>
        /// Get the number of vertices every basic primitive type in the shape is composed of
        /// </summary>
        [XmlAttribute]
        public abstract int VerticesPerPrimitive { get; }
        /// <summary>
        /// Get the buffer with individual colors for the shape or sets it
        /// </summary>
        [XmlIgnore]
        public ILColorsBuffer Colors {
            get {
                lock (StructureLock)
                    return m_buffers.Colors;
            }
            set {
                if (object.Equals(value, null)) {
                    throw new ILArgumentException("null is not allowed as value for Colors");
                }
                if (!ReferenceEquals(m_buffers.Colors, value)) {
                    lock (StructureLock) {
                        m_buffers.Colors = value;
                    }
                    OnPropertyChanged("Colors");
                }
            }
        }
        /// <summary>
        /// Get the specular color if the shape is lit or sets it
        /// </summary>
        [XmlAttribute]
        [ILXmlSerializeAs("{R},{G},{B},{A}")]
        public Color SpecularColor {
            get { return m_buffers.Material.Specular; }
            set {
                lock (StructureLock) {
                    if (m_buffers.Material.Specular != value) {
                        m_buffers.Material.Specular = value;
                        OnPropertyChanged("SpecularColor");
                    }
                }
            }
        }
        /// Get the emmisive color if the shape is lit or sets it
        [XmlAttribute]
        [ILXmlSerializeAs("{R},{G},{B},{A}")]
        public Color EmissionColor {
            get { return m_buffers.Material.Emission; }
            set {
                lock (StructureLock) {
                    if (m_buffers.Material.Emission != value) {
                        m_buffers.Material.Emission = value;
                        OnPropertyChanged("EmissionColor");
                    }
                }
            }
        }
        /// Get the shininess factor (basically the size of the shiny reflection) if the shape is lit or sets it
        [XmlAttribute]
        public float Shininess {
            get { return m_buffers.Material.Shininess; }
            set {
                lock (StructureLock) {
                    if (m_buffers.Material.Shininess != value) {
                        m_buffers.Material.Shininess = value;
                        OnPropertyChanged("Shininess");
                    }
                }
            }
        }
        /// Specifies if the normal vectors for lighting are to be computed automatically; default: true
        [XmlAttribute]
        public bool AutoNormals { 
            get { return m_autoNormals; }
            set {
                if (m_autoNormals != value) {
                    m_autoNormals = value; 
                    OnPropertyChanged("AutoNormals");
                }
            }
        }
        /// <summary>
        /// Get the buffer with vertex positions or sets it
        /// </summary>
        [XmlIgnore]
        public ILPositionsBuffer Positions {
            get {
                lock (StructureLock) 
                    return m_buffers.Positions;
            }
            set {
                if (object.Equals(value, null)) {
                    throw new ILArgumentException("null is not allowed as value for Positions");
                }
                if (!ReferenceEquals(m_buffers.Positions, value)) {
                    lock (StructureLock) {
                        m_buffers.Positions = value;
                    }
                    OnPropertyChanged("Positions");
                }
            }
        }
        /// <summary>
        /// Get the buffer with vertex normals or sets it
        /// </summary>
        [XmlIgnore]
        public virtual ILNormalsBuffer Normals {
            get {
                lock (StructureLock)
                    return m_buffers.Normals; 
            }
            set {
                if (object.Equals(value, null)) {
                    throw new ILArgumentException("null is not allowed as value for Normals");
                }
                if (!ReferenceEquals(m_buffers.Normals, value)) {
                    lock (StructureLock) {
                        // we handle changes to normals by switching AutoNormals off automatically
                        if (m_buffers.Normals != null)
                            m_buffers.Normals.Changed -= new EventHandler<ILBufferChangedEventArgs>(Normals_Changed);
                        m_buffers.Normals = value;
                        m_buffers.Normals.Changed += new EventHandler<ILBufferChangedEventArgs>(Normals_Changed);
                    }
                    OnPropertyChanged("Normals");
                }
            }
        }
        /// <summary>
        /// Get the buffer with indices composing basic primitives or sets it
        /// </summary>
        [XmlIgnore]
        public ILIndicesBuffer Indices {
            get {
                lock (StructureLock)
                    return m_buffers.Indices; 
            }
            set {
                if (object.Equals(value, null)) {
                    throw new ILArgumentException("null is not allowed as value for Indices"); 
                }
                if (!ReferenceEquals(m_buffers.Indices, value)) {
                    lock (StructureLock) {
                        m_buffers.Indices = value;
                    }
                    OnPropertyChanged("Indices");
                }
            }
        }
        /// <summary>
        /// Get the primitive type of this shape
        /// </summary>
        [XmlAttribute]
        public Primitives Type {
            get { return m_type; }
            internal set { 
                if (m_type != value) 
                    m_type = value; 
            }
        }
        /// <summary>
        /// Get the bounding box limits for this shape
        /// </summary>
        [XmlIgnore]
        public ILLimits Limits {
            get {
                return m_buffers.Limits;
            }
        }
        /// <summary>
        /// Determines if this shape is currently in selected state
        /// </summary>
        [XmlAttribute]
        public bool Selected {
            get { return m_isSelected; }
            set {
                if (m_selectable && m_isSelected != value) {
                    m_isSelected = value;
                    if (m_isSelected) {
                        Parent.Add(SelectionBox);
                        SelectionBox.Visible = true;
                        //SelectionBox.Configure(); 
                    } else {
                        Parent.Remove(SelectionBox);
                        SelectionBox = null; 
                        SelectionBox.Visible = false;
                    }
                    OnPropertyChanged("Selected");
                }
            }
        }
        /// <summary>
        /// after configuration: determine transparency state for this shape
        /// </summary>
        [XmlIgnore]
        internal override bool IsTransparent {
            get {
                if (Color.HasValue) {
                    return Color.Value.A < 255;
                } else {
                    return Buffers.TransparencyFlag;
                }
            }
        }
        #endregion

        #region constructor
        protected ILShape(object tag = null)
            : base(tag) {
            IsDisposed = false;
            Buffers = new ILBufferSet();
            AutoNormals = true; 
        }
        internal ILShape(ILShape source)
            : base(source) {
            Buffers = source.Buffers; 
            AutoNormals = source.AutoNormals; 
        }
        #endregion

        #region public interface
        protected override void VisitInternal(ILRenderParameter parameter) {
            base.VisitInternal(parameter);
            parameter.Driver.VisitNode(this, parameter);
        }
        /// <summary>
        /// Configures the shape; must be called after changes to any vertex buffers 
        /// </summary>
        /// <param name="configureChildren">[optional] configure subtree; default: true</param>
        /// <param name="configurePath2Root">[optional] configure nodes on the path to the root node; default: true</param>
        /// <remarks>Configure must be called to populate changes to the driver. If you can be sure, changes will not effect 
        /// any other shapes, set <paramref name="configureChildren"/> and/or <paramref name="configurePath2Root"/> to true. This 
        /// limits the configuration to this shape and may gives better performance.</remarks>
        public override void Configure(bool configureChildren = true, bool configurePath2Root = true) {
            if (IsDisposed) return;
            Buffers.Configure();
            if (configurePath2Root) {
                ILNode cur = this.Parent;
                while (cur != null) {
                    cur.Configure(false, true);
                    cur = cur.Parent;
                }
            }
        }
        internal protected override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            ILShape copyShape = (ILShape)base.Synchronize(copy, syncParams);
            if (!syncParams.BufferSets.ContainsKey(Buffers)) {
                syncParams.BufferSets.Add(Buffers, copyShape.Buffers);
            } else {
                // this handles all reference management etc.:
                copyShape.Buffers = syncParams.BufferSets[Buffers];
            } 
            Buffers.Synchronize(copyShape.Buffers, syncParams);
            if (copy == null) {
                copyShape.Type = Type; 
            }
            // we do not populate the selection state to the rendering tree. Selection are handled completely on the source tree level.
            //copyShape.Selected = Selected; 
            return copyShape;
        }
        /// <summary>
        /// Dispose off this shape, release any references to buffer resources
        /// </summary>
        public override void Dispose() {
            lock (StructureLock) {
                if (!IsDisposed) {
                    base.Dispose();  // triggers Disposing event
                    IsDisposed = true; 
                    if (m_buffers != null && !m_buffers.IsDisposed) {
                        m_buffers.DecreaseReference();
                        m_buffers = null; 
                    }
                }
            }
        }
        /// <summary>
        /// Detach this shapes buffers from any shared buffers and allows individual changes
        /// </summary>
        /// <remarks>This function allows a shape to act completely independent of any other shapes in a scene. 
        /// After calling Detach() changes to local buffers are save to only effect the appearance of this shape. 
        /// Therefore, any buffers which have been shared will detach from their source and use up individual storage. 
        /// However, this new storage is not immediately allocated but only, once changes really arrive ("lazy").</remarks>
        public override ILNode Detach() {
            lock (StructureLock) {
                Buffers = Buffers.Copy();
            }
            return this; 
        }
        /// <summary>
        /// Get midpoint of the vertex positions
        /// </summary>
        /// <returns></returns>
        public override Vector3 GetPosition() {
            return Buffers.Limits.CenterF; 
        }
        /// <summary>
        /// Default mouse click handler, toogles the selection state
        /// </summary>
        /// <param name="args">ILNumerics mouse event arguments</param>
        /// <returns>true if the event should get handled further</returns>
        protected internal override void OnMouseClick(ILMouseEventArgs args) {
            base.OnMouseClick(args); 
            if (args.Cancel) return; 
            Selected = !Selected;
            args.Refresh = true; 
        }
        /// <summary>
        /// Get the number of basic primitives which are configured for this shape 
        /// </summary>
        /// <returns>number of basic primitives</returns>
        /// <remarks>The number of basic primitives returned depend on several factors, like 
        /// <list type="bullet">
        /// <item>The existence of an index buffer and its size</item>
        /// <item>The shape type (lines, line strip, triangles, -strip or -fan, etc.)</item>
        /// </list>
        /// This function returns the number of basic primitives <strong>defined</strong> - regardless of
        /// how many of them will later really be drawn. Hence, clipping is not considered here.
        /// </remarks>
        public virtual int GetPrimitiveCount() {
            if (Indices != null && !Indices.IsEmpty) {
                return Indices.Storage.S.NumberOfElements / VerticesPerPrimitive; 
            } else {
                return Buffers.Positions.DataCount / VerticesPerPrimitive; 
            }
        }
        #endregion

        #region private helper 
        void Normals_Changed(object sender, ILBufferChangedEventArgs e) {
            AutoNormals = false;
        }

        /// <summary>
        /// create indices matrix for individually sortable primitives
        /// </summary>
        /// <returns>discrete indices matrix/element (indices) buffer storage</returns>
        internal virtual ILRetArray<int> GetIndicesForSorting(ILInArray<int> sourceIndices) {
            using (ILScope.Enter(sourceIndices)) {
                ILArray<int> indices = sourceIndices;
                if (ILMath.isnull(indices) || indices.IsEmpty) {
                    int vertices2Take = Positions.DataCount / VerticesPerPrimitive * VerticesPerPrimitive;
                    indices.a = ILMath.counter<int>(0, 1, ILMath.size(1, vertices2Take));
                }
                ILArray<int> ret = ILMath.reshape(indices, VerticesPerPrimitive, indices.Length / VerticesPerPrimitive);
                return ret;
            }
        }
        protected void StartPipeline(ILRenderParameter parameters, SortingMode sorting, bool frustumClipping,
                        ILOutArray<float> positions_camera, ILOutArray<int> indices, ILOutArray<float> positions_screen,
                        ILOutArray<float> normals_camera) {
            using (ILScope.Enter()) {
                if (parameters.LogState.Peek().W != 0) {
                    positions_camera.a = Positions.Storage;
                    if (parameters.LogState.Peek().X != 0) positions_camera["0;:"] = ILMath.log10(Positions.Storage["0;:"]);
                    if (parameters.LogState.Peek().Y != 0) positions_camera["1;:"] = ILMath.log10(Positions.Storage["1;:"]);
                    if (parameters.LogState.Peek().Z != 0) positions_camera["2;:"] = ILMath.log10(Positions.Storage["2;:"]);
                    positions_camera.a = parameters.CurrentModel2CameraTransform * positions_camera;    // ProcessVertices(parameters, indices, positions_camera);
                } else {
                    positions_camera.a = parameters.CurrentModel2CameraTransform * Positions.Storage;    // ProcessVertices(parameters, indices, positions_camera);
                }
                #region sorting, indices
                indices.a = ILMath.empty<int>();
                if (sorting == SortingMode.None) {
                    if (this is ILTrianglesFan || this is ILTrianglesStrip || this is ILLineStrip) {
                        indices.a = GetIndicesForSorting(Indices.Storage);
                    } else if (!Indices.IsEmpty) {
                        indices.a = Indices.Storage;
                    }
                } else {
                    indices.a = ILShape.Computation.GetSortedIndices(positions_camera, GetIndicesForSorting(Indices.Storage), VerticesPerPrimitive);
                }
                #endregion
                positions_screen.a = Matrix4.ViewTransformWithPerspDivide(parameters.ViewTransform, parameters.ProjectionTransform * positions_camera);
                Matrix3 normalsCameraTransform = Matrix3.TransposeInvert(parameters.CurrentModel2CameraTransform);
                normals_camera.a = normalsCameraTransform * Normals.Storage;
            }
        }

        [System.Security.SecuritySafeCritical]
        protected class Computation : ILMath {
            public static ILRetArray<int> GetSortedIndices(ILInArray<float> positions, ILInArray<int> indices, int verticesPerPrimitive) {
                using (ILScope.Enter(positions, indices)) {
                    // sort primitives
                    // expects positions to be already in NDC space! 
                    ILArray<int> locIndices = check(indices); 
                    if (indices.IsEmpty) {
                        locIndices.a = counter<int>(0, 1, size(verticesPerPrimitive, positions.S[1] / verticesPerPrimitive)); 
                    }
                    ILArray<int> outIndices = empty<int>();
                    sort(sum(positions[2, full][locIndices], 0), outIndices, descending: false).Dispose();
                    return locIndices[full, outIndices];
                }
            }
        }
        #endregion

        #region IILBSPShape Members
//        internal override void ToBSPBuildPrimitives<T>(IList<BSPPrimitive<T>> primitives, Matrix4 model2CameraTransform, Matrix4 camera2ClipTransform, T data) {
//            using (ILScope.Enter()) {
//                ILArray<float> pos_clip = (camera2ClipTransform * model2CameraTransform) * Positions.Storage;
//                ILArray<float> posCam = model2CameraTransform * Positions.Storage;
//                // perspective divide
//                pos_clip.a = pos_clip / pos_clip["end;:"]; 

//                ILArray<float> nor = 1;
//                if (!Normals.IsEmpty) {
//                    nor = Matrix3.TransposeInvert(model2CameraTransform) * Normals.Storage; 
//                }
//                #region indexed
//                ILArray<int> indices = GetIndicesForSorting(Indices.Storage);
//                for (int i = 0; i < indices.S[1]; i++) {
//                    var bspprim = new BSPPrimitive<T>();
//                    bspprim.Data = data; 
//                    // clip positions
//                    bspprim.Positions = new Vector3[VerticesPerPrimitive];
//                    for (int p = 0; p < VerticesPerPrimitive; p++) {
//                        bspprim.Positions[p] = pos_clip.GetPositionAt(indices.GetValue(i * VerticesPerPrimitive + p));
//                    }
//                    bspprim.CameraPos = new Vector3[VerticesPerPrimitive];
//                    for (int p = 0; p < VerticesPerPrimitive; p++) {
//                        bspprim.CameraPos[p] = posCam.GetPositionAt(indices.GetValue(i * VerticesPerPrimitive + p));
//                    }
//                    if (!Colors.IsEmpty) {
//                        bspprim.Colors = new Vector4[VerticesPerPrimitive];
//                        for (int p = 0; p < VerticesPerPrimitive; p++) {
//                            bspprim.Colors[p] = Colors.GetVector4At(indices.GetValue(i * VerticesPerPrimitive + p));
//                        }
//                    }
//                    if (!Normals.IsEmpty) {
//                        bspprim.Normals = new Vector3[VerticesPerPrimitive];
//                        for (int p = 0; p < VerticesPerPrimitive; p++) {
//                            bspprim.Normals[p] = nor.GetPositionAt(indices.GetValue(i * VerticesPerPrimitive + p));
//                        }
//                    }
//#if DEBUG
//                    bspprim.ID = i; 
//#endif
//                    primitives.Add(bspprim);
//                }
//                #endregion
//            }
//        }
//        internal override void FromBSPBuildPrimitives(IEnumerable<BSPPrimitive<ILDrawable>> primitives) {
//            using (ILScope.Enter()) {
//                ILArray<float> pos = ILMath.zeros<float>(3, primitives.Count() * VerticesPerPrimitive);
//                ILArray<float> colors = 0;
//                ILArray<float> normals = 0;
//                if (!Colors.IsEmpty) {
//                    colors = ILMath.zeros<float>(4, primitives.Count() * VerticesPerPrimitive);
//                }
//                if (!Normals.IsEmpty) {
//                    normals = ILMath.zeros<float>(3, primitives.Count() * VerticesPerPrimitive);
//                }
//                int curID = 0;
//                foreach (var prim in primitives) {
//                    System.Diagnostics.Debug.Assert(prim.VertexCount == VerticesPerPrimitive);
//                    for (int v = 0; v < VerticesPerPrimitive; v++) {
//                        pos.SetValue(prim.Positions[v].X, 0, curID + v);
//                        pos.SetValue(prim.Positions[v].Y, 1, curID + v);
//                        pos.SetValue(prim.Positions[v].Z, 2, curID + v);
//                    }
//                    if (prim.Colors != null) {
//                        System.Diagnostics.Debug.Assert(!Colors.IsEmpty);
//                        for (int v = 0; v < VerticesPerPrimitive; v++) {
//                            colors.SetValue(prim.Colors[v].X, 0, curID + v);
//                            colors.SetValue(prim.Colors[v].Y, 1, curID + v);
//                            colors.SetValue(prim.Colors[v].Z, 2, curID + v);
//                            colors.SetValue(prim.Colors[v].W, 3, curID + v);
//                        }
//                    }
//                    if (prim.Normals != null) {
//                        System.Diagnostics.Debug.Assert(!Normals.IsEmpty);
//                        for (int v = 0; v < VerticesPerPrimitive; v++) {
//                            normals.SetValue(prim.Normals[v].X, 0, curID + v);
//                            normals.SetValue(prim.Normals[v].Y, 1, curID + v);
//                            normals.SetValue(prim.Normals[v].Z, 2, curID + v);
//                        }
//                    }
//                    curID++; 
//                }
//                Positions.Update(pos);
//                if (!Colors.IsEmpty) {
//                    Colors.Update(colors);
//                }
//                if (!Normals.IsEmpty) {
//                    Normals.Update(normals);
//                }
//                Indices.Update(null); 
//                // fix the shape type (we are not optimized anymore)
//                if (Type == Primitives.LineStrip) {
//                    Type = Primitives.Lines; 
//                }
//                if (Type == Primitives.TriangleFan || Type == Primitives.TriangleStrip) {
//                    Type = Primitives.Triangles; 
//                }
//            }
//        }
        #endregion
    }
}
