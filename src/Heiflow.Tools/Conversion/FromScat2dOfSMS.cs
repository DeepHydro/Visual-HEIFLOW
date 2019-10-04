using Heiflow.Core.Data;
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.Conversion
{
    public class FromScat2dOfSMS : ModelTool
    {
        public FromScat2dOfSMS()
        {
            Name = "From Scat2d of SMS";
            Category = "Conversion";
            Description = "Load data cube from scat2d file used by SMS";
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
        [Description("The name of the output data cube")]
        public string OutputDataCube { get; set; }


        public override void Initialize()
        {
            Initialized = File.Exists(DataFileName);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            StreamReader sr = new StreamReader(DataFileName);
            var line = sr.ReadLine();
            line = sr.ReadLine();
            var buf = TypeConverterEx.Split<string>(line);
            int ncell = int.Parse(buf[1]);
            int nvar = int.Parse(buf[3]);
            var variables = new string[nvar + 2];
            variables[0] = "x";
            variables[1] = "y";
            for (int i = 0; i < nvar; i++)
            {
                variables[i + 2] = buf[4 + i];
            }
            var mat_out = new DataCube<float>(nvar + 2, 1, ncell);
            mat_out.Name = OutputDataCube;
            mat_out.Variables = variables;
            for (int i = 0; i < ncell; i++)
            {
                line = sr.ReadLine().Trim();
                var vec = TypeConverterEx.Split<float>(line);
                for (int j = 0; j < nvar + 2; j++)
                {
                    mat_out[j, 0, i] = vec[j];
                }
            }
            sr.Close();
            Workspace.Add(mat_out);
            cancelProgressHandler.Progress("Package_Tool", 100, "Finished");
            return true;
        }
    }
}