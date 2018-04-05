// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic.Parameters
{
    public interface IPackageCoverage
    {
        System.Collections.Generic.List<ArealPropertyInfo> ArealProperties { get; set; }
        string PackageName { get; set; }
        IPackage Package { get; set; }
        int GridLayer { get; set; }
        DataTable LookupTable { get; set; }
        IFeatureSet TargetFeatureSet { get; set; }
        /// <summary>
        /// Full file name with path, extension of the coverage layer
        /// </summary>
        string FullCoverageFileName { get; }
        /// <summary>
        /// Relative file name of the coverage layer that will be saved in the vhfx project
        /// </summary>
        string CoverageFilePath { get; set; }
         string FullLookupTableFileName { get;  }
         string LookupTableFilePath { get; set; }
         string LegendText { get; set; }
         string ID { get; set; }
         ModelObjectState State { get; set; }
        void SaveLookupTable();
        void Clear();
    }
}
