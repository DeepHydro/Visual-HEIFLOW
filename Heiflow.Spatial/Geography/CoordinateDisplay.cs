// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Net;

namespace Heiflow.Spatial.Geography
{
    /// <summary>
    /// This class is used to display
    /// geographic co-ordinates in different
    /// formats
    /// </summary>
    public class CoordinateDisplay
    {
        /// <summary>
        /// Displays the longitude coordinate with hemisphere information
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public static string LatitudeToString(double latitude)
        {
            string hem = "N";
            if (latitude < 0) hem = "S";
            return (Math.Abs(latitude)).ToString("0.000") + " " + hem;
        }

        /// <summary>
        /// Displays the latitude coordinate with hemisphere information
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static string LongitudeToString(double longitude)
        {
            string hem = "E";
            if (longitude < 0) hem = "W";
            return (Math.Abs(longitude)).ToString("0.000") + " " + hem;
        }
    }
}
