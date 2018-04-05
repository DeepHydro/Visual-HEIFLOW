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