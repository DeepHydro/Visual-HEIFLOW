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
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

namespace  Heiflow.AI.SVM
{
    /// <summary>
    /// Encapsulates a problem, or set of vectors which must be classified.
    /// </summary>
	[Serializable]
	public class Problem
	{
        private int _count;
        private double[] _Y;
        private Node[][] _X;
        private int _maxIndex;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="count">Number of vectors</param>
        /// <param name="y">The class labels</param>
        /// <param name="x">Vector data.</param>
        /// <param name="maxIndex">Maximum index for a vector</param>
        public Problem(int count, double[] y, Node[][] x, int maxIndex)
        {
            _count = count;
            _Y = y;
            _X = x;
            _maxIndex = maxIndex;
        }
        /// <summary>
        /// Empty Constructor.  Nothing is initialized.
        /// </summary>
        public Problem()
        {
        }
        /// <summary>
        /// Number of vectors.
        /// </summary>
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
            }
        }
        /// <summary>
        /// Class labels.
        /// </summary>
        public double[] Y
        {
            get
            {
                return _Y;
            }
            set
            {
                _Y = value;
            }
        }
        /// <summary>
        /// Vector data.
        /// </summary>
        public Node[][] X
        {
            get
            {
                return _X;
            }
            set
            {
                _X = value;
            }
        }
        /// <summary>
        /// Maximum index for a vector.
        /// </summary>
        public int MaxIndex
        {
            get
            {
                return _maxIndex;
            }
            set
            {
                _maxIndex = value;
            }
        }

        /// <summary>
        /// Reads a problem from a stream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns>The problem</returns>
        public static Problem Read(Stream stream)
        {
            TemporaryCulture.Start();

            StreamReader input = new StreamReader(stream);
            List<double> vy = new List<double>();
            List<Node[]> vx = new List<Node[]>();
            int max_index = 0;

            while (input.Peek() > -1)
            {
                string[] parts = input.ReadLine().Trim().Split();

                vy.Add(double.Parse(parts[0]));
                int m = parts.Length - 1;
                Node[] x = new Node[m];
                for (int j = 0; j < m; j++)
                {
                    x[j] = new Node();
                    string[] nodeParts = parts[j + 1].Split(':');
                    x[j].Index = int.Parse(nodeParts[0]);
                    x[j].Value = double.Parse(nodeParts[1]);
                }
                if (m > 0)
                    max_index = System.Math.Max(max_index, x[m - 1].Index);
                vx.Add(x);
            }

            TemporaryCulture.Stop();

            return new Problem(vy.Count, vy.ToArray(), vx.ToArray(), max_index);
        }

        /// <summary>
        /// Writes a problem to a stream.
        /// </summary>
        /// <param name="stream">The stream to write the problem to.</param>
        /// <param name="problem">The problem to write.</param>
        public static void Write(Stream stream, Problem problem)
        {
            TemporaryCulture.Start();

            StreamWriter output = new StreamWriter(stream);
            for (int i = 0; i < problem.Count; i++)
            {
                output.Write(problem.Y[i]);
                for (int j = 0; j < problem.X[i].Length; j++)
                    output.Write(" {0}:{1}", problem.X[i][j].Index, problem.X[i][j].Value);
                output.WriteLine();
            }
            output.Flush();

            TemporaryCulture.Stop();
        }

        /// <summary>
        /// Reads a Problem from a file.
        /// </summary>
        /// <param name="filename">The file to read from.</param>
        /// <returns>the Probem</returns>
        public static Problem Read(string filename)
        {
            FileStream input = File.OpenRead(filename);
            try
            {
                return Read(input);
            }
            finally
            {
                input.Close();
            }
        }

        /// <summary>
        /// Writes a problem to a file.   This will overwrite any previous data in the file.
        /// </summary>
        /// <param name="filename">The file to write to</param>
        /// <param name="problem">The problem to write</param>
        public static void Write(string filename, Problem problem)
        {
            FileStream output = File.Open(filename, FileMode.Create);
            try
            {
                Write(output, problem);
            }
            finally
            {
                output.Close();
            }
        }
    }
}