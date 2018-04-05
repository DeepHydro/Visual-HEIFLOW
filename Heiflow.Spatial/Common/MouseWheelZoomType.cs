// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial
{
   /// <summary>
   /// map zooming type
   /// </summary>
   public enum MouseWheelZoomType
   {
      /// <summary>
      /// zooms map to current mouse position and makes it map center
      /// </summary>
      MousePositionAndCenter,

      /// <summary>
      /// zooms to current mouse position, but doesn't make it map center,
      /// google/bing style ;}
      /// </summary>
      MousePositionWithoutCenter,

      /// <summary>
      /// zooms map to current view center
      /// </summary>
      ViewCenter,        
   }
}
