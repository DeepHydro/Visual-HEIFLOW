// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
namespace Heiflow.Models.Running
{
    public interface IRunningWorker
    {
        event EventHandler<string> Echoed;
        event EventHandler Completed;
        bool IsBusy { get; }
        string ExePath { get; set; }
        string WorkingDirectory { get; set; }
        string OutputInfo { get; }
        void Start(object arg);
        void Stop();
     
    }
}
