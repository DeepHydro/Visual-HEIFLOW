// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Subsurface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic
{
    public static  class ModelService
    {
        public static IBasicModel Model { get; set; }

        public static string WorkDirectory { get; set; }

        public static string ProjectDirectory { get; set; }

        public static DateTime Start { get; set; }

        public static DateTime End { get; set; }
 
        public static int NHRU{ get; set; }
        /// <summary>
        /// in acre
        /// </summary>
        public static double BasinArea { get; set; }

        public static int CurrentGridLayer { get; set; }

        public static int CurrentStressPeriod { get; set; }

        public static int CurrentTimeStep { get; set; }
    }
}
