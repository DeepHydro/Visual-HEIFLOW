// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  Heiflow.AI.GeneticProgramming
{
    /// <summary>
    /// GPdotNET 4.0 implements the Correlation Coefficient fitness function.The Correlation Coefficient fitness is based
    /// on the standard correlation coefficient, which is a dimensionless index that ranges from -1 to 1 
    /// and reflects the extent of a linear relationship between the predicted values and the target values.
    /// Ci=Cov(T,P)/sigmaT*sigmaP
    /// </summary>
    [Serializable]
    public class CCFitness:IFitnessFunction
    {
        #region IFitnessFunction Members

        public void Evaluate(List<int> lst, GPFunctionSet gpFunctionSet, GPTerminalSet gpTerminalSet, GPChromosome c)
        {
            c.Fitness = 0;
            double rowFitness = 0.0;
            
            double CovTP = 0.0;
            double SigmaP = 0.0;
            double SigmaT = 0.0;
           
            //number of sample case
            int indexOutput = gpTerminalSet.NumConstants + gpTerminalSet.NumVariables;

            double[] y = new double[gpTerminalSet.RowCount];
            double ymean=0;
            
            //Calculate output
            for (int i = 0; i < gpTerminalSet.RowCount; i++)
            {
                // evalue the function
                y[i] = gpFunctionSet.Evaluate(lst, gpTerminalSet, i);

                // check for correct numeric value
                if (double.IsNaN(y[i]) || double.IsInfinity(y[i]))
                {
                    //if output is not a number return infinity fitness
                    c.Fitness = 0;
                    return;
                }
                ymean += y[i];
            }

            //calculate AverageValue output
            ymean = ymean / gpTerminalSet.RowCount;

            //Calculate Corelation coeficient
            for (int i = 0; i < gpTerminalSet.RowCount; i++)
            {
                CovTP = CovTP + ((gpTerminalSet.TrainingData[i][indexOutput] - gpTerminalSet.AverageValue) * (y[i] - ymean));
                SigmaP += System.Math.Pow((y[i] - ymean),2);
                SigmaT += System.Math.Pow((gpTerminalSet.TrainingData[i][indexOutput] - gpTerminalSet.AverageValue), 2);
            }

            rowFitness =CovTP / (System.Math.Sqrt(SigmaP * SigmaT));

            if (double.IsNaN(rowFitness) || double.IsInfinity(rowFitness))
            {
                //if output is not a number return zero fitness
                c.Fitness = 0;
                return;
            }
            //Fitness
            c.Fitness = (float)(rowFitness*rowFitness* 1000.0);
        }

        #endregion
    }
}
