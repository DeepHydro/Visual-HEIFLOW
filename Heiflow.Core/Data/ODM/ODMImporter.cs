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
