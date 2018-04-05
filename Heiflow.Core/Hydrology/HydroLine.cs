// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core.Hydrology
{
    public class HydroLine : HydroFeature
    {
        public HydroLine(int id)
            : base(id)
        {
            //HydroFeatureType = HydroFeatureType.HydroLine;
            //Width = 20;
            //Length = 1000;
            //TopElevation = 500;
            //Slope = 0.001;
            //BedThick = 3;
        }

        public double Slope { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public double TopElevation { get; set; }
        public double BedThick { get; set; }

        //public int InletNodeIndex { get; set; }
        //public int OutletNodeIndex { get; set; }

        public HydroPoint InletNode { get; set; }
        public HydroPoint OutletNode { get; set; }

        public double ETSW { get; set; }
        public double PPTSW { get; set; }
        public double ROUGHCH { get; set; }
        public double Width1 { get; set; }
        public double Width2 { get; set; }

        /// <summary>
        /// Row * Column = Variable * Time Step
        /// </summary>
        public double[,] AnimationMatrix { get; set; }
    }

}
