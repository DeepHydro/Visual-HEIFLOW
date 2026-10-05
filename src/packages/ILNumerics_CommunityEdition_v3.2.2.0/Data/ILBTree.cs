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
using keytype = System.Int32; 

namespace ILNumerics.Data {
    /// <summary>
    /// incomplete BTree implementation (it lags of a remove method)
    /// </summary>
    /// <typeparam name="T">value element type</typeparam>
    public class ILBTree<T> {

        //private static readonly int ORDER = 4;   // max number of childs
        //private static readonly int MAX_KEYS = 3;  // uneven ! (order - 1) 
        //private static readonly int MIN_CHILDS = 2;  
        //private static readonly int MIN_KEYS = 1;  // uneven ! (order / 2 - 1)        
        private static int m_order = 24; 
        public static int ORDER  {
            get { return m_order; }
            set { 
                if (value / 2 * 2 != value) {
                    value++; 
                }
                m_order = value; 
                MAX_KEYS = value -1; 
                MIN_KEYS = value / 2 - 1; 
            }
        }   // max number of childs

        public static int MAX_KEYS = 15;  // uneven ! (order - 1) 
        public static int MIN_KEYS = 7;  // uneven ! (order / 2 - 1)
        public static int DEFAULT_LIST_CAPACITY = 8; 

        private int m_nodeCount = 0; 
        private int m_levelCount = 1;
        
        private nodetype m_root; 

        //private class itemtype {
        //    public keytype Key; 
        //    public List<T> Values;  
        //    //private itemtype () {
        //    //    Key = -1;
        //    //    Values = new LinkedList<T>(); 
        //    //}
        //    public itemtype (keytype key, T value) {
        //        Key = key;
        //        Values = new List<T>(8); 
        //        Values.Add(value);  
        //    }
        //    public override string ToString() {
        //        return String.Format("K:{0} Count:{1}",Key,Values.Count); 
        //    }
        //}
        private class nodetype {
            //public itemtype[] Items; 
            public int[] Keys; 
            public List<T>[] Values; 
            public nodetype[] Childs; 
            /// <summary>
            /// number of keys in the node
            /// </summary>
            public int Count; 

            public nodetype() {
                Count = 0;
                //Items = new itemtype[ MAX_KEYS ];
                Keys = new int[MAX_KEYS]; 
                Values = new List<T>[MAX_KEYS]; 
                Childs = new nodetype[ MAX_KEYS + 1 ];
            }
            public override string ToString() {
                string itemDesc = "---";
                if (Count > 0) {
                    itemDesc = String.Format("{0} - {1}",Keys[0],Keys[Count-1]); 
                }
                return String.Format("{0} Items:[{1}]",Count,itemDesc); 
            }

        }

        #region constructor
        public ILBTree() {
            m_root = new nodetype(); 
            m_nodeCount = 1; 
        }
        #endregion

        #region public interface

        public int NodeCount {
            get { return m_nodeCount; }
        }
        public int LevelCount {
            get { return m_levelCount; }
        }


        public void Add(keytype key, T value) {
            if (m_root.Count >= MAX_KEYS) {
                nodetype newright = new nodetype(); 
                keytype upkey; 
                List<T> upval;
                SplitNode(m_root,newright, out upkey, out upval); 
                nodetype newRoot = new nodetype(); 
                newRoot.Childs[0] = m_root; 
                newRoot.Childs[1] = newright; 
                newRoot.Keys[0] = upkey; 
                newRoot.Values[0] = upval; 
                newRoot.Count = 1; 
                m_root = newRoot;
                m_nodeCount += 2; 
                m_levelCount++; 
            }
            AddValue(key, value, m_root); 
        }
        #endregion
        
        #region private helpers 
        /// <summary>
        ///  is the key contained in curNode ?
        /// </summary>
        /// <param name="curNode"></param>
        /// <param name="key"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private unsafe static bool SearchNode(nodetype curNode, keytype key, ref int location) {
            int loc = 0, max = curNode.Count; int[] curNodeKeys = curNode.Keys; 
            for (; loc < max; loc++) {
                int curKey = curNodeKeys[loc];
                if (curKey == key) {
                    location = loc; 
                    return true; 
                } else if (curKey > key) {
                    location = loc; 
                    return false; 
                }
            }
            location = loc; 
            return false; 

            //int loc = 0, max = curNode.Count;
            //foreach (keytype curKey in curNode.Keys) {
            //    if (curKey == key) {
            //        location = loc; 
            //        return true; 
            //    } else if (curKey > key) {
            //        location = loc; 
            //        return false; 
            //    }
            //    if (++loc >= max) break; 
            //}
            //location = loc; 
            //return false; 

