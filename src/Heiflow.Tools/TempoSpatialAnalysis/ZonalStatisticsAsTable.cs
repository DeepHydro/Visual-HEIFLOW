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

namespace Heiflow.Tools.TempoSpatialAnalysis
{
    public class ZonalStatisticsAsTable : MapLayerRequiredTool
    {
        private IMapLayerDescriptor _FeatureLayer;
    
        public ZonalStatisticsAsTable()
        {
            Name = "Zonal Stats As Table";
            Category = "Tempo-Spatial Analysis";
            Description = "Zonal Stats As Table";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            Output = "zonal";
            FilterOperation = DataFilters.None;
            DataOperation = DataOperations.Average;
            BaseTimeStep = 0;
        }

        [Category("Input")]
        [Description("Input datacube. The matrix name should be written as A[0][:][:]")]
        public string DataCube { get; set; }
  
        [Category("Input")]
        [Description("Zone field")]
        [EditorAttribute(typeof(FieldsDropdownList), typeof(System.Drawing.Design.UITypeEditor))]
        public string ZoneField
        {
            get;
            set;
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

        [Category("Parameter")]
        [Description("if BaseTimeStep >=0, spatial cells will be fixed on the basis of NoDataValue. Otherwise, spatial cells may change at different time step")]
        public int BaseTimeStep { get; set; }
        
        [Category("Output")]
        [Description("The name of  output statistics table")]
        public string Output { get; set; }
        [Category("Parameter")]
        [Description("Value used to applied on the Data Operation")]
        public float ThreshholdValue { get; set; }

        [Category("Parameter")]
        [Description("Filter operation")]
        public DataFilters FilterOperation { get; set; }

        [Category("Parameter")]
        [Description("Data operation")]
        public DataOperations DataOperation { get; set; }

        public override void Initialize()
        {
            var mat = Get3DMat(DataCube);
            Initialized = mat != null && FeatureLayer != null && !TypeConverterEx.IsNull(ZoneField);
            Initialized = true;
        }

        public Dictionary<int, List<int>> GetZone()
        {
            Dictionary<int, List<int>> dic = new Dictionary<int, List<int>>();
            var fs_centroid = ProjectService.Project.CentroidLayer.FeatureSet;
            var fs = FeatureLayer.DataSet as IFeatureSet;
            var dt = fs.DataTable;
            var npolygon = fs.NumRows();
            var npt = fs_centroid.NumRows();
            var polygon = new List<IFeature>();
            var list_id = new List<int>();
            for (int i = 0; i < npolygon; i++)
            {
                var id = int.Parse(dt.Rows[i][ZoneField].ToString());
                var pts = new List<int>();
                dic.Add(id, pts);
                var py= fs.GetFeature(i);
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
            }

            return dic;
        }

        public override bool Execute(DotSpatial.Data.ICancelProgressHandler cancelProgressHandler)
        {
            int var_indexA = 0;
            var mat = Get3DMat(DataCube, ref var_indexA);
            double prg = 0;
            var dic = GetZone();
            int nzone = dic.Keys.Count;


            if (mat != null)
            {
                int nstep = mat.Size[1];
                int ncell = mat.Size[2];

                var mat_out = new DataCube<float>(1, nstep, nzone);
                mat_out.Name = Output;
                mat_out.Variables = new string[] { "Mean" };
                var zonstrs = string.Join(",", dic.Keys);
                cancelProgressHandler.Progress("Package_Tool", 1, "Zone ID List: " + zonstrs);

                for (int c = 0; c < nzone; c++)
                {
                    cancelProgressHandler.Progress("Package_Tool", (int)prg, "Calculating Zone: " + (c + 1));
                    var sub_id = dic[dic.Keys.ElementAt(c)];
                    int nsub_id = sub_id.Count;
                    prg = (c + 1) * 100.0 / nzone;
                    for (int t = 0; t < nstep; t++)
                    {
                        var vec = mat.ILArrays[var_indexA][t, ":"].ToArray();
                        var buf = sub_id.Select(pos => vec[pos - 1]);
                        IEnumerable<float> selectedValues = null;
                        switch (FilterOperation)
                        {
                            case DataFilters.EqualTo:
                                selectedValues = buf.Where(v => v == ThreshholdValue);
                                break;
                            case DataFilters.GreaterThan:
                                selectedValues = buf.Where(v => v > ThreshholdValue);
                                break;
                            case DataFilters.LessThan:
                                selectedValues = buf.Where(v => v < ThreshholdValue);
                                break;
                            case DataFilters.NotEqualTo:
                                selectedValues = buf.Where(v => v != ThreshholdValue);
                                break;
                            case DataFilters.None:
                                selectedValues = buf;
                                break;
                        }

                        if (selectedValues.Any())
                        {
                            if (DataOperation == DataOperations.Average)
                                mat_out[0, t, c] = selectedValues.Average();
                            else if (DataOperation == DataOperations.Sum)
                                mat_out[0, t, c] = selectedValues.Sum();
                        }
                        else
                        {
                            mat_out[0, t, c] = 0;
                        }
                        var tprg = (int)((t + 1) * 100.0 / nstep);
                        if (tprg % 10 == 0)
                            cancelProgressHandler.Progress("Package_Tool", (int)prg, string.Format("Step: {0}", t + 1));
                    }
                    cancelProgressHandler.Progress("Package_Tool", (int)prg, "Zone: " + (c + 1) + " finished");
                }
                cancelProgressHandler.Progress("Package_Tool", 100, "All zones finished");
                Workspace.Add(mat_out);
                return true;
            }
            else
            {

                return false;
            }
        }
    }
}