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

namespace ILNumerics.Data {
    /// <summary>
    /// experimental performant priority queue implementation (WORK IN PROGRESS!)
    /// </summary>
    /// <typeparam name="T">inner type for elements (arbitrary)</typeparam>
    public class ILProrityQueue<T> : ILBinTree<T> where T:IComparable {
        /// <summary>
        /// add an element to the queue
        /// </summary>
        /// <param name="element"></param>
        public void Add(T element) {
            ILBinTreeNode<T> cur = null; // m_root; 
            while (cur != null) {
                
            }
            m_count++; 
        }

        private void createHeap() {
            
        }
        private void heapify(ILBinTreeNode<T> top) {
            System.Diagnostics.Debug.Assert(top != null && top.Data != null); 
            int res; 
            ILBinTreeNode<T> child; 
            if (top.LeftSon != null) {
                System.Diagnostics.Debug.Assert(top.LeftSon.Data != null);
                T ls = top.LeftSon.Data; 
                if (top.RightSon != null) {
                    System.Diagnostics.Debug.Assert(top.RightSon.Data != null);
                    T rs = top.RightSon.Data; 
                    if (ls.CompareTo(rs) > 0) 
                        child = top.LeftSon; 
                    else 
                        child = top.RightSon;
                } else {
                    child = top.LeftSon; 
                }
            } else {
                if (top.RightSon != null) {
                    System.Diagnostics.Debug.Assert(top.RightSon.Data != null);
                    child = top.RightSon; 
                } else {
                    return; 
                }
            }
            // may exchange this data with largest node's value 
            if (child.m_data.CompareTo(top.m_data) > 0) {
                T tmp = child.m_data; 
                child.m_data = top.m_data; 
                top.m_data = tmp; 
                heapify(child); 
            }   
        }
    }
}
