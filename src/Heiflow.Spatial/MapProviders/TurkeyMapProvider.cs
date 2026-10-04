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
   /// TurkeyMap provider, http://maps.pergo.com.tr/
   /// </summary>
   public class TurkeyMapProvider : GMapProvider
   {
      public static readonly TurkeyMapProvider Instance;

      TurkeyMapProvider()
      {
         Copyright = string.Format("©{0} Pergo - Map data ©{0} Fideltus Advanced Technology", DateTime.Today.Year);
         Area = new RectLatLng(42.5830078125, 25.48828125, 19.05029296875, 6.83349609375);
      }

      static TurkeyMapProvider()
      {
         Instance = new TurkeyMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("EDE895BD-756D-4BE4-8D03-D54DD8856F1D");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "TurkeyMap";
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
         // http://{domain}/{layerName}/{zoomLevel}/{first3LetterOfTileX}/{second3LetterOfTileX}/{third3LetterOfTileX}/{first3LetterOfTileY}/{second3LetterOfTileY}/{third3LetterOfTileXY}.png

         // http://map3.pergo.com.tr/tile/00/000/000/001/000/000/000.png   
         // That means: Zoom Level: 0 TileX: 1 TileY: 0

         // http://domain/tile/14/000/019/371/000/011/825.png
         // That means: Zoom Level: 14 TileX: 19371 TileY:11825

         // updated version
         // http://map1.pergo.com.tr/publish/tile/tile9913/06/000/000/038/000/000/039.png

         string x = pos.X.ToString(Zeros).Insert(3, Slash).Insert(7, Slash); // - 000/000/001
         string y = pos.Y.ToString(Zeros).Insert(3, Slash).Insert(7, Slash); // - 000/000/000

         return string.Format(UrlFormat, GetServerNum(pos, 3), zoom, x, y);
      }

      static readonly string Zeros = "000000000";
      static readonly string Slash = "/";
      static readonly string UrlFormat = "http://map{0}.pergo.com.tr/publish/tile/tile9913/{1:00}/{2}/{3}.png";
   }
}