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
using keytype = System.Int64; 

namespace ILNumerics.Data {
    /// <summary>
    /// AVL balanced search tree
    /// </summary>
    /// <remarks>The tree stores unique keys (type:System.Int64, configurable via build) in a balanced search tree.
    /// <para>The implementation is optimized for random seek access and consecutive write access - if no matching key found. 
    /// Also, the next higher key for a given query can be returned in O(1).</para>
    /// <para>The AVL tree class is not intended to be used directly. It supports the ILNumerics infrastructure (ILMemoryPool) 
    /// internally.</para></remarks>
    [System.Security.SecuritySafeCritical]
    internal class ILAVLTree {

        internal ILAVLTreeNode m_root = getNode();
        private int m_nodeCount = 0;
#if DEBUG
        internal int m_itemCount = 0; 
#endif

        #region caching
        private static readonly int MAX_STACKSIZE = 50; 
        private static readonly ILAVLTreeNode[] s_stack = new ILAVLTreeNode[MAX_STACKSIZE]; 
        private static readonly int[] s_dirs = new int[MAX_STACKSIZE];
        #endregion

        #region public interface 
        /// <summary>
        /// Return number of keys currently existing in the tree
        /// </summary>
        public int Count {
            get { return m_nodeCount; }   
        } 
        /// <summary>
        /// Check and add a key to the collection
        /// </summary>
        /// <param name="key">Key</param>
        /// <remarks>The function makes sure, the key is stored in the tree.</remarks>
        public void Add(keytype key) {
            int depth, added2Side = 0;
            if (FindNode(key, m_root, s_stack, s_dirs, out depth)) {
                // nothing to do - key is already there
                return; 
            } else {
                // parent is the node where the key should be inserted below
                ILAVLTreeNode newNode = getNode(); 
                newNode.key = key; 
                m_nodeCount++; 
                added2Side = s_dirs[depth--]; 
                if (added2Side == -1) { // insert as left child 
                    System.Diagnostics.Debug.Assert(s_stack[depth+1] == null);
                    s_stack[depth].left = newNode; 
                    s_stack[depth].balance--; 
                } else { // insert as right child
                    System.Diagnostics.Debug.Assert(s_stack[depth+1] == null); 
                    s_stack[depth].right = newNode; 
                    s_stack[depth].balance++; 
                }
            }
            // check balances along 's_stack' path -> up to root
            // depth is at parent level now
            ILAVLTreeNode node = s_stack[depth];
            while (depth > 0) {
                if (node.balance == 0) {
                    return; 
                }
                if (node.balance * node.balance == 1) {
                    node = s_stack[--depth]; 
                    node.balance += s_dirs[depth+1]; 
                    continue;
                }
                if (node.balance == 2) {
                    if (node.right.balance >= 0) {
                        // rebalance RR -> simple left rotation
                        ILAVLTreeNode node2 = node.right; 
                        if (s_dirs[depth] == 1) {
                            s_stack[depth-1].right = node2; 
                        } else {
                            s_stack[depth-1].left = node2; 
                        }
                        if (node2.balance == 0) { 
                            node.balance = 1; 
                            node2.balance = -1; 
                        } else {
                            System.Diagnostics.Debug.Assert (node2.balance == 1); 
                            node.balance = 0; 
                            node2.balance = 0;  
                        } 
                        node.right = node2.left; 
                        node2.left = node;
                    } else {
                        // rebalance RL -> double left rotation
                        ILAVLTreeNode node2 = node.right;
                        ILAVLTreeNode node3 = node2.left;
                        if (s_dirs[depth] == 1) {
                            s_stack[depth-1].right = node3; 
                        } else {
                            s_stack[depth-1].left = node3;  
                        }
                        if (node3.balance == -1) {
                            node2.balance = 1; 
                            node.balance = 0;
                        } else if (node3.balance == 1) {
                            node2.balance = 0;
                            node.balance = -1;
                        } else {
                            node2.balance = 0; 
                            node.balance = 0; 
                        }
                        node3.balance = 0; 

                        node.right = node3.left; 
                        node2.left = node3.right; 
                        node3.left = node; 
                        node3.right = node2; 
                    }
                } else { 
                    System.Diagnostics.Debug.Assert(node.balance == -2);
                    if (node.left.balance <= 0) {
                        ILAVLTreeNode node2 = node.left; 
                        // rebalance LL -> simple rotate right
                        if (s_dirs[depth] == 1) {
                            s_stack[depth-1].right = node2; 
                        } else {
                            s_stack[depth-1].left = node2; 
                        }
                        if (node2.balance == 0) {
                            node.balance = -1; 
                            node2.balance = 1; 
                        } else {
                            System.Diagnostics.Debug.Assert (node2.balance == -1); 
                            node.balance = 0; 
                            node2.balance = 0;  
                        }
                        node.left = node2.right; 
                        node2.right = node;
                    } else {
                        // rebalance LR - double right rotation 
                        ILAVLTreeNode node2 = node.left;
                        ILAVLTreeNode node3 = node2.right;
                        if (s_dirs[depth] == 1) {
                            s_stack[depth-1].right = node3; 
                        } else {
                            s_stack[depth-1].left = node3;  
                        }
                        if (node3.balance == 1) {
                            node2.balance = -1; 
                            node.balance = 0; 
                        } else if (node3.balance == -1) {
                            node2.balance = 0; 
                            node.balance = 1;  
                        } else {
                            node2.balance = 0; 
                            node.balance = 0; 
                        }
                        node3.balance = 0; 

                        node.left = node3.right; 
                        node2.right = node3.left; 
                        node3.right = node; 
                        node3.left = node2; 
                    }
                }
#if DEBUG
                //if (!CheckTree()) {
                //    return;
                //}
#endif
                return; 
            }
        }
        /// <summary>
        /// Remove key from tree
        /// </summary>
        /// <param name="key">Key to be removed</param>
        /// <remarks>The function makes sure, the tree does not contain a node with the given key.</remarks>
        public void Remove(keytype key) {
            int depth;
            if (!FindNode(key, m_root, s_stack, s_dirs, out depth)) {
                return; 
            }
#if DEBUG
            int remDepth = depth; 
#endif
            // last entry in stack is the matching node
            ILAVLTreeNode cur = s_stack[depth];
            if (cur.left != null) {
                if (cur.right != null) {
                    // find replacement node 
                    ILAVLTreeNode orig = cur; 
                    cur = cur.left;
                    s_stack[++depth] = cur; 
                    s_dirs[depth] = -1; 
                    while (cur.right != null) {
                        cur = cur.right;
                        s_stack[++depth] = cur; 
                        s_dirs[depth] = 1; 
                    }
                    // exchange keys with current node
                    orig.key = cur.key; 
                    cur = cur.left;     
                } else {
                    cur = cur.left; 
                }
            } else {
                if (cur.right != null) {
                    // replace with right child
                    cur = cur.right; 
                } else {
                    // no childs simply delete cur
                    cur = null; 
                }
            }
            depth--;
            ILAVLTreeNode par = s_stack[depth];
            if (s_dirs[depth+1] == 1) {
                par.right = cur;
                par.balance--; 
            } else {
                par.left = cur;
                par.balance++; 
            }
            m_nodeCount--; 
#if DEBUG
            if (par.left != null) 
                System.Diagnostics.Debug.Assert(par.left != par); 
            if (par.right != null) 
                System.Diagnostics.Debug.Assert(par.right != par); 
         
#endif
            #region rebalance
            cur = par; 
            while (depth > 0) {
                if (cur.balance == 0) {
                    cur = s_stack[--depth];
                    cur.balance -= s_dirs[depth + 1];
                    continue;
                }
                if (cur.balance * cur.balance == 1) {
                    break;
                } else if (cur.balance == 2) {
                    #region needs left rotations
                    if (cur.right.balance >= 0) {
                        // rebalance RR -> simple left rotation
                        ILAVLTreeNode node2 = cur.right;
                        if (s_dirs[depth] == 1) {
                            s_stack[depth - 1].right = node2;
                        } else {
                            s_stack[depth - 1].left = node2;
                        }
                        cur.right = node2.left;
                        node2.left = cur;
                        if (node2.balance == 0) {
                            cur.balance = 1;
                            node2.balance = -1;
                            break; 
                        } else {
                            System.Diagnostics.Debug.Assert(node2.balance == 1);
                            cur.balance = 0;
                            node2.balance = 0;
                        }
                    } else {
                        // rebalance RL -> double left rotation
                        ILAVLTreeNode node2 = cur.right;
                        ILAVLTreeNode node3 = node2.left;
                        if (s_dirs[depth] == 1) {
                            s_stack[depth - 1].right = node3;
                        } else {
                            s_stack[depth - 1].left = node3;
                        }
                        if (node3.balance == -1) {
                            node2.balance = 1;
                            cur.balance = 0;
                        } else if (node3.balance == 1) {
                            node2.balance = 0;
                            cur.balance = -1;
                        } else {
                            node2.balance = 0;
                            cur.balance = 0;
                        }
                        node3.balance = 0;

                        cur.right = node3.left;
                        node2.left = node3.right;
                        node3.left = cur;
                        node3.right = node2;
                    }
                    #endregion
                } else {
                    #region needs right rotation
                    System.Diagnostics.Debug.Assert(cur.balance == -2);
                    if (cur.left.balance <= 0) {
                        ILAVLTreeNode node2 = cur.left;
                        // rebalance LL -> simple rotate right
                        if (s_dirs[depth] == 1) {
                            s_stack[depth - 1].right = node2;
                        } else {
                            s_stack[depth - 1].left = node2;
                        }
                        cur.left = node2.right;
                        node2.right = cur;
                        if (node2.balance == 0) {
                            cur.balance = -1;
                            node2.balance = 1;
                            break; 
                        } else {
                            System.Diagnostics.Debug.Assert(node2.balance == -1);
                            cur.balance = 0;
                            node2.balance = 0;
                        }
                    } else {
                        // rebalance LR - double right rotation 
                        ILAVLTreeNode node2 = cur.left;
                        ILAVLTreeNode node3 = node2.right;
                        if (s_dirs[depth] == 1) {
                            s_stack[depth - 1].right = node3;
                        } else {
                            s_stack[depth - 1].left = node3;
                        }
                        if (node3.balance == 1) {
                            node2.balance = -1;
                            cur.balance = 0;
                        } else if (node3.balance == -1) {
                            node2.balance = 0;
                            cur.balance = 1;
                        } else {
                            node2.balance = 0;
                            cur.balance = 0;
                        }
                        node3.balance = 0;

                        cur.left = node3.right;
                        node2.right = node3.left;
                        node3.right = cur;
                        node3.left = node2;
                    }
                    #endregion
                }
                // populate to parent 
                if (--depth <= 0)
                    break; 
                cur = s_stack[depth];
                cur.balance -= s_dirs[depth + 1];
            }
            #endregion
#if DEBUG
                //if (!CheckTree()) {
                //    return;
                //}
#endif
        }
        /// <summary>
        /// Find next higher key
        /// </summary>
        /// <param name="key">Key to find</param>
        /// <returns>The key with the next higher value compared with <paramref name="key"/>, or -1 if no such key exists</returns>
        public keytype Next(keytype key) {
            if (m_root.right == null) {
                return -1; 
            }
            int level = 0;
            if (!FindNode(key, m_root, s_stack, s_dirs, out level) && level > 0)
                level--; 
            ILAVLTreeNode node = s_stack[level]; 
            if (node.key >= key) return node.key; 
            node = NextAsc(s_stack[level], s_stack, s_dirs, level); 
            if (node != null && node.key > key) 
                return node.key; 
            return -1; 
        }
        /// <summary>
        /// Clear the tree from all keys
        /// </summary>
        internal void Clear() {
            m_root.right = null; 
#if DEBUG
            m_nodeCount = 0;
            m_itemCount = 0; 
#endif
        }
        #endregion

