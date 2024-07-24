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
using Microsoft.Research.Science.Data.NetCDF4;
using Heiflow.Core.MyMath;
using Heiflow.Core.Data.Classification;
using Newtonsoft.Json;

namespace Heiflow.Tools.Conversion
{
    public class ToJson : ModelTool
    {
        private string _OutputFileName;

        public ToJson()
        {
            Name = "To Json File";
            Category = "Conversion";
            Description = "Save data cube as Json file";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            MultiThreadRequired = true;
            NumBreaks = 10;
            IntervalMethod = Core.Data.Classification.IntervalMethod.EqualInterval;
        }

        [Category("Input")]
        [Description("The name of the input matrix")]
        public string Source { get; set; }

        [Category("Input")]
        [Description("The number of breaks")]
        public int NumBreaks { get; set; }
        public IntervalMethod IntervalMethod
        {
            get;
            set;
        }


        [Category("Output")]
        [Description("The output filename")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OutputFileName
        {
            get
            {
                return _OutputFileName;
            }
            set
            {
                _OutputFileName = value;
            }
        }



        public override void Initialize()
        {
            this.Initialized = Validate(Source);
            if (ProjectService == null)
                this.Initialized = false;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var var_index = 0;
            var mat = Get3DMat(Source, ref var_index);
            int progress;
            int nsteps = mat.Size[1];
            var times = new float[nsteps];
            List<int[]> lists = new List<int[]>();

            if (mat.DateTimes != null)
            {
                for (int t = 0; t < nsteps; t++)
                {
                    times[t] = (float)mat.DateTimes[t].ToOADate();
                }
            }
            else
            {
                for (int t = 0; t < nsteps; t++)
                {
                    times[t] = (float)DateTime.Now.AddDays(t).ToOADate();
                }
            }
            if (IntervalMethod == Core.Data.Classification.IntervalMethod.NaturalBreaks)
            {
                var breaks=new List<float>();
                for (int t = 0; t < nsteps; t++)
                {
                    var vec = mat[var_index, t.ToString(), ":"];
                    var jki = JenksFisher.CreateJenksFisherIndex(vec.ToList(), NumBreaks,ref breaks);
                    lists.Add(jki);
                    progress = t * 100 / nsteps;
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + t);
                }
            }
            else
            {
                var scheme = new Scheme();
                scheme.EditorSettings.NumBreaks = NumBreaks;
                scheme.EditorSettings.IntervalMethod = IntervalMethod;
         
                for (int t = 0; t < nsteps; t++)
                {
                    var vec = mat[var_index, t.ToString(), ":"];
                    var veccopy = Array.ConvertAll(vec, x => (double)x);
                    Array.Sort(veccopy);
                    scheme.Values = veccopy.ToList();
                    scheme.CreateBreakCategories();
                    var index = scheme.GetBreakIndex(vec);

                    lists.Add(index);
                    progress = t * 100 / nsteps;
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing step:" + t);
                }
            }
            string json = JsonConvert.SerializeObject(lists, Formatting.None);
            File.WriteAllText(_OutputFileName, json);
            return true;
        }
    }
}