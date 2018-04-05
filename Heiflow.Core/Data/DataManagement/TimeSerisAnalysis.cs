// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
