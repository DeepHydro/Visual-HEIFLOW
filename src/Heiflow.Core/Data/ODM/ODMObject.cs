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

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data.ODM
{
    [ServiceContract]
    public class Site:IObservationsSite
    {
        public Site()
        {
            Name = "site";
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public Variable[] Variables { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        public int SpatialIndex { get; set; }
        public string Code { get; set; }

        public double Elevation { get; set; }

        public double Cell_Elevation { get; set; }

        public string Comments { get; set; }

        public string State { get; set; }

        public int MonitorType { get; set; }
        public string Country { get; set; }
        public double LocalX { get; set; }
        public double LocalY { get; set; }
        public double Distance { get; set; }

        public string SiteType { get; set; }

        public DataCube<double> TimeSeries { get; set; }

        public List<DataCube<double>> TimeSeriesCollection { get; set; }

    }

    [ServiceContract]
    public class Variable
    {
        public Variable()
        {

        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int ID { get; set; }

        public string Code { get; set; }

        public string GeneralCategory { get; set; }

    }
    //[ServiceContract]
    //public class DoubleTimeSeries : Heiflow.Core.Data.IVectorTimeSeries<double>
    //{
    //    public DoubleTimeSeries()
    //    {
    //        Name = "Time Series";
    //    }
    //    public DoubleTimeSeries(double [] vv, DateTime [] dates)
    //    {
    //        Value = vv;
    //        DateTimes = dates;
    //        Name = "Time Series";
    //    }

    //    [DataMember]
    //    public double[] Value { get; set; }
    //    [DataMember]
    //    public DateTime[] DateTimes { get; set; }
    //    public TimeSeriesPair<double>[] ToPairs()
    //    {
    //        var pairs = new TimeSeriesPair<double>[Value.Length];
    //        for (int i = 0; i < Value.Length; i++)
    //        {
    //            pairs[i] = new TimeSeriesPair<double>(DateTimes[i], Value[i]);
    //        }
    //        return pairs;
    //    }

    //    public void From(IEnumerable<TimeSeriesPair<double>> source)
    //    {
    //        DateTimes = (from ss in source select ss.DateTime).ToArray();
    //        Value = (from ss in source select ss.Value).ToArray();
    //    }

    //    public string[] Variables
    //    {
    //        get;
    //        set;
    //    }
    //    public string Name
    //    {
    //        get;
    //        set;
    //    }

    //    public double[] GetSeriesAt(int var_index, int spatial_index)
    //    {
    //        throw new NotImplementedException();
    //    }


    //    public DataTable ToDataTable(string var_name)
    //    {
    //        DataTable dt = new DataTable();
    //        DataColumn dc = new DataColumn("DateTime", typeof(DateTime));
    //        dt.Columns.Add(dc);
    //        dc = new DataColumn(var_name, typeof(double));
    //        dt.Columns.Add(dc);
    //        for (int i = 0; i < DateTimes.Length; i++)
    //        {
    //            var dr = dt.NewRow();
    //            dr[0] = DateTimes[i];
    //            dr[1] = Value[i];
    //            dt.Rows.Add(dr);
    //        }
    //        return dt;
    //    }
    //}
    //[ServiceContract]
    //public class FloatTimeSeries : Heiflow.Core.Data.IVectorTimeSeries<float>
    //{
    //    public FloatTimeSeries()
    //    {
    //        Name = "Time Series";
    //    }
    //    public FloatTimeSeries(float[] vv, DateTime[] dates)
    //    {
    //        Value = vv;
    //        DateTimes = dates;
    //        Name = "Time Series";
    //    }

    //    public string Name
    //    {
    //        get;
    //        set;
    //    }

    //    public string[] Variables
    //    {
    //        get;
    //        set;
    //    }

    //    [DataMember]
    //    public float[] Value { get; set; }
    //    [DataMember]
    //    public DateTime[] DateTimes { get; set; }
    //    public TimeSeriesPair<float>[] ToPairs()
    //    {
    //        var pairs = new TimeSeriesPair<float>[Value.Length];
    //        for (int i = 0; i < Value.Length; i++)
    //        {
    //            pairs[i] = new TimeSeriesPair<float>(DateTimes[i], Value[i]);
    //        }
    //        return pairs;
    //    }

    //    public void From(IEnumerable<TimeSeriesPair<float>> source)
    //    {
    //        DateTimes = (from ss in source select ss.DateTime).ToArray();
    //        Value = (from ss in source select ss.Value).ToArray();
    //    }

    //    public float[] GetSeriesAt(int var_index, int spatial_index)
    //    {
    //        throw new NotImplementedException();
    //    }


    //    public DataTable ToDataTable(string var_name)
    //    {
    //        DataTable dt = new DataTable();
    //        DataColumn dc = new DataColumn("Date", typeof(DateTime));
    //        dt.Columns.Add(dc);
    //        DataColumn dc_value = new DataColumn(var_name, typeof(double));
    //        dt.Columns.Add(dc_value);
    //        if (Value != null)
    //        {
    //            for (int i = 0; i < Value.Length; i++)
    //            {
    //                DataRow dr = dt.NewRow();
    //                dr[0] = DateTimes[i];
    //                dr[1] = Value[i];
    //                dt.Rows.Add(dr);
    //            }
    //        }
    //        return dt;
    //    }
    //}

    [ServiceContract]
    public class QueryCriteria
    {
        public QueryCriteria()
        {
            AllTime = false;
        }
        [DataMember]
        public DateTime Start { get; set; }
        [DataMember]
        public DateTime End { get; set; }
        [DataMember]
        public int SiteID { get; set; }
        [DataMember]
        public int VariableID { get; set; }
        [DataMember]
        public string VariableName { get; set; }
        [DataMember]
        public BBox BBox { get; set; }

        public bool AllTime { get; set; }
    }
    [ServiceContract]
    public class BBox
    {
        [DataMember]
        public double West { get; set; }
        [DataMember]
        public double East { get; set; }
        [DataMember]
        public double South { get; set; }
        [DataMember]
        public double North { get; set; }
    }
}
