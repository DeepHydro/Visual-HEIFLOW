// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Heiflow.Core.Data;
using Heiflow.Core;

namespace Heiflow.Core.Data.ODM
{
    public class ODMImporter
    {
        public ODMImporter(ODMSource odm)
        {
            _ODMSource = odm;
        }

        private ODMSource _ODMSource;

        public void ImportVaribles(string filePath, string sheet)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            excelReader.IsFirstRowAsColumnNames = true;
            System.Data.DataSet result = excelReader.AsDataSet();
            var dt = result.Tables[sheet];
            int nvar = dt.Rows.Count;
            var varibles = new Heiflow.Core.Variable[nvar];
            //VariableID	VariableCode	VariableName	Specification	ValueType	DataType	SampleMedium	
            //TimeUnitsID	VariableUnitsID	TimeSupport	GeneralCategory	NoDataValue
 
            for (int i = 0; i < nvar;i++ )
            {
                var dr= dt.Rows[i];
                varibles[i] = new Heiflow.Core.Variable(int.Parse(dr["VariableID"].ToString()))
                {
                    Code=dr["VariableCode"].ToString(),
                    Name = dr["VariableName"].ToString(),
                    Specification = dr["Specification"].ToString(),
                   ValueType = dr["ValueType"].ToString(),
                    DataType = dr["DataType"].ToString(),
                    SampleMedium = dr["SampleMedium"].ToString(),
                    TimeUnits = EnumHelper.FromString<TimeUnits>(dr["TimeUnitsID"].ToString()),
                    VariableUnitsID = int.Parse(dr["VariableUnitsID"].ToString()),
                    TimeSupport = int.Parse(dr["TimeSupport"].ToString()),
                    GeneralCategory = dr["GeneralCategory"].ToString(),
                    NoDataValue = double.Parse(dr["NoDataValue"].ToString()),
                };
            }
            //_ODMSource.s
            
             stream.Close();
            excelReader.Close();

        }
    }
}
