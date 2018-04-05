// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Controls;
using Heiflow.Core.Data;
using System.IO;
using Heiflow.Core.Utility;
using Heiflow.Spatial.SpatialAnalyst;

namespace Heiflow.Models.Generic.Parameters
{
    [Serializable]
    public class RasterCoverage : PackageCoverage
    {
        public RasterCoverage()
        {

        }
        public override void Map()
        {
            var fea_target = TargetFeatureSet.Features;
            int progress = 0;
            int index_ap = 0;
            if (Source is IRaster)
            {
                try
                {
                    OnProcessing(progress);
                    var ras = Source as IRaster;
                    float[] vec = null;
                    if (TargetFeatureSet.FeatureType == FeatureType.Point)
                    {
                        vec = ZonalStatastics.ZonalByPoint(ras, TargetFeatureSet);
                    }
                    else
                        if (TargetFeatureSet.FeatureType == FeatureType.Polygon)
                        {
                            vec = ZonalStatastics.ZonalByGrid(ras, TargetFeatureSet, AveragingMethod);
                        }
                    foreach (var ap in ArealProperties)
                    {
                        if (ap.IsParameter)
                        {
                            for (int i = 0; i < vec.Length; i++)
                            {
                                var vv = GetValue(ap.ParameterName, vec[i].ToString());
                                if (vv != ZonalStatastics.NoDataValueString)
                                    ap.Parameter.SetValue(float.Parse(vv), i);
                                else
                                    ap.Parameter.SetValue(ap.DefaultValue, i);
                            }
                        }
                        else
                        {
                            var mat = Package.GetType().GetProperty(ap.PropertyName).GetValue(Package) as My3DMat<float>;
                            if (mat != null)
                            {
                                if (mat.Value[GridLayer] != null)
                                {
                                    for (int i = 0; i < vec.Length; i++)
                                    {
                                        var vv = GetValue(ap.PropertyName, vec[i].ToString());
                                        if (vv != ZonalStatastics.NoDataValueString)
                                            mat.Value[GridLayer][0][i] = float.Parse(vv);
                                        else
                                            mat.Value[GridLayer][0][i] = (float)ap.DefaultValue;
                                    }
                                }
                            }
                        }
                        progress = index_ap * 100 / ArealProperties.Count;
                        OnProcessing(progress);
                        index_ap++;
                    }
                    OnProcessing(100);
                    OnProcessed(ConstantWords.Successful);
                }
                catch (Exception ex)
                {
                    OnProcessing(100);
                    OnProcessed("Failed. Error message: " + ex.Message);
                }
            }
        }
    }
}
