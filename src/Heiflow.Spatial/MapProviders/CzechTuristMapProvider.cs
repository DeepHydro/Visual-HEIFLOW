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
   /// CzechTuristMap provider, http://www.mapy.cz/
   /// </summary>
   public class CzechTuristMapProvider : CzechMapProviderBase
   {
      public static readonly CzechTuristMapProvider Instance;

      CzechTuristMapProvider()
      {
      }

      static CzechTuristMapProvider()
      {
         Instance = new CzechTuristMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("B923C81D-880C-42EB-88AB-AF8FE42B564D");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "CzechTuristMap";
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
         // http://m1.mapserver.mapy.cz/turist/3_8000000_8000000

         int xx = pos.X << (28 - zoom);
         int yy = ((((int)Math.Pow(2.0, (double)zoom)) - 1) - pos.Y) << (28 - zoom);

         return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, xx, yy);
      }

      static readonly string UrlFormat = "http://m{0}.mapserver.mapy.cz/turist/{1}_{2:x7}_{3:x7}";
   }
}