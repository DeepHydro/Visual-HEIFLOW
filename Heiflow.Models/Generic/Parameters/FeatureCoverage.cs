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