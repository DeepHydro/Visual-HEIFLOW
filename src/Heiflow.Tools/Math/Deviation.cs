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

using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Core.Data.ODM;
using Heiflow.Models.Generic;
using Heiflow.Models.Tools;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Tools.Math
{
    [Serializable]
    public class Deviation : ModelTool
    {

        public Deviation()
        {
            Name = "Deviation";
            Category = "Math";
            Description = "Constrain deviation to a given inteval";
            Version = "1.0.0.0";
            Output = "newmat";
            this.Author = "Yong Tian";
            MaxPositiveDeviation = 10;
            MinPositiveDeviation = -10;
            NeighborCount = 3;
            MaxPositiveDepth = 20;
        }

        [Category("Input")]
        [Description("The input matrix which should be [0][-1][-1]")]
        public string Source
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("The output matrix name")]
        public string Output
        {
            get;
            set;
        }

        public float MaxPositiveDeviation
        {
            get;
            set;
        }

        public float MinPositiveDeviation
        {
            get;
            set;
        }

        public float MaxPositiveDepth
        {
            get;
            set;
        }

        public int NeighborCount
        {
            get;
            set;
        }

        public override void Initialize()
        {
            var m1 = Validate(Source);
            this.Initialized = m1;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var var_index = 0;
            var mat = Get3DMat(Source, ref var_index);
            double prg = 0;
            int count = 1;
            var grid = ProjectService.Project.Model.Grid as RegularGrid;

            if (mat != null && grid != null)
            {
                int nstep = mat.Size[1];
                int ncell = mat.Size[2];

                var mat_out = new DataCube<float>(1, nstep, ncell, false);
                mat_out.Name = Output;
                mat_out.Variables = mat.Variables;
                mat_out.DateTimes = mat.DateTimes;
                mat.CopyTo(mat_out);
                List<float[]> neibor = new List<float[]>();
                int num_dep_modified = 0;
                int num_head_modified = 0;
                int num_no_neighbor_found = 0;
                for (int c = 0; c < ncell; c++)
                {
                    var vec = mat[var_index, ":", c.ToString()];
                    var buf = (from vv in vec select vv - grid.Elevations[0,0,c]).ToArray();
                    var max = buf.Maximum();
                    if (max > MaxPositiveDepth)
                    {
                        var scale = max / MaxPositiveDepth;
                        for (int i = 0; i < nstep; i++)
                        {
                            if (buf[i] > 0)
                            {
                                buf[i] /= scale;
                                mat_out[var_index, i, c] = grid.Elevations[0,0,c] + buf[i];
                            }
                        }
                        num_dep_modified++;
                    }
                }

                for (int c = 0; c < ncell; c++)
                {
                    var vec = mat_out[var_index, ":", c.ToString()];
                    var buf = (from vv in vec select vv - vec[0]).ToArray();
                    var max = buf.Maximum();
                    var min = buf.Minimum();
                    bool modify = false;
                    if (max > MaxPositiveDeviation)
                    {
                        var scale = max / MaxPositiveDeviation;
                        for (int i = 1; i < nstep; i++)
                        {
                            if (buf[i] > 0)
                            {
                                buf[i] /= scale;
                            }
                        }
                        modify = true;
                    }

                    if (min < MinPositiveDeviation)
                    {
                        var scale = min / MinPositiveDeviation;
                        for (int i = 1; i < nstep; i++)
                        {
                            if (buf[i] < 0)
                            {
                                buf[i] /= scale;
                            }
                        }
                        modify = true;
                    }

                    if (modify)
                    {
                        //var cell_id = grid.Topology.ActiveCellIDs[c];
                        int[] loc = grid.Topology.ActiveCell[c];
                        neibor.Clear();
                        for (int ii = loc[0] - NeighborCount; ii <= loc[0] + NeighborCount; ii++)
                        {
                            for (int jj = loc[1] - NeighborCount; jj <= loc[1] + NeighborCount; jj++)
                            {
                                if (ii >= 0 && ii < grid.RowCount && jj >= 0 && jj < grid.ColumnCount && grid.IBound[0,ii,jj] > 0)
                                {
                                    var cell_index = grid.Topology.GetSerialIndex(ii, jj);
                                    if (!RequireModify(mat_out, var_index, cell_index))
                                    {
                                        var neibor_vec = mat_out[var_index, ":", cell_index.ToString()];
                                        var buf_nei = (from vv in neibor_vec select vv - neibor_vec[0]).ToArray();
                                        neibor.Add(buf_nei);
                                    }
                                }
                            }
                        }
                        if (neibor.Count > 0)
                        {
                            for (int i = 1; i < nstep; i++)
                            {
                                // float av_value = 0;
                                var sds = (from vv in neibor select vv.StandardDeviation()).ToArray();
                                var min_sd = sds.Min();
                                var min_sd_index = 0;
                                for (int n = 0; n < sds.Count(); n++)
                                {
                                    if (min_sd == sds[n])
                                    {
                                        min_sd_index = n;
                                        break;
                                    }
                                }
                                //foreach (var ss in neibor)
                                //{
                                //    av_value += ss[i];
                                //}
                                //av_value /= neibor.Count;
                                //mat_out.Value[var_index][i][c] = mat_out.Value[var_index][0][c] + av_value;
                                mat_out[var_index, i, c] = mat_out[var_index, 0, c] + neibor[min_sd_index][i];
                            }
                        }
                        else
                        {
                            for (int i = 1; i < nstep; i++)
                            {
                                vec[i] = vec[0] + buf[i] * 0.5f;
                                mat_out[var_index, i, c] = vec[i];
                            }
                            num_no_neighbor_found++;
                        }
                        num_head_modified++;
                    }

                    prg = (c + 1) * 100.0 / ncell;
                    if (prg > count)
                    {
                        cancelProgressHandler.Progress("Package_Tool", (int)prg, "Caculating Cell: " + (c + 1));
                        count++;
                    }
                }
                cancelProgressHandler.Progress("Package_Tool", 100, string.Format(" \num_dep_modified: {0};\n num_head_modified: {1};\n num_no_neighbor_found:{2}",
                    num_dep_modified, num_head_modified, num_no_neighbor_found));

                Workspace.Add(mat_out);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool RequireModify(DataCube<float> mat, int var_index, int c)
        {
            var modify = false;
            var vec = mat[var_index, ":", c.ToString()];
            var buf = (from vv in vec select vv - vec[0]).ToArray();
            var max = buf.Maximum();
            var min = buf.Minimum();
            if (max > MaxPositiveDeviation || min < MinPositiveDeviation)
            {

                modify = true;
            }
            return modify;
        }
    }
}
