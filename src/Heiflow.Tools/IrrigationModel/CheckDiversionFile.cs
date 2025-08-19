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
