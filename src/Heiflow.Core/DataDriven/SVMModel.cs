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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Heiflow.AI.SVM;
using Heiflow.Models.AI;

namespace Heiflow.Core.DataDriven
{
    public class SVMModel:ForecastingModel
    {
        public SVMModel(Parameter parameter):base(parameter)
        {
            Orgnization = "HUST WREIS";
            ID = "34934C27-DD41-44D8-B359-BF56B663A6B7";
            Name = "Support Vector Machine";
            Descriptions = "";
        }
        private RangeTransform mRange;

        public Model TrainedModel { get; set; }

        #region IForecastingModel 成员

        public override void Train(IForecastingDataSets datasets)
        {
              Parameter svmpara = mParameter as Parameter;
            OnStartRunning(new ComponentRunEventArgs(datasets));
            double[] yvalue = null;
            int maxIndex = 0;
            Node[][] nodes = CreateNodes(datasets, out yvalue, out maxIndex);
            Problem problem = new Problem(nodes.Length, yvalue, nodes, maxIndex);
            mRange = Scaling.DetermineRange(problem);
            problem = Scaling.Scale(mRange, problem);

            TrainedModel = Training.Train(problem, svmpara as Parameter);
            datasets.ForecastedData = new double[datasets.InputData.Length][];

            for (int i = 0; i < datasets.InputData.Length; i++)
            {
                datasets.ForecastedData[i] = new double[1];
                datasets.ForecastedData[i][0] = Forecast(datasets.InputData[i]);
                OnRunningEpoch(new ComponentRunEpochEventArgs(i));
            }

            svmpara.Count = TrainedModel.SupportVectorCount;
            svmpara.Percentage = TrainedModel.SupportVectorCount /  (double) problem.Count;
            
            OnFinishRunning(new ComponentRunEventArgs(datasets));
        }
        #endregion

        private Node[][] CreateNodes(IForecastingDataSets datasets, out double[] yvalue, out int maxIndex)
        {     
            Node[][] nodes;
            int rows = datasets.InputData.Length;
            int columns = datasets.InputData[0].Length;
            maxIndex = columns;
            nodes = new Node[rows][];
            yvalue = new double[rows];
            for (int r = 0; r < rows; r++)
            {
                nodes[r] = new Node[columns];
                yvalue[r] = datasets.OutputData[r][0];
                for (int c = 0; c < columns; c++)
                {
                    nodes[r][c] = new Node();
                    nodes[r][c].Index = c + 1;
                    nodes[r][c].Value = datasets.InputData[r][c];
                }
            }   
            return nodes;
        }

        public override double Forecast(double[] vector)
        {
            double result = 0;
            if (TrainedModel != null && vector != null)
            {
                Node[] node = new Node[vector.Length];
                for (int i = 0; i < vector.Length; i++)
                {
                    node[i] = new Node();
                    node[i].Index = i + 1;
                    node[i].Value = vector[i];
                }
                node = mRange.Transform(node);
                result = Prediction.Predict(TrainedModel, node);
            }
            return result;
        }
    }
}
