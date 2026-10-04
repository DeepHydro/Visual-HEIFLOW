// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data.ODM;
using System;
using System.Data;

namespace Heiflow.Models.Hydrodynamics.Susbed.SWMM
{
    /// <summary>
    /// DoubleTimeSeries only carries DateTimes and Values. The conversion used by the Susbed
    /// views lives here so that the time series can still be shown in a grid.
    /// </summary>
    public static class TimeSeriesExtension
    {
        public static DataTable ToDataTable(this DoubleTimeSeries series, string valueColumnName)
        {
            var dt = new DataTable();
            dt.Columns.Add("时间", typeof(DateTime));
            dt.Columns.Add(valueColumnName, typeof(double));

            if (series == null || series.DateTimes == null || series.Values == null)
                return dt;

            var length = Math.Min(series.DateTimes.Length, series.Values.Length);
            for (int i = 0; i < length; i++)
                dt.Rows.Add(series.DateTimes[i], series.Values[i]);

            return dt;
        }
    }
}
