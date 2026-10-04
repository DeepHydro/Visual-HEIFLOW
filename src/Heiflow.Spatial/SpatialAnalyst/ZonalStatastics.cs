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
using GeoAPI.Geometries;
using Heiflow.Core.Alglib;
using Heiflow.Core.MyMath;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Spatial.SpatialAnalyst
{
    public class ZonalStatastics
    {
        public static float NoDataValue = -9999;
        public static string NoDataValueString = "-9999";
        public static float[] ZonalByGrid(IRaster raster, IFeatureSet featureset, AveragingMethod method)
        {
            var nfea = featureset.Features.Count;
            float[] vec = new float[nfea];
            var dx = System.Math.Sqrt(featureset.GetFeature(0).Geometry.Area);
            int nsample = (int)System.Math.Floor(dx / raster.CellHeight);
            List<Coordinate> list = new List<Coordinate>();
            List<float> rasvv = new List<float>();
            if (nsample < 1)
                nsample = 1;
            for (int i = 0; i < nfea; i++)
            {              
                list.Clear();
                rasvv.Clear();
                var fea = featureset.GetFeature(i).Geometry;
                var x0 = (from p in fea.Coordinates select p.X).Min();
                var y0 = (from p in fea.Coordinates select p.Y).Min();
                for (int r = 0; r <= nsample; r++)
                {
                    var y = y0 + r * raster.CellHeight;
                    for (int c = 0; c <= nsample; c++)
                    {
                        var x = x0 + c * raster.CellWidth;
                        Coordinate pt = new Coordinate(x, y);
                        list.Add(pt);
                    }
                }
                foreach (var pt in list)
                {
                    var cell = raster.ProjToCell(pt);
                    if (cell != null && cell.Row > 0)
                    {
                        var buf = (float)raster.Value[cell.Row, cell.Column];
                        if (buf != raster.NoDataValue)
                        {
                            rasvv.Add(buf);
   
                        }
                    }
                }
                if (rasvv.Count > 0)
                {
                    if (method == AveragingMethod.Mean)
                        vec[i] = rasvv.Average();
                    else if (method == AveragingMethod.Median)
                    {
                        vec[i] = MyStatisticsMath.Median(rasvv.ToArray());
                    }
                    else if (method == AveragingMethod.PseudoMedian)
                    {
                        vec[i] = MyStatisticsMath.PseudoMedian(rasvv.ToArray());
                    }
                    else if (method == AveragingMethod.Majority)
                    {
                        vec[i] = MyStatisticsMath.Majority(rasvv.ToArray());
                    }
                }
                else
                    vec[i] = NoDataValue;
            }
            return vec;
        }

        public static float[] ZonalByPoint(IRaster raster, IFeatureSet featureset)
        {
            var nfea = featureset.Features.Count;
            float[] vec = new float[nfea];
            Coordinate[] coors = new Coordinate[nfea];

            for (int i = 0; i < nfea; i++)
            {
                var geo_pt = featureset.GetFeature(i).Geometry;
                coors[i] = geo_pt.Coordinate;
            }

            for (int i = 0; i < nfea; i++)
            {
                var cell = raster.ProjToCell(coors[i]);
                if (cell != null && cell.Row > 0)
                {
                    vec[i] = (float)raster.Value[cell.Row, cell.Column];
                }
                else
                    vec[i] = NoDataValue;
                //vec[i] = (float)raster.GetNearestValue(coors[i]);
                //if(vec[i] == raster.NoDataValue)
                //{
                //    vec[i] = NoDataValue;
                //}
               // vec[i] = FindNearestCellValue(raster, coors[i],1, AveragingMethod.Majority);
            }
            return vec;
        }

        public static float GetCellAverage(IRaster raster, Coordinate lowerleft, float cell_size, AveragingMethod method)
        {
            float averaged=0;
            int nsample = (int)System.Math.Floor(cell_size / raster.CellHeight);
            var x0 = lowerleft.X;
            var y0 = lowerleft.Y;
            nsample = nsample < 1 ? 1 : nsample;
            List<Coordinate> list = new List<Coordinate>();
            List<float> rasvv = new List<float>();
            for (int r = 0; r < nsample; r++)
            {
                var y = y0 + r * raster.CellHeight;
                for (int c = 0; c < nsample; c++)
                {
                    var x = x0 + c * raster.CellWidth;
                    Coordinate pt = new Coordinate(x, y);
                    list.Add(pt);
                }
            }
            foreach (var pt in list)
            {
                var cell = raster.ProjToCell(pt);
                if (cell != null && cell.Row > 0)
                {
                    var buf = (float)raster.Value[cell.Row, cell.Column];
                    if (buf != raster.NoDataValue)
                    {
                        rasvv.Add(buf);
                    }
                }
            }
            if (rasvv.Count > 0)
            {
                if (method == AveragingMethod.Mean)
                    averaged = rasvv.Average();
                else if (method == AveragingMethod.Median)
                {
                    averaged = MyStatisticsMath.Median(rasvv.ToArray());
                }
                else if (method == AveragingMethod.PseudoMedian)
                {
                    averaged = MyStatisticsMath.PseudoMedian(rasvv.ToArray());
                }
                else if (method == AveragingMethod.Majority)
                {
                    averaged = MyStatisticsMath.Majority(rasvv.ToArray());
                }
            }
            else
                averaged = NoDataValue;
            return averaged;
        }

        public static float FindNearestCellValue(IRaster raster, Coordinate pt, int startneibor,AveragingMethod method)
        {
            var list = new List<double>();
            var p1 = new Coordinate(pt.X - raster.CellWidth * startneibor, pt.Y + raster.CellHeight * startneibor);
            var vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X, pt.Y + raster.CellHeight * startneibor);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X + raster.CellWidth * startneibor, pt.Y + raster.CellHeight * startneibor);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X - raster.CellWidth * startneibor, pt.Y);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X + raster.CellWidth * startneibor, pt.Y);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X - raster.CellWidth * startneibor, pt.Y - raster.CellHeight * startneibor);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X, pt.Y - raster.CellHeight * startneibor);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            p1 = new Coordinate(pt.X + raster.CellWidth * startneibor, pt.Y - raster.CellHeight * startneibor);
            vv = raster.GetNearestValue(p1);
            if (vv != raster.NoDataValue)
                list.Add(vv);

            if (list.Count > 0)
            {
                if(method == AveragingMethod.Mean)
                {
                    return (float)list.Average();
                }
                else
                {
                    return (float)MyStatisticsMath.Majority(list.ToArray());
                }
            }
              
            else
                return FindNearestCellValue(raster, pt, startneibor + 1, method);
        }
    }
}
