// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Data;
namespace Heiflow.Models.Running
{
    public interface IMonitorItem
    {
        List<MonitorItem> Children { get; set; }
        IFileMonitor Monitor { get; set; }
        string Group { get; set; }
        string Name { get; }
        MonitorItem Parent { get; set; }
        int VariableIndex { get; set; }
        SequenceType SequenceType { get; set; }

        DataTable ToDataTable(ListTimeSeries<double> sourcedata);
    }
}
