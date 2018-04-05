// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial
{
   using System.Collections.Generic;

   /// <summary>
   /// geocoding interface
   /// </summary>
   public interface GeocodingProvider
   {
      GeoCoderStatusCode GetPoints(string keywords, out List<PointLatLng> pointList);

      PointLatLng? GetPoint(string keywords, out GeoCoderStatusCode status);

      // ...

      GeoCoderStatusCode GetPlacemarks(PointLatLng location, out List<Placemark> placemarkList);

      Placemark GetPlacemark(PointLatLng location, out GeoCoderStatusCode status);
   }
}
