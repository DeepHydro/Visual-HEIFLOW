﻿//
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace Heiflow.Core.Data
{
    public class LookupTable<T>
    {
        private string[] _ColNames;
        private string[] _RowNames;
        private T[] _DefaultValues;

        /// <summary>
        /// [uid][var]
        /// </summary>
        private DataCube<T> _MappingTable;
        private Dictionary<double, int> _RowIndex;
        private Dictionary<string, int> _ColIndex;

        public LookupTable(string[] col_names, string[] row_names, T[] default_values)
        {
            if (!col_names.Contains("ID"))
            {
                var list = col_names.ToList();
                list.Insert(0, "ID");
                _ColNames = list.ToArray();
            }
            else
            {
                _ColNames = col_names;
            }

            _RowNames = row_names;
            _DefaultValues = default_values;
            _MappingTable = new DataCube<T>(1, row_names.Length, _ColNames.Length - 1);
            _RowIndex = new Dictionary<double, int>();
            _ColIndex = new Dictionary<string, int>();

            for (int c = 0; c < col_names.Length; c++)
            {
                for (int r = 0; r < row_names.Length; r++)
                {
                    _MappingTable[0,r,c] = default_values[c];
                }
            }

            for (int i = 0; i < col_names.Length; i++)
            {
                _ColIndex.Add(col_names[i], i);
            }
            for (int i = 0; i < row_names.Length; i++)
            {
                _RowIndex.Add(double.Parse(row_names[i]), i);
            }
        }

        public LookupTable()
        {

        }

        public T NoValue
        {
            get;
            set;
        }

        public string [] RowNames
        {
            get
            {
                return _RowNames;
            }
        }

        /// <summary>
        /// contains the ID column
        /// </summary>
        public string[] ColNames
        {
            get
            {
                return _ColNames;
            }
        }

        public DataCube<T> MappingTable
        {
            get
            {
                return _MappingTable;
            }
        }

        public T GetValue(string col_name, int row_index)
        {
            if (_ColIndex.Keys.Contains(col_name) && row_index < _RowNames.Length)
                return _MappingTable[0,row_index,_ColIndex[col_name]];
            else
                return NoValue;
        }

        public T GetValue(string col_name, double row_name)
        {
            if (_RowIndex.Keys.Contains(row_name) && _ColIndex.Keys.Contains(col_name))
                return _MappingTable[0,_RowIndex[row_name],_ColIndex[col_name]];
            else
                return NoValue;
        }

        public void FromDataTable(DataTable dt)
        {
            int row = 0;
            foreach (DataRow dr in dt.Rows)
            {
                for (int c = 1; c < dt.Columns.Count; c++)
                {
                    _MappingTable[0,row,c - 1] = TypeConverterEx.ChangeType<T>(dr[c]);
                }
                row++;
            }
        }
        /// <summary>
        /// a text file that has the following layout
        /// ID,soil_moist_max,soil_rechr_max,soil_type (First line, column names)
        /// 255,2,1,1
        /// </summary>
        /// <param name="filename"></param>
        public void FromTextFile(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            string line = sr.ReadLine().Trim();
            _ColNames = TypeConverterEx.Split<string>(line);
            List<string> rownames = new List<string>();
            while(!sr.EndOfStream )
            {
                line = sr.ReadLine();
                if(!string.IsNullOrEmpty(line))
                {
                    line = line.Trim();
                    var strs = TypeConverterEx.Split<string>(line);
                    rownames.Add(strs[0]);
                }
            }
            _RowNames = rownames.ToArray();
            _MappingTable = new DataCube<T>(1,_RowNames.Length, _ColNames.Length - 1);
            _RowIndex = new Dictionary<double, int>();
            _ColIndex = new Dictionary<string, int>();

            for (int i = 1; i < _ColNames.Length; i++)
            {
                _ColIndex.Add(_ColNames[i], i - 1);
            }
            for (int i = 0; i < _RowNames.Length; i++)
            {
                _RowIndex.Add(double.Parse(_RowNames[i]), i);
            }
            sr.Close();

            sr = new StreamReader(filename);
            sr.ReadLine();
            for (int i = 0; i < _RowNames.Length; i++)
            {
                line = sr.ReadLine().Trim();
                var vv = TypeConverterEx.SkipSplit<T>(line, 1);
                _MappingTable[0,i.ToString(),":"] = vv;
            }
            sr.Close();
        }

        public void Save(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            string line = "";
            string cols =  string.Join(",", _ColNames);
            sw.WriteLine(cols);
            for (int i = 0; i < _RowNames.Length; i++)
            {
                line = _RowNames[i] + "," + string.Join(",", _MappingTable[0,i.ToString(),":"]);
                sw.WriteLine(line);
            }
            sw.Close();
        }

        public DataTable ToDataTable()
        {
            int ir = 0;
            DataTable dt = new DataTable();
            DataColumn dc_first = new DataColumn("ID", typeof(string));
            dc_first.Unique = true;
            dt.Columns.Add(dc_first);

            for (int i = 1; i < _ColNames.Length; i++)
            {
                DataColumn dc = new DataColumn(_ColNames[i], typeof(T));
                dt.Columns.Add(dc);
            }

            foreach (var row in _RowNames)
            {
                DataRow dr = dt.NewRow();
                dr[0] = row;
                for (int j = 1; j < _ColNames.Length; j++)
                {
                    dr[j] = _MappingTable[0,ir,j - 1];
                }
                dt.Rows.Add(dr);
                ir++;
            }

            return dt;
        }
    }
}
