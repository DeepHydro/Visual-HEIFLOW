// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heiflow.AI.Neuro;
using Heiflow.AI.Neuro.Learning;
using Heiflow.Models.AI;

namespace Heiflow.Core.DataDriven
{
    public struct LayerWeight
    {
        public double[][] Weight;
        public double[][] ThreashHold;
        public AnnLayerType LayerType;
        public string[][] Text;
    }

    public class BpANNModel:IForecastingModel
    {
        public BpANNModel(AnnModelParameter parameter)
        {
            Orgnization = "HUST WREIS";
            ID = "2F710E58-91E8-4CF4-A721-A7FA243D0D69";
            Name = "Artificial Neuron Network";
            Descriptions = "";
            mAnnModelParameter = parameter;
            mAnnModelParameter.Component = this;
        }

        public event ComponentRunEpochEventHandler ModelRunningEpoch;
        public event ComponentRunEventHandler ModelStartRunning;
        public event ComponentRunEventHandler ModelFinishRunning;
        private AnnModelParameter mAnnModelParameter;

        private volatile bool mNeedToStop;
        private ActivationNetwork mNetwork;
        LayerWeight[] LayerWeightCollection { get; set; }

        #region IForecastingModel 成员

        public  bool NeedToStop
        {
            get
            {
                return mNeedToStop;
            }
            set
            {
                mNeedToStop = value;
            }
        }

        public void Train(IForecastingDataSets datasets)
        {
            NeedToStop = false;
            if (ModelStartRunning != null)
                ModelStartRunning(this, new ComponentRunEventArgs(datasets));
            IActivationFunction actFunc = null;
            double sigmoidAlphaValue = mAnnModelParameter.SigmoidAlphaValue;
            if (mAnnModelParameter.HiddenActivationFunction == fann_activationfunc_enum.FANN_SIGMOID_SYMMETRIC)
                actFunc = new BipolarSigmoidFunction(sigmoidAlphaValue);
            else if (mAnnModelParameter.HiddenActivationFunction == fann_activationfunc_enum.FANN_SIGMOID)
                actFunc = new SigmoidFunction(sigmoidAlphaValue);
            else if (mAnnModelParameter.HiddenActivationFunction == fann_activationfunc_enum.FANN_THRESHOLD)
                actFunc = new ThresholdFunction();
            else
                actFunc = new BipolarSigmoidFunction(sigmoidAlphaValue);

            mAnnModelParameter.InputNeuronCount = datasets.InputData[0].Length;
            mAnnModelParameter.OutputNeuronCount = datasets.OutputData[0].Length;
            int inputsCount = mAnnModelParameter.InputNeuronCount;
            int outputsCount = mAnnModelParameter.OutputNeuronCount;
           // mAnnModelParameter.HiddenNeuronsCount = new int[1];
      //      mAnnModelParameter.HiddenNeuronsCount[0] = datasets.InputData[0].Length * 2 + 1;
            mAnnModelParameter.HiddenCount = 1;

            int[] neuronsCount = new int[mAnnModelParameter.HiddenNeuronsCount.Length + 1];
            for (int i = 0; i < mAnnModelParameter.HiddenNeuronsCount.Length; i++)
            {
                neuronsCount[i] = mAnnModelParameter.HiddenNeuronsCount[i];
            }
            neuronsCount[mAnnModelParameter.HiddenNeuronsCount.Length] = outputsCount;

            mNetwork = new ActivationNetwork(actFunc, inputsCount, neuronsCount);
            BackPropagationLearning teacher = new BackPropagationLearning(mNetwork);
            ActivationLayer layer = mNetwork[0];
            teacher.LearningRate = mAnnModelParameter.LearningRate;
            teacher.Momentum = mAnnModelParameter.LearningMomentum;
           
            List<double> arError = new List<double>();
            int solutionSize = datasets.InputData.Length;
            datasets.ForecastedData = new double[solutionSize][];
            int iteration = 1;
            while (!mNeedToStop)
            {
                double error = teacher.RunEpoch(datasets.InputData, datasets.OutputData);
                arError.Add(error);

                double learningError = 0.0;
                double predictionError = 0.0;

                for (int i = 0, n = solutionSize; i < n; i++)
                {
                    datasets.ForecastedData[i] = (double[])mNetwork.Compute(datasets.InputData[i]).Clone();

                    if (i >= n - mAnnModelParameter.MaximumWindowSize)
                    {
                        predictionError += Math.Abs(datasets.OutputData[i][0] - datasets.ForecastedData[i][0]);
                    }
                    else
                    {
                        learningError += Math.Abs(datasets.OutputData[i][0] - datasets.ForecastedData[i][0]);
                    }
                }
                if (iteration >= mAnnModelParameter.Iterations)
                {
                    NeedToStop = true;
                }
                if (learningError <= mAnnModelParameter.DesiredError)
                {
                    NeedToStop = true;
                }
                if (ModelRunningEpoch != null)
                {
                    ModelRunningEpoch(this, new AnnModelRunEpochEventArgs(iteration,error));
                }
                iteration++;
            }

            LayerWeightCollection = new LayerWeight[mNetwork.LayersCount];

            LayerWeightCollection[0].Weight = new double[layer.NeuronsCount][];
            LayerWeightCollection[0].ThreashHold = new double[layer.NeuronsCount][];
            for (int i = 0; i < layer.NeuronsCount; i++)
            {
                LayerWeightCollection[0].Weight[i] = new double[layer.InputsCount];
                LayerWeightCollection[0].ThreashHold[i] = new double[layer.InputsCount];
                for (int j = 0; j < layer.InputsCount; j++)
                {
                    LayerWeightCollection[0].Weight[i][j] = layer[i][j];
                    LayerWeightCollection[0].ThreashHold[i][j] = layer[i][j];
                }
            }

            layer = mNetwork[1];
            LayerWeightCollection[1].Weight = new double[layer.NeuronsCount][];
            LayerWeightCollection[1].ThreashHold = new double[layer.NeuronsCount][];
            for (int i = 0; i < layer.NeuronsCount; i++)
            {
                LayerWeightCollection[1].Weight[i] = new double[layer.InputsCount];
                LayerWeightCollection[1].ThreashHold[i] = new double[layer.InputsCount];
                for (int j = 0; j < layer.InputsCount; j++)
                {
                    LayerWeightCollection[1].Weight[i][j] = layer[i][j];
                    LayerWeightCollection[1].ThreashHold[i][j] = layer[i][j];
                }
            }

            if (ModelFinishRunning != null)
                ModelFinishRunning(this, new ComponentRunEventArgs(datasets));            
        }

        public double Forecast(double[] vector)
        {
            return mNetwork.Compute(vector)[0];
        }
        
        #endregion

        #region IComponent 成员

        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Descriptions
        {
            get;
            set;
        }

        public string Orgnization
        {
            get;
            set;
        }

        public IComponentParameter Parameter
        {
            get
            {
                return mAnnModelParameter;
            }
        }

        #endregion
    }
}
