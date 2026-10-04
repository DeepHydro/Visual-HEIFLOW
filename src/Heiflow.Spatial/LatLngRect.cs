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
using System.Linq;
using System.Text;

namespace Heiflow.Spatial
{
    public class LatLngRect
    {
        public double West { get; set; }
        public double East { get; set; }
        public double South { get; set; }
        public double North { get; set; }
        private static int pixelsPerTile = 256;
        private static Projection _proj;
        private static double earthRadius = 6378137;
        private static double earthCircum = 40075016.685578488;

        public LatLngRect(double w, double e, double s, double n)
        {
            this.West = w;
            this.East = e;
            this.South = s;
            this.North = n;

            string[] projectionParameters = new string[] { "proj=merc", "ellps=sphere", "a=" + earthRadius.ToString(), "es=0.0", "no.defs" };
            _proj = new Projection(projectionParameters);
        }

        static LatLngRect()
        {
            string[] projectionParameters = new string[] { "proj=merc", "ellps=sphere", "a=" + earthRadius.ToString(), "es=0.0", "no.defs" };
            _proj = new Projection(projectionParameters);
        }

        private void Initialize()
        {

        }

        public static LatLngRect GetLatLngBounds(int row, int col, int level)
        {
            double metersPerPixel = MetersPerPixel(level);
            double totalTilesPerEdge = Math.Pow(2, level);
            double totalMeters = totalTilesPerEdge * pixelsPerTile * metersPerPixel;
            double halfMeters = totalMeters / 2;

            //do meters calculation in VE space
            //the 0,0 origin for VE is in upper left
            double N = row * (pixelsPerTile * metersPerPixel);
            double W = col * (pixelsPerTile * metersPerPixel);
            //now convert it to +/- meter coordinates for Proj.4
            //the 0,0 origin for Proj.4 is 0 lat, 0 lon
            //-22 to 22 million, -11 to 11 million
            N = halfMeters - N;
            W = W - halfMeters;
            double E = W + (pixelsPerTile * metersPerPixel);
            double S = N - (pixelsPerTile * metersPerPixel);

            UV UL = new UV(W, N);
            UV UR = new UV(E, N);
            UV LL = new UV(W, S);
            UV LR = new UV(E, S);
            UV geoUL = _proj.Inverse(UL);
            UV geoLR = _proj.Inverse(LR);
            double latRange = (geoUL.U - geoLR.U) * 180 / Math.PI;

            double north = geoUL.V * 180 / Math.PI;
            double south = geoLR.V * 180 / Math.PI;
            double west = geoUL.U * 180 / Math.PI;
            double east = geoLR.U * 180 / Math.PI;

            //double west = ProjectionMercator.xToLon(W);
            //double east = ProjectionMercator.xToLon(E);
            //double south = ProjectionMercator.yToLat(S);
            //double north = ProjectionMercator.yToLat(N);

            return new LatLngRect(west, east, south, north);
        }

        public static double MetersPerTile(int zoom)
        {
            return MetersPerPixel(zoom) * pixelsPerTile;
        }

        public static double MetersPerPixel(int zoom)
        {
            double arc;
            arc = earthCircum / ((1 << zoom) * pixelsPerTile);
            return arc;
        }

        public static double DegToRad(double d)
        {
            return d * Math.PI / 180.0;
        }

        public static double RadToDeg(double d)
        {
            return d * 180 / Math.PI;
        }
    }
}
