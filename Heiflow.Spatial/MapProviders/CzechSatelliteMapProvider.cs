// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.MapProviders
{
   using System;

   /// <summary>
   /// CzechSatelliteMap provider, http://www.mapy.cz/
   /// </summary>
   public class CzechSatelliteMapProvider : CzechMapProviderBase
   {
      public static readonly CzechSatelliteMapProvider Instance;

      CzechSatelliteMapProvider()
      {
      }

      static CzechSatelliteMapProvider()
      {
         Instance = new CzechSatelliteMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("7846D655-5F9C-4042-8652-60B6BF629C3C");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "CzechSatelliteMap";
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
         //http://m3.mapserver.mapy.cz/ophoto/9_7a80000_7a80000

         int xx = pos.X << (28 - zoom);
         int yy = ((((int)Math.Pow(2.0, (double)zoom)) - 1) - pos.Y) << (28 - zoom);

         return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, xx, yy);
      }

      static readonly string UrlFormat = "http://m{0}.mapserver.mapy.cz/ophoto/{1}_{2:x7}_{3:x7}";
   }
}