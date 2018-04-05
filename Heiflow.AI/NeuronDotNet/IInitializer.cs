// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System.Runtime.Serialization;
using  Heiflow.AI.NeuronDotNet.Core.Backpropagation;
using  Heiflow.AI.NeuronDotNet.Core.SOM;

namespace  Heiflow.AI.NeuronDotNet.Core
{
    /// <summary>
    /// Initializer interface. An initializer should define initialization methods for all concrete
    /// initializable layers and connectors.
    /// </summary>
    public interface IInitializer : ISerializable
    {
        /// <summary>
        /// Initializes bias values of activation neurons in an activation layer.
        /// </summary>
        /// <param name="activationLayer">
        /// The activation layer to initialize
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If <c>activationLayer</c> is <c>null</c>
        /// </exception>
        void Initialize(ActivationLayer activationLayer);

        /// <summary>
        /// Initializes weights of all backpropagation synapses in a backpropagation connector.
        /// </summary>
        /// <param name="connector">
        /// The backpropagation connector to initialize.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If <c>connector</c> is <c>null</c>
        /// </exception>
        void Initialize(BackpropagationConnector connector);

        /// <summary>
        /// Initializes weights of all spatial synapses in a Kohonen connector.
        /// </summary>
        /// <param name="connector">
        /// The Kohonen connector to initialize.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If <c>connector</c> is <c>null</c>
        /// </exception>
        void Initialize(KohonenConnector connector);
    }
}