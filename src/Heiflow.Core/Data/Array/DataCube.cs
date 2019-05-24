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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILNumerics;
using Heiflow.Core.Data.ODM;
using System.Data;

namespace Heiflow.Core.Data
{
    public enum DataCubeLayout { OneD,TwoD,ThreeD,OneDTimeSeries};
    public class DataCube<T>:IDataCubeObject
    {
        public event EventHandler DataCubeValueChanged;
        protected ILArray<T>[] _arrays;
        protected int _nvar;
        protected int _ntime;
        protected int _ncell;
        protected int[] _size;
        protected string[] _Variables;
        protected bool _isLazy;
        protected DataCubeLayout _DataCubeLayout;
        public DataCube(int nvar, int ntime, int ncell, bool islazy = false)
        {
            _isLazy = islazy;
            _nvar = nvar;
            _ntime = ntime;
            _ncell = ncell;
            _size = new int[] { nvar, ntime, ncell };
            _arrays = new ILArray<T>[nvar];
            if (!islazy)
            {
                for (int i = 0; i < nvar; i++)
                {
                    _arrays[i] = ILMath.zeros<T>(ntime, ncell);
                }
                DataCubeType = Data.DataCubeType.General;
            }
            else
            {
                DataCubeType = Data.DataCubeType.Varient;
            }
            Name = "default";
            PopulateVariables();
            InitFlags(Size[0], Size[1]);
         
            AllowTableEdit = false;
            TimeBrowsable = false;
            _DataCubeLayout = DataCubeLayout.ThreeD;
        }
        public DataCube(T[] values, DateTime [] dates)
        {
            _isLazy = false;
            _nvar = 1;
            _ntime = dates.Length;
            _ncell = 1;
            _size = new int[] { _nvar, _ntime, _ncell };
            _arrays = new ILArray<T>[_nvar];
            _arrays[0] = ILMath.zeros<T>(_ntime, 1);
            _arrays[0][":", "0"] = values;
            DateTimes = dates;
            Name = "Time Series";
            PopulateVariables();
            InitFlags(Size[0], Size[1]);
            DataCubeType = Data.DataCubeType.Vector;
            AllowTableEdit = true;
            TimeBrowsable = true;
            _DataCubeLayout = DataCubeLayout.ThreeD;
        }
        public ILArray<T> [] ILArrays
        {
            get
            {
                return _arrays;
            }
        }
        public bool IsLazy
        {
            get
            {
                return _isLazy;
            }
        }
        public T this[int var_index, int time_index, int cell_index]
        {
            get
            {
                return _arrays[var_index].GetValue(time_index, cell_index);
            }
            set
            {
                _arrays[var_index].SetValue(value,time_index, cell_index);
            }
        }

        public T[] this[int var_index, string time_index, string cell_index]
        {
            get
            {
                return _arrays[var_index].Subarray(time_index, cell_index).ToArray();
            }
            set
            {
                _arrays[var_index][time_index, cell_index] = value;
            }
        }
        public ILArray<T> this[int var_index]
        {
            get
            {
                return _arrays[var_index];
            }
            set
            {
                _arrays[var_index] = value;
            }
        }
        public Array ArrayObject
        {
            get { return null; }
        }

        public string Name
        {
            get;
            set;
        }

        public object DataOwner
        {
            get;
            set;
        }

        public string OwnerName
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

        public DateTime[] DateTimes
        {
            get;
            set;
        }

        public int[] Size
        {
            get 
            {
                return _size;
            }
        }

        public TimeVarientFlag[,] Flags
        {
            get;
            set;
        }

        public string[] Variables
        {
            get
            {
                return _Variables;
            }
            set
            {
                _Variables = value;
            }
        }

        public float[,] Constants
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
        public bool TimeBrowsable
        {
            get;
            set;
        }

        public DataCubeType DataCubeType
        {
            get;
            set;
        }

        public bool AllowTableEdit
        {
            get;
            set;
        }
        
