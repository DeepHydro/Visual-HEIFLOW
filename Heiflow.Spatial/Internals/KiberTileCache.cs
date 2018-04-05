// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.Internals
{
   using System.Collections.Generic;
   using System.IO;
   using System;

   /// <summary>
   /// kiber speed memory cache for tiles with history support ;}
   /// </summary>
   internal class KiberTileCache : Dictionary<RawTile, byte[]>
   {
      readonly Queue<RawTile> Queue = new Queue<RawTile>();

      /// <summary>
      /// the amount of tiles in MB to keep in memmory, default: 22MB, if each ~100Kb it's ~222 tiles
      /// </summary>
#if !PocketPC
      public int MemoryCacheCapacity = 22;
#else
      public int MemoryCacheCapacity = 3;
#endif

      long memoryCacheSize = 0;

      /// <summary>
      /// current memmory cache size in MB
      /// </summary>
      public double MemoryCacheSize
      {
         get
         {
            return memoryCacheSize/1048576.0;
         }
      }

      public new void Add(RawTile key, byte[] value)
      {
         Queue.Enqueue(key);
         base.Add(key, value);

         memoryCacheSize += value.Length;
      }

      // do not allow directly removal of elements
      private new void Remove(RawTile key)
      {

      }

      internal void RemoveMemoryOverload()
      {
         while(MemoryCacheSize > MemoryCacheCapacity)
         {
            if(Keys.Count > 0 && Queue.Count > 0)
            {
               RawTile first = Queue.Dequeue();
               try
               {
                  var m = base[first];
                  {
                     base.Remove(first);
                     memoryCacheSize -= m.Length;
                  }
                  m = null;
               }
               catch
               {
               }
            }
            else
            {
               break;
            }
         }
      }
   }
}
