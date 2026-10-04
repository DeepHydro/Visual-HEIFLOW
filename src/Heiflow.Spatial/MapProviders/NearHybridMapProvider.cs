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
   /// NearHybridMap provider
   /// </summary>
   public class NearHybridMapProvider : NearMapProviderBase
   {
      public static readonly NearHybridMapProvider Instance;

      NearHybridMapProvider()
      {
      }

      static NearHybridMapProvider()
      {
         Instance = new NearHybridMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("4BF8819A-635D-4A94-8DC7-94C0E0F04BFD");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "NearHybridMap";
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
               overlays = new GMapProvider[] { NearSatelliteMapProvider.Instance, this };
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
         // http://web1.nearmap.com/maps/hl=en&x=37&y=19&z=6&nml=MapT&nmg=1&s=2KbhmZZ             
         // http://web1.nearmap.com/maps/hl=en&x=36&y=19&z=6&nml=MapT&nmg=1&s=2YKWhQi

         return string.Format(UrlFormat, GetServerNum(pos, 3), pos.X, pos.Y, zoom);
      }

      static readonly string UrlFormat = "http://web{0}.nearmap.com/maps/hl=en&x={1}&y={2}&z={3}&nml=MapT&nmg=1";
   }
}