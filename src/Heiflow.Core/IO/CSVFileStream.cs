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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using ILNumerics;
using Heiflow.Core.Data;
using System.ComponentModel;

namespace Heiflow.Core.IO
{
    public class CSVFileStream
    {
        private string _Filename;
        public static string Comma = ",";
        private StreamReader _StreamReader;

        public CSVFileStream(string filename)
        {
            _Filename = filename;
            HasHeader = true;
        }
        public bool HasHeader
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            protected set;
        }
        public void Save(string content)
        {
            StreamWriter sw = new StreamWriter(_Filename);
            sw.Write(content);
            sw.Close();
        }

        public void Save(DataTable dt)
        {
            string line = "";
            StreamWriter sw = new StreamWriter(_Filename);
            var buf = from DataColumn dc in dt.Columns select dc.ColumnName;
            string head = string.Join(Comma, buf.ToArray());
            sw.WriteLine(head);

            foreach (DataRow dr in dt.Rows)
            {
                line = string.Join(Comma, dr.ItemArray);
                sw.WriteLine(line);
            }
            sw.Close();
        }

        public void Save(Array array, string[] headers)
        {
            int nrow = array.GetLength(0);
            int ncol = array.GetLength(1);
            string line = "";
            StreamWriter sw = new StreamWriter(_Filename);
            if (headers != null)
            {
                line = string.Join(",", headers);
                sw.WriteLine(line);
            }
            for (int i = 0; i < nrow; i++)
            {
                line = "";
                for (int j = 0; j < ncol; j++)
                {
                    line += array.GetValue(i, j) + ",";
                }
                line = line.TrimEnd(TypeConverterEx.Comma);
                sw.WriteLine(line);
            }
            sw.Close();
        }
        public float[,] LoadFloatMatrix()
        {
            StreamReader sr = new StreamReader(_Filename);
            string line = "";
            int nrow = 0;
            int ncol = 0;
            if (HasHeader)
                line = sr.ReadLine();
            else
            {
                line = sr.ReadLine();
                nrow++;
            }
            var strs = TypeConverterEx.Split<string>(line);
            ncol = strs.Length;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (TypeConverterEx.IsNull(line))
                    break;
                nrow++;
            }
            sr.Close();

            sr = new StreamReader(_Filename);
            if (HasHeader)
                line = sr.ReadLine();
            var array = new float[nrow, ncol];
            for (int i = 0; i < nrow; i++)
            {
                line = sr.ReadLine();
                var buf = TypeConverterEx.Split<float>(line);
                for (int j = 0; j < ncol; j++)
                {
                    array[i, j] = buf[j];
                }
            }

