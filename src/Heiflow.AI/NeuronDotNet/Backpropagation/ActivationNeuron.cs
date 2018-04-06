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

namespace  Heiflow.AI.NeuronDotNet.Core.Backpropagation
{
    /// <summary>
    /// Activation Neuron is a buiding block of a back-propagation neural network.
    /// </summary>
    public class ActivationNeuron : INeuron
    {
        internal double input;
        internal double output;
        internal double error;
        internal double bias;

        private readonly IList<ISynapse> sourceSynapses = new List<ISynapse>();
        private readonly IList<ISynapse> targetSynapses = new List<ISynapse>();

        private ActivationLayer parent;

        /// <summary>
        /// Gets or sets the neuron input.
        /// </summary>
        /// <value>
        /// Input to the neuron. For input neurons, this value is specified by user, whereas other
        /// neurons will have their inputs updated when the source synapses propagate
        /// </value>
        public double Input
        {
            get { return input; }
            set { input = value; }
        }

        /// <summary>
        /// Gets the output of the neuron.
        /// </summary>
        /// <value>
        /// Neuron Output
        /// </value>
        public double Output
        {
            get { return output; }
        }

        /// <summary>
        /// Gets the neuron error
        /// </summary>
        /// <value>
        /// Neuron Error
        /// </value>
        public double Error
        {
            get { return error; }
        }

        /// <summary>
        /// Gets the neuron bias
        /// </summary>
        /// <value>
        /// The current value of neuron bias
        /// </value>
        public double Bias
        {
            get { return bias; }
        }

        /// <summary>
        /// Gets the list of source synapses associated with this neuron
        /// </summary>
        /// <value>
        /// A list of source synapses. It can neither be <c>null</c>, nor contain <c>null</c> elements.
        /// </value>
        public IList<ISynapse> SourceSynapses
        {
            get { return sourceSynapses; }
        }

        /// <summary>
        /// Gets the list of target synapses associated with this neuron
        /// </summary>
        /// <value>
        /// A list of target synapses. It can neither be <c>null</c>, nor contains <c>null</c> elements.
        /// </value>
        public IList<ISynapse> TargetSynapses
        {
            get { return targetSynapses; }
        }

        /// <summary>
        /// Gets the parent layer containing this neuron
        /// </summary>
        /// <value>
        /// The parent layer containing this neuron. It is never <c>null</c>
        /// </value>
        public ActivationLayer Parent
        {
            get { return parent; }
        }

        ILayer INeuron.Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Create a new activation neuron
        /// </summary>
        /// <param name="parent">
        /// The parent layer containing this neuron
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If <c>parent</c> is <c>null</c>
        /// </exception>
        public ActivationNeuron(ActivationLayer parent)
        {
            Helper.ValidateNotNull(parent, "parent");

            this.input = 0d;
            this.output = 0d;
            this.error = 0d;
            this.bias = 0d;
            this.parent = parent;
        }

        /// <summary>
        /// Obtains input from source synapses and activates to update the output
        /// </summary>
        public void Run()
        {
            if (sourceSynapses.Count > 0)
            {
                input = 0d;
                for (int i = 0; i < sourceSynapses.Count; i++)
                {
                    sourceSynapses[i].Propagate();
                }
            }
            output = parent.Activate(bias + input, output);
        }

        /// <summary>
        /// Backpropagates the target synapses and evaluates the error
        /// </summary>
        public void EvaluateError()
        {
            if (targetSynapses.Count > 0)
            {
                error = 0d;
                foreach (BackpropagationSynapse synapse in targetSynapses)
                {
                    synapse.Backpropagate();
                }
            }
            error *= parent.Derivative(input, output);
        }

        /// <summary>
        /// Optimizes the bias value (if not <c>UseFixedBiasValues</c>) and the weights of all the
        /// source synapses using back propagation algorithm
        /// </summary>
        /// <param name="learningRate">
        /// The current learning rate (this depends on training progress as well)
        /// </param>
        public void Learn(double learningRate)
        {
            if (!parent.useFixedBiasValues)
            {
                bias += learningRate * error;
            }
            for (int i = 0; i < sourceSynapses.Count; i++)
            {
                sourceSynapses[i].OptimizeWeight(learningRate);
            }
        }
    }
}
