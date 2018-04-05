// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using System;
namespace Heiflow.Presentation.Services
{
    public  interface IPackageService
    {
        System.Collections.Generic.IEnumerable<Heiflow.Models.Subsurface.IMFPackage> SupportedMFPackages { get; set; }
    }
}
