// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.MapProviders
{
   using System;
   using Heiflow.Spatial.Projections;

   /// <summary>
   /// OpenStreetMapSurferTerrain provider
   /// </summary>
   public class OpenStreetMapSurferTerrainProvider : OpenStreetMapProviderBase
   {
      public static readonly OpenStreetMapSurferTerrainProvider Instance;

      OpenStreetMapSurferTerrainProvider()
      {
         RefererUrl = "http://www.mapsurfer.net/";
      }

      static OpenStreetMapSurferTerrainProvider()
      {
         Instance = new OpenStreetMapSurferTerrainProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("E87954A4-1852-4B64-95FA-24E512E4B021");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OpenStreetMapSurferTerrain";
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

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         string url = MakeTileImageUrl(pos, zoom, string.Empty);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      public override string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         return string.Format(UrlFormat, pos.X, pos.Y, zoom);
      }

      static readonly string UrlFormat = "http://tiles2.mapsurfer.net/tms_t.ashx?x={0}&y={1}&z={2}";
   }
}
