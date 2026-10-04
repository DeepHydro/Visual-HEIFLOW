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
   /// LithuaniaHybridMap provider
   /// </summary>
   public class LithuaniaHybridMapProvider : LithuaniaMapProviderBase
   {
      public static readonly LithuaniaHybridMapProvider Instance;

      LithuaniaHybridMapProvider()
      {
      }

      static LithuaniaHybridMapProvider()
      {
         Instance = new LithuaniaHybridMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("279AB0E0-4704-4AA6-86AD-87D13B1F8975");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "LithuaniaHybridMap";
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
               overlays = new GMapProvider[] { LithuaniaOrtoFotoMapProvider.Instance, this };
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
         //http://arcgis.maps.lt/ArcGIS/rest/services/mapslt_ortofoto_overlay/MapServer/tile/0/9/13
         //return string.Format("http://arcgis.maps.lt/ArcGIS/rest/services/mapslt_ortofoto_overlay/MapServer/tile/{0}/{1}/{2}", zoom, pos.Y, pos.X);
         //http://dc1.maps.lt/cache/mapslt_ortofoto_overlay_512/map/_alllayers/L03/R0000001d/C00000029.png

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      internal static readonly string UrlFormat = "http://dc1.maps.lt/cache/mapslt_ortofoto_overlay/map/_alllayers/L{0:00}/R{1:x8}/C{2:x8}.png";
   }
}