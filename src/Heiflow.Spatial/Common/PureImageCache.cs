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
