// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Heiflow.Models.Generic
{
    [Export(typeof(GridFileFactory))]
    public class GridFileFactory : IGridFileFactory
    {
        public GridFileFactory()
        {
 
        }

        private List<IGridFileProvider> list = new List<IGridFileProvider>();

        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IGridFileProvider> Providers
        { 
            get; 
            private set; 
        }

        public  IGridFileProvider Select(string filename)
        {
            IGridFileProvider result = null;
            var extension = Path.GetExtension(filename);
            foreach(var provider in Providers)
            {
                if(provider.Extension == extension)
                {
                    result = provider;
                    break;
                }
            }
            return result;
        }

        public void Add(IGridFileProvider provider)
        {
            if(!list.Contains(provider))
            {
                list.Add(provider);
            }
        }

        public void Composite()
        {
            Providers = list;           
        }
    }
}
