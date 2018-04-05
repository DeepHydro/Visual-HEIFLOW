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

namespace  Heiflow.AI.Genetic
{
    using System;

    /// <summary>
    /// Binary chromosome, which supports length from 2 till 64.
    /// </summary>
    /// 
    /// <remarks><para>The binary chromosome is the simplest type of chromosomes,
    /// which is represented by a set of bits. Maximum number of bits comprising
    /// the chromosome is 64.</para></remarks>
    /// 
    public class BinaryChromosome : ChromosomeBase
    {
        /// <summary>
        /// Chromosome's length in bits.
        /// </summary>
        protected int length;

        /// <summary>
        /// Numerical chromosome's value.
        /// </summary>
        protected ulong val = 0;

        /// <summary>
        /// Random number generator for chromosoms generation, crossover, mutation, etc.
        /// </summary>
        protected static Random rand = new Random( );

        /// <summary>
        /// Chromosome's maximum length.
        /// </summary>
        /// 
        /// <remarks><para>Maxim chromosome's length in bits, which is supported
        /// by the class</para></remarks>
        /// 
        public const int MaxLength = 64;

        /// <summary>
        /// Chromosome's length.
        /// </summary>
        /// 
        /// <remarks><para>Length of the chromosome in bits.</para></remarks>
        /// 
        public int Length
        {
            get { return length; }
        }

        /// <summary>
        /// Chromosome's value.
        /// </summary>
        /// 
        /// <remarks><para>Current numerical value of the chromosome.</para></remarks>
        /// 
        public ulong Value
        {
            get { return val & ( 0xFFFFFFFFFFFFFFFF >> ( 64 - length ) ); }
        }

        /// <summary>
        /// Max possible chromosome's value.
        /// </summary>
        /// 
        /// <remarks><para>Maximum possible numerical value, which may be represented
        /// by the chromosome of current length.</para></remarks>
        /// 
        public ulong MaxValue
        {
            get { return 0xFFFFFFFFFFFFFFFF >> ( 64 - length ); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="length">Chromosome's length in bits, [2, <see cref="MaxLength"/>].</param>
        /// 
        public BinaryChromosome( int length )
        {
            this.length = Math.Max( 2, Math.Min( MaxLength, length ) );
            // randomize the chromosome
            Generate( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryChromosome"/> class.
        /// </summary>
        /// 
        /// <param name="source">Source chromosome to copy.</param>
        /// 
        /// <remarks><para>This is a copy constructor, which creates the exact copy
        /// of specified chromosome.</para></remarks>
        /// 
        protected BinaryChromosome( BinaryChromosome source )
        {
            length  = source.length;
            val     = source.val;
            fitness = source.fitness;
        }

        /// <summary>
        /// Get string representation of the chromosome.
        /// </summary>
        /// 
        /// <returns>Returns string representation of the chromosome.</returns>
        /// 
        public override string ToString( )
        {
            ulong	tval = val;
            char[]	chars = new char[length];

            for ( int i = length - 1; i >= 0; i-- )
            {
                chars[i] = (char) ( ( tval & 1 ) + '0' );
                tval >>= 1;
            }

            // return the result string
            return new string( chars );
        }

        /// <summary>
        /// Generate random chromosome value.
        /// </summary>
        /// 
        /// <remarks><para>Regenerates chromosome's value using random number generator.</para>
        /// </remarks>
        /// 
        public override void Generate( )
        {
            byte[] bytes = new byte[8];

            // generate value
            rand.NextBytes( bytes );
            val = BitConverter.ToUInt64( bytes, 0 );
        }

        /// <summary>
        /// Create new random chromosome with same parameters (factory method).
        /// </summary>
        /// 
        /// <remarks><para>The method creates new chromosome of the same type, but randomly
        /// initialized. The method is useful as factory method for those classes, which work
        /// with chromosome's interface, but not with particular chromosome type.</para></remarks>
        /// 
        public override IChromosome CreateNew( )
        {
            return new BinaryChromosome( length );
        }

        /// <summary>
        /// Clone the chromosome.
        /// </summary>
        /// 
        /// <returns>Return's clone of the chromosome.</returns>
        /// 
        /// <remarks><para>The method clones the chromosome returning the exact copy of it.</para>
        /// </remarks>
        ///
        public override IChromosome Clone( )
        {
            return new BinaryChromosome( this );
        }

        /// <summary>
        /// Mutation operator.
        /// </summary>
        /// 
        /// <remarks><para>The method performs chromosome's mutation, changing randomly
        /// one of its bits.</para></remarks>
        /// 
        public override void Mutate( )
        {
            val ^= ( (ulong) 1 << rand.Next( length ) );
        }

        /// <summary>
        /// Crossover operator.
        /// </summary>
        /// 
        /// <param name="pair">Pair chromosome to crossover with.</param>
        /// 
        /// <remarks><para>The method performs crossover between two chromosomes ?interchanging
        /// range of bits between these chromosomes.</para></remarks>
        ///
        public override void Crossover( IChromosome pair )
        {
            BinaryChromosome p = (BinaryChromosome) pair;

            // check for correct pair
            if ( ( p != null ) && ( p.length == length ) )
            {
                int		crossOverPoint = 63 - rand.Next( length - 1 );
                ulong	mask1 = 0xFFFFFFFFFFFFFFFF >> crossOverPoint;
                ulong	mask2 = ~mask1;

                ulong	v1 = val;
                ulong	v2 = p.val;

                // calculate new values
                val   = ( v1 & mask1 ) | ( v2 & mask2 );
                p.val = ( v2 & mask1 ) | ( v1 & mask2 );
            }
        }
    }
}
