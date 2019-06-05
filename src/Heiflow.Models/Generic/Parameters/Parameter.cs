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

using Heiflow.Core.Data;
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
    /// <summary>
    /// 
    /// </summary>
    /// 
    [Serializable]
    public class Parameter : IParameter, IDataCubeObject
    {
        public event EventHandler DataCubeValueChanged;
        protected int _valueCount;
        protected int[] _Size;
        protected Array _array;   
        protected DataCube<float> _DataCube;
        public Parameter(string name)
        {
            Name = name;
            Init();
        }
        public Parameter()
        {
            Name = "Unknown";
            Init();
        }
        //public Parameter()
        //{
        //    Name = "Unknown";
        //    VariableType = ParameterType.Parameter;
        //    Description = "";
        //    ModuleName = Modules.basin;
        //    CanEdit = true;
        //    DataCubeType = Core.Data.DataCubeType.Vector;
        //    SupportTableEdit = true;
        //}

        [XmlElement]
        public string Name
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
        [Browsable(false)]
        [XmlIgnore]
        public ParameterDimension ParameterDimension
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public string OwnerName
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public bool SupportGridLayout
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
            get { return this._Size; }
        }
        [XmlIgnore]
        [Browsable(false)]
        public virtual Array ArrayObject
        {
            get { return _array; }
        }
        [XmlIgnore]
        [Browsable(false)]
        public DataCube<float> DataCubeObject
        {
            get
            {
                return _DataCube;
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public bool TimeBrowsable
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public TimeVarientFlag[,] Flags
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public float[,] Constants
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public float[,] Multipliers
        {
            get;
            set;
        }

        [XmlIgnore]
        [Browsable(false)]
        public IGridTopology Topology
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public DataCubeType DataCubeType
        {
            get;
            protected set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public bool AllowTableEdit
        {
            get;
            protected set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public string[] Variables
        {
            get
            {
                return new string[] { Name };
            }
            set
            {
                Name = value[0];
            }
        }
        [XmlIgnore]
        [Browsable(false)]
        public object DataOwner
        {
            get;
            set;
        }
        [XmlIgnore]
        [Browsable(false)]
        public DataCubeLayout Layout
        {
            get;
            set;
        }
        protected virtual void Init()
        {
            VariableType = ParameterType.Parameter;
            Description = "";
            ModuleName = Modules.basin;
            CanEdit = true;
            TimeBrowsable = false;
            DataCubeType = Core.Data.DataCubeType.Vector;
            AllowTableEdit = true;
            _Size = new int[] { 1, 1, 1 };
            Multipliers = new float[1, 1];
            Constants = new float[1, 1];
            Flags = new TimeVarientFlag[1, 1];
            Multipliers[0, 0] = 1;
            Constants[0, 0] = 1;
            Flags[0, 0] = TimeVarientFlag.Individual;
            Maximum = 1;
            Minimum = 0;
            Units = "";
        }
        public virtual IEnumerable<double> ToDouble()
        {
            return null;
        }

        public virtual IEnumerable<float> ToFloat()
        {
            return null;
        }
        public virtual IEnumerable<int> ToInt32()
        {
            return null;
        }

        public virtual string[] ToStrings()
        {
            return null;
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
        public string SizeString()
        {
            return string.Format("[{0}][{1}][{2}]", _Size[0], _Size[1], _Size[2]);
        }

        public virtual void FromDataTable(System.Data.DataTable dt)
        {

        }

        public DataTable ToDataTable(int time_index)
        {
            return new DataTable();
        }

        public T[] Get<T>(int var_index, int time_index)
        {
            return null;
        }


        public virtual void AlterDimLength(int new_length)
        {

        }


        public virtual void SetValue(object vv, int index)
        {
            
        }
        public virtual void SetValues<T>(T[] vv)
        {

        }
        public virtual void Constant(object vv)
        {

        }
        public virtual void ResetToDefault()
        {

        }
        public virtual void UpdateFrom(IParameter new_para)
        {
           
        }

       
        public virtual Array GetByTime(int var_index, int time_index)
        {
            return null;
        }

        public virtual Array GetBySpace(int var_index, int space_index)
        {
            return null;
        }

        public virtual DataTable ToDataTableByTime(int var_index, int time_index)
        {
            return new DataTable();
        }

        public virtual DataTable ToDataTableBySpace(int var_index, int space_index)
        {
            return new DataTable();
        }

        public virtual DataTable ToDataTable()
        {
            return new DataTable();
        }

        public virtual Array GetSerialArrayByTime(int p1, int p2)
        {
            return null;
        }

        public virtual Array GetRegularlArrayByTime(int p1, int p2)
        {
            return null;
        }

        public virtual void FromRegularArray(int p1, int p2, Array array)
        {
          
        }

        public virtual void FromSerialArray(int p1, int p2, Array array)
        {
           
        }

        public virtual void AllocateVariable(int var_index, int ntime, int ncell)
        {
        
        }

        public ILNumerics.ILBaseArray ToILBaseArray(int var_index, int time_index)
        {
            return null;
        }

        public bool IsAllocated(int var_index)
        {
            return true;
        }
        public virtual int GetSpaceDimLength(int var_index, int time_index)
        {
            return 0;
        }
        public virtual void OnDataCubeValueChanged()
        {
            if (DataCubeValueChanged != null)
            {
                DataCubeValueChanged(this, EventArgs.Empty);
            }
        }
    }
}
