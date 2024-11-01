﻿//
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Heiflow.AI.Math.Metrics;
using Heiflow.AI.NeuronDotNet.Core;
using Heiflow.Models.AI;

namespace Heiflow.Core.Data
{
    public  class ForecastingDataSets:IForecastingDataSets
    {
        public ForecastingDataSets(double [][] inputs, double [][] outputs)
        {
            InputData = inputs;
            OutputData = outputs;
        }

        /// <summary>
        /// The total number of input vector
        /// </summary>
        public int Length
        {
            get
            {
                return InputData.Length;
            }
        }
        /// <summary>
        /// The length of an individual input vector
        /// </summary>
        public int InputVectorLength
        {
            get
            {
                return InputData[0].Length;
            }
        }
        /// <summary>
        /// The length of an individual output vector
        /// </summary>
        public int OutputVectorLength
        {
            get
            {
                return OutputData[0].Length;
            }
        }

        public string[] InputColumnNames
        {
            get;
            set;
        }

        public string[] OutputColumnNames
        {
            get;
            set;
        }

        #region IForecastingDataSets members



        public double[][] InputData
        {
            get;
            set;
        }

        public double[][] OutputData
        {
            get;
            set;
        }

        public double[][] ForecastedData
        {
            get;
            set;
        }


        public DateTime [] Date
        {
            get;
            set;
        }

        #endregion

        public static ForecastingDataSets Merge(IForecastingDataSets[] sets)
        {
            ForecastingDataSets result = null;
            if (sets != null)
            {
                int length = (from s in sets select s.InputData.Length).Sum();
                int dimension = sets[0].InputData[0].Length;
                int outputDimension = sets[0].OutputData[0].Length;
                int i = 0;
                double[][] inputs = new double[length][];
                double[][] outputs = new double[length][];
                DateTime[] date = new DateTime[length];
                foreach (IForecastingDataSets set in sets)
                {                
                    for (int j = 0; j < set.InputData.Length; j++)
                    {
                        inputs[i] = new double[dimension];
                        outputs[i] = new double[outputDimension];
                        for (int c = 0; c < dimension; c++)
                        {
                            inputs[i][c] = set.InputData[j][c];
                        }
                        for (int c = 0; c < outputDimension; c++)
                        {
                            outputs[i][c] = set.OutputData[j][c];
                        }
                        date[i] = set.Date[j];
                        i++;
                    }
                }

                result = new ForecastingDataSets(inputs, outputs)
                {
                    Date = date
                };
            }
            return result;
        }

        public static TrainingSet ConvertToTrainingSet(IForecastingDataSets sets)
        {
            TrainingSet trainingset = new TrainingSet(sets.InputData[0].Length, sets.OutputData[0].Length);
            for (int i = 0; i < sets.InputData.Length; i++)
            {
                TrainingSample ts = new TrainingSample(sets.InputData[i],sets.OutputData[i]);
                trainingset.Add(ts);
            }
            return trainingset;
        }

        public static TrainingSet ConvertToUnSupervisedTrainingSet(IForecastingDataSets sets)
        {
            TrainingSet trainingset = new TrainingSet(sets.InputData[0].Length);
            for (int i = 0; i < sets.InputData.Length; i++)
            {
                TrainingSample ts = new TrainingSample(sets.InputData[i]);
                trainingset.Add(ts);
            }
            return trainingset;
        }

        public double[] this[int column, bool isInput]
        {
            get
            {
                if (isInput)
                    return GetDataColumn(InputData, column);
                else
                    return GetDataColumn(OutputData, column);
            }
        }

        public double[] GetDataColumn(double [][] data,int column)
        {
            double[] colData = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                colData[i] = data[i][column];
            }
            return colData;
        }

        public double[] GetInputDataColumn( int column)
        {
            double[] colData = new double[InputData.Length];
            for (int i = 0; i < InputData.Length; i++)
            {
                colData[i] = InputData[i][column];
            }
            return colData;
        }

        public double[] GetOutputDataColumn(int column)
        {
            double[] colData = new double[OutputData.Length];
            for (int i = 0; i < OutputData.Length; i++)
            {
                colData[i] = OutputData[i][column];
            }
            return colData;
        }

        /// <summary>
        /// Compute Pearson Correlation between each data column of the inputs and the identified data column of the output
        /// </summary>
        /// <param name="outputColumn">Indicates the column index of output data </param>
        /// <returns></returns>
        public double[] GetPearsonCorrelations(int outputColumn)
        {
            double[] result = new double[InputVectorLength];
            PearsonCorrelation pc = new PearsonCorrelation();
            for (int i = 0; i < InputVectorLength; i++)
            {
                result[i] = pc.GetSimilarityScore(this[i, true], this[outputColumn, false]);
            }
            return result;
        }


    }
}
