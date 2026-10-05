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
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization; 

namespace ILNumerics.Drawing {
    [Serializable]
    public abstract class ILNode : IDisposable, INotifyPropertyChanged {

        #region attributes
        internal object StructureLock = new object(); 
        protected static int s_nodesCount = 0;
        private ILGroup m_parent; 
        private bool m_visible;
        private object m_tag;
        private int m_id;
        private int m_sourceID = -1;
        private bool m_marked;
        private bool m_markable;
        private int? m_pickingID;
        private bool m_isSyncedNode = false; 
        #endregion

        #region events
        /// <summary>
        /// Fires on any property state changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(String propertyName = "") {
            Version++; 
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// Fires, before the node is getting disposed
        /// </summary>
        public event EventHandler Disposing;
        protected void OnDisposing() {
            Version++; 
            if (Disposing != null) {
                Disposing(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Fires when the mouse enters the object region
        /// </summary>
        public event EventHandler<ILMouseEventArgs> MouseEnter;
        /// <summary>
        /// Fires when the mouse leaves the object region
        /// </summary>
        public event EventHandler<ILMouseEventArgs> MouseLeave;
        /// <summary>
        /// Fires when the object is clicked
        /// </summary>
        public event EventHandler<ILMouseEventArgs> MouseClick;
        /// <summary>
        /// Fires when the object was double clicked
        /// </summary>
        public event EventHandler<ILMouseEventArgs> MouseDoubleClick;
        /// <summary>
        /// Fires when a mouse button was pressed over the object
        /// </summary>
        public event EventHandler<ILMouseEventArgs> MouseDown;
        /// <summary>
        /// Fires when a mouse button was released over the object
        /// </summary>
        public event EventHandler<ILMouseEventArgs> MouseUp;
        /// <summary>
        /// Fires when the mouse was moved over the object
        /// </summary>
        public event EventHandler<ILMouseEventArgs> MouseMove;
        /// <summary>
        /// Fires when the mouse wheel was moved over the object
        /// </summary>
        public event EventHandler<ILMouseEventArgs> MouseWheel;

        protected internal virtual void OnMouseClick(ILMouseEventArgs args) {
            RaiseMouseClick(args);
        }
        protected internal virtual void OnMouseDoubleClick(ILMouseEventArgs args) {
            RaiseMouseDoubleClick(args);
        }
        protected internal virtual void OnMouseDown(ILMouseEventArgs args) {
            RaiseMouseDown(args);
        }
        protected internal virtual void OnMouseUp(ILMouseEventArgs args) {
            RaiseMouseUp(args);
        }
        protected internal virtual void OnMouseMove(ILMouseEventArgs args) {
            RaiseMouseMove(args);
        }
        protected internal virtual void OnMouseEnter(ILMouseEventArgs args) {
            RaiseMouseEnter(args);
        }
        protected internal virtual void OnMouseLeave(ILMouseEventArgs args) {
            RaiseMouseLeave(args);
        }
        protected internal virtual void OnMouseWheel(ILMouseEventArgs args) {
            RaiseMouseWheel(args);
        }

        protected void RaiseMouseClick(ILMouseEventArgs args) {
            if (MouseClick != null) {
                MouseClick(this, args);
            }
        }
        protected void RaiseMouseDoubleClick(ILMouseEventArgs args) {
            if (MouseDoubleClick != null) {
                MouseDoubleClick(this, args);
            }
        }
        protected void RaiseMouseDown(ILMouseEventArgs args) {
            if (MouseDown != null) {
                MouseDown(this, args);
            }
        }
        protected void RaiseMouseUp(ILMouseEventArgs args) {
            if (MouseUp != null) {
                MouseUp(this, args);
            }
        }
        protected void RaiseMouseMove(ILMouseEventArgs args) {
            if (MouseMove != null) {
                MouseMove(this, args);
            }
        }
        protected void RaiseMouseEnter(ILMouseEventArgs args) {
            if (MouseEnter != null) {
                MouseEnter(this, args);
            }
        }
        protected void RaiseMouseLeave(ILMouseEventArgs args) {
            if (MouseLeave != null) {
                MouseLeave(this, args);
            }
        }
        protected void RaiseMouseWheel(ILMouseEventArgs args) {
            if (MouseWheel != null) {
                MouseWheel(this, args);
            }
        }
        #endregion

        #region constructors
        public ILNode(object tag = null) {
            m_tag = tag;
            m_id = System.Threading.Interlocked.Increment(ref s_nodesCount);
            m_visible = true;
            m_markable = true;
        }
        
        internal ILNode(ILNode source) {
            ID = System.Threading.Interlocked.Increment(ref s_nodesCount);
            this.m_sourceID = source.ID; 
            this.Parent = null; 
            this.m_tag = source.m_tag; 
            this.m_visible = source.m_visible; 
            this.m_markable = source.m_markable; 
            this.m_marked = source.m_marked; 
            this.m_pickingID = null; //source.PickingID; 

            this.MouseClick = source.MouseClick;
            this.MouseDoubleClick = source.MouseDoubleClick; 
            this.MouseDown = source.MouseDown; 
            this.MouseEnter = source.MouseEnter; 
            this.MouseLeave = source.MouseLeave; 
            this.MouseMove = source.MouseMove; 
            this.MouseUp = source.MouseUp; 
            this.MouseWheel = source.MouseWheel; 

        }
        #endregion

        #region properties
        /// <summary>
        /// The source node ID when this node was created by Copy()
        /// </summary>
        [XmlIgnore]
        internal int SourceID { get { return m_sourceID; } }
        /// <summary>
        /// The version of the sync source from the last synchronization.
        /// </summary>
        [XmlIgnore]
        internal long SynchedVersion { get; set; }
        /// <summary>
        /// Modification version of this node, gets incremented at every change
        /// </summary>
        [XmlAttribute]
        public long Version { get; internal set; }
        /// <summary>
        /// If set to a valid shape ID, that shape will be marked in picking operations insted of this shape. 
        /// </summary>
        [XmlAttribute]
        public int PickingID {
            get {
                return m_pickingID ?? ID;
            }
            set {
                if (m_pickingID != value) {
                    m_pickingID = value;
                    OnPropertyChanged("PickingID");
                }
            }
        }
        /// <summary>
        /// Get the parent of this node or null, if this node is a root node
        /// </summary>
        [XmlIgnore]
        public ILGroup Parent {
            get {
                return m_parent; 
            }
            internal set {
                if ((m_parent == null && value != null) || !ReferenceEquals(Parent,value)) {
                    m_parent = value;
                    if (m_parent != null)
                        StructureLock = m_parent.StructureLock;
                    OnPropertyChanged("Parent");
                }
            } 
        }
        /// <summary>
        /// Determines, if this node is visible or sets the visible state
        /// </summary>
        [XmlAttribute]
        public virtual bool Visible {
            get { return m_visible; }
            set {
                if (m_visible != value) {
                    m_visible = value;
                    OnPropertyChanged("Visible");
                }
            }
        }
        /// <summary>
        /// Object tag used to identify the node within the scene 
        /// </summary>
        [XmlAttribute]
        public virtual object Tag {
            get { return m_tag; }
            set {
                if (m_tag != value) {
                    m_tag = value;
                    OnPropertyChanged("Tag"); 
                }
            }
        }
        /// <summary>
        /// Unique ID for the node within the scene
        /// </summary>
        [XmlAttribute]
        public int ID { 
            get { return m_id; }
            internal set{
                if (m_id != value) {
                    m_id = value; 
                   OnPropertyChanged("ID"); 
                }
            }
        }
        /// <summary>
        /// Marked state for the node
        /// </summary>
        [XmlAttribute]
        public bool Marked {
            get { return m_marked; }
            internal set {
                if (m_marked != value) {
                    m_marked = value;
                    OnPropertyChanged("Marked");
                }
            }
        }
        /// <summary>
        /// Determines, if this node can be marked by the user
        /// </summary>
        [XmlIgnore]
        public bool Markable {
            get { return m_markable; }
            set {
                if (m_markable != value) {
                    m_markable = value;
                    OnPropertyChanged("Markable");
                }
            }
        }
        [XmlIgnore]
        protected bool IsSynchedNode {
            get { return m_isSyncedNode; }
        }
        #endregion

        #region public interface
        /// <summary>
        /// Find the root of the current node in this scene
        /// </summary>
        /// <returns>The root node of the current scene node or the node itself, if it is a group node</returns>
        internal ILGroup GetRoot() {
            if (Parent == null) {
                return this as ILGroup; 
            } 
            ILGroup ret = this.Parent;
            while (ret.Parent != null) {
                ret = ret.Parent; 
            }
            return ret; 
        }

        public abstract ILNode Detach();

        public virtual void Configure(bool configureChildren = true, bool configurePath2Root = true) {
            if (configurePath2Root) {
                ILNode cur = this.Parent;
                while (cur != null) {
                    cur.Configure(false, true);
                    cur = cur.Parent;
                }
            }
        }

        public virtual void Dispose() {
            OnDisposing();
            Parent = null; 
        }
        /// <summary>
        /// copy of the structure of this node, for general use in the tree description
        /// </summary>
        /// <returns></returns>
        internal abstract ILNode Copy(); 
        internal void Visit(ILRenderParameter parameter) {
            if (Visible) {
                if (BeginVisit(parameter)) { 
                    VisitInternal(parameter);
                    EndVisit(parameter);
                }
            }
        }
        internal protected virtual ILNode Synchronize(ILNode copy, ILSyncParams syncParams) {
            if (copy == null) {
                copy = CreateSynchedCopy(this);
                copy.m_isSyncedNode = true;
                copy.m_id = this.ID;   // <- only at creation; one time setter!
            }
            if (Version != copy.SynchedVersion) {
                //copy.m_synchedVersion = Version;  <- do not release tooo early! 
                //copy.m_id = this.m_id; <- Ids must always match. This is ensured by ILGroup / ILScene.Snapshot()
                System.Diagnostics.Debug.Assert(copy.m_id == this.m_id, "While synchronizing two (existing) nodes, the IDs of the nodes must match!"); 
                copy.m_marked = this.m_marked;
                copy.m_tag = this.m_tag;
                copy.m_visible = this.m_visible;
                copy.m_markable = this.m_markable;
                copy.m_pickingID = this.m_pickingID;
                copy.MouseClick = this.MouseClick;
                copy.MouseDoubleClick = this.MouseDoubleClick;
                copy.MouseDown = this.MouseDown;
                copy.MouseEnter = this.MouseEnter;
                copy.MouseLeave = this.MouseLeave;
                copy.MouseMove = this.MouseMove;
                copy.MouseUp = this.MouseUp;
                copy.MouseWheel = this.MouseWheel; 
            }
            return copy; 
        }
        /// <summary>
        /// handle transformations before visiting the node 
        /// </summary>
        /// <param name="parameter">render parameter instance</param>
        /// <returns>true if the node requires further processing, false: skip this node for the current rendering attempt</returns>
        protected virtual bool BeginVisit(ILRenderParameter parameter) {
            //System.Diagnostics.Debug.WriteLine("Begin Visit {0} '{1}'", GetType().Name, Tag);
            return true; 
        }
        protected virtual void VisitInternal(ILRenderParameter parameter) {
            //System.Diagnostics.Debug.WriteLine("Visiting {0} '{1}'", GetType().Name, Tag);
        }
        protected virtual void EndVisit(ILRenderParameter parameter) {
        }
        internal abstract ILNode CreateSynchedCopy(ILNode source);
        public override string ToString() {
            return String.Format("{2} #{1}{3} '{0}'", (Tag ?? "--").ToString(),
                                ID, GetType().Name, m_isSyncedNode ? "(S)":"");
        }
        internal virtual void TranslateEventLocation(bool capture, ILMouseEventArgs e) { }
        #endregion

    }
}
