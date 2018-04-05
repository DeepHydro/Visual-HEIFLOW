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
    /// Genetic selection method interface.
    /// </summary>
    /// 
    /// <remarks>The interface should be implemented by all classes, which
    /// implement genetic selection algorithm. These algorithms select members of
    /// current generation, which should be kept in the new generation. Basically,
    /// these algorithms filter provided population keeping only specified amount of
    /// members.</remarks>
    /// 
    public interface ISelectionMethod
    {
        /// <summary>
        /// Apply selection to the specified population.
        /// </summary>
        /// 
        /// <param name="chromosomes">Population, which should be filtered.</param>
        /// <param name="size">The amount of chromosomes to keep.</param>
        /// 
        /// <remarks>Filters specified population according to the implemented
        /// selection algorithm.</remarks>
        /// 
        void ApplySelection( List<IChromosome> chromosomes, int size );
    }
}