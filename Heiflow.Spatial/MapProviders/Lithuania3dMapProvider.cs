// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.MapProviders
{
   using System;

   /// <summary>
   /// Lithuania3dMap (2.5d) provider
   /// </summary>
   public class Lithuania3dMapProvider : LithuaniaMapProviderBase
   {
      public static readonly Lithuania3dMapProvider Instance;

      Lithuania3dMapProvider()
      {
      }

      static Lithuania3dMapProvider()
      {
         Instance = new Lithuania3dMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("CCC5B65F-C8BC-47CE-B39D-5E262E6BF083");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "Lithuania 2.5d Map";
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
         // http://dc1.maps.lt/cache/mapslt_25d_vkkp/map/_alllayers/L01/R00007194/C0000a481.png
         int z = zoom;
         if(zoom >= 10)
         {
            z -= 10;
         }

         return string.Format(UrlFormat, z, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://dc1.maps.lt/cache/mapslt_25d_vkkp/map/_alllayers/L{0:00}/R{1:x8}/C{2:x8}.png";
   }
}