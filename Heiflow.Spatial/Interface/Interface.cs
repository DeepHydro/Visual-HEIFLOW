// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial
{
   using Heiflow.Spatial.MapProviders;

   public interface Interface
   {
      PointLatLng Position
      {
         get;
         set;
      }

      GPoint CurrentPositionGPixel
      {
         get;
      }

      string CacheLocation
      {
         get;
         set;
      }

      bool IsDragging
      {
         get;
      }

      RectLatLng CurrentViewArea
      {
         get;
      }

      GMapProvider MapProvider
      {
         get;
         set;
      }

      bool CanDragMap
      {
         get;
         set;
      }

      RenderMode RenderMode
      {
         get;
      }

      // events
      event PositionChanged OnPositionChanged;
      event TileLoadComplete OnTileLoadComplete;
      event TileLoadStart OnTileLoadStart;
      event MapDrag OnMapDrag;
      event MapZoomChanged OnMapZoomChanged;
      event MapTypeChanged OnMapTypeChanged;

      void ReloadMap();

      PointLatLng FromLocalToLatLng(int x, int y);
      GPoint FromLatLngToLocal(PointLatLng point);

#if !PocketPC
#if SQLite
      bool ShowExportDialog();
      bool ShowImportDialog();
#endif
#endif
   }
}
