// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Net;

namespace Heiflow.Spatial
{
    public enum ModelType
    {
        Gate = 0,
        HydroStation = 1,
        WeatherStation = 2,
        PowerReservoir = 3,
        MonitoringPoint = 4,
        ModelNull = 5,
        River = 6,
        GroundwaterStation = 7,
        PricipitatinStation = 8,
    }

    public enum GeometryType
    {
        GeometryNull = 0,
        GeometryPoint = 1,
        GeometryMultipoint = 2,
        GeometryLine = 3,
        GeometryPolygon = 4,
    }
}
