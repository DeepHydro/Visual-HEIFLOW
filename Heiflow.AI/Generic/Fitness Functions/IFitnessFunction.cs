// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI.Genetic
{
    using System;

    /// <summary>
    /// Fitness function interface.
    /// </summary>
    /// 
    /// <remarks>The interface should be implemented by all fitness function
    /// classes, which are supposed to be used for calculation of chromosomes
    /// fitness values. All fitness functions should return positive (<b>greater
    /// then zero</b>) value, which indicates how good is the evaluated chromosome - 
    /// the greater the value, the better the chromosome.
    /// </remarks>
    public interface IFitnessFunction
    {
        /// <summary>
        /// Evaluates chromosome.
        /// </summary>
        /// 
        /// <param name="chromosome">Chromosome to evaluate.</param>
        /// 
        /// <returns>Returns chromosome's fitness value.</returns>
        ///
        /// <remarks>The method calculates fitness value of the specified
        /// chromosome.</remarks>
        ///
        double Evaluate( IChromosome chromosome );
    }
}