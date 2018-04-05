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
    public class Scatter : ModelTool
    {
        public Scatter()
        {
            Name = "Scatter";
            Category = "Plot";
            Description = "Plot scatter";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            SeriesTitle = "Scatter Plot";
        }
        [Category("Input")]
        [Description("The title of the plotted ")]
        public string SeriesTitle
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The X matrix. The matrix style should be [-1][0][0] or [0][-1][0] or [0][0][:]")]
        public string MatrixX
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The Y matrix. The matrix style should be [-1][0][0] or [0][-1][0] or [0][0][:]")]
        public string MatrixY
        {
            get;
            set;
        }

        public override void Initialize()
        {
            var mx = Validate(MatrixX);
            var my = Validate(MatrixY);
            this.Initialized = mx && my;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var vecX = GetVector(MatrixX);
            var vecY = GetVector(MatrixY);
            if (vecX != null && vecY != null)
            {
                WorkspaceView.Plot<float>(vecX, vecY,  SeriesTitle, Models.UI.MySeriesChartType.Point);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
