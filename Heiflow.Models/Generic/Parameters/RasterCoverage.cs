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
