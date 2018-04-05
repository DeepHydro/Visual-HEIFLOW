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
using System.Threading;
using System.Globalization;

namespace  Heiflow.AI.SVM
{
    /// <summary>
    /// Class which encapsulates a range transformation.
    /// </summary>
    public class RangeTransform : IRangeTransform
    {
        /// <summary>
        /// Default lower bound for scaling (-1).
        /// </summary>
        public const int DEFAULT_LOWER_BOUND = -1;
        /// <summary>
        /// Default upper bound for scaling (1).
        /// </summary>
        public const int DEFAULT_UPPER_BOUND = 1;

        /// <summary>
        /// Determines the Range transform for the provided problem.  Uses the default lower and upper bounds.
        /// </summary>
        /// <param name="prob">The Problem to analyze</param>
        /// <returns>The Range transform for the problem</returns>
        public static RangeTransform Compute(Problem prob)
        {
            return Compute(prob, DEFAULT_LOWER_BOUND, DEFAULT_UPPER_BOUND);
        }
        /// <summary>
        /// Determines the Range transform for the provided problem.
        /// </summary>
        /// <param name="prob">The Problem to analyze</param>
        /// <param name="lowerBound">The lower bound for scaling</param>
        /// <param name="upperBound">The upper bound for scaling</param>
        /// <returns>The Range transform for the problem</returns>
        public static RangeTransform Compute(Problem prob, double lowerBound, double upperBound)
        {
            double[] minVals = new double[prob.MaxIndex];
            double[] maxVals = new double[prob.MaxIndex];
            for (int i = 0; i < prob.MaxIndex; i++)
            {
                minVals[i] = double.MaxValue;
                maxVals[i] = double.MinValue;
            }
            for (int i = 0; i < prob.Count; i++)
            {
                for (int j = 0; j < prob.X[i].Length; j++)
                {
                    int index = prob.X[i][j].Index - 1;
                    double value = prob.X[i][j].Value;
                    minVals[index] = System.Math.Min(minVals[index], value);
                    maxVals[index] = System.Math.Max(maxVals[index], value);
                }
            }
            for (int i = 0; i < prob.MaxIndex; i++)
            {
                if (minVals[i] == double.MaxValue || maxVals[i] == double.MinValue)
                {
                    minVals[i] = 0;
                    maxVals[i] = 0;
                }
            }
            return new RangeTransform(minVals, maxVals, lowerBound, upperBound);
        }

        private double[] _inputStart;
        private double[] _inputScale;
        private double _outputStart;
        private double _outputScale;
        private int _length;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="minValues">The minimum values in each dimension.</param>
        /// <param name="maxValues">The maximum values in each dimension.</param>
        /// <param name="lowerBound">The desired lower bound for all dimensions.</param>
        /// <param name="upperBound">The desired upper bound for all dimensions.</param>
        public RangeTransform(double[] minValues, double[] maxValues, double lowerBound, double upperBound)
        {
            _length = minValues.Length;
            if(maxValues.Length != _length)
                throw new Exception("Number of max and min values must be equal.");
            _inputStart = new double[_length];
            _inputScale = new double[_length];
            for (int i = 0; i < _length; i++)
            {
                _inputStart[i] = minValues[i];
                _inputScale[i] = maxValues[i] - minValues[i];
            }
            _outputStart = lowerBound;
            _outputScale = upperBound - lowerBound;
        }
        private RangeTransform(double[] inputStart, double[] inputScale, double outputStart, double outputScale, int length)
        {
            _inputStart = inputStart;
            _inputScale = inputScale;
            _outputStart = outputStart;
            _outputScale = outputScale;
            _length = length;
        }
        /// <summary>
        /// Transforms the input array based upon the values provided.
        /// </summary>
        /// <param name="input">The input array</param>
        /// <returns>A scaled array</returns>
        public Node[] Transform(Node[] input)
        {
            Node[] output = new Node[input.Length];
            for (int i = 0; i < output.Length; i++)
            {
                int index = input[i].Index;
                double value = input[i].Value;
                output[i] = new Node(index, Transform(value, index));
            }
            return output;
        }

        /// <summary>
        /// Transforms this an input value using the scaling transform for the provided dimension.
        /// </summary>
        /// <param name="input">The input value to transform</param>
        /// <param name="index">The dimension whose scaling transform should be used</param>
        /// <returns>The scaled value</returns>
        public double Transform(double input, int index)
        {
            index--;
            double tmp = input - _inputStart[index];
            if (_inputScale[index] == 0)
                return 0;
            tmp /= _inputScale[index];
            tmp *= _outputScale;
            return tmp + _outputStart;
        }
        /// <summary>
        /// Writes this Range transform to a stream.
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <param name="r">The range to write</param>
        public static void Write(Stream stream, RangeTransform r)
        {
            TemporaryCulture.Start();

            StreamWriter output = new StreamWriter(stream);
            output.WriteLine(r._length);
            output.Write(r._inputStart[0]);
            for(int i=1; i<r._inputStart.Length; i++)
                output.Write(" " + r._inputStart[i]);
            output.WriteLine();
            output.Write(r._inputScale[0]);
            for (int i = 1; i < r._inputScale.Length; i++)
                output.Write(" " + r._inputScale[i]);
            output.WriteLine();
            output.WriteLine("{0} {1}", r._outputStart, r._outputScale);
            output.Flush();

            TemporaryCulture.Stop();
        }

        /// <summary>
        /// Writes this Range transform to a file.    This will overwrite any previous data in the file.
        /// </summary>
        /// <param name="outputFile">The file to write to</param>
        /// <param name="r">The Range to write</param>
        public static void Write(string outputFile, RangeTransform r)
        {
            FileStream s = File.Open(outputFile, FileMode.Create);
            try
            {
                Write(s, r);
            }
            finally
            {
                s.Close();
            }
        }

        /// <summary>
        /// Reads a Range transform from a file.
        /// </summary>
        /// <param name="inputFile">The file to read from</param>
        /// <returns>The Range transform</returns>
        public static RangeTransform Read(string inputFile)
        {
            FileStream s = File.OpenRead(inputFile);
            try
            {
                return Read(s);
            }
            finally
            {
                s.Close();
            }
        }

        /// <summary>
        /// Reads a Range transform from a stream.
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <returns>The Range transform</returns>
        public static RangeTransform Read(Stream stream)
        {
            TemporaryCulture.Start();

            StreamReader input = new StreamReader(stream);
            int length = int.Parse(input.ReadLine());
            double[] inputStart = new double[length];
            double[] inputScale = new double[length];
            string[] parts = input.ReadLine().Split();
            for (int i = 0; i < length; i++)
                inputStart[i] = double.Parse(parts[i]);
            parts = input.ReadLine().Split();
            for (int i = 0; i < length; i++)
                inputScale[i] = double.Parse(parts[i]);
            parts = input.ReadLine().Split();
            double outputStart = double.Parse(parts[0]);
            double outputScale = double.Parse(parts[1]);

            TemporaryCulture.Stop();

            return new RangeTransform(inputStart, inputScale, outputStart, outputScale, length);
        }
    }
}
