﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Data;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Data;
using ILNumerics;

namespace Heiflow.Models.Generic.Parameters
{
    /// <summary>
    /// Parameter DataCube with 2D layout. The time and cell dimensions are treated as row and column respectively.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DataCubeParameter<T> : DataCube<T>, IParameter
    {
        protected int _valueCount;
        private DataCube2DLayout<float> _FloatDataCube;

        public DataCubeParameter()
        {

        }
        public DataCubeParameter(int nvar, int nrow, int ncol, bool islazy = false)
            : base(nvar, nrow, ncol, islazy)
        {
            Layout = DataCubeLayout.TwoD;
            VariableType = ParameterType.Parameter;
            Description = "";
            ModuleName = Modules.basin;
            CanEdit = true;
            Maximum = 1;
            Minimum = 0;
            Units = "";
            IsFixed = false;
        }
        [XmlElement]
        public string Description
        {
            get;
            set;
        }
        [Browsable(false)]
        [XmlIgnore]
        public IPackage Owner
        {
            get;
            set;
        }
        /// <summary>
        /// 0 is for short; 1 is for integer;2 is for real; 3 is for double; 4 is for character string; 5 is for object; 6 is for bool
        /// </summary>
        /// 
        [XmlElement]
        public int ValueType
        {
            get;
            set;
        }
        [Browsable(false)]
        [XmlIgnore]
        public int ValueCount
        {
            get
            {
                _valueCount = Size[1] * Size[2];
                return _valueCount;
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public object Tag
        {
            get;
            set;
        }

        [XmlElement]
        [Browsable(false)]
        public ParameterType VariableType
        {
            get;
            set;
        }

        [XmlElement]
        public int Dimension
        {
            get;
            set;
        }
        [XmlElement]
        public string[] DimensionNames
        {
            get;
            set;
        }
        [XmlIgnore]
        public string DimensionCat
        {
            get
            {
                return string.Join("_", DimensionNames);
            }
        }
        //[XmlIgnore]
        //public int[] DimensionLengh
        //{
        //    get
        //    {
        //        if (VariableType == ParameterType.Dimension || VariableType == ParameterType.Control)
        //        {
        //            return new int[] { 1 };
        //        }
        //        else
        //        {
        //            int[] lens = new int[Dimension];
        //            for (int i = 0; i < Dimension; i++)
        //            {
        //                int.Parse(Owner.Parameters[DimensionNames[i]].GetValue(0, 0, 0).ToString());
        //            }
        //            return lens;
        //        }
        //    }
        //}
        [XmlElement]
        public double DefaultValue
        {
            get;
            set;
        }
        [XmlElement]
        [Browsable(true)]
        public Modules ModuleName
        {
            get;
            set;
        }
        [XmlElement]
        [Browsable(true)]
        public float Maximum
        {
            get;
            set;
        }
        [XmlElement]
        [Browsable(true)]
        public float Minimum
        {
            get;
            set;
        }
        [XmlElement]
        [Browsable(true)]
        public string Units
        {
            get;
            set;
        }

        [XmlIgnore]
        [Browsable(false)]
        public bool CanEdit
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public DataCube2DLayout<float> FloatDataCube
        {
            get
            {
                _FloatDataCube = new DataCube2DLayout<float>(1, Size[1], Size[2]);
                _FloatDataCube.DataOwner = this;
                for (int i = 0; i < Size[1]; i++)
                {
                    for (int j = 0; j < Size[2]; j++)
                    {
                        _FloatDataCube[0, i, j] = float.Parse(this[0, i, j].ToString());
                    }
                }

                return _FloatDataCube;
            }
        }
        [XmlIgnore]
        [Browsable(true)]
        public bool IsFixed
        {
            get;
            set;
        }
        public Type GetVariableType()
        {
            Type type = null;
            switch (this.ValueType)
            {
                case 0:
                    type = typeof(short);
                    break;
                case 1:
                    type = typeof(int);
                    break;
                case 2:
                    type = typeof(float);
                    break;
                case 3:
                    type = typeof(double);
                    break;
                case 4:
                    type = typeof(string);
                    break;
                case 5:
                    type = typeof(object);
                    break;
                case 6:
                    type = typeof(bool);
                    break;
            }
            return type;
        }

        public override void FromDataTable(DataTable dt, int var_index, int time_index, int cell_index)
        {
            if (Dimension == 1)
            {
                for (int i = 0; i < Size[1]; i++)
                {
                    this[0, i, 0] = (T)dt.Rows[i][0];
                }
            }
            else if (Dimension == 2)
            {
                for (int i = 0; i < Size[1]; i++)
                {
                    for (int j = 0; j < Size[2]; j++)
                    {
                        this[0, i, j] = (T)dt.Rows[i][j];
                    }
                }
            }
        }
        public override DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            if (Dimension == 1)
            {
                DataColumn dc = new DataColumn(this.Name, typeof(T));
                dt.Columns.Add(dc);
                for (int i = 0; i < Size[1]; i++)
                {
                    var dr = dt.NewRow();
                    dr[0] = this[0, i, 0];
                    dt.Rows.Add(dr);
                }
            }
            else if (Dimension == 2)
            {
                for (int i = 0; i < Size[2]; i++)
                {
                    DataColumn dc = new DataColumn(DimensionNames[1] + (i + 1), typeof(T));
                    dt.Columns.Add(dc);
                }
                for (int i = 0; i < Size[1]; i++)
                {
                    var dr = dt.NewRow();
                    for (int j = 0; j < Size[2]; j++)
                    {
                        dr[j] = this[0, i, j];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        public virtual void AlterDimLength(string dim_name, int new_length)
        {
            int dim_index = GetDimIndex(dim_name);
            if (Size[dim_index + 1] == new_length)
            {
                return;
            }
            Size[dim_index + 1] = new_length;
            _arrays[0] = ILMath.zeros<T>(Size[1], Size[2]);
            Constant(DefaultValue);
        }

        public virtual void Constant(object vv)
        {
            var constant = TypeConverterEx.ChangeType<T>(vv);
            for (int i = 0; i < Size[1]; i++)
            {
                for (int j = 0; j < Size[2]; j++)
                {
                    _arrays[0][i, j] = constant;
                }
            }
        }
        public virtual void SetToDefault()
        {
            Constant(DefaultValue);
        }
        public virtual void UpdateFrom(IParameter new_para)
        {
            var vv = new_para as DataCubeParameter<T>;
            if (vv != null && this.SizeEquals(vv))
            {
                for (int i = 0; i < Size[1]; i++)
                {
                    for (int j = 0; j < Size[2]; j++)
                    {
                        _arrays[0][i, j] = vv[0][i, j];
                    }
                }
            }
        }
        public int GetDimIndex(string dim_name)
        {
            int dim_index = 0;
            for (int i = 0; i < Dimension; i++)
            {
                if (DimensionNames[i] == dim_name)
                {
                    dim_index = i;
                    break;
                }
            }
            return dim_index;
        }
        public object GetValue(int var_index, int time_index, int cell_index)
        {
            return this[var_index, time_index, cell_index];
        }

        public void SetValue(int var_index, int time_index, int cell_index, object new_value)
        {
            this[var_index, time_index, cell_index] = TypeConverterEx.ChangeType<T>(new_value);
        }

        public void FromStringArrays(string[] strs, int start, int end)
        {
            var buf = TypeConverterEx.ChangeType<T>(strs, start, end);
            if (Dimension == 1)
            {
                this[0][":", 0] = buf;
            }
            else if (Dimension == 2)
            {
                int k = 0;
                for (int i = 0; i < Size[2]; i++)
                {
                    for (int j = 0; j < Size[1]; j++)
                    {
                        this[0, j, i] = buf[k];
                        k++;
                    }
                }
            }
        }
        /// <summary>
        /// Column principal order 
        /// </summary>
        /// <returns></returns>
        public T[] ToVector()
        {
            if (Dimension == 1)
            {
                return this[0][":", 0].ToArray();
            }
            else if (Dimension == 2)
            {
                T[] vec = new T[ValueCount];
                int k = 0;
                for (int j = 0; j < Size[2]; j++)
                {
                    for (int i = 0; i < Size[1]; i++)
                    {
                        vec[k] = this[0, i, j];
                        k++;
                    }
                }
                return vec;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// get column vector
        /// </summary>
        /// <returns></returns>
        public float[] GetColumnVector(int col_index)
        {
            float[] vec = new float[Size[1]];
            for (int i = 0; i < vec.Length; i++)
            {
                vec[i] = float.Parse(this[0, i, col_index].ToString());
            }
            return vec;
        }
        public string[] ToStringVector()
        {
            var vec = ToVector();
            var vv = (from v in vec select v.ToString()).ToArray();
            return vv;
        }

        public void UpdateFromFloatDataCube()
        {
            if (_FloatDataCube != null)
            {
                for (int i = 0; i < Size[1]; i++)
                {
                    for (int j = 0; j < Size[2]; j++)
                    {
                        this[0, i, j] = TypeConverterEx.ChangeType<T>(_FloatDataCube[0, i, j]);
                    }
                }
            }
        }
    }
}
