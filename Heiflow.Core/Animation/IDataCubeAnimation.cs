// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
namespace Heiflow.Core.Animation
{
    public interface IDataCubeAnimation:IAnimation
    {
        event EventHandler<IDataCubeObject> DataSourceChanged;
        event EventHandler Stopped;
        string Name { get; }
        Heiflow.Core.Data.IDataCubeObject DataSource { get; set; }
        void Cache();
    }
}
