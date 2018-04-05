// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System.Collections.Generic;
namespace  Heiflow.AI.GeneticProgramming
{
    /// <summary>
    /// Interface for implementing Custom fitness function for chromosome evaluation.
    /// Important note is that every class derived form this interface must implement Serialize atribut in order to allow serilization of population.
    /// </summary>
    public interface IFitnessFunction
    {
        void Evaluate(List<int> lst, GPFunctionSet gpFunctionSet, GPTerminalSet gpTerminalSet, GPChromosome c);
    }
}
