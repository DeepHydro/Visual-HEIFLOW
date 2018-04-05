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
   /// OpenStreetMapSurfer provider
   /// </summary>
   public class OpenStreetMapSurferProvider : OpenStreetMapProviderBase
   {
      public static readonly OpenStreetMapSurferProvider Instance;

      OpenStreetMapSurferProvider()
      {
         RefererUrl = "http://www.mapsurfer.net/";
      }

      static OpenStreetMapSurferProvider()
      {
         Instance = new OpenStreetMapSurferProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("6282888B-2F01-4029-9CD8-0CFFCB043995");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OpenStreetMapSurfer";
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

      static readonly string UrlFormat = "http://tiles1.mapsurfer.net/tms_r.ashx?x={0}&y={1}&z={2}";
   }
}
