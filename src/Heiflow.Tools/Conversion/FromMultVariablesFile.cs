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
    public class FromMultVariablesFile : ModelTool
    {
        public FromMultVariablesFile()
        {
            Name = "From Multiple Variable Text File";
            Category = "Conversion";
            Description = "Load data cube from a text file. Each line stores values of multiple variables at  single location";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            ContainsHeader = true;
            ContainsDateTime = true;
            OutputMatrix = "dc";
            Interval = 86400;
            Start = DateTime.Now;
        }

        [Category("Input")]
        [Description("The text  filename")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string DataFileName
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("True indicates that the first line is header")]
        public bool ContainsHeader
        {
            get;
            set;
        }
        [Category("Input")]
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
            var date = DateTime.Now;
            string[] vars = null;
            StreamReader sr = new StreamReader(DataFileName);
            if (ContainsHeader)
            {
                line = sr.ReadLine();
                vars = TypeConverterEx.Split<string>(line.Trim());
            }
            line = sr.ReadLine();
            var buf = TypeConverterEx.Split<string>(line.Trim());
            if (ContainsDateTime)
            {
                buf = TypeConverterEx.SkipSplit<string>(line.Trim(), 1);
            }
            int nvar = buf.Length;
            int nstep = 1;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (!TypeConverterEx.IsNull(line))
                    nstep++;
            }
            sr.Close();
            if (!ContainsHeader)
            {
                vars = new string[nvar];
                for (int i = 0; i < nvar; i++)
                {
                    vars[i] = "var" + i;
                }
            }

            sr = new StreamReader(DataFileName);
            string var_name = Path.GetFileNameWithoutExtension(DataFileName);
            var mat_out = new DataCube<float>(nvar, nstep, 1, false);
            mat_out.Name = OutputMatrix;
            mat_out.AllowTableEdit = false;
            mat_out.TimeBrowsable = true;
            mat_out.Variables = vars;
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
                    for (int i = 0; i < nvar; i++)
                    {
                        float.TryParse(strs[i + 1], out temp);
                        mat_out[i,t,0] = temp;
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
                    for (int i = 0; i < nvar; i++)
                    {
                        mat_out[i,t,0] = vec[i];
                    }
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