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
    public  interface IFileMonitor
    {
        string MonitorName { get; }
        string FileName { get; set; }        
        List<IMonitorItem>  Root { get; set; }
        Heiflow.Models.IO.IArrayWatcher Watcher { get; set; }
        ListTimeSeries<double>  DataSource { get; }
        int CurrentStep { get; set; }

        List<IFileMonitor> Partners { get; }
        bool IsStarted { get; }
        void Start();
        void Stop();
        void Clear();
        DataTable Balance( ref string budget);
        Dictionary<string, double> ZonalBudgets();
 
    }
}
