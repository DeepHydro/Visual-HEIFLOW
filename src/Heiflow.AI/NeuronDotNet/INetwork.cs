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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace  Heiflow.AI.NeuronDotNet.Core
{
    /// <summary>
    /// <para>
    /// This interface represents a neural network. A  A typical neural network consists of a set of
    /// <see cref="ILayer"/>s acyclically interconnected by various <see cref="IConnector"/>s. Input
    /// layer gets the input from the user and network output is obtained from the output layer.
    /// </para>
    /// <para>
    /// To create a neural network, follow these steps
    /// <list type="bullet">
    /// <item>Create and customize layers</item>
    /// <item>Establish connections between layers (No cycles should exist)</item>
    /// <item>Construct Network specifying the desired input and output layers</item>
    /// </list>
    /// </para>
    /// <para>
    /// There are two modes in which a neural network can be trained. In 'Batch Training', the neural
    /// network is allowed to learn by specifying a predefined training set containing various training
    /// samples. In 'Online training mode', a random training sample is generated every time (usually
    /// by another neural network, called 'teacher' network) and is used for training. Both modes are
    /// supported by overloaded <c>Learn()</c> methods. <c>Run()</c> method is used to run a neural
    /// network against a particular input.
    /// </para>
    /// </summary>
    public interface INetwork : ISerializable
    {
        /// <summary>
        /// Gets the input layer of the network
        /// </summary>
        /// <value>
        /// Input Layer of the network. This property is never <c>null</c>.
        /// </value>
        ILayer InputLayer { get; }

        /// <summary>
        /// Gets the output layer of the network
        /// </summary>
        /// <value>
        /// Output Layer of the network. This property is never <c>null</c>.
        /// </value>
        ILayer OutputLayer { get; }

        /// <summary>
        /// Gets the number of layers in the network.
        /// </summary>
        /// <value>
        /// Layer Count. This value is always positive.
        /// </value>
        int LayerCount { get; }

        /// <summary>
        /// Exposes an enumerator to iterate over layers in the network.
        /// </summary>
        /// <value>
        /// Layer Enumerator. No layer in the network can be <c>null</c>.
        /// </value>
        IEnumerable<ILayer> Layers { get; }

        /// <summary>
        /// Layer Indexer
        /// </summary>
        /// <param name="index">
        /// The index
        /// </param>
        /// <returns>
        /// Layer at the given index
        /// </returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// If the index is out of range
        /// </exception>
        ILayer this[int index] { get; }

        /// <summary>
        /// Gets the number of connectors in the network.
        /// </summary>
        /// <value>
        /// Connector Count. This value is never negative.
        /// </value>
        int ConnectorCount { get; }

        /// <summary>
        /// Exposes an enumerator to iterate over connectors in the network. 
        /// </summary>
        /// <value>
        /// Connector Enumerator. No connector in a network can be <c>null</c>.
        /// </value>
        IEnumerable<IConnector> Connectors { get; }

        /// <summary>
        /// Gets or sets maximum absolute limit to the jitter noise
        /// </summary>
        /// <value>
        /// Maximum absolute limit to the random noise added while <c>Jitter</c>
        /// </value>
        double JitterNoiseLimit { get; set; }

        /// <summary>
        /// Gets or sets the jitter epoch
        /// </summary>
        /// <value>
        /// The epoch (interval) at which jitter is performed. If this value is not positive, no
        /// jitter is performed.
        /// </value>
        int JitterEpoch { get; set; }

        /// <summary>
        /// This event is invoked during the commencement of a new training iteration during 'Batch
        /// training' mode.
        /// </summary>
        event TrainingEpochEventHandler BeginEpochEvent;

        /// <summary>
        /// This event is invoked whenever the network is about to learn a training sample.
        /// </summary>
        event TrainingSampleEventHandler BeginSampleEvent;

        /// <summary>
        /// This event is invoked whenever the network has successfully completed learning a training
        /// sample.
        /// </summary>
        event TrainingSampleEventHandler EndSampleEvent;

        /// <summary>
        /// This event is invoked whenever a training iteration is successfully completed during 'Batch
        /// training' mode.
        /// </summary>
        event TrainingEpochEventHandler EndEpochEvent;

        /// <summary>
        /// Sets the learning rate to the given value. All layers in the network will use this constant
        /// value as learning rate during the learning process.
        /// </summary>
        /// <param name="learningRate">
        /// The learning rate
        /// </param>
        void SetLearningRate(double learningRate);

        /// <summary>
        /// Sets the initial and final values for learning rate. During the learning process, all
        /// layers in the network will use an efeective learning rate which varies uniformly from
        /// the initial value to the final value.
        /// </summary>
        /// <param name="initialLearningRate">
        /// Initial value of learning rate
        /// </param>
        /// <param name="finalLearningRate">
        /// Final value of learning rate
        /// </param>
        void SetLearningRate(double initialLearningRate, double finalLearningRate);

        /// <summary>
        /// Sets the learning rate function.
        /// </summary>
        /// <param name="learningRateFunction">
        /// Learning rate function to use.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If <c>learningRateFunction</c> is <c>null</c>
        /// </exception>
        void SetLearningRate(ILearningRateFunction learningRateFunction);

        /// <summary>
        /// Initializes all layers and connectors and makes them ready to undergo fresh training.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Runs the neural network against the given input
        /// </summary>
        /// <param name="input">
        /// Input to the network
        /// </param>
        /// <returns>
        /// The output of the network
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// If input array is <c>null</c>
        /// </exception>
        double[] Run(double[] input);

        /// <summary>
        /// Trains the neural network for the given training set (Batch Training)
        /// </summary>
        /// <param name="trainingSet">
        /// The training set to use
        /// </param>
        /// <param name="trainingEpochs">
        /// Number of training epochs. (All samples are trained in some random order, in every
        /// training epoch)
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// if <c>trainingSet</c> is <c>null</c>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// if <c>trainingEpochs</c> is zero or negative
        /// </exception>
        void Learn(TrainingSet trainingSet, int trainingEpochs);

        /// <summary>
        /// Trains the network for the given training sample (Online training mode). Note that this
        /// method trains the sample only once, irrespective of what current epoch is. The arguments
        /// are just used to evaluate training progress and adjust parameter values depending on it.
        /// </summary>
        /// <param name="trainingSample">
        /// Training sample to use
        /// </param>
        /// <param name="currentIteration">
        /// Current training iteration
        /// </param>
        /// <param name="trainingEpochs">
        /// Number of training epochs
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If <c>trainingSample</c> is <c>null</c>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// If <c>trainingEpochs</c> is not positive
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If <c>currentIteration</c> is negative or, if it is not less than <c>trainingEpochs</c>
        /// </exception>
        void Learn(TrainingSample trainingSample, int currentIteration , int trainingEpochs);

        /// <summary>
        /// If the network is currently learning, this method stops the learning.
        /// </summary>
        void StopLearning();
    }
}