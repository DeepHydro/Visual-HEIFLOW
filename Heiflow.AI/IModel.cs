// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Models.AI
{
    public interface IComponent
    {
        string ID { get; set; }
        string Name { get; set; }
        string Descriptions { get; set; }
        string Orgnization { get; set; }
        IComponentParameter Parameter  { get;}
        event ComponentRunEpochEventHandler ModelRunningEpoch;
        event ComponentRunEventHandler ModelStartRunning;
        event ComponentRunEventHandler ModelFinishRunning;
    }

    public interface IComponentParameter
    {
        IComponent Component { get; set; }
    }

    public interface IForecastingDataSets
    {
        double[][] InputData { get; set; }
        double[][] OutputData { get; set; }
        double[][] ForecastedData { get; set; }
        DateTime[] Date { get; set; }
        /// <summary>
        /// The total number of input vector
        /// </summary>
        int Length { get; }
        /// <summary>
        /// The length of an individual input vector
        /// </summary>
        int InputVectorLength { get; }
        /// <summary>
        /// The length of an individual output vector
        /// </summary>
        int OutputVectorLength { get; }
    }


    

}
