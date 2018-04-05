// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Subsurface
{
    [DataPackageCollectionItem("Subsurface Output")]
    public class MFOutputPackage : DataPackageCollection
    {
        public static string PackageName="ModelFlow Output";
        public MFOutputPackage()
        {
            Name = "ModelFlow Output";
        }

        public override void Initialize()
        {
            foreach(var ch in Children)
            {
                ch.Initialize();
            }
            base.Initialize();
        }
        public override bool Load()
        {
            OnLoaded("");
            return true;
        }

        public override void Clear()
        {
            Children.Clear();
            base.Clear();
        }
    }
}
