//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

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
