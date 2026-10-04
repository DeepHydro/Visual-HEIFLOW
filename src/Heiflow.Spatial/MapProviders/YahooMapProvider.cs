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

   public abstract class YahooMapProviderBase : GMapProvider
   {
      public YahooMapProviderBase()
      {
         RefererUrl = "http://maps.yahoo.com/";
         Copyright = string.Format("© Yahoo! Inc. - Map data & Imagery ©{0} NAVTEQ", DateTime.Today.Year);
      }

      #region GMapProvider Members
      public override Guid Id
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override string Name
      {
         get
         {
            throw new NotImplementedException();
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
            if(overlays == null)
            {
               overlays = new GMapProvider[] { this };
            }
            return overlays;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         throw new NotImplementedException();
      }
      #endregion
   }

   /// <summary>
   /// YahooMap provider
   /// </summary>
   public class YahooMapProvider : YahooMapProviderBase
   {
      public static readonly YahooMapProvider Instance;

      YahooMapProvider()
      {
      }

      static YahooMapProvider()
      {
         Instance = new YahooMapProvider();
      }

      public string Version = "4.3";

      #region GMapProvider Members

      readonly Guid id = new Guid("65DB032C-6869-49B0-A7FC-3AE41A26AF4D");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "YahooMap";
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
         // http://maps1.yimg.com/hx/tl?b=1&v=4.3&.intl=en&x=12&y=7&z=7&r=1

         return string.Format(UrlFormat, ((GetServerNum(pos, 2)) + 1), Version, language, pos.X, (((1 << zoom) >> 1) - 1 - pos.Y), (zoom + 1));
      }

      static readonly string UrlFormat = "http://maps{0}.yimg.com/hx/tl?v={1}&.intl={2}&x={3}&y={4}&z={5}&r=1";
   }
}