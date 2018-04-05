// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System.Runtime.Serialization;

namespace  Heiflow.AI.NeuronDotNet.Core
{
    /// <summary>
    /// Learning Rate Function interface. This interface defines the way in which learning rate
    /// changes from its initial value to its final value as the training progresses.
    /// </summary>
    public interface ILearningRateFunction : ISerializable
    {
        /// <summary>
        /// Gets the initial value of learning rate
        /// </summary>
        /// <value>
        /// Initial Learning Rate
        /// </value>
        double InitialLearningRate { get; }

        /// <summary>
        /// Gets the final value of learning rate
        /// </summary>
        /// <value>
        /// Final Learning Rate
        /// </value>
        double FinalLearningRate { get; }

        /// <summary>
        /// Gets effective learning rate for current training iteration. No validation is performed
        /// on the arguments.
        /// </summary>
        /// <param name="currentIteration">
        /// Current training iteration
        /// </param>
        /// <param name="trainingEpochs">
        /// Total number of training epochs
        /// </param>
        /// <returns>
        /// The effective learning rate for current training iteration
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// If <c>trainingEpochs</c> is zero or negative
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If <c>currentIteration</c> is negative or, if it is not less than <c>trainingEpochs</c>
        /// </exception>
        double GetLearningRate(int currentIteration, int trainingEpochs);
    }
}