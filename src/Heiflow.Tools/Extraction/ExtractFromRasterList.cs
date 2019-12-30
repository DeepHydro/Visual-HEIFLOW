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
using Heiflow.Spatial.SpatialAnalyst;

namespace Heiflow.Tools.Conversion
{
    public class FromRasterList : ModelTool
    {
        public FromRasterList()
        {
            Name = "Extract From Raster Lists";
            Category = "Extraction";
            Description = "Extract data cube from a set of rasters to a given point shapefile";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";

            VariableName = "unknown";
        }
        [Category("Input")]
        [Description("A text file that contains the list of raster file names")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string FilenameList
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
        [Category("Input")]
        [Description("The target feature file")]
        [EditorAttribute(typeof(FileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string TargetFeatureFile { get; set; }

        [Category("Output")]
        [Description("The name of the output Data Cube")]
        public string OutputDataCube { get; set; }

        [Category("Optional")]
        [Description("Averaging method")]
        public Core.MyMath.AveragingMethod AveragingMethod
        {
            get;
            set;
        }
        [Category("Optional")]
        [Description("Set NoDataValue")]
        public float NoDataValue
        {
            get;
            set;
        }
        public override void Initialize()
        {
            Initialized = File.Exists(TargetFeatureFile);
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var fs = FeatureSet.Open(TargetFeatureFile);
            if (fs != null && File.Exists(FilenameList))
            {
                var npt = fs.NumRows();
                int progress = 0;
                List<string> files = new List<string>();
                StreamReader sr = new StreamReader(FilenameList);
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (TypeConverterEx.IsNotNull(line))
                        files.Add(line.Trim());
                }
                sr.Close();
                if (files != null)
                {
                    int nstep = files.Count();
                    var mat_out = new DataCube<float>(1, nstep, npt)
                    {
                        Name = OutputDataCube,
                        Variables = new string[] { VariableName }
                    };
                    for (int t = 0; t < nstep; t++)
                    { 
                        progress = t * 100 / nstep;
                        cancelProgressHandler.Progress("Package_Tool", progress, "Processing raster:" + files[t]);
                        if (File.Exists(files[t]))
                        {
                            IRaster raster = Raster.Open(files[t]);
                            float[] vec = null;
                            if (fs.FeatureType == FeatureType.Point)
                            {
                                vec = ZonalStatastics.ZonalByPoint(raster, fs);
                            }
                            else if (fs.FeatureType == FeatureType.Polygon)
                            {
                                vec = ZonalStatastics.ZonalByGrid(raster, fs, AveragingMethod);
                            }
                            for (int i = 0; i < npt; i++)
                            {
                                mat_out[0, t, i] = vec[i] != ZonalStatastics.NoDataValue ? vec[i] : this.NoDataValue;
                            }
                        }
                        else
                        {
                            cancelProgressHandler.Progress("Package_Tool", progress, "Warning. The raster dose not exist: " + files[t]);
                        }
                    }
                    Workspace.Add(mat_out);
                    fs.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 50, "Failed to run. The input parameters are incorrect.");
                return false;
            }
        }
    
        private float FindNearestCellValue(IRaster raster, Coordinate pt, int neibor)
        {
            var list = new List<double>();
            var p1 = new Coordinate(pt.X - raster.CellWidth * neibor, pt.Y + raster.CellHeight * neibor);
            var vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X, pt.Y + raster.CellHeight * neibor);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X + raster.CellWidth * neibor, pt.Y + raster.CellHeight * neibor);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X - raster.CellWidth * neibor, pt.Y );
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X + raster.CellWidth * neibor, pt.Y);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X - raster.CellWidth * neibor, pt.Y - raster.CellHeight * neibor);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X, pt.Y - raster.CellHeight * neibor);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X + raster.CellWidth * neibor, pt.Y - raster.CellHeight * neibor);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            if (list.Count > 0)
                return (float)list.Average();
            else
                return FindNearestCellValue(raster, pt, neibor + 1);
        }

        public bool Execute1(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var fs = FeatureSet.Open(TargetFeatureFile);
            if (fs != null && File.Exists(FilenameList))
            {
                var npt = fs.NumRows();
                Coordinate[] coors = new Coordinate[npt];
                int progress = 0;
                for (int i = 0; i < npt; i++)
                {
                    var geo_pt = fs.GetFeature(i).Geometry;
                    coors[i] = geo_pt.Coordinate;
                }
                List<string> files = new List<string>();
                StreamReader sr = new StreamReader(FilenameList);
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (TypeConverterEx.IsNotNull(line))
                        files.Add(line.Trim());
                }
                sr.Close();
                if (files != null)
                {
                    int nstep = files.Count();
                    var mat_out = new DataCube<float>(1, nstep, npt)
                    {
                        Name = OutputDataCube,
                        Variables = new string[] { VariableName }
                    };
                    for (int t = 0; t < nstep; t++)
                    {
                        progress = t * 100 / nstep;
                        if (File.Exists(files[t]))
                        {
                            IRaster raster = Raster.Open(files[t]);
                            for (int i = 0; i < npt; i++)
                            {
                                var vv = raster.GetNearestValue(coors[i]);
                                if (vv != raster.NoDataValue)
                                    mat_out[0, t, i] = (float)vv;
                                else
                                {
                                    mat_out[0, t, i] = FindNearestCellValue(raster, coors[i], 1);
                                }

                            }
                            cancelProgressHandler.Progress("Package_Tool", progress, "Processing raster:" + files[t]);
                        }
                        else
                        {
                            cancelProgressHandler.Progress("Package_Tool", progress, "Warning. The raster dose not exist: " + files[t]);
                        }
                    }
                    Workspace.Add(mat_out);
                    fs.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 50, "Failed to run. The input parameters are incorrect.");
                return false;
            }
        }
    }
}