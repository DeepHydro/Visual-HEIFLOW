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
