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
   using System.Collections.Generic;
   using System.Runtime.Serialization;
   using Heiflow.Spatial.MapProviders;

   /// <summary>
   /// represents route of map
   /// </summary>
   [Serializable]
#if !PocketPC
   public class MapRoute : ISerializable, IDeserializationCallback
#else
   public class MapRoute
#endif
   {
      /// <summary>
      /// points of route
      /// </summary>
      public readonly List<PointLatLng> Points;

      /// <summary>
      /// route info
      /// </summary>
      public string Name;

      /// <summary>
      /// custom object
      /// </summary>
      public object Tag;

      /// <summary>
      /// route start point
      /// </summary>
      public PointLatLng? From
      {
         get
         {
            if(Points.Count > 0)
            {
               return Points[0];
            }

            return null;
         }
      }

      /// <summary>
      /// route end point
      /// </summary>
      public PointLatLng? To
      {
         get
         {
            if(Points.Count > 1)
            {
               return Points[Points.Count - 1];
            }

            return null;
         }
      }

      public MapRoute(List<PointLatLng> points, string name)
      {
         Points = new List<PointLatLng>(points);
         Points.TrimExcess();

         Name = name;
      }

      /// <summary>
      /// route distance (in km)
      /// </summary>
      public double Distance
      {
         get
         {
            double distance = 0.0;

            if(From.HasValue && To.HasValue)
            {
               for(int i = 1; i < Points.Count; i++)
               {
                  distance += GMapProviders.EmptyProvider.Projection.GetDistance(Points[i - 1], Points[i]);
               }
            }

            return distance;
         }
      }

#if !PocketPC
      #region ISerializable Members

      // Temp store for de-serialization.
      private PointLatLng[] deserializedPoints;

      /// <summary>
      /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
      /// </summary>
      /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
      /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
      /// <exception cref="T:System.Security.SecurityException">
      /// The caller does not have the required permission.
      /// </exception>
      public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
      {
         info.AddValue("Name", this.Name);
         info.AddValue("Tag", this.Tag);
         info.AddValue("Points", this.Points.ToArray());
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="MapRoute"/> class.
      /// </summary>
      /// <param name="info">The info.</param>
      /// <param name="context">The context.</param>
      protected MapRoute(SerializationInfo info, StreamingContext context)
      {
         this.Name = info.GetString("Name");
         this.Tag = Extensions.GetValue<object>(info, "Tag", null);
         this.deserializedPoints = Extensions.GetValue<PointLatLng[]>(info, "Points");
         this.Points = new List<PointLatLng>();
      }

      #endregion

      #region IDeserializationCallback Members

      /// <summary>
      /// Runs when the entire object graph has been de-serialized.
      /// </summary>
      /// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
      public virtual void OnDeserialization(object sender)
      {
         // Accounts for the de-serialization being breadth first rather than depth first.
         Points.AddRange(deserializedPoints);
         Points.TrimExcess();
      }

      #endregion
#endif
   }
}
