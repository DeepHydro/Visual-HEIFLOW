// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI.NeuronDotNet.Core.Backpropagation
{
    /// <summary>
    /// Backpropagation synapse connects two activation neurons. A typical backpropagation network
    /// contains thousands of such synapses.
    /// </summary>
    public class BackpropagationSynapse : ISynapse
    {
        private double weight;
        private double delta;
        private readonly ActivationNeuron sourceNeuron;
        private readonly ActivationNeuron targetNeuron;
        private readonly BackpropagationConnector parent;

        /// <summary>
        /// Gets the source neuron
        /// </summary>
        /// <value>
        /// The source neuron of the synapse. It is never <c>null</c>.
        /// </value>
        public ActivationNeuron SourceNeuron
        {
            get { return sourceNeuron; }
        }

        /// <summary>
        /// Gets the target neuron
        /// </summary>
        /// <value>
        /// The target neuron of the synapse. It is never <c>null</c>.
        /// </value>
        public ActivationNeuron TargetNeuron
        {
            get { return targetNeuron; }
        }

        INeuron ISynapse.SourceNeuron
        {
            get { return sourceNeuron; }
        }

        INeuron ISynapse.TargetNeuron
        {
            get { return targetNeuron; }
        }

        /// <summary>
        /// Gets or sets the weight of the synapse
        /// </summary>
        /// <value>
        /// Weight of the synapse
        /// </value>
        public double Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        /// <summary>
        /// Gets the parent connector
        /// </summary>
        /// <value>
        /// Parent connector containing this synapse. It is never <c>null</c>.
        /// </value>
        public BackpropagationConnector Parent
        {
            get { return parent; }
        }

        IConnector ISynapse.Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Creates a new Backpropagation Synapse connecting the given neurons
        /// </summary>
        /// <param name="sourceNeuron">
        /// The source neuron
        /// </param>
        /// <param name="targetNeuron">
        /// The target neuron
        /// </param>
        /// <param name="parent">
        /// Parent connector containing this syanpse
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If any of the arguments is <c>null</c>.
        /// </exception>
        public BackpropagationSynapse(
            ActivationNeuron sourceNeuron, ActivationNeuron targetNeuron, BackpropagationConnector parent)
        {
            Helper.ValidateNotNull(sourceNeuron, "sourceNeuron");
            Helper.ValidateNotNull(targetNeuron, "targetNeuron");
            Helper.ValidateNotNull(parent, "parent");

            this.weight = 1f;
            this.delta = 0f;

            sourceNeuron.TargetSynapses.Add(this);
            targetNeuron.SourceSynapses.Add(this);

            this.sourceNeuron = sourceNeuron;
            this.targetNeuron = targetNeuron;
            this.parent = parent;
        }

        /// <summary>
        /// Propagates the information from source neuron to target neuron
        /// </summary>
        public void Propagate()
        {
            targetNeuron.input += sourceNeuron.output * this.weight;
        }

        /// <summary>
        /// Optimizes the weight using back propagation algorithm to minimize the error
        /// </summary>
        /// <param name="learningFactor">
        /// Effective learning factor (A function of learning rate, training progress and other parameters)
        /// </param>
        public void OptimizeWeight(double learningFactor)
        {
            delta = delta * parent.momentum + learningFactor * targetNeuron.error * sourceNeuron.output;
            weight += delta;
        }

        /// <summary>
        /// Back-propagates the error from target neuron to source neuron
        /// </summary>
        public void Backpropagate()
        {
            sourceNeuron.error += targetNeuron.error * this.weight;
        }

        /// <summary>
        /// Adds small random noise to weight of this synapse so that the network deviates from
        /// its local optimum position (a local equilibrium state where further learning is of
        /// no use)
        /// </summary>
        /// <param name="jitterNoiseLimit">
        /// Maximum absolute limit to the random noise added
        /// </param>
        public void Jitter(double jitterNoiseLimit)
        {
            weight += Helper.GetRandom(-jitterNoiseLimit, jitterNoiseLimit) ;
        }
    }
}