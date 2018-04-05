// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Data;
using Heiflow.Core.Data;
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
    public class SetCanalParameters : ModelTool
    {
        private string _DiversionFileName;
        private string _CanalFileName;

        public SetCanalParameters()
        {
            Name = "Set Canal Parameters";
            Category = "Irrigation Model";
            Description = "Set Canal Parameters";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("The diversion filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DiversionFileName
        {
            get
            {
                return _DiversionFileName;
            }
            set
            {
                _DiversionFileName = value;
            }
        }

        [Category("Input")]
        [Description("The canal filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string CanalFileName
        {
            get
            {
                return _CanalFileName;
            }
            set
            {
                _CanalFileName = value;
            }
        }

        public override void Initialize()
        {
            this.Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int nseg = 0;
            string div_out = DiversionFileName + ".out";
            StreamReader sr_div = new StreamReader(DiversionFileName);
            StreamReader sr_canal = new StreamReader(CanalFileName);
            StreamWriter sw_div_out = new StreamWriter(div_out);

            var line = sr_div.ReadLine();
            sw_div_out.WriteLine(line);
            line = sr_div.ReadLine();
            sw_div_out.WriteLine(line);
            var div_buf = TypeConverterEx.Split<string>(line);
            nseg = int.Parse(div_buf[0]);

            sr_canal.ReadLine();

            for (int i = 0; i < nseg; i++)
            {
                line = sr_div.ReadLine();
                sw_div_out.WriteLine(line);
                div_buf = TypeConverterEx.Split<string>(line);
                int nhru = int.Parse(div_buf[1]);

                if (nhru > 0)
                {
                    line = sr_div.ReadLine();
                    sw_div_out.WriteLine(line);
                    line = sr_div.ReadLine();
                    sw_div_out.WriteLine(line);
                    sr_div.ReadLine();
                    sr_div.ReadLine();

                    var line_canal = sr_canal.ReadLine();
                    var buf_canal = TypeConverterEx.Split<string>(line_canal);
                    float area_ratio = float.Parse(buf_canal[1]);
                    float effcy_ratio = float.Parse(buf_canal[2]);
                    var vec_area_ratio = new float[nhru];
                    var vec_effcy_ratio = new float[nhru];
                    for (int j = 0; j < nhru; j++)
                    {
                        vec_area_ratio[j] = area_ratio;
                        vec_effcy_ratio[j] = effcy_ratio;
                    }

                    line = string.Join("\t", vec_effcy_ratio);
                    sw_div_out.WriteLine(line);
                    line = string.Join("\t", vec_area_ratio);
                    sw_div_out.WriteLine(line);
                }
            }
            while (!sr_div.EndOfStream)
            {
                line = sr_div.ReadLine();
                sw_div_out.WriteLine(line);
            }

            sr_div.Close();
            sr_canal.Close();
            sw_div_out.Close();
            cancelProgressHandler.Progress("Package_Tool", 100, "Done");
            return true;
        }
    }
}
