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

namespace Heiflow.Spatial.Projections
{
   using System;

   /// <summary>
   /// Plate Carrée (literally, “plane square”) projection
   /// PROJCS["WGS 84 / World Equidistant Cylindrical",GEOGCS["GCS_WGS_1984",DATUM["D_WGS_1984",SPHEROID["WGS_1984",6378137,298.257223563]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]],UNIT["Meter",1]]
   /// </summary>
   public class PlateCarreeProjectionPergo : PureProjection
   {
      public static readonly PlateCarreeProjectionPergo Instance = new PlateCarreeProjectionPergo();

      static readonly double MinLatitude = -85.05112878;
      static readonly double MaxLatitude = 85.05112878;
      static readonly double MinLongitude = -180;
      static readonly double MaxLongitude = 180;

      GSize tileSize = new GSize(256, 256);
      public override GSize TileSize
      {
         get
         {
            return tileSize;
         }
      }

      public override double Axis
      {
         get
         {
            return 6378137;
         }
      }

      public override double Flattening
      {
         get
         {
            return (1.0 / 298.257223563);
         }
      }

      public override GPoint FromLatLngToPixel(double lat, double lng, int zoom)
      {
         GPoint ret = GPoint.Empty;

         lat = Clip(lat, MinLatitude, MaxLatitude);
         lng = Clip(lng, MinLongitude, MaxLongitude);

         GSize s = GetTileMatrixSizePixel(zoom);
         double mapSizeX = s.Width;
         double mapSizeY = s.Height;

         double scale = 360.0 / mapSizeX;

         ret.Y = (int) ((90.0 - lat) / scale);
         ret.X = (int) ((lng + 180.0) / scale);

         return ret;
      }

      public override PointLatLng FromPixelToLatLng(int x, int y, int zoom)
      {
         PointLatLng ret = PointLatLng.Zero;

         GSize s = GetTileMatrixSizePixel(zoom);
         double mapSizeX = s.Width;
         double mapSizeY = s.Height;

         double scale = 360.0 / mapSizeX;

         ret.Lat = 90 - (y * scale);
         ret.Lng = (x * scale) - 180;

         return ret;
      }

      public override GSize GetTileMatrixMaxXY(int zoom)
      {
         int y = (int) Math.Pow(2, zoom);
         return new GSize((2*y) - 1, y - 1);
      }

      public override GSize GetTileMatrixMinXY(int zoom)
      {
         return new GSize(0, 0);
      }
   }
}
