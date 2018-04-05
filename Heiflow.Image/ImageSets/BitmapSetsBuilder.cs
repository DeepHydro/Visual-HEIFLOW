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

using Heiflow.Image.ColorSpace;
using Heiflow.Image.Recognization;
using Heiflow.Models.AI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Image.ImageSets
{
    public class BitmapSetsBuilder : IImageSetsBuilder
    {
        private Bitmap[] _Sources;
        private Bitmap[] _Targets;
        private IColorSpace _ColorSpace;

        public BitmapSetsBuilder()
        {
            _ColorSpace = new HSVColorSpace();
        }

        public double[][] InputData
        {
            get;
            set;
        }

        public double[][] OutputData
        {
            get;
            set;
        }

        public double[][] ForecastedData
        {
            get;
            set;
        }

        public DateTime[] Date
        {
            get;
            set;
        }

        public int Length
        {
            get;
            protected set;
        }

        public int InputVectorLength
        {
            get;
            protected set;
        }

        public int OutputVectorLength
        {
            get;
            protected set;
        }

        public IColorSpace ColorSpace
        {
            get
            {
                return _ColorSpace;
            }
            set
            {
                _ColorSpace = value;
            }
        }

        public IColorClassifier Classifier
        {
            get;
            set;
        }

        public void Build(Bitmap[] sources, Bitmap[] targets)
        {
            _Sources = sources;
            _Targets = targets;

            var nrow = _Sources[0].Height;
            var ncol = _Sources[0].Width;
            int nvar = _Sources.Length;
            List<double[]> input = new List<double[]>();
            List<double[]> output = new List<double[]>();

            for (int i = 0; i < nvar; i++)
            {
                _ColorSpace.Source = sources[i];
                for (int r = 0; r < nrow; r++)
                {
                    for (int c = 0; c < ncol; c++)
                    {
                        var color = _Targets[i].GetPixel(c,r);
                        double buf = Classifier.Classfiy(color);
                        if (buf != ColorClassifier.NoData)
                        {
                            var stat = _ColorSpace.WindowStat(c, r);
                            input.Add(stat);
                            output.Add(new[] { buf });
                            if (input.Count > 1000)
                                goto limite;
                        }
                    }
                }
            }

            limite:

            InputData = input.ToArray();
            OutputData = output.ToArray();
            Length = input.Count;
            InputVectorLength = 7;
            OutputVectorLength = 1;

        }
    }
}
