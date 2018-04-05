using ILNumerics.Drawing.Plotting;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Controls.WinForm.Controls
{
    internal class ILSurfaceMeta
    {
        public ILSurfaceMeta()
        {

        }

        public string Name { get; set; }
        public bool Visible { get; set; }
        public ILSurface Owner { get; set; }

        public static List<ILSurfaceMeta> ToList(ILPlotCube cube)
        {
            List<ILSurfaceMeta> list = new List<ILSurfaceMeta>();
            int i=1;
           foreach(var il in cube.Children)
           {
               var meta = new ILSurfaceMeta()
               {
                   Name = "Series" + i,
                   Owner = il as ILSurface,
                   Visible = il.Visible
               };
               list.Add(meta);
               i++;
           }
            return list;
        }
    }
}
