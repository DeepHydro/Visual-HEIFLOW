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
    /// An <see cref="IInitializer"/> using constant function
    /// </summary>
    [Serializable]
    public class ConstantFunction : IInitializer
    {
        private readonly double constant;

        /// <summary>
        /// Gets the initializer Constant
        /// </summary>
        /// <value>
        /// The constant with which each parameter (bias values and weights) is initialized
        /// </value>
        public double Constant
        {
            get { return constant; }
        }

        /// <summary>
        /// Creates a new constant function
        /// </summary>
        /// <param name="constant">
        /// The constant to use
        /// </param>
        public ConstantFunction(double constant)
        {
            this.constant = constant;
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
        public ConstantFunction(SerializationInfo info, StreamingContext context)
        {
            Helper.ValidateNotNull(info, "info");
            constant = info.GetDouble("constant");
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
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Helper.ValidateNotNull(info, "info");
            info.AddValue("constant", constant);
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
            Helper.ValidateNotNull(activationLayer, "layer");
            foreach (ActivationNeuron neuron in activationLayer.Neurons)
            {
                neuron.bias = constant;
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
            foreach (BackpropagationSynapse synapse in connector.Synapses)
            {
                synapse.Weight = constant;
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
            foreach (KohonenSynapse synapse in connector.Synapses)
            {
                synapse.Weight = constant;
            }
        }
    }
}