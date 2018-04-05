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
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

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