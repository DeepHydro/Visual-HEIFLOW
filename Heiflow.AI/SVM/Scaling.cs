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
    /// Deals with the scaling of Problems so they have uniform ranges across all dimensions in order to
    /// result in better  Heiflow.AI.SVM performance.
    /// </summary>
    public static class Scaling
    {
        /// <summary>
        /// Scales a problem using the provided range.  This will not affect the parameter.
        /// </summary>
        /// <param name="prob">The problem to scale</param>
        /// <param name="range">The Range transform to use in scaling</param>
        /// <returns>The Scaled problem</returns>
        public static Problem Scale(this IRangeTransform range, Problem prob)
        {
            Problem scaledProblem = new Problem(prob.Count, new double[prob.Count], new Node[prob.Count][], prob.MaxIndex);
            for (int i = 0; i < scaledProblem.Count; i++)
            {
                scaledProblem.X[i] = new Node[prob.X[i].Length];
                for (int j = 0; j < scaledProblem.X[i].Length; j++)
                    scaledProblem.X[i][j] = new Node(prob.X[i][j].Index, range.Transform(prob.X[i][j].Value, prob.X[i][j].Index));
                scaledProblem.Y[i] = prob.Y[i];
            }
            return scaledProblem;
        }
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
        public static RangeTransform DetermineRange(Problem prob)
        {
            return DetermineRangeTransform(prob, DEFAULT_LOWER_BOUND, DEFAULT_UPPER_BOUND);
        }
        /// <summary>
        /// Determines the Range transform for the provided problem.
        /// </summary>
        /// <param name="prob">The Problem to analyze</param>
        /// <param name="lowerBound">The lower bound for scaling</param>
        /// <param name="upperBound">The upper bound for scaling</param>
        /// <returns>The Range transform for the problem</returns>
        public static RangeTransform DetermineRangeTransform(Problem prob, double lowerBound, double upperBound)
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
    }
}
