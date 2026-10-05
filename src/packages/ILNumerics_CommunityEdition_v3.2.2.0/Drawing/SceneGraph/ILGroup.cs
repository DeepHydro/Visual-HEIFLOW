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
using OpenTK; 
using ILNumerics.Drawing.Animation;
using System.Xml.Serialization;
using System.Security; 

namespace ILNumerics.Drawing {
    /// <summary>
    /// ILGroup holds and manages arbitrary nodes and serves as base class for custom visual object implementations
    /// </summary>
    [System.Diagnostics.DebuggerTypeProxy(typeof(ILGroupVisualizer))] 
    [Serializable]
    public class ILGroup : ILNode, IILGroup, IEnumerable<ILNode> {

        #region attributes
        Matrix4 m_transform;
        protected ILAnimationCollection<ILGroup> m_animations;
        float? m_alpha = null; 
        Color? m_colorOverride = null;
        internal RenderTarget? m_renderTarget;
        private ILClipParams m_clipParams;
        protected ILNodeCollection m_children;

        #endregion
        
        #region properties
        /// <summary>
        /// Access to direct children, safe even for derived classes
        /// </summary>
        /// <remarks>This property gives safe access to the internal node collection of children. Since a derived class 
        /// may change the Children property semantic, this property ensures the access to the original collection.</remarks>
        [XmlIgnore]
        internal ILNodeCollection InternalChildren { get { return m_children; } }
        /// <summary>
        /// If set, determines the target for rendering: world (3D) or screen (2D) on top. Default: not set (derive from parent)
        /// </summary>
        /// <remarks>Screen components are drawn <b>on top</b> of World (3D) content. Per default, nodes derive the target setting from their parent. The root node of a scene graph is set to World (3D), so by default, all nodes are drawn as World targets.</remarks>
        [XmlAttribute]
        public RenderTarget? Target {
            [SecuritySafeCritical]
            get {
                return m_renderTarget;
            }
            protected set {
                if (m_renderTarget != value) {
                    m_renderTarget = value;
                    OnPropertyChanged("Target");
                }
            }
        }
        /// <summary>
        /// Gets all 6 available clipping planes for the group or sets it. Null: derive clipping from parent node (default)
        /// </summary>
        public ILClipParams Clipping {
            get {
                return m_clipParams;
            }
            set {
                if (m_clipParams != value) {
                    m_clipParams = value;
                    OnPropertyChanged("Clipping");
                }
            }
        }
        [XmlIgnore]
        public ILAnimationCollection<ILGroup> Animations {
            get {
                if (m_animations == null) {
                    m_animations = new ILAnimationCollection<ILGroup>(this);
                }
                return m_animations; 
            }
        }
        /// <summary>
        /// [4x4] matrix with the affine transforms which this node applies to all children
        /// </summary>
        public Matrix4 Transform { 
            get { return m_transform; }
            set {
                if (!m_transform.Equals(value)) {
                    m_transform = value;
                    OnPropertyChanged("Transform"); 
                }
            }
        }
        /// <summary>
        /// Collection of child for this group
        /// </summary>
        [XmlIgnore]
        public virtual ILNodeCollection Children {
            get { return m_children; }
            private set { m_children = value; }
        }
        /// <summary>
        /// Collection of child for this group
        /// </summary>
        [Obsolete("Use ILGroup.Children instead!")]
        [XmlIgnore]
        public virtual ILNodeCollection Childs {
            get { return m_children; }
            private set { m_children = value; }
        }
        /// <summary>
        /// Alpha value, range [0...1],  if set, this value modifies the alpha values for ALL nodes of this subtree. Use for blend effects only!
        /// </summary>
        [XmlAttribute]
        public float? Alpha {
            get { return m_alpha; }
            set {
                if (m_alpha != value) {
                    m_alpha = value; 
                    OnPropertyChanged("Alpha"); 
                }
            }
        }
        /// <summary>
        /// If set, this color will override all individual colors in this subtree
        /// </summary>
        public Color? ColorOverride {
            get { return m_colorOverride; }
            set {
                if (m_colorOverride != value) {
                    m_colorOverride = value; 
                    OnPropertyChanged("ColorOverride"); 
                }
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new empty group
        /// </summary>
        /// <param name="tag">tag object, used to identify the node within the scene</param>
        /// <param name="angle">rotation angle for groups children</param>
        /// <param name="rotateAxis">rotation axis for groups children</param>
        /// <param name="scale">X, Y and Z scale for groups children</param>
        /// <param name="translate">translation for groups children</param>
        /// <remarks>If specified, the transformations are applied in that order: scale, translate, rotate.</remarks>
        public ILGroup(object tag = null, Vector3? rotateAxis = null, double angle = 0, Vector3? scale = null, Vector3? translate = null)
            : base(tag) {
            m_children = new ILNodeCollection(this);
            Transform = Matrix4.Identity;
            if (scale.HasValue) {
                Transform = Transform.Scale(scale.GetValueOrDefault().X, scale.GetValueOrDefault().Y, scale.GetValueOrDefault().Z); 
            }
            if (translate.HasValue) {
                Transform = Transform.Translate(translate.GetValueOrDefault().X, translate.GetValueOrDefault().Y, translate.GetValueOrDefault().Z);
            }
            if (rotateAxis.HasValue) {
                Transform = Transform.Rotate(rotateAxis.GetValueOrDefault(), (float)angle);
            }
            Alpha = null; 
            ColorOverride = null; 
        }
        /// <summary>
        ///  create a new group as shallow copy of a subtree, shared buffers
        /// </summary>
        /// <param name="source">source subtree</param>
        internal ILGroup(ILGroup source) 
            : base(source) {
                lock (source.StructureLock) {
                    m_children = new ILNodeCollection(this);
                    foreach (ILNode node in source.m_children) {
                        m_children.Add(node.Copy());
                    }
                    m_transform = source.m_transform;
                    m_alpha = source.m_alpha; 
                    m_colorOverride = source.m_colorOverride;
                    m_renderTarget = source.m_renderTarget;
                    if (source.m_clipParams != null)
                        m_clipParams = source.m_clipParams.Copy(); 
                }
        }
        #endregion

        #region public interface 
        /// <summary>
        /// Searches the subtree for nodes with matching tag and (optional) primitive type
        /// </summary>
        /// <param name="tag">tag filter</param>
        /// <param name="kind">kind (optional)</param>
        /// <returns>collection of matching nodes</returns>
        public IEnumerable<ILNode> Find(object tag, Primitives? kind = null) {
            List<ILNode> list = new List<ILNode>(); 
            Find(tag, this, list, kind);
            return list; 
        }
        /// <summary>
        /// Gets first node with matching criteria from subtree
        /// </summary>
        /// <typeparam name="T">node type to filter for</typeparam>
        /// <param name="tag">tag filter (optional); if omitted, all tags are accepted</param>
        /// <param name="predicate">arbitrary predicate function (optional); if omitted, all nodes are accepted</param>
        /// <returns>first node, which matches all given criteria</returns>
        public T First<T>(object tag = null, Predicate<T> predicate = null) where T : ILNode {
            IEnumerable<T> list = Find<T>(tag, predicate);
            return list.Count() > 0 ? list.First() : null;
        }
        /// <summary>
        /// Searches the subtree for all nodes with matching criteria
        /// </summary>
        /// <typeparam name="T">node type to filter for</typeparam>
        /// <param name="tag">tag filter (optional); if omitted, all tags are accepted</param>
        /// <param name="predicate">arbitrary predicate function (optional); if omitted, all nodes are accepted</param>
        /// <returns>all nodes, with matching criteria or null</returns>
        public IEnumerable<T> Find<T>(object tag = null, Predicate<T> predicate = null)
                    where T : ILNode {
            List<T> list = new List<T>();
            Find<T>(tag, this, list, predicate);
            return list;
        }
        /// <summary>
        /// Finds a group node on the path up to root
        /// </summary>
        /// <typeparam name="T">The concrete type of the node</typeparam>
        /// <param name="key">tag filter, leave null to match all tags</param>
        /// <param name="predicate">arbitrary predicate, leave null to match any node</param>
        /// <returns>the first node matching the filter creteria or null, if none exists</returns>
        public T FirstUp<T>(object key = null, Predicate<T> predicate = null)
                    where T : ILGroup {
            ILGroup cur = this.Parent;
            while (cur != null) {
                if (cur is T) {
                    T currentT = cur as T;
                    if ((predicate == null || predicate(currentT))
                        && (key == null || string.IsNullOrEmpty(key.ToString()) ||
                            (currentT != null && currentT.Tag != null && currentT.Tag.ToString().ToLower().Contains(key.ToString().ToLower())))) {
                        return currentT; 
                    }
                }
                cur = cur.Parent; 
            }
            return null; 
        }

        /// <summary>
        /// Finds a typed node in this subtree, filter by ID 
        /// </summary>
        /// <param name="id">ID filter</param>
        /// <returns>matching node or null</returns>
        public T FindById<T>(int id) where T : ILNode {
            return FindId<T>(id, this);
        }
        /// <summary>
        /// Configure the subtree after changes to any buffer
        /// </summary>
        public override void Configure(bool configureChildren = true, bool configurePath2Root = true) {
            if (configureChildren) {
                foreach (ILNode node in m_children) {
                    node.Configure(true, false);
                }
            }
            if (configurePath2Root) {
                ILNode cur = this.Parent;
                while (cur != null) {
                    cur.Configure(false, true);
                    cur = cur.Parent;
                }
            }
        }
        /// <summary>
        /// Dispose the complete subtree (this is rarely needed)
        /// </summary>
        public override void Dispose() {
            foreach (ILNode node in m_children) {
                if (node != null) 
                    node.Dispose(); 
            }
        }
        /// <summary>
        /// Rotate this subtree by quaternion
        /// </summary>
        /// <param name="offset">Quaternion, describing the target orientation</param>
        /// <returns>this group</returns>
        public ILGroup Rotate(Quaternion offset) {
            Transform = Transform.Rotate(offset);
            return this;
        }
        /// <summary>
        /// Rotate this subtree by rotation axis and angle
        /// </summary>
        /// <param name="directionX">rotation axis X coordinate</param>
        /// <param name="directionY">rotation axis Y coordinate</param>
        /// <param name="directionZ">rotation axis Z coordinate</param>
        /// <param name="angle">angle in radians</param>
        /// <returns>this group</returns>
        public ILGroup Rotate(double directionX, double directionY, double directionZ, double angle) {
            return Rotate(new Vector3(directionX,directionY,directionZ), angle);
        }
        /// <summary>
        /// Rotate this subtree by rotation axis and angle
        /// </summary>
        /// <param name="direction">rotation axis</param>
        /// <param name="angle">angle in radians</param>
        /// <returns>this group</returns>
        public ILGroup Rotate(Vector3 direction, double angle) {
            Transform = Transform.Rotate(direction, (float)angle);
            OnPropertyChanged("Transform");
            return this;
        }
        /// <summary>
        /// Translates this subtree
        /// </summary>
        /// <param name="x">Offset in X direction</param>
        /// <param name="y">Offset in Y direction</param>
        /// <param name="z">Offset in Z direction</param>
        /// <returns>this group</returns>
        public ILGroup Translate(double x, double y, double z) {
            m_transform = m_transform.Translate(x, y, z); 
            OnPropertyChanged("Transform"); 
            return this;
        }
        /// <summary>
        /// Translates this subtree
        /// </summary>
        /// <param name="vec">Offset vector</param>
        /// <returns>this group</returns>
        public ILGroup Translate(Vector3 vec) {
            Translate(vec.X, vec.Y, vec.Z);
            return this; 
        }
        /// <summary>
        /// Scales this subtree
        /// </summary>
        /// <param name="x">scale factor in X direction</param>
        /// <param name="y">scale factor in Y direction</param>
        /// <param name="z">scale factor in Z direction</param>
        /// <returns>this group</returns>
        public ILGroup Scale(double x, double y, double z) {
            m_transform = m_transform.Scale(x, y, z); 
            OnPropertyChanged("Transform"); 
            return this; 
        }
        /// <summary>
        /// Scales this subtree
        /// </summary>
        /// <param name="vec">vector with scale factors in X, Y and Z direction</param>
        /// <returns>this group</returns>
        public ILGroup Scale(Vector3 vec) { 
            Scale(vec.X,vec.Y,vec.Z);
            return this;
        }

        /// <summary>
        /// pushes the transform, alpha and color overrides of this node to the render parameter stack
        /// </summary>
        /// <param name="parameter"></param>
        protected override bool BeginVisit(ILRenderParameter parameter) {
            bool ret = base.BeginVisit(parameter); 
            parameter.Push(Transform);
            if (Alpha.HasValue) {
                parameter.Alpha.Push(Alpha.Value);
            }
            if (ColorOverride.HasValue) {
                parameter.ColorOverride.Push(ColorOverride);
            }
            if (Target.HasValue) {
                parameter.RenderTargets.Push(Target.GetValueOrDefault());
            }
            if (Clipping != null) {
                parameter.PushClipping(Clipping);
            }
            return ret; 
        }
        /// <summary>
        /// pops the render parameter from render parameter stack
        /// </summary>
        /// <param name="parameter"></param>
        protected override void EndVisit(ILRenderParameter parameter) {
            parameter.Pop();
            if (Alpha.HasValue) {
                parameter.Alpha.Pop();
            }
            if (ColorOverride.HasValue) {
                parameter.ColorOverride.Pop();
            }
            if (Target.HasValue) {
                parameter.RenderTargets.Pop();
            }
            if (Clipping != null) {
                parameter.PopClipping();
            }
        }

        protected override void VisitInternal(ILRenderParameter parameter) {
            foreach (ILNode child in m_children) {
                child.Visit(parameter);
            }
        }
        /// <summary>
        /// Add a node to the end of this groups child collection
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <param name="node">node object</param>
        /// <param name="tag">tag for the newly added node (optional)</param>
        /// <param name="shareBuffers">[optional] if adding creates a clone, determines, if its buffer are shared. Default: true</param>
        /// <returns>the newly added node</returns>
        /// <remarks>If the <paramref name="node"/> given is already used within any scene graph a (shallow, lazy) copy is made and the copy is added to the group instead. Such copies are 
        /// individually configurable in most scalar properties. However, they (by default) share the buffer sets - i.e. vertices, indices, normals and interpolating colors buffers.</remarks>
        public virtual void Add(ILNode node) {
            Add(node, null, true); 
        }
        /// <summary>
        /// Add a node to the end of this groups child collection
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <param name="node">node object</param>
        /// <param name="tag">tag for the newly added node (optional)</param>
        /// <param name="shareBuffers">[optional] if adding creates a clone, determines, if its buffer are shared. Default: true</param>
        /// <returns>the newly added node</returns>
        /// <remarks>If the <paramref name="node"/> given is already used within any scene graph a (shallow, lazy) copy is made and the copy is added to the group instead. Such copies are 
        /// individually configurable in most scalar properties. However, they (by default) share the buffer sets - i.e. vertices, indices, normals and interpolating colors buffers.</remarks>
        public virtual T Add<T>(T node, object tag = null, bool shareBuffers = true) where T : ILNode {
            ILNode copyNode = node;
            if (node != null && node.Parent != null) {
                copyNode = node.Copy();
                if (!shareBuffers) {
                    copyNode.Detach();
                }
            }
            if (tag != null) {
                copyNode.Tag = tag;
            }
            m_children.Add(copyNode);

#if SHOWNORMALS
            ILShape shape = node as ILShape; 
            if (shape != null && shape is ILTriangles) 
                Childs.Add(new ILNormalVisualizer(shape)); 
#endif

            return (T)copyNode;
        }
        /// <summary>
        /// Insert a node into the child collection at predefined index 
        /// </summary>
        /// <typeparam name="T">node type</typeparam>
        /// <param name="index">index to place the new node at</param>
        /// <param name="node">node object</param>
        /// <param name="tag">tag for the newly added node (optional)</param>
        /// <returns>the newly added node</returns>
        /// <remarks>If the <paramref name="node"/> given is already used within any scene graph a (shallow, lazy) copy is made and the copy is added to the group instead. Such copies are 
        /// individually configurable in most scalar properties. However, they (by default) share the buffer sets - i.e. vertices, indices, normals and interpolating colors buffers.</remarks>
        public virtual T Insert<T>(int index, T node, object tag = null) where T : ILNode {
            ILNode copyNode = (node.Parent != null) ? node.Copy() : node;
            if (tag != null) copyNode.Tag = tag;
            m_children.Insert(index, copyNode);

#if SHOWNORMALS
            ILShape shape = node as ILShape; 
            if (shape != null && shape is ILTriangles) 
                Childs.Add(new ILNormalVisualizer(shape)); 
#endif

            return node;
        }
        /// <summary>
        /// Remove a node from this subtree
        /// </summary>
        /// <param name="node">node to remove</param>
        /// <returns>true if the node was found and successfully removed, false otherwise</returns>
        public bool Remove(ILNode node) {
            if (node.Parent == null) return false; 
            // check if the node is really part of my subtree
            ILNode cur = node; 
            int exit = 1000; 
            while (exit-->0 && cur.Parent != null && cur.Parent != this) cur = cur.Parent;
            if (cur.Parent == this && exit > 0) {
                return node.Parent.m_children.Remove(node);
            } else {
                return false;
            }
        }
        /// <summary>
        /// Detach all shapes from this subtree for individual configurations
        /// </summary>
        public override ILNode Detach() {
            foreach (var drawable in Find<ILDrawable>()) {
                drawable.Detach(); 
            }
            return this; 
        }
        /// <summary>
        /// Compute the 3D limits (extent) of this subtree
        /// </summary>
        /// <param name="lowerBound">The lower limit of the search.</param>
        /// <returns>Limits of the bounding box of all nodes in this subtree</returns>
        /// <remarks>The function walks the complete subtree and computes the extent for every shape contained. 
        /// Group node transformations are taken into account and the limits of the bounding box for all nodes is returned.
        /// <para>For every axis, only those positions 
        /// from shape vertices are considered, which lay <i>above</i> the bound determined by <paramref name="lowerBound"/>. 
        /// This parameter is usefull for working with logarithmic axes scales. If ommited, all data are taken into account [Default].</para></remarks>
        public ILLimits GetLimits(Vector3? lowerBound = null) {
            ILLimits ret = new ILLimits(); 
            Stack<Matrix4> transforms = new Stack<Matrix4>(); 
            transforms.Push(Matrix4.Identity); 
            getLimitsInternal(transforms, ret, lowerBound: lowerBound);
            //// the following tries to cheaply ensure a non-empty volume. 
            //// it fails for ill conditioned limits (appr. float +/- max)!
            //if (ret.DepthF < ILMath.epsf) {
            //    if (ret.ZMin > float.MinValue) 
            //        ret.ZMin = ret.ZMin - 1);
            //    ret.ZMax = ret.ZMin + 1;
            //}
            //if (ret.WidthF < ILMath.epsf) {
            //    ret.XMin -= Math.Min(float.MinValue, ret.XMin - 1);
            //    ret.XMax = ret.XMin + 1;
            //}
            //if (ret.HeightF < ILMath.epsf) {
            //    ret.YMin -= Math.Min(float.MinValue, ret.YMin - 1);
            //    ret.YMax = ret.YMin + 1;
            //}
            return ret; 
        }

        protected virtual void SynchronizeChildren(ILSyncParams syncParams, ILGroup copyGroup) {
            if (m_children.Count == 0) {
                copyGroup.m_children.Clear();
            } else {
                #region naive attempt (obsolete)
                //for (int i = 0; i < Math.Max(copyGroup.Childs.Count, Childs.Count); i++) {
                //    if (Childs.Count <= i) {
                //        while (copyGroup.Childs.Count > Childs.Count) {
                //            copyGroup.Childs.RemoveAt(copyGroup.Childs.Count-1);
                //        }
                //        break; 
                //    }
                //    if (copyGroup.Childs.Count <= i) {
                //        // new node created
                //        ILNode cur = Childs[i];
                //        ILNode newNode = cur.Synchronize(null, syncParams);
                //        if (i < copyGroup.Childs.Count) {
                //            copyGroup.Childs.Insert(i, newNode);
                //        } else {
                //            copyGroup.Childs.Add(newNode); 
                //        }
                //    } else if (copyGroup.Childs.Count > i) {
                //        if (Childs[i].ID != copyGroup.Childs[i].ID) {
                //            ILNode curNode = Childs[i];
                //            ILNode newNode = curNode.CreateSelf();
                //            newNode.ID = curNode.ID;
                //        }
                //        ILNode cur = Childs[i];
                //        cur.Synchronize(copyGroup.Childs[i], syncParams);
                //    }
                //}
                #endregion
                int i = 0;
                while (i < m_children.Count) {
                    int myID = m_children[i].ID;
                    if (copyGroup.m_children.Count > i && myID == copyGroup.m_children[i].ID) {
                        m_children[i].Synchronize(copyGroup.m_children[i], syncParams);
                        // only AFTER all synchs are completed, synch the versions of the nodes
                        copyGroup.m_children[i].SynchedVersion = m_children[i].Version; 
                    } else {
                        // diff found, is the node in rest of copy? (i.e. a node was removed)
                        int foundId = -1;
                        for (int c = i; c < copyGroup.m_children.Count; c++) { // start at i since all nodes are unique!
                            if (copyGroup.m_children[c].ID == myID) {
                                foundId = c;
                                break;
                            }
                        }
                        if (foundId > -1) {
                            copyGroup.m_children.Move(foundId, i);
                        } else {
                            // create new 
                            ILNode newNode = m_children[i].Synchronize(null, syncParams);
                            newNode.SynchedVersion = m_children[i].Version; 
                            copyGroup.Insert(i, newNode); // needed for plotcube.autosizeonadd
                        }
                    }
                    i++;
                }
                // if 'old' nodes are left over, they are removed from cur
                while (i < copyGroup.m_children.Count) {
                    copyGroup.m_children.RemoveAt(i);
                    i++;
                }
            }
        }
        /// <summary>
        /// String representation of the group node
        /// </summary>
        /// <returns>string summarizing this group node</returns>
        public override string ToString() {
            return String.Format("{0} Children:[{1}]", base.ToString(), m_children.Count);
        }
        #endregion

        #region private helper
        internal class ILGroupVisualizer {

            ILGroup m_group; 
            public ILGroupVisualizer(ILGroup group) {
                m_group = group; 
            }

            [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)]            
            public ILNode[] Nodes {
                get { return m_group.m_children.ToArray(); }
            }

        }
        internal override ILNode CreateSynchedCopy(ILNode source) {
            return new ILGroup();
        }
        /// <summary>
        /// Create a shallow copy of this subtree
        /// </summary>
        /// <returns>copy of this subtree</returns>
        internal override ILNode Copy() {
            ILGroup ret = new ILGroup(this);
            return ret;
        }
        internal protected override ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            //Console.WriteLine("Syncronizing {1} on ID {0}  - mapped: {2}", ID, copy.ID, syncParams.BufferMap.Count); 
            ILGroup copyGroup = (ILGroup)base.Synchronize(copy, syncParams);
            copyGroup.Transform = Transform;
            if (copyGroup.SynchedVersion != Version) {
                copyGroup.Alpha = Alpha;
                copyGroup.ColorOverride = ColorOverride;
                copyGroup.m_renderTarget = m_renderTarget;
                if (m_clipParams != null) {
                    copyGroup.m_clipParams = m_clipParams.Copy();
                }
            }

            SynchronizeChildren(syncParams, copyGroup);
            // apply animations 
            foreach (var anim in Animations.Copy()) {
                anim.Frame(syncParams.CurrentTime, copyGroup);
            }
            return copyGroup;
        }
        protected virtual void getLimitsInternal(Stack<Matrix4> transforms, ILLimits ret, bool ignoreRootTransform = true, Vector3? lowerBound = null) {
            Matrix4 curTrafo = transforms.Peek();
            if (!ignoreRootTransform || transforms.Count > 1)
                curTrafo = curTrafo * Transform;
            transforms.Push(curTrafo);
            foreach (var n in m_children) {
                ILShape shape = n as ILShape;
                if (shape != null && !shape.Positions.IsEmpty) {
                    Vector3 min = new Vector3();
                    Vector3 max = new Vector3(); 
                    if (lowerBound.HasValue) {
                        // for log axis wie must select the limits of the shape 
                        // from the positive vertices only. Take the transformations 
                        // and indexed rendering into account ...
                        using (ILScope.Enter()) {
                            ILArray<float> pos = shape.Positions.Storage;
                            if (!shape.Indices.IsEmpty) {
                                pos.a = pos[":", shape.Indices.Storage[":"]];
                            }
                            pos.a = curTrafo * pos; 
                            for (int idx = 0; idx < 3; idx++) {
                                using (ILScope.Enter()) {
                                    ILArray<float> axisVals = pos[idx, ":"];
                                    float bound = lowerBound.Value[idx];

                                    if (!float.IsNaN(bound)) {
                                        axisVals.a = axisVals[axisVals > bound];
                                    }
                                    float minA, maxA;
                                    if (axisVals.GetLimits(out minA, out maxA)) {
                                        min[idx] = minA; max[idx] = maxA;
                                    }
                                }
                            }
                        }
                    } else {
                        min = curTrafo * shape.Limits.Min;
                        max = curTrafo * shape.Limits.Max;
                    }
                    ret.Update(min, max);
                } else {
                    ILGroup group = n as ILGroup;
                    if (group != null) {
                        group.getLimitsInternal(transforms, ret, lowerBound: lowerBound);
                    }
                }
            }
            transforms.Pop();
        }