            //int loc = curNode.Count - 1;
            //fixed (int* keys = curNode.Keys) {
            //    int* curKey = keys + loc;
            //    do {
            //        if (*curKey == key) {
            //            location = loc; 
            //            return true; 
            //        } else if (*curKey < key) {
            //            location = ++loc; 
            //            return false; 
            //        }
            //        curKey--;
            //    } while (loc --> 0); 
            //}
            //location = 0; 
            //return false; 
        }
        private void AddValue(keytype key, T value, nodetype curNode) {
            System.Diagnostics.Debug.Assert(curNode.Count < MAX_KEYS); 
            while (true) {
                // search node (inlined from func) 
                short max = (short)curNode.Count; 
                short location = 0;
                int[] curNodeKeys = curNode.Keys; 
                for (; location < max; location++) {
                    int curKey = curNodeKeys[location];
                    if (curKey == key) {
                        curNode.Values[location].Add(value); 
                        return; 
                    }
                    if (curKey > key) {
                        break; 
                    }
                }
                nodetype childNode = curNode.Childs[location]; 
                if (childNode == null) {
                    // insert right here
                    var newVals = new List<T>(8);
                    newVals.Add(value); 
                    InsertItemIntoNode(curNode,key,newVals,location,null);  
                    break; 
                } else {
                    // check if the child is filled up allready
                    if (childNode.Count >= MAX_KEYS) {
                        nodetype newRightChild = new nodetype(); 
                        int upkey; 
                        List<T> upval; 
                        SplitNode(childNode, newRightChild, out upkey, out upval); 
                        m_nodeCount++; 
                        InsertItemIntoNode(curNode,upkey, upval, location, newRightChild);  // cur node should always have space left! 
                        // in which subtree to go? 
                        if (key == upkey) {
                            curNode.Values[location].Add(value); 
                            return; 
                        }
                        if (key > upkey) 
                            location++; 
                    }
                    //AddValue(key, value, curNode.Childs[location]); 
                    curNode = curNode.Childs[location]; 
                }
            }
        }

        //private void AddValue(keytype key, T value, nodetype curNode) {
        //    System.Diagnostics.Debug.Assert(curNode.Count < MAX_KEYS); 
        //    int location; 
        //    if (SearchNode(curNode, key, out location)) {
        //        curNode.Values[location].Add(value); 
        //    } else {
        //        nodetype childNode = curNode.Childs[location]; 
        //        if (childNode == null) {
        //            // insert right here
        //            var newVals = new List<T>(8);
        //            newVals.Add(value); 
        //            InsertItemIntoNode(curNode,key,newVals,location,null);  
        //        } else {
        //            // check if the child is filled up allready
        //            if (childNode.Count >= MAX_KEYS) {
        //                nodetype newRightChild = new nodetype(); 
        //                int upkey; 
        //                List<T> upval; 
        //                SplitNode(childNode, newRightChild, out upkey, out upval); 
        //                m_nodeCount++; 
        //                InsertItemIntoNode(curNode,upkey, upval, location, newRightChild);  // cur node should always have space left! 
        //                // in which subtree to go? 
        //                if (key == upkey) {
        //                    curNode.Values[location].Add(value); 
        //                    return; 
        //                }
        //                if (key > upkey) 
        //                    location++; 
        //            }
        //            AddValue(key, value, curNode.Childs[location]); 
        //        }
        //    }
        //}

        #region break 
        #endregion

        //private void AddValue(keytype key, T value, nodetype curNode, ref itemtype upcomingItem, ref nodetype rightchild) {
        //    if (curNode == null) {
        //        // we came from a leaf! let the parent handle the insertion
        //        upcomingItem = new itemtype(key,value); 
        //        rightchild = null; 
        //        return; 
        //    }
        //    System.Diagnostics.Debug.Assert(curNode.Count < MAX_KEYS); 
        //    int location; 
        //    if (SearchNode(curNode, key, out location)) {
        //        curNode.Items[location].Values.AddFirst(value); 
        //        upcomingItem = null; 
        //    } else {
        //        nodetype childNode = curNode.Childs[location]; 
        //        if (childNode.Count >= MAX_KEYS) {
        //            nodetype newRightChild = new nodetype(); 
        //            itemtype newUpItem; 
        //            SplitNode(childNode, newRightChild, out newUpItem); 
        //            InsertItemIntoNode(curNode,newUpItem,location,newRightChild);  // cur node should always have space left! 
        //            // in which subtree to go? 
        //            if (key > newUpItem.Key) 
        //                location++; 
        //            if (key == newUpItem.Key) {
        //                curNode.Items[location].Values.AddFirst(value); 
        //                upcomingItem = null; 
        //                return; 
        //            }
        //        }
        //        // location = index of child/ insertion point
        //        AddValue(key, value, curNode.Childs[location], ref upcomingItem, ref rightchild); 
        //        // handle upcoming node
        //        if (upcomingItem != null) {
        //            if (curNode.Count < MAX_KEYS) {
        //                // insert right here
        //                InsertItemIntoNode(curNode, upcomingItem, location, rightchild); 
        //                upcomingItem = null; 
        //            } else {
        //                // split the node and move median up
        //                SplitNode(curNode, upcomingItem, location, ref upcomingItem, ref rightchild); 
        //            }
        //        }
        //    }
        //}

