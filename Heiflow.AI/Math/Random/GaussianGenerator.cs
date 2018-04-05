// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
