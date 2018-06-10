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

using ILNumerics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    //public class My2DMat<T> : MyArray<T>
    //{
    //    public My2DMat(int size0,int size1)
    //    {
    //        Size = new int[] { size0, size1 };
    //        Initialize();
    //    }

    //    public override T[] this[int i0, int i1= -2]
    //    {
    //        get
    //        {
    //            return Value[i0];
    //        }
    //        set
    //        {
    //            Value[i0] = value;
    //        }
    //    }
    //    /// <summary>
    //    /// get or set single value
    //    /// </summary>
    //    /// <param name="i0"></param>
    //    /// <param name="i1"></param>
    //    /// <param name="v"></param>
    //    /// <returns></returns>
    //    public override T this[int i0, int i1, int i2= -2]
    //    {
    //        get
    //        {
    //            return Value[i0][i1];
    //        }
    //        set
    //        {
    //            Value[i0][i1] = value;
    //        }
    //    }

    //    public T[][] Value
    //    {
    //        get;
    //        set;
    //    }

    //    protected override void Initialize()
    //    {
    //        if (Size.Length == 2)
    //        {
    //            Value = new T[Size[0]][];
    //            for (int r = 0; r < Size[0]; r++)
    //            {
    //                Value[r] = new T[Size[1]];
    //            }
    //        }
    //    }

    //    public override T[] GetVector(int i0, int i1, int i2=-1)
    //    {
    //        T[] vector = null;
    //        if (i0 == MyMath.full)
    //        {
    //            vector = new T[Size[0]];
    //            for (int r = 0; r < Size[0]; r++)
    //            {
    //                vector[r]=Value[r][i1];
    //            }
    //        }
    //        else if (i1 == MyMath.full)
    //        {
    //            vector = Value[i0];
    //        }
    //        return vector;
    //    }

    //    public override void SetBy(T[] vector, int i0, int i1, int i2 = -1)
    //    {
    //        if (i0 == MyMath.full)
    //        {
    //            for (int r = 0; r < Size[0]; r++)
    //            {
    //                Value[r][i1] = vector[r];
    //            }
    //        }
    //        else if (i1 == MyMath.full)
    //        {
    //            Value[i0] = vector;
    //        }
    //    }

    //    public override ILArray<T> ToILArray()
    //    {
    //        ILArray<T> array = ILMath.zeros<T>(Size[0], Size[1]);
    //        for (int r = 0; r < Size[0]; r++)
    //        {
    //            array[r, ILMath.full] = GetVector(r, MyMath.full);
    //        }
    //        return array;
    //    }

    //    public override void Constant(T cnst)
    //    {
    //        for (int r = 0; r < Size[0]; r++)
    //            for (int c = 0; c < Size[1]; c++)
    //                Value[r][c] = cnst;
    //    }

    //    public override double Sum()
    //    {
    //        double sum = 0.0;
    //        for (int r = 0; r < Size[0]; r++)
    //            for (int c = 0; c < Size[1]; c++)
    //                sum += double.Parse(Value[r][c].ToString());
    //        return sum;
    //    }

    //    public  System.Data.DataTable ToDataTable()
    //    {
    //        DataTable dt = new DataTable();

    //        foreach (var nm in Variables)
    //        {
    //            DataColumn dc = new DataColumn(nm, typeof(T));
    //            dt.Columns.Add(dc);
    //        }

    //        if (Value != null)
    //        {
    //            for (int r = 0; r < Size[1]; r++)
    //            {
    //                DataRow dr = dt.NewRow();
    //                for (int c = 0; c < Size[0]; c++)
    //                {
    //                    dr[c] = Value[c][r];
    //                }
    //                dt.Rows.Add(dr);
    //            }
    //        }
    //        return dt;
        
    //    }

    //    public DataTable AsDataTable(int col, string col_name)
    //    {
    //        DataTable dt = new DataTable();
    //        if (!TypeConverterEx.IsNull(col_name))
    //        {
    //            DataColumn dc = new DataColumn(col_name, typeof(T));
    //            dt.Columns.Add(dc);
    //        }
    //        else
    //        {
    //            DataColumn dc = new DataColumn("A" + (col + 1), typeof(T));
    //            dt.Columns.Add(dc);
    //        }
    //        if (Value != null)
    //        {
    //            for (int r = 0; r < Size[1]; r++)
    //            {
    //                DataRow dr = dt.NewRow();
    //                dr[0] = Value[col][r];
    //                dt.Rows.Add(dr);
    //            }
    //        }
    //        return dt;
    //    }

    //    public  void FromDataTable(System.Data.DataTable dt)
    //    {
    //        if(Value == null)
    //        {
    //            int nrow = dt.Rows.Count;
    //            int ncol = dt.Columns.Count;
    //            base.Size = new int[] { ncol, nrow };
    //            Initialize();
    //        }
    //        for (int r = 0; r < Size[1]; r++)
    //        {
    //            DataRow dr = dt.Rows[r];
    //            for (int c = 0; c < Size[0]; c++)
    //            {                 
    //                Value[c][r] =(T) dr.ItemArray[c];
    //            }
    //        }
    //    }
    //}
}
