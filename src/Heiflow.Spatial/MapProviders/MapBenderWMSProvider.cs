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

namespace Heiflow.Spatial.MapProviders
{
   using System;
   using Heiflow.Spatial.Projections;
   using System.Globalization;

   /// <summary>
   /// MapBender provider, WMS demo http://www.mapbender.org/
   /// </summary>
   public class MapBenderWMSProvider : GMapProvider
   {
      public static readonly MapBenderWMSProvider Instance;

      MapBenderWMSProvider()
      {
      }

      static MapBenderWMSProvider()
      {
         Instance = new MapBenderWMSProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("45742F8D-B552-4CAF-89AE-F20951BBDB2B");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "MapBender, WMS demo";
      public override string Name
      {
         get
         {
            return name;
         }
      }

      GMapProvider[] overlays;
      public override GMapProvider[] Overlays
      {
         get
         {
            if(overlays == null)
            {
               overlays = new GMapProvider[] { this };
            }
            return overlays;
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return MercatorProjection.Instance;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         string url = MakeTileImageUrl(pos, zoom, LanguageStr);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      public override string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         var px1 = Projection.FromTileXYToPixel(pos);
         var px2 = px1;

         px1.Offset(0, Projection.TileSize.Height);
         PointLatLng p1 = Projection.FromPixelToLatLng(px1, zoom);

         px2.Offset(Projection.TileSize.Width, 0);
         PointLatLng p2 = Projection.FromPixelToLatLng(px2, zoom);

         var ret = string.Format(CultureInfo.InvariantCulture, UrlFormat, p1.Lng, p1.Lat, p2.Lng, p2.Lat, Projection.TileSize.Width, Projection.TileSize.Height);

         return ret;
      }

      static readonly string UrlFormat = "http://mapbender.wheregroup.com/cgi-bin/mapserv?map=/data/umn/osm/osm_basic.map&VERSION=1.1.1&REQUEST=GetMap&SERVICE=WMS&LAYERS=OSM_Basic&styles=&bbox={0},{1},{2},{3}&width={4}&height={5}&srs=EPSG:4326&format=image/png";
   }
}