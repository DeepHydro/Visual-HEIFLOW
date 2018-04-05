//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using System;

namespace  Heiflow.AI.SVM
{
    /// <summary>
    /// Encapsulates a node in a Problem vector, with an index and a value (for more efficient representation
    /// of sparse data.
    /// </summary>
	[Serializable]
	public class Node : IComparable<Node>
	{
        internal int _index;
        internal double _value;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Node()
        {
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="index">The index of the value.</param>
        /// <param name="value">The value to store.</param>
        public Node(int index, double value)
        {
            _index = index;
            _value = value;
        }

        /// <summary>
        /// Index of this Node.
        /// </summary>
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
            }
        }
        /// <summary>
        /// Value at Index.
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// String representation of this Node as {index}:{value}.
        /// </summary>
        /// <returns>{index}:{value}</returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}", _index, _value);
        }

        #region IComparable<Node> Members

        /// <summary>
        /// Compares this node with another.
        /// </summary>
        /// <param name="other">The node to compare to</param>
        /// <returns>A positive number if this node is greater, a negative number if it is less than, or 0 if equal</returns>
        public int CompareTo(Node other)
        {
            return _index.CompareTo(other._index);
        }

        #endregion
    }
}