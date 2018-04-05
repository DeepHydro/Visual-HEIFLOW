﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.MapProviders
{
   using System;

   /// <summary>
   /// ArcGIS_World_Terrain_Base_Map provider, http://server.arcgisonline.com
   /// </summary>
   public class ArcGIS_World_Terrain_Base_MapProvider : ArcGISMapMercatorProviderBase
   {
      public static readonly ArcGIS_World_Terrain_Base_MapProvider Instance;

      ArcGIS_World_Terrain_Base_MapProvider()
      {
      }

      static ArcGIS_World_Terrain_Base_MapProvider()
      {
         Instance = new ArcGIS_World_Terrain_Base_MapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("927F175B-5200-4D95-A99B-1C87C93099DA");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "ArcGIS_World_Terrain_Base_Map";
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
         // http://services.arcgisonline.com/ArcGIS/rest/services/World_Terrain_Base/MapServer/tile/0/0/0jpg

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://server.arcgisonline.com/ArcGIS/rest/services/World_Terrain_Base/MapServer/tile/{0}/{1}/{2}";
   }
}