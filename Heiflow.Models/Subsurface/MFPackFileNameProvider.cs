// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Heiflow.Models.Subsurface
{
    public class MFPackFileNameProvider : IPackageFileNameProvider
    {
        private Modflow _Modflow;
        public MFPackFileNameProvider(Modflow mf)
        {
            _Modflow = mf;
        }

        public string GetFileName(string pckName)
        {
            var pp = (from pck in _Modflow.Packages where pck.Key == pckName select pck.Value).First();
            if (pp != null)
                return (pp as MFPackage).PackageInfo.FileName;
            else
                return "null";
        }
    }
}
