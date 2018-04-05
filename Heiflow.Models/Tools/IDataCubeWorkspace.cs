// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Data;
namespace Heiflow.Models.Tools
{
    public interface IDataCubeWorkspace
    {
        event EventHandler DataSourceCollectionChanged;
        void Add(IDataCubeObject mat);
        void Clear();
        bool Contains(string name);
        System.Collections.ObjectModel.ObservableCollection<IDataCubeObject> DataSources { get; }
       IDataCubeObject Get(string name);
        void Remove(string name);

        DataTable ToDataTable();
    }
}
