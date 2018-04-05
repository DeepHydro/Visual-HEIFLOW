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