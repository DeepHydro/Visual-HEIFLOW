// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial
{
   using System;
   using System.Runtime.Serialization;

   public static class Extensions
   {
      /// <summary>
      /// Retrieves a value from the SerializationInfo of the given type.
      /// </summary>
      /// <typeparam name="T">The Type that we are attempting to de-serialize.</typeparam>
      /// <param name="info">The SerializationInfo.</param>
      /// <param name="key">The key of the value we wish to retrieve.</param>
      /// <returns>The value if found, otherwise null.</returns>
      public static T GetValue<T>(SerializationInfo info, string key) where T : class
      {
         // Return the value from the SerializationInfo, casting it to type T.
         return info.GetValue(key, typeof(T)) as T;
      }

      /// <summary>
      /// Retrieves a value from the SerializationInfo of the given type.
      /// </summary>
      /// <typeparam name="T">The Type that we are attempting to de-serialize.</typeparam>
      /// <param name="info">The SerializationInfo.</param>
      /// <param name="key">The key of the value we wish to retrieve.</param>
      /// <param name="defaultValue">The default value if the de-serialized value was null.</param>
      /// <returns>The value if found, otherwise the default value.</returns>
      public static T GetValue<T>(SerializationInfo info, string key, T defaultValue) where T : class
      {
         T deserializedValue = GetValue<T>(info, key);
         if(deserializedValue != null)
         {
            return deserializedValue;
         }

         return defaultValue;
      }

      /// <summary>
      /// Retrieves a value from the SerializationInfo of the given type for structs.
      /// </summary>
      /// <typeparam name="T">The Type that we are attempting to de-serialize.</typeparam>
      /// <param name="info">The SerializationInfo.</param>
      /// <param name="key">The key of the value we wish to retrieve.</param>
      /// <param name="defaultValue">The default value if the de-serialized value was null.</param>
      /// <returns>The value if found, otherwise the default value.</returns>
      public static T GetStruct<T>(SerializationInfo info, string key, T defaultValue) where T : struct
      {
         try
         {
            return (T) info.GetValue(key, typeof(T));
         }
         catch(Exception)
         {
            return defaultValue;
         }
      }
   }
}