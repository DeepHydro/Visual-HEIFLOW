// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.MapProviders
{
   using System;

   /// <summary>
   /// OviHybridMap provider
   /// </summary>
   public class OviHybridMapProvider : OviMapProviderBase
   {
      public static readonly OviHybridMapProvider Instance;

      OviHybridMapProvider()
      {
      }

      static OviHybridMapProvider()
      {
         Instance = new OviHybridMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("B85A8FD2-40F4-40EE-9B45-491AA45D86C1");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OviHybridMap";
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
         // http://c.maptile.maps.svc.ovi.com/maptiler/v2/maptile/newest/hybrid.day/12/2316/1277/256/png8

         return string.Format(UrlFormat, UrlServerLetters[GetServerNum(pos, 4)], zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://{0}.maptile.maps.svc.ovi.com/maptiler/v2/maptile/newest/hybrid.day/{1}/{2}/{3}/256/png8";
   }
}