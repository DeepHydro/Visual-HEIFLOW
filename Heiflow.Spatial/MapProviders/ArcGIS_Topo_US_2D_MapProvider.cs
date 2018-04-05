// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.MapProviders
{
   using System;

   /// <summary>
   /// ArcGIS_Topo_US_2D_Map provider, http://server.arcgisonline.com
   /// </summary>
   public class ArcGIS_Topo_US_2D_MapProvider : ArcGISMapPlateCarreeProviderBase
   {
      public static readonly ArcGIS_Topo_US_2D_MapProvider Instance;

      ArcGIS_Topo_US_2D_MapProvider()
      {
      }

      static ArcGIS_Topo_US_2D_MapProvider()
      {
         Instance = new ArcGIS_Topo_US_2D_MapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("7652CC72-5C92-40F5-B572-B8FEAA728F6D");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "ArcGIS_Topo_US_2D_Map";
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
         // http://server.arcgisonline.com/ArcGIS/rest/services/NGS_Topo_US_2D/MapServer/tile/4/3/15

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://server.arcgisonline.com/ArcGIS/rest/services/NGS_Topo_US_2D/MapServer/tile/{0}/{1}/{2}";
   }
}