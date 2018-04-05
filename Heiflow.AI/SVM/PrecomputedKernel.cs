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
using System.Collections.Generic;

namespace  Heiflow.AI.SVM
{
    /// <summary>
    /// Class encapsulating a precomputed kernel, where each position indicates the similarity score for two items in the training data.
    /// </summary>
    [Serializable]
    public class PrecomputedKernel
    {
        private float[,] _similarities;
        private int _rows;
        private int _columns;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="similarities">The similarity scores between all items in the training data</param>
        public PrecomputedKernel(float[,] similarities)
        {
            _similarities = similarities;
            _rows = _similarities.GetLength(0);
            _columns = _similarities.GetLength(1);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nodes">Nodes for self-similarity analysis</param>
        /// <param name="param">Parameters to use when computing similarities</param>
        public PrecomputedKernel(List<Node[]> nodes, Parameter param)
        {
            _rows = nodes.Count;
            _columns = _rows;
            _similarities = new float[_rows, _columns];
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < r; c++)
                    _similarities[r, c] = _similarities[c, r];
                _similarities[r, r] = 1;
                for (int c = r + 1; c < _columns; c++)
                    _similarities[r, c] = (float)Kernel.KernelFunction(nodes[r], nodes[c], param);
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rows">Nodes to use as the rows of the matrix</param>
        /// <param name="columns">Nodes to use as the columns of the matrix</param>
        /// <param name="param">Parameters to use when compute similarities</param>
        public PrecomputedKernel(List<Node[]> rows, List<Node[]> columns, Parameter param)
        {
            _rows = rows.Count;
            _columns = columns.Count;
            _similarities = new float[_rows, _columns];
            for (int r = 0; r < _rows; r++)
                for (int c = 0; c < _columns; c++)
                    _similarities[r, c] = (float)Kernel.KernelFunction(rows[r], columns[c], param);
        }

        /// <summary>
        /// Constructs a <see cref="Problem"/> object using the labels provided.  If a label is set to "0" that item is ignored.
        /// </summary>
        /// <param name="rowLabels">The labels for the row items</param>
        /// <param name="columnLabels">The labels for the column items</param>
        /// <returns>A <see cref="Problem"/> object</returns>
        public Problem Compute(double[] rowLabels, double[] columnLabels)
        {
            List<Node[]> X = new List<Node[]>();
            List<double> Y = new List<double>();
            int maxIndex = 0;
            for (int i = 0; i < columnLabels.Length; i++)
                if (columnLabels[i] != 0)
                    maxIndex++;
            maxIndex++;
            for (int r = 0; r < _rows; r++)
            {
                if (rowLabels[r] == 0)
                    continue;
                List<Node> nodes = new List<Node>();
                nodes.Add(new Node(0, X.Count + 1));
                for (int c = 0; c < _columns; c++)
                {
                    if (columnLabels[c] == 0)
                        continue;
                    double value = _similarities[r, c];
                    nodes.Add(new Node(nodes.Count, value));
                }
                X.Add(nodes.ToArray());
                Y.Add(rowLabels[r]);
            }
            return new Problem(X.Count, Y.ToArray(), X.ToArray(), maxIndex);
        }

    }
}
