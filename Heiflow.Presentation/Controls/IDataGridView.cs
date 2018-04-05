// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using ILNumerics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Parameters;

namespace Heiflow.Presentation.Controls
{
    public interface IDataGridView
    {
        string DataObjectName { get; set; }   
        void Bind<T>(ILArray<T> data);
        void Bind<T>(T[] array);
        void Bind<T>(T[][] array);
        void Bind(DataTable table);
        void Bind(IDataCubeObject dc);
        void Bind(IParameter[] paras);
        void ShowView();
    }
}
