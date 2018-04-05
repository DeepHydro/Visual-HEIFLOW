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
