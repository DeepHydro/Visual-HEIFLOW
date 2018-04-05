// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.MapProviders
{
   using System;
   using Heiflow.Spatial.Projections;

   public abstract class ArcGISMapPlateCarreeProviderBase : GMapProvider
   {
      public ArcGISMapPlateCarreeProviderBase()
      {
         Copyright = string.Format("©{0} ESRI - Map data ©{0} ArcGIS", DateTime.Today.Year);
      }

      #region GMapProvider Members
      public override Guid Id
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override string Name
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return PlateCarreeProjection.Instance;
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
         throw new NotImplementedException();
      }
      #endregion
   }

   public abstract class ArcGISMapMercatorProviderBase : GMapProvider
   {
      public ArcGISMapMercatorProviderBase()
      {
         Copyright = string.Format("©{0} ESRI - Map data ©{0} ArcGIS", DateTime.Today.Year);
      }

      #region GMapProvider Members
      public override Guid Id
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override string Name
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return MercatorProjection.Instance;
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
         throw new NotImplementedException();
      }
      #endregion
   }

   /// <summary>
   /// ArcGIS_StreetMap_World_2D_Map provider, http://server.arcgisonline.com
   /// </summary>
   public class ArcGIS_StreetMap_World_2D_MapProvider : ArcGISMapPlateCarreeProviderBase
   {
      public static readonly ArcGIS_StreetMap_World_2D_MapProvider Instance;

      ArcGIS_StreetMap_World_2D_MapProvider()
      {
      }

      static ArcGIS_StreetMap_World_2D_MapProvider()
      {
         Instance = new ArcGIS_StreetMap_World_2D_MapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("00BF56D4-4B48-4939-9B11-575BBBE4A718");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "ArcGIS_StreetMap_World_2D_Map";
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
         // http://server.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer/tile/0/0/0.jpg

         return string.Format(UrlFormat, zoom, pos.Y, pos.X);
      }

      static readonly string UrlFormat = "http://server.arcgisonline.com/ArcGIS/rest/services/ESRI_StreetMap_World_2D/MapServer/tile/{0}/{1}/{2}";
   }
}