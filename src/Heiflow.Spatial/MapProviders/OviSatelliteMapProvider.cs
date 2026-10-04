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
   /// OviSatelliteMap provider
   /// </summary>
   public class OviSatelliteMapProvider : OviMapProviderBase
   {
      public static readonly OviSatelliteMapProvider Instance;

      OviSatelliteMapProvider()
      {
      }

      static OviSatelliteMapProvider()
      {
         Instance = new OviSatelliteMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("6696CE12-7694-4073-BC48-79EE849F2563");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OviSatelliteMap";
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
         // http://b.maptile.maps.svc.ovi.com/maptiler/v2/maptile/newest/satellite.day/12/2313/1275/256/png8

         return string.Format(UrlFormat, UrlServerLetters[GetServerNum(pos, 4)], zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://{0}.maptile.maps.svc.ovi.com/maptiler/v2/maptile/newest/satellite.day/{1}/{2}/{3}/256/png8";
   }
}