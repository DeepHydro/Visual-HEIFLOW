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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Hydrodynamics.Susbed
{
    public class FlowOutPackage : Package
    {
        public FlowOutPackage()
        {
            Values = new List<double[]>();
            Times = new List<double>();
        }

        public List<double[]> Values { get; set; }
        public List<double> Times { get; set; }

        public int NumTimes
        {
            get
            {
                return Times.Count;
            }
        }

        public int NumSections
        {
            get
            {
                return Values[0].Length;
            }
        }

        public override LoadingState Load(ICancelProgressHandler progress)
        {
            if (File.Exists(FileName))
            {
                Values.Clear();
                Times.Clear();
                StreamReader sr = new StreamReader(FileName, Encoding.Default);
                var line = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (TypeConverterEx.IsNotNull(line))
                    {
                        var buf = TypeConverterEx.Split<double>(line);
                        Times.Add(buf[0]);
                        buf = TypeConverterEx.SkipSplit<double>(line, 1);
                        Values.Add(buf);
                    }
                }
                sr.Close();
                return LoadingState.Normal;
            }
            else
            {
                OnLoadFailed("输出文件不存在：" + FileName);
                return LoadingState.FatalError;
            }
        }

        public override void SaveAs(string filename, ICancelProgressHandler progress)
        {
            if (NumTimes == 0 || Values.Count == 0)
                return;
            var sw = new StreamWriter(filename);
            sw.WriteLine("Time\t" + string.Join("\t", Enumerable.Range(1, NumSections)));
            for (int i = 0; i < NumTimes; i++)
            {
                sw.WriteLine(Times[i] + "\t" + string.Join("\t", Values[i]));
            }
            sw.Close();
        }

        public double[] GetValueAt(int sec_index)
        {
            var vv = new double[NumTimes];
            for (int i = 0; i < NumTimes; i++)
            {
                vv[i] = Values[i][sec_index];
            }
            return vv;
        }

        public double[,] ToArray()
        {
            double[,] array = new double[NumTimes, NumSections];
            for (int i = 0; i < NumTimes; i++)
            {
                for (int j = 0;j < NumSections; j++)
                {
                    array[i, j] = Values[i][j];
                }
            }

            return array;
        }
    }
}