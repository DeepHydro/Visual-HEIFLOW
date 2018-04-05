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
using GeoAPI.Geometries;
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Core.Data;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.DataManagement
{
    public class ExtractFromRaster : MapLayerRequiredTool
    {
        private IFeatureSet _target_layer;
        private IRaster _dem_layer;
        public ExtractFromRaster()
        {
            Name = "Extract From Raster";
            Category = "Extraction";
            Description = "Extract values from a Raster layer  to a data cube";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }

        [Category("Input")]
        [Description("Raster")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor RasterLayer
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("The data cube to which the raster values will be extracted. The data cube style should be [0][0][:]")]
        public string Matrix
        {
            get;
            set;
        }

        [Category("Input")]
        [Description("Stream layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor TargetFeature
        {
            get;
            set;
        }

        public override void Initialize()
        {
            if (TargetFeature == null || RasterLayer == null)
            {
                Initialized = false;
                return;
            }
            _target_layer = TargetFeature.DataSet as IFeatureSet;
            _dem_layer = RasterLayer.DataSet as IRaster;
            if (_target_layer == null || _dem_layer == null)
            {
                Initialized = false;
                return;
            }
            this.Initialized = _target_layer.FeatureType == FeatureType.Polygon||  _target_layer.FeatureType == FeatureType.Point;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int progress = 0;
            int count = 1;
            if (_target_layer != null )
            {
                var nrow = _target_layer.NumRows();
                var dx = System.Math.Sqrt(_target_layer.GetFeature(0).Geometry.Area);
                int nsample = (int)System.Math.Floor(dx / _dem_layer.CellHeight);
                var mat = new My3DMat<float>(1, 1, nrow);
                mat.Name = Matrix;
                mat.Variables = new string[] { Matrix };
                mat.TimeBrowsable = false;
                mat.AllowTableEdit = true;
                List<Coordinate> list = new List<Coordinate>();
                if (_target_layer.FeatureType == FeatureType.Polygon)
                {
                    for (int i = 0; i < nrow; i++)
                    {
                        float sum_cellv = 0;
                        int npt = 0;
                        list.Clear();
                        var fea = _target_layer.GetFeature(i).Geometry;
                        var x0 = (from p in fea.Coordinates select p.X).Min();
                        var y0 = (from p in fea.Coordinates select p.Y).Min();
                        for (int r = 0; r <= nsample; r++)
                        {
                            var y = y0 + r * _dem_layer.CellHeight;
                            for (int c = 0; c <= nsample; c++)
                            {
                                var x = x0 + c * _dem_layer.CellWidth;
                                Coordinate pt = new Coordinate(x, y);
                                list.Add(pt);
                            }
                        }
                        foreach (var pt in list)
                        {
                             var cell = _dem_layer.ProjToCell(pt);
                             if (cell != null && cell.Row > 0)
                             {
                                 var buf = (float)_dem_layer.Value[cell.Row, cell.Column]; 
                                 if(buf != _dem_layer.NoDataValue)
                                 {
                                     sum_cellv += buf;
                                     npt++;
                                 }
                             }
                        }
                        if (npt > 0)
                            sum_cellv = sum_cellv / npt;
                        mat.Value[0][0][i] = sum_cellv;

                        progress = i * 100 / nrow;
                        if (progress > count)
                        {
                            cancelProgressHandler.Progress("Package_Tool", progress, "Processing polygon: " + i);
                            count++;
                        }
                    }
                }
                else
                {
                    Coordinate[] coors = new Coordinate[nrow];

                    for (int i = 0; i < nrow; i++)
                    {
                        var geo_pt = _target_layer.GetFeature(i).Geometry;
                        coors[i] = geo_pt.Coordinate;
                    }

                    for (int i = 0; i < nrow; i++)
                    {
                        var cell = _dem_layer.ProjToCell(coors[i]);
                        if (cell != null && cell.Row > 0)
                        {
                            mat.Value[0][0][i] = (float)_dem_layer.Value[cell.Row, cell.Column];
                        }
                        progress = i * 100 / nrow;
                        if (progress > count)
                        {
                            cancelProgressHandler.Progress("Package_Tool", progress, "Processing point: " + i);
                            count++;
                        }
                    }
                }
                Workspace.Add(mat);
                return true;
            }
            else
            {
                cancelProgressHandler.Progress("Package_Tool", 100, "Error message: the input layers are incorrect.");
                return false;
            }
        }
    }
}