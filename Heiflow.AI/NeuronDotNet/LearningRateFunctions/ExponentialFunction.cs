// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Runtime.Serialization;

namespace  Heiflow.AI.NeuronDotNet.Core.LearningRateFunctions
{
    /// <summary>
    /// Exponential Learning Rate Function. As training progresses, The learning rate exponentially
    /// changes from its initial value to the final value.
    /// </summary>
    [Serializable]
    public sealed class ExponentialFunction : AbstractFunction
    {
        private readonly double logFinalByInitial;

        /// <summary>
        /// Constructs a new instance of the exponential function with the specified initial and
        /// final values of learning rate.
        /// </summary>
        /// <param name="initialLearningRate">
        /// Initial value learning rate
        /// </param>
        /// <param name="finalLearningRate">
        /// Final value learning rate
        /// </param>
        public ExponentialFunction(double initialLearningRate, double finalLearningRate)
            : base(initialLearningRate, finalLearningRate)
        {
            logFinalByInitial
                = System.Math.Log(System.Math.Max(initialLearningRate, initialLearningRate + 1e-4)
                / System.Math.Max(finalLearningRate, finalLearningRate + 1e-4));
        }

        /// <summary>
        /// Deserialization Constructor
        /// </summary>
        /// <param name="info">
        /// Serialization information to deserialize and obtain the data
        /// </param>
        /// <param name="context">
        /// Serialization context to use
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// if <c>info</c> is <c>null</c>
        /// </exception>
        public ExponentialFunction(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            logFinalByInitial
                = System.Math.Log(System.Math.Max(initialLearningRate, initialLearningRate + 1e-4)
                / System.Math.Max(finalLearningRate, finalLearningRate + 1e-4));
        }

        /// <summary>
        /// Gets effective learning rate for current training iteration. (As training progresses, The
        /// learning rate exponentially changes from its initial value to the final value)
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
        /// <exception cref="ArgumentException">
        /// If <c>trainingEpochs</c> is zero or negative
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <c>currentIteration</c> is negative or, if it is not less than <c>trainingEpochs</c>
        /// </exception>
        public override double GetLearningRate(int currentIteration, int trainingEpochs)
        {
            Helper.ValidatePositive(trainingEpochs, "trainingEpochs");
            Helper.ValidateWithinRange(currentIteration, 0, trainingEpochs - 1, "currentIteration");

            return initialLearningRate * System.Math.Exp((logFinalByInitial * currentIteration) / trainingEpochs);
        }
    }
}