        private void SplitNode(nodetype curNode, nodetype newRight, out keytype upKey, out List<T> upval) {
            System.Diagnostics.Debug.Assert(Math.Round(curNode.Count / 2.0) != curNode.Count / 2.0);
            System.Diagnostics.Debug.Assert(curNode.Count == (MIN_KEYS * 2 + 1));
            System.Diagnostics.Debug.Assert(curNode.Count == MAX_KEYS);
            System.Diagnostics.Debug.Assert(newRight != null && newRight.Childs != null && newRight.Keys != null);
            System.Diagnostics.Debug.Assert(newRight.Childs.Length == ORDER && newRight.Keys.Length == MAX_KEYS && newRight.Values.Length == MAX_KEYS);
            for (int i = MIN_KEYS + 1, ni = 0; ni < MIN_KEYS; ni++, i++) {
                newRight.Keys[ni] = curNode.Keys[i]; 
                newRight.Values[ni] = curNode.Values[i]; 
                newRight.Childs[ni+1] = curNode.Childs[i+1]; 
                // clear old data
                curNode.Keys[i] = -1; 
                curNode.Values[i] = null; 
                curNode.Childs[i+1] = null; 
            }
            curNode.Count = MIN_KEYS;
            newRight.Count = MIN_KEYS; 
            newRight.Childs[0] = curNode.Childs[MIN_KEYS + 1]; 
            upKey = curNode.Keys[MIN_KEYS]; 
            upval = curNode.Values[MIN_KEYS]; 
            // clear old data
            curNode.Keys[MIN_KEYS] = -1; 
            curNode.Values[MIN_KEYS] = null; 
            curNode.Childs[MIN_KEYS + 1] = null; 
        }

        //private void SplitNode(nodetype curNode, itemtype newItem, int location, ref itemtype upcomingItem, ref nodetype upcomingrightnode) {
        //    var newnode = new nodetype();
        //    if (location <= MIN_KEYS) {
        //        // copy right node  
        //        for (int ci = MIN_KEYS, ni = 0; ci < MAX_KEYS; ci++, ni++ ) {
        //            newnode.Items[ni] = curNode.Items[ci]; 
        //            newnode.Childs[ni+1] = curNode.Childs[ci]; 
        //        }
        //        // copy curNode 
        //        for (int ci = MIN_KEYS; ci --> location; ) {
        //            curNode.Items[ci+1] = curNode.Items[ci]; 
        //            curNode.Childs[ci+1] = curNode.Childs[ci]; 
        //        }
        //        curNode.Count = MIN_KEYS + 1; 
        //        newnode.Count = MAX_KEYS - MIN_KEYS; 
        //        // insert new item  
        //        curNode.Items[location] = newItem; 
        //        curNode.Childs[location] = upcomingrightnode; 
        //        newnode.Childs[0] = curNode.Childs[MIN_KEYS]; 
        //        // give the median back upwards
        //        upcomingItem = curNode.Items[MIN_KEYS]; 
        //        upcomingrightnode = newnode; 
        //    } else {
        //        // copy right node -> up to insert location
        //        int i = MIN_KEYS, ni = 0; 
        //        for (; i < location; i++, ni++) {
        //            newnode.Items[ni] = curNode.Items[i]; 
        //            newnode.Childs[ni+1] = curNode.Childs[i]; 
        //        }
        //        // insert new item
        //        newnode.Items[ni] = newItem; 
        //        newnode.Childs[ni++] = upcomingrightnode; 
        //        // copy rest of node to right side
        //        for (; i < curNode.Count; i++, ni++) {
        //            newnode.Items[ni] = curNode.Items[i]; 
        //            newnode.Childs[ni+1] = curNode.Childs[i]; 
        //        }
        //        // update curNode
        //        curNode.Count = MIN_KEYS + 1; 
        //        newnode.Count = ni; 
        //        newnode.Childs[0] = curNode.Childs[MIN_KEYS + 1]; 
        //        upcomingItem = curNode.Items[MIN_KEYS + 1]; 
        //        upcomingrightnode = newnode; 
        //    }
        //    m_nodeCount++; 
        //}

        private void InsertItemIntoNode(nodetype curNode, keytype newKey,List<T> newvalue, int location, nodetype rigthchild) {
            int a = curNode.Count; 
            for (; a --> location; ) {
                curNode.Keys[a+1] = curNode.Keys[a];
                curNode.Values[a+1] = curNode.Values[a];
                curNode.Childs[a+2] = curNode.Childs[a+1]; 
            }
            // insert a new linked list at freed position
            curNode.Keys[location] = newKey;
            curNode.Values[location] = newvalue; 
            curNode.Childs[location+1] = rigthchild; 
            curNode.Count++; 
        }

        #endregion

    }
}
