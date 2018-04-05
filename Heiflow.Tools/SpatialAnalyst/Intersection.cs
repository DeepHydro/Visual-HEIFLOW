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
using DotSpatial.Projections;
using GeoAPI.Geometries;
using Heiflow.Controls.WinForm.Editors;
using Microsoft.Research.Science.Data;
using Microsoft.Research.Science.Data.NetCDF4;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms.Design;

namespace Heiflow.Tools.SpatialAnalyst
{
    public class Intersection : ModelTool
    {
        public Intersection()
        {
            Name = "Intersection";
            Category = "Spatial Analyst";
            Description = "Convert nc file to tif files";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";

            FeatureFile = @"C:\Users\Administrator\Documents\MIHO\Geospatial\New_Shapefile.shp";
            PolygonFile = @"C:\Users\Administrator\Documents\MIHO\Geospatial\Grid.shp";
            OutputFeatureFile = @"C:\Users\Administrator\Documents\MIHO\Geospatial\cliped_out.shp";
         
        }
        public string FeatureFile
        {
            get;
            set;
        }
        public string PolygonFile
        {
            get;
            set;
        }
        public string OutputFeatureFile
        {
            get;
            set;
        }

        public override void Initialize()
        {
            Initialized = true;
        }

        public override bool Execute(ICancelProgressHandler cancelProgressHandler)
        {
            IFeatureSet fs = FeatureSet.Open(FeatureFile);
            IFeatureSet polygon_fs = FeatureSet.Open(PolygonFile);

            var outfs = fs.Intersection1(polygon_fs, FieldJoinType.All, cancelProgressHandler);
            outfs.SaveAs(OutputFeatureFile, true);
            return true;
        }
    }
}
