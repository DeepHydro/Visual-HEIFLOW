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
   using System;
   using System.Globalization;

   /// <summary>
   /// the point of coordinates
   /// </summary>
   [Serializable]
   public struct PointLatLng
   {
      public static readonly PointLatLng Zero;
      private double lat;
      private double lng;

      public PointLatLng(double lat, double lng)
      {
         this.lat = lat;
         this.lng = lng;
      }

      public bool IsZero
      {
         get
         {
            return ((this.lng == 0d) && (this.lat == 0d));
         }
      }

      public double Lat
      {
         get
         {
            return this.lat;
         }
         set
         {
            this.lat = value;
         }
      }

      public double Lng
      {
         get
         {
            return this.lng;
         }
         set
         {
            this.lng = value;
         }
      }

      public static PointLatLng operator+(PointLatLng pt, SizeLatLng sz)
      {
         return Add(pt, sz);
      }

      public static PointLatLng operator-(PointLatLng pt, SizeLatLng sz)
      {
         return Subtract(pt, sz);
      }

      public static bool operator==(PointLatLng left, PointLatLng right)
      {
         return ((left.Lng == right.Lng) && (left.Lat == right.Lat));
      }

      public static bool operator!=(PointLatLng left, PointLatLng right)
      {
         return !(left == right);
      }

      public static PointLatLng Add(PointLatLng pt, SizeLatLng sz)
      {
         return new PointLatLng(pt.Lat - sz.HeightLat, pt.Lng + sz.WidthLng);
      }

      public static PointLatLng Subtract(PointLatLng pt, SizeLatLng sz)
      {
         return new PointLatLng(pt.Lat + sz.HeightLat, pt.Lng - sz.WidthLng);
      }

      public override bool Equals(object obj)
      {
         if(!(obj is PointLatLng))
         {
            return false;
         }
         PointLatLng tf = (PointLatLng) obj;
         return (((tf.Lng == this.Lng) && (tf.Lat == this.Lat)) && tf.GetType().Equals(base.GetType()));
      }

      public void Offset(PointLatLng pos)
      {
         this.Offset(pos.Lat, pos.Lng);
      }

      public void Offset(double lat, double lng)
      {
         this.Lng += lng;
         this.Lat -= lat;
      }

      public override int GetHashCode()
      {
         return (this.Lng.GetHashCode() ^ this.Lat.GetHashCode());
      }

      public override string ToString()
      {
         return string.Format(CultureInfo.CurrentCulture, "{{Lat={0}, Lng={1}}}", this.Lat, this.Lng);
      }

      static PointLatLng()
      {
         Zero = new PointLatLng();
      }
   }
}