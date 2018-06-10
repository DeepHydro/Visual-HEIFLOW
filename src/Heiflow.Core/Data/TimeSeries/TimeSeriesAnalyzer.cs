//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

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
        public static DataCube<float> Derieve(DataCube<float> source, NumericalDataType datatype, TimeUnits outseriesTimeUnit)
        {
            var sourcePairs = source.ToPairs();
            DataCube<float> result = null;
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
                        result = DataCube<float>.FromPairs(derivedValues);
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new
                        {
                            Year = t.DateTime.Year,
                            Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                        }).Select(group =>
                            new TimeSeriesPair<float>(DateTimeHelper.GetBy(group.Key.Year, group.Key.Week), group.Sum(s => s.Value)));
                        result = DataCube<float>.FromPairs(derivedValues);
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.Month)
                {
                    if (datatype == NumericalDataType.Average)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year, t.DateTime.Month }).Select(group =>
                            new TimeSeriesPair<float>(new DateTime(group.Key.Year, group.Key.Month, 1), group.Sum(s => s.Value) / group.Count()));
                        result = DataCube<float>.FromPairs(derivedValues);
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year, t.DateTime.Month }).Select(group =>
                       new TimeSeriesPair<float>(new DateTime(group.Key.Year, group.Key.Month, 1), group.Sum(s => s.Value)));
                        result = DataCube<float>.FromPairs(derivedValues);
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.CommanYear)
                {
                    if (datatype == NumericalDataType.Average)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year }).Select(group =>
                            new TimeSeriesPair<float>(new DateTime(group.Key.Year, 1, 1), group.Sum(s => s.Value) / group.Count()));
                        result = DataCube<float>.FromPairs(derivedValues);
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year }).Select(group =>
                            new TimeSeriesPair<float>(new DateTime(group.Key.Year, 1, 1), group.Sum(s => s.Value)));
                        result = DataCube<float>.FromPairs(derivedValues);
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.Day)
                {
                    result = source;
                }
            }
            return result;
        }

        public static DataCube<double> Derieve(DataCube<double> source, NumericalDataType datatype, TimeUnits outseriesTimeUnit)
        {
            var sourcePairs = source.ToPairs();
            DataCube<double> result = null;
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
                        result = DataCube<double>.FromPairs(derivedValues);
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new
                        {
                            Year = t.DateTime.Year,
                            Week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(t.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                        }).Select(group =>
                            new TimeSeriesPair<double>(DateTimeHelper.GetBy(group.Key.Year, group.Key.Week), group.Sum(s => s.Value)));
                        result = DataCube<double>.FromPairs(derivedValues);
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.Month)
                {
                    if (datatype == NumericalDataType.Average)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year, t.DateTime.Month }).Select(group =>
                            new TimeSeriesPair<double>(new DateTime(group.Key.Year, group.Key.Month, 1), group.Sum(s => s.Value) / group.Count()));
                        result = DataCube<double>.FromPairs(derivedValues);
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year, t.DateTime.Month }).Select(group =>
                       new TimeSeriesPair<double>(new DateTime(group.Key.Year, group.Key.Month, 1), group.Sum(s => s.Value)));
                        result = DataCube<double>.FromPairs(derivedValues);
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.CommanYear)
                {
                    if (datatype == NumericalDataType.Average)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year }).Select(group =>
                            new TimeSeriesPair<double>(new DateTime(group.Key.Year, 1, 1), group.Sum(s => s.Value) / group.Count()));
                        result = DataCube<double>.FromPairs(derivedValues);
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year }).Select(group =>
                            new TimeSeriesPair<double>(new DateTime(group.Key.Year, 1, 1), group.Sum(s => s.Value)));
                        result = DataCube<double>.FromPairs(derivedValues);
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.Day)
                {
                    result = source;
                }
            }
            return result;
        }

        public static void Compensate(DataCube<double> source, DataCube<double> target)
        {
            if (source.DateTimes.Length == target.DateTimes.Length)
            {
                double delta = target[0, 0, 0] - source[0, 0, 0];
                for (int i = 0; i < source.Size[1]; i++)
                {
                    source[0, i, 0] += delta;
                }
            }
            else
            {
                var len = Math.Min(source.Size[1], target.Size[1]);
                var start_index = 0;
                var inteval = Math.Abs((source.DateTimes[0] - target.DateTimes[0]).Days);

                for (int j = 0; j < target.DateTimes.Length; j++)
                {
                    var ts = Math.Abs((source.DateTimes[0] - target.DateTimes[j]).Days);
                    if (inteval > ts)
                        start_index = j;
                }
                var delta = target[0,start_index,0] - source[0,0,0];
                for (int i = 0; i < source.Size[1]; i++)
                {
                    source[0,i,0] += delta;
                }
            }
        }

        public static float[] GetMonthlyMean(DataCube<float> source, NumericalDataType datatype)
        {
            var mean_mon = new float[12];
            var mon = Derieve(source, datatype, TimeUnits.Month);
            int nmon = mon.DateTimes.Length;
            int nyear = nmon / 12;

            for (int j = 0; j < 12; j++)
            {
                for (int i = 0; i < nyear; i++)
                {
                    mean_mon[j] += mon[0, 12 * i + j, 0];
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
