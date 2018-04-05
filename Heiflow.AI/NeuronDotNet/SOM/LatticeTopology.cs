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

namespace  Heiflow.AI.NeuronDotNet.Core.SOM
{
    /// <summary>
    /// Lattice Topology of the position neurons in a Kohonen Layer
    /// </summary>
    public enum LatticeTopology
    {
        // Arrangement of neurons in a rectangular lattice
        //
        //            0 0 0 0 0 0
        //            0 0 0 * 0 0
        //            0 0 * O * 0
        //            0 0 0 * 0 0
        //            0 0 0 0 0 0
        //
        // The four immediate neighbors of 'O' are shown as '*'

        /// <summary>
        /// Each neuron has four immediate neighbors
        /// </summary>
        Rectangular = 0,



        // Arrangement of neurons in a hexagonal lattice
        //
        //            0 0 0 0 0
        //             0 0 * * 0
        //            0 0 * O * 0
        //             0 0 * * 0 0
        //              0 0 0 0 0
        //
        // The six immediate neighbors of 'O' are shown as '*'

        /// <summary>
        /// Each neuron has six immediate neighbors
        /// </summary>
        Hexagonal = 1,
    }
}
