﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using ILNumerics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic.Parameters
{
    [Serializable]
    public class SingleParam<T>:Parameter
    {
        private T _value;
        public SingleParam(string name)
            : base(name)
        {
            _valueCount = 1;
            ParameterDimension = Generic.ParameterDimension.Single;
            _array = new T[1];
        }

        [Browsable(false)]
        [XmlIgnore]
        public T Value 
        { 
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                _array.SetValue(_value, 0);
                if (ValueType < 4)
                {
                    _DataCube = new Core.Data.My3DMat<float>(1, 1, 1);
                    _DataCube.Value[0][0][0] = float.Parse(_value.ToString());
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
                return 1;
            }
        }

        public  void SetValue(T value)
        {
            Value = value;
        }

        public override void ResetToDefault()
        {
            Value = (T)Convert.ChangeType(DefaultValue, typeof(T)); ;
        }
        public override IEnumerable<double> ToDouble()
        {
            return new double[] { double.Parse(Value.ToString()) }; 
        }

        public override IEnumerable<float> ToFloat()
        {
            return new float[] { float.Parse(Value.ToString()) };
        }

        public override IEnumerable<int> ToInt32()
        {
            return new int[] { int.Parse(Value.ToString()) };
        }

        public override string[] ToStrings()
        {
            return new string[] { Value.ToString() };
        }

        public override void AlterDimLength(int new_length)
        {
            
        }

        public override void SetValue(object vv, int index)
        {
            Value = (T)Convert.ChangeType(vv, typeof(T));
        }

        public override void Constant(object vv)
        {
            Value = (T)vv;
        }
        public override void UpdateFrom(IParameter new_para)
        {
            var vv = new_para as SingleParam<T>;
            if(vv != null)
                this.Value = vv.Value;
        }
        public override void FromDataTable(System.Data.DataTable dt)
        {
            Value = (T)dt.Rows[0][0];
        }
        public override System.Data.DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn(Name, typeof(T));
            dt.Columns.Add(dc);

            var dr = dt.NewRow();
            dr[0] = Value;
            dt.Rows.Add(dr);

            return dt;
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
            return new T[] { Value };
        }

        public override Array GetByTime(int var_index, int time_index)
        {
            return new T[] { Value };
        }



        public override int GetSpaceDimLength(int var_index, int time_index)
        {
            return 1;
        }
        public override void FromSerialArray(int p1, int p2, Array array)
        {
            this.Value = (T)array.GetValue(0, 0);
        }

        public override Array GetSerialArrayByTime(int p1, int p2)
        {
            T[,] array = new T[1, 1];
            array[0, 0] = Value;
            return array;
        }
    }
}