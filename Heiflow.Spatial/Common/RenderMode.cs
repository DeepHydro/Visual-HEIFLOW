// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726

namespace Heiflow.Spatial
{
   /// <summary>
   /// types of map rendering
   /// </summary>
   public enum RenderMode
   {
      /// <summary>
      /// gdi+ should work anywhere on Windows Forms
      /// </summary>
      GDI_PLUS,

      /// <summary>
      /// only on Windows Presentation Foundation
      /// </summary>
      WPF,
   }
}
