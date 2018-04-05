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
