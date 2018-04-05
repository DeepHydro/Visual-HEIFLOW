// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI.NeuronDotNet.Core
{
    /// <summary>
    /// Mode of connection between layers.
    /// </summary>
    public enum ConnectionMode
    {
        /// <summary>
        /// A connection mode where all neurons of source layer are connected to all neurons of
        /// target layer
        /// </summary>
        Complete = 0,

        /// <summary>
        /// A connection mode where each neuron in source layer is connected to a single distinct
        /// neuron in the target layer. The source and target layers should have same number of
        /// neurons.
        /// </summary>
        OneOne = 1
    }
}
