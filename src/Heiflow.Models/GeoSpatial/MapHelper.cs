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
