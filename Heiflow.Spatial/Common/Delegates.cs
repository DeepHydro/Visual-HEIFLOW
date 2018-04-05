// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial
{
   using Heiflow.Spatial.MapProviders;

   public delegate void PositionChanged(PointLatLng point);

   public delegate void TileLoadComplete(long ElapsedMilliseconds);
   public delegate void TileLoadStart();
  
   public delegate void MapDrag();
   public delegate void MapZoomChanged();
   public delegate void MapTypeChanged(GMapProvider type);

   public delegate void EmptyTileError(int zoom, GPoint pos);
}
