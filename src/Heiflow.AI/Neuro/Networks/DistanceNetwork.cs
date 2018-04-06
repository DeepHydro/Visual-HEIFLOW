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

namespace  Heiflow.AI.Neuro
{
    using System;

    /// <summary>
    /// Distance network.
    /// </summary>
    ///
    /// <remarks>Distance network is a neural network of only one <see cref="DistanceLayer">distance
    /// layer</see>. The network is a base for such neural networks as SOM, Elastic net, etc.
    /// </remarks>
    ///
    [Serializable]
    public class DistanceNetwork : Network
    {
        /// <summary>
        /// Network's layers accessor.
        /// </summary>
        /// 
        /// <param name="index">Layer index.</param>
        /// 
        /// <remarks>Allows to access network's layer.</remarks>
        /// 
        public new DistanceLayer this[int index]
        {
            get { return ( (DistanceLayer) layers[index] ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistanceNetwork"/> class.
        /// </summary>
        /// 
        /// <param name="inputsCount">Network's inputs count.</param>
        /// <param name="neuronsCount">Network's neurons count.</param>
        /// 
        /// <remarks>The new network is randomized (see <see cref="Neuron.Randomize"/>
        /// method) after it is created.</remarks>
        /// 
        public DistanceNetwork( int inputsCount, int neuronsCount )
            : base( inputsCount, 1 )
        {
            // create layer
            layers[0] = new DistanceLayer( neuronsCount, inputsCount );
        }

        /// <summary>
        /// Get winner neuron.
        /// </summary>
        /// 
        /// <returns>Index of the winner neuron.</returns>
        /// 
        /// <remarks>The method returns index of the neuron, which weights have
        /// the minimum distance from network's input.</remarks>
        /// 
        public int GetWinner( )
        {
            // find the MIN value
            double min = output[0];
            int    minIndex = 0;

            for ( int i = 1, n = output.Length; i < n; i++ )
            {
                if ( output[i] < min )
                {
                    // found new MIN value
                    min = output[i];
                    minIndex = i;
                }
            }

            return minIndex;
        }
    }
}
