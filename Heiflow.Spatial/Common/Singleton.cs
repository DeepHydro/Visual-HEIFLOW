// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

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
