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
   /// OpenCycleMap provider
   /// </summary>
   public class OpenCycleMapProvider : OpenStreetMapProviderBase
   {
      public static readonly OpenCycleMapProvider Instance;

      OpenCycleMapProvider()
      {
         RefererUrl = "http://www.opencyclemap.org/";
      }

      static OpenCycleMapProvider()
      {
         Instance = new OpenCycleMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("D7E1826E-EE1E-4441-9F15-7C2DE0FE0B0A");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "OpenCycleMap";
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
         char letter = ServerLetters[GMapProvider.GetServerNum(pos, 3)];
         return string.Format(UrlFormat, letter, zoom, pos.X, pos.Y);
      }

      static readonly string UrlFormat = "http://{0}.tile.opencyclemap.org/cycle/{1}/{2}/{3}.png";
   }
}
