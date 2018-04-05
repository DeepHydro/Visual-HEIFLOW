// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using Heiflow.Core.Data;
using Heiflow.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.GeoSpatial
{
    public static class MapHelper
    {
        public static FeatureLayer Select(string filename, IMap map, string map_directory)
        {
            FeatureLayer selected = null;
            if (File.Exists(filename))
            {
                foreach (var fe in map.Layers)
                {
                    if (fe is FeatureLayer)
                    {
                        var layer = (fe as FeatureLayer);
                        var fs = layer.FeatureSet;
                        if (!TypeConverterEx.IsNull(fs.Filename))
                        {
                            var fs_file = Path.Combine(map_directory, fs.Filename);
                            if(DirectoryHelper.Compare(filename, fs_file))
                                selected = layer;
                        }
                    }
                }
            }
            return selected;
        }

        public static IDataSet SelectDataSet(string filename, IMap map, string map_directory)
        {
            IDataSet ds = null;
            foreach (var layer in map.Layers)
            {
                var fn_ds = layer.DataSet.Filename;
                var fn_source = filename;
                if(DirectoryHelper.IsRelativePath(layer.DataSet.Filename))
                {
                    fn_ds = Path.Combine(map_directory, fn_ds);
                }
                if (DirectoryHelper.IsRelativePath(filename))
                {
                    fn_source = Path.Combine(map_directory, filename);
                }
                if (DirectoryHelper.Compare(fn_ds, fn_source))
                {
                    ds = layer.DataSet;
                    break;
                }
            }
            return ds;
        }
    }
}
