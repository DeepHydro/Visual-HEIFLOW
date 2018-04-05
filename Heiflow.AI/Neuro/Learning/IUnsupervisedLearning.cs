// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI.Neuro.Learning
{
    using System;

    /// <summary>
    /// Unsupervised learning interface.
    /// </summary>
    /// 
    /// <remarks><para>The interface describes methods, which should be implemented
    /// by all unsupervised learning algorithms. Unsupervised learning is such
    /// type of learning algorithms, where system's desired output is not known on
    /// the learning stage. Given sample input values, it is expected, that
    /// system will organize itself in the way to find similarities betweed provided
    /// samples.</para></remarks>
    /// 
    public interface IUnsupervisedLearning
    {
        /// <summary>
        /// Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns learning error.</returns>
        /// 
        double Run( double[] input );

        /// <summary>
        /// Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        ///
        /// <returns>Returns sum of learning errors.</returns>
        /// 
        double RunEpoch( double[][] input );
    }
}
