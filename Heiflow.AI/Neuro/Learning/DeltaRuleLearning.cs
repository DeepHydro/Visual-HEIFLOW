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

namespace  Heiflow.AI.Neuro.Learning
{
    using System;

    /// <summary>
    /// Delta rule learning algorithm.
    /// </summary>
    /// 
    /// <remarks><para>This learning algorithm is used to train one layer neural
    /// network of <see cref="ActivationNeuron">Activation Neurons</see>
    /// with continuous activation function, see <see cref="SigmoidFunction"/>
    /// for example.</para>
    /// 
    /// <para>See information about <a href="http://en.wikipedia.org/wiki/Delta_rule">delta rule</a>
    /// learning algorithm.</para>
    /// </remarks>
    /// 
    public class DeltaRuleLearning : ISupervisedLearning
    {
        // network to teach
        private ActivationNetwork network;
        // learning rate
        private double learningRate = 0.1;

        /// <summary>
        /// Learning rate, [0, 1].
        /// </summary>
        /// 
        /// <remarks><para>The value determines speed of learning.</para>
        /// 
        /// <para>Default value equals to <b>0.1</b>.</para>
        /// </remarks>
        /// 
        public double LearningRate
        {
            get { return learningRate; }
            set
            {
                learningRate = Math.Max( 0.0, Math.Min( 1.0, value ) );
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeltaRuleLearning"/> class.
        /// </summary>
        /// 
        /// <param name="network">Network to teach.</param>
        /// 
        /// <exception cref="ArgumentException">Invalid nuaral network. It should have one layer only.</exception>
        /// 
        public DeltaRuleLearning( ActivationNetwork network )
        {
            // check layers count
            if ( network.LayersCount != 1 )
            {
                throw new ArgumentException( "Invalid nuaral network. It should have one layer only." );
            }

            this.network = network;
        }

        /// <summary>
        /// Runs learning iteration.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// <param name="output">Desired output vector.</param>
        /// 
        /// <returns>Returns squared error (difference between current network's output and
        /// desired output) divided by 2.</returns>
        /// 
        /// <remarks><para>Runs one learning iteration and updates neuron's
        /// weights.</para></remarks>
        ///
        public double Run( double[] input, double[] output )
        {
            // compute output of network
            double[] networkOutput = network.Compute( input );

            // get the only layer of the network
            ActivationLayer layer = network[0];
            // get activation function of the layer
            IActivationFunction activationFunction = layer[0].ActivationFunction;

            // summary network absolute error
            double error = 0.0;

            // update weights of each neuron
            for ( int j = 0, k = layer.NeuronsCount; j < k; j++ )
            {
                // get neuron of the layer
                ActivationNeuron neuron = layer[j];
                // calculate neuron's error
                double e = output[j] - networkOutput[j];
                // get activation function's derivative
                double functionDerivative = activationFunction.Derivative2( networkOutput[j] );

                // update weights
                for ( int i = 0, n = neuron.InputsCount; i < n; i++ )
                {
                    neuron[i] += learningRate * e * functionDerivative * input[i];
                }

                // update threshold value
                neuron.Threshold += learningRate * e * functionDerivative;

                // sum error
                error += ( e * e );
            }

            return error / 2;
        }

        /// <summary>
        /// Runs learning epoch.
        /// </summary>
        /// 
        /// <param name="input">Array of input vectors.</param>
        /// <param name="output">Array of output vectors.</param>
        /// 
        /// <returns>Returns summary learning error for the epoch. See <see cref="Run"/>
        /// method for details about learning error calculation.</returns>
        /// 
        /// <remarks><para>The method runs one learning epoch, by calling <see cref="Run"/> method
        /// for each vector provided in the <paramref name="input"/> array.</para></remarks>
        ///
        public double RunEpoch( double[][] input, double[][] output )
        {
            double error = 0.0;

            // run learning procedure for all samples
            for ( int i = 0, n = input.Length; i < n; i++ )
            {
                error += Run( input[i], output[i] );
            }

            // return summary error
            return error;
        }
    }
}
