// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Heiflow.AI;
using Heiflow.AI.NeuronDotNet.Core.SOM;
using System.Drawing;
using Heiflow.AI.NeuronDotNet.Core.Initializers;
using Heiflow.AI.NeuronDotNet.Core;
using Heiflow.Models.AI;
using Heiflow.Core.Data;

namespace Heiflow.Core.DataDriven
{
    public class SOMModel : IMultivariateAnalysis
    {
        public SOMModel(SOMParameter parameter)
        {
            Name = "Self-organizing Map"; 
            Orgnization = "HUST WREIS";
            ID = "71ACCE8D-9783-4637-83EB-25ABE24F953D";
            Descriptions = "";
            mSOMParameter = parameter;
            mSOMParameter.Component = this;
        }

        private SOMParameter mSOMParameter;
        private KohonenNetwork mNetwork;
        private TrainingSet mTrainingSet;
        private List<KohonenMapClassification> mMapClassifications;

        public event ComponentRunEpochEventHandler ModelRunningEpoch;
        public event ComponentRunEventHandler ModelStartRunning;
        public event ComponentRunEventHandler ModelFinishRunning;
       
        public IComponentParameter Parameter
        {
            get
            {
                return mSOMParameter;
            }
        }

        public KohonenNetwork Network
        {
            get
            {
                return mNetwork;
            }
        }

        public List<KohonenMapClassification> MapClassifications
        {
      get
            {
                return mMapClassifications;
            }
        }

        #region IMultivariateAnalysis 成员

        public void Analyze(IForecastingDataSets datasets)
        {
            if (ModelStartRunning != null)
                ModelStartRunning(this, new ComponentRunEventArgs(datasets));
            int learningRadius = Math.Max(mSOMParameter.LayerWidth, mSOMParameter.LayerHeight) / 2;

            KohonenLayer inputLayer = new KohonenLayer(datasets.InputData[0].Length);
            KohonenLayer outputLayer = new KohonenLayer(new Size(mSOMParameter.LayerWidth, mSOMParameter.LayerHeight),
                mSOMParameter.NeighborhoodFunction, mSOMParameter.Topology);
            KohonenConnector connector = new KohonenConnector(inputLayer, outputLayer);
            connector.Initializer = new RandomFunction(0, 100);
            outputLayer.SetLearningRate(mSOMParameter.LearningRate, mSOMParameter.FinalLearningRate);
            outputLayer.IsRowCircular = mSOMParameter.IsRowCircular;
            outputLayer.IsColumnCircular = mSOMParameter.IsColumnCircular;
            mNetwork = new KohonenNetwork(inputLayer, outputLayer);

            mNetwork.EndEpochEvent += new TrainingEpochEventHandler(
                delegate(object senderNetwork, TrainingEpochEventArgs args)
                {
                    if(ModelRunningEpoch != null)
                        ModelRunningEpoch(this, new ComponentRunEpochEventArgs(args.TrainingIteration));
                });
            mTrainingSet=ForecastingDataSets.ConvertToUnSupervisedTrainingSet(datasets);
            mNetwork.Learn(mTrainingSet, mSOMParameter.Iterations);

            if (ModelFinishRunning != null)
                ModelFinishRunning(this, new ComponentRunEventArgs(datasets));
        }

        public int [,] GetWinnerFreqency()
        {
            return (mNetwork.OutputLayer as KohonenLayer).CalculateWinnerFreqency(mTrainingSet, out mMapClassifications);
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

        #endregion
    }
}
