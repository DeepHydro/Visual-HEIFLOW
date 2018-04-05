// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI.Math.Random
{
    using System;

    /// <summary>
    /// Interface for random numbers generators.
    /// </summary>
    /// 
    /// <remarks><para>The interface defines set of methods and properties, which should
    /// be implemented by different algorithms for random numbers generatation.</para>
    /// </remarks>
    /// 
    public interface IRandomNumberGenerator
    {
        /// <summary>
        /// Mean value of generator.
        /// </summary>
        /// 
        double Mean { get; }

        /// <summary>
        /// Variance value of generator.
        /// </summary>
        /// 
        double Variance { get; }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        double Next( );

        /// <summary>
        /// Set seed of the random numbers generator.
        /// </summary>
        /// 
        /// <param name="seed">Seed value.</param>
        /// 
        void SetSeed( int seed );
    }
}
