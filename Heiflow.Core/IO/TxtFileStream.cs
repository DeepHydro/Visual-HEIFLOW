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

        public My3DMat<float> LoadAsDC()
        {
            var mat = LoadAsFloatMatrix();
            var buf = new float[1][][];
            buf[1] = mat;
            My3DMat<float> mat_3d = new My3DMat<float>(buf);
            return mat_3d ;
        }
    }
}
