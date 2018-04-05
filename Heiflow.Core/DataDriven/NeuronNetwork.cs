// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heiflow.AI.NeuronDotNet.Core.Backpropagation;
using Heiflow.AI.NeuronDotNet.Core.Initializers;
using Heiflow.AI.NeuronDotNet.Core;
using Heiflow.Models.AI;
using Heiflow.Core.Data;

namespace Heiflow.Core.DataDriven
{
    public class NeuralNetworkModel:ForecastingModel
    {
        public NeuralNetworkModel(AnnModelParameter parameter)
            : base(parameter)
        {
            Orgnization = "HUST WREIS";
            ID = "80056193-8885-4BCA-B402-457DB4A9CCB4";
            Name = "Neural  Network";
            Descriptions = "";
        }
        private BackpropagationNetwork network;

        #region IForecastingModel 成员

        public override void Train(IForecastingDataSets datasets)
        {
            OnStartRunning(new ComponentRunEventArgs(datasets));
            AnnModelParameter para = mParameter as AnnModelParameter;

            LinearLayer inputLayer = new LinearLayer(datasets.InputData[0].Length);

            SigmoidLayer hiddenLayer = new SigmoidLayer(para.HiddenNeuronsCount[0]);
            SigmoidLayer outputLayer = new SigmoidLayer(1);
            new BackpropagationConnector(inputLayer, hiddenLayer).Initializer = new RandomFunction(0d, 0.3d);
            new BackpropagationConnector(hiddenLayer, outputLayer).Initializer = new RandomFunction(0d, 0.3d);
            network = new BackpropagationNetwork(inputLayer, outputLayer);
            network.SetLearningRate(para.LearningRate);
            network.JitterEpoch = para.JitterEpoch;
            network.JitterNoiseLimit = para.JitterNoiseLimit;
            network.EndEpochEvent += new TrainingEpochEventHandler(
               delegate(object senderNetwork, TrainingEpochEventArgs args)
               {
                   // TODO: trainning error needs to be calculated
                   OnRunningEpoch(new AnnModelRunEpochEventArgs(args.TrainingIteration+1, 0));
               });

            network.Learn(ForecastingDataSets.ConvertToTrainingSet(datasets), para.Iterations);

            datasets.ForecastedData = new double[datasets.InputData.Length][];
            for (int i = 0; i < datasets.InputData.Length; i++)
            {
                datasets.ForecastedData[i] = new double[1];
                datasets.ForecastedData[i][0] = Forecast(datasets.InputData[i]);
            }
            OnFinishRunning(new ComponentRunEventArgs(datasets));

        }

        public override double Forecast(double[] inputVector)
        {
            if (network != null)
            {
                return network.Run(inputVector)[0];
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}
