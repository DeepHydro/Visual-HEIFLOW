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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data.ODM
{
    public class DataValuesTable : ODMTable
    {
        public DataValuesTable()
        {
            _TableName = "DataValues";
        }

        public override bool Check(DataTable dt)
        {
            bool succ = true;
            var dt_source = ODM.GetDataTable(TableName, 1);
            var col_external = from DataColumn dc in dt.Columns select dc.ColumnName;
            var col_source = (from DataColumn dc in dt_source.Columns select dc.ColumnName).ToList();
            col_source.Remove("ValueID");

            foreach (var str in col_source)
            {
                if (!col_external.Contains(str))
                {
                    _Message = string.Format("The neccessary field {0} is missing", str);
                    succ = false;
                    break;
                }
            }
            if (!succ)
            {
                return false;
            }
            else
            {
                return succ;
            }
        }

        public override bool Save(System.Data.DataTable dt)
        {
            string sql = "";
            var ODMDB = ODM.ODMDB;
            int progress = 0;
            int total = dt.Rows.Count;
            int i = 1;
            double temp = -123456;
            OnStarted();

            foreach (DataRow dr in dt.Rows)
            {
                temp = -123456;
                double.TryParse(dr["datavalue"].ToString(), out temp);
                if (temp == -123456)
                {
                    _Message += string.Format("Error found in row {0}: {1}\n", i, dr["datavalue"]);
                    temp = 0;
                }
                sql = string.Format("select * from DataValues where SiteID={0} and VariableID = {1} and DateTimeUTC= #{2}#",
                    dr["SiteID"], dr["VariableID"], ((DateTime)dr["DateTimeUTC"]).ToString("yyyy/MM/dd"));
                if (ODMDB.Exists(sql))
                {
                    sql = string.Format("update DataValues set datavalue={0} where SiteID={1} and variableid={2} and datetimeutc=#{3}#",
                        temp, dr["SiteID"], dr["VariableID"],
                        ((DateTime)dr["DateTimeUTC"]).ToString("yyyy/MM/dd HH:mm:ss")
                         );
                    ODMDB.CreateNonQueryCommand(sql);
                }
                else
                {
                    sql = string.Format("insert into DataValues (SiteID,VariableID,DateTimeUTC,DataValue)  values ({0}, {1},  #{2}#,{3})",
                        dr["SiteID"],
                        dr["VariableID"],
                        ((DateTime)dr["DateTimeUTC"]).ToString("yyyy/MM/dd HH:mm:ss"),
                       temp
                         );
                    ODMDB.CreateNonQueryCommand(sql);
                }
                progress = i * 100 / total;
                i++;
                OnProgressChanged(progress);
            }

            OnProgressChanged(0);
            UpdateSeriesCatalog(dt);

            OnFinished();
            return true;
        }

        private void UpdateSeriesCatalog(System.Data.DataTable dt)
        {
            var dt_eu=dt.AsEnumerable();
            var sites = (from dr in dt_eu select int.Parse(dr["SiteID"].ToString())).Distinct();
            var ODMDB = ODM.ODMDB;
            int total = sites.Count();
            int i = 1;
            int progress = 0;

            foreach(int site_id in sites)
            {
                var vars = (from dr in dt_eu where int.Parse(dr["SiteID"].ToString()) == site_id select int.Parse(dr["VariableID"].ToString())).Distinct();
             
                foreach(var var_id in vars)
                {
                    // update SeriesCatalog table
                    var sql = string.Format("select * from SeriesCatalog where SiteID={0} and VariableID = {1}", site_id, var_id);
                    if (ODMDB.Exists(sql))
                    {
                        sql = string.Format("delete * from SeriesCatalog where SiteID={0} and VariableID = {1}", site_id, var_id);
                        ODMDB.CreateNonQueryCommand(sql);
                    }

                    sql = string.Format("select DateTimeUTC from DataValues where SiteID={0} and VariableID = {1}", site_id, var_id);
                    var dt_cata = ODMDB.QueryDataTable(sql);
                    if (dt_cata != null)
                    {
                        var dates = from r in dt_cata.AsEnumerable() select r.Field<DateTime>("DateTimeUTC");
                        sql = string.Format("insert into SeriesCatalog (SiteID,VariableID,BeginDateTime,EndDateTime,ValueCount)  values ({0}, {1},  #{2}#, #{3}#, {4})",
                        site_id, var_id, dates.Min().ToString("yyyy/MM/dd"), dates.Max().ToString("yyyy/MM/dd"), dates.Count());
                        ODMDB.CreateNonQueryCommand(sql);
                    }
                }
                progress = i * 100 / total;
                i++;
                OnProgressChanged(progress);
            }
        }
    }
}
