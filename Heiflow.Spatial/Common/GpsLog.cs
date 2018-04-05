// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial
{
   using System;

   public struct GpsLog
   {
      public DateTime TimeUTC;
      public long SessionCounter;
      public double? Delta;
      public double? Speed;
      public double? SeaLevelAltitude;
      public double? EllipsoidAltitude;
      public short? SatellitesInView;
      public short? SatelliteCount;
      public PointLatLng Position;
      public double? PositionDilutionOfPrecision;
      public double? HorizontalDilutionOfPrecision;
      public double? VerticalDilutionOfPrecision;
      public FixQuality FixQuality;
      public FixType FixType;
      public FixSelection FixSelection;
   }

   public enum FixQuality : int
   {
      Unknown=0,
      Gps,
      DGps
   }
   public enum FixType : int
   {
      Unknown=0,
      XyD,
      XyzD
   }

   public enum FixSelection : int
   {
      Unknown=0,
      Auto,
      Manual
   }
}
