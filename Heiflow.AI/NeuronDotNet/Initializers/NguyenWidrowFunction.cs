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
using  Heiflow.AI.NeuronDotNet.Core.Backpropagation;
using  Heiflow.AI.NeuronDotNet.Core.SOM;

namespace  Heiflow.AI.NeuronDotNet.Core.Initializers
{
    /// <summary>
    /// An <see cref="IInitializer"/> using Nguyen Widrow function.
    /// </summary>
    [Serializable]
    public class NguyenWidrowFunction : IInitializer
    {
        private readonly double outputRange;

        /// <summary>
        /// Gets the output range
        /// </summary>
        /// <value>
        /// The range of values, that network output takes
        /// </value>
        public double OutputRange
        {
            get { return outputRange; }
        }

        /// <summary>
        /// Creates a new NGuyen Widrow Initialization function
        /// </summary>
        public NguyenWidrowFunction()
            : this(1d)
        {
        }

        /// <summary>
        /// Creates a new NGuyen Widrow function using the given output range
        /// </summary>
        /// <param name="outputRange">
        /// the range of values, that output of a neuron can take (i.e. maximum minus minimum)
        /// </param>
        public NguyenWidrowFunction(double outputRange)
        {
            this.outputRange = outputRange;
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
        public NguyenWidrowFunction(SerializationInfo info, StreamingContext context)
        {
            Helper.ValidateNotNull(info, "info");
            this.outputRange = info.GetDouble("outputRange");
        }

        /// <summary>
        /// Populates the serialization info with the data needed to serialize the initializer
        /// </summary>
        /// <param name="info">
        /// The serialization info to populate the data with
        /// </param>
        /// <param name="context">
        /// The serialization context to use
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>info</c> is <c>null</c>
        /// </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Helper.ValidateNotNull(info, "info");
            info.AddValue("outputRange", outputRange);
        }

        /// <summary>
        /// Initializes bias values of activation neurons in the activation layer.
        /// </summary>
        /// <param name="activationLayer">
        /// The activation layer to initialize
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>activationLayer</c> is <c>null</c>
        /// </exception>
        public void Initialize(ActivationLayer activationLayer)
        {
            Helper.ValidateNotNull(activationLayer, "activationLayer");

            int hiddenNeuronCount = 0;
            foreach (IConnector targetConnector in activationLayer.TargetConnectors)
            {
                    hiddenNeuronCount += targetConnector.TargetLayer.NeuronCount;
            }

            double nGuyenWidrowFactor = NGuyenWidrowFactor(activationLayer.NeuronCount, hiddenNeuronCount);

            foreach (ActivationNeuron neuron in activationLayer.Neurons)
            {
                neuron.bias = Helper.GetRandom(-nGuyenWidrowFactor, nGuyenWidrowFactor);
            }
        }

        /// <summary>
        /// Initializes weights of all backpropagation synapses in the backpropagation connector.
        /// </summary>
        /// <param name="connector">
        /// The backpropagation connector to initialize.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>connector</c> is <c>null</c>
        /// </exception>
        public void Initialize(BackpropagationConnector connector)
        {
            Helper.ValidateNotNull(connector, "connector");
            
            double nGuyenWidrowFactor = NGuyenWidrowFactor(
                connector.SourceLayer.NeuronCount, connector.TargetLayer.NeuronCount);

            int synapsesPerNeuron = connector.SynapseCount / connector.TargetLayer.NeuronCount;

            foreach (INeuron neuron in connector.TargetLayer.Neurons)
            {
                int i = 0;
                double[] normalizedVector = Helper.GetRandomVector(synapsesPerNeuron, nGuyenWidrowFactor);
                foreach (BackpropagationSynapse synapse in connector.GetSourceSynapses(neuron))
                {
                    synapse.Weight = normalizedVector[i++];
                }
            }
        }

        /// <summary>
        /// Initializes weights of all spatial synapses in a Kohonen connector.
        /// </summary>
        /// <param name="connector">
        /// The Kohonen connector to initialize.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>connector</c> is <c>null</c>
        /// </exception>
        public void Initialize(KohonenConnector connector)
        {
            Helper.ValidateNotNull(connector, "connector");
            double nGuyenWidrowFactor = NGuyenWidrowFactor(
                connector.SourceLayer.NeuronCount, connector.TargetLayer.NeuronCount);

            int synapsesPerNeuron = connector.SynapseCount / connector.TargetLayer.NeuronCount;

            foreach (INeuron neuron in connector.TargetLayer.Neurons)
            {
                int i = 0;
                double[] normalizedVector = Helper.GetRandomVector(synapsesPerNeuron, nGuyenWidrowFactor);
                foreach (KohonenSynapse synapse in connector.GetSourceSynapses(neuron))
                {
                    synapse.Weight = normalizedVector[i++];
                }
            }
        }

        /// <summary>
        /// Private helper method to calculate Nguyen-Widrow factor
        /// </summary>
        /// <param name="inputNeuronCount">
        /// Number of input neurons
        /// </param>
        /// <param name="hiddenNeuronCount">
        /// Number of hidden neurons
        /// </param>
        /// <returns>
        /// The Nguyen-Widrow factor
        /// </returns>
        private double NGuyenWidrowFactor(int inputNeuronCount, int hiddenNeuronCount)
        {
            return 0.7d * System.Math.Pow(hiddenNeuronCount, (1d / inputNeuronCount)) / outputRange;
        }
    }
}