        internal void TakeChildsFrom(ILGroup iLGroup) {
            foreach (ILNode node in iLGroup.m_children) {
                node.Parent = null;
                m_children.Add(node);
            }
            iLGroup.m_children.Clear(false);
        }

        private void Find<T>(object key, ILNode current, List<T> list, Predicate<T> predicate = null) where T : ILNode {
            if (current is T) {
                T currentT = current as T;
                if ((predicate == null || predicate(currentT))
                    && (key == null || string.IsNullOrEmpty(key.ToString()) ||
                        (currentT != null && currentT.Tag != null && currentT.Tag.ToString().ToLower().Contains(key.ToString().ToLower())))) {
                    list.Add(currentT);
                }
            }
            if (current is ILGroup) {
                ILNodeCollection nodes = (current as ILGroup).m_children;
                foreach (ILNode node in nodes) {
                    Find<T>(key, node, list, predicate);
                }
            }
        }

        private void Find(object key, ILNode current, List<ILNode> list, Primitives? Kind = null) {
            ILShape shape = current as ILShape;
            if (((Kind.HasValue) ? shape != null && Kind.Value == shape.Type : true)
                && ((key != null && !string.IsNullOrEmpty(key.ToString())) ? current != null && current.Tag != null && current.Tag.ToString().ToLower().Contains(key.ToString().ToLower()) : true)) {
                list.Add(current);
            }
            if (current is ILGroup) {
                ILNodeCollection nodes = (current as ILGroup).m_children;
                foreach (ILNode node in nodes) {
                    Find(key, node, list, Kind);
                }
            }
        }

        internal T FindId<T>(int id, ILNode current) where T : ILNode {
            if (current.ID == id && current is T) return current as T;
            if (current is ILGroup) {
                ILNodeCollection nodes = (current as ILGroup).m_children;
                foreach (ILNode node in nodes) {
                    T cur = FindId<T>(id, node);
                    if (cur != null)
                        return cur;
                }
            }
            return null;
        }
        #endregion

        #region IEnumerable<ILNode> Members
        [ThreadStatic]
        private Stack<ILNode> m_traverseStack;
        [XmlIgnore]
        private Stack<ILNode> TraverseStack {
            get {
                if (m_traverseStack == null) {
                    m_traverseStack = new Stack<ILNode>(); 
                }
                return m_traverseStack; 
            }
        }
        public IEnumerator<ILNode> GetEnumerator() {
            ILNode node;
            TraverseStack.Push(this);
            while (m_traverseStack.Count > 0) {
                node = m_traverseStack.Pop();
                yield return node;
                ILGroup group = node as ILGroup;
                if (group != null) {
                    foreach (ILNode child in group.m_children) {
                        m_traverseStack.Push(child);
                    }
                }
            }
        }
        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator(); 
        }

        #endregion

    }
}
