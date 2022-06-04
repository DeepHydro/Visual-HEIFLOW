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

using Heiflow.Controls.WinForm.Editors;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Tools;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using System.IO;
using Heiflow.Presentation.Services;
using Heiflow.Applications;

namespace Heiflow.Tools.Preprocessing
{
    public class CreateGwhAssimFile : ModelTool
    {
        public CreateGwhAssimFile()
        {
            Name = "Create GW Head Assimilation File";
            Category = "Preprocessing";
            Description = "Create GW Head Assimilation File";
            MultiThreadRequired = true;
            Start = new DateTime(2010, 1, 1);
            End = new DateTime(2021, 12, 31);
            VariableID = 65;
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }
        [Category("Input")]
        [Description("The filename of site id")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string SiteIDFileName
        {
            get;
            set;
        }
        [Category("Input")]
        [Description("The filename of output matrix")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OutputFileName
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("The varialbe ID")]
        public int VariableID
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("The start")]
        public DateTime Start
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("The end datetime")]
        public DateTime End
        {
            get;
            set;
        }

        public override void Initialize()
        {

            this.Initialized = true;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            StreamReader sr = new StreamReader(SiteIDFileName);
            List<int> siteid_list = new List<int>();
            var projectService = MyAppManager.Instance.CompositionContainer.GetExportedValue<IProjectService>();
            var ODM = projectService.Project.ODMSource;
            if (ODM != null)
            {
                var line = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (TypeConverterEx.IsNotNull(line))
                    {
                        var bufs = TypeConverterEx.Split<int>(line);
                        siteid_list.Add(bufs[0]);
                    }
                }
                var dates = new List<DateTime>();
                var days = new int[] { 1, 6, 11, 16, 21, 26 };
                for (int i = Start.Year; i <= End.Year; i++)
                {
                    for (int j = 1; j < 13; j++)
                    {
                        for (int k = 0; k < days.Length; k++)
                        {
                            var date = new DateTime(i, j, days[k]);
                            dates.Add(date);
                        }
                    }
                }
                int nsite = siteid_list.Count;
                int nday = dates.Count;
                float[][] mat = new float[nsite][];
                string sql="";
                int progress = 0;
                for (int i = 0; i < nsite; i++)
                {
                    mat[i] = new float[nday];
                    for (int j = 0; j < nday; j++)
                    {
                        sql = string.Format("select DateTimeUTC, DataValue from DataValues where SiteID={0} and VariableID={1} and DateTimeUTC=#{2}# ",
                siteid_list[i], VariableID, dates[j].ToString("yyyy/MM/dd"));
                        var dt = ODM.Execute(sql);
                        if(dt != null)
                        {
                            mat[i][j] = float.Parse(dt.Rows[0][1].ToString());
                        }
                        else
                        {
                            mat[i][j] = -9999;
                        }
                    }
                    progress = (i + 1) / nsite * 100;
                    cancelProgressHandler.Progress("Package_Tool", progress, "Processing Site: " + (i + 1));
                }

                StreamWriter sw = new StreamWriter(OutputFileName);
                for (int i = 0; i < nsite; i++)
                {
                    line = string.Join(" ", mat[i]);
                    sw.WriteLine(line);
                }
                sw.Close();
                sr.Close();
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Failed. The ODM database is not connected.");
            }
            return true;

        }
    }
}

