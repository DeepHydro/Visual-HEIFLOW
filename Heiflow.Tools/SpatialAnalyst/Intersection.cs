// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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
