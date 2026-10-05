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

namespace ILNumerics.Drawing.Animation {
    [Serializable]
    public class ILAnimationCollection<NodeT> : IEnumerable<ILAnimation<NodeT>> where NodeT : ILNode {

        IList<ILAnimation<NodeT>> m_actions;
        private object m_syncLock = new object();
        private NodeT m_parent; 

        public ILAnimationCollection(NodeT parent) {
            m_actions = new List<ILAnimation<NodeT>>();
            m_parent = parent; 
        }

        #region IEnumerable<IILAction<T>> Members
        internal NodeT Parent { get { return m_parent; } }

        public IEnumerator<ILAnimation<NodeT>> GetEnumerator() {
            return m_actions.GetEnumerator();  
        }

        #endregion
        #region IEnumerable Members 

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator(); 
        }

        #endregion

        #region public interface 

        public void Add(ILAnimation<NodeT> action) {
            lock (m_syncLock) {
                m_actions.Add(action);
                action.Parent = Parent; 
            }
        }
        public void Insert(int index, ILAnimation<NodeT> action) {
            lock (m_syncLock) {
                m_actions.Insert(index, action);
                action.Parent = Parent; 
            }
        }
        public bool Remove(ILAnimation<NodeT> action) {
            lock (m_syncLock) {
                bool ret = m_actions.Remove(action);
                if (ret) {
                    action.Parent = null; 
                }
                return ret; 
            }
        }
        public bool Remove(object tag) {
            lock (m_syncLock) {
                ILAnimation<NodeT> act = m_actions.Where((a) => { return (a.Tag ?? "").Equals(tag); }).First(); 
                if (act != null) 
                    return Remove(act);
                return false; 
            }
        }

        public ILAnimationCollection<NodeT> Copy() {
            ILAnimationCollection<NodeT> ret = new ILAnimationCollection<NodeT>(Parent); 
            lock (m_syncLock) {
                foreach (var item in m_actions) {
                    ret.Add(item); 
                }
            }
            return ret; 
        }

        #endregion


    }
}
