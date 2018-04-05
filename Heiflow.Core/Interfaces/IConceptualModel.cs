// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Heiflow.Models.AI;

namespace Heiflow.Core
{
    public interface IConceptualModel:IComponent
    {
        double Simulate(double[] inputVector);
    }

    public interface ICalibrationModel:IComponent
    {
        void Calibrate(ICalibratonDataSets datasets);
    }
}
