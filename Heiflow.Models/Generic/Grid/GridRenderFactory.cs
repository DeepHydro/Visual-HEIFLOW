using Heiflow.Models.Visualization;
// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic
{
     [Export(typeof(GridFileFactory))]
    public class GridRenderFactory : IGridRenderFactory
    {
         public GridRenderFactory()
         {

         }

         private List<I3DLayerRender> list = new List<I3DLayerRender>();

         [ImportMany(AllowRecomposition = true)]
         public IEnumerable<I3DLayerRender> Renders
         {
             get;
             private set;
         }

         public I3DLayerRender Select(string rendername)
         {
             I3DLayerRender result = null;
             foreach (var provider in Renders)
             {
                 if (provider.Name == rendername)
                 {
                     result = provider;
                     break;
                 }
             }
             return result;
         }

         public void Add(I3DLayerRender render)
         {
             if (!list.Contains(render))
             {
                 list.Add(render);
             }
         }

         public void Composite()
         {
             Renders = list;
         }
    }
}
