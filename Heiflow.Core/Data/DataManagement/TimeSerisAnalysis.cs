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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Heiflow.Core;

namespace Heiflow.Core.Data
{
    public class DerivedTimeSeries : NumericalTimeSeries
    {
        public DerivedTimeSeries(double[] values, DateTime[] time, TimeUnits inputSeriesTimeUnit)
            : base
                (values, time)
        {
            mInputSeriesTimeUnit = inputSeriesTimeUnit;
        }

        private TimeUnits mInputSeriesTimeUnit;

        public NumericalSeriesPair[] Derive(NumericalDataType datatype, TimeUnits outseriesTimeUnit)
        {
            INumericalSeriesPair[] sourcePairs = SeriesPairs;
            NumericalSeriesPair[] derivedParis = null;
            DateTime[] mDateTimeVector = DateTimes;
            double[] mDataValueVector = Value;

            if (datatype == NumericalDataType.Average || datatype == NumericalDataType.Cumulative)
            {
                if (outseriesTimeUnit == TimeUnits.Week)
                {
                    int startIndex = 0;
                    DateTime start = mDateTimeVector[0];
                    foreach (DateTime t in mDateTimeVector)
                    {
                        if (t.DayOfWeek == DayOfWeek.Monday)
                        {
                            start = t;
                            break;
                        }
                        startIndex++;
                    }
                    int weekCount = (mDateTimeVector.Length - startIndex + 1) / 7;
                    if (weekCount > 0)
                    {
                        derivedParis = new NumericalSeriesPair[weekCount];
                        for (int w = 0; w < weekCount; w++)
                        {
                            double weekSum = 0;
                            double max = startIndex + (w + 1) * 7 > mDateTimeVector.Length ? mDateTimeVector.Length : startIndex + (w + 1) * 7;
                            for (int j = startIndex + w * 7; j < max; j++)
                            {
                                weekSum += mDataValueVector[j];
                            }
                            if( datatype== NumericalDataType.Average)
                                derivedParis[w] = new NumericalSeriesPair(start.AddDays(w * 7), Math.Round(weekSum / 7, 4));
                            else if (datatype == NumericalDataType.Cumulative)
                                derivedParis[w] = new NumericalSeriesPair(start.AddDays(w * 7), weekSum);
                        }
                    }
                }
//                    Customer Number: G864910
//License number : 8117376
//Password : 814146992251470

                else if (outseriesTimeUnit == TimeUnits.Month)
                {
                    if (datatype == NumericalDataType.Average)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year, t.DateTime.Month }).Select(group =>
                            new NumericalSeriesPair(new DateTime(group.Key.Year, group.Key.Month, 1), Math.Round(group.Sum(s => s.Value) / group.Count(), 4)));
                        derivedParis = derivedValues.ToArray();
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year, t.DateTime.Month }).Select(group =>
                       new NumericalSeriesPair(new DateTime(group.Key.Year, group.Key.Month, 1), group.Sum(s => s.Value)));
                        derivedParis = derivedValues.ToArray();
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.CommanYear)
                {
                    if (datatype == NumericalDataType.Average)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year }).Select(group =>
                            new NumericalSeriesPair(new DateTime(group.Key.Year, 1, 1), Math.Round(group.Sum(s => s.Value) / group.Count(), 4)));
                        derivedParis = derivedValues.ToArray();
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        var derivedValues = sourcePairs.GroupBy(t => new { t.DateTime.Year }).Select(group =>
                            new NumericalSeriesPair(new DateTime(group.Key.Year, 1, 1), group.Sum(s => s.Value)));
                        derivedParis = derivedValues.ToArray();
                    }
                }
                else if (outseriesTimeUnit == TimeUnits.FiveDays)
                {
                    derivedParis = Derive(datatype, 5);
                }
                else if (outseriesTimeUnit == TimeUnits.Day)
                {
                    derivedParis =  (NumericalSeriesPair[])sourcePairs;
                }
            }

            return derivedParis;
        }

        public NumericalTimeSeries DeriveTS(NumericalDataType datatype, TimeUnits outseriesTimeUnit)
        {
            var derivedParis = Derive(datatype, outseriesTimeUnit);
            var values = from dp in derivedParis select dp.Value;
            var times = from dp in derivedParis select dp.DateTime;
            var ts = new NumericalTimeSeries(values.ToArray(), times.ToArray());
            return ts;
        }

        public NumericalSeriesPair[] Derive(NumericalDataType datatype, int timeInteval)
        {
            if (timeInteval == 0)
                timeInteval = 1;
            DateTime[] mDateTimeVector = DateTimes;
            double[] mDataValueVector = Value;

            List<NumericalSeriesPair> list = new List<NumericalSeriesPair>();
            int startYear = mDateTimeVector.Min().Year;
            int year = mDateTimeVector.Max().Year - mDateTimeVector.Min().Year + 1;

            int index=0;
            DateTime start = mDateTimeVector.Min();
            for (int i = 0; i < year; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    int daysInMonth = DateTime.DaysInMonth(startYear+i, j + 1);
                    double s = (double)daysInMonth / (double)timeInteval;
                    //int seg =(int)Math.Ceiling( s);
                    int seg = 6;
                    for (int c = 0; c < seg-1; c++)
                    {
                        double sum = 0;
                        DateTime now = start.AddDays(index);
                        for (int t = 0; t < timeInteval ; t++)
                        {
                            sum += mDataValueVector[index];
                            index++;                          
                        }                 
                        double ave = Math.Round( sum / timeInteval,3);
                        NumericalSeriesPair np = new NumericalSeriesPair(now, ave);
                        list.Add(np);
                    }
                    double sum1 = 0;
                    DateTime now1 = start.AddDays(index);
                    for (int c = (seg -1)* timeInteval; c < daysInMonth; c++)
                    {
                        sum1 += mDataValueVector[c];
                        index++;
                    }
                    if (datatype == NumericalDataType.Average)
                    {
                        double ave1 = Math.Round(sum1 / (daysInMonth - (seg - 1) * timeInteval), 3);
                        list.Add(new NumericalSeriesPair(now1, ave1));
                    }
                    else if (datatype == NumericalDataType.Cumulative)
                    {
                        list.Add(new NumericalSeriesPair(now1, sum1));
                    }
                }
            }    
            return list.ToArray();
        }
    }
}
