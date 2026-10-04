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
   /// LithuaniaOrtoFotoMap provider
   /// </summary>
   public class LithuaniaOrtoFotoMapProvider : LithuaniaMapProviderBase
   {
      public static readonly LithuaniaOrtoFotoMapProvider Instance;

      LithuaniaOrtoFotoMapProvider()
      {
      }

      static LithuaniaOrtoFotoMapProvider()
      {
         Instance = new LithuaniaOrtoFotoMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("043FF9EF-612C-411F-943C-32C787A88D6A");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "LithuaniaOrtoFotoMap";
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
         // old stuff
         // http://www.maps.lt/ortofoto/mapslt_ortofoto_vector_512/map/_alllayers/L02/R0000001b/C00000028.jpg
         // http://arcgis.maps.lt/ArcGIS/rest/services/mapslt_ortofoto/MapServer/tile/0/9/13
         // return string.Format("http://www.maps.lt/ortofoto/mapslt_ortofoto_vector_512/map/_alllayers/L{0:00}/R{1:x8}/C{2:x8}.jpg", zoom, pos.Y, pos.X);
         // http://dc1.maps.lt/cache/mapslt_ortofoto_512/map/_alllayers/L03/R0000001c/C00000029.jpg
         // return string.Format("http://arcgis.maps.lt/ArcGIS/rest/services/mapslt_ortofoto/MapServer/tile/{0}/{1}/{2}", zoom, pos.Y, pos.X);
         // http://dc1.maps.lt/cache/mapslt_ortofoto_512/map/_alllayers/L03/R0000001d/C0000002a.jpg

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://dc1.maps.lt/cache/mapslt_ortofoto/map/_alllayers/L{0:00}/R{1:x8}/C{2:x8}.jpg";
   }
}