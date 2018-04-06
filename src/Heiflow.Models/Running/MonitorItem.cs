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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Running
{
    public enum SequenceType { Accumulative, StepbyStep }
    public  class MonitorItem : IMonitorItem
    {
        protected string _Name;
        protected MonitorItem _Parent;

        
        public MonitorItem(string name)
        {
            _Name = name;
            Group = "Custom";
            Derivable = false;
            Children = new List<MonitorItem>();
            this.SequenceType = SequenceType.StepbyStep;
        }

        public string Name
        {
            get
            {
                return _Name;
            }
        }

        public int VariableIndex
        {
            get;
            set;
        }

        public bool Derivable
        {
            get;
            set;
        }

        public int[] DerivedIndex
        {
            get;
            set;
        }

        public string Group
        {
            get;
            set;
        }

        public List<MonitorItem> Children
        {
            get;
            set;
        }
        public SequenceType SequenceType
        {
            get;
            set;
        }

        public IFileMonitor Monitor
        {
            get;
            set;
        }

        public MonitorItem Parent
        {
            get
            {
                return _Parent;
            }
            set
            {
                _Parent = value;
           
                if(!_Parent.Children.Contains(this))
                {
                    _Parent.Children.Add(this);
                }
            }
        }


        public double[] DerivedValues
        {
            get;
            set;
        }

        public virtual double [] Derive(ListTimeSeries<double> source)
        {
            if (DerivedIndex != null)
            {
                double[] values = new double[source.Dates.Count];
                for (int i = 0; i < source.Dates.Count; i++)
                {
                    foreach(var index in DerivedIndex)
                    {
                        values[i] += source.Values[index][i];
                    }
                }
                return values;
            }
            else
            {
                return null;
            }
        }

        public virtual DataTable ToDataTable(ListTimeSeries<double> sourcedata)
        {
            DataTable dt = new DataTable();
            DataColumn date_col = new DataColumn("Date", Type.GetType("System.DateTime"));
            dt.Columns.Add(date_col);
            DataColumn value_col = new DataColumn(this.Name, Type.GetType("System.Double"));
            dt.Columns.Add(value_col);
            double[] buf = null;
            if (Derivable)
            {
                if (this.DerivedValues == null)
                    this.DerivedValues = this.Derive(sourcedata);
                buf = this.DerivedValues;
            }
            else
            {
                buf = sourcedata.Values[this.VariableIndex].ToArray();
            }
            for (int i = 0; i < buf.Length; i++)
            {
                var dr = dt.NewRow();
                dr[0] = sourcedata.Dates[i];
                dr[1] = buf[i];
                dt.Rows.Add(dr);
            }
            return dt;
        }


    }
}
