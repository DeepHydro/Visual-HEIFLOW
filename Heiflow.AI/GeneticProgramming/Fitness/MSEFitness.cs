// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;

namespace  Heiflow.AI.GeneticProgramming
{
    /// <summary>
    /// Implement Mean Square Error fitness function
    /// </summary>
    [Serializable]
    public class MSEFitness:IFitnessFunction
    {
        #region IFitnessFunction Members

        public void Evaluate(List<int> lst, GPFunctionSet gpFunctionSet, GPTerminalSet gpTerminalSet, GPChromosome c)
        {
            c.Fitness = 0;
            double rowFitness = 0.0;
            double y,temp;
           
            //Translate chromosome to list expressions
            int indexOutput = gpTerminalSet.NumConstants + gpTerminalSet.NumVariables;
            for (int i = 0; i < gpTerminalSet.RowCount; i++)
            {
                // evalue the function
                y = gpFunctionSet.Evaluate(lst, gpTerminalSet, i);

                // check for correct numeric value
                if (double.IsNaN(y) || double.IsInfinity(y))
                    y = 0;
                temp=y - gpTerminalSet.TrainingData[i][indexOutput];
                rowFitness += temp*temp;
            }
            //Fitness
            c.Fitness = (float)((1.0 / (1.0 + rowFitness / indexOutput)) * 1000.0);
        }

        #endregion
    }
}
