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

using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using GeoAPI.Geometries;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
using Heiflow.Tools.MachineLearning;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools
{
    [InheritedExport(typeof(IModelTool))]
    public abstract class MLModelTool : ModelTool
    {
        public MLModelTool()
        {
            Description = "This is a modeling tool";
            MultiThreadRequired = true;
        }
        protected ICancelProgressHandler _cancelProgressHandler;
        protected MLMode _MLMode;
        [Category("Training")]
        [Description("The input data cube. The data cube style should be [n][0][:]")]
        public string InputTraningDC
        {
            get;
            set;
        }
        [Category("Prediction")]
        [Description("a csv/txt file that contains predication file list. Each row in the file contains input file names separated by comma")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string PredictionFileList
        {
            get;
            set;
        }
        [Category("Prediction")]
        [Description("The mask is represented by a polygon shapefile.")]
        public string MaskFile
        {
            get;
            set;
        }

        [Category("Parameter")]
        [Description("The quota filename")]
        public MLMode Mode
        {
            get
            {
                return _MLMode;
            }
            set
            {
                _MLMode = value;
            }
        }

        [Category("Training")]
        [Description("The output data cube. The data cube style should be [n][0][:]")]
        public string OutputTraningDC
        {
            get;
            set;
        }

        [Category("Parameter")]
        public float Maximum
        { get; set; }

        [Category("Parameter")]
        public float Minimum
        { get; set; }

        protected virtual void ForecastRaster(ICancelProgressHandler cancelProgressHandler)
        {
            var inputdc = Get3DMat(InputTraningDC);
            int ninputvar = inputdc.Size[0];
            IFeatureSet mask = null;
            List<IFeature> mask_polygons = null;
            IRaster output = null;
            int ntask = 0;
            int progress = 0;
            double forecasted = 0;
            string line = "";
            RcIndex index1;
            IRaster input1 = null;
            List<IRaster> rasters = new List<IRaster>();
            double[] vec = new double[ninputvar];
            Extent envelope = null;
            StreamReader sr = new StreamReader(PredictionFileList);
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (TypeConverterEx.IsNotNull(line))
                    ntask++;
            }
            sr.Close();
            sr = new StreamReader(PredictionFileList);
            if (TypeConverterEx.IsNotNull(MaskFile) && File.Exists(MaskFile))
            {
                mask = FeatureSet.Open(MaskFile);
                mask_polygons = SpatialRelationship.GetPolygons(mask);
            }
            for (int n = 0; n < ntask; n++)
            {
                line = sr.ReadLine();
                var fns = TypeConverterEx.Split<string>(line, ninputvar + 1);
                cancelProgressHandler.Progress("Package_Tool", progress, "begin to process task " + (n + 1));

                rasters.Clear();
                for (int i = 0; i < ninputvar; i++)
                {
                    var ras = Raster.Open(fns[i]);
                    rasters.Add(ras);
                }
                input1 = rasters[0];
                int nrow = input1.NumRows;
                int ncol = input1.NumColumns;
                if (mask != null)
                {
                    nrow = Convert.ToInt32(System.Math.Abs(mask.Extent.Height / input1.CellHeight));
                    ncol = Convert.ToInt32(System.Math.Abs(mask.Extent.Width / input1.CellWidth));
                    envelope = mask.Extent;
                }
                else
                {
                    envelope = input1.Extent;
                }
                output = Raster.CreateRaster(fns[ninputvar], string.Empty, ncol, nrow, 1, typeof(float), new[] { string.Empty });
                RasterBounds bound = new RasterBounds(nrow, ncol, envelope);
                output.Bounds = bound;
                output.Projection = input1.Projection;
                output.NoDataValue = 0;

                if (mask != null)
                {
                    for (int i = 0; i < nrow; i++)
                    {
                        for (int j = 0; j < ncol; j++)
                        {
                            Coordinate cellCenter = output.CellToProj(i, j);
                            if (SpatialRelationship.PointInPolygon(mask_polygons, cellCenter))
                            {
                                for (int k = 0; k < ninputvar; k++)
                                {
                                    var ras = rasters[k];
                                    index1 = ras.ProjToCell(cellCenter);
                                    if (index1.Row > -1 && index1.Column > -1)
                                    {
                                        vec[k] = ras.Value[index1.Row, index1.Column] == ras.NoDataValue
                                                  ? 0
                                                  : ras.Value[index1.Row, index1.Column];
                                    }
                                    else
                                    {
                                        vec[k] = 0;
                                    }
                                }
                                forecasted = Forecast(vec);
                                if (forecasted < Minimum || forecasted > Maximum)
                                    output.Value[i, j] = input1.NoDataValue;
                                else
                                    output.Value[i, j] = forecasted;
                            }
                            else
                            {
                                output.Value[i, j] = input1.NoDataValue;
                            }
                        }
                        progress = i * 100 / nrow;
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing row " + (i + 1));
                    }
                }
                else
                {
                    for (int i = 0; i < nrow; i++)
                    {
                        for (int j = 0; j < ncol; j++)
                        {
                            for (int k = 0; k < ninputvar; k++)
                            {
                                var ras = rasters[k];
                                vec[k] = ras.Value[i, j] == ras.NoDataValue ? 0 : ras.Value[i, j];
                            }
                            if (vec.Sum() == 0)
                            {
                                output.Value[i, j] = 0;
                            }
                            else
                            {
                                forecasted = Forecast(vec);
                                if (forecasted < Minimum || forecasted > Maximum)
                                    output.Value[i, j] = 0;
                                else
                                    output.Value[i, j] = forecasted;
                            }
                        }
                        progress = i * 100 / nrow;
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing row " + (i + 1));
                    }
                }
                output.Save();
                for (int i = 0; i < ninputvar; i++)
                {
                    rasters[i].Close();
                }
                progress = n * 100 / ntask;
                cancelProgressHandler.Progress("Package_Tool", progress, "Output is saved to: " + fns[ninputvar]);
            }
            sr.Close();
        }

        protected abstract double Forecast(double[] inputVector);
    }
}