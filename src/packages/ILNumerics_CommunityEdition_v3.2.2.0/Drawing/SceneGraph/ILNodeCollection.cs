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
using System.Runtime.Serialization; 

namespace ILNumerics.Drawing {
    [Serializable]
    public class ILNodeCollection : IList<ILNode> {

        public event EventHandler NodeAdded;
        protected void OnNodeAdded() {
            if (NodeAdded != null) {
                NodeAdded(this, EventArgs.Empty); 
            }
        }

        #region attributes
        List<ILNode> m_nodes; 
        ILGroup m_thisNode;
        #endregion

        #region constructors
        internal ILNodeCollection(ILGroup thisNode) {
            m_nodes = new List<ILNode>(); 
            m_thisNode = thisNode; 
        }
        #endregion

        #region IList<ILNode> Members

        public int IndexOf(ILNode item) {
            lock (m_thisNode.StructureLock) {
                return m_nodes.LastIndexOf(item);
            }
        }

        public void Insert(int index, ILNode item) {
            lock (m_thisNode.StructureLock) {
                if (item.Parent != null) {
                    item = item.Copy();
                }
                item.StructureLock = m_thisNode.StructureLock;
                m_nodes.Insert(index, item);
                item.Parent = m_thisNode;
                OnNodeAdded();
            }
        }

        public void RemoveAt(int index) {
            lock (m_thisNode.StructureLock) {
                ILNode node = m_nodes[index];
                m_nodes.RemoveAt(index);
                node.Parent = null; 
                node.Dispose();
            }
        }

        public ILNode this[int index] {
            get {
                lock (m_thisNode.StructureLock)
                    return m_nodes[index];
            }
            set {
                lock (m_thisNode.StructureLock) {
                    ILNode node = m_nodes[index];
                    node.Dispose();
                    if (value.Parent != null)
                        value = value.Copy();
                    m_nodes[index] = value;
                    value.Parent = m_thisNode;
                }
            }
        }

        #endregion

        #region ICollection<ILNode> Members

        public void Add(ILNode item) {
            lock (m_thisNode.StructureLock) {
                if (item.Parent != null)
                    item = item.Copy();
                item.Parent = m_thisNode;
                m_nodes.Add(item);
                OnNodeAdded(); 
            }
        }

        public void Clear() {
            Clear(true); 
        }
        internal void Clear(bool dispose) {
            lock (m_thisNode.StructureLock) {
                if (dispose) {
                    foreach (ILNode node in m_nodes) {
                        node.Dispose();
                    }
                }
                m_nodes.Clear();
            }
        }

        public bool Contains(ILNode item) {
            lock (m_thisNode.StructureLock) {
                return m_nodes.Contains(item);
            }
        }

        public void CopyTo(ILNode[] array, int arrayIndex) {
            lock (m_thisNode.StructureLock) {
                for (int i = arrayIndex; i < array.Length && i < this.Count; i++) {
                    array[i] = this[i];
                }
            }
        }

        public int Count {
            get {
                lock (m_thisNode.StructureLock) {
                    return m_nodes.Count;
                }
            }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(ILNode item) {
            lock (m_thisNode.StructureLock) {
                if (m_nodes.Contains(item)) {
                    m_nodes.Remove(item);
                    item.Parent = null; 
                    item.Dispose();
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region IEnumerable<ILNode> Members

        public IEnumerator<ILNode> GetEnumerator() {
            lock (m_thisNode.StructureLock) {
                return m_nodes.GetEnumerator();
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            lock (m_thisNode.StructureLock) {
                foreach (ILNode node in m_nodes) {
                    yield return node;
                }
            }
        }

        #endregion

        public ILNodeCollection Copy(ILGroup parent) {
            lock (m_thisNode.StructureLock) {
                ILNodeCollection ret = new ILNodeCollection(parent);
                foreach (ILNode node in this) {
                    ret.Add(node.Copy());
                }
                return ret;
            }
        }

        /// <summary>
        /// moves a node from its current position to a new position
        /// </summary>
        /// <param name="currentId">the current position of the node to move</param>
        /// <param name="targetId">target index of the node, before inserting)</param>
        internal void Move(int currentId, int targetId) {
            ILNode node = m_nodes[currentId];
            m_nodes.RemoveAt(currentId);
            m_nodes.Insert(currentId < targetId ? targetId + 1 : targetId,node);
        }
    }
}
