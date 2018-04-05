// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI.Neuro.Learning
{
    using System;

    /// <summary>
    /// Supervised learning interface.
    /// </summary>
    /// 
    /// <remarks><para>The interface describes methods, which should be implemented
    /// by all supervised learning algorithms. Supervised learning is such
    /// type of learning algorithms, where system's desired output is known on
    /// the learning stage. So, given sample input values and desired outputs,
    /// system should adopt its internals to produce correct (or close to correct)
    /// result after the learning step is complete.</para></remarks>
    /// 
    public interface ISupervisedLearning
    {
        /// <summary>
        /// Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// <param name="output">Desired output vector.</param>
        /// 
        /// <returns>Returns learning error.</returns>
        /// 
        double Run( double[] input, double[] output );

        /// <summary>
        /// Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        /// <param name="output">Array of output vectors.</param>
        /// 
        /// <returns>Returns sum of learning errors.</returns>
        /// 
        double RunEpoch( double[][] input, double[][] output );
    }
}
