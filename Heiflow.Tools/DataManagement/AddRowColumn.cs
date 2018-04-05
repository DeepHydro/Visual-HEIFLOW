// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
