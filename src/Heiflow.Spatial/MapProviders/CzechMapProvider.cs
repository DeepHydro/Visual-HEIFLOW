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

   public abstract class CzechMapProviderBase : GMapProvider
   {
      public CzechMapProviderBase()
      {
         RefererUrl = "http://www.mapy.cz/";
         Area = new RectLatLng(51.2024819920053, 11.8401353319027, 7.22833716731277, 2.78312271922872);
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
            return MapyCZProjection.Instance;
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
   /// CzechMap provider, http://www.mapy.cz/
   /// </summary>
   public class CzechMapProvider : CzechMapProviderBase
   {
      public static readonly CzechMapProvider Instance;

      CzechMapProvider()
      {
      }

      static CzechMapProvider()
      {
         Instance = new CzechMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("6A1AF99A-84C6-4EF6-91A5-77B9D03257C2");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "CzechMap";
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
         // ['base','ophoto','turist','army2']  
         // http://m1.mapserver.mapy.cz/base-n/3_8000000_8000000

         int xx = pos.X << (28 - zoom);
         int yy = ((((int)Math.Pow(2.0, (double)zoom)) - 1) - pos.Y) << (28 - zoom);

         return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, xx, yy);
      }

      static readonly string UrlFormat = "http://m{0}.mapserver.mapy.cz/base-n/{1}_{2:x7}_{3:x7}";
   }
}