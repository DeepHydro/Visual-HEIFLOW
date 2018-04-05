// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Running
{
    public class MonitorItemCollection : MonitorItem
    {
        public MonitorItemCollection(string name)
            : base(name)
        {
            Derivable = false;          
        }


        public override System.Data.DataTable ToDataTable(ListTimeSeries<double> sourcedata)
        {
            int c = 0;
            DataTable dt = new DataTable();
            DataColumn date_col = new DataColumn("Date", Type.GetType("System.DateTime"));
            dt.Columns.Add(date_col);
            foreach(var item in Children)
            {
                DataColumn value_col = new DataColumn(item.Name, Type.GetType("System.Double"));
                dt.Columns.Add(value_col);
                if (item.Derivable && item.DerivedValues == null)
                    item.DerivedValues = item.Derive(sourcedata);
            }

            for (int i = 0; i < sourcedata.Dates.Count; i++)
            {                
                var dr = dt.NewRow();
                dr[0] = sourcedata.Dates[i];
                c = 1;
                foreach (var item in Children)
                {
                    if (item.Derivable)
                        dr[c] = item.DerivedValues[i];
                    else
                        dr[c] = sourcedata.Values[item.VariableIndex][i];  
                    c++;
                }
           
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
}
