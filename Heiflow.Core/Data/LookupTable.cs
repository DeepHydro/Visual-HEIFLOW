// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
        private My2DMat<T> _MappingTable;
        private Dictionary<string, int> _RowIndex;
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
            _MappingTable = new My2DMat<T>(row_names.Length, _ColNames.Length - 1);
            _RowIndex = new Dictionary<string, int>();
            _ColIndex = new Dictionary<string, int>();

            for (int c = 0; c < col_names.Length; c++)
            {
                for (int r = 0; r < row_names.Length; r++)
                {
                    _MappingTable.Value[r][c] = default_values[c];
                }
            }

            for (int i = 0; i < col_names.Length; i++)
            {
                _ColIndex.Add(col_names[i], i);
            }
            for (int i = 0; i < row_names.Length; i++)
            {
                _RowIndex.Add(row_names[i], i);
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

        public My2DMat<T> MappingTable
        {
            get
            {
                return _MappingTable;
            }
        }

        public T GetValue(string col_name, int row_index)
        {
            if (_ColIndex.Keys.Contains(col_name) && row_index < _RowNames.Length)
                return _MappingTable.Value[row_index][_ColIndex[col_name]];
            else
                return NoValue;
        }

        public T GetValue(string col_name, string row_name)
        {
            if (_RowIndex.Keys.Contains(row_name) && _ColIndex.Keys.Contains(col_name))
                return _MappingTable.Value[_RowIndex[row_name]][_ColIndex[col_name]];
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
                    _MappingTable.Value[row][c - 1] = TypeConverterEx.ChangeType<T>(dr[c]);
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
            _MappingTable = new My2DMat<T>(_RowNames.Length, _ColNames.Length - 1);
            _RowIndex = new Dictionary<string, int>();
            _ColIndex = new Dictionary<string, int>();

            for (int i = 1; i < _ColNames.Length; i++)
            {
                _ColIndex.Add(_ColNames[i], i - 1);
            }
            for (int i = 0; i < _RowNames.Length; i++)
            {
                _RowIndex.Add(_RowNames[i], i);
            }
            sr.Close();

            sr = new StreamReader(filename);
            sr.ReadLine();
            for (int i = 0; i < _RowNames.Length; i++)
            {
                line = sr.ReadLine().Trim();
                var vv = TypeConverterEx.SkipSplit<T>(line, 1);
                _MappingTable.Value[i] = vv;
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
                line = _RowNames[i] + "," + string.Join(",", _MappingTable.Value[i]);
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
                    dr[j] = _MappingTable.Value[ir][j - 1];
                }
                dt.Rows.Add(dr);
                ir++;
            }

            return dt;
        }
    }
}
