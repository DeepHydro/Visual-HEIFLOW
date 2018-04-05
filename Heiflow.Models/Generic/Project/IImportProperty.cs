// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic.Project
{
    public interface IImportProperty
    {
        string Token { get;  }
        string FileName { get; set; }
        string WorkDirectory { get; set; }
        double OriginX { get; set; }
        double OriginY { get; set; }
        DateTime Start { get; set; }
        ProjectionInfo Projection { get; set; }
    }
}
