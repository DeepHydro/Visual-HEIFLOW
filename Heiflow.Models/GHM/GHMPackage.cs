// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Animation;
using Heiflow.Core.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.UI;
using Heiflow.Models.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.GHM
{
    [GHMPackageItem]
    public class GHMPackage : Package
    {
        public GHMPackage()
        {
            Name = "Master";
        }

        public GHMSerializer Serializer
        {
            get;
            set;
        }

        public MyArray<float> DataSource
        {
            get;
            protected set;
        }


        public GHModel GHModel
        {
            get;
            set;
        }


        public override bool Save(IProgress progress)
        {
            throw new NotImplementedException();
        }

        public override bool SaveAs(string filename, IProgress progress)
        {
            throw new NotImplementedException();
        }

        public override bool New()
        {
            return true;
        }

        public override void Serialize(string filename)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(string filename)
        {
            throw new NotImplementedException();
        }


        public override bool Load()
        {
            throw new NotImplementedException();
        }
    }
}
