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
   /// BingHybridMap provider
   /// </summary>
   public class BingHybridMapProvider : BingMapProviderBase
   {
      public static readonly BingHybridMapProvider Instance;

      BingHybridMapProvider()
      {
      }

      static BingHybridMapProvider()
      {
         Instance = new BingHybridMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("94E2FCB4-CAAC-45EA-A1F9-8147C4B14970");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "BingHybridMap";
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
         string key = TileXYToQuadKey(pos.X, pos.Y, zoom);
         return string.Format(UrlFormat, GetServerNum(pos, 4), key, Version, language, (!string.IsNullOrEmpty(ClientKey) ? "&key=" + ClientKey : string.Empty));
      }

      static readonly string UrlFormat = "http://ecn.t{0}.tiles.virtualearth.net/tiles/h{1}.jpeg?g={2}&mkt={3}{4}";
   }
}
