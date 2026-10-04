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
