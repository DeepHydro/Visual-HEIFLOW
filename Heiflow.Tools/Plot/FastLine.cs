// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Plot
{
    public class FastLine : ModelTool
    {
        public FastLine()
        {
            Name = "FastLine";
            Category = "Plot";
            Description = "Plot fastLine";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            SeriesTitle = "Fast Line Plot";
        }
        [Category("Input")]
        [Description("The title of the plotted ")]
        public string SeriesTitle
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The input matrix. The matrix style should be [-1][0][0] or [0][-1][0] or [0][0][:]")]
        public string MatrixX
        {
            get;
            set;
        }

        public override void Initialize()
        {
            var mx = Validate(MatrixX);
            this.Initialized = mx;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var vecX = GetVector(MatrixX);
            if (vecX != null )
            {
                WorkspaceView.Plot<float>(vecX, SeriesTitle, Models.UI.MySeriesChartType.FastLine);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
