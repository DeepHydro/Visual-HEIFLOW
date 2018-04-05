// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using AForge.Imaging;
using Heiflow.Core;
using Heiflow.Image.ImageSets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Heiflow.Image.Recognization
{
    public class ImageRecognizer : IRecognizer
    {
        private IForecastingModel _Model;
        private IImageSetsBuilder _Builder;

        public ImageRecognizer(IForecastingModel model, IImageSetsBuilder builder, IColorClassifier classifier)
        {
            _Model = model;
            _Builder = builder;
            _Builder.Classifier = classifier;
        }

        public IForecastingModel Model
        {
            get
            {
                return _Model;
            }
            set
            {
                _Model = value;
            }
        }

        public void Train(Bitmap[] sources, Bitmap[] targets)
        {
            _Builder.Build(sources, targets);
            _Model.Train(_Builder);
        }

        public Bitmap Recognize(Bitmap image)
        {
            Bitmap recognized = new Bitmap(image);
            _Builder.ColorSpace.Source = image;
            var nrow = image.Height;
            var ncol = image.Width;

            for (int i = 0; i < nrow; i++)
            {
                for (int j = 0; j < ncol; j++)
                {
                    var stat = _Builder.ColorSpace.WindowStat(i, j);
                    var forcasted = _Model.Forecast(stat);
                }
            }
               
            //
            //throw new NotImplementedException();
            return null;
        }
    }
}