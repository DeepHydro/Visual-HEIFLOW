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
    using  Heiflow.AI;

    /// <summary>
    /// Base neuron class.
    /// </summary>
    /// 
    /// <remarks>This is a base neuron class, which encapsulates such
    /// common properties, like neuron's input, output and weights.</remarks>
    /// 
    [Serializable]
    public abstract class Neuron
    {
        /// <summary>
        /// Neuron's inputs count.
        /// </summary>
        protected int inputsCount = 0;

        /// <summary>
        /// Nouron's wieghts.
        /// </summary>
        protected double[] weights = null;

        /// <summary>
        /// Neuron's output value.
        /// </summary>
        protected double output = 0;

        /// <summary>
        /// Random number generator.
        /// </summary>
        /// 
        /// <remarks>The generator is used for neuron's weights randomization.</remarks>
        /// 
        protected static Random rand = new Random( );

        /// <summary>
        /// Random generator range.
        /// </summary>
        /// 
        /// <remarks>Sets the range of random generator. Affects initial values of neuron's weight.
        /// Default value is [0, 1].</remarks>
        /// 
        protected static DoubleRange randRange = new DoubleRange( 0.0, 1.0 );

        /// <summary>
        /// Random number generator.
        /// </summary>
        /// 
        /// <remarks>The property allows to initialize random generator with a custom seed. The generator is
        /// used for neuron's weights randomization.</remarks>
        /// 
        public static Random RandGenerator
        {
            get { return rand; }
            set
            {
                if ( value != null )
                {
                    rand = value;
                }
            }
        }

        /// <summary>
        /// Random generator range.
        /// </summary>
        /// 
        /// <remarks>Sets the range of random generator. Affects initial values of neuron's weight.
        /// Default value is [0, 1].</remarks>
        /// 
        public static DoubleRange RandRange
        {
            get { return randRange; }
            set
            {
                if ( value != null )
                {
                    randRange = value;
                }
            }
        }

        /// <summary>
        /// Neuron's inputs count.
        /// </summary>
        public int InputsCount
        {
            get { return inputsCount; }
        }

        /// <summary>
        /// Neuron's output value.
        /// </summary>
        /// 
        /// <remarks>The calculation way of neuron's output value is determined by inherited class.</remarks>
        /// 
        public double Output
        {
            get { return output; }
        }

        /// <summary>
        /// Neuron's weights accessor.
        /// </summary>
        /// 
        /// <param name="index">Weight index.</param>
        /// 
        /// <remarks>Allows to access neuron's weights.</remarks>
        /// 
        public double this[int index]
        {
            get { return weights[index]; }
            set { weights[index] = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Neuron"/> class.
        /// </summary>
        ///
        /// <param name="inputs">Neuron's inputs count.</param>
        /// 
        /// <remarks>The new neuron will be randomized (see <see cref="Randomize"/> method)
        /// after it is created.</remarks>
        ///
        protected Neuron( int inputs )
        {
            // allocate weights
            inputsCount = Math.Max( 1, inputs );
            weights = new double[inputsCount];
            // randomize the neuron
            Randomize( );
        }

        /// <summary>
        /// Randomize neuron.
        /// </summary>
        /// 
        /// <remarks>Initialize neuron's weights with random values within the range specified
        /// by <see cref="RandRange"/>.</remarks>
        /// 
        public virtual void Randomize( )
        {
            double d = randRange.Length;

            // randomize weights
            for ( int i = 0; i < inputsCount; i++ )
                weights[i] = rand.NextDouble( ) * d + randRange.Min;
        }

        /// <summary>
        /// Computes output value of neuron.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns neuron's output value.</returns>
        /// 
        /// <remarks>The actual neuron's output value is determined by inherited class.
        /// The output value is also stored in <see cref="Output"/> property.</remarks>
        /// 
        public abstract double Compute( double[] input );
    }
}
