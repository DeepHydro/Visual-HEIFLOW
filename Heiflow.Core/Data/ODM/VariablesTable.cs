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
