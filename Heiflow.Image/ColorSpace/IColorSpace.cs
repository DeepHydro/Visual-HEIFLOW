// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Drawing;
namespace Heiflow.Image.ColorSpace
{
    public interface IColorSpace
    {
        Bitmap Source { get; set; }
        double[] WindowStat(int i, int j);
        double[] ToDouble(int i, int j);
        float[] ToFloat(int i, int j);
    }
}
