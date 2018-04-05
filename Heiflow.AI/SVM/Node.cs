// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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