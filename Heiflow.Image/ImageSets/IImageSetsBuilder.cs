// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Image.ColorSpace;
using Heiflow.Models.AI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Image.ImageSets
{
    public interface IImageSetsBuilder : IForecastingDataSets
    {
        IColorClassifier Classifier { get; set; }
        IColorSpace ColorSpace { get; set; }
        void Build(Bitmap[] sources, Bitmap[] targets);
    }
}
