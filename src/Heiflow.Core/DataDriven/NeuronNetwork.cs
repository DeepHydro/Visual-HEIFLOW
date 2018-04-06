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
