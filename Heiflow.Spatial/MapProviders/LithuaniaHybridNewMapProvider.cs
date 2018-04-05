// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial.MapProviders
{
   using System;

   /// <summary>
   /// LithuaniaHybridNewMap, from 2010 data, provider
   /// </summary>
   public class LithuaniaHybridNewMapProvider : LithuaniaMapProviderBase
   {
      public static readonly LithuaniaHybridNewMapProvider Instance;

      LithuaniaHybridNewMapProvider()
      {
      }

      static LithuaniaHybridNewMapProvider()
      {
         Instance = new LithuaniaHybridNewMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("35C5C685-E868-4AC7-97BE-10A9A37A81B5");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "LithuaniaHybridMap 2010";
      public override string Name
      {
         get
         {
            return name;
         }
      }

      GMapProvider[] overlays;
      public override GMapProvider[] Overlays
      {
         get
         {
            if(overlays == null)
            {
               overlays = new GMapProvider[] { LithuaniaOrtoFotoNewMapProvider.Instance, LithuaniaHybridMapProvider.Instance };
            }
            return overlays;
         }
      }

      #endregion
   }
}