            sr.Close();
            return array;
        }
        public Array LoadArray()
        {
            StreamReader sr = new StreamReader(_Filename);
            string line = "";
            if(HasHeader)
                line = sr.ReadLine();
            int nrow = 0;
            int ncol = 0;
            var strs = TypeConverterEx.Split<string>(line);
            ncol = strs.Length;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (TypeConverterEx.IsNull(line))
                    break;
                nrow++;
            }
            sr.Close();

            sr = new StreamReader(_Filename);
            if(HasHeader)
                line = sr.ReadLine();
            var array = new float[nrow, ncol];
            for (int i = 0; i < nrow; i++)
            {
                line = sr.ReadLine();
                var buf = TypeConverterEx.Split<float>(line);
                for (int j = 0; j < ncol; j++)
                {
                    array[i, j] = buf[j];
                }
            }

            sr.Close();
            return array;
        }

        public void Save<T>(DataCube<T> ts)
        {
            if (ts != null)
            {
                string line = "";
                StreamWriter sw = new StreamWriter(_Filename);
                string head = "DateTime," + ts.Name;
                sw.WriteLine(head);

                for (int i = 0; i < ts.DateTimes.Length; i++)
                {
                    line = string.Join(Comma, ts.DateTimes[i].ToString("yyyy-MM-dd"), ts[0,i,0]);
                    sw.WriteLine(line);
                }
                sw.Close();
            }
        }

        /// <summary>
        /// load datatable from .csv file.
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable Load()
        {
            DataTable dtCsv = new DataTable();
            using (StreamReader sr = new StreamReader(_Filename))
            {
                var line = sr.ReadLine();
                string[] rowValues = line.Split(','); //split each row with comma to get individual values  
                for (int j = 0; j < rowValues.Count(); j++)
                {
                    dtCsv.Columns.Add(rowValues[j]); //add headers  
                }
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (TypeConverterEx.IsNotNull(line))
                    {
                        rowValues = line.Split(','); //split each row with comma to get individual values  
                        DataRow dr = dtCsv.NewRow();
                        for (int k = 0; k < rowValues.Count(); k++)
                        {
                            dr[k] = rowValues[k].ToString();
                        }
                        dtCsv.Rows.Add(dr); //add other rows  
                    }
                }
            }

            return dtCsv;
        }

        public void LoadTo(DataTable source)
        {
            StreamReader sr = new StreamReader(_Filename);
            string line = sr.ReadLine().Trim();
            var varlist = (TypeConverterEx.Split<string>(line, TypeConverterEx.Comma)).ToList();
            var colnames = (from DataColumn dc in source.Columns select dc.ColumnName).ToList();
            int nrow = source.Rows.Count;
            var buf = varlist.Intersect(colnames);
            int l = 0;
            if (buf.Any())
            {
                int nvar = buf.Count();
                int[] var_index = new int[nvar];
                int[] col_index = new int[nvar];
                TypeConverter[] converts = new TypeConverter[nvar];
                for (int i = 0; i < nvar; i++)
                {
                    var var_name = buf.ElementAt(i);
                    var_index[i] = varlist.IndexOf(var_name);
                    col_index[i] = colnames.IndexOf(var_name);
                    converts[i] = TypeDescriptor.GetConverter(source.Columns[col_index[i]].DataType);
                }

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine().Trim();
                    var strs = TypeConverterEx.Split<string>(line);
                    if (l < nrow)
                    {
                        var dr = source.Rows[l];
                        for(int c=0;c<nvar;c++)
                        {                 
                            dr[col_index[c]] = converts[c].ConvertFromString(strs[var_index[c]]);
                        }
                    }
                    else
                    {
                        break;
                    }
                    l++;
                }
            }
            sr.Close();
        }

        public void GetContentInfo(ref int nrow, ref int ncol, ref string header)
        {
            nrow = 0;
            StreamReader sr = new StreamReader(_Filename);
            header = sr.ReadLine();
            string line = sr.ReadLine();
            var strs = TypeConverterEx.Split<string>(line);
            nrow++;
            ncol = strs.Length;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if(!TypeConverterEx.IsNull(line))
                    nrow++;
            }
            sr.Close();
        }

        public T[][] Load<T>(int nrow, int ncol)
        {
            T[][] mat = new T[nrow][];
            StreamReader sr = new StreamReader(_Filename);
            var header = sr.ReadLine();
            string line = "";
            for (int i = 0; i < nrow; i++)
            {
                line = sr.ReadLine();
                mat[i] = TypeConverterEx.Split<T>(line);
            }
            sr.Close();
            return mat;
        }

        public ILArray<T> Load<T>()
        {
            int i = 0;
             int nrow=0;
            int ncol=0;
             string header="";
            GetContentInfo(ref  nrow, ref  ncol, ref  header);
            ILArray<T> array = ILMath.zeros<T>(nrow, ncol);
            StreamReader sr = new StreamReader(_Filename);
            try
            {
          
                header = sr.ReadLine();
                string line = "";
                for (i = 0; i < nrow; i++)
                {
                    line = sr.ReadLine();
                    var buf = TypeConverterEx.Split<T>(line);
                    for (int j = 0; j < ncol; j++)
                    {
                        array.SetValue(buf[j], i, j);
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            sr.Close();

            return array;
        }

        public void Open()
        {
            _StreamReader = new StreamReader(_Filename);
        }

        public void Close()
        {
            _StreamReader.Close();
        }

        public string ReadHeader()
        {
            return _StreamReader.ReadLine();
        }

        public T[] ReadLine<T>()
        {
            string line = _StreamReader.ReadLine();
            var vec=  TypeConverterEx.Split<T>(line);
            return vec;
        }
    }
}
