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
    public class CheckDiversionFile : ModelTool
    {
        private string _DiversionFileName;
        //private string _CanalFileName;

        public CheckDiversionFile()
        {
            Name = "Check Diversion File";
            Category = "Irrigation Model";
            Description = "Check Diversion File";
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

     
        public override void Initialize()
        {
            this.Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int nseg = 0;
            string div_out = DiversionFileName + "_report.out";
            StreamReader sr_div = new StreamReader(DiversionFileName);
            StreamWriter sw_div_out = new StreamWriter(div_out);

            var line = sr_div.ReadLine();
            line = sr_div.ReadLine();
            var div_buf = TypeConverterEx.Split<string>(line);
            nseg = int.Parse(div_buf[0]);

            sw_div_out.WriteLine("HRU_ID\tNHRU\tSumOfRatio");
            for (int i = 0; i < nseg; i++)
            {
                line = sr_div.ReadLine();     
                div_buf = TypeConverterEx.Split<string>(line);
                int hru_id = int.Parse(div_buf[0]);
                int nhru = int.Parse(div_buf[1]);
                if (nhru > 0)
                {
                    line = sr_div.ReadLine();
                    line = sr_div.ReadLine();
                    var buf = TypeConverterEx.Split<double>(line);
                    var sum = buf.Sum();
                    sr_div.ReadLine();
                    sr_div.ReadLine();
                    var newline = string.Format("{0}\t{1}\t{2}", hru_id, nhru, sum);
                    sw_div_out.WriteLine(newline);
                }
                else
                {
                    var newline = string.Format("{0}\t{1}\t{2}", hru_id, nhru, 0);
                    sw_div_out.WriteLine(newline);
                }
            }
            sr_div.Close();
            sw_div_out.Close();
            cancelProgressHandler.Progress("Package_Tool", 100, "Done");
            return true;
        }
    }
}
