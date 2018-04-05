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
using ILNumerics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    public class My3DMat<T> : MyArray<T>, IDataCubeObject, ITimeSeries<T>
    {
        public event EventHandler DataCubeValueChanged;
         public My3DMat()
        {

        }
        public My3DMat(int size0, int size1, int size2)
        {
            Size = new int[] { size0, size1, size2 };
            Value = new T[Size[0]][][];
            for (int r = 0; r < Size[0]; r++)
            {
                Value[r] = new T[Size[1]][];
                for (int c = 0; c < Size[1]; c++)
                {
                    Value[r][c] = new T[Size[2]];
                }
            }
            PopulateVariables();
            InitFlags(Size[0], Size[1]);
            DataCubeType = Data.DataCubeType.General;
            AllowTableEdit = false;
            TimeBrowsable = false;
        }

        public My3DMat(T[][][] value)
        {
            Value = value;
            Size = new int[3];
            Size[0] = value.Length;
            Size[1] = value[0].Length;
            Size[2] = value[0][0].Length;

            InitFlags(Size[0], Size[1]);
            PopulateVariables();
            DataCubeType = Data.DataCubeType.General;
            TimeBrowsable = false;
            AllowTableEdit = false;     
        }

        public T[][][] Value
        {
            get;
            set;
        }

        public IGridTopology Topology
        {
            get;
            set;
        }
        public int SelectedVariableIndex
        {
            get;
            set;
        }
        public int SelectedTimeIndex
        {
            get;
            set;
        }
        public int SelectedSpaceIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 2d mat [nvar,ntime]
        /// </summary>
        public TimeVarientFlag[,] Flags
        {
            get;
            set;
        }
        /// <summary>
        ///  2d mat [nvar,ntime]
        /// </summary>
        public float[,] Constants
        {
            get;
            set;
        }
        public object DataOwner
        {
            get;
            set;
        }
        public float[,] Multipliers
        {
            get;
            set;
        }

        /// <summary>
        /// a flag that indicates if the array being read should be printed (written to the listing file) after it has been read. If IPRN is less than zero, the array will not be printed.
        /// </summary>
        public int[,] IPRN
        {
            get;
            set;
        }

        public Array ArrayObject
        {
            get { return this.Value; }
        }

        public bool TimeBrowsable
        {
            get;
            set;
        }

        public DataCubeType DataCubeType
        {
            get;
            protected set;
        }
        public bool AllowTableEdit
        {
            get;
            set;
        }
     
        /// <summary>
        ///  get vector
        /// </summary>
        /// <param name="i0">the index in zero dimension</param>
        /// <param name="c">the index in first dimension</param>
        /// <returns></returns>
        public override T[] this[int i0, int i1]
        {
            get
            {
                return Value[i0][i1];
            }
            set
            {
                Value[i0][i1] = value;
            }
        }

        public override T this[int i0, int i1, int i2]
        {
            get
            {
                return Value[i0][i1][i2];
            }
            set
            {
                Value[i0][i1][i2] = value;
            }
        }

        
        public void InitFlags(int size0, int size1)
        {
            Flags = new TimeVarientFlag[size0, size1];
            Constants = new float[size0, size1];
            Multipliers = new float[size0, size1];
            IPRN = new int[size0, size1];
            for (int i = 0; i < size0; i++)
            {
                for (int j = 0; j < size1; j++)
                {
                    Flags[i, j] = TimeVarientFlag.Individual;
                    Multipliers.SetValue(1, i, j);
                    Constants.SetValue(0, i, j);
                    IPRN[i, j] = -1;
                }
            }
        }

        public override T[] GetVector(int i0, int i1, int i2)
        {
            T[] vector = null;
            if (i0 == MyMath.full)
            {
                vector = new T[Size[0]];
                for (int i = 0; i < Size[0]; i++)
                {
                    vector[i] = Value[i][i1][i2];
                }
            }
            else if (i1 == MyMath.full)
            {
                vector = new T[Size[1]];
                if (Value[i0] != null)
                {
                    for (int i = 0; i < Size[1]; i++)
                    {
                        vector[i] = Value[i0][i][i2];
                    }
                }
            }
            else if (i2 == MyMath.full)
            {
                vector = Value[i0][i1];
            }
            return vector;
        }

        public override void SetBy(T[] vector, int i0, int i1, int i2)
        {
            if (vector != null)
            {
                if (i0 == MyMath.full)
                {
                    vector = new T[Size[0]];
                    for (int i = 0; i < Size[0]; i++)
                    {
                        Value[i][i1][i2] = vector[i];
                    }
                }
                else if (i1 == MyMath.full)
                {
                    vector = new T[Size[1]];
                    for (int i = 0; i < Size[1]; i++)
                    {
                        Value[i0][i][i2] = vector[i];
                    }
                }
                else if (i2 == MyMath.full)
                {
                    Value[i0][i1] = vector;
                }
            }
        }

        public override ILArray<T> ToILArray()
        {
            if (Topology != null)
            {
                int nrow = Topology.RowCount;
                int ncol = Topology.ColumnCount;
                ILArray<T> array = ILMath.zeros<T>(nrow, ncol);

                for (int i = 0; i < Topology.ActiveCellCount; i++)
                {
                    var loc = Topology.ActiveCell[i];
                    //flip the matrix
                    array.SetValue(this.Value[SelectedVariableIndex][SelectedTimeIndex][i], nrow - 1 - loc[0], loc[1]);
                }
                return array;
            }
            else
            {
                return null;
            }
        }
        public virtual ILBaseArray ToILBaseArray(int var_index, int time_index)
        {
            this.SelectedVariableIndex = var_index;
            this.SelectedTimeIndex = time_index;
            return this.ToILArray();
        }
        public override void Constant(T cnst)
        {
            for (int r = 0; r < Size[0]; r++)
                for (int c = 0; c < Size[1]; c++)
                    for (int v = 0; v < Size[2]; v++)
                        Value[r][c][v] = cnst;
        }

        public override double Sum()
        {
            double sum = 0.0;
            for (int r = 0; r < Size[0]; r++)
                for (int c = 0; c < Size[1]; c++)
                    for (int v = 0; v < Size[2]; v++)
                        sum += double.Parse(Value[r][c][v].ToString());
            return sum;
        }
        public override string SizeString()
        {
            if (Value != null)
                return string.Format("{0}×{1}×{2}", Size[0], Size[1], Size[2]);
            else
                return "empty";
        }
        public bool SizeEqualTo(My3DMat<T> target)
        {
            bool equal = true;
            if (target != null)
            {
                var tsize = target.Size;
                equal = this.Size[0] == tsize[0] && this.Size[1] == tsize[1] && this.Size[2] == tsize[2];
            }
            else
            {
                equal = false;
            }
            return equal;
        }
        public void CopyTo(My3DMat<T> target)
        {
            for (int i = 0; i < Size[0]; i++)
                for (int j = 0; j < Size[1]; j++)
                    for (int k = 0; k < Size[2]; k++)
                    {
                        target.Value[i][j][k] = this.Value[i][j][k];
                    }
        }
        public virtual Array GetSerialArrayByTime(int var_index, int time_index)
        {
            T[,] array = null;

            var vec = Value[var_index][time_index];
            array = new T[vec.Length, 1];
            for (int i = 0; i < vec.Length; i++)
            {
                array[i, 0] = vec[i];
            }
            return array;
        }
        public virtual Array GetRegularlArrayByTime(int var_index, int time_index)
        {
            T[,] array = null;

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

            return array;
        }
        public virtual void FromSerialArray(int var_index, int time_index, Array array)
        {
            var len = array.GetLength(0);
            if (this.Value[var_index][time_index] == null || this.Value[var_index][time_index].Length != len)
                this.Value[var_index][time_index] = new T[len];
            for (int i = 0; i < len; i++)
                this.Value[var_index][time_index][i] = TypeConverterEx.ChangeType<T>(array.GetValue(i, 0));
        }
        public virtual void FromRegularArray(int var_index, int time_index, Array array)
        {
            var nrow = array.GetLength(0);
            var ncol = array.GetLength(1);
            if (Topology != null)
            {
                var vec = this.Value[var_index][time_index];
                for (int i = 0; i < vec.Length; i++)
                {
                    var lc = Topology.ActiveCell[i];
                    this.Value[var_index][time_index][i] = TypeConverterEx.ChangeType<T>(array.GetValue(lc[0], lc[1]));
                }
            }
        }
        public virtual Array GetByTime(int var_index, int time_index)
        {
            return GetVector(var_index, time_index, MyMath.full);
        }
        public virtual Array GetBySpace(int var_index, int space_index)
        {
            return GetVector(var_index, MyMath.full, space_index);
        }
        public virtual DataTable ToDataTableByTime(int var_index, int time_index)
        {
            DataTable dt = new DataTable();
            int nvar = Size[0];

            if (var_index < 0)
            {
                for (int i = 0; i < nvar; i++)
                {
                    DataColumn dc = new DataColumn(Variables[i], typeof(T));
                    dt.Columns.Add(dc);
                }

                for (int r = 0; r < Size[2]; r++)
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < nvar; i++)
                    {
                        dr[r] = Value[i][time_index][r];      
                    }
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                DataColumn dc = new DataColumn(Variables[var_index], typeof(T));
                dt.Columns.Add(dc);

                if (Value[var_index][time_index] != null)
                {
                    for (int r = 0; r < Size[2]; r++)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = Value[var_index][time_index][r];
                        dt.Rows.Add(dr);
                    }
                }
            }

            return dt;
        }
        public virtual DataTable ToDataTableBySpace(int var_index, int space_index)
        {
            DataTable dt = new DataTable();
            int nvar = Size[0];
            int nstep = Size[1];
            DataColumn dc_time = new DataColumn("DateTime", Type.GetType("System.DateTime"));
            dt.Columns.Add(dc_time);
            if (var_index < 0)
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
            else
            {
                DataColumn dc = new DataColumn(Variables[var_index], typeof(T));
                dt.Columns.Add(dc);


                for (int t = 0; t < nstep; t++)
                {
                    var dr = dt.NewRow();
                    dr[0] = DateTime.Now;
                    dr[1] = Value[var_index][t][space_index];
                    dt.Rows.Add(dr);
                }
            }

            if (DateTimes != null && DateTimes.Length == dt.Rows.Count)
            {
                for (int t = 0; t < nstep; t++)
                {
                    dt.Rows[t][0] = DateTimes[t];
                }
            }
            else
            {
                for (int t = 0; t < nstep; t++)
                {
                    dt.Rows[t][0] = DateTime.Now.AddDays(t);
                }
            }
            return dt;
        }
        public IVectorTimeSeries<float> GetSeriesBetween(int var_index, int spatial_index, DateTime start, DateTime end)
        {
            int start_index = 0;
            int end_index = 0;
            int i = 0;
            var vector = GetVector(var_index, MyMath.full, spatial_index).Cast<float>().ToArray();
            foreach (var date in DateTimes)
            {
                if (date >= start)
                {
                    start_index = i;
                    break;
                }
                if (date >= end)
                {
                    end_index = i;
                }
                i++;
            }
            i = 0;
            foreach (var date in DateTimes)
            {
                if (date >= end)
                {
                    end_index = i;
                }
                i++;
            }
            int nlen = end_index - start_index + 1;
            nlen = Math.Min(nlen, vector.Length);
            float[] values = new float[nlen];
            var dates = new DateTime[nlen];
            for (i = 0; i < nlen; i++)
            {
                values[i] = vector[start_index + i];
                dates[i] = DateTimes[start_index + i];
            }
            FloatTimeSeries ts = new FloatTimeSeries(values, dates);
            return ts;
        }
        public IVectorTimeSeries<double> GetDoubleSeriesBetween(int var_index, int spatial_index, DateTime start, DateTime end)
        {
            int start_index = 0;
            int end_index = 0;
            int i = 0;
            var vector = GetVector(var_index, MyMath.full, spatial_index).Cast<float>().ToArray();
            foreach (var date in DateTimes)
            {
                if (date >= start)
                {
                    start_index = i;
                    break;
                }
                i++;
            }
            i = 0;
            foreach (var date in DateTimes)
            {
                if (date >= end)
                {
                    end_index = i;
                    break;
                }
                i++;
            }
            if(end > DateTimes.Last())
            {
                end_index = DateTimes.Length - 1;
            }
            int nlen = end_index - start_index + 1;
            nlen = Math.Min(nlen, vector.Length);
            double[] values = new double[nlen];
            var dates = new DateTime[nlen];
            for (i = 0; i < nlen; i++)
            {
                values[i] = vector[start_index + i];
                dates[i] = DateTimes[start_index + i];
            }
            DoubleTimeSeries ts = new DoubleTimeSeries(values, dates);
            return ts;
        }
        public virtual DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            foreach (var str in Variables)
            {
                DataColumn dc = new DataColumn(str, typeof(T));
                dt.Columns.Add(dc);
            }
            for (int i = 0; i < Size[2]; i++)
            {
                var dr = dt.NewRow();
                for (int j = 0; j < Size[0]; j++)
                {
                    dr[j] = this.Value[j][SelectedTimeIndex][i];
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public virtual void FromDataTable(System.Data.DataTable dt)
        {
            if (Value == null)
            {
                int nrow = dt.Rows.Count;
                int ncol = dt.Columns.Count;
                base.Size = new int[] { nrow, ncol };
                Initialize();
            }
            //TODO
            if (SelectedVariableIndex >= 0)
            {
                for (int r = 0; r < Size[2]; r++)
                {
                    DataRow dr = dt.Rows[r];
                    Value[SelectedVariableIndex][SelectedTimeIndex][r] = (T)dr[0];
                }
            }
            else
            {
                var colnames = (from DataColumn dc in dt.Columns select dc.ColumnName).ToList();
                var varlist = Variables.ToList();
                int ncol = dt.Columns.Count;
                var buf = varlist.Intersect(colnames);
                if (buf.Any())
                {
                    int nvar = buf.Count();
                    int[] var_index = new int[nvar];
                    int[] col_index = new int[nvar];
                    for (int i = 0; i < nvar; i++)
                    {
                        var var_name=buf.ElementAt(i);
                        var_index[i]=varlist.IndexOf(var_name);
                        col_index[i] = colnames.IndexOf(var_name);
                    }
                    for (int r = 0; r < Size[2]; r++)
                    {
                        DataRow dr = dt.Rows[r];
                        for(int i=0;i<nvar;i++)
                        {
                            Value[var_index[i]][SelectedTimeIndex][r] = (T)dr[col_index[i]];
                        }
                    }
                }
            }
        }
        public T[] GetSeriesAt(int var_index, int spatial_index)
        {
            return GetVector(var_index, MyMath.full, spatial_index);
        }

        public virtual int GetSpaceDimLength(int var_index, int time_index)
        {
            return Value[var_index][time_index].Length;
        }
        public virtual void AllocateSpaceDim(int var_index, int time_index, int nlen)
        {
            Value[var_index][time_index] = new T[nlen];
        }

        public virtual void AllocateSpaceDim(int var_index, int time_index, int space_row,int space_col)
        {
         }


        public virtual bool IsAllocated(int var_index)
        {
            return Value[var_index] != null;
        }

        public void OnDataCubeValueChanged()
        {
           if(DataCubeValueChanged != null)
           {
               DataCubeValueChanged(this, EventArgs.Empty);
           }
        }

        public void Clear()
        {
            for(int i=0;i<Size[0];i++)
            {
                Value[i] = null;
            }
        }
    }
}