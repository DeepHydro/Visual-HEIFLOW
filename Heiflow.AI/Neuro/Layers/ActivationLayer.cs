// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI.Neuro
{
    using System;

    /// <summary>
    /// Activation layer.
    /// </summary>
    /// 
    /// <remarks>Activation layer is a layer of <see cref="ActivationNeuron">activation neurons</see>.
    /// The layer is usually used in multi-layer neural networks.</remarks>
    ///
    [Serializable]
    public class ActivationLayer : Layer
    {
        /// <summary>
        /// Layer's neurons accessor.
        /// </summary>
        /// 
        /// <param name="index">Neuron index.</param>
        /// 
        /// <remarks>Allows to access layer's neurons.</remarks>
        /// 
        public new ActivationNeuron this[int index]
        {
            get { return (ActivationNeuron) neurons[index]; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationLayer"/> class.
        /// </summary>
        /// 
        /// <param name="neuronsCount">Layer's neurons count.</param>
        /// <param name="inputsCount">Layer's inputs count.</param>
        /// <param name="function">Activation function of neurons of the layer.</param>
        /// 
        /// <remarks>The new layer is randomized (see <see cref="ActivationNeuron.Randomize"/>
        /// method) after it is created.</remarks>
        /// 
        public ActivationLayer( int neuronsCount, int inputsCount, IActivationFunction function )
            : base( neuronsCount, inputsCount )
        {
            // create each neuron
            for ( int i = 0; i < neuronsCount; i++ )
                neurons[i] = new ActivationNeuron( inputsCount, function );
        }

        /// <summary>
        /// Set new activation function for all neurons of the layer.
        /// </summary>
        /// 
        /// <param name="function">Activation function to set.</param>
        /// 
        /// <remarks><para>The methods sets new activation function for each neuron by setting
        /// their <see cref="ActivationNeuron.ActivationFunction"/> property.</para></remarks>
        /// 
        public void SetActivationFunction( IActivationFunction function )
        {
            for ( int i = 0; i < neuronsCount; i++ )
            {
                ( (ActivationNeuron) neurons[i] ).ActivationFunction = function;
            }
        }
    }
}
