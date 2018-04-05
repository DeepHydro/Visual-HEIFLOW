// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using DotSpatial.Controls;
using DotSpatial.Data;
using Heiflow.Core.Data;
using Heiflow.Core.Utility;
using Heiflow.Models.Generic.Attributes;
using Heiflow.Spatial.SpatialRelation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic.Parameters
{
    [Serializable]
    public class FeatureCoverage : PackageCoverage
    {

        public FeatureCoverage()
        {

        }

        public override void Map()
        {
            int progress = 0;
            OnProcessing(progress);
            int count = 1;
            if (Source is IFeatureSet)
            {
                try
                {
                    var fea_target = TargetFeatureSet.Features;
                    var fea_source = (Source as FeatureSet);
                    int nfea_target = fea_target.Count;
                    int[] target_index = new int[nfea_target];
                    DataTable dt_source = fea_source.DataTable;
                    int index_ap = 0;

                    for (int i = 0; i < nfea_target; i++)
                    {
                        target_index[i] = -1;
                        int j = 0;
                        foreach (var fea in fea_source.Features)
                        {
                            if (SpatialRelationship.PointInPolygon(fea.Geometry.Coordinates, fea_target[i].Geometry.Coordinates[0]))
                            {
                                target_index[i] = j;
                                break;
                            }
                            j++;
                        }
                        progress = i * 100 / nfea_target;
                        OnProcessing(progress);
                    }

                    OnProcessing(0);
                    foreach (var ap in ArealProperties)
                    {
                        var vv = Package.GetType().GetProperty(ap.PropertyName).GetValue(Package);
                        if (vv != null)
                        {
                            if (ap.TypeName == typeof(float).FullName)
                            {
                                var fl = vv as My3DMat<float>;
                                if (fl.Value[GridLayer] != null)
                                {
                                    for (int i = 0; i < nfea_target; i++)
                                    {
                                        if (target_index[i] >= 0)
                                            fl.Value[GridLayer][0][i] = float.Parse(GetValue(ap.AliasName, target_index[i]));
                                        else
                                            fl.Value[GridLayer][0][i] = (float)ap.DefaultValue;
                                    }
                                }
                            }
                            else if (ap.TypeName == typeof(short).FullName)
                            {
                                var fl = vv as My3DMat<short>;
                                if (fl.Value[GridLayer] != null)
                                {
                                    for (int i = 0; i < nfea_target; i++)
                                    {
                                        if (target_index[i] >= 0)
                                            fl.Value[GridLayer][0][i] = short.Parse(GetValue(ap.AliasName, target_index[i]));
                                        else
                                            fl.Value[GridLayer][0][i] = (short)ap.DefaultValue;
                                    }
                                }
                            }
                        }
                        progress = index_ap * 100 / ArealProperties.Count;
                        if (progress > count)
                        {
                            OnProcessing(progress);
                            count++;
                        }

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