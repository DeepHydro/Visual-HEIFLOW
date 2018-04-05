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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Tools.Statisitcs
{
    public class TemporalSimpleStatistics : ModelTool
    {
        public TemporalSimpleStatistics()
        {
            Name = "Temporal Simple Statistics";
            Category = "Statistics";
            Description = "Calculate the spatial simple statistics at each time step: mean, variance, skewness, kurtosis.";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            NoDataValue = -999;
            BaseTimeStep = 0;
        }

        [Category("Input")]
        [Description("The input matrix being analyzed. The matrix style shoud be mat[0][-1][-1]")]
        public string Matrix { get; set; }

        [Category("Parameter")]
        [Description("Values equal to NoDataValue will be excluded during statistics")]
        public float NoDataValue { get; set; }

        [Category("Parameter")]
        [Description("if BaseTimeStep >=0, spatial cells will be fixed on the basis of NoDataValue. Otherwise, spatial cells may change at different time step")]
        public int BaseTimeStep { get; set; }

        public override void Initialize()
        {
            var mat = Get3DMat(Matrix);
            Initialized = mat != null;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int var_index = 0;
            var mat = Get3DMat(Matrix, ref var_index);
            double prg = 0;

            if (mat != null)
            {
                DataTable dt = new DataTable();

                DataColumn dc_time = new DataColumn("DateTime", Type.GetType("System.DateTime"));
                dt.Columns.Add(dc_time);
                DataColumn dc = new DataColumn("Mean", Type.GetType("System.Double"));
                dt.Columns.Add(dc);
                dc = new DataColumn("Variance", Type.GetType("System.Double"));
                dt.Columns.Add(dc);
                dc = new DataColumn("Skewness", Type.GetType("System.Double"));
                dt.Columns.Add(dc);
                dc = new DataColumn("Kurtosis", Type.GetType("System.Double"));
                dt.Columns.Add(dc);

                int nstep = mat.Size[1];
                double mean = 0, variance = 0, skewness = 0, kurtosis = 0;
                List<int> list_selected = new List<int>();
                bool use_selected = false;
                //   selected_index = null;

                if (BaseTimeStep >= 0 && BaseTimeStep < mat.Size[0])
                {
                    var vec = mat[var_index, BaseTimeStep];
                    for (int i = 0; i < vec.Length; i++)
                    {
                        if (vec[i] != NoDataValue)
                            list_selected.Add(i);
                    }
                    use_selected = true;
                }
                for (int i = 0; i < nstep; i++)
                {
                    var dr = dt.NewRow();
                    var vec = mat[var_index, i];
                    double[] vec_selected = null;
                    if (use_selected)
                    {
                        vec_selected = new double[list_selected.Count];
                        for(int  j=0;j<list_selected.Count;j++)
                        {
                            vec_selected[j] = vec[list_selected[j]];
                        }
                    }
                    else
                    {
                        var dou_vec = Array.ConvertAll<float, double>(vec, s => s);
                        vec_selected = dou_vec.Where(s => s != NoDataValue).ToArray();
                    }
                    Heiflow.Core.Alglib.alglib.basestat.samplemoments(vec_selected, vec_selected.Length, ref mean, ref variance, ref skewness, ref kurtosis);
                    dr[0] = DateTime.Now;
                    dr[1] = mean;
                    dr[2] = variance;
                    dr[3] = skewness;
                    dr[4] = kurtosis;
                    dt.Rows.Add(dr);
                    prg = (i + 1) * 100.0 / nstep;
                    if (prg % 10 == 0)
                        cancelProgressHandler.Progress("Package_Tool", (int)prg, "Time Step:" + dr[0].ToString());
                }

                if (mat.DateTimes != null)
                {
                    for (int i = 0; i < nstep; i++)
                    {
                        dt.Rows[i][0] = mat.DateTimes[i];
                    }
                }
                else
                {
                    for (int i = 0; i < nstep; i++)
                    {
                        dt.Rows[i][0] = DateTime.Now.AddDays(i);
                    }
                }
                WorkspaceView.OutputTo(dt);
                return true;
            }
            else
            {

                return false;
            }
        }
    }
}
