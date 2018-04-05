// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface.Packages
{
     [PackageItem]
    [Export(typeof(IMFPackage))]
    [PackageCategory("Boundary Conditions,Head Dependent Flux",false)]
    public class ESFR : MFPackage
    {
        public static string PackageName = "SFR";

        public ESFR()
        {
            Name = "Enhanced SFR"; 
            _PackageInfo.Format = FileFormat.Text;
            _PackageInfo.IOState = IOState.OLD;
            _PackageInfo.FileExtension = ".sfr";
            _PackageInfo.ModuleName = "SFR";
            Description = "Enhanced SFR Package is developed by Prof. Huang at Tongji University; It provides advanced stream routing functionality, which enables to  simulate streamflow more accurately";
 
        }
     }
}
