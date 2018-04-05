// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
namespace Heiflow.Models.Tools
{
    public  interface IModelWorkspace
    {
        void Add(Heiflow.Core.Data.My3DMat<float> mat);
        void Clear();
        bool Contains(string name);
        System.Collections.ObjectModel.ObservableCollection<Heiflow.Core.Data.My3DMat<float>> DataSources { get; }
        Heiflow.Core.Data.My3DMat<float> Get(string name);
        void Remove(string name);
    }
}
