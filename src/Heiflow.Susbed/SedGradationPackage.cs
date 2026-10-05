// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Hydrodynamics.Susbed
{
    public class SedGradationPackage : Package
    {
        public static string PackageName = "SedGradation";
        public SedGradationPackage()
        {
            Name = PackageName;
        }
        public int NumSections { get; set; }
        public int NumGradations { get; private set; }
        public double[] Gradations { get; private set; }
        public string[] CSNames { get; private set; }
        public int[][,] CSGradation { get; private set; }

        public override LoadingState Load(ICancelProgressHandler progress)
        {
            if (File.Exists(FileName))
            {
                StreamReader sr = new StreamReader(FileName, Encoding.Default);
                var line = sr.ReadLine();
                line = sr.ReadLine();
                Gradations = TypeConverterEx.Split<double>(line);
                NumGradations = Gradations.Length;
                CSNames = new string[NumSections];
                CSGradation = new int[2][,];
                for (int i = 0; i < 2; i++)
                    CSGradation[i] = new int[NumSections, NumGradations];
              
                for (int i = 0; i < 2; i++)
                {
                    line = sr.ReadLine();
                    for (int j = 0; j < NumSections; j++)
                    {
                        line = sr.ReadLine();
                        var strs = TypeConverterEx.Split<string>(line);
                        CSNames[j] = strs[1];
                        for (int k = 0; k < NumGradations; k++)
                        {
                            CSGradation[i][j, k] = int.Parse(strs[2 + k]);
                        }
                    }
                    if (!sr.EndOfStream)
                        line = sr.ReadLine();
                }
                sr.Close();
                return LoadingState.Normal;
            }
            else
            {
                OnLoadFailed("床沙级配文件不存在：" + FileName);
                return LoadingState.FatalError;
            }
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            // The gradation file is written by the pre-processor. The package only reads it.
        }

        public DataTable ToDataTable(int index)
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("断面计算编号", Type.GetType("System.Int32"));
            dt.Columns.Add(dc);
            dc = new DataColumn("工程编号", Type.GetType("System.String"));
            dt.Columns.Add(dc);
            for (int i = 0; i < NumGradations; i++)
            {
                dc = new DataColumn("床沙级配" + (i + 1), Type.GetType("System.Int32"));
                dt.Columns.Add(dc);
            }
            for (int i = 0; i < NumSections; i++)
            {
                var dr = dt.NewRow();
                dr[0] = (i + 1);
                dr[1] = CSNames[i];
                for (int j = 0; j < NumGradations; j++)
                {
                    dr[j + 2] = CSGradation[index][i,j];
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}