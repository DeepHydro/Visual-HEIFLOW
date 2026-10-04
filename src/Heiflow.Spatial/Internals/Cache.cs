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
   using System;
   using System.Diagnostics;
   using System.IO;
   using System.Text;
   using Heiflow.Spatial.CacheProviders;

   /// <summary>
   /// cache system for tiles, geocoding, etc...
   /// </summary>
   public class Cache : Singleton<Cache>
   {
      /// <summary>
      /// abstract image cache
      /// </summary>
      public PureImageCache ImageCache;

      /// <summary>
      /// second level abstract image cache
      /// </summary>
      public PureImageCache ImageCacheSecond;

      string cache;

      /// <summary>
      /// local cache location
      /// </summary>
      public string CacheLocation
      {
         get
         {
            return cache;
         }
         set
         {
            cache = value;
#if SQLite
            if(ImageCache is SQLitePureImageCache)
            {
               (ImageCache as SQLitePureImageCache).CacheLocation = value;
            }
#else
            //if(ImageCache is MsSQLCePureImageCache)
            //{
            //   (ImageCache as MsSQLCePureImageCache).CacheLocation = value;
            //}
#endif
         }
      }

      public Cache()
      {
         #region singleton check
         if(Instance != null)
         {
            throw (new System.Exception("You have tried to create a new singleton class where you should have instanced it. Replace your \"new class()\" with \"class.Instance\""));
         }
         #endregion

#if SQLite
         ImageCache = new SQLitePureImageCache();
#else
         // you can use $ms stuff if you like too ;}
       //  ImageCache = new MsSQLCePureImageCache();
#endif

         if(string.IsNullOrEmpty(CacheLocation))
         {
#if PocketPC
            // use sd card if exist for cache
            string sd = Native.GetRemovableStorageDirectory();
            if(!string.IsNullOrEmpty(sd))
            {
               CacheLocation = sd + Path.DirectorySeparatorChar +  "Heiflow.Spatial" + Path.DirectorySeparatorChar;
            }
            else
#endif
            {
               string oldCache = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "Heiflow.Spatial" + Path.DirectorySeparatorChar;
#if PocketPC
               CacheLocation = oldCache;
#else
               string newCache = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "Heiflow.Spatial" + Path.DirectorySeparatorChar;

               // move database to non-roaming user directory
               if(Directory.Exists(oldCache))
               {
                  try
                  {
                     if(Directory.Exists(newCache))
                     {
                        Directory.Delete(oldCache, true);
                     }
                     else
                     {
                        Directory.Move(oldCache, newCache);
                     }
                     CacheLocation = newCache;
                  }
                  catch(Exception ex)
                  {
                     CacheLocation = oldCache;
                     Trace.WriteLine("SQLitePureImageCache, moving data: " + ex.ToString());
                  }
               }
               else
               {
                  CacheLocation = newCache;
               }
#endif
            }
         }
      }

      #region -- etc cache --

      public void SaveContent(string urlEnd, CacheType type, string content)
      {
         try
         {
            Stuff.RemoveInvalidPathSymbols(ref urlEnd);

            string dir = cache + type + Path.DirectorySeparatorChar;

            // precrete dir
            if(!Directory.Exists(dir))
            {
               Directory.CreateDirectory(dir);
            }

            string file = dir + urlEnd;

            switch(type)
            {
               case CacheType.GeocoderCache:
               file += ".geo";
               break;

               case CacheType.PlacemarkCache:
               file += ".plc";
               break;

               case CacheType.RouteCache:
               file += ".dragdir";
               break;

               case CacheType.UrlCache:
               file += ".url";
               break;

               case CacheType.DirectionsCache:
               file += ".dir";
               break;

               default:
               file += ".txt";
               break;
            }

            using(StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
            {
               writer.Write(content);
            }
         }
         catch(Exception ex)
         {
            Debug.WriteLine("SaveContent: " + ex);
         }
      }

      public string GetContent(string urlEnd, CacheType type, TimeSpan stayInCache)
      {
         string ret = null;

         try
         {
            Stuff.RemoveInvalidPathSymbols(ref urlEnd);

            string dir = cache + type + Path.DirectorySeparatorChar;
            string file = dir + urlEnd;

            switch(type)
            {
               case CacheType.GeocoderCache:
               file += ".geo";
               break;

               case CacheType.PlacemarkCache:
               file += ".plc";
               break;

               case CacheType.RouteCache:
               file += ".dragdir";
               break;

               case CacheType.UrlCache:
               file += ".url";
               break;

               default:
               file += ".txt";
               break;
            }

            if(File.Exists(file))
            {
               var writeTime = File.GetLastWriteTime(file);
               if(DateTime.Now - writeTime < stayInCache)
               {
                  using(StreamReader r = new StreamReader(file, Encoding.UTF8))
                  {
                     ret = r.ReadToEnd();
                  }
               }
            }
         }
         catch(Exception ex)
         {
            ret = null;
            Debug.WriteLine("GetContent: " + ex);
         }

         return ret;
      }

      public string GetContent(string urlEnd, CacheType type)
      {
         return GetContent(urlEnd, type, TimeSpan.FromDays(88));
      }

      #endregion
   }

   public enum CacheType
   {
      GeocoderCache,
      PlacemarkCache,
      RouteCache,
      UrlCache,
      DirectionsCache,
   }
}
