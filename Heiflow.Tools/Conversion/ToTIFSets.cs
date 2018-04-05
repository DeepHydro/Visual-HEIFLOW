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

namespace Heiflow.Tools.Conversion
{
    public class ToTIFSets : ModelTool
    {
        public enum FilterMode { Maximum, Minimum, None}
        public ToTIFSets()
        {
            Name = "Save As TIF Data Sets";
            Category = "Conversion";
            Description = "Convert data cube  to tif data sets";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            DateFormat = "yyyy-MM-dd";
            VariableName = "unknown";
            Scale = 1f;
            Filter = FilterMode.None;
            Direcotry = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        [Category("Input")]
        [Description("The name of the matrix being exported")]
        public string Matrix { get; set; }

        [Category("Input")]
        [Description("The template raster")]
       public string TemplateFile
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The raster")]
        public string FeatureFile
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The scale value will be applied on the matrix")]
        public float Scale
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("The name of exported variable")]
        public string VariableName
        {
            get;
            set;
        }


        [Category("Output")]
        [Description("The file folder directory for output")]
        [EditorAttribute(typeof(FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Direcotry
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("If using maximum or minimum Filter, the filter matrix (ConMat) will be applied on the input matrix.")]
        public FilterMode Filter
        {
            get;
            set;
        }
        [Category("Parameter")]
        [Description("Conditional matrix that will be applied on the input matrix. Values that are greater or smaller than the conditional value will be set to the conditional value")]
        public string ConMat
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("Specify the date format used to generate the output files automatically. Its style should be yyyy-MM-dd or yyyy-MM")]
        public string DateFormat
        {
            get;
            set;
        }

        public override void Initialize()
        {
            var mat = Get3DMat(Matrix);
            Initialized = mat != null;
            if(Filter != FilterMode.None)
            {
                mat = Get3DMat(ConMat);
                Initialized = mat != null;
            }
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            IFeatureSet fs = FeatureSet.Open(FeatureFile);
            int var_index=0;
            int con_var_index = 0;
            var mat = Get3DMat(Matrix, ref var_index);
            My3DMat<float> con_mat = null;
            if (Filter != FilterMode.None)
                con_mat = Get3DMat(ConMat, ref con_var_index);
            if (fs != null &&  mat != null)
            { 
                IRaster raster = Raster.Open(TemplateFile);
                raster.SaveAs(TemplateFile+".tif");
                int fea_count = fs.NumRows();
                Coordinate[] coors = new Coordinate[fea_count];
                for (int i = 0; i < fea_count; i++)
                {
                    coors[i] = fs.GetFeature(i).Geometry.Coordinates[0];
                }
                var nsteps = mat.Size[1];
                int progress = 0;
                string[] fns = new string[nsteps];
                if (mat.DateTimes != null)
                {
                    for (int t= 0; t < nsteps; t++)
                    {
                        fns[t] = string.Format("{0}_{1}.tif", VariableName, mat.DateTimes[t].ToString(DateFormat));
                    }
                }
                else
                {
                    for (int t = 0; t < nsteps; t++)
                    {
                        fns[t] = string.Format("{0}_{1}.tif", VariableName, t.ToString("0000"));
                    }
                }
                if (Filter != FilterMode.None)
                {
                    for (int t = 0; t < nsteps; t++)
                    {
                        string outras = Path.Combine(Direcotry, fns[t]);
                        int i = 0;
                        foreach (var cor in coors)
                        {
                            var cell = raster.ProjToCell(cor);
                            var temp = mat.Value[var_index][t][i] * Scale;

                            if (Filter == FilterMode.Maximum)
                            {
                                if (temp > con_mat.Value[0][0][i])
                                    temp = con_mat.Value[0][0][i];
                            }
                            else if (Filter == FilterMode.Minimum)
                            {
                                if (temp < con_mat.Value[0][0][i])
                                    temp = con_mat.Value[0][0][i];
                            }

                            raster.Value[cell.Row, cell.Column] = temp;
                            i++;
                        }
                        raster.SaveAs(outras);

                        progress = t * 100 / nsteps;
                        cancelProgressHandler.Progress("Package_Tool", progress, "Saving raster to:" + outras);
                    }
                }
                else
                {
                    for (int t = 0; t < nsteps; t++)
                    {
                        string outras = Path.Combine(Direcotry, fns[t]);
                        int i = 0;
                        foreach (var cor in coors)
                        {
                            var cell = raster.ProjToCell(cor.X - 500, cor.Y + 500);
                            var temp = mat.Value[var_index][t][i] * Scale;
                            raster.Value[cell.Row, cell.Column] = temp;
                            i++;
                        }
                        raster.SaveAs(outras);

                        progress = t * 100 / nsteps;
                        cancelProgressHandler.Progress("Package_Tool", progress, "Saving raster to:" + outras);
                    }
                }
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Failed to run. The input parameters are incorrect.");
                return false;
            }
        }

    }
}