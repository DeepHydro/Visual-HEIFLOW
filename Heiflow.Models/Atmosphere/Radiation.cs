// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Models.Atmosphere
{
    public class Radiation 
    {
        public Radiation()
        {

        }

        static Radiation()
        {
            StefanBoltzmannContant = 5.67032e-8;
            AtmosphereCE = 9.37e-6;
            KelvinConstant = 273.15;
        }

        /// <summary>
        /// 5.67032e-8
        /// </summary>
        public static double StefanBoltzmannContant { get; set; }

        /// <summary>
        /// 9.37e-6
        /// </summary>
        public static double AtmosphereCE { get; set; }

        /// <summary>
        /// 273.15
        /// </summary>
        public static double KelvinConstant { get; set; }
    }
}
