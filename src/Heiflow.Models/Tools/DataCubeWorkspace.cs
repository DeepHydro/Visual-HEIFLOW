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
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Tools
{
    public class DataCubeWorkspace : Heiflow.Models.Tools.IDataCubeWorkspace
    {
        private int _mat_counter = 0;
        public event EventHandler DataSourceCollectionChanged;
        public DataCubeWorkspace()
        {
            DataSources = new ObservableCollection<IDataCubeObject>();
        }

        public ObservableCollection<IDataCubeObject> DataSources { get; protected set; }

        public void Add(IDataCubeObject mat)
        {
            if (TypeConverterEx.IsNull(mat.Name))
            {
                mat.Name = GetName();
            }
            var buf = from mm in DataSources where mm.Name == mat.Name select mm;
            if (buf.Any())
            {
                Remove(mat.Name);
            }
            DataSources.Add(mat);
            OnDataSourceCollectionChanged();
    }

        public IDataCubeObject Get(string name)
        {
            var buf = from mm in DataSources where mm.Name == name select mm;
            if (buf.Any())
            {
                var mat_old = buf.First();
                return buf.First();
            }
            else
            {
                return null;
            }
        }

        public void Remove(string name)
        {
            var buf = from mm in DataSources where mm.Name == name select mm;
            if (buf.Any())
            {
                var mat_old = buf.First();
                DataSources.Remove(mat_old);
                mat_old = null;
                OnDataSourceCollectionChanged();
            }
        }

        public bool Contains(string name)
        {
            var buf = from mm in DataSources where mm.Name == name select mm;
            if (buf.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            DataSources.Clear();
            OnDataSourceCollectionChanged();
        }

        private string GetName()
        {
            string name = "Mat" + _mat_counter;
            _mat_counter++;
            return name;
        }


        public DataTable ToDataTable()
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("ID", Type.GetType("System.Int32"));
            dt.Columns.Add(dc);
            dc = new DataColumn("ParentID", Type.GetType("System.Int32"));
            dt.Columns.Add(dc);
            dc = new DataColumn("Name", Type.GetType("System.String"));
            dt.Columns.Add(dc);
            dc = new DataColumn("Size", Type.GetType("System.String"));
            dt.Columns.Add(dc);
            dc = new DataColumn("Owner", Type.GetType("System.String"));
            dt.Columns.Add(dc);
            dc = new DataColumn("ImageName", Type.GetType("System.String"));
            dt.Columns.Add(dc);
            dc = new DataColumn("DataCubeObject", typeof(IDataCubeObject));
            dt.Columns.Add(dc);
            int i = 100;
            foreach (var ds in DataSources)
            {
                int j = 0;
                DataRow dr = dt.NewRow();
                dr[0] = i;
                dr[1] = 9999;
                dr[2] = ds.Name;
                dr[3] = ds.SizeString();
                dr[4] = ds.OwnerName;
                dr[5] = "ready";
                dr[6] = ds;
                dt.Rows.Add(dr);
                
                foreach (var vv in ds.Variables)
                {
                    DataRow dr1 = dt.NewRow();
                    dr1[0] = j;
                    dr1[1] = i;
                    dr1[2] = vv;
                    dr1[3] = string.Format("[1][{0}][{1}]", ds.Size[1], ds.Size[2]);// TODO
                    dr1[4] = ds.OwnerName;
                    dr1[5] = ds.IsAllocated(j) ? "ready" : "standby";
                    dr1[6] = ds;
                    dt.Rows.Add(dr1);
                    j++;
                }
                i++;
            }
            return dt;
        }

        protected void OnDataSourceCollectionChanged()
        {
            if (DataSourceCollectionChanged != null)
                DataSourceCollectionChanged(this, EventArgs.Empty);
        }
    }
}