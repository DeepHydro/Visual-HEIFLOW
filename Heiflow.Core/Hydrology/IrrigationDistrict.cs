// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Hydrology
{
    public class IrrigationDistrict:HydroArea
    {
        public IrrigationDistrict(int id):base(id)
        {
            
        }

        public int RiverID	 {get;set;}

        public int ReachID { get; set; }

        public int DatasourceFlag { get; set; }

        public IVectorTimeSeries<double> Divserion { get; set; }

        public double Scale { get; set; }

        public int[] HRUs { get; set; }

        public double[] HRURatio { get; set; }

        public IrrigationDistrict[] Childs { get; set; }
    }
}
