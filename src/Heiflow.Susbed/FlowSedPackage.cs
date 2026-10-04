// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System.Data;
using DotSpatial.Data;
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Heiflow.Core.Data;

namespace Heiflow.Models.Hydrodynamics.Susbed
{
    public class FlowSedPackage : Package
    {
        public static string PackageName = "FlowSed";
        public FlowSedPackage()
        {
            Name = PackageName;
        }

        public string[] DataFiles { get; set; }
        public string[] TableNames { get; set; }

        public override LoadingState Load(ICancelProgressHandler progress)
        {
            // The data files are loaded on demand through LoadTo(int index)
            return LoadingState.Normal;
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            // The flow and sediment files are produced by the model, they are read only here.
        }

        public void GetTableName(int index)
        {
            var file = Path.Combine(ModelService.WorkDirectory, DataFiles[index]);
            if (File.Exists(file))
            {
                StreamReader sr = new StreamReader(file, Encoding.Default);
                var line = sr.ReadLine();
                if (TypeConverterEx.IsNull(line))
                    TableNames[index] = "未命名";
                else
                    TableNames[index] = line;
                sr.Close();
            }
        }

        public DataTable LoadTo(int index)
        {
            DataTable dt = new DataTable();
            var file = Path.Combine(ModelService.WorkDirectory, DataFiles[index]);
            if (File.Exists(file))
            {
                StreamReader sr = new StreamReader(file, Encoding.Default);
                var line = sr.ReadLine();
                dt.TableName = line;
                line = sr.ReadLine();
                line = sr.ReadLine();
                var strs = TypeConverterEx.Split<string>(line);
                for (int i = 0; i < strs.Length; i++)
                {
                    DataColumn dc = new DataColumn(strs[i], Type.GetType("System.Double"));
                    dt.Columns.Add(dc);
                }
                line = sr.ReadLine();
                line = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (TypeConverterEx.IsNotNull(line))
                    {
                        var buf = TypeConverterEx.Split<double>(line);
                        if (buf != null && buf.Length == dt.Columns.Count)
                        {
                            var dr = dt.NewRow();
                            for (int i = 0; i < buf.Length; i++)
                            {
                                dr[i] = buf[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                sr.Close();
            }
            return dt;
        }
    }
}
