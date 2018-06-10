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
using Heiflow.Core.Data;

namespace Heiflow.Core.IO
{
    public class TxtFileStream
    {
        private string _Filename;
        public static string Comma = ",";
        public TxtFileStream(string filename)
        {
            _Filename = filename;
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
        /// <summary>
        /// load datatable from txt file.
        /// </summary>
        /// <returns>DataTable that has one column with float type</returns>
        public DataTable LoadAsTable()
        {
            DataTable dt = new DataTable();
            StreamReader sr = new StreamReader(_Filename);
            var datatype = Type.GetType("System.Single");
            string line = sr.ReadLine().Trim();
            var buf = TypeConverterEx.Split<string>(line);
            if (buf != null)
            {
                if (TypeConverterEx.IsNumeric(buf[0]))
                {
                    for (int i = 0; i < buf.Length; i++)
                    {
                        DataColumn dc = new DataColumn("A" + i, datatype);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    for (int i = 0; i < buf.Length; i++)
                    {
                        DataColumn dc = new DataColumn(buf[i], datatype);
                        dt.Columns.Add(dc);
                    }
                }
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine().Trim();
                    if (!TypeConverterEx.IsNull(line))
                    {
                        var dr = dt.NewRow();
                        var vec = TypeConverterEx.Split<float>(line);
                        if(vec.Length == dt.Columns.Count)
                        {
                            for(int i=0; i<vec.Length;i++)
                            {
                                dr[i] = vec[i];
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// load datatable from txt file.
        /// </summary>
        /// <returns>DataTable that has one column with float type</returns>
        public float[][] LoadAsFloatMatrix()
        {
            List<float[]> list = new List<float[]>();
            StreamReader sr = new StreamReader(_Filename);
            var datatype = Type.GetType("System.Single");
            string line = sr.ReadLine().Trim();
            var buf = TypeConverterEx.Split<string>(line);

            if (buf != null)
            {
                if (TypeConverterEx.IsNumeric(buf[0]))
                {
                    var vec= TypeConverterEx.Split<float>(line);
                    list.Add(vec);
                }
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine().Trim();
                    if (!TypeConverterEx.IsNull(line))
                    {
                        var vec = TypeConverterEx.Split<float>(line);
                        list.Add(vec);
                    }
                }
            }
            return list.ToArray();
        }

        public int[][] LoadAsIntMatrix()
        {
            List<int[]> list = new List<int[]>();
            StreamReader sr = new StreamReader(_Filename);
            var datatype = Type.GetType("System.Int32");
            string line = sr.ReadLine().Trim();
            var buf = TypeConverterEx.Split<string>(line);

            if (buf != null)
            {
                if (TypeConverterEx.IsNumeric(buf[0]))
                {
                    var vec = TypeConverterEx.Split<int>(line);
                    list.Add(vec);
                }
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine().Trim();
                    if (!TypeConverterEx.IsNull(line))
                    {
                        var vec = TypeConverterEx.Split<int>(line);
                        list.Add(vec);
                    }
                }
            }
            return list.ToArray();
        }

    }
}
