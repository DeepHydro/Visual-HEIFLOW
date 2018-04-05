﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Spatial.Geography;


namespace Heiflow.Spatial
{
    public interface IViewTransform
    {
        Point WorldToView(Point point);
        Point ViewToWorld(Point point);
    }
}