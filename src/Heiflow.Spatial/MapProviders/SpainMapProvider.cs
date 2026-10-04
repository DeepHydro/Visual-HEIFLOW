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

   /// <summary>
   /// SpainMap provider, http://sigpac.mapa.es/fega/visor/
   /// </summary>
   public class SpainMapProvider : GMapProvider
   {
      public static readonly SpainMapProvider Instance;

      SpainMapProvider()
      {
         Copyright = string.Format("©{0} SIGPAC", DateTime.Today.Year);
         MinZoom = 5;
         Area = new RectLatLng(43.8741381814747, -9.700927734375, 14.34814453125, 7.8605775962932);
      }

      static SpainMapProvider()
      {
         Instance = new SpainMapProvider();
      }

      readonly string[] levels =
      {
         "0", "1", "2", "3", "4", 
         "MTNSIGPAC", 
         "MTN2000", "MTN2000", "MTN2000", "MTN2000", "MTN2000", 
         "MTN200", "MTN200", "MTN200", 
         "MTN25", "MTN25",
         "ORTOFOTOS","ORTOFOTOS","ORTOFOTOS","ORTOFOTOS"
      };

      #region GMapProvider Members

      readonly Guid id = new Guid("7B70ABB0-1265-4D34-9442-F0788F4F689F");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "SpainMap";
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
               overlays = new GMapProvider[] { this };
            }
            return overlays;
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return MercatorProjection.Instance;
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
         return string.Format(UrlFormat, levels[zoom], zoom, pos.X, ((2 << zoom - 1) - pos.Y - 1));
      }

      static readonly string UrlFormat = "http://sigpac.mapa.es/kmlserver/raster/{0}@3785/{1}.{2}.{3}.img";
   }
}