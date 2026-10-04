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
   /// Lithuania3dMap (2.5d) provider
   /// </summary>
   public class Lithuania3dMapProvider : LithuaniaMapProviderBase
   {
      public static readonly Lithuania3dMapProvider Instance;

      Lithuania3dMapProvider()
      {
      }

      static Lithuania3dMapProvider()
      {
         Instance = new Lithuania3dMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("CCC5B65F-C8BC-47CE-B39D-5E262E6BF083");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "Lithuania 2.5d Map";
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
         // http://dc1.maps.lt/cache/mapslt_25d_vkkp/map/_alllayers/L01/R00007194/C0000a481.png
         int z = zoom;
         if(zoom >= 10)
         {
            z -= 10;
         }

         return string.Format(UrlFormat, z, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://dc1.maps.lt/cache/mapslt_25d_vkkp/map/_alllayers/L{0:00}/R{1:x8}/C{2:x8}.png";
   }
}