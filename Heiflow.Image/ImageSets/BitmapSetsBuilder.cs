// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
