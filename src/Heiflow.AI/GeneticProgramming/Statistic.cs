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

namespace  Heiflow.AI.GeneticProgramming
{
    /// <summary>
    /// Set of statistics functions
    /// </summary>
    /// 
    /// <remarks>The class represents collection of functions used
    /// in statistics</remarks>
    /// 
    public class Statistics
    {
        public static double Mean(double[] values)
        {
            // for all values
            double sum = 0;
            for (int i = 0, n = values.Length; i < n; i++)
            {
                sum += values[i]; 
            }
            return sum / (double)values.Length;
        }

        /// <summary>
        /// Standard deviation of discreate values
        /// </summary>
        public static double StdDev(double[] values)
        {
            double mean = Mean(values);
            double stddev = 0;
            
            // for all values
            for (int i = 0, n = values.Length; i < n; i++)
            {
                stddev += System.Math.Pow(values[i] - mean, 2); 
            }

            return System.Math.Sqrt(stddev / values.Length - 1);
        }
        /// <summary>
        ///Square of Standard deviation -Variance 
        /// </summary>
        public static double StdDev_Square(double[] values)
        {
            double mean = Mean(values);
            double stddev = 0;

            // for all values
            for (int i = 0, n = values.Length; i < n; i++)
            {
                stddev += System.Math.Pow(values[i] - mean, 2);
            }

            return (stddev / values.Length-1);
        }

        /// <summary>
        /// Skewness is a measure of symmetry, or more precisely, 
        /// the lack of symmetry. A distribution, or TrainingData set, 
        /// is symmetric if it looks the same to the left and right of the center point. 
        /// </summary>
        public static double Skewness(double[] values)
        {
            double stdDev = StdDev(values);
            double mean = Mean(values);
            double b = 0;

            // for all values
            for (int i = 0, n = values.Length; i < n; i++)
            {
                b += System.Math.Pow(values[i] - mean, 3) / stdDev * stdDev * stdDev;
            }

            return (b / values.Length - 1);
        }
        /// <summary>
        /// Kurtosis is a measure of whether the TrainingData are peaked or flat relative
        /// to a normal distribution. 
        /// </summary>
        public static double Kurtosis(double[] values)
        {
            double stdDev = StdDev(values);
            double mean = Mean(values);
            double b = 0;

            // for all values
            for (int i = 0, n = values.Length; i < n; i++)
            {
                b += System.Math.Pow(values[i] - mean, 4) / stdDev * stdDev * stdDev * stdDev;
            }

            return (b / values.Length - 1);
        }
        
    }
}
