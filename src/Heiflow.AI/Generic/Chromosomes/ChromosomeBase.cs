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

namespace  Heiflow.AI.Genetic
{
    using System;

    /// <summary>
    /// Chromosomes' base class.
    /// </summary>
    /// 
    /// <remarks><para>The base class provides implementation of some <see cref="IChromosome"/>
    /// methods and properties, which are identical to all types of chromosomes.</para></remarks>
    /// 
    public abstract class ChromosomeBase : IChromosome
    {
        /// <summary>
        /// Chromosome's fintess value.
        /// </summary>
        protected double fitness = 0;

        /// <summary>
        /// Chromosome's fintess value.
        /// </summary>
        /// 
        /// <remarks><para>Fitness value (usefulness) of the chromosome calculate by calling
        /// <see cref="Evaluate"/> method. The greater the value, the more useful the chromosome.
        /// </para></remarks>
        /// 
        public double Fitness
        {
            get { return fitness; }
        }

        /// <summary>
        /// Generate random chromosome value.
        /// </summary>
        /// 
        /// <remarks><para>Regenerates chromosome's value using random number generator.</para>
        /// </remarks>
        /// 
        public abstract void Generate( );

        /// <summary>
        /// Create new random chromosome with same parameters (factory method).
        /// </summary>
        /// 
        /// <remarks><para>The method creates new chromosome of the same type, but randomly
        /// initialized. The method is useful as factory method for those classes, which work
        /// with chromosome's interface, but not with particular chromosome class.</para></remarks>
        /// 
        public abstract IChromosome CreateNew( );

        /// <summary>
        /// Clone the chromosome.
        /// </summary>
        /// 
        /// <remarks><para>The method clones the chromosome returning the exact copy of it.</para>
        /// </remarks>
        /// 
        public abstract IChromosome Clone( );

        /// <summary>
        /// Mutation operator.
        /// </summary>
        /// 
        /// <remarks><para>The method performs chromosome's mutation, changing its part randomly.</para></remarks>
        /// 
        public abstract void Mutate( );

        /// <summary>
        /// Crossover operator.
        /// </summary>
        /// 
        /// <param name="pair">Pair chromosome to crossover with.</param>
        /// 
        /// <remarks><para>The method performs crossover between two chromosomes – interchanging some parts of chromosomes.</para></remarks>
        /// 
        public abstract void Crossover( IChromosome pair );

        /// <summary>
        /// Evaluate chromosome with specified fitness function.
        /// </summary>
        /// 
        /// <param name="function">Fitness function to use for evaluation of the chromosome.</param>
        /// 
        /// <remarks><para>Calculates chromosome's fitness using the specifed fitness function.</para></remarks>
        ///
        public void Evaluate( IFitnessFunction function )
        {
            fitness = function.Evaluate( this );
        }

        /// <summary>
        /// Compare two chromosomes.
        /// </summary>
        /// 
        /// <param name="o">Binary chromosome to compare to.</param>
        /// 
        /// <returns>Returns comparison result, which equals to 0 if fitness values
        /// of both chromosomes are equal, 1 if fitness value of this chromosome
        /// is less than fitness value of the specified chromosome, -1 otherwise.</returns>
        /// 
        public int CompareTo( object o )
        {
            double f = ( (ChromosomeBase) o ).fitness;

            return ( fitness == f ) ? 0 : ( fitness < f ) ? 1 : -1;
        }
    }
}
