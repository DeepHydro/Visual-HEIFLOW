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
   using System.Diagnostics;
   using System.Reflection;

   /// <summary>
   /// generic for singletons
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class Singleton<T> where T : new()
   {
      // ctor
      protected Singleton()
      {
         if(Instance != null)
         {
            throw (new Exception("You have tried to create a new singleton class where you should have instanced it. Replace your \"new class()\" with \"class.Instance\""));
         }
      }

      public static T Instance
      {
         get
         {
            if(SingletonCreator.exception != null)
            {
               throw SingletonCreator.exception;
            }
            return SingletonCreator.instance;
         }
      }

      class SingletonCreator
      {
         static SingletonCreator()
         {
            try
            {
               instance = new T();
            }
            catch(Exception ex)
            {
               if(ex.InnerException != null)
               {
                  exception = ex.InnerException;
               }
               else
               {
                  exception = ex;
               }
               Trace.WriteLine("Singleton: " + exception);
            }
         }
         internal static readonly T instance;
         internal static readonly Exception exception;
      }
   }
}
