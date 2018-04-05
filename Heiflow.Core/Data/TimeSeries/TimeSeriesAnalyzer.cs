// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data.ODM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Heiflow.Core.Data
{
    public static class TimeSeriesAnalyzer
    {
        public static IVectorTimeSeries<float> Derieve(IVectorTimeSeries<float> source, NumericalDataType datatype, TimeUnits outseriesTimeUnit)
        {
            var sourcePairs = source.ToPairs();
            IVectorTimeSeries<float> result = new FloatTimeSeries();
            if (datatype == NumericalDataType.Average || datatype == NumericalDataType.Cumulative)
            {
                if (outseriesTimeUnit == TimeUnits.Week)
                {
                    if (datatype == NumericalDataType.Average)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new
                        {
                            Year = t.DateTime.Year,
                            Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                        }).Select(group =>
                                  new TimeSeriesPair<float>(DateTimeHelper.GetBy(group.Key.Year, group.Key.Week), group.Sum(s => s.Value) / group.Count()));
                        result.From(derivedValues);
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new
                        {
                            Year = t.DateTime.Year,
                            Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                        }).Select(group =>
                            new TimeSeriesPair<float>(DateTimeHelper.GetBy(group.Key.Year, group.Key.Week), group.Sum(s => s.Value)));
                        result.From(derivedValues);
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.Month)
                {
                    if (datatype == NumericalDataType.Average)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year, t.DateTime.Month }).Select(group =>
                            new TimeSeriesPair<float>(new DateTime(group.Key.Year, group.Key.Month, 1), group.Sum(s => s.Value) / group.Count()));
                        result.From(derivedValues);
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year, t.DateTime.Month }).Select(group =>
                       new TimeSeriesPair<float>(new DateTime(group.Key.Year, group.Key.Month, 1), group.Sum(s => s.Value)));
                        result.From(derivedValues);
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.CommanYear)
                {
                    if (datatype == NumericalDataType.Average)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year }).Select(group =>
                            new TimeSeriesPair<float>(new DateTime(group.Key.Year, 1, 1), group.Sum(s => s.Value) / group.Count()));
                        result.From(derivedValues);
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year }).Select(group =>
                            new TimeSeriesPair<float>(new DateTime(group.Key.Year, 1, 1), group.Sum(s => s.Value)));
                        result.From(derivedValues);
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.Day)
                {
                    result = source;
                }
            }
            return result;
        }

        public static IVectorTimeSeries<double> Derieve(IVectorTimeSeries<double> source, NumericalDataType datatype, TimeUnits outseriesTimeUnit)
        {
            var sourcePairs = source.ToPairs();
            IVectorTimeSeries<double> result = new DoubleTimeSeries();
            if (datatype == NumericalDataType.Average || datatype == NumericalDataType.Cumulative)
            {
                if (outseriesTimeUnit == TimeUnits.Week)
                {
                    if (datatype == NumericalDataType.Average)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new
                        {
                            Year = t.DateTime.Year,
                            Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                        }).Select(group =>
                                  new TimeSeriesPair<double>(DateTimeHelper.GetBy(group.Key.Year, group.Key.Week), group.Sum(s => s.Value) / group.Count()));
                        result.From(derivedValues);
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new
                        {
                            Year = t.DateTime.Year,
                            Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                        }).Select(group =>
                            new TimeSeriesPair<double>(DateTimeHelper.GetBy(group.Key.Year, group.Key.Week), group.Sum(s => s.Value)));
                        result.From(derivedValues);
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.Month)
                {
                    if (datatype == NumericalDataType.Average)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year, t.DateTime.Month }).Select(group =>
                            new TimeSeriesPair<double>(new DateTime(group.Key.Year, group.Key.Month, 1), group.Sum(s => s.Value) / group.Count()));
                        result.From(derivedValues);
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year, t.DateTime.Month }).Select(group =>
                       new TimeSeriesPair<double>(new DateTime(group.Key.Year, group.Key.Month, 1), group.Sum(s => s.Value)));
                        result.From(derivedValues);
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.CommanYear)
                {
                    if (datatype == NumericalDataType.Average)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year }).Select(group =>
                            new TimeSeriesPair<double>(new DateTime(group.Key.Year, 1, 1), group.Sum(s => s.Value) / group.Count()));
                        result.From(derivedValues);
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year }).Select(group =>
                            new TimeSeriesPair<double>(new DateTime(group.Key.Year, 1, 1), group.Sum(s => s.Value)));
                        result.From(derivedValues);
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.Day)
                {
                    result = source;
                }
            }
            return result;
        }

        public static void Compensate(IVectorTimeSeries<double> source, IVectorTimeSeries<double> target)
        {
            if (source.DateTimes.Length == target.DateTimes.Length)
            {
                double delta = target.Value[0] - source.Value[0];
                for (int i = 0; i < source.Value.Length; i++)
                {
                    source.Value[i] += delta;
                }
            }
            else
            {
                var len = Math.Min(source.Value.Length, target.Value.Length);
                var start_index = 0;
                var inteval = Math.Abs((source.DateTimes[0] - target.DateTimes[0]).Days);

                for (int j = 0; j < target.DateTimes.Length; j++)
                {
                    var ts = Math.Abs((source.DateTimes[0] - target.DateTimes[j]).Days);
                    if (inteval > ts)
                        start_index = j;
                }
                var delta = target.Value[start_index] - source.Value[0];
                for (int i = 0; i < source.Value.Length; i++)
                {
                    source.Value[i] += delta;
                }
            }
        }

        public static float[] GetMonthlyMean(IVectorTimeSeries<float> source, NumericalDataType datatype)
        {
            var mean_mon = new float[12];
            var mon = Derieve(source, datatype, TimeUnits.Month);
            int nmon = mon.DateTimes.Length;
            int nyear = nmon / 12;

            for (int j = 0; j < 12; j++)
            {
                for (int i = 0; i < nyear; i++)
                {
                    mean_mon[j] += mon.Value[12 * i + j];
                }
            }
            for (int j = 0; j < 12; j++)
            {
                mean_mon[j] /= nyear;
            }
            return mean_mon;
        }
    }
}
