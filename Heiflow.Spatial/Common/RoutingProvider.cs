// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial
{
   /// <summary>
   /// routing interface
   /// </summary>
   public interface RoutingProvider
   {
      /// <summary>
      /// get route between two points
      /// </summary>
      MapRoute GetRouteBetweenPoints(PointLatLng start, PointLatLng end, bool avoidHighways, bool walkingMode, int Zoom);

      /// <summary>
      /// get route between two points
      /// </summary>
      MapRoute GetRouteBetweenPoints(string start, string end, bool avoidHighways, bool walkingMode, int Zoom);
   }
}
