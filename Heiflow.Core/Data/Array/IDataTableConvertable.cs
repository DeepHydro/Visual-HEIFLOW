// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    public interface IDataTableConvertable
    {
        DataTable ToDataTableByTime(int var_index, int time_index);
        DataTable ToDataTableBySpace(int var_index, int space_index);
        DataTable ToDataTable();
        void FromDataTable(System.Data.DataTable dt);
    }
}
