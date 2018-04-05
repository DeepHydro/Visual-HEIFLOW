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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

namespace  Heiflow.AI.Neuro
{
    using System;

    /// <summary>
    /// Activation network.
    /// </summary>
    /// 
    /// <remarks><para>Activation network is a base for multi-layer neural network
    /// with activation functions. It consists of <see cref="ActivationLayer">activation
    /// layers</see>.</para>
    ///
    /// <para>Sample usage:</para>
    /// <code>
    /// // create activation network
    ///	ActivationNetwork network = new ActivationNetwork(
    ///		new SigmoidFunction( ), // sigmoid activation function
    ///		3,                      // 3 inputs
    ///		4, 1 );                 // 2 layers:
    ///                             // 4 neurons in the firs layer
    ///                             // 1 neuron in the second layer
    ///	</code>
    /// </remarks>
    /// 
    [Serializable]
    public class ActivationNetwork : Network
    {
        /// <summary>
        /// Network's layers accessor.
        /// </summary>
        /// 
        /// <param name="index">Layer index.</param>
        /// 
        /// <remarks>Allows to access network's layer.</remarks>
        /// 
        public new ActivationLayer this[int index]
        {
            get { return ( (ActivationLayer) layers[index] ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationNetwork"/> class.
        /// </summary>
        /// 
        /// <param name="function">Activation function of neurons of the network.</param>
        /// <param name="inputsCount">Network's inputs count.</param>
        /// <param name="neuronsCount">Array, which specifies the amount of neurons in
        /// each layer of the neural network.</param>
        /// 
        /// <remarks>The new network is randomized (see <see cref="ActivationNeuron.Randomize"/>
        /// method) after it is created.</remarks>
        /// 
        public ActivationNetwork( IActivationFunction function, int inputsCount, params int[] neuronsCount )
            : base( inputsCount, neuronsCount.Length )
        {
            // create each layer
            for ( int i = 0; i < layersCount; i++ )
            {
                layers[i] = new ActivationLayer(
                    // neurons count in the layer
                    neuronsCount[i],
                    // inputs count of the layer
                    ( i == 0 ) ? inputsCount : neuronsCount[i - 1],
                    // activation function of the layer
                    function );
            }
        }

        /// <summary>
        /// Set new activation function for all neurons of the network.
        /// </summary>
        /// 
        /// <param name="function">Activation function to set.</param>
        /// 
        /// <remarks><para>The method sets new activation function for all neurons by calling
        /// <see cref="ActivationLayer.SetActivationFunction"/> method for each layer of the network.</para></remarks>
        /// 
        public void SetActivationFunction( IActivationFunction function )
        {
            for ( int i = 0; i < layersCount; i++ )
            {
                ( (ActivationLayer) layers[i] ).SetActivationFunction( function );
            }
        }
    }
}
