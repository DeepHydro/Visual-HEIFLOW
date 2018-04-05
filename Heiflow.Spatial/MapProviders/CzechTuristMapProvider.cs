﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.MapProviders
{
   using System;

   /// <summary>
   /// CzechTuristMap provider, http://www.mapy.cz/
   /// </summary>
   public class CzechTuristMapProvider : CzechMapProviderBase
   {
      public static readonly CzechTuristMapProvider Instance;

      CzechTuristMapProvider()
      {
      }

      static CzechTuristMapProvider()
      {
         Instance = new CzechTuristMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("B923C81D-880C-42EB-88AB-AF8FE42B564D");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "CzechTuristMap";
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
         // http://m1.mapserver.mapy.cz/turist/3_8000000_8000000

         int xx = pos.X << (28 - zoom);
         int yy = ((((int)Math.Pow(2.0, (double)zoom)) - 1) - pos.Y) << (28 - zoom);

         return string.Format(UrlFormat, GetServerNum(pos, 3) + 1, zoom, xx, yy);
      }

      static readonly string UrlFormat = "http://m{0}.mapserver.mapy.cz/turist/{1}_{2:x7}_{3:x7}";
   }
}