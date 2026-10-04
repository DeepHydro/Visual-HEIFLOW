//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

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
