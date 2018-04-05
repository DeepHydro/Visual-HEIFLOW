// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Tools;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.Statisitcs
{
    public class ShowOnFeatureLayer : ModelTool
    {
        private string _ValueField;
        private int _SelectedVarIndex = 0;
        private string _FeatureFileName;
        private IFeatureSet _FeatureSet;

        public ShowOnFeatureLayer()
        {
            Name = "Show on Layer";
            Category = "Visualization";
            Description = " ";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = false;
        }

        [Category("Input")]
        [Description("The matrix that is to be shown on the map")]
        public string Matrix
        {
            get;
            set;
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
                if(File.Exists(_FeatureFileName))
                {
                    _FeatureSet = FeatureSet.Open(_FeatureFileName);
                    var buf = from DataColumn dc in _FeatureSet.DataTable.Columns select dc.ColumnName;
                    Fields = buf.ToArray();
                }
            }
        }

        [Category("Parameter")]
        [Description("Name of the variable")]
        [EditorAttribute(typeof(StringDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        [DropdownListSource("Fields")]
        public string ValueField
        {
            get
            {
                return _ValueField;
            }
            set
            {
                _ValueField = value;
                if (Fields != null)
                {
                    for (int i = 0; i < Fields.Length; i++)
                    {
                        if (_ValueField == Fields[i])
                        {
                            _SelectedVarIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        [Browsable(false)]
        public string[] Fields
        {
            get;
            protected set;
        }

        public override void Initialize()
        {
            this.Initialized = Validate(Matrix) && _FeatureSet != null && !TypeConverterEx.IsNull(_ValueField);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var vector = GetVector(Matrix);
            if (vector != null)
            {
                cancelProgressHandler.Progress("Package_Tool", 10, "Calculating...");
                var dt = _FeatureSet.DataTable;
                var type = dt.Columns[_SelectedVarIndex].DataType;
                if (vector.Length == dt.Rows.Count)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i][ValueField] = Convert.ChangeType((vector[i]), type);
                    }
                }
                cancelProgressHandler.Progress("Package_Tool", 80, "Saving...");
                _FeatureSet.Save();
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
