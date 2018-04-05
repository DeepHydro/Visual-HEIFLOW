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

using DotSpatial.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.DataManagement
{
    public class AddRowColumn : ModelTool
    {
        //private string _ValueField;
        //private int _SelectedVarIndex = 0;
        private string _FeatureFileName;
        private IFeatureSet _FeatureSet;

        public AddRowColumn()
        {
            Name = "Add Row & Column";
            Category = "Data Management";
            Description = "Add row and column index  to the given grid shpfile";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The shpfile name")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string FeatureFileName
        {
            get
            {
                return _FeatureFileName;
            }
            set
            {
                _FeatureFileName = value;
                if (File.Exists(_FeatureFileName))
                {
                    _FeatureSet = FeatureSet.Open(_FeatureFileName);
                    var buf = from DataColumn dc in _FeatureSet.DataTable.Columns select dc.ColumnName;
                }
            }
        }

        public int RowCount
        {
            get;
            set;
        }

        public int ColumnCount
        {
            get;
            set;
        }

        public string SortingField
        {
            get;
            set;
        }

        public override void Initialize()
        {
            this.Initialized = _FeatureSet != null;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var dt = _FeatureSet.DataTable;
            var dv = new DataView(dt);
            dv.Sort = SortingField + " asc";        
          //  dt = dv.ToTable();
       
            string[] fields = new string[] { "Cell_ID", "Row", "Column" };
            double prg = 0;
            foreach (var str in fields)
            {
                if (dt.Columns.Contains(str))
                {
                    dt.Columns.Remove(str);
                }
                DataColumn dc = new DataColumn(str, Type.GetType("System.Int64")); 
                dt.Columns.Add(dc);
            }

            int index = 0;
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    var dr = dt.Rows[index];
                    dr["Cell_ID"] = index + 1;
                    dr["Row"] = i + 1;
                    dr["Column"] = j + 1;
                    index++;
                }
                prg = (i + 1) * 100.0 / RowCount;
                cancelProgressHandler.Progress("Package_Tool", (int)prg, "Processing Row: " + (i + 1));
            }
            _FeatureSet.Save();
            return true;
        }
    }
}
