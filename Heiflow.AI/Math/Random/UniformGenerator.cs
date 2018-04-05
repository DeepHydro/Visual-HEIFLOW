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

namespace  Heiflow.AI.Math.Random
{
    using System;
    using  Heiflow.AI;

    /// <summary>
    /// Uniform random numbers generator.
    /// </summary>
    /// 
    /// <remarks><para>The random numbers generator generates uniformly
    /// distributed numbers in the <see cref="Range">specified range</see> - values
    /// are greater or equal to minimum range's value and less than maximum range's
    /// value.</para>
    /// 
    /// <para>The generator uses <see cref="UniformOneGenerator"/> generator
    /// to generate random numbers.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create instance of random generator
    /// IRandomNumberGenerator generator = new UniformGenerator( new DoubleRange( 50, 100 ) );
    /// // generate random number
    /// double randomNumber = generator.Next( );
    /// </code>
    /// </remarks>
    /// 
    public class UniformGenerator : IRandomNumberGenerator
    {
        private UniformOneGenerator rand = null;

        // generator's range
        private double min;
        private double length;

        /// <summary>
        /// Mean value of the generator.
        /// </summary>
        ///
        public double Mean
        {
            get { return ( min + min + length ) / 2; }
        }

        /// <summary>
        /// Variance value of the generator.
        /// </summary>
        ///
        public double Variance
        {
            get { return length * length / 12; }
        }

        /// <summary>
        /// Random numbers range.
        /// </summary>
        /// 
        /// <remarks><para>Range of random numbers to generate. Generated numbers are
        /// greater or equal to minimum range's value and less than maximum range's
        /// value.</para>
        /// </remarks>
        /// 
        public DoubleRange Range
        {
            get { return new DoubleRange( min, min + length ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="range">Random numbers range.</param>
        /// 
        /// <remarks>Initializes random numbers generator with zero seed.</remarks>
        /// 
        public UniformGenerator( DoubleRange range ) :
            this( range, 0 )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="range">Random numbers range.</param>
        /// <param name="seed">Seed value to initialize random numbers generator.</param>
        /// 
        public UniformGenerator( DoubleRange range, int seed )
        {
            rand = new UniformOneGenerator( seed );

            min     = range.Min;
            length  = range.Length;
        }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        public double Next( )
        {
            return rand.Next( ) * length + min;
        }

        /// <summary>
        /// Set seed of the random numbers generator.
        /// </summary>
        /// 
        /// <param name="seed">Seed value.</param>
        /// 
        /// <remarks>Resets random numbers generator initializing it with
        /// specified seed value.</remarks>
        /// 
        public void SetSeed( int seed )
        {
            rand = new UniformOneGenerator( seed );
        }
    }
}
