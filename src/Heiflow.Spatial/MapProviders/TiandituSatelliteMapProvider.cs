//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
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

   /// <summary>
   /// TiandituSatelliteMapProvider provider, the Chinese national imagery service.
   /// The service is WMTS in KVP form over web mercator tiles, so the tile row, column and level of
   /// the globe can be used as they are. It refuses every request without a key, and the key has to
   /// be one registered for server side use: a browser key answers 403 with "Key权限类型为浏览器端".
   /// The key is not part of the provider, it comes from the Token element of the layer in
   /// Layers.xml and is passed to MakeTileImageUrl by ProjectedMapImageStore.
   /// </summary>
   public class TiandituSatelliteMapProvider : GMapProvider
   {
      public static readonly TiandituSatelliteMapProvider Instance;

      TiandituSatelliteMapProvider()
      {
         Copyright = string.Format("©{0} 天地图 - 国家地理信息公共服务平台", DateTime.Today.Year);
      }

      static TiandituSatelliteMapProvider()
      {
         Instance = new TiandituSatelliteMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("8C1D2E53-7A64-4F1B-9D57-2E0C4A6B9F13");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "TiandituSatelliteMap";
      public override string Name
      {
         get
         {
            return name;
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return MercatorProjection.Instance;
         }
      }

      GMapProvider[] overlays;
      public override GMapProvider[] Overlays
      {
         get
         {
            if (overlays == null)
            {
               overlays = new GMapProvider[] { this };
            }
            return overlays;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         string url = MakeTileImageUrl(pos, zoom, LanguageStr);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      /// <summary>
      /// The key every request needs. It stays empty here, a layer that is used has to supply it.
      /// </summary>
      public static string Token = "";

      public override string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         return MakeTileImageUrl(pos, zoom, language, Token);
      }

      public string MakeTileImageUrl(GPoint pos, int zoom, string language, string token)
      {
         // https://t0.tianditu.gov.cn/img_w/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=img&
         // STYLE=default&TILEMATRIXSET=w&FORMAT=tiles&TILEMATRIX=5&TILEROW=12&TILECOL=26&tk=yourkey

         return string.Format(UrlFormat, GetServerNum(pos, 8), zoom, pos.Y, pos.X, token);
      }

      static readonly string UrlFormat = "https://t{0}.tianditu.gov.cn/img_w/wmts?SERVICE=WMTS&REQUEST=GetTile&VERSION=1.0.0&LAYER=img&STYLE=default&TILEMATRIXSET=w&FORMAT=tiles&TILEMATRIX={1}&TILEROW={2}&TILECOL={3}&tk={4}";
   }
}
