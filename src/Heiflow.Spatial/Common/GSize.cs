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
   using System.Globalization;

   /// <summary>
   /// the size
   /// </summary>
   public struct GSize
   {
      public static readonly GSize Empty = new GSize();

      private int width;
      private int height;

      public GSize(GPoint pt)
      {
         width = pt.X;
         height = pt.Y;
      }

      public GSize(int width, int height)
      {
         this.width = width;
         this.height = height;
      }

      public static GSize operator+(GSize sz1, GSize sz2)
      {
         return Add(sz1, sz2);
      }

      public static GSize operator-(GSize sz1, GSize sz2)
      {
         return Subtract(sz1, sz2);
      }

      public static bool operator==(GSize sz1, GSize sz2)
      {
         return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
      }

      public static bool operator!=(GSize sz1, GSize sz2)
      {
         return !(sz1 == sz2);
      }

      public static explicit operator GPoint(GSize size)
      {
         return new GPoint(size.Width, size.Height);
      }

      public bool IsEmpty
      {
         get
         {
            return width == 0 && height == 0;
         }
      }

      public int Width
      {
         get
         {
            return width;
         }
         set
         {
            width = value;
         }
      }

      public int Height
      {
         get
         {
            return height;
         }
         set
         {
            height = value;
         }
      }

      public static GSize Add(GSize sz1, GSize sz2)
      {
         return new GSize(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
      }

      public static GSize Subtract(GSize sz1, GSize sz2)
      {
         return new GSize(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
      }

      public override bool Equals(object obj)
      {
         if(!(obj is GSize))
            return false;

         GSize comp = (GSize) obj;
         // Note value types can't have derived classes, so we don't need to
         //
         return (comp.width == this.width) &&
                   (comp.height == this.height);
      }

      public override int GetHashCode()
      {
         if(this.IsEmpty)
         {
            return 0;
         }
         return (Width.GetHashCode() ^ Height.GetHashCode());
      }

      public override string ToString()
      {
         return "{Width=" + width.ToString(CultureInfo.CurrentCulture) + ", Height=" + height.ToString(CultureInfo.CurrentCulture) + "}";
      }
   }
}
