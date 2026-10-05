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
using System.Text;
using ILNumerics.Algorithms; 
using System.Collections; 

namespace ILNumerics.Misc {
    /// <summary>
    /// List items to be used in ILQueueList
    /// </summary>
    /// <typeparam name="T1">Data type</typeparam>
    /// <typeparam name="T2">Index type</typeparam>
    /// <remarks>This class is not intended to be used directly. Internally it serves as a helper class for sorting.</remarks>
    [System.Security.SecuritySafeCritical]
    public class ILListItem<T1, T2> {
        internal T1 Data; 
        internal T2 m_index; 
        internal ILListItem<T1,T2> Next; 
        /// <summary>
        /// construct list item by data
        /// </summary>
        /// <param name="item">item data</param>
        /// <remarks>the indet will be set to its default value</remarks>
        public ILListItem(T1 item) {
            Data = item; 
            m_index = default(T2); 
        }
        /// <summary>
        /// construct list item, takes item data and index
        /// </summary>
        /// <param name="item">item data</param>
        /// <param name="index">index</param>
        public ILListItem(T1 item, T2 index) {
            Data = item; 
            m_index = index; 
        }
        /// <summary>
        /// index stored with this item
        /// </summary>
        public T2 Index {
            get {
                return m_index; 
            } 
            set {
                m_index = value; 
            }
        }
    }
    /// <summary>
    /// Queuelist - a queue with partial list properties 
    /// </summary>
    /// <typeparam name="T1">data type</typeparam>
    /// <typeparam name="T2">index type</typeparam>
    /// <remarks>This class is not intended to be used directly. Internally it serves as a helper class for sorting.</remarks>
    [System.Security.SecuritySafeCritical]
    public class ILQueueList<T1, T2> : IEnumerable<T1> {
        internal ILListItem<T1,T2> Head;  
        internal ILListItem<T1,T2> Tail;  
        internal int m_count = 0; 
        /// <summary>
        /// number of items currently in the queue (readonly)
        /// </summary>
        public int Count {                    
            get {
                return m_count; 
            }
        }

        /// <summary>
        /// add indexed item at end of queue
        /// </summary>
        public void Enqueue(T1 item, T2 index) {
            if (Head == null) {
                Head = new ILListItem<T1,T2>(item, index); 
                Tail = Head; 
            } else {
                ILListItem<T1,T2> newItem = new ILListItem<T1,T2>(item,index); 
                Tail.Next = newItem; 
                Tail = newItem; 
            }
            m_count++; 
        }

        /// <summary>
        /// add item at end of queue
        /// </summary>
        public void Enqueue(T1 item) {
            if (Head == null) {
                Head = new ILListItem<T1,T2>(item); 
                Tail = Head; 
            } else {
                ILListItem<T1,T2> newItem = new ILListItem<T1,T2>(item); 
                Tail.Next = newItem; 
                Tail = newItem; 
            }
            m_count++; 
        }
        /// <summary>
        /// add item at end of queue
        /// </summary>
        public void Enqueue(ILListItem<T1,T2> item) {
            System.Diagnostics.Debug.Assert(item.Next == null); 
            if (Head == null) {
                Head = item; 
                Tail = item; 
            } else {
                Tail.Next = item; 
                Tail = item; 
            }
            m_count++; 
        }
        /// <summary>
        /// add queue list to end of this queue list
        /// </summary>
        /// <param name="list">queue list to be added</param>
        public void Enqueue(ILQueueList<T1,T2> list) {
            if (list == null || list.Count == 0) return; 
            if (Tail != null) {
                Tail.Next = list.Head; 
                Tail = list.Tail; 
            } else {
                Head = list.Head; 
                Tail = list.Tail; 
            }
            m_count += list.m_count; 
        }
        
        /// <summary>
        /// Remove from start of queue
        /// </summary>
        /// <returns>item from start of queue</returns>
        public ILListItem<T1,T2> Dequeue() {
            switch (m_count) {
                case 0: 
                    return null; 
                case 1:
                    ILListItem<T1,T2> retIt = Head;
                    Head = null; 
                    Tail = null; 
                    m_count--; 
                    System.Diagnostics.Debug.Assert(retIt.Next == null);
                    return retIt; 
                default: 
                    retIt = Head;
                    Head = Head.Next; 
                    m_count--; 
                    retIt.Next = null; 
                    return retIt; 
            }
        }

        /// <summary>
        /// Add to start of queue
        /// </summary>
        /// <param name="item">item data to add to start of queue</param>
        public void AddToStart(T1 item) {
            ILListItem<T1,T2> newLItem = new ILListItem<T1,T2>(item); 
            newLItem.Next = Head;
            Head = newLItem; 
            m_count++; 
        }
        /// <summary>
        /// concatenate 2 queuelists
        /// </summary>
        /// <param name="qlist">queue list to be added at start of this queuelist</param>
        public void AddToStart(ILQueueList<T1,T2> qlist) {
            m_count += qlist.m_count; 
            qlist.Tail.Next = Head; 
            Head = qlist.Head; 
        }
        /// <summary>
        /// sort utilizing bucket sort
        /// </summary>
        /// <typeparam name="SubelementType">subelement type</typeparam>
        /// <param name="mapper">keymapper mapping subelement items to buckets</param>
        public void Sort<SubelementType>(ILKeyMapper<T1,SubelementType> mapper) {
            ILQueueList<T1,T2> ret = ILBucketSort.BucketSort<T1,SubelementType,T2>(this,null,mapper,ILBucketSort.SortMethod.ConstantLength); 
            this.Head = ret.Head;
            this.Tail = ret.Tail; 
        }
        /// <summary>
        /// convert (copy) items to system array
        /// </summary>
        /// <returns>system array with items</returns>
        public T1[] ToArray() {
            T1[] ret = new T1[m_count];
            ILListItem<T1,T2> curr = Head; 
            for (int i = 0; curr != null; i++) {
                ret[i] = curr.Data; 
                curr = curr.Next; 
            }
            return ret; 
        }
        /// <summary>
        /// Clear this queue list from all elements 
        /// </summary>
        public void Clear() {
            Head = null; 
            Tail = null; 
            m_count = 0; 
        }

        #region IEnumerable<T> Member
        /// <summary>
        /// Create enumerator utilizing 'foreach'
        /// </summary>
        /// <returns>enumerator for contained elements</returns>
        public IEnumerator<T1> GetEnumerator() {
            ILListItem<T1,T2> curr = Head; 
            while (curr != null) { 
                yield return curr.Data; 
                curr = curr.Next; 
            }
        }
        /// <summary>
        /// Gives enumerator for contained items (ILListItem)
        /// </summary>
        public IEnumerable<ILListItem<T1,T2>> ListItems {
            get {
                ILListItem<T1,T2> curr = Head; 
                while (curr != null) { 
                    yield return curr; 
                    curr = curr.Next; 
                }
            }
        }
        #endregion

        #region IEnumerable Member
        /// <summary>
        /// gives enumerator for internal list items (ILListItem)
        /// </summary>
        /// <returns>ILListItem's</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            ILListItem<T1,T2> curr = Head; 
            while (curr != null) {
                yield return curr; 
                curr = curr.Next;
            }
        }

        #endregion
    }       
}
