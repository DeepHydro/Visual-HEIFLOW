// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
