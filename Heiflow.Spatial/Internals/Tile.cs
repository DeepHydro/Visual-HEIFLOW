// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.Internals
{
   using System.Collections.Generic;
   using System;

   /// <summary>
   /// represent tile
   /// </summary>
   public struct Tile : IDisposable
   {
      public static readonly Tile Empty = new Tile();

      GPoint pos;
      int zoom;
      public List<PureImage> Overlays;

      public Tile(int zoom, GPoint pos)
      {
         this.zoom = zoom;
         this.pos = pos;
         this.Overlays = new List<PureImage>();
      }

      public void Clear()
      {
         lock(Overlays)
         {
            foreach(PureImage i in Overlays)
            {
               i.Dispose();
            }

            Overlays.Clear();
         }
      }

      public int Zoom
      {
         get
         {
            return zoom;
         }
         private set
         {
            zoom = value;
         }
      }

      public GPoint Pos
      {
         get
         {
            return pos;
         }
         private set
         {
            pos = value;
         }
      }

      #region IDisposable Members

      public void Dispose()
      {
         Overlays = null;
      }

      #endregion

      public static bool operator ==(Tile m1, Tile m2)
      {
         return m1.pos == m2.pos && m1.zoom == m2.zoom;
      }

      public static bool operator !=(Tile m1, Tile m2)
      {
         return !(m1 == m2);
      }

      public override bool Equals(object obj)
      {
         return base.Equals(obj);
      }

      public override int GetHashCode()
      {
         return base.GetHashCode();
      }
   }
}
