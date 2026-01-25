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
using Heiflow.Applications;
using Heiflow.Controls.WinForm.Editors;
using Heiflow.Controls.WinForm.Toolbox;
using Heiflow.Core;
using Heiflow.Core.Data;
using Heiflow.Core.MyMath;
using Heiflow.Models.Tools;
using Heiflow.Presentation.Services;
using Heiflow.Spatial.SpatialRelation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Heiflow.Tools.Statisitcs
{
    public class SaveZoneMappingTable : MapLayerRequiredTool
    {
        private IMapLayerDescriptor _FeatureLayer;

        public SaveZoneMappingTable()
        {
            Name = "Save Zone Map Table";
            Category = "Tempo-Spatial Analysis";
            Description = "Save Zone Map Table";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
        }


        [Category("Input")]
        [Description("Input feature zone layer")]
        [EditorAttribute(typeof(MapLayerDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public IMapLayerDescriptor FeatureLayer
        {
            get
            {
                return _FeatureLayer;
            }
            set
            {
                _FeatureLayer = value;
                if (_FeatureLayer.DataSet is IFeatureSet)
                {
                    var buf = from DataColumn cc in (_FeatureLayer.DataSet as IFeatureSet).DataTable.Columns where cc.DataType == typeof(Int32) select cc.ColumnName;
                    _FieldsOfSelectedLayer = buf.ToArray();
                }
                else
                {
                    _FieldsOfSelectedLayer = null;
                }
            }
        }

        [Category("Input")]
        [Description("Zone field")]
        [EditorAttribute(typeof(FieldsDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public string ZoneField
        {
            get;
            set;
        }

        [Category("Output")]
        [Description("The output filename")]
        [EditorAttribute(typeof(SaveFileNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string OutputFileName
        {
            get;
            set;
        }

        public override void Initialize()
        {
            Initialized = FeatureLayer != null && !TypeConverterEx.IsNull(OutputFileName) && !TypeConverterEx.IsNull(ZoneField);
        }

        public Dictionary<int, List<int>> GetZone(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            Dictionary<int, List<int>> dic = new Dictionary<int, List<int>>();
            var fs_centroid = ProjectService.Project.CentroidLayer.FeatureSet;
            var fs = FeatureLayer.DataSet as IFeatureSet;
            var dt = fs.DataTable;
            var npolygon = fs.NumRows();
            var npt = fs_centroid.NumRows();
            var polygon = new List<IFeature>();
            var list_id = new List<int>();
            double prg = 0;

            for (int i = 0; i < npolygon; i++)
            {
                var id = int.Parse(dt.Rows[i][ZoneField].ToString());
                var pts = new List<int>();
                dic.Add(id, pts);
                var py = fs.GetFeature(i);
                polygon.Add(py);
                list_id.Add(id);
            }
            for (int i = 0; i < npt; i++)
            {
                var geo_pt = fs_centroid.GetFeature(i).Geometry;
                int t = 0;
                foreach (var fea in polygon)
                {
                    if (geo_pt.Within(fea.Geometry))
                    {
                        dic[list_id[t]].Add(i + 1);
                        break;
                    }
                    t++;
                }
                prg = (i + 1) * 100.0 / npt;
                if (prg % 10 == 0)
                    cancelProgressHandler.Progress("Package_Tool", (int)prg, "Caculating Point: " + (i + 1));
            }

            return dic;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            var dic = GetZone(cancelProgressHandler);
            int nzone = dic.Keys.Count;
            cancelProgressHandler.Progress("Package_Tool", 1, "Saving zone mapping. Please wait....");
            StreamWriter sw = new StreamWriter(OutputFileName);
            string line = "zoneid,hruid";
            sw.WriteLine(line);
            for (int i = 0; i < nzone; i++)
            {
                var key = dic.Keys.ElementAt(i);
                var list = dic.ElementAt(i).Value;
                for(int j=0;j<list.Count;j++)
                {
                    line = string.Format("{0},{1}", key, list[j]);
                    sw.WriteLine(line);
                }
            }
            sw.Close();
            cancelProgressHandler.Progress("Package_Tool", 100, "Mapping file to: " + OutputFileName);
            return true;
        }
    }
}