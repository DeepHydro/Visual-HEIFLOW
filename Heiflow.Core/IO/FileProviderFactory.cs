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

namespace Heiflow.Core.IO
{
     [Export(typeof(FileProviderFactory))]
    public class FileProviderFactory
    {

         private static List<IFileProvider> _Providers = new List<IFileProvider>();
        public static List<IFileProvider> Providers
        {
            get
            {
                return _Providers;
            }
        }

        public static IFileProvider GetProvider(string filename)
        {
            IFileProvider provider = null;
            string ext = Path.GetExtension(filename);
            var prvs = from pr in Providers where string.Equals(pr.Extension, ext, StringComparison.OrdinalIgnoreCase) select pr;
            if (prvs != null && prvs.Count() == 1)
                provider = prvs.First();
            return provider;
        }
    }
}
