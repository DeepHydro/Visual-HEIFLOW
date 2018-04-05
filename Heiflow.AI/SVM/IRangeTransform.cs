// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI.SVM
{
    /// <summary>
    /// Interface implemented by range transforms.
    /// </summary>
    public interface IRangeTransform
    {
        /// <summary>
        /// Transform the input value using the transform stored for the provided index.
        /// </summary>
        /// <param name="input">Input value</param>
        /// <param name="index">Index of the transform to use</param>
        /// <returns>The transformed value</returns>
        double Transform(double input, int index);
        /// <summary>
        /// Transforms the input array.
        /// </summary>
        /// <param name="input">The array to transform</param>
        /// <returns>The transformed array</returns>
        Node[] Transform(Node[] input);
    }
}
