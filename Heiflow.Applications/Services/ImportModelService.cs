// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic.Project;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Heiflow.Applications.Services
{
    [Export(typeof(IImportModelService))]
    public class ImportModelService : IImportModelService
    {

        public ImportModelService()
        {

        }

        [ImportMany]
        public IEnumerable<IImportProperty> ImportProperties
        {
            get;
            set;
        }
    }
}
