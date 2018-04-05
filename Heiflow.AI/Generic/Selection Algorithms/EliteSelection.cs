// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI.Genetic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Elite selection method.
    /// </summary>
    /// 
    /// <remarks>Elite selection method selects specified amount of
    /// best chromosomes to the next generation.</remarks> 
    /// 
    public class EliteSelection : ISelectionMethod
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EliteSelection"/> class.
        /// </summary>
        public EliteSelection( ) { }

        /// <summary>
        /// Apply selection to the specified population.
        /// </summary>
        /// 
        /// <param name="chromosomes">Population, which should be filtered.</param>
        /// <param name="size">The amount of chromosomes to keep.</param>
        /// 
        /// <remarks>Filters specified population keeping only specified amount of best
        /// chromosomes.</remarks>
        /// 
        public void ApplySelection( List<IChromosome> chromosomes, int size )
        {
            // sort chromosomes
            chromosomes.Sort( );

            // remove bad chromosomes
            chromosomes.RemoveRange( size, chromosomes.Count - size );
        }
    }
}
