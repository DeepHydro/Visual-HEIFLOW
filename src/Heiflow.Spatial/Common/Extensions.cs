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