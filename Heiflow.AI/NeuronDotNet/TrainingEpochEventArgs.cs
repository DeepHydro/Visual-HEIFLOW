// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;

namespace  Heiflow.AI.NeuronDotNet.Core
{
    /// <summary>
    /// Training Epoch Event Handler. This delegate handles events invoked whenever a training epoch
    /// starts or ends
    /// </summary>
    /// <param name="sender">
    /// The sender invoking the event
    /// </param>
    /// <param name="e">
    /// Event Arguments
    /// </param>
    public delegate void TrainingEpochEventHandler(object sender, TrainingEpochEventArgs e);

    /// <summary>
    /// Training Epoch Event Arguments
    /// </summary>
    public class TrainingEpochEventArgs : EventArgs
    {
        private int trainingIteration;
        private TrainingSet trainingSet;

        /// <summary>
        /// Gets the current training iteration
        /// </summary>
        /// <value>
        /// Current Training Iteration.
        /// </value>
        public int TrainingIteration
        {
            get { return trainingIteration; }
        }

        /// <summary>
        /// Gets the training set associated
        /// </summary>
        /// <value>
        /// Training set associated with the event
        /// </value>
        public TrainingSet TrainingSet
        {
            get { return trainingSet; }
        }

        /// <summary>
        /// Creates a new instance of training epoch event arguments
        /// </summary>
        /// <param name="trainingIteration">
        /// Current training iteration
        /// </param>
        /// <param name="trainingSet">
        /// The training set associated with the event
        /// </param>
        public TrainingEpochEventArgs(int trainingIteration, TrainingSet trainingSet)
        {
            this.trainingSet = trainingSet;
            this.trainingIteration = trainingIteration;
        }
    }
}
