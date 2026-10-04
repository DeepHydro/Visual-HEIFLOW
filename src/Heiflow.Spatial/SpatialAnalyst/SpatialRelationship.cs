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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Spatial.SpatialRelation
{
    public static class SpatialRelationship
    {
        public static List<DotSpatial.Data.IFeature> GetPolygons(IFeatureSet fs)
        {
            var dt = fs.DataTable;
            var npolygon = fs.NumRows();
            var polygon = new List<DotSpatial.Data.IFeature>();
            for (int i = 0; i < npolygon; i++)
            {
                var py = fs.GetFeature(i);
                polygon.Add(py);
            }
            return polygon;
        }

        public static bool PointInPolygon(List<DotSpatial.Data.IFeature> polygons, Coordinate point)
        {
            var oddNodes = false;
            foreach (var fea in polygons)
            {
                if (SpatialRelationship.PointInPolygon(fea.Geometry.Coordinates, point))
                {
                    oddNodes = true;
                    break;
                }
            }
            return oddNodes;
        }

        public static bool PointInPolygon(Coordinate[] _vertices, Coordinate point)
        {
            var j = _vertices.Length - 1;
            var oddNodes = false;

            for (var i = 0; i < _vertices.Length; i++)
            {
                if (_vertices[i].Y < point.Y && _vertices[j].Y >= point.Y ||
                    _vertices[j].Y < point.Y && _vertices[i].Y >= point.Y)
                {
                    if (_vertices[i].X +
                        (point.Y - _vertices[i].Y) / (_vertices[j].Y - _vertices[i].Y) * (_vertices[j].X - _vertices[i].X) < point.X)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;
        }

        public static bool PointInPolygon(IList<Coordinate> _vertices, Coordinate point)
        {
            var j = _vertices.Count - 1;
            var oddNodes = false;

            for (var i = 0; i < _vertices.Count; i++)
            {
                if (_vertices[i].Y < point.Y && _vertices[j].Y >= point.Y ||
                    _vertices[j].Y < point.Y && _vertices[i].Y >= point.Y)
                {
                    if (_vertices[i].X +
                        (point.Y - _vertices[i].Y) / (_vertices[j].Y - _vertices[i].Y) * (_vertices[j].X - _vertices[i].X) < point.X)
                    {
                        oddNodes = !oddNodes;
                    }
                }
                j = i;
            }

            return oddNodes;
        }
    }
}
