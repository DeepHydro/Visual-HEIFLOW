// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Runtime.Serialization;
using  Heiflow.AI.NeuronDotNet.Core.Backpropagation;
using  Heiflow.AI.NeuronDotNet.Core.SOM;

namespace  Heiflow.AI.NeuronDotNet.Core.Initializers
{
    /// <summary>
    /// An <see cref="IInitializer"/> using Normalized Random function.
    /// </summary>
    public class NormalizedRandomFunction : IInitializer
    {
        /// <summary>
        /// Creates a new normalized random function
        /// </summary>
        public NormalizedRandomFunction()
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
        public NormalizedRandomFunction(SerializationInfo info, StreamingContext context)
        {
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
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
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

            int i = 0;
            double[] normalized = Helper.GetRandomVector(activationLayer.NeuronCount, 1d);
            foreach (ActivationNeuron neuron in activationLayer.Neurons)
            {
                neuron.bias = normalized[i++];
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

            int i = 0;
            double[] normalized = Helper.GetRandomVector(connector.SynapseCount, 1d);
            foreach (BackpropagationSynapse synapse in connector.Synapses)
            {
                synapse.Weight = normalized[i++];
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

            int i = 0;
            double[] normalized = Helper.GetRandomVector(connector.SynapseCount, 1d);
            foreach (KohonenSynapse synapse in connector.Synapses)
            {
                synapse.Weight = normalized[i++];
            }
        }
    }
}
