// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Heiflow.Image.Recognization
{
    public interface IRecognizer
    {
        Heiflow.Core.IForecastingModel Model { get; set; }
        void Train(Bitmap[] sources, Bitmap[] targets);
        Bitmap Recognize(Bitmap image);
     
    }
}
