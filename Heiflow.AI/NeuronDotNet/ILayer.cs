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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace  Heiflow.AI.NeuronDotNet.Core
{
    /// <summary>
    /// This interface represents a Layer in a neural network. A layer is a container for similar
    /// neurons. No two neurons within a layer can be connected to each other.
    /// </summary>
    public interface ILayer : ISerializable
    {
        /// <summary>
        /// Gets the neuron count
        /// </summary>
        /// <value>
        /// Number of neurons in the layer. It is always positive.
        /// </value>
        int NeuronCount { get; }

        /// <summary>
        /// Exposes an enumerator to iterate over all neurons in the layer
        /// </summary>
        /// <value>
        /// Neurons Enumerator. No neuron enumerated can be <c>null</c>.
        /// </value>
        IEnumerable<INeuron> Neurons { get; }

        /// <summary>
        /// Neuron Indexer
        /// </summary>
        /// <param name="index">
        /// Index
        /// </param>
        /// <returns>
        /// Neuron at the given index
        /// </returns>
        /// <exception cref="System.IndexOutOfRangeException">
        /// If index is out of range
        /// </exception>
        INeuron this[int index] { get; }

        /// <summary>
        /// Gets the list of source connectors
        /// </summary>
        /// <value>
        /// The list of source connectors associated with this layer. It is neither <c>null</c>,
        /// nor contains <c>null</c> elements.
        /// </value>
        IList<IConnector> SourceConnectors { get; }

        /// <summary>
        /// Gets the list of target connectors
        /// </summary>
        /// <value>
        /// The list of target connectors associated with this layer. It is neither <c>null</c>,
        /// nor contains <c>null</c> elements.
        /// </value>
        IList<IConnector> TargetConnectors { get; }

        /// <summary>
        /// Gets or sets the Initializer used to initialize the layer
        /// </summary>
        /// <value>
        /// Initializer used to initialize the layer. If this value is <c>null</c>, initialization
        /// is NOT performed.
        /// </value>
        IInitializer Initializer { get; set; }

        /// <summary>
        /// Gets the initial value of learning rate
        /// </summary>
        /// <value>
        /// Initial value of learning rate.
        /// </value>
        double LearningRate { get; }

        /// <summary>
        /// Gets the learning rate function
        /// </summary>
        /// <value>
        /// Learning Rate Function used while training. It is never <c>null</c>
        /// </value>
        ILearningRateFunction LearningRateFunction { get; }

        /// <summary>
        /// Initializes all neurons and makes them ready to undergo training freshly.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Sets the learning rate to the given value. The layer will use this constant value
        /// as learning rate throughout the learning process
        /// </summary>
        /// <param name="learningRate">
        /// The learning rate
        /// </param>
        void SetLearningRate(double learningRate);

        /// <summary>
        /// Sets the initial and final values for learning rate. During the learning process, the
        /// effective learning rate uniformly changes from its initial value to final value
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
        /// Sets neuron inputs to the values specified by the given array
        /// </summary>
        /// <param name="input">
        /// The input array
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If <c>input</c> is <c>null</c>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// If length of <c>input</c> array is different from number of neurons
        /// </exception>
        void SetInput(double[] input);

        /// <summary>
        /// Gets the neuron outputs as an array
        /// </summary>
        /// <returns>
        /// Array of double values representing neuron outputs
        /// </returns>
        double[] GetOutput();

        /// <summary>
        /// Runs all neurons in the layer.
        /// </summary>
        void Run();

        /// <summary>
        /// All neurons and their are source connectors are allowed to learn. This method assumes a
        /// learning environment where inputs, outputs and other parameters (if any) are appropriate.
        /// </summary>
        /// <param name="currentIteration">
        /// Current learning iteration
        /// </param>
        /// <param name="trainingEpochs">
        /// Total number of training epochs
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// If <c>trainingEpochs</c> is zero or negative
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If <c>currentIteration</c> is negative or, if it is greater than <c>trainingEpochs</c>
        /// </exception>
        void Learn(int currentIteration, int trainingEpochs);
    }
}