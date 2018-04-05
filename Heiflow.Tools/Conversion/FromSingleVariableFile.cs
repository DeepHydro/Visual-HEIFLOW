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
    public class FromSingleVariableFile : ModelTool
    {
        public FromSingleVariableFile()
        {
            Name = "From Single Variable Text File";
            Category = "Conversion";
            Description = "Load data cube from a text file. Each line stores values of the variable at  multiple locations";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            ContainsHeader = false;
            ContainsDateTime = false;
            OutputMatrix = "dc";
            Interval = 86400;
            Start = DateTime.Now;
        }

        [Category("Input")]
        [Description("The text filename.")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DataFileName
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("True indicates that the first line is header")]
        public bool ContainsHeader
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("True indicates that the first column represents DateTime")]
        public bool ContainsDateTime
        {
            get;
            set;
        }
        [Category("Input")]
        [Description("The name of the output matrix")]
        public string OutputMatrix { get; set; }

        [Category("Optional")]
        [Description("The start date time of the time series if date time column is not provided.")]
        public DateTime Start { get; set; }

        [Category("Optional")]
        [Description("The time step in seconds")]
        public int Interval { get; set; }

        public override void Initialize()
        {
            Initialized = File.Exists(DataFileName);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int progress = 0;
            string line = "";
            float temp = 0;
            int count = 1;
            int nstep = 0;
            int ncell = 0;
            var date = DateTime.Now;
            StreamReader sr = new StreamReader(DataFileName);
            if (ContainsHeader)
                line = sr.ReadLine();
            line = sr.ReadLine();
            var buf = TypeConverterEx.Split<string>(line.Trim());
            if (ContainsDateTime)
            {
                buf = TypeConverterEx.SkipSplit<string>(line.Trim(), 1);
            }
            ncell = buf.Length;
            nstep++;

            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (!TypeConverterEx.IsNull(line))
                    nstep++;
            }
            sr.Close();

            sr = new StreamReader(DataFileName);
            string var_name = Path.GetFileNameWithoutExtension(DataFileName);
            var mat_out = new MyLazy3DMat<float>(1, nstep, ncell);
            mat_out.Name = OutputMatrix;
            mat_out.AllowTableEdit = false;
            mat_out.TimeBrowsable = true;
            mat_out.Variables = new string[] { var_name };
            mat_out.Allocate(0, nstep);
            mat_out.DateTimes = new DateTime[nstep];
            if (ContainsHeader)
                line = sr.ReadLine();
            if (ContainsDateTime)
            {
                for (int t = 0; t < nstep; t++)
                {
                    line = sr.ReadLine();
                    var strs = TypeConverterEx.Split<string>(line);
                    DateTime.TryParse(strs[0], out date);
                    mat_out.DateTimes[t] = date;
                    for (int i = 0; i < ncell; i++)
                    {
                        float.TryParse(strs[i + 1], out temp);
                        mat_out.Value[0][t][i] = temp;
                    }
                    if (progress > count)
                    {
                        progress = t * 100 / nstep;
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + (t + 1));
                    }
                }
            }
            else
            {
                for (int t = 0; t < nstep; t++)
                {
                    line = sr.ReadLine();
                    var vec = TypeConverterEx.Split<float>(line);
                    mat_out.Value[0][t] = vec;
                    mat_out.DateTimes[t] = Start.AddSeconds(Interval * t);
                    if (progress > count)
                    {
                        progress = t * 100 / nstep;
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + (t + 1));
                    }
                }
            }
            Workspace.Add(mat_out);
            return true;
        }
    }
}