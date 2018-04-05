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
using Heiflow.Models.Generic;
using System.ComponentModel;
using Heiflow.Core.Data;
using System.ComponentModel.Composition;

namespace Heiflow.Models.Subsurface
{
      [Export(typeof(IMFDataPackage))]
    public class MFDataPackage : DataPackage, IMFDataPackage
    {
        private static int DataIndex = 0;
        public MFDataPackage()
        {
            Name = "Data" + DataIndex;
            DataIndex++;
            _PackageInfo = new PackageInfo();
        }

        public override bool Scan()
        {
            return false;
        }

        public override bool Load(int var_index)
        {
            return false;
        }
    }
}
