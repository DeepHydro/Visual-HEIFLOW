// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial
{
   /// <summary>
   /// directions interface
   /// </summary>
   interface DirectionsProvider
   {
      DirectionsStatusCode GetDirections(out GDirections direction, PointLatLng start, PointLatLng end, bool avoidHighways, bool walkingMode, bool sensor, bool metric);

      DirectionsStatusCode GetDirections(out GDirections direction, string start, string end, bool avoidHighways, bool walkingMode, bool sensor, bool metric);
   }
}
