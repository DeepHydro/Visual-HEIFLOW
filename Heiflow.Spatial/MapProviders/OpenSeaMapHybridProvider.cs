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
   /// OpenSeaMapHybrid provider
   /// </summary>
   public class OpenSeaMapHybridProvider : OpenStreetMapProviderBase
   {
      public static readonly OpenSeaMapHybridProvider Instance;

      OpenSeaMapHybridProvider()
      {
         RefererUrl = "http://openseamap.org/";
      }

      static OpenSeaMapHybridProvider()
      {
         Instance = new OpenSeaMapHybridProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("FAACDE73-4B90-4AE6-BB4A-ADE4F3545592");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OpenSeaMapHybrid";
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
               overlays = new GMapProvider[] { OpenStreetMapProvider.Instance, this };
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
         return string.Format(UrlFormat, zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://tiles.openseamap.org/seamark/{0}/{1}/{2}.png";
   }
}
