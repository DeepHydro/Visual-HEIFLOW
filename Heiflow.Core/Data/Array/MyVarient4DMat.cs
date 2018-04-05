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

using ILNumerics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    public class MyVarient4DMat<T> : My3DMat<T>
    {
        private float scale = 1;
        private T[][][][] _array;

        public MyVarient4DMat(int size0, int size1)
        {
            Construct(size0, size1);
            _array = new T[size0][][][];
            for (int i = 0; i < size0; i++)
            {
                _array[i] = new T[size1][][];
            }
            Size = new int[] { size0, size1, -1, -1 };

            InitFlags(size0, size1);
            PopulateVariables();
            DataCubeType = Data.DataCubeType.Varient;
            TimeBrowsable = false;
            AllowTableEdit = false; 
        }

        public MyVarient4DMat(int size0, int size1, int size2_row, int size2_col)
        {
            Construct(size0, size1);
            _array = new T[size0][][][];
            for (int i = 0; i < size0; i++)
            {
                _array[i] = new T[size1][][];
                for (int j = 0; j < size1; j++)
                {
                    _array[i][j] = new T[size2_row][];
                    for (int k = 0; k < size2_row; k++)
                        _array[i][j][k] = new T[size2_col];
                }
            }
        }

        public new Array ArrayObject
        {
            get
            {
                return _array;
            }
        }

        public new T[][][][] Value
        {
            get
            {
                return _array;
            }
        }

        public int DefaultSpaceColumnIndex
        {
            get;
            set;
        }

        public string[] ColumnNames
        {
            get;
            set;
        }

        private void Construct(int size0, int size1)
        {
            Variables = new string[size0];
            Flags = new TimeVarientFlag[size0, size1];
            Constants = new float[size0, size1];
            Multipliers = new float[size0, size1];
            IPRN = new int[size0, size1];     
            for (int i = 0; i < size0; i++)
            {
                for (int j = 0; j < size1; j++)
                {
                    Flags[i, j] = TimeVarientFlag.Individual;
                    Multipliers.SetValue(scale, i, j);
                    Constants.SetValue(0, i, j);
                    IPRN[i, j] = -1;
                }
            }
            DataCubeType = Data.DataCubeType.Varient;
            TimeBrowsable = true;
            AllowTableEdit = true;
        }

        public override void AllocateSpaceDim(int var_index, int time_index, int nlen)
        {
            _array[var_index][time_index] = new T[1][];
            _array[var_index][time_index][0] = new T[nlen];
        }

        public override void AllocateSpaceDim(int var_index, int time_index, int space_row, int space_col)
        {
            _array[var_index][time_index] = new T[space_row][];
            for (int i = 0; i < space_row; i++)
                _array[var_index][time_index][i] = new T[space_col];
        }
        public int[] GetSpaceDims(int var_index, int time_index)
        {
            var mat = _array[var_index][time_index];
            var nrow = mat.GetLength(0);
            var ncol = mat[0].GetLength(0);
            return new int[] { nrow, ncol };
        }

        public override void Constant(T cnst)
        {
            for (int i = 0; i < Size[0]; i++)
            {
                for (int j = 0; j < Size[1]; j++)
                {
                    var nrow = _array[i][j].GetLength(0);
                    var ncol = _array[i][j].GetLength(1);
                    for (int r = 0; r < nrow; r++)
                    {
                        for (int c = 0; c < ncol; c++)
                        {
                            _array[i][j][r][c] = cnst;
                        }
                    }
                }
            }
        }

        public override void FromDataTable(DataTable dt)
        {
            var mat = _array[SelectedVariableIndex][SelectedTimeIndex];
            var mat_size = GetSpaceDims(SelectedVariableIndex, SelectedTimeIndex);

            if(dt.Rows.Count == mat_size[0] && dt.Columns.Count == mat_size[1])
            {
                for (int r = 0; r < mat_size[0]; r++)
                {
                    var dr = dt.Rows[r];
                    for (int c = 0; c < mat_size[1]; c++)
                    {
                        mat[r][c] = (T)(dr[c]);
                    }
                }
            }
        }

        public override DataTable ToDataTableByTime(int var_index, int time_index)
        {
            var dt = new DataTable();
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                DataColumn dc = new DataColumn(ColumnNames[i], typeof(T));
                dt.Columns.Add(dc);
            }
            if (Flags[var_index, time_index] == TimeVarientFlag.Individual)
            {
                var mat_size = GetSpaceDims(var_index, time_index);
            
                if (_array[var_index][time_index] != null)
                {
                    for (int r = 0; r < mat_size[0]; r++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int c = 0; c < mat_size[1]; c++)
                        {
                            dr[c] = _array[var_index][time_index][r][c];
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            else if (Flags[var_index, time_index] == TimeVarientFlag.Repeat)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < ColumnNames.Length; i++)
                {  
                    dr[i] = -1;
                }
                dt.Rows.Add(dr);
            }
            else if (Flags[var_index, time_index] == TimeVarientFlag.Constant)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < ColumnNames.Length; i++)
                {
                    dr[i] = Constants[var_index, time_index];
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public override DataTable ToDataTable()
        {

            return new DataTable();
        }
        public override void FromRegularArray(int var_index, int time_index, Array array)
        {
            var mat_size = GetSpaceDims(var_index, time_index);
            {
                for (int r = 0; r < mat_size[0]; r++)
                {
                    for (int c = 0; c < mat_size[1]; c++)
                    {
                        _array[var_index][time_index][r][c] = (T)array.GetValue(r, c);
                    }
                }
            }
        }

        public override Array GetRegularlArrayByTime(int var_index, int time_index)
        {
            if (Flags[var_index, time_index] == TimeVarientFlag.Individual)
            {
                var mat_size = GetSpaceDims(var_index, time_index);
                var array = new T[mat_size[0], mat_size[1]];
                for (int r = 0; r < mat_size[0]; r++)
                {
                    for (int c = 0; c < mat_size[1]; c++)
                    {
                        array[r, c] = _array[var_index][time_index][r][c];
                    }
                }
                return array;
            }
            else if (Flags[var_index, time_index] == TimeVarientFlag.Repeat)
            {
                var array = new T[1,1];
                array[0, 0] =  TypeConverterEx.ChangeType<T>(-1);
                return array;
            }
            else if (Flags[var_index, time_index] == TimeVarientFlag.Constant)
            {
                var array = new T[1, 1];
                array[0, 0] = TypeConverterEx.ChangeType<T>(Constants[var_index,time_index]);
                return array;
            }
            else
                return null;
        }

        public override Array GetSerialArrayByTime(int var_index, int time_index)
        {
            return GetRegularlArrayByTime(var_index, time_index);
        }

        public override string SizeString()
        {
            var str = string.Format("[{0}][{1}][.,.]", Size[0], Size[1]);
            return str;
        }

        public override Array GetByTime(int var_index, int time_index)
        {
            if (_array[var_index][time_index] != null)
            {
                var mat_size = GetSpaceDims(var_index, time_index);
                var vec = new T[mat_size[0]];
                for (int r = 0; r < mat_size[0]; r++)
                {
                    vec[r] = _array[var_index][time_index][r][DefaultSpaceColumnIndex];
                }
                return vec;
            }
            else
            {
                return null;
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
                    array.SetValue(this.Value[SelectedVariableIndex][SelectedTimeIndex][i][DefaultSpaceColumnIndex], nrow - 1 - loc[0], loc[1]);
                }
                return array;
            }
            else
            {
                return null;
            }
        }

        public override ILBaseArray ToILBaseArray(int var_index, int time_index)
        {
            this.SelectedTimeIndex = time_index;
            this.SelectedVariableIndex = var_index;
            return this.ToILArray();
        }

        public override bool IsAllocated(int var_index)
        {
            return _array[var_index] != null;
        }

    }
}
