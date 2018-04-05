// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Models.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Tools
{
    public interface IWorkspaceView
    {
        void OutputTo(DataTable dt);
        void OutputTo(string msg);
        void Plot<T>(T[] xx, T[] yy,  string seriesName, MySeriesChartType chartType);
        void Plot<T>(T[] yy, string seriesName, MySeriesChartType chartType);
        void Plot<T>(ITimeSeries<T> source, MySeriesChartType chartType);
        void RefreshLayerBy(object gridLayer, string fieldName);
    }
}
