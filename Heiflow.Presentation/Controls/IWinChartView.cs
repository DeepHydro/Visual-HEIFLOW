// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Models.UI;
using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
namespace Heiflow.Presentation.Controls
{
    public interface IWinChartView:IChildView
    {
        bool IsDisposed { get; }
        void Plot<T>(DateTime[] xx, T[] yy, string series_name, SeriesChartType chartType);
        void Plot<T>(T[] yy, string series_name, SeriesChartType chartType);
        void Plot<T>(ITimeSeries<T> source, SeriesChartType chartType);
        void CloseView();
    }
}
