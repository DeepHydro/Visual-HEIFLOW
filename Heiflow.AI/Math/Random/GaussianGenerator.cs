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

    /// <summary>
    /// Gaussian random numbers generator.
    /// </summary>
    /// 
    /// <remarks><para>The random number generator generates gaussian
    /// random numbers with specified mean and standard deviation values.</para>
    /// 
    /// <para>The generator uses <see cref="StandardGenerator"/> generator as base
    /// to generate random numbers.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create instance of random generator
    /// IRandomNumberGenerator generator = new GaussianGenerator( 5.0, 1.5 );
    /// // generate random number
    /// double randomNumber = generator.Next( );
    /// </code>
    /// </remarks>
    /// 
    public class GaussianGenerator : IRandomNumberGenerator
    {
        // standard numbers generator
        private StandardGenerator rand = null;
        // mean value
        private double mean;
        // standard deviation value
        private double stdDev;

        /// <summary>
        /// Mean value of the generator.
        /// </summary>
        ///
        public double Mean
        {
            get { return mean;  }
        }

        /// <summary>
        /// Variance value of the generator.
        /// </summary>
        ///
        public double Variance
        {
            get { return stdDev * stdDev; }
        }

        /// <summary>
        /// Standard deviation value.
        /// </summary>
        ///
        public double StdDev
        {
            get { return stdDev; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="mean">Mean value.</param>
        /// <param name="stdDev">Standard deviation value.</param>
        /// 
        public GaussianGenerator( double mean, double stdDev ) :
            this( mean, stdDev, 0 )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussianGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="mean">Mean value.</param>
        /// <param name="stdDev">Standard deviation value.</param>
        /// <param name="seed">Seed value to initialize random numbers generator.</param>
        /// 
        public GaussianGenerator( double mean, double stdDev, int seed )
        {
            this.mean   = mean;
            this.stdDev = stdDev;

            rand = new StandardGenerator( seed );
        }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        public double Next( )
        {
            return rand.Next( ) * stdDev + mean;
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
            rand = new StandardGenerator( seed );
        }
    }
}
