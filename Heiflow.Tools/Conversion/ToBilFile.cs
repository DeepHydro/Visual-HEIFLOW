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
    public class ToBilFile : ModelTool
    {
        public ToBilFile()
        {
            Name = "Save As Bil File";
            Category = "Conversion";
            Description = "Save data cube as bil file";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The name of the input matrix")]
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
            FileStream fs = new FileStream(OutputFileName, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            StreamWriter sw = new StreamWriter(OutputFileName + ".hdr");
            var var_index = 0;
            var mat = Get3DMat(Source, ref var_index);
            int progress = 0;
            int nsteps = mat.Size[1];
            var grid = ProjectService.Project.Model.Grid as RegularGrid;
            var lonlat = grid.GetLonLatAxis();
            var nrow = grid.RowCount;
            var ncol = grid.ColumnCount;
            var times = new float[nsteps];
            if (mat.DateTimes != null)
            {
                for (int t = 0; t < nsteps; t++)
                {
                    times[t] = (float) mat.DateTimes[t].ToOADate();
                }
            }
            else
            {
                for (int t = 0; t < nsteps; t++)
                {
                    times[t] = (float)DateTime.Now.AddDays(t).ToOADate();
                }
            }

            sw.WriteLine(ncol + "," + nrow + "," + nsteps);
            var timestr = string.Join("\n", times);
            sw.WriteLine(timestr);
            sw.Close();

            for (int t = 0; t < nsteps; t++)
            {
                var mat_step = grid.ToMatrix<float>(mat.Value[var_index][t], 0);
                for (int r = 0; r < nrow; r++)
                {
                    for (int c = 0; c < ncol; c++)
                    {
                        //var bb= (byte) mat_step[r][c];
                        //byte hi = (byte)(bb >> 8);
                        //byte low = (byte)(bb - (short)(hi << 8));
                        var a = (int)mat_step[r][c];
                        byte hi = (byte)(a >> 8);
                        byte low = (byte)(a - (a << 8));

                        bw.Write(low);
                        bw.Write(hi);
                    }
                }
                progress = t * 100 / nsteps;
                cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + t);
            }

            sw.Close();
            fs.Close();
            bw.Close();
            return true;
        }
    }
}