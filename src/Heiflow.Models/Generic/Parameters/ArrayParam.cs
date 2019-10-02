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

using DotSpatial.Data;
using GeoAPI.Geometries;
using Heiflow.Core.Data;
using Heiflow.Spatial.Geography;
using ILNumerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic.Parameters
{
    [Serializable]
    public class ArrayParam<T> : Parameter
    {
        private T[] _Values;


        public ArrayParam(string name)
            : base(name)
        {
        }


        [Browsable(false)]
        [XmlIgnore]
        public T[] Values
        {
            get
            {
                return _Values;
            }
            set
            {
                _Values = value;
                _valueCount = _Values.Length;
                _Size[2] = _valueCount;
                _array = _Values;
                if (ValueType < 4)
                {
                    _DataCube = new DataCube2DLayout<float>(1, 1, _valueCount, false);
                    _DataCube[0, "0", ":"] = (from v in _Values select float.Parse(v.ToString())).ToArray();
                    _DataCube.Variables = new string[] { this.Name };
                }
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public new int ValueCount
        {
            get
            {
                if (Values != null)
                    _valueCount = Values.Length;
                else
                    _valueCount = 0;
                return _valueCount;
            }
        }
        public override IEnumerable<double> ToDouble()
        {
            return TypeConverterEx.ChangeType<T, double>(_Values);
        }

        public override IEnumerable<float> ToFloat()
        {
            return TypeConverterEx.ChangeType<T, float>(_Values);
        }

        public override IEnumerable<int> ToInt32()
        {
            return TypeConverterEx.ChangeType<T, int>(_Values);
        }

        public override string ToString()
        {
            return string.Join(",", _Values);
        }

        public override string[] ToStringVector()
        {
            var vv = (from v in _Values select v.ToString()).ToArray();
            return vv;
        }

        public override void AlterDimLength(int new_length)
        {
            if (new_length == Values.Length)
                return;
            var default_vv = Values[0];
            Values = new T[new_length];
            MatrixExtension<T>.Set(Values, default_vv);
        }

        public override void SetValue(object vv, int index)
        {
            Values[index] = (T)Convert.ChangeType(vv, typeof(T));
        }

        public override void Constant(object vv)
        {
            var constant = (T)vv;
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = constant;
            }
        }

        public override void SetValues<T1>(T1[] vv)
        {
            for (int i = 0; i < vv.Length; i++)
            {
                Values[i] = (T)Convert.ChangeType(vv[i], typeof(T1));
            }
        }

        public override void ResetToDefault()
        {
            if (Values != null)
            {
                for (int i = 0; i < Values.Length; i++)
                {
                    Values[i] = (T)Convert.ChangeType(DefaultValue, typeof(T)); ;
                }
            }
        }
        public override void UpdateFrom(IParameter new_para)
        {
            var vv = new_para as ArrayParam<T>;
            if (vv != null && vv.ValueCount == this.ValueCount)
            {
                for (int i = 0; i < ValueCount; i++)
                {
                    this.Values[i] = vv.Values[i];
                }
            }
        }

        public override System.Data.DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn(Name, typeof(T));
            dt.Columns.Add(dc);
            for (int i = 0; i < ValueCount; i++)
            {
                var dr = dt.NewRow();
                dr[0] = Values[i];
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public override void FromDataTable(System.Data.DataTable dt)
        {
            if (dt.Rows.Count == Values.Length)
            {
                if (dt.Columns[0].DataType == typeof(T))
                {
                    var vv = from dr in dt.AsEnumerable() select dr.Field<T>(0);
                    Values = vv.ToArray();
                }
                else
                {
                    var type = typeof(T);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Values[i] = (T)Convert.ChangeType(dt.Rows[i][0], type);
                    }
                }
            }
        }

        public override DataTable ToDataTableBySpace(int var_index, int space_index)
        {
            return ToDataTable();
        }

        public override DataTable ToDataTableByTime(int var_index, int time_index)
        {
            return ToDataTable();
        }

        public override Array GetBySpace(int var_index, int space_index)
        {
            return this.Values;
        }

        public override Array GetByTime(int var_index, int time_index)
        {
            return this.Values;
        }

        public override int GetSpaceDimLength(int var_index, int time_index)
        {
            return Values.Length;
        }

        public override void AllocateVariable(int var_index, int ntime, int ncell)
        {
            Values = new T[ncell];
        }

        public override void FromRegularArray(int p1, int p2, Array array)
        {
            var nrow = array.GetLength(0);
            var ncol = array.GetLength(1);
            if (Topology != null && ValueCount == Topology.ActiveCellCount)
            {
                for (int i = 0; i < ValueCount; i++)
                {
                    var lc = Topology.ActiveCell[i];
                    this.Values[i] = (T)(array.GetValue(lc[0], lc[1]));
                }
            }
        }

        public override void FromSerialArray(int p1, int p2, Array array)
        {
            if (array.GetLength(0) == ValueCount)
            {
                for (int i = 0; i < ValueCount; i++)
                {
                    this.Values[i] = (T)array.GetValue(i, 0);
                }
            }
        }

        public override Array GetSerialArrayByTime(int p1, int p2)
        {
            T[,] array = null;

            array = new T[ValueCount, 1];
            for (int i = 0; i < ValueCount; i++)
            {
                array[i, 0] = Values[i];
            }
            return array;
        }

        public override Array GetRegularlArrayByTime(int p1, int p2)
        {
            T[,] array = null;

            if (Topology != null && ValueCount == Topology.ActiveCellCount)
            {
                array = new T[Topology.RowCount, Topology.ColumnCount];
                for (int i = 0; i < ValueCount; i++)
                {
                    var lc = Topology.ActiveCell[i];
                    array[lc[0], lc[1]] = Values[i];
                }
            }

            return array;
        }

        public override void OnDataCubeValueChanged()
        {
            //for (int i = 0; i < ValueCount; i++)
            //{
            //    Values[i] = TypeConverterEx.ChangeType<T>(_DataCube.Value[0][0][i]);
            //}
            base.OnDataCubeValueChanged();
        }
    }
}
