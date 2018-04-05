// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
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

namespace Heiflow.Tools.Statisitcs
{
    public class ZonalStatisticsAsTable : MapLayerRequiredTool
    {
        private NumericalDataType mNumericalDataType;
        private TimeUnits mTimeUnit;
        private IMapLayerDescriptor _FeatureLayer;
    

        public ZonalStatisticsAsTable()
        {
            Name = "Zonal Statistics As Table";
            Category = "Tempo-Spatial Analysis";
            Description = "Zonal As Table";
            Version = "1.0.0.0";
            this.Author = "Yong Tian";
            Output = "zonal";
            mNumericalDataType = Core.NumericalDataType.Average;
            mTimeUnit = TimeUnits.Month;
            NoDataValue = -999;
            BaseTimeStep = 0;
        }

        [Category("Input")]
        [Description("Input matrix. The matrix name should be written as A[0][-1][-1]")]
        public string Matrix { get; set; }
  
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
        public NumericalDataType NumericalDataType
        {
            get
            {
                return mNumericalDataType;
            }
            set
            {
                mNumericalDataType = value;
            }
        }

        [Category("Parameter")]
        public TimeUnits TimeUnits
        {
            get
            {
                return mTimeUnit;
            }
            set
            {
                mTimeUnit = value;
            }
        }

        [Category("Parameter")]
        [Description("Values equal to NoDataValue will be excluded during statistics")]
        public float NoDataValue { get; set; }

        [Category("Parameter")]
        [Description("if BaseTimeStep >=0, spatial cells will be fixed on the basis of NoDataValue. Otherwise, spatial cells may change at different time step")]
        public int BaseTimeStep { get; set; }
        
        [Category("Output")]
        [Description("The name of  output statistics table")]
        public string Output { get; set; }



        public override void Initialize()
        {
            var mat = Get3DMat(Matrix);
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
                    if (SpatialRelationship.PointInPolygon(fea.Geometry.Coordinates, geo_pt.Coordinates[0]))
                    {
                        dic[list_id[t]].Add(i);
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
            var mat = Get3DMat(Matrix, ref var_indexA);
            double prg = 0;
            var dic = GetZone();
            int nzone = dic.Keys.Count;


            if (BaseTimeStep >= 0 && BaseTimeStep < mat.Size[0])
            {
                var vec = mat[var_indexA, BaseTimeStep];
                for (int c = 0; c < nzone; c++)
                {
                    var key = dic.Keys.ElementAt(c);
                    List<int> list_selected = new List<int>();
                    for (int i = 0; i < dic[key].Count; i++)
                    {
                        if (vec[i] != NoDataValue)
                            list_selected.Add(dic[key][i]);
                    }
                    dic[key] = list_selected;
                }
            }

            if (mat != null)
            {
                //FeatureLayer.DataSet
                int nstep = mat.Size[1];
                int ncell = mat.Size[2];
               

                var mat_out = new My3DMat<float>(1, nstep, nzone);
                mat_out.Name = Output;
                mat_out.Variables = new string[] { "Mean"};
                for (int t = 0; t < nstep; t++)
                {
                    for (int c = 0; c < nzone; c++)
                    {
                        var sub_id = dic[dic.Keys.ElementAt(c)];
                        int nsub_id = sub_id.Count;
                        float sum = 0;
                        int len = 0;
                        for (int j = 0; j < nsub_id; j++)
                        {
                            if (mat.Value[var_indexA][t][sub_id[j]] != NoDataValue)
                            {
                                sum += mat.Value[var_indexA][t][sub_id[j]];
                                len++;
                            }
                        }
                        if (len > 0)
                            mat_out.Value[0][t][c] = sum / len;
                        else
                            mat_out.Value[0][t][c] = 0;
                    }
                    prg = (t + 1) * 100.0 / nstep;
                    if (prg % 10 == 0)
                        cancelProgressHandler.Progress("Package_Tool", (int)prg, "Caculating Step: " + (t + 1));
                }
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