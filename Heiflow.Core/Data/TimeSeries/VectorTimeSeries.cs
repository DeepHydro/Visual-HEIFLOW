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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    //public class VectorTimeSeries<T> : IVectorTimeSeries<T>, IDataTableBinding, IVectorConvertor
    //{
    //    public VectorTimeSeries(DateTime [] dates, T[] vector)
    //    {
    //        DateTimes = dates;
    //        Value = vector;
    //    }

    //    public string Name
    //    {
    //        get;
    //        set;
    //    }
    //    public DateTime[] DateTimes
    //    {
    //        get;
    //        set;
    //    }

    //    public string[] Variables
    //    {
    //        get;
    //        set;
    //    }
    //    public T[] Value
    //    {
    //        get;
    //        set;
    //    }

    //    public int VariableIndex
    //    {
    //        get;
    //        set;
    //    }

    //    public int TimeStepIndex
    //    {
    //        get;
    //        set;
    //    }

    //    public int ValueIndex
    //    {
    //        get;
    //        set;
    //    }

    //    public bool ShowAll
    //    {
    //        get;
    //        set;
    //    }

    //    public object[] ToVector()
    //    {
    //        return Value.Cast<object>().ToArray();
    //    }

    //    public void FromVector(object[] source)
    //    {
    //        Value = new T[source.Length];
    //        for(int i=0;i<source.Length;i++)
    //        {
    //            Value[i] =(T)source[i];
    //        }
    //    }

    //    public System.Data.DataTable ToDataTable(string[] col_names)
    //    {
    //        DataTable dt = new DataTable();
    //        DataColumn dc = new DataColumn("DateTime", typeof(DateTime));
    //        dt.Columns.Add(dc);
    //        if (col_names != null)
    //        {
    //            dc = new DataColumn(col_names[0], typeof(T));
    //            dt.Columns.Add(dc);            
    //        }
    //        else
    //        {
    //            dc = new DataColumn("Data Value", typeof(T));
    //            dt.Columns.Add(dc);            
    //        }
    //        if (Value != null)
    //        {
    //            for (int r = 0; r < Value.Length; r++)
    //            {
    //                DataRow dr = dt.NewRow();
    //                dr[0] = DateTimes[r];
    //                dr[1] = Value[r];
    //                dt.Rows.Add(dr);
    //            }
    //        }
    //        return dt;
    //    }

    //    public void FromDataTable(System.Data.DataTable dt)
    //    {
    //        int nrow = dt.Rows.Count;
    //        if (Value == null)
    //        {             
    //            Value = new T[nrow];
    //        }
    //        for (int r = 0; r < nrow; r++)
    //        {
    //            DataRow dr = dt.Rows[r];
    //            Value[r] = (T)dr[1];
    //        }
    //    }

    //    public TimeSeriesPair<T>[] ToPairs()
    //    {
    //        var pairs = new TimeSeriesPair<T>[Value.Length];
    //        for (int i = 0; i < Value.Length; i++)
    //        {
    //            pairs[i] = new TimeSeriesPair<T>(DateTimes[i], Value[i]);
    //        }
    //        return pairs;
    //    }

    //    public void From(IEnumerable<TimeSeriesPair<T>> source)
    //    {
    //        DateTimes = (from ss in source select ss.DateTime).ToArray();
    //        Value = (from ss in source select ss.Value).ToArray();
    //    }
    //    public T[] GetVector()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
