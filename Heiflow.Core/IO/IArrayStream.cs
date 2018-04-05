// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
namespace Heiflow.Core.IO
{
    public interface IArrayStream
    {
        event EventHandler<MyLazy3DMat<float>> Loaded;
        event EventHandler<int> Loading;
        int StepsToLoad { get; set; }
        int NumTimeStep { get;  }

        string[] Variables { get; }
        void Scan();
        Heiflow.Core.Data.My3DMat<float> Load();
        Heiflow.Core.Data.My3DMat<float> Load(int var_index);
    }
}
