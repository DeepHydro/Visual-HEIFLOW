// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Applications.Services
{
    [Export(typeof(IActiveDataService))]
    public class ActiveDataService : IActiveDataService
    {
        public ActiveDataService()
        {

        }

        public My3DMat<float> Source
        {
            get;
            set;
        }

        public My3DMat<float> SourceStatistics
        {
            get;
            set;
        }
    }
}