        #region private helper
        private static bool FindNode(keytype key, ILAVLTreeNode cur, ILAVLTreeNode[] path, int[] dirs, out int level) {
            int depth = 0; 
            while (true) {
                path[depth] = cur; 
                //if (cur.isNil) {
                //    //Console.Out.WriteLine("Key:{0} depth: {1} {2}",cur.key, depth, s_dirs[depth] ); 
                //    level = depth;  
                //    return false;  
                //}
                if (cur.key == key) {
                    level = depth; 
                    return true; 
                } else { 
                    if (key < cur.key) {
                        dirs[++depth] = -1; 
                        path[depth] = cur.left; 
                        if (cur.left == null) {
                            level = depth; 
                            return false; 
                        } 
                        cur = cur.left; 
                    } else { // key > cur.key 
                        dirs[++depth] = 1; 
                        path[depth] = cur.right; 
                        if (cur.right == null) {
                            level = depth; 
                            return false; 
                        }
                        cur = cur.right; 
                    }
                }
            }
        }
        private static ILAVLTreeNode NextAsc(ILAVLTreeNode node, ILAVLTreeNode[] path, int[] dirs, int level) {
            ILAVLTreeNode ret = node; 
            if (node.right != null) {
                node = node.right; 
                while (node.left != null) {
                    node = node.left; 
                }
                ret = node; 
            } else {
                while (level > 0) {
                    // dirs[0] is always 0 (dummy root node) 
                    if (dirs[level] == -1) {
                        ret = path[--level];
                        break;
                    }
                    level--;
                }
            }
            return ret; 
        }
        /// <summary>
        /// check the consistency of the tree
        /// </summary>
        /// <returns></returns>
        internal bool CheckTree() {
            if (m_root.right == null)
                return true;
            ILAVLTreeNode cur = m_root.right;
            while (cur.left != null)
                cur = cur.left;
            keytype key = cur.key; 
            int depth;
            ILAVLTreeNode node = cur;
            do {
                Console.Out.Write("checking node: " + node.key); 
                if (FindNode(node.key, m_root, s_stack, s_dirs, out depth)) {
                    if (s_stack[depth] != node) 
                        return false; 
                    if (!checkTree(node)) {
                        return false;
                    }
                    node = NextAsc(node, s_stack, s_dirs, depth); 
                } else {
                    return false;
                }
                Console.Out.WriteLine("... ok"); 
            } while (node != null);
            return true; 
        }
        private static ILAVLTreeNode getNode() {
            var ret = new ILAVLTreeNode();
            ret.key = -1; 
            ret.balance = 0; 
            //ret.right = ret; 
            return ret; 
        }
        private bool checkTree(int key) {
            ILAVLTreeNode[] tmpNodes = new ILAVLTreeNode[50]; 
            int[] tmpKeys = new int[50]; 
            int level;
            if (!FindNode(key, m_root, tmpNodes, tmpKeys, out level)) {
                return false; 
            } 
            return checkTree(tmpNodes[level]); 
        }
        private static bool checkTree(ILAVLTreeNode root) {
            // check balance condition
            int ltree = 0, rtree = 0; 
            if (root.left != null) {
                ltree = levelTree(root.left); 
            } 
            if (root.right != null) {
                rtree = levelTree(root.right); 
            }
            int bal = rtree - ltree;
            if (root.balance != bal) 
                return false; 
            if (bal * bal > 1) {
                return false;   
            }  
     
            return true; 

        }
        private static int levelTree(ILAVLTreeNode root) {
            if (root.left == null && root.right == null) {
                return 1; 
            } else {
                int ltree = 0, rtree = 0; 
                if (root.left != null) {
                    ltree = levelTree(root.left);                 
                }
                if (root.right != null) {
                    rtree = levelTree(root.right); 
                }
                return Math.Max(ltree,rtree) + 1; 
            }
        }
        #endregion
        /// <summary>
        /// an AVL tree node 
        /// </summary>
        public class ILAVLTreeNode {
            /// <summary>
            /// Key
            /// </summary>
            public keytype key; 
            /// <summary>
            /// Right child
            /// </summary>
            public ILAVLTreeNode right;
            /// <summary>
            /// Left child
            /// </summary>
            public ILAVLTreeNode left;
            /// <summary>
            /// Balance factor for the node
            /// </summary>
            public int balance;
            /// <summary>
            /// Rext representation of the node 
            /// </summary>
            /// <returns>String representation for the node</returns>
            public override string ToString() {
                return String.Format("{0} [{1}-{2}]",key,(left == null) ? "*" : left.key.ToString(), (right == null) ? "*" : right.key.ToString());
            }
        }

    }
}
