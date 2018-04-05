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
using System.IO;
using Heiflow.Core;

namespace Heiflow.Core.Data
{
    public class NumericalSeriesPair : INumericalSeriesPair
    {
        public NumericalSeriesPair( DateTime d,double v)
        {
            Value = v;
            DateTime = d;
        }

        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }

    public class TimeSeriesConsistencyInfo
    {
        public TimeSeriesConsistencyInfo (TimeUnits tu)
        {
            tUnit = tu;
            MissingDateTimes = new List<DateTime>();
            IsNormal = true;
        }
        private TimeUnits tUnit;
        public DateTime DateTimeStamp { get; set; }
        public int DesiredCounts { get; set; }
        public int AcutalCounts { get; set; }
        public List<DateTime> MissingDateTimes { get; set; }
        public bool IsNormal { get; set; }
    }

    public class NumericalTimeSeries : IVectorTimeSeries<double>
    {
        protected DateTime[] mDateTimeVector;
        protected double[] mDataValueVector;
        protected NumericalSeriesPair[] mNumericalSeriesPair;
        protected long count;

        public NumericalTimeSeries(DataTable table)
        {
            if (table != null)
            {
                if (table.Columns.Contains("DateTimeUTC") && table.Columns.Contains("DataValue"))
                {
                    var timev = from dt in table.AsEnumerable() select dt.Field<DateTime>("DateTimeUTC");
                    var vv = from dt in table.AsEnumerable() select dt.Field<double>("DataValue");
                    count = table.Rows.Count;
                   // CreatePairs(timev.ToArray(),vv.ToArray());

                    mDataValueVector = vv.ToArray();
                    mDateTimeVector = timev.ToArray();
                }
            }
            Name = "Time Series";
        }

        public NumericalTimeSeries(double [] values, DateTime [] time)
        {
            if (values == null || time == null || values.Length != time.Length)
            {
                throw new Exception("Invalid value vector or datetime vector.");
            }
            count = values != null ? values.Length : 0;
           // CreatePairs(time,values);
            mDataValueVector = values;
            mDateTimeVector = time;
            Name = "Time Series";
        }

        private void CreatePairs(DateTime[] dateTimeVector, double[] dataValueVector)
        {
            mNumericalSeriesPair = new NumericalSeriesPair[dataValueVector.Length];
            for (int i = 0; i < dataValueVector.Length; i++)
            {
                mNumericalSeriesPair[i] = new NumericalSeriesPair(dateTimeVector[i], dataValueVector[i]);
            }
        }

        public void SaveAs(string filename)
        {
            StreamWriter sw = new StreamWriter(filename);
            for (int i = 0; i < mDataValueVector.Length; i++)
            {
                string line = string.Format("{0}\t{1}", mDateTimeVector[i], mDataValueVector[i]);
                sw.WriteLine(line);
            }
            sw.Close();
        }

        #region ITimeSeries 成员

        public string[] Variables
        {
            get;
            set;
        }
        public DateTime[] DateTimes
        {
            get 
            {
                return mDateTimeVector;
            }
            set
            {
                mDateTimeVector = value;
            }
        }

        public double[] Value
        {
            get 
            {
                return mDataValueVector;
            }
            set
            {
                mDataValueVector = value;
            }
        }

        public INumericalSeriesPair[] SeriesPairs
        {
            get
            {
                if (mNumericalSeriesPair == null)
                    CreatePairs(mDateTimeVector, mDataValueVector);
                return mNumericalSeriesPair;
            }
        }

        public long Count
        {
            get
            {
                return count;
            }
        }

        public string Name
        { get; set; }
        #endregion

        public int[] GroupByYear()
        {
            var dtv = DateTimes;
            if (dtv != null)
            {
                var years = dtv.GroupBy(t => t.Year).Select(g => g.Key);
                int[] sortedYears = years.ToArray();
                System.Array.Sort(sortedYears);
                return sortedYears;
            }
            else
            {
                return null;
            }
        }

        public NumericalSeriesPair[] Select(int year)
        {
            var pairs = from p in mNumericalSeriesPair where p.DateTime.Year == year select p;
            return pairs.ToArray();
        }

        public NumericalSeriesPair[] Select(int year,int month)
        {
            var pairs = from p in mNumericalSeriesPair where p.DateTime.Year == year && p.DateTime.Month==month select p ;
            return pairs.ToArray();
        }

        public NumericalSeriesPair[] Select(DateTime start, DateTime end)
        {
            var pairs = from p in mNumericalSeriesPair where p.DateTime >= start && p.DateTime <= end select p;
            return pairs.ToArray();
        }

        public TimeSeriesConsistencyInfo[]  GetTimeSeriesConsistencyInfo()
        {
            var dtv = DateTimes;
            var derivedValues = dtv.GroupBy(t => new { t.Year }).Select(group =>
                    new { Year=group.Key.Year, Count = group.Count()});
            TimeSeriesConsistencyInfo[] infos = null;
            if (derivedValues.Count() > 0)
            {
                infos = new TimeSeriesConsistencyInfo[derivedValues.Count()];
                int i = 0;
                foreach (var dv in derivedValues)
                {
                    infos[i] = new TimeSeriesConsistencyInfo(TimeUnits.CommanYear);
                    infos[i].DateTimeStamp = new DateTime(dv.Year, 1, 1);
                    infos[i].AcutalCounts = dv.Count;
                    infos[i].DesiredCounts = DateTime.IsLeapYear(dv.Year) ? 366 : 365;
                    if (infos[i].AcutalCounts != infos[i].DesiredCounts)
                    {
                        infos[i].IsNormal = false;
                        DateTime [] desiredTimes=new DateTime[infos[i].DesiredCounts];
                        DateTime start=new DateTime(dv.Year, 1, 1);
                        for (int j= 0; j < infos[i].DesiredCounts; j++)
                        {
                            desiredTimes[j] = start.AddDays(j);
                        }
                        var times = (from t in dtv where t >= start && t <= new DateTime(dv.Year, 12, 31) select t).ToArray();
                        infos[i].MissingDateTimes.AddRange(desiredTimes.Except(times).ToArray());
                    }
                    i++;
                }         
            }
            return infos;
        }

 

        public TimeSeriesPair<double>[] ToPairs()
        {
            throw new NotImplementedException();
        }

        public void From(IEnumerable<TimeSeriesPair<double>> source)
        {
            throw new NotImplementedException();
        }

        public double[] GetSeriesAt(int var_index, int spatial_index)
        {
            throw new NotImplementedException();
        }


        public DataTable ToDataTable(string var_name)
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("Date", typeof(DateTime));
            dt.Columns.Add(dc);
            DataColumn dc_value = new DataColumn(var_name, typeof(double));
            dt.Columns.Add(dc_value);
            if (Value != null)
            {
                for (int i = 0; i < Value.Length; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = DateTimes[i];
                    dr[1] = Value[i];
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
    }

}
