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
   /// YandexSatelliteMap provider
   /// </summary>
   public class YandexSatelliteMapProvider : YandexMapProviderBase
   {
      public static readonly YandexSatelliteMapProvider Instance;

      YandexSatelliteMapProvider()
      {
      }

      static YandexSatelliteMapProvider()
      {
         Instance = new YandexSatelliteMapProvider();
      }

      public new string Version = "1.23.0";

      #region GMapProvider Members

      readonly Guid id = new Guid("2D4CE763-0F91-40B2-A511-13EF428237AD");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "YandexSatelliteMap";
      public override string Name
      {
         get
         {
            return name;
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
         //http://sat04.maps.yandex.ru/tiles?l=sat&v=1.18.0&x=149511&y=83513&z=18&g=Gagari
         //http://sat01.maps.yandex.net/tiles?l=sat&v=1.23.0&x=584&y=324&z=10&g=Gaga

         return string.Format(UrlFormat, UrlServer, GetServerNum(pos, 4) + 1, Version, pos.X, pos.Y, zoom);
      }

      static readonly string UrlServer = "sat";
      static readonly string UrlFormat = "http://{0}0{1}.maps.yandex.ru/tiles?l=sat&v={2}&x={3}&y={4}&z={5}";
   }
}