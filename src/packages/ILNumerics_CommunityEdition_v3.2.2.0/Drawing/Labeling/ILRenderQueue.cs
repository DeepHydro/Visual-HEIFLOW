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
using System.Collections; 
using System.Collections.Generic;
using System.Text;
using System.Drawing; 

namespace ILNumerics.Drawing {
    /// <summary>
    /// the class collects renderable items which define 
    /// the graphical output for a render expression
    /// </summary>
    /// <remarks>ILRenderQueues are semi-immutable. Instances - once created - can only be cleared and 
    /// re-created, but not altered. Therefore, they keep the size of the output cached over their livetime.</remarks>
    public class ILRenderQueue : IEnumerable {

        #region attributes 
        SizeF m_size; 
        List<ILRenderQueueItem> m_list; 
        String m_expression;
        #endregion

        #region constructor
        /// <summary>
        ///  constructor, creates a new render queue with content
        /// </summary>
        /// <param name="expression">expression, which led to this queue</param>
        /// <param name="queue">prepared queue</param>
        /// <param name="size">size of content after rendering</param>
        public ILRenderQueue(string expression, List<ILRenderQueueItem> queue, SizeF size) : base () {
            m_size = size; 
            m_list = queue; 
            m_expression = expression;
        }
        #endregion 

        #region properties 
        /// <summary>
        /// overall size of content of this render queue in pixels
        /// </summary>
        public SizeF Size {
            get {
                return m_size;     
            }
        }

        /// <summary>
        /// Expression which led to this queue
        /// </summary>
        public string Expression {
            get { return m_expression; }
        }
        #endregion

        #region IList<ILRenderQueueItem> Member

        public int IndexOf(ILRenderQueueItem item) {
            return m_list.IndexOf(item); 
        }

        //public void Insert(int index, ILRenderQueueItem item) {
        //    m_list.Insert(index,item); 
        //}

        //public void RemoveAt(int index) {
        //    m_list.RemoveAt(index); 
        //}

        public ILRenderQueueItem this[int index] {
            get {
                return m_list[index]; 
            }
            //set {
            //    m_list[index] = value; 
            //}
        }

        #endregion

        #region ICollection<ILRenderQueueItem> Member

        //public void Add(ILRenderQueueItem item) {
        //    // todo: implement pooling of render queue item objects! 
        //    m_list.Add(item); 
        //}

        public void Clear() {
            // todo: implement pooling of render queue item objects! 
            m_list.Clear(); 
        }

        public bool Contains(ILRenderQueueItem item) {
            return m_list.Contains(item); 
        }

        public void CopyTo(ILRenderQueueItem[] array, int arrayIndex) {
            m_list.CopyTo(array,arrayIndex); 
        }

        public int Count {
            get { return m_list.Count;  }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        //public bool Remove(ILRenderQueueItem item) {
        //    return m_list.Remove(item); 
        //}

        #endregion

        #region IEnumerable<ILRenderQueueItem> Member

        public IEnumerator<ILRenderQueueItem> GetEnumerator() {
            foreach (ILRenderQueueItem item in m_list) 
                yield return item; 
        }

        #endregion

        #region IEnumerable Member

        IEnumerator IEnumerable.GetEnumerator() {
            return m_list.GetEnumerator(); 
        }

        #endregion

    }
}
