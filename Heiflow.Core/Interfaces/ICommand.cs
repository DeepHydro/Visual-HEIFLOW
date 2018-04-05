// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core
{
    public interface ICommandItem
    {
        string Caption { get; set; }
        string Message { get; set; }
        string Name { get; set; }
        object Tag { get; set; }
        void Delete();
        void Execute();
        void Refresh();
    }
    public interface ICommandBars
    {
        ICommandItem Add();
        int Count { get; }
        bool IsVisible();
        ICommandItem GetItem(int Index);
    }
}
