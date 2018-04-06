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
