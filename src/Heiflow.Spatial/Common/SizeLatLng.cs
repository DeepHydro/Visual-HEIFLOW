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
   /// the size of coordinates
   /// </summary>
   public struct SizeLatLng
   {
      public static readonly SizeLatLng Empty;

      private double heightLat;
      private double widthLng;

      public SizeLatLng(SizeLatLng size)
      {
         this.widthLng = size.widthLng;
         this.heightLat = size.heightLat;
      }

      public SizeLatLng(PointLatLng pt)
      {
         this.heightLat = pt.Lat;
         this.widthLng = pt.Lng;
      }

      public SizeLatLng(double heightLat, double widthLng)
      {
         this.heightLat = heightLat;
         this.widthLng = widthLng;
      }

      public static SizeLatLng operator+(SizeLatLng sz1, SizeLatLng sz2)
      {
         return Add(sz1, sz2);
      }

      public static SizeLatLng operator-(SizeLatLng sz1, SizeLatLng sz2)
      {
         return Subtract(sz1, sz2);
      }

      public static bool operator==(SizeLatLng sz1, SizeLatLng sz2)
      {
         return ((sz1.WidthLng == sz2.WidthLng) && (sz1.HeightLat == sz2.HeightLat));
      }

      public static bool operator!=(SizeLatLng sz1, SizeLatLng sz2)
      {
         return !(sz1 == sz2);
      }

      public static explicit operator PointLatLng(SizeLatLng size)
      {
         return new PointLatLng(size.HeightLat, size.WidthLng);
      }

      public bool IsEmpty
      {
         get
         {
            return ((this.widthLng == 0d) && (this.heightLat == 0d));
         }
      }

      public double WidthLng
      {
         get
         {
            return this.widthLng;
         }
         set
         {
            this.widthLng = value;
         }
      }

      public double HeightLat
      {
         get
         {
            return this.heightLat;
         }
         set
         {
            this.heightLat = value;
         }
      }

      public static SizeLatLng Add(SizeLatLng sz1, SizeLatLng sz2)
      {
         return new SizeLatLng(sz1.HeightLat + sz2.HeightLat, sz1.WidthLng + sz2.WidthLng);
      }

      public static SizeLatLng Subtract(SizeLatLng sz1, SizeLatLng sz2)
      {
         return new SizeLatLng(sz1.HeightLat - sz2.HeightLat, sz1.WidthLng - sz2.WidthLng);
      }

      public override bool Equals(object obj)
      {
         if(!(obj is SizeLatLng))
         {
            return false;
         }
         SizeLatLng ef = (SizeLatLng) obj;
         return (((ef.WidthLng == this.WidthLng) && (ef.HeightLat == this.HeightLat)) && ef.GetType().Equals(base.GetType()));
      }

      public override int GetHashCode()
      {
         if(this.IsEmpty)
         {
            return 0;
         }
         return (this.WidthLng.GetHashCode() ^ this.HeightLat.GetHashCode());
      }

      public PointLatLng ToPointLatLng()
      {
         return (PointLatLng) this;
      }

      public override string ToString()
      {
         return ("{WidthLng=" + this.widthLng.ToString(CultureInfo.CurrentCulture) + ", HeightLng=" + this.heightLat.ToString(CultureInfo.CurrentCulture) + "}");
      }

      static SizeLatLng()
      {
         Empty = new SizeLatLng();
      }
   }
}
