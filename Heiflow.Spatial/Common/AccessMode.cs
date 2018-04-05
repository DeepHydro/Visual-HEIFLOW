// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial
{
   /// <summary>
   /// tile access mode
   /// </summary>
   public enum AccessMode
   {
      /// <summary>
      /// access only server
      /// </summary>
      ServerOnly,

      /// <summary>
      /// access first server and caches localy
      /// </summary>
      ServerAndCache,

      /// <summary>
      /// access only cache
      /// </summary>
      CacheOnly,
   }
}
