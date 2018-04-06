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
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    public enum TimeVarientFlag { Repeat = -1, Constant = 0, Individual = 2 };
    /// <summary>
    /// Represents 3d matrix with varient structure. It is mainly used to represent time-varient parameters rather than to represent input/output
    /// data. The zero dimension stores parameter in each time period, the second dimension is regarded as 1 .
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MyVarient3DMat<T> : My3DMat<T>
    {
        public MyVarient3DMat(int size0, int size1)
        {
            Size = new int[] { size0, size1, -1 };
            Value = new T[size0][][];
            for (int i = 0; i < size0; i++)
            {
                Value[i] = new T[size1][];
            }
            Variables = new string[size0];

            InitFlags(size0, size1);
            PopulateVariables();
            DataCubeType = Data.DataCubeType.Varient;
            TimeBrowsable = false;
            AllowTableEdit = false;
        }
        /// <summary>
        /// creat 3d mat with individual layout style
        /// </summary>
        /// <param name="size0"></param>
        /// <param name="size1"></param>
        /// <param name="size2"></param>
        public MyVarient3DMat(int size0, int size1, int size2)
            : base(size0,size1,size2)
        {
            DataCubeType = Data.DataCubeType.Varient;
        }
        public override T this[int i0, int i1, int i2]
        {
            get
            {
                if (Flags[i0,i1] == TimeVarientFlag.Constant)
                    return (T)(Constants.GetValue(i0, i1));
                else if (Flags[i0,i1] == TimeVarientFlag.Individual)
                    return Value[i0][i1][i2];
                else
                {
                    var time = RepeatAt(i0, i1);
                    return Value[i0][time][i2];
                }
            }
            set
            {
                if (Flags[i0,i1] == TimeVarientFlag.Individual)
                    Value[i0][i1][i2] = value;
            }
        }
        public override System.Data.DataTable ToDataTableByTime(int var_index, int time_index)
        {
            DataTable dt = new DataTable();
            int nvar = Size[0];

            //if (var_index < 0)
            //{
            //    bool unifrom_flag = true;
            //    for (int i = 0; i < nvar; i++)
            //    {
            //        if (this.Flags[i,0] == TimeVarientFlag.Individual)
            //            unifrom_flag = false;
            //    }
            //    if (!unifrom_flag)
            //    {
            //        foreach (var str in Variables)
            //        {
            //            DataColumn dc = new DataColumn(str, typeof(T));
            //            dt.Columns.Add(dc);
            //        }
            //        for (int i = 0; i < Value[0][time_index].Length; i++)
            //        {
            //            var dr = dt.NewRow();
            //            for (int j = 0; j < Size[0]; j++)
            //            {
            //                dr[j] = this.Value[j][time_index][i];
            //            }
            //            dt.Rows.Add(dr);
            //        }
            //    }
            //}
            //else
            //{
                DataColumn dc = new DataColumn(Variables[var_index], typeof(T));
                dt.Columns.Add(dc);
                if (Flags[var_index,time_index] == TimeVarientFlag.Individual)
                {
                    if (Value[var_index] != null && Value[var_index][time_index] != null)
                    {
                    for (int r = 0; r < Value[var_index][time_index].Length; r++)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = Value[var_index][time_index][r];
                        dt.Rows.Add(dr);
                    }
                    }
                }
                else if (Flags[var_index,time_index] == TimeVarientFlag.Constant)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = Constants.GetValue(var_index,time_index);
                    dt.Rows.Add(dr);
                }
                else if (Flags[var_index,time_index] == TimeVarientFlag.Repeat)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = -1;
                    dt.Rows.Add(dr);
                }
            //}

            return dt;
        }
        public override System.Data.DataTable ToDataTableBySpace(int var_index, int space_index)
        {
            DataTable dt = new DataTable();
            int nvar = Size[0];
            int nstep = Size[1];
            DataColumn dc_time = new DataColumn("DateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add(dc_time);
            if (var_index < 0)
            {
                bool unifrom_flag = true;
                for (int i = 0; i < nvar; i++)
                {
                    if (this.Flags[i,0] == TimeVarientFlag.Individual)
                        unifrom_flag = false;
                }
                if (unifrom_flag)
                {
                    for (int i = 0; i < nvar; i++)
                    {
                        DataColumn dc = new DataColumn(Variables[i], typeof(T));
                        dt.Columns.Add(dc);
                    }

                    for (int t = 0; t < nstep; t++)
                    {
                        var dr = dt.NewRow();
                        dr[0] = DateTime.Now;
                        for (int i = 0; i < nvar; i++)
                        {
                            dr[i + 1] = Value[i][t][space_index];
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            else
            {
                DataColumn dc = new DataColumn(Variables[var_index], typeof(T));
                dt.Columns.Add(dc);

                if (Flags[var_index,0] == TimeVarientFlag.Individual)
                {
                    for (int t = 0; t < nstep; t++)
                    {
                        var dr = dt.NewRow();
                        dr[0] = DateTime.Now;
                        dr[1] = Value[var_index][t][space_index];
                        dt.Rows.Add(dr);
                    }

                    if (DateTimes != null && DateTimes.Length == dt.Rows.Count)
                    {
                        for (int t = 0; t < nstep; t++)
                        {
                            dt.Rows[t][0] = DateTimes[t];
                        }
                    }
                }
                else if (Flags[var_index,0] == TimeVarientFlag.Constant)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = DateTime.Now;
                    dr[1] = Constants.GetValue(var_index,0);
                    dt.Rows.Add(dr);
                }
                else if (Flags[var_index,0] == TimeVarientFlag.Repeat)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = DateTime.Now;
                    dr[1] = -1;
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        public override DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            int nvar = Size[0];
            int nrow = GetSpaceDimLength(0, 0);
            foreach (var str in Variables)
            {
                DataColumn dc = new DataColumn(str, typeof(T));
                dt.Columns.Add(dc);
            }
            for (int i = 0; i < nrow;i++ )
            {
                var dr = dt.NewRow();
                for (int j = 0; j < Variables.Length;j++)
                {
                    if(Flags[j,SelectedTimeIndex] == TimeVarientFlag.Individual)
                    {
                        dr[j] = Value[j][SelectedTimeIndex][i];
                    }
                   else if (Flags[j, SelectedTimeIndex] == TimeVarientFlag.Constant)
                    {
                        dr[j] = Constants[j, SelectedTimeIndex];
                    }
                    else if (Flags[j, SelectedTimeIndex] == TimeVarientFlag.Repeat)
                    {
                        var vv = RepeatAt(j);
                        dr[j] = Value[vv][SelectedTimeIndex][i];
                    }
                }

                    dt.Rows.Add(dr);
            }
                return dt;
        }
        public override Array GetSerialArrayByTime(int var_index, int time_index)
        {
            T[,] array = null;
            if (Flags[var_index,time_index] == TimeVarientFlag.Individual)
            {
                var vec = Value[var_index][time_index];
                array = new T[vec.Length, 1];
                for (int i = 0; i < vec.Length; i++)
                {
                    array[i, 0] = vec[i];
                }
            }
            else if (Flags[var_index,time_index] == TimeVarientFlag.Constant)
            {
                array = new T[1, 1];
                array[0, 0] = (T)Constants.GetValue(var_index,time_index);
            }
            else if (Flags[var_index,time_index] == TimeVarientFlag.Repeat)
            {
                array = new T[1, 1];
                array[0, 0] = TypeConverterEx.ChangeType<T>(-1);
            }

            return array;
        }
        public override Array GetRegularlArrayByTime(int var_index, int time_index)
        {
            T[,] array = null;
            if (Flags[var_index,time_index] == TimeVarientFlag.Individual)
            {
                if (Topology != null)
                {
                    array = new T[Topology.RowCount, Topology.ColumnCount];
                    var vec = this.Value[var_index][time_index];
                    for (int i = 0; i < vec.Length; i++)
                    {
                        var lc = Topology.ActiveCell[i];
                        array[lc[0], lc[1]] = vec[i];
                    }
                }
            }
            else if (Flags[var_index,time_index] == TimeVarientFlag.Constant)
            {
                array = new T[1, 1];
                array[0, 0] = (T)Constants.GetValue(var_index,time_index);
            }
            else if (Flags[var_index,time_index] == TimeVarientFlag.Repeat)
            {
                array = new T[1, 1];
                array[0, 0] = TypeConverterEx.ChangeType<T>(-1);
            }
            return array;
        }
        public override void FromSerialArray(int var_index, int time_index, Array array)
        {
            if (Flags[var_index,time_index] == TimeVarientFlag.Individual)
            {
                var len = array.GetLength(0);
                if (this.Value[var_index][time_index] == null || this.Value[var_index][time_index].Length != len)
                    this.Value[var_index][time_index] = new T[len];
                for (int i = 0; i < len; i++)
                    this.Value[var_index][time_index][i] = (T)(array.GetValue(i, 0));
            }
        }
        public override void FromRegularArray(int var_index, int time_index, Array array)
        {
            if (Flags[var_index,time_index] == TimeVarientFlag.Individual)
            {
                var nrow = array.GetLength(0);
                var ncol = array.GetLength(1);
                if(Topology != null)
                {
                    var vec = this.Value[var_index][time_index];
                    for (int i = 0; i < vec.Length; i++)
                    {
                        var lc = Topology.ActiveCell[i];
                        this.Value[var_index][time_index][i] = (T)(array.GetValue(lc[0], lc[1]));
                    }
                }        
            }
        }
        public override string SizeString()
        {
            if (Value != null)
                return string.Format("{0}×{1}×*", Size[0], Size[1]);
            else
                return "Empty";
        }

        public override double Sum()
        {
            double sum = 0.0;
            for (int r = 0; r < Size[0]; r++)
                for (int c = 0; c < Size[1]; c++)
                {
                    if (Flags[r, c] == TimeVarientFlag.Individual)
                    {
                        var vec = Value[r][c];
                        for (int v = 0; v < vec.Length; v++)
                            sum += double.Parse(Value[r][c][v].ToString());
                    }
                }
            return sum;
        }

        /// <summary>
        /// find avaiable time index for repeating
        /// </summary>
        /// <param name="var_index"></param>
        /// <param name="time_index"></param>
        /// <returns></returns>
        public int RepeatAt(int var_index, int time_index)
        {
            int pre = time_index - 1;
            if (pre == 0)
                return pre;
            else
            {
                if (Flags[var_index,pre] == TimeVarientFlag.Individual)
                    return pre;
                else
                    return RepeatAt(var_index, pre);
            }
        }
        /// <summary>
        /// find avaiable variable index for repeating
        /// </summary>
        /// <param name="var_index"></param>
        /// <param name="time_index"></param>
        /// <returns></returns>
        public int RepeatAt(int var_index)
        {
            int pre = var_index - 1;
            if (pre == 0)
                return pre;
            else
            {
                if (Flags[pre, 0] == TimeVarientFlag.Individual)
                    return pre;
                else
                    return RepeatAt(pre, 0);
            }
        }

        public override int GetSpaceDimLength(int var_index, int time_index)
        {
            if (Flags[var_index, time_index] == TimeVarientFlag.Individual)
                return base.GetSpaceDimLength(var_index, time_index);
            else
            {
                return 1;
            }
        }

        public override ILArray<T> ToILArray()
        {
            if (Topology != null)
            {
                int nrow = Topology.RowCount;
                int ncol = Topology.ColumnCount;
                ILArray<T> array = ILMath.zeros<T>(nrow, ncol);
                if(this.Flags[SelectedVariableIndex,SelectedTimeIndex] == TimeVarientFlag.Constant)
                {
                    array[":,:"] = TypeConverterEx.ChangeType<T>(Constants[SelectedVariableIndex, SelectedTimeIndex]);
                }
                else if (this.Flags[SelectedVariableIndex, SelectedTimeIndex] == TimeVarientFlag.Individual)
                {
                    for (int i = 0; i < Topology.ActiveCellCount; i++)
                    {
                        var loc = Topology.ActiveCell[i];
                        //flip the matrix
                        array.SetValue(this.Value[SelectedVariableIndex][SelectedTimeIndex][i], nrow - 1 - loc[0], loc[1]);
                    }
                }
                else
                {

                }
                return array;
            }
            else
            {
                return null;
            }
        }

    }
}
