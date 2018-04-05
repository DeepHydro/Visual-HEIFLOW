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
using System.IO;
using Heiflow.Core.IO;

namespace Heiflow.Core.Data.ODM
{
    public class CVTable : ODMTable
    {
        public CVTable(string table_name)
        {
            _TableName = table_name;
        }


        public override bool Save(System.Data.DataTable dt)
        {
            string sql = "";
            var ODMDB = ODM.ODMDB;
            int progress = 0;
            int total = dt.Rows.Count;
            int i = 1;

            OnStarted();

            foreach (DataRow dr in dt.Rows)
            {
                sql = string.Format("select * from {0} where Term='{1}'", _TableName, dr["Term"]);
                if (ODMDB.Exists(sql))
                {
                    sql = string.Format("update {2} set Definition='{1}' where Term='{0}'",
           dr["Term"], dr["Definition"],_TableName);
                }
                else
                {
                    sql = string.Format("insert into {2} (Term, Definition) " +
                    " values ('{0}', '{1}')",
                     dr["Term"], dr["Definition"], _TableName);
                }
                ODMDB.CreateNonQueryCommand(sql);
                progress = i * 100 / total;
                i++;
                OnProgressChanged(progress);
            }
            OnFinished();
            return true;
        }

    }
}
