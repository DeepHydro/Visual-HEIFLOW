// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace Heiflow.Spatial.Internals
{
   using System.IO;
   using System;

   /// <summary>
   /// struct for raw tile
   /// </summary>
   internal struct RawTile
   {
      public int Type;
      public GPoint Pos;
      public int Zoom;

      public RawTile(int Type, GPoint Pos, int Zoom)
      {
         this.Type = Type;
         this.Pos = Pos;
         this.Zoom = Zoom;
      }

      public override string ToString()
      {
         return Type + " at zoom " + Zoom + ", pos: " + Pos;
      }
   }
}
