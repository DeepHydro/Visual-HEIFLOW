// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.MapProviders
{
   using System;

   /// <summary>
   /// ArcGIS_ShadedRelief_World_2D_Map provider, http://server.arcgisonline.com
   /// </summary>
   public class ArcGIS_ShadedRelief_World_2D_MapProvider : ArcGISMapPlateCarreeProviderBase
   {
      public static readonly ArcGIS_ShadedRelief_World_2D_MapProvider Instance;

      ArcGIS_ShadedRelief_World_2D_MapProvider()
      {
      }

      static ArcGIS_ShadedRelief_World_2D_MapProvider()
      {
         Instance = new ArcGIS_ShadedRelief_World_2D_MapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("A8995FA4-D9D8-415B-87D0-51A7E53A90D4");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "ArcGIS_ShadedRelief_World_2D_Map";
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
         // http://server.arcgisonline.com/ArcGIS/rest/services/ESRI_ShadedRelief_World_2D/MapServer/tile/1/0/1.jpg

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://server.arcgisonline.com/ArcGIS/rest/services/ESRI_ShadedRelief_World_2D/MapServer/tile/{0}/{1}/{2}";
   }
}