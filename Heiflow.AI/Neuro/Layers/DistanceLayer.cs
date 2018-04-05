// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI.Neuro
{
    using System;

    /// <summary>
    /// Distance layer.
    /// </summary>
    /// 
    /// <remarks>Distance layer is a layer of <see cref="DistanceNeuron">distance neurons</see>.
    /// The layer is usually a single layer of such networks as Kohonen Self
    /// Organizing Map, Elastic Net, Hamming Memory Net.</remarks>
    /// 
    [Serializable]
    public class DistanceLayer : Layer
    {
        /// <summary>
        /// Layer's neurons accessor.
        /// </summary>
        /// 
        /// <param name="index">Neuron index.</param>
        /// 
        /// <remarks>Allows to access layer's neurons.</remarks>
        /// 
        public new DistanceNeuron this[int index]
        {
            get { return (DistanceNeuron) neurons[index]; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistanceLayer"/> class.
        /// </summary>
        /// 
        /// <param name="neuronsCount">Layer's neurons count.</param>
        /// <param name="inputsCount">Layer's inputs count.</param>
        /// 
        /// <remarks>The new layet is randomized (see <see cref="Neuron.Randomize"/>
        /// method) after it is created.</remarks>
        /// 
        public DistanceLayer( int neuronsCount, int inputsCount )
            : base( neuronsCount, inputsCount )
        {
            // create each neuron
            for ( int i = 0; i < neuronsCount; i++ )
                neurons[i] = new DistanceNeuron( inputsCount );
        }
    }
}
