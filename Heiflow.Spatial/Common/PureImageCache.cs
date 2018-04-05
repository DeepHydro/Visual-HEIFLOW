// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial
{
   using System.IO;
   using System;

   /// <summary>
   /// pure abstraction for image cache
   /// </summary>
   public interface PureImageCache
   {
      /// <summary>
      /// puts image to db
      /// </summary>
      /// <param name="tile"></param>
      /// <param name="type"></param>
      /// <param name="pos"></param>
      /// <param name="zoom"></param>
      /// <returns></returns>
      bool PutImageToCache(byte[] tile, int type, GPoint pos, int zoom);

      /// <summary>
      /// gets image from db
      /// </summary>
      /// <param name="type"></param>
      /// <param name="pos"></param>
      /// <param name="zoom"></param>
      /// <returns></returns>
      PureImage GetImageFromCache(int type, GPoint pos, int zoom);

      /// <summary>
      /// gets image from db
      /// </summary>
      /// <param name="type"></param>
      /// <param name="pos"></param>
      /// <param name="zoom"></param>
      /// <returns></returns>
      MemoryStream GetImageMemoryStream(int type, GPoint pos, int zoom);

      /// <summary>
      /// delete old tiles beyond a supplied date
      /// </summary>
      /// <param name="date">Tiles older than this will be deleted.</param>
      /// <returns>The number of deleted tiles.</returns>
      int DeleteOlderThan(DateTime date);

      bool Exists(int type, GPoint pos, int zoom);
   }
}
