// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
