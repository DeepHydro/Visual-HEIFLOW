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
