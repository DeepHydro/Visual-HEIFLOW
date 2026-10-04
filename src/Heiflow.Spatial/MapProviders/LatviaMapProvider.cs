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

   public abstract class LatviaMapProviderBase : GMapProvider
   {
      public LatviaMapProviderBase()
      {
         RefererUrl = "http://www.ikarte.lv/map/default.aspx?lang=en";
         Copyright = string.Format("©{0} Hnit-Baltic - Map data ©{0} LR Valsts zemes dieniests, SIA Envirotech", DateTime.Today.Year);
         MaxZoom = 11;
         Area = new RectLatLng(58.0794870805093, 20.3286067123543, 7.90883164336887, 2.506129113082);
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
            return LKS92Projection.Instance;
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
   /// LatviaMap provider, http://www.ikarte.lv/
   /// </summary>
   public class LatviaMapProvider : LatviaMapProviderBase
   {
      public static readonly LatviaMapProvider Instance;

      LatviaMapProvider()
      {
      }

      static LatviaMapProvider()
      {
         Instance = new LatviaMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("2A21CBB1-D37C-458D-905E-05F19536EF1F");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "LatviaMap";
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
         //       http://www.maps.lt/cache/ikartelv/map/_alllayers/L03/R00000037/C00000053.png
         // http://www.maps.lt/arcgiscache/ikartelv/map/_alllayers/L02/R0000001c/C0000002a.png

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://www.maps.lt/arcgiscache/ikartelv/map/_alllayers/L{0:00}/R{1:x8}/C{2:x8}.png";
   }
}