// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core;
using Heiflow.Core.DataDriven;
using Heiflow.Image.ImageSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Image.Recognization
{
    public class RecognizerFactory
    {
        private IImageSetsBuilder _IImageSetsBuilder;
        private IColorClassifier _IColorClassification;
        public RecognizerFactory(IImageSetsBuilder builder, IColorClassifier classifier)
        {
            _IColorClassification = classifier;
            _IImageSetsBuilder = builder;
        }

        public List<IForecastingModel> Models
        {
            get;
            set;
        }

        public List<IRecognizer> Recognizers
        {
            get;
            set;
        }

        public void Initialize()
        {
            IForecastingModel model = null;
            Models = new List<IForecastingModel>();

            AnnModelParameter annPara = new AnnModelParameter();
            model = new NeuralNetworkModel(annPara);
            Models.Add(model);

            Heiflow.AI.SVM.Parameter p = new Heiflow.AI.SVM.Parameter();
            model = new SVMModel(p);
            Models.Add(model);

            ModelParameter mp = new ModelParameter();
            model = new MLRModel(mp);
            Models.Add(model);

            Recognizers = new List<IRecognizer>();

            foreach (var mm in Models)
            {
                var recognizer = new ImageRecognizer(mm, _IImageSetsBuilder, _IColorClassification);
                Recognizers.Add(recognizer);
            }
        }

        public IRecognizer Select(string name)
        {
            IRecognizer recognizer = null;

            var reg = from mo in Recognizers where mo.Model.Name == name select mo;
            if (reg.Count() == 1)
                recognizer = reg.First();
            return recognizer;
        }
    }
}
