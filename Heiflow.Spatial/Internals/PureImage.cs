// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial
{
   using System;
   using System.IO;

   /// <summary>
   /// image abstraction proxy
   /// </summary>
   public abstract class PureImageProxy
   {
      abstract public PureImage FromStream(Stream stream);
      abstract public bool Save(Stream stream, PureImage image);

      public PureImage FromArray(byte[] data)
      {
         MemoryStream m = new MemoryStream(data, 0, data.Length, false, true);
         var pi = FromStream(m);
         if(pi != null)
         {
            m.Position = 0;
            pi.Data = m;
         }
         else
         {
            m.Dispose();
            m = null;
         }
         return pi;
      }
   }

   /// <summary>
   /// image abstraction
   /// </summary>
   public abstract class PureImage : IDisposable
   {
      public MemoryStream Data;

      #region IDisposable Members

      public abstract void Dispose();

      #endregion
   }
}
