using System;
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
    public class DataCubeParameter<T>:DataCube<T>,IParameter
    {
        protected int _valueCount;

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
            TimeBrowsable = false;
            DataCubeType = Core.Data.DataCubeType.Vector;
            AllowTableEdit = true;
            Maximum = 1;
            Minimum = 0;
            Units = "";
        }
        [XmlElement]
        public new string Name
        {
            get;
            set;
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
        [Browsable(false)]
        public string[] DimensionNames
        {
            get;
            set;
        }
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
        public override void FromDataTable(System.Data.DataTable dt)
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
                var nrow = (int)this.Owner.Parameters[DimensionNames[0]].GetValue(0, 0, 0);
                var ncol = (int)this.Owner.Parameters[DimensionNames[1]].GetValue(0, 0, 0);
                for (int i = 0; i < nrow; i++)
                {
                    for (int j = 0; j < ncol; j++)
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
                var nrow = (int) this.Owner.Parameters[DimensionNames[0]].GetValue(0, 0, 0);
                var ncol = (int)this.Owner.Parameters[DimensionNames[1]].GetValue(0, 0, 0);
                for (int i = 0; i < ncol; i++)
                {
                    DataColumn dc = new DataColumn(DimensionNames[1] + (i + 1), typeof(T));
                    dt.Columns.Add(dc);
                }
                for (int i = 0; i < nrow; i++)
                {
                    var dr = dt.NewRow();
                    for (int j = 0; j < ncol; j++)
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
            DataCubeType = DataCubeType.General;
            Constant(DefaultValue);
        }

        public virtual void Constant(object vv)
        {
            var constant = (T)vv;
            for (int i = 0; i < Size[1]; i++)
            {
                for (int j = 0; j < Size[2]; j++)
                {
                    _arrays[0][i, j] = constant;
                }
            }
        }
        public virtual void ResetToDefault()
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
            this[var_index, time_index, cell_index] = (T)new_value;
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
                        this[0][j, i] = buf[k];
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
        public IEnumerable<double> ToDouble()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<float> ToFloat()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> ToInt32()
        {
            throw new NotImplementedException();
        }

        public string[] ToStrings()
        {
            var vec = ToVector();
            var vv = (from v in vec select v.ToString()).ToArray();
            return vv;
        }

        public void AlterDimLength(int new_length)
        {
            throw new NotImplementedException();
        }

        public void SetValue(object vv, int index)
        {
            throw new NotImplementedException();
        }

        public void SetValues<T1>(T1[] vv)
        {
            throw new NotImplementedException();
        }


        public ParameterDimension ParameterDimension
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DataCube<float> DataCubeObject
        {
            get { throw new NotImplementedException(); }
        }
    }
}
