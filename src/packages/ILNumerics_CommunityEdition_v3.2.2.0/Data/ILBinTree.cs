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


#region LGPL License
/*    
    This file is part of ILNumerics.Net Core Module.

    ILNumerics.Net Core Module is free software: you can redistribute it 
    and/or modify it under the terms of the GNU Lesser General Public 
    License as published by the Free Software Foundation, either version 3
    of the License, or (at your option) any later version.

    ILNumerics.Net Core Module is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with ILNumerics.Net Core Module.  
    If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
        using System.Windows.Forms; 


namespace ILNumerics.Data {
    [System.Security.SecuritySafeCritical]
    public class ILBinTree<T> {
        
        protected int m_count; 
        protected ILBinTreeNode<T> m_root;
        public virtual bool Contains (T data) {
            return Contains(m_root,data); 
        }
        private bool Contains(ILBinTreeNode<T> _root, T data) {
            if (object.Equals(_root.Data,data)) return true;  
            if (_root.LeftSon != null && Contains(_root.LeftSon,data)) return true; 
            if (_root.RightSon != null && Contains(_root.RightSon,data)) return true; 
            return false;      
        }
        //public ILBinTreeNode<T> Find(T data) {
                
        //}
        //public ILBinTreeNode<T> AddLeftSon (ILBinTreeNode<T> node, T) {
        //    T ret = null; 
        //    if (node.LeftSon == null) {

        //    } else {

        //        return 
        //    }
        //}

    }
    public class ILBinTreeNode<T> {
        internal T m_data; 
        internal ILBinTreeNode<T> m_father; 
        internal ILBinTreeNode<T> m_leftSon; 
        internal ILBinTreeNode<T> m_rightSon; 

        public T Data {
            get {
                return m_data; 
            }
            set {
                m_data = value; 
            }
        }
        public ILBinTreeNode<T> Father {
            get {
                return m_father; 
            }
            set {
                m_father = value; 
            }
        }
        public ILBinTreeNode<T> LeftSon {
            get {
                return m_leftSon; 
            }
            set {
                m_leftSon = value; 
            }
        }
        public ILBinTreeNode<T> RightSon {
            get {
                return m_rightSon; 
            }
            set {
                m_rightSon = value; 
            }
        }
        public ILBinTreeNode() {}
        public ILBinTreeNode(T data) {
            m_data = data; 
        }

        public bool IsLeaf() {
            return m_leftSon == null && m_rightSon == null; 
        }
        public int Height () {
            if (IsLeaf()) return 0; 
            int lh = (m_leftSon != null) ? m_leftSon.Height() : 0; 
            int rh = (m_rightSon != null) ? m_rightSon.Height() : 0; 
            if (lh > rh) return lh + 1; else return rh + 1; 
        }
        public override string  ToString() {
 	        return ToString(0, '-', BinTreeWalkingMode.PreOrder);
        }
        public string ToString(int indent, char indentChar, BinTreeWalkingMode walkingMode) {
            StringBuilder sb = new StringBuilder(); 
            switch (walkingMode) {
                case BinTreeWalkingMode.PreOrder:
                    #region PreOrder
                    string s; 
                    if (m_data != null) {
                        s = m_data.ToString(); 
                    } else {
                        s = "(null)"; 
                    }
                    sb.Append(s); indent += s.Length; 
                    sb.Append(indentChar); 
                    if (m_leftSon != null) {
                        sb.Append(m_leftSon.ToString(indent + 1, indentChar,walkingMode)); 
                    } else {
                        sb.Append(Environment.NewLine); 
                    }
                    sb.Append(new String(' ',indent) + indentChar); 
                    if (m_rightSon != null) {
                        sb.Append(m_rightSon.ToString(indent + 1, indentChar,walkingMode)); 
                    } else {
                        sb.Append(Environment.NewLine); 
                    } 
                    break;
                    #endregion
                default: 
                    throw new InvalidOperationException(); 
            }
            return sb.ToString();
        }
        
    
    }
    public enum BinTreeWalkingMode {
        PreOrder,
        PostOrder,
        InOrder
    }

}
