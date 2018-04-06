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
    /// Activation neuron.
    /// </summary>
    /// 
    /// <remarks><para>Activation neuron computes weighted sum of its inputs, adds
    /// threshold value and then applies <see cref="ActivationFunction">activation function</see>.
    /// The neuron isusually used in multi-layer neural networks.</para></remarks>
    /// 
    /// <seealso cref="IActivationFunction"/>
    /// 
    [Serializable]
    public class ActivationNeuron : Neuron
    {
        /// <summary>
        /// Threshold value.
        /// </summary>
        /// 
        /// <remarks>The value is added to inputs weighted sum before it is passed to activation
        /// function.</remarks>
        /// 
        protected double threshold = 0.0;

        /// <summary>
        /// Activation function.
        /// </summary>
        /// 
        /// <remarks>The function is applied to inputs weighted sum plus
        /// threshold value.</remarks>
        /// 
        protected IActivationFunction function = null;

        /// <summary>
        /// Threshold value.
        /// </summary>
        /// 
        /// <remarks>The value is added to inputs weighted sum before it is passed to activation
        /// function.</remarks>
        /// 
        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

        /// <summary>
        /// Neuron's activation function.
        /// </summary>
        /// 
        public IActivationFunction ActivationFunction
        {
            get { return function; }
            set { function = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationNeuron"/> class.
        /// </summary>
        /// 
        /// <param name="inputs">Neuron's inputs count.</param>
        /// <param name="function">Neuron's activation function.</param>
        /// 
        public ActivationNeuron( int inputs, IActivationFunction function )
            : base( inputs )
        {
            this.function = function;
        }

        /// <summary>
        /// Randomize neuron.
        /// </summary>
        /// 
        /// <remarks>Calls base class <see cref="Neuron.Randomize">Randomize</see> method
        /// to randomize neuron's weights and then randomizes threshold's value.</remarks>
        /// 
        public override void Randomize( )
        {
            // randomize weights
            base.Randomize( );
            // randomize threshold
            threshold = rand.NextDouble( ) * ( randRange.Length ) + randRange.Min;
        }

        /// <summary>
        /// Computes output value of neuron.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns neuron's output value.</returns>
        /// 
        /// <remarks><para>The output value of activation neuron is equal to value
        /// of nueron's activation function, which parameter is weighted sum
        /// of its inputs plus threshold value. The output value is also stored
        /// in <see cref="Neuron.Output">Output</see> property.</para>
        /// 
        /// <para><note>The method may be called safely from multiple threads to compute neuron's
        /// output value for the specified input values. However, the value of
        /// <see cref="Neuron.Output"/> property in multi-threaded environment is not predictable,
        /// since it may hold neuron's output computed from any of the caller threads. Multi-threaded
        /// access to the method is useful in those cases when it is required to improve performance
        /// by utilizing several threads and the computation is based on the immediate return value
        /// of the method, but not on neuron's output property.</note></para>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">Wrong length of the input vector, which is not
        /// equal to the <see cref="Neuron.InputsCount">expected value</see>.</exception>
        /// 
        public override double Compute( double[] input )
        {
            // check for corrent input vector
            if ( input.Length != inputsCount )
                throw new ArgumentException( "Wrong length of the input vector." );

            // initial sum value
            double sum = 0.0;

            // compute weighted sum of inputs
            for ( int i = 0; i < inputsCount; i++ )
            {
                sum += weights[i] * input[i];
            }
            sum += threshold;

            // local variable to avoid mutlithreaded conflicts
            double output = function.Function( sum );
            // assign output property as well (works correctly for single threaded usage)
            this.output = output;

            return output;
        }
    }
}
