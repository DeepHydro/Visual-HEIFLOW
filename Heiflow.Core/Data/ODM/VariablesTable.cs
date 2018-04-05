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

namespace Heiflow.Core.Data.ODM
{
    public class VariablesTable : ODMTable
    {

        public VariablesTable()
        {
            _TableName = "Variables";
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
                sql = string.Format("select * from Variables where VariableID={0}", dr["VariableID"]);
                if (ODMDB.Exists(sql))
                {
                    sql = string.Format("update Variables set VariableCode='{1}',	VariableName='{2}',	Specification='{3}',	ValueType='{4}',	DataType='{5}', SampleMedium='{6}'," +
                 "TimeUnitsID={7},	VariableUnitsID={8},	TimeSupport={9},	GeneralCategory='{10}',	NoDataValue='{11}' " +
                 " where VariableID={0}",
                 dr["VariableID"], dr["VariableCode"], dr["VariableName"], dr["Specification"], dr["ValueType"], dr["DataType"],
                 dr["SampleMedium"], dr["TimeUnitsID"], dr["VariableUnitsID"], dr["TimeSupport"], dr["GeneralCategory"], dr["NoDataValue"]);
                }
                else
                {
                    //VariableID	VariableCode	VariableName	Specification	ValueType	DataType	SampleMedium	 TimeUnitsID	VariableUnitsID	TimeSupport	GeneralCategory	NoDataValue
                    sql = string.Format("insert into Variables (VariableID	VariableCode	VariableName	Specification	ValueType	DataType	 SampleMedium" +
                    "TimeUnitsID	VariableUnitsID	TimeSupport	GeneralCategory	NoDataValue)" +
                        "values ({0}, '{1}', '{2}','{3}', '{4}', '{5}','{6}', {7}, {8},{9}, '{10}',{11})",
                           dr["VariableID"], dr["VariableCode"], dr["VariableName"], dr["Specification"], dr["ValueType"], dr["DataType"],
                    dr["SampleMedium"], dr["TimeUnitsID"], dr["VariableUnitsID"], dr["TimeSupport"], dr["GeneralCategory"], dr["NoDataValue"]);
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
