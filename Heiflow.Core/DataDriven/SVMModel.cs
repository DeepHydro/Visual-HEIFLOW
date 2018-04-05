// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
