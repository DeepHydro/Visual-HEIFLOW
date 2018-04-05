// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Models.Tools;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Statisitcs
{
    public class CDFPlot : ModelTool
    {
        public CDFPlot()
        {
            Name = "Cumulative Distribution Functions";
            Category = "Statistics";
            Description = "Calculate and plot CDF";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The input matrix. The matrix style should be [-1][0][0] or [0][-1][0] or [0][0][:]")]
        public string Matrix
        {
            get;
            set;
        }

        public override void Initialize()
        {
            this.Initialized = Validate(Matrix);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var vec = GetVector(Matrix);
            if (vec != null)
            {
                var dou_vec = MyMath.ToDouble(vec);
                string vec_name = "CDF of " + GetName(Matrix);
                int nlen = dou_vec.Length;
                cancelProgressHandler.Progress("Package_Tool", 10, "Calculating...");
                Array.Sort<double>(dou_vec);
                var cdf = MathNet.Numerics.Statistics.Statistics.EmpiricalCDFFunc(dou_vec);
                var cdf_value = new double[nlen];
                for (int i = 0; i < nlen; i++)
                {
                    cdf_value[i] = cdf(dou_vec[i]);
                }
                WorkspaceView.Plot<double>(dou_vec, cdf_value,  vec_name, Models.UI.MySeriesChartType.FastLine);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
