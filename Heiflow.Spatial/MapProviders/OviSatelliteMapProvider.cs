// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.MapProviders
{
   using System;

   /// <summary>
   /// OviSatelliteMap provider
   /// </summary>
   public class OviSatelliteMapProvider : OviMapProviderBase
   {
      public static readonly OviSatelliteMapProvider Instance;

      OviSatelliteMapProvider()
      {
      }

      static OviSatelliteMapProvider()
      {
         Instance = new OviSatelliteMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("6696CE12-7694-4073-BC48-79EE849F2563");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OviSatelliteMap";
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
         // http://b.maptile.maps.svc.ovi.com/maptiler/v2/maptile/newest/satellite.day/12/2313/1275/256/png8

         return string.Format(UrlFormat, UrlServerLetters[GetServerNum(pos, 4)], zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://{0}.maptile.maps.svc.ovi.com/maptiler/v2/maptile/newest/satellite.day/{1}/{2}/{3}/256/png8";
   }
}