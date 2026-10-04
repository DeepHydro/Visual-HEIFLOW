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
   /// YahooHybridMap provider
   /// </summary>
   public class YahooHybridMapProvider : YahooMapProviderBase
   {
      public static readonly YahooHybridMapProvider Instance;

      YahooHybridMapProvider()
      {
      }

      static YahooHybridMapProvider()
      {
         Instance = new YahooHybridMapProvider();
      }

      public string Version = "4.3";

      #region GMapProvider Members

      readonly Guid id = new Guid("A084E0DB-F9A6-45C1-BC2F-791E1F4E958E");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "YahooHybridMap";
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
               overlays = new GMapProvider[] { YahooSatelliteMapProvider.Instance, this };
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
         // http://maps1.yimg.com/hx/tl?b=1&v=4.3&t=h&.intl=en&x=14&y=5&z=7&r=1

         return string.Format(UrlFormat, ((GetServerNum(pos, 2)) + 1), Version, language, pos.X, (((1 << zoom) >> 1) - 1 - pos.Y), (zoom + 1));
      }

      static readonly string UrlFormat = "http://maps{0}.yimg.com/hx/tl?v={1}&t=h&.intl={2}&x={3}&y={4}&z={5}&r=1";
   }
}