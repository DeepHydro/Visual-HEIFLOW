// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
