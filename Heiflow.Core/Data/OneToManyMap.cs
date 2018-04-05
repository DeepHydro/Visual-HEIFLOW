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

using Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Heiflow.Core.Data
{
    /// <summary>
    /// one to many mapping
    /// </summary>
    /// <typeparam name="T">ID type</typeparam>
    /// <typeparam name="T1">Mapped value type</typeparam>
    public class OneToManyMap<T, T1>
    {
        private T[] mIDs;
        private string[] varNames;
        private T1[,] mValues;
        public Dictionary<T, T1>[] Mapping { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="varname"></param>
        /// <param name="values">[ids.length,varname.length]</param>
        public OneToManyMap(T[] ids, string[] varname, T1[,] values)
        {
            varNames = varname;
            mIDs = ids;
            mValues = values;
            Build();
        }


        private void Build()
        {
            Mapping = new Dictionary<T, T1>[varNames.Length];
            for (int v = 0; v < varNames.Length; v++)
            {
                Mapping[v] = new Dictionary<T, T1>();
                for (int i = 0; i < mIDs.Length; i++)
                {
                    Mapping[v].Add(mIDs[i], mValues[i, v]);
                }
            }
        }

        public T1[] Map(T[] ids, string varname)
        {
            T1[] mappedArray = new T1[ids.Length];
            int variableIndex = 0;
            for (int i = 0; i < varNames.Length; i++)
            {
                if (varname == varNames[i])
                {
                    variableIndex = i;
                    break;
                }
            }
            for (int i = 0; i < ids.Length; i++)
            {
                mappedArray[i] = Mapping[variableIndex][ids[i]];
            }
            return mappedArray;
        }

        public static OneToManyMap<T, T1> Read(string excelfile, string sheet, string keyField, string [] skippedFields)
        {
            FileStream stream = File.Open(excelfile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            excelReader.IsFirstRowAsColumnNames = true;
            System.Data.DataSet result = excelReader.AsDataSet();
            var dt = result.Tables[sheet];
            var dtEnum = dt.AsEnumerable();
            int num = dt.Rows.Count;

            var varNames = new List<string>();
            var varColIndex = new List<int>();

            for (int c = 0; c < dt.Columns.Count; c++)
            {
                var coln=dt.Columns[c].ColumnName;
                if (!skippedFields.Contains(coln) && coln != keyField)
                {
                    varNames.Add(dt.Columns[c].ColumnName);
                    varColIndex.Add(c);
                }
            }
            T[] ids = (from r in dtEnum select TypeConverterEx.ChangeType<T>(r.Field<string>(keyField))).ToArray();
            T1[,] values = new T1[num, varNames.Count];

            for (int i = 0; i < num;i++ )
            {
                for (int c = 0; c < varNames.Count; c++)
                {
                    values[i, c] = TypeConverterEx.ChangeType<T1>(dt.Rows[i][varColIndex[c]].ToString());
                }
            }

             return new OneToManyMap<T, T1>(ids, varNames.ToArray(), values);
        }
    

    }
}
