﻿using Heiflow.Models.Visualization;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
namespace Heiflow.Models.Generic
{
    public  interface IGridRenderFactory
    {
      IEnumerable<I3DLayerRender> Renders { get; }
      I3DLayerRender Select(string rendername);
      void Add(I3DLayerRender render);
      void Composite();
    }
}