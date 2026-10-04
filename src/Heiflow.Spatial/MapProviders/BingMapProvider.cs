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

namespace Heiflow.Spatial.MapProviders
{
   using System;
   using System.Text;
   using Heiflow.Spatial.Projections;
   using System.Diagnostics;
   using System.Net;
   using System.IO;
   using System.Text.RegularExpressions;
   using System.Threading;
   using Heiflow.Spatial.Internals;

   public abstract class BingMapProviderBase : GMapProvider
   {
      public BingMapProviderBase()
      {
         RefererUrl = "http://www.bing.com/maps/";
         Copyright = string.Format("©{0} Microsoft Corporation, ©{0} NAVTEQ, ©{0} Image courtesy of NASA", DateTime.Today.Year);
      }

      public string Version = "790";

      /// <summary>
      /// Bing Maps Customer Identification, more info here
      /// http://msdn.microsoft.com/en-us/library/bb924353.aspx
      /// </summary>
      public string ClientKey = null;

      /// <summary>
      /// Converts tile XY coordinates into a QuadKey at a specified level of detail.
      /// </summary>
      /// <param name="tileX">Tile X coordinate.</param>
      /// <param name="tileY">Tile Y coordinate.</param>
      /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
      /// to 23 (highest detail).</param>
      /// <returns>A string containing the QuadKey.</returns>
      internal string TileXYToQuadKey(int tileX, int tileY, int levelOfDetail)
      {
         StringBuilder quadKey = new StringBuilder();
         for(int i = levelOfDetail; i > 0; i--)
         {
            char digit = '0';
            int mask = 1 << (i - 1);
            if((tileX & mask) != 0)
            {
               digit++;
            }
            if((tileY & mask) != 0)
            {
               digit++;
               digit++;
            }
            quadKey.Append(digit);
         }
         return quadKey.ToString();
      }

      #region GMapProvider Members
      public override Guid Id
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override string Name
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return MercatorProjection.Instance;
         }
      }

      GMapProvider[] overlays;
      public override GMapProvider[] Overlays
      {
         get
         {
            if(overlays == null)
            {
               overlays = new GMapProvider[] { this };
            }
            return overlays;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         throw new NotImplementedException();
      }
      #endregion

      public bool TryCorrectVersion = true;
      static bool init = false;

      public override void OnInitialized()
      {
         if(!init && TryCorrectVersion)
         {
            string url = @"http://www.bing.com/maps";

            try
            {
               string html = Cache.Instance.GetContent(url, CacheType.UrlCache, TimeSpan.FromHours(8));

               if(string.IsNullOrEmpty(html))
               {
                  html = GetContentUsingHttp(url);
                  if(!string.IsNullOrEmpty(html))
                  {
                     Cache.Instance.SaveContent(url, CacheType.UrlCache, html);
                  }
               }

               if(!string.IsNullOrEmpty(html))
               {
                  #region -- match versions --
                  Regex reg = new Regex("http://ecn.t(\\d*).tiles.virtualearth.net/tiles/r(\\d*)[?*]g=(\\d*)", RegexOptions.IgnoreCase);
                  Match mat = reg.Match(html);
                  if(mat.Success)
                  {
                     GroupCollection gc = mat.Groups;
                     int count = gc.Count;
                     if(count > 2)
                     {
                        string ver = gc[3].Value;
                        string old = GMapProviders.BingMap.Version;
                        if(ver != old)
                        {
                           GMapProviders.BingMap.Version = ver;
                           GMapProviders.BingMapOld.Version = ver;
                           GMapProviders.BingSatelliteMap.Version = ver;
                           GMapProviders.BingHybridMap.Version = ver;

                           Debug.WriteLine("GMapProviders.BingMap.Version: " + ver + ", old: " + old + ", consider updating source");
                           if(Debugger.IsAttached)
                           {
                              Thread.Sleep(5555);
                           }
                        }
                        else
                        {
                           Debug.WriteLine("GMapProviders.BingMap.Version: " + ver + ", OK");
                        }
                     }
                  }
                  #endregion
               }

               init = true; // try it only once
            }
            catch(Exception ex)
            {
               Debug.WriteLine("TryCorrectBingVersions failed: " + ex.ToString());
            }
         }
      }
   }

   /// <summary>
   /// BingMapProvider provider
   /// </summary>
   public class BingMapProvider : BingMapProviderBase
   {
      public static readonly BingMapProvider Instance;

      public BingMapProvider()
      {
      }

      static BingMapProvider()
      {
         Instance = new BingMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("D0CEB371-F10A-4E12-A2C1-DF617D6674A8");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "BingMap";
      public override string Name
      {
         get
         {
            return name;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         string url = MakeTileImageUrl(pos, zoom, LanguageStr);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      public override string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         string key = TileXYToQuadKey(pos.X, pos.Y, zoom);
         return string.Format(UrlFormat, GetServerNum(pos, 4), key, Version, language, (!string.IsNullOrEmpty(ClientKey) ? "&key=" + ClientKey : string.Empty));
      }

      static readonly string UrlFormat = "http://ecn.t{0}.tiles.virtualearth.net/tiles/r{1}.png?g={2}&mkt={3}&lbl=l1&stl=h&shading=hill&n=z{4}";
   }
}
