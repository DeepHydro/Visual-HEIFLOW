// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.Internals
{
   using System.IO;
   using System;

   /// <summary>
   /// cache queue item
   /// </summary>
   internal struct CacheQueueItem
   {
      public RawTile Tile;
      public byte[] Img;
      public CacheUsage CacheType;

      public CacheQueueItem(RawTile tile, byte[] Img, CacheUsage cacheType)
      {
         this.Tile = tile;
         this.Img = Img;
         this.CacheType = cacheType;
      }

      public override string ToString()
      {
         return Tile + ", CacheType:" + CacheType;
      }

      public void Clear()
      {
         Img = null;
      }
   }

   internal enum CacheUsage
   {
      First = 2,
      Second = 4,
      Both = First | Second
   }
}
