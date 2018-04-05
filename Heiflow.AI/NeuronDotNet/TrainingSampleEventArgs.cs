// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;

namespace  Heiflow.AI.NeuronDotNet.Core
{
    /// <summary>
    /// Training Sample Event Handler. This is used by events associated with training samples.
    /// </summary>
    /// <param name="sender">
    /// The sender invoking the event
    /// </param>
    /// <param name="e">
    /// Event Arguments
    /// </param>
    public delegate void TrainingSampleEventHandler(object sender, TrainingSampleEventArgs e);

    /// <summary>
    /// Training Sample Event Arguments. This class represents arguments for an event associated
    /// with a training sample
    /// </summary>
    public class TrainingSampleEventArgs : EventArgs
    {
        private int trainingIteration;
        private TrainingSample trainingSample;

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
        /// Gets the training sample associated with the event
        /// </summary>
        /// <value>
        /// Training sample associated with the event
        /// </value>
        public TrainingSample TrainingSample
        {
            get { return trainingSample; }
        }

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        /// <param name="trainingIteration">
        /// Current training iteration
        /// </param>
        /// <param name="trainingSample">
        /// The training sample associated with the event
        /// </param>
        public TrainingSampleEventArgs(int trainingIteration, TrainingSample trainingSample)
        {
            this.trainingIteration = trainingIteration;
            this.trainingSample = trainingSample;
        }
    }
}
