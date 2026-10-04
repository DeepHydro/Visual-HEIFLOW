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
   /// GoogleChinaMap provider
   /// </summary>
   public class GoogleChinaMapProvider : GoogleMapProviderBase
   {
      public static readonly GoogleChinaMapProvider Instance;

      GoogleChinaMapProvider()
      {
         RefererUrl = string.Format("http://ditu.{0}/", ServerChina);
      }

      static GoogleChinaMapProvider()
      {
         Instance = new GoogleChinaMapProvider();
      }

      public string Version = "m@161";

      #region GMapProvider Members

      readonly Guid id = new Guid("1213F763-64EE-4AB6-A14A-D84D6BCC3426");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "GoogleChinaMap";
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
         string sec1 = string.Empty; // after &x=...
         string sec2 = string.Empty; // after &zoom=...
         GetSecureWords(pos, out sec1, out sec2);

         return string.Format(UrlFormat, UrlFormatServer, GetServerNum(pos, 4), UrlFormatRequest, Version, ChinaLanguage, pos.X, sec1, pos.Y, zoom, sec2, ServerChina);
      }

      static readonly string ChinaLanguage = "zh-CN";
      static readonly string UrlFormatServer = "mt";
      static readonly string UrlFormatRequest = "vt";
      static readonly string UrlFormat = "http://{0}{1}.{10}/{2}/lyrs={3}&hl={4}&gl=cn&x={5}{6}&y={7}&z={8}&s={9}";
   }
}