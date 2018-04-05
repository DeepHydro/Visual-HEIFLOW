// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heiflow.Core.Data;
using System.IO;
using Heiflow.Models.Generic;
using System.ComponentModel;
using Heiflow.Controls.WinForm.Editors;
using System.Windows.Forms.Design;
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;

namespace Heiflow.Tools.Conversion
{
    public class ToTextFile : ModelTool
    {
        public ToTextFile()
        {
            Name = "Save As Text File";
            Category = "Conversion";
            Description = "Save data cube as txt file";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The name of the input matrix. The matrix style should be mat[0][:][:]")]
        public string Source { get; set; }


        [Category("Output")]
        [Description("The output filename")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OutputFileName
        {
            get;
            set;
        }


        public override void Initialize()
        {
            this.Initialized = Validate(Source);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var var_index = 0;
            var mat = Get3DMat(Source, ref var_index);
            int progress = 0;
            int nstep = mat.Size[1];
            StreamWriter sw = new StreamWriter(OutputFileName);
            string line = "";
            for (int t = 0; t < nstep; t++)
            {
                var vec = mat.Value[var_index][t];
                line = string.Join("\t", vec);
                sw.WriteLine(line);
                progress = t * 100 / nstep;
                cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + t);
            }
            sw.Close();
            return true;
        }
    }
}