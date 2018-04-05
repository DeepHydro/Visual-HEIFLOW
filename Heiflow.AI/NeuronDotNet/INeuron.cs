// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System.Collections.Generic;

namespace  Heiflow.AI.NeuronDotNet.Core
{
    /// <summary>
    /// Interface representing a neuron. A neuron is a basic building block of a neural network.
    /// </summary>
    public interface INeuron
    {
        /// <summary>
        /// Gets or sets the neuron input.
        /// </summary>
        /// <value>
        /// Input to the neuron. For input neurons, this value is specified by user, whereas other
        /// neurons will have their inputs updated when the source synapses propagate
        /// </value>
        double Input { get; set; }

        /// <summary>
        /// Gets the output of the neuron.
        /// </summary>
        /// <value>
        /// Neuron Output
        /// </value>
        double Output { get; }

        /// <summary>
        /// Gets the list of source synapses associated with this neuron
        /// </summary>
        /// <value>
        /// A list of source synapses. It can neither be <c>null</c>, nor contain <c>null</c> elements.
        /// </value>
        IList<ISynapse> SourceSynapses { get; }

        /// <summary>
        /// Gets the list of target synapses associated with this neuron
        /// </summary>
        /// <value>
        /// A list of target synapses. It can neither be <c>null</c>, nor contains <c>null</c> elements.
        /// </value>
        IList<ISynapse> TargetSynapses { get; }

        /// <summary>
        /// Gets the parent layer containing this neuron
        /// </summary>
        /// <value>
        /// The parent layer containing this neuron. It is never <c>null</c>
        /// </value>
        ILayer Parent { get; }

        /// <summary>
        /// Runs the neuron. (Propagates the source synapses and update input and output values)
        /// </summary>
        void Run();

        /// <summary>
        /// Trains various parameters associated with this neuron and associated source synapses.
        /// </summary>
        /// <param name="learningRate">
        /// The current learning rate (this depends on training progress as well)
        /// </param>
        void Learn(double learningRate);
    }
}
