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

   /// <summary>
   /// YandexHybridMap provider
   /// </summary>
   public class YandexHybridMapProvider : YandexMapProviderBase
   {
      public static readonly YandexHybridMapProvider Instance;

      YandexHybridMapProvider()
      {
      }

      static YandexHybridMapProvider()
      {
         Instance = new YandexHybridMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("78A3830F-5EE3-432C-A32E-91B7AF6BBCB9");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "YandexHybridMap";
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
               overlays = new GMapProvider[] { YandexSatelliteMapProvider.Instance, this };
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

      public override string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         // http://vec01.maps.yandex.ru/tiles?l=map&v=2.10.2&x=1494&y=650&z=11

         return string.Format(UrlFormat, UrlServer, GetServerNum(pos, 4) + 1, Version, pos.X, pos.Y, zoom);
      }

      static readonly string UrlServer = "vec";
      static readonly string UrlFormat = "http://{0}0{1}.maps.yandex.ru/tiles?l=skl&v={2}&x={3}&y={4}&z={5}";
   }
}