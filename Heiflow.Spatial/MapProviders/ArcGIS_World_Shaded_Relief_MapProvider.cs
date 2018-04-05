// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.MapProviders
{
   using System;

   /// <summary>
   /// ArcGIS_World_Shaded_Relief_Map provider, http://server.arcgisonline.com
   /// </summary>
   public class ArcGIS_World_Shaded_Relief_MapProvider : ArcGISMapMercatorProviderBase
   {
      public static readonly ArcGIS_World_Shaded_Relief_MapProvider Instance;

      ArcGIS_World_Shaded_Relief_MapProvider()
      {
      }

      static ArcGIS_World_Shaded_Relief_MapProvider()
      {
         Instance = new ArcGIS_World_Shaded_Relief_MapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("2E821FEF-8EA1-458A-BC82-4F699F4DEE79");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "ArcGIS_World_Shaded_Relief_Map";
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
         // http://services.arcgisonline.com/ArcGIS/rest/services/World_Shaded_Relief/MapServer/tile/0/0/0jpg

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://server.arcgisonline.com/ArcGIS/rest/services/World_Shaded_Relief/MapServer/tile/{0}/{1}/{2}";
   }
}