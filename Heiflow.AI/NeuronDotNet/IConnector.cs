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
using System.Runtime.Serialization;

namespace  Heiflow.AI.NeuronDotNet.Core
{
    /// <summary>
    /// This interface represents a connector. A connector is a collection of synapses connecting
    /// two layers in a network.
    /// </summary>
    public interface IConnector : ISerializable
    {
        /// <summary>
        /// Gets the source layer
        /// </summary>
        /// <value>
        /// The source layer. It is never <c>null</c>.
        /// </value>
        ILayer SourceLayer { get; }

        /// <summary>
        /// Gets the target layer
        /// </summary>
        /// <value>
        /// The target layer. It is never <c>null</c>.
        /// </value>
        ILayer TargetLayer { get; }

        /// <summary>
        /// Gets the number of synapses in the connector. 
        /// </summary>
        /// <value>
        /// Synapse Count. It is always positive.
        /// </value>
        int SynapseCount { get; }

        /// <summary>
        /// Exposes an enumerator to iterate over all synapses in the connector.
        /// </summary>
        /// <value>
        /// Synapses Enumerator. No synapse enumerated can be <c>null</c>.
        /// </value>
        IEnumerable<ISynapse> Synapses { get; }

        /// <summary>
        /// Gets the connection mode
        /// </summary>
        /// <value>
        /// Connection Mode
        /// </value>
        ConnectionMode ConnectionMode { get; }

        /// <summary>
        /// Gets or sets the Initializer used to initialize the connector
        /// </summary>
        /// <value>
        /// Initializer used to initialize the connector. If this value is <c>null</c>, initialization
        /// is NOT performed.
        /// </value>
        IInitializer Initializer { get; set; }

        /// <summary>
        /// Initializes all synapses in the connector and makes them ready to undergo training
        /// freshly. (Adjusts the weights of synapses using the initializer)
        /// </summary>
        void Initialize();

        /// <summary>
        /// Adds small random noise to weights of synapses so that the network deviates from its
        /// local optimum position (a local equilibrium state where further learning is of no use)
        /// </summary>
        /// <param name="jitterNoiseLimit">
        /// Maximum absolute limit to the random noise added
        /// </param>
        void Jitter(double jitterNoiseLimit);
    }
}