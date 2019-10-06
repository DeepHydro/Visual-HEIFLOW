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
using System.Xml.Serialization;
using System.ComponentModel;

namespace Heiflow.Core.Data
{
    public enum DataCubeLayout { TwoD, ThreeD, OneDTimeSeries };
    [Serializable]
    public class DataCube<T> : IDataCubeObject
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
        public DataCube()
        {

        }
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
            }
            Name = "Default";        
            PopulateVariables();
            InitFlags(Size[0]);
            _DataCubeLayout = DataCubeLayout.ThreeD;
        }
        public DataCube(T[] values, DateTime[] dates)
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
            InitFlags(Size[0]);
            _DataCubeLayout = DataCubeLayout.ThreeD;
            ZeroDimension = DimensionFlag.Variable;
        }
        [XmlIgnore]
        [Browsable(false)]
        public ILArray<T>[] ILArrays
        {
            get
            {
                return _arrays;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public bool IsLazy
        {
            get
            {
                return _isLazy;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public T this[int var_index, int time_index, int cell_index]
        {
            get
            {
                return _arrays[var_index].GetValue(time_index, cell_index);
            }
            set
            {
                _arrays[var_index].SetValue(value, time_index, cell_index);
            }
        }
        [XmlIgnore]
        [Browsable(false)]
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
        [XmlIgnore]
        [Browsable(false)]
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
        [XmlElement]
        public string Name
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public object DataOwner
        {
            get;
            set;
        }
        [XmlIgnore]
        public string OwnerName
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        /// <summary>
        /// Default values are provided.
        /// </summary>
        public string[] ColumnNames
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public DimensionFlag ZeroDimension
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public int SelectedVariableIndex
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public int SelectedTimeIndex
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public int SelectedSpaceIndex
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public DateTime[] DateTimes
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public int[] Size
        {
            get
            {
                return _size;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public TimeVarientFlag[] Flags
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
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
        /// <summary>
        /// 1d array [nvar]
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public float[] Constants
        {
            get;
            set;
        }
        /// <summary>
        /// 1d array [nvar]
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public float[] Multipliers
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        /// <summary>
        /// 1d array [nvar]. A flag that indicates if the array being read should be printed (written to the listing file) after it has been read. If IPRN is less than zero, the array will not be printed.
        /// </summary>
        public int[] IPRN
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
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
        [XmlIgnore]
        [Browsable(false)]
        public IGridTopology Topology
        {
            get;
            set;
        }
        /// <summary>
        /// init flags, constants, multipliers and IPRN
        /// </summary>
        /// <param name="init">wthether perform initing</param>
        /// <param name="size0">number of variables</param>
        /// <param name="size1">number of time</param>
        public virtual void InitFlags(int size0)
        {
            Flags = new TimeVarientFlag[size0];
            Constants = new float[size0];
            Multipliers = new float[size0];
            IPRN = new int[size0];
            for (int i = 0; i < size0; i++)
            {
                Flags[i] = TimeVarientFlag.Individual;
                Multipliers[i] = 1;
                Constants[i] = 0;
                IPRN[i] = -1;
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
            ColumnNames = new string[Size[1]];
            for (int i = 0; i < Size[1]; i++)
            {
                ColumnNames[i] = "C" + i;
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
        public string SizeString()
        {
            if (Size != null)
                return string.Format("{0}×{1}×{2}", Size[0], Size[1], Size[2]);
            else
                return "empty";
        }
        public void Allocate(int var_index)
        {
            _arrays[var_index] = ILMath.zeros<T>(_ntime, _ncell);
        }
        public bool IsAllocated(int var_index)
        {
            return _arrays[var_index] != null;
        }
        public void Allocate(int var_index, int ntime, int ncell)
        {
            _arrays[var_index] = ILMath.zeros<T>(ntime, ncell);
        }
        public void AllocateVariable(int var_index, int ntime, int ncell)
        {
            _arrays[var_index] = ILMath.zeros<T>(ntime, ncell);
        }

        //public Array GetByTime(int var_index, int time_index)
        //{
        //    return GetVector(var_index, time_index.ToString(), ":");
        //}
        public int GetSpaceDimLength(int var_index, int time_index)
        {
            return _arrays[var_index].Size.ToIntArray()[1];
        }
        /// <summary>
        /// get a vector for the specified variable, time and spatial dimensions
        /// </summary>
        /// <param name="var_index"></param>
        /// <param name="time_arg"></param>
        /// <param name="cell_arg"></param>
        /// <returns></returns>
        public T[] GetVector(int var_index, string time_arg, string cell_arg)
        {
            if (_arrays != null && _arrays[var_index] != null)
                return _arrays[var_index].Subarray(time_arg, cell_arg).ToArray();
            else
                return null;
        }
        /// <summary>
        /// get a 1D vector in the type of Array. The array size is [len,1]
        /// </summary>
        /// <param name="var_index"></param>
        /// <param name="time_arg"></param>
        /// <param name="cell_arg"></param>
        /// <returns></returns>
        public Array GetVectorAsArray(int var_index, string time_arg, string cell_arg)
        {
            return GetVector(var_index, time_arg, cell_arg);
        }
        /// <summary>
        /// get a 2D array for the specified variable and time dimensions. The array size is [ncell, 1]
        /// </summary>
        /// <param name="var_index"></param>
        /// <param name="time_index"></param>
        /// <returns></returns>
        public virtual Array GetSpatialSerialArray(int var_index, int time_index)
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
        /// <summary>
        /// get a 2D array for the specified variable and time dimensions. The array size is [Topology.RowCount, Topology.ColumnCount]
        /// </summary>
        /// <param name="var_index"></param>
        /// <param name="time_index"></param>
        /// <returns></returns>
        public virtual Array GetSpatialRegularArray(int var_index, int time_index)
        {
            T[,] array = null;

            if (Topology != null)
            {
                array = new T[Topology.RowCount, Topology.ColumnCount];
                var vec = GetVector(var_index, time_index.ToString(), ":");
                for (int i = 0; i < vec.Length; i++)
                {
                    var lc = Topology.ActiveCellLocation[i];
                    array[lc[0], lc[1]] = vec[i];
                }
            }

            return array;
        }
        /// <summary>
        /// retrieve value a given array. The array size must be [Topology.RowCount, Topology.ColumnCount]
        /// </summary>
        /// <param name="var_index"></param>
        /// <param name="time_index"></param>
        /// <param name="array"></param>
        public virtual void FromSpatialSerialArray(int var_index, int time_index, Array array)
        {
            var len = array.GetLength(0);
            for (int i = 0; i < len; i++)
                this[var_index, time_index, i] = TypeConverterEx.ChangeType<T>(array.GetValue(i, 0));
        }
        /// <summary>
        /// retrieve value a given array. The array size must be [ncell, 1]
        /// </summary>
        /// <param name="var_index"></param>
        /// <param name="time_index"></param>
        /// <param name="array"></param>
        public virtual void FromSpatialRegularArray(int var_index, int time_index, Array array)
        {
            var nrow = array.GetLength(0);
            var ncol = array.GetLength(1);
            if (Topology != null)
            {
                var vec = GetVector(var_index, time_index.ToString(), ":");
                for (int i = 0; i < vec.Length; i++)
                {
                    var lc = Topology.ActiveCellLocation[i];
                    this[var_index, time_index, i] = TypeConverterEx.ChangeType<T>(array.GetValue(lc[0], lc[1]));
                }
            }
        }
        /// <summary>
        /// convert the spatial dimention to 2D array
        /// </summary>
        /// <param name="var_index"></param>
        /// <param name="time_index"></param>
        /// <returns></returns>
        public virtual ILBaseArray ToSpatialILBaseArray(int var_index, int time_index)
        {
            if (Topology != null)
            {
                int nrow = Topology.RowCount;
                int ncol = Topology.ColumnCount;
                ILArray<T> array = ILMath.zeros<T>(nrow, ncol);

                for (int i = 0; i < Topology.ActiveCellCount; i++)
                {
                    var loc = Topology.ActiveCellLocation[i];
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
        public virtual System.Data.DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            return dt;
        }
        /// <summary>
        /// Convert datacube to datable. If one variable is specified, time dimension will be used as row, and cell dimension will be used as column.
        /// </summary>e
        /// <param name="var_index">if var_index = -1, all variables are regarded as table columns. In such a case, time_index or  cell_index must be -1, and the rest must be 0</param>
        /// <param name="time_index">if time_index = -1, all times will be used</param>
        /// <param name="cell_index">if cell_index = -1, all cells will be used</param>
        /// <returns></returns>
        public virtual DataTable ToDataTable(int var_index, int time_index, int cell_index)
        {
            DataTable dt = new DataTable();
            int ncell = Size[2];
            int ntime = Size[1];
            int nvar = Size[0];
            if(Flags[var_index] == TimeVarientFlag.Constant)
            {
                var dc = new DataColumn("C0", typeof(T));
                dt.Rows.Add(dc);
                var dr = dt.NewRow();
                dr[0] = Constants[var_index];
                dt.Rows.Add(dr);
            }
            else if (Flags[var_index] == TimeVarientFlag.Repeat)
            {
                var dc = new DataColumn("C0", typeof(T));
                dt.Rows.Add(dc);
                var dr = dt.NewRow();
                dr[0] = -1;
                dt.Rows.Add(dr);
            }
            else
            {
                if (var_index > -1)
                {
                    // all times
                    if (time_index == -1)
                    {
                        //all cells
                        if (cell_index == -1)
                        {
                            for (int i = 0; i < ncell; i++)
                            {
                                var dc = new DataColumn("C" + i, typeof(T));
                                dt.Columns.Add(dc);
                            }
                            for (int i = 0; i < ntime; i++)
                            {
                                var dr = dt.NewRow();
                                for (int j = 0; j < ncell; j++)
                                {
                                    dr[j] = this[var_index, i, j];
                                }
                                dt.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            var dc = new DataColumn("C0", typeof(T));
                            dt.Columns.Add(dc);
                            for (int i = 0; i < ntime; i++)
                            {
                                var dr = dt.NewRow();
                                dr[0] = this[var_index, i, cell_index];
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                    else
                    {
                        //all cells
                        if (cell_index == -1)
                        {
                            var dc = new DataColumn("C0", typeof(T));
                            dt.Columns.Add(dc);
                            for (int i = 0; i < ncell; i++)
                            {
                                var dr = dt.NewRow();
                                dr[0] = this[var_index, time_index, i];
                                dt.Rows.Add(dr);
                            }
                        }
                        else
                        {
                            var dc = new DataColumn("C0", typeof(T));
                            dt.Columns.Add(dc);
                            var dr = dt.NewRow();
                            dr[0] = this[var_index, time_index, cell_index];
                            dt.Rows.Add(dr);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < nvar; i++)
                    {
                        var dc = new DataColumn(Variables[i], typeof(T));
                        dt.Columns.Add(dc);
                    }
                    if (time_index == -1 && cell_index == 0)
                    {
                        for (int i = 0; i < ntime; i++)
                        {
                            var dr = dt.NewRow();
                            for (int j = 0; j < nvar; j++)
                            {
                                dr[j] = this[j, i, 0];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                    else if (time_index == 0 && cell_index == -1)
                    {
                        for (int i = 0; i < ncell; i++)
                        {
                            var dr = dt.NewRow();
                            for (int j = 0; j < nvar; j++)
                            {
                                dr[j] = this[j, 0, i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        throw new Exception("Wrong value for the argument time_index or cell_index");
                    }
                }
            }
            return dt;
        }
        public virtual void FromDataTable(System.Data.DataTable dt)
        {
            int ncell = Size[2];
            int ntime = Size[1];
            int nvar = Size[0];

            if (SelectedVariableIndex > -1)
            {
                if (Flags[SelectedVariableIndex] == TimeVarientFlag.Constant)
                {
                    Constants[SelectedVariableIndex] = float.Parse(dt.Rows[0][0].ToString());
                }
                else if (Flags[SelectedVariableIndex] == TimeVarientFlag.Repeat)
                {
                    
                }
                else
                {
                    // all times
                    if (SelectedTimeIndex == -1)
                    {
                        //all cells
                        if (SelectedSpaceIndex == -1)
                        {
                            for (int i = 0; i < ntime; i++)
                            {
                                var dr = dt.Rows[i];
                                for (int j = 0; j < ncell; j++)
                                {
                                    this[SelectedVariableIndex, i, j] = TypeConverterEx.ChangeType<T>(dr[j]);
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ntime; i++)
                            {
                                var dr = dt.Rows[i];
                                this[SelectedVariableIndex, i, SelectedSpaceIndex] = TypeConverterEx.ChangeType<T>(dr[0]);
                            }
                        }
                    }
                    else
                    {
                        //all cells
                        if (SelectedSpaceIndex == -1)
                        {
                            for (int i = 0; i < ncell; i++)
                            {
                                var dr = dt.NewRow();
                                 this[SelectedVariableIndex, SelectedTimeIndex, i] = TypeConverterEx.ChangeType<T>(dr[0]);
                            }
                        }
                        else
                        {
                            var dr = dt.NewRow();
                            this[SelectedVariableIndex, SelectedTimeIndex, SelectedSpaceIndex] = TypeConverterEx.ChangeType<T>(dr[0]);
                        }
                    }
                }
            }
            else
            {
               if (SelectedTimeIndex == -1 && SelectedSpaceIndex == 0)
                {
                    for (int i = 0; i < ntime; i++)
                    {
                        var dr = dt.Rows[i];
                        for (int j = 0; j < nvar; j++)
                        {
                            this[j, i, 0] = TypeConverterEx.ChangeType<T>(dr[j]);
                        }
                    }
                }
               else if (SelectedTimeIndex == 0 && SelectedSpaceIndex == -1)
                {
                    for (int i = 0; i < ncell; i++)
                    {
                        var dr = dt.Rows[i];
                        for (int j = 0; j < nvar; j++)
                        {
                            this[j, 0, i] = TypeConverterEx.ChangeType<T>(dr[j]);
                        }
                    }
                }
                else
                {
                    throw new Exception("Wrong value for the argument time_index or cell_index");
                }
            }

            OnDataCubeValueChanged();
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
                _arrays[i] = null;
            }
        }
        public void OnDataCubeValueChanged()
        {
            if (DataCubeValueChanged != null)
                DataCubeValueChanged(this, EventArgs.Empty);
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

        //public virtual DataTable ToDataTableByTime(int var_index, int time_index)
        //{
        //    DataTable dt = new DataTable();
        //    int nvar = Size[0];

        //    if (var_index < 0)
        //    {
        //        for (int i = 0; i < nvar; i++)
        //        {
        //            DataColumn dc = new DataColumn(Variables[i], typeof(T));
        //            dt.Columns.Add(dc);
        //        }

        //        for (int r = 0; r < Size[2]; r++)
        //        {
        //            DataRow dr = dt.NewRow();
        //            for (int i = 0; i < nvar; i++)
        //            {
        //                dr[i] = this[i, time_index, r];
        //            }
        //            dt.Rows.Add(dr);
        //        }
        //    }
        //    else
        //    {
        //        DataColumn dc = new DataColumn(Variables[var_index], typeof(T));
        //        dt.Columns.Add(dc);

        //        if (_arrays[var_index] != null)
        //        {
        //            for (int r = 0; r < Size[2]; r++)
        //            {
        //                DataRow dr = dt.NewRow();
        //                dr[0] = this[var_index, time_index, r];
        //                dt.Rows.Add(dr);
        //            }
        //        }
        //    }

        //    return dt;
        //}
        //public virtual DataTable ToDataTableBySpace(int var_index, int space_index)
        //{
        //    DataTable dt = new DataTable();
        //    int nvar = Size[0];
        //    int nstep = Size[1];
        //    DataColumn dc_time = new DataColumn("DateTime", Type.GetType("System.DateTime"));
        //    dt.Columns.Add(dc_time);
        //    if (var_index < 0)
        //    {
        //        for (int i = 0; i < nvar; i++)
        //        {
        //            DataColumn dc = new DataColumn(Variables[i], typeof(T));
        //            dt.Columns.Add(dc);
        //        }

        //        for (int t = 0; t < nstep; t++)
        //        {
        //            var dr = dt.NewRow();
        //            for (int i = 0; i < nvar; i++)
        //            {
        //                dr[i + 1] = this[i, t, space_index];
        //            }
        //            dt.Rows.Add(dr);
        //        }
        //    }
        //    else
        //    {
        //        DataColumn dc = new DataColumn(Variables[var_index], typeof(T));
        //        dt.Columns.Add(dc);

        //        for (int t = 0; t < nstep; t++)
        //        {
        //            var dr = dt.NewRow();
        //            dr[1] = this[var_index, t, space_index];
        //            dt.Rows.Add(dr);
        //        }
        //    }

        //    if (DateTimes != null && DateTimes.Length == dt.Rows.Count)
        //    {
        //        for (int t = 0; t < nstep; t++)
        //        {
        //            dt.Rows[t][0] = DateTimes[t];
        //        }
        //    }
        //    else
        //    {
        //        for (int t = 0; t < nstep; t++)
        //        {
        //            dt.Rows[t][0] = DateTime.Now.AddDays(t);
        //        }
        //    }
        //    dt.AcceptChanges();
        //    return dt;
        //}
        //public void FromDataTable(DataTable dt, int var_index, int time_index, int cell_index)
        //{
        //    int ncell = Size[2];
        //    int ntime = Size[1];
        //    int nvar = Size[0];
        //    if (var_index > -1)
        //    {
        //        // all times
        //        if (time_index == -1)
        //        {
        //            //all cells
        //            if (cell_index == -1)
        //            {
        //                for (int i = 0; i < ntime; i++)
        //                {
        //                    var dr = dt.Rows[i];
        //                    for (int j = 0; j < ncell; j++)
        //                    {
        //                        this[var_index, i, j] = (T)dr[j];
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                for (int i = 0; i < ntime; i++)
        //                {
        //                    var dr = dt.Rows[i];
        //                    this[var_index, i, cell_index] = (T)dr[0];
        //                }
        //            }
        //        }
        //        else
        //        {
        //            //all cells
        //            if (cell_index == -1)
        //            {
        //                var dc = new DataColumn("C0", typeof(T));
        //                dt.Columns.Add(dc);
        //                for (int i = 0; i < ncell; i++)
        //                {
        //                    var dr = dt.Rows[i];
        //                    this[var_index, time_index, i] = (T)dr[0];
        //                }
        //            }
        //            else
        //            {
        //                this[var_index, time_index, cell_index] = (T)dt.Rows[0][0];
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (time_index == -1 && cell_index == 0)
        //        {
        //            for (int i = 0; i < ntime; i++)
        //            {
        //                var dr = dt.Rows[i];
        //                for (int j = 0; j < nvar; j++)
        //                {
        //                    this[j, i, 0] = (T)dr[j];
        //                }
        //            }
        //        }
        //        else if (time_index == 0 && cell_index == -1)
        //        {
        //            for (int i = 0; i < ncell; i++)
        //            {
        //                var dr = dt.Rows[i];
        //                for (int j = 0; j < nvar; j++)
        //                {
        //                    this[j, 0, i] = (T)dr[j];
        //                }
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("Wrong value for the argument time_index or cell_index");
        //        }
        //    }
        //}
    }
}
