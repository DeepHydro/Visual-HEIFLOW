// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core
{
    public interface ICalibrationSchema : IDiagramSchema
    {
        ICalibrationModel CalibrationModel { get; }
        IConceptualModel TargetModel{ get; }
        ICalibrationDataSelection CalibrationDataSelectionModel { get; set; }
        ICalibratonDataSets CalibratonDataSets { get; set; }
    }

    public interface ICalibratonDataSets
    {
        double[][] InputData { get; set; }
        double [] ObservedData { get; }
        double[] SimulatedData { get; set; }
        DateTime[] Date { get; }
        /// <summary>
        /// The length of the observed ( Simulated and Date) data vector
        /// </summary>
        int Length { get; }
    }

    public interface ICalibrationDataSelection
    {
        ICalibratonDataSets Select(ICalibrationSchema schema);
    }
}
