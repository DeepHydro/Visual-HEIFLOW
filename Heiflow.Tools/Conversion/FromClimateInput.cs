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
    public class FromClimateInput : ModelTool
    {
        public FromClimateInput()
        {
            Name = "From Climate Input";
            Category = "Conversion";
            Description = "Load data cube from climate input data file";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";

        }

        [Category("Input")]
        [Description("The climate input data filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DataFileName
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The name of the output matrix")]
        public string OutputMatrix { get; set; }


        public override void Initialize()
        {
            Initialized = File.Exists(DataFileName);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            StreamReader sr = new StreamReader(DataFileName);
            string line = sr.ReadLine();
            line = sr.ReadLine();
            var buf = TypeConverterEx.Split<string>(line.Trim());
            int ncell = int.Parse(buf[1]);
            var var_name = buf[0];
            line = sr.ReadLine();
            int nstep = 0;
            int progress = 0;
            int count = 1;
            while(!sr.EndOfStream)
            {
                  line = sr.ReadLine();
                  if (!TypeConverterEx.IsNull(line))
                      nstep++;
            }
            sr.Close();

            var mat_out = new MyLazy3DMat<float>(1, nstep, ncell);
            mat_out.Name = OutputMatrix;
            mat_out.Variables = new string[] { var_name };
            sr = new StreamReader(DataFileName);
            mat_out.Allocate(0,nstep);
            mat_out.DateTimes = new DateTime[nstep];
            for (int i = 0; i < 3; i++)
            {
                sr.ReadLine();
            }

            for (int t = 0; t < nstep; t++)
            {
                line = sr.ReadLine();
                var vec= TypeConverterEx.SkipSplit<float>(line, 6);
                mat_out.Value[0][t] = vec;  
                var dd= TypeConverterEx.Split<int>(line, 3);
                mat_out.DateTimes [t]= new DateTime(dd[0], dd[1], dd[2]);
                progress = t * 100 / nstep;
                if (progress > count)
                {
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + (t + 1));
                    count++;
                }
            }
            Workspace.Add(mat_out);
            return true;
        }
    }
}