        public DataCubeLayout Layout
        {
            get
            {
                return _DataCubeLayout;
            }
            set
            {
                _DataCubeLayout = value;
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
        public virtual void PopulateVariables()
        {
            int nvar = Size[0];
            _Variables = new string[nvar];
            for (int i = 0; i < nvar; i++)
            {
                _Variables[i] = "V" + (i + 1);
            }
        }
        public bool SizeEquals(DataCube<T> target)
        {
            if (target != null)
            {
                bool equal = true;
                for (int i = 0; i < 3; i++)
                {
                    equal = this.Size[i] == target.Size[i];
                }
                return equal;
            }
            else
            {
                return false;
            }
        }

        public int[] GetVariableSize(int var_index)
        {
            int[] size = null;
            if(_arrays[var_index] != null)
            {
                size = _arrays[var_index].Size.ToIntArray();
            }
            return size;
        }
        public IGridTopology Topology
        {
            get;
            set;
        }
        public void Allocate(int var_index)
        {
            _arrays[var_index] = ILMath.zeros<T>(_ntime, _ncell);
        }
        public void Allocate(int var_index, int ntime, int ncell)
        {
            _arrays[var_index] = ILMath.zeros<T>(ntime, ncell);
        }
        public Array GetByTime(int var_index, int time_index)
        {
            return GetVector(var_index, time_index.ToString(), ":");
        }

        public Array GetBySpace(int var_index, int space_index)
        {
            return GetVector(var_index, ":", space_index.ToString());
        }

        public int GetSpaceDimLength(int var_index, int time_index)
        {
            return _arrays[var_index].Size.ToIntArray()[1];
        }

        public Array GetSerialArrayByTime(int var_index, int time_index)
        {
            T[,] array = null;
            var vec = GetVector(var_index, time_index.ToString(), ":");
            array = new T[vec.Length, 1];
            for (int i = 0; i < vec.Length; i++)
            {
                array[i, 0] = vec[i];
            }
            return array;
        }

        public Array GetRegularlArrayByTime(int var_index, int time_index)
        {
            T[,] array = null;

            if (Topology != null)
            {
                array = new T[Topology.RowCount, Topology.ColumnCount];
                var vec = GetVector(var_index, time_index.ToString(), ":");
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
            for (int i = 0; i < len; i++)
                this[var_index,time_index,i] = TypeConverterEx.ChangeType<T>(array.GetValue(i, 0));
        }
        public virtual void FromRegularArray(int var_index, int time_index, Array array)
        {
            var nrow = array.GetLength(0);
            var ncol = array.GetLength(1);
            if (Topology != null)
            {
                var vec = GetVector(var_index, time_index.ToString(), ":");
                for (int i = 0; i < vec.Length; i++)
                {
                    var lc = Topology.ActiveCell[i];
                    this[var_index,time_index,i] = TypeConverterEx.ChangeType<T>(array.GetValue(lc[0], lc[1]));
                }
            }
        }

        public bool IsAllocated(int var_index)
        {
            return _arrays[var_index] != null;
        }

        public ILNumerics.ILBaseArray ToILBaseArray(int var_index, int time_index)
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
                    array[nrow - 1 - loc[0], loc[1]] = this[var_index, time_index, i];
                }
                return array;
            }
            else
            {
                return null;
            }
        }
        public void SetSize(int [] size)
        {
            _size = size;
        }
        public string SizeString()
        {
            if (Size != null)
                return string.Format("{0}×{1}×{2}", Size[0], Size[1], Size[2]);
            else
                return "empty";
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
                        dr[i] = this[i,time_index,r];
                    }
                    dt.Rows.Add(dr);
                }
            }
            else
            {
                DataColumn dc = new DataColumn(Variables[var_index], typeof(T));
                dt.Columns.Add(dc);

                if (_arrays[var_index]!= null)
                {
                    for (int r = 0; r < Size[2]; r++)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = this[var_index,time_index,r];
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
                    for (int i = 0; i < nvar; i++)
                    {
                        dr[i + 1] = this[i,t,space_index];
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
                    dr[1] = this[var_index,t,space_index];
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
            dt.AcceptChanges();
            return dt;
        }

        public System.Data.DataTable ToDataTable()
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
                    dr[j] = this[j,SelectedTimeIndex,i];
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public void FromDataTable(System.Data.DataTable dt)
        {
            if (_arrays == null)
            {
                int nrow = dt.Rows.Count;
                int ncol = dt.Columns.Count;
                _size = new int[] { nrow, ncol };
                 
            }
            //TODO
            if (SelectedVariableIndex >= 0)
            {
                for (int r = 0; r < Size[2]; r++)
                {
                    DataRow dr = dt.Rows[r];
                    this[SelectedVariableIndex,SelectedTimeIndex,r] = (T)dr[0];
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
                        var var_name = buf.ElementAt(i);
                        var_index[i] = varlist.IndexOf(var_name);
                        col_index[i] = colnames.IndexOf(var_name);
                    }
                    for (int r = 0; r < Size[2]; r++)
                    {
                        DataRow dr = dt.Rows[r];
                        for (int i = 0; i < nvar; i++)
                        {
                            this[var_index[i],SelectedTimeIndex,r] = (T)dr[col_index[i]];
                        }
                    }
                }
            }
            OnDataCubeValueChanged();
        }

        public T[] GetVector(int var_index, string time_arg, string cell_arg)
        {
            if (_arrays != null && _arrays[var_index] != null)
                return _arrays[var_index].Subarray(time_arg, cell_arg).ToArray();
            else
                return null;
        }
        public DataCube<float> GetSeriesBetween(int var_index, int spatial_index, DateTime start, DateTime end)
        {
            int start_index = 0;
            int end_index = 0;
            int i = 0;
            var vector = GetVector(var_index, ":", spatial_index.ToString()).Cast<float>().ToArray();
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
            DataCube<float> ts = new DataCube<float>(values, dates);
            return ts;
        }
        public DataCube<double> GetDoubleSeriesBetween(int var_index, int spatial_index, DateTime start, DateTime end)
        {
            int start_index = 0;
            int end_index = 0;
            int i = 0;
            var vector = GetVector(var_index, ":", spatial_index.ToString()).Cast<float>().ToArray();
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
            if (end > DateTimes.Last())
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
            DataCube<double> ts = new DataCube<double>(values, dates);
            return ts;
        }

        public void Clear()
        {
            for (int i = 0; i < Size[0]; i++)
            {
                _arrays[i].Dispose();
                _arrays[i]= null;
            }
        }

        public void OnDataCubeValueChanged()
        {
            if (DataCubeValueChanged != null)
                DataCubeValueChanged(this, EventArgs.Empty);
        }


        public void AllocateVariable(int var_index,int ntime, int ncell)
        {
            _arrays[var_index] = ILMath.zeros<T>(ntime, ncell);
        }

        public DataCube<float> SpatialMean(int var_index)
        {
           var mean_mat = new DataCube<float>(1, _ntime, 1);
            if (ILArrays[var_index] != null)
            {
                for (int j = 0; j < _ntime; j++)
                {
                    var vec = GetVector(var_index, j.ToString(), ":").Cast<float>();
                    var av = vec.Average();
                    mean_mat[0, j, 0] = av;
                }
            }
            return mean_mat;
        }

        public IEnumerable<TimeSeriesPair<T>> ToPairs()
        {
            var pairs = new TimeSeriesPair<T>[Size[1]];
            for (int i = 0; i < Size[1]; i++)
            {
                pairs[i] = new TimeSeriesPair<T>(DateTimes[i], this[0, i, 0]);
            }
            return pairs;
        }

        public static DataCube<T> FromPairs(IEnumerable<TimeSeriesPair<T>> pairs)
        {
            var values = from pair in pairs select pair.Value;
            var dates = from pair in pairs select pair.DateTime;
            DataCube<T> dc = new DataCube<T>(values.ToArray(), dates.ToArray());
            return dc;
        }
    }
}
