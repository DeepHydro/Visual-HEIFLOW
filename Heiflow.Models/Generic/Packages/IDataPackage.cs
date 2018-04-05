// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Models.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic
{
    public interface IDataPackage:IPackage
    {
        int MaxTimeStep { get; set; }
        string[] Variables { get; }
        int Layer { get; set; }
        int NumTimeStep { get; }
        DateTime End { get; }
        DateTime EndOfLoading { get; set; }
        DateTime Start { get; }
        DateTime StartOfLoading { get; set; }
        MyLazy3DMat<float> Values { get; }
        Heiflow.Models.IO.DataViewMode DataViewMode { get; set; }
        Heiflow.Core.NumericalDataType NumericalDataType { get; set; }
        Heiflow.Core.TimeUnits TimeUnits { get; set; }
        int ODMVariableID { get; set; }
        bool Scan();
        bool Load(int var_index);
    }
}
