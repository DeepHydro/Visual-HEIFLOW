// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data.ODM
{
    public interface IDendritiRecord<T>
    {
        int ID { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        bool CanDelete { get; set; }
        bool CanExport2Shp { get; set; }
        bool CanExport2Excel { get; set; }
        IDendritiRecord<T> Parent { get; set; }
        List<IDendritiRecord<T>> Children { get; set; }
        T Value { get; set; }
        int Level { get; set; }
        object Tag { get; set; }
    }
}
