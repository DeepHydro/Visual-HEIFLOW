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
