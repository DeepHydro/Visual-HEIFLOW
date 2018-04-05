// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
namespace Heiflow.Image.ImageSets
{
    public  interface IColorClassifier
    {
        System.Drawing.Color[] KnownColors { get; set; }
        double[] KnownValues { get; set; }
        /// <summary>
        /// classify input color to known values
        /// </summary>
        /// <param name="input"></param>
        /// <returns>return double.NaN if no known value matches the input color</returns>
        double Classfiy(System.Drawing.Color input);
        void Update();
    }
}
