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

using System;
using System.Runtime.Serialization;

namespace  Heiflow.AI.NeuronDotNet.Core.SOM
{
    /// <summary>
    /// This class extends a <see cref="Network"/> and represents a Kohonen Self-Organizing Map.
    /// </summary>
    [Serializable]
    public class KohonenNetwork : Network
    {
        /// <summary>
        /// Gets the winner neuron of the network
        /// </summary>
        /// <value>
        /// Winner Neuron
        /// </value>
        public PositionNeuron Winner
        {
            get { return (outputLayer as KohonenLayer).Winner; }
        }

        /// <summary>
        /// Creates a new Kohonen SOM, with the specified input and output layers. (You are required
        /// to connect all layers using appropriate synapses, before using the constructor. Any changes
        /// made to the structure of the network here-after, may lead to complete malfunctioning of the
        /// network)
        /// </summary>
        /// <param name="inputLayer">
        /// The input layer
        /// </param>
        /// <param name="outputLayer">
        /// The output layer
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>inputLayer</c> or <c>outputLayer</c> is <c>null</c>
        /// </exception>
        public KohonenNetwork(ILayer inputLayer, KohonenLayer outputLayer)
            : base(inputLayer, outputLayer, TrainingMethod.Unsupervised)
        {
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
        /// If <c>info</c> is <c>null</c>
        /// </exception>
        public KohonenNetwork(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// A protected helper function used to train single learning sample
        /// </summary>
        /// <param name="trainingSample">
        /// Training sample to use
        /// </param>
        /// <param name="currentIteration">
        /// Current training epoch (Assumed to be positive and less than <c>trainingEpochs</c>)
        /// </param>
        /// <param name="trainingEpochs">
        /// Number of training epochs (Assumed to be positive)
        /// </param>
        protected override void LearnSample(TrainingSample trainingSample, int currentIteration, int trainingEpochs)
        {
            // No validation here
            inputLayer.SetInput(trainingSample.InputVector);
            foreach (ILayer layer in layers)
            {
                layer.Run();
                layer.Learn(currentIteration, trainingEpochs);
            }
        }
